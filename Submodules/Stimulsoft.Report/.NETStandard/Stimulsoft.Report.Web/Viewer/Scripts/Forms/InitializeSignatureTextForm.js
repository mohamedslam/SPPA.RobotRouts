
StiJsViewer.prototype.InitializeSignatureTextForm = function () {
    var jsObject = this;
    var form = this.BaseForm("signatureText", this.collections.loc["Text"], 3);
    form.container.style.padding = "1px";

    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "12px";
    form.container.appendChild(toolBar);

    var controls = [
        ["fontName", this.FontList("signTextFontName", 100), "0"],
        ["fontSize", this.DropDownList("signTextFontSize", 40, null, this.GetFontSizeItems(), false), "4px"],
        ["fontBold", this.SmallButton(null, null, "Font.Bold.png", this.collections.loc["FontStyleBold"]), "12px"],
        ["fontItalic", this.SmallButton(null, null, "Font.Italic.png", this.collections.loc["FontStyleItalic"]), "4px"],
        ["fontUnderline", this.SmallButton(null, null, "Font.Underline.png", this.collections.loc["FontStyleUnderline"]), "4px"],
        ["textColor", this.ColorControlWithImage("signTextColor", "ColorControl.TextColor.png", null, true), "12px"],
        ["horAlignLeft", this.SmallButton(null, null, "Font.AlignLeft.png", this.collections.loc["AlignLeft"]), "12px"],
        ["horAlignCenter", this.SmallButton(null, null, "Font.AlignCenter.png", this.collections.loc["AlignCenter"]), "4px"],
        ["horAlignRight", this.SmallButton(null, null, "Font.AlignRight.png", this.collections.loc["AlignRight"]), "4px"]
    ]

    for (var i = 0; i < controls.length; i++) {
        var control = controls[i][1];
        var controlName = control.controlName = controls[i][0];
        form[controlName] = control;
        toolBar.addCell(control);
        control.style.marginLeft = controls[i][2];

        control.action = function () {
            if (this.controlName == "horAlignLeft" || this.controlName == "horAlignCenter" || this.controlName == "horAlignRight") {
                form.horAlignLeft.setSelected(false);
                form.horAlignCenter.setSelected(false);
                form.horAlignRight.setSelected(false);
                this.setSelected(true);
            }
            else if (this.controlName == "fontBold" || this.controlName == "fontItalic" || this.controlName == "fontUnderline") {
                this.setSelected(!this.isSelected);
            }
            form.textContainer.update();
        }
    }

    var textContainer = form.textContainer = this.TextArea(null, this.options.isTouchDevice ? 440 : 400, 400);
    textContainer.style.margin = "0 12px 12px 12px";
    form.container.appendChild(textContainer);

    textContainer.update = function () {
        this.style.fontFamily = form.fontName.key;
        this.style.fontSize = form.fontSize.key + "px";
        this.style.color = jsObject.getHTMLColor(form.textColor.key);
        this.style.textAlign = form.horAlignLeft.isSelected ? "left" : (form.horAlignRight.isSelected ? "right" : "center");
        this.style.fontWeight = form.fontBold.isSelected ? "bold" : "normal";
        this.style.fontStyle = form.fontItalic.isSelected ? "italic" : "normal";
        this.style.textDecoration = form.fontUnderline.isSelected ? "underline" : "";
    }

    form.show = function (textProps) {
        this.changeVisibleState(true);

        var font = textProps.font;
        this.fontName.setKey(font.name);
        this.fontSize.setKey(font.size);
        this.fontBold.setSelected(font.bold);
        this.fontItalic.setSelected(font.italic);
        this.fontUnderline.setSelected(font.underline);
        this.textColor.setKey(textProps.color);
        this.horAlignLeft.setSelected(textProps.horAlign == "Left");
        this.horAlignCenter.setSelected(textProps.horAlign == "Center");
        this.horAlignRight.setSelected(textProps.horAlign == "Right");

        textContainer.value = StiBase64.decode(textProps.text);
        textContainer.update();
        textContainer.focus();
    }

    return form;
}
