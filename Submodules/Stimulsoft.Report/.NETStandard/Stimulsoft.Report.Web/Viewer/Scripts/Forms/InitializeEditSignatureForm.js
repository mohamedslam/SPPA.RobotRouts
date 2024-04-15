
StiJsViewer.prototype.InitializeEditSignatureForm = function () {
    var jsObject = this;
    var form = this.BaseForm("editSignature", this.collections.loc["Signature"], 1);
    form.container.style.padding = "1px";
    form.controls = {};

    //Ovveride Buttons Panel 
    while (form.buttonsPanel.childNodes[0]) form.buttonsPanel.removeChild(form.buttonsPanel.childNodes[0]);
    var buttonsTable = this.CreateHTMLTable();
    form.buttonsPanel.appendChild(buttonsTable);

    //Back
    var buttonBack = this.FormButton(null, this.collections.loc["ButtonBack"]);
    buttonsTable.addCell(buttonBack).style.padding = "12px 0 12px 12px";

    buttonBack.action = function () {
        form.saveStep(form.stepIndex);
        form.showStep(form.stepIndex - 1);
        form.updateNavigateButtons();
    };

    //Next
    var buttonNext = this.FormButton(null, this.collections.loc["ButtonNext"]);
    buttonsTable.addCell(buttonNext).style.padding = "12px";

    buttonNext.action = function () {
        form.saveStep(form.stepIndex);
        form.showStep(form.stepIndex + 1);
        form.updateNavigateButtons();
    };

    //Sign
    var buttonSign = this.FormButton(null, this.collections.loc["ButtonSign"], null, null, "stiJsViewerFormButtonTheme");
    buttonsTable.addCell(buttonSign).style.padding = "12px 0 12px 12px";

    buttonSign.action = function () {
        form.action();
    };

    //Cancel
    var buttonCancel = this.FormButton(null, this.collections.loc["ButtonCancel"]);
    buttonsTable.addCell(buttonCancel).style.padding = "12px";

    buttonCancel.action = function () {
        form.changeVisibleState(false);
    };

    var tabs = [];
    tabs.push({ name: "type", caption: this.collections.loc["Type"] });
    tabs.push({ name: "draw", caption: this.collections.loc["Draw"] });

    var tabbedPane = this.TabbedPane("editSignatureTabbedPane", tabs, "stiJsViewerStandartTab");
    tabbedPane.style.margin = "12px";
    form.container.appendChild(tabbedPane);

    for (var i = 0; i < tabs.length; i++) {
        var tabsPanel = tabbedPane.tabsPanels[tabs[i].name];
        tabsPanel.style.width = "400px";
        tabsPanel.style.height = "220px";
    }

    tabbedPane.tabsPanels.type.appendChild(this.SignatureFormTypePanel(form));
    tabbedPane.tabsPanels.draw.appendChild(this.SignatureFormDrawPanel(form));

    form.getCurrentSignatureMode = function () {
        return jsObject.upperFirstChar(tabbedPane.selectedTab.panelName);
    }

    form.setCurrentSignatureMode = function (signatureMode) {
        tabbedPane.showTabPanel(jsObject.lowerFirstChar(signatureMode));
    }

    form.fill = function (component) {
        this.setCurrentSignatureMode(component.signatureMode);
        this.controls.fullName.value = component.typeFullName;
        this.controls.initials.value = component.typeInitials;
        this.controls.changeStyle.setTypeStyle(component.typeStyle);
        this.controls.sampleText.update();

        var drawContainer = this.controls.drawContainer;

        drawContainer.textProps = {
            text: component.drawText,
            font: component.drawTextFont,
            color: component.drawTextColor,
            horAlign: component.drawTextHorAlignment
        }

        drawContainer.imageProps = {
            image: component.drawImage,
            horAlignment: component.drawImageHorAlignment,
            vertAlignment: component.drawImageVertAlignment,
            stretch: component.drawImageStretch,
            aspectRatio: component.drawImageAspectRatio
        }

        drawContainer.update();
    }

    form.updateNavigateButtons = function () {
        buttonBack.style.display = buttonNext.style.display = this.signatures.length > 1 ? "" : "none";
        buttonBack.setEnabled(this.stepIndex > 0);
        buttonNext.setEnabled(this.stepIndex < this.signatures.length - 1);
        buttonSign.setEnabled(this.signatures.length > 0);
    }

    form.showStep = function (stepIndex) {
        this.stepIndex = stepIndex;

        if (stepIndex < this.signatures.length) {
            this.fill(this.signatures[stepIndex]);
        }
    };

    form.saveStep = function (stepIndex) {
        if (stepIndex < this.signatures.length) {
            var props = this.signatures[stepIndex];

            props.signatureMode = this.getCurrentSignatureMode();
            props.typeFullName = this.controls.fullName.value;
            props.typeInitials = this.controls.initials.value;
            props.typeStyle = this.controls.changeStyle.typeStyle;

            var imageProps = this.controls.drawContainer.imageProps;
            props.drawImage = imageProps.image;
            props.drawImageHorAlignment = imageProps.horAlignment;
            props.drawImageVertAlignment = imageProps.vertAlignment;
            props.drawImageStretch = imageProps.stretch;
            props.drawImageAspectRatio = imageProps.aspectRatio;

            var textProps = this.controls.drawContainer.textProps;
            props.drawText = textProps.text;
            props.drawTextFont = textProps.font;
            props.drawTextColor = textProps.color;
            props.drawTextHorAlignment = textProps.horAlign;
        }
    };

    form.show = function () {
        this.changeVisibleState(true);
        this.stepIndex = 0;
        this.signatures = [];
        this.updateNavigateButtons();

        jsObject.postAjax(jsObject.getActionRequestUrl(jsObject.options.requestUrl, jsObject.options.actions.viewerEvent),
            {
                action: "GetSignatureData",
                getStyles: !form.signatureStyles
            },
            function (answer) {
                if (answer) {
                    var data = JSON.parse(jsObject.options.server.useCompression ? StiGZipHelper.unpack(answer) : answer);

                    if (data.styles) {
                        for (var i = 0; i < data.styles.length; i++) {
                            var style = data.styles[i];

                            jsObject.addCustomFontStyles([{
                                contentForCss: style.fontContent,
                                originalFontFamily: style.fontName
                            }]);
                        }
                        form.signatureStyles = data.styles;
                    }

                    if (data.signatures) {
                        form.signatures = data.signatures;
                        form.updateNavigateButtons();

                        if (data.signatures.length > 0) {
                            form.showStep(0);
                        }
                    }
                }
            });
    }

    form.action = function () {        
        this.saveStep(this.stepIndex);
        jsObject.postInteraction({ action: "Signatures", signatures: this.signatures });

        this.changeVisibleState(false);
    }

    return form;
}

