
StiMobileDesigner.prototype.ParseReport = function (reportInObject) {
    if (reportInObject == null) return null;
    return JSON.parse(reportInObject);
}

StiMobileDesigner.prototype.LoadReport = function (reportObject, notResetToHomePanel) {
    var jsObject = this;
    var attachedItems = (this.options.serverMode && this.options.report && this.options.report.dictionary.attachedItems)
        ? jsObject.CopyObject(jsObject.options.report.dictionary.attachedItems) : null;

    if (reportObject == null) {
        this.options.report = null;
        if (this.options.processImage) this.options.processImage.hide();
        return;
    }

    this.SetEnabledAllControls(true);

    if (!notResetToHomePanel) {
        this.options.workPanel.showPanel(this.options.homePanel);
        this.options.buttons.homeToolButton.setSelected(true);
    }

    if (!this.options.runWizardAfterLoad && !this.options.runSpecificWizardAfterLoad && (!this.options.wizardTypeRunningAfterLoad || this.options.wizardTypeRunningAfterLoad == "None") &&
        (!this.options.forms.authForm || (this.options.forms.authForm && !this.options.forms.authForm.visible)) &&
        (!this.options.menus.fileMenu || (this.options.menus.fileMenu && !this.options.menus.fileMenu.visible))) {
        this.options.workPanel.changeVisibleState(true);
    }

    this.options.clipboardMode = false;
    this.options.in_resize = null;
    this.options.in_drag = null;
    this.options.homePanel.updateControls();
    this.options.report = this.InitializeReportObject();
    if (reportObject.encryptedPassword != null) this.options.report.encryptedPassword = reportObject.encryptedPassword;
    this.options.report.isJsonReport = reportObject.isJsonReport;
    this.options.report.containsDashboard = reportObject.containsDashboard;
    this.options.report.containsForm = reportObject.containsForm;
    this.options.reportIsModified = false;
    this.options.report.zoom = this.StrToDouble(reportObject.zoom);
    this.options.controls.zoomScale.setZoomPosition();
    this.options.report.properties = reportObject.properties;
    this.options.report.info = reportObject.info;
    this.options.buttons.unitButton.updateCaption(this.options.report.properties.reportUnit);
    this.options.buttons.undoButton.setEnabled(jsObject.undoState != null && jsObject.options.jsMode ? jsObject.undoState : false);
    this.options.buttons.redoButton.setEnabled(jsObject.redoState != null && jsObject.options.jsMode ? jsObject.redoState : false);
    this.options.report.gridSize = this.StrToDouble(reportObject.gridSize);
    this.options.report.dictionary = reportObject.dictionary;
    if (attachedItems) this.options.report.dictionary.attachedItems = attachedItems;
    this.options.dictionaryTree.build(reportObject.dictionary, true);
    this.options.report.stylesCollection = reportObject.stylesCollection;
    this.options.report.pages = {};
    this.options.paintPanel.clear();
    this.ClearAllGalleries();
    this.UpdateResourcesFonts();
    if (this.options.showPreviewButton) this.options.buttons.previewToolButton.style.display = "";

    for (var indexPage = 0; indexPage < reportObject.pages.length; indexPage++) {
        var page = reportObject.pages[indexPage].properties.isDashboard
            ? this.CreateDashboard(reportObject.pages[indexPage])
            : this.CreatePage(reportObject.pages[indexPage]);

        page.repaint();
        this.options.paintPanel.addPage(page);
        this.options.report.pages[page.properties.name] = page;
        this.options.report.pages[page.properties.name].components = {};

        for (var numComponent = 0; numComponent < reportObject.pages[indexPage].components.length; numComponent++) {
            if (ComponentCollection[reportObject.pages[indexPage].components[numComponent].typeComponent]) {
                var component = reportObject.pages[indexPage].components[numComponent].properties.isDashboardElement
                    ? this.CreateDashboardElement(reportObject.pages[indexPage].components[numComponent])
                    : this.CreateComponent(reportObject.pages[indexPage].components[numComponent]);
                if (component) {
                    component.repaint();
                    this.options.report.pages[page.properties.name].components[component.properties.name] = component;
                }
            }
        }

        page.addComponents();
        page.updateWatermarkLevels();

        if (page.properties.pageIndex == 0) {
            this.options.paintPanel.showPage(page);
            this.options.currentPage.setSelected();
        }
    }

    if (this.GetCountObjects(reportObject.pages) == 0) {
        this.SetEnabledAllControls(false);
        var errorMessageForm = this.options.forms.errorMessageForm || this.InitializeErrorMessageForm();
        if (!errorMessageForm.visible) errorMessageForm.show(this.loc.Errors.Error);
    }
    this.options.pagesPanel.pagesContainer.updatePages();
    clearTimeout(this.options.timerUpdateCache);

    this.options.timerUpdateCache = setTimeout(function () {
        jsObject.SendCommandUpdateCache();
    }, this.options.timeUpdateCache);

    clearTimeout(this.options.timerAutoSave);
    var this_ = this;
    if (this.options.report.info.enableAutoSaveMode && this.options.report.info.autoSaveInterval) {
        this.options.timerAutoSave = setInterval(function () {
            if (this_.options.report) {
                if ((this_.options.cloudMode || this_.options.serverMode) && this_.options.cloudParameters && this_.options.cloudParameters.reportTemplateItemKey) {
                    this_.SendCommandItemResourceSave(this_.options.cloudParameters.reportTemplateItemKey);
                }
                else {
                    this_.ActionSaveReport();
                }
            }
        }, this.StrToInt(this.options.report.info.autoSaveInterval) * 60000);
    }

    var reportFile = this.options.report.properties.reportFile;
    if (reportFile != null) reportFile = reportFile.substring(reportFile.lastIndexOf("/")).substring(reportFile.lastIndexOf("\\"));
    var reportName = reportFile || StiBase64.decode(this.options.report.properties.reportName.replace("Base64Code;", ""));

    if (jsObject.options.cloudParameters && jsObject.options.cloudParameters.reportTemplateItemKey && jsObject.options.cloudParameters.reportName) {
        reportName = jsObject.options.cloudParameters.reportName;
    }

    if (!notResetToHomePanel) {
        this.SetWindowTitle(reportName ? reportName + " - " + this.loc.FormDesigner.title : this.loc.FormDesigner.title);
    }

    var processImage = this.options.processImage || this.InitializeProcessImage();
    processImage.hide();
    if (this.options.reportTree) {
        this.options.reportTree.reset();
        this.options.reportTree.build();
    }
}

