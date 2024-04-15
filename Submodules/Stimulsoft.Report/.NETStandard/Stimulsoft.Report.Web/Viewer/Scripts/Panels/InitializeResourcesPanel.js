
StiJsViewer.prototype.InitializeResourcesPanel = function () {
    var resourcesPanel = document.createElement("div");
    this.controls.resourcesPanel = resourcesPanel;
    this.controls.mainPanel.appendChild(resourcesPanel);
    resourcesPanel.style.display = "none";
    resourcesPanel.style.zIndex = "3";
    resourcesPanel.visible = false;
    resourcesPanel.style.fontFamily = this.options.toolbar.fontFamily;
    if (this.options.toolbar.fontColor != "") resourcesPanel.style.color = this.options.toolbar.fontColor;
    resourcesPanel.id = this.controls.viewer.id + "_ResourcesPanel";
    resourcesPanel.className = "stiJsViewerToolBar";
    if (this.options.toolbar.displayMode == "Separated") resourcesPanel.className += " stiJsViewerToolBarSeparated";
    resourcesPanel.jsObject = this;

    var innerPanel = document.createElement("div");
    resourcesPanel.appendChild(innerPanel);
    if (this.options.toolbar.displayMode == "Simple") innerPanel.style.paddingTop = "2px";

    var innerBlock = document.createElement("div");
    innerPanel.appendChild(innerBlock);
    innerBlock.className = "stiJsViewerToolBarTable";
    if (this.options.toolbar.displayMode == "Separated") innerBlock.style.border = "0px";
    innerBlock.style.boxSizing = "border-box";
    innerBlock.style.display = "table";

    resourcesPanel.changeVisibleState = function (state) {
        var isStateChanged = this.visible != state;
        this.style.display = state ? "" : "none";
        this.visible = state;

        if (this.jsObject.options.toolbar.visible && this.jsObject.options.toolbar.showResourcesButton) this.jsObject.controls.toolbar.controls.Resources.setSelected(state);

        if (isStateChanged) this.jsObject.updateLayout();
    }

    resourcesPanel.update = function () {
        this.clearResources();

        if (this.jsObject.options.toolbar.visible && this.jsObject.options.toolbar.showResourcesButton) {
            this.jsObject.controls.toolbar.controls.Resources.setEnabled(this.jsObject.reportParams.resources != null);
        }

        if (this.jsObject.reportParams.resources) {
            var innerTable = this.jsObject.CreateHTMLTable();
            innerBlock.appendChild(innerTable);

            for (var i = 0; i < this.jsObject.reportParams.resources.length; i++) {
                var resource = this.jsObject.reportParams.resources[i];
                var captionText = resource.name + "<br>" + this.jsObject.GetHumanFileSize(resource.size, 1);
                var button = this.jsObject.ResourceButton(resource.name, captionText, this.jsObject.GetResourceImage(resource.type), resource.type);
                button.caption.innerHTML = captionText;
                innerTable.addCell(button);
            }
        }

        this.changeVisibleState(this.jsObject.reportParams.resources != null);
    }

    resourcesPanel.clearResources = function () {
        while (innerBlock.childNodes[0]) {
            innerBlock.removeChild(innerBlock.childNodes[0]);
        }
    }
}

StiJsViewer.prototype.ResourceButton = function (name, captionText, imageName, resourceType) {
    var button = this.SmallButton(name, captionText, imageName, this.collections.loc["ButtonView"] + " " + name, null, "stiJsViewerSmallButtonWithBorder");
    button.style.height = "auto";
    button.resourceType = resourceType;
    button.style.margin = "3px 0 3px 3px";
    var dropDownButton = this.SmallButton(null, null, "Arrows.SmallArrowDown.png", null, null, null, null, { width: 8, height: 8 });
    dropDownButton.style.height = dropDownButton.style.width = this.options.isTouchDevice ? "23px" : "18px";
    dropDownButton.style.margin = "0 7px 0 3px";
    dropDownButton.innerTable.style.width = "100%";
    dropDownButton.imageCell.style.textAlign = "center";
    if (!this.options.jsMode) button.innerTable.addCell(dropDownButton).style.width = "1px";
    button.innerTable.style.width = "100%";
    button.style.minWidth = "80px";

    if (button.caption) {
        button.caption.style.textAlign = "left";
        button.caption.style.paddinLeft = "3px";
        button.caption.style.maxWidth = "150px";
        button.caption.style.lineHeight = "14px";
        button.caption.style.whiteSpace = "nowrap";
        button.caption.style.overflow = "hidden";
        button.caption.style.textOverflow = "ellipsis";
    }

    if (button.imageCell) {
        button.imageCell.style.width = "1px";
        button.imageCell.style.padding = "4px 8px 4px 4px";
        button.image.style.width = button.image.style.height = "32px";
    }

    var items = [];
    items.push(this.Item("View", this.collections.loc["ButtonView"], "Zoom.png", "View"));
    items.push(this.Item("SaveFile", this.collections.loc["Save"], "Save.png", "SaveFile"));

    var resourceMenu = this.VerticalMenu(name + "_ResourceMenu", dropDownButton, "Down", items);

    resourceMenu.action = function (menuItem) {
        resourceMenu.changeVisibleState(false);
        this.jsObject.postReportResource(button.name, menuItem.key);
    }

    button.action = function () {
        if (!dropDownButton.clicked) {
            var resTypesAllowedViewInBrowser = ["Image", "Pdf", "Txt"];
            var viewType = resTypesAllowedViewInBrowser.indexOf(button.resourceType) >= 0 ? "View" : "SaveFile";
            this.jsObject.postReportResource(button.name, this.jsObject.options.jsMode ? "SaveFile" : viewType);
        }
        dropDownButton.clicked = false;
    }

    button.onmouseup = function (event) {
        if (event.button == 2 && !this.jsObject.options.jsMode) {
            resourceMenu.changeVisibleState(true);
        }
    }

    button.oncontextmenu = function (event) {
        return false;
    }

    dropDownButton.action = function () {
        resourceMenu.changeVisibleState(!resourceMenu.visible);
        resourceMenu.items.View.setEnabled(button.resourceType != "Report");
        dropDownButton.clicked = true;
    }

    return button;
}

StiJsViewer.prototype.GetResourceImage = function (resourceType) {
    if (StiJsViewer.checkImageSource(this.options, this.collections, "BigResource" + resourceType + ".png"))
        return "BigResource" + resourceType + ".png";
    else
        return "BigResource.png";
}
