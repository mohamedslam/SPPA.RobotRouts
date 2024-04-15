
StiMobileDesigner.prototype.InitializeOpenPanel = function () {
    var openPanel = document.createElement("div");
    openPanel.jsObject = this;
    this.options.openPanel = openPanel;
    this.options.mainPanel.appendChild(openPanel);
    openPanel.style.display = "none";
    openPanel.className = "stiDesignerNewReportPanel";
    openPanel.style.overflow = "auto";
    var jsObject = this;

    var header = this.FileMenuPanelHeader(this.loc.MainMenu.menuFileOpen.replace("&", "").replace("...", ""));
    openPanel.appendChild(header);

    var mainTable = this.CreateHTMLTable();
    openPanel.appendChild(mainTable);
    mainTable.style.margin = "10px 30px 0px 30px";
    mainTable.style.height = "calc(100% - 130px)";

    var mainButtonsTable = this.CreateHTMLTable();
    mainTable.addCell(mainButtonsTable).className = "wizardFormStepsPanel";

    var additionalCell = mainTable.addCell();
    additionalCell.style.verticalAlign = "top";
    openPanel.additionalCell = additionalCell;

    //Import
    var importButtonsTable = this.CreateHTMLTable();
    importButtonsTable.style.display = "none";
    additionalCell.appendChild(importButtonsTable);

    var importButtons = [];
    importButtons.push(["ActiveReports", "Active Reports", "Open.ImportFiles.png"]);
    importButtons.push(["ComponentOneReports", "ComponentOne Reports", "Open.ImportFiles.png"]);
    importButtons.push(["CrystalReports", "Crystal Reports", "Open.ImportFiles.png"]);
    importButtons.push(["FastReports", "FastReport.Net", "Open.ImportFiles.png"]);
    importButtons.push(["ReportSharpShooter", "Report SharpShooter", "Open.ImportFiles.png"]);
    importButtons.push(["RichText", "Rich Text Format", "Open.ImportFiles.png"]);
    importButtons.push(["ReportingServices", "Reporting Services", "Open.ImportFiles.png"]);
    importButtons.push(["TelerikReports", "Telerik Reporting", "Open.ImportFiles.png"]);
    importButtons.push(["VisualFoxPro", "Visual FoxPro", "Open.ImportFiles.png"]);

    for (var i = 0; i < importButtons.length; i++) {
        var button = this.FileMenuInnerPanelButton(importButtons[i][0], null, importButtons[i][1], importButtons[i][2]);
        button.style.margin = "0px 10px 3px 10px";
        importButtonsTable.addCellInNextRow(button);

        button.action = function () {
            if (this.name == "VisualFoxPro") {
                jsObject.ActionImportFile(this.name);
            }
            else {
                var this_ = this;
                var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
                fileMenu.changeVisibleState(false);
                setTimeout(function () {
                    jsObject.ActionImportFile(this_.name);
                }, 200);
            }
        }
    }

    openPanel.openReportFromCloudItem = function (itemObject, notSaveToRecent, isFormChanged) {
        var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
        fileMenu.changeVisibleState(false);

        setTimeout(function () {
            if (jsObject.options.formsDesignerMode) {
                if (isFormChanged == null) {
                    jsObject.InitializeFormsDesignerFrame(function (frame) {
                        frame.sendCommand({ action: "checkIsFormChanged", nextAction: "openReportFromCloudItem", params: { itemObject: itemObject, notSaveToRecent: notSaveToRecent } });
                    });
                }
                else {
                    if (isFormChanged) {
                        var messageForm = jsObject.MessageFormForSave();
                        messageForm.changeVisibleState(true);
                        messageForm.action = function (state) {
                            if (state) {
                                jsObject.ActionSaveReport(function () { jsObject.OpenReportFromCloud(itemObject, notSaveToRecent); });
                            }
                            else {
                                jsObject.OpenReportFromCloud(itemObject, notSaveToRecent);
                            }
                        }
                    }
                    else {
                        jsObject.OpenReportFromCloud(itemObject, notSaveToRecent);
                    }
                }
            }
            else {
                if (jsObject.options.report != null && jsObject.options.reportIsModified) {
                    var messageForm = jsObject.MessageFormForSave();
                    messageForm.changeVisibleState(true);
                    messageForm.action = function (state) {
                        if (state) {
                            jsObject.ActionSaveReport(function () { jsObject.OpenReportFromCloud(itemObject, notSaveToRecent); });
                        }
                        else {
                            jsObject.OpenReportFromCloud(itemObject, notSaveToRecent);
                        }
                    }
                }
                else {
                    jsObject.OpenReportFromCloud(itemObject, notSaveToRecent);
                }
            }
        });
    }

    //Online
    var onlinePanel = this.InitializeOnlineOpenReportPanel();
    jsObject.options.mainPanel.appendChild(onlinePanel);

    //Browse
    var browsePanel = this.InitializeBrowseOpenReportPanel();
    jsObject.options.mainPanel.appendChild(browsePanel);

    //Main Buttons
    var mainButtons = [];
    mainButtons.push(["recentFilesOpen", this.loc.FormDatabaseEdit.RecentConnections, "Open.RecentFiles.png"]);
    if (this.options.cloudMode || this.options.serverMode) {
        mainButtons.push(["onlineItemsOpen", this.getCloudName(), this.options.serverMode ? "Server.png" : "Open.Online.png"]);
    }
    else {
        mainButtons.push(["importFilesOpen", this.loc.ReportOpen.Import, "Open.ImportFiles.png"]);
    }
    mainButtons.push(["browseFilesOpen", this.loc.ReportOpen.Browse, "Open.OpenFiles.png"]);

    for (var i = 0; i < mainButtons.length; i++) {
        var button = this.FileMenuInnerPanelButton(mainButtons[i][0], "OpenPanelMainButtons", mainButtons[i][1], mainButtons[i][2]);
        button.style.margin = "0 6px 3px 0";
        mainButtonsTable.addCellInNextRow(button);

        button.action = function (ignoreSelect) {
            if (this.isSelected && !ignoreSelect) return;
            this.setSelected(true);
            openPanel.mode = this.name;
            importButtonsTable.style.display = "none";
            onlinePanel.style.display = "none";
            browsePanel.style.display = "none";
            if (openPanel.recentButtonsTable) openPanel.recentButtonsTable.style.display = "none";

            switch (this.name) {
                case "recentFilesOpen": {
                    if (openPanel.recentButtonsTable) openPanel.recentButtonsTable.style.display = "";
                    break;
                }
                case "importFilesOpen": {
                    importButtonsTable.style.display = "";
                    break;
                }
                case "onlineItemsOpen": {
                    onlinePanel.show();
                    break;
                }
                case "browseFilesOpen": {
                    if (jsObject.options.cloudMode) {
                        if (!jsObject.CheckUserActivated()) return;
                        browsePanel.show();
                    }
                    else {
                        var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
                        fileMenu.changeVisibleState(false);
                        setTimeout(function () { jsObject.ActionOpenReport(); }, 200);
                    }
                    break;
                }
            }
        }
    }

    openPanel.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        if (state) {
            //Recent
            jsObject.options.buttons.recentFilesOpen.action(true);

            if (openPanel.recentButtonsTable) additionalCell.removeChild(openPanel.recentButtonsTable);
            openPanel.recentButtonsTable = jsObject.CreateHTMLTable();
            additionalCell.appendChild(openPanel.recentButtonsTable);

            var recentArray = jsObject.GetRecentArray();
            if ((jsObject.options.cloudMode || jsObject.options.serverMode) && recentArray.length == 0) jsObject.options.buttons.onlineItemsOpen.action(true);

            for (var i = 0; i < recentArray.length; i++) {
                var icon = recentArray[i].containsForm ? "Open.Forms.png" : (recentArray[i].containsDashboard ? "Open.DashboardCloud.png" : "Open.ReportCloud.png");
                var caption = (jsObject.options.cloudMode || jsObject.options.serverMode) ? recentArray[i].name : recentArray[i].name + "<br>" + recentArray[i].path;
                var button = jsObject.FileMenuInnerPanelButton(null, null, caption, icon);
                button.style.margin = "0px 10px 3px 10px";
                if (jsObject.options.cloudMode || jsObject.options.serverMode) button.style.cursor = "pointer";
                button.fileObject = recentArray[i];
                openPanel.recentButtonsTable.addCellInNextRow(button);

                button.action = function () {
                    var this_ = this;
                    var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
                    fileMenu.changeVisibleState(false);
                    setTimeout(function () {
                        if (jsObject.options.cloudMode || jsObject.options.serverMode) {
                            openPanel.openReportFromCloudItem({ Name: this_.fileObject.name, Key: this_.fileObject.path });
                        }
                        else {
                            jsObject.ActionOpenRecentFile(this_.fileObject);
                        }
                    }, 200);
                }
            }
        }
        else {
            onlinePanel.style.display = "none";
            browsePanel.style.display = "none";
        }
    }

    return openPanel;
}

