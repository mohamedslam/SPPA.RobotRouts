
StiMobileDesigner.prototype.InitializeWizardForm_ = function () {
    var wizardForm = this.BaseForm("wizardForm", this.loc.Wizards.title, 1, this.HelpLinks["wizard"]);
    var mainTable = this.CreateHTMLTable();
    mainTable.className = "wizardFormMainTable stiDesignerClearAllStyles";
    wizardForm.container.appendChild(mainTable);
    wizardForm.typeReport = "Standart";
    wizardForm.workPanels = {};
    wizardForm.steps = [];
    wizardForm.stepNumber = 0;
    wizardForm.reportOptions = {};
    wizardForm.dataSources = {};
    wizardForm.dataSourcesFromServer = [];

    //Info Cell
    wizardForm.infoCell = mainTable.addCell();
    wizardForm.infoCell.className = "wizardFormInfoCell";
    wizardForm.infoCell.setAttribute("colspan", "2");

    //Steps Panel
    wizardForm.stepsPanel = mainTable.addCellInNextRow();
    wizardForm.stepsPanel.className = "wizardFormStepsPanel";
    wizardForm.stepsPanel.stepItems = {};

    //Parent Work Panel
    wizardForm.parentWorkPanel = document.createElement("div");
    wizardForm.parentWorkPanel.className = "wizardFormWorkPanel";
    mainTable.addCellInLastRow(wizardForm.parentWorkPanel);

    //Add Work Panels
    this.InitializeWizardFormDataSource(wizardForm);
    this.InitializeWizardFormColumns(wizardForm);
    this.InitializeWizardFormColumnsOrder(wizardForm);
    this.InitializeWizardFormSort(wizardForm);
    this.InitializeWizardFormFilter(wizardForm);
    this.InitializeWizardFormGroups(wizardForm);
    this.InitializeWizardFormTotals(wizardForm);
    this.InitializeWizardFormTheme(wizardForm);
    this.InitializeWizardFormLayout(wizardForm);

    wizardForm.showStep = function (stepNumber) {
        if (stepNumber < 0) stepNumber = 0;
        if (stepNumber > wizardForm.steps.length - 1) stepNumber = wizardForm.steps;
        wizardForm.stepsPanel.stepItems[this.steps[stepNumber]].setSelected();
        this.buttonBack.setEnabled(stepNumber > 0);
        this.buttonNext.setEnabled(this.jsObject.options.cloudMode ? stepNumber < this.steps.length - 2 : stepNumber < this.steps.length - 1);
        this.buttonFinish.setEnabled(stepNumber > 1);

        this.workPanels[this.steps[this.stepNumber]].style.display = "none";
        this.workPanels[this.steps[this.stepNumber]].onHide();
        this.stepNumber = stepNumber;
        this.infoCell.innerHTML = (wizardForm.typeReport == "Standart")
            ? this.workPanels[this.steps[stepNumber]].helpTextStandart
            : (this.workPanels[this.steps[stepNumber]].helpTextMasterDetail || this.workPanels[this.steps[stepNumber]].helpTextStandart);
        this.workPanels[this.steps[stepNumber]].style.display = "";
        this.workPanels[this.steps[stepNumber]].onShow();
    }

    wizardForm.action = function () {
        this.changeVisibleState(false);
        this.finishCheck();
        this.jsObject.SendCommandWizardResult({ "dataSources": this.dataSources, "reportOptions": this.reportOptions });
    }

    wizardForm.onshow = function (allowDefaultSelect) {
        if (this.jsObject.options.cloudMode) this.stepsPanel.stepItems["layout"].style.display = "none";
        this.dataSources = {};
        this.workPanels.columns.columnsHeaderKeys = {};
        this.reportOptions = this.jsObject.DefaultReportOptions();
        this.showStep(0);
        if (allowDefaultSelect) {
            var dataSourcesContainer = wizardForm.workPanels.dataSource.itemsContent;
            if (dataSourcesContainer.childNodes.length == 1) {
                dataSourcesContainer.childNodes[0].setChecked(true);
                dataSourcesContainer.childNodes[0].action();
            }
        }
        this.helpUrl = this.typeReport == "Standart"
            ? "user-manual/index.html?reports_designer_creating_reports_in_designer_overview_standard_report_wizard.htm"
            : "user-manual/index.html?reports_designer_creating_reports_in_designer_overview_master_detail_report_wizard.htm";
    }

    wizardForm.onhide = function () {
        this.jsObject.DeleteTemporaryMenus();
    }

    wizardForm.getDataSourceByName = function (name) {
        if (!this.dataSourcesFromServer) return null;
        for (var i = 0; i < this.dataSourcesFromServer.length; i++) {
            if (this.dataSourcesFromServer[i].typeItem == "BusinessObject" && this.dataSourcesFromServer[i].fullName == name) {
                return this.dataSourcesFromServer[i];
            }
            else if (this.dataSourcesFromServer[i].name == name) {
                return this.dataSourcesFromServer[i];
            }
        }
        return null;
    }

    wizardForm.finishCheck = function () {
        this.showStep(this.jsObject.options.cloudMode ? this.steps.length - 2 : this.steps.length - 1);
        for (var dataSourceName in this.dataSources) {
            this.workPanels.groups.check(dataSourceName);
            this.workPanels.totals.check(dataSourceName);
        }
    }

    //Ovveride Buttons Panel 
    while (wizardForm.buttonsPanel.childNodes[0]) wizardForm.buttonsPanel.removeChild(wizardForm.buttonsPanel.childNodes[0]);
    var buttonsTable = this.CreateHTMLTable();
    wizardForm.buttonsPanel.appendChild(buttonsTable);

    //Back
    wizardForm.buttonBack = this.FormButton(wizardForm, wizardForm.name + "ButtonBack", this.loc.Wizards.ButtonBack.replace("&", ""), null);
    wizardForm.buttonBack.action = function () { wizardForm.showStep(wizardForm.stepNumber - 1); };
    buttonsTable.addCell(wizardForm.buttonBack).style.padding = "12px 0 12px 12px";

    //Next
    wizardForm.buttonNext = this.FormButton(wizardForm, wizardForm.name + "ButtonNext", this.loc.Wizards.ButtonNext.replace("&", ""), null);
    wizardForm.buttonNext.action = function () { wizardForm.showStep(wizardForm.stepNumber + 1); };
    buttonsTable.addCell(wizardForm.buttonNext).style.padding = "12px 0 12px 12px";

    //Finish
    wizardForm.buttonFinish = this.FormButton(wizardForm, wizardForm.name + "ButtonFinish", this.loc.Wizards.ButtonFinish.replace("&", ""), null, null, null, null, "stiDesignerFormButtonTheme");
    wizardForm.buttonFinish.action = function () { wizardForm.action(); };
    buttonsTable.addCell(wizardForm.buttonFinish).style.padding = "12px 0 12px 12px";

    //Cancel
    wizardForm.buttonCancel = this.FormButton(wizardForm, wizardForm.name + "ButtonCancel", this.loc.Buttons.Cancel.replace("&", ""), null);
    wizardForm.buttonCancel.action = function () { wizardForm.changeVisibleState(false); };
    buttonsTable.addCell(wizardForm.buttonCancel).style.padding = "12px";

    return wizardForm;
}

