
StiMobileDesigner.prototype.ShapeTextPropertiesGroup = function () {
    var jsObject = this;
    var group = this.PropertiesGroup("shapeTextPropertiesGroup", this.loc.PropertyCategory.TextCategory);
    group.style.display = "none";

    var props = [
        ["shapeText", this.loc.PropertyMain.Text, this.PropertyTextBox("controlPropertyShapeText", this.options.propertyControlWidth)],
        ["textMargins", this.loc.PropertyMain.Margins, this.PropertyMarginsControl("controlPropertyShapeTextMargins", this.options.propertyControlWidth + 61), "shapeTextMargins"],
        ["horAlignment", this.loc.PropertyMain.HorAlignment, this.PropertyEnumExpressionControl("controlPropertyShapeHorizontalAlignment", this.options.propertyControlWidth, this.GetHorizontalAlignmentItems(true), true, false), "shapeHorAlignment"],
        ["vertAlignment", this.loc.PropertyMain.VertAlignment, this.PropertyEnumExpressionControl("controlPropertyShapeVerticalAlignment", this.options.propertyControlWidth, this.GetVerticalAlignmentItems(), true, false), "shapeVertAlignment"],
        ["foreColor", this.loc.PropertyMain.TextColor, this.PropertyColorExpressionControl("controlPropertyShapeForeColor", null, this.options.propertyControlWidth), "shapeForeColor"]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        group.container.appendChild(this.Property(props[i].length > 3 ? props[i][3] : props[i][0], props[i][1], control));

        if (control.propertyName == "horAlignment" || control.propertyName == "vertAlignment" || control.propertyName == "foreColor") {
            control.action = function () {
                jsObject.ApplyPropertyExpressionControlValue(this.propertyName, this.key, this.expression);
            }
        }
        else {
            control.action = function () {
                jsObject.ApplyPropertyValue(this.propertyName, this.propertyName == "shapeText" ? StiBase64.encode(this.value) : jsObject.GetControlValue(this));
            }
        }
    }

    return group;
}