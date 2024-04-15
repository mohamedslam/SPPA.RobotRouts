
StiJsViewer.prototype.InitializeViewModeMenu = function () {
    var items = [];
    items.push(this.Item("SinglePage", this.collections.loc["SinglePage"], "SinglePage.png", "ViewModeSinglePage"));
    items.push(this.Item("Continuous", this.collections.loc["Continuous"], "Continuous.png", "ViewModeContinuous"));
    items.push(this.Item("MultiplePages", this.collections.loc["MultiplePages"], "MultiplePages.png", "ViewModeMultiplePages"));

    var viewModeMenu = this.VerticalMenu("viewModeMenu", this.controls.toolbar.controls["ViewMode"], "Down", items, null, null, this.options.appearance.rightToLeft);

    viewModeMenu.action = function (menuItem) {
        viewModeMenu.changeVisibleState(false);
        viewModeMenu.jsObject.postAction(menuItem.key);
    }
}