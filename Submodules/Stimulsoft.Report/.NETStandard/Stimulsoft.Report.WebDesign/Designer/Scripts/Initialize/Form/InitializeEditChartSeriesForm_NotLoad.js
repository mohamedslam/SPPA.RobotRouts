
StiMobileDesigner.prototype.InitializeEditChartSeriesForm_ = function () {
    var jsObject = this;
    var form = this.DashboardBaseForm("editChartSeriesForm", this.loc.Chart.SeriesEditorForm, 1);
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";

    var addSeriesButton = this.SmallButton(null, null, this.loc.Chart.AddSeries.replace("&", ""), null, null, "Down", "stiDesignerFormButton");
    addSeriesButton.style.margin = "12px 12px 0 12px";
    addSeriesButton.style.display = "inline-block";
    form.container.appendChild(addSeriesButton);

    var seriesContainer = this.ChartSeriesContainer(form);
    seriesContainer.style.margin = "12px";
    form.seriesContainer = seriesContainer;
    form.container.appendChild(seriesContainer);

    var addSeriesMenu = this.InitializeAddSeriesMenu_SeriesForm(form, addSeriesButton, seriesContainer);
    addSeriesMenu.innerContent.style.maxHeight = null;
    addSeriesButton.action = function () { addSeriesMenu.changeVisibleState(!this.isSelected); }

    addSeriesMenu.action = function (menuItem) {
        var params = {
            innerCommand: "AddSeries",
            componentName: form.chartProperties.name,
            seriesType: menuItem.key
        }

        jsObject.SendCommandToDesignerServer("UpdateChart", params,
            function (answer) {
                form.chartProperties = answer.chartProperties;
                var series = form.chartProperties.series;
                if (series.length > 0) {
                    form.updateProperties(answer.chartProperties);
                    form.updateSvgContent(answer.chartProperties.svgContent);
                    jsObject.RemoveStylesFromCache(form.chartProperties.name, "StiChart");
                    jsObject.UpdatePropertiesControls();
                    seriesContainer.addSeries(series[series.length - 1]).select();
                }
            });
    }

    form.updateSvgContent = function (svgContent) {
        form.currentChartComponent.properties.svgContent = svgContent;
        form.currentChartComponent.repaint();
    }

    form.updateProperties = function (properties) {
        form.currentChartComponent.properties.chartSeries = properties.series;
    }

    form.onshow = function () {
        form.currentPanelName = jsObject.options.propertiesPanel.getCurrentPanelName();
        seriesContainer.fill(form.chartProperties.series, 0);
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
            jsObject.RemoveStylesFromCache(form.chartProperties.name, "StiChart");
            jsObject.UpdatePropertiesControls();
        }
    }

    form.action = function () {
        form.changeVisibleState(false);
    }

    return form;
}

StiMobileDesigner.prototype.InitializeAddSeriesMenu_SeriesForm = function (form, addSeriesButton, seriesContainer) {
    var menu = this.VerticalMenu("addSeriesMenu_SeriesForm", addSeriesButton, "Down", this.GetAddSeriesItems());

    this.InitializeSubMenu("chartClusteredColumnMenu_SeriesForm", this.GetChartClusteredColumnItems(), menu.items["ClusteredColumn"], menu);
    this.InitializeSubMenu("chartLineMenu_SeriesForm", this.GetChartLineItems(), menu.items["Line"], menu);
    this.InitializeSubMenu("chartPieMenu_SeriesForm", this.GetChartPieItems(), menu.items["Pie"], menu);
    this.InitializeSubMenu("chartClusteredBarMenu_SeriesForm", this.GetChartClusteredBarItems(), menu.items["ClusteredBar"], menu);
    this.InitializeSubMenu("chartAreaMenu_SeriesForm", this.GetChartAreaItems(), menu.items["Area"], menu);
    this.InitializeSubMenu("chartRangeMenu_SeriesForm", this.GetChartRangeItems(), menu.items["Range"], menu);
    this.InitializeSubMenu("chartScatterMenu_SeriesForm", this.GetChartScatterItems(), menu.items["Scatter"], menu);
    this.InitializeSubMenu("chartRadarMenu_SeriesForm", this.GetChartRadarItems(), menu.items["Radar"], menu);
    this.InitializeSubMenu("chartFunnelMenu_SeriesForm", this.GetChartFunnelItems(), menu.items["Funnel"], menu);
    this.InitializeSubMenu("chartFinancialMenu_SeriesForm", this.GetChartFinancialItems(), menu.items["Financial"], menu);
    this.InitializeSubMenu("chartTreemapMenu_SeriesForm", this.GetChartTreemapItems(), menu.items["Treemap"], menu);
    this.InitializeSubMenu("chartSunburstMenu_SeriesForm", this.GetChartSunburstItems(), menu.items["Sunburst"], menu);
    this.InitializeSubMenu("chartHistogramMenu_SeriesForm", this.GetChartHistogramItems(), menu.items["Histogram"], menu);
    this.InitializeSubMenu("chartBoxAndWhiskerMenu_SeriesForm", this.GetChartBoxAndWhiskerItems(), menu.items["BoxAndWhisker"], menu);
    this.InitializeSubMenu("chartWaterfallMenu_SeriesForm", this.GetChartWaterfallItems(), menu.items["Waterfall"], menu);
    this.InitializeSubMenu("chartPictorialMenu_SeriesForm", this.GetChartPictorialItems(), menu.items["Pictorial"], menu);

    menu.update = function () {
        var types = form.chartProperties.typesCollection;

        for (var mainItemName in menu.items) {
            if (mainItemName == "separator") continue;
            var mainItem = menu.items[mainItemName];
            var categoryEnabled = false;
            for (var subItemName in mainItem.menu.items) {
                if (subItemName == "separator") continue;
                var subItem = mainItem.menu.items[subItemName];
                var chartType = subItem.key;
                var finded = false;

                for (var i = 0; i < types.length; i++) {
                    var type = this.jsObject.options.isJava ? types[i].substring(types[i].lastIndexOf(".") + 1) : types[i];

                    if (type == chartType) {
                        finded = true;
                        categoryEnabled = true;
                        break;
                    }
                }

                if (seriesContainer.getCountItems() == 0) {
                    subItem.setEnabled(true);
                    categoryEnabled = true;
                }
                else
                    subItem.setEnabled(finded);

            }
            mainItem.setEnabled(categoryEnabled);
        }
    }

    menu.onshow = function () {
        menu.update();
    }

    return menu;
}

