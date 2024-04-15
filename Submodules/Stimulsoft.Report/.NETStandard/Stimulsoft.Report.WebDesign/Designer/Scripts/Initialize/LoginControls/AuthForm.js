
StiMobileDesigner.prototype.InitializeAuthForm = function () {
    var jsObject = this;
    var authForm = this.BaseForm("authForm", this.loc.Authorization.WindowTitleLogin, 2, null, true);
    authForm.container.style.overflowX = "hidden";
    authForm.container.style.overflowY = "hidden";
    authForm.onkeyup = null;
    authForm.controls = {};
    authForm.formWidth = 380;
    authForm.style.width = authForm.formWidth + "px";
    authForm.style.backgroundColor = "white";
    authForm.hideButtonsPanel();
    authForm.mode = "login";
    authForm.result = {};
    authForm.className = "stiDesignerForm";
    authForm.header.className = "stiDesignerFormContainer stiDesignerFormHeader";
    authForm.caption.className = "stiDesignerLoginFormCaption";
    authForm.caption.style.padding = "45px 133px 45px 33px";
    authForm.container.className = "stiDesignerFormContainer";
    authForm.container.style.padding = "0px";
    authForm.container.style.overflowY = "auto";
    authForm.container.style.overflowX = "hidden";

    //Logo
    var imgLogo = document.createElement("img");
    imgLogo.setAttribute("style", "position: absolute; top: 50px; right: 42px; width: 87px; height: 21px");
    StiMobileDesigner.setImageSource(imgLogo, this.options, "LoginControls.LogoStimulsoftCloud.png");
    authForm.header.style.position = "relative";
    authForm.header.appendChild(imgLogo);

    //Add Panels
    authForm.this_ = this;
    authForm.panels = {};
    authForm.panels.login = this.AuthFormLoginPanel(authForm);
    authForm.panels.signUp = this.AuthFormSignUpPanel(authForm);
    authForm.panels.forgotPassword = this.AuthFormForgotPasswordPanel(authForm);
    authForm.panels.registerSuccessfully = this.AuthFormRegisterSuccessfullyPanel(authForm);
    authForm.panels.resetPassword = this.AuthFormResetPasswordPanel(authForm);
    authForm.panels.enterPassword = this.AuthFormEnterPasswordPanel(authForm);
    authForm.panels.wheelPanel = this.AddWheelPanel();

    for (var pName in authForm.panels) {
        authForm.container.appendChild(authForm.panels[pName]);
    }

    authForm.onhide = function () {
        this.googleAuthParams = null;
    }

    authForm.cancelAction = function () {
        if (jsObject.options.forms.shareForm && jsObject.options.forms.shareForm.visible) {
            jsObject.options.forms.shareForm.changeVisibleState(false);
        }
    }

    authForm.onshow = function () {
        authForm.controls.buttonSignUp.setEnabled(false);
        authForm.controls.buttonSignUpWithGoogle.setEnabled(false);
        authForm.controls.buttonSignUpWithGitHub.setEnabled(false);
        authForm.controls.buttonSignUpWithFacebook.setEnabled(false);
        authForm.controls.agreeCheckbox.setChecked(false);
        authForm.controls.buttonSignUp.agreeTermsAndPolicy = false;
        authForm.controls.buttonSignUp.completeCaptcha = false;

        if (window["captchaId"] != null && window["grecaptcha"] != null) {
            // eslint-disable-next-line no-undef
            grecaptcha.reset(window["captchaId"]);
        }

        if (jsObject.options.previewMode) {
            jsObject.options.workPanel.showPanel(jsObject.options.homePanel);
            jsObject.options.buttons.homeToolButton.setSelected(true);
        }
    }

    authForm.changeMode = function (mode) {
        var changedFormSizes = this.mode != mode;

        this.mode = mode;
        switch (this.mode) {
            case "login":
                this.caption.innerHTML = jsObject.loc.Authorization.WindowTitleLogin;
                break;

            case "signUp":
            case "registerSuccessfully":
                this.caption.innerHTML = jsObject.loc.Authorization.WindowTitleSignUp;
                break;

            case "forgotPassword":
                if (authForm.controls.resetInfoText) {
                    authForm.controls.resetInfoText.parentNode.removeChild(authForm.controls.resetInfoText);
                }
                this.caption.innerHTML = jsObject.loc.Authorization.WindowTitleForgotPassword;
                break;

            case "resetPassword":
                this.caption.innerHTML = jsObject.loc.Authorization.WindowTitleForgotPassword;
                break;

            case "enterPassword":
                this.caption.innerHTML = jsObject.loc.Authorization.WindowTitleLogin;
                break;
        }

        if (this.caption.innerHTML) this.caption.innerHTML = this.caption.innerHTML.toUpperCase();

        for (var name in this.panels) {
            this.panels[name].style.display = name == mode ? "" : "none";
        }

        this.focusFirstControl();

        if (changedFormSizes && !this.movedByUser) jsObject.SetObjectToCenter(this);
    }

    authForm.focusFirstControl = function () {
        if (!jsObject.isTouchDevice) {
            if (this.mode == "login") { this.controls.loginUserName.focus(); return; }
            if (this.mode == "signUp") { this.controls.signUpFirstName.focus(); return; }
            if (this.mode == "forgotPassword") { this.controls.forgotPasswordUserName.focus(); return; }
            if (this.mode == "resetPassword") { this.controls.buttonSetPassword.focus(); return; }
            if (this.mode == "enterPassword") { this.controls.enterPassword.focus(); return; }
        }
    }

    authForm.resetControls = function () {
        for (var name in this.controls) {
            if ("value" in this.controls[name] && this.controls[name] != this.controls.loginUserName) this.controls[name].value = "";
        }
    }

    authForm.hideAllProgressMarkers = function () {
        var buttons = ["buttonLogin", "buttonResetPassword", "buttonSignUp", "buttonLoginWithGoogle", "buttonSignUpWithGoogle", "buttonLoginWithGitHub", "buttonSignUpWithGitHub",
            "buttonLoginWithFacebook", "buttonSignUpWithFacebook", "buttonSendPassword"];
        for (var i = 0; i < buttons.length; i++) {
            this.controls[buttons[i]].progressMarker.changeVisibleState(false);
        }
    }

    authForm.show = function (chh) {
        jsObject.options.logins = [];
        try {
            jsObject.options.logins = eval(jsObject.GetCookie("logins")) || [];
        }
        catch (e) { };

        this.resetControls();
        this.hideAllProgressMarkers();

        if (!chh) {
            this.changeMode("login");
        }

        this.changeVisibleState(true);
        this.focusFirstControl();
    }

    authForm.start = function () {
        authForm.show(true);
    }

    authForm.loginComplete = function (sessionKey, userKey, userName) {
        authForm.hideAllProgressMarkers();
        jsObject.options.cloudParameters.sessionKey = sessionKey;
        jsObject.options.cloudParameters.userKey = userKey;

        var saveCookies = true;

        if (userName) {
            if (saveCookies && jsObject.options.logins.indexOf(userName) < 0) {
                jsObject.options.logins.push(userName);
            } else if (!saveCookies && jsObject.options.logins.indexOf(userName) >= 0) {
                jsObject.options.logins.splice(jsObject.options.logins.indexOf(userName), 1);
            }
        }
        if (saveCookies) {
            var expDate = new Date(new Date().getTime() + 2592000000);
            jsObject.SetCookie("sti_SessionKey", jsObject.options.cloudParameters.sessionKey, jsObject.options.cookiesPath, jsObject.options.cookiesDomain, null, expDate.toUTCString());

            if (jsObject.options.cloudParameters.userKey) {
                jsObject.SetCookie("sti_UserKey", jsObject.options.cloudParameters.userKey, jsObject.options.cookiesPath, jsObject.options.cookiesDomain, null, expDate.toUTCString());
            }
        }

        jsObject.setUserInfo(function () {
            //Build tree after login
            var forms = jsObject.options.forms;
            if (forms.onlineOpenReport && forms.onlineOpenReport.visible) {
                forms.onlineOpenReport.setToTreeMode();
            }
            if (forms.onlineSaveAsReport && forms.onlineSaveAsReport.visible) {
                forms.onlineSaveAsReport.setToTreeMode();
            }
            if (forms.browseSaveAsReport && forms.browseSaveAsReport.visible) {

                forms.browseSaveAsReport.show(jsObject.options.saveAsPanel.getFileName(true), jsObject.options.saveAsPanel.nextFunc);
            }
            if (forms.browseOpenReport && forms.browseOpenReport.visible) {
                forms.browseOpenReport.nextFunc();
            }
            if (forms.shareForm && forms.shareForm.visible) {
                forms.shareForm.checkReportSavingToCloud();
            }
            if (jsObject.options.newReportPanel && jsObject.options.newReportPanel.style.display == "" && jsObject.options.newReportPanel.resultsPanel.style.display == "") {
                jsObject.options.newReportPanel.resultsPanel.show();
            }
            if (jsObject.options.profilePanel && jsObject.options.profilePanel.visible) {
                jsObject.options.profilePanel.show();
            }
            if (jsObject.options.fileMenuTeamPanel && jsObject.options.fileMenuTeamPanel.visible) {
                jsObject.options.fileMenuTeamPanel.show();
            }
            if (jsObject.options.subscriptionsPanel && jsObject.options.subscriptionsPanel.visible) {
                jsObject.options.subscriptionsPanel.show();
            }
            if (jsObject.options.checkForUpdatePanel && jsObject.options.checkForUpdatePanel.visible) {
                jsObject.options.checkForUpdatePanel.show();
            }
        });

        authForm.changeVisibleState(false);
        authForm.ready = true;
    }

    authForm.startLogin = function (userName, password, md5) {
        var this_ = this;
        var params = {};
        params.Ident = "UserLogin";
        params.UserName = userName;
        params.Password = password;
        params.Device = { Ident: "WebDevice" };
        params.Version = jsObject.options.shortProductVersion;
        params.Localization = jsObject.options.defaultLocalization;
        params.LoginAndActivateLicense = true;
        if (md5) params.md5 = md5;

        this.controls.buttonLogin.progressMarker.changeVisibleState(true);

        jsObject.SetCookie("loginRememberMe", true);

        jsObject.SendCloudCommand("UserLogin", params,
            function (data) {
                authForm.loginComplete(data.ResultSessionKey, data.ResultUserKey, userName);
            },
            function (data, msg) {
                this_.ready = true;
                this_.hideAllProgressMarkers();
                if (data && data.ResultNotice && data.ResultNotice.Ident == "AuthAccountIsNotActivated") {
                    var ttx = jsObject.loc.Authorization.TextRegistrationSuccessfully.replace("{0}", this_.controls.loginUserName.value);
                    ttx = ttx.substring(ttx.indexOf("\n") + 1);
                    this_.controls.registerSuccessfullyText.innerHTML = ttx;
                    this_.changeMode("registerSuccessfully");
                }
                else {
                    if (this_.visible) jsObject.ShowMessagesTooltip(jsObject.formatResultMsg(data), this_.controls.loginUserName);
                }
            });
    }

    authForm.apply = function () {
        var result = {};
        result.typeResult = this.mode;
        var this_ = this;
        switch (this.mode) {
            case "login": {
                if (this.controls.loginUserName.checkEmpty(jsObject.loc.Messages.ThisFieldIsNotSpecified) &&
                    this.controls.loginPassword.checkEmpty(jsObject.loc.Messages.ThisFieldIsNotSpecified)) {
                    this.startLogin(this.controls.loginUserName.value, this.controls.loginPassword.value);
                }
                break;
            }
            case "signUp": {
                if (this.controls.agreeCheckbox.isChecked &&
                    this.controls.signUpFirstName.checkEmpty(jsObject.loc.Notices.AuthFirstNameIsNotSpecified, null, null, true) &&
                    this.controls.signUpLastName.checkEmpty(jsObject.loc.Notices.AuthLastNameIsNotSpecified, null, null, true) &&
                    this.controls.signUpUserName.checkEmpty(jsObject.loc.Notices.AuthUserNameIsNotSpecified, null, null, true) &&
                    this.controls.signUpUserName.checkEmail(jsObject.loc.Notices.AuthUserNameShouldLookLikeAnEmailAddress) &&
                    this.controls.signUpPassword.checkEmpty(jsObject.loc.Notices.AuthPasswordIsNotSpecified, null, null, true) &&
                    this.controls.signUpPassword.checkLength(6, jsObject.loc.Notices.AuthPasswordIsTooShort)) {
                    this.controls.buttonSignUp.progressMarker.changeVisibleState(true);
                    result.FirstName = this.controls.signUpFirstName.value;
                    result.LastName = this.controls.signUpLastName.value;
                    result.UserName = this.controls.signUpUserName.value;
                    result.Password = this.controls.signUpPassword.value;
                    result.Localization = jsObject.GetOnlyBaseLocalization(jsObject.options.defaultLocalization);
                    result.Module = "Reports";
                    result.Version = jsObject.options.shortProductVersion;
                    result.Source = "DesignerCloud";

                    jsObject.SendCloudCommand("UserSignUp", result,
                        function (data) {
                            if (data.ResultActivationByEmail) {
                                this_.controls.registerSuccessfullyText.innerHTML = jsObject.loc.Authorization.TextRegistrationSuccessfully.replace("{0}", this_.controls.signUpUserName.value);
                                this_.controls.loginUserName.value = result.UserName;
                                this_.controls.loginPassword.value = result.Password;
                                this_.changeMode("registerSuccessfully");
                            }
                            else {
                                this_.changeMode("login");
                                this_.startLogin(result.UserName, result.Password);
                            }
                        },
                        function (data, msg) {
                            jsObject.ShowMessagesTooltip(msg, this_.controls.signUpUserName);
                            this_.hideAllProgressMarkers();
                        });

                }
                break;
            }
            case "forgotPassword": {
                if (this.controls.forgotPasswordUserName.checkEmpty(jsObject.loc.Messages.ThisFieldIsNotSpecified) &&
                    this.controls.forgotPasswordUserName.checkEmail(jsObject.loc.Notices.AuthUserNameShouldLookLikeAnEmailAddress)) {
                    result.UserName = this.controls.forgotPasswordUserName.value;
                    this.controls.buttonResetPassword.progressMarker.changeVisibleState(true);
                    jsObject.SendCloudCommand("UserResetPassword", result, function (data) {
                        this_.hideAllProgressMarkers();
                        if (this_.controls.resetInfoText) {
                            this_.controls.resetInfoText.remove();
                        }

                        var info = $("<div style='text-align:left; white-space:normal; width:230px; overflow:hidden; padding: 3px 4px 3px 0;' class='stiDesignerCaptionControls'>" + jsObject.formatString(jsObject.loc.Notices.AuthSendMessageWithInstructions, result.UserName) + "</div>");
                        $(this_.controls.buttonResetPassword).after(info);

                        var hh = info.height();
                        info.height(0);
                        info.animate({ height: hh });
                        this_.controls.resetInfoText = info;
                    }, function (data, msg) {
                        jsObject.ShowMessagesTooltip(msg, this_.controls.forgotPasswordUserName);
                        this_.hideAllProgressMarkers();
                    });
                }
                break;
            }
            case "enterPassword": {
                if (this.controls.enterPassword.checkEmpty(jsObject.loc.Messages.ThisFieldIsNotSpecified)) {
                    var progress = function (visibleState) {
                        authForm.controls.buttonSendPassword.progressMarker.changeVisibleState(visibleState);
                    }
                    if (authForm.googleEncryptingToken) {
                        progress(true);

                        var params = {
                            password: this.controls.enterPassword.value,
                            encryptingToken: authForm.googleEncryptingToken,
                            email: authForm.googleEmail
                        }
                        jsObject.SendCommandToDesignerServer("AssociatedGoogleAuthWithYourAccount", params,
                            function (answer) {
                                progress(false);

                                if (answer.sessionKey && answer.userKey) {
                                    authForm.loginComplete(answer.sessionKey, answer.userKey);
                                }
                                else if (answer.enterPasswordError) {
                                    jsObject.ShowMessagesTooltip(answer.enterPasswordError, authForm.controls.enterPassword);
                                }
                            }
                        );
                        authForm.googleEncryptingToken = null;
                    }
                    else if (authForm.gitHubEncryptingToken) {
                        progress(true);

                        var params = {
                            Password: authForm.controls.enterPassword.value,
                            GitHubToken: authForm.gitHubEncryptingToken,
                            UserName: authForm.gitHubEmail,
                            Source: "CloudDesigner"
                        }
                        jsObject.SendCloudCommand("UserAddLoginWithGitHub", params,
                            function (data) {
                                progress(false);
                                authForm.startLogin(authForm.gitHubEmail, authForm.controls.enterPassword.value);
                            },
                            function (data, msg) {
                                progress(false);
                                jsObject.ShowMessagesTooltip(msg, authForm.controls.enterPassword);
                            }
                        );
                        authForm.gitHubEncryptingToken = null;
                    }
                    else if (authForm.facebookEncryptingToken) {
                        progress(true);

                        var params = {
                            Password: authForm.controls.enterPassword.value,
                            FacebookToken: authForm.facebookEncryptingToken,
                            UserName: authForm.facebookEmail,
                            Source: "CloudDesigner"
                        }
                        jsObject.SendCloudCommand("UserAddLoginWithFacebook", params,
                            function (data) {
                                progress(false);
                                authForm.startLogin(authForm.facebookEmail, authForm.controls.enterPassword.value);
                            },
                            function (data, msg) {
                                progress(false);
                                jsObject.ShowMessagesTooltip(msg, authForm.controls.enterPassword);
                            }
                        );
                        authForm.facebookEncryptingToken = null;
                    }
                }
                break;
            }
        }

        if (result["userName"] != null && result["userName"] == "")
            return false;
        else
            return result;
    }

    authForm.action = function () {
        this.apply();
    }

    var this_ = this;
    if (window.location.search.indexOf("reset=") >= 0) {
        authForm.resetId = window.location.search.substring(window.location.search.indexOf("reset=") + 6);
        authForm.changeMode("resetPassword");
        authForm.ready = true;
    }
    else {
        var currentSession = this_.GetCookie("sti_SessionKey");
        var userKey = this_.GetCookie("sti_UserKey");

        if (!this_.options.cloudParameters.sessionKey && currentSession) {
            this_.options.cloudParameters.sessionKey = currentSession;
        }

        if (!this_.options.cloudParameters.userKey && userKey) {
            this_.options.cloudParameters.userKey = userKey;
        }

        if (this_.options.cloudParameters.sessionKey && this_.options.cloudParameters.userKey) {
            authForm.changeVisibleState(false);
            authForm.ready = true;

            this_.setUserInfo();
        }
        else {
            authForm.changeMode("login");
            authForm.ready = true;
            if (this_.options.cloudMode) this_.FinishSession();
        }
    }

    //Initialize captcha
    var captchaScript = document.createElement("script");
    captchaScript.src = "https://www.google.com/recaptcha/api.js?onload=captchaLoadedComplete&render=explicit";
    this.options.mainPanel.appendChild(captchaScript);
}

