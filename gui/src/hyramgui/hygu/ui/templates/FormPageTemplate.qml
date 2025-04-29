/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick 2.12
import QtQuick.Layouts
import QtQuick.Controls 2.12
import QtQuick.Dialogs
import QtQuick.Window
import QtQuick.Controls.Material 2.12

import "../hygu/ui/components"
import "../hygu/ui/parameters"
import "../hygu/ui/pages"
import hygu.classes


FormPage {
    function updateScrollButtons()
    {
        // update button highlighting
        let cy = paramContainer.contentY;
        let btns = [scrollBtn1, scrollBtn2, scrollBtn3];

        for (var i=0; i < btns.length; ++i)
        {
            changeButtonTextColor(btns[i]);
        }

        if (cy >= section3.y)
        {
            changeButtonTextColor(scrollBtn3, 0.1);
        }
        else if (cy >= section2.y)
        {
            changeButtonTextColor(scrollBtn2, 0.1);
        }
        else
        {
            changeButtonTextColor(scrollBtn1, 0.1);
        }
    }


    function refreshForm()
    {
        let val = param_c.value;

        // update readonly parameters
        param.refresh();

        // param-specific alerts
        if (param.value >= 1)
        {
            param.showAlert("Alert");
        }
    }


    ColumnLayout {
        width: parent.width - 50  // account for scrollbuttons

        // Form header
        Text {
            font.pointSize: header1FontSize
            id: formHeader
            font.weight: 600
            Layout.leftMargin: 14
            text: "Form Title"
        }

        Rectangle {
            height: 2
            Layout.fillWidth: true
            Layout.leftMargin: 16
            Layout.rightMargin: 4
            color: Material.color(Material.Blue, Material.Shade400)
        }

        Item {
            id: inputSpacer2
            Layout.preferredHeight: 4
        }

        Flickable
        {
            id: paramContainer
            Layout.leftMargin: 20
            Layout.fillHeight: true
            Layout.fillWidth: true
            Layout.minimumWidth: 400
            contentWidth: 840
            contentHeight: paramCols.height + 20 + 120
            height: 580

            flickableDirection: Flickable.HorizontalAndVerticalFlick
            boundsBehavior: Flickable.StopAtBounds
            boundsMovement: Flickable.FollowBoundsBehavior
            flickDeceleration: 10000
            clip: true

            ScrollBar.vertical: ScrollBar {
                policy: ScrollBar.AsNeeded
            }

            ScrollBar.horizontal: ScrollBar {
                policy: ScrollBar.AsNeeded
            }

            Component.onCompleted: {
                updateScrollButtons();
            }

            onContentYChanged: {
                updateScrollButtons();
            }

            // ==========================
            // ==== Parameter Inputs ====
            Item {
                id: paramColContainer
                Layout.fillHeight: true
                height: paramCols.height

                ColumnLayout {
                    id: paramCols

                    FormSectionHeader {
                        id: section1
                        title: "Analysis Settings";
                        Layout.topMargin: 10
                        iconSrc: 'gear-solid'
                    }

                    StringParamField {
                        param: name_c
                        inputLength: 240
                        tipText: "Optionally enter an alphanumeric name"
                    }
                    ChoiceParamField {
                        param: choice_c
                        tipText: "Select an analysis types"
                    }
                    IntField {
                        param: seed_c
                        tipText: "Integer used to generate the random seed enabling regeneration of results"
                    }

                    FormSectionHeader {
                        id: section2
                        title: "Section 2 inputs"
                        iconSrc: 'chart-simple-solid'
                    }
                    ParameterInput {
                        param: param_2_c
                        tipText: "Train direction"
                    }

                    FormSectionHeader {
                        id: section3
                        title: "Section 3"
                        iconSrc: 'shapes-solid'
                    }
                    ReadonlyParameter {
                        id: val_ro
                        param: val_ro_c
                        tipText: "% ratio"
                    }
                    BoolField {
                        id: plotSelector
                        param: do_plot1_c
                    }
                }
            }
        }

        Item {
            Layout.fillHeight: true
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
                Layout.preferredWidth: 120
                Layout.alignment: Qt.AlignCenter
                Layout.leftMargin: 48
                Layout.bottomMargin: 8
                Material.roundedScale: Material.SmallScale
                Material.accent: Material.Blue
                highlighted: true

                onClicked: {
                    forceActiveFocus();
                    app_form.request_analysis();
                }

                Row {
                    anchors.horizontalCenter: parent.horizontalCenter
                    anchors.verticalCenter: parent.verticalCenter
                    spacing: 0

                    AppIcon {
                        anchors.verticalCenter: parent.verticalCenter
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

            Rectangle {
                id: alertSection
                visible: false
                Layout.alignment: Qt.AlignLeft
                Layout.leftMargin: 10
                color: color_danger
                radius: 5
                Layout.preferredHeight: 30
                Layout.preferredWidth: alertContents.width

                Row {
                    id: alertContents
                    spacing: 5
                    anchors.verticalCenter: parent.verticalCenter
                    leftPadding: 8

                    AppIcon {
                        id: alertIcon
                        source: 'circle-exclamation-solid'
                        iconColor: color_text_danger
                        anchors.verticalCenter: parent.verticalCenter
                    }
                    TextEdit {
                        id: alertText
                        text: "Test alert NOTIFICATION lorem ipsum"
                        rightPadding: 10
                        color: color_text_danger
                        readOnly: true
                        selectByMouse: true
                        font.pointSize: 12
                        font.bold: true
                        anchors.verticalCenter: parent.verticalCenter
                    }
                }

            }
        }

    }
    // ========================
    // ==== Scroll Buttons ====
    Column {
        id: scrollBtns
        width: 25
        height: 300
        anchors.top: parent.top
        anchors.topMargin: formHeader.height + 2 + inputSpacer2.height + 10
        anchors.right: parent.right
        anchors.rightMargin: 18

        spacing: 0

        SimpleScrollButton {
            id: scrollBtn1
            tipText: "Scroll to Analysis Settings"
            iconSrc: 'gear-solid'
            onClicked: { paramContainer.contentY = section1.y; }
        }

        SimpleScrollButton {
            id: scrollBtn2
            tipText: "Scroll to probabilistic section"
            onClicked: { paramContainer.contentY = section2.y; }
            iconSrc: 'chart-simple-solid'
        }

        SimpleScrollButton {
            id: scrollBtn3
            tipText: "Scroll to method inputs"
            onClicked: { paramContainer.contentY = section3.y; }
            iconSrc: 'shapes-solid'
        }
    }
}
