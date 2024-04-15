
StiMobileDesigner.prototype.ReportDataPropertiesGroup = function () {
    var group = this.PropertiesGroup("reportDataPropertiesGroup", this.loc.PropertyCategory.DataCategory);
    group.style.display = "none";

    var properties = [];
    properties.push(["CacheAllData", this.loc.PropertyMain.CacheAllData, this.CheckBox("controlReportPropertyCacheAllData"), "Checkbox"]);
    properties.push(["ConvertNulls", this.loc.PropertyMain.ConvertNulls, this.CheckBox("controlReportPropertyConvertNulls"), "Checkbox"]);
    properties.push(["RetrieveOnlyUsedData", this.loc.PropertyMain.RetrieveOnlyUsedData, this.CheckBox("controlReportPropertyRetrieveOnlyUsedData"), "Checkbox"]);
    var jsObject = this;

    for (var i = 0; i < properties.length; i++) {
        var control = properties[i][2];
        control.propertyName = this.LowerFirstChar(properties[i][0]);
        control.controlType = properties[i][3];
        group.container.appendChild(this.Property(control.propertyName, properties[i][1], control));
        group.jsObject.AddMainMethodsToPropertyControl(control);

        control.action = function () {
            jsObject.options.selectedObject.properties[this.propertyName] = this.getValue();
            jsObject.SendCommandSetReportProperties([this.propertyName]);
        }
    }

    return group;
}