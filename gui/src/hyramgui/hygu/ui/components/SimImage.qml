/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Dialogs
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Window
import QtQuick.Controls.Material


Image {
    property string filename

    id: thisImage
    // Filepath to image; if not using resource system, can't use relative paths
    source: ""
    fillMode:Image.PreserveAspectFit
    height: 450
    sourceSize.height: height

    function handleClick(mouse)
    {
        if (mouse.button === Qt.LeftButton)
        {
            ;
        }
        else if (mouse.button === Qt.RightButton)
        {
            contextMenu.popup();
        }
    }

    function handleCopy()
    {
        app_form.copy_image_to_clipboard(filename);
    }

    MouseArea {
        anchors.fill: parent
        acceptedButtons: Qt.LeftButton | Qt.RightButton
        onClicked: (mouse) => handleClick(mouse)

        Menu {
            id: contextMenu
            Action {
                text: "Copy"
                onTriggered: handleCopy()
            }
            Action {
                text: "Save As..."
                onTriggered: {
                    dialog.title = "Save image to file"
                    dialog.nameFilters = [ "PNG files (*.png)" ]
                    dialog.open()
                }
            }
        }
    }

    FileDialog {
        id: dialog
//        currentFolder: shortcuts.home
        fileMode: FileDialog.SaveFile
        onAccepted: {
            thisImage.grabToImage(function(result) {
                result.saveToFile(dialog.selectedFile);
            });
        }
    }

}

