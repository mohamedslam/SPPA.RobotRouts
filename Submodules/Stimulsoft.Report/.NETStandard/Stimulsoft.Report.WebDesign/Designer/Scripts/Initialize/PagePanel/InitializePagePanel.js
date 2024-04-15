
StiMobileDesigner.prototype.InitializePagePanel = function () {
    var jsObject = this;
    var pagePanel = this.ChildWorkPanel("pagePanel", "stiDesignerLayoutPanel");
    pagePanel.style.display = "none";

    pagePanel.mainTable = this.CreateHTMLTable();
    pagePanel.mainTable.addCell(this.PageSetupBlock());
    pagePanel.mainTable.addCell(this.GroupBlockSeparator());
    var pageViewBlock = this.PageViewBlock();
    var viewBlockSep = this.GroupBlockSeparator();
    pagePanel.mainTable.addCell(pageViewBlock);
    pagePanel.mainTable.addCell(viewBlockSep);
    pagePanel.mainTable.addCell(this.ViewOptionsBlock());
    pagePanel.mainTable.addCell(this.GroupBlockSeparator());
    pagePanel.appendChild(pagePanel.mainTable);

    pagePanel.updateControls = function () {
        var buttons = jsObject.options.buttons;
        var designerOptions = jsObject.options.report ? jsObject.options.report.info : null;
        var currentPage = jsObject.options.currentPage;

        var buttonNames = ["pagePanelShowGrid", "pagePanelAlignToGrid", "pagePanelShowHeaders", "pagePanelGridMode",
            "groupBlockPageSetupButton", "marginsPage", "orientationPage", "pageSize", "columnsPage", "watermarkPage"];

        for (var i = 0; i < buttonNames.length; i++) {
            var button = buttons[buttonNames[i]];
            if (button) {
                var isEnabled = designerOptions;
                if (currentPage && currentPage.isDashboard && ["marginsPage", "orientationPage", "pageSize", "columnsPage", "pagePanelShowHeaders"].indexOf(buttonNames[i]) >= 0) {
                    isEnabled = false;
                }
                button.setEnabled(isEnabled);
            }
        }

        if (designerOptions) {
            buttons.pagePanelShowGrid.setSelected(designerOptions.showGrid);
            buttons.pagePanelAlignToGrid.setSelected(designerOptions.alignToGrid);
            buttons.pagePanelShowHeaders.setSelected(designerOptions.showHeaders);
            StiMobileDesigner.setImageSource(buttons.pagePanelGridMode.image, jsObject.options, "ViewOptions.Grid" + designerOptions.gridMode + ".png");
        }

        if (currentPage && currentPage.isDashboard) {
            buttons.pageViewDesktop.setSelected(currentPage.properties.dashboardViewMode == "Desktop");
            buttons.pageViewMobile.setSelected(currentPage.properties.dashboardViewMode == "Mobile");
            buttons.pageViewComponentsButton.setEnabled(currentPage.properties.dashboardViewMode == "Mobile");
            buttons.pageViewRemoveMobile.parentElement.style.display = buttons.pageViewComponentsButton.parentElement.style.display =
                pageViewBlock.mobileSep.parentElement.style.display = currentPage.properties.mobileViewModePresent ? "" : "none";
        }
        pageViewBlock.style.display = viewBlockSep.style.display = currentPage && currentPage.isDashboard ? "" : "none";
    }
}

