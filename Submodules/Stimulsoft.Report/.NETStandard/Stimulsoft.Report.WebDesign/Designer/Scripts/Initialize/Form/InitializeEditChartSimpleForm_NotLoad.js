
StiMobileDesigner.prototype.InitializeEditChartSimpleForm_ = function () {
    var jsObject = this;
    var form = this.DashboardBaseForm("editChartSimpleForm", this.loc.Components.StiChart, 1, this.HelpLinks["chartElement"]);
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";

    var moreOptionsButton = this.FormButton(null, null, this.loc.Buttons.MoreOptions);
    moreOptionsButton.style.display = "inline-block";
    moreOptionsButton.style.margin = "12px";

    moreOptionsButton.action = function () {
        form.changeVisibleState(false);
        form.currentChartComponent.properties.editorType = "Advanced";
        jsObject.SendCommandSendProperties([form.currentChartComponent], ["editorType"]);
        jsObject.InitializeEditChartForm(function (editChartForm) {
            editChartForm.currentChartComponent = form.currentChartComponent;
            editChartForm.oldSvgContent = form.oldSvgContent;
            editChartForm.chartProperties = form.chartProperties;
            editChartForm.changeVisibleState(true);
        });
    }

    var footerTable = this.CreateHTMLTable();
    footerTable.style.width = "100%";
    var buttonsPanel = form.buttonsPanel;
    form.removeChild(buttonsPanel);
    form.appendChild(footerTable);
    footerTable.addCell(moreOptionsButton).style.width = "1px";
    footerTable.addCell();
    footerTable.addCell(form.buttonOk).style.width = "1px";
    footerTable.addCell(form.buttonCancel).style.width = "1px";

    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.width = "370px";
    form.container.appendChild(controlsTable);
    form.container.style.padding = "6px 0 6px 0";

    var dataColumns = [
        ["values", this.loc.PropertyMain.Values],
        ["endValues", this.loc.PropertyMain.EndValues],
        ["closeValues", this.loc.PropertyMain.CloseValues],
        ["lowValues", this.loc.PropertyMain.LowValues],
        ["highValues", this.loc.PropertyMain.HighValues],
        ["arguments", this.loc.PropertyMain.Arguments],
        ["weights", this.loc.PropertyMain.Weights]
    ];

    for (var i = 0; i < dataColumns.length; i++) {
        var containerName = dataColumns[i][0];
        var containerBlock = this.ChartSimpleFormDataColumnsBlock(form, containerName, dataColumns[i][1], dataColumns, containerName == "values");
        containerBlock.style.width = "calc(100% - 24px)";
        form.addControlRow(controlsTable, null, containerName + "Block", containerBlock, "0px 12px 0px 12px");
    }

    var seriesButton = this.FormImageButton(null, "SeriesEditor.png", this.loc.Chart.SeriesEditorForm);
    seriesButton.style.display = "inline-block";

    var dopCell = form.controls.valuesBlock.addCellInRow(0, seriesButton);
    dopCell.style.textAlign = "right";
    form.controls.valuesBlock.container.parentElement.setAttribute("colspan", "2");

    seriesButton.action = function () {
        form.changeVisibleState(false);
        var chartComponent = jsObject.options.selectedObject;
        if (chartComponent) {
            jsObject.InitializeEditChartSeriesForm(function (seriesForm) {
                seriesForm.currentChartComponent = chartComponent;
                seriesForm.oldSvgContent = chartComponent.properties.svgContent;
                jsObject.SendCommandStartEditChartComponent(chartComponent.properties.name, seriesForm.name);
            });
        }
    }

    var parentValuesContainer = form.controls.valuesBlock.container.parentElement;

    var seriesTypeControl = this.ChartSeriesTypeControl("chartSimpleFormSeriesType", null);
    seriesTypeControl.style.width = "calc(100% - 24px)";
    form.addControlRow(controlsTable, null, "seriesType", seriesTypeControl, "18px 12px 6px 12px", null, true, null, true);

    var fieldControl = this.PropertyDataControl("chartSimpleFormField", 250, true);
    fieldControl.textBox.setAttribute("placeholder", "[" + jsObject.loc.Dashboard.DataNotDefined + "]");
    form.addControlRow(controlsTable, this.loc.PropertyMain.Field, "fieldControl", fieldControl, "12px 12px 6px 12px", null, true, null, true);

    var iconControl = this.IconControl("chartSimpleFormIcon", 80);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Icon, "icon", iconControl, "6px 12px 6px 12px", null, true, null, true);

    seriesTypeControl.action = function () {
        jsObject.RemoveStylesFromCache(form.currentChartComponent.properties.name, "StiChart");
        form.setSeriesPropertyValue("SeriesType", this.key);
    }

    fieldControl.action = function () {
        form.setSeriesPropertyValue("DataColumn", StiBase64.encode(this.textBox.value));
    }

    iconControl.action = function () {
        form.setSeriesPropertyValue("Icon", this.key);
    }

    form.checkStartMode = function () {
        var itemsCount = 0;

        for (var i = 0; i < dataColumns.length; i++) {
            var container = form.controls[dataColumns[i][0] + "Block"].container;
            itemsCount += container.getCountItems();
        }

        var valuesContainer = form.controls.valuesBlock.container;

        if (itemsCount == 0) {
            form.container.appendChild(valuesContainer);
            controlsTable.style.display = "none";
            valuesContainer.style.height = valuesContainer.style.maxHeight = "260px";
            valuesContainer.style.width = "350px";
            valuesContainer.style.margin = "6px 12px 6px 12px";
        }
        else {
            parentValuesContainer.appendChild(valuesContainer);
            controlsTable.style.display = "";
            valuesContainer.style.height = "auto";
            valuesContainer.style.width = "auto";
            valuesContainer.style.margin = "0";
            valuesContainer.style.maxHeight = "100px";
        }
    }

    form.updateControlsValues = function (selectedContainerName, selectedIndex) {
        //reset all controls
        form.currentSeriesType = "ClusteredColumn";
        fieldControl.setEnabled(false);
        fieldControl.textBox.value = "";
        seriesTypeControl.setEnabled(false);
        seriesTypeControl.setKey(form.currentSeriesType);
        iconControl.setKey("QuarterFull");

        for (var i = 0; i < dataColumns.length; i++) {
            form.controls[dataColumns[i][0] + "Block"].container.clear();
        }

        //fill values
        var series = this.chartProperties.series;

        if (series.length > 0) {
            for (var i = dataColumns.length - 1; i >= 0; i--) {
                var containerName = dataColumns[i][0];
                form.controls[containerName + "Block"].container.updateItems(series);
            }

            form.currentSeriesType = form.seriesTypeToChartSeriesType(series[0].type);
            seriesTypeControl.setKey(form.currentSeriesType);
        }

        form.updateControlsVisibleStates();

        //select current item
        if (selectedContainerName) {
            var containerBlock = form.controls[selectedContainerName + "Block"];
            if (containerBlock.style.display == "none") {
                containerBlock = form.controls.valuesBlock;
                selectedIndex = 0;
            }

            var selectedContainer = containerBlock.container;
            var countItems = selectedContainer.getCountItems();

            if (selectedIndex != null && selectedIndex >= 0 && selectedIndex < countItems) {
                selectedContainer.childNodes[selectedIndex].select();
            }
            else if (countItems > 0) {
                selectedContainer.childNodes[countItems - 1].select();
            }
        }
    }

    form.updateSvgContent = function (svgContent) {
        this.currentChartComponent.properties.svgContent = svgContent;
        this.currentChartComponent.repaint();
    }

    form.updateControlsVisibleStates = function () {
        form.controls.valuesBlock.header.innerHTML = form.isBubbleSeriesType(seriesTypeControl.key) ? "Y"
            : form.isFinancialSeriesType(seriesTypeControl.key) ? jsObject.loc.PropertyMain.OpenValues : jsObject.loc.PropertyMain.Values;
        form.controls.argumentsBlock.header.innerHTML = form.isBubbleSeriesType(seriesTypeControl.key) ? "X" : jsObject.loc.PropertyMain.Arguments;
        form.controls.weightsBlock.style.display = form.isBubbleSeriesType(seriesTypeControl.key) ? "" : "none";
        form.controls.endValuesBlock.style.display = form.isRangeSeriesType(seriesTypeControl.key) ? "" : "none";
        form.controls.closeValuesBlock.style.display = form.controls.lowValuesBlock.style.display = form.controls.highValuesBlock.style.display =
            form.isFinancialSeriesType(seriesTypeControl.key) ? "" : "none";
        form.controls.iconRow.style.display = seriesTypeControl.key == "Pictorial" || seriesTypeControl.key == "PictorialStacked" ? "" : "none";
    }

    form.setBubbleMode = function (state) {
        form.controls.weightsBlock.style.display = state ? "" : "none";
        form.controls.valuesBlock.header.innerHTML = state ? "X" : this.jsObject.loc.PropertyMain.Values;
        form.controls.argumentsBlock.header.innerHTML = state ? "Y" : this.jsObject.loc.PropertyMain.Arguments;
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

    form.getSelectedItem = function () {
        for (var i = 0; i < dataColumns.length; i++) {
            var container = form.controls[dataColumns[i][0] + "Block"].container;
            if (container.selectedItem) {
                return container.selectedItem;
            }
        }
        return null;
    }

    form.seriesTypeToChartSeriesType = function (seriesType) {
        return seriesType.replace("Sti", "").replace("Series", "");
    }

    form.onshow = function () {
        form.currentPanelName = form.jsObject.options.propertiesPanel.getCurrentPanelName();
        jsObject.options.propertiesPanel.showContainer("Dictionary");
        moreOptionsButton.style.display = jsObject.options.designerSpecification == "Beginner" ? "none" : "inline-block";

        form.updateControlsValues("values", 0);
        form.checkStartMode();
        form.correctTopPosition();
    }

    form.onhide = function () {
        var propertiesPanel = jsObject.options.propertiesPanel;
        if (propertiesPanel && propertiesPanel.editChartSeriesMode) {
            propertiesPanel.setEditChartSeriesMode(false);
            propertiesPanel.showContainer(form.currentPanelName);
        }
    }

    form.cancelAction = function () {
        jsObject.SendCommandCanceledEditComponent(form.chartProperties.name);
        if (form.oldSvgContent) {
            form.currentChartComponent.properties.svgContent = form.oldSvgContent;
            form.currentChartComponent.repaint();
            jsObject.RemoveStylesFromCache(form.currentChartComponent.properties.name, "StiChart");
            jsObject.UpdatePropertiesControls();
        }
    }

    form.action = function () {
        this.changeVisibleState(false);
    }

    form.setSeriesPropertyValue = function (propertyName, propertyValue) {
        var selectedItem = this.getSelectedItem();
        if (selectedItem) {
            var containerName = selectedItem.container.name;
            var itemIndex = selectedItem.container.getItemIndex(selectedItem);

            var params = {
                innerCommand: "SetPropertyValue",
                componentName: form.chartProperties.name,
                containerName: containerName,
                itemIndex: itemIndex,
                propertyName: propertyName,
                propertyValue: propertyValue
            }

            jsObject.SendCommandToDesignerServer("UpdateChart", params,
                function (answer) {
                    form.chartProperties = answer.chartProperties;
                    jsObject.UpdatePropertiesControls();
                    form.updateControlsValues(containerName, itemIndex);
                    form.checkStartMode();
                    form.correctTopPosition();
                    form.updateSvgContent(answer.chartProperties.svgContent);
                });
        }
    }

    return form;
}

