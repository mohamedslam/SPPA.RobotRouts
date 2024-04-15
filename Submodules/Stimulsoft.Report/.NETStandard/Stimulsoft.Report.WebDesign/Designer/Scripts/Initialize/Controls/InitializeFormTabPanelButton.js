
StiMobileDesigner.prototype.FormTabPanelButton = function (name, caption, imageName, toolTip, minWidth, imageSizes, height) {
    var button = this.SmallButton(name, null, caption, imageName, toolTip, null, "stiFormTabPanelButton", null, imageSizes);
    button.style.margin = "6px 12px 6px 12px";
    if (minWidth) button.style.minWidth = minWidth + "px";
    if (button.caption) button.caption.style.padding = imageName ? "0px 30px 0px 0px" : "0px 30px 0px 4px";

    var bar = document.createElement("div");
    bar.className = "stiFormTabPanelButtonBar";
    bar.style.visibility = "hidden";
    button.innerTable.insertCell(0, bar);

    if (height) {
        button.style.height = height + "px";
        bar.style.height = (height - 10) + "px";
    }

    button.setSelected = function (state) {
        this.isSelected = state;
        this.className = this.isEnabled ? (state ? this.selectedClass : (this.isOver ? this.overClass : this.defaultClass)) : this.disabledClass;
        bar.style.visibility = state ? "visible" : "hidden";
    }

    return button;
}