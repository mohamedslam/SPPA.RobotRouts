
StiMobileDesigner.prototype.LoginWithFacebook = function (ownerButton, actionName) {
    var jsObject = this;
    var authForm = this.options.forms.authForm;
    var profilePanel = this.options.profilePanel;
    var errorForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();

    var clearAllCookies = function () {
        jsObject.RemoveCookie("sti_FacebookAuthCode");
        jsObject.RemoveCookie("sti_FacebookAuthError");
    };

    var progress = function (visibleState) {
        if (ownerButton.progressMarker) {
            ownerButton.progressMarker.changeVisibleState(visibleState);
        }
    }

    clearAllCookies();

    jsObject.SendCommandToDesignerServer("GetFacebookAuthorizationUrl", {}, function (answer) {
        jsObject.openNewWindow(StiBase64.decode(answer.url), null, "width=600,height=700");

        //waiting facebook authorize result
        if (authForm || profilePanel) {
            var closeFormsTimer = 0;
            var waitResultTimer = setInterval(function () {
                if (!((authForm && authForm.visible) || (profilePanel && profilePanel.visible))) closeFormsTimer++;
                if (closeFormsTimer > 10) clearInterval(waitResultTimer);
                
                var authCode = jsObject.GetCookie("sti_FacebookAuthCode");
                var error = jsObject.GetCookie("sti_FacebookAuthError");

                if (authCode || error) {
                    clearAllCookies();
                    clearInterval(waitResultTimer);

                    if (authCode) {
                        if (actionName == "AddLoginWithFacebook") {
                            jsObject.SendCloudCommand("UserAddLoginWithFacebook", { FacebookAuthCode: authCode, Source: "CloudDesigner" },
                                function (data) {
                                    if (profilePanel && profilePanel.visible) {
                                        profilePanel.updateFacebookEmail(data.ResultFacebookId);
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
                                FacebookAuthCode: authCode,
                                Source: "CloudDesigner"
                            }

                            jsObject.SendCloudCommand("UserFacebookLogin", params,
                                function (data) {
                                    progress(false);

                                    var processFacebookAuthEmailIsUsed = function (data) {
                                        authForm.facebookId = data.ResultFacebookId;
                                        authForm.facebookEncryptingToken = data.ResultFacebookToken;
                                        authForm.facebookEmail = data.ResultFacebookEmail;
                                        authForm.changeMode("enterPassword");
                                    }

                                    var processFacebookAuthResultSuccess = function (data) {
                                        authForm.loginComplete(data.ResultSessionKey, data.ResultUserKey);
                                    }

                                    if (data.ResultFacebookEmailIsEmpty) {
                                        if (data.ResultFacebookId && data.ResultFacebookToken) {
                                            var emailForm = jsObject.options.forms.enterEmailForm || jsObject.InitializeEnterEmailForm();

                                            emailForm.show(function (email) {
                                                var params = {
                                                    UserName: email,
                                                    FacebookId: data.ResultFacebookId,
                                                    FacebookToken: data.ResultFacebookToken,
                                                    Source: "CloudDesigner"
                                                }
                                                progress(true);

                                                jsObject.SendCloudCommand("UserFacebookLogin", params,
                                                    function (data) {
                                                        progress(false);

                                                        if (data.ResultFacebookEmailIsUsed) {
                                                            processFacebookAuthEmailIsUsed(data);
                                                        }
                                                        else if (data.ResultSuccess && data.ResultSessionKey && data.ResultUserKey) {
                                                            processFacebookAuthResultSuccess(data);
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
                                    else if (data.ResultFacebookEmailIsUsed) {
                                        processFacebookAuthEmailIsUsed(data);
                                    }
                                    else if (data.ResultSuccess && data.ResultSessionKey && data.ResultUserKey) {
                                        processFacebookAuthResultSuccess(data);
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