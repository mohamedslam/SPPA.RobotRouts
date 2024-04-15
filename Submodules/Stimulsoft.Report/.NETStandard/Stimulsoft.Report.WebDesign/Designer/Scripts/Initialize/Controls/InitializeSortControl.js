
StiMobileDesigner.prototype.SortControl = function (name, columns, widthContainer, heightContainer) {
    var sortControl = document.createElement("div");
    sortControl.jsObject = this;

    var toolBar = sortControl.toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "4px";
    sortControl.appendChild(toolBar);

    sortControl.columns = columns;
    sortControl.currentDataSourceName = null;
    sortControl.noSortText = this.loc.FormBand.NoSort;

    var addSortButton = sortControl.addSortButton = this.FormButton(null, null, this.loc.FormBand.AddSort.replace("&", ""));
    toolBar.addCell(addSortButton);

    addSortButton.action = function () {
        this.sortContainer.addSort({ "column": sortControl.noSortText, "direction": "ASC" });
        this.sortContainer.onAction();
    }

    var moveUpButton = sortControl.moveUpButton = this.StandartSmallButton(null, null, null, "Arrows.ArrowUpBlue.png", null, null);
    var moveDownButton = sortControl.moveDownButton = this.StandartSmallButton(null, null, null, "Arrows.ArrowDownBlue.png", null, null);

    moveUpButton.style.marginLeft = moveDownButton.style.marginLeft = "6px";

    moveUpButton.setEnabled(false);
    moveDownButton.setEnabled(false);

    toolBar.addCell(moveUpButton);
    toolBar.addCell(moveDownButton);

    moveUpButton.action = function () {
        if (sortControl.sortContainer.selectedItem) { sortControl.sortContainer.selectedItem.move("Up"); }
    }

    moveDownButton.action = function () {
        if (sortControl.sortContainer.selectedItem) { sortControl.sortContainer.selectedItem.move("Down"); }
    }

    if (!this.options.isTouchDevice) {
        moveUpButton.style.display = moveDownButton.style.display = "none";
    }

    //Container
    var sortContainer = sortControl.sortContainer = this.SortContainer(sortControl);
    sortControl.appendChild(sortContainer);
    addSortButton.sortContainer = sortContainer;

    if (widthContainer) sortContainer.style.width = widthContainer + "px";
    if (heightContainer) sortContainer.style.height = heightContainer + "px";

    sortControl.fill = function (sorts) {
        sortContainer.clear();
        if (!sorts) return;
        for (var i = 0; i < sorts.length; i++) {
            sortContainer.addSort(sorts[i], true);
        }
        sortContainer.onAction();
    }

    sortControl.getValue = function () {
        var result = [];
        for (var num = 0; num < sortContainer.childNodes.length; num++) {
            var item = sortContainer.childNodes[num];
            if (item.key && item.column.textBox.value != sortControl.noSortText) {
                var columnKey = item.column.key || item.column.textBox.value;
                result.push({ "direction": item.direction.key, "column": columnKey });
            }
        }
        return result;
    }

    return sortControl;
}

