
StiJsViewer.prototype.GetImageTypesItems = function () {
    var items = [];
    if (this.options.exports.showExportToImageBmp) items.push(this.Item("Bmp", "Bmp", null, "Bmp"));
    if (this.options.exports.showExportToImageGif) items.push(this.Item("Gif", "Gif", null, "Gif"));
    if (this.options.exports.showExportToImageJpeg) items.push(this.Item("Jpeg", "Jpeg", null, "Jpeg"));
    if (this.options.exports.showExportToImagePcx) items.push(this.Item("Pcx", "Pcx", null, "Pcx"));
    if (this.options.exports.showExportToImagePng) items.push(this.Item("Png", "Png", null, "Png"));
    if (this.options.exports.showExportToImageTiff) items.push(this.Item("Tiff", "Tiff", null, "Tiff"));
    if (this.options.exports.showExportToImageMetafile) items.push(this.Item("Emf", "Emf", null, "Emf"));
    if (this.options.exports.showExportToImageSvg) items.push(this.Item("Svg", "Svg", null, "Svg"));
    if (this.options.exports.showExportToImageSvgz) items.push(this.Item("Svgz", "Svgz", null, "Svgz"));

    return items;
}

StiJsViewer.prototype.GetDataTypesItems = function () {
    var items = [];
    if (this.options.exports.showExportToCsv) items.push(this.Item("Csv", "CSV", null, "Csv"));
    if (this.options.exports.showExportToDbf) items.push(this.Item("Dbf", "DBF", null, "Dbf"));
    if (this.options.exports.showExportToXml) items.push(this.Item("Xml", "XML", null, "Xml"));
    if (this.options.exports.showExportToDif) items.push(this.Item("Dif", "DIF", null, "Dif"));
    if (this.options.exports.showExportToSylk) items.push(this.Item("Sylk", "SYLK", null, "Sylk"));
    if (this.options.exports.showExportToJson) items.push(this.Item("Json", "JSON", null, "Json"));

    return items;
}

StiJsViewer.prototype.GetExcelTypesItems = function () {
    var items = [];
    if (this.options.exports.showExportToExcel2007) items.push(this.Item("Excel2007", "Excel", null, "Excel2007"));
    if (this.options.exports.showExportToExcel) items.push(this.Item("ExcelBinary", "Excel 97-2003", null, "ExcelBinary"));
    if (this.options.exports.showExportToExcelXml) items.push(this.Item("ExcelXml", "Excel Xml 2003", null, "ExcelXml"));

    return items;
}

StiJsViewer.prototype.GetHtmlTypesItems = function () {
    var items = [];
    if (this.options.exports.showExportToHtml) items.push(this.Item("Html", "Html", null, "Html"));
    if (this.options.exports.showExportToHtml5) items.push(this.Item("Html5", "Html5", null, "Html5"));
    if (this.options.exports.showExportToMht) items.push(this.Item("Mht", "Mht", null, "Mht"));

    return items;
}

StiJsViewer.prototype.GetZoomItems = function () {
    var items = [];
    var values = [0.25, 0.5, 0.75, 1, 1.25, 1.5, 2];
    for (var i = 0; i < values.length; i++)
        items.push(this.Item("item" + i, (values[i] * 100) + "%", null, values[i].toString()));

    return items;
}

StiJsViewer.prototype.GetImageFormatForHtmlItems = function () {
    var items = [];
    items.push(this.Item("item0", "Jpeg", null, "Jpeg"));
    items.push(this.Item("item1", "Gif", null, "Gif"));
    items.push(this.Item("item2", "Bmp", null, "Bmp"));
    items.push(this.Item("item3", "Png", null, "Png"));

    return items;
}

StiJsViewer.prototype.GetExportModeItems = function () {
    var items = [];
    items.push(this.Item("item0", "Table", null, "Table"));
    items.push(this.Item("item1", "Span", null, "Span"));
    items.push(this.Item("item2", "Div", null, "Div"));

    return items;
}

StiJsViewer.prototype.GetImageResolutionItems = function () {
    var items = [];
    var values = ["10", "25", "50", "75", "100", "200", "300", "400", "500"];
    for (var i = 0; i < values.length; i++)
        items.push(this.Item("item" + i, values[i], null, values[i]));

    return items;
}

StiJsViewer.prototype.GetImageCompressionMethodItems = function () {
    var items = [];
    items.push(this.Item("item0", "Jpeg", null, "Jpeg"));
    items.push(this.Item("item1", "Flate", null, "Flate"));

    return items;
}

