
StiMobileDesigner.prototype.ButtonElementPropertiesGroup = function () {
    var jsObject = this;
    var group = this.PropertiesGroup("buttonElementPropertiesGroup", this.loc.PropertyCategory.ButtonCategory);
    group.style.display = "none";
    group.innerGroups = {};

    var groupNames = [
        ["iconSet", this.loc.PropertyMain.IconSet, "main", 1]
    ]

    for (var i = 0; i < groupNames.length; i++) {
        var innerGroup = this.PropertiesGroup(null, groupNames[i][1], null, groupNames[i][3]);
        group.innerGroups[groupNames[i][0]] = innerGroup;
        innerGroup.parentGroup = groupNames[i][2] == "main" ? group : group.innerGroups[groupNames[i][2]];
    }

    var props = [
        ["buttonText", this.loc.PropertyMain.Text, this.PropertyTextBox("controlButtonText", this.options.propertyControlWidth), "main"],
        ["buttonChecked", this.loc.PropertyMain.Checked, this.CheckBox("controlButtonChecked"), "main"],
        ["buttonGroup", this.loc.PropertyMain.Group, this.PropertyTextBox("controlButtonGroup", this.options.propertyControlWidth), "main"],
        ["iconAlignment", this.loc.PropertyMain.IconAlignment, this.PropertyDropDownList("controlButtonIconAlignment", this.options.propertyControlWidth, this.GetIconAlignmentItems(true), true, false, false, true), "main", "buttonIconAlignment"],
        ["horAlignment", this.loc.PropertyMain.HorAlignment, this.PropertyDropDownList("controlButtonHorAlignment", this.options.propertyControlWidth, this.GetHorizontalAlignmentItems(), true, false, false, true), "main", "buttonHorAlignment"],
        ["vertAlignment", this.loc.PropertyMain.VertAlignment, this.PropertyDropDownList("controlButtonVertAlignment", this.options.propertyControlWidth, this.GetVerticalAlignmentItems(), true, false, false, true), "main", "buttonVertAlignment"],
        ["buttonType", this.loc.PropertyMain.Type, this.PropertyDropDownList("controlButtonType", this.options.propertyControlWidth, this.GetButtonTypeItems(), true, false, false, true), "main", "buttonType"],
        ["buttonShapeType", this.loc.PropertyMain.ShapeType, this.PropertyDropDownList("controlButtonShapeType", this.options.propertyControlWidth, this.GetButtonShapeTypeItems(), true, false, false, true), "main"],
        ["buttonStretch", this.loc.PropertyMain.Stretch, this.PropertyDropDownList("controlButtonStretch", this.options.propertyControlWidth, this.GetButtonStretchItems(), true, false, false, true), "main"],
        ["wordWrap", this.loc.PropertyMain.WordWrap, this.CheckBox("controlButtonWordWrap"), "main", "buttonWordWrap"],
        ["buttonIconSetIcon", this.loc.PropertyMain.Icon, this.IconControl("controlButtonIconSetIcon", this.options.propertyControlWidth, this.options.propertyControlsHeight, null, null, true), "iconSet"],
        ["buttonIconSetCheckedIcon", this.loc.PropertyMain.CheckedIcon, this.IconControl("controlButtonIconSetCheckedIcon", this.options.propertyControlWidth, this.options.propertyControlsHeight, null, null, true), "iconSet"],
        ["buttonIconSetUncheckedIcon", this.loc.PropertyMain.UncheckedIcon, this.IconControl("controlButtonIconSetUncheckedIcon", this.options.propertyControlWidth, this.options.propertyControlsHeight, null, null, true), "iconSet"]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        var currentGroup = props[i][3] == "main" ? group : group.innerGroups[props[i][3]];
        currentGroup.container.appendChild(this.Property(props[i].length > 4 ? props[i][4] : props[i][0], props[i][1], control, null, currentGroup.nestingLevel));

        control.action = function () {
            var value = jsObject.GetControlValue(this);

            if (this.propertyName == "buttonText") {
                value = StiBase64.encode(value);
            }

            jsObject.ApplyPropertyValue(this.propertyName, value);
        }
    }

    for (var i = 0; i < groupNames.length; i++) {
        group.innerGroups[groupNames[i][0]].parentGroup.container.appendChild(group.innerGroups[groupNames[i][0]]);
    }

    return group;
}