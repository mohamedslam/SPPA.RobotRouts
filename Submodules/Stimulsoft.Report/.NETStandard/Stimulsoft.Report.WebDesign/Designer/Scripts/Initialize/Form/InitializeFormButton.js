
StiMobileDesigner.prototype.FormButton = function (form, name, caption, imageName, tooltip, imageSizes, autoWidth, buttonStyle, arrow) {
    var button = this.SmallButton(name, null, caption, imageName, tooltip, arrow, buttonStyle || "stiDesignerFormButton", true, imageSizes);
    button.innerTable.style.width = "100%";
    button.style.minWidth = "80px";
    button.thisForm = form;

    if (button.caption) {
        button.caption.style.textAlign = "center";
        button.caption.style.width = "100%";
    }
    if (autoWidth) {
        button.style.display = "inline-block";
    }

    return button;
}

StiMobileDesigner.prototype.FormButtonWithThemeBorder = function (name, groupName, caption, imageName, toolTip) {
    var button = this.SmallButton(name, groupName, caption, imageName, toolTip, null, "stiDesignerFormButtonWithThemeBorder");
    button.innerTable.style.width = "100%";

    if (button.caption) {
        button.caption.style.textAlign = "center";
        button.caption.style.width = "100%";
    }

    if (button.imageCell) {
        button.imageCell.style.padding = (this.options.isTouchDevice && caption == null) ? "0 6px 0 6px" : "0 3px 0 3px";
    }

    button.onmouseenter = function () {
        if (!this.isEnabled || this.isSelected || this.jsObject.options.isTouchClick) return;
        this.className = this.overClass;
        this.isOver = true;
    }

    button.onmouseleave = function () {
        this.isOver = false;
        if (!this.isEnabled || this.isSelected) return;
        this.className = this.isSelected ? this.selectedClass : this.defaultClass;
    }

    return button;
}

StiMobileDesigner.prototype.FormImageButton = function (name, imageName, toolTip) {
    var button = this.SmallButton(name, null, null, imageName, toolTip, null, "stiDesignerFormButton");
    button.style.width = button.style.height = this.options.isTouchDevice ? "28px" : "24px";
    button.innerTable.style.width = "100%";
    if (button.imageCell) button.imageCell.style.padding = "0px";

    return button;
}