StiMobileDesigner.prototype.ChartSimpleFormDataColumnsBlock = function (form, containerName, headerText, dataColumns, showItemImage) {
    var block = this.DashboardDataColumnsBlock(form, null, containerName, headerText, true, showItemImage);

    if (block.header)
        block.header.style.padding = "12px 0 6px 0";

    var container = block.container;
    var jsObject = this;

    container.updateItems = function (series) {
        var containerName = this.name;
        var oldScrollTop = this.scrollTop;
        this.style.height = this.offsetHeight + "px";
        this.clear();
        
        for (var i = 0; i < series.length; i++) {
            var commonProps = series[i].properties.Common;
            var seriesType = form.seriesTypeToChartSeriesType(series[i].type);
            var dataColumn = null;

            if (containerName == "values") dataColumn = form.isFinancialSeriesType(seriesType) ? commonProps.ValueDataColumnOpen : commonProps.ValueDataColumn;
            else if (containerName == "endValues") dataColumn = commonProps.ValueDataColumnEnd;
            else if (containerName == "closeValues") dataColumn = commonProps.ValueDataColumnClose;
            else if (containerName == "lowValues") dataColumn = commonProps.ValueDataColumnLow;
            else if (containerName == "highValues") dataColumn = commonProps.ValueDataColumnHigh;
            else if (containerName == "weights") dataColumn = commonProps.WeightDataColumn;
            else if (containerName == "arguments") dataColumn = commonProps.ArgumentDataColumn;

            if (dataColumn) {
                dataColumn = jsObject.ExtractBase64Value(dataColumn);

                if (dataColumn || containerName != "arguments") {
                    var itemObject = {
                        typeItem: "Meter",
                        dataColumn: dataColumn,
                        seriesType: seriesType,
                        icon: commonProps.Icon
                    };
                    var imageName = null;

                    if (containerName == "values") {
                        imageName = "ChartSeries.Light." + seriesType + ".png";
                        
                        if (!StiMobileDesigner.checkImageSource(jsObject.options, imageName)) {
                            imageName = "ChartSeries.Light.ClusteredColumn.png";
                        }
                    }

                    this.addItem(itemObject.dataColumn || ("[" + jsObject.loc.Dashboard.DataNotDefined + "]"), imageName, itemObject);
                }
            }
        }

        this.scrollTop = oldScrollTop;
        this.style.height = "auto";
        this.style.paddingBottom = this.getCountItems() > 0 ? "30px" : "0px";
    }

    container.onmouseup = function (event) {
        if (event.button != 2 && jsObject.options.itemInDrag) {
            var itemObject = jsObject.CopyObject(jsObject.options.itemInDrag.originalItem.itemObject);
            if (!itemObject) return;
            var typeItem = itemObject.typeItem;
            var containerName = this.name;
            var seriesType = form.currentSeriesType || "ClusteredColumn";

            if (typeItem == "Meter") {
                var fromContainerName = jsObject.options.itemInDrag.originalItem.container.name;
                var fromContainer = form.controls[fromContainerName + "Block"].container;
                var fromIndex = fromContainer.getItemIndex(jsObject.options.itemInDrag.originalItem);
                var toIndex = this.getOverItemIndex();

                if (containerName != fromContainerName || (toIndex != null && fromIndex != null && fromIndex != toIndex)) {
                    var params = {
                        innerCommand: containerName != fromContainerName ? "MoveMeter" : (containerName == "arguments" ? "MoveArgument" : "MoveSeries"),
                        componentName: form.chartProperties.name,
                        toContainerName: containerName,
                        fromContainerName: fromContainerName,
                        toIndex: toIndex,
                        fromIndex: fromIndex,
                        seriesType: seriesType
                    }
                    jsObject.SendCommandToDesignerServer("UpdateChart", params,
                        function (answer) {
                            form.chartProperties = answer.chartProperties;
                            form.updateControlsValues(containerName, toIndex);
                            form.checkStartMode();
                            form.correctTopPosition();
                            form.updateSvgContent(answer.chartProperties.svgContent);
                        });
                }
            }
            else if (typeItem == "Column") {
                var params = {
                    innerCommand: "InsertDataColumn",
                    componentName: form.chartProperties.name,
                    containerName: containerName,
                    insertIndex: container.getOverItemIndex(),
                    draggedItem: { itemObject: itemObject }
                }

                var columnParent = jsObject.options.dictionaryTree.getCurrentColumnParent();
                if (columnParent) {
                    params.draggedItem.currentParentType = columnParent.type;
                    params.draggedItem.currentParentName = columnParent.name;
                }

                if (form.currentSeriesType) {
                    params.seriesType = form.currentSeriesType;
                }

                jsObject.SendCommandToDesignerServer("UpdateChart", params,
                    function (answer) {
                        form.chartProperties = answer.chartProperties;
                        form.updateControlsValues(params.containerName, params.insertIndex);
                        form.checkStartMode();
                        form.correctTopPosition();
                        form.updateSvgContent(answer.chartProperties.svgContent);
                    });
            }
        }

        return false;
    }

    container.onRemove = function (itemIndex) {
        var params = {
            innerCommand: "RemoveDataColumn",
            componentName: form.chartProperties.name,
            containerName: containerName,
            itemIndex: itemIndex
        }

        jsObject.SendCommandToDesignerServer("UpdateChart", params,
            function (answer) {
                form.chartProperties = answer.chartProperties;
                form.updateControlsValues(params.containerName, params.itemIndex);
                form.checkStartMode();
                form.correctTopPosition();
                form.updateSvgContent(answer.chartProperties.svgContent);
            });
    }

    container.onAction = function (actionName) {
        var selectedItem = null;

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

        var itemObject = selectedItem != null ? selectedItem.itemObject : null;
        form.controls.fieldControl.setEnabled(itemObject != null);
        form.controls.fieldControl.textBox.value = itemObject != null ? itemObject.dataColumn : "";
        form.controls.seriesType.setEnabled(itemObject != null && itemObject.seriesType);
        form.controls.seriesType.setKey(itemObject != null && itemObject.seriesType ? itemObject.seriesType : "ClusteredColumn");
        form.controls.icon.setKey(itemObject != null && itemObject.icon ? itemObject.icon : "QuarterFull");

        var propertiesPanel = jsObject.options.propertiesPanel;
        if (propertiesPanel) {
            if (actionName == "select") {
                var valuesContainer = form.controls.valuesBlock.container;
                var seriesIndex = valuesContainer.getSelectedItemIndex();
                var series = form.chartProperties.series;
                if (seriesIndex != null && seriesIndex < series.length) {
                    propertiesPanel.setEditChartSeriesMode(true);

                    if (valuesContainer.firstAction == null) valuesContainer.firstAction = true;
                    var chartPropsPanel = jsObject.options.propertiesPanel.editChartPropertiesPanel;
                    chartPropsPanel.showAllSeriesGroups(series[seriesIndex], valuesContainer.firstAction);
                    valuesContainer.firstAction = false;
                }
                else {
                    propertiesPanel.setEditChartSeriesMode(false);
                }
            }
            else if (actionName == "remove") {
                if (propertiesPanel.editChartSeriesMode)
                    propertiesPanel.setEditChartSeriesMode(false);
                if (propertiesPanel.editChartStripsMode)
                    propertiesPanel.setEditChartStripsMode(false);
            }
        }
    }

    return block;
}