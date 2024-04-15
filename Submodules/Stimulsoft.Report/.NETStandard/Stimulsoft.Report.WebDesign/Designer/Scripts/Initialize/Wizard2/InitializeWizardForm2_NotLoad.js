
StiMobileDesigner.prototype.InitializeWizardForm2_ = function () {
    var wizardForm = this.BaseForm("wizardForm2", this.loc.FormTitles.ReportWizard, 1);
    wizardForm.container.style.borderTop = "0";
    wizardForm.style.backgroundColor = "#fafafa";
    wizardForm.stepPanels = [];
    var mainTable = this.CreateHTMLTable();
    mainTable.className = "wizardFormMainTable stiDesignerClearAllStyles";
    mainTable.style.width = "890px";
    wizardForm.container.appendChild(mainTable);
    wizardForm.typeReport = "Invoice";

    wizardForm.appendStepPanel = function (content, caption) {
        var panel = document.createElement("div");
        panel.style.display = "none";
        var captionPanel = document.createElement("div");
        captionPanel.className = "wizardStepCaption";
        captionPanel.innerHTML = caption;
        panel.appendChild(captionPanel);
        panel.appendChild(content);
        panel.content = content;
        mainTable.stepPanel.appendChild(panel);
        wizardForm.stepPanels.push(panel);
    }

    wizardForm.showStep = function (stepNumber) {
        wizardForm.stepNumber = stepNumber;
        wizardForm.stepPanelContainer.setStep(stepNumber);
        wizardForm.stepPanel = wizardForm.stepPanels[stepNumber].content;
        for (var i = 0; i < wizardForm.stepPanels.length; i++)
            wizardForm.stepPanels[i].style.display = i == stepNumber ? "" : "none";

        if (wizardForm.stepPanels[stepNumber].content.update)
            wizardForm.stepPanels[stepNumber].content.update(wizardForm.stepPanels[stepNumber].content.level > wizardForm.stepLevel);
    }

    wizardForm.enableButtons = function (prev, next, finish) {
        wizardForm.buttonPrev.setEnabled(prev);
        wizardForm.buttonNext.setEnabled(next);
        wizardForm.buttonFinish.setEnabled(finish);
    }

    wizardForm.getTemplate = function () {
        var templates = wizardForm.jsObject.options.wizardData[wizardForm.typeReport].templates;
        for (var i in templates)
            if (wizardForm.templateName == templates[i].name)
                return templates[i];
    }

    wizardForm.getDatasource = function () {
        var datasources = wizardForm.getTemplate().datasources;
        for (var i in datasources)
            if (wizardForm.datasourceName == datasources[i].name && wizardForm.datasourceCategory == datasources[i].category)
                return datasources[i];

        var databases = wizardForm.jsObject.options.report.dictionary.databases;
        for (var i in databases) {
            var db = databases[i];
            for (var j in db.dataSources) {
                if (wizardForm.datasourceName == db.dataSources[j].name && wizardForm.datasourceCategory == db.name)
                    return db.dataSources[j];
            }
        }
    }

    wizardForm.action = function () {
        if (wizardForm.stepNumber != wizardForm.stepPanels.length - 1) {
            wizardForm.showStep(wizardForm.stepNumber + 1);
        } else {
            this.changeVisibleState(false);
            this.jsObject.SendCommandWizardResult({ "reportOptions": this.getReportOptions() });
        }
    }

    wizardForm.getReportOptions = function () {
        var options = { "typeReport": this.typeReport };
        options.templateName = wizardForm.templateName;
        options.templateFileName = wizardForm.templateFileName;
        for (var i in this.stepPanels) {
            this.stepPanels[i].content.getReportOptions(options);
        }
        return options;
    }

    wizardForm.createStepsComplete = function () { };

    wizardForm.createSteps = function () {
        var data = this.jsObject.options.wizardData[this.typeReport];
        if (wizardForm.typeReport == "Invoice" || wizardForm.typeReport == "Order") {
            this.jsObject.InitializeWizardSelectTemplatePanel(data.templates, wizardForm, 0);
            this.jsObject.InitializeWizardSelectDatasourcePanel(data.templates, wizardForm, 1);
            this.jsObject.InitializeWizardMappingPanel(data.templates, wizardForm, 2);
            this.jsObject.InitializeWizardCompanyPanel(data.templates, wizardForm, 3);
            this.jsObject.InitializeWizardLanguagePanel(data.templates, wizardForm, 4);
            this.jsObject.InitializeWizardThemePanel(data.templates, wizardForm, 5);
        } else if (wizardForm.typeReport == "Quotation") {
            wizardForm.templateName = data.templates[0].name;
            wizardForm.templateFileName = data.templates[0].templateFileName;

            this.jsObject.InitializeWizardSelectDatasourcePanel(data.templates, wizardForm, 0);
            this.jsObject.InitializeWizardMappingPanel(data.templates, wizardForm, 1);
            this.jsObject.InitializeWizardCompanyPanel(data.templates, wizardForm, 2);
            this.jsObject.InitializeWizardLanguagePanel(data.templates, wizardForm, 3);
            this.jsObject.InitializeWizardThemePanel(data.templates, wizardForm, 4);
        } else if (wizardForm.typeReport == "Label") {
            wizardForm.templateName = wizardForm.typeReport;
            wizardForm.templateFileName = wizardForm.typeReport;
            this.jsObject.InitializeWizardSelectDatasourcePanel(data.templates, wizardForm, 0);
            this.jsObject.InitializeWizardLabelsPanel(data.templates, wizardForm, 1);
        }
        this.showStep(0);
        this.createStepsComplete();
    }

    wizardForm.onshow = function () {
        wizardForm.stepNumber = 0;
        wizardForm.stepLevel = -1;
        while (mainTable.childNodes.length > 0) mainTable.removeChild(mainTable.childNodes[0]);
        wizardForm.stepPanels = [];

        var stepNames = [];
        if (wizardForm.typeReport == "Invoice" || wizardForm.typeReport == "Order") {
            stepNames = [this.jsObject.loc.Wizards.SelectTemplate,
            this.jsObject.loc.Wizards.DataSource,
            this.jsObject.loc.Wizards.Mapping,
            this.jsObject.loc.Wizards.Company,
            this.jsObject.loc.Services.categoryLanguages,
            this.jsObject.loc.Wizards.Themes];
        } if (wizardForm.typeReport == "Quotation") {
            stepNames = [this.jsObject.loc.Wizards.DataSource,
            this.jsObject.loc.Wizards.Mapping,
            this.jsObject.loc.Wizards.Company,
            this.jsObject.loc.Services.categoryLanguages,
            this.jsObject.loc.Wizards.Themes];
        } if (wizardForm.typeReport == "Label") {
            stepNames = [this.jsObject.loc.Wizards.DataSource,
            this.jsObject.loc.Wizards.LabelSettings];
        }
        while (mainTable.childNodes.length > 0) mainTable.removeChild(mainTable.childNodes[0]);

        mainTable.addRow();
        var stepCell = mainTable.addCellInLastRow();
        wizardForm.stepPanelContainer = this.jsObject.InitializeWizardStepPanel(stepNames);
        stepCell.appendChild(wizardForm.stepPanelContainer);

        mainTable.addRow().className += " wizardStepPanel";
        mainTable.stepPanel = mainTable.addCellInLastRow();

        if (!this.jsObject.options.wizardData) this.jsObject.options.wizardData = {};
        if (!this.jsObject.options.wizardData[this.typeReport]) {
            this.jsObject.AddProgressToControl(wizardForm);
            this.progress.show();
            this.enableButtons(false, false, false);
            this.jsObject.SendCommandGetWizardData(this.typeReport, function (answer) {
                wizardForm.jsObject.options.wizardData[wizardForm.typeReport] = answer.wizardData;
                wizardForm.createSteps();
                wizardForm.progress.hide();
            });
        } else {
            this.createSteps();
        }
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

    //Ovveride Buttons Panel 
    while (wizardForm.buttonsPanel.childNodes[0]) wizardForm.buttonsPanel.removeChild(wizardForm.buttonsPanel.childNodes[0]);
    var buttonsTable = this.CreateHTMLTable();
    wizardForm.buttonsPanel.appendChild(buttonsTable);

    //Back
    wizardForm.buttonPrev = this.FormButton(wizardForm, wizardForm.name + "ButtonBack", this.loc.A_WebViewer.ButtonPrev, null);
    wizardForm.buttonPrev.style.minWidth = "100px";
    wizardForm.buttonPrev.action = function () { wizardForm.showStep(wizardForm.stepNumber - 1); };
    buttonsTable.addCell(wizardForm.buttonPrev).style.padding = "8px 3px 8px 3px";

    //Next
    wizardForm.buttonNext = this.FormButton(wizardForm, wizardForm.name + "ButtonNext", this.loc.A_WebViewer.ButtonNext, null);
    wizardForm.buttonNext.style.minWidth = "100px";
    wizardForm.buttonNext.action = function () { wizardForm.showStep(wizardForm.stepNumber + 1); };
    buttonsTable.addCell(wizardForm.buttonNext).style.padding = "8px 23px 8px 3px";

    //Finish
    wizardForm.buttonFinish = this.FormButton(wizardForm, wizardForm.name + "ButtonFinish", this.loc.Wizards.ButtonFinish.replace("&", ""), null);
    wizardForm.buttonFinish.style.minWidth = "100px";
    wizardForm.buttonFinish.action = function () { wizardForm.action(); };
    buttonsTable.addCell(wizardForm.buttonFinish).style.padding = "8px 3px 8px 3px";

    //Cancel
    wizardForm.buttonCancel = this.FormButton(wizardForm, wizardForm.name + "ButtonCancel", this.loc.Wizards.ButtonCancel, null);
    wizardForm.buttonCancel.style.minWidth = "100px";
    wizardForm.buttonCancel.action = function () { wizardForm.changeVisibleState(false); };
    buttonsTable.addCell(wizardForm.buttonCancel).style.padding = "8px";

    return wizardForm;
}