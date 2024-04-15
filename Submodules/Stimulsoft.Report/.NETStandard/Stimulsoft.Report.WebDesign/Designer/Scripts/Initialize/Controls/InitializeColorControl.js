
StiMobileDesigner.prototype.ColorControl = function (name, toolTip, rightToLeft, width, showBorder, withImage) {
    var colorControl = this.CreateHTMLTable();
    colorControl.jsObject = this;
    colorControl.name = name != null ? name : this.generateKey();
    colorControl.key = null;
    colorControl.isEnabled = true;
    colorControl.rightToLeft = rightToLeft;
    if (name != null) this.options.controls[name] = colorControl;

    //Button
    var button = colorControl.button = this.StandartSmallButton(name != null ? name + "Button" : null, null, null, true, toolTip, "Down");
    button.imageCell.style.padding = this.options.isTouchDevice ? "0 5px 0 5px" : "0 2px 0 2px";
    button.arrowCell.style.padding = this.options.isTouchDevice ? "0 7px 0 2px" : "0px 4px 0px 2px";

    button.colorControl = colorControl;
    colorControl.addCell(button);

    if (showBorder) {
        colorControl.className = "stiColorControlWithBorder";
        button.style.height = (this.options.controlsHeight - 2) + "px";
    }

    //Override image
    var newImageParent = document.createElement("div");
    newImageParent.className = "stiColorControlImage";
    newImageParent.style.height = (this.options.controlsHeight - 8) + "px";

    var newImage = document.createElement("div");
    newImage.style.height = "100%";
    newImageParent.appendChild(newImage);
    if (width) newImageParent.style.width = (width - (this.options.isTouchDevice ? 27 : 18)) + "px";

    if (withImage) {
        newImageParent.style.borderRadius = newImage.style.borderRadius = "0";
    }

    var imageCell = button.image.parentElement;
    imageCell.removeChild(button.image);
    imageCell.appendChild(newImageParent);
    button.image = newImage;

    button.action = function () {
        var colorDialog = this.jsObject.options.menus.colorDialog || this.jsObject.InitializeColorDialog();
        colorDialog.rightToLeft = this.colorControl.rightToLeft;
        colorDialog.noFillButton.caption.innerHTML = colorControl.isDbsElement && !colorDialog.visible
            ? this.jsObject.loc.FormStyleDesigner.FromStyle
            : this.jsObject.loc.Gui.colorpicker_nofill.replace("&", "");

        colorDialog.changeVisibleState(!colorDialog.visible, this);
    }

    button.choosedColor = function (key) {
        colorControl.setKey(key, colorControl.isDbsElement);
        colorControl.action();
    };

    //Color Control
    colorControl.setKey = function (key, isDbsElement) {
        this.key = key;
        this.isDbsElement = isDbsElement;

        if (key == "StiEmptyValue") {
            button.image.style.opacity = 0;
            return;
        }

        button.image.style.opacity = 1;

        var color;
        if (key == "transparent") {
            color = "255,255,255";
        }
        else if (key != null) {
            var colors = key.split(",");
            if (colors.length == 4) {
                button.image.style.opacity = this.jsObject.StrToInt(colors[0]) / 255;
                colors.splice(0, 1);
            }
            color = colors[0] + "," + colors[1] + "," + colors[2];
        }

        button.image.style.background = "rgb(" + color + ")";

        if (!withImage) {
            newImageParent.style.background = button.image.style.background;
        }
    };

    //Override methods
    colorControl.setEnabled = function (state) {
        this.isEnabled = state;
        button.setEnabled(state);
        if (showBorder) this.className = state ? "stiColorControlWithBorder" : "stiColorControlWithBorderDisabled";
    }

    colorControl.action = function () { };

    return colorControl;
}