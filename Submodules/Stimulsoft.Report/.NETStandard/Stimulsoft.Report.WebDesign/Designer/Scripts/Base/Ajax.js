
StiMobileDesigner.prototype.ShowMainLoadProcess = function (command) {
    var commandsWithMainLoadProcess = [
        "CreateDashboard", "CreateReport", "CreateForm", "OpenReport", "SaveReport", "CloseReport", "WizardResult", "GetPreviewPages",
        "GetConnectionTypes", "CreateOrEditConnection", "DeleteConnection", "CreateOrEditRelation", "DeleteRelation", "CreateOrEditColumn", "CreateOrEditRelation",
        "DeleteRelation", "CreateOrEditColumn", "DeleteColumn", "CreateOrEditDataSource", "DeleteDataSource", "GetAllConnections", "RetrieveColumns", "SynchronizeDictionary",
        "CreateOrEditBusinessObject", "DeleteBusinessObject", "UpdateStyles", "AddStyle", "GetReportFromData", "ItemResourceSave", "StartEditChartComponent", "AddSeries", "RemoveSeries",
        "SeriesMove", "SetLabelsType", "AddConstantLineOrStrip", "RemoveConstantLineOrStrip", "ConstantLineOrStripMove", "SendContainerValue", "CreateTextComponent", "CreateDataComponent",
        "RunQueryScript", "ApplyDesignerOptions", "StartEditCrossTabComponent", "UpdateCrossTabComponent", "Undo", "Redo", "GetReportForDesigner",
        "OpenStyle", "SaveStyle", "GetGlobalizationStrings", "SetCultureSettingsToReport", "GetCultureSettingsFromReport", "StartEditGaugeComponent", "UpdateGaugeComponent",
        "DeleteResource", "LoadReportFromCloud", "MoveDictionaryItem", "GetVariableItemsFromDataColumn", "TestConnection", "StartEditMapComponent",
        "ApplySelectedData", "MoveConnectionDataToResource", "StartEditBarCodeComponent", "CreateTableElement", "CreateTextElement", "UpdateTableElement", "UpdatePivotTableElement", "UpdateChartElement",
        "UpdateGaugeElement", "UpdateTextElement", "UpdateMapElement", "UpdateShapeElement", "UpdateProgressElement", "UpdateIndicatorElement", "UpdateImageElement", "ChangeDashboardStyle", "OpenDictionary",
        "OpenWizardDashboard", "ChangeTypeElement", "ChangeDashboardViewMode", "GetMobileViewUnplacedElements", "OpenWizardReport", "PrepareReportBeforeGetData", "ChangeReportType", "RestoreOldReport",
        "CreateDatePickerElement", "CreateComboBoxElement", "GetSpecialSymbols", "EmbedAllDataToResources", "UpdateChart", "GetAzureBlobStorageContainerNamesItems", "GetAzureBlobStorageBlobNameItems",
        "GetMathFormulaInfo", "AddDemoDataToReport", "GetGoogleAnalyticsParameters", "GetStylesContentByType"
    ];

    return (commandsWithMainLoadProcess.indexOf(command) != -1);
}

StiMobileDesigner.prototype.ShowDictionaryLoadProcess = function (command) {
    var commandsWithDictionaryLoadProcess = ["CreateDatabaseFromResource", "CreateOrEditResource"];

    return (commandsWithDictionaryLoadProcess.indexOf(command) != -1);
}

StiMobileDesigner.prototype.IgnoreLoadProcess = function (command) {
    var commandsIgnoreLoadProcess = ["GetGoogleAuthorizationResult", "UpdateImagesArray", "EncryptMachineName"];

    return (commandsIgnoreLoadProcess.indexOf(command) != -1);
}

StiMobileDesigner.prototype.CreateXMLHttp = function () {
    if (typeof XMLHttpRequest != "undefined") return new XMLHttpRequest();
    else if (window.ActiveXObject) {
        var allVersions = [
            "MSXML2.XMLHttp.5.0",
            "MSXML2.XMLHttp.4.0",
            "MSXML2.XMLHttp.3.0",
            "MSXML2.XMLHttp",
            "Microsoft.XMLHttp"
        ];
        for (var i = 0; i < allVersions.length; i++) {
            try {
                // eslint-disable-next-line no-undef
                var xmlHttp = new ActiveXObject(allVersions[i]);
                return xmlHttp;
            }
            catch (oError) {
            }
        }
    }
    throw new Error("Unable to create XMLHttp object.");
}

StiMobileDesigner.prototype.OpenConnection = function (http, url, contentType, requestType, responseType) {
    http.open(requestType, url);

    http.setRequestHeader("Content-Type", contentType);
    if (this.options.requestToken)
        http.setRequestHeader("RequestVerificationToken", this.options.requestToken);

    http.responseType = responseType;
}

StiMobileDesigner.prototype.CreatePostParameters = function (data, asObject) {
    var params = {
        "designerId": this.options.mobileDesignerId,
        "reportGuid": this.options.reportGuid,
        "clientGuid": this.options.clientGuid,
        "cloudMode": this.options.cloudMode,
        "serverMode": this.options.serverMode,
        "cacheMode": this.options.cacheMode,
        "cacheTimeout": this.options.cacheTimeout,
        "cacheItemPriority": this.options.cacheItemPriority,
        "undoMaxLevel": this.options.undoMaxLevel,
        "useRelativeUrls": this.options.useRelativeUrls,
        "passQueryParametersForResources": this.options.passQueryParametersForResources,
        "routes": this.options.routes,
        "formValues": this.options.formValues,
        "version": this.options.shortProductVersion,
        "useAliases": this.options.useAliases,
        "checkReportBeforePreview": this.options.checkReportBeforePreview,
        "currentCultureName": this.options.cultureName,
        "newReportDictionary": this.options.newReportDictionary,
        "isAngular": this.options.isAngular,
        "useCacheHelper": this.options.useCacheHelper,
        "allowAutoUpdateCookies": this.options.allowAutoUpdateCookies
    };

    if ((this.options.cacheMode == "StringCache" || this.options.cacheMode == "StringSession" || this.options.useCacheHelper) && !this.options.jsMode) {
        params.designerOptions = this.GetCookie("StimulsoftMobileDesignerOptions");
    }

    //Set plan for cloud mode
    if (this.options.cloudMode) {
        params.cp = this.GetCloudPlanNumberValue();
    }

    if (this.options.report) {
        params.reportFile = this.options.report.properties.reportFile;
    }

    // Add data object fields to params
    if (data) {
        for (var key in data) {
            params[key] = data[key];
        }
    }

    // Add cloud parameters
    if (this.options.cloudParameters) {
        params.cloudParameters = this.options.cloudParameters;
    }

    // Object params
    var postParams = {
        stiweb_component: "Designer",
        stiweb_action: "RunCommand"
    };
    if (params.action) {
        postParams["stiweb_action"] = params.action;
        delete params.action;
    }
    if (params.base64Data) {
        postParams["stiweb_data"] = params.base64Data;
        delete params.base64Data;
    }

    // Params
    var jsonParams = JSON.stringify(params);
    if (this.options.useCompression) postParams["stiweb_packed_parameters"] = StiGZipHelper.pack(jsonParams);
    else postParams["stiweb_parameters"] = StiBase64.encode(jsonParams);
    if (this.options.requestToken) postParams["__RequestVerificationToken"] = this.options.requestToken;
    if (asObject) return postParams;

    // URL params
    var urlParams = "stiweb_component=" + postParams["stiweb_component"] + "&";
    if (postParams["stiweb_action"]) urlParams += "stiweb_action=" + postParams["stiweb_action"] + "&";
    if (postParams["stiweb_data"]) urlParams += "stiweb_data=" + encodeURIComponent(postParams["stiweb_data"]) + "&";
    if (postParams["stiweb_parameters"]) urlParams += "stiweb_parameters=" + encodeURIComponent(postParams["stiweb_parameters"]);
    else urlParams += "stiweb_packed_parameters=" + encodeURIComponent(postParams["stiweb_packed_parameters"]);
    if (this.options.requestToken) urlParams += "&__RequestVerificationToken=" + this.options.requestToken;

    return urlParams;
}

StiMobileDesigner.prototype.GetMvcActionUrl = function (url, data) {
    switch (data.command) {
        case "GetReportForDesigner": return url.replace("{action}", this.options.actions.getReport || this.options.actions.designerEvent);
        case "OpenReport": return url.replace("{action}", this.options.actions.openReport || this.options.actions.designerEvent);
        case "CreateDashboard":
        case "CreateReport":
        case "CreateForm":
        case "WizardResult": return url.replace("{action}", this.options.actions.createReport || this.options.actions.designerEvent);
        case "SaveReport": return url.replace("{action}", this.options.actions.saveReport || this.options.actions.designerEvent);
        case "SaveAsReport": return url.replace("{action}", this.options.actions.saveReportAs || this.options.actions.designerEvent);
        case "LoadReportToViewer": return url.replace("{action}", this.options.actions.previewReport || this.options.actions.designerEvent);
        case "ExitDesigner": return url.replace("{action}", this.options.actions.exit || this.options.actions.designerEvent);
        case "SetLocalization": return url.replace("{action}", (this.options.routes["action"] || ""));
    }
    return url.replace("{action}", this.options.actions.designerEvent);
}

StiMobileDesigner.prototype.PostAjax = function (url, data, callback, requestType) {
    var jsObject = this;
    var xmlHttp = this.xmlHttp = this.CreateXMLHttp();
    this.xmlHttpAbortedByUser = false;
    if (this.options.actions) url = this.GetMvcActionUrl(url, data);

    if (jsObject.options.requestTimeout != 0) {
        var requestTimeout = this.CheckRequestTimeout(data);

        setTimeout(function () {
            if (xmlHttp.readyState < 4) xmlHttp.abort();
        }, (requestTimeout || jsObject.options.requestTimeout) * 1000);
    }

    this.OpenConnection(
        xmlHttp, url,
        this.options.requestHeaderContentType || "application/x-www-form-urlencoded",
        requestType || "POST",
        data && data.responseType ? data && data.responseType : "text");

    xmlHttp.onreadystatechange = function () {
        if (xmlHttp.readyState == 4) {
            var status = 0;
            try {
                status = xmlHttp.status;
            }
            catch (e) {
            }

            if (status == 0) {
                callback("ServerError:Timeout response from the server.", jsObject);
            } else if (status == 200) {
                callback(xmlHttp.response ? xmlHttp.response : xmlHttp.responseText, jsObject);
            } else {
                // Try to display error message from server
                if (xmlHttp.responseText && xmlHttp.responseText.substr(0, 12) == "ServerError:") callback(xmlHttp.responseText, jsObject);
                // Display HTTP request error code and status
                else callback("ServerError:" + status + " - " + xmlHttp.statusText, jsObject);
            }
        }
    };

    var params = this.CreatePostParameters(data, false);
    xmlHttp.id = this.options.mobileDesignerId;
    xmlHttp.send(params);
}

StiMobileDesigner.prototype.PostForm = function (data, doc, url, postOnlyData, target) {
    if (!doc) doc = document;
    url = url || this.options.requestUrl;
    if (this.options.actions) url = this.GetMvcActionUrl(url, data);

    var form = doc.createElement("FORM");
    form.setAttribute("method", "POST");
    form.setAttribute("action", url);

    var params = postOnlyData ? data : this.CreatePostParameters(data, true);
    if (this.options.requestToken)
        params["__RequestVerificationToken"] = this.options.requestToken;

    for (var key in params) {
        var hiddenField = doc.createElement("INPUT");
        hiddenField.setAttribute("type", "hidden");
        hiddenField.setAttribute("name", key);
        hiddenField.setAttribute("value", params[key]);
        form.appendChild(hiddenField);
    }

    doc.body.appendChild(form);
    form.submit();
    doc.body.removeChild(form);
}

StiMobileDesigner.prototype.AddCommandToStack = function (params, commandGuid) {
    params.commandGuid = commandGuid || this.generateKey();
    this.options.commands.push(params);
    if (this.options.commands.length == 1) this.ExecuteCommandFromStack();
}

StiMobileDesigner.prototype.ExecuteCommandFromStack = function () {
    if (this.options.commands.length == 0) return;

    var jsObject = this;
    var params = this.options.commands[0];

    if (!this.IgnoreLoadProcess(params.command)) {
        if (this.ShowMainLoadProcess(params.command)) {
            var processImage = this.options.processImage || this.InitializeProcessImage();
            processImage.show(params.progress);
        }
        else if (this.ShowDictionaryLoadProcess(params.command) && this.options.dictionaryPanel) {
            if (this.options.menus.fileMenu && this.options.menus.fileMenu.visible) {
                (this.options.processImage || this.InitializeProcessImage()).show();
            }
            else {
                this.options.dictionaryPanel.showProgress(null, params.commandGuid);
            }
        }
        else if (this.options.processImageStatusPanel) {
            this.options.processImageStatusPanel.show();
        }
    }

    this.PostAjax(this.options.requestUrl, params, this.receveFromServer);

    if (params.isAsyncCommand) {
        this.options.commands.splice(0, 1);

        if (this.options.commands.length > 0) {
            this.ExecuteCommandFromStack();
        }
    }

    clearTimeout(this.options.timerAjax);

    if (!this.options.jsMode) {
        this.options.timerAjax = setTimeout(function () {
            jsObject.Synchronization();
        }, params.command != "Synchronization" ? Math.max(this.options.requestTimeout, 120000) : 15000);
    }
}

StiMobileDesigner.prototype.Synchronization = function () {
    this.options.commands = [];
    this.SendCommandSynchronization();
}

