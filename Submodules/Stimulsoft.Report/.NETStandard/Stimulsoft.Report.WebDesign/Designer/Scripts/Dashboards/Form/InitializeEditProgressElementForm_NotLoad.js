
StiMobileDesigner.prototype.InitializeEditProgressElementForm_ = function () {
    var form = this.DashboardBaseForm("editProgressElementForm", this.loc.Components.StiProgress, 1, this.HelpLinks["progressElement"]);
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
    form.container.appendChild(dataManuallyTable);

    var dataModeUsingFieldsButton = this.FormImageButton(null, "RetrieveColumnsArrow.png", jsObject.loc.Report.UseDataFields);
    dataModeUsingFieldsButton.style.display = "inline-block";
    dataManuallyTable.addCell(dataModeUsingFieldsButton).style.textAlign = "right";

    dataModeUsingFieldsButton.action = function () {
        form.switchOffStartMode();
        form.setDataMode("UsingDataFields");
        form.applyPropertyValueToProgressElement("DataMode", form.dataMode);
    }

    var dataManuallyControl = this.InitializeManualDataControl(311, 260, [["Value", this.loc.PropertyMain.Value], ["Target", this.loc.PropertyMain.Target], ["Series", this.loc.Chart.Series]], 10);
    dataManuallyTable.addCellInNextRow(dataManuallyControl).setAttribute("colspan", "2");

    dataManuallyControl.action = function () {
        var value = this.getValue();

        form.sendCommand({ command: "SetValueToManuallyEnteredData", propertyValue: value != null ? JSON.stringify(value) : null },
            function (answer) {
                form.updateElementProperties(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
            }
        );
    }

    form.container.appendChild(dataManuallyTable);
    var controlsTable = this.CreateHTMLTable();
    form.container.appendChild(controlsTable);
    var controlsTable2 = this.CreateHTMLTable();
    form.container.appendChild(controlsTable2);

    var expressionControl = this.ExpressionControlWithMenu(null, 305, null, null);
    var progressExpressionMenu = this.options.menus.progressExpressionMenu || this.InitializeDataColumnExpressionMenu("progressExpressionMenu", expressionControl, form);
    expressionControl.menu = progressExpressionMenu;
    progressExpressionMenu.parentButton = expressionControl.button;
    expressionControl.textBox.setAttribute("placeholder", jsObject.loc.PropertyMain.Field);

    expressionControl.action = function () {
        if (this.currentContainer) {
            form.applyExpressionPropertyToProgressElement(this.currentContainer, this.textBox.value);
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
        var container = this.DashboardDataColumnContainer(form, dataColumns[i][0], dataColumns[i][1], null, null, progressExpressionMenu, dataColumns[i][0] == "value");
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
            form.applyDataColumnPropertyToProgressElement(this);
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
        form.applyPropertyValueToProgressElement("DataMode", form.dataMode);
    }

    form.controls.valueDataColumn.innerTable.addCell(dataModeManuallyButton).style.textAlign = "right";
    parentValueContainer.setAttribute("colspan", "2");

    form.controls.valueDataColumn.actionEnterManuallyData = function () {
        form.switchOffStartMode();
        form.setDataMode("ManuallyEnteringData");
        form.applyPropertyValueToProgressElement("DataMode", "ManuallyEnteringData");
    }

    form.controls.valueDataColumn.actionShowMore = function () {
        form.switchOffStartMode();
        form.setDataMode("UsingDataFields");
        form.applyPropertyValueToProgressElement("DataMode", "UsingDataFields");
    }

    form.addControlRow(controlsTable, this.loc.PropertyMain.Expression, "expressionControlCaption", null, null, "12px 12px 6px 12px");
    form.addControlRow(controlsTable, null, "expression", expressionControl, "6px 12px 6px 12px");

    //Mode Types
    var modeTypes = ["Circle", "Pie", "DataBars"];
    var modeTypesTable = this.CreateHTMLTable();
    modeTypesTable.buttons = {};

    for (var i = 0; i < modeTypes.length; i++) {
        var button = this.FormButtonWithThemeBorder(null, null, null, "Dashboards.Progress." + modeTypes[i] + ".png", this.loc.PropertyEnum["StiProgressElementMode" + modeTypes[i]]);
        button.modeType = modeTypes[i];
        button.style.marginRight = "6px";
        modeTypesTable.addCell(button);
        modeTypesTable.buttons[modeTypes[i]] = button;

        button.action = function () {
            this.select();
            form.applyPropertyValueToProgressElement("Mode", this.modeType);
        }

        button.select = function () {
            for (var name in modeTypesTable.buttons) {
                modeTypesTable.buttons[name].setSelected(false);
            }
            this.setSelected(true);
        }
    }
    form.addControlRow(controlsTable2, this.loc.PropertyMain.Mode, "modeTypesTable", modeTypesTable, "6px 12px 6px 40px");

    form.setDataMode = function (dataMode) {
        form.dataMode = dataMode;
        controlsTable.style.display = valueContainer.style.display = dataMode == "UsingDataFields" ? "" : "none";
        dataManuallyTable.style.display = dataMode == "UsingDataFields" ? "none" : "";
    }

    form.setValues = function () {
        expressionControl.textBox.value = "";
        expressionControl.setEnabled(false);
        dataManuallyControl.setValue(this.progressProperties.manuallyEnteredData);
        form.setDataMode(this.progressProperties.dataMode);

        if (modeTypesTable.buttons[this.progressProperties.mode]) {
            modeTypesTable.buttons[this.progressProperties.mode].select();
        }

        var meters = this.progressProperties.meters;

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

        var valueContainer = form.controls.valueDataColumn.innerContainer;

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
        dataManuallyControl.setValue(null);

        for (var i = 0; i < dataColumns.length; i++) {
            var container = this.controls[dataColumns[i][0] + "DataColumn"];
            container.clear();
        }

        form.setDataMode("UsingDataFields");
        form.checkStartMode();

        form.sendCommand({ command: "GetProgressElementProperties" },
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

    form.updateControls = function (progressElement) {
        form.progressProperties = progressElement;
        form.setValues();
    }

    form.applyPropertyValueToProgressElement = function (propertyName, propertyValue) {
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
            }
        );
    }

    form.applyExpressionPropertyToProgressElement = function (container, expressionValue) {
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

    form.applyDataColumnPropertyToProgressElement = function (container) {
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

    form.sendCommand = function (updateParameters, callbackFunction) {
        jsObject.SendCommandToDesignerServer("UpdateProgressElement",
            {
                componentName: form.currentProgressElement.properties.name,
                updateParameters: updateParameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    form.updateSvgContent = function (svgContent) {
        this.currentProgressElement.properties.svgContent = svgContent;
        this.currentProgressElement.repaint();
    }

    form.updateElementProperties = function (properties) {
        for (var propertyName in properties) {
            this.currentProgressElement.properties[propertyName] = properties[propertyName];
        }
        jsObject.options.homePanel.updateControls();
        if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
    }

    return form;
}