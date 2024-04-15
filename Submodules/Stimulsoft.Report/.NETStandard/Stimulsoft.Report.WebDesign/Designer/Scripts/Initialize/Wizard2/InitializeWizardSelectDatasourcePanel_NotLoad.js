
StiMobileDesigner.prototype.InitializeWizardSelectDatasourcePanel = function (templates, form, level) {
    var datasourcePanel = document.createElement("div");
    datasourcePanel.name = name;
    datasourcePanel.className = "wizardStepPanel";
    var jsObject = datasourcePanel.jsObject = this;
    datasourcePanel.level = level;

    //Hint Text
    var createDataHintItem = this.CreateDataHintItem();
    datasourcePanel.appendChild(createDataHintItem);

    createDataHintItem.onclick = function () {
        jsObject.InitializeSelectConnectionForm(function (selectConnectionForm) {
            selectConnectionForm.changeVisibleState(true);
        });
    }

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "500px";
    mainTable.style.marginLeft = "auto";
    mainTable.style.marginRight = "auto";
    mainTable.style.marginTop = "10px";
    datasourcePanel.appendChild(mainTable);

    var newDataSourceButton = this.StandartSmallButton(null, null, this.loc.FormDictionaryDesigner.DataSourceNew, "DataSourceNew.png", null, null, true);
    newDataSourceButton.style.display = "inline-block";
    newDataSourceButton.style.margin = "4px";
    mainTable.addCell(newDataSourceButton);

    var separator = document.createElement("div");
    separator.className = "stiDesignerFormSeparator";
    separator.style.margin = "0 0 4px 0";
    mainTable.addCellInNextRow(separator);

    datasourcePanel.dataTree = this.Tree();
    mainTable.addCellInNextRow(datasourcePanel.dataTree);

    datasourcePanel.update = function (needReset) {
        if (needReset) {
            var tree = datasourcePanel.dataTree;
            tree.clear();
            var datasources = form.getTemplate().datasources;
            var usedDatasources = [];
            for (var i in datasources) {
                var ds = datasources[i];
                var categoryItem;
                for (var j in tree.childNodes) {
                    if (tree.childNodes[j].itemObject == ds.category)
                        categoryItem = tree.childNodes[j];
                }
                if (!categoryItem) {
                    categoryItem = jsObject.DictionaryTreeItem(ds.category, "Connection.png", { name: ds.category, typeItem: "" }, tree);
                    tree.appendChild(categoryItem);
                    categoryItem.setOpening(true);
                }
                categoryItem.addChild(jsObject.DictionaryTreeItem(ds.name, "DataSource.png", ds, tree));
                usedDatasources.push(ds.category + ds.name);
            }

            var databases = jsObject.options.report.dictionary.databases;
            for (var i in databases) {
                var db = databases[i];
                if (db.dataSources.length > 0) {
                    var categoryItem = jsObject.DictionaryTreeItem(db.name, "Connection.png", db, tree);
                    categoryItem.setOpening(true);
                    var found = false;
                    for (var j in db.dataSources) {
                        if (usedDatasources.indexOf(db.name + db.dataSources[j].name) == -1) {
                            db.dataSources[j].category = db.name;
                            categoryItem.addChild(jsObject.DictionaryTreeItem(db.dataSources[j].name, "DataSource.png", db.dataSources[j], tree));
                            found = true;
                        }
                    }
                    if (found) {
                        tree.appendChild(categoryItem);
                    }
                }
            }
            form.datasourceName = null;
            form.enableButtons(form.typeReport != "Label", form.typeReport == "Label", false);
        } else {
            form.enableButtons(form.typeReport != "Label", form.datasourceName != null || form.typeReport == "Label", false);
        }

        createDataHintItem.style.top = "calc(50% - " + createDataHintItem.offsetHeight / 2 + "px)";
        createDataHintItem.style.left = "calc(50% - " + createDataHintItem.offsetWidth / 2 + "px)";
        mainTable.style.display = datasourcePanel.dataTree.childNodes.length > 0 ? "" : "none";
        createDataHintItem.style.display = datasourcePanel.dataTree.childNodes.length > 0 ? "none" : "";
    }

    newDataSourceButton.action = function () {
        jsObject.InitializeSelectConnectionForm(function (selectConnectionForm) {
            selectConnectionForm.changeVisibleState(true);
        });
    };

    datasourcePanel.dataTree.onActionItem = function (item) {
        if (item.itemObject.typeItem != "") {
            form.datasourceName = item.itemObject.name;
            form.datasourceCategory = item.itemObject.category;
            form.enableButtons(form.typeReport != "Label", true, false);
            form.stepLevel = datasourcePanel.level;
        } else {
            form.datasourceName = null;
            form.enableButtons(form.typeReport != "Label", false, false);
        }
    }

    datasourcePanel.getReportOptions = function (options) {
        options.datasourceName = form.datasourceName;
        options.datasourceCategory = form.datasourceCategory;
    }

    datasourcePanel.onShow = function () { };
    datasourcePanel.onHide = function () { };

    this.AddDragAndDropToContainer(datasourcePanel, function (files, content) {
        if (!jsObject.options.report) return;

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

    form.appendStepPanel(datasourcePanel, form.jsObject.loc.Wizards.infoDataSource);

    return datasourcePanel;
}