//PageSetup
StiMobileDesigner.prototype.PageSetupBlock = function () {
    var jsObject = this;
    var pageSetupGroupBlock = this.GroupBlock("groupBlockPageSetup", this.loc.Toolbars.ToolbarPageSetup, true, null);
    var innerTable = this.GroupBlockInnerTable();
    pageSetupGroupBlock.container.appendChild(innerTable);
    innerTable.style.height = "100%";

    //Margins
    var marginsButton = this.BigButton("marginsPage", null, this.loc.FormPageSetup.Margins, "Margins.png",
        [this.loc.HelpDesigner.Margins, this.HelpLinks["page"]], true, "stiDesignerStandartBigButton", null, 60);
    marginsButton.cellImage.style.height = "40px";
    innerTable.addCell(marginsButton).style.padding = "2px";
    var marginsMenu = this.MarginsMenu();

    marginsButton.action = function () {
        marginsMenu.changeVisibleState(!marginsMenu.visible);
    }

    //Orientation
    var orientationButton = this.BigButton("orientationPage", null, this.loc.FormPageSetup.Orientation, "Orientation.png",
        [this.loc.HelpDesigner.Orientation, this.HelpLinks["page"]], true, "stiDesignerStandartBigButton", null, 70);
    orientationButton.cellImage.style.height = "40px";
    innerTable.addCell(orientationButton).style.padding = "2px";
    var orientationMenu = this.OrientationMenu();

    orientationButton.action = function () {
        orientationMenu.changeVisibleState(!orientationMenu.visible);
    }

    //PageSize
    var pageSizeButton = this.BigButton("pageSize", null, this.loc.PropertyMain.Size, "Size.png",
        [this.loc.HelpDesigner.PageSize, this.HelpLinks["page"]], true, "stiDesignerStandartBigButton", null, 60);
    pageSizeButton.cellImage.style.height = "40px";
    innerTable.addCell(pageSizeButton).style.padding = "2px";
    var pageSizeMenu = this.PageSizeMenu("pageSizeMenu", pageSizeButton);

    pageSizeButton.action = function () {
        pageSizeMenu.changeVisibleState(!pageSizeMenu.visible);
    }

    //Columns
    var columnsButton = this.BigButton("columnsPage", null, this.loc.FormPageSetup.Columns, "Columns.png",
        [this.loc.HelpDesigner.Columns, this.HelpLinks["page"]], true, "stiDesignerStandartBigButton", null, 60);
    columnsButton.cellImage.style.height = "40px";
    innerTable.addCell(columnsButton).style.padding = "2px";
    var columnsMenu = this.ColumnsMenu();

    columnsButton.action = function () {
        columnsMenu.changeVisibleState(!columnsMenu.visible);
    }

    //Watermark
    var watermarkButton = this.StandartBigButton("watermarkPage", null, this.loc.PropertyMain.Watermark, "PageWatermark.png",
        [this.loc.PropertyMain.Watermark, this.HelpLinks["page"]], 60);
    innerTable.addCell(watermarkButton).style.padding = "2px";

    watermarkButton.action = function () {
        var currentPage = jsObject.options.currentPage;
        var currentObject = jsObject.options.selectedObject;

        if ((currentObject && currentObject.typeComponent == "StiPanelElement") || currentPage.isDashboard) {
            jsObject.InitializeDashboardWatermarkForm(function (form) {
                form.show();
            });
        }
        else {
            jsObject.InitializePageSetupForm(function (pageSetupForm) {
                pageSetupForm.changeVisibleState(true);
                pageSetupForm.setMode("Watermark");
            });
        }
    }

    return pageSetupGroupBlock;
}

//PageView
StiMobileDesigner.prototype.PageViewBlock = function () {
    var jsObject = this;
    var pageViewGroupBlock = this.GroupBlock("groupBlockPageView", this.loc.Toolbars.TabView);
    var innerTable = this.GroupBlockInnerTable();
    pageViewGroupBlock.container.appendChild(innerTable);
    innerTable.style.height = "100%";

    //Desktop
    var desktopButton = this.StandartBigButton("pageViewDesktop", null, this.loc.Dashboard.ViewModeDesktop, "ViewDesktop.png",
        [this.loc.Dashboard.ViewModeDesktop, this.HelpLinks["page"]], 70);
    innerTable.addCell(desktopButton).style.padding = "2px";

    //Mobile
    var mobileButton = this.StandartBigButton("pageViewMobile", null, this.loc.Dashboard.ViewModeMobile, "ViewMobile.png",
        [this.loc.Dashboard.ViewModeMobile, this.HelpLinks["page"]], 60);
    innerTable.addCell(mobileButton).style.padding = "2px";

    //Separator
    pageViewGroupBlock.mobileSep = this.HomePanelSeparator();
    pageViewGroupBlock.mobileSep.style.height = this.options.isTouchDevice ? "90px" : "70px";
    innerTable.addCell(pageViewGroupBlock.mobileSep);

    //Remove Mobile Surface
    var removeMobileButton = this.StandartBigButton("pageViewRemoveMobile", null, this.loc.Dashboard.RemoveMobileSurface, "RemoveMobileSurface.png",
        [this.loc.Dashboard.RemoveMobileSurface, this.HelpLinks["page"]], 90);
    innerTable.addCell(removeMobileButton).style.padding = "2px";

    //Components
    var componentsButton = this.StandartBigButton("pageViewComponentsButton", null, this.loc.Report.Components, "UnplacedComponents.png",
        [this.loc.Report.Components, this.HelpLinks["page"]], 80);
    innerTable.addCell(componentsButton).style.padding = "2px";

    var changeDashboardViewMode = function (viewMode, removeMobileSurface) {
        var currentPage = jsObject.options.currentPage;
        if (currentPage) {
            currentPage.properties.dashboardViewMode = viewMode;
            if (removeMobileSurface) currentPage.properties.mobileViewModePresent = false;
            desktopButton.setSelected(viewMode == "Desktop");
            mobileButton.setSelected(viewMode == "Mobile");
            jsObject.SendCommandChangeDashboardViewMode(currentPage.properties.name, viewMode, removeMobileSurface);
        }
        if (jsObject.options.forms.mobileViewComponentsForm && jsObject.options.forms.mobileViewComponentsForm.visible) {
            jsObject.options.forms.mobileViewComponentsForm.changeVisibleState(false);
        }
    }

    desktopButton.action = function () {
        if (this.isSelected) return;
        changeDashboardViewMode("Desktop");
    }

    mobileButton.action = function () {
        if (this.isSelected) return;
        changeDashboardViewMode("Mobile");
    }

    removeMobileButton.action = function () {
        var messageForm = jsObject.MessageFormForRemoveMobileSurface();
        messageForm.changeVisibleState(true);
        messageForm.action = function (state) {
            if (state)
                changeDashboardViewMode("Desktop", true);
            else
                this.changeVisibleState(false);
        }
    }

    componentsButton.action = function () {
        this.setSelected(!this.isSelected);

        if (this.isSelected) {
            var currentPage = jsObject.options.currentPage;
            if (currentPage) {
                jsObject.SendCommandToDesignerServer("GetMobileViewUnplacedElements", { dashboardName: currentPage.properties.name }, function (answer) {
                    jsObject.InitializeMobileViewComponentsForm(function (form) {
                        form.show(answer.elements);
                    });
                });
            }
        }
        else if (jsObject.options.forms.mobileViewComponentsForm && jsObject.options.forms.mobileViewComponentsForm.visible) {
            jsObject.options.forms.mobileViewComponentsForm.changeVisibleState(false);
        }
    }

    return pageViewGroupBlock;
}