StiMobileDesigner.prototype.SortContainer = function (sortControl) {
    var sortContainer = sortControl.sortContainer = document.createElement("div");
    var jsObject = sortContainer.jsObject = this;
    sortContainer.className = "stiDesignerSortContainer";
    sortContainer.style.position = "relative";
    sortContainer.sortControl = sortControl;
    sortContainer.selectedItem = null;

    sortContainer.addSort = function (sortObject, notAction) {
        var sortItem = jsObject.SortItem(this);
        this.appendChild(sortItem);
        sortItem.direction.setKey(sortObject.direction);

        if (sortControl.columns != null)
            sortItem.column.setKey(sortObject.column);
        else
            sortItem.column.textBox.value = sortObject.column;

        sortItem.setSelected(true);
        if (!notAction) this.onAction();
    }

    sortContainer.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        sortControl.moveUpButton.setEnabled(false);
        sortControl.moveDownButton.setEnabled(false);
    }

    sortContainer.getCountItems = function () {
        var itemsCount = 0;
        for (var i = 0; i < this.childNodes.length; i++) {
            if (this.childNodes[i].key) itemsCount++;
        }
        return itemsCount;
    }

    sortContainer.getOverItemIndex = function () {
        for (var i = 0; i < this.childNodes.length; i++)
            if (this.childNodes[i].isOver) return i;

        return null;
    }

    sortContainer.getItemIndex = function (item) {
        for (var i = 0; i < this.childNodes.length; i++)
            if (this.childNodes[i] == item) return i;

        return null;
    }

    sortContainer.getItemByIndex = function (index) {
        if (index != null && !this.hintText && index >= 0 && index < this.childNodes.length) {
            return this.childNodes[index];
        }

        return null;
    }

    sortContainer.getSelectedItemIndex = function () {
        return this.selectedItem ? this.getItemIndex(this.selectedItem) : null;
    }

    sortContainer.moveItem = function (fromIndex, toIndex) {
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

    sortContainer.onmouseup = function (event) {
        if (event.button != 2 && jsObject.options.itemInDrag) {
            var itemObject = jsObject.CopyObject(jsObject.options.itemInDrag.itemObject);
            if (!itemObject) return;
            var typeItem = itemObject.typeItem;

            if (typeItem == "SortItem") {
                var toIndex = this.getOverItemIndex();
                var fromIndex = this.getSelectedItemIndex();
                if (toIndex != null && fromIndex != null && fromIndex != toIndex) {
                    sortContainer.moveItem(fromIndex, toIndex);
                    sortContainer.onAction();
                }
            }
        }

        return false;
    }

    sortContainer.onAction = function () {
        var count = this.getCountItems();
        var index = this.selectedItem ? this.selectedItem.getIndex() : -1;
        sortControl.moveUpButton.setEnabled(index > 0);
        sortControl.moveDownButton.setEnabled(index != -1 && index < count - 1);

        this.checkEmptyPanel();
    }

    sortContainer.checkEmptyPanel = function () {
        var itemsCount = 0;
        var emptyPanel = null;
        for (var i = 0; i < this.childNodes.length; i++) {
            if (this.childNodes[i].key)
                itemsCount++;
            else
                emptyPanel = this.childNodes[i];
        }
        if (itemsCount > 0) {
            if (emptyPanel) this.removeChild(emptyPanel);
        }
        else {
            if (!emptyPanel) {
                emptyPanel = jsObject.EmptyTextPanel(sortControl.isGroupsControl ? "BigGroups.png" : "BigSort.png", sortControl.isGroupsControl ? jsObject.loc.PropertyMain.NoElements : jsObject.loc.FormBand.NoSort, "0.5", { width: 24, height: 24 });
                this.appendChild(emptyPanel);
            }
        }
    }

    return sortContainer;
}


