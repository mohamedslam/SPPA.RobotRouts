
StiMobileDesigner.prototype.MarginsControl = function (name, width, height, images) {
    var control = this.CreateHTMLTable();
    control.className = "stiDesignerMarginsControl stiDesignerMarginsControlDefault";
    control.isEnabled = true;
    control.isOver = false;
    var arrows = ["Left", "Top", "Right", "Bottom"];
    if (!width) width = 150;
    if (name) {
        this.options.controls[name] = control;
        control.name = name;
    }

    for (var i = 0; i < arrows.length; i++) {
        var textBox = this.TextBox(null, width / 4 - 30, height);
        textBox.style.border = "0px";
        control["value" + arrows[i]] = textBox;

        var cell = control.addCell(textBox);

        if (this.allowRoundedControls()) {
            if (i == 0) textBox.style.borderRadius = "3px 0 0 3px";
            if (i == 3) cell.style.borderRadius = "0 3px 3px 0";
        }

        var img = document.createElement("img");
        img.style.margin = "0 4px 1px 0";
        img.style.width = "7px";
        img.style.height = "7px";
        StiMobileDesigner.setImageSource(img, this.options, images ? images[i] : ("SizeArrows.Arrow" + arrows[i] + ".png"));

        var cell2 = control["cell" + arrows[i]] = control.addCell(img);

        if (i < 3) {
            cell2.className = "stiDesignerMarginsControlInnerCell";
        }
        else {
            cell2.style.borderRadius = this.allowRoundedControls() ? "0 3px 3px 0" : "0";
        }

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

    control.setValue = function (value, isPageMargins) {
        var marginsArray = value.split(isPageMargins ? "!" : ";");
        control.valueLeft.value = !value ? "" : (marginsArray.length != 4 ? 0 : marginsArray[0]);
        control.valueTop.value = !value ? "" : (marginsArray.length != 4 ? 0 : marginsArray[1]);
        control.valueRight.value = !value ? "" : (marginsArray.length != 4 ? 0 : marginsArray[2]);
        control.valueBottom.value = !value ? "" : (marginsArray.length != 4 ? 0 : marginsArray[3]);
    }

    control.getValue = function (isPageMargins) {
        var sep = isPageMargins ? "!" : ";";

        return control.valueLeft.value + sep + control.valueTop.value + sep + control.valueRight.value + sep + control.valueBottom.value;
    }

    control.setEnabled = function (state) {
        this.isEnabled = state;
        for (var i = 0; i < arrows.length; i++) {
            control["value" + arrows[i]].disabled = !state;
        }
    }

    control.action = function () { }

    return control;
}

//TextBox
StiMobileDesigner.prototype.PropertyMarginsControl = function (name, width, images) {
    return this.MarginsControl(name, width, this.options.propertyControlsHeight, images);
}

StiMobileDesigner.prototype.PropertyCornerRadiusControl = function (name, width) {
    return this.PropertyMarginsControl(name, width, ["Corners.TopLeft.png", "Corners.TopRight.png", "Corners.BottomRight.png", "Corners.BottomLeft.png"]);
}