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
    property alias alertRef: alert;

    function refreshForm()
    {
        let autoLimits = autoPlumePlotLimits.checked();
        xLimInput.visible = !autoLimits;
        yLimInput.visible = !autoLimits;
        moleLimInput.visible = !autoLimits;
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
            text: "Plume Dispersion"
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
            text: "Calculate characteristics of a plume"
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

                    FloatParamField {
                        param: plume_mass_flow_c
                        enabled: app_form.is_unchoked
                        opacity: app_form.is_unchoked ? 1 : 0.3
                        tipText: "Optional mass flow rate of fuel out of orifice (unchoked)"
                    }

                    FloatParamField {
                        param: leak_d_c
                        tipText: "Diameter of leaking orifice"
                    }

                    FormSectionHeader {
                        id: section2
                        title: "Plotting"
                        iconSrc: 'chart-simple-solid'
                    }

                    StringParamField {
                        param: plume_plot_title_c
                        inputLength: 240
                        tipText: "Optional descriptive title for output plot"
                    }

                    NumListParamField {
                        param: mole_contours_c
                        inputLength: 240
                        tipText: "Optional mole fraction contours to plot. Separate values with a space (' ')"
                    }

                    BoolParamField {
                        id: autoPlumePlotLimits
                        param: plume_auto_limits_c
                    }
                    NumPairParamField {
                        id: xLimInput
                        labelText: "Plot X limits"
                        param1: plume_xmin_c
                        param2: plume_xmax_c
                        // tipText: "Plot X bounds"
                    }
                    NumPairParamField {
                        id: yLimInput
                        labelText: "Plot Y limits"
                        param1: plume_ymin_c
                        param2: plume_ymax_c
                        // tipText: "Plot Y bounds"
                    }
                    NumPairParamField {
                        id: moleLimInput
                        labelText: "Mole fraction scale limits"
                        param1: plume_mole_min_c
                        param2: plume_mole_max_c
                        tipText: "Plot mole fraction scale bounds. Fractions outside this range will appear as single color."
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
                    app_form.request_analysis("plume");
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
