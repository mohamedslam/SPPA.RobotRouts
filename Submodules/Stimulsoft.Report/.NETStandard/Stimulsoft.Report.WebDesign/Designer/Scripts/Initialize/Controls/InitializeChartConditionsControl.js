
StiMobileDesigner.prototype.ChartConditionsControl = function (width, height) {
    var jsObject = this;
    var control = document.createElement("div");

    var toolBar = control.toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "12px";
    control.appendChild(toolBar);

    //Add
    var addButton = control.addButton = this.FormButton(null, null, this.loc.Chart.AddCondition.replace("&", ""));
    toolBar.addCell(addButton);

    addButton.action = function () {
        var meters = [];
        if (control.valueMeters) meters = meters.concat(control.valueMeters);
        if (control.argumentMeters) meters = meters.concat(control.argumentMeters);
        if (control.seriesMeters) meters = meters.concat(control.seriesMeters);
        var keyMeter = meters.length > 0 ? meters[0].key : "";
        control.container.addCondition({ keyValueMeter: keyMeter, dataType: "Numeric", condition: "EqualTo", value: "", color: "255,255,255", markerType: "Circle", markerAngle: "0" });
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
    var container = control.container = this.ChartConditionsContainer(control);
    container.style.margin = "0 12px 12px 12px";
    control.appendChild(container);

    if (width) container.style.width = width + "px";
    if (height) container.style.height = height + "px";

    control.fill = function (conditions) {
        this.showMarker = this.valueMeters && jsObject.ShowChartElementMarkerProperty(this.valueMeters);
        container.clear();
        if (!conditions) return;
        for (var i = 0; i < conditions.length; i++) container.addCondition(conditions[i], true);
        container.onAction();
    }

    control.getValue = function () {
        var result = [];
        for (var i = 0; i < container.childNodes.length; i++) {
            var item = container.childNodes[i];
            if (item.key) {
                var value = StiBase64.encode(item.controls.dataType.key == "DateTime" && !item.controls.isExpression.key == "Expression" ? jsObject.DateToStringAmericanFormat(item.controls.valueDate.key, true) : item.controls.value.textBox.value);
                result.push({
                    keyValueMeter: item.controls.keyValueMeter.key,
                    dataType: item.controls.dataType.key,
                    condition: item.controls.condition.key,
                    value: value,
                    color: item.controls.color.key,
                    markerType: item.controls.markerType.key,
                    markerAngle: item.controls.markerAngle.getValue(),
                    isExpression: item.controls.isExpression.key == "Expression"
                });
            }
        }
        return result;
    }

    return control;
}

StiMobileDesigner.prototype.ChartConditionsContainer = function (mainControl) {
    var jsObject = this;
    var container = mainControl.container = document.createElement("div");
    container.className = "stiSimpleContainerWithBorder";
    container.style.overflow = "auto";
    container.style.position = "relative";
    container.mainControl = mainControl;
    container.selectedItem = null;

    container.checkDuplicateMeterLabels = function (meters) {
        var valueMeters = jsObject.CopyObject(mainControl.valueMeters || []);
        var argumentMeters = jsObject.CopyObject(mainControl.argumentMeters || []);
        var seriesMeters = jsObject.CopyObject(mainControl.seriesMeters || []);
        for (var i = 0; i < meters.length; i++) {
            var label = meters[i].label;
            var key = meters[i].key;
            for (var k = 0; k < valueMeters.length; k++)
                if (label == valueMeters[k].label && key != valueMeters[k].key) valueMeters[k].label += (" - " + jsObject.loc.PropertyMain.Value);
            for (var l = 0; l < argumentMeters.length; l++)
                if (label == argumentMeters[l].label && key != argumentMeters[l].key) argumentMeters[l].label += (" - " + jsObject.loc.PropertyMain.Argument);
            for (var m = 0; m < seriesMeters.length; m++)
                if (label == seriesMeters[m].label && key != seriesMeters[m].key) seriesMeters[m].label += (" - " + jsObject.loc.PropertyMain.Series);
        }
        var newMeters = [].concat(valueMeters).concat(argumentMeters).concat(seriesMeters);
        return newMeters;
    }

    container.addCondition = function (conditionObject, notAction) {
        var item = jsObject.ChartConditionsItem(this);
        this.appendChild(item);
        this.checkEmptyPanel();

        var meters = [];
        if (mainControl.valueMeters) meters = meters.concat(mainControl.valueMeters);
        if (mainControl.argumentMeters) meters = meters.concat(mainControl.argumentMeters);
        if (mainControl.seriesMeters) meters = meters.concat(mainControl.seriesMeters);
        meters = container.checkDuplicateMeterLabels(meters);

        var fieldIsItems = [];
        for (var i = 0; i < meters.length; i++) {
            fieldIsItems.push(jsObject.Item(meters[i].key, meters[i].label, null, meters[i].key));
        }
        item.controls.keyValueMeter.addItems(fieldIsItems);

        item.controls.keyValueMeter.setKey(conditionObject.keyValueMeter);
        item.controls.dataType.setKey(conditionObject.dataType);
        item.controls.condition.setKey(conditionObject.condition);
        item.controls.color.setKey(conditionObject.color);
        item.controls.markerType.setKey(conditionObject.markerType);
        item.controls.markerAngle.setValue(conditionObject.markerAngle);
        item.controls.isExpression.setKey(conditionObject.isExpression ? "Expression" : "Value");

        var value = StiBase64.decode(conditionObject.value);

        if (value != null) {
            if (conditionObject.dataType == "DateTime" && !conditionObject.isExpression)
                item.controls.valueDate.setKey(value ? new Date(value) : new Date());
            else
                item.controls.value.textBox.value = value;
        }

        item.updateControls();
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

            if (typeItem == "ConditionsItem") {
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
                emptyPanel = jsObject.EmptyTextPanel("BigConditions.png", jsObject.loc.Chart.NoConditions, "0.5");
                this.appendChild(emptyPanel);
            }
        }
    }

    return container;
}