StiJsViewer.prototype.SignatureFormTypePanel = function (form) {
    var jsObject = this;
    var table = this.CreateHTMLTable();
    table.style.width = "100%";

    //FullName
    var fullNameControl = this.TextBox(null, 220);
    form.addControlRow(table, this.collections.loc["FullName"], "fullName", fullNameControl, "12px 12px 6px 12px", 30).style.textAlign = "right";

    //Initials
    var initialsControl = this.TextBox(null, 220);
    form.addControlRow(table, this.collections.loc["Initials"], "initials", initialsControl, "6px 12px 6px 12px", 30).style.textAlign = "right";

    //ChangeStyle
    var changeStyleButton = this.SmallButton(null, this.collections.loc["Style"], null, null, "Down", "stiJsViewerFormButton");
    changeStyleButton.style.display = "inline-block";
    form.addControlRow(table, " ", "changeStyle", changeStyleButton, "24px 12px 3px 12px", 50).style.textAlign = "right";

    changeStyleButton.setTypeStyle = function (typeStyle) {
        this.typeStyle = typeStyle;
    }

    var changeStyleMenu = this.VerticalMenu("editSignatureChangeStyle", changeStyleButton, "Down", []);

    changeStyleButton.action = function () {
        changeStyleMenu.changeVisibleState(!changeStyleMenu.visible);
    }

    var getSignatureText = function () {
        return (fullNameControl.value || initialsControl.value ? fullNameControl.value + "  " + initialsControl.value : "FullName FN");
    }

    changeStyleMenu.onshow = function () {
        this.clear();

        var items = [];
        var styles = form.signatureStyles;

        if (styles) {
            for (var i = 0; i < styles.length; i++) {
                items.push(jsObject.Item("item" + i, getSignatureText(), null, styles[i]));
            }
        }

        this.addItems(items);

        for (var itemKey in this.items) {
            var item = this.items[itemKey];
            item.style.height = "40px";

            if (item.caption) {
                item.caption.style.fontSize = "16px";
                item.caption.style.fontFamily = item.key.fontName;
            }
            if (item.key.styleName == changeStyleButton.typeStyle) {
                item.setSelected(true);
            }
        }
    }

    //SampleText
    var sampleText = document.createElement("div");
    sampleText.className = "stiJsViewerSimpleContainerWithBorder";
    sampleText.setAttribute("style", "height: 80px; overflow: hidden; font-size: 16pt; display: flex; justify-content: center; align-items: center;");
    form.addControlRow(table, null, "sampleText", sampleText, "0 12px 6px 12px");

    sampleText.update = function () {
        var typeStyle = changeStyleButton.typeStyle;
        for (var i = 0; i < form.signatureStyles.length; i++) {
            if (typeStyle == form.signatureStyles[i].styleName) {
                this.style.fontFamily = form.signatureStyles[i].fontName;
            }
        }
        this.innerText = getSignatureText();
    }

    changeStyleMenu.action = function (menuItem) {
        changeStyleButton.setTypeStyle(menuItem.key.styleName);
        sampleText.update();
        this.changeVisibleState(false);
    }

    fullNameControl.onchange = function () {
        sampleText.update();
    }

    initialsControl.onchange = function () {
        sampleText.update();
    }

    return table;
}