StiMobileDesigner.prototype.AuthFormLoginPanel = function (authForm) {
    var loginPanel = document.createElement("div");
    var mTable = this.CreateHTMLTable();
    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.margin = "0 auto 0 auto";
    loginPanel.appendChild(mTable);
    mTable.style.width = "100%";
    var ct = mTable.addCell(controlsTable);
    ct.style.width = "50%";
    var rDiv = $("<div style='width: " + authForm.formWidth + "px;margin:0 auto;border-top:'></div>")[0];
    var ds = mTable.addCellInNextRow(rDiv);
    ds.style.textAlign = "center";
    ds.colSpan = 2;

    //UserName
    var loginControl = this.TextBoxWithHintText(null, 280, this.loc.Authorization.TextUserName, this.loc.Authorization.TextUserName, "login.png");
    authForm.controls.loginUserName = loginControl;
    controlsTable.addCellInNextRow(loginControl.table);
    loginControl.value = this.GetCookie("loginLogin") || "";

    //Password
    var passwordControl = this.TextBoxWithHintText(null, 280, this.loc.Authorization.TextPassword, this.loc.Authorization.TextPassword, "password.png");
    passwordControl.setAttribute("type", "password");
    authForm.controls.loginPassword = passwordControl;
    controlsTable.addCellInNextRow(passwordControl.table).style.paddingTop = "17px";
    passwordControl.value = this.GetCookie("loginPassword") || "";

    //Button Login
    var buttonLogin = this.LoginButton(null, this.loc.Authorization.ButtonLogin, null);
    authForm.controls.buttonLogin = buttonLogin;
    buttonLogin.style.margin = "8px 0 8px 0";
    controlsTable.addCellInNextRow(buttonLogin).style.paddingTop = "10px";
    buttonLogin.action = function () { authForm.action(); }
    this.AddSmallProgressMarkerToControl(buttonLogin, true);

    //Button Forgot Password
    var buttonForgotPassword = this.HiperLinkButtonForAuthForm(null, this.loc.Authorization.HyperlinkForgotPassword, true);
    authForm.controls.buttonForgotPassword = buttonForgotPassword;
    buttonForgotPassword.style.marginTop = "5px";
    buttonForgotPassword.style.display = "inline-block";
    controlsTable.addCellInNextRow(buttonForgotPassword).setAttribute("style", "text-align: left;");
    buttonForgotPassword.action = function () { authForm.changeMode("forgotPassword"); }

    var sep = this.SeparatorOr(315, "#c6c6c6");
    controlsTable.addCellInNextRow(sep).style.padding = "10px 0 6px 0";

    var buttonsTable = this.CreateHTMLTable();
    buttonsTable.style.width = "100%";
    controlsTable.addCellInNextRow(buttonsTable).style.padding = "10px 0 20px 0";

    //Google Login
    var butGoogleLog = this.ButtonLoginWith("Account.Google.png", "#dc4e41");
    butGoogleLog.style.margin = "0 6px 0 0";
    authForm.controls.buttonLoginWithGoogle = butGoogleLog;
    buttonsTable.addCell(butGoogleLog);
    this.AddSmallProgressMarkerToControl(butGoogleLog, true);

    butGoogleLog.action = function () {
        this.jsObject.LoginWithGoogle(this);
    }

    //Facebook Login
    var butFacebookLog = this.ButtonLoginWith("Account.Facebook.png", "#1976d2");
    butFacebookLog.style.margin = "0 6px 0 6px";
    authForm.controls.buttonLoginWithFacebook = butFacebookLog;
    buttonsTable.addCell(butFacebookLog);
    this.AddSmallProgressMarkerToControl(butFacebookLog, true);

    butFacebookLog.action = function () {
        this.jsObject.LoginWithFacebook(this);
    }

    //GitHub Login
    var butGitHubLog = this.ButtonLoginWith("Account.GitHub.png", "#2d333b");
    butGitHubLog.style.margin = "0 0 0 6px";
    authForm.controls.buttonLoginWithGitHub = butGitHubLog;
    buttonsTable.addCell(butGitHubLog);
    this.AddSmallProgressMarkerToControl(butGitHubLog, true);

    butGitHubLog.action = function () {
        this.jsObject.LoginWithGitHub(this);
    }

    //Button Register Account
    var buttonRegisterAccount = this.HiperLinkButtonForAuthForm(null, this.loc.Cloud.ButtonSignUp, true);
    authForm.controls.buttonRegisterAccount = buttonRegisterAccount;
    buttonRegisterAccount.style.display = "inline-block";
    controlsTable.addCellInNextRow(buttonRegisterAccount).setAttribute("style", "text-align: center; border-top: 1px solid #f2f1f1; padding: 15px 0 15px 0;");
    buttonRegisterAccount.action = function () { authForm.changeMode("signUp"); }

    return loginPanel;
}

