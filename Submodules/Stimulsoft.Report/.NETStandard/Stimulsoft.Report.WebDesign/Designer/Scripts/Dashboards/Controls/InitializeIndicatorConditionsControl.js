
StiMobileDesigner.prototype.IndicatorConditionsControl = function (width, height) {
    var control = document.createElement("div");
    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "12px";
    control.appendChild(toolBar);

    //Add
    var addButton = control.addButton = this.FormButton(null, null, this.loc.Chart.AddCondition.replace("&", ""));
    toolBar.addCell(addButton);

    addButton.action = function () {
        var conditionObj = {
            field: "Value", condition: "EqualTo", value: "", font: "Arial!8!0!0!0!0", textColor: "0,0,0", backColor: "transparent",
            icon: "Rating4", customIcon: null, iconAlignment: "Right", iconColor: "68,114,196",
            targetIcon: "Rating4", targetIconAlignment: "Right", targetIconColor: "68,114,196",
            permissions: "Font, FontSize, FontStyleBold, FontStyleItalic, FontStyleUnderline, FontStyleStrikeout, TextColor, BackColor, Icon, TargetIcon"
        }
        control.container.addCondition(conditionObj);
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
        if (container.selectedItem) {
            control.container.selectedItem.move("Up");
        }
    }

    control.moveDownButton.action = function () {
        if (container.selectedItem) {
            control.container.selectedItem.move("Down");
        }
    }

    if (!this.options.isTouchDevice) {
        moveUpButton.style.display = moveDownButton.style.display = separator.style.display = "none";
    }

    //Container
    var container = control.container = this.IndicatorConditionsContainer(control);
    container.style.margin = "0 12px 12px 12px";
    control.appendChild(container);

    if (width) container.style.width = width + "px";
    if (height) container.style.height = height + "px";

    control.fill = function (conditions) {
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
                result.push(item.getCondition());
            }
        }
        return result;
    }

    return control;
}

