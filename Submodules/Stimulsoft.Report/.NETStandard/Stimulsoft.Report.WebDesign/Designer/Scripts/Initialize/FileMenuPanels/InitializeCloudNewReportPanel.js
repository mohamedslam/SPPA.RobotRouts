
StiMobileDesigner.prototype.InitializeCloudNewReportPanel = function () {
    var newReportPanel = document.createElement("div");
    var jsObject = newReportPanel.jsObject = this;
    this.options.newReportPanel = newReportPanel;
    this.options.mainPanel.appendChild(newReportPanel);
    newReportPanel.style.display = "none";
    newReportPanel.style.overflow = "auto";
    newReportPanel.className = "stiDesignerNewReportPanel";
    newReportPanel.visibleMode = "all";
    jsObject.AddProgressToControl(newReportPanel);

    var header = this.FileMenuPanelHeader(this.loc.MainMenu.menuFileNew.replace("&", ""));
    newReportPanel.appendChild(header);

    if (jsObject.options.standaloneJsMode) {
        var buttonClose = newReportPanel.buttonClose = this.StandartSmallButton(null, null, null, "CloseForm.png", null, null, null);
        buttonClose.image.style.margin = "0 2px 0 2px";
        buttonClose.style.position = "absolute";
        buttonClose.style.top = buttonClose.style.right = "6px";
        buttonClose.allwaysEnabled = true;
        buttonClose.style.webkitAppRegion = "none";

        header.style.position = "relative";
        header.appendChild(buttonClose);

        buttonClose.action = function () {
            if (jsObject.options.toolBar.closeButton) {
                jsObject.options.toolBar.closeButton.action();
            }
        }
    }

    var buttonsPanel = newReportPanel.buttonsPanel = document.createElement("div");
    buttonsPanel.style.margin = "0 20px 0 20px";
    newReportPanel.appendChild(buttonsPanel);

    var buttonsTable = this.CreateHTMLTable();
    buttonsTable.style.display = "inline-block";
    buttonsPanel.appendChild(buttonsTable);

    if (this.options.showFileMenuNewReport !== false) {
        buttonsTable.addCell(this.NewReportCloudPanelButton("blankReportButton_Cloud", this.loc.Wizards.BlankReport, "Empty16.png", { width: 144, height: 203 }, null, null, true)).style.verticalAlign = "bottom";
    }

    if (this.options.dashboardAssemblyLoaded && this.options.showFileMenuNewDashboard !== false) {
        buttonsTable.addCell(this.NewReportCloudPanelButton("blankDashboardButton_Cloud", this.loc.Wizards.BlankDashboard, "Empty16.png", { width: 216, height: 120 }, null, null, true)).style.verticalAlign = "bottom";
    }

    if (this.options.cloudMode) {
        buttonsTable.addCell(this.NewReportCloudPanelButton("blankFormButton_Cloud", this.loc.Wizards.BlankForm, "Empty16.png", { width: 160, height: 190 }, null, null, true)).style.verticalAlign = "bottom";
    }

    var dataWizard = this.NewReportCloudPanelButton("dataWizard", this.loc.Wizards.GetData, "DataWizard.png", { width: 174, height: 174 }, null, null, true);
    buttonsTable.addCell(dataWizard).style.verticalAlign = "bottom";

    dataWizard.action = function () {
        jsObject.InitializeGetDataForm(function (form) {
            var params = {
                zoom: jsObject.options.report ? jsObject.options.report.zoom.toString() : "1",
                designerOptions: jsObject.GetCookie("StimulsoftMobileDesignerOptions")
            };
            jsObject.SendCommandToDesignerServer("PrepareReportBeforeGetData", params, function (answer) {
                form.oldReportContent = answer.oldReportContent;
                if (jsObject.options.cloudParameters) {
                    form.oldReportTemplateItemKey = jsObject.options.cloudParameters.reportTemplateItemKey;
                    jsObject.options.cloudParameters.reportTemplateItemKey = null
                }
                jsObject.CloseReport();
                jsObject.options.reportGuid = answer.reportGuid;
                jsObject.LoadReport(jsObject.ParseReport(answer.reportObject));
                form.changeVisibleState(true);
            });
        });
    }

    var sep = this.FormSeparator();
    sep.style.margin = "20px";
    sep.style.borderTopStyle = "solid";
    newReportPanel.appendChild(sep);

    var table = this.CreateHTMLTable();
    table.style.marginLeft = "20px";
    newReportPanel.appendChild(table);

    var backButton = this.SmallButton(null, null, this.loc.Report.Office2010Back, "Arrows.ArrowLeftBlue.png", null, null, "stiDesignerHyperlinkButton", true);
    backButton.style.display = "none";
    backButton.style.margin = "0 10px 0 10px";
    backButton.style.height = "28px";
    table.addCell(backButton);

    var findTable = this.CreateHTMLTable();
    findTable.className = "stiDesignerTextBox stiDesignerTextBoxDefault";
    table.addCell(findTable);

    var findTextbox = this.TextBox(null, 400);
    findTable.addCell(findTextbox);
    findTextbox.style.height = "28px";
    findTextbox.style.border = "0";
    findTextbox.setAttribute("placeholder", this.loc.Cloud.SearchForOnlineTemplates);

    var findButton = this.StandartSmallButton(null, null, null, "View.png", this.loc.FormViewer.Find);
    findButton.innerTable.style.width = "100%";
    findButton.style.width = findButton.style.height = "28px";
    findTable.addCell(findButton);

    findButton.action = function () {
        findTextbox.actionOnKeyEnter();
    }

    newReportPanel.modeButtons = {};

    var modeButtons = [
        ["all", this.loc.Report.RangeAll],
        ["reports", this.loc.Cloud.TextReports],
        ["dashboards", this.loc.Permissions.ItemDashboards]
    ]

    for (var i = 0; i < modeButtons.length; i++) {
        var mode = modeButtons[i][0];
        var button = newReportPanel.modeButtons[mode] = this.SmallButton("newPanel" + mode, "cloudNewPanelTypeButtons", modeButtons[i][1]);
        button.mode = mode;
        button.style.marginLeft = mode == "all" ? "12px" : "6px";
        table.addCell(button);

        button.action = function () {
            newReportPanel.setVisibleMode(this.mode);
        }
    }

    var samplesTable = this.CreateHTMLTable();
    samplesTable.style.margin = "8px 20px 20px 20px";
    newReportPanel.appendChild(samplesTable);
    var samleText = samplesTable.addTextCell(this.loc.MainMenu.menuHelpSamples + ":");
    samleText.className = "stiDesignerTextContainer";
    samleText.style.color = "#808080";

    newReportPanel.samplesButtons = {};
    var samples = this.options.helpLanguage == "ru" ? ["Шаблон", "Аналитика", "Бланк", "Диаграмма", "Бизнес"] : ["Invoice", "Order", "Analytics", "Chart", "Business"];

    for (var i = 0; i < samples.length; i++) {
        var button = this.SmallButton(null, null, samples[i], null, null, null, "stiDesignerHyperlinkButton", true);
        button.style.color = "#808080";
        button.sampleText = samples[i];
        newReportPanel.samplesButtons[samples[i]] = button;
        samplesTable.addCell(button);
        button.style.marginLeft = "5px";
        button.action = function () {
            findTextbox.value = this.sampleText;
            findTextbox.actionOnKeyEnter();
        }
    }

    var wizardsPanel = document.createElement("div");
    wizardsPanel.style.marginLeft = "20px";
    newReportPanel.appendChild(wizardsPanel);

    var wizardReports = [];
    if (this.options.standaloneJsMode) {
        wizardReports.push(["standartReportButton", this.loc.Wizards.StandardReport, "WizardReports.StandartReport.png"]);
    }

    wizardReports.push(["masterDetailReportButton", this.options.standaloneJsMode ? this.loc.Wizards.MasterDetailReport : this.loc.FormTitles.ReportWizard, "WizardReports.MasterDetailReport.png"]);

    if (!this.options.standaloneJsMode) {
        wizardReports.push(["labelReportButton", "Label", "WizardReports.LabelReport.png"]);
        wizardReports.push(["invoiceReportButton", "Invoice", "WizardReports.Invoice.png"]);
        wizardReports.push(["orderReportButton", "Order", "WizardReports.Order.png"]);
        wizardReports.push(["quotationReportButton", "Quotation", "WizardReports.Quotation.png"]);
    }

    for (var i = 0; i < wizardReports.length; i++) {
        var repButton = this.NewReportCloudPanelButton(wizardReports[i][0], wizardReports[i][1], wizardReports[i][2], { width: 144, height: 203 }, null, true, true);
        repButton.reportName = wizardReports[i][1];
        repButton.newReportPanel = newReportPanel;
        wizardsPanel.appendChild(repButton);
    }

    if (this.options.dashboardAssemblyLoaded) {
        var wizardDashboards = ["TicketsStatistics", "WebsiteAnalytics", "Financial", "TrafficAnalytics", "VehicleProduction", "SalesOverview"];
        for (var i = 0; i < wizardDashboards.length; i++) {
            var wizardButton = this.NewReportCloudPanelButton("dashboard" + wizardDashboards[i] + "Button", wizardDashboards[i], "WizardDashboards." + wizardDashboards[i] + "Dashboard.png", { width: 216, height: 120 }, null, true, true);
            wizardButton.dashboardName = wizardDashboards[i];
            wizardButton.newReportPanel = newReportPanel;
            wizardsPanel.appendChild(wizardButton);
        }
    }

    wizardsPanel.updateVisibleMode = function () {
        for (var i = 0; i < wizardsPanel.childNodes.length; i++) {
            var button = wizardsPanel.childNodes[i];
            button.style.display = newReportPanel.visibleMode == "all" || (button.reportName && newReportPanel.visibleMode == "reports") || (button.dashboardName && newReportPanel.visibleMode == "dashboards") ? "inline-block" : "none";
        }
    }
    
    //Results Panel
    var resultsPanel = document.createElement("div");
    resultsPanel.className = "stiDesignerDictionaryItemsContainer";
    resultsPanel.style.display = "none";
    resultsPanel.style.top = "170px";
    resultsPanel.style.padding = "20px";
    newReportPanel.resultsPanel = resultsPanel;
    newReportPanel.appendChild(resultsPanel);

    resultsPanel.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
    }

    resultsPanel.checkEmpty = function () {
        if (this.childNodes.length == 0) {
            var emptyText = document.createElement("div");
            emptyText.className = "stiCreateDataHintText";
            emptyText.style.fontSize = "14px";
            emptyText.style.width = "300px";
            emptyText.style.position = "absolute";
            emptyText.style.left = "calc(50% - 150px)";
            emptyText.style.top = "50%";
            emptyText.innerHTML = jsObject.loc.Cloud.WeDidntFindAnything;
            resultsPanel.appendChild(emptyText);
        }
    }

    resultsPanel.show = function () {
        resultsPanel.style.display = "";
        backButton.style.display = "inline-block";
        buttonsPanel.style.display = "none";
        sep.style.display = "none";
        samplesTable.style.display = "none";
        wizardsPanel.style.display = "none";
        resultsPanel.clear();

        var openReportFromCloudItem = function (itemObject) {
            var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
            fileMenu.changeVisibleState(false);

            setTimeout(function () {
                var openPanel = jsObject.options.openPanel || jsObject.InitializeOpenPanel();
                openPanel.openReportFromCloudItem(itemObject, true);
            }, 200);
        }

        //Get from offline store
        var items = jsObject.GetOfflineStoreItems();
        var filteredItems = [];
        for (var i = 0; i < items.length; i++) {
            var isDashboard = items[i].key.toLowerCase().indexOf("dashboard") >= 0;

            if ((isDashboard && newReportPanel.visibleMode == "dashboards") || (!isDashboard && newReportPanel.visibleMode == "reports")) {
                if (items[i].key.toLowerCase().indexOf(findTextbox.value.toLowerCase()) >= 0 || items[i].text.toLowerCase().indexOf(findTextbox.value.toLowerCase()) >= 0) {
                    filteredItems.push(items[i]);
                }
                else {
                    for (var k = 0; k < items[i].tags.length; k++) {
                        if (items[i].tags[k].toLowerCase().indexOf(findTextbox.value.toLowerCase()) >= 0) {
                            filteredItems.push(items[i]);
                            break;
                        }
                    }
                }
            }
        }

        for (var i = 0; i < filteredItems.length; i++) {
            var button = jsObject.NewReportCloudPanelButton(null, filteredItems[i].text, filteredItems[i].image.name, filteredItems[i].image.size, true, null, true);
            button.itemObject = filteredItems[i];
            resultsPanel.appendChild(button);

            button.action = function () {
                var itemObject = this.itemObject;
                if (itemObject.action) {
                    jsObject.ExecuteAction(itemObject.action);
                }
                else if (itemObject.wizard && itemObject.resourceName) {
                    jsObject.InitializeOfflineStoreItemsForm(function (form) {
                        form.show(itemObject);
                    });
                }
            }
        }

        //Get from online store
        if (findTextbox.value.length > 2) {
            newReportPanel.progress.show();

            var requestTimer = setTimeout(function () {
                newReportPanel.progress.hide();
            }, jsObject.options.requestTimeout * 1000);

            jsObject.SendCloudCommand("ReportFindPublic", { SearchString: findTextbox.value, Count: 30 },
                function (data) {
                    clearTimeout(requestTimer);
                    newReportPanel.progress.hide();
                    var items = data.ResultItems;

                    if (items) {
                        for (var i = 0; i < items.length; i++) {
                            var mode = newReportPanel.visibleMode;

                            if (mode == "all" || (items[i].ContentType == "Report" && mode == "reports") || (items[i].ContentType == "Dashboard" && mode == "dashboards")) {
                                items[i].isOnlineStoreItem = true;

                                var button = jsObject.NewReportCloudPanelThumbButton(items[i]);
                                resultsPanel.appendChild(button);
                                button.getThumbnail();

                                button.action = function () {
                                    openReportFromCloudItem(this.itemObject);
                                }    
                            }
                        }
                        resultsPanel.checkEmpty();
                    }
                },
                function (data) {
                    clearTimeout(requestTimer);
                    newReportPanel.progress.hide();
                }
            );
        }
        else {
            resultsPanel.checkEmpty();
        }
    }

    resultsPanel.hide = function () {
        resultsPanel.style.display = "none";
        backButton.style.display = "none";
        buttonsPanel.style.display = "";
        sep.style.display = "";
        samplesTable.style.display = "";
        wizardsPanel.style.display = "";
        findTextbox.value = "";
        findTextbox.focus();
    }

    backButton.action = function () {
        resultsPanel.hide();
    }

    findTextbox.actionOnKeyEnter = function () {
        resultsPanel.show();
    }

    newReportPanel.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        if (state) findTextbox.focus();
    }

    newReportPanel.setVisibleMode = function (mode) {
        this.visibleMode = mode;
        this.modeButtons[mode].setSelected(true);
        wizardsPanel.updateVisibleMode();

        if (resultsPanel.style.display == "") {
            resultsPanel.show();
        }
    }

    newReportPanel.loadPinnedFromCookies = function () {
        var pinnedStr = jsObject.GetCookie("StimulsoftMobileDesignerNewPanelPinned");
        if (pinnedStr) {
            var names = JSON.parse(pinnedStr);
            for (var name in names) {
                var button = jsObject.options.buttons[name];
                if (button && button.pinImg) {
                    button.pinImg.action();
                }
            }
        }
    }

    newReportPanel.setVisibleMode("all");
    newReportPanel.loadPinnedFromCookies();

    return newReportPanel;
}

