
StiJsViewer.prototype.InitializeDashboardExportForm = function () {
    var jsObject = this;
    var form = this.BaseForm("dashboardExportForm", this.collections.loc["ExportFormTitle"], 1, this.helpLinks["DashboardExport"]);
    form.container.style.padding = "0px";
    form.controls = {};
    form.defaultExportSettings = {};

    var properties = [
        ["PaperSize", this.collections.loc["PaperSize"], this.DropDownList("exportPaperSize", 180, null, this.GetPaperSizesItems(), true), "6px 12px 6px 12px"],
        ["Orientation", this.collections.loc["Orientation"], this.DropDownList("exportOrientation", 180, null, this.GetOrientationItems(), true), "6px 12px 6px 12px"],
        ["DataType", this.collections.loc["DataType"], this.DropDownList("exportDataType", 180, null, this.GetDataTypesItems(), true), "6px 12px 6px 12px"],
        ["ImageType", this.collections.loc["ImageFormat"], this.DropDownList("exportImageType", 180, null, this.GetImageTypesItems(), true), "6px 12px 6px 12px"],
        ["ImageQuality", this.collections.loc["ImageQuality"], this.DropDownList("exportImageQuality", 180, null, this.GetDashboardImageQualityItems()), "6px 12px 6px 12px"],
        ["Scale", this.collections.loc["Scale"], this.DropDownList("exportScale", 180, null, this.GetDashboardImageQualityItems()), "6px 12px 6px 12px"],
        ["ExportDataOnly", null, this.CheckBox("exportDataOnly", this.collections.loc["ExportDataOnly"]), "6px 12px 6px 12px"],
        ["EnableAnimation", null, this.CheckBox("exportEnableAnimation", this.collections.loc["EnableAnimation"]), "6px 12px 6px 12px"],
        ["OpenAfterExport", null, this.CheckBox("exportOpenAfterExport", this.collections.loc["OpenAfterExport"]), "6px 12px 6px 12px"]
    ];

    var proprtiesTable = this.CreateHTMLTable();
    proprtiesTable.style.margin = "6px 0px 6px 0px";
    form.container.appendChild(proprtiesTable);

    for (var i = 0; i < properties.length; i++) {
        form.addControlRow(proprtiesTable, properties[i][1], properties[i][0], properties[i][2], properties[i][3]);
    }

    form.controls.ImageType.action = function () {
        form.controls.Scale.setEnabled(this.key != "Svg" && this.key != "Svgz") ? "" : "none";
    }

    //Form Methods
    form.setHelpUrls = function () {
        var helpUrlKey = "DashboardExport";

        switch (form.exportFormat) {
            case "Pdf": helpUrlKey = "DashboardPdfExport"; break;
            case "Excel2007": helpUrlKey = "DashboardExcelExport"; break;
            case "Image": helpUrlKey = "DashboardImageExport"; break;
            case "Data": helpUrlKey = "DashboardDataExport"; break;
            case "Html": helpUrlKey = "DashboardHtmlExport"; break;
            default: helpUrlKey = "DashboardExport"; break;
        }

        form.helpUrl = jsObject.helpLinks[helpUrlKey];
    }

    form.setDefaultValues = function () {
        var defaultExportSettings = this.defaultExportSettings[this.exportFormat] || jsObject.getDefaultExportSettings(this.exportFormat, true);
        if (!defaultExportSettings) return;

        for (var propertyName in this.controls) {
            var control = this.controls[propertyName];
            var value = defaultExportSettings[propertyName];
            if (control && value != null) {
                if (control.setKey != null) control.setKey(control.haveKey ? (control.haveKey(value) ? value : (control.items && control.items.length > 0 ? control.items[0].key : value)) : value);
                else if (control.setChecked != null) control.setChecked(value);
                else if (control.value != null) control.value = value;
            }
        }
    }

    form.showControlsByFormat = function () {
        this.controls.PaperSizeRow.style.display = this.controls.OrientationRow.style.display = this.exportFormat == "Pdf" ? "" : "none";
        this.controls.ImageQualityRow.style.display = this.exportFormat == "Html" || this.exportFormat == "Pdf" || this.exportFormat == "Excel2007" ? "" : "none";
        this.controls.ImageTypeRow.style.display = this.exportFormat == "Image" ? "" : "none";
        this.controls.ScaleRow.style.display = this.exportFormat == "Html" || this.exportFormat == "Image" ? "" : "none";
        this.controls.DataTypeRow.style.display = this.exportFormat == "Data" ? "" : "none";
        this.controls.EnableAnimationRow.style.display = this.exportFormat == "Html" ? "" : "none";
        this.controls.OpenAfterExportRow.style.display = !jsObject.options.jsMode && jsObject.options.exports.showOpenAfterExport !== false && (this.exportFormat == "Pdf" || this.exportFormat == "Image" || this.exportFormat == "Html") ? "" : "none";
        this.controls.ExportDataOnlyRow.style.display = this.exportFormat == "Excel2007" && (this.elementType == "StiTableElement" || this.elementType == "StiPivotTableElement") ? "" : "none";
    }

    form.getExportSettingsValues = function () {
        var settings = {};
        for (var i = 0; i < properties.length; i++) {
            var propertyName = properties[i][0];
            if (this.controls[propertyName + "Row"].style.display == "") {
                var control = this.controls[propertyName];
                if (control.setKey != null) settings[propertyName] = control.key;
                else if (control.setChecked != null) settings[propertyName] = control.isChecked;
                else if (control.value != null) settings[propertyName] = control.value;
            }
        }
        if (!jsObject.options.jsMode && jsObject.options.exports.showOpenAfterExport === false && (this.exportFormat == "Pdf" || this.exportFormat == "Image" || this.exportFormat == "Html")) {
            settings.OpenAfterExport = jsObject.options.exports.openAfterExport != null ? jsObject.options.exports.openAfterExport : true;
        }

        return settings;
    }

    form.show = function (exportFormat, elementName, elementType) {
        this.exportFormat = exportFormat;
        this.elementName = elementName;
        this.elementType = elementType;
        this.showControlsByFormat();
        this.setDefaultValues();
        this.setHelpUrls();
        this.controls.ImageType.action();
        this.changeVisibleState(true);
    }

    form.action = function () {
        this.changeVisibleState(false);
        var resultSettings = this.getExportSettingsValues();
        this.defaultExportSettings[this.exportFormat] = resultSettings;
        jsObject.postExport(this.exportFormat, resultSettings, this.elementName, true);
    }

    return form;
}