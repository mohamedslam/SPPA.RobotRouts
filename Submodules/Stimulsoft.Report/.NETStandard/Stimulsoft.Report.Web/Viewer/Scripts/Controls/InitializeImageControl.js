
StiJsViewer.prototype.ImageControl = function (name, width, height, hideButtons, easyType) {
    var jsObject = this;
    var imageControl = document.createElement("div");
    imageControl.src = null;
    imageControl.jsObject = this;
    imageControl.isEnabled = true;
    imageControl.className = "stiJsViewerSimpleContainerWithBorder";

    if (name) {
        if (!this.controls.imageControls) this.controls.imageControls = {};
        this.controls.imageControls[name] = imageControl;
    }

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
        hintText.className = "stiJsViewerTextContainer stiJsViewerDragAndDropHintText";
        hintText.style.margin = "0";
        hintText.innerHTML = this.collections.loc["TextDropImageHere"];
        imageControl.hintText = hintText;
        imageCell.appendChild(hintText);

        if (!hideButtons) {
            var openButton = this.SmallButton(null, null, "Open.png", this.collections.loc["Open"], null, "stiJsViewerFormButton");
            openButton.style.position = "absolute";
            openButton.style.right = "4px";
            openButton.style.top = "4px";
            imageControl.openButton = openButton;

            var removeButton = this.SmallButton(null, null, "Remove.png", this.collections.loc["ButtonRemove"], null, "stiJsViewerFormButton");
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
                var openDialog = jsObject.InitializeOpenDialog("imageControlImageDialog", function (fileName, filePath, content) {
                    //var files = evt.target.files;

                    //for (var i = 0; i < files.length; i++) {
                    //    var f = files[i];
                    //    var reader = new FileReader();

                    //    reader.onload = (function (theFile) {
                    //        return function (e) {
                    //            imageControl.setImage(e.target.result);
                    //            imageControl.action(files, e.target.result, evt.target.value);
                    //        };
                    //    })(f);

                    //    reader.readAsDataURL(f);
                    //}
                    imageControl.setImage(content);
                    imageControl.action();
                }, filesMask);
                openDialog.action();
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

StiJsViewer.prototype.AddDragAndDropToContainer = function (container, draggableSuccessFunc) {
    var jsObject = this;
    container.draggable = true;

    var stopEvent = function (event) {
        event.stopPropagation();
        event.preventDefault();
    };

    var dropFiles = function (files) {
        var reader = new FileReader();

        reader.onload = function (event) {
            try {
                draggableSuccessFunc(files, event.target.result);
            }
            catch (error) {
                var errorForm = this.controls.forms.errorMessageForm || this.InitializeErrorMessageForm();
                errorForm.show(error.message);
            }
        };

        reader.onerror = function (event) {
            var errorForm = this.controls.forms.errorMessageForm || this.InitializeErrorMessageForm();
            errorForm.show(event.target.error.code);
        };

        if (files && files.length > 0) {
            reader.readAsDataURL(files[0]);
        }
    };

    container.draggable = false;

    jsObject.addEvent(container, "dragover", function (event) {
        stopEvent(event);
    });

    jsObject.addEvent(container, "drop", function (event) {
        stopEvent(event);
        event.preventDefault && event.preventDefault();
        dropFiles(event.dataTransfer.files);
        return false;
    });
}