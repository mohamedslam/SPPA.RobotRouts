
StiMobileDesigner.prototype.ChartSeriesTypeControl = function (name, width, isDashboard) {
    var jsObject = this;
    var control = this.CreateHTMLTable();
    control.isEnabled = true;

    var button = this.SmallButton(null, null, "Chart", "Charts.Big.ClusteredColumn.png", null, "Down", "stiDesignerFormButton", true, { width: 32, height: 32 });
    if (width) button.style.width = width + "px";
    button.style.height = "38px";
    button.innerTable.style.width = "100%";
    button.imageCell.style.width = "1px";
    button.arrowCell.style.width = "1px";
    control.addCell(button);

    var capBlock = document.createElement("div");
    capBlock.setAttribute("style", "text-overflow: ellipsis; overflow: hidden; white-space: nowrap;");
    if (width) capBlock.style.width = (width - (this.options.isTouchDevice ? 50 : 45)) + "px";
    button.caption.innerHTML = "";
    button.caption.appendChild(capBlock);

    var allItems = {
        mainMenu: this.GetAddSeriesItems(),
        clusteredColumn: this.GetChartClusteredColumnItems(),
        line: this.GetChartLineItems(),
        pie: this.GetChartPieItems(),
        clusteredBar: this.GetChartClusteredBarItems(),
        area: this.GetChartAreaItems(),
        range: this.GetChartRangeItems(),
        scatter: this.GetChartScatterItems(),
        radar: this.GetChartRadarItems(),
        funnel: this.GetChartFunnelItems(),
        financial: this.GetChartFinancialItems(),
        treemap: this.GetChartTreemapItems(),
        sunburst: this.GetChartSunburstItems(),
        histogram: this.GetChartHistogramItems(),
        boxAndWhisker: this.GetChartBoxAndWhiskerItems(),
        waterfall: this.GetChartWaterfallItems(),
        pictorial: this.GetChartPictorialItems()
    };

    var menu = this.VerticalMenu(name + "Menu", button, "Down", allItems.mainMenu);
    menu.firstChild.style.maxHeight = "1000px";

    this.InitializeSubMenu(name + "ClusteredColumnMenu", allItems.clusteredColumn, menu.items["ClusteredColumn"], menu);
    this.InitializeSubMenu(name + "LineMenu", allItems.line, menu.items["Line"], menu);
    this.InitializeSubMenu(name + "PieMenu", allItems.pie, menu.items["Pie"], menu);
    this.InitializeSubMenu(name + "ClusteredBarMenu", allItems.clusteredBar, menu.items["ClusteredBar"], menu);
    this.InitializeSubMenu(name + "AreaMenu", allItems.area, menu.items["Area"], menu);
    this.InitializeSubMenu(name + "RangeMenu", allItems.range, menu.items["Range"], menu);
    this.InitializeSubMenu(name + "ScatterMenu", allItems.scatter, menu.items["Scatter"], menu);
    this.InitializeSubMenu(name + "RadarMenu", allItems.radar, menu.items["Radar"], menu);
    this.InitializeSubMenu(name + "FunnelMenu", allItems.funnel, menu.items["Funnel"], menu);
    this.InitializeSubMenu(name + "FinancialMenu", allItems.financial, menu.items["Financial"], menu);
    this.InitializeSubMenu(name + "TreemapMenu", allItems.treemap, menu.items["Treemap"], menu);
    this.InitializeSubMenu(name + "SunburstMenu", allItems.sunburst, menu.items["Sunburst"], menu);
    this.InitializeSubMenu(name + "HistogramMenu", allItems.histogram, menu.items["Histogram"], menu);
    this.InitializeSubMenu(name + "BoxAndWhiskerMenu", allItems.boxAndWhisker, menu.items["BoxAndWhisker"], menu);
    this.InitializeSubMenu(name + "WaterfallMenu", allItems.waterfall, menu.items["Waterfall"], menu);
    this.InitializeSubMenu(name + "PictorialMenu", allItems.pictorial, menu.items["Pictorial"], menu);

    button.action = function () {
        menu.changeVisibleState(!menu.visible);
    }

    control.convertSeriesTypeToSeriesClassName = function (seriesType) {
        if (jsObject.EndsWith(seriesType, "3D")) {
            return ("Sti" + seriesType.substring(0, seriesType.length - 2) + "Series3D")
        }
        else {
            return ("Sti" + seriesType + "Series")
        }
    }

    control.action = function () { }

    control.setKey = function (key) {
        this.key = key;
        var seriesClass = this.convertSeriesTypeToSeriesClassName(key);

        for (var items in allItems) {
            if (allItems[items].length) {
                for (var i = 0; i < allItems[items].length; i++) {
                    if (seriesClass == allItems[items][i].key) {
                        var imageName = allItems[items][i].imageName.replace(".Small.", ".Big.");
                        StiMobileDesigner.setImageSource(button.image, this.jsObject.options, imageName);
                        capBlock.innerText = allItems[items][i].caption;
                        capBlock.setAttribute("title", capBlock.innerText);
                    }
                }
            }
        }
    }

    control.setEnabled = function (state) {
        control.isEnabled = state;
        button.setEnabled(state);
    }

    menu.action = function (menuItem) {
        if (menuItem.haveSubMenu) return;
        menu.changeVisibleState(false);
        control.setKey(menuItem.key.replace("Sti", "").replace("Series", ""));
        control.action();
    }

    return control;
}