StiMobileDesigner.prototype.FileMenuInnerPanelButton = function (name, groupName, caption, image) {
    var button = this.SmallButton(name, groupName, caption, image, caption, null, null, true, { width: 32, height: 32 });
    if (button.imageCell) button.imageCell.style.padding = "0 10px 0 10px";
    if (button.caption) {
        button.caption.style.padding = "0 20px 0 10px";
        button.caption.style.lineHeight = "1.4";
    }
    button.style.minWidth = "250px";
    button.style.height = "50px";

    return button;
}

StiMobileDesigner.prototype.InitializeOnlineOpenReportPanel = function () {
    var jsObject = this;

    var form = this.BaseForm("onlineOpenReport", this.loc.MainMenu.menuFileOpen.replace("&", "").replace("...", ""), 1, this.HelpLinks["onlineOpenReport"]);
    form.style.width = "600px";
    form.style.border = "0";
    form.style.boxShadow = "none";
    form.style.position = "absolute";
    form.style.bottom = "15px";
    form.header.style.display = "none";
    form.container.style.borderTop = "0";
    form.container.style.position = "absolute";
    form.container.style.overflow = "auto";
    form.container.style.width = "600px";
    form.container.style.top = form.container.style.bottom = "0px";
    form.hideButtonsPanel();

    form.buttonOk.caption.innerHTML = this.loc.MainMenu.menuFileOpen.replace("&", "").replace("...", "");
    form.buttonOk.style.margin = "15px 15px 15px 0px";
    form.buttonCancel.style.margin = "15px 15px 15px 0px";

    form.buttonCancel.action = function () {
        var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
        fileMenu.changeVisibleState(false);
        form.changeVisibleState(false);
    }

    form.cancelAction = function () {
        form.buttonCancel.action();
    }

    //Online
    var onlineTree = jsObject.CloudTree();
    form.onlineTree = onlineTree;
    onlineTree.progress = jsObject.AddProgressToControl(form.container);
    form.container.appendChild(onlineTree);

    onlineTree.action = function (item) {
        if (item && item.itemObject.Ident == "ReportTemplateItem") {
            form.action();
        }
    }

    onlineTree.ondblClickAction = function (item) {
        form.action();
    }

    if (this.options.cloudMode) {
        //Cloud Demo text
        var text = "You can open your report from <a style='text-decoration: none;' href='https://cloud.stimulsoft.com/' target='_blank'>Stimulsoft Cloud.</a><br>" +
            "Please login using your Stimulsoft account credentials or register a new account";
        form.demoPanel = jsObject.CloudDemoPanel(text);
        form.container.appendChild(form.demoPanel);
    }

    form.setToLoginMode = function () {
        onlineTree.style.display = "none";
        if (form.demoPanel) form.demoPanel.style.display = "";
        form.buttonOk.style.display = "none";
        form.buttonCancel.style.display = "none";
        form.correctWidth(300);
    }

    form.setToTreeMode = function () {
        onlineTree.style.display = "";
        if (form.demoPanel) form.demoPanel.style.display = "none";
        form.buttonOk.style.display = "";
        form.buttonCancel.style.display = "";
        form.correctWidth(300);
        onlineTree.correctHeight(form);
        onlineTree.build();
    }

    form.show = function () {
        form.style.display = "";
        form.visible = true;
        form.buttonOk.setEnabled(false);
        form.style.left = (jsObject.FindPosX(jsObject.options.openPanel.additionalCell, "stiDesignerMainPanel") + 10) + "px";
        form.style.top = jsObject.FindPosY(jsObject.options.openPanel.additionalCell, "stiDesignerMainPanel") + "px";
        onlineTree.findControl.setValue("");

        if (jsObject.options.cloudParameters.sessionKey)
            form.setToTreeMode();
        else
            form.setToLoginMode();
    }

    form.action = function () {
        if (onlineTree.selectedItem && onlineTree.selectedItem.itemObject.Ident == "ReportTemplateItem") {
            form.changeVisibleState(false);

            setTimeout(function () {
                jsObject.options.openPanel.openReportFromCloudItem(onlineTree.selectedItem.itemObject);
            }, 200);
        }
    }

    return form;
}

