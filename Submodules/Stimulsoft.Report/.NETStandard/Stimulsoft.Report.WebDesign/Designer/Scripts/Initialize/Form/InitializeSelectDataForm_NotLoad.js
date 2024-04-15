
StiMobileDesigner.prototype.InitializeSelectDataForm_ = function () {
    //Select Data Form
    var selectDataForm = this.BaseForm("selectDataForm", this.loc.FormDatabaseEdit.SelectData, 3, this.HelpLinks["data"]);
    selectDataForm.rootItems = ["Tables", "Views", "Queries", "StoredProcedures"];

    var jsObject = this;

    var newQuery = this.StandartSmallButton(null, null, this.loc.FormDictionaryDesigner.QueryNew, "DataSourceNew.png");
    newQuery.style.display = "inline-block";
    newQuery.style.margin = "12px 12px 8px 12px";
    selectDataForm.container.appendChild(newQuery);
    var newQuerySep = this.FormSeparator();
    selectDataForm.container.appendChild(newQuerySep);

    newQuery.action = function () {
        var this_ = this;
        selectDataForm.changeVisibleState(false);
        jsObject.InitializeEditDataSourceForm(function (editDataSourceForm) {
            editDataSourceForm.datasource = jsObject.GetDataAdapterTypeFromDatabaseType(selectDataForm.connectionObject.typeConnection) || selectDataForm.connectionObject.dataAdapterType;
            editDataSourceForm.nameInSource = selectDataForm.connectionObject.name;
            editDataSourceForm.changeVisibleState(true);
        });
    }

    var findTextbox = this.TextBox(null, 438);
    findTextbox.setAttribute("placeholder", this.loc.FormViewer.Find);
    findTextbox.style.margin = "12px";
    findTextbox.style.display = "none";
    selectDataForm.container.appendChild(findTextbox);

    //Tree
    var tree = this.Tree();
    tree.style.overflow = "auto";
    tree.style.width = "450px";
    tree.style.height = "400px";
    tree.style.padding = "12px";
    selectDataForm.container.appendChild(tree);

    findTextbox.onchange = function () {
        for (var key in tree.items) {
            var itemObject = tree.items[key].itemObject;
            if (itemObject && itemObject.typeItem == "Table") {
                var name = itemObject.name.toLowerCase();
                var correctName = itemObject.correctName.toLowerCase();
                var findValue = findTextbox.value.toLowerCase();
                tree.items[key].style.display = (findValue == "" || name.indexOf(findValue) >= 0 || correctName.indexOf(findValue) >= 0) ? "" : "none";
            }
        }
    }

    //Progress
    var progress = this.Progress();
    progress.className = "stiDesignerSelectDataProgress";
    progress.style.display = "none";
    selectDataForm.container.appendChild(progress);

    selectDataForm.onhide = function () {
        findTextbox.onblur();
    }

    selectDataForm.onshow = function () {
        tree.clear();
        progress.style.display = "";
        findTextbox.style.display = "none";
        tree.style.height = "400px";

        for (var i = 0; i < selectDataForm.rootItems.length; i++) {
            tree["rootItem" + selectDataForm.rootItems[i]] = null;
        }

        var isFileDataConnection = this.typeConnection && (
            jsObject.EndsWith(this.typeConnection, "StiXmlDatabase") ||
            jsObject.EndsWith(this.typeConnection, "StiJsonDatabase") ||
            jsObject.EndsWith(this.typeConnection, "StiDBaseDatabase") ||
            jsObject.EndsWith(this.typeConnection, "StiCsvDatabase") ||
            jsObject.EndsWith(this.typeConnection, "StiExcelDatabase"));

        var hideNewQueryButton = isFileDataConnection || this.typeConnection == "StiODataDatabase";
        newQuery.style.display = hideNewQueryButton ? "none" : "inline-block";
        newQuerySep.style.display = hideNewQueryButton ? "none" : "";
        selectDataForm.buttonOk.setEnabled(false);

        setTimeout(function () {
            jsObject.SendCommandToDesignerServer("GetDatabaseData", {
                databaseName: selectDataForm.databaseName,
                isAsyncCommand: true
            }, function (answer) {
                selectDataForm.buildTree(answer.data);
            });
        }, jsObject.options.formAnimDuration);
    }

    selectDataForm.buildTree = function (data) {
        var addRelations = function (parentItem, collection) {
            if (collection) {
                collection.sort(selectDataForm.jsObject.SortByName);
                for (var m = 0; m < collection.length; m++) {
                    var relationObject = collection[m];
                    var relationItem = selectDataForm.jsObject.TreeItemWithCheckBox(relationObject.name, relationObject.typeIcon + ".png", relationObject, tree);
                    parentItem.addChild(relationItem);
                }
            }
        }

        var addColumns = function (parentItem, collection) {
            if (collection) {
                collection.sort(selectDataForm.jsObject.SortByName);
                for (var m = 0; m < collection.length; m++) {
                    var columnObject = collection[m];
                    var columnItem = selectDataForm.jsObject.TreeItemWithCheckBox(columnObject.name, columnObject.typeIcon + ".png", columnObject, tree);
                    parentItem.addChild(columnItem);
                }
            }
        }

        var addTables = function (parentItem, collection) {
            if (collection) {
                collection.sort(selectDataForm.jsObject.SortByName);

                for (var k = 0; k < collection.length; k++) {
                    var tableItem = selectDataForm.jsObject.TreeItemWithCheckBox(collection[k].name, "Data.Data" + collection[k].typeItem + ".png", collection[k], tree);
                    parentItem.addChild(tableItem);

                    var relations = collection[k].relations;
                    if (relations) addRelations(tableItem, relations);

                    var columns = collection[k].columns;
                    if (columns) addColumns(tableItem, columns);
                }
            }
        }

        progress.style.display = "none";
        selectDataForm.buttonOk.setEnabled(true);

        if (!data || jsObject.GetCountObjects(data) == 0) {
            var emptyText = document.createElement("div");
            emptyText.className = "stiCreateDataHintText";
            emptyText.setAttribute("style", "position:absolute; width:300px; left:calc(50% - 150px); top:50%;");
            emptyText.innerHTML = jsObject.loc.Errors.DataNotLoaded;
            tree.appendChild(emptyText);
            return;
        }

        findTextbox.value = "";
        findTextbox.style.display = "";
        findTextbox.focus();
        tree.style.height = "353px";

        for (var i = 0; i < selectDataForm.rootItems.length; i++) {
            var caption = jsObject.loc.QueryBuilder.Collections;
            if (selectDataForm.typeConnection != "StiMongoDbDatabase") {
                switch (selectDataForm.rootItems[i]) {
                    case "Tables": caption = jsObject.loc.QueryBuilder.Tables; break;
                    case "Views": caption = jsObject.loc.QueryBuilder.Views; break;
                    case "Queries": caption = jsObject.loc.FormDictionaryDesigner.Queries; break;
                    default: caption = selectDataForm.rootItems[i]; break;
                }
            }

            var rootItem = jsObject.TreeItemWithCheckBox(caption, "Data.Data" + selectDataForm.rootItems[i] + ".png", null, tree);
            tree["rootItem" + selectDataForm.rootItems[i]] = rootItem;
            tree.appendChild(rootItem);

            if (!data || !data[selectDataForm.rootItems[i]]) {
                rootItem.style.display = "none";
            }

            if (data && data[selectDataForm.rootItems[i]]) {
                rootItem.setOpening(true);
                addTables(rootItem, data[selectDataForm.rootItems[i]]);
            }
        }
    }

    selectDataForm.action = function () {
        var resultCollection = [];

        for (var i = 0; i < selectDataForm.rootItems.length; i++) {
            var rootItem = tree["rootItem" + selectDataForm.rootItems[i]];
            if (!rootItem) continue;

            for (var tableKey in rootItem.childs) {
                var addTable = false;
                var tableItem = rootItem.childs[tableKey];
                if (tableItem.isChecked) addTable = true;

                var columns = [];
                var relations = [];
                var allColumns = [];
                for (var childKey in tableItem.childs) {
                    var childItem = tableItem.childs[childKey];
                    if (childItem.itemObject.typeItem == "Relation") {
                        if (childItem.isChecked) {
                            addTable = true;
                            relations.push(childItem.itemObject);
                        }
                    }
                    else {
                        allColumns.push(childItem.itemObject);
                        if (childItem.isChecked) {
                            addTable = true;
                            columns.push(childItem.itemObject);
                        }
                    }
                }
                columns.sort(selectDataForm.jsObject.SortByName);
                relations.sort(selectDataForm.jsObject.SortByName);

                if (addTable) {
                    var table = tableItem.itemObject;
                    table.countAllColumns = allColumns.length;
                    table.columns = columns;
                    table.relations = relations;
                    table.allColumns = allColumns;
                    resultCollection.push(table);
                }
            }
        }

        selectDataForm.changeVisibleState(false);
        if (resultCollection.length > 0) selectDataForm.jsObject.SendCommandApplySelectedData(resultCollection, selectDataForm.databaseName);
    }

    return selectDataForm;
}