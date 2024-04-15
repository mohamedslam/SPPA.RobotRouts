
StiMobileDesigner.prototype.InitializeFileMenu = function () {
    var fileMenu = document.createElement("div");
    fileMenu.id = this.options.mobileDesigner.id + "fileMenu";
    fileMenu.className = "stiDesignerFileMenu";
    var jsObject = fileMenu.jsObject = this;
    fileMenu.items = {};
    fileMenu.style.display = "none";
    fileMenu.style.width = "0px";
    fileMenu.style.minHeight = this.options.cloudMode || this.options.serverMode ? "400px" : "500px";
    this.options.menus["fileMenu"] = fileMenu;
    this.options.mainPanel.appendChild(fileMenu);

    fileMenu.appendChild(this.FileMenuCloseButton());
    if (this.options.showFileMenuInfo)
        fileMenu.appendChild(this.FileMenuItem("infoReport", this.loc.ReportInfo.Info, "FileMenu.Home.png"));
    if (this.options.showFileMenuNew)
        fileMenu.appendChild(this.FileMenuItem("newReport", this.loc.MainMenu.menuFileNew.replace("&", ""), "FileMenu.Report.png"));
    if (this.options.showFileMenuOpen && this.options.canOpenFiles)
        fileMenu.appendChild(this.FileMenuItem("openReport", this.loc.MainMenu.menuFileOpen.replace("&", "").replace("...", ""), "FileMenu.Folder.png"));
    if (this.options.showFileMenuInfo || this.options.showFileMenuNew || this.options.showFileMenuOpen)
        fileMenu.appendChild(this.FileMenuSeparator());
    if (this.options.showFileMenuSave)
        fileMenu.appendChild(this.FileMenuItem("saveReport", this.loc.A_WebViewer.SaveReport));
    if (this.options.showFileMenuSaveAs)
        fileMenu.appendChild(this.FileMenuItem("saveAsReport", this.loc.MainMenu.menuFileSaveAs.replace("...", "")));
    fileMenu.appendChild(this.FileMenuItem("preview", this.loc.FormDesigner.Preview));
    fileMenu.appendChild(this.FileMenuItem("share", this.loc.Cloud.ButtonShare));
    fileMenu.appendChild(this.FileMenuItem("publish", this.loc.Cloud.ButtonPublish));
    if (this.options.showFileMenuHelp)
        fileMenu.appendChild(this.FileMenuItem("help", this.loc.Buttons.Help));
    if (this.options.showFileMenuClose)
        fileMenu.appendChild(this.FileMenuItem("closeReport", this.loc.MainMenu.menuFileClose.replace("&", "")));
    if (this.options.showFileMenuExit)
        fileMenu.appendChild(this.FileMenuItem("exitDesigner", this.loc.MainMenu.menuFileExit.replace("&", "")));
    if (this.options.showFileMenuOptions || this.options.showFileMenuAbout || this.options.cloudMode || this.options.standaloneJsMode)
        fileMenu.appendChild(this.FileMenuSeparator());
    if (this.options.cloudMode || this.options.standaloneJsMode)
        fileMenu.appendChild(this.FileMenuItem("account", this.loc.Cloud.Account));
    if (this.options.showFileMenuOptions)
        fileMenu.appendChild(this.FileMenuItem("optionsDesigner", this.loc.FormOptions.title));
    if (this.options.showFileMenuAbout)
        fileMenu.appendChild(this.FileMenuItem("aboutDesigner", this.loc.MainMenu.menuHelpAboutProgramm.replace("&", "").replace("...", "")));

    fileMenu.changeVisibleState = function (state) {
        var options = jsObject.options;
        if (options.viewerContainer)
            options.viewerContainer.changeVisibleState(options.previewMode && !options.mvcMode ? !state : false);
        options.toolBar.changeVisibleState(!state);
        options.infoPanel.changeVisibleState(options.infoPanel.visibleState ? !state : false);
        options.workPanel.changeVisibleState(options.workPanel.visibleState ? !state : false);
        options.statusPanel.changeVisibleState(!state);
        options.pagesPanel.changeVisibleState((options.previewMode && options.mvcMode) ? false : !state);
        if (options.showPanelPropertiesAndDictionary) {
            options.propertiesPanel.changeVisibleState(!options.propertiesPanel.fixedViewMode ? (options.previewMode && options.mvcMode ? false : !state) : false);
            options.propertiesPanel.showButtonsPanel.changeVisibleState((options.previewMode && options.mvcMode) || !options.propertiesPanel.fixedViewMode ? false : (state ? false : true));
        }
        options.paintPanel.changeVisibleState(!state);

        if (state) {
            if (options.currentForm && options.currentForm.name != "whoAreYouForm" && options.currentForm.name != "notificationCheckTrDaysForm" && options.currentForm.name != "authForm") {
                options.currentForm.changeVisibleState(false);
            }
            if (options.showFileMenuSave) this.items.saveReport.setEnabled(options.report || options.formsDesignerMode);
            if (options.showFileMenuSaveAs) this.items.saveAsReport.setEnabled(options.report || options.formsDesignerMode);
            if (options.showFileMenuClose) this.items.closeReport.setEnabled(options.report || options.formsDesignerMode);
            if (options.showFileMenuInfo) this.items.infoReport.setEnabled(options.report);
            if (options.showFileMenuOptions) this.items.optionsDesigner.setEnabled(options.report);

            if (this.items.share) {
                this.items.share.setEnabled(options.report);
                this.items.share.style.display = (options.cloudMode || options.standaloneJsMode) ? "" : "none";
            }
            if (this.items.publish) {
                this.items.publish.style.display = ((options.cloudMode || options.standaloneJsMode) && !options.designerSpecification == "Developer") ? "" : "none";
                this.items.publish.setEnabled(options.report);
            }
            if (this.items.preview) {
                this.items.preview.setEnabled(options.report);
                this.items.preview.style.display = options.standaloneJsMode ? "" : "none";
            }

            if (options.cloudMode || options.serverMode) {
                if (options.cloudParameters && (options.cloudParameters.thenOpenWizard || !options.report)) {
                    this.items.newReport.action();
                }
                else {
                    this.items.openReport.action();
                }
            }
            else if (options.showFileMenuNew) {
                this.items.newReport.action();
            }

            this.style.display = "";
        }
        else {
            this.closeAllPanels();

            if (options.previewMode) {
                options.buttons.homeToolButton.action();
            }
            if (options.toolbox) {
                options.toolbox.resize();
            }
            if (jsObject.options.formsDesignerMode && jsObject.options.formsDesignerFrame) {
                jsObject.options.formsDesignerFrame.show();
            }
        }

        this.visible = state;

        var d = new Date();
        var endTime = d.getTime() + options.menuAnimDuration;

        jsObject.ShowAnimationFileMenu(this, (state ? 170 : 0), endTime);
    }

    fileMenu.action = function (menuItem) {
        this.closeAllPanels(menuItem.name);
        jsObject.ExecuteAction(menuItem.name);
    }

    fileMenu.closeAllPanels = function (openingPanelName) {
        if (jsObject.options.newReportPanel && openingPanelName != "newReport") jsObject.options.newReportPanel.changeVisibleState(false);
        if (jsObject.options.infoReportPanel && openingPanelName != "infoReport") jsObject.options.infoReportPanel.changeVisibleState(false);
        if (jsObject.options.openPanel && openingPanelName != "openReport") jsObject.options.openPanel.changeVisibleState(false);
        if (jsObject.options.saveAsPanel && openingPanelName != "saveAsReport") jsObject.options.saveAsPanel.changeVisibleState(false);
        if (jsObject.options.helpPanel && openingPanelName != "help") jsObject.options.helpPanel.changeVisibleState(false);
        if (jsObject.options.accountPanel && openingPanelName != "account") jsObject.options.accountPanel.changeVisibleState(false);
    }

    return fileMenu;
}

