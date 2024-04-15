
StiMobileDesigner.prototype.InitializeLayoutPanel = function () {
    var layoutPanel = this.ChildWorkPanel("layoutPanel", "stiDesignerLayoutPanel");
    layoutPanel.style.display = "none";

    layoutPanel.mainTable = this.CreateHTMLTable();
    layoutPanel.mainTable.addCell(this.LayoutPanelArrangeBlock());
    layoutPanel.mainTable.addCell(this.GroupBlockSeparator());
    layoutPanel.mainTable.addCell(this.LayoutPanelDesignBlock());
    layoutPanel.mainTable.addCell(this.GroupBlockSeparator());
    layoutPanel.appendChild(layoutPanel.mainTable);

    layoutPanel.updateControls = function () {
        var currentObject = this.jsObject.options.selectedObject || this.jsObject.GetCommonObject(this.jsObject.options.selectedObjects);

        var enableArrangeButtons = currentObject && currentObject.typeComponent != "StiPage" && currentObject.typeComponent != "StiReport";
        var arrangeButtons = ["alignToGrid", "alignLayout", "bringToFront", "sendToBack", "moveForward", "moveBackward",
            "layoutSize", "link", "lock"]
        for (var i = 0; i < arrangeButtons.length; i++) {
            var button = this.jsObject.options.buttons[arrangeButtons[i]];
            if (button) button.setEnabled(enableArrangeButtons);
        }

        if (currentObject) {
            this.jsObject.options.buttons.lock.setSelected(currentObject.properties.locked === true);
            this.jsObject.options.buttons.link.setSelected(currentObject.properties.linked === true);
            this.jsObject.options.buttons.layoutSize.setEnabled(this.jsObject.options.selectedObjects && this.jsObject.options.selectedObjects.length > 0);
        }
    }
}

//Arrange
StiMobileDesigner.prototype.LayoutPanelArrangeBlock = function () {
    var arrangeGroupBlock = this.GroupBlock("groupBlockArrange", this.loc.Toolbars.ToolbarArrange, false, null);
    var innerTable = this.GroupBlockInnerTable();
    arrangeGroupBlock.container.appendChild(innerTable);
    innerTable.style.height = "100%";

    //Align To Grid 
    var alignToGridButton = this.StandartBigButton("alignToGrid", null, this.loc.Toolbars.AlignToGrid, "Layout.BigAlignToGrid.png",
        [this.loc.HelpDesigner.AlignToGrid, this.HelpLinks["layout"]], 60);
    innerTable.addCell(alignToGridButton).style.padding = "2px";
    alignToGridButton.action = function () {
        this.jsObject.SendCommandAlignToGridComponents();
    }

    //Separator
    var sep1 = this.HomePanelSeparator();
    sep1.style.height = this.options.isTouchDevice ? "90px" : "70px";
    innerTable.addCell(sep1);

    //Align
    var alignButton = this.BigButton("alignLayout", null, this.loc.Toolbars.Align, "Layout.BigAlignToGrid.png",
        [this.loc.HelpDesigner.Align, this.HelpLinks["layout"]], true, "stiDesignerStandartBigButton", null, 80);
    alignButton.cellImage.style.height = "40px";
    innerTable.addCell(alignButton).style.padding = "2px";
    var alignMenu = this.LayoutAlignMenu();

    alignButton.action = function () {
        alignMenu.changeVisibleState(!alignMenu.visible);
    }

    //Separator
    var sep2 = this.HomePanelSeparator();
    sep2.style.height = this.options.isTouchDevice ? "90px" : "70px";
    innerTable.addCell(sep2);

    //Bring To Front
    var bringToFrontButton = this.StandartBigButton("bringToFront", null, this.loc.Toolbars.BringToFront, "Layout.BigBringToFront.png",
        [this.loc.HelpDesigner.BringToFront, this.HelpLinks["layout"]], 60);
    innerTable.addCell(bringToFrontButton).style.padding = "2px";
    bringToFrontButton.action = function () {
        this.jsObject.SendCommandChangeArrangeComponents("BringToFront");
    }

    //Send To Back
    var sendToBackButton = this.StandartBigButton("sendToBack", null, this.loc.Toolbars.SendToBack, "Layout.BigSendToBack.png",
        [this.loc.HelpDesigner.SendToBack, this.HelpLinks["layout"]], 60);
    innerTable.addCell(sendToBackButton).style.padding = "2px";
    sendToBackButton.action = function () {
        this.jsObject.SendCommandChangeArrangeComponents("SendToBack");
    }

    //Move Forward
    var moveForwardButton = this.StandartBigButton("moveForward", null, this.loc.Toolbars.MoveForward, "Layout.BigMoveForward.png",
        [this.loc.HelpDesigner.MoveForward, this.HelpLinks["layout"]], 60);
    innerTable.addCell(moveForwardButton).style.padding = "2px";
    moveForwardButton.action = function () {
        this.jsObject.SendCommandChangeArrangeComponents("MoveForward");
    }

    //Move Backward
    var moveBackwardButton = this.StandartBigButton("moveBackward", null, this.loc.Toolbars.MoveBackward, "Layout.BigMoveBackward.png",
        [this.loc.HelpDesigner.MoveBackward, this.HelpLinks["layout"]], 60);
    innerTable.addCell(moveBackwardButton).style.padding = "2px";
    moveBackwardButton.action = function () {
        this.jsObject.SendCommandChangeArrangeComponents("MoveBackward");
    }

    return arrangeGroupBlock;
}

//Design
StiMobileDesigner.prototype.LayoutPanelDesignBlock = function () {
    var designGroupBlock = this.GroupBlock("groupBlockLayoutDesign", this.loc.PropertyCategory.DesignCategory, false, null);
    var innerTable = this.CreateHTMLTable();
    designGroupBlock.container.appendChild(innerTable);
    innerTable.style.height = "100%";

    //Size
    var sizeCell = innerTable.addCell();
    sizeCell.setAttribute("rowspan", "2");
    var sizeButton = this.BigButton("layoutSize", null, this.loc.PropertyMain.Size, "Layout.BigSize.png",
        [this.loc.HelpDesigner.ComponentSize, this.HelpLinks["layout"]], true, "stiDesignerStandartBigButton", null, 60);

    sizeCell.appendChild(sizeButton);
    sizeCell.style.padding = "1px 1px 0 1px";

    sizeButton.action = function () {
        var layoutSizeMenu = this.jsObject.options.menus.layoutSizeMenu || this.jsObject.LayoutSizeMenu();
        layoutSizeMenu.changeVisibleState(!layoutSizeMenu.visible);
    }

    //Lock
    var lockButton = this.StandartSmallButton("lock", null, this.loc.Toolbars.Lock, "Layout.Lock.png",
        [this.loc.HelpDesigner.Lock, this.HelpLinks["layout"]], null);
    innerTable.addCell(lockButton).style.padding = "1px 1px 0 1px";
    lockButton.action = function () {
        this.setSelected(!this.isSelected);
        this.jsObject.ApplyPropertyValue("locked", this.isSelected);
    }

    //Link
    var linkButton = this.StandartSmallButton("link", null, this.loc.Toolbars.Link, "Layout.Link.png",
        [this.loc.HelpDesigner.Link, this.HelpLinks["layout"]], null);
    innerTable.addCellInNextRow(linkButton).style.padding = "0 1px 0 1px";
    linkButton.action = function () {
        this.setSelected(!this.isSelected);
        this.jsObject.ApplyPropertyValue("linked", this.isSelected);
    }

    return designGroupBlock;
}