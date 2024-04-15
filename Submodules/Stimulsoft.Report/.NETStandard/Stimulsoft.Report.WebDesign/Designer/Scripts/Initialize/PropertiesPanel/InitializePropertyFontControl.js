//Font Control
StiMobileDesigner.prototype.PropertyFontControl = function (controlName, propertyName, enableActionEvent) {
    var jsObject = this;
    var fontControl = this.CreateHTMLTable();
    fontControl.style.margin = "2px 0 2px 0";
    fontControl.isEnabled = true;
    fontControl.controlName = controlName;
    fontControl.propertyName = propertyName || controlName;
    fontControl.key = "Arial!8!0!0!0!0";
    fontControl.multiRows = true;
    fontControl.controls = {};

    fontControl.applyFontPropertyValue = function (fontPropertyName, fontPropertyValue) {
        var selectedObjects = jsObject.options.selectedObjects;
        for (var i = 0; i < selectedObjects.length; i++) {
            var font = jsObject.FontStrToObject(selectedObjects[i].properties[this.propertyName]);
            font[fontPropertyName] = fontPropertyValue;
            var fontResult = jsObject.FontObjectToStr(font);
            selectedObjects[i].properties[this.propertyName] = fontResult;
        }
        jsObject.SendCommandSendProperties(selectedObjects, [this.propertyName]);
    }

    fontControl.actionFontChildControl = function (fontPropertyName, fontPropertyValue) {
        if (enableActionEvent) {
            var font = jsObject.FontStrToObject(this.key);
            font[fontPropertyName] = fontPropertyValue;
            this.key = jsObject.FontObjectToStr(font);
            this.action();
        }
        else if (jsObject.options.selectedObjects) {
            this.applyFontPropertyValue(fontPropertyName, fontPropertyValue);
        }
        else if (jsObject.options.selectedObject) {
            var font = jsObject.FontStrToObject(jsObject.options.selectedObject.properties[this.propertyName]);
            font[fontPropertyName] = fontPropertyValue;
            var fontResult = jsObject.FontObjectToStr(font);
            jsObject.options.selectedObject.properties[this.propertyName] = fontResult;
            jsObject.SendCommandSendProperties(jsObject.options.selectedObject, [this.propertyName]);
        }
    }

    //Name
    var fontName = fontControl.controls.fontName = this.PropertyFontList("controlProperty" + controlName + "Name", this.options.propertyControlWidth - 56, false);
    fontControl.addCell(fontName);
    fontName.action = function () {
        if (this.key == "Aharoni") { fontControl.boldButton.setSelected(true); }
        fontControl.boldButton.isEnabled = !(this.key == "Aharoni");
        fontControl.actionFontChildControl("name", this.key);
    };

    //Size
    var sizeItems = [];
    for (var i = 0; i < this.options.fontSizes.length; i++) {
        sizeItems.push(this.Item(controlName + "SizesFont" + i, this.options.fontSizes[i], null, this.options.fontSizes[i]));
    }
    var fontSize = fontControl.controls.fontSize = this.PropertyDropDownList("controlProperty" + controlName + "Size", 45, sizeItems, false, false, this.loc.HelpDesigner.FontSize);
    fontControl.addCell(fontSize).style.paddingLeft = "5px";
    fontSize.action = function () {
        var sizeValue = Math.abs(jsObject.StrToDouble(this.key));
        if (sizeValue == 0) sizeValue = 1;
        this.setKey(sizeValue.toString());
        fontControl.actionFontChildControl("size", this.key);
    }

    var fontDownTable = this.CreateHTMLTable();
    fontControl.addCellInNextRow(fontDownTable).setAttribute("colspan", "2");

    //Bold
    var boldButton = fontControl.controls.boldButton = this.StandartSmallButton("controlProperty" + controlName + "Bold", null, null, "Bold.png", this.loc.PropertyMain.Bold, null);
    fontDownTable.addCell(boldButton).style.padding = "3px 2px 0 0";
    fontControl.boldButton = boldButton;
    boldButton.action = function () {
        this.setSelected(!this.isSelected);
        fontControl.actionFontChildControl("bold", this.isSelected ? "1" : "0");
    }

    //Italic
    var italicButton = fontControl.controls.italicButton = this.StandartSmallButton("controlProperty" + controlName + "Italic", null, null, "Italic.png", this.loc.PropertyMain.Italic, null);
    fontDownTable.addCell(italicButton).style.padding = "3px 2px 0 2px";
    italicButton.action = function () {
        this.setSelected(!this.isSelected);
        fontControl.actionFontChildControl("italic", this.isSelected ? "1" : "0");
    }

    //Underline
    var underlineButton = fontControl.controls.underlineButton = this.StandartSmallButton("controlProperty" + controlName + "Underline", null, null, "Underline.png", this.loc.PropertyMain.Underline, null);
    fontDownTable.addCell(underlineButton).style.padding = "3px 2px 0 2px";
    underlineButton.action = function () {
        this.setSelected(!this.isSelected);
        fontControl.actionFontChildControl("underline", this.isSelected ? "1" : "0");
    }

    //Strikeout
    var strikeoutButton = fontControl.controls.strikeoutButton = this.StandartSmallButton("controlProperty" + controlName + "Strikeout", null, null, "Strikeout.png", this.loc.PropertyMain.FontStrikeout, null);
    fontDownTable.addCell(strikeoutButton).style.padding = "3px 2px 0 2px";
    strikeoutButton.action = function () {
        this.setSelected(!this.isSelected);
        fontControl.actionFontChildControl("strikeout", this.isSelected ? "1" : "0");
    }

    fontControl.setEnabled = function (state) {
        this.isEnabled = state;
        fontName.setEnabled(state);
        fontSize.setEnabled(state);
        boldButton.setEnabled(state);
        italicButton.setEnabled(state);
        underlineButton.setEnabled(state);
        strikeoutButton.setEnabled(state);
    }

    fontControl.setKey = function (key) {
        this.key = key;
        var font = key.split("!");
        fontName.setKey(font[0]);
        fontSize.setKey(font[1]);
        boldButton.setSelected(font[2] == "1");
        italicButton.setSelected(font[3] == "1");
        underlineButton.setSelected(font[4] == "1");
        strikeoutButton.setSelected(font[5] == "1");
    }

    fontControl.action = function () { }

    return fontControl;
}