StiMobileDesigner.prototype.AuthFormSignUpPanel = function (authForm) {
    var signUpPanel = document.createElement("div");
    var mTable = this.CreateHTMLTable();
    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.margin = "0 auto 0 auto";
    signUpPanel.appendChild(mTable);
    mTable.style.width = "100%";
    var ct = mTable.addCell(controlsTable);
    ct.style.width = "50%";
    var rDiv = $("<div style='width: " + authForm.formWidth + "px;margin:0 auto;'></div>")[0];
    var ds = mTable.addCellInNextRow(rDiv);
    ds.style.textAlign = "center";
    ds.colSpan = 2;

    //First Name
    var firstNameControl = this.TextBoxWithHintText(null, 280, this.loc.Authorization.TextFirstName, this.loc.Authorization.TextFirstName, "login.png");
    authForm.controls.signUpFirstName = firstNameControl;
    controlsTable.addCellInNextRow(firstNameControl.table);

    //Last Name
    var lastNameControl = this.TextBoxWithHintText(null, 280, this.loc.Authorization.TextLastName, this.loc.Authorization.TextLastName, "login.png");
    authForm.controls.signUpLastName = lastNameControl;
    var lastNameControlCell = controlsTable.addCellInNextRow(lastNameControl.table);
    lastNameControlCell.style.paddingTop = "17px";

    //UserName
    var loginControl = this.TextBoxWithHintText(null, 280, this.loc.Authorization.TextUserName, this.loc.Authorization.TextUserName, "email.png");
    authForm.controls.signUpUserName = loginControl;
    var loginControlCell = controlsTable.addCellInNextRow(loginControl.table);
    loginControlCell.style.paddingTop = "17px";

    //Password
    var passwordControl = this.TextBoxWithHintText(null, 280, this.loc.Authorization.TextPassword, this.loc.Authorization.TextPassword, "password.png");
    passwordControl.setAttribute("type", "password");
    authForm.controls.signUpPassword = passwordControl;
    var passwordControlCell = controlsTable.addCellInNextRow(passwordControl.table);
    passwordControlCell.style.paddingTop = "17px";

    //Captcha
    var captchaContainer = $("<div id='sti_authform_captcha_container'></div>")[0];
    authForm.controls.captchaCell = controlsTable.addCellInNextRow(captchaContainer);

    //Agree To Terms And Policy
    var agreeTextContainer = document.createElement("div");
    agreeTextContainer.style.width = "300px";
    controlsTable.addCellInNextRow(agreeTextContainer).style.padding = "20px 0 7px 0";

    var agreeCheckbox = this.CheckBox();
    agreeCheckbox.id = "sti_authform_agreecheckbox";
    authForm.controls.agreeCheckbox = agreeCheckbox;
    agreeCheckbox.style.marginRight = "6px";
    agreeCheckbox.style.display = "inline-block";
    agreeTextContainer.appendChild(agreeCheckbox);

    agreeCheckbox.action = function () {
        authForm.controls.buttonSignUp.agreeTermsAndPolicy = this.isChecked;
        if (authForm.controls.buttonSignUp.completeCaptcha) {
            authForm.controls.buttonSignUp.setEnabled(this.isChecked);
            authForm.controls.buttonSignUpWithGoogle.setEnabled(this.isChecked);
            authForm.controls.buttonSignUpWithGitHub.setEnabled(this.isChecked);
            authForm.controls.buttonSignUpWithFacebook.setEnabled(this.isChecked);
        }
    }

    var textTermsAndPolicy = this.loc.Cloud.AcceptTermsAndPrivacyPolicy;

    var agreeText = this.CreateHTMLTable();
    agreeText.className = "stiDesignerTextContainer";
    agreeText.style.fontSize = "14px";
    agreeText.style.display = "inline-block";
    agreeText.addTextCell(textTermsAndPolicy ? textTermsAndPolicy.substring(0, textTermsAndPolicy.indexOf("{0}")) : "I accept the");
    agreeTextContainer.appendChild(agreeText);

    var buttonPrivacy = this.HiperLinkButton(null, this.loc.Cloud.PrivacyPolicy || "Privacy");
    buttonPrivacy.style.margin = "0 4px 0 4px";
    buttonPrivacy.caption.style.padding = "0px";
    buttonPrivacy.caption.style.whiteSpace = "normal";
    buttonPrivacy.style.display = "inline-block";
    buttonPrivacy.action = function () {
        authForm.this_.openNewWindow("https://www.stimulsoft.com/en/privacy-policy");
    }
    agreeTextContainer.appendChild(buttonPrivacy);

    var andText = this.CreateHTMLTable();
    andText.className = "stiDesignerTextContainer";
    andText.style.fontSize = "14px";
    andText.style.display = "inline-block";
    andText.addTextCell(textTermsAndPolicy ? textTermsAndPolicy.substring(textTermsAndPolicy.indexOf("{0}") + 3, textTermsAndPolicy.indexOf("{1}")) : "and");
    agreeTextContainer.appendChild(andText);

    var buttonTerms = this.HiperLinkButton(null, this.loc.Cloud.TermsOfUse || "Terms");
    buttonTerms.style.margin = "0 4px 0 4px";
    buttonTerms.caption.style.padding = "0px";
    buttonTerms.caption.style.whiteSpace = "normal";
    buttonTerms.style.display = "inline-block";
    buttonTerms.action = function () {
        var form = authForm.this_.InitializeLicenseForm();
        form.buttonSave.caption.innerHTML = authForm.this_.loc.Common.ButtonOK;
        form.buttonCancel.style.display = "none";
    }
    agreeTextContainer.appendChild(buttonTerms);

    var endText = this.CreateHTMLTable();
    endText.className = "stiDesignerTextContainer";
    endText.style.fontSize = "14px";
    endText.style.display = "inline-block";
    endText.addTextCell(textTermsAndPolicy ? textTermsAndPolicy.substring(textTermsAndPolicy.indexOf("{1}") + 3) : "");
    agreeTextContainer.appendChild(endText);

    //Button SignUp
    var buttonSignUp = this.LoginButton(null, this.loc.Authorization.ButtonSignUp, null);
    buttonSignUp.id = "sti_authform_buttonsignup";
    authForm.controls.buttonSignUp = buttonSignUp;
    buttonSignUp.style.margin = "3px 0 8px 0";
    buttonSignUp.style.display = "inline-block";
    controlsTable.addCellInNextRow(buttonSignUp).style.textAlign = "right";
    buttonSignUp.action = function () { authForm.action(); }
    this.AddSmallProgressMarkerToControl(buttonSignUp, true);

    var sep = this.SeparatorOr(315, "#c6c6c6");
    controlsTable.addCellInNextRow(sep).style.padding = "6px 0 6px 0";

    var buttonsTable = this.CreateHTMLTable();
    buttonsTable.style.width = "100%";
    controlsTable.addCellInNextRow(buttonsTable).style.padding = "10px 0 20px 0";

    //Google SignUp
    var butGoogleSignUp = this.ButtonLoginWith("Account.Google.png", "#dc4e41");
    butGoogleSignUp.id = "sti_authform_buttonsignupwithgoogle";
    butGoogleSignUp.style.margin = "0 6px 0 0";
    authForm.controls.buttonSignUpWithGoogle = butGoogleSignUp;
    this.AddSmallProgressMarkerToControl(butGoogleSignUp, true);
    buttonsTable.addCell(butGoogleSignUp);

    butGoogleSignUp.action = function () {
        this.jsObject.LoginWithGoogle(this);
    }

    //Facebook SignUp
    var butFacebookSignUp = this.ButtonLoginWith("Account.Facebook.png", "#1976d2");
    butFacebookSignUp.id = "sti_authform_buttonsignupwithfacebook";
    butFacebookSignUp.style.margin = "0 6px 0 6px";
    authForm.controls.buttonSignUpWithFacebook = butFacebookSignUp;
    this.AddSmallProgressMarkerToControl(butFacebookSignUp, true);
    buttonsTable.addCell(butFacebookSignUp);

    butFacebookSignUp.action = function () {
        this.jsObject.LoginWithFacebook(this);
    }

    //GitHub SignUp
    var butGitHubSignUp = this.ButtonLoginWith("Account.GitHub.png", "#2d333b");
    butGitHubSignUp.id = "sti_authform_buttonsignupwithgithub";
    butGitHubSignUp.style.margin = "0 0 0 6px";
    authForm.controls.buttonSignUpWithGitHub = butGitHubSignUp;
    this.AddSmallProgressMarkerToControl(butGitHubSignUp, true);
    buttonsTable.addCell(butGitHubSignUp);

    butGitHubSignUp.action = function () {
        this.jsObject.LoginWithGitHub(this);
    }

    //Button AlreadyHaveCloudReportsAccount
    var buttonAlreadyHaveAccount = this.HiperLinkButton(null, this.loc.Authorization.HyperlinkAlreadyHaveAccount, 23);
    authForm.controls.buttonAlreadyHaveAccount = buttonAlreadyHaveAccount;
    buttonAlreadyHaveAccount.style.margin = "0 0 15px 0";
    buttonAlreadyHaveAccount.style.display = "inline-block";
    rDiv.appendChild(buttonAlreadyHaveAccount);
    buttonAlreadyHaveAccount.action = function () { authForm.changeMode("login"); }

    return signUpPanel;
}

