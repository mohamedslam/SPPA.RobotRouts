
StiJsViewer.prototype.SmallButton = function (name, captionText, imageName, toolTip, arrow, styleName, styleColors, imageSizes) {
    var button = document.createElement("div");
    button.style.fontFamily = this.options.toolbar.fontFamily;
    var jsObject = button.jsObject = this;
    button.name = name;
    button.styleName = styleName || "stiJsViewerStandartSmallButton";
    button.isEnabled = true;
    button.isSelected = false;
    button.isOver = false;
    button.className = button.styleName + " " + button.styleName + "Default";
    button.toolTip = toolTip;
    button.style.height = this.options.isTouchDevice ? "28px" : "23px";
    button.style.boxSizing = "content-box";
    button.imageName = imageName;
    button.styleColors = styleColors;

    if (styleColors) {
        button.style.background = styleColors.backColor;
        button.style.color = styleColors.foreColor;
    }

    if (name) {
        if (!this.controls.buttons) this.controls.buttons = {};
        this.controls.buttons[name] = button;
    }

    var innerTable = button.innerTable = this.CreateHTMLTable();
    innerTable.style.height = innerTable.style.width = "100%";
    button.appendChild(innerTable);

    if (imageName != null) {
        var image = button.image = document.createElement("img");
        image.style.width = (imageSizes ? imageSizes.width : 16) + "px";
        image.style.height = (imageSizes ? imageSizes.height : 16) + "px";
        if (StiJsViewer.checkImageSource(this.options, this.collections, imageName)) StiJsViewer.setImageSource(image, this.options, this.collections, imageName);

        var imageCell = button.imageCell = innerTable.addCell(image);
        imageCell.style.lineHeight = "0";
        imageCell.style.textAlign = "center";
        imageCell.style.padding = (this.options.isTouchDevice && captionText == null) ? "0 7px 0 7px" : "0 4px 0 4px";
    }

    if (captionText != null) {
        var caption = button.caption = innerTable.addCell();
        caption.style.padding = (arrow ? "0 0 " : "0 10px ") + (imageName ? "0 0" : "0 10px");
        caption.style.whiteSpace = "nowrap";
        caption.style.textAlign = "left";
        caption.innerText = captionText;
    }

    if (arrow != null) {
        button.arrow = document.createElement("img");
        button.arrow.style.width = button.arrow.style.height = "8px";
        button.arrow.style.verticalAlign = "baseline";
        StiJsViewer.setImageSource(button.arrow, this.options, this.collections, "Arrows.SmallArrow" + arrow + (arrow == "Down" && styleColors && styleColors.isDarkStyle ? "White.png" : ".png"));

        var arrowCell = button.arrowCell = innerTable.addCell(button.arrow);
        arrowCell.style.lineHeight = "0";
        arrowCell.style.padding = captionText ? (this.options.isTouchDevice ? "0 7px 0 7px" : "0 7px 0 4px") : (this.options.isTouchDevice ? "0 4px 0 0" : "0 4px 0 1px");
    }

    if (toolTip && typeof (toolTip) != "object") {
        button.setAttribute("title", toolTip);
    }

    button.onmouseoverAction = function () {
        if (!this.isEnabled || jsObject.options.isTouchClick || (this["haveMenu"] && this.isSelected)) return;
        this.isOver = true;
        if (!jsObject.options.isTouchDevice && jsObject.options.appearance.showTooltips && this.toolTip && typeof (this.toolTip) == "object") {
            jsObject.controls.toolTip.showWithDelay(
                this.toolTip[0],
                this.toolTip[1],
                this.toolTip.length == 3 && this.toolTip[2].left
                    ? this.toolTip[2].left
                    : jsObject.FindPosX(this, "stiJsViewerMainPanel"),
                this.toolTip.length == 3 && this.toolTip[2].top
                    ? (this.toolTip[2].top == "auto" ? this.offsetHeight + jsObject.FindPosY(this, "stiJsViewerMainPanel") : this.toolTip[2].top)
                    : jsObject.controls.toolbar.offsetHeight + jsObject.controls.dashboardsPanel.offsetHeight,
                this.toolTip.length == 3 && this.toolTip[2].rightToLeft ? this.offsetWidth : null
            );
        }
        if (this.styleColors) {
            this.style.background = this.isSelected ? this.styleColors.hotSelectedBackColor : this.styleColors.hotBackColor;
            this.style.color = this.isSelected ? this.styleColors.hotSelectedForeColor : this.styleColors.hotForeColor;
        }
        else {
            this.className = this.styleName + " " + this.styleName + "Over";
        }
    }

    button.onmouseoutAction = function () {
        this.isOver = false;
        if (!this.isEnabled) return;
        if (jsObject.options.appearance.showTooltips && this.toolTip && typeof (this.toolTip) == "object") jsObject.controls.toolTip.hideWithDelay();
        if (this.styleColors) {
            this.style.background = this.isSelected ? this.styleColors.selectedBackColor : this.styleColors.backColor;
            this.style.color = this.isSelected ? this.styleColors.selectedForeColor : this.styleColors.foreColor;
        }
        else {
            this.className = this.styleName + " " + this.styleName + (this.isSelected ? "Selected" : "Default");
        }
    }

    button.onmouseover = function () {
        if (!jsObject.options.isTouchDevice) this.onmouseenter();
    }

    button.onmouseout = function () {
        if (!jsObject.options.isTouchDevice) this.onmouseleave();
    }

    button.onmouseenter = function () {
        this.onmouseoverAction();
    }

    button.onmouseleave = function () {
        this.onmouseoutAction();
    }

    button.onmousedown = function () {
        if (this.isTouchStartFlag || !this.isEnabled) return;
        jsObject.options.buttonPressed = this;
    }

    button.onclick = function () {
        if (this.isTouchEndFlag || !this.isEnabled || jsObject.options.isTouchClick) return;
        if (jsObject.options.appearance.showTooltips && this.toolTip && typeof (this.toolTip) == "object") jsObject.controls.toolTip.hide();
        this.action();
    }

    button.ontouchend = function () {
        if (!this.isEnabled || jsObject.options.fingerIsMoved) return;
        var this_ = this;
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);
        var timer = setTimeout(function (buttonId) {
            jsObject.options.buttonsTimer = null;
            if (this_.styleColors) {
                this_.style.background = this_.isSelected ? this_.styleColors.selectedBackColor : this_.styleColors.backColor;
                this_.style.color = this_.isSelected ? this_.styleColors.selectedForeColor : this_.styleColors.foreColor;
            }
            else {
                this_.className = this_.styleName + " " + this_.styleName + "Default";
            }
            this_.action();
        }, 150);
        jsObject.options.buttonsTimer = [this, this.className, timer];
        this.isTouchEndTimer = setTimeout(function () {
            this_.isTouchEndFlag = false;
        }, 1000);
    }

    button.ontouchstart = function () {
        var this_ = this;
        this.isTouchStartFlag = true;
        clearTimeout(this.isTouchStartTimer);
        jsObject.options.fingerIsMoved = false;
        jsObject.options.buttonPressed = this;
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    button.setEnabled = function (state) {
        if (this.image) this.image.style.opacity = state ? "1" : "0.5";
        if (this.arrow) this.arrow.style.opacity = state ? "1" : "0.5";
        this.isEnabled = state;
        if (!state && !this.isOver) this.isOver = false;
        if (this.styleColors) {
            this.style.background = state ? (this.isOver ? this.styleColors.hotBackColor : (this.isSelected ? this.styleColors.selectedBackColor : this.styleColors.backColor)) : this.styleColors.backColor;
            this.style.color = state ? (this.isOver ? this.styleColors.hotForeColor : (this.isSelected ? this.styleColors.selectedForeColor : this.styleColors.foreColor)) : this.styleColors.foreColor;
        }
        else {
            this.className = this.styleName + " " + (state ? (this.styleName + (this.isOver ? "Over" : (this.isSelected ? "Selected" : "Default"))) : this.styleName + "Disabled");
        }
    }

    button.setSelected = function (state) {
        this.isSelected = state;
        if (this.styleColors) {
            this.style.background = (state
                ? (this.isOver ? this.styleColors.hotSelectedBackColor : this.styleColors.selectedBackColor)
                : (this.isOver ? this.styleColors.hotBackColor : this.styleColors.backColor));
            this.style.color = (state
                ? (this.isOver ? this.styleColors.hotSelectedForeColor : this.styleColors.selectedForeColor)
                : (this.isOver ? this.styleColors.hotForeColor : this.styleColors.foreColor));
        }
        else {
            this.className = this.styleName + " " + this.styleName +
                (state ? "Selected" : (this.isEnabled ? (this.isOver ? "Over" : "Default") : "Disabled"));
        }
    }

    button.applyStyleColors = function (styleColors) {
        this.styleColors = styleColors;

        if (styleColors) { 
            button.style.background = styleColors.backColor;
            button.style.color = styleColors.foreColor;
            if (button.showBorders) {
                var borderRgb = jsObject.HexToRgb(styleColors.foreColor);
                var borderColor = borderRgb ? "rgba(" + borderRgb.r + "," + borderRgb.g + "," + borderRgb.b + ",0.5)" : styleColors.foreColor;
                button.style.border = "1px solid " + borderColor;
            }
        }
        else {
            button.style.background = "";
            button.style.color = "";
        }

        if (button.arrow)
            StiJsViewer.setImageSource(button.arrow, jsObject.options, jsObject.collections, "Arrows.SmallArrow" + arrow + (arrow == "Down" && styleColors && styleColors.isDarkStyle ? "White.png" : ".png"));

        button.onmouseoutAction();
    }

    button.action = function () { jsObject.postAction(this.name); }

    return button;
}