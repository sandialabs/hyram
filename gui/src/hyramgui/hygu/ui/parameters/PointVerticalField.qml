/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Window
import QtQuick.Controls.Material

import hygu.classes
import "../components"
import "../components/buttons"


Rectangle {
    property bool watchChanges: true;
    property bool isAlive: true;  // indicates when component is being destroyed, which isn't instant.
    property int index;

    id: rect
    width: container.width
    height: container.height
    Layout.topMargin: 4
    Layout.bottomMargin: 10
    radius: 5
    border.width: 1
    border.color: "#b4b4b4"
    color: "#F9F7F9"

    // clip: true  // hide extra content when collapsed
    Behavior on height { NumberAnimation { duration: 200}}

    Connections {target: vx; function onEditingFinished() { handleDataChanged(); } }
    Connections {target: vx; function onActiveFocusChanged() { handleDataChanged(); } }
    Connections {target: vy; function onEditingFinished() { handleDataChanged(); } }
    Connections {target: vy; function onActiveFocusChanged() { handleDataChanged(); } }
    Connections {target: vz; function onEditingFinished() { handleDataChanged(); } }
    Connections {target: vz; function onActiveFocusChanged() { handleDataChanged(); } }

    function getData() {
        return [vx.val, vy.val, vz.val];
    }

    function handleDataChanged()
    {
        if (watchChanges && isAlive)
        {
            setModelPointData(index);
        }
    }

    function setFromModel(data, idx)
    {
        var watcher = watchChanges;
        watchChanges = false;

        index = idx;
        vx.val = data[0];
        vy.val = data[1];
        vz.val = data[2];

        watchChanges = watcher;
    }

    ColumnLayout {
        id: container
        width: 50
        height: 135

        Item {
            Layout.preferredHeight: 5
            Layout.preferredWidth: 5
        }
        FloatInput {id: vx; w: 44; leftPadding: 4; rightPadding: 4; }
        FloatInput {id: vy; w: 44; leftPadding: 4; rightPadding: 4; }
        FloatInput {id: vz; w: 44; leftPadding: 4; rightPadding: 4; }

        Item {
            Layout.fillHeight: true
        }

        IconButtonFlat {
            id: deleteBtn
            width: 36
            Layout.preferredHeight: 36
            Layout.preferredWidth: 33
            Layout.alignment: Qt.AlignCenter
            img: 'xmark-solid'
            tooltip: "Remove point"
            onClicked: function() {
                removePoint(index);
            }
        }
    }
}

