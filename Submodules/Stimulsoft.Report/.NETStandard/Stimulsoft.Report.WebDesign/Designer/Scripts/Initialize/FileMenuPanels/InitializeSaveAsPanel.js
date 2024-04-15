
StiMobileDesigner.prototype.InitializeSaveAsPanel = function () {
    var saveAsPanel = document.createElement("div");
    saveAsPanel.jsObject = this;
    this.options.saveAsPanel = saveAsPanel;
    this.options.mainPanel.appendChild(saveAsPanel);
    saveAsPanel.style.display = "none";
    saveAsPanel.className = "stiDesignerNewReportPanel";
    saveAsPanel.style.overflow = "auto";
    var jsObject = this;

    saveAsPanel.header = this.FileMenuPanelHeader(this.loc.MainMenu.menuFileSaveAs.replace("...", ""));
    saveAsPanel.appendChild(saveAsPanel.header);

    var mainTable = this.CreateHTMLTable();
    saveAsPanel.appendChild(mainTable);
    mainTable.style.margin = "10px 30px 0px 30px";
    mainTable.style.height = "calc(100% - 130px)";

    var mainButtonsTable = this.CreateHTMLTable();
    mainTable.addCell(mainButtonsTable).className = "wizardFormStepsPanel";
    var rightCell = mainTable.addCell();
    rightCell.className = "wizardFormStepsPanel";
    rightCell.style.border = "0";
    saveAsPanel.rightCell = rightCell;

    //Online
    var onlinePanel = this.InitializeOnlineSaveAsReportPanel();
    jsObject.options.mainPanel.appendChild(onlinePanel);

    //Browse
    var browsePanel = this.InitializeBrowseSaveAsReportPanel();
    jsObject.options.mainPanel.appendChild(browsePanel);

    saveAsPanel.getFileName = function (withExt) {
        return jsObject.GetReportFileName(withExt);
    }

    //Main Buttons
    var mainButtons = [];
    mainButtons.push(["onlineItemsSaveAs", this.getCloudName(), this.options.serverMode ? "Server.png" : "Open.Online.png"]);
    mainButtons.push(["browseFilesSaveAs", this.loc.ReportOpen.Browse, "Open.OpenFiles.png"]);

    for (var i = 0; i < mainButtons.length; i++) {
        var button = this.FileMenuInnerPanelButton(mainButtons[i][0], "SaveAsPanelMainButtons", mainButtons[i][1], mainButtons[i][2]);
        button.style.margin = "0 6px 3px 0";
        mainButtonsTable.addCellInNextRow(button);

        button.action = function (ignoreSelect) {
            if (this.isSelected && !ignoreSelect) return;
            this.setSelected(true);
            saveAsPanel.mode = this.name;
            onlinePanel.style.display = "none";
            browsePanel.style.display = "none";

            switch (this.name) {
                case "onlineItemsSaveAs": {
                    onlinePanel.show(saveAsPanel.getFileName(false), saveAsPanel.nextFunc);
                    break;
                }
                case "browseFilesSaveAs": {
                    browsePanel.show(saveAsPanel.getFileName(true), saveAsPanel.nextFunc);
                    break;
                }
            }
        }
    }

    saveAsPanel.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        jsObject.options.buttons.onlineItemsSaveAs.setSelected(false);
        jsObject.options.buttons.browseFilesSaveAs.setSelected(false);
        if (state) {
            jsObject.options.buttons.onlineItemsSaveAs.action();
            saveAsPanel.header.innerHTML = jsObject.loc.MainMenu.menuFileSaveAs.replace("...", "");
            saveAsPanel.nextFunc = null;
        }
        else {
            onlinePanel.style.display = "none";
            browsePanel.style.display = "none";
        }
    }

    return saveAsPanel;
}