StiMobileDesigner.prototype.CloseReport = function () {
    this.options.report = null;
    this.options.currentPage = null;
    this.options.selectedObject = null;
    this.options.reportGuid = null;
    this.options.reportIsModified = false;
    this.options.previewPageNumber = 0;
    this.SetEnabledAllControls(false);
    this.options.workPanel.showPanel(this.options.homePanel);
    this.options.buttons.homeToolButton.setSelected(true);
    this.options.paintPanel.clear();
    this.options.pagesPanel.pagesContainer.clear();
    this.options.dictionaryTree.clear();
    this.options.homePanel.updateControls();
    this.options.propertiesPanel.updateControls();
    this.ClearAllGalleries();
    if (this.options.showPreviewButton) this.options.buttons.previewToolButton.style.display = "none";
    if (this.options.layoutPanel) this.options.layoutPanel.updateControls();
    if (this.options.pagePanel) this.options.pagePanel.updateControls();
    if (this.options.dictionaryPanel) this.options.dictionaryPanel.createDataHintItem.style.display = "none";
    clearTimeout(this.options.timerUpdateCache);
    clearTimeout(this.options.timerAutoSave);
    if (!this.options.cloudMode && !this.options.serverMode) {
        this.SetWindowTitle(this.loc.FormDesigner.title);
    }
    if (this.options.buttons.reportCheckerButton) {
        this.options.buttons.reportCheckerButton.updateCaption();
    }
    if (this.options.reportTree) {
        this.options.reportTree.clear();
    }
    if (this.options.jsMode) {
        this.undoState = null;
        this.redoState = null;
    }
}