StiMobileDesigner.prototype.InitializeBrowseOpenReportPanel = function () {
    var form = this.BaseForm("browseOpenReport", this.loc.MainMenu.menuFileOpen.replace("&", "").replace("...", ""), 1);
    form.style.width = "500px";
    var jsObject = this;

    if (this.options.cloudMode) {
        form.hideButtonsPanel();

        form.nextFunc = function () {
            form.style.display = "none";
            form.visible = false;
            var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
            fileMenu.changeVisibleState(false);
            setTimeout(function () {
                if (jsObject.options.cloudMode && jsObject.GetCloudPlanNumberValue() == 0) {
                    jsObject.InitializeNotificationForm(function (form) {
                        form.show(jsObject.NotificationMessages("openReportInTrial"), jsObject.NotificationMessages("upgradeYourPlan"), "Notifications.Blocked.png");
                    });
                    return;
                }
                jsObject.ActionOpenReport();
            }, 200);
        }

        var text = "Please login using your Stimulsoft account credentials or register a new account before opening report file.";
        form.demoPanel = jsObject.CloudDemoPanel(text);
        form.demoPanel.image.style.display = "none";
        form.demoPanel.style.height = "auto";
        form.demoPanel.style.margin = "15px 0 15px 0";
        form.container.appendChild(form.demoPanel);

        form.buttonCancel.action = function () {
            var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
            fileMenu.changeVisibleState(false);
        }

        form.cancelAction = function () {
            form.buttonCancel.action();
        }

        form.show = function () {
            if (jsObject.options.cloudMode && jsObject.options.cloudParameters && !jsObject.options.cloudParameters.sessionKey) {
                form.style.width = "500px";
                form.style.display = "";
                form.visible = true;
                form.style.left = (jsObject.FindPosX(jsObject.options.openPanel.additionalCell, "stiDesignerMainPanel") + 10) + "px";
                form.style.top = jsObject.FindPosY(jsObject.options.openPanel.additionalCell, "stiDesignerMainPanel") + "px";
                form.correctWidth(300);
            }
            else {
                form.nextFunc();
            }
        }
    }

    return form;
}

