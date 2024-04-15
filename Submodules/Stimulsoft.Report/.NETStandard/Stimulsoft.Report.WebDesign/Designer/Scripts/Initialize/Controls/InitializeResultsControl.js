
StiMobileDesigner.prototype.ResultsControl = function (name, widthContainer, heightContainer) {
    var control = document.createElement("div");
    control.jsObject = this;

    var toolBar = control.toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "4px";
    control.appendChild(toolBar);

    control.currentDataSourceName = null;
    control.noResultText = this.loc.PropertyEnum.DialogResultNo;

    //Add Result
    var addResultButton = control.addResultButton = this.FormButton(null, null, this.loc.FormBand.AddResult.replace("&", ""));
    toolBar.addCell(addResultButton);

    addResultButton.action = function () {
        var item = control.resultContainer.addResult({ "column": control.noResultText, "aggrFunction": "No", "name": "" });
        control.resultContainer.onAction();
    }

    var moveUpButton = control.moveUpButton = this.StandartSmallButton(null, null, null, "Arrows.ArrowUpBlue.png", null, null);
    var moveDownButton = control.moveDownButton = this.StandartSmallButton(null, null, null, "Arrows.ArrowDownBlue.png", null, null);

    moveUpButton.style.marginLeft = moveDownButton.style.marginLeft = "6px";

    moveUpButton.setEnabled(false);
    moveDownButton.setEnabled(false);

    toolBar.addCell(moveUpButton);
    toolBar.addCell(moveDownButton);

    if (!this.options.isTouchDevice) {
        moveUpButton.style.display = moveDownButton.style.display = "none";
    }

    //Container
    var container = control.resultContainer = this.ResultContainer(control);
    control.appendChild(container);

    if (widthContainer) container.style.width = widthContainer + "px";
    if (heightContainer) container.style.height = heightContainer + "px";

    control.fill = function (results) {
        container.clear();
        if (!results) return;
        for (var i = 0; i < results.length; i++) {
            container.addResult(results[i], true);
        }
        container.onAction();
    }

    control.getValue = function () {
        var result = [];

        for (var num = 0; num < container.childNodes.length; num++) {
            var item = container.childNodes[num];
            if (item.key) {
                result.push({
                    "column": item.column.textBox.value == control.noResultText ? "" : item.column.textBox.value,
                    "aggrFunction": item.aggrFunction.key == "No" ? "" : item.aggrFunction.key,
                    "name": item.name.value
                });
            }
        }

        return result;
    }

    return control;
}

StiMobileDesigner.prototype.ResultContainer = function (resultControl) {
    var container = document.createElement("div");
    var jsObject = container.jsObject = this;
    container.className = "stiDesignerSortContainer";
    container.style.position = "relative";
    container.resultControl = resultControl;

    container.addResult = function (resultObject, notAction) {
        var item = jsObject.ResultItem(this);
        this.appendChild(item);
        if (!notAction) container.onAction();

        item.column.textBox.value = resultObject.column == "" ? resultControl.noResultText : resultObject.column;
        item.aggrFunction.setKey(resultObject.aggrFunction == "" ? "No" : resultObject.aggrFunction);
        item.name.value = resultObject.name;
        item.setSelected(true);

        return item;
    }

    container.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        this.onAction();
    }

    container.getCountItems = function () {
        var itemsCount = 0;
        for (var i = 0; i < this.childNodes.length; i++) {
            if (this.childNodes[i].key) itemsCount++;
        }
        return itemsCount;
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

            if (typeItem == "ResultItem") {
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
        resultControl.moveUpButton.setEnabled(index > 0);
        resultControl.moveDownButton.setEnabled(index != -1 && index < count - 1);

        this.checkEmptyPanel();
    }

    container.checkEmptyPanel = function () {
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
                emptyPanel = jsObject.EmptyTextPanel("BigSummary.png", jsObject.loc.PropertyMain.NoElements, "0.5", { width: 24, height: 24 });
                this.appendChild(emptyPanel);
            }
        }
    }

    return container;
}


