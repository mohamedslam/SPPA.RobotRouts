
StiJsViewer.prototype.InitializeZoomMenu = function () {
    var items = [];
    var zoomItems = ["25", "50", "75", "100", "150", "200"];
    for (var i = 0; i < zoomItems.length; i++) {
        items.push(this.Item("Zoom" + zoomItems[i], zoomItems[i] + "%", "SelectedItem.png", "Zoom" + zoomItems[i]));
    }

    if (this.options.toolbar.displayMode != "Separated") {
        items.push("separator1");
        items.push(this.Item("ZoomOnePage", this.collections.loc["ZoomOnePage"], "ZoomOnePage.png", "ZoomOnePage"));
        items.push(this.Item("ZoomPageWidth", this.collections.loc["ZoomPageWidth"], "ZoomPageWidth.png", "ZoomPageWidth"));
    }

    var zoomMenu = this.VerticalMenu("zoomMenu", this.controls.toolbar.controls["Zoom"],
        this.options.toolbar.displayMode == "Separated" ? "Up" : "Down", items, null, null, this.options.toolbar.displayMode == "Separated" || this.options.appearance.rightToLeft);

    zoomMenu.action = function (menuItem) {
        zoomMenu.changeVisibleState(false);
        if (this.jsObject.options.toolbar.displayMode == "Separated") {
            this.jsObject.controls.toolbar.controls.ZoomOnePage.setSelected(false);
            this.jsObject.controls.toolbar.controls.ZoomPageWidth.setSelected(false);
        }
        zoomMenu.jsObject.postAction(menuItem.key);
    }
}