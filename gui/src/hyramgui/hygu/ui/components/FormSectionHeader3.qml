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
    property int rWidth: 800;
    property int bottomPad: 0;
    property int topPad: 20;
    property int fontSize: 13;

    Layout.topMargin: topPad

    Row {
        Text {
            text: title
            horizontalAlignment: Text.AlignLeft
            anchors.verticalCenter: parent.verticalCenter
            font.pointSize: fontSize
            font.bold: true
            color: headerColor;
        }

    }

    VSpacer {
        height: bottomPad
    }
}

