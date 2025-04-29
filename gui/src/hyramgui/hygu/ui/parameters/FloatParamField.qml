/*
 * Copyright 2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Window
import QtQuick.Controls.Material 2.12

import hygu.classes
import "../components"


Rectangle {
    property NumFormField param;
    property string tipText;
    property bool hasError: false
    property string errorMsg: "test long error msg ERROR HERE"
    property bool isReadOnly: false
    property bool showUnitSelector: true
    property alias label: paramLabel;
    property alias input: valueInput;
    property alias extraText: extraTextId;

    id: container
    color: "transparent";
    width: paramInputRow.width
    Behavior on opacity {NumberAnimation { duration: 100 }}

    Component.onCompleted:
    {
        refresh();
    }

    function refresh()
    {
        if (hasError)
        {
            container.Layout.preferredHeight = 60;
            paramLabel.color = color_danger;
            alertMsg.text = errorMsg;
            alertDisplay.visible = true;
        }
        else
        {
            container.Layout.preferredHeight = 30;
            paramLabel.color = color_primary;
            alertMsg.text = "";
            alertDisplay.visible = false;
        }

        if (showUnitSelector)
        {
            unitSelector.currentIndex = param.get_unit_index();
        }

        if (isReadOnly)
        {
            valueInput.visible = true;
            valueInput.readOnly = true;
            valueInput.text = param.value;
            return;
        }

        valueInput.refresh();
    }


    RowLayout
    {
        id: paramInputRow
        anchors.verticalCenter: parent.verticalCenter

        Component.onCompleted:
        {
            refresh();
        }


        Connections {
            target: param
            function onModelChanged() { refresh(); }
            function onUnitChanged() { refresh(); }
        }

        Text {
            id: paramLabel
            text: param?.label_rtf ?? ''
            Layout.preferredWidth: paramLabelWidth
            horizontalAlignment: Text.AlignLeft
            Layout.alignment: Qt.AlignVCenter
            width: paramLabelWidth
            font.pointSize: labelFontSize
            textFormat: Text.RichText
            wrapMode: Text.WordWrap

            ToolTip {
                delay: 200
                timeout: 3000
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

        DoubleTextInput {
            id: valueInput
            field: 'value'
            Layout.preferredWidth: defaultInputW
            Layout.maximumWidth: defaultInputW
            Layout.alignment: Qt.AlignVCenter
            tooltip.text: param?.value_tooltip ?? "Enter a value between " + minValue + " and " + maxValue
        }

        DenseComboBox {
            id: unitSelector
            Layout.alignment: Qt.AlignVCenter
            visible: showUnitSelector && param && param.unit_type !== 'unitless'
            model: param?.unit_choices ?? null
            currentIndex: param?.get_unit_index() ?? 0
            onActivated: {
                if (param !== null) param.unit = displayText
            }

        }

        Text {
            id: extraTextId
            Layout.leftMargin: 4
            font.pointSize: labelFontSize - 1
            textFormat: Text.RichText
        }

    }

    Row
    {
        id: alertDisplay
        anchors.top: paramInputRow.bottom
        leftPadding: 125

        AppIcon {
            id: alertIcon
            source: 'circle-exclamation-solid'
            iconColor: Material.color(Material.Red)
            width: 24
            height: 24
        }
        Text {
            id: alertMsg
            text: ""
            anchors.topMargin: 4
            font.italic: true
            anchors.verticalCenter: parent.verticalCenter
            color: color_danger
        }
    }
}
