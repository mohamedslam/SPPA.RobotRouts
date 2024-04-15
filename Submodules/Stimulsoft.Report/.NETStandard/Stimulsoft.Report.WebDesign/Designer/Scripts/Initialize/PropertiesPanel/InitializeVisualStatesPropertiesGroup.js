
StiMobileDesigner.prototype.VisualStatesPropertiesGroup = function () {
    var jsObject = this;
    var group = this.PropertiesGroup("visualStatesPropertiesGroup", this.loc.PropertyMain.VisualStates, null, 1);
    group.style.display = "none";
    group.innerGroups = {};

    var groupNames = [
        ["visualStateChecked", this.loc.Report.VisualChecked, "main", 2],
        ["visualStateHover", this.loc.Report.VisualHover, "main", 2],
        ["visualStatePressed", this.loc.Report.VisualPressed, "main", 2],
        ["visualStateCheckedIconSet", this.loc.PropertyMain.IconSet, "visualStateChecked", 3],
        ["visualStateHoverIconSet", this.loc.PropertyMain.IconSet, "visualStateHover", 3],
        ["visualStatePressedIconSet", this.loc.PropertyMain.IconSet, "visualStatePressed", 3]
    ]

    for (var i = 0; i < groupNames.length; i++) {
        var innerGroup = this.PropertiesGroup(null, groupNames[i][1], null, groupNames[i][3]);
        group.innerGroups[groupNames[i][0]] = innerGroup;
        innerGroup.parentGroup = groupNames[i][2] == "main" ? group : group.innerGroups[groupNames[i][2]];
    }

    var props = [
        ["visualStatesCheckBorder", this.loc.PropertyMain.Border, this.PropertyComplexBorderControl("controlPropertyVSCheckedBorder", this.options.propertyControlWidth), "visualStateChecked"],
        ["visualStatesCheckBrush", this.loc.PropertyMain.Brush, this.PropertyBrushExpressionControl("controlPropertyVSCheckedBrush", null, this.options.propertyControlWidth, true, true), "visualStateChecked"],
        ["visualStatesCheckFont", this.loc.PropertyMain.Font, this.PropertyComplexFontControl("controlPropertyVSCheckedBorder", this.options.propertyControlWidth), "visualStateChecked"],
        ["visualStatesCheckIconBrush", this.loc.PropertyMain.IconBrush, this.PropertyBrushExpressionControl("controlPropertyVSCheckedIconBrush", null, this.options.propertyControlWidth, true, true), "visualStateChecked"],
        ["visualStatesCheckTextBrush", this.loc.PropertyMain.TextBrush, this.PropertyBrushExpressionControl("controlPropertyVSCheckedTextBrush", null, this.options.propertyControlWidth, true, true), "visualStateChecked"],
        ["visualStatesCheckIconSetIcon", this.loc.PropertyMain.Icon, this.IconControl("controlVSCheckedIconSetIcon", this.options.propertyControlWidth, this.options.propertyControlsHeight, null, null, true), "visualStateCheckedIconSet"],
        ["visualStatesCheckIconSetCheckedIcon", this.loc.PropertyMain.CheckedIcon, this.IconControl("controlVSCheckedIconSetCheckedIcon", this.options.propertyControlWidth, this.options.propertyControlsHeight, null, null, true), "visualStateCheckedIconSet"],
        ["visualStatesCheckIconSetUncheckedIcon", this.loc.PropertyMain.UncheckedIcon, this.IconControl("controlVSCheckedIconSetUncheckedIcon", this.options.propertyControlWidth, this.options.propertyControlsHeight, null, null, true), "visualStateCheckedIconSet"],

        ["visualStatesHoverBorder", this.loc.PropertyMain.Border, this.PropertyComplexBorderControl("controlPropertyVSHoverBorder", this.options.propertyControlWidth), "visualStateHover"],
        ["visualStatesHoverBrush", this.loc.PropertyMain.Brush, this.PropertyBrushExpressionControl("controlPropertyVSHoverBrush", null, this.options.propertyControlWidth, true, true), "visualStateHover"],
        ["visualStatesHoverFont", this.loc.PropertyMain.Font, this.PropertyComplexFontControl("controlPropertyVSHoverBorder", this.options.propertyControlWidth), "visualStateHover"],
        ["visualStatesHoverIconBrush", this.loc.PropertyMain.IconBrush, this.PropertyBrushExpressionControl("controlPropertyVSHoverIconBrush", null, this.options.propertyControlWidth, true, true), "visualStateHover"],
        ["visualStatesHoverTextBrush", this.loc.PropertyMain.TextBrush, this.PropertyBrushExpressionControl("controlPropertyVSHoverTextBrush", null, this.options.propertyControlWidth, true, true), "visualStateHover"],
        ["visualStatesHoverIconSetIcon", this.loc.PropertyMain.Icon, this.IconControl("controlVSHoverIconSetIcon", this.options.propertyControlWidth, this.options.propertyControlsHeight, null, null, true), "visualStateHoverIconSet"],
        ["visualStatesHoverIconSetCheckedIcon", this.loc.PropertyMain.CheckedIcon, this.IconControl("controlVSHoverIconSetCheckedIcon", this.options.propertyControlWidth, this.options.propertyControlsHeight, null, null, true), "visualStateHoverIconSet"],
        ["visualStatesHoverIconSetUncheckedIcon", this.loc.PropertyMain.UncheckedIcon, this.IconControl("controlVSHoverIconSetUncheckedIcon", this.options.propertyControlWidth, this.options.propertyControlsHeight, null, null, true), "visualStateHoverIconSet"],

        ["visualStatesPressedBorder", this.loc.PropertyMain.Border, this.PropertyComplexBorderControl("controlPropertyVSPressedBorder", this.options.propertyControlWidth), "visualStatePressed"],
        ["visualStatesPressedBrush", this.loc.PropertyMain.Brush, this.PropertyBrushExpressionControl("controlPropertyVSPressedBrush", null, this.options.propertyControlWidth, true, true), "visualStatePressed"],
        ["visualStatesPressedFont", this.loc.PropertyMain.Font, this.PropertyComplexFontControl("controlPropertyVSPressedBorder", this.options.propertyControlWidth), "visualStatePressed"],
        ["visualStatesPressedIconBrush", this.loc.PropertyMain.IconBrush, this.PropertyBrushExpressionControl("controlPropertyVSPressedIconBrush", null, this.options.propertyControlWidth, true, true), "visualStatePressed"],
        ["visualStatesPressedTextBrush", this.loc.PropertyMain.TextBrush, this.PropertyBrushExpressionControl("controlPropertyVSPressedTextBrush", null, this.options.propertyControlWidth, true, true), "visualStatePressed"],
        ["visualStatesPressedIconSetIcon", this.loc.PropertyMain.Icon, this.IconControl("controlVSPressedIconSetIcon", this.options.propertyControlWidth, this.options.propertyControlsHeight, null, null, true), "visualStatePressedIconSet"],
        ["visualStatesPressedIconSetCheckedIcon", this.loc.PropertyMain.CheckedIcon, this.IconControl("controlVSPressedIconSetCheckedIcon", this.options.propertyControlWidth, this.options.propertyControlsHeight, null, null, true), "visualStatePressedIconSet"],
        ["visualStatesPressedIconSetUncheckedIcon", this.loc.PropertyMain.UncheckedIcon, this.IconControl("controlVSPressedIconSetUncheckedIcon", this.options.propertyControlWidth, this.options.propertyControlsHeight, null, null, true), "visualStatePressedIconSet"]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        var currentGroup = props[i][3] == "main" ? group : group.innerGroups[props[i][3]];
        currentGroup.container.appendChild(this.Property(props[i].length > 4 ? props[i][4] : props[i][0], props[i][1], control, null, currentGroup.nestingLevel));

        control.action = function () {
            jsObject.ApplyPropertyValue(this.propertyName, jsObject.GetControlValue(this));
        }
    }

    for (var i = 0; i < groupNames.length; i++) {
        group.innerGroups[groupNames[i][0]].parentGroup.container.appendChild(group.innerGroups[groupNames[i][0]]);
    }

    return group;

}