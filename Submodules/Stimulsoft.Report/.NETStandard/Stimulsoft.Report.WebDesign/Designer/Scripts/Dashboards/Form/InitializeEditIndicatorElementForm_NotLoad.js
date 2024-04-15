
StiMobileDesigner.prototype.InitializeEditIndicatorElementForm_ = function () {
    var form = this.DashboardBaseForm("editIndicatorElementForm", this.loc.Components.StiIndicator, 1, this.HelpLinks["indicatorElement"]);
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
        form.applyPropertyValueToIndicatorElement("DataMode", form.dataMode);
    }

    var dataManuallyControl = this.InitializeManualDataControl(311, 260, [["Value", this.loc.PropertyMain.Value], ["Target", this.loc.PropertyMain.Target], ["Series", this.loc.Chart.Series]], 10);
    dataManuallyTable.addCellInNextRow(dataManuallyControl).setAttribute("colspan", "2");
    form.container.appendChild(dataManuallyTable);

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

    var controlsTable = this.CreateHTMLTable();
    form.container.appendChild(controlsTable);

    var controlsTable2 = this.CreateHTMLTable();
    controlsTable2.style.width = "100%";
    form.container.appendChild(controlsTable2);

    var expressionControl = this.ExpressionControlWithMenu(null, 305, null, null);
    var indicatorExpressionMenu = this.options.menus.indicatorExpressionMenu || this.InitializeDataColumnExpressionMenu("indicatorExpressionMenu", expressionControl, form);
    expressionControl.menu = indicatorExpressionMenu;
    indicatorExpressionMenu.parentButton = expressionControl.button;
    expressionControl.textBox.setAttribute("placeholder", jsObject.loc.PropertyMain.Field);

    expressionControl.action = function () {
        if (this.currentContainer) {
            form.applyExpressionPropertyToIndicatorElement(this.currentContainer, this.textBox.value);
        }
    }

    expressionControl.refreshExpressionHint = function () {
        expressionControl.textBox.setAttribute("placeholder", this.currentContainer ? (this.currentContainer.name == "series" ? jsObject.loc.PropertyMain.Field : "Sum(" + jsObject.loc.PropertyMain.Field + ")") : "");
    }

    //DataColumns
    var dataColumns = [
        ["value", this.loc.PropertyMain.Value],
        ["target", this.loc.PropertyMain.Target],
        ["series", this.loc.Chart.Series]
    ];

    for (var i = 0; i < dataColumns.length; i++) {
        var container = this.DashboardDataColumnContainer(form, dataColumns[i][0], dataColumns[i][1], null, null, indicatorExpressionMenu, dataColumns[i][0] == "value");
        container.allowSelected = true;
        container.maxWidth = 300;
        form.addControlRow(controlsTable, null, dataColumns[i][0] + "DataColumn", container, "0px 12px " + (i == dataColumns.length - 1 ? "6px" : "0px" + " 12px"));

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
            form.applyDataColumnPropertyToIndicatorElement(this);
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
        form.applyPropertyValueToIndicatorElement("DataMode", form.dataMode);
    }

    form.controls.valueDataColumn.innerTable.addCell(dataModeManuallyButton).style.textAlign = "right";
    parentValueContainer.setAttribute("colspan", "2");

    form.controls.valueDataColumn.actionEnterManuallyData = function () {
        form.switchOffStartMode();
        form.setDataMode("ManuallyEnteringData");
        form.applyPropertyValueToIndicatorElement("DataMode", "ManuallyEnteringData");
    }

    form.controls.valueDataColumn.actionShowMore = function () {
        form.switchOffStartMode();
        form.setDataMode("UsingDataFields");
        form.applyPropertyValueToIndicatorElement("DataMode", "UsingDataFields");
    }

    form.addControlRow(controlsTable, this.loc.PropertyMain.Expression, "expressionControlCaption", null, null, "6px 12px 6px 12px");
    form.addControlRow(controlsTable, null, "expression", expressionControl, "6px 12px 6px 12px");

    //IconMode
    var iconMode = this.DropDownList("indicatorFormIconMode", 180, null, this.GetGaugeCalculationModeItems(), true, null, null, true);
    form.addControlRow(controlsTable2, this.loc.PropertyMain.Mode, "iconMode", iconMode, "6px 12px 6px 12px", null, true, true);

    iconMode.action = function () {
        form.applyPropertyValueToIndicatorElement("IconMode", this.key);
    }

    var rangeTable = this.CreateHTMLTable();
    form.addControlRow(controlsTable2, null, "rangeTable", rangeTable, "6px 0px 8px 0px");
    rangeTable.style.width = "100%";

    //IconRangeMode
    var iconRangeMode = this.DropDownList("indicatorFormIconRangeMode", 100, null, this.GetGaugeRangeModeItems(), true, null, null, true);
    iconRangeMode.style.marginLeft = "12px";
    rangeTable.addCell(iconRangeMode);

    iconRangeMode.action = function () {
        form.applyPropertyValueToIndicatorElement("IconRangeMode", this.key);
    }

    rangeTable.addCell().style.width = "100%";

    //AddRangeButton
    var addRangeButton = this.FormButton(null, null, this.loc.Dashboard.AddRange);
    addRangeButton.style.marginRight = "12px";
    addRangeButton.style.height = this.options.isTouchDevice ? "26px" : "21px";
    rangeTable.addCell(addRangeButton);

    //Icon Ranges Container
    var iconRangesContainer = this.IndicatorIconRangesContainer(form);
    iconRangesContainer.style.margin = "8px 12px 0 12px";
    rangeTable.addCellInNextRow(iconRangesContainer).setAttribute("colspan", "3");

    addRangeButton.action = function () {
        form.sendCommand({ command: "AddIndicatorIconRange" },
            function (answer) {
                form.updateControls(answer.elementProperties);
                if (iconRangesContainer.childNodes.length > 0) {
                    iconRangesContainer.childNodes[iconRangesContainer.childNodes.length - 1].select();
                }
                form.updateSvgContent(answer.elementProperties.svgContent);
            }
        );
    }

    //Icon Range Values
    var rangeValuesTable = this.CreateHTMLTable();
    rangeValuesTable.style.width = "calc(100% - 24px)";
    form.addControlRow(controlsTable2, null, "rangeValuesTable", rangeValuesTable, "0px 12px 12px 12px");

    var rangeIcon = this.IconControl("indicatorFormIconRange", 90);
    rangeIcon.style.display = "inline-block";
    rangeValuesTable.addCell(rangeIcon);
    rangeIcon.action = function () {
        if (iconRangesContainer.selectedItem) {
            form.applyPropertiesToIndicatorIconRange("Icon", this.key, iconRangesContainer.getSelectedItemIndex());
        }
    }

    var rangeStart = this.ExpressionControl(null, 90);
    rangeStart.style.display = "inline-block";
    rangeValuesTable.addCell(rangeStart).style.textAlign = "center";
    rangeStart.action = function () {
        if (iconRangesContainer.selectedItem) {
            form.applyPropertiesToIndicatorIconRange("StartExpression", this.textBox.value, iconRangesContainer.getSelectedItemIndex());
        }
    }

    var rangeEnd = this.ExpressionControl(null, 90);
    rangeEnd.style.display = "inline-block";
    rangeValuesTable.addCell(rangeEnd).style.textAlign = "right";
    rangeEnd.action = function () {
        if (iconRangesContainer.selectedItem) {
            form.applyPropertiesToIndicatorIconRange("EndExpression", this.textBox.value, iconRangesContainer.getSelectedItemIndex());
        }
    }

    iconRangesContainer.onAction = function (actionName) {
        rangeIcon.setKey(this.selectedItem ? this.selectedItem.itemObject.icon : null);
        rangeStart.textBox.value = this.selectedItem ? this.selectedItem.itemObject.startExpression : "";
        rangeEnd.textBox.value = this.selectedItem ? this.selectedItem.itemObject.endExpression : "";
        rangeIcon.setEnabled(this.selectedItem);
        rangeStart.setEnabled(this.selectedItem);
        rangeEnd.setEnabled(this.selectedItem);
    }

    iconRangesContainer.onRemove = function (itemIndex) {
        form.sendCommand({ command: "RemoveIndicatorIconRange", rangeIndex: itemIndex },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
            }
        );
    }

    //Icon
    var customIconControl = this.ImageControl(null, 184, 70);
    customIconControl.style.borderStyle = "solid";
    customIconControl.style.display = "inline-block";
    form.addControlRow(controlsTable2, this.loc.PropertyMain.Icon, "customIcon", customIconControl, "6px 12px 6px 12px", null, true, true);

    customIconControl.action = function () {
        form.applyPropertyValueToIndicatorElement("CustomIcon", this.src);
    }

    var iconControl = this.IconControl("indicatorFormIcon", 80, null, null, customIconControl);
    form.addControlRow(controlsTable2, this.loc.PropertyMain.Icon, "icon", iconControl, "6px 12px 6px 12px", null, true);

    iconControl.action = function () {
        form.applyPropertyValueToIndicatorElement("Icon", this.key);
    }

    //IconSet
    var iconSetControl = this.DbsIconSetControl("indicatorFormIconSet", 180);
    form.addControlRow(controlsTable2, this.loc.PropertyMain.IconSet, "iconSet", iconSetControl, "6px 12px 6px 12px", null, true, true);

    iconSetControl.action = function () {
        form.applyPropertyValueToIndicatorElement("IconSet", this.key);
    }

    form.setDataMode = function (dataMode) {
        form.dataMode = dataMode;
        controlsTable.style.display = valueContainer.style.display = dataMode == "UsingDataFields" ? "" : "none";
        dataManuallyTable.style.display = dataMode == "UsingDataFields" ? "none" : "";
    }

    form.setValues = function () {
        expressionControl.textBox.value = "";
        expressionControl.setEnabled(false);
        iconControl.setKey(this.indicatorProperties.icon);
        iconSetControl.setKey(this.indicatorProperties.iconSet);
        customIconControl.setImage(this.indicatorProperties.customIcon);
        iconMode.setKey(this.indicatorProperties.iconMode);
        iconRangeMode.setKey(this.indicatorProperties.iconRangeMode);
        dataManuallyControl.setValue(this.indicatorProperties.manuallyEnteredData);
        form.setDataMode(this.indicatorProperties.dataMode);

        var meters = this.indicatorProperties.meters;

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

        var iconRanges = this.indicatorProperties.iconRanges;
        var selectedIndex = iconRangesContainer.getSelectedItemIndex();

        iconRangesContainer.clear();
        for (var i = 0; i < iconRanges.length; i++) {
            iconRangesContainer.addItem(iconRanges[i]);
        }

        var selectedItem = iconRangesContainer.getItemByIndex(selectedIndex);

        if (selectedItem) {
            selectedItem.select();
        }
        else if (iconRangesContainer.getCountItems() > 0) {
            iconRangesContainer.childNodes[0].select();
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
        customIconControl.setImage(null);
        iconMode.setKey("Auto");
        iconRangeMode.setKey("Percentage");
        iconRangesContainer.clear();
        dataManuallyControl.setValue(null);
        form.controls.iconRow.style.display = form.controls.iconSetRow.style.display = form.controls.customIconRow.style.display = form.controls.iconModeRow.style.display =
            form.controls.rangeTableRow.style.display = form.controls.rangeValuesTableRow.style.display = "none";

        for (var i = 0; i < dataColumns.length; i++) {
            var container = this.controls[dataColumns[i][0] + "DataColumn"];
            container.clear();
        }

        form.setDataMode("UsingDataFields");
        form.checkStartMode();

        form.sendCommand({ command: "GetIndicatorElementProperties" },
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

    form.updateControls = function (indicatorProperties) {
        form.indicatorProperties = indicatorProperties;
        form.setValues();
        form.updateControlsVisibleStates();
    }

    form.updateControlsVisibleStates = function () {
        var valueIsPresent = form.controls.valueDataColumn.dataColumnObject != null;
        var seriesIsPresent = form.controls.seriesDataColumn.dataColumnObject != null;
        var targetIsPresent = form.controls.targetDataColumn.dataColumnObject != null;

        if (form.dataMode == "ManuallyEnteringData") {
            valueIsPresent = seriesIsPresent = targetIsPresent = false;
            var data = dataManuallyControl.getValue();
            if (data && data.length > 1) {
                for (var i = 1; i < data.length; i++) {
                    if (data[i][0] != "") valueIsPresent = true;
                    if (data[i][1] != "") targetIsPresent = true;
                    if (data[i][2] != "") seriesIsPresent = true;
                }
            }
        }

        var showIconSet = valueIsPresent && seriesIsPresent && !targetIsPresent;
        form.controls.iconRow.style.display = valueIsPresent && !seriesIsPresent && !form.indicatorProperties.customIcon ? "" : "none";
        form.controls.iconSetRow.style.display = showIconSet && form.controls.iconMode.key == "Auto" ? "" : "none";
        iconControl.textBox.style.color = iconSetControl.textBox.style.color = form.indicatorProperties.realGlyphColor == "255,255,255" ? "#c6c6c6" : jsObject.GetHTMLColor(form.indicatorProperties.realGlyphColor);
        form.controls.customIconRow.style.display = form.controls.iconSetRow.style.display == "none" && form.indicatorProperties.customIcon ? "" : "none";
        form.controls.iconModeRow.style.display = showIconSet ? "" : "none";
        form.controls.rangeTableRow.style.display = form.controls.rangeValuesTableRow.style.display = showIconSet && form.controls.iconMode.key == "Custom" ? "" : "none";
        form.controls.rangeValuesTableRow.style.display = showIconSet && form.controls.iconMode.key == "Custom" && iconRangesContainer.getCountItems() > 0 ? "" : "none";
    }

    form.applyPropertyValueToIndicatorElement = function (propertyName, propertyValue) {
        form.sendCommand(
            {
                command: "SetPropertyValue",
                propertyName: propertyName,
                propertyValue: propertyValue
            },
            function (answer) {
                jsObject.RemoveStylesFromCache(answer.elementProperties.name);
                form.updateControls(answer.elementProperties);
                form.updateElementProperties(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
                form.correctTopPosition();
            }
        );
    }

    form.applyExpressionPropertyToIndicatorElement = function (container, expressionValue) {
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

    form.applyDataColumnPropertyToIndicatorElement = function (container) {
        form.sendCommand(
            {
                command: "SetDataColumn",
                containerName: jsObject.UpperFirstChar(container.name),
                dataColumnObject: container.dataColumnObject
            },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
                form.updateElementProperties(answer.elementProperties);
                if (container.item) container.item.action();
                form.checkStartMode();
                form.correctTopPosition();
            }
        );
    }

    form.applyPropertiesToIndicatorIconRange = function (propertyName, propertyValue, rangeIndex) {
        form.sendCommand({ command: "SetPropertyValueToIndicatorIconRange", propertyName: propertyName, propertyValue: propertyValue, rangeIndex: rangeIndex },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
            }
        );
    }

    form.sendCommand = function (updateParameters, callbackFunction) {
        jsObject.SendCommandToDesignerServer("UpdateIndicatorElement",
            {
                componentName: form.currentIndicatorElement.properties.name,
                updateParameters: updateParameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    form.updateSvgContent = function (svgContent) {
        this.currentIndicatorElement.properties.svgContent = svgContent;
        this.currentIndicatorElement.repaint();
    }

    form.updateElementProperties = function (properties) {
        for (var propertyName in properties) {
            this.currentIndicatorElement.properties[propertyName] = properties[propertyName];
        }
        jsObject.options.homePanel.updateControls();
        if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
    }

    return form;
}

StiMobileDesigner.prototype.IndicatorIconRangesContainer = function (form) {
    var container = this.DataContainer(null, null, true);
    container.multiItems = true;
    container.style.minHeight = "30px";
    container.style.maxHeight = "150px";
    container.style.marginTop = "6px";

    container.addItem = function (rangeObject) {
        var item = this.jsObject.IndicatorIconRangesContainerItem(rangeObject, this);
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

StiMobileDesigner.prototype.IndicatorIconRangesContainerItem = function (rangeObject, container) {
    var text = rangeObject.startExpression + " - " + rangeObject.endExpression;
    var item = this.DataContainerItem(text, "Empty16.png", rangeObject, container);

    var imageCell = item.image.parentElement;
    imageCell.removeChild(item.image);

    imageCell.style.fontFamily = "Stimulsoft";
    imageCell.style.fontSize = "17px";
    imageCell.style.color = "#4472c4";
    imageCell.style.padding = "0 5px 0 5px";

    var fontIcons = this.options.fontIcons;
    for (var i = 0; i < fontIcons.length; i++) {
        var items = fontIcons[i].items;
        for (var k = 0; k < items.length; k++) {
            var iconKey = items[k].key;
            if (iconKey == rangeObject.icon) {
                imageCell.innerHTML = items[k].text;
                return item;
            }
        }
    }

    return item;
}