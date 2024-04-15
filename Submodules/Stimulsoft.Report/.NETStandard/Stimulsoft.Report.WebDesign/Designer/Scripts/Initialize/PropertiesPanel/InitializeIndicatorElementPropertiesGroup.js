
StiMobileDesigner.prototype.IndicatorElementPropertiesGroup = function () {
    var group = this.PropertiesGroup("indicatorElementPropertiesGroup", this.loc.PropertyCategory.IndicatorCategory);
    group.style.display = "none";

    var customIconControl = this.ImageControl("controlPropertyIndicatorElementCustomIcon", this.options.propertyControlWidth + 4, 70);
    customIconControl.style.borderStyle = "solid";

    var props = [
        ["crossFiltering", this.loc.PropertyMain.CrossFiltering, this.CheckBox("controlPropertyIndicatorElementCrossFiltering"), "crossFilteringIndicator"],
        ["dataTransformation", this.loc.PropertyMain.DataTransformation, this.PropertyDataTransformationControl("indicatorElementDataTransformation", this.options.propertyControlWidth), "dataTransformationIndicator"],
        ["group", this.loc.PropertyMain.Group, this.PropertyTextBox("controlPropertyIndicatorElementGroup", this.options.propertyControlWidth), "groupIndicator"],
        ["targetMode", this.loc.PropertyMain.TargetMode, this.PropertyDropDownList("controlPropertyIndicatorElementTargetMode", this.options.propertyControlWidth, this.GetTargetModeItems(), true, false, false, true)]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        group.container.appendChild(this.Property(props[i].length > 3 ? props[i][3] : props[i][0], props[i][1], control));

        if (props[i][0] == "textFormat") {
            control.button.action = function () {
                this.jsObject.InitializeTextFormatForm(function (textFormatForm) {
                    textFormatForm.show();
                });
            }
        }
        else if (props[i][0] == "customIcon") {
            control.action = function () {
                this.jsObject.ApplyPropertyValue(["customIcon"], [this.src]);
            }
        }
        else {
            control.action = function () {
                if ((this.propertyName == "icon" || this.propertyName == "iconSet") && this.jsObject.options.selectedObject) {
                    this.jsObject.RemoveStylesFromCache(this.jsObject.options.selectedObject.properties.name);
                }
                this.jsObject.ApplyPropertyValue(this.propertyName, this.jsObject.GetControlValue(this));
            }
        }
    }

    return group;
}