StiMobileDesigner.prototype.ActionCloseReport = function () {
    if (this.options.reportIsModified) {
        var messageForm = this.MessageFormForSave();
        messageForm.changeVisibleState(true);
        messageForm.action = function (state) {
            if (state) {
                var jsObject = this.jsObject;
                jsObject.ActionSaveReport(function () { jsObject.SendCommandCloseReport(); });
            }
            else { this.jsObject.SendCommandCloseReport(); }
        }
    }
    else this.SendCommandCloseReport();
}

StiMobileDesigner.prototype.ActionNewReport = function (isFormChanged) {
    var jsObject = this;

    this.options.workPanel.showPanel(this.options.homePanel);
    this.options.buttons.homeToolButton.setSelected(true);

    if (this.options.formsDesignerMode) {
        if (isFormChanged == null) {
            jsObject.InitializeFormsDesignerFrame(function (frame) {
                frame.sendCommand({ action: "checkIsFormChanged", nextAction: "newReport" });
            });
        }
        else {
            if (isFormChanged) {
                var messageForm = jsObject.MessageFormForSave();
                messageForm.changeVisibleState(true);
                messageForm.action = function (state) {
                    if (state) {
                        jsObject.ActionSaveReport(function () { jsObject.SendCommandCreateReport(null, true); });
                    }
                    else { this.jsObject.SendCommandCreateReport(null, true); }
                }
            }
            else { this.SendCommandCreateReport(null, true); }
        }
    }
    else {
        if (this.options.report != null) {
            if (this.options.reportIsModified) {
                var messageForm = jsObject.MessageFormForSave();
                messageForm.changeVisibleState(true);
                messageForm.action = function (state) {
                    if (state) {
                        jsObject.ActionSaveReport(function () { jsObject.SendCommandCreateReport(null, true); });
                    }
                    else { jsObject.SendCommandCreateReport(null, true); }
                }
            }
            else { this.SendCommandCreateReport(null, true); }
        }
        else { this.SendCommandCreateReport(); }
    }
}

StiMobileDesigner.prototype.ActionNewDashboard = function (isFormChanged) {
    var jsObject = this;

    this.options.workPanel.showPanel(this.options.homePanel);
    this.options.buttons.homeToolButton.setSelected(true);

    if (this.options.formsDesignerMode) {
        if (isFormChanged == null) {
            jsObject.InitializeFormsDesignerFrame(function (frame) {
                frame.sendCommand({ action: "checkIsFormChanged", nextAction: "newDashboard" });
            });
        }
        else {
            if (isFormChanged) {
                var messageForm = jsObject.MessageFormForSave();
                messageForm.changeVisibleState(true);
                messageForm.action = function (state) {
                    if (state) {                        
                        jsObject.ActionSaveReport(function () { jsObject.SendCommandCreateDashboard(null, true); });
                    }
                    else { jsObject.SendCommandCreateDashboard(null, true); }
                }
            }
            else { this.SendCommandCreateDashboard(null, true); }
        }
    }
    else {
        if (this.options.report != null) {
            if (this.options.reportIsModified) {
                var messageForm = jsObject.MessageFormForSave();
                messageForm.changeVisibleState(true);
                messageForm.action = function (state) {
                    if (state) {
                        jsObject.ActionSaveReport(function () { jsObject.SendCommandCreateDashboard(null, true); });
                    }
                    else { jsObject.SendCommandCreateDashboard(null, true); }
                }
            }
            else { this.SendCommandCreateDashboard(null, true); }
        }
        else { this.SendCommandCreateDashboard(); }
    }
}

