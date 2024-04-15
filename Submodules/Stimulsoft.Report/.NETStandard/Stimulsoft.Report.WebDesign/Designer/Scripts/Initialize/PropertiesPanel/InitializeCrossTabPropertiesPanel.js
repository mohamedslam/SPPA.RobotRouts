
StiMobileDesigner.prototype.CrossTabPropertiesPanel = function () {
    var panel = document.createElement("div");
    panel.jsObject = this;
    panel.style.display = "none";

    var groupNames = [
        ["Data", this.loc.PropertyMain.Data],
        ["Text", this.loc.PropertyMain.Text],
        ["ImageAdditional", this.loc.PropertyCategory.ImageAdditionalCategory],
        ["TextAdditional", this.loc.PropertyCategory.TextAdditionalCategory],
        ["Position", this.loc.PropertyMain.Position],
        ["Appearance", this.loc.PropertyCategory.AppearanceCategory],
        ["Behavior", this.loc.PropertyCategory.BehaviorCategory]
    ]

    this.AddGroupsToPropertiesPanel(groupNames, panel);
    panel.groups.Data.changeOpenedState(true);
    panel.groups.Text.changeOpenedState(true);

    var properties = [
        ["text", this.loc.PropertyMain.Text, this.PropertyExpressionControl(null, this.options.propertyControlWidth), panel.groups["Text"], "Expression"],
        ["showPercents", this.loc.PropertyMain.ShowPercents, this.CheckBox(null), panel.groups["Data"], "Checkbox"],
        ["summary", this.loc.PropertyMain.Summary,
            this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetSummaryTypeForCrossTabFiledItems(), true, false), panel.groups["Data"], "DropdownList"],
        ["summaryValues", this.loc.PropertyMain.SummaryValues,
            this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetSummaryValuesItems(), true, false), panel.groups["Data"], "DropdownList"],
        ["displayValue", this.loc.PropertyMain.DisplayValue, this.PropertyExpressionControl(null, this.options.propertyControlWidth), panel.groups["Data"], "Expression"],
        ["sortDirection", this.loc.PropertyMain.SortDirection,
            this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetSortDirectionForCrossTabField(), true, false), panel.groups["Data"], "DropdownList"],
        ["sortType", this.loc.PropertyMain.SortType,
            this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetSortType(), true, false), panel.groups["Data"], "DropdownList"],
        ["value", this.loc.PropertyMain.Value, this.PropertyExpressionControl(null, this.options.propertyControlWidth), panel.groups["Data"], "Expression"],
        ["imageHorAlignment", this.loc.PropertyMain.ImageHorAlignment,
            this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetHorizontalAlignmentItems(true), true), panel.groups["ImageAdditional"], "DropdownList"],
        ["imageVertAlignment", this.loc.PropertyMain.ImageVertAlignment,
            this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetVerticalAlignmentItems(), true), panel.groups["ImageAdditional"], "DropdownList"],
        ["aspectRatio", this.loc.PropertyMain.AspectRatio, this.CheckBox(null), panel.groups["ImageAdditional"], "Checkbox"],
        ["stretch", this.loc.PropertyMain.Stretch, this.CheckBox(null), panel.groups["ImageAdditional"], "Checkbox"],
        ["allowHtmlTags", this.loc.PropertyMain.AllowHtmlTags, this.CheckBox(null), panel.groups["TextAdditional"], "Checkbox"],
        ["angle", this.loc.PropertyMain.Angle, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["TextAdditional"], "Textbox"],
        ["textBrush", this.loc.PropertyMain.TextBrush, this.PropertyBrushControl("crossTabFieldTextBrush", null, this.options.propertyControlWidth), panel.groups["TextAdditional"], "Brush"],
        ["font", this.loc.PropertyMain.Font, this.PropertyFontControl(null, null, true), panel.groups["TextAdditional"], "Font"],
        ["hideZeros", this.loc.PropertyMain.HideZeros, this.CheckBox(null), panel.groups["TextAdditional"], "Checkbox"],
        ["horAlignment", this.loc.PropertyMain.HorAlignment,
            this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetHorizontalAlignmentItems(), true), panel.groups["TextAdditional"], "DropdownList"],
        ["vertAlignment", this.loc.PropertyMain.VertAlignment,
            this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetVerticalAlignmentItems(), true), panel.groups["TextAdditional"], "DropdownList"],
        ["margins", this.loc.PropertyMain.Margins, this.PropertyMarginsControl(null, this.options.propertyControlWidth + 61), panel.groups["TextAdditional"], "Margins"],
        ["textFormat", this.loc.PropertyMain.TextFormat, this.PropertyTextFormatControl(null, this.options.propertyControlWidth), panel.groups["TextAdditional"], "TextFormat"],
        ["textOptions.distanceBetweenTabs", this.loc.PropertyMain.DistanceBetweenTabs, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["TextAdditional"], "Textbox"],
        ["textOptions.firstTabOffset", this.loc.PropertyMain.FirstTabOffset, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["TextAdditional"], "Textbox"],
        ["textOptions.hotkeyPrefix", this.loc.PropertyMain.HotkeyPrefix,
            this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetHotkeyPrefixItems(), true), panel.groups["TextAdditional"], "DropdownList"],
        ["textOptions.lineLimit", this.loc.PropertyMain.LineLimit, this.CheckBox(null), panel.groups["TextAdditional"], "Checkbox"],
        ["textOptions.rightToLeft", this.loc.PropertyMain.RightToLeft, this.CheckBox(null), panel.groups["TextAdditional"], "Checkbox"],
        ["textOptions.trimming", this.loc.PropertyMain.Trimming,
            this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetTrimmingItems(), true), panel.groups["TextAdditional"], "DropdownList"],
        ["wordWrap", this.loc.PropertyMain.WordWrap, this.CheckBox(null), panel.groups["TextAdditional"], "Checkbox"],
        ["minSize", this.loc.PropertyMain.MinSize, this.PropertySizeControl(null, this.options.propertyNumbersControlWidth + 40), panel.groups["Position"], "Size"],
        ["maxSize", this.loc.PropertyMain.MaxSize, this.PropertySizeControl(null, this.options.propertyNumbersControlWidth + 40), panel.groups["Position"], "Size"],
        ["brush", this.loc.PropertyMain.Brush, this.PropertyBrushControl("crossTabFieldBrush", null, this.options.propertyControlWidth), panel.groups["Appearance"], "Brush"],
        ["border", this.loc.PropertyMain.Border, this.PropertyBorderControl(null, this.options.propertyControlWidth), panel.groups["Appearance"], "Border"],
        ["conditions", this.loc.PropertyMain.Conditions, this.PropertyConditionsControl(null, this.options.propertyControlWidth), panel.groups["Appearance"], "Conditions"],
        ["componentStyle", this.loc.PropertyMain.ComponentStyle,
            this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetComponentStyleItems(), true, false), panel.groups["Appearance"], "DropdownList"],
        ["useParentStyles", this.loc.PropertyMain.UseParentStyles, this.CheckBox(null), panel.groups["Appearance"], "Checkbox"],
        ["useStyleOfSummaryInRowTotal", this.loc.PropertyMain.UseStyleOfSummaryInRowTotal, this.CheckBox(null), panel.groups["Appearance"], "Checkbox"],
        ["useStyleOfSummaryInColumnTotal", this.loc.PropertyMain.UseStyleOfSummaryInColumnTotal, this.CheckBox(null), panel.groups["Appearance"], "Checkbox"],
        ["interaction", null, this.PropertyInteractionControl(null, this.options.propertyControlWidth), panel.groups["Behavior"], "Interaction"],
        ["enumeratorSeparator", this.loc.PropertyMain.EnumeratorSeparator, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["Behavior"], "Textbox"],
        ["enumeratorType", this.loc.PropertyMain.EnumeratorType,
            this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetEnumeratorTypeItems(), true), panel.groups["Behavior"], "DropdownList"],
        ["mergeHeaders", this.loc.PropertyMain.MergeHeaders, this.CheckBox(null), panel.groups["Behavior"], "Checkbox"],
        ["enabled", this.loc.PropertyMain.Enabled, this.CheckBox(null), panel.groups["Behavior"], "Checkbox"],
        ["printOnAllPages", this.loc.PropertyMain.PrintOnAllPages, this.CheckBox(null), panel.groups["Behavior"], "Checkbox"],
        ["showTotal", this.loc.PropertyMain.ShowTotal, this.CheckBox(null), panel.groups["Behavior"], "Checkbox"]
    ]

    if (this.options.isJava) {
        properties.splice(properties.length - 1, 1);
    }

    for (var i = 0; i < properties.length; i++) {
        // eslint-disable-next-line no-unused-vars
        var property = this.AddCrossTabPropertyToPropertiesGroup(properties[i][0], properties[i][1], properties[i][2], properties[i][3], properties[i][4]);
    }

    panel.updateProperties = function (propertiesValues) {
        if (propertiesValues == null) propertiesValues = {};
        this.propertiesValues = propertiesValues;
        var propertiesGroups = this.groups;

        for (var groupName in propertiesGroups) {
            var propertyGroup = propertiesGroups[groupName];
            var showGroup = false;
            for (var propertyName in propertyGroup.properties) {
                var property = propertyGroup.properties[propertyName];
                if (property.isUserProperty) {
                    var showProperty = propertiesValues[property.name] != null;
                    property.style.display = showProperty ? "" : "none";
                    if (showProperty) {
                        showGroup = true;
                        this.jsObject.SetPropertyValue(property, propertiesValues[property.name]);
                    }
                }
            }
            propertyGroup.style.display = showGroup ? "" : "none";
        }

        if (propertiesValues.summary) {
            propertiesGroups.ImageAdditional.style.display = propertiesValues.summary == "Image" ? "" : "none";
            propertiesGroups.TextAdditional.style.display = propertiesValues.summary != "Image" ? "" : "none";
        }
    }

    return panel;
}

