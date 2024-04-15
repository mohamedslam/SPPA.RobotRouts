
StiMobileDesigner.prototype.InitializeBarCodeForm_ = function () {
    var barCodeForm = this.BaseFormPanel("barCodeForm", this.loc.Components.StiBarCode, 1, this.HelpLinks["barcodeform"]);
    barCodeForm.isDockableToComponent = true;

    barCodeForm.controls = {};
    var jsObject = this;

    //Properties
    var propertiesContainer = barCodeForm.propertiesContainer = document.createElement("div");
    propertiesContainer.style.padding = "6px 0 6px 0";
    barCodeForm.container.appendChild(propertiesContainer);

    var properties = [
        ["codeType", this.loc.PropertyMain.Type, this.BarCodeTypeControl("barCodeFormCodeType", this.options.propertyControlWidth + 4), "6px 12px 6px 12px"],
        ["code", this.loc.PropertyMain.Code, this.ExpressionTextArea("barCodeFormCode", this.options.propertyControlWidth, 40, true), "6px 12px 6px 12px"],
        ["angle", this.loc.PropertyMain.Angle, this.PropertyDropDownList("barCodeFormAngle", this.options.propertyControlWidth, this.GetBarCodeAngleItems(), true, false), "6px 12px 6px 12px"],
        ["autoScale", " ", this.CheckBox("barCodeFormAutoScale", this.loc.PropertyMain.AutoScale), "7px 12px 7px 12px"],
        ["showQuietZones", " ", this.CheckBox("barCodeShowQuietZones", this.loc.PropertyMain.ShowQuietZones), "7px 12px 7px 12px"],
        ["separator1"],
        ["foreColor", this.loc.PropertyMain.ForeColor, this.PropertyColorControl("barCodeFormForeColor", null, this.options.propertyControlWidth), "6px 12px 6px 12px"],
        ["backColor", this.loc.PropertyMain.BackColor, this.PropertyColorControl("barCodeFormBackColor", null, this.options.propertyControlWidth), "6px 12px 6px 12px"],
        ["separator2"],
        ["font", this.loc.PropertyMain.Font, this.PropertyFontControl("barCodeFormFont", "font", true), "6px 12px 6px 12px"],
        ["showLabelText", " ", this.CheckBox("barCodeShowLabelText", this.loc.PropertyMain.ShowLabelText), "7px 12px 7px 12px"]
    ];

    var proprtiesTable = this.CreateHTMLTable();
    propertiesContainer.appendChild(proprtiesTable);

    for (var i = 0; i < properties.length; i++) {
        var propertyName = properties[i][0];
        barCodeForm.controls[propertyName + "Row"] = proprtiesTable.addRow();

        if (properties[i][0].indexOf("separator") == 0) {
            var sep = this.FormSeparator();
            sep.style.margin = "6px 0 6px 0";
            barCodeForm.controls[control.propertyName] = sep;
            proprtiesTable.addCellInLastRow(sep).setAttribute("colspan", 2);
            continue;
        }

        var textCell = proprtiesTable.addTextCellInLastRow(properties[i][1]);
        textCell.className = "stiDesignerCaptionControlsBigIntervals";
        textCell.style.whiteSpace = "normal";
        textCell.style.minWidth = "130px";

        if (properties[i][0] == "font") {
            textCell.style.verticalAlign = "top";
            textCell.style.paddingTop = "10px";
        }

        var control = properties[i][2];
        control.propertyName = propertyName;
        control.style.margin = properties[i][3];
        proprtiesTable.addCellInLastRow(control);
        barCodeForm.controls[propertyName] = control;

        jsObject.AddMainMethodsToPropertyControl(control);

        control.action = function () {
            barCodeForm.applyBarCodeProperty({ name: this.propertyName, value: this.getValue() });
        }
    }

    propertiesContainer.update = function () {
        var codeType = barCodeForm.barCode.codeType;
        var commonProps = barCodeForm.barCode.properties.common;

        barCodeForm.controls.codeType.setKey(codeType);

        for (var i = 0; i < properties.length; i++) {
            if (properties[i][0].indexOf("separator") == 0) {
                continue;
            }
            var control = properties[i][2];
            if (commonProps[control.propertyName] != null) {
                control.setValue(commonProps[control.propertyName]);
            }
            if (control.propertyName == "code") {
                control.setEnabled(codeType != "StiSSCC18BarCodeType");

                if (codeType == "StiSSCC18BarCodeType") {
                    // eslint-disable-next-line no-control-regex
                    control.setValue(StiBase64.decode(commonProps[control.propertyName].replace("Base64Code;", "")).replace(/\u001f/g, ''));
                }
            }
            var hideFont = codeType == "StiQRCodeBarCodeType" || codeType == "StiDataMatrixBarCodeType" || codeType == "StiMaxicodeBarCodeType" || codeType == "StiPdf417BarCodeType" || codeType == "StiAztecBarCodeType";
            barCodeForm.controls.fontRow.style.display = barCodeForm.controls.showLabelTextRow.style.display = barCodeForm.controls.separator2Row.style.display = hideFont ? "none" : "";
        }
    }

    barCodeForm.onshow = function () {
        var selectedObject = jsObject.options.selectedObject;

        if (!selectedObject || selectedObject.typeComponent != "StiBarCode") {
            this.changeVisibleState(false);
        }

        this.currentComponent = selectedObject;
        this.oldSvgContent = selectedObject.properties.svgContent;

        jsObject.options.propertiesPanel.setEditBarCodeMode(true);
        this.updateControls();
    }

    barCodeForm.onhide = function () {
        jsObject.options.propertiesPanel.setEditBarCodeMode(false);
    }

    barCodeForm.action = function () {
        this.changeVisibleState(false);
        jsObject.options.updateLastStyleProperties = true;
    }

    barCodeForm.cancelAction = function () {
        jsObject.SendCommandCanceledEditComponent(barCodeForm.barCode.name);
        if (this.oldSvgContent) {
            this.currentComponent.properties.svgContent = this.oldSvgContent;
            this.currentComponent.repaint();
        }
    }

    barCodeForm.applyBarCodeProperty = function (property) {
        jsObject.SendCommandToDesignerServer("ApplyBarCodeProperty", { componentName: barCodeForm.barCode.name, property: property }, function (answer) {
            barCodeForm.barCode = answer.barCode;
            barCodeForm.updateControls();
            barCodeForm.updateSvgContent(answer.barCode.svgContent);
        });
    }

    barCodeForm.updateSvgContent = function (svgContent) {
        this.currentComponent.properties.svgContent = svgContent;
        this.currentComponent.repaint();
    }

    barCodeForm.updateControls = function () {
        propertiesContainer.update();
        var barCodePropertiesPanel = jsObject.options.propertiesPanel.editBarCodePropertiesPanel;
        if (barCodePropertiesPanel) {
            barCodePropertiesPanel.updateProperties(this.barCode.properties.additional);
        }
    }

    return barCodeForm;
}