
StiMobileDesigner.prototype.FindControl = function (name, width, height, hideButton) {
    var jsObject = this;
    var control = this.CreateHTMLTable();
    control.isEnabled = true;
    control.className = "stiDesignerTextBoxDefault";
    control.style.borderRadius = this.allowRoundedControls() ? "3px" : "0";

    if (name) {
        this.options.controls[name] = control;
        control.name = name;
    }

    var textBox = control.textBox = this.TextBox(null, width ? (hideButton ? width : (width - (this.options.isTouchDevice ? 30 : 26))) : 100);
    textBox.style.border = "0";
    textBox.setAttribute("placeholder", this.loc.Editor.Search);
    control.addCell(textBox);

    var button = this.StandartSmallButton(null, null, null, "View.png");
    control.addCell(button);

    if (hideButton) {
        button.style.display = "none";
    }

    if (height) {
        textBox.style.height = button.style.height = (height - 2) + "px";
    }

    control.onmouseenter = function () {
        if (!this.isEnabled || jsObject.options.isTouchClick) return;
        this.className = "stiDesignerTextBoxOver";
    }

    control.onmouseleave = function () {
        if (!this.isEnabled) return;
        this.className = "stiDesignerTextBoxDefault";
    }

    control.setEnabled = function (state) {
        this.isEnabled = state;
        textBox.setEnabled(state);
        button.setEnabled(state);
        this.className = state ? " stiDesignerTextBoxDefault" : " stiDesignerTextBoxDisabled";
    }

    textBox.action = function () {
        control.action();
    }

    button.action = function () {
        control.action();
    }

    control.getValue = function () {
        return textBox.value;
    }

    control.setValue = function (value) {
        textBox.value = value;
    }

    control.action = function () { }

    return control;
}