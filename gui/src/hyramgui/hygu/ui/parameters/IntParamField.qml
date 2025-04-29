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
import "../components"


Rectangle {
    property IntFormField param;
    property bool hasError: false;
    property bool allowNull: false;
    property string errorMsg: "test long error msg ERROR HERE";
    property string tipText: "";
    property alias label: paramLabel;
    property alias input: valueInput;
    property int minValue: param.min_value !== null ? parseInt(param.min_value) : 0
    property int maxValue: param.max_value !== null ? parseInt(param.max_value) : 2e9

    id: container
    color: "transparent";
    width: paramInputRow.width
    Behavior on opacity {NumberAnimation { duration: 100 }}

    Component.onCompleted:
    {
        refresh();
    }

    function refresh()
    {
        if (allowNull && param.is_null)
        {
            valueInput.text = "";
        }
        else
        {
            valueInput.text = param.value;
        }

        if (hasError)
        {
            container.Layout.preferredHeight = 60;
            paramLabel.color = color_danger;
            alertMsg.text = errorMsg;
            alertDisplay.visible = true;
        }
        else
        {
            container.Layout.preferredHeight = 30;
            paramLabel.color = color_primary;
            alertMsg.text = "";
            alertDisplay.visible = false;
        }
    }


    RowLayout
    {
        id: paramInputRow
        anchors.verticalCenter: parent.verticalCenter

        Component.onCompleted:
        {
            refresh();
        }

        Connections {
            target: param
            function onModelChanged() { refresh(); }
        }

        Text {
            id: paramLabel
            text: param?.label ?? ''
            Layout.preferredWidth: paramLabelWidth
            horizontalAlignment: Text.AlignLeft
            font.pointSize: labelFontSize
            textFormat: Text.RichText
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

        IntNullableParamInput {
            id: valueInput
            allowNullVal: allowNull
            Layout.preferredWidth: defaultInputW
            Layout.maximumWidth: defaultInputW
            min: minValue
            max: maxValue
            tooltip.text: param?.value_tooltip ?? "Enter a value between " + minValue + " and " + maxValue
        }
    }

    Row
    {
        id: alertDisplay
        anchors.top: paramInputRow.bottom
        leftPadding: 125

        AppIcon {
            id: alertIcon
            source: 'circle-exclamation-solid'
            iconColor: Material.color(Material.Red)
            width: 24
            height: 24
        }
        Text {
            id: alertMsg
            text: ""
            anchors.topMargin: 4
            font.italic: true
            anchors.verticalCenter: parent.verticalCenter
            color: color_danger
        }
    }
}
