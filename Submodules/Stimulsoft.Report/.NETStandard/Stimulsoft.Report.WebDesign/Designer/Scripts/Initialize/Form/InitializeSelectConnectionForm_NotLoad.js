
StiMobileDesigner.prototype.InitializeSelectConnectionForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("selectConnectionForm", this.loc.FormTitles.ConnectionSelectForm, 3, this.HelpLinks["connectionNew"]);
    form.connectionGroups = {};
    form.connectionButtons = {};

    form.skipSchemaWizard = this.CheckBox(null, this.loc.FormDictionaryDesigner.SkipSchemaWizard);
    form.skipSchemaWizard.style.margin = "12px";

    form.hideUnsupDatabases = this.CheckBox(null, this.loc.FormDictionaryDesigner.HideUnsupportedDatabases);
    form.hideUnsupDatabases.style.margin = "12px 12px 12px 30px";

    var footerTable = this.CreateHTMLTable();
    footerTable.style.width = "100%";

    var buttonsPanel = form.buttonsPanel;
    form.removeChild(buttonsPanel);
    form.appendChild(footerTable);
    footerTable.addCell(form.skipSchemaWizard).style.width = "1px";

    if (this.options.jsMode)
        footerTable.addCell(form.hideUnsupDatabases).style.width = "1px";

    footerTable.addCell();
    var cancelCell = footerTable.addCell(form.buttonCancel)
    cancelCell.style.width = "1px";
    cancelCell.style.lineHeight = "0";

    var mainTable = this.CreateHTMLTable();
    form.container.appendChild(mainTable);

    var findControl = this.FindControl(null, 154);
    findControl.style.margin = "12px";

    var leftCell = mainTable.addCell(findControl);
    leftCell.className = "stiDesignerRightBorderContainer";

    var groupsContainer = this.EasyContainer(160, 450);
    groupsContainer.style.margin = "0 12px 12px 12px";
    leftCell.appendChild(groupsContainer);

    var connectsContainer = this.EasyContainer(750, 510);
    connectsContainer.groups = {};
    connectsContainer.style.margin = "0 12px 0 12px";
    connectsContainer.style.overflowX = "hidden";
    connectsContainer.style.overflowY = "auto";
    mainTable.addCell(connectsContainer);

    var groupsProps = [
        { name: "All", localizedName: this.loc.Report.RangeAll },
        { name: "ReportConnections", localizedName: this.loc.FormDatabaseEdit.ReportConnections },
        { name: "Favorites", localizedName: this.loc.FormDatabaseEdit.Favorites },
        { name: "Recent", localizedName: this.loc.FormDatabaseEdit.RecentConnections },
        { name: "Files", localizedName: this.loc.PropertyMain.File },
        { name: "SQL" },
        { name: "NoSQL" },
        { name: "Azure" },
        { name: "Google" },
        { name: "OnlineServices", localizedName: "Online Services" },
        { name: "REST" },
        { name: "Objects", localizedName: this.loc.QueryBuilder.Objects }
    ];

    for (var i = 0; i < groupsProps.length; i++) {
        var groupName = groupsProps[i].name;
        var groupLabel = groupsProps[i].localizedName || groupName;
        groupsContainer.addItem(groupName, {}, groupLabel, null, true);

        if (groupName != "All") {
            var group = this.ConnectionsGroupPanel(groupLabel);
            connectsContainer.groups[groupName] = group;
            form.connectionGroups[groupName] = group;
            connectsContainer.appendChild(group);
        }
    }

    groupsContainer.onSelected = function (item) {
        form.clearAllConnections();
        form.fillConnections();

        if (findControl.textBox.value) {
            findControl.textBox.value = "";
            findControl.textBox.onchange();
        }
    }

    form.hideAllGroupItems = function () {
        for (var i = 0; i < groupsContainer.childNodes.length; i++) {
            groupsContainer.childNodes[i].style.display = "none";
        }
    }

    form.clearAllConnections = function () {
        for (var groupName in form.connectionGroups) {
            var group = form.connectionGroups[groupName];
            group.clear();
            group.hide();
        }
    }

    form.resetAllContainers = function () {
        form.hideAllGroupItems();
        form.clearAllConnections();
        form.connectionButtons = {};
        var itemAll = groupsContainer.getItemByName("All");
        itemAll.style.display = "";
        itemAll.select();
    }

    findControl.textBox.onchange = function () {
        var text = this.value.toLowerCase();
        for (var groupName in form.connectionGroups) {
            var group = form.connectionGroups[groupName];
            if (group.visible) {
                var showGroup = false;
                for (var i = 0; i < group.container.childNodes.length; i++) {
                    var item = group.container.childNodes[i];
                    if (item.visible) {
                        var isFound = item.caption.innerHTML.toLowerCase().indexOf(text) >= 0;
                        item.style.display = !text || isFound ? "inline-block" : "none";
                        if (!text || isFound) showGroup = true;
                    }
                }
                group.style.display = showGroup ? "" : "none";
            }
        }
    }

    form.hideUnsupDatabases.action = function () {
        var currGroupName = groupsContainer.selectedItem ? groupsContainer.selectedItem.name : null;
        form.resetAllContainers();
        form.clearAllConnections();
        form.fillConnections();
        if (currGroupName) {
            var item = groupsContainer.getItemByName(currGroupName);
            if (item && item.style.display == "") item.action();
        }
        jsObject.SetCookie("StimulsoftMobileDesignerHideUnsupportedDatabases", this.isChecked.toString());
    }

    form.skipSchemaWizard.action = function () {
        jsObject.SetCookie("StimulsoftMobileDesignerSkipSchemaWizard", this.isChecked.toString());
    }

    form.onshow = function () {
        form.resetAllContainers();
        form.skipSchemaWizard.setChecked(jsObject.GetCookie("StimulsoftMobileDesignerSkipSchemaWizard") == "true");

        findControl.textBox.value = "";
        findControl.textBox.focus();

        var hideUnsupportedCookie = jsObject.GetCookie("StimulsoftMobileDesignerHideUnsupportedDatabases")
        form.hideUnsupDatabases.setChecked(hideUnsupportedCookie == null || hideUnsupportedCookie == "true");

        jsObject.SendCommandToDesignerServer("GetConnectionTypes", {}, function (answer) {
            if (answer.connections) {
                form.connections = answer.connections;
                form.fillConnections();

                if (jsObject.options.cloudMode && jsObject.GetCurrentPlanLimitValue("AllowDatabases") === false && !form.connections["ReportConnections"]) {
                    var filesItem = groupsContainer.getItemByName("Files");
                    if (filesItem) filesItem.action();
                }
            }
        });
    }

    form.onhide = function () {
        findControl.textBox.onblur();
    }

    form.fillConnections = function () {
        var connections = form.connections;
        var currentGroupName = groupsContainer.selectedItem ? groupsContainer.selectedItem.name : null;
        var favoriteConnections = jsObject.GetFavoriteConnectionsFromCookies();
        var allowCreateConnection = !jsObject.options.permissionDataConnections || jsObject.options.permissionDataConnections.indexOf("All") >= 0 || jsObject.options.permissionDataConnections.indexOf("Create") >= 0;

        for (var i = 0; i < groupsProps.length; i++) {
            var groupName = groupsProps[i].name;
            var group = form.connectionGroups[groupName];
            var connectionItems = connections[groupName];

            //for JS mode
            var hasSupported = !jsObject.options.jsMode;
            if (jsObject.options.jsMode && connectionItems) {
                for (var n = 0; n < connectionItems.length; n++) {
                    if (connectionItems[n].support) {
                        hasSupported = true;
                        break;
                    }
                }
            }

            if (group && connectionItems && connectionItems.length > 0 && (hasSupported || !form.hideUnsupDatabases.isChecked)) {
                var allowShowGroup = allowCreateConnection || groupName == "ReportConnections" || groupName == "Objects";
                var hasFavoriteIcon = groupName != "Objects" && groupName != "Recent" && groupName != "ReportConnections";

                if (allowShowGroup) {
                    var groupItem = groupsContainer.getItemByName(groupName);
                    if (groupItem) groupItem.style.display = "";

                    var showGroup = currentGroupName == "All" || currentGroupName == groupName;
                    if (showGroup) group.show();

                    for (var k = 0; k < connectionItems.length; k++) {
                        var typeConnection = connectionItems[k].typeConnection;
                        var button = jsObject.ConnectionButton(connectionItems[k], groupName, this, hasFavoriteIcon);
                        this.connectionButtons[typeConnection] = button;

                        if (!jsObject.options.jsMode || connectionItems[k].support || !form.hideUnsupDatabases.isChecked) {
                            group.container.appendChild(button);
                        }

                        if (hasFavoriteIcon && favoriteConnections[typeConnection]) {
                            button.setFavorite(true, false, true);
                        }

                        if (jsObject.options.designerSpecification != "Developer" && (
                            typeConnection == "StiBusinessObjectAdapterService" ||
                            typeConnection == "StiDataTableAdapterService" ||
                            typeConnection == "StiUserAdapterService" ||
                            typeConnection == "StiDataViewAdapterService")) {
                            button.hide();
                        }

                        if (jsObject.options.designerSpecification == "Beginner" && (
                            typeConnection == "StiVirtualAdapterService" ||
                            typeConnection == "StiCrossTabAdapterService")) {
                            button.hide();
                        }
                    }
                }
            }
        }

        var recentConnections = jsObject.GetRecentConnectionsFromCookies();
        if (recentConnections.length > 0) {
            groupsContainer.getItemByName("Recent").style.display = "";

            if (currentGroupName == "All" || currentGroupName == "Recent") {
                form.connectionGroups.Recent.show();

                for (var i = 0; i < recentConnections.length; i++) {
                    var button = jsObject.ConnectionButton(recentConnections[i], "Recent", this, false);
                    form.connectionGroups.Recent.container.appendChild(button);
                }
            }
        }

        if (jsObject.options.designerSpecification == "Beginner" && groupsContainer.getItemByName("Objects")) {
            groupsContainer.getItemByName("Objects").style.display = "none";
            if (connectsContainer.groups.Objects) connectsContainer.groups.Objects.style.display = "none";
        }
    }

    form.addToFavoritesGroup = function (button) {
        var currentGroupName = groupsContainer.selectedItem ? groupsContainer.selectedItem.name : null;
        var favoritesGroup = this.connectionGroups.Favorites;

        if (currentGroupName == "All" || currentGroupName == "Favorites")
            favoritesGroup.show();
        else
            favoritesGroup.hide();

        groupsContainer.getItemByName("Favorites").style.display = "";

        var favoriteButton = jsObject.ConnectionButton(button.connectionObject, button.groupName, this, true);
        favoriteButton.setFavorite(true);
        favoriteButton.inFavoriteGroup = true;
        favoritesGroup.container.appendChild(favoriteButton);

        if (currentGroupName == "All") button.hide();
    }

    form.deleteFromFavoritesGroup = function (button) {
        var favoritesGroup = this.connectionGroups.Favorites;
        if (!button.inFavoriteGroup) {
            for (var i = 0; i < favoritesGroup.container.childNodes.length; i++) {
                var connectionObject = favoritesGroup.container.childNodes[i].connectionObject;
                if (connectionObject && connectionObject.typeConnection == button.connectionObject.typeConnection)
                    favoritesGroup.container.removeChild(favoritesGroup.container.childNodes[i]);
            }
        }
        else {
            var connectionButton = this.connectionButtons[button.connectionObject.typeConnection];
            if (connectionButton) {
                connectionButton.setFavorite(false);
                connectionButton.show();
            }
            favoritesGroup.container.removeChild(button);
        }

        if (favoritesGroup.container.childNodes.length == 0) {
            favoritesGroup.hide();
            var favoritesItem = groupsContainer.getItemByName("Favorites");
            favoritesItem.style.display = "none";
            if (favoritesItem.isSelected) groupsContainer.getItemByName("All").action();
        }
    }

    return form;
}

