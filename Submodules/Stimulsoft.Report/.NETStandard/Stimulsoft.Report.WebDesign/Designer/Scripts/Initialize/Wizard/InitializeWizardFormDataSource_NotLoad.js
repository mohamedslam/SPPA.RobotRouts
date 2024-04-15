
StiMobileDesigner.prototype.InitializeWizardFormDataSource = function (wizardForm) {
    var dataSourcePanel = this.WizardFormWorkPanel(wizardForm, "dataSource");
    dataSourcePanel.style.position = "relative";
    dataSourcePanel.style.overflow = "hidden";
    dataSourcePanel.helpTextStandart = "<b>" + this.loc.Wizards.DataSource + "</b><br>" + this.loc.Wizards.infoDataSource;
    dataSourcePanel.helpTextMasterDetail = "<b>" + this.loc.Wizards.DataSource + "</b><br>" + this.loc.Wizards.infoDataSources;
    this.InitializeWizardFormStepItem(wizardForm, dataSourcePanel.name, this.loc.Wizards.DataSource);
    dataSourcePanel.dataSourceControls = {};
    dataSourcePanel.wizardForm = wizardForm;

    var jsObject = this;
    var canDeletedDataSources = !jsObject.options.permissionDataSources || jsObject.options.permissionDataSources.indexOf("All") >= 0 || jsObject.options.permissionDataSources.indexOf("Delete") >= 0;

    var buttonsTable = this.CreateHTMLTable();
    dataSourcePanel.appendChild(buttonsTable);

    var newDataSourceButton = this.FormButton(null, null, this.loc.FormDictionaryDesigner.DataSourceNew);
    newDataSourceButton.style.display = "inline-block";
    newDataSourceButton.style.margin = "12px";
    buttonsTable.addCell(newDataSourceButton);

    var deleteDataSourceButton = this.StandartSmallButton(null, null, null, "Remove.png", this.loc.Cloud.ToolTipDelete, null, true);
    deleteDataSourceButton.style.display = "inline-block";
    deleteDataSourceButton.style.margin = "12px 0 12px 0";
    buttonsTable.addCell(deleteDataSourceButton);

    newDataSourceButton.action = function () {
        this.jsObject.InitializeSelectConnectionForm(function (selectConnectionForm) {
            selectConnectionForm.changeVisibleState(true);
        });
    };

    newDataSourceButton.setEnabled(!jsObject.options.permissionDataSources || jsObject.options.permissionDataSources.indexOf("All") >= 0 || jsObject.options.permissionDataSources.indexOf("Create") >= 0);

    deleteDataSourceButton.action = function () {
        var deleteDataSources = function (dataSources) {
            for (var i = 0; i < dataSources.length; i++) {
                if (wizardForm.dataSources[dataSources[i].name]) {
                    delete wizardForm.dataSources[dataSources[i].name];
                    var itemName = dataSources[i].typeItem == "BusinessObject" && dataSources[i].name ? dataSources[i].name.split(".").reverse() : dataSources[i].name;
                    var item = jsObject.options.dictionaryTree.getItemByNameAndType(itemName, dataSources[i].typeItem);
                    if (item) {
                        if (item.parent && item.parent.childsContainer.childNodes.length == 1) {
                            item.parent.remove();
                        }
                        else {
                            item.remove();
                        }
                    }
                }
            }
            dataSourcePanel.onShow();
        }
        var dataSources = [];
        for (var i = 0; i < dataSourcePanel.itemsContent.childNodes.length; i++) {
            var checkBox = dataSourcePanel.itemsContent.childNodes[i];
            if (checkBox.isChecked) {
                dataSources.push({ name: checkBox.key, nameInSource: checkBox.nameInSource, typeItem: checkBox.typeItem });
            }
        }
        if (dataSources.length == 0) return;
        if (jsObject.options.report) {
            jsObject.SendCommandToDesignerServer("DeleteAllDataSources", { dataSources: dataSources }, function (answer) {
                if (answer.databases) jsObject.options.report.dictionary.databases = answer.databases;
                if (answer.businessObjects) jsObject.options.report.dictionary.businessObjects = answer.businessObjects;
                wizardForm.dataSourcesFromServer = jsObject.GetDataSourcesAndBusinessObjectsFromDictionary(jsObject.options.report.dictionary);
                wizardForm.reportOptions.dataSourcesOrder = [];
                jsObject.ClearAllGalleries();
                jsObject.UpdateStateUndoRedoButtons();
                deleteDataSources(dataSources);
            });
        }
        else {
            deleteDataSources(dataSources);
        }
    }

    dataSourcePanel.itemsContent = document.createElement("div");
    dataSourcePanel.itemsContent.className = "wizardFormDataSourcesContent";
    dataSourcePanel.appendChild(dataSourcePanel.itemsContent);

    var hintText = document.createElement("div");
    dataSourcePanel.appendChild(hintText);
    hintText.className = "wizardFormHintText";
    hintText.innerHTML = this.loc.FormDictionaryDesigner.TextDropDataFileHere;

    var dataSourcesContextMenu = this.InitializeDeleteItemsContextMenu("dataSourcesContextMenu");

    dataSourcePanel.itemsContent.onmouseup = function (event) {
        if (event.button == 2) {
            event.stopPropagation();
            var point = jsObject.FindMousePosOnMainPanel(event);
            dataSourcesContextMenu.items.delete.setEnabled(deleteDataSourceButton.isEnabled);
            dataSourcesContextMenu.items.deleteAll.setEnabled(canDeletedDataSources);
            dataSourcesContextMenu.show(point.xPixels + 3, point.yPixels + 3, "Down", "Right");
        }
        return false;
    }

    dataSourcePanel.oncontextmenu = function (event) {
        return false;
    }

    dataSourcesContextMenu.action = function (item) {
        this.changeVisibleState(false);
        switch (item.key) {
            case "delete": {
                deleteDataSourceButton.action();
                break;
            }
            case "deleteAll": {
                var deleteAll = function () {
                    wizardForm.dataSources = {}
                    wizardForm.dataSourcesFromServer = [];
                    wizardForm.reportOptions.dataSourcesOrder = [];
                    dataSourcePanel.onShow();
                }
                if (jsObject.options.report) {
                    jsObject.SendCommandToDesignerServer("DeleteAllDataSources", {}, function (answer) {
                        if (answer.databases) jsObject.options.report.dictionary.databases = answer.databases;
                        if (answer.businessObjects) jsObject.options.report.dictionary.businessObjects = answer.businessObjects;

                        jsObject.options.dictionaryTree.mainItems["DataSources"].removeAllChilds();
                        jsObject.options.dictionaryTree.mainItems["BusinessObjects"].removeAllChilds();
                        jsObject.options.report.dictionary.databases = answer.databases;
                        jsObject.ClearAllGalleries();
                        jsObject.UpdateStateUndoRedoButtons();
                        deleteAll();
                    });
                }
                else {
                    deleteAll();
                }
                break;
            }
        }
    }

    this.AddDragAndDropToContainer(dataSourcePanel.itemsContent, function (files, content) {
        if (!jsObject.options.report) return;
        if (jsObject.options.permissionDataSources && jsObject.options.permissionDataSources.indexOf("All") < 0 && jsObject.options.permissionDataSources.indexOf("Create") < 0) return;

        var dataExts = ["xls", "xlsx", "csv", "dbf", "json", "xml"];
        var fileName = files[0].name || "Resource";
        var fileExt = fileName.substring(fileName.lastIndexOf(".") + 1).toLowerCase();
        var resourceType;

        if (fileExt == "xml") {
            resourceType = "Xml";
        }
        else if (fileExt == "xsd") {
            resourceType = "Xsd";
        }
        else if (fileExt == "xls" || fileExt == "xlsx") {
            resourceType = "Excel";
        }
        else if (fileExt == "csv") {
            resourceType = "Csv";
        }
        else if (fileExt == "dbf") {
            resourceType = "Dbf";
        }
        else if (fileExt == "json") {
            resourceType = "Json";
        }
        else {
            return;
        }

        var resourceName = jsObject.GetNewName("Resource", null, fileName.substring(0, fileName.lastIndexOf(".")));

        var resource = {};
        resource.mode = "New";
        resource.name = resourceName;
        resource.alias = resourceName;
        resource.type = resourceType;
        resource.loadedContent = jsObject.options.mvcMode ? encodeURIComponent(content) : content;
        resource.haveContent = true;

        jsObject.SendCommandCreateDatabaseFromResource(resource);
    });

    dataSourcePanel.buttonsContent = document.createElement("div");
    dataSourcePanel.buttonsContent.className = "wizardFormColumnsButtonsContent";
    dataSourcePanel.appendChild(dataSourcePanel.buttonsContent);

    var buttonsTable2 = this.CreateHTMLTable();
    dataSourcePanel.buttonsContent.appendChild(buttonsTable2);
    buttonsTable2.className = "wizardFormColumnsButtonsTable stiDesignerClearAllStyles";
    buttonsTable2.addCell().style.width = "100%";

    //Button Relations
    dataSourcePanel.relationsButton = this.FormButton(null, "wizardFormDataSourceRelationsButton", this.loc.PropertyMain.Relations, null);
    dataSourcePanel.relationsButton.style.marginRight = "12px";
    buttonsTable2.addCell(dataSourcePanel.relationsButton);
    dataSourcePanel.relationsButton.dataSourcePanel = dataSourcePanel;
    dataSourcePanel.relationsButton.action = function () {
        var wizardRelationsForm = this.jsObject.options.forms.wizardRelationsForm || this.jsObject.InitializeWizardRelationsForm();
        wizardRelationsForm.changeVisibleState(true);
    }

    dataSourcePanel.onShow = function () {
        this.update();
        hintText.style.top = "calc(50% - " + hintText.offsetHeight / 2 + "px)";
        hintText.style.left = "calc(50% - " + hintText.offsetWidth / 2 + "px)";
    }

    dataSourcePanel.update = function () {
        while (this.itemsContent.childNodes[0]) this.itemsContent.removeChild(this.itemsContent.childNodes[0]);
        this.dataSourceControls = {};
        var dataSources = this.wizardForm.dataSourcesFromServer;
        var dataSourcesOrder = this.wizardForm.reportOptions.dataSourcesOrder;

        if (this.wizardForm.typeReport == "MasterDetail") {
            for (var i = 0; i < dataSourcesOrder.length; i++) {
                var alias = null;
                var nameInSource = null;
                var typeItem = null;
                for (var k = 0; k < dataSources.length; k++) {
                    if (dataSources[k].typeItem == "BusinessObject" && dataSources[k].fullName == dataSourcesOrder[i]) {
                        alias = dataSources[k].alias;
                        nameInSource = dataSources[k].fullName;
                        typeItem = "BusinessObject";
                        break;
                    }
                    else if (dataSources[k].name == dataSourcesOrder[i]) {
                        alias = dataSources[k].alias;
                        nameInSource = dataSources[k].nameInSource;
                        typeItem = "DataSource";
                        break;
                    }
                }
                this.createItem(dataSourcesOrder[i], alias, nameInSource, typeItem);
            }
        }
        for (var i = 0; i < dataSources.length; i++) {
            if (!this.wizardForm.dataSources[dataSources[i].typeItem == "BusinessObject" ? dataSources[i].fullName : dataSources[i].name] ||
                this.wizardForm.typeReport == "Standart") {
                if (dataSources[i].typeItem == "BusinessObject") {
                    this.createItem(dataSources[i].fullName, dataSources[i].alias, dataSources[i].fullName, "BusinessObject");
                }
                else {
                    this.createItem(dataSources[i].name, dataSources[i].alias, dataSources[i].nameInSource, "DataSource");
                }
            }
        }

        var countSelected = this.jsObject.GetCountObjects(wizardForm.dataSources);
        this.wizardForm.buttonNext.setEnabled(countSelected != 0);
        deleteDataSourceButton.setEnabled(countSelected != 0 && canDeletedDataSources);
        this.relationsButton.setEnabled(this.jsObject.GetCountObjects(wizardForm.reportOptions.relations) != 0);
        this.buttonsContent.style.display = wizardForm.typeReport == "Standart" ? "none" : "";
        this.itemsContent.className = wizardForm.typeReport == "Standart" ? "wizardFormDataSourcesContentWithoutButtons" : "wizardFormDataSourcesContent";
        hintText.style.display = this.itemsContent.childNodes.length == 0 && this.jsObject.options.report ? "" : "none";
    }

    dataSourcePanel.createItem = function (dataSourceName, dataSourceAlias, nameInSource, typeItem) {
        var dataSources = this.wizardForm.dataSourcesFromServer;
        var checkBox = this.jsObject.WizardFormCheckBox(null, (dataSourceName == dataSourceAlias || !dataSourceAlias)
            ? dataSourceName : dataSourceName + " [" + dataSourceAlias + "]", dataSourceName);
        checkBox.style.position = "relative";
        checkBox.nameInSource = nameInSource;
        checkBox.typeItem = typeItem;
        this.itemsContent.appendChild(checkBox);
        this.dataSourceControls[dataSourceName] = checkBox;
        checkBox.dataSourcePanel = this;
        checkBox.setChecked(this.wizardForm.dataSources[dataSourceName])

        checkBox.onmouseup = function (event) {
            this.onclick();
            dataSourcePanel.itemsContent.onmouseup(event);
        }

        checkBox.action = function () {
            if (wizardForm.typeReport == "Standart") {
                wizardForm.reportOptions.dataSourcesOrder = [];
                wizardForm.reportOptions.dataSourcesOrder.push(this.key);
                if (!this.isChecked) { this.setChecked(true); return; }
                wizardForm.buttonNext.setEnabled(true);
                deleteDataSourceButton.setEnabled(true && canDeletedDataSources);
                wizardForm.dataSources = {}
                wizardForm.dataSources[this.key] = this.jsObject.EmptyDataSourceObject(this.typeItem);
                for (var controlName in this.dataSourcePanel.dataSourceControls) this.dataSourcePanel.dataSourceControls[controlName].setChecked(this.key == controlName);
            }
            else {
                var numElement = this.jsObject.GetElementNumberInArray(this.key, wizardForm.reportOptions.dataSourcesOrder);
                if (numElement != -1) wizardForm.reportOptions.dataSourcesOrder.splice(numElement, 1);
                if (this.isChecked) {
                    wizardForm.reportOptions.dataSourcesOrder.push(this.key);
                    wizardForm.dataSources[this.key] = this.jsObject.EmptyDataSourceObject(this.typeItem);

                    var relation = this.dataSourcePanel.dataSourceHaveRelationOnMainDataSource(this.key);
                    if (relation) wizardForm.reportOptions.relations[this.key] = { "name": relation.name, "nameInSource": relation.nameInSource, "checked": true };
                }
                else {
                    if (wizardForm.dataSources[this.key]) {
                        delete wizardForm.dataSources[this.key];
                        delete wizardForm.reportOptions.relations[this.key];
                        for (var relationKey in wizardForm.reportOptions.relations)
                            if (wizardForm.reportOptions.relations[relationKey].name == this.key) delete wizardForm.reportOptions.relations[relationKey];
                    }
                }
                var countSelected = this.jsObject.GetCountObjects(wizardForm.dataSources);
                wizardForm.buttonNext.setEnabled(countSelected);
                deleteDataSourceButton.setEnabled(countSelected && canDeletedDataSources);
                this.dataSourcePanel.update();
            }
        }
    }

    dataSourcePanel.dataSourceHaveRelationOnMainDataSource = function (dataSourceName) {
        if (this.wizardForm.reportOptions.dataSourcesOrder.length <= 1) return false;
        var mainDataSourceName = wizardForm.reportOptions.dataSourcesOrder[0];
        var dataSource = this.wizardForm.getDataSourceByName(dataSourceName);
        if (dataSource && dataSource.relations) {
            var relations = dataSource.relations;
            for (var i = 0; i < relations.length; i++) {
                if (relations[i].parentDataSource == mainDataSourceName) return relations[i];
            }
        }
        return false;
    }
}


StiMobileDesigner.prototype.EmptyDataSourceObject = function (typeItem) {
    return {
        "columns": [],
        "columnsText": {},
        "sort": [],
        "filterMode": "And",
        "filterOn": true,
        "filterEngine": "ReportEngine",
        "filters": [],
        "groups": [],
        "totals": {},
        "typeItem": typeItem
    }
}