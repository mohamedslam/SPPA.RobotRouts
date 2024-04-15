
StiMobileDesigner.prototype.InitializeComponentContextMenu = function () {
    var jsObject = this;
    var menu = this.BaseContextMenu("componentContextMenu", "Down", null);
    var menuContainer = menu.innerContent;
    menuContainer.className = "stiDesignerOpacityMenu";
    menu.controls = {};
    menuContainer.style.overflowY = "visible";
    menuContainer.style.overflowX = "visible";

    var buttons = this.options.buttons;
    var controls = this.options.controls;
    var currentObject = this.options.selectedObject || this.GetCommonObject(this.options.selectedObjects);
    if (!currentObject) return;

    var upTable = this.CreateHTMLTable();
    upTable.className = "stiDesignerInnerContainerOpacityMenu";
    menuContainer.appendChild(upTable);

    var showDesignButton = controls.propertiesDesignButtonBlock.style.display == "";
    var buttonProps = [];
    if (currentObject.typeComponent == "StiPage" || controls.propertiesDesignButtonBlock.style.display == "") buttonProps.push(["design", "Design.png", this.loc.Buttons.Design, "2px 1px 2px 2px"]);
    if (currentObject.typeComponent == "StiPage" || controls.propertiesDesignButtonBlock.style.display == "") buttonProps.push(["separator0"]);
    buttonProps.push(["copy", "Copy.png", this.loc.MainMenu.menuEditCopy.replace("&", ""), "2px 1px 2px 1px"]);
    buttonProps.push(["cut", "Cut.png", this.loc.MainMenu.menuEditCut.replace("&", ""), "2px 1px 2px 1px"]);
    buttonProps.push(["paste", "PasteSmall.png", this.loc.MainMenu.menuEditPaste.replace("&", ""), "2px 1px 2px 1px"]);
    buttonProps.push(["remove", "Remove.png", this.loc.MainMenu.menuEditDelete.replace("&", ""), "2px 1px 2px 1px"]);
    buttonProps.push(["selectAll", "ContextMenu.SelectAll.png", this.loc.MainMenu.menuEditSelectAll.replace("&", ""), "2px 1px 2px 1px"]);
    buttonProps.push(["separator1"]);
    buttonProps.push(["properties", "Properties.png", this.loc.Panels.Properties, "2px 1px 2px 2px"]);

    for (var i = 0; i < buttonProps.length; i++) {
        var control = buttonProps[i][0].indexOf("separator") >= 0 ? this.HomePanelSeparator() : this.StandartSmallButton(null, null, null, buttonProps[i][1], buttonProps[i][2]);
        control.name = buttonProps[i][0];
        menu.controls[control.name] = control;
        upTable.addCell(control);
        if (buttonProps[i][0].indexOf("separator") >= 0) continue;
        control.style.margin = buttonProps[i][3];

        control.action = function () {
            menu.action(this);
        }
    }

    menu.controls.copy.setEnabled(buttons.copyComponent.isEnabled);
    menu.controls.cut.setEnabled(buttons.cutComponent.isEnabled);
    menu.controls.paste.setEnabled(buttons.pasteComponent.isEnabled);
    menu.controls.remove.setEnabled(buttons.removeComponent.isEnabled);

    var downTable = this.CreateHTMLTable();
    downTable.className = "stiDesignerInnerContainerOpacityMenu";
    downTable.style.marginTop = "1px";
    downTable.style.width = "100%";
    menuContainer.appendChild(downTable);

    //Add Menu Items
    var selectedObjects = this.options.selectedObject ? [this.options.selectedObject] : this.options.selectedObjects;
    var itemProps = [];

    if (this.IsTableCell(selectedObjects)) {
        itemProps.push(["table", this.loc.PropertyMain.Table, "ContextMenu.ChangeTable.png", "tableContextSubMenu"]);
        itemProps.push(["separator1_0"]);
    }

    if (currentObject.typeComponent != "StiPage" && currentObject.typeComponent != "StiReport" && !this.HaveTableCell(selectedObjects)) {
        itemProps.push(["align", this.loc.Toolbars.Align, null, "alignContextSubMenu"]);
        itemProps.push(["order", this.loc.Toolbars.Order, null, "orderContextSubMenu"]);
        itemProps.push(["separator1_1"]);
    }

    if (currentObject.typeComponent == "StiPage") {
        itemProps.push([!currentObject.isDashboard ? "addPage" : "addDashboard",
        !currentObject.isDashboard ? this.loc.MainMenu.menuFilePageNew : this.loc.MainMenu.menuFileDashboardNew,
        !currentObject.isDashboard ? "AddPage.png" : "AddDashboard.png"]);
        itemProps.push(["removePage", !currentObject.isDashboard ? this.loc.MainMenu.menuFilePageDelete : this.loc.MainMenu.menuFileDashboardDelete,
            !currentObject.isDashboard ? "RemovePage.png" : "Remove.png"]);
        itemProps.push(["separator2"]);
        itemProps.push(["renamePage", this.loc.Buttons.Rename, null]);
        itemProps.push(["separator3"]);
    }

    if (currentObject.properties.interaction) {
        itemProps.push(["interaction", this.loc.PropertyMain.Interaction, "Interaction.png"]);
        itemProps.push(["separatorInteraction"]);
    }

    for (var i = 0; i < itemProps.length; i++) {
        if (itemProps[i][0].indexOf("separator") >= 0) {
            downTable.addCellInNextRow(this.VerticalMenuSeparator()).style.padding = "0 1px 0 1px";
            continue;
        }

        var control = this.StandartSmallButton(null, null, itemProps[i][1], itemProps[i][2]);
        if (control.imageCell) control.imageCell.style.width = "20px";
        if (control.caption) control.caption.style.padding = control.imageCell ? "0px 15px 0px 2px" : "0px 15px 0px 28px";

        control.style.height = "24px";
        control.name = itemProps[i][0];
        menu.controls[control.name] = control;
        if (i == 0)
            downTable.addCell(control).style.padding = "1px 1px 0 1px";
        else
            downTable.addCellInNextRow(control).style.padding = "0px 1px 0 1px";

        if (itemProps[i].length > 3) {
            control.innerTable.addCell().style.width = "100%";
            var arrowImg = document.createElement("img");
            arrowImg.style.margin = "0 4px 0 4px";
            arrowImg.style.width = arrowImg.style.height = "8px";
            StiMobileDesigner.setImageSource(arrowImg, this.options, "Arrows.SmallArrowRight.png");
            control.innerTable.addCell(arrowImg);

            switch (itemProps[i][3]) {
                case "tableContextSubMenu": { this.InitializeTableContextSubMenu(control, menu); break; }
                case "alignContextSubMenu": { this.InitializeAlignContextSubMenu(control, menu); break; }
                case "orderContextSubMenu": { this.InitializeOrderContextSubMenu(control, menu); break; }
            }
        }
        else {
            control.action = function () {
                menu.action(this);
            }
        }
    }

    //Add Menu Checkbox Items
    var itemCheckboxes = [];
    if (currentObject.properties.allowHtmlTags != null) itemCheckboxes.push(this.CheckBoxMenuItem("allowHtmlTags", this.loc.PropertyMain.AllowHtmlTags));
    if (currentObject.properties.autoWidth != null) itemCheckboxes.push(this.CheckBoxMenuItem("autoWidth", this.loc.PropertyMain.AutoWidth));
    if (currentObject.properties.calcInvisible != null) itemCheckboxes.push(this.CheckBoxMenuItem("calcInvisible", this.loc.PropertyMain.CalcInvisible));
    if (currentObject.properties.canBreak != null) itemCheckboxes.push(this.CheckBoxMenuItem("canBreak", this.loc.PropertyMain.CanBreak));
    if (currentObject.properties.canGrow != null) itemCheckboxes.push(this.CheckBoxMenuItem("canGrow", this.loc.PropertyMain.CanGrow));
    if (currentObject.properties.canShrink != null) itemCheckboxes.push(this.CheckBoxMenuItem("canShrink", this.loc.PropertyMain.CanShrink));
    if (currentObject.properties.editableText != null) itemCheckboxes.push(this.CheckBoxMenuItem("editableText", this.loc.PropertyMain.Editable));
    if (currentObject.properties.enabled != null) itemCheckboxes.push(this.CheckBoxMenuItem("enabled", this.loc.PropertyMain.Enabled));
    if (currentObject.properties.growToHeight != null) itemCheckboxes.push(this.CheckBoxMenuItem("growToHeight", this.loc.PropertyMain.GrowToHeight));
    if (currentObject.properties.hideZeros != null) itemCheckboxes.push(this.CheckBoxMenuItem("hideZeros", this.loc.PropertyMain.HideZeros));
    if (currentObject.properties.keepGroupHeaderTogether != null) itemCheckboxes.push(this.CheckBoxMenuItem("keepGroupHeaderTogether", this.loc.PropertyMain.KeepGroupHeaderTogether));
    if (currentObject.properties.keepGroupFooterTogether != null) itemCheckboxes.push(this.CheckBoxMenuItem("keepGroupFooterTogether", this.loc.PropertyMain.KeepGroupFooterTogether));
    if (currentObject.properties.keepGroupTogether != null) itemCheckboxes.push(this.CheckBoxMenuItem("keepGroupTogether", this.loc.PropertyMain.KeepGroupTogether));
    if (currentObject.properties.keepHeaderTogether != null) itemCheckboxes.push(this.CheckBoxMenuItem("keepHeaderTogether", this.loc.PropertyMain.KeepHeaderTogether));
    if (currentObject.properties.keepFooterTogether != null) itemCheckboxes.push(this.CheckBoxMenuItem("keepFooterTogether", this.loc.PropertyMain.KeepFooterTogether));
    if (currentObject.properties.keepDetailsTogether != null) itemCheckboxes.push(this.CheckBoxMenuItem("keepDetailsTogether", this.loc.PropertyMain.KeepDetailsTogether));
    if (currentObject.properties.keepReportSummaryTogether != null) itemCheckboxes.push(this.CheckBoxMenuItem("keepReportSummaryTogether", this.loc.PropertyMain.KeepReportSummaryTogether));
    if (currentObject.properties.keepSubReportTogether != null) itemCheckboxes.push(this.CheckBoxMenuItem("keepSubReportTogether", this.loc.PropertyMain.KeepSubReportTogether));
    if (currentObject.properties.keepCrossTabTogether != null) itemCheckboxes.push(this.CheckBoxMenuItem("keepCrossTabTogether", this.loc.PropertyMain.KeepCrossTabTogether));
    if (currentObject.properties.onlyText != null) itemCheckboxes.push(this.CheckBoxMenuItem("onlyText", this.loc.PropertyMain.OnlyText));
    if (currentObject.properties.printable != null) itemCheckboxes.push(this.CheckBoxMenuItem("printable", this.loc.PropertyMain.Printable));
    if (currentObject.properties.printAtBottom != null) itemCheckboxes.push(this.CheckBoxMenuItem("printAtBottom", this.loc.PropertyMain.PrintAtBottom));
    if (currentObject.properties.printOnAllPages != null) itemCheckboxes.push(this.CheckBoxMenuItem("printOnAllPages", this.loc.PropertyMain.PrintOnAllPages));
    if (currentObject.properties.printIfEmpty != null) itemCheckboxes.push(this.CheckBoxMenuItem("printIfEmpty", this.loc.PropertyMain.PrintIfEmpty));
    if (currentObject.properties.printIfDetailEmpty != null) itemCheckboxes.push(this.CheckBoxMenuItem("printIfDetailEmpty", this.loc.PropertyMain.PrintIfDetailEmpty));
    if (currentObject.properties.printHeadersFootersFromPreviousPage != null) itemCheckboxes.push(this.CheckBoxMenuItem("printHeadersFootersFromPreviousPage", this.loc.PropertyMain.PrintHeadersFootersFromPreviousPage));
    if (currentObject.properties.printOnPreviousPage != null) itemCheckboxes.push(this.CheckBoxMenuItem("printOnPreviousPage", this.loc.PropertyMain.PrintOnPreviousPage));
    if (currentObject.properties.resetPageNumber != null) itemCheckboxes.push(this.CheckBoxMenuItem("resetPageNumber", this.loc.PropertyMain.ResetPageNumber));
    if (currentObject.properties.titleBeforeHeader != null) itemCheckboxes.push(this.CheckBoxMenuItem("titleBeforeHeader", this.loc.PropertyMain.TitleBeforeHeader));
    if (currentObject.properties.wordWrap != null) itemCheckboxes.push(this.CheckBoxMenuItem("wordWrap", this.loc.PropertyMain.WordWrap));

    for (var i = 0; i < itemCheckboxes.length; i++) {
        if (itemCheckboxes[i] == "separator") {
            downTable.addCellInNextRow(this.VerticalMenuSeparator()).style.padding = "0 1px 0 1px";
            continue;
        }
        var control = itemCheckboxes[i];
        menu.controls[control.name] = control;
        downTable.addCellInNextRow(control).style.padding = i == itemCheckboxes.length - 1 ? "0 1px 1px 1px" : "0 1px 0 1px";
        control.setChecked(currentObject.properties[control.name]);

        control.action = function () {
            jsObject.ApplyPropertyValue(this.name, this.isChecked);
        }
    }

    if (itemProps.length == 0 && itemCheckboxes.length == 0) downTable.style.display = "none";

    if (currentObject.typeComponent == "StiPage") {
        var pagesCount = this.options.paintPanel.getPagesCount();
        menu.controls["removePage"].setEnabled(pagesCount > 1);
    }

    menu.action = function (menuItem) {
        var currentObject = jsObject.options.selectedObject || jsObject.GetCommonObject(jsObject.options.selectedObjects);
        var buttons = jsObject.options.buttons;
        switch (menuItem.name) {
            case "design":
                {
                    if (!currentObject) break;
                    if (currentObject.typeComponent == "StiPage") {
                        if (currentObject.isDashboard) {
                            jsObject.InitializeDashboardSetupForm(function (form) {
                                form.changeVisibleState(true);
                            });
                        }
                        else {
                            jsObject.InitializePageSetupForm(function (form) {
                                form.changeVisibleState(true);
                            });
                        }
                    }
                    else
                        jsObject.ShowComponentForm(currentObject);
                    break;
                }
            case "copy":
                {
                    buttons.copyComponent.action();
                    break;
                }
            case "cut":
                {
                    buttons.cutComponent.action();
                    break;
                }
            case "paste":
                {
                    buttons.pasteComponent.action();
                    break;
                }
            case "remove":
                {
                    buttons.removeComponent.action();
                    break;
                }
            case "selectAll":
                {
                    jsObject.SelectAllComponentsOnPage(jsObject.options.currentPage);
                    break;
                }
            case "properties":
                {
                    if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.showContainer("Properties");
                    break;
                }
            case "interaction":
                {
                    jsObject.InitializeInteractionForm(function (interactionForm) {
                        interactionForm.show();
                    });
                    break;
                }
            case "addPage":
            case "addDashboard":
            case "removePage":
                {
                    jsObject.ExecuteAction(menuItem.name);
                    break;
                }
            case "renamePage":
                {
                    var pageIndex = jsObject.StrToInt(jsObject.options.currentPage.properties.pageIndex);
                    var pageButton = jsObject.options.pagesPanel.pagesContainer.pages[pageIndex];
                    if (pageButton) pageButton.setEditMode(true);
                    break;
                }
            case "convertToText":
            case "convertToImage":
            case "convertToCheckBox":
            case "convertToRichText":
                {
                    jsObject.SendCommandChangeTableComponent({ command: "convertTo", cellType: menuItem.name.replace("convertTo", "") });
                    break;
                }
            case "joinCells":
            case "insertColumnToLeft":
            case "insertColumnToRight":
            case "deleteColumn":
            case "selectColumn":
            case "insertRowAbove":
            case "insertRowBelow":
            case "deleteRow":
            case "selectRow":
                {
                    var command = menuItem.name;
                    if (command == "joinCells" && menuItem.isSelected) command = "unJoinCells";
                    jsObject.SendCommandChangeTableComponent({ command: command });
                    break;
                }
            case "BringToFront":
            case "SendToBack":
            case "MoveForward":
            case "MoveBackward":
            case "AlignLeft":
            case "AlignCenter":
            case "AlignRight":
            case "AlignTop":
            case "AlignMiddle":
            case "AlignBottom":
            case "MakeHorizontalSpacingEqual":
            case "MakeVerticalSpacingEqual":
            case "CenterHorizontally":
            case "CenterVertically":
                {
                    jsObject.SendCommandChangeArrangeComponents(menuItem.name);
                    break;
                }
            case "AlignToGrid":
                {
                    jsObject.SendCommandAlignToGridComponents();
                    break;
                }
        }

        menu.changeVisibleState(false);
    }

    return menu;
}

