
StiMobileDesigner.prototype.DictionaryTree = function () {
    var jsObject = this;
    var dictionaryTree = this.Tree();
    dictionaryTree.style.margin = "10px";
    this.options.dictionaryTree = dictionaryTree;

    dictionaryTree.clear = function () {
        if (this.mainItems) {
            var mainItemsNames = ["DataSources", "BusinessObjects", "Variables", "Resources", "Images", "RichTexts", "SubReports"];
            for (var i = 0; i < mainItemsNames.length; i++) {
                if (this.mainItems[mainItemsNames[i]]) {
                    this.removeChild(this.mainItems[mainItemsNames[i]]);
                    this.mainItems[mainItemsNames[i]] = null;
                }
            }
            this.mainItems["SystemVariables"].style.display = "none";
            this.mainItems["Functions"].style.display = "none";
            this.mainItems["Formats"].style.display = "none";
            this.mainItems["HtmlTags"].style.display = "none";
        }
        this.items = {};
    }

    dictionaryTree.build = function (dictionary, buildOnlyOneLevel) {
        if (dictionary == null) return;
        this.clear();

        if (!this.mainItems) {
            this.mainItems = {};
            this.mainItems["SystemVariables"] = jsObject.DictionaryTreeItem(jsObject.loc.PropertyMain.SystemVariables, "SystemVariables.png", { name: "SystemVariablesMainItem", typeItem: "SystemVariablesMainItem" });
            this.mainItems["Functions"] = jsObject.DictionaryTreeItem(jsObject.loc.PropertyMain.Functions, "Function.png", { name: "FunctionsMainItem", typeItem: "FunctionsMainItem" });
            this.mainItems["Formats"] = jsObject.DictionaryTreeItem(jsObject.loc.PropertyMain.Format, "Dictionary.Format.png", { name: "FormatsMainItem", typeItem: "FormatsMainItem" });
            this.mainItems["HtmlTags"] = jsObject.DictionaryTreeItem(jsObject.loc.PropertyMain.HtmlTags, "Dictionary.HtmlTag.png", { name: "HtmlTagsMainItem", typeItem: "HtmlTagsMainItem" });

            this.appendChild(this.mainItems["SystemVariables"]);
            this.appendChild(this.mainItems["Functions"]);
            this.appendChild(this.mainItems["Formats"]);
            this.appendChild(this.mainItems["HtmlTags"]);

            this.addSystemVariables(dictionary.systemVariables, this.mainItems["SystemVariables"]);
            this.addFunctions(dictionary.functions, this.mainItems["Functions"]);
            this.addFormats(this.mainItems["Formats"]);
            this.addHtmlTags(this.mainItems["HtmlTags"]);
        }
        else {
            this.mainItems["SystemVariables"].style.display = "";
            this.mainItems["Functions"].style.display = "";
        }

        this.mainItems["Formats"].style.display = "none";
        this.mainItems["HtmlTags"].style.display = "none";

        this.mainItems["Resources"] = jsObject.DictionaryTreeItem(jsObject.loc.PropertyMain.Resources, "Resources.Resource.png", { name: "ResourcesMainItem", typeItem: "ResourcesMainItem" });
        this.appendChild(this.mainItems["Resources"]);

        this.mainItems["DataSources"] = jsObject.DictionaryTreeItem(jsObject.loc.PropertyMain.DataSources, "DataSource.png", { name: "DataSourceMainItem", typeItem: "DataSourceMainItem" });
        this.mainItems["BusinessObjects"] = jsObject.DictionaryTreeItem(jsObject.loc.Report.BusinessObjects, "BusinessObject.png", { name: "BusinessObjectMainItem", typeItem: "BusinessObjectMainItem" });
        this.mainItems["Variables"] = jsObject.DictionaryTreeItem(jsObject.loc.PropertyMain.Variables, "Variable.png", { name: "VariablesMainItem", typeItem: "VariablesMainItem" });

        var mainItemsNames = ["DataSources", "BusinessObjects", "Variables"];
        for (var i = 0; i < mainItemsNames.length; i++) {
            this.insertBefore(this.mainItems[mainItemsNames[i]], this.mainItems["SystemVariables"]);
        }

        this.addTreeItems(dictionary.databases, this.mainItems["DataSources"], buildOnlyOneLevel);
        this.addTreeItems(dictionary.businessObjects, this.mainItems["BusinessObjects"]);
        this.addTreeItems(dictionary.variables, this.mainItems["Variables"]);
        this.addTreeItems(dictionary.resources, this.mainItems["Resources"]);
        this.mainItems["DataSources"].setSelected();

        if (jsObject.options.isJava || jsObject.options.jsMode || jsObject.options.designerSpecification != "Developer") {
            this.mainItems.BusinessObjects.style.display = "none";
        }

        //Cloud Server Mode
        if (dictionary.attachedItems) {
            mainItemsNames = ["Image", "RichText", "SubReport"];
            for (var i = 0; i < mainItemsNames.length; i++) {
                var categoryName = mainItemsNames[i] + "s";
                if (dictionary.attachedItems[mainItemsNames[i]]) {
                    var captionCategory = jsObject.options.cloudReportsClient
                        ? jsObject.options.cloudReportsClient.loc.ReportTemplate["Category" + categoryName] : categoryName;
                    this.mainItems[categoryName] = jsObject.DictionaryTreeItem(captionCategory, "Cloud" + mainItemsNames[i] + ".png", { name: categoryName + "MainItem", typeItem: categoryName + "MainItem" });
                    this.appendChild(this.mainItems[categoryName]);
                    this.addTreeItems(dictionary.attachedItems[mainItemsNames[i]], this.mainItems[categoryName]);
                }
            }
        }
    }

    dictionaryTree.getCloudItemByKey = function (itemType, key) {
        var mainItem = this.mainItems[itemType + "s"];
        if (mainItem) {
            for (var itemKey in mainItem.childs) {
                if (mainItem.childs[itemKey].itemObject.key == key)
                    return mainItem.childs[itemKey];
            }
        }
        return null;
    }

    //Add Tree Items
    dictionaryTree.addTreeItems = function (collection, parentItem, buildOnlyOneLevel) {
        if (collection) {
            //Sort only connections
            if (jsObject.options.dictionarySorting == "ascending" || parentItem == this.mainItems["DataSources"]) {
                collection.sort(jsObject.SortByName);
            }
            else if (jsObject.options.dictionarySorting == "descending") {
                collection.sort(jsObject.SortByNameDescending);
            }

            for (var i = 0; i < collection.length; i++) {
                if (collection[i].typeItem == "Category") {
                    var categoryItem = parentItem.addChild(jsObject.DictionaryTreeItemFromObject(jsObject.CategoryObject(collection[i].name, collection[i].requestFromUser, collection[i].readOnly)));
                    this.addTreeItems(collection[i].categoryItems, categoryItem);
                    continue;
                }

                var childItem = parentItem.addChild(jsObject.DictionaryTreeItemFromObject(collection[i]));
                var childCollection = [];

                switch (collection[i].typeItem) {
                    case "DataBase": { childCollection = collection[i].dataSources; break; }
                    case "DataSource": { childCollection = collection[i].relations; break; }
                    case "Relation": { childCollection = collection[i].relations; break; }
                    case "BusinessObject":
                        {
                            childItem.itemObject.fullName = childItem.getBusinessObjectStringFullName();
                            childCollection = collection[i].businessObjects;
                            break;
                        }
                }

                if (buildOnlyOneLevel) {
                    var showOpenIcon = childCollection.length > 0;

                    if (!showOpenIcon && childItem.itemObject.typeItem == "Relation") {
                        var dataSource = jsObject.GetDataSourceByNameFromDictionary(childItem.itemObject.parentDataSource);
                        if (dataSource && ((dataSource.columns && dataSource.columns.length > 0) || (dataSource.parameters && dataSource.parameters.length > 0))) {
                            showOpenIcon = true;
                        }
                    }

                    if (!showOpenIcon &&
                        ((childItem.itemObject.columns && childItem.itemObject.columns.length > 0) ||
                            (childItem.itemObject.parameters && childItem.itemObject.parameters.length > 0))) {
                        showOpenIcon = true;
                    }

                    if (childItem.iconOpening && showOpenIcon) {
                        childItem.iconOpening.style.visibility = "visible";
                        childItem.buildChildsNotCompleted = true;
                        childItem.childCollection = childCollection;
                    }
                }
                else {
                    this.addTreeItems(childCollection, childItem);
                }
            }

            if (parentItem.itemObject["columns"] || parentItem.itemObject["parameters"]) {
                if (parentItem.itemObject["columns"]) this.addColumns(parentItem, parentItem.itemObject["columns"]);
                if (parentItem.itemObject["parameters"]) this.addParameters(parentItem, parentItem.itemObject["parameters"]);
                return;
            }

            if (parentItem.itemObject.typeItem == "Relation") {
                var dataSource = jsObject.GetDataSourceByNameFromDictionary(parentItem.itemObject.parentDataSource);
                if (dataSource) this.addColumns(parentItem, dataSource.columns);
                if (dataSource) this.addParameters(parentItem, dataSource.parameters);
            }
        }
    }

    /* --------------- System Variables Tree ---------------------- */
    dictionaryTree.addSystemVariables = function (systemVariables, parentItem) {
        for (var i = 0; i < systemVariables.length; i++) {
            var item = jsObject.DictionaryTreeItem(systemVariables[i], "SystemVariable" + systemVariables[i] + ".png",
                {
                    typeItem: "SystemVariable", name: systemVariables[i],
                    typeIcon: "SystemVariable" + systemVariables[i]
                });
            parentItem.addChild(item);
        }
    }

    /* --------------- Formats Tree ---------------------- */
    dictionaryTree.addFormats = function (parentItem) {
        var formats = jsObject.GetFormatsDictionaryItems();

        for (var i = 0; i < formats.length; i++) {
            var formatItem = jsObject.DictionaryTreeItem(formats[i].caption, formats[i].imageName, { typeItem: "Format", name: formats[i].name, key: formats[i].key }, dictionaryTree);
            parentItem.addChild(formatItem);
        }
    }

    /* --------------- HtmlTags Tree ---------------------- */
    dictionaryTree.addHtmlTags = function (parentItem) {
        var htmlTags = jsObject.GetHtmlTagsDictionaryItems();

        for (var i = 0; i < htmlTags.length; i++) {
            var htmlTagsItem = jsObject.DictionaryTreeItem(htmlTags[i].caption, "Dictionary.HtmlTag.png", { typeItem: "HtmlTag", name: htmlTags[i].name, key: htmlTags[i].key }, dictionaryTree);
            parentItem.addChild(htmlTagsItem);
        }
    }

    /* --------------- Functions Tree ---------------------- */
    dictionaryTree.addFunctions = function (functions, parentItem) {
        for (var i = 0; i < functions.length; i++) {
            if (functions[i].typeItem == "FunctionsCategory") {
                var categoryItem = jsObject.DictionaryTreeItem(functions[i].name, functions[i].typeIcon + ".png", functions[i], dictionaryTree);
                parentItem.addChild(categoryItem);
                dictionaryTree.addFunctions(functions[i].items, categoryItem);
            }
            else {
                var functionItem = jsObject.DictionaryTreeItem(functions[i].caption, functions[i].typeIcon + ".png", functions[i], dictionaryTree);
                parentItem.addChild(functionItem);
            }
        }
    }

    /* --------------- Variables Tree ---------------------- */
    dictionaryTree.addVariable = function (variableObject) {
        var parentItem = this.getCurrentVariableParent();
        var variableItem = jsObject.DictionaryTreeItemFromObject(variableObject);
        parentItem.addChild(variableItem);
        variableItem.setSelected();
        variableItem.openTree();
    }

    dictionaryTree.editVariable = function (variableObject) {
        var editVariableForm = jsObject.options.forms.editVariableForm;
        var variableItem = editVariableForm && editVariableForm.editableDictionaryItem ? editVariableForm.editableDictionaryItem : this.selectedItem;

        if (variableItem) {
            variableItem.itemObject = variableObject;
            variableItem.repaint();
        }
    }

    dictionaryTree.createVariablesCategory = function (answer) {
        var categories = jsObject.options.report.dictionary.variables["Categories"];
        var canCreate = true;
        if (categories)
            for (var nameCategory in categories)
                if (answer.name == nameCategory) canCreate = false;

        if (canCreate) {
            var categoryObj = jsObject.CategoryObject(answer.name, answer.requestFromUser, answer.readOnly);
            var categoryItem = jsObject.DictionaryTreeItemFromObject(categoryObj);
            this.mainItems["Variables"].addChild(categoryItem);
            categoryItem.setSelected();
            categoryItem.openTree();
        }
    }

    /* --------------- Resources Tree ---------------------- */
    dictionaryTree.addResource = function (resourceObject) {
        var resourceItem = jsObject.DictionaryTreeItemFromObject(resourceObject);
        this.mainItems["Resources"].addChild(resourceItem);
        resourceItem.setSelected();
        resourceItem.openTree();
    }

    dictionaryTree.editResource = function (resourceObject) {
        this.selectedItem.itemObject = resourceObject;
        this.selectedItem.repaint();
    }

    /* --------------- BusinessObject Tree ---------------------- */

    dictionaryTree.deleteBusinessObject = function () {
        if (!this.selectedItem) return;
        var businessObjectParentItem = this.selectedItem.parent;
        this.selectedItem.remove();
        if (jsObject.GetCountObjects(businessObjectParentItem.childs) == 0 && businessObjectParentItem.itemObject.typeItem == "Category")
            businessObjectParentItem.remove();
    }

    dictionaryTree.addBusinessObject = function (businessObjectObject, parentBusinessObjectFullName) {
        var parentItem = parentBusinessObjectFullName == null ? this.mainItems["BusinessObjects"] : this.getItemByNameAndType(parentBusinessObjectFullName, "BusinessObject");
        if (!parentItem) return;
        var category = businessObjectObject.category;
        if (category != "") {
            for (var itemKey in parentItem.childs)
                if (parentItem.childs[itemKey].itemObject.typeItem == "Category" && parentItem.childs[itemKey].itemObject.name == category) {
                    parentItem = parentItem.childs[itemKey];
                    break;
                }
            if (parentItem.itemObject.typeItem == "BusinessObjectMainItem") {
                var categoryItem = jsObject.DictionaryTreeItemFromObject(jsObject.CategoryObject(category));
                parentItem.addChild(categoryItem);
                parentItem = categoryItem;
            }
        }

        var businessObjectItem = jsObject.DictionaryTreeItemFromObject(businessObjectObject);

        if (parentBusinessObjectFullName != null) {
            this.removeColumns(parentItem);
            parentItem.addChild(businessObjectItem);
            var parentBusinessObjectInDictionary = jsObject.GetBusinessObjectByNameFromDictionary(parentBusinessObjectFullName);
            if (parentBusinessObjectInDictionary) this.addColumns(parentItem, parentBusinessObjectInDictionary.columns);
        }
        else {
            parentItem.addChild(businessObjectItem);
        }
        this.addTreeItems(businessObjectObject.businessObjects, businessObjectItem);
        businessObjectItem.setSelected();
        businessObjectItem.openTree();
    }

    dictionaryTree.editBusinessObject = function (businessObjectNewObject) {
        var editDataSourceForm = jsObject.options.forms.editDataSourceForm;
        var currBusinessObjectItem = editDataSourceForm && editDataSourceForm.editableDictionaryItem ? editDataSourceForm.editableDictionaryItem : this.selectedItem;

        var businessObjectOldObject = currBusinessObjectItem.itemObject;
        if (businessObjectOldObject.category == businessObjectNewObject.category) {
            currBusinessObjectItem.repaint(businessObjectNewObject);
            this.updateColumns(currBusinessObjectItem, businessObjectNewObject.columns);
        }
        else {
            this.deleteBusinessObject();
            this.addBusinessObject(businessObjectNewObject);
        }
    }

    dictionaryTree.editBusinessObjectCategory = function (answer) {
        var oldCategoryName = answer.oldName;
        var newCategoryName = answer.name;
        var categoryItem = this.getItemByNameAndType(oldCategoryName, "Category");
        if (categoryItem) {
            categoryItem.itemObject.name = newCategoryName;
            categoryItem.repaint();
            for (var itemKey in categoryItem.childs) {
                if (categoryItem.childs[itemKey].itemObject.typeItem == "BusinessObject")
                    categoryItem.childs[itemKey].itemObject.category = newCategoryName;
            }
        }
    }

    /* --------------- DataSources Tree ---------------------- */

    /* Connection */
    dictionaryTree.addConnectionToTree = function (connectionObject) {
        var connectionItem = jsObject.DictionaryTreeItemFromObject(connectionObject);
        this.mainItems["DataSources"].addChild(connectionItem);
        connectionItem.setSelected();
        connectionItem.openTree();
    }

    dictionaryTree.setConnectionItemFail = function (connectionItem) {
        connectionItem.itemObject.typeIcon = "ConnectionFail";
        connectionItem.itemObject.nameInSource = connectionItem.itemObject.name;
        connectionItem.repaint();
    }

    dictionaryTree.createOrEditConnection = function (answer) {
        var newConnectionItem = this.mainItems["DataSources"].getChildByName(answer.itemObject.name);
        if (answer.mode == "Edit") {
            var editConnectionForm = jsObject.options.forms.editConnectionForm;
            var currConnectionItem = editConnectionForm && editConnectionForm.editableDictionaryItem ? editConnectionForm.editableDictionaryItem : this.selectedItem;

            if (newConnectionItem) {
                if (currConnectionItem.itemObject.name != newConnectionItem.itemObject.name) {
                    newConnectionItem.repaint(answer.itemObject);
                    if (jsObject.GetCountObjects(currConnectionItem.childs) == 0)
                        currConnectionItem.remove();
                    else
                        this.setConnectionItemFail(currConnectionItem);
                }
                else {
                    newConnectionItem.repaint(answer.itemObject);
                }
            }
            else {
                currConnectionItem.removeAllChilds();
                var databaseInDictionary = jsObject.GetObjectByPropertyValueFromCollection(answer.databases, "name", answer.itemObject.name);
                if (databaseInDictionary) {
                    var newItemObject = jsObject.CopyObject(databaseInDictionary);
                    currConnectionItem.repaint(newItemObject);
                    dictionaryTree.addTreeItems(newItemObject.dataSources, currConnectionItem);
                }
            }
        }
        else {
            if (newConnectionItem)
                newConnectionItem.repaint(answer.itemObject);
            else
                this.addConnectionToTree(answer.itemObject);
        }
    }

    /* DataSource */
    dictionaryTree.deleteDataSource = function () {
        var currentItem = this.selectedItem;
        //check nested relations
        for (var itemKey in this.items) {
            if (this.items[itemKey].itemObject.typeItem == "Relation" && this.items[itemKey].itemObject.parentDataSource == currentItem.itemObject.name) {
                var relationItem = this.items[itemKey];
                var parentItem = relationItem.parent;
                if (parentItem && parentItem.itemObject.relations) {
                    for (var k = 0; k < parentItem.itemObject.relations.length; k++) {
                        if (relationItem.itemObject.nameInSource == parentItem.itemObject.relations[k].nameInSource) {
                            parentItem.itemObject.relations.splice(k, 1);
                        }
                    }
                }
                relationItem.remove();
            }
        }
        //check parent element
        var dataBaseItem = currentItem.parent;
        currentItem.remove();
        if (dataBaseItem.itemObject.typeItem == "DataBase" && !dataBaseItem.itemObject.typeConnection && jsObject.GetCountObjects(dataBaseItem.childs) == 0) {
            dataBaseItem.remove();
        }
    }

    dictionaryTree.addDataSource = function (dataSourceObject) {
        var connectionName = this.getConnectionNameFromNameInSource(dataSourceObject);
        var connectionForDataSource = jsObject.options.dictionaryTree.mainItems["DataSources"].getChildByName(connectionName);
        var dataSourceItem = jsObject.DictionaryTreeItemFromObject(dataSourceObject);

        if (connectionForDataSource.buildChildsNotCompleted) connectionForDataSource.completeBuildTree(true);

        if (connectionForDataSource) {
            connectionForDataSource.addChild(dataSourceItem);
        }
        else {
            var newConnectionObject = jsObject.ConnectionObject();
            newConnectionObject.typeIcon = dataSourceObject.typeDataSource == "StiDataTransformation" ? "DataTransformationCategory" : "ConnectionFail";
            newConnectionObject.name = connectionName;
            newConnectionObject.alias = connectionName;
            newConnectionObject.nameInSource = connectionName;
            var newConnectionItem = jsObject.DictionaryTreeItemFromObject(newConnectionObject);
            this.mainItems["DataSources"].addChild(newConnectionItem);
            newConnectionItem.addChild(dataSourceItem);
        }

        this.updateColumns(dataSourceItem, dataSourceObject.columns);
        this.updateParameters(dataSourceItem, dataSourceObject.parameters);
        dataSourceItem.setSelected();
        dataSourceItem.openTree();
    }

    dictionaryTree.editDataSource = function (dataSourceNewObject) {
        var editDataSourceForm = dataSourceNewObject.typeDataSource == "StiVirtualSource"
            ? jsObject.options.forms.editDataSourceFromOtherDatasourcesForm
            : dataSourceNewObject.typeDataSource == "StiDataTransformation"
                ? jsObject.options.forms.editDataTransformationForm
                : jsObject.options.forms.editDataSourceForm;

        var currDataSourceItem = editDataSourceForm && editDataSourceForm.editableDictionaryItem ? editDataSourceForm.editableDictionaryItem : this.selectedItem;

        if (currDataSourceItem) {
            if (currDataSourceItem.buildChildsNotCompleted) currDataSourceItem.completeBuildTree();

            var dataSourceOldObject = currDataSourceItem.itemObject;
            if (dataSourceOldObject.nameInSource == dataSourceNewObject.nameInSource) {
                currDataSourceItem.repaint(dataSourceNewObject);
                this.updateColumns(currDataSourceItem, dataSourceNewObject.columns);
                this.updateParameters(currDataSourceItem, dataSourceNewObject.parameters);
                for (var itemKey in this.items) {
                    var itemObject = this.items[itemKey].itemObject;
                    if (itemObject.typeItem == "Relation" && (itemObject.parentDataSource == dataSourceOldObject.name || itemObject.childDataSource == dataSourceOldObject.name)) {
                        itemObject[itemObject.parentDataSource == dataSourceOldObject.name ? "parentDataSource" : "childDataSource"] = dataSourceNewObject.name;
                    }
                }
            }
            else {
                this.deleteDataSource();
                this.addDataSource(dataSourceNewObject);
            }
        }
    }

    /* Relations */
    dictionaryTree.addRelation = function (relationObject, answer) {
        var mainItemDataSources = jsObject.options.dictionaryTree.mainItems["DataSources"];

        for (var childId in mainItemDataSources.childs) {
            if (mainItemDataSources.childs[childId].buildChildsNotCompleted) {
                mainItemDataSources.childs[childId].completeBuildTree(true);
            }
        }

        var childDataSource = jsObject.GetDataSourceByNameFromDictionary(relationObject.childDataSource);
        var childDataSourceItem = this.getItemByNameAndType(relationObject.childDataSource, "DataSource");
        var parentDataSourceItem = this.getItemByNameAndType(relationObject.parentDataSource, "DataSource");
        if (!childDataSourceItem || !parentDataSourceItem || !childDataSource) return;
        var selectedItem = null;


        for (var itemKey in this.items) {
            if ((this.items[itemKey].itemObject.typeItem == "Relation" && this.items[itemKey].itemObject.parentDataSource == relationObject.childDataSource) || (this.items[itemKey] == childDataSourceItem)) {
                var relationItem = jsObject.DictionaryTreeItemFromObject(relationObject);
                if (this.items[itemKey].buildChildsNotCompleted) this.items[itemKey].completeBuildTree();
                this.removeColumns(this.items[itemKey]);
                this.removeParameters(this.items[itemKey]);
                this.items[itemKey].addChild(relationItem);
                if (this.items[itemKey].itemObject.relations) {
                    this.items[itemKey].itemObject.relations.push(relationObject);
                }
                this.addColumns(this.items[itemKey], childDataSource.columns);
                this.addParameters(this.items[itemKey], childDataSource.parameters);
                this.addTreeItems(relationObject.relations, relationItem);
                if (this.items[itemKey] == childDataSourceItem) selectedItem = relationItem;
            }
        }
        if (answer && answer.databases)
            this.updateIcons(answer.databases);
        if (selectedItem) { selectedItem.setSelected(); selectedItem.openTree(); }
    }

    dictionaryTree.editRelation = function (answer) {
        if (answer.changedChildDataSource) {
            this.selectedItem.remove();
            for (var itemKey in this.items)
                if (this.items[itemKey].itemObject.typeItem == "Relation" && this.items[itemKey].itemObject.nameInSource == answer.oldNameInSource)
                    this.items[itemKey].remove();
            this.addRelation(answer.itemObject);
        }
        else {
            var selectedItem = this.selectedItem;
            var parentDataSource = jsObject.GetDataSourceByNameFromDictionary(answer.itemObject.parentDataSource);
            var columns = parentDataSource ? parentDataSource.columns : null;
            var parameters = parentDataSource ? parentDataSource.parameters : null;
            if (selectedItem.buildChildsNotCompleted) selectedItem.completeBuildTree();
            if (!columns && !parameters) return;
            this.selectedItem.repaint(answer.itemObject);
            this.updateColumns(this.selectedItem, columns);
            this.updateParameters(this.selectedItem, parameters);
            for (var itemKey in this.items)
                if (this.items[itemKey].itemObject.typeItem == "Relation" && this.items[itemKey].itemObject.nameInSource == answer.oldNameInSource) {
                    this.items[itemKey].repaint(answer.itemObject);
                    this.updateColumns(this.items[itemKey], columns);
                    this.updateParameters(this.items[itemKey], parameters);
                }

            selectedItem.setSelected();
            selectedItem.openTree();
        }
        if (answer.databases)
            this.updateIcons(answer.databases);
    }

    dictionaryTree.updateIcons = function (databases) {
        for (var i = 0; i < databases.length; i++) {
            var db = databases[i];
            for (var j = 0; j < db.dataSources.length; j++) {
                var ds = db.dataSources[j];
                for (var k = 0; k < ds.relations.length; k++) {
                    var rl = ds.relations[k];
                    for (var itemKey in this.items)
                        if (this.items[itemKey].itemObject.typeItem == "Relation" && this.items[itemKey].itemObject.nameInSource == rl.nameInSource &&
                            this.items[itemKey].itemObject.childDataSource == rl.childDataSource)
                            this.items[itemKey].repaint(rl);
                }
            }
        }
    }

    dictionaryTree.deleteRelation = function () {
        var parent = this.selectedItem.parent;

        var selectedRelationObject = this.selectedItem.itemObject;
        for (var itemKey in this.items)
            if (this.items[itemKey].itemObject.typeItem == "Relation" &&
                this.items[itemKey].itemObject.nameInSource == selectedRelationObject.nameInSource &&
                this.items[itemKey].itemObject.name == selectedRelationObject.name) {
                var parentItem = this.items[itemKey].parent;
                if (parentItem && parentItem.itemObject.relations) {
                    for (var k = 0; k < parentItem.itemObject.relations.length; k++) {
                        if (selectedRelationObject.nameInSource == parentItem.itemObject.relations[k].nameInSource) {
                            parentItem.itemObject.relations.splice(k, 1);
                        }
                    }
                }
                this.items[itemKey].remove();
            }
    }

    /* Columns */
    dictionaryTree.addColumn = function (answer) {
        var columnObject = answer.itemObject
        var isBusinessObject = answer.currentParentType == "BusinessObject";
        var parentName = answer.currentParentName;
        var selectedItem = null;

        if (!isBusinessObject) {
            var parentDataSource = this.getItemByNameAndType(parentName, "DataSource");
            if (!parentDataSource) return;
            if (parentDataSource.buildChildsNotCompleted) parentDataSource.completeBuildTree();
            for (var itemKey in this.items) {
                if ((this.items[itemKey].itemObject.typeItem == "Relation" && this.items[itemKey].itemObject.parentDataSource == parentName) ||
                    (this.items[itemKey].itemObject.typeItem == "DataSource" && this.items[itemKey].itemObject.name == parentName)) {
                    var columnItem = jsObject.DictionaryTreeItemFromObject(columnObject);
                    this.items[itemKey].addChild(columnItem);
                    if (this.items[itemKey].itemObject.columns) this.items[itemKey].itemObject.columns.push(columnObject);
                    if (this.items[itemKey] == parentDataSource) selectedItem = columnItem;
                }
            }
        }
        else {
            var columnItem = jsObject.DictionaryTreeItemFromObject(columnObject);
            var parentBusinessObjectItem = this.getItemByNameAndType(parentName, "BusinessObject");
            if (parentBusinessObjectItem) parentBusinessObjectItem.addChild(columnItem);
            if (parentBusinessObjectItem.itemObject.columns) parentBusinessObjectItem.itemObject.columns.push(columnObject);
            selectedItem = columnItem;
        }

        if (selectedItem) {
            selectedItem.setSelected();
            selectedItem.openTree();
        }
    }

    dictionaryTree.editColumn = function (answer) {
        var editColumnForm = jsObject.options.forms.editColumnForm;
        var columnItem = editColumnForm && editColumnForm.editableDictionaryItem ? editColumnForm.editableDictionaryItem : this.selectedItem;

        if (columnItem) {
            var columnObject = answer.itemObject
            var currentParentType = answer.currentParentType;
            var currentParentName = answer.currentParentName;

            if (currentParentType == "BusinessObject") {
                columnItem.repaint(columnObject);
            }
            else {
                var columnName = columnItem.itemObject.name;
                for (var itemKey in this.items)
                    if (this.isColumnItemOfDataSource(this.items[itemKey], columnName, currentParentName))
                        this.items[itemKey].repaint(columnObject);
            }
        }
    }

    dictionaryTree.deleteColumn = function (answer) {
        var currentParentType = answer.currentParentType;
        var currentParentName = answer.currentParentName;
        var columnObject = this.selectedItem.itemObject;

        var deleteColumnObjectFromParent = function (parentItem) {
            if (parentItem && parentItem.itemObject.columns) {
                for (var k = 0; k < parentItem.itemObject.columns.length; k++) {
                    if (parentItem.itemObject.columns[k].name == columnObject.name) {
                        parentItem.itemObject.columns.splice(k, 1);
                    }
                }
            }
        }

        if (currentParentType == "BusinessObject") {
            this.selectedItem.remove();
            deleteColumnObjectFromParent(this.selectedItem.parent);
        }
        else {
            var columnObject = this.selectedItem.itemObject;
            for (var key in this.items) {
                if (this.isColumnItemOfDataSource(this.items[key], columnObject.name, currentParentName)) {
                    deleteColumnObjectFromParent(this.items[key].parent);
                    this.items[key].remove();
                }
            }
        }
    }

    /* Parameters */
    dictionaryTree.addParameter = function (answer) {
        var parameterObject = answer.itemObject
        var parentName = answer.currentParentName;
        var selectedItem = null;

        var parentDataSource = this.getItemByNameAndType(parentName, "DataSource");
        if (!parentDataSource) return;
        if (parentDataSource.buildChildsNotCompleted) parentDataSource.completeBuildTree();
        if (parentDataSource.itemObject.parameters) parentDataSource.itemObject.parameters.push(parameterObject);

        for (var itemKey in this.items) {
            var parentItem = this.items[itemKey];
            if ((parentItem.itemObject.typeItem == "Relation" && parentItem.itemObject.parentDataSource == parentName) ||
                (parentItem.itemObject.typeItem == "DataSource" && parentItem.itemObject.name == parentName)) {
                var haveParameters = false;
                for (var itemKey2 in parentItem.childs) {
                    if (parentItem.childs[itemKey2].itemObject.typeItem == "Parameters") {
                        parentItem = parentItem.childs[itemKey2];
                        haveParameters = true;
                        break;
                    }
                }
                if (!haveParameters) {
                    var parametersItem = jsObject.DictionaryTreeItem(jsObject.loc.PropertyMain.Parameters, "Parameter.png", { name: "Parameters", typeItem: "Parameters" }, dictionaryTree);
                    parentItem.addChild(parametersItem);
                    parentItem = parametersItem;
                }

                var parameterItem = jsObject.DictionaryTreeItemFromObject(parameterObject);
                parentItem.addChild(parameterItem);
                if (this.items[itemKey] == parentDataSource) selectedItem = parameterItem;
            }
        }

        if (selectedItem) {
            selectedItem.setSelected();
            selectedItem.openTree();
        }
    }

    dictionaryTree.editParameter = function (answer) {
        var parameterObject = answer.itemObject
        var currentParentName = answer.currentParentName;
        var parameterName = this.selectedItem.itemObject.name;
        for (var itemKey in this.items)
            if (this.isParameterItemOfDataSource(this.items[itemKey], parameterName, currentParentName))
                this.items[itemKey].repaint(parameterObject);
    }

    dictionaryTree.deleteParameter = function (answer) {
        var currentParentName = answer.currentParentName;
        var parent = this.selectedItem.parent;
        var mainParent = parent.parent;
        var parameterObject = this.selectedItem.itemObject;

        if (mainParent && mainParent.itemObject.parameters) {
            for (var k = 0; k < mainParent.itemObject.parameters.length; k++) {
                if (mainParent.itemObject.parameters[k].name == parameterObject.name) {
                    mainParent.itemObject.parameters.splice(k, 1);
                }
            }
        }

        for (var itemKey in this.items)
            if (this.isParameterItemOfDataSource(this.items[itemKey], parameterObject.name, currentParentName)) {
                if (this.items[itemKey].parent && jsObject.GetCountObjects(this.items[itemKey].parent.childs) == 1) {
                    parent = mainParent;
                    this.items[itemKey].parent.remove();
                }
                else
                    this.items[itemKey].remove();
            }
    }

    /* Helper Methods */
    dictionaryTree.getCurrentColumnParent = function (columnItem) {
        var currItem = columnItem || this.selectedItem;
        if (!currItem) return;

        var parent = {
            name: "",
            type: ""
        };

        while (currItem.parent != null) {
            if (currItem.itemObject.typeItem == "DataSource" || currItem.itemObject.typeItem == "BusinessObject") {
                parent.type = currItem.itemObject.typeItem;
                parent.typeDataSource = currItem.itemObject.typeDataSource;
                parent.name = currItem.itemObject.name;
                parent.parameterTypes = currItem.itemObject.parameterTypes;

                return parent;
            }
            if (currItem.itemObject.typeItem == "Relation") {
                parent.type = "DataSource";
                parent.name = currItem.itemObject.parentDataSource;

                var dataSource = jsObject.GetDataSourceByNameFromDictionary(parent.name);
                if (dataSource) {
                    parent.typeDataSource = currItem.itemObject.typeDataSource;
                }

                return parent;
            }
            currItem = currItem.parent;
        }
        return parent;
    }

    dictionaryTree.getCurrentVariableParent = function () {
        if (!this.selectedItem) return this.mainItems["Variables"];
        var currItem = this.selectedItem;

        while (currItem.parent != null) {
            if (currItem.itemObject.typeItem == "Category" && currItem.parent == this.mainItems["Variables"])
                return currItem;
            currItem = currItem.parent;
        }

        return this.mainItems["Variables"];
    }

    dictionaryTree.getItemByNameAndType = function (name, type) {
        if (name == null) return null;
        if (typeof (name) == "string") {
            for (var itemKey in this.items) {
                if (this.items[itemKey].itemObject.typeItem == type && this.items[itemKey].itemObject.name == name)
                    return this.items[itemKey];
            }
        }
        else {
            if (name.length == 0) return null;
            for (var itemKey in this.items) {
                if (this.items[itemKey].itemObject.typeItem == type &&
                    this.items[itemKey].itemObject.name == name[0]) {
                    var resultItem = this.items[itemKey];
                    if (name.length == 1 && resultItem.parent != null && resultItem.parent.itemObject.typeItem != "BusinessObject") {
                        return resultItem;
                    }
                    else {
                        var flag = true;
                        var parentItem = resultItem.parent;
                        for (var k = 1; k < name.length; k++) {
                            if (parentItem != null && parentItem.itemObject.typeItem == type && parentItem.itemObject.name == name[k]) {
                                parentItem = parentItem.parent;
                            }
                            else {
                                flag = false;
                            }
                            if (flag) return resultItem;
                        }
                    }
                }
            }
        }

        return null;
    }

    dictionaryTree.cutAliasFromName = function (name) {
        return name.substring(name.indexOf("[") + 1, name.indexOf("]"));
    }

    dictionaryTree.getItemCaption = function (itemObject) {
        var options = jsObject.options;
        var useAliases = options.useAliases;
        var captionText = (!itemObject.alias || itemObject.name == itemObject.alias) ? itemObject.name : itemObject.name + " [" + itemObject.alias + "]";

        if (itemObject.typeItem == "DataBase") {
            if (useAliases && itemObject.isDataStore && itemObject.alias == itemObject.name && itemObject.name.indexOf("[") >= 0 && itemObject.name.indexOf("]") >= 0) {
                return dictionaryTree.cutAliasFromName(itemObject.name);
            }
            else {
                if (useAliases && jsObject.options.showOnlyAliasForDatabase && itemObject.alias)
                    return itemObject.serviceName ? itemObject.alias + " [" + itemObject.serviceName + "]" : itemObject.alias
                else
                    return (itemObject.alias == itemObject.name ? (itemObject.serviceName ? itemObject.name + " [" + itemObject.serviceName + "]" : itemObject.nameInSource) : captionText);
            }
        }
        else if (useAliases && itemObject.alias) {
            if (itemObject.typeItem == "DataSource")
                return options.showOnlyAliasForDataSource ? itemObject.alias : captionText;
            else if (itemObject.typeItem == "BusinessObject")
                return options.showOnlyAliasForBusinessObject ? itemObject.alias : captionText;
            else if (itemObject.typeItem == "Column")
                return options.showOnlyAliasForDataColumn ? itemObject.alias : captionText;
            else if (itemObject.typeItem == "Relation")
                return options.showOnlyAliasForDataRelation ? itemObject.alias : captionText;
            else if (itemObject.typeItem == "Resource")
                return options.showOnlyAliasForResource ? itemObject.alias : captionText;
            else if (itemObject.typeItem == "Variable")
                return options.showOnlyAliasForVariable ? itemObject.alias : captionText;
        }
        else
            return captionText;
    }

    dictionaryTree.getConnectionNameFromNameInSource = function (dataSourceObject) {
        var nameInSourceArray = dataSourceObject.nameInSource.split(".");
        var nameInSourceFirstName = nameInSourceArray[0];
        var connectionName = nameInSourceFirstName == ""
            ? (dataSourceObject["typeDataAdapter"]
                ? jsObject.GetLocalizedAdapterName(dataSourceObject.typeDataAdapter)
                : jsObject.loc.Database.Connection)
            : nameInSourceFirstName

        return connectionName;
    }

    dictionaryTree.isColumnItemOfDataSource = function (item, columnName, dataSourceName) {
        if (item.itemObject.typeItem == "Column" && item.itemObject.name == columnName
            && ((item.parent.itemObject.typeItem == "Relation" && item.parent.itemObject.parentDataSource == dataSourceName) ||
                (item.parent.itemObject.typeItem == "DataSource" && item.parent.itemObject.name == dataSourceName)))
            return true;
        return false;
    }

    dictionaryTree.isParameterItemOfDataSource = function (item, parameterName, dataSourceName) {
        var parameterParent = item.parent ? item.parent.parent : null;
        if (parameterParent && item.itemObject.typeItem == "Parameter" && item.itemObject.name == parameterName
            && ((parameterParent.itemObject.typeItem == "Relation" && parameterParent.itemObject.parentDataSource == dataSourceName) ||
                (parameterParent.itemObject.typeItem == "DataSource" && parameterParent.itemObject.name == dataSourceName)))
            return true;
        return false;
    }

    //Columns
    dictionaryTree.addColumns = function (item, columns) {
        if (!columns) return;

        if (jsObject.options.dictionarySorting == "ascending") {
            columns.sort(jsObject.SortByName);
        }
        else if (jsObject.options.dictionarySorting == "descending") {
            columns.sort(jsObject.SortByNameDescending);
        }

        for (var i = 0; i < columns.length; i++) {
            item.addChild(jsObject.DictionaryTreeItemFromObject(columns[i]));
        }
    }

    dictionaryTree.removeColumns = function (item) {
        for (var itemKey in item.childs)
            if (item.childs[itemKey].itemObject.typeItem == "Column")
                item.childs[itemKey].remove();
    }

    dictionaryTree.updateColumns = function (item, columns) {
        this.removeColumns(item);
        this.addColumns(item, columns);
    }

    //Parameters
    dictionaryTree.addParameters = function (item, parameters) {
        if (!parameters || parameters.length == 0) return;

        var parametersItem = jsObject.DictionaryTreeItem(jsObject.loc.PropertyMain.Parameters, "Parameter.png", { name: "Parameters", typeItem: "Parameters" }, dictionaryTree);
        item.addChild(parametersItem);

        for (var i = 0; i < parameters.length; i++) {
            parametersItem.addChild(jsObject.DictionaryTreeItemFromObject(parameters[i]));
        }
    }

    dictionaryTree.removeParameters = function (item) {
        for (var itemKey in item.childs)
            if (item.childs[itemKey].itemObject.typeItem == "Parameter" || item.childs[itemKey].itemObject.typeItem == "Parameters")
                item.childs[itemKey].remove();
    }

    dictionaryTree.updateParameters = function (item, parameters) {
        this.removeParameters(item);
        this.addParameters(item, parameters);
    }

    //Override
    dictionaryTree.onSelectedItem = function (selectedItem) {
        var dictionaryPanel = jsObject.options.dictionaryPanel;
        dictionaryPanel.toolBar.updateControls();
        dictionaryPanel.descriptionPanel.hide();

        if (selectedItem) {
            for (var contName in jsObject.options.containers) {
                var container = jsObject.options.containers[contName];
                container.getItemButton.setEnabled(container.canInsertItem(selectedItem.itemObject));
            }
            dictionaryPanel.setFocused(true);
            if (selectedItem.itemObject) {
                if (selectedItem.itemObject.typeItem == "SystemVariable") {
                    dictionaryPanel.descriptionPanel.show(jsObject.GetSystemVariableDescription(selectedItem.itemObject.name));
                }
                else if (selectedItem.itemObject.typeItem == "Function") {
                    dictionaryPanel.descriptionPanel.show(jsObject.GetFunctionDescription(selectedItem.itemObject));
                }
            }
        }
    }        

    return dictionaryTree;
}