StiMobileDesigner.prototype.InitializeOnlineSaveAsReportPanel = function () {
    var jsObject = this;
    var form = this.BaseForm("onlineSaveAsReport", this.loc.MainMenu.menuFileSaveAs.replace("...", ""), 1, this.HelpLinks["onlineSaveReport"]);
    form.style.width = form.container.style.width = "600px";
    form.style.border = "0";
    form.style.boxShadow = "none";
    form.style.position = "absolute";
    form.style.bottom = "15px";

    form.header.style.display = "none";
    form.container.style.borderTop = "0";
    form.container.style.position = "absolute";
    form.container.style.overflow = "auto";
    form.container.style.top = form.container.style.bottom = "0px";
    form.hideButtonsPanel();

    //Online
    var onlineTree = form.onlineTree = jsObject.CloudTree();
    onlineTree.style.bottom = "70px";
    onlineTree.progress = jsObject.AddProgressToControl(form.container);
    form.container.appendChild(onlineTree);

    //Cloud Demo text
    if (this.options.cloudMode) {
        var text = "You can save your report in <a style='text-decoration: none;' href='https://cloud.stimulsoft.com/' target='_blank'>Stimulsoft Cloud.</a><br>" +
            "Please login using your Stimulsoft account credentials or register a new account";
        form.demoPanel = jsObject.CloudDemoPanel(text);
        form.container.appendChild(form.demoPanel);
    }

    //Name Control
    var nameTable = this.CreateHTMLTable();
    nameTable.style.position = "absolute";
    nameTable.style.bottom = "0px";

    var nameLabel = nameTable.addTextCell(this.loc.PropertyMain.Name);
    nameLabel.className = "stiDesignerCaptionControls";
    nameLabel.style.padding = "0 25px 0 15px";

    var nameControl = form.nameControl = jsObject.TextBox(null, 350, 26);
    nameControl.style.marginRight = "12px";

    var cellControl = nameTable.addCell(nameControl);
    cellControl.style.textAlign = "right";

    form.buttonOk.caption.innerHTML = this.loc.A_WebViewer.SaveReport;
    form.buttonOk.style.margin = "12px 12px 12px 0px";
    form.buttonOk.style.height = "26px";
    form.insertBefore(nameTable, form.buttonsPanel);
    nameTable.addCell(form.buttonOk);

    onlineTree.action = function (item) {
        if (item.itemObject.Ident == "ReportTemplateItem") {
            nameControl.value = item.itemObject.Name;
        }
    }

    onlineTree.ondblClickAction = function (item) {
        if (item.itemObject.Ident == "ReportTemplateItem") {
            form.action();
        }
    }

    onlineTree.onbuildcomplete = function (item) {
        form.buttonOk.setEnabled(true);
        onlineTree.newFolderButton.setEnabled(true);
        nameControl.setEnabled(true);
        var newFreeName = onlineTree.getNewFreeName(nameControl.value);
        nameControl.value = newFreeName;
        nameControl.focus();
    }

    onlineTree.getNewFreeName = function (name) {
        var item = onlineTree.checkExistItem(name, "ReportTemplateItem");
        if (!item) return name;

        var i = 2;
        while (onlineTree.checkExistItem(name + i, "ReportTemplateItem")) {
            i++;
        }

        return name + i;
    }

    onlineTree.checkExistItem = function (itemName, itemIdent) {
        for (var i = 0; i < this.innerContainer.childNodes.length; i++) {
            var item = this.innerContainer.childNodes[i];
            if (item.itemObject && item.itemObject.Ident == itemIdent && item.itemObject.Name == itemName) {
                return item.itemObject;
            }
        }
        return false;
    }

    form.setToLoginMode = function () {
        onlineTree.style.display = "none";
        if (form.demoPanel) form.demoPanel.style.display = "";
        form.buttonOk.style.display = "none";
        onlineTree.newFolderButton.style.display = "none";
        nameTable.style.display = "none";
        form.correctWidth(300);
    }

    form.setToTreeMode = function () {
        onlineTree.style.display = "";
        if (form.demoPanel) form.demoPanel.style.display = "none";
        form.buttonOk.style.display = "";
        onlineTree.newFolderButton.style.display = "inline-block";
        nameTable.style.display = "";
        nameControl.setEnabled(false);
        form.correctWidth(300);
        onlineTree.correctHeight(form);
        onlineTree.build();
    }

    form.show = function (fileName, nextFunc) {
        form.style.display = "";
        form.visible = true;
        form.style.left = (jsObject.FindPosX(jsObject.options.saveAsPanel.rightCell, "stiDesignerMainPanel") + 10) + "px";
        form.style.top = jsObject.FindPosY(jsObject.options.saveAsPanel.rightCell, "stiDesignerMainPanel") + "px";
        nameControl.value = fileName;
        nameControl.focus();
        form.buttonOk.setEnabled(false);
        onlineTree.newFolderButton.setEnabled(false);
        onlineTree.findControl.setValue("");
        form.nextFunc = nextFunc;

        if (jsObject.options.cloudParameters.sessionKey)
            form.setToTreeMode();
        else
            form.setToLoginMode();
    }

    form.action = function () {
        var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
        var newItem = onlineTree.checkExistItem(nameControl.value, "ReportTemplateItem");
        if (newItem) {
            //Replase existing online item
            var messageReplaceForm = jsObject.MessageFormForReplaceItem(nameControl.value);
            messageReplaceForm.changeVisibleState(true);
            messageReplaceForm.action = function (state) {
                if (state) {
                    form.changeVisibleState(false);
                    if (!jsObject.options.previewMode) {
                        fileMenu.changeVisibleState(false);
                    }
                    setTimeout(function () {
                        jsObject.options.cloudParameters.reportTemplateItemKey = newItem.Key;
                        jsObject.options.cloudParameters.reportName = nameControl.value;
                        jsObject.SetWindowTitle(nameControl.value + " - " + jsObject.loc.FormDesigner.title);
                        jsObject.SendCommandItemResourceSave(newItem.Key);
                        if (form.nextFunc) form.nextFunc();
                    }, 200);
                }
            }
        }
        else {
            form.changeVisibleState(false);
            if (!jsObject.options.previewMode) {
                fileMenu.changeVisibleState(false);
            }
            setTimeout(function () {
                //Create new online item
                var folderKey = onlineTree.rootItem && onlineTree.rootItem.itemObject.Key != "root" ? onlineTree.rootItem.itemObject.Key : null;
                jsObject.AddNewReportItemToCloud(nameControl.value, folderKey);
                if (form.nextFunc) form.nextFunc();
            }, 200);
        }
    }

    nameControl.actionOnKeyEnter = function () {
        form.action();
    }

    return form;
}

