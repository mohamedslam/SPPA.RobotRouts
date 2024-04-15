
StiMobileDesigner.prototype.InitializeEditElectronicSignatureForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("editElectronicSignature", this.loc.Components.StiSignature, 1, this.HelpLinks["signature"]);
    form.controls = {};

    var tabs = [];
    tabs.push({ name: "type", caption: this.loc.PropertyMain.Type });
    tabs.push({ name: "draw", caption: this.loc.PropertyEnum.StiSignatureTypeDraw });

    var tabbedPane = this.TabbedPane("editSignatureTabbedPane", tabs, "stiDesignerStandartTab");
    tabbedPane.style.margin = "12px";
    form.container.appendChild(tabbedPane);

    for (var i = 0; i < tabs.length; i++) {
        var tabsPanel = tabbedPane.tabsPanels[tabs[i].name];
        tabsPanel.style.width = "350px";
        tabsPanel.style.height = "220px";
    }

    tabbedPane.tabsPanels.type.appendChild(this.SignatureFormTypePanel(form));
    tabbedPane.tabsPanels.draw.appendChild(this.SignatureFormDrawPanel(form));

    form.getCurrentSignatureMode = function () {
        return jsObject.UpperFirstChar(tabbedPane.selectedTab.panelName);
    }

    form.setCurrentSignatureMode = function (signatureMode) {
        tabbedPane.showTabPanel(jsObject.LowerFirstChar(signatureMode));
    }

    form.fill = function () {
        var props = this.currentComponent.properties;

        this.setCurrentSignatureMode(props.signatureMode);
        this.controls.fullName.value = props.typeFullName;
        this.controls.initials.value = props.typeInitials;
        this.controls.changeStyle.setStyleName(props.typeStyle);
        this.controls.sampleText.update();

        var drawContainer = this.controls.drawContainer;

        drawContainer.textProps = {
            text: props.drawText,
            font: props.drawTextFont,
            color: props.drawTextColor,
            horAlign: props.drawTextHorAlignment
        }

        drawContainer.imageProps = {
            image: props.drawImage,
            horAlignment: props.drawImageHorAlignment,
            vertAlignment: props.drawImageVertAlignment,
            stretch: props.drawImageStretch,
            aspectRatio: props.drawImageAspectRatio
        }

        drawContainer.update();
    }

    form.show = function (component) {
        this.currentComponent = component;
        this.changeVisibleState(true);

        if (!this.signatureStyles) {
            jsObject.SendCommandToDesignerServer("GetStylesForSignature", {}, function (answer) {
                if (answer.styles) {
                    for (var i = 0; i < answer.styles.length; i++) {
                        var style = answer.styles[i];
                        jsObject.AddCustomFontsCss(jsObject.GetCustomFontsCssText(style.fontContent, style.fontName));
                    }
                    form.signatureStyles = answer.styles;
                    form.fill();
                }
            });
        }
        else {
            form.fill();
        }
    }

    form.action = function () {
        this.changeVisibleState(false);
        var props = this.currentComponent.properties;

        props.signatureMode = this.getCurrentSignatureMode();
        props.typeFullName = this.controls.fullName.value;
        props.typeInitials = this.controls.initials.value;
        props.typeStyle = this.controls.changeStyle.styleName;

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

        jsObject.SendCommandSendProperties(this.currentComponent, ["signatureMode", "typeFullName", "typeInitials", "typeStyle", "drawImage", "drawImageHorAlignment", "drawImageVertAlignment",
            "drawImageStretch", "drawImageAspectRatio", "drawText", "drawTextFont", "drawTextColor", "drawTextHorAlignment"]);
    }

    return form;
}

