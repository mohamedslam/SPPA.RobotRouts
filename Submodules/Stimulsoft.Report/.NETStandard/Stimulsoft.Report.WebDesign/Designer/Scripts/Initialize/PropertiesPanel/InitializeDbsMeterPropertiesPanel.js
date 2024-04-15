
StiMobileDesigner.prototype.DbsMeterPropertiesPanel = function () {
    var panel = document.createElement("div");
    var jsObject = panel.jsObject = this;
    panel.style.display = "none";

    var groupNames = [
        ["Main", this.loc.PropertyCategory.MainCategory],
        ["Appearance", this.loc.PropertyCategory.AppearanceCategory],
        ["Behavior", this.loc.PropertyCategory.BehaviorCategory],
        ["Size", this.loc.PropertyMain.Size],
    ]

    this.AddGroupsToPropertiesPanel(groupNames, panel);
    panel.groups.Main.changeOpenedState(true);
    panel.groups.Size.changeOpenedState(true);

    var properties = [
        ["expression", this.loc.PropertyMain.Expression, this.PropertyExpressionControl(null, this.options.propertyControlWidth, false, true), panel.groups["Main"], "Expression"],
        ["label", this.loc.PropertyMain.Label, this.PropertyTextBox(null, this.options.propertyControlWidth), panel.groups["Main"], "Textbox"],
        ["showTotalSummary", this.loc.PropertyMain.ShowTotal, this.CheckBox(), panel.groups["Main"], "Checkbox"],
        ["showTotal", this.loc.PropertyMain.ShowTotal, this.CheckBox(), panel.groups["Main"], "Checkbox"],
        ["totalLabel", this.loc.PropertyMain.TotalLabel, this.PropertyTextBox(null, this.options.propertyControlWidth), panel.groups["Main"], "Textbox"],
        ["expandExpression", this.loc.PropertyMain.Expand, this.PropertyExpressionControl("expandExpression", this.options.propertyControlWidth, null, null, null), panel.groups["Appearance"], "Expression"],
        ["hideZeros", this.loc.PropertyMain.HideZeros, this.CheckBox(null), panel.groups["Main"], "Checkbox"],
        ["showHyperlink", this.loc.PropertyMain.ShowHyperlink, this.CheckBox(), panel.groups["Main"], "Checkbox"],
        ["hyperlinkPattern", this.loc.PropertyMain.HyperlinkPattern, this.PropertyExpressionControl(null, this.options.propertyControlWidth, false, true), panel.groups["Main"], "Expression"],
        ["sortDirection", this.loc.PropertyMain.SortDirection, this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetSortDirectionItems(), true, false), panel.groups["Appearance"], "DropdownList"],

        ["font", this.loc.PropertyMain.Font, this.PropertyFontControl(null, null, true), panel.groups["Appearance"], "Font"],
        ["foreColor", this.loc.PropertyMain.ForeColor, this.PropertyColorControl(null, null, this.options.propertyControlWidth), panel.groups["Appearance"], "Color"],
        ["height", this.loc.PropertyMain.Height, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["Appearance"], "Textbox"],
        ["headerAlignment", this.loc.PropertyMain.HeaderAlignment, this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetHorizontalAlignmentItems(true), true, false), panel.groups["Appearance"], "DropdownList"],
        ["horAlignment", this.loc.PropertyMain.HorAlignment, this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetHorizontalAlignmentItems(true), true, false), panel.groups["Appearance"], "DropdownList"],
        ["vertAlignment", this.loc.PropertyMain.VertAlignment, this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetVerticalAlignmentItems(true), true, false), panel.groups["Appearance"], "DropdownList"],
        ["summaryAlignment", this.loc.PropertyMain.SummaryAlignment, this.PropertyDropDownList(null, this.options.propertyControlWidth, this.GetHorizontalAlignmentItems(true), true, false), panel.groups["Appearance"], "DropdownList"],
        ["wrapLine", this.loc.PropertyMain.WrapLine, this.CheckBox(null), panel.groups["Appearance"], "Checkbox"],

        ["textFormat", this.loc.PropertyMain.TextFormat, this.PropertyTextFormatControl(null, this.options.propertyControlWidth), panel.groups["Behavior"], "TextFormat"],

        ["size.MaxWidth", this.loc.PropertyMain.MaxWidth, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["Size"], "Textbox"],
        ["size.MinWidth", this.loc.PropertyMain.MinWidth, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["Size"], "Textbox"],
        ["size.Width", this.loc.PropertyMain.Width, this.PropertyTextBox(null, this.options.propertyNumbersControlWidth), panel.groups["Size"], "Textbox"],
        ["size.WordWrap", this.loc.PropertyMain.WordWrap, this.CheckBox(null), panel.groups["Size"], "Checkbox"]
    ]

    for (var i = 0; i < properties.length; i++) {
        this.AddDbsMeterPropertyToPropertiesGroup(properties[i][0], properties[i][1], properties[i][2], properties[i][3], properties[i][4]);
    }

    panel.updateProperties = function (elementForm, meterItem) {
        if (meterItem) {
            var propertiesValues = meterItem.itemObject;
            var propertiesGroups = this.groups;

            for (var groupName in propertiesGroups) {
                var propertyGroup = propertiesGroups[groupName];
                var showGroup = false;

                for (var propertyName in propertyGroup.properties) {
                    var property = propertyGroup.properties[propertyName];
                    property.elementForm = elementForm;
                    property.meterItem = meterItem;

                    if (property.isUserProperty) {
                        var propertyValue = propertiesValues[property.name];
                        property.style.display = propertyValue != null ? "" : "none";
                        if (propertyValue != null) {
                            showGroup = true;
                            jsObject.SetPropertyValue(property, propertyName == "expression" || propertyName == "hyperlinkPattern" ? StiBase64.decode(propertyValue) : propertyValue);
                        }
                    }

                    if (propertyName == "expandExpression" && meterItem.container) {
                        property.style.display = meterItem.container.getCountItems() > 1 ? "" : "none";
                    }
                }
                propertyGroup.style.display = showGroup ? "" : "none";
            }
        }
    }

    return panel;
}

StiMobileDesigner.prototype.AddDbsMeterPropertyToPropertiesGroup = function (propertyName, propertyCaption, propertyControl, propertyGroup, controlType) {
    var jsObject = this;
    var property = this.AddPropertyToPropertiesGroup(propertyName, propertyCaption, propertyControl, propertyGroup, controlType);

    propertyControl.action = function () {
        var propertyName = this.property.name;
        var form = this.property.elementForm;
        var meterItem = this.property.meterItem;

        if (form && meterItem) {
            if (propertyName == "expression" || propertyName == "hyperlinkPattern") {
                form.setPropertyValue(jsObject.UpperFirstChar(propertyName), StiBase64.encode(this.textBox.value));
            }
            else if (propertyName == "label") {
                meterItem.itemObject.label = this.value;
                meterItem.container.onAction("rename");
            }
            else {
                form.setPropertyValue(jsObject.UpperFirstChar(propertyName), jsObject.ExtractBase64Value(this.property.getValue()));
            }
        }
    }

    return property;
}