StiMobileDesigner.prototype.InitializeBrowseSaveAsReportPanel = function () {
    var jsObject = this;
    var form = this.BaseForm("browseSaveAsReport", this.loc.MainMenu.menuFileSaveAs.replace("...", ""), 1);
    form.style.width = form.container.style.width = "500px";
    form.style.border = "0";
    form.style.boxShadow = "none";
    form.style.position = "absolute";
    form.style.bottom = "15px";

    form.header.style.display = "none";
    form.container.style.borderTop = "0";
    form.container.style.position = "absolute";
    form.container.style.overflow = "auto";
    form.container.style.top = form.container.style.bottom = "0px";
    form.hideButtonsPanel();

    //Name Control
    var nameTable = this.CreateHTMLTable();
    form.container.appendChild(nameTable);
    var nameLabel = nameTable.addTextCell(this.loc.PropertyMain.Name);
    nameLabel.className = "stiDesignerCaptionControls";
    var nameControl = this.TextBox(null, 400, 26);
    nameTable.addCell(nameControl).style.textAlign = "right";
    nameTable.style.margin = "12px";
    nameTable.style.width = "calc(100% - 24px)";

    var saveTypeTable = this.CreateHTMLTable();
    var textCell = saveTypeTable.addTextCell(this.loc.Cloud.SaveAsType);
    textCell.className = "stiDesignerCaptionControls";
    textCell.style.padding = "0px 20px 0px 19px";
    saveTypeTable.style.marginBottom = "3px";

    var saveType = this.DropDownList("saveAsType", 80, null, this.GetSaveTypeItems(), true, false, 26);
    saveTypeTable.addCell(saveType);

    var footerTable = this.CreateHTMLTable();
    footerTable.style.width = "100%";
    footerTable.addCell(saveTypeTable).style.width = "1px";
    footerTable.addCell();
    footerTable.addCell(form.buttonOk).style.width = "1px";
    form.container.appendChild(footerTable);
    form.buttonOk.caption.innerHTML = this.loc.A_WebViewer.SaveReport;
    form.buttonOk.style.margin = "12px 12px 12px 0px";

    //Cloud Demo text
    if (this.options.cloudMode) {
        var text = "Please login using your Stimulsoft account credentials or register a new account before saving report file.";
        form.demoPanel = jsObject.CloudDemoPanel(text);
        form.demoPanel.image.style.display = "none";
        form.demoPanel.style.height = "auto";
        form.demoPanel.style.margin = "15px 0 15px 0";
        form.container.appendChild(form.demoPanel);
    }

    form.show = function (fileName, nextFunc) {
        form.style.display = "";
        form.visible = true;
        form.style.left = (jsObject.FindPosX(jsObject.options.saveAsPanel.rightCell, "stiDesignerMainPanel") + 10) + "px";
        form.style.top = jsObject.FindPosY(jsObject.options.saveAsPanel.rightCell, "stiDesignerMainPanel") + "px";
        nameControl.value = fileName;
        nameControl.focus();
        form.nextFunc = nextFunc;

        saveType.setKey(jsObject.options.report && jsObject.options.report.isJsonReport ? "json" : "xml");
        saveTypeTable.style.display = (jsObject.options.cloudMode || jsObject.options.serverMode) && jsObject.options.report && !jsObject.options.report.encryptedPassword && jsObject.options.designerSpecification == "Developer" ? "" : "none";

        if (this.jsObject.options.cloudParameters.sessionKey) {
            nameTable.style.display = "";
            footerTable.style.display = "";
            if (form.demoPanel) form.demoPanel.style.display = "none";
        }
        else {
            nameTable.style.display = "none";
            footerTable.style.display = "none";
            if (form.demoPanel) form.demoPanel.style.display = "";
        }

        form.correctWidth(400);
    }

    form.action = function () {
        var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
        fileMenu.changeVisibleState(false);

        setTimeout(function () {            
            if (jsObject.options.cloudMode) {
                if (!jsObject.CheckUserActivated()) return;

                if (jsObject.GetCloudPlanNumberValue() == 0) {
                    jsObject.InitializeNotificationForm(function (form) {
                        form.show(jsObject.NotificationMessages("saveReportInTrial"), jsObject.NotificationMessages("upgradeYourPlan"), "Notifications.Blocked.png");
                    });
                    return;
                }
            }
                        
            jsObject.options.cloudParameters.reportTemplateItemKey = null;

            if (jsObject.options.formsDesignerMode) {
                jsObject.InitializeFormsDesignerFrame(function (frame) {
                    var fileName = nameControl.value || "Form.mrt";
                    if (!jsObject.EndsWith(fileName, ".mrt")) fileName += ".mrt";
                    frame.show();
                    frame.sendCommand({ action: "downloadForm", fileName: fileName });
                    jsObject.SetWindowTitle(fileName + " - " + jsObject.loc.FormDesigner.title);
                    jsObject.options.formsDesignerFrame.formName = fileName;
                    //jsObject.options.reportIsModified = false;
                });
            }
            else {
                var isNewReport = !jsObject.options.report.properties.reportFile;
                jsObject.options.report.properties.reportFile = nameControl.value;

                //Update designer title
                var reportFile = jsObject.options.report.properties.reportFile;
                if (reportFile != null) reportFile = reportFile.substring(reportFile.lastIndexOf("/")).substring(reportFile.lastIndexOf("\\"));
                var reportName = reportFile || StiBase64.decode(jsObject.options.report.properties.reportName.replace("Base64Code;", ""));
                jsObject.SetWindowTitle(reportName ? reportName + " - " + jsObject.loc.FormDesigner.title : jsObject.loc.FormDesigner.title);

                jsObject.SendCommandSaveAsReport(saveType.key, isNewReport);
                if (form.nextFunc) {
                    form.nextFunc();
                    form.nextFunc = null;
                }
            }
        }, 200);
    }

    nameControl.actionOnKeyEnter = function () {
        form.action();
    }

    return form;
}

