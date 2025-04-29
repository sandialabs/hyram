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

import hygu.classes


Row {
    property BoolFormField param;
    property alias label: paramLabel;
    property alias input: valueInput;
    // property string tipText;  mouseArea capture breaks checkbox

    function refresh()
    {
        valueInput.checked = param.value
    }

    function checked()
    {
        return param.value === true;
    }

    Component.onCompleted: { }

    height: 30
    Layout.preferredHeight: 30

    RowLayout {

        Connections {
            target: param
            function onModelChanged() { refresh(); }
        }

        Item {
            id: paramLabel
            Layout.preferredWidth: paramLabelWidth
        }

        CheckBox {
            id: valueInput
            implicitHeight: 30
            checked: param?.value ?? true
            text: param?.label ?? ''
            font.pointSize: labelFontSize
            onToggled:  param.value = checked
            leftPadding: 0
        }

        Item {}
    }
}
