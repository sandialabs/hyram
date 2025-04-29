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

import hygu.classes
import "buttons"


Rectangle {
    property alias btnRef: toggleBtn;
    property alias containerRef: container;  // to insert components into the layout
    property string title;
    property int w: 400
    property int closedH: 30;
    property int btmMargin: 10;
    property bool startOpen: false

    property bool asHeader: false
    property int titleFontSize: 14;
    property var textColor: headerColor
    property bool isBold: true;
    property bool isItalic: false;

    property bool useBorder: false;
    // property var borderColor: Material.color(Material.Grey, Material.Shade300);
    property var borderColor: "#e3e3e3";

    id: rect
    width: container.width

    // ensure expansion works in Columns and Layouts
    height: startOpen ? container.childrenRect.height + btmMargin : closedH
    Layout.preferredHeight: height
    Layout.preferredWidth: w
    Layout.bottomMargin: btmMargin;
    anchors.bottomMargin: btmMargin;

    radius: 3
    border.width: useBorder ? 1 : 0;
    border.color: useBorder ? borderColor : "transparent";
    color: "transparent"
    // border.color: "#b4b4b4"
    // color: formBgColor

    Behavior on height { NumberAnimation { duration: 100}}
    clip: true

    function toggleDisplay()
    {
        if (rect.height > closedH + 10) {
            // closing
            toggleBtn.img = 'chevron-left-solid';
            rect.height = closedH;
        } else {
            toggleBtn.img = 'chevron-down-solid';
            rect.height = container.childrenRect.height + btmMargin;
        }
    }

    ColumnLayout {
        id: container
        width: w

        // header row
        Rectangle {
            id: header
            Layout.preferredHeight: closedH
            Layout.preferredWidth: w
            color: "transparent"
            // color: Material.color(Material.BlueGrey, Material.Shade100)

            Text {
                text: title
                anchors.left: parent.left
                anchors.leftMargin: useBorder ? 5 : 0
                anchors.verticalCenter: parent.verticalCenter
                horizontalAlignment: Text.AlignLeft
                font.pointSize: titleFontSize
                font.bold: isBold
                font.italic: isItalic
                color: textColor
            }

            IconButtonFlat {
                id: toggleBtn
                implicitHeight: 36
                implicitWidth: 33
                anchors.left: parent.left
                anchors.verticalCenter: parent.verticalCenter
                anchors.leftMargin: w - 33;  // constant distance from left side regardless of title length
                img: startOpen ? 'chevron-down-solid' : 'chevron-left-solid'

            }
            MouseArea {
                anchors.fill: parent
                onClicked: function() { toggleDisplay();}
            }

        }

        Rectangle {
            visible: asHeader
            enabled: asHeader
            width: w
            height: 1
            color: Material.color(Material.Blue)
            opacity: 0.3
        }
    }
}
