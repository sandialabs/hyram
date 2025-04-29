/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick 2.12
import QtQuick.Controls 2.12


TextEdit {
    property string tipText;

    text: "";
    font: paramFont;
    readOnly: true;
    selectByMouse: true;
    textFormat: Text.RichText
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