StiMobileDesigner.prototype.NewReportCloudPanelButton = function (name, caption, image, imageSizes, fixWidth, showPin, showImageBorder) {
    var jsObject = this;
    var button = this.BigButton(name, null, caption, image, null, null, "stiDesignerNewReportButton", true, null, imageSizes);
    button.style.width = button.style.height = "auto";
    button.style.display = "inline-block";
    button.captionText = caption;
    button.imageSizes = imageSizes;
    button.fixWidth = fixWidth;
    button.showPin = showPin;
    button.showImageBorder = showImageBorder;

    if (fixWidth) {
        button.style.minWidth = "230px";
    }
    else {
        button.style.marginRight = "30px";
    }

    if (button.cellImage) {
        button.cellImage.style.verticalAlign = "bottom";
        button.cellImage.style.padding = fixWidth ? "12px 0 0 0" : "12px 12px 0 12px";

        if (showImageBorder) {
            button.image.style.border = "1px solid #808080";
            button.image.style.background = "#ffffff";
        }
    }

    if (button.caption) {
        button.caption.style.padding = "2px 3px 4px 2px";
        button.caption.style.height = "25px";
        button.caption.style.maxWidth = (imageSizes ? imageSizes.width + 10 : 155) + "px";
    }

    if (name == "invoiceReportButton" || name == "orderReportButton" || name == "quotationReportButton") {
        jsObject.OverrideWizardButtonAction(button);
    }

    if (showPin) {
        var pinImg = button.pinImg = document.createElement("img");
        pinImg.style.position = "absolute";
        pinImg.style.right = pinImg.style.bottom = "10px";
        pinImg.style.width = pinImg.style.height = "16px";
        pinImg.style.display = "none";
        StiMobileDesigner.setImageSource(pinImg, jsObject.options, "Pin.Pin.png");

        button.appendChild(pinImg);
        button.style.position = "relative";
        button.isPinned = false;

        pinImg.action = function () {
            button.setPinned(!button.isPinned);
            button.pinClicked = true;

            if (button.isClone) {
                button.removeFromPinnedPanel();
                button.originalButton.setPinned(false);
            }
            else {
                if (button.isPinned) {
                    button.addToPinnedPanel();
                }
                else {
                    button.cloneButton.removeFromPinnedPanel();
                }
            }
        }

        pinImg.onclick = function () {
            this.action();
        }

        button.addToCookies = function () {
            var pinnedStr = jsObject.GetCookie("StimulsoftMobileDesignerNewPanelPinned");
            var names = pinnedStr ? JSON.parse(pinnedStr) : {};
            names[this.name] = true;
            jsObject.SetCookie("StimulsoftMobileDesignerNewPanelPinned", JSON.stringify(names));
        }

        button.removeFromCookies = function () {
            var pinnedStr = jsObject.GetCookie("StimulsoftMobileDesignerNewPanelPinned");
            if (pinnedStr) {
                var names = JSON.parse(pinnedStr);
                delete names[this.name];
                jsObject.SetCookie("StimulsoftMobileDesignerNewPanelPinned", JSON.stringify(names));
            }
        }

        button.onclick = function () {
            if (this.pinClicked || this.isTouchEndFlag || !this.isEnabled || jsObject.options.isTouchClick) return;
            this.action();
            this.pinClicked = false;
        }

        button.addToPinnedPanel = function () {
            var cloneButton = this.getClone();
            this.newReportPanel.buttonsPanel.appendChild(cloneButton);
            this.addToCookies();
        }

        button.removeFromPinnedPanel = function () {
            this.parentElement.removeChild(this);
            this.removeFromCookies();
        }

        button.setPinned = function (state) {
            this.isPinned = state;
            StiMobileDesigner.setImageSource(pinImg, jsObject.options, "Pin." + (state ? "Pinned" : "Pin") + ".png");
            pinImg.style.display = state ? "" : (this.isOver ? "" : "none");
        }

        button.getClone = function () {
            var cloneB = jsObject.NewReportCloudPanelButton(null, this.captionText, this.imageName, this.imageSizes, this.fixWidth, this.showPin, this.showImageBorder);
            cloneB.name = this.name;
            cloneB.reportName = this.reportName;
            cloneB.dashboardName = this.dashboardName;
            cloneB.newReportPanel = this.newReportPanel;
            cloneB.isClone = true;
            cloneB.originalButton = this;
            cloneB.setPinned(this.isPinned);
            this.cloneButton = cloneB;

            if (this.name == "invoiceReportButton" || this.name == "orderReportButton" || this.name == "quotationReportButton") {
                jsObject.OverrideWizardButtonAction(cloneB);
            }

            return cloneB;
        }

        button.onmouseenter_ = button.onmouseenter;
        button.onmouseleave_ = button.onmouseleave;
                
        button.onmouseenter = function () {
            this.onmouseenter_();
            pinImg.style.display = "";
        }

        button.onmouseleave = function () {
            this.onmouseleave_();

            if (!this.isPinned) {
                pinImg.style.display = "none";
            }
        }
    }

    return button;
}

