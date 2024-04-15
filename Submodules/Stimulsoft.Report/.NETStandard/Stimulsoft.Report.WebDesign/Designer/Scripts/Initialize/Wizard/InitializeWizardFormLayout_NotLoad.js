
StiMobileDesigner.prototype.InitializeWizardFormLayout = function (wizardForm) {
    var layoutPanel = this.WizardFormWorkPanel(wizardForm, "layout");
    layoutPanel.helpTextStandart = "<b>" + this.loc.Wizards.Layout + "</b><br>" + this.loc.Wizards.infoLayout;
    this.InitializeWizardFormStepItem(wizardForm, layoutPanel.name, this.loc.Wizards.Layout);
    layoutPanel.wizardForm = wizardForm;

    layoutPanel.innerTable = this.CreateHTMLTable();
    layoutPanel.innerTable.style.margin = "10px";
    layoutPanel.appendChild(layoutPanel.innerTable);

    //Orientation
    var orientationText = layoutPanel.innerTable.addCell();
    orientationText.innerHTML = this.loc.PropertyMain.Orientation + ":";
    orientationText.className = "stiDesignerCaptionControls";
    layoutPanel.orientation = this.DropDownList(null, 120, null, this.GetPageOrientationItems(false), true);
    var orientationControlCell = layoutPanel.innerTable.addCell(layoutPanel.orientation);
    orientationControlCell.className = "stiDesignerControlCellsBigIntervals2";
    layoutPanel.orientation.action = function () { this.jsObject.options.forms.wizardForm.reportOptions.orientation = this.key; }

    //Unit
    var unitText = layoutPanel.innerTable.addCellInNextRow();
    unitText.innerHTML = this.loc.PropertyMain.Unit + ":";
    unitText.className = "stiDesignerCaptionControls";
    layoutPanel.unit = this.DropDownList(null, 120, null, this.GetUnitItems(), true);
    var unitControlCell = layoutPanel.innerTable.addCellInLastRow(layoutPanel.unit);
    unitControlCell.className = "stiDesignerControlCellsBigIntervals2";
    layoutPanel.unit.action = function () { this.jsObject.options.forms.wizardForm.reportOptions.unit = this.key; }

    //Language
    var languageText = layoutPanel.innerTable.addCellInNextRow();
    languageText.innerHTML = this.loc.PropertyMain.Language + ":";
    languageText.className = "stiDesignerCaptionControls";
    layoutPanel.languageControl = this.DropDownList(null, 120, null, this.GetLanguagesItems(), true);

    var languageControlCell = layoutPanel.innerTable.addCellInLastRow(layoutPanel.languageControl);
    languageControlCell.className = "stiDesignerControlCellsBigIntervals2";
    layoutPanel.languageControl.action = function () { this.jsObject.options.forms.wizardForm.reportOptions.language = this.key; }

    //Component Type
    var componentTypeText = layoutPanel.innerTable.addCellInNextRow();
    componentTypeText.innerHTML = this.loc.Components.StiComponent + ":";
    componentTypeText.className = "stiDesignerCaptionControls";
    layoutPanel.componentType = this.DropDownList(null, 120, null, this.GetComponentTypeItems(), true);

    var componentTypeControlCell = layoutPanel.innerTable.addCellInLastRow(layoutPanel.componentType);
    componentTypeControlCell.className = "stiDesignerControlCellsBigIntervals2";
    layoutPanel.componentType.action = function () { this.jsObject.options.forms.wizardForm.reportOptions.componentType = this.key; }

    layoutPanel.onShow = function () {
        this.update();
    }

    layoutPanel.update = function () {
        this.orientation.setKey(this.wizardForm.reportOptions.orientation);
        this.unit.setKey(this.wizardForm.reportOptions.unit);
        this.languageControl.setKey(this.wizardForm.reportOptions.language);
        this.componentType.setKey(this.wizardForm.reportOptions.componentType);
    }

}