StiMobileDesigner.prototype.InitializeOnlineSaveAsForm = function () {
    var jsObject = this;
    var form = this.BaseForm("onlineSaveAsReport", this.loc.MainMenu.menuFileSaveAs.replace("...", ""), 1, this.HelpLinks["onlineSaveReport"]);

    //Online
    var onlineTree = form.onlineTree = jsObject.CloudTree();
    onlineTree.style.position = "relative";
    onlineTree.progress = jsObject.AddProgressToControl(form.container);

    var innerContainer = onlineTree.innerContainer;
    innerContainer.style.position = "relative";
    innerContainer.style.height = "400px";
    innerContainer.style.width = "600px";
    innerContainer.style.top = "0";
    form.container.appendChild(onlineTree);
        
    //Name Control
    var nameTable = this.CreateHTMLTable();
    nameTable.style.width = "calc(100% - 24px)";
    nameTable.style.margin = "24px 0 12px 0";

    var nameLabel = nameTable.addTextCell(this.loc.PropertyMain.Name);
    nameLabel.className = "stiDesignerCaptionControls";
    nameLabel.style.padding = "0 25px 0 15px";

    var nameControl = form.nameControl = jsObject.TextBox(null, 450, 26);
    nameControl.style.marginRight = "12px";

    nameTable.addCell(nameControl).style.textAlign = "right";

    form.buttonOk.caption.innerHTML = this.loc.A_WebViewer.SaveReport;
    form.container.appendChild(nameTable);

    onlineTree.action = function (item) {
        if (item.itemObject.Ident == "ReportTemplateItem") {
            nameControl.value = item.itemObject.Name;
        }
    }

    onlineTree.ondblClickAction = function (item) {
        if (item.itemObject.Ident == "ReportTemplateItem") {
            form.action();
        }
    }

    onlineTree.onbuildcomplete = function (item) {
        form.buttonOk.setEnabled(true);
        onlineTree.newFolderButton.setEnabled(true);
        nameControl.setEnabled(true);
        var newFreeName = onlineTree.getNewFreeName(nameControl.value);
        nameControl.value = newFreeName;
        nameControl.focus();
    }

    onlineTree.getNewFreeName = function (name) {
        var item = onlineTree.checkExistItem(name, "ReportTemplateItem");
        if (!item) return name;

        var i = 2;
        while (onlineTree.checkExistItem(name + i, "ReportTemplateItem")) {
            i++;
        }

        return name + i;
    }

    onlineTree.checkExistItem = function (itemName, itemIdent) {
        for (var i = 0; i < this.innerContainer.childNodes.length; i++) {
            var item = this.innerContainer.childNodes[i];
            if (item.itemObject && item.itemObject.Ident == itemIdent && item.itemObject.Name == itemName) {
                return item.itemObject;
            }
        }
        return false;
    }

    form.show = function () {
        form.changeVisibleState(true);
        form.buttonOk.setEnabled(false);
        onlineTree.newFolderButton.setEnabled(false);
        onlineTree.findControl.setValue("");
        nameControl.value = jsObject.GetReportFileName();
        onlineTree.build();
    }

    form.action = function () {
        var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
        var newItem = onlineTree.checkExistItem(nameControl.value, "ReportTemplateItem");
        if (newItem) {
            //Replase existing online item
            var messageReplaceForm = jsObject.MessageFormForReplaceItem(nameControl.value);
            messageReplaceForm.changeVisibleState(true);
            messageReplaceForm.action = function (state) {
                if (state) {
                    form.changeVisibleState(false);
                    if (!jsObject.options.previewMode) {
                        fileMenu.changeVisibleState(false);
                    }
                    setTimeout(function () {
                        jsObject.options.cloudParameters.reportTemplateItemKey = newItem.Key;
                        jsObject.options.cloudParameters.reportName = nameControl.value;
                        jsObject.SetWindowTitle(nameControl.value + " - " + jsObject.loc.FormDesigner.title);
                        jsObject.SendCommandItemResourceSave(newItem.Key);
                        if (form.nextFunc) form.nextFunc();
                    }, 200);
                }
            }
        }
        else {
            form.changeVisibleState(false);
            if (!jsObject.options.previewMode) {
                fileMenu.changeVisibleState(false);
            }
            setTimeout(function () {
                //Create new online item
                var folderKey = onlineTree.rootItem && onlineTree.rootItem.itemObject.Key != "root" ? onlineTree.rootItem.itemObject.Key : null;
                jsObject.AddNewReportItemToCloud(nameControl.value, folderKey);
                if (form.nextFunc) form.nextFunc();
            }, 200);
        }
    }

    nameControl.actionOnKeyEnter = function () {
        form.action();
    }

    return form;
}