StiMobileDesigner.prototype.ConnectionButton = function (connectionObject, groupName, selectConnectionForm, haveFavorit) {
    var jsObject = this;
    var imageName = "Connections." + connectionObject.typeConnection + ".png";
    if (!StiMobileDesigner.checkImageSource(this.options, imageName)) imageName = "Connections.BigDataSource.png";
    var caption = groupName == "Objects" ? this.GetLocalizedAdapterName(connectionObject.typeConnection) : connectionObject.name;

    var button = this.SmallButton(null, null, caption, imageName, caption, null, "stiDesignerSmallButtonWithBorder", true, { width: 32, height: 32 });
    button.groupName = groupName;
    button.visible = true;
    button.connectionObject = connectionObject;
    button.isFavorite = false;
    button.style.height = "45px";
    button.style.width = "170px";
    button.style.margin = "0 8px 8px 0";
    button.style.display = "inline-block";
    button.style.overflow = "hidden";
    button.style.position = "relative";

    if (button.caption) {
        button.caption.style.paddingRight = "15px";
        button.caption.style.whiteSpace = "normal";
        button.caption.style.lineHeight = "1";
    }

    if (button.imageCell) {
        button.imageCell.style.padding = "0 6px";
    }

    if (haveFavorit) {
        var favoriteImg = document.createElement("img");
        favoriteImg.jsObject = this;
        StiMobileDesigner.setImageSource(favoriteImg, this.options, "Connections.Favorites.png");
        favoriteImg.style.visibility = "hidden";
        favoriteImg.style.width = favoriteImg.style.height = "16px";
        var favoriteImgCell = button.innerTable.addCell(favoriteImg);
        favoriteImgCell.style.verticalAlign = "top";
        favoriteImgCell.style.width = "1px";
        button.favoriteImg = favoriteImg;
        button.innerTable.style.width = "100%";
        if (button.imageCell) button.imageCell.style.width = "1px";

        favoriteImg.onclick = function (event) {
            if (this.isTouchProcessFlag) return;
            button.setFavorite(!button.isFavorite, true, true);
            button.favoriteClicked = true;
        }

        favoriteImg.ontouchend = function (event) {
            var this_ = this;
            this.isTouchProcessFlag = true;

            button.setFavorite(!button.isFavorite, true, true);
            button.favoriteClicked = true;

            setTimeout(function () {
                this_.isTouchProcessFlag = false;
            }, 1000);
        }

        button.onmouseover = function () {
            if (jsObject.options.isTouchDevice) return;
            this.className = this.overClass;
            this.isOver = true;
            favoriteImg.style.visibility = "visible";
        }

        button.onmouseout = function () {
            if (jsObject.options.isTouchDevice) return;
            this.isOver = false;
            this.className = this.isSelected ? this.selectedClass : this.defaultClass;
            if (!this.isFavorite) favoriteImg.style.visibility = "hidden";
        }

        button.setFavorite = function (state, updateCookies, updateFavoritesGroup) {
            this.isFavorite = state;
            StiMobileDesigner.setImageSource(favoriteImg, jsObject.options, this.isFavorite ? "Connections.FavoritesYellow.png" : "Connections.Favorites.png");
            favoriteImg.style.visibility = state ? "visible" : "hidden";

            if (updateCookies) {
                if (state)
                    jsObject.SaveFavoriteConnectionToCookies(this.connectionObject.typeConnection);
                else
                    jsObject.DeleteFavoriteConnectionFromCookies(this.connectionObject.typeConnection);
            }

            if (updateFavoritesGroup) {
                if (state)
                    selectConnectionForm.addToFavoritesGroup(this);
                else
                    selectConnectionForm.deleteFromFavoritesGroup(this);
            }
        }
    }

    button.show = function () {
        this.visible = true;
        this.style.display = "inline-block";
    }

    button.hide = function () {
        this.visible = false;
        this.style.display = "none";
    }

    button.isFileDataConnection = function () {
        var connectionObject = this.connectionObject;
        var typeConnection = connectionObject ? connectionObject.typeConnection : null;

        return (typeConnection && (
            jsObject.EndsWith(typeConnection, "StiXmlDatabase") ||
            jsObject.EndsWith(typeConnection, "StiJsonDatabase") ||
            jsObject.EndsWith(typeConnection, "StiDBaseDatabase") ||
            jsObject.EndsWith(typeConnection, "StiCsvDatabase") ||
            jsObject.EndsWith(typeConnection, "StiExcelDatabase"))
        )
    }

    button.action = function () {
        if (this.favoriteClicked) {
            this.favoriteClicked = false;
            return;
        }

        selectConnectionForm.changeVisibleState(false);

        var skipSchema = selectConnectionForm.skipSchemaWizard.isChecked;
        var connectionObject = this.connectionObject;
        var connectionName = connectionObject ? connectionObject.name : null;
        var typeConnection = connectionObject ? connectionObject.typeConnection : null;

        if (this.isFileDataConnection()) skipSchema = false;

        if (this.groupName == "ReportConnections") {
            if (skipSchema) {
                jsObject.InitializeEditDataSourceForm(function (editDataSourceForm) {
                    editDataSourceForm.datasource = jsObject.GetDataAdapterTypeFromDatabaseType(typeConnection);
                    editDataSourceForm.nameInSource = connectionName;
                    editDataSourceForm.changeVisibleState(true);
                });
            }
            else {
                jsObject.InitializeSelectDataForm(function (selectDataForm) {
                    selectDataForm.databaseName = connectionName;
                    selectDataForm.connectionObject = connectionObject;
                    selectDataForm.typeConnection = typeConnection;
                    selectDataForm.changeVisibleState(true);
                });
            }
        }
        else if (this.groupName == "Objects") {
            if (typeConnection == "StiVirtualAdapterService") {
                jsObject.InitializeEditDataSourceFromOtherDatasourcesForm(function (form) {
                    form.datasource = typeConnection;
                    form.changeVisibleState(true);
                });
            }
            else if (typeConnection == "StiCrossTabAdapterService") {
                jsObject.InitializeEditDataSourceFromCrossTabForm(function (form) {
                    form.datasource = typeConnection;
                    form.changeVisibleState(true);
                });
            }
            else {
                jsObject.InitializeEditDataSourceForm(function (form) {
                    form.datasource = typeConnection;
                    form.changeVisibleState(true);
                });
            }
        }
        else {
            if (jsObject.options.cloudMode && !this.isFileDataConnection() && jsObject.GetCurrentPlanLimitValue("AllowDatabases") === false) {
                jsObject.InitializeNotificationForm(function (form) {
                    form.show(jsObject.NotificationMessages("availableDataSources"), jsObject.NotificationMessages("availableDataSourcesInDesktopVersion"), "Notifications.Blocked.png");
                });
                return;
            }
            if (this.groupName == "Recent") {
                jsObject.InitializeEditConnectionForm(function (editConnectionForm) {
                    editConnectionForm.skipSchemaWizard = skipSchema;
                    editConnectionForm.connection = connectionObject;
                    editConnectionForm.connection.isRecentConnection = true;
                    editConnectionForm.changeVisibleState(true);
                });
            }
            else {
                jsObject.InitializeEditConnectionForm(function (editConnectionForm) {
                    editConnectionForm.skipSchemaWizard = skipSchema;
                    editConnectionForm.connection = typeConnection;
                    editConnectionForm.serviceName = typeConnection == "StiCustomDatabase" ? connectionName : null;
                    editConnectionForm.changeVisibleState(true);
                });
            }
        }
    }

    if (jsObject.options.cloudMode && !button.isFileDataConnection() && jsObject.GetCurrentPlanLimitValue("AllowDatabases") === false) {
        var upgrBlock = jsObject.CreateHTMLTable();
        upgrBlock.setAttribute("style", "position: absolute; bottom: 2px; right: 2px; background: #2f7b51;");
        upgrBlock.addTextCell(jsObject.loc.Buttons.Upgrade).setAttribute("style", "height: 15px; padding: 0 5px 0 5px; font-size: 10px; font-family: 'Arial'; color: #ffffff;");
        button.appendChild(upgrBlock);
    }

    return button;
}

