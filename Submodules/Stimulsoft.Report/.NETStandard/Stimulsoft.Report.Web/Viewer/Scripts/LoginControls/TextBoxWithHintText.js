StiJsViewer.prototype.TextBoxWithHintText = function (name, width, hintText, toolTip, imageName) {
    var jsObject = this;
    var table = this.CreateHTMLTable();

    var textBox = this.TextBox(name, width, toolTip);
    textBox.setAttribute("placeholder", hintText);
    textBox.style.backgroundColor = "#f2f1f1";
    textBox.style.border = "none";
    textBox.style.width = "280px";
    textBox.style.height = "30px";

    if (imageName) {
        var img = document.createElement("img");
        img.style.verticalAlign = "middle";
        img.style.opacity = "0.8";
        img.style.width = "16px";
        img.style.height = "16px";
        StiJsViewer.setImageSource(img, jsObject.options, jsObject.collections, "LoginControls.LogIn." + jsObject.GetThemeColor() + "." + imageName);

        var iim = table.addCell(img);
        iim.style.lineHeight = "0";
        iim.style.backgroundColor = "#f2f1f1";
        iim.style.width = "30px";
        iim.style.textAlign = "center";
    }

    table.addCell(textBox);
    textBox.table = table;

    //Override
    textBox.onkeypress = function (event) {
        if (this.readOnly) return false;
        if (event && event.keyCode == 13) {
            this.blur();
            if (!this.readOnly) this.action();
            return false;
        }
    }

    textBox.onblur = function () {
        this.isFocused = false;
        this.setSelected(false);
        this.jsObject.options.controlsIsFocused = false;
    }

    return textBox;
}

StiJsViewer.prototype.GetThemeColor = function () {
    var themeColor = this.options.theme;
    themeColor = themeColor.replace("Office2013White", "");
    themeColor = themeColor.replace("Office2013LightGray", "");
    themeColor = themeColor.replace("Office2013VeryDarkGray", "");
    themeColor = themeColor.replace("Office2013DarkGray", "");
    themeColor = themeColor.replace("Office2022White", "");

    return (themeColor || "Blue");
}