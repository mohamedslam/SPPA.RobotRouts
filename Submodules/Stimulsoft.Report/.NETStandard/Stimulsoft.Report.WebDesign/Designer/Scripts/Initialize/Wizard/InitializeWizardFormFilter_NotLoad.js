
StiMobileDesigner.prototype.InitializeWizardFormFilter = function (wizardForm) {
    var jsObject = this;
    var filterPanel = this.WizardFormWorkPanel(wizardForm, "filter");
    filterPanel.style.overflow = "hidden";
    filterPanel.helpTextStandart = "<b>" + this.loc.PropertyMain.Filters + "</b><br>" + this.loc.Wizards.infoFilters;
    this.InitializeWizardFormStepItem(wizardForm, filterPanel.name, this.loc.PropertyMain.Filters);
    filterPanel.wizardForm = wizardForm;
    filterPanel.selectedColumnsItem = null;

    filterPanel.onShow = function () {
        this.update();
    }

    filterPanel.onHide = function () {
        if (jsObject.GetCountObjects(this.wizardForm.dataSources) != 0) this.apply();
    }

    filterPanel.update = function () {
        this.clear();
        var dataSourcesTabs = [];

        for (var dataSourceName in this.wizardForm.dataSources) {
            dataSourcesTabs.push(dataSourceName);
        }

        this.tabbedPane = jsObject.WizardFormTabbedPane("WizardFormFilterTabbedPane", dataSourcesTabs);
        this.tabbedPane.tabsPanel.style.margin = "6px";
        this.appendChild(this.tabbedPane);

        for (var i = 0; i < dataSourcesTabs.length; i++) {
            var columnsItems = this.wizardForm.workPanels.columns.getColumnsItems(dataSourcesTabs[i]);
            var filterControl = jsObject.FilterControl("WizardFormFilterControl" + jsObject.generateKey(), columnsItems, null, jsObject.options.isTouchDevice ? 250 : 260);
            filterControl.controls.toolBar.style.margin = "6px";
            filterControl.controls.filterContainer.style.margin = "0 2px";

            var currentTabPanel = this.tabbedPane.tabsPanels[dataSourcesTabs[i]];
            currentTabPanel.appendChild(filterControl);
            currentTabPanel.filterControl = filterControl;

            currentTabPanel.appendChild(jsObject.FormSeparator());
            var filterEngineTable = jsObject.CreateHTMLTable();
            currentTabPanel.appendChild(filterEngineTable);
            filterEngineTable.addTextCell(jsObject.loc.PropertyMain.FilterEngine).className = "stiDesignerCaptionControls";

            var filterEngine = jsObject.DropDownList("dataFormFilterEngine" + dataSourcesTabs[i], 125, jsObject.loc.PropertyMain.FilterEngine, jsObject.GetFilterIngineItems(), true, false);
            filterEngine.setKey("ReportEngine");
            filterEngineTable.addCell(filterEngine).style.padding = "12px 15px 12px 0px";
            currentTabPanel.filterEngine = filterEngine;

            var currentDataSource = this.wizardForm.dataSources[dataSourcesTabs[i]];
            filterControl.fill(currentDataSource.filters, currentDataSource.filterOn, currentDataSource.filterMode);
        }
    }

    filterPanel.apply = function () {
        for (var dataSourceName in this.tabbedPane.tabsPanels) {
            var filterControl = this.tabbedPane.tabsPanels[dataSourceName].filterControl;
            var filterResult = filterControl.getValue();
            var dataSource = this.wizardForm.dataSources[dataSourceName];
            var filterEngine = this.tabbedPane.tabsPanels[dataSourceName].filterEngine;

            dataSource.filters = filterResult.filters;
            dataSource.filterOn = filterResult.filterOn;
            dataSource.filterMode = filterResult.filterMode;
            dataSource.filterEngine = filterEngine.key;
        }
    }
}