StiMobileDesigner.prototype.DictionaryTreeItem = function (caption, imageName, itemObject, tree, notMainDictionary) {
    var jsObject = this;
    if (!tree) tree = this.options.dictionaryTree;

    var dictTreeItem = this.TreeItem(caption, imageName, itemObject, tree);

    if (notMainDictionary) dictTreeItem.notMainDictionary = true;

    //Override
    dictTreeItem.repaint = function (itemObject) {
        if (itemObject == null) itemObject = this.itemObject;
        StiMobileDesigner.setImageSource(this.button.image, this.jsObject.options, (itemObject.typeIcon == "ConnectionFail" ? this.jsObject.GetConnectionFailIconName(itemObject) : itemObject.typeIcon) + ".png");
        this.button.captionCell.innerText = tree.getItemCaption(itemObject);
        this.itemObject = itemObject;
    }

    dictTreeItem.setSelected = function () {
        var baseClass = "stiDesignerTreeItemButton stiDesignerTreeItemButton";
        if (tree.selectedItem) {
            tree.selectedItem.button.className = baseClass + "Default";
            tree.selectedItem.isSelected = false;
        }
        this.button.className = baseClass + "Selected";
        tree.selectedItem = this;
        this.isSelected = true;
        tree.onSelectedItem(this);

        if (!this.notMainDictionary && this.jsObject.options.dictionaryPanel && !this.jsObject.options.dictionaryPanel.isFocused) {
            if (this.button.className.indexOf("stiDesignerTreeItemButtonSelectedNotActive") < 0) this.button.className += " stiDesignerTreeItemButtonSelectedNotActive";
        }
    }

    dictTreeItem.button.ondblclick = function () {
        var propertiesPanel = jsObject.options.propertiesPanel;
        if (propertiesPanel.dictionaryMode && propertiesPanel.editFormControl) {
            if (propertiesPanel.editFormControl["addItem"] != null) {
                propertiesPanel.editFormControl.addItem(dictTreeItem.itemObject);
            }
            else {
                var text = dictTreeItem.getResultForEditForm();
                if (propertiesPanel.editFormControl.cutBrackets && text.indexOf("{") == 0 && jsObject.EndsWith(text, "}")) {
                    text = text.substr(1, text.length - 2);
                }
                if ("insertText" in propertiesPanel.editFormControl) {
                    propertiesPanel.editFormControl.insertText(text);
                }
                else {
                    propertiesPanel.editFormControl.value += text;
                }
            }
        }
        else {
            if (jsObject.options.menus.dictionarySettingsMenu.controls.createFieldOnDoubleClick.isChecked &&
                (dictTreeItem.itemObject.typeItem == "Parameter" || dictTreeItem.itemObject.typeItem == "Column" ||
                    (dictTreeItem.itemObject.typeItem == "Resource" && (dictTreeItem.itemObject.type == "Image" || dictTreeItem.itemObject.type == "Rtf")))) {
                jsObject.SendCommandCreateFieldOnDblClick(dictTreeItem);
            }
            else if (jsObject.options.dictionaryPanel.toolBar.controls["EditItem"].isEnabled) {
                jsObject.EditItemDictionaryTree();
            }
        }
    }

    dictTreeItem.getResultForEditForm = function (withOutBrackets, useOriginalName) {
        var currItem = this;
        var fullName = "";
        if (currItem.itemObject.typeItem == "Variable" || currItem.itemObject.typeItem == "SystemVariable") {
            fullName = useOriginalName ? currItem.itemObject.name : (currItem.itemObject.correctName || currItem.itemObject.name);
        }
        else if (currItem.itemObject.typeItem == "Function") {
            fullName = currItem.itemObject.text;
        }
        else if (currItem.itemObject.typeItem == "Format" || currItem.itemObject.typeItem == "HtmlTag") {
            withOutBrackets = true;
            fullName = currItem.itemObject.key;
        }
        else if (currItem.itemObject.typeItem == "Column") {
            while (currItem.parent != null) {
                if (fullName != "") fullName = "." + fullName;
                fullName = (useOriginalName ? currItem.itemObject.name : (currItem.itemObject.correctName || currItem.itemObject.name)) + fullName;
                if (currItem.itemObject.typeItem == "BusinessObject" && currItem.parent != null && currItem.parent.itemObject.typeItem != "BusinessObject"
                    || currItem.itemObject.typeItem == "DataSource") break;
                currItem = currItem.parent;
            }
        }
        else if (currItem.itemObject.typeItem == "Parameter") {
            fullName = ".Parameters[\"" + (useOriginalName ? currItem.itemObject.name : (currItem.itemObject.correctName || currItem.itemObject.name)) + "\"].ParameterValue";
            while (currItem.parent != null) {
                if (currItem.itemObject.typeItem == "DataSource") {
                    fullName = (useOriginalName ? currItem.itemObject.name : (currItem.itemObject.correctName || currItem.itemObject.name)) + fullName;
                    break;
                }
                currItem = currItem.parent;
            }
        }
        else if (currItem.itemObject.typeItem == "Resource") {
            return jsObject.options.resourceIdent + currItem.itemObject.name;
        }

        fullName = fullName ? (withOutBrackets ? fullName : "{" + fullName + "}") : "";

        return fullName;
    }

    dictTreeItem.getChildNames = function () {
        if (this.buildChildsNotCompleted) this.completeBuildTree(true);

        var childNames = {};
        for (var itemKey in this.childs)
            childNames[itemKey] = this.childs[itemKey].itemObject.name;

        return childNames;
    }

    dictTreeItem.getBusinessObjectFullName = function () {
        var currItem = this;
        var fullName = [];
        while (currItem.parent != null) {
            if (currItem.itemObject.typeItem == "BusinessObject") {
                fullName.push(currItem.itemObject.name);
            }
            currItem = currItem.parent;
        }
        return fullName;
    }

    dictTreeItem.getBusinessObjectStringFullName = function () {
        var currItem = this;
        var fullName = "";
        while (currItem.parent != null) {
            if (currItem.itemObject.typeItem == "BusinessObject") {
                if (fullName != "") fullName = "." + fullName;
                fullName = (currItem.itemObject.correctName || currItem.itemObject.name) + fullName;
            }
            currItem = currItem.parent;
        }
        return fullName;
    }

    dictTreeItem.canShow = function () {
        var permissionDataSources = jsObject.options.permissionDataSources;
        var permissionDataConnections = jsObject.options.permissionDataConnections;
        var permissionBusinessObjects = jsObject.options.permissionBusinessObjects;
        var permissionVariables = jsObject.options.permissionVariables;
        var permissionDataRelations = jsObject.options.permissionDataRelations;
        var permissionDataColumns = jsObject.options.permissionDataColumns;
        var permissionResources = jsObject.options.permissionResources;
        var permissionSqlParameters = jsObject.options.permissionSqlParameters;

        var getPermissionResult = function (itemObject, permissionType) {
            return (itemObject.typeItem == "DataBase" && !(!permissionDataConnections || permissionDataConnections.indexOf("All") >= 0 || permissionDataConnections.indexOf(permissionType) >= 0)) ||
                ((itemObject.typeItem == "DataSource" || itemObject.typeItem == "DataSourceMainItem")
                    && !(!permissionDataSources || permissionDataSources.indexOf("All") >= 0 || permissionDataSources.indexOf(permissionType) >= 0)) ||
                ((itemObject.typeItem == "BusinessObject" || itemObject.typeItem == "BusinessObjectMainItem")
                    && !(!permissionBusinessObjects || permissionBusinessObjects.indexOf("All") >= 0 || permissionBusinessObjects.indexOf(permissionType) >= 0)) ||
                ((itemObject.typeItem == "Variable" || itemObject.typeItem == "VariablesMainItem")
                    && !(!permissionVariables || permissionVariables.indexOf("All") >= 0 || permissionVariables.indexOf(permissionType) >= 0)) ||
                ((itemObject.typeItem == "Resource" || itemObject.typeItem == "ResourcesMainItem")
                    && !(!permissionResources || permissionResources.indexOf("All") >= 0 || permissionResources.indexOf(permissionType) >= 0)) ||
                ((itemObject.typeItem == "Parameter" || itemObject.typeItem == "Parameters")
                    && !(!permissionSqlParameters || permissionSqlParameters.indexOf("All") >= 0 || permissionSqlParameters.indexOf(permissionType) >= 0)) ||
                (itemObject.typeItem == "Relation" && !(!permissionDataRelations || permissionDataRelations.indexOf("All") >= 0 || permissionDataRelations.indexOf(permissionType) >= 0)) ||
                (itemObject.typeItem == "Column" && !(!permissionDataColumns || permissionDataColumns.indexOf("All") >= 0 || permissionDataColumns.indexOf(permissionType) >= 0))
        }

        if (dictTreeItem.itemObject.restrictions && dictTreeItem.itemObject.restrictions.isAllowShow === false) {
            return false;
        }

        return !(getPermissionResult(dictTreeItem.itemObject, "View"));
    }

    dictTreeItem.canDragged = function () {
        return (
            this.itemObject.typeItem == "DataSource" ||
            this.itemObject.typeItem == "Column" ||
            this.itemObject.typeItem == "SystemVariable" ||
            this.itemObject.typeItem == "Variable" ||
            this.itemObject.typeItem == "BusinessObject" ||
            this.itemObject.typeItem == "Function" ||
            this.itemObject.typeItem == "Category" ||
            this.itemObject.typeItem == "Parameter" ||
            this.itemObject.typeItem == "Format" ||
            this.itemObject.typeItem == "HtmlTag" ||
            (this.itemObject.typeItem == "Resource" &&
                (this.itemObject.type == "Image" ||
                    this.itemObject.type == "Map" ||
                    this.itemObject.type == "Rtf" ||
                    this.itemObject.type == "Report" ||
                    this.itemObject.type == "ReportSnapshot")) ||
            this.itemObject.isCloudAttachedItem
        )
    }

    dictTreeItem.canMove = function (direction) {
        if (!this.itemObject) return false;

        if (this.itemObject.restrictions && this.itemObject.restrictions.isAllowMove === false) {
            return false;
        }

        if (this.itemObject.typeItem == "Column") {
            return ((direction == "Up" && this.previousSibling && this.previousSibling.itemObject.typeItem == "Column") ||
                (direction == "Down" && this.nextSibling && this.nextSibling.itemObject.typeItem == "Column"))
        }
        else if (this.itemObject.typeItem == "Variable") {
            var item = null;

            if (direction == "Up") {
                item = this.previousSibling || this.parent.previousSibling || this.parent.parent;
            }
            else if (direction == "Down") {
                item = this.nextSibling || this.parent.nextSibling || this.parent.parent;
            }
            if (item && item.itemObject && (item.itemObject.typeItem == "Variable" || item.itemObject.typeItem == "Category" || item.itemObject.typeItem == "VariablesMainItem")) {
                return item.itemObject;
            }
        }
        else if (this.itemObject.typeItem == "DataSource" ||
            (this.itemObject.typeItem == "Category" && this.parent && this.parent.itemObject.typeItem == "VariablesMainItem")) {
            return ((direction == "Up" && this.previousSibling && this.previousSibling.itemObject) ||
                (direction == "Down" && this.nextSibling && this.nextSibling.itemObject))
        }

        return false;
    }

    //Touch Start
    dictTreeItem.button.ontouchstart = function (event, mouseProcess) {
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);

        if (event) event.preventDefault();
        this.action();

        if (dictTreeItem.canDragged() && (!event || event.button != 2)) {
            var itemInDragObject;
            var itemObject = dictTreeItem.itemObject;
            var propertiesPanel = jsObject.options.propertiesPanel;

            if (itemObject.typeItem == "Resource" && !(propertiesPanel.dictionaryMode && propertiesPanel.editFormControl)) {
                var compType;

                switch (itemObject.type) {
                    case "Image":
                        compType = "StiImage";
                        break;
                    case "Rtf":
                        compType = "StiRichText";
                        break;
                    case "Report":
                    case "ReportSnapshot":
                        compType = "StiSubReport";
                        break;
                    case "Map":
                        compType = "StiMap";
                        break;
                }

                if (!compType) return;
                itemInDragObject = jsObject.ComponentForDragDrop(dictTreeItem.itemObject, compType);
            }
            else {
                itemInDragObject = jsObject.TreeItemForDragDrop(dictTreeItem.itemObject, tree);
            }

            itemInDragObject.originalItem = dictTreeItem;

            if (itemInDragObject.itemObject.typeItem != "BusinessObject") {
                var fullName = dictTreeItem.getResultForEditForm();
                itemInDragObject.itemObject.fullName = StiBase64.encode(fullName);
            }
            itemInDragObject.beginingOffset = 0;
            jsObject.options.itemInDrag = itemInDragObject;
        }

        this.isTouchStartTimer = setTimeout(function () {
            dictTreeItem.button.isTouchStartFlag = false;
        }, 1000);
    }

    //Mouse Down
    dictTreeItem.button.onmousedown = function (event) {
        if (this.isTouchStartFlag) return;
        this.ontouchstart(event, true);
    }

    //Mouse Up
    dictTreeItem.button.onmouseup = function (event) {
        if (jsObject.options.itemInDrag && !dictTreeItem.notMainDictionary) {
            var selectedItem = jsObject.options.dictionaryTree.selectedItem;
            if (selectedItem) {
                var fromObject = selectedItem.itemObject;
                var toItem = dictTreeItem;
                var toObject = toItem ? toItem.itemObject : null;
                if (fromObject && (fromObject.typeItem == "Variable" || fromObject.typeItem == "Category") && fromObject != toObject && (!fromObject.restrictions || fromObject.restrictions.isAllowMove !== false)) {
                    jsObject.SendCommandMoveDictionaryItem(fromObject, toObject);
                }
            }
        }
    }

    jsObject.addEvent(dictTreeItem.button, "touchend", function (event) {
        if (jsObject.options.itemInDrag && jsObject.options.itemInDrag.beginingOffset && jsObject.options.itemInDrag.beginingOffset >= 10) {
            jsObject.DropDragableItemToActiveContainer(jsObject.options.itemInDrag);
        }
    });

    //Override opening method
    dictTreeItem.iconOpening.action = function () {
        if (tree.isDisable) return;
        dictTreeItem.isOpening = !dictTreeItem.isOpening;
        dictTreeItem.childsRow.style.display = dictTreeItem.isOpening ? "" : "none";
        var imgName = dictTreeItem.isOpening ? "IconCloseItem.png" : "IconOpenItem.png";
        if (jsObject.options.isTouchDevice) imgName = imgName.replace(".png", "Big.png");
        StiMobileDesigner.setImageSource(dictTreeItem.iconOpening, jsObject.options, imgName);
        dictTreeItem.setSelected();

        if (dictTreeItem.isOpening && dictTreeItem.buildChildsNotCompleted && dictTreeItem.childCollection) {
            if (dictTreeItem.itemObject.typeItem == "DataBase") {
                tree.addTreeItems(dictTreeItem.childCollection, dictTreeItem, true);
            }
            else {
                var dataSource = jsObject.GetDataSourceByNameAndNameInSourceFromDictionary(dictTreeItem.itemObject.name, dictTreeItem.itemObject.nameInSource);
                if (dataSource) {
                    dictTreeItem.childCollection = jsObject.CopyObject(dataSource.relations);
                    dictTreeItem.itemObject.columns = jsObject.CopyObject(dataSource.columns);
                    dictTreeItem.itemObject.parameters = jsObject.CopyObject(dataSource.parameters);
                }
                tree.addTreeItems(dictTreeItem.childCollection, dictTreeItem);
            }
            delete dictTreeItem.buildChildsNotCompleted;
            delete dictTreeItem.childCollection;
        }
    }

    dictTreeItem.completeBuildTree = function (buildOnlyOneLevel) {
        if (this.buildChildsNotCompleted && this.childCollection) {
            tree.addTreeItems(this.childCollection, this, buildOnlyOneLevel);
            delete this.buildChildsNotCompleted;
            delete this.childCollection;
        }
    }

    dictTreeItem.style.display = dictTreeItem.canShow() ? "" : "none";

    return dictTreeItem;

}

