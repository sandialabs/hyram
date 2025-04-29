/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Window
import QtQuick.Dialogs
import QtQuick.Controls.Material

import "../components"
import "../components/buttons"
import hygu.classes


Item {
    property StringFormField param;
    property int inputLength: 240;
    property bool hasError: false;  // TODO: unused?
    property string errorMsg: "test long error msg ERROR HERE";
    property string tipText;
    property alias folderDialog: dialog;
    property alias extraButton: extraBtn;


    id: paramContainer
    Layout.preferredHeight: 70

    Component.onCompleted:
    {
        refresh();
    }

    function refresh()
    {
        let color = color_primary;
        if (param.status === 1)
        {
            alertText.text = "";
            alertDisplay.visible = false;
        }
        else
        {
            alertText.text = param.alert;
            alertDisplay.visible = true;
        }

        paramLabel.color = color_text_levels[param.status];
        alertText.color = color_text_levels[param.status];
        alertIcon.iconColor = color_text_levels[param.status];
        alertDisplay.color = color_levels[param.status];

        valueInput.visible = true;
        valueInput.text = param.value;
        valueInput.enabled = param.enabled;
    }

    FolderDialog {
        // dir selection handled in parent
        id: dialog
        currentFolder: "file:///" + param?.value
    }


    Row
    {
        id: paramInputRow

        Component.onCompleted:
        {
            refresh();
        }

        RowLayout {
            Connections {
                target: param
                function onModelChanged() { refresh(); }
            }

            Text {
                id: paramLabel
                text: param?.label ?? ''
                Layout.preferredWidth: 120
                horizontalAlignment: Text.AlignLeft
                font.pointSize: labelFontSize

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

            StringTextField {
                id: valueInput
                text: param.value
                field: "value"
                readOnly: true
                Layout.maximumWidth: inputLength
                Layout.preferredWidth: inputLength
                horizontalAlignment: Text.AlignLeft
            }

            IconButton {
                id: folderBtn
                img: "ellipsis-solid"
                tooltip: "select an output directory for analysis results"
                Layout.maximumHeight: 40
                Layout.maximumWidth: Layout.maximumHeight * 9/8
                onClicked:
                {
                    dialog.open();
                }
            }

            IconButton {
                id: extraBtn
                img: "repeat-solid"
                tooltip: "Reset output behavior to always create new session directory"
                Layout.maximumHeight: 40
                Layout.maximumWidth: Layout.maximumHeight * 9/8
            }
        }
    }

    Rectangle {
        id: alertDisplay
        visible: true
        color: color_danger
        radius: 5
        anchors.left: parent.left
        anchors.leftMargin: 125
        anchors.top: paramInputRow.bottom
        height: 28
        width: alertContents.width

        Row {
            id: alertContents
            spacing: 4
            anchors.verticalCenter: parent.verticalCenter
            leftPadding: 6

            AppIcon {
                id: alertIcon
                source: 'circle-exclamation-solid'
                iconColor: color_text_danger
                height: 26
                anchors.verticalCenter: parent.verticalCenter
            }
            TextEdit {
                id: alertText
                text: "Test alert lorem ipsum"
                rightPadding: 10
                color: color_text_danger
                readOnly: true
                selectByMouse: true
                font.pointSize: 10
                font.bold: true
                anchors.verticalCenter: parent.verticalCenter
            }
        }

    }
}