StiMobileDesigner.prototype.SortItem = function (sortContainer) {
    var sortItem = document.createElement("div");
    var jsObject = sortItem.jsObject = this;
    sortItem.sortContainer = sortContainer;
    sortItem.key = this.generateKey();
    sortItem.isSelected = false;
    sortItem.className = "stiDesignerSortPanel";

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "100%";
    sortItem.appendChild(mainTable);
    var mainCell = mainTable.addCell();

    //Remove Button
    var removeButton = sortItem.removeButton = this.StandartSmallButton(null, null, null, "RemoveBlack.png");
    removeButton.style.visibility = "hidden";
    removeButton.style.display = "inline-block";
    removeButton.style.margin = "2px 2px 2px 0";

    var closeCell = mainTable.addCell(removeButton);
    closeCell.style.width = "1px";
    closeCell.style.verticalAlign = "top";

    removeButton.action = function () {
        sortItem.remove();
    }

    var innerTable = sortItem.innerTable = this.CreateHTMLTable();
    mainCell.appendChild(innerTable);

    var textCell = innerTable.addCell();
    textCell.innerHTML = this.loc.FormBand.SortBy;
    textCell.style.padding = "6px";
    sortItem.textCell = textCell;

    //Column
    if (sortContainer.sortControl.columns != null) {
        sortItem.column = this.DropDownList(null, 180, null, sortContainer.sortControl.columns, true, false);
        sortItem.innerTable.addCell(sortItem.column).style.padding = "6px";
    }
    else {
        sortItem.column = this.ExpressionControl(null, 180, null, null, true);
        sortItem.innerTable.addCell(sortItem.column).style.padding = "6px";

        //FX
        var fxButton = sortItem.column.button;
        StiMobileDesigner.setImageSource(fxButton.image, this.options, "Function.png");
        fxButton.image.style.width = fxButton.image.style.height = "16px";
        fxButton.style.width = this.options.controlsButtonsWidth + "px";
        fxButton.imageCell.style.padding = "0px";
        fxButton.style.borderRadius = fxButton.parentElement.style.borderRadius = "0";
        fxButton.parentElement.style.borderRight = "0";

        fxButton.action = function () {
            jsObject.InitializeExpressionEditorForm(function (form) {
                var propertiesPanel = jsObject.options.propertiesPanel;
                form.propertiesPanelZIndex = propertiesPanel.style.zIndex;
                form.propertiesPanelIsEnabled = propertiesPanel.isEnabled;
                form.resultControl = sortItem.column;
                form.changeVisibleState(true);

                var text = sortItem.column.textBox.value;
                if (text && text.indexOf("{") == 0 && jsObject.EndsWith(text, "}")) {
                    text = text.substring(1, text.length - 1);
                }
                form.expressionTextArea.value = text == sortContainer.sortControl.noSortText ? "" : text;

                form.action = function () {
                    var newText = form.expressionTextArea.value;
                    if (newText.indexOf("{") != 0 && !jsObject.EndsWith(newText, "}")) {
                        newText = "{" + newText + "}";
                    }
                    sortItem.column.textBox.value = newText;
                    form.changeVisibleState(false);
                }
            });
        }

        //Column
        var columnButton = this.StandartSmallButton(null, null, null, "EditButton.png", null, null, null);
        columnButton.imageCell.style.padding = "0px";
        columnButton.style.height = (this.options.controlsHeight - 4) + "px";
        columnButton.style.width = this.options.controlsButtonsWidth + "px";
        columnButton.innerTable.style.width = "100%";

        var baseCellClass = "stiDesignerTextBoxEditButton stiDesignerTextBoxEditButton";
        var cell2 = sortItem.column.addCell(columnButton)
        cell2.className = baseCellClass + "Default";

        if (this.allowRoundedControls()) {
            columnButton.style.borderRadius = cell2.style.borderRadius = "0 3px 3px 0";
        }

        columnButton.onmouseenter = function () {
            if (!this.isEnabled || jsObject.options.isTouchClick) return;
            this.className = this.overClass;
            this.isOver = true;
            cell2.className = baseCellClass + "Over";
        }

        columnButton.onmouseleave = function () {
            this.isOver = false;
            if (!this.isEnabled) return;
            this.className = this.isSelected ? this.selectedClass : this.defaultClass;
            cell2.className = baseCellClass + "Default";
        }

        columnButton.action = function () {
            var currentDataSourceName = (sortContainer.sortControl.currentDataSourceName != null)
                ? sortContainer.sortControl.currentDataSourceName : jsObject.options.selectedObject.properties["dataSource"];
            this.key = currentDataSourceName + "." + sortItem.column.textBox.value;

            jsObject.InitializeDataColumnForm(function (dataColumnForm) {
                dataColumnForm.dataTree.build("Column", currentDataSourceName);
                dataColumnForm.needBuildTree = false;
                dataColumnForm.parentButton = columnButton;
                dataColumnForm.changeVisibleState(true);

                dataColumnForm.action = function () {
                    this.changeVisibleState(false);
                    sortItem.column.textBox.value = this.dataTree.key != ""
                        ? this.dataTree.key.substring(this.dataTree.key.indexOf(".") + 1, this.dataTree.key.length)
                        : sortContainer.sortControl.noSortText;
                }
            });
        }

        sortItem.column.onmouseenter = function () {
            if (!this.isEnabled || jsObject.options.isTouchClick) return;
            this.isOver = true;
            this.textBox.onmouseenter();
            this.buttonCell.className = baseCellClass + "Over";
            cell2.className = baseCellClass + "Over";
        }

        sortItem.column.onmouseleave = function () {
            if (!this.isEnabled) return;
            this.isOver = false;
            this.textBox.onmouseleave();
            this.buttonCell.className = baseCellClass + "Default";
            cell2.className = baseCellClass + "Default";
        }
    }

    //Direction
    sortItem.direction = this.DropDownList(null, 120, null, this.GetSortDirectionItemsForSortForm(), true);
    sortItem.innerTable.addCell(sortItem.direction).style.padding = "6px";

    sortItem.setSelected = function (state) {
        if (state) {
            if (sortContainer.selectedItem) {
                sortContainer.selectedItem.setSelected(false);
            }
            sortContainer.selectedItem = this;
        }
        else {
            if (sortContainer.selectedItem && sortContainer.selectedItem == this) {
                sortContainer.selectedItem = null;
            }
        }
        this.className = state ? "stiDesignerSortPanelSelected" : "stiDesignerSortPanel";
        this.isSelected = state;
        this.removeButton.style.visibility = state ? "visible" : "hidden";
    }

    sortItem.remove = function () {
        if (sortContainer.selectedItem == this) {
            var prevItem = this.previousSibling;
            var nextItem = this.nextSibling;
            sortContainer.selectedItem = null;
            if (sortContainer.childNodes.length > 1) {
                if (nextItem) {
                    nextItem.setSelected(true);
                    sortContainer.selectedItem = nextItem;
                }
                else if (prevItem) {
                    prevItem.setSelected(true);
                    sortContainer.selectedItem = prevItem;
                }
            }
        }
        sortContainer.removeChild(this);
        sortContainer.onAction();
    }

    sortItem.getIndex = function () {
        for (var i = 0; i < sortContainer.childNodes.length; i++)
            if (sortContainer.childNodes[i] == this) return i;
    };

    sortItem.move = function (direction) {
        var index = this.getIndex();
        sortContainer.removeChild(this);
        var count = sortContainer.getCountItems();
        var newIndex = direction == "Up" ? index - 1 : index + 1;
        if (direction == "Up" && newIndex == -1) newIndex = 0;
        if (direction == "Down" && newIndex >= count) {
            sortContainer.appendChild(this);
            sortContainer.onAction();
            return;
        }
        sortContainer.insertBefore(this, sortContainer.childNodes[newIndex]);
        sortContainer.onAction();
    }

    if (jsObject.options.isTouchDevice) {
        sortItem.onclick = function () {
            if (!this.parentElement) return;
            this.setSelected(true);
            sortContainer.onAction();
        }
    }
    else {
        sortItem.onmousedown = function (event) {
            this.setSelected(true);

            if (this.isTouchStartFlag || (event && event.target && event.target.nodeName && event.target.nodeName.toLowerCase() == "input")) return;
            event.preventDefault();

            var options = jsObject.options;
            if (options.controlsIsFocused && options.controlsIsFocused.action) {
                options.controlsIsFocused.blur();
                options.controlsIsFocused = null;
            }

            if (event.button != 2 && !options.controlsIsFocused) {
                var itemInDrag = jsObject.TreeItemForDragDrop({ name: jsObject.loc.PropertyMain.Sort, typeItem: "SortItem" }, null, true);
                if (itemInDrag.button.captionCell) itemInDrag.button.captionCell.style.padding = "5px 20px 5px 10px";
                itemInDrag.beginingOffset = 0;
                options.itemInDrag = itemInDrag;
            }
        }

        sortItem.onmouseover = function () {
            this.isOver = true;
            if (jsObject.options.itemInDrag && jsObject.options.itemInDrag.itemObject) {
                var typeItem = jsObject.options.itemInDrag.itemObject.typeItem;
                if (typeItem == "SortItem") {
                    this.style.borderStyle = "dashed";
                    this.style.borderColor = jsObject.options.themeColors[jsObject.GetThemeColor()];
                }
            }
        }

        sortItem.onmouseout = function () {
            this.isOver = false;
            this.style.borderStyle = "solid";
            this.style.borderColor = "";
        }
    }

    return sortItem;
}