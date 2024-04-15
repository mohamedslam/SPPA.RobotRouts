
StiJsViewer.prototype.InitializeReportPanel = function () {
    var reportPanel = document.createElement("div");
    reportPanel.id = this.controls.viewer.id + "ReportPanel";
    var jsObject = reportPanel.jsObject = this;
    this.controls.reportPanel = reportPanel;
    this.controls.mainPanel.appendChild(reportPanel);
    reportPanel.style.textAlign = this.options.appearance.pageAlignment == "default" ? "center" : this.options.appearance.pageAlignment;
    reportPanel.className = "stiJsViewerReportPanel";
    reportPanel.style.top = "0";
    reportPanel.style.bottom = "0";
    reportPanel.pages = [];
    reportPanel.touchesLength = 0;
    if (this.options.isMobileDevice) reportPanel.style.transition = "margin 200ms ease";

    reportPanel.addDashboard = function (pageAttributes) {
        var page = document.createElement("div");
        page.jsObject = jsObject;
        reportPanel.appendChild(page);
        reportPanel.scrollTop = 0;

        page.className = "stiJsViewerPage";
        page.style.width = pageAttributes.width + "px";
        page.style.height = pageAttributes.height + "px";
        page.style.overflow = "hidden";
        page.style.color = "Black";
        page.style.background = pageAttributes.background == "Transparent" || pageAttributes.background == "rgba(255,255,255,0)" ? "White" : pageAttributes.background;

        var hasReports = jsObject.controls.dashboardsPanel.reportsCount > 0;

        if (!hasReports) jsObject.controls.mainPanel.style.background = page.style.background;
        jsObject.controls.dashboardsPanel.updateButtonsStyles(pageAttributes.actionColors, hasReports);

        if (pageAttributes.dashboardWatermark) {
            jsObject.AddWatermarkToPanel(page, pageAttributes.dashboardWatermark);
        }

        if (!jsObject.controls.dashboardsPanel.selectedButton.isValid) {
            var backPanel = document.createElement("div");
            backPanel.className = "stiJsViewerDashboardBackPanel";
            backPanel.innerHTML = "&#x54;r&#105;al";
            page.appendChild(backPanel);
        }

        for (var i = 0; i < pageAttributes.elements.length; i++) {
            var elementAttributes = pageAttributes.elements[i];
            jsObject.AddDashboardElementToPage(page, elementAttributes);
        }

        return page;
    }

    reportPanel.repaintDashboardContent = function (parameters) {
        var pagesArray = jsObject.reportParams.pagesArray;
        if (!pagesArray || !parameters) return;
        var currentElementName = parameters.currentElementName;
        var currentElementGroup = parameters.currentElementGroup;

        if (parameters.action == "DashboardFiltering" && parameters.filterGuid != jsObject.filterGuid) return;

        for (var num = 0; num <= pagesArray.length - 3; num++) {
            var pageAttributes = jsObject.reportParams.pagesArray[num];
            for (var i = 0; i < pageAttributes.elements.length; i++) {
                var elementAttrs = pageAttributes.elements[i];
                var element = reportPanel.getDashboardElementByName(elementAttrs.name);

                if (element) {
                    element.elementAttributes = elementAttrs;
                    jsObject.UpdateButtonsPanel(element);
                    jsObject.UpdateFiltersStringPanel(element);

                    if ((parameters.action == "DashboardFiltering" && jsObject.isFilterElement(elementAttrs.type) && (currentElementName == elementAttrs.name || !elementAttrs.parentKey)) ||
                        (parameters.action == "DashboardFiltering" && (elementAttrs.type == "StiChartElement" || elementAttrs.type == "StiRegionMapElement") && currentElementName == elementAttrs.name) ||
                        ((parameters.action == "DashboardFiltering") && (elementAttrs.group || currentElementGroup) && currentElementGroup != elementAttrs.group) ||
                        ((parameters.action == "DashboardSorting" || parameters.action == "DashboardElementDrillDown" || parameters.action == "DashboardElementDrillUp") && currentElementName != elementAttrs.name)) {
                        continue;
                    }

                    jsObject.InsertContentToDashboardElement(element);
                }
            }
        }
    }

    reportPanel.getDashboardElementByName = function (elementName) {
        for (var i = 0; i < reportPanel.childNodes.length; i++) {
            var dashboard = reportPanel.childNodes[i];
            for (var k = 0; k < dashboard.childNodes.length; k++) {
                if (dashboard.childNodes[k].elementAttributes && dashboard.childNodes[k].elementAttributes.name == elementName)
                    return dashboard.childNodes[k];
            }
        }
        return null;
    }

    reportPanel.getPagesSize = function () {
        var size = { width: 0, height: 0 };
        if (this.pages && this.pages.length > 0) {
            for (var i = 0; i < this.pages.length; i++) {
                size.width = Math.max(size.width, (this.pages[i].offsetWidth || this.pages[i].pageWidth));
                size.height = Math.max(size.height, (this.pages[i].offsetHeight || this.pages[i].pageHeight));
            }
        }
        return size;
    }

    reportPanel.addPage = function (pageAttributes) {
        if (!pageAttributes) return null;

        var page = document.createElement("div");
        page.jsObject = jsObject;
        reportPanel.appendChild(page);
        reportPanel.pages.push(page);

        page.loadContent = function (pageContent) {
            page.style.display = "inline-block";
            var pageAttributes = pageContent[0];
            page.style.background = pageAttributes.background == "Transparent" || pageAttributes.background == "rgba(255,255,255,0)" ? "White" : pageAttributes.background;
            page.innerHTML = pageAttributes.content;
        }

        page.className = jsObject.options.appearance.showPageShadow ? "stiJsViewerPageShadow" : "stiJsViewerPage";

        var margins = [0, 0, 0, 0];
        if (pageAttributes.margins) {
            var marginsPx = pageAttributes.margins.split(" ");
            for (var i = 0; i < marginsPx.length; i++) {
                margins[i] = parseInt(marginsPx[i].replace("px", ""));
            }
        }

        var pageSizes = pageAttributes.sizes ? pageAttributes.sizes.split(";") : null;
        if (pageSizes) {
            page.pageWidth = parseInt(pageSizes[0]);
            page.pageHeight = parseInt(pageSizes[1]);
        }

        page.margins = margins;
        page.style.overflow = "hidden";
        page.style.margin = jsObject.reportParams.viewMode == "Continuous" ? "10px auto 10px auto" : "10px";
        page.style.display = jsObject.reportParams.viewMode == "Continuous" ? "table" : "inline-block";
        page.style.textAlign = "left";
        page.style.verticalAlign = "top";
        page.style.padding = pageAttributes.margins;
        page.style.border = "1px solid " + jsObject.options.appearance.pageBorderColor;
        page.style.color = "#000000";
        page.style.boxSizing = "content-box";
        page.innerHTML = pageAttributes.content;

        page.addBackPanelToPage = function () {
            page.style.position = "relative";

            var backPanel = jsObject.CreateSvgElement("svg");
            backPanel.setAttribute("style", "height: 100%; width: 100%; top: 0; left: 0; position: absolute; z-index: -1;");
            page.backPanel = backPanel;

            if (page.firstChild)
                page.insertBefore(backPanel, page.firstChild);
            else
                page.appendChild(backPanel);
        }

        page.applyBrush = function (pageAttributes) {
            var brush = pageAttributes.brush;
            if (brush) {
                var brushArray = brush.split(";");

                switch (brushArray[0]) {
                    case "0": {
                        page.style.background = "White";
                        break;
                    }
                    case "1": {
                        var color = brushArray[1];
                        page.style.background = (color == "Transparent" || color == "rgba(255,255,255,0)") ? "White" : color;
                        break;
                    }
                    case "2": {
                        page.addBackPanelToPage();
                        page.backPanel.appendChild(jsObject.getSvgHatchBrush(brushArray, 0, 0, page.pageWidth, page.pageHeight));
                        break;
                    }
                    case "3":
                    case "4": {
                        page.addBackPanelToPage();
                        jsObject.addGradientBrushToElement(page.backPanel).applyBrush(brushArray);
                        break;
                    }
                    case "5": {
                        page.style.background = "linear-gradient(" + brushArray[4] + " 50%, " + brushArray[1] + " 50%)";
                        break;
                    }

                }
            }
            else if (pageAttributes.background) {
                page.style.background = pageAttributes.background == "Transparent" || pageAttributes.background == "rgba(255,255,255,0)" ? "White" : pageAttributes.background;
            }
        }

        page.applyBrush(pageAttributes);

        if (!pageAttributes.content)
            page.style.display = "none";

        //Correct Watermark
        if (pageAttributes.existsWatermark) {
            page.style.position = "relative";

            for (var i = 0; i < page.childNodes.length; i++) {
                if (page.childNodes[i].className == "stiWatermarkImage") {
                    page.childNodes[i].style.width = "auto";
                    page.childNodes[i].style.height = "auto";
                    page.childNodes[i].style.margin = "1px";
                    break;
                }
            }
        }

        var reportDisplayMode = jsObject.options.displayModeFromReport || jsObject.options.appearance.reportDisplayMode;

        if (reportDisplayMode == "Div" || reportDisplayMode == "Span") {
            var childs = page.getElementsByClassName("StiPageContainer");
            if (childs && childs.length > 0) {
                var pageContainer = childs[0];
                pageContainer.style.position = "relative";
                if (reportDisplayMode == "Span") pageContainer.style.margin = "0 1px"; // fix Chrome bug with SPAN position
                page.style.width = (page.pageWidth - page.margins[1] - page.margins[3]) + "px";
                page.style.height = (page.pageHeight - page.margins[0] - page.margins[2]) + "px";
            }
        }

        if (pageSizes) {
            //fixed bug with long time execute
            if (reportDisplayMode != "Table" && jsObject.reportParams.viewMode != "SinglePage") {
                setTimeout(function () {
                    var currentPageHeight = page.offsetHeight - margins[0] - margins[2];
                    if (reportPanel.maxHeights[pageSizes[1]] == null || currentPageHeight > reportPanel.maxHeights[pageSizes[1]])
                        reportPanel.maxHeights[pageSizes[1]] = currentPageHeight;
                });
            }
            else {
                var currentPageHeight = page.offsetHeight - margins[0] - margins[2];
                if (reportPanel.maxHeights[pageSizes[1]] == null || currentPageHeight > reportPanel.maxHeights[pageSizes[1]])
                    reportPanel.maxHeights[pageSizes[1]] = currentPageHeight;
            }
        }

        jsObject.InitializeInteractions(page);

        // Touch events
        page.touchesLength = 0;
        page.lastTouches = [{ x: 0, y: 0, time: 0 }, { x: 0, y: 0, time: 0 }];

        page.translateX = function (value) {
            var _this = this;
            this.style.transitionDuration = "300ms";
            this.style.transform = value == 0 ? "" : "translateX(" + value + "px)";
            setTimeout(function () {
                _this.style.transitionDuration = "";
            }, 300);
        }

        page.eventTouchStart = function (e) {
            this.touchAllowPageAction = this.touchesLength == 0 && Math.abs(reportPanel.offsetWidth - reportPanel.scrollWidth) <= 10;
            this.touchesLength++;

            if (this.touchAllowPageAction) {
                this.touchStartX = parseInt(e.changedTouches[0].clientX);
                this.touchStartScrollY = reportPanel.scrollTop;
            }
        }

        page.eventTouchMove = function (e) {
            if (this.touchAllowPageAction) {
                this.lastTouches.shift();
                this.lastTouches.push({
                    x: e.changedTouches[0].clientX,
                    y: e.changedTouches[0].clientY,
                    time: new Date().getTime()
                });

                if (reportPanel.offsetWidth == reportPanel.scrollWidth && this.touchStartScrollY == reportPanel.scrollTop) {
                    this.touchPosX = parseInt(this.lastTouches[1].x - this.touchStartX);
                    if (scrollX == 0) this.style.transform = "translateX(" + this.touchPosX + "px)";
                }
            }
        }

        page.eventTouchEnd = function (e) {
            if (this.touchesLength > 0) this.touchesLength--;
            if (this.touchAllowPageAction && this.touchesLength == 0) {
                var dX = this.lastTouches[1].x - this.lastTouches[0].x;
                var dT = new Date().getTime() - this.lastTouches[1].time;

                if (this.touchStartScrollY != reportPanel.scrollTop ||
                    (dX <= 0 && jsObject.reportParams.pageNumber >= jsObject.reportParams.pagesCount - 1) ||
                    (dX >= 0 && jsObject.reportParams.pageNumber <= 0)) {
                    this.translateX(0);
                }
                else if ((dX < -5 && dT <= 14 && this.lastTouches[1].x < this.touchStartX) ||
                    (dX < 0 && this.touchPosX < -this.pageWidth / 3)) {
                    jsObject.postAction("NextPage");
                    this.translateX(-this.pageWidth);
                }
                else if ((dX > 5 && dT <= 14 && this.lastTouches[1].x > this.touchStartX) ||
                    (dX > 0 && this.touchPosX > this.pageWidth / 3)) {
                    jsObject.postAction("PrevPage");
                    this.translateX(this.pageWidth);
                }
                else {
                    this.translateX(0);
                }
            }
        }

        if (jsObject.options.isMobileDevice) {
            jsObject.addEvent(page, "touchstart", page.eventTouchStart);
            jsObject.addEvent(page, "touchmove", page.eventTouchMove);
            jsObject.addEvent(page, "touchend", page.eventTouchEnd);
        }

        return page;
    }

    reportPanel.eventTouchStart = function (e) {
        reportPanel.touchesLength++;
        reportPanel.touchStartX = parseInt(e.changedTouches[0].clientX);

        if (jsObject.options.appearance.allowTouchZoom && reportPanel.touchesLength == 1) {
            reportPanel.touchZoomFirstDistance = 0;
            reportPanel.touchZoomSecondDistance = 0;
            reportPanel.touchZoomValue = 0;
        }
    }

    reportPanel.eventTouchMove = function (e) {
        if (jsObject.options.appearance.allowTouchZoom && e.touches.length > 1) {
            if ("preventDefault" in e) e.preventDefault();

            reportPanel.touchZoomSecondDistance = Math.sqrt(Math.pow(e.touches[0].pageX - e.touches[1].pageX, 2) + Math.pow(e.touches[0].pageY - e.touches[1].pageY, 2));
            if (reportPanel.touchZoomFirstDistance == 0)
                reportPanel.touchZoomFirstDistance = Math.sqrt(Math.pow(e.touches[0].pageX - e.touches[1].pageX, 2) + Math.pow(e.touches[0].pageY - e.touches[1].pageY, 2));

            var touchZoomOffset = parseInt((reportPanel.touchZoomSecondDistance - reportPanel.touchZoomFirstDistance) / 2.5);
            if (Math.abs(touchZoomOffset) >= 5) {
                reportPanel.touchZoomValue = parseInt((jsObject.reportParams.zoom + touchZoomOffset) / 5) * 5;
                reportPanel.touchZoomValue = Math.min(Math.max(reportPanel.touchZoomValue, 20), 200);
                jsObject.controls.centerText.setText(reportPanel.touchZoomValue);
            }
        }
    }

    reportPanel.eventTouchEnd = function (e) {
        if (reportPanel.touchesLength > 0) reportPanel.touchesLength--;

        if (jsObject.options.isMobileDevice && jsObject.options.toolbar.autoHide) {
            if (parseInt(reportPanel.touchStartX - e.changedTouches[0].clientX) != 0) {
                reportPanel.keepToolbar();
            }
            else {
                if (reportPanel.isToolbarHidden) reportPanel.showToolbar();
                else reportPanel.hideToolbar();
            }
        }

        if (jsObject.options.appearance.allowTouchZoom && reportPanel.touchZoomValue != 0 && reportPanel.touchesLength == 0) {
            jsObject.controls.centerText.hide();
            jsObject.reportParams.zoom = reportPanel.touchZoomValue;
            jsObject.postAction("GetPages");
            if (jsObject.options.toolbar.displayMode == "Separated") {
                jsObject.controls.toolbar.controls.ZoomOnePage.setSelected(false);
                jsObject.controls.toolbar.controls.ZoomPageWidth.setSelected(false);
            }
        }
    }

    reportPanel.showToolbar = function () {
        if (!jsObject.options.isMobileDevice || !jsObject.options.toolbar.autoHide) return;
        if (this.toolbarHideTimer) clearTimeout(this.toolbarHideTimer);
        jsObject.controls.toolbar.style.opacity = jsObject.controls.navigatePanel.style.opacity = 0.9;
        jsObject.controls.toolbar.style.marginTop = jsObject.controls.navigatePanel.style.marginBottom = "0";
        setTimeout(function () {
            reportPanel.isToolbarHidden = false;
            reportPanel.keepToolbar();
        }, 300);
    }

    reportPanel.hideToolbar = function () {
        if (!jsObject.options.isMobileDevice || !jsObject.options.toolbar.autoHide) return;
        if (this.toolbarHideTimer) clearTimeout(this.toolbarHideTimer);
        this.toolbarHideTimer = null;
        jsObject.controls.toolbar.style.opacity = jsObject.controls.navigatePanel.style.opacity = 0;
        jsObject.controls.toolbar.style.marginTop = jsObject.controls.navigatePanel.style.marginBottom = "-0.55in";
        setTimeout(function () {
            reportPanel.isToolbarHidden = true;
        }, 300);
    }

    reportPanel.keepToolbar = function () {
        if (!jsObject.options.isMobileDevice || !jsObject.options.toolbar.autoHide || this.isToolbarHidden) return;
        if (this.toolbarHideTimer) clearTimeout(this.toolbarHideTimer);
        clearTimeout(this.toolbarHideTimer);
        this.toolbarHideTimer = setTimeout(function () {
            reportPanel.hideToolbar();
        }, 4000);
    }

    reportPanel.getZoomByPageWidth = function () {
        var pagesWidth = this.getPagesSize().width;
        if (pagesWidth == 0) return 100;
        jsObject.calculateLayout();
        return (this.layout.width - 40) * jsObject.reportParams.zoom / pagesWidth;
    }

    reportPanel.getZoomByPageHeight = function () {
        var pagesHeight = this.getPagesSize().height;
        if (pagesHeight == 0) return 100;
        jsObject.calculateLayout();
        return (this.layout.height - 40) * jsObject.reportParams.zoom / pagesHeight;
    }

    reportPanel.addPages = function (parameters) {
        if (jsObject.reportParams.pagesArray == null) return;

        var oldScrollTop = this.scrollTop;
        this.clear();
        this.maxHeights = {};
        var count = jsObject.reportParams.pagesArray.length;

        //add pages styles
        if (!jsObject.controls.css) {
            var css = document.getElementById(jsObject.options.viewerId + "Styles");
            if (!css) {
                css = document.createElement("STYLE");
                css.id = jsObject.options.viewerId + "Styles";
                css.setAttribute('type', 'text/css');
                css.setAttribute("stimulsoft", "stimulsoft");
                jsObject.controls.head.appendChild(css);
            }
            jsObject.controls.css = css;
        }

        if (jsObject.controls.css.styleSheet)
            jsObject.controls.css.styleSheet.cssText = jsObject.reportParams.pagesArray[count - 2];
        else
            jsObject.controls.css.innerHTML = jsObject.reportParams.pagesArray[count - 2];

        //add chart scripts
        var currChartScripts = document.getElementById(jsObject.options.viewerId + "chartScriptJsViewer");
        if (currChartScripts) jsObject.controls.head.removeChild(currChartScripts);

        if (jsObject.reportParams.pagesArray[count - 1]) {
            var chartScripts = document.createElement("Script");
            chartScripts.setAttribute('type', 'text/javascript');
            chartScripts.setAttribute("stimulsoft", "stimulsoft");
            chartScripts.id = jsObject.options.viewerId + "chartScriptJsViewer";
            chartScripts.textContent = jsObject.reportParams.pagesArray[count - 1];
            jsObject.controls.head.appendChild(chartScripts);
        }
        for (var num = 0; num <= count - 3; num++) {
            var pageAttributes = jsObject.reportParams.pagesArray[num];
            var page = jsObject.reportParams.type == "Dashboard"
                ? this.addDashboard(pageAttributes)
                : this.addPage(pageAttributes);
        }

        if (jsObject.reportParams.viewMode == "MultiplePages") {
            reportPanel.correctHeights();
        }

        // eslint-disable-next-line no-undef
        if (typeof stiEvalCharts === "function") stiEvalCharts();

        if (jsObject.options.editableMode) jsObject.ShowAllEditableFields();
        jsObject.UpdateAllHyperLinks();
        jsObject.notRepaintingElement = null;

        if (parameters.action == "Collapsing" && oldScrollTop) {
            this.scrollTop = oldScrollTop;
        }
    }

    reportPanel.clear = function () {
        reportPanel.pages = [];

        if (jsObject.framesCollection) {
            for (var i = 0; i < jsObject.framesCollection.length; i++) {
                jsObject.framesCollection[i].dispatch();
            }
            jsObject.framesCollection = [];
        }

        var clearEvents = function (node) {
            if (jsObject.viewerEvents) {
                var add = [];
                for (var i = 0; i < jsObject.viewerEvents.length; i++) {
                    var evnt = jsObject.viewerEvents[i];
                    if (node === evnt.mainElement) {
                        var eventName = evnt.eventName;
                        var fn = evnt.fn;
                        if (evnt.element.removeEventListener) evnt.element.removeEventListener(eventName, fn, false);
                        else if (evnt.element.detachEvent) evnt.element.detachEvent('on' + eventName, fn);
                        else evnt.element["on" + eventName] = null;
                    }
                    else if (evnt.mainElement.parentElement) {
                        add.push(evnt);
                    }
                }
                jsObject.viewerEvents = add;
            }

            jsObject.removeAllEvents(node);

            for (var name in Object.getOwnPropertyNames(node)) {
                node[name] = null;
            }
        }

        var clearChildrens = function (node) {
            if (node instanceof HTMLIFrameElement) {
                if (node.contentWindow) {
                    var body = node.contentWindow.document.firstChild;
                    jsObject.removeAllEvents(body);
                    jsObject.removeAllEvents(node.contentWindow.document);

                    if (node.parentNode) {
                        var element = node.parentNode;
                        element.frame = null;                        

                        if ("dispatchAllGeoms" in node.contentWindow) {
                            node.contentWindow.dispatchAllGeoms();
                        }

                        jsObject.removeAllEvents(element);
                        element.removeChild(node);
                        element = null;
                    }                                       
                    
                    while (body.childNodes[0]) {
                        clearChildrens(body.childNodes[0]);
                        body.removeChild(body.childNodes[0]);
                    }
                    
                    jsObject.removeAllEvents(node);

                    node.src = "about:blank";
                    node = null;
                    body = null;
                }
            }
            else {
                for (var i = 0; i < node.childNodes.length; i++) {
                    clearChildrens(node.childNodes[i]);
                }
                clearEvents(node);
            }
        }

        while (this.childNodes[0]) {
            clearChildrens(this.childNodes[0]);
            if (this.childNodes.length > 0)
                this.removeChild(this.childNodes[0]);
        }
    }

    reportPanel.correctHeights = function () {
        for (var i in this.childNodes) {
            if (this.childNodes[i].pageHeight != null) {
                var height = reportPanel.maxHeights[this.childNodes[i].pageHeight.toString()];
                if (height) this.childNodes[i].style.height = height + "px";
            }
        }
    }

    reportPanel.pagesNavigationIsActive = function () {
        return (jsObject.options.appearance.fullScreenMode || jsObject.options.appearance.scrollbarsMode) && jsObject.reportParams.viewMode == "Continuous";
    }

    reportPanel.updateToolbarStateByPagePosition = function () {
        var reportParams = jsObject.reportParams;
        var commonPagesHeight = 0;
        var index = 0;

        for (index = 0; index < reportPanel.pages.length; index++) {
            commonPagesHeight += reportPanel.pages[index].offsetHeight + 10;
            if (commonPagesHeight > reportPanel.scrollTop) break;
        }

        if (index < reportParams.pagesCount && index >= 0 && index != reportParams.pageNumber) {
            jsObject.reportParams.pageNumber = index;
            if (jsObject.controls.toolbar) jsObject.controls.toolbar.changeToolBarState();
        }
    }

    reportPanel.onscroll = function () {
        if (reportPanel.pagesNavigationIsActive()) {
            clearTimeout(reportPanel.scrollTimer);

            var this_ = this;
            reportPanel.scrollTimer = setTimeout(function () {
                reportPanel.updateToolbarStateByPagePosition();
            }, 300);
        }
    }

    this.addEvent(reportPanel, "touchstart", reportPanel.eventTouchStart);
    this.addEvent(reportPanel, "touchmove", reportPanel.eventTouchMove);
    this.addEvent(reportPanel, "touchend", reportPanel.eventTouchEnd);
}