StiMobileDesigner.prototype.ActionNewForm = function (isFormChanged) {
    var jsObject = this;

    this.options.workPanel.showPanel(this.options.homePanel);
    this.options.buttons.homeToolButton.setSelected(true);

    if (this.options.formsDesignerMode) {
        if (isFormChanged == null) {
            jsObject.InitializeFormsDesignerFrame(function (frame) {
                frame.sendCommand({ action: "checkIsFormChanged", nextAction: "newForm" });
            });
        }
        else {
            if (isFormChanged) {
                var messageForm = jsObject.MessageFormForSave();
                messageForm.changeVisibleState(true);
                messageForm.action = function (state) {
                    if (state) {
                        jsObject.ActionSaveReport(function () { jsObject.SendCommandCreateForm(); });
                    }
                    else { jsObject.SendCommandCreateForm(); }
                }
            }
            else { this.SendCommandCreateForm(); }
        }
    }
    else {
        if (this.options.report != null) {
            if (this.options.reportIsModified) {
                var messageForm = jsObject.MessageFormForSave();
                messageForm.changeVisibleState(true);
                messageForm.action = function (state) {
                    if (state) {
                        jsObject.ActionSaveReport(function () { jsObject.SendCommandCreateForm(); });
                    }
                    else { jsObject.SendCommandCreateForm(); }
                }
            }
            else { this.SendCommandCreateForm(); }
        }
        else { this.SendCommandCreateForm(); }
    }
}

StiMobileDesigner.prototype.ActionOpenReport = function (isFormChanged) {
    var jsObject = this;
    this.options.workPanel.showPanel(this.options.homePanel);
    this.options.buttons.homeToolButton.setSelected(true);
    this.InitializeOpenDialog("openReport", this.StiHandleOpenReport, ".mrt,.mrz,.mrx");

    var openFunc = function () {
        if (jsObject.options.showOpenDialog === false) {
            jsObject.SendCommandOpenReport("", "", {});
        }
        else {
            jsObject.options.openDialogs.openReport.action();
        }
    }

    if (this.options.formsDesignerMode) {
        if (isFormChanged == null) {
            jsObject.InitializeFormsDesignerFrame(function (frame) {
                frame.sendCommand({ action: "checkIsFormChanged", nextAction: "openReport" });
            });
        }
        else {
            if (isFormChanged) {
                var messageForm = jsObject.MessageFormForSave();
                messageForm.changeVisibleState(true);
                messageForm.action = function (state) {
                    if (state)
                        jsObject.ActionSaveReport(openFunc);
                    else
                        openFunc();
                }
            }
            else {
                openFunc();
            }
        }
    }
    else {
        if (this.options.report != null && this.options.reportIsModified) {
            var messageForm = jsObject.MessageFormForSave();
            messageForm.changeVisibleState(true);
            messageForm.action = function (state) {
                if (state)
                    jsObject.ActionSaveReport(openFunc);
                else
                    openFunc();
            }
        }
        else {
            openFunc();
        }
    }
}