StiMobileDesigner.prototype.GetConnectionFailIconName = function (itemObject) {
    var iconName = (this.options.cloudMode || this.options.serverMode)
        ? (itemObject.isCloud ? "CloudDataBase" : "DataStore")
        : "ConnectionFail";

    return iconName;
}

StiMobileDesigner.prototype.DictionaryTreeItemFromObject = function (itemObject, tree, notMainDictionary) {
    var dictionaryTreeItem = this.DictionaryTreeItem(
        this.options.dictionaryTree.getItemCaption(itemObject),
        (itemObject.typeIcon == "ConnectionFail" ? this.GetConnectionFailIconName(itemObject) : itemObject.typeIcon) + ".png",
        itemObject,
        tree || this.options.dictionaryTree);

    if (notMainDictionary) dictionaryTreeItem.notMainDictionary = true;

    return dictionaryTreeItem;
}

StiMobileDesigner.prototype.EditItemDictionaryTree = function () {
    var jsObject = this;
    var selectedItem = this.options.dictionaryTree.selectedItem;
    var typeItem = selectedItem.itemObject.typeItem.replace("DataBase", "Connection").replace("BusinessObject", "DataSource");
    var form = selectedItem.itemObject.typeDataSource == "StiVirtualSource"
        ? this.options.forms.editDataSourceFromOtherDatasourcesForm
        : selectedItem.itemObject.typeDataSource == "StiDataTransformation"
            ? this.options.forms.editDataTransformationForm
            : this.options.forms["edit" + typeItem + "Form"];

    if (!form) {
        switch (typeItem) {
            case "DataSource":
                {
                    if (selectedItem.itemObject.typeDataSource == "StiVirtualSource") {
                        this.InitializeEditDataSourceFromOtherDatasourcesForm(function (form) {
                            form.datasource = jsObject.CopyObject(selectedItem.itemObject);
                            form.changeVisibleState(true);
                        });
                    }
                    else if (selectedItem.itemObject.typeDataSource == "StiCrossTabDataSource") {
                        this.InitializeEditDataSourceFromCrossTabForm(function (form) {
                            form.datasource = jsObject.CopyObject(selectedItem.itemObject);
                            form.changeVisibleState(true);
                        });
                    }
                    else if (selectedItem.itemObject.typeDataSource == "StiDataTransformation") {
                        this.InitializeEditDataTransformationForm(function (form) {
                            form.datasource = jsObject.CopyObject(selectedItem.itemObject);
                            form.changeVisibleState(true);
                        });
                    }
                    else {
                        this.InitializeEditDataSourceForm(function (form) {
                            form.datasource = jsObject.CopyObject(selectedItem.itemObject);
                            form.changeVisibleState(true);
                        });
                    }
                    break;
                }
            case "Category":
                this.InitializeEditCategoryForm(function (editCategoryForm) {
                    editCategoryForm.category = jsObject.CopyObject(selectedItem.itemObject);
                    editCategoryForm.changeVisibleState(true);
                });
                break;
            case "Column":
                this.InitializeEditColumnForm(function (editColumnForm) {
                    editColumnForm.column = jsObject.CopyObject(selectedItem.itemObject);
                    editColumnForm.changeVisibleState(true);
                });
                break;
            case "Connection":
                this.InitializeEditConnectionForm(function (editConnectionForm) {
                    editConnectionForm.connection = jsObject.CopyObject(selectedItem.itemObject);
                    editConnectionForm.changeVisibleState(true);
                });
                break;
            case "Parameter":
                this.InitializeEditParameterForm(function (editParameterForm) {
                    editParameterForm.parameter = jsObject.CopyObject(selectedItem.itemObject);
                    editParameterForm.changeVisibleState(true);
                });
                break;
            case "Relation":
                this.InitializeEditRelationForm(function (editRelationForm) {
                    editRelationForm.relation = jsObject.CopyObject(selectedItem.itemObject);
                    editRelationForm.changeVisibleState(true);
                });
                break;
            case "Variable":
                this.InitializeEditVariableForm(function (editVariableForm) {
                    editVariableForm.variable = jsObject.CopyObject(selectedItem.itemObject);
                    editVariableForm.changeVisibleState(true);
                });
                break;
            case "Resource":
                this.InitializeEditResourceForm(function (editResourceForm) {
                    editResourceForm.resource = jsObject.CopyObject(selectedItem.itemObject);
                    editResourceForm.changeVisibleState(true);
                });
                break;
        }
    }
    if (!form) return;
    form[typeItem.toLowerCase()] = this.CopyObject(selectedItem.itemObject);
    form.changeVisibleState(true);
}

