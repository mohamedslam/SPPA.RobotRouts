function StiJsViewer(parameters, collections) {
    var jsObject = this;
    this.defaultParameters = {};
    this.options = parameters.options;

    // Options
    this.options.isTouchDevice = this.options.appearance.interfaceType == "Auto"
        ? this.IsTouchDevice() && this.IsMobileDevice()
        : (this.options.appearance.interfaceType == "Touch" || this.options.appearance.interfaceType == "Mobile");

    this.options.isMobileDevice = this.options.appearance.allowMobileMode === false
        ? false
        : (this.options.appearance.interfaceType == "Auto" && this.options.reportDesignerMode == false
            ? this.IsTouchDevice() && this.IsMobileDevice()
            : this.options.appearance.interfaceType == "Mobile");

    this.options.menuAnimDuration = 150;
    this.options.formAnimDuration = 200;
    this.options.scrollDuration = 350;
    this.options.menuHideDelay = 250;
    this.options.server.timeoutAutoUpdateCache = 180000;
    this.options.toolbar.backgroundColor = this.getHTMLColor(this.options.toolbar.backgroundColor);
    this.options.toolbar.borderColor = this.getHTMLColor(this.options.toolbar.borderColor);
    this.options.toolbar.fontColor = this.getHTMLColor(this.options.toolbar.fontColor);
    this.options.appearance.pageBorderColor = this.getHTMLColor(this.options.appearance.pageBorderColor);
    this.options.exports.defaultSettings = parameters.defaultExportSettings;
    this.options.parametersValues = {};
    this.options.parameterRowHeight = 35;
    this.options.minParametersCountForMultiColumns = 5;
    this.options.osWin11 = this.checkWin11();

    // Collections
    this.collections = {};
    if (collections) this.collections = collections;
    if (parameters.loc && this.collections.loc) this.collections.loc = parameters.loc;

    //Days Of Week
    this.collections.dayOfWeek = [
        this.collections.loc.AbbreviatedDayMonday,
        this.collections.loc.AbbreviatedDayTuesday,
        this.collections.loc.AbbreviatedDayWednesday,
        this.collections.loc.AbbreviatedDayThursday,
        this.collections.loc.AbbreviatedDayFriday,
        this.collections.loc.AbbreviatedDaySaturday,
        this.collections.loc.AbbreviatedDaySunday
    ];

    //First Day Of Week
    var startDay = this.options.appearance.datePickerFirstDayOfWeek == "Auto" ? this.GetFirstDayOfWeek() : this.options.appearance.datePickerFirstDayOfWeek;
    if (startDay == "Sunday") {
        this.collections.dayOfWeek.splice(6, 1);
        this.collections.dayOfWeek.splice(0, 0, this.collections.loc.AbbreviatedDaySunday);
    }

    // Controls
    this.controls = {};
    this.controls.forms = {};
    this.controls.head = document.getElementsByTagName("head")[0];
    this.controls.viewer = document.getElementById(this.options.viewerId);
    this.controls.mainPanel = document.getElementById(this.options.viewerId + "_JsViewerMainPanel");
    this.controls.findHelper = { findLabels: [] };

    // Parameters of the current report
    this.reportParams = {
        type: "Auto",
        pageNumber: 0,
        pagesCount: 0,
        zoom: this.options.toolbar.zoom,
        viewMode: this.options.toolbar.viewMode,
        reportFileName: null,
        pagesArray: [],
        collapsingStates: null,
        bookmarksContent: null,
        editableParameters: null,
        drillDownGuid: null,
        dashboardDrillDownGuid: null,
        drillDownParameters: []
    };

    // Service objects and states
    this.service = {};
    this.service.resizeTimer = null;
    this.service.refreshReportTimer = null;
    this.service.isRequestInProcess = false;
    this.service.elementRefreshTimers = {};

    // Actions
    if (!this.options.actions.getReport) this.options.actions.getReport = this.options.actions.viewerEvent;
    if (!this.options.actions.printReport) this.options.actions.printReport = this.options.actions.viewerEvent;
    if (!this.options.actions.openReport) this.options.actions.openReport = this.options.actions.viewerEvent;
    if (!this.options.actions.exportReport) this.options.actions.exportReport = this.options.actions.viewerEvent;
    if (!this.options.actions.interaction) this.options.actions.interaction = this.options.actions.viewerEvent;

    if (!(window.File && window.FileReader && window.FileList && window.Blob)) this.options.toolbar.showOpenButton = false;

    // Render JsViewer styles into HEAD
    if (this.options.requestResourcesUrl || this.options.appearance.customStylesUrl) {
        this.LoadStyle(this.options.appearance.customStylesUrl || this.GetResourceUrl("styles"));
    }

    // Append stimulsoft font
    if (this.options.stimulsoftFontContent) {
        this.addCustomFontStyles([{
            contentForCss: this.options.stimulsoftFontContent,
            originalFontFamily: "Stimulsoft"
        }]);
    }

    // Append custom open type fonts
    if (this.options.customOpenTypeFonts && !jsObject.options.reportDesignerMode) {
        this.addCustomFontStyles(this.options.customOpenTypeFonts);
    }

    this.options.imagesScalingFactor = this.getImagesScalingFactor();

    //Initialize Viewer Controls
    if (this.collections.images) {
        this.InitializeViewerControls();
    }
    else {
        //Get Images Collection
        var params = {
            method: "GET",
            imagesScalingFactor: this.options.imagesScalingFactor,
            useCompression: this.options.server.useCompression
        }

        //Only for Cloud & Server Sharing 
        if (this.options.viewerId == "StiCloudShareViewer" || this.options.viewerId == "StiCloudReportsShareViewer") {
            params.sharingLocalization = this.GetCookie("sti_CloudLocalization") || this.getDefaultLocalization();
        }

        var url = jsObject.GetResourceUrl("images");
        for (var key in params) {
            if (key != "method")
                url += "&stiweb_" + key.toLowerCase() + "=" + params[key];
        }

        this.postAjax(url, params, function (data) {
            if (data) {
                if (jsObject.options.server.useCompression) data = StiGZipHelper.unpack(data);
                data = JSON.parse(data);
                if (data.images) jsObject.collections.images = data.images;
                if (data.localizationItems) jsObject.collections.loc = data.localizationItems;
            }

            jsObject.InitializeViewerControls(function () {
                if (!jsObject.options.reportDesignerMode) {
                    if (document.readyState == 'complete') jsObject.postAction();
                    else jsObject.addEvent(window, 'load', function () { jsObject.postAction(); });
                }
            });
        });
    }
}

