
StiJsViewer.prototype.InitializeFolderReportsPanel = function () {
    this.options.cloudParameters = this.options.formValues;
    var folderKey = this.options.cloudParameters ? this.options.cloudParameters.folderKey : null;
    if (!folderKey || this.options.jsDesigner) return;
    var productName = "STIMULSOFT";
    var currItemKey = this.options.cloudParameters ? /*this.options.cloudParameters.reportSnapshotItemKey ||*/ this.options.cloudParameters.reportTemplateItemKey : null;
    var restUrl = this.options.cloudParameters ? this.options.cloudParameters.restUrl : null;

    var jsObject = this;
    var panel = document.createElement("div");
    this.controls.folderReportsPanel = panel;
    panel.className = "stiJsViewerFolderReportsPanel";
    panel.selectedButton = null;

    //Logo Table
    var logo = document.createElement("div");
    logo.className = "stiJsViewerFolderReportsPanelLogo";
    var logoTable = this.CreateHTMLTable();
    panel.appendChild(logo);
    logo.appendChild(logoTable);

    //Resize Item
    var resizeItem = document.createElement("div");
    logoTable.addCell(resizeItem).style.width = "44px";
    resizeItem.className = "stiJsViewerFolderReportsPanelItem";
    var resizeImg = document.createElement("img");
    resizeImg.style.width = resizeImg.style.height = "16px";
    resizeImg.style.margin = "12px 0 0 14px";
    StiJsViewer.setImageSource(resizeImg, jsObject.options, jsObject.collections, "MainItem.png");
    resizeItem.appendChild(resizeImg);

    resizeItem.onclick = function () {
        panel.resize();
    }

    //Logo Text
    var logoText = logoTable.addTextCell(productName);
    logoText.style.textAlign = "center";
    logoText.style.width = "100%";
    logoText.style.paddingRight = "5px";
    logoText.onclick = function () { window.open("https://www.stimulsoft.com/en"); };

    //Container
    var reportsContainer = document.createElement("div");
    reportsContainer.className = "stiJsViewerFolderReportsPanelContainer";
    panel.appendChild(reportsContainer);

    panel.clear = function () {
        while (reportsContainer.childNodes[0]) reportsContainer.removeChild(reportsContainer.childNodes[0]);
    }

    panel.buildReports = function (rootItemKey, completeFunc) {
        var optionsObject = {
            Options: {
                SortType: "Name",
                SortDirection: "Ascending",
                ViewMode: "All",
                FilterIdent: "ReportTemplateItem"
            }
        }
        if (rootItemKey) optionsObject.ItemKey = rootItemKey;

        jsObject.SendCloudCommand("ItemFetchAll", optionsObject,
            function (data) {
                if (data.ResultItems) {
                    completeFunc(data.ResultItems);
                }
            });
    }

    panel.addToViewer = function () {
        if (jsObject.controls.mainPanel.parentElement) {
            jsObject.controls.mainPanel.parentElement.insertBefore(panel, jsObject.controls.mainPanel);
            jsObject.controls.mainPanel.style.marginLeft = panel.offsetWidth + "px";
            jsObject.controls.mainPanel.style.width = "calc(100% - " + panel.offsetWidth + "px)";

            var minimize = jsObject.GetCookie("StimulsoftWebDemoMainMenuMinimize");
            panel.resize(minimize == null ? false : minimize == "true", true);
            jsObject.calculateLayout();
        }
    }

    panel.loadReportItemToViewer = function (itemObject) {
        var viewerParameters = jsObject.copyObject(jsObject.options.cloudParameters);
        viewerParameters.reportTemplateItemKey = itemObject.Key;
        viewerParameters.reportName = StiBase64.encode(itemObject.Name);
        if (viewerParameters["windowTitle"]) delete viewerParameters["windowTitle"];
        if (viewerParameters["attachedItems"]) delete viewerParameters["attachedItems"];
        jsObject.controls.processImage.show();

        setTimeout(function () {
            var viewerUrl = viewerParameters.restUrl + "mobileviewer";
            jsObject.postForm(viewerUrl, viewerParameters, document, true);
        }, 50);
    }

    panel.loadDashboardToDbsViewer = function (itemObject) {
        jsObject.controls.processImage.show();
        panel.selectButtonByItemKey(itemObject.Key);

        setTimeout(function () {
            jsObject.SendCloudCommand("ItemResourceGet", { ItemKey: itemObject.Key },
                function (data) {
                    if (data.ResultResource) {
                        document.title = itemObject.Name + " - " + jsObject.collections.loc["FormViewerTitle"];
                        jsObject.postOpen(itemObject.Name + ".mrt", data.ResultResource);
                    }
                    else
                        jsObject.controls.processImage.hide();
                },
                function (data) {
                    jsObject.controls.processImage.hide();
                    if (data) {
                        var errorForm = jsObject.controls.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                        errorForm.show(jsObject.formatResultMsg(data));
                    }
                }
            );
        }, 50);
    }

    panel.buildReports(folderKey != "root" ? folderKey : null, function (resultItems) {
        var reportItems = [];
        for (var i = 0; i < resultItems.length; i++) {
            if (resultItems[i].Ident == "ReportTemplateItem"/*|| resultItems[i].Ident == "ReportSnapshotItem"*/) {
                reportItems.push(resultItems[i]);
            }
        }

        if (reportItems.length > 0) {
            panel.addToViewer();
            panel.clear();
            for (var i = 0; i < reportItems.length; i++) {
                var button = jsObject.ReportsPanelButton(reportItems[i]);
                if (restUrl) button.image.src = restUrl + "service/smallthumbnail/" + reportItems[i].Key;
                reportsContainer.appendChild(button);

                button.action = function () {
                    if (this.isSelected) return;
                    var itemObject = this.itemObject;

                    if (itemObject.Ident == "ReportSnapshotItem") {
                        panel.loadReportItemToViewer(itemObject);
                    }
                    else {
                        jsObject.SendCloudCommand("ReportGetReportInfo", { ReportTemplateItemKey: itemObject.Key }, function (data) {
                            if (!data.ResultContainsDashboards) {
                                panel.loadReportItemToViewer(itemObject);
                            }
                            else {
                                panel.loadDashboardToDbsViewer(itemObject);
                            }
                        });
                    }
                }
            }
        }
        panel.selectButtonByItemKey(currItemKey);
    });

    panel.selectButtonByItemKey = function (itemKey) {
        if (panel.selectedButton) panel.selectedButton.setSelected(false);
        for (var i = 0; i < reportsContainer.childNodes.length; i++) {
            if (reportsContainer.childNodes[i].itemObject && reportsContainer.childNodes[i].itemObject.Key == itemKey) {
                reportsContainer.childNodes[i].setSelected(true);
                panel.selectedButton = reportsContainer.childNodes[i];
            }
        }
    }

    panel.resize = function (minimize, noAnimation) {
        var mainPanel = jsObject.controls.mainPanel;
        if (minimize == null) minimize = !panel.isMinimize;

        var finishWidth = minimize ? 44 : 170;
        var durationAnimation = noAnimation ? 0 : 150;
        panel.isMinimize = minimize;
        jsObject.SetCookie("StimulsoftWebDemoMainMenuMinimize", minimize ? "true" : "false");

        if (noAnimation) {
            panel.style.width = finishWidth + "px";
            mainPanel.style.marginLeft = panel.offsetWidth + "px";
            mainPanel.style.width = "calc(100% - " + panel.offsetWidth + "px)";
        }
        else {
            resizeImg.className = "stiJsViewerFolderReportsPanelItemImageRotate";
            panel.style.width = finishWidth + "px";
            mainPanel.style.marginLeft = panel.offsetWidth + "px";
            mainPanel.style.width = "calc(100% - " + panel.offsetWidth + "px)";

            clearInterval(panel.timer);
            panel.timer = setTimeout(function () {
                resizeImg.className = "";
                jsObject.postAction("GetPages");
            }, durationAnimation);
        }

        var finishOpacity = minimize ? 0 : 1;
        reportsContainer.style.opacity = finishOpacity;
        logoText.style.opacity = finishOpacity;
    }

    return panel;
}

StiJsViewer.prototype.ReportsPanelButton = function (itemObject) {
    var button = this.CreateHTMLTable();
    button.itemObject = itemObject;
    button.className = "stiJsViewerReportButton";

    var image = document.createElement("img");
    image.className = "stiJsViewerReportButtonImage";
    button.addCell(image).style.textAlign = "center";
    button.image = image;

    var caption = document.createElement("div");
    caption.className = "stiJsViewerReportButtonCaption";
    button.addCellInNextRow(caption).style.textAlign = "center";
    caption.innerHTML = itemObject.Name;

    button.setSelected = function (state) {
        this.isSelected = state;
        this.className = state ? "stiJsViewerReportButton stiJsViewerReportButtonSelected" : "stiJsViewerReportButton";
    }

    button.onclick = function () {
        this.action();
    }

    button.action = function () { };

    return button;
}

StiJsViewer.prototype.ReportsPanelSeparator = function () {
    var sep = document.createElement("div");
    sep.className = "stiJsViewerFolderReportsPanelSeparator";

    return sep;
}