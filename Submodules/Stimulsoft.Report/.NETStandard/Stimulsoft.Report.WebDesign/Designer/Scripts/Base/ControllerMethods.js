
StiMobileDesigner.prototype.PostAjaxToCloudServer = function (url, data, callback) {
    var jsObject = this;
    var xmlHttp = this.CreateXMLHttp();

    if (jsObject.options.requestTimeout != 0) {
        setTimeout(function () {
            if (xmlHttp.readyState < 4) xmlHttp.abort();
        }, jsObject.options.requestTimeout * 1000);
    }

    xmlHttp.open("POST", url, true);
    xmlHttp.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    xmlHttp.responseType = "text";
    xmlHttp.onreadystatechange = function () {
        if (xmlHttp.readyState == 4) {
            var status = 0;
            try {
                status = xmlHttp.status;
            }
            catch (e) {
            }

            if (status == 0) {
                callback("Error: Timeout response from the server", jsObject);
            } else if (status == 200) {
                callback(xmlHttp.response ? xmlHttp.response : xmlHttp.responseText, jsObject);
            } else {
                callback("Error: " + status + " - " + xmlHttp.statusText, jsObject);
            }
        }
    };
    xmlHttp.send(JSON.stringify(data, null, 2));
}

StiMobileDesigner.prototype.formatString = function (format, args) {
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

StiMobileDesigner.prototype.formatResultMsg = function (data) {
    var msg = "";
    if (data && data.ResultNotice) {
        var arg = data.ResultNotice.Argument ? data.ResultNotice.Argument : "";
        if (data.ResultNotice.Arguments) {
            for (var i = 0; i < data.ResultNotice.Arguments.length; i++) {
                arg += (arg != "" ? ", " : "") + data.ResultNotice.Arguments[i];
            }
        }
        if (this.loc.Notices && this.loc.Notices[data.ResultNotice.Ident]) {
            return this.formatString(this.loc.Notices[data.ResultNotice.Ident], data.ResultNotice.Arguments ? data.ResultNotice.Arguments : arg);
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

StiMobileDesigner.prototype.SendCloudCommand = function (commandName, parameters, completeFunc, errorFunc, ignoreSessionDate, url) {
    if (!this.options.cloudParameters) return;
    var url = url || (this.options.cloudParameters.restUrl + "1/runcommand/" + this.generateKey());
    var jsObject = this;

    parameters.Ident = commandName;
    if (this.options.cloudParameters.sessionKey) parameters.SessionKey = this.options.cloudParameters.sessionKey;
    if (this.options.cloudParameters.userKey && !parameters.UserKey) parameters.UserKey = this.options.cloudParameters.userKey;

    this.PostAjaxToCloudServer(url, parameters, function (responseText, jsObject) {
        if (typeof (responseText) == "string" && responseText.indexOf("Error") == 0) {
            if (errorFunc) errorFunc({ Ident: commandName }, responseText);
            var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
            errorMessageForm.show(responseText);
        }
        else if (typeof (responseText) == "string" && responseText.substr(0, 1) == "{") {
            var data = JSON.parse(responseText);
            if (data) {
                try {
                    if (data.ResultSuccess) {
                        if (completeFunc) completeFunc(data);
                    }
                    else {
                        var msg = jsObject.formatResultMsg(data);

                        //User LogOut
                        if (data.ResultNotice && ((data.ResultNotice.Arguments && jsObject.IsContains(data.ResultNotice.Arguments, "SessionKey")) ||
                            (data.ResultNotice.Ident == "AuthUserHasLoggedOut" && data.Ident != "UserLogout"))) {
                            jsObject.options.cloudParameters.sessionKey = null;
                            jsObject.options.forms.authForm.show();
                            if (errorFunc) errorFunc();
                            return;
                        }

                        //Quota Maximum Items
                        if (data && data.ResultNotice && data.ResultNotice.Ident == "QuotaMaximumItemsCountExceeded") {
                            msg = jsObject.loc.Notices.QuotaMaximumItemsCountExceeded;
                            var maxItems = jsObject.GetCurrentPlanLimitValue("MaxItems");
                            if (maxItems) msg += "<br/>" + jsObject.loc.PropertyMain.Maximum + ": " + maxItems;

                            jsObject.InitializeNotificationForm(function (form) {
                                form.show(msg, jsObject.NotificationMessages("upgradeYourPlan"), "Notifications.Elements.png");
                            });
                            if (errorFunc) errorFunc();
                            return;
                        }

                        if (errorFunc) {
                            errorFunc(data, msg);
                        }
                        else {
                            var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                            errorMessageForm.show(msg);
                        }
                    }
                }
                catch (e) {
                    var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                    errorMessageForm.show(e.message);
                }
            }
        }
        else {
            if (errorFunc) errorFunc({ Ident: commandName }, responseText);
        }
    })
}