/*
 * Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

 * You should have received a copy of the GNU General Public License along with HyRAM+.
 * If not, see https://www.gnu.org/licenses/.
 *
 */

import QtQuick
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Window
import QtQuick.Controls.Material

import hygu.classes
import "../../hygu/ui/parameters"
import "../../hygu/ui/components"
import "../../hygu/ui/components/buttons"

import "../parameters"


Rectangle {
    // property alias btnRef: toggleBtn;
    property bool watchChanges: true;
    property bool isAlive: true;  // indicates when component is being destroyed, which isn't instant.
    property var minRate;
    property var maxRate;
    property int index;

    id: rect
    width: container.width
    height: container.height
    radius: 5
    border.width: 1
    border.color: "#b4b4b4"
    color: "#F9F7F9"

    clip: true  // hide extra content when collapsed
    Behavior on height { NumberAnimation { duration: 200}}

    Connections {target: immedProb; function onEditingFinished() { handleDataChanged(); } }
    Connections {target: immedProb; function onActiveFocusChanged() { handleDataChanged(); } }
    Connections {target: delayProb; function onEditingFinished() { handleDataChanged(); } }
    Connections {target: delayProb; function onActiveFocusChanged() { handleDataChanged(); } }

    function getData() {
        var data = {
            "min": minRate,
            "max": maxRate,
            "immed": immedProb.val,
            "delay": delayProb.val,
        };
        return data;
    }

    function handleDataChanged()
    {
        if (watchChanges && isAlive)
        {
            setModelIgnitionData(index);
        }
    }

    function setFromModel(data, idx, hideDelete)
    {
        var watcher = watchChanges;
        watchChanges = false;

        index = idx;
        minRate = data.min;
        maxRate = data.max;

        if (minRate === null || minRate === undefined)
        {
            rate.text = "< " + maxRate;
        }
        else if (maxRate === null || maxRate === undefined)
        {
            rate.text = "≥ " + minRate;
        }
        else
        {
            rate.text = "" + minRate + " - " + maxRate;
        }
        immedProb.val = data.immed;
        delayProb.val = data.delay;

        if (hideDelete)
        {
            deleteBtn.enabled = false;
            deleteBtn.visible = false;
        }

        watchChanges = watcher;
    }

    ColumnLayout {
        id: container
        Layout.alignment: Qt.AlignCenter
        width: 600

        RowLayout {
            Layout.leftMargin: 28
            Layout.rightMargin: 8
            Layout.bottomMargin: 8
            clip: true

            GridLayout {
                // id: inputLayout
                rows: 2
                columns: 3
                flow: GridLayout.TopToBottom
                columnSpacing: 4
                rowSpacing: 4

                Text {
                    text: "Release Rate (kg/s)"
                    horizontalAlignment: Text.AlignHCenter
                    Layout.alignment: Qt.AlignVCenter | Qt.AlignHCenter
                    font.pointSize: labelFontSize - 1
                    Layout.preferredWidth: 140
                    wrapMode: Text.WordWrap
                }
                Text {
                    id: rate
                    horizontalAlignment: Text.AlignHCenter
                    Layout.alignment: Qt.AlignVCenter | Qt.AlignHCenter
                    font.pointSize: labelFontSize
                    Layout.preferredWidth: 140
                }

                Text {
                    text: "Immediate Ignition Probability"
                    horizontalAlignment: Text.AlignHCenter
                    Layout.alignment: Qt.AlignVCenter | Qt.AlignHCenter
                    font.pointSize: labelFontSize - 1
                    Layout.preferredWidth: 170
                    wrapMode: Text.WordWrap
                }
                FloatInput {
                    id: immedProb
                    w: 100
                    minValue: 0
                    maxValue: 1
                    tipText: "Enter a value between 0 and 1"
                }

                Text {
                    text: "Delayed Ignition Probability"
                    horizontalAlignment: Text.AlignHCenter
                    Layout.alignment: Qt.AlignVCenter | Qt.AlignHCenter
                    font.pointSize: labelFontSize - 1
                    Layout.preferredWidth: 170
                    wrapMode: Text.WordWrap
                }
                FloatInput {
                    id: delayProb
                    w: 100
                    minValue: 0
                    maxValue: 1
                    tipText: "Enter a value between 0 and 1"
                }
            }

            Item {
                id: spacer
                Layout.fillWidth: true
            }

            ColumnLayout {

                Item {Layout.fillHeight: true; }

                IconButton {
                    id: deleteBtn
                    width: 40
                    Layout.preferredHeight: 40
                    Layout.preferredWidth: 36
                    img: 'trash-solid'
                    tooltip: "Remove ignition"
                    bgColor: Material.color(Material.Red, Material.Shade300)
                    onClicked: function() {
                        removeIgnition(index);
                    }
                }

            }
        }
    }
}

