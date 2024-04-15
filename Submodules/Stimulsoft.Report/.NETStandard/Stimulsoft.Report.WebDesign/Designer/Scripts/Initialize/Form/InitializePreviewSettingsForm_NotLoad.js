
StiMobileDesigner.prototype.InitializePreviewSettingsForm_ = function () {
    var form = this.BaseFormPanel("previewSettingsForm", this.loc.PropertyMain.PreviewSettings, 1);
    form.mode = "Report";
    form.controls = {};
    var jsObject = this;

    //Main Table
    var mainTable = this.CreateHTMLTable();
    mainTable.className = "stiDesignerImageFormMainPanel";
    form.container.appendChild(mainTable);
    form.container.style.padding = "0px";

    //Buttons
    var buttonProps = [
        ["Report", "PreviewSettingsForm.StiPage.png", this.loc.Components.StiReport],
        ["Dashboard", "PreviewSettingsForm.StiDashboard.png", this.loc.Components.StiDashboard]
    ];

    //Add Panels && Buttons
    var buttonsPanel = mainTable.addCell();
    var panelsContainer = mainTable.addCell();
    buttonsPanel.style.verticalAlign = "top";
    buttonsPanel.style.paddingTop = "6px";
    form.mainButtons = {};
    form.panels = {};

    for (var i = 0; i < buttonProps.length; i++) {
        var panel = document.createElement("Div");
        panel.className = "stiDesignerEditFormPanel";
        panel.style.width = "auto";
        panel.style.minWidth = "450px";
        if (i != 0) panel.style.display = "none";
        panelsContainer.appendChild(panel);
        form.panels[buttonProps[i][0]] = panel;

        var button = this.FormTabPanelButton(null, buttonProps[i][2], buttonProps[i][1], buttonProps[i][2], null, { width: 24, height: 24 }, 34);
        form.mainButtons[buttonProps[i][0]] = button;
        buttonsPanel.appendChild(button);

        if (this.options.isJava && i > 2) {
            button.style.display = 'none';
        }

        button.panelName = buttonProps[i][0];
        button.action = function () {
            form.setMode(this.panelName);
        }
    }

    //Report
    var reportTable = this.CreateHTMLTable();
    form.panels.Report.appendChild(reportTable);

    var reportChecks = [
        ["reportPrint", this.loc.A_WebViewer.PrintReport],
        ["reportOpen", this.loc.Buttons.Open],
        ["reportSave", this.loc.A_WebViewer.SaveReport],
        ["reportSendEMail", this.loc.FormViewer.SendEMail.replace("...", "")],
        ["reportPageControl", this.loc.FormViewer.PageControl],
        ["reportEditor", this.loc.FormViewer.Editor],
        ["reportFind", this.loc.FormViewer.Find],
        ["reportSignature", this.loc.Components.StiSignature],
        ["reportPageViewMode", this.loc.FormViewer.ViewMode],
        ["reportStatusBar", this.loc.FormViewer.StatusBar],
        ["reportBookmarks", this.loc.FormViewer.Bookmarks],
        ["reportParameters", this.loc.FormViewer.Parameters],
        ["reportResources", this.loc.PropertyMain.Resources],
        ["reportZoom", this.loc.PropertyMain.Zoom],
        ["reportToolbar", this.loc.FormViewer.Toolbar]
    ];

    var innerTable = this.CreateHTMLTable();
    for (var i = 0; i < reportChecks.length; i++) {
        if (["reportPrint", "reportPageViewMode"].indexOf(reportChecks[i][0]) >= 0) {
            innerTable = this.CreateHTMLTable();
            reportTable.addCell(innerTable).style.verticalAlign = "top";
        }
        var control = this.CheckBox("null", reportChecks[i][1]);
        control.style.margin = "20px 20px 0 20px";
        form.controls[reportChecks[i][0]] = control;
        innerTable.addCellInNextRow(control);
    }

    //Additional controls
    var sep = this.FormSeparator();
    sep.style.margin = "12px 0 12px 0";
    form.panels.Report.appendChild(sep);

    var innerTable2 = this.CreateHTMLTable();
    form.panels.Report.appendChild(innerTable2);

    var previewModeControl = this.DropDownList(null, 150, null, this.GetHtmlPreviewModeItems(), true);
    innerTable2.addTextCell(this.loc.PropertyMain.HtmlPreviewMode).className = "stiDesignerCaptionControlsBigIntervals";
    innerTable2.addCell(previewModeControl).className = "stiDesignerControlCellsBigIntervals2";

    var repToolbarAlignControl = this.DropDownList(null, 150, null, this.GetHorizontalAlignmentItems(true), true);
    var repToolbarReverseControl = this.CheckBox(null, this.loc.Buttons.Reverse);
    repToolbarReverseControl.style.margin = "0 2px 0 2px";

    innerTable2.addTextCellInNextRow(this.loc.Gui.cust_tab_toolbar_alignment).className = "stiDesignerCaptionControlsBigIntervals";
    innerTable2.addCellInLastRow(repToolbarAlignControl).className = "stiDesignerControlCellsBigIntervals2";
    innerTable2.addCellInLastRow(repToolbarReverseControl);

    //Dashboard
    var dbsTable = this.CreateHTMLTable();
    form.panels.Dashboard.appendChild(dbsTable);

    var dbsChecks = [
        ["dashboardToolBar", this.loc.Buttons.ShowSpecific.replace("{0}", this.loc.FormViewer.Toolbar)],
        ["dashboardRefreshButton", this.loc.Buttons.ShowSpecific.replace("{0}", this.loc.PropertyMain.Refresh)],        
        ["dashboardOpenButton", this.loc.Buttons.ShowSpecific.replace("{0}", this.loc.Buttons.Open)],
        ["dashboardEditButton", this.loc.Buttons.ShowSpecific.replace("{0}", this.loc.MainMenu.menuEditEdit)],
        ["dashboardResetAllFiltersButton", this.loc.Buttons.ShowSpecific.replace("{0}", this.loc.PropertyMain.ResetAllFilters)],
        ["dashboardParametersButton", this.loc.Buttons.ShowSpecific.replace("{0}", this.loc.PropertyMain.Parameters)],
        ["dashboardFullScreenButton", this.loc.Buttons.ShowSpecific.replace("{0}", this.loc.FormViewer.FullScreen)],
        ["dashboardMenuButton", this.loc.Buttons.ShowSpecific.replace("{0}", this.loc.Buttons.Save)],
        ["dashboardShowReportSnapshots", this.loc.Buttons.ShowSpecific.replace("{0}", this.loc.Permissions.ItemReportSnapshots)],
        ["dashboardShowExports", this.loc.Buttons.ShowSpecific.replace("{0}", this.loc.Services.categoryExport)]
    ];

    for (var i = 0; i < dbsChecks.length; i++) {
        var control = this.CheckBox("null", dbsChecks[i][1]);
        control.style.margin = "20px 20px 0 20px";
        form.controls[dbsChecks[i][0]] = control;
        dbsTable.addCellInNextRow(control);
    }

    //Additional controls
    var sep2 = this.FormSeparator();
    sep2.style.margin = "12px 0 12px 0";
    form.panels.Dashboard.appendChild(sep2);

    var innerTable3 = this.CreateHTMLTable();
    form.panels.Dashboard.appendChild(innerTable3);

    var dbsToolbarAlignControl = this.DropDownList(null, 150, null, this.GetHorizontalAlignmentItems(true), true);
    var dbsToolbarReverseControl = this.CheckBox(null, this.loc.Buttons.Reverse);
    dbsToolbarReverseControl.style.margin = "0 2px 0 2px";

    innerTable3.addTextCellInNextRow(this.loc.Gui.cust_tab_toolbar_alignment).className = "stiDesignerCaptionControlsBigIntervals";
    innerTable3.addCellInLastRow(dbsToolbarAlignControl).className = "stiDesignerControlCellsBigIntervals2";
    innerTable3.addCellInLastRow(dbsToolbarReverseControl);
    
    form.controls.dashboardMenuButton.style.display = "none"; //hide save option

    if (this.options.jsMode) {
        form.controls.reportSignature.style.display = "none";
    }

    form.setMode = function (mode) {
        form.mode = mode;
        for (var panelName in form.panels) {
            form.panels[panelName].style.display = mode == panelName ? "" : "none";
            form.mainButtons[panelName].setSelected(mode == panelName);
        }
    }

    form.show = function () {
        form.changeVisibleState(true);
        form.setMode("Report");
        form.mainButtons.Dashboard.style.display = this.jsObject.options.dashboardAssemblyLoaded ? "" : "none";

        if (jsObject.options.report) {
            var previewSettings = jsObject.options.report.properties.previewSettings;
            for (var i = 0; i < reportChecks.length; i++)
                form.controls[reportChecks[i][0]].setChecked(previewSettings[reportChecks[i][0]]);
            for (var i = 0; i < dbsChecks.length; i++)
                form.controls[dbsChecks[i][0]].setChecked(previewSettings[dbsChecks[i][0]]);

            previewModeControl.setKey(previewSettings.htmlPreviewMode);
            repToolbarAlignControl.setKey(previewSettings.reportToolbarHorAlignment);
            repToolbarReverseControl.setChecked(previewSettings.reportToolbarReverse);
            dbsToolbarAlignControl.setKey(previewSettings.dashboardToolbarHorAlignment);
            dbsToolbarReverseControl.setChecked(previewSettings.dashboardToolbarReverse);
        }
    }

    form.action = function () {
        this.changeVisibleState(false);
        var previewSettings = {};

        for (var i = 0; i < reportChecks.length; i++)
            previewSettings[reportChecks[i][0]] = form.controls[reportChecks[i][0]].isChecked;
        for (var i = 0; i < dbsChecks.length; i++)
            previewSettings[dbsChecks[i][0]] = form.controls[dbsChecks[i][0]].isChecked;

        previewSettings.htmlPreviewMode = previewModeControl.key;
        previewSettings.reportToolbarHorAlignment = repToolbarAlignControl.key;
        previewSettings.reportToolbarReverse = repToolbarReverseControl.isChecked;
        previewSettings.dashboardToolbarHorAlignment = dbsToolbarAlignControl.key;
        previewSettings.dashboardToolbarReverse = dbsToolbarReverseControl.isChecked;

        jsObject.SendCommandSetPreviewSettings(previewSettings);
    }

    return form;
}