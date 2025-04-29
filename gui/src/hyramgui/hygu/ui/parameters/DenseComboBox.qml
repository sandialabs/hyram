/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Controls
import QtQuick.Layouts
import QtQuick.Controls.Material 2.12


ComboBox {
    id: control
    implicitHeight: 36
    rightPadding: 5
    Layout.alignment: Qt.AlignCenter
    Layout.maximumWidth: 120
    // Layout.preferredWidth: 120
    textRole: "display"

    contentItem: Text {
        leftPadding: 10
        text: control.displayText
        font: control.font
        verticalAlignment: Text.AlignVCenter
        elide: Text.ElideRight
        textFormat: Text.RichText
    }

    delegate: ItemDelegate {
        width: control.width
        contentItem: Text {
            text: model[control.textRole]
            font: control.font
            elide: Text.ElideRight
            verticalAlignment: Text.AlignVCenter
            textFormat: Text.RichText
        }
        highlighted: control.highlightedIndex === index
    }
}
