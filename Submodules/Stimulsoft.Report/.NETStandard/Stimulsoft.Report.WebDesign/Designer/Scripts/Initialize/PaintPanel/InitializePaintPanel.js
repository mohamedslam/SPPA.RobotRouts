
StiMobileDesigner.prototype.InitializePaintPanel = function () {
    var paintPanel = document.createElement("div");
    this.options.paintPanel = paintPanel;
    this.options.mainPanel.appendChild(paintPanel);
    var jsObject = paintPanel.jsObject = this;
    paintPanel.className = "stiDesignerPaintPanel";
    paintPanel.style.cursor = "default";
    paintPanel.style.top = (this.options.toolBar.offsetHeight + this.options.workPanel.offsetHeight + this.options.pagesPanel.offsetHeight + this.options.infoPanel.offsetHeight) + "px";
    paintPanel.style.bottom = this.options.statusPanel.offsetHeight + "px";

    if (this.options.propertiesGridPosition == "Right") {
        paintPanel.style.left = "0px";
        paintPanel.style.right = this.options.propertiesPanel.offsetWidth + "px";
    }
    else {
        paintPanel.style.left = this.options.propertiesPanel.offsetWidth + (this.options.toolbox ? this.options.toolbox.offsetWidth : 0) + "px";
        paintPanel.style.right = "0px";
    }

    paintPanel.previewPages = [];

    paintPanel.oncontextmenu = function (event) {
        return false;
    }

    paintPanel.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        this.previewPages = [];
    }

    paintPanel.addPage = function (page) {
        this.appendChild(page);
    }

    paintPanel.removePage = function (page) {
        this.removeChild(page);
    }

    paintPanel.findPageByIndex = function (pageIndex) {
        for (var pageName in jsObject.options.report.pages)
            if (jsObject.options.report.pages[pageName].properties.pageIndex == pageIndex)
                return jsObject.options.report.pages[pageName];

        return false;
    }

    paintPanel.getPagesCount = function () {
        return (jsObject.options.report && jsObject.options.report.pages ? Object.keys(jsObject.options.report.pages).length : 0);
    }

    paintPanel.showPage = function (page) {
        if (jsObject.options.currentForm && jsObject.options.currentForm.isNotModal) {
            jsObject.options.currentForm.changeVisibleState(false);
        }
        var pagesButtons = jsObject.options.pagesPanel.pagesContainer.pages;
        var pageIndex = jsObject.StrToInt(page.properties.pageIndex);
        for (var i = 0; i < pagesButtons.length; i++) {
            pagesButtons[i].setSelected(false);
        }
        if (pageIndex < pagesButtons.length) {
            pagesButtons[pageIndex].setSelected(true);
        }
        if (jsObject.options.currentPage != null) {
            jsObject.options.currentPage.style.display = "none";
        }
        jsObject.PreZoomPage(page);
        page.style.display = "";
        jsObject.options.currentPage = page;
        page.setSelected();
        jsObject.UpdatePropertiesControls();

        jsObject.options.buttons.unitButton.style.display = page.isDashboard ? "none" : "";
        jsObject.SetWindowTitle();
    }

    paintPanel.setCopyStyleMode = function (state) {
        this.style.cursor = state ? (jsObject.options.jsMode ? "pointer" : "url('" + jsObject.options.urlCursorStyleSet + "'), pointer") : "default";
        this.copyStyleMode = state;
        jsObject.options.buttons.copyStyleButton.setSelected(state);
        if (state) {
            jsObject.options.copyStyleProperties = {};
            jsObject.SaveCurrentStylePropertiesToObject(jsObject.options.copyStyleProperties);
        }
        else {
            jsObject.options.copyStyleProperties = null;
        }
    }

    paintPanel.changeCursorType = function (draw) {
        this.style.cursor = draw
            ? (jsObject.options.jsMode ? "crosshair" : ("url('" + jsObject.options.urlCursorPen + "'), crosshair"))
            : (this.copyStyleMode ? (jsObject.options.jsMode ? "pointer" : "url('" + jsObject.options.urlCursorStyleSet + "'), pointer") : "default");
    }

    paintPanel.addPreviewPage = function (pageAttributes) {
        var page = document.createElement("DIV");
        page.style.margin = "10px";
        page.style.display = "inline-block";
        page.style.verticalAlign = "top";
        page.style.padding = pageAttributes["margins"];
        page.style.border = "1px solid gray";
        page.style.color = "#000000";
        page.style.background = pageAttributes["background"];
        page.innerHTML = pageAttributes["content"];
        var pageSizes = pageAttributes["sizes"].split(";");
        var marginsPx = pageAttributes["margins"].split(" ");
        var margins = [];
        for (var i = 0; i < marginsPx.length; i++) {
            margins.push(parseInt(marginsPx[i].replace("px", "")));
        }
        page.pageHeight = parseInt(pageSizes[1]);
        this.appendChild(page);
        this.previewPages.push(page);

        var currentPageHeight = page.offsetHeight - margins[0] - margins[2];
        if (paintPanel.maxHeights[pageSizes[1]] == null || currentPageHeight > paintPanel.maxHeights[pageSizes[1]])
            paintPanel.maxHeights[pageSizes[1]] = currentPageHeight;
    }

    paintPanel.addPreviewPages = function () {
        if (jsObject.previewCountPages == 0 || jsObject.options.previewPagesArray.length < 3) {
            var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
            errorMessageForm.show("Error: Report cannot be rendered!");
        }

        if (jsObject.options.previewPagesArray == null) return;
        this.removePreviewPages();
        this.maxHeights = {};
        var count = jsObject.options.previewPagesArray.length;

        if (jsObject.options.css == null) {
            jsObject.options.css = document.createElement("STYLE");
            jsObject.options.css.setAttribute('type', 'text/css');
            jsObject.options.css.setAttribute("stimulsoft", "stimulsoft");
            jsObject.options.head.appendChild(jsObject.options.css);
        }

        //add pages styles
        if (jsObject.options.css.styleSheet) jsObject.options.css.styleSheet.cssText = jsObject.options.previewPagesArray[count - 2];
        else jsObject.options.css.innerHTML = jsObject.options.previewPagesArray[count - 2];

        //add chart scripts
        var currChartScripts = document.getElementById("chartScriptMobileDesigner");
        if (currChartScripts) jsObject.options.head.removeChild(currChartScripts);

        if (jsObject.options.previewPagesArray[count - 1]) {
            var chartScripts = document.createElement("Script");
            chartScripts.setAttribute('type', 'text/javascript');
            chartScripts.id = "chartScriptMobileDesigner";
            chartScripts.setAttribute("stimulsoft", "stimulsoft");
            chartScripts.textContent = jsObject.options.previewPagesArray[count - 1];
            jsObject.options.head.appendChild(chartScripts);
        }

        //add page contents
        for (var num = 0; num <= count - 3; num++) {
            this.addPreviewPage(jsObject.options.previewPagesArray[num]);
        }

        paintPanel.correctHeights();

        // eslint-disable-next-line no-undef
        if (typeof stiEvalCharts === "function") stiEvalCharts();
    }

    paintPanel.correctHeights = function () {
        for (var i = 0; i < this.childNodes.length; i++) {
            if (this.childNodes[i].pageHeight != null) {
                var height = paintPanel.maxHeights[this.childNodes[i].pageHeight.toString()];
                if (height) this.childNodes[i].style.height = height + "px";
            }
        }
    }

    paintPanel.removePreviewPages = function () {
        for (var i = 0; i < this.previewPages.length; i++) {
            this.removeChild(this.previewPages[i]);
        }
        this.previewPages = [];
    }

    paintPanel.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        this.style.top = (jsObject.options.toolBar.offsetHeight + jsObject.options.workPanel.offsetHeight + jsObject.options.pagesPanel.offsetHeight + jsObject.options.infoPanel.offsetHeight) + "px";
    }

    paintPanel.onmouseup = function () {
        if (!jsObject.options.isTouchClick) paintPanel.action();
    }

    paintPanel.ontouchend = function () {
        if (jsObject.options.isTouchClick) paintPanel.action();
    }

    paintPanel.ondblclick = function () {
        if (jsObject.options.previewMode) return;
        if (!jsObject.options.pageIsDblClick && !jsObject.options.pageIsTouched &&
            jsObject.options.showFileMenuReportSetup && jsObject.options.report) {
            jsObject.InitializeReportSetupForm(function (reportSetupForm) {
                reportSetupForm.show();
            });
        }
        jsObject.options.pageIsDblClick = false;
    }

    paintPanel.action = function () {
        if (!jsObject.options.pageIsTouched && !jsObject.options.in_drag && !jsObject.options.in_resize &&
            jsObject.options.report && !jsObject.options.itemInDrag) {
            if (jsObject.options.currentPage && jsObject.options.selectingRect) {
                jsObject.MultiSelectComponents(jsObject.options.currentPage);
                jsObject.PaintSelectedLines();
            }
            else if (jsObject.options.currentPage && jsObject.options.drawComponent) {
                jsObject.options.currentPage.ontouchend();
            }
            else {
                jsObject.options.report.setSelected();
                if (jsObject.options.propertiesPanel && jsObject.options.showPropertiesGrid) {
                    jsObject.options.propertiesPanel.showContainer("Properties");
                }
                jsObject.UpdatePropertiesControls();
                if (jsObject.options.currentPage) {
                    jsObject.options.currentPage.updateComponentsLevels();
                }
            }
        }
        jsObject.options.pageIsTouched = false;
    }
}