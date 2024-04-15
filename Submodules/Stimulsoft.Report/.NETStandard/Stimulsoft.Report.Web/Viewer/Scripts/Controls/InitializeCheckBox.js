
StiJsViewer.prototype.CheckBox = function (name, captionText, toolTip, styleColors) {
    var checkBox = this.CreateHTMLTable();
    checkBox.style.fontFamily = this.options.toolbar.fontFamily;
    if (this.options.toolbar.fontColor != "") checkBox.style.color = this.options.toolbar.fontColor;
    checkBox.jsObject = this;
    checkBox.isEnabled = true;
    checkBox.isChecked = false;
    checkBox.id = this.generateKey();
    checkBox.name = name;
    checkBox.captionText = captionText;
    if (toolTip) checkBox.setAttribute("title", toolTip);
    checkBox.className = "stiJsViewerCheckBox";
    checkBox.style.boxSizing = "content-box";
    if (name) {
        if (!this.controls.checkBoxes) this.controls.checkBoxes = {};
        this.controls.checkBoxes[name] = checkBox;
    }

    //Image
    checkBox.imageBlock = document.createElement("div");
    var size = this.options.isTouchDevice ? "16px" : "13px";
    checkBox.imageBlock.style.width = size;
    checkBox.imageBlock.style.height = size;
    checkBox.imageBlock.style.boxSizing = "content-box";
    checkBox.imageBlock.className = "stiJsViewerCheckBoxImageBlock";
    var imageBlockCell = checkBox.addCell(checkBox.imageBlock);
    imageBlockCell.style.lineHeight = "0";
    if (this.options.isTouchDevice) imageBlockCell.style.padding = "1px 3px 1px 1px";

    checkBox.image = document.createElement("img");
    checkBox.image.style.width = checkBox.image.style.height = "12px";
    StiJsViewer.setImageSource(checkBox.image, this.options, this.collections, "CheckBox" + (styleColors && styleColors.isDarkStyle ? "White.png" : ".png"));
    checkBox.image.style.visibility = "hidden";
    checkBox.image.style.verticalAlign = "baseline";
    var imgTable = this.CreateHTMLTable();
    imgTable.style.width = "100%";
    imgTable.style.height = "100%";
    checkBox.imageBlock.appendChild(imgTable);
    var imgCell = imgTable.addCell(checkBox.image);
    imgCell.style.textAlign = this.options.isTouchDevice ? "center" : "right";
    imgCell.style.verticalAlign = this.options.isTouchDevice ? "middle" : "top";

    //Caption
    if (captionText != null) {
        checkBox.captionCell = checkBox.addCell();
        if (!this.options.isTouchDevice) checkBox.captionCell.style.padding = "1px 0 0 4px";
        checkBox.captionCell.style.whiteSpace = "nowrap";
        checkBox.captionCell.innerText = captionText;
    }

    checkBox.onmouseover = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseenter();
    }

    checkBox.onmouseout = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseleave();
    }

    checkBox.onmouseenter = function () {
        if (!this.isEnabled) return;
        this.imageBlock.className = "stiJsViewerCheckBoxImageBlockOver";
    }

    checkBox.onmouseleave = function () {
        if (!this.isEnabled) return;
        this.imageBlock.className = "stiJsViewerCheckBoxImageBlock";
    }

    checkBox.onclick = function () {
        if (this.isTouchEndFlag || !this.isEnabled || this.jsObject.options.isTouchClick) return;
        this.setChecked(!this.isChecked);
        this.action();
    }

    checkBox.ontouchend = function () {
        if (!this.isEnabled || this.jsObject.options.fingerIsMoved) return;
        var this_ = this;
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);
        this.imageBlock.className = "stiJsViewerCheckBoxImageBlockOver";

        setTimeout(function () {
            this_.imageBlock.className = "stiJsViewerCheckBoxImageBlock";
            this_.setChecked(!this_.isChecked);
            this_.action();
        }, 150);

        this.isTouchEndTimer = setTimeout(function () {
            this_.isTouchEndFlag = false;
        }, 1000);
    }

    checkBox.ontouchstart = function () {
        this.jsObject.options.fingerIsMoved = false;
    }

    checkBox.setEnabled = function (state) {
        this.image.style.opacity = state ? "1" : "0.5";
        this.isEnabled = state;
        this.className = state ? "stiJsViewerCheckBox" : "stiJsViewerCheckBoxDisabled";
        this.imageBlock.className = state ? "stiJsViewerCheckBoxImageBlock" : "stiJsViewerCheckBoxImageBlockDisabled";
    }

    checkBox.setChecked = function (state, ignoreOnChecked) {
        this.image.style.visibility = (state) ? "visible" : "hidden";
        this.isChecked = state;
        this.setIndeterminate(false);
        if (!ignoreOnChecked) this.onChecked();
    }

    checkBox.setIndeterminate = function (state) {
        StiJsViewer.setImageSource(checkBox.image, this.jsObject.options, this.jsObject.collections, (state ? "CheckBoxIndeterminate" : "CheckBox") + (styleColors && styleColors.isDarkStyle ? "White.png" : ".png"));
        checkBox.image.style.visibility = state || this.isChecked ? "visible" : "hidden";
        checkBox.image.style.width = checkBox.image.style.height = state ? "13px" : "12px";
    }

    checkBox.onChecked = function () { }
    checkBox.action = function () { }

    return checkBox;
}