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
import "../components"
import hygu.classes
import hyram.classes


Item {
    property LognormFormField param;
    property string tipText;
    property bool hasError: false
    property string errorMsg: "test long error msg ERROR HERE"
    property bool isReadOnly: false

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
            paramContainer.Layout.preferredHeight = 20;
            paramLabel.color = color_primary;
            alertMsg.text = "";
            alertDisplay.visible = false;
        }

        muInput.text = param.mu.toFixed(4);
        sigmaInput.text = param.sigma.toFixed(4);
        meanInput.text = expo(param.mean, 2);
        p5Input.text = expo(param.p5, 2);
        medianInput.text = expo(param.median, 2);
        p95Input.text = expo(param.p95, 2);
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
                function onPChanged() { refresh(); }
                function onMuChanged() { refresh(); }
                function onSigmaChanged() { refresh(); }
                function onMedianChanged() { refresh(); }
            }

            Text {
                id: paramLabel
                text: param?.label_rtf ?? ''
                Layout.preferredWidth: paramLabelWidth
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
                id: muInput
                field: 'mu'
                tooltip.text: "Enter a value"
            }
            DoubleTextInput {
                id: sigmaInput
                field: 'sigma'
            }
            ReadOnlyText {
                id: meanInput
                field: 'mean'
            }
            ReadOnlyText {
                id: p5Input
                field: 'p5'
            }
            DoubleTextInput {
                id: medianInput
                field: 'median'
            }
            ReadOnlyText {
                id: p95Input
                field: 'p95'
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
