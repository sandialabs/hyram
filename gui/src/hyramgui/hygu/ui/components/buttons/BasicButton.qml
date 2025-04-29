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
    property string tooltip;
    property string btnText: "Button";

    Material.background: bgColor;
    Material.roundedScale: Material.SmallScale
    Material.elevation: enabled ? 1 : 0

    height: parent.height - 2  // possible recursive layout error?
    width: height *(9/8) < 100? 100 : height * (9/8)
    // width: btnIcon.width + 4 + btnTextElem.width

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
        font.pixelSize: 16
        wrapMode: Text.WordWrap
        width: 100
    }


    hoverEnabled: tooltip !== ""
    ToolTip {
        enabled: tooltip !== ""
        delay: 400
        timeout: 5000
        visible: parent.hovered
        text: tooltip
    }
}
