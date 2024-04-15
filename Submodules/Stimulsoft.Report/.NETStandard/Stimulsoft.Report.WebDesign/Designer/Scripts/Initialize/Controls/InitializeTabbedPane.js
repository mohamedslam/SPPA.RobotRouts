
StiMobileDesigner.prototype.TabbedPane = function (name, tabsArray, styleTabs) {
    var tabbedPane = document.createElement("div");
    var jsObject = tabbedPane.jsObject = this;

    tabbedPane.className = "stiDesignerTabbedPane";
    tabbedPane.name = name;
    tabbedPane.tabs = {};
    tabbedPane.tabsPanels = {};
    tabbedPane.selectedTab = null;

    if (name) {
        this.options.controls[name] = tabbedPane;
    }

    //Tabs
    var tabsPanel = document.createElement("div");
    tabsPanel.className = "stiDesignerTabsPanel";
    tabbedPane.appendChild(tabsPanel);
    tabbedPane.tabsPanel = tabsPanel;

    var tabsPanelTable = this.CreateHTMLTable();
    tabsPanel.appendChild(tabsPanelTable);

    var sep = this.FormSeparator();
    sep.style.margin = "6px 0 6px 0";
    tabbedPane.appendChild(sep);

    var paneContainer = tabbedPane.container = document.createElement("div");
    paneContainer.className = "stiDesignerTabbedPaneContainer";
    tabbedPane.appendChild(paneContainer);

    for (var i = 0; i < tabsArray.length; i++) {
        var tab = tabbedPane.tabs[tabsArray[i].name] = this.Tab(tabbedPane, tabsArray[i].name, tabsArray[i].caption, styleTabs);
        tab.style.marginRight = "2px";
        tabsPanelTable.addCell(tab).style.width = "1px";

        var tabPanel = document.createElement("div");
        tabPanel.className = "stiDesignerTabbedPanePanel";
        tabPanel.name = tabsArray[i].name;
        tabbedPane.tabsPanels[tabPanel.name] = tabPanel;
        paneContainer.appendChild(tabPanel);

        tabPanel.onshow = function () { };
    }

    tabsPanelTable.addCell();

    tabbedPane.showTabPanel = function (name) {
        for (var namePanel in this.tabsPanels) {
            if (namePanel == name) {
                this.tabsPanels[namePanel].style.display = "";
                this.tabs[name].setSelected(true);
                this.selectedTab = this.tabs[name];
                this.tabsPanels[namePanel].onshow();
            }
            else {
                this.tabsPanels[namePanel].style.display = "none";
            }
        }
    }

    if (tabsArray.length > 0) {
        tabbedPane.showTabPanel(tabsArray[0].name);
    }

    return tabbedPane;
}

StiMobileDesigner.prototype.Tab = function (tabbedPane, name, captionText, styleTabs) {
    if (captionText != null) {
        captionText = captionText.substr(0, 1).toUpperCase() + captionText.substr(1);
    }

    var tab = this.TabButton(tabbedPane.name + name, tabbedPane.name, captionText, null, null, styleTabs);
    tab.tabbedPane = tabbedPane;
    tab.panelName = name;

    tab.action = function () {
        this.tabbedPane.showTabPanel(this.panelName);
    }

    return tab;
}

StiMobileDesigner.prototype.TabButton = function (name, groupName, captionText, imageName, toolTip, styleTabs) {
    var button = this.SmallButton(name, groupName, captionText, imageName, toolTip, null, styleTabs || "stiDesignerStandartTab");

    if (!imageName) {
        button.innerTable.style.margin = "6px 1px 0px 1px";

        var footer = button.footer = document.createElement("div");
        footer.style.margin = "1px 9px 3px 9px";
        footer.style.height = "3px";
        button.appendChild(footer);
    }
    else {
        button.caption.style.padding = "6px 10px 0 0";
        button.caption.style.textAlign = "center";
        button.imageCell.style.padding = "0 6px 0 6px";
        button.style.minWidth = "75px";
        button.style.height = "26px";
        button.innerTable.style.width = "100%";

        var footer = button.footer = document.createElement("div");
        footer.style.margin = "1px 0 3px 0";
        footer.style.height = "3px";
        button.caption.appendChild(footer);
    }

    button.setSelected = function (state) {
        if (this.groupName && state) {
            for (var name in this.jsObject.options.buttons) {
                if (this.groupName == this.jsObject.options.buttons[name].groupName) {
                    this.jsObject.options.buttons[name].setSelected(false);
                }
            }
        }

        this.isSelected = state;
        this.className = state ? this.selectedClass : (this.isEnabled ? (this.isOver ? this.overClass : this.defaultClass) : this.disabledClass);
        this.footer.className = state ? "stiDesignerStandartTabFooter" : "";
    }

    return button;
}