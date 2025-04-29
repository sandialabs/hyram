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

import "../utils.js" as Utils


TextField {
    property alias tooltip: ttip;
    property bool useLimits: true;
    property double min: -Infinity;
    property double max: Infinity;
    property int w: 100;
    property double value;
    property bool isNull: true;

    Material.containerStyle: Material.Filled
    implicitHeight: 24
    topPadding: 5
    bottomPadding: 5
    Layout.alignment: Qt.AlignCenter
    Layout.maximumWidth: w
    Layout.preferredWidth: w
    horizontalAlignment: Text.AlignHCenter

    text: isNull ? "" : value

    // store new value if within limits and valid

    function updateValue()
    {
        if (Utils.isNullish(text))
        {
            isNull = true;
        }
        else if (length > 0)
        {
            var v = parseFloat(text);
            if (v !== undefined && v !== null && !isNaN(v) && (useLimits || v >= min && v <= max))
            {
                isNull = false;
                value = v;
            }
            else
            {
                text = "";
                value = null;
                isNull = true;
            }
        }
    }

    onEditingFinished: { updateValue(); }
    onActiveFocusChanged: { updateValue(); }

    hoverEnabled: true
    ToolTip {
        id: ttip
        delay: 400
        timeout: 5000
        visible: parent.hovered
        text: useLimits ? "Enter a value between " + min + " and " + max : "Enter a value"
    }
}
