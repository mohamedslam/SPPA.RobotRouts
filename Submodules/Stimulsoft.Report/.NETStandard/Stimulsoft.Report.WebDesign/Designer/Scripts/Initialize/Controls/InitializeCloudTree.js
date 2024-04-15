
StiMobileDesigner.prototype.CloudTree = function () {
    var tree = document.createElement("div");
    var jsObject = tree.jsObject = this;    
    tree.style.position = "absolute";
    tree.style.top = tree.style.bottom = tree.style.left = tree.style.right = "0px";
    tree.selectedItem = null;
    tree.returnItems = {};
    tree.sortType = "Name";
    tree.sortDirection = "Ascending";

    var toolBar = this.CreateHTMLTable();
    tree.appendChild(toolBar);

    //Find
    var findControl = tree.findControl = this.FindControl(null, 400, 26);
    findControl.style.margin = "12px";
    toolBar.addCell(findControl);

    findControl.textBox.onchange = function () {
        findControl.action();
    }
        
    //New Folder
    var newFolderButton = tree.newFolderButton = jsObject.FormButton(null, null, this.loc.Cloud.FolderWindowTitleNew, null, null, null, true);
    newFolderButton.style.margin = "12px 0 12px 0";
    newFolderButton.style.height = "26px";
    toolBar.addCell(newFolderButton).style.lineHeight = "0";

    newFolderButton.action = function () {
        jsObject.InitializeNewFolderForm(function (newFolderForm) {
            newFolderForm.show(function () {
                if (newFolderForm.nameTextBox.value) {
                    var newFolderKey = jsObject.generateKey();

                    var param = {};
                    param.AllowSignalsReturn = true;
                    param.Items = [{
                        Ident: "FolderItem",
                        Name: newFolderForm.nameTextBox.value,
                        Description: "",
                        Type: "Common",
                        Key: newFolderKey
                    }];

                    if (tree.rootItem && tree.rootItem.itemObject.Key != "root") {
                        param.Items[0].FolderKey = tree.rootItem.itemObject.Key;
                    }

                    if ((jsObject.options.cloudMode || jsObject.options.standaloneJsMode) && (!jsObject.CheckUserTrExpired() || !jsObject.CheckUserActivated()))
                        return;

                    tree.progress.show();
                    newFolderForm.changeVisibleState(false);

                    jsObject.SendCloudCommand("ItemSave", param,
                        function (data) {
                            tree.progress.hide();
                            if (data.ResultItems && data.ResultItems.length > 0) {
                                tree.build(tree.rootItem.itemObject, tree.returnItems[tree.rootItem.itemObject.Key], newFolderKey);
                            }
                        },
                        function (data, msg) {
                            tree.progress.hide();
                            if (msg || data) {
                                var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                                errorMessageForm.show(msg || jsObject.formatResultMsg(data));
                            }
                        });
                }
                else {
                    tree.progress.hide();
                    var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                    errorMessageForm.show(jsObject.loc.Errors.FieldRequire.replace("{0}", jsObject.loc.PropertyMain.Name));
                }
            })
        });
    }

    var headTable = this.CreateHTMLTable();
    headTable.style.width = "calc(100% - 24px)";
    headTable.style.margin = "0 12px 0 12px";
    headTable.className = "stiDesignerCloudTreeItemBorder";
    tree.appendChild(headTable);

    var nameButton = this.CloudTreeHeaderButton(this.loc.PropertyMain.Name);
    headTable.addCell(nameButton);

    nameButton.action = function () {
        if (tree.sortType == "Name") {
            tree.sortDirection = tree.sortDirection == "Ascending" ? "Descending" : "Ascending";
        }
        else {
            tree.sortType = "Name";
            tree.sortDirection = "Ascending";
        }
        tree.build(tree.rootItemObject, tree.returnItemObject, tree.selectItemKey);
    }

    var dateButton = this.CloudTreeHeaderButton(this.loc.RecentFiles.DateModified);
    dateButton.style.marginRight = "30px";
    headTable.addCell(dateButton).style.textAlign = "right";

    dateButton.action = function () {
        if (tree.sortType == "ModificationDate") {
            tree.sortDirection = tree.sortDirection == "Ascending" ? "Descending" : "Ascending";
        }
        else {
            tree.sortType = "ModificationDate";
            tree.sortDirection = "Ascending";
        }
        tree.build(tree.rootItemObject, tree.returnItemObject, tree.selectItemKey);
    }

    var innerContainer = document.createElement("div");
    innerContainer.style.overflow = "auto";
    innerContainer.style.position = "absolute";
    innerContainer.style.top = "100px";
    innerContainer.style.bottom = "0px";

    tree.appendChild(innerContainer);
    tree.innerContainer = innerContainer;

    findControl.action = function () {
        var value = this.getValue().toLowerCase();
        for (var i = 0; i < innerContainer.childNodes.length; i++) {
            var item = innerContainer.childNodes[i];
            if (item.itemObject && item.itemObject.Ident == "ReturnItem") continue;
            item.style.display = value == "" || (item.caption && item.caption.innerHTML.toLowerCase().indexOf(value) >= 0) ? "" : "none";
        }
    }

    tree.clear = function () {
        while (innerContainer.childNodes[0]) innerContainer.removeChild(innerContainer.childNodes[0]);
        tree.selectedItem = null;
    }

    tree.getOptionsForTree = function (rootItemKey) {
        var optionsObject = {
            Options: {
                SortType: tree.sortType,
                SortDirection: tree.sortDirection,
                ViewMode: "All",
                FilterIdent: "ReportTemplateItem"
            }
        }
        if (rootItemKey) optionsObject.ItemKey = rootItemKey;

        return optionsObject;
    }

    tree.addItems = function (rootItemObject, returnItemObject, dataItems) {
        tree.clear();
        tree.rootItem = jsObject.CloudTreeItem(tree, rootItemObject || { Ident: "RootAllItem", Name: "RootFolder", Key: "root" });

        if (tree.rootItem.itemObject.Key != "root" && returnItemObject) {
            var returnItem = jsObject.CloudTreeItem(tree, { Ident: "ReturnItem", Name: ". . .", Key: returnItemObject.Key + "ReturnItem", ReturnObject: returnItemObject });
            innerContainer.appendChild(returnItem);
            tree.returnItems[tree.rootItem.itemObject.Key] = returnItemObject;
        }

        if (dataItems) {
            for (var i = 0; i < dataItems.length; i++) {
                var item = jsObject.CloudTreeItem(tree, dataItems[i]);
                innerContainer.appendChild(item);
            }
        }
    }   

    tree.build = function (rootItemObject, returnItemObject, selectItemKey) {        
        tree.clear();
        tree.progress.show(null, null);

        tree.rootItemObject = rootItemObject;
        tree.returnItemObject = returnItemObject;
        tree.selectItemKey = selectItemKey;

        nameButton.showSortDirection(tree.sortType == "Name" ? tree.sortDirection : null);
        dateButton.showSortDirection(tree.sortType == "ModificationDate" ? tree.sortDirection : null);
                
        jsObject.SendCloudCommand("ItemFetchAll", this.getOptionsForTree(rootItemObject && rootItemObject.Key != "root" ? rootItemObject.Key : null),
            function (data) {
                if (data.ResultItems) {
                    tree.progress.hide();
                    tree.addItems(rootItemObject, returnItemObject, data.ResultItems);
                    tree.onbuildcomplete();
                    if (selectItemKey) {
                        for (var i = 0; i < innerContainer.childNodes.length; i++) {
                            if (innerContainer.childNodes[i].itemObject && selectItemKey == innerContainer.childNodes[i].itemObject.Key) {
                                innerContainer.childNodes[i].action();
                            }
                        }
                    }
                    findControl.action();
                }
            },
            function (data, msg) {
                tree.progress.hide();
                if (msg || data) {
                    var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                    errorMessageForm.show(msg || jsObject.formatResultMsg(data));
                }
            });
    }

    tree.correctHeight = function (ownerForm) {
        var treeHeight = this.offsetHeight;
        var formTop = parseInt(ownerForm.style.top.replace("px", ""));
        if (formTop + ownerForm.offsetHeight > jsObject.options.mainPanel.offsetHeight) {
            var deltaY = ownerForm.offsetHeight + formTop - jsObject.options.mainPanel.offsetHeight;
            this.style.height = ((deltaY > 0 && treeHeight - deltaY > 200) ? (treeHeight - deltaY - 30) : 200) + "px";
        }
    }

    tree.action = function (item) { }
    tree.ondblClickAction = function (item) { }
    tree.onbuildcomplete = function () { }

    return tree;
}

