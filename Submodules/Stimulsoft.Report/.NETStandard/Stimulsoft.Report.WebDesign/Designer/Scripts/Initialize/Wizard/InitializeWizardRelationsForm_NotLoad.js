
StiMobileDesigner.prototype.InitializeWizardRelationsForm = function () {
    var wizardRelationsForm = this.BaseForm("wizardRelationsForm", this.loc.PropertyMain.Relations, 2);
    wizardRelationsForm.itemsContainer = document.createElement("div");
    wizardRelationsForm.itemsContainer.className = "wizardRelationsFormItemsContainer";
    wizardRelationsForm.container.appendChild(wizardRelationsForm.itemsContainer);
    wizardRelationsForm.itemsConrols = {};

    wizardRelationsForm.onshow = function () {
        var wizardForm = this.jsObject.options.forms.wizardForm;
        while (this.itemsContainer.childNodes[0]) this.itemsContainer.removeChild(this.itemsContainer.childNodes[0]);
        for (var relationKey in wizardForm.reportOptions.relations) {
            var relation = wizardForm.reportOptions.relations[relationKey];
            this.itemsContainer.appendChild(this.jsObject.WizardFormSeparator(relationKey));
            var checkBox = this.jsObject.WizardFormCheckBox("WizardRelationsForm" + relationKey, relation.name, relationKey);
            checkBox.setChecked(relation.checked);
            this.itemsContainer.appendChild(checkBox);
            this.itemsConrols[relationKey] = checkBox;
        }
    }

    wizardRelationsForm.action = function () {
        this.changeVisibleState(false);
        var wizardForm = this.jsObject.options.forms.wizardForm;
        for (var itemKey in this.itemsConrols)
            wizardForm.reportOptions.relations[this.itemsConrols[itemKey].key].checked = this.itemsConrols[itemKey].isChecked;
    }

    return wizardRelationsForm;
}