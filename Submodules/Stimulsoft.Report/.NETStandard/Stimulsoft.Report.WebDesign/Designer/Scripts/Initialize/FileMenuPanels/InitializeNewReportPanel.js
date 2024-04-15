
StiMobileDesigner.prototype.InitializeNewReportPanel = function () {
    var newReportPanel = document.createElement("div");
    var jsObject = newReportPanel.jsObject = this;
    this.options.newReportPanel = newReportPanel;
    this.options.mainPanel.appendChild(newReportPanel);
    newReportPanel.style.display = "none";
    newReportPanel.style.overflow = "auto";
    newReportPanel.className = "stiDesignerNewReportPanel";

    var header = this.FileMenuPanelHeader(this.loc.MainMenu.menuFileNew.replace("&", ""));
    newReportPanel.appendChild(header);

    var buttonsTable = this.CreateHTMLTable();
    buttonsTable.style.margin = "0 16px 0 16px";
    newReportPanel.appendChild(buttonsTable);

    if (this.options.showFileMenuNewReport !== false) {
        var blankReportButton = this.NewReportPanelButton("blankReportButton", this.loc.Wizards.BlankReport, "Empty16.png", { width: 144, height: 203 }, true);
        buttonsTable.addCell(blankReportButton);

        if (this.options.isDebugMode) {
            var blankFormButton = this.NewReportPanelButton("blankFormButton", this.loc.Wizards.BlankForm, "Empty16.png", { width: 160, height: 190 }, true);
            buttonsTable.addCell(blankFormButton);
        }

        buttonsTable.addCell(this.NewReportPanelButton("standartReportButton", this.loc.Wizards.StandardReport, "WizardReports.StandartReport.png", null, true));
        buttonsTable.addCell(this.NewReportPanelButton("masterDetailReportButton", this.loc.Wizards.MasterDetailReport, "WizardReports.MasterDetailReport.png", null, true));

        if (!this.options.jsMode) {
            buttonsTable.addCell(this.NewReportPanelButton("labelReportButton", "Label", "WizardReports.LabelReport.png", null, true));
            buttonsTable.addCell(this.NewReportPanelButton("invoiceReportButton", "Invoice", "WizardReports.Invoice.png", null, true));
            buttonsTable.addCell(this.NewReportPanelButton("orderReportButton", "Order", "WizardReports.Order.png", null, true));
            buttonsTable.addCell(this.NewReportPanelButton("quotationReportButton", "Quotation", "WizardReports.Quotation.png", null, true));
        }
    }

    if (this.options.dashboardAssemblyLoaded && this.options.showFileMenuNewDashboard !== false) {
        var dbsButtonsPanel = document.createElement("div");
        dbsButtonsPanel.style.margin = "40px 16px 16px 15px";
        newReportPanel.appendChild(dbsButtonsPanel);

        var blankDbsButton = this.WizardDashboardButton("blankDashboardButton", this.loc.Wizards.BlankDashboard, "Empty16.png");
        dbsButtonsPanel.appendChild(blankDbsButton);

        var wizardDashboards = ["Financial", "Orders", "SalesOverview", "TicketsStatistics", "TrafficAnalytics", "VehicleProduction", "WebsiteAnalytics"];
        for (var i = 0; i < wizardDashboards.length; i++) {
            var wizardButton = this.WizardDashboardButton("dashboard" + wizardDashboards[i] + "Button", wizardDashboards[i], "WizardDashboards." + wizardDashboards[i] + "Dashboard.png", wizardDashboards[i]);
            dbsButtonsPanel.appendChild(wizardButton);
        }
    }

    newReportPanel.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
    }

    return newReportPanel;
}

StiMobileDesigner.prototype.NewReportPanelButton = function (name, caption, image, imageSizes, showImageBorder) {
    var button = this.BigButton(name, null, caption, image, caption, null, "stiDesignerNewReportButton", true, null, imageSizes || { width: 144, height: 203 });
    button.style.marginRight = "30px";

    if (button.image && showImageBorder) {
        button.image.style.border = "1px solid #808080";
        button.image.style.background = "#ffffff";
    }

    if (name == "invoiceReportButton" || name == "orderReportButton" || name == "quotationReportButton") {
        this.OverrideWizardButtonAction(button);
    }

    return button;
}

StiMobileDesigner.prototype.OverrideWizardButtonAction = function (button) {
    var jsObject = this;

    button.action = function () {
        var items = jsObject.GetOfflineStoreItems();
        var itemObject = null;
        for (var i = 0; i < items.length; i++) {
            if (items[i].wizard && items[i].wizard.action == this.name) {
                itemObject = items[i];
                break;
            }
        }
        if (itemObject) {
            jsObject.InitializeOfflineStoreItemsForm(function (form) {
                form.show(itemObject);
            });
        }
        else {
            jsObject.ExecuteAction(this.name);
        }
    }
}

StiMobileDesigner.prototype.WizardDashboardButton = function (name, caption, image, dashboardName) {
    var button = this.NewReportPanelButton(name, caption, image, { width: 216, height: 120 }, true);
    button.style.width = "250px";
    button.style.height = "165px";
    button.style.marginBottom = "12px";
    button.style.display = "inline-block";
    button.dashboardName = dashboardName;

    return button;
}