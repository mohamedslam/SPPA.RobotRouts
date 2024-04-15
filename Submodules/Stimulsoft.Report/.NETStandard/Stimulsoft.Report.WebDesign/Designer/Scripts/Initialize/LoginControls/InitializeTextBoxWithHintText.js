
StiMobileDesigner.prototype.TextBoxWithHintText = function (name, width, hintText, toolTip, image) {
    var jsObject = this;
    var table = this.CreateHTMLTable();
    table.className = "stiDesignerTextBox stiDesignerTextBoxDefault";
    table.style.padding = "0";

    var getPath = function (fileName) {
        return "LoginControls.LogIn." + jsObject.GetThemeColor() + "." + fileName;
    }

    var textBox = this.TextBox(name, width, toolTip);
    textBox.setAttribute("placeholder", hintText);

    textBox.style.border = "0px";
    textBox.style.height = "30px";

    var img = $("<img style='vertical-align: middle;opacity: 0.8'></img>")[0];
    StiMobileDesigner.setImageSource(img, jsObject.options, getPath(image));
    img.style.width = img.style.height = "16px";

    var imCell = table.addCell(img);
    imCell.className = "stiDesignerTextBox stiDesignerTextBoxDefault";
    imCell.style.border = "0px";
    imCell.style.lineHeight = "0";
    imCell.fileName = image;
    imCell.style.width = "30px";
    imCell.style.textAlign = "center";

    table.addCell(textBox);
    textBox.table = table;
    textBox.style.borderRadius = this.allowRoundedControls() ? "0 3px 3px 0" : "0";

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

    $(this).on("eventTheme", function () {
        StiMobileDesigner.setImageSource(img, jsObject.options, getPath(imCell.fileName));
    });

    return textBox;
}