StiMobileDesigner.prototype.DeleteItemDictionaryTree = function () {
    var jsObject = this;
    var messageForm = this.MessageFormForDelete();
    messageForm.changeVisibleState(true);
    messageForm.action = function (state) {
        if (state) {
            var selectedItem = jsObject.options.dictionaryTree.selectedItem;
            switch (selectedItem.itemObject.typeItem) {
                case "DataBase": {
                    if (!selectedItem.itemObject.typeConnection) {
                        if (selectedItem.buildChildsNotCompleted) selectedItem.completeBuildTree(true);
                        var dataSources = [];
                        for (var key in selectedItem.childs) {
                            dataSources.push(selectedItem.childs[key].itemObject);
                        }
                        jsObject.SendCommandToDesignerServer("DeleteAllDataSources", { dataSources: dataSources, connectionName: selectedItem.itemObject.name }, function (answer) {
                            if (answer.databases) {
                                jsObject.options.dictionaryTree.selectedItem.remove();
                                jsObject.options.report.dictionary.databases = answer.databases;
                                jsObject.ClearAllGalleries();
                                jsObject.UpdateStateUndoRedoButtons();
                            }
                        }); break;
                    }
                    else {
                        this.jsObject.SendCommandDeleteConnection(selectedItem);
                    }
                    break;
                }
                case "Relation": { jsObject.SendCommandDeleteRelation(selectedItem); break; }
                case "Column": { jsObject.SendCommandDeleteColumn(selectedItem); break; }
                case "Parameter": { jsObject.SendCommandDeleteParameter(selectedItem); break; }
                case "DataSource": { jsObject.SendCommandDeleteDataSource(selectedItem); break; }
                case "BusinessObject": { jsObject.SendCommandDeleteBusinessObject(selectedItem); break; }
                case "Category": { jsObject.SendCommandDeleteCategory(selectedItem); break; }
                case "Variable": { jsObject.SendCommandDeleteVariable(selectedItem); break; }
                case "Resource": { jsObject.SendCommandDeleteResource(selectedItem); break; }
            }
        }
    }
}

