
StiMobileDesigner.prototype.DictionaryPanel = function () {
    var dictionaryPanel = document.createElement("div");
    var jsObject = dictionaryPanel.jsObject = this;
    dictionaryPanel.className = "stiDesignerPropertiesPanelInnerContent";
    this.options.dictionaryPanel = dictionaryPanel;
    dictionaryPanel.style.top = "35px";

    //Progress
    var progressPanel = document.createElement("div");
    dictionaryPanel.progressPanel = progressPanel;
    progressPanel.className = "stiDesignerDictionaryProgressPanel";
    progressPanel.style.display = "none";
    dictionaryPanel.appendChild(progressPanel);
    this.AddProgressToControl(progressPanel);
    var progress = progressPanel.progress;

    progress.buttonCancel.action = function () {
        if (jsObject.xmlHttp) {
            jsObject.xmlHttpAbortedByUser = true;
            jsObject.xmlHttp.abort();
        }
        if (progress.commandGuid) jsObject.options.abortedCommands[progress.commandGuid] = true;
        progressPanel.style.display = "none";
        progress.hide();
    }

    dictionaryPanel.showProgress = function (progressValue, commandGuid) {
        if (progressPanel.style.display == "none") {
            progressPanel.style.display = "";
            progress.show(null, null, progressValue, commandGuid);
            progress.showCancelButton();
        }
        else {
            progress.progressText.innerHTML = progressValue ? parseInt(parseFloat(progressValue) * 100) + "%" : "";
        }
    }

    dictionaryPanel.hideProgress = function () {
        progressPanel.style.display = "none";
        progress.hide();
    }

    //Hint Text
    var createDataHintItem = this.CreateDataHintItem();
    dictionaryPanel.appendChild(createDataHintItem);
    dictionaryPanel.createDataHintItem = createDataHintItem;

    createDataHintItem.onclick = function () {
        if (jsObject.options.dictionaryShowMoreActivated) {
            return;
        }

        jsObject.InitializeSelectConnectionForm(function (selectConnectionForm) {
            selectConnectionForm.changeVisibleState(true);
        });
    }

    //Show More
    var showMoreButton = this.SmallButton("dictionaryShowMore", null, this.loc.Buttons.ShowMore, null, null, null, "stiDesignerHyperlinkButton");
    showMoreButton.caption.className = "stiCreateDataHintHeaderText";
    showMoreButton.style.marginTop = "10px";
    showMoreButton.style.display = "inline-block";
    createDataHintItem.addCellInNextRow(showMoreButton).style.textAlign = "center";

    showMoreButton.action = function () {
        jsObject.options.dictionaryShowMoreActivated = true;
        dictionaryPanel.toolBar.updateControls();
    }

    //Main Dictionary
    dictionaryPanel.mainDictionary = document.createElement("div");
    dictionaryPanel.appendChild(dictionaryPanel.mainDictionary);

    //Dictionary Toolbar
    var toolBar = dictionaryPanel.toolBar = this.CreateHTMLTable();
    toolBar.jsObject = this;
    toolBar.controls = {};
    dictionaryPanel.mainDictionary.appendChild(toolBar);

    //Items Container
    var dictionaryItemsContainer = document.createElement("div");
    dictionaryItemsContainer.className = "stiDesignerDictionaryItemsContainer";
    dictionaryItemsContainer.style.top = this.options.isTouchDevice ? "35px" : "34px";
    dictionaryPanel.mainDictionary.appendChild(dictionaryItemsContainer);
    dictionaryItemsContainer.appendChild(this.DictionaryTree());

    //Description Panel
    var descriptionPanel = document.createElement("div");
    descriptionPanel.style.display = "none";
    descriptionPanel.className = "stiDesignerDictionaryDescriptionPanel";
    dictionaryPanel.mainDictionary.appendChild(descriptionPanel);
    dictionaryPanel.descriptionPanel = descriptionPanel;

    descriptionPanel.show = function (text) {
        descriptionPanel.style.display = "";
        dictionaryItemsContainer.style.bottom = "121px";
        descriptionPanel.innerHTML = text;
    }

    descriptionPanel.hide = function () {
        descriptionPanel.style.display = "none";
        dictionaryItemsContainer.style.bottom = "0px";
    }

    toolBar.updateControls = function (currentMenu) {
        var selectedItem = jsObject.options.dictionaryTree.selectedItem;
        if (selectedItem == null) return;

        var selectedItemObject = selectedItem.itemObject;
        var typeItem = selectedItemObject.typeItem;
        var isNoSorting = !jsObject.options.dictionarySorting || jsObject.options.dictionarySorting == "noSorting";

        this.controls["MoveUp"].setEnabled(selectedItem.canMove("Up") && isNoSorting);
        this.controls["MoveDown"].setEnabled(selectedItem.canMove("Down") && isNoSorting);

        //Permissions
        var permissionDataSources = jsObject.options.permissionDataSources;
        var permissionDataTransformations = jsObject.options.permissionDataTransformations;
        var permissionDataConnections = jsObject.options.permissionDataConnections;
        var permissionBusinessObjects = jsObject.options.permissionBusinessObjects;
        var permissionVariables = jsObject.options.permissionVariables;
        var permissionDataRelations = jsObject.options.permissionDataRelations;
        var permissionDataColumns = jsObject.options.permissionDataColumns;
        var permissionResources = jsObject.options.permissionResources;
        var permissionSqlParameters = jsObject.options.permissionSqlParameters;

        var getPermissionResult = function (selectedItemObj, permType) {
            var typeI = selectedItemObj.typeItem;
            return (typeI == "DataBase" && !(!permissionDataConnections || permissionDataConnections.indexOf("All") >= 0 || permissionDataConnections.indexOf(permType) >= 0)) ||
                (typeI == "DataSource" && !(!permissionDataSources || permissionDataSources.indexOf("All") >= 0 || permissionDataSources.indexOf(permType) >= 0)) ||
                (typeI == "DataTransformation" && !(!permissionDataTransformations || permissionDataTransformations.indexOf("All") >= 0 || permissionDataTransformations.indexOf(permType) >= 0)) ||
                (typeI == "BusinessObject" && !(!permissionBusinessObjects || permissionBusinessObjects.indexOf("All") >= 0 || permissionBusinessObjects.indexOf(permType) >= 0)) ||
                (typeI == "Variable" && !(!permissionVariables || permissionVariables.indexOf("All") >= 0 || permissionVariables.indexOf(permType) >= 0)) ||
                (typeI == "Relation" && !(!permissionDataRelations || permissionDataRelations.indexOf("All") >= 0 || permissionDataRelations.indexOf(permType) >= 0)) ||
                (typeI == "Column" && !(!permissionDataColumns || permissionDataColumns.indexOf("All") >= 0 || permissionDataColumns.indexOf(permType) >= 0)) ||
                (typeI == "Resource" && !(!permissionResources || permissionResources.indexOf("All") >= 0 || permissionResources.indexOf(permType) >= 0)) ||
                (typeI == "Parameter" && !(!permissionSqlParameters || permissionSqlParameters.indexOf("All") >= 0 || permissionSqlParameters.indexOf(permType) >= 0))
        }

        var cannotEdit = getPermissionResult(selectedItemObject, "Modify");
        if (selectedItemObject.restrictions && selectedItemObject.restrictions.isAllowEdit === false) cannotEdit = true;

        var cannotDelete = getPermissionResult(selectedItemObject, "Delete");
        if (selectedItemObject.restrictions && selectedItemObject.restrictions.isAllowDelete === false) cannotDelete = true;

        //Enabled or Disabled Buttons
        var enableEdit = !(typeItem.indexOf("MainItem") != -1 ||
            selectedItemObject.isCloudAttachedItem ||
            (typeItem == "DataBase" && (!selectedItemObject.typeConnection || selectedItemObject.typeConnection == "StiUndefinedDatabase")) ||
            (typeItem == "DataSource" && (!selectedItemObject.typeDataSource || selectedItemObject.typeDataSource == "StiUndefinedDataSource"))) &&
            typeItem != "SystemVariable" &&
            typeItem != "Function" &&
            typeItem != "FunctionsCategory" &&
            typeItem != "Parameters" &&
            typeItem != "Format" &&
            typeItem != "HtmlTag";

        var enableDelete = enableEdit || (typeItem == "DataBase" && (!selectedItemObject.typeConnection || selectedItemObject.typeConnection == "StiUndefinedDatabase"));

        this.controls["EditItem"].setEnabled(enableEdit && !cannotEdit);
        this.controls["DeleteItem"].setEnabled(enableDelete && !cannotDelete);

        var showColumnItem = (typeItem == "DataSource" || typeItem == "BusinessObject" || typeItem == "Column") && !selectedItemObject.isCloud &&
            (!permissionDataColumns || permissionDataColumns.indexOf("All") >= 0 || permissionDataColumns.indexOf("Create") >= 0);

        var showCalcColumnItem = showColumnItem && jsObject.options.dictionaryTree.getCurrentColumnParent().typeDataSource != "StiDataTransformation";

        var showParameterItem = ((typeItem == "DataSource" && selectedItemObject.parameterTypes) || typeItem == "Parameter" || typeItem == "Parameters" ||
            (typeItem == "Column" || jsObject.options.dictionaryTree.getCurrentColumnParent().parameterTypes) &&
            !selectedItemObject.isCloud && (!permissionDataColumns || permissionDataColumns.indexOf("All") >= 0 || permissionDataColumns.indexOf("Create") >= 0)) &&
            (!permissionSqlParameters || permissionSqlParameters.indexOf("All") >= 0 || permissionSqlParameters.indexOf("Create") >= 0);

        if (currentMenu) {
            if (currentMenu.items["editItem"]) currentMenu.items["editItem"].setEnabled(enableEdit && !cannotEdit);
            if (currentMenu.items["deleteItem"]) currentMenu.items["deleteItem"].setEnabled(enableDelete && !cannotDelete);

            currentMenu.items["columnNew"].style.display = showColumnItem ? "" : "none";
            currentMenu.items["calcColumnNew"].style.display = showCalcColumnItem ? "" : "none";
            currentMenu.items["parameterNew"].style.display = showParameterItem && !jsObject.options.jsMode ? "" : "none";

            var showDuplicateItem = enableEdit && (typeItem == "DataBase" || typeItem == "DataSource" || typeItem == "Variable" ||
                typeItem == "Relation" || typeItem == "Resource" || (typeItem == "Category" && selectedItem.parent.itemObject.typeItem == "VariablesMainItem"))

            if (currentMenu.items["duplicateItem"]) currentMenu.items["duplicateItem"].style.display = showDuplicateItem ? "" : "none";

            var showPropertiesItem = enableEdit && jsObject.options.showDictionaryContextMenuProperties &&
                (typeItem == "DataBase" || typeItem == "DataSource" || typeItem == "Relation" || typeItem == "Parameter" || typeItem == "Column" || typeItem == "Variable" || typeItem == "BusinessObject");
            if (currentMenu.items["properties"]) currentMenu.items["properties"].style.display = showPropertiesItem ? "" : "none";

            var permCreateDataSources = !permissionDataSources || permissionDataSources.indexOf("All") >= 0 || permissionDataSources.indexOf("Create") >= 0;
            var permCreateDataTransformations = !permissionDataTransformations || permissionDataTransformations.indexOf("All") >= 0 || permissionDataTransformations.indexOf("Create") >= 0;

            currentMenu.items["dataSourceNew"].style.display = permCreateDataSources ? "" : "none";
            currentMenu.items["separator1"].style.display = currentMenu.items["dataSourceNew"].style.display;

            if (currentMenu.items["dataSourceNewFromResource"]) {
                currentMenu.items["dataSourceNewFromResource"].style.display = permCreateDataSources && typeItem == "Resource" &&
                    ["Excel", "Csv", "Dbf", "Json", "Xml"].indexOf(selectedItemObject.type) >= 0 ? "" : "none";

                currentMenu.items["dataSourceNewFromResource"].caption.innerText = jsObject.loc.MainMenu.menuEditDataSourceNew.replace("...", "") + " [" + selectedItemObject.name + "]";
                StiMobileDesigner.setImageSource(currentMenu.items["dataSourceNewFromResource"].image, jsObject.options, selectedItemObject.typeIcon ? selectedItemObject.typeIcon + ".png" : "DataSourceNew.png");
            }

            if (currentMenu.items["dataTransformationNew"]) {
                currentMenu.items["dataTransformationNew"].style.display = permCreateDataTransformations ? "" : "none";
            }

            if (!jsObject.options.isJava && currentMenu.items["businessObjectNew"]) {
                currentMenu.items["businessObjectNew"].style.display = jsObject.options.designerSpecification == "Developer" && (!permissionBusinessObjects || permissionBusinessObjects.indexOf("All") >= 0 || permissionBusinessObjects.indexOf("Create") >= 0) ? "" : "none";
            }
            currentMenu.items["relationNew"].style.display = (!permissionDataRelations || permissionDataRelations.indexOf("All") >= 0 || permissionDataRelations.indexOf("Create") >= 0) ? "" : "none";
            currentMenu.items["variableNew"].style.display = (!permissionVariables || permissionVariables.indexOf("All") >= 0 || permissionVariables.indexOf("Create") >= 0) ? "" : "none";
            currentMenu.items["resourceNew"].style.display = (!permissionResources || permissionResources.indexOf("All") >= 0 || permissionResources.indexOf("Create") >= 0) ? "" : "none";
            currentMenu.items["categoryNew"].style.display = currentMenu.items["variableNew"].style.display;
            currentMenu.items["separator2"].style.display = currentMenu.items["variableNew"].style.display;

            currentMenu.items["menuMakeThisRelationActive"].style.display = typeItem == "Relation" && !selectedItemObject.active && (!permissionDataRelations || permissionDataRelations.indexOf("All") >= 0 || permissionDataRelations.indexOf("Edit") >= 0) ? "" : "none";
            currentMenu.items["separator2_1"].style.display = currentMenu.items["menuMakeThisRelationActive"].style.display;
            if (currentMenu.items["separator3_0"] && currentMenu.items["viewData"]) {
                currentMenu.items["separator3_0"].style.display = typeItem == "DataSource" || typeItem == "BusinessObject" ? "" : "none";
                currentMenu.items["viewData"].style.display = typeItem == "DataSource" || typeItem == "BusinessObject" ? "" : "none";
            }
        }
        if (currentMenu && currentMenu == jsObject.options.menus.dictionaryNewItemMenu) {
            var hideParentButton = true;
            for (var itemName in currentMenu.items) {
                if (currentMenu.items[itemName].style.display == "") hideParentButton = false;
            }
            this.controls["NewItem"].style.display = hideParentButton ? "none" : "";
        }

        var showCreateDataHint = true;
        if (jsObject.options.dictionaryShowMoreActivated)
            showCreateDataHint = false;

        var report = jsObject.options.report;
        if (report && report.dictionary.attachedItems && jsObject.GetCountObjects(report.dictionary.attachedItems) > 0)
            showCreateDataHint = false;

        if (jsObject.options.dictionaryTree && showCreateDataHint) {
            var mainItems = jsObject.options.dictionaryTree.mainItems;
            var mainItemsNames = ["DataSources", "BusinessObjects", "Variables", "Resources", "SystemVariables", "Functions", "Images", "RichTexts", "SubReports"];
            for (var i = 0; i < mainItemsNames.length; i++) {
                if (mainItems[mainItemsNames[i]]) {
                    if (mainItems[mainItemsNames[i]].isOpening ||
                        ((mainItemsNames[i] == "DataSources" ||
                            mainItemsNames[i] == "BusinessObjects" ||
                            mainItemsNames[i] == "Variables" ||
                            mainItemsNames[i] == "Resources") &&
                            mainItems[mainItemsNames[i]].childsContainer.childNodes.length > 0)) {
                        showCreateDataHint = false;
                        break;
                    }
                }
            }
        }

        createDataHintItem.style.display = showCreateDataHint ? "" : "none";
        showMoreButton.style.display = showCreateDataHint ? "inline-block" : "none";
        dictionaryItemsContainer.style.display = !showCreateDataHint ? "" : "none";
    }

    var buttons = [
        ["NewItem", this.FormButton(null, "dictionaryNewItem", this.loc.MainMenu.menuFileNew.replace("&", ""), null, null, null, null, null, "Down")],
        ["Actions", this.FormButton(null, "dictionaryActions", this.loc.FormDictionaryDesigner.Actions, null, null, null, null, "stiDesignerSmallButtonWithBorder", "Down")],
        ["EditItem", this.SmallButton("dictionaryEditItem", null, null, "Edit.png", this.loc.QueryBuilder.Edit, null)],
        ["DeleteItem", this.SmallButton("dictionaryDeleteItem", null, null, "Remove.png", this.loc.MainMenu.menuEditDelete.replace("&", ""))],
        ["MoveUp", this.SmallButton("dictionaryMoveUpItem", null, null, "Arrows.ArrowUpBlue.png", this.loc.QueryBuilder.MoveUp)],
        ["MoveDown", this.SmallButton("dictionaryMoveDownItem", null, null, "Arrows.ArrowDownBlue.png", this.loc.QueryBuilder.MoveDown)],
        ["Settings", this.SmallButton("dictionarySettings", null, null, "Settings.png", this.loc.Export.Settings, "Down")]
    ]

    for (var i = 0; i < buttons.length; i++) {
        var toolButton = buttons[i][1];
        toolBar.controls[buttons[i][0]] = toolButton;
        toolButton.style.margin = this.options.isTouchDevice ? "3px 0px 3px 3px" : "5px 0px 5px 5px";
        toolBar.addCell(toolButton);
    }

    toolBar.controls.NewItem.allwaysEnabled = false;
    toolBar.controls.Actions.allwaysEnabled = false;

    toolBar.controls.NewItem.action = function () {
        var dictionaryNewItemMenu = jsObject.options.menus.dictionaryNewItemMenu || jsObject.InitializeDictionaryNewItemMenu();
        if (!dictionaryNewItemMenu.visible) toolBar.updateControls(dictionaryNewItemMenu);
        dictionaryNewItemMenu.changeVisibleState(!dictionaryNewItemMenu.visible);
    }

    toolBar.controls.Actions.action = function () {
        var dictionaryActionsMenu = jsObject.options.menus.dictionaryActionsMenu || jsObject.InitializeDictionaryActionsMenu();
        dictionaryActionsMenu.changeVisibleState(!dictionaryActionsMenu.visible);
    }

    toolBar.controls.EditItem.action = function () {
        jsObject.EditItemDictionaryTree();
    }

    toolBar.controls.DeleteItem.action = function () {
        jsObject.DeleteItemDictionaryTree();
    }

    toolBar.controls.MoveUp.action = toolBar.controls.MoveDown.action = function () {
        var selectedItem = jsObject.options.dictionaryTree.selectedItem;
        if (selectedItem) {
            var direction = this.name == "dictionaryMoveUpItem" ? "Up" : "Down";
            var fromObject = selectedItem.itemObject;
            var toItem = direction == "Down"
                ? (selectedItem.nextSibling || (fromObject.typeItem == "Variable" ? (selectedItem.parent.nextSibling || selectedItem.parent.parent) : null))
                : (selectedItem.previousSibling || (fromObject.typeItem == "Variable" ? (selectedItem.parent.previousSibling || selectedItem.parent.parent) : null));
            var toObject = toItem ? toItem.itemObject : null;
            this.setEnabled(false);
            jsObject.SendCommandMoveDictionaryItem(fromObject, toObject, direction);
        }
    }

    var settingsMenu = this.InitializeDictionarySettingsMenu(toolBar.controls.Settings);

    toolBar.controls.Settings.action = function () {
        settingsMenu.changeVisibleState(!settingsMenu.visible);
    }

    dictionaryPanel.onmouseup = function (event) {
        if (event.button == 2) {
            event.stopPropagation();
            if (!jsObject.options.report) return;
            var dictionaryContextMenu = jsObject.options.menus.dictionaryContextMenu || jsObject.InitializeDictionaryContextMenu();
            toolBar.updateControls(dictionaryContextMenu);
            var point = jsObject.FindMousePosOnMainPanel(event);
            dictionaryContextMenu.show(point.xPixels + 3, point.yPixels + 3, "Down", "Right");
        }
        return false;
    }

    dictionaryPanel.oncontextmenu = function (event) {
        return false;
    }

    dictionaryPanel.checkResourcesCount = function () {
        if (jsObject.options.reportResourcesMaximumCount && jsObject.options.report) {
            var resourcesCount = jsObject.options.report.dictionary.resources.length;
            if (resourcesCount >= jsObject.options.reportResourcesMaximumCount) {
                jsObject.InitializeNotificationForm(function (form) {
                    form.show(
                        jsObject.loc.Notices.QuotaMaximumResourcesCountExceeded + "<br>" + jsObject.loc.PropertyMain.Maximum + ": " + jsObject.options.reportResourcesMaximumCount,
                        jsObject.NotificationMessages("upgradeYourPlan"),
                        "Notifications.Files.png"
                    );
                });
                return true;
            }
        }
        return false;
    }

    dictionaryPanel.onmousedown = function () {
        dictionaryPanel.setFocused(true);
        jsObject.options.dictionaryPanelPressed = true;
    }

    dictionaryPanel.checkDataHintItemVisibility = function () {
        setTimeout(function () {
            if (dictionaryPanel.offsetHeight > 0 && dictionaryPanel.offsetHeight < 230) {
                if (createDataHintItem.parentElement == dictionaryPanel)
                    dictionaryPanel.removeChild(createDataHintItem);
            }
            else {
                if (!createDataHintItem.parentElement)
                    dictionaryPanel.appendChild(createDataHintItem);
            }
        }, 0);
    }

    this.addEvent(window, "resize", function (event) {
        dictionaryPanel.checkDataHintItemVisibility();
    });

    dictionaryPanel.setFocused = function (state) {
        this.isFocused = state;
        var selectedItem = jsObject.options.dictionaryTree.selectedItem;

        if (selectedItem) {
            if (state)
                selectedItem.button.className = selectedItem.button.className.replace(" stiDesignerTreeItemButtonSelectedNotActive", "");
            else if (selectedItem.button.className.indexOf("stiDesignerTreeItemButtonSelectedNotActive") < 0)
                selectedItem.button.className += " stiDesignerTreeItemButtonSelectedNotActive";
        }
    }

    return dictionaryPanel;
}

