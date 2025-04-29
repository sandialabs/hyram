/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Controls 2.12

Item
{
    id: appIcon

    property string source
    property string iconColor
    property alias icon: btn.icon

    implicitWidth: btn.icon.width
    implicitHeight: btn.icon.height / Screen.devicePixelRatio


    Button
    {
        id: btn
        anchors.centerIn: parent
        height: icon.height

        icon.source: (source !== "") ?  '../../resources/icons/' + parent.source + '.svg' : ""
        icon.color: iconColor ? iconColor : color_primary

        // disable button functionality
        enabled: false
        flat: true
        MouseArea {
            enabled: false
            hoverEnabled: false
            scrollGestureEnabled: false
        }
    }
}
