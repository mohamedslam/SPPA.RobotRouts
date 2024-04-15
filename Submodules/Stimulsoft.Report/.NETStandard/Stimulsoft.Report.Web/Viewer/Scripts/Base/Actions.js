StiJsViewer.prototype.postAction = function (action, bookmarkPage, bookmarkAnchor, componentGuid) {
    var jsObject = this;
    switch (action) {
        case "Refresh":
            if (jsObject.options.refreshInProgress || jsObject.options.resizeInProgress || jsObject.service.isRequestInProcess) return;
            jsObject.options.tablesColumnsOrder = {};
            jsObject.options.refreshInProgress = true;
            break;

        case "Print":
            switch (this.options.toolbar.printDestination) {
                case "Pdf": this.postPrint("PrintPdf"); break;
                case "Direct": this.postPrint("PrintWithoutPreview"); break;
                case "WithPreview": this.postPrint("PrintWithPreview"); break;
                default: this.controls.menus.printMenu.changeVisibleState(!this.controls.menus.printMenu.visible); break;
            }
            return;

        case "Open":
            if (!this.checkCloudAuthorization("open")) return;
            var openReportDialog = this.InitializeOpenDialog("openReportDialog", function (fileName, filePath, content) {
                openReportDialog.jsObject.postOpen(fileName, content);
            }, ".mdc,.mdz,.mdx,.mrt,.mrz,.mrx");
            openReportDialog.action();
            return;

        case "Save":
            this.controls.menus.saveMenu.changeVisibleState(!this.controls.menus.saveMenu.visible);
            return;

        case "SendEmail":
            this.controls.menus.sendEmailMenu.changeVisibleState(!this.controls.menus.sendEmailMenu.visible);
            return;

        case "Zoom":
            this.controls.menus.zoomMenu.changeVisibleState(!this.controls.menus.zoomMenu.visible);
            return;

        case "ViewMode":
            this.controls.menus.viewModeMenu.changeVisibleState(!this.controls.menus.viewModeMenu.visible);
            return;

        case "FirstPage":
        case "PrevPage":
        case "NextPage":
        case "LastPage":
            if (action == "FirstPage") this.reportParams.pageNumber = 0;
            if (action == "PrevPage" && this.reportParams.pageNumber > 0) this.reportParams.pageNumber--;
            if (action == "NextPage" && this.reportParams.pageNumber < this.reportParams.pagesCount - 1) this.reportParams.pageNumber++;
            if (action == "LastPage") this.reportParams.pageNumber = this.reportParams.pagesCount - 1;
            if (this.controls.reportPanel.pagesNavigationIsActive()) {
                this.scrollToPage(this.reportParams.pageNumber);
                if (this.controls.toolbar) this.controls.toolbar.changeToolBarState();
                return;
            }
            break;

        case "FullScreen":
            this.changeFullScreenMode(!this.options.appearance.fullScreenMode);
            return;

        case "Zoom25": this.reportParams.zoom = 25; break;
        case "Zoom50": this.reportParams.zoom = 50; break;
        case "Zoom75": this.reportParams.zoom = 75; break;
        case "Zoom100": this.reportParams.zoom = 100; break;
        case "Zoom150": this.reportParams.zoom = 150; break;
        case "Zoom200": this.reportParams.zoom = 200; break;

        case "ZoomOnePage":
        case "ZoomPageWidth":
            var toolbar = this.controls.toolbar;
            if (this.options.toolbar.displayMode == "Separated") {
                if (toolbar.controls.ZoomOnePage) toolbar.controls.ZoomOnePage.setSelected(action == "ZoomOnePage");
                if (toolbar.controls.ZoomPageWidth) toolbar.controls.ZoomPageWidth.setSelected(action == "ZoomPageWidth");
            }
            this.reportParams.zoom = action == "ZoomPageWidth"
                ? parseInt(this.controls.reportPanel.getZoomByPageWidth())
                : parseInt(this.controls.reportPanel.getZoomByPageHeight());
            break;

        case "ViewModeSinglePage":
            this.reportParams.viewMode = "SinglePage";
            break;

        case "ViewModeContinuous":
            this.reportParams.viewMode = "Continuous";
            break;

        case "ViewModeMultiplePages":
            this.reportParams.viewMode = "MultiplePages";
            break;

        case "ViewModeMultiPage":
            this.reportParams.viewMode = "MultiPage";
            this.reportParams.multiPageContainerWidth = this.controls.reportPanel.offsetWidth;
            this.reportParams.multiPageContainerHeight = this.controls.reportPanel.offsetHeight;
            this.reportParams.multiPageMargins = 10;
            break;

        case "GoToPage":
            this.reportParams.pageNumber = this.controls.toolbar.controls["PageControl"].textBox.getCorrectValue() - 1;
            if (this.controls.reportPanel.pagesNavigationIsActive()) {
                this.scrollToPage(this.reportParams.pageNumber);
                if (this.controls.toolbar) this.controls.toolbar.changeToolBarState();
                return;
            }
            break;

        case "BookmarkAction":
            if (this.reportParams.pageNumber == bookmarkPage || this.reportParams.viewMode != "SinglePage") {
                this.scrollToAnchor(bookmarkAnchor, componentGuid);
                return;
            }
            else {
                this.reportParams.pageNumber = bookmarkPage;
                this.options.bookmarkAnchor = bookmarkAnchor;
                this.options.componentGuid = componentGuid;
            }
            break;

        case "Bookmarks":
            this.controls.bookmarksPanel.changeVisibleState(!this.controls.buttons["Bookmarks"].isSelected);
            return;

        case "Parameters":
            this.controls.parametersPanel.changeVisibleState(!this.controls.buttons["Parameters"].isSelected);
            return;

        case "Resources":
            this.controls.resourcesPanel.changeVisibleState(!this.controls.buttons["Resources"].isSelected);
            return;

        case "Find":
            this.controls.findPanel.changeVisibleState(!this.controls.toolbar.controls.Find.isSelected);
            return;

        case "About":
            this.controls.aboutPanel.changeVisibleState(!this.controls.toolbar.controls.About.isSelected);
            return;

        case "Design":
            this.postDesign();
            return;

        case "Pin":
            if (this.controls.toolbar) this.controls.toolbar.changePinState(!this.options.toolbar.autoHide);
            return;

        case "Submit":
            this.reportParams.editableParameters = null;
            if (this.reportParams.type == "Report") this.reportParams.pageNumber = 0;
            if (this.options.isMobileDevice) this.controls.parametersPanel.changeVisibleState(false);
            setTimeout(function () {
                jsObject.postInteraction({ action: "Variables", variables: jsObject.controls.parametersPanel.getParametersValues() });
            }, jsObject.options.isMobileDevice ? 500 : 0);
            return;

        case "Reset":
            this.options.parameters = {};
            if (this.options.paramsVariablesStartValues) {
                this.options.paramsVariables = this.options.paramsVariablesStartValues;
            }
            this.controls.parametersPanel.clearParameters();
            this.controls.parametersPanel.addParameters();
            this.controls.parametersPanel.hideHiddenRows();
            return;

        case "Editor":
            this.SetEditableMode(!this.options.editableMode);
            return;

        case "Signature":
            var signatureForm = this.controls.forms.editSignature || this.InitializeEditSignatureForm();
            signatureForm.show();
            return;
    }

    // Check for OnePage or PageWidth zoom when the viewer is launched for the first time
    if (this.reportParams.zoom == -1 || this.reportParams.zoom == -2)
        this.reportParams.autoZoom = this.reportParams.zoom;

    // Correct viewer action
    var viewerAction = action == "Refresh" ? "RefreshReport" : "GetPages";
    if (!action || action == "GetReport") {
        this.clearViewerState(true);
        viewerAction = "GetReport";
    }

    var requestUrl = this.getActionRequestUrl(this.options.requestUrl,
        (viewerAction == "GetReport" || this.options.server.cacheMode == "None")
            ? this.options.actions.getReport
            : this.options.actions.viewerEvent);

    if (this.options.reportDesignerMode && (viewerAction == "GetPages" || viewerAction == "GetReport") && this.options.startPageNumber && this.controls.dashboardsPanel) {
        if (viewerAction == "GetReport") {
            this.reportParams.originalPageNumber = this.options.startPageNumber;
        }
        else {
            var buttons = this.controls.dashboardsPanel.buttons;
            for (var i = 0; i < buttons.length; i++) {
                if (buttons[i].reportParams && buttons[i].reportParams.originalPageNumber == this.options.startPageNumber && !buttons[i].isNestedPage) {
                    if (!buttons[i].closeButtonAction) {
                        buttons[i].action();
                        return;
                    }
                }
            }
            this.options.startPageNumber = null;
        }
    }

    this.controls.processImage.show();
    this.postAjax(requestUrl, { action: viewerAction }, this.showReportPage);
}

