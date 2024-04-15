
StiMobileDesigner.prototype.InitializeWizardFormTotals = function (wizardForm) {
    var jsObject = this;
    var totalsPanel = this.WizardFormWorkPanel(wizardForm, "totals");
    totalsPanel.helpTextStandart = "<b>" + this.loc.Wizards.Totals + "</b><br>" + this.loc.Wizards.infoTotals;

    this.InitializeWizardFormStepItem(wizardForm, totalsPanel.name, this.loc.Wizards.Totals);
    totalsPanel.wizardForm = wizardForm;

    totalsPanel.onShow = function () {
        this.update();
    }

    totalsPanel.update = function () {
        this.clear();
        for (var selDataSourceName in this.wizardForm.dataSources) {
            this.check(selDataSourceName);
            var selDataSource = this.wizardForm.dataSources[selDataSourceName];
            var selColumns = selDataSource.columns;
            if (selColumns.length > 0) {
                this.appendChild(jsObject.WizardFormSeparator(selDataSourceName));
                var totalsTable = jsObject.CreateHTMLTable();
                this.appendChild(totalsTable);
                totalsTable.className = "wizardFormTotalsTable";
                totalsTable.headerColumn = totalsTable.addCell();
                totalsTable.headerColumn.className = "wizardFormTotalsHeaderColumn";
                totalsTable.headerColumn.innerHTML = jsObject.loc.PropertyMain.Column;
                totalsTable.headerFunction = totalsTable.addCell();
                totalsTable.headerFunction.className = "wizardFormTotalsHeaderColumn";
                totalsTable.headerFunction.innerHTML = jsObject.loc.PropertyMain.Function;
            }

            for (var index = 0; index < selColumns.length; index++) {
                totalsTable.cellColumn = document.createElement("div");
                totalsTable.cellColumn.className = "wizardFormTotalsDivColumn";
                totalsTable.cellColumn.innerHTML = selColumns[index];
                totalsTable.addRow();
                totalsTable.addCellInLastRow(totalsTable.cellColumn).className = "wizardFormTotalsCellColumn stiDesignerClearAllStyles";

                var functionControl = jsObject.DropDownList(null, 110, null, jsObject.GetTotalFuntionItems(), true, false, jsObject.options.propertyControlsHeight);
                functionControl.style.margin = "4px";
                totalsTable.addCellInLastRow(functionControl).className = "wizardFormTotalsCellFunction";
                functionControl.dataSourceName = selDataSourceName;
                functionControl.columnName = selColumns[index];
                functionControl.wizardForm = this.wizardForm;
                var functionValue = this.wizardForm.dataSources[selDataSourceName].totals[selColumns[index]] || "none";
                functionControl.setKey(functionValue);

                functionControl.action = function () {
                    if (this.key != "none")
                        this.wizardForm.dataSources[this.dataSourceName].totals[this.columnName] = this.key;
                    else
                        delete this.wizardForm.dataSources[this.dataSourceName].totals[this.columnName];
                }
            }
        }
    }

    totalsPanel.check = function (dataSourceName) {
        for (var columnName in this.wizardForm.dataSources[dataSourceName].totals) {
            var numElement = jsObject.GetElementNumberInArray(columnName, wizardForm.dataSources[dataSourceName].columns);
            if (numElement == -1) delete this.wizardForm.dataSources[dataSourceName].totals[columnName];
        }
    }

}