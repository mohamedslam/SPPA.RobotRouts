
StiMobileDesigner.prototype.InitializeWizardFormSort = function (wizardForm) {
    var sortPanel = this.WizardFormWorkPanel(wizardForm, "sort");
    sortPanel.style.overflow = "hidden";
    sortPanel.helpTextStandart = "<b>" + this.loc.PropertyMain.Sort + "</b><br>" + this.loc.Wizards.infoSort;
    this.InitializeWizardFormStepItem(wizardForm, sortPanel.name, this.loc.PropertyMain.Sort);
    sortPanel.wizardForm = wizardForm;

    sortPanel.onShow = function () {
        this.update();
    }

    sortPanel.onHide = function () {
        if (this.jsObject.GetCountObjects(this.wizardForm.dataSources) != 0) this.apply();
    }

    sortPanel.update = function () {
        this.clear();
        var dataSourcesTabs = [];

        for (var dataSourceName in this.wizardForm.dataSources) {
            if (this.wizardForm.dataSources[dataSourceName].columns.length != 0) {
                dataSourcesTabs.push(dataSourceName);
            }
        }

        this.tabbedPane = this.jsObject.WizardFormTabbedPane("WizardFormSortTabbedPane", dataSourcesTabs);
        this.tabbedPane.tabsPanel.style.margin = "6px";
        this.appendChild(this.tabbedPane);

        for (var i = 0; i < dataSourcesTabs.length; i++) {
            var columnsItems = this.wizardForm.workPanels.columns.getColumnsItems(dataSourcesTabs[i]);
            var sortControl = this.jsObject.SortControl("WizardFormSortControl" + this.jsObject.generateKey(), columnsItems, null, this.jsObject.options.isTouchDevice ? 290 : 300);
            sortControl.toolBar.style.margin = "6px";
            sortControl.sortContainer.style.margin = "0 2px";
            sortControl.dataSourceName = dataSourcesTabs[i];

            sortControl.fill = function (sorts) {
                for (var i = 0; i < sorts.length; i++) {
                    this.sortContainer.addSort(sorts[i]);
                }
            }

            var currentTabPanel = this.tabbedPane.tabsPanels[dataSourcesTabs[i]];
            currentTabPanel.appendChild(sortControl);
            currentTabPanel.sortControl = sortControl;
            sortControl.fill(this.wizardForm.dataSources[dataSourcesTabs[i]].sort);
        }
    }

    sortPanel.apply = function () {
        for (var dataSourceName in this.tabbedPane.tabsPanels) {
            var sortControl = this.tabbedPane.tabsPanels[dataSourceName].sortControl;
            this.wizardForm.dataSources[dataSourceName].sort = sortControl.getValue();
        }
    }
}

