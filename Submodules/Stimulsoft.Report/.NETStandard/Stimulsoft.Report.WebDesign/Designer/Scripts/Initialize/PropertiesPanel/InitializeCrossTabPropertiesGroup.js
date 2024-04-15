
StiMobileDesigner.prototype.CrossTabPropertiesGroup = function () {
    var crossTabPropertiesGroup = this.PropertiesGroup("crossTabPropertiesGroup", this.loc.PropertyCategory.CrossTabCategory);
    crossTabPropertiesGroup.style.display = "none";

    //EmptyValue
    var controlPropertyCrossTabEmptyValue = this.PropertyTextBox("controlPropertyCrossTabEmptyValue", this.options.propertyControlWidth);
    controlPropertyCrossTabEmptyValue.action = function () {
        this.jsObject.ApplyPropertyValue("crossTabEmptyValue", StiBase64.encode(this.value));
    }
    crossTabPropertiesGroup.container.appendChild(this.Property("crossTabEmptyValue",
        this.loc.PropertyMain.EmptyValue, controlPropertyCrossTabEmptyValue, "EmptyValue"));

    //CrossTabHorAlign
    var controlPropertyCrossTabHorAlign = this.PropertyDropDownList("controlPropertyCrossTabHorAlign", this.options.propertyControlWidth, this.GetCrossTabHorAlignItems(), true, false);
    controlPropertyCrossTabHorAlign.action = function () {
        this.jsObject.ApplyPropertyValue("crossTabHorAlign", this.key);
    }
    crossTabPropertiesGroup.container.appendChild(this.Property("crossTabHorAlign",
        this.loc.PropertyMain.HorAlignment, controlPropertyCrossTabHorAlign, "HorAlignment"));

    //PrintIfEmpty
    var controlPropertyCrossTabPrintIfEmpty = this.CheckBox("controlPropertyCrossTabPrintIfEmpty");
    controlPropertyCrossTabPrintIfEmpty.action = function () {
        this.jsObject.ApplyPropertyValue("printIfEmpty", this.isChecked);
    }
    crossTabPropertiesGroup.container.appendChild(this.Property("crossTabPrintIfEmpty",
        this.loc.PropertyMain.PrintIfEmpty, controlPropertyCrossTabPrintIfEmpty, "PrintIfEmpty"));

    //RightToLeft
    var controlPropertyCrossTabRightToLeft = this.CheckBox("controlPropertyCrossTabRightToLeft");
    controlPropertyCrossTabRightToLeft.action = function () {
        this.jsObject.ApplyPropertyValue("rightToLeft", this.isChecked);
    }
    crossTabPropertiesGroup.container.appendChild(this.Property("crossTabRightToLeft",
        this.loc.PropertyMain.RightToLeft, controlPropertyCrossTabRightToLeft, "RightToLeft"));

    //Wrap
    var controlPropertyCrossTabWrap = this.CheckBox("controlPropertyCrossTabWrap");
    controlPropertyCrossTabWrap.action = function () {
        this.jsObject.ApplyPropertyValue("crossTabWrap", this.isChecked);
    }
    crossTabPropertiesGroup.container.appendChild(this.Property("crossTabWrap",
        this.loc.PropertyMain.Wrap, controlPropertyCrossTabWrap, "Wrap"));

    //WrapGap
    var controlPropertyCrossTabWrapGap = this.PropertyTextBox("controlPropertyCrossTabWrapGap", this.options.propertyNumbersControlWidth);
    controlPropertyCrossTabWrapGap.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue("crossTabWrapGap", this.value);
    }
    crossTabPropertiesGroup.container.appendChild(this.Property("crossTabWrapGap",
        this.loc.PropertyMain.WrapGap, controlPropertyCrossTabWrapGap, "WrapGap"));


    return crossTabPropertiesGroup;
}