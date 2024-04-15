
StiMobileDesigner.prototype.InitializeEditChartElementForm_ = function () {
    var jsObject = this;
    var form = this.DashboardBaseForm("editChartElementForm", this.loc.Components.StiChart, 1, this.HelpLinks["chartElement"]);
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.container.style.padding = "0 0 6px 0";
    form.caption.style.padding = "0px 10px 0 12px";
    form.hideButtonsPanel();

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
        form.updateControlsVisibleStates();
        form.applyPropertyValueToChartElement("DataMode", form.dataMode, function (answer) {
            form.chartElementProperties = answer.elementProperties;
            form.setValues();
            form.updateControlsVisibleStates();
            form.correctTopPosition();
        });
    }

    var manuallDataColuns = [
        ["Value", this.loc.PropertyMain.Values],
        ["EndValue", this.loc.PropertyMain.EndValues],
        ["CloseValue", this.loc.PropertyMain.CloseValues],
        ["LowValue", this.loc.PropertyMain.LowValues],
        ["HighValue", this.loc.PropertyMain.HighValues],
        ["Argument", this.loc.PropertyMain.Arguments],
        ["Weight", this.loc.PropertyMain.Weights],
        ["Series", this.loc.PropertyMain.Series]
    ];

    var dataManuallyControl = this.InitializeManualDataControl(306, 290, manuallDataColuns, 10);
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
    form.container.appendChild(controlsTable);

    var controlsTable2 = this.CreateHTMLTable();
    controlsTable2.style.width = "330px";
    form.container.appendChild(controlsTable2);

    var dataColumns = [
        ["values", this.loc.PropertyMain.Values],
        ["endValues", this.loc.PropertyMain.EndValues],
        ["closeValues", this.loc.PropertyMain.CloseValues],
        ["lowValues", this.loc.PropertyMain.LowValues],
        ["highValues", this.loc.PropertyMain.HighValues],
        ["arguments", this.loc.PropertyMain.Arguments],
        ["weights", this.loc.PropertyMain.Weights],
        ["series", this.loc.PropertyMain.Series]
    ];

    var expressionControl = this.ExpressionControlWithMenu(null, 300, null, null);
    var expressionMenu = this.options.menus.chartElementExpressionMenu || this.InitializeDataColumnsExpressionMenu("chartElementExpressionMenu", expressionControl, form);
    expressionControl.textBox.setAttribute("placeholder", jsObject.loc.PropertyMain.Field);

    for (var i = 0; i < dataColumns.length; i++) {
        var containerName = dataColumns[i][0];
        var containerBlock = this.ChartDataColumnsBlock(form, expressionMenu, containerName, dataColumns[i][1], containerName != "series", dataColumns, containerName == "values", containerName == "values");
        containerBlock.container.maxWidth = 315;
        containerBlock.style.width = "calc(100% - 24px)";
        form.addControlRow(controlsTable, null, containerName + "Block", containerBlock, containerName == "series" ? "0px 12px 12px 12px" : "0px 12px 0px 12px");
    }

    var valuesContainer = form.controls.valuesBlock.container;
    var parentValuesContainer = valuesContainer.parentElement;

    var dataModeManuallyButton = this.FormImageButton(null, "EditTable.png", this.loc.Report.EnterDataManually);
    dataModeManuallyButton.style.display = "inline-block";

    dataModeManuallyButton.action = function () {
        form.switchOffStartMode();
        form.setDataMode("ManuallyEnteringData");
        form.updateControlsVisibleStates();
        form.applyPropertyValueToChartElement("DataMode", form.dataMode, function (answer) {
            form.chartElementProperties = answer.elementProperties;
            form.setValues();
            form.updateControlsVisibleStates();
            form.correctTopPosition();
        });
    }

    form.controls.valuesBlock.addCell(dataModeManuallyButton).style.textAlign = "right";
    parentValuesContainer.setAttribute("colspan", "2");

    valuesContainer.actionEnterManuallyData = function () {
        form.switchOffStartMode();
        form.setDataMode("ManuallyEnteringData");
        form.updateControlsVisibleStates();
        form.applyPropertyValueToChartElement("DataMode", "ManuallyEnteringData", function (answer) {
            form.chartElementProperties = answer.elementProperties;
            form.setValues();
            form.updateControlsVisibleStates();
        });
    }

    valuesContainer.actionShowMore = function () {
        form.switchOffStartMode();
        form.setDataMode("UsingDataFields");
        form.updateControlsVisibleStates();
        form.applyPropertyValueToChartElement("DataMode", "UsingDataFields", function (answer) {
            form.chartElementProperties = answer.elementProperties;
            form.setValues();
            form.updateControlsVisibleStates();
        });
    }

    form.addControlRow(controlsTable, this.loc.PropertyMain.Expression, "expressionControlCaption", null, null, "6px 12px 6px 12px");
    form.addControlRow(controlsTable, null, "expression", expressionControl, "6px 12px 6px 12px");
    expressionControl.style.display = "inline-block";
    expressionControl.parentNode.style.textAlign = "right";
    expressionControl.menu = expressionMenu;
    expressionMenu.parentButton = expressionControl.button;

    expressionControl.action = function () {
        var selectedItem = form.getSelectedItem();
        if (selectedItem) {
            var containerName = selectedItem.container.name;
            var itemIndex = selectedItem.container.getItemIndex(selectedItem);
            form.applyExpressionPropertyToChartElement(containerName, itemIndex, expressionControl.textBox.value);
        }
    }

    expressionControl.refreshExpressionHint = function () {
        var selectedItem = form.getSelectedItem();
        expressionControl.textBox.setAttribute("placeholder", selectedItem
            ? (selectedItem.container.name == "series" ||
                selectedItem.container.name == "arguments" ||
                selectedItem.container.name == "weights" ? jsObject.loc.PropertyMain.Field : "Sum(" + jsObject.loc.PropertyMain.Field + ")")
            : "");
    }

    var seriesTypeControl = this.ChartSeriesTypeControl("chartElementFormSeriesType", null, true);
    seriesTypeControl.style.width = "calc(100% - 24px)";
    form.addControlRow(controlsTable2, null, "seriesType", seriesTypeControl, "6px 12px 6px 12px");

    var iconControl = this.IconControl("chartElementFormIcon", 80, null, null, null, true);
    form.addControlRow(controlsTable2, this.loc.PropertyMain.Icon, "icon", iconControl, "6px 12px 6px 12px", null, true, null, true);

    iconControl.action = function () {
        form.applyPropertyValueToChartElement("Icon", this.key);
        form.controls.roundValues.setEnabled(this.key != null);
    }

    var roundValuesControl = this.DropDownList("chartElementFormRoundValues", 153, null, this.GetBoolItems(), true, false, null, true);
    form.addControlRow(controlsTable2, this.loc.PropertyMain.RoundValues, "roundValues", roundValuesControl, "6px 12px 6px 12px", null, true, null, true);

    roundValuesControl.action = function () {
        form.applyPropertyValueToChartElement("RoundValues", this.key == "True");
    }

    var moreOptionsButton = this.FormButton(null, null, this.loc.Buttons.MoreOptions);
    moreOptionsButton.caption.style.padding = "0 10px 0 10px";
    moreOptionsButton.isVisible = true;
    form.addControlRow(controlsTable2, null, "moreOptionsButton", moreOptionsButton, "6px 12px 6px 12px", null, null, true);

    moreOptionsButton.action = function () {
        this.isVisible = !this.isVisible;
        form.updateControlsVisibleStates();
        form.correctTopPosition();
    }

    var columnShapeControl = this.DropDownList("chartElementFormColumnShape", 153, null, this.GetColumnShapeItems(), true, false, null, true);
    form.addControlRow(controlsTable2, this.loc.PropertyMain.ColumnShape, "columnShape", columnShapeControl, "6px 12px 6px 12px", null, true, null, true);

    columnShapeControl.action = function () {
        form.applyPropertyValueToChartElement("ColumnShape", this.key);
    }

    var yAxisControl = this.DropDownList("chartElementFormYAxis", 153, null, this.GetYAxisItems(), true, false, null, true);
    form.addControlRow(controlsTable2, this.loc.PropertyMain.YAxis, "yAxis", yAxisControl, "6px 12px 6px 12px", null, true, null, true);

    yAxisControl.action = function () {
        form.applyPropertyValueToValueMeter("YAxis", this.key);
    }

    var lineStyleControl = this.DropDownList("chartElementFormLineStyle", 153, null, this.GetLineStyleItems(), true, true, null, true);
    form.addControlRow(controlsTable2, this.loc.PropertyMain.LineStyle, "lineStyle", lineStyleControl, "6px 12px 6px 12px", null, true, null, true);

    lineStyleControl.action = function () {
        form.applyPropertyValueToValueMeter("LineStyle", this.key);
    }

    var lineWidthControl = this.TextBox("chartElementFormLineWidth", 100);
    form.addControlRow(controlsTable2, this.loc.PropertyMain.LineWidth, "lineWidth", lineWidthControl, "6px 12px 6px 12px", null, true, null, true);

    lineWidthControl.action = function () {
        form.applyPropertyValueToValueMeter("LineWidth", this.value);
    }

    var showZerosControl = this.DropDownList("chartElementFormShowZeros", 153, null, this.GetShowZerosOrNullsItems(), true, false, null, true);
    form.addControlRow(controlsTable2, this.loc.PropertyMain.ShowZeros, "showZeros", showZerosControl, "6px 12px 6px 12px", null, true, null, true);

    showZerosControl.action = function () {
        form.applyPropertyValueToValueMeter("ShowZeros", this.key);
    }

    var showNullsControl = this.DropDownList("chartElementFormShowNulls", 153, null, this.GetShowZerosOrNullsItems(), true, false, null, true);
    form.addControlRow(controlsTable2, this.loc.PropertyMain.ShowNulls, "showNulls", showNullsControl, "6px 12px 6px 12px", null, true, null, true);

    showNullsControl.action = function () {
        form.applyPropertyValueToValueMeter("ShowNulls", this.key);
    }

    var userViewStatesControl = this.UserViewStatesControl(form, "chartElementFormUserViewStates", 153);
    form.addControlRow(controlsTable2, this.loc.QueryBuilder.Views, "userViewStates", userViewStatesControl, "6px 12px 6px 12px", null, true, null, true);

    userViewStatesControl.action = function () {
        form.applyPropertyValueToChartElement("UserViewState", this.key, function (answer) {
            form.chartElementProperties = answer.elementProperties;
            form.setValues();
        });
    }

    var lessOptionsButton = this.FormButton(null, null, this.loc.Buttons.LessOptions);
    lessOptionsButton.caption.style.padding = "0 10px 0 10px";
    form.addControlRow(controlsTable2, null, "lessOptionsButton", lessOptionsButton, "6px 12px 6px 12px", null, null, true, null, true);

    lessOptionsButton.action = function () {
        moreOptionsButton.isVisible = true;
        form.updateControlsVisibleStates();
        form.correctTopPosition();
    }

    seriesTypeControl.action = function () {
        var selectedItem = form.getSelectedItem();
        if (form.dataMode == "ManuallyEnteringData" || (selectedItem && selectedItem.container.name == "values")) {
            var itemIndex = selectedItem ? selectedItem.container.getItemIndex(selectedItem) : -1;
            var oldIsBubble = form.oldSeriesType != null && form.oldSeriesType == "Bubble";
            var oldIsGantt = form.oldSeriesType != null && form.oldSeriesType == "Gantt";
            var seriesType = this.key;
            form.oldSeriesType = seriesType;
            form.sendCommand(
                {
                    command: "SetSeriesType",
                    itemIndex: itemIndex,
                    seriesType: seriesType,
                    oldIsBubble: oldIsBubble,
                    oldIsGantt: oldIsGantt
                },
                function (answer) {
                    for (var i = dataColumns.length - 1; i >= 0; i--) {
                        var containerName = dataColumns[i][0];
                        form.controls[containerName + "Block"].container.updateMeters(answer.elementProperties.meters[containerName], containerName == "values" ? itemIndex : null);
                    }
                    jsObject.RemoveStylesFromCache(answer.elementProperties.name);
                    form.updateControlsVisibleStates();
                    form.checkStartMode();
                    form.updateElementProperties(answer.elementProperties);
                    form.updateSvgContent(answer.elementProperties.svgContent);
                    form.correctTopPosition();
                    iconControl.setKey(answer.elementProperties.icon);
                    columnShapeControl.setKey(answer.elementProperties.columnShape);
                    roundValuesControl.setKey(answer.elementProperties.roundValues ? "True" : "False");
                    roundValuesControl.setEnabled(answer.elementProperties.icon != null);
                }
            );
        }
    }

    form.switchOffStartMode = function () {
        form.ignoreStartMode = true;
        parentValuesContainer.appendChild(valuesContainer);
        valuesContainer.style.height = "auto";
        valuesContainer.style.width = "auto";
        valuesContainer.style.margin = "0";
        valuesContainer.style.maxHeight = "100px";
        valuesContainer.isMaximize = false;
        controlsTable2.style.display = "";
        valuesContainer.updateHintText();
    }

    form.checkStartMode = function () {
        var itemsCount = 0;

        for (var i = 0; i < dataColumns.length; i++) {
            var container = form.controls[dataColumns[i][0] + "Block"].container;
            itemsCount += container.getCountItems();
        }

        if (itemsCount == 0 && !form.ignoreStartMode && form.dataMode == "UsingDataFields") {
            form.container.appendChild(valuesContainer);
            controlsTable.style.display = controlsTable2.style.display = "none";
            valuesContainer.style.height = valuesContainer.style.maxHeight = "260px";
            valuesContainer.style.width = "273px";
            valuesContainer.style.margin = "6px 12px 6px 12px";
            valuesContainer.isMaximize = true;
        }
        else {
            parentValuesContainer.appendChild(valuesContainer);
            controlsTable.style.display = form.dataMode == "UsingDataFields" ? "" : "none";
            controlsTable2.style.display = "";
            valuesContainer.style.height = "auto";
            valuesContainer.style.width = "auto";
            valuesContainer.style.margin = "0";
            valuesContainer.style.maxHeight = "100px";
            valuesContainer.isMaximize = false;
        }
        valuesContainer.updateHintText();
    }

    form.setDataMode = function (dataMode) {
        form.dataMode = dataMode;
        controlsTable.style.display = valuesContainer.style.display = dataMode == "UsingDataFields" ? "" : "none";
        dataManuallyTable.style.display = dataMode == "UsingDataFields" ? "none" : "";
    }

    form.setValues = function () {
        form.setDataMode(this.chartElementProperties.dataMode);
        iconControl.setKey(this.chartElementProperties.icon);
        columnShapeControl.setKey(this.chartElementProperties.columnShape);
        roundValuesControl.setKey(this.chartElementProperties.roundValues ? "True" : "False");
        userViewStatesControl.setKey(this.chartElementProperties.selectedViewStateKey);
        roundValuesControl.setEnabled(this.chartElementProperties.icon != null);
        dataManuallyControl.setValue(this.chartElementProperties.manuallyEnteredData);

        var meters = this.chartElementProperties.meters;

        if (form.dataMode == "ManuallyEnteringData") {
            form.oldSeriesType = seriesTypeControl.key;
            seriesTypeControl.setKey(this.chartElementProperties.manuallyEnteredSeriesType || "ClusteredColumn");
        }
        else if (meters.values.length > 0) {
            form.oldSeriesType = meters.values[0].seriesType;
            seriesTypeControl.setKey(meters.values[0].seriesType);
        }

        for (var i = dataColumns.length - 1; i >= 0; i--) {
            var containerName = dataColumns[i][0];
            form.controls[containerName + "Block"].container.updateMeters(meters[containerName], containerName == "values" ? 0 : null);
        }
    }

    form.updateControlsVisibleStates = function () {
        var selectedItem = form.getSelectedItem();
        form.controls.valuesBlock.header.innerHTML = form.isBubbleSeriesType(seriesTypeControl.key) ? "Y" : form.isFinancialSeriesType(seriesTypeControl.key) ? jsObject.loc.PropertyMain.OpenValues : jsObject.loc.PropertyMain.Values;
        form.controls.argumentsBlock.header.innerHTML = form.isBubbleSeriesType(seriesTypeControl.key) ? "X" : jsObject.loc.PropertyMain.Arguments;
        form.controls.weightsBlock.style.display = form.isBubbleSeriesType(seriesTypeControl.key) ? "" : "none";
        form.controls.endValuesBlock.style.display = form.isRangeSeriesType(seriesTypeControl.key) ? "" : "none";
        form.controls.closeValuesBlock.style.display = form.controls.lowValuesBlock.style.display = form.controls.highValuesBlock.style.display = form.isFinancialSeriesType(seriesTypeControl.key) ? "" : "none";

        if (form.dataMode == "ManuallyEnteringData") {
            var columnsParams = {};
            for (var i = 0; i < manuallDataColuns.length; i++) {
                var columnName = manuallDataColuns[i][0];
                columnsParams[columnName] = {
                    caption: manuallDataColuns[i][1],
                    visible: true
                }
                if (columnName == "Value") columnsParams[columnName].caption = form.controls.valuesBlock.header.innerHTML;
                if (columnName == "Argument") columnsParams[columnName].caption = form.controls.argumentsBlock.header.innerHTML;
                if (columnName == "Weight") columnsParams[columnName].visible = form.isBubbleSeriesType(seriesTypeControl.key);
                if (columnName == "EndValue") columnsParams[columnName].visible = form.isRangeSeriesType(seriesTypeControl.key);
                if (columnName == "CloseValue" || columnName == "LowValue" || columnName == "HighValue") columnsParams[columnName].visible = form.isFinancialSeriesType(seriesTypeControl.key);
            }
            dataManuallyControl.updateColumnsVisibility(columnsParams);
        }

        var isValue = selectedItem && selectedItem.container.name == "values";
        var showViewStates = form.dataMode == "UsingDataFields";
        var showLine = isValue && selectedItem.itemObject.lineWidth != null && form.dataMode == "UsingDataFields";
        var showZeros = isValue && selectedItem.itemObject.showZeros != null && form.dataMode == "UsingDataFields";
        var showNulls = isValue && selectedItem.itemObject.showNulls != null && form.dataMode == "UsingDataFields";
        var showYAxis = isValue && selectedItem.itemObject.yAxis != null && form.dataMode == "UsingDataFields";
        var showColumnShape = form.isAxisAreaChart3D(seriesTypeControl.key);
        var showIcon = form.isUsedNullableIcon(seriesTypeControl.key);
        var showMoreProps = showLine || showZeros || showNulls || showYAxis || showViewStates || showIcon || showColumnShape;

        form.controls.moreOptionsButtonRow.style.display = form.controls.moreOptionsButton.isVisible && showMoreProps ? "" : "none";
        form.controls.lessOptionsButtonRow.style.display = !form.controls.moreOptionsButton.isVisible && showMoreProps ? "" : "none";
        form.controls.lineWidthRow.style.display = form.controls.lineStyleRow.style.display = showLine && !form.controls.moreOptionsButton.isVisible ? "" : "none";
        form.controls.showZerosRow.style.display = showZeros && !form.controls.moreOptionsButton.isVisible ? "" : "none";
        form.controls.showNullsRow.style.display = showNulls && !form.controls.moreOptionsButton.isVisible ? "" : "none";
        form.controls.yAxisRow.style.display = showYAxis && !form.controls.moreOptionsButton.isVisible ? "" : "none";
        form.controls.columnShapeRow.style.display = showColumnShape && !form.controls.moreOptionsButton.isVisible ? "" : "none";
        form.controls.iconRow.style.display = form.isUsedIcon(seriesTypeControl.key) || (showIcon && !form.controls.moreOptionsButton.isVisible) ? "" : "none";
        form.controls.roundValuesRow.style.display = (form.isUsedIcon(seriesTypeControl.key) || (showIcon && !form.controls.moreOptionsButton.isVisible)) && seriesTypeControl.key != "PictorialStacked" ? "" : "none";
        form.controls.userViewStatesRow.style.display = !form.controls.moreOptionsButton.isVisible && showViewStates ? "" : "none";
    }

    form.onshow = function () {
        form.ignoreStartMode = false;
        form.currentPanelName = jsObject.options.propertiesPanel.getCurrentPanelName();
        if (jsObject.options.showDictionary) jsObject.options.propertiesPanel.showContainer("Dictionary");
        form.oldSeriesType = null;

        expressionControl.setEnabled(false);
        expressionControl.textBox.value = "";
        seriesTypeControl.setEnabled(false);
        seriesTypeControl.setKey("ClusteredColumn");
        iconControl.setKey(null);        
        columnShapeControl.setKey("Box");
        roundValuesControl.setKey("True");
        roundValuesControl.setEnabled(false);

        for (var i = 0; i < dataColumns.length; i++) {
            form.controls[dataColumns[i][0] + "Block"].container.clear();
        }

        form.updateControlsVisibleStates();
        form.setDataMode("UsingDataFields");
        form.checkStartMode();

        form.sendCommand({ command: "GetChartElementProperties" },
            function (answer) {
                form.chartElementProperties = answer.elementProperties;
                form.setValues();
                form.updateControlsVisibleStates();
                form.checkStartMode();
                form.correctTopPosition();
            }
        );
    }

    form.onhide = function () {
        var propertiesPanel = jsObject.options.propertiesPanel;
        if (propertiesPanel.editDbsMeterMode) {
            propertiesPanel.setEditDbsMeterMode(false);
            propertiesPanel.updateControls();
        }
        propertiesPanel.showContainer(form.currentPanelName);
    }

    form.setBubbleMode = function (state) {
        form.controls.weightsBlock.style.display = state ? "" : "none";
        form.controls.valuesBlock.header.innerHTML = state ? "X" : jsObject.loc.PropertyMain.Values;
        form.controls.argumentsBlock.header.innerHTML = state ? "Y" : jsObject.loc.PropertyMain.Arguments;
    }

    form.isBubbleSeriesType = function (seriesType) {
        return (seriesType == "Bubble");
    }

    form.isRangeSeriesType = function (seriesType) {
        return (seriesType == "Range" || seriesType == "RangeBar" || seriesType == "SplineRange" || seriesType == "SteppedRange" || seriesType == "Gantt");
    }

    form.isFinancialSeriesType = function (seriesType) {
        return (seriesType == "Candlestick" || seriesType == "Stock");
    }

    form.isAxisAreaChart3D = function (seriesType) {
        return (seriesType == "ClusteredColumn3D" || seriesType == "StackedColumn3D" || seriesType == "FullStackedColumn3D");
    }

    form.isUsedNullableIcon = function (seriesType) {
        return (seriesType == "Pie" || seriesType == "ClusteredColumn" || seriesType == "StackedColumn" || seriesType == "FullStackedColumn" || seriesType == "ClusteredBar" ||
            seriesType == "StackedBar" || seriesType == "FullStackedBar" || seriesType == "Bubble" || seriesType == "Histogram" || seriesType == "Pareto" || seriesType == "Doughnut" ||
            seriesType == "Funnel" || seriesType == "FunnelWeightedSlices" || seriesType == "Gantt" || seriesType == "RangeBar" || seriesType == "Treemap" || seriesType == "Waterfall");
    }

    form.isUsedIcon = function (seriesType) {
        return (seriesType == "Pictorial" || seriesType == "PictorialStacked");
    }

    form.getSelectedItem = function () {
        for (var i = 0; i < dataColumns.length; i++) {
            var container = form.controls[dataColumns[i][0] + "Block"].container;
            if (container.selectedItem) {
                return container.selectedItem;
            }
        }
        return null;
    }

    form.applyExpressionPropertyToChartElement = function (containerName, itemIndex, expressionValue) {
        form.sendCommand(
            {
                command: "SetExpression",
                itemIndex: itemIndex,
                containerName: containerName,
                expressionValue: StiBase64.encode(expressionValue)
            },
            function (answer) {
                var container = form.controls[containerName + "Block"].container;
                container.updateMeters(answer.elementProperties.meters[containerName], itemIndex);
                form.updateSvgContent(answer.elementProperties.svgContent);
            }
        );
    }

    form.sendCommand = function (updateParameters, callbackFunction) {
        jsObject.SendCommandToDesignerServer("UpdateChartElement",
            {
                componentName: form.currentChartElement.properties.name,
                updateParameters: updateParameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    form.updateSvgContent = function (svgContent) {
        this.currentChartElement.properties.svgContent = svgContent;
        this.currentChartElement.repaint();
    }

    form.updateElementProperties = function (properties) {
        for (var propertyName in properties) {
            this.currentChartElement.properties[propertyName] = properties[propertyName];
        }
        jsObject.options.homePanel.updateControls();
        if (jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.editDbsMeterMode) {
            jsObject.options.propertiesPanel.updateControls();
        }
    }

    form.applyPropertyValueToChartElement = function (propertyName, propertyValue, callbackFunc) {
        form.sendCommand(
            {
                command: "SetPropertyValue",
                propertyName: propertyName,
                propertyValue: propertyValue
            },
            function (answer) {
                form.updateElementProperties(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
                if (callbackFunc) callbackFunc(answer);
            }
        );
    }

    form.applyPropertyValueToValueMeter = function (propertyName, propertyValue) {
        var selectedItem = form.getSelectedItem();
        if (selectedItem) {
            var containerName = selectedItem.container.name;
            var itemIndex = selectedItem.container.getItemIndex(selectedItem);
            if (containerName == "values") {
                form.sendCommand(
                    {
                        command: "SetPropertyValueToValueMeter",
                        propertyName: propertyName,
                        propertyValue: propertyValue,
                        itemIndex: itemIndex
                    },
                    function (answer) {
                        form.updateElementProperties(answer.elementProperties);
                        form.controls[containerName + "Block"].container.updateMeters(answer.elementProperties.meters[containerName], itemIndex);
                        form.updateSvgContent(answer.elementProperties.svgContent);
                    }
                );
            }
        }
    }

    return form;
}


StiMobileDesigner.prototype.ChartDataColumnsBlock = function (form, contextMenu, containerName, headerText, multiItems, dataColumns, showItemImage, allowManuallyData) {
    var jsObject = this;
    var block = this.DashboardDataColumnsBlock(form, contextMenu, containerName, headerText, multiItems, showItemImage, allowManuallyData);

    block.container.onAction = function (actionName) {
        var selectedItem = null;

        if (actionName == "rename") {
            selectedItem = form.getSelectedItem();
            if (selectedItem) {
                var containerName = selectedItem.container.name;
                var itemIndex = selectedItem.container.getItemIndex(selectedItem);
                form.sendCommand({
                    command: "RenameMeter",
                    itemIndex: itemIndex,
                    containerName: containerName,
                    newLabel: selectedItem.itemObject.label
                },
                    function (answer) {
                        var container = form.controls[containerName + "Block"].container;
                        container.updateMeters(answer.elementProperties.meters[containerName], itemIndex);
                        form.updateSvgContent(answer.elementProperties.svgContent);
                    }
                );
            }
            return;
        }

        for (var i = 0; i < dataColumns.length; i++) {
            var container = form.controls[dataColumns[i][0] + "Block"].container;
            if (actionName != "clear" && this != container && container.selectedItem) {
                container.selectedItem.setSelected(false);
                container.selectedItem = null;
            }
            if (container.selectedItem) {
                selectedItem = container.selectedItem;
            }
        }

        form.controls.expression.setEnabled(selectedItem != null);
        form.controls.expression.textBox.value = selectedItem != null ? StiBase64.decode(selectedItem.itemObject.expression) : "";
        form.controls.expression.refreshExpressionHint();
        form.controls.seriesType.setEnabled((selectedItem != null && selectedItem.itemObject.seriesType) || form.dataMode == "ManuallyEnteringData");

        if (selectedItem != null) {
            form.controls.lineWidth.value = selectedItem.itemObject.lineWidth;
            form.controls.lineStyle.setKey(selectedItem.itemObject.lineStyle);

            var showZerosOrNullsItems = selectedItem.itemObject.showEmptyValuesInSimpleWay ? jsObject.GetShowZerosOrNullsInSimpleWayItems() : jsObject.GetShowZerosOrNullsItems()

            if (selectedItem.itemObject.showZeros) {
                form.controls.showZeros.addItems(showZerosOrNullsItems);
                form.controls.showZeros.setKey(selectedItem.itemObject.showZeros);
            }

            if (selectedItem.itemObject.showNulls) {
                form.controls.showNulls.addItems(showZerosOrNullsItems);
                form.controls.showNulls.setKey(selectedItem.itemObject.showNulls);
            }

            if (selectedItem.itemObject.yAxis) {
                form.controls.yAxis.setKey(selectedItem.itemObject.yAxis);
            }
        }

        if (selectedItem != null && selectedItem.itemObject.seriesType && form.dataMode == "UsingDataFields") {
            form.controls.seriesType.setKey(selectedItem.itemObject.seriesType);
        }

        form.updateControlsVisibleStates();

        var propertiesPanel = jsObject.options.propertiesPanel;

        if (actionName == "select" && selectedItem) {
            propertiesPanel.setEditDbsMeterMode(true);
            propertiesPanel.editDbsMeterPropertiesPanel.updateProperties(form, selectedItem);
        }
        else if (actionName == "remove" && propertiesPanel.editDbsMeterMode && this.getCountItems() == 0) {
            propertiesPanel.setEditDbsMeterMode(false);
        }
        if (actionName == "select" || actionName == "remove") {
            form.setDataMode("UsingDataFields");
            form.switchOffStartMode();
        }
    }

    return block;
}

StiMobileDesigner.prototype.UserViewStatesControl = function (form, name, width) {
    var jsObject = this;
    var control = this.DropDownList(name, width, null, [], false, false, null, false, null, true);

    control.button.action = function () {
        if (!control.menu.visible) {
            var userViewStates = form.chartElementProperties.userViewStates;
            var menuItems = [];
            menuItems.push(jsObject.Item("new", jsObject.loc.Gui.colorpickerdialog_newcolorlabel, null, "new"));
            menuItems.push(jsObject.Item("duplicate", jsObject.loc.Buttons.Duplicate, null, "duplicate"));

            if (userViewStates.length > 1) {
                menuItems.push("separator");
                for (var i = 0; i < userViewStates.length; i++) {
                    menuItems.push(jsObject.Item("item" + i, userViewStates[i].name, null, userViewStates[i].key));
                }
            }

            control.addItems(menuItems);

            control.menu.items.new.setEnabled(userViewStates.length < 5);
            control.menu.items.duplicate.setEnabled(userViewStates.length < 5);

            for (var itemName in control.menu.items) {
                var item = control.menu.items[itemName];
                if (item.setSelected) {
                    item.setSelected(control.key == item.key)
                }
            }
            control.menu.changeVisibleState(true);
        }
        else {
            control.menu.changeVisibleState(false);
        }
    }

    control.menu.action = function (item) {
        form.sendCommand({
            command: "SetUserViewState",
            viewStateKey: item.key,
            seriesType: form.controls.seriesType.key,
            newItemName: jsObject.loc.PropertyCategory.ViewCategory
        },
            function (answer) {
                jsObject.RemoveStylesFromCache(answer.elementProperties.name);
                form.chartElementProperties = answer.elementProperties;
                form.updateElementProperties(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
                form.setValues();
                form.updateControlsVisibleStates();
                form.checkStartMode();
                form.correctTopPosition();
            }
        );
        this.changeVisibleState(false);
    }

    StiMobileDesigner.setImageSource(control.editButton.image, this.options, "SmallCross.png");
    control.editButton.image.style.width = control.editButton.image.style.height = "8px";

    control.editButton.action = function () {
        form.sendCommand({ command: "RemoveUserViewState", viewStateKey: control.key },
            function (answer) {
                jsObject.RemoveStylesFromCache(answer.elementProperties.name);
                form.chartElementProperties = answer.elementProperties;
                form.updateElementProperties(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
                form.setValues();
                form.updateControlsVisibleStates();
                form.checkStartMode();
                form.correctTopPosition();
            }
        );
    }

    control.setKey = function (key) {
        this.key = key;
        var userViewStates = form.chartElementProperties.userViewStates;
        var enableViewState = userViewStates.length > 1;

        if (key && enableViewState) {
            for (var i = 0; i < userViewStates.length; i++) {
                if (userViewStates[i].key == key) {
                    this.textBox.value = userViewStates[i].name;
                    break;
                }
            }

        }
        else {
            this.textBox.value = "[" + jsObject.loc.FormStyleDesigner.NotSpecified + "]";
        }

        control.editButton.setEnabled(enableViewState);
        control.readOnly = control.textBox.readOnly = !enableViewState;
    }

    return control;
}