StiMobileDesigner.prototype.CheckBoxMenuItem = function (name, caption) {
    var checkBox = this.CheckBox(null, caption);
    checkBox.className = "stiDesignerCheckBoxMenuItem";
    checkBox.name = name;
    checkBox.style.width = "100%";
    checkBox.style.border = 0;
    checkBox.style.margin = 0;

    var imageBlock = checkBox.imageBlock;
    imageBlock.className = "stiDesignerCheckBoxMenuItemImageBlock";
    imageBlock.style.margin = "2px";
    imageBlock.style.width = checkBox.imageBlock.style.height = "18px";
    imageBlock.parentElement.style.width = "1px";
    imageBlock.parentElement.style.width = "1px";

    checkBox.captionCell.style.paddingRight = "15px";
    checkBox.image.parentElement.style.textAlign = "center";
    checkBox.image.parentElement.style.verticalAlign = "middle";

    checkBox.setChecked = function (state) {
        this.image.style.visibility = (state) ? "visible" : "hidden";
        imageBlock.className = (state) ? "stiDesignerCheckBoxMenuItemImageBlockChecked" : "stiDesignerCheckBoxMenuItemImageBlock";
        this.isChecked = state;
    }

    checkBox.onmouseenter = function () {
        if (this.jsObject.options.isTouchDevice || !this.isEnabled) return;
        this.className = "stiDesignerCheckBoxMenuItem stiDesignerCheckBoxMenuItemOver";
    }

    checkBox.onmouseleave = function () {
        if (this.jsObject.options.isTouchDevice || !this.isEnabled) return;
        this.className = "stiDesignerCheckBoxMenuItem";
    }

    return checkBox;
}

