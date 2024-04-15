

StiJsViewer.prototype.createConnection = function () {
    if (typeof XMLHttpRequest != "undefined") return new XMLHttpRequest();

    // old IE
    if (window.ActiveXObject) {
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
                return new ActiveXObject(allVersions[i]);
            }
            catch (error) {
            }
        }
    }

    throw new Error("Unable to create XMLHttp object.");
}

StiJsViewer.prototype.openConnection = function (http, url, responseType, method) {
    method = method || "POST";
    http.open(method, url);

    http.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
    if (this.options.requestToken)
        http.setRequestHeader("RequestVerificationToken", this.options.requestToken);

    http.responseType = responseType ? responseType : "text";
}

StiJsViewer.prototype.createPostParameters = function (data, asObject) {
    var params = {
        "viewerId": this.options.viewerId,
        "routes": this.options.routes,
        "formValues": this.options.formValues,
        "clientGuid": this.options.clientGuid,
        "drillDownGuid": this.reportParams.drillDownGuid,
        "dashboardDrillDownGuid": this.reportParams.dashboardDrillDownGuid,
        "cacheMode": this.options.server.cacheMode,
        "cacheTimeout": this.options.server.cacheTimeout,
        "cacheItemPriority": this.options.server.cacheItemPriority,
        "pageNumber": this.reportParams.pageNumber,
        "originalPageNumber": this.reportParams.originalPageNumber,
        "reportType": this.reportParams.type,
        "zoom": (this.reportParams.zoom && this.reportParams.zoom > 0) ? this.reportParams.zoom : 100,
        "viewMode": this.reportParams.viewMode,
        "multiPageWidthCount": this.reportParams.multiPageWidthCount,
        "multiPageHeightCount": this.reportParams.multiPageHeightCount,
        "multiPageContainerWidth": this.reportParams.multiPageContainerWidth,
        "multiPageContainerHeight": this.reportParams.multiPageContainerHeight,
        "multiPageMargins": this.reportParams.multiPageMargins,
        "showBookmarks": this.options.toolbar.showBookmarksButton,
        "openLinksWindow": this.options.appearance.openLinksWindow,
        "chartRenderType": this.options.appearance.chartRenderType,
        "reportDisplayMode": (this.options.displayModeFromReport || this.options.appearance.reportDisplayMode),
        "drillDownParameters": this.reportParams.drillDownParameters,
        "editableParameters": this.reportParams.editableParameters,
        "useRelativeUrls": this.options.server.useRelativeUrls,
        "passQueryParametersForResources": this.options.server.passQueryParametersForResources,
        "passQueryParametersToReport": this.options.server.passQueryParametersToReport,
        "version": this.options.shortProductVersion,
        "reportDesignerMode": this.options.reportDesignerMode,
        "imagesQuality": this.options.appearance.imagesQuality,
        "parametersPanelSortDataItems": this.options.appearance.parametersPanelSortDataItems,
        "combineReportPages": this.options.appearance.combineReportPages,
        "isAngular": this.options.isAngular,
        "allowAutoUpdateCookies": this.options.server.allowAutoUpdateCookies
    };

    if (this.options.server.useLocalizedCache && this.options.localization) {
        params.useLocalizedCache = true;
        params.localization = this.options.localization;
    }

    if (this.options.userValues)
        params.userValues = this.options.userValues;

    if (this.reportParams.type == "Dashboard") {
        this.calculateLayout();
        params.dashboardWidth = this.controls.reportPanel.layout.width;
        params.dashboardHeight = this.controls.reportPanel.layout.height;
        params.elementName = this.reportParams.elementName;
    }

    if (data) {
        for (var key in data) {
            params[key] = data[key];
        }
    }

    // Object params
    var postParams = {
        stiweb_component: "Viewer"
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
    if (this.options.server.useCompression) postParams["stiweb_packed_parameters"] = StiGZipHelper.pack(jsonParams);
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

StiJsViewer.prototype.postAjax = function (url, data, callback) {
    if (data && data.action == "GetReport") {
        this.options.paramsVariablesStartValues = null;

        if (this.controls.toolbar) {
            this.controls.toolbar.setEnabled(false);
            if (this.controls.navigatePanel) this.controls.navigatePanel.setEnabled(false);
        }
    }

    var jsObject = this;
    var xmlHttp = this.createConnection();
    this.openConnection(xmlHttp, url, data ? data.responseType : "text", data ? data.method : "POST");

    if (jsObject.options.server.requestTimeout != 0) {
        setTimeout(function () {
            if (xmlHttp.readyState < 4) xmlHttp.abort();
        }, jsObject.options.server.requestTimeout * 1000);
    }

    xmlHttp.onreadystatechange = function () {
        if (xmlHttp.readyState == 4) {
            jsObject.service.isRequestInProcess = false;
            clearTimeout(jsObject.dashboardProcessTimeout);

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
                // Try to display error from content
                else if (jsObject.options.server.showServerErrorPage && xmlHttp.responseText) jsObject.controls.reportPanel.innerHTML = xmlHttp.responseText;
                // Display HTTP request error code and status
                else callback("ServerError:" + status + " - " + xmlHttp.statusText, jsObject);
            }
        }
    };

    this.service.isRequestInProcess = true;
    var params = this.createPostParameters(data, false);
    xmlHttp.id = this.options.viewerId;
    xmlHttp.send(params);
}

StiJsViewer.prototype.postForm = function (url, data, doc, postOnlyData) {
    var jsObject = this;
    if (!doc) doc = document;
    var form = doc.createElement("FORM");
    form.setAttribute("method", "POST");
    form.setAttribute("action", url);

    var params = postOnlyData ? data : jsObject.createPostParameters(data, true);
    if (this.options.requestToken)
        params["__RequestVerificationToken"] = this.options.requestToken;

    for (var key in params) {
        var paramsField = doc.createElement("INPUT");
        paramsField.setAttribute("type", "hidden");
        paramsField.setAttribute("name", key);
        paramsField.setAttribute("value", params[key]);
        form.appendChild(paramsField);
    }

    if (jsObject.options.jsDesigner) {
        jsObject.options.jsDesigner.options.ignoreBeforeUnload = true;
    }

    doc.body.appendChild(form);
    form.submit();
    doc.body.removeChild(form);

    setTimeout(function () {
        if (jsObject.options.jsDesigner) {
            jsObject.options.jsDesigner.options.ignoreBeforeUnload = false;
        }
    }, 500);
}

StiJsViewer.prototype.showError = function (message) {
    var messageType = "Error";
    var messageText = null;

    // Check for error in "ServerError:" string format
    if (message != null && typeof (message) == "string" && message.substr(0, 17) == "CloudServerError:") {
        if (message.length <= 18) messageText = "An unknown error occurred (the server returned an empty value).";
        else messageText = message.substr(17);
        var messageBlocks = messageText.split(";");

        var notificationForm = this.controls.forms.notificationForm || this.InitializeNotificationForm();
        notificationForm.show(messageBlocks[0], messageBlocks.length > 0 ? messageBlocks[1] : null, "Notifications.Warning.png");

        notificationForm.upgradeButton.caption.innerText = this.collections.loc["ButtonOk"];

        notificationForm.upgradeButton.action = function () {
            notificationForm.changeVisibleState(false);
        }
        return true;
    }

    // Check for error in "ServerError:" string format
    if (message != null && typeof (message) == "string" && message.substr(0, 12) == "ServerError:") {
        if (message.length <= 13) messageText = "An unknown error occurred (the server returned an empty value).";
        else messageText = message.substr(12);
    }

    // Check for error in JSON format
    if (message != null && message.success === false && message.type && message.text) {
        messageType = message.type;
        messageText = message.text;
    }

    if (messageText != null) {
        if (messageText == "The report is not specified." && !this.options.appearance.showReportIsNotSpecifiedMessage)
            return true;

        if (this.collections.images) {
            var errorForm = this.controls.forms.errorMessageForm || this.InitializeErrorMessageForm();
            errorForm.show(messageText.replace("\n", "<br>"), messageType);
        }
        else {
            alert(messageText);
        }
        return true;
    }

    return false;
}

StiJsViewer.prototype.getActionRequestUrl = function (requestUrl, action) {
    if (!action)
        return requestUrl.replace("{action}", "");

    if (action.indexOf('?') < 0)
        return requestUrl.replace("{action}", action);

    var query = action.substring(action.indexOf('?') + 1);
    action = action.substring(0, action.indexOf('?'));

    return requestUrl.replace("{action}", action) + (requestUrl.indexOf('?') > 0 ? '&' : '?') + query;
}