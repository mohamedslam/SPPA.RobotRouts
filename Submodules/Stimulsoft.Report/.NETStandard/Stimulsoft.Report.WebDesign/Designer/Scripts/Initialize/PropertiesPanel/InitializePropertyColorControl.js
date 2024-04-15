
StiMobileDesigner.prototype.PropertyColorControl = function (name, toolTip, width) {
    var propertyColorControl = this.ColorControl(name, toolTip, true);
    propertyColorControl.isColorControl = true;
    var jsObject = this;

    //Ovveride New Button
    var newButton = this.SmallButton(name + "Button", null, "0,0,0", "BrushSolid.png", toolTip, "Down", "stiDesignerPropertiesBrushControlButton");
    newButton.style.width = (width + 4) + "px";
    newButton.caption.style.width = "100%";
    newButton.colorControl = propertyColorControl;

    var colorBar = document.createElement("div");
    colorBar.className = "stiBrushControlColorBar";

    var imageCell = newButton.image.parentElement;
    imageCell.removeChild(newButton.image);
    imageCell.appendChild(colorBar);
    newButton.image = colorBar;

    //Override Old Button
    var oldButtonCell = propertyColorControl.button.parentElement;
    oldButtonCell.removeChild(propertyColorControl.button);
    oldButtonCell.appendChild(newButton);
    propertyColorControl.button = newButton;

    //Override Methods
    propertyColorControl.button.action = function () {
        var colorDialog = jsObject.options.menus.colorDialog || jsObject.InitializeColorDialog();
        colorDialog.rightToLeft = this.colorControl.rightToLeft;
        colorDialog.noFillButton.caption.innerHTML = propertyColorControl.isDbsElement && !colorDialog.visible
            ? jsObject.loc.FormStyleDesigner.FromStyle
            : jsObject.loc.Gui.colorpicker_nofill.replace("&", "");

        colorDialog.changeVisibleState(!colorDialog.visible, this);
    }

    propertyColorControl.button.choosedColor = function (key) {
        this.colorControl.setKey(key);
        this.colorControl.action();
    };

    propertyColorControl.setKey = function (key, isDbsElement) {
        var notLocalizeValues = this.ownerIsProperty && jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.localizePropertyGrid;
        this.key = key;
        this.isDbsElement = isDbsElement;
        if (key == "StiEmptyValue") {
            this.button.image.style.opacity = 0;
            this.button.caption.innerHTML = "";
            return;
        }
        this.button.image.style.opacity = 1;
        var color;
        if (key == "transparent")
            color = "255,255,255";
        else {
            var colors = key.split(",");
            if (colors.length == 4) {
                this.button.image.style.opacity = jsObject.StrToInt(colors[0]) / 255;
                colors.splice(0, 1);
            }
            color = colors[0] + "," + colors[1] + "," + colors[2];
        }

        this.button.image.style.background = "rgb(" + color + ")";
        var colorName = jsObject.GetColorNameByRGB(this.key, notLocalizeValues);
        this.button.caption.innerHTML = (isDbsElement && key == "transparent") ? (notLocalizeValues ? "FromStyle" : jsObject.loc.FormStyleDesigner.FromStyle) : (colorName || this.key);
    };

    return propertyColorControl;
}