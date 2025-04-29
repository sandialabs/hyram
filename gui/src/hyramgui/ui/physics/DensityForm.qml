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

    function refreshForm()
    {
        var saturated = rel_phase_c.value !== 'fluid';

        if (saturated)
        {
            etk_t_c.set_null('value');
            tempInput.input.text = "";
        }
    }

    function calculate()
    {
        var missing = 0;
        var saturated = rel_phase_c.value !== 'fluid';
        var pIsNull = etk_p_c.is_null;
        var dIsNull = etk_d_c.is_null;
        var tIsNull = etk_t_c.is_null;

        if (tIsNull)
        {
            missing += 1;
        }
        if (pIsNull)
        {
            missing += 1;
        }
        if (dIsNull)
        {
            missing += 1;
        }

        alert.level = 1;
        alert.msg = "";

        if (saturated &&
            ((pIsNull && dIsNull) || (!pIsNull && !dIsNull)))
        {
            alert.level = 2;
            alert.msg = "Saturated fluid requires pressure or density for calculation";
        }
        else if (!saturated && (missing > 1 || missing === 0)) {
            alert.level = 2;
            alert.msg = "Provide exactly two of temperature, pressure, and density";
        }
        else
        {
            var msg = app_form.compute_tpd();
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
            text: "Density Calculations"
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
            text: "Calculate property from known inputs and shared fuel state"
        }

        Item {
            id: inputSpacer2
            Layout.preferredHeight: 4
        }

        FlickableForm {
            contentHeight: paramCols.height + 20 + 120

            // ==========================
            // ==== Parameter Inputs ====
            Item {
                id: paramColContainer
                Layout.fillHeight: true
                height: paramCols.height

                ColumnLayout {
                    id: paramCols

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
                        id: tempInput
                        param: etk_t_c
                        enabled: rel_phase_c.value === 'fluid'
                        opacity: rel_phase_c.value === 'fluid' ? 1 : fadeVal
                        input.tooltip.text: "Enter a value. Leave blank to calculate this value."
                    }
                    FloatNullableParamField {
                        input.tooltip.text: "Enter a non-negative value. Leave blank to calculate this value."
                        param: etk_p_c
                    }
                    FloatNullableParamField {
                        input.tooltip.text: "Enter a non-negative value. Leave blank to calculate this value."
                        param: etk_d_c
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
                                anchors.verticalCenter: parent.verticalCenter
                                icon.width: 16
                                source: 'bolt-solid'
                                iconColor: "white"
                            }

                            Text {
                                anchors.verticalCenter: parent.verticalCenter
                                text: "Calculate"
                                font.pixelSize: 16
                                font.bold: true
                                color: "white"
                            }
                        }
                    }

                    SectionAlert {id: alert; }

                    Item {Layout.fillHeight: true; }

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
