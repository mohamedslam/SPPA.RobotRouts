
StiMobileDesigner.prototype.InitializeEditVariableForm_ = function () {

    //Edit Variable Form
    var editVariableForm = this.BaseForm("editVariableForm", this.loc.FormDictionaryDesigner.VariableNew, 3, this.HelpLinks["variableEdit"]);
    editVariableForm.variable = null;
    editVariableForm.mode = "Edit";
    editVariableForm.controls = {};
    var jsObject = this;

    var saveCopyButton = this.FormButton(null, null, this.loc.Buttons.SaveCopy, null);
    saveCopyButton.style.display = "inline-block";
    saveCopyButton.style.margin = "8px";

    saveCopyButton.action = function () {
        if (!editVariableForm.controls.name.checkExists(jsObject.GetVariablesFromDictionary(jsObject.options.report.dictionary), "name")) {
            if (editVariableForm.controls.name.value == editVariableForm.controls.alias.value) {
                editVariableForm.controls.alias.value += "Copy";
            }
            editVariableForm.controls.name.value += "Copy";

            var resultName = editVariableForm.controls.name.value;
            var i = 2;
            while (!editVariableForm.controls.name.checkExists(jsObject.GetVariablesFromDictionary(jsObject.options.report.dictionary), "name")) {
                editVariableForm.controls.name.value = editVariableForm.controls.alias.value = resultName + i;
                i++;
            }
        }

        editVariableForm.mode = "New";
        editVariableForm.action();
    }

    var footerTable = this.CreateHTMLTable();
    footerTable.style.width = "100%";
    var buttonsPanel = editVariableForm.buttonsPanel;
    editVariableForm.removeChild(buttonsPanel);
    editVariableForm.appendChild(footerTable);
    footerTable.addCell(saveCopyButton).style.width = "1px";
    footerTable.addCell();
    footerTable.addCell(editVariableForm.buttonOk).style.width = "1px";
    footerTable.addCell(editVariableForm.buttonCancel).style.width = "1px";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "5px 0 5px 0";
    editVariableForm.container.appendChild(innerTable);

    //Name, Alias, Description
    var textBoxes = [
        ["name", this.loc.PropertyMain.Name, 200],
        ["alias", this.loc.PropertyMain.Alias, 200],
        ["description", this.loc.PropertyMain.Description, 311]
    ]
    for (var i = 0; i < textBoxes.length; i++) {
        var text = innerTable.addCellInNextRow();
        text.className = "stiDesignerCaptionControlsBigIntervals";
        text.innerHTML = textBoxes[i][1];
        text.style.minWidth = "140px";
        editVariableForm.controls[textBoxes[i][0]] = this.TextBox("editVariableForm" + textBoxes[i][0], textBoxes[i][2]);
        innerTable.addCellInLastRow(editVariableForm.controls[textBoxes[i][0]]).className = "stiDesignerControlCellsBigIntervals2";
    }

    //Type
    var textType = innerTable.addCellInNextRow();
    textType.className = "stiDesignerCaptionControlsBigIntervals";
    textType.innerHTML = this.loc.PropertyMain.Type;
    var typeControlsTable = this.CreateHTMLTable();
    innerTable.addCellInLastRow(typeControlsTable).className = "stiDesignerControlCellsBigIntervals2";
    editVariableForm.controls.type = this.DropDownList("editVariableFormTypeControl", 170, null, this.GetVariableTypesItems(), true, true, null, null, { width: 16, height: 16 });

    //Override
    editVariableForm.controls.type.image.style.width = "16px";
    editVariableForm.controls.type.image.style.margin = "0 8px 0 8px";
    typeControlsTable.addCell(editVariableForm.controls.type).style.padding = "0 10px 0 0";
    editVariableForm.controls.basicType = this.DropDownList("editVariableFormBasicTypeControl", 130, null, this.GetVariableBasicTypesItems(), true);
    typeControlsTable.addCell(editVariableForm.controls.basicType).style.padding = "0 10px 0 0";
    var separator1 = this.Separator();
    innerTable.addCellInNextRow(separator1).setAttribute("colspan", "2");
    separator1.style.margin = "4px 0 4px 0";

    //InitBy && Values
    var controlProps = [
        ["initBy", this.loc.PropertyMain.InitBy, 180, "DropDownList", this.GetFilterFieldIsItems()],
        ["value", this.loc.PropertyMain.Value, 310, "ExpressionTextArea", null, false],
        ["valueFrom", this.loc.PropertyMain.RangeFrom, 310, "ExpressionTextArea", null, false],
        ["valueTo", this.loc.PropertyMain.RangeTo, 310, "ExpressionTextArea", null, false],
        ["expression", this.loc.PropertyEnum.StiFilterItemExpression, 310, "ExpressionTextArea", null, true],
        ["expressionFrom", this.loc.PropertyMain.RangeFrom, 310, "ExpressionTextArea", null, true],
        ["expressionTo", this.loc.PropertyMain.RangeTo, 310, "ExpressionTextArea", null, true],
        ["valueDateTime", this.loc.PropertyMain.Value, 200, "DateControlWithCheckBox", null],
        ["valueDateTimeFrom", this.loc.PropertyMain.RangeFrom, 200, "DateControlWithCheckBox", null],
        ["valueDateTimeTo", this.loc.PropertyMain.RangeTo, 200, "DateControlWithCheckBox", null],
        ["valueBool", this.loc.Report.LabelDefaultValue, 200, "CheckBox", null],
        ["valueImage", this.loc.PropertyMain.Image, 314, "ImageControl", null],
        ["separatorInitBy"],
        ["readOnly", "", null, "CheckBox", this.loc.PropertyMain.ReadOnly],
        ["requestFromUser", "", null, "CheckBox", this.loc.PropertyMain.RequestFromUser],
        ["allowUseAsSqlParameter", "", null, "CheckBox", this.loc.PropertyMain.AllowUsingAsSqlParameter],
        ["separatorReadOnly"],
        ["allowUserValues", "", null, "CheckBox", this.loc.PropertyMain.AllowUserValues],
        ["dataSource", this.loc.PropertyMain.DataSource, 200, "DropDownList", this.GetVariableDataSourceItems()],
        ["selection", this.loc.PropertyMain.Selection, 200, "DropDownList", this.GetVariableSelectionItems()],
        ["items", this.loc.PropertyMain.Items, 280, "TextAreaWithEditButton", 70],
        ["keys", this.loc.PropertyMain.Keys, 300, "DataControl", null],
        ["values", this.loc.PropertyMain.Labels, 300, "DataControl", null],
        ["sortDirection", this.loc.FormBand.SortBy, 152, "DropDownList", this.GetVariableSortDirectionItems()],
        ["sortField", "", 130, "DropDownList", this.GetVariableSortFieldItems()],
        ["checkedColumnDataSource", this.loc.PropertyMain.Checked, 152, "DropDownList", this.GetVariableSortDirectionItems()],
        ["checkedColumnExpression", "", 130, "ExpressionControl"],
        ["dependentValue", "", null, "CheckBox", this.loc.PropertyMain.DependentValue],
        ["dependentVariable", this.loc.PropertyMain.Variable, 180, "DropDownList", []],
        ["dependentColumn", this.loc.PropertyMain.DependentColumn, 300, "DataControl", null],
        ["formatMask", this.loc.FormFormatEditor.FormatMask.replace(":", ""), 180, "TextBox", null],
        ["dateTimeFormat", this.loc.FormFormatEditor.DateTimeFormat, 180, "DropDownList", this.GetVariableDateTimeTypesItems()],
    ]

    for (var i = 0; i < controlProps.length; i++) {
        editVariableForm.controls[controlProps[i][0] + "Row"] = innerTable.addRow();

        if (controlProps[i][0].indexOf("separator") == 0) {
            var separator = this.Separator();
            innerTable.addCellInLastRow(separator).setAttribute("colspan", "2");
            separator.style.margin = "4px 0 4px 0";
        }
        else {
            var textControl = innerTable.addCellInLastRow();
            textControl.className = "stiDesignerCaptionControlsBigIntervals";
            textControl.innerHTML = controlProps[i][1];
            var control;
            if (controlProps[i][3] == "DropDownList") control = this.DropDownList("editVariableForm" + controlProps[i][0], controlProps[i][2], null, controlProps[i][4], true);
            else if (controlProps[i][3] == "ExpressionControl") control = this.ExpressionControl("editVariableForm" + controlProps[i][0], controlProps[i][2], null, null, true);
            else if (controlProps[i][3] == "ExpressionTextArea") control = this.ExpressionTextArea("editVariableForm" + controlProps[i][0], controlProps[i][2], 45, true, controlProps[i][5]);
            else if (controlProps[i][3] == "DataControl") control = this.DataControl("editVariableForm" + controlProps[i][0], controlProps[i][2]);
            else if (controlProps[i][3] == "TextBox") control = this.TextBox("editVariableForm" + controlProps[i][0], controlProps[i][2]);
            else if (controlProps[i][3] == "DateControlWithCheckBox") control = this.DateControlWithCheckBox("editVariableForm" + controlProps[i][0], controlProps[i][2]);
            else if (controlProps[i][3] == "CheckBox") control = this.CheckBox("editVariableForm" + controlProps[i][0], controlProps[i][4]);
            else if (controlProps[i][3] == "TextAreaWithEditButton") control = this.TextAreaWithEditButton("editVariableForm" + controlProps[i][0], controlProps[i][2], controlProps[i][4], true);
            else if (controlProps[i][3] == "ImageControl") control = this.ImageControl(null, controlProps[i][2], 80);

            control.editVariableForm = editVariableForm;
            editVariableForm.controls[controlProps[i][0]] = control;
            innerTable.addCellInLastRow(control).className = "stiDesignerControlCellsBigIntervals2";
        }
    }

    editVariableForm.controls.sortDirection.style.display = editVariableForm.controls.sortField.style.display = "inline-block";
    editVariableForm.controls.sortField.style.marginLeft = "12px";
    editVariableForm.controls.sortDirection.parentElement.appendChild(editVariableForm.controls.sortField);
    editVariableForm.controls.sortFieldRow.style.display = "none";

    editVariableForm.controls.checkedColumnDataSource.style.display = editVariableForm.controls.checkedColumnExpression.style.display = "inline-block";
    editVariableForm.controls.checkedColumnExpression.style.marginLeft = "12px";
    editVariableForm.controls.checkedColumnDataSource.parentElement.appendChild(editVariableForm.controls.checkedColumnExpression);
    editVariableForm.controls.checkedColumnExpressionRow.style.display = "none";

    editVariableForm.typeAndBasicTypeConflicted = function () {
        var basicType = this.variable.basicType;
        var type = this.variable.type;

        if (((basicType == "List" || basicType == "Range") && (type == "sbyte" || type == "ushort" || type == "uint" || type == "ulong" || type == "object" || type == "image" || type == "byte[]" || type == "datetimeoffset"))
            || (basicType == "Range" && type == "bool") || (basicType == "NullableValue" && type == "string"))
            return true;

        return false;
    }

    editVariableForm.changeConflictedControls = function (currPropName, conflictedPropName, currentKey) {
        if (this.variable[currPropName] != currentKey && (currPropName == "type" || (currPropName == "basicType" && (this.variable[currPropName] == "Range" || currentKey == "Range")))) {
            this.clearItems();
        }

        this.variable[currPropName] = currentKey;

        if (this.typeAndBasicTypeConflicted()) {
            if (currPropName == "basicType" && currentKey == "NullableValue" && this.variable[conflictedPropName] == "string") {
                this.controls.basicType.setKey("Value");
                this.variable.basicType = "Value";
                this.controls.basicType.showError("!", true);
            }
            var key = conflictedPropName == "type" ? "string" : "Value";
            this.controls[conflictedPropName].setKey(key);
            this.variable[conflictedPropName] = key;
        }

        this.showControlsByType();
    }

    var controlNames = ["initBy", "readOnly", "requestFromUser", "dataSource", "dependentValue", "selection", "sortDirection", "allowUserValues"];
    for (var i = 0; i < controlNames.length; i++) {
        editVariableForm.controls[controlNames[i]].action = function () {
            editVariableForm.showControlsByType();
        }
    }

    editVariableForm.controls.items.button.action = function () {
        jsObject.InitializeVariableItemsForm(function (variableItemsForm) {
            variableItemsForm.show(editVariableForm);
        });
    }

    editVariableForm.controls.valueDateTime.checkBox.action = function () {
        this.parentControl.dateControl.setEnabled(!this.isChecked);
        if (this.isChecked) {
            editVariableForm.controls.basicType.setKey("NullableValue");
            editVariableForm.controls.basicType.action();
        }
    }

    editVariableForm.controls.type.action = function () {
        editVariableForm.changeConflictedControls("type", "basicType", this.key);
    }

    editVariableForm.controls.basicType.action = function () {
        editVariableForm.changeConflictedControls("basicType", "type", this.key);
    }

    editVariableForm.updateDateTimeControls = function () {
        var controls = editVariableForm.controls;
        var dateTimeFormat = editVariableForm.controls.dateTimeFormat.key;
        controls.valueDateTime.dateTimeFormat = dateTimeFormat;
        controls.valueDateTimeFrom.dateTimeFormat = dateTimeFormat;
        controls.valueDateTimeTo.dateTimeFormat = dateTimeFormat;
        controls.valueDateTime.setKey(editVariableForm.controls.valueDateTime.key);
        controls.valueDateTimeFrom.setKey(editVariableForm.controls.valueDateTimeFrom.key);
        controls.valueDateTimeTo.setKey(editVariableForm.controls.valueDateTimeTo.key);
    }

    editVariableForm.controls.dateTimeFormat.action = function () {
        editVariableForm.controls.items.textArea.value = editVariableForm.getItemsStr();
        editVariableForm.updateDateTimeControls();
    }

    editVariableForm.controls.name.action = function () {
        if (this.oldValue == editVariableForm.controls.alias.value) {
            editVariableForm.controls.alias.value = this.value;
        }
    }

    editVariableForm.clearItems = function () {
        this.variable.items = null;
        this.controls.items.textArea.value = this.getItemsStr();
    }

    editVariableForm.showControlsByType = function () {
        var type = this.controls.type.key;
        var basicType = this.controls.basicType.key;
        var initBy = this.controls.initBy.key;
        var isValueBasicType = basicType != "Range" && basicType != "List";
        var showInitBy = type != "image" && basicType != "List";
        var arrayType = type == "byte[]" || type == "object" || type == "image";
        var enabledRequestFromUserCheckBox = !this.controls.readOnly.isChecked && !arrayType;
        var showRequestFromUserControls = this.controls.requestFromUser.isChecked && enabledRequestFromUserCheckBox;
        var enabledDataSource = basicType != "Range";
        var showDefault = this.controls.selection.key == "FromVariable" || basicType != "Value";
        if (!enabledRequestFromUserCheckBox) this.controls.requestFromUser.setChecked(false);

        this.controls.basicType.style.display = (type != "byte[]" && type != "object" && type != "image") ? "" : "none";
        this.controls.valueBoolRow.style.display = (showDefault && type == "bool" && isValueBasicType && initBy == "Value") ? "" : "none";
        this.controls.valueImageRow.style.display = (showDefault && type == "image" && isValueBasicType) ? "" : "none";
        this.controls.valueRow.style.display = (showDefault && type != "datetime" && type != "datetimeoffset" && type != "bool" && type != "image" && isValueBasicType && initBy == "Value") ? "" : "none";
        this.controls.valueFromRow.style.display = (showDefault && type != "datetime" && type != "datetimeoffset" && basicType == "Range" && initBy == "Value") ? "" : "none";
        this.controls.valueToRow.style.display = (showDefault && type != "datetime" && type != "datetimeoffset" && basicType == "Range" && initBy == "Value") ? "" : "none";
        this.controls.expressionRow.style.display = (showDefault && type != "image" && isValueBasicType && initBy == "Expression") ? "" : "none";
        this.controls.expressionFromRow.style.display = (showDefault && basicType == "Range" && initBy == "Expression") ? "" : "none";
        this.controls.expressionToRow.style.display = (showDefault && basicType == "Range" && initBy == "Expression") ? "" : "none";
        this.controls.valueDateTimeRow.style.display = (showDefault && (type == "datetime" || type == "datetimeoffset") && isValueBasicType && initBy == "Value") ? "" : "none";
        this.controls.valueDateTimeFromRow.style.display = (showDefault && type == "datetime" && basicType == "Range" && initBy == "Value") ? "" : "none";
        this.controls.valueDateTimeToRow.style.display = (showDefault && type == "datetime" && basicType == "Range" && initBy == "Value") ? "" : "none";
        this.controls.initByRow.style.display = showDefault && showInitBy ? "" : "none";
        this.controls.separatorInitByRow.style.display = showDefault && showInitBy ? "" : "none";
        this.controls.requestFromUser.setEnabled(enabledRequestFromUserCheckBox);
        this.controls.separatorReadOnlyRow.style.display = showRequestFromUserControls ? "" : "none";
        this.controls.allowUserValuesRow.style.display = showRequestFromUserControls ? "" : "none";
        this.controls.dataSourceRow.style.display = showRequestFromUserControls ? "" : "none";
        this.controls.selectionRow.style.display = showRequestFromUserControls && basicType == "Value" ? "" : "none";
        this.controls.readOnlyRow.style.display = !arrayType ? "" : "none";
        this.controls.requestFromUserRow.style.display = !arrayType ? "" : "none";
        this.controls.allowUseAsSqlParameterRow.style.display = !arrayType ? "" : "none";

        if (!enabledDataSource) this.controls.dataSource.setKey("Items");
        this.controls.dataSource.setEnabled(enabledDataSource);
        this.controls.sortField.setEnabled(this.controls.sortDirection.key != "None");
        this.controls.checkedColumnDataSource.setEnabled(!this.controls.allowUserValues.isChecked);
        this.controls.checkedColumnExpression.setEnabled(!this.controls.allowUserValues.isChecked);

        if (basicType == "Range" || basicType == "List") {
            editVariableForm.controls.allowUseAsSqlParameter.setChecked(false);
        }
        editVariableForm.controls.allowUseAsSqlParameter.setEnabled(basicType != "Range" && basicType != "List");

        var showColumns = showRequestFromUserControls && this.controls.dataSource.isEnabled && this.controls.dataSource.key == "Columns";
        var showDependentValue = showColumns && type != "object" && (basicType == "Value" || basicType == "List");

        this.controls.itemsRow.style.display = (showRequestFromUserControls && this.controls.dataSource.key == "Items") ? "" : "none";
        this.controls.keysRow.style.display = showColumns ? "" : "none";
        this.controls.valuesRow.style.display = showColumns ? "" : "none";
        this.controls.sortDirectionRow.style.display = showColumns ? "" : "none";
        this.controls.formatMaskRow.style.display = (showRequestFromUserControls && type == "string") ? "" : "none";
        this.controls.dateTimeFormatRow.style.display = (showRequestFromUserControls && (type == "datetime" || type == "datetimeoffset")) ? "" : "none";
        this.controls.dependentValueRow.style.display = showDependentValue ? "" : "none";
        this.controls.dependentVariableRow.style.display = (showDependentValue && this.controls.dependentValue.isChecked) ? "" : "none";
        this.controls.dependentColumnRow.style.display = (showDependentValue && this.controls.dependentValue.isChecked) ? "" : "none";
        this.controls.checkedColumnDataSourceRow.style.display = showColumns && basicType == "List" ? "" : "none";
    }

    editVariableForm.resetControls = function () {
        for (var i = 0; i < controlProps.length; i++) {
            if (editVariableForm.controls[controlProps[i][0]]) {
                if (editVariableForm.controls[controlProps[i][0]].value != null)
                    editVariableForm.controls[controlProps[i][0]].value = ""
                else if (editVariableForm.controls[controlProps[i][0]].textBox != null)
                    editVariableForm.controls[controlProps[i][0]].textBox.value = "";
            }
        }
        this.controls.valueDateTime.setChecked(false);
        this.controls.valueDateTimeFrom.setChecked(false);
        this.controls.valueDateTimeTo.setChecked(false);
        this.controls.valueDateTime.setKey(new Date());
        this.controls.valueDateTimeFrom.setKey(new Date());
        this.controls.valueDateTimeTo.setKey(new Date());
        this.controls.expressionFrom.setValue("");
        this.controls.expression.setValue("");
        this.controls.expressionFrom.setValue("");
        this.controls.expressionTo.setValue("");
        this.controls.value.setValue("");
        this.controls.valueFrom.setValue("");
        this.controls.valueTo.setValue("");
    }

    editVariableForm.onshow = function () {
        this.resetControls();
        this.mode = "Edit";
        if (this.variable == null) {
            this.variable = jsObject.VariableObject();
            this.mode = "New";
        }
        saveCopyButton.style.display = this.mode == "Edit" ? "" : "none";
        this.editableDictionaryItem = this.mode == "Edit" && jsObject.options.dictionaryTree
            ? jsObject.options.dictionaryTree.selectedItem : null;
        var caption = jsObject.loc.FormDictionaryDesigner["Variable" + this.mode];
        this.caption.innerHTML = caption;
        this.controls.name.hideError();
        this.controls.name.focus();
        this.controls.name.value = this.variable.name;
        this.controls.alias.value = this.variable.alias;
        this.controls.description.value = StiBase64.decode(this.variable.description);
        this.controls.type.setKey(this.variable.type);
        this.controls.basicType.setKey(this.variable.basicType);
        this.controls.initBy.setKey(this.variable.initBy);
        this.controls.readOnly.setChecked(this.variable.readOnly);
        this.controls.allowUseAsSqlParameter.setChecked(this.variable.allowUseAsSqlParameter);
        this.controls.requestFromUser.setChecked(this.variable.requestFromUser);
        this.controls.allowUserValues.setChecked(this.variable.allowUserValues);
        this.controls.dateTimeFormat.setKey(this.variable.dateTimeFormat);
        this.controls.sortDirection.setKey(this.variable.sortDirection);
        this.controls.sortField.setKey(this.variable.sortField);
        this.controls.dataSource.setKey(this.variable.dataSource);
        this.controls.selection.setKey(this.variable.selection);
        this.controls.formatMask.value = StiBase64.decode(this.variable.formatMask);
        this.controls.items.textArea.value = this.getItemsStr();
        this.controls.keys.textBox.value = this.variable.keys;
        this.controls.values.textBox.value = this.variable.values;
        this.controls.dependentValue.setChecked(this.variable.dependentValue);
        this.controls.dependentVariable.menu.addItems(this.getVariablesItems());
        this.controls.dependentVariable.setKey(this.variable.dependentVariable);
        this.controls.dependentColumn.textBox.value = this.variable.dependentColumn;
        this.controls.valueImage.setImage(null);

        var dataSourceItems = [];
        dataSourceItems.push(jsObject.Item("[No]", "[" + jsObject.loc.FormFormatEditor.nameNo + "]", null, ""));
        dataSourceItems = dataSourceItems.concat(jsObject.GetDataSourceItemsFromDictionary());
        this.controls.checkedColumnDataSource.addItems(dataSourceItems);

        if ("checkedColumn" in this.variable) {
            var checkedColumnArray = this.variable.checkedColumn.split(";");
            this.controls.checkedColumnDataSource.setKey(checkedColumnArray[0] || "");
            this.controls.checkedColumnExpression.textBox.value = checkedColumnArray.length > 1 ? checkedColumnArray[1] : "";
        }

        switch (this.variable.basicType) {
            case "Value":
            case "NullableValue":
                {
                    if (this.variable.initBy == "Value") {
                        if (this.variable.type == "image") {
                            this.controls.valueImage.setImage(this.variable.value);
                        }
                        else {
                            this.variable.value = StiBase64.decode(this.variable.value);
                            if (this.variable.type == "bool")
                                this.controls.valueBool.setChecked(this.variable.value == "true" || this.variable.value == "True");
                            else
                                if (this.variable.type == "datetime" || this.variable.type == "datetimeoffset") {
                                    if (this.variable.value == "") {
                                        this.controls.valueDateTime.setChecked(true);
                                    }
                                    else {
                                        var dateStr = this.variable.value;

                                        if (this.variable.type == "datetimeoffset" && dateStr.indexOf("+") > 0)
                                            dateStr = dateStr.substring(0, dateStr.indexOf("+"));

                                        this.controls.valueDateTime.setKey(new Date(dateStr));
                                    }
                                }
                                else
                                    this.controls.value.setValue(this.variable.value != null ? this.variable.value : "");
                        }
                    }
                    else {

                        this.variable.expression = StiBase64.decode(this.variable.expression);
                        this.controls.expression.setValue(this.variable.expression != null ? this.variable.expression : "");
                    }
                    break;
                }
            case "Range":
                {
                    if (this.variable.initBy == "Value") {
                        this.variable.valueFrom = StiBase64.decode(this.variable.valueFrom);
                        this.variable.valueTo = StiBase64.decode(this.variable.valueTo);
                        if (this.variable.type == "datetime") {
                            if (this.variable.valueFrom == "") this.controls.valueDateTimeFrom.setChecked(true);
                            else this.controls.valueDateTimeFrom.setKey(new Date(this.variable.valueFrom));
                            if (this.variable.valueTo == "") this.controls.valueDateTimeTo.setChecked(true);
                            else this.controls.valueDateTimeTo.setKey(new Date(this.variable.valueTo));
                        }
                        else {
                            this.controls.valueFrom.setValue(this.variable.valueFrom);
                            this.controls.valueTo.setValue(this.variable.valueTo);
                        }
                    }
                    else {
                        this.variable.expressionFrom = StiBase64.decode(this.variable.expressionFrom);
                        this.variable.expressionTo = StiBase64.decode(this.variable.expressionTo);
                        this.controls.expressionFrom.setValue(this.variable.expressionFrom);
                        this.controls.expressionTo.setValue(this.variable.expressionTo);
                    }
                    break;
                }
        }

        this.showControlsByType();
        this.updateDateTimeControls();
    }

    editVariableForm.action = function () {
        var variable = {};
        variable.mode = this.mode;

        if (!this.controls.name.checkNotEmpty(jsObject.loc.PropertyMain.Name)) return;
        if ((this.mode == "New" || this.controls.name.value != this.variable.name) &&
            !(this.controls.name.checkExists(jsObject.GetVariablesFromDictionary(jsObject.options.report.dictionary), "name") &&
                this.controls.name.checkExists(jsObject.GetDataSourcesFromDictionary(jsObject.options.report.dictionary), "name")))
            return;

        if (this.mode == "Edit") variable.oldName = this.variable.name;
        var variableCategoryItem = jsObject.options.dictionaryTree.getCurrentVariableParent();
        variable.category = variableCategoryItem.itemObject.typeItem == "Category" ? variableCategoryItem.itemObject.name : "";
        variable.name = this.controls.name.value;
        variable.alias = this.controls.alias.value;
        variable.description = StiBase64.encode(this.controls.description.value);
        variable.type = this.controls.type.key;
        variable.basicType = this.controls.basicType.key;
        if (variable.basicType != "List") {
            variable.initBy = this.controls.initBy.key;
            if (variable.type == "image") {
                variable.value = this.controls.valueImage.src;
            }
            else if (variable.initBy == "Expression") {
                if (variable.basicType != "Range") variable.expression = StiBase64.encode(this.controls.expression.getValue());
                else {
                    variable.expressionFrom = StiBase64.encode(this.controls.expressionFrom.getValue());
                    variable.expressionTo = StiBase64.encode(this.controls.expressionTo.getValue());
                }
            }
            else {
                if (variable.basicType != "Range") {
                    variable.value = StiBase64.encode(variable.type == "bool"
                        ? (this.controls.valueBool.isChecked ? "True" : "False")
                        : ((variable.type == "datetime" || variable.type == "datetimeoffset")
                            ? (!this.controls.valueDateTime.isChecked ? jsObject.DateToStringAmericanFormat(this.controls.valueDateTime.key) : "")
                            : this.controls.value.getValue()));
                }
                else {
                    variable.valueFrom = StiBase64.encode(variable.type == "datetime"
                        ? (!this.controls.valueDateTimeFrom.isChecked ? jsObject.DateToStringAmericanFormat(this.controls.valueDateTimeFrom.key) : "")
                        : this.controls.valueFrom.getValue());
                    variable.valueTo = StiBase64.encode(variable.type == "datetime"
                        ? (!this.controls.valueDateTimeTo.isChecked ? jsObject.DateToStringAmericanFormat(this.controls.valueDateTimeTo.key) : "")
                        : this.controls.valueTo.getValue());
                }
            }
        }

        variable.readOnly = this.controls.readOnly.isChecked && variable.type != "image";
        variable.allowUseAsSqlParameter = this.controls.allowUseAsSqlParameter.isChecked && variable.type != "image";
        variable.requestFromUser = this.controls.requestFromUser.isEnabled && this.controls.requestFromUser.isChecked && variable.type != "image";
        variable.allowUserValues = variable.requestFromUser ? this.controls.allowUserValues.isChecked : true;
        variable.dateTimeFormat = this.controls.dateTimeFormatRow.style.display == "" ? this.controls.dateTimeFormat.key : "DateAndTime";
        variable.dataSource = this.controls.dataSourceRow.style.display == "" ? this.controls.dataSource.key : "Items";
        variable.sortDirection = variable.dataSource == "Items" ? "None" : this.controls.sortDirection.key;
        variable.sortField = this.controls.sortField.key;
        variable.selection = this.controls.selection.key;
        variable.formatMask = this.controls.formatMaskRow.style.display == "" ? StiBase64.encode(this.controls.formatMask.value) : "";
        variable.keys = this.controls.keysRow.style.display == "" ? this.controls.keys.textBox.value : "";
        variable.values = this.controls.valuesRow.style.display == "" ? this.controls.values.textBox.value : "";
        variable.dependentValue = this.controls.dependentValueRow.style.display == "" ? this.controls.dependentValue.isChecked : false;
        variable.dependentVariable = this.controls.dependentVariableRow.style.display == "" ? this.controls.dependentVariable.key : "";
        variable.dependentColumn = this.controls.dependentColumnRow.style.display == "" ? this.controls.dependentColumn.textBox.value : "";
        variable.items = variable.dataSource == "Items" ? this.variable.items : [];
        variable.checkedColumn = this.controls.checkedColumnDataSource.key != "" ? (this.controls.checkedColumnDataSource.key + ";" + this.controls.checkedColumnExpression.textBox.value) : "";
        variable.checkedStates = this.variable.checkedStates || [];

        this.changeVisibleState(false);
        jsObject.SendCommandCreateOrEditVariable(variable);
    }

    editVariableForm.getVariablesItems = function () {
        var items = [];
        var variables = jsObject.GetVariablesFromDictionary(jsObject.options.report.dictionary);
        for (var i = 0; i < variables.length; i++)
            if (variables[i].name != this.variable.name && variables[i].dependentVariable != this.variable.name)
                items.push(jsObject.Item("item" + i, variables[i].name, null, variables[i].name));

        return items;
    }

    editVariableForm.getItemsStr = function () {
        var items = this.variable.items;
        var resultStr = "";
        if (items == null) return "";
        for (var i = 0; i < items.length; i++) {
            if (i != 0) resultStr += (jsObject.options.listSeparator ? jsObject.options.listSeparator + " " : "; ");
            resultStr += this.getItemCaption(items[i]);
        }

        return resultStr;
    }

    editVariableForm.getItemCaption = function (itemObject) {
        var itemCaption = StiBase64.decode(itemObject.value || "");
        if (itemCaption == "") {
            var key = itemObject.key != null ? StiBase64.decode(itemObject.key) : null;
            var keyTo = itemObject.keyTo != null ? StiBase64.decode(itemObject.keyTo) : null;
            itemCaption = key;
            var isDateTimeValue = this.variable.type == "datetime" && itemObject.type != "expression";
            if (isDateTimeValue) itemCaption = jsObject.DateAmericanFormatToLocalFormat(key, this.controls.dateTimeFormat.key);
            if (keyTo != null) {
                if (isDateTimeValue) itemCaption += " - " + jsObject.DateAmericanFormatToLocalFormat(keyTo, this.controls.dateTimeFormat.key);
                else itemCaption += " - " + keyTo;
            }
        }
        if (itemObject.type == "expression") itemCaption = "{" + itemCaption + "}";
        if (itemCaption == "") itemCaption = jsObject.loc.PropertyMain.Value;

        return itemCaption;
    }

    return editVariableForm;
}