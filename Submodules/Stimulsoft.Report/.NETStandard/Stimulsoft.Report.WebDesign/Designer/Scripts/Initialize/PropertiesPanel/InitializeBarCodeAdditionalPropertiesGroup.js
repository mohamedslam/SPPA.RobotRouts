
StiMobileDesigner.prototype.BarCodeAdditionalPropertiesGroup = function () {
    var barCodeAdditionalPropertiesGroup = this.PropertiesGroup("barCodeAdditionalPropertiesGroup", this.loc.PropertyCategory.BarCodeAdditionalCategory);
    barCodeAdditionalPropertiesGroup.style.display = "none";

    //Horizontal Alignment
    var controlPropertyHorAlign = this.PropertyEnumExpressionControl("controlPropertyBarCodeHorizontalAlignment", this.options.propertyControlWidth, this.GetHorizontalAlignmentItems(true), true, false);
    controlPropertyHorAlign.action = function () {
        this.jsObject.ApplyPropertyExpressionControlValue("horAlignment", this.key, this.expression);
    }
    barCodeAdditionalPropertiesGroup.container.appendChild(this.Property("barCodeHorizontalAlignment", this.loc.PropertyMain.HorAlignment, controlPropertyHorAlign, "HorAlignment"));

    //Vertical Alignment
    var controlPropertyVertAlign = this.PropertyEnumExpressionControl("controlPropertyBarCodeVerticalAlignment", this.options.propertyControlWidth, this.GetVerticalAlignmentItems(), true, false);
    controlPropertyVertAlign.action = function () {
        this.jsObject.ApplyPropertyExpressionControlValue("vertAlignment", this.key, this.expression);
    }
    barCodeAdditionalPropertiesGroup.container.appendChild(this.Property("barCodeVerticalAlignment", this.loc.PropertyMain.VertAlignment, controlPropertyVertAlign, "VertAlignment"));

    return barCodeAdditionalPropertiesGroup;
}