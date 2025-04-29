/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick 2.12
import QtQuick.Layouts
import QtQuick.Controls.Material 2.12


Column {
    property string title;
    property alias titleRef: textId;
    property alias rectRef: rectId;
    property int rWidth: typeof(sectionW) === "undefined" ? 800 : sectionW;
    property int bottomPad: 0;
    property int topPad: 24;
    property int fontSize: 15;
    property int hAlign: Text.AlignLeft;

    property string iconSrc: "";

    Layout.topMargin: topPad

    Row {
        spacing: 4

        AppIcon {
            enabled: iconSrc !== "";
            visible: iconSrc !== "";  // don't layout
            source: iconSrc
            iconColor: Material.color(Material.Grey)
            anchors.verticalCenter: parent.verticalCenter
        }
        Text {
            id: textId
            text: title
            horizontalAlignment: hAlign
            anchors.verticalCenter: parent.verticalCenter
            font.pointSize: fontSize
            font.bold: true
            color: headerColor;
        }

    }

    Rectangle {
        id: rectId
        width: rWidth
        height: 2
        color: Material.color(Material.Blue)
        opacity: 0.4
    }

    VSpacer {
        height: bottomPad
    }
}

