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
     property string field;  // reference to associated parameter field, e.g. 'value' or 'a'
     property var paramRef: param ?? null;
     property alias tooltip: ttip;
     property bool useLimits: true;

     function refreshLims()
     {
         if (useLimits)
         {
             ttip.text = paramRef?.value_tooltip ?? '';
             vdtr.bottom = paramRef?.min_value ?? -Infinity;
             vdtr.top = paramRef?.max_value ?? Infinity;
         }
         else
         {
             ttip.text = "Enter a value";
             vdtr.bottom = -Infinity;
             vdtr.top = Infinity;
         }
     }

     function refresh() {
         text = paramRef ? paramRef[field] : '';
         refreshLims();
     }

     Material.containerStyle: Material.Filled
     implicitHeight: 24
     topPadding: 5
     bottomPadding: 5
     Layout.alignment: Qt.AlignCenter
     Layout.maximumWidth: 100
     horizontalAlignment: Text.AlignHCenter

     text: paramRef ? paramRef[field] : ''

     validator: DoubleValidator {
         id: vdtr
         bottom: useLimits ? (paramRef?.min_value ?? -Infinity) : -Infinity
         top: useLimits ? (paramRef?.max_value ?? Infinity) : Infinity
     }

     // record change; only fires if input passes validator.
     onEditingFinished: function()
     {
         if (length > 0)
         {
             paramRef[field] = text;
         }
     }

     onActiveFocusChanged: {
         if (!activeFocus)
         {
             // disallow blank input
             if (paramRef && (length === 0 || !acceptableInput))
             {
                text = paramRef[field];
             }
         }
     }

     hoverEnabled: true
     ToolTip {
         id: ttip
         delay: 400
         timeout: 5000
         visible: parent.hovered
         text: useLimits ? (paramRef?.value_tooltip ?? '') : "Enter a value"
     }
 }
