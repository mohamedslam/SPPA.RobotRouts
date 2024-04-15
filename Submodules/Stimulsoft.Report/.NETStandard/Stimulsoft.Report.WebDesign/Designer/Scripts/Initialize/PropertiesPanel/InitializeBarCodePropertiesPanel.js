
StiMobileDesigner.prototype.BarCodePropertiesPanel = function () {
    var panel = document.createElement("div");
    panel.jsObject = this;
    panel.style.display = "none";

    var groupNames = [
        ["BarCode", this.loc.PropertyCategory.BarCodeCategory],
        ["Appearance", this.loc.PropertyCategory.AppearanceCategory]
    ]

    this.AddGroupsToPropertiesPanel(groupNames, panel);
    panel.groups.BarCode.changeOpenedState(true);
    panel.groups.Appearance.changeOpenedState(true);

    var properties = [
        ["addClearZone", this.loc.PropertyMain.AddClearZone, this.CheckBox(null), panel.groups["BarCode"], "Checkbox"],
        ["aspectRatio", this.loc.PropertyMain.AspectRatio, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["BarCode"], "Textbox"],
        ["autoDataColumns", this.loc.PropertyMain.AutoDataColumns, this.CheckBox(null), panel.groups["BarCode"], "Checkbox"],
        ["autoDataRows", this.loc.PropertyMain.AutoDataRows, this.CheckBox(null), panel.groups["BarCode"], "Checkbox"],
        ["checksum", this.loc.PropertyMain.Checksum, this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetBarCodeChecksumItems(), true, false), panel.groups["BarCode"], "DropdownList"],
        ["checkSum", this.loc.PropertyMain.Checksum, this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetBarCodeCheckSumItems(), true, false), panel.groups["BarCode"], "DropdownList"],
        ["checkSum1", this.loc.PropertyMain.CheckSum1, this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetBarCodePlesseyCheckSumItems(), true, false), panel.groups["BarCode"], "DropdownList"],
        ["checkSum2", this.loc.PropertyMain.CheckSum2, this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetBarCodePlesseyCheckSumItems(), true, false), panel.groups["BarCode"], "DropdownList"],
        ["companyPrefix", this.loc.PropertyMain.CompanyPrefix, this.PropertyTextBox(null, this.options.propertyControlWidth), panel.groups["BarCode"], "Textbox"],
        ["dataColumns", this.loc.PropertyMain.DataColumns, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["BarCode"], "Textbox"],
        ["dataRows", this.loc.PropertyMain.DataRows, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["BarCode"], "Textbox"],
        ["encodingMode", this.loc.PropertyMain.EncodingMode, this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetBarCodeEncodingModeItems(), true, false), panel.groups["BarCode"], "DropdownList"],
        ["encodingType", this.loc.PropertyMain.EncodingType, this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetBarCodeEncodingTypeItems(), true, false), panel.groups["BarCode"], "DropdownList"],
        ["errorsCorrectionLevel", this.loc.PropertyMain.ErrorsCorrectionLevel, this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetBarCodeErrorsCorrectionLevelItems(), true, false), panel.groups["BarCode"], "DropdownList"],
        ["extensionDigit", this.loc.PropertyMain.ExtensionDigit, this.PropertyTextBox(null, this.options.propertyControlWidth), panel.groups["BarCode"], "Textbox"],
        ["height", this.loc.PropertyMain.Height, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["BarCode"], "Textbox"],
        ["image", this.loc.PropertyMain.Image, this.PropertyImageControl("barcodeImageProperty", this.options.propertyControlWidth), panel.groups["BarCode"], "ImageControl"],
        ["imageMultipleFactor", this.loc.PropertyMain.ImageMultipleFactor, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["BarCode"], "Textbox"],
        ["matrixSize", this.loc.PropertyMain.MatrixSize, this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetBarCodeMatrixSizeDataMatrixItems(), true, false), panel.groups["BarCode"], "DropdownList"],
        ["module", this.loc.PropertyMain.Module, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["BarCode"], "Textbox"],
        ["mode", this.loc.PropertyMain.Mode, this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetBarCodeModeItems(), true, false), panel.groups["BarCode"], "DropdownList"],
        ["processTilde", this.loc.PropertyMain.ProcessTilde, this.CheckBox(null), panel.groups["BarCode"], "Checkbox"],
        ["printVerticalBars", this.loc.PropertyMain.PrintVerticalBars, this.CheckBox(null), panel.groups["BarCode"], "Checkbox"],
        ["ratioY", this.loc.PropertyMain.RatioY, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["BarCode"], "Textbox"],
        ["ratio", this.loc.PropertyMain.Ratio, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["BarCode"], "Textbox"],
        ["serialNumber", this.loc.PropertyMain.SerialNumber, this.PropertyTextBox(null, this.options.propertyControlWidth), panel.groups["BarCode"], "Textbox"],
        ["showQuietZoneIndicator", this.loc.PropertyMain.ShowQuietZoneIndicator, this.CheckBox(null), panel.groups["BarCode"], "Checkbox"],
        ["supplementType", this.loc.PropertyMain.SupplementType, this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetBarCodeSupplementTypeItems(), true, false), panel.groups["BarCode"], "DropdownList"],
        ["space", this.loc.PropertyMain.Space, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["BarCode"], "Textbox"],
        ["structuredAppendPosition", this.loc.PropertyMain.StructuredAppendPosition, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["BarCode"], "Textbox"],
        ["structuredAppendTotal", this.loc.PropertyMain.StructuredAppendTotal, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["BarCode"], "Textbox"],
        ["supplementCode", this.loc.PropertyMain.SupplementCode, this.PropertyTextBox(null, this.options.propertyControlWidth), panel.groups["BarCode"], "Textbox"],
        ["trimExcessData", this.loc.PropertyMain.TrimExcessData, this.CheckBox(null), panel.groups["BarCode"], "Checkbox"],
        ["useRectangularSymbols", this.loc.PropertyMain.UseRectangularSymbols, this.CheckBox(null), panel.groups["BarCode"], "Checkbox"],
        ["bodyBrush", "Body Brush", this.PropertyBrushControl(null, null, this.options.propertyControlWidth), panel.groups["Appearance"], "Brush"],
        ["bodyShape", "Body Shape", this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetBarCodeBodyShapeItems(), true, false), panel.groups["Appearance"], "DropdownList"],
        ["eyeBallBrush", "Eye Ball Brush", this.PropertyBrushControl(null, null, this.options.propertyControlWidth), panel.groups["Appearance"], "Brush"],
        ["eyeBallShape", "Eye Ball Shape", this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetBarCodeEyeBallShapeItems(), true, false), panel.groups["Appearance"], "DropdownList"],
        ["eyeFrameBrush", "Eye Frame Brush", this.PropertyBrushControl(null, null, this.options.propertyControlWidth), panel.groups["Appearance"], "Brush"],
        ["eyeFrameShape", "Eye Frame Shape", this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetBarCodeEyeFrameShapeItems(), true, false), panel.groups["Appearance"], "DropdownList"]
    ]

    for (var i = 0; i < properties.length; i++) {
        var property = this.AddBarCodePropertyToPropertiesGroup(properties[i][0], properties[i][1], properties[i][2], properties[i][3], properties[i][4]);
    }

    panel.updateProperties = function (propertiesValues) {
        if (propertiesValues == null) propertiesValues = {};
        this.propertiesValues = propertiesValues;
        var propertiesGroups = this.groups;
        var barCodeForm = this.jsObject.options.forms.barCodeForm;

        for (var groupName in propertiesGroups) {
            var propertyGroup = propertiesGroups[groupName];
            var showGroup = false;
            for (var propertyName in propertyGroup.properties) {
                var property = propertyGroup.properties[propertyName];
                if (property.isUserProperty) {
                    var showProperty = propertiesValues[property.name] != null;

                    //Exceptions
                    if (property.name == "height" && (
                        barCodeForm.barCode.codeType == "StiQRCodeBarCodeType" ||
                        barCodeForm.barCode.codeType == "StiMaxicodeBarCodeType" ||
                        barCodeForm.barCode.codeType == "StiPdf417BarCodeType" ||
                        barCodeForm.barCode.codeType == "StiDataMatrixBarCodeType" ||
                        barCodeForm.barCode.codeType == "StiAustraliaPost4StateBarCodeType" ||
                        barCodeForm.barCode.codeType == "StiFIMBarCodeType"))
                        showProperty = false;

                    if (property.name == "module" && (
                        barCodeForm.barCode.codeType == "StiMaxicodeBarCodeType" ||
                        barCodeForm.barCode.codeType == "StiFIMBarCodeType" ||
                        barCodeForm.barCode.codeType == "StiPharmacodeBarCodeType"))
                        showProperty = false;

                    property.style.display = showProperty ? "" : "none";
                    if (showProperty) {
                        showGroup = true;
                        if (property.name == "matrixSize") {
                            property.control.addItems(barCodeForm && barCodeForm.barCode.codeType == "StiQRCodeBarCodeType"
                                ? this.jsObject.GetBarCodeMatrixSizeQRCodeItems() : this.jsObject.GetBarCodeMatrixSizeDataMatrixItems());
                        }
                        this.jsObject.SetPropertyValue(property, propertiesValues[property.name]);
                    }
                }
            }
            propertyGroup.style.display = showGroup ? "" : "none";
        }
    }

    return panel;
}

StiMobileDesigner.prototype.AddBarCodePropertyToPropertiesGroup = function (propertyName, propertyCaption, propertyControl, propertyGroup, controlType) {
    var property = this.AddPropertyToPropertiesGroup(propertyName, propertyCaption, propertyControl, propertyGroup, controlType);

    propertyControl.action = function () {
        var barCodeForm = property.jsObject.options.forms.barCodeForm;
        var editBarCodePropertiesPanel = property.jsObject.options.propertiesPanel.editBarCodePropertiesPanel;

        if (barCodeForm && barCodeForm.visible && editBarCodePropertiesPanel && editBarCodePropertiesPanel.propertiesValues) {
            barCodeForm.applyBarCodeProperty({ name: "BarCodeType." + property.jsObject.UpperFirstChar(this.property.name), value: this.property.getValue() });
        }
    }

    return property;
}