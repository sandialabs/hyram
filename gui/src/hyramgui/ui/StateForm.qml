/* Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
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

import "../hygu/ui/components"
import "../hygu/ui/components/buttons"
import "../hygu/ui/parameters"
import "../hygu/ui/pages"
import "parameters"
import "components"
import hyram.classes


FormPage {
    property alias alertRef: alert;

    function updateConcTotal()
    {
        let totalConc = (conc_h2_c.value +
                         conc_ch4_c.value +
                         conc_pro_c.value +
                         conc_n2_c.value +
                         conc_co2_c.value +
                         conc_eth_c.value +
                         conc_nbu_c.value +
                         conc_isb_c.value +
                         conc_npe_c.value +
                         conc_isp_c.value +
                         conc_nhx_c.value);
        totalConc = Math.round((totalConc + Number.EPSILON) * 1000) / 1000;
        totalConcDisp.text = totalConc;
    }


    function refreshForm()
    {
        updateConcTotal();
        relPInput.extraText.text = rel_p_abs_c.value ? "absolute" : "gauge";
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
            text: "Welcome to HyRAM+"
        }

        Rectangle {
            height: 2
            Layout.fillWidth: true
            Layout.leftMargin: 16
            Layout.rightMargin: 4
            color: Material.color(Material.Blue, Material.Shade400)
        }
        Text {
            id: formDescrip
            text: "Describe fuel and system state, then select an analysis form using the side-menu."
            font.pointSize: contentFontSize + 2
            Layout.leftMargin: 14
            Layout.bottomMargin: 0
        }

        FlickableForm
        {
            id: paramContainer
            contentHeight: paramCols.height + 20 + 120

            // ==========================
            // ==== Parameter Inputs ====
            Item {
                id: paramColContainer
                Layout.fillHeight: true
                Layout.fillWidth: true
                height: paramCols.height

                ColumnLayout {
                    id: paramCols
                    // spacing: 0

                    FormSectionHeader {
                        id: fuelSection
                        title: "Fuel Specification"
                        iconSrc: 'chart-pie-solid'
                        Layout.topMargin: 12
                    }

                    Text {
                        font.italic: true
                        font.pointSize: contentFontSize + 1
                        text: "Specify single fuel or fuel blend by adjusting concentrations"
                        Layout.topMargin: 0
                        Layout.bottomMargin: 5
                        Layout.leftMargin: 10
                    }

                    ChoiceParamField {
                        param: fuel_mix_c
                        tipText: "Select a preset fuel mixture"
                        label.horizontalAlignment: Text.AlignLeft
                        Layout.leftMargin: 10

                        selector.onActivated: {
                            app_form.set_fuel_mix(selector.currentIndex);
                        }
                    }
                    Item {
                        height: 2
                    }

                    RowLayout {
                        id: concRow

                        ColumnLayout {
                            id: concInputs
                            spacing: 0

                            FuelConcentrationField {
                                param: conc_h2_c
                                formula: "H<sub>2</sub>"
                                doShade: true
                            }
                            FuelConcentrationField {
                                param: conc_ch4_c
                                formula: "CH<sub>4</sub>"
                            }
                            FuelConcentrationField {
                                param: conc_pro_c
                                formula: "C<sub>3</sub>H<sub>8</sub>"
                                doShade: true
                            }
                            FuelConcentrationField {
                                param: conc_n2_c
                                formula: "N<sub>2</sub>"
                            }
                            FuelConcentrationField {
                                param: conc_co2_c
                                formula: "CO<sub>2</sub>"
                                doShade: true
                            }
                            FuelConcentrationField {
                                param: conc_eth_c
                                formula: "C<sub>2</sub>H<sub>6</sub>"
                            }
                            FuelConcentrationField {
                                param: conc_nbu_c
                                formula: "N-C<sub>4</sub>H<sub>10</sub>"
                                doShade: true
                            }
                            FuelConcentrationField {
                                param: conc_isb_c
                                formula: "HC(CH<sub>3</sub>)<sub>3</sub>"
                            }
                            FuelConcentrationField {
                                param: conc_npe_c
                                formula: "N-C<sub>5</sub>H<sub>12</sub>"
                                doShade: true
                            }
                            FuelConcentrationField {
                                param: conc_isp_c
                                formula: "CH(CH<sub>3</sub>)<sub>2</sub>(C<sub>2</sub>H<sub>5</sub>)"
                            }
                            FuelConcentrationField {
                                param: conc_nhx_c
                                formula: "N-C<sub>6</sub>H<sub>14</sub>"
                                doShade: true
                            }

                        }
                        ColumnLayout {
                            Layout.fillWidth: true
                            Text {
                                text: "Concentration Total"
                                font.italic: true
                                font.pointSize: labelFontSize + 1
                                horizontalAlignment: Text.AlignHCenter
                                verticalAlignment: Text.AlignVCenter
                                Layout.leftMargin: 40
                            }
                            RowLayout {
                                Layout.leftMargin: 40
                                Layout.alignment: Qt.AlignVCenter | Qt.AlignHCenter
                                Text {
                                    id: totalConcDisp
                                    text: "100"
                                    font.italic: true
                                    font.pointSize: labelFontSize + 1
                                    horizontalAlignment: Text.AlignHCenter
                                }
                                Text {
                                    text: "%"
                                    font.bold: true
                                }
                            }
                        }
                    }
                    Text {
                        text:  ("Note: blends capabilities have not been validated due to limited availability of blends data.\n" +
                                "Analysis of blends may fail to solve or require additional time (>10 min).")
                        font.pointSize: contentFontSize
                        font.italic: true
                        Layout.topMargin: 8
                    }


                    FormSectionHeader {
                        id: sectionInputs
                        title: "Common Inputs"
                        iconSrc: 'gauge-solid'
                        topPad: 16
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

                    RowLayout {
                        spacing: 5
                        ChoiceParamField {
                            param: nozzle_c
                            label.horizontalAlignment: Text.AlignLeft
                            tipText: "Model for calculating diameter, velocity, and thermodynamic state after complex shock structure of an under-expanded jet"
                        }

                        ButtonPopup {
                            id: nozzlePopup
                            h: nozzleCol.height + 30
                            tipText: "Click to view descriptions of notional nozzle models"

                            Column {
                                id: nozzleCol
                                parent: nozzlePopup.contentRef
                                width: 680
                                spacing: 8

                                FormSectionHeader3 {
                                    title: "Notional nozzle models"
                                }
                                TextPara {
                                    txt: ("Used to calculate effective diameter, velocity, and thermodynamic state
                                     (e.g., temperature, pressure, density) for an atmospheric pressure jet behaving like
                                     an under-expanded jet. See Section 3.2.1 of the Technical Reference Manual for
                                     detailed descriptions.")
                                }
                                TextPara {
                                    txt: ("<b>Yüceil/Ötügen</b>: conserves mass, momentum, and energy")
                                }
                                TextPara {
                                    txt: ("<b>Birch</b>: conserves mass and momentum, notional nozzle gas is at ambient
                                     pressure and has same temperature as fuel upstream of orifice")
                                }
                                TextPara {
                                    txt: ("<b>Birch2</b>: conserves mass, gas is at ambient pressure and has same
                                     temperature as fuel upstream of orifice, and velocity is the speed of sound")
                                }
                                TextPara {
                                    txt: ("<b>Ewan/Moody</b>: conserves mass, gas is at ambient pressure and has same
                                     temperature as fuel at orifice, and velocity is the speed of sound")
                                }
                                TextPara {
                                    txt: ("<b>Molkov</b>: conserves mass and energy, and velocity is the speed of sound for
                                    the expanded conditions")
                                }

                            }
                        }
                    }

                    BoolParamField {
                        id: pIsAbsInput
                        param: rel_p_abs_c
                    }

                    FloatParamField {
                        param: rel_t_c
                        tipText: "Release temperature of fluid"
                        label.horizontalAlignment: Text.AlignLeft
                        enabled: rel_phase_c.value === 'fluid'
                        opacity: rel_phase_c.value === 'fluid' ? 1 : fadeVal
                    }
                    FloatParamField {
                        param: rel_p_c
                        id: relPInput
                        tipText: "Release pressure"
                        label.horizontalAlignment: Text.AlignLeft

                        extraText.text: "absolute"
                        extraText.font.italic: true
                    }
                    FloatParamField {
                        param: amb_t_c
                        tipText: "Ambient temperature"
                        label.horizontalAlignment: Text.AlignLeft
                    }
                    FloatParamField {
                        param: amb_p_c
                        tipText: "Ambient pressure"
                        label.horizontalAlignment: Text.AlignLeft
                    }
                    FloatParamField {
                        param: rel_angle_c
                        tipText: ("Angle between centerline of jet near leak and the x-axis (horizontal).\n" +
                            "Centerline of jet is always on x-y plane.")
                    }

                    FloatParamField {
                        param: discharge_c
                        tipText: "Discharge coefficient"
                        label.horizontalAlignment: Text.AlignLeft
                    }
                    FloatParamField {
                        param: humidity_c
                        tipText: "Relative humidity"
                        label.horizontalAlignment: Text.AlignLeft
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

        SectionAlert {
            id: alert;
            Layout.bottomMargin: 5
            Layout.preferredWidth: 640
        }

    }
}
