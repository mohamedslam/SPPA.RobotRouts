
StiMobileDesigner.prototype.InitializeEditImageElementForm_ = function () {
    var form = this.DashboardBaseForm("editImageElementForm", this.loc.Components.StiImage, 1, this.HelpLinks["imageElement"]);
    form.isDockableToComponent = true;
    form.caption.style.padding = "0px 10px 0 12px";
    form.container.style.borderTop = "0px";
    form.hideButtonsPanel();
    var jsObject = this;

    //src container
    var imageSrcContainer = this.ImageControl(null, 203, 200);
    imageSrcContainer.style.margin = "12px";
    form.container.appendChild(imageSrcContainer);

    //separator or
    var separator = this.SeparatorOr();
    separator.style.margin = "16px 12px 8px 12px";
    form.container.appendChild(separator);

    //icon
    var iconText = document.createElement("div");
    iconText.innerHTML = this.loc.PropertyMain.Icon;
    iconText.className = "stiDesignerTextContainer";
    iconText.style.padding = "4px 0 4px 12px";
    form.container.appendChild(iconText);

    var iconTable = this.CreateHTMLTable();
    iconTable.style.width = "100%";
    form.container.appendChild(iconTable);

    var iconControl = this.IconControl("imageElementIcon", 80, null, null, null, true);
    iconControl.style.marginLeft = "12px";
    iconTable.addCell(iconControl);

    iconControl.action = function () {
        form.setPropertyValue("icon", this.key);
    }

    var iconColorControl = this.ColorControl("imageElementIconColor", null, true, 80, true);
    iconColorControl.style.marginRight = "12px";
    iconTable.addCell(iconColorControl).style.width = "1px";

    iconColorControl.action = function () {
        form.setPropertyValue("iconColor", this.key);
        iconControl.textBox.style.color = jsObject.GetHTMLColor(iconColorControl.key);
    }

    //separator or
    var separator2 = this.SeparatorOr();
    separator2.style.margin = "16px 12px 8px 12px";
    form.container.appendChild(separator2);

    //hyperlink
    var hLinkText = document.createElement("div");
    hLinkText.innerHTML = this.loc.PropertyMain.Hyperlink;
    hLinkText.className = "stiDesignerTextContainer";
    hLinkText.style.padding = "4px 0 4px 12px";
    form.container.appendChild(hLinkText);

    var hLinkControl = this.ExpressionControl(null, 200, null, false);
    hLinkControl.style.margin = "0 12px 12px 12px";
    form.container.appendChild(hLinkControl);

    hLinkControl.textBox.onmouseup = function () {
        if (jsObject.options.itemInDrag) {
            var originalItem = jsObject.options.itemInDrag.originalItem;
            if (originalItem.itemObject.typeItem == "Column" && (originalItem.itemObject.type == "byte[]" || originalItem.itemObject.type == "image")) {
                var text = jsObject.options.itemInDrag.originalItem.getResultForEditForm();
                if (text.indexOf("{") == 0 && jsObject.EndsWith(text, "}")) {
                    text = text.substr(1, text.length - 2);
                }
                hLinkControl.textBox.value = "datacolumn://" + text;
                hLinkControl.action();
            }
        }
    }

    hLinkControl.button.action = function () {
        jsObject.InitializeExpressionEditorForm(function (expressionEditorForm) {
            var propertiesPanel = jsObject.options.propertiesPanel;
            expressionEditorForm.propertiesPanelZIndex = propertiesPanel.style.zIndex;
            expressionEditorForm.propertiesPanelIsEnabled = propertiesPanel.isEnabled;
            expressionEditorForm.resultControl = hLinkControl;
            expressionEditorForm.changeVisibleState(true);

            expressionEditorForm.expressionTextArea.insertButton.action = function () {
                var dictionaryTree = this.jsObject.options.dictionaryTree;
                if (dictionaryTree && dictionaryTree.selectedItem) {
                    var itemObject = dictionaryTree.selectedItem.itemObject;
                    if (itemObject.typeItem == "Column") {
                        var text = dictionaryTree.selectedItem.getResultForEditForm();
                        if (text.indexOf("{") == 0 && jsObject.EndsWith(text, "}")) {
                            text = text.substr(1, text.length - 2);
                        }
                        expressionEditorForm.expressionTextArea.insertText("datacolumn://" + text);
                        return;
                    }
                    var text = dictionaryTree.selectedItem.getResultForEditForm();
                    if (expressionEditorForm.resultControl) {
                        if (expressionEditorForm.resultControl.cutBrackets && text.indexOf("{") == 0 && this.jsObject.EndsWith(text, "}")) {
                            text = text.substr(1, text.length - 2);
                        }
                    }
                    expressionEditorForm.expressionTextArea.insertText(text);
                }
            }
        });
    }

    hLinkControl.action = function () {
        form.setPropertyValue("imageUrl", StiBase64.encode(this.textBox.value));
    }

    var saveButton = this.SmallButton(null, null, null, "Save.png", this.loc.MainMenu.menuFileSave.replace("&", ""), null, "stiDesignerFormButton");
    saveButton.style.position = "absolute";
    saveButton.style.right = "4px";
    saveButton.style.top = this.options.isTouchDevice ? "36px" : "31px";
    imageSrcContainer.buttonsCell.appendChild(saveButton);

    saveButton.action = function () {
        var imageSrc = !imageSrcContainer.src ? "" : imageSrcContainer.src;
        var imageData = jsObject.options.mvcMode ? encodeURIComponent(imageSrc) : imageSrc;
        if (imageData) jsObject.SendCommandDownloadImageContent(imageData);
    }

    var moveToResButton = this.SmallButton(null, null, null, "Resources.Resource.png", this.loc.Buttons.MoveToResource, null, "stiDesignerFormButton");
    moveToResButton.style.position = "absolute";
    moveToResButton.style.right = "4px";
    moveToResButton.style.top = this.options.isTouchDevice ? "63px" : "58px";
    imageSrcContainer.buttonsCell.appendChild(moveToResButton);

    moveToResButton.action = function () {
        var imageSrc = !imageSrcContainer.src ? "" : imageSrcContainer.src;
        var imageData = jsObject.options.mvcMode ? encodeURIComponent(imageSrc) : imageSrc;
        if (imageData) {
            var resourceName = jsObject.GetNewName("Resource", null, "Image");

            jsObject.SendCommandToDesignerServer("MoveImageToResource", { resourceName: resourceName, imageData: imageData }, function (answer) {
                if (answer.itemObject) {
                    jsObject.options.dictionaryTree.addResource(answer.itemObject);
                    jsObject.options.report.dictionary.resources = answer.resources;
                    jsObject.UpdateStateUndoRedoButtons();
                    imageSrcContainer.setImage(null);
                    imageSrcContainer.action();
                    hLinkControl.textBox.value = jsObject.options.resourceIdent + resourceName;
                    hLinkControl.action();
                }
            });
        }
    }

    imageSrcContainer.removeButton.style.top = this.options.isTouchDevice ? "90px" : "85px";

    imageSrcContainer.setImage_ = imageSrcContainer.setImage;

    imageSrcContainer.setImage = function (imageSrc, onLoadFunc) {
        this.setImage_(imageSrc, onLoadFunc);
        saveButton.setEnabled(imageSrc);
        moveToResButton.setEnabled(imageSrc);
    }

    imageSrcContainer.action = function () {
        form.setPropertyValue("imageSrc", this.src);
    }

    form.onshow = function () {
        imageSrcContainer.setImage(form.currentImageElement.properties.imageSrc);
        hLinkControl.textBox.value = StiBase64.decode(form.currentImageElement.properties.imageUrl);
        iconColorControl.setKey(form.currentImageElement.properties.iconColor);
        iconControl.setKey(form.currentImageElement.properties.icon);
        iconControl.textBox.style.color = form.jsObject.GetHTMLColor(iconColorControl.key);
    }

    form.setPropertyValue = function (propertyName, propertyValue) {
        form.sendCommand(
            {
                command: "SetPropertyValue",
                propertyName: propertyName,
                propertyValue: propertyValue
            },
            function (answer) {
                if (answer.elementProperties) {
                    form.updateSvgContent(answer.elementProperties.svgContent);
                    form.updateElementProperties(answer.elementProperties);
                }
            }
        );
    }

    form.updateSvgContent = function (svgContent) {
        this.currentImageElement.properties.svgContent = svgContent;
        this.currentImageElement.repaint();
    }

    form.updateElementProperties = function (properties) {
        var jsObject = this.jsObject;
        for (var propertyName in properties) {
            this.currentImageElement.properties[propertyName] = properties[propertyName];
        }
        jsObject.options.homePanel.updateControls();
        if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
    }

    form.sendCommand = function (updateParameters, callbackFunction) {
        form.jsObject.SendCommandToDesignerServer("UpdateImageElement",
            {
                componentName: form.currentImageElement.properties.name,
                updateParameters: updateParameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    return form;
}
