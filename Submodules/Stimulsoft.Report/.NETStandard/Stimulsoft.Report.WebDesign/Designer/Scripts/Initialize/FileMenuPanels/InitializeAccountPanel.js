
StiMobileDesigner.prototype.InitializeAccountPanel = function () {
    var accountPanel = document.createElement("div");
    this.options.accountPanel = accountPanel;
    this.options.mainPanel.appendChild(accountPanel);
    accountPanel.style.display = "none";
    accountPanel.className = "stiDesignerNewReportPanel";
    accountPanel.style.overflow = "auto";
    accountPanel.mainButtons = {};
    var jsObject = this;

    var header = this.FileMenuPanelHeader(this.loc.Cloud.Account);
    accountPanel.appendChild(header);

    var mainTable = this.CreateHTMLTable();
    accountPanel.appendChild(mainTable);
    mainTable.style.margin = "10px 30px 0px 30px";
    mainTable.style.height = "calc(100% - 130px)";

    var mainButtonsTable = this.CreateHTMLTable();
    mainTable.addCell(mainButtonsTable).className = "wizardFormStepsPanel";

    var additionalCell = mainTable.addCell();
    additionalCell.style.verticalAlign = "top";
    accountPanel.additionalCell = additionalCell;

    //Profile
    var profilePanel = this.InitializeFileMenuProfilePanel(additionalCell);
    this.options.mainPanel.appendChild(profilePanel);

    //Team
    var teamPanel = this.InitializeFileMenuTeamPanel(additionalCell);
    this.options.mainPanel.appendChild(teamPanel);

    //Subscriptions
    var subscriptionsPanel = this.InitializeFileMenuSubscriptionsPanel(additionalCell);
    this.options.mainPanel.appendChild(subscriptionsPanel);

    //CheckForUpdate
    var checkForUpdatePanel = this.InitializeFileMenuCheckForUpdatePanel(additionalCell);
    this.options.mainPanel.appendChild(checkForUpdatePanel);

    var mainButtons = [];
    mainButtons.push(["profile", this.loc.Cloud.TextProfile, "Account.Profile.png"]);
    mainButtons.push(["team", this.loc.Cloud.Team, "Account.Team.png"]);
    mainButtons.push(["subscriptions", this.loc.Cloud.Subscriptions, "Account.Subscriptions.png"]);
    mainButtons.push(["checkForUpdate", this.loc.Cloud.CheckForUpdate, "Account.CheckForUpdate.png"]);

    for (var i = 0; i < mainButtons.length; i++) {
        var button = this.FileMenuInnerPanelButton(mainButtons[i][0], "AccountPanelMainButtons", mainButtons[i][1], mainButtons[i][2]);
        button.style.margin = "0 6px 3px 0";
        mainButtonsTable.addCellInNextRow(button);
        accountPanel.mainButtons[mainButtons[i][0]] = button;

        button.action = function () {
            if (this.isSelected) return;
            this.setSelected(true);
            accountPanel.mode = this.name;

            profilePanel.hide();
            teamPanel.hide();
            subscriptionsPanel.hide();
            checkForUpdatePanel.hide();

            switch (this.name) {
                case "profile": {
                    profilePanel.show();
                    break;
                }
                case "team": {
                    teamPanel.show();
                    break;
                }
                case "subscriptions": {
                    subscriptionsPanel.show();
                    break;
                }
                case "checkForUpdate": {
                    checkForUpdatePanel.show();
                    break;
                }
            }
        }
    }

    accountPanel.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        if (state) {
            if (accountPanel.mode) {
                accountPanel.mainButtons[accountPanel.mode].action();
            }
            else {
                accountPanel.mainButtons.profile.action();
            }
            if ((jsObject.options.cloudMode && !jsObject.options.cloudParameters.sessionKey) || (jsObject.options.standaloneJsMode && !jsObject.options.SessionKey)) {
                jsObject.options.forms.authForm.show();
            }
        }
        else {
            profilePanel.hide();
            teamPanel.hide();
            subscriptionsPanel.hide();
            checkForUpdatePanel.hide();
            if (accountPanel.mode) accountPanel.mainButtons[accountPanel.mode].setSelected(false);
        }
    }

    return accountPanel;
}