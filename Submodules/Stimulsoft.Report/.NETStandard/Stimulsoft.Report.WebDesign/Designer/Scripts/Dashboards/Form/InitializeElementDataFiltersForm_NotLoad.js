
StiMobileDesigner.prototype.InitializeElementDataFiltersForm_ = function () {
    var form = this.DashboardBaseForm("elementDataFiltersForm", this.loc.PropertyMain.Filters, 1, this.HelpLinks["elementDataFilters"]);
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";
    form.hideButtonsPanel();
    var jsObject = this;

    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.width = "350px";

    form.container.appendChild(controlsTable);
    form.container.style.padding = "0 0 6px 0";

    //Field
    var field = this.ExpressionControlWithMenu(null, 210, null, null, true);
    var fieldMenu = this.options.menus.dataFiltersMenu || this.InitializeFilterElementFieldMenu("dataFiltersMenu", field, form);
    field.menu = fieldMenu;
    fieldMenu.parentButton = field.button;
    field.textBox.setAttribute("placeholder", jsObject.loc.PropertyMain.Field);

    //Filters Container
    var filtersBlock = this.FiltersDataColumnsBlock(form, fieldMenu, "filters", null, true);
    filtersBlock.style.width = "350px";
    form.addControlRow(controlsTable, null, "filtersBlock", filtersBlock, "12px");
    form.addControlRow(controlsTable, this.loc.PropertyMain.Field, "field", field, "6px 12px 6px 12px");

    var parentFiltersContainer = filtersBlock.container.parentElement;
    field.currentContainer = filtersBlock.container;

    fieldMenu.onshow = function () {
        this.currentContainer = filtersBlock.container;

        var items = [];
        items.push(jsObject.Item("newItem", jsObject.loc.FormBand.AddFilter.replace("&", ""), "Empty16.png", "newItem"));

        if (filtersBlock.container.selectedItem) {
            items.push(jsObject.Item("removeField", jsObject.loc.FormBand.RemoveFilter.replace("&", ""), "Remove.png", "removeField"));
            items.push("separator");
            items.push(jsObject.Item("editField", jsObject.loc.Dashboard.EditField, "EditButton.png", "editField"));
        }

        this.addItems(items);
    }

    fieldMenu.oldAction = fieldMenu.action;

    fieldMenu.action = function (menuItem) {
        if (menuItem.key == "newItem") {
            var currentContainer = filtersBlock.container;

            form.sendCommand({ command: "NewItem" }, function (answer) {
                form.updateControls(answer.elementProperties);
                if (form.updateElementProperties) form.updateElementProperties(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
                form.checkStartMode();
                var lastItem = currentContainer.getItemByIndex(currentContainer.getCountItems() - 1);
                if (lastItem) lastItem.select();
            });

            fieldMenu.changeVisibleState(false);
        }
        else {
            fieldMenu.oldAction(menuItem);
        }
    }

    //Operation
    var operation = this.DropDownList("elementDataFiltersOperation", 210, null, this.GetFilterOperationItems(), true, null, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Operation, "operation", operation, "6px 12px 6px 12px", null, true, null, true);

    var operationMessage = document.createElement("div");
    operationMessage.className = "stiDesignerTextContainer";
    operationMessage.style.color = "#aaaaaa";
    operationMessage.style.width = "350px";
    operationMessage.innerHTML = this.loc.Dashboard.DataFilterGrouping;
    form.addControlRow(controlsTable, null, "operationMessage", operationMessage, "12px 12px 6px 12px");
    form.addControlRow(controlsTable, null, "operationSep", this.FormSeparator(), "12px 12px 6px 12px");

    operation.action = function () {
        form.applyPropertiesToDataFilter("Operation", this.key);
    }

    //Condition
    var condition = this.DropDownList("elementDataFiltersCondition", 210, null, [], true, null, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Condition, "condition", condition, "12px 12px 6px 12px", null, true, null, true);

    condition.action = function () {
        form.updateControlsVisibleStates();
        form.applyPropertiesToDataFilter("Condition", this.key);
    }

    //Value
    var value = this.DropDownList("elementDataFiltersValue", 210, null, [], false, null, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Value, "value", value, "12px 12px 6px 12px", null, true, null, true);

    value.action = function () {
        form.applyPropertiesToDataFilter("Value", this.key);
    }

    value.getItems = function (itemObject) {
        var items = [];
        if (itemObject) {
            if (itemObject.type == "bool") {
                items = jsObject.GetBoolItems();
            }
            else if (itemObject.values && itemObject.values.length > 0) {
                for (var i = 0; i < itemObject.values.length; i++) {
                    items.push(jsObject.Item("item" + i, itemObject.values[i], null, itemObject.values[i]));
                }
            }
        }
        return items;
    }

    value.updateItems = function (itemObject) {
        this.addItems(this.getItems(itemObject));
    }

    //Value Expression
    var valueExp = this.ExpressionControl(null, 210);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Value, "valueExp", valueExp, "12px 12px 6px 12px", null, true, null, true);

    valueExp.action = function () {
        form.applyPropertiesToDataFilter("ValueExp", this.textBox.value);
    }

    //ValueDate
    var valueDate = this.DateControl("elementDataFiltersValueDate", 210, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Value, "valueDate", valueDate, "12px 12px 6px 12px", null, true, null, true);

    valueDate.action = function () {
        form.applyPropertiesToDataFilter("ValueDate", jsObject.formatDate(this.key, "MM/dd/yyyy"));
    }

    //Value2
    var value2 = this.DropDownList("elementDataFiltersValue2", 210, null, [], false, null, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.To, "value2", value2, "12px 12px 6px 12px", null, true, null, true);

    value2.action = function () {
        form.applyPropertiesToDataFilter("Value2", this.key);
    }

    value2.updateItems = function (itemObject) {
        this.addItems(value.getItems(itemObject));
    }

    //Value2 Expression
    var value2Exp = this.ExpressionControl(null, 210);
    form.addControlRow(controlsTable, this.loc.PropertyMain.To, "value2Exp", value2Exp, "12px 12px 6px 12px", null, true, null, true);

    value2Exp.action = function () {
        form.applyPropertiesToDataFilter("Value2Exp", this.textBox.value);
    }

    //Value2Date
    var value2Date = this.DateControl("elementDataFiltersValue2Date", 210, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.To, "value2Date", value2Date, "12px 12px 6px 12px", null, true, null, true);

    value2Date.action = function () {
        form.applyPropertiesToDataFilter("Value2Date", jsObject.formatDate(this.key, "MM/dd/yyyy"));
    }

    //Expression
    var expression = this.CheckBox(null, this.loc.PropertyMain.Expression);
    form.addControlRow(controlsTable, " ", "expression", expression, "12px 12px 6px 12px", null, true, null, true);

    expression.action = function () {
        var itemObject = filtersBlock.container.selectedItem ? filtersBlock.container.selectedItem.itemObject : null;
        if (itemObject) {
            if (this.isChecked) {
                valueExp.textBox.value = jsObject.ColumnIsDateType(itemObject.type) ? valueDate.textBox.value : value.textBox.value;
                value2Exp.textBox.value = jsObject.ColumnIsDateType(itemObject.type) ? value2Date.textBox.value : value2.textBox.value;
            }
            else if (!jsObject.ColumnIsDateType(itemObject.type)) {
                value.textBox.value = valueExp.textBox.value;
                value2.textBox.value = value2Exp.textBox.value;
            }
        }
        form.updateControlsVisibleStates();
        form.applyPropertiesToDataFilter("Expression", this.isChecked);
    }

    //FilterOn
    var filterOn = this.CheckBox(null, this.loc.PropertyMain.FilterOn);
    form.addControlRow(controlsTable, " ", "filterOn", filterOn, "12px", null, true, null, true);

    filterOn.action = function () {
        form.applyPropertiesToDataFilter("IsEnabled", this.isChecked);
    }

    condition.updateItems = function (filterObject) {
        this.setEnabled(filterObject != null);

        if (filterObject) {
            var conditionItems = [];

            if (jsObject.ColumnIsNumericType(filterObject.type)) {
                conditionItems = jsObject.GetFilterConditionItems("Numeric", false);
            }
            else if (jsObject.ColumnIsDateType(filterObject.type)) {
                conditionItems = jsObject.GetFilterConditionItems("DateTime", false);
            }
            else if (filterObject.type == "bool") {
                conditionItems = jsObject.GetFilterConditionItems("Boolean", false);
            }
            else {
                conditionItems = jsObject.GetFilterConditionItems("String", false, true);
            }
            this.addItems(conditionItems);
        }
    }

    form.updateControlsVisibleStates = function () {
        var filterObject = filtersBlock.container.selectedItem ? filtersBlock.container.selectedItem.itemObject : null;
        var showValue2 = condition.key == "Between" || condition.key == "NotBetween";
        var isExpression = form.controls.expression.isChecked;
        var showDateControls = filterObject && jsObject.ColumnIsDateType(filterObject.type);

        form.controls.operationRow.style.display = form.controls.operationSepRow.style.display = form.elementProperties && form.elementProperties.tableFiltersGroupsType != "None" ? "" : "none";
        form.controls.operationMessageRow.style.display = form.elementProperties && form.elementProperties.tableFiltersGroupsType == "Complex" ? "" : "none";
        form.controls.valueRow.style.display = !isExpression && !showDateControls ? "" : "none";
        form.controls.valueDateRow.style.display = !isExpression && showDateControls ? "" : "none";
        form.controls.valueExpRow.style.display = isExpression ? "" : "none";
        form.controls.value2Row.style.display = showValue2 && !isExpression && !showDateControls ? "" : "none";
        form.controls.value2DateRow.style.display = showValue2 && !isExpression && showDateControls ? "" : "none";
        form.controls.value2ExpRow.style.display = showValue2 && isExpression ? "" : "none";
        form.controls.valueText.innerHTML = form.controls.valueExpText.innerHTML = form.controls.valueDateText.innerHTML =
            showValue2 ? jsObject.loc.PropertyMain.From : jsObject.loc.PropertyMain.Value;
    }

    form.updateControls = function (elementProperties) {
        if (!elementProperties) return;
        form.elementProperties = elementProperties;
        filtersBlock.container.updateFilters(elementProperties.filters, filtersBlock.container.getSelectedItemIndex());
        operation.setKey(elementProperties.operation);
        form.updateControlsVisibleStates();
    }

    form.checkStartMode = function () {
        var itemsCount = filtersBlock.container.getCountItems();

        if (itemsCount == 0) {
            form.container.appendChild(filtersBlock.container);
            controlsTable.style.display = "none";
            filtersBlock.container.style.height = filtersBlock.container.style.maxHeight = "260px";
            filtersBlock.container.style.width = "267px";
            filtersBlock.container.style.margin = "6px 12px 6px 12px";
        }
        else {
            parentFiltersContainer.appendChild(filtersBlock.container);
            controlsTable.style.display = "";
            filtersBlock.container.style.height = "auto";
            filtersBlock.container.style.width = "auto";
            filtersBlock.container.style.margin = "0";
            filtersBlock.container.style.maxHeight = "100px";
        }
    }

    form.onshow = function () {
        form.currentPanelName = jsObject.options.propertiesPanel.getCurrentPanelName();
        if (jsObject.options.showDictionary) jsObject.options.propertiesPanel.showContainer("Dictionary");

        filtersBlock.container.clear();
        filterOn.setChecked(true);
        condition.setKey("EqualTo");
        operation.setKey("AND");
        value.setKey("");
        value2.setKey("");
        expression.setChecked(false);
        form.updateControlsVisibleStates();
        form.checkStartMode();

        form.sendCommand({ command: "GetElementDataFiltersProperties" },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.checkStartMode();
                if (filtersBlock.container.getCountItems() > 0) {
                    filtersBlock.container.getItemByIndex(0).select();
                }
            }
        );
    }

    form.onhide = function () {
        jsObject.options.propertiesPanel.showContainer(form.currentPanelName);
    }

    form.getSelectedItem = function () {
        return filtersBlock.container.selectedItem;
    }

    form.applyPropertiesToDataFilter = function (propertyName, propertyValue) {
        var itemIndex = filtersBlock.container.getSelectedItemIndex();
        if (itemIndex != null) {
            form.sendCommand({ command: "SetPropertyValue", propertyName: propertyName, propertyValue: propertyValue, itemIndex: itemIndex },
                function (answer) {
                    form.updateControls(answer.elementProperties);
                    form.updateSvgContent(answer.elementProperties.svgContent);
                }
            );
        }
    }

    form.sendCommand = function (updateParameters, callbackFunction) {
        jsObject.SendCommandToDesignerServer("UpdateElementDataFilters",
            {
                componentName: form.currentElement.properties.name,
                updateParameters: updateParameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    form.updateSvgContent = function (svgContent) {
        this.currentElement.properties.svgContent = svgContent;
        this.currentElement.repaint();
    }

    return form;
}

StiMobileDesigner.prototype.FiltersDataColumnsBlock = function (form, contextMenu, containerName, headerText, multiItems) {
    var block = this.DashboardDataColumnsBlock(form, contextMenu, containerName, headerText, multiItems);
    var container = block.container;
    var jsObject = this;

    container.updateFilters = function (filters, selectedIndex) {
        var oldScrollTop = this.scrollTop;
        this.style.height = this.offsetHeight + "px";

        this.clear();
        for (var i = 0; i < filters.length; i++) {
            var item = this.addItem(filters[i].label, null, filters[i]);
            if (item.captionContainer) item.captionContainer.style.maxWidth = "240px";
        }
        if (selectedIndex != null && selectedIndex < filters.length && selectedIndex >= 0) {
            this.childNodes[selectedIndex].select();
        }
        this.scrollTop = oldScrollTop;
        this.style.height = "auto";
        this.style.paddingBottom = (multiItems && this.getCountItems() > 0) ? "30px" : "0px";
    }

    container.onAction = function (a) {
        if (this.selectedItem) {
            var itemObject = this.selectedItem.itemObject;
            form.controls.condition.updateItems(itemObject);
            form.controls.condition.setKey(itemObject.condition);
            form.controls.expression.setChecked(itemObject.isExpression);
            form.controls.filterOn.setChecked(itemObject.isEnabled);
            form.controls.field.textBox.value = itemObject.path;

            if (itemObject.isExpression) {
                form.controls.valueExp.textBox.value = itemObject.value;
                form.controls.value2Exp.textBox.value = itemObject.value2;
            }
            else if (jsObject.ColumnIsDateType(itemObject.type)) {
                form.controls.valueDate.setKey(itemObject.value ? new Date(itemObject.value) : new Date());
                form.controls.value2Date.setKey(itemObject.value2 ? new Date(itemObject.value2) : new Date());
            }
            else {
                form.controls.value.setKey(itemObject.value != null ? itemObject.value : "");
                form.controls.value2.setKey(itemObject.value2 != null ? itemObject.value2 : "");
                form.controls.value.updateItems(itemObject);
                form.controls.value2.updateItems(itemObject);
            }
            form.updateControlsVisibleStates();
        }
    }

    container.onmouseup = function (event) {
        if (event.button == 2) {
            //context menu
            event.stopPropagation();
            if (contextMenu) {
                var point = jsObject.FindMousePosOnMainPanel(event);
                contextMenu.currentContainer = container;
                contextMenu.show(point.xPixels + 3, point.yPixels + 3, "Down", "Right");
            }
            return false;
        }
        else if (jsObject.options.itemInDrag) {
            var itemObject = jsObject.CopyObject(jsObject.options.itemInDrag.originalItem.itemObject);
            if (!itemObject) return;
            var typeItem = itemObject.typeItem;

            if (typeItem == "FilterRule") {
                var toIndex = this.getOverItemIndex();
                var fromIndex = container.getItemIndex(jsObject.options.itemInDrag.originalItem);
                var commandName = jsObject.options.CTRL_pressed ? "MoveAndDuplicateFilter" : "MoveFilter";

                if (toIndex != null && fromIndex != null && fromIndex != toIndex) {
                    var params = {
                        command: commandName,
                        toIndex: toIndex,
                        fromIndex: fromIndex
                    };

                    form.sendCommand(params,
                        function (answer) {
                            container.updateFilters(answer.elementProperties.filters);
                            var selectedIndex = toIndex != null ? toIndex : container.getCountItems();
                            container.updateFilters(answer.elementProperties.filters, selectedIndex);
                            form.updateSvgContent(answer.elementProperties.svgContent);
                            form.checkStartMode();
                        }
                    );
                }
            }
            else if (typeItem == "Column" || typeItem == "DataSource" || typeItem == "BusinessObject") {
                var draggedItem = {
                    itemObject: itemObject
                };

                if (typeItem == "Column") {
                    var columnParent = jsObject.options.dictionaryTree.getCurrentColumnParent();
                    if (columnParent) {
                        draggedItem.currentParentType = columnParent.type;
                        draggedItem.currentParentName = (columnParent.type == "BusinessObject") ? jsObject.options.itemInDrag.originalItem.getBusinessObjectFullName() : columnParent.name;
                    }
                }
                else {
                    draggedItem.currentParentType = typeItem;
                    draggedItem.currentParentName = itemObject.name;
                }

                var params = {
                    command: "InsertFilters",
                    draggedItem: draggedItem
                }

                if (typeItem == "Column") {
                    params.insertIndex = this.getOverItemIndex();
                }

                form.sendCommand(params,
                    function (answer) {
                        var insertIndex = params.insertIndex != null ? params.insertIndex : answer.elementProperties.filters.length - 1;
                        container.updateFilters(answer.elementProperties.filters, insertIndex);
                        form.updateControls(answer.elementProperties);
                        form.updateSvgContent(answer.elementProperties.svgContent);
                        form.checkStartMode();
                        form.correctTopPosition();
                    }
                );
            }
        }

        return false;
    }

    container.onRemove = function (itemIndex) {
        form.sendCommand({ command: "RemoveFilter", itemIndex: itemIndex },
            function (answer) {
                container.updateMeters(answer.elementProperties.filters, container.getSelectedItemIndex());
                form.updateControls(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
                form.checkStartMode();
            }
        );
    }

    container.oncontextmenu = function (event) {
        return false;
    }

    return block;
}