StiJsViewer.prototype.postOpen = function (fileName, content) {
    if (typeof content != "string" || content == "") return;
    if (content.indexOf("<?xml") == 0 || content.indexOf("{") == 0) content = StiBase64.encode(content);

    var data = {
        "action": "OpenReport",
        "openingFileName": fileName || "Report.mdc",
        "base64Data": content.indexOf("base64,") > 0 ? content.substr(content.indexOf("base64,") + 7) : content
    };

    this.clearViewerState();
    this.reportParams.reportFileName = fileName;

    var jsObject = this;
    if (fileName && (fileName.toLowerCase().indexOf(".mdx") >= 0 || fileName.toLowerCase().indexOf(".mrx") >= 0)) {
        var passwordForm = this.InitializePasswordForm();
        passwordForm.show(function (password) {
            data.openingFilePassword = password;
            jsObject.controls.processImage.show();
            jsObject.postAjax(jsObject.getActionRequestUrl(jsObject.options.requestUrl, jsObject.options.actions.openReport), data, jsObject.showReportPage);
        }, this.collections.loc["PasswordEnter"] + ":");
    }
    else {
        this.controls.processImage.show();
        this.postAjax(this.getActionRequestUrl(this.options.requestUrl, this.options.actions.openReport), data, this.showReportPage);
    }
}

