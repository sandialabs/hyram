/*
 * Copyright 2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Window
import QtQuick.Controls.Material 2.12

import "../components"
import hygu.classes


Item {
    property NumFormField param1;
    property NumFormField param2;
    property string labelText;
    property string tipText;
    property bool hasError: false
    property string errorMsg: "test long error msg ERROR HERE"
    property bool isReadOnly: false

    id: param1Container

    Component.onCompleted:
    {
        refresh();
    }

    function refresh()
    {
        if (hasError)
        {
            param1Container.Layout.preferredHeight = 80;
            paramLabel.color = color_danger;
            alertMsg.text = errorMsg;
            alertDisplay.visible = true;
        }
        else
        {
            param1Container.Layout.preferredHeight = 40;
            paramLabel.color = color_primary;
            alertMsg.text = "";
            alertDisplay.visible = false;
        }

        unitSelector.currentIndex = param1.get_unit_index();


        if (isReadOnly)
        {
            value1Label.text.font.italic = true;
            value1Input.visible = true;

            value1Input.text = param1.value;
            value1Input.readOnly = true;
            return;
        }

        value1Input.refreshLims();
    }


    Row
    {
        id: paramInputRow

        Component.onCompleted:
        {
            refresh();
        }


        RowLayout {
            id: paramGrid

            Connections {
                target: param1
                function onModelChanged() { refresh(); }
                function onUnitChanged() { refresh(); }
            }

            Text {
                id: paramLabel
                text: labelText
                Layout.preferredWidth: paramLabelWidth
                width: paramLabelWidth
                horizontalAlignment: Text.AlignLeft
                font.pointSize: labelFontSize
                textFormat: Text.RichText

                ToolTip {
                    delay: 200
                    timeout: 3000
                    visible: tipText ? ma.containsMouse : false
                    text: tipText
                }

                // for tooltip hover
                MouseArea {
                    id: ma
                    anchors.fill: parent
                    hoverEnabled: true
                }
            }

            DoubleTextInput {
                id: value1Input
                field: 'value'
                paramRef: param1
            }

            Text {text: "to"; }

            DoubleTextInput {
                id: value2Input
                field: 'value'
                paramRef: param2
            }

            DenseComboBox {
                id: unitSelector
                model: param1?.unit_choices ?? null
                currentIndex: param1?.get_unit_index() ?? 0
                onActivated: {
                    if (param1 !== null) param1.unit = displayText
                }
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
