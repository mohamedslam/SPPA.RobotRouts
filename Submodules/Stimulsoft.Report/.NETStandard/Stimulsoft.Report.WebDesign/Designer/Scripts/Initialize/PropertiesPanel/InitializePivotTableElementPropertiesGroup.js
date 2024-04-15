
StiMobileDesigner.prototype.PivotTableElementPropertiesGroup = function () {
    var group = this.PropertiesGroup("pivotTableElementPropertiesGroup", this.loc.PropertyCategory.PivotTableCategory);
    group.style.display = "none";

    var props = [
        ["crossFiltering", this.loc.PropertyMain.CrossFiltering, this.CheckBox("controlPropertyPivotTableCrossFiltering"), "crossFilteringPivotTable"],
        ["dataTransformation", this.loc.PropertyMain.DataTransformation, this.PropertyDataTransformationControl("pivotTableElementDataTransformation", this.options.propertyControlWidth), "dataTransformationPivotTable"],
        ["group", this.loc.PropertyMain.Group, this.PropertyTextBox("controlPropertyPivotTableElementGroup", this.options.propertyControlWidth), "groupPivotTable"]
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