StiMobileDesigner.prototype.AuthFormResetPasswordPanel = function (authForm) {
    var resetPasswordPanel = document.createElement("div");
    var mTable = this.CreateHTMLTable();
    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.margin = "0 auto 0 auto";
    resetPasswordPanel.appendChild(mTable);
    mTable.style.width = "100%";
    var ct = mTable.addCell(controlsTable);
    ct.style.width = "50%";
    var rDiv = $("<div style='width: " + authForm.formWidth + "px;margin:0 auto;'></div>")[0];
    var ds = mTable.addCellInNextRow(rDiv);
    ds.style.textAlign = "center";
    ds.colSpan = 2;

    //UserName
    var hintText = this.loc.Administration.LabelNewPassword ? this.loc.Administration.LabelNewPassword.replace(":", "") : "";
    var passwordControl = this.TextBoxWithHintText(null, 280, hintText, hintText, "password.png");
    passwordControl.setAttribute("type", "password");
    authForm.controls.resetPasswordPassword = passwordControl;
    controlsTable.addCellInNextRow(passwordControl.table);

    //Button ResetPassword
    var buttonSetPassword = this.LoginButton(null, this.loc.Authorization.ButtonResetPassword, null);
    authForm.controls.buttonSetPassword = buttonSetPassword;
    buttonSetPassword.style.margin = "8px 0 45px 0";
    buttonSetPassword.style.display = "inline-block";
    controlsTable.addCellInNextRow(buttonSetPassword).style.paddingTop = "10px";
    var this_ = authForm.jsObject;

    buttonSetPassword.action = function () {
        authForm.jsObject.SendCloudCommand("UserResetPasswordComplete", { NewPassword: passwordControl.value, ResetKey: authForm.resetId },
            function (data) {
                authForm.startLogin(data.ResultUserName, passwordControl.value);
            });
    }
    this.AddSmallProgressMarkerToControl(buttonSetPassword, true);

    return resetPasswordPanel;
}

