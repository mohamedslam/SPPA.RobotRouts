
StiMobileDesigner.prototype.CheckPropertiesGroup = function () {
    var checkPropertiesGroup = this.PropertiesGroup("checkPropertiesGroup", this.loc.PropertyCategory.CheckCategory);
    checkPropertiesGroup.style.display = "none";

    //Checked
    var controlPropertyChecked = this.PropertyTextBoxWithEditButton("controlPropertyChecked", this.options.propertyControlWidth);
    controlPropertyChecked.textBox.action = function () {
        this.jsObject.ApplyPropertyValue("checked", StiBase64.encode(this.value));
    }
    controlPropertyChecked.button.action = function () {
        this.jsObject.InitializeTextEditorForm(function (textEditorForm) {
            textEditorForm.propertyName = "checked";
            textEditorForm.changeVisibleState(true);
        });
    }
    checkPropertiesGroup.container.appendChild(this.Property("checked", this.loc.PropertyMain.Checked, controlPropertyChecked));

    //CheckStyleForTrue
    var controlPropertyCheckStyleForTrue = this.PropertyDropDownList("controlPropertyCheckStyleForTrue", this.options.propertyControlWidth, this.GetCheckBoxStyleItems(), true, true);
    controlPropertyCheckStyleForTrue.action = function () {
        this.jsObject.ApplyPropertyValue("checkStyleForTrue", this.key);
    };
    checkPropertiesGroup.container.appendChild(this.Property("checkStyleForTrue", this.loc.PropertyMain.CheckStyleForTrue, controlPropertyCheckStyleForTrue))

    //CheckStyleForFalse
    var controlPropertyCheckStyleForFalse = this.PropertyDropDownList("controlPropertyCheckStyleForFalse", this.options.propertyControlWidth, this.GetCheckBoxStyleItems(), true, true);
    controlPropertyCheckStyleForFalse.action = function () {
        this.jsObject.ApplyPropertyValue("checkStyleForFalse", this.key);
    };
    checkPropertiesGroup.container.appendChild(this.Property("checkStyleForFalse", this.loc.PropertyMain.CheckStyleForFalse, controlPropertyCheckStyleForFalse))

    //Editable
    var controlPropertyCheckEditableControl = this.CheckBox("controlPropertyCheckEditable");
    controlPropertyCheckEditableControl.action = function () {
        this.jsObject.ApplyPropertyValue("editable", this.isChecked);
    }
    checkPropertiesGroup.container.appendChild(this.Property("checkEditable", this.loc.PropertyMain.Editable, controlPropertyCheckEditableControl, "Editable"));

    //Size
    var controlPropertyCheckSize = this.PropertyTextBox("controlPropertyCheckSize", this.options.propertyNumbersControlWidth);
    controlPropertyCheckSize.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue("size", this.value);
    }
    checkPropertiesGroup.container.appendChild(this.Property("checkSize", this.loc.PropertyMain.Size, controlPropertyCheckSize, "Size"));

    //Values
    var controlPropertyCheckValues = this.PropertyDropDownList("controlPropertyCheckValues", this.options.propertyControlWidth, this.GetCheckBoxValuesItems(), false, false);
    controlPropertyCheckValues.action = function () {
        this.jsObject.ApplyPropertyValue("checkValues", this.key);
    };
    checkPropertiesGroup.container.appendChild(this.Property("checkValues", this.loc.PropertyMain.Values, controlPropertyCheckValues, "Values"));

    return checkPropertiesGroup;
}