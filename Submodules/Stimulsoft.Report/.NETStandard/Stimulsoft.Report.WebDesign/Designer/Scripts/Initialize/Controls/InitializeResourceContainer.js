
StiMobileDesigner.prototype.ResourceContainer = function (name, width, height) {
    var jsObject = this;
    var resContainer = document.createElement("div");
    resContainer.className = "stiResourceContainerWithBorder";
    resContainer.loadedContent = null;
    resContainer.haveContent = false;
    resContainer.resourceType = null;
    resContainer.resourceName = null;
    var jsObject = resContainer.jsObject = this;
    this.AddProgressToControl(resContainer);

    if (name != null) this.options.controls[name] = resContainer;
    if (!width) width = 250;
    if (!height) height = 150;
    resContainer.progress.style.marginTop = (height / 2 - 50) + "px";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.width = "100%";
    resContainer.appendChild(innerTable);

    //Container
    var innerContainer = document.createElement("div");
    innerContainer.style.display = "none";
    innerContainer.className = "stiResourceInnerContainer";
    if (width) innerContainer.style.maxWidth = width + "px";
    if (height) innerContainer.style.maxHeight = height + "px";
    var contentCell = innerTable.addCell(innerContainer);
    contentCell.style.textAlign = "center";
    contentCell.style.width = width + "px";
    contentCell.style.height = height + "px";

    var hintText = document.createElement("div");
    hintText.className = "stiDesignerTextContainer stiDragAndDropHintText";
    hintText.innerHTML = this.loc.FormDictionaryDesigner.TextDropFileHere;
    contentCell.appendChild(hintText);

    //Buttons
    var buttonsCell = innerTable.addCell();
    buttonsCell.style.verticalAlign = "top";
    buttonsCell.style.width = "1px";

    var buttonProps = [
        ["openButton", "Open.png", this.loc.MainMenu.menuFileOpen.replace("&", "").replace("...", ""), "4px"],
        ["saveButton", "Save.png", this.loc.MainMenu.menuFileSave.replace("&", ""), "0 4px 4px 4px"],
        ["removeButton", "Remove.png", this.loc.MainMenu.menuEditDelete.replace("&", ""), "0 4px 4px 4px"],
        ["viewButton", "View.png", this.loc.Cloud.ButtonView, "0 4px 4px 4px"],
        ["editButton", "EditButton.png", this.loc.MainMenu.menuEditEdit, "0 4px 4px 4px"]
    ]

    for (var i = 0; i < buttonProps.length; i++) {
        var button = this.SmallButton(null, null, null, buttonProps[i][1], buttonProps[i][2], null, "stiDesignerFormButton");
        resContainer[buttonProps[i][0]] = button;
        button.style.margin = buttonProps[i][3];
        buttonsCell.appendChild(button);
    }

    resContainer.saveButton.action = function () {
        jsObject.SendCommandDownloadResource(resContainer.resourceName);
    }

    resContainer.removeButton.action = function () {
        resContainer.clear();
        resContainer.action();
        resContainer.onChange();
    }

    resContainer.editButton.action = function () {
        var showTextEditForm = function (text) {
            jsObject.InitializeTextEditorFormOnlyText(function (textEditorOnlyText) {

                textEditorOnlyText.showFunction = function () {
                    this.textArea.value = text;
                }

                textEditorOnlyText.actionFunction = function () {
                    var newContent = StiBase64.encode(this.textArea.value);
                    if (resContainer.loadedContent) {
                        newContent = "data:text/plain;base64," + newContent;
                        resContainer.setResource(newContent, resContainer.resourceType, resContainer.resourceName, resContainer.resourceSize, null, resContainer.haveContent);
                    }
                    else {
                        jsObject.SendCommandSetResourceText(resContainer.resourceName, newContent);
                        resContainer.setResource(null, resContainer.resourceType, resContainer.resourceName, resContainer.resourceSize, newContent, resContainer.haveContent);
                    }
                }

                textEditorOnlyText.changeVisibleState(true);
            });
        }
        if (resContainer.resourceType == "Rtf") {
            if (resContainer.loadedContent) {
                resContainer.setProgress(true);
                jsObject.SendCommandConvertResourceContent(resContainer.loadedContent, resContainer.resourceType, function (answer) {
                    resContainer.setProgress(false);

                    jsObject.InitializeRichTextEditorForm(function (form) {
                        form.show(StiBase64.decode(answer.content));

                        form.action = function () {
                            form.changeVisibleState(false);

                            jsObject.SendCommandToDesignerServer("GetRtfResourceContentFromHtmlText", { resourceText: StiBase64.encode(form.richTextEditor.getText()) },
                                function (answer) {
                                    resContainer.setResource(answer.resourceContent, resContainer.resourceType, resContainer.resourceName, resContainer.resourceSize, null, true);
                                });
                        }
                    });
                });
            }
            else {
                resContainer.setProgress(true);

                jsObject.SendCommandGetResourceText(resContainer.resourceName, function (answer) {
                    resContainer.setProgress(false);

                    if (answer.resourceText != null) {
                        jsObject.InitializeRichTextEditorForm(function (form) {
                            form.show(StiBase64.decode(answer.resourceText));

                            form.action = function () {
                                form.changeVisibleState(false);
                                jsObject.SendCommandSetResourceText(resContainer.resourceName, StiBase64.encode(form.richTextEditor.getText()), function (answer) {
                                    resContainer.setResource(null, resContainer.resourceType, resContainer.resourceName, resContainer.resourceSize, answer.resourceContent, resContainer.haveContent);
                                });
                            }
                        });
                    }
                });
            }
        }
        else if (resContainer.resourceType == "Map") {
            if (resContainer.loadedContent) {
                var text = StiBase64.decode(resContainer.loadedContent.substring(resContainer.loadedContent.indexOf("base64,") + "base64,".length));
                jsObject.InitializeEditCustomMapForm(function (form) {
                    form.resource = JSON.parse(text);
                    form.resourceName = resContainer.resourceName;
                    form.resourceContainer = resContainer;
                    form.changeVisibleState(true);
                });
            }
            else {
                resContainer.setProgress(true);
                jsObject.SendCommandGetResourceText(resContainer.resourceName, function (answer) {
                    resContainer.setProgress(false);
                    if (answer.resourceText != null) {
                        jsObject.InitializeEditCustomMapForm(function (form) {
                            form.resource = JSON.parse(StiBase64.decode(answer.resourceText));
                            form.resourceName = resContainer.resourceName;
                            form.resourceContainer = resContainer;
                            form.changeVisibleState(true);
                        });
                    }
                });
            }
        }
        else {
            if (resContainer.loadedContent) {
                var text = StiBase64.decode(resContainer.loadedContent.substring(resContainer.loadedContent.indexOf("base64,") + "base64,".length));
                showTextEditForm(text);
            }
            else {
                resContainer.setProgress(true);
                jsObject.SendCommandGetResourceText(resContainer.resourceName, function (answer) {
                    resContainer.setProgress(false);
                    if (answer.resourceText != null) {
                        showTextEditForm(StiBase64.decode(answer.resourceText));
                    }
                });
            }
        }
    }

    resContainer.viewButton.action = function () {
        resContainer.setProgress(true);
        jsObject.SendCommandGetResourceViewData(resContainer.resourceName, resContainer.resourceType, resContainer.loadedContent,
            function (answer) {
                resContainer.setProgress(false);
                if (answer.dataTables != null) {
                    jsObject.InitializeResourceViewDataForm(function (resourceViewDataForm) {
                        resourceViewDataForm.show(answer.dataTables, resContainer.resourceName);
                    });
                }
            });
    }

    resContainer.removeButton.setEnabled(false);
    resContainer.saveButton.setEnabled(false);

    var filesMask = ".bmp,.gif,.jpeg,.jpg,.png,.tiff,.ico,.emf,.wmf,.svg,.csv,.dbf,.xls,.xlsx,.json,.xml,.xsd,.rtf,.mrt,.mrz,.mdc,.mdz,.txt," +
        ".pdf,.doc,.docx,.ttf,.otf,.ttc,.map,.wkt";

    resContainer.openButton.action = function () {
        if (jsObject.options.canOpenFiles) {
            var openDialog = jsObject.InitializeOpenDialog("resourceContainerImageDialog", function (evt) {
                var files = evt.target.files;
                if (!jsObject.options.standaloneJsMode && files[0].size > jsObject.options.reportResourcesMaximumSize) {
                    var message = jsObject.loc.Notices.QuotaMaximumResourceSizeExceeded + "<br>" + jsObject.loc.PropertyMain.Maximum + ": " + jsObject.GetHumanFileSize(jsObject.options.reportResourcesMaximumSize, true);
                    if (jsObject.options.cloudMode) {
                        jsObject.InitializeNotificationForm(function (form) {
                            form.show(message, jsObject.NotificationMessages("upgradeYourPlan"), "Notifications.Resources.png");
                        });
                    }
                    else {
                        var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                        errorMessageForm.show(message, "Warning");
                    }
                    return;
                }

                for (var i = 0; i < files.length; i++) {
                    var f = files[i];
                    var reader = new FileReader();

                    reader.onload = (function (theFile) {
                        return function (e) {
                            jsObject.ResetOpenDialogs();
                            var resourceName = files[0].name.substring(0, files[0].name.lastIndexOf("."));
                            resContainer.setResource(e.target.result, resContainer.getResourceTypeByFileName(files[0].name), resourceName, files[0].size, null, true);
                            resContainer.action();
                            jsObject.ReturnFocusToDesigner();
                        };
                    })(f);

                    reader.readAsDataURL(f);
                }
            }, filesMask);
            openDialog.action();
        }
    }

    resContainer.action = function () { }

    resContainer.clear = function () {
        innerContainer.innerHTML = "";
        this.loadedContent = null;
        this.haveContent = false;
        this.openButton.setEnabled(true);
        this.removeButton.setEnabled(false);
        this.saveButton.setEnabled(false);
        this.viewButton.setEnabled(false);
        this.editButton.setEnabled(false);
        this.progress.hide();
        hintText.style.display = "";
        contentCell.style.textAlign = "center";
        contentCell.style.verticalAlign = "middle";
    }

    resContainer.setResource = function (loadedContent, type, name, size, contentFromServer, haveContent) {
        innerContainer.style.display = haveContent ? "" : "none";
        innerContainer.style.overflow = "auto";
        innerContainer.style.lineHeight = "2";

        this.clear();
        this.resourceType = type;
        this.resourceName = name;
        this.resourceSize = size;
        this.loadedContent = loadedContent;
        this.contentFromServer = contentFromServer;
        this.haveContent = haveContent;
        this.onChange();

        if (!haveContent) return;
        this.removeButton.setEnabled(true);
        this.saveButton.setEnabled(true);
        this.viewButton.setEnabled(this.isDataResourceType(type));
        this.editButton.setEnabled(this.isTextResourceType(type));
        hintText.style.display = "none";

        var addEasyContainerItem = function () {
            var imageName = StiMobileDesigner.checkImageSource(jsObject.options, "Resources.BigResource" + type + ".png")
                ? "Resources.BigResource" + type + ".png" : "Resources.BigResource.png";
            var item = jsObject.EasyContainerItem(name + ", " + jsObject.GetHumanFileSize(size, true), imageName);
            item.style.margin = "8px";
            contentCell.style.textAlign = "left";
            contentCell.style.verticalAlign = "top";
            innerContainer.style.overflow = "hidden";
            innerContainer.appendChild(item);
        }

        if (type == "Image" && (loadedContent || contentFromServer)) {
            var img = document.createElement("img");

            img.onerror = function () {
                img.style.display = "none";
                if (img.src && img.src.indexOf("data:image/x-wmf;") >= 0) {
                    jsObject.SendCommandToDesignerServer("ConvertMetaFileToPng", { fileContent: img.src }, function (answer) {
                        if (answer.fileContent) {
                            img.src = answer.fileContent;
                            img.style.display = "";
                        }
                    });
                }
            }

            img.src = loadedContent || contentFromServer;
            img.style.maxWidth = innerContainer.style.maxWidth;
            img.style.maxHeight = innerContainer.style.maxHeight;
            innerContainer.style.overflow = "hidden";
            innerContainer.appendChild(img);
        }
        else if (type == "Rtf" && (loadedContent || contentFromServer)) {
            var rtfContent = document.createElement("div");
            rtfContent.style.maxWidth = innerContainer.style.maxWidth;
            rtfContent.style.maxHeight = innerContainer.style.maxHeight;
            innerContainer.appendChild(rtfContent);

            if (contentFromServer) {
                rtfContent.innerHTML = StiBase64.decode(contentFromServer);
            }
            else if (loadedContent) {
                resContainer.setProgress(true);
                jsObject.SendCommandConvertResourceContent(loadedContent, type, function (answer) {
                    resContainer.setProgress(false);
                    if (answer.content) {
                        rtfContent.innerHTML = StiBase64.decode(answer.content);
                    }
                    else {
                        innerContainer.removeChild(rtfContent);
                        addEasyContainerItem();
                    }
                });
            }
        }
        else if (type == "Txt" && (loadedContent || contentFromServer)) {
            var txtContent = jsObject.TextArea(null, width - 10, height - 10);
            txtContent.style.border = "0";
            txtContent.readOnly = true;
            txtContent.style.cursor = "default";
            txtContent.style.background = "transparent";
            innerContainer.appendChild(txtContent);

            if (contentFromServer) {
                txtContent.value = StiBase64.decode(contentFromServer);
            }
            else if (loadedContent) {
                txtContent.value = StiBase64.decode(loadedContent.substring(loadedContent.indexOf("base64,") + "base64,".length));
            }
        }
        else if (type == "Report" || type == "ReportSnapshot") {
            var reportContent = document.createElement("div");
            reportContent.style.maxWidth = innerContainer.style.maxWidth;
            reportContent.style.maxHeight = innerContainer.style.maxHeight;
            reportContent.style.overflow = "hidden";
            innerContainer.appendChild(reportContent);

            if (contentFromServer) {
                reportContent.innerHTML = StiBase64.decode(contentFromServer);
            }
            else {
                resContainer.setProgress(true);
                jsObject.SendCommandConvertResourceContent(loadedContent, type, function (answer) {
                    resContainer.setProgress(false);
                    if (answer.content) {
                        reportContent.innerHTML = StiBase64.decode(answer.content);
                    }
                    else {
                        innerContainer.removeChild(reportContent);
                        addEasyContainerItem();
                    }
                });
            }
        }
        else if (type.indexOf("Font") == 0) {
            var fontContent = document.createElement("div");
            fontContent.style.maxWidth = innerContainer.style.maxWidth;
            fontContent.style.maxHeight = innerContainer.style.maxHeight;
            fontContent.style.overflow = "hidden";
            innerContainer.appendChild(fontContent);

            var paintFontSample = function (fontData, fontName) {
                var cssText = jsObject.GetCustomFontsCssText(fontData, fontName);
                var style = document.createElement("style");
                style.innerHTML = cssText;
                fontContent.style.fontFamily = fontName;
                fontContent.style.fontSize = "50";
                fontContent.innerHTML = fontName;
                fontContent.appendChild(style);
            }

            if (contentFromServer) {
                paintFontSample(contentFromServer, name);
            }
            else {
                resContainer.setProgress(true);
                jsObject.SendCommandConvertResourceContent(loadedContent, type, function (answer) {
                    resContainer.setProgress(false);
                    if (answer.content) {
                        paintFontSample(answer.content, name);
                    }
                    else {
                        innerContainer.removeChild(fontContent);
                        addEasyContainerItem();
                    }
                });
            }
        }
        else {
            addEasyContainerItem();
        }
    }

    resContainer.getResourceContentFromServer = function (resourceName) {
        resContainer.setProgress(true);
        jsObject.SendCommandGetResourceContent(resourceName, function (answer) {
            resContainer.setProgress(false);
            resContainer.setResource(null, answer.resourceType, answer.resourceName, answer.resourceSize, answer.resourceContent, answer.haveContent);
        });
    }

    var buttonsStates = {};

    resContainer.setProgress = function (state) {
        if (state)
            resContainer.progress.show();
        else
            resContainer.progress.hide();

        for (var i = 0; i < buttonProps.length; i++) {
            if (state) buttonsStates[buttonProps[i][0]] = resContainer[buttonProps[i][0]].isEnabled;
            resContainer[buttonProps[i][0]].setEnabled(state ? false : buttonsStates[buttonProps[i][0]]);
        }
    }

    resContainer.getResourceTypeByFileName = function (fileName) {
        fileName = fileName.toLowerCase();

        if (jsObject.EndsWith(fileName, ".csv")) return "Csv";
        else if (jsObject.EndsWith(fileName, ".dbf")) return "Dbf";
        else if (jsObject.EndsWith(fileName, ".xls") || jsObject.EndsWith(fileName, ".xlsx")) return "Excel";
        else if (jsObject.EndsWith(fileName, ".json")) return "Json";
        else if (jsObject.EndsWith(fileName, ".xml")) return "Xml";
        else if (jsObject.EndsWith(fileName, ".map")) return "Map";
        else if (jsObject.EndsWith(fileName, ".xsd")) return "Xsd";
        else if (jsObject.EndsWith(fileName, ".ttf")) return "FontTtf";
        else if (jsObject.EndsWith(fileName, ".otf")) return "FontOtf";
        else if (jsObject.EndsWith(fileName, ".woff")) return "FontWoff";
        else if (jsObject.EndsWith(fileName, ".ttc")) return "FontTtc";
        else if (jsObject.EndsWith(fileName, ".eot")) return "FontEot";
        else if (jsObject.EndsWith(fileName, ".rtf")) return "Rtf";
        else if (jsObject.EndsWith(fileName, ".txt")) return "Txt";
        else if (jsObject.EndsWith(fileName, ".pdf")) return "Pdf";
        else if (jsObject.EndsWith(fileName, ".wkt")) return "Gis";
        else if (jsObject.EndsWith(fileName, ".doc") || jsObject.EndsWith(fileName, ".docx")) return "Word";
        else if (jsObject.EndsWith(fileName, ".mrt") || jsObject.EndsWith(fileName, ".mrz")) return "Report";
        else if (jsObject.EndsWith(fileName, ".mdc") || jsObject.EndsWith(fileName, ".mdz")) return "ReportSnapshot";
        else if (jsObject.EndsWith(fileName, ".gif") || jsObject.EndsWith(fileName, ".png") || jsObject.EndsWith(fileName, ".jpeg") || jsObject.EndsWith(fileName, ".jpg") || jsObject.EndsWith(fileName, ".wmf") ||
            jsObject.EndsWith(fileName, ".bmp") || jsObject.EndsWith(fileName, ".tiff") || jsObject.EndsWith(fileName, ".ico") || jsObject.EndsWith(fileName, ".emf") || jsObject.EndsWith(fileName, ".svg")) return "Image";
        else return null;
    }

    resContainer.isDataResourceType = function (resourceType) {
        return (resourceType == "Json" ||
            resourceType == "Csv" ||
            resourceType == "Xml" ||
            resourceType == "Dbf" ||
            resourceType == "Excel");
    }

    resContainer.isTextResourceType = function (resourceType) {
        return (resourceType == "Json" ||
            resourceType == "Csv" ||
            resourceType == "Xml" ||
            resourceType == "Xsd" ||
            resourceType == "Txt" ||
            resourceType == "Map" ||
            (resourceType == "Rtf" && !jsObject.options.jsMode));
    }

    resContainer.isFontResourceType = function (resourceType) {
        return (resourceType == "FontOtf" ||
            resourceType == "FontTtc" ||
            resourceType == "FontTtf");
    }

    this.AddDragAndDropToContainer(resContainer, function (files, content) {
        if (!jsObject.options.standaloneJsMode && files[0].size > jsObject.options.reportResourcesMaximumSize) {
            var message = jsObject.loc.Notices.QuotaMaximumResourceSizeExceeded + "<br>" + jsObject.loc.PropertyMain.Maximum + ": " + jsObject.GetHumanFileSize(jsObject.options.reportResourcesMaximumSize, true);
            if (jsObject.options.cloudMode) {
                jsObject.InitializeNotificationForm(function (form) {
                    form.show(message, jsObject.NotificationMessage("upgradeYourPlan"), "Notifications.Resources.png");
                });
            }
            else {
                var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                errorMessageForm.show(message, "Warning");
            }
            return;
        }

        var resourceName = files[0].name.substring(0, files[0].name.lastIndexOf("."));
        var fileExt = files[0].name.toLowerCase().substring(files[0].name.lastIndexOf("."));
        if (filesMask.indexOf(fileExt) >= 0) {
            resContainer.setResource(content, resContainer.getResourceTypeByFileName(files[0].name.toLowerCase()), resourceName, files[0].size, null, true);
            resContainer.action();
        }
    });

    resContainer.onChange = function () { }

    return resContainer;
}