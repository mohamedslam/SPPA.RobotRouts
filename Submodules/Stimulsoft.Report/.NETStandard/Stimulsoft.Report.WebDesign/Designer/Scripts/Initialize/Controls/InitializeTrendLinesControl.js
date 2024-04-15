
StiMobileDesigner.prototype.TrendLinesControl = function (width, height) {
    var control = document.createElement("div");

    var toolBar = control.toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "4px";
    control.appendChild(toolBar);

    //Add
    var addButton = control.addButton = this.FormButton(null, null, this.loc.Chart.AddTrendLine);
    toolBar.addCell(addButton);

    addButton.action = function () {
        var keyValueMeter = (control.valueMeters && control.valueMeters.length > 0) ? control.valueMeters[0].key : "";
        control.container.addTrendLine({ keyValueMeter: keyValueMeter, type: "None", lineColor: "0,0,0", lineStyle: "0", lineWidth: "1" });
    }

    var separator = this.HomePanelSeparator();
    separator.style.margin = "0 2px 0 2px";
    toolBar.addCell(separator);

    var moveUpButton = control.moveUpButton = this.StandartSmallButton(null, null, null, "Arrows.ArrowUpBlue.png", null, null);
    var moveDownButton = control.moveDownButton = this.StandartSmallButton(null, null, null, "Arrows.ArrowDownBlue.png", null, null);

    moveUpButton.setEnabled(false);
    moveDownButton.setEnabled(false);

    toolBar.addCell(moveUpButton);
    toolBar.addCell(moveDownButton);

    moveUpButton.action = function () {
        if (control.container.selectedItem) { control.container.selectedItem.move("Up"); }
    }

    moveDownButton.action = function () {
        if (control.container.selectedItem) { control.container.selectedItem.move("Down"); }
    }

    if (!this.options.isTouchDevice) {
        moveUpButton.style.display = moveDownButton.style.display = separator.style.display = "none";
    }

    //Container
    var container = control.container = this.TrendLinesContainer(control);
    control.appendChild(container);

    if (width) container.style.width = width + "px";
    if (height) container.style.height = height + "px";

    control.fill = function (trendLines) {
        container.clear();
        if (!trendLines) return;
        for (var i = 0; i < trendLines.length; i++) container.addTrendLine(trendLines[i], true);
        container.onAction();
    }

    control.getValue = function () {
        var result = [];
        for (var i = 0; i < container.childNodes.length; i++) {
            var item = container.childNodes[i];
            if (item.key) {
                result.push({
                    keyValueMeter: item.controls.keyValueMeter.key,
                    type: item.controls.type.key,
                    lineColor: item.controls.lineColor.key,
                    lineStyle: item.controls.lineStyle.key,
                    lineWidth: item.controls.lineWidth.value
                });
            }
        }
        return result;
    }

    return control;
}

StiMobileDesigner.prototype.TrendLinesContainer = function (mainControl) {
    var jsObject = this;
    var container = document.createElement("div");
    mainControl.container = container;

    container.className = "stiDesignerSortContainer";
    container.mainControl = mainControl;
    container.selectedItem = null;

    container.addTrendLine = function (trendLineObject, notAction) {
        var item = jsObject.TrendLineItem(this);
        this.appendChild(item);
        this.checkEmptyPanel();

        var fieldIsItems = [];
        if (mainControl.valueMeters) {
            for (var i = 0; i < mainControl.valueMeters.length; i++) {
                fieldIsItems.push(jsObject.Item(mainControl.valueMeters[i].key, mainControl.valueMeters[i].label, null, mainControl.valueMeters[i].key));
            }
        }

        item.controls.keyValueMeter.addItems(fieldIsItems);
        item.controls.keyValueMeter.setKey(trendLineObject.keyValueMeter);
        item.controls.type.setKey(trendLineObject.type);
        item.controls.lineColor.setKey(trendLineObject.lineColor, true);
        item.controls.lineStyle.setKey(trendLineObject.lineStyle);
        item.controls.lineWidth.value = trendLineObject.lineWidth;

        item.setSelected();
        if (!notAction) this.onAction();
    }

    container.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        mainControl.moveUpButton.setEnabled(false);
        mainControl.moveDownButton.setEnabled(false);
        this.checkEmptyPanel();
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

            if (typeItem == "TrendLinesItem") {
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
        var count = this.getCountItems();
        var index = this.selectedItem ? this.selectedItem.getIndex() : -1;
        mainControl.moveUpButton.setEnabled(index > 0);
        mainControl.moveDownButton.setEnabled(index != -1 && index < count - 1);
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
                emptyPanel = jsObject.EmptyTextPanel("Series_TrendLine.png", jsObject.loc.PropertyMain.NoElements, "0.5");
                this.appendChild(emptyPanel);
            }
        }
    }

    return container;
}


