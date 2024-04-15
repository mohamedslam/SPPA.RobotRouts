
StiMobileDesigner.prototype.TextArea = function (name, width, height) {
    var textArea = document.createElement("textarea");

    if (width) {
        textArea.style.width = textArea.style.minWidth = textArea.style.maxWidth = width + "px";
    }
    if (height) {
        textArea.style.height = textArea.style.minHeight = textArea.style.maxHeight = height + "px";
    }

    var baseClass = "stiDesignerTextArea stiDesignerTextArea";
    var jsObject = textArea.jsObject = this;

    this.options.controls[name] = textArea;
    textArea.name = name;
    textArea.isEnabled = true;
    textArea.isSelected = false;
    textArea.isOver = false;
    textArea.className = baseClass + "Default";

    if (this.options.allowWordWrapTextEditors === false) {
        textArea.style.overflowWrap = "normal";
        textArea.style.whiteSpace = "pre";
        textArea.setAttribute("wrap", "off");
    }

    textArea.setEnabled = function (state) {
        this.isEnabled = state;
        this.disabled = !state;
        this.className = baseClass + (state ? "Default" : "Disabled")
    }

    textArea.onmouseover = function () {
        if (!jsObject.options.isTouchDevice) this.onmouseenter();
    }

    textArea.onmouseenter = function () {
        if (!this.isEnabled || jsObject.options.isTouchClick) return;
        this.isOver = true;
        if (!this.isSelected) this.className = baseClass + "Over";
    }

    textArea.onfocus = function () {
        this.hideError();
        jsObject.options.controlsIsFocused = this;
    }

    textArea.onmouseleave = function () {
        if (!this.isEnabled) return;
        this.isOver = false;
        if (!this.isSelected) this.className = baseClass + "Default";
    }

    textArea.setSelected = function (state) {
        this.isSelected = state;
        this.className = baseClass + (state ? "Over" : (this.isEnabled ? (this.isOver ? "Over" : "Default") : "Disabled"));
    }

    textArea.onblur = function () {
        jsObject.options.controlsIsFocused = false;
        this.hideError();
        this.action();
    }

    textArea.setReadOnly = function (state) {
        this.style.cursor = state ? "default" : "";
        this.readOnly = state;
        try {
            this.setAttribute("unselectable", state ? "on" : "off");
            this.setAttribute("onselectstart", state ? "return false" : "");
        }
        catch (e) { };
    }

    textArea.action = function () { };

    textArea.insertText = function (text) {
        if (this.selectionStart != null) {
            var cursorPosAfter = textArea.selectionStart + text.length;
            textArea.value = textArea.value.substring(0, textArea.selectionStart) + text + textArea.value.substring(textArea.selectionEnd);
            if (this.setSelectionRange) textArea.setSelectionRange(cursorPosAfter, cursorPosAfter);
        }
        else {
            textArea.value += text;
        }
    }

    textArea.addInsertButton = function () {
        var insertButton = jsObject.SmallButton(null, null, null, "GetItem.png", null, null, "stiDesignerFormButton", true);
        insertButton.textArea = textArea;
        insertButton.style.position = "absolute";
        insertButton.style.marginLeft = (width - 40) + "px";
        insertButton.style.marginTop = (height - 40) + "px";
        this.insertButton = insertButton;

        if (this.parentElement) {
            this.parentElement.insertBefore(insertButton, this);
        }

        insertButton.action = function () {
            var dictionaryTree = jsObject.options.dictionaryTree;
            if (dictionaryTree && dictionaryTree.selectedItem) {
                textArea.insertText(dictionaryTree.selectedItem.getResultForEditForm());
            }
        }

        //Events
        this.onmouseup = function () { if (jsObject.options.itemInDrag) insertButton.action(); }
        this.ontouchend = function () { this.onmouseup(); }
    }

    textArea.removeInsertButton = function () {
        if (this.insertButton && this.parentElement) {
            this.parentElement.removeChild(this.insertButton);
            this.onmouseup = null;
            this.ontouchend = null;
            this.insertButton = null;
        }
    }

    textArea.resize = function (newWidth, newHeight) {
        if (newWidth) {
            textArea.style.width = textArea.style.minWidth = textArea.style.maxWidth = newWidth + "px";
        }
        if (newHeight) {
            textArea.style.height = textArea.style.minHeight = textArea.style.maxHeight = newHeight + "px";
        }
    }

    textArea.checkNotEmpty = function (fieldName) {
        if (this.value == "") {
            var text = fieldName ? jsObject.loc.Errors.FieldRequire.replace("{0}", fieldName) : jsObject.loc.Errors.FieldRequire.replace("'{0}'", "");
            this.showError(text);
            return false;
        }
        return true;
    }

    textArea.hideError = function () {
        if (this.parentElement && this.errorImage) {
            this.parentElement.removeChild(this.errorImage);
            this.errorImage = null;
        }
    }

    textArea.showError = function (text) {
        var img = document.createElement("img");
        StiMobileDesigner.setImageSource(img, jsObject.options, "Warning.png");
        img.style.width = "14px";
        img.style.height = "14px";
        img.style.marginLeft = (width + 10) + "px";
        img.style.position = "absolute";
        img.style.marginTop = jsObject.options.isTouchDevice ? "7px" : "5px";
        img.title = text;

        if (this.parentElement) {
            this.hideError();
            this.errorImage = img;
            this.parentElement.insertBefore(img, this);
        }

        var i = 0;
        var intervalTimer = setInterval(function () {
            img.style.display = i % 2 != 0 ? "" : "none";
            i++;
            if (i > 5) clearInterval(intervalTimer);
        }, 400);
    }

    textArea.onkeypress = function () { this.onTextChange(); }
    textArea.onpaste = function () { this.onTextChange(); }
    textArea.oninput = function () { this.onTextChange(); }
    textArea.onTextChange = function () { }

    return textArea;
}