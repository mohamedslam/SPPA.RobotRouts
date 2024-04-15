
StiMobileDesigner.prototype.TextElementPropertiesGroup = function () {
    var jsObject = this;
    var group = this.PropertiesGroup("textElementPropertiesGroup", this.loc.PropertyCategory.TextCategory);
    group.style.display = "none";

    var props = [
        ["crossFiltering", this.loc.PropertyMain.CrossFiltering, this.CheckBox("textElementCrossFiltering"), "crossFilteringText"],
        ["group", this.loc.PropertyMain.Group, this.PropertyTextBox("textElementGroup", this.options.propertyControlWidth), "groupText"],
        ["text", this.loc.PropertyMain.Text, this.PropertyTextBoxWithEditButton("textElementText", this.options.propertyControlWidth, true), "textElementText"],
        ["textSizeMode", this.loc.PropertyMain.SizeMode, this.PropertyDropDownList("textElementSizeMode", this.options.propertyControlWidth, this.GetTextElementSizeModeItems(), true)]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        group.container.appendChild(this.Property(props[i].length > 3 ? props[i][3] : props[i][0], props[i][1], control));

        if (control.propertyName == "text") {
            control.button.action = function () {
                jsObject.ShowComponentForm(jsObject.options.selectedObject);
            }
        }
        else {
            control.action = function () {
                jsObject.ApplyPropertyValue(this.propertyName, jsObject.GetControlValue(this));
            }
        }
    }

    return group;
}

