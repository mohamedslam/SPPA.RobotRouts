
StiMobileDesigner.prototype.InitializeSignatureTextForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("signatureText", this.loc.Components.StiText, 3);

    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "12px";
    form.container.appendChild(toolBar);

    var controls = [
        ["fontName", this.FontList("signTextFontName", 100), "0"],
        ["fontSize", this.DropDownList("signTextFontSize", 40, null, this.GetFontSizeItems(), false), "4px"],
        ["fontBold", this.StandartSmallButton(null, null, null, "Bold.png", this.loc.HelpDesigner.FontStyleBold), "12px"],
        ["fontItalic", this.StandartSmallButton(null, null, null, "Italic.png", this.loc.HelpDesigner.FontStyleItalic), "4px"],
        ["fontUnderline", this.StandartSmallButton(null, null, null, "Underline.png", this.loc.HelpDesigner.FontStyleUnderline), "4px"],
        ["textColor", this.ColorControlWithImage("signTextColor", "TextColor.png", null, true), "12px"],
        ["horAlignLeft", this.StandartSmallButton("signTextAlignLeft", "SignatureTextHorAlign", null, "AlignLeft.png", this.loc.Toolbars.AlignLeft), "12px"],
        ["horAlignCenter", this.StandartSmallButton("signTextAlignCenter", "SignatureTextHorAlign", null, "AlignCenter.png", this.loc.Toolbars.AlignCenter), "4px"],
        ["horAlignRight", this.StandartSmallButton("signTextAlignRight", "SignatureTextHorAlign", null, "AlignRight.png", this.loc.Toolbars.AlignRight), "4px"]
    ]

    for (var i = 0; i < controls.length; i++) {
        var control = controls[i][1];
        var controlName = control.controlName = controls[i][0];
        form[controlName] = control;
        toolBar.addCell(control);
        control.style.marginLeft = controls[i][2];

        control.action = function () {
            if (this.controlName == "horAlignLeft" || this.controlName == "horAlignCenter" || this.controlName == "horAlignRight") {
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
        this.style.color = jsObject.GetHTMLColor(form.textColor.key);
        this.style.textAlign = form.horAlignLeft.isSelected ? "left" : (form.horAlignRight.isSelected ? "right" : "center");
        this.style.fontWeight = form.fontBold.isSelected ? "bold" : "normal";
        this.style.fontStyle = form.fontItalic.isSelected ? "italic" : "normal";
        this.style.textDecoration = form.fontUnderline.isSelected ? "underline" : "";
    }

    form.show = function (textProps) {
        this.changeVisibleState(true);

        var font = jsObject.FontStrToObject(textProps.font);
        this.fontName.setKey(font.name);
        this.fontSize.setKey(font.size);
        this.fontBold.setSelected(font.bold == "1");
        this.fontItalic.setSelected(font.italic == "1");
        this.fontUnderline.setSelected(font.underline == "1");
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
