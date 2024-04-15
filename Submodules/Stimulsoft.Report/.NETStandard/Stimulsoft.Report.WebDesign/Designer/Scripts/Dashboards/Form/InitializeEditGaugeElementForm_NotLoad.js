
StiMobileDesigner.prototype.InitializeEditGaugeElementForm_ = function () {
    var form = this.DashboardBaseForm("editGaugeElementForm", this.loc.Components.StiGauge, 1, this.HelpLinks["gaugeElement"]);
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.container.style.padding = "0 0 6px 0";
    form.caption.style.padding = "0px 10px 0 12px";
    form.hideButtonsPanel();
    var jsObject = this;

    //Data Manually Block
    var dataManuallyTable = this.CreateHTMLTable();
    dataManuallyTable.style.margin = "0 12px 0 12px";
    dataManuallyTable.style.display = "none";
    var dataHeader = dataManuallyTable.addTextCell(this.loc.PropertyMain.Data);
    dataHeader.className = "stiDesignerTextContainer";
    dataHeader.style.padding = "12px 0 12px 0";

    var dataModeUsingFieldsButton = this.FormImageButton(null, "RetrieveColumnsArrow.png", jsObject.loc.Report.UseDataFields);
    dataModeUsingFieldsButton.style.display = "inline-block";
    dataManuallyTable.addCell(dataModeUsingFieldsButton).style.textAlign = "right";

    dataModeUsingFieldsButton.action = function () {
        form.switchOffStartMode();
        form.setDataMode("UsingDataFields");
        form.applyPropertiesToGaugeElement("DataMode", form.dataMode);
    }

    var dataManuallyControl = this.InitializeManualDataControl(330, 260, [["Value", this.loc.PropertyMain.Value], ["Target", this.loc.PropertyMain.Target], ["Series", this.loc.Chart.Series]], 10);
    dataManuallyTable.addCellInNextRow(dataManuallyControl).setAttribute("colspan", "2");

    dataManuallyControl.action = function () {
        var value = this.getValue();
        form.updateControlsVisibleStates();

        form.sendCommand({ command: "SetValueToManuallyEnteredData", propertyValue: value != null ? JSON.stringify(value) : null },
            function (answer) {
                form.updateElementProperties(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
            }
        );
    }

    form.container.appendChild(dataManuallyTable);
    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.width = "100%";
    form.container.appendChild(controlsTable);

    var controlsTable2 = this.CreateHTMLTable();
    controlsTable2.style.width = "355px";
    form.container.appendChild(controlsTable2);

    //Expression
    var expressionControl = this.ExpressionControlWithMenu(null, 325, null, null);
    var gaugeExpressionMenu = this.options.menus.gaugeExpressionMenu || this.InitializeDataColumnExpressionMenu("gaugeExpressionMenu", expressionControl, form);
    expressionControl.menu = gaugeExpressionMenu;
    gaugeExpressionMenu.parentButton = expressionControl.button;
    expressionControl.textBox.setAttribute("placeholder", jsObject.loc.PropertyMain.Field);

    expressionControl.action = function () {
        if (this.currentContainer) {
            form.applyExpressionPropertyToGaugeElement(this.currentContainer, this.textBox.value);
        }
    }

    expressionControl.refreshExpressionHint = function () {
        expressionControl.textBox.setAttribute("placeholder", this.currentContainer ? (this.currentContainer.name == "series" ? jsObject.loc.PropertyMain.Field : "Sum(" + jsObject.loc.PropertyMain.Field + ")") : "");
    }

    var dataColumns = [
        ["value", this.loc.PropertyMain.Value],
        ["target", this.loc.PropertyMain.Target],
        ["series", this.loc.Chart.Series]
    ];

    for (var i = 0; i < dataColumns.length; i++) {
        var container = this.DashboardDataColumnContainer(form, dataColumns[i][0], dataColumns[i][1], null, null, gaugeExpressionMenu, dataColumns[i][0] == "value");
        container.allowSelected = true;
        container.maxWidth = 300;
        form.addControlRow(controlsTable, null, dataColumns[i][0] + "DataColumn", container, "0px 12px 0px 12px");

        container.action = function (actionName) {
            if (actionName == "rename" && this.dataColumnObject) {
                form.sendCommand({
                    command: "RenameMeter",
                    containerName: form.jsObject.UpperFirstChar(this.name),
                    newLabel: this.dataColumnObject.label
                },
                    function (answer) {
                        form.updateControls(answer.elementProperties);
                        form.updateSvgContent(answer.elementProperties.svgContent);
                    }
                );
                return;
            }
            if (!this.dataColumnObject) {
                form.controls.expression.currentContainer = null;
                form.controls.expression.setEnabled(false);
                form.controls.expression.refreshExpressionHint();
            }
            form.setDataMode("UsingDataFields");
            form.switchOffStartMode();
            form.applyDataColumnPropertyToGaugeElement(this);
        }

        container.onSelected = function () {
            for (var i = 0; i < dataColumns.length; i++) {
                if (form.controls[dataColumns[i][0] + "DataColumn"] != this)
                    form.controls[dataColumns[i][0] + "DataColumn"].setSelected(false);
            }

            if (this.dataColumnObject && this.dataColumnObject.expression != null) {
                form.controls.expression.currentContainer = this;
                form.controls.expression.setEnabled(true);
                form.controls.expression.textBox.value = StiBase64.decode(this.dataColumnObject.expression);
                form.controls.expression.refreshExpressionHint();
            }
        }
    }

    var valueContainer = form.controls.valueDataColumn.innerContainer;
    var parentValueContainer = valueContainer.parentElement;

    var dataModeManuallyButton = this.FormImageButton(null, "EditTable.png", this.loc.Report.EnterDataManually);
    dataModeManuallyButton.style.display = "inline-block";

    dataModeManuallyButton.action = function () {
        form.switchOffStartMode();
        form.setDataMode("ManuallyEnteringData");
        form.applyPropertiesToGaugeElement("DataMode", form.dataMode);
    }

    form.controls.valueDataColumn.innerTable.addCell(dataModeManuallyButton).style.textAlign = "right";
    parentValueContainer.setAttribute("colspan", "2");

    form.controls.valueDataColumn.actionEnterManuallyData = function () {
        form.switchOffStartMode();
        form.setDataMode("ManuallyEnteringData");
        form.applyPropertiesToGaugeElement("DataMode", "ManuallyEnteringData");
    }

    form.controls.valueDataColumn.actionShowMore = function () {
        form.switchOffStartMode();
        form.setDataMode("UsingDataFields");
        form.applyPropertiesToGaugeElement("DataMode", "UsingDataFields");
    }

    form.addControlRow(controlsTable, this.loc.PropertyMain.Expression, "expressionControlCaption", null, null, "6px 12px 6px 12px");
    form.addControlRow(controlsTable, null, "expression", expressionControl, "6px 12px 6px 12px");

    //Type
    var types = ["FullCircular", "HalfCircular", "Linear", "HorizontalLinear", "Bullet"];
    var typesTable = this.CreateHTMLTable();
    typesTable.buttons = {};
    for (var i = 0; i < types.length; i++) {
        var button = this.FormButtonWithThemeBorder(null, null, null, "Gauge.Small." + types[i] + ".png", this.loc.PropertyEnum["StiGaugeType" + types[i]]);
        button.type = types[i];
        button.style.marginRight = "6px";
        typesTable.addCell(button);
        typesTable.buttons[types[i]] = button;

        if (this.options.isTouchDevice) {
            button.style.width = button.style.height = "24px";
            button.imageCell.style.padding = "0";
        }

        button.action = function () {
            this.select();
            form.applyPropertiesToGaugeElement("Type", this.type);
        }

        button.select = function () {
            for (var name in typesTable.buttons) {
                typesTable.buttons[name].setSelected(false);
            }
            this.setSelected(true);
        }
    }
    form.addControlRow(controlsTable2, this.loc.PropertyMain.Type, "typesTable", typesTable, "6px 0 6px 12px");

    form.getGaugetType = function () {
        for (var name in typesTable.buttons) {
            if (typesTable.buttons[name].isSelected)
                return name;
        }
        return null
    }

    //CalculationMode
    var calculationMode = this.DropDownList("gaugeElementCalculationMode", 180, null, this.GetGaugeCalculationModeItems(), true, null, null, true);
    form.addControlRow(controlsTable2, this.loc.PropertyMain.Mode, "calculationMode", calculationMode, "6px 12px 6px 12px", null, true, null, true);
    calculationMode.action = function () {
        form.updateControlsVisibleStates();
        form.applyPropertiesToGaugeElement("CalculationMode", this.key);
        form.correctTopPosition();
    }

    //Min Max
    var minMaxTable = this.CreateHTMLTable();
    minMaxTable.style.maxWidth = "150px";
    minMaxTable.addTextCell(this.loc.PropertyMain.Minimum).className = "stiDesignerTextContainer";
    minMaxTable.addCell();
    minMaxTable.addTextCell(this.loc.PropertyMain.Maximum).className = "stiDesignerTextContainer";
    form.addControlRow(controlsTable2, " ", "minMaxTable", minMaxTable, "6px 12px 6px 12px", null, true, null, true);

    var minControl = this.TextBoxEnumerator(null, 80);
    minMaxTable.addCellInNextRow(minControl);
    minControl.action = function () {
        form.applyPropertiesToGaugeElement("Minimum", this.textBox.value);
    }

    minMaxTable.addTextCellInLastRow(" - ").style.padding = "7px 5px 7px 5px";
    var maxControl = this.TextBoxEnumerator(null, 79);
    minMaxTable.addCellInLastRow(maxControl);
    maxControl.action = function () {
        form.applyPropertiesToGaugeElement("Maximum", this.textBox.value);
    }

    //RangeType
    var rangeType = this.DropDownList("gaugeElementRangeType", 180, null, this.GetGaugeRangeTypeItems(), true, null, null, true);
    form.addControlRow(controlsTable2, this.loc.Dashboard.RangeType, "rangeType", rangeType, "6px 12px 6px 12px", null, true, null, true);
    rangeType.action = function () {
        form.updateControlsVisibleStates();
        form.applyPropertiesToGaugeElement("RangeType", this.key);
        form.correctTopPosition();
    }

    var rangesSeparator = this.FormSeparator();
    form.addControlRow(controlsTable2, null, "rangesSeparator", rangesSeparator, "6px 12px 6px 12px");

    var rangeTable = this.CreateHTMLTable();
    form.addControlRow(controlsTable2, null, "rangeTable", rangeTable, "6px 0px 6px 0px", null, true, null, true);
    rangeTable.style.width = "100%";

    //RangeMode
    var rangeMode = this.DropDownList("gaugeElementRangeMode", 120, null, this.GetGaugeRangeModeItems(), true, null, null, true);
    rangeMode.style.marginLeft = "12px";
    rangeTable.addCell(rangeMode);
    rangeMode.action = function () {
        form.applyPropertiesToGaugeElement("RangeMode", this.key);
    }

    rangeTable.addCell().style.width = "100%";

    //AddRangeButton
    var addRangeButton = this.FormButton(null, null, this.loc.Dashboard.AddRange);
    addRangeButton.style.marginRight = "12px";
    rangeTable.addCell(addRangeButton);

    //Ranges Container
    var rangesContainer = this.GaugeRangesContainer(form);
    rangesContainer.style.margin = "6px 12px 0 12px";
    rangeTable.addCellInNextRow(rangesContainer).setAttribute("colspan", "3");

    addRangeButton.action = function () {
        form.sendCommand({ command: "AddGaugeRange" },
            function (answer) {
                form.updateControls(answer.elementProperties);
                if (rangesContainer.childNodes.length > 0) {
                    rangesContainer.childNodes[rangesContainer.childNodes.length - 1].select();
                }
                form.updateSvgContent(answer.elementProperties.svgContent);
            }
        );
    }

    //Range Values
    var rangeValuesTable = this.CreateHTMLTable();
    form.addControlRow(controlsTable2, " ", "rangeValuesTable", rangeValuesTable, "0px 12px 6px 12px", null, true, null, true);
    rangeValuesTable.parentNode.style.textAlign = "right";

    var rangeStart = this.TextBox(null, 80);
    rangeValuesTable.addCell(rangeStart);
    rangeStart.action = function () {
        if (rangesContainer.selectedItem) {
            form.applyPropertiesToGaugeRange("Start", jsObject.StrToDouble(this.value), rangesContainer.getSelectedItemIndex());
        }
    }

    rangeValuesTable.addTextCellInLastRow(" - ").style.padding = "7px 5px 7px 5px";
    var rangeEnd = this.TextBox(null, 79);
    rangeValuesTable.addCell(rangeEnd);
    rangeEnd.action = function () {
        if (rangesContainer.selectedItem) {
            form.applyPropertiesToGaugeRange("End", jsObject.StrToDouble(this.value), rangesContainer.getSelectedItemIndex());
        }
    }

    //Color
    var rangeColor = this.ColorControl(null, null, null, 180, true);
    rangeColor.action = function () {
        if (rangesContainer.selectedItem) {
            form.applyPropertiesToGaugeRange("Color", this.key, rangesContainer.getSelectedItemIndex());
        }
    }
    form.addControlRow(controlsTable2, " ", "rangeColor", rangeColor, "0px 12px 6px 12px", null, true, null, true);

    rangesContainer.onAction = function (actionName) {
        if (this.selectedItem) {
            rangeColor.setKey(this.selectedItem.itemObject.color);
            rangeStart.value = this.selectedItem.itemObject.start;
            rangeEnd.value = this.selectedItem.itemObject.end;
        }
        rangeStart.setEnabled(this.selectedItem);
        rangeEnd.setEnabled(this.selectedItem);
        rangeColor.setEnabled(this.selectedItem);
    }

    rangesContainer.onRemove = function (itemIndex) {
        form.sendCommand({ command: "RemoveGaugeRange", rangeIndex: itemIndex },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
            }
        );
    }

    form.setDataMode = function (dataMode) {
        form.dataMode = dataMode;
        controlsTable.style.display = valueContainer.style.display = dataMode == "UsingDataFields" ? "" : "none";
        dataManuallyTable.style.display = dataMode == "UsingDataFields" ? "none" : "";
    }

    form.setValues = function () {
        expressionControl.textBox.value = "";
        expressionControl.setEnabled(false);
        if (typesTable.buttons[this.gaugeProperties.type]) {
            typesTable.buttons[this.gaugeProperties.type].select();
        }
        calculationMode.setKey(this.gaugeProperties.calculationMode);
        minControl.setValue(this.gaugeProperties.minimum);
        maxControl.setValue(this.gaugeProperties.maximum);
        rangeType.setKey(this.gaugeProperties.rangeType);
        rangeMode.setKey(this.gaugeProperties.rangeMode);
        dataManuallyControl.setValue(this.gaugeProperties.manuallyEnteredData);
        form.setDataMode(this.gaugeProperties.dataMode);

        var meters = this.gaugeProperties.meters;

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

        var ranges = this.gaugeProperties.ranges;
        var selectedIndex = rangesContainer.getSelectedItemIndex();

        rangesContainer.clear();
        for (var i = 0; i < ranges.length; i++) {
            rangesContainer.addItem(ranges[i]);
        }

        var selectedItem = rangesContainer.getItemByIndex(selectedIndex);

        if (selectedItem) {
            selectedItem.select();
        }
        else if (rangesContainer.getCountItems() > 0) {
            rangesContainer.childNodes[0].select();
        }
    }

    form.switchOffStartMode = function () {
        form.ignoreStartMode = true;
        parentValueContainer.appendChild(valueContainer);
        valueContainer.style.height = "30px";
        valueContainer.style.width = "auto";
        valueContainer.style.margin = "0";
        if (!form.controls.valueDataColumn.dataColumnObject) form.controls.valueDataColumn.clear();
        controlsTable2.style.display = "";
    }

    form.checkStartMode = function () {
        var itemsCount = 0;
        for (var i = 0; i < dataColumns.length; i++) {
            var container = form.controls[dataColumns[i][0] + "DataColumn"];
            if (container.dataColumnObject) itemsCount++;
        }

        if (itemsCount == 0 && !form.ignoreStartMode && form.dataMode == "UsingDataFields") {
            form.container.appendChild(valueContainer);
            controlsTable.style.display = controlsTable2.style.display = "none";
            valueContainer.style.height = valueContainer.style.maxHeight = "260px";
            valueContainer.style.width = "267px";
            valueContainer.style.margin = "6px 12px 6px 12px";
        }
        else {
            parentValueContainer.appendChild(valueContainer);
            controlsTable.style.display = form.dataMode == "UsingDataFields" ? "" : "none";
            controlsTable2.style.display = "";
            valueContainer.style.height = "30px";
            valueContainer.style.width = "auto";
            valueContainer.style.margin = "0";

            if (!form.controls.valueDataColumn.item) {
                form.controls.valueDataColumn.clear();
            }
        }
    }

    form.onshow = function () {
        form.ignoreStartMode = false;
        form.currentPanelName = jsObject.options.propertiesPanel.getCurrentPanelName();
        if (jsObject.options.showDictionary) jsObject.options.propertiesPanel.showContainer("Dictionary");
        expressionControl.textBox.value = "";
        expressionControl.setEnabled(false);
        rangeType.setKey("None");
        rangeMode.setKey("Percentage");
        calculationMode.setKey("Auto");
        form.updateControlsVisibleStates();
        rangesContainer.clear();
        dataManuallyControl.setValue(null);

        for (var i = 0; i < dataColumns.length; i++) {
            var container = this.controls[dataColumns[i][0] + "DataColumn"];
            container.clear();
        }

        form.setDataMode("UsingDataFields");
        form.checkStartMode();

        form.sendCommand({ command: "GetGaugeElementProperties" },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.checkStartMode();
                form.correctTopPosition();
                for (var i = 0; i < dataColumns.length; i++) {
                    var container = form.controls[dataColumns[i][0] + "DataColumn"];
                    if (container.item) {
                        container.item.action();
                        break;
                    }
                }
            }
        );
    }

    form.onhide = function () {
        jsObject.options.propertiesPanel.showContainer(form.currentPanelName);
    }

    form.updateControls = function (gaugeProperties) {
        if (!gaugeProperties) return;
        form.gaugeProperties = gaugeProperties;
        form.setValues();
        form.updateControlsVisibleStates();
    }

    form.updateControlsVisibleStates = function () {
        minMaxTable.style.display = calculationMode.key == "Custom" ? "" : "none";
        rangesSeparator.style.display = rangeType.key == "Color" ? "" : "none";
        rangeTable.style.display = rangeType.key == "Color" ? "" : "none";
        rangeValuesTable.style.display = rangeType.key == "Color" ? "" : "none";
        rangeColor.style.display = rangeType.key == "Color" ? "" : "none";
    }

    form.applyExpressionPropertyToGaugeElement = function (container, expressionValue) {
        if (container) {
            form.sendCommand(
                {
                    command: "SetExpression",
                    containerName: jsObject.UpperFirstChar(container.name),
                    expressionValue: StiBase64.encode(expressionValue)
                },
                function (answer) {
                    form.updateControls(answer.elementProperties);
                    form.updateSvgContent(answer.elementProperties.svgContent);
                }
            );
        }
    }

    form.applyDataColumnPropertyToGaugeElement = function (container) {
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
                form.correctTopPosition();
            }
        );
    }

    form.applyPropertiesToGaugeElement = function (propertyName, propertyValue) {
        form.sendCommand({ command: "SetPropertyValue", propertyName: propertyName, propertyValue: propertyValue },
            function (answer) {
                jsObject.RemoveStylesFromCache(answer.elementProperties.name);
                form.updateControls(answer.elementProperties);
                form.updateElementProperties(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
                form.correctTopPosition();
            }
        );
    }

    form.applyPropertiesToGaugeRange = function (propertyName, propertyValue, rangeIndex) {
        form.sendCommand({ command: "SetPropertyValueToGaugeRange", propertyName: propertyName, propertyValue: propertyValue, rangeIndex: rangeIndex },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
            }
        );
    }

    form.sendCommand = function (updateParameters, callbackFunction) {
        setTimeout(function () {
            form.jsObject.SendCommandToDesignerServer("UpdateGaugeElement",
                {
                    componentName: form.currentGaugeElement.properties.name,
                    updateParameters: updateParameters
                },
                function (answer) {
                    callbackFunction(answer);
                });
        }, 100);
    }

    form.updateSvgContent = function (svgContent) {
        this.currentGaugeElement.properties.svgContent = svgContent;
        this.currentGaugeElement.repaint();
    }

    form.updateElementProperties = function (properties) {
        for (var propertyName in properties) {
            this.currentGaugeElement.properties[propertyName] = properties[propertyName];
        }
        jsObject.options.homePanel.updateControls();
        if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
    }

    return form;
}

