/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick 2.12
import QtQuick.Layouts
import QtQuick.Controls 2.12
import QtQuick.Dialogs
import QtQuick.Window
import QtQuick.Controls.Material 2.12

import "buttons"


Rectangle {
    id: alert
    property string msg;
    property int level: 1;
    property alias textRef: txt;

    visible: true
    Layout.preferredWidth: 500
    Layout.leftMargin: 20
    Layout.preferredHeight: level === 1 ? 0 : 24 * txt.lineCount
    radius: 5
    color: level === 3 ? color_warning : color_danger
    Behavior on Layout.preferredHeight { NumberAnimation { duration: 100 }}

    Text {
        id: txt
        color: level === 3 ? color_text_warning : color_text_danger
        anchors.margins: 5
        anchors.fill: parent
        anchors.verticalCenter: parent.verticalCenter
        horizontalAlignment: Text.AlignLeft
        verticalAlignment: Text.AlignVCenter
        text: msg
        font.pointSize: labelFontSize
        font.bold: true
        wrapMode: Text.WordWrap
        maximumLineCount: 2
    }
}
