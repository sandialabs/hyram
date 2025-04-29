/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Window
import QtQuick.Controls.Material
import Qt.labs.qmlmodels

import "../utils.js" as Utils
import "../parameters"
import "buttons"

import hygu.classes


Rectangle {
    property alias tableViewRef: tableView

    property int tableW: 400;
    property int tableH: 400;
    property int maxH: 500;
    property int headerH: 35;
    property int rowH: 25;
    property int nCols: 4;
    property int nRows: 10;
    property int colW: 100;  // Note: use tableViewRef.columnWidthProvider to customize individual column widths
    property var tableModelRef;
    property bool allowSort: true;
    // disable autosizing if cell heights vary (i.e. wordwrap is likely)
    property bool autoSizeHeight: true;

    // array of strings defining RTF column header text.
    property var headers: [];

    // array of strings defining column key accessors into dicts. Used for sorting.
    property var keys: [];

    property var columnAlignments: [];

    // function that returns table data as list of dicts. NOTE: use function to allow dynamic sorting.
    property var dataSourceCallback;

    function copySelection() {
        var copiedText = "";
        var indexes = tableView.selectionModel.selectedIndexes;
        for (let i = 0; i < indexes.length; ++i) {
            var idx = indexes[i];
            var nextIdx = i < indexes.length-1 ? indexes[i+1] : idx;
            var txt = tableView.model.data(idx, 'display');
            copiedText += txt;
            if (i === indexes.length-1) {
                // avoid adding dangling tab or newline
                break;
            }
            if (nextIdx.row !== idx.row) {
                copiedText += "\n";
            } else {
                copiedText += "\t";
            }
        }
        // don't have direct access to clipboard from QML so use TextEdit as workaround.
        clipboardAccessor.text = copiedText;
        clipboardAccessor.selectAll();
        clipboardAccessor.copy();
    }

    function pasteSelection() {
        var pastedText = "";  // TODO: load clipboard text
        var pastedRows = pastedText.split("\n");
        var startIndex = tableView.selectionModel.currentIndex;

        for (let row = 0; row < pastedRows.length; ++row) {
            var cells = pastedRows[row].split("\t");
            for (let col = 0; col < cells.length; ++col) {
                var index = model.index(startIndex.row + row, startIndex.column + col);
                model.setData(index, cells[col]);
            }
        }
    }

    Layout.topMargin: 0
    Layout.preferredWidth: tableW
    Layout.preferredHeight: tableH
    // The background color will show through the cell spacing and therefore be grid line color.
    color: "#f3f3f3"

    /**
     * Updates table and sorts by selected column index/key
     * @param key
     */
    function updateTable(key) {

        colW = tableW / nCols;
        tableModelRef.clear();

        var data = JSON.parse(dataSourceCallback());

        // sort by provided key
        if (!Utils.isNullish(key) && allowSort)
        {
            var isNumeric = !Number.isNaN(parseFloat(data[0][key]));
            if (isNumeric)
            {
                // sort by numeric column
                data.sort((a,b) => (parseFloat(a[key]) > parseFloat(b[key])) ? 1 : ((parseFloat(b[key]) > parseFloat(a[key])) ? -1 : 0));
            }
            else
            {
                data.sort((a,b) => (a[key].toString() > b[key].toString()) ? 1 : ((b[key].toString() > a[key].toString()) ? -1 : 0));
            }
        }

        nRows = data.length;
        tableModelRef.rows = data;

        if (autoSizeHeight) {
            var newH = (nRows + 1) * rowH + headerH;
            if (newH > tableH) newH = tableH;
            if (newH > maxH) newH = maxH;
            // tableView.height = newH;
            Layout.preferredHeight = newH;
        }
    }

    Component.onCompleted:
    {
        updateTable(null);
    }

    HorizontalHeaderView {
        id: dataHeader
        anchors.left: tableView.left
        anchors.top: parent.top
        width: parent.width
        syncView: tableView
        clip: true

        model: headers

        delegate: Rectangle {
            implicitWidth: colW
            implicitHeight: headerH
            color: "#fafafa"

            MouseArea {
                anchors.fill: parent
                onClicked: (mouse) => {
                    var sortKey = keys[row];  // must pass row instead of column for correct column index (Qt bug?)
                    updateTable(sortKey);
                }
            }

            Text {
                text: modelData
                anchors.centerIn: parent
                font.pointSize: labelFontSize
                font.bold: true
                wrapMode: Text.WordWrap
                width: parent.width
                horizontalAlignment: Text.AlignHCenter
                textFormat: Text.RichText
            }
        }
    }

    TableView {
        id: tableView
        height: tableH - headerH
        width: parent.width
        anchors.top: dataHeader.bottom
        columnSpacing: 1
        rowSpacing: 1
        clip: true
        interactive: false
        boundsBehavior: Flickable.StopAtBounds
        selectionBehavior: TableView.SelectCells
        selectionMode: TableView.ContiguousSelection
        selectionModel: ItemSelectionModel { model: tableView.model }

        MouseArea
        {
            id: mousearea
            anchors.fill: parent
            //When flickable is flicking and interactive is true,
            //this will have no effect until current flicking ends and interactive set to false
            //so we need to keep interactive false and scroll only with flick()
            preventStealing: true

            onWheel: function(wheel)
                {
                    wheel.accepted = true
                    tableView.flick(0, 15 * wheel.angleDelta.y)
                }

            //make sure items can only be clicked when view is not scrolling
            onPressed: function(mouse) { mouse.accepted = tableView.moving}
            onReleased: function(mouse) { mouse.accepted = tableView.moving}
            onClicked: function(mouse) { mouse.accepted = tableView.moving}
        }


        // Copy selected data to clipboard
        // Shortcut {
        //     // for Mac; doesn't always work on Windows?
        //     sequences: [StandardKey.Copy, "Ctrl+C"]
        //     onActivated: {
        //         console.log("Shortcut copy");
        //         if (tableView.selectionModel.hasSelection) {
        //             copySelection();
        //         }
        //     }
        // }

        focus: true

        Keys.onPressed: (event) => {
            // Ctrl-C; on Mac, Cmd is ControlModifier and Ctrl is MetaModifier
            if (((event.key == Qt.Key_C) && (event.modifiers & Qt.ControlModifier))
            ) {
                if (tableView.selectionModel.hasSelection) {
                    copySelection();
                }
            }
        }

        delegate: Rectangle {
            implicitWidth: colW
            // dynamically increase cell row height to match contents, with some padding
            implicitHeight: textElement.contentHeight+8 > rowH ? textElement.contentHeight+8 : rowH
            height: textElement.contentHeight + 8
            color: selected ? Material.color(Material.Blue, Material.Shade50) : "white"

            required property bool selected

            Text {
                id: textElement
                text: display
                anchors.fill: parent
                font.pointSize: labelFontSize
                wrapMode: Text.WordWrap
                verticalAlignment: Text.AlignVCenter
                horizontalAlignment: {
                    if (columnAlignments.length === 0 || columnAlignments.length < column) {
                        return Text.AlignHCenter;
                    }
                    else {
                        return columnAlignments[column];
                    }
                }
                leftPadding: 4
                rightPadding: 4
            }

            SelectionRectangle {
                target: tableView
                topLeftHandle: Rectangle {
                    width: 12
                    height: 12
                    color: Material.color(Material.Blue, Material.Shade100)
                    visible: SelectionRectangle.control.active
                }
                bottomRightHandle: Rectangle {
                    width: 12
                    height: 12
                    color: Material.color(Material.Blue, Material.Shade100)
                    visible: SelectionRectangle.control.active
                }
            }
        }
    }
    TextEdit {
        id: clipboardAccessor
        textFormat: TextEdit.PlainText
        visible: false
    }
}
