
StiMobileDesigner.prototype.ChartAdditionalPropertiesGroup = function () {
    var jsObject = this;
    var group = this.PropertiesGroup("chartAdditionalPropertiesGroup", this.loc.PropertyCategory.ChartAdditionalCategory);
    group.style.display = "none";

    var props = [
        ["allowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("controlPropertyChartAllowApplyStyle")],
        ["horizontalSpacing", this.loc.PropertyMain.HorSpacing, this.PropertyTextBox("controlPropertyHorSpacing", this.options.propertyNumbersControlWidth)],
        ["processAtEnd", this.loc.PropertyMain.ProcessAtEnd, this.CheckBox("controlPropertyChartProcessAtEnd")],
        ["chartRotation", this.loc.PropertyMain.Rotation, this.PropertyDropDownList("controlPropertyChartRotation", this.options.propertyControlWidth, this.GetImageRotationItems(), true, false)],
        ["verticalSpacing", this.loc.PropertyMain.VertSpacing, this.PropertyTextBox("controlPropertyVertSpacing", this.options.propertyNumbersControlWidth)]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        group.container.appendChild(this.Property(props[i][0], props[i][1], control));

        control.action = function () {
            jsObject.ApplyPropertyValue(this.propertyName, jsObject.GetControlValue(this));
        }
    }

    return group;
}