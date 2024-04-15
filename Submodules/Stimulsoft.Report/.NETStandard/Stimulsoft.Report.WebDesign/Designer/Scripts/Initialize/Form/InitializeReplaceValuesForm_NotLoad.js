
StiMobileDesigner.prototype.InitializeReplaceValuesForm_ = function () {

    var form = this.BaseForm("replaceValuesForm", this.loc.Dashboard.ReplaceValues, 2);

    var replaceControl = this.ReplaceValuesControl("dataTransformationReplaceControl", 700, 350);
    form.replaceControl = replaceControl;
    form.container.appendChild(replaceControl);

    replaceControl.style.margin = "12px 8px 12px 8px";
    replaceControl.toolBar.style.margin = "4px 4px 8px 4px";

    form.show = function (columnObject, replaceRules, filterItems) {
        this.changeVisibleState(true);
        replaceControl.columnObject = columnObject;
        replaceControl.filterItems = filterItems;
        replaceControl.setReplaceRules(this.jsObject.CopyObject(replaceRules));
    }

    return form;
}


StiMobileDesigner.prototype.ReplaceValuesControl = function (name, width, height) {
    var replaceControl = document.createElement("div");
    var jsObject = this;

    replaceControl.controls = {};
    replaceControl.name = name;
    replaceControl.columnObject = null;

    //ToolBar
    var toolBar = replaceControl.toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "4px";
    replaceControl.appendChild(toolBar);

    var addButton = this.FormButton(null, null, this.loc.Buttons.Add);
    toolBar.addCell(addButton);

    var moveUpButton = replaceControl.moveUpButton = this.StandartSmallButton(null, null, null, "Arrows.ArrowUpBlue.png", null, null);
    var moveDownButton = replaceControl.moveDownButton = this.StandartSmallButton(null, null, null, "Arrows.ArrowDownBlue.png", null, null);

    moveUpButton.style.marginLeft = moveDownButton.style.marginLeft = "6px";

    moveUpButton.setEnabled(false);
    moveDownButton.setEnabled(false);

    toolBar.addCell(moveUpButton);
    toolBar.addCell(moveDownButton);

    moveUpButton.action = function () {
        if (replaceControl.container.selectedItem) {
            replaceControl.container.selectedItem.move("Up");
        }
    }

    moveDownButton.action = function () {
        if (replaceControl.container.selectedItem) {
            replaceControl.container.selectedItem.move("Down");
        }
    }

    if (!this.options.isTouchDevice) {
        moveUpButton.style.display = moveDownButton.style.display = "none";
    }

    //Container
    var container = replaceControl.container = this.ReplaceValuesContainer(replaceControl);
    container.style.width = (width ? width : 600) + "px";
    container.style.height = (height ? height : 300) + "px";
    replaceControl.appendChild(container);

    addButton.action = function () {
        container.addReplaceRule(jsObject.DataActionRuleObject(replaceControl.columnObject.key, replaceControl.columnObject.path, "Replace"));
        container.onAction();
    }

    replaceControl.setReplaceRules = function (replaceRules) {
        container.clear();

        if (!replaceRules) return;

        for (var i = 0; i < replaceRules.length; i++) {
            container.addReplaceRule(replaceRules[i], true);
        }

        container.onAction();
    }

    replaceControl.getReplaceRules = function () {
        var replaceRules = [];

        for (var i = 0; i < container.childNodes.length; i++) {
            var replaceRule = container.childNodes[i].replaceRule;
            if (replaceRule.valueFrom) {
                replaceRules.push(container.childNodes[i].replaceRule)
            }
        }

        return replaceRules;
    }

    return replaceControl;
}

StiMobileDesigner.prototype.ReplaceValuesContainer = function (replaceControl) {
    var container = document.createElement("div");
    var jsObject = this;
    container.className = "stiDesignerFilterContainer";
    container.replaceControl = replaceControl;
    container.selectedItem = null;

    container.addReplaceRule = function (replaceRule, notAction) {
        var item = jsObject.ReplaceRuleItem(container, replaceRule);
        this.appendChild(item);
        item.valueFromControl.focus();

        item.setSelected(true);
        if (!notAction) this.onAction();
    }

    container.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        replaceControl.moveUpButton.setEnabled(false);
        replaceControl.moveDownButton.setEnabled(false);
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

            if (typeItem == "FilterItem") {
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
        replaceControl.moveUpButton.setEnabled(index > 0);
        replaceControl.moveDownButton.setEnabled(index != -1 && index < count - 1);
    }

    return container;
}

