/*
 * Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

 * You should have received a copy of the GNU General Public License along with HyRAM+.
 * If not, see https://www.gnu.org/licenses/.
 */

import QtQuick 2.12
import QtQuick.Layouts
import QtQuick.Controls 2.12
import QtQuick.Dialogs
import QtQuick.Window
import QtQuick.Controls.Material 2.12

import "../../hygu/ui/components"
import "../../hygu/ui/components/buttons"
import "../../hygu/ui/parameters"
import "../../hygu/ui/pages"
import "../components"
import "../parameters"
import hyram.classes


FormPage {
    property alias alertRef: alert;

    function refreshForm()
    {
        let autoLimits = autoPlotLimits.checked();
        heatXLimInput.visible = !autoLimits;
        heatYLimInput.visible = !autoLimits;
        heatZLimInput.visible = !autoLimits;
        tempXLimInput.visible = !autoLimits;
        tempYLimInput.visible = !autoLimits;

        var diffKeys = app_form.get_diff_keys;
        if (diffKeys.indexOf('jet_flame_points') > -1)
        {
            setUiPointData();
        }
    }

    function addPoint() {
        app_form.add_jet_flame_point();
    }

    /**
     * Removes selected Point and persists to db.
     * @param index
     */
    function removePoint(index) {
        app_form.remove_jet_flame_point(index)
    }

    /**
     * Updates UI display of point groups based on model data.
     * @constructor
     */
    function setUiPointData()
    {
        var dataArr = JSON.parse(app_form.jet_flame_point_data);
        var nStaleFields = pointContainer.children.length;
        var nNewFields = dataArr.length;

        // clear stale
        for (let i = 0; i < nStaleFields; i++)
        {
            var obj = pointContainer.children[i];
            if (obj.isAlive)
            {
                obj.isAlive = false;
                obj.destroy();
            }
        }

        for (let i = 0; i < nNewFields; i++)
        {
            var comp = Qt.createComponent("../../hygu/ui/parameters/PointVerticalField.qml");
            var field = comp.createObject(pointContainer);
            if (comp.status === Component.Ready)
            {
                field.setFromModel(dataArr[i], i);
            }
        }
    }

    /**
     * Updates backend model data of single point entry.
     */
    function setModelPointData(index)
    {
        if (pointContainer.children.length > index)
        {
            var pt = pointContainer.children[index];
            if (pt.isAlive)
            {
                var jsonStr = JSON.stringify(pt.getData());
                app_form.set_jet_flame_point_data(index, jsonStr);
            }
        }
    }

    Component.onCompleted:
    {
        refreshForm();
        setUiPointData();
    }


    ColumnLayout {
        width: parent.width
        anchors.fill: parent

        // Form header
        Text {
            font.pointSize: header1FontSize
            id: formHeader
            font.weight: 600
            Layout.leftMargin: 14
            text: "Jet Flame"
        }

        Rectangle {
            height: 2
            Layout.fillWidth: true
            Layout.leftMargin: 16
            Layout.rightMargin: 4
            color: Material.color(Material.Blue, Material.Shade400)
        }
        Text {
            font.pointSize: contentFontSize + 1
            id: formDescrip
            font.italic: true
            Layout.leftMargin: 14
            text: "Calculate behavior of a jet flame including flame temperature, direction, and heat flux"
        }

        Item {
            id: inputSpacer2
            Layout.preferredHeight: 4
        }

        FlickableForm
        {
            contentHeight: paramCols.height + 20 + 120

            // ==========================
            // ==== Parameter Inputs ====
            Item {
                id: paramColContainer
                Layout.fillHeight: true
                height: paramCols.height

                ColumnLayout {
                    id: paramCols

                    // Point section
                    RowLayout {
                        Layout.bottomMargin: 10

                        Text {
                            text: "Locations of interest (m)"
                            Layout.preferredWidth: paramLabelWidth - addPointBtn.width - 4
                            width: paramLabelWidth - addPointBtn.width - 4
                            horizontalAlignment: Text.AlignLeft
                            Layout.alignment: Qt.AlignVCenter
                            font.pointSize: labelFontSize
                            textFormat: Text.RichText
                            wrapMode: Text.WordWrap

                            ToolTip {
                                delay: 200
                                timeout: 3000
                                visible: ma.containsMouse
                                text: "Enter (x,y,z) point values"
                            }

                            // for tooltip hover
                            MouseArea {
                                id: ma
                                anchors.fill: parent
                                hoverEnabled: true
                            }
                        }

                        IconButton {
                            id: addPointBtn
                            width: 50
                            Layout.preferredHeight: 50
                            Layout.preferredWidth: 44
                            Layout.alignment: Qt.AlignCenter
                            img: 'plus-solid'
                            tooltip: "Add location"
                            onClicked: function() {
                                addPoint();
                            }
                        }

                        // X,Y,Z labels
                        Rectangle {
                            Layout.preferredHeight: 135
                            Layout.preferredWidth: 14
                            Layout.leftMargin: 10
                            color: "transparent"

                            ColumnLayout {
                                width: 20
                                height: 135

                                Item {
                                    Layout.preferredHeight: 1
                                }
                                Text {text: "X"}
                                Item {height: 2}
                                Text {text: "Y"}
                                Item {height: 2}
                                Text {text: "Z"}

                                Item {Layout.fillHeight: true}
                            }
                        }

                        Rectangle {
                            Layout.preferredWidth: pointFlickable.contentWidth
                            Layout.maximumWidth: 500
                            Layout.preferredHeight: 160
                            Layout.alignment: Qt.AlignVCenter
                            color: "#fffbfe"
                            Behavior on Layout.preferredWidth { NumberAnimation { duration: 200 } }

                            Flickable {
                                id: pointFlickable
                                anchors.fill: parent

                                flickableDirection: Flickable.HorizontalFlick
                                contentWidth: pointContainer.width + 20
                                clip: true
                                ScrollBar.horizontal: ScrollBar { policy: ScrollBar.AsNeeded; }

                                ScrollMouseArea {container: pointFlickable}

                                RowLayout {
                                    id: pointContainer
                                    clip: true
                                }
                            }
                        }
                    }  // end point section

                    FloatParamField {
                        param: plume_mass_flow_c
                        enabled: app_form.is_unchoked
                        opacity: app_form.is_unchoked ? 1 : 0.3
                        tipText: "Optional mass flow rate of fuel out of orifice (unchoked)"
                    }

                    FloatParamField {
                        param: leak_d_c
                        tipText: "Diameter of leaking orifice"
                    }

                    FormSectionHeader {
                        id: section2
                        title: "Plotting"
                        iconSrc: 'chart-simple-solid'
                    }

                    NumListParamField {
                        param: flame_contours_c
                        inputLength: 240
                        tipText: "Heat flux levels to mark on plot. Separate values with a space (' ')."
                    }

                    BoolParamField {
                        id: autoPlotLimits
                        param: flame_auto_limits_c
                    }
                    NumPairParamField {
                        id: heatXLimInput
                        labelText: "Heat flux plot X limits"
                        param1: heat_xmin_c
                        param2: heat_xmax_c
                    }
                    NumPairParamField {
                        id: heatYLimInput
                        labelText: "Heat flux plot Y limits"
                        param1: heat_ymin_c
                        param2: heat_ymax_c
                    }
                    NumPairParamField {
                        id: heatZLimInput
                        labelText: "Heat flux plot Z limits"
                        param1: heat_zmin_c
                        param2: heat_zmax_c
                    }

                    NumPairParamField {
                        id: tempXLimInput
                        labelText: "Temperature plot X limits"
                        param1: temp_xmin_c
                        param2: temp_xmax_c
                    }
                    NumPairParamField {
                        id: tempYLimInput
                        labelText: "Temperature plot Y limits"
                        param1: temp_ymin_c
                        param2: temp_ymax_c
                    }

                    Item {
                        Layout.fillHeight: true
                    }

                }
            }
        }

        Rectangle {
            height: 1
            Layout.fillWidth: true
            Layout.leftMargin: 16
            Layout.rightMargin: 4
            color: Material.color(Material.Blue, Material.Shade400)
        }

        RowLayout {

            // =========================
            // ==== Analysis Button ====
            Button {
                id: submitBtn
                Layout.preferredWidth: 110
                Layout.alignment: Qt.AlignCenter
                Layout.leftMargin: 40
                Layout.bottomMargin: 8
                Material.roundedScale: Material.SmallScale
                Material.accent: Material.Blue
                highlighted: true

                onClicked: {
                    forceActiveFocus();
                    app_form.request_analysis("flame");
                }

                Row {
                    anchors.horizontalCenter: parent.horizontalCenter
                    anchors.verticalCenter: parent.verticalCenter
                    spacing: 0

                    AppIcon {
                        anchors.verticalCenter: parent.verticalCenter
                        icon.width: 20
                        source: 'bolt-solid'
                        iconColor: "white"
                    }

                    Text {
                        anchors.verticalCenter: parent.verticalCenter
                        text: "Analyze"
                        font.pixelSize: 20
                        font.bold: true
                        color: "white"
                        bottomPadding: lgBtnBottomPadding
                    }
                }
            }

            SectionAlert {
                id: alert;
                Layout.bottomMargin: 5
                Layout.preferredWidth: 640
            }
        }
    }
}