StiMobileDesigner.prototype.TrendLineItem = function (container) {
    var jsObject = this;
    var item = document.createElement("div");
    item.controls = {};
    item.container = container;
    item.isSelected = false;
    item.className = "stiDesignerSortPanel";
    item.key = this.generateKey();

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "100%";
    item.appendChild(mainTable);
    var mainCell = mainTable.addCell();

    //Remove Button
    var removeButton = this.StandartSmallButton(null, null, null, "RemoveBlack.png");
    removeButton.style.visibility = "hidden";
    removeButton.style.display = "inline-block";
    removeButton.style.margin = "2px 2px 2px 0";
    item.removeButton = removeButton;
    var closeCell = mainTable.addCell(removeButton);
    closeCell.style.width = "1px";
    closeCell.style.verticalAlign = "top";

    removeButton.action = function () {
        item.remove();
    }

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "6px 0 6px 6px";
    innerTable.style.width = "100%";
    mainCell.appendChild(innerTable);

    var props = [
        ["keyValueMeter", this.loc.PropertyMain.FieldIs, this.DropDownList(null, 150, null, [], true)],
        ["type", this.loc.PropertyMain.Type, this.DropDownList(null, 150, null, this.GetTrendLinesTypeItems(), true)],
        ["lineColor", this.loc.PropertyMain.Color, this.ColorControl(null, null, true, 150, true)],
        ["lineStyle", this.loc.PropertyMain.Style, this.DropDownList(null, 150, null, this.GetBorderStyleItems(), true, true)],
        ["lineWidth", this.loc.PropertyMain.Width, this.TextBoxPositiveIntValue(null, 150)]
    ];

    for (var i = 0; i < props.length; i++) {
        var textCell = innerTable.addTextCellInLastRow(props[i][1]);
        textCell.className = "stiDesignerCaptionControlsBigIntervals";
        textCell.style.width = "100%";
        var control = props[i][2];
        control.style.margin = "6px 12px 6px 12px";
        item.controls[props[i][0]] = control;
        innerTable.addCellInLastRow(control);
        if (i < props.length - 1) innerTable.addRow();
    }

    item.setSelected = function () {
        for (var i = 0; i < container.childNodes.length; i++) {
            container.childNodes[i].className = "stiDesignerSortPanel";
            container.childNodes[i].isSelected = false;
            container.childNodes[i].removeButton.style.visibility = "hidden";
        }
        container.selectedItem = this;
        this.isSelected = true;
        this.className = "stiDesignerSortPanelSelected";
        this.removeButton.style.visibility = "visible";
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
        for (var i = 0; i < container.childNodes.length; i++)
            if (container.childNodes[i] == this) return i;
    };

    item.move = function (direction) {
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
        item.onclick = function () {
            if (!this.parentElement) return;
            this.setSelected();
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
                var itemInDrag = jsObject.TreeItemForDragDrop({ name: jsObject.loc.PropertyMain.TrendLine, typeItem: "TrendLinesItem" }, null, true);
                if (itemInDrag.button.captionCell) itemInDrag.button.captionCell.style.padding = "5px 20px 5px 10px";
                itemInDrag.beginingOffset = 0;
                options.itemInDrag = itemInDrag;
            }
        }

        item.onmouseover = function () {
            this.isOver = true;
            if (jsObject.options.itemInDrag && jsObject.options.itemInDrag.itemObject) {
                var typeItem = jsObject.options.itemInDrag.itemObject.typeItem;
                if (typeItem == "TrendLinesItem") {
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