StiJsViewer.prototype.GetImageQualityItems = function () {
    var items = [];
    var values = [0.25, 0.5, 0.75, 0.85, 0.9, 0.95, 1];
    for (var i = 0; i < values.length; i++)
        items.push(this.Item("item" + i, values[i] * 100, null, values[i].toString()));

    return items;
}

StiJsViewer.prototype.GetBorderTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.collections.loc["BorderTypeSimple"], null, "Simple"));
    items.push(this.Item("item1", this.collections.loc["BorderTypeSingle"], null, "UnicodeSingle"));
    items.push(this.Item("item2", this.collections.loc["BorderTypeDouble"], null, "UnicodeDouble"));

    return items;
}

StiJsViewer.prototype.GetEncodingDataItems = function () {
    var items = [];
    for (var i = 0; i < this.collections.encodingData.length; i++) {
        var item = this.collections.encodingData[i];
        items.push(this.Item("item" + i, item.value, null, item.key));
    }

    return items;
}

StiJsViewer.prototype.GetImageFormatItems = function (withoutMonochrome) {
    var items = [];
    items.push(this.Item("item0", this.collections.loc["ImageFormatColor"], null, "Color"));
    items.push(this.Item("item1", this.collections.loc["ImageFormatGrayscale"], null, "Grayscale"));
    if (!withoutMonochrome) items.push(this.Item("item2", this.collections.loc["ImageFormatMonochrome"], null, "Monochrome"));

    return items;
}

StiJsViewer.prototype.GetMonochromeDitheringTypeItems = function () {
    var items = [];
    items.push(this.Item("item0", "None", null, "None"));
    items.push(this.Item("item1", "FloydSteinberg", null, "FloydSteinberg"));
    items.push(this.Item("item2", "Ordered", null, "Ordered"));

    return items;
}

StiJsViewer.prototype.GetTiffCompressionSchemeItems = function () {
    var items = [];
    items.push(this.Item("item0", "Default", null, "Default"));
    items.push(this.Item("item1", "CCITT3", null, "CCITT3"));
    items.push(this.Item("item2", "CCITT4", null, "CCITT4"));
    items.push(this.Item("item3", "LZW", null, "LZW"));
    items.push(this.Item("item4", "None", null, "None"));
    items.push(this.Item("item5", "Rle", null, "Rle"));

    return items;
}

StiJsViewer.prototype.GetEncodingDifFileItems = function () {
    var items = [];
    items.push(this.Item("item0", "437", null, "437"));
    items.push(this.Item("item1", "850", null, "850"));
    items.push(this.Item("item2", "852", null, "852"));
    items.push(this.Item("item3", "857", null, "857"));
    items.push(this.Item("item4", "860", null, "860"));
    items.push(this.Item("item5", "861", null, "861"));
    items.push(this.Item("item6", "862", null, "862"));
    items.push(this.Item("item7", "863", null, "863"));
    items.push(this.Item("item8", "865", null, "865"));
    items.push(this.Item("item9", "866", null, "866"));
    items.push(this.Item("item10", "869", null, "869"));

    return items;
}

StiJsViewer.prototype.GetExportModeRtfItems = function () {
    var items = [];
    items.push(this.Item("item0", this.collections.loc["ExportModeRtfTable"], null, "Table"));
    items.push(this.Item("item1", this.collections.loc["ExportModeRtfFrame"], null, "Frame"));

    return items;
}

StiJsViewer.prototype.GetEncodingDbfFileItems = function () {
    var items = [];
    items.push(this.Item("item0", "Default", null, "Default"));
    items.push(this.Item("item1", "437 U.S. MS-DOS", null, "USDOS"));
    items.push(this.Item("item2", "620 Mazovia(Polish) MS-DOS", null, "MazoviaDOS"));
    items.push(this.Item("item3", "737 Greek MS-DOS(437G)", null, "GreekDOS"));
    items.push(this.Item("item4", "850 International MS-DOS", null, "InternationalDOS"));
    items.push(this.Item("item5", "852 Eastern European MS-DOS", null, "EasternEuropeanDOS"));
    items.push(this.Item("item6", "857 Turkish MS-DOS", null, "TurkishDOS"));
    items.push(this.Item("item7", "861 Icelandic MS-DOS", null, "IcelandicDOS"));
    items.push(this.Item("item8", "865 Nordic MS-DOS", null, "NordicDOS"));
    items.push(this.Item("item9", "866 Russian MS-DOS", null, "RussianDOS"));
    items.push(this.Item("item10", "895 Kamenicky(Czech) MS-DOS", null, "KamenickyDOS"));
    items.push(this.Item("item11", "1250 Eastern European Windows", null, "EasternEuropeanWindows"));
    items.push(this.Item("item12", "1251 Russian Windows", null, "RussianWindows"));
    items.push(this.Item("item13", "1252 WindowsANSI", null, "WindowsANSI"));
    items.push(this.Item("item14", "1253 GreekWindows", null, "GreekWindows"));
    items.push(this.Item("item15", "1254 TurkishWindows", null, "TurkishWindows"));
    items.push(this.Item("item16", "10000 StandardMacintosh", null, "StandardMacintosh"));
    items.push(this.Item("item17", "10006 GreekMacintosh", null, "GreekMacintosh"));
    items.push(this.Item("item18", "10007 RussianMacintosh", null, "RussianMacintosh"));
    items.push(this.Item("item19", "10029 EasternEuropeanMacintosh", null, "EasternEuropeanMacintosh"));

    return items;
}