StiMobileDesigner.prototype.DuplicateItemDictionaryTree = function () {
    var selectedItem = this.options.dictionaryTree.selectedItem;
    if (selectedItem) this.SendCommandDuplicateDictionaryElement(selectedItem);
}

StiMobileDesigner.prototype.SetOpeningAllChildDictionaryTree = function (state) {
    var selectedItem = this.options.dictionaryTree.selectedItem;
    if (selectedItem) {
        selectedItem.setOpening(state);
        var childs = selectedItem.getAllChilds();
        for (var i = 0; i < childs.length; i++) {
            childs[i].setOpening(state);
        }
    }
}

StiMobileDesigner.prototype.HiddenMainTreeItem = function (tree, haveItemNone) {
    var mainItem = this.TreeItem("Main", null, { "typeItem": "MainItem" }, tree);
    mainItem.style.margin = "12px";
    mainItem.childsRow.style.display = "";
    mainItem.button.style.display = "none";
    mainItem.iconOpening.style.display = "none";
    tree.mainItem = mainItem;

    if (haveItemNone) {
        var itemNone = this.TreeItem(this.loc.Report.NotAssigned, "ItemNo.png", { "typeItem": "NoItem" }, tree);
        mainItem.addChild(itemNone);
        itemNone.setSelected();
        tree.itemNone = itemNone;
        itemNone.button.ondblclick = function () { tree.action(); }
    }

    tree.getItem = function (keyValue, keyName, typeItem) {
        for (var itemKey in tree.items) {
            if (tree.items[itemKey].itemObject[keyName || "name"] == keyValue && (!typeItem || (typeItem && tree.items[itemKey].itemObject.typeItem == typeItem))) {
                return tree.items[itemKey];
            }
        }
        return null;
    }

    return mainItem;
}

