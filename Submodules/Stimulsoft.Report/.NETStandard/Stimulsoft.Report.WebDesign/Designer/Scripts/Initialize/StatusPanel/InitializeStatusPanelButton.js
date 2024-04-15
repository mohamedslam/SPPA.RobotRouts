
StiMobileDesigner.prototype.StatusPanelButton = function (name, caption, imageName, toolTip, arrow, height, width, imageSizes) {
    var button = this.SmallButton(name, null, caption, imageName, toolTip, arrow, "stiDesignerStatusPanelButton", null, imageSizes);

    if (height) button.style.height = height + "px";
    if (width) {
        button.style.minWidth = width + "px";
        button.innerTable.style.width = "100%";
    }

    return button;
}