/*
 * Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
 * Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
 * You should have received a copy of the BSD License along with HELPR.
 */
import QtQuick
import QtQuick.Layouts
import QtQuick.Controls
import QtQuick.Window
import QtQuick.Controls.Material 2.12

import "../components"
import hygu.classes


Item {
    property UncertainFormField param;
    property string tipText;
    property bool hasError: false
    property string errorMsg: "test long error msg ERROR HERE"
    property bool isReadOnly: false

    id: paramContainer

    Component.onCompleted:
    {
        refresh();
    }

    function refresh()
    {
        if (hasError)
        {
            paramContainer.Layout.preferredHeight = 80;
            paramLabel.color = color_danger;
            alertMsg.text = errorMsg;
            alertDisplay.visible = true;
        }
        else
        {
            paramContainer.Layout.preferredHeight = 40;
            paramLabel.color = color_primary;
            alertMsg.text = "";
            alertDisplay.visible = false;
        }

        valueLabel.visible = false;
        valueInput.visible = false;

        // truncated
        muLabel.visible = false;
        muInput.visible = false;
        sigmaLabel.visible = false;
        sigmaInput.visible = false;
        normalCLabel.visible = false;
        normalCInput.visible = false;
        normalDLabel.visible = false;
        normalDInput.visible = false;

        // uniform
        uniformALabel.visible = false;
        uniformAInput.visible = false;
        uniformBLabel.visible = false;
        uniformBInput.visible = false;

        unitSelector.currentIndex = param.get_unit_index();


        if (isReadOnly)
        {
            valueLabel.visible = true;
            valueLabel.text.font.italic = true;
            valueInput.visible = true;

            valueInput.text = param.value;
            inputTypeSelector.visible = false;
            valueInput.readOnly = true;
            return;
        }

        let inputType = param.input_type;
        inputTypeSelector.currentIndex = param.get_input_type_index();

        let isProb = inputType !== 'det';
        valueLabel.visible = !isProb;
        valueInput.visible = !isProb;
        uncertaintyLabel.visible = isProb;
        uncertaintySelector.visible = isProb;
        nominalLabel.visible = isProb;
        nominalInput.visible = isProb;

        if (inputType === 'det')
        {
            valueInput.text = param.value;
        }
        else if (inputType === 'uni')
        {
            uniformALabel.visible = true;
            uniformAInput.visible = true;
            uniformBLabel.visible = true;
            uniformBInput.visible = true;

            uncertaintySelector.currentIndex = param.get_uncertainty_index();
            nominalInput.text = param.value;
            uniformAInput.text = param.a;
            uniformBInput.text = param.b;
        }

        else
        {
            muLabel.visible = true;
            muInput.visible = true;
            sigmaLabel.visible = true;
            sigmaInput.visible = true;

            uncertaintySelector.currentIndex = param.get_uncertainty_index();
            nominalInput.text = param.value;
            muInput.text = param.a;
            sigmaInput.text = param.b;

            if (inputType === 'tnor' || inputType === 'tlog')
            {
                normalCLabel.visible = true;
                normalCInput.visible = true;
                normalDLabel.visible = true;
                normalDInput.visible = true;
                normalCInput.text = param.c;
                normalDInput.refresh();
            }
        }

        valueInput.refreshLims();
        nominalInput.refreshLims();
        muInput.refreshLims();
        sigmaInput.refreshLims();
        uniformAInput.refreshLims();
        uniformBInput.refreshLims();
    }

    Row
    {
        id: paramInputRow

        Component.onCompleted:
        {
            refresh();
        }

        GridLayout {
            id: paramGrid
            rows: 2
            columns: 9
            flow: GridLayout.TopToBottom

            Connections {
                target: param
                function onInputTypeChanged() { refresh(); }
                function onModelChanged() { refresh(); }
                function onUncertaintyChanged() { refresh(); }
                function onUnitChanged() { refresh(); }
            }

            Item { }
            Text {
                id: paramLabel
                text: param?.label_rtf ?? ''
                Layout.preferredWidth: paramLabelWidth
                horizontalAlignment: Text.AlignLeft
                font.pointSize: labelFontSize
                textFormat: Text.RichText

                ToolTip {
                    delay: 200
                    timeout: 3000
                    visible: tipText ? ma.containsMouse : false
                    text: tipText
                }

                // for tooltip hover
                MouseArea {
                    id: ma
                    anchors.fill: parent
                    hoverEnabled: true
                }

            }

            Item {
                id: unitLabel
            }
            DenseComboBox {
                id: unitSelector
                model: param?.unit_choices // ?? null
                currentIndex: param?.get_unit_index() ?? 0
                onActivated: {
                    if (param !== null) param.unit = displayText
                }
                Layout.maximumWidth: medInputW

            }

            InputTopLabel {
                id: inputTypeLabel
                text: ""
            }

            DenseComboBox {
                id: inputTypeSelector
                textRole: "text"
                valueRole: "value"
                model: ListModel {
                    ListElement { value: "det"; text: "Deterministic" }
                    ListElement { value: "tnor"; text: "Normal" }
                    ListElement { value: "tlog"; text: "Lognormal" }
                    ListElement { value: "uni"; text: "Uniform" }
                }
                currentIndex: param.get_input_type_index()
                onActivated: param.input_type = currentValue
                Layout.maximumWidth: 115
            }

            InputTopLabel {
                id: valueLabel
                text: ""
            }
            DoubleTextInput {
                id: valueInput
                field: 'value'
                Layout.maximumWidth: medInputW
            }

            InputTopLabel {
                id: nominalLabel
                text: "Nominal value"
                visible: false
            }
            DoubleTextInput {
                id: nominalInput
                field: 'value'
                Layout.maximumWidth: medInputW
            }

            InputTopLabel {
                id: uncertaintyLabel
                text: "Uncertainty"
            }
            DenseComboBox {
                id: uncertaintySelector
                visible: false
                // Layout.maximumWidth: 100
                Layout.maximumWidth: 90
                textRole: "text"
                valueRole: "value"
                model: ListModel {
                    ListElement { value: "ale"; text: "Aleatory" }
                    ListElement { value: "epi"; text: "Epistemic" }
                }
                currentIndex: param?.get_uncertainty_index() ?? 0
                onActivated: {
                    if (param !== null) param.uncertainty = currentValue;
                }
            }

            // Truncated Normal/lognormal inputs
            InputTopLabel {
                id: muLabel
                text: "\u03BC"
            }
            DoubleTextInput {
                id: muInput
                field: 'a'
                Layout.maximumWidth: shortInputW
            }

            InputTopLabel {
                id: sigmaLabel
                text: "\u03C3"
            }
            DoubleTextInput {
                id: sigmaInput
                field: 'b'
                useLimits: false
                Layout.maximumWidth: shortInputW
            }

            InputTopLabel {
                id: normalCLabel
                text: "Lower bound"
            }
            DoubleTextInput {
                id: normalCInput
                field: 'c'
                Layout.maximumWidth: shortInputW
            }

            InputTopLabel {
                id: normalDLabel
                text: "Upper bound"
            }
            DoubleNullableTextInput {
                id: normalDInput
                field: 'd'
                Layout.maximumWidth: shortInputW
                min: 1
            }

            // Uniform distr inputs
            InputTopLabel {
                id: uniformALabel
                text: "Lower bound"
            }
            DoubleTextInput {
                id: uniformAInput
                field: 'a'
                Layout.maximumWidth: shortInputW
            }

            InputTopLabel {
                id: uniformBLabel
                text: "Upper bound"
            }
            DoubleTextInput {
                id: uniformBInput
                field: 'b'
                Layout.maximumWidth: shortInputW
            }
        }
    }

    Row
    {
        id: alertDisplay
        anchors.top: paramInputRow.bottom
        leftPadding: 125

        AppIcon {
            id: alertIcon
            source: 'circle-exclamation-solid'
            iconColor: Material.color(Material.Red)
            width: 24
            height: 24
        }
        Text {
            id: alertMsg
            text: ""
            anchors.topMargin: 4
            font.italic: true
            anchors.verticalCenter: parent.verticalCenter
            color: color_danger
        }
    }
}
