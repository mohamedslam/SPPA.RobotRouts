
StiMobileDesigner.prototype.ChartsMenu = function (menuName, parentButton, isToolboxMenu) {
    var jsObject = this;
    var items = this.GetAddSeriesItems();

    var menu = isToolboxMenu
        ? this.HorizontalMenu(menuName, parentButton, "Right", items)
        : this.VerticalMenu(menuName, parentButton, "Down", items);

    menu.firstChild.style.maxHeight = "1000px";
    if (isToolboxMenu) menu.type = "Menu";

    var subMenus = [];
    subMenus.push(this.InitializeSubMenu(menuName + "ClusteredColumnMenu", this.GetChartClusteredColumnItems(), menu.items["ClusteredColumn"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "LineMenu", this.GetChartLineItems(), menu.items["Line"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "PieMenu", this.GetChartPieItems(), menu.items["Pie"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "ClusteredBarMenu", this.GetChartClusteredBarItems(), menu.items["ClusteredBar"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "AreaMenu", this.GetChartAreaItems(), menu.items["Area"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "RangeMenu", this.GetChartRangeItems(), menu.items["Range"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "ScatterMenu", this.GetChartScatterItems(), menu.items["Scatter"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "RadarMenu", this.GetChartRadarItems(), menu.items["Radar"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "FunnelMenu", this.GetChartFunnelItems(), menu.items["Funnel"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "FinancialMenu", this.GetChartFinancialItems(), menu.items["Financial"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "TreemapMenu", this.GetChartTreemapItems(), menu.items["Treemap"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "SunburstMenu", this.GetChartSunburstItems(), menu.items["Sunburst"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "HistogramMenu", this.GetChartHistogramItems(), menu.items["Histogram"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "BoxAndWhiskerMenu", this.GetChartBoxAndWhiskerItems(), menu.items["BoxAndWhisker"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "WaterfallMenu", this.GetChartWaterfallItems(), menu.items["Waterfall"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "PictorialMenu", this.GetChartPictorialItems(), menu.items["Pictorial"], menu));

    for (var i = 0; i < subMenus.length; i++) {
        for (var itemKey in subMenus[i].items) {
            var item = subMenus[i].items[itemKey];
            item.name = "Infographic;StiChart;" + item.key;
            this.AddDragEventsToComponentButton(item);
        }
    }

    menu.action = function (menuItem) {
        if (!menuItem.haveSubMenu) {
            menu.changeVisibleState(false);

            var panel = isToolboxMenu ? jsObject.options.toolbox : jsObject.options.insertPanel;
            if (panel) panel.resetChoose();

            jsObject.options.drawComponent = true;
            jsObject.options.paintPanel.setCopyStyleMode(false);
            jsObject.options.paintPanel.changeCursorType(true);

            if (panel) panel.selectedComponent = menuItem;
        }
    }

    return menu;
}