StiJsViewer.prototype.SignatureFormDrawPanel = function (form) {
    var jsObject = this;
    var panel = document.createElement("div");
    panel.setAttribute("style", "height: 100%;");

    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "6px 0 6px 12px";
    toolBar.style.display = "inline-block";
    panel.appendChild(toolBar);

    //Use Brush
    var useBrush = this.ColorControlWithImage("signatureFormUseBrush", "ColorControl.BorderColor.png");
    useBrush.style.display = "none";
    useBrush.style.marginRight = "4px";
    var cell = useBrush.button.innerTable.insertCell(1);
    cell.innerText = this.collections.loc["UseBrush"];
    toolBar.addCell(useBrush);

    //Insert Text
    var insertText = this.SmallButton(null, this.collections.loc["InsertText"], "SignatureForm.Text.png");
    insertText.style.marginRight = "4px";
    toolBar.addCell(insertText);

    //Insert Image
    var insertImage = this.SmallButton(null, this.collections.loc["InsertImage"], "SignatureForm.Image.png");
    insertImage.style.marginRight = "4px";
    toolBar.addCell(insertImage);

    //Clear
    var clear = this.SmallButton(null, null, "SignatureForm.ClearAllFormatting.png", this.collections.loc["Clear"]);
    clear.style.marginRight = "4px";
    toolBar.addCell(clear);

    clear.action = function () {
        drawContainer.imageProps.image = "";
        drawContainer.textProps.text = "";
        drawContainer.update();
    }

    //DrawContainer
    var drawContainer = this.SignatureFormDrawContainer(form);
    panel.appendChild(drawContainer);

    insertImage.action = function () {
        var imageForm = jsObject.controls.forms.signatureImage || jsObject.InitializeSignatureImageForm();
        imageForm.show(drawContainer.imageProps);

        imageForm.action = function () {
            drawContainer.imageProps = {
                image: imageForm.imageSrcContainer.src,
                horAlignment: imageForm.horAlignLeft.isSelected ? "Left" : (imageForm.horAlignCenter.isSelected ? "Center" : "Right"),
                vertAlignment: imageForm.vertAlignTop.isSelected ? "Top" : (imageForm.vertAlignMiddle.isSelected ? "Center" : "Bottom"),
                stretch: imageForm.stretch.isChecked,
                aspectRatio: imageForm.aspectRatio.isChecked
            }
            imageForm.changeVisibleState(false);
            drawContainer.update();
        }
    }

    insertText.action = function () {
        var textForm = jsObject.controls.forms.signatureText || jsObject.InitializeSignatureTextForm();
        textForm.show(drawContainer.textProps);

        textForm.action = function () {
            var font = {
                name: textForm.fontName.key,
                size: textForm.fontSize.key,
                bold: textForm.fontBold.isSelected,
                italic: textForm.fontItalic.isSelected,
                underline: textForm.fontUnderline.isSelected,
                strikeout: false
            }
            drawContainer.textProps = {
                text: StiBase64.encode(textForm.textContainer.value),
                font: font,
                color: textForm.textColor.key,
                horAlign: textForm.horAlignLeft.isSelected ? "Left" : (textForm.horAlignRight.isSelected ? "Right" : "Center")
            }
            textForm.changeVisibleState(false);
            drawContainer.update();
        }
    }

    return panel;
}