StiMobileDesigner.prototype.ChartConditionsItem = function (container) {
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
    innerTable.style.margin = "12px 0 12px 12px";
    mainCell.appendChild(innerTable);

    //Captions
    innerTable.addTextCell(jsObject.loc.PropertyMain.FieldIs);
    innerTable.addTextCell(jsObject.loc.PropertyMain.DataType);
    innerTable.addTextCell(jsObject.loc.PropertyMain.Condition);
    innerTable.addTextCell(jsObject.loc.PropertyMain.Value);

    //FieldIs
    var keyValueMeter = this.DropDownList(null, 120, null, [], true, false);
    item.controls.keyValueMeter = keyValueMeter;
    keyValueMeter.style.margin = "3px 7px 3px 0";
    innerTable.addCellInNextRow(keyValueMeter);

    keyValueMeter.action = function () {
        item.updateControls();
    };

    //Data Type
    var dataType = this.DropDownList(null, 120, null, this.GetConditionsDataTypeItems(), true, false);
    item.controls.dataType = dataType;
    dataType.style.margin = "3px 7px 3px 0";
    innerTable.addCellInLastRow(dataType);

    dataType.action = function () {
        item.updateControls();
    };

    //Condition
    var condition = this.DropDownList(null, 120, null, [], true, false);
    item.controls.condition = condition;
    condition.style.margin = "3px 7px 3px 0";
    innerTable.addCellInLastRow(condition);

    //Value
    var value = this.ExpressionControl(null, 120, null, null, true);
    value.cutBrackets = true;
    item.controls.value = value;
    value.style.margin = "3px 7px 3px 0";
    var valueCell = innerTable.addCellInLastRow(value);

    //Value Date
    var valueDate = this.DateControl(null, 120, null, true);
    item.controls.valueDate = valueDate;
    valueDate.style.margin = "3px 7px 3px 0";
    valueCell.appendChild(valueDate);

    //IsExpression
    var isExpression = this.DropDownList(null, 100, null, this.GetFilterFieldIsItems(), true, false);
    item.controls.isExpression = isExpression;
    isExpression.style.margin = "3px 7px 3px 0";
    innerTable.addCellInLastRow(isExpression);

    isExpression.action = function () {
        item.updateControls();
    }

    var innerTable2 = this.CreateHTMLTable();
    innerTable2.style.margin = "0 0 12px 12px";
    item.appendChild(innerTable2);

    //Captions row2
    innerTable2.addTextCell(jsObject.loc.PropertyMain.Color);
    var markerTypeText = innerTable2.addTextCell(jsObject.loc.PropertyMain.MarkerType);
    var markerAngleText = innerTable2.addTextCell(jsObject.loc.PropertyMain.MarkerAngle);

    //Color
    var color = this.ColorControl(null, null, null, 120, true);
    item.controls.color = color;
    color.style.margin = "3px 7px 3px 0";
    innerTable2.addCellInNextRow(color);

    //MarkerType
    var markerType = this.DropDownList(null, 120, null, this.GetMarkerTypeItems(), true, false);
    item.controls.markerType = markerType;
    markerType.style.margin = "3px 7px 3px 0";
    innerTable2.addCellInLastRow(markerType);

    //MarkerAngle
    var markerAngle = this.TextBoxEnumerator(null, 120, null, false, 360, -360);
    item.controls.markerAngle = markerAngle;
    markerAngle.style.margin = "3px 7px 3px 0";
    innerTable2.addCellInLastRow(markerAngle);

    item.updateControls = function () {
        condition.addItems(jsObject.GetFilterConditionItems(dataType.key, true));
        condition.setKey((!condition.haveKey(condition.key) && condition.items != null && condition.items.length > 0) ? condition.items[0].key : condition.key);
        value.style.display = dataType.key != "DateTime" || isExpression.key == "Expression" ? "" : "none";
        valueDate.style.display = dataType.key == "DateTime" && isExpression.key != "Expression" ? "" : "none";
        var showMarker = false;
        if (container.mainControl.showMarker && container.mainControl.valueMeters && keyValueMeter.key) {
            for (var i = 0; i < container.mainControl.valueMeters.length; i++) {
                if (container.mainControl.valueMeters[i].key == keyValueMeter.key) {
                    showMarker = true;
                    break;
                }
            }
        }
        markerTypeText.style.display = markerType.style.display = markerAngleText.style.display = markerAngle.style.display = showMarker ? "" : "none";
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
                var itemInDrag = jsObject.TreeItemForDragDrop({ name: jsObject.loc.PropertyMain.Condition, typeItem: "ConditionsItem" }, null, true);
                if (itemInDrag.button.captionCell) itemInDrag.button.captionCell.style.padding = "5px 20px 5px 10px";
                itemInDrag.beginingOffset = 0;
                options.itemInDrag = itemInDrag;
            }
        }

        item.onmouseover = function () {
            this.isOver = true;
            if (jsObject.options.itemInDrag && jsObject.options.itemInDrag.itemObject) {
                var typeItem = jsObject.options.itemInDrag.itemObject.typeItem;
                if (typeItem == "ConditionsItem") {
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