
StiMobileDesigner.prototype.FilterRulesControl = function (name, width, height) {
    var filterControl = document.createElement("div");
    var jsObject = this;
    filterControl.controls = {};
    filterControl.name = name;
    filterControl.columnObject = null;

    //ToolBar
    var toolBar = filterControl.controls.toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "4px";
    toolBar.style.width = "calc(100% - 8px)";
    filterControl.appendChild(toolBar);

    var controlProps = [
        ["addFilter", this.FormButton(null, null, this.loc.FormBand.AddFilter.replace("&", ""))],
        ["separator"],
        ["moveUp", this.StandartSmallButton(null, null, null, "Arrows.ArrowUpBlue.png")],
        ["moveDown", this.StandartSmallButton(null, null, null, "Arrows.ArrowDownBlue.png")],
        ["filterOn", this.CheckBox(null, this.loc.PropertyMain.FilterOn)]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        if (controlProps[i][0].indexOf("separator") >= 0) {
            filterControl.controls[controlProps[i][0]] = this.HomePanelSeparator();
            toolBar.addCell(filterControl.controls[controlProps[i][0]]);
            continue;
        }
        var control = controlProps[i][1];
        control.style.margin = "0 6px 0 0";
        filterControl.controls[controlProps[i][0]] = control;

        if (controlProps[i][0] == "filterOn") {
            toolBar.addCell();
        }

        toolBar.addCell(control).style.width = "1px";
    }

    //Container
    var container = this.FilterRulesContainer(filterControl);
    filterControl.appendChild(container);

    container.style.width = (width ? width : 600) + "px";
    container.style.height = (height ? height : 300) + "px";

    if (!this.options.isTouchDevice) {
        filterControl.controls.moveUp.style.display = filterControl.controls.moveDown.style.display = filterControl.controls.separator.style.display = "none";
    }

    filterControl.controls.addFilter.action = function () {
        container.addFilterRule(jsObject.DataFilterRuleObject(filterControl.columnObject.key, filterControl.columnObject.path, "EqualTo",
            filterControl.columnObject.type == "bool" ? "False" : null, null, filterControl.controls.filterOn.isChecked));
    }

    filterControl.controls.moveUp.setEnabled(false);
    filterControl.controls.moveDown.setEnabled(false);

    filterControl.controls.moveUp.action = function () {
        if (container.selectedItem) {
            container.selectedItem.move("Up");
        }
    }

    filterControl.controls.moveDown.action = function () {
        if (container.selectedItem) {
            container.selectedItem.move("Down");
        }
    }

    filterControl.controls.filterOn.action = function () {
        for (var i = 0; i < container.childNodes.length; i++) {
            var filterItem = container.childNodes[i];
            filterItem.filterRule.isEnabled = this.isChecked;
        }
    }

    filterControl.setFilterRules = function (filterRules) {
        container.clear();
        filterControl.controls.filterOn.setChecked(true);

        if (!filterRules) return;

        for (var i = 0; i < filterRules.length; i++) {
            container.addFilterRule(filterRules[i], true);
            if (i == 0) {
                filterControl.controls.filterOn.setChecked(filterRules[i].isEnabled);
            }
        }

        container.onAction();
    }

    filterControl.getFilterRules = function () {
        var filterRules = [];

        for (var i = 0; i < container.childNodes.length; i++) {
            filterRules.push(container.childNodes[i].filterRule)
        }

        return filterRules;
    }

    return filterControl;
}

StiMobileDesigner.prototype.FilterRulesContainer = function (filterControl) {
    var container = document.createElement("div");
    var jsObject = this;
    container.className = "stiDesignerFilterContainer";
    container.selectedItem = null;
    container.filterControl = filterControl;

    container.addFilterRule = function (filterRule, notAction) {
        var filterItem = jsObject.FilterRuleItem(container, filterRule);
        this.appendChild(filterItem);
        if (!notAction) container.onAction();
        if (filterItem.valueControl && filterItem.valueControl.textBox && filterItem.valueControl.textBox["focus"] && !filterItem.valueControl.readOnly) {
            filterItem.valueControl.textBox.focus();
        }
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
        var count = container.getCountItems();
        var index = container.selectedItem ? container.selectedItem.getIndex() : -1;
        filterControl.controls.moveUp.setEnabled(index > 0);
        filterControl.controls.moveDown.setEnabled(index != -1 && index < count - 1);
    }

    return container;
}

