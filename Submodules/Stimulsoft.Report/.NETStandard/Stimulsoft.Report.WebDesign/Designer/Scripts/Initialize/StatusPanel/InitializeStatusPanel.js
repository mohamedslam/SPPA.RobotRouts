
StiMobileDesigner.prototype.InitializeStatusPanel = function () {
    var statusPanel = document.createElement("div");
    statusPanel.className = "stiDesignerStatusPanel";
    var jsObject = statusPanel.jsObject = this;
    this.options.statusPanel = statusPanel;
    this.options.mainPanel.appendChild(statusPanel);

    statusPanel.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        jsObject.options.paintPanel.style.bottom = this.offsetHeight + "px";
    }

    var innerTable = this.CreateHTMLTable();
    innerTable.style.width = "100%";
    statusPanel.appendChild(innerTable);

    /* Unit Button */
    var unitButton = this.StatusPanelButton("unitButton", this.loc.PropertyEnum.StiReportUnitTypeCentimeters, null, null, "Up", 35);
    StiMobileDesigner.setImageSource(unitButton.arrow, this.options, "Arrows.SmallArrowUpWhite.png");
    unitButton.innerTable.style.margin = "0 10px 0 10px";
    unitButton.style.marginLeft = "3px";
    innerTable.addCell(unitButton);
    this.UnitMenu();

    unitButton.updateCaption = function (reportUnit) {
        var captionText = "";
        switch (reportUnit) {
            case "cm": { captionText = jsObject.loc.PropertyEnum.StiReportUnitTypeCentimeters; break; }
            case "hi": { captionText = jsObject.loc.PropertyEnum.StiReportUnitTypeHundredthsOfInch; break; }
            case "in": { captionText = jsObject.loc.PropertyEnum.StiReportUnitTypeInches; break; }
            case "mm": { captionText = jsObject.loc.PropertyEnum.StiReportUnitTypeMillimeters; break; }
        }
        unitButton.caption.innerHTML = captionText;
    }

    var sep1 = document.createElement("div");
    sep1.className = "stiDesignerStatusPanelSeparator";
    innerTable.addCell(sep1);

    /* Report Checker */
    var reportCheckerButton = this.StatusPanelButton("reportCheckerButton", this.loc.MainMenu.menuCheckIssues, "Arrows.ArrowRightWhite.png", null, null, 35);
    innerTable.addCell(reportCheckerButton);

    var checkTypes = ["Error", "Warning", "Information", "ReportRenderingMessage"];
    for (var i = 0; i < checkTypes.length; i++) {
        var img = document.createElement("img");
        img.style.width = img.style.height = "16px";
        img.style.margin = "0 3px 0 6px";
        StiMobileDesigner.setImageSource(img, this.options, "ReportChecker." + checkTypes[i] + ".png");
        reportCheckerButton["cellImage" + checkTypes[i]] = reportCheckerButton.innerTable.addCell(img);
        reportCheckerButton["cellValue" + checkTypes[i]] = reportCheckerButton.innerTable.addCell();
    }

    reportCheckerButton.updateCaption = function (checkItems) {
        for (var i = 0; i < checkTypes.length; i++) {
            reportCheckerButton["cellImage" + checkTypes[i]].style.display = reportCheckerButton["cellValue" + checkTypes[i]].style.display = "none";
        }

        if (checkItems) {
            var errorCount = 0;
            var warningCount = 0;
            var informationCount = 0;
            var reportRenderingMessageCount = 0;

            for (var i = 0; i < checkItems.length; i++) {
                switch (checkItems[i].status) {
                    case "Error": errorCount++; break;
                    case "Warning": warningCount++; break;
                    case "Information": informationCount++; break;
                    case "ReportRenderingMessage": reportRenderingMessageCount++; break;
                }
            }

            if (errorCount > 0) {
                reportCheckerButton.cellImageError.style.display = reportCheckerButton.cellValueError.style.display = "";
                reportCheckerButton.cellValueError.innerHTML = errorCount;
            }
            if (warningCount > 0) {
                reportCheckerButton.cellImageWarning.style.display = reportCheckerButton.cellValueWarning.style.display = "";
                reportCheckerButton.cellValueWarning.innerHTML = warningCount;
            }
            if (informationCount > 0) {
                reportCheckerButton.cellImageInformation.style.display = reportCheckerButton.cellValueInformation.style.display = "";
                reportCheckerButton.cellValueInformation.innerHTML = informationCount;
            }
            if (reportRenderingMessageCount > 0) {
                reportCheckerButton.cellImageReportRenderingMessage.style.display = reportCheckerButton.cellValueReportRenderingMessage.style.display = "";
                reportCheckerButton.cellValueReportRenderingMessage.innerHTML = reportRenderingMessageCount;
            }
        }
    }

    reportCheckerButton.updateCaption();

    reportCheckerButton.action = function () {
        jsObject.SendCommandGetReportCheckItems(function (answer) {
            if (jsObject.options.buttons.reportCheckerButton) {
                jsObject.options.buttons.reportCheckerButton.updateCaption(answer.checkItems);
            }
            jsObject.InitializeReportCheckForm(function (reportCheckForm) {
                reportCheckForm.show(answer.checkItems);
            });
        });
    }

    innerTable.addCell(this.StatusPanelSeparator());

    /* Component Name */
    statusPanel.componentNameCell = innerTable.addCell();
    statusPanel.componentNameCell.style.padding = "0px 10px 0px 10px";
    statusPanel.componentNameCell.style.whiteSpace = "nowrap";

    innerTable.addCell(this.StatusPanelSeparator());

    /* Component Name */
    statusPanel.positionsCell = innerTable.addCell();
    statusPanel.positionsCell.style.display = "none";
    statusPanel.positionsCell.style.padding = "0px 10px 0px 10px";
    statusPanel.positionsCell.style.whiteSpace = "nowrap";

    var sep4 = this.StatusPanelSeparator();
    sep4.style.display = "none";
    innerTable.addCell(sep4);

    statusPanel.showPositions = function (x, y, width, height) {
        var posText = "";
        if (x != null) posText += " X:" + jsObject.StrToDouble(x).toFixed(2);
        if (y != null) posText += " Y:" + jsObject.StrToDouble(y).toFixed(2);
        if (width != null) posText += " " + jsObject.loc.PropertyMain.Width + ":" + jsObject.StrToDouble(width).toFixed(2);
        if (height != null) posText += " " + jsObject.loc.PropertyMain.Height + ":" + jsObject.StrToDouble(height).toFixed(2);
        statusPanel.positionsCell.innerHTML = posText;
        statusPanel.positionsCell.style.display = posText ? "" : "none";
        sep4.style.display = posText ? "" : "none";
    }

    /* Loading Image */
    innerTable.addCell(this.ProcessImageStatusPanel()).style.width = "100%";

    //Page Width & Height
    innerTable.addCell(this.StatusPanelSeparator());

    var pageWidthButton = this.StatusPanelButton("zoomPageWidth", null, "ZoomPageWidth.png", this.loc.PropertyMain.PageWidth, null, null, 35);
    innerTable.addCell(pageWidthButton);

    pageWidthButton.action = function () {
        jsObject.SetZoomBy("Width");
    }

    var pageHeightButton = this.StatusPanelButton("zoomPageHeight", null, "ZoomPageHeight.png", this.loc.PropertyMain.PageHeight, null, null, 35);
    innerTable.addCell(pageHeightButton);

    pageHeightButton.action = function () {
        jsObject.SetZoomBy("Height");
    }

    var zoom100 = this.StatusPanelButton("zoom100", null, "Zoom100.png", "100%", null, null, 35);
    innerTable.addCell(zoom100);

    zoom100.action = function () {
        jsObject.options.report.zoom = 1;
        jsObject.PreZoomPage(jsObject.options.currentPage);
    }

    innerTable.addCell(this.StatusPanelSeparator());

    /* Zoom */
    innerTable.addCell(this.ZoomControl());
}

StiMobileDesigner.prototype.StatusPanelSeparator = function () {
    var sep = document.createElement("div");
    sep.className = "stiDesignerStatusPanelSeparator";

    return sep;
}

StiMobileDesigner.prototype.SetZoomBy = function (zoomType) {
    if (!this.options.currentPage || !this.options.report) return;
    var newZoom = ((this.options.paintPanel["offset" + zoomType] - (this.options.paintPanelPadding * 2 + 20)) *
        this.options.report.zoom / this.options.currentPage[zoomType == "Width" ? "widthPx" : "heightPx"]);
    if (newZoom > 2) newZoom = 2;
    this.options.report.zoom = newZoom;
    this.PreZoomPage(this.options.currentPage);
}