StiMobileDesigner.prototype.AuthFormForgotPasswordPanel = function (authForm) {
    var forgotPasswordPanel = document.createElement("div");
    var mTable = this.CreateHTMLTable();
    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.margin = "0 auto 0 auto";
    forgotPasswordPanel.appendChild(mTable);
    mTable.style.width = "100%";
    var ct = mTable.addCell(controlsTable);
    ct.style.width = "50%";
    var rDiv = $("<div style='width: " + authForm.formWidth + "px;margin:0 auto;'></div>")[0];
    var ds = mTable.addCellInNextRow(rDiv);
    ds.style.textAlign = "center";
    ds.colSpan = 2;

    //UserName
    var loginControl = this.TextBoxWithHintText(null, 280, this.loc.Authorization.TextUserName, this.loc.Authorization.TextUserName, "email.png");
    authForm.controls.forgotPasswordUserName = loginControl;
    controlsTable.addCellInNextRow(loginControl.table);

    //Button ResetPassword
    var buttonResetPassword = this.LoginButton(null, this.loc.Authorization.ButtonResetPassword, null);
    authForm.controls.buttonResetPassword = buttonResetPassword;
    buttonResetPassword.style.margin = "8px 0 8px 0";
    buttonResetPassword.style.display = "inline-block";
    controlsTable.addCellInNextRow(buttonResetPassword).style.paddingTop = "10px";
    buttonResetPassword.action = function () { authForm.action(); }
    this.AddSmallProgressMarkerToControl(buttonResetPassword, true);

    //Button Have Password
    var buttonHavePassword = this.HiperLinkButtonForAuthForm(null, this.loc.Authorization.HyperlinkHavePassword, true);
    authForm.controls.buttonHavePassword = buttonHavePassword;
    buttonHavePassword.style.display = "inline-block";
    controlsTable.addCellInNextRow(buttonHavePassword).setAttribute("style", "text-align: left; padding: 5px 0 25px 0;");
    buttonHavePassword.action = function () { this.jsObject.options.forms.authForm.changeMode("login"); }

    //Button Register Account
    var buttonRegisterAccount = this.HiperLinkButtonForAuthForm(null, this.loc.Cloud.ButtonSignUp, true);
    authForm.controls.buttonRegisterAccountForgotPassword = buttonRegisterAccount;
    buttonRegisterAccount.style.display = "inline-block";
    controlsTable.addCellInNextRow(buttonRegisterAccount).setAttribute("style", "text-align: center; border-top: 1px solid #f2f1f1; padding: 15px 0 15px 0;");
    buttonRegisterAccount.action = function () { this.jsObject.options.forms.authForm.changeMode("signUp"); }

    return forgotPasswordPanel;
}

