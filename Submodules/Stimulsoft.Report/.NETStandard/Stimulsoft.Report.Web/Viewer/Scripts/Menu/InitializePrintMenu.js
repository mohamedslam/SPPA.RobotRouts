
StiJsViewer.prototype.InitializePrintMenu = function () {
    var items = [];
    items.push(this.Item("PrintPdf", this.collections.loc["PrintPdf"], "Save.Small.Pdf.png", "PrintPdf"));
    items.push(this.Item("PrintWithPreview", this.collections.loc["PrintWithPreview"], "ViewMode.png", "PrintWithPreview"));
    items.push(this.Item("PrintWithoutPreview", this.collections.loc["PrintWithoutPreview"], "Print.png", "PrintWithoutPreview"));

    var printMenu = this.VerticalMenu("printMenu", this.controls.toolbar.controls["Print"], "Down", items, null, null, this.options.appearance.rightToLeft);

    printMenu.action = function (menuItem) {
        printMenu.changeVisibleState(false);
        printMenu.jsObject.postPrint(menuItem.key);
    }
}