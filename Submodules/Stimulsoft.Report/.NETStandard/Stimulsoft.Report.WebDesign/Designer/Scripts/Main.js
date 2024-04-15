function StiMobileDesigner(parameters) {
    var jsObject = this;
    this.defaultParameters = {};
    this.options = parameters;
    this.options.buttons = {};
    this.options.controls = {};
    this.options.menus = {};
    this.options.forms = {};
    this.options.radioButtons = {};
    this.options.callbackFunctions = {};
    this.options.openDialogs = {};
    this.options.properties = {};
    this.options.propertiesGroups = {};
    this.options.dataBasesTreeOpeningArray = {};
    this.options.dataBasesTreeOpeningArrayTemp = {};
    this.options.paintPanelPadding = 15;
    this.options.previewPageNumber = 0;
    this.options.previewCountPages = 0;
    this.options.commands = [];
    this.options.abortedCommands = {};
    this.options.touchZoom = {};
    this.options.startZoom = 0;
    this.options.oldDeltaPos = 0;
    this.options.timeUpdateCache = 60000;
    this.options.uploadBlockSize = 10000000;
    this.options.modifyRestrictions = true;
    this.options.mobileDesigner = document.getElementById(parameters.mobileDesignerId);
    this.options.mainPanel = document.getElementById(jsObject.options.mobileDesigner.id + "_MainPanel");
    this.options.head = document.getElementsByTagName("head")[0];
    this.options.isTouchDevice = parameters.interfaceType == "Auto" ? this.IsTouchDevice() : (parameters.interfaceType == "Touch" || parameters.interfaceType == "Mobile");
    this.options.canOpenFiles = window.File && window.FileReader && window.FileList && window.Blob;
    this.options.menuAnimDuration = parameters.showAnimation ? 150 : 0;
    this.options.formAnimDuration = parameters.showAnimation ? 200 : 0;
    this.options.xOffset = parameters.focusingX ? 0 : 0.5;
    this.options.yOffset = parameters.focusingY ? 0 : 0.5;
    this.options.touchMovingMinOffset = 15;
    this.options.containers = {};
    this.options.droppedContainers = [];
    this.options.designerIsFocused = true;
    this.options.fontSizes = this.GetFontSizes();
    this.options.monthesCollection = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    this.options.themeColors = { Blue: "#19478a", Carmine: "#912c2f", Green: "#0b6433", Orange: "#b73a1c", Purple: "#8653a5", Teal: "#23645c", Violet: "#6d3069" };
    this.options.propertyControlWidth = this.options.propertiesGridWidth - this.options.propertiesGridLabelWidth - 35;
    this.options.propertyNumbersControlWidth = Math.max(this.options.propertyControlWidth - 100, 40);
    this.options.showPanelPropertiesAndDictionary = this.options.showDictionary || this.options.showPropertiesGrid || this.options.showReportTree;
    if (!this.options.requestTimeout) this.options.requestTimeout = 20;
    if (this.options.fullScreenMode) this.options.mobileDesigner.style.zIndex = "10000";
    this.options.helpLanguage = this.IsRusCulture(this.options.cultureName) ? "ru" : "en";
    this.options.designerSpecification = "Developer";
    this.options.osWin11 = this.checkWin11();
    this.options.controlsHeight = this.options.isTouchDevice ? 28 : 23;
    this.options.controlsButtonsWidth = this.options.isTouchDevice ? 22 : 18;
    this.options.propertyControlsHeight = this.options.isTouchDevice ? 25 : 22;

    var setupToolboxCookie = this.GetCookie("StimulsoftMobileDesignerSetupToolbox");
    var setupToolbox = setupToolboxCookie ? JSON.parse(setupToolboxCookie) : null;
    this.options.showToolbox = setupToolbox ? setupToolbox.showToolbox : true;
    this.options.showInsertTab = setupToolbox ? setupToolbox.showInsertTab : true;
    this.options.publishUrl = "https://publish.stimulsoft.com/";
    this.options.formsDesignerUrl = "https://designer-forms.stimulsoft.com/";
    //this.options.formsDesignerUrl = "http://localhost:15665";

    this.options.newReportDictionary = this.options.newReportDictionary && this.options.newReportDictionary != "Auto" ? this.options.newReportDictionary : (this.GetCookie("StimulsoftMobileDesignerNewReportDictionary") || "DictionaryNew");
    this.options.chartEditorType = this.GetCookie("StimulsoftMobileDesignerChartEditorType") || "Simple";

    this.options.blocklyNotSupported = this.GetNavigatorName() == "MSIE" || (this.options.jsMode && !this.options.blocklyAssemblyLoaded);
    this.options.defaultScriptMode = this.options.blocklyNotSupported ? "Code" : (this.GetCookie("StimulsoftMobileDesignerDefaultScriptMode") || "Blocks");

    this.CheckOAuthParameters();
    this.RemoveCookie("StimulsoftMobileDesignerComponentsIntoInsertTab_NewVers"); //Temporarily, for clear old settings and show new components

    this.LoadThemeSettings();

    if (parameters.loc) {
        var loc = typeof parameters.loc == 'string' ? JSON.parse(parameters.loc) : parameters.loc;
        this.loc = loc.Localization || loc;
        delete this.options.loc;
    }

    this.options.dayOfWeekCollection = [
        this.loc.A_WebViewer.AbbreviatedDayMonday,
        this.loc.A_WebViewer.AbbreviatedDayTuesday,
        this.loc.A_WebViewer.AbbreviatedDayWednesday,
        this.loc.A_WebViewer.AbbreviatedDayThursday,
        this.loc.A_WebViewer.AbbreviatedDayFriday,
        this.loc.A_WebViewer.AbbreviatedDaySaturday,
        this.loc.A_WebViewer.AbbreviatedDaySunday
    ];

    var startDay = this.options.datePickerFirstDayOfWeek == "Auto" ? this.GetFirstDayOfWeek() : this.options.datePickerFirstDayOfWeek;
    if (startDay == "Sunday") {
        this.options.dayOfWeekCollection.splice(6, 1);
        this.options.dayOfWeekCollection.splice(0, 0, this.loc.A_WebViewer.AbbreviatedDaySunday);
    }

    if (!this.options.jsMode) {
        var processImage = this.options.processImage || this.InitializeProcessImage();
        processImage.show();

        // Load designer styles
        this.LoadStyle(this.options.stylesUrl);
    }

    if (this.options.serverMode && this.options.cloudParameters) {
        this.SetWindowIcon(this.options.cloudParameters.favIcon);
    }

    this.options.imagesScalingFactor = this.GetImagesScalingFactor();

    if (this.options.images) {
        this.InitializeDesignerControls();
    }
    else {
        //Get Images Collection
        this.SendCommandToDesignerServer("GetImagesArray", { imagesUrl: this.options.imagesUrl, imagesScalingFactor: this.options.imagesScalingFactor }, function (answer) {
            if (answer.images) jsObject.options.images = answer.images;
            jsObject.InitializeDesignerControls();
        });
    }
}

