
StiMobileDesigner.prototype.TreeControl = function (name, width, showFind, descriptionText) {
    var jsObject = this;
    var treeControl = this.DropDownList(name, width, null, [], true, false, null, true);

    var findTextbox = this.TextBox(null);
    findTextbox.setAttribute("placeholder", this.loc.FormViewer.Find);
    findTextbox.style.margin = "12px";
    findTextbox.style.width = "calc(100% - 35px)";
    findTextbox.style.display = showFind ? "" : "none";
    treeControl.menu.innerContent.appendChild(findTextbox);
    treeControl.menu.innerContent.style.maxHeight = "";
    treeControl.menu.innerContent.style.width = "auto";

    if (descriptionText) {
        var desc = document.createElement("div");
        desc.style.width = "calc(100% - 35px)";
        desc.className = "stiCreateDataHintText";
        findTextbox.style.margin = "12px 12px 4px 12px";
        desc.style.margin = "0 12px 4px 12px";
        desc.style.textAlign = "left";
        desc.innerHTML = descriptionText;
        treeControl.menu.innerContent.appendChild(desc);
        var sep = this.FormSeparator();
        treeControl.menu.innerContent.appendChild(sep);
    }

    //Tree
    var tree = this.Tree();
    tree.style.overflow = "auto";
    tree.style.width = (width > 250 ? (width - 24) : 250) + "px";
    tree.style.height = "300px";
    tree.style.margin = "0 12px 12px 12px";
    treeControl.menu.innerContent.appendChild(tree);

    tree.onChecked = function () {
        tree.isChanged = true;
    }

    findTextbox.onchange = function () {
        //close groups for prev find results
        if (tree.openingItems) {
            for (var key in tree.openingItems) {
                tree.openingItems[key].setOpening(false);
            }
        }
        tree.openingItems = {};

        //find items
        var findValue = findTextbox.value.toLowerCase();
        for (var key in tree.items) {
            var itemObject = tree.items[key].itemObject;
            if (itemObject && itemObject.key) {
                var itemKey = itemObject.key.toLowerCase();
                tree.items[key].style.display = (findValue == "" || itemKey.indexOf(findValue) >= 0 || itemKey.indexOf(findValue) >= 0) ? "" : "none";
            }
        }

        //open groups
        for (var key in tree.items) {
            var itemObject = tree.items[key].itemObject;
            if (itemObject && itemObject.groupName) {
                var showGroupItem = false;
                var childs = tree.items[key].childs;
                for (var childKey in childs) {
                    if (childs[childKey].style.display == "") {
                        showGroupItem = true;
                    }
                }
                tree.items[key].style.display = showGroupItem ? "" : "none";
                if (findValue != "" && showGroupItem && !tree.items[key].isOpening) {
                    tree.items[key].setOpening(true);
                    tree.openingItems[key] = tree.items[key]
                }
            }
        }
    }

    treeControl.buildTree = function (data) {
        tree.clear();

        var rootItem = jsObject.TreeItemWithCheckBox(" ", null, null, tree);
        rootItem.button.style.display = rootItem.iconOpening.style.display = "none";
        rootItem.setOpening(true);
        tree.rootItem = rootItem;
        tree.appendChild(rootItem);

        var groupNames = [];
        for (var groupName in data) {
            groupNames.push(groupName);
        }
        groupNames.sort();

        for (var k = 0; k < groupNames.length; k++) {
            var groupName = groupNames[k];
            var groupItem = jsObject.TreeItemWithCheckBox(groupName, null, { groupName: groupName }, tree);
            rootItem.addChild(groupItem);

            var groupItems = data[groupName];
            for (var i = 0; i < groupItems.length; i++) {
                var item = jsObject.TreeItemWithCheckBox(groupItems[i].key, null, groupItems[i], tree);
                item.setAttribute("title", groupItems[i].description);
                groupItem.addChild(item);
            }
        }
    }

    treeControl.setValue = function (value) {
        this.textBox.value = value;
        var values = value ? value.split(",") : [];
        for (var i = 0; i < values.length; i++) {
            values[i] = values[i].trim();
        }
        for (var key in tree.items) {
            var item = tree.items[key];
            if (item.itemObject) {
                item.setOpening(false);
                item.setChecked(false);
            }
        }
        for (var key in tree.items) {
            var item = tree.items[key];
            var itemObject = item.itemObject;
            if (itemObject && itemObject.key) {
                var isChecked = values.indexOf(itemObject.key.trim()) >= 0;
                if (isChecked) {
                    item.setChecked(true);
                    item.parent.setOpening(true);
                    item.parent.setChecked(true);
                }
            }
        }
    }

    treeControl.getValue = function () {
        var result = "";
        for (var key in tree.items) {
            var item = tree.items[key];
            if (item.itemObject && item.itemObject.key && item.isChecked) {
                if (result != "") result += ", ";
                result += item.itemObject.key;
            }
        }
        return result;
    }

    treeControl.menu.onshow = function () {
        findTextbox.focus();
        tree.isChanged = false;
    }

    treeControl.menu.onhide = function () {
        if (tree.isChanged) {
            treeControl.textBox.value = treeControl.getValue();
        }
    }

    return treeControl;
}