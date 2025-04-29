/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Controls
import QtQuick.Controls.Material

import hygu.classes

import "buttons/"

Rectangle {
    property string name
    property ResultsForm fm
    property int qIndex
    property int a_id: fm.analysis_id
    property double startTime

    id: queueItem
    radius: 5.0
    width: parent?.width ?? 0
    height: 38
    color: "#e0e0e0"

    function updateIcons()
    {
        iconWaiting.visible = false;
        iconProgressing.visible = false;
        iconFinished.visible = false;
        iconError.visible = false;
        let tipText = "";

        if (!fm.started)
        {
            iconWaiting.visible = true;
            tipText = "Analysis queued";
        }
        else if (fm.started && !fm.finished)
        {
            iconProgressing.visible = true;
            tipText = "Analysis in-progress";
        }
        else if (fm.finished && !fm.has_error)
        {
            iconFinished.visible = true;
            tipText = "Analysis complete - click to view results";
        }
        else if (fm.finished && fm.has_error)
        {
            iconError.visible = true;
            tipText = "Error during analysis - click to view more info";
        }

        tooltip.text = tipText;
    }

    Connections {
        target: fm
        function onStartedChanged(val) { updateIcons(); }
        function onFinishedChanged(val) {
            updateIcons();
            timer.stop();
        }
    }

    Component.onCompleted: {
        startTime = new Date().getTime();
    }

    HoverHandler {
        id: mouse
    }

    MouseArea {
        id: ma
        anchors.fill: parent
        onClicked: {
            resultContainer.show(fm, qIndex);
        }
        hoverEnabled: true
        propagateComposedEvents: true

    }

    ToolTip {
        id: tooltip
        delay: 400
        timeout: 5000
        visible: ma.containsMouse
        text: "Analysis in queue"
    }

    Text {
        id: nameLabel
        anchors.verticalCenter: parent.verticalCenter
        anchors.left: parent.left
        leftPadding: 10
        horizontalAlignment: Text.AlignLeft
        font.pointSize: 14
        text: '' + fm.name_str
        width: 200
        elide: "ElideRight"
    }


    Text {
        anchors.verticalCenter: parent.verticalCenter
        anchors.right: statusIcon.left

        id: timeLabel
        rightPadding: 30
        horizontalAlignment: Text.AlignLeft
        font.pointSize: 12
        text: ''
    }

    Timer {
        id: timer
        interval: 1000; running: true; repeat: true
        onTriggered: {
            var currTime = new Date().getTime();
            var diff = currTime - startTime;  // milliseconds
            var diffStr = new Date(diff).toISOString().slice(11,19);
            timeLabel.text = diffStr;
        }
    }


    Item {
        id: statusIcon
        anchors.verticalCenter: parent.verticalCenter
        anchors.right: parent.right
        width: 32


        AppIcon {
            id: iconWaiting
            visible: !fm.started
            source: 'ellipsis-solid'
            anchors.verticalCenter: parent.verticalCenter
            anchors.right: parent.right
            anchors.rightMargin: 12
        }

        BusyIndicator {
            id: iconProgressing
            visible: fm.started && !fm.finished
            height: queueItem.height - 10
            running: true
            anchors.verticalCenter: parent.verticalCenter
            anchors.right: parent.right
        }

        AppIcon {
            id: iconFinished
            visible: fm.finished && !fm.has_error
            source: 'check-solid'
            iconColor: Material.color(Material.Green)
            anchors.rightMargin: 12
            anchors.verticalCenter: parent.verticalCenter
            anchors.right: parent.right
        }

        AppIcon {
            id: iconError
            visible: fm.finished && fm.has_error
            source: 'circle-exclamation-solid'
            iconColor: Material.color(Material.Red)
            anchors.rightMargin: 12
            anchors.verticalCenter: parent.verticalCenter
            anchors.right: parent.right
        }
    }

    Rectangle {
        id: progressDrawer
        parent: queueItem
        width: parent.width
        height: parent.height
        color: "#e0e0e0"
        visible: !fm.finished && (ma.containsMouse || cancelBtnMa.containsMouse)

        Label {  // same as above
            anchors.verticalCenter: parent.verticalCenter
            anchors.left: parent.left
            leftPadding: 10
            horizontalAlignment: Text.AlignLeft
            font.pointSize: 14
            text: '' + fm.name_str
            width: 260
            elide: "ElideRight"
        }
        IconButton {
            id: cancelBtn
            anchors.rightMargin: 12
            anchors.verticalCenter: parent.verticalCenter
            anchors.right: parent.right
            img: 'xmark-solid'
            tooltip: ("Cancel analysis")
            bgColor: Material.color(Material.Red, Material.Shade300)

            MouseArea {
                id: cancelBtnMa
                anchors.fill: parent
                anchors.centerIn: parent
                hoverEnabled: true

                onClicked: function() {
                    // tell backend to cancel analysis, then update frontend immediately.
                    app_form.cancel_analysis(fm.analysis_id);
                    queueListView.analysisCanceled(qIndex);
                }
            }
        }

    }

    Rectangle {
        id: completionDrawer
        parent: queueItem
        width: parent.width
        height: parent.height
        color: "#e0e0e0"
        visible: fm.finished && (ma.containsMouse || deleteBtnMa.containsMouse)

        Label {
            text: "View results"
            font.italic: true
            anchors.centerIn: parent
            anchors.verticalCenter: parent.verticalCenter
            anchors.left: parent.left
            leftPadding: 10
            horizontalAlignment: Text.AlignLeft
            font.pointSize: 16
        }
        IconButton {
            id: deleteBtn
            anchors.rightMargin: 12
            anchors.verticalCenter: parent.verticalCenter
            anchors.right: parent.right
            img: 'trash-solid'
            tooltip: ("Delete analysis results")
            bgColor: Material.color(Material.Red, Material.Shade300)

            MouseArea {
                id: deleteBtnMa
                anchors.fill: parent
                anchors.centerIn: parent
                hoverEnabled: true

                onClicked: function() {
                    queueListView.removeItem(qIndex);
                }
            }
        }

    }


}

