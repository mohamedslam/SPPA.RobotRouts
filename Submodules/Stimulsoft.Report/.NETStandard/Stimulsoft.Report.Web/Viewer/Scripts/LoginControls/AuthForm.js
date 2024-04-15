StiJsViewer.prototype.InitializeAuthForm = function () {
    var jsObject = this;
    var authForm = this.BaseForm("authForm", this.collections.loc["AuthorizationWindowTitleLogin"].toUpperCase(), 2);
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
    authForm.container.className = "stiJsViewerAuthFormContainer";
    authForm.header.className = "stiJsViewerAuthFormHeader";
    authForm.caption.className = "stiJsViewerAuthFormCaption";

    authForm.container.style.padding = "0px";
    authForm.caption.style.padding = "45px 133px 45px 33px";
    var imgLogo = document.createElement("img");
    imgLogo.style.position = "absolute";
    imgLogo.style.top = "50px";
    imgLogo.style.right = "42px";
    imgLogo.style.width = "87px";
    imgLogo.style.height = "21px";
    StiJsViewer.setImageSource(imgLogo, this.options, this.collections, this.options.serverMode ? "LoginControls.LogoStimulsoftServer.png" : "LoginControls.LogoStimulsoftCloud.png");
    authForm.header.style.position = "relative";
    authForm.header.appendChild(imgLogo);

    //Add Panels
    authForm.this_ = this;
    authForm.panels = {};
    authForm.panels.login = this.AuthFormLoginPanel(authForm);
    authForm.container.appendChild(authForm.panels.login);
    authForm.panels.login.style.display = "";

    authForm.resetControls = function () {
        for (var name in this.controls) {
            if ("value" in this.controls[name] && this.controls[name] != this.controls.loginUserName)
                this.controls[name].value = "";
        }
    }

    authForm.show = function () {
        this.resetControls();
        this.changeVisibleState(true);
        this.panels.login.style.display = "";
        this.controls.loginUserName.focus();
    }

    authForm.action = function () {

    }

    return authForm;
}

StiJsViewer.prototype.AuthFormLoginPanel = function (authForm) {
    var loginPanel = document.createElement("div");
    var mTable = this.CreateHTMLTable();
    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.margin = "0 auto 0 auto";
    loginPanel.appendChild(mTable);
    mTable.style.width = "100%";
    var ct = mTable.addCell(controlsTable);
    ct.style.width = "50%";
    var loginRememberMe = true;
    var rDiv = document.createElement("div");
    rDiv.style.width = authForm.formWidth + "px";
    rDiv.style.margin = "0 auto";
    rDiv.style.borderTop = "";
    var ds = mTable.addCellInNextRow(rDiv);
    ds.style.textAlign = "center";
    ds.colSpan = 2;

    //UserName
    var loginControl = this.TextBoxWithHintText(null, 230, this.collections.loc["AuthorizationTextUserName"], this.collections.loc["AuthorizationTextUserName"], "login.png");
    authForm.controls.loginUserName = loginControl;
    controlsTable.addCellInNextRow(loginControl.table);
    loginControl.value = this.GetCookie("loginLogin") || "";

    //Password
    var passwordControl = this.TextBoxWithHintText(null, 230, this.collections.loc["AuthorizationTextPassword"], this.collections.loc["AuthorizationTextPassword"], "password.png");
    passwordControl.setAttribute("type", "password");
    authForm.controls.loginPassword = passwordControl;
    var passwControlCell = controlsTable.addCellInNextRow(passwordControl.table);
    passwControlCell.style.paddingTop = "17px";
    passwordControl.value = this.GetCookie("loginPassword") || "";

    var buttonsTable = this.CreateHTMLTable();
    controlsTable.addCellInNextRow(buttonsTable);
    buttonsTable.style.width = "100%";

    //Button Login
    var buttonLogin = this.LoginButton(null, this.collections.loc["AuthorizationButtonLogin"], null);
    authForm.controls.buttonLogin = buttonLogin;
    buttonLogin.style.margin = "8px 0 8px 0";
    buttonLogin.style.display = "inline-block";
    buttonsTable.addCellInLastRow(buttonLogin).style.paddingTop = "20px";
    buttonLogin.style.marginBottom = "80px";

    buttonLogin.action = function () {
        authForm.action();
    }

    return loginPanel;
}

StiJsViewer.prototype.AddAuthNotices = function (name, caption, imageName, minWidth, tooltip) {
    if (this.collections.loc && !this.collections.loc.Notices) {
        this.collections.loc.Notices = {
            AuthAccountCantBeUsedNow: "The account cannot be used now!",
            AuthAccountIsNotActivated: "The account is not activated yet! Please follow the instructions sent to the Email during registration.",
            AuthCantChangeRoleBecauseLastAdministratorUser: "The user role cannot be changed because this is the last administrator user in this workspace!",
            AuthCantChangeRoleBecauseLastSupervisorUser: "The user role cannot be changed because this is the last supervisor user at this server!",
            AuthCantChangeSystemRole: "The system role cannot be changed!",
            AuthCantDeleteHimselfUser: "The user cannot delete himself!",
            AuthCantDeleteLastAdministratorUser: "The user cannot be deleted because this is the last administrator user in this workspace!",
            AuthCantDeleteLastSupervisorUser: "The user cannot be deleted because this is the last supervisor user at this server!",
            AuthCantDeleteSystemRole: "Cannot delete this role, because it is a system role!",
            AuthCantDisableUserBecauseLastAdministratorUser: "The user cannot be disabled because this is the last administrator user in this workspace!",
            AuthCantDisableUserBecauseLastSupervisorUser: "The user cannot be disabled because this is the last supervisor user at this server!",
            AuthFirstNameIsNotSpecified: "The first name is not specified!",
            AuthLastNameIsNotSpecified: "The last name is not specified!",
            AuthOAuthIdNotSpecified: "The OAuth identificator is not specified!",
            AuthPasswordIsNotCorrect: "The password is not correct!",
            AuthPasswordIsNotSpecified: "The password is not specified!",
            AuthPasswordIsTooShort: "The password is too short (a minimum length is 6 chars)!",
            AuthRoleCantBeDeletedBecauseUsedByUsers: "You cannot delete the role because it is used by other users.",
            AuthRoleNameAlreadyExists: "The role with the specified name \"{0}\" already exists!",
            AuthRoleNameIsSystemRole: "The role with the specified name \"{0}\" is a system role!",
            AuthSendMessageWithInstructions: "A message with further instructions is sent to \"{0}\"!",
            AuthUserHasLoggedOut: "You have logged out!",
            AuthUserNameAlreadyExists: "The username (Email) is already in use!",
            AuthUserNameIsNotSpecified: "The username (Email) is not specified!",
            AuthUserNameOrPasswordIsNotCorrect: "The username (Email) or password is incorrect!",
            AuthUserNameShouldLookLikeAnEmailAddress: "The username should be similar to the Email address!",
            AuthWorkspaceNameAlreadyInUse: "The workspace name is already in use!</AuthWorkspaceNameAlreadyInUse"
        }
    }
}