StiMobileDesigner.prototype.ActionSaveReport = function (nextFunc) {
    if (this.options.cloudMode || this.options.serverMode) {
        if (!this.options.cloudParameters.sessionKey) {
            var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
            fileMenu.changeVisibleState(true);
            fileMenu.action(fileMenu.items.saveAsReport);
            fileMenu.items.saveReport.setSelected(true);

            if (this.options.saveAsPanel) {
                this.options.saveAsPanel.header.innerHTML = this.options.formsDesignerMode ? "Save Form" : this.loc.A_WebViewer.SaveReport; // TO DO lcalization
                this.options.saveAsPanel.nextFunc = nextFunc;
            }
        }
        else {
            if (this.options.cloudParameters.reportTemplateItemKey) {
                var fileMenu = this.options.menus.fileMenu;
                if (fileMenu && fileMenu.visible) {
                    fileMenu.changeVisibleState(false);
                }
                this.InitializeSaveDescriptionForm(function (saveDescriptionForm) {
                    saveDescriptionForm.nextFunc = nextFunc || null;
                    if (saveDescriptionForm.jsObject.options.requestChangesWhenSaving) {
                        saveDescriptionForm.changeVisibleState(true);
                        saveDescriptionForm.textArea.focus();
                    }
                    else {
                        saveDescriptionForm.action(true);
                    }
                });
            }
            else {
                var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
                fileMenu.changeVisibleState(true);
                fileMenu.action(fileMenu.items.saveAsReport);
                fileMenu.items.saveReport.setSelected(true);

                if (this.options.saveAsPanel) {
                    this.options.saveAsPanel.header.innerHTML = this.loc.A_WebViewer.SaveReport;
                    this.options.saveAsPanel.nextFunc = nextFunc;
                }
            }
        }
    }
    else if (this.options.showSaveDialog && !this.options.report.properties.reportFile) {
        this.InitializeSaveReportForm(function (saveReportForm) {
            saveReportForm.show(false, nextFunc);
        });
    }
    else {
        this.SendCommandSaveReport(!this.options.report.properties.reportFile);
        if (nextFunc) nextFunc();
    }
}

StiMobileDesigner.prototype.ActionSaveAsReport = function () {
    if (this.options.showSaveDialog) {
        this.InitializeSaveReportForm(function (saveReportForm) {
            saveReportForm.show(true);
        });
    }
    else {
        this.SendCommandSaveAsReport(null, !this.options.report.properties.reportFile);
    }
}

StiMobileDesigner.prototype.OpenReportFromCloud = function (itemObject, notSaveToRecent) {
    var jsObject = this;

    var params = {
        itemObject: itemObject,
        sessionKey: this.options.cloudParameters.sessionKey,
        cloudMode: this.options.cloudMode,
        designerOptions: this.GetCookie("StimulsoftMobileDesignerOptions")
    };

    if ((jsObject.options.cloudMode || jsObject.options.standaloneJsMode) && (!jsObject.CheckUserTrExpired() || !jsObject.CheckUserActivated()))
        return;

    this.SendCommandToDesignerServer("LoadReportFromCloud", params, function (answer) {
        if (answer.formContent) {
            if (!jsObject.options.formsDesignerFrame) {
                answer.loadingCompleted = false;
            }            
            if (jsObject.options.cloudParameters) {
                jsObject.options.cloudParameters.reportTemplateItemKey = itemObject.Key;
                jsObject.options.cloudParameters.reportName = itemObject.Name;
            }
            jsObject.CloseReport();
            jsObject.InitializeFormsDesignerFrame(function (frame) {
                frame.openForm(itemObject.Name, answer.formContent);
            });

            if (!notSaveToRecent) jsObject.SaveFileToRecentArray(itemObject.Name, itemObject.Key, false, true);
        }
        else if (answer.reportObject && answer.reportGuid) {
            if (jsObject.options.formsDesignerFrame) {
                jsObject.options.formsDesignerFrame.close();
            }

            jsObject.options.cloudParameters.reportTemplateItemKey = itemObject.Key;
            jsObject.options.cloudParameters.reportName = itemObject.Name;
            jsObject.CloseReport();
            jsObject.options.reportGuid = answer.reportGuid;

            var reportObject = jsObject.ParseReport(answer.reportObject);
            jsObject.LoadReport(reportObject);
            jsObject.SetWindowTitle(itemObject.Name + " - " + jsObject.loc.FormDesigner.title);

            if (!notSaveToRecent) jsObject.SaveFileToRecentArray(itemObject.Name, itemObject.Key, reportObject.containsDashboard, false);
        }
        else {
            var errorMessage = answer["errorMessage"] || jsObject.loc.Notices.IsNotFound.replace("{0}", itemObject.Name);
            var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
            errorMessageForm.show(errorMessage);
        }
    });
}

