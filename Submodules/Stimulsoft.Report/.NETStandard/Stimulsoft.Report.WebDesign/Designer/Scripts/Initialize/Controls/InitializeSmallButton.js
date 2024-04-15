
StiMobileDesigner.prototype.SmallButton = function (name, groupName, caption, imageName, toolTip, arrow, style, allwaysEnabled, imageSizes, captionAllowHtml) {
    var button = document.createElement("div");
    var jsObject = button.jsObject = this;

    button.name = name != null ? name : this.generateKey();
    button.id = this.options.mobileDesigner.id + button.name;
    if (name != null) this.options.buttons[name] = button;

    button.groupName = groupName;
    button.isEnabled = true;
    button.isSelected = false;
    button.isOver = false;
    button.image = null;
    button.caption = null;
    button.captionText = caption;
    button.arrow = null;
    button.allwaysEnabled = allwaysEnabled;
    button.toolTip = toolTip;
    button.style.boxSizing = "content-box";

    if (!style) style = "stiDesignerStandartSmallButton";

    var baseStyle = style + " " + style + (this.options.isTouchDevice ? "_Touch" : "_Mouse") + " " + style;
    var defaultClass = button.defaultClass = baseStyle + "Default";
    var overClass = button.overClass = baseStyle + "Over";
    var selectedClass = button.selectedClass = baseStyle + "Selected";
    var disabledClass = button.disabledClass = baseStyle + "Disabled";

    button.style = style;
    button.className = defaultClass;

    var innerTable = button.innerTable = this.CreateHTMLTable();
    innerTable.style.height = "100%";
    button.appendChild(innerTable);

    if (imageName) {
        var img = button.image = document.createElement("img");        
        img.style.pointerEvents = "none";
        img.style.width = (imageSizes ? imageSizes.width : 16) + "px";
        img.style.height = (imageSizes ? imageSizes.height : 16) + "px";
        if (StiMobileDesigner.checkImageSource(this.options, imageName)) StiMobileDesigner.setImageSource(img, this.options, imageName);

        var imgCell = button.imageCell = innerTable.addCell(img);
        imgCell.style.padding = (this.options.isTouchDevice && caption == null) ? "0 7px 0 7px" : "0 4px 0 4px";
        imgCell.style.textAlign = "center";
        imgCell.style.fontSize = "0px";
    }

    if (caption != null || typeof (caption) == "undefined") {
        var capCell = button.caption = innerTable.addCell();
        capCell.style.padding = (arrow ? "0 0 " : "0 10px ") + (imageName ? "0 0" : "0 10px");
        capCell.style.whiteSpace = "nowrap";
        capCell.style.textAlign = "left";

        if (caption) {
            if (captionAllowHtml)
                capCell.innerHTML = caption;
            else
                capCell.innerText = caption;
        }
    }

    if (arrow) {
        var arrowImg = button.arrow = document.createElement("img");
        arrowImg.style.width = arrowImg.style.height = "8px";
        arrowImg.setAttribute("draggable", "false");
        StiMobileDesigner.setImageSource(arrowImg, this.options, "Arrows.SmallArrow" + arrow + ".png");

        var arrowCell = button.arrowCell = innerTable.addCell(arrowImg);
        arrowCell.style.padding = caption ? (this.options.isTouchDevice ? "0 7px 0 7px" : "0 6px 0 4px") : (this.options.isTouchDevice ? "0 4px 0 0" : "0 4px 0 1px");
        arrowCell.style.fontSize = "0px";
    }

    if (toolTip && this.options.showTooltips && typeof (toolTip) != "object") {
        button.setAttribute("title", toolTip);
    }

    button.onmouseover = function () {
        if (!jsObject.options.isTouchDevice) this.onmouseenter();
    }

    button.onmouseenter = function () {
        if (!this.isEnabled || (this["haveMenu"] && this.isSelected) || jsObject.options.isTouchClick) return;
        this.className = overClass;
        this.isOver = true;
        if (jsObject.options.showTooltips && this.toolTip && typeof (this.toolTip) == "object") {
            jsObject.options.toolTip.showWithDelay(
                this.toolTip[0],
                this.toolTip[1],
                this.toolTip.length == 3 ? this.toolTip[2].left : jsObject.FindPosX(this, "stiDesignerMainPanel"),
                this.toolTip.length == 3 ? this.toolTip[2].top : jsObject.options.toolBar.offsetHeight + jsObject.options.workPanel.offsetHeight - 1
            );
        }
    }

    button.onmouseleave = function () {
        this.isOver = false;
        if (!this.isEnabled) return;
        this.className = this.isSelected ? selectedClass : defaultClass;
        if (jsObject.options.showTooltips && this.toolTip && typeof (this.toolTip) == "object") jsObject.options.toolTip.hideWithDelay();
    }

    button.onmousedown = function () {
        if (this.isTouchStartFlag || !this.isEnabled) return;
        jsObject.options.buttonPressed = this;
    }

    button.onclick = function () {
        if (this.isTouchEndFlag || !this.isEnabled || jsObject.options.isTouchClick) return;
        if (jsObject.options.showTooltips && this.toolTip && typeof (this.toolTip) == "object") jsObject.options.toolTip.hide();
        this.action();
    }

    button.ontouchend = function () {
        if (!this.isEnabled || jsObject.options.fingerIsMoved) return;
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);

        jsObject.options.buttonsTimer = [this, this.className, setTimeout(function () {
            jsObject.options.buttonsTimer = null;
            button.className = defaultClass;
            if (jsObject.options.showTooltips && button.toolTip && typeof (button.toolTip) == "object") jsObject.options.toolTip.hide();
            button.action();
        }, 150)];

        this.className = overClass;
        this.isTouchEndTimer = setTimeout(function () {
            button.isTouchEndFlag = false;
        }, 1000);
    }

    button.ontouchstart = function () {
        this.isTouchStartFlag = true;
        clearTimeout(this.isTouchStartTimer);
        jsObject.options.fingerIsMoved = false;
        jsObject.options.buttonPressed = this;
        this.isTouchStartTimer = setTimeout(function () {
            button.isTouchStartFlag = false;
        }, 1000);
    }

    button.setEnabled = function (state) {
        if (this.image) this.image.style.opacity = state ? "1" : "0.3";
        if (this.arrow) this.arrow.style.opacity = state ? "1" : "0.3";
        this.isEnabled = state;
        if (!state && !this.isOver) this.isOver = false;
        this.className = state ? (this.isOver ? overClass : (this.isSelected ? selectedClass : defaultClass)) : disabledClass;
    }

    button.setSelected = function (state) {
        if (this.groupName && state) {
            for (var name in jsObject.options.buttons) {
                if (this.groupName == jsObject.options.buttons[name].groupName)
                    jsObject.options.buttons[name].setSelected(false);
            }
        }
        this.isSelected = state;
        this.className = this.isEnabled ? (state ? selectedClass : (this.isOver ? overClass : defaultClass)) : disabledClass;
    }

    button.action = function () { jsObject.ExecuteAction(this.name); }

    return button;
}

StiMobileDesigner.prototype.SmallImageButtonWithBorder = function (name, groupName, imageName, toolTip) {
    var button = this.SmallButton(name, groupName, null, imageName, toolTip, null, "stiDesignerSmallButtonWithBorder");
    button.style.width = button.style.height = this.options.isTouchDevice ? "28px" : "24px";
    button.innerTable.style.width = "100%";
    if (button.imageCell) button.imageCell.style.padding = "0px";

    return button;
}