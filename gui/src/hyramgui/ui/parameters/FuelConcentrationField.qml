/*
 * Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

 * You should have received a copy of the GNU General Public License along with HyRAM+.
 * If not, see https://www.gnu.org/licenses/.
 */
import QtQuick
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Window
import QtQuick.Controls.Material 2.12

import "../../hygu/ui/components"
import "../../hygu/ui/components/buttons"
import "../../hygu/ui/parameters"
import hygu.classes


Rectangle {
    property NumFormField param;
    property alias valueInputRef: valueInput;

    property bool doShade: false;
    property string tipText;
    property string formula;
    property bool hasError: false
    property string errorMsg: "test long error msg ERROR HERE"

    id: paramContainer
    color: doShade ? Material.color(Material.Grey, Material.Shade100) : "transparent"
    radius: 5
    Layout.preferredWidth: paramInputRow.implicitWidth
    Layout.preferredHeight: 35

    Component.onCompleted:
    {
        refresh();
    }

    function refresh()
    {
        valueInput.text = param.value;
        if (hasError)
        {
            paramContainer.Layout.preferredHeight = 60;
            paramLabel.color = color_danger;
            alertMsg.text = errorMsg;
            alertDisplay.visible = true;
        }
        else
        {
            paramContainer.Layout.preferredHeight = 35;
            paramLabel.color = color_primary;
            alertMsg.text = "";
            alertDisplay.visible = false;
        }
    }


    Row
    {
        id: paramInputRow
        anchors.fill: parent

        Component.onCompleted:
        {
            refresh();
        }


        GridLayout {
            id: paramGrid
            rows: 1
            columns: 5
            flow: GridLayout.TopToBottom

            Connections {
                target: param
                function onModelChanged() { refresh(); }
                function onUnitChanged() { refresh(); }
            }

            Text {
                id: paramLabel
                text: param?.label_rtf ?? ''
                Layout.preferredWidth: 80
                Layout.leftMargin: 20
                horizontalAlignment: Text.AlignLeft
                verticalAlignment: Text.AlignVCenter
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

            Text {
                text: formula
                Layout.preferredWidth: paramLabelWidth
                width: paramLabelWidth
                horizontalAlignment: Text.AlignHCenter
                verticalAlignment: Text.AlignVCenter
                font.pointSize: labelFontSize
                textFormat: Text.RichText
            }

            DoubleTextInput {
                id: valueInput
                field: 'value'
                tooltip.delay: 1000
            }

            Text {
                text: "%"
            }

            IconButton {
                id: allocBtn
                img: "fill-drip-solid"
                tooltip: "Allocate remaining concentration"
                tooltipRef.delay: 1000
                scale: 0.6
                topInset: 0
                bottomInset: 0
                onClicked: {
                    app_form.allocate_remaining_conc(param.label);
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
//    }
}