StiMobileDesigner.prototype.CreateDataHintItem = function () {
    var createDataHintItem = this.CreateHTMLTable();
    var widthHintItem = this.options.propertiesGridWidth - 120;
    createDataHintItem.className = "stiCreateDataHintItem";
    createDataHintItem.style.top = "calc(50% - 60px)";
    createDataHintItem.style.left = "calc(50% - " + widthHintItem / 2 + "px)";

    var img = document.createElement("img");
    img.style.width = img.style.height = "60px";
    img.setAttribute("draggable", "false");
    StiMobileDesigner.setImageSource(img, this.options, "ItemCreateData.png");

    createDataHintItem.addCell(img).style.textAlign = "center";
    createDataHintItem.addTextCellInNextRow(this.loc.FormDictionaryDesigner.ClickHere).className = "stiCreateDataHintHeaderText";

    createDataHintItem.onmouseup = function (event) {
        this.jsObject.options.dictionaryPanel.onmouseup(event);
    }

    //text create
    var text = document.createElement("div");
    text.className = "stiCreateDataHintText";
    text.innerHTML = this.loc.FormDictionaryDesigner.CreateNewDataSource;
    text.style.width = widthHintItem + "px";
    createDataHintItem.addCellInNextRow(text).style.textAlign = "center";

    //separator
    var separator = this.SeparatorOr(widthHintItem - 50);
    separator.style.display = "inline-block";
    createDataHintItem.addCellInNextRow(separator).style.textAlign = "center";

    //text drop
    var text2 = document.createElement("div");
    text2.className = "stiCreateDataHintText";
    text2.innerHTML = this.loc.FormDictionaryDesigner.DragNewDataSource;
    text2.style.width = widthHintItem + "px";
    createDataHintItem.addCellInNextRow(text2).style.textAlign = "center";

    return createDataHintItem;
}