StiMobileDesigner.prototype.ReplaceRuleItem = function (replaceContainer, replaceRule) {
    var item = document.createElement("div");
    var jsObject = this;
    item.key = this.generateKey();
    item.isSelected = false;
    item.replaceRule = replaceRule;
    item.className = "stiDesignerFilterPanel";

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
    innerTable.style.margin = "8px";
    mainCell.appendChild(innerTable);

    innerTable.addTextCell(this.loc.Editor.FindWhat.replace(":", "")).className = "stiDesignerTextContainer";
    innerTable.addTextCell(this.loc.Editor.ReplaceWith.replace(":", "")).className = "stiDesignerTextContainer";
    innerTable.addCell();

    //Value From
    var valueFromControl = this.DropDownList(null, 180, null, replaceContainer.replaceControl.filterItems);
    innerTable.addCellInNextRow(valueFromControl).style.padding = "7px 7px 0 0";
    valueFromControl.textBox.value = item.replaceRule.valueFrom;
    item.valueFromControl = valueFromControl;

    valueFromControl.action = function () {
        item.replaceRule.valueFrom = this.textBox.value;
    }

    //Value To
    var valueToControl = this.DropDownList(null, 180, null, replaceContainer.replaceControl.filterItems);
    innerTable.addCellInLastRow(valueToControl).style.padding = "7px 7px 0 0";
    valueToControl.textBox.value = item.replaceRule.valueTo;

    valueToControl.action = function () {
        item.replaceRule.valueTo = this.textBox.value;
    }

    //MatchCase
    var matchCaseControl = this.CheckBox(null, this.loc.Editor.MatchCase.replace("&", ""));
    innerTable.addCellInLastRow(matchCaseControl).style.padding = "7px 7px 0 0";
    matchCaseControl.setChecked(item.replaceRule.matchCase);

    matchCaseControl.action = function () {
        item.replaceRule.matchCase = this.isChecked;
    }

    //MatchWholeWord
    var matchWholeWordControl = this.CheckBox(null, this.loc.Editor.MatchWholeWord.replace("&", ""));
    innerTable.addCellInLastRow(matchWholeWordControl).style.padding = "7px 7px 0 0";
    matchWholeWordControl.setChecked(item.replaceRule.matchWholeWord);

    matchWholeWordControl.action = function () {
        item.replaceRule.matchWholeWord = this.isChecked;
    }

    item.setSelected = function (state) {
        if (state) {
            if (replaceContainer.selectedItem) {
                replaceContainer.selectedItem.setSelected(false);
            }
            replaceContainer.selectedItem = this;
        }
        else {
            if (replaceContainer.selectedItem && replaceContainer.selectedItem == this) {
                replaceContainer.selectedItem = null;
            }
        }
        this.className = state ? "stiDesignerSortPanelSelected" : "stiDesignerSortPanel";
        this.isSelected = state;
        this.removeButton.style.visibility = state ? "visible" : "hidden";
    }

    item.remove = function () {
        if (replaceContainer.selectedItem == this) {
            var prevItem = this.previousSibling;
            var nextItem = this.nextSibling;
            replaceContainer.selectedItem = null;
            if (replaceContainer.childNodes.length > 1) {
                if (nextItem) {
                    nextItem.setSelected(true);
                    replaceContainer.selectedItem = nextItem;
                }
                else if (prevItem) {
                    prevItem.setSelected(true);
                    replaceContainer.selectedItem = prevItem;
                }
            }
        }
        replaceContainer.removeChild(this);
        replaceContainer.onAction();
    }

    item.getIndex = function () {
        for (var i = 0; i < replaceContainer.childNodes.length; i++)
            if (replaceContainer.childNodes[i] == this) return i;
    };

    item.move = function (direction) {
        var index = this.getIndex();
        replaceContainer.removeChild(this);
        var count = replaceContainer.getCountItems();
        var newIndex = direction == "Up" ? index - 1 : index + 1;
        if (direction == "Up" && newIndex == -1) newIndex = 0;
        if (direction == "Down" && newIndex >= count) {
            replaceContainer.appendChild(this);
            replaceContainer.onAction();
            return;
        }
        replaceContainer.insertBefore(this, replaceContainer.childNodes[newIndex]);
        replaceContainer.onAction();
    }

    if (jsObject.options.isTouchDevice) {
        item.onclick = function () {
            if (!this.parentElement) return;
            this.setSelected(true);
            replaceContainer.onAction();
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
                var itemInDrag = jsObject.TreeItemForDragDrop({ name: jsObject.loc.PropertyMain.Filter, typeItem: "FilterItem" }, null, true);
                if (itemInDrag.button.captionCell) itemInDrag.button.captionCell.style.padding = "5px 20px 5px 10px";
                itemInDrag.beginingOffset = 0;
                options.itemInDrag = itemInDrag;
            }
        }

        item.onmouseover = function () {
            this.isOver = true;
            if (jsObject.options.itemInDrag && jsObject.options.itemInDrag.itemObject) {
                var typeItem = jsObject.options.itemInDrag.itemObject.typeItem;
                if (typeItem == "FilterItem") {
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