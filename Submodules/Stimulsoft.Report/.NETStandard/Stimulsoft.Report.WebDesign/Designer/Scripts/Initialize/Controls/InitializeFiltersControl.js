
StiMobileDesigner.prototype.FilterControl = function (name, columns, widthContainer, heightContainer, isConditionsFilter) {
    var filterControl = document.createElement("div");
    filterControl.name = name;

    var jsObject = filterControl.jsObject = this;
    filterControl.columns = columns;
    filterControl.currentDataSourceName = null;
    filterControl.isConditionsFilter = isConditionsFilter;
    filterControl.controls = {};

    //ToolBar
    var toolBar = filterControl.controls.toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "4px";
    toolBar.style.width = "calc(100% - 8px)";
    filterControl.appendChild(toolBar);

    var controlProps = [
        ["addFilter", this.FormButton(null, null, this.loc.FormBand.AddFilter.replace("&", ""))],
        ["moveUp", this.StandartSmallButton(null, null, null, "Arrows.ArrowUpBlue.png")],
        ["moveDown", this.StandartSmallButton(null, null, null, "Arrows.ArrowDownBlue.png")],
        ["filterMode", this.DropDownList(name + "_filterMode", 60, null, this.GetFilterTypeItems(), true)],
        ["filterOn", this.CheckBox(null, this.loc.PropertyMain.FilterOn)]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        var control = controlProps[i][1];
        control.style.margin = controlProps[i][0] == "filterMode" ? "0 16px 0 0" : "0 6px 0 0";
        filterControl.controls[controlProps[i][0]] = control;

        if (controlProps[i][0] == "filterMode") {
            toolBar.addCell();
        }

        if (controlProps[i][0] == "filterMode") {
            var textCell = control.textCell = toolBar.addTextCell(this.loc.PropertyMain.FilterMode);
            textCell.className = "stiDesignerCaptionControls";
            textCell.style.padding = "1px 4px 0 0";
        }

        toolBar.addCell(control).style.width = "1px";
    }

    filterControl.controls.moveUp.setEnabled(false);
    filterControl.controls.moveDown.setEnabled(false);

    filterControl.controls.moveUp.action = function () {
        if (filterContainer.selectedItem) { filterContainer.selectedItem.move("Up"); }
    }

    filterControl.controls.moveDown.action = function () {
        if (filterContainer.selectedItem) { filterContainer.selectedItem.move("Down"); }
    }

    if (!this.options.isTouchDevice) {
        filterControl.controls.moveUp.style.display = filterControl.controls.moveDown.style.display = "none";
    }

    //Container
    var filterContainer = filterControl.controls.filterContainer = this.FilterContainer(filterControl);
    filterControl.appendChild(filterContainer);

    if (widthContainer) filterContainer.style.width = widthContainer + "px";
    if (heightContainer) filterContainer.style.height = heightContainer + "px";

    filterControl.controls.addFilter.action = function () {
        var filterItem = filterContainer.addFilter(jsObject.FilterObject());
        filterItem.setSelected(true);
        filterContainer.onAction();
    }

    filterControl.fill = function (filters, filterOn, filterMode) {
        filterContainer.clear();
        filterControl.controls.filterOn.setChecked(filterOn);
        filterControl.controls.filterMode.setKey(filterMode);

        if (!filters) return;
        for (var i = 0; i < filters.length; i++) {
            filterContainer.addFilter(filters[i], true);
        }

        filterContainer.onAction();
    }

    filterControl.getValue = function () {
        var result = {
            filters: [],
            filterOn: filterControl.controls.filterOn.isChecked,
            filterMode: filterControl.controls.filterMode.key
        }
        for (var num = 0; num < filterContainer.childNodes.length; num++) {
            var filterItem = filterContainer.childNodes[num];
            if (filterItem.key) {
                var filter = jsObject.FilterObject();
                result.filters.push(filter);
                filter.fieldIs = filterItem.fieldIs.key;
                filter.column = filterItem.column.textBox.value != "[" + jsObject.loc.FormFormatEditor.nameNo + "]"
                    ? (filterItem.column.key || filterItem.column.textBox.value) : "";
                filter.dataType = filterItem.dataType.key;
                filter.condition = filterItem.condition.key;
                filter.value1 = filter.dataType == "DateTime" ? jsObject.DateToStringAmericanFormat(filterItem.value1Date.key, true) : filterItem.value1.textBox.value;
                filter.value2 = filter.dataType == "DateTime" ? jsObject.DateToStringAmericanFormat(filterItem.value2Date.key, true) : filterItem.value2.textBox.value;
                filter.expression = filterItem.expression.textBox.value;
            }
        }

        return result;
    }

    return filterControl;
}

