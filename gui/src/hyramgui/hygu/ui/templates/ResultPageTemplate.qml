/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Dialogs
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Window
import QtQuick.Controls.Material

import "../hygu/ui/components"
import "../hygu/ui/pages"
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
        let images = [plot1, plot2, plot3];
        images.forEach((img, i) => clearImage(img));

        type1Section.visible = false;
        type2Section.visible = false;

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
        showChoiceParam(choice_param, pyform.choice_param);
        showBasicParam(seed, pyform.seed);
        showParam(out_diam, pyform.out_diam);

        paramSection.visible = true;

        if (pyform.has_error)
        {
            status.text = "error";
            status.color = color_danger;
            errorMessage.text = pyform.error_message;
            errorSection.visible = true;
            resultSection.contentHeight = paramSection.height + type1Section.height;
            return;
        }

        if (pyform.was_canceled)
        {
            cancellationSection.visible = true;
            resultSection.contentHeight = paramSection.height + type1Section.height;
            return;
        }

        // Analysis complete and successful
        status.text = "complete";
        status.color = color_success;

        if (pyform.analysis_type.value === 'a')
        {
            updateImage(plot1, pyform.crack_growth_plot);
            updateImage(plot2, pyform.design_curve_plot);
            type2_param.visible = false;
            type1Section.visible = true;
        }

        else if (pyform.analysis_type.value === 'b')
        {
            updateImage(plot3, pyform.ensemble_plot);
            type2_param.visible = true;
            type2Section.visible = true;
        }

        else
        {
            ;
        }

        updateHeight();
    }

    /**
     * Updates content height of scroll section based on contents.
     * Do this separately from above so it can be tied to heightChanged event of Flickable.
     */
    function updateHeight()
    {
        if (pyform === null) return;

        let h = 1000;
        if (pyform.analysis_type.value === 'a')
        {
            h = 980;
        }
        else
        {
            h = 680;
        }
        resultSection.contentHeight = h;
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
                contentHeight: (paramSection.height + type1Section.height +
                    senSection.height +
                    type2Section.height
                )
                contentWidth: parent.width
                height: parent.height - y
                width: parent.width
                visible: pyform && pyform.finished

                onHeightChanged: { updateHeight(); }

                Layout.fillHeight: true
                Layout.fillWidth: true

                boundsBehavior: Flickable.StopAtBounds
                boundsMovement: Flickable.FollowBoundsBehavior
                flickDeceleration: 10000

                ScrollBar.vertical: ScrollBar {
                    policy: ScrollBar.AlwaysOn
                }

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
                            TextEdit { id: analysis_type; text: ""; font: paramFont; readOnly: true; selectByMouse: true; textFormat: Text.RichText}
                            TextEdit { id: seed; text: ""; font: paramFont; readOnly: true; selectByMouse: true; textFormat: Text.RichText}
                        }
                        Column {
                            TextEdit { id: out_diam; text: ""; font: paramFont; readOnly: true; selectByMouse: true; textFormat: Text.RichText}
                        }

                        Column {
                            TextEdit { id: p_max; text: ""; font: paramFont; readOnly: true; selectByMouse: true; textFormat: Text.RichText}
                        }
                    }
                }

                Column {
                    id: type1Section
                    anchors.top: paramSection.bottom
                    anchors.topMargin: 30

                    FormSectionHeader {
                        title: "Analysis Type A Results"
                        iconSrc: "chart-line-solid"
                        rWidth: barWidth
                        topPad: 10
                        bottomPad: 8
                    }

                    Grid {
                        columns: 2
                        SimImage {
                            id: plot1
                        }

                        SimImage {
                            id: plot2
                        }
                    }
                    VSpacer { height: 20 }
                }

                Column {
                    id: type2Section
                    anchors.top: paramSection.bottom
                    anchors.topMargin: 30

                    FormSectionHeader {
                        iconSrc: "chart-line-solid"
                        title: "Analysis Type B Results"
                        rWidth: barWidth
                        topPad: 10
                        bottomPad: 8
                    }

                    Grid {
                        columns: 3

                        SimImage {
                            id: plot3
                            height: 350
                        }
                    }

                    VSpacer { height: 20 }
                }
            }
        }
    }


}
