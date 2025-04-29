/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Layouts
import QtQuick.Controls 2.12
import QtQuick.Dialogs
import QtQuick.Window
import QtQuick.Controls.Material 2.12

import "../components"
import "../parameters"


Popup {
    x: parent.width * 0.04
    y: parent.height * 0.04
    width: parent.width * 0.92
    height: parent.height * 0.9
    modal: true
    focus: true

    AppIcon {
        source: 'xmark-solid'
        anchors.right: parent.right
        anchors.top: parent.top

        MouseArea {
            anchors.fill: parent
            onClicked: close()
        }
    }

    ColumnLayout {
        spacing: 10
        anchors.fill: parent

        FormSectionHeader {
            title: "Application Settings";
            rWidth: parent.width * 0.96
            fontSize: 20
            Layout.topMargin: 10
        }

        Text {
            font.pointSize: 12
            font.italic: true
            text: "Configure global settings for analyses"
        }

        Item {
            height: 20
        }

        DirectorySelector {
            param: session_dir_c
            inputLength: 460
            folderDialog.onAccepted: {
                app_form.set_session_dir(folderDialog.selectedFolder);
            }
        }

        Item {
            Layout.fillHeight: true
        }

        RowLayout {
            spacing: 10

            Item {
                Layout.fillWidth: true
            }
        }
    }
}
