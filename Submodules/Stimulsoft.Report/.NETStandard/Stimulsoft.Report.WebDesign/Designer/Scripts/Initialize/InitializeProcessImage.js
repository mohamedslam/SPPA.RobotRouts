
StiMobileDesigner.prototype.ProcessImageStatusPanel = function () {
    var processImage = this.CreateHTMLTable();;
    processImage.jsObject = this;
    processImage.style.display = "none";
    processImage.className = "stiDesignerProcessImageStatusPanel stiDesignerClearAllStyles";
    this.options.processImageStatusPanel = processImage;

    var img = this.ProgressMini("white");
    processImage.addCell(img).style.padding = "0 3px 0 3px";

    var text = processImage.addCell();
    text.innerHTML = this.loc.A_WebViewer.Loading.replace("...", "");
    text.style.fontSize = "12px";
    text.style.padding = "0 5px 0 0";

    processImage.show = function () {
        this.style.display = "";
    }

    processImage.hide = function () {
        this.style.display = "none";
    }

    return processImage;
}

StiMobileDesigner.prototype.InitializeProcessImage = function () {
    var processImage = this.Progress();
    processImage.jsObject = this;
    processImage.style.display = "none";
    processImage.style.top = "50%"
    processImage.style.left = "50%"
    processImage.style.marginLeft = "-32px";
    processImage.style.marginTop = "-100px";
    this.options.processImage = processImage;
    this.options.mainPanel.appendChild(processImage);

    processImage.show = function (progressValue, commandGuid) {
        this.progressText.innerHTML = progressValue != null ? parseInt(parseFloat(progressValue) * 100) + "%" : "";
        this.commandGuid = commandGuid;
        this.style.display = "";
        if (!this.jsObject.options.disabledPanels) this.jsObject.InitializeDisabledPanels();
        this.jsObject.options.disabledPanels[6].style.display = "";
    }

    processImage.hide = function () {
        this.style.display = "none";
        this.hideCancelButton();
        if (!this.jsObject.options.disabledPanels) this.jsObject.InitializeDisabledPanels();
        this.jsObject.options.disabledPanels[6].style.display = "none";
    }

    return processImage;
}