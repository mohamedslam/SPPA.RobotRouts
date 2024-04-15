
StiMobileDesigner.prototype.TableConditionsControl = function (width, height) {
    var control = document.createElement("div");
    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "12px";
    control.appendChild(toolBar);

    //Add
    var addButton = control.addButton = this.FormButton(null, null, this.loc.Chart.AddCondition.replace("&", ""));
    toolBar.addCell(addButton);

    addButton.action = function () {
        control.container.addCondition({
            keyDataFieldMeters: [], keyDestinationMeters: [], dataType: "Numeric", condition: "EqualTo", value: "", foreColor: "0,0,0", backColor: "transparent", font: "Arial!8!0!0!0!0", isExpression: false,
            permissions: "Font, FontSize, FontStyleBold, FontStyleItalic, FontStyleUnderline, FontStyleStrikeout, ForeColor, BackColor"
        });
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
        if (control.container.selectedItem) {
            control.container.selectedItem.move("Up");
        }
    }

    moveDownButton.action = function () {
        if (control.container.selectedItem) {
            control.container.selectedItem.move("Down");
        }
    }

    if (!this.options.isTouchDevice) {
        moveUpButton.style.display = moveDownButton.style.display = separator.style.display = "none";
    }

    //Container
    var container = control.container = this.TableConditionsContainer(control);
    container.style.margin = "0 12px 12px 12px";
    control.appendChild(container);

    if (width) control.container.style.width = width + "px";
    if (height) control.container.style.height = height + "px";

    control.fill = function (conditions) {
        container.clear();
        if (!conditions) return;
        for (var i = 0; i < conditions.length; i++) {
            container.addCondition(conditions[i], true);
        }
        container.onAction();
    }

    control.getValue = function () {
        var result = [];
        for (var i = 0; i < container.childNodes.length; i++) {
            var item = container.childNodes[i];
            if (item.key) {
                result.push(item.getCondition());
            }
        }
        return result;
    }

    return control;
}

