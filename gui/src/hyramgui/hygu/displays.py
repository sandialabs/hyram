"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""
from PySide6.QtCore import (Slot, QAbstractListModel, QModelIndex, Qt)
from ..forms.results import ResultsForm


QML_IMPORT_NAME = "hygu.classes"
QML_IMPORT_MAJOR_VERSION = 1


class QueueDisplay(QAbstractListModel):
    """Manages display of analysis queue.

    Attributes
    ----------
    item_role : int
        Qt property for item management.

    Notes
    -----
    QML reserves the terms 'data' and 'model' so don't use those here.

    """
    item_role = Qt.UserRole + 2
    _controllers: [type(ResultsForm)]
    _removedItems = {}  # {id, ac}

    _roles = {item_role: b"item"}

    def __init__(self):
        """Initialize with empty controller list. """
        super().__init__(parent=None)
        self._controllers = []

    def rowCount(self, parent=None, *args, **kwargs):
        """Returns count of controllers. """
        return len(self._controllers)

    def data(self, index, role=Qt.DisplayRole):
        """Returns controller based on index.

        Parameters
        ----------
        index : int
            Array index of selected item.
        role : int
            Qt internal role

        Returns
        -------
        AnalysisController
            Controller matching given index.

        """
        try:
            item = self._controllers[index.row()]
        except IndexError:
            return Qt.QVariant()

        if role == self.item_role:
            return item

    def roleNames(self):
        return self._roles

    @Slot(type(ResultsForm))
    def add_item(self, ac):
        """Adds new analysis item to queue. """
        self.beginInsertRows(QModelIndex(), self.rowCount(), self.rowCount())
        self._controllers.append(ac)
        self.endInsertRows()

    @Slot(int)
    def remove_item(self, idx):
        """Removes result item from queue and saves it for potential restoration. """
        self.beginRemoveRows(QModelIndex(), idx, idx)
        # save in case of restore
        removed = self._controllers.pop(idx)
        self._removedItems[removed.analysis_id] = removed
        self.endRemoveRows()

    @Slot(int)
    def restore_item(self, a_id):
        """Restores result entry to queue. """
        ac = self._removedItems.get(a_id, None)
        if ac is not None:
            self.add_item(ac)

    def handle_new_analysis(self, fm: type(ResultsForm)):
        """Tracks given ResultForm via queue. """
        self.add_item(fm)