StiMobileDesigner.prototype.PropertyComplexFontControl = function (name, width) {
    var jsObject = this;
    var control = this.CreateHTMLTable();
    if (!name) name = this.generateKey();
    control.name = name;
    control.key = null;
    control.isEnabled = true;
    this.options.controls[name] = control;

    var button = this.SmallButton(name + "Button", null, " ", null, null, "Down", "stiDesignerPropertiesBrushControlButton");
    button.style.width = (width + 4) + "px";
    button.caption.style.width = "100%";
    control.addCell(button);

    var menu = this.BaseMenu(name + "Menu", button, "Down");
    menu.innerContent.style.minWidth = width + "px";

    button.action = function () {
        menu.changeVisibleState(!menu.visible);
    }

    var defStateButton = this.StandartSmallButton(null, null, this.loc.Report.FromDefaultState);
    menu.innerContent.appendChild(defStateButton);

    var currStateButton = this.StandartSmallButton(null, null, this.loc.Report.FromCurrentState);
    currStateButton.style.width = currStateButton.innerTable.style.width = "100%";
    menu.innerContent.appendChild(currStateButton);

    var sep = this.FormSeparator();
    sep.style.margin = "4px 0 4px 0";
    menu.innerContent.appendChild(sep);

    var fontControl = this.PropertyFontControl(name + "FontControl", null, true);
    fontControl.style.margin = "0 4px 4px 4px";
    menu.innerContent.appendChild(fontControl);

    fontControl.action = function () {
        control.setKey(this.key);
        control.action();
    }

    defStateButton.action = function () {
        control.setKey("");
        control.action();
        menu.changeVisibleState(false);
    }

    currStateButton.action = function () {
        control.setKey(fontControl.key);
        control.action();
    }

    control.setKey = function (key) {
        this.key = key;
        button.caption.innerText = jsObject.loc.Report.FromDefaultState;
        if (key != "") {
            var font = jsObject.FontStrToObject(key);
            button.caption.innerText = font.name + ", " + font.size;
        }
        fontControl.setKey(key || "Arial!10!0!0!0!0");
        defStateButton.setSelected(key == "");
        currStateButton.setSelected(key != "");
        fontControl.style.display = sep.style.display = key != "" ? "" : "none";
    }

    control.setEnabled = function (state) {
        button.setEnabled(state);
    }

    control.action = function () { }

    return control;
}