StiMobileDesigner.prototype.NewReportCloudPanelThumbButton = function (itemObject) {
    var button = this.NewReportCloudPanelButton(null, itemObject.Name, true, null, true);
    button.itemObject = itemObject;

    var imageContainer = document.createElement("div");
    imageContainer.style.border = "1px solid #808080";
    imageContainer.style.overflow = "hidden";
    imageContainer.style.display = "inline-block";
    imageContainer.style.position = "relative";
    imageContainer.style.lineHeight = "0";
    imageContainer.style.minWidth = imageContainer.style.minHeight = "150px";
    imageContainer.appendChild(button.image);
    button.cellImage.appendChild(imageContainer);
    this.AddProgressToControl(imageContainer);
    imageContainer.progress.style.opacity = "0.7";

    button.image.style.display = "none";
    button.image.style.margin = "-1px";
    button.image.style.maxWidth = button.image.style.maxHeight = "201px";
    button.image.style.width = button.image.style.height = "auto";

    button.getThumbnail = function () {
        imageContainer.progress.show();
        var options = this.jsObject.options;
        var restUrl = options.cloudParameters && options.cloudParameters.restUrl ? options.cloudParameters.restUrl : options.restUrl;
        if (restUrl) {
            button.image.src = restUrl + "service/smallthumbnail/" + itemObject.Key;
        }
    }

    button.image.onload = function () {
        button.image.style.display = "";
        imageContainer.style.minWidth = imageContainer.style.minHeight = "auto";
        imageContainer.progress.hide();
    }

    button.image.onerror = function () {
        button.image.style.display = "none";
        imageContainer.style.minWidth = imageContainer.style.minHeight = "150px";
        imageContainer.progress.hide();
    }

    return button;
}

