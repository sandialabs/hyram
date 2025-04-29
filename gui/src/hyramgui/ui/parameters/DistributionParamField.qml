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

import "../../hygu/ui/utils.js" as Utils
import "../../hygu/ui/components"
import "../../hygu/ui/components/buttons"
import "../../hygu/ui/parameters"
import "../components"
import hygu.classes
import hyram.classes


Item {
    property DistributionFormField param;
    property string tipText;
    property bool hasError: false
    property string errorMsg: "test long error msg ERROR HERE"
    property bool isReadOnly: false
    property var distrs: ["beta", "ln", "ev"]
    property var distrIndexes: {"beta": 0, "ln": 1, "ev": 2}

    id: paramContainer

    Component.onCompleted:
    {
        refresh();
    }

    function refresh() {
        if (hasError) {
            paramContainer.Layout.preferredHeight = 60;
            paramLabel.color = color_danger;
            alertMsg.text = errorMsg;
            alertDisplay.visible = true;
        } else {
            paramContainer.Layout.preferredHeight = 30;
            paramLabel.color = color_primary;
            alertMsg.text = "";
            alertDisplay.visible = false;
        }

        if (Utils.isNullish(param)) return;

        distrSelector.currentIndex = distrIndexes[param.distr];
        paInput.text = param.pa;
        pbInput.text = param.pb;
    }

    Row
    {
        id: paramInputRow

        Component.onCompleted:
        {
            refresh();
        }

        RowLayout
        {
            id: paramGrid

            Connections {
                target: param
                function onModelChanged() { refresh(); }
                function onDistrChanged() { refresh(); }
                function onPChanged() { refresh(); }
            }

            Text {
                id: paramLabel
                text: param?.label_rtf ?? ''
                Layout.preferredWidth: paramLabelWidth + 100
                horizontalAlignment: Text.AlignLeft
                font.pointSize: labelFontSize
                textFormat: Text.RichText
                wrapMode: Text.WordWrap

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

            ComboBox {
                id: distrSelector
                Layout.preferredWidth: 145
                model: ListModel {
                    id: model
                    ListElement { text: "Beta" }
                    ListElement { text: "LogNormal" }
                    ListElement { text: "Expected value" }
                }
                implicitHeight: 36
                currentIndex: if (param !== null) distrIndexes[param.distr]
                onActivated: {
                    if (param !== null) param.distr = distrs[currentIndex];
                }
            }

            DoubleTextInput {
                id: paInput
                field: 'pa'
                tooltip.text: "Enter a value"
            }
            DoubleTextInput {
                id: pbInput
                field: 'pb'
                visible: distrSelector.currentIndex === 0 || distrSelector.currentIndex === 1
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
