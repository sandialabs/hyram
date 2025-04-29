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


Item {
    property bool hasError: false;
    property string errorMsg: "test long error msg ERROR HERE";
    property string tipText;
    property alias labelRef: paramLabel;
    property alias inputRef: valueInput;

    property int value;
    property int minValue: -Infinity;
    property int maxValue: Infinity;

    id: field

    Component.onCompleted:
    {
        refresh();
    }

    function refresh()
    {
        if (hasError)
        {
            field.Layout.preferredHeight = 80;
            paramLabel.color = color_danger;
            alertMsg.text = errorMsg;
            alertDisplay.visible = true;
        }
        else
        {
            field.Layout.preferredHeight = 36;
            paramLabel.color = color_primary;
            alertMsg.text = "";
            alertDisplay.visible = false;
        }

        valueLabel.visible = true;
        valueInput.visible = true;
        valueLabel.text = "";
    }


    Row
    {
        id: paramInputRow

        Component.onCompleted:
        {
            refresh();
        }

        GridLayout {
            id: paramGrid
            rows: 1
            columns: 2

            Connections {
                // target: param
                // function onModelChanged() { refresh(); }
            }

            Text {
                id: paramLabel
                Layout.preferredWidth: paramLabelWidth
                horizontalAlignment: Text.AlignLeft
                font.pointSize: labelFontSize

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

            FloatInput {
                id: valueInput
                text: value
                Layout.maximumWidth: 120
            }
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
