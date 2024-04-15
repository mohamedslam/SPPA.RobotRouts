
StiMobileDesigner.prototype.CheckBox = function (name, caption, infoToolTip) {
    var checkBox = this.CreateHTMLTable();
    var jsObject = checkBox.jsObject = this;

    if (name != null) this.options.controls[name] = checkBox;
    checkBox.name = name != null ? name : this.generateKey();
    checkBox.id = this.options.mobileDesigner.id + checkBox.name;
    checkBox.isEnabled = true;
    checkBox.isChecked = false;
    checkBox.className = "stiDesignerCheckBox stiDesignerClearAllStyles";
    checkBox.style.boxSizing = "content-box";

    //Image
    var imageBlock = checkBox.imageBlock = document.createElement("div");
    imageBlock.className = "stiDesignerCheckBoxImageBlock";
    imageBlock.style.width = imageBlock.style.height = this.options.isTouchDevice ? "16px" : "13px";

    var imageBlockCell = checkBox.addCell(imageBlock);
    imageBlockCell.style.padding = "0px";
    imageBlockCell.style.border = "0px";
    imageBlockCell.style.lineHeight = "0";
    if (this.options.isTouchDevice) imageBlockCell.style.padding = "1px 3px 1px 1px";

    var image = checkBox.image = document.createElement("img");
    image.setAttribute("draggable", "false");
    image.style.width = image.style.height = "12px";
    StiMobileDesigner.setImageSource(image, this.options, "CheckBox.png");
    image.style.visibility = "hidden";

    var imgTable = this.CreateHTMLTable();
    imgTable.style.width = imgTable.style.height = "100%";
    imageBlock.appendChild(imgTable);
    var imgCell = imgTable.addCell(image);
    imgCell.style.textAlign = this.options.isTouchDevice ? "center" : "right";
    imgCell.style.verticalAlign = this.options.isTouchDevice ? "middle" : "top";

    //Caption
    if (caption != null || typeof (caption) == "undefined") {
        var captionCell = checkBox.captionCell = checkBox.addCell();
        captionCell.style.padding = "0px";
        captionCell.style.border = "0px";
        captionCell.style.whiteSpace = "nowrap";
        if (!this.options.isTouchDevice) captionCell.style.padding = "1px 0 0 4px";
        if (caption) captionCell.innerText = caption;
    }

    //InfoToolTip
    if (infoToolTip) {
        var infoImg = document.createElement("img");
        infoImg.style.width = infoImg.style.height = "12px";
        StiMobileDesigner.setImageSource(infoImg, this.options, "Information.png");
        infoImg.style.margin = "4px 4px 0 4px";
        checkBox.addCell(infoImg);
        infoImg.setAttribute("title", infoToolTip);
    }

    checkBox.onmouseover = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseenter();
    }

    checkBox.onmouseenter = function () {
        if (!this.isEnabled || this.jsObject.options.isTouchClick) return;
        imageBlock.className = "stiDesignerCheckBoxImageBlockOver";
    }

    checkBox.onmouseleave = function () {
        if (!this.isEnabled) return;
        imageBlock.className = "stiDesignerCheckBoxImageBlock";
    }

    checkBox.onclick = function () {
        if (this.isTouchEndFlag || !this.isEnabled || this.jsObject.options.isTouchClick) return;
        checkBox.setChecked(!checkBox.isChecked);
        checkBox.action();
    }

    checkBox.ontouchend = function () {
        if (!this.isEnabled || this.jsObject.options.fingerIsMoved) return;
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);
        imageBlock.className = "stiDesignerCheckBoxImageBlockOver";

        setTimeout(function () {
            imageBlock.className = "stiDesignerCheckBoxImageBlock";
            checkBox.setChecked(!checkBox.isChecked);
            checkBox.action();
        }, 150);

        this.isTouchEndTimer = setTimeout(function () {
            checkBox.isTouchEndFlag = false;
        }, 1000);
    }

    checkBox.ontouchstart = function () {
        this.jsObject.options.fingerIsMoved = false;
    }

    checkBox.setEnabled = function (state) {
        this.image.style.opacity = state ? "1" : "0.3";
        this.isEnabled = state;
        this.className = (state ? "stiDesignerCheckBox" : "stiDesignerCheckBoxDisabled") + " stiDesignerClearAllStyles";
        imageBlock.className = state ? "stiDesignerCheckBoxImageBlock" : "stiDesignerCheckBoxImageBlockDisabled";
    }

    checkBox.setChecked = function (state) {
        this.image.style.visibility = state ? "visible" : "hidden";
        this.isChecked = state || false;
    }

    checkBox.action = function () { }

    return checkBox;
}