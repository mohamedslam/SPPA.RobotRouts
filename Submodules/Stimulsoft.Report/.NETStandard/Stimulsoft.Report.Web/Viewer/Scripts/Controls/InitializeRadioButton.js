

StiJsViewer.prototype.RadioButton = function (name, groupName, caption, tooltip) {
    var radioButton = this.CreateHTMLTable();
    radioButton.style.fontFamily = this.options.toolbar.fontFamily;
    radioButton.jsObject = this;
    radioButton.name = name;
    radioButton.isEnabled = true;
    radioButton.isChecked = false;
    radioButton.groupName = groupName;
    radioButton.className = "stiJsViewerRadioButton";
    radioButton.captionText = caption;
    if (tooltip) radioButton.setAttribute("title", tooltip);
    if (name) {
        if (!this.controls.radioButtons) this.controls.radioButtons = {};
        this.controls.radioButtons[name] = radioButton;
    }

    radioButton.outCircle = document.createElement("div");
    radioButton.outCircle.className = "stiJsViewerRadioButtonOutCircle";
    radioButton.circleCell = radioButton.addCell(radioButton.outCircle);

    radioButton.innerCircle = document.createElement("div");
    radioButton.innerCircle.style.visibility = "hidden";
    radioButton.innerCircle.className = "stiJsViewerRadioButtonInnerCircle";

    radioButton.innerCircle.style.margin = this.options.isTouchDevice ? "4px" : "3px";
    radioButton.innerCircle.style.width = this.options.isTouchDevice ? "9px" : "7px";
    radioButton.innerCircle.style.height = this.options.isTouchDevice ? "9px" : "7px";
    radioButton.outCircle.appendChild(radioButton.innerCircle);

    //Caption
    if (caption != null) {
        radioButton.captionCell = radioButton.addCell();
        radioButton.captionCell.style.paddingLeft = "4px";
        radioButton.captionCell.style.whiteSpace = "nowrap";
        radioButton.captionCell.innerText = caption;
    }

    radioButton.lastCell = radioButton.addCell();

    radioButton.onmouseover = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseenter();
    }

    radioButton.onmouseout = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseleave();
    }

    radioButton.onmouseenter = function () {
        if (!this.isEnabled) return;
        this.outCircle.className = "stiJsViewerRadioButtonOutCircleOver";
    }

    radioButton.onmouseleave = function () {
        if (!this.isEnabled) return;
        this.outCircle.className = "stiJsViewerRadioButtonOutCircle";
    }

    radioButton.onclick = function () {
        if (this.isTouchEndFlag || !this.isEnabled || this.jsObject.options.isTouchClick) return;
        radioButton.setChecked(true);
        radioButton.action();
    }

    radioButton.ontouchend = function () {
        if (!this.isEnabled || this.jsObject.options.fingerIsMoved) return;
        this.outCircle.className = "stiJsViewerRadioButtonOutCircleOver";
        var this_ = this;
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);
        setTimeout(function () {
            this_.outCircle.className = "stiJsViewerRadioButtonOutCircle";
            this_.setChecked(true);
            this_.action();
        }, 150);
        this.isTouchEndTimer = setTimeout(function () {
            this_.isTouchEndFlag = false;
        }, 1000);
    }

    radioButton.ontouchstart = function () {
        this.jsObject.options.fingerIsMoved = false;
    }

    radioButton.setEnabled = function (state) {
        this.innerCircle.style.opacity = state ? "1" : "0.5";
        this.isEnabled = state;
        this.className = state ? "stiJsViewerRadioButton" : "stiJsViewerRadioButtonDisabled";
        this.outCircle.className = state ? "stiJsViewerRadioButtonOutCircle" : "stiJsViewerRadioButtonOutCircleDisabled";
    }

    radioButton.setChecked = function (state) {
        if (this.groupName && state)
            for (var name in this.jsObject.controls.radioButtons) {
                if (this.groupName == this.jsObject.controls.radioButtons[name].groupName)
                    this.jsObject.controls.radioButtons[name].setChecked(false);
            }

        this.innerCircle.style.visibility = (state) ? "visible" : "hidden";
        this.isChecked = state;
        this.onChecked();
    }

    radioButton.onChecked = function () { }
    radioButton.action = function () { }

    return radioButton;
}