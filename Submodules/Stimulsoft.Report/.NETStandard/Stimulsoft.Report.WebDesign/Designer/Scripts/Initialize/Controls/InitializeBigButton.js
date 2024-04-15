
StiMobileDesigner.prototype.BigButton = function (name, groupName, caption, imageName, toolTip, arrow, style, allwaysEnabled, maxWidth, imageSizes) {
    var button = document.createElement("div");
    var jsObject = button.jsObject = this;

    button.name = name != null ? name : this.generateKey();
    button.id = this.options.mobileDesigner.id + button.name;
    if (name != null) this.options.buttons[name] = button;

    button.groupName = groupName;
    button.isEnabled = true;
    button.isSelected = false;
    button.isOver = false;
    button.arrow = null;
    button.toolTip = toolTip;
    button.allwaysEnabled = allwaysEnabled;
    button.style.boxSizing = "content-box";

    if (!style) style = "stiDesignerStandartBigButton";

    var baseStyle = style + " " + style + (this.options.isTouchDevice ? "_Touch" : "_Mouse") + " " + style;
    var defaultClass = button.defaultClass = baseStyle + "Default";
    var overClass = button.overClass = baseStyle + "Over";
    var selectedClass = button.selectedClass = baseStyle + "Selected";
    var disabledClass = button.disabledClass = baseStyle + "Disabled";

    button.style = style;
    button.className = defaultClass;

    var innerTable = this.CreateHTMLTable();
    button.innerTable = innerTable;
    innerTable.style.height = "100%";
    innerTable.style.width = "100%";
    button.appendChild(innerTable);

    if (imageName) {
        button.imageName = imageName;
        var img = button.image = document.createElement("img");
        img.style.width = (imageSizes ? imageSizes.width : 32) + "px";
        img.style.height = (imageSizes ? imageSizes.height : 32) + "px";
        img.style.pointerEvents = "none";
        if (StiMobileDesigner.checkImageSource(this.options, imageName)) StiMobileDesigner.setImageSource(img, this.options, imageName);

        var cellImg = button.cellImage = innerTable.addCell(img);
        cellImg.style.padding = "2px 2px 0px 2px";
        cellImg.style.textAlign = "center";
    }

    if (caption != null || typeof (caption) == "undefined") {
        var capCell = button.caption = innerTable.addCellInNextRow();
        capCell.style.padding = "0 3px 2px 3px";
        capCell.style.textAlign = "center";
        capCell.style.lineHeight = "1.1";

        if (maxWidth) {
            var div = document.createElement("div");
            div.style.display = "inline-block";
            div.innerHTML = caption;
            this.options.mainPanel.appendChild(div);
            var textWidth = div.offsetWidth;
            this.options.mainPanel.removeChild(div);
            if (maxWidth < textWidth / 2) maxWidth = textWidth / 2 + 5;
            capCell.style.maxWidth = maxWidth + "px";
        }

        if (caption) capCell.innerHTML = caption;
    }

    if (arrow) {
        var arrowImg = button.arrow = document.createElement("img");
        arrowImg.style.width = arrowImg.style.height = "8px";
        arrowImg.setAttribute("draggable", "false");
        StiMobileDesigner.setImageSource(arrowImg, this.options, "Arrows.SmallArrowDown.png");

        var cellArrow = button.cellArrow = innerTable.addCellInNextRow(arrowImg);
        cellArrow.style.padding = "0";
        cellArrow.style.textAlign = "center";
    }

    if (toolTip && this.options.showTooltips && typeof (toolTip) != "object") {
        button.setAttribute("title", toolTip);
    }

    button.onmouseover = function () {
        if (!jsObject.options.isTouchDevice) this.onmouseenter();
    }

    button.onmouseenter = function () {
        if (!this.isEnabled || (this["haveMenu"] && this.isSelected) || jsObject.options.isTouchClick) return;
        this.isOver = true;
        this.className = overClass;

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
        this.className = state ? (this.isOver ? overClass : defaultClass) : disabledClass;
    }

    button.setSelected = function (state) {
        if (this.groupName && state)
            for (var name in jsObject.options.buttons) {
                if (this.groupName == jsObject.options.buttons[name].groupName)
                    jsObject.options.buttons[name].setSelected(false);
            }

        this.isSelected = state;
        this.className = state ? selectedClass : (this.isEnabled ? (this.isOver ? overClass : defaultClass) : disabledClass);
    }

    button.setMarkerVisibleState = function (state) {
        if (state) {
            if (!this.marker) {
                var marker = document.createElement("div");
                marker.className = "stiUsingMarkerForBigButton";
                var markerInner = document.createElement("div");
                marker.appendChild(markerInner);
                this.style.position = "relative";
                this.appendChild(marker);
                this.marker = marker;
            }
        }
        else if (this.marker) {
            this.removeChild(this.marker);
            this.marker = null;
        }
    }

    button.action = function () { jsObject.ExecuteAction(this.name); }

    return button;
}