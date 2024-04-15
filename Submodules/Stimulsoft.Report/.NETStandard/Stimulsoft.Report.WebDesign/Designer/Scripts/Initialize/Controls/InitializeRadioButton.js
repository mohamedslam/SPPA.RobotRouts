
StiMobileDesigner.prototype.RadioButton = function (name, groupName, caption) {
    var radioButton = this.CreateHTMLTable();
    var jsObject = radioButton.jsObject = this;
    if (name != null) this.options.radioButtons[name] = radioButton;
    radioButton.name = name != null ? name : this.generateKey();
    radioButton.id = radioButton.name;
    radioButton.isEnabled = true;
    radioButton.isChecked = false;
    radioButton.groupName = groupName;
    radioButton.className = "stiMobileDesignerRadioButton stiMobileDesignerRadioButtonDefault";
    radioButton.captionText = caption;

    var outCircle = radioButton.outCircle = document.createElement("div");
    outCircle.className = "stiMobileDesignerRadioButtonOutCircle";
    radioButton.circleCell = radioButton.addCell(outCircle);

    var innerCircle = radioButton.innerCircle = document.createElement("div");
    innerCircle.style.visibility = "hidden";
    innerCircle.className = "stiMobileDesignerRadioButtonInnerCircle stiMobileDesignerRadioButtonInnerCircle" + (this.options.isTouchDevice ? "_Touch" : "_Mouse");
    outCircle.appendChild(innerCircle);

    //Caption
    if (caption != null || typeof (caption) == "undefined") {
        var captionCell = radioButton.captionCell = radioButton.addCell();
        captionCell.style.paddingLeft = "4px";
        captionCell.style.whiteSpace = "nowrap";
        if (caption) captionCell.innerHTML = caption;
    }

    radioButton.onmouseover = function () {
        if (!jsObject.options.isTouchDevice) this.onmouseenter();
    }

    radioButton.onmouseenter = function () {
        if (!this.isEnabled || jsObject.options.isTouchClick) return;
        outCircle.className = "stiMobileDesignerRadioButtonOutCircleOver";
    }

    radioButton.onmouseleave = function () {
        if (!this.isEnabled) return;
        outCircle.className = "stiMobileDesignerRadioButtonOutCircle";
    }

    radioButton.onclick = function () {
        if (this.isTouchEndFlag || !this.isEnabled || jsObject.options.isTouchClick) return;
        radioButton.setChecked(true);
        radioButton.action();
    }

    radioButton.ontouchend = function () {
        if (!this.isEnabled || jsObject.options.fingerIsMoved) return;
        outCircle.className = "stiMobileDesignerRadioButtonOutCircleOver";
        this.isTouchEndFlag = true;

        clearTimeout(this.isTouchEndTimer);
        setTimeout(function () {
            outCircle.className = "stiMobileDesignerRadioButtonOutCircle";
            radioButton.setChecked(true);
            radioButton.action();
        }, 150);

        this.isTouchEndTimer = setTimeout(function () {
            radioButton.isTouchEndFlag = false;
        }, 1000);
    }

    radioButton.ontouchstart = function () {
        jsObject.options.fingerIsMoved = false;
    }

    radioButton.setEnabled = function (state) {
        this.isEnabled = state;
        this.className = "stiMobileDesignerRadioButton stiMobileDesignerRadioButton" + (state ? "Default" : "Disabled");
        outCircle.className = "stiMobileDesignerRadioButtonOutCircle" + (state ? "" : "Disabled");
        innerCircle.style.opacity = state ? "1" : "0.3";
    }

    radioButton.setChecked = function (state) {
        if (this.groupName && state)
            for (var name in jsObject.options.radioButtons) {
                if (this.groupName == jsObject.options.radioButtons[name].groupName)
                    jsObject.options.radioButtons[name].setChecked(false);
            }

        innerCircle.style.visibility = (state) ? "visible" : "hidden";
        this.isChecked = state;
        this.onChecked();
    }

    radioButton.onChecked = function () { }
    radioButton.action = function () { }

    return radioButton;
}