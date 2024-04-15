
StiMobileDesigner.prototype.LoginWithGitHub = function (ownerButton, actionName) {
    var jsObject = this;
    var authForm = this.options.forms.authForm;
    var profilePanel = this.options.profilePanel;
    var errorForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();

    var clearAllCookies = function () {
        jsObject.RemoveCookie("sti_GitHubAuthCode");
        jsObject.RemoveCookie("sti_GitHubAuthError");
    };

    var progress = function (visibleState) {
        if (ownerButton.progressMarker) {
            ownerButton.progressMarker.changeVisibleState(visibleState);
        }
    }

    clearAllCookies();

    jsObject.SendCommandToDesignerServer("GetGitHubAuthorizationUrl", {}, function (answer) {
        jsObject.openNewWindow(StiBase64.decode(answer.url), null, "width=600,height=700");

        //waiting google authorize result
        if (authForm || profilePanel) {
            var closeFormsTimer = 0;
            var waitResultTimer = setInterval(function () {
                if (!((authForm && authForm.visible) || (profilePanel && profilePanel.visible))) closeFormsTimer++;
                if (closeFormsTimer > 10) clearInterval(waitResultTimer);

                var authCode = jsObject.GetCookie("sti_GitHubAuthCode");
                var error = jsObject.GetCookie("sti_GitHubAuthError");

                if (authCode || error) {
                    clearAllCookies();
                    clearInterval(waitResultTimer);

                    if (authCode) {
                        if (actionName == "AddLoginWithGitHub") {
                            jsObject.SendCloudCommand("UserAddLoginWithGitHub", { GitHubAuthCode: authCode, Source: "CloudDesigner" },
                                function (data) {
                                    if (profilePanel && profilePanel.visible) {
                                        profilePanel.updateGitHubEmail(data.ResultGitHubId);
                                    }
                                },
                                function (data) {
                                    errorForm.show(jsObject.formatResultMsg(data));
                                }
                            );
                        }
                        else {
                            progress(true);

                            var params = {
                                GitHubAuthCode: authCode,
                                Source: "CloudDesigner"
                            }

                            jsObject.SendCloudCommand("UserGitHubLogin", params,
                                function (data) {
                                    progress(false);

                                    var processGitHubAuthEmailIsUsed = function (data) {
                                        authForm.gitHubId = data.ResultGitHubId;
                                        authForm.gitHubEncryptingToken = data.ResultGitHubToken;
                                        authForm.gitHubEmail = data.ResultGitHubEmail;
                                        authForm.changeMode("enterPassword");
                                    }

                                    var processGitHubAuthResultSuccess = function (data) {
                                        authForm.loginComplete(data.ResultSessionKey, data.ResultUserKey);
                                    }

                                    if (data.ResultGitHubEmailIsEmpty) {
                                        if (data.ResultGitHubId && data.ResultGitHubToken) {
                                            var emailForm = jsObject.options.forms.enterEmailForm || jsObject.InitializeEnterEmailForm();

                                            emailForm.show(function (email) {
                                                var params = {
                                                    UserName: email,
                                                    GitHubId: data.ResultGitHubId,
                                                    GitHubToken: data.ResultGitHubToken,
                                                    Source: "CloudDesigner"
                                                }
                                                progress(true);

                                                jsObject.SendCloudCommand("UserGitHubLogin", params,
                                                    function (data) {
                                                        progress(false);

                                                        if (data.ResultGitHubEmailIsUsed) {
                                                            processGitHubAuthEmailIsUsed(data);
                                                        }
                                                        else if (data.ResultSuccess && data.ResultSessionKey && data.ResultUserKey) {
                                                            processGitHubAuthResultSuccess(data);
                                                        }
                                                        else {
                                                            errorForm.show(jsObject.formatResultMsg(data));
                                                        }
                                                    },
                                                    function (data) {
                                                        progress(false);
                                                        errorForm.show(jsObject.formatResultMsg(data));
                                                    });
                                            });
                                        }
                                    }
                                    else if (data.ResultGitHubEmailIsUsed) {
                                        processGitHubAuthEmailIsUsed(data);
                                    }
                                    else if (data.ResultSuccess && data.ResultSessionKey && data.ResultUserKey) {
                                        processGitHubAuthResultSuccess(data);
                                    }
                                },
                                function (data) {
                                    progress(false);
                                    errorForm.show(jsObject.formatResultMsg(data));
                                });
                        }
                    }
                    else {
                        errorForm.show(error);
                    }
                }
            }, 500);
        }
    });
}