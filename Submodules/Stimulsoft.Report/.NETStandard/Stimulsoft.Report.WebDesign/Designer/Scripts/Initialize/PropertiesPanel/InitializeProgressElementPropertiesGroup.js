
StiMobileDesigner.prototype.ProgressElementPropertiesGroup = function () {
    var group = this.PropertiesGroup("progressElementPropertiesGroup", this.loc.PropertyCategory.ProgressCategory);
    group.style.display = "none";

    var props = [
        ["colorEach", this.loc.PropertyMain.ColorEach, this.CheckBox("controlPropertyProgressElementColorEach")],
        ["crossFiltering", this.loc.PropertyMain.CrossFiltering, this.CheckBox("controlPropertyProgressElementCrossFiltering"), "crossFilteringProgress"],
        ["dataTransformation", this.loc.PropertyMain.DataTransformation, this.PropertyDataTransformationControl("progressElementDataTransformation", this.options.propertyControlWidth), "dataTransformationProgress"],
        ["group", this.loc.PropertyMain.Group, this.PropertyTextBox("controlPropertyProgressElementGroup", this.options.propertyControlWidth), "groupProgress"]
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
        else {
            control.action = function () {
                if (this.propertyName == "mode" && this.jsObject.options.selectedObject) {
                    this.jsObject.RemoveStylesFromCache(this.jsObject.options.selectedObject.properties.name);
                }
                this.jsObject.ApplyPropertyValue([this.propertyName], [this.jsObject.GetControlValue(this)]);
            }
        }
    }

    return group;
}

