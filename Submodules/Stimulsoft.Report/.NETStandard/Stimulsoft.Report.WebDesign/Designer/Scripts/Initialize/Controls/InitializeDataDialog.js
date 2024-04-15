
StiMobileDesigner.prototype.DataTree = function (width, height) {
    var jsObject = this;
    var dataTree = this.Tree();
    if (width) dataTree.style.width = width + "px";
    if (height) dataTree.style.height = height + "px";
    dataTree.style.padding = "12px";
    dataTree.key = "";
    dataTree.selectingItemType = "Column";
    dataTree.showNoneItem = true;

    dataTree.build = function (selectingItemType, dataSourceName, allowDragDrop, buildOnlyOneLevel) {
        this.clear();
        this.allowDragDrop = allowDragDrop;
        this.selectingItemType = (selectingItemType) ? selectingItemType : "Column";

        var mainItem = this.mainItem = jsObject.DataTreeItem("Main", null, { "typeItem": "MainItem" }, dataTree);
        mainItem.childsRow.style.display = "";
        mainItem.button.style.display = "none";
        mainItem.iconOpening.style.display = "none";
        this.appendChild(mainItem);

        if (dataTree.showNoneItem) {
            this.noneItem = jsObject.DataTreeItem(jsObject.loc.FormFormatEditor.nameNo, "ItemNo.png", { "typeItem": "NoItem" }, dataTree);
            mainItem.addChild(this.noneItem);
            this.noneItem.setSelected();
        }

        if (dataSourceName == "[Not Assigned]") return;
        if (jsObject.options.report == null) return;
        var dictionary = jsObject.options.report.dictionary;
        if (dictionary == null) return;

        if (dataSourceName != null) {
            var dataSource = jsObject.GetDataSourceByNameFromDictionary(dataSourceName);
            if (!dataSource) dataSource = jsObject.GetBusinessObjectByNameFromDictionary(dataSourceName);
            if (dataSource) this.addTreeItems([dataSource], mainItem);
        }
        else {
            if (selectingItemType != "BusinessObject") {
                for (var i = 0; i < dictionary.databases.length; i++) {
                    var dataBase = dictionary.databases[i];
                    this.addTreeItems(dataBase.dataSources, mainItem, buildOnlyOneLevel);
                }
            }
            this.addTreeItems(dictionary.businessObjects, mainItem);
        }
    }

    //Add Tree Items
    dataTree.addTreeItems = function (collection, parentItem, buildOnlyOneLevel) {
        if (collection) {
            if (jsObject.options.dictionarySorting == "ascending") {
                collection.sort(jsObject.SortByName);
            }
            else if (jsObject.options.dictionarySorting == "descending") {
                collection.sort(jsObject.SortByNameDescending);
            }

            for (var i = 0; i < collection.length; i++) {
                if (collection[i].typeItem == "Category") {
                    this.addTreeItems(collection[i].categoryItems, parentItem);
                    continue;
                }

                if (collection[i].typeItem != "DataBase") var childItem = parentItem.addChild(jsObject.DataTreeItemFromObject(collection[i], dataTree));
                var childCollection = [];

                switch (collection[i].typeItem) {
                    case "DataBase": { childCollection = collection[i].dataSources; childItem = parentItem; break; }
                    case "DataSource": { childCollection = collection[i].relations; break; }
                    case "Relation": { childCollection = collection[i].relations; break; }
                    case "BusinessObject": {
                        childCollection = collection[i].businessObjects;
                        break;
                    }
                }

                if (buildOnlyOneLevel) {
                    var showOpenIcon = childCollection.length > 0;

                    if (!showOpenIcon && childItem.itemObject.typeItem == "Relation") {
                        var dataSource = jsObject.GetDataSourceByNameFromDictionary(childItem.itemObject.parentDataSource);
                        if (dataSource && (dataSource.columns && dataSource.columns.length > 0)) {
                            showOpenIcon = true;
                        }
                    }

                    if (!showOpenIcon && (childItem.itemObject.columns && childItem.itemObject.columns.length > 0)) {
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

            if (parentItem.itemObject["columns"]) {
                this.addColumns(parentItem, parentItem.itemObject["columns"]);
                return;
            }

            if (parentItem.itemObject.typeItem == "Relation") {
                var dataSource = jsObject.GetDataSourceByNameFromDictionary(parentItem.itemObject.parentDataSource);
                if (dataSource) this.addColumns(parentItem, dataSource.columns);
            }
        }
    }

    dataTree.addColumns = function (item, columns) {
        if (columns) {
            for (var i = 0; i < columns.length; i++) {
                var columnItem = jsObject.DataTreeItemFromObject(columns[i], dataTree);
                item.addChild(columnItem);

                if (this.allowDragDrop) {
                    //Touch Start
                    columnItem.button.ontouchstart = function (event, mouseProcess) {
                        var this_ = this;
                        this.isTouchStartFlag = mouseProcess ? false : true;
                        clearTimeout(this.isTouchStartTimer);

                        if (event) event.preventDefault();
                        this.action();

                        if (event.button != 2) {
                            var itemInDragObject = jsObject.TreeItemForDragDrop(this.treeItem.itemObject, this.treeItem.tree);
                            var fullName = "";
                            var currItem = this.treeItem;

                            while (currItem.parent != null) {
                                if (fullName != "") fullName = "." + fullName;
                                fullName = (currItem.itemObject.correctName || currItem.itemObject.name) + fullName;
                                if (currItem.itemObject.typeItem == "BusinessObject" && currItem.parent != null &&
                                    currItem.parent.itemObject.typeItem != "BusinessObject" || currItem.itemObject.typeItem == "DataSource") break;
                                currItem = currItem.parent;
                            }

                            itemInDragObject.itemObject.columnFullName = fullName;
                            itemInDragObject.originalItem = this.treeItem;
                            itemInDragObject.beginingOffset = 0;
                            jsObject.options.itemInDrag = itemInDragObject;
                        }

                        this.isTouchStartTimer = setTimeout(function () {
                            this_.isTouchStartFlag = false;
                        }, 1000);
                    }

                    //Mouse Down
                    columnItem.button.onmousedown = function (event) {
                        if (this.isTouchStartFlag) return;
                        this.ontouchstart(event, true);
                    }
                }
            }
        }
    }

    dataTree.onSelectedItem = function (item) {
        var typeItem = item.itemObject.typeItem;
        if (typeItem == "NoItem" || typeItem != dataTree.selectingItemType) {
            this.key = "";
        }
        else {
            this.key = item.getFullName();
        }
    }

    dataTree.selectItemByFullName = function (fullName) {
        var nameArray = fullName.split(".");
        var parentItem = this.mainItem;
        if (!parentItem) return false;

        for (var i = 0; i < nameArray.length; i++) {
            parentItem = parentItem.getChildByName(nameArray[i], true);
            if (!parentItem) return false;
            if (parentItem.buildChildsNotCompleted) parentItem.completeBuildTree();
        }

        parentItem.setSelected();
        parentItem.openTree();
        return true;
    };

    dataTree.setKey = function (key) {
        this.key = key;
        if (this.key == "") {
            if (this.noneItem) this.noneItem.setSelected();
            return false;
        }
        return this.selectItemByFullName(key);
    }

    dataTree.action = function () {
    };

    return dataTree;
}

StiMobileDesigner.prototype.DataTreeItem = function (caption, imageName, itemObject, tree, showCheckBox) {
    var jsObject = this;
    var dataTreeItem = this.TreeItem(caption, imageName, itemObject, tree, showCheckBox);

    dataTreeItem.getBusinessObjectFullName = function () {
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

    dataTreeItem.button.ondblclick = function () {
        tree.onSelectedItem(dataTreeItem);
        tree.action();
    }

    //Override opening method
    dataTreeItem.iconOpening.action = function () {
        if (this.treeItem.tree.isDisable) return;
        this.treeItem.isOpening = !this.treeItem.isOpening;
        this.treeItem.childsRow.style.display = this.treeItem.isOpening ? "" : "none";
        var imgName = this.treeItem.isOpening ? "IconCloseItem.png" : "IconOpenItem.png";
        if (jsObject.options.isTouchDevice) imgName = imgName.replace(".png", "Big.png");
        StiMobileDesigner.setImageSource(this.treeItem.iconOpening, jsObject.options, imgName);
        this.treeItem.setSelected();

        if (this.treeItem.isOpening && this.treeItem.buildChildsNotCompleted && this.treeItem.childCollection) {
            if (this.treeItem.itemObject.typeItem == "DataBase") {
                this.treeItem.tree.addTreeItems(this.treeItem.childCollection, this.treeItem, true);
            }
            else {
                var dataSource = jsObject.GetDataSourceByNameAndNameInSourceFromDictionary(this.treeItem.itemObject.name, this.treeItem.itemObject.nameInSource);
                if (dataSource) {
                    this.treeItem.childCollection = jsObject.CopyObject(dataSource.relations);
                    this.treeItem.itemObject.columns = jsObject.CopyObject(dataSource.columns);
                    this.treeItem.itemObject.parameters = jsObject.CopyObject(dataSource.parameters);
                }
                this.treeItem.tree.addTreeItems(this.treeItem.childCollection, this.treeItem);
            }
            delete this.treeItem.buildChildsNotCompleted;
            delete this.treeItem.childCollection;
        }
    }

    dataTreeItem.completeBuildTree = function (buildOnlyOneLevel) {
        if (this.buildChildsNotCompleted && this.childCollection) {
            this.tree.addTreeItems(this.childCollection, this, buildOnlyOneLevel);
            delete this.buildChildsNotCompleted;
            delete this.childCollection;
        }
    }

    return dataTreeItem;
}

StiMobileDesigner.prototype.DataTreeItemFromObject = function (itemObject, tree) {
    var dataTreeItem = this.DataTreeItem(this.options.dictionaryTree.getItemCaption(itemObject), itemObject.typeIcon + ".png", itemObject, tree);

    return dataTreeItem;
}