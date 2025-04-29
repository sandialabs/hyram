/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Controls.Material


Rectangle {
    property int w: 500;
    property alias flickRef: plotFlickable
    property alias containerRef: plotContainer

    Layout.preferredWidth: w
    Layout.preferredHeight: plotContainer.height + 30
    Layout.alignment: Qt.AlignVCenter
    color: Material.color(Material.Grey, Material.Shade100)

    Flickable {
        id: plotFlickable
        anchors.fill: parent

        flickableDirection: Flickable.HorizontalFlick
        contentWidth: plotContainer.width + 20
        ScrollBar.horizontal: ScrollBar { policy: ScrollBar.AlwaysOn; }
        clip: true

        RowLayout {
            id: plotContainer
            spacing: 10
            anchors.left: parent.left
            anchors.leftMargin: 10
            // center vertically while accounting for scrollbar
            anchors.top: parent.top
            anchors.topMargin: 10
            clip: true

            // sample plot component
            // SimImage {id: plot1; height: 350; }
        }
    }
}
