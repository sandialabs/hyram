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
    property NumFormField param;
    property string tipText;
    property bool hasError: false
    property string errorMsg: "test long error msg ERROR HERE"
    property bool isReadOnly: false
    property alias label: paramLabel;
    property alias extraText: extraTextId;

    id: paramContainer

    Component.onCompleted:
    {
        refresh();
    }

    function refresh()
    {
        if (hasError)
        {
            paramContainer.Layout.preferredHeight = 80;
            paramLabel.color = color_danger;
            alertMsg.text = errorMsg;
            alertDisplay.visible = true;
        }
        else
        {
            paramContainer.Layout.preferredHeight = 40;
            paramLabel.color = color_primary;
            alertMsg.text = "";
            alertDisplay.visible = false;
        }

        unitSelector.currentIndex = param.get_unit_index();


        if (isReadOnly)
        {
            valueLabel.text.font.italic = true;
            valueInput.visible = true;

            valueInput.text = param.value;
            valueInput.readOnly = true;
            return;
        }

        valueInput.refreshLims();
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
            rows: 2
            columns: 4
            flow: GridLayout.TopToBottom

            Connections {
                target: param
                function onModelChanged() { refresh(); }
                function onUnitChanged() { refresh(); }
            }

            Item { }
            Text {
                id: paramLabel
                text: param?.label_rtf ?? ''
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

            Item {
                id: unitLabel
            }
            DenseComboBox {
                id: unitSelector
                model: param?.unit_choices ?? null
                currentIndex: param?.get_unit_index() ?? 0
                onActivated: {
                    if (param !== null) param.unit = displayText
                }

            }

            InputTopLabel {
                id: valueLabel
                text: ""
            }
            DoubleTextInput {
                id: valueInput
                field: 'value'
            }

            InputTopLabel {
                text: ""
            }
            Text {
                id: extraTextId
                Layout.leftMargin: 4
                font.pointSize: labelFontSize - 1
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
