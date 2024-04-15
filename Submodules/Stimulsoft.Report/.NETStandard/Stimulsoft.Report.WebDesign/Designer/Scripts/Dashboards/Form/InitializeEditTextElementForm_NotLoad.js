
StiMobileDesigner.prototype.InitializeEditTextElementForm_ = function () {
    var form = this.DashboardBaseForm("editTextElementForm", this.loc.Components.StiText, 1, this.HelpLinks["textElement"]);
    var jsObject = this;
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";
    form.hideButtonsPanel();

    var textElemEditor = this.TextElementEditor("editTextElementFormEditor", 600, 300);
    textElemEditor.richText.style.margin = "0px";
    textElemEditor.richText.style.border = "0px";
    textElemEditor.style.margin = "12px";
    form.container.appendChild(textElemEditor);

    textElemEditor.controls.fontList.action = function () {
        form.setProperty("fontName", this.key);
    }

    textElemEditor.controls.sizeList.action = function () {
        form.setProperty("fontSize", this.key);
    }

    textElemEditor.controls.buttonAlignLeft.action = function () {
        if (!textElemEditor.writingInProgress)
            form.setProperty("horAlignment", "Left");
    }

    textElemEditor.controls.buttonAlignCenter.action = function () {
        if (!textElemEditor.writingInProgress)
            form.setProperty("horAlignment", "Center");
    }

    textElemEditor.controls.buttonAlignRight.action = function () {
        if (!textElemEditor.writingInProgress)
            form.setProperty("horAlignment", "Right");
    }

    textElemEditor.controls.buttonAlignWidth.action = function () {
        if (!textElemEditor.writingInProgress)
            form.setProperty("horAlignment", "Width");
    }

    textElemEditor.getFontProperties = function () {
        var controls = textElemEditor.controls;
        var align = "Center";
        if (controls.buttonAlignLeft.isSelected) align = "Left";
        else if (controls.buttonAlignCenter.isSelected) align = "Center";
        else if (controls.buttonAlignRight.isSelected) align = "Right";
        else if (controls.buttonAlignWidth.isSelected) align = "Width";
        return {
            fontName: controls.fontList.key,
            fontSize: controls.sizeList.key,
            horAlignment: align
        };
    }

    textElemEditor.controls.buttonClearFormatting.action = function () {
        textElemEditor.richText.selectAllText();
        form.setProperty("clearAllFormatting");
        if (form.currentTextElement) {
            form.currentTextElement.properties.vertAlignment = "Center";
            jsObject.SendCommandSendProperties([form.currentTextElement], ["vertAlignment"]);
        }
    }

    textElemEditor.checkCurrentText = function () {
        this.needToRewrite = false;
        var text = textElemEditor.getText();
        if (text.indexOf("<") > 0) {
            this.needToRewrite = true;
        }
        return StiBase64.encode(text);
    }

    textElemEditor.setPropertyText = function () {
        form.setProperty("text", textElemEditor.checkCurrentText(), true);
        if (this.needToRewrite) {
            this.needToRewrite = false;
            form.setProperty("fontSize", textElemEditor.controls.sizeList.key);
        }
    }

    textElemEditor.onchange = function () {
        textElemEditor.setPropertyText();
    };

    textElemEditor.onchangetext = function () {
        textElemEditor.setPropertyText();
    };

    form.updateControls = function () {
        var controls = textElemEditor.controls;
        var font = form.currentTextElement.properties.font ? jsObject.FontStrToObject(form.currentTextElement.properties.font) : null;
        if (form.currentTextElement.properties.text != null) {
            textElemEditor.setText(StiBase64.decode(form.currentTextElement.properties.text));
        }
        if (font) {
            controls.fontList.setKey(font.name);
            controls.sizeList.setKey(font.size);
            controls.buttonStyleBold.setSelected(font.bold == "1");
            controls.buttonStyleItalic.setSelected(font.italic == "1");
            controls.buttonStyleUnderline.setSelected(font.underline == "1");
        }
        var horAlignment = form.currentTextElement.properties.horAlignment;
        if (horAlignment) {
            controls.buttonAlignLeft.setSelected(horAlignment == "Left");
            controls.buttonAlignCenter.setSelected(horAlignment == "Center");
            controls.buttonAlignRight.setSelected(horAlignment == "Right");
            controls.buttonAlignWidth.setSelected(horAlignment == "Width");
        }
    }

    form.onshow = function () {
        textElemEditor.onLoadComplete = function () {
            var body = textElemEditor.richText.doc.getElementsByTagName("body")[0];
            if (body) {
                body.style.color = jsObject.GetHTMLColor(form.currentTextElement.properties.realForeColor);

                var horAlignment = form.currentTextElement.properties.horAlignment;
                horAlignment = horAlignment == "Width" ? "left" : horAlignment.toLowerCase();
                body.style.textAlign = horAlignment;

                var font = form.currentTextElement.properties.font ? jsObject.FontStrToObject(form.currentTextElement.properties.font) : null;
                if (font) {
                    body.style.fontFamily = font.name;
                    body.style.fontSize = font.size;
                }
            }
        }
        form.updateControls();
        textElemEditor.richText.style.background = jsObject.GetHTMLColor(form.currentTextElement.properties.realBackColor);
    }

    form.setProperty = function (propertyName, propertyValue, notUpdateControls) {
        if (propertyName == "fontName" || propertyName == "fontSize" || propertyName == "horAlignment") {
            var fontProps = textElemEditor.getFontProperties();
            fontProps[propertyName] = propertyValue;
            propertyName = "fontProperties";
            propertyValue = fontProps;
        }
        form.sendCommandToTextElement(
            {
                command: "SetProperty",
                propertyName: propertyName,
                propertyValue: propertyValue
            },
            function (answer) {
                for (var propName in answer.elementProperties) {
                    form.currentTextElement.properties[propName] = answer.elementProperties[propName];
                }
                form.currentTextElement.repaint();
                form.jsObject.options.homePanel.updateControls();
                if (!notUpdateControls) form.updateControls();
            }
        );
    }

    form.sendCommandToTextElement = function (updateParameters, callbackFunction) {
        form.jsObject.SendCommandToDesignerServer("UpdateTextElement",
            {
                componentName: form.currentTextElement.properties.name,
                updateParameters: updateParameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    return form;
}
