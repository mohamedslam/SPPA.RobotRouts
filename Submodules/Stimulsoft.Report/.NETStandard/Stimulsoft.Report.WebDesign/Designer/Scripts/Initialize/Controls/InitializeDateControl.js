
StiMobileDesigner.prototype.DateControl = function (name, width, toolTip, shortFormat) {
    var dateControl = this.CreateHTMLTable();
    var jsObject = dateControl.jsObject = this;
    if (name != null) this.options.controls[name] = dateControl;
    dateControl.name = name != null ? name : this.generateKey();
    dateControl.key = new Date();
    dateControl.shortFormat = shortFormat;
    dateControl.isEnabled = true;
    dateControl.isSelected = false;
    dateControl.isOver = false;
    dateControl.style.margin = "0px";
    dateControl.style.padding = "0px";
    dateControl.className = "stiDesignerDateControl";

    if (toolTip) dateControl.setAttribute("title", toolTip);
    var buttonWidth = this.options.controlsButtonsWidth + 2;
    var textBoxWidth = width - buttonWidth;

    //TextBox
    var textBox = document.createElement("input");
    dateControl.textBox = textBox;
    textBox.jsObject = this;
    textBox.style.width = textBoxWidth + "px";
    textBox.dateControl = dateControl;
    textBox.readOnly = true;
    textBox.style.cursor = "default";
    textBox.value = !shortFormat ? dateControl.key.toLocaleString() : dateControl.key.toLocaleDateString();
    textBox.style.height = this.options.isTouchDevice ? "26px" : "21px";
    textBox.className = "stiDesignerDateControl_TextBox";
    dateControl.addCell(textBox);

    textBox.onclick = function () {
        if (!this.isTouchEndFlag && this.readOnly) {
            this.dateControl.button.onclick();
        }
    }

    textBox.ontouchend = function () {
        if (!this.readOnly) return;
        var this_ = this;
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);
        this.dateControl.button.ontouchend();
        this.isTouchEndTimer = setTimeout(function () {
            this_.isTouchEndFlag = false;
        }, 1000);
    }

    textBox.onfocus = function () {
        jsObject.options.controlsIsFocused = this;
    }

    textBox.onblur = function () {
        jsObject.options.controlsIsFocused = false;

        if (!this.readOnly) {
            dateControl.action();
        }
    }

    //DropDownButton
    var button = dateControl.button = this.SmallButton((name != null) ? name + "DropDownButton" : null, null, null, "Arrows.SmallArrowDown.png", null, null, "stiDesignerDateControlDropDownListButton", null, { width: 8, height: 8 });
    button.style.width = buttonWidth + "px";
    button.style.borderRadius = this.allowRoundedControls() ? "0 3px 3px 0" : "0";
    button.imageCell.style.padding = "0px";
    button.innerTable.style.width = "100%";
    button.dateControl = dateControl;
    dateControl.addCell(button);

    button.action = function () {
        var datePicker = jsObject.options.menus.datePicker || jsObject.InitializeDatePicker();
        datePicker.parentDateControl = this.dateControl;
        datePicker.parentButton = button;
        datePicker.changeVisibleState(!datePicker.visible);
    }

    dateControl.onmouseover = function () {
        if (!jsObject.options.isTouchDevice) this.onmouseenter();
    }

    dateControl.onmouseenter = function () {
        if (!this.isEnabled || jsObject.options.isTouchClick) return;
        this.isOver = true;
        if (!this.isSelected) {
            this.className = "stiDesignerDateControlOver";
        }
    }

    dateControl.onmouseleave = function () {
        if (!this.isEnabled) return;
        this.isOver = false;
        if (!this.isSelected) {
            this.className = "stiDesignerDateControl";
        }
    }

    dateControl.setEnabled = function (state) {
        button.setEnabled(state);
        this.isEnabled = state;
        this.textBox.disabled = !state;
        this.textBox.style.visibility = state ? "visible" : "hidden";
        this.className = state ? "stiDesignerDateControl" : "stiDesignerDateControlDisabled";
    }

    dateControl.setSelected = function (state) {
        this.isSelected = state;
        this.className = state ? "stiDesignerDateControlOver" : (this.isEnabled ? (this.isOver ? "stiDesignerDateControlOver" : "stiDesignerDateControl") : "stiDesignerDateControlDisabled");
    }

    dateControl.setKey = function (key) {
        this.key = key;
        this.textBox.value = this.dateTimeFormat == "Time"
            ? key.toLocaleTimeString()
            : this.dateTimeFormat == "Date"
                ? key.toLocaleDateString()
                : (this.shortFormat ? key.toLocaleDateString() : key.toLocaleString());
    }

    dateControl.action = function () { }

    return dateControl;
}