//View Options
StiMobileDesigner.prototype.ViewOptionsBlock = function () {
    var viewOptionsGroupBlock = this.GroupBlock("groupBlockViewOptions", this.loc.Toolbars.ToolbarViewOptions);
    var innerTable = this.CreateHTMLTable();
    viewOptionsGroupBlock.container.appendChild(innerTable);
    innerTable.style.height = this.options.isTouchDevice ? "65px" : "55px";

    //ShowGrid
    var showGridButton = this.StandartSmallButton("pagePanelShowGrid", null, this.loc.MainMenu.menuViewShowGrid, "ViewOptions.ShowGrid.png",
        [this.loc.HelpDesigner.menuViewShowGrid, this.HelpLinks["viewOptions"]], null);
    showGridButton.propertyName = "showGrid";
    innerTable.addCell(showGridButton).style.padding = "1px 1px 0px 1px";

    //Show Headers
    var showHeadersButton = this.StandartSmallButton("pagePanelShowHeaders", null, this.loc.MainMenu.menuViewShowHeaders, "ViewOptions.ShowHeaders.png",
        [this.loc.HelpDesigner.menuViewShowHeaders, this.HelpLinks["viewOptions"]], null);
    innerTable.addCell(showHeadersButton).style.padding = "1px 1px 0px 1px";
    showHeadersButton.propertyName = "showHeaders";

    //AlignToGrid
    var alignToGridButton = this.StandartSmallButton("pagePanelAlignToGrid", null, this.loc.MainMenu.menuViewAlignToGrid, "ViewOptions.AlignToGrid.png",
        [this.loc.HelpDesigner.menuViewAlignToGrid, this.HelpLinks["viewOptions"]], null);
    alignToGridButton.propertyName = "alignToGrid";
    innerTable.addCellInNextRow(alignToGridButton).style.padding = "0 1px 0 1px";

    //GridMode
    var gridModeButton = this.StandartSmallButton("pagePanelGridMode", null, this.loc.FormOptions.GridMode, "ViewOptions.GridDots.png",
        [this.loc.HelpDesigner.GridMode, this.HelpLinks["viewOptions"]], "Down");
    innerTable.addCellInLastRow(gridModeButton).style.padding = "0 1px 1px 1px";

    var gridModeMenu = this.GridModeMenu();
    gridModeButton.action = function () { gridModeMenu.changeVisibleState(!gridModeMenu.visible); };

    var changeViewOption = function (button) {
        if (!button.jsObject.options.report) return;
        var designerOptions = button.jsObject.options.report.info;
        button.setSelected(!button.isSelected);
        designerOptions[button.propertyName] = button.isSelected;
        button.jsObject.SendCommandApplyDesignerOptions(designerOptions);
    }

    showGridButton.action = function () { changeViewOption(this); }
    showHeadersButton.action = function () { changeViewOption(this); }
    alignToGridButton.action = function () { changeViewOption(this); }

    return viewOptionsGroupBlock;
}