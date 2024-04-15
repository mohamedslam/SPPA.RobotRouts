
StiMobileDesigner.prototype.InitializeWorkPanel = function () {
    var jsObject = this;
    var workPanel = document.createElement("div");
    workPanel.className = "stiDesignerWorkPanel";
    workPanel.jsObject = this;
    this.options.workPanel = workPanel;
    this.options.mainPanel.appendChild(workPanel);
    workPanel.currentPanel = null;
    workPanel.visibleState = true;

    var hideToolbarButton = this.StandartSmallButton("hideToolbarButton", null, null, "Toolbar.HideToolbar.png", null, null);
    hideToolbarButton.style.position = "absolute";
    hideToolbarButton.style.bottom = hideToolbarButton.style.right = "1px";
    workPanel.appendChild(hideToolbarButton);
    if (this.options.showToolbar === false) hideToolbarButton.style.display = "none";

    workPanel.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        var toolBar = jsObject.options.toolBar;
        var paintPanel = jsObject.options.paintPanel;
        var pagesPanel = jsObject.options.pagesPanel;
        var toolbox = jsObject.options.toolbox;
        var propertiesPanel = jsObject.options.propertiesPanel;
        var infoPanel = jsObject.options.infoPanel;
        paintPanel.style.top = toolBar.offsetHeight + pagesPanel.offsetHeight + infoPanel.offsetHeight + this.offsetHeight + "px";
        pagesPanel.style.top = toolBar.offsetHeight + infoPanel.offsetHeight + this.offsetHeight + "px";
        propertiesPanel.style.top = toolBar.offsetHeight + infoPanel.offsetHeight + this.offsetHeight + "px";
        if (toolbox) toolbox.style.top = toolBar.offsetHeight + infoPanel.offsetHeight + this.offsetHeight + "px";
        propertiesPanel.showButtonsPanel.style.top = (toolBar.offsetHeight + workPanel.offsetHeight + infoPanel.offsetHeight + 40) + "px";
        this.jsObject.options.buttons.showToolBarButton.style.display = state ? "none" : "";
        if (state) this.visibleState = true;
    }

    workPanel.showPanel = function (panel) {
        if (this.currentPanel != null && this.currentPanel != panel) {
            this.currentPanel.style.display = "none";
            this.currentPanel.onhide();
        }
        this.currentPanel = panel;
        panel.style.display = "";
        if (this.jsObject.options.paintPanel)
            this.jsObject.options.paintPanel.style.top = this.jsObject.options.toolBar.offsetHeight + this.jsObject.options.pagesPanel.offsetHeight + this.offsetHeight + "px";
        panel.onshow();
    }
}

StiMobileDesigner.prototype.ChildWorkPanel = function (name, className) {
    var childPanel = document.createElement("div");
    childPanel.className = className + (this.options.isTouchDevice ? "_Touch" : "_Mouse");
    childPanel.jsObject = this;
    this.options[name] = childPanel;
    this.options.workPanel.appendChild(childPanel);
    childPanel.style.marginRight = this.options.isTouchDevice ? "31px" : "23px";

    childPanel.onshow = function () { };
    childPanel.onhide = function () { };

    return childPanel;
}

StiMobileDesigner.prototype.DesignPanelInnerTable = function () {
    var table = this.CreateHTMLTable();

    table.addCell = function (control) {
        var cell = document.createElement("td");
        cell.style.verticalAlign = "top";
        this.tr[0].appendChild(cell);
        if (control) cell.appendChild(control);

        return cell;
    }

    return table;
}