StiJsViewer.setImageSource = function (image, options, collections, name, transform) {
    if (image.tagName == "IMG")
        image.src = collections.images[name];
    else if (image.tagName == "image")
        image.href.baseVal = collections.images[name];
    else if (image.tagName == "DIV")
        image.style.backgroundImage = "url(" + collections.images[name] + ")";
    else
        throw "";
}

StiJsViewer.checkImageSource = function (options, collections, name) {
    return collections.images[name] != null;
}

StiJsViewer.getImageSource = function (options, collections, name) {
    return collections.images[name];
}

StiJsViewer.prototype.LoadStyle = function (src) {
    var link = document.createElement("link");
    link.setAttribute("rel", "stylesheet");
    link.setAttribute("type", "text/css");
    link.setAttribute("href", src);
    link.setAttribute("stimulsoft", "stimulsoft");
    this.controls.head.appendChild(link);
}

StiJsViewer.prototype.GetResourceUrl = function (resourceParameter) {
    var url = this.getActionRequestUrl(this.options.requestResourcesUrl, this.options.actions.viewerEvent);
    url += url.indexOf("?") > 0 ? "&" : "?";
    url += "stiweb_component=Viewer&stiweb_action=Resource&stiweb_data=" + resourceParameter + "&stiweb_theme=" + this.options.theme;
    url += "&stiweb_cachemode=" + (this.options.server.useCacheForResources
        ? this.options.server.cacheMode == "ObjectSession" || this.options.server.cacheMode == "StringSession"
            ? "session"
            : "cache"
        : "none");
    url += "&stiweb_version=" + this.options.shortProductVersion;

    return url;
}

