
StiMobileDesigner.prototype.DesignPropertiesGroup = function () {
    var designPropertiesGroup = this.PropertiesGroup("designPropertiesGroup", this.loc.PropertyCategory.DesignCategory);
    designPropertiesGroup.style.display = "none";

    //Name
    var controlPropertyComponentName = this.PropertyTextBox("controlPropertyComponentName", this.options.propertyControlWidth);
    controlPropertyComponentName.action = function () {
        if (this.jsObject.options.selectedObject)
            this.jsObject.SendCommandRenameComponent(this.jsObject.options.selectedObject, this.value);
    }
    designPropertiesGroup.container.appendChild(this.Property("componentName",
        this.loc.PropertyMain.Name, controlPropertyComponentName, "Name"));

    //Alias
    var controlPropertyAlias = this.PropertyTextBox("controlPropertyAlias", this.options.propertyControlWidth);
    controlPropertyAlias.action = function () {
        this.jsObject.ApplyPropertyValue("aliasName", StiBase64.encode(this.value));
        if (this.jsObject.options.selectedObject && this.jsObject.options.selectedObject.typeComponent == "StiPage") {
            if (this.jsObject.options.pagesPanel) this.jsObject.options.pagesPanel.pagesContainer.updatePages();
            if (this.jsObject.options.reportTree) this.jsObject.options.reportTree.build();
        }
    }
    designPropertiesGroup.container.appendChild(this.Property("aliasName",
        this.loc.PropertyMain.Alias, controlPropertyAlias, "Alias"));

    //Globalized Name
    var controlPropertyGlobalizedName = this.PropertyTextBox("controlPropertyGlobalizedName", this.options.propertyControlWidth);
    controlPropertyGlobalizedName.action = function () {
        this.jsObject.ApplyPropertyValue("globalizedName", this.value);
    }
    designPropertiesGroup.container.appendChild(this.Property("globalizedName", this.loc.PropertyMain.GlobalizedName, controlPropertyGlobalizedName));

    //Icon
    var controlPropertyIcon = this.PropertyImageControl("controlPropertyPageIcon", this.options.propertyControlWidth);
    controlPropertyIcon.action = function () {
        this.jsObject.ApplyPropertyValue("pageIcon", this.key);
    }
    designPropertiesGroup.container.appendChild(this.Property("pageIcon", this.loc.PropertyMain.Icon, controlPropertyIcon));

    //LargeHeight
    var controlPropertyLargeHeight = this.CheckBox("controlPropertyLargeHeight");
    controlPropertyLargeHeight.action = function () {
        this.jsObject.ApplyPropertyValue("largeHeight", this.isChecked);
    }
    designPropertiesGroup.container.appendChild(this.Property("largeHeight", this.loc.PropertyMain.LargeHeight, controlPropertyLargeHeight));

    //LargeHeightFactor
    var controlLargeHeightFactor = this.PropertyTextBox("controlPropertyLargeHeightFactor", this.options.propertyNumbersControlWidth);
    controlLargeHeightFactor.action = function () {
        var val = Math.abs(this.jsObject.StrToInt(this.value));
        this.value = val == 0 ? 1 : val;
        this.jsObject.ApplyPropertyValue("largeHeightFactor", this.value);
    }
    designPropertiesGroup.container.appendChild(this.Property("largeHeightFactor",
        this.loc.PropertyMain.LargeHeightFactor, controlLargeHeightFactor));

    //Linked
    var controlPropertyLinked = this.CheckBox("controlPropertyLinked");
    controlPropertyLinked.action = function () {
        this.jsObject.ApplyPropertyValue("linked", this.isChecked);
    }
    designPropertiesGroup.container.appendChild(this.Property("linked", this.loc.PropertyMain.Linked, controlPropertyLinked));

    //Locked
    var controlPropertyLocked = this.CheckBox("controlPropertyLocked");
    controlPropertyLocked.action = function () {
        this.jsObject.ApplyPropertyValue("locked", this.isChecked);
    }
    designPropertiesGroup.container.appendChild(this.Property("locked", this.loc.PropertyMain.Locked, controlPropertyLocked));

    //Restrictions
    var controlPropertyRestrictions = this.PropertyRestrictionsControl("controlPropertyRestrictions", this.options.propertyControlWidth);
    controlPropertyRestrictions.action = function () {
        this.jsObject.ApplyPropertyValue("restrictions", this.key);
    }
    designPropertiesGroup.container.appendChild(this.Property("restrictions",
        this.loc.PropertyMain.Restrictions, controlPropertyRestrictions));

    return designPropertiesGroup;
}