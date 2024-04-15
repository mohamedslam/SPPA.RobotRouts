
StiMobileDesigner.prototype.InitializeWizardFormTheme = function (wizardForm) {
    var themePanel = this.WizardFormWorkPanel(wizardForm, "theme");
    themePanel.helpTextStandart = "<b>" + this.loc.Wizards.Themes + "</b><br>" + this.loc.Wizards.infoThemes;
    this.InitializeWizardFormStepItem(wizardForm, themePanel.name, this.loc.Wizards.Themes);
    themePanel.wizardForm = wizardForm;

    themePanel.innerTable = this.CreateHTMLTable();
    themePanel.innerTable.style.margin = "6px";
    themePanel.appendChild(themePanel.innerTable);

    var noneButton = this.StandartBigButton("wizardFormThemesNone_", "wizardFormThemesButtons", this.loc.PropertyEnum.StiCheckStyleNone, "ReportThemeNone.png", null);
    noneButton.allwaysEnabled = true;
    noneButton.key = "None_";
    var noneCell = themePanel.innerTable.addCell(noneButton);
    noneButton.style.margin = "2px 10px 2px 10px";
    noneButton.action = function () { this.setSelected(true); this.jsObject.options.forms.wizardForm.reportOptions.theme = this.key; }

    var separator = document.createElement("div");
    separator.className = "wizardFormThemesTableSeparator";
    themePanel.innerTable.addCellInNextRow(separator).setAttribute("colspan", "4");

    var themeNames = ["Red", "Green", "Blue", "Gray"];
    for (var i = 0; i < 4; i++) {
        themePanel.innerTable.addRow();
        for (var k = 0; k < 4; k++) {
            var themePercent = 100 - k * 25;
            var themeName = themeNames[i] + "_" + themePercent;
            var button = this.StandartBigButton("wizardFormThemes" + themeName, "wizardFormThemesButtons",
                this.loc.PropertyColor[themeNames[i]] + " " + themePercent + "%", "ReportTheme" + themeNames[i] + themePercent + ".png", null);
            button.allwaysEnabled = true;
            button.caption.style.fontSize = "12px";
            button.caption.style.whiteSpace = "nowrap";
            button.style.margin = "2px 10px 0 10px";
            button.key = themeNames[i] + "_" + themePercent;
            button.action = function () { this.setSelected(true); this.jsObject.options.forms.wizardForm.reportOptions.theme = this.key; }
            themePanel.innerTable.addCellInRow(i + 2, button);
        }
    }


    themePanel.onShow = function () {
        this.update();
    }

    themePanel.update = function () {
        this.jsObject.options.buttons["wizardFormThemes" + this.jsObject.options.forms.wizardForm.reportOptions.theme].setSelected(true);
    }

}