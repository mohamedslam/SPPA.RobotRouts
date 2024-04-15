
StiJsViewer.prototype.InitializeSaveDashboardMenu = function (itemStyleName, menuStyleName, isElementExport) {
    var exports = this.options.exports;
    var showExportToImage = exports.showExportToImageBmp || exports.showExportToImageGif || exports.showExportToImageJpeg || exports.showExportToImagePcx ||
        exports.showExportToImagePng || exports.showExportToImageTiff || exports.showExportToImageMetafile || exports.showExportToImageSvg || exports.showExportToImageSvgz;
    var showExportToData = exports.showExportToCsv || exports.showExportToDbf || exports.showExportToXml || exports.showExportToDif || exports.showExportToSylk || exports.showExportToJson;
    var jsObject = this;
    var items = [];

    if (exports.showExportToDocument && !isElementExport && !this.reportParams.isCompilationMode) {
        items.push(this.Item("Document", this.collections.loc["ReportSnapshot"], null, "Document"));
        items.push("separator");
    }

    if (this.options.exports.showExportToPdf) items.push(this.Item("Pdf", "Adobe PDF", null, "Pdf"));
    if (this.options.exports.showExportToExcel2007) items.push(this.Item("Excel2007", "Microsoft Excel", null, "Excel2007"));
    if (showExportToData) items.push(this.Item("Data", this.collections.loc["Data"], null, "Data"));

    if (this.options.exports.showExportToHtml) {
        items.push(this.Item("Html", "HTML", null, "Html"));
    }

    if (showExportToImage) {
        items.push(this.Item("Image", this.collections.loc["Image"], null, "Image"));
    }

    var saveMenu = this.VerticalMenu("saveDashboardMenu", null, "Down", items, itemStyleName, menuStyleName, true);

    if (saveMenu.items.separator) saveMenu.items.separator.style.margin = "1px 2px 0 2px";

    saveMenu.action = function (menuItem) {
        saveMenu.changeVisibleState(false);

        if (!jsObject.checkCloudAuthorization("export")) return;

        if (!(jsObject.options.jsMode && menuItem.key == "Data") && menuItem.key != "Document" && !(menuItem.key == "Image" && jsObject.options.jsMode) && jsObject.options.exports.showExportDialog) {
            var exportForm = jsObject.controls.forms.dashboardExportForm || jsObject.InitializeDashboardExportForm();
            exportForm.show(menuItem.key, saveMenu.parentButton.elementName, saveMenu.parentButton.elementType);
        }
        else {
            jsObject.postExport(menuItem.key, jsObject.getDefaultExportSettings(menuItem.key, true), saveMenu.parentButton.elementName, true);
        }
    }

    saveMenu._changeVisibleState = saveMenu.changeVisibleState;

    saveMenu.changeVisibleState = function (state, parentButton, rightAlign, leftOffset, onlyCorrectPosition) {
        if (state && parentButton && this.items.Data)
            this.items.Data.style.display = parentButton.elementType == "StiTableElement" ? "" : "none";

        saveMenu._changeVisibleState(state, parentButton, rightAlign, leftOffset, onlyCorrectPosition);

        var jsDesigner = jsObject.options.jsDesigner;
        if (state && jsDesigner && jsObject.options.cloudMode && this.items.Document && (!jsDesigner.options.cloudParameters || !jsDesigner.options.cloudParameters.sessionKey)) {
            this.items.Document.style.display = this.items.separator.style.display = "none";
        }
    }

    for (var itemName in saveMenu.items) {
        if (itemName.indexOf("separator") < 0 && saveMenu.items[itemName].caption) {
            saveMenu.items[itemName].caption.style.padding = "0 20px 0 30px";
        }
    }

    return saveMenu;
}