StiMobileDesigner.prototype.TableConditionsContainer = function (mainControl) {
    var jsObject = this;
    var container = mainControl.container = document.createElement("div");
    container.className = "stiSimpleContainerWithBorder";
    container.style.overflow = "auto";
    container.mainControl = mainControl;
    container.selectedItem = null;

    container.addCondition = function (conditionObject, notAction) {
        var item = jsObject.TableConditionsItem(this, mainControl.valueMeters);
        this.appendChild(item);
        this.checkEmptyPanel();

        item.controls.keyDataFieldMeters.setKey(conditionObject.keyDataFieldMeters);
        item.controls.keyDestinationMeters.setKey(conditionObject.keyDestinationMeters);
        item.controls.dataType.setKey(conditionObject.dataType);
        item.controls.condition.setKey(conditionObject.condition);
        var font = jsObject.FontStrToObject(conditionObject.font);
        item.controls.buttonBold.setSelected(font.bold == "1");
        item.controls.buttonItalic.setSelected(font.italic == "1");
        item.controls.buttonUnderline.setSelected(font.underline == "1");
        item.controls.buttonStrikeout.setSelected(font.strikeout == "1");
        item.controls.foreColor.setKey(conditionObject.foreColor);
        item.controls.backColor.setKey(conditionObject.backColor);
        item.controls.permissions.setKey(conditionObject.permissions);
        item.controls.isExpression.setKey(conditionObject.isExpression ? "Expression" : "Value");

        var value = StiBase64.decode(conditionObject.value);

        if (value != null) {
            if (item.controls.isExpression.key == "Expression")
                item.controls.value.textBox.value = value;
            else if (conditionObject.dataType == "DateTime")
                item.controls.valueDate.setKey(value ? new Date(value) : new Date());
            else if (conditionObject.dataType == "Boolean")
                item.controls.valueBool.setChecked(value.toLowerCase() == "true");
            else
                item.controls.value.textBox.value = value;
        }

        item.updateControls();
        item.setSelected();
        if (!notAction) this.onAction();
    }

    container.removeCondition = function () {
        for (var i = 0; i < this.childNodes.length; i++) {
            if (this.childNodes[i].isSelected) {
                this.removeChild(this.childNodes[i]);
                this.selectedItem = null;
            }
        }
        if (this.childNodes.length > 0) this.childNodes[0].setSelected();
        this.onAction();
    }

    container.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        this.mainControl.moveUpButton.setEnabled(false);
        this.mainControl.moveDownButton.setEnabled(false);
        this.checkEmptyPanel();
    }

    container.getCountItems = function () {
        return container.childNodes.length;
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


StiMobileDesigner.prototype.TableConditionsItem = function (container, valueMeters) {
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
    innerTable.style.margin = "12px 0 6px 12px";
    mainCell.appendChild(innerTable);

    //Captions
    innerTable.addTextCell(jsObject.loc.PropertyMain.DataField);
    innerTable.addTextCell(jsObject.loc.PropertyMain.DataType);
    innerTable.addTextCell(jsObject.loc.PropertyMain.Condition);
    innerTable.addTextCell(jsObject.loc.PropertyMain.Value);

    //FieldIs
    var keyDataFieldMeters = this.CheckBoxesDropDownControl(null, 130, valueMeters, true);
    item.controls.keyDataFieldMeters = keyDataFieldMeters;
    keyDataFieldMeters.style.margin = "3px 7px 3px 0";
    innerTable.addCellInNextRow(keyDataFieldMeters);

    //Data Type
    var dataType = this.DropDownList(null, 125, null, this.GetConditionsDataTypeItems(), true, false);
    item.controls.dataType = dataType;
    dataType.style.margin = "3px 7px 3px 0";
    innerTable.addCellInLastRow(dataType);

    dataType.action = function () {
        item.updateControls();
    };

    //Condition
    var condition = this.DropDownList(null, 110, null, this.GetFilterConditionItems("Numeric", true), true, false);
    item.controls.condition = condition;
    condition.style.margin = "3px 7px 3px 0";
    innerTable.addCellInLastRow(condition);

    //Value
    var value = this.ExpressionControl(null, 128, null, null, true);
    value.button.style.display = "none"; //temporary
    value.button.parentElement.style.width = "3px"; //temporary
    item.controls.value = value;
    value.style.margin = "3px 7px 3px 0";
    var valueCell = innerTable.addCellInLastRow(value);

    //Value Date
    var valueDate = this.DateControl(null, 111, null, true);
    item.controls.valueDate = valueDate;
    valueDate.style.margin = "3px 7px 3px 0";
    valueCell.appendChild(valueDate);

    //Value Bool
    var valueBool = this.CheckBox();
    item.controls.valueBool = valueBool;
    valueBool.style.margin = "3px 103px 3px 2px";
    valueCell.appendChild(valueBool);

    //IsExpression
    var isExpression = this.DropDownList(null, 110, null, this.GetFilterFieldIsItems(), true, false);
    item.controls.isExpression = isExpression;
    isExpression.style.margin = "3px 0 3px 0";
    innerTable.addCellInLastRow(isExpression);

    isExpression.action = function () {
        item.updateControls();
    };

    var innerTable2 = this.CreateHTMLTable();
    innerTable2.style.margin = "0 0 6px 12px";
    mainCell.appendChild(innerTable2);

    //Captions row2
    innerTable2.addTextCell(jsObject.loc.PropertyMain.Destination);
    innerTable2.addTextCell(" ");
    innerTable2.addTextCell(" ");
    innerTable2.addTextCell(jsObject.loc.PropertyMain.ForeColor);
    innerTable2.addTextCell(jsObject.loc.PropertyMain.BackColor);

    //Destination
    var keyDestinationMeters = this.CheckBoxesDropDownControl(null, 130, valueMeters, true);
    item.controls.keyDestinationMeters = keyDestinationMeters;
    keyDestinationMeters.style.margin = "3px 7px 3px 0";
    innerTable2.addCellInNextRow(keyDestinationMeters);
        
    //Font Buttons
    var fontTable = this.CreateHTMLTable();
    fontTable.style.marginRight = "4px";
    innerTable2.addCellInLastRow(fontTable).style.verticalAlign = "top";

    var buttonBold = this.StandartSmallButton(null, null, null, "Bold.png");
    buttonBold.style.margin = "3px 7px 3px 0";
    item.controls.buttonBold = buttonBold;
    fontTable.addCell(buttonBold);

    buttonBold.action = function () {
        this.setSelected(!this.isSelected);
        item.updateControls();
    };

    var buttonItalic = this.StandartSmallButton(null, null, null, "Italic.png");
    buttonItalic.style.margin = "3px 7px 3px 0";
    item.controls.buttonItalic = buttonItalic;
    fontTable.addCell(buttonItalic);

    buttonItalic.action = function () {
        this.setSelected(!this.isSelected);
        item.updateControls();
    };

    var buttonUnderline = this.StandartSmallButton(null, null, null, "Underline.png");
    buttonUnderline.style.margin = "3px 7px 3px 0";
    item.controls.buttonUnderline = buttonUnderline;
    fontTable.addCell(buttonUnderline);

    buttonUnderline.action = function () {
        this.setSelected(!this.isSelected);
        item.updateControls();
    };

    var buttonStrikeout = this.StandartSmallButton(null, null, null, "Strikeout.png");
    buttonStrikeout.style.margin = "3px 7px 3px 0";
    item.controls.buttonStrikeout = buttonStrikeout;
    fontTable.addCell(buttonStrikeout);

    buttonStrikeout.action = function () {
        this.setSelected(!this.isSelected);
        item.updateControls();
    };

    //Permissions
    var checkBoxes = [
        ["FontStyleBold", this.loc.PropertyMain.FontBold],
        ["FontStyleItalic", this.loc.PropertyMain.FontItalic],
        ["FontStyleUnderline", this.loc.PropertyMain.FontUnderline],
        ["FontStyleStrikeout", this.loc.PropertyMain.FontStrikeout],
        ["ForeColor", this.loc.PropertyMain.ForeColor],
        ["BackColor", this.loc.PropertyMain.BackColor]
    ];
    var permissions = this.CheckBoxesControl(null, null, "Permissions.png", checkBoxes, true);
    permissions.style.margin = "3px 85px 3px 2px";
    item.controls.permissions = permissions;
    innerTable2.addCellInLastRow(permissions);

    permissions.action = function () {
        item.updateControls();
    };

    //ForeColor
    var foreColor = this.ColorControl(null, null, null, 110, true);
    foreColor.style.margin = "3px 7px 3px 0";
    item.controls.foreColor = foreColor;
    innerTable2.addCellInLastRow(foreColor);

    foreColor.action = function () {
        item.updateControls();
    };

    //BackColor
    var backColor = this.ColorControl(null, null, null, 110, true);
    backColor.style.margin = "3px 0 3px 0";
    item.controls.backColor = backColor;
    innerTable2.addCellInLastRow(backColor);

    backColor.action = function () {
        item.updateControls();
    };

    item.updateControls = function () {
        condition.addItems(jsObject.GetFilterConditionItems(dataType.key, true));
        condition.setKey((!condition.haveKey(condition.key) && condition.items != null && condition.items.length > 0) ? condition.items[0].key : condition.key);
        value.style.display = ((dataType.key == "DateTime" || dataType.key == "Boolean") && isExpression.key == "Value") ? "none" : "";
        valueDate.style.display = dataType.key == "DateTime" && isExpression.key == "Value" ? "" : "none";
        valueBool.style.display = dataType.key == "Boolean" && isExpression.key == "Value" ? "" : "none";

        var conditions = item.getCondition();
        var permissions = conditions.permissions;

        var fontBoldE = (permissions == "All" || permissions.indexOf("FontStyleBold") >= 0);
        var fontItalicE = (permissions == "All" || permissions.indexOf("FontStyleItalic") >= 0);
        var fontUnderlineE = (permissions == "All" || permissions.indexOf("FontStyleUnderline") >= 0);
        var fontStrikeoutE = (permissions == "All" || permissions.indexOf("FontStyleStrikeout") >= 0);
        var foreColorE = (permissions == "All" || permissions.indexOf("ForeColor") >= 0);
        var backColorE = (permissions == "All" || permissions.indexOf("BackColor") >= 0);

        buttonBold.setEnabled(fontBoldE);
        buttonItalic.setEnabled(fontItalicE);
        buttonStrikeout.setEnabled(fontStrikeoutE);
        buttonUnderline.setEnabled(fontUnderlineE);
        foreColor.setEnabled(foreColorE);
        backColor.setEnabled(backColorE);
    }

    item.getCondition = function () {
        var value = item.controls.value.textBox.value;
        if (item.controls.dataType.key == "DateTime" && isExpression.key == "Value")
            value = jsObject.DateToStringAmericanFormat(item.controls.valueDate.key, true);
        if (item.controls.dataType.key == "Boolean" && isExpression.key == "Value")
            value = valueBool.isChecked ? (jsObject.options.jsMode ? "true" : "True") : (jsObject.options.jsMode ? "false" : "False");

        return {
            keyDataFieldMeters: item.controls.keyDataFieldMeters.key,
            keyDestinationMeters: item.controls.keyDestinationMeters.key,
            dataType: item.controls.dataType.key,
            condition: item.controls.condition.key,
            value: StiBase64.encode(value),
            font: jsObject.FontObjectToStr({
                name: "Arial",
                size: "8",
                bold: buttonBold.isSelected ? "1" : "0",
                italic: buttonItalic.isSelected ? "1" : "0",
                underline: buttonUnderline.isSelected ? "1" : "0",
                strikeout: buttonStrikeout.isSelected ? "1" : "0",
            }),
            foreColor: foreColor.key,
            backColor: backColor.key,
            permissions: permissions.key,
            isExpression: isExpression.key == "Expression"
        };
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
            this.setSelected();

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