StiMobileDesigner.prototype.FilterContainer = function (filterControl) {
    var filterContainer = document.createElement("div");
    var jsObject = filterContainer.jsObject = this;
    filterContainer.className = "stiDesignerFilterContainer";
    filterContainer.style.position = "relative";
    filterContainer.selectedItem = null;
    filterContainer.filterControl = filterControl;

    filterContainer.addFilter = function (filter, notAction) {
        var filterItem = jsObject.FilterItem(filterContainer);
        this.appendChild(filterItem);
        if (!notAction) filterContainer.onAction();

        filterItem.fieldIs.setKey(filter.fieldIs);
        filterItem.dataType.setKey(filter.dataType);

        var noneText = jsObject.loc.FormFormatEditor.nameNo;

        if (filterControl.columns != null)
            filterItem.column.setKey(filter.column || "[" + noneText + "]");
        else
            filterItem.column.textBox.value = filter.column || "[" + noneText + "]";

        if (filter.dataType == "DateTime") {
            filterItem.value1Date.setKey(filter.value1 ? new Date(filter.value1) : new Date());
            filterItem.value2Date.setKey(filter.value2 ? new Date(filter.value2) : new Date());
        }
        else {
            filterItem.value1.textBox.value = filter.value1;
            filterItem.value2.textBox.value = filter.value2;
        }
        filterItem.expression.textBox.value = filter.expression;
        filterItem.condition.setKey(filter.condition);
        filterItem.updateControls();
        filterItem.condition.setKey(filter.condition);

        return filterItem;
    }

    filterContainer.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        this.onAction();
    }

    filterContainer.getCountItems = function () {
        var itemsCount = 0;
        for (var i = 0; i < this.childNodes.length; i++) {
            if (this.childNodes[i].key) itemsCount++;
        }
        return itemsCount;
    }

    filterContainer.getOverItemIndex = function () {
        for (var i = 0; i < this.childNodes.length; i++)
            if (this.childNodes[i].isOver) return i;

        return null;
    }

    filterContainer.getItemIndex = function (item) {
        for (var i = 0; i < this.childNodes.length; i++)
            if (this.childNodes[i] == item) return i;

        return null;
    }

    filterContainer.getSelectedItemIndex = function () {
        return this.selectedItem ? this.getItemIndex(this.selectedItem) : null;
    }

    filterContainer.moveItem = function (fromIndex, toIndex) {
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

    filterContainer.onmouseup = function (event) {
        if (event.button != 2 && jsObject.options.itemInDrag) {
            var itemObject = jsObject.CopyObject(jsObject.options.itemInDrag.itemObject);
            if (!itemObject) return;
            var typeItem = itemObject.typeItem;

            if (typeItem == "FilterItem") {
                var toIndex = this.getOverItemIndex();
                var fromIndex = this.getSelectedItemIndex();
                if (toIndex != null && fromIndex != null && fromIndex != toIndex) {
                    filterContainer.moveItem(fromIndex, toIndex);
                    filterContainer.onAction();
                }
            }
        }

        return false;
    }

    filterContainer.onAction = function () {
        var count = filterContainer.getCountItems();
        var index = filterContainer.selectedItem ? filterContainer.selectedItem.getIndex() : -1;
        filterControl.controls.moveUp.setEnabled(index > 0);
        filterControl.controls.moveDown.setEnabled(index != -1 && index < count - 1);
        filterControl.controls.filterMode.style.display = filterControl.controls.filterMode.textCell.style.display = count > 1 ? "" : "none";

        this.checkEmptyPanel();
    }

    filterContainer.checkEmptyPanel = function () {
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
                emptyPanel = jsObject.EmptyTextPanel("BigFilter.png", jsObject.loc.FormBand.NoFilters, "0.5", { width: 24, height: 24 });
                this.appendChild(emptyPanel);
            }
        }
    }

    return filterContainer;
}

