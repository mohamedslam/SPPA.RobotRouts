
StiJsViewer.prototype.InitializeNavigatePanel = function () {
    var navigatePanel = document.createElement("div");
    navigatePanel.id = this.controls.viewer.id + "NavigatePanel";
    navigatePanel.jsObject = this;
    navigatePanel.visible = false;
    navigatePanel.style.display = "none";
    navigatePanel.controls = {};
    this.controls.navigatePanel = navigatePanel;
    this.controls.mainPanel.appendChild(navigatePanel);
    navigatePanel.className = "stiJsViewerNavigatePanel";

    if (this.options.isMobileDevice) {
        navigatePanel.style.transition = "margin 300ms ease, opacity 300ms ease";
        if (this.options.toolbar.autoHide) navigatePanel.style.zIndex = 5;
    }

    var controlsTable = this.CreateHTMLTable();
    navigatePanel.appendChild(controlsTable);

    var controlProps = [];
    if (this.options.toolbar.showFirstPageButton) controlProps.push(["FirstPage", null, "PageFirst20.png", this.collections.loc["FirstPageToolTip"], null]);
    if (this.options.toolbar.showPreviousPageButton) controlProps.push(["PrevPage", null, "PagePrevious20.png", this.collections.loc["PrevPageToolTip"], null]);
    if (this.options.toolbar.showCurrentPageControl) {
        controlProps.push(["Separator"]);
        controlProps.push(["PageControl"]);
        controlProps.push(["Separator"]);
    }
    if (this.options.toolbar.showNextPageButton) controlProps.push(["NextPage", null, "PageNext20.png", this.collections.loc["NextPageToolTip"], null]);
    if (this.options.toolbar.showLastPageButton) controlProps.push(["LastPage", null, "PageLast20.png", this.collections.loc["LastPageToolTip"], null]);
    controlProps.push(["Space"]);
    controlProps.push(["ZoomPageWidth", null, "ZoomPageWidth20.png", this.collections.loc["ZoomPageWidth"], null]);
    controlProps.push(["ZoomOnePage", null, "ZoomOnePage20.png", this.collections.loc["ZoomOnePage"], null]);
    if (this.options.toolbar.showZoomButton) {
        controlProps.push(["Separator"]);
        controlProps.push(["Zoom", "100%", null, this.collections.loc["Zoom"], "Up"]);
    }

    for (var index = 0; index < controlProps.length; index++) {
        var name = controlProps[index][0];

        if (name.indexOf("Space") == 0) {
            controlsTable.addCell().style.width = "100%";
            continue;
        }

        if (name.indexOf("Separator") == 0) {
            controlsTable.addCell(this.NavigatePanelSeparator());
            continue;
        }

        var helpLink = this.helpLinks[name] || "user-manual/index.html?viewer_reports.htm";
        var control = (name != "PageControl")
            ? this.NavigateButton(name, controlProps[index][1], controlProps[index][2],
                (controlProps[index][3] ? [controlProps[index][3], helpLink] : null), controlProps[index][4])
            : this.PageControl();

        if (name != "PageControl") {
            if (control.caption == null) {
                control.imageCell.style.textAlign = "center";
                control.innerTable.style.width = "100%";
                control.style.width = this.options.isMobileDevice ? "0.4in" : "35px";
            }
            if (control.toolTip) {
                var positions = { top: "isNavigatePanelTooltip" }
                if (name == "Zoom" || name == "ZoomPageWidth" || name == "ZoomOnePage") {
                    positions.rightToLeft = true;
                }
                control.toolTip.push(positions);
            }
        }
        else {
            control.textBox.style.border = "0px";
        }
        if (control.arrow) StiJsViewer.setImageSource(control.arrow, this.options, this.collections, "Arrows.SmallArrowUpWhite.png");
        if (name == "FirstPage") control.style.margin = "0 1px 0 3px";
        else if (name == "Zoom") control.style.margin = "0 3px 0 1px";
        else control.style.margin = "0px 1px 0 1px";

        this.controls.toolbar.controls[name] = navigatePanel.controls[name] = control;
        controlsTable.addCell(control);
    }

    var disabledPanel = document.createElement("div");
    navigatePanel.disabledPanel = disabledPanel;
    disabledPanel.className = "stiJsViewerNavigatePanelDisabledPanel";
    navigatePanel.appendChild(disabledPanel);

    navigatePanel.setEnabled = function (state) {
        disabledPanel.style.display = state ? "none" : "";
    }

    navigatePanel.changeVisibleState = function (state) {
        this.visible = state;
        this.style.display = state ? "block" : "none";
    }

    navigatePanel.setEnabled(true);

    if (this.options.isMobileDevice) {
        this.addEvent(this.controls.toolbar, "touchstart", function () { navigatePanel.jsObject.controls.reportPanel.keepToolbar(); });
        this.addEvent(navigatePanel, "touchstart", function () { navigatePanel.jsObject.controls.reportPanel.keepToolbar(); });
        this.controls.reportPanel.showToolbar();
    }
}

//Separator
StiJsViewer.prototype.NavigatePanelSeparator = function () {
    var separator = document.createElement("div");
    separator.style.height = this.options.isMobileDevice ? "0.35in" : "22px";
    separator.className = "stiJsViewerNavigatePanelSeparator";

    return separator;
}

//Navigate Button
StiJsViewer.prototype.NavigateButton = function (name, caption, imageName, toolTip, arrowType) {
    var button = this.SmallButton(name, caption, imageName, toolTip, arrowType, "stiJsViewerNavigateButton");
    button.style.height = this.options.isMobileDevice ? "0.5in" : "35px";
    button.style.boxSizing = "border-box";
    if (button.arrow) button.arrow.style.marginTop = "1px";

    return button;
}