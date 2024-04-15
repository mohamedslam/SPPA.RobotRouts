
StiMobileDesigner.prototype.InitializeToolBar = function () {
    var toolBar = document.createElement("div");
    this.options.toolBar = toolBar;
    this.options.mainPanel.appendChild(toolBar);
    toolBar.className = "stiDesignerToolBar";
    toolBar.style.height = "38px";
    var jsObject = toolBar.jsObject = this;

    if (this.options.isTouchDevice) {
        toolBar.style.overflowX = "auto";
        toolBar.style.overflowY = "hidden";
    }
    else
        toolBar.style.overflow = "hidden";

    var toolBarTable = this.CreateHTMLTable();
    toolBarTable.style.width = "100%";
    toolBarTable.style.height = "100%";
    toolBar.appendChild(toolBarTable);

    toolBar.changeVisibleState = function (state) {
        this.style.display = state ? (jsObject.options.showToolbar !== false ? "" : "none") : "none";
        jsObject.options.paintPanel.style.top = (jsObject.options.workPanel.offsetHeight + jsObject.options.pagesPanel.offsetHeight + this.offsetHeight) + "px";
    }

    if (this.options.showSaveButton) {
        var saveReportHotButton = this.ToolButtonAdditional("saveReportHotButton", null, this.isDarkToolBar() ? "Toolbar.SaveWhite.png" : "Toolbar.SaveBlack.png");
        saveReportHotButton.style.marginLeft = "3px";
        var saveCell = toolBarTable.addCell(saveReportHotButton);

        if (this.isOffice2013Theme()) {
            saveCell.style.verticalAlign = "bottom";
        }

        saveReportHotButton.action = function () {
            if (jsObject.options.previewMode) {
                jsObject.options.workPanel.showPanel(this.jsObject.options.homePanel);
                jsObject.options.buttons.homeToolButton.setSelected(true);
            }
            jsObject.ActionSaveReport();
        }
    }
        
    var fileButton = this.FileButton();
    fileButton.style.marginLeft = "3px";
    toolBarTable.addCell(fileButton).className = "stiDesignerToolButtonCell";
    fileButton.style.display = this.options.showFileMenu ? "" : "none";

    var buttons = [
        ["homeToolButton", this.loc.Toolbars.TabHome, true],
        ["insertToolButton", this.loc.PropertyMain.Insert, !this.options.showInsertButton ? false : this.options.showInsertTab],
        ["pageToolButton", this.loc.Toolbars.TabPage, this.options.showPageButton],
        ["layoutToolButton", this.loc.Toolbars.TabLayout, this.options.showLayoutButton],
        ["previewToolButton", this.loc.Wizards.Preview, this.options.showPreviewButton],
    ]

    for (var i = 0; i < buttons.length; i++) {
        var toolButton = this.ToolButton(buttons[i][0], buttons[i][1]);
        toolButton.style.marginLeft = "3px";
        var toolButtonCell = toolBarTable.addCell(toolButton);
        toolButtonCell.className = "stiDesignerToolButtonCell";
        toolButtonCell.style.display = buttons[i][2] ? "" : "none";
    }

    if ((this.options.cloudMode || this.options.serverMode) && this.options.buttons.previewToolButton) {
        var previewToolButton = this.options.buttons.previewToolButton;
        var progress = previewToolButton.progress = this.ProgressMini();
        progress.style.visibility = "hidden";
        progress.style.top = "6px";
        progress.style.position = "absolute";
        previewToolButton.innerTable.addCell(progress).style.position = "relative";
    }

    toolBarTable.addCell().style.width = "100%";

    if (!this.options.fullScreenMode) {
        var resizeButton = this.ToolButtonAdditional("resizeDesigner", null, this.isDarkToolBar() ? "Toolbar.ResizeWindowWhite.png" : "Toolbar.ResizeWindow.png", this.loc.PropertyMain.Resize, null, 30, 30);
        toolBarTable.addCell(resizeButton);
        resizeButton.allwaysEnabled = true;
    }

    //Share
    var shareButton = this.ToolButtonAdditional("buttonShare", null, this.isDarkToolBar() ? "Toolbar.ShareWhite.png" : "Toolbar.Share.png", this.loc.Cloud.ButtonShare);
    shareButton.style.marginLeft = "3px";
    toolBarTable.addCell(shareButton);
    shareButton.style.display = this.options.cloudMode || this.options.standaloneJsMode ? "" : "none";

    shareButton.action = function () {
        if (jsObject.options.standaloneJsMode && (!jsObject.CheckUserTrExpired() || !jsObject.CheckUserActivated() || !jsObject.CheckUserProductsExpired()))
            return;

        if (jsObject.options.report && jsObject.options.report.properties.calculationMode == "Compilation") {
            var messageForm = jsObject.MessageFormForRenderReportInCompilationMode();
            messageForm.changeVisibleState(true);
            return;
        }

        jsObject.InitializeShareForm(function (shareForm) {
            shareForm.show();
        });
    }

    //Publish
    var publishButton = this.ToolButtonAdditional("buttonPublish", this.loc.Cloud.ButtonPublish);
    publishButton.style.marginLeft = "3px";
    toolBarTable.addCell(publishButton);
    publishButton.style.display = this.options.cloudMode ? "" : "none";

    publishButton.action = function () {
        if (jsObject.options.cloudMode) {
            if (!jsObject.options.cloudParameters || !jsObject.options.cloudParameters.sessionKey) {
                var messageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                messageForm.show(jsObject.loc.Notices.PleaseLogin, "Warning");
                return;
            }
        }

        var win = jsObject.openNewWindow();

        if (jsObject.options.report) {
            jsObject.SendCommandToDesignerServer("GetReportString", {}, function (answer) {
                jsObject.InitializePublishForm(function (publishForm) {
                    publishForm.show(answer.reportString, win);
                });
            });
        }
        else {
            jsObject.InitializePublishForm(function (publishForm) {
                publishForm.show(null, win);
            });
        }
    }

    //Localization
    if ((this.options.jsMode && this.options.showLocalization) ||
        (this.options.locFiles && this.options.locFiles.length > 0)) {
        var locControl = this.localizationControl = document.createElement("div");
        locControl.jsObject = this;
        locControl.style.marginLeft = "3px";
        locControl.locName = "en";
        toolBarTable.addCell(locControl);

        var locButton = this.ToolButtonAdditional("localizationButton", "EN");
        if (locButton.caption) locButton.caption.style.textAlign = "center";
        locControl.appendChild(locButton);
        locButton.allwaysEnabled = true;

        var locMenu = this.VerticalMenu("localizationMenu", locButton, "Down", null, "stiDesignerMenuMiddleItem");
        locMenu.rightToLeft = true;
        locMenu.innerContent.style.maxHeight = "550px";

        locButton.action = function () { locMenu.changeVisibleState(!locMenu.visible); }

        locControl.action = function () { }

        locControl.setLoc = function (locName, locShortName) {
            this.locName = locName;
            locButton.caption.innerHTML = locShortName ? locShortName.toUpperCase() : locName.toUpperCase();
        }

        locControl.addItems = function (items) {
            locMenu.clear();

            if (items && items.length) {
                items.sort(jsObject.SortByCaption);
                if (items.length > 10) {
                    var locItem = function (menu, itemObject) {
                        var item = jsObject.VerticalMenuItem(menu, itemObject.name, itemObject.caption, itemObject.imageName, itemObject.key, menu.itemsStyle);
                        item.style.height = "32px";
                        item.style.minWidth = "150px";
                        item.style.margin = "2px";
                        item.style.border = "1px solid #c6c6c6";
                        if (item.caption) {
                            item.caption.style.padding = "0 5px 0 5px";
                            item.caption.style.textAlign = "center";
                        }
                        return item;
                    }

                    locMenu.innerContent.style.maxHeight = "600px";
                    var topTable = jsObject.CreateHTMLTable();
                    locMenu.innerContent.appendChild(topTable);
                    var sep = jsObject.FormSeparator();
                    sep.style.margin = "2px";
                    sep.style.display = "none";
                    locMenu.innerContent.appendChild(sep);
                    var itemsTable = jsObject.CreateHTMLTable();
                    locMenu.innerContent.appendChild(itemsTable);

                    var itemsInColumn = parseInt(items.length / 3);
                    var rowIndex = 0;
                    var defaultLocalization = jsObject.GetDefaultLocalization();
                    var topItems = [];

                    for (var i = 0; i < items.length; i++) {
                        var item = locItem(locMenu, items[i]);

                        if (items[i].key == defaultLocalization) {
                            var defaultItem = jsObject.CopyObject(items[i]);
                            defaultItem.caption = jsObject.loc.PropertyMain.Default;
                            topItems.push(defaultItem);
                        }

                        if (items[i].key && (items[i].key == "en" || items[i].key.toLowerCase() == "english")) {
                            topItems.push(items[i]);
                        }

                        if (rowIndex > itemsInColumn) {
                            itemsTable.addCellInRow(0, item);
                            rowIndex = 1;
                        }
                        else {
                            if (itemsTable.tr.length == 0 || rowIndex >= itemsTable.tr.length) itemsTable.addRow();
                            itemsTable.addCellInRow(rowIndex, item);
                            rowIndex++;
                        }
                    }

                    if (topItems.length > 0) {
                        sep.style.display = "";
                        for (var i = 0; i < topItems.length; i++) {
                            topTable.addCell(locItem(locMenu, topItems[i]));
                        }
                    }
                }
                else {
                    locMenu.addItems(items);
                }
            }
        }

        locMenu.action = function (menuItem) {
            locMenu.changeVisibleState(false);
            locControl.setLoc(menuItem.key, menuItem.name);
            locControl.action();
        }

        locMenu.onshow = function () {
            for (var itemKey in this.items) {
                if (this.items[itemKey].caption)
                    this.items[itemKey].setSelected(locControl.locName == this.items[itemKey].key);
            }
        }

        if (this.options.locFiles && this.options.locFiles.length > 0) {
            if (this.options.cultureName) {
                locControl.locName = this.options.cultureName;
                if (locButton.caption) locButton.caption.innerHTML = this.options.cultureName.toUpperCase();
            }

            //Add loc items
            var locItems = [];
            for (var i = 0; i < this.options.locFiles.length; i++) {
                locItems.push(this.Item(this.options.locFiles[i].FileName, this.options.locFiles[i].Description, null, this.options.locFiles[i].CultureName));
            }
            locControl.addItems(locItems);

            //Override methods
            locControl.setLoc = function (locName, locFileName) {
                this.locName = locName;
                this.locFileName = locFileName;
                locButton.caption.innerHTML = locName.toUpperCase();
            }

            locMenu.action = function (menuItem) {
                locMenu.changeVisibleState(false);
                locControl.setLoc(menuItem.key, menuItem.name);
                locControl.action();
            }

            locControl.action = function () {
                var params = {
                    command: "SetLocalization",
                    localization: this.locFileName
                };
                var paramsPos = window.location.href.indexOf("?params=");
                if (jsObject.options.cloudMode && paramsPos > 0) {
                    var firstPartUrl = window.location.href.substring(0, paramsPos);
                    var secondPartUrl = window.location.href.substring(paramsPos + "?params=".length);
                    var urlParams = StiBase64.decode(secondPartUrl).split(";");
                    if (urlParams.length > 0) {
                        urlParams[1] = this.locName;
                        var newParams = "";
                        for (var i = 0; i < urlParams.length; i++) {
                            newParams += urlParams[i];
                            if (i != urlParams.length - 1) newParams += ";";
                        }
                        var newUrl = firstPartUrl + "?params=" + StiBase64.encode(newParams);
                        window.location.href = newUrl;
                    }
                }
                else {
                    jsObject.PostForm(params);
                }
            }
        }
    }

    //About Button
    var aboutButton = this.ToolButtonAdditional("aboutButton", null, "Toolbar.AboutIcon.png", this.loc.MainMenu.menuHelpAboutProgramm.replace("&", "").replace("...", ""))
    aboutButton.style.marginLeft = "3px";
    toolBarTable.addCell(aboutButton);
    aboutButton.allwaysEnabled = true;
    aboutButton.style.display = this.options.showAboutButton ? "" : "none";

    //Show Toolbar button
    var showToolBarButton = this.ToolButtonAdditional("showToolBarButton", null, this.isDarkToolBar() ? "Toolbar.ShowToolbarWhite.png" : "Toolbar.ShowToolbar.png");
    showToolBarButton.allwaysEnabled = true;
    showToolBarButton.style.display = "none";
    showToolBarButton.style.marginLeft = "3px";
    toolBarTable.addCell(showToolBarButton);

    this.options.buttons["homeToolButton"].setSelected(true);
}

StiMobileDesigner.prototype.isDarkToolBar = function () {
    return this.isOffice2013Theme();
}