StiMobileDesigner.prototype.SystemVariablesTree = function () {
    var systemVarsTree = this.Tree();

    systemVarsTree.build = function () {
        systemVarsTree.clear();

        var mainItemSysVars = this.jsObject.HiddenMainTreeItem(systemVarsTree);
        systemVarsTree.appendChild(mainItemSysVars);

        if (this.jsObject.options.report) {
            var systemVariables = this.jsObject.options.report.dictionary.systemVariables;
            for (var i = 0; i < systemVariables.length; i++) {
                var item = this.jsObject.TreeItem(systemVariables[i], "SystemVariable" + systemVariables[i] + ".png", { typeItem: "SystemVariable", name: systemVariables[i] }, systemVarsTree);
                mainItemSysVars.addChild(item);

                item.button.ondblclick = function () { systemVarsTree.action(); }
            }
        }
    }

    return systemVarsTree;
}

StiMobileDesigner.prototype.VariablesTree = function () {
    var varsTree = this.Tree();

    varsTree.build = function () {
        varsTree.clear();

        var mainItemVars = this.jsObject.HiddenMainTreeItem(varsTree, true);
        varsTree.appendChild(mainItemVars);

        if (this.jsObject.options.report) {
            var variables = this.jsObject.GetVariablesFromDictionary(this.jsObject.options.report.dictionary);
            for (var i = 0; i < variables.length; i++) {
                var item = this.jsObject.DictionaryTreeItemFromObject(variables[i], varsTree, true);
                mainItemVars.addChild(item);

                item.button.ondblclick = function () { varsTree.action(); }
            }
        }
    }

    return varsTree;
}

