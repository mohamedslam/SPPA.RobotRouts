
StiMobileDesigner.prototype.InitializeWizardFormColumnsOrder = function (wizardForm) {
    var columnsOrderPanel = this.WizardFormWorkPanel(wizardForm, "columnsOrder");
    columnsOrderPanel.style.overflow = "hidden";
    columnsOrderPanel.helpTextStandart = "<b>" + this.loc.Wizards.ColumnsOrder + "</b><br>" + this.loc.Wizards.infoColumnsOrder;
    this.InitializeWizardFormStepItem(wizardForm, columnsOrderPanel.name, this.loc.Wizards.ColumnsOrder);
    columnsOrderPanel.wizardForm = wizardForm;
    columnsOrderPanel.selectedColumnsItem = null;
    columnsOrderPanel.columnsControls = {};

    var columnsOrderTable = this.CreateHTMLTable();
    columnsOrderPanel.appendChild(columnsOrderTable);
    columnsOrderPanel.itemsContent = document.createElement("div");
    columnsOrderPanel.itemsContent.className = "wizardFormColumnsOrderItemsContent";
    columnsOrderTable.addCell(columnsOrderPanel.itemsContent);
    columnsOrderPanel.buttonsContent = document.createElement("div");
    columnsOrderPanel.buttonsContent.className = "wizardFormColumnsOrderButtonsContent";
    columnsOrderTable.addCell(columnsOrderPanel.buttonsContent).style.verticalAlign = "top";

    //Button Up
    columnsOrderPanel.upButton = this.StandartSmallButton("wizardFormColumnOrderUpButton", null, null, "Arrows.ArrowUpBlue.png", null, null);
    columnsOrderPanel.upButton.innerTable.style.width = "100%";
    columnsOrderPanel.buttonsContent.appendChild(columnsOrderPanel.upButton);
    columnsOrderPanel.upButton.columnsOrderPanel = columnsOrderPanel;
    columnsOrderPanel.upButton.action = function () {
        this.columnsOrderPanel.columnMove("up");
        this.columnsOrderPanel.update();
    }

    //Button Down
    columnsOrderPanel.downButton = this.StandartSmallButton("wizardFormColumnOrderDownButton", null, null, "Arrows.ArrowDownBlue.png", null, null);
    columnsOrderPanel.downButton.innerTable.style.width = "100%";
    columnsOrderPanel.buttonsContent.appendChild(columnsOrderPanel.downButton);
    columnsOrderPanel.downButton.columnsOrderPanel = columnsOrderPanel;
    columnsOrderPanel.downButton.action = function () {
        this.columnsOrderPanel.columnMove("down");
        this.columnsOrderPanel.update();
    }

    columnsOrderPanel.onShow = function () {
        this.selectedColumnsItem = null;
        this.upButton.setEnabled(false);
        this.downButton.setEnabled(false);
        this.update();
    }

    columnsOrderPanel.update = function () {
        this.columnsControls = {};
        while (this.itemsContent.childNodes[0]) this.itemsContent.removeChild(this.itemsContent.childNodes[0]);
        for (var selDataSourceName in this.wizardForm.dataSources) {
            var selDataSource = this.wizardForm.dataSources[selDataSourceName];

            var selColumns = selDataSource.columns;
            if (selColumns.length > 0) this.itemsContent.appendChild(this.jsObject.WizardFormSeparator(selDataSourceName));

            for (var i = 0; i < selColumns.length; i++) {
                var columnItemName = "WizardFormColumnOrder" + selDataSourceName + selColumns[i];
                var columnItem = this.jsObject.WizardFormColumnsOrderItem(columnItemName, selDataSource.columnsText[selColumns[i]] || selColumns[i]);
                columnItem.key = selColumns[i];
                this.columnsControls[columnItemName] = columnItem;
                columnItem.dataSourceName = selDataSourceName;
                columnItem.columnsOrderPanel = this;
                this.itemsContent.appendChild(columnItem);

                columnItem.action = function () {
                    for (var itemName in this.columnsOrderPanel.columnsControls) this.columnsOrderPanel.columnsControls[itemName].setSelected(false);
                    this.setSelected(true);
                    var currentDataSource = this.columnsOrderPanel.wizardForm.dataSources[this.dataSourceName];
                    var currentColumns = currentDataSource.columns;
                    this.columnsOrderPanel.upButton.setEnabled(this.jsObject.GetElementNumberInArray(this.key, currentColumns) > 0);
                    this.columnsOrderPanel.downButton.setEnabled(this.jsObject.GetElementNumberInArray(this.key, currentColumns) < currentColumns.length - 1);
                    this.columnsOrderPanel.selectedColumnsItem = this;
                }

                if (this.selectedColumnsItem && this.selectedColumnsItem.name == columnItemName) columnItem.action();
            }
        }
    }

    columnsOrderPanel.columnMove = function (direction) {
        var currentDataSource = this.wizardForm.dataSources[this.selectedColumnsItem.dataSourceName];
        var currentColumns = currentDataSource.columns;
        var currentPos = this.jsObject.GetElementNumberInArray(this.selectedColumnsItem.key, currentColumns);
        if ((currentPos > 0 && direction == "up") || (currentPos < currentColumns.length - 1 && direction == "down")) {
            var newPos = (direction == "up") ? currentPos - 1 : currentPos + 1;
            var temp = currentColumns[newPos];
            currentColumns[newPos] = currentColumns[currentPos];
            currentColumns[currentPos] = temp;
        }
    }
}

