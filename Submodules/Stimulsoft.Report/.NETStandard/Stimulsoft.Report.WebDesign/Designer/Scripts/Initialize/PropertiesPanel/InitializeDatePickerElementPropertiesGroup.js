
StiMobileDesigner.prototype.DatePickerElementPropertiesGroup = function () {
    var group = this.PropertiesGroup("datePickerElementPropertiesGroup", this.loc.PropertyCategory.DatePickerCategory);
    group.style.display = "none";

    var props = [
        ["dataTransformation", this.loc.PropertyMain.DataTransformation, this.PropertyDataTransformationControl("datePickerElementDataTransformation", this.options.propertyControlWidth), "dataTransformationDatePicker"],
        ["group", this.loc.PropertyMain.Group, this.PropertyTextBox("controlPropertyDatePickerElementGroup", this.options.propertyControlWidth), "groupDatePicker"]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        group.container.appendChild(this.Property(props[i].length > 3 ? props[i][3] : props[i][0], props[i][1], control));

        control.action = function () {
            this.jsObject.ApplyPropertyValue(this.propertyName, this.jsObject.GetControlValue(this));
        }
    }

    return group;
}