StiMobileDesigner.prototype.AuthFormEnterPasswordPanel = function (authForm) {
    var enterPasswordPanel = document.createElement("div");
    var mTable = this.CreateHTMLTable();
    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.margin = "0 auto 0 auto";
    enterPasswordPanel.appendChild(mTable);
    mTable.style.width = "100%";
    var ct = mTable.addCell(controlsTable);
    ct.style.width = "50%";
    var rDiv = $("<div style='width: " + authForm.formWidth + "px;margin:0 auto;'></div>")[0];
    var ds = mTable.addCellInNextRow(rDiv);
    ds.style.textAlign = "center";
    ds.colSpan = 2;

    //Password
    var passwordControl = this.TextBoxWithHintText(null, 280, this.loc.Authorization.TextPassword, this.loc.Authorization.TextPassword, "password.png");
    passwordControl.setAttribute("type", "password");
    authForm.controls.enterPassword = passwordControl;
    controlsTable.addCellInNextRow(passwordControl.table);

    //Text
    var textPanel = document.createElement("div");
    textPanel.className = "stiDesignerAuthFormSuccessfullyText";
    textPanel.style.margin = "0";
    textPanel.innerHTML = "The account with this User Name(email) already exists.To associate it with your Google account, please use your Stimulsoft account password.";
    controlsTable.addCellInNextRow(textPanel).setAttribute("style", "display: inline-block; padding: 20px 0 0 0; width: 315px");

    //Button Send
    var buttonSendPassword = this.LoginButton(null, this.loc.A_WebViewer.ButtonSend, null);
    authForm.controls.buttonSendPassword = buttonSendPassword;
    buttonSendPassword.style.margin = "8px 0 8px 0";
    buttonSendPassword.style.display = "inline-block";
    controlsTable.addCellInNextRow(buttonSendPassword).style.paddingTop = "10px";
    buttonSendPassword.action = function () { authForm.action(); }
    this.AddSmallProgressMarkerToControl(buttonSendPassword, true);

    //Button Forgot Password
    var buttonForgotPassword = this.HiperLinkButtonForAuthForm(null, this.loc.Authorization.HyperlinkForgotPassword, true);
    authForm.controls.buttonForgotPassword2 = buttonForgotPassword;
    buttonForgotPassword.style.display = "inline-block";
    controlsTable.addCellInNextRow(buttonForgotPassword).setAttribute("style", "text-align: left; padding: 10px 0 10px 0;");
    buttonForgotPassword.action = function () { this.jsObject.options.forms.authForm.changeMode("forgotPassword"); }

    //Button LogIn
    var buttonLogin = this.HiperLinkButtonForAuthForm(null, this.loc.Authorization.ButtonLogin, true);
    authForm.controls.buttonLogin2 = buttonLogin;
    buttonLogin.style.display = "inline-block";
    controlsTable.addCellInNextRow(buttonLogin).setAttribute("style", "text-align: center; border-top: 1px solid #f2f1f1; padding: 15px 0 15px 0;");
    buttonLogin.action = function () { this.jsObject.options.forms.authForm.changeMode("login"); }

    return enterPasswordPanel;
}