StiMobileDesigner.prototype.FileMenuItem = function (name, caption, imageName, imageSizes) {
    var menuItem = document.createElement("div");
    menuItem.jsObject = this;
    menuItem.isEnabled = true;
    menuItem.isSelected = false;
    menuItem.name = name;
    menuItem.id = this.options.mobileDesigner.id + "FileMenuItem" + name;
    menuItem.className = "stiDesignerFileMenuItem";
    menuItem.style.whiteSpace = "normal";
    menuItem.style.lineHeight = "1.4";
    this.options.menus["fileMenu"].items[name] = menuItem;

    var innerTable = this.CreateHTMLTable();
    menuItem.appendChild(innerTable);
    var imageBlock = document.createElement("div");
    innerTable.addCell(imageBlock);
    imageBlock.setAttribute("style", "width: 40px; text-align: center; line-height: 0;");

    if (imageName) {
        var image = menuItem.image = document.createElement("img");
        StiMobileDesigner.setImageSource(image, this.options, imageName);
        image.style.width = (imageSizes ? imageSizes.width : 16) + "px";
        image.style.height = (imageSizes ? imageSizes.height : 16) + "px";
        imageBlock.appendChild(image);
    }

    innerTable.addTextCell(caption);

    menuItem.onmouseover = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseenter();
    }

    menuItem.onmouseenter = function () {
        if (!this.isEnabled || this.jsObject.options.isTouchClick) return;
        this.className = "stiDesignerFileMenuItemOver";
    }

    menuItem.onmouseleave = function () {
        if (!this.isEnabled) return;
        this.className = "stiDesignerFileMenuItem";
        this.className = this.isSelected ? "stiDesignerFileMenuItemSelected" : "stiDesignerFileMenuItem";
    }

    menuItem.onclick = function () {
        if (this.isTouchEndFlag || !this.isEnabled || this.jsObject.options.isTouchClick) return;
        this.action();
    }

    menuItem.ontouchend = function () {
        if (!this.isEnabled) return;
        this.className = "stiDesignerFileMenuItemOver";
        var this_ = this;
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);

        setTimeout(function () {
            this_.className = "stiDesignerFileMenuItem";
            this_.action();
        }, 500);

        this.isTouchEndTimer = setTimeout(function () {
            this_.isTouchEndFlag = false;
        }, 1000);
    }

    menuItem.action = function () {
        this.setSelected(true);
        this.jsObject.options.menus.fileMenu.action(this);
    }

    menuItem.setEnabled = function (state) {
        this.isEnabled = state;
        this.className = state ? "stiDesignerFileMenuItem" : "stiDesignerFileMenuItemDisabled";
        if (this.image) this.image.style.opacity = state ? "1" : "0.7";
    }

    menuItem.setSelected = function (state) {
        if (state) {
            var items = this.jsObject.options.menus.fileMenu.items;
            for (var name in items) {
                if ("setSelected" in items[name]) items[name].setSelected(false);
            }
        }
        this.isSelected = state;
        this.className = state ? "stiDesignerFileMenuItemSelected" : (this.isEnabled ? "stiDesignerFileMenuItem" : "stiDesignerFileMenuItemDisabled");
    }

    return menuItem;
}

