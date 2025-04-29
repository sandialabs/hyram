/*
 * Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

 * You should have received a copy of the GNU General Public License along with HyRAM+.
 * If not, see https://www.gnu.org/licenses/.
 *
 */

import QtQuick
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Window
import QtQuick.Controls.Material


ComboBox {
    property int w: 125;
    // property alias val: currentText;

    Layout.preferredWidth: w
    model: ListModel {
        id: model
        ListElement { text: "Normal" }
        ListElement { text: "Uniform" }
        ListElement { text: "Constant" }
    }
    implicitHeight: 36
    // highlighted: control.highlightedIndex === index
}

