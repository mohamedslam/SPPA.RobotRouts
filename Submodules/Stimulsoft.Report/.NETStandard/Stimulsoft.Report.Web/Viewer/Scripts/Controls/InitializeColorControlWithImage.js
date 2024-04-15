
StiJsViewer.prototype.ColorControlWithImage = function (name, imageName, toolTip, topMarginIcon) {
    var colorControl = this.ColorControl(name, toolTip, null, null, null, true);

    //Override image
    var icon = document.createElement("img");
    icon.style.width = "16px";
    icon.style.height = "12px";
    icon.style.pointerEvents = "none";
    StiJsViewer.setImageSource(icon, this.options, this.collections, imageName);

    var button = colorControl.button;
    var colorBar = button.image;
    var colorBarParent = colorBar.parentElement;
    var imageCell = colorBarParent.parentElement;

    colorBarParent.className = "stiJsViewerColorControlWithImage_ColorBar";
    colorBarParent.style.height = "2px";

    if (topMarginIcon) {
        colorBarParent.parentElement.style.paddingTop = "4px";
    }

    imageCell.removeChild(colorBarParent);
    imageCell.appendChild(icon);
    imageCell.appendChild(colorBarParent);

    button.image = colorBar;
    button.icon = icon;
    
    //Override methods
    colorControl.setEnabled = function (state) {
        if (button.image) button.image.style.opacity = state ? "1" : "0.3";
        if (button.icon) button.icon.style.opacity = state ? "1" : "0.3";
        if (button.arrow) button.arrow.style.opacity = state ? "1" : "0.3";
        this.isEnabled = state;
        button.isEnabled = state;
        button.className = state ? button.defaultClass : button.disabledClass;
    }
    
    return colorControl;
}