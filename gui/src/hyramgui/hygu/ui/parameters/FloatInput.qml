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
     property string tipText: "";

     property double minValue: -Infinity;
     property double maxValue: Infinity;

     function refreshLims()
     {
         if (tipText === "")
         {
             tipText = "Enter a value between " + minValue + " and " + maxValue;
         }
     }

     Material.containerStyle: Material.Filled
     implicitHeight: 24
     topPadding: 5
     bottomPadding: 5
     Layout.alignment: Qt.AlignCenter
     Layout.maximumWidth: w
     horizontalAlignment: Text.AlignHCenter

     text: val
     validator: DoubleValidator {bottom: minValue; top: maxValue}

     // record change; only fires if input passes validator.
     onEditingFinished: { if (length > 0) val = parseFloat(text); }

     // disallow blank input
     onActiveFocusChanged: { if (!activeFocus && (length === 0 || !acceptableInput)) text = val.toString(); }

     hoverEnabled: true
     ToolTip {
         id: ttip
         delay: 400
         timeout: 5000
         visible: tipText ? parent.hovered : false
         text: tipText
     }
 }