//Fixed bugs with some symblos
StiMobileDesigner.prototype.EncodeSymbols = function (str) {
    str = str.replace(/'/g, '\\\''); // (')
    str = str.replace(/\\\"/g, '&ldquo;'); // (\")
    return str;
}

//Error
StiMobileDesigner.prototype.errorFromServer = function (args, jsObject) {
}

//Receive
StiMobileDesigner.prototype.receveFromServer = function (args, jsObject) {
    if (!jsObject && this.options) jsObject = this;
    var answer = { command: null };

    try {
        if (jsObject.options.useCompression) args = StiGZipHelper.unpack(args);
        answer = JSON.parse(args);
    }
    catch (e) {
        var errorMessage = "An error occurred while parsing the response from the server.";

        if (typeof args != "string" || args == "" || args == "ServerError:")
            errorMessage = "An unknown error occurred (the server returned an empty value).";
        else if (args.substr(0, 12) == "ServerError:")
            errorMessage = args.substr(12);

        answer = { error: errorMessage };
    }

    clearTimeout(jsObject.options.timerAjax);
    var messageText = answer.error || answer.infoMessage || answer.warningMessage;

    if (jsObject.options.cloudMode) {
        jsObject.CheckCloudNotifications(answer);
    }

    if (messageText) {
        if (!jsObject.xmlHttpAbortedByUser && answer.command != "UpdateCache") {
            if (jsObject.options.images) {
                var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                var messageType = answer.infoMessage ? "Info" : (answer.warningMessage ? "Warning" : "Error");
                errorMessageForm.show(messageText, messageType);
            }
            else {
                alert(messageText);
            }
        }

        if (jsObject.options.processImage) jsObject.options.processImage.hide();
        if (jsObject.options.processImageStatusPanel) jsObject.options.processImageStatusPanel.hide();
        if (jsObject.options.dictionaryPanel) jsObject.options.dictionaryPanel.hideProgress();
        if (jsObject.options.viewer) (jsObject.options.viewer.jsObject.controls || jsObject.options.viewer.jsObject.options).processImage.hide();

        if (jsObject.options.buttons.reportCheckerButton) {
            jsObject.options.buttons.reportCheckerButton.updateCaption(answer.checkItems);
        }

        if (answer.checkItems && answer.checkItems.length > 0) {
            for (var i = 0; i < answer.checkItems.length; i++) {
                if (answer.checkItems[i].status == "Error") {
                    jsObject.InitializeReportCheckForm(function (reportCheckForm) {
                        reportCheckForm.show(answer.checkItems);
                    });
                    break;
                }
            }
        }

        // If an error occurs, clear file name for the new report
        if (answer.isNewReport && jsObject.options.report)
            jsObject.options.report.properties.reportFile = null;
    }
    else {
        switch (answer.command) {
            case "SessionCompleted":
                {
                    var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                    errorMessageForm.show("Your session has expired!", "Warning");
                    break;
                }
            case "SynchronizationError":
                {
                    var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                    errorMessageForm.onAction = function () {
                        jsObject.options.ignoreBeforeUnload = true;
                        location.reload(true);
                    }
                    errorMessageForm.show("The website is temporarily unavailable. Updating is in progress...");
                    break;
                }
            case "Synchronization":
                {
                    jsObject.LoadReport(jsObject.ParseReport(answer.reportObject));
                    break;
                }
            case "CreateReport":
            case "CreateDashboard":
                {
                    if (jsObject.options.formsDesignerFrame) jsObject.options.formsDesignerFrame.close();
                    if (answer.needClearAfterOldReport) jsObject.CloseReport();
                    if (jsObject.options.paintPanel.copyStyleMode) jsObject.options.paintPanel.setCopyStyleMode(false);
                    if (jsObject.options.cloudParameters) jsObject.options.cloudParameters.reportTemplateItemKey = null;
                    if (jsObject.options.jsMode) jsObject.undoState = jsObject.redoState = null;
                    jsObject.options.reportGuid = answer.reportGuid;
                    jsObject.LoadReport(jsObject.ParseReport(answer.reportObject));
                    if (jsObject.options.callbackFunctions[answer["callbackFunctionId"]]) {
                        jsObject.options.callbackFunctions[answer["callbackFunctionId"]](answer);
                        delete jsObject.options.callbackFunctions[answer["callbackFunctionId"]];
                    }
                    
                    break;
                }
            case "CreateForm":
                {
                    jsObject.CloseReport();
                    if (!jsObject.options.formsDesignerFrame) {
                        answer.loadingCompleted = false;
                    }
                    jsObject.InitializeFormsDesignerFrame(function (frame) {
                        frame.show();
                        frame.formName = "Form";
                        frame.sendCommand({ action: "createForm" });
                    });
                    jsObject.SetWindowTitle("Form - " + jsObject.loc.FormDesigner.title);
                    break;
                }
            case "GetReportFromData":
            case "OpenReport":
            case "OpenWizardDashboard":
            case "OpenWizardReport":
                {
                    if (jsObject.options.cloudMode && (answer.maxResourcesExceeded || answer.maxResourceSizeExceeded)) {
                        var message = answer.maxResourcesExceeded
                            ? jsObject.loc.Notices.QuotaMaximumResourcesCountExceeded + "<br>" + jsObject.loc.PropertyMain.Maximum + ": " + answer.maxResourcesExceeded
                            : jsObject.loc.Notices.QuotaMaximumResourceSizeExceeded + "<br>" + jsObject.loc.PropertyMain.Maximum + ": " + jsObject.GetHumanFileSize(answer.maxResourceSizeExceeded, true);

                        jsObject.InitializeNotificationForm(function (form) {
                            form.show(message, jsObject.NotificationMessages("upgradeYourPlan"), "Notifications.Files.png");
                        });
                    }
                    else if (answer.formContent) {
                        if (!jsObject.options.formsDesignerFrame) {
                            answer.loadingCompleted = false;
                        }
                        if (answer.command == "OpenReport" && jsObject.options.cloudParameters) {
                            jsObject.options.cloudParameters.reportTemplateItemKey = null;
                        }
                        jsObject.InitializeFormsDesignerFrame(function (frame) {
                            frame.openForm(answer.formName, answer.formContent);
                        });
                    }
                    else if (!(answer.commandGuid && jsObject.options.abortedCommands[answer.commandGuid])) {
                        if (answer.loadingCompleted !== false) {
                            if (jsObject.options.formsDesignerFrame) {
                                jsObject.options.formsDesignerFrame.close();
                            }
                            jsObject.CloseReport();

                            if (answer["errorMessage"] != null) {
                                var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                                errorMessageForm.show(answer["errorMessage"]);
                            }
                            else {
                                if (answer.reportGuid == null && answer.reportObject == null) {
                                    var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                                    errorMessageForm.show(jsObject.loc.Errors.Error);
                                }
                                else {
                                    if (answer.command == "OpenReport" && jsObject.options.cloudParameters) {
                                        jsObject.options.cloudParameters.reportTemplateItemKey = null;
                                    }
                                    jsObject.options.reportGuid = answer.reportGuid;
                                    var reportObject = jsObject.ParseReport(answer.reportObject);
                                    reportObject.encryptedPassword = answer.encryptedPassword;
                                    jsObject.LoadReport(reportObject);
                                    if (jsObject.options.report && answer.fileSize) {
                                        jsObject.options.report.fileSize = answer.fileSize;
                                    }
                                }
                            }
                        }
                        else {
                            (jsObject.options.processImage || jsObject.InitializeProcessImage()).show(answer.progress, answer.commandGuid);
                        }                        
                    }
                    if (jsObject.options.paintPanel.copyStyleMode) jsObject.options.paintPanel.setCopyStyleMode(false);                    
                    break;
                }
            case "ItemResourceSave":
                {
                    if (answer["openSaveAsDialog"]) {
                        var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
                        fileMenu.changeVisibleState(true);
                        jsObject.ExecuteAction("saveAsReport");
                    }
                    else if (answer["errorMessage"] != null) {
                        var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                        errorMessageForm.show(answer["errorMessage"]);
                    }
                    else {
                        jsObject.options.reportIsModified = false;
                        if (jsObject.options.forms.shareForm && jsObject.options.forms.shareForm.visible) {
                            jsObject.options.forms.shareForm.fillShareInfo();
                        };
                    }
                    if (jsObject.options.formsDesignerMode && jsObject.options.formsDesignerFrame) {
                        jsObject.options.formsDesignerFrame.show();
                    }
                    break;
                }
            case "CloneItemResourceSave":
                {
                    if (answer["errorMessage"] != null) {
                        var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                        errorMessageForm.show(answer["errorMessage"]);
                    } else {
                        if (answer["resultItemKey"]) {
                            var navigatorUrl = jsObject.options.cloudParameters.navigatorUrl;
                            var viewerParameters = jsObject.CopyObject(jsObject.options.cloudParameters);
                            viewerParameters["reportName"] = StiBase64.encode(answer["reportName"]);
                            viewerParameters["reportTemplateItemKey"] = answer["resultItemKey"];
                            if (viewerParameters["attachedItems"]) delete viewerParameters["attachedItems"];
                            delete viewerParameters["navigatorUrl"];
                            var viewerHref = navigatorUrl.replace("main.aspx", "mobileviewer");

                            if (!jsObject.options.viewerContainer) jsObject.InitializePreviewPanel();
                            jsObject.options.viewerContainer.addFrame();
                            jsObject.options.viewerContainer.changeVisibleState(true);

                            var previewFrame = jsObject.options.viewerContainer.frame;
                            previewFrame.src = "about:blank";
                            var loadFlag = false;

                            previewFrame.onload = function () {
                                if (!loadFlag) {
                                    if (jsObject.options.buttons.previewToolButton) jsObject.options.buttons.previewToolButton.progress.style.visibility = "hidden";
                                    loadFlag = true;
                                    var doc = this.contentDocument || this.document;
                                    jsObject.options.ignoreBeforeUnload = true;
                                    jsObject.PostForm(viewerParameters, doc, viewerHref, true);
                                    setTimeout(function () { jsObject.options.ignoreBeforeUnload = false; }, 500);
                                }
                            }
                        }
                    }
                    break;
                }
            case "CloseReport":
                {
                    jsObject.CloseReport();
                    if (jsObject.options.formsDesignerFrame) {
                        jsObject.options.formsDesignerFrame.close();
                    }
                    break;
                }
            case "SaveReport":
            case "SaveAsReport":
                {
                    if (answer.infoMessage || answer.errorMessage || answer.warningMessage) {
                        var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                        var messageType = answer.infoMessage ? "Info" : (answer.warningMessage ? "Warning" : "Error");
                        errorMessageForm.show(answer.infoMessage || answer.errorMessage || answer.warningMessage, messageType);
                    }
                    break;
                }
            case "MoveComponent":
            case "ResizeComponent":
                {
                    var componentsNames = [];
                    var firstComponent;

                    for (var i = 0; i < answer.components.length; i++) {
                        var component = jsObject.options.report.pages[answer.pageName].components[answer.components[i].componentName];
                        if (!component) continue;

                        if (i == 0) firstComponent = component;
                        componentsNames.push(answer.components[i].componentName);

                        if (answer.components[i].svgContent != null)
                            component.properties.svgContent = answer.components[i].svgContent;

                        component.repaint();

                        if (component.typeComponent == "StiSubReport" && jsObject.options.report.pages[component.properties.subReportPage])
                            jsObject.SendCommandSendProperties(jsObject.options.report.pages[component.properties.subReportPage], []);

                        if (component.typeComponent == "StiShape" && component.shapeSvgContent) {
                            component.shapeSvgContent.removeAttribute("viewBox");
                            component.shapeSvgContent.removeAttribute("preserveAspectRatio");
                            component.shapeSvgContent = null;
                        }
                    }
                    jsObject.CheckLargeHeight(jsObject.options.report.pages[answer.pageName], answer.largeHeightAutoFactor);
                    jsObject.options.report.pages[answer.pageName].rebuild(answer.rebuildProps);
                    jsObject.PaintSelectedLines();

                    if (answer.components.length == 1 || (answer.command == "ResizeComponent" && !answer.isMultiResize)) {
                        if (firstComponent) {
                            if (firstComponent.typeComponent == "StiTable" && answer.cells) {
                                jsObject.RebuildTable(firstComponent, answer.cells);
                            }

                            firstComponent.setSelected();
                            firstComponent.setOnTopLevel();
                        }
                    }
                    else {
                        jsObject.SetSelectedObjectsByNames(jsObject.options.report.pages[answer.pageName], componentsNames);
                    }

                    jsObject.UpdatePropertiesControls();
                    jsObject.UpdateStateUndoRedoButtons();

                    if (jsObject.options.reportTree)
                        jsObject.options.reportTree.build();

                    if (jsObject.options.forms.mobileViewComponentsForm && jsObject.options.forms.mobileViewComponentsForm.visible && answer.isUnplacedComponent &&
                        jsObject.options.currentPage && jsObject.options.currentPage.properties.dashboardViewMode == "Mobile") {
                        jsObject.SendCommandToDesignerServer("GetMobileViewUnplacedElements", { dashboardName: jsObject.options.currentPage.properties.name }, function (answer) {
                            if (answer.elements && answer.elements.length > 0) {
                                jsObject.options.forms.mobileViewComponentsForm.show(answer.elements);
                            }
                        });
                    }
                    break;
                }
            case "CreateComponent":
                {
                    var component = answer.properties.isDashboardElement
                        ? jsObject.CreateDashboardElement(answer)
                        : jsObject.CreateComponent(answer);

                    if (component) {
                        component.repaint();
                        jsObject.CheckLargeHeight(jsObject.options.report.pages[component.properties.pageName], answer.largeHeightAutoFactor);
                        jsObject.options.report.pages[component.properties.pageName].appendChild(component);
                        jsObject.options.report.pages[component.properties.pageName].components[component.properties.name] = component;
                        jsObject.options.report.pages[component.properties.pageName].rebuild(answer.rebuildProps);
                        component.setOnTopLevel();
                        component.setSelected();

                        if (answer.tableCells) {
                            for (var i = 0; i < answer.tableCells.length; i++) {
                                var cell = jsObject.CreateComponent(answer.tableCells[i]);
                                if (cell) {
                                    cell.repaint();
                                    jsObject.options.report.pages[cell.properties.pageName].appendChild(cell);
                                    jsObject.options.report.pages[cell.properties.pageName].components[cell.properties.name] = cell;
                                }
                            }
                        }

                        if (answer.newSubReportPage)
                            jsObject.AddPage(answer.newSubReportPage, true);

                        jsObject.UpdatePropertiesControls();

                        if (jsObject.options.report.info.runDesignerAfterInsert)
                            jsObject.ShowComponentForm(component, answer.additionalParams);

                        jsObject.UpdateStateUndoRedoButtons();

                        if (jsObject.options.reportTree)
                            jsObject.options.reportTree.build();
                    }
                    break;
                }
            case "RemoveComponent":
                {
                    if (answer.rebuildProps && answer.pageName) {
                        jsObject.options.report.pages[answer.pageName].rebuild(answer.rebuildProps);
                        jsObject.UpdateStateUndoRedoButtons();
                        jsObject.CheckLargeHeight(jsObject.options.report.pages[answer.pageName], answer.largeHeightAutoFactor);
                        if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                    }
                    break;
                }
            case "AddPage":
                {
                    jsObject.AddPage(answer);
                    jsObject.UpdateStateUndoRedoButtons();
                    if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                    break;
                }
            case "OpenPage":
                {
                    var newPage = answer.pageProps.properties.isDashboard
                        ? jsObject.AddDashboard(answer.pageProps)
                        : jsObject.AddPage(answer.pageProps);

                    jsObject.UpdateStateUndoRedoButtons();

                    var components = answer.pageProps.components;

                    for (var numComponent = 0; numComponent < components.length; numComponent++) {
                        if (ComponentCollection[components[numComponent].typeComponent]) {
                            var component = components[numComponent].properties.isDashboardElement
                                ? jsObject.CreateDashboardElement(components[numComponent])
                                : jsObject.CreateComponent(components[numComponent]);

                            if (component) {
                                component.repaint();
                                jsObject.options.report.pages[newPage.properties.name].components[component.properties.name] = component;
                            }
                        }
                    }

                    newPage.addComponents();

                    if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                    break;
                }
            case "AddDashboard":
                {
                    jsObject.AddDashboard(answer);
                    jsObject.UpdateStateUndoRedoButtons();
                    if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                    break;
                }
            case "RemovePage":
                {
                    jsObject.UpdateStateUndoRedoButtons();
                    if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                    break;
                }
            case "SendProperties":
                {
                    var checkLargeHeight = false;
                    var components = answer.components;
                    var lastComponent;

                    for (var i = 0; i < components.length; i++) {
                        var object = components[i].typeComponent == "StiPage"
                            ? jsObject.options.report.pages[components[i].componentName]
                            : jsObject.options.report.pages[answer.pageName].components[components[i].componentName];
                        if (!object) continue;

                        jsObject.WriteAllProperties(object, components[i].properties);

                        if (object.typeComponent == "StiPage") {
                            object.repaint(true);
                            object.rebuild(answer.rebuildProps);
                            object.repaintAllComponents();
                            jsObject.options.pagesPanel.pagesContainer.updatePages();
                        }
                        else {
                            object.properties.svgContent = components[i].svgContent;
                            object.repaint();
                            checkLargeHeight = true;
                            lastComponent = object;
                        }
                    }
                    if (lastComponent && !lastComponent.isDashboard && !lastComponent.isDashboardElement && jsObject.options.updateLastStyleProperties) {
                        jsObject.SaveLastStyleProperties(lastComponent);
                    }

                    if (checkLargeHeight && answer.pageName && answer.largeHeightAutoFactor) {
                        jsObject.CheckLargeHeight(jsObject.options.report.pages[answer.pageName], answer.largeHeightAutoFactor);
                    }
                    if (answer.rebuildProps) {
                        jsObject.options.report.pages[answer.pageName].rebuild(answer.rebuildProps);
                        var selectedObjects = jsObject.options.selectedObjects || [jsObject.options.selectedObject];
                        if (selectedObjects) {
                            for (var i = 0; i < selectedObjects.length; i++) {
                                if (selectedObjects[i].typeComponent != "StiPage" && selectedObjects[i].typeComponent != "StiReport") selectedObjects[i].setOnTopLevel();
                            }
                        }
                    }

                    jsObject.UpdatePropertiesControls();
                    jsObject.UpdateStateUndoRedoButtons();
                    jsObject.options.updateLastStyleProperties = false;
                    break;
                }
            case "ChangeUnit":
                {
                    jsObject.options.report.properties.reportUnit = answer.reportUnit;
                    jsObject.options.buttons.unitButton.updateCaption(answer.reportUnit);
                    jsObject.options.report.gridSize = jsObject.ConvertUnitToPixel(jsObject.StrToDouble(answer.gridSize));
                    jsObject.ConvertAllComponentsToCurrentUnit(answer.pagePositions, answer.compPositions);
                    if (jsObject.options.selectedObject) jsObject.options.selectedObject.setSelected();
                    jsObject.UpdatePropertiesControls();
                    jsObject.UpdateStateUndoRedoButtons();
                    break;
                }
            case "RebuildPage":
                {
                    jsObject.options.report.pages[answer.pageName].rebuild(answer.rebuildProps);
                    break;
                }
            case "LoadReportToViewer":
                {
                    var stopViewer = false;

                    if (jsObject.options.buttons.reportCheckerButton) {
                        jsObject.options.buttons.reportCheckerButton.updateCaption(answer.checkItems);
                    }

                    if (answer.checkItems && answer.checkItems.length > 0) {
                        for (var i = 0; i < answer.checkItems.length; i++) {
                            if (answer.checkItems[i].status == "Error") {
                                jsObject.InitializeReportCheckForm(function (reportCheckForm) {
                                    reportCheckForm.show(answer.checkItems);
                                });
                                stopViewer = true;
                                break;
                            }
                        }
                    }

                    if (!stopViewer) {
                        var params = {};
                        params.pageNumber = 0;
                        params.zoom = (jsObject.options.viewer.jsObject.reportParams || jsObject.options.viewer.jsObject.options).zoom || 100;
                        params.viewmode = (jsObject.options.viewer.jsObject.reportParams ? jsObject.options.viewer.jsObject.reportParams.viewMode : jsObject.options.viewer.jsObject.options.menuViewMode) || "OnePage";

                        jsObject.options.viewer.jsObject.options.startPageNumber = jsObject.options.report && jsObject.options.report.dashboardsPresent() && jsObject.options.currentPage.properties.enabled
                            ? parseInt(jsObject.options.currentPage.properties.pageIndex)
                            : null;

                        if (typeof jsObject.options.viewer.jsObject.postAction != 'undefined') {
                            jsObject.options.viewer.jsObject.postAction();
                        }
                        else
                            jsObject.options.viewer.jsObject.sendToServer("LoadReportFromCache", params);
                    }
                    break;
                }
            case "SetToClipboard":
                {
                    jsObject.options.buttons.pasteComponent.setEnabled(true);
                    if (jsObject.options.isTouchDevice) {
                        var selectedObject = jsObject.options.selectedObject;
                        if (selectedObject && selectedObject.typeComponent != "StiPage" && selectedObject.typeComponent != "StiReport") {
                            jsObject.ChangeVisibilityStateResizingIcons(selectedObject, true);
                        }
                    }
                    if (answer.clipboardResult) {
                        jsObject.copyTextToClipboard(answer.clipboardResult);
                    }
                    break;
                }
            case "GetFromClipboard":
                {
                    var components = answer["components"];
                    if (components && components.length > 0) {
                        var containTable = false;
                        jsObject.options.clipboardMode = true;
                        for (var i = 0; i < components.length; i++) {
                            var component = components[i].properties.isDashboardElement
                                ? jsObject.CreateDashboardElement(components[i])
                                : jsObject.CreateComponent(components[i]);
                            if (component) {
                                if (component.typeComponent == "StiTable") containTable = true;
                                component.repaint();
                                jsObject.options.report.pages[component.properties.pageName].appendChild(component);
                                jsObject.options.report.pages[component.properties.pageName].components[component.properties.name] = component;
                                if (!jsObject.isTouchDevice) {
                                    if (i == 0) {
                                        jsObject.options.in_drag = [[], [], [], [], true];
                                        var pagePositions = jsObject.FindPagePositions();
                                        jsObject.options.startMousePos = [
                                            pagePositions.posX + parseInt(component.getAttribute("left")) - 3,
                                            pagePositions.posY + parseInt(component.getAttribute("top")) - 3
                                        ];
                                    }
                                    jsObject.options.in_drag[0].push(component);
                                    jsObject.options.in_drag[1].push(parseInt(component.getAttribute("left")));
                                    jsObject.options.in_drag[2].push(parseInt(component.getAttribute("top")));
                                    jsObject.options.in_drag[3].push(component.getAllChildsComponents());
                                }
                            }
                        }
                        if (jsObject.isTouchDevice && answer.rebuildProps)
                            jsObject.options.report.pages[component.properties.pageName].rebuild(answer.rebuildProps);
                        jsObject.UpdateStateUndoRedoButtons();
                        if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                        if (!jsObject.options.mouseOverPage || containTable) {
                            jsObject.PasteCurrentClipboardComponent();
                        }
                    }
                    break;
                }
            case "Undo":
                {
                    if (answer["reportGuid"] && answer["reportObject"]) {
                        jsObject.oldScrollTopPaintPanel = jsObject.options.paintPanel.scrollTop;
                        jsObject.options.reportGuid = answer.reportGuid;
                        jsObject.LoadReport(jsObject.ParseReport(answer.reportObject), true);
                        jsObject.options.reportIsModified = true;
                        jsObject.options.buttons.undoButton.setEnabled(answer.enabledUndoButton);
                        jsObject.options.buttons.redoButton.setEnabled(true);
                        jsObject.options.paintPanel.scrollTop = jsObject.oldScrollTopPaintPanel;
                        jsObject.undoState = answer.enabledUndoButton;
                        jsObject.redoState = true;
                    }
                    if (answer.selectedObjectName)
                        jsObject.BackToSelectedComponent(answer.selectedObjectName);
                    break;
                }
            case "Redo":
                {
                    if (answer["reportGuid"] && answer["reportObject"]) {
                        jsObject.options.reportGuid = answer.reportGuid;
                        jsObject.LoadReport(jsObject.ParseReport(answer.reportObject), true);
                        jsObject.options.reportIsModified = true;
                    }
                    jsObject.options.buttons.redoButton.setEnabled(answer.enabledRedoButton);
                    jsObject.options.buttons.undoButton.setEnabled(true);
                    jsObject.undoState = true;
                    jsObject.redoState = answer.enabledRedoButton;

                    if (answer.selectedObjectName)
                        jsObject.BackToSelectedComponent(answer.selectedObjectName);
                    break;
                }
            case "RenameComponent":
                {
                    if (answer.newName != answer.oldName) {
                        if (answer.typeComponent == "StiPage") {
                            var page = jsObject.options.report.pages[answer.oldName];
                            if (page) page.rename(answer.newName);
                        }
                        else {
                            var component = jsObject.FindComponentByName(answer.oldName);
                            if (component) component.rename(answer.newName);
                            jsObject.options.report.pages[component.properties.pageName].rebuild(answer.rebuildProps);
                        }
                    }
                    jsObject.UpdatePropertiesControls();
                    jsObject.options.statusPanel.componentNameCell.innerHTML = answer.newName;
                    jsObject.UpdateStateUndoRedoButtons();
                    if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                    break;
                }
            case "WizardResult":
                {
                    jsObject.options.reportGuid = answer.reportGuid;
                    jsObject.LoadReport(jsObject.ParseReport(answer.reportObject));
                    jsObject.options.reportIsModified = true;
                    if (jsObject.options.cloudParameters) {
                        if (!jsObject.options.cloudParameters.thenOpenWizard) {
                            jsObject.options.cloudParameters.reportTemplateItemKey = null;
                        }
                        jsObject.options.cloudParameters.thenOpenWizard = false;
                    }
                    break;
                }
            case "NewDictionary":
            case "SynchronizeDictionary":
            case "EmbedAllDataToResources":
                {
                    answer.dictionary.attachedItems = jsObject.CopyObject(jsObject.options.report.dictionary.attachedItems);
                    jsObject.options.report.dictionary = answer.dictionary;
                    jsObject.options.dictionaryTree.build(answer.dictionary, true);
                    jsObject.ClearAllGalleries();
                    break;
                }
            case "CreateOrEditConnection":
                {
                    jsObject.options.report.dictionary.databases = answer.databases;
                    if (answer.itemObject) jsObject.options.dictionaryTree.createOrEditConnection(answer);
                    jsObject.options.dictionaryPanel.toolBar.updateControls();
                    jsObject.UpdateStateUndoRedoButtons();
                    jsObject.ClearAllGalleries();
                    if (answer.mode == "New") {
                        var processImage = jsObject.options.processImage || jsObject.InitializeProcessImage();
                        processImage.hide();

                        if (answer.skipSchemaWizard) {
                            jsObject.InitializeEditDataSourceForm(function (editDataSourceForm) {
                                editDataSourceForm.datasource = jsObject.GetDataAdapterTypeFromDatabaseType(answer.itemObject.typeConnection);
                                editDataSourceForm.nameInSource = answer.itemObject.name;
                                editDataSourceForm.databaseObject = answer.itemObject;
                                editDataSourceForm.changeVisibleState(true);
                            });
                        }
                        else {
                            jsObject.InitializeSelectDataForm(function (selectDataForm) {
                                selectDataForm.databaseName = answer.itemObject.name;
                                selectDataForm.typeConnection = answer.itemObject.typeConnection;
                                selectDataForm.connectionObject = answer.itemObject;
                                selectDataForm.changeVisibleState(true);
                            });
                        }
                    }
                    break;
                }
            case "DeleteConnection":
                {
                    if (answer.deleteResult) {
                        jsObject.options.dictionaryTree.selectedItem.remove();
                        jsObject.options.report.dictionary.databases = answer.databases;
                        jsObject.ClearAllGalleries();
                        jsObject.UpdateStateUndoRedoButtons();
                    }
                    break;
                }
            case "CreateOrEditRelation":
                {
                    if (answer.itemObject) {
                        jsObject.options.report.dictionary.databases = answer.databases;
                        if (answer.mode == "New" || answer.copyModeActivated == true)
                            jsObject.options.dictionaryTree.addRelation(answer.itemObject, answer);
                        else
                            jsObject.options.dictionaryTree.editRelation(answer);

                        jsObject.UpdateStateUndoRedoButtons();
                        jsObject.ClearAllGalleries();
                        if (jsObject.options.forms.dataForm && jsObject.options.forms.dataForm.visible) {
                            jsObject.options.forms.dataForm.rebuildTrees(answer.itemObject.nameInSource, "Relation");
                        }
                        if (jsObject.options.forms.crossTabForm && jsObject.options.forms.crossTabForm.visible) {
                            jsObject.options.forms.crossTabForm.tabbedPane.tabsPanels.Data.rebuildTrees(answer.itemObject.nameInSource, "Relation");
                        }
                    }
                    break;
                }
            case "DeleteRelation":
                {
                    if (answer.deleteResult) {
                        jsObject.options.dictionaryTree.deleteRelation();
                        jsObject.options.report.dictionary.databases = answer.databases;
                        jsObject.UpdateStateUndoRedoButtons();
                        jsObject.ClearAllGalleries();
                    }
                    break;
                }
            case "CreateOrEditColumn":
                {
                    if (answer.itemObject) {
                        if (answer.mode == "New")
                            jsObject.options.dictionaryTree.addColumn(answer);
                        else
                            jsObject.options.dictionaryTree.editColumn(answer);
                        if (answer.databases)
                            jsObject.options.report.dictionary.databases = answer.databases;
                        if (answer.businessObjects)
                            jsObject.options.report.dictionary.businessObjects = answer.businessObjects;

                        jsObject.UpdateStateUndoRedoButtons();
                        jsObject.ClearAllGalleries();
                    }
                    break;
                }
            case "DeleteColumn":
                {
                    if (answer.deleteResult) {
                        jsObject.options.dictionaryTree.deleteColumn(answer);
                        if (answer.databases) jsObject.options.report.dictionary.databases = answer.databases;
                        if (answer.businessObjects) jsObject.options.report.dictionary.businessObjects = answer.businessObjects;
                        jsObject.UpdateStateUndoRedoButtons();
                        jsObject.ClearAllGalleries();
                    }
                    break;
                }
            case "CreateOrEditParameter":
                {
                    if (answer.itemObject) {
                        if (answer.mode == "New") jsObject.options.dictionaryTree.addParameter(answer);
                        else jsObject.options.dictionaryTree.editParameter(answer);
                        if (answer.databases) jsObject.options.report.dictionary.databases = answer.databases;
                        jsObject.UpdateStateUndoRedoButtons();
                        jsObject.ClearAllGalleries();
                    }
                    break;
                }
            case "DeleteParameter":
                {
                    if (answer.deleteResult) {
                        jsObject.options.dictionaryTree.deleteParameter(answer);
                        if (answer.databases) jsObject.options.report.dictionary.databases = answer.databases;
                        jsObject.UpdateStateUndoRedoButtons();
                        jsObject.ClearAllGalleries();
                    }
                    break;
                }
            case "CreateOrEditDataSource":
                {
                    if (answer.itemObject) {
                        if (answer.mode == "New")
                            jsObject.options.dictionaryTree.addDataSource(answer.itemObject);
                        else
                            jsObject.options.dictionaryTree.editDataSource(answer.itemObject);
                        jsObject.options.report.dictionary.databases = answer.databases;

                        jsObject.UpdateStateUndoRedoButtons();
                        if (jsObject.options.forms.dataForm && jsObject.options.forms.dataForm.visible) {
                            jsObject.options.forms.dataForm.rebuildTrees(answer.itemObject.name, "DataSource");
                        }
                        if (jsObject.options.forms.editDataSourceFromOtherDatasourcesForm && jsObject.options.forms.editDataSourceFromOtherDatasourcesForm.visible) {
                            jsObject.options.forms.editDataSourceFromOtherDatasourcesForm.rebuildTrees(answer.itemObject.name);
                        }
                        if (jsObject.options.forms.crossTabForm && jsObject.options.forms.crossTabForm.visible) {
                            jsObject.options.forms.crossTabForm.tabbedPane.tabsPanels.Data.rebuildTrees(answer.itemObject.name, "DataSource");
                        }
                        if (jsObject.options.forms.wizardForm && jsObject.options.forms.wizardForm.visible) {
                            var dataSources = jsObject.options.report ? jsObject.GetDataSourcesAndBusinessObjectsFromDictionary(jsObject.options.report.dictionary) : null;
                            jsObject.options.forms.wizardForm.dataSourcesFromServer = dataSources;
                            jsObject.options.forms.wizardForm.onshow(true);
                        }
                        if (jsObject.options.forms.wizardForm2 && jsObject.options.forms.wizardForm2.visible) {
                            jsObject.options.forms.wizardForm2.stepPanel.update(true);
                        }
                        if (jsObject.options.forms.getDataForm && jsObject.options.forms.getDataForm.visible) {
                            jsObject.options.forms.getDataForm.updateData();
                        }
                        jsObject.ClearAllGalleries();
                    }
                    break;
                }
            case "DeleteDataSource":
                {
                    if (answer.deleteResult) {
                        jsObject.options.dictionaryTree.deleteDataSource();
                        jsObject.options.report.dictionary.databases = answer.databases;
                        jsObject.UpdateStateUndoRedoButtons();
                        jsObject.ClearAllGalleries();
                    }
                    break;
                }
            case "CreateOrEditBusinessObject":
                {
                    if (answer.itemObject) {
                        if (answer.mode == "New")
                            jsObject.options.dictionaryTree.addBusinessObject(answer.itemObject, answer.parentBusinessObjectFullName);
                        else
                            jsObject.options.dictionaryTree.editBusinessObject(answer.itemObject);
                        jsObject.options.report.dictionary.businessObjects = answer.businessObjects;
                        jsObject.UpdateStateUndoRedoButtons();
                        if (jsObject.options.forms.dataForm && jsObject.options.forms.dataForm.visible) {
                            jsObject.options.forms.dataForm.rebuildTrees(answer.itemObject.name, "BusinessObject");
                        }
                        if (jsObject.options.forms.crossTabForm && jsObject.options.forms.crossTabForm.visible) {
                            jsObject.options.forms.crossTabForm.tabbedPane.tabsPanels.Data.rebuildTrees(answer.itemObject.name, "BusinessObject");
                        }
                        jsObject.ClearAllGalleries();
                    }
                    break;
                }
            case "DeleteBusinessObject":
                {
                    if (answer.deleteResult) {
                        jsObject.options.dictionaryTree.deleteBusinessObject();
                        jsObject.options.report.dictionary.businessObjects = answer.businessObjects;
                        jsObject.UpdateStateUndoRedoButtons();
                        jsObject.ClearAllGalleries();
                    }
                    break;
                }
            case "DeleteBusinessObjectCategory":
                {
                    jsObject.options.dictionaryTree.selectedItem.remove();
                    jsObject.options.report.dictionary.businessObjects = answer.businessObjects;
                    jsObject.UpdateStateUndoRedoButtons();
                    jsObject.ClearAllGalleries();
                    break;
                }
            case "EditBusinessObjectCategory":
                {
                    jsObject.options.dictionaryTree.editBusinessObjectCategory(answer);
                    jsObject.options.report.dictionary.businessObjects = answer.businessObjects;
                    jsObject.UpdateStateUndoRedoButtons();
                    jsObject.ClearAllGalleries();
                    break;
                }
            case "DeleteVariablesCategory":
                {
                    jsObject.options.dictionaryTree.selectedItem.remove();
                    jsObject.options.report.dictionary.variables = answer.variables;
                    jsObject.UpdateStateUndoRedoButtons();
                    jsObject.ClearAllGalleries();
                    break;
                }
            case "EditVariablesCategory":
                {
                    var itemObject = jsObject.options.dictionaryTree.selectedItem.itemObject;
                    itemObject.name = answer.newName;
                    itemObject.requestFromUser = answer.requestFromUser;
                    itemObject.readOnly = answer.readOnly;
                    jsObject.options.dictionaryTree.selectedItem.repaint();
                    jsObject.options.report.dictionary.variables = answer.variables;
                    jsObject.UpdateStateUndoRedoButtons();
                    jsObject.ClearAllGalleries();
                    break;
                }
            case "CreateVariablesCategory":
                {
                    jsObject.options.dictionaryTree.createVariablesCategory(answer);
                    jsObject.options.report.dictionary.variables = answer.variables;
                    jsObject.UpdateStateUndoRedoButtons();
                    jsObject.ClearAllGalleries();
                    break;
                }
            case "CreateOrEditVariable":
                {
                    if (answer.itemObject) {
                        if (answer.mode == "New")
                            jsObject.options.dictionaryTree.addVariable(answer.itemObject);
                        else
                            jsObject.options.dictionaryTree.editVariable(answer.itemObject);

                        jsObject.options.report.dictionary.variables = answer.variables;
                        jsObject.UpdateStateUndoRedoButtons();
                        jsObject.ClearAllGalleries();
                        jsObject.SendCommandRepaintAllDbsElements();
                    }
                    break;
                }
            case "DeleteVariable":
                {
                    if (answer.deleteResult) {
                        jsObject.options.dictionaryTree.selectedItem.remove();
                        jsObject.options.report.dictionary.variables = answer.variables;
                        jsObject.UpdateStateUndoRedoButtons();
                        jsObject.ClearAllGalleries();
                        jsObject.SendCommandRepaintAllDbsElements();
                    }
                    break;
                }
            case "CreateOrEditResource":
                {
                    if (!(answer.commandGuid && jsObject.options.abortedCommands[answer.commandGuid])) {
                        if (answer.loadingCompleted !== false) {
                            if (answer.itemObject) {
                                if (answer.mode == "New")
                                    jsObject.options.dictionaryTree.addResource(answer.itemObject);
                                else
                                    jsObject.options.dictionaryTree.editResource(answer.itemObject);

                                jsObject.options.report.dictionary.resources = answer.resources;
                                jsObject.UpdateStateUndoRedoButtons();
                                jsObject.ClearAllGalleries();
                                jsObject.UpdateResourcesFonts();

                                if (answer.callbackFunctionId && jsObject.options.callbackFunctions[answer.callbackFunctionId]) {
                                    jsObject.options.callbackFunctions[answer.callbackFunctionId](answer.itemObject);
                                    delete jsObject.options.callbackFunctions[answer.callbackFunctionId];
                                }
                            }
                        }
                        else if (jsObject.options.dictionaryPanel) {
                            jsObject.options.dictionaryPanel.showProgress(answer.progress, answer.commandGuid);
                        }
                    }
                    break;
                }
            case "DeleteResource":
                {
                    if (answer.deleteResult) {
                        jsObject.options.dictionaryTree.selectedItem.remove();
                        jsObject.options.report.dictionary.resources = answer.resources;
                        jsObject.UpdateStateUndoRedoButtons();
                        jsObject.ClearAllGalleries();
                        jsObject.UpdateResourcesFonts();
                    }
                    else if (answer.usedObjects) {
                        var form = jsObject.MessageFormForDeleteUsedResource();
                        form.show(answer.resourceName, answer.usedObjects);
                        form.action = function (state) {
                            if (state) {
                                var selectedItem = jsObject.options.dictionaryTree.selectedItem;
                                if (selectedItem) jsObject.SendCommandDeleteResource(selectedItem, true);
                            }
                        }
                    }
                    break;
                }
            case "DuplicateDictionaryElement":
                {
                    if (answer.typeItem == "DataBase") {
                        jsObject.options.report.dictionary.databases = answer.databases;
                        jsObject.options.dictionaryTree.createOrEditConnection(answer);
                    }
                    else if (answer.typeItem == "DataSource") {
                        jsObject.options.report.dictionary.databases = answer.databases;
                        jsObject.options.dictionaryTree.addDataSource(answer.itemObject);
                    }
                    else if (answer.typeItem == "Relation") {
                        jsObject.options.report.dictionary.databases = answer.databases;
                        jsObject.options.dictionaryTree.addRelation(answer.itemObject, answer);
                    }
                    else if (answer.typeItem == "Variable") {
                        jsObject.options.dictionaryTree.addVariable(answer.itemObject);
                        jsObject.options.report.dictionary.variables = answer.variables;
                    }
                    else if (answer.typeItem == "Resource") {
                        jsObject.options.dictionaryTree.addResource(answer.itemObject);
                        jsObject.options.report.dictionary.resources = answer.resources;
                    }
                    else if (answer.typeItem == "Category") {
                        jsObject.options.dictionaryTree.createVariablesCategory(answer);
                        jsObject.options.report.dictionary.variables = answer.variables;
                    }
                    jsObject.UpdateStateUndoRedoButtons();
                    jsObject.ClearAllGalleries();
                    break;
                }
            case "GetAllConnections":
                {
                    jsObject.InitializeNameInSourceForm(function (nameInSourceForm) {
                        nameInSourceForm.connections = answer.connections;
                        nameInSourceForm.changeVisibleState(true);
                    });
                    break;
                }
            case "RetrieveColumns":
                {
                    if (answer.columns) {
                        jsObject.InitializeEditDataSourceForm(function (editDataSourceForm) {
                            var currentColumns = editDataSourceForm.columnsAndParametersTree.getItemObjects("Column");
                            var currentParameters = editDataSourceForm.columnsAndParametersTree.getItemObjects("Parameter");
                            var allColumns = jsObject.ConcatColumns(currentColumns, answer.columns);
                            var allParameters = jsObject.ConcatColumns(currentParameters, answer.parameters);
                            editDataSourceForm.columnsAndParametersTree.addColumnsAndParameters(allColumns, allParameters, true);
                            editDataSourceForm.columnsAndParametersTree.parametersItem.style.display = editDataSourceForm.datasource.parameterTypes ? "" : "none";
                            editDataSourceForm.columnsAndParametersTree.onSelectedItem();
                        });
                        jsObject.ClearAllGalleries();
                    }
                    break;
                }           
            case "CreateStylesFromComponents":
                {
                    if (answer.styles) {
                        jsObject.InitializeStyleDesignerForm(function (styleDesignerForm) {
                            for (var i = 0; i < answer.styles.length; i++) {
                                styleDesignerForm.stylesTree.addItem(answer.styles[i]);
                            }
                        });
                    }
                    break;
                }
            case "UpdateStyles":
                {
                    jsObject.options.reportGuid = answer.reportGuid;
                    jsObject.LoadReport(jsObject.ParseReport(answer.reportObject), true);
                    jsObject.options.reportIsModified = true;
                    jsObject.UpdateStateUndoRedoButtons();

                    if (answer.selectedObjectName) jsObject.BackToSelectedComponent(answer.selectedObjectName);
                    if (jsObject.options.forms.crossTabForm && jsObject.options.forms.crossTabForm.visible) {
                        jsObject.options.forms.crossTabForm.controls.stylesContainer.update();
                    }

                    if (jsObject.options.report.stylesCache) {
                        jsObject.options.report.stylesCache = {
                            chartStyles: {},
                            gaugeStyles: {},
                            mapStyles: {},
                            tableStyles: null,
                            crossTabStyles: null
                        };
                    }

                    var editChartForm = jsObject.options.forms.editChart;
                    if (editChartForm && editChartForm.visible) {
                        editChartForm.stylesContainer.stylesProgress.style.display = "";
                        editChartForm.stylesContainer.clear();
                        editChartForm.jsObject.SendCommandGetStylesContent({ componentName: editChartForm.chartProperties.name });
                    }
                    break;
                }
            case "CreateStyleCollection":
                {
                    jsObject.InitializeStyleDesignerForm(function (styleDesignerForm) {
                        styleDesignerForm.addStylesCollection(answer.newStylesCollection, answer.collectionName, answer.removeExistingStyles);
                    });
                    break;
                }
            case "StartEditChartComponent":
            case "StartEditChartElement":
                {
                    var forms = jsObject.options.forms;
                    if (answer.formName == "editChartSeriesForm") {
                        if (forms.editChartSimpleForm && forms.editChartSimpleForm.visible)
                            forms.editChartSimpleForm.changeVisibleState(false);

                        if (forms.editChart && forms.editChart.visible)
                            forms.editChart.changeVisibleState(false);

                        forms.editChartSeriesForm.chartProperties = answer.properties;
                        forms.editChartSeriesForm.changeVisibleState(true);
                    }
                    else if (answer.formName == "editChartStripsForm") {
                        if (forms.editChartSimpleForm && forms.editChartSimpleForm.visible)
                            forms.editChartSimpleForm.changeVisibleState(false);

                        if (forms.editChart && forms.editChart.visible)
                            forms.editChart.changeVisibleState(false);

                        forms.editChartStripsForm.chartProperties = answer.properties;
                        forms.editChartStripsForm.changeVisibleState(true);
                    }
                    else if (answer.formName == "editChartConstantLinesForm") {
                        if (forms.editChartSimpleForm && forms.editChartSimpleForm.visible)
                            forms.editChartSimpleForm.changeVisibleState(false);

                        if (forms.editChart && forms.editChart.visible)
                            forms.editChart.changeVisibleState(false);

                        forms.editChartConstantLinesForm.chartProperties = answer.properties;
                        forms.editChartConstantLinesForm.changeVisibleState(true);
                    }
                    else if (answer.formName == "editChartSimpleForm") {
                        if (forms.editChartSeriesForm && forms.editChartSeriesForm.visible)
                            forms.editChartSeriesForm.changeVisibleState(false);

                        forms.editChartSimpleForm.chartProperties = answer.properties;
                        forms.editChartSimpleForm.changeVisibleState(true);
                    }
                    else if (answer.formName == "editChart") {
                        if (forms.editChartSeriesForm && forms.editChartSeriesForm.visible)
                            forms.editChartSeriesForm.changeVisibleState(false);

                        forms.editChart.chartProperties = answer.properties;
                        forms.editChart.changeVisibleState(true);
                    }
                    break;
                }
            case "StartEditMapComponent":
                {
                    if (answer.properties && !jsObject.options.previewMode) {
                        jsObject.InitializeEditMapForm(function (editMapForm) {
                            editMapForm.mapProperties = answer.properties;
                            editMapForm.mapSvgContent = answer.svgContent;
                            editMapForm.mapIframeContent = answer.iframeContent;
                            editMapForm.mapCultures = answer.cultures;

                            if (answer.additionalParams && answer.additionalParams.createdByDragged) {
                                editMapForm.changeVisibleState(true);
                                editMapForm.style.display = "none";
                                editMapForm.controls.mapID.button.action();
                            }
                            else {
                                editMapForm.changeVisibleState(true);
                            }
                        });
                    }
                    break;
                }
            case "StartEditGaugeComponent":
                {
                    if (answer.properties && !jsObject.options.previewMode) {
                        jsObject.InitializeEditGaugeForm(function (editGaugeForm) {
                            editGaugeForm.gaugeProperties = answer.properties;
                            editGaugeForm.gaugeSvgContent = answer.svgContent;
                            editGaugeForm.changeVisibleState(true);
                        });
                    }
                    break;
                }
            case "StartEditSparklineComponent":
                {
                    if (answer.properties && !jsObject.options.previewMode) {
                        jsObject.InitializeEditSparklineForm(function (form) {
                            form.sparklineProperties = answer.properties;
                            form.sparklineSvgContent = answer.svgContent;
                            form.changeVisibleState(true);
                        });
                    }
                    break;
                }
            case "AddSeries":
                {
                    if (answer.properties) {
                        var editChartForm = jsObject.options.forms.editChart;
                        editChartForm.chartProperties = answer.properties;
                        var lastSeries = editChartForm.chartProperties.series[editChartForm.chartProperties.series.length - 1];
                        var seriesItem = editChartForm.seriesContainer.addItem(lastSeries.name, lastSeries);
                        editChartForm.chartImage.update();
                    }
                    break;
                }
            case "SeriesMove":
            case "RemoveSeries":
                {
                    if (answer.properties) {
                        var editChartForm = jsObject.options.forms.editChart;
                        editChartForm.chartProperties = answer.properties;
                        editChartForm.seriesContainer.update();
                        editChartForm.chartImage.update();
                        if (answer.selectedIndex != null) {
                            editChartForm.seriesContainer.items[answer.selectedIndex].action();
                        }
                    }
                    break;
                }
            case "AddConstantLineOrStrip":
                {
                    if (answer.properties) {
                        var editChartForm = jsObject.options.forms.editChart;
                        editChartForm.chartProperties = answer.properties;
                        var itemType = answer.itemType;
                        var container = editChartForm[itemType + "Container"];
                        var collection = itemType == "ConstantLines" ? editChartForm.chartProperties.constantLines : editChartForm.chartProperties.strips;
                        var lastItem = collection[collection.length - 1];
                        var newItem = container.addItem(lastItem.name, lastItem);
                        editChartForm.chartImage.update();
                    }
                    break;
                }
            case "ConstantLineOrStripMove":
            case "RemoveConstantLineOrStrip":
                {
                    if (answer.properties) {
                        var editChartForm = jsObject.options.forms.editChart;
                        editChartForm.chartProperties = answer.properties;
                        var itemType = answer.itemType;
                        var container = editChartForm[itemType + "Container"];
                        container.update();
                        editChartForm.chartImage.update();
                        if (answer.selectedIndex != null) {
                            container.items[answer.selectedIndex].action();
                        }
                    }
                    break;
                }
            case "GetStylesContent":
                {
                    if (answer.stylesContent) {
                        var editChartForm = jsObject.options.forms.editChart;
                        editChartForm.stylesContainer.update(answer.stylesContent);
                    }
                    break;
                }
            case "SetLabelsType":
            case "SetChartStyle":
            case "SetChartPropertyValue":
                {
                    if (answer.properties) {
                        var forms = jsObject.options.forms;

                        if (forms.editChartSimpleForm && forms.editChartSimpleForm.visible) {
                            var simpleForm = forms.editChartSimpleForm;
                            simpleForm.chartProperties = answer.properties;
                            var selectedItem = simpleForm.getSelectedItem();
                            simpleForm.updateControlsValues(selectedItem ? selectedItem.container.name : "values", selectedItem ? selectedItem.container.getItemIndex(selectedItem) : 0);
                            simpleForm.updateControlsVisibleStates();
                            simpleForm.checkStartMode();
                            simpleForm.correctTopPosition();
                            simpleForm.updateSvgContent(answer.properties.svgContent);
                        }
                        else if (forms.editChartSeriesForm && forms.editChartSeriesForm.visible) {
                            var seriesForm = forms.editChartSeriesForm;
                            seriesForm.chartProperties = answer.properties;
                            seriesForm.seriesContainer.fill(answer.properties.series, seriesForm.seriesContainer.getSelectedItemIndex() || 0);
                            seriesForm.updateSvgContent(answer.properties.svgContent);
                        }
                        else if (forms.editChartStripsForm && forms.editChartStripsForm.visible) {
                            var stripsForm = forms.editChartStripsForm;
                            stripsForm.chartProperties = answer.properties;
                            stripsForm.stripsContainer.fill(answer.properties.strips || answer.properties.chartStrips, stripsForm.stripsContainer.getSelectedItemIndex() || 0);
                            stripsForm.updateSvgContent(answer.properties.svgContent);
                        }
                        else if (forms.editChartConstantLinesForm && forms.editChartConstantLinesForm.visible) {
                            var constantLinesForm = forms.editChartConstantLinesForm;
                            constantLinesForm.chartProperties = answer.properties;
                            constantLinesForm.constantLinesContainer.fill(answer.properties.constantLines || answer.properties.chartConstantLines, constantLinesForm.constantLinesContainer.getSelectedItemIndex() || 0);
                            constantLinesForm.updateSvgContent(answer.properties.svgContent);
                        }
                        else if (forms.editChart && forms.editChart.visible) {
                            var chartForm = forms.editChart;
                            chartForm.chartProperties = answer.properties;
                            chartForm.chartImage.update();
                            if (answer.command == "SetLabelsType") {
                                if (!answer.isSeriesLabels)
                                    chartForm.labelPropertiesContainer.buttons.Common.action();
                                else
                                    chartForm.seriesPropertiesContainer.buttons.SeriesLabels.action();
                            }
                        }
                    }
                    break;
                }
            case "SendContainerValue":
                {
                    if (answer.properties) {
                        var editChartForm = jsObject.options.forms.editChart;
                        editChartForm.chartProperties = answer.properties;
                        editChartForm.chartImage.update();
                    }
                    if (answer.closeChartForm) editChartForm.action();
                    break;
                }
            case "ApplySelectedData":
                {
                    if (answer.dictionary) {
                        answer.dictionary.attachedItems = jsObject.CopyObject(jsObject.options.report.dictionary.attachedItems);
                        jsObject.options.report.dictionary = answer.dictionary;
                        jsObject.options.dictionaryTree.build(answer.dictionary, true);
                        var selectedDatabaseItem = null;
                        for (var key in jsObject.options.dictionaryTree.mainItems.DataSources.childs) {
                            var databaseItem = jsObject.options.dictionaryTree.mainItems.DataSources.childs[key];
                            if (databaseItem.itemObject.name == answer.databaseName) {
                                selectedDatabaseItem = databaseItem;
                                break;
                            }
                        }
                        if (selectedDatabaseItem) {
                            selectedDatabaseItem.openTree();
                            selectedDatabaseItem.setOpening(true);
                            for (var key in selectedDatabaseItem.childs) {
                                var dataSourceItem = selectedDatabaseItem.childs[key];
                                if (dataSourceItem.itemObject.name.indexOf(answer.selectedDataSource.name) == 0) {
                                    dataSourceItem.setSelected();
                                    break;
                                }
                            }
                        }
                        if (jsObject.options.forms.dataForm && jsObject.options.forms.dataForm.visible) {
                            jsObject.options.forms.dataForm.rebuildTrees(answer.selectedDataSource.name, "DataSource");
                        }
                        if (jsObject.options.forms.editDataSourceFromOtherDatasourcesForm && jsObject.options.forms.editDataSourceFromOtherDatasourcesForm.visible) {
                            jsObject.options.forms.editDataSourceFromOtherDatasourcesForm.rebuildTrees(answer.selectedDataSource.name);
                        }
                        if (jsObject.options.forms.crossTabForm && jsObject.options.forms.crossTabForm.visible) {
                            jsObject.options.forms.crossTabForm.tabbedPane.tabsPanels.Data.rebuildTrees(answer.selectedDataSource.name, "DataSource");
                        }
                        if (jsObject.options.forms.wizardForm && jsObject.options.forms.wizardForm.visible) {
                            var dataSources = jsObject.options.report ? jsObject.GetDataSourcesAndBusinessObjectsFromDictionary(jsObject.options.report.dictionary) : null;
                            jsObject.options.forms.wizardForm.dataSourcesFromServer = dataSources;
                            jsObject.options.forms.wizardForm.onshow(true);
                        }
                        if (jsObject.options.forms.wizardForm2 && jsObject.options.forms.wizardForm2.visible) {
                            jsObject.options.forms.wizardForm2.stepPanel.update(true);
                        }
                        if (jsObject.options.forms.getDataForm && jsObject.options.forms.getDataForm.visible) {
                            jsObject.options.forms.getDataForm.updateData();
                        }
                    }
                    jsObject.ClearAllGalleries();
                    break;
                }
            case "CreateTextComponent":
                {
                    if (answer.newComponents) {
                        for (var i = 0; i < answer.newComponents.length; i++) {
                            var component = jsObject.CreateComponent(answer.newComponents[i]);
                            if (component) {
                                component.repaint();
                                jsObject.options.report.pages[component.properties.pageName].appendChild(component);
                                jsObject.options.report.pages[component.properties.pageName].components[component.properties.name] = component;
                                jsObject.options.report.pages[component.properties.pageName].rebuild(answer.rebuildProps);
                                component.setOnTopLevel();
                                component.setSelected();
                                jsObject.UpdatePropertiesControls();
                                jsObject.UpdateStateUndoRedoButtons();
                            }
                        }
                        if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                    }
                    break;
                }
            case "CreateDataComponent":
                {
                    if (answer.pageComponents) {
                        var pageComponents = answer.pageComponents;
                        var lastComponent = null;
                        for (var componentName in pageComponents.components) {
                            var componentProps = pageComponents.components[componentName];

                            if (jsObject.options.report.pages[answer.pageName].components[componentProps.name] == null) {
                                var component = jsObject.CreateComponent(componentProps);
                                if (component) {
                                    component.repaint();
                                    lastComponent = component;
                                    jsObject.options.report.pages[answer.pageName].appendChild(component);
                                    jsObject.options.report.pages[answer.pageName].components[componentProps.name] = component;
                                }
                            }
                        }
                        if (lastComponent) {
                            lastComponent.setOnTopLevel();
                            lastComponent.setSelected();
                            jsObject.UpdatePropertiesControls();
                            jsObject.UpdateStateUndoRedoButtons();
                        }
                        jsObject.options.report.pages[answer.pageName].rebuild(answer.rebuildProps);
                        if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                        if (answer.stylesCollection) jsObject.options.report.stylesCollection = answer.stylesCollection;
                    }
                    break;
                }
            case "SetReportProperties":
                {
                    if (jsObject.options.report) {
                        jsObject.WriteAllProperties(jsObject.options.report, answer.properties);
                        jsObject.UpdatePropertiesControls();
                        jsObject.UpdateStateUndoRedoButtons();
                    }
                    break;
                }
            case "PageMove":
                {
                    jsObject.ChangePageIndexes(answer.pageIndexes);
                    jsObject.options.pagesPanel.pagesContainer.updatePages();
                    if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                    break;
                }
            case "TestConnection":
                {
                    jsObject.InitializeEditConnectionForm(function (editConnectionForm) {
                        editConnectionForm.testConnection.setEnabled(true);
                        var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                        if (answer.testResult != null) errorMessageForm.show(answer.testResult, true);
                    });
                    break;
                }
            case "RunQueryScript":
                {
                    var text = answer.resultQueryScript == "successfully"
                        ? jsObject.loc.FormDictionaryDesigner.ExecutedSQLStatementSuccessfully
                        : (answer.resultQueryScript || jsObject.loc.DesignerFx.ConnectionError);
                    var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                    errorMessageForm.show(text, answer.resultQueryScript == "successfully");
                    break;
                }
            case "ApplyDesignerOptions":
                {
                    jsObject.LoadReport(jsObject.ParseReport(answer.reportObject), true);
                    jsObject.options.reportIsModified = true;
                    if (answer.selectedObjectName) {
                        jsObject.BackToSelectedComponent(answer.selectedObjectName);
                    }
                    break;
                }
            case "GetSqlParameterTypes":
                {
                    if (jsObject.options.forms.editDataSourceForm && jsObject.options.forms.editDataSourceForm.visible &&
                        answer.sqlParameterTypes && jsObject.options.forms.editDataSourceForm.datasource) {
                        var editDataSourceForm = jsObject.options.forms.editDataSourceForm;
                        var isUndefined = editDataSourceForm.datasource.typeDataSource == "StiUndefinedDataSource";
                        if (!isUndefined) {
                            editDataSourceForm.columnsHeader.caption.innerHTML = jsObject.loc.FormPageSetup.Columns + " & " + jsObject.loc.PropertyMain.Parameters;
                            jsObject.options.forms.editDataSourceForm.datasource.parameterTypes = answer.sqlParameterTypes;
                            jsObject.options.forms.editDataSourceForm.columnToolBar.parameterNew.style.display = "";
                            jsObject.options.forms.editDataSourceForm.columnToolBar.retrieveColumnsAndParameters.style.display = "";
                            jsObject.options.forms.editDataSourceForm.columnsAndParametersTree.parametersItem.style.display = "";
                        }
                    }
                    break;
                }
            case "AlignToGridComponents":
            case "ChangeArrangeComponents":
            case "ChangeSizeComponents":
                {
                    jsObject.options.report.pages[answer.pageName].rebuild(answer.rebuildProps);
                    jsObject.PaintSelectedLines();
                    jsObject.UpdatePropertiesControls();
                    jsObject.UpdateStateUndoRedoButtons();
                    if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                    break;
                }
            case "UpdateSampleTextFormat":
                {
                    var textFormatForm = jsObject.options.forms.textFormatForm;
                    if (textFormatForm && textFormatForm.visible) {
                        textFormatForm.sampleContainer.innerHTML = answer.sampleText;
                    }
                    break;
                }
            case "StartEditCrossTabComponent":
                {
                    if (!jsObject.options.previewMode) {
                        jsObject.InitializeCrossTabForm(function (editCrossTabForm) {
                            editCrossTabForm.changeVisibleState(true);
                        });
                    }
                    break;
                }
            case "UpdateCrossTabComponent":
                {
                    if (answer.command && answer.callbackFunctionId && jsObject.options.callbackFunctions[answer["callbackFunctionId"]] != null) {
                        jsObject.options.callbackFunctions[answer["callbackFunctionId"]](answer);
                        delete jsObject.options.callbackFunctions[answer["callbackFunctionId"]];
                    }
                    else {
                        jsObject.InitializeCrossTabForm(function (editCrossTabForm) {
                            editCrossTabForm.recieveCommandResult(answer.updateResult);
                        });
                    }
                    break;
                }
            case "GetCrossTabColorStyles":
                {
                    if (answer.colorStyles && jsObject.options.forms.crossTabForm && jsObject.options.forms.crossTabForm.visible) {
                        jsObject.options.forms.crossTabForm.controls.stylesContainer.fill(answer.colorStyles);
                    }
                    break;
                }
            case "DuplicatePage":
                {
                    if (answer["reportGuid"] && answer["reportObject"]) {
                        jsObject.options.reportGuid = answer.reportGuid;
                        jsObject.LoadReport(jsObject.ParseReport(answer.reportObject), true);
                        jsObject.options.reportIsModified = true;
                        jsObject.UpdateStateUndoRedoButtons();
                        var pageButton = jsObject.options.pagesPanel.pagesContainer.pages[answer.selectedPageIndex];
                        if (pageButton) pageButton.action();
                    }
                    break;
                }
            case "SetEventValue":
                {
                    break;
                }
            case "GetCrossTabStylesContent":
            case "GetTableStylesContent":
            case "GetGaugeStylesContent":
            case "GetMapStylesContent":
            case "GetSparklineStylesContent":
                {
                    if (jsObject.options.callbackFunctions[answer["callbackFunctionId"]]) {
                        jsObject.options.callbackFunctions[answer["callbackFunctionId"]](answer["stylesContent"]);
                        delete jsObject.options.callbackFunctions[answer["callbackFunctionId"]];
                    }
                    break;
                }
            case "ChangeTableComponent":
                {
                    if (answer.result) {
                        switch (answer.result.command) {
                            case "convertTo":
                            case "insertColumnToLeft":
                            case "insertColumnToRight":
                            case "deleteColumn":
                            case "insertRowAbove":
                            case "insertRowBelow":
                            case "deleteRow":
                            case "joinCells":
                            case "unJoinCells":
                            case "changeColumnsOrRowsCount":
                            case "applyStyle":
                                {
                                    var cells = answer.result.cells;
                                    var page = jsObject.options.report.pages[answer.result.pageName];
                                    var table = page.components[answer.result.tableName];
                                    if (cells && table) {
                                        jsObject.RebuildTable(table, cells, answer.result.command != "convertTo");
                                        if (answer.result.tableProperties) {
                                            jsObject.CreateComponentProperties(table, answer.result.tableProperties);
                                            table.repaint();
                                        }

                                        if (answer.result.selectedCells) {
                                            if (answer.result.selectedCells.length == 0) {
                                                table.setSelected();
                                            }
                                            else if (answer.result.selectedCells.length == 1) {
                                                var cell = page.components[answer.result.selectedCells[0]];
                                                if (cell) cell.setSelected();
                                            }
                                            else {
                                                jsObject.SetSelectedObjectsByNames(page, answer.result.selectedCells);
                                            }
                                        }
                                        jsObject.UpdatePropertiesControls();
                                        jsObject.UpdateStateUndoRedoButtons();
                                    }
                                    if (answer.result.rebuildProps) page.rebuild(answer.result.rebuildProps);
                                    if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                                    break;
                                }

                        }
                    }
                    break;
                }
            case "UpdateImagesArray":
                {
                    if (answer.images) {
                        for (var imageName in answer.images) {
                            jsObject.options.images[imageName] = answer.images[imageName];
                        }
                    }
                    break;
                }
            case "OpenStyle":
                {
                    var styleDesignerForm = jsObject.options.forms.styleDesignerForm
                    if (styleDesignerForm && styleDesignerForm.visible) {
                        styleDesignerForm.stylesCollection = answer.stylesCollection;
                        styleDesignerForm.stylesTree.updateItems(styleDesignerForm.stylesCollection);
                    }
                    break;
                }
            case "OpenDictionary":
            case "MergeDictionary":
                {
                    answer.dictionary.attachedItems = jsObject.CopyObject(jsObject.options.report.dictionary.attachedItems);
                    jsObject.options.report.dictionary = answer.dictionary;
                    jsObject.options.dictionaryTree.build(answer.dictionary, true);
                    jsObject.ClearAllGalleries();
                    break;
                }
            case "CreateFieldOnDblClick":
                {
                    var page = jsObject.options.report.pages[answer.pageName];
                    var newComponents = answer.newComponents;
                    var lastComponent = null;

                    //Add or change new cells
                    for (var i = 0; i < newComponents.length; i++) {
                        var compObject = newComponents[i];

                        var component = jsObject.CreateComponent(compObject);
                        page.appendChild(component);
                        page.components[compObject.name] = component;
                        component.repaint();
                        lastComponent = component;
                    }

                    jsObject.options.report.pages[answer.pageName].rebuild(answer.rebuildProps);

                    if (lastComponent) {
                        lastComponent.setOnTopLevel();
                        lastComponent.setSelected();
                        jsObject.UpdatePropertiesControls();
                        jsObject.UpdateStateUndoRedoButtons();
                    }
                    if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                    break;
                }
            case "GetParamsFromQueryString":
                {
                    if (jsObject.options.callbackFunctions[answer["callbackFunctionId"]]) {
                        jsObject.options.callbackFunctions[answer["callbackFunctionId"]](answer.params);
                        delete jsObject.options.callbackFunctions[answer["callbackFunctionId"]];
                    }
                    break;
                }
            case "CreateMovingCopyComponent":
                {
                    var components = answer["components"];
                    if (components && components.length > 0) {
                        for (var i = 0; i < components.length; i++) {
                            var component = components[i].properties.isDashboardElement
                                ? jsObject.CreateDashboardElement(components[i])
                                : jsObject.CreateComponent(components[i]);

                            if (component) {
                                component.repaint();
                                jsObject.options.report.pages[component.properties.pageName].appendChild(component);
                                jsObject.options.report.pages[component.properties.pageName].components[component.properties.name] = component;

                                if (i == 0) {
                                    component.setOnTopLevel();
                                    if (answer["isLastCommand"]) component.setSelected();
                                }
                            }
                        }
                        jsObject.options.currentPage.rebuild(answer.rebuildProps);
                        jsObject.UpdateStateUndoRedoButtons();
                    }

                    if (jsObject.options.callbackFunctions[answer["callbackFunctionId"]]) {
                        jsObject.options.callbackFunctions[answer["callbackFunctionId"]](answer);
                        delete jsObject.options.callbackFunctions[answer["callbackFunctionId"]];
                    }
                    if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                    break;
                }
            case "UpdateGaugeComponent":
                {
                    jsObject.InitializeEditGaugeForm(function (editGaugeForm) {
                        editGaugeForm.recieveCommandResult(answer.updateResult);
                    });
                    break;
                }
            case "CreateComponentFromResource":
                {
                    if (answer.newComponent) {
                        var component = jsObject.CreateComponent(answer.newComponent);
                        if (component) {
                            component.repaint();
                            jsObject.options.report.pages[component.properties.pageName].appendChild(component);
                            jsObject.options.report.pages[component.properties.pageName].components[component.properties.name] = component;
                            jsObject.options.report.pages[component.properties.pageName].rebuild(answer.rebuildProps);
                            component.setOnTopLevel();
                            component.setSelected();
                            jsObject.UpdatePropertiesControls();
                            jsObject.UpdateStateUndoRedoButtons();
                        }
                        if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                    }
                    break;
                }
            case "CreateDatabaseFromResource":
                {
                    if (!(answer.commandGuid && jsObject.options.abortedCommands[answer.commandGuid])) {
                        if (answer.loadingCompleted !== false) {
                            if (jsObject.options.dictionaryPanel) {
                                jsObject.options.dictionaryPanel.showProgress(answer.progress, answer.commandGuid);
                            }
                            if (answer.resourceItemObject) {
                                jsObject.options.dictionaryTree.addResource(answer.resourceItemObject);
                                jsObject.options.report.dictionary.resources = answer.resources;
                                jsObject.ClearAllGalleries();
                            }
                            if (answer.newDataBaseName) {
                                answer.itemObject = jsObject.GetObjectByPropertyValueFromCollection(answer.databases, "name", answer.newDataBaseName);
                                if (answer.itemObject) {
                                    answer.mode = "New";
                                    jsObject.options.report.dictionary.databases = answer.databases;
                                    jsObject.options.dictionaryTree.createOrEditConnection(answer);
                                    var newConnectionItem = jsObject.options.dictionaryTree.mainItems["DataSources"].getChildByName(answer.itemObject.name);
                                    if (newConnectionItem) {
                                        for (var key in newConnectionItem.childs) newConnectionItem.childs[key].remove();
                                        jsObject.options.dictionaryTree.addTreeItems(answer.itemObject.dataSources, newConnectionItem);
                                    }
                                    jsObject.options.dictionaryPanel.toolBar.updateControls();
                                }
                            }
                            if (jsObject.options.forms.wizardForm && jsObject.options.forms.wizardForm.visible) {
                                var dataSources = jsObject.options.report ? jsObject.GetDataSourcesAndBusinessObjectsFromDictionary(jsObject.options.report.dictionary) : null;
                                jsObject.options.forms.wizardForm.dataSourcesFromServer = dataSources;
                                jsObject.options.forms.wizardForm.onshow(true);
                            }
                            if (jsObject.options.forms.wizardForm2 && jsObject.options.forms.wizardForm2.visible) {
                                jsObject.options.forms.wizardForm2.stepPanel.update(true);
                            }
                            if (jsObject.options.forms.getDataForm && jsObject.options.forms.getDataForm.visible) {
                                jsObject.options.forms.getDataForm.updateData();
                            }
                            jsObject.UpdateStateUndoRedoButtons();
                        }
                        else if (jsObject.options.dictionaryPanel) {
                            jsObject.options.dictionaryPanel.showProgress(answer.progress, answer.commandGuid);
                        }
                    }
                    break;
                }
            case "StartEditBarCodeComponent":
                {
                    if (answer.barCode && !jsObject.options.previewMode) {
                        jsObject.InitializeBarCodeForm(function (form) {
                            form.barCode = answer.barCode;
                            form.changeVisibleState(true);
                        });
                    }
                    break;
                }
            case "StartEditShapeComponent":
                {
                    if (answer.shape && !jsObject.options.previewMode) {
                        jsObject.InitializeShapeForm(function (form) {
                            form.shape = answer.shape;
                            form.changeVisibleState(true);
                        });
                    }
                    break;
                }
            case "MoveDictionaryItem":
                {
                    if (answer.moveCompleted) {
                        var fromItem = jsObject.options.dictionaryTree.selectedItem;
                        var direction = answer.direction;
                        if (direction) {
                            var toItem = direction == "Down"
                                ? (fromItem.nextSibling || (fromItem && fromItem.itemObject.typeItem == "Variable"
                                    ? (fromItem.parent.nextSibling || fromItem.parent.parent) : null))
                                : (fromItem.previousSibling || (fromItem && fromItem.itemObject.typeItem == "Variable"
                                    ? (fromItem.parent.previousSibling || fromItem.parent.parent) : null))

                            if (toItem != null && fromItem) {
                                if (fromItem.itemObject.typeItem == "Variable" && (toItem.itemObject.typeItem == "Category" || toItem.itemObject.typeItem == "VariablesMainItem")) {
                                    fromItem.remove();
                                    var item = toItem.addChild(fromItem);
                                    item.setSelected();
                                    toItem.setOpening(true);
                                }
                                else if (fromItem.itemObject.typeItem == "Variable" && fromItem.parent.itemObject.typeItem == "Category" &&
                                    toItem.itemObject.typeItem == "Variable" && toItem.parent.itemObject.typeItem == "VariablesMainItem") {
                                    fromItem.remove();
                                    var item = toItem.parent.addChild(fromItem);
                                    item.setSelected();
                                }
                                else {
                                    fromItem.move(answer.direction);
                                    jsObject.options.report.dictionary.databases = answer.databases;
                                }
                                jsObject.options.dictionaryPanel.toolBar.updateControls();
                            }
                        }
                        else if (answer.fromObject && answer.toObject &&
                            (answer.fromObject.typeItem == "Variable" || answer.fromObject.typeItem == "Category") &&
                            (answer.toObject.typeItem == "Variable" || answer.toObject.typeItem == "Category")) {

                            //Drag & Drop Variables or Category
                            var dictionaryTree = jsObject.options.dictionaryTree;

                            //Save opening items
                            var openingCategories = {};
                            var allItems = dictionaryTree.mainItems["Variables"].getAllChilds();
                            for (var i = 0; i < allItems.length; i++) {
                                if (allItems[i].itemObject.typeItem == "Category" && allItems[i].isOpening) {
                                    openingCategories[allItems[i].itemObject.name] = true;
                                }
                            }

                            //repaint variables tree
                            while (dictionaryTree.mainItems["Variables"].childsContainer.childNodes[0]) {
                                dictionaryTree.mainItems["Variables"].childsContainer.childNodes[0].remove();
                            }
                            dictionaryTree.addTreeItems(answer.variablesTree, dictionaryTree.mainItems["Variables"]);

                            allItems = dictionaryTree.mainItems["Variables"].getAllChilds();
                            for (var i = 0; i < allItems.length; i++) {
                                if (allItems[i].itemObject.typeItem == "Category" && openingCategories[allItems[i].itemObject.name]) {
                                    allItems[i].setOpening(true);
                                }
                                if (allItems[i].itemObject.name == answer.fromObject.name) {
                                    allItems[i].setSelected();
                                    allItems[i].openTree();
                                }
                            }
                        }
                    }
                    break;
                }
            case "UpdateReportAliases":
                {
                    if (answer["reportGuid"] && answer["reportObject"]) {
                        jsObject.options.reportGuid = answer.reportGuid;
                        jsObject.LoadReport(jsObject.ParseReport(answer.reportObject), true);
                        jsObject.options.reportIsModified = true;
                    }
                    if (answer.selectedObjectName)
                        jsObject.BackToSelectedComponent(answer.selectedObjectName);
                    break;
                }
            case "MoveConnectionDataToResource":
                {
                    if (answer["resourcesTree"] && jsObject.options.dictionaryTree && jsObject.options.report) {
                        jsObject.options.report.dictionary.resources = answer["resourcesTree"];
                        var resMainItem = jsObject.options.dictionaryTree.mainItems["Resources"];
                        resMainItem.removeAllChilds();
                        jsObject.options.dictionaryTree.addTreeItems(answer["resourcesTree"], resMainItem);
                        resMainItem.setOpening(true);

                        var editConnectionForm = jsObject.options.forms.editConnectionForm;

                        if (answer["databases"]) {
                            if (editConnectionForm) editConnectionForm.changeVisibleState(false);

                            var dataMainItem = jsObject.options.dictionaryTree.mainItems["DataSources"];
                            dataMainItem.removeAllChilds();
                            jsObject.options.dictionaryTree.addTreeItems(answer["databases"], dataMainItem);
                            dataMainItem.setOpening(true);

                            jsObject.options.report.dictionary.databases = answer.databases;
                            jsObject.ClearAllGalleries();

                            if (answer.connectionObject) {
                                jsObject.InitializeSelectDataForm(function (selectDataForm) {
                                    selectDataForm.databaseName = answer.connectionObject.name;
                                    selectDataForm.typeConnection = answer.connectionObject.typeConnection;
                                    selectDataForm.connectionObject = answer.connectionObject;
                                    selectDataForm.changeVisibleState(true);
                                });
                            }
                        }
                        else {
                            if (editConnectionForm && editConnectionForm.visible) {
                                if (answer.pathSchema != null) editConnectionForm.pathSchemaControl.textBox.value = answer.pathSchema;
                                if (answer.pathData != null) editConnectionForm.pathDataControl.textBox.value = answer.pathData;
                                editConnectionForm.action();
                            }
                        }
                    }
                    break;
                }
            case "SetMapProperties":
            case "UpdateMapData":
                {
                    var mapComp = (jsObject.options.selectedObject && jsObject.options.selectedObject.properties.name == answer.componentName)
                        ? jsObject.options.selectedObject
                        : jsObject.options.report.getComponentByName(answer.componentName);

                    if (mapComp) {
                        mapComp.properties.svgContent = answer.svgContent;
                        mapComp.properties.iframeContent = answer.iframeContent;
                        if (answer.cultures) mapComp.properties.cultures = answer.cultures;

                        mapComp.repaint();
                    }

                    if (answer.mapData && jsObject.options.forms.editMapForm && jsObject.options.forms.editMapForm.visible) {
                        jsObject.options.forms.editMapForm.controls.dataGridView.fillData(answer.mapData);
                    }
                    break;
                }
            case "SetGaugeProperties":
                {
                    var gaugeComp = (jsObject.options.selectedObject && jsObject.options.selectedObject.properties.name == answer.componentName)
                        ? jsObject.options.selectedObject
                        : jsObject.options.report.getComponentByName(answer.componentName);

                    if (gaugeComp) {
                        gaugeComp.properties.svgContent = answer.svgContent;
                        gaugeComp.repaint();
                    }
                    break;
                }
            case "CreateTextElement":
            case "CreateDatePickerElement":
            case "CreateComboBoxElement":
            case "CreateTableElement":
            case "CreateElementFromResource":
                {
                    if (answer.newComponent) {
                        var component = jsObject.CreateDashboardElement(answer.newComponent);
                        if (component) {
                            component.repaint();
                            jsObject.options.report.pages[component.properties.pageName].appendChild(component);
                            jsObject.options.report.pages[component.properties.pageName].components[component.properties.name] = component;
                            jsObject.options.report.pages[component.properties.pageName].rebuild(answer.rebuildProps);
                            component.setOnTopLevel();
                            component.setSelected();
                            jsObject.UpdatePropertiesControls();
                            jsObject.UpdateStateUndoRedoButtons();
                            if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                            if (jsObject.options.report.info.runDesignerAfterInsert) jsObject.ShowComponentForm(component);
                        }
                    }
                    break;
                }
            case "ChangeDashboardStyle":
                {
                    if (answer.elementsProperties) {
                        var dashboard = jsObject.options.report.pages[answer.dashboardName];
                        if (dashboard) {
                            if (answer.dashboardProperties) {
                                jsObject.WriteAllProperties(dashboard, answer.dashboardProperties);
                                dashboard.repaint(true);
                            }
                            for (var elementName in answer.elementsProperties) {
                                var element = dashboard.components[elementName];
                                if (element) {
                                    jsObject.WriteAllProperties(element, answer.elementsProperties[elementName]);
                                    element.repaint();
                                }
                            }
                            jsObject.UpdatePropertiesControls();
                        }
                    }
                    break;
                }
            case "SetPreviewSettings":
                {
                    if (answer.resultPreviewSettings && jsObject.options.report) {
                        jsObject.options.report.properties.previewSettings = answer.resultPreviewSettings;
                    }
                    break;
                }
            case "RepaintAllDbsElements":
                {
                    if (answer.svgContents)
                        jsObject.options.report.repaintAllSvgContents(answer.svgContents);
                    break;
                }
            case "ChangeTypeElement":
                {
                    var newCompProps = answer.newComponentProps;
                    if (jsObject.options.report && newCompProps) {
                        var comp = jsObject.options.report.getComponentByName(newCompProps.name);
                        if (comp) {
                            jsObject.CreateComponentProperties(comp, newCompProps);
                            comp.repaint();
                            jsObject.UpdatePropertiesControls();
                            jsObject.UpdateStateUndoRedoButtons();
                            if (jsObject.options.reportTree) jsObject.options.reportTree.build();
                        }
                        if (!comp.originalElementContent) comp.originalElementContent = answer.originalElementContent;
                    }
                    break;
                }
            case "ChangeDashboardViewMode":
                {
                    if (answer.pageName) {
                        var page = jsObject.options.report.pages[answer.pageName];
                        if (page) {
                            jsObject.WriteAllProperties(page, answer.pageProperties);
                            page.repaint(true);
                            page.rebuild(answer.rebuildProps);

                            if (answer.svgContents) {
                                for (var i = 0; i < answer.svgContents.length; i++) {
                                    var componentName = answer.svgContents[i].name;
                                    var component = page.components[componentName];
                                    if (component) {
                                        component.properties.svgContent = answer.svgContents[i].svgContent;
                                        component.repaint();
                                    }
                                }
                            }

                            jsObject.options.homePanel.updateControls();
                            if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
                            if (jsObject.options.pagePanel) jsObject.options.pagePanel.updateControls();
                            if (jsObject.options.reportTree) jsObject.options.reportTree.build();

                            if (page.properties.mobileViewModePresent) {
                                jsObject.SendCommandToDesignerServer("GetMobileViewUnplacedElements", { dashboardName: answer.pageName }, function (answer) {
                                    if (answer.elements && answer.elements.length > 0) {
                                        jsObject.InitializeMobileViewComponentsForm(function (form) {
                                            form.show(answer.elements);
                                        });
                                    }
                                    else if (jsObject.options.forms.mobileViewComponentsForm && jsObject.options.forms.mobileViewComponentsForm.visible) {
                                        jsObject.options.forms.mobileViewComponentsForm.changeVisibleState(false);
                                    }
                                });
                            }
                        }
                    }
                    break;
                }
            default: {
                if (answer.command && answer.callbackFunctionId && jsObject.options.callbackFunctions[answer["callbackFunctionId"]] != null) {
                    jsObject.options.callbackFunctions[answer["callbackFunctionId"]](answer);
                    delete jsObject.options.callbackFunctions[answer["callbackFunctionId"]];
                    break;
                }
            }
        }
    }

    if (jsObject.options.dictionaryPanel && jsObject.ShowDictionaryLoadProcess(answer.command) && answer.loadingCompleted !== false) {
        setTimeout(function () { jsObject.options.dictionaryPanel.hideProgress(); }, answer.progress ? 100 : 0);
    }

    if (jsObject.options.commands.length >= 1) {
        jsObject.options.commands.splice(0, 1);
    }

    if (jsObject.options.commands.length == 0) {
        if (jsObject.options.processImageStatusPanel) {
            jsObject.options.processImageStatusPanel.hide();
        }
        if (jsObject.options.processImage && answer.loadingCompleted !== false && !jsObject.IgnoreLoadProcess(answer.command)) {
            setTimeout(function () { jsObject.options.processImage.hide(); }, answer.progress ? 100 : 0);
        }
    }
    else {
        jsObject.ExecuteCommandFromStack();
    }
}

//Send Create Report
StiMobileDesigner.prototype.SendCommandCreateReport = function (callbackFunction, needClearAfterOldReport) {
    var params = {
        command: "CreateReport",
        defaultUnit: this.options.defaultUnit,
        callbackFunctionId: this.generateKey(),
        designerOptions: this.GetCookie("StimulsoftMobileDesignerOptions")
    };
    if (needClearAfterOldReport) params.needClearAfterOldReport = true;
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;
    if (this.options.controls.zoomScale.value) params.zoom = this.options.controls.zoomScale.value;
    this.AddCommandToStack(params);
}

//Send Create Dashboard
StiMobileDesigner.prototype.SendCommandCreateDashboard = function (callbackFunction, needClearAfterOldReport) {
    var params = {
        command: "CreateDashboard",
        callbackFunctionId: this.generateKey(),
        isDashboard: true,
        designerOptions: this.GetCookie("StimulsoftMobileDesignerOptions")
    };
    if (needClearAfterOldReport) params.needClearAfterOldReport = true;
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;
    if (this.options.controls.zoomScale.value) params.zoom = this.options.controls.zoomScale.value;

    this.AddCommandToStack(params);
}

//Send Create Form
StiMobileDesigner.prototype.SendCommandCreateForm = function () {
    var params = {
        command: "CreateForm",
        isForm: true,
        designerOptions: this.GetCookie("StimulsoftMobileDesignerOptions")
    };
    if (this.options.controls.zoomScale.value) params.zoom = this.options.controls.zoomScale.value;

    this.AddCommandToStack(params);
}

//Send Open Report
StiMobileDesigner.prototype.SendCommandOpenReport = function (fileContent, fileName, reportParams, filePath, fileSize, addToRecent) {
    var commandGuid = this.generateKey();
    var params = {
        command: "OpenReport",
        openReportFile: fileName,
        filePath: filePath,
        fileSize: fileSize,
        password: reportParams.password,
        isPacked: reportParams.isPacked,
        designerOptions: this.GetCookie("StimulsoftMobileDesignerOptions"),
        addToRecent: addToRecent
    };
    if (fileContent) params.base64Data = fileContent.indexOf("base64,") >= 0 ? fileContent.substr(fileContent.indexOf("base64,") + 7) : fileContent;
    if (this.options.controls.zoomScale.value) params.zoom = this.options.controls.zoomScale.value;

    if (!this.options.jsMode && params.base64Data && params.base64Data.length > this.options.uploadBlockSize) {
        var contentStr = params.base64Data;
        var countBlocks = parseInt(contentStr.length / this.options.uploadBlockSize) + 1;
        var cacheGuid = this.generateKey();
        delete params.base64Data;

        for (var i = 0; i < countBlocks; i++) {
            var params_ = this.CopyObject(params);
            params_.cacheGuid = cacheGuid;
            params_.countBlocks = countBlocks;
            params_.blockContent = i + contentStr.substr(i * this.options.uploadBlockSize, this.options.uploadBlockSize);
            params_.progress = i / countBlocks;
            this.AddCommandToStack(params_, commandGuid);
        }
    }
    else {
        this.AddCommandToStack(params, commandGuid);
    }
}

//Send Save Report
StiMobileDesigner.prototype.SendCommandSaveReport = function (isNewReport) {
    this.options.reportIsModified = false;
    if (this.options.haveSaveEvent) {
        var params = {
            command: "SaveReport",
            reportFile: this.options.report.properties.reportFile,
            isNewReport: isNewReport
        };
        if (this.options.report.encryptedPassword)
            params.encryptedPassword = this.options.report.encryptedPassword;

        if (this.options.saveReportMode == "Hidden") {
            this.AddCommandToStack(params);
        }
        else if (this.options.saveReportMode == "Visible") {
            this.options.ignoreBeforeUnload = true;
            this.PostForm(params);
            this.options.ignoreBeforeUnload = false;
        }
        else if (this.options.saveReportMode == "NewWindow") {
            this.options.ignoreBeforeUnload = true;
            var win = this.openNewWindow("about:blank", '_blank');
            this.PostForm(params, win ? win.document : document);
            this.options.ignoreBeforeUnload = false;
        }
    }
    else {
        var jsObject = this;
        this.options.ignoreBeforeUnload = true;
        var params = {
            command: "DownloadReport",
            reportFile: this.options.report.properties.reportFile,
            reportGuid: this.options.reportGuid,
            isNewReport: isNewReport
        };
        if (this.options.report.encryptedPassword) params.encryptedPassword = this.options.report.encryptedPassword;
        this.PostForm(params);
        setTimeout(function () { jsObject.options.ignoreBeforeUnload = false; }, 500);
    }
}

//Send Save As Report
StiMobileDesigner.prototype.SendCommandSaveAsReport = function (saveType, isNewReport) {
    this.options.reportIsModified = false;

    if (this.options.haveSaveAsEvent) {
        var params = {
            command: "SaveAsReport",
            reportFile: this.options.report.properties.reportFile,
            isNewReport: isNewReport
        };
        if (this.options.report.encryptedPassword)
            params.encryptedPassword = this.options.report.encryptedPassword;

        if (saveType)
            params.saveType = saveType;

        if (this.options.saveReportAsMode == "Hidden") {
            this.AddCommandToStack(params);
        }
        else if (this.options.saveReportAsMode == "Visible") {
            this.options.ignoreBeforeUnload = true;
            this.PostForm(params);
            this.options.ignoreBeforeUnload = false;
        }
        else if (this.options.saveReportAsMode == "NewWindow") {
            this.options.ignoreBeforeUnload = true;
            var win = this.openNewWindow("about:blank", '_blank');
            this.PostForm(params, win ? win.document : document);
            this.options.ignoreBeforeUnload = false;
        }
    }
    else {
        var jsObject = this;
        this.options.ignoreBeforeUnload = true;
        var params = {
            command: "DownloadReport",
            reportFile: this.options.report.properties.reportFile,
            reportGuid: this.options.reportGuid,
            isNewReport: isNewReport
        };
        if (this.options.report.encryptedPassword) params.encryptedPassword = this.options.report.encryptedPassword;
        if (saveType) params.saveType = saveType;
        this.PostForm(params);
        setTimeout(function () { jsObject.options.ignoreBeforeUnload = false; }, 500);
    }
}

//Send Close Report
StiMobileDesigner.prototype.SendCommandCloseReport = function () {
    var params = {};
    params.command = "CloseReport";
    this.AddCommandToStack(params);
}

//Send Change Rect Component
StiMobileDesigner.prototype.SendCommandChangeRectComponent = function (component, command, runFromProperty, resizeType, isUnplacedComponent) {
    if (!component) return;
    var params = {};
    params.command = command;
    params.zoom = this.options.report.zoom.toString();
    params.runFromProperty = runFromProperty;
    params.isUnplacedComponent = isUnplacedComponent;
    params.resizeType = resizeType;
    params.components = [];
    if (this.options.in_drag && this.options.in_drag.length > 4) params.moveAfterCopyPaste = true;

    var components = this.Is_array(component) ? component : [component];
    for (var i = 0; i < components.length; i++) {
        if (command == "MoveComponent" && this.IsTableCell(components[i])) continue;
        if (components[i].properties.unitLeft == null || components[i].properties.unitTop == null ||
            components[i].properties.unitWidth == null || components[i].properties.unitHeight == null)
            continue;
        if (!params.pageName) params.pageName = components[i].properties.pageName;
        var compParams = {
            componentName: components[i].properties.name,
            invertWidth: components[i].properties.invertWidth,
            invertHeight: components[i].properties.invertHeight,
            componentRect: components[i].properties.unitLeft + "!" + components[i].properties.unitTop + "!" +
                components[i].properties.unitWidth + "!" + components[i].properties.unitHeight
        }
        if (params.moveAfterCopyPaste) {
            var ignoreThisComp = false;
            for (var k = 0; k < components.length; k++) {
                if (components[k].properties.childs) {
                    var childs = components[k].properties.childs.split(",");
                    if (childs.indexOf(compParams.componentName) >= 0) {
                        ignoreThisComp = true;
                        break;
                    }
                }
            }
            if (ignoreThisComp) continue;
        }
        params.components.push(compParams);
    }
    if (params.components.length == 0) return;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Create Component
StiMobileDesigner.prototype.SendCommandCreateComponent = function (pageName, typeComponent, componentRect, additionalParams) {
    var params = {};
    params.command = "CreateComponent";
    params.pageName = pageName;
    params.typeComponent = typeComponent;
    params.componentRect = componentRect;
    params.zoom = this.options.report.zoom.toString();
    params.chartEditorType = this.options.designerSpecification == "Beginner" ? "Simple" : this.options.chartEditorType;
    if (additionalParams) params.additionalParams = additionalParams;

    if (this.options.report.info.useLastFormat && this.options.lastStyleProperties && !this.IsBandComponent(params) && !this.EndsWith(typeComponent, "Element")) {
        params.lastStyleProperties = this.options.lastStyleProperties;
    }

    if (typeComponent == "StiTableOfContents" && this.options.report && this.options.report.tableOfContentsPresent()) {
        var messageForm = this.options.forms.errorMessageForm || this.InitializeErrorMessageForm();
        messageForm.show(this.loc.Errors.OneTableOfContentsAllowed, "Warning");
        return;
    }

    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Remove Component
StiMobileDesigner.prototype.SendCommandRemoveComponent = function (component) {
    var params = {};
    params.command = "RemoveComponent";
    params.components = [];

    var components = this.Is_array(component) ? component : [component];
    for (var i = 0; i < components.length; i++) {
        params.components.push(components[i].properties.name);
        this.RemoveStylesFromCache(components[i].properties.name, components[i].typeComponent);
    }
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Add Page
StiMobileDesigner.prototype.SendCommandAddPage = function (pageIndex) {
    var params = {};
    params.command = "AddPage";
    params.pageIndex = pageIndex.toString();
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Add Dashboard
StiMobileDesigner.prototype.SendCommandAddDashboard = function (pageIndex) {
    var params = {};
    params.command = "AddDashboard";
    params.pageIndex = pageIndex.toString();
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Remove Page
StiMobileDesigner.prototype.SendCommandRemovePage = function (page) {
    var params = {};
    params.command = "RemovePage";
    params.pageName = page.properties.name;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Rebuild Page
StiMobileDesigner.prototype.SendCommandRebuildPage = function (page) {
    var params = {};
    params.command = "RebuildPage";
    params.pageName = page.properties.name;
    this.AddCommandToStack(params);
}

//Send Change Unit
StiMobileDesigner.prototype.SendCommandChangeUnit = function (reportUnit) {
    var params = {};
    params.command = "ChangeUnit";
    params.reportUnit = reportUnit;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Properties
StiMobileDesigner.prototype.SendCommandSendProperties = function (object, propertiesNames, updateAllControls) {
    var objects = this.Is_array(object) ? object : [object];

    if (objects.length > 0 && objects[0].typeComponent == "StiCrossField" && propertiesNames) {
        for (var i = 0; i < propertiesNames.length; i++) {
            this.ApplyCrossTabFieldProperty(objects[0], propertiesNames[i], objects[0].properties[propertiesNames[i]]);
        }
    }
    else {
        var params = {};
        params.command = "SendProperties";
        params.updateAllControls = updateAllControls ? true : false;
        params.zoom = this.options.report.zoom.toString();
        params.components = [];
        this.options.updateLastStyleProperties = false;

        var stylePropertyNames = ["border", "brush", "backColor", "textBrush", "font", "horAlignment", "vertAlignment"];

        for (var i = 0; i < objects.length; i++) {
            var cannotChange = objects[i].properties.restrictions && !(objects[i].properties.restrictions == "All" || objects[i].properties.restrictions.indexOf("AllowChange") >= 0);
            var properties = [];
            var component = {};

            for (var num = 0; num < propertiesNames.length; num++) {
                if (typeof (objects[i].properties[propertiesNames[num]]) != "undefined") {
                    properties.push({
                        name: propertiesNames[num],
                        value: objects[i].properties[propertiesNames[num]]
                    });

                    if (propertiesNames[num] == "restrictions") cannotChange = false;
                    if (stylePropertyNames.indexOf(propertiesNames[num]) >= 0) this.options.updateLastStyleProperties = true;
                }
            }

            component.typeComponent = objects[i].typeComponent;
            component.componentName = objects[i].properties.name;
            component.properties = properties;
            if (cannotChange) component.cannotChange = true;
            params.components.push(component);
        }

        this.options.reportIsModified = true;
        this.AddCommandToStack(params);
    }
}

//Send Get Preview Pages
StiMobileDesigner.prototype.SendCommandLoadReportToViewer = function () {
    if (this.options.report == null) return;
    (this.options.viewer.jsObject.controls || this.options.viewer.jsObject.options).reportPanel.clear();
    (this.options.viewer.jsObject.controls || this.options.viewer.jsObject.options).processImage.show();

    var params = {
        command: "LoadReportToViewer",
        viewerClientGuid: this.options.viewer.jsObject.options.clientGuid,
        checkReportBeforePreview: this.options.checkReportBeforePreview
    };
    this.AddCommandToStack(params);
}

//Set To Clipboard
StiMobileDesigner.prototype.SendCommandSetToClipboard = function (component) {
    if (this.options.movingCloneComponents) return;
    this.options.clipboard = true;
    var params = {};
    params.command = "SetToClipboard";
    params.pageName = this.options.currentPage.properties.name;
    params.components = [];
    var components = this.Is_array(component) ? component : [component];
    for (var i = 0; i < components.length; i++) {
        params.components.push(components[i].properties.name);
    }
    this.AddCommandToStack(params);
}

//Get From Clipboard
StiMobileDesigner.prototype.SendCommandGetFromClipboard = function (clipboardResult) {
    var jsObject = this;
    if (this.options.movingCloneComponents) return;
    var params = {};
    params.command = "GetFromClipboard";
    params.pageName = this.options.currentPage.properties.name;
    params.zoom = this.options.report.zoom.toString();

    if (clipboardResult && clipboardResult.indexOf("stiClipboard_") == 0) {
        params.clipboardResult = clipboardResult;
    }
    jsObject.AddCommandToStack(params);
}

//Send Synchronize
StiMobileDesigner.prototype.SendCommandSynchronization = function () {
    var params = {};
    params.command = "Synchronization";
    this.AddCommandToStack(params);
}

//Send Undo
StiMobileDesigner.prototype.SendCommandUndo = function () {
    var params = {};
    params.command = "Undo";
    params.zoom = this.options.report.zoom.toString();
    params.selectedObjectName = this.options.selectedObject ? this.options.selectedObject.properties.name : null;
    params.designerOptions = this.GetCookie("StimulsoftMobileDesignerOptions");
    this.AddCommandToStack(params);
}

//Send Redo
StiMobileDesigner.prototype.SendCommandRedo = function () {
    var params = {};
    params.command = "Redo";
    params.zoom = this.options.report.zoom.toString();
    params.selectedObjectName = this.options.selectedObject ? this.options.selectedObject.properties.name : null;
    params.designerOptions = this.GetCookie("StimulsoftMobileDesignerOptions");
    this.AddCommandToStack(params);
}

//Send Rename Component
StiMobileDesigner.prototype.SendCommandRenameComponent = function (component, newName) {
    var params = {};
    params.command = "RenameComponent";
    params.typeComponent = component.typeComponent;
    params.oldName = component.properties.name;
    params.newName = newName;
    this.RemoveStylesFromCache(params.oldName, component.typeComponent);
    this.RemoveStylesFromCache(params.newName, component.typeComponent);
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Wizard Result
StiMobileDesigner.prototype.SendCommandWizardResult = function (wizardResult) {
    var params = {
        command: "WizardResult",
        defaultUnit: this.options.defaultUnit,
        wizardResult: wizardResult,
        designerOptions: this.GetCookie("StimulsoftMobileDesignerOptions")
    };
    if (this.options.serverMode && this.options.cloudParameters && this.options.report && this.options.report.dictionary.attachedItems) {
        params.attachedItems = this.options.report.getAttachedItems();
        params.sessionKey = this.options.cloudParameters.sessionKey;
    }
    if (this.options.controls.zoomScale.value) params.zoom = this.options.controls.zoomScale.value;

    var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
    fileMenu.changeVisibleState(false);
    this.AddCommandToStack(params);
}

//Send ExitDesigner
StiMobileDesigner.prototype.SendCommandExitDesigner = function () {
    this.PostForm({ command: "ExitDesigner" });
}

//Send Synchronize Dictionary
StiMobileDesigner.prototype.SendCommandSynchronizeDictionary = function () {
    var params = {};
    params.command = "SynchronizeDictionary";
    this.AddCommandToStack(params);
}

//Send New Dictionary
StiMobileDesigner.prototype.SendCommandNewDictionary = function () {
    var params = {};
    params.command = "NewDictionary";
    this.AddCommandToStack(params);
}

//Send Open Dictionary
StiMobileDesigner.prototype.SendCommandOpenDictionary = function (fileContent, fileName) {
    var params = {
        command: "OpenDictionary"
    };
    if (fileContent) params.base64Data = fileContent.substr(fileContent.indexOf("base64,") + 7);
    this.AddCommandToStack(params);
}

//Send Merge Dictionary
StiMobileDesigner.prototype.SendCommandMergeDictionary = function (fileContent, fileName) {
    var params = {
        command: "MergeDictionary"
    };
    if (fileContent) params.base64Data = fileContent.substr(fileContent.indexOf("base64,") + 7);
    this.AddCommandToStack(params);
}

//Send Create Or Edit Connection
StiMobileDesigner.prototype.SendCommandCreateOrEditConnection = function (connectionFormResult) {
    var params = {};
    params.command = "CreateOrEditConnection";
    params.connectionFormResult = connectionFormResult;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Delete Connection
StiMobileDesigner.prototype.SendCommandDeleteConnection = function (selectedItem) {
    var params = {};
    params.command = "DeleteConnection";
    params.connectionName = selectedItem.itemObject.name;
    params.dataSourceNames = selectedItem.getChildNames();
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Create Or Edit Relation
StiMobileDesigner.prototype.SendCommandCreateOrEditRelation = function (relationFormResult) {
    var params = {};
    params.command = "CreateOrEditRelation";
    params.relationFormResult = relationFormResult;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Delete Relation
StiMobileDesigner.prototype.SendCommandDeleteRelation = function (selectedItem) {
    var params = {};
    params.command = "DeleteRelation";
    params.relationNameInSource = selectedItem.itemObject.nameInSource;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Create Or Edit Column
StiMobileDesigner.prototype.SendCommandCreateOrEditColumn = function (columnFormResult) {
    var params = {};
    params.command = "CreateOrEditColumn";
    params.columnFormResult = columnFormResult;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Delete Column
StiMobileDesigner.prototype.SendCommandDeleteColumn = function (selectedItem) {
    var params = {};
    params.command = "DeleteColumn";
    params.columnName = selectedItem.itemObject.name;
    var columnParent = this.options.dictionaryTree.getCurrentColumnParent();
    params.currentParentType = columnParent.type;
    params.currentParentName = (columnParent.type == "BusinessObject") ? selectedItem.getBusinessObjectFullName() : columnParent.name;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Create Or Edit Parameter
StiMobileDesigner.prototype.SendCommandCreateOrEditParameter = function (parameterFormResult) {
    var params = {};
    params.command = "CreateOrEditParameter";
    params.parameterFormResult = parameterFormResult;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Delete Parameter
StiMobileDesigner.prototype.SendCommandDeleteParameter = function (selectedItem) {
    var params = {};
    params.command = "DeleteParameter";
    params.parameterName = selectedItem.itemObject.name;
    var parameterParent = this.options.dictionaryTree.getCurrentColumnParent();
    params.currentParentName = parameterParent.name;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Create Or Edit Data Source
StiMobileDesigner.prototype.SendCommandCreateOrEditDataSource = function (dataSourceFormResult) {
    var params = {};
    params.command = "CreateOrEditDataSource";
    params.dataSourceFormResult = dataSourceFormResult;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Delete DataSource
StiMobileDesigner.prototype.SendCommandDeleteDataSource = function (selectedItem) {
    var params = {};
    params.command = "DeleteDataSource";
    params.dataSourceName = selectedItem.itemObject.name;
    params.dataSourceNameInSource = selectedItem.itemObject.nameInSource;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Create Or Edit BusinessObject
StiMobileDesigner.prototype.SendCommandCreateOrEditBusinessObject = function (businessObjectFormResult) {
    var params = {};
    params.command = "CreateOrEditBusinessObject";
    params.businessObjectFormResult = businessObjectFormResult;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Delete BusinessObject
StiMobileDesigner.prototype.SendCommandDeleteBusinessObject = function (selectedItem) {
    var params = {};
    params.command = "DeleteBusinessObject";
    params.businessObjectFullName = selectedItem.getBusinessObjectFullName();
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Get All Connections
StiMobileDesigner.prototype.SendCommandGetAllConnections = function (typeDataAdapter) {
    var params = {};
    params.command = "GetAllConnections";
    params.typeDataAdapter = typeDataAdapter;
    this.AddCommandToStack(params);
}

//Send RetrieveColumns
StiMobileDesigner.prototype.SendCommandRetrieveColumns = function (params) {
    params.command = "RetrieveColumns";
    this.AddCommandToStack(params);
}

//Send Delete Category
StiMobileDesigner.prototype.SendCommandDeleteCategory = function (selectedItem) {
    var params = {};
    params.command = selectedItem.parent.itemObject.typeItem == "VariablesMainItem"
        ? "DeleteVariablesCategory"
        : "DeleteBusinessObjectCategory";
    params.categoryName = selectedItem.itemObject.name;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Edit Category
StiMobileDesigner.prototype.SendCommandEditCategory = function (categoryFormResult) {
    var params = {};
    params.command = this.options.dictionaryTree.selectedItem.parent.itemObject.typeItem == "VariablesMainItem"
        ? "EditVariablesCategory"
        : "EditBusinessObjectCategory";
    params.categoryFormResult = categoryFormResult;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Update Cache
StiMobileDesigner.prototype.SendCommandUpdateCache = function () {
    var jsObject = this;
    if (this.options.allowAutoUpdateCache) {
        this.options.timerUpdateCache = setTimeout(function () {
            jsObject.SendCommandUpdateCache();
        }, this.options.timeUpdateCache);

        var params = {};
        params.command = "UpdateCache";
        this.AddCommandToStack(params);
    }
}

//Send Create Or Edit Variable
StiMobileDesigner.prototype.SendCommandCreateOrEditVariable = function (variableFormResult) {
    var params = {};
    params.command = "CreateOrEditVariable";
    params.variableFormResult = variableFormResult;
    if (this.options.currentPage) params.pageName = this.options.currentPage.properties.name;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Delete Variable
StiMobileDesigner.prototype.SendCommandDeleteVariable = function (selectedItem) {
    var params = {};
    params.command = "DeleteVariable";
    params.variableName = selectedItem.itemObject.name;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Create Variables Category
StiMobileDesigner.prototype.SendCommandCreateVariablesCategory = function (categoryFormResult) {
    var params = {};
    params.command = "CreateVariablesCategory";
    params.categoryFormResult = categoryFormResult;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Create Or Edit Resource
StiMobileDesigner.prototype.SendCommandCreateOrEditResource = function (resourceFormResult, callbackFunction) {
    var commandGuid = this.generateKey();

    var params = {
        command: "CreateOrEditResource",
        resourceFormResult: resourceFormResult,
        isAsyncCommand: true
    };

    if (callbackFunction) {
        params.callbackFunctionId = this.generateKey();
        this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;
    }

    if (!this.options.jsMode && resourceFormResult.loadedContent && resourceFormResult.loadedContent.length > this.options.uploadBlockSize) {
        var contentStr = resourceFormResult.loadedContent;
        delete params.resourceFormResult.loadedContent;

        var countBlocks = parseInt(contentStr.length / this.options.uploadBlockSize) + 1;
        var cacheGuid = this.generateKey();

        for (var i = 0; i < countBlocks; i++) {
            var params_ = this.CopyObject(params);
            params_.cacheGuid = cacheGuid;
            params_.countBlocks = countBlocks;
            params_.blockContent = i + contentStr.substr(i * this.options.uploadBlockSize, this.options.uploadBlockSize);
            this.AddCommandToStack(params_, commandGuid);
        }
    }
    else {
        this.AddCommandToStack(params, commandGuid);
    }

    this.options.reportIsModified = true;
}

//Send Delete Resource
StiMobileDesigner.prototype.SendCommandDeleteResource = function (selectedItem, ignoreCheckUsed) {
    var params = {};
    params.command = "DeleteResource";
    params.resourceName = selectedItem.itemObject.name;
    if (ignoreCheckUsed) params.ignoreCheckUsed = true;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Update Styles
StiMobileDesigner.prototype.SendCommandUpdateStyles = function (stylesCollection, collectionName) {
    this.options.reportIsModified = true;
    this.options.stylesIsModified = true;
    var params = {};
    params.command = "UpdateStyles";
    if (stylesCollection) params.stylesCollection = stylesCollection;
    if (collectionName) params.collectionName = collectionName;
    params.zoom = this.options.report.zoom.toString();
    if (this.options.selectedObject) params.selectedObjectName = this.options.selectedObject.properties.name;
    this.AddCommandToStack(params);
}

//Send Create Style Collection
StiMobileDesigner.prototype.SendCommandCreateStyleCollection = function (styleCollectionProperties) {
    this.options.reportIsModified = true;
    var params = {};
    params.command = "CreateStyleCollection";
    params.styleCollectionProperties = styleCollectionProperties;
    this.AddCommandToStack(params);
}

//Send Item Resource Save
StiMobileDesigner.prototype.SendCommandItemResourceSave = function (itemKey, customMessage, formContent) {
    var params = {
        command: "ItemResourceSave",
        reportTemplateItemKey: itemKey,
        sessionKey: this.options.cloudParameters.sessionKey,
        cloudMode: this.options.cloudMode
    }
    if (customMessage) {
        params.customMessage = customMessage;
    }
    if (this.options.formsDesignerMode) {
        if (formContent == null) {
            this.InitializeFormsDesignerFrame(function (frame) {
                frame.sendCommand({ action: "itemResourceSave", params: { itemKey: itemKey, customMessage: customMessage } });
            });
        }
        else {
            params.formContent = StiBase64.encode(formContent);
            this.AddCommandToStack(params);
        }
    }
    else {
        this.AddCommandToStack(params);
    }
}

//Send Clone Item Resource Save
StiMobileDesigner.prototype.SendCommandCloneItemResourceSave = function (params) {
    params.command = "CloneItemResourceSave";

    this.AddCommandToStack(params);
}

//Send Clone Chart Component
StiMobileDesigner.prototype.SendCommandStartEditChartComponent = function (componentName, formName) {
    var params = {};
    params.componentName = componentName;
    params.formName = formName;
    params.command = "StartEditChartComponent";
    this.AddCommandToStack(params);
}

//Send Clone Chart Element
StiMobileDesigner.prototype.SendCommandStartEditChartElement = function (componentName, formName) {
    var params = {};
    params.componentName = componentName;
    params.formName = formName;
    params.command = "StartEditChartElement";
    this.AddCommandToStack(params);
}

//Send Clone Map Component
StiMobileDesigner.prototype.SendCommandStartEditMapComponent = function (componentName, additionalParams) {
    var params = {
        command: "StartEditMapComponent",
        componentName: componentName,
        additionalParams: additionalParams,
        zoom: this.options.report.zoom.toString()
    }
    this.AddCommandToStack(params);
}

// Send Canceled Edit Component
StiMobileDesigner.prototype.SendCommandCanceledEditComponent = function (componentName) {
    var params = {};
    params.componentName = componentName;
    params.command = "CanceledEditComponent";
    this.AddCommandToStack(params);
}

// Send Add Series
StiMobileDesigner.prototype.SendCommandAddSeries = function (params) {
    params.command = "AddSeries";
    this.AddCommandToStack(params);
}

// Send Remove Series
StiMobileDesigner.prototype.SendCommandRemoveSeries = function (params) {
    params.command = "RemoveSeries";
    this.AddCommandToStack(params);
}

// Send Series Move
StiMobileDesigner.prototype.SendCommandSeriesMove = function (params) {
    params.command = "SeriesMove";
    this.AddCommandToStack(params);
}

// Get Styles Content
StiMobileDesigner.prototype.SendCommandGetStylesContent = function (params) {
    params.command = "GetStylesContent";
    this.AddCommandToStack(params);
}

// Set Labels Type
StiMobileDesigner.prototype.SendCommandSetLabelsType = function (params) {
    params.command = "SetLabelsType";
    this.AddCommandToStack(params);
}

// Set Chart Style
StiMobileDesigner.prototype.SendCommandSetChartStyle = function (params) {
    params.command = "SetChartStyle";
    this.AddCommandToStack(params);
}

// Set Chart Property Value
StiMobileDesigner.prototype.SendCommandSetChartPropertyValue = function (params) {
    params.command = "SetChartPropertyValue";
    this.AddCommandToStack(params);
}

// Send Add ConstantLine Or Strip
StiMobileDesigner.prototype.SendCommandAddConstantLineOrStrip = function (params) {
    params.command = "AddConstantLineOrStrip";
    this.AddCommandToStack(params);
}

// Send Remove ConstantLine Or Strip
StiMobileDesigner.prototype.SendCommandRemoveConstantLineOrStrip = function (params) {
    params.command = "RemoveConstantLineOrStrip";
    this.AddCommandToStack(params);
}

// Send ConstantLine Or Strip Move
StiMobileDesigner.prototype.SendCommandConstantLineOrStripMove = function (params) {
    params.command = "ConstantLineOrStripMove";
    this.AddCommandToStack(params);
}

// Send Container Value
StiMobileDesigner.prototype.SendCommandSendContainerValue = function (params) {
    params.command = "SendContainerValue";
    this.AddCommandToStack(params);
}

// Apply Selected Data
StiMobileDesigner.prototype.SendCommandApplySelectedData = function (data, databaseName) {
    var params = {
        command: "ApplySelectedData",
        data: data,
        databaseName: databaseName
    }
    this.AddCommandToStack(params);
}

// Create Text Component
StiMobileDesigner.prototype.SendCommandCreateTextComponent = function (itemObject, point, pageName) {
    this.options.reportIsModified = true;
    var params = {
        command: "CreateTextComponent",
        itemObject: itemObject,
        pageName: pageName,
        zoom: this.options.report.zoom.toString(),
        useAliases: this.options.useAliases,
        point: point
    }

    if (this.options.dictionaryPanel) {
        params.createLabel = this.options.menus.dictionarySettingsMenu.controls.createLabel.isChecked;
    }

    if (this.options.report.info.useLastFormat && this.options.lastStyleProperties) {
        params.lastStyleProperties = this.options.lastStyleProperties;
    }

    this.AddCommandToStack(params);
}

// Create Data Component
StiMobileDesigner.prototype.SendCommandCreateDataComponent = function (params) {
    this.options.reportIsModified = true;
    params.command = "CreateDataComponent";
    params.zoom = this.options.report.zoom.toString();
    this.AddCommandToStack(params);
}

//Send Report Properties
StiMobileDesigner.prototype.SendCommandSetReportProperties = function (propertiesNames) {
    var properties = {};
    var report = this.options.report;
    if (!report) return;

    for (var i = 0; i < propertiesNames.length; i++) {
        if (typeof (report.properties[propertiesNames[i]]) != "undefined") {
            properties[propertiesNames[i]] = report.properties[propertiesNames[i]];
        }
    }

    var params = {};
    params.command = "SetReportProperties";
    params.properties = properties;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Page Move
StiMobileDesigner.prototype.SendCommandPageMove = function (direction, pageIndex) {
    var params = {};
    params.command = "PageMove";
    params.direction = direction;
    params.pageIndex = pageIndex;
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Command Test Connection
StiMobileDesigner.prototype.SendCommandTestConnection = function (connection, connectionString, serviceName) {
    var params = {};
    params.command = "TestConnection";
    params.typeConnection = connection.typeConnection;
    params.connectionString = connectionString;
    params.serviceName = serviceName;
    this.AddCommandToStack(params);
}

//Send Command Run Query Script
StiMobileDesigner.prototype.SendCommandRunQueryScript = function (params) {
    params.command = "RunQueryScript";
    this.AddCommandToStack(params);
}

//Send Command View Data
StiMobileDesigner.prototype.SendCommandApplyDesignerOptions = function (designerOptions) {
    var params = {
        command: "ApplyDesignerOptions",
        designerOptions: designerOptions,
        zoom: this.options.report.zoom.toString(),
        selectedObjectName: this.options.selectedObject ? this.options.selectedObject.properties.name : null
    }
    this.SetCookie("StimulsoftMobileDesignerOptions", StiBase64.encode(JSON.stringify(designerOptions)));
    this.AddCommandToStack(params);
}

//Send Command Get Sql Parameter Types
StiMobileDesigner.prototype.SendCommandGetSqlParameterTypes = function (dataSource) {
    var params = {
        command: "GetSqlParameterTypes",
        dataSource: dataSource
    }
    this.AddCommandToStack(params);
}

//Send Command Align To Grid
StiMobileDesigner.prototype.SendCommandAlignToGridComponents = function () {
    var params = {};
    params.command = "AlignToGridComponents";
    params.components = [];
    params.pageName = this.options.currentPage.properties.name;

    var components = [];
    if (this.options.selectedObjects) components = this.options.selectedObjects;
    else if (this.options.selectedObject) components.push(this.options.selectedObject);

    for (var i = 0; i < components.length; i++) {
        params.components.push(components[i].properties.name);
    }
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Command Change Arrange Components
StiMobileDesigner.prototype.SendCommandChangeArrangeComponents = function (arrangeCommand) {
    var params = {};
    params.command = "ChangeArrangeComponents";
    params.arrangeCommand = arrangeCommand;
    params.components = [];
    params.pageName = this.options.currentPage.properties.name;

    var components = [];
    if (this.options.selectedObjects) components = this.options.selectedObjects;
    else if (this.options.selectedObject) components.push(this.options.selectedObject);

    for (var i = 0; i < components.length; i++) {
        params.components.push(components[i].properties.name);
    }
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Update Sample Text Format
StiMobileDesigner.prototype.SendCommandUpdateSampleTextFormat = function (textFormat) {
    var params = {};
    params.command = "UpdateSampleTextFormat";
    params.textFormat = textFormat;

    this.AddCommandToStack(params);
}

//Send Clone CrossTab Component
StiMobileDesigner.prototype.SendCommandStartEditCrossTabComponent = function (componentName) {
    var params = {};
    params.componentName = componentName;
    params.command = "StartEditCrossTabComponent";
    this.AddCommandToStack(params);
}

//Send Update CrossTab Component
StiMobileDesigner.prototype.SendCommandUpdateCrossTabComponent = function (componentName, updateParameters) {
    var params = {};
    params.componentName = componentName;
    params.updateParameters = updateParameters;
    params.command = "UpdateCrossTabComponent";
    this.AddCommandToStack(params);
}

//Send Get CrossTab Color Styles
StiMobileDesigner.prototype.SendCommandGetCrossTabColorStyles = function () {
    var params = {};
    params.command = "GetCrossTabColorStyles";
    this.AddCommandToStack(params);
}

//Send Duplicate Page
StiMobileDesigner.prototype.SendCommandDuplicatePage = function (pageIndex) {
    var params = {};
    params.command = "DuplicatePage";
    params.pageIndex = pageIndex;
    this.AddCommandToStack(params);
}

//Send Set Event Value
StiMobileDesigner.prototype.SendCommandSetEventValue = function (components, eventName, eventValue) {
    var params = {};
    params.command = "SetEventValue";
    params.components = components;
    params.eventValue = eventValue;
    params.eventName = eventName;
    this.AddCommandToStack(params);
}

//Get CrossTab Styles Content
StiMobileDesigner.prototype.SendCommandGetCrossTabStylesContent = function (callbackFunction) {
    var params = {};
    params.command = "GetCrossTabStylesContent";
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;

    this.AddCommandToStack(params);
}

//Get Gauge Styles Content
StiMobileDesigner.prototype.SendCommandGetGaugeStylesContent = function (callbackFunction) {
    var params = {};
    params.command = "GetGaugeStylesContent";
    params.componentName = this.options.selectedObject ? this.options.selectedObject.properties.name : this.options.selectedObjects[0].properties.name;
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;

    this.AddCommandToStack(params);
}

//Get Map Styles Content
StiMobileDesigner.prototype.SendCommandGetMapStylesContent = function (callbackFunction) {
    var params = {};
    params.command = "GetMapStylesContent";
    params.componentName = this.options.selectedObject ? this.options.selectedObject.properties.name : this.options.selectedObjects[0].properties.name;
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;

    this.AddCommandToStack(params);
}

//Get Table Styles Content
StiMobileDesigner.prototype.SendCommandGetTableStylesContent = function (callbackFunction) {
    var params = {};
    params.command = "GetTableStylesContent";
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;

    this.AddCommandToStack(params);
}

//Get Sparkline Styles Content
StiMobileDesigner.prototype.SendCommandGetSparklineStylesContent = function (callbackFunction) {
    var params = {};
    params.command = "GetSparklineStylesContent";
    params.componentName = this.options.selectedObject ? this.options.selectedObject.properties.name : this.options.selectedObjects[0].properties.name;
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;

    this.AddCommandToStack(params);
}

//Change Table
StiMobileDesigner.prototype.SendCommandChangeTableComponent = function (changeParameters) {
    var selectedObjects = this.options.selectedObject ? [this.options.selectedObject] : this.options.selectedObjects;
    if (!selectedObjects) return;

    changeParameters.cells = [];
    for (var i = 0; i < selectedObjects.length; i++) {
        changeParameters.cells.push(selectedObjects[i].properties.name);
    }

    var params = {
        command: "ChangeTableComponent",
        tableName: selectedObjects[0].typeComponent == "StiTable" ? selectedObjects[0].properties.name : selectedObjects[0].properties.parentName,
        zoom: this.options.report.zoom.toString(),
        changeParameters: changeParameters
    };

    this.AddCommandToStack(params);
}

//Send Open Style
StiMobileDesigner.prototype.SendCommandOpenStyle = function (fileContent, fileName) {
    var params = {
        command: "OpenStyle"
    };
    if (fileContent) params.base64Data = fileContent.substr(fileContent.indexOf("base64,") + 7);
    this.AddCommandToStack(params);
}

//Send Save Style
StiMobileDesigner.prototype.SendCommandSaveStyle = function (stylesCollection) {
    this.options.ignoreBeforeUnload = true;
    if (this.options.mvcMode) {
        var params = {
            command: "DownloadStyles",
            stylesCollection: JSON.stringify(stylesCollection)
        };
        this.AddMainParameters(params);
        this.PostForm(this.options.requestUrl.replace("{action}", this.options.actionDesignerEvent), params);
    }
    else {
        this.PostForm({ command: "DownloadStyles", reportGuid: this.options.reportGuid, stylesCollection: JSON.stringify(stylesCollection) });
    }
    var jsObject = this;
    setTimeout(function () { jsObject.options.ignoreBeforeUnload = false; }, 500);
}

//Send Create Style From Component
StiMobileDesigner.prototype.SendCommandCreateStylesFromComponents = function (componentsNames) {
    var params = {};
    params.command = "CreateStylesFromComponents";
    params.componentsNames = componentsNames;
    this.AddCommandToStack(params);
}

//Send Change Size Components
StiMobileDesigner.prototype.SendCommandChangeSizeComponents = function (actionName) {
    var params = {};
    params.command = "ChangeSizeComponents";
    params.actionName = actionName;
    params.pageName = this.options.currentPage.properties.name;
    params.zoom = this.options.report.zoom.toString();
    params.components = [];

    var components = [];
    if (this.options.selectedObjects) components = this.options.selectedObjects;
    else if (this.options.selectedObject) components.push(this.options.selectedObject);

    for (var i = 0; i < components.length; i++) {
        params.components.push(components[i].properties.name);
    }

    this.AddCommandToStack(params);
}

//Send Delete Column
StiMobileDesigner.prototype.SendCommandCreateFieldOnDblClick = function (selectedItem) {
    var params = {};
    params.command = "CreateFieldOnDblClick";
    params.pageName = this.options.currentPage.properties.name;
    params.fullName = selectedItem.getResultForEditForm();
    params.selectedComponents = [];
    params.zoom = this.options.report.zoom.toString();

    var selectedComponents = [];
    if (this.options.selectedObjects) selectedComponents = this.options.selectedObjects;
    else if (this.options.selectedObject) selectedComponents.push(this.options.selectedObject);

    for (var i = 0; i < selectedComponents.length; i++) {
        params.selectedComponents.push(selectedComponents[i].properties.name);
    }

    if (this.options.report.info.useLastFormat && this.options.lastStyleProperties) {
        params.lastStyleProperties = this.options.lastStyleProperties;
    }

    if (selectedItem.itemObject.typeItem == "Column") {
        params.columnName = selectedItem.itemObject.name;
        var columnParent = this.options.dictionaryTree.getCurrentColumnParent();
        params.currentParentType = columnParent.type;
        params.currentParentName = (columnParent.type == "BusinessObject") ? selectedItem.getBusinessObjectFullName() : columnParent.name;
    }
    else if (selectedItem.itemObject.typeItem == "Parameter") {
        params.parameterName = selectedItem.itemObject.name;
        var parameterParent = this.options.dictionaryTree.getCurrentColumnParent();
        params.currentParentName = parameterParent.name;
    }
    else if (selectedItem.itemObject.typeItem == "Resource") {
        params.resourceName = selectedItem.itemObject.name;
        params.resourceType = selectedItem.itemObject.type;
    }

    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Get Params From QueryString
StiMobileDesigner.prototype.SendCommandGetParamsFromQueryString = function (queryString, dataSourceName, callbackFunction) {
    var params = {};
    params.command = "GetParamsFromQueryString";
    params.queryString = queryString;
    params.dataSourceName = dataSourceName;
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;

    this.AddCommandToStack(params);
}

//Create Moving Copy Component
StiMobileDesigner.prototype.SendCommandCreateMovingCopyComponent = function (componentName, componentRect, isLastCommand, callbackFunction) {
    var params = {};
    params.command = "CreateMovingCopyComponent";
    params.components = [componentName];
    params.pageName = this.options.currentPage.properties.name;
    params.componentRect = componentRect;
    params.zoom = this.options.report.zoom.toString();
    params.isLastCommand = isLastCommand;
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;

    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Get Report Check Items
StiMobileDesigner.prototype.SendCommandGetReportCheckItems = function (callbackFunction) {
    var params = {};
    params.command = "GetReportCheckItems";
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;

    this.AddCommandToStack(params);
}

//Get Check Preview
StiMobileDesigner.prototype.SendCommandGetCheckPreview = function (checkIndex, callbackFunction) {
    var params = {};
    params.command = "GetCheckPreview";
    params.checkIndex = checkIndex;
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;

    this.AddCommandToStack(params);
}

//Action Check
StiMobileDesigner.prototype.SendCommandActionCheck = function (checkIndex, actionIndex, callbackFunction) {
    var params = {};
    params.command = "ActionCheck";
    params.checkIndex = checkIndex;
    params.actionIndex = actionIndex;
    params.zoom = this.options.report.zoom.toString();
    params.selectedObjectName = this.options.selectedObject.properties.name;
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;

    this.AddCommandToStack(params);
}

//Check Expression
StiMobileDesigner.prototype.SendCommandCheckExpression = function (expressionText, callbackFunction) {
    var params = {};
    params.command = "CheckExpression";
    params.expressionText = expressionText;
    params.componentName = this.options.selectedObject ? this.options.selectedObject.properties.name : null;
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;
    this.AddCommandToStack(params);
}

//Get WizardData
StiMobileDesigner.prototype.SendCommandGetWizardData = function (reportType, callbackFunction) {
    var params = {};
    params.command = "GetWizardData";
    params.reportType = reportType;
    params.scalingFactor = this.options.imagesScalingFactor;
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;
    this.AddCommandToStack(params);
}

//Get Globalization Strings
StiMobileDesigner.prototype.SendCommandGetGlobalizationStrings = function (callbackFunction) {
    var params = {};
    params.command = "GetGlobalizationStrings";
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;
    this.AddCommandToStack(params);
}

//Add Globalization Strings
StiMobileDesigner.prototype.SendCommandAddGlobalizationStrings = function (cultureName, callbackFunction) {
    var params = {};
    params.command = "AddGlobalizationStrings";
    params.callbackFunctionId = this.generateKey();
    params.cultureName = cultureName;
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;
    this.AddCommandToStack(params);
}

//Remove Globalization Strings
StiMobileDesigner.prototype.SendCommandRemoveGlobalizationStrings = function (index, callbackFunction) {
    var params = {};
    params.command = "RemoveGlobalizationStrings";
    params.callbackFunctionId = this.generateKey();
    params.index = index;
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;
    this.AddCommandToStack(params);
}

//Get Culture Settings From Report
StiMobileDesigner.prototype.GetCultureSettingsFromReport = function (index, callbackFunction) {
    var params = {};
    params.command = "GetCultureSettingsFromReport";
    params.callbackFunctionId = this.generateKey();
    params.index = index;
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;
    this.AddCommandToStack(params);
}

//Set Culture Settings To Report
StiMobileDesigner.prototype.SetCultureSettingsToReport = function (cultureName, callbackFunction) {
    var params = {};
    params.command = "SetCultureSettingsToReport";
    params.callbackFunctionId = this.generateKey();
    params.cultureName = cultureName;
    params.zoom = this.options.report.zoom.toString();
    params.selectedObjectName = this.options.selectedObject.properties.name;
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;
    this.AddCommandToStack(params);
}

//Apply Globalization Strings
StiMobileDesigner.prototype.SendCommandApplyGlobalizationStrings = function (index, propertyName, propertyValue) {
    var params = {};
    params.command = "ApplyGlobalizationStrings";
    params.index = index;
    params.propertyName = propertyName;
    params.propertyValue = propertyValue;
    this.AddCommandToStack(params);
}

//Remove Unlocalized Globalization Strings
StiMobileDesigner.prototype.SendCommandRemoveUnlocalizedGlobalizationStrings = function () {
    var params = {};
    params.command = "RemoveUnlocalizedGlobalizationStrings";
    this.AddCommandToStack(params);
}

//Send Clone Gauge Component
StiMobileDesigner.prototype.SendCommandStartEditGaugeComponent = function (componentName) {
    var params = {
        command: "StartEditGaugeComponent",
        componentName: componentName,
        zoom: this.options.report.zoom.toString()
    }
    this.AddCommandToStack(params);
}

//Send Clone Sparkline Component
StiMobileDesigner.prototype.SendCommandStartEditSparklineComponent = function (componentName) {
    var params = {
        command: "StartEditSparklineComponent",
        componentName: componentName,
        zoom: this.options.report.zoom.toString()
    }
    this.AddCommandToStack(params);
}

//Get Resource Content
StiMobileDesigner.prototype.SendCommandGetResourceContent = function (resourceName, callbackFunction) {
    var params = {};
    params.command = "GetResourceContent";
    params.resourceName = resourceName;
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;

    this.AddCommandToStack(params);
}

//Convert Resource Content
StiMobileDesigner.prototype.SendCommandConvertResourceContent = function (content, type, callbackFunction) {
    var params = {
        command: "ConvertResourceContent",
        type: type
    };
    params.callbackFunctionId = this.generateKey();
    if (content) params.base64Data = content.substr(content.indexOf("base64,") + 7);
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;
    this.AddCommandToStack(params);
}

//Get Resource Text
StiMobileDesigner.prototype.SendCommandGetResourceText = function (resourceName, callbackFunction) {
    var params = {};
    params.command = "GetResourceText";
    params.resourceName = resourceName;
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;

    this.AddCommandToStack(params);
}

//Set Resource Text
StiMobileDesigner.prototype.SendCommandSetResourceText = function (resourceName, resourceText, callbackFunction) {
    var params = {};
    params.command = "SetResourceText";
    params.resourceName = resourceName;
    params.resourceText = resourceText;
    if (callbackFunction) {
        params.callbackFunctionId = this.generateKey();
        this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;
    }

    this.AddCommandToStack(params);
}

//Get Resource View Data
StiMobileDesigner.prototype.SendCommandGetResourceViewData = function (resourceName, resourceType, resourceContent, callbackFunction) {
    var params = {};
    params.command = "GetResourceViewData";
    params.resourceName = resourceName;
    params.resourceType = resourceType;
    if (resourceContent) params.resourceContent = resourceContent;
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;
    this.AddCommandToStack(params);
}

//Create Component From Resource
StiMobileDesigner.prototype.SendCommandCreateComponentFromResource = function (itemObject, point, pageName) {
    this.options.reportIsModified = true;
    var params = {
        command: "CreateComponentFromResource",
        itemObject: itemObject,
        pageName: pageName,
        zoom: this.options.report.zoom.toString(),
        point: point
    }

    if (this.options.report.info.useLastFormat && this.options.lastStyleProperties) {
        params.lastStyleProperties = this.options.lastStyleProperties;
    }

    this.AddCommandToStack(params);
}

//Create Element From Resource
StiMobileDesigner.prototype.SendCommandCreateElementFromResource = function (itemObject, point, pageName) {
    this.options.reportIsModified = true;
    var params = {
        command: "CreateElementFromResource",
        itemObject: itemObject,
        pageName: pageName,
        point: point
    }

    this.AddCommandToStack(params);
}

//Get Sample Connection String
StiMobileDesigner.prototype.SendCommandGetSampleConnectionString = function (typeConnection, serviceName, callbackFunction) {
    var params = {};
    params.command = "GetSampleConnectionString";
    params.typeConnection = typeConnection;
    params.serviceName = serviceName;
    params.callbackFunctionId = this.generateKey();
    this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;

    this.AddCommandToStack(params);
}

//Create Data Item From Resource
StiMobileDesigner.prototype.SendCommandCreateDatabaseFromResource = function (resourceObject, existingResource) {
    var commandGuid = this.generateKey();
    var params = {
        command: "CreateDatabaseFromResource",
        resourceObject: resourceObject,
        existingResource: existingResource,
        isAsyncCommand: true
    }

    if (!this.options.jsMode && resourceObject.loadedContent && resourceObject.loadedContent.length > this.options.uploadBlockSize) {
        var contentStr = resourceObject.loadedContent;
        delete params.resourceObject.loadedContent;

        var countBlocks = parseInt(contentStr.length / this.options.uploadBlockSize) + 1;
        var cacheGuid = this.generateKey();

        for (var i = 0; i < countBlocks; i++) {
            var params_ = this.CopyObject(params);
            params_.cacheGuid = cacheGuid;
            params_.countBlocks = countBlocks;
            params_.blockContent = i + contentStr.substr(i * this.options.uploadBlockSize, this.options.uploadBlockSize);
            this.AddCommandToStack(params_, commandGuid);
        }
    }
    else {
        this.AddCommandToStack(params, commandGuid);
    }

    this.options.reportIsModified = true;
}

//Send Clone BarCode Component
StiMobileDesigner.prototype.SendCommandStartEditBarCodeComponent = function (componentName) {
    var params = {};
    params.componentName = componentName;
    params.command = "StartEditBarCodeComponent";
    this.AddCommandToStack(params);
}

//Send Clone Shape Component
StiMobileDesigner.prototype.SendCommandStartEditShapeComponent = function (componentName) {
    var params = {};
    params.componentName = componentName;
    params.command = "StartEditShapeComponent";
    this.AddCommandToStack(params);
}

//Move Dictionary Item
StiMobileDesigner.prototype.SendCommandMoveDictionaryItem = function (fromObject, toObject, direction) {
    var params = {};
    params.command = "MoveDictionaryItem";
    params.direction = direction;
    params.fromObject = fromObject;
    params.toObject = toObject;

    if (fromObject.typeItem == "Column") {
        var columnParent = this.options.dictionaryTree.getCurrentColumnParent();
        params.currentParentType = columnParent.type;
        params.currentParentName = (columnParent.type == "BusinessObject") ? this.options.dictionaryTree.selectedItem.getBusinessObjectFullName() : columnParent.name;
    }

    this.AddCommandToStack(params);
}


StiMobileDesigner.prototype.SendCommandUpdateReportAliases = function () {
    var params = {};
    params.command = "UpdateReportAliases";
    params.zoom = this.options.report.zoom.toString();
    params.selectedObjectName = this.options.selectedObject ? this.options.selectedObject.properties.name : null;

    this.AddCommandToStack(params);
}

//Send Any Command
StiMobileDesigner.prototype.SendCommandToDesignerServer = function (commandName, parameters, callbackFunction) {
    var params = {};
    params.command = commandName;

    if (callbackFunction) {
        params.callbackFunctionId = this.generateKey();
        this.options.callbackFunctions[params.callbackFunctionId] = callbackFunction;
    }

    if (parameters) {
        for (var key in parameters) {
            params[key] = parameters[key];
        }
    }

    this.AddCommandToStack(params);
}

//Move Connection Data To Resource
StiMobileDesigner.prototype.SendCommandMoveConnectionDataToResource = function (editConnectionForm) {
    var params = {};
    params.command = "MoveConnectionDataToResource";
    params.pathSchema = StiBase64.encode(editConnectionForm.pathSchemaControl.textBox.value);
    params.pathData = StiBase64.encode(editConnectionForm.pathDataControl.textBox.value);
    params.typeConnection = editConnectionForm.connection.typeConnection;
    params.name = editConnectionForm.nameControl.value;
    params.alias = editConnectionForm.aliasControl.value;
    params.codePageCsv = editConnectionForm.codePageCsvControl.key;
    params.codePageDbase = editConnectionForm.codePageDbaseControl.key;
    params.separator = editConnectionForm.getCsvSeparatorText(editConnectionForm.csvSeparatorControl.key);
    if (editConnectionForm.mode == "Edit") params.oldName = editConnectionForm.connection.name;

    this.AddCommandToStack(params);
}

//Move Connection Data To Resource
StiMobileDesigner.prototype.SendCommandMoveConnectionDataToResource = function (editConnectionForm) {
    var params = {};
    params.command = "MoveConnectionDataToResource";
    params.pathSchema = StiBase64.encode(editConnectionForm.pathSchemaControl.textBox.value);
    params.pathData = StiBase64.encode(editConnectionForm.pathDataControl.textBox.value);
    params.typeConnection = editConnectionForm.connection.typeConnection;
    params.name = editConnectionForm.nameControl.value;
    params.alias = editConnectionForm.aliasControl.value;
    params.codePageCsv = editConnectionForm.codePageCsvControl.key;
    params.codePageDbase = editConnectionForm.codePageDbaseControl.key;
    params.separator = editConnectionForm.getCsvSeparatorText(editConnectionForm.csvSeparatorControl.key);
    if (editConnectionForm.mode == "Edit") params.oldName = editConnectionForm.connection.name;

    this.AddCommandToStack(params);
}

//Set Map Properties
StiMobileDesigner.prototype.SendCommandSetMapProperties = function (componentName, properties, updateMapData) {
    var params = {
        command: "SetMapProperties",
        componentName: componentName,
        properties: properties,
        zoom: this.options.report.zoom.toString()
    }

    if (updateMapData) params.updateMapData = true;

    this.AddCommandToStack(params);
}

//Update Map Data
StiMobileDesigner.prototype.SendCommandUpdateMapData = function (componentName, rowIndex, columnName, textValue) {
    var params = {
        command: "UpdateMapData",
        componentName: componentName,
        rowIndex: rowIndex,
        columnName: columnName,
        textValue: textValue,
        zoom: this.options.report.zoom.toString()
    }

    this.AddCommandToStack(params);
}

//Send Open Page
StiMobileDesigner.prototype.SendCommandOpenPage = function (fileContent, fileName) {
    var params = {
        command: "OpenPage"
    };
    if (fileContent) params.base64Data = fileContent.substr(fileContent.indexOf("base64,") + 7);
    this.AddCommandToStack(params);
}

//Send Save Page
StiMobileDesigner.prototype.SendCommandSavePage = function (pageIndex) {
    this.options.ignoreBeforeUnload = true;
    if (this.options.mvcMode) {
        var params = {
            command: "DownloadPage",
            pageIndex: pageIndex
        };
        this.AddMainParameters(params);
        this.PostForm(this.options.requestUrl.replace("{action}", this.options.actionDesignerEvent), params);
    }
    else {
        this.PostForm({ command: "DownloadPage", reportGuid: this.options.reportGuid, pageIndex: pageIndex });
    }
    var jsObject = this;
    setTimeout(function () { jsObject.options.ignoreBeforeUnload = false; }, 500);
}

// Create Text Element
StiMobileDesigner.prototype.SendCommandCreateTextElement = function (itemObject, point, pageName) {
    this.options.reportIsModified = true;
    var params = {
        command: "CreateTextElement",
        itemObject: itemObject,
        pageName: pageName,
        zoom: this.options.report.zoom.toString(),
        point: point
    }

    this.AddCommandToStack(params);
}

// Create DatePicker Element
StiMobileDesigner.prototype.SendCommandCreateDatePickerElement = function (itemObject, point, pageName) {
    this.options.reportIsModified = true;
    var params = {
        command: "CreateDatePickerElement",
        itemObject: itemObject,
        pageName: pageName,
        zoom: this.options.report.zoom.toString(),
        point: point
    }

    this.AddCommandToStack(params);
}

// Create ComboBox Element
StiMobileDesigner.prototype.SendCommandCreateComboBoxElement = function (itemObject, point, pageName) {
    this.options.reportIsModified = true;
    var params = {
        command: "CreateComboBoxElement",
        itemObject: itemObject,
        pageName: pageName,
        zoom: this.options.report.zoom.toString(),
        point: point
    }

    this.AddCommandToStack(params);
}

// Create Table Element
StiMobileDesigner.prototype.SendCommandCreateTableElement = function (draggedItem, point, pageName) {
    this.options.reportIsModified = true;
    var params = {
        command: "CreateTableElement",
        draggedItem: draggedItem,
        pageName: pageName,
        zoom: this.options.report.zoom.toString(),
        point: point
    }

    this.AddCommandToStack(params);
}

//Change Dashboard Style
StiMobileDesigner.prototype.SendCommandChangeDashboardStyle = function (dashboardName, styleIdent) {
    this.options.reportIsModified = true;
    var params = {
        command: "ChangeDashboardStyle",
        dashboardName: dashboardName,
        styleIdent: styleIdent
    }
    this.AddCommandToStack(params);
}

//Set Gauge Properties
StiMobileDesigner.prototype.SendCommandSetGaugeProperties = function (componentName, properties) {
    var params = {
        command: "SetGaugeProperties",
        componentName: componentName,
        properties: properties,
        zoom: this.options.report.zoom.toString()
    }

    this.AddCommandToStack(params);
}

//Send Save Dictionary
StiMobileDesigner.prototype.SendCommandSaveDictionary = function () {
    this.options.ignoreBeforeUnload = true;
    if (this.options.mvcMode) {
        var params = {
            command: "DownloadDictionary"
        };
        this.AddMainParameters(params);
        this.PostForm(this.options.requestUrl.replace("{action}", this.options.actionDesignerEvent), params);
    }
    else {
        this.PostForm({ command: "DownloadDictionary", reportGuid: this.options.reportGuid });
    }
    var jsObject = this;
    setTimeout(function () { jsObject.options.ignoreBeforeUnload = false; }, 500);
}

//Open Wizard Dashboard
StiMobileDesigner.prototype.SendCommandOpenWizardDashboard = function (dashboardName) {
    var params = {
        command: "OpenWizardDashboard",
        dashboardName: dashboardName,
        designerOptions: this.GetCookie("StimulsoftMobileDesignerOptions")
    };
    if (this.options.controls.zoomScale.value) params.zoom = this.options.controls.zoomScale.value;

    this.AddCommandToStack(params);
}

//Open Wizard Report
StiMobileDesigner.prototype.SendCommandOpenWizardReport = function (reportName) {
    var params = {
        command: "OpenWizardReport",
        reportName: reportName,
        designerOptions: this.GetCookie("StimulsoftMobileDesignerOptions")
    };
    if (this.options.controls.zoomScale.value) params.zoom = this.options.controls.zoomScale.value;

    this.AddCommandToStack(params);
}

//Save Preview Settings
StiMobileDesigner.prototype.SendCommandSetPreviewSettings = function (previewSettings) {
    var params = {
        command: "SetPreviewSettings",
        previewSettings: previewSettings
    };
    this.AddCommandToStack(params);
}

//RepaintAllDbsElements
StiMobileDesigner.prototype.SendCommandRepaintAllDbsElements = function () {
    var params = {
        command: "RepaintAllDbsElements"
    };
    this.AddCommandToStack(params);
}

//ChangeTypeElement
StiMobileDesigner.prototype.SendCommandChangeTypeElement = function (component, newType) {
    var params = {
        command: "ChangeTypeElement",
        elementName: component.properties.name,
        newType: newType,
        zoom: this.options.report.zoom.toString()
    };
    if (component.originalElementContent) {
        params.originalElementContent = component.originalElementContent;
        params.originalElementType = component.originalElementType;
    }
    this.RemoveStylesFromCache(component.properties.name, component.originalElementType);
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//ChangeDashboardViewMode
StiMobileDesigner.prototype.SendCommandChangeDashboardViewMode = function (dashboardName, dashboardViewMode, removeMobileSurface) {
    var params = {
        command: "ChangeDashboardViewMode",
        dashboardName: dashboardName,
        dashboardViewMode: dashboardViewMode,
        removeMobileSurface: removeMobileSurface
    };
    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Embed All Data To Resources
StiMobileDesigner.prototype.SendCommandEmbedAllDataToResources = function () {
    var params = {};
    params.command = "EmbedAllDataToResources";
    this.AddCommandToStack(params);
}

//Send Duplicate Dictionary Element
StiMobileDesigner.prototype.SendCommandDuplicateDictionaryElement = function (selectedItem) {
    var params = {};
    params.command = "DuplicateDictionaryElement";
    params.itemObject = selectedItem.itemObject;
    params.locCopyOf = this.loc.Report.CopyOf;

    this.options.reportIsModified = true;
    this.AddCommandToStack(params);
}

//Send Save Blockly
StiMobileDesigner.prototype.SendCommandSaveBlockly = function (blocklyContent, eventName) {
    var jsObject = this;
    this.options.ignoreBeforeUnload = true;
    if (this.options.mvcMode) {
        var params = {
            command: "DownloadBlockly",
            blocklyContent: StiBase64.encode(blocklyContent),
            eventName: eventName
        };
        this.AddMainParameters(params);
        this.PostForm(this.options.requestUrl.replace("{action}", this.options.actionDesignerEvent), params);
    }
    else {
        this.PostForm({ command: "DownloadBlockly", reportGuid: this.options.reportGuid, blocklyContent: StiBase64.encode(blocklyContent), eventName: eventName });
    }
    setTimeout(function () { jsObject.options.ignoreBeforeUnload = false; }, 500);
}

//Send Download Resource
StiMobileDesigner.prototype.SendCommandDownloadResource = function (resourceName) {
    var jsObject = this;
    this.options.ignoreBeforeUnload = true;

    if (this.options.mvcMode) {
        var params = {
            command: "DownloadResource",
            resourceName: resourceName
        };
        this.AddMainParameters(params);
        this.PostForm(this.options.requestUrl.replace("{action}", this.options.actionDesignerEvent), params);
    }
    else {
        this.PostForm({ command: "DownloadResource", resourceName: resourceName });
    }

    setTimeout(function () { jsObject.options.ignoreBeforeUnload = false; }, 500);
}

//Send Download Image Content
StiMobileDesigner.prototype.SendCommandDownloadImageContent = function (imageData) {
    var jsObject = this;
    this.options.ignoreBeforeUnload = true;

    if (this.options.mvcMode) {
        var params = {
            command: "DownloadImageContent",
            imageData: imageData
        };
        this.AddMainParameters(params);
        this.PostForm(this.options.requestUrl.replace("{action}", this.options.actionDesignerEvent), params);
    }
    else {
        this.PostForm({ command: "DownloadImageContent", imageData: imageData });
    }

    setTimeout(function () { jsObject.options.ignoreBeforeUnload = false; }, 500);
}