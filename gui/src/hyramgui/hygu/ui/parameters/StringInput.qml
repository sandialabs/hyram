/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Controls.Material 2.12

import hygu.classes


TextField {
    property int w: 100;
    property alias tooltip: ttip;

    Material.containerStyle: Material.Filled
    implicitHeight: 24
    topPadding: 5
    bottomPadding: 5
    Layout.alignment: Qt.AlignCenter
    Layout.maximumWidth: w
    width: w
    horizontalAlignment: Text.AlignHCenter

    text: ''

    hoverEnabled: true
    ToolTip {
        id: ttip
        delay: 400
        timeout: 5000
        visible: parent.hovered
    }
}
