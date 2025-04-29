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

import "../components"
import hygu.classes


Row {
    property ChoiceFormField param;
    property string tipText;
    property alias label: labelId;
    property alias selector: valueSelector;
    Behavior on opacity {NumberAnimation { duration: 100 }}

    function refresh()
    {
        valueSelector.currentIndex = param.get_index()
    }

    height: 34
    Layout.preferredHeight: 34
    width: layout.width
    Layout.preferredWidth: layout.width

    RowLayout {
        id: layout

        Connections {
            target: param
            function onModelChanged() { refresh(); }
        }

        Text {
            id: labelId
            text: param?.label ?? ''
            font.pointSize: labelFontSize
            Layout.preferredWidth: paramLabelWidth
            horizontalAlignment: Text.AlignLeft
            wrapMode: Text.WordWrap

            MouseArea {
                id: ma
                anchors.fill: parent
                hoverEnabled: true
            }

            ToolTip {
                delay: 200
                timeout: 3000
                visible: tipText ? ma.containsMouse : false
                text: tipText
            }
        }

        DenseComboBox {
            id: valueSelector
            Layout.preferredWidth: defaultSelectorW
            Layout.maximumWidth: defaultSelectorW
            model: param?.choices ?? null
            currentIndex: param?.get_index() ?? 0
            textRole: "display"
            onActivated: param.value = currentIndex
        }
    }
}
