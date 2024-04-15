
StiJsViewer.prototype.InitializeProcessImage = function () {
    var jsObject = this;
    var processImage = this.Progress();
    processImage.jsObject = this;
    processImage.style.display = "none";
    this.controls.processImage = processImage;
    this.controls.mainPanel.appendChild(processImage);
    processImage.style.left = "calc(50% - 35px)";

    if (this.options.appearance.fullScreenMode) {
        processImage.style.top = "calc(50% - 100px)";
    }
    else {
        processImage.style.top = "250px";
    }

    processImage.show = function (progressValue) {
        this.changeStyle();
        this.style.display = "";

        if (progressValue)
            this.progressText.innerHTML = typeof progressValue == "string" ? progressValue : (parseInt(parseFloat(progressValue) * 100) + "%");
        else
            this.progressText.innerHTML = "";
    }

    processImage.hide = function () {
        this.style.display = "none";
        this.hideCancelButton();
    }

    processImage.changeStyle = function () {
        var reportPanel = jsObject.controls.reportPanel;
        var currIsReportPage = reportPanel && reportPanel.pages && reportPanel.pages.length > 0;
        var dbsParams = !currIsReportPage && jsObject.reportParams && jsObject.reportParams.pagesArray && jsObject.reportParams.pagesArray.length > 0 ? jsObject.reportParams.pagesArray : null;
        var isDarkStyle = dbsParams && dbsParams[0].actionColors && dbsParams[0].actionColors.isDarkStyle;

        if (isDarkStyle)
            this.setToLightStyle();
        else
            this.setToDefaultStyle();
    }

    return processImage;
}

StiJsViewer.prototype.AddProgressToControl = function (control) {
    if (!control) return;
    var progress = this.Progress();
    progress.style.display = "none";
    control.appendChild(progress);
    control.progress = progress;
    progress.owner = control;

    progress.show = function (left, top) {
        this.style.display = "";
        this.style.left = (left || (this.owner.offsetWidth / 2 - this.offsetWidth / 2)) + "px";
        this.style.top = (top || (this.owner.offsetHeight / 2 - this.offsetHeight / 2)) + "px";
    }

    progress.hide = function () {
        this.style.display = "none";
    }

    return progress;
}

StiJsViewer.prototype.Progress = function () {
    var jsObject = this;
    var progressContainer = document.createElement("div");
    progressContainer.style.position = "absolute";
    progressContainer.style.zIndex = "1000";

    var progress = document.createElement("div");
    progressContainer.appendChild(progress);
    progress.className = "js_viewer_loader js_viewer_loader_default";

    var progressText = document.createElement("div");
    progressText.className = "stiProgressText";
    progressContainer.appendChild(progressText);
    progressContainer.progressText = progressText;

    var buttonCancel = this.FormButton(null, this.collections.loc.ButtonCancel);
    buttonCancel.style.position = "absolute";
    buttonCancel.style.display = "none";
    buttonCancel.style.top = "145px";
    buttonCancel.style.border = "1px solid #c6c6c6";
    buttonCancel.style.left = "calc(50% - 40px)";
    buttonCancel.style.height = "20px";
    progressContainer.appendChild(buttonCancel);
    progressContainer.buttonCancel = buttonCancel;

    progressContainer.showCancelButton = function () {
        this.cancelTimer = setTimeout(function () {
            buttonCancel.style.display = "";
            buttonCancel.style.opacity = 1 / 100;
            var d = new Date();
            var endTime = d.getTime() + 300;
            jsObject.ShowAnimationForm(buttonCancel, endTime);
        }, 3000);
    }

    progressContainer.hideCancelButton = function () {
        buttonCancel.style.display = "none";
        clearTimeout(buttonCancel.animationTimer);
        clearTimeout(progressContainer.cancelTimer);
    }

    progressContainer.setToLightStyle = function () {
        progress.className = "js_viewer_loader js_viewer_loader_light";
    }

    progressContainer.setToDefaultStyle = function () {
        progress.className = "js_viewer_loader js_viewer_loader_default";
    }

    return progressContainer;
}

StiJsViewer.prototype.InitializeCenterText = function () {
    var centerTextContainer = document.createElement("div");
    centerTextContainer.style.position = "absolute";
    centerTextContainer.style.zIndex = "1000";
    centerTextContainer.style.display = "none";
    centerTextContainer.style.opacity = 0;
    centerTextContainer.style.transitionProperty = "opacity";
    centerTextContainer.style.transitionDuration = "300ms";
    centerTextContainer.style.fontFamily = this.options.toolbarFontFamily;
    centerTextContainer.style.color = this.options.toolbarFontColor;
    centerTextContainer.style.textShadow = "-1px -1px 0 #000, 1px -1px 0 #000, -1px 1px 0 #000, 1px 1px 0 #000";
    centerTextContainer.style.fontSize = "100px";

    var centerText = document.createElement("div");
    centerTextContainer.jsObject = this;
    centerTextContainer.text = centerText;
    centerTextContainer.appendChild(centerText);

    this.controls.centerText = centerTextContainer;
    this.controls.mainPanel.appendChild(centerTextContainer);

    centerTextContainer.show = function () {
        this.isAnimationProcess = true;
        this.toolbarHideTimer = null;
        this.style.display = "";
        this.jsObject.setObjectToCenter(this);

        setTimeout(function () {
            centerTextContainer.style.opacity = 1;
        });
        if (this.hideTimer) clearTimeout(this.hideTimer);
        this.hideTimer = setTimeout(function () {
            centerTextContainer.hide();
        }, 2000);
    }

    centerTextContainer.hide = function () {
        this.style.opacity = 0;
        if (this.hideTimer) clearTimeout(this.hideTimer);
        this.hideTimer = setTimeout(function () {
            centerTextContainer.style.display = "none";
        }, 300);
    }

    centerTextContainer.setText = function (text) {
        centerTextContainer.text.innerHTML = text;
        centerTextContainer.show();
    }

    return centerTextContainer;
}