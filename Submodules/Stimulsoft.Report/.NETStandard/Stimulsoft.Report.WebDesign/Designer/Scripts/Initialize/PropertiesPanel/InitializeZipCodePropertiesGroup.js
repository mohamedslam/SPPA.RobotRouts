
StiMobileDesigner.prototype.ZipCodePropertiesGroup = function () {
    var zipCodePropertiesGroup = this.PropertiesGroup("zipCodePropertiesGroup", this.loc.PropertyCategory.ZipCodeCategory);
    zipCodePropertiesGroup.style.display = "none";

    //Code
    var controlPropertyCode = this.PropertyTextBoxWithEditButton("controlPropertyZipCode", this.options.propertyControlWidth);
    controlPropertyCode.textBox.action = function () {
        this.jsObject.ApplyPropertyValue("code", StiBase64.encode(this.value));
    }
    controlPropertyCode.button.action = function () {
        this.jsObject.InitializeTextEditorForm(function (textEditorForm) {
            textEditorForm.propertyName = "code";
            textEditorForm.changeVisibleState(true);
        });
    }
    zipCodePropertiesGroup.container.appendChild(this.Property("zipCode", this.loc.PropertyMain.Code, controlPropertyCode, "Code"));

    //Ratio
    var controlPropertyZipCodeRatioControl = this.CheckBox("controlPropertyZipCodeRatio");
    controlPropertyZipCodeRatioControl.action = function () {
        this.jsObject.ApplyPropertyValue("ratio", this.isChecked);
    }
    zipCodePropertiesGroup.container.appendChild(this.Property("zipCodeRatio", this.loc.PropertyMain.Ratio, controlPropertyZipCodeRatioControl, "Ratio"));

    //Size
    var controlPropertyZipCodeSize = this.PropertyTextBox("controlPropertyZipCodeSize", this.options.propertyNumbersControlWidth);
    controlPropertyZipCodeSize.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue("size", this.value);
    }
    zipCodePropertiesGroup.container.appendChild(this.Property("zipCodeSize", this.loc.PropertyMain.Size, controlPropertyZipCodeSize, "Size"));

    //SpaceRatio
    var controlPropertyZipCodeSpaceRatio = this.PropertyTextBox("controlPropertyZipCodeSpaceRatio", this.options.propertyNumbersControlWidth);
    controlPropertyZipCodeSpaceRatio.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue("spaceRatio", this.value);
    }
    zipCodePropertiesGroup.container.appendChild(this.Property("spaceRatio", this.loc.PropertyMain.SpaceRatio, controlPropertyZipCodeSpaceRatio));

    //UpperMarks
    var controlPropertyZipCodeUpperMarksControl = this.CheckBox("controlPropertyZipCodeUpperMarks");
    controlPropertyZipCodeUpperMarksControl.action = function () {
        this.jsObject.ApplyPropertyValue("upperMarks", this.isChecked);
    }
    zipCodePropertiesGroup.container.appendChild(this.Property("upperMarks", this.loc.PropertyMain.UpperMarks, controlPropertyZipCodeUpperMarksControl));

    return zipCodePropertiesGroup;
}