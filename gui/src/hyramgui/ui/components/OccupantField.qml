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
import QtQuick.Controls.Material

import hygu.classes
import "../../hygu/ui/parameters"
import "../../hygu/ui/components"
import "../../hygu/ui/components/buttons"

import "../parameters"


Rectangle {
    property alias btnRef: toggleBtn;
    property bool watchChanges: true;
    property bool isAlive: true;  // indicates when component is being destroyed, which isn't instant.

    id: rect
    width: container.width
    height: container.height
    radius: 5
    border.width: 1
    border.color: "#b4b4b4"
    color: "#F9F7F9"

    clip: true  // hide extra content when collapsed
    Behavior on height { NumberAnimation { duration: 200}}

    Connections {target: nOccupantsInput; function onValChanged() { handleDataChanged(); } }
    Connections {target: occupantDescrip; function onEditingFinished() { handleDataChanged(); } }
    Connections {target: occupantDescrip; function onActiveFocusChanged() { handleDataChanged(); } }
    Connections {target: hours; function onValChanged() { handleDataChanged(); } }
    Connections {target: units; function onActivated() { handleDataChanged(); } }
    Connections {target: xd; function onActivated() { handleDataChanged(); } }
    Connections {target: xa; function onValChanged() { handleDataChanged(); } }
    Connections {target: xb; function onValChanged() { handleDataChanged(); } }
    Connections {target: yd; function onActivated() { handleDataChanged(); } }
    Connections {target: ya; function onValChanged() { handleDataChanged(); } }
    Connections {target: yb; function onValChanged() { handleDataChanged(); } }
    Connections {target: zd; function onActivated() { handleDataChanged(); } }
    Connections {target: za; function onValChanged() { handleDataChanged(); } }
    Connections {target: zb; function onValChanged() { handleDataChanged(); } }

    function getData() {
        var data = {
            "occupants": nOccupantsInput.val,
            "descrip": occupantDescrip.text,
            "hours": hours.val,
            "units": units.currentIndex,
            "xd": xd.currentIndex,
            "xa": xa.val,
            "xb": xb.val,

            "yd": yd.currentIndex,
            "ya": ya.val,
            "yb": yb.val,

            "zd": zd.currentIndex,
            "za": za.val,
            "zb": zb.val,
        };
        return data;
    }

    function handleDataChanged()
    {
        // TODO: convert to event-driven
        if (watchChanges)
        {
            setModelOccupantData();
            refreshDisplay();
        }
    }

    function refreshDisplay()
    {
        xb.enabled = xd.currentIndex !== 2;
        yb.enabled = yd.currentIndex !== 2;
        zb.enabled = zd.currentIndex !== 2;
    }

    function setFromModel(data)
    {
        var watcher = watchChanges;
        watchChanges = false;

        nOccupantsInput.val = data.occupants;
        occupantDescrip.text = data.descrip;
        hours.val = data.hours;
        units.currentIndex = data.units;
        xd.currentIndex = data.xd;
        xa.val = data.xa;
        xb.val = data.xb;
        yd.currentIndex = data.yd;
        ya.val = data.ya;
        yb.val = data.yb;
        zd.currentIndex = data.zd;
        za.val = data.za;
        zb.val = data.zb;

        watchChanges = watcher;
        refreshDisplay();
    }

    function toggleDisplay()
    {
        if (rect.height > 100) {
            // closing
            toggleBtn.img = 'chevron-left-solid';
            rect.height = 48;
        } else {
            toggleBtn.img = 'chevron-down-solid';
            rect.height = 195;
        }
    }

    Component.onCompleted: {
        refreshDisplay();
    }

    ColumnLayout {
        id: container
        Layout.alignment: Qt.AlignCenter
        width: 740

        // header row
        RowLayout {
            Layout.topMargin: 0
            Layout.leftMargin: 8
            Layout.rightMargin: 8
            Layout.minimumHeight: 45

            InputLabel {
                text: "Occupants"
                w: 90
            }

            IntInput {
                id: nOccupantsInput
                w: 103
                minValue: 1
                val: 1
                Layout.rightMargin: 0
                tooltip.text: "Enter a number of occupants for this group"
            }

            StringInput {
                id: occupantDescrip
                horizontalAlignment: Text.AlignLeft
                Layout.leftMargin: 0
                w: 272
                Layout.preferredWidth: 272
                text: "Station workers"
                tooltip.text: "(Optional) Enter a description of the occupants"
            }

            FloatInput {
                w: 103;
                id: hours;
                val: 2000
                tipText: "Enter a positive number representing exposed hours per occupant, per year, for each occupant in the group"
            }
            Text {
                text: "Exposed hours / occupant / year"
                font.italic: true
                wrapMode: Text.WordWrap
                Layout.preferredWidth: 95
            }

            IconButton {
                id: toggleBtn
                implicitHeight: 34
                implicitWidth: 32
                Layout.alignment: Qt.AlignVCenter
                img: 'chevron-down-solid'
                tooltip: "Toggle"
                bgColor: "white"

                onClicked: function() { toggleDisplay();}
            }
        }

        RowLayout {
            id: extraInputSection
            Layout.leftMargin: 28
            Layout.rightMargin: 8
            Layout.bottomMargin: 8
            clip: true

            GridLayout {
                id: distrInputs
                rows: 4
                columns: 5
                flow: GridLayout.LeftToRight
                columnSpacing: 4
                rowSpacing: 4

                Text {}
                Text {
                    text: "Distribution"
                    Layout.alignment: Qt.AlignVCenter | Qt.AlignHCenter
                    font.pointSize: labelFontSize
                    Layout.margins: 0
                }
                Text {
                    text: "Parameter A"
                    Layout.alignment: Qt.AlignVCenter | Qt.AlignHCenter
                    font.pointSize: labelFontSize
                    Layout.margins: 0
                }
                Text {
                    text: "Parameter B"
                    Layout.alignment: Qt.AlignVCenter | Qt.AlignHCenter
                    font.pointSize: labelFontSize
                    Layout.margins: 0
                }

                Text {
                    text: "Units"
                    Layout.alignment: Qt.AlignVCenter | Qt.AlignHCenter
                    font.pointSize: labelFontSize
                    Layout.margins: 0
                }


                Text {
                    text: "X (length)"
                    Layout.margins: 0
                    Layout.preferredWidth: 70
                }
                DistrComboBox {w: 105; id: xd; currentIndex: 1}
                FloatInput {
                    w: 80
                    id: xa
                    val: 1
                    tipText: "Enter mean (Normal distribution), closest value (Uniform), or value to use (Constant)."
                }
                FloatInput {
                    w: 80
                    id: xb
                    val: 20
                    tipText: "Enter std deviation (Normal distribution) or furthest value (Uniform). Unused in Constant distribution."
                }

                ComboBox {
                    id: units
                    Layout.preferredWidth: 105
                    model: ListModel {
                        id: model
                        ListElement { text: "Meter" }
                        ListElement { text: "Inch" }
                        ListElement { text: "Foot" }
                        ListElement { text: "Yard" }
                    }
                    implicitHeight: 36
                    currentIndex: 0
                }

                Text {
                    text: "Y (height)"
                }
                DistrComboBox {w: 105; id: yd; currentIndex: 2}
                FloatInput {
                    w: 80
                    id: ya
                    val: 0
                    tipText: "Enter mean (Normal distribution), closest value (Uniform), or value to use (Constant)."
                    // tipText: "For Normal distribution, A is the mean. For Uniform distribution, A is the closest value. For Constant, A is the value to be used."
                }
                FloatInput {
                    w: 80
                    id: yb
                    val: 0
                    tipText: "Enter std deviation (Normal distribution) or furthest value (Uniform). Unused in Constant distribution."
                    // tipText: "For Normal distribution, B is the std deviation. For Uniform distribution, B is the furthest value. For Constant, B is not used."
                }

                Text { text: units.currentText;  Layout.leftMargin: 10;  font.italic: true }

                Text {
                    text: "Z (width)"
                }
                DistrComboBox {w: 105; id: zd; currentIndex: 1}
                FloatInput {
                    w: 80
                    id: za
                    val: 1
                    tipText: "Enter mean (Normal distribution), closest value (Uniform), or value to use (Constant)."
                }
                FloatInput {
                    w: 80
                    id: zb
                    val: 12
                    tipText: "Enter std deviation (Normal distribution) or furthest value (Uniform). Unused in Constant distribution."
                }

                Text { text: units.currentText;  Layout.leftMargin: 10;  font.italic: true }
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
                    tooltip: "Remove occupant group"
                    bgColor: Material.color(Material.Red, Material.Shade300)
                    onClicked: function() {
                        removeOccupantGroup(rect);
                    }
                }

            }
        }
    }
}

