
StiMobileDesigner.prototype.MapPropertiesGroup = function () {
    var jsObject = this;
    var mapPropertiesGroup = this.PropertiesGroup("mapPropertiesGroup", this.loc.PropertyCategory.MapCategory);
    mapPropertiesGroup.style.display = "none";

    var props = [
        ["colorEach", this.loc.PropertyMain.ColorEach, this.CheckBox(), "colorEachMap"],
        ["displayNameType", this.loc.PropertyMain.DisplayNameType, this.PropertyDropDownList("controlPropertyMapDisplayNameType", this.options.propertyControlWidth, this.GetMapDisplayNameTypeItems(), true), "displayNameTypeMap"],
        ["shortValue", this.loc.PropertyMain.ShortValue, this.CheckBox(), "shortValueMap"],
        ["showValue", this.loc.PropertyMain.ShowValue, this.CheckBox(), "showValueMap"],
        ["showZeros", this.loc.PropertyMain.ShowZeros, this.CheckBox(), "showZerosMap"],
        ["stretch", this.loc.PropertyMain.Stretch, this.CheckBox(), "stretchMap"]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        mapPropertiesGroup.container.appendChild(this.Property(props[i].length > 3 ? props[i][3] : props[i][0], props[i][1], control));

        control.action = function () {
            var value = jsObject.GetControlValue(this);
            jsObject.ApplyPropertyValue(this.propertyName, value);
        }
    }

    return mapPropertiesGroup;
}