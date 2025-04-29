/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Window
import QtQuick.Controls.Material


TextEdit {
    Layout.alignment: Qt.AlignCenter
    horizontalAlignment: Text.AlignHCenter
    font.italic: true
    font.pointSize: inputTopLabelFontSize
    Layout.maximumHeight: 6
    Layout.maximumWidth: 100
    readOnly: true
    selectByMouse: true
}
