
StiJsViewer.prototype.InitializeToolBar = function () {    
    var toolbar = document.createElement("div");
    toolbar.controls = {};
    toolbar.separators = {};
    toolbar.shortType = false;
    toolbar.minWidth = 0;
        
    if (this.controls.toolbar) {
        this.controls.mainPanel.insertBefore(toolbar, this.controls.toolbar);
        this.controls.mainPanel.removeChild(this.controls.toolbar);
    }
    else {
        this.controls.mainPanel.appendChild(toolbar);
    }

    this.controls.toolbar = toolbar;
    
    var jsObject = toolbar.jsObject = this;
    toolbar.visible = false;
    toolbar.style.display = "none";
    toolbar.className = "stiJsViewerToolBar";
    if (this.options.toolbar.displayMode == "Separated") toolbar.className += " stiJsViewerToolBarSeparated";

    if (this.options.isMobileDevice) {
        toolbar.style.transition = "margin 300ms ease, opacity 300ms ease";
        if (this.options.toolbar.autoHide) {
            toolbar.style.position = "absolute";
            toolbar.style.zIndex = 5;
        }
    }

    if (!this.options.toolbar.visible) {
        toolbar.style.height = "0px";
        toolbar.style.width = "0px";
    }

    var toolbarInnerContent = document.createElement("div");
    toolbar.innerContent = toolbarInnerContent;
    toolbar.appendChild(toolbarInnerContent);
    if (this.options.toolbar.displayMode == "Simple") toolbarInnerContent.style.paddingTop = "2px";

    var toolbarTable = this.CreateHTMLTable();
    toolbarInnerContent.appendChild(toolbarTable);
    toolbarTable.className = "stiJsViewerToolBarTable";
    if (this.options.toolbar.displayMode == "Separated") toolbarTable.style.border = "0px";
    toolbarTable.style.margin = 0;
    toolbarTable.style.boxSizing = "border-box";

    if (this.options.toolbar.backgroundColor != "") toolbarTable.style.background = this.options.toolbar.backgroundColor;
    if (this.options.toolbar.borderColor != "") toolbarTable.style.border = "1px solid " + this.options.toolbar.borderColor;
    if (this.options.toolbar.fontColor != "") toolbarTable.style.color = this.options.toolbar.fontColor;
    toolbarTable.style.fontFamily = this.options.toolbar.fontFamily;

    var rightToLeft = this.options.previewSettingsRepToolbarReverse != null ? this.options.previewSettingsRepToolbarReverse : this.options.appearance.rightToLeft;

    var cell1 = toolbarTable.addCell();
    var cell2 = toolbarTable.addCell();
    var mainCell = (!rightToLeft) ? cell1 : cell2;
    var dopCell = (!rightToLeft) ? cell2 : cell1;
    mainCell.style.width = "100%";

    var mainTable = this.CreateHTMLTable();
    var dopTable = this.CreateHTMLTable();
    toolbar.dopTable = dopTable;
    mainCell.appendChild(mainTable);
    dopCell.appendChild(dopTable);

    var align = this.options.appearance.rightToLeft ? "right" : (this.options.toolbar.alignment == "Default" ? "left" : this.options.toolbar.alignment.toLowerCase());
    if (this.options.previewSettingsRepToolbarAlign) align = this.options.previewSettingsRepToolbarAlign.toLowerCase();;

    mainTable.setAttribute("align", align.toLowerCase());
    mainCell.style.textAlign = "-webkit-" + align.toLowerCase();
    mainTable.style.margin = "1px";
    dopTable.style.margin = "1px";

    if (!this.options.exports.showExportToPowerPoint && !this.options.exports.showExportToPdf && !this.options.exports.showExportToXps &&
        !this.options.exports.showExportToOpenDocumentWriter && !this.options.exports.showExportToOpenDocumentCalc && !this.options.exports.showExportToText &&
        !this.options.exports.showExportToRtf && !this.options.exports.showExportToWord2007 && !this.options.exports.showExportToCsv && !this.options.exports.showExportToJson &&
        !this.options.exports.showExportToDbf && !this.options.exports.showExportToXml && !this.options.exports.showExportToDif && !this.options.exports.showExportToSylk &&
        !this.options.exports.showExportToExcel && !this.options.exports.showExportToExcel2007 && !this.options.exports.showExportToExcelXml && !this.options.exports.showExportToHtml &&
        !this.options.exports.showExportToHtml5 && !this.options.exports.showExportToMht && !this.options.exports.showExportToImageBmp && !this.options.exports.showExportToImageGif &&
        !this.options.exports.showExportToImageJpeg && !this.options.exports.showExportToImageMetafile && !this.options.exports.showExportToImagePcx &&
        !this.options.exports.showExportToImagePng && !this.options.exports.showExportToImageTiff && !this.options.exports.showExportToImageSvg && !this.options.exports.showExportToImageSvgz) {
        if (!this.options.exports.showExportToDocument) this.options.toolbar.showSaveButton = false;
        this.options.toolbar.showSendEmailButton = false;
    }

    //Add Controls
    //1 - name, 2 - caption, 3 - image, 4 - showToolTip;

    var isFirst = true;
    var controlProps = []
    if (this.options.toolbar.showAboutButton) controlProps.push(["About", null, "Help.png", false]);
    if (this.options.toolbar.showAboutButton && this.options.toolbar.showDesignButton) controlProps.push(["Separator0"]);
    if (this.options.toolbar.showDesignButton) controlProps.push(["Design", this.collections.loc["Design"], "Design.png", false]);
    if (this.options.toolbar.showPinToolbarButton && this.options.toolbar.showDesignButton) controlProps.push(["Separator1"]);
    if (this.options.toolbar.showPinToolbarButton) controlProps.push(["Pin", null, "Pin.png", false]);
    if (this.options.toolbar.showPrintButton) { controlProps.push(["Print", this.collections.loc["Print"], "Print.png", true]); isFirst = false; }
    if (this.options.toolbar.showOpenButton) {
        controlProps.push(["Open", this.collections.loc["Open"], "Open.png", true]);
        isFirst = false;
    }
    if (this.options.toolbar.showSaveButton) {
        controlProps.push(["Save", this.collections.loc["Save"], "Save.png", true]);
        isFirst = false;
    }
    if (this.options.toolbar.showSendEmailButton) {
        controlProps.push(["SendEmail", this.collections.loc["SendEmail"], "SendEmail.png", true]);
        isFirst = false;
    }
    if (this.options.toolbar.showBookmarksButton || this.options.toolbar.showParametersButton) {
        if (!isFirst) controlProps.push(["Separator2"]);
        isFirst = false;
    }
    if (this.options.toolbar.showBookmarksButton) {
        controlProps.push(["Bookmarks", this.options.toolbar.displayMode == "Separated" ? this.collections.loc["Bookmarks"] : null, "Bookmarks.png", true]);
        isFirst = false;
    }
    if (this.options.toolbar.showParametersButton) {
        controlProps.push(["Parameters", this.options.toolbar.displayMode == "Separated" ? this.collections.loc["Parameters"] : null, "Parameters.png", true]);
        isFirst = false;
    }
    if (this.options.toolbar.showResourcesButton) {
        controlProps.push(["Resources", this.options.toolbar.displayMode == "Separated" ? this.collections.loc["Resources"] : null, "Resources.png", true]);
        isFirst = false;
    }
    if (this.options.toolbar.showFindButton || this.options.toolbar.showEditorButton || this.options.toolbar.showSignatureButton) {
        if (!isFirst) controlProps.push(["Separator2_1"]);
        isFirst = false;
    }
    if (this.options.toolbar.showFindButton) {
        controlProps.push(["Find", null, "Find.png", true]);
        isFirst = false;
    }
    if (this.options.toolbar.showEditorButton) {
        controlProps.push(["Editor", null, "Editor.png", true]);
        isFirst = false;
    }
    if (this.options.toolbar.showSignatureButton) {
        controlProps.push(["Signature", null, "Signature.png", true]);
        isFirst = false;
    }

    if (this.options.toolbar.displayMode != "Separated") {
        if (this.options.toolbar.showFirstPageButton || this.options.toolbar.showPreviousPageButton || this.options.toolbar.showNextPageButton ||
            this.options.toolbar.showLastPageButton || this.options.toolbar.showCurrentPageControl) {
            if (!isFirst) controlProps.push(["Separator3"]);
            isFirst = false;
        }
        if (this.options.toolbar.showFirstPageButton) { controlProps.push(["FirstPage", null, rightToLeft ? "LastPage.png" : "FirstPage.png", true]); isFirst = false; }
        if (this.options.toolbar.showPreviousPageButton) { controlProps.push(["PrevPage", null, rightToLeft ? "NextPage.png" : "PrevPage.png", true]); isFirst = false; }
        if (this.options.toolbar.showCurrentPageControl) { controlProps.push(["PageControl"]); isFirst = false; }
        if (this.options.toolbar.showNextPageButton) { controlProps.push(["NextPage", null, rightToLeft ? "PrevPage.png" : "NextPage.png", true]); isFirst = false; }
        if (this.options.toolbar.showLastPageButton) { controlProps.push(["LastPage", null, rightToLeft ? "FirstPage.png" : "LastPage.png", true]); isFirst = false; }
        if (this.options.toolbar.showViewModeButton || this.options.toolbar.showZoomButton) {
            if (!isFirst) controlProps.push(["Separator4"]);
            isFirst = false;
        }
    }

    if (this.options.toolbar.showFullScreenButton) {
        controlProps.push(["FullScreen", null, "FullScreen.png", true]);
        controlProps.push(["Separator5"]);
        isFirst = false;
    }

    if (this.options.toolbar.showZoomButton && this.options.toolbar.displayMode != "Separated") {
        controlProps.push(["Zoom", "100%", "Zoom.png", true]);
        isFirst = false;
    }

    if (this.options.toolbar.showViewModeButton) {
        controlProps.push(["ViewMode", this.collections.loc["SinglePage"], "SinglePage.png", true]);
        isFirst = false;
    }

    if (typeof (this.options.toolbar.multiPageWidthCount) != "undefined")
        this.reportParams.multiPageWidthCount = this.options.toolbar.multiPageWidthCount;

    if (typeof (this.options.toolbar.multiPageHeightCount) != "undefined")
        this.reportParams.multiPageHeightCount = this.options.toolbar.multiPageHeightCount;

    if (!rightToLeft && !this.options.previewSettingsRepToolbarReverse && (this.options.toolbar.alignment == "Right" || this.options.previewSettingsRepToolbarAlign == "right") &&
        (this.options.toolbar.showPinToolbarButton || this.options.toolbar.showAboutButton || this.options.toolbar.showDesignButton)) {
        controlProps.push(["Separator6"]);
    }

    for (var i = 0; i < controlProps.length; i++) {
        var index = rightToLeft ? controlProps.length - 1 - i : i;
        var name = controlProps[index][0];
        var table = (name == "Pin" || name == "About" || name == "Design" || name == "Separator0" || name == "Separator1") ? dopTable : mainTable;

        if (name.indexOf("Separator") == 0) {
            var sep = this.ToolBarSeparator();
            table.addCell(sep);
            toolbar.separators[name] = sep;
            continue;
        }

        var buttonArrow = ((name == "Print" && this.options.toolbar.printDestination == "Default") || name == "Save" || name == "SendEmail" || name == "Zoom" || name == "ViewMode") ? "Down" : null;
        if (this.options.isMobileDevice) buttonArrow = null;

        var helpLink = this.helpLinks[name] || "user-manual/index.html?viewer_reports.htm";
        var control = (name != "PageControl")
            ? this.SmallButton(name, controlProps[index][1], controlProps[index][2], (controlProps[index][3] ? [this.collections.loc[name + "ToolTip"], helpLink] : null), buttonArrow)
            : this.PageControl();

        if (control.caption) {
            control.caption.style.display = this.options.toolbar.showButtonCaptions ? "" : "none";
        }

        if (name == "Editor" || name == "Signature") {
            control.style.display = "none";
        }

        if (this.options.toolbar.displayMode == "Separated" && name != "PageControl") {
            control.style.height = "28px";
            if (this.options.isMobileDevice) {
                control.imageCell.style.textAlign = "center";
                control.innerTable.style.width = "100%";
                control.style.width = "0.4in";
                control.style.height = "0.5in";
            }
            if (name == "Find" || name == "Editor" || name == "Signature" || name == "FullScreen" || name == "About") {
                control.style.width = "28px";
                control.innerTable.style.width = "100%";
                control.imageCell.style.textAlign = "center";
            }
        }

        control.style.margin = (name == "Design") ? "1px 5px 1px 5px" : "1px";
        toolbar.controls[name] = control;
        table.addCell(control);
    }

    //Add Hover Events
    if (this.options.toolbar.showMenuMode == "Hover") {
        var buttonsWithMenu = ["Print", "Save", "SendEmail", "Zoom", "ViewMode"];
        for (var i = 0; i < buttonsWithMenu.length; i++) {
            var button = toolbar.controls[buttonsWithMenu[i]];
            if (button) {
                button.onmouseover = function () {
                    var menuName = jsObject.lowerFirstChar(this.name) + "Menu";
                    clearTimeout(jsObject.options.toolbar["hideTimer" + this.name + "Menu"]);
                    if (jsObject.options.isTouchDevice || !this.isEnabled || (this["haveMenu"] && this.isSelected)) return;
                    this.className = this.styleName + " " + this.styleName + "Over";
                    jsObject.controls.menus[menuName].changeVisibleState(true);
                }

                button.onmouseout = function () {
                    var menuName = jsObject.lowerFirstChar(this.name) + "Menu";
                    jsObject.options.toolbar["hideTimer" + this.name + "Menu"] = setTimeout(function () {
                        jsObject.controls.menus[menuName].changeVisibleState(false);
                    }, jsObject.options.menuHideDelay);
                }
            }
        }
    }

    var menus = jsObject.controls.menus;
    if (menus) {
        if (menus.printMenu) menus.printMenu.parentButton = toolbar.controls.Print;
        if (menus.saveMenu) menus.saveMenu.parentButton = toolbar.controls.Save;
        if (menus.viewModeMenu) menus.viewModeMenu.parentButton = toolbar.controls.ViewMode;
        if (menus.zoomMenu && jsObject.options.toolbar.displayMode != "Separated") menus.zoomMenu.parentButton = toolbar.controls.Zoom;        
    }

    var navigatePanel = jsObject.controls.navigatePanel;
    if (navigatePanel) {
        for (var name in navigatePanel.controls) {
            toolbar.controls[name] = navigatePanel.controls[name];
        }
    }

    toolbar.haveScroll = function () {
        return (toolbar.scrollWidth > toolbar.offsetWidth)
    }

    toolbar.getMinWidth = function () {
        var a = mainCell.offsetWidth;
        var b = mainTable.offsetWidth
        var c = toolbarTable.offsetWidth;

        return c - (a - b) + 50;
    }

    toolbar.minWidth = toolbar.getMinWidth();

    toolbar.changeToolBarState = function () {
        var reportParams = jsObject.reportParams;
        var controls = toolbar.controls;
        var collections = jsObject.collections;
        var disableNaviButtons = reportParams.viewMode == "MultiplePages" || reportParams.viewMode == "WholeReport" ||
            (reportParams.viewMode == "Continuous" && !jsObject.options.appearance.scrollbarsMode && !jsObject.options.appearance.fullScreenMode);

        if (reportParams.type == "Report" && reportParams.pagesCount > 0 && reportParams.pageNumber > reportParams.pagesCount - 1)
            reportParams.pageNumber = reportParams.pagesCount - 1;

        if (controls["FirstPage"]) controls["FirstPage"].setEnabled(reportParams.pageNumber > 0 && !disableNaviButtons);
        if (controls["PrevPage"]) controls["PrevPage"].setEnabled(reportParams.pageNumber > 0 && !disableNaviButtons);
        if (controls["NextPage"]) controls["NextPage"].setEnabled(reportParams.pageNumber < reportParams.pagesCount - 1 && !disableNaviButtons);
        if (controls["LastPage"]) controls["LastPage"].setEnabled(reportParams.pageNumber < reportParams.pagesCount - 1 && !disableNaviButtons);
        if (controls["ViewMode"]) {
            var viewMode = reportParams.viewMode;
            if (viewMode == "OnePage") viewMode = "SinglePage";
            if (viewMode == "WholeReport") viewMode = "MultiplePages";
            if (collections.loc[viewMode]) controls["ViewMode"].caption.innerHTML = collections.loc[viewMode];
            if (StiJsViewer.checkImageSource(jsObject.options, jsObject.collections, viewMode + ".png")) StiJsViewer.setImageSource(controls["ViewMode"].image, jsObject.options, collections, viewMode + ".png");
        }
        if (controls["Zoom"] && reportParams.zoom) controls["Zoom"].caption.innerHTML = reportParams.zoom + "%";
        if (controls["PageControl"]) {
            controls["PageControl"].countLabel.innerHTML = reportParams.pagesCount || 0;
            controls["PageControl"].textBox.value = reportParams.pageNumber + 1;
            controls["PageControl"].setEnabled(!(reportParams.pagesCount <= 1 || disableNaviButtons));
        }

        if (jsObject.controls.menus["zoomMenu"]) {
            var zoomItems = jsObject.controls.menus["zoomMenu"].items;
            for (var i in zoomItems) {
                if (zoomItems[i]["image"] == null) continue;
                if (zoomItems[i].name != "ZoomOnePage" && zoomItems[i].name != "ZoomPageWidth") {
                    zoomItems[i].setSelected(zoomItems[i].name == "Zoom" + reportParams.zoom);
                    zoomItems[i].image.style.visibility = "hidden";
                }
            }
        }
    }

    toolbar.changePinState = function (state) {
        jsObject.options.toolbar.autoHide = state;
        if (jsObject.options.isMobileDevice) {
            if (state) {
                toolbar.style.position = "absolute";
                toolbar.style.zIndex = 5;
                jsObject.controls.reportPanel.style.marginBottom = "0";
                if (jsObject.controls.navigatePanel) jsObject.controls.navigatePanel.style.zIndex = 5;
                setTimeout(function () {
                    jsObject.controls.reportPanel.hideToolbar();
                }, 200);
            }
            else {
                toolbar.style.position = "relative";
                toolbar.style.zIndex = 2;
                toolbar.style.opacity = 1;
                jsObject.controls.reportPanel.style.marginBottom = "0.5in";
                if (jsObject.controls.navigatePanel) {
                    jsObject.controls.navigatePanel.style.zIndex = 2;
                    jsObject.controls.navigatePanel.style.opacity = 1;
                }
            }

            toolbar.jsObject.updateLayout();
            if (jsObject.controls.parametersPanel) jsObject.InitializeParametersPanel();
            if (jsObject.controls.bookmarksPanel) jsObject.InitializeBookmarksPanel();
        }
    }

    toolbar.changeShortType = function () {
        if (toolbar.shortType && jsObject.controls.viewer.offsetWidth < toolbar.minWidth) return;
        toolbar.shortType = jsObject.controls.viewer.offsetWidth < toolbar.minWidth;
        var shortButtons = ["Print", "Save", "Zoom", "ViewMode", "Design"];
        for (var index in shortButtons) {
            button = toolbar.controls[shortButtons[index]];
            if (button && button.caption) {
                button.caption.style.display = toolbar.shortType ? "none" : "";
            }
        }
    }

    toolbar.changeVisibleState = function (state) {
        this.style.display = state ? "block" : "none";
        this.visible = state;
    }

    toolbar.setEnabled = function (state) {
        if (!state) {
            if (!toolbar.disabledPanel) {
                toolbar.disabledPanel = document.createElement("div");
                toolbar.disabledPanel.className = "stiJsViewerDisabledPanel";
                toolbar.appendChild(toolbar.disabledPanel);
            }
        }
        else if (toolbar.disabledPanel) {
            toolbar.removeChild(toolbar.disabledPanel);
            toolbar.disabledPanel = null;
        }
    }

    if (toolbar.controls["Bookmarks"]) toolbar.controls["Bookmarks"].setEnabled(false);
    if (toolbar.controls["Parameters"]) toolbar.controls["Parameters"].setEnabled(false);
    if (toolbar.controls["Resources"]) toolbar.controls["Resources"].setEnabled(false);

    return toolbar;
}

