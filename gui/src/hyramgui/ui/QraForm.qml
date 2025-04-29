/* Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

 * You should have received a copy of the GNU General Public License along with HyRAM+.
 * If not, see https://www.gnu.org/licenses/.
 */

import QtQuick 2.12
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Dialogs
import QtQuick.Window
import QtQuick.Controls.Material 2.12

import "../hygu/ui/components"
import "../hygu/ui/components/buttons"
import "../hygu/ui/parameters"
import "../hygu/ui/pages"
import "components"
import "parameters"
import hyram.classes


FormPage {
    property int componentFieldW: 320;
    property int occGroupPk: 0;
    property int alertStatus: 1;
    property string alertMsg: "";
    property bool hasAlert: alertStatus !== 1;

    Connections {
        target: app_form

        function onAlertChanged(genMsg, genLevel) {
            updateAlerts();
        }
    }

    function updateAlerts() {
        // Get shared-state and QRA ValidationResponses separately
        var resp = app_form.check_valid_shared_state();
        alertStatus = parseInt(resp[0]);
        alertMsg = "" + resp[1];

        // update status/msg for QRA if shared state is valid
        if (!hasAlert)
        {
            resp = app_form.check_valid_qra();
            alertStatus = parseInt(resp[0]);
            alertMsg = "" + resp[1];
        }
    }

    function scrollToSection1()
    {
        paramContainer.contentY = sectionSystemDescrip.y;
    }

    function scrollToSection2()
    {
        paramContainer.contentY = sectionFreqs.y + 360;
    }

    function scrollToSection3()
    {
        paramContainer.contentY = sectionConseqModels.y + 284;
    }


    function refreshForm()
    {
        var diffKeys = app_form.get_diff_keys;
        if (diffKeys.indexOf('occupant_data') > -1)
        {
            setUiOccupantData();
        }
        if (diffKeys.indexOf('ignition_data') > -1)
        {
            setUiIgnitionData();
        }

        // conditionally display either pipe inner diam or pipe outer diam + thickness
        pipeDInput.enabled = pipe_od_c.is_null && pipe_thick_c.is_null;
        pipeDInput.opacity = pipe_od_c.is_null && pipe_thick_c.is_null ? 1 : fadeVal;

        pipeThickInput.enabled = pipe_d_c.is_null;
        pipeThickInput.opacity = pipe_d_c.is_null ? 1 : fadeVal;
        pipeOdInput.enabled = pipe_d_c.is_null;
        pipeOdInput.opacity = pipe_d_c.is_null ? 1 : fadeVal;

        var isUnchoked = app_form.is_unchoked;
        massFlowLeakSelector.enabled = isUnchoked;
        massFlowLeakSelector.opacity = isUnchoked ? 1 : fadeVal;
        massFlowInput.enabled = isUnchoked;
        massFlowInput.opacity = isUnchoked ? 1 : fadeVal;

        updateAlerts();
    }

    // ==================
    // OCCUPANT FUNCTIONS

    /**
     * Adds new occupant group and persists to db.
     * @param index
     */
    function addOccupantGroup() {
        var field;
        var comp = Qt.createComponent("components/OccupantField.qml");
        if (comp.status === Component.Ready) {
            field = comp.createObject(occupantContainer);
            setModelOccupantData();
        }
    }

    /**
     * Removes selected occupant group and persists to db.
     * @param index
     */
    function removeOccupantGroup(obj) {
        if (occupantContainer.children.length === 1) return;

        obj.isAlive = false;
        obj.destroy();
        setModelOccupantData();
    }

    /**
     * Updates UI display of occupant groups based on model data.
     * @constructor
     */
    function setUiOccupantData()
    {
        var occDataStr = app_form.occupant_data;
        var occDataArr = JSON.parse(occDataStr);
        var nStaleFields = occupantContainer.children.length;
        var nNewFields = occDataArr.length;

        // clear stale
        for (let i = 0; i < nStaleFields; i++)
        {
            var obj = occupantContainer.children[i];
            if (obj.isAlive)
            {
                obj.isAlive = false;
                obj.destroy();
            }
        }

        for (let i = 0; i < nNewFields; i++)
        {
            var field;
            var objData = occDataArr[i];
            var comp = Qt.createComponent("components/OccupantField.qml");
            field = comp.createObject(occupantContainer);
            if (comp.status === Component.Ready)
            {
                field.setFromModel(objData);
            }
        }
    }

    /**
     * Updates backend model data of occupant groups.
     */
    function setModelOccupantData()
    {
        var arr = [];
        var grps = occupantContainer.children;
        for (let i=0; i<grps.length; i++)
        {
            var obj = grps[i];
            if (obj.isAlive)
            {
                arr.push(obj.getData());
            }
        }

        var jsonStr = JSON.stringify(arr);
        app_form.set_occupant_data(jsonStr);
    }

    // ==================
    // IGNITION FUNCTIONS
    function addIgnition() {
        if (newIgnitionInput.isNull) return;

        var threshold = parseFloat(newIgnitionInput.value);
        if (!isNaN(threshold))
        {
            app_form.add_ignition(threshold);
        }
    }

    /**
     * Removes selected Ignition and persists to db.
     * @param index
     */
    function removeIgnition(index) {
        if (ignitionContainer.children.length === 1) return;

        app_form.remove_ignition(index)
    }

    /**
     * Updates UI display of ignition groups based on model data.
     * @constructor
     */
    function setUiIgnitionData()
    {
        var ignDataStr = app_form.ignition_data;
        var ignDataArr = JSON.parse(ignDataStr);
        var nStaleFields = ignitionContainer.children.length;
        var nNewFields = ignDataArr.length;

        // clear stale
        for (let i = 0; i < nStaleFields; i++)
        {
            var obj = ignitionContainer.children[i];
            if (obj.isAlive)
            {
                obj.isAlive = false;
                obj.destroy();
            }
        }

        for (let i = 0; i < nNewFields; i++)
        {
            var field;
            var objData = ignDataArr[i];
            var comp = Qt.createComponent("components/IgnitionField.qml");
            field = comp.createObject(ignitionContainer);
            if (comp.status === Component.Ready)
            {
                // disallow deletion if only 2 elems, or if rendering final elem
                var hideDelete = i === nNewFields - 1 || nNewFields <= 2;
                field.setFromModel(objData, i, hideDelete);
            }
        }
    }

    /**
     * Updates backend model data of single ignition entry.
     */
    function setModelIgnitionData(index)
    {
        if (ignitionContainer.children.length > index)
        {
            var ign = ignitionContainer.children[index];
            if (ign.isAlive)
            {
                var jsonStr = JSON.stringify(ign.getData());
                app_form.set_ignition_data(index, jsonStr);
            }
        }
        // var arr = [];
        // var grps = ignitionContainer.children;
        // for (let i=0; i<grps.length; i++)
        // {
        //     var obj = grps[i];
        //     if (obj.isAlive)
        //     {
        //         arr.push(obj.getData());
        //     }
        // }
        //
        // var jsonStr = JSON.stringify(arr);
        // app_form.set_ignition_data(jsonStr);
    }


    Component.onCompleted:
    {
        refreshForm();
        setUiOccupantData();
        setUiIgnitionData();
    }

    ColumnLayout {
        width: parent.width;
        anchors.fill: parent  // expand height when window expands

        // Form header
        Text {
            font.pointSize: header1FontSize
            id: formHeader
            font.weight: 600
            Layout.leftMargin: 10
            text: "Quantitative Risk Analysis"
        }

        Rectangle {
            height: 2
            Layout.fillWidth: true
            Layout.leftMargin: 10
            Layout.rightMargin: 4
            color: Material.color(Material.Blue, Material.Shade400)
        }
        Text {
            font.pointSize: contentFontSize + 1
            id: formDescrip
            font.italic: true
            Layout.leftMargin: 10
            text: ("Estimate system risk for hydrogen and other alternative fuels.
            Includes consequences and fatality risk of jet flames and overpressures for different release sizes, each
            of which can be predicted to occur with different frequencies.")
            wrapMode: Text.WordWrap
            textFormat: Text.RichText
            Layout.preferredWidth: 750
            Layout.maximumWidth: 750
        }

        Item {
            id: inputSpacer2
            Layout.preferredHeight: 4
        }

        FlickableForm
        {
            id: paramContainer
            contentHeight: paramLayout1.height + paramLayout2.height + paramLayout3.height + 20 + 120
            h: 540

            // ==========================
            // ==== Parameter Inputs ====
            Item {
                id: paramColContainer
                Layout.fillHeight: true
                height: paramLayout1.height + paramLayout2.height + paramLayout3.height

                ColumnLayout {
                    id: paramLayout1

                    FormSectionHeader {
                        id: sectionSystemDescrip
                        title: "System Description";
                        Layout.topMargin: 10
                        iconSrc: 'building-user-solid'
                    }

                    FormSectionHeader2 {title: "Components"; }
                    Header3Description {
                        txt: "Number of each type of component in system"
                        Layout.bottomMargin: 5
                    }

                }

                GridLayout {
                    id: paramLayout2
                    anchors.top: paramLayout1.bottom
                    columns: 2
                    rows: 7
                    columnSpacing: 8
                    flow: GridLayout.TopToBottom

                    IntParamField {
                        param: n_compressors_c
                        tipText: "Number of compressor/pump components in system"
                        Layout.preferredWidth: componentFieldW
                    }
                    IntParamField {
                        param: n_vessels_c
                        tipText: "Number of vessel components (cylinders, tanks, etc.) in system"
                        Layout.preferredWidth: componentFieldW
                    }
                    IntParamField {
                        param: n_valves_c
                        tipText: "Number of valve components in system"
                        Layout.preferredWidth: componentFieldW
                    }
                    IntParamField {
                        param: n_instruments_c
                        tipText: "Number of instrument components in system"
                        Layout.preferredWidth: componentFieldW
                    }
                    IntParamField {
                        param: n_joints_c
                        tipText: "Number of joints in system"
                        Layout.preferredWidth: componentFieldW
                    }
                    IntParamField {
                        param: n_hoses_c
                        tipText: "Number of hoses in system"
                        Layout.preferredWidth: componentFieldW
                    }
                    IntParamField {
                        param: n_filters_c
                        tipText: "Number of filter components in system"
                        Layout.preferredWidth: componentFieldW
                    }

                    IntParamField {
                        param: n_flanges_c
                        tipText: "Number of flange components in system"
                        Layout.preferredWidth: componentFieldW
                    }
                    IntParamField {
                        param: n_exchangers_c
                        tipText: "Number of heat exchangers in system"
                        Layout.preferredWidth: componentFieldW
                    }
                    IntParamField {
                        param: n_vaporizers_c
                        tipText: "Number of vaporizers components in system"
                        Layout.preferredWidth: componentFieldW
                    }
                    IntParamField {
                        param: n_arms_c
                        tipText: "Number of loading arms in system"
                        Layout.preferredWidth: componentFieldW
                    }
                    IntParamField {
                        param: n_extra1_c
                        tipText: "Additional components of an unspecified type"
                        Layout.preferredWidth: componentFieldW
                    }
                    IntParamField {
                        param: n_extra2_c
                        tipText: "Second set of additional components of an unspecified type"
                        Layout.preferredWidth: componentFieldW
                    }
                }

                ColumnLayout {
                    id: paramLayout3
                    anchors.top: paramLayout2.bottom

                    FormSectionHeader3 {title: "Interconnecting Piping" }
                    // Header3Description {
                    //     txt: "Size of interconnecting piping"
                    //     Layout.bottomMargin: 5
                    // }

                    FloatParamField {
                        param: pipe_l_c
                        Layout.preferredWidth: componentFieldW
                    }
                    FloatNullableParamField {
                        id: pipeDInput
                        param: pipe_d_c
                        Layout.preferredWidth: componentFieldW
                        input.tooltip.text: "Enter a positive value. Leave blank to calculate inner diameter."
                    }
                    FloatNullableParamField {
                        id: pipeOdInput
                        param: pipe_od_c
                        Layout.preferredWidth: componentFieldW
                        input.tooltip.text: "Enter a positive value. Leave blank to enter inner diameter."
                    }
                    FloatNullableParamField {
                        id: pipeThickInput
                        param: pipe_thick_c
                        Layout.preferredWidth: componentFieldW
                        input.tooltip.text: "Enter a positive value. Leave blank to enter inner diameter."
                    }

                    Item { Layout.preferredHeight: 5 }

                    Rectangle {
                        Layout.preferredHeight: unchokedInputs.height + 10
                        Layout.preferredWidth: unchokedInputs.width + 10
                        radius: 3
                        border.width: 1
                        border.color: "#e3e3e3"
                        color: formBgColor

                        ColumnLayout {
                            id: unchokedInputs
                            anchors.centerIn: parent
                            width: 400
                            anchors.margins: 10

                            Text {
                                text: "Release mass flow rate (if unchoked):"
                                font.italic: true
                                horizontalAlignment: Text.AlignLeft
                                Layout.alignment: Qt.AlignVCenter
                                font.pointSize: labelFontSize
                            }
                            ChoiceParamField {
                                id: massFlowLeakSelector
                                param: mass_flow_leak_c
                                Layout.preferredWidth: componentFieldW
                                enabled: app_form.is_unchoked
                            }
                            FloatParamField {
                                id: massFlowInput
                                param: mass_flow_c
                                Layout.preferredWidth: componentFieldW
                                enabled: app_form.is_unchoked
                            }

                        }
                    }

                    FormSectionHeader3 {title: "Refueling Demands" }

                    IntParamField {
                        param: n_vehicles_c
                        Layout.preferredWidth: componentFieldW
                        // input.tooltip.text: "Enter a non-negative value"
                    }
                    FloatParamField {
                        param: n_fuelings_c
                        Layout.preferredWidth: componentFieldW
                        showUnitSelector: false
                        // input.tooltip.text: "Enter a non-negative value"
                    }
                    FloatParamField {
                        param: n_vehicle_days_c
                        Layout.preferredWidth: componentFieldW
                        showUnitSelector: false
                        // input.tooltip.text: "Enter a non-negative value"
                    }

                    Item { height: 5}

                    RowLayout
                    {
                        Text {
                            text: "Number of refuelings per year"
                            Layout.preferredWidth: paramLabelWidth
                            font.pointSize: labelFontSize

                            ToolTip {
                                delay: 200
                                timeout: 3000
                                visible: ma.containsMouse
                                text: "tooltip text"
                            }
                            MouseArea {id: ma; anchors.fill: parent; hoverEnabled: true}
                        }

                        Text {
                            id: annualDemandVal
                            text: { (n_vehicles_c.value * n_fuelings_c.value * n_vehicle_days_c.value).toLocaleString(); }
                            font.italic: true
                            font.pointSize: labelFontSize
                            horizontalAlignment: Text.AlignHCenter
                            Layout.preferredWidth: 120
                        }

                    }



                    FormSectionHeader2 {
                        id: subSectionFacility
                        title: "Occupants";
                    }
                    LeadText2 {
                        text: ("Specify distributions, locations, and number of hours each occupant group spends " +
                         "in location annually. " +
                         "Leak occurs at origin corresponding to (length, height, width) and extends in +x direction.")
                    }

                    Item {height: 10; }

                    // Occupant groups
                    Rectangle {
                        Layout.preferredWidth: 730
                        color: "#fffbfe"

                        height: occupantContainer.childrenRect.height
                        implicitHeight: occupantContainer.childrenRect.height
                        Behavior on implicitHeight { NumberAnimation { duration: 200 } }


                        Item {
                            // id: occupantSection
                            anchors.fill: parent
                            height: occupantContainer.height
                            anchors.topMargin: 5

                            Column {
                                id: occupantContainer
                                spacing: 10

                                Component.onCompleted: { }
                            }
                        }
                    }

                    IconButton {
                        id: newOccupantBtn
                        Layout.preferredHeight: 50
                        Layout.preferredWidth: 44
                        img: 'plus-solid'
                        tooltip: "Add occupant group"
                        onClicked: function() {
                            addOccupantGroup();
                        }
                    }
                    Item {height: 10; }

                    FloatParamField {
                        param: exclusion_c
                        tipText: "Approximate space occupied by leak source (e.g. equipment). Generated occupant positions will exclude this area."
                    }

                    IntParamField {
                        param: seed_c
                        tipText: "Determines pseudo-random occupant positions. Change this between runs to generate new positions."
                    }


                    FormSectionHeader2 {
                        id: subSectionOverrides
                        title: "Analysis Overrides";
                        Layout.topMargin: 8
                    }
                    LeadText2 {
                        text: ("Optional parameter overrides for leak-size fuel release and gas credit. " +
                            "Supersedes component-specific leak calculations. Clear any entered value to disable the " +
                             "override and enable default calculation method.")
                    }

                    FloatNullableParamField {
                        param: override_d01_c
                        label.Layout.preferredWidth: 210
                        label.wrapMode: Text.WordWrap
                        input.tooltip.text: "Enter a non-negative value. Leave blank to not use this override, annual leak frequency will be calculated instead."
                    }

                    FloatNullableParamField {
                        param: override_d1_c
                        label.Layout.preferredWidth: 210
                        label.wrapMode: Text.WordWrap
                        input.tooltip.text: "Enter a non-negative value. Leave blank to not use this override, annual leak frequency will be calculated instead."
                    }

                    FloatNullableParamField {
                        param: override_1_c
                        label.Layout.preferredWidth: 210
                        label.wrapMode: Text.WordWrap
                        input.tooltip.text: "Enter a non-negative value. Leave blank to not use this override, annual leak frequency will be calculated instead."
                    }

                    FloatNullableParamField {
                        param: override_10_c
                        label.Layout.preferredWidth: 210
                        label.wrapMode: Text.WordWrap
                        input.tooltip.text: "Enter a non-negative value. Leave blank to not use this override, annual leak frequency will be calculated instead."
                    }

                    FloatNullableParamField {
                        param: override_100_c
                        label.Layout.preferredWidth: 210
                        label.wrapMode: Text.WordWrap
                        input.tooltip.text: "Enter a non-negative value. Leave blank to not use this override, annual leak frequency will be calculated instead."
                    }

                    FloatParamField {
                        param: detection_c
                        label.Layout.preferredWidth: 210
                        label.wrapMode: Text.WordWrap
                        tipText: "Probability of detection and isolation of leak before any consequential outcomes."
                    }


                    // ==============
                    // DATA / PROBABILITIES
                    FormSectionHeader {
                        id: sectionFreqs
                        title: "Frequencies / Probabilities"
                        iconSrc: 'scroll-solid'
                        Layout.topMargin: 30
                    }

                    FormSectionHeader2 {
                        id: subSectionLeaks
                        title: "Component Annual Leak Frequencies";
                        Layout.topMargin: 8
                    }
                    LeadText2 {
                        text: "Describe parameters for annual leak frequency distributions"
                        Layout.bottomMargin: 5
                    }

                    Rectangle {
                        height: leakSection.childrenRect.height
                        implicitHeight: leakSection.childrenRect.height
                        Behavior on implicitHeight { NumberAnimation { duration: 100 } }

                        Item {
                            anchors.fill: parent
                            height: leakSection.height
                            anchors.topMargin: 5

                            Column {
                                id: leakSection
                                spacing: 15

                                CollapsibleSection {
                                    id: compressorFreqs
                                    title: "Compressors / Pumps";
                                    w: 800
                                    useBorder: true

                                    ColumnLayout {
                                        parent: compressorFreqs.containerRef
                                        Layout.leftMargin: 28

                                        LeakFreqHeaderRow {}
                                        LognormParamField {param: compressor_d01_c}
                                        LognormParamField {param: compressor_d1_c}
                                        LognormParamField {param: compressor_1_c}
                                        LognormParamField {param: compressor_10_c}
                                        LognormParamField {param: compressor_100_c}
                                    }
                                }

                                CollapsibleSection {
                                    id: vesselFreqs
                                    title: "Vessels";
                                    w: 800
                                    useBorder: true

                                    ColumnLayout {
                                        parent: vesselFreqs.containerRef
                                        Layout.leftMargin: 28

                                        LeakFreqHeaderRow {}
                                        LognormParamField {param: vessel_d01_c}
                                        LognormParamField {param: vessel_d1_c}
                                        LognormParamField {param: vessel_1_c}
                                        LognormParamField {param: vessel_10_c}
                                        LognormParamField {param: vessel_100_c}
                                    }
                                }

                                CollapsibleSection {
                                    id: filterFreqs
                                    title: "Filters";
                                    w: 800
                                    useBorder: true

                                    ColumnLayout {
                                        parent: filterFreqs.containerRef
                                        Layout.leftMargin: 28

                                        LeakFreqHeaderRow {}
                                        LognormParamField {param: filter_d01_c}
                                        LognormParamField {param: filter_d1_c}
                                        LognormParamField {param: filter_1_c}
                                        LognormParamField {param: filter_10_c}
                                        LognormParamField {param: filter_100_c}
                                    }
                                }

                                CollapsibleSection {
                                    id: flangeFreqs
                                    title: "Flanges";
                                    w: 800
                                    useBorder: true

                                    ColumnLayout {
                                        parent: flangeFreqs.containerRef
                                        Layout.leftMargin: 28

                                        LeakFreqHeaderRow {}
                                        LognormParamField {param: flange_d01_c}
                                        LognormParamField {param: flange_d1_c}
                                        LognormParamField {param: flange_1_c}
                                        LognormParamField {param: flange_10_c}
                                        LognormParamField {param: flange_100_c}
                                    }
                                }

                                CollapsibleSection {
                                    id: hoseFreqs
                                    title: "Hoses";
                                    w: 800
                                    useBorder: true

                                    ColumnLayout {
                                        parent: hoseFreqs.containerRef
                                        Layout.leftMargin: 28

                                        LeakFreqHeaderRow {}
                                        LognormParamField {param: hose_d01_c}
                                        LognormParamField {param: hose_d1_c}
                                        LognormParamField {param: hose_1_c}
                                        LognormParamField {param: hose_10_c}
                                        LognormParamField {param: hose_100_c}
                                    }
                                }

                                CollapsibleSection {
                                    id: jointFreqs
                                    title: "Joints";
                                    w: 800
                                    useBorder: true

                                    ColumnLayout {
                                        parent: jointFreqs.containerRef
                                        Layout.leftMargin: 28

                                        LeakFreqHeaderRow {}
                                        LognormParamField {param: joint_d01_c}
                                        LognormParamField {param: joint_d1_c}
                                        LognormParamField {param: joint_1_c}
                                        LognormParamField {param: joint_10_c}
                                        LognormParamField {param: joint_100_c}
                                    }
                                }

                                CollapsibleSection {
                                    id: pipeFreqs
                                    title: "Pipes";
                                    w: 800
                                    useBorder: true

                                    ColumnLayout {
                                        parent: pipeFreqs.containerRef
                                        Layout.leftMargin: 28

                                        LeakFreqHeaderRow {}
                                        LognormParamField {param: pipe_d01_c}
                                        LognormParamField {param: pipe_d1_c}
                                        LognormParamField {param: pipe_1_c}
                                        LognormParamField {param: pipe_10_c}
                                        LognormParamField {param: pipe_100_c}
                                    }
                                }

                                CollapsibleSection {
                                    id: valveFreqs
                                    title: "Valves";
                                    w: 800
                                    useBorder: true

                                    ColumnLayout {
                                        parent: valveFreqs.containerRef
                                        Layout.leftMargin: 28

                                        LeakFreqHeaderRow {}
                                        LognormParamField {param: valve_d01_c}
                                        LognormParamField {param: valve_d1_c}
                                        LognormParamField {param: valve_1_c}
                                        LognormParamField {param: valve_10_c}
                                        LognormParamField {param: valve_100_c}
                                    }
                                }

                                CollapsibleSection {
                                    id: instrumentFreqs
                                    title: "Instruments";
                                    w: 800
                                    useBorder: true

                                    ColumnLayout {
                                        parent: instrumentFreqs.containerRef
                                        Layout.leftMargin: 28

                                        LeakFreqHeaderRow {}
                                        LognormParamField {param: instrument_d01_c}
                                        LognormParamField {param: instrument_d1_c}
                                        LognormParamField {param: instrument_1_c}
                                        LognormParamField {param: instrument_10_c}
                                        LognormParamField {param: instrument_100_c}
                                    }
                                }

                                CollapsibleSection {
                                    id: exchangerFreqs
                                    title: "Heat Exchangers";
                                    w: 800
                                    useBorder: true

                                    ColumnLayout {
                                        parent: exchangerFreqs.containerRef
                                        Layout.leftMargin: 28

                                        LeakFreqHeaderRow {}
                                        LognormParamField {param: exchanger_d01_c}
                                        LognormParamField {param: exchanger_d1_c}
                                        LognormParamField {param: exchanger_1_c}
                                        LognormParamField {param: exchanger_10_c}
                                        LognormParamField {param: exchanger_100_c}
                                    }
                                }

                                CollapsibleSection {
                                    id: vaporizerFreqs
                                    title: "Vaporizers";
                                    w: 800
                                    useBorder: true

                                    ColumnLayout {
                                        parent: vaporizerFreqs.containerRef
                                        Layout.leftMargin: 28

                                        LeakFreqHeaderRow {}
                                        LognormParamField {param: vaporizer_d01_c}
                                        LognormParamField {param: vaporizer_d1_c}
                                        LognormParamField {param: vaporizer_1_c}
                                        LognormParamField {param: vaporizer_10_c}
                                        LognormParamField {param: vaporizer_100_c}
                                    }
                                }

                                CollapsibleSection {
                                    id: armFreqs
                                    title: "Loading Arms";
                                    w: 800
                                    useBorder: true

                                    ColumnLayout {
                                        parent: armFreqs.containerRef
                                        Layout.leftMargin: 28

                                        LeakFreqHeaderRow {}
                                        LognormParamField {param: arm_d01_c}
                                        LognormParamField {param: arm_d1_c}
                                        LognormParamField {param: arm_1_c}
                                        LognormParamField {param: arm_10_c}
                                        LognormParamField {param: arm_100_c}
                                    }
                                }

                                CollapsibleSection {
                                    id: extra1Freqs
                                    title: "Extra Component #1";
                                    w: 800
                                    useBorder: true

                                    ColumnLayout {
                                        parent: extra1Freqs.containerRef
                                        Layout.leftMargin: 28

                                        LeakFreqHeaderRow {}
                                        LognormParamField {param: extra1_d01_c}
                                        LognormParamField {param: extra1_d1_c}
                                        LognormParamField {param: extra1_1_c}
                                        LognormParamField {param: extra1_10_c}
                                        LognormParamField {param: extra1_100_c}
                                    }
                                }

                                CollapsibleSection {
                                    id: extra2Freqs
                                    title: "Extra Component #2";
                                    w: 800
                                    useBorder: true

                                    ColumnLayout {
                                        parent: extra2Freqs.containerRef
                                        Layout.leftMargin: 28

                                        LeakFreqHeaderRow {}
                                        LognormParamField {param: extra2_d01_c}
                                        LognormParamField {param: extra2_d1_c}
                                        LognormParamField {param: extra2_1_c}
                                        LognormParamField {param: extra2_10_c}
                                        LognormParamField {param: extra2_100_c}
                                    }
                                }

                                Item {height: 150}  // spacer so anim doesn't overlap below content
                            }
                        }
                    }


                    FormSectionHeader2 {
                        id: subSectionFailures
                        title: "Dispenser Failure Probabilities";
                        Layout.topMargin: 15
                    }
                    LeadText2 {
                        text: "Specify distribution parameters describing annual accident and shutdown failure frequencies for refueling dispensers"
                        font.italic: true
                    }

                    FormSectionHeader3 {
                        title: "Component Failures";
                        Layout.topMargin: 8
                    }

                    DistributionParamField {
                        param: fail_nozzle_po_c
                        tipText: ""
                    }
                    DistributionParamField {
                        param: fail_nozzle_ftc_c
                        tipText: ""
                    }
                    DistributionParamField {
                        param: fail_mvalve_ftc_c
                        tipText: ""
                    }
                    DistributionParamField {
                        param: fail_svalve_ftc_c
                        tipText: ""
                    }
                    DistributionParamField {
                        param: fail_svalve_ccf_c
                        tipText: ""
                    }
                    DistributionParamField {
                        param: fail_pvalve_fto_c
                        tipText: ""
                    }
                    DistributionParamField {
                        param: fail_coupling_ftc_c
                        tipText: ""
                    }

                    FormSectionHeader3 {
                        title: "Accidents";
                        Layout.topMargin: 8
                    }
                    DistributionParamField {
                        param: acc_fuel_overp_c
                        tipText: ""
                    }
                    DistributionParamField {
                        param: acc_driveoff_c
                        tipText: ""
                    }


                    // ======================
                    // IGNITION PROBABILITIES
                    FormSectionHeader2 {
                        id: subSectionIgnitions
                        title: "Ignition Probabilities";
                        Layout.topMargin: 15
                    }
                    LeadText2 {
                        text: "Add/remove flow rate thresholds and modify corresponding ignition probabilities"
                        font.italic: true
                    }

                    // Item {
                    //     Layout.preferredHeight: 5
                    //     Layout.preferredWidth: 20
                    // }
                    RowLayout {
                        Text {
                            text: "Add ignition flow rate threshold"
                            horizontalAlignment: Text.AlignLeft
                            Layout.alignment: Qt.AlignVCenter | Qt.AlignHCenter
                            font.pointSize: labelFontSize
                            Layout.preferredWidth: 198
                            wrapMode: Text.WordWrap
                        }
                        FloatNullInput {
                            id: newIgnitionInput
                            w: 100
                            min: 0
                            isNull: true
                            tooltip.text: "Enter a positive value to add a new threshold"
                        }
                        Text {
                            text: "kg/s"
                            horizontalAlignment: Text.AlignLeft
                            Layout.alignment: Qt.AlignVCenter | Qt.AlignHCenter
                            font.pointSize: labelFontSize
                            Layout.preferredWidth: 40
                            wrapMode: Text.WordWrap
                        }
                        IconButton {
                            id: newIgnitionBtn
                            Layout.preferredHeight: 40
                            Layout.preferredWidth: 40
                            img: 'plus-solid'
                            tooltip: "Add ignition flow rate threshold"
                            onClicked: function() {
                                newIgnitionBtn.forceActiveFocus();
                                addIgnition();
                            }
                        }

                    }

                    Rectangle {
                        Layout.preferredWidth: 730
                        color: "#fffbfe"

                        height: ignitionContainer.childrenRect.height
                        implicitHeight: ignitionContainer.childrenRect.height
                        Behavior on implicitHeight { NumberAnimation { duration: 200 } }


                        Item {
                            anchors.fill: parent
                            height: ignitionContainer.height
                            anchors.topMargin: 5

                            Column {
                                id: ignitionContainer
                                spacing: 10

                                Component.onCompleted: { }
                            }
                        }
                    }


                    // ==================
                    // CONSEQUENCE MODELS
                    FormSectionHeader {
                        id: sectionConseqModels
                        title: "Consequence Models"
                        iconSrc: 'fire-solid'
                        Layout.topMargin: 35
                    }

                    FormSectionHeader3 {title: "Physical Consequence Model Options"; Layout.topMargin: 8; }

                    ChoiceParamField {
                        param: overp_method_c;
                        tipText: "Unconfined overpressure model"
                    }

                    ChoiceParamField {
                        param: mach_speed_c;
                        enabled: overp_method_c.value === "bst"
                        opacity: overp_method_c.value === "bst" ? 1 : 0.3
                        tipText: "Flame front speed for the BST overpressure method"
                    }

                    FloatParamField {
                        param: tnt_factor_c;
                        enabled: overp_method_c.value === "tnt"
                        opacity: overp_method_c.value === "tnt" ? 1 : 0.3
                        input.Layout.preferredWidth: defaultSelectorW;
                        input.Layout.maximumWidth: defaultSelectorW;
                    }

                    FormSectionHeader3 {title: "Harm Model Options"; Layout.topMargin: 8; }

                    ChoiceParamField {
                        param: thermal_probit_c;
                        tipText: "Model calculating individual probability of fatality due to heat flux from a jet fire"
                    }

                    FloatParamField {
                        param: exposure_time_c;
                        input.Layout.preferredWidth: defaultSelectorW;
                        input.Layout.maximumWidth: defaultSelectorW;
                        tipText: "Amount of time individual is exposed to jet fire heat flux"
                    }

                    ChoiceParamField {
                        param: overp_probit_c;
                        label.wrapMode: Text.WordWrap;
                        tipText: "Model calculating individual probability of fatality due to overpressure from an explosion"
                    }

                    // Rectangle {
                    //     visible: overp_method_c.value === 'bauwens' && (overp_probit_c.value === 'head' || overp_probit_c.value === 'coll')
                    //     Layout.preferredWidth: 550
                    //     Layout.preferredHeight: 48
                    //     radius: 5
                    //     color: color_danger
                    //
                    //     Text {
                    //         id: bauwensAlertText
                    //         anchors.margins: 5
                    //         anchors.fill: parent
                    //         horizontalAlignment: Text.AlignLeft
                    //         text: ("Overpressure method 'Bauwens' does not produce impulse values and cannot be used with " +
                    //             "overpressure probit models 'TNO - Head Impact' or 'TNO - Structural Collapse'")
                    //         font.pointSize: labelFontSize
                    //         font.bold: true
                    //         color: color_text_danger
                    //         wrapMode: Text.WordWrap
                    //     }
                    // }

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
                    app_form.request_analysis("qra");
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

            Rectangle {
                id: alert
                color: alertStatus === 0 ? color_danger : color_warning;
                radius: 5
                Layout.alignment: Qt.AlignLeft
                Layout.leftMargin: 10
                Layout.preferredWidth: alertIcon.width + 20 + alertText.contentWidth
                // Layout.preferredWidth: 500
                Layout.bottomMargin: 5
                Layout.preferredHeight: hasAlert ? 24 * alertText.lineCount : 0
                Behavior on Layout.preferredHeight { NumberAnimation { duration: 100 }}
                Behavior on Layout.preferredWidth { NumberAnimation { duration: 100 }}

                RowLayout {
                    id: alertContents
                    spacing: 5
                    anchors.verticalCenter: parent.verticalCenter

                    AppIcon {
                        id: alertIcon
                        source: 'circle-exclamation-solid'
                        iconColor: alertStatus === 0 ? color_text_danger : color_text_warning;
                        Layout.alignment: Qt.AlignVCenter
                        visible: hasAlert
                    }
                    Text {
                        id: alertText
                        text: alertMsg
                        Layout.maximumWidth: 650
                        Layout.maximumHeight: 50
                        Layout.alignment: Qt.AlignVCenter
                        color: alertStatus === 0 ? color_text_danger : color_text_warning;
                        maximumLineCount: 2
                        font.pointSize: labelFontSize
                        font.bold: true
                        wrapMode: Text.WordWrap
                    }
                }

            }
        }

    }
}