StiJsViewer.prototype.postPrint = function (printAction) {
    var data = {
        "action": "PrintReport",
        "printAction": printAction,
        "bookmarksPrint": this.options.appearance.bookmarksPrint
    };

    var url = this.getActionRequestUrl(this.options.requestUrl, this.options.actions.printReport);
    switch (printAction) {
        case "PrintPdf":
            if (this.options.appearance.printToPdfMode == 'Popup' || this.getNavigatorName() == 'Safari' || this.getNavigatorName() == 'iPad') this.printAsPdfPopup(data);
            else this.printAsPdf(url, data);
            break;

        case "PrintWithPreview":
            this.postAjax(url, data, this.printAsPopup);
            break;

        case "PrintWithoutPreview":
            this.postAjax(url, data, this.printAsHtml);
            break;
    }
}

StiJsViewer.prototype.printAsPdfPopup = function (data) {
    var url = this.getActionRequestUrl(this.options.requestAbsoluteUrl, this.options.actions.printReport);
    var win = this.openNewWindow("about:blank", "_blank");
    if (win != null) this.postForm(url, data, win.document);
}

StiJsViewer.prototype.printAsPdf = function (url, data) {
    data.responseType = "blob";

    var printFrameId = this.options.viewerId + "_PdfPrintFrame";
    var printFrame = document.getElementById(printFrameId);
    if (printFrame) this.controls.viewer.removeChild(printFrame);

    printFrame = document.createElement("iframe");
    printFrame.id = printFrameId;
    printFrame.name = printFrameId;
    printFrame.width = "0";
    printFrame.height = "0";
    printFrame.style.position = "absolute";
    printFrame.style.zIndex = "-100";
    printFrame.style.border = "none";
    printFrame.style.pointerEvents = "none";
    printFrame.style.visibility = "hidden";

    // Firefox does not load the invisible content of the iframe
    if (this.getNavigatorName() == "Mozilla") {
        printFrame.width = "100px";
        printFrame.height = "100px";
    }

    this.controls.viewer.insertBefore(printFrame, this.controls.viewer.firstChild);

    // Print as Blob for Blazor
    if (this.options.blazorMode) {
        this.postAjax(url, data, this.printAsPdfBlob);
        return;
    }

    // Manual printing in browsers that do not support automatic PDF printing
    if (this.getNavigatorName() != "Mozilla") {
        printFrame.onload = function () {
            printFrame.contentWindow.focus();
            printFrame.contentWindow.print();
        }
    }

    var form = document.createElement("FORM");
    form.setAttribute("id", "printForm");
    form.setAttribute("method", "POST");
    form.setAttribute("action", url);
    form.setAttribute("target", this.options.viewerId + "_PdfPrintFrame");

    var params = this.createPostParameters(data, true);
    for (var key in params) {
        var paramsField = document.createElement("INPUT");
        paramsField.setAttribute("type", "hidden");
        paramsField.setAttribute("name", key);
        paramsField.setAttribute("value", params[key]);
        form.appendChild(paramsField);
    }

    document.body.appendChild(form);
    form.submit();
    document.body.removeChild(form);

    this.removePrintFrame(printFrame);
}

StiJsViewer.prototype.printAsPdfBlob = function (data, jsObject) {
    var binary = atob(data.replace(/\s/g, ''));
    var buffer = new ArrayBuffer(binary.length);
    var bytes = new Uint8Array(buffer);
    for (let i = 0; i < binary.length; i++) {
        bytes[i] = binary.charCodeAt(i);
    }

    var blob = new Blob([bytes], { type: "application/pdf" });
    var printFrameId = jsObject.options.viewerId + "_PdfPrintFrame";
    var printFrame = document.getElementById(printFrameId);

    if (jsObject.getNavigatorName() != "Mozilla") {
        printFrame.onload = function () {
            printFrame.contentWindow.focus();
            printFrame.contentWindow.print();
        }
    }

    printFrame.src = URL.createObjectURL(blob);

    jsObject.removePrintFrame(printFrame);
}

