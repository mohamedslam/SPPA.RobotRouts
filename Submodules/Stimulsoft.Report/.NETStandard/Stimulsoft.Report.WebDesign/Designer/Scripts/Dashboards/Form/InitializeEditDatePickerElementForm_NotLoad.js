
StiMobileDesigner.prototype.InitializeEditDatePickerElementForm_ = function () {
    var form = this.DashboardBaseForm("editDatePickerElementForm", this.loc.Components.StiDatePicker, 1, this.HelpLinks["datePickerElement"]);
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";
    form.hideButtonsPanel();
    var jsObject = this;

    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.width = "330px";

    form.container.appendChild(controlsTable);
    form.container.style.padding = "0 0 6px 0";

    //field
    var field = this.ExpressionControlWithMenu(null, 305, null, null, true, true);
    var fieldMenu = this.options.menus.datePickerFieldMenu || this.InitializeFilterElementFieldMenu("datePickerFieldMenu", field, form, true);
    field.menu = fieldMenu;
    fieldMenu.parentButton = field.button;
    field.textBox.setAttribute("placeholder", jsObject.loc.PropertyMain.Field);

    //DataColumns
    var dataColumns = [
        ["value", this.loc.PropertyMain.Value]
    ];

    for (var i = 0; i < dataColumns.length; i++) {
        var container = this.DashboardDataColumnContainer(form, dataColumns[i][0], dataColumns[i][1], null, null, fieldMenu);
        container.allowSelected = true;
        container.maxWidth = 360;
        form.addControlRow(controlsTable, null, dataColumns[i][0] + "DataColumn", container, "0px 12px " + (i == dataColumns.length - 1 ? "12px" : "0px" + " 12px"));

        container.action = function (actionName) {
            if (actionName == "rename" && this.dataColumnObject) {
                form.sendCommand({
                    command: "RenameMeter",
                    containerName: form.jsObject.UpperFirstChar(this.name),
                    newLabel: this.dataColumnObject.label
                },
                    function (answer) {
                        form.updateControls(answer.elementProperties);
                        form.updateElementProperties(answer.elementProperties);
                        form.updateSvgContent(answer.elementProperties.svgContent);
                    }
                );
                return;
            }
            if (!this.dataColumnObject) {
                form.controls.field.currentContainer = null;
                form.controls.field.textBox.value = "";
                form.controls.field.setEnabled(false);
            }
            else if (this.dataColumnObject.type != "datetime") {
                this.dataColumnObject = null;
                var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                errorMessageForm.show(jsObject.loc.Dashboard.FieldTypeRestrictionHint.replace("{0}", jsObject.loc.FormFormatEditor.Date), true);
            }
            form.applyDataColumnPropertyToDatePickerElement(this);
        }

        container.onSelected = function () {
            for (var i = 0; i < dataColumns.length; i++) {
                if (form.controls[dataColumns[i][0] + "DataColumn"] != this)
                    form.controls[dataColumns[i][0] + "DataColumn"].setSelected(false);
            }
            if (this.dataColumnObject) {
                form.controls.field.currentContainer = this;
                form.controls.field.setEnabled(true);
                form.controls.field.textBox.value = this.dataColumnObject.expression;
            }
        }
    }

    var parentValueContainer = form.controls.valueDataColumn.innerContainer.parentElement;

    form.addControlRow(controlsTable, this.loc.PropertyMain.Field, "fieldCaption", null, "6px 12px 6px 12px");
    form.addControlRow(controlsTable, null, "field", field, "6px 12px 6px 12px");

    //SelectionMode
    var selectionMode = this.DropDownList("datePickerElementSelectionMode", 155, null, this.GetDateSelectionModeItems(), true, null, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.SelectionMode, "selectionMode", selectionMode, "6px 12px 6px 12px", null, true, null, true);
    selectionMode.action = function () {
        form.updateControlsVisibleStates();
        form.applyPropertiesToDatePickerElement("SelectionMode", this.key);
    }

    //Condition
    var condition = this.DropDownList("datePickerElementCondition", 155, null, this.GetDateConditionItems(), true, null, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Condition, "condition", condition, "6px 12px 6px 12px", null, true, null, true);
    condition.action = function () {
        form.updateControlsVisibleStates();
        form.applyPropertiesToDatePickerElement("Condition", this.key);
    }

    //InitialRangeSelectionSource
    var initialRangeSelectionSource = this.DropDownList("datePickerElementRangeSelectionSource", 155, null, this.GetDateInitialRangeSelectionSourceItems(), true, null, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.InitialSelectionSource, "initialRangeSelectionSource", initialRangeSelectionSource, "6px 12px 6px 12px", null, true, null, true);
    initialRangeSelectionSource.action = function () {
        form.applyPropertiesToDatePickerElement("InitialRangeSelectionSource", this.key);
        form.updateControlsVisibleStates();
    }

    //InitialRangeSelection
    var initialRangeSelection = this.DropDownList("datePickerElementRangeSelection", 155, null, this.GetDateInitialRangeSelectionItems(), true, null, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.InitialSelection, "initialRangeSelection", initialRangeSelection, "6px 12px 6px 12px", null, true, null, true);
    initialRangeSelection.action = function () {
        form.applyPropertiesToDatePickerElement("InitialRangeSelection", this.key);
    }

    form.setValues = function () {
        selectionMode.setKey(this.datePickerProperties.selectionMode);
        condition.setKey(this.datePickerProperties.conditionDatePicker);
        initialRangeSelection.setKey(this.datePickerProperties.initialRangeSelection);
        initialRangeSelectionSource.setKey(this.datePickerProperties.initialRangeSelectionSource);

        var meters = this.datePickerProperties.meters;

        for (var i = 0; i < dataColumns.length; i++) {
            var meter = meters[dataColumns[i][0]];
            var container = this.controls[dataColumns[i][0] + "DataColumn"];

            if (meter) {
                container.addColumn(meter.label, meter);
                if (container.isSelected && container.item) container.item.action();
            }
            else
                container.clear();
        }
    }

    form.checkStartMode = function () {
        var itemsCount = 0;
        for (var i = 0; i < dataColumns.length; i++) {
            var container = form.controls[dataColumns[i][0] + "DataColumn"];
            if (container.dataColumnObject) itemsCount++;
        }

        var valueContainer = form.controls.valueDataColumn.innerContainer;

        if (itemsCount == 0) {
            form.container.appendChild(valueContainer);
            controlsTable.style.display = "none";
            valueContainer.style.height = valueContainer.style.maxHeight = "210px";
            valueContainer.style.width = "267px";
            valueContainer.style.margin = "6px 12px 6px 12px";
        }
        else {
            parentValueContainer.appendChild(valueContainer);
            controlsTable.style.display = "";
            valueContainer.style.height = "30px";
            valueContainer.style.width = "auto";
            valueContainer.style.margin = "0";
        }
    }

    form.onshow = function () {
        form.currentPanelName = jsObject.options.propertiesPanel.getCurrentPanelName();
        if (jsObject.options.showDictionary) jsObject.options.propertiesPanel.showContainer("Dictionary");

        selectionMode.setKey("Single");
        condition.setKey("GreaterThanOrEqualTo");
        form.controls.conditionRow.style.display = "none";
        form.controls.initialRangeSelectionRow.style.display = "none";
        form.controls.initialRangeSelectionSourceRow.style.display = "none";
        field.textBox.value = "";
        field.setEnabled(false);

        for (var i = 0; i < dataColumns.length; i++) {
            var container = this.controls[dataColumns[i][0] + "DataColumn"];
            container.clear();
        }

        form.controls.valueDataColumn.setSelected(true);
        form.checkStartMode();

        form.sendCommand({ command: "GetDatePickerElementProperties" },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.checkStartMode();
                form.correctTopPosition();
            }
        );
    }

    form.onhide = function () {
        jsObject.options.propertiesPanel.showContainer(form.currentPanelName);
    }

    form.updateControlsVisibleStates = function () {
        var isVariablePresent = form.datePickerProperties.isVariablePresent;
        var isRangeVariablePresent = form.datePickerProperties.isRangeVariablePresent;
        form.controls.conditionRow.style.display = selectionMode.key == "Single" && !isVariablePresent ? "" : "none";
        form.controls.initialRangeSelectionRow.style.display = selectionMode.key == "Range" && (!isVariablePresent || (isRangeVariablePresent && initialRangeSelectionSource.key == "Selection")) ? "" : "none";
        form.controls.initialRangeSelectionSourceRow.style.display = isRangeVariablePresent ? "" : "none";
        selectionMode.setEnabled(!isVariablePresent);
    }

    form.updateControls = function (datePickerProperties) {
        if (!datePickerProperties) return;
        form.datePickerProperties = datePickerProperties;
        form.setValues();
        form.updateControlsVisibleStates();
    }

    form.applyDataColumnPropertyToDatePickerElement = function (container) {
        form.sendCommand(
            {
                command: "SetDataColumn",
                containerName: jsObject.UpperFirstChar(container.name),
                dataColumnObject: container.dataColumnObject
            },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
                if (container.item) container.item.action();
                form.checkStartMode();
            }
        );
    }

    form.applyPropertiesToDatePickerElement = function (propertyName, propertyValue) {
        form.sendCommand({ command: "SetPropertyValue", propertyName: propertyName, propertyValue: propertyValue },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.updateElementProperties(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
            }
        );
    }

    form.sendCommand = function (updateParameters, callbackFunction) {
        jsObject.SendCommandToDesignerServer("UpdateDatePickerElement",
            {
                componentName: form.currentDatePickerElement.properties.name,
                updateParameters: updateParameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    form.updateSvgContent = function (svgContent) {
        this.currentDatePickerElement.properties.svgContent = svgContent;
        this.currentDatePickerElement.repaint();
    }

    form.updateElementProperties = function (properties) {
        var jsObject = this.jsObject;
        for (var propertyName in properties) {
            this.currentDatePickerElement.properties[propertyName] = properties[propertyName];
        }
        jsObject.options.homePanel.updateControls();
        if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
    }

    return form;
}