StiMobileDesigner.prototype.FilterRuleItem = function (filterContainer, filterRule) {
    var filterItem = document.createElement("div");
    var columnObject = filterContainer.filterControl.columnObject;
    var jsObject = this;
    filterItem.key = this.generateKey();
    filterItem.isSelected = false;
    filterItem.filterRule = filterRule;
    filterItem.className = "stiDesignerFilterPanel";

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "100%";
    filterItem.appendChild(mainTable);
    var mainCell = mainTable.addCell();
    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "6px";
    mainCell.appendChild(innerTable);

    //Remove Button
    var removeButton = this.StandartSmallButton(null, null, null, "RemoveBlack.png");
    removeButton.style.margin = "2px 2px 2px 0px";
    removeButton.style.visibility = "hidden";
    removeButton.style.display = "inline-block";
    filterItem.removeButton = removeButton;
    var closeCell = mainTable.addCell(removeButton);
    closeCell.style.width = "1px";
    closeCell.style.verticalAlign = "top";

    removeButton.action = function () {
        filterItem.remove();
    }

    var conditionItems = [];

    if (jsObject.ColumnIsNumericType(columnObject.type)) {
        conditionItems = jsObject.GetFilterConditionItems("Numeric", false);
    }
    else if (jsObject.ColumnIsDateType(columnObject.type)) {
        conditionItems = jsObject.GetFilterConditionItems("DateTime", false);
    }
    else if (columnObject.type == "bool") {
        conditionItems = jsObject.GetFilterConditionItems("Boolean", false);
    }
    else {
        conditionItems = jsObject.GetFilterConditionItems("String", false, true);
    }

    //Condition
    var conditionControl = this.DropDownList(null, 145, null, conditionItems, true, false);
    conditionControl.style.marginRight = "7px";
    innerTable.addCell(conditionControl);
    conditionControl.setKey(filterRule.condition);

    conditionControl.action = function () {
        filterRule.condition = this.key;
        filterItem.updateControls();
    };

    //Value
    var valueControl;

    if (jsObject.ColumnIsDateType(columnObject.type)) {
        valueControl = this.DateControl(null, 130, null, true);

        if (!filterRule.isExpression) {
            valueControl.setKey(filterRule.value ? new Date(filterRule.value) : new Date());
            if (!filterRule.value) filterRule.value = jsObject.formatDate(new Date(), "MM/dd/yyyy");
        }
        else {
            valueControl.textBox.value = filterRule.value;
        }

        valueControl.action = function () {
            filterRule.value = !filterRule.isExpression
                ? jsObject.formatDate(this.key, "MM/dd/yyyy")
                : this.textBox.value;
        }
    }
    else if (columnObject.type == "bool") {
        valueControl = this.DropDownList(null, 160, null, this.GetBoolItems());
        valueControl.setKey(filterRule.value);

        valueControl.action = function () {
            filterRule.value = this.key;
        }
    }
    else {
        valueControl = this.DropDownList(null, 130, null, filterContainer.filterControl.filterItems);
        valueControl.textBox.value = filterRule.value || "";

        valueControl.action = function () {
            filterRule.value = this.textBox.value;
        }
    }

    valueControl.style.marginRight = "7px";
    innerTable.addCell(valueControl);
    filterItem.valueControl = valueControl;

    //And Caption
    var andCaption = innerTable.addTextCell(this.loc.PropertyEnum.StiFilterModeAnd);

    //Value2
    var value2Control;

    if (jsObject.ColumnIsDateType(columnObject.type)) {
        value2Control = this.DateControl(null, 13, null, true);

        if (!filterRule.isExpression) {
            value2Control.setKey(filterRule.value2 ? new Date(filterRule.value2) : new Date());
            if (!filterRule.value2) filterRule.value2 = jsObject.formatDate(new Date(), "MM/dd/yyyy");
        }
        else {
            value2Control.textBox.value = filterRule.value2;
        }

        value2Control.action = function () {
            filterRule.value2 = !filterRule.isExpression
                ? jsObject.formatDate(this.key, "MM/dd/yyyy")
                : this.textBox.value;
        }
    }
    else {
        value2Control = this.DropDownList(null, 130, null, filterContainer.filterControl.filterItems);
        value2Control.textBox.value = filterRule.value2 || "";

        value2Control.action = function () {
            filterRule.value2 = this.textBox.value;
        }
    }

    value2Control.style.margin = "0 7px 0 7px";
    innerTable.addCell(value2Control);

    //Expression
    var isExpressionControl = this.CheckBox(null, this.loc.PropertyMain.Expression);
    isExpressionControl.style.marginRight = "7px";
    innerTable.addCell(isExpressionControl);
    isExpressionControl.setChecked(filterRule.isExpression);

    isExpressionControl.action = function () {
        filterRule.isExpression = this.isChecked;
        filterItem.updateControls();
        if (!this.isChecked) {
            if (columnObject.type == "bool") {
                valueControl.setKey("False");
            }
            else if (jsObject.ColumnIsDateType(columnObject.type)) {
                valueControl.setKey(new Date());
            }
            else {
                valueControl.textBox.value = "";
                valueControl.textBox.focus();
            }
        }
        else {
            valueControl.textBox.value = "";
        }
    }

    filterItem.setSelected = function (state) {
        if (state) {
            if (filterContainer.selectedItem) {
                filterContainer.selectedItem.setSelected(false);
            }
            filterContainer.selectedItem = this;
        }
        else {
            if (filterContainer.selectedItem && filterContainer.selectedItem == this) {
                filterContainer.selectedItem = null;
            }
        }
        this.className = state ? "stiDesignerFilterPanelSelected" : "stiDesignerFilterPanel";
        this.isSelected = state;
        this.removeButton.style.visibility = state ? "visible" : "hidden";
    }

    filterItem.remove = function () {
        if (filterContainer.selectedItem == this) {
            var prevItem = this.previousSibling;
            var nextItem = this.nextSibling;
            filterContainer.selectedItem = null;
            if (filterContainer.childNodes.length > 1) {
                if (nextItem) {
                    nextItem.setSelected(true);
                    filterContainer.selectedItem = nextItem;
                }
                else if (prevItem) {
                    prevItem.setSelected(true);
                    filterContainer.selectedItem = prevItem;
                }
            }
        }
        filterContainer.removeChild(this);
        filterContainer.onAction();
    }

    filterItem.getIndex = function () {
        for (var i = 0; i < filterContainer.childNodes.length; i++)
            if (filterContainer.childNodes[i] == this) return i;
    };

    filterItem.move = function (direction) {
        var index = this.getIndex();
        filterContainer.removeChild(this);
        var count = filterContainer.getCountItems();
        var newIndex = direction == "Up" ? index - 1 : index + 1;
        if (direction == "Up" && newIndex == -1) newIndex = 0;
        if (direction == "Down" && newIndex >= count) {
            filterContainer.appendChild(this);
            filterContainer.onAction();
            return;
        }
        filterContainer.insertBefore(this, filterContainer.childNodes[newIndex]);
        filterContainer.onAction();
    }

    if (jsObject.options.isTouchDevice) {
        filterItem.onclick = function () {
            if (!this.parentElement) return;
            this.setSelected(true);
        }
    }
    else {
        filterItem.onmousedown = function (event) {
            this.setSelected(true);

            if (this.isTouchStartFlag || (event && event.target && event.target.nodeName && event.target.nodeName.toLowerCase() == "input")) return;
            event.preventDefault();

            if (event.button != 2) {
                var itemInDrag = jsObject.TreeItemForDragDrop({ name: jsObject.loc.PropertyMain.Filter, typeItem: "FilterItem" }, null, true);
                if (itemInDrag.button.captionCell) itemInDrag.button.captionCell.style.padding = "5px 20px 5px 10px";
                itemInDrag.beginingOffset = 0;
                jsObject.options.itemInDrag = itemInDrag;
            }
        }

        filterItem.onmouseover = function () {
            this.isOver = true;
            if (jsObject.options.itemInDrag && jsObject.options.itemInDrag.itemObject) {
                var typeItem = jsObject.options.itemInDrag.itemObject.typeItem;
                if (typeItem == "FilterItem") {
                    this.style.borderStyle = "dashed";
                    this.style.borderColor = jsObject.options.themeColors[jsObject.GetThemeColor()];
                }
            }
        }

        filterItem.onmouseout = function () {
            this.isOver = false;
            this.style.borderStyle = "solid";
            this.style.borderColor = "";
        }
    }

    filterItem.updateControls = function () {
        var showValue2 = filterRule.condition == "Between" || filterRule.condition == "NotBetween";
        var showValues = filterRule.condition != "IsNull" && filterRule.condition != "IsNotNull" && filterRule.condition != "IsBlank" && filterRule.condition != "IsNotBlank";
        valueControl.style.display = showValues ? "" : "none";
        value2Control.style.display = showValues && showValue2 ? "" : "none";
        andCaption.style.display = showValue2 ? "" : "none";
        isExpressionControl.style.display = showValues ? "" : "none";

        valueControl.button.parentElement.style.display = !filterRule.isExpression ? "" : "none";
        value2Control.button.parentElement.style.display = !filterRule.isExpression ? "" : "none";

        if (columnObject.type == "bool") {
            valueControl.readOnly = valueControl.textBox.readOnly = !filterRule.isExpression;
        }
        else if (jsObject.ColumnIsDateType(columnObject.type)) {
            valueControl.textBox.readOnly = !filterRule.isExpression;
            value2Control.textBox.readOnly = !filterRule.isExpression;
        }
    }

    filterItem.updateControls();

    return filterItem;
}