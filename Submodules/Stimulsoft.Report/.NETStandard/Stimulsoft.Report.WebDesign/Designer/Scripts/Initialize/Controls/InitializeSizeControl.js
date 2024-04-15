
StiMobileDesigner.prototype.SizeControl = function (name, width, height) {
    var control = this.CreateHTMLTable();
    control.className = "stiDesignerMarginsControl stiDesignerMarginsControlDefault";
    control.isEnabled = true;
    control.isOver = false;
    var arrows = ["Width", "Height"];
    if (!width) width = 150;
    if (name) {
        this.options.controls[name] = control;
        control.name = name;
    }

    for (var i = 0; i < arrows.length; i++) {
        var textBox = this.TextBox(null, (width - 30) / 2, height);
        textBox.style.border = "0px";
        control["value" + arrows[i]] = textBox;
        control.addCell(textBox);
        var img = document.createElement("img");
        img.style.margin = "0 4px 1px 0";
        img.style.width = "11px";
        img.style.height = "11px";
        StiMobileDesigner.setImageSource(img, this.options, "SizeArrows.Arrow" + arrows[i] + ".png");
        control["cell" + arrows[i]] = control.addCell(img);
        if (i == 0) control["cell" + arrows[i]].className = "stiDesignerMarginsControlInnerCell";
        textBox.action = function () {
            this.value = Math.abs(this.jsObject.StrToDouble(this.value));
            control.action();
        }
    }

    control.onmouseover = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseenter();
    }

    control.onmouseenter = function () {
        if (!this.isEnabled || this.jsObject.options.isTouchClick) return;
        this.isOver = true;
        this.className = "stiDesignerMarginsControl stiDesignerMarginsControlOver";
        for (var i = 0; i < arrows.length - 1; i++) {
            control["cell" + arrows[i]].className = "stiDesignerMarginsControlInnerCellOver";
        }
    }

    control.onmouseleave = function () {
        if (!this.isEnabled) return;
        this.isOver = false;
        this.className = "stiDesignerMarginsControl stiDesignerMarginsControlDefault";
        for (var i = 0; i < arrows.length - 1; i++) {
            control["cell" + arrows[i]].className = "stiDesignerMarginsControlInnerCell";
        }
    }

    control.setValue = function (value) {
        var marginsArray = value.split(";");
        control.valueWidth.value = !value ? "" : (marginsArray.length != 2 ? 0 : marginsArray[0]);
        control.valueHeight.value = !value ? "" : (marginsArray.length != 2 ? 0 : marginsArray[1]);
    }

    control.getValue = function () {
        return control.valueWidth.value + ";" + control.valueHeight.value;
    }

    control.setEnabled = function (state) {
        this.isEnabled = state;
        for (var i = 0; i < arrows.length; i++) {
            control["value" + arrows[i]].disabled = !state;
            control["value" + arrows[i]].className = "stiDesignerMarginsControl " + (state ? "stiDesignerMarginsControlDefault" : "stiDesignerMarginsControlDisabled");
        }
    }

    control.action = function () { }

    return control;
}

StiMobileDesigner.prototype.PropertySizeControl = function (name, width) {
    return this.SizeControl(name, width, this.options.propertyControlsHeight);
}