StiMobileDesigner.prototype.AuthFormRegisterSuccessfullyPanel = function (authForm) {
    var registerSuccessfullyPanel = document.createElement("div");
    var mTable = this.CreateHTMLTable();
    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.margin = "0 auto 0 auto";
    registerSuccessfullyPanel.appendChild(mTable);
    mTable.style.width = "100%";
    var ct = mTable.addCell(controlsTable);
    ct.style.width = "50%";
    var rDiv = $("<div style='width: " + authForm.formWidth + "px;margin:0 auto;'></div>")[0];
    var ds = mTable.addCellInNextRow(rDiv);
    ds.style.textAlign = "center";
    ds.colSpan = 2;

    //Text
    var textPanel = document.createElement("div");
    textPanel.className = "stiDesignerAuthFormSuccessfullyText";
    authForm.controls.registerSuccessfullyText = textPanel;
    var cell = controlsTable.addCellInNextRow(textPanel);
    cell.className = "stiDesignerCaptionControls";
    cell.style.padding = "0px 30px 0px 30px";
    cell.style.width = "auto";
    cell.colSpan = 2;
    cell.style.width = "250px";

    //Button Resend Email
    var buttonResendEmail = this.LoginButton(null, this.loc.Authorization.ButtonResendEmail, null);
    authForm.controls.buttonResendEmail = buttonResendEmail;
    buttonResendEmail.style.margin = "8px 0 8px 0";
    buttonResendEmail.style.display = "inline-block";
    buttonResendEmail.style.minWidth = "315px";
    controlsTable.addCellInNextRow(buttonResendEmail).style.textAlign = "left";

    buttonResendEmail.action = function () {
        this.jsObject.SendCloudCommand("UserActivate", { UserName: authForm.controls.loginUserName.value, ResultSuccess: true }, function () {
            authForm.resetControls();
            textPanel.innerHTML = "";
        }, function () {
            authForm.resetControls();
            textPanel.innerHTML = "";
        });
    }

    //Button Continue
    var buttonContinue = this.HiperLinkButton(null, this.loc.Common.ButtonBack, 23);
    authForm.controls.buttonContinue = buttonContinue;
    buttonContinue.style.margin = "8px 0 45px 0";
    buttonContinue.style.display = "inline-block";
    rDiv.appendChild(buttonContinue);
    buttonContinue.action = function () {
        authForm.resetControls();
        authForm.changeMode("login");
    }

    return registerSuccessfullyPanel;
}

