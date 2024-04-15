
StiJsViewer.prototype.CreateTreeViewElementContent = function (element) {
    this.CreateTreeViewItemsContent(element, element.contentPanel);
}

StiJsViewer.prototype.CreateTreeViewItemsContent = function (element, parentPanel, actionFunction) {
    var jsObject = this;
    var elementAttrs = element.elementAttributes;
    var contentAttrs = elementAttrs.contentAttributes;
    var itemAllValue;

    while (parentPanel.childNodes[0]) {
        parentPanel.removeChild(parentPanel.childNodes[0]);
    }

    var tree = jsObject.TreeViewElementTree(elementAttrs);
    element.itemsPanel = tree;

    tree.onChecked = function (item) {
        if (item.id == "allValue") {
            element.setStatesForAllItems(item.isChecked);
        }
        else if (itemAllValue) {
            var itemsStates = element.getAllItemsCheckedState();
            itemAllValue.setChecked(itemsStates.unCheckedItems.length == 0);
        }
        tree.updateCheckBoxesStates();

        if (actionFunction) actionFunction();

        if (element.menu && element.menu.visible) {
            element.menu.isModified = true;
        }
        else {
            clearTimeout(element.actionTimer);
            element.actionTimer = setTimeout(function () { jsObject.ApplyFiltersToDashboardElement(element, element.getFilters()); }, 500);
        }
    }

    tree.onActionItem = function (item) {
        if (item && item.itemObject) {
            var filters = item.id != "allValue" ? [{ condition: "EqualTo", value: item.itemObject.key, path: item.itemObject.columnPath }] : [];
            jsObject.ApplyFiltersToDashboardElement(element, filters);
        }
        if (actionFunction) actionFunction();
    }

    //Helper methods
    element.setStatesForAllItems = function (state) {
        if (contentAttrs.selectionMode == "One") {
            if (!state) tree.mainItem.setSelected();
        }
        else {
            for (var itemKey in tree.items) {
                if (tree.items[itemKey].setChecked) {
                    tree.items[itemKey].setChecked(state);
                }
            }
        }
    }

    element.applyFiltersToItems = function () {
        var state = contentAttrs.selectionMode == "Multi";

        for (var itemKey in tree.items) {
            var item = tree.items[itemKey];
            var itemObject = item.itemObject;
            if (item.id == "mainItem" || item.id == "allValue" || !itemObject) continue;

            //Mode Multi
            if (contentAttrs.selectionMode == "Multi") {
                state = contentAttrs.filters[0].condition == "NotEqualTo" || contentAttrs.filters[0].condition == "IsFalse";

                if (state && contentAttrs.filters.some(function (f) { return ((f.condition == "NotEqualTo" && f.value == itemObject.key && f.path == itemObject.columnPath) || f.condition == "IsFalse") })) {
                    item.setChecked(false);
                }
                else if (!state && contentAttrs.filters.some(function (f) { return (f.condition == "EqualTo" && f.value == itemObject.key && f.path == itemObject.columnPath) })) {
                    item.setChecked(true);
                }
                else {
                    item.setChecked(state);
                }

                if (itemAllValue && !item.isChecked) {
                    itemAllValue.setChecked(false);
                }
            }
            //Mode One
            else if (contentAttrs.filters.some(function (f) { return (f.condition == "EqualTo" && f.value == itemObject.key && f.path == itemObject.columnPath) })) {
                item.setSelected();
                item.openTree();
                tree.autoscroll();
                return;
            }
        }
    }

    element.getFilters = function () {
        var filters = [];
        var states = element.getAllItemsCheckedState();

        if (states.checkedItems.length == 0) {
            filters.push({ condition: "IsFalse", path: contentAttrs.columnPath });
        }
        else if (states.unCheckedItems.length > states.checkedItems.length) {
            for (var i = 0; i < states.checkedItems.length; i++) {
                filters.push({ condition: "EqualTo", value: states.checkedItems[i].itemObject.key, path: states.checkedItems[i].itemObject.columnPath });
            }
        }
        else {
            for (var i = 0; i < states.unCheckedItems.length; i++) {
                filters.push({ condition: "NotEqualTo", value: states.unCheckedItems[i].itemObject.key, path: states.unCheckedItems[i].itemObject.columnPath });
            }
        }

        return filters;
    }

    element.getAllItemsCheckedState = function () {
        var states = {
            checkedItems: [],
            unCheckedItems: []
        };

        for (var itemKey in tree.items) {
            var item = tree.items[itemKey];
            if (item.id == "mainItem" || item.id == "allValue" || !item.setChecked)
                continue;

            states[item.isChecked ? "checkedItems" : "unCheckedItems"].push(item);
        }

        return states;
    }

    if (contentAttrs.items) {
        if (contentAttrs.selectionMode == "Multi") {
            //Mode Multi
            if (contentAttrs.showAllValue) {
                itemAllValue = jsObject.TreeViewElementTreeItem(this.collections.loc.DashboardAllValue, {}, tree, true, "allValue", elementAttrs);
                parentPanel.appendChild(itemAllValue);
                parentPanel.appendChild(jsObject.TreeViewElementSeparator(elementAttrs));
                tree.style.top = (itemAllValue.offsetHeight + 5) + "px";
            }
            tree.addTreeItems(contentAttrs.items, tree.mainItem, true);
        }
        else {
            //Mode One
            if (contentAttrs.showAllValue) {
                itemAllValue = jsObject.TreeViewElementTreeItem(this.collections.loc.DashboardAllValue, {}, tree, null, "allValue", elementAttrs);
                tree.mainItem.addChild(itemAllValue);
                tree.mainItem.childsContainer.appendChild(jsObject.TreeViewElementSeparator(elementAttrs));
            }
            tree.addTreeItems(contentAttrs.items, tree.mainItem);
        }

        element.setStatesForAllItems(contentAttrs.selectionMode == "Multi");

        if (contentAttrs.filters.length > 0) {
            element.applyFiltersToItems();
            tree.updateCheckBoxesStates();
        }
        else if (itemAllValue && contentAttrs.selectionMode == "One") {
            itemAllValue.setSelected();
        }
    }

    parentPanel.appendChild(tree);
}