StiMobileDesigner.prototype.FileMenuSeparator = function () {
    var menuSeparator = document.createElement("div");
    menuSeparator.style.padding = "5px 25px 5px 25px"
    var menuSeparatorInner = document.createElement("div");
    menuSeparatorInner.className = "stiDesignerFileMenuSeparator";
    menuSeparator.appendChild(menuSeparatorInner);

    return menuSeparator;
}

StiMobileDesigner.prototype.FileMenuCloseButton = function () {
    var closeButtonParent = document.createElement("div");
    closeButtonParent.style.padding = "20px 0px 10px 23px";

    var closeButton = document.createElement("img");
    closeButton.style.width = closeButton.style.height = "35px";
    StiMobileDesigner.setImageSource(closeButton, this.options, "CloseFileMenu.png");
    closeButton.jsObject = this;
    closeButton.name = "closeFileMenu";
    closeButton.id = this.options.mobileDesigner.id + closeButton.name;
    closeButtonParent.appendChild(closeButton);

    closeButton.onmouseover = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseenter();
    }

    closeButton.onmouseenter = function () {
        if (this.jsObject.options.isTouchClick) return;
        this.style.opacity = "0.7";
    }

    closeButton.onmouseleave = function () {
        this.style.opacity = "1";
    }

    closeButton.onclick = function () {
        if (this.isTouchEndFlag) return;
        this.action();
    }

    closeButton.ontouchend = function () {
        this.style.opacity = "0.7";
        var this_ = this;
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);
        setTimeout(function () {
            this_.style.opacity = "1";
            this_.action();
        }, 150);
        this.isTouchEndTimer = setTimeout(function () {
            this_.isTouchEndFlag = false;
        }, 1000);
    }

    closeButton.action = function () {
        this.jsObject.ExecuteAction(this.name);
    }

    return closeButtonParent;
}