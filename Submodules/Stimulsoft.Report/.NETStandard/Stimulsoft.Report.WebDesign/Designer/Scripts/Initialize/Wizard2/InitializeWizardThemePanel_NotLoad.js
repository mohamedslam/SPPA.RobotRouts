
StiMobileDesigner.prototype.InitializeWizardThemePanel = function (templates, form, level) {
    var themePanel = document.createElement("div");
    themePanel.name = name;
    themePanel.className = "wizardStepPanel";
    themePanel.jsObject = this;
    themePanel.level = level;

    var mainTable = this.CreateHTMLTable();
    mainTable.style.marginLeft = "auto";
    mainTable.style.marginRight = "auto";
    mainTable.style.marginTop = "10px";
    themePanel.appendChild(mainTable);

    themePanel.onShow = function () { };
    themePanel.onHide = function () { };

    themePanel.update = function (needReset) {
        if (needReset) {
            while (mainTable.childNodes.length > 0) mainTable.removeChild(mainTable.childNodes[0]);
            mainTable.addRow();

            var themes = form.getTemplate().themes;
            for (var i in themes) {
                var imgName = form.templateName + themes[i].name;
                this.jsObject.options.images[imgName] = "data:image/png;base64," + themes[i].img;

                var button = this.jsObject.SmallButton("theme" + i, "wizardThemeButton", null, imgName, themes[i].desc, null, "stiDesignerWizardThemeButton", true, { width: 141, height: 200 });
                button.style.width = "148px";
                button.theme = themes[i].name;

                button.action = function () {
                    this.setSelected(true);
                    themePanel.theme = this.theme;
                }

                mainTable.addCellInLastRow(button);
                if (i == 0) button.action();
            }
            if (themePanel.level < form.stepLevel)
                form.stepLevel = themePanel.level;
        }
        form.enableButtons(true, false, true);

        if (this.level > form.stepLevel)
            form.stepLevel = this.level;
    }

    themePanel.getReportOptions = function (options) {
        options.theme = themePanel.theme;
    }

    form.appendStepPanel(themePanel, form.jsObject.loc.Wizards.infoSelectTemplate);
    return themePanel;
}