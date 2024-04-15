
StiMobileDesigner.prototype.InitializeEditDataTransformationForm_ = function () {

    var form = this.BaseFormPanel("editDataTransformationForm", this.loc.PropertyMain.DataTransformation, 1, this.HelpLinks["dataTransformation"]);
    form.dataTransformation = form.datasource = null;
    form.mode = "Edit";
    var jsObject = this;

    var controlsTable = this.CreateHTMLTable();
    form.container.appendChild(controlsTable);
    controlsTable.style.margin = "6px 0 6px 0";

    //Names Controls
    var textBoxes = [
        ["nameInSource", this.loc.PropertyMain.NameInSource],
        ["name", this.loc.PropertyMain.Name],
        ["alias", this.loc.PropertyMain.Alias]
    ]

    for (var i = 0; i < textBoxes.length; i++) {
        var textBox = this.TextBox(null, 450);
        form.addControlRow(controlsTable, textBoxes[i][1], textBoxes[i][0], textBox, "6px 12px 6px 12px");
    }

    //Data Header
    var dataHeader = document.createElement("div");
    dataHeader.className = "stiDesignerFormBlockHeader";
    dataHeader.style.padding = "6px 6px 6px 12px";
    dataHeader.innerHTML = this.loc.FormBand.title;
    form.addControlRow(controlsTable, null, "dataHeader", dataHeader, "6px 12px 6px 12px");

    //Data Containers
    var dataGrid = this.DataGrid(550, 290);
    form.addControlRow(controlsTable, " ", "dataGrid", dataGrid, "6px 12px 6px 12px");
    dataGrid.parentNode.style.position = "relative";
    this.AddProgressToControl(dataGrid.parentNode);

    var columnsContainer = this.DataTransformationContainer(190, 290, form);
    columnsContainer.style.margin = "6px 0 6px 12px";
    form.controls.dataGridText.style.padding = "0px";
    form.controls.dataGridText.appendChild(columnsContainer);
    form.controls.columnsContainer = columnsContainer;

    dataGrid.update = function (completeFunc) {
        dataGrid.parentNode.progress.show();

        form.sendCommand(
            {
                command: "GetDataGridContent",
                columns: columnsContainer.getColumns(),
                sortRules: form.dataTransformation.sortRules,
                filterRules: form.dataTransformation.filterRules,
                actionRules: form.dataTransformation.actionRules
            },
            function (answer) {
                dataGrid.parentNode.progress.hide();
                if (completeFunc) completeFunc();
                var content = answer.dataGridContent;

                if (content.errorMessage) {
                    form.controls.errorBlock.style.display = "";
                    form.controls.errorBlock.innerHTML = content.errorMessage;
                }
                else {
                    form.controls.errorBlock.style.display = "none";
                    var oldScrollLeft = dataGrid.scrollLeft;
                    dataGrid.clear();
                    dataGrid.showData(content.data, content.sortLabels, content.filterLabels);
                    dataGrid.scrollLeft = oldScrollLeft;
                }
            });
    }

    dataGrid.action = function (headerButton) {
        var sortFilterMenu = jsObject.SortFilterMenu(headerButton, form);
        sortFilterMenu.changeVisibleState(true);
        this.selectedHeaderButton = headerButton;
    }

    //Column Types
    var modes = ["Dimension", "Measure"];
    var modesTable = this.CreateHTMLTable();
    modesTable.buttons = {};
    for (var i = 0; i < modes.length; i++) {
        var button = this.FormButtonWithThemeBorder(null, null, null, "Meters." + modes[i] + ".png", this.loc.Dashboard[modes[i]]);
        button.mode = modes[i];
        button.style.marginRight = "5px";
        modesTable.addCell(button);
        modesTable.buttons[modes[i]] = button;

        button.action = function () {
            this.select();
            var newMode = this.mode;
            if (columnsContainer.selectedItem) {
                form.sendCommand(
                    {
                        command: "ChangeDataColumnMode",
                        columnObject: columnsContainer.selectedItem.itemObject,
                        newMode: newMode
                    },
                    function (answer) {
                        if (answer.newColumn) {
                            columnsContainer.selectedItem.repaint(null, "Meters." + newMode + ".png", answer.newColumn);
                            columnsContainer.onAction();
                        }
                    });
            }
        }

        button.select = function () {
            for (var name in modesTable.buttons) {
                modesTable.buttons[name].setSelected(false);
            }
            this.setSelected(true);
        }
    }
    form.addControlRow(controlsTable, this.loc.PropertyMain.Type, "modesTable", modesTable, "6px 12px 6px 12px");

    //Expression
    var expressionControl = this.ExpressionControlWithMenu(null, 450, null, null);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Expression, "expressionControl", expressionControl, "6px 12px 6px 12px");
    form.expressionMenu = this.options.menus.dataTransfExpressionMenu || this.InitializeDataTransformationExpressionMenu("dataTransfExpressionMenu", expressionControl, columnsContainer, form);
    expressionControl.menu = form.expressionMenu;
    expressionControl.cutBrackets = true;
    expressionControl.textBox.setAttribute("placeholder", jsObject.loc.PropertyMain.Field);
    form.expressionMenu.parentButton = expressionControl.button;

    expressionControl.action = function () {
        if (columnsContainer.selectedItem) {
            columnsContainer.selectedItem.itemObject.expression = StiBase64.encode(this.textBox.value);
            dataGrid.update();
        }
    }

    expressionControl.refreshHintText = function () {
        if (modesTable.buttons.Dimension.isSelected)
            expressionControl.textBox.setAttribute("placeholder", jsObject.loc.PropertyMain.Field);
        else if (modesTable.buttons.Measure.isSelected)
            expressionControl.textBox.setAttribute("placeholder", "Sum(" + jsObject.loc.PropertyMain.Field + ")");
        else
            expressionControl.textBox.setAttribute("placeholder", "");
    }

    //Type
    var typeControl = this.DropDownList("dataTransformTypeControl", 170, null, this.GetVariableTypesItems(), true, true, null, null, { width: 16, height: 16 });
    typeControl.image.style.width = "16px";
    typeControl.image.style.margin = "0 8px 0 8px";
    form.addControlRow(controlsTable, this.loc.PropertyMain.Type, "typeControl", typeControl, "6px 12px 6px 12px");

    typeControl.action = function () {
        if (columnsContainer.selectedItem) {
            columnsContainer.selectedItem.itemObject.originalType = columnsContainer.selectedItem.itemObject.type = this.key;
            dataGrid.update();
        }
    }

    //Error block
    var errorBlock = document.createElement("div");
    errorBlock.className = "stiDataTransformErrorBlock";
    errorBlock.style.display = "none";
    form.controls.errorBlock = errorBlock;
    form.container.appendChild(errorBlock);

    form.resetColumnsControls = function () {
        modesTable.buttons.Dimension.setEnabled(false);
        modesTable.buttons.Measure.setEnabled(false);
        expressionControl.textBox.value = "";
        expressionControl.setEnabled(false);
        typeControl.setKey("object");
        typeControl.setEnabled(false);
    }

    form.onshow = function () {
        this.dataTransformation = this.datasource;
        this.mode = "Edit";
        if (jsObject.options.propertiesPanel) {
            this.oldDictionaryMode = jsObject.options.propertiesPanel.dictionaryMode;
            jsObject.options.propertiesPanel.setDictionaryMode(true);
            jsObject.options.propertiesPanel.setEnabled(true);
        }
        if (this.dataTransformation == null) {
            this.dataTransformation = jsObject.DataTransformationObject();
            this.mode = "New";
        }
        var caption = jsObject.loc.FormDictionaryDesigner["DataTransformation" + this.mode];
        if (caption) this.caption.innerHTML = caption;
        var controls = form.controls;

        this.editableDictionaryItem = this.mode == "Edit" && jsObject.options.dictionaryTree ? jsObject.options.dictionaryTree.selectedItem : null;
        controls.name.hideError();
        controls.name.focus();
        this.nameInSource = null;

        columnsContainer.clear();
        dataGrid.clear();
        controls.name.value = this.dataTransformation.name;
        controls.nameInSource.value = this.dataTransformation.nameInSource;
        controls.alias.value = this.dataTransformation.alias;

        if (this.mode == "Edit") {
            var currDataTransform = jsObject.GetDataSourceByNameFromDictionary(this.dataTransformation.name);
            if (currDataTransform) this.dataTransformation.columns = currDataTransform.columns;
        }

        columnsContainer.addColumns(this.dataTransformation.columns);
    }

    form.onhide = function () {
        if (jsObject.options.propertiesPanel) {
            jsObject.options.propertiesPanel.setDictionaryMode(this.oldDictionaryMode);
        }
        jsObject.DeleteTemporaryMenus();
    }

    form.action = function () {
        this.dataTransformation.mode = this.mode;

        if (!this.controls.name.checkNotEmpty(this.jsObject.loc.PropertyMain.Name)) return;
        if ((this.mode == "New" || this.controls.name.value != this.dataTransformation.name) &&
            !(this.controls.name.checkExists(this.jsObject.GetDataSourcesFromDictionary(this.jsObject.options.report.dictionary), "name") &&
                this.controls.name.checkExists(this.jsObject.GetVariablesFromDictionary(this.jsObject.options.report.dictionary), "name")))
            return;

        if (this.mode == "Edit") this.dataTransformation.oldName = this.dataTransformation.name;

        this.dataTransformation.name = this.controls.name.value;
        this.dataTransformation.nameInSource = this.controls.nameInSource.value;
        this.dataTransformation.alias = this.controls.alias.value;
        this.dataTransformation.columns = columnsContainer.getColumns();

        this.changeVisibleState(false);
        this.jsObject.SendCommandCreateOrEditDataSource(this.dataTransformation);
    }

    form.sendCommand = function (parameters, callbackFunction) {
        form.jsObject.SendCommandToDesignerServer("ExecuteCommandForDataTransformation",
            {
                parameters: parameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    return form;
}

StiMobileDesigner.prototype.DataTransformationContainer = function (width, height, form) {
    var dataContainer = this.DataContainer(width, height, true);
    var jsObject = this;

    dataContainer.addColumns = function (columnsObjects, insertIndex) {
        var firstItem = null;
        for (var i = 0; i < columnsObjects.length; i++) {
            var columnObject = dataContainer.checkColumnName(columnsObjects[i]);
            var item = this.addItem(jsObject.GetItemCaption(columnObject), "Meters." + columnObject.mode + ".png", columnObject, insertIndex);
            if (!firstItem) firstItem = item;
        }
        if (firstItem) firstItem.select();
        form.controls.dataGrid.update();
    }

    dataContainer.getColumns = function () {
        var columns = [];
        if (this.getCountItems() > 0) {
            for (var i = 0; i < this.childNodes.length; i++) {
                columns.push(this.childNodes[i].itemObject);
            }
        }
        return columns;
    }

    dataContainer.checkColumnName = function (columnObject) {
        if (this.getCountItems() > 0) {
            for (var i = 0; i < this.childNodes.length; i++) {
                var itemObject = this.childNodes[i].itemObject;
                if (itemObject.name.toLowerCase() == columnObject.name.toLowerCase()) {
                    var counter = 1;
                    var flag = false;
                    while (!flag) {
                        counter++;
                        flag = true;
                        for (var k = 0; k < this.childNodes.length; k++) {
                            if (this.childNodes[k].itemObject.name.toLowerCase() == columnObject.name.toLowerCase() + counter) {
                                flag = false;
                                break;
                            }
                        }
                    }
                    if (columnObject.alias != null && columnObject.name == columnObject.alias) {
                        columnObject.alias = columnObject.alias + counter;
                    }
                    columnObject.name = columnObject.name + counter;

                    return columnObject;
                }
            }
        }
        return columnObject;
    }

    dataContainer.onmouseup = function (event) {
        if (event.button == 2) {
            event.stopPropagation();
            var point = jsObject.FindMousePosOnMainPanel(event);
            form.expressionMenu.show(point.xPixels + 3, point.yPixels + 3, "Down", "Right");
        }
        else if (jsObject.options.itemInDrag) {
            var itemObject = jsObject.CopyObject(jsObject.options.itemInDrag.originalItem.itemObject);
            if (!itemObject) return;

            if (jsObject.options.itemInDrag.originalItem.container == this) {
                var toIndex = this.getOverItemIndex();
                var fromIndex = this.getItemIndex(jsObject.options.itemInDrag.originalItem);
                if (toIndex != null && fromIndex != null && fromIndex != toIndex) {
                    var movingItem = this.moveItem(fromIndex, toIndex);
                    if (movingItem) movingItem.select();
                    form.controls.dataGrid.update();
                }
            }
            else if (dataContainer.canInsert(itemObject)) {
                var draggedItem = {
                    itemObject: itemObject
                };

                if (itemObject.typeItem == "Column") {
                    var columnParent = jsObject.options.dictionaryTree.getCurrentColumnParent();
                    if (columnParent) {
                        draggedItem.currentParentType = columnParent.type;
                        draggedItem.currentParentName = columnParent.name;
                    }
                }
                else {
                    draggedItem.currentParentType = itemObject.typeItem;
                    draggedItem.currentParentName = itemObject.name;
                }

                form.sendCommand(
                    {
                        command: "GetDataTransformationColumnsFromColumns",
                        draggedItem: draggedItem
                    },
                    function (answer) {
                        if (answer.newColumns) {
                            var insertIndex = answer.newColumns.length == 1 ? dataContainer.getOverItemIndex() : null;
                            dataContainer.addColumns(answer.newColumns, insertIndex);
                        }
                    });
            }
        }

        return false;
    }

    dataContainer.oncontextmenu = function (event) {
        return false;
    }

    dataContainer.canInsert = function (itemObject) {
        return itemObject &&
            ((itemObject.typeItem == "Column" && !itemObject.isDataTransformationColumn) ||
                (itemObject.typeItem == "DataSource" && itemObject.typeDataSource != "StiDataTransformation"))
    }

    dataContainer.onmouseover = function () {
        if (jsObject.options.itemInDrag && dataContainer.canInsert(jsObject.options.itemInDrag.originalItem.itemObject)) {
            this.style.borderStyle = "dashed";
            this.style.borderColor = jsObject.options.themeColors[jsObject.GetThemeColor()];
        }
    }

    dataContainer.checkExistsRules = function () {
        var columnKeys = [];
        if (this.getCountItems() > 0) {
            for (var i = 0; i < this.childNodes.length; i++) {
                columnKeys.push(this.childNodes[i].itemObject.key);
            }
        }

        var ruleTypes = ["sortRules", "filterRules", "actionRules"];
        for (var i = 0; i < ruleTypes.length; i++) {
            var rules = form.dataTransformation[ruleTypes[i]];
            for (var k = 0; k < rules.length; k++) {
                if (columnKeys.indexOf(rules[k].key) < 0) {
                    jsObject.RemoveElementFromArray(rules, rules[k]);
                    k--;
                }
            }
        }
    }

    dataContainer.onAction = function (actionName) {
        var controls = form.controls;
        form.resetColumnsControls();

        if (this.selectedItem) {
            var itemObject = this.selectedItem.itemObject;
            var buttons = controls.modesTable.buttons;
            controls.modesTable.buttons.Dimension.setEnabled(true);
            controls.modesTable.buttons.Measure.setEnabled(true);
            controls.modesTable.buttons.Dimension.setSelected(itemObject.mode == "Dimension");
            controls.modesTable.buttons.Measure.setSelected(itemObject.mode == "Measure");
            controls.expressionControl.setEnabled(true);
            controls.expressionControl.textBox.value = StiBase64.decode(itemObject.expression);
            controls.typeControl.setEnabled(true);
            controls.typeControl.setKey(itemObject.type);
        }

        if (actionName == "remove") {
            this.checkExistsRules();
        }

        if (actionName != "clear" && actionName != "select") {
            form.controls.dataGrid.update();
        }

        controls.expressionControl.refreshHintText();
    }

    return dataContainer;
}


StiMobileDesigner.prototype.InitializeDataTransformationExpressionMenu = function (name, expressionControl, dataContainer, form) {
    var menu = this.InitializeDataContainerExpressionMenu(name, expressionControl, dataContainer, form);
    var jsObject = this;

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
        var selectedIndex = dataContainer.getSelectedItemIndex();

        if (menuItem.key.indexOf("Function_") == 0) {
            if (dataContainer.selectedItem) {
                form.sendCommand(
                    {
                        command: "SetFunctionToColumn",
                        function: menuItem.key.replace("Function_", ""),
                        columnObject: dataContainer.selectedItem.itemObject
                    },
                    function (answer) {
                        if (answer.column) {
                            dataContainer.selectedItem.itemObject = answer.column;
                            dataContainer.onAction();
                        }
                    });
            }
            return;
        }

        switch (menuItem.key) {
            case "renameField": {
                if (dataContainer.selectedItem) dataContainer.selectedItem.setEditable(true);
                break;
            }
            case "removeField": {
                if (dataContainer.selectedItem) dataContainer.selectedItem.remove();
                break;
            }
            case "removeAllFields": {
                dataContainer.clear();
                form.controls.dataGrid.update();
                break;
            }
            case "editExpression": {
                jsObject.InitializeExpressionEditorForm(function (expressionEditorForm) {
                    var propertiesPanel = jsObject.options.propertiesPanel;
                    expressionEditorForm.propertiesPanelZIndex = propertiesPanel.style.zIndex;
                    expressionEditorForm.propertiesPanelIsEnabled = propertiesPanel.isEnabled;
                    expressionEditorForm.resultControl = expressionControl;
                    expressionEditorForm.changeVisibleState(true);
                });
                break;
            }
            case "newDimension":
            case "newMeasure": {
                var columnObject = dataContainer.checkColumnName({ name: jsObject.loc.PropertyMain.Column });
                form.sendCommand(
                    {
                        command: "CreateNewDataTransformationColumn",
                        columnMode: menuItem.key == "newDimension" ? "Dimension" : "Measure",
                        columnName: columnObject.name
                    },
                    function (answer) {
                        if (answer.newColumn) {
                            dataContainer.addColumns([answer.newColumn], selectedIndex);
                            form.controls.expressionControl.textBox.focus();
                        }
                    });
                break;
            }
            case "duplicateField": {
                if (dataContainer.selectedItem) {
                    var newColumnObject = dataContainer.checkColumnName(jsObject.CopyObject(dataContainer.selectedItem.itemObject));
                    dataContainer.addColumns([newColumnObject], selectedIndex);
                }
                break;
            }
        }
    }

    return menu;
}