StiMobileDesigner.prototype.FilterItem = function (filterContainer) {
    var filterItem = document.createElement("div");
    var jsObject = filterItem.jsObject = this;
    filterItem.key = this.generateKey();
    filterItem.isSelected = false;
    filterItem.className = "stiDesignerFilterPanel";

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "100%";
    filterItem.appendChild(mainTable);
    var mainCell = mainTable.addCell();
    filterItem.innerTable = this.CreateHTMLTable();
    filterItem.innerTable.style.margin = "6px 0 0 6px";
    mainCell.appendChild(filterItem.innerTable);

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

    //Captions
    filterItem.fieldIsCaption = filterItem.innerTable.addCell();
    filterItem.fieldIsCaption.innerHTML = this.loc.PropertyMain.FieldIs;

    filterItem.dataTypeCaption = filterItem.innerTable.addCell();
    filterItem.dataTypeCaption.innerHTML = this.loc.PropertyMain.DataType;

    filterItem.columnCaption = filterItem.innerTable.addCell();
    filterItem.columnCaption.innerHTML = this.loc.PropertyMain.Column;

    //FieldIs
    filterItem.fieldIs = this.DropDownList(null, 115, null, this.GetFilterFieldIsItems(), true, false);
    filterItem.fieldIs.style.margin = "3px 7px 3px 0";
    filterItem.innerTable.addCellInNextRow(filterItem.fieldIs);
    filterItem.fieldIs.action = function () { filterItem.updateControls(); };

    //Data Type
    filterItem.dataType = this.DropDownList(null, 115, null, this.GetFilterDataTypeItems(), true, false);
    filterItem.dataType.style.margin = "3px 7px 3px 0";
    filterItem.innerTable.addCellInLastRow(filterItem.dataType);
    filterItem.dataType.action = function () { filterItem.updateControls(); };

    //Column
    if (filterContainer.filterControl && filterContainer.filterControl.columns) {
        filterItem.column = this.DropDownList(null, 190, null, filterContainer.filterControl.columns, true, false);
        filterItem.column.style.margin = "3px 7px 3px 0";
        filterItem.innerTable.addCellInLastRow(filterItem.column);
    }
    else if (filterContainer.filterControl.isConditionsFilter) {
        filterItem.column = this.DataControl(null, 190, true);
        filterItem.column.style.margin = "3px 7px 3px 0";
        filterItem.innerTable.addCellInLastRow(filterItem.column);
    }
    else {
        filterItem.column = this.TextBoxWithEditButton(null, 199);
        filterItem.column.style.margin = "3px 7px 3px 0";
        filterItem.innerTable.addCellInLastRow(filterItem.column);
        filterItem.column.button.action = function () {

            var currentDataSourceName = (filterContainer.filterControl.currentDataSourceName != null)
                ? filterContainer.filterControl.currentDataSourceName
                : jsObject.options.selectedObject.properties["dataSource"];
            this.key = currentDataSourceName + "." + this.textBox.value;
            var this_ = this;

            jsObject.InitializeDataColumnForm(function (dataColumnForm) {
                dataColumnForm.dataTree.build("Column", currentDataSourceName);
                dataColumnForm.needBuildTree = false;
                dataColumnForm.parentButton = this_;
                dataColumnForm.changeVisibleState(true);
                dataColumnForm.action = function () {
                    this.changeVisibleState(false);
                    var columnName = this.dataTree.key ? this.dataTree.key.substring(this.dataTree.key.indexOf(".") + 1, this.dataTree.key.length) : null;
                    var selectedItem = this.dataTree.selectedItem;
                    this.parentButton.textBox.value = this.dataTree.key != ""
                        ? (selectedItem.parent && selectedItem.parent.itemObject.typeItem == "BusinessObject" && currentDataSourceName ? (currentDataSourceName + "." + columnName) : columnName)
                        : "[" + jsObject.loc.FormFormatEditor.nameNo + "]";
                }
            });
        }
    }

    filterItem.innerTable2 = this.CreateHTMLTable();
    filterItem.innerTable2.style.margin = "0 0 6px 6px";
    mainCell.appendChild(filterItem.innerTable2);

    //Condition
    filterItem.condition = this.DropDownList(null, 115, null, null, true, false);
    filterItem.condition.style.margin = "3px 7px 3px 0";
    filterItem.innerTable2.addCell(filterItem.condition);
    filterItem.condition.action = function () { filterItem.updateControls(); };

    //Value1
    filterItem.value1 = this.ExpressionControl(null, 160, null, null, true);
    filterItem.value1.style.margin = "3px 7px 3px 0";
    filterItem.innerTable2.addCell(filterItem.value1);

    filterItem.value1Date = this.DateControl(null, 143, null, true);
    filterItem.value1Date.style.margin = "3px 7px 3px 0";
    filterItem.innerTable2.addCell(filterItem.value1Date);

    //And Caption
    filterItem.andCaption = filterItem.innerTable2.addCell();
    filterItem.andCaption.innerHTML = this.loc.PropertyEnum.StiFilterModeAnd;

    //Value2
    filterItem.value2 = this.ExpressionControl(null, 160, null, null, true);
    filterItem.value2.style.margin = "3px 7px 3px 7px";
    filterItem.innerTable2.addCell(filterItem.value2);

    filterItem.value2Date = this.DateControl(null, 143, null, true);
    filterItem.value2Date.style.margin = "3px 7px 3px 7px";
    filterItem.innerTable2.addCell(filterItem.value2Date);

    filterItem.innerTable3 = this.CreateHTMLTable();
    filterItem.innerTable3.style.margin = "0 0 3px 5px";
    mainCell.appendChild(filterItem.innerTable3);

    //Expression
    filterItem.expression = this.ExpressionControl(null, filterContainer.filterControl.isConditionsFilter ? 600 : 450, null, null, true, false);
    filterItem.expression.style.margin = "3px 7px 3px 1px";
    filterItem.innerTable3.addCell(filterItem.expression);

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
            if (filterContainer.getCountItems() > 1) {
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
        this.condition.items = jsObject.GetFilterConditionItems(this.dataType.key);
        if (!this.condition.haveKey(this.condition.key) && this.condition.items != null && this.condition.items.length > 0) this.condition.setKey(this.condition.items[0].key);
        this.dataType.style.display = (this.fieldIs.key == "Value") ? "" : "none";
        this.dataTypeCaption.style.display = (this.fieldIs.key == "Value") ? "" : "none";
        this.column.style.display = (this.fieldIs.key == "Value") ? "" : "none";
        this.columnCaption.style.display = (this.fieldIs.key == "Value") ? "" : "none";
        this.condition.style.display = (this.fieldIs.key == "Value") ? "" : "none";
        this.value1.style.display = (this.fieldIs.key == "Value" && this.dataType.key != "DateTime" && this.condition.key != "IsNull" && this.condition.key != "IsNotNull") ? "" : "none";
        this.value1Date.style.display = (this.fieldIs.key == "Value" && this.dataType.key == "DateTime" && this.condition.key != "IsNull" && this.condition.key != "IsNotNull") ? "" : "none";
        this.andCaption.style.display = (this.fieldIs.key == "Value" && (this.condition.key == "Between" || this.condition.key == "NotBetween")) ? "" : "none";
        this.value2.style.display = (this.fieldIs.key == "Value" && this.dataType.key != "DateTime" && (this.condition.key == "Between" || this.condition.key == "NotBetween")) ? "" : "none";
        this.value2Date.style.display = (this.fieldIs.key == "Value" && this.dataType.key == "DateTime" && (this.condition.key == "Between" || this.condition.key == "NotBetween")) ? "" : "none";
        this.expression.style.display = (this.fieldIs.key == "Expression") ? "" : "none";
        var showExpButton = this.dataType.key == "Expression" && !filterContainer.filterControl.columns;
        this.value1.button.style.display = this.value2.button.style.display = showExpButton ? "" : "none";
        this.value1.button.parentElement.style.width = this.value2.button.parentElement.style.width = showExpButton ? "auto" : "3px";
    }

    return filterItem;
}

StiMobileDesigner.prototype.FilterObject = function () {
    var filter = {};
    filter.fieldIs = "Value";
    filter.column = "[" + this.loc.FormFormatEditor.nameNo + "]";
    filter.dataType = "String";
    filter.condition = "EqualTo";
    filter.value1 = "";
    filter.value2 = "";
    filter.expression = "";

    return filter;
}

StiMobileDesigner.prototype.EmptyTextPanel = function (imageName, text, imageOpacity, imageSizes) {
    var panel = this.CreateHTMLTable();
    panel.className = "stiEmptyTextPanel";

    if (imageName) {
        var img = document.createElement("img");
        img.style.width = (imageSizes ? imageSizes.width : 32) + "px";
        img.style.height = (imageSizes ? imageSizes.height : 32) + "px";
        img.style.filter = "grayscale(100%)";
        panel.addCell(img).style.textAlign = "center";
        StiMobileDesigner.setImageSource(img, this.options, imageName);
        if (imageOpacity) img.style.opacity = imageOpacity;
    }
    if (text) {
        var textCell = panel.addTextCellInNextRow(text);
        textCell.className = "stiCreateDataHintText";
        textCell.style.padding = "10px 0 0 0";
    }

    return panel;
}