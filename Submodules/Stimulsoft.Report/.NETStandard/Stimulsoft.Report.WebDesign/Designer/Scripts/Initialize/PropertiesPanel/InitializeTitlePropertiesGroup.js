
StiMobileDesigner.prototype.TitlePropertiesGroup = function () {
    var titlePropertiesGroup = this.PropertiesGroup("titlePropertiesGroup", this.loc.PropertyCategory.TitleCategory, null, 1);
    titlePropertiesGroup.style.display = "none";

    //BackColor
    var controlPropertyTitleBackColor = this.PropertyColorControl("controlPropertyTitleBackColor", null, this.options.propertyControlWidth);
    controlPropertyTitleBackColor.action = function () {
        this.jsObject.ApplyPropertyValue("titleBackColor", this.key);
    }
    titlePropertiesGroup.container.appendChild(this.Property("titleBackColor", this.loc.PropertyMain.BackColor, controlPropertyTitleBackColor, "BackColor", 2));

    //ForeColor
    var controlPropertyTitleForeColor = this.PropertyColorControl("controlPropertyTitleForeColor", null, this.options.propertyControlWidth);
    controlPropertyTitleForeColor.action = function () {
        this.jsObject.ApplyPropertyValue("titleForeColor", this.key);
    }
    titlePropertiesGroup.container.appendChild(this.Property("titleForeColor", this.loc.PropertyMain.ForeColor, controlPropertyTitleForeColor, "ForeColor", 2));

    //Font
    var controlPropertyTitleFont = this.PropertyFontControl("titleFont");
    titlePropertiesGroup.container.appendChild(this.Property("titleFont", this.loc.PropertyMain.Font, controlPropertyTitleFont, "Font", 2));

    //Horizontal Alignment
    var controlPropertyTitleHorAlignment = this.PropertyDropDownList("controlPropertyTitleHorAlignment", this.options.propertyControlWidth, this.GetHorizontalAlignmentItems(true), true, false);
    controlPropertyTitleHorAlignment.action = function () {
        this.jsObject.ApplyPropertyValue("titleHorAlignment", this.key);
    }
    titlePropertiesGroup.container.appendChild(this.Property("titleHorAlignment", this.loc.PropertyMain.HorAlignment, controlPropertyTitleHorAlignment, "HorAlignment", 2));

    //Size Mode
    var controlPropertySizeMode = this.PropertyDropDownList("controlPropertyTitleSizeMode", this.options.propertyControlWidth, this.GetTitleSizeModeItems(), true, false);
    controlPropertySizeMode.action = function () {
        this.jsObject.ApplyPropertyValue("titleSizeMode", this.key);
    }
    titlePropertiesGroup.container.appendChild(this.Property("titleSizeMode", this.loc.PropertyMain.SizeMode, controlPropertySizeMode, "SizeMode", 2));

    //Text
    var controlPropertyTitleText = this.PropertyTextBox("controlPropertyTitleText", this.options.propertyControlWidth);
    controlPropertyTitleText.action = function () {
        this.jsObject.ApplyPropertyValue("titleText", StiBase64.encode(this.value));
    }
    titlePropertiesGroup.container.appendChild(this.Property("titleText", this.loc.PropertyMain.Text, controlPropertyTitleText, "Text", 2));

    //Visible
    var controlPropertyTitleVisible = this.CheckBox("controlPropertyTitleVisible");
    controlPropertyTitleVisible.action = function () {
        this.jsObject.ApplyPropertyValue("titleVisible", this.isChecked);
    }
    titlePropertiesGroup.container.appendChild(this.Property("titleVisible", this.loc.PropertyMain.Visible, controlPropertyTitleVisible, "Visible", 2));

    return titlePropertiesGroup;
}