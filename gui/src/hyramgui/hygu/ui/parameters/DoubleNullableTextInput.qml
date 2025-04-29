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
    property string field: "value";  // reference to associated parameter field, e.g. 'value' or 'a'
    property double max: Infinity
    property double min: -Infinity
    property var paramRef: param ?? null
    property alias tooltip: ttip

    function refresh()
    {
        // update text, check explicitly for null fields since qml converts None from python to 0)
        if (field === 'd' && paramRef.d_is_null) {
            text = '';
        }
        else if (field === 'c' && paramRef.c_is_null) {
            text = '';
        }
        else if (!paramRef || Utils.isNullish(paramRef[field])) {
            text = '';

        } else if (paramRef && paramRef.is_null) {
            // optional analysis parameter that can be null (i.e. not a distribution parameter)
            text = '';
        } else {
            text = paramRef[field];
        }
    }

    Layout.alignment: Qt.AlignCenter
    Layout.maximumWidth: 100
    Material.containerStyle: Material.Filled
    bottomPadding: 5
    horizontalAlignment: Text.AlignHCenter
    hoverEnabled: true
    implicitHeight: 24
    topPadding: 5

    onEditingFinished: function () {
        // custom validation
        if (Utils.isNullish(text)) {
            paramRef.set_null(field);
        }
        if (length > 0 && text >= min && text <= max) {
                paramRef[field] = text;
        } else {
            refresh();  // restore value
        }
    }

    ToolTip {
        id: ttip
        delay: 400
        text: "Enter a value between " + min + " and " + max + ". Leave blank to indicate infinity.";
        timeout: 5000
        visible: parent.hovered
    }
}
