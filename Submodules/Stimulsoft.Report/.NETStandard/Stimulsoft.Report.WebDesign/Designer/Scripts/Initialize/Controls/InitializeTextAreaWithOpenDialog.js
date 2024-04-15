
StiMobileDesigner.prototype.TextAreaWithOpenDialog = function (name, width, height, filesMask) {
    var textAreaParent = document.createElement("div");
    var jsObject = textAreaParent.jsObject = this;
    textAreaParent.name = name;
    textAreaParent.style.position = "relative";
    if (name) this.options.controls[name] = textAreaParent;

    var buttonSize = this.options.isTouchDevice ? 28 : 24;
    var openButton = this.FormImageButton(null, "Open.png");
    openButton.style.width = buttonSize + "px";
    openButton.style.height = buttonSize + "px";
    openButton.style.margin = "1px 0 0 " + (width + 10) + "px";
    openButton.style.position = "absolute";
    textAreaParent.openButton = openButton;
    textAreaParent.appendChild(openButton);

    var removeButton = this.FormImageButton(null, "Remove.png");
    removeButton.style.width = buttonSize + "px";
    removeButton.style.height = buttonSize + "px";
    removeButton.style.margin = "1px 0 0 " + (width + 10) + "px";
    removeButton.style.position = "absolute";
    removeButton.style.top = (buttonSize + 5) + "px";
    textAreaParent.removeButton = removeButton;
    textAreaParent.appendChild(removeButton);

    var textArea = this.TextArea(null, width, height);
    textAreaParent.appendChild(textArea);
    textAreaParent.textArea = textArea;
    textAreaParent.filesMask = filesMask;
    textArea.style.marginRight = (buttonSize + 15) + "px";

    openButton.action = function () {
        if (jsObject.options.canOpenFiles) {
            var openDialog = jsObject.InitializeOpenDialog(name + "OpenDialog", function (evt) {
                var files = evt.target.files;

                for (var i = 0; i < files.length; i++) {
                    var f = files[i];
                    var reader = new FileReader();

                    reader.onload = (function (theFile) {
                        return function (e) {
                            jsObject.ResetOpenDialogs();
                            if (e.target.result) {
                                var value = e.target.result;
                                textArea.value = StiBase64.decode(value.substring(value.indexOf("base64,") + 7));
                            }
                            textArea.action();
                            jsObject.ReturnFocusToDesigner();
                        };
                    })(f);
                    reader.readAsDataURL(f);
                }
            }, textAreaParent.filesMask);
            openDialog.action();
        }
    }

    removeButton.action = function () {
        textArea.value = "";
        textArea.action();
    }

    textAreaParent.setValue = function (value) {
        textArea.value = value;
    }

    textAreaParent.getValue = function () {
        return textArea.value;
    }

    textAreaParent.action = function () { }
    textArea.action = function () { textAreaParent.action(); }

    return textAreaParent;
}