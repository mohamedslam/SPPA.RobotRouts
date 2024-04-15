
StiMobileDesigner.prototype.InitializeReportSetupForm_ = function () {

    var reportSetupForm = this.BaseForm("reportSetupForm", this.loc.FormReportSetup.title, 1, this.HelpLinks["reportSetup"]);
    reportSetupForm.mode = "Main";

    //Main Table
    var mainTable = this.CreateHTMLTable();
    mainTable.className = "stiDesignerImageFormMainPanel";
    reportSetupForm.container.appendChild(mainTable);
    reportSetupForm.container.style.padding = "0px";

    //Buttons
    var buttonProps = [
        ["Main", "ReportOptionsForm.ReportOptionMain.png", this.loc.FormOptions.Main],
        ["Description", "ReportOptionsForm.ReportOptionDescription.png", this.loc.PropertyMain.Description]
    ];

    //Add Panels && Buttons
    var buttonsPanel = mainTable.addCell();
    var panelsContainer = mainTable.addCell();
    buttonsPanel.style.verticalAlign = "top";
    buttonsPanel.style.paddingTop = "6px";
    reportSetupForm.mainButtons = {};
    reportSetupForm.panels = {};

    for (var i = 0; i < buttonProps.length; i++) {
        var panel = document.createElement("Div");
        panel.className = "stiDesignerEditFormPanel";
        if (i != 0) panel.style.display = "none";
        panelsContainer.appendChild(panel);
        reportSetupForm.panels[buttonProps[i][0]] = panel;

        var button = this.FormTabPanelButton(null, buttonProps[i][2], buttonProps[i][1], buttonProps[i][2], null, { width: 24, height: 24 }, 34);
        reportSetupForm.mainButtons[buttonProps[i][0]] = button;
        buttonsPanel.appendChild(button);
        button.panelName = buttonProps[i][0];
        button.action = function () {
            reportSetupForm.setMode(this.panelName);
        }
    }

    if (this.options.cloudMode) {
        reportSetupForm.mainButtons.Description.style.display = "none";
    }

    var mainTable = this.CreateHTMLTable();
    var descriptionTable = this.CreateHTMLTable();
    reportSetupForm.panels.Main.appendChild(mainTable);
    reportSetupForm.panels.Description.appendChild(descriptionTable);
    mainTable.style.width = "100%";
    descriptionTable.style.width = "100%";

    reportSetupForm.controls = {};
    var controlProps = [
        ["groupMainParameters", null, this.FormBlockHeader(this.loc.FormReportSetup.groupMainParameters), "0 0 6px 0", mainTable],
        ["cacheAllData", null, this.CheckBox(null, this.loc.PropertyMain.CacheAllData), "7px 6px 7px 15px", mainTable],
        ["convertNulls", null, this.CheckBox(null, this.loc.PropertyMain.ConvertNulls), "7px 6px 7px 15px", mainTable],
        ["numberOfPass", this.loc.PropertyMain.NumberOfPass, this.DropDownList("reportSetupFormNumberOfPass", 180, null, this.GetNumberOfPassItems(), true), "6px 6px 6px 15px", mainTable],
        ["reportCacheMode", this.loc.PropertyMain.ReportCacheMode, this.DropDownList("reportSetupFormReportCacheMode", 180, null, this.GetReportCacheModeItems(), true), "6px 6px 6px 15px", mainTable],
        ["groupScript", null, this.FormBlockHeader(this.loc.FormReportSetup.groupScript), "6px 0 6px 0", mainTable],
        ["scriptCSharp", null, this.RadioButton("reportSetupFormCSharp", "reportSetupFormScriptLanguage", "C#"), "6px 6px 6px 15px", mainTable],
        ["scriptVB", null, this.RadioButton("reportSetupFormVB", "reportSetupFormScriptLanguage", "VB.Net"), "6px 6px 6px 15px", mainTable],
        ["groupUnits", null, this.FormBlockHeader(this.loc.FormReportSetup.groupUnits), "6px 0 6px 0", mainTable],
        ["units_cm", null, this.RadioButton("reportSetupFormUnits_cm", "reportSetupFormUnits", this.loc.PropertyEnum.StiReportUnitTypeCentimeters), "6px 6px 6px 15px", mainTable],
        ["units_mm", null, this.RadioButton("reportSetupFormUnits_mm", "reportSetupFormUnits", this.loc.PropertyEnum.StiReportUnitTypeMillimeters), "6px 6px 6px 15px", mainTable],
        ["units_in", null, this.RadioButton("reportSetupFormUnits_in", "reportSetupFormUnits", this.loc.PropertyEnum.StiReportUnitTypeInches), "6px 6px 6px 15px", mainTable],
        ["units_hi", null, this.RadioButton("reportSetupFormUnits_hi", "reportSetupFormUnits", this.loc.PropertyEnum.StiReportUnitTypeHundredthsOfInch), "6px 6px 6px 15px", mainTable],
        ["groupNames", null, this.FormBlockHeader(this.loc.FormReportSetup.groupNames), "0 0 6px 0", descriptionTable],
        ["reportName", this.loc.PropertyMain.ReportName, this.TextBox(null, 250), "6px 6px 6px 15px", descriptionTable],
        ["reportAlias", this.loc.PropertyMain.ReportAlias, this.TextBox(null, 250), "6px 6px 6px 15px", descriptionTable],
        ["reportAuthor", this.loc.PropertyMain.ReportAuthor, this.TextBox(null, 250), "6px 6px 6px 15px", descriptionTable],
        ["groupDescription", null, this.FormBlockHeader(this.loc.FormReportSetup.groupDescription), "6px 0 6px 0", descriptionTable],
        ["reportDescription", null, this.TextArea(null, 415, 80), "6px 6px 6px 15px", descriptionTable],
        ["groupDates", null, this.FormBlockHeader(this.loc.FormReportSetup.groupDates), "6px 0 10px 0", descriptionTable],
        ["reportCreated", this.loc.FormReportSetup.ReportCreated.replace(":", ""), this.SimpleTextContainer(), "10px 6px 10px 15px", descriptionTable],
        ["reportChanged", this.loc.FormReportSetup.ReportChanged.replace(":", ""), this.SimpleTextContainer(), "10px 6px 10px 15px", descriptionTable]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        var name = controlProps[i][0];
        var caption = controlProps[i][1];
        var control = controlProps[i][2];
        if (controlProps[i][3]) control.style.margin = controlProps[i][3];
        var table = controlProps[i][4];
        reportSetupForm.controls[name] = control;
        if (i != 0) reportSetupForm.controls[name + "Row"] = table.addRow();
        if (caption) {
            var textCell = table.addTextCellInLastRow(caption);
            textCell.className = "stiDesignerCaptionControls";
            textCell.style.padding = "0 15px 0 15px";
        }
        var controlCell = table.addCellInLastRow(control);
        if (!caption) controlCell.setAttribute("colspan", "2");
    }

    if (this.options.jsMode) {
        reportSetupForm.controls.groupScript.style.display = "none";
        reportSetupForm.controls.scriptCSharp.style.display = "none";
        reportSetupForm.controls.scriptVB.style.display = "none";
        reportSetupForm.controls.reportCacheModeRow.style.display = "none";
    }

    //Form Methods
    reportSetupForm.setMode = function (mode) {
        reportSetupForm.mode = mode;
        for (var panelName in reportSetupForm.panels) {
            reportSetupForm.panels[panelName].style.display = mode == panelName ? "" : "none";
            reportSetupForm.mainButtons[panelName].setSelected(mode == panelName);
        }
    }

    reportSetupForm.show = function () {
        var report = this.jsObject.options.report;
        if (!report) return;

        reportSetupForm.changeVisibleState(true);
        reportSetupForm.setMode("Main");
        reportSetupForm.controls.cacheAllData.setChecked(report.properties.cacheAllData);
        reportSetupForm.controls.convertNulls.setChecked(report.properties.convertNulls);
        reportSetupForm.controls.numberOfPass.setKey(report.properties.numberOfPass);
        reportSetupForm.controls.reportCacheMode.setKey(report.properties.reportCacheMode);

        reportSetupForm.controls.reportCreated.innerText = report.properties.reportCreated;
        reportSetupForm.controls.reportChanged.innerText = report.properties.reportChanged;

        var scriptControl = reportSetupForm.controls["script" + report.properties.scriptLanguage];
        if (scriptControl) scriptControl.setChecked(true);

        var unitControl = reportSetupForm.controls["units_" + report.properties.reportUnit];
        if (unitControl) unitControl.setChecked(true);

        var descriptionProperties = ["reportName", "reportAlias", "reportAuthor", "reportDescription"];
        for (var i = 0; i < descriptionProperties.length; i++) {
            reportSetupForm.controls[descriptionProperties[i]].value = StiBase64.decode(report.properties[descriptionProperties[i]].replace("Base64Code;", ""));
        }
    }

    reportSetupForm.action = function () {
        this.changeVisibleState(false);
        var report = this.jsObject.options.report;
        if (!report) return;
        var descriptionProperties = ["reportName", "reportAlias", "reportAuthor", "reportDescription"];
        for (var i = 0; i < descriptionProperties.length; i++) {
            report.properties[descriptionProperties[i]] = "Base64Code;" + StiBase64.encode(reportSetupForm.controls[descriptionProperties[i]].value);
        }
        report.properties.cacheAllData = reportSetupForm.controls.cacheAllData.isChecked;
        report.properties.convertNulls = reportSetupForm.controls.convertNulls.isChecked;
        report.properties.numberOfPass = reportSetupForm.controls.numberOfPass.key;
        report.properties.reportCacheMode = reportSetupForm.controls.reportCacheMode.key;
        report.properties.scriptLanguage = reportSetupForm.controls.scriptCSharp.isChecked ? "CSharp" : "VB";
        this.jsObject.SendCommandSetReportProperties(["reportName", "reportAlias", "reportAuthor", "reportDescription", "cacheAllData",
            "convertNulls", "numberOfPass", "reportCacheMode", "scriptLanguage"]);

        var unitValue = "cm";
        if (reportSetupForm.controls.units_mm.isChecked) unitValue = "mm"
        else if (reportSetupForm.controls.units_hi.isChecked) unitValue = "hi"
        else if (reportSetupForm.controls.units_in.isChecked) unitValue = "in";
        if (unitValue != report.properties.reportUnit) this.jsObject.SendCommandChangeUnit(unitValue);
    }

    return reportSetupForm;
}