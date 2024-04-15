
StiJsViewer.prototype.ColorControl = function (name, toolTip, rightToLeft, width, showBorder, withImage) {
    var colorControl = this.CreateHTMLTable();
    var jsObject = colorControl.jsObject = this;
    colorControl.name = name != null ? name : this.generateKey();
    colorControl.key = null;
    colorControl.isEnabled = true;
    colorControl.rightToLeft = rightToLeft;

    if (name) {
        if (!this.controls.colorControls) this.controls.colorControls = {};
        this.controls.colorControls[name] = colorControl;
    }

    //Button
    var button = colorControl.button = this.SmallButton(null, null, true, toolTip, "Down");
    button.imageCell.style.padding = this.options.isTouchDevice ? "0 5px 0 5px" : "0 2px 0 2px";
    button.arrowCell.style.padding = this.options.isTouchDevice ? "0 7px 0 2px" : "0px 4px 0px 2px";

    button.colorControl = colorControl;
    colorControl.addCell(button);

    if (showBorder) {
        colorControl.className = "stiJsViewerColorControlWithBorder";
        button.style.height = this.options.isTouchDevice ? "26px" : "21px";
    }

    //Override image
    var newImageParent = document.createElement("div");
    newImageParent.className = "stiJsViewerColorControlImage";
    newImageParent.style.height = this.options.isTouchDevice ? "20px" : "15px";

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
        var colorDialog = jsObject.controls.menus.colorDialog || jsObject.InitializeColorDialog();
        colorDialog.rightToLeft = this.colorControl.rightToLeft;
        colorDialog.changeVisibleState(!colorDialog.visible, this);
    }

    button.choosedColor = function (key) {
        colorControl.setKey(key);
        colorControl.action();
    };

    //Color Control
    colorControl.setKey = function (key) {
        this.key = key;

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
                button.image.style.opacity = jsObject.StrToInt(colors[0]) / 255;
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
        if (showBorder) this.className = state ? "stiJsViewerColorControlWithBorder" : "stiJsViewerColorControlWithBorderDisabled";
    }

    colorControl.action = function () { };

    return colorControl;
}