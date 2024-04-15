
StiMobileDesigner.prototype.CrossBandsMenu = function (menuName, parentButton, isToolboxMenu) {
    var componentTypes = ["StiCrossGroupHeaderBand", "StiCrossGroupFooterBand", "StiCrossHeaderBand", "StiCrossFooterBand", "StiCrossDataBand"];

    var items = [];

    for (var i = 0; i < componentTypes.length; i++) {
        if (this.options.visibilityCrossBands[componentTypes[i]]) {
            items.push(this.Item(componentTypes[i], this.loc.Components[componentTypes[i]], "SmallComponents." + componentTypes[i] + ".png", componentTypes[i]));
        }
    }

    var menu = isToolboxMenu
        ? this.HorizontalMenu(menuName, parentButton, "Right", items)
        : this.VerticalMenu(menuName, parentButton, "Down", items);

    for (var itemKey in menu.items) {
        this.AddDragEventsToComponentButton(menu.items[itemKey]);
        menu.items[itemKey].setAttribute("title", this.loc.HelpComponents[itemKey] || menu.items[itemKey].caption.innerText);
    }

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
        var panel = isToolboxMenu ? this.jsObject.options.toolbox : this.jsObject.options.insertPanel;
        panel.resetChoose();
        panel.setChoose(menuItem);
    }

    return menu;
}