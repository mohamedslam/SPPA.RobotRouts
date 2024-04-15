
StiJsViewer.prototype.TextArea = function (name, width, height) {
    var textArea = document.createElement("textarea");
    textArea.style.width = width + "px";
    textArea.style.height = height + "px";
    textArea.style.minWidth = width + "px";
    textArea.style.minHeight = height + "px";
    textArea.style.paddingTop = "3px";
    textArea.style.fontFamily = "Arial";
    textArea.jsObject = this;
    textArea.name = name;
    textArea.isEnabled = true;
    textArea.isSelected = false;
    textArea.isOver = false;
    var styleName = "stiJsViewerTextBox";
    textArea.className = styleName + " " + styleName + "Default";
    if (name) {
        if (!this.controls.textBoxes) this.controls.textBoxes = {};
        this.controls.textBoxes[name] = textArea;
    }

    textArea.setEnabled = function (state) {
        this.isEnabled = state;
        this.disabled = !state;
        this.className = styleName + " " + styleName + (state ? "Default" : "Disabled");
    }

    textArea.onmouseover = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseenter();
    }

    textArea.onmouseout = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseleave();
    }

    textArea.onmouseenter = function () {
        if (!this.isEnabled || this.readOnly) return;
        this.isOver = true;
        if (!this.isSelected && !this.isFocused) this.className = styleName + " " + styleName + "Over";
    }

    textArea.onfocus = function () {
        this.jsObject.options.controlsIsFocused = true;
    }

    textArea.onmouseleave = function () {
        if (!this.isEnabled || this.readOnly) return;
        this.isOver = false;
        if (!this.isSelected && !this.isFocused) this.className = styleName + " " + styleName + "Default";
    }

    textArea.setSelected = function (state) {
        this.isSelected = state;
        this.className = styleName + " " + styleName + (state ? "Over" : (this.isEnabled ? (this.isOver ? "Over" : "Default") : "Disabled"));
    }

    textArea.onblur = function () {
        this.jsObject.options.controlsIsFocused = false;
        this.action();
    }

    textArea.action = function () { };

    return textArea;
}