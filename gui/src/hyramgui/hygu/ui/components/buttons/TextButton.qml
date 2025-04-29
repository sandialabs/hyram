/*
 * Copyright 2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Window
import QtQuick.Controls.Material


Button {
    property string btnText;
    property string tipText;

    flat: true
    width: 40
    height: 50

    Text {
        anchors.verticalCenter: parent.verticalCenter
        anchors.horizontalCenter: parent.horizontalCenter
        font.bold: true
        font.pixelSize: 18
        text: btnText
        color: Material.color(Material.Grey)
    }

    background: Rectangle {
        color: Material.color(Material.Blue)
        opacity: 0
    }

    hoverEnabled: true
    ToolTip {
        delay: 300
        timeout: 4000
        visible: parent.hovered
        text: tipText
        y: 40
        x: 30
    }
}
