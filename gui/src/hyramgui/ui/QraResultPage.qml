/* Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

 * You should have received a copy of the GNU General Public License along with HyRAM+.
 * If not, see https://www.gnu.org/licenses/.
 */

import QtQuick
import QtQuick.Dialogs
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Window
import QtQuick.Controls.Material
import Qt.labs.qmlmodels

import "../hygu/ui/utils.js" as Utils
import "../hygu/ui/components"
import "../hygu/ui/components/buttons"
import "../hygu/ui/pages"
// import "components"
import hygu.classes


ResultPage
{
    property ResultsForm pyform
    property int sectionWCalc: resultContainer.width * 0.94;
    property int sectionW: 1130 > sectionWCalc ? 1130 : sectionWCalc;
    property int datatableW: 1120;

    /**
     * Updates result data conditionally, based on analysis type. Also displays error/warning if found.
     */
    function updateContent()
    {
        // reset all sections
        let thermalPlots = [thermalPlot1, thermalPlot2, thermalPlot3, thermalPlot4, thermalPlot5];
        thermalPlots.forEach((img, i) => clearImage(img));

        let impulsePlots = [impulsePlot1, impulsePlot2, impulsePlot3, impulsePlot4, impulsePlot5];
        impulsePlots.forEach((img, i) => clearImage(img));

        let overpPlots = [overpPlot1, overpPlot2, overpPlot3, overpPlot4, overpPlot5];
        overpPlots.forEach((img, i) => clearImage(img));

        paramSection.visible = false;
        errorSection.visible = false;
        cancellationSection.visible = false;

        descrip.text = "";

        if (pyform === null)
        {
            title.text = "Submit an analysis";
            return;
        }

        title.text = "" + pyform.name_str;
        if (!pyform.finished)
        {
            status.text = "in-progress";
            status.color = color_progress;
            return;
        }

        // Analysis complete
        Utils.showChoiceParam(fuel_mix, pyform.fuel_mix);
        Utils.showChoiceParam(rel_phase, pyform.rel_phase);
        Utils.showParam(amb_t, pyform.amb_t);
        Utils.showParam(amb_p, pyform.amb_p);
        Utils.showParam(rel_t, pyform.rel_t);
        Utils.showParam(rel_p, pyform.rel_p);

        pipe_d.visible = !pyform.pipe_d.is_null;
        pipe_od.visible = pyform.pipe_d.is_null;
        pipe_thick.visible = pyform.pipe_d.is_null;
        if (pyform.pipe_d.is_null)
        {
            Utils.showParam(pipe_od, pyform.pipe_od);
            Utils.showParam(pipe_thick, pyform.pipe_thick);
        }
        else
        {
            Utils.showParam(pipe_d, pyform.pipe_d);
        }
        Utils.showParam(pipe_l, pyform.pipe_l);

        Utils.showParam(leak_d, pyform.leak_d);
        Utils.showParam(rel_angle, pyform.rel_angle);
        Utils.showParam(humid, pyform.humid);
        Utils.showChoiceParam(nozzle, pyform.nozzle);

        Utils.showChoiceParam(overp_method, pyform.overp_method);
        Utils.showChoiceParam(overp_probit, pyform.overp_probit);
        Utils.showParam(tnt_factor, pyform.tnt_factor);
        Utils.showChoiceParam(mach_speed, pyform.mach_speed);
        Utils.showChoiceParam(thermal_probit, pyform.thermal_probit);
        Utils.showParam(exposure_t, pyform.exposure_t);

        Utils.showParam(discharge, pyform.discharge);
        Utils.showParam(detection, pyform.detection);
        Utils.showParam(exclusion, pyform.exclusion);
        Utils.showChoiceParam(mass_flow_leak, pyform.mass_flow_leak);
        Utils.showParam(mass_flow, pyform.mass_flow);
        Utils.showBasicParam(seed, pyform.seed);

        Utils.showBasicParam(n_vehicles, pyform.n_vehicles);
        Utils.showParam(n_fuelings, pyform.n_fuelings);
        Utils.showParam(n_vehicle_days, pyform.n_vehicle_days);

        Utils.showBasicParam(n_compressors, pyform.n_compressors);
        Utils.showBasicParam(n_vessels, pyform.n_vessels);
        Utils.showBasicParam(n_valves, pyform.n_valves);
        Utils.showBasicParam(n_instruments, pyform.n_instruments);
        Utils.showBasicParam(n_joints, pyform.n_joints);
        Utils.showBasicParam(n_hoses, pyform.n_hoses);
        Utils.showBasicParam(n_filters, pyform.n_filters);
        Utils.showBasicParam(n_flanges, pyform.n_flanges);
        Utils.showBasicParam(n_exchangers, pyform.n_exchangers);
        Utils.showBasicParam(n_vaporizers, pyform.n_vaporizers);
        Utils.showBasicParam(n_arms, pyform.n_arms);
        Utils.showBasicParam(n_extra1, pyform.n_extra1);
        Utils.showBasicParam(n_extra2, pyform.n_extra2);

        mach_speed.visible = pyform.overp_method.value === 'bst';
        tnt_factor.visible = pyform.overp_method.value === 'tnt';

        mass_flow.visible = pyform.is_unchoked;
        mass_flow_leak.visible = pyform.is_unchoked;

        Utils.showStrValue(air, "Average individual risk (AIR)", pyform.qra_air, "fatalities / exposure-year");
        Utils.showStrValue(pll, "Potential loss of life (PLL)", pyform.qra_pll, "fatalities / year");
        Utils.showStrValue(far, "Fatal accident rate (FAR)", pyform.qra_far, "fatalities in 10<sup>8</sup> person-hours");

        paramSection.visible = true;

        if (pyform.has_error)
        {
            status.text = "error";
            status.color = color_danger;
            errorMessage.text = pyform.error_message;
            errorSection.visible = true;
            resultSection.visible = false;
            // scrollContainer.contentHeight = paramSection.height + resultSection.height;
            return;
        }

        if (pyform.was_canceled)
        {
            cancellationSection.visible = true;
            resultSection.visible = false;
            // scrollContainer.contentHeight = paramSection.height + resultSection.height;
            return;
        }

        resultSection.visible = true;
        status.text = "complete";
        status.color = color_success;

        var thermalPlotPaths = JSON.parse(pyform.qra_thermal_plots);
        thermalPlots.forEach((elem, i) => updateImage(elem, thermalPlotPaths[i]));

        var overpPlotPaths = JSON.parse(pyform.qra_overp_plots);
        overpPlots.forEach((elem, i) => updateImage(elem, overpPlotPaths[i]));

        var impulsePlotPaths = JSON.parse(pyform.qra_impulse_plots);
        impulsePlots.forEach((elem, i) => updateImage(elem, impulsePlotPaths[i]));

        // updateHeight();
    }

    /**
     * Updates content height of scroll section based on contents.
     * Do this separately from above so it can be tied to heightChanged event of Flickable.
     */
    // function updateHeight()
    // {
    //     if (pyform === null) return;
    // }

    Pane {
        anchors.fill: parent

        Column {  // main positioner
            anchors.fill: parent
            spacing: 10

            // Top section with title, buttons
            Item {
                width: parent.width
                height: title.height

                Text {
                    id: title
                    font.pointSize: 20
                    text: ""
                    elide: Text.ElideRight
                }
                Text {
                    id: status
                    font.pointSize: 13
                    font.italic: true
                    text: ""
                    anchors.left: title.right
                    anchors.leftMargin: 5
                }

                Row {
                    id: buttonBar
                    visible: pyform && pyform.finished && !pyform.has_error
                    height: 40
                    anchors.right: parent.right
                    anchors.top: parent.top
                    anchors.rightMargin: 100
                    spacing: 4

                    IconButton {
                        id: restoreParamsBtn
                        img: 'tent-arrows-down-solid'
                        tooltip: ("Overwrites form parameter values to match this analysis")
                        onClicked: pyform.overwrite_form_data()
                    }

                    IconButton {
                        id: openOutputBtn
                        img: 'folder-open-solid'
                        tooltip: ("Open directory containing analysis results")
                        onClicked: pyform.open_output_directory()
                    }

                    IconButton {
                        id: deleteBtn
                        img: 'trash-solid'
                        tooltip: ("Delete analysis results")
                        bgColor: Material.color(Material.Red, Material.Shade300)
                        onClicked: function() {
                            queueListView.removeItem(qIndex);
                            resultContainer.close();
                        }
                    }
                }


                AppIcon {
                    source: 'xmark-solid'
                    anchors.right: parent.right
                    anchors.top: parent.top

                    MouseArea {
                        anchors.fill: parent
                        onClicked: resultContainer.close()
                    }
                }
            }

            Text {
                id: descrip
                font.pointSize: 16
                font.italic: true
                text: ""
            }
            Text {
                id: inProgressMessage
                visible: pyform && !pyform.finished
                font.pointSize: 16
                font.italic: true
                width: parent.width
                horizontalAlignment: Text.AlignHCenter
                text: "Analysis in-progress - reopen once analysis is complete"
            }

            VSpacer { height: 5 }

            Column {
                id: errorSection

                Text {
                    id: errorHeader
                    font.pointSize: 16
                    font.italic: true
                    color: Material.color(Material.Red)
                    text: "Error during analysis"
                }
                Text {
                    id: errorMessage
                    font.pointSize: 13
                    color: Material.color(Material.Red)
                    text: ""
                }
                VSpacer { height: 10 }
            }

            // Note: this section rarely visible since queue item removed immediately.
            Column {
                id: cancellationSection

                Row {
                    spacing: 2

                    AppIcon {
                        anchors.verticalCenter: parent.verticalCenter
                        source: 'circle-exclamation-solid'
                        iconColor: color_text_warning
                    }

                    Text {
                        anchors.verticalCenter: parent.verticalCenter
                        id: cancellationHeader
                        font.pointSize: 16
                        font.italic: true
                        color: color_text_warning
                        text: " Analysis canceled"
                    }
                }

                Text {
                    id: cancelMessage
                    font.pointSize: 13
                    color: color_text_warning
                    text: "Analysis successfully canceled"
                }
                VSpacer { height: 10 }
            }

            // Scrollable results display
            Flickable
            {
                id: scrollContainer
                clip: true
                height: parent.height - y
                width: parent.width
                contentHeight: paramSection.height + resultSection.childrenRect.height + 15  // allows for expanding
                contentWidth: parent.width
                visible: pyform && pyform.finished

                // onHeightChanged: { updateHeight(); }

                Layout.fillHeight: true
                Layout.fillWidth: true

                boundsBehavior: Flickable.StopAtBounds
                boundsMovement: Flickable.FollowBoundsBehavior

                // increase scroll speed
                flickDeceleration: 15000
                ScrollMouseArea {container: scrollContainer}

                ScrollBar.vertical: ScrollBar {policy: ScrollBar.AlwaysOn; }

                Column {
                    id: paramSection

                    FormSectionHeader {
                        iconSrc: "list-solid"
                        title: "Analysis Parameters"
                        rWidth: barWidth
                        bottomPad: 8
                    }
                    VSpacer {
                        height: 5
                    }

                    FormSectionHeader2 {
                        title: "System Description"
                        rWidth: barWidth
                        fontSize: 12
                        topPad: 10
                        bottomPad: 5
                    }

                    Row {
                        spacing: 35
                        Column {
                            width: 250
                            ResultParamText { id: fuel_mix; }
                            ResultParamText { id: rel_phase; }
                            ResultParamText { id: amb_t; }
                            ResultParamText { id: amb_p; }
                            ResultParamText { id: rel_t; }
                            ResultParamText { id: rel_p; }
                            ResultParamText { id: nozzle; }
                            ResultParamText { id: leak_d; }
                            ResultParamText { id: rel_angle; }
                            ResultParamText { id: seed; }
                        }
                        Column {
                            width: 250
                            ResultParamText { id: discharge; }
                            ResultParamText { id: humid; }
                            ResultParamText { id: exclusion; }
                            ResultParamText { id: n_vehicles; }
                            ResultParamText { id: n_fuelings; }
                            ResultParamText { id: n_vehicle_days; }
                            ResultParamText { id: mass_flow_leak; }
                            ResultParamText { id: mass_flow; }
                            ResultParamText { id: detection; }
                        }
                        Column {
                            width: 250

                            ResultParamText { id: n_compressors; }
                            ResultParamText { id: n_vessels; }
                            ResultParamText { id: n_valves; }
                            ResultParamText { id: n_instruments; }
                            ResultParamText { id: n_joints; }
                            ResultParamText { id: pipe_l; }
                            ResultParamText { id: pipe_d; }
                            ResultParamText { id: pipe_od; }
                            ResultParamText { id: pipe_thick; }
                        }
                        Column {
                            width: 250
                            ResultParamText { id: n_hoses; }
                            ResultParamText { id: n_filters; }
                            ResultParamText { id: n_flanges; }
                            ResultParamText { id: n_exchangers; }
                            ResultParamText { id: n_vaporizers; }
                            ResultParamText { id: n_arms; }
                            ResultParamText { id: n_extra1; }
                            ResultParamText { id: n_extra2; }
                        }
                    }
                    VSpacer {
                        height: 5
                    }
                    // Row {
                    //     spacing: 35
                    //     Column {
                    //         width: 250
                    //     }
                    //     Column {
                    //         width: 250
                    //     }
                    //     Column {
                    //         width: 250
                    //
                    //     }
                    // }
                    //
                    // VSpacer {height: 15}
                    // FormSectionHeader2 {
                    //     title: "Frequencies / Probabilities"
                    //     rWidth: barWidth
                    //     fontSize: 12
                    //     topPad: 10
                    //     bottomPad: 5
                    // }
                    // Row {
                    //     spacing: 35
                    //     Column {
                    //         width: 250
                    //     }
                    //     Column {
                    //         width: 250
                    //     }
                    //     Column {
                    //         width: 250
                    //     }
                    // }


                    VSpacer {height: 15}
                    FormSectionHeader2 {
                        title: "Consequence Models"
                        rWidth: barWidth
                        fontSize: 12
                        topPad: 10
                        bottomPad: 5
                    }
                    Row {
                        spacing: 35
                        Column {
                            width: 250
                            ResultParamText { id: overp_method; }
                            ResultParamText { id: tnt_factor; }
                            ResultParamText { id: mach_speed; }
                        }
                        Column {
                            width: 250
                            ResultParamText { id: thermal_probit; }
                            ResultParamText { id: exposure_t; }
                        }
                        Column {
                            width: 250
                            ResultParamText { id: overp_probit; }
                        }
                    }

                    VSpacer {
                        height: 15
                    }
                    // Row {
                    //     spacing: 35
                    //     Column {
                    //         width: 250
                    //     }
                    //     Column {
                    //         width: 250
                    //     }
                    //     Column {
                    //         width: 250
                    //     }
                    //     Column {
                    //         width: 250
                    //     }
                    // }
                }

                ColumnLayout {
                    id: resultSection
                    anchors.top: paramSection.bottom
                    anchors.topMargin: 30

                    FormSectionHeader {
                        title: "Analysis Results"
                        iconSrc: "chart-line-solid"
                        rWidth: barWidth
                        topPad: 10
                        bottomPad: 8
                    }

                    ResultParamText { id: pll;}
                    ResultParamText { id: far;}
                    ResultParamText { id: air;}

                    Item {
                        Layout.preferredHeight: 20
                    }

                    CollapsibleSection {
                        id: scenarioSection
                        w: sectionW
                        title: "Scenario Ranking"
                        titleFontSize: labelFontSize + 2;
                        asHeader: true
                        startOpen: true

                        ColumnLayout {
                            parent: scenarioSection.containerRef
                            Layout.leftMargin: 5

                            DataTable {
                                tableW: 710;
                                maxH: 800;
                                tableH: 800;
                                headerH: 40
                                nCols: 4;
                                colW: 160;
                                tableModelRef: rankingTableModel
                                headers: [ "Scenario", "Scenario Outcome", "Average Events / Year", "Risk (PLL) Contribution"]
                                keys: ['scenario', 'outcome', 'events', 'pll']
                                dataSourceCallback: function() { return pyform.qra_scenario_ranking_data; };

                                tableViewRef.model: TableModel {
                                    id: rankingTableModel
                                    TableModelColumn { display: "scenario" }
                                    TableModelColumn { display: "outcome" }
                                    TableModelColumn { display: "events" }
                                    TableModelColumn { display: "pll" }
                                }
                            }

                        }
                    }

                    CollapsibleSection {
                        id: scenarioDetailsSection
                        w: sectionW
                        title: "Scenario Details"
                        titleFontSize: labelFontSize + 2;
                        asHeader: true
                        startOpen: true

                        ColumnLayout {
                            parent: scenarioDetailsSection.containerRef
                            Layout.leftMargin: 5

                            Caption {text: "Leak scenario details"}

                            DataTable {
                                tableW: 620;
                                tableH: 40 + 5 * 25 + 10;
                                headerH: 40;
                                nCols: 4;
                                colW: 150;
                                tableModelRef: leakTableModel
                                headers: [ "Leak Size", "Mass Flow Rate (kg/s)", "Leak Diameter (m)", "Annual Leak
                                 Frequency"]
                                keys: ['leak', 'flow', 'leak_d', 'f_release']
                                dataSourceCallback: function() { return pyform.qra_scenario_details_data; };

                                tableViewRef.model: TableModel {
                                    id: leakTableModel
                                    TableModelColumn { display: "leak" }
                                    TableModelColumn { display: "flow" }
                                    TableModelColumn { display: "leak_d" }
                                    TableModelColumn { display: "f_release" }
                                }
                            }

                            Caption {
                                Layout.topMargin: 16
                                text: "Leak outcome probabilities"
                            }

                            DataTable {
                                tableW: 620;
                                tableH: 40 + 4 * 25;
                                headerH: 40;
                                nCols: 6;
                                colW: 100;
                                tableModelRef: outcomeTableModel
                                headers: [ "Scenario Outcome", "0.01% Release", "0.10% Release", "1% Release",
                                    "10% Release", "100% Release"]
                                keys: ['outcome', 'r0d01', 'r0d1', 'r1', 'r10', 'r100']
                                dataSourceCallback: function() { return pyform.qra_scenario_outcome_data; };

                                tableViewRef.model: TableModel {
                                    id: outcomeTableModel
                                    TableModelColumn { display: "outcome" }
                                    TableModelColumn { display: "r0d01" }
                                    TableModelColumn { display: "r0d1" }
                                    TableModelColumn { display: "r1" }
                                    TableModelColumn { display: "r10" }
                                    TableModelColumn { display: "r100" }
                                }
                            }

                        }
                    }


                    CollapsibleSection {
                        id: cutSection
                        w: sectionW
                        title: "Cut Sets"
                        titleFontSize: labelFontSize + 2;
                        asHeader: true
                        startOpen: true

                        ColumnLayout {
                            parent: cutSection.containerRef
                            Layout.leftMargin: 5

                            Caption {text: "Annual frequencies for specific components or failures for each leak size"}

                            DataTable {
                                tableW: 800;
                                tableH: 680;
                                maxH: 680;
                                autoSizeHeight: false
                                nCols: 6;
                                colW: 100;
                                allowSort: false
                                tableViewRef.columnWidthProvider: function (column) {
                                    var columnWidths = [200, 120, 120, 120, 120, 120];
                                    return columnWidths[column];
                                }

                                headers: [ "Cut Set", "0.01% Release", "0.10% Release", "1% Release",
                                            "10% Release", "100% Release"]
                                keys: ['cutSet', 'r0d01', 'r0d1', 'r1', 'r10', 'r100']

                                columnAlignments: [Text.AlignLeft, Text.AlignHCenter, Text.AlignHCenter,
                                                    Text.AlignHCenter, Text.AlignHCenter, Text.AlignHCenter]

                                dataSourceCallback: function() { return pyform.qra_cut_set_data; };
                                tableModelRef: cutModel

                                tableViewRef.model: TableModel {
                                    id: cutModel
                                    TableModelColumn { display: "cutSet" }
                                    TableModelColumn { display: "r0d01" }
                                    TableModelColumn { display: "r0d1" }
                                    TableModelColumn { display: "r1" }
                                    TableModelColumn { display: "r10" }
                                    TableModelColumn { display: "r100" }
                                }
                            }

                        }
                    }

                    CollapsibleSection {
                        id: thermalSection
                        w: sectionW
                        title: "Thermal Effects"
                        titleFontSize: labelFontSize + 2;
                        asHeader: true
                        startOpen: true

                        ColumnLayout {
                            parent: thermalSection.containerRef
                            Layout.leftMargin: 5
                            spacing: 20

                            GridLayout {
                                id: thermalPlotCol
                                rows: 3
                                columns: 2
                                rowSpacing: 10
                                columnSpacing: 8
                                SimImage {id: thermalPlot1; height: 350; }
                                SimImage {id: thermalPlot2; height: 350; }
                                SimImage {id: thermalPlot3; height: 350; }
                                SimImage {id: thermalPlot4; height: 350; }
                                SimImage {id: thermalPlot5; height: 350; }
                            }

                            DataTable {
                                id: thermalTable
                                tableW: 1120;
                                tableH: 40 + 10 * 25;
                                nCols: 9;
                                colW: 100;
                                tableViewRef.columnWidthProvider: function (column) {
                                    var columnWidths = [100, 100, 100, 100, 140, 140, 140, 140, 140];
                                    return columnWidths[column];
                                }

                                headers: [
                                    "Position", "X (m)", "Y (m)", "Z (m)",
                                    "0.01% Leak Heat Flux (kW/m<sup>2</sup>)",
                                    "0.1% Leak Heat Flux (kW/m<sup>2</sup>)",
                                    "1% Leak Heat Flux (kW/m<sup>2</sup>)",
                                    "10% Leak Heat Flux (kW/m<sup>2</sup>)",
                                    "100% Leak Heat Flux (kW/m<sup>2</sup>)",
                                ]
                                keys: ['idx', 'x', 'y', 'z', 'r0d01', '0rd1', 'r1', 'r10', 'r100']

                                dataSourceCallback: function() { return pyform.qra_thermal_data; };
                                tableModelRef: thermalModel

                                tableViewRef.model: TableModel {
                                    id: thermalModel
                                    TableModelColumn { display: "idx" }
                                    TableModelColumn { display: "x" }
                                    TableModelColumn { display: "y" }
                                    TableModelColumn { display: "z" }
                                    TableModelColumn { display: "r0d01" }
                                    TableModelColumn { display: "r0d1" }
                                    TableModelColumn { display: "r1" }
                                    TableModelColumn { display: "r10" }
                                    TableModelColumn { display: "r100" }
                                }
                            }

                        }
                    }


                    CollapsibleSection {
                        id: overpSection
                        w: sectionW
                        title: "Overpressure"
                        titleFontSize: labelFontSize + 2;
                        asHeader: true
                        startOpen: true

                        ColumnLayout {
                            parent: overpSection.containerRef
                            Layout.leftMargin: 5
                            spacing: 20

                            GridLayout {
                                // id: thermalPlotCol
                                rows: 3
                                columns: 2
                                rowSpacing: 10
                                columnSpacing: 8
                                SimImage {id: overpPlot1; height: 350; }
                                SimImage {id: overpPlot2; height: 350; }
                                SimImage {id: overpPlot3; height: 350; }
                                SimImage {id: overpPlot4; height: 350; }
                                SimImage {id: overpPlot5; height: 350; }
                            }

                            DataTable {
                                id: overpTable
                                tableW: datatableW;
                                tableH: 40 + 10 * 25;
                                nCols: 9;
                                colW: 100;
                                tableViewRef.columnWidthProvider: function (column) {
                                    var columnWidths = [100, 100, 100, 100, 140, 140, 140, 140, 140];
                                    return columnWidths[column];
                                }

                                headers: [
                                    "Position", "X (m)", "Y (m)", "Z (m)",
                                    "0.01% Leak Overpressure (kPa)",
                                    "0.1% Leak Overpressure (kPa)",
                                    "1% Leak Overpressure (kPa)",
                                    "10% Leak Overpressure (kPa)",
                                    "100% Leak Overpressure (kPa)",
                                ]
                                keys: ['idx', 'x', 'y', 'z', 'r0d01', '0rd1', 'r1', 'r10', 'r100']

                                dataSourceCallback: function() { return pyform.qra_overp_data; };
                                tableModelRef: overpModel

                                tableViewRef.model: TableModel {
                                    id: overpModel
                                    TableModelColumn { display: "idx" }
                                    TableModelColumn { display: "x" }
                                    TableModelColumn { display: "y" }
                                    TableModelColumn { display: "z" }
                                    TableModelColumn { display: "r0d01" }
                                    TableModelColumn { display: "r0d1" }
                                    TableModelColumn { display: "r1" }
                                    TableModelColumn { display: "r10" }
                                    TableModelColumn { display: "r100" }
                                }
                            }

                        }
                    }


                    CollapsibleSection {
                        id: impulseSection
                        w: sectionW
                        title: "Impulse"
                        titleFontSize: labelFontSize + 2;
                        asHeader: true
                        startOpen: true

                        ColumnLayout {
                            parent: impulseSection.containerRef
                            Layout.leftMargin: 5
                            spacing: 20

                            GridLayout {
                                rows: 3
                                columns: 2
                                rowSpacing: 10
                                columnSpacing: 8
                                SimImage {id: impulsePlot1; height: 350; }
                                SimImage {id: impulsePlot2; height: 350; }
                                SimImage {id: impulsePlot3; height: 350; }
                                SimImage {id: impulsePlot4; height: 350; }
                                SimImage {id: impulsePlot5; height: 350; }
                            }


                            DataTable {
                                id: impulseTable
                                tableW: datatableW;
                                tableH: 40 + 10 * 25;
                                nCols: 9;
                                colW: 100;
                                tableViewRef.columnWidthProvider: function (column) {
                                    var columnWidths = [100, 100, 100, 100, 140, 140, 140, 140, 140];
                                    return columnWidths[column];
                                }

                                headers: [
                                    "Position", "X (m)", "Y (m)", "Z (m)",
                                    "0.01% Leak Impulse (kPa*s)",
                                    "0.1% Leak Impulse (kPa*s)",
                                    "1% Leak Impulse (kPa*s)",
                                    "10% Leak Impulse (kPa*s)",
                                    "100% Leak Impulse (kPa*s)",
                                ]
                                keys: ['idx', 'x', 'y', 'z', 'r0d01', '0rd1', 'r1', 'r10', 'r100']

                                dataSourceCallback: function() { return pyform.qra_impulse_data; };
                                tableModelRef: impulseModel

                                tableViewRef.model: TableModel {
                                    id: impulseModel
                                    TableModelColumn { display: "idx" }
                                    TableModelColumn { display: "x" }
                                    TableModelColumn { display: "y" }
                                    TableModelColumn { display: "z" }
                                    TableModelColumn { display: "r0d01" }
                                    TableModelColumn { display: "r0d1" }
                                    TableModelColumn { display: "r1" }
                                    TableModelColumn { display: "r10" }
                                    TableModelColumn { display: "r100" }
                                }
                            }

                        }
                    }


                    VSpacer { height: 20 }
                }
            }
        }
    }


}
