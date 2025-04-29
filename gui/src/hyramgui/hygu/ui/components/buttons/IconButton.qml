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
    property alias tooltipRef: ttip;
    property color bgColor: btnColorDefault;
    property color btnIconColor: color_primary;
    property string img;
    property string tooltip;

    font.pixelSize: 14
    Material.background: bgColor;
    Material.roundedScale: Material.SmallScale
    Material.elevation: enabled ? 1 : 0
    height: parent.height - 2
    width: height * 9/8

    AppIcon {
        id: btnIcon
        source: img
        anchors.verticalCenter: parent.verticalCenter
        anchors.horizontalCenter: parent.horizontalCenter
        iconColor: enabled ? btnIconColor : color_disabled
    }

    hoverEnabled: true
    ToolTip {
        id: ttip
        delay: 400
        timeout: 5000
        visible: parent.hovered && tooltip !== ""
        text: tooltip
    }
}