StiJsViewer.prototype.printAsPopup = function (text, jsObject) {
    var width = jsObject.controls.reportPanel.getPagesSize().width || 790;
    var win = jsObject.openNewWindow("about:blank", "PrintReport", "height=900,width=" + width + ",toolbar=no,menubar=yes,scrollbars=yes,resizable=yes,location=no,directories=no,status=no");
    if (win != null) {
        win.document.open();
        win.document.write(text);
        win.document.close();
    }
}

StiJsViewer.prototype.printAsHtml = function (text, jsObject) {
    if (jsObject.showError(text)) return;

    // Remove '_PdfPrintFrame', this should fix IE strange error
    var printFrameId = jsObject.options.viewerId + "_PdfPrintFrame";
    var printFrame = document.getElementById(printFrameId);
    if (printFrame) jsObject.controls.viewer.removeChild(printFrame);

    printFrameId = jsObject.options.viewerId + "_HtmlPrintFrame";
    printFrame = document.getElementById(printFrameId);
    if (printFrame) jsObject.controls.viewer.removeChild(printFrame);

    printFrame = document.createElement("iframe");
    printFrame.id = printFrameId;
    printFrame.name = printFrameId;
    printFrame.width = "0";
    printFrame.height = "0";
    printFrame.style.position = "absolute";
    printFrame.style.zIndex = "-100";
    printFrame.style.border = "none";
    printFrame.style.pointerEvents = "none";
    printFrame.style.visibility = "hidden";
    jsObject.controls.viewer.insertBefore(printFrame, jsObject.controls.viewer.firstChild);

    printFrame.onload = function () {
        printFrame.contentWindow.focus();
        printFrame.contentWindow.print();
    }

    printFrame.contentWindow.document.open();
    printFrame.contentWindow.document.write(text);
    printFrame.contentWindow.document.close();

    jsObject.removePrintFrame(printFrame);
}

StiJsViewer.prototype.removePrintFrame = function (printFrame) {
    var jsObject = this;
    var removeFunction = function () {
        if (printFrame) jsObject.controls.viewer.removeChild(printFrame);
        window.removeEventListener("focus", removeFunction);
    }
    if (window.addEventListener)
        window.addEventListener("focus", removeFunction);
}

StiJsViewer.prototype.postExport = function (format, settings, elementName, isDashboardExport) {
    var data = {
        action: isDashboardExport ? "ExportDashboard" : "ExportReport",
        exportFormat: format,
        exportSettings: settings,
        elementName: elementName
    };

    var doc = settings && settings.OpenAfterExport && this.options.appearance.openExportedReportWindow == "_blank" ? this.openNewWindow("about:blank", "_blank").document : null;
    var url = doc ? this.options.requestAbsoluteUrl : this.options.requestUrl;
    this.postForm(this.getActionRequestUrl(url, this.options.actions.exportReport), data, doc);
}

StiJsViewer.prototype.postEmail = function (format, settings) {
    var jsObject = this;
    var data = {
        action: "EmailReport",
        exportFormat: format,
        exportSettings: settings
    };

    this.controls.processImage.show();
    this.postAjax(this.getActionRequestUrl(this.options.requestUrl, this.options.actions.emailReport), data, this.emailResult);
    setTimeout(function () {
        jsObject.controls.processImage.hide();
    }, 3000);
}

StiJsViewer.prototype.postDesign = function () {
    var doc = this.options.appearance.designWindow == "_blank" ? this.openNewWindow("about:blank", "_blank").document : null;
    var url = doc ? this.options.requestAbsoluteUrl : this.options.requestUrl;
    this.postForm(this.getActionRequestUrl(url, this.options.actions.designReport), { action: "DesignReport" }, doc);
}

StiJsViewer.prototype.postInteraction = function (params) {
    if (!this.options.actions.interaction) {
        if (this.controls.buttons["Parameters"]) this.controls.buttons["Parameters"].setEnabled(false);
        return;
    }
    // Add new drill-down parameters to drill-down queue and calc guid
    if (params.action != "InitVars" && (params.action == "DrillDown" || params.action == "DashboardDrillDown")) {
        if (this.options.drillDownInProgress) return;

        if (params.action == "DashboardDrillDown" && params.drillDownParameters) {
            params.drillDownParameters.isDashboardDrillDown = true;
        }
        var drillDownParameters = this.reportParams.drillDownParameters || [];
        params.drillDownParameters = params.drillDownParameters ? drillDownParameters.concat(params.drillDownParameters) : drillDownParameters;

        if (params.action == "DrillDown")
            params.drillDownGuid = hex_md5(JSON.stringify(this.sortPropsInDrillDownParameters(params.drillDownParameters)));
        else
            params.dashboardDrillDownGuid = hex_md5(JSON.stringify(this.sortPropsInDrillDownParameters(params.drillDownParameters)));

        if (this.controls.parametersPanel) {
            params.variables = this.controls.parametersPanel.getParametersValues()
        }

        this.options.drillDownInProgress = true;
    }

    this.controls.processImage.show();
    this.postAjax(
        this.getActionRequestUrl(this.options.requestUrl, this.options.actions.interaction),
        params,
        params.action == "InitVars" ? this.showParametersPanel : this.showReportPage
    );
}