StiMobileDesigner.prototype.AddNewReportItemToCloud = function (reportName, folderKey) {
    var jsObject = this;

    var params = {
        SaveEmptyResources: true,
        AllowSignalsReturn: true,
        Items: [{
            Ident: "ReportTemplateItem",
            Key: this.generateKey(),
            Name: reportName,
            Description: "",
            AttachedItems: this.options.report ? this.options.report.getAttachedItems() : []
        }]
    }

    if (folderKey) params.Items[0].FolderKey = folderKey;

    if ((jsObject.options.cloudMode || jsObject.options.standaloneJsMode) && (!jsObject.CheckUserTrExpired() || !jsObject.CheckUserActivated()))
        return;

    var processImage = this.options.processImage || this.InitializeProcessImage();
    processImage.show();

    this.SendCloudCommand("ItemSave", params,
        function (data) {
            processImage.hide();
            jsObject.SendCommandItemResourceSave(params.Items[0].Key);
            jsObject.options.cloudParameters.reportTemplateItemKey = params.Items[0].Key;
            jsObject.options.cloudParameters.reportName = reportName;
            jsObject.SetWindowTitle(reportName + " - " + jsObject.loc.FormDesigner.title);
        },
        function (data, msg) {
            processImage.hide();
            if (msg || data) {
                var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                errorMessageForm.show(msg || jsObject.formatResultMsg(data));
            }
        });
}

StiMobileDesigner.prototype.InitializeReportObject = function () {
    var report = {
        jsObject: this,
        typeComponent: "StiReport",
        properties: {},
        info: this.CopyObject(this.options.defaultDesignerOptions)
    };

    report.setSelected = function () {
        this.jsObject.SetSelectedObject(this);
    }

    report.getAttachedItems = function () {
        if (this.jsObject.options.cloudParameters && this.jsObject.options.cloudParameters.attachedItems) {
            return this.jsObject.CopyObject(this.jsObject.options.cloudParameters.attachedItems);
        }
        return [];
    }

    report.getComponentByName = function (name) {
        for (var pageName in this.pages) {
            if (this.pages[pageName].components[name])
                return this.pages[pageName].components[name];
        }
        for (var pageName in this.pages) {
            for (var componentName in this.pages[pageName].components) {
                var comp = this.pages[pageName].components[componentName];
                if (comp.typeComponent == "StiCrossTab") {
                    var crossTabChilds = comp.controls.crossTabContainer.childNodes;
                    for (var i = 0; i < crossTabChilds.length; i++) {
                        if (name == crossTabChilds[i].properties.name)
                            return crossTabChilds[i];
                    }
                }
            }
        }
        return null;
    }

    report.repaintAllSvgContents = function (svgContents) {
        for (var pageName in svgContents) {
            var pageContents = svgContents[pageName];
            if (this.pages[pageName]) {
                for (var componentName in pageContents) {
                    var component = this.pages[pageName].components[componentName];
                    if (component) {
                        component.properties.svgContent = pageContents[componentName];
                        component.repaint();
                    }
                }
            }
        }
    }

    report.dashboardsPresent = function () {
        for (var pageName in this.pages) {
            if (this.pages[pageName].isDashboard)
                return true;
        }
        return false;
    }

    report.pagesPresent = function () {
        for (var pageName in this.pages) {
            if (!this.pages[pageName].isDashboard)
                return true;
        }
        return false;
    }

    report.tableOfContentsPresent = function () {
        for (var pageName in this.pages) {
            for (var componentName in this.pages[pageName].components) {
                var comp = this.pages[pageName].components[componentName];
                if (comp.typeComponent == "StiTableOfContents") return true;
            }
        }
        return false;
    }

    return report;
}