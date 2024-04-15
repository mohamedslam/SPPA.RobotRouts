
StiMobileDesigner.prototype.OnlineMapElementPropertiesGroup = function () {
    var group = this.PropertiesGroup("onlineMapElementPropertiesGroup", this.loc.PropertyCategory.OnlineMapCategory);
    group.style.display = "none";

    var props = [
        ["crossFiltering", this.loc.PropertyMain.CrossFiltering, this.CheckBox("onlineMapElementCrossFiltering"), "crossFilteringOnlineMap"],
        ["dataTransformation", this.loc.PropertyMain.DataTransformation, this.PropertyDataTransformationControl("onlineMapElementDataTransformation", this.options.propertyControlWidth), "dataTransformationOnlineMap"],
        ["group", this.loc.PropertyMain.Group, this.PropertyTextBox("controlPropertyOnlineMapElementGroup", this.options.propertyControlWidth), "groupOnlineMap"]
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

