
StiJsViewer.prototype.TextBox = function (name, width, toolTip, actionLostFocus) {
    var textBox = document.createElement("input");
    textBox.style.fontFamily = this.options.toolbar.fontFamily;
    if (this.options.toolbar.fontColor != "") textBox.style.color = this.options.toolbar.fontColor;
    if (width) textBox.style.width = width + "px";
    textBox.jsObject = this;
    textBox.name = name;
    textBox.isEnabled = true;
    textBox.isSelected = false;
    textBox.isFocused = false;
    textBox.isOver = false;
    textBox.actionLostFocus = actionLostFocus;
    if (toolTip) {
        try { textBox.setAttribute("title", toolTip); } catch (e) { }
    }
    textBox.style.height = this.options.isTouchDevice ? "26px" : "21px";
    textBox.style.lineHeight = textBox.style.height;
    textBox.style.boxSizing = "content-box";
    var styleName = "stiJsViewerTextBox";
    textBox.className = styleName + " " + styleName + "Default";
    if (name) {
        if (!this.controls.textBoxes) this.controls.textBoxes = {};
        this.controls.textBoxes[name] = textBox;
    }

    textBox.setEnabled = function (state) {
        this.isEnabled = state;
        this.disabled = !state;
        this.className = styleName + " " + styleName + (state ? "Default" : "Disabled");
    }

    textBox.onmouseover = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseenter();
    }

    textBox.onmouseout = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseleave();
    }

    textBox.onmouseenter = function () {
        if (!this.isEnabled || this.readOnly) return;
        this.isOver = true;
        if (!this.isSelected && !this.isFocused) this.className = styleName + " " + styleName + "Over";
    }

    textBox.onmouseleave = function () {
        if (!this.isEnabled || this.readOnly) return;
        this.isOver = false;
        if (!this.isSelected && !this.isFocused) this.className = styleName + " " + styleName + "Default";
    }

    textBox.setSelected = function (state) {
        this.isSelected = state;
        this.className = styleName + " " + styleName + (state ? "Over" : (this.isEnabled ? (this.isOver ? "Over" : "Default") : "Disabled"));
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

    var onChangeTimer;

    textBox.activateOnChangeTimer = function () {
        if (textBox.onChange != null) {
            var lastValue = textBox.value;
            onChangeTimer = setInterval(function () {
                if (textBox.value != lastValue) {
                    textBox.onChange();
                    lastValue = textBox.value;
                }
            }, 1000);
        }
    }

    textBox.onfocus = function () {
        this.isFocused = true;
        this.setSelected(true);
        this.oldValue = this.value;
        this.activateOnChangeTimer();
    }

    textBox.onblur = function () {
        this.isFocused = false;
        this.setSelected(false);
        this.action();
        clearInterval(onChangeTimer);
    }

    textBox.onkeypress = function (event) {
        if (this.readOnly) return false;

        if (textBox.onChange != null) {
            clearInterval(onChangeTimer);
            textBox.activateOnChangeTimer();
        }

        if (event && event.keyCode == 13) {
            this.actionOnKeyEnter();

            if ("blur" in this && this.actionLostFocus)
                this.blur();
            else
                this.action();

            return false;
        }
    }

    textBox.action = function () { };

    textBox.actionOnKeyEnter = function () { };

    return textBox;
}