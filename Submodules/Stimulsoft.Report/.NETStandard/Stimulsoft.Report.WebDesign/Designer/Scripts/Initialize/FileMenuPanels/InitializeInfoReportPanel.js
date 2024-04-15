
StiMobileDesigner.prototype.InitializeInfoReportPanel = function () {
    var infoReportPanel = document.createElement("div");
    var jsObject = infoReportPanel.jsObject = this;
    this.options.infoReportPanel = infoReportPanel;
    this.options.mainPanel.appendChild(infoReportPanel);
    infoReportPanel.style.display = "none";
    infoReportPanel.className = "stiDesignerNewReportPanel";

    var header = this.FileMenuPanelHeader(this.loc.ReportInfo.Info);
    infoReportPanel.appendChild(header);

    var buttonsTable = this.CreateHTMLTable();
    buttonsTable.style.margin = "10px 30px 30px 30px";
    infoReportPanel.appendChild(buttonsTable);

    var infoButtons = [];
    if (this.options.showFileMenuReportSetup)
        infoButtons.push(["reportSetup", this.loc.ReportInfo.ReportOptions, "ReportInfo.ReportOptions.png", "<b style='font-size: 14px;'>" + this.loc.ReportInfo.ReportOptions + "</b><br>" + this.loc.ReportInfo.ReportOptionsAdditionalDescription]);

    infoButtons.push(["reportEncrypt", this.loc.ReportInfo.EncryptWithPassword, "ReportInfo.EncryptDocument.png", "<b style='font-size: 14px;'>" + this.loc.ReportInfo.EncryptWithPasswordDescription + "</b><br>" + this.loc.ReportInfo.EncryptWithPasswordAdditionalDescription]);
    infoButtons.push(["reportCheckIssues", this.loc.MainMenu.menuCheckIssues, "ReportInfo.CheckIssues.png", "<b style='font-size: 14px;'>" + this.loc.MainMenu.menuCheckIssues + "</b><br>" + this.loc.ReportInfo.CheckIssuesAdditionalDescription]);

    for (var i = 0; i < infoButtons.length; i++) {
        var button = this.FileMenuPanelButton(infoButtons[i][0], infoButtons[i][1], infoButtons[i][2], infoButtons[i][3]);
        buttonsTable.addCellInNextRow(button).style.paddingBottom = "30px";

        button.action = function () {
            var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
            fileMenu.changeVisibleState(false);
            switch (this.name) {
                case "reportSetup": {
                    jsObject.ExecuteAction("reportSetup");
                    break;
                }
                case "reportEncrypt": {
                    var passwordForm = jsObject.options.forms.passwordForm || jsObject.InitializePasswordForm();
                    passwordForm.show(function (password) {
                        if (jsObject.options.report) {
                            jsObject.options.report.encryptedPassword = password;
                        }
                    }, jsObject.loc.Password.lbPasswordSave.replace(":", ""));
                    break;
                }
                case "reportCheckIssues": {
                    setTimeout(function () {
                        jsObject.SendCommandGetReportCheckItems(function (answer) {
                            if (jsObject.options.buttons.reportCheckerButton) {
                                jsObject.options.buttons.reportCheckerButton.updateCaption(answer.checkItems);
                            }
                            jsObject.InitializeReportCheckForm(function (reportCheckForm) {
                                reportCheckForm.show(answer.checkItems);
                            });
                        });
                    }, 200);
                    break;
                }

            }
        }
    }

    infoReportPanel.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        if (state) {
            this.jsObject.options.buttons.reportEncrypt.setEnabled(this.jsObject.options.report && this.jsObject.options.report.encryptedPassword == null);
        }
    }

    return infoReportPanel;
}

StiMobileDesigner.prototype.FileMenuPanelButton = function (name, caption, image, description) {
    var button = this.CreateHTMLTable();
    if (name != null) this.options.buttons[name] = button;
    button.name = name;
    button.isEnabled = true;
    button.jsObject = this;
    button.allwaysEnabled = true;

    var innerButton = this.BigButton(null, null, caption, image, caption, null, "stiDesignerSmallButtonWithBorder", true);
    innerButton.style.width = innerButton.style.height = "80px";
    button.addCell(innerButton);
    innerButton.action = function () { button.action(); }

    var descriptionCell = button.addTextCell(description);
    descriptionCell.className = "stiDesignerDescriptionForBigButton";

    button.action = function () { };

    button.setEnabled = function (state) {
        this.isEnabled = state;
        descriptionCell.style.opacity = state ? "1" : "0.3";
        innerButton.setEnabled(state);
    }

    return button;
}

StiMobileDesigner.prototype.FileMenuPanelHeader = function (text) {
    var panel = document.createElement("div");
    panel.className = "stiDesignerFileMenuPanelHeader";
    panel.style.webkitAppRegion = "drag"; //for js standalone
    panel.innerHTML = text;

    return panel;
}