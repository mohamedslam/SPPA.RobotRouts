
StiMobileDesigner.prototype.InitializePagesPanel = function () {
    var jsObject = this;
    var pagesPanel = document.createElement("div");
    pagesPanel.id = this.options.mobileDesigner.id + "pagesPanel";
    pagesPanel.className = "stiDesignerPagesPanel";
    pagesPanel.jsObject = this;

    this.options.pagesPanel = pagesPanel;
    this.options.mainPanel.appendChild(pagesPanel);

    pagesPanel.style.top = (this.options.toolBar.offsetHeight + this.options.workPanel.offsetHeight + this.options.infoPanel.offsetHeight) + "px";

    if (this.options.propertiesGridPosition == "Right") {
        pagesPanel.style.right = this.options.propertiesPanel.offsetWidth + "px";
        pagesPanel.style.left = (this.options.toolbox ? this.options.toolbox.offsetWidth : 0) + "px";
    }
    else {
        pagesPanel.style.left = (this.options.propertiesPanel.offsetWidth + (this.options.toolbox ? this.options.toolbox.offsetWidth : 0)) + "px";
        pagesPanel.style.right = "0px";
    }

    pagesPanel.style.height = "34px";
    pagesPanel.style.overflow = "hidden";

    var buttonsSize = "26px";
    var innerTable = this.CreateHTMLTable();
    var lButton = this.StandartSmallButton("scrollLeft", null, null, "Arrows.ArrowLeft.png");
    lButton.style.width = lButton.style.height = buttonsSize;
    lButton.style.margin = "3px 0 2px 3px";
    lButton.innerTable.style.width = "100%";
    innerTable.addCell(lButton);

    var rButton = this.StandartSmallButton("scrollRight", null, null, "Arrows.ArrowRight.png");
    rButton.style.width = rButton.style.height = buttonsSize;
    rButton.style.margin = "3px 0 2px 3px";
    rButton.innerTable.style.width = "100%";
    innerTable.addCell(rButton);

    var addButton = this.StandartSmallButton("addPage", null, null, "PagePlus.png", this.loc.MainMenu.menuFilePageNew);
    addButton.style.width = addButton.style.height = buttonsSize;
    addButton.innerTable.style.width = "100%";
    var hideAddPageButton = this.options.dashboardAssemblyLoaded ? (this.options.showNewPageButton === false && this.options.showNewDashboardButton === false) : this.options.showNewPageButton === false;
    addButton.style.display = hideAddPageButton ? "none" : "";

    if (this.options.dashboardAssemblyLoaded && this.options.showNewPageButton === true && this.options.showNewDashboardButton === true) {
        addButton.action = function () {
            var items = [];
            items.push(jsObject.Item("addPage", jsObject.loc.MainMenu.menuFilePageNew, "AddPage.png", "addPage"));
            items.push(jsObject.Item("addDashboard", jsObject.loc.MainMenu.menuFileDashboardNew, "AddDashboard.png", "addDashboard"));

            var menu = jsObject.VerticalMenu("addPageAndDashboardMenu", addButton, "Down", items);

            menu.action = function (menuItem) {
                jsObject.ExecuteAction(menuItem.key);
                this.changeVisibleState(false);
            }

            menu.changeVisibleState(!menu.visible);
        }
    }

    pagesPanel.appendChild(innerTable);

    var pagesCont = document.createElement("div");
    pagesCont.pages = [];
    pagesPanel.appendChild(pagesCont);
    pagesPanel.pagesContainer = pagesCont;
    pagesCont.className = "stiDesignerPagesContainer"
    pagesCont.style.height = "70px";
    pagesCont.style.left = "65px";

    pagesPanel.updateScrollButtons = function () {
        if (!pagesCont || !pagesCont.innerTable) return;
        var haveScroll = pagesCont.innerTable.offsetWidth > pagesCont.offsetWidth;
        lButton.style.display = haveScroll ? "" : "none";
        rButton.style.display = haveScroll ? "" : "none";
        pagesCont.style.left = haveScroll ? "65px" : "15px";
    }

    this.addEvent(window, 'resize', function () {
        pagesPanel.updateScrollButtons();
    });

    lButton.action = function () {
        pagesCont.scrollLeft = pagesCont.scrollLeft - 40;
    };

    rButton.action = function () {
        pagesCont.scrollLeft = pagesCont.scrollLeft + 40;
    };

    lButton.onmousedown = function () {
        if (!this.isEnabled) return;
        jsObject.options.buttonPressed = this;
        jsObject.options.scrollLeftTimer = setInterval(function () { lButton.action(); }, 80);
    }
    rButton.onmousedown = function () {
        if (!this.isEnabled) return;
        jsObject.options.buttonPressed = this;
        jsObject.options.scrollRightTimer = setInterval(function () { rButton.action(); }, 80);
    }

    pagesCont.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
    }

    pagesCont.updatePages = function () {
        this.pages = [];
        this.clear();

        var pagesCount = jsObject.options.paintPanel.getPagesCount();
        var innerTable = pagesCont.innerTable = jsObject.CreateHTMLTable();
        this.appendChild(innerTable);

        var pageButton = null;
        var selectedButton = null;

        for (var i = 0; i < pagesCount; i++) {
            var page = jsObject.options.paintPanel.findPageByIndex(i);
            if (page) {
                pageButton = jsObject.PagesButton(page);
                pageButton.ownerPage = page;
                pageButton.style.margin = "3px 3px 3px 0";
                pageButton.style.opacity = page.properties.enabled === false ? "0.5" : "1";
                this.pages.push(pageButton);
                innerTable.addCell(pageButton).style.verticalAlign = "bottom";

                if (page == jsObject.options.currentPage) {
                    selectedButton = pageButton;
                }
            }
        }

        if (selectedButton) {
            selectedButton.setSelected(true);
        }

        pagesPanel.updateScrollButtons();
        innerTable.addCell(addButton);
        addButton.onmouseleave();
    }

    pagesPanel.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        jsObject.options.paintPanel.style.top = (jsObject.options.toolBar.offsetHeight + jsObject.options.workPanel.offsetHeight + jsObject.options.infoPanel.offsetHeight + jsObject.offsetHeight) + "px";
    }
}