StiMobileDesigner.prototype.DataSourcesTree = function (width, height) {
    var jsObject = this;
    var dataTree = this.Tree();
    dataTree.isDisable = false;

    if (width) dataTree.style.width = width + "px";
    if (height) dataTree.style.height = height + "px";

    dataTree.setEnabled = function (state) {
        dataTree.style.opacity = state ? "1" : "0.2";
        dataTree.isDisable = !state;
        if (!state && dataTree.itemNone) dataTree.itemNone.setSelected();
    }

    dataTree.addTreeItems = function (collection, parentItem, openAllItems) {
        if (collection) {
            collection.sort(jsObject.SortByName);

            for (var i = 0; i < collection.length; i++) {
                if (collection[i].typeItem == "Category") {
                    var categoryItem = parentItem.addChild(jsObject.DictionaryTreeItemFromObject(jsObject.CategoryObject(collection[i].name), dataTree, true));
                    categoryItem.button.ondblclick = null;
                    this.addTreeItems(collection[i].categoryItems, categoryItem);
                    if (openAllItems) categoryItem.setOpening(true);
                    continue;
                }

                var childItem = parentItem.addChild(jsObject.DictionaryTreeItemFromObject(collection[i], dataTree, true));
                if (openAllItems) childItem.setOpening(true);
                childItem.button.ondblclick = (childItem.itemObject.typeItem == "DataSource" || childItem.itemObject.typeItem == "BusinessObject") ? function () { if (!dataTree.isDisable) dataTree.action(); } : null;

                var childCollection;
                switch (collection[i].typeItem) {
                    case "DataBase": { childCollection = collection[i].dataSources; break; }
                    case "BusinessObject": { if (!dataTree.oneLevelBusinessObjects) childCollection = collection[i].businessObjects; break; }
                }

                this.addTreeItems(childCollection, childItem);
            }
        }
    }

    dataTree.build = function (openAllItems, hideBusinessObject) {
        dataTree.clear();

        var mainItem = jsObject.HiddenMainTreeItem(dataTree, true);
        dataTree.appendChild(mainItem);

        var dataSourcesItem = jsObject.DictionaryTreeItem(jsObject.loc.PropertyMain.DataSources, "DataSource.png", { name: "DataSourceMainItem", typeItem: "DataSourceMainItem" }, dataTree, true);
        mainItem.addChild(dataSourcesItem);

        var businessObjectsItem = (!hideBusinessObject && jsObject.options.designerSpecification == "Developer" && !jsObject.options.jsMode)
            ? jsObject.DictionaryTreeItem(jsObject.loc.Report.BusinessObjects, "BusinessObject.png", { name: "BusinessObjectMainItem", typeItem: "BusinessObjectMainItem" }, dataTree, true)
            : null;

        if (businessObjectsItem) mainItem.addChild(businessObjectsItem);

        if (openAllItems) {
            dataSourcesItem.setOpening(true);
            if (businessObjectsItem) businessObjectsItem.setOpening(true);
        }

        if (jsObject.options.report) {
            var dictionary = jsObject.options.report.dictionary;
            this.addTreeItems(dictionary.databases, dataSourcesItem, openAllItems);
            if (businessObjectsItem) this.addTreeItems(dictionary.businessObjects, businessObjectsItem, openAllItems);
        }
    }

    dataTree.getBusinessObjectItemByFullName = function (fullName) {
        var nameArray = fullName.split(".");
        var mainItem = this.mainItem;
        if (!mainItem) return null;
        var parentItem = null;
        for (var key in mainItem.childs) {
            if (mainItem.childs[key].itemObject.typeItem == "BusinessObjectMainItem") {
                parentItem = mainItem.childs[key];
                break;
            }
        }
        if (parentItem) {
            for (var i = 0; i < nameArray.length; i++) {
                parentItem = parentItem.getChildByName(nameArray[i], true);
                if (!parentItem) return null;
            }
            return parentItem;
        }
        return null;
    };


    return dataTree;
}

