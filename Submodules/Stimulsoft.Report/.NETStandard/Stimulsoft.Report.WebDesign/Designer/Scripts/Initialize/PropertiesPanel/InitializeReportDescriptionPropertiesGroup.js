
StiMobileDesigner.prototype.ReportDescriptionPropertiesGroup = function () {
    var jsObject = this;
    var descriptionPropertiesGroup = this.PropertiesGroup("reportDescriptionPropertiesGroup", this.loc.PropertyCategory.DesignCategory);
    descriptionPropertiesGroup.style.display = "none";

    var properties = [
        ["ReportName", this.loc.PropertyMain.ReportName, this.PropertyTextBox("controlReportPropertyReportName", this.options.propertyControlWidth), "Textbox"],
        ["ReportAlias", this.loc.PropertyMain.ReportAlias, this.PropertyTextBox("controlReportPropertyReportAlias", this.options.propertyControlWidth), "Textbox"],
        ["ReportAuthor", this.loc.PropertyMain.ReportAuthor, this.PropertyTextBox("controlReportPropertyReportAuthor", this.options.propertyControlWidth), "Textbox"],
        ["ReportDescription", this.loc.PropertyMain.ReportDescription, this.PropertyTextBox("controlReportPropertyReportDescription", this.options.propertyControlWidth), "Textbox"],
        ["ReportImage", this.loc.PropertyMain.ReportImage, this.PropertyImageControl("controlReportPropertyReportImage", this.options.propertyControlWidth), "Image"]
    ]

    for (var i = 0; i < properties.length; i++) {
        var control = properties[i][2];
        control.propertyName = this.LowerFirstChar(properties[i][0]);
        control.controlType = properties[i][3];

        descriptionPropertiesGroup.container.appendChild(this.Property(control.propertyName, properties[i][1], control));
        jsObject.AddMainMethodsToPropertyControl(control);

        control.action = function () {
            var propsArray = [this.propertyName];
            jsObject.options.selectedObject.properties[this.propertyName] = this.getValue();

            if (this.propertyName == "reportName") {
                jsObject.options.selectedObject.properties.reportAlias = jsObject.options.selectedObject.properties.reportName;
                propsArray.push("reportAlias");
            }

            jsObject.SendCommandSetReportProperties(propsArray);
        }
    }

    return descriptionPropertiesGroup;
}