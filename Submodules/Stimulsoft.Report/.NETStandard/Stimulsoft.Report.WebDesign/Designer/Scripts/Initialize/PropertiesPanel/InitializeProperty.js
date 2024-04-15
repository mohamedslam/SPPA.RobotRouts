
StiMobileDesigner.prototype.Property = function (name, caption, propertyControl, originalName, nestingLevel) {
    var property = document.createElement("div");
    property.jsObject = this;
    if (name != null) this.options.properties[name] = property;
    property.name = name;
    property.originalName = originalName;
    property.captionText = caption;

    var propertyTable = this.CreateHTMLTable();
    property.appendChild(propertyTable);

    property.getOriginalPropertyName = function () {
        return this.jsObject.UpperFirstChar(this.originalName || this.name || this.captionText);
    }

    property.updateCaption = function () {
        if (this.caption)
            this.caption.innerHTML = !this.jsObject.options.propertiesPanel.localizePropertyGrid && this.name ? this.getOriginalPropertyName() : this.captionText;
    }

    //Caption
    if (caption != null) {
        property.caption = document.createElement("div");
        property.caption.setAttribute("title", caption);
        property.captionCell = propertyTable.addCell(property.caption);
        if (propertyControl && propertyControl.multiRows) {
            property.captionCell.style.verticalAlign = "top";
            property.captionCell.style.paddingTop = "5px";
        }
        else {
            property.captionCell.style.verticalAlign = "middle";
        }
        property.caption.className = "stiDesignerPropertyCaption";
        var captionWidth = this.options.propertiesGridLabelWidth;
        if (nestingLevel) captionWidth -= nestingLevel * 5;
        property.caption.style.width = captionWidth + "px";
        property.updateCaption();
    }
    else {
        propertyTable.style.width = "100%";
    }

    property.controlCell = propertyTable.addCell(propertyControl);
    property.controlCell.style.height = this.options.isTouchDevice ? "29px" : "24px";
    property.propertyControl = propertyControl;
    propertyControl.ownerIsProperty = true;

    return property;
}
