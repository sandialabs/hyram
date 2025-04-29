/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick 2.12
import QtQuick.Layouts


Text {
    property int w: 780;

    font.pointSize: contentFontSize + 1
    Layout.preferredWidth: w
    Layout.topMargin: 4
    Layout.bottomMargin: 8
    wrapMode: Text.WordWrap
    text: ''
    font.italic: true
}