StiJsViewer.prototype.SignatureFormDrawContainer = function (form) {
    var jsObject = this;
    var panel = document.createElement("div");
    var width = "375px";
    var height = this.options.isTouchDevice ? "170px" : "175px"
    panel.setAttribute("style", "margin: 0 12px 0 12px; position: relative; line-height: 0; width: " + width + "; height: " + height + ";");
    form.controls.drawContainer = panel;

    var imgTable = this.CreateHTMLTable();
    panel.appendChild(imgTable);
    var img = document.createElement("img");
    img.style.maxWidth = width;
    img.style.maxHeight = height;
    var imgCell = imgTable.addCell(img);
    imgCell.style.width = width;
    imgCell.style.height = height;

    var textCont = document.createElement("div");
    textCont.className = "stiJsViewerSignatureFormDrawContainer";
    textCont.style.display = "flex";
    textCont.style.alignItems = "center";
    panel.appendChild(textCont);

    panel.update = function () {
        textCont.innerText = StiBase64.decode(this.textProps.text);

        var font = this.textProps.font;
        textCont.style.fontFamily = font.name;
        textCont.style.fontSize = font.size + "px";
        textCont.style.fontWeight = font.bold ? "bold" : "normal";
        textCont.style.fontStyle = font.italic ? "italic" : "normal";
        textCont.style.textDecoration = font.underline ? "underline" : "";
        textCont.style.color = jsObject.getHTMLColor(this.textProps.color);
        textCont.style.justifyContent = textCont.style.textAlign = this.textProps.horAlign.toLowerCase();

        img.style.display = this.imageProps.image ? "" : "none";
        if (this.imageProps.image) img.src = this.imageProps.image;

        img.onload = function () {
            img.style.width = panel.imageProps.stretch ? width : "auto";
            img.style.height = panel.imageProps.stretch ? height : "auto";

            if (panel.imageProps.stretch && panel.imageProps.aspectRatio) {
                img.style.height = img.offsetHeight > 0 && img.offsetHeight > img.offsetWidth ? width : "auto";
                img.style.width = img.offsetHeight > 0 && img.offsetHeight > img.offsetWidth ? "auto" : height;
            }

            imgCell.style.textAlign = panel.imageProps.horAlignment.toLowerCase();
            imgCell.style.verticalAlign = panel.imageProps.vertAlignment == "Top" ? "top" : (panel.imageProps.vertAlignment == "Bottom" ? "bottom" : "middle");
        }
    }

    return panel
}