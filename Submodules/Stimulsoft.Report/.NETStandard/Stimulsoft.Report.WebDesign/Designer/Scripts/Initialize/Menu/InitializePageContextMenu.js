
StiMobileDesigner.prototype.InitializePageContextMenu = function () {
    var items = [];
    if (this.options.showNewPageButton !== false) {
        items.push(this.Item("addPage", this.loc.MainMenu.menuFilePageNew, "AddPage.png", "addPage"));
    }
    if (this.options.dashboardAssemblyLoaded && this.options.showNewDashboardButton !== false) {
        items.push(this.Item("addDashboard", this.loc.MainMenu.menuFileDashboardNew, "AddDashboard.png", "addDashboard"));
        items.push("separator0");
    }
    items.push(this.Item("duplicatePage", this.loc.Buttons.Duplicate, "Duplicate.png", "duplicatePage"));
    items.push(this.Item("renamePage", this.loc.Buttons.Rename, "Rename.png", "renamePage"));
    items.push(this.Item("removePage", this.loc.Buttons.Delete, "Remove.png", "removePage"));
    items.push("separator0_1");
    items.push(this.Item("viewQuery", this.loc.QueryBuilder.ViewQuery, "Query.ViewData.png", "viewQuery"));
    items.push("separator1");
    items.push(this.Item("pageMoveLeft", this.loc.Buttons.MoveLeft, "PageMoveLeft.png", "pageMoveLeft"));
    items.push(this.Item("pageMoveRight", this.loc.Buttons.MoveRight, "PageMoveRight.png", "pageMoveRight"));
    items.push("separator2");
    items.push(this.Item("pageSetup", this.loc.Toolbars.ToolbarPageSetup, "PageSetup.png", "pageSetup"));
    items.push("separator3");
    items.push(this.Item("openPage", this.loc.MainMenu.menuFilePageOpen, "Open.png", "openPage"));
    items.push(this.Item("savePage", this.loc.MainMenu.menuFilePageSaveAs, "Save.png", "savePage"));

    var menu = this.BaseContextMenu("pageContextMenu", "Up", items);

    menu.action = function (menuItem) {
        if (menuItem.key == "renamePage") {
            this.pageButton.setEditMode(true);
        }
        else {
            this.jsObject.ExecuteAction(menuItem.key);
        }
        this.changeVisibleState(false);
    }

    menu.onshow = function () {
        var pagesCount = this.jsObject.options.paintPanel.getPagesCount();
        var currentPage = this.jsObject.options.currentPage;
        if (currentPage) {
            var pageIndex = this.jsObject.StrToInt(currentPage.properties.pageIndex);
            this.items["pageMoveLeft"].setEnabled(pageIndex > 0);
            this.items["pageMoveRight"].setEnabled(pageIndex < pagesCount - 1);
            this.items["removePage"].setEnabled(pagesCount > 1);
            this.items["renamePage"].style.display = ((this.jsObject.options.useAliases && this.jsObject.options.showOnlyAliasForPages) || currentPage.properties.aliasName) ? "none" : "";
        }
        this.items["viewQuery"].style.display = this.items["separator0_1"].style.display = currentPage && currentPage.isDashboard ? "" : "none";
    }

    return menu;
}