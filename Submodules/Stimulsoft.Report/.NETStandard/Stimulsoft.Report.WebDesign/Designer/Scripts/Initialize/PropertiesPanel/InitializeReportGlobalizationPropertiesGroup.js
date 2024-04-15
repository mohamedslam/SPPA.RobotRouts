
StiMobileDesigner.prototype.ReportGlobalizationPropertiesGroup = function () {
    var group = this.PropertiesGroup("reportGlobalizationPropertiesGroup", this.loc.PropertyCategory.GlobalizationCategory);
    group.style.display = "none";

    var properties = [];
    properties.push(["AutoLocalizeReportOnRun", this.loc.PropertyMain.AutoLocalizeReportOnRun, this.CheckBox("controlReportPropertyAutoLocalizeReportOnRun"), "Checkbox"]);
    properties.push(["Culture", this.loc.PropertyMain.Culture, this.PropertyCultureControl("controlReportPropertyCulture", this.options.propertyControlWidth), "DropdownList"]);
    properties.push(["GlobalizationStrings", this.loc.PropertyMain.GlobalizationStrings, this.PropertyTextBoxWithEditButton("controlReportPropertyGlobalizationStrings", this.options.propertyControlWidth, true), "TextBoxWithEditButton"]);
    var jsObject = this;

    for (var i = 0; i < properties.length; i++) {
        var control = properties[i][2];
        control.propertyName = this.LowerFirstChar(properties[i][0]);
        control.controlType = properties[i][3];
        group.container.appendChild(this.Property(control.propertyName, properties[i][1], control));
        jsObject.AddMainMethodsToPropertyControl(control);

        control.action = function () {
            jsObject.options.selectedObject.properties[this.propertyName] = this.getValue();
            jsObject.SendCommandSetReportProperties([this.propertyName]);

            if (this.propertyName == "culture") {
                jsObject.SendCommandRepaintAllDbsElements();
            }
        }

        if (control.propertyName == "globalizationStrings") {
            control.textBox.value = "(" + jsObject.loc.PropertyMain.GlobalizationStrings + ")";
            control.button.action = function () {
                jsObject.SendCommandGetGlobalizationStrings(function (answer) {
                    jsObject.InitializeGlobalizationEditorForm(function (globalizationEditorForm) {
                        globalizationEditorForm.show(answer);
                    })
                });
            }
        }
    }

    return group;
}