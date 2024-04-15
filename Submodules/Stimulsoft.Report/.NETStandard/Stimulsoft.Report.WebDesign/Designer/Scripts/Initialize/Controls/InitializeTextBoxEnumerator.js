
StiMobileDesigner.prototype.TextBoxEnumerator = function (name, width, toolTip, readOnly, maxValue, minValue, allowDecimal) {
    var enumControl = this.CreateHTMLTable();
    var jsObject = enumControl.jsObject = this;
    if (name != null) this.options.controls[name] = enumControl;
    enumControl.name = name != null ? name : this.generateKey();
    enumControl.readOnly = readOnly;
    enumControl.isEnabled = true;
    enumControl.isSelected = false;
    enumControl.isOver = false;
    enumControl.isFocused = false;
    enumControl.maxValue = maxValue;
    enumControl.minValue = minValue;
    if (toolTip) enumControl.setAttribute("title", toolTip);
    enumControl.className = "stiDesignerTextBoxEnumerator";

    //TextBox
    var textBox = enumControl.textBox = document.createElement("input");
    textBox.textBoxEnumerator = enumControl;
    textBox.jsObject = this;
    textBox.style.width = (this.options.isTouchDevice ? width - 25 : width - 17) + "px";
    textBox.style.height = this.options.isTouchDevice ? "26px" : "21px";
    textBox.className = "stiDesignerTextBoxEnumerator_TextBox stiDesignerTextBoxEnumerator_TextBoxDefault";

    var textboxCell = enumControl.addCell(textBox);
    textboxCell.style.fontSize = "0";
    textboxCell.style.lineHeight = "0";
    textBox.readOnly = readOnly;
    textBox.style.cursor = readOnly ? "default" : "text";

    textBox.onfocus = function () {
        enumControl.isFocused = true;
        enumControl.setSelected(true);
    }

    textBox.onblur = function () {
        enumControl.isFocused = false;
        enumControl.setSelected(false);
        this.setCorrectValue(allowDecimal ? jsObject.StrToDouble(this.value) : jsObject.StrToInt(this.value));
        enumControl.action();
    }

    textBox.onkeypress = function (event) {
        if (enumControl.readOnly) return false;
        if (event && event.keyCode == 13) {
            this.setCorrectValue(allowDecimal ? jsObject.StrToDouble(this.value) : jsObject.StrToInt(this.value));
            enumControl.action();
            return false;
        }
    }

    textBox.setCorrectValue = function (value) {
        if (enumControl.maxValue != null && value > enumControl.maxValue) { this.value = enumControl.maxValue; return; }
        if (enumControl.minValue != null && value < enumControl.minValue) { this.value = enumControl.minValue; return; }
        var countNums = this.getCountNumbersAfterPoint();
        this.value = allowDecimal && countNums > 0 ? value.toFixed(countNums) : value;
    }

    textBox.setEnabled = function (state) {
        this.isEnabled = state;
        this.disabled = !state;
        this.className = "stiDesignerTextBoxEnumerator_TextBox stiDesignerTextBoxEnumerator_TextBox" + (state ? "Default" : "Disabled");
    }

    textBox.getCountNumbersAfterPoint = function () {
        var strVal = this.value.replace(",", ".");
        var pointIndex = strVal.indexOf(".");
        return (pointIndex >= 0 ? strVal.length - pointIndex - 1 : 0);
    }

    var buttonsCell = enumControl.addCell();

    //Button Up
    var buttonUp = enumControl.buttonUp = this.TextBoxEnumeratorButton("Arrows.SmallArrowUp.png");
    buttonUp.style.marginBottom = "1px";
    buttonsCell.appendChild(buttonUp);

    buttonUp.action = function () {
        var value = allowDecimal ? jsObject.StrToDouble(textBox.value, true) : jsObject.StrToInt(textBox.value);
        value++;
        textBox.setCorrectValue(value);
        enumControl.action();
    }

    //Button Down
    var buttonDown = enumControl.buttonDown = this.TextBoxEnumeratorButton("Arrows.SmallArrowDown.png");
    buttonsCell.appendChild(buttonDown);

    buttonDown.action = function () {
        var value = allowDecimal ? jsObject.StrToDouble(textBox.value, true) : jsObject.StrToInt(textBox.value);
        value--;
        textBox.setCorrectValue(value);
        enumControl.action();
    }

    //Methods
    enumControl.setEnabled = function (state) {
        this.isEnabled = state;
        textBox.setEnabled(state);
        buttonUp.setEnabled(state);
        buttonDown.setEnabled(state);
        this.className = state ? "stiDesignerTextBoxEnumerator" : "stiDesignerTextBoxEnumeratorDisabled";
    }

    enumControl.onmouseover = function () {
        if (jsObject.options.isTouchDevice || !this.isEnabled) return;
        this.isOver = true;
        if (!this.isSelected && !this.isFocused) this.className = "stiDesignerTextBoxEnumeratorOver";
    }

    enumControl.onmouseout = function () {
        if (jsObject.options.isTouchDevice || !this.isEnabled) return;
        this.isOver = false;
        if (!this.isSelected && !this.isFocused) this.className = "stiDesignerTextBoxEnumerator";
    }

    enumControl.setSelected = function (state) {
        this.isSelected = state;
        this.className = state ? "stiDesignerTextBoxEnumeratorOver" : (this.isEnabled ? (this.isOver ? "stiDesignerTextBoxEnumeratorOver" : "stiDesignerTextBoxEnumerator") : "stiDesignerTextBoxEnumeratorDisabled");
    }

    enumControl.setValue = function (value) {
        textBox.setCorrectValue(typeof (value) == "string" ? (allowDecimal ? jsObject.StrToDouble(value) : jsObject.StrToInt(value)) : value);
    }

    enumControl.getValue = function () {
        return textBox.value;
    }

    enumControl.action = function () { };

    return enumControl;
}

StiMobileDesigner.prototype.TextBoxEnumeratorButton = function (imageName) {
    var button = this.SmallButton(null, null, null, imageName, null, null, "stiDesignerTextBoxEnumeratorButton", null, { width: 8, height: 8 });
    button.style.width = this.options.isTouchDevice ? "25px" : "17px";
    button.imageCell.style.padding = "0px";
    button.innerTable.style.width = "100%";

    return button;
}