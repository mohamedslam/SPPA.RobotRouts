
StiMobileDesigner.prototype.OrientationMenu = function () {

    var menu = this.VerticalMenu("orientationMenu", this.options.buttons.orientationPage, "Down", this.GetPageOrientationItems(true), "stiDesignerMenuMiddleItem")

    menu.action = function (menuItem) {
        this.jsObject.options.currentPage.properties.orientation = menuItem.key;
        menuItem.setSelected(true);
        this.changeVisibleState(false);
        this.jsObject.SendCommandSendProperties(this.jsObject.options.currentPage, ["orientation"]);
    }

    menu.onshow = function () {
        if (this.jsObject.options.currentPage.properties.orientation == "Portrait") this.items.portrait.setSelected(true);
        else this.items.landscape.setSelected(true);
    }

    return menu;
}