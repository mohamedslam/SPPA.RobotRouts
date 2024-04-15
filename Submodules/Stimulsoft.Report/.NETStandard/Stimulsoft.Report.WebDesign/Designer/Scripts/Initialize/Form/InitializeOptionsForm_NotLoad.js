
StiMobileDesigner.prototype.InitializeOptionsForm_ = function () {
    var jsObject = this;
    var optionsForm = this.BaseForm("optionsForm", this.loc.FormOptions.title, 1, this.HelpLinks["options"]);
    optionsForm.mode = "Main";

    var restoreDefaults = this.FormButton(null, null, this.loc.Buttons.RestoreDefaults, null);
    restoreDefaults.style.display = "inline-block";
    restoreDefaults.style.margin = "12px";
    optionsForm.restoreDefaults = restoreDefaults;

    restoreDefaults.action = function () {
        var msgForm = jsObject.MessageFormRestoreDefaults();
        msgForm.changeVisibleState(true);
        msgForm.action = function (state) {
            if (state) optionsForm.fill(jsObject.options.defaultDesignerOptions);
        }
    }

    var footerTable = this.CreateHTMLTable();
    footerTable.style.width = "100%";
    var buttonsPanel = optionsForm.buttonsPanel;
    optionsForm.removeChild(buttonsPanel);
    optionsForm.appendChild(footerTable);
    footerTable.addCell(restoreDefaults).style.width = "1px";
    footerTable.addCell();
    footerTable.addCell(optionsForm.buttonOk).style.width = "1px";
    footerTable.addCell(optionsForm.buttonCancel).style.width = "1px";

    //Main Table
    var mainTable = this.CreateHTMLTable();
    mainTable.className = "stiDesignerImageFormMainPanel";
    optionsForm.container.appendChild(mainTable);
    optionsForm.container.style.padding = "0px";

    //Buttons
    var buttonProps = [
        ["Main", "OptionsForm.DesignerOptionMain.png", this.loc.FormOptions.Main],
        ["Design", "OptionsForm.DesignerOptionDesign.png", this.loc.Buttons.Design],
        ["Grid", "OptionsForm.DesignerOptionGrid.png", this.loc.FormOptions.Grid],
        ["QuickInfo", "OptionsForm.DesignerOptionQuickInfo.png", this.loc.MainMenu.menuViewQuickInfo],
        ["AutoSave", "OptionsForm.DesignerOptionAutoSave.png", this.loc.DesignerFx.Saving]
    ];

    //Add Panels && Buttons
    var buttonsPanel = mainTable.addCell();
    var panelsContainer = mainTable.addCell();
    buttonsPanel.style.verticalAlign = "top";
    buttonsPanel.style.paddingTop = "6px";
    optionsForm.mainButtons = {};
    optionsForm.panels = {};

    for (var i = 0; i < buttonProps.length; i++) {
        var panel = document.createElement("div");
        panel.className = "stiDesignerEditFormPanel";
        if (i != 0) panel.style.display = "none";
        panelsContainer.appendChild(panel);
        optionsForm.panels[buttonProps[i][0]] = panel;

        var button = this.FormTabPanelButton(null, buttonProps[i][2], buttonProps[i][1], buttonProps[i][2], null, { width: 24, height: 24 }, 34);
        optionsForm.mainButtons[buttonProps[i][0]] = button;
        buttonsPanel.appendChild(button);
        button.panelName = buttonProps[i][0];

        button.action = function () {
            optionsForm.setMode(this.panelName);
        }
    }

    //Create Tables
    for (var name in optionsForm.panels) {
        var table = this.CreateHTMLTable();
        optionsForm.panels[name].appendChild(table);
        optionsForm.panels[name].innerTable = table;
        table.style.width = "100%";
    }

    optionsForm.controls = {};
    var controlProps = [
        ["groupMainOptions", null, this.FormBlockHeader(this.loc.FormOptions.groupMainOptions), "0 0 6px 0", optionsForm.panels.Main.innerTable],
        ["runDesignerAfterInsert", null, this.CheckBox(null, this.loc.FormOptions.EditAfterInsert), "7px 7px 7px 15px", optionsForm.panels.Main.innerTable],
        ["useLastFormat", null, this.CheckBox(null, this.loc.FormOptions.UseLastFormat), "7px 7px 7px 15px", optionsForm.panels.Main.innerTable],
        ["generateLocalizedName", null, this.CheckBox(null, this.loc.FormOptions.GenerateLocalizedName), "7px 7px 15px 15px", optionsForm.panels.Main.innerTable],
        ["chartEditorType", this.loc.Chart.ChartEditorForm, this.DropDownList("optionsFormChartEditorType", 180, null, this.GetChartEditorTypeItems(), true), "6px 6px 6px 15px", optionsForm.panels.Main.innerTable],
        ["defaultScriptMode", this.loc.FormOptions.DefaultScriptMode, this.DropDownList("optionsFormDefaultScriptMode", 180, null, this.GetDefaultScriptModeItems(), true), "6px 6px 6px 15px", optionsForm.panels.Main.innerTable],
        ["startScreen", this.loc.FormOptions.StartScreen, this.DropDownList("optionsFormStartScreen", 180, null, this.GetStartScreenItems(), true), "6px 6px 6px 15px", optionsForm.panels.Main.innerTable],
        ["newReportDictionary", this.loc.Wizards.title, this.DropDownList("optionsFormNewReportDictionary", 180, null, this.GetNewReportDictionaryItems(), true), "6px 6px 6px 15px", optionsForm.panels.Main.innerTable],

        ["groupToolbox", null, this.FormBlockHeader(this.loc.Toolbox.title), "0 0 6px 0", optionsForm.panels.Design.innerTable],
        ["showToolbox", null, this.CheckBox(null, this.loc.MainMenu.menuViewShowToolbox), "7px 7px 7px 15px", optionsForm.panels.Design.innerTable],
        ["showInsertTab", null, this.CheckBox(null, this.loc.MainMenu.menuViewShowInsertTab), "7px 7px 7px 15px", optionsForm.panels.Design.innerTable],

        ["groupViewOptions", null, this.FormBlockHeader(this.loc.Toolbars.ToolbarViewOptions), "6px 0 6px 0", optionsForm.panels.Design.innerTable],
        ["showHeaders", null, this.CheckBox(null, this.loc.MainMenu.menuViewShowHeaders), "7px 7px 7px 15px", optionsForm.panels.Design.innerTable],
        ["showRulers", null, this.CheckBox(null, this.loc.MainMenu.menuViewShowRulers), "7px 7px 7px 15px", optionsForm.panels.Design.innerTable],
        ["showOrder", null, this.CheckBox(null, this.loc.MainMenu.menuViewShowOrder), "7px 7px 7px 15px", optionsForm.panels.Design.innerTable],
        ["showDimensionLines", null, this.CheckBox(null, this.loc.FormOptions.ShowDimensionLines), "7px 7px 7px 15px", optionsForm.panels.Design.innerTable],

        ["groupGridOptions", null, this.FormBlockHeader(this.loc.FormOptions.groupGridOptions), "0 0 6px 0", optionsForm.panels.Grid.innerTable],
        ["alignToGrid", null, this.CheckBox(null, this.loc.MainMenu.menuViewAlignToGrid), "7px 7px 7px 15px", optionsForm.panels.Grid.innerTable],
        ["showGrid", null, this.CheckBox(null, this.loc.MainMenu.menuViewShowGrid), "7px 7px 7px 15px", optionsForm.panels.Grid.innerTable],

        ["groupGridDrawingOptions", null, this.FormBlockHeader(this.loc.FormOptions.groupGridDrawingOptions), "6px 0 6px 0", optionsForm.panels.Grid.innerTable],
        ["gridModeLines", null, this.RadioButton("optionsForm_Lines", "optionsFormGrid", this.loc.FormOptions.GridLines), "6px 6px 6px 15px", optionsForm.panels.Grid.innerTable],
        ["gridModeDots", null, this.RadioButton("optionsForm_Dots", "optionsFormGrid", this.loc.FormOptions.GridDots), "6px 6px 6px 15px", optionsForm.panels.Grid.innerTable],

        ["groupGridSize", null, this.FormBlockHeader(this.loc.FormOptions.groupGridSize), "6px 0 6px 0", optionsForm.panels.Grid.innerTable],
        ["gridSizeInch", this.loc.PropertyEnum.StiReportUnitTypeInches, this.TextBox(null, 100), "6px 6px 6px 15px", optionsForm.panels.Grid.innerTable],
        ["gridSizeHundredthsOfInch", this.loc.PropertyEnum.StiReportUnitTypeHundredthsOfInch, this.TextBox(null, 100), "6px 6px 6px 15px", optionsForm.panels.Grid.innerTable],
        ["gridSizeCentimetres", this.loc.PropertyEnum.StiReportUnitTypeCentimeters, this.TextBox(null, 100), "6px 6px 6px 15px", optionsForm.panels.Grid.innerTable],
        ["gridSizeMillimeters", this.loc.PropertyEnum.StiReportUnitTypeMillimeters, this.TextBox(null, 100), "6px 6px 6px 15px", optionsForm.panels.Grid.innerTable],
        ["gridSizePixels", this.loc.PropertyEnum.StiReportUnitTypePixels, this.TextBox(null, 100), "7px 7px 7px 15px", optionsForm.panels.Grid.innerTable],

        ["groupOptionsOfQuickInfo", null, this.FormBlockHeader(this.loc.FormOptions.groupOptionsOfQuickInfo), "0 0 6px 0", optionsForm.panels.QuickInfo.innerTable],
        ["quickInfoTypeNone", null, this.RadioButton("optionsForm_QuickInfoNone", "optionsFormOptionsOfQuickInfo", this.loc.MainMenu.menuViewQuickInfoNone), "7px 7px 7px 15px", optionsForm.panels.QuickInfo.innerTable],
        ["quickInfoTypeShowComponentsNames", null, this.RadioButton("optionsForm_QuickInfoShowComponentsNames", "optionsFormOptionsOfQuickInfo", this.loc.MainMenu.menuViewQuickInfoShowComponentsNames), "7px 7px 7px 15px", optionsForm.panels.QuickInfo.innerTable],
        ["quickInfoTypeShowContent", null, this.RadioButton("optionsForm_QuickInfoShowContent", "optionsFormOptionsOfQuickInfo", this.loc.MainMenu.menuViewQuickInfoShowContent), "7px 7px 7px 15px", optionsForm.panels.QuickInfo.innerTable],
        ["quickInfoTypeShowFields", null, this.RadioButton("optionsForm_QuickInfoShowFields", "optionsFormOptionsOfQuickInfo", this.loc.MainMenu.menuViewQuickInfoShowFields), "7px 7px 7px 15px", optionsForm.panels.QuickInfo.innerTable],
        ["quickInfoTypeShowFieldsOnly", null, this.RadioButton("optionsForm_QuickInfoShowFieldsOnly", "optionsFormOptionsOfQuickInfo", this.loc.MainMenu.menuViewQuickInfoShowFieldsOnly), "7px 7px 7px 15px", optionsForm.panels.QuickInfo.innerTable],
        ["quickInfoTypeShowEvents", null, this.RadioButton("optionsForm_QuickInfoShowEvents", "optionsFormOptionsOfQuickInfo", this.loc.MainMenu.menuViewQuickInfoShowEvents), "7px 7px 7px 15px", optionsForm.panels.QuickInfo.innerTable],
        ["separator1", null, this.FormSeparator(), "6px 0 6px 0", optionsForm.panels.QuickInfo.innerTable],
        ["quickInfoOverlay", null, this.CheckBox(null, this.loc.MainMenu.menuViewQuickInfoOverlay), "7px 7px 7px 15px", optionsForm.panels.QuickInfo.innerTable],

        ["groupAutoSaveOptions", null, this.FormBlockHeader(this.loc.FormOptions.groupAutoSaveOptions), "0 0 6px 0", optionsForm.panels.AutoSave.innerTable],
        ["enableAutoSaveMode", null, this.CheckBox(null, this.loc.FormOptions.EnableAutoSaveMode), "7px 7px 7px 15px", optionsForm.panels.AutoSave.innerTable],
        ["autoSaveInterval", this.loc.FormOptions.SaveReportEvery.replace(":", ""), this.DropDownList("reportSetupFormSaveReportEvery", 180, null, this.GetAutoSaveItems(), true), "6px 6px 6px 15px", optionsForm.panels.AutoSave.innerTable],
        ["groupSaveOthers", null, this.FormBlockHeader(this.loc.FormDesigner.Others), "6px 0 6px 0", optionsForm.panels.AutoSave.innerTable],
        ["requestChangesWhenSaving", null, this.CheckBox(null, this.loc.Cloud.RequestChangesWhenSavingToCloud), "7px 7px 7px 15px", optionsForm.panels.AutoSave.innerTable]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        var name = controlProps[i][0];
        var caption = controlProps[i][1];
        var control = controlProps[i][2];
        if (controlProps[i][3]) control.style.margin = controlProps[i][3];
        var table = controlProps[i][4];
        optionsForm.controls[name] = control;
        if (i != 0) optionsForm.controls[name + "Row"] = table.addRow();
        if (caption) {
            var textCell = table.addTextCellInLastRow(caption);
            textCell.className = "stiDesignerCaptionControls";
            textCell.style.padding = "0 15px 0 15px";
        }
        var controlCell = control.controlCell = table.addCellInLastRow(control);
        if (!caption) controlCell.setAttribute("colspan", "2");
    }

    //Hide not use properties
    optionsForm.controls.showOrder.style.display = "none";
    optionsForm.controls.showRulersRow.style.display = "none";
    optionsForm.controls.showDimensionLinesRow.style.display = "none";
    optionsForm.mainButtons.QuickInfo.style.display = "none";

    optionsForm.controls.gridSizeInch.controlCell.appendChild(this.SimpleTextContainer("in"));
    optionsForm.controls.gridSizeHundredthsOfInch.controlCell.appendChild(this.SimpleTextContainer("hi"));
    optionsForm.controls.gridSizeCentimetres.controlCell.appendChild(this.SimpleTextContainer("cm"));
    optionsForm.controls.gridSizeMillimeters.controlCell.appendChild(this.SimpleTextContainer("mm"));
    optionsForm.controls.gridSizePixels.controlCell.appendChild(this.SimpleTextContainer("px"));

    if (!this.options.cloudMode && !this.options.serverMode && !this.options.standaloneJsMode) {
        optionsForm.controls.groupSaveOthersRow.style.display = "none";
        optionsForm.controls.requestChangesWhenSavingRow.style.display = "none";
    }

    if (this.options.designerSpecification != "Developer") {
        optionsForm.controls.runDesignerAfterInsertRow.style.display = "none";
        optionsForm.controls.useLastFormatRow.style.display = "none";
        optionsForm.controls.generateLocalizedNameRow.style.display = "none";
    }

    if (this.options.designerSpecification == "Beginner") {
        optionsForm.controls.chartEditorTypeRow.style.display = "none";
    }

    if (this.options.serverMode) {
        optionsForm.controls.startScreenRow.style.display = "none";
    }

    optionsForm.controls.enableAutoSaveMode.action = function () {
        optionsForm.controls.autoSaveInterval.setEnabled(this.isChecked);
    }

    optionsForm.controls.showToolbox.action = function () {
        if (!optionsForm.controls.showInsertTab.isChecked && !this.isChecked)
            optionsForm.controls.showInsertTab.setChecked(true);
    }

    optionsForm.controls.showInsertTab.action = function () {
        if (!optionsForm.controls.showToolbox.isChecked && !this.isChecked)
            optionsForm.controls.showToolbox.setChecked(true);
    }

    //Form Methods
    optionsForm.setMode = function (mode) {
        optionsForm.mode = mode;
        for (var panelName in optionsForm.panels) {
            optionsForm.panels[panelName].style.display = mode == panelName ? "" : "none";
            optionsForm.mainButtons[panelName].setSelected(mode == panelName);
        }
    }

    optionsForm.fill = function (designerOptions) {
        var boolProps = ["showHeaders", "showRulers", "showOrder", "runDesignerAfterInsert", "useLastFormat", "showDimensionLines", "generateLocalizedName",
            "alignToGrid", "showGrid", "quickInfoOverlay", "enableAutoSaveMode"];
        for (var i = 0; i < boolProps.length; i++) {
            this.controls[boolProps[i]].setChecked(designerOptions[boolProps[i]]);
        }
        this.controls.autoSaveInterval.setEnabled(designerOptions.enableAutoSaveMode);
        if (this.controls["gridMode" + designerOptions.gridMode]) this.controls["gridMode" + designerOptions.gridMode].setChecked(true);

        var textProps = ["gridSizeInch", "gridSizeHundredthsOfInch", "gridSizeCentimetres", "gridSizeMillimeters", "gridSizePixels"];
        for (var i = 0; i < textProps.length; i++) {
            this.controls[textProps[i]].value = designerOptions[textProps[i]];
        }
        if (this.controls["quickInfoType" + designerOptions.quickInfoType]) this.controls["quickInfoType" + designerOptions.quickInfoType].setChecked(true);
        this.controls.autoSaveInterval.setKey(designerOptions.autoSaveInterval);
        this.controls.showToolbox.setChecked(jsObject.options.showToolbox);
        this.controls.showInsertTab.setChecked(jsObject.options.showInsertTab);
        this.controls.requestChangesWhenSaving.setChecked(jsObject.options.requestChangesWhenSaving);
        this.controls.newReportDictionary.setKey(jsObject.options.newReportDictionary);
        this.controls.chartEditorType.setKey(jsObject.options.chartEditorType);
        this.controls.defaultScriptMode.setKey(jsObject.options.defaultScriptMode);

        if (!jsObject.options.serverMode) {
            this.controls.startScreen.setKey(jsObject.options.jsMode ? jsObject.options.startScreen : designerOptions.startScreen);
        }
    }

    optionsForm.show = function () {
        optionsForm.setMode("Main");
        var designerOptions = jsObject.options.report ? jsObject.options.report.info : jsObject.options.defaultDesignerOptions;
        optionsForm.fill(designerOptions);

        this.changeVisibleState(true);
    }

    optionsForm.action = function () {
        var designerOptions = {};
        var boolProps = ["showHeaders", "showRulers", "showOrder", "runDesignerAfterInsert", "useLastFormat", "showDimensionLines", "generateLocalizedName",
            "alignToGrid", "showGrid", "quickInfoOverlay", "enableAutoSaveMode"];
        for (var i = 0; i < boolProps.length; i++) {
            designerOptions[boolProps[i]] = this.controls[boolProps[i]].isChecked;
        }
        var textProps = ["gridSizeInch", "gridSizeHundredthsOfInch", "gridSizeCentimetres", "gridSizeMillimeters", "gridSizePixels"];
        for (var i = 0; i < textProps.length; i++) {
            designerOptions[textProps[i]] = this.controls[textProps[i]].value;
        }
        designerOptions.gridMode = this.controls.gridModeLines.isChecked ? "Lines" : "Dots";

        var quickInfoTypes = ["None", "ShowComponentsNames", "ShowContent", "ShowFields", "ShowFieldsOnly", "ShowEvents"];
        for (var i = 0; i < quickInfoTypes.length; i++) {
            if (this.controls["quickInfoType" + quickInfoTypes[i]] && this.controls["quickInfoType" + quickInfoTypes[i]].isChecked) {
                designerOptions.quickInfoType = quickInfoTypes[i];
                break;
            }
        }
        designerOptions.autoSaveInterval = this.controls.autoSaveInterval.key;
        jsObject.options.showInsertTab = optionsForm.controls.showInsertTab.isChecked;
        jsObject.options.showToolbox = optionsForm.controls.showToolbox.isChecked;
        jsObject.options.requestChangesWhenSaving = optionsForm.controls.requestChangesWhenSaving.isChecked;

        if (!jsObject.options.serverMode) {
            jsObject.options.defaultDesignerOptions.startScreen = designerOptions.startScreen = this.controls.startScreen.key;
        }

        if (jsObject.options.buttons.insertToolButton) {
            jsObject.options.buttons.insertToolButton.parentElement.style.display = (!jsObject.options.showInsertButton ? false : jsObject.options.showInsertTab) ? "" : "none";
            if (jsObject.options.workPanel.currentPanel == jsObject.options.insertPanel && !jsObject.options.showInsertTab.isChecked) {
                jsObject.options.workPanel.showPanel(jsObject.options.homePanel);
                jsObject.options.buttons.homeToolButton.setSelected(true);
            }
        }

        jsObject.options.toolbox.changeVisibleState(jsObject.options.showToolbox);
        if (optionsForm.controls.showToolbox.isChecked) jsObject.options.toolbox.update();

        jsObject.SetCookie("StimulsoftMobileDesignerSetupToolbox", JSON.stringify({
            showToolbox: jsObject.options.showToolbox,
            showInsertTab: jsObject.options.showInsertTab
        }));

        jsObject.SetCookie("StimulsoftMobileDesignerRequestChangesWhenSaving", jsObject.options.requestChangesWhenSaving ? "true" : "false");

        jsObject.options.newReportDictionary = optionsForm.controls.newReportDictionary.key;
        jsObject.SetCookie("StimulsoftMobileDesignerNewReportDictionary", jsObject.options.newReportDictionary);

        jsObject.options.chartEditorType = optionsForm.controls.chartEditorType.key;
        jsObject.SetCookie("StimulsoftMobileDesignerChartEditorType", jsObject.options.chartEditorType);

        jsObject.options.defaultScriptMode = optionsForm.controls.defaultScriptMode.key;
        jsObject.SetCookie("StimulsoftMobileDesignerDefaultScriptMode", jsObject.options.defaultScriptMode);

        this.changeVisibleState(false);
        jsObject.SendCommandApplyDesignerOptions(designerOptions);
    }

    return optionsForm;
}