
StiMobileDesigner.prototype.ColumnsMenu = function () {

    var menu = this.VerticalMenu("columnsMenu", this.options.buttons.columnsPage, "Down", this.GetPageColumnsItems(), "stiDesignerMenuMiddleItem")

    menu.action = function (menuItem) {
        this.jsObject.options.currentPage.properties.columns = menuItem.key;
        menuItem.setSelected(true);
        this.changeVisibleState(false);
        this.jsObject.SendCommandSendProperties(this.jsObject.options.currentPage, ["columns"]);
    }

    menu.onshow = function () {
        for (var itemName in this.items)
            this.items[itemName].setSelected(this.items[itemName].key == this.jsObject.options.currentPage.properties.columns);
    }

    return menu;
}