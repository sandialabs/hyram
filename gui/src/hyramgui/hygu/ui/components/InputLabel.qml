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


Text {
    property int w: 100;
    property string tipText;

    Layout.preferredWidth: w
    width: w
    horizontalAlignment: Text.AlignLeft
    font.pointSize: labelFontSize
    wrapMode: Text.WordWrap

    MouseArea {
        id: ma
        anchors.fill: parent
        hoverEnabled: true
    }

    ToolTip {
        delay: 200
        timeout: 3000
        visible: tipText ? ma.containsMouse : false
        text: tipText
    }
}
