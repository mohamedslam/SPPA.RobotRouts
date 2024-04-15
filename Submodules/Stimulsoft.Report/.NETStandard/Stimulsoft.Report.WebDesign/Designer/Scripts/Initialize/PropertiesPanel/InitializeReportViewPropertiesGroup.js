
StiMobileDesigner.prototype.ReportViewPropertiesGroup = function () {
    var group = this.PropertiesGroup("reportViewPropertiesGroup", this.loc.PropertyCategory.ViewCategory);
    group.style.display = "none";

    var properties = [];
    if (this.options.cloudMode || this.options.serverMode) {
        properties.push(["ParametersDateFormat", this.loc.PropertyMain.ParametersDateFormat, this.PropertyDropDownList("controlReportPropertyParametersDateFormat", this.options.propertyControlWidth, this.GetParametersDateFormatItems(), false), "DropdownList"]);
    }
    properties.push(["ParametersOrientation", this.loc.PropertyMain.ParametersOrientation, this.PropertyDropDownList("controlReportPropertyParametersOrientation", this.options.propertyControlWidth, this.GetParametersOrientationItems(), true), "DropdownList"]);
    properties.push(["ParameterWidth", this.loc.PropertyMain.ParameterWidth, this.PropertyTextBox("controlReportPropertyParameterWidth", this.options.propertyControlWidth), "Textbox"]);
    properties.push(["PreviewSettings", this.loc.PropertyMain.PreviewSettings, this.PropertyTextBoxWithEditButton("controlReportPropertyPreviewSettings", this.options.propertyControlWidth, true), "TextBoxWithEditButton"]);
    properties.push(["RefreshTime", this.loc.PropertyMain.RefreshTime, this.PropertyDropDownList("controlReportPropertyRefreshTime", this.options.propertyControlWidth, this.GetRefreshTimeItems()), "DropdownList"]);
    properties.push(["RequestParameters", this.loc.PropertyMain.RequestParameters, this.CheckBox("controlReportPropertyRequestParameters"), "Checkbox"]);

    var jsObject = this;

    for (var i = 0; i < properties.length; i++) {
        var control = properties[i][2];
        control.propertyName = this.LowerFirstChar(properties[i][0]);
        control.controlType = properties[i][3];
        group.container.appendChild(this.Property(control.propertyName, properties[i][1], control));
        group.jsObject.AddMainMethodsToPropertyControl(control);

        control.action = function () {
            if (this.propertyName == "refreshTime") {
                if (jsObject.options.cloudMode && parseInt(this.key) > 0 && parseInt(this.key) < 30) {
                    this.setKey("30");
                }
                this.textBox.value = this.key;
            }
            if (this.propertyName == "parameterWidth") {
                this.value = jsObject.StrToCorrectPositiveInt(this.value);
            }

            jsObject.options.selectedObject.properties[this.propertyName] = this.getValue();
            jsObject.SendCommandSetReportProperties([this.propertyName]);
        }

        if (control.propertyName == "previewSettings") {
            control.textBox.value = "(" + jsObject.loc.PropertyMain.PreviewSettings + ")";
            control.button.action = function () {
                jsObject.InitializePreviewSettingsForm(function (form) {
                    form.show();
                });
            }
        }
    }

    return group;
}