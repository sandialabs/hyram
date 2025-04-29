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

import "../hygu/ui/utils.js" as Utils
import "../hygu/ui/components"
import "../hygu/ui/components/buttons"
import "../hygu/ui/parameters"
import "../hygu/ui/pages"
import "components"
import "physics"


ApplicationWindow {
    id: main
    width: 1366
    height: 768

    minimumWidth: 1024
    minimumHeight: 700

    visible: true
    title: "Hydrogen Plus Other Alternative Fuels Risk Assessment Models (HyRAM+)"

    // ===========================
    // ==== Global Properties ====
    property bool isWindows: Qt.platform.os === "windows";
    property bool isDebug: app_form.is_debug_mode;

    property string appDir: Qt.resolvedUrl("../");

    Material.theme: Material.Light
    Material.primary: Material.Teal
    Material.accent: Material.Blue

    property string color_primary: "#020202"
    property string color_success: Material.color(Material.Green)
    property string color_info: Material.color(Material.Blue)
    property string color_progress: Material.color(Material.Orange)
    property string color_danger: Material.color(Material.Red)
    property string color_danger_bg: Material.color(Material.Red, Material.Shade100)
    property string color_warning: "lightyellow"
    property string color_disabled: Material.color(Material.Grey, Material.Shade700)
    property string oldSampleType: ""
    property int oldNumEpi: -1
    property int oldNumAle: -1

    // Panel colors
    property color formBgColor: "#FFFBFE";
    property color btnColorDefault: Material.color(Material.Grey, Material.Shade200);
    property color sideMenuColor: "#EBF4F4";

    property string color_text_info: "black"
    property string color_text_warning: "darkgoldenrod"
    property string color_text_danger: "white"

    property var color_levels: [color_danger, color_primary, color_info, color_warning]
    property var color_text_levels: [color_text_danger, color_text_info, color_text_info, color_text_warning]

    property int scrollBtnHeight: 40

    // Platform-specific properties adjusted via onComplete below
    // default windows font point size is 9; mac is 13
    property int labelFontSize: 13;
    property int inputTopLabelFontSize: 11;
    property int contentFontSize: 13;
    property int header1FontSize: 16;
    property int lgBtnBottomPadding: 4;
    // property int btnBottomPadding: 0;
    property int headerTopPadding: 12;
    property int paramLabelWidth: 145;
    property int defaultInputW: 120;
    property int defaultSelectorW: 130;

    property double fadeVal: 0.3;

    property color headerColor: "#919191"

    property string activeFormId: "state";
    property string stateFormId: "state";
    property string qraFormId: "qra";
    property string plumeFormId: "plume";
    property string flameFormId: "flame";
    property string accumFormId: "accum";
    property string uoFormId: "uo";

    property string densityFormId: "density";
    property string tankMassFormId: "tank";
    property string massFlowFormId: "mass";

    property font paramFont: Qt.font({
        bold: false,
        italic: false,
        pixelSize: 14,
    });
    property font paramFontBold: Qt.font({
        bold: true,
        italic: false,
        pixelSize: 14,
    });


    property FormPage activeForm;

    function refreshAlerts(msg, level)
    {
        let excluded = [qraFormId, accumFormId, densityFormId, tankMassFormId, massFlowFormId];
        // skip forms that handle their own alerts
        if (excluded.includes(activeFormId))
        {
            return;
        }

        activeForm.alertRef.level = level;
        activeForm.alertRef.msg = msg
    }


    Connections {
        target: app_form
        function onAlertChanged(msg, level) {refreshAlerts(msg, level);}

        function onNewMessageEvent(msg) {handleNewMessageEvent(msg);}

        function onHistoryChanged() {refreshForm();}
    }

    onClosing: function(close) {
        close.accepted = false;
        doShutdown();
    }

    // onHeightChanged: {
    //     console.log("window height: " + height + ", " + formContainer.height + ", " + activeForm.height);
    // }

    Component.onCompleted: {
        refreshForm();

        // make platform-specific adjustments
        if (isWindows)
        {
            labelFontSize = 10;
            inputTopLabelFontSize = 9;
            contentFontSize = 10;
        }
        else
        {
            labelFontSize = 13;
            inputTopLabelFontSize = 11;
            contentFontSize = 12;
            lgBtnBottomPadding = 2;
            header1FontSize = 20;
            headerTopPadding = 16;
        }

        if (isDebug)
        {
            ;
        }
        else
        {
            mainMenu.removeMenu(devMenu);
        }

        changeForm(activeFormId);
    }

    /// Shutdown application by first opening dialog, then calling shutdown.
    function doShutdown()
    {
        shutdownPopup.open();
    }

    function refreshForm()
    {
        if (formLoader.status === Loader.Ready)
        {
            formLoader.item.refreshForm();
        }

        undoBtn.enabled = app_form.can_undo;
        redoBtn.enabled = app_form.can_redo;

        var showPhase = rel_phase_c.value !== "fluid";
        var phaseDisp = showPhase ? rel_phase_c.value_display : "";
        phaseDisp = phaseDisp.toLowerCase();

        // update displayed fuel
        if (fuel_mix_c.value === 'h2')
        {
            fuelDisp.text = showPhase ? "Hydrogen (" + phaseDisp + ")" : "Hydrogen";
            fuelDisp.color = Material.color(Material.Blue, Material.Shade400);
        }
        else if (fuel_mix_c.value === 'ch4')
        {
            fuelDisp.text = showPhase ? "Methane (" + phaseDisp + ")" : "Methane";
            fuelDisp.color = Material.color(Material.DeepPurple, Material.Shade400);
        }
        else if (fuel_mix_c.value === 'c3h8')
        {
            fuelDisp.text = showPhase ? "Propane (" + phaseDisp + ")" : "Propane";
            fuelDisp.color = Material.color(Material.Green, Material.Shade400);
        }
        else if (['nist1', 'nist2', 'rg2', 'gu1', 'gu2'].indexOf(fuel_mix_c.value) >= 0)
        {
            if (showPhase)
            {
                fuelDisp.text = "Blend (" + fuel_mix_c.value_display + ", " + phaseDisp + ")";
            }
            else
            {
                fuelDisp.text = "Blend (" + fuel_mix_c.value_display + ")";
            }
            fuelDisp.color = Material.color(Material.DeepOrange, Material.Shade200);
        }
        else
        {
            fuelDisp.text = showPhase ? "Blend (manual, " + phaseDisp + ")" : "Blend (manual)";
            fuelDisp.color = Material.color(Material.Brown, Material.Shade400);
        }
    }

    function changeForm(formId, subFormId)
    {
        const formBtns = [stateFormBtn, qraFormBtn, plumeFormBtn, accumFormBtn, flameFormBtn, uoFormBtn, densityFormBtn,
                            tankMassFormBtn, massFlowFormBtn];

        activeFormId = formId;

        qraFormBtn.implicitHeight = 70;
        qraSubFormBtn1.implicitHeight = 0;
        qraSubFormBtn2.implicitHeight = 0;
        qraSubFormBtn3.implicitHeight = 0;

        formBtns.forEach((btn) => {btn.bgColor = sideMenuColor; });

        if (formId === plumeFormId)
        {
            plumeFormBtn.bgColor = formBgColor;
            formLoader.source = "physics/PlumeForm.qml";
        }
        else if (formId === flameFormId)
        {
            flameFormBtn.bgColor = formBgColor;
            formLoader.source = "physics/FlameForm.qml";
        }
        else if (formId === accumFormId)
        {
            accumFormBtn.bgColor = formBgColor;
            formLoader.source = "physics/AccumForm.qml";
        }
        else if (formId === uoFormId)
        {
            uoFormBtn.bgColor = formBgColor;
            formLoader.source = "physics/UnconfOverpForm.qml";
        }
        else if (formId === densityFormId)
        {
            densityFormBtn.bgColor = formBgColor;
            formLoader.source = "physics/DensityForm.qml";
        }
        else if (formId === tankMassFormId)
        {
            tankMassFormBtn.bgColor = formBgColor;
            formLoader.source = "physics/TankMassForm.qml";
        }
        else if (formId === massFlowFormId)
        {
            massFlowFormBtn.bgColor = formBgColor;
            formLoader.source = "physics/MassFlowForm.qml";
        }

        else if (formId === qraFormId)
        {
            qraSubFormBtn1.implicitHeight = 24;
            qraSubFormBtn2.implicitHeight = 24;
            qraSubFormBtn3.implicitHeight = 24;
            qraFormBtn.implicitHeight = 50;
            qraFormBtn.bgColor = formBgColor;
            formLoader.source = "QraForm.qml";

            if (formLoader.status === Loader.Ready)
            {
                if (subFormId === 2) formLoader.item.scrollToSection2();
                else if (subFormId === 3) formLoader.item.scrollToSection3();
                else formLoader.item.scrollToSection1();
            }
        }
        else
        {
            stateFormBtn.bgColor = formBgColor;
            formLoader.source = "StateForm.qml";
        }
        activeForm = formLoader.item;

        // refresh alerts after switch
        var resp = app_form.check_valid_shared_state();
        refreshAlerts("" + resp[1], parseInt(resp[0]));
    }

    /// Top-level function that restores deleted analysis queue item onto listview queue
    function restoreAnalysis(analysis_id)
    {
        queue.restore_item(ac_id);
    }


    /// Displays generic message events
    function handleNewMessageEvent(msg)
    {
        // post temp message to allow undo
        let ctx = {
            "msg": msg,
            "callback": function() { ; }
        };
        messagesView.model.append(ctx);
        resultContainer.close();
    }

    function expo(x, f) {
        return Number.parseFloat(x).toExponential(f);
    }


    MessagePopupSmall {
        id: shutdownPopup
        allowClose: false
        isCentered: true
        header: "Closing HyRAM+"
        content: "Please wait while HyRAM+ shuts down..."
        onOpened: app_form.shutdown()
    }

    Popup {
        id: alertPopup
        x: 80
        y: parent.height - 50 - 10
//        width: alertText.width + alertIcon.width
        width: alertContents.width
        height: 50
        modal: false
        dim: false
        closePolicy: Popup.NoAutoClose
        padding: 0

        Rectangle {
            id: alertRect
            anchors.fill: parent
            color: color_warning
            radius: 5

            Row {
                id: alertContents
                spacing: 5
                anchors.verticalCenter: parent.verticalCenter
                leftPadding: 8

                AppIcon {
                    id: alertIcon
                    source: 'circle-exclamation-solid'
                    iconColor: color_text_warning
                    anchors.verticalCenter: parent.verticalCenter
                }
                TextEdit {
                    id: alertText
                    text: "Test alert NOTIFICATION lorem ipsum"
                    rightPadding: 10
                    color: color_text_warning
                    readOnly: true
                    selectByMouse: true
                    font.pointSize: 12
                    font.bold: true
                    anchors.verticalCenter: parent.verticalCenter
                }
            }
        }
    }



    // ======================
    // ==== File Dialogs ====
    FileDialog {
        id: saveFileDialog
        onAccepted: {
            app_form.save_file_as(selectedFile);
        }
        fileMode: FileDialog.SaveFile
        nameFilters: ["HyRAM+ files (*.hrm)", "JSON files (*.JSON *.json)"]
        defaultSuffix: "hrm"
    }

    FileDialog {
        id: loadFileDialog
        onAccepted: {
            app_form.load_save_file(selectedFile);
        }
        fileMode: FileDialog.OpenFile
        nameFilters: ["HyRAM+ files (*.hrm)", "JSON files (*.JSON *.json)"]
        defaultSuffix: "hrm"
    }

    // ==== App settings ====
   SettingsPage {
       id: settingsPage
   }


    // =======================
    // ==== RESULTS Popup ====
    Popup {
        id: resultContainer
        x: parent.width * 0.02
        y: parent.height * 0.02
        width: parent.width * 0.92
        height: parent.height * 0.96
        modal: true
        focus: true

        Loader {
            id: resultPageLoader
            anchors.fill: parent

           onStatusChanged: {
               // if (resultPageLoader.status === Loader.Ready)
               // {
               //     resultPageLoader.item.updateContent();
               //     resultContainer.open();
               // }
           }
        }

        function show(resultForm, qIndex) {
            let analysisType = resultForm.analysis_type;

            if (analysisType === 'plume')
            {
                resultPageLoader.setSource("physics/PlumeResultPage.qml", {"qIndex": qIndex, "pyform": resultForm});
            }
            else if (analysisType === 'accum')
            {
                resultPageLoader.setSource("physics/AccumResultPage.qml", {"qIndex": qIndex, "pyform": resultForm});
            }
            else if (analysisType === 'flame')
            {
                resultPageLoader.setSource("physics/FlameResultPage.qml", {"qIndex": qIndex, "pyform": resultForm});
            }
            else if (analysisType === 'uo')
            {
                resultPageLoader.setSource("physics/UnconfOverpResultPage.qml", {"qIndex": qIndex, "pyform": resultForm});
            }
            else if (analysisType === 'qra')
            {
                resultPageLoader.setSource("QraResultPage.qml", {"qIndex": qIndex, "pyform": resultForm});
            }

            if (resultPageLoader.status === Loader.Ready)
            {
                resultPageLoader.item.updateContent();
                resultContainer.open();
            }
            else if (resultPageLoader.status === Loader.Error)
            {
                app_form.do_log("Loader error:");
                var msg = resultPageLoader.sourceComponent.errorString();
                app_form.do_log(msg);

            }

            // resultPageLoader.item.updateContent();
            // resultContainer.open();
        }
    }

    AboutPage {
        id: aboutPopup
        logoSrc: "assets/logo/banner_slim_lg.jpg"
        title: "About HyRAM+"
        urlDescrip: '<html><style type="text/css"></style><a href="https://hyram.sandia.gov">HyRAM+ Website</a></html>'
        url: "https://hyram.sandia.gov"
        height: parent.height * 0.9
    }

    // =====================================
    // ==== Main Contents: form & queue ====
    RowLayout {
        anchors.fill: parent
        spacing: 0

        // ==============================================
        // ==== Left Section (Menu & Parameter Form) ====
        Pane {
            id: inputSection
            Layout.fillHeight: true
            Layout.fillWidth: true
            topPadding: 0
            leftPadding: 0
            bottomPadding: 0
            rightPadding: 0

            ColumnLayout {
                anchors.fill: parent
                spacing: 0

                // Main menu
                MenuBar {
                    id: mainMenu
                    Layout.fillWidth: true

                    background: Rectangle {
                        anchors.fill: parent
                        color: "white"
                    }

                     Connections {}

                     Menu {
                         title: "File"
                         Action {
                             text: "New..."
                             onTriggered: app_form.load_new_form()
                         }
                         Action {
                             text: "Open..."
                             onTriggered: loadFileDialog.open()
                         }
                         Action {
                             text: "Save"
                             onTriggered: {
                                 if (app_form.save_file_exists)
                                 {
                                     app_form.save_file();
                                 }
                                 else
                                 {
                                     saveFileDialog.open();  // no existing save-file
                                 }
                             }
                         }

                         Action {
                             text: "Save As..."
                             onTriggered: saveFileDialog.open()
                         }
                         MenuSeparator { }

                         Action {
                             text: "Open Settings"
                             icon.source: '../hygu/resources/icons/gear-solid.svg'
                             onTriggered:  settingsPage.open();
                         }
                         Action {
                             text: "Open Data Directory"
                             icon.source: '../hygu/resources/icons/folder-open-solid.svg'
                             onTriggered: app_form.open_data_directory()
                         }

                         MenuSeparator { }
                         Action {
                             text: "Quit"
                             onTriggered: doShutdown()
                         }
                     }
                     // Menu {
                     //     title: "Demo"
                     //     Action {
                     //         text: "Load Demo 1"
                     //         onTriggered: app_form.load_demo1()
                     //     }
                     //     Action {
                     //         text: "Load Demo 2"
                     //         onTriggered: app_form.load_demo2()
                     //     }
                     // }
                    Menu {
                        title: "Help"
                        Action {
                            text: "&About"
                            onTriggered: aboutPopup.open()
                        }
                    }
                     Menu {
                         id: devMenu
                         title: "Dev"
                         Action {
                             text: "Print state"
                             onTriggered: app_form.print_state()
                         }
                         Action {
                             text: "Print history"
                             onTriggered: app_form.print_history()
                         }
                     }


                    RowLayout {
                        id: buttonBar
                        parent: mainMenu
                        height: 40
                        anchors.top: parent.top
                        anchors.left: parent.left
                        anchors.right: parent.right
                        anchors.leftMargin: 156
                        // anchors.fill: parent
                        spacing: 4

                        Item {
                            height: 32
                            Layout.preferredWidth: 42
                        }

                        IconButton {
                            id: openSettingsBtn
                            Layout.preferredWidth: 40
                            Layout.preferredHeight: 36
                            img: 'gear-solid'
                            tooltip: "View HyRAM+ settings"
                            onClicked: {
                                settingsPage.open();
                            }
                        }

                         IconButton {
                             Layout.preferredWidth: 40
                             Layout.preferredHeight: 36
                             id: topSaveBtn
                             img: 'save-solid'
                             tooltip: "Save changes"
                             onClicked: {
                                 if (app_form.save_file_exists)
                                 {
                                     app_form.save_file();
                                 }
                                 else
                                 {
                                     saveFileDialog.open();  // no existing save-file
                                 }
                             }

                         }

                         IconButton {
                             Layout.preferredWidth: 40
                             Layout.preferredHeight: 36
                             id: undoBtn
                             img: 'rotate-left-solid'
                             tooltip: "Undo last change"
                             onClicked: app_form.undo()

                         }

                         IconButton {
                             Layout.preferredWidth: 40
                             Layout.preferredHeight: 36
                             id: redoBtn
                             img: 'rotate-right-solid'
                             tooltip: "Revert last undo"
                             onClicked: app_form.redo()
                         }

                        Item {
                            height: 32
                            // width: 20
                            Layout.fillWidth: true
                        }

                        Text {
                            height: 32
                            font.pointSize: 13
                            font.bold: true
                            text: "Active fuel: "
                            horizontalAlignment: Text.AlignRight
                        }

                        Text {
                            id: fuelDisp
                            height: 32
                            Layout.rightMargin: 20
                            // Layout.preferredWidth:
                            font.pointSize: 13
                            font.bold: true
                            horizontalAlignment: Text.AlignRight
                        }
                    }
                }

//                Rectangle {   height: 10 Layout.fillWidth: true color: "blue" }  // layout guide

                RowLayout {
                    spacing: 4

                    // Side menu
                    Rectangle {
                        id: sideMenuRect
                        color: sideMenuColor
                        Layout.preferredWidth: 170
                        Layout.fillHeight: true

                        ColumnLayout {
                            id: sideMenuLayout
                            spacing: 0
                            anchors.fill: sideMenuRect

                            SideMenuTextButton {
                                id: stateFormBtn
                                btnText: "Shared State"
                                img: "gauge-solid"
                                onClicked: { changeForm(stateFormId); }
                            }

                            SideMenuTextButton {
                                id: qraFormBtn
                                btnText: "QRA"
                                Layout.topMargin: 0
                                Layout.bottomMargin: 0
                                img: "person-solid"
                                onClicked: { changeForm(qraFormId, 1); }
                                Behavior on implicitHeight { NumberAnimation { duration: 200; }}
                            }
                            SideMenuSmallTextButton {
                                id: qraSubFormBtn1
                                btnText: "System Description"
                                // img: "circle-regular"
                                img: "building-user-solid"
                                implicitHeight: 0
                                onClicked: { changeForm(qraFormId, 1); }
                                Behavior on implicitHeight { NumberAnimation { duration: 200; }}
                                visible: implicitHeight > 1;
                            }
                            SideMenuSmallTextButton {
                                id: qraSubFormBtn2
                                btnText: "Frequencies / Prob..."
                                img: "scroll-solid"
                                implicitHeight: 0
                                onClicked: { changeForm(qraFormId, 2); }
                                Behavior on implicitHeight { NumberAnimation { duration: 200; }}
                                visible: implicitHeight > 1;
                            }
                            SideMenuSmallTextButton {
                                id: qraSubFormBtn3
                                btnText: "Consequence Models"
                                img: "fire-solid"
                                implicitHeight: 0
                                onClicked: { changeForm(qraFormId, 3); }
                                Behavior on implicitHeight { NumberAnimation { duration: 200; }}
                                visible: implicitHeight > 1;
                                bottomInset: -10
                            }

                            SideMenuTextButton {
                                id: plumeFormBtn
                                btnText: "Plume Dispersion"
                                img: "wind-solid"
                                onClicked: { changeForm(plumeFormId); }
                            }
                            SideMenuTextButton {
                                id: accumFormBtn
                                btnText: "Accumulation"
                                img: "warehouse-solid"
                                onClicked: { changeForm(accumFormId); }
                            }
                            SideMenuTextButton {
                                id: flameFormBtn
                                btnText: "Jet Flame"
                                img: "fire-solid"
                                onClicked: { changeForm(flameFormId); }
                            }
                            SideMenuTextButton {
                                id: uoFormBtn
                                btnText: "Unconfined Overpressure"
                                img: "explosion-solid"
                                onClicked: { changeForm(uoFormId); }
                            }

                            SideMenuTextButton {
                                id: densityFormBtn
                                btnText: "Density Calculations"
                                img: "gauge-high-solid"
                                onClicked: { changeForm(densityFormId); }
                            }
                            SideMenuTextButton {
                                id: tankMassFormBtn
                                btnText: "Tank Mass Calculations"
                                img: "fill-solid"
                                onClicked: { changeForm(tankMassFormId); }
                            }
                            SideMenuTextButton {
                                id: massFlowFormBtn
                                btnText: "Mass Flow Rate"
                                img: "arrow-up-from-ground-water-solid"
                                onClicked: { changeForm(massFlowFormId); }
                            }
                            Item { height: 100 }
                            Item { Layout.fillHeight: true}

//                            Rectangle {
//                                color: "blue"
//                                Layout.preferredHeight: 10
//                                Layout.preferredWidth: 50
//                                Layout.fillHeight: true
//                            }
                        }
                    }

                    Item {
                        id: formContainer
                        Layout.fillWidth: true
                        Layout.fillHeight: true
                        Layout.topMargin: 20
                        // anchors.fill: parent

                        Loader {
                            id: formLoader
                            anchors.fill: parent
                        }
                    }
                }
            }
        }

        // Rectangle {
            // Layout.preferredWidth: 4
            // Layout.fillHeight: true
        // }

        // ========================================
        // ==== RIGHT SECTION (ANALYSIS QUEUE) ====
        Pane {
            id: resultQueuePane
            Layout.preferredWidth: 320
            Layout.fillHeight: true
            spacing: 2
            topPadding: 2
            leftPadding: 0
            bottomPadding: 0
            rightPadding: 4

            Image {
                id: queueHeaderImage
                source: "../assets/logo/icon_banner.jpg"
                width: 320
                fillMode: Image.PreserveAspectFit
            }

            FormSectionHeader {
                id: queueHeaderId
                title: "Analyses"
                anchors.top: queueHeaderImage.bottom
                topPadding: 4
                bottomPadding: 2
                titleRef.bottomPadding: 2
                titleRef.font.italic: true
                rectRef.color: Material.color(Material.Grey)
                rectRef.height: 1
                titleRef.horizontalAlignment: Text.AlignHCenter
                titleRef.width: parent.width
                fontSize: 17
            }

            ListView {
                id: queueListView
                anchors.horizontalCenter: parent.horizontalCenter
                width: 320
                anchors.top: queueHeaderId.bottom
                anchors.topMargin: 2
                height: parent.height - queueHeaderImage.height - 4

                boundsBehavior: Flickable.StopAtBounds
                boundsMovement: Flickable.FollowBoundsBehavior
                flickDeceleration: 10000
                clip: true

                spacing: 4
                model: queue  // py QueueDisplay
                delegate:  QueueItem {
                    fm: model.item
                    qIndex: index
                }

                // called from list elem (delete btn clicked)
                function removeItem(idx)
                {
                    // save refs to ac before deleting elem
                    var elem = queueListView.itemAtIndex(idx);
                    var fm = elem.fm;

                    queue.remove_item(idx);

                    // post temp message to allow undo
                    let ctx = {
                        "msg": "Analysis deleted. to undo, click here.",
                        "callback": function() { restoreItem(fm.analysis_id) }
                    };
                    messagesView.model.append(ctx);
                }

                // called from queue item (cancel btn clicked)
                function analysisCanceled(idx)
                {
                    queue.remove_item(idx);

                    let ctx = {
                        "msg": "Analysis canceled",
                        "callback": function() { ; }
                    };
                    messagesView.model.append(ctx);

                }

                // restores deleted AC list element
                function restoreItem(ac_id)
                {
                    queue.restore_item(ac_id);
                }

                remove: Transition {
                    ParallelAnimation {
                        NumberAnimation { property: "opacity"; to: 0; duration: 100 }
                        NumberAnimation { properties: "height"; to: 0; duration: 200 }
                    }
                }
                removeDisplaced: Transition {
                    NumberAnimation { properties: "y"; duration: 200 }
                }

            }
        }

    }

    // ===========================
    // ==== Notification Area ====
    // Temporary top msg display. Messages can include a callback function which is executed when the message is clicked.
    // Messages disappear after 5s.

    ListView {
        id: messagesView
        model: ListModel { dynamicRoles: true }
        orientation: Qt.Vertical
        verticalLayoutDirection: ListView.BottomToTop
        width: 240
        height: 150
        spacing: 4
        anchors.bottom: parent.bottom
        anchors.bottomMargin: 5
        anchors.right: parent.right
        interactive: false

        remove: Transition {
            ParallelAnimation {
                NumberAnimation { property: "opacity"; to: 0; duration: 100 }
                NumberAnimation { properties: "height"; to: 0; duration: 200 }
            }
        }
        removeDisplaced: Transition {
            NumberAnimation { properties: "y"; duration: 200 }
        }

        // TODO: add transitions don't work because analysis item is added via backend(?)
        add: Transition {
            ParallelAnimation {
                NumberAnimation { property: "opacity"; to: 1; duration: 100 }
                NumberAnimation { properties: "height"; to: 30; duration: 200 }
            }
        }
        addDisplaced: Transition {
            NumberAnimation { properties: "y"; duration: 200 }
        }

        delegate:  Rectangle {
            color: Material.color(Material.Blue, Material.Shade100)
            // width: messagesView.width
            width: msgLabel.width
            anchors.right: parent?.right
            anchors.rightMargin: 4
            radius: 5
            height: 30

            Component.onCompleted: {
                tmr.start();
            }

            Label {
                id: msgLabel
                anchors.verticalCenter: parent.verticalCenter
                anchors.right: parent.right
                leftPadding: 5
                rightPadding: 5
                horizontalAlignment: Text.AlignRight
                font.pointSize: contentFontSize
                font.weight: 500
                font.italic: true
                text: msg
                color: "#333333"
            }

            MouseArea {
                id: msgArea
                anchors.fill: parent
                anchors.centerIn: parent
                onClicked: function(mouse) {
                    if (typeof callback === "function")
                    {
                        callback();
                        // immediately remove msg
                        messagesView.model.remove(index);
                        mouse.accepted = true;  // don't propagate
                    }
                }
            }

            Timer {
                id: tmr
                interval: 4000
                running: true
                repeat: false
                onTriggered: messagesView.model.remove(index)
            }

        }
    }
 }