StiMobileDesigner.prototype.LoginButton = function (name, caption, imageName, minWidth, tooltip) {
    var button = this.SmallButton(name, null, caption || "", imageName, tooltip, null, "stiDesignerLoginButton");
    button.style.height = "34px";
    button.style.fontSize = "19px";
    button.style.width = "100%";

    button.setEnabled = function (state) {
        this.style.opacity = state ? "1" : "0.3";
        this.style.cursor = state ? "pointer" : "default";
        this.isEnabled = state;
        if (!state && !this.isOver) this.isOver = false;
        this.className = state ? (this.isOver ? this.overClass : (this.isSelected ? this.selectedClass : this.defaultClass)) : this.disabledClass;
    }

    button.innerTable.style.width = "100%";
    button.style.minWidth = (minWidth || 80) + "px";
    button.caption.style.textAlign = "center";
    button.style.cursor = "pointer";

    return button;
}

StiMobileDesigner.prototype.ButtonLoginWith = function (imageName, backColor) {
    var button = this.FormButton(null, null, null, imageName, null, { width: 32, height: 32 });
    button.style.border = "0";
    button.style.height = "32px";
    button.innerTable.style.width = "auto";
    button.innerTable.style.display = "inline-block";
    button.style.textAlign = "center";
    button.style.background = backColor;
    button.style.cursor = "pointer";
    button.style.border = "1px solid #ffffff";

    button.onmouseenter = function () {
        if (!this.isEnabled) return;
        this.isOver = true;
        this.style.opacity = "0.8";
    }

    button.onmouseleave = function () {
        if (!this.isEnabled) return;
        this.isOver = false;
        this.style.opacity = "1";
    }

    return button;
}

StiMobileDesigner.prototype.AddWheelPanel = function (authForm) {
    var panel = document.createElement("div");
    this.AddBigProgressMarkerToControl(panel);
    panel.progressMarker.changeVisibleState(true, 460, 350);

    return panel;
}

var captchaLoadedComplete = function (data) {
    if (window["grecaptcha"]) {
        var captchaContainer = document.getElementById("sti_authform_captcha_container");
        if (captchaContainer) {
            captchaContainer.style.marginTop = "17px";

            // eslint-disable-next-line no-undef
            window["captchaId"] = grecaptcha.render('sti_authform_captcha_container', {
                'sitekey': '6LdmZCwUAAAAAHuIu_7B2NgFC7ks7A9y0NBHf9YD',
                'callback': function (response) {
                    if (response) {
                        var buttonSignUp = document.getElementById("sti_authform_buttonsignup");
                        var buttonSignUpWithGoogle = document.getElementById("sti_authform_buttonsignupwithgoogle");
                        var buttonSignUpWithGitHub = document.getElementById("sti_authform_buttonsignupwithgithub");
                        var buttonSignUpWithFacebook = document.getElementById("sti_authform_buttonsignupwithfacebook");
                        if (buttonSignUp && buttonSignUpWithGoogle && buttonSignUpWithGitHub && buttonSignUpWithFacebook) {
                            buttonSignUp.completeCaptcha = true;
                            if (buttonSignUp.agreeTermsAndPolicy) {
                                buttonSignUp.setEnabled(true);
                                buttonSignUpWithGoogle.setEnabled(true);
                                buttonSignUpWithGitHub.setEnabled(true);
                                buttonSignUpWithFacebook.setEnabled(true);
                            }
                        }
                    }
                }
            });
        }
    }
}