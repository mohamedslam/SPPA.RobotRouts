
StiMobileDesigner.prototype.LoginWithGoogle = function (ownerButton, actionName) {
    var jsObject = this;
    var clientID = "87797872002-0ohi2vph6jt8cqtmt9qmacpu30cb5aac.apps.googleusercontent.com";
    var authorizationEndpoint = "https://accounts.google.com/o/oauth2/auth";
    var redirectUrl = this.options.googleAuthRedirectUrl;
    var authForm = this.options.forms.authForm;
    var profilePanel = this.options.profilePanel;
    var errorForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();

    var clearAllCookies = function () {
        jsObject.RemoveCookie("sti_GoogleAuthSessionKey");
        jsObject.RemoveCookie("sti_GoogleAuthUserKey");
        jsObject.RemoveCookie("sti_GoogleAuthEmail");
        jsObject.RemoveCookie("sti_GoogleAuthError");
        jsObject.RemoveCookie("sti_GoogleAuthException");
        jsObject.RemoveCookie("sti_GoogleAuthAction");
        jsObject.RemoveCookie("sti_GoogleAuthCurrentSessionKey");
        jsObject.RemoveCookie("sti_GoogleAuthEncryptingToken");
    };

    clearAllCookies();

    if (actionName) {
        jsObject.SetCookie("sti_GoogleAuthAction", "AddLoginWithGoogle");
        jsObject.SetCookie("sti_GoogleAuthCurrentSessionKey", jsObject.options.cloudParameters.sessionKey);
    }

    var authorizationRequestUrl = authorizationEndpoint + "?response_type=code&scope=email%20profile&redirect_uri=" + redirectUrl + "&client_id=" + clientID;
    window.open(authorizationRequestUrl, null, "width=600,height=700");

    //waiting google authorize result
    if (authForm || profilePanel) {
        var closeFormsTimer = 0;
        var waitResultTimer = setInterval(function () {
            if (!((authForm && authForm.visible) || (profilePanel && profilePanel.visible))) closeFormsTimer++;
            if (closeFormsTimer > 10) clearInterval(waitResultTimer);

            var sessionKey = jsObject.GetCookie("sti_GoogleAuthSessionKey");
            var userKey = jsObject.GetCookie("sti_GoogleAuthUserKey");
            var email = jsObject.GetCookie("sti_GoogleAuthEmail");
            var error = jsObject.GetCookie("sti_GoogleAuthError");
            var exception = jsObject.GetCookie("sti_GoogleAuthException");
            var encryptingToken = jsObject.GetCookie("sti_GoogleAuthEncryptingToken");

            if (sessionKey || error || exception || email) {
                clearAllCookies();
                clearInterval(waitResultTimer);

                if (actionName == "AddLoginWithGoogle" && email) {
                    if (profilePanel && profilePanel.visible) {
                        profilePanel.updateGoogleEmail(email);
                    }
                }
                else {
                    if (sessionKey && userKey) {
                        authForm.loginComplete(sessionKey, userKey);
                    }
                    else if (exception == "NotAssociatedWithYourAccount" && authForm.visible) {
                        authForm.googleEncryptingToken = encryptingToken;
                        authForm.googleEmail = email;
                        authForm.changeMode("enterPassword");
                    }
                    else if (error) {
                        errorForm.show(error);
                    }
                }
            }
        }, 500);
    }
}