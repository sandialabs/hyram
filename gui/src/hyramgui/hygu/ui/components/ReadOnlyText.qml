/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Layouts
import QtQuick.Controls


Text {
    property string field;  // reference to associated parameter field, e.g. 'value' or 'a'
    property var paramRef: param ?? null;
    property string tipText;

    horizontalAlignment: Text.AlignHCenter
    // Layout.leftMargin: 4
    Layout.preferredWidth: 100
    Layout.maximumWidth: 100
    font.pointSize: labelFontSize
    textFormat: Text.RichText

    text: paramRef ? paramRef[field] : ''

    ToolTip {
        delay: 200
        timeout: 5000
        visible: tipText ? ma.containsMouse : false
        text: tipText
    }

    // for tooltip hover
    MouseArea {
        id: ma
        anchors.fill: parent
        hoverEnabled: true
    }

}
