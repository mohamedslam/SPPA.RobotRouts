
StiMobileDesigner.prototype.InitializeColorsCollectionForm_ = function () {

    var form = this.BaseForm("colorsCollectionForm", this.loc.PropertyCategory.ColorsCategory, 3);
    form.controls = {};

    //Main Table
    var mainTable = this.CreateHTMLTable();
    form.container.appendChild(mainTable);
    form.container.style.padding = "0px";

    //Toolbar
    var buttons = [
        ["addColor", this.loc.Buttons.Add, null],
        ["separator"],
        ["moveUp", null, "Arrows.ArrowUpBlue.png"],
        ["moveDown", null, "Arrows.ArrowDownBlue.png"]
    ]

    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "12px 12px 0 12px";
    form.container.appendChild(toolBar);

    for (var i = 0; i < buttons.length; i++) {
        if (buttons[i][0].indexOf("separator") >= 0) {
            form.controls[buttons[i][0]] = this.HomePanelSeparator();
            toolBar.addCell(form.controls[buttons[i][0]]).style.paddingRight = "4px";
            continue;
        }
        var buttonStyle = buttons[i][0] == "addColor" ? "stiDesignerFormButton" : "stiDesignerStandartSmallButton";
        var button = form.controls[buttons[i][0]] = this.SmallButton(null, null, buttons[i][1], buttons[i][2], null, null, buttonStyle, true);
        button.style.marginRight = "4px";
        toolBar.addCell(button);
    }

    if (!this.options.isTouchDevice) {
        form.controls.moveUp.style.display = form.controls.moveDown.style.display = form.controls.separator.style.display = "none";
    }

    //Items Container
    var container = form.controls.colorsContainer = this.ColorsCollectionContainer(form);
    container.style.margin = "12px";
    form.container.appendChild(container);

    form.controls.addColor.action = function () {
        var item = container.addColorItem("255,255,255");
        item.setSelected(true);
        container.onAction();
    }

    form.controls.moveUp.action = function () {
        if (container.selectedItem) { container.selectedItem.move("Up"); }
    }

    form.controls.moveDown.action = function () {
        if (container.selectedItem) { container.selectedItem.move("Down"); }
    }

    form.show = function (colors) {
        container.clear();
        for (var i = 0; i < colors.length; i++) {
            container.addColorItem(colors[i], true);
        }
        container.onAction();
        form.changeVisibleState(true);
    }

    form.action = function () {
        form.changeVisibleState(false);
    }

    return form;
}

StiMobileDesigner.prototype.ColorsCollectionContainer = function (form) {
    var container = document.createElement("div");
    var jsObject = this;
    container.className = "stiSimpleContainerWithBorder";
    container.style.overflow = "auto";
    container.style.width = "320px";
    container.style.height = "400px";
    container.selectedItem = null;

    container.addColorItem = function (color, notAction) {
        var colorItem = jsObject.ColorsCollectionItem(container, color);
        this.appendChild(colorItem);
        if (!notAction) container.onAction();
        return colorItem;
    }

    container.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
    }

    container.getCountItems = function () {
        return this.childNodes.length;
    }

    container.getOverItemIndex = function () {
        for (var i = 0; i < this.childNodes.length; i++)
            if (this.childNodes[i].isOver) return i;

        return null;
    }

    container.getItemIndex = function (item) {
        for (var i = 0; i < this.childNodes.length; i++)
            if (this.childNodes[i] == item) return i;

        return null;
    }

    container.getItemByIndex = function (index) {
        if (index != null && !this.hintText && index >= 0 && index < this.childNodes.length) {
            return this.childNodes[index];
        }

        return null;
    }

    container.getSelectedItemIndex = function () {
        return this.selectedItem ? this.getItemIndex(this.selectedItem) : null;
    }

    container.moveItem = function (fromIndex, toIndex) {
        if (fromIndex < this.childNodes.length && toIndex < this.childNodes.length) {
            var fromItem = this.childNodes[fromIndex];
            if (fromIndex < toIndex) {
                if (toIndex < this.childNodes.length - 1) {
                    this.insertBefore(fromItem, this.childNodes[toIndex + 1]);
                }
                else {
                    this.appendChild(fromItem);
                }
            }
            else {
                this.insertBefore(fromItem, this.childNodes[toIndex]);
            }
            return fromItem;
        }
    }

    container.onmouseup = function (event) {
        if (event.button != 2 && jsObject.options.itemInDrag) {
            var itemObject = jsObject.CopyObject(jsObject.options.itemInDrag.itemObject);
            if (!itemObject) return;
            var typeItem = itemObject.typeItem;

            if (typeItem == "ColorItem") {
                var toIndex = this.getOverItemIndex();
                var fromIndex = this.getSelectedItemIndex();
                if (toIndex != null && fromIndex != null && fromIndex != toIndex) {
                    container.moveItem(fromIndex, toIndex);
                    container.onAction();
                }
            }
        }

        return false;
    }

    container.onAction = function () {
        var count = container.getCountItems();
        var index = container.selectedItem ? container.selectedItem.getIndex() : -1;
        form.controls.moveUp.setEnabled(index > 0);
        form.controls.moveDown.setEnabled(index != -1 && index < count - 1);
    }

    return container;
}

