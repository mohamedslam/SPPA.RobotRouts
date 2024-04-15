
StiJsViewer.prototype.PostAjaxToCloudServer = function (url, data, callback) {
    var jsObject = this;
    var xmlHttp = this.createConnection();
    this.openConnection(xmlHttp, url, data ? data.responseType : "text");

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
            catch (e) { }

            if (status == 0) {
                callback("ServerError:Timeout response from the server.", jsObject);
            } else if (status == 200) {
                callback(xmlHttp.response ? xmlHttp.response : xmlHttp.responseText, jsObject);
            } else {
                if (jsObject.options.server.showServerErrorPage && xmlHttp.response) jsObject.controls.reportPanel.innerHTML = xmlHttp.response;
                callback("ServerError:" + status + " - " + xmlHttp.statusText, jsObject);
            }
        }
    };

    xmlHttp.send(JSON.stringify(data, null, 2));
}

StiJsViewer.prototype.formatString = function (format, args) {
    if (format) {
        var result = format;
        for (var i = 1; i < arguments.length; i++) {
            result = result.replace('{' + (i - 1) + '}', arguments[i]);
        }
        return arguments.length > 1 ? result : format.replace('{0}', "");
    } else {
        return "";
    }
}

StiJsViewer.prototype.formatResultMsg = function (data) {
    var msg = "";
    if (data && data.ResultNotice) {
        var arg = data.ResultNotice.Argument ? data.ResultNotice.Argument : "";
        if (data.ResultNotice.Arguments) {
            for (var i = 0; i < data.ResultNotice.Arguments.length; i++) {
                arg += (arg != "" ? ", " : "") + data.ResultNotice.Arguments[i];
            }
        }
        if (this.collections.loc.Notices && this.collections.loc.Notices[data.ResultNotice.Ident]) {
            return this.formatString(this.collections.loc.Notices[data.ResultNotice.Ident], data.ResultNotice.Arguments ? data.ResultNotice.Arguments : arg);
        }
        if (!data.ResultNotice.Arguments && arg == "") {
            msg = data.ResultNotice[data.ResultNotice.Ident];
            if (!msg) {
                msg = data.ResultNotice.Ident;
                if (data.ResultNotice.CustomMessage) msg += ". " + data.ResultNotice.CustomMessage;
            }
        } else {
            msg = this.formatString(data.ResultNotice.Ident, data.ResultNotice.Arguments ? data.ResultNotice.Arguments : arg);
        }
    }
    return msg;
}

StiJsViewer.prototype.SendCloudCommand = function (commandName, parameters, completeFunc, errorFunc) {
    var cloudUrl = this.options.cloudParameters ? this.options.cloudParameters.restUrl : null;

    if (!cloudUrl) {
        cloudUrl = this.options.requestAbsoluteUrl;
        if (cloudUrl.indexOf("?") >= 0) cloudUrl = cloudUrl.substring(0, cloudUrl.indexOf("?"));
        if (cloudUrl.indexOf("/s/") >= 0) cloudUrl = cloudUrl.substring(0, cloudUrl.indexOf(/s/));
    }

    var url = cloudUrl + "/1/runcommand/" + this.generateKey();
    var jsObject = this;

    parameters.Ident = commandName;

    if (!parameters.SessionKey && this.options.cloudParameters && this.options.cloudParameters.sessionKey) {
        parameters.SessionKey = this.options.cloudParameters.sessionKey;
    }

    this.PostAjaxToCloudServer(url, parameters, function (responseText, jsObject) {
        if (typeof (responseText) == "string" && responseText.indexOf("Error") == 0) {
            if (errorFunc) errorFunc();
            var errorForm = jsObject.controls.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
            errorForm.show(responseText);
        }
        else if (typeof (responseText) == "string" && responseText.substr(0, 1) == "{") {
            var data = JSON.parse(responseText);

            try {
                if (data.ResultSuccess) {
                    if (completeFunc)
                        completeFunc(data);
                } else {
                    if (errorFunc)
                        errorFunc(data);
                    else {
                        var errorForm = jsObject.controls.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                        errorForm.show(jsObject.formatResultMsg(data));
                    }
                }
            }
            catch (e) {
                var errorForm = jsObject.controls.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                errorForm.show(e.message);
            }
        }
        else {
            if (errorFunc) errorFunc(responseText);
        }
    });
}