StiMobileDesigner.prototype.ResultItem = function (container) {
    var item = document.createElement("div");
    var jsObject = item.jsObject = this;
    item.key = this.generateKey();
    item.isSelected = false;
    item.className = "stiDesignerSortPanel";

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "100%";
    item.appendChild(mainTable);
    var mainCell = mainTable.addCell();

    //Remove Button
    var removeButton = item.removeButton = this.StandartSmallButton(null, null, null, "RemoveBlack.png");
    removeButton.style.visibility = "hidden";
    removeButton.style.display = "inline-block";
    removeButton.style.margin = "2px 2px 2px 0";

    var closeCell = mainTable.addCell(removeButton);
    closeCell.style.width = "1px";
    closeCell.style.verticalAlign = "top";

    removeButton.action = function () {
        item.remove();
    }

    var innerTable = this.CreateHTMLTable();
    mainCell.appendChild(innerTable);

    //Captions
    innerTable.addTextCell(this.loc.PropertyMain.Column).style.padding = "6px 6px 0 6px";
    innerTable.addTextCell(this.loc.PropertyMain.AggregateFunction).style.padding = "6px 6px 0 6px";
    innerTable.addTextCell(this.loc.PropertyMain.Name).style.padding = "6px 6px 0 6px";

    item.updateControls = function () {
        var column = item.column.textBox.value != container.resultControl.noResultText ? item.column.textBox.value : "";
        item.name.value = column;
        if (item.aggrFunction.key != "No") {
            if (column) item.name.value += ".";
            item.name.value += item.aggrFunction.key;
        }
    }

    //Column
    item.column = this.TextBoxWithEditButton(null, 150);
    innerTable.addCellInNextRow(item.column).style.padding = "4px 6px 6px 6px";

    item.column.button.action = function () {
        var currentDataSourceName = container.resultControl.currentDataSourceName != null ? container.resultControl.currentDataSourceName : jsObject.options.selectedObject.properties["dataSource"];
        this.key = currentDataSourceName + "." + this.textBox.value;

        jsObject.InitializeDataColumnForm(function (form) {
            form.dataTree.build("Column", currentDataSourceName);
            form.needBuildTree = false;
            form.parentButton = item.column.button;
            form.changeVisibleState(true);

            form.action = function () {
                this.changeVisibleState(false);
                this.parentButton.textBox.value = this.dataTree.key != ""
                    ? this.dataTree.key.substring(this.dataTree.key.indexOf(".") + 1, this.dataTree.key.length)
                    : container.resultControl.noResultText;
                item.updateControls();
            }
        });
    }

    //AggrFunction    
    item.aggrFunction = this.DropDownList(null, this.options.isTouchDevice ? 125 : 140, null, this.GetResultFunctionItems(), true, false);
    innerTable.addCellInLastRow(item.aggrFunction).style.padding = "4px 6px 6px 6px";

    item.aggrFunction.action = function () {
        item.updateControls();
    }

    //Name
    item.name = this.TextBox(null, this.options.isTouchDevice ? 125 : 140);
    innerTable.addCellInLastRow(item.name).style.padding = "4px 6px 6px 6px";

    item.setSelected = function (state) {
        if (state) {
            if (container.selectedItem) {
                container.selectedItem.setSelected(false);
            }
            container.selectedItem = this;
        }
        else {
            if (container.selectedItem && container.selectedItem == this) {
                container.selectedItem = null;
            }
        }
        this.className = state ? "stiDesignerSortPanelSelected" : "stiDesignerSortPanel";
        this.isSelected = state;
        this.removeButton.style.visibility = state ? "visible" : "hidden";
    }

    item.remove = function () {
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

    item.getIndex = function () {
        for (var i = 0; i < container.childNodes.length; i++) {
            if (container.childNodes[i] == this) return i;
        }
    };

    item.move = function (direction) {
        var index = this.getIndex();
        container.removeChild(this);
        var count = container.getCountItems();
        var newIndex = direction == "Up" ? index - 1 : index + 1;

        if (direction == "Up" && newIndex == -1) {
            newIndex = 0;
        }
        if (direction == "Down" && newIndex >= count) {
            container.appendChild(this);
            container.onAction();
            return;
        }

        container.insertBefore(this, container.childNodes[newIndex]);
        container.onAction();
    }

    if (jsObject.options.isTouchDevice) {
        item.onclick = function () {
            if (!this.parentElement) return;
            this.setSelected(true);
            container.onAction();
        }
    }
    else {
        item.onmousedown = function (event) {
            this.setSelected(true);

            if (this.isTouchStartFlag || (event && event.target && event.target.nodeName && event.target.nodeName.toLowerCase() == "input")) return;
            event.preventDefault();

            var options = jsObject.options;
            if (options.controlsIsFocused && options.controlsIsFocused.action) {
                options.controlsIsFocused.blur();
                options.controlsIsFocused = null;
            }

            if (event.button != 2 && !options.controlsIsFocused) {
                var itemInDrag = jsObject.TreeItemForDragDrop({ name: jsObject.loc.PropertyMain.Sort, typeItem: "ResultItem" }, null, true);
                if (itemInDrag.button.captionCell) itemInDrag.button.captionCell.style.padding = "5px 20px 5px 10px";
                itemInDrag.beginingOffset = 0;
                options.itemInDrag = itemInDrag;
            }
        }

        item.onmouseover = function () {
            this.isOver = true;
            if (jsObject.options.itemInDrag && jsObject.options.itemInDrag.itemObject) {
                var typeItem = jsObject.options.itemInDrag.itemObject.typeItem;
                if (typeItem == "ResultItem") {
                    this.style.borderStyle = "dashed";
                    this.style.borderColor = jsObject.options.themeColors[jsObject.GetThemeColor()];
                }
            }
        }

        item.onmouseout = function () {
            this.isOver = false;
            this.style.borderStyle = "solid";
            this.style.borderColor = "";
        }
    }

    return item;
}