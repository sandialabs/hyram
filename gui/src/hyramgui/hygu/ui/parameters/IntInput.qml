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


 TextField {
     property int w: 100;
     property double val;
     property alias tooltip: ttip;

     property int minValue: 0;
     property int maxValue: 2147483647;

     function refreshLims()
     {
         ttip.text = "Enter a value between " + minValue + " and " + maxValue;
     }

     Material.containerStyle: Material.Filled
     implicitHeight: 24
     topPadding: 5
     bottomPadding: 5
     Layout.alignment: Qt.AlignCenter
     Layout.maximumWidth: w
     Layout.preferredWidth: w
     horizontalAlignment: Text.AlignHCenter

     text: val
     validator: IntValidator {bottom: minValue; top: maxValue}

     // record change; only fires if input passes validator.
     onEditingFinished: { if (length > 0) val = parseInt(text); }

     // disallow blank input
     onActiveFocusChanged: { if (!activeFocus && (length === 0 || !acceptableInput)) text = val.toString(); }

     hoverEnabled: true
     ToolTip {
         id: ttip
         delay: 400
         timeout: 5000
         visible: parent.hovered
     }
 }