StiJsViewer.prototype.postReportResource = function (resourceName, viewType) {
    var data = {
        action: "ReportResource",
        reportResourceParams: {
            resourceName: resourceName,
            viewType: viewType
        }
    };

    var doc = viewType == "View" ? this.openNewWindow("about:blank", "_blank").document : null;
    var url = doc ? this.options.requestAbsoluteUrl : this.options.requestUrl;
    this.postForm(this.getActionRequestUrl(url, this.options.actions.viewerEvent), data, doc);
}

StiJsViewer.prototype.initAutoUpdateCache = function (data, jsObject) {
    if (jsObject.options.server.allowAutoUpdateCache) {
        if (jsObject.controls.timerAutoUpdateCache) clearTimeout(jsObject.controls.timerAutoUpdateCache);
        jsObject.controls.timerAutoUpdateCache = setTimeout(function () {
            jsObject.postAjax(jsObject.getActionRequestUrl(jsObject.options.requestUrl, jsObject.options.actions.viewerEvent), { action: "UpdateCache" }, jsObject.initAutoUpdateCache);
        }, jsObject.options.server.timeoutAutoUpdateCache);
    }
}

StiJsViewer.prototype.stopAutoUpdateCache = function () {
    if (this.controls.timerAutoUpdateCache) {
        clearTimeout(this.controls.timerAutoUpdateCache);
        this.controls.timerAutoUpdateCache = null;
    }
}

StiJsViewer.prototype.emailResult = function (data, jsObject) {
    jsObject.controls.processImage.hide();
    if (data == "0")
        alert(jsObject.collections.loc["EmailSuccessfullySent"]);
    else {
        if (data.indexOf("<?xml") == 0) {
            alert(jsObject.GetXmlValue(data, "ErrorCode"));
            alert(jsObject.GetXmlValue(data, "ErrorDescription"));
        }
        else
            alert(data);
    }
}

StiJsViewer.prototype.parseReportParameters = function (parameters) {
    this.reportParams.pagesArray = parameters.pagesArray;

    // Apply new report parameters, if not update current page
    if (parameters.action != "GetPages") {
        this.reportParams.type = parameters.reportType;
        this.reportParams.drillDownGuid = parameters.drillDownGuid;
        this.reportParams.dashboardDrillDownGuid = parameters.dashboardDrillDownGuid;
        this.reportParams.pagesCount = parameters.pagesCount;
        if (parameters.pageNumber != null) this.reportParams.pageNumber = parameters.pageNumber;
        this.reportParams.zoom = parameters.zoom;
        this.reportParams.viewMode = parameters.viewMode;
        this.reportParams.reportFileName = parameters.reportFileName;
        this.reportParams.collapsingStates = parameters.collapsingStates;
        if (parameters.bookmarksContent) this.reportParams.bookmarksContent = parameters.bookmarksContent;
        if (parameters.resources) this.reportParams.resources = parameters.resources;
        this.reportParams.isCompilationMode = parameters.isCompilationMode;
        if (parameters.variablesValues) this.reportParams.variablesValues = parameters.variablesValues;
        if (parameters.parametersDateFormat) this.options.appearance.parametersPanelDateFormat = parameters.parametersDateFormat;
        if (parameters.tableOfContentsPointers) this.reportParams.tableOfContentsPointers = parameters.tableOfContentsPointers;
    }
}

