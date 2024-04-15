var Stimulsoft = Stimulsoft || {};
Stimulsoft.Designer = Stimulsoft.Designer || {};

Stimulsoft.Designer.StiDesignerOptions = function () {
    this.serializeObject(Stimulsoft.Designer.defaultOptions, this, null);

    this.width = this.normalizeSize(this.width);
    this.height = this.normalizeSize(this.height);
}

Stimulsoft.Designer.StiDesignerOptions.prototype.normalizeSize = function (value) {
    if (value && value.isEmpty || value == "")
        return "0px";

    if (value && value.value)
        return value.value + (value.type == "Percentage" ? "%" : "px");

    return parseInt(value) + (value.indexOf("%") > 0 ? "%" : "px");
}

Stimulsoft.Designer.StiDesignerOptions.prototype.toParameters = function () {
    var parameters = {};
    this.serializeObject(this, parameters, "options");
    this.copyObject(Stimulsoft.Designer.parameters, parameters);
    delete parameters.width;
    delete parameters.height;

    return parameters;
}

Stimulsoft.Designer.StiDesignerOptions.prototype.serializeObject = function (fromObject, toObject, parentName) {
    for (var value in fromObject) {
        if (typeof fromObject[value] == "function") continue;

        var toValue = value;
        if (typeof fromObject[value] == "object") {
            var toObjectChild = toObject;
            if (parentName == null) {
                toObject[value] = {};
                toObjectChild = toObject[value];
            }
            else if (value == "actions" || value == "bands" || value == "crossBands" || value == "components" || value == "dashboardElements") {
                if (value != "actions")
                    toValue = "visibility" + value[0].toUpperCase() + value.slice(1);

                toObject[toValue] = {};
                toObjectChild = toObject[toValue];
            }

            this.serializeObject(fromObject[value], toObjectChild, parentName == null ? null : value);
        }
        else {
            if (parentName == "bands" || parentName == "crossBands" || parentName == "components" || parentName == "dashboardElements")
                toValue = value.replace("show", "Sti");

            else if (parentName == "fileMenu")
                toValue = value.replace(/show|visible/, "showFileMenu");

            else if (parentName == "propertiesGrid" && value == "visible")
                toValue = "showPropertiesGrid";

            else if (parentName == "propertiesGrid")
                toValue = "propertiesGrid" + value[0].toUpperCase() + value.slice(1);

            else if (parentName == "dictionary" && value == "visible")
                toValue = "showDictionary";

            toObject[toValue] = fromObject[value];
        }
    }
}

Stimulsoft.Designer.StiDesignerOptions.prototype.copyObject = function (fromObject, toObject) {
    for (var value in fromObject) {
        toObject[value] = fromObject[value];
    }
}

Stimulsoft.Designer.StiDesigner = function (options, designerId, renderAfterCreate) {
    this.options = options;
    this.designerId = designerId || "StiDesigner";

    if (renderAfterCreate) this.renderHtml();
}

Stimulsoft.Designer.StiDesigner.prototype.newToken = function () {
    var a = "1234567890abcdefghijklmnopqrstuvwxyz".split("");
    var b = [];
    var length = 32;
    for (var i = 0; i < length; i++) {
        var j = (Math.random() * (a.length - 1)).toFixed(0);
        b[i] = a[j];
    }
    return b.join("");
}

Stimulsoft.Designer.StiDesigner.prototype.getParameters = function () {
    var jsParameters = this.options.toParameters();
    jsParameters.mobileDesignerId = this.designerId;
    jsParameters.viewerId = this.designerId + "Viewer";
    jsParameters.cloudMode = false;
    jsParameters.clientGuid = this.newToken();
    jsParameters.reportGuid = jsParameters.clientGuid;
    jsParameters.fullScreenMode = this.options.width == "0px" && this.options.height == "0px";
    jsParameters.zoom = this.options.zoom / 100;
    jsParameters.actions.designerEvent = "DesignerEvent";

    return jsParameters;
}

Stimulsoft.Designer.StiDesigner.prototype.renderHtml = function (element) {
    var designer = document.createElement("div");
    designer.id = this.designerId;

    // Render preview control
    var viewerOptions = new Stimulsoft.Viewer.StiViewerOptions();
    var viewer = new Stimulsoft.Viewer.StiViewer(viewerOptions, this.designerId + "Viewer", false);
    viewer.reportDesignerMode = true;
    viewer.renderHtml(designer);

    var mainPanel = document.createElement("div");
    mainPanel.className = "stiDesignerMainPanel";
    mainPanel.id = this.designerId + "_MainPanel";
    designer.appendChild(mainPanel);

    if (this.options.width == "0px" && this.options.height == "0px") {
        designer.style.width = "100%";
        designer.style.position = "fixed";
        designer.style.zIndex = "1000000";
        designer.style.top = "0px";
        designer.style.left = "0px";
        designer.style.right = "0px";
        designer.style.bottom = "0px";
    }
    else {
        designer.style.width = this.options.width;
        designer.style.height = this.options.height;
    }

    var html = designer.outerHTML;

    if (element && typeof element == "string")
        element = document.getElementById(element);

    if (element && element["innerHTML"] !== undefined) {
        element["innerHTML"] = html;
    }
    else {
        document.write(html);
    }

    var jsParameters = this.getParameters();
    var jsDesigner = new StiMobileDesigner(jsParameters);
}
