
StiMobileDesigner.prototype.PivotTableConditionsControl = function (width, height) {
    var control = document.createElement("div");
    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "12px";
    control.appendChild(toolBar);

    //Add
    var addButton = control.addButton = this.FormButton(null, null, this.loc.Chart.AddCondition.replace("&", ""));
    toolBar.addCell(addButton);

    addButton.action = function () {
        var keyValueMeter = control.valueMeters && control.valueMeters.length > 0 ? control.valueMeters[0].key : "";
        control.container.addCondition({
            keyValueMeter: keyValueMeter, dataType: "Numeric", condition: "EqualTo", value: "", backColor: "transparent", customIcon: null, font: "Arial!8!0!0!0!0",
            icon: "Adjust", iconAlignment: "None", iconColor: "128,0,0", textColor: "0,0,0", permissions: "Font, FontSize, FontStyleBold, FontStyleItalic, FontStyleUnderline, FontStyleStrikeout, TextColor, BackColor"
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
        if (control.container.selectedItem) { control.container.selectedItem.move("Up"); }
    }

    moveDownButton.action = function () {
        if (control.container.selectedItem) { control.container.selectedItem.move("Down"); }
    }

    if (!this.options.isTouchDevice) {
        moveUpButton.style.display = moveDownButton.style.display = separator.style.display = "none";
    }

    //Container
    var container = control.container = this.PivotTableConditionsContainer(control);
    container.style.margin = "0 12px 12px 12px";
    control.appendChild(container);

    if (width) container.style.width = width + "px";
    if (height) container.style.height = height + "px";

    control.fill = function (conditions) {
        container.clear();
        if (!conditions) return;
        for (var i = 0; i < conditions.length; i++)
            this.container.addCondition(conditions[i], true);
        this.container.onAction();
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

StiMobileDesigner.prototype.PivotTableConditionsContainer = function (mainControl) {
    var jsObject = this;
    var container = mainControl.container = document.createElement("div");
    container.className = "stiSimpleContainerWithBorder";
    container.style.overflow = "auto";
    container.mainControl = mainControl;
    container.selectedItem = null;

    container.addCondition = function (conditionObject, notAction) {
        var item = jsObject.PivotTableConditionsItem(this);
        this.appendChild(item);
        this.checkEmptyPanel();

        var valueMetersItems = [];
        for (var key in mainControl.valueMeters) {
            valueMetersItems.push(jsObject.Item(mainControl.valueMeters[key], mainControl.valueMeters[key], null, key));
        }
        item.controls.keyValueMeter.addItems(valueMetersItems);
        if (mainControl.valueMeters[conditionObject.keyValueMeter]) {
            item.controls.keyValueMeter.setKey(conditionObject.keyValueMeter);
        } else {
            for (var key in mainControl.valueMeters) {
                var value = mainControl.valueMeters[key];
                if (value == conditionObject.keyValueMeter) {
                    item.controls.keyValueMeter.setKey(conditionObject.keyValueMeter);
                    item.controls.keyValueMeter.key = key;
                }
            }
        }
        item.controls.dataType.setKey(conditionObject.dataType);
        item.controls.condition.setKey(conditionObject.condition);
        var font = jsObject.FontStrToObject(conditionObject.font);
        item.controls.fontName.setKey(font.name);
        item.controls.fontSize.setKey(font.size);
        item.controls.buttonBold.setSelected(font.bold == "1");
        item.controls.buttonItalic.setSelected(font.italic == "1");
        item.controls.buttonUnderline.setSelected(font.underline == "1");
        item.controls.buttonStrikeout.setSelected(font.strikeout == "1");
        item.controls.iconColor.setKey(conditionObject.iconColor);
        item.controls.textColor.setKey(conditionObject.textColor);
        item.controls.backColor.setKey(conditionObject.backColor);
        item.controls.permissions.setKey(conditionObject.permissions);
        item.controls.customIconControl.setImage(conditionObject.customIcon);
        item.controls.iconControl.setKey(conditionObject.icon);
        item.controls.iconAlign.setKey(conditionObject.iconAlignment);

        var value = StiBase64.decode(conditionObject.value);

        if (value != null) {
            if (conditionObject.dataType == "DateTime")
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


StiMobileDesigner.prototype.PivotTableConditionsItem = function (container) {
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
    innerTable.addTextCell(jsObject.loc.PropertyMain.FieldIs);
    innerTable.addTextCell(jsObject.loc.PropertyMain.DataType);
    innerTable.addTextCell(jsObject.loc.PropertyMain.Condition);
    innerTable.addTextCell(jsObject.loc.PropertyMain.Value);

    //FieldIs
    var keyValueMeter = this.DropDownList(null, 107, null, [], true, false);
    item.controls.keyValueMeter = keyValueMeter;
    keyValueMeter.style.margin = "3px 7px 3px 0";
    innerTable.addCellInNextRow(keyValueMeter);

    //Data Type
    var dataType = this.DropDownList(null, 107, null, this.GetConditionsDataTypeItems(), true, false);
    item.controls.dataType = dataType;
    dataType.style.margin = "3px 7px 3px 0";
    innerTable.addCellInLastRow(dataType);

    dataType.action = function () {
        item.updateControls();
    };

    //Condition
    var condition = this.DropDownList(null, 107, null, [], true, false);
    item.controls.condition = condition;
    condition.style.margin = "3px 7px 3px 0";
    innerTable.addCellInLastRow(condition);

    //Value
    var value = this.ExpressionControl(null, 210, null, null, true);
    value.button.style.display = "none"; //temporary
    value.button.parentElement.style.width = "3px"; //temporary
    item.controls.value = value;
    value.style.margin = "3px 7px 3px 0";
    var valueCell = innerTable.addCellInLastRow(value);

    //Value Date
    var valueDate = this.DateControl(null, 193, null, true);
    item.controls.valueDate = valueDate;
    valueDate.style.margin = "3px 7px 3px 0";
    valueCell.appendChild(valueDate);

    var innerTable2 = this.CreateHTMLTable();
    innerTable2.style.margin = "0 0 6px 12px";
    item.appendChild(innerTable2);

    //Captions row2
    innerTable2.addTextCell(jsObject.loc.PropertyMain.Font).colSpan = 12;
    innerTable2.addTextCell(jsObject.loc.PropertyMain.Icon);

    var lbAbc = document.createElement("div");
    lbAbc.setAttribute("style", "height: 54px; width: 110px; margin-right: 7px; display: table; overflow: hidden; text-align: center; vertical-align: top;");
    lbAbc.className = "stiSimpleContainerWithBorder";
    item.controls.lbAbc = lbAbc;
    innerTable2.addCellInNextRow(lbAbc).rowSpan = 2;

    var lbAbcText = document.createElement("div");
    lbAbcText.innerHTML = "AaBbCcYyZz";
    lbAbcText.style.display = "table-cell";
    lbAbcText.style.maxWidth = lbAbc.style.width;
    lbAbcText.style.verticalAlign = "middle";
    lbAbc.appendChild(lbAbcText);

    var fontName = this.FontList(null, 140, null, true);
    fontName.setKey("Arial");
    fontName.style.margin = "3px 7px 3px 0";
    item.controls.fontName = fontName;
    innerTable2.addCellInLastRow(fontName).colSpan = 9;

    fontName.action = function () {
        item.updateControls();
    };

    var fontSize = this.DropDownList(null, 40, [this.loc.HelpDesigner.FontSize, this.HelpLinks["font"]], this.GetFontSizeItems(), false);
    fontSize.setKey("8");
    fontSize.style.margin = "3px 7px 3px 0";
    item.controls.fontSize = fontSize;
    innerTable2.addCellInLastRow(fontSize);

    fontSize.action = function () {
        item.updateControls();
    };

    var checkBoxes = [
        ["Font", this.loc.PropertyMain.FontName],
        ["FontSize", this.loc.PropertyMain.FontSize],
        ["FontStyleBold", this.loc.PropertyMain.FontBold],
        ["FontStyleItalic", this.loc.PropertyMain.FontItalic],
        ["FontStyleUnderline", this.loc.PropertyMain.FontUnderline],
        ["FontStyleStrikeout", this.loc.PropertyMain.FontStrikeout],
        ["TextColor", this.loc.PropertyMain.TextColor],
        ["BackColor", this.loc.PropertyMain.BackColor]
    ];
    var permissions = this.CheckBoxesControl(null, null, "Permissions.png", checkBoxes, true);
    permissions.style.margin = "3px 7px 3px 0";
    item.controls.permissions = permissions;
    innerTable2.addCellInLastRow(permissions);

    permissions.action = function () {
        item.updateControls();
    };

    var iconAlign = item.controls.iconAlign = this.DropDownList(null, 70, null, this.GetTargetIconAlignmentItems(), true);
    iconAlign.setKey("None");
    iconAlign.style.margin = "3px 7px 3px 0";
    innerTable2.addCellInLastRow(iconAlign);

    iconAlign.action = function () {
        item.updateControls();
    };

    var customIconControl = this.ImageControl(null, 104, 62);
    customIconControl.style.borderStyle = "solid";
    item.controls.customIconControl = customIconControl;
    item.controls.customIconControlCell = innerTable2.addCellInLastRow(customIconControl);
    item.controls.customIconControlCell.rowSpan = 2;

    customIconControl.action = function () {
        item.updateControls();
    };

    var iconControl = this.IconControl(null, 100, null, null, customIconControl);
    item.controls.iconControl = iconControl;
    innerTable2.addCellInLastRow(iconControl);

    iconControl.action = function () {
        item.updateControls();
    };

    var fontTable = this.CreateHTMLTable();
    innerTable2.addCellInNextRow(fontTable).colSpan = 11;

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

    var sep = this.HomePanelSeparator();
    sep.style.margin = "3px 7px 3px 0";
    fontTable.addCell(sep);

    var textColor = this.ColorControlWithImage(null, "TextColor.png", null, true);
    textColor.setKey("255,0,0");
    textColor.style.margin = "3px 7px 3px 0";
    item.controls.textColor = textColor;
    fontTable.addCell(textColor);

    textColor.action = function () {
        item.updateControls();
    };

    var backColor = this.ColorControlWithImage(null, "BackgroundColor.png", null, true);
    backColor.setKey("255,255,255");
    backColor.style.margin = "3px 7px 3px 0";
    item.controls.backColor = backColor;
    fontTable.addCell(backColor);

    backColor.action = function () {
        item.updateControls();
    };

    innerTable2.addCellInLastRow(document.createElement("div"));

    var iconColor = this.ColorControl(null, null, null, 100, true);
    item.controls.iconColor = iconColor;
    innerTable2.addCellInLastRow(iconColor);

    iconColor.action = function () {
        item.updateControls();
    };

    item.updateControls = function () {
        condition.addItems(jsObject.GetFilterConditionItems(dataType.key, true));
        condition.setKey((!condition.haveKey(condition.key) && condition.items != null && condition.items.length > 0) ? condition.items[0].key : condition.key);
        value.style.display = dataType.key != "DateTime" ? "" : "none";
        valueDate.style.display = dataType.key == "DateTime" ? "" : "none";
        item.controls.customIconControlCell.style.display = iconAlign.key != "None" && customIconControl.src != null && customIconControl.src != "" ? "" : "none";
        iconColor.style.display = iconControl.style.display = item.controls.customIconControlCell.style.display == "none" ? "" : "none";

        var conditions = item.getCondition();
        var font = jsObject.FontStrToObject(conditions.font);
        var permissions = conditions.permissions;

        var fontNameE = (permissions == "All" || permissions == "Font" || permissions.indexOf("Font,") == 0 || permissions.indexOf(", Font,") >= 0 || jsObject.EndsWith(permissions, "Font"));
        var fontSizeE = (permissions == "All" || permissions.indexOf("FontSize") >= 0);
        var fontBoldE = (permissions == "All" || permissions.indexOf("FontStyleBold") >= 0);
        var fontItalicE = (permissions == "All" || permissions.indexOf("FontStyleItalic") >= 0);
        var fontUnderlineE = (permissions == "All" || permissions.indexOf("FontStyleUnderline") >= 0);
        var fontStrikeoutE = (permissions == "All" || permissions.indexOf("FontStyleStrikeout") >= 0);
        var textColorE = (permissions == "All" || permissions.indexOf("TextColor") >= 0);
        var backColorE = (permissions == "All" || permissions.indexOf("BackColor") >= 0);

        lbAbcText.style.fontFamily = fontNameE ? font.name : "Arial";
        lbAbcText.style.fontSize = (fontSizeE ? font.size : 8) + "pt";
        lbAbcText.style.fontWeight = fontBoldE && font.bold == "1" ? "bold" : "normal";
        lbAbcText.style.fontStyle = fontItalicE && font.italic == "1" ? "italic" : "normal";
        var decoration = "";
        if (fontUnderlineE && font.underline == "1") decoration = "underline ";
        if (fontStrikeoutE && font.strikeout == "1") decoration += "line-through";
        lbAbcText.style.textDecoration = decoration;

        lbAbcText.style.color = textColorE ? (conditions.textColor == "transparent" ? "transparent" : "rgb(" + conditions.textColor + ")") : "black";
        lbAbc.style.background = backColorE ? (conditions.backColor == "transparent" ? "transparent" : "rgb(" + conditions.backColor + ")") : "transparent";

        fontName.setEnabled(fontNameE);
        fontSize.setEnabled(fontSizeE);
        buttonBold.setEnabled(fontBoldE);
        buttonItalic.setEnabled(fontItalicE);
        buttonStrikeout.setEnabled(fontStrikeoutE);
        buttonUnderline.setEnabled(fontUnderlineE);
        textColor.setEnabled(textColorE);
        backColor.setEnabled(backColorE);

        iconColor.setEnabled(iconAlign.key != "None");
        iconControl.setEnabled(iconAlign.key != "None");
        iconControl.textBox.style.color = iconColor.key == "transparent" ? "transparent" : "rgb(" + iconColor.key + ")";
    }

    item.getCondition = function () {
        return {
            keyValueMeter: item.controls.keyValueMeter.key,
            dataType: item.controls.dataType.key,
            condition: item.controls.condition.key,
            value: StiBase64.encode(item.controls.dataType.key == "DateTime" ? jsObject.DateToStringAmericanFormat(item.controls.valueDate.key, true) : item.controls.value.textBox.value),
            font: jsObject.FontObjectToStr({
                name: fontName.key,
                size: fontSize.key,
                bold: buttonBold.isSelected ? "1" : "0",
                italic: buttonItalic.isSelected ? "1" : "0",
                underline: buttonUnderline.isSelected ? "1" : "0",
                strikeout: buttonStrikeout.isSelected ? "1" : "0",
            }),
            iconColor: iconColor.key,
            textColor: textColor.key,
            backColor: backColor.key,
            permissions: permissions.key,
            customIcon: customIconControl.src,
            icon: iconControl.key,
            iconAlignment: iconAlign.key
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