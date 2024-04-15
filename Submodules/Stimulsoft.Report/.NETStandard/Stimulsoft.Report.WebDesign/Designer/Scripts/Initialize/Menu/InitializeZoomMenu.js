
StiMobileDesigner.prototype.ZoomMenu = function () {

    var zoomValues = ["200", "100", "75", "50", "25"];
    var items = [];

    for (var i = 0; i < zoomValues.length; i++) {
        items.push(this.Item(zoomValues[i], zoomValues[i] + "%", null, parseInt(zoomValues[i]) / 100));
    }

    var menu = this.VerticalMenu("zoomMenu", this.options.buttons.zoomInfo, "Up", items)
    menu.rightToLeft = true;

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
        this.jsObject.options.report.zoom = menuItem.key;
        this.jsObject.PreZoomPage(this.jsObject.options.currentPage);
    }

    menu.onshow = function () {
        if (this.jsObject.options.report.zoom) {
            for (var itemKey in this.items) {
                if (this.items[itemKey] && this.items[itemKey].key)
                    this.items[itemKey].setSelected(this.items[itemKey].key == this.jsObject.options.report.zoom);
            }
        }
    }

    return menu;
}