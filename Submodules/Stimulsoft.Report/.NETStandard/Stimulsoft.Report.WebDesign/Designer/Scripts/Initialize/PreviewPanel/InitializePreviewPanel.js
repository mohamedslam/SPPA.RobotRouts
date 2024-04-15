
StiMobileDesigner.prototype.InitializePreviewPanel = function () {
    var jsObject = this;
    var previewPanel = this.ChildWorkPanel("previewPanel", "stiDesignerPreviewPanel");
    previewPanel.style.display = "none";

    var viewerContainer = document.createElement("div");
    this.options.viewerContainer = viewerContainer;
    this.options.mainPanel.appendChild(viewerContainer);
    viewerContainer.style.position = "absolute";
    viewerContainer.style.background = "#ffffff";
    viewerContainer.style.zIndex = "10";
    viewerContainer.style.display = "none";
    viewerContainer.visible = false;
    viewerContainer.style.bottom = "0px";
    viewerContainer.style.right = "0px";
    viewerContainer.style.left = "0px";
    viewerContainer.style.top = this.options.toolBar.offsetHeight + "px";

    viewerContainer.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
    }

    viewerContainer.addFrame = function () {
        if (viewerContainer.frame) viewerContainer.removeChild(viewerContainer.frame);
        viewerContainer.frame = document.createElement("iframe");
        viewerContainer.appendChild(viewerContainer.frame);
        viewerContainer.frame.style.position = "absolute";
        viewerContainer.frame.style.width = "100%";
        viewerContainer.frame.style.height = "100%";
        viewerContainer.frame.style.border = "0px";
    }

    previewPanel.onshow = function () {
        jsObject.options.previewMode = true;
        this.undoButtonState = jsObject.options.buttons.undoButton.isEnabled;
        this.redoButtonState = jsObject.options.buttons.redoButton.isEnabled;
        jsObject.options.buttons.undoButton.setEnabled(false);
        jsObject.options.buttons.redoButton.setEnabled(false);
        if (jsObject.options.jsMode && jsObject.options.buttons.saveReportHotButton) {
            jsObject.options.buttons.saveReportHotButton.setEnabled(false);
        }
        jsObject.options.viewerContainer.changeVisibleState(true);
        if (!jsObject.options.cloudMode && !jsObject.options.serverMode && jsObject.options.viewer) {
            (jsObject.options.viewer.jsObject.controls || jsObject.options.viewer.jsObject.options).reportPanel.style.top = jsObject.options.viewer.jsObject.options.toolbar.offsetHeight + "px";
        }
    }

    previewPanel.onhide = function () {
        jsObject.options.previewMode = false;
        jsObject.options.buttons.undoButton.setEnabled(this.undoButtonState);
        jsObject.options.buttons.redoButton.setEnabled(this.redoButtonState);
        if (jsObject.options.jsMode && jsObject.options.buttons.saveReportHotButton) {
            jsObject.options.buttons.saveReportHotButton.setEnabled(true);
        }
        this.jsObject.options.viewerContainer.changeVisibleState(false);

        if (jsObject.options.serverMode) {
            var previewFrame = jsObject.options.viewerContainer.frame;
            var win = previewFrame ? (previewFrame.contentWindow || previewFrame.window) : null;

            if (win && win.jsStiCloudReportsMobileViewer) {
                win.jsStiCloudReportsMobileViewer.StopRenderReportTask();
            }

            if (jsObject.options.buttons.previewToolButton)
                jsObject.options.buttons.previewToolButton.progress.style.visibility = "hidden";
        }
        else {
            var viewer = jsObject.options.viewer || jsObject.options.viewerContainer.firstChild;
            if (viewer) {
                viewer.jsObject.stopRefreshReportTimer();

                var controlsCollection = (viewer.jsObject.controls || viewer.jsObject.options);
                controlsCollection.processImage.hide();

                if (viewer.jsObject.options.currentForm && viewer.jsObject.options.currentForm.visible)
                    viewer.jsObject.options.currentForm.changeVisibleState(false);

                if (viewer.jsObject.options.currentMenu)
                    viewer.jsObject.options.currentMenu.changeVisibleState(false);

                setTimeout(function () { controlsCollection.reportPanel.clear(); }, 50);
            }
        }

        if (jsObject.SendCommandCloseViewer) jsObject.SendCommandCloseViewer();
    }
}