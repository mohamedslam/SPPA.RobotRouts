
StiJsViewer.prototype.InitializeSaveMenu = function (menuName, parentButton) {
    var saveMenu = this.InitializeBaseSaveMenu("saveMenu", this.controls.toolbar.controls["Save"]);

    saveMenu.action = function (menuItem) {
        saveMenu.changeVisibleState(false);

        if (!this.jsObject.checkCloudAuthorization("export")) return;

        if (saveMenu.jsObject.options.exports.showExportDialog)
            saveMenu.jsObject.controls.forms.exportForm.show(menuItem.key, saveMenu.jsObject.options.actions.exportReport);
        else
            saveMenu.jsObject.postExport(menuItem.key, saveMenu.jsObject.getDefaultExportSettings(menuItem.key));
    }
}


StiJsViewer.prototype.InitializeBaseSaveMenu = function (menuName, parentButton) {
    var imageSize = this.options.appearance.saveMenuImageSize;
    var getImage = function (imageName) {
        if (imageSize == "Big") return "Save.Big." + imageName + ".png";
        else if (imageSize == "None") return null;
        else return "Save.Small." + imageName + ".png";
    }
    var isFirst = true;
    var items = [];
    if (this.options.exports.showExportToDocument && menuName == "saveMenu") {
        items.push(this.Item("Document", this.collections.loc["ReportSnapshot"], getImage("Document"), "Document"));
        isFirst = false;
    }
    if (menuName == "saveMenu" && this.options.exports.showExportToPdf || this.options.exports.showExportToXps || this.options.exports.showExportToPowerPoint) {
        if (!isFirst) items.push("separator1");
        isFirst = false;
    }
    if (this.options.exports.showExportToPdf) items.push(this.Item("Pdf", "Adobe PDF", getImage("Pdf"), "Pdf"));
    if (this.options.exports.showExportToXps) items.push(this.Item("Xps", "Microsoft XPS", getImage("Xps"), "Xps"));
    if (this.options.exports.showExportToPowerPoint) items.push(this.Item("Ppt2007", "Microsoft PowerPoint", getImage("Ppt"), "Ppt2007"));

    if (this.options.exports.showExportToHtml || this.options.exports.showExportToHtml5 || this.options.exports.showExportToMht) {
        if (!isFirst) items.push("separator2");
        isFirst = false;
        var htmlType = this.options.exports.defaultSettings["StiHtmlExportSettings"].HtmlType;
        if (!this.options.exports["showExportTo" + htmlType]) {
            if (this.options.exports.showExportToHtml) htmlType = "Html";
            else if (this.options.exports.showExportToHtml5) htmlType = "Html5";
            else if (this.options.exports.showExportToMht) htmlType = "Mht";
        }
        items.push(this.Item(htmlType, "HTML", getImage("Html"), htmlType));
    }
    if (this.options.exports.showExportToText || this.options.exports.showExportToRtf || this.options.exports.showExportToWord2007 || this.options.exports.showExportToOdt) {
        if (!isFirst) items.push("separator3");
        isFirst = false;
    }
    if (this.options.exports.showExportToText) items.push(this.Item("Text", this.collections.loc["Text"], getImage("Text"), "Text"));
    if (this.options.exports.showExportToRtf) items.push(this.Item("Rtf", "RTF", getImage("Rtf"), "Rtf"));
    if (this.options.exports.showExportToWord2007) items.push(this.Item("Word2007", "Microsoft Word", getImage("Word"), "Word2007"));
    if (this.options.exports.showExportToOpenDocumentWriter) items.push(this.Item("Odt", "OpenDocument Writer", getImage("Odt"), "Odt"));
    if (this.options.exports.showExportToExcel || this.options.exports.showExportToExcel2007 || this.options.exports.showExportToExcelXml || this.options.exports.showExportToOpenDocumentWriter) {
        if (!isFirst) items.push("separator4");
        isFirst = false;
    }
    if (this.options.exports.showExportToExcel || this.options.exports.showExportToExcelXml || this.options.exports.showExportToExcel2007) {
        var excelType = this.options.exports.defaultSettings["StiExcelExportSettings"].ExcelType;
        if (excelType == "ExcelBinary") excelType = "Excel";
        if (!this.options.exports["showExportTo" + excelType]) {
            if (this.options.exports.showExportToExcel) excelType = "Excel";
            else if (this.options.exports.showExportToExcel2007) excelType = "Excel2007";
            else if (this.options.exports.showExportToExcelXml) excelType = "ExcelXml";
        }
        items.push(this.Item(excelType, "Microsoft Excel", getImage("Excel"), excelType));
    }
    if (this.options.exports.showExportToOpenDocumentCalc) {
        items.push(this.Item("Ods", "OpenDocument Calc", getImage("Ods"), "Ods"));
    }
    var showData = this.options.jsMode
        ? this.options.exports.showExportToCsv || this.options.exports.showExportToJson
        : this.options.exports.showExportToCsv || this.options.exports.showExportToDbf || this.options.exports.showExportToXml || this.options.exports.showExportToDif || this.options.exports.showExportToSylk || this.options.exports.showExportToJson

    if (showData) {
        if (!isFirst) items.push("separator5");
        isFirst = false;
        var dataType = this.options.exports.defaultSettings["StiDataExportSettings"].DataType;
        if (!this.options.exports["showExportTo" + dataType]) {
            if (this.options.exports.showExportToCsv) dataType = "Csv";
            else if (this.options.exports.showExportToDbf) dataType = "Dbf";
            else if (this.options.exports.showExportToXml) dataType = "Xml";
            else if (this.options.exports.showExportToDif) dataType = "Dif";
            else if (this.options.exports.showExportToSylk) dataType = "Sylk";
            else if (this.options.exports.showExportToJson) dataType = "Json";
        }
        items.push(this.Item(dataType, this.collections.loc["Data"], getImage("Data"), dataType));
    }
    if (this.options.exports.showExportToImageBmp || this.options.exports.showExportToImageGif || this.options.exports.showExportToImageJpeg || this.options.exports.showExportToImagePcx ||
        this.options.exports.showExportToImagePng || this.options.exports.showExportToImageTiff || this.options.exports.showExportToImageMetafile || this.options.exports.showExportToImageSvg || this.options.exports.showExportToImageSvgz) {
        if (!isFirst) items.push("separator6");
        isFirst = false;
        var imageType = this.options.exports.defaultSettings["StiImageExportSettings"].ImageType;
        var imageType_ = imageType == "Emf" ? "Metafile" : imageType;
        if (!this.options.exports["showExportToImage" + imageType_]) {
            if (this.options.exports.showExportToImageBmp) imageType = "Bmp";
            else if (this.options.exports.showExportToImageGif) imageType = "Gif";
            else if (this.options.exports.showExportToImageJpeg) imageType = "Jpeg";
            else if (this.options.exports.showExportToImagePcx) imageType = "Pcx";
            else if (this.options.exports.showExportToImagePng) imageType = "Png";
            else if (this.options.exports.showExportToImageTiff) imageType = "Tiff";
            else if (this.options.exports.showExportToImageMetafile) imageType = "Emf";
            else if (this.options.exports.showExportToImageSvg) imageType = "Svg";
            else if (this.options.exports.showExportToImageSvgz) imageType = "Svgz";
        }
        items.push(this.Item("Image" + imageType, this.collections.loc["Image"], getImage("Image"), "Image" + imageType));
    }

    if (this.options.appearance.rightToLeft) {
        for (var i = 0; i < items.length; i++) {
            if (items[i].caption) items[i].caption = items[i].caption.replace("...", "");
        }
    }

    var baseSaveMenu = this.VerticalMenu(menuName, parentButton, "Down", items, null, null, this.options.appearance.rightToLeft);
    baseSaveMenu.menuName = menuName;

    if (imageSize == "Big") {
        baseSaveMenu.innerContent.style.maxHeight = "550px";

        if (baseSaveMenu.items.Document) {
            baseSaveMenu.items.Document.cellImage.style.width = "32px";
        }
    }

    for (var itemName in baseSaveMenu.items) {
        var image = baseSaveMenu.items[itemName].image;
        var caption = baseSaveMenu.items[itemName].caption;
        if (itemName.indexOf("separator") < 0) {
            if (imageSize == "None" && caption) {
                caption.style.padding = "0 20px 0 30px";
            }
            else if (imageSize == "Big" && image) {
                baseSaveMenu.items[itemName].style.height = "38px";
                image.style.width = image.style.height = "32px";
            }
        }
        else if (imageSize == "Big") {
            baseSaveMenu.items[itemName].style.margin = "1px 2px 1px 2px";
        }
    }

    return baseSaveMenu;
}