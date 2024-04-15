
StiMobileDesigner.prototype.ExecuteAction = function (name) {
    var jsObject = this;
    switch (name) {
        case "newReport":
            {
                var newReportPanel = this.options.newReportPanel || (this.options.cloudMode || this.options.serverMode || this.options.standaloneJsMode ? this.InitializeCloudNewReportPanel() : this.InitializeNewReportPanel());
                newReportPanel.changeVisibleState(true);
                break;
            }
        case "infoReport":
            {
                var infoReportPanel = this.options.infoReportPanel || this.InitializeInfoReportPanel();
                infoReportPanel.changeVisibleState(true);
                break;
            }
        case "blankReportButton":
        case "blankReportButton_Cloud":
            {
                var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
                fileMenu.changeVisibleState(false);
                setTimeout(function () {
                    if (jsObject.options.cloudParameters && jsObject.options.cloudParameters.thenOpenWizard) {
                        jsObject.options.cloudParameters.thenOpenWizard = false;
                    }
                    else {
                        jsObject.ActionNewReport();
                    }
                }, 200);
                break;
            }
        case "invoiceReportButton":
        case "orderReportButton":
        case "quotationReportButton":
        case "labelReportButton": {
            jsObject.StartWizardForm2(name);
            break;
        }
        case "standartReportButton":
        case "masterDetailReportButton":
            {
                this.InitializeWizardForm(function (wizardForm) {
                    wizardForm.typeReport = name == "masterDetailReportButton" ? "MasterDetail" : "Standart";
                    wizardForm.dataSourcesFromServer = jsObject.options.report && jsObject.options.newReportDictionary == "DictionaryMerge"
                        ? jsObject.GetDataSourcesAndBusinessObjectsFromDictionary(jsObject.options.report.dictionary) : [];

                    var showWizard = function () {
                        jsObject.SendCommandCreateReport(function () {
                            wizardForm.changeVisibleState(true);
                        });
                    }

                    if (jsObject.options.report != null && jsObject.options.reportIsModified) {
                        var messageForm = jsObject.MessageFormForSave();
                        messageForm.changeVisibleState(true);
                        messageForm.action = function (state) {
                            if (state) {
                                jsObject.ActionSaveReport(function () { showWizard(); });
                            }
                            else {
                                showWizard();
                            }
                        }
                    }
                    else {
                        showWizard();
                    }
                });
                break;
            }
        case "blankDashboardButton":
        case "blankDashboardButton_Cloud":
            {
                var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
                fileMenu.changeVisibleState(false);
                setTimeout(function () {
                    jsObject.ActionNewDashboard();
                }, 200);
                break;
            }
        case "dashboardFinancialButton":
        case "dashboardOrdersButton":
        case "dashboardSalesOverviewButton":
        case "dashboardTicketsStatisticsButton":
        case "dashboardTrafficAnalyticsButton":
        case "dashboardVehicleProductionButton":
        case "dashboardWebsiteAnalyticsButton":
            {
                var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
                fileMenu.changeVisibleState(false);
                var dashboardName = name.substring("dashboard".length, name.length - "Button".length);
                setTimeout(function () {
                    jsObject.SendCommandOpenWizardDashboard(dashboardName);
                }, 200);
                break;
            }
        case "blankFormButton":
        case "blankFormButton_Cloud":
            {
                var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
                fileMenu.changeVisibleState(false);
                setTimeout(function () {
                    jsObject.ActionNewForm();
                }, 200);
                break;
            }
        case "openReport":
            {
                if (this.options.cloudMode || this.options.serverMode) {
                    var openPanel = this.options.openPanel || this.InitializeOpenPanel();
                    openPanel.changeVisibleState(true);
                }
                else {
                    var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
                    fileMenu.changeVisibleState(false);
                    setTimeout(function () { jsObject.ActionOpenReport(); }, 200);
                }
                break;
            }
        case "saveReport":
            {
                var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
                if (this.options.formsDesignerMode) {
                    if (!this.options.cloudParameters.reportTemplateItemKey && !this.options.formsDesignerFrame.formName) {
                        fileMenu.action(fileMenu.items.saveAsReport);
                        fileMenu.items.saveReport.setSelected(true);
                        if (this.options.saveAsPanel) this.options.saveAsPanel.header.innerHTML = "Save Form"; //TO DO localization
                    }
                    else {
                        jsObject.ActionSaveReport();
                    }
                }
                else if ((this.options.cloudMode || this.options.serverMode) && !this.options.cloudParameters.reportTemplateItemKey && !this.options.report.properties.reportFile) {
                    fileMenu.action(fileMenu.items.saveAsReport);
                    fileMenu.items.saveReport.setSelected(true);
                    if (this.options.saveAsPanel) this.options.saveAsPanel.header.innerHTML = this.loc.A_WebViewer.SaveReport;
                }
                else {
                    fileMenu.changeVisibleState(false);
                    setTimeout(function () { jsObject.ActionSaveReport(); }, 200);
                }
                break;
            }
        case "saveAsReport":
            {
                if (this.options.cloudMode || this.options.serverMode) {
                    var saveAsPanel = this.options.saveAsPanel || this.InitializeSaveAsPanel();
                    saveAsPanel.changeVisibleState(true);
                }
                else {
                    var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
                    fileMenu.changeVisibleState(false);
                    setTimeout(function () { jsObject.ActionSaveAsReport(); }, 200);
                }
                break;
            }
        case "closeReport":
            {
                var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
                fileMenu.changeVisibleState(false);
                setTimeout(function () { jsObject.ActionCloseReport(); }, 200);
                break;
            }
        case "exitDesigner":
            {
                if (this.options.isJava)
                    window.history.back();
                else {
                    var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
                    fileMenu.changeVisibleState(false);
                    setTimeout(function () { jsObject.ActionExitDesigner(); }, 200);
                }
                break;
            }
        case "closeFileMenu":
            {
                var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
                fileMenu.changeVisibleState(false);

                if (this.options.formsDesignerMode) {
                    jsObject.InitializeFormsDesignerFrame(function (frame) {
                        frame.show();
                    });
                }
                break;
            }
        case "aboutDesigner":
            {
                var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
                fileMenu.changeVisibleState(false);
                setTimeout(function () {
                    var aboutPanel = jsObject.options.aboutPanel || jsObject.InitializeAboutPanel();
                    aboutPanel.changeVisibleState(true);
                }, 200);
                break;
            }
        case "help":
            {
                var helpPanel = this.options.helpPanel || this.InitializeHelpPanel();
                helpPanel.changeVisibleState(true);
                break;
            }
        case "account":
            {
                var accountPanel = this.options.accountPanel || this.InitializeAccountPanel();
                accountPanel.changeVisibleState(true);
                break;
            }
        case "fileButton":
            {
                var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
                fileMenu.changeVisibleState(true);
                break;
            }
        case "homeToolButton":
            {
                this.options.workPanel.showPanel(this.options.homePanel);
                this.options.workPanel.changeVisibleState(true);
                break;
            }
        case "insertToolButton":
            {
                if (!this.options.insertPanel) this.InitializeInsertPanel();
                this.options.workPanel.showPanel(this.options.insertPanel);
                this.options.workPanel.changeVisibleState(true);
                break;
            }
        case "pageToolButton":
            {
                if (!this.options.pagePanel) this.InitializePagePanel();
                this.options.pagePanel.updateControls();
                this.options.workPanel.showPanel(this.options.pagePanel);
                this.options.workPanel.changeVisibleState(true);
                break;
            }
        case "layoutToolButton":
            {
                if (!this.options.layoutPanel) this.InitializeLayoutPanel();
                this.options.layoutPanel.updateControls();
                this.options.workPanel.showPanel(this.options.layoutPanel);
                this.options.workPanel.changeVisibleState(true);
                break;
            }
        case "previewToolButton":
            {
                if (this.options.currentForm) this.options.currentForm.changeVisibleState(false);
                if (this.options.previewMode) return;
                if (!this.options.previewPanel) this.InitializePreviewPanel();

                var viewerContainer = this.options.viewerContainer;
                if (viewerContainer.frame) {
                    viewerContainer.removeChild(viewerContainer.frame);
                    viewerContainer.frame = null;
                }
                if (this.options.viewer) {
                    this.options.viewer.style.display = "none";
                }
                if (this.options.serverMode && this.options.report && this.options.cloudParameters && !this.options.report.dashboardsPresent() && !this.NeedToUseNewViewer() && this.options.cloudParameters.itemCacheMode == "On") {
                    this.options.viewerContainer.addFrame();
                    if (this.options.buttons.previewToolButton) {
                        this.options.buttons.previewToolButton.progress.style.visibility = "visible";
                    }
                    this.options.workPanel.showPanel(this.options.previewPanel);
                    var reportName = this.options.report.properties.reportFile || "Report.mrt";
                    reportName = reportName.substring(0, reportName.length - 4);
                    if (this.options.cloudParameters.reportTemplateItemKey) {
                        reportName = this.options.cloudParameters.reportName;
                    }
                    this.ShowReportInTheViewer(reportName);
                }
                else {
                    if (!this.options.viewer && !this.options.mvcMode && !this.options.jsMode) {
                        var processImage = this.options.processImage || this.InitializeProcessImage();
                        processImage.show();
                        var viewerParameters = window["js" + jsObject.options.viewerId + "Parameters"];
                        var viewerCollections = window["js" + jsObject.options.viewerId + "Collections"];
                        var createViewer = function (viewerParameters, viewerCollections) {
                            if (viewerParameters) {
                                // eslint-disable-next-line no-undef
                                var jsViewer = window["js" + jsObject.options.viewerId] = new StiJsViewer(viewerParameters, viewerCollections);
                                jsObject.options.viewer = jsViewer.controls.viewer;
                                jsObject.options.viewerContainer.appendChild(jsObject.options.viewer);
                                jsObject.options.viewer.style.display = "";
                                jsViewer.options.jsDesigner = jsObject;
                            }
                            var waitViewerImages = setInterval(function () {
                                if (jsViewer.collections.images) {
                                    clearInterval(waitViewerImages);
                                    jsObject.options.workPanel.showPanel(jsObject.options.previewPanel);
                                    jsObject.options.workPanel.changeVisibleState(true);
                                    jsObject.options.processImage.hide();
                                    jsObject.SendCommandLoadReportToViewer();
                                }
                            }, 50);
                        }

                        if (typeof (StiJsViewer) != 'undefined') {
                            createViewer(viewerParameters, viewerCollections);
                        }
                        else {
                            this.LoadScript(viewerParameters.scriptsUrl, function () {
                                createViewer(viewerParameters, viewerCollections);
                            });
                        }
                    }
                    else {
                        if (this.options.viewer) {
                            this.options.viewer.style.display = "";
                            var viewerOptions = this.options.viewer.jsObject.options;
                            if (viewerOptions.currentMenu) viewerOptions.currentMenu.changeVisibleState(false);
                            if (viewerOptions.currentDatePicker) viewerOptions.currentDatePicker.changeVisibleState(false);
                        }
                        this.options.workPanel.showPanel(this.options.previewPanel);
                        this.options.workPanel.changeVisibleState(true);
                        this.SendCommandLoadReportToViewer();
                    }
                }
                break;
            }
        case "zoomIn":
            {
                var zoom = Math.round(this.options.report.zoom * 10) / 10;
                if (zoom <= 1.9) {
                    this.options.report.zoom = zoom > this.options.report.zoom ? zoom : zoom + 0.1;
                    this.PreZoomPage(this.options.currentPage);
                }
                else {
                    if (this.options.report.zoom != 2) {
                        this.options.report.zoom = 2;
                        this.PreZoomPage(this.options.currentPage);
                    }
                }
                break;
            }
        case "zoomOut":
            {
                var zoom = Math.round(this.options.report.zoom * 10) / 10;
                if (zoom >= 0.2) {
                    this.options.report.zoom = zoom < this.options.report.zoom ? zoom : zoom - 0.1;
                    this.PreZoomPage(this.options.currentPage);
                }
                else {
                    if (this.options.report.zoom != 0.1) {
                        this.options.report.zoom = 0.1;
                        this.PreZoomPage(this.options.currentPage);
                    }
                }
                break;
            }
        case "unitButton":
            {
                this.options.menus.unitMenu.changeVisibleState(!this.options.menus.unitMenu.visible);
                break;
            }
        case "insertBands":
            {
                this.options.menus.bandsMenu.changeVisibleState(!this.options.menus.bandsMenu.visible);
                break;
            }
        case "insertCrossBands":
            {
                this.options.menus.crossBandsMenu.changeVisibleState(!this.options.menus.crossBandsMenu.visible);
                break;
            }
        case "insertComponents":
            {
                this.options.menus.componentsMenu.changeVisibleState(!this.options.menus.componentsMenu.visible);
                break;
            }
        case "insertBarCodes":
            {
                this.options.menus.barCodesMenu.changeVisibleState(!this.options.menus.barCodesMenu.visible);
                break;
            }
        case "insertShapes":
            {
                this.options.menus.shapesMenu.changeVisibleState(!this.options.menus.shapesMenu.visible);
                break;
            }
        case "insertSignatures":
            {
                this.options.menus.signaturesMenu.changeVisibleState(!this.options.menus.signaturesMenu.visible);
                break;
            }
        case "insertCharts":
            {
                this.options.menus.chartsMenu.changeVisibleState(!this.options.menus.chartsMenu.visible);
                break;
            }
        case "insertMaps":
            {
                this.options.menus.mapsMenu.changeVisibleState(!this.options.menus.mapsMenu.visible);
                break;
            }
        case "insertGauges":
            {
                this.options.menus.gaugesMenu.changeVisibleState(!this.options.menus.gaugesMenu.visible);
                break;
            }
        case "insertPanelAddPage":
        case "addPage":
            {
                if (this.options.dashboardAssemblyLoaded && this.options.showNewPageButton === false) {
                    this.SendCommandAddDashboard(this.options.currentPage.properties.pageIndex);
                }
                else {
                    this.SendCommandAddPage(this.options.currentPage.properties.pageIndex);
                }
                break;
            }
        case "insertPanelAddDashboard":
        case "addDashboard":
            {
                this.SendCommandAddDashboard(this.options.currentPage.properties.pageIndex);
                break;
            }
        case "removePage":
            {
                this.options.currentPage.remove();
                break;
            }
        case "duplicatePage":
            {
                this.SendCommandDuplicatePage(this.options.currentPage.properties.pageIndex);
                break;
            }
        case "groupBlockPageSetupButton":
            {
                if (this.options.currentPage && this.options.currentPage.isDashboard) {
                    this.InitializeDashboardSetupForm(function (form) {
                        form.changeVisibleState(true);
                    });
                }
                else {
                    this.InitializePageSetupForm(function (pageSetupForm) {
                        pageSetupForm.changeVisibleState(true);
                    });
                }
                break;
            }
        case "removeComponent":
            {
                var currentPage = this.options.currentPage;
                if (currentPage && currentPage.isDashboard && currentPage.properties.dashboardViewMode == "Mobile") {
                    this.MoveSelectedComponentsToUnplaced();
                }
                else {
                    if (this.options.selectedObjects) this.RemoveComponent(this.options.selectedObjects);
                    else if (this.options.selectedObject) this.options.selectedObject.remove();
                }
                break;
            }
        case "copyComponent":
            {
                if (this.options.selectedObjects) this.CopyComponent(this.options.selectedObjects);
                else if (this.options.selectedObject) this.options.selectedObject.copy();
                break;
            }
        case "cutComponent":
            {
                if (this.options.selectedObjects) this.CutComponent(this.options.selectedObjects);
                else if (this.options.selectedObject) this.options.selectedObject.cut();
                break;
            }
        case "pasteComponent":
            {
                this.SendCommandGetFromClipboard();
                break;
            }
        case "aboutButton":
            {
                var aboutPanel = this.options.aboutPanel || this.InitializeAboutPanel();
                aboutPanel.changeVisibleState(true);
                break;
            }
        case "showToolBarButton":
            {
                this.options.workPanel.changeVisibleState(true);
                this.options.workPanel.visibleState = true;
                break;
            }
        case "hideToolbarButton":
            {
                this.options.workPanel.changeVisibleState(false);
                this.options.workPanel.visibleState = false;
                break;
            }
        case "undoButton":
            {
                this.SendCommandUndo();
                break;
            }
        case "redoButton":
            {
                this.SendCommandRedo();
                break;
            }
        case "resizeDesigner":
            {
                this.ResizeDesigner();
                break;
            }
        case "groupBlockBordersButton":
            {
                this.InitializeBorderSetupForm(function (borderSetupForm) {
                    borderSetupForm.showFunction = null;
                    borderSetupForm.actionFunction = null;
                    borderSetupForm.changeVisibleState(true);
                });
                break;
            }
        case "reportSetup":
            {
                setTimeout(function () {
                    jsObject.InitializeReportSetupForm(function (reportSetupForm) {
                        reportSetupForm.show();
                    });
                }, 200);
                break;
            }
        case "pageSetup":
            {
                if (this.options.currentPage && this.options.currentPage.isDashboard) {
                    this.InitializeDashboardSetupForm(function (form) {
                        form.changeVisibleState(true);
                    });
                }
                else {
                    this.InitializePageSetupForm(function (pageSetupForm) {
                        pageSetupForm.changeVisibleState(true);
                    });
                }
                break;
            }
        case "pageMoveLeft":
            {
                this.SendCommandPageMove("Left", this.options.currentPage.properties.pageIndex);
                break;
            }
        case "pageMoveRight":
            {
                this.SendCommandPageMove("Right", this.options.currentPage.properties.pageIndex);
                break;
            }
        case "openPage":
            {
                if (this.options.canOpenFiles) {
                    this.InitializeOpenDialog("loadPageFromFile", this.StiHandleOpenPage, ".pg");
                    this.options.openDialogs.loadPageFromFile.action();
                }
                break;
            }
        case "savePage":
            {
                this.SendCommandSavePage(this.options.currentPage.properties.pageIndex);
                break;
            }
        case "optionsDesigner":
            {
                var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
                fileMenu.changeVisibleState(false);
                setTimeout(function () {
                    jsObject.InitializeOptionsForm(function (optionsForm) {
                        optionsForm.show();
                    });
                }, 200);
                break;
            }
        case "groupBlockReportButton":
            {
                jsObject.InitializeReportSetupForm(function (reportSetupForm) {
                    reportSetupForm.show();
                });
                break;
            }
        case "publish":
            {
                var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
                fileMenu.changeVisibleState(false);
                this.options.buttons.buttonPublish.action();
                break;
            }
        case "share":
            {
                var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();
                fileMenu.changeVisibleState(false);

                if ((this.options.cloudMode || this.options.standaloneJsMode) && !this.CheckUserActivated()) return;
                if (this.options.standaloneJsMode && !this.CheckUserProductsExpired()) return;

                if (this.options.report && this.options.report.properties.calculationMode == "Compilation") {
                    var messageForm = this.MessageFormForRenderReportInCompilationMode();
                    messageForm.changeVisibleState(true);
                    return;
                }

                this.InitializeShareForm(function (shareForm) {
                    shareForm.show();
                });
                break;
            }
        case "viewQuery":
            {
                var currentPage = jsObject.options.currentPage;
                if (currentPage && currentPage.isDashboard) {
                    jsObject.InitializeViewQueryForm(function (form) {
                        form.show(currentPage);
                    });
                }
                break;
            }
    }
}