StiMobileDesigner.setImageSource = function (image, options, name, transform) {
    if (image != null) {
        if (image.tagName == "IMG")
            image.src = options.images[name];
        else if (image.tagName == "image")
            image.href.baseVal = options.images[name];
        else if (image.tagName == "DIV")
            image.style.backgroundImage = "url(" + options.images[name] + ")";
    }
}

StiMobileDesigner.checkImageSource = function (options, name) {
    return options.images[name] != null;
}

StiMobileDesigner.getImageSource = function (options, name) {
    return options.images[name];
}

StiMobileDesigner.prototype.InitializeDesignerControls = function (callback) {
    var jsObject = this;

    if (jsObject.options.plansLimits) {
        jsObject.options.plansLimits = JSON.parse(jsObject.options.plansLimits);
    }

    if ((jsObject.options.cloudMode || jsObject.options.serverMode) && this.options.cloudParameters) {
        if (jsObject.options.cloudParameters.isTouchDevice != null) {
            jsObject.options.isTouchDevice = jsObject.options.cloudParameters.isTouchDevice == "true";
        }

        var title = jsObject.loc.FormDesigner.title;
        if (jsObject.options.cloudParameters.reportName) title = jsObject.options.cloudParameters.reportName + " - " + title;
        jsObject.SetWindowTitle(title);

        if (jsObject.options.cloudMode) {
            jsObject.UpdateResourcesLimits();
        }

        var requestChangesCookie = jsObject.GetCookie("StimulsoftMobileDesignerRequestChangesWhenSaving");
        jsObject.options.requestChangesWhenSaving = requestChangesCookie == null || requestChangesCookie == "true";
    }

    jsObject.AddCustomOpenTypeFontsCss();

    // Data Tree
    jsObject.options.dataTree = jsObject.DataTree();
    jsObject.options.mobileDesigner.jsObject = jsObject;

    jsObject.CreateMetaTag();
    jsObject.InitializeDesigner();
    jsObject.InitializeToolBar();
    jsObject.InitializeWorkPanel();
    jsObject.InitializeHomePanel();
    jsObject.InitializeStatusPanel();
    jsObject.InitializeInfoPanel();
    jsObject.InitializePropertiesPanel();
    jsObject.InitializePagesPanel();
    jsObject.InitializePaintPanel();
    jsObject.InitializeToolbox();
    jsObject.InitializeToolTip();
    if (jsObject.options.jsMode) jsObject.InitializePreviewPanel();
    if (jsObject.options.cloudMode) jsObject.InitializeLoginControls();
    if (jsObject.options.showToolbar === false) jsObject.options.toolBar.changeVisibleState(false);
    jsObject.UpdateDesignerSpecification();
    jsObject.UpdateDesignerControlsBySpecification();

    jsObject.SetEnabledAllControls(false);

    jsObject.addEvent(document, 'mousemove', function (event) {
        jsObject.DocumentMouseMove(event);
    });

    jsObject.addEvent(document, 'touchmove', function (event) {
        jsObject.DocumentTouchMove(event);
    });

    jsObject.addEvent(document, 'touchend', function (event) {
        jsObject.isTouchEndFlag = true;
        clearTimeout(jsObject.isTouchEndTimer);

        jsObject.DocumentTouchEnd(event);

        jsObject.isTouchEndTimer = setTimeout(function () {
            jsObject.isTouchEndFlag = false;
        }, 1000);
    });

    jsObject.addEvent(document, 'mouseup', function (event) {
        if (jsObject.isTouchEndTimer) return;
        jsObject.DocumentMouseUp(event);
    });

    //Load Report
    if (jsObject.options.jsMode) {
        jsObject.CloseReport();

        //append stimulsoft font
        if (jsObject.options.stimulsoftFontContent) {
            jsObject.AddCustomFontsCss(jsObject.GetCustomFontsCssText(jsObject.options.stimulsoftFontContent, "Stimulsoft"));
        }

        if (jsObject.options.buttons.resizeDesigner && jsObject.options.maximizeAfterCreating) {
            jsObject.options.buttons.resizeDesigner.action();
        }

        if (jsObject.options.standaloneJsMode) {
            var designerLoading = document.getElementById("stiDesignerLoading");
            if (designerLoading) designerLoading.parentElement.removeChild(designerLoading);
        }

        jsObject.startDesignerTimer = setTimeout(function () {
            var startScreen = jsObject.options.startScreen;

            if (!startScreen || startScreen == "NotAssigned") {
                var designerOptionsJs = jsObject.GetCookie("StimulsoftMobileDesignerOptions");
                if (designerOptionsJs) startScreen = JSON.parse(StiBase64.decode(designerOptionsJs)).startScreen;
                if (startScreen) jsObject.options.startScreen = startScreen;
            }

            if (startScreen == "BlankReport") {
                jsObject.ActionNewReport();
            }
            else if (startScreen == "BlankDashboard" && jsObject.options.dashboardAssemblyLoaded) {
                jsObject.ActionNewDashboard();
            }
            else {
                var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
                fileMenu.changeVisibleState(true);
                fileMenu.items.newReport.action();
            }
        }, 500);
    }
    else {
        var processImage = this.options.processImage || this.InitializeProcessImage();
        processImage.show();

        if (document.readyState == 'complete') {
            jsObject.BuildDesignerComplete();
        }
        else {
            jsObject.addEvent(window, 'load', function () { jsObject.BuildDesignerComplete(); });
        }
    }

    if (jsObject.onready) jsObject.onready();

    if (this.onreadyasync) this.onreadyasync(callback);
    else if (callback) callback();
}

