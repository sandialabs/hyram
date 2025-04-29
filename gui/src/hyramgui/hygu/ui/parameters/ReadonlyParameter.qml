/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Window
import QtQuick.Controls.Material 2.12

import "../components"
import hygu.classes


Item {
    property UncertainFormField param;
    property string tipText;

    id: paramContainer
    Layout.topMargin: 8

    Component.onCompleted:
    {
        refresh();
    }

    function refresh()
    {
        paramContainer.Layout.preferredHeight = 14;
        paramLabel.color = color_primary;

        // invalid calculation stored as np.inf
        if (param.value === Number.POSITIVE_INFINITY)
        {
            valueInput.text = "-";
            unitLabel.text = "";
            return;
        }

        let val = param.value;
        if (val > 1)
        {
            valueInput.text = Math.round(val * 100) / 100;
        }
        else {
            valueInput.text = Math.round(val * 1000) / 1000;
        }

        let units = param.get_unit_disp;
        if (units === "-" || units === null)
        {
            units = "";
        }
        unitLabel.text = units;
    }

    function showAlert(msg, asWarning)
    {
        if (asWarning === undefined) asWarning = false;

        let clr = color_text_danger;
        if (asWarning) clr = color_text_warning;

        alertIcon.iconColor = clr;
        alertMsg.color = clr;

        // Note: alert handling done in parent form.
        alertMsg.text = msg;
        alertMsg.visible = true;
        alertIcon.visible = true;
    }

    function hideAlert()
    {
        alertIcon.visible = false;
        alertMsg.visible = false;
    }


    Row
    {
        id: paramInputRow

        Component.onCompleted:
        {
            refresh();
        }

        GridLayout {
            id: paramGrid
            rows: 1
            columns: 7
            flow: GridLayout.TopToBottom

            Connections {
                target: param
                function onInputTypeChanged() { refresh(); }
                function onModelChanged() { refresh(); }
                function onUncertaintyChanged() { refresh(); }
            }

            Text {
                id: paramLabel
                text: param?.label_rtf ?? ''
                Layout.preferredWidth: paramLabelWidth
                horizontalAlignment: Text.AlignLeft
                font.pointSize: labelFontSize
                font.italic: true
                textFormat: Text.RichText

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

            Text {
                id: valueInput
                Layout.leftMargin: 4
                font.pointSize: labelFontSize
                textFormat: Text.RichText
            }

            Text {
                id: unitLabel
                Layout.preferredWidth: 20
                font.pointSize: labelFontSize
                textFormat: Text.RichText
            }

            AppIcon {
                id: alertIcon
                source: 'circle-exclamation-solid'
                Layout.leftMargin: 4
                iconColor: color_text_danger
                width: 24
                height: 24
                visible: false
            }
            Text {
                id: alertMsg
                text: ""
                font.italic: true
                color: color_text_danger
                visible: false
            }

        }
    }
}
