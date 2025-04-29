/*
 * Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

 * You should have received a copy of the GNU General Public License along with HyRAM+.
 * If not, see https://www.gnu.org/licenses/.
 */
import QtQuick 2.12
import QtQuick.Layouts
import QtQuick.Controls 2.12
import QtQuick.Dialogs
import QtQuick.Window
import QtQuick.Controls.Material 2.12

import "../../hygu/ui/components"
import "../../hygu/ui/components/buttons"
import "../../hygu/ui/parameters"
import "../../hygu/ui/pages"
import "../components"
import "../parameters"
import hyram.classes


FormPage {
    property int labelW: 225;

    function refreshForm()
    {
        let doPresLines = doPresLinesInput.checked();
        linePsInput.enabled = doPresLines;

        let doPresTimes = doPresTimesInput.checked();
        pairPsInput.enabled = doPresTimes;
        pairTsInput.enabled = doPresTimes;
    }

    Connections {
        target: app_form

        function onAlertChanged(genMsg, genLevel) {
            updateAlerts();
        }
    }

    function updateAlerts() {
        alert.level = 1;
        alert.msg = "";
        // Get shared-state and phys ValidationResponses separately
        var resp = app_form.check_valid_shared_state();
        alert.level = parseInt(resp[0]);
        alert.msg = "" + resp[1];

        if (alert.level === 1)
        {
            resp = app_form.check_valid_accum();
            alert.level = parseInt(resp[0]);
            alert.msg = "" + resp[1];
        }
    }


    Component.onCompleted:
    {
        refreshForm();
    }

    ColumnLayout {
        width: parent.width
        anchors.fill: parent

        // Form header
        Text {
            font.pointSize: header1FontSize
            id: formHeader
            font.weight: 600
            Layout.leftMargin: 14
            text: "Accumulation"
        }

        Rectangle {
            height: 2
            Layout.fillWidth: true
            Layout.leftMargin: 16
            Layout.rightMargin: 4
            color: Material.color(Material.Blue, Material.Shade400)
        }
        RowLayout {
            Layout.leftMargin: 14

            Text {
                font.pointSize: contentFontSize + 1
                id: formDescrip
                font.italic: true
                text: "Calculate overpressure and layering (accumulation) behavior for gaseous hydrogen in an enclosure"
            }

            ButtonPopup {
                id: infoPopup
                h: infoCol.height + 30
                tipText: "View illustration of indoor release geometry"

                ColumnLayout {
                    id: infoCol
                    parent: infoPopup.contentRef
                    width: parent.width - 20
                    spacing: 20
                    SimImage {
                        filename: appDir + "ui/resources/geometry_of_indoor_release.png";
                        source: appDir + "ui/resources/geometry_of_indoor_release.png";
                        height: 400
                        sourceSize.height: height
                    }

                    Text {
                        text: "Enclosure parameters for accumulation analysis. See Section 3.3.4 of the HyRAM+ Technical Reference Manual for more information."
                        Layout.preferredWidth: 700
                        font.italic: true
                        font.pointSize: 14
                        wrapMode: Text.WordWrap
                    }
                }
            }

        }
        Item {
            id: inputSpacer2
            Layout.preferredHeight: 4
        }

        FlickableForm
        {
            contentHeight: paramCols.height + 20 + 120

            // ==========================
            // ==== Parameter Inputs ====
            Item {
                id: paramColContainer
                Layout.fillHeight: true
                height: paramCols.height

                ColumnLayout {
                    id: paramCols

                    GridLayout {
                        id: releaseRow
                        columns: 3
                        rows: 2
                        flow: GridLayout.TopToBottom
                        Layout.preferredWidth: 700
                        Layout.preferredHeight: 60

                        Text {
                            text: "Release type"
                            font.pointSize: labelFontSize
                            topPadding: 6
                            Layout.topMargin: 0
                            Layout.preferredHeight: 30
                            Layout.preferredWidth: labelW
                            Layout.alignment: Qt.AlignVCenter
                        }
                        Item {}

                        RadioButton {
                            id: isBlowdown
                            checked: is_blowdown_c.value === true
                            text: "Blowdown"
                            font.pointSize: labelFontSize
                            Layout.maximumHeight: 30
                            Layout.alignment: Qt.AlignLeft
                            onClicked: { is_blowdown_c.value = true; }
                        }

                        RadioButton {
                            checked: is_blowdown_c.value === false
                            text: "Steady"
                            font.pointSize: labelFontSize
                            Layout.maximumHeight: 30
                            Layout.alignment: Qt.AlignLeft
                            onClicked: { is_blowdown_c.value = false; }
                        }

                        Item {
                            Layout.fillWidth: true
                            Layout.preferredHeight: 30
                        }
                        Item {
                            Layout.fillWidth: true
                            Layout.preferredHeight: 30
                        }
                    }

                    FloatParamField {
                        param: leak_d_c
                        label.Layout.preferredWidth: labelW
                    }

                    FloatParamField {
                        param: rel_h_c
                        label.Layout.preferredWidth: labelW
                    }

                    FloatParamField {
                        param: enclosure_h_c
                        label.Layout.preferredWidth: labelW
                    }

                    FloatParamField {
                        param: floor_ceil_a_c
                        label.Layout.preferredWidth: labelW
                    }

                    FloatParamField {
                        param: rel_wall_dist_c
                        label.Layout.preferredWidth: labelW
                    }

                    FloatParamField {
                        param: ceil_xarea_c
                        label.Layout.preferredWidth: labelW
                    }

                    FloatParamField {
                        param: ceil_h_c
                        label.Layout.preferredWidth: labelW
                    }

                    FloatParamField {
                        param: floor_xarea_c
                        label.Layout.preferredWidth: labelW
                    }

                    FloatParamField {
                        param: floor_h_c
                        label.Layout.preferredWidth: labelW
                    }

                    FloatParamField {
                        param: tank_v_c
                        enabled: is_blowdown_c.value
                        opacity: is_blowdown_c.value ? 1 : fadeVal
                        label.Layout.preferredWidth: labelW
                    }

                    FloatParamField {
                        param: vent_flow_c
                        label.Layout.preferredWidth: labelW
                        extraText.text: "m<sup>3</sup>/s"
                    }

                    FormSectionHeader2 {title: "Output Options" }

                    NumListParamField {
                        param: out_ts_c
                        label.Layout.preferredWidth: labelW
                        inputLength: 500
                        tipText: "Separate values with a space (' ')"
                    }

                    FloatParamField {
                        param: t_max_c
                        label.Layout.preferredWidth: labelW
                    }

                    BoolParamField {
                        id: doPresLinesInput
                        param: do_p_lines_c
                        Layout.topMargin: 10
                        label.Layout.preferredWidth: labelW
                    }
                    NumListParamField {
                        param: line_ps_c
                        id: linePsInput
                        label.Layout.preferredWidth: labelW
                        inputLength: 300
                        tipText: "Separate values with a space (' ')"
                    }

                    BoolParamField {
                        id: doPresTimesInput
                        param: do_p_ts_c
                        Layout.topMargin: 10
                        label.Layout.preferredWidth: labelW
                    }
                    NumListParamField {
                        id: pairTsInput
                        param: pair_ts_c
                        label.Layout.preferredWidth: labelW
                        inputLength: 300
                        tipText: "Separate values with a space (' ')"
                    }
                    NumListParamField {
                        param: pair_ps_c
                        id: pairPsInput
                        label.Layout.preferredWidth: labelW
                        inputLength: 300
                        tipText: "Separate values with a space (' ')"
                    }

                    Item {
                        Layout.fillHeight: true
                    }

                }
            }
        }

        Rectangle {
            height: 1
            Layout.fillWidth: true
            Layout.leftMargin: 16
            Layout.rightMargin: 4
            color: Material.color(Material.Blue, Material.Shade400)
        }

        RowLayout {

            // =========================
            // ==== Analysis Button ====
            Button {
                id: submitBtn
                Layout.preferredWidth: 110
                Layout.alignment: Qt.AlignCenter
                Layout.leftMargin: 40
                Layout.bottomMargin: 8
                Material.roundedScale: Material.SmallScale
                Material.accent: Material.Blue
                highlighted: true

                onClicked: {
                    forceActiveFocus();
                    app_form.request_analysis("accum");
                }

                Row {
                    anchors.horizontalCenter: parent.horizontalCenter
                    anchors.verticalCenter: parent.verticalCenter
                    spacing: 0

                    AppIcon {
                        anchors.verticalCenter: parent.verticalCenter
                        icon.width: 20
                        source: 'bolt-solid'
                        iconColor: "white"
                    }

                    Text {
                        anchors.verticalCenter: parent.verticalCenter
                        text: "Analyze"
                        font.pixelSize: 20
                        font.bold: true
                        color: "white"
                        bottomPadding: lgBtnBottomPadding
                    }
                }
            }

            SectionAlert {
                id: alert;
                Layout.bottomMargin: 5
                Layout.preferredWidth: 640
            }
        }
    }
}
