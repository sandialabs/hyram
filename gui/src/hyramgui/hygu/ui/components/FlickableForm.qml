/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick 2.12
import QtQuick.Layouts
import QtQuick.Controls 2.12


Flickable {
    property int h: 560;

    id: form
    Layout.leftMargin: 15
    Layout.fillHeight: true
    Layout.fillWidth: true
    Layout.minimumWidth: 400
    height: parent.height
    Layout.minimumHeight: h

    flickableDirection: Flickable.VerticalFlick
    Behavior on contentY { NumberAnimation { duration: 200; } }
    boundsBehavior: Flickable.StopAtBounds
    boundsMovement: Flickable.FollowBoundsBehavior
    clip: true
    ScrollBar.vertical: ScrollBar { policy: ScrollBar.AsNeeded; }

    // increase scroll speed
    flickDeceleration: 15000
    ScrollMouseArea {container: form}
}