StiMobileDesigner.prototype.IndicatorConditionsContainer = function (mainControl) {
    var jsObject = this;
    var container = document.createElement("div");
    mainControl.container = container;

    container.className = "stiSimpleContainerWithBorder";
    container.style.overflow = "auto";
    container.mainControl = mainControl;
    container.selectedItem = null;

    container.addCondition = function (conditionObject, notAction) {
        var item = jsObject.IndicatorConditionsItem(this);
        this.appendChild(item);
        this.checkEmptyPanel();

        item.controls.field.setKey(conditionObject.field);
        item.controls.condition.setKey(conditionObject.condition);
        item.controls.value.textBox.value = StiBase64.decode(conditionObject.value);
        var font = jsObject.FontStrToObject(conditionObject.font);
        item.controls.fontName.setKey(font.name);
        item.controls.fontSize.setKey(font.size);
        item.controls.buttonBold.setSelected(font.bold == "1");
        item.controls.buttonItalic.setSelected(font.italic == "1");
        item.controls.buttonUnderline.setSelected(font.underline == "1");
        item.controls.buttonStrikeout.setSelected(font.strikeout == "1");
        item.controls.textColor.setKey(conditionObject.textColor);
        item.controls.backColor.setKey(conditionObject.backColor);
        item.controls.customIconControl.setImage(conditionObject.customIcon);
        item.controls.iconControl.setKey(conditionObject.icon);
        item.controls.iconAlign.setKey(conditionObject.iconAlignment);
        item.controls.iconColor.setKey(conditionObject.iconColor);
        item.controls.targetIconControl.setKey(conditionObject.targetIcon);
        item.controls.targetIconAlign.setKey(conditionObject.targetIconAlignment);
        item.controls.targetIconColor.setKey(conditionObject.targetIconColor);
        item.controls.permissions.setKey(conditionObject.permissions);

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

StiMobileDesigner.prototype.IndicatorConditionsItem = function (container) {
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
    innerTable.addTextCell(jsObject.loc.PropertyMain.Condition);
    innerTable.addTextCell(jsObject.loc.PropertyMain.Value);

    //Field
    var field = this.DropDownList(null, 120, null, this.GetIndicatorConditionsFieldIsItems(), true, false);
    item.controls.field = field;
    field.style.margin = "3px 7px 3px 0";
    innerTable.addCellInNextRow(field);

    //Condition
    var condition = this.DropDownList(null, 130, null, this.GetFilterConditionItems("Numeric", true), true, false);
    item.controls.condition = condition;
    condition.style.margin = "3px 7px 3px 0";
    innerTable.addCellInLastRow(condition);

    field.action = function () {
        condition.addItems(jsObject.GetFilterConditionItems(field.key != "Series" ? "Numeric" : "String", true, false));
        if (!condition.getItemByKey(condition.key)) condition.setKey("EqualTo");
    }

    //Value
    var value = item.controls.value = this.ExpressionControl(null, 270, null, null, true);
    value.button.style.display = "none"; //temporary
    value.button.parentElement.style.width = "3px"; //temporary
    value.style.margin = "3px 0 3px 0";
    innerTable.addCellInLastRow(value);

    var innerTable2 = this.CreateHTMLTable();
    innerTable2.style.margin = "0 0 6px 12px";
    mainCell.appendChild(innerTable2);

    //Captions row2
    innerTable2.addTextCell(jsObject.loc.PropertyMain.Font);
    innerTable2.addTextCell(jsObject.loc.PropertyMain.Icon);
    innerTable2.addTextCell(jsObject.loc.PropertyMain.TargetIcon);
        
    //FontName
    var fontTable1 = this.CreateHTMLTable();
    innerTable2.addCellInNextRow(fontTable1).style.verticalAlign = "top";

    var fontName = this.FontList(null, this.options.isTouchDevice ? 210 : 160, null, true);
    fontName.style.margin = "3px 7px 3px 0";
    item.controls.fontName = fontName;
    fontTable1.addCell(fontName);

    fontName.action = function () {
        item.updateControls();
    };

    var fontSize = this.DropDownList(null, 45, null, this.GetFontSizeItems(), false);
    fontSize.style.margin = "3px 7px 3px 0";
    item.controls.fontSize = fontSize;
    fontTable1.addCell(fontSize);

    fontSize.action = function () {
        item.updateControls();
    };

    //Permissions
    var checkBoxes = [
        ["Font", this.loc.PropertyMain.FontName],
        ["FontSize", this.loc.PropertyMain.FontSize],
        ["FontStyleBold", this.loc.PropertyMain.FontBold],
        ["FontStyleItalic", this.loc.PropertyMain.FontItalic],
        ["FontStyleUnderline", this.loc.PropertyMain.FontUnderline],
        ["FontStyleStrikeout", this.loc.PropertyMain.FontStrikeout],
        ["TextColor", this.loc.PropertyMain.TextColor],
        ["BackColor", this.loc.PropertyMain.BackColor],
        ["Icon", this.loc.PropertyMain.Icon],
        ["TargetIcon", this.loc.PropertyMain.TargetIcon]
    ];
    var permissions = this.CheckBoxesControl(null, null, "Permissions.png", checkBoxes, true);
    permissions.style.margin = "3px 7px 3px 0";
    item.controls.permissions = permissions;
    fontTable1.addCell(permissions);

    permissions.action = function () {
        item.updateControls();
    };

    //IconAlign
    var iconAlign = this.DropDownList(null, 120, null, this.GetIconAlignmentItems(), true);
    iconAlign.style.margin = "3px 7px 3px 0";
    item.controls.iconAlign = iconAlign;
    innerTable2.addCellInLastRow(iconAlign);

    iconAlign.action = function () {
        item.updateControls();
    };

    //TargetIconAlign
    var targetIconAlign = this.DropDownList(null, 120, null, this.GetTargetIconAlignmentItems(), true);
    targetIconAlign.style.margin = "3px 0 3px 0";
    item.controls.targetIconAlign = targetIconAlign;
    innerTable2.addCellInLastRow(targetIconAlign);

    targetIconAlign.action = function () {
        item.updateControls();
    };

    //Font Buttons
    var fontTable2 = this.CreateHTMLTable();
    innerTable2.addCellInNextRow(fontTable2).style.verticalAlign = "top";

    var buttonBold = this.StandartSmallButton(null, null, null, "Bold.png");
    buttonBold.style.margin = "3px 7px 3px 0";
    item.controls.buttonBold = buttonBold;
    fontTable2.addCell(buttonBold);
    buttonBold.action = function () {
        this.setSelected(!this.isSelected);
        item.updateControls();
    };

    var buttonItalic = this.StandartSmallButton(null, null, null, "Italic.png");
    buttonItalic.style.margin = "3px 7px 3px 0";
    item.controls.buttonItalic = buttonItalic;
    fontTable2.addCell(buttonItalic);

    buttonItalic.action = function () {
        this.setSelected(!this.isSelected);
        item.updateControls();
    };

    var buttonUnderline = this.StandartSmallButton(null, null, null, "Underline.png");
    buttonUnderline.style.margin = "3px 7px 3px 0";
    item.controls.buttonUnderline = buttonUnderline;
    fontTable2.addCell(buttonUnderline);

    buttonUnderline.action = function () {
        this.setSelected(!this.isSelected);
        item.updateControls();
    };

    var buttonStrikeout = this.StandartSmallButton(null, null, null, "Strikeout.png");
    buttonStrikeout.style.margin = "3px 7px 3px 0";
    item.controls.buttonStrikeout = buttonStrikeout;
    fontTable2.addCell(buttonStrikeout);
    buttonStrikeout.action = function () {
        this.setSelected(!this.isSelected);
        item.updateControls();
    };

    var sep1 = this.HomePanelSeparator();
    sep1.style.margin = "3px 7px 3px 0";
    fontTable2.addCell(sep1);

    var textColor = this.ColorControlWithImage(null, "TextColor.png", null, true);
    textColor.style.margin = "3px 7px 3px 0";
    item.controls.textColor = textColor;
    fontTable2.addCell(textColor);

    textColor.action = function () {
        item.updateControls();
    };

    var backColor = this.ColorControlWithImage(null, "BackgroundColor.png", null, true);
    backColor.style.margin = "3px 7px 3px 0";
    item.controls.backColor = backColor;
    fontTable2.addCell(backColor);

    backColor.action = function () {
        item.updateControls();
    };   

    //Icon
    var iconCell = innerTable2.addCellInLastRow();
    iconCell.style.verticalAlign = "top";

    var customIconControl = this.ImageControl(null, 124, 62);
    customIconControl.style.margin = "6px 7px 3px 0";
    customIconControl.style.borderStyle = "solid";
    item.controls.customIconControl = customIconControl;
    iconCell.appendChild(customIconControl);
    customIconControl.action = function () {
        item.updateControls();
    };

    var iconControl = this.IconControl(null, 120, null, null, customIconControl);
    iconControl.style.margin = "3px 7px 0px 0";
    item.controls.iconControl = iconControl;
    iconCell.appendChild(iconControl);
    iconControl.action = function () {
        item.updateControls();
    };

    var iconColor = this.ColorControl(null, null, null, this.options.isTouchDevice ? 112 : 120, true);
    iconColor.style.margin = "6px 7px 3px 0";
    item.controls.iconColor = iconColor;
    iconCell.appendChild(iconColor);
    iconColor.action = function () {
        item.updateControls();
    };

    //TargetIcon
    var targetIconCell = innerTable2.addCellInLastRow();
    targetIconCell.style.verticalAlign = "top";

    var targetIconControl = this.IconControl(null, 120, null, null);
    targetIconControl.style.margin = "3px 0 0px 0";
    item.controls.targetIconControl = targetIconControl;
    targetIconCell.appendChild(targetIconControl);
    targetIconControl.action = function () {
        item.updateControls();
    };

    var targetIconColor = this.ColorControl(null, null, null, this.options.isTouchDevice ? 112 : 120, true);
    targetIconColor.style.margin = "6px 0 3px 0";
    item.controls.targetIconColor = targetIconColor;
    targetIconCell.appendChild(targetIconColor);
    targetIconColor.action = function () {
        item.updateControls();
    };

    item.updateControls = function () {
        customIconControl.style.display = iconAlign.key != "None" && customIconControl.src != null && customIconControl.src != "" ? "" : "none";
        iconColor.style.display = iconControl.style.display = customIconControl.style.display == "none" ? "" : "none";

        var conditions = item.getCondition();
        var font = jsObject.FontStrToObject(conditions.font);
        var permissions = conditions.permissions;

        var fontNameE = (permissions == "All" || permissions == "Font" || permissions.indexOf("Font,") == 0 || permissions.indexOf(", Font,") >= 0 || jsObject.EndsWith(permissions, " Font"));
        var fontSizeE = (permissions == "All" || permissions.indexOf("FontSize") >= 0);
        var fontBoldE = (permissions == "All" || permissions.indexOf("FontStyleBold") >= 0);
        var fontItalicE = (permissions == "All" || permissions.indexOf("FontStyleItalic") >= 0);
        var fontUnderlineE = (permissions == "All" || permissions.indexOf("FontStyleUnderline") >= 0);
        var fontStrikeoutE = (permissions == "All" || permissions.indexOf("FontStyleStrikeout") >= 0);
        var textColorE = (permissions == "All" || permissions.indexOf("TextColor") >= 0);
        var backColorE = (permissions == "All" || permissions.indexOf("BackColor") >= 0);
        var iconE = (permissions == "All" || permissions == "Icon" || permissions.indexOf("Icon,") == 0 || permissions.indexOf(", Icon,") >= 0 || jsObject.EndsWith(permissions, " Icon"));
        var targetIconE = (permissions == "All" || permissions.indexOf("TargetIcon") >= 0);

        fontName.setEnabled(fontNameE);
        fontSize.setEnabled(fontSizeE);
        buttonBold.setEnabled(fontBoldE);
        buttonItalic.setEnabled(fontItalicE);
        buttonStrikeout.setEnabled(fontStrikeoutE);
        buttonUnderline.setEnabled(fontUnderlineE);
        textColor.setEnabled(textColorE);
        backColor.setEnabled(backColorE);

        iconAlign.setEnabled(iconE);
        iconColor.setEnabled(iconAlign.key != "None" && iconE);
        iconControl.setEnabled(iconAlign.key != "None" && iconE);
        iconControl.textBox.style.color = iconColor.key == "transparent" ? "transparent" : "rgb(" + iconColor.key + ")";

        targetIconAlign.setEnabled(targetIconE);
        targetIconColor.setEnabled(targetIconAlign.key != "None" && targetIconE);
        targetIconControl.setEnabled(targetIconAlign.key != "None" && targetIconE);
        targetIconControl.textBox.style.color = targetIconColor.key == "transparent" ? "transparent" : "rgb(" + targetIconColor.key + ")";
    }

    item.getCondition = function () {
        return {
            field: item.controls.field.key,
            condition: item.controls.condition.key,
            value: StiBase64.encode(item.controls.value.textBox.value),
            font: jsObject.FontObjectToStr({
                name: fontName.key,
                size: fontSize.key,
                bold: buttonBold.isSelected ? "1" : "0",
                italic: buttonItalic.isSelected ? "1" : "0",
                underline: buttonUnderline.isSelected ? "1" : "0",
                strikeout: buttonStrikeout.isSelected ? "1" : "0"
            }),
            textColor: textColor.key,
            backColor: backColor.key,
            permissions: permissions.key,
            customIcon: customIconControl.src,
            icon: iconControl.key,
            iconAlignment: iconAlign.key,
            iconColor: iconColor.key,
            targetIcon: targetIconControl.key,
            targetIconAlignment: targetIconAlign.key,
            targetIconColor: targetIconColor.key
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