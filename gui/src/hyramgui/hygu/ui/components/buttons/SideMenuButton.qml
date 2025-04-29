/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Controls
import QtQuick.Controls.Material


import "../"


Button {
    property color bgColor: btnColorDefault;
    property color btnIconColor: color_primary;
    property string img: "";
//    property string tooltip;
    property string btnText: "Button";

    topInset: 1
    bottomInset: 1
    Material.background: bgColor;
    Material.roundedScale: Material.NotRounded
    Material.elevation: 0
    implicitWidth: parent.width
    implicitHeight: 54

    AppIcon {
        enabled: img !== ""
        id: btnIcon
        source: img
        anchors.verticalCenter: parent.verticalCenter
        anchors.horizontalCenter: parent.horizontalCenter
        iconColor: enabled ? btnIconColor : color_disabled
    }

    Text {
        id: btnTextElem
        anchors.verticalCenter: parent.verticalCenter
        anchors.horizontalCenter: parent.horizontalCenter
        horizontalAlignment: Text.AlignHCenter
        text: btnText
        font.pixelSize: 14
        wrapMode: Text.WordWrap
        width: 110
    }


    hoverEnabled: true
//    ToolTip {
//        enabled: tooltip !== ""
//        delay: 400
//        timeout: 5000
//        visible: parent.hovered
//        text: tooltip
//    }
}
