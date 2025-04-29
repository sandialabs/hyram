/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick 2.12

// increase scroll speed
MouseArea
{
    property var container
    anchors.fill: parent

    onWheel: function(wheel)
        {
            wheel.accepted = true;
            container.flick(0, 130 * wheel.angleDelta.y);
        }
}

