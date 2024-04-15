
StiMobileDesigner.prototype.ImageControl = function (name, width, height, hideButtons, easyType) {
    var jsObject = this;
    var imageControl = document.createElement("div");
    imageControl.src = null;
    imageControl.jsObject = this;
    imageControl.isEnabled = true;
    imageControl.className = "stiImageContainerWithBorder";

    if (name != null) this.options.controls[name] = imageControl;
    if (!width) width = 250;
    if (!height) height = 100;
    imageControl.style.width = width + "px";
    imageControl.style.height = height + "px";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.width = "100%";
    imageControl.appendChild(innerTable);

    var imageContainer = document.createElement("img");
    imageContainer.style.display = "none";
    imageContainer.style.maxWidth = width + "px";
    imageContainer.style.maxHeight = height + "px";
    imageControl.imageContainer = imageContainer;

    imageContainer.onerror = function () {
        imageContainer.style.display = "none";
        if (imageContainer.src && imageContainer.src.indexOf("data:image/x-wmf;") >= 0) {
            jsObject.SendCommandToDesignerServer("ConvertMetaFileToPng", { fileContent: imageContainer.src }, function (answer) {
                if (answer.fileContent) {
                    imageContainer.src = answer.fileContent;
                    imageContainer.style.display = "";
                }
            });
        }
    }

    imageContainer.isValidSrc = function (src) {
        return (src && src.indexOf("{") != 0)
    }

    var imageCell = innerTable.addCell();
    imageCell.style.width = width + "px";
    imageCell.style.height = height + "px";
    imageCell.style.textAlign = "center";
    imageCell.appendChild(imageContainer);

    var filesMask = ".bmp,.gif,.jpeg,.jpg,.png,.tiff,.ico,.emf,.wmf,.svg";

    if (!easyType) {
        var hintText = document.createElement("div");
        hintText.className = "stiDesignerTextContainer stiDragAndDropHintText";
        hintText.style.margin = "0";
        hintText.innerHTML = this.loc.FormDictionaryDesigner.TextDropImageHere;
        imageControl.hintText = hintText;
        imageCell.appendChild(hintText);

        if (!hideButtons) {
            var openButton = this.SmallButton(null, null, null, "Open.png", this.loc.MainMenu.menuFileOpen.replace("&", "").replace("...", ""), null, "stiDesignerFormButton");
            openButton.style.position = "absolute";
            openButton.style.right = "4px";
            openButton.style.top = "4px";
            imageControl.openButton = openButton;

            var removeButton = this.SmallButton(null, null, null, "Remove.png", this.loc.MainMenu.menuEditDelete.replace("&", ""), null, "stiDesignerFormButton");
            removeButton.style.position = "absolute";
            removeButton.style.right = "4px";
            removeButton.style.top = this.options.isTouchDevice ? "37px" : "33px";
            imageContainer.removeButton = imageControl.removeButton = removeButton;
            removeButton.setEnabled(false);

            var buttonsCell = imageControl.buttonsCell = innerTable.addCell();
            buttonsCell.style.width = "1px";
            buttonsCell.style.position = "relative";
            buttonsCell.appendChild(openButton);
            buttonsCell.appendChild(removeButton);

            removeButton.action = function () {
                imageControl.setImage(null);
                imageControl.action();
            }

            openButton.action = function () {
                if (jsObject.options.canOpenFiles) {
                    var openDialog = jsObject.InitializeOpenDialog("imageControlImageDialog", function (evt) {
                        var files = evt.target.files;

                        for (var i = 0; i < files.length; i++) {
                            var f = files[i];
                            var reader = new FileReader();

                            reader.onload = (function (theFile) {
                                return function (e) {
                                    if (!jsObject.options.standaloneJsMode && theFile.size > jsObject.options.reportResourcesMaximumSize) {
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
                                    jsObject.ResetOpenDialogs();
                                    imageControl.setImage(e.target.result);
                                    imageControl.action(files, e.target.result, evt.target.value);
                                    jsObject.ReturnFocusToDesigner();
                                };
                            })(f);

                            reader.readAsDataURL(f);
                        }
                    }, filesMask);
                    openDialog.action();
                }
            }
        }

        imageControl.action = function () { }

        imageControl.setImage = function (imageSrc, onLoadFunc) {
            imageContainer.style.display = imageSrc ? "" : "none";
            hintText.style.display = imageSrc ? "none" : "";

            if (onLoadFunc) {
                imageContainer.onload = function () {
                    onLoadFunc();
                }
            }

            if (imageSrc && imageContainer.isValidSrc(imageSrc)) {
                imageContainer.src = imageSrc;
            }
            else {
                imageContainer.removeAttribute("src");
            }

            imageControl.src = imageSrc;

            if (imageContainer.removeButton) {
                removeButton.setEnabled(imageSrc);
            }
        }

        this.AddDragAndDropToContainer(imageCell, function (files, content) {
            if (!imageControl.isEnabled) return;

            var fileName = files[0].name.toLowerCase();
            var fileExt = fileName.substring(fileName.lastIndexOf("."));

            if (filesMask.indexOf(fileExt) >= 0) {
                imageControl.setImage(content);
                imageControl.action(files, content);
            }
        });
    }
    else {
        imageControl.setImage = function (imageSrc) {
            imageContainer.style.display = imageSrc ? "" : "none";
            if (imageSrc && imageContainer.isValidSrc(imageSrc))
                imageContainer.src = imageSrc
            else {
                imageContainer.removeAttribute("src");
            }
            imageControl.src = imageSrc;
        }
    }

    imageControl.resize = function (newWidth, newHeight) {
        imageControl.style.width = newWidth + "px";
        imageControl.style.height = newHeight + "px";
        imageContainer.style.maxWidth = newWidth + "px";
        imageContainer.style.maxHeight = newHeight + "px";
        imageCell.style.width = newWidth + "px";
        imageCell.style.height = newHeight + "px";
    }

    imageControl.setEnabled = function (state) {
        this.isEnabled = state;
        if (imageControl.openButton) imageControl.openButton.setEnabled(state);
        if (imageControl.removeButton) imageControl.removeButton.setEnabled(state && imageControl.src);
        imageContainer.style.opacity = state ? "1" : "0.4";
    }

    return imageControl;
}