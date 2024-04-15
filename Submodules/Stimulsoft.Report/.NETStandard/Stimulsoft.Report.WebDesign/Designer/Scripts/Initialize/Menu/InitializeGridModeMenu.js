
StiMobileDesigner.prototype.GridModeMenu = function () {
    var jsObject = this;
    var items = [];
    items.push(this.Item("Lines", this.loc.FormOptions.GridLines, "ViewOptions.GridLines.png", "Lines"));
    items.push(this.Item("Dots", this.loc.FormOptions.GridDots, "ViewOptions.GridDots.png", "Dots"));

    var menu = this.VerticalMenu("gridModeMenu", this.options.buttons.pagePanelGridMode, "Down", items);

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
        if (!jsObject.options.report) return;
        var designerOptions = jsObject.options.report.info;
        designerOptions.gridMode = menuItem.key;
        StiMobileDesigner.setImageSource(jsObject.options.buttons.pagePanelGridMode.image, jsObject.options, "ViewOptions.Grid" + designerOptions.gridMode + ".png");
        jsObject.SendCommandApplyDesignerOptions(designerOptions);
    }

    menu.onshow = function (menuItem) {
        var designerOptions = jsObject.options.report ? jsObject.options.report.info : null;
        if (designerOptions) {
            for (var name in this.items) {
                this.items[name].setSelected(name == designerOptions.gridMode);
            }
        }
    }

    return menu;
}