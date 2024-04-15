
StiMobileDesigner.prototype.InitializeUserMenu = function () {
    var jsObject = this;
    var menu = this.VerticalMenu("userMenu", this.options.toolBar.userNameButton, "Down", []);
    menu.style.top = "40px";
    menu.style.right = "0px";
    menu.innerContent.className = "stiDesignerMenu";
    menu.innerContent.style.borderTop = "0px";
    menu.innerContent.style.borderRight = "0px";
    menu.innerContent.style.whiteSpace = "nowrap";
    this.options.userMenu = menu;

    this.options.toolBar.userNameButton.action = function () {
        menu.changeVisibleState(!menu.visible);
    }
        
    var mainTable = this.CreateHTMLTable();
    menu.innerContent.appendChild(mainTable);
    menu.userImageCell = mainTable.addCell();
    menu.userImageCell.style.padding = "15px";

    var buttonsTable = this.CreateHTMLTable();
    mainTable.addCell(buttonsTable).style.paddingTop = "20px";

    menu.nameCell = buttonsTable.addCell();
    menu.nameCell.style.padding = "0 45px 15px 0";
    menu.nameCell.style.fontSize = "12px";
    menu.nameCell.style.fontFamily = "Arial";

    var accountButton = this.HiperLinkButton(null, this.loc.Cloud.Account, 23);
    accountButton.style.fontSize = "13px";
    buttonsTable.addCellInNextRow(accountButton);

    accountButton.action = function () {
        menu.changeVisibleState(false);
        var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
        fileMenu.changeVisibleState(true);
        setTimeout(function () { fileMenu.items.account.action(); }, 200);
    }

    var goToCloudButton = this.HiperLinkButton(null, "Stimulsoft Cloud", 23);
    goToCloudButton.style.fontSize = "13px";
    goToCloudButton.style.marginRight = "25px";
    buttonsTable.addCellInNextRow(goToCloudButton);

    goToCloudButton.action = function () {
        menu.changeVisibleState(false);
        var url = "https://cloud.stimulsoft.com/main.aspx";

        if (jsObject.options.standaloneJsMode) {
            if (jsObject.options.SessionKey && jsObject.options.UserKey) {
                url += ("?_loc=" + (jsObject.loc["@cultureName"] || "en") + "&_sessionkey=" + jsObject.options.SessionKey + "&_userkey=" + jsObject.options.UserKey);
            }
        }
        else {
            var cloudParameters = jsObject.options.cloudParameters;
            if (cloudParameters && cloudParameters.sessionKey && cloudParameters.userKey) {
                url += ("?_loc=" + (jsObject.localizationControl.locName || "en") + "&_sessionkey=" + cloudParameters.sessionKey + "&_userkey=" + cloudParameters.userKey);
            }
        }

        jsObject.openNewWindow(url);
    }

    var downloadButton = this.HiperLinkButton(null, "Install Designer App", 23);
    downloadButton.style.fontSize = "13px";
    downloadButton.style.marginBottom = "15px";
    buttonsTable.addCellInNextRow(downloadButton);

    downloadButton.action = function () {
        menu.changeVisibleState(false);
        var osName = jsObject.GetOSName();
        var url = "https://www.stimulsoft.com/en/downloads/reports";
        var ver = jsObject.options.shortProductVersion;

        switch (osName) {
            case "MacOS": url = "https://admin.stimulsoft.com/install/Stimulsoft-Designer-" + ver + ".dmg"; break;
            case "Linux": url = "https://admin.stimulsoft.com/install/stimulsoft-designer_" + ver + "-1_amd64.deb"; break;
            case "Windows": url = "https://admin.stimulsoft.com/install/Stimulsoft-Designer-" + ver + ".exe"; break;
        }

        jsObject.openNewWindow(url);
    }

    var footer = this.CreateHTMLTable();
    menu.innerContent.appendChild(footer);
    footer.style.width = "100%";

    var logOutButton = this.FormButton(null, null, this.loc.Navigator.ButtonLogout, null, null, null, null, "stiDesignerFormButtonTheme");
    logOutButton.style.margin = "0 12px 12px 12px";
    logOutButton.style.display = "inline-block";
    var fcell = footer.addCell(logOutButton);
    fcell.style.textAlign = "right";
    fcell.style.lineHeight = "0";

    logOutButton.action = function () {
        menu.changeVisibleState(false);
        jsObject.FinishSession();

        if (jsObject.options.standaloneJsMode) {
            jsObject.options.forms.authForm.show();
        }
    }

    menu.changeVisibleState = function (state) {
        if (state) {
            this.onshow();
            this.style.opacity = 0;
            this.style.display = "";
            this.visible = true;
            if (this.parentButton) this.parentButton.setSelected(true);
            this.jsObject.options.currentMenu = this;
            this.style.width = this.innerContent.offsetWidth + "px";
            this.style.height = this.innerContent.offsetHeight + "px";
            $(this).animate({ opacity: 1 }, { duration: this.jsObject.options.formAnimDuration });
        }
        else {
            this.visible = false;
            if (this.parentButton) this.parentButton.setSelected(false);
            this.style.display = "none";
            if (this.jsObject.options.currentMenu == this) this.jsObject.options.currentMenu = null;
        }
    }

    return menu;
}