StiJsViewer.prototype.parseCloudParameters = function (parameters) {
    var jsObject = this;
    var description = "Upgrade your plan and get more possibilities for your report.";
    var initializeNotificationForm = function () {
        return jsObject.controls.forms.notificationForm || jsObject.InitializeNotificationForm();
    }
    if (parameters.maxRefreshes) {
        var message = this.collections.loc["QuotaMaximumRefreshCountExceeded"];
        initializeNotificationForm().show(message, description, "Notifications.Warning.png");
        return;
    }
    if (parameters.maxReportPages) {
        var message = this.collections.loc["QuotaMaximumReportPagesCountExceeded"] + "<br>" + this.collections.loc["Maximum"] + " " + this.numberWithSpaces(parameters.maxReportPages) + ".";
        initializeNotificationForm().show(message, description, "Notifications.Elements.png");
        return;
    }
    if (parameters.maxDataRows) {
        var message = this.collections.loc["QuotaMaximumDataRowsCountExceeded"] + "<br>" + this.collections.loc["Maximum"] + " " + this.numberWithSpaces(parameters.maxDataRows) + ".";
        initializeNotificationForm().show(message, description, "Notifications.Lines.png");
        return;
    }
    if (parameters.maxResources) {
        var message = this.collections.loc["QuotaMaximumResourcesCountExceeded"] + "<br>" + this.collections.loc["Maximum"] + " " + this.numberWithSpaces(parameters.maxResources) + "."
        initializeNotificationForm().show(message, description, "Notifications.Files.png");
        return;
    }
    if (parameters.maxResourceSize) {
        var message = this.collections.loc["QuotaMaximumResourceSizeExceeded"] + "<br>" + this.collections.loc["Maximum"] + " " + this.GetHumanFileSize(parameters.maxResourceSize) + "."
        initializeNotificationForm().show(message, description, "Notifications.Files.png");
        return;
    };
    if (parameters.notAllowDatabase) {
        var message = "The '" + parameters.notAllowDatabase + "' data source is not available in your subscription.";
        initializeNotificationForm().show(message, description, "Notifications.Blocked.png");
        return;
    }
    if (parameters.notAllowDataTransformation) {
        var message = "Data transformation is not available in your subscription.";
        initializeNotificationForm().show(message, description, "Notifications.Blocked.png");
        return;
    }
}

StiJsViewer.prototype.showParametersPanel = function (data, jsObject) {
    if (jsObject.showError(data)) data = null;
    var paramsVariables = typeof data == "string" ? JSON.parse(data) : data;

    jsObject.options.isParametersReceived = true;
    jsObject.controls.processImage.hide();

    if (jsObject.checkParametersPanelAlreadyBuildedForDrillDown(paramsVariables))
        return;

    jsObject.options.paramsVariables = paramsVariables;
    jsObject.InitializeParametersPanel();

    if (jsObject.reportParams.type == "Dashboard" || (jsObject.options.reportDesignerMode && jsObject.options.startPageNumber && jsObject.controls.dashboardsPanel)) {
        jsObject.postAction("GetPages");
        jsObject.options.startPageNumber = null;
    }
}

StiJsViewer.prototype.checkParametersPanelAlreadyBuildedForDrillDown = function (paramsVariables) {
    if (this.controls.drillDownPanel && this.controls.drillDownPanel.visible && this.getCountObjects(paramsVariables) > 0 && this.getCountObjects(this.options.paramsVariables) == this.getCountObjects(paramsVariables)) {
        for (var key in paramsVariables) {
            if (paramsVariables[key].name != this.options.paramsVariables[key].name)
                return false;
        }
        return true;
    }
    return false;
}

StiJsViewer.prototype.showDrillDownPage = function (reportFileName, drillDownGuid, drillDownParameters, useDbsDrillDownPanel) {
    if (useDbsDrillDownPanel) return;

    this.controls.drillDownPanel.changeVisibleState(true);

    var buttonExist = false;
    var currentButton = this.controls.drillDownPanel.selectedButton;
    var reportScrollPos = this.controls.reportPanel.scrollTop;

    if (currentButton && reportScrollPos) {
        currentButton.reportScrollPos = reportScrollPos;
    }

    for (var name in this.controls.drillDownPanel.buttons) {
        var button = this.controls.drillDownPanel.buttons[name];
        if (button.reportParams.drillDownGuid == drillDownGuid) {
            buttonExist = true;
            button.style.display = "inline-block";
            button.select();
            break;
        }
    }
    if (!buttonExist) {
        var button = this.controls.drillDownPanel.addButton(reportFileName, null, drillDownParameters);
        this.reportParams.drillDownParameters = drillDownParameters;
        this.reportParams.pageNumber = 0;
    }

    this.controls.reportPanel.scrollTop = 0;
}

StiJsViewer.prototype.startRefreshReportTimer = function (timeout) {
    if (this.service.refreshReportTimer != null)
        clearInterval(this.service.refreshReportTimer);

    var jsObject = this;
    this.service.refreshReportTimer = setInterval(function () {
        if (jsObject.options.reportDesignerMode) {
            var viewerContainer = jsObject.controls.viewer ? jsObject.controls.viewer.parentElement : null;
            if (viewerContainer.style.display == "none") {
                clearInterval(jsObject.service.refreshReportTimer);
                return;
            }
        }
        if (!jsObject.service.isRequestInProcess)
            jsObject.postAction("Refresh");
    }, timeout * 1000);
}

