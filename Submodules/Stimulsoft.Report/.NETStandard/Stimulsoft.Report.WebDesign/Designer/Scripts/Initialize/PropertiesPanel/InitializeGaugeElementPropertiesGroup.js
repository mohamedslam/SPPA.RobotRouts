
StiMobileDesigner.prototype.GaugeElementPropertiesGroup = function () {
    var group = this.PropertiesGroup("gaugeElementPropertiesGroup", this.loc.PropertyCategory.GaugeCategory);
    group.style.display = "none";
    group.innerGroups = {};

    var groupNames = [
        ["labels", this.loc.PropertyCategory.LabelsCategory, "main", 1],
        ["targetSettings", this.loc.PropertyMain.TargetSettings, "main", 1]
    ]

    for (var i = 0; i < groupNames.length; i++) {
        var innerGroup = this.PropertiesGroup(null, groupNames[i][1], null, groupNames[i][3]);
        group.innerGroups[groupNames[i][0]] = innerGroup;
        innerGroup.parentGroup = groupNames[i][2] == "main" ? group : group.innerGroups[groupNames[i][2]];
    }

    var props = [
        ["crossFiltering", this.loc.PropertyMain.CrossFiltering, this.CheckBox("controlPropertyGaugeElementCrossFiltering"), "main", "crossFilteringGauge"],
        ["dataTransformation", this.loc.PropertyMain.DataTransformation, this.PropertyDataTransformationControl("gaugeElementDataTransformation", this.options.propertyControlWidth), "main", "dataTransformationGauge"],
        ["group", this.loc.PropertyMain.Group, this.PropertyTextBox("controlPropertyGaugeElementGroup", this.options.propertyControlWidth), "main", "groupGauge"],
        ["labelsVisible", this.loc.PropertyMain.Visible, this.CheckBox("controlPropertyGaugeElementLabelsVisible"), "labels", "labelsVisibleGauge"],
        ["labelsPlacement", this.loc.PropertyMain.Placement, this.PropertyDropDownList("controlPropertyGaugeElementLabelsPlacement", this.options.propertyControlWidth, this.GetXAxisTitlePositionItems(), true), "labels", "labelsPlacementGauge"],
        ["targetSettingsShowLabel", this.loc.PropertyMain.ShowLabel, this.CheckBox("gaugeElementTargetShowLabel"), "targetSettings"],
        ["targetSettingsPlacement", this.loc.PropertyMain.Placement, this.PropertyDropDownList("gaugeElementTargetPlacement", this.options.propertyControlWidth, this.GetTargetPlacementPositionItems(), true), "targetSettings"]

    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        var currentGroup = props[i][3] == "main" ? group : group.innerGroups[props[i][3]];
        currentGroup.container.appendChild(this.Property(props[i].length > 4 ? props[i][4] : props[i][0], props[i][1], control, null, currentGroup.nestingLevel));

        control.action = function () {
            if (this.propertyName == "type" && this.jsObject.options.selectedObject) {
                this.jsObject.RemoveStylesFromCache(this.jsObject.options.selectedObject.properties.name);
            }
            if (this.propertyName == "minimum" || this.propertyName == "maximum") {
                this.value = Math.abs(this.jsObject.StrToDouble(this.value));
            }
            this.jsObject.ApplyPropertyValue(this.propertyName, this.jsObject.GetControlValue(this));
        }
    }

    for (var i = 0; i < groupNames.length; i++) {
        group.innerGroups[groupNames[i][0]].parentGroup.container.appendChild(group.innerGroups[groupNames[i][0]]);
    }

    return group;
}

