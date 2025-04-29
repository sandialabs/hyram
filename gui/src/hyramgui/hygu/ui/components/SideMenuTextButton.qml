/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Controls
import QtQuick.Layouts
import QtQuick.Controls.Material

import "buttons"


Button {
    property color bgColor: "#EBF4F4";
    property color btnIconColor: color_primary;
    property string img: "";
    property string btnText: "Button";

    topInset: 1
    bottomInset: 1
    Material.background: bgColor;
    Material.roundedScale: Material.NotRounded
    flat: true
    implicitWidth: parent.width
    width: parent.width
    implicitHeight: 70

    AppIcon {
        enabled: img !== ""
        id: btnIcon
        source: img
        anchors.verticalCenter: parent.verticalCenter
        //anchors.horizontalCenter: parent.left
        anchors.left: parent.left
        anchors.leftMargin: 12
        iconColor: enabled ? btnIconColor : color_disabled
    }

    Text {
        id: btnTextElem
        anchors.verticalCenter: parent.verticalCenter
        // anchors.horizontalCenter: parent.horizontalCenter
        anchors.left: btnIcon.right
        anchors.leftMargin: 8
        horizontalAlignment: Text.AlignLeft
        text: btnText
        font.pixelSize: 16
        wrapMode: Text.WordWrap
        width: 120
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
