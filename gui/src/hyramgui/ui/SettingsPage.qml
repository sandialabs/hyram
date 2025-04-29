/* Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

 * You should have received a copy of the GNU General Public License along with HyRAM+.
 * If not, see https://www.gnu.org/licenses/.
 */

import QtQuick
import QtQuick.Layouts
import QtQuick.Controls 2.12
import QtQuick.Dialogs
import QtQuick.Window
import QtQuick.Controls.Material 2.12

import "../hygu/ui/components"
import "../hygu/ui/components/buttons"
import "../hygu/ui/parameters"


Popup {
    x: parent.width * 0.1
    y: parent.height * 0.2
    width: parent.width * 0.8
    height: parent.height * 0.5
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
            text: "Configure global settings for analyses. Will be applied to analyses executed after settings updated."
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
            extraButton.onClicked: {
                app_form.reset_session_dir();
            }
        }

        Item {
            Layout.fillHeight: true
        }

        RowLayout {
            BasicButton {
                id: submitBtn
                btnText: "Close"
                tooltip: "Close settings"
                Layout.alignment: Qt.AlignCenter
                Layout.leftMargin: 145
                Layout.bottomMargin: 8

                onClicked: {
                    settingsPage.close();
                }
            }

            Item {
                Layout.fillWidth: true
            }
        }
    }
}
