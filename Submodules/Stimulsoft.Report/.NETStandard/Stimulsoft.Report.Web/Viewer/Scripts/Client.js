var Stimulsoft = Stimulsoft || {};
Stimulsoft.Viewer = Stimulsoft.Viewer || {};

Stimulsoft.Viewer.StiViewerOptions = function () {
    this.serializeObject(Stimulsoft.Viewer.defaultOptions, this, null);

    this.width = this.normalizeSize(this.width);
    this.height = this.normalizeSize(this.height);

    if (this.height == "0px")
        this.height = this.appearance.scrollbarsMode ? "650px" : "100%";
}

Stimulsoft.Viewer.StiViewerOptions.prototype.normalizeSize = function (value) {
    if (value && value.isEmpty || value == "")
        return "0px";

    if (value && value.value)
        return value.value + (value.type == "Percentage" ? "%" : "px");

    return parseInt(value) + (value.indexOf("%") > 0 ? "%" : "px");
}

Stimulsoft.Viewer.StiViewerOptions.prototype.toParameters = function () {
    var parameters = {};
    this.serializeObject(this, parameters, "options");
    delete parameters.width;
    delete parameters.height;

    return { options: parameters };
}

Stimulsoft.Viewer.StiViewerOptions.prototype.serializeObject = function (fromObject, toObject, parentName) {
    for (var value in fromObject) {
        if (typeof fromObject[value] == "function") continue;
        if (typeof fromObject[value] == "object") {
            toObject[value] = {};
            this.serializeObject(fromObject[value], toObject[value], value);
        }
        else {
            toObject[value] = fromObject[value];
        }
    }
}

Stimulsoft.Viewer.StiViewer = function (options, viewerId, renderAfterCreate) {
    this.options = options;
    this.viewerId = viewerId || "StiViewer";
    this.reportDesignerMode = false;

    if (renderAfterCreate) this.renderHtml();
}

Stimulsoft.Viewer.StiViewer.prototype.newToken = function () {
    var a = "1234567890abcdefghijklmnopqrstuvwxyz".split("");
    var b = [];
    var length = 32;
    for (var i = 0; i < length; i++) {
        var j = (Math.random() * (a.length - 1)).toFixed(0);
        b[i] = a[j];
    }
    return b.join("");
}

Stimulsoft.Viewer.StiViewer.prototype.getParameters = function () {
    var jsParameters = this.options.toParameters();
    jsParameters.options.viewerId = this.viewerId;
    jsParameters.options.clientGuid = this.newToken();
    jsParameters.options.requestUrl = Stimulsoft.Viewer.parameters.requestUrl;
    jsParameters.options.requestStylesUrl = Stimulsoft.Viewer.parameters.requestUrl; //obsolete
    jsParameters.options.requestResourcesUrl = Stimulsoft.Viewer.parameters.requestUrl;
    jsParameters.options.requestAbsoluteUrl = Stimulsoft.Viewer.parameters.requestAbsoluteUrl;
    jsParameters.options.heightType = this.options.height.indexOf("%") > 0 ? "Percentage" : "Pixel";
    jsParameters.options.shortProductVersion = Stimulsoft.Viewer.parameters.shortProductVersion;
    jsParameters.options.productVersion = Stimulsoft.Viewer.parameters.productVersion;
    jsParameters.options.frameworkType = Stimulsoft.Viewer.parameters.frameworkType;
    jsParameters.options.actions.viewerEvent = "ViewerEvent";
    jsParameters.options.reportDesignerMode = this.reportDesignerMode;
    jsParameters.options.isAngular = Stimulsoft.Viewer.parameters.isAngular;
    jsParameters.defaultExportSettings = Stimulsoft.Viewer.defaultExportSettings;

    if (!this.reportDesignerMode)
        jsParameters.options.stimulsoftFontContent = this.options.stimulsoftFontContent;

    return jsParameters;
}

Stimulsoft.Viewer.StiViewer.prototype.renderHtml = function (element) {
    var mainPanel = document.createElement("div");
    mainPanel.className = "stiJsViewerMainPanel";
    mainPanel.id = this.viewerId + "_JsViewerMainPanel";

    var viewer = document.createElement("div");
    viewer.id = this.viewerId;
    viewer.style.width = this.options.width;
    viewer.style.height = this.options.height;
    viewer.style.backgroundColor = this.options.appearance.backgroundColor.split(",").length == 3
        ? "RGB(" + this.options.appearance.backgroundColor + ")"
        : this.options.appearance.backgroundColor;
    if (this.reportDesignerMode)
        viewer.style.display = "none";
    viewer.appendChild(mainPanel);

    var html = viewer.outerHTML;

    if (element && typeof element == "string")
        element = document.getElementById(element);

    if (element && element["innerHTML"] !== undefined) {
        element["innerHTML"] = html;
    }
    else {
        document.write(html);
    }

    var jsParameters = this.getParameters();

    if (this.reportDesignerMode) {
        window["js" + this.viewerId + "Parameters"] = jsParameters;
    }
    else {
        window["js" + this.viewerId] = new StiJsViewer(jsParameters);
    }
}
