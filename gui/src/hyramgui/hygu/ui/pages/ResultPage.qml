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

import "../components"
import hygu.classes


Item {
    property int qIndex
    property int barWidth: 1162  // align with right side of last button

    property font paramFont: Qt.font({
        bold: false,
        italic: false,
        pixelSize: 14,
    });
    property font paramFontBold: Qt.font({
        bold: true,
        italic: false,
        pixelSize: 14,
    });

    function clearImage(qimg)
    {
        qimg.source = "";
        qimg.filename = "";
    }

    function updateImage(qimg, fl)
    {
        let val = fl ? fl : "";
        if (val === "" || val === null)
        {
            qimg.visible = false;
            return;
        }
        qimg.visible = true;
        qimg.source = 'file:' + val;
        qimg.filename = val;
    }

    function updateContent()
    {
        // Fill this
    }

    function refresh(resultsForm)
    {
        if (resultsForm !== null)
        {
            pyform = resultsForm;
        }
        updateContent();
    }

    anchors.fill: parent
}
