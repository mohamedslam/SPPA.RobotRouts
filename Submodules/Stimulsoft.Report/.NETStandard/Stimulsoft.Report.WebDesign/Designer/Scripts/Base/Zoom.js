
StiMobileDesigner.prototype.ZoomPage = function (pageName) {
    if (!this.options || !this.options.report) return;
    var page = this.options.report.pages[pageName];
    page.repaint();

    if ("viewBox" in page && page.viewBox["baseVal"]) {
        page.viewBox.baseVal.height = 0; //for IE
        page.viewBox.baseVal.width = 0; //for IE
        page.removeAttribute("viewBox");
    }

    page.repaintAllComponents();

    if (this.options.zoomWithTouch) {
        this.SendCommandRebuildPage(page);
    }

    if (this.options.isTouchDevice) {
        var selectedObject = this.options.selectedObject;
        if (selectedObject && selectedObject.typeComponent != "StiPage" && selectedObject.typeComponent != "StiReport") {
            this.ChangeVisibilityStateResizingIcons(selectedObject, true);
        }
    }
}

StiMobileDesigner.prototype.PreZoomPage = function (page) {
    var pageWidthPx = parseInt(this.ConvertUnitToPixel(this.StrToDouble(page.properties.unitWidth), page.isDashboard) * this.options.report.zoom);
    var pageHeightPx = parseInt(this.ConvertUnitToPixel(this.StrToDouble(page.properties.unitHeight), page.isDashboard) * this.options.report.zoom);

    var marginsStr = page.properties.unitMargins.split("!");
    var verticalMarginsPx = parseInt(this.ConvertUnitToPixel(this.StrToDouble(marginsStr[1]) + this.StrToDouble(marginsStr[3]), page.isDashboard) * this.options.report.zoom);
    var horizontalMarginsPx = parseInt(this.ConvertUnitToPixel(this.StrToDouble(marginsStr[0]) + this.StrToDouble(marginsStr[2]), page.isDashboard) * this.options.report.zoom);

    if (page.isDashboard) {
        pageWidthPx += horizontalMarginsPx;
        pageHeightPx += verticalMarginsPx;
    }

    var segmentPerHeight = this.StrToDouble(page.properties.segmentPerHeight);
    var segmentPerWidth = this.StrToDouble(page.properties.segmentPerWidth);
    if (segmentPerWidth > 1) pageWidthPx = ((pageWidthPx - horizontalMarginsPx) * segmentPerWidth) + horizontalMarginsPx;
    if (segmentPerHeight > 1) pageHeightPx = ((pageHeightPx - verticalMarginsPx) * segmentPerHeight) + verticalMarginsPx;

    var largeHeightFactor = (page.properties.largeHeight) ? this.StrToInt(page.properties.largeHeightFactor) : this.StrToDouble(page.properties.largeHeightAutoFactor);
    pageHeightPx = (pageHeightPx - verticalMarginsPx) * largeHeightFactor + verticalMarginsPx;

    page.setAttribute("width", pageWidthPx);
    page.setAttribute("height", pageHeightPx);

    this.options.controls.zoomScale.setZoomPosition();

    if (!this.options.previewMode) {
        page.style.display = 'none';  //for Chrome
        page.style.display = '';   //for Chrome
    }

    page.setAttribute("viewBox", "0,0," + page.widthPx + "," + page.heightPx);

    clearTimeout(this.options.zoomTimer);

    var jsObject = this;
    this.options.zoomTimer = setTimeout(function () {
        jsObject.ZoomPage(page.properties.name);
    }, 300);
}

