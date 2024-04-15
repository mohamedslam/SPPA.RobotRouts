
StiMobileDesigner.prototype.ReportEnginePropertiesGroup = function () {
    var group = this.PropertiesGroup("reportEnginePropertiesGroup", this.loc.PropertyCategory.EngineCategory);
    group.style.display = "none";

    var properties = [];
    properties.push(["CacheTotals", this.loc.PropertyMain.CacheTotals, this.CheckBox("controlReportPropertyCacheTotals"), "Checkbox"]);
    if (!this.options.jsMode) {
        properties.push(["CalculationMode", this.loc.PropertyMain.CalculationMode, this.PropertyDropDownList("controlReportPropertyCalculationMode", this.options.propertyControlWidth, this.GetCalculationModeItems(), true), "DropdownList"]);
    }
    properties.push(["Collate", this.loc.PropertyMain.Collate, this.PropertyTextBox("controlReportPropertyCollate", this.options.propertyNumbersControlWidth), "Textbox"]);
    if (!this.options.cloudMode && !this.options.serverMode && !this.options.jsMode)
        properties.push(["EngineVersion", this.loc.PropertyMain.EngineVersion, this.PropertyDropDownList("controlReportPropertyEngineVersion", this.options.propertyControlWidth, this.GetEngineVersionItems(), true), "DropdownList"]);
    properties.push(["NumberOfPass", this.loc.PropertyMain.NumberOfPass, this.PropertyDropDownList("controlReportPropertyNumberOfPass", this.options.propertyControlWidth, this.GetNumberOfPassItems(), true), "DropdownList"]);
    if (!this.options.jsMode && !this.options.serverMode)
        properties.push(["ReportCacheMode", this.loc.PropertyMain.ReportCacheMode, this.PropertyDropDownList("controlReportPropertyReportCacheMode", this.options.propertyControlWidth, this.GetReportCacheModeItems(), true), "DropdownList"]);
    properties.push(["ReportUnit", this.loc.PropertyMain.ReportUnit, this.PropertyDropDownList("controlReportPropertyReportUnit", this.options.propertyControlWidth, this.GetUnitItems(), true), "DropdownList"]);
    if (!this.options.cloudMode && !this.options.serverMode && !this.options.jsMode)
        properties.push(["ScriptLanguage", this.loc.PropertyMain.ScriptLanguage, this.PropertyDropDownList("controlReportPropertyScriptLanguage", this.options.propertyControlWidth, this.GetScriptLanguageItems(), true), "DropdownList"]);
    properties.push(["StopBeforePage", this.loc.PropertyMain.StopBeforePage, this.PropertyTextBox("controlReportPropertyStopBeforePage", this.options.propertyNumbersControlWidth), "Textbox"]);

    var jsObject = this;

    for (var i = 0; i < properties.length; i++) {
        var control = properties[i][2];
        control.propertyName = this.LowerFirstChar(properties[i][0]);
        control.controlType = properties[i][3];
        group.container.appendChild(this.Property(control.propertyName, properties[i][1], control));
        group.jsObject.AddMainMethodsToPropertyControl(control);

        control.action = function () {
            if (this.propertyName == "reportUnit") {
                jsObject.SendCommandChangeUnit(this.key);
            }
            else {
                jsObject.options.selectedObject.properties[this.propertyName] = this.getValue();
                jsObject.SendCommandSetReportProperties([this.propertyName]);
            }
        }
    }

    return group;
}