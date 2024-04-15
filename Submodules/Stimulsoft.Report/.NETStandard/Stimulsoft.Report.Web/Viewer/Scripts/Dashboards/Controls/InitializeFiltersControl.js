
StiJsViewer.prototype.FiltersControl = function (name, width, height) {
    var filterControl = document.createElement("div");
    var jsObject = this;
    filterControl.controls = {};
    filterControl.name = name;
    filterControl.columnObject = null;

    //ToolBar
    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "4px";
    filterControl.appendChild(toolBar);

    var controlProps = [
        ["addFilter", this.SmallButton(null, this.collections.loc["AddFilter"], "Dashboards.AddFilter.png")],
        ["separator"],
        ["moveUp", this.SmallButton(null, null, "Dashboards.MoveUp.png")],
        ["moveDown", this.SmallButton(null, null, "Dashboards.MoveDown.png")],
        ["separator"],
        ["filterOn", this.CheckBox(null, this.collections.loc["FilterOn"])]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        if (controlProps[i][0] == "separator") {
            toolBar.addCell(this.ToolBarSeparator());
            continue;
        }
        var control = controlProps[i][1];
        control.style.margin = "0 2px 0 2px";
        filterControl.controls[controlProps[i][0]] = control;
        if (controlProps[i][0] == "filterOn") {
            control.style.margin = "0 6px 0 6px";
        }
        toolBar.addCell(control);
    }

    //Container
    var container = this.FiltersContainer(filterControl);
    container.style.width = (width ? width : 600) + "px";
    container.style.height = (height ? height : 300) + "px";
    filterControl.appendChild(container);

    filterControl.controls.addFilter.action = function () {
        container.addFilter(jsObject.DataFilterObject(filterControl.columnObject.key, filterControl.columnObject.path, "EqualTo",
            filterControl.columnObject.dataType == "bool" ? "False" : null, null, filterControl.controls.filterOn.isChecked));
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
            filterItem.filter.isEnabled = this.isChecked;
        }
    }

    container.onAction = function () {
        var count = container.getCountItems();
        var index = container.selectedItem ? container.selectedItem.getIndex() : -1;
        filterControl.controls.moveUp.setEnabled(index > 0);
        filterControl.controls.moveDown.setEnabled(index != -1 && index < count - 1);
    }

    filterControl.setFilters = function (filters) {
        container.clear();
        filterControl.controls.filterOn.setChecked(true);

        if (!filters) return;

        for (var i = 0; i < filters.length; i++) {
            container.addFilter(filters[i], true);
            if (i == 0) {
                filterControl.controls.filterOn.setChecked(filters[i].isEnabled);
            }
        }

        container.onAction();
    }

    filterControl.getFilters = function () {
        var filters = [];

        for (var i = 0; i < container.childNodes.length; i++) {
            filters.push(container.childNodes[i].filter)
        }

        return filters;
    }

    return filterControl;
}

