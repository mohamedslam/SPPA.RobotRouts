
StiMobileDesigner.prototype.InitializeWizardSelectTemplatePanel = function (templates, form, level) {
    var templatesPanel = document.createElement("div");
    templatesPanel.name = name;
    templatesPanel.className = "wizardStepPanel";
    templatesPanel.jsObject = this;
    templatesPanel.level = level;
    templatesPanel.buttons = {};

    var mainTable = this.CreateHTMLTable();
    mainTable.className = "wizardStepPanel";
    templatesPanel.appendChild(mainTable);

    var buttonsCell = mainTable.addCell();
    buttonsCell.align = "center";
    buttonsCell.style.width = "300px";
    buttonsCell.style.height = "100%";

    var previewCell = mainTable.addCell();
    previewCell.style.height = "100%";
    var img = document.createElement("img");
    previewCell.appendChild(img);

    for (var i in templates) {
        var button = this.SmallButton("template" + i, "wizardTemplateButton", templates[i].name, null, null, null, "stiDesignerWizardButton", true);
        button.innerTable.style.width = "100%";
        button.style.width = "120px";
        button.caption.style.textAlign = "center";
        button.img = templates[i].thumbnail;
        button.templateFileName = templates[i].templateFileName;
        buttonsCell.appendChild(button);
        templatesPanel.buttons[templates[i].name] = button;

        button.action = function () {
            this.setSelected(true);
            img.src = "data:image/png;base64," + this.img;
            form.templateName = this.captionText;
            form.templateFileName = this.templateFileName;
            form.stepLevel = templatesPanel.level;
        }
        if (i == 0) button.action();
    }

    templatesPanel.onShow = function () { };
    templatesPanel.onHide = function () { };

    templatesPanel.update = function (needReset) {
        form.enableButtons(false, true, false);
    }

    templatesPanel.getReportOptions = function (options) {
        options.templateName = form.templateName;
        options.templateFileName = form.templateFileName;
    }

    form.appendStepPanel(templatesPanel, form.jsObject.loc.Wizards.infoSelectTemplate);
    return templatesPanel;
}