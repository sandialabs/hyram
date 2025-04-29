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

     Material.containerStyle: Material.Filled
     implicitHeight: 24
     topPadding: 5
     bottomPadding: 5
     Layout.alignment: Qt.AlignCenter
     Layout.maximumWidth: 100
     horizontalAlignment: Text.AlignHCenter

     text: param ? param[field] : ''

     // record change; only fires if input passes validator.
     onEditingFinished: function()
     {
         if (length > 0)
         {
             param[field] = text;
         }
     }

     onActiveFocusChanged: {
         if (!activeFocus)
         {
             // disallow blank input
//             if (length === 0 || !acceptableInput)
//             {
//                text = param[field];
//             }
         }
     }
 }
