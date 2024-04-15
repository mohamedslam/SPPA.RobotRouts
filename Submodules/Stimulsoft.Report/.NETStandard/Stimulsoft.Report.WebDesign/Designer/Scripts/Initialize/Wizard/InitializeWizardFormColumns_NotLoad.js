
StiMobileDesigner.prototype.InitializeWizardFormColumns = function (wizardForm) {
    var columnsPanel = this.WizardFormWorkPanel(wizardForm, "columns");
    columnsPanel.style.overflow = "hidden";
    columnsPanel.helpTextStandart = "<b>" + this.loc.Wizards.SelectColumns + "</b><br>" + this.loc.Wizards.infoSelectColumns;
    this.InitializeWizardFormStepItem(wizardForm, columnsPanel.name, this.loc.Wizards.SelectColumns);
    columnsPanel.wizardForm = wizardForm;
    columnsPanel.columnsControls = {};
    columnsPanel.columnsHeaderKeys = {};

    columnsPanel.itemsContent = document.createElement("div");
    columnsPanel.itemsContent.className = "wizardFormColumnsItemsContent";
    columnsPanel.appendChild(columnsPanel.itemsContent);
    columnsPanel.buttonsContent = document.createElement("div");
    columnsPanel.buttonsContent.className = "wizardFormColumnsButtonsContent";
    columnsPanel.appendChild(columnsPanel.buttonsContent);
    var buttonsTable = this.CreateHTMLTable();
    columnsPanel.buttonsContent.appendChild(buttonsTable);
    buttonsTable.className = "wizardFormColumnsButtonsTable stiDesignerClearAllStyles";

    buttonsTable.addCell().style.width = "100%";

    //Button MarkAll
    columnsPanel.markAllButton = this.FormButton(null, "wizardFormColumnMarkAllButton", this.loc.Wizards.MarkAll.replace("&", ""), null);
    columnsPanel.markAllButton.style.marginRight = "12px";
    buttonsTable.addCell(columnsPanel.markAllButton);
    columnsPanel.markAllButton.columnsPanel = columnsPanel;
    columnsPanel.markAllButton.action = function () { this.columnsPanel.markAll(); }

    //Button Reset
    columnsPanel.resetButton = this.FormButton(null, "wizardFormColumnResetButton", this.loc.Wizards.Reset.replace("&", ""), null);
    columnsPanel.resetButton.style.marginRight = "12px";
    buttonsTable.addCell(columnsPanel.resetButton);
    columnsPanel.resetButton.columnsPanel = columnsPanel;
    columnsPanel.resetButton.action = function () { this.columnsPanel.resetAll(); }

    columnsPanel.onShow = function () {
        this.update();
        if (!this.haveCheckedColumns()) this.markAll();
    }

    columnsPanel.update = function () {
        while (this.itemsContent.childNodes[0]) this.itemsContent.removeChild(this.itemsContent.childNodes[0]);
        this.columnsControls = {};

        for (var selDataSourceName in this.wizardForm.dataSources) {
            var selDataSource = this.wizardForm.getDataSourceByName(selDataSourceName);
            if (!selDataSource) continue;
            var columnsHeader = this.jsObject.WizardFormColumnsHeader(this.wizardForm, selDataSource);
            this.itemsContent.appendChild(columnsHeader);
            if (!this.columnsHeaderKeys[selDataSourceName]) this.columnsHeaderKeys[selDataSourceName] = "NotAssigned";
            columnsHeader.relations.setKey(this.columnsHeaderKeys[selDataSourceName]);
            var columns = [];
            this.jsObject.GetAllColumnsFromDataSource(selDataSourceName, null, "", columns);

            for (var index = 0; index < columns.length; index++) {
                var checkBoxName = "WizardFormColumn" + selDataSourceName + columns[index].relation + columns[index].column;
                var ckeckBoxKey = columns[index].relation + (columns[index].relation != "" ? "." : "") + columns[index].column;
                var columnText = columns[index].column != columns[index].columnAlias ? ckeckBoxKey + " [" + columns[index].columnAlias + "]" : ckeckBoxKey;
                var checkBox = this.jsObject.WizardFormCheckBox(checkBoxName, columnText, ckeckBoxKey);
                this.itemsContent.appendChild(checkBox);
                checkBox.dataSourceName = selDataSourceName;
                var numElement = this.jsObject.GetElementNumberInArray(ckeckBoxKey, this.wizardForm.dataSources[selDataSourceName].columns);
                checkBox.setChecked(numElement != -1);
                checkBox.columnsPanel = this;
                this.columnsControls[checkBoxName] = checkBox;
                checkBox.style.display =
                    ((columns[index].relation == "" && this.columnsHeaderKeys[selDataSourceName] == "NotAssigned") ||
                        (columns[index].relation == this.columnsHeaderKeys[selDataSourceName])) ? "" : "none";

                checkBox.action = function () {
                    if (this.isChecked) {
                        wizardForm.dataSources[this.dataSourceName].columns.push(this.key);
                        wizardForm.dataSources[this.dataSourceName].columnsText[this.key] = this.captionCell.innerHTML;
                    }
                    else {
                        var numElement = this.jsObject.GetElementNumberInArray(this.key, wizardForm.dataSources[this.dataSourceName].columns);
                        if (numElement != -1) {
                            wizardForm.dataSources[this.dataSourceName].columns.splice(numElement, 1);
                            if (wizardForm.dataSources[this.dataSourceName].columnsText[this.key]) {
                                delete wizardForm.dataSources[this.dataSourceName].columnsText[this.key];
                            }
                        }
                    }
                    wizardForm.buttonNext.setEnabled(this.columnsPanel.haveCheckedColumns());
                }
            }
        }

        this.wizardForm.buttonNext.setEnabled(this.haveCheckedColumns());
    }

    columnsPanel.markAll = function () {
        for (var controlName in this.columnsControls)
            if (this.columnsControls[controlName].style.display == "" && !this.columnsControls[controlName].isChecked) {
                this.columnsControls[controlName].setChecked(true);
                this.columnsControls[controlName].action();
            }
    }

    columnsPanel.resetAll = function () {
        for (var controlName in this.columnsControls) {
            this.columnsControls[controlName].setChecked(false);
            this.columnsControls[controlName].action();
        }
    }

    columnsPanel.getColumnsItems = function (dataSourceName) {
        var columns = [];
        var items = [];
        this.jsObject.GetAllColumnsFromDataSource(dataSourceName, null, "", columns);

        for (var i = 0; i < columns.length; i++) {
            var columnKey = columns[i].relation ? columns[i].relation + "." + columns[i].column : columns[i].column;
            var columnText = (columns[i].column != columns[i].columnAlias) ? columnKey + " [" + columns[i].columnAlias + "]" : columnKey;
            items.push(this.jsObject.Item(columns[i].column, columnText, null, columnKey));
        }

        return items;
    }

    columnsPanel.haveCheckedColumns = function () {
        for (var controlName in this.columnsControls)
            if (this.columnsControls[controlName].isChecked) return true;

        return false;
    }
}

StiMobileDesigner.prototype.GetAllColumnsFromDataSource = function (dataSourceName, relations, relationFullName, columns) {
    var wizardForm = this.options.forms.wizardForm;
    var dataSource = wizardForm.getDataSourceByName(dataSourceName);
    if (dataSource) {
        if (relations == null) relations = dataSource.relations;
        for (var i = 0; i < dataSource.columns.length; i++) {
            columns.push({
                relation: relationFullName,
                column: dataSource.columns[i].correctName || dataSource.columns[i].name,
                columnAlias: dataSource.columns[i].alias
            });
        }
        if (relations) {
            for (var i = 0; i < relations.length; i++) {
                var currentRelation = relations[i];
                this.GetAllColumnsFromDataSource(currentRelation.parentDataSource || currentRelation.name,
                    currentRelation.relations, relationFullName + (relationFullName != "" ? "." : "") + currentRelation.name, columns);
            }
        }
    }
}