StiMobileDesigner.prototype.GetFavoriteConnectionsFromCookies = function () {
    var connectionsStr = this.GetCookie("StimulsoftMobileDesignerFavoriteConnections");
    return (connectionsStr ? JSON.parse(connectionsStr) : {});
}

StiMobileDesigner.prototype.SaveFavoriteConnectionToCookies = function (connectionType) {
    var favoriteConnections = this.GetFavoriteConnectionsFromCookies();
    favoriteConnections[connectionType] = true;
    this.SetCookie("StimulsoftMobileDesignerFavoriteConnections", JSON.stringify(favoriteConnections));
}

StiMobileDesigner.prototype.DeleteFavoriteConnectionFromCookies = function (connectionType) {
    var favoriteConnections = this.GetFavoriteConnectionsFromCookies();
    delete favoriteConnections[connectionType];
    this.SetCookie("StimulsoftMobileDesignerFavoriteConnections", JSON.stringify(favoriteConnections));
}

StiMobileDesigner.prototype.ConnectionsGroupPanel = function (caption) {
    var groupPanel = this.GroupPanel(caption);
    groupPanel.container.style.lineHeight = "0";
    groupPanel.container.style.padding = "8px 0 0 0";
    groupPanel.visible = true;

    groupPanel.show = function () {
        this.visible = true;
        this.style.display = "";
    }

    groupPanel.hide = function () {
        this.visible = false;
        this.style.display = "none";
    }

    return groupPanel;
}