//Separator
StiJsViewer.prototype.ToolBarSeparator = function () {
    var separator = document.createElement("div");
    separator.style.width = "1px";
    if (this.options.isMobileDevice) separator.style.height = "0.4in";
    else if (this.options.isTouchDevice) separator.style.height = "26px";
    else separator.style.height = "21px";
    separator.className = "stiJsViewerToolBarSeparator";

    return separator;
}

//PageControl
StiJsViewer.prototype.PageControl = function () {
    var pageControl = this.CreateHTMLTable();
    var text1 = pageControl.addCell();
    text1.style.padding = "0 2px 0 2px";
    text1.style.whiteSpace = "nowrap";
    text1.innerHTML = this.collections.loc["Page"];

    var textBox = null;
    if (this.options.isMobileDevice) {
        textBox = document.createElement("span");
        Object.defineProperty(textBox, "value", {
            get: function () {
                return parseInt(this.innerHTML);
            },
            set: function (value) {
                this.innerHTML = value;
            }
        });
        pageControl.setEnabled = function (state) { };
    }
    else {
        textBox = this.TextBox("PageControl", 45);
        pageControl.setEnabled = function (state) {
            this.textBox.setEnabled(state);
            var toolbarFontColor = this.jsObject.options.toolbar.fontColor;
            this.textBox.style.color = state
                ? (toolbarFontColor && toolbarFontColor != "Empty" ? toolbarFontColor : "")
                : (this.jsObject.reportParams && this.jsObject.reportParams.viewMode != "SinglePage" ? "transparent" : toolbarFontColor);
            this.style.opacity = state ? "1" : "0.5";
        }
    }

    pageControl.addCell(textBox);
    pageControl.textBox = textBox;
    textBox.action = function () {
        if (textBox.jsObject.reportParams.pagesCount > 0 && textBox.jsObject.reportParams.pageNumber != textBox.getCorrectValue() - 1) {
            textBox.jsObject.postAction("GoToPage");
        }
    }

    textBox.getCorrectValue = function () {
        var value = parseInt(this.value);
        if (value < 1 || !value) value = 1;
        if (value > textBox.jsObject.reportParams.pagesCount) value = textBox.jsObject.reportParams.pagesCount;
        return value;
    }

    var text2 = pageControl.addCell();
    text2.style.padding = "0 2px 0 2px";
    text2.innerHTML = this.collections.loc["PageOf"];

    var countLabel = pageControl.addCell();
    pageControl.countLabel = countLabel;
    countLabel.style.padding = "0 2px 0 0";
    countLabel.innerHTML = "?";

    return pageControl;
}
