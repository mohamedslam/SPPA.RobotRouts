
StiMobileDesigner.prototype.TextBoxWithOpenDialog = function (name, width, filesMask) {
    var textBoxParent = document.createElement("div");
    var jsObject = textBoxParent.jsObject = this;
    textBoxParent.name = name;
    textBoxParent.style.position = "relative";
    if (name) this.options.controls[name] = textBoxParent;

    var openButton = this.StandartSmallButton(null, null, null, "Open.png");
    textBoxParent.openButton = openButton;
    textBoxParent.appendChild(openButton);

    var buttonSize = this.options.controlsHeight - 4;
    openButton.style.width = openButton.style.height = buttonSize + "px";
    openButton.style.margin = "1px 0 0 " + (width - buttonSize + 5) + "px";
    openButton.style.position = "absolute";
    openButton.style.right = "1px";
    openButton.imageCell.style.padding = "0px 0 0 1px";
    openButton.innerTable.style.width = "100%";

    if (jsObject.allowRoundedControls()) {
        openButton.style.borderRadius = "0 3px 3px 0";
    }

    var textBox = this.TextBox(null, width);
    textBoxParent.appendChild(textBox);
    textBoxParent.textBox = textBox;
    textBoxParent.filesMask = filesMask;

    openButton.action = function () {
        if (jsObject.options.canOpenFiles) {
            var openDialog = jsObject.InitializeOpenDialog(name + "OpenDialog", function (evt) {
                if (evt.target.files && evt.target.files[0] && evt.target.files[0].path) {
                    textBox.value = evt.target.files[0].path;
                }
                else {
                    textBox.value = evt.target.value;
                }
                textBox.action();
            }, textBoxParent.filesMask);
            openDialog.action();
        }
    }

    textBoxParent.setValue = function (value) {
        textBox.value = value;
    }

    textBoxParent.getValue = function () {
        return textBox.value;
    }

    textBoxParent.action = function () { }
    textBox.action = function () { textBoxParent.action(); }

    return textBoxParent;
}