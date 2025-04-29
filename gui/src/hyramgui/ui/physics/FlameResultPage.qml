
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
import "../components"
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
        let images = [fluxPlot, tempPlot];
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
        Utils.showChoiceParam(fuel_mix, pyform.fuel_mix);
        Utils.showParam(amb_t, pyform.amb_t);
        Utils.showParam(amb_p, pyform.amb_p);

        Utils.showParam(rel_t, pyform.rel_t);
        Utils.showParam(rel_p, pyform.rel_p);
        Utils.showChoiceParam(nozzle, pyform.nozzle);
        Utils.showParam(discharge, pyform.discharge);

        Utils.showParam(leak_d, pyform.leak_d);
        Utils.showParam(rel_angle, pyform.rel_angle);
        Utils.showParam(plume_mass_flow, pyform.plume_mass_flow);
        Utils.showParam(humid, pyform.humid);

        Utils.showStrValue(massFlow, "Mass flow rate", pyform.jet_mass_flow, "kg/s");
        Utils.showStrValue(srad, "Total emitted radiative power", pyform.jet_srad, "W");
        Utils.showStrValue(flameLen, "Visible flame length", pyform.jet_visible_len, "m");
        Utils.showStrValue(radFrac, "Radiant fraction", pyform.jet_radiant_frac, "");

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

        status.text = "complete";
        status.color = color_success;

        updateImage(fluxPlot, pyform.jet_flux_plot);
        updateImage(tempPlot, pyform.jet_temp_plot);

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
                contentHeight: paramSection.height + plotSection.height
                contentWidth: parent.width
                height: parent.height - y
                width: parent.width
                visible: pyform && pyform.finished

                onHeightChanged: { updateHeight(); }

                Layout.fillHeight: true
                Layout.fillWidth: true

                boundsBehavior: Flickable.StopAtBounds
                boundsMovement: Flickable.FollowBoundsBehavior
                flickDeceleration: 15000
                ScrollMouseArea {container: resultSection}

                ScrollBar.vertical: ScrollBar {policy: ScrollBar.AlwaysOn; }

                Column {
                    id: paramSection

                    FormSectionHeader {
                        iconSrc: "vials-solid"
                        title: "Analysis Parameters"
                        rWidth: barWidth
                        bottomPad: 8
                    }

                    Row {
                        id: paramRow
                        spacing: 35
                        Column {
                            ResultParamText { id: fuel_mix; }
                            ResultParamText { id: amb_t;  }
                            ResultParamText { id: amb_p;  }
                            ResultParamText { id: discharge;  }
                        }
                        Column {
                            ResultParamText { id: leak_d;  }
                            ResultParamText { id: rel_t;  }
                            ResultParamText { id: rel_p;  }
                            ResultParamText { id: nozzle;  }
                        }
                        Column {
                            ResultParamText { id: rel_angle;  }
                            ResultParamText { id: humid;  }
                            ResultParamText {
                                id: plume_mass_flow
                                visible: pyform.is_unchoked
                            }
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
                        spacing: 35
                        bottomPadding: 10

                        Column {
                            ResultParamText { id: massFlow;}
                            ResultParamText { id: srad;}
                        }
                        Column {
                            ResultParamText {
                                id: flameLen
                                tipText: "Length of flame along curved centerline trajectory"
                            }
                            ResultParamText {
                                id: radFrac
                                tipText: "Fraction of energy from combustion of fuel emitted as radiant heat"
                            }
                        }
                    }

                    ColumnLayout {
                        // columns: 2
                        // rows: 2
                        spacing: 20
                        // columnSpacing: 20

                        SimImage {id: fluxPlot; height: 600; }

                        SimImage {id: tempPlot; height: 600; }

                        ColumnLayout {
                            Text {
                                text: "Radiative heat flux calculated at specified locations";
                                font.bold: true;
                                font.pointSize: contentFontSize + 1;
                            }

                            DataTable {
                                tableW: 400;
                                tableH: 400;
                                nCols: 4;
                                colW: 100;
                                headers: [ "X (m)", "Y (m)", "Z (m)", "Flux (kW/m<sup>2</sup>)"]
                                keys: ['x', 'y', 'z', 'flux']
                                tableModelRef: tableModel
                                dataSourceCallback: function() { return pyform.jet_flux_data; };
                                // dataSourceCallback: pyform.get_jet_flux_data;

                                tableViewRef.model: TableModel {
                                    id: tableModel
                                    TableModelColumn { display: "x" }
                                    TableModelColumn { display: "y" }
                                    TableModelColumn { display: "z" }
                                    TableModelColumn { display: "flux" }
                                    // rows: JSON.parse(pyform.jet_flux_data)
                                }

                            }

                            // Rectangle {
                            //     Layout.topMargin: 0
                            //     Layout.preferredWidth: 400
                            //     Layout.preferredHeight: 400
                            //     // The background color will show through the cell spacing and therefore be grid line color.
                            //     color: "#f3f3f3"
                            //
                            //     HorizontalHeaderView {
                            //         id: dataHeader
                            //         anchors.left: tableView.left
                            //         anchors.top: parent.top
                            //         width: parent.width
                            //         syncView: tableView
                            //         clip: true
                            //
                            //         model: [ "X (m)", "Y (m)", "Z (m)", "Flux (kW/m<sup>2</sup>)"]
                            //
                            //         delegate: Rectangle {
                            //             implicitWidth: 100
                            //             implicitHeight: 30
                            //             color: "#fafafa"
                            //
                            //             Text {
                            //                 text: modelData
                            //                 anchors.centerIn: parent
                            //                 font.pointSize: labelFontSize
                            //                 font.bold: true
                            //                 wrapMode: Text.WordWrap
                            //                 width: parent.width
                            //                 horizontalAlignment: Text.AlignHCenter
                            //                 textFormat: Text.RichText
                            //             }
                            //         }
                            //     }
                            //
                            //     TableView {
                            //         id: tableView
                            //         height: 350
                            //         width: parent.width
                            //         anchors.top: dataHeader.bottom
                            //         columnSpacing: 1
                            //         rowSpacing: 1
                            //         clip: true
                            //         interactive: false
                            //         boundsBehavior: Flickable.StopAtBounds
                            //         selectionBehavior: TableView.SelectCells
                            //         selectionMode: TableView.ContiguousSelection
                            //         selectionModel: ItemSelectionModel { model: tableView.model }
                            //
                            //         MouseArea
                            //         {
                            //             id: mousearea
                            //             anchors.fill: parent
                            //             //When flickable is flicking and interactive is true,
                            //             //this will have no effect until current flicking ends and interactive set to false
                            //             //so we need to keep interactive false and scroll only with flick()
                            //             preventStealing: true
                            //
                            //             onWheel: function(wheel)
                            //                 {
                            //                     wheel.accepted = true
                            //                     tableView.flick(0, 15 * wheel.angleDelta.y)
                            //                 }
                            //
                            //             //make sure items can only be clicked when view is not scrolling
                            //             onPressed: function(mouse) { mouse.accepted = tableView.moving}
                            //             onReleased: function(mouse) { mouse.accepted = tableView.moving}
                            //             onClicked: function(mouse) { mouse.accepted = tableView.moving}
                            //         }
                            //
                            //
                            //         // Copy selected data to clipboard
                            //         Shortcut {
                            //             sequences: [StandardKey.Copy]
                            //             onActivated: {
                            //                 if (tableView.selectionModel.hasSelection)
                            //                 {
                            //                     let indexes = tableView.selectionModel.selectedIndexes;
                            //                     pyform.copy_to_clipboard(indexes[0].row, indexes[0].column, indexes[1].row, indexes[1].column);
                            //                 }
                            //             }
                            //         }
                            //
                            //         model: TableModel {
                            //             TableModelColumn { display: "x" }
                            //             TableModelColumn { display: "y" }
                            //             TableModelColumn { display: "z" }
                            //             TableModelColumn { display: "flux" }
                            //
                            //             rows: JSON.parse(pyform.jet_flux_data)
                            //         }
                            //
                            //         delegate: Rectangle {
                            //             implicitWidth: 100
                            //             implicitHeight: 25
                            //             color: selected ? Material.color(Material.Blue, Material.Shade50) : "white"
                            //
                            //             required property bool selected
                            //
                            //             Text {
                            //                 text: display
                            //                 anchors.centerIn: parent
                            //                 font.pointSize: labelFontSize - 1
                            //                 wrapMode: Text.WordWrap
                            //             }
                            //
                            //             SelectionRectangle {
                            //                 target: tableView
                            //                 topLeftHandle: Rectangle {
                            //                     width: 12
                            //                     height: 12
                            //                     color: Material.color(Material.Blue, Material.Shade100)
                            //                     visible: SelectionRectangle.control.active
                            //                 }
                            //                 bottomRightHandle: Rectangle {
                            //                     width: 12
                            //                     height: 12
                            //                     color: Material.color(Material.Blue, Material.Shade100)
                            //                     visible: SelectionRectangle.control.active
                            //                 }
                            //             }
                            //         }
                            //     }
                            // }
                        }
                    }
                    VSpacer { height: 20 }
                }
            }
        }
    }


}
