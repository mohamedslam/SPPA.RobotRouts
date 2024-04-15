
StiMobileDesigner.prototype.TextBox = function (name, width, height, toolTip) {
    var textBox = document.createElement("input");
    var jsObject = textBox.jsObject = this;
    textBox.isEnabled = true;
    textBox.isSelected = false;
    textBox.isOver = false;
    textBox.toolTip = toolTip;
    textBox.className = "stiDesignerTextBox stiDesignerTextBoxDefault";
    textBox.style.height = ((height || this.options.controlsHeight) - 2) + "px";

    if (toolTip && typeof (toolTip) != "object") {
        textBox.setAttribute("title", toolTip);
    }

    if (width != null) {
        textBox.style.width = typeof (width) == "string" ? width : (width + "px");
    }

    if (name) {
        this.options.controls[name] = textBox;
        textBox.name = name;
    }

    textBox.setEnabled = function (state) {
        this.isEnabled = state;
        this.disabled = !state;
        this.className = "stiDesignerTextBox" + (state ? " stiDesignerTextBoxDefault" : " stiDesignerTextBoxDisabled");
    }

    textBox.onmouseover = function () {
        if (!jsObject.options.isTouchDevice) this.onmouseenter();
    }

    textBox.onmouseenter = function () {
        if (!this.isEnabled || jsObject.options.isTouchClick) return;
        this.isOver = true;
        if (!this.isSelected) this.className = "stiDesignerTextBox stiDesignerTextBoxOver";

        if (this.toolTip && typeof (this.toolTip) == "object") {
            jsObject.options.toolTip.showWithDelay(
                this.toolTip[0],
                this.toolTip[1],
                this.toolTip.length == 3 ? this.toolTip[2].left : jsObject.FindPosX(this, "stiDesignerMainPanel"),
                this.toolTip.length == 3 ? this.toolTip[2].top : jsObject.options.toolBar.offsetHeight + jsObject.options.workPanel.offsetHeight - 1
            );
        }
    }

    textBox.onmouseleave = function () {
        if (!this.isEnabled) return;
        this.isOver = false;
        if (!this.isSelected) this.className = "stiDesignerTextBox stiDesignerTextBoxDefault";
        if (this.toolTip && typeof (this.toolTip) == "object") jsObject.options.toolTip.hideWithDelay();
    }

    textBox.setSelected = function (state) {
        this.isSelected = state;
        this.className = "stiDesignerTextBox" + (state ? " stiDesignerTextBoxOver" : (this.isEnabled ? (this.isOver ? " stiDesignerTextBoxOver" : " stiDesignerTextBoxDefault") : "stiDesignerTextBoxDisabled"));
    }

    var onChangeTimer;

    textBox.onfocus = function () {
        jsObject.options.controlsIsFocused = this;
        this.hideError();
        this.oldValue = this.value;

        if (textBox.onchange != null) {
            var lastValue = textBox.value;
            onChangeTimer = setInterval(function () {
                if (textBox.value != lastValue) {
                    textBox.onchange();
                    lastValue = textBox.value;
                }
            }, 1000);
        }
    }

    textBox.onblur = function () {
        this.isOver = false;
        this.setSelected(false);
        jsObject.options.controlsIsFocused = false;
        if (!this.readOnly && (this.oldValue != this.value || this.keyEnterPressed)) this.action();
        this.keyEnterPressed = false;
        this.hideError();
        clearInterval(onChangeTimer);
    }

    textBox.onkeypress = function (event) {
        if (this.readOnly) return false;
        if (event && event.keyCode == 13) {
            this.keyEnterPressed = true;
            this.blur();
            this.actionOnKeyEnter();
            jsObject.ReturnFocusToDesigner();
            return false;
        }
    }

    textBox.setReadOnly = function (state) {
        this.style.cursor = state ? "default" : "";
        this.readOnly = state;
        try {
            this.setAttribute("unselectable", state ? "on" : "off");
            this.setAttribute("onselectstart", state ? "return false" : "");
        }
        catch (e) { };
    }

    textBox.hideError = function () {
        if (this.parentElement && this.errorImage) {
            this.parentElement.removeChild(this.errorImage);
            this.errorImage = null;
        }
    }

    textBox.showError = function (text) {
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

    textBox.checkNotEmpty = function (fieldName) {
        if (this.value == "") {
            var text = fieldName ? jsObject.loc.Errors.FieldRequire.replace("{0}", fieldName) : jsObject.loc.Errors.FieldRequire.replace("'{0}'", "");
            this.showError(text);
            return false;
        }
        return true;
    }

    textBox.checkExists = function (collection, propertyName) {
        if (collection && propertyName) {
            for (var i = 0; i < collection.length; i++) {
                var propertValue = collection[i][propertyName];
                if (typeof (propertValue) == "string" && propertValue.toLowerCase() == this.value.toLowerCase()) {
                    this.showError(jsObject.loc.Errors.NameExists.replace("{0}", this.value));
                    return false;
                }
            }
        }
        return true;
    }

    textBox.action = function () { };

    textBox.actionOnKeyEnter = function () { };

    return textBox;
}

StiMobileDesigner.prototype.TextBoxDoubleValue = function (name, width) {
    var textBox = this.TextBox(name, width);

    textBox.action = function () {
        this.value = this.jsObject.StrToDouble(this.value);
    }

    return textBox;
}

StiMobileDesigner.prototype.TextBoxIntValue = function (name, width) {
    var textBox = this.TextBox(name, width);

    textBox.action = function () {
        this.value = this.jsObject.StrToInt(this.value);
    }

    return textBox;
}

StiMobileDesigner.prototype.TextBoxPositiveIntValue = function (name, width) {
    var textBox = this.TextBox(name, width);

    textBox.action = function () {
        this.value = this.jsObject.StrToCorrectPositiveInt(this.value);
    }

    return textBox;
}

StiMobileDesigner.prototype.TextBoxPositiveDoubleValue = function (name, width) {
    var textBox = this.TextBox(name, width);

    textBox.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
    }

    return textBox;
}