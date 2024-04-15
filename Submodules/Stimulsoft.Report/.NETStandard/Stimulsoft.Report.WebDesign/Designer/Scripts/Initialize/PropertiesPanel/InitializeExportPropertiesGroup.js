
StiMobileDesigner.prototype.ExportPropertiesGroup = function () {
    var exportPropertiesGroup = this.PropertiesGroup("exportPropertiesGroup", this.loc.PropertyCategory.ExportCategory);
    exportPropertiesGroup.style.display = "none";

    //ExcelValue
    var controlPropertyExcelValue = this.PropertyExpressionControl("controlPropertyExcelValue", this.options.propertyControlWidth, false);
    controlPropertyExcelValue.action = function () {
        this.jsObject.ApplyPropertyValue("excelValue", StiBase64.encode(this.textBox.value));
    }
    exportPropertiesGroup.container.appendChild(this.Property("excelValue", this.loc.PropertyMain.ExcelValue, controlPropertyExcelValue));

    //ExcelSheet
    var controlPropertyExcelSheet = this.PropertyExpressionControl("controlPropertyExcelSheet", this.options.propertyControlWidth, false);
    controlPropertyExcelSheet.action = function () {
        this.jsObject.ApplyPropertyValue("excelSheet", StiBase64.encode(this.textBox.value));
    }
    exportPropertiesGroup.container.appendChild(this.Property("excelSheet", this.loc.PropertyMain.ExcelSheet, controlPropertyExcelSheet));


    //ExportAsImage
    var controlPropertyExportAsImage = this.CheckBox("controlPropertyExportAsImage");
    controlPropertyExportAsImage.action = function () {
        this.jsObject.ApplyPropertyValue("exportAsImage", this.isChecked);
    }
    exportPropertiesGroup.container.appendChild(this.Property("exportAsImage", this.loc.PropertyMain.ExportAsImage, controlPropertyExportAsImage));

    return exportPropertiesGroup;
}