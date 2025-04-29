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
        overpXLimInput.visible = !autoLimits;
        overpYLimInput.visible = !autoLimits;
        overpZLimInput.visible = !autoLimits;
        impulseXLimInput.visible = !autoLimits;
        impulseYLimInput.visible = !autoLimits;
        impulseZLimInput.visible = !autoLimits;

        var diffKeys = app_form.get_diff_keys;
        if (diffKeys.indexOf('uo_points') > -1)
        {
            setUiPointData();
        }
    }

    function addPoint() {
        app_form.add_uo_point();
        // pointFlickable.flick(-1000, 0);
    }

    /**
     * Removes selected Point and persists to db.
     * @param index
     */
    function removePoint(index) {
        app_form.remove_uo_point(index)
    }

    /**
     * Updates UI display of point groups based on model data.
     * @constructor
     */
    function setUiPointData()
    {
        var dataArr = JSON.parse(app_form.uo_point_data);
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
                app_form.set_uo_point_data(index, jsonStr);
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
            text: "Unconfined Overpressure"
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
            text: "Calculate unconfined overpressure and impulse behavior for delayed ignition of gaseous hydrogen jets"
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

                    ChoiceParamField {
                        param: overp_method_c;
                    }

                    ChoiceParamField {
                        param: mach_speed_c;
                        tipText: "Speed of the flame front. Only applicable for BST method."
                        enabled: overp_method_c.value === "bst"
                        opacity: overp_method_c.value === "bst" ? 1 : fadeVal
                    }

                    FloatParamField {
                        param: tnt_factor_c;
                        tipText: "Only applicable for TNT method"
                        input.Layout.preferredWidth: defaultSelectorW;
                        input.Layout.maximumWidth: defaultSelectorW;
                        enabled: overp_method_c.value === "tnt"
                        opacity: overp_method_c.value === "tnt" ? 1 : fadeVal
                    }

                    // Position Point input section. Point container expands to max width as points added.
                    RowLayout {
                        Layout.bottomMargin: 10

                        Text {
                            text: "Positions (m)"
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
                            tooltip: "Add position"
                            onClicked: function() {
                                addPoint();
                            }
                        }

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

                    NumListParamField {
                        param: uo_overp_contours_c
                        afterText: "kPa"
                        inputLength: 240
                        tipText: "Overpressure contours to plot. Separate values with a space (' ')"
                    }

                    NumListParamField {
                        param: uo_impulse_contours_c
                        afterText: "kPa*s"
                        inputLength: 240
                        tipText: "Impulse contours to plot. Separate values with a space (' ')"
                    }

                    FloatParamField {
                        param: plume_mass_flow_c
                        enabled: app_form.is_unchoked
                        tipText: "Optional unchoked flux mass flow rate"
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

                    BoolParamField {
                        id: autoPlotLimits
                        param: uo_auto_limits_c
                    }

                    NumPairParamField {
                        id: overpXLimInput
                        labelText: "Overpressure X limits"
                        param1: uo_overp_xmin_c
                        param2: uo_overp_xmax_c
                        tipText: "Overpressure plot X bounds"
                    }
                    NumPairParamField {
                        id: overpYLimInput
                        labelText: "Overpressure Y limits"
                        param1: uo_overp_ymin_c
                        param2: uo_overp_ymax_c
                        tipText: "Overpressure plot Y bounds"
                    }
                    NumPairParamField {
                        id: overpZLimInput
                        labelText: "Overpressure Z limits"
                        param1: uo_overp_zmin_c
                        param2: uo_overp_zmax_c
                        tipText: "Overpressure plot Z bounds"
                    }

                    NumPairParamField {
                        id: impulseXLimInput
                        labelText: "Impulse X limits"
                        param1: uo_impulse_xmin_c
                        param2: uo_impulse_xmax_c
                        tipText: "Impulse plot X bounds"
                    }
                    NumPairParamField {
                        id: impulseYLimInput
                        labelText: "Impulse Y limits"
                        param1: uo_impulse_ymin_c
                        param2: uo_impulse_ymax_c
                        tipText: "Impulse plot Y bounds"
                    }
                    NumPairParamField {
                        id: impulseZLimInput
                        labelText: "Impulse Z limits"
                        param1: uo_impulse_zmin_c
                        param2: uo_impulse_zmax_c
                        tipText: "Impulse plot Z bounds"
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
                Layout.leftMargin: 20
                Layout.bottomMargin: 8
                Material.roundedScale: Material.SmallScale
                Material.accent: Material.Blue
                highlighted: true

                onClicked: {
                    forceActiveFocus();
                    app_form.request_analysis("uo");
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
