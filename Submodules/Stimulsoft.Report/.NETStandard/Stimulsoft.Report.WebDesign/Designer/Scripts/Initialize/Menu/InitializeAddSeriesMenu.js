
StiMobileDesigner.prototype.InitializeAddSeriesMenu = function (editChartForm) {
    var menu = this.VerticalMenu("addSeriesMenu", editChartForm.seriesToolBar.addSeries, "Down", this.GetAddSeriesItems())

    this.InitializeSubMenu("chartClusteredColumnMenu", this.GetChartClusteredColumnItems(), menu.items["ClusteredColumn"], menu);
    this.InitializeSubMenu("chartLineMenu", this.GetChartLineItems(), menu.items["Line"], menu);
    this.InitializeSubMenu("chartPieMenu", this.GetChartPieItems(), menu.items["Pie"], menu);
    this.InitializeSubMenu("chartClusteredBarMenu", this.GetChartClusteredBarItems(), menu.items["ClusteredBar"], menu);
    this.InitializeSubMenu("chartAreaMenu", this.GetChartAreaItems(), menu.items["Area"], menu);
    this.InitializeSubMenu("chartRangeMenu", this.GetChartRangeItems(), menu.items["Range"], menu);
    this.InitializeSubMenu("chartScatterMenu", this.GetChartScatterItems(), menu.items["Scatter"], menu);
    this.InitializeSubMenu("chartRadarMenu", this.GetChartRadarItems(), menu.items["Radar"], menu);
    this.InitializeSubMenu("chartFunnelMenu", this.GetChartFunnelItems(), menu.items["Funnel"], menu);
    this.InitializeSubMenu("chartFinancialMenu", this.GetChartFinancialItems(), menu.items["Financial"], menu);
    this.InitializeSubMenu("chartTreemapMenu", this.GetChartTreemapItems(), menu.items["Treemap"], menu);
    this.InitializeSubMenu("chartSunburstMenu", this.GetChartSunburstItems(), menu.items["Sunburst"], menu);
    this.InitializeSubMenu("chartHistogramMenu", this.GetChartHistogramItems(), menu.items["Histogram"], menu);
    this.InitializeSubMenu("chartBoxAndWhiskerMenu", this.GetChartBoxAndWhiskerItems(), menu.items["BoxAndWhisker"], menu);
    this.InitializeSubMenu("chartWaterfallMenu", this.GetChartWaterfallItems(), menu.items["Waterfall"], menu);
    this.InitializeSubMenu("chartPictorialMenu", this.GetChartPictorialItems(), menu.items["Pictorial"], menu);

    menu.update = function () {
        var types = editChartForm.chartProperties.typesCollection;

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

                if (editChartForm.seriesContainer.items.length == 0) {
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