StiJsViewer.prototype.GetAllowEditableItems = function () {
    var items = [];
    items.push(this.Item("item0", this.collections.loc["NameYes"], null, "Yes"));
    items.push(this.Item("item1", this.collections.loc["NameNo"], null, "No"));

    return items;
}

StiJsViewer.prototype.GetEncryptionKeyLengthItems = function () {
    var items = [];
    items.push(this.Item("item0", "40 bit RC4 (Acrobat 3)", null, "Bit40"));
    items.push(this.Item("item1", "128 bit RC4 (Acrobat 5)", null, "Bit128"));
    items.push(this.Item("item2", "128 bit AES (Acrobat 7)", null, "Bit128_r4"));
    items.push(this.Item("item3", "256 bit AES (Acrobat 9)", null, "Bit256_r5"));
    items.push(this.Item("item4", "256 bit AES (Acrobat X)", null, "Bit256_r6"));

    return items;
}

StiJsViewer.prototype.GetDataExportModeItems = function () {
    var items = [];
    items.push(this.Item("item0", this.collections.loc["BandsFilterDataOnly"], null, "Data"));
    items.push(this.Item("item1", this.collections.loc["BandsFilterDataAndHeaders"], null, "DataAndHeaders"));
    items.push(this.Item("item2", this.collections.loc["BandsFilterDataAndHeadersFooters"], null, "DataAndHeadersFooters"));
    items.push(this.Item("item3", this.collections.loc["BandsFilterAllBands"], null, "AllBands"));

    return items;
}

StiJsViewer.prototype.GetFilterConditionItems = function (dataType) {
    var items = [];
    switch (dataType) {
        case "String":
            {
                items.push(this.Item("item0", this.collections.loc["ConditionEqualTo"], "", "EqualTo"));
                items.push(this.Item("item1", this.collections.loc["ConditionNotEqualTo"], "", "NotEqualTo"));
                items.push("separator1");
                items.push(this.Item("item2", this.collections.loc["ConditionContaining"], "", "Containing"));
                items.push(this.Item("item3", this.collections.loc["ConditionNotContaining"], "", "NotContaining"));
                items.push("separator2");
                items.push(this.Item("item4", this.collections.loc["ConditionBeginningWith"], "", "BeginningWith"));
                items.push(this.Item("item5", this.collections.loc["ConditionEndingWith"], "", "EndingWith"));
                items.push("separator3");
                items.push(this.Item("item2", this.collections.loc["ConditionBetween"], "", "Between"));
                items.push(this.Item("item3", this.collections.loc["ConditionNotBetween"], "", "NotBetween"));
                items.push("separator4");
                items.push(this.Item("item6", this.collections.loc["ConditionGreaterThan"], "", "GreaterThan"));
                items.push(this.Item("item7", this.collections.loc["ConditionGreaterThanOrEqualTo"], "", "GreaterThanOrEqualTo"));
                items.push("separator5");
                items.push(this.Item("item8", this.collections.loc["ConditionLessThan"], "", "LessThan"));
                items.push(this.Item("item9", this.collections.loc["ConditionLessThanOrEqualTo"], "", "LessThanOrEqualTo"));
                items.push("separator6");
                items.push(this.Item("item10", this.collections.loc["ConditionIsNull"], "", "IsNull"));
                items.push(this.Item("item11", this.collections.loc["ConditionIsNotNull"], "", "IsNotNull"));
                items.push("separator7");
                items.push(this.Item("item12", this.collections.loc["ConditionIsBlank"], "", "IsBlank"));
                items.push(this.Item("item13", this.collections.loc["ConditionIsNotBlank"], "", "IsNotBlank"));
                break;
            }
        case "Numeric":
        case "DateTime":
            {
                items.push(this.Item("item0", this.collections.loc["ConditionEqualTo"], "", "EqualTo"));
                items.push(this.Item("item1", this.collections.loc["ConditionNotEqualTo"], "", "NotEqualTo"));
                items.push("separator1");
                items.push(this.Item("item2", this.collections.loc["ConditionBetween"], "", "Between"));
                items.push(this.Item("item3", this.collections.loc["ConditionNotBetween"], "", "NotBetween"));
                items.push("separator2");
                items.push(this.Item("item4", this.collections.loc["ConditionGreaterThan"], "", "GreaterThan"));
                items.push(this.Item("item5", this.collections.loc["ConditionGreaterThanOrEqualTo"], "", "GreaterThanOrEqualTo"));
                items.push("separator3");
                items.push(this.Item("item6", this.collections.loc["ConditionLessThan"], "", "LessThan"));
                items.push(this.Item("item7", this.collections.loc["ConditionLessThanOrEqualTo"], "", "LessThanOrEqualTo"));
                items.push("separator4");
                items.push(this.Item("item8", this.collections.loc["ConditionIsNull"], "", "IsNull"));
                items.push(this.Item("item9", this.collections.loc["ConditionIsNotNull"], "", "IsNotNull"));
                break;
            }
        case "Boolean":
            {
                items.push(this.Item("item0", this.collections.loc["ConditionEqualTo"], "", "EqualTo"));
                items.push(this.Item("item1", this.collections.loc["ConditionNotEqualTo"], "", "NotEqualTo"));
                break;
            }
    }

    return items;
}