StiMobileDesigner.prototype.CloudTreeItem = function (tree, itemObject) {
    var caption = itemObject.Name;
    var image = "CloudItems.BigFile.png";
    switch (itemObject.Ident) {
        case "ReportTemplateItem": {
            if (itemObject.ContentType == "Dashboard")
                image = "Open.DashboardCloud.png";
            else if (itemObject.ContentType == "Form")
                image = "Open.Forms.png";
            else
                image = "Open.ReportCloud.png";
            break;
        }
        case "FolderItem": {
            image = "CloudItems.BigFolder.png";
            break;
        }
        case "ReturnItem": {
            image = "CloudItems.ReturnItem.png";
            break;
        }
    }

    var button = this.SmallButton(null, null, caption, image, caption, null, null, true, { width: 32, height: 32 });
    if (itemObject.Ident == "ReportTemplateItem") button.style.cursor = "pointer";
    if (button.imageCell) button.imageCell.style.padding = "0 10px 0 10px";
    if (button.caption) {
        button.caption.style.padding = "0 20px 0 10px";
        button.caption.style.lineHeight = "1.4";
    }

    button.tree = tree;
    button.itemObject = itemObject;
    button.style.minWidth = "250px";
    button.style.height = "55px";
    button.style.margin = "0px 12px 0 12px";

    button.innerTable.style.width = "100%";
    button.innerTable.className = "stiDesignerCloudTreeItemBorder";

    var dateCell = button.innerTable.addCell();
    dateCell.style.width = "100%";
    dateCell.style.textAlign = "right";
    dateCell.style.paddingRight = "30px";
    dateCell.innerHTML = itemObject.Created ? this.JSONDateFormatToDate(itemObject.Created, "dd.MM.yyyy h:nn") : "";

    button.ondblclick = function () {
        if (this.itemObject.Ident == "FolderItem") {
            tree.findControl.setValue("");
            tree.build(this.itemObject, this.tree.rootItem.itemObject);
        }
        if (this.itemObject.Ident == "ReturnItem") {
            tree.findControl.setValue("");
            tree.build(this.itemObject.ReturnObject, this.tree.returnItems[this.itemObject.ReturnObject.Key]);
        }
        tree.ondblClickAction(this);
        if (event) event.stopPropagation();
    }

    button.action = function () {
        if (this.tree.selectedItem) this.tree.selectedItem.setSelected(false);
        this.setSelected(true);
        this.tree.selectedItem = this;
        this.tree.action(this);
    }


    return button;
}

StiMobileDesigner.prototype.CloudTreeHeaderButton = function (caption) {
    var button = this.SmallButton(null, null, caption, null, null, "Down");    
    button.style.overflow = "hidden";
    button.style.padding = "0 4px 0 4px";
    button.style.display = "inline-block";

    var sortImage = button.arrow;
    StiMobileDesigner.setImageSource(sortImage, this.options, "DataTransformation.SortAsc.png");
    sortImage.style.visibility = "hidden";
    sortImage.style.width = "5px";
    sortImage.style.height = "12px";
        
    button.showSortDirection = function (sortDirection) {
        sortImage.style.visibility = sortDirection == "Ascending" || sortDirection == "Descending" ? "visible" : "hidden";
        StiMobileDesigner.setImageSource(sortImage, this.jsObject.options, sortDirection == "Ascending" ? "DataTransformation.SortAsc.png" : "DataTransformation.SortDesc.png");
    }

    return button;
}