StiMobileDesigner.prototype.GaugeRangesContainer = function (form) {
    var container = this.DataContainer(null, null, true);
    container.multiItems = true;
    container.style.minHeight = "30px";
    container.style.maxHeight = "150px";
    container.style.marginTop = "6px";

    container.addItem = function (rangeObject) {
        var item = this.jsObject.GaugeRangesContainerItem(rangeObject, this);
        this.appendChild(item);
        this.updateHintText();

        return item;
    }

    container.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        this.selectedItem = null;
        this.hintText = null;
        this.onAction("clear");
        this.updateHintText();
    }

    container.updateHintText = function () {
        if (this.childNodes.length == 0 && !this.hintText) {
            this.hintText = document.createElement("div");
            this.hintText.className = "wizardFormHintText";
            this.hintText.innerHTML = this.jsObject.loc.Dashboard.NoRanges;
            this.appendChild(this.hintText);
            this.hintText.style.width = "100%";
            this.hintText.style.textAlign = "center";
            this.hintText.style.top = "calc(50% - 9px)";
            this.style.borderStyle = "dashed";
        }
        else {
            if (this.hintText && this.childNodes.length > 1) {
                this.removeChild(this.hintText);
                this.hintText = null;
            }
            this.style.borderStyle = "solid";
        }
    }

    return container;
}

StiMobileDesigner.prototype.GaugeRangesContainerItem = function (rangeObject, container) {
    var text = rangeObject.start + " - " + rangeObject.end;
    var item = this.DataContainerItem(text, "Empty16.png", rangeObject, container);

    var imageCell = item.image.parentElement;
    imageCell.removeChild(item.image);

    var colorBar = document.createElement("div");
    colorBar.style.margin = "0 5px 0 5px";
    colorBar.style.width = colorBar.style.height = "16px";
    colorBar.style.background = this.GetHTMLColor(rangeObject.color);
    item.colorBar = colorBar;
    imageCell.appendChild(colorBar);

    return item;
}