StiMobileDesigner.prototype.InitializeTableContextSubMenu = function (parentControl, parentMenu) {
    var menu = this.InitializeSubMenu("tableContextSubMenu", this.GetTableContextSubMenuItems(), parentControl, parentMenu);

    menu.onshow = function () {
        var selectedObjects = this.jsObject.options.selectedObject ? [this.jsObject.options.selectedObject] : this.jsObject.options.selectedObjects;
        var merged = false;
        if (selectedObjects) {
            for (var i = 0; i < selectedObjects.length; i++) {
                if (selectedObjects[i].properties.merged) merged = true;
            }
        }
        menu.items.joinCells.setEnabled(merged || selectedObjects.length > 1);
        if (menu.items.joinCells.isEnabled) menu.items.joinCells.setSelected(merged);
    }

    return menu;
}

StiMobileDesigner.prototype.InitializeAlignContextSubMenu = function (parentControl, parentMenu) {
    var menu = this.InitializeSubMenu("alignContextSubMenu", this.GetLayoutAlignItems(true), parentControl, parentMenu);

    menu.onshow = function () {
        var selectedObjects = this.jsObject.options.selectedObject ? [this.jsObject.options.selectedObject] : this.jsObject.options.selectedObjects;
        var itemNames = ["AlignLeft", "AlignCenter", "AlignRight", "AlignTop", "AlignMiddle", "AlignBottom", "MakeHorizontalSpacingEqual", "MakeVerticalSpacingEqual"];
        for (var i = 0; i < itemNames.length; i++)
            this.items[itemNames[i]].setEnabled(selectedObjects && selectedObjects.length > 1);
    }

    return menu;
}

StiMobileDesigner.prototype.InitializeOrderContextSubMenu = function (parentControl, parentMenu) {
    var menu = this.InitializeSubMenu("orderContextSubMenu", this.GetOrderContextSubMenuItems(), parentControl, parentMenu);

    return menu;
}