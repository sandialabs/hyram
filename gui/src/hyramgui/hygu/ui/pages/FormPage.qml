/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick 2.12
import QtQuick.Layouts
import QtQuick.Controls 2.12
import QtQuick.Dialogs
import QtQuick.Window
import QtQuick.Controls.Material 2.12

import "../components"
import "../parameters"
import hygu.classes


Item {
    property AppForm appForm

    Connections {
        target: appForm
        function onAlertChanged(msg, level)
        {
            refreshAlerts(msg, level);
        }
    }


    function refreshAlerts(msg, level)
    {
        if (level === 1)
        {
            alertSection.visible = false;
            alertText.text = "";
        }
        else
        {
            alertSection.visible = true;
            alertText.text = msg;
            alertText.color = color_text_levels[level];
            alertSection.color = color_levels[level];
            alertIcon.iconColor = color_text_levels[level];
        }
    }

    function changeButtonTextColor(btn, opac = 0)
    {
        btn.background.opacity = opac;
    }

    function refreshForm()
    {
    }

}
