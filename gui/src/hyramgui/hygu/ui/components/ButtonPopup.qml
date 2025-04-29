/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */

import QtQuick 2.12
import QtQuick.Layouts
import QtQuick.Controls 2.12

import "buttons"

IconButtonFlat {
    property alias contentRef: popupContent
    property string tipText: "";
    property int w: 720;
    property int h: 600;

    Layout.preferredWidth: 14
    Layout.preferredHeight: 14
    Layout.leftMargin: 2
    tooltip: tipText
    img: "circle-question-solid"
    onClicked: popup.open()

    Popup {
        id: popup
        parent: Overlay.overlay
        x: Math.round((parent.width - width) / 2)
        y: Math.round((parent.height - height) / 2)
        width: w
        height: h
        modal: true
        dim: true

        contentItem: Rectangle {
            width: parent.width
            height: parent.height
            anchors.fill: parent
            anchors.centerIn: parent
            color: "#fdfdfd"

            Rectangle {
                id: popupContent
                width: parent.width - 20
                height: parent.height - 20
                anchors.margins: 10
                anchors.centerIn: parent
                color: "white"

            }

            // add content by parenting it like so:
            // parent: buttonPopup.contentRef
        }
    }
}