StiMobileDesigner.prototype.AddCrossTabPropertyToPropertiesGroup = function (propertyName, propertyCaption, propertyControl, propertyGroup, controlType) {
    var property = this.AddPropertyToPropertiesGroup(propertyName, propertyCaption, propertyControl, propertyGroup, controlType);

    propertyControl.action = function () {
        var crossTabForm = property.jsObject.options.forms.crossTabForm;
        var editCrossTabPropertiesPanel = property.jsObject.options.propertiesPanel.editCrossTabPropertiesPanel;

        if (crossTabForm && crossTabForm.visible && editCrossTabPropertiesPanel && editCrossTabPropertiesPanel.propertiesValues && editCrossTabPropertiesPanel.propertiesValues.name) {
            crossTabForm.sendCommand({
                command: "UpdateProperty",
                componentName: editCrossTabPropertiesPanel.propertiesValues.name,
                propertyName: this.property.name,
                propertyValue: this.property.getValue()
            });
        }
        else if (property.jsObject.options.selectedObject) {
            property.jsObject.ApplyCrossTabFieldProperty(property.jsObject.options.selectedObject, this.property.name, this.property.getValue());
        }
    }

    return property;
}

StiMobileDesigner.prototype.ApplyCrossTabFieldProperty = function (crossTabField, propertyName, propertyValue) {
    var jsObject = this;
    var editCrossTabPropertiesPanel = this.options.propertiesPanel.editCrossTabPropertiesPanel;
    if (editCrossTabPropertiesPanel) {

        var params = {
            componentName: crossTabField.properties.parentCrossTabName,
            updateParameters: {
                command: "UpdateProperty",
                componentName: editCrossTabPropertiesPanel.propertiesValues.name,
                propertyName: propertyName,
                propertyValue: propertyValue
            }
        };

        this.SendCommandToDesignerServer("UpdateCrossTabComponent", params, function (answer) {
            var crossTab = jsObject.options.report.getComponentByName(params.componentName);
            if (crossTab) {
                crossTab.properties.crossTabFields = answer.updateResult.fieldsProperties;
                jsObject.RepaintCrossTabFields(crossTab, answer.updateResult.selectedComponentName);
            }
        });
    }
}