StiMobileDesigner.prototype.CloudDemoPanel = function (text) {
    var panel = document.createElement("div");
    panel.className = "stiCloudDemoPanel";

    //Cloud Image
    var img = document.createElement("img");
    img.style.margin = "25px 0 25px 0";
    img.style.width = "271px";
    img.style.height = "180px";
    StiMobileDesigner.setImageSource(img, this.options, "LoginControls.BigCloud.png");
    panel.image = img;
    panel.appendChild(img);

    //Cloud Text
    var textContainer = document.createElement("div");
    panel.textContainer = textContainer;
    textContainer.style.padding = "0px 35px 40px 35px";
    textContainer.style.lineHeight = "1.3";
    textContainer.innerHTML = text;
    panel.appendChild(textContainer);

    //Login
    var loginButton = this.LoginButton(null, this.loc.Authorization.ButtonLogin, null);
    loginButton.innerTable.style.width = "100%";
    loginButton.allwaysEnabled = true;
    loginButton.style.width = "200px";
    loginButton.style.margin = "0 auto";

    loginButton.action = function () {
        panel.action();
        this.jsObject.options.forms.authForm.show();
    }

    panel.appendChild(loginButton);

    //Register
    var buttonRegisterAccount = this.HiperLinkButtonForAuthForm(null, this.loc.Cloud.ButtonSignUp, true);
    buttonRegisterAccount.style.display = "inline-block";
    buttonRegisterAccount.style.margin = "25px 0 15px 0";
    panel.appendChild(buttonRegisterAccount);

    buttonRegisterAccount.action = function () {
        panel.action();
        this.jsObject.options.forms.authForm.changeMode("signUp");
        this.jsObject.options.forms.authForm.show(true);
    }

    panel.action = function () { };

    return panel
}

StiMobileDesigner.prototype.ActionOpenRecentFile = function (fileObject) { }

StiMobileDesigner.prototype.ActionImportFile = function (importType) { }