
StiJsViewer.prototype.InitializeInteractions = function (page) {
    var jsObject = this;

    page.getComponentOffset = function (component) {
        var offsetX = 0;
        var offsetY = 0;
        var startComponent = component;
        while (component && !isNaN(component.offsetLeft) && !isNaN(component.offsetTop)
            && (component == startComponent || component.style.position == "" || component.style.position == "static")) {
            offsetX += component.offsetLeft - component.scrollLeft;
            offsetY += component.offsetTop - component.scrollTop;
            component = component.offsetParent;
        }
        return { top: offsetY, left: offsetX };
    }

    page.paintSortingArrow = function (component, sort) {
        if (component.arrowImg) return;
        var zoom = jsObject.reportParams.zoom / 100;
        var arrowImg = document.createElement("div");
        var sortUpSrc = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 12 12' width='" + parseInt(zoom * 12) + "' height='" + parseInt(zoom * 12) + "'><path d='M1 9l5-4 5 4z' fill='#eeeeee' stroke='#666'/></svg>";
        var sortDownSrc = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 12 12' width='" + parseInt(zoom * 12) + "' height='" + parseInt(zoom * 12) + "'><path d='M1 5l5 4 5-4z' fill='#eeeeee' stroke='#666'/></svg>";
        arrowImg.innerHTML = sort == "asc" ? sortDownSrc : sortUpSrc;

        var arrowWidth = zoom * 16;
        var arrowHeight = zoom * 16;
        arrowImg.style.position = "absolute";
        arrowImg.style.width = arrowWidth + "px";
        arrowImg.style.height = arrowHeight + "px";
        component.appendChild(arrowImg);
        component.arrowImg = arrowImg;

        var oldPosition = component.style.position;
        var oldClassName = component.className;
        var reportDisplayMode = jsObject.options.displayModeFromReport || jsObject.options.appearance.reportDisplayMode;

        if (reportDisplayMode == "Table") component.style.position = "relative";
        if (!oldClassName) component.className = "stiSortingParentElement";

        var arrowLeftPos = jsObject.FindPosX(arrowImg, component.className);
        var arrowTopPos = jsObject.FindPosY(arrowImg, component.className);

        arrowImg.style.marginLeft = (component.offsetWidth - arrowLeftPos - arrowWidth - (zoom * 3)) + "px";
        arrowImg.style.marginTop = (component.offsetHeight / 2 - arrowHeight / 2 - arrowTopPos) + "px";

        if ((jsObject.getNavigatorName() == "Mozilla" || oldPosition) && reportDisplayMode == "Table") {
            component.style.position = oldPosition;
        }
        component.className = oldClassName;
    }

    page.paintCollapsingIcon = function (component, collapsed) {
        if (component.collapsImg) return;
        var collapsImg = document.createElement("img");
        StiJsViewer.setImageSource(collapsImg, jsObject.options, jsObject.collections, collapsed ? "CollapsingPlus.png" : "CollapsingMinus.png");
        collapsImg.style.position = "absolute";
        var collapsWidth = (jsObject.reportParams.zoom / 100) * 10;
        var collapsHeight = (jsObject.reportParams.zoom / 100) * 10;
        collapsImg.style.width = collapsWidth + "px";
        collapsImg.style.height = collapsHeight + "px";
        component.appendChild(collapsImg);
        component.collapsImg = collapsImg;
        var componentOffset = page.getComponentOffset(component);
        var collapsOffset = page.getComponentOffset(collapsImg);
        collapsImg.style.marginLeft = (componentOffset.left - collapsOffset.left + collapsWidth / 3) + "px";
        collapsImg.style.marginTop = (componentOffset.top - collapsOffset.top + collapsWidth / 3) + "px";
    }

    page.postInteractionSorting = function (component, isCtrl) {
        var params = {
            "action": "Sorting",
            "sortingParameters": {
                "ComponentName": component.getAttribute("interaction") + ";" + isCtrl.toString(),
                "DataBand": component.getAttribute("databandsort")
            }
        };

        if (jsObject.controls.parametersPanel) {
            params.variables = jsObject.controls.parametersPanel.getParametersValues();
        }

        if (jsObject.reportParams.collapsingStates) {
            params.collapsingParameters = {
                ComponentName: jsObject.reportParams.collapsingComponentName,
                CollapsingStates: jsObject.reportParams.collapsingStates
            }
        }

        jsObject.postInteraction(params);
    }

    page.postInteractionDrillDown = function (component) {
        var params = {
            "action": "DrillDown",
            "drillDownParameters": {
                "ComponentIndex": component.getAttribute("compindex"),
                "ElementIndex": component.getAttribute("elementindex"),
                "PageIndex": component.getAttribute("pageindex"),
                "PageGuid": component.getAttribute("pageguid"),
                "DrillDownMode": component.getAttribute("drilldownmode"),
                "ReportFile": component.getAttribute("reportfile")
            }
        };

        jsObject.postInteraction(params);
    }

    page.postInteractionCollapsing = function (component) {
        var componentName = component.getAttribute("interaction");
        var collapsingIndex = component.getAttribute("compindex");
        var collapsed = component.getAttribute("collapsed") == "true" ? false : true;

        if (!jsObject.reportParams.collapsingStates) jsObject.reportParams.collapsingStates = {};
        if (!jsObject.reportParams.collapsingStates[componentName]) jsObject.reportParams.collapsingStates[componentName] = {};
        jsObject.reportParams.collapsingStates[componentName][collapsingIndex] = collapsed;
        jsObject.reportParams.collapsingComponentName = componentName;

        var params = {
            "action": "Collapsing",
            "collapsingParameters": {
                "ComponentName": componentName,
                "CollapsingStates": jsObject.reportParams.collapsingStates
            }
        };

        if (jsObject.controls.parametersPanel) {
            params.variables = jsObject.controls.parametersPanel.getParametersValues();
        }

        jsObject.postInteraction(params);
    }

    page.postInteractionEvent = function (component) {
        var types = component.getAttribute("interactionevents").split(", ");
        for (var k = 0; k < types.length; k++) {
            var type = types[k];
            component["override" + type] = component[type];
            component[type] = function (e) {
                var type = "on" + e.type;
                var params = {
                    "PageIndex": this.getAttribute("pageindex"),
                    "Type": type,
                    "DomComponent": component
                };

                if (this.getAttribute("compindex"))
                    params["ComponentIndex"] = this.getAttribute("compindex");

                if (jsObject.viewer.invokeComponentsEvents) jsObject.viewer.invokeComponentsEvents(params);
                if (this["override" + type]) this["override" + type](e);
            }
        }
    }

    //fix a bug with browser zoom
    if (this.reportParams.viewMode == "SinglePage") {
        page.style.margin = "10px auto 10px auto";
        page.style.display = page.innerHTML != "" ? "table" : "none";
    }

    var elems = page.querySelectorAll ? page.querySelectorAll("td,div,span,rect,path,ellipse") : page.getElementsByTagName("td");
    var collapsedHash = [];
    for (var i = 0; i < elems.length; i++) {
        if (elems[i].getAttribute("interaction") && (
            elems[i].getAttribute("pageguid") ||
            elems[i].getAttribute("reportfile") ||
            elems[i].getAttribute("collapsed") ||
            elems[i].getAttribute("databandsort"))) {

            elems[i].style.cursor = "pointer";
            elems[i].jsObject = this;

            var sort = elems[i].getAttribute("sort");
            if (sort) {
                page.paintSortingArrow(elems[i], sort);
            }

            var collapsed = elems[i].getAttribute("collapsed");
            if (collapsed) {
                var compId = elems[i].getAttribute("compindex") + "|" + elems[i].getAttribute("interaction");
                if (collapsedHash.indexOf(compId) < 0) {
                    page.paintCollapsingIcon(elems[i], collapsed == "true");
                    collapsedHash.push(compId);
                }
            }

            elems[i].onclick = function (e) {
                if (jsObject.options.interactionInProgress) return;
                if (this.getAttribute("pageguid") || this.getAttribute("reportfile")) page.postInteractionDrillDown(this);
                else if (this.getAttribute("collapsed")) page.postInteractionCollapsing(this);
                else page.postInteractionSorting(this, e.ctrlKey);
                jsObject.options.interactionInProgress = true;
            }

            if (elems[i].getAttribute("pageguid") || elems[i].getAttribute("reportfile")) {
                elems[i].onmouseover = function (e) { this.style.opacity = 0.75; }
                elems[i].onmouseout = function (e) { this.style.opacity = 1; }
            }
        }

        if (jsObject.options.jsMode && elems[i].getAttribute("interactionevents")) {
            page.postInteractionEvent(elems[i]);
        }

        if (elems[i].getAttribute("interactionhyperlink")) {
            elems[i].style.cursor = "pointer";

            elems[i].onclick = function (e) {
                var link = this.getAttribute("interactionhyperlink");

                if (jsObject.options.appearance.openLinksWindow == "_self")
                    window.location.href = link;
                else
                    jsObject.openNewWindow(link, jsObject.options.appearance.openLinksWindow, undefined, false);
            }
        }
    }

    if (jsObject.options.jsMode && page.getAttribute("interactionevents")) {
        page.postInteractionEvent(page);
    }
}