StiMobileDesigner.prototype.mergeOptions = function (fromObject, toObject) {
    for (var value in fromObject) {
        if (toObject[value] === undefined || toObject[value] == null || typeof toObject[value] !== "object" || (typeof Array == "function" && Array.isArray(toObject[value])))
            toObject[value] = fromObject[value];
        else
            this.mergeOptions(fromObject[value], toObject[value]);
    }
}

StiMobileDesigner.prototype.BuildDesignerComplete = function () {
    var jsObject = this;

    var params = {
        defaultUnit: this.options.defaultUnit,
        zoom: this.options.zoom,
        designerOptions: jsObject.GetCookie("StimulsoftMobileDesignerOptions")
    };

    if (this.options.serverMode) {
        params.sessionKey = this.options.cloudParameters.sessionKey;
        params.reportTemplateItemKey = this.options.cloudParameters.reportTemplateItemKey;
        params.attachedItems = this.options.cloudParameters.attachedItems || [];
        params.resourceItems = this.options.cloudParameters.resourceItems || [];
    }

    if ((this.options.serverMode || this.options.cloudMode) && this.options.cloudParameters && this.options.cloudParameters.startParameters) {
        params.startParameters = JSON.parse(StiBase64.decode(this.options.cloudParameters.startParameters));
    }

    if (params.startParameters && (params.startParameters.action || params.startParameters.wizard)) {
        if (params.startParameters.action) {
            this.ExecuteAction(params.startParameters.action);
        }
        else {
            jsObject.StartWizardForm2(params.startParameters.wizard, params.startParameters.template);
        }
    }
    else if (params.startParameters && params.startParameters.resourceName) {
        jsObject.SendCommandOpenWizardReport(params.startParameters.resourceName);
    }
    else {
        this.SendCommandToDesignerServer("GetReportForDesigner", params, function (answer) {
            if (answer.formContent) {
                jsObject.InitializeFormsDesignerFrame(function (frame) {
                    frame.openForm(jsObject.options.cloudParameters ? jsObject.options.cloudParameters.reportName : "Form", answer.formContent);
                });
            }
            else {
                if (answer.reportObject) {
                    jsObject.LoadReport(jsObject.ParseReport(answer.reportObject));

                    if (jsObject.options.setZoomToPageWidth) {
                        jsObject.SetZoomBy("Width");
                    }
                    else if (jsObject.options.setZoomToPageHeight) {
                        jsObject.SetZoomBy("Height");
                    }
                }

                if ((jsObject.options.serverMode || jsObject.options.cloudMode) && params.startParameters && params.startParameters.useDemoData) {
                    jsObject.autoCreateDataComponent();
                }

                if ((jsObject.options.cloudParameters && jsObject.options.cloudParameters.thenOpenWizard) ||
                    jsObject.options.runWizardAfterLoad ||
                    jsObject.options.runSpecificWizardAfterLoad ||
                    !answer.reportObject ||
                    (jsObject.options.wizardTypeRunningAfterLoad && jsObject.options.wizardTypeRunningAfterLoad != "None")) {
                    var wizardType = jsObject.options.runSpecificWizardAfterLoad || jsObject.options.wizardTypeRunningAfterLoad;
                    if (wizardType && wizardType != "None") {
                        jsObject.RunWizard(wizardType.toString());
                    }
                    else {
                        var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
                        fileMenu.changeVisibleState(true);
                    }
                }
            }

            // Update images array
            jsObject.PostAjax(jsObject.options.requestUrl, { command: "UpdateImagesArray", imagesScalingFactor: jsObject.options.imagesScalingFactor }, jsObject.receveFromServer);

            // Load all scripts
            jsObject.LoadScript(jsObject.options.scriptsUrl + "AllNotLoadedScripts");
        });
    }

    //append stimulsoft font
    if (this.options.stimulsoftFontContent) {
        this.AddCustomFontsCss(this.GetCustomFontsCssText(this.options.stimulsoftFontContent, "Stimulsoft"));
    }
}

StiMobileDesigner.prototype.LoadThemeSettings = function () {

}

StiMobileDesigner.prototype.isTouchEndFlag = null;
StiMobileDesigner.prototype.isTouchEndTimer = null;
StiMobileDesigner.prototype.startDesignerTimer = null;
