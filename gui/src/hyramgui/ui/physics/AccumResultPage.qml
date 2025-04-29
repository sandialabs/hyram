/*
 * Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
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

import "../../hygu/ui/utils.js" as Utils
import "../../hygu/ui/components"
import "../../hygu/ui/components/buttons"
import "../../hygu/ui/pages"
import hygu.classes


ResultPage
{
    property ResultsForm pyform

    /**
     * Updates result data conditionally, based on analysis type. Also displays error/warning if found.
     */
    function updateContent()
    {
        // reset all sections
        let images = [plot1, plot2, plot3, plot4, plot5];
        images.forEach((img, i) => clearImage(img));

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
        releaseTypeDisp.text = pyform.is_blowdown.value ? "Blowdown" : "Steady";
        Utils.showChoiceParam(fuel_mix, pyform.fuel_mix);
        Utils.showChoiceParam(rel_phase, pyform.rel_phase);
        Utils.showParam(rel_t, pyform.rel_t);
        Utils.showParam(rel_p, pyform.rel_p);
        Utils.showParam(discharge, pyform.discharge);
        Utils.showChoiceParam(nozzle, pyform.nozzle);

        Utils.showParam(leak_d, pyform.leak_d);
        Utils.showParam(rel_h, pyform.rel_h);
        Utils.showParam(enclosure_h, pyform.enclosure_h);
        Utils.showParam(floor_ceil_a, pyform.floor_ceil_a);
        Utils.showParam(rel_wall_dist, pyform.rel_wall_dist);

        Utils.showParam(ceil_xarea, pyform.ceil_xarea);
        Utils.showParam(ceil_h, pyform.ceil_h);
        Utils.showParam(floor_xarea, pyform.floor_xarea);
        Utils.showParam(floor_h, pyform.floor_h);
        Utils.showParam(vent_flow, pyform.vent_flow);

        Utils.showParam(amb_t, pyform.amb_t);
        Utils.showParam(amb_p, pyform.amb_p);
        Utils.showParam(rel_angle, pyform.rel_angle);
        Utils.showParam(tank_v, pyform.tank_v);
        Utils.showBasicParam(seed, pyform.seed);

        tank_v.visible = pyform.is_blowdown.value;

        maxOverpDisp.text = "" + pyform.acc_max_overp + " kPa at " + pyform.acc_overp_t + " s";

        paramSection.visible = true;

        if (pyform.has_error)
        {
            status.text = "error";
            status.color = color_danger;
            errorMessage.text = pyform.error_message;
            errorSection.visible = true;
            resultSection.contentHeight = paramSection.height + plotSection.height;
            return;
        }

        if (pyform.was_canceled)
        {
            cancellationSection.visible = true;
            resultSection.contentHeight = paramSection.height + plotSection.height;
            return;
        }

        // Analysis complete and successful
        status.text = "complete";
        status.color = color_success;

        updateImage(plot1, pyform.acc_pressure_plot);
        updateImage(plot2, pyform.acc_flam_plot);
        updateImage(plot3, pyform.acc_layer_plot);
        updateImage(plot4, pyform.acc_traj_plot);
        updateImage(plot5, pyform.acc_flow_plot);

        updateHeight();
    }

    /**
     * Updates content height of scroll section based on contents.
     * Do this separately from above so it can be tied to heightChanged event of Flickable.
     */
    function updateHeight()
    {
        if (pyform === null) return;

//        let h = 680;
//        if (pyform.analysis_type.value === 'a')
//        {
//            h = 980;
//        }
//        else
//        {
//            h = 680;
//        }
//        resultSection.contentHeight = h;
    }

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
                id: resultSection
                clip: true
                contentHeight: paramSection.height + plotSection.height + 100
                contentWidth: parent.width
                height: parent.height - y
                width: parent.width
                visible: pyform && pyform.finished

                onHeightChanged: { updateHeight(); }

                Layout.fillHeight: true
                Layout.fillWidth: true

                boundsBehavior: Flickable.StopAtBounds
                boundsMovement: Flickable.FollowBoundsBehavior
                ScrollBar.vertical: ScrollBar {policy: ScrollBar.AlwaysOn; }

                // increase scroll speed
                flickDeceleration: 15000
                ScrollMouseArea {container: resultSection}

                Column {
                    id: paramSection

                    FormSectionHeader {
                        iconSrc: "vials-solid"
                        title: "Analysis Parameters"
                        rWidth: barWidth
                        bottomPad: 8
                    }

                    Row {
                        Text {
                            text: "Release type: "
                            font.pointSize: labelFontSize + 2
                            font.bold: true
                        }
                        Text {
                            id: releaseTypeDisp
                            text: "Blowdown"
                            font.pointSize: labelFontSize + 2
                            font.italic: true
                        }
                    }
                    Item {
                        height: 5
                        width: 5
                    }

                    Row {
                        id: paramRow
                        spacing: 35
                        Column {
                            ResultParamText { id: fuel_mix; }
                            ResultParamText { id: rel_phase; }
                            ResultParamText { id: rel_t; }
                            ResultParamText { id: rel_p; }
                            ResultParamText { id: discharge; }
                            ResultParamText { id: nozzle; }
                        }
                        Column {
                            ResultParamText { id: leak_d; }
                            ResultParamText { id: rel_h; }
                            ResultParamText { id: enclosure_h; }
                            ResultParamText { id: floor_ceil_a; }
                            ResultParamText { id: rel_wall_dist; }
                        }
                        Column {
                            ResultParamText { id: ceil_xarea; }
                            ResultParamText { id: ceil_h; }
                            ResultParamText { id: floor_xarea; }
                            ResultParamText { id: floor_h; }
                            Row {
                                ResultParamText { id: vent_flow; }
                                Text {
                                    font: paramFont;
                                    text: "m<sup>3</sup>/s"
                                    textFormat: Text.RichText
                                }
                            }
                        }
                        Column {
                            ResultParamText { id: amb_t; }
                            ResultParamText { id: amb_p; }
                            ResultParamText { id: rel_angle; }
                            ResultParamText {id: tank_v; }
                            ResultParamText { id: seed; }
                        }
                    }
                }

                Column {
                    id: plotSection
                    anchors.top: paramSection.bottom
                    anchors.topMargin: 30

                    FormSectionHeader {
                        title: "Analysis Results"
                        iconSrc: "chart-line-solid"
                        rWidth: barWidth
                        topPad: 10
                        bottomPad: 8
                    }

                    Row {
                        bottomPadding: 10
                        Text {
                            text: "Maximum overpressure: "
                            font.bold: true
                            font.pointSize: labelFontSize
                        }
                        Text {
                            id: maxOverpDisp
                            font.pointSize: labelFontSize
                        }
                    }
                    Row {
                        bottomPadding: 10
                        Text {
                            text: "Time this occurred (s): ";
                            font.bold: true;
                            font.pointSize: labelFontSize
                        }
                        Text {
                            text: pyform.acc_overp_t;
                            font.pointSize: labelFontSize
                        }
                    }

                    GridLayout {
                        // height: plot1.height * 2 + dataTable.height + 20 + 30
                        columns: 2
                        // rowSpacing: 20
                        // colSpacing: 20

                        SimImage {id: plot1; Layout.preferredWidth: 500; }
                        SimImage {id: plot2; Layout.preferredWidth: 500; }
                        SimImage {id: plot3; Layout.preferredWidth: 500; }
                        SimImage {id: plot4; Layout.preferredWidth: 500; }

                        DataTable {
                            id: dataTable
                            tableW: 600;
                            tableH: 550;
                            headerH: 45;
                            nCols: 5;
                            colW: 120;
                            tableModelRef: tableModel
                            headers: ["Time (s)",
                                    "Combined Pressure (kPa)",
                                    "Depth (m)",
                                    "Concentration (%)",
                                    "Mass Flow Rate (kg/s)",]
                            keys: ['time', 'pres', 'depth', 'conc', 'flow' ]
                            dataSourceCallback: function() { return pyform.acc_data_dicts; };

                            tableViewRef.model: TableModel {
                                id: tableModel
                                TableModelColumn { display: "time" }
                                TableModelColumn { display: "pres" }
                                TableModelColumn { display: "depth" }
                                TableModelColumn { display: "conc" }
                                TableModelColumn { display: "flow" }
                            }
                        }

                        SimImage {id: plot5; Layout.preferredWidth: 500; }

                    }

                    VSpacer { height: 20 }
                }
            }
        }
    }


}
