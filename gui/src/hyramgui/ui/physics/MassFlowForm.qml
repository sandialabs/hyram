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

import "../../hygu/ui/utils.js" as Utils
import "../../hygu/ui/components"
import "../../hygu/ui/components/buttons"
import "../../hygu/ui/parameters"
import "../../hygu/ui/pages"
import "../components"
import "../parameters"
import hygu.classes
import hyram.classes


FormPage {
    property alias alertRef: alert;

    function refreshForm()
    {
        var diffKeys = app_form.get_diff_keys;

        if (diffKeys.indexOf('etk_t_empty') > -1)
        {
            etk_flow_c.set_null('');
            tEmpty.text = getTimeEmpty();
            massFlow.text = getMassFlow();
            Utils.updateImage(plot1, app_form.etk_blowdown_plot)
        }
        else if (diffKeys.indexOf('etk_flow') > -1)
        {
            etk_t_empty_c.set_null('');
            tEmpty.text = getTimeEmpty();
            massFlow.text = getMassFlow();
        }
        else {
            etk_flow_c.set_null('');
            etk_t_empty_c.set_null('');
        }

    }

    function getTimeEmpty()
    {
        if (etk_t_empty_c.is_null)
        {
            return "<strong>Time to empty (s)</strong>:";
        }
        else
        {
            var val = parseFloat(etk_t_empty_c.value).toExponential(3);
            return "<strong>Time to empty (s)</strong>: " + val;
        }
    }

    function getMassFlow()
    {
        if (etk_flow_c.is_null)
        {
            return "<strong>Mass flow rate (kg/s)</strong>:";
        }
        else {
            var val = parseFloat(etk_flow_c.value).toExponential(3);
            return "<strong>Mass flow rate (kg/s)</strong>: " + val;
        }
    }

    function calculate()
    {
        var saturated = rel_phase_c.value !== 'fluid';
        var isBlowdown = etk_is_blowdown_c.value;

        Utils.clearImage(plot1);
        alert.level = 1;
        alert.msg = "";

        if (etk_p_c.is_null || etk_p_amb_c.is_null || etk_orif_d_c.is_null || etk_discharge_c.is_null)
        {
            alert.level = 2;
            alert.msg = "Ensure all required parameters are provided";
        }

        else if (isBlowdown && etk_v_c.is_null)
        {
            alert.level = 2;
            alert.msg = "Blowdown requires a volume";
        }

        else if (!saturated && etk_t_c.is_null)
        {
            alert.level = 2;
            alert.msg = "Unsaturated fuel requires a temperature";
        }

        else
        {
            var msg = app_form.calc_mass_flow_blowdown();
            if (msg !== "")
            {
                alert.level = 2;
                alert.msg = msg;
            }
        }
    }

    Component.onCompleted:
    {
        refreshForm();
    }


    ColumnLayout {
        width: parent.width

        // Form header
        Text {
            font.pointSize: header1FontSize
            id: formHeader
            font.weight: 600
            Layout.leftMargin: 14
            text: "Mass Flow Rate"
        }

        Rectangle {
            height: 2
            Layout.fillWidth: true
            Layout.leftMargin: 16
            Layout.rightMargin: 4
            color: Material.color(Material.Blue, Material.Shade400)
        }
        Text {
            font.pointSize: contentFontSize + 1
            id: formDescrip
            font.italic: true
            Layout.leftMargin: 14
            text: "Calculate mass flow for steady or blowdown release from parameters and shared fuel state"
        }

        Item {
            id: inputSpacer2
            Layout.preferredHeight: 4
        }

        FlickableForm {
            contentHeight: paramCols.height + 20 + 50

            // ==========================
            // ==== Parameter Inputs ====
            Item {
                id: paramColContainer
                Layout.fillHeight: true
                height: paramCols.height

                RowLayout {
                    spacing: 20

                    ColumnLayout {
                        Layout.maximumWidth: 400
                        id: paramCols

                        // release type radio buttons
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
                                Layout.preferredWidth: paramLabelWidth
                                Layout.alignment: Qt.AlignVCenter
                            }
                            Item {}

                            RadioButton {
                                id: isBlowdown
                                checked: etk_is_blowdown_c.value === true
                                text: "Blowdown"
                                font.pointSize: labelFontSize
                                Layout.maximumHeight: 30
                                Layout.alignment: Qt.AlignLeft
                                onClicked: { etk_is_blowdown_c.value = true; }
                            }

                            RadioButton {
                                checked: etk_is_blowdown_c.value === false
                                text: "Steady"
                                font.pointSize: labelFontSize
                                Layout.maximumHeight: 30
                                Layout.alignment: Qt.AlignLeft
                                onClicked: { etk_is_blowdown_c.value = false; }
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

                        RowLayout {
                            spacing: 5
                            ChoiceParamField {
                                param: rel_phase_c
                                label.horizontalAlignment: Text.AlignLeft
                                tipText: "Phase of release fluid"
                            }

                            ButtonPopup {
                                id: phasePopup
                                h: phaseCol.height + 30
                                tipText: "Click to view descriptions of fluid phases"

                                Column {
                                    id: phaseCol
                                    parent: phasePopup.contentRef
                                    width: 680
                                    spacing: 8

                                    FormSectionHeader3 {
                                        title: "Fluid Phases"
                                    }
                                    TextPara {
                                        txt: ("<b>Fluid</b>: state specified using temperature and pressure")
                                    }
                                    TextPara {
                                        txt: ("<b>Saturated Vapor</b>: state specified with pressure, fuel is at its boiling
                                    point and 100% vapor (i.e., vapor above liquid)")
                                    }
                                    TextPara {
                                        txt: ("<b>Saturated Liquid</b>: state specified with pressure, fuel is at its boiling
                                    point and 100% liquid (i.e., liquid below vapor)")
                                    }
                                }
                            }
                        }

                        FloatNullableParamField {
                            param: etk_t_c
                            input.tooltip.text: "Enter a value."
                            opacity: {
                                if (rel_phase_c.value === 'fluid')
                                {
                                    if (!etk_t_c.is_null) input.text = etk_t_c.value;
                                    return 1;
                                }
                                else
                                {
                                    input.text = "";
                                    return 0.3;
                                }
                            }
                        }
                        FloatNullableParamField {
                            param: etk_p_c
                            input.tooltip.text: "Enter a positive value"
                        }
                        FloatNullableParamField {
                            param: etk_p_amb_c
                            input.tooltip.text: "Enter a positive value"
                        }
                        FloatNullableParamField {
                            param: etk_v_c
                            input.tooltip.text: "Enter a positive value"
                            enabled: etk_is_blowdown_c.value
                            opacity: {
                                if (etk_is_blowdown_c.value)
                                {
                                    if (!etk_v_c.is_null)
                                    {
                                        input.text = etk_v_c.value;
                                    }
                                    return 1;
                                }
                                else
                                {
                                    input.text = "";
                                    return 0.3;
                                }
                            }
                        }
                        FloatNullableParamField {
                            param: etk_orif_d_c
                            input.tooltip.text: "Enter a positive value"
                        }
                        FloatNullableParamField {
                            param: etk_discharge_c
                            input.tooltip.text: "Enter a value between 0 and 1.0"
                        }

                        Button {
                            Layout.preferredWidth: defaultInputW
                            Layout.leftMargin: 150
                            Layout.bottomMargin: 8
                            Material.roundedScale: Material.SmallScale
                            Material.accent: Material.BlueGrey
                            highlighted: true

                            onClicked: {
                                forceActiveFocus();
                                calculate();
                            }

                            Row {
                                anchors.horizontalCenter: parent.horizontalCenter
                                anchors.verticalCenter: parent.verticalCenter
                                spacing: 0

                                AppIcon {
                                    id: regIcon
                                    anchors.verticalCenter: parent.verticalCenter
                                    icon.width: 16
                                    source: 'bolt-solid'
                                    iconColor: "white"
                                }
                                // BusyIndicator {  // doesn't work because calc in same thread
                                //     id: spinner
                                //     visible: false
                                //     height: 32
                                //     width: 32
                                //     running: true
                                //     // anchors.verticalCenter: parent.verticalCenter
                                //     // Material.accent: "white"
                                // }

                                Text {
                                    anchors.verticalCenter: parent.verticalCenter
                                    text: "Calculate"
                                    font.pixelSize: 16
                                    font.bold: true
                                    color: "white"
                                }
                            }
                        }


                        Item {
                            Layout.preferredHeight: 20
                        }

                        // mass flow rate (is steady)
                        ResultParamText {
                            id: massFlow
                            text: getMassFlow()
                            visible: etk_is_blowdown_c.value === false
                        }

                        // time to empty (blowdown)
                        ResultParamText {
                            id: tEmpty
                            text: getTimeEmpty()
                            visible: etk_is_blowdown_c.value === true
                        }

                        SectionAlert {id: alert; }

                        Item {Layout.fillHeight: true; }

                    }


                    ColumnLayout {
                        Layout.fillWidth: true

                        SimImage {id: plot1; Layout.fillWidth: true }
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

    }
}