StiJsViewer.prototype.InitializeViewerControls = function (callback) {
    var jsObject = this;

    if (this.options.isMobileDevice) this.InitializeMobile();
    else this.options.toolbar.showPinToolbarButton = false;

    this.InitializeJsViewer();
    this.InitializeDashboardsPanel();
    this.InitializeToolBar();
    if (this.options.toolbar.showFindButton) this.InitializeFindPanel();
    this.InitializeDrillDownPanel();
    if (this.options.toolbar.showResourcesButton) this.InitializeResourcesPanel();
    this.InitializeDisabledPanels();
    this.InitializeAboutPanel();
    this.InitializeReportPanel();
    this.InitializeProcessImage();
    this.InitializeDatePicker();
    this.InitializeToolTip();

    if (this.options.toolbar.displayMode == "Separated" && this.options.toolbar.visible) this.InitializeNavigatePanel();
    if (this.options.toolbar.showSaveButton && this.options.toolbar.visible) this.InitializeSaveMenu();
    if (this.options.toolbar.showSendEmailButton && this.options.toolbar.visible) this.InitializeSendEmailMenu();
    if (this.options.toolbar.showPrintButton && this.options.toolbar.visible) this.InitializePrintMenu();
    if (this.options.toolbar.showZoomButton && (this.options.toolbar.visible || this.options.toolbar.displayMode == "Separated")) this.InitializeZoomMenu();
    if (this.options.toolbar.showViewModeButton && this.options.toolbar.visible) this.InitializeViewModeMenu();
    if (this.options.exports.showExportDialog || this.options.email.showExportDialog) this.InitializeExportForm();
    if (this.options.toolbar.showSendEmailButton && this.options.email.showEmailDialog && this.options.toolbar.visible) this.InitializeSendEmailForm();

    this.addHoverEventsToMenus();
    this.checkTrExp();
    this.InitializeEvents();

    if (this.options.serverMode) this.InitializeFolderReportsPanel();

    this.addEvent(document, 'mouseup', function (event) {
        jsObject.DocumentMouseUp(event);
    });

    this.addEvent(document, 'mousemove', function (event) {
        jsObject.DocumentMouseMove(event);
    });

    if (document.all && !document.querySelector) {
        alert("Your web browser is not supported by our application. Please upgrade your browser!");
    }

    this.controls.viewer.style.top = 0;
    this.controls.viewer.style.right = 0;
    this.controls.viewer.style.bottom = 0;
    this.controls.viewer.style.left = 0;
    this.changeFullScreenMode(this.options.appearance.fullScreenMode);

    if (this.onready) this.onready();

    if (this.onreadyasync) this.onreadyasync(callback);
    else if (callback) callback();
}

StiJsViewer.prototype.InitializeMobile = function () {
    var isViewPortExist = false;
    var metas = this.controls.head.getElementsByTagName("meta");
    for (var i = 0; i < metas.length; i++) {
        if (metas[i].name && metas[i].name.toLowerCase() == "viewport") {
            isViewPortExist = true;
            break;
        }
    }

    if (!isViewPortExist) {
        var viewPortTag = document.createElement("meta");
        viewPortTag.id = "viewport";
        viewPortTag.name = "viewport";
        viewPortTag.content = "initial-scale=1.0,width=device-width,user-scalable=0";
        viewPortTag.setAttribute("stimulsoft", "stimulsoft");
        this.controls.head.appendChild(viewPortTag);
    }

    this.options.appearance.fullScreenMode = true;
    this.options.appearance.scrollbarsMode = true;
    this.options.appearance.parametersPanelPosition = "Left";
    this.options.appearance.parametersPanelColumnsCount = 1;
    this.options.toolbar.displayMode = "Separated";
    this.options.toolbar.viewMode = "SinglePage";
    this.options.toolbar.showZoomButton = false;
    var defaultZoom = this.options.toolbar.zoom == -2 ? -2 : -1; // PageWidth or PageHeight
    this.options.toolbar.zoom = this.reportParams.zoom = defaultZoom;
    this.options.toolbar.showButtonCaptions = false;
    this.options.toolbar.showOpenButton = false;
    this.options.toolbar.showFindButton = false;
    this.options.toolbar.showFullScreenButton = false;
    this.options.toolbar.showAboutButton = false;
    this.options.toolbar.showViewModeButton = false;

    this.InitializeCenterText();
}