StiMobileDesigner.prototype.RelationsTree = function (width, height) {
    var relTree = this.Tree();
    if (width) relTree.style.width = width + "px";
    if (height) relTree.style.height = height + "px";

    relTree.build = function (dataSource) {
        relTree.clear();

        var mainItem = this.jsObject.HiddenMainTreeItem(relTree, true);
        relTree.appendChild(mainItem);

        if (dataSource && dataSource.relations) {
            for (var i = 0; i < dataSource.relations.length; i++) {
                var relationItem = mainItem.addChild(this.jsObject.DictionaryTreeItemFromObject(dataSource.relations[i], relTree, true));
                relationItem.button.ondblclick = function () { relTree.action(); }
            }
        }
    }

    return relTree;
}

StiMobileDesigner.prototype.MasterComponentsTree = function () {
    var masterTree = this.Tree();

    masterTree.build = function (dataSource) {
        masterTree.clear();

        var mainItem = this.jsObject.HiddenMainTreeItem(masterTree, true);
        masterTree.appendChild(mainItem);

        if (this.jsObject.options.report) {
            var masterComponents = this.jsObject.GetMasterComponentItems(true);
            if (!masterComponents) return;
            for (var i = 0; i < masterComponents.length; i++) {
                var item = this.jsObject.TreeItem(masterComponents[i].name, "Small" + masterComponents[i].type + ".png", masterComponents[i], masterTree);
                mainItem.addChild(item);
                item.button.ondblclick = function () { masterTree.action(); }
            }
        }
    }

    return masterTree;
}

StiMobileDesigner.prototype.TreeItemForDragDrop = function (itemObject, tree, noImage) {
    var jsObject = this;
    var treeItem = this.DictionaryTreeItemFromObject(itemObject, tree);
    this.options.mainPanel.appendChild(treeItem);
    if (tree && tree.items[treeItem.id]) delete tree.items[treeItem.id];

    treeItem.style.position = "absolute";
    treeItem.style.display = "none";
    treeItem.style.width = "";
    treeItem.iconOpening.style.visibility = "hidden";
    treeItem.style.zIndex = "300";

    var button = treeItem.button;
    button.className = "stiDesignerTreeItemForDragDrop stiDesignerTreeItemForDragDrop" + (this.options.isTouchDevice ? "_Touch" : "_Mouse");
    button.ondblclick = null;
    button.ontouchstart = null;
    button.onmousedown = null;
    button.ontouchend = null;
    button.onmouseup = null;
    button.onmousemove = null;
    button.ontouchmove = null;

    if (noImage && treeItem.button.imageCell) {
        button.imageCell.style.display = "none";
        if (button.captionCell) button.captionCell.style.padding = "3px 5px 3px 5px";
    }

    treeItem.move = function (event, offsetX, offsetY) {
        treeItem.style.display = "";
        var clientX = event.touches ? event.touches[0].pageX : event.clientX;
        var clientY = event.touches ? event.touches[0].pageY : event.clientY;

        var designerOffsetX = jsObject.FindPosX(jsObject.options.mainPanel);
        var designerOffsetY = jsObject.FindPosY(jsObject.options.mainPanel);
        clientX -= designerOffsetX;
        clientY -= designerOffsetY;

        if (offsetX) clientX += offsetX;
        if (offsetY) clientY += offsetY;

        this.style.left = clientX + "px";
        this.style.top = (clientY + 20) + "px";
    }

    return treeItem;
}