
StiMobileDesigner.prototype.RegionMapElementPropertiesGroup = function () {
    var group = this.PropertiesGroup("regionMapElementPropertiesGroup", this.loc.PropertyCategory.RegionMapCategory);
    group.style.display = "none";

    var props = [
        ["crossFiltering", this.loc.PropertyMain.CrossFiltering, this.CheckBox("controlPropertyRegionMapElementCrossFiltering"), "crossFilteringRegionMap"],
        ["dataTransformation", this.loc.PropertyMain.DataTransformation, this.PropertyDataTransformationControl("regionMapElementDataTransformation", this.options.propertyControlWidth), "dataTransformationRegionMap"],
        ["group", this.loc.PropertyMain.Group, this.PropertyTextBox("controlPropertyRegionMapElementGroup", this.options.propertyControlWidth), "groupRegionMap"],
        ["shortValue", this.loc.PropertyMain.ShortValue, this.CheckBox("controlPropertyRegionMapElementShortValue"), "shortValueRegionMap"],
        ["showValue", this.loc.PropertyMain.ShowValue, this.CheckBox("controlPropertyRegionMapElementShowValue"), "showValueRegionMap"],
        ["showZeros", this.loc.PropertyMain.ShowZeros, this.CheckBox("controlPropertyRegionMapElementShowZeros"), "showZerosRegionMap"]
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