StiJsViewer.prototype.FiltersContainer = function (filterControl) {
    var container = document.createElement("div");
    var jsObject = this;
    container.className = "stiJsViewerFilterContainer";
    container.selectedItem = null;
    container.filterControl = filterControl;

    container.addFilter = function (filter, notAction) {
        var filterItem = jsObject.FilterItem(container, filter);
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

    container.onAction = function () { };

    return container;
}

StiJsViewer.prototype.FilterItem = function (filterContainer, filter) {
    var filterItem = document.createElement("div");
    var columnObject = filterContainer.filterControl.columnObject;
    var jsObject = this;
    filterItem.key = this.newGuid().replace(/-/g, '');
    filterItem.isSelected = false;
    filterItem.filter = filter;
    filterItem.className = "stiJsViewerFilterPanel";

    //Header
    var header = this.CreateHTMLTable();
    header.className = "stiJsViewerFilterPanelHeader";
    header.style.width = "100%";
    filterItem.appendChild(header);

    var headerButton = this.SmallButton(null, "", "CheckBox.png", null, null, null, null, { width: 12, height: 12 });
    headerButton.style.margin = "2px 0px 2px 2px";
    headerButton.image.style.visibility = "hidden";
    headerButton.caption.style.width = "100%";
    headerButton.caption.style.textAlign = "center";
    headerButton.action = function () { };
    header.addCell(headerButton).style.width = "100%";

    var removeButton = this.SmallButton(null, null, "RemoveItemButton.png");
    removeButton.style.margin = "2px 2px 2px 0px";
    header.addCell(removeButton);

    removeButton.action = function () {
        filterItem.remove();
    }

    header.onclick = function () {
        if (this.isTouchEndFlag || jsObject.options.isTouchClick) return;
        this.action();
    }

    header.ontouchend = function () {
        if (jsObject.options.fingerIsMoved) return;
        var this_ = this;
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);
        this.action();
        this.isTouchEndTimer = setTimeout(function () {
            this_.isTouchEndFlag = false;
        }, 1000);
    }

    header.action = function () {
        filterItem.setSelected(!filterItem.isSelected);
        filterContainer.onAction();
    }

    headerButton.oldonmouseenter = headerButton.onmouseenter;
    headerButton.oldonmouseleave = headerButton.onmouseleave;

    headerButton.onmouseenter = function () {
        headerButton.oldonmouseenter();
        removeButton.onmouseenter();
    }

    headerButton.onmouseleave = function () {
        headerButton.oldonmouseleave();
        removeButton.onmouseleave();
    }

    filterItem.setSelected = function (state) {
        if (state) {
            if (filterContainer.selectedItem) filterContainer.selectedItem.setSelected(false);
            filterContainer.selectedItem = this;
        }
        else {
            if (filterContainer.selectedItem && filterContainer.selectedItem == this) filterContainer.selectedItem = null;
        }
        filterItem.isSelected = state;
        headerButton.image.style.visibility = state ? "visible" : "hidden";
    }

    filterItem.remove = function () {
        filterContainer.removeChild(this);
        if (filterContainer.selectedItem == this) filterContainer.selectedItem = null;
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

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "7px";
    filterItem.appendChild(innerTable);

    var conditionItems = [];

    if (jsObject.ColumnIsNumericType(columnObject.dataType)) {
        conditionItems = jsObject.GetFilterConditionItems("Numeric");
    }
    else if (jsObject.ColumnIsDateType(columnObject.dataType)) {
        conditionItems = jsObject.GetFilterConditionItems("DateTime");
    }
    else if (columnObject.dataType == "bool") {
        conditionItems = jsObject.GetFilterConditionItems("Boolean");
    }
    else {
        conditionItems = jsObject.GetFilterConditionItems("String");
    }

    //Condition
    var conditionControl = this.DropDownList(null, 145, null, conditionItems, true, false);
    conditionControl.style.marginRight = "7px";
    innerTable.addCell(conditionControl);
    conditionControl.setKey(filter.condition);

    conditionControl.action = function () {
        filter.condition = this.key;
        filterItem.updateControls();
    };

    //Value
    var valueControl;

    /*if (jsObject.ColumnIsDateType(columnObject.type)) {
        valueControl = this.DateControl(null, 130, null, true);

        if (!filter.isExpression) {
            valueControl.setKey(filter.value ? new Date(filter.value) : new Date());
        }
        else {
            valueControl.textBox.value = filter.value;
        }

        valueControl.action = function () {
            filter.value = !filter.isExpression
                ? this.jsObject.formatDate(this.key, "MM/dd/yyyy")
                : this.textBox.value;
        }
    }
    else*/ if (columnObject.dataType == "bool") {
        valueControl = this.DropDownList(null, 160, null, this.GetBoolItems());
        valueControl.setKey(filter.value);

        valueControl.action = function () {
            filter.value = this.key;
        }
    }
    else {
        valueControl = this.DropDownList(null, 160, null, filterContainer.filterControl.filterItems);
        valueControl.textBox.value = filter.value || "";

        valueControl.action = function () {
            filter.value = this.textBox.value;
        }
    }

    valueControl.style.marginRight = "7px";
    innerTable.addCell(valueControl);
    filterItem.valueControl = valueControl;

    //And Caption
    var andCaption = innerTable.addTextCell(this.collections.loc["FilterModeAnd"]);

    //Value2
    var value2Control;

    /*if (jsObject.ColumnIsDateType(columnObject.dataType)) {
        value2Control = this.DateControl(null, 13, null, true);

        if (!filter.isExpression) {
            value2Control.setKey(filter.value2 ? new Date(filter.value2) : new Date());
        }
        else {
            value2Control.textBox.value = filter.value2;
        }

        value2Control.action = function () {
            filter.value2 = !filter.isExpression
                ? this.jsObject.formatDate(this.key, "MM/dd/yyyy")
                : this.textBox.value;
        }
    }
    else */{
        value2Control = this.DropDownList(null, 160, null, filterContainer.filterControl.filterItems);
        value2Control.textBox.value = filter.value2 || "";

        value2Control.action = function () {
            filter.value2 = this.textBox.value;
        }
    }

    value2Control.style.margin = "0 7px 0 7px";
    innerTable.addCell(value2Control);

    //Expression
    var isExpressionControl = this.CheckBox(null, this.collections.loc["Expression"]);
    isExpressionControl.style.marginRight = "7px";
    innerTable.addCell(isExpressionControl);
    isExpressionControl.setChecked(filter.isExpression);

    isExpressionControl.action = function () {
        filter.isExpression = this.isChecked;
        filterItem.updateControls();
        if (!this.isChecked) {
            if (columnObject.dataType == "bool") {
                valueControl.setKey("False");
            }
            //else if (jsObject.ColumnIsDateType(columnObject.dataType)) {
            //    valueControl.setKey(new Date());
            //}
            else {
                valueControl.textBox.value = "";
                valueControl.textBox.focus();
            }
        }
    }

    filterItem.updateControls = function () {
        var showValue2 = filter.condition == "Between" || filter.condition == "NotBetween";
        var showValues = filter.condition != "IsNull" && filter.condition != "IsNotNull" && filter.condition != "IsBlank" && filter.condition != "IsNotBlank";
        valueControl.style.display = showValues ? "" : "none";
        value2Control.style.display = showValues && showValue2 ? "" : "none";
        andCaption.style.display = showValue2 ? "" : "none";
        isExpressionControl.style.display = showValues ? "" : "none";

        valueControl.button.parentElement.style.visibility = !filter.isExpression ? "visible" : "hidden";
        value2Control.button.parentElement.style.visibility = !filter.isExpression ? "visible" : "hidden";

        if (columnObject.dataType == "bool") {
            valueControl.readOnly = valueControl.textBox.readOnly = !filter.isExpression;
        }
        //else if (jsObject.ColumnIsDateType(columnObject.dataType)) {
        //    valueControl.textBox.readOnly = !filter.isExpression;
        //    value2Control.textBox.readOnly = !filter.isExpression;
        //}
    }

    filterItem.updateControls();

    return filterItem;
}