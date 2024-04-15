
StiMobileDesigner.prototype.InitializeWizardFormGroups = function (wizardForm) {
    var groupsPanel = this.WizardFormWorkPanel(wizardForm, "groups");
    groupsPanel.helpTextStandart = "<b>" + this.loc.Wizards.Groups + "</b><br>" + this.loc.Wizards.infoGroups;
    this.InitializeWizardFormStepItem(wizardForm, groupsPanel.name, this.loc.Wizards.Groups);
    groupsPanel.wizardForm = wizardForm;

    groupsPanel.onShow = function () {
        this.update();
    }

    groupsPanel.update = function () {
        this.clear();
        for (var selDataSourceName in this.wizardForm.dataSources) {
            this.check(selDataSourceName);
            var selDataSource = this.wizardForm.dataSources[selDataSourceName];
            var selColumns = selDataSource.columns;
            if (selColumns.length > 0) this.appendChild(this.jsObject.WizardFormSeparator(selDataSourceName));

            for (var i = 0; i < selColumns.length; i++) {
                var checkBoxName = "WizardFormGroups" + selDataSourceName + selColumns[i];
                var checkBox = this.jsObject.WizardFormCheckBox(checkBoxName, selDataSource.columnsText[selColumns[i]] || selColumns[i], selColumns[i]);
                this.appendChild(checkBox);
                checkBox.dataSourceName = selDataSourceName;
                var numElement = this.jsObject.GetElementNumberInArray(selColumns[i], this.wizardForm.dataSources[selDataSourceName].groups);
                checkBox.setChecked(numElement != -1);
                checkBox.groupsPanel = this;

                checkBox.action = function () {
                    var wizardForm = this.jsObject.options.forms.wizardForm;
                    if (this.isChecked)
                        wizardForm.dataSources[this.dataSourceName].groups.push(this.key);
                    else {
                        var numElement = this.jsObject.GetElementNumberInArray(this.key, wizardForm.dataSources[this.dataSourceName].groups);
                        if (numElement != -1) wizardForm.dataSources[this.dataSourceName].groups.splice(numElement, 1);
                    }
                }
            }
        }
    }

    groupsPanel.check = function (dataSourceName) {
        var newArray = [];
        var selGroups = this.wizardForm.dataSources[dataSourceName].groups;

        for (var i = 0; i < selGroups.length; i++) {
            var numElement = this.jsObject.GetElementNumberInArray(selGroups[i], wizardForm.dataSources[dataSourceName].columns);
            if (numElement != -1) newArray.push(selGroups[i]);
        }
        this.wizardForm.dataSources[dataSourceName].groups = newArray;
    }
}