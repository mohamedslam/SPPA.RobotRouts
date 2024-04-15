
StiMobileDesigner.prototype.SladerControl = function (name, width, minValue, maxValue, showValue) {
    var sladerControl = document.createElement("div");
    var jsObject = sladerControl.jsObject = this;
    var buttonWidth = 7;
    var controlWidth = width || 200;
    var range = maxValue - minValue;
    var maxLeftPos = controlWidth - buttonWidth;
    if (name) this.options.controls[name] = sladerControl;

    sladerControl.className = "stiDesignerSliderControl";
    sladerControl.style.width = controlWidth + "px";
    sladerControl.value = minValue;
    sladerControl.isEnabled = true;

    var sladerLine = document.createElement("div");
    sladerLine.style.width = controlWidth + "px";
    sladerLine.className = "stiDesignerSliderControlLine";
    sladerControl.appendChild(sladerLine);

    var sladerButton = document.createElement("div");
    sladerButton.style.width = buttonWidth + "px";
    sladerButton.isPressed = false;
    sladerButton.className = "stiDesignerSliderControlButton stiDesignerSliderControlButtonDefault";
    sladerControl.sladerButton = sladerButton;
    sladerControl.appendChild(sladerButton);

    if (showValue) {
        var vTable = this.CreateHTMLTable();
        vTable.className = "stiDesignerSliderValueBox";
        vTable.style.left = controlWidth + "px";
        sladerControl.appendChild(vTable);
        sladerControl.valueBox = vTable.addTextCell();
        sladerControl.style.width = (controlWidth + 40) + "px";
    }

    sladerButton.onmouseover = function () {
        if (!jsObject.options.isTouchDevice) this.onmouseenter();
    }

    sladerButton.onmouseenter = function () {
        if (jsObject.options.isTouchDevice || this.isPressed || !sladerControl.isEnabled || jsObject.options.isTouchClick) return;
        this.className = "stiDesignerSliderControlButton stiDesignerSliderControlButtonOver";
    }

    sladerButton.onmouseleave = function () {
        if (jsObject.options.isTouchDevice || this.isPressed || !sladerControl.isEnabled) return;
        this.className = "stiDesignerSliderControlButton stiDesignerSliderControlButtonDefault";
    }

    sladerButton.onmousedown = function (event) {
        if (this.isTouchStartFlag || !sladerControl.isEnabled) return;
        this.ontouchstart(event, true);
    }

    sladerButton.ontouchstart = function (event, mouseProcess) {
        var this_ = this;
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        if (!sladerControl.isEnabled) return;
        jsObject.options.movingSliderControl = sladerControl;
        sladerControl.mouseStartPos = !mouseProcess ? event.touches[0].pageX : event.screenX;
        this.startLeftPos = parseInt(this.style.left.replace("px", ""));
        if (event) event.preventDefault();
        this.isPressed = true;
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    sladerButton.setLeftPos = function (newLeftPos) {
        if (newLeftPos < 0) newLeftPos = 0;
        if (newLeftPos > maxLeftPos) newLeftPos = maxLeftPos;
        this.style.left = newLeftPos + "px";
    }

    sladerButton.ontouchend = function (event) {
        if (!sladerControl.isEnabled) return;
        if (event) event.preventDefault();
        jsObject.options.movingSliderControl = false;
        this.isPressed = false;
        this.onmouseleave();
        sladerControl.updateValue();
        sladerControl.action();
    }

    sladerControl.setEnabled = function (state) {
        this.isEnabled = state;
        this.style.opacity = sladerButton.style.opacity = state ? "1" : "0.5";
    }

    sladerControl.setValue = function (value) {
        if (value < minValue) value = minValue;
        if (value > maxValue) value = maxValue;
        this.value = value;
        var factor = (value - minValue) / range;
        sladerButton.style.left = parseInt(factor * (controlWidth - buttonWidth)) + "px";
        if (sladerControl.valueBox) sladerControl.valueBox.innerText = value;
    }

    sladerControl.getValue = function () {
        return this.value;
    }

    sladerControl.updateValue = function () {
        var newLeftPos = parseInt(sladerButton.style.left.replace("px", ""));
        var factor = newLeftPos / maxLeftPos;
        sladerControl.value = parseInt(minValue + range * factor);
        if (sladerControl.valueBox) sladerControl.valueBox.innerText = sladerControl.value;
    }

    sladerControl.ontouchmove = function (event, mouseProcess) {
        if (event) event.preventDefault();
        var currentPos = mouseProcess ? (event.screenX) : event.touches[0].pageX;
        var deltaPos = parseInt(currentPos - sladerControl.mouseStartPos);
        var newLeftPos = sladerButton.startLeftPos + deltaPos;
        sladerButton.setLeftPos(newLeftPos);
        sladerControl.updateValue();
    }

    sladerControl.onmousedown = function (event) {
        if (sladerButton.isPressed || !sladerControl.isEnabled) return;
        var sladerControlPosX = jsObject.FindPosX(this, null, true);
        var mousePosX = event.clientX || event.x;
        var newLeftPos = mousePosX - sladerControlPosX;
        sladerButton.setLeftPos(newLeftPos);
        sladerControl.updateValue();
        sladerControl.action();
    }

    sladerControl.setValue(minValue);

    sladerControl.action = function () { };

    return sladerControl;
}