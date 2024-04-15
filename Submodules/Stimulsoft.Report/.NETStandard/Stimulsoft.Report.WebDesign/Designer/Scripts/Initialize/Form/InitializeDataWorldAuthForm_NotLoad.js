
StiMobileDesigner.prototype.InitializeDataWorldAuthForm_ = function () {
    var form = this.BaseForm("dataWorldAuthForm", "Data.World", 3);
    var jsObject = this;
    var redirectUri = "https://reports.stimulsoft.com";
    var authorizationUrl = "https://data.world/oauth/authorize";
    var tokenUrl = "https://data.world/oauth/access_token";
    var clientId = "7011d98eeb82b9c1";
    var clientSecret = "secret-n0kCrYEfS8IVL5SJjBMjJHG6";
    var responseType = "code";

    form.showError = function (messageText) {
        var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
        errorMessageForm.show(messageText);
    }

    form.show = function (connection, owner, database) {
        this.connection = connection;
        this.owner = owner;
        this.database = database;

        var authRequestUrl = authorizationUrl + "?client_id=" + clientId + "&redirect_uri=" + encodeURIComponent(redirectUri) + "&response_type=" + responseType;
        var authWindow = jsObject.openNewWindow(authRequestUrl, null, "width=600,height=700");

        var waitResultTimer = setInterval(function () {
            try {
                if (authWindow && authWindow.location && authWindow.location.href) {
                    var currentUrl = authWindow.location.href;
                    if (currentUrl.indexOf(redirectUri) >= 0) {
                        var code = jsObject.GetParameterFromUrl("code", currentUrl);
                        if (code) {
                            clearInterval(waitResultTimer);
                            authWindow.close();
                            var url = tokenUrl + "?code=" + code + "&client_id=" + clientId + "&client_secret=" + clientSecret + "&grant_type=authorization_code";
                            jsObject.PostAjax(url, {}, function (data) {
                                if (typeof data == "string") {
                                    if (data.indexOf("ServerError:") == 0) {
                                        form.showError(data.substr(12));
                                    }
                                    else {
                                        var token = JSON.parse(data).access_token;
                                        if (token) {
                                            form.connection.connectionString = StiBase64.encode("Owner=" + form.owner + ";Database=" + form.database + ";Token=" + token);
                                            jsObject.SendCommandCreateOrEditConnection(form.connection);
                                        }
                                    }
                                }
                            });
                        }
                        else {
                            var error = jsObject.GetParameterFromUrl("error", currentUrl);
                            if (error) form.showError(error);
                        }
                    }
                }
            }
            catch (e) { }
        }, 500);
    }

    return form;
}