StiMobileDesigner.prototype.SignatureFormTypePanel = function (form) {
    var jsObject = this;
    var table = this.CreateHTMLTable();
    table.style.width = "100%";

    //FullName
    var fullNameControl = this.TextBox(null, 220);
    form.addControlRow(table, this.loc.PropertyMain.FullName, "fullName", fullNameControl, "12px 12px 6px 12px", 30).style.textAlign = "right";

    //Initials
    var initialsControl = this.TextBox(null, 220);
    form.addControlRow(table, this.loc.PropertyMain.Initials, "initials", initialsControl, "6px 12px 6px 12px", 30).style.textAlign = "right";

    //ChangeStyle
    var changeStyleButton = this.SmallButton(null, null, this.loc.ChartRibbon.Style, null, null, "Down", "stiDesignerFormButton");
    changeStyleButton.style.display = "inline-block";
    form.addControlRow(table, " ", "changeStyle", changeStyleButton, "24px 12px 3px 12px", 50).style.textAlign = "right";

    changeStyleButton.setStyleName = function (styleName) {
        this.styleName = styleName;
    }

    var changeStyleMenu = this.VerticalMenu("editSignatureChangeStyle", changeStyleButton, "Down", [], "stiDesignerMenuMiddleItem");

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
            if (item.caption) {
                item.caption.style.fontSize = "16px";
                item.caption.style.fontFamily = item.key.fontName;
            }
            if (item.key.styleName == changeStyleButton.styleName) {
                item.setSelected(true);
            }
        }
    }

    //SampleText
    var sampleText = document.createElement("div");
    sampleText.className = "stiSimpleContainerWithBorder";
    sampleText.setAttribute("style", "height: 80px; overflow: hidden; font-size: 16pt; display: flex; justify-content: center; align-items: center;");
    form.addControlRow(table, null, "sampleText", sampleText, "0 12px 6px 12px");

    sampleText.update = function () {
        var styleName = changeStyleButton.styleName;
        for (var i = 0; i < form.signatureStyles.length; i++) {
            if (styleName == form.signatureStyles[i].styleName) {
                this.style.fontFamily = form.signatureStyles[i].fontName;
            }
        }
        this.innerText = getSignatureText();
    }

    changeStyleMenu.action = function (menuItem) {
        changeStyleButton.setStyleName(menuItem.key.styleName);
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

StiMobileDesigner.prototype.SignatureFormDrawPanel = function (form) {
    var jsObject = this;
    var panel = document.createElement("div");
    panel.setAttribute("style", "height: 100%;");

    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "6px 0 6px 12px";
    toolBar.style.display = "inline-block";
    panel.appendChild(toolBar);

    //Use Brush
    var useBrush = this.ColorControlWithImage("signatureFormUseBrush", "BorderColor.png");
    useBrush.style.display = "none";
    useBrush.style.marginRight = "4px";
    var cell = useBrush.button.innerTable.insertCell(1);
    cell.innerText = this.loc.Signature.UseBrush;
    toolBar.addCell(useBrush);

    //Insert Text
    var insertText = this.StandartSmallButton(null, null, null, "Signature.StiSignatureText.png");
    insertText.style.marginRight = "4px";
    toolBar.addCell(insertText);

    //Insert Image
    var insertImage = this.StandartSmallButton(null, null, null, "ContextMenu.StiImage.png");
    insertImage.style.marginRight = "4px";
    toolBar.addCell(insertImage);

    //Clear
    var clear = this.StandartSmallButton(null, null, null, "ClearAllFormatting.png", this.loc.Gui.monthcalendar_clearbutton);
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
        jsObject.InitializeSignatureImageForm(function (imageForm) {
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
        });
    }

    insertText.action = function () {
        jsObject.InitializeSignatureTextForm(function (textForm) {
            textForm.show(drawContainer.textProps);

            textForm.action = function () {
                var font = {
                    "name": textForm.fontName.key,
                    "size": textForm.fontSize.key,
                    "bold": textForm.fontBold.isSelected ? "1" : "0",
                    "italic": textForm.fontItalic.isSelected ? "1" : "0",
                    "underline": textForm.fontUnderline.isSelected ? "1" : "0",
                    "strikeout": "0"
                }
                drawContainer.textProps = {
                    text: StiBase64.encode(textForm.textContainer.value),
                    font: jsObject.FontObjectToStr(font),
                    color: textForm.textColor.key,
                    horAlign: textForm.horAlignLeft.isSelected ? "Left" : (textForm.horAlignRight.isSelected ? "Right" : "Center")
                }
                textForm.changeVisibleState(false);
                drawContainer.update();
            }
        });
    }

    return panel;
}

StiMobileDesigner.prototype.SignatureFormDrawContainer = function (form) {
    var jsObject = this;
    var panel = document.createElement("div");
    var width = "325px";
    var height = this.options.isTouchDevice ? "183px" : "188px"
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
    textCont.className = "stiSignatureFormDrawContainer";
    textCont.style.display = "flex";
    textCont.style.alignItems = "center";
    panel.appendChild(textCont);

    panel.update = function () {
        textCont.innerText = StiBase64.decode(this.textProps.text);

        var font = jsObject.FontStrToObject(this.textProps.font);
        textCont.style.fontFamily = font.name;
        textCont.style.fontSize = font.size + "px";
        textCont.style.fontWeight = font.bold == "1" ? "bold" : "normal";
        textCont.style.fontStyle = font.italic == "1" ? "italic" : "normal";
        textCont.style.textDecoration = font.underline == "1" ? "underline" : "";
        textCont.style.color = jsObject.GetHTMLColor(this.textProps.color);
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