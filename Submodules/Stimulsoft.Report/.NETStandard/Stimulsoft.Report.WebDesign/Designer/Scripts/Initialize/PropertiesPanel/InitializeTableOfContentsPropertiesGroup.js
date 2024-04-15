
StiMobileDesigner.prototype.TableOfContentsPropertiesGroup = function () {
    var jsObject = this;
    var group = this.PropertiesGroup("tableOfContentsPropertiesGroup", this.loc.PropertyCategory.TableOfContentsCategory);
    group.style.display = "none";

    var props = [
        ["tableOfContentsIndent", this.loc.PropertyMain.Indent, this.PropertyTextBox("controlPropertyTableOfContentsIndent", this.options.propertyNumbersControlWidth), "Indent"],
        ["tableOfContentsMargins", this.loc.PropertyMain.Margins, this.PropertyMarginsControl("controlPropertyTableOfContentsMargins", this.options.propertyControlWidth + 61), "Margins"],
        ["tableOfContentsNewPageBefore", this.loc.PropertyMain.NewPageBefore, this.CheckBox("controlPropertyTableOfContentsNewPageBefore"), "NewPageBefore"],
        ["tableOfContentsNewPageAfter", this.loc.PropertyMain.NewPageAfter, this.CheckBox("controlPropertyTableOfContentsNewPageAfter"), "NewPageAfter"],
        ["tableOfContentsRightToLeft", this.loc.PropertyMain.RightToLeft, this.CheckBox("controlPropertyTableOfContentsRightToLeft"), "RightToLeft"],
        ["tableOfContentsStyles", this.loc.PropertyMain.Styles, this.PropertyStylesControl("controlPropertyTableOfContentsStyles", this.options.propertyControlWidth), "Styles"]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        group.container.appendChild(this.Property(props[i][0], props[i][1], control, props[i][3]));

        control.action = function () {
            var value = this.propertyName == "tableOfContentsMargins" ? this.getValue() : jsObject.GetControlValue(this);
            jsObject.ApplyPropertyValue([this.propertyName], [value]);
        }
    }

    return group;
}