StiJsViewer.prototype.stopRefreshReportTimer = function () {
    if (this.service.refreshReportTimer != null) {
        clearInterval(this.service.refreshReportTimer);
        this.service.refreshReportTimer = null;
    }
}

StiJsViewer.prototype.showReportPage = function (data, jsObject) {
    // If report not found, try to get the report again
    if (data == "ServerError:The report is not specified." && jsObject.options.isReportRecieved) {
        jsObject.options.isReportRecieved = false;
        jsObject.reportParams.prevPageNumber = jsObject.reportParams.pageNumber;
        jsObject.postAction("GetReport");
        return;
    }

    // Hide old tooltip
    jsObject.hideDocToolTip();

    jsObject.controls.processImage.hide();
    jsObject.options.isReportRecieved = true;
    jsObject.options.drillDownInProgress = false;
    jsObject.options.interactionInProgress = false;
    jsObject.options.refreshInProgress = false;
    jsObject.options.resizeInProgress = false;

    // Update the intermediate state of the panels
    jsObject.updateVisibleState();

    if (jsObject.showError(data)) return; // Check for error

    if (jsObject.options.server.useCompression) {
        data = StiGZipHelper.unpack(data);
        if (jsObject.showError(data)) return; // Check for error unpacked data
    }

    // Get JSON parameters and check for error in JSON format
    var parameters = (typeof (data) == "string" && data.substr(0, 1) == "{") ? JSON.parse(data) : data;
    if (jsObject.showError(parameters)) return;

    // Set Parameters Orientation from report option
    if ((parameters.action == "GetReport" || parameters.action == "OpenReport") && jsObject.options.appearance.parametersPanelPosition == "FromReport" && parameters.parametersOrientation) {
        jsObject.options.currentParametersPanelPosition = parameters.parametersOrientation == "Horizontal" ? "Top" : "Left";
    }

    if (jsObject.options.jsMode && parameters.action == "InitVars" && (jsObject.reportParams.type == "Dashboard" || jsObject.options.startPageNumber > 0)) {
        jsObject.controls.processImage.show();
    }

    if (parameters.reportDisplayMode)
        jsObject.options.displayModeFromReport = parameters.reportDisplayMode;

    // Get collection of user values
    if (parameters.userValues)
        jsObject.options.userValues = parameters.userValues;

    // Add first report do drill-down panel, show drill-down page
    if (jsObject.controls.drillDownPanel.buttonsRow.children.length == 0)
        jsObject.controls.drillDownPanel.addButton(parameters.reportFileName, jsObject.reportParams);

    if (parameters.action == "DrillDown") {
        var useDbsDrillDownPanel = jsObject.controls.dashboardsPanel && jsObject.controls.dashboardsPanel.visible && jsObject.controls.dashboardsPanel.selectedButton;
        if (useDbsDrillDownPanel) {
            jsObject.controls.dashboardsPanel.addDrillDownButton(null, parameters.drillDownGuid, parameters.drillDownParameters, parameters.previewSettings, parameters.reportFileName);
        }
        jsObject.showDrillDownPage(parameters.reportFileName, parameters.drillDownGuid, parameters.drillDownParameters, useDbsDrillDownPanel);
    }

    // Add button to report do drill-down panel, show drill-down page
    if (parameters.action == "DashboardDrillDown" && jsObject.controls.dashboardsPanel)
        jsObject.controls.dashboardsPanel.addDrillDownButton(parameters.dashboardDrillDownGuid, null, parameters.drillDownParameters, parameters.previewSettings, parameters.reportFileName);

    // Parse report and cloud parameters
    jsObject.parseReportParameters(parameters);
    jsObject.parseCloudParameters(parameters);

    // Add custom & stimulsoft fonts for new report
    if (parameters.action == "GetReport" || parameters.action == "OpenReport") {
        jsObject.addCustomFontStyles(parameters.customFonts);

        if (parameters.stimulsoftFontContent && !jsObject.options.stimulsoftFontContent) {
            jsObject.options.stimulsoftFontContent = parameters.stimulsoftFontContent;
            jsObject.addCustomFontStyles([{
                contentForCss: parameters.stimulsoftFontContent,
                originalFontFamily: "Stimulsoft"
            }]);
        }

        jsObject.options.currentParameterWidth = parameters.parameterWidth;
    }

    // Init viewer panels
    if (parameters.bookmarksContent) jsObject.InitializeBookmarksPanel();
    if (jsObject.controls.resourcesPanel) jsObject.controls.resourcesPanel.update();

    // Fill report panel
    if (parameters.pagesArray) {
        if (parameters.repaintOnlyDashboardContent)
            jsObject.controls.reportPanel.repaintDashboardContent(parameters);
        else
            jsObject.controls.reportPanel.addPages(parameters);
    }

    if ((parameters.action == "GetReport" || parameters.action == "OpenReport" || parameters.action == "RefreshReport") && jsObject.reportParams.type == "Report" && parameters.previewSettings) {
        jsObject.applyPreviewSettingsToViewer(parameters.previewSettings);
    }

    if (jsObject.controls.toolbar) {
        jsObject.controls.toolbar.changeToolBarState();
        jsObject.controls.toolbar.setEnabled(true);
        if (jsObject.controls.navigatePanel) {
            jsObject.controls.navigatePanel.setEnabled(true);
        }
        if (parameters.action == "GetReport" || parameters.action == "OpenReport" || parameters.action == "DrillDown" || parameters.action == "RefreshReport") {
            if (jsObject.controls.buttons.Editor) {
                jsObject.controls.buttons.Editor.style.display = parameters.isEditableReport ? "" : "none";
            }
            if (jsObject.controls.buttons.Signature) {
                jsObject.controls.buttons.Signature.style.display = parameters.isSignedReport ? "" : "none";
            }
            if (!jsObject.options.toolbar.showFindButton && jsObject.controls.toolbar.separators.Separator2_1) {
                jsObject.controls.toolbar.separators.Separator2_1.style.display = jsObject.controls.buttons.Editor.style.display == "" || jsObject.controls.buttons.Signature.style.display == "" ? "" : "none";
            }
        }
    }

    // Check for auto zoom by page width or page height
    jsObject.controls.reportPanel.style.visibility = "";
    if (jsObject.reportParams.autoZoom != null) {
        if (jsObject.reportParams.type == "Report") {
            // Temporarily hide the report panel to prevent display of a report rendered without autozoom
            jsObject.controls.reportPanel.style.visibility = "hidden";

            jsObject.postAction(jsObject.reportParams.autoZoom == -1 ? "ZoomPageWidth" : "ZoomOnePage");
        }

        delete jsObject.reportParams.autoZoom;
    }

    // Go to the bookmark, if it present
    if (jsObject.options.bookmarkAnchor != null) {
        jsObject.scrollToAnchor(jsObject.options.bookmarkAnchor, jsObject.options.componentGuid);
        jsObject.options.bookmarkAnchor = null;
        jsObject.options.componentGuid = null;
    }

    // Find text in the report
    if (jsObject.options.findMode && jsObject.controls.findPanel) jsObject.showFindLabels(jsObject.controls.findPanel.controls.findTextBox.value);

    // Init auto-update report cache
    jsObject.initAutoUpdateCache(null, jsObject);

    // Init auto refresh report timer
    jsObject.stopRefreshReportTimer();
    if (parameters.refreshTime && parseInt(parameters.refreshTime) > 0) jsObject.startRefreshReportTimer(parameters.refreshTime);

    // Update all panels
    jsObject.updateVisibleState();
    jsObject.updateLayout();

    var interactionParams = { action: "InitVars" };

    // Get the request from user variables if drilldown report variables presents
    if (parameters.action == "DrillDown" && parameters.variablesPresentsInReport && parameters.drillDownParameters && parameters.drillDownParameters[parameters.drillDownParameters.length - 1].ReportFile) {
        jsObject.options.isParametersReceived = false;
        interactionParams.drillDownReportFile = true;
    }

    // Get the request from user variables if they are not already received
    if (!jsObject.options.isParametersReceived && ((jsObject.reportParams.type == "Report" && jsObject.options.toolbar.showParametersButton) || jsObject.reportParams.type == "Dashboard")) {
        jsObject.postInteraction(interactionParams);
    }

    // If report contains dashboards, add dashboards buttons and get dashboard page
    if (parameters.dashboards) {
        jsObject.applyPreviewSettingsToDashboardsPanel(parameters.previewSettings);
        jsObject.controls.dashboardsPanel.update(parameters.dashboards, parameters.previewSettings);
        if (!jsObject.controls.menus.saveDashboardMenu) jsObject.InitializeSaveDashboardMenu();
    }

    //back to the scroll position before drilldown action
    if (jsObject.controls.drillDownPanel.selectedButton && jsObject.controls.drillDownPanel.selectedButton.reportScrollPos) {
        jsObject.controls.reportPanel.scrollTop = jsObject.controls.drillDownPanel.selectedButton.reportScrollPos;
        jsObject.controls.drillDownPanel.selectedButton.reportScrollPos = null;
    }

    // Used in cloud viewer
    return parameters;
}
