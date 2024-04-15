
StiMobileDesigner.prototype.GaugePropertiesGroup = function () {
    var jsObject = this;
    var group = this.PropertiesGroup("gaugePropertiesGroup", this.loc.PropertyCategory.GaugeCategory);
    group.style.display = "none";
    group.innerGroups = {};

    var props = [
        ["allowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("controlPropertyGaugeAllowApplyStyle"), "allowApplyStyleGaugeComp"],
        ["shortValue", this.loc.PropertyMain.ShortValue, this.CheckBox("controlPropertyGaugeShortValue"), "shortValueGaugeComp"]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        group.container.appendChild(this.Property(props[i].length > 3 ? props[i][3] : props[i][0], props[i][1], control, null, group.nestingLevel));

        control.action = function () {
            jsObject.ApplyPropertyValue(this.propertyName, jsObject.GetControlValue(this));
        }
    }

    return group;
}