StiJsViewer.prototype.GetBoolItems = function () {
    var items = [];
    items.push(this.Item("item0", this.collections.loc["NameTrue"], null, "True"));
    items.push(this.Item("item1", this.collections.loc["NameFalse"], null, "False"));

    return items;
}

StiJsViewer.prototype.GetPaperSizesItems = function () {
    var items = [];
    for (var i = 0; i < this.collections.paperSizes.length; i++) {
        var paperSize = this.collections.paperSizes[i];
        items.push(this.Item("item" + i, paperSize, null, paperSize));
    }

    return items;
}

StiJsViewer.prototype.GetOrientationItems = function () {
    var items = [];
    items.push(this.Item("item0", this.collections.loc["Portrait"], null, "Portrait"));
    items.push(this.Item("item1", this.collections.loc["Landscape"], null, "Landscape"));

    return items;
}

StiJsViewer.prototype.GetDashboardImageQualityItems = function () {
    var items = [];
    var values = [50, 75, 100, 150, 200, 300, 500];
    for (var i = 0; i < values.length; i++)
        items.push(this.Item("item" + i, values[i] + "%", null, values[i].toString()));

    return items;
}

StiJsViewer.prototype.GetPdfSecurityCertificatesItems = function () {
    var items = [];
    if (this.collections.pdfSecurityCertificates) {
        for (var i = 0; i < this.collections.pdfSecurityCertificates.length; i++) {
            var item = this.collections.pdfSecurityCertificates[i];
            items.push(this.Item("item" + i, "Name: " + item.name + "<br>Issuer: " + item.issuer + "<br>Valid from: " + item.from + " to " + item.to, null, item.thumbprint));
        }
    }

    return items;
}

StiJsViewer.prototype.GetImageResolutionModeItems = function () {
    var items = [];
    items.push(this.Item("Exactly", this.collections.loc["ImageResolutionModeExactly"], null, "Exactly"));
    items.push(this.Item("NoMoreThan", this.collections.loc["ImageResolutionModeNoMoreThan"], null, "NoMoreThan"));
    items.push(this.Item("Auto", this.collections.loc["ImageResolutionModeAuto"], null, "Auto"));

    return items;
}

StiJsViewer.prototype.GetFontSizeItems = function () {
    var fontSizes = ["5", "6", "7", "8", "9", "10", "11", "12", "14", "16", "18", "20", "22", "24", "26", "28", "36", "48", "72"];
    var sizeItems = [];
    for (var i = 0; i < fontSizes.length; i++) {
        sizeItems.push(this.Item("homeSizesFont" + i, fontSizes[i], null, fontSizes[i]));
    }

    return sizeItems;
}

StiJsViewer.prototype.GetFontNamesItems = function () {
    var items = [];
    for (var i = 0; i < this.options.fontNames.length; i++)
        items.push(this.Item("fontItem" + i, this.options.fontNames[i].value, null, this.options.fontNames[i].value));

    return items;
}