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
    property color bgColor: formBgColor;
    property color btnIconColor: color_primary;
    property string img: "";
    property string btnText: "Button";

    topInset: 0
    bottomInset: 0
    Material.background: bgColor;
    Material.roundedScale: Material.NotRounded
    flat: true
    implicitWidth: parent.width
    Layout.topMargin: 0
    implicitHeight: 24

    AppIcon {
        enabled: img !== ""
        id: btnIcon
        source: img
        anchors.verticalCenter: parent.verticalCenter
        anchors.left: parent.left
        anchors.leftMargin: 16
        iconColor: enabled ? btnIconColor : color_disabled
        icon.width: isWindows ? 16 : 20
        icon.height: isWindows ? 16 : 20
    }

    Text {
        id: btnTextElem
        anchors.verticalCenter: parent.verticalCenter
        anchors.left: btnIcon.right
        anchors.leftMargin: 3
        horizontalAlignment: Text.AlignLeft
        text: btnText
        font.pixelSize: 13
        width: 100
    }
}
