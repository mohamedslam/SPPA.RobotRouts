
StiMobileDesigner.prototype.InitializeTextFormatForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("textFormatForm", this.loc.FormFormatEditor.title, 2, this.HelpLinks["textFormat"]);
    form.controls = {};

    var mainTable = this.CreateHTMLTable();
    form.container.appendChild(mainTable);
    mainTable.addCell(this.FormBlockHeader(this.loc.FormFormatEditor.Formats)).style.padding = "12px";
    mainTable.addCell(this.FormBlockHeader(this.loc.FormFormatEditor.Sample)).style.padding = "12px 12px 12px 0";

    //Format Items
    var formatsContainer = form.controls.formatsContainer = this.EasyContainer(150, 350);
    formatsContainer.className = "stiSimpleContainerWithBorder";
    formatsContainer.style.margin = "0 12px 12px 12px";
    formatsContainer.style.overflow = "auto";
    mainTable.addCellInNextRow(formatsContainer);

    //Work Right Panel
    var workContainer = document.createElement("div");
    workContainer.style.margin = "0 12px 12px 0";
    workContainer.style.width = "400px";
    workContainer.style.height = "350px";
    mainTable.addCellInLastRow(workContainer);

    //Sample
    var sampleTable = this.CreateHTMLTable();
    var sampleContainer = sampleTable.addCell();
    workContainer.appendChild(sampleTable);
    sampleContainer.style.paddingLeft = "15px";
    sampleContainer.style.height = "30px";
    sampleContainer.style.fontSize = "12px";
    sampleContainer.style.fontFamily = "Arial";
    sampleContainer.innerHTML = "Sample Text";
    form.sampleContainer = sampleContainer;

    //Properties
    workContainer.appendChild(this.FormBlockHeader(this.loc.FormFormatEditor.Properties));

    var propertiesContainer = document.createElement("div");
    workContainer.appendChild(propertiesContainer);
    propertiesContainer.style.overflowY = "auto";
    propertiesContainer.style.overflowX = "hidden";
    propertiesContainer.style.height = "294px";
    var propertiesTable = this.CreateHTMLTable();
    propertiesTable.style.width = "100%";
    propertiesContainer.appendChild(propertiesTable);

    var controlProps = [
        ["useGroupSeparator", null, this.CheckBox(null, this.loc.FormFormatEditor.UseGroupSeparator), "10px 10px 6px 10px", false],
        ["useLocalSetting", null, this.CheckBox(null, this.loc.FormFormatEditor.UseLocalSetting), "6px 10px 6px 10px", false],
        ["useAbbreviation", null, this.CheckBox(null, this.loc.FormFormatEditor.UseAbbreviation), "6px 10px 6px 10px", false],
        ["negativeInRed", null, this.CheckBox(null, this.loc.FormFormatEditor.NegativeInRed), "6px 10px 6px 10px", false],
        ["decimalDigits", this.loc.FormFormatEditor.DecimalDigits.replace(":", ""), this.TextBoxEnumerator(null, 140, null, false, 20, 0), "4px 10px 4px 10px", "DecimalDigits"],
        ["decimalSeparator", this.loc.FormFormatEditor.DecimalSeparator.replace(":", ""), this.DropDownList(null, 140, null, this.GetDecimalSeparatorItems(), false, null, null, true), "4px 10px 4px 10px", "DecimalSeparator"],
        ["groupSeparator", this.loc.FormFormatEditor.GroupSeparator.replace(":", ""), this.DropDownList(null, 140, null, this.GetGroupSeparatorItems(), false, null, null, true), "4px 10px 4px 10px", "GroupSeparator"],
        ["groupSize", this.loc.FormFormatEditor.GroupSize.replace(":", ""), this.TextBoxEnumerator(null, 140, null, false, 9, 0), "4px 10px 4px 10px", "GroupSize"],
        ["currencyPositivePattern", this.loc.FormFormatEditor.PositivePattern.replace(":", ""), this.DropDownList(null, 140, null, this.GetCurrencyPositivePatternItems(), true, null, null, true), "4px 10px 4px 10px", "PositivePattern"],
        ["currencyNegativePattern", this.loc.FormFormatEditor.NegativePattern.replace(":", ""), this.DropDownList(null, 140, null, this.GetCurrencyNegativePatternItems(), true, null, null, true), "4px 10px 4px 10px", "NegativePattern"],
        ["percentagePositivePattern", this.loc.FormFormatEditor.PositivePattern.replace(":", ""), this.DropDownList(null, 140, null, this.GetPercentagePositivePatternItems(), true, null, null, true), "4px 10px 4px 10px", "PositivePattern"],
        ["percentageNegativePattern", this.loc.FormFormatEditor.NegativePattern.replace(":", ""), this.DropDownList(null, 140, null, this.GetPercentageNegativePatternItems(), true, null, null, true), "4px 10px 4px 10px", "NegativePattern"],
        ["numberNegativePattern", this.loc.FormFormatEditor.NegativePattern.replace(":", ""), this.DropDownList(null, 140, null, this.GetNumberNegativePatternItems(), true, null, null, true), "4px 10px 4px 10px", "NegativePattern"],
        ["percentageSymbol", this.loc.FormFormatEditor.PercentageSymbol.replace(":", ""), this.DropDownList(null, 140, null, this.GetPercentageSymbolItems(), false, null, null, true), "4px 10px 4px 10px", "PercentageSymbol"],
        ["currencySymbol", this.loc.FormFormatEditor.CurrencySymbol.replace(":", ""), this.DropDownList(null, 140, null, this.GetCurrencySymbolItems(), false, null, null, true), "4px 10px 4px 10px", "CurrencySymbol"],
        ["groupFalse", null, this.FormBlockHeader(this.loc.FormFormatEditor.nameFalse.replace(":", "")), "20px 0 4px 6px", false],
        ["groupFalseSeparator", null, this.FormSeparator(), "0px 4px 6px 4px", false],
        ["falseValue", this.loc.FormFormatEditor.BooleanValue.replace(":", ""), this.DropDownList(null, 140, null, this.GetBooleanFormatItems(this.options.jsMode), false, null, null, true), "4px 10px 4px 10px", false],
        ["falseDisplay", this.loc.FormFormatEditor.BooleanDisplay.replace(":", ""), this.DropDownList(null, 140, null, this.GetBooleanFormatItems(), false, null, null, true), "4px 10px 4px 10px", false],
        ["groupTrue", null, this.FormBlockHeader(this.loc.FormFormatEditor.nameTrue.replace(":", "")), "20px 0 4px 6px", false],
        ["groupTrueSeparator", null, this.FormSeparator(), "0px 4px 6px 4px", false],
        ["trueValue", this.loc.FormFormatEditor.BooleanValue.replace(":", ""), this.DropDownList(null, 140, null, this.GetBooleanFormatItems(this.options.jsMode), false, null, null, true), "4px 10px 4px 10px", false],
        ["trueDisplay", this.loc.FormFormatEditor.BooleanDisplay.replace(":", ""), this.DropDownList(null, 140, null, this.GetBooleanFormatItems(), false, null, null, true), "4px 10px 4px 10px", false],
        ["formatMask", this.loc.FormFormatEditor.FormatMask.replace(":", ""), this.TextBox(null, 140), "8px 10px 8px 10px", false],
        ["dateFormat", null, this.FormatsContainer(294, "date"), "0px", false],
        ["timeFormat", null, this.FormatsContainer(294, "time"), "0px", false],
        ["customFormat", null, this.FormatsContainer(this.options.isTouchDevice ? 249 : 254, "custom"), "0px", false]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        var name = controlProps[i][0];
        var caption = controlProps[i][1];
        var control = controlProps[i][2];
        control.propertyName = name;
        if (controlProps[i][3]) control.style.margin = controlProps[i][3];
        var checkBoxKey = controlProps[i][4];
        form.controls[name] = control;
        form.controls[name + "Row"] = propertiesTable.addRow();
        if (control.menu) control.menu.innerContent.style.maxHeight = "220px";

        control.setPropertyValue = function (value) {
            if (this["getItemByName"] != null) {
                var selItem = this.getItemByName(value);
                if (selItem)
                    selItem.select();
                else {
                    if (this.selectedItem) this.selectedItem.setSelected(false);
                    this.selectedItem = null;
                }
            }
            else if (this["setKey"] != null) this.setKey(value);
            else if (this["setChecked"] != null) this.setChecked(value);
            else if (this["setValue"] != null) this.setValue(value);
            else if (this["value"] != null) this.value = value;
        }

        control.action = function () {
            if (!formatsContainer.selectedItem) return;
            var selItemObject = formatsContainer.selectedItem.itemObject;
            if (this.propertyName == "useLocalSetting") propertiesContainer.updateEnabledStates();
            if (this["getItemByName"] != null) {
                if (this.selectedItem) {
                    selItemObject[this.propertyName] = this.selectedItem.itemObject.key;
                    if (this.propertyName == "customFormat") {
                        form.controls.formatMask.value = this.selectedItem.itemObject.key;
                        selItemObject.formatMask = this.selectedItem.itemObject.key;
                    }
                }
            }
            else {
                selItemObject[this.propertyName] =
                    this["setKey"] != null
                        ? this.key
                        : this["setChecked"] != null
                            ? this.isChecked
                            : this["setValue"] != null
                                ? jsObject.StrToInt(this.textBox.value) : this.value;
            }

            if (this.propertyName == "formatMask") {
                selItemObject.customFormat = this.value;
                var customItem = form.controls.customFormat.getItemByName(this.value);
                if (!customItem && form.controls.customFormat.selectedItem) {
                    form.controls.customFormat.selectedItem.setSelected(false);
                    form.controls.customFormat.selectedItem = null;
                }
            }

            propertiesContainer.updateSample();
        }

        if (caption) {
            var textCell = propertiesTable.addTextCellInLastRow(caption);
            textCell.className = "stiDesignerCaptionControls";
            textCell.style.padding = "0 0 0 10px";
            textCell.style.whiteSpace = "normal";
            textCell.style.width = "100%";
        }

        var controlCell = propertiesTable.addCellInLastRow();

        if (checkBoxKey) {
            var table = this.CreateHTMLTable();
            controlCell.appendChild(table);
            var controlCheckBox = this.CheckBox();
            controlCheckBox.captionCell.style.padding = "0";
            controlCheckBox.key = checkBoxKey;
            controlCheckBox.action = function () {
                var itemObject = formatsContainer.selectedItem.itemObject;
                if (itemObject && itemObject.state) {
                    if (this.isChecked) {
                        if (itemObject.state == "None") itemObject.state = "";
                        if (itemObject.state != "") itemObject.state += ",";
                        itemObject.state += this.key;
                    }
                    else {
                        itemObject.state = itemObject.state.replace(this.key + ",", "").replace(", " + this.key, "").replace(this.key, "");
                        if (itemObject.state == "") itemObject.state = "None";
                    }
                    propertiesContainer.updateControls();
                }
            };
            control.checkBox = controlCheckBox;
            form.controls[name + "Checkbox"] = control;
            table.addCell(controlCheckBox);
            table.addCell(control);
            controlCell.appendChild(table);
        }
        else {
            controlCell.appendChild(control);
        }

        if (!caption)
            controlCell.setAttribute("colspan", "2");
        else
            controlCell.style.textAlign = "right";
    }

    form.controls.customFormat.className = "stiDesignerTextFormatFormCustomContainer";

    propertiesContainer.updateControls = function () {
        var itemObject = formatsContainer.selectedItem.itemObject;

        for (var i = 0; i < controlProps.length; i++) {
            var propertyName = controlProps[i][0];
            var controlRow = form.controls[propertyName + "Row"];
            if (controlRow) {
                controlRow.style.display = typeof (itemObject[propertyName]) != "undefined" ? "" : "none";
                var control = form.controls[propertyName];
                if (control) {
                    var propertyValue = itemObject[propertyName];
                    if (propertyName == "formatMask" && typeof (itemObject[propertyName]) == "undefined")
                        propertyValue = itemObject["customFormat"];
                    control.setPropertyValue(propertyValue);

                    if (itemObject.state && control.checkBox) {
                        control.checkBox.setChecked(itemObject.state.indexOf(control.checkBox.key) >= 0);
                    }
                }
            }
        }

        form.controls.groupTrue.caption.style.padding = form.controls.groupFalse.caption.style.padding = "0px 0px 0px 6px";
        form.controls.groupTrue.style.background = form.controls.groupFalse.style.background = "transparent";
        form.controls.groupTrueRow.style.display = form.controls.groupTrueSeparatorRow.style.display = typeof (itemObject["trueValue"]) != "undefined" ? "" : "none";
        form.controls.groupFalseRow.style.display = form.controls.groupFalseSeparatorRow.style.display = typeof (itemObject["falseValue"]) != "undefined" ? "" : "none";
        form.controls.formatMaskRow.style.display = typeof (itemObject["customFormat"]) != "undefined" ? "" : "none";
        propertiesContainer.updateEnabledStates();
        propertiesContainer.updateSample();
    }

    propertiesContainer.updateEnabledStates = function () {
        var controlNames = ["decimalDigits", "decimalSeparator", "groupSeparator", "groupSize", "currencyPositivePattern", "currencyNegativePattern",
            "percentagePositivePattern", "percentageNegativePattern", "numberNegativePattern", "percentageSymbol", "currencySymbol"];
        for (var i = 0; i < controlNames.length; i++) {
            var control = form.controls[controlNames[i]];
            control.setEnabled(!form.controls.useLocalSetting.isChecked);
            if (control.checkBox) {
                control.checkBox.style.display = form.controls.useLocalSetting.isChecked ? "" : "none";
                if (form.controls.useLocalSetting.isChecked) control.setEnabled(control.checkBox.isChecked);
            }
        }
    }

    propertiesContainer.updateSample = function () {
        if (formatsContainer.selectedItem)
            jsObject.SendCommandUpdateSampleTextFormat(formatsContainer.selectedItem.itemObject);
    }

    form.show = function (textFormatValue, textFormatPropertyName, propertyOwner, propertyControl) {
        this.changeVisibleState(true);
        formatsContainer.clear();

        this.textFormatPropertyName = textFormatPropertyName;
        this.propertyOwner = propertyOwner;
        this.propertyControl = propertyControl;

        var formatTypes = ["StiGeneralFormatService", "StiNumberFormatService", "StiCurrencyFormatService", "StiDateFormatService", "StiTimeFormatService",
            "StiPercentageFormatService", "StiBooleanFormatService", "StiCustomFormatService"];

        jsObject.SendCommandToDesignerServer("UpdateTextFormatItemsByReportCulture", {}, function (answer) {
            if (answer.textFormats) jsObject.options.textFormats = answer.textFormats;
            if (answer.dateFormats) jsObject.options.dateFormats = answer.dateFormats;
            if (answer.timeFormats) jsObject.options.timeFormats = answer.timeFormats;
            if (answer.customFormats) jsObject.options.customFormats = answer.customFormats;
            form.controls.dateFormat.fillItems();
            form.controls.timeFormat.fillItems();
            form.controls.customFormat.fillItems();

            for (var i = 0; i < formatTypes.length; i++) {
                var item = formatsContainer.addItem(formatTypes[i], jsObject.CopyObject(jsObject.options.textFormats[formatTypes[i]]), jsObject.GetTextFormatLocalizedName(formatTypes[i]), null);
                item.style.margin = "5px";
                if (item.caption) item.caption.style.padding = "0px 10px";
            }

            var currentObject = jsObject.options.selectedObject || jsObject.GetCommonObject(jsObject.options.selectedObjects);
            var textFormatPropertyValue = currentObject ? currentObject.properties[textFormatPropertyName ? textFormatPropertyName : "textFormat"] : null;

            if (textFormatValue != null || (textFormatPropertyValue && textFormatPropertyValue.type != "StiEmptyValue")) {
                var currentItem = formatsContainer.getItemByName(textFormatValue ? textFormatValue.type : textFormatPropertyValue.type);
                if (currentItem) {
                    currentItem.itemObject = jsObject.CopyObject(textFormatValue || textFormatPropertyValue);
                    currentItem.action();
                }
            }
            else
                formatsContainer.getItemByName("StiGeneralFormatService").action();
        });
    }

    formatsContainer.onSelected = function () {
        propertiesContainer.updateControls();
    }

    form.action = function () {
        if (formatsContainer.selectedItem) {
            var resultTextFormat = formatsContainer.selectedItem.itemObject;
            var selectedObjects = this.propertyOwner ? [this.propertyOwner] : (jsObject.options.selectedObjects || [jsObject.options.selectedObject]);
            var propertyName = this.textFormatPropertyName || "textFormat";

            if (selectedObjects) {
                //dbs table element cells
                if (selectedObjects.length == 1 && selectedObjects[0].typeComponent == "StiTableElement") {
                    var editTableElementForm = jsObject.options.forms.editTableElementForm;
                    if (editTableElementForm && editTableElementForm.visible && editTableElementForm.controls.dataContainer.selectedItem) {
                        editTableElementForm.setPropertyValue("TextFormat", resultTextFormat);
                    }
                }
                else if (selectedObjects.length == 1 && selectedObjects[0].typeComponent == "StiPivotTableElement") {
                    var editPivotTableElementForm = jsObject.options.forms.editPivotTableElementForm;
                    if (editPivotTableElementForm && editPivotTableElementForm.visible && editPivotTableElementForm.getSelectedItem()) {
                        editPivotTableElementForm.setPropertyValue("TextFormat", resultTextFormat);
                    }
                }
                //style item
                else if (selectedObjects.length == 1 && selectedObjects[0].type == "StiStyle") {
                    selectedObjects[0].properties[propertyName] = resultTextFormat;
                    if (this.propertyControl) {
                        var notLocalizeValues = this.propertyControl.ownerIsProperty && jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.localizePropertyGrid;
                        this.propertyControl.value = jsObject.GetTextFormatLocalizedName(resultTextFormat.type, notLocalizeValues);
                    }
                }
                //other
                else {
                    for (var i = 0; i < selectedObjects.length; i++) {
                        selectedObjects[i].properties[propertyName] = resultTextFormat;
                    }
                    jsObject.UpdatePropertiesControls();
                    jsObject.SendCommandSendProperties(selectedObjects, [propertyName]);
                }
            }
        }

        this.changeVisibleState(false);
    }

    return form;
}


StiMobileDesigner.prototype.FormatsContainer = function (height, type) {
    var jsObject = this;
    var container = this.EasyContainer(null, height);

    container.action = function () { }
    container.onSelected = function () { this.action(); }

    container.fillItems = function () {
        container.clear();
        var formats = jsObject.options[type + "Formats"];
        if (formats) {
            for (var i = 0; i < formats.length; i++) {
                var item = container.addItem(formats[i].key, { key: formats[i].key }, formats[i].value, null);
                item.style.margin = "5px 10px 5px 10px";
                if (i == 0) item.action();
            }
        }
    }

    return container;
}