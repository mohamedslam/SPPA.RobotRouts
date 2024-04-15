
StiMobileDesigner.prototype.TableElementPropertiesGroup = function () {
    var jsObject = this;
    var group = this.PropertiesGroup("tableElementPropertiesGroup", this.loc.PropertyCategory.TableCategory);
    group.style.display = "none";

    var props = [
        ["crossFiltering", this.loc.PropertyMain.CrossFiltering, this.CheckBox("controlPropertyTableElementCrossFiltering"), "crossFilteringTable"],
        ["dataTransformation", this.loc.PropertyMain.DataTransformation, this.PropertyDataTransformationControl("tableElementDataTransformation", this.options.propertyControlWidth), "dataTransformationTable"],
        ["frozenColumns", this.loc.PropertyMain.FrozenColumns, this.PropertyTextBox("controlPropertyTableElementFrozenColumns", this.options.propertyControlWidth)],
        ["group", this.loc.PropertyMain.Group, this.PropertyTextBox("controlPropertyTableElementGroup", this.options.propertyControlWidth), "groupTable"],
        ["sizeMode", this.loc.PropertyMain.SizeMode, this.PropertyDropDownList("controlPropertyTableElementSizeMode", this.options.propertyControlWidth, this.GetTableElementSizeModeItems(), true)]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        group.container.appendChild(this.Property(props[i].length > 3 ? props[i][3] : props[i][0], props[i][1], control));

        control.action = function () {
            if (this.propertyName == "frozenColumns") {
                this.value = jsObject.StrToCorrectPositiveInt(this.value);
            }
            jsObject.ApplyPropertyValue(this.propertyName, jsObject.GetControlValue(this));
        }
    }

    return group;
}