StiJsViewer.prototype.TreeViewElementTreeItem = function (caption, itemObject, tree, showCheckBox, id, elementAttrs) {
    var treeItem = this.TreeItem(caption, null, itemObject, tree, showCheckBox, id, elementAttrs.contentAttributes.settings);
    treeItem.style.width = "100%";
    treeItem.button.style.width = "100%";
    treeItem.childsContainer.style.width = "100%";

    if (elementAttrs) {
        this.ApplyAttributesToObject(treeItem.button, elementAttrs);

        if (elementAttrs.foreColor && treeItem.checkBox) {
            treeItem.checkBox.imageBlock.style.borderColor = elementAttrs.foreColor;
            treeItem.checkBox.imageBlock.style.background = "transparent";
        }
    }

    return treeItem;
}

StiJsViewer.prototype.TreeViewElementTree = function (elementAttrs) {
    var tree = this.Tree();
    var jsObject = tree.jsObject = this;
    tree.style.position = "absolute";
    tree.style.left = tree.style.top = tree.style.right = tree.style.bottom = "0px";
    tree.style.overflow = "auto";
    tree.className = "stiJsViewerScrollContainer";

    var mainItem = jsObject.TreeViewElementTreeItem(" ", { typeItem: "mainItem" }, tree, null, "mainItem", elementAttrs);
    mainItem.childsRow.style.display = "";
    mainItem.button.style.display = "none";
    mainItem.iconOpening.style.display = "none";
    mainItem.style.width = "auto";
    tree.appendChild(mainItem);
    tree.mainItem = mainItem;

    tree.addTreeItems = function (collection, parentItem, showCheckBoxes) {
        if (collection) {
            for (var i = 0; i < collection.length; i++) {
                var childItem = jsObject.TreeViewElementTreeItem(collection[i].text, collection[i], tree, showCheckBoxes, null, elementAttrs);
                parentItem.addChild(childItem);

                if (collection[i].items) {
                    this.addTreeItems(collection[i].items, childItem, showCheckBoxes);
                }
            }
        }
    }

    tree.getItemsLastLevel = function () {
        var items = [];

        for (var itemKey in tree.items) {
            var item = tree.items[itemKey];
            if (item.id == "mainItem" || item.id == "allValue" || !item.setChecked || jsObject.getCountObjects(item.childs) > 0)
                continue;

            items.push(item);
        }

        return items;
    }

    tree.updateCheckBoxesStates = function (startParents) {
        if (!startParents) {
            startParents = [];

            for (var itemKey in tree.items) {
                var item = tree.items[itemKey];
                if (item.id == "mainItem" || item.id == "allValue" || !item.setChecked || jsObject.getCountObjects(item.childs) > 0)
                    continue;

                if (item.parent && startParents.indexOf(item.parent) < 0) startParents.push(item.parent);
            }
        }

        var nextLevelParents = [];
        var allValueIsIndeterminate = false;
        var allParentsUnChecked = true;

        for (var i = 0; i < startParents.length; i++) {
            var parentItem = startParents[i];
            if (!parentItem.setChecked) continue;

            if (parentItem.parent && parentItem.parent.id != "allValue" &&
                parentItem.parent.id != "mainItem" && nextLevelParents.indexOf(parentItem.parent) < 0) {
                nextLevelParents.push(parentItem.parent);
            }

            var checkedCount = 0;
            var unCheckedCount = 0;
            var indeterminateCount = 0;

            for (var itemKey in parentItem.childs) {
                var child = parentItem.childs[itemKey]
                if (child.isIndeterminate) indeterminateCount++;
                else if (child.isChecked) checkedCount++;
                else unCheckedCount++;
            }

            if (indeterminateCount > 0 || (checkedCount > 0 && unCheckedCount > 0)) {
                parentItem.setIndeterminate(true);
                allValueIsIndeterminate = true;
                allParentsUnChecked = false;
            }
            else {
                parentItem.setChecked(unCheckedCount == 0);
                if (unCheckedCount > 0) allValueIsIndeterminate = true;
                if (checkedCount > 0) allParentsUnChecked = false;
            }
        }

        if (nextLevelParents.length > 0) {
            tree.updateCheckBoxesStates(nextLevelParents);
        }
        else if (tree.items["allValue"] && allValueIsIndeterminate && !allParentsUnChecked) {
            tree.items["allValue"].setIndeterminate(true);
        }
    }

    return tree;
}

StiJsViewer.prototype.TreeViewElementSeparator = function (elementAttrs) {
    var sep = this.ListBoxElementSeparator(elementAttrs);
    sep.style.margin = "2px";

    return sep;
}