StiJsViewer.prototype.mergeOptions = function (fromObject, toObject) {
    for (var value in fromObject) {
        if (toObject[value] === undefined || typeof toObject[value] !== "object") toObject[value] = fromObject[value];
        else this.mergeOptions(fromObject[value], toObject[value]);
    }
}

StiJsViewer.prototype.clearViewerState = function (clearParameters) {
    // Clear report state
    this.reportParams.type = "Auto";
    this.reportParams.pageNumber = 0;
    this.reportParams.originalPageNumber = 0;
    this.reportParams.drillDownGuid = null;
    this.reportParams.dashboardDrillDownGuid = null;
    this.reportParams.collapsingStates = null;
    this.reportParams.bookmarksContent = null;
    this.reportParams.editableParameters = null;
    this.reportParams.resources = null;
    this.reportParams.drillDownParameters = [];
    this.reportParams.elementName = null;
    this.reportParams.variablesValues = null;
    this.reportParams.tableOfContentsPointers = [];
    this.options.paramsVariables = null;
    this.options.multiFilterStates = null;
    this.options.tablesColumnsOrder = {};
    this.options.isParametersReceived = false;
    this.options.drillDownInProgress = false;
    this.controls.mainPanel.style.background = "";
    this.tableElementGridStates = {};
    this.tableElementHiddenColumns = {};
    this.framesCollection = [];
    this.options.paramsVariablesStartValues = null;
    this.options.isAutoHeight = null;
    this.options.isFullScreenHeight = null;
    this.options.displayModeFromReport = null;
    this.options.currentParameterWidth = null;
    this.options.previewSettingsRepToolbarAlign = null;
    this.options.previewSettingsRepToolbarReverse = null;
    this.options.previewSettingsDbsToolbarAlign = null;
    this.options.previewSettingsDbsToolbarReverse = null;

    if (this.options.toolBarRebuilded) {
        this.InitializeToolBar();
        this.options.toolBarRebuilded = false;
    }

    if (this.options.dashboardsPanelRebuilded) {
        this.InitializeDashboardsPanel();
        this.options.dashboardsPanelRebuilded = false;
    }

    // Restore current page number, if reload current report
    if (this.reportParams.prevPageNumber) {
        this.reportParams.pageNumber = this.reportParams.prevPageNumber;
        delete this.reportParams.prevPageNumber;
    }

    // Hide panels
    this.InitializeBookmarksPanel();
    this.InitializeParametersPanel();

    var dashboardsPanel = this.controls.dashboardsPanel;
    if (dashboardsPanel) {
        dashboardsPanel.changeVisibleState(false);
        dashboardsPanel.dashboardsCount = 0;
        dashboardsPanel.reportsCount = 0;
        dashboardsPanel.clear();
    }

    if (this.controls.drillDownPanel) this.controls.drillDownPanel.reset();
    if (this.controls.findPanel) this.controls.findPanel.changeVisibleState(false);

    // Clear parameters state
    if (clearParameters) {
        this.options.isParametersReceived = false;
        this.options.isReportRecieved = false;
    }

    if (this.options.currentMenu) this.options.currentMenu.changeVisibleState(false);
    if (this.options.currentDatePicker) this.options.currentDatePicker.changeVisibleState(false);
    if (this.options.currentForm && this.options.currentForm.visible) this.options.currentForm.changeVisibleState(false);

    for (var elementName in this.service.elementRefreshTimers) {
        clearInterval(this.service.elementRefreshTimers[elementName]);
    }
}
