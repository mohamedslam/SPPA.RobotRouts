
StiMobileDesigner.prototype.InitializeFilterElementFieldMenu = function (name, expressionControl, form, allowExpressions) {
    var menu = this.BaseContextMenu(name, "Down", null);
    menu.rightToLeft = true;
    var jsObject = this;

    menu.onshow = function () {
        var currentContainer = this.currentContainer || expressionControl.currentContainer;
        if (!currentContainer) return;
        var selectedItem = currentContainer.item || currentContainer.selectedItem;
        var variables = jsObject.options.report ? jsObject.GetVariablesFromDictionary(jsObject.options.report.dictionary) : [];
        var items = [];
        items.push(jsObject.Item("newItem", jsObject.loc.Dashboard.NewField, "Empty16.png", "newItem"));

        if (selectedItem && expressionControl.isEnabled) {
            items.push("separator");
            items.push(jsObject.Item("renameField", jsObject.loc.Buttons.Rename, "DataColumn.png", "renameField"));
            items.push(jsObject.Item("removeField", jsObject.loc.Dashboard.RemoveField, "Remove.png", "removeField"));
            items.push(jsObject.Item("editField", jsObject.loc.PropertyMain.Field, "EditButton.png", "editField"));

            if (variables.length > 0) {
                items.push("separator1_1");
                items.push(jsObject.Item("variable", jsObject.loc.PropertyMain.Variable, "Empty16.png", "variable", null, true));
            }
        }

        this.addItems(items);

        if (this.items.variable) {
            var varItems = [];
            for (var i = 0; i < variables.length; i++) {
                varItems.push(jsObject.Item(variables[i].name, variables[i].name, "Empty16.png", variables[i].name));
            }

            var varMenu = jsObject.InitializeSubMenu(name ? name + "_VariablesSubMenu" : null, varItems, this.items.variable, this);

            varMenu.action = function (item) {
                var params = {
                    command: "EditField",
                    containerName: jsObject.UpperFirstChar(currentContainer.name),
                    expression: jsObject.CheckExpressionBrackets(item.key),
                    itemIndex: currentContainer.getSelectedItemIndex ? currentContainer.getSelectedItemIndex() : 0
                }
                form.sendCommand(params, function (answer) {
                    form.updateControls(answer.elementProperties);
                    if (form.updateElementProperties) form.updateElementProperties(answer.elementProperties);
                    form.updateSvgContent(answer.elementProperties.svgContent);
                    form.checkStartMode();

                    if (currentContainer.selectedItem) {
                        currentContainer.selectedItem.action();
                    }
                    else if (currentContainer.getCountItems) {
                        var lastItem = currentContainer.getItemByIndex(currentContainer.getCountItems() - 1);
                        if (lastItem) lastItem.select();
                    }
                    else if (currentContainer.item) {
                        currentContainer.item.action();
                    }
                });
                menu.changeVisibleState(false);
            }
        }
    }

    menu.onhide = function () {
        this.currentContainer = null;
    }

    menu.action = function (menuItem) {
        var currentContainer = this.currentContainer || expressionControl.currentContainer;
        if (currentContainer) {
            var selectedItem = currentContainer.item || currentContainer.selectedItem;

            switch (menuItem.key) {
                case "newItem":
                case "editField": {
                    expressionControl.button.key = expressionControl.textBox.value;
                    jsObject.options.dataTree.showNoneItem = false;

                    jsObject.InitializeDataColumnForm(function (colForm) {
                        colForm.parentButton = expressionControl.button;
                        colForm.caption.innerHTML = jsObject.loc.PropertyMain.Field;
                        colForm.changeVisibleState(true);

                        colForm.onhide = function () {
                            jsObject.options.dataTree.showNoneItem = true;
                            colForm.caption.innerHTML = jsObject.loc.PropertyMain.DataColumn;
                        };

                        colForm.action = function () {
                            this.changeVisibleState(false);
                            var expression = "";
                            var dataTreeSelItem = colForm.dataTree.selectedItem;
                            if (dataTreeSelItem && dataTreeSelItem.itemObject && dataTreeSelItem.itemObject.typeItem == "Column") {
                                var currItem = dataTreeSelItem;
                                while (currItem.parent != null) {
                                    if (expression != "") expression = "." + expression;
                                    expression = currItem.itemObject.name + expression;
                                    if (currItem.itemObject.typeItem == "BusinessObject" && currItem.parent != null &&
                                        currItem.parent.itemObject.typeItem != "BusinessObject" || currItem.itemObject.typeItem == "DataSource") break;
                                    currItem = currItem.parent;
                                }
                                if (expression) expression = jsObject.CheckExpressionBrackets(expression);
                            }

                            var params = {
                                command: menuItem.key == "newItem" ? "NewItem" : "EditField",
                                containerName: jsObject.UpperFirstChar(currentContainer.name),
                                expression: expression,
                                itemIndex: currentContainer.getSelectedItemIndex ? currentContainer.getSelectedItemIndex() : 0
                            }

                            form.sendCommand(params, function (answer) {
                                form.updateControls(answer.elementProperties);
                                if (form.updateElementProperties) form.updateElementProperties(answer.elementProperties);
                                form.updateSvgContent(answer.elementProperties.svgContent);
                                form.checkStartMode();

                                if (currentContainer.selectedItem) {
                                    currentContainer.selectedItem.action();
                                }
                                else if (currentContainer.getCountItems) {
                                    var lastItem = currentContainer.getItemByIndex(currentContainer.getCountItems() - 1);
                                    if (lastItem) lastItem.select();
                                }
                                else if (currentContainer.item) {
                                    currentContainer.item.action();
                                }
                            });
                        }
                    });
                    break;
                }
                case "renameField": {
                    if (selectedItem) selectedItem.setEditable(true);
                    break;
                }
                case "removeField": {
                    if (currentContainer.item) {
                        currentContainer.clear();
                        currentContainer.action();
                    }
                    else if (currentContainer.selectedItem) {
                        currentContainer.selectedItem.remove();
                    }
                    break;
                }
            }
        }

        this.changeVisibleState(false);
    }

    if (expressionControl && expressionControl.editButton) {
        if (allowExpressions) {
            expressionControl.textBox.action = function () {
                var expression = this.value;
                var currentContainer = expressionControl.currentContainer;

                var params = {
                    command: "EditField",
                    containerName: jsObject.UpperFirstChar(currentContainer.name),
                    expression: expression,
                    itemIndex: currentContainer.getSelectedItemIndex ? currentContainer.getSelectedItemIndex() : 0
                }

                form.sendCommand(params, function (answer) {
                    form.updateControls(answer.elementProperties);
                    if (form.updateElementProperties) form.updateElementProperties(answer.elementProperties);
                    form.updateSvgContent(answer.elementProperties.svgContent);
                    form.checkStartMode();

                    if (currentContainer.selectedItem) {
                        currentContainer.selectedItem.action();
                    }
                    else if (currentContainer.getCountItems) {
                        var lastItem = currentContainer.getItemByIndex(currentContainer.getCountItems() - 1);
                        if (lastItem) lastItem.select();
                    }
                    else if (currentContainer.item) {
                        currentContainer.item.action();
                    }
                });
            }
        }
        else {
            expressionControl.editButton.action = function () {
                menu.action({ key: "editField" });
            }
        }
    }

    return menu;
}