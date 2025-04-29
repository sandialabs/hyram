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
    property bool isCentered: false
    property string fpath
    property int h: 400

    closePolicy: Popup.CloseOnEscape | Popup.CloseOnPressOutside
    focus: false
    modal: false
    x: 20
    y: 20

    ColumnLayout {
        anchors.fill: parent

        Image {
            id: img
            fillMode: Image.PreserveAspectFit
            height: h
            source: fpath ? 'file:' + fpath : ""
            sourceSize.height: height
        }
    }
}