StiMobileDesigner.prototype.NewReportCloudPanelWizardButton = function (caption, imageName) {
    var button = this.SmallButton(null, null, caption, imageName, null, null, "stiDesignerFormButtonTheme", true);
    button.style.display = "inline-block";

    return button;
}

StiMobileDesigner.prototype.GetOfflineStoreItems = function () {
    var offlineItems = [
        {
            key: "StandardReport",
            action: "standartReportButton",
            tags: ["simple", "standard", "plain"],
            text: this.loc.Wizards.StandardReport,
            image: { name: "WizardReports.StandartReport.png", size: { width: 144, height: 203 } }
        },
        {
            key: "MasterDetailReport",
            action: "masterDetailReportButton",
            tags: ["master", "detail"],
            text: this.loc.Wizards.MasterDetailReport,
            image: { name: "WizardReports.MasterDetailReport.png", size: { width: 144, height: 203 } }
        }
    ];

    if (!this.options.standaloneJsMode) {
        offlineItems = offlineItems.concat([
            {
                key: "LabelReport",
                action: "labelReportButton",
                tags: ["label", "labels"],
                text: "Label",
                image: { name: "WizardReports.LabelReport.png", size: { width: 144, height: 203 } }
            },
            {
                key: "Invoice",
                wizard: { action: "invoiceReportButton", templateName: "Invoice" },
                resourceName: "Invoice.Invoice",
                tags: ["invoice", "sales", "plain", "billing statement", "business", "recurring"],
                text: "Invoice",
                image: { name: "WizardReports.Invoice.png", size: { width: 144, height: 203 } },
                description: "The sample demonstrates how to create the invoice with bank details"
            },
            {
                key: "SalesInvoice",
                wizard: { action: "invoiceReportButton", templateName: "Sales Invoice" },
                resourceName: "Invoice.SalesInvoice",
                tags: ["invoice", "sales", "plain", "billing statement", "business", "recurring"],
                text: "Sales Invoice",
                image: { name: "WizardReports.SalesInvoice.png", size: { width: 144, height: 203 } },
                description: "The sample demonstrates how to create the sales invoice"
            },
            {
                key: "ServiceInvoice",
                wizard: { action: "invoiceReportButton", templateName: "Service Invoice" },
                resourceName: "Invoice.ServiceInvoice",
                tags: ["invoice", "sales", "plain", "billing statement", "business", "recurring"],
                text: "Service Invoice",
                image: { name: "WizardReports.ServiceInvoice.png", size: { width: 144, height: 203 } },
                description: "The sample demonstrates how to create the service invoice with the hours and rate"
            },
            {
                key: "BillingStatement",
                wizard: { action: "invoiceReportButton", templateName: "Billing Statement" },
                resourceName: "Invoice.BillingStatement",
                tags: ["invoice", "sales", "plain", "billing statement", "business", "recurring"],
                text: "Billing Statement",
                image: { name: "WizardReports.BillingStatement.png", size: { width: 144, height: 203 } },
                description: "The sample demonstrates how to create the billing statement"
            },
            {
                key: "BusinessInvoice",
                wizard: { action: "invoiceReportButton", templateName: "Business Invoice" },
                resourceName: "Invoice.BusinessInvoice",
                tags: ["invoice", "sales", "plain", "billing statement", "business", "recurring"],
                text: "Business Invoice",
                image: { name: "WizardReports.BusinessInvoice.png", size: { width: 144, height: 203 } },
                description: "The sample demonstrates how to create business invoice"
            },
            {
                key: "RecurringInvoice",
                wizard: { action: "invoiceReportButton", templateName: "Recurring Invoice" },
                resourceName: "Invoice.RecurringInvoice",
                tags: ["invoice", "sales", "plain", "billing statement", "business", "recurring"],
                text: "Recurring Invoice",
                image: { name: "WizardReports.RecurringInvoice.png", size: { width: 144, height: 203 } },
                description: "The sample demonstrates how to create the recurring invoice"
            },
            {
                key: "Order",
                wizard: { action: "orderReportButton", templateName: "Order" },
                resourceName: "Order.Order",
                tags: ["order", "purchase"],
                text: "Order",
                image: { name: "WizardReports.Order.png", size: { width: 144, height: 203 } },
                description: "The sample demonstrates how to create Order"
            },
            {
                key: "PurchaseOrder",
                wizard: { action: "orderReportButton", templateName: "Purchase Order" },
                resourceName: "Order.PurchaseOrder",
                tags: ["order", "purchase"],
                text: "Purchase Order",
                image: { name: "WizardReports.PurchaseOrder.png", size: { width: 144, height: 203 } },
                description: "The sample demonstrates how to create Purchase Order"
            },
            {
                key: "Quotation",
                wizard: { action: "quotationReportButton" },
                resourceName: "Quotation.Quotation",
                tags: ["quotation"],
                text: "Quotation",
                image: { name: "WizardReports.Quotation.png", size: { width: 144, height: 203 } },
                description: "The sample demonstrates how to create Quotation"
            },
            {
                key: "FinancialDashboard",
                action: "dashboardFinancialButton",
                tags: ["financial"],
                text: "Financial",
                image: { name: "WizardDashboards.FinancialDashboard.png", size: { width: 216, height: 120 } }
            },
            {
                key: "OrdersDashboard",
                action: "dashboardOrdersButton",
                tags: ["orders"],
                text: "Orders",
                image: { name: "WizardDashboards.OrdersDashboard.png", size: { width: 216, height: 120 } }
            },
            {
                key: "SalesOverviewDashboard",
                action: "dashboardSalesOverviewButton",
                tags: ["sale", "sales", "selling"],
                text: "Sales Overview",
                image: { name: "WizardDashboards.SalesOverviewDashboard.png", size: { width: 216, height: 120 } }
            },
            {
                key: "TicketsStatisticsDashboard",
                action: "dashboardTicketsStatisticsButton",
                tags: ["tickets", "ticket"],
                text: "Tickets Statistics",
                image: { name: "WizardDashboards.TicketsStatisticsDashboard.png", size: { width: 216, height: 120 } }
            },
            {
                key: "TrafficAnalyticsDashboard",
                action: "dashboardTrafficAnalyticsButton",
                tags: ["traffic"],
                text: "Traffic Analytics",
                image: { name: "WizardDashboards.TrafficAnalyticsDashboard.png", size: { width: 216, height: 120 } }
            },
            {
                key: "VehicleProductionDashboard",
                action: "dashboardVehicleProductionButton",
                tags: ["vehicle", "country"],
                text: "Vehicle Production",
                image: { name: "WizardDashboards.VehicleProductionDashboard.png", size: { width: 216, height: 120 } }
            },
            {
                key: "WebsiteAnalyticsDashboard",
                action: "dashboardWebsiteAnalyticsButton",
                tags: ["website", "analytics", "site", "www"],
                text: "Website Analytics",
                image: { name: "WizardDashboards.WebsiteAnalyticsDashboard.png", size: { width: 216, height: 120 } }
            }
        ]);
    }

    return offlineItems;
}