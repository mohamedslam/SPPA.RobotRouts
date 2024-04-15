
StiMobileDesigner.prototype.UnitMenu = function () {

    var menu = this.VerticalMenu("unitMenu", this.options.buttons.unitButton, "Up", this.GetUnitItems())

    menu.action = function (menuItem) {
        menuItem.setSelected(true);
        this.changeVisibleState(false);
        this.jsObject.SendCommandChangeUnit(menuItem.key);
    }

    menu.onshow = function () {
        if (this.items[this.jsObject.options.report.properties.reportUnit])
            this.items[this.jsObject.options.report.properties.reportUnit].setSelected(true);
    }

    return menu;
}