
StiMobileDesigner.prototype.InitializeNameInSourceForm_ = function () {

    //nameInSource Form
    var nameInSourceForm = this.BaseForm("nameInSourceForm", this.loc.PropertyMain.NameInSource, 4);
    nameInSourceForm.connections = {};
    nameInSourceForm.nameInSource = null;

    var connectionsTree = this.Tree();
    connectionsTree.className = "stiDesignerConnectionsTree";
    connectionsTree.style.margin = "12px";
    nameInSourceForm.connectionsTree = connectionsTree;
    nameInSourceForm.container.appendChild(connectionsTree);

    connectionsTree.build = function (connections) {
        var mainItem = this.jsObject.ConnectionsTreeItem(this.jsObject.loc.PropertyMain.DataSources, "DataSource.png", this.jsObject.NameInSourceObject("Main", false), this);
        mainItem.style.marginLeft = "4px";
        this.appendChild(mainItem);
        for (var nameParent in connections) {
            var parentItem = mainItem.addChild(this.jsObject.ConnectionsTreeItem(nameParent, "Connection.png", this.jsObject.NameInSourceObject(nameParent, false), this));
            for (var i = 0; i < connections[nameParent].length; i++) {
                var nameChild = connections[nameParent][i];
                var childItem = parentItem.addChild(this.jsObject.ConnectionsTreeItem(nameChild, "DataSource.png", this.jsObject.NameInSourceObject(nameChild, true), this));
                childItem.ondblclick = function () {
                    if (!this.jsObject.options.isTouchDevice && nameInSourceForm.nameInSource != null) nameInSourceForm.action();
                }
                if (i == 0) childItem.openTree();
            }
        }
    }

    connectionsTree.onSelectedItem = function (item) {
        var nameInSourceForm = this.jsObject.options.forms.nameInSourceForm;
        nameInSourceForm.nameInSource = (item.itemObject.isCanSelectingItem) ? item.itemObject.name : null;
        nameInSourceForm.buttonOk.setEnabled(nameInSourceForm.nameInSource != null);
    }

    nameInSourceForm.onshow = function () {
        this.nameInSource = null;
        this.connectionsTree.clear();
        if (this.jsObject.GetCountObjects(this.connections) > 0) {
            this.connectionsTree.build(this.connections);
            this.buttonOk.setEnabled(false);
        }
        else
            this.buttonOk.setEnabled(true);
    }

    nameInSourceForm.action = function () {
        var this_ = this;
        if (this.nameInSource) {
            this.jsObject.InitializeEditDataSourceForm(function (editDataSourceForm) {
                editDataSourceForm.nameInSourceControl.textBox.value = this_.nameInSource;
                this_.changeVisibleState(false);
            });
        }
        else {
            this.changeVisibleState(false);
        }
    }

    return nameInSourceForm;
}

StiMobileDesigner.prototype.ConnectionsTreeItem = function (caption, imageName, itemObject, tree) {
    var connectionsTreeItem = this.TreeItem(caption, imageName, itemObject, tree);

    return connectionsTreeItem;
}

StiMobileDesigner.prototype.NameInSourceObject = function (name, isCanSelectingItem) {
    return {
        "name": name,
        "isCanSelectingItem": isCanSelectingItem
    }
}