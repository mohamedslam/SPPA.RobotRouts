
StiMobileDesigner.prototype.ImageElementPropertiesGroup = function () {
    var jsObject = this;
    var group = this.PropertiesGroup("imageElementPropertiesGroup", this.loc.PropertyCategory.ImageCategory);
    group.style.display = "none";

    var props = [
        ["crossFiltering", this.loc.PropertyMain.CrossFiltering, this.CheckBox("imageElementCrossFiltering"), "crossFilteringImage"],
        ["group", this.loc.PropertyMain.Group, this.PropertyTextBox("imageElementGroup", this.options.propertyControlWidth), "groupImage"],
        ["horAlignment", this.loc.PropertyMain.HorAlignment, this.PropertyDropDownList("imageElementImageHorizontalAlignment", this.options.propertyControlWidth, this.GetHorizontalAlignmentItems(true)), "imageElementHorAlignment"],
        ["vertAlignment", this.loc.PropertyMain.VertAlignment, this.PropertyDropDownList("imageElementImageVerticalAlignment", this.options.propertyControlWidth, this.GetVerticalAlignmentItems()), "imageElementVertAlignment"],
        ["ratio", this.loc.PropertyMain.AspectRatio, this.CheckBox("imageElementImageAspectRatio"), "imageElementAspectRatio"]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        group.container.appendChild(this.Property(props[i].length > 3 ? props[i][3] : props[i][0], props[i][1], control));

        control.action = function () {
            jsObject.ApplyPropertyValue(this.propertyName, jsObject.GetControlValue(this));
        }
    }

    return group;
}