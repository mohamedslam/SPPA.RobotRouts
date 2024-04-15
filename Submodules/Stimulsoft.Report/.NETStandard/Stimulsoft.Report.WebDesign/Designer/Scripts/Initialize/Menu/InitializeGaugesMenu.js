
StiMobileDesigner.prototype.GaugesMenu = function (menuName, parentButton, isToolboxMenu) {
    var jsObject = this;
    var items = this.GetGaugeTypesItems();

    var menu = isToolboxMenu
        ? this.HorizontalMenu(menuName, parentButton, "Right", items)
        : this.VerticalMenu(menuName, parentButton, "Down", items);

    menu.firstChild.style.maxHeight = "1000px";
    if (isToolboxMenu) menu.type = "Menu";

    for (var itemKey in menu.items) {
        var item = menu.items[itemKey];
        item.name = "Infographic;StiGauge;" + item.key;
        this.AddDragEventsToComponentButton(item);
    }

    menu.action = function (menuItem) {
        if (!menuItem.haveSubMenu) {
            menuItem.name = "Infographic;StiGauge;" + menuItem.key;
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
