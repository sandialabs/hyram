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


Popup {
    property int w: 384
    property int h: 128
    property bool allowClose: true
    property bool isCentered: false
    property string header
    property string content

    x: parent.width * 0.5 - w / 2
    y: parent.height * 0.5 - h / 2
    width: w
    height: h
    modal: true
    focus: true
    closePolicy: allowClose ? Popup.CloseOnEscape | Popup.CloseOnPressOutside : Popup.NoAutoClose;

    ColumnLayout {
        // spacing: 5
        anchors.fill: parent

        Text {
            Layout.fillWidth: true
            Layout.topMargin: 0
            topPadding: 1
            font.pointSize: 18
            text: header
            horizontalAlignment: isCentered ? Text.AlignHCenter : Text.AlignLeft;
        }

        Text {
            Layout.fillWidth: true
            font.pointSize: contentFontSize
            wrapMode: Text.WordWrap
            Layout.maximumWidth: parent.width * 0.95
            text: content
            horizontalAlignment: isCentered ? Text.AlignHCenter : Text.AlignLeft;
        }
    }
}