StiMobileDesigner.prototype.InitializeWizardFormStepItem = function (wizardForm, name, caption) {
    var stepItem = document.createElement("div");
    stepItem.name = name;
    stepItem.wizardForm = wizardForm;
    stepItem.className = "wizardFormStepItem";
    stepItem.jsObject = this;
    wizardForm.stepsPanel.appendChild(stepItem);
    wizardForm.stepsPanel.stepItems[name] = stepItem;
    stepItem.innerHTML = caption;

    stepItem.setSelected = function () {
        for (var stepItemName in this.wizardForm.stepsPanel.stepItems) {
            var stepItem = this.wizardForm.stepsPanel.stepItems[stepItemName];
            stepItem.className = (this.name == stepItem.name) ? "wizardFormStepItemSelected" : "wizardFormStepItem";
        }
    }
}


StiMobileDesigner.prototype.WizardFormWorkPanel = function (wizardForm, name) {
    var workPanel = document.createElement("div");
    workPanel.className = "wizardFormWorkPanel";
    workPanel.style.display = "none";
    workPanel.name = name;
    workPanel.jsObject = this;
    workPanel.wizardForm = wizardForm;
    wizardForm.workPanels[name] = workPanel;
    wizardForm.steps.push(name);
    wizardForm.parentWorkPanel.appendChild(workPanel);

    workPanel.clear = function () { while (this.childNodes[0]) this.removeChild(this.childNodes[0]); }
    workPanel.onShow = function () { };
    workPanel.onHide = function () { };

    return workPanel;
}

StiMobileDesigner.prototype.DefaultReportOptions = function () {
    return {
        "orientation": "Portrait",
        "unit": this.GetUnitShortName(this.options.defaultUnit),
        "language": "C",
        "componentType": "Data",
        "theme": "None_",
        "dataSourcesOrder": [],
        "relations": {}
    }
}