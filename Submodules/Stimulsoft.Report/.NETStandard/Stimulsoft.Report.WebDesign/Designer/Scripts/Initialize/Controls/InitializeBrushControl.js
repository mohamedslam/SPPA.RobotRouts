
StiMobileDesigner.prototype.BrushControl = function (name, caption, imageName, toolTip, topMarginIcon) {
    var brushControl = this.CreateHTMLTable();
    brushControl.jsObject = this;
    brushControl.name = name;
    brushControl.key = null;
    brushControl.isEnabled = true;
    this.options.controls[name] = brushControl;

    var button = brushControl.button = this.StandartSmallButton(name + "Button", null, caption, imageName, toolTip, null, null, { width: 16, height: 12 });
    brushControl.addCell(brushControl.button);

    var brushMenu = brushControl.menu = this.BrushMenu(name + "BrushMenu", button);

    //Override image
    var colorBar = document.createElement("div");
    colorBar.className = "stiColorControlWithImage_ColorBar";

    var icon = button.image;
    var imageCell = button.image.parentElement;
    imageCell.appendChild(colorBar);
    button.image = colorBar;
    button.icon = icon;

    if (imageName == "TextColor.png" || topMarginIcon) {
        imageCell.style.paddingTop = "4px";
    }

    //Override methods
    brushControl.setEnabled = function (state) {
        if (button.image) button.image.style.opacity = state ? "1" : "0.3";
        if (button.icon) button.icon.style.opacity = state ? "1" : "0.3";
        if (button.arrow) button.arrow.style.opacity = state ? "1" : "0.3";
        this.isEnabled = state;
        button.isEnabled = state;
        button.className = state ? button.defaultClass : button.disabledClass;
    }

    button.action = function () {
        brushMenu.changeVisibleState(!brushMenu.visible);
    }

    brushControl.action = function () { }

    brushControl.setKey = function (key) {
        this.key = key;
        button.key = key;
        if (key == "StiEmptyValue") {
            button.image.style.background = "#ffffff";
            button.caption.innerHTML = "";
            return;
        }
        var brushColor = this.jsObject.GetColorFromBrushStr(key);
        button.image.style.opacity = 1;
        var color;
        if (brushColor == "transparent")
            color = "255,255,255";
        else {
            var colors = brushColor.split(",");
            if (colors.length == 4) {
                button.image.style.opacity = this.jsObject.StrToInt(colors[0]) / 255;
                colors.splice(0, 1);
            }
            color = colors[0] + "," + colors[1] + "," + colors[2];
        }

        button.image.style.background = "rgb(" + color + ")";
    }

    brushMenu.action = function () {
        brushControl.setKey(this.parentButton.key);
        brushControl.action();
    }

    return brushControl;
}
