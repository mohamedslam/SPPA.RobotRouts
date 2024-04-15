
StiMobileDesigner.prototype.StandartBigButton = function (name, groupName, caption, imageName, toolTip, maxWidth, minWidth) {
    var button = this.BigButton(name, groupName, caption, imageName, toolTip, null, "stiDesignerStandartBigButton", null, maxWidth);

    //Override
    if (button.cellImage) button.cellImage.style.height = "40px";
    if (minWidth) button.style.minWidth = minWidth + "px";

    return button;
}

StiMobileDesigner.prototype.StandartFormBigButton = function (name, groupName, caption, imageName, toolTip, maxWidth) {
    var button = this.StandartBigButton(name, groupName, caption, imageName, toolTip, maxWidth);
    button.style.minWidth = "80px";
    button.style.minHeight = "69px";
    if (button.cellImage) button.cellImage.style.padding = "2px 2px 0px 2px";
    if (button.caption) button.caption.style.padding = "0 3px 2px 3px";

    return button;
}