StiMobileDesigner.prototype.ColorsCollectionItem = function (container, color) {
    var colorItem = document.createElement("div");
    var jsObject = this;
    colorItem.key = this.generateKey();
    colorItem.isSelected = false;
    colorItem.className = "stiDesignerFilterPanel";
    colorItem.controls = {};

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "100%";
    colorItem.appendChild(mainTable);
    var mainCell = mainTable.addCell();
    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "6px 0 6px 6px";
    mainCell.appendChild(innerTable);

    //Remove Button
    var removeButton = this.StandartSmallButton(null, null, null, "RemoveBlack.png");
    removeButton.style.margin = "2px 2px 2px 0px";
    removeButton.style.visibility = "hidden";
    removeButton.style.display = "inline-block";
    colorItem.removeButton = removeButton;
    var closeCell = mainTable.addCell(removeButton);
    closeCell.style.width = "1px";
    closeCell.style.verticalAlign = "top";

    removeButton.action = function () {
        colorItem.remove();
    }

    var colorControl = jsObject.ColorControl(null, null, null, 240, false);
    colorControl.setKey(color);
    colorItem.controls.colorControl = colorControl;
    innerTable.addCell(colorControl);

    colorControl.button.action = function () {
        colorControl.action();
    };

    colorControl.button.onmouseenter = function () { }
    colorControl.button.onmouseleave = function () { }

    var arrowButton = jsObject.StandartSmallButton(null, null, null, "Arrows.SmallArrowDown.png", null, null, null, { width: 8, height: 8 });
    colorControl.button.innerTable.addCell(arrowButton);
    colorControl.button.arrowCell.style.display = "none";

    arrowButton.style.height = arrowButton.style.width = (this.options.controlsHeight - 2) + "px";
    arrowButton.innerTable.style.width = "100%";

    arrowButton.action = function () {
        var colorDialog = jsObject.options.menus.colorDialog || jsObject.InitializeColorDialog();
        colorDialog.changeVisibleState(!colorDialog.visible, colorControl.button);
    }

    colorItem.setSelected = function (state) {
        if (state) {
            if (container.selectedItem) container.selectedItem.setSelected(false);
            container.selectedItem = this;
        }
        else {
            if (container.selectedItem && container.selectedItem == this) container.selectedItem = null;
        }
        colorItem.isSelected = state;
        colorItem.className = state ? "stiDesignerFilterPanelSelected" : "stiDesignerFilterPanel";
        colorItem.removeButton.style.visibility = state ? "visible" : "hidden";
    }

    colorItem.remove = function () {
        if (container.selectedItem == this) {
            var prevItem = this.previousSibling;
            var nextItem = this.nextSibling;
            container.selectedItem = null;
            if (container.childNodes.length > 1) {
                if (nextItem) {
                    nextItem.setSelected(true);
                    container.selectedItem = nextItem;
                }
                else if (prevItem) {
                    prevItem.setSelected(true);
                    container.selectedItem = prevItem;
                }
            }
        }
        container.removeChild(this);
        container.onAction();
    }

    colorItem.getIndex = function () {
        for (var i = 0; i < container.childNodes.length; i++)
            if (container.childNodes[i] == this) return i;
    };

    colorItem.move = function (direction) {
        var index = this.getIndex();
        container.removeChild(this);
        var count = container.getCountItems();
        var newIndex = direction == "Up" ? index - 1 : index + 1;
        if (direction == "Up" && newIndex == -1) newIndex = 0;
        if (direction == "Down" && newIndex >= count) {
            container.appendChild(this);
            container.onAction();
            return;
        }
        container.insertBefore(this, container.childNodes[newIndex]);
        container.onAction();
    }

    if (jsObject.options.isTouchDevice) {
        colorItem.onclick = function () {
            if (!this.parentElement) return;
            this.setSelected(true);
        }
    }
    else {
        colorItem.onmousedown = function (event) {
            this.setSelected(true);

            if (this.isTouchStartFlag || (event && event.target && event.target.nodeName && event.target.nodeName.toLowerCase() == "input")) return;
            event.preventDefault();

            var options = jsObject.options;
            if (options.controlsIsFocused && options.controlsIsFocused.action) {
                options.controlsIsFocused.blur();
                options.controlsIsFocused = null;
            }

            if (event.button != 2 && !options.controlsIsFocused) {
                var itemInDrag = jsObject.TreeItemForDragDrop({ name: jsObject.loc.PropertyMain.Color, typeItem: "ColorItem" }, null, true);
                if (itemInDrag.button.captionCell) itemInDrag.button.captionCell.style.padding = "5px 20px 5px 10px";
                itemInDrag.beginingOffset = 0;
                options.itemInDrag = itemInDrag;
            }
        }

        colorItem.onmouseover = function () {
            this.isOver = true;
            if (jsObject.options.itemInDrag && jsObject.options.itemInDrag.itemObject) {
                var typeItem = jsObject.options.itemInDrag.itemObject.typeItem;
                if (typeItem == "ColorItem") {
                    this.style.borderStyle = "dashed";
                    this.style.borderColor = jsObject.options.themeColors[jsObject.GetThemeColor()];
                }
            }
        }

        colorItem.onmouseout = function () {
            this.isOver = false;
            this.style.borderStyle = "solid";
            this.style.borderColor = "";
        }
    }

    return colorItem;
}