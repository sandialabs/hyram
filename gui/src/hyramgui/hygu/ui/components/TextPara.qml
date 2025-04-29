/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */

import QtQuick 2.12


Text {
    property string txt: ""
    property int w: parent.width
    property int align: Text.AlignLeft

    text: txt
    horizontalAlignment: align
    font.pointSize: labelFontSize
    wrapMode: Text.WordWrap
    textFormat: Text.RichText
    width: w
}
