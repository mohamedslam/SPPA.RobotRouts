
StiMobileDesigner.prototype.OverrideToolbar = function () {
    var jsObject = this;
    var toolBar = this.options.toolBar;
    var toolBarTable = toolBar.firstChild;
    if (this.options.buttons.aboutButton) this.options.buttons.aboutButton.style.display = "none";
    if (this.options.buttons.showToolBarButton) this.options.buttons.showToolBarButton.style.display = "none";

    //LogIn
    var loginButton = this.ToolButtonAdditional("login", this.loc.Authorization.ButtonLogin);
    loginButton.allwaysEnabled = true;
    loginButton.style.marginLeft = "3px";
    toolBarTable.addCell(loginButton);
    toolBar.loginButton = loginButton;

    loginButton.action = function () {
        jsObject.options.forms.authForm.show();
    }

    //SignUp
    var signUpButton = this.SmallButton("signUp", null, this.loc.Cloud.ButtonSignUp, null, null, null, "stiDesignerFormButtonTheme")
    signUpButton.style.margin = "0 3px 0 3px";
    signUpButton.style.border = "0";
    signUpButton.style.height = "30px";
    signUpButton.allwaysEnabled = true;
    signUpButton.style.display = "none";
    signUpButton.caption.style.padding = "0 17px 0 17px";
    toolBarTable.addCell(signUpButton);
    toolBar.signUpButton = signUpButton;


    signUpButton.action = function () {
        jsObject.options.forms.authForm.changeMode("signUp");
        jsObject.options.forms.authForm.show(true);
    }

    //UserName
    var userNameButton = toolBar.userNameButton = this.ToolButtonAdditional("userName", " ");
    userNameButton.caption.style.padding = "0px 2px 0 10px";
    userNameButton.style.margin = "0 3px 0 3px";
    userNameButton.allwaysEnabled = true;
    userNameButton.style.webkitAppRegion = "no-drag";
    userNameButton.style.display = "none";
    toolBarTable.addCell(userNameButton);

    var userNameTable = this.CreateHTMLTable();
    userNameTable.style.webkitAppRegion = "no-drag";
    userNameButton.nameCell = userNameTable.addCell();
    userNameButton.caption.appendChild(userNameTable);
    userNameButton.userImageCell = userNameButton.innerTable.addCell();
    userNameButton.userImageCell.style.lineHeight = "0";

    this.InitializeUserMenu();
}