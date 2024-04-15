
StiMobileDesigner.prototype.PointControl = function (name, width, height) {
    var jsObject = this;
    var control = this.CreateHTMLTable();
    control.className = "stiDesignerMarginsControl stiDesignerMarginsControlDefault";
    control.isEnabled = true;
    control.isOver = false;

    var coords = ["X", "Y"];
    if (!width) width = 150;

    if (name) {
        this.options.controls[name] = control;
        control.name = name;
    }

    for (var i = 0; i < coords.length; i++) {
        var textCell = control.addTextCell(coords[i]);
        textCell.style.width = "20px";
        textCell.style.textAlign = "center";

        var textBox = this.TextBox(null, (width - 40) / 2 - 6, height);
        textBox.style.border = "0px";
        textBox.style.textAlign = "right";
        textBox.style.paddingRight = "6px";
        control["value" + coords[i]] = textBox;
        var controlCell = control.addCell(textBox);

        if (i == 0) {
            control.middleCell = controlCell;
            controlCell.className = "stiDesignerMarginsControlInnerCell";
        }

        textBox.action = function () {
            this.value = jsObject.StrToDouble(this.value);
            control.action();
        }
    }

    control.onmouseover = function () {
        if (!jsObject.options.isTouchDevice) this.onmouseenter();
    }

    control.onmouseenter = function () {
        if (!this.isEnabled || jsObject.options.isTouchClick) return;
        this.isOver = true;
        this.className = "stiDesignerMarginsControl stiDesignerMarginsControlOver";
        control.middleCell.className = "stiDesignerMarginsControlInnerCellOver";
    }

    control.onmouseleave = function () {
        if (!this.isEnabled) return;
        this.isOver = false;
        this.className = "stiDesignerMarginsControl stiDesignerMarginsControlDefault";
        control.middleCell.className = "stiDesignerMarginsControlInnerCell";
    }

    control.setValue = function (value) {
        if (value == "StiEmptyValue" || !value) value = ";";
        var values = value.split(";");
        control.valueX.value = values.length >= 2 ? values[0] : 0;
        control.valueY.value = values.length >= 2 ? values[1] : 0;
    }

    control.getValue = function () {
        return control.valueX.value + ";" + control.valueY.value;
    }

    control.setEnabled = function (state) {
        this.isEnabled = state;
        for (var i = 0; i < coords.length; i++) {
            control["value" + coords[i]].disabled = !state;
            control["value" + coords[i]].className = "stiDesignerMarginsControl " + (state ? "stiDesignerMarginsControlDefault" : "stiDesignerMarginsControlDisabled");
        }
    }

    control.action = function () { }

    return control;
}

StiMobileDesigner.prototype.PropertyPointControl = function (name, width) {
    return this.PointControl(name, width, this.options.propertyControlsHeight);
}