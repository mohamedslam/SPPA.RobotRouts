
StiMobileDesigner.prototype.InitializeWizardLanguagePanel = function (templates, form, level) {
    var languagePanel = document.createElement("div");
    languagePanel.name = name;
    languagePanel.className = "wizardStepPanel";
    languagePanel.jsObject = this;
    languagePanel.style.maxHeight = "410px";
    languagePanel.level = level;

    var mainTable = this.CreateHTMLTable();
    mainTable.style.marginLeft = "auto";
    mainTable.style.marginRight = "auto";
    mainTable.style.marginTop = "10px";
    languagePanel.appendChild(mainTable);

    languagePanel.update = function (needReset) {
        if (needReset) {
            while (mainTable.childNodes.length > 0) mainTable.removeChild(mainTable.childNodes[0]);
            mainTable.addRow();

            var languages = form.getTemplate().languages;
            for (var i in languages) {

                var button = this.jsObject.SmallButton("language" + i, "wizardLanguageButton", languages[i], null, null, null, "stiDesignerWizardButton", true);
                button.innerTable.style.width = "100%";
                button.style.width = "50px";
                button.caption.style.textAlign = "center";
                button.language = languages[i];

                button.action = function () {
                    this.setSelected(true);
                    languagePanel.language = this.language;
                }

                mainTable.addCellInLastRow(button);
                if (i == 0) button.action();
            }
            if (languagePanel.level < form.stepLevel)
                form.stepLevel = languagePanel.level;
        }
        form.enableButtons(true, true, true);

        if (this.level > form.stepLevel)
            form.stepLevel = this.level;
    }

    languagePanel.getReportOptions = function (options) {
        options.language = languagePanel.language;
    }

    languagePanel.onShow = function () { };
    languagePanel.onHide = function () { };

    form.appendStepPanel(languagePanel, form.jsObject.loc.Wizards.infoLanguages);
    return languagePanel;
}