StiMobileDesigner.prototype.ChartSeriesContainer = function (form) {
    var jsObject = this;
    var container = this.DataContainer(null, null, true, " ");
    container.multiItems = true;
    container.style.height = "300px";
    container.style.width = "300px";

    container.seriesTypeToChartSeriesType = function (seriesType) {
        return seriesType.replace("Sti", "").replace("Series", "");
    }

    container.addSeries = function (seriesObject) {
        seriesObject.typeItem = "Series";
        var imageName = "ChartSeries.Light." + this.seriesTypeToChartSeriesType(seriesObject.type) + ".png";

        if (!StiMobileDesigner.checkImageSource(jsObject.options, imageName)) {
            imageName = "ChartSeries.Light.ClusteredColumn.png";
        }

        return this.addItem(seriesObject.name, imageName, jsObject.CopyObject(seriesObject));
    }

    container.fill = function (series, selectedIndex) {
        this.clear();
        for (var i = 0; i < series.length; i++) {
            this.addSeries(series[i]);
        }
        if (selectedIndex != null) {
            var item = this.getItemByIndex(selectedIndex);
            if (item) item.select();
        }
    }

    container.onmouseup = function (event) {
        if (event.button != 2 && jsObject.options.itemInDrag) {
            var itemObject = jsObject.CopyObject(jsObject.options.itemInDrag.originalItem.itemObject);
            if (!itemObject) return;
            var typeItem = itemObject.typeItem;

            if (typeItem == "Series") {
                var toIndex = this.getOverItemIndex();
                var fromIndex = this.getItemIndex(jsObject.options.itemInDrag.originalItem);
                if (toIndex != null && fromIndex != null && fromIndex != toIndex) {
                    var params = {
                        innerCommand: "MoveSeries",
                        componentName: form.chartProperties.name,
                        toIndex: toIndex,
                        fromIndex: fromIndex
                    }
                    jsObject.SendCommandToDesignerServer("UpdateChart", params,
                        function (answer) {
                            form.chartProperties = answer.chartProperties;
                            form.updateProperties(answer.chartProperties);
                            form.updateSvgContent(answer.chartProperties.svgContent);
                            container.fill(form.chartProperties.series, toIndex);
                        });
                }
            }
        }

        return false;
    }

    container.onRemove = function (itemIndex) {
        var params = {
            innerCommand: "RemoveSeries",
            componentName: form.chartProperties.name,
            seriesIndex: itemIndex
        }
        jsObject.SendCommandToDesignerServer("UpdateChart", params,
            function (answer) {
                form.chartProperties = answer.chartProperties;
                form.updateProperties(answer.chartProperties);
                form.updateSvgContent(answer.chartProperties.svgContent);
                jsObject.RemoveStylesFromCache(form.chartProperties.name, "StiChart");
                jsObject.UpdatePropertiesControls();
                if (container.selectedItem) container.selectedItem.select();
            });
    }

    container.oncontextmenu = function (event) {
        return false;
    }

    container.onAction = function (actionName) {
        var propertiesPanel = jsObject.options.propertiesPanel;
        if (propertiesPanel) {
            if (actionName == "select") {
                var seriesIndex = container.getSelectedItemIndex();
                var series = form.chartProperties.series;
                if (seriesIndex != null && seriesIndex < series.length) {
                    propertiesPanel.setEditChartSeriesMode(true);

                    if (container.firstAction == null) container.firstAction = true;
                    var chartPropsPanel = jsObject.options.propertiesPanel.editChartPropertiesPanel;
                    chartPropsPanel.showAllSeriesGroups(series[seriesIndex], container.firstAction);
                    container.firstAction = false;
                }
            }
            else if (actionName == "remove" && container.getCountItems() == 0 && propertiesPanel.editChartSeriesMode) {
                propertiesPanel.setEditChartSeriesMode(false);
            }
        }
    }

    container.onmouseover = function () { }
    container.onmouseout = function () { }

    return container;
}