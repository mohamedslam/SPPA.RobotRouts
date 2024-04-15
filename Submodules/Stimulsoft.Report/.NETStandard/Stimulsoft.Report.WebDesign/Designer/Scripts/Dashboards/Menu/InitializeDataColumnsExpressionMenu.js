
StiMobileDesigner.prototype.InitializeDataColumnsExpressionMenu = function (name, expressionControl, form) {
    var menu = this.BaseContextMenu(name, "Down", null);
    menu.rightToLeft = true;
    var jsObject = this;

    menu.action = function (menuItem) {
        var formSelectedItem = form.getSelectedItem();
        var currentContainer = this.currentContainer || (formSelectedItem ? formSelectedItem.container : null);
        if (currentContainer) {
            var selectedItem = currentContainer.item || currentContainer.selectedItem;

            if (menuItem.key == "newItem") {
                form.sendCommand({ command: "NewItem", containerName: currentContainer.name, oldSeriesType: form.oldSeriesType },
                    function (answer) {
                        var resultMeters = answer.elementProperties.meters[currentContainer.name];
                        currentContainer.updateMeters(resultMeters, resultMeters.length - 1);
                        form.updateSvgContent(answer.elementProperties.svgContent);
                        if (form.updateElementProperties) form.updateElementProperties(answer.elementProperties);
                        form.checkStartMode();
                    }
                );
            }
            else if (selectedItem) {
                var itemIndex = currentContainer.getItemIndex(selectedItem);

                if (menuItem.key.indexOf("Function_") == 0) {
                    form.sendCommand(
                        {
                            command: "SetFunction",
                            containerName: currentContainer.name,
                            itemIndex: itemIndex,
                            function: menuItem.key.replace("Function_", "")
                        },
                        function (answer) {
                            currentContainer.updateMeters(answer.elementProperties.meters[currentContainer.name], itemIndex);
                            form.updateSvgContent(answer.elementProperties.svgContent);
                        }
                    );
                }
                else {
                    switch (menuItem.key) {
                        case "renameField": {
                            selectedItem.setEditable(true);
                            break;
                        }
                        case "removeField": {
                            selectedItem.remove();
                            break;
                        }
                        case "removeAllFields": {
                            form.sendCommand({ command: "RemoveAllMeters", containerName: currentContainer.name },
                                function (answer) {
                                    currentContainer.updateMeters([]);
                                    form.updateSvgContent(answer.elementProperties.svgContent);
                                    form.checkStartMode();
                                }
                            );
                            break;
                        }
                        case "editExpression": {
                            jsObject.InitializeExpressionEditorForm(function (expForm) {
                                var propertiesPanel = jsObject.options.propertiesPanel;
                                expForm.propertiesPanelZIndex = propertiesPanel.style.zIndex;
                                expForm.propertiesPanelIsEnabled = propertiesPanel.isEnabled;
                                expForm.resultControl = expressionControl;
                                expForm.changeVisibleState(true);
                            });
                            break;
                        }
                        case "duplicateField": {
                            form.sendCommand({ command: "DuplicateMeter", containerName: currentContainer.name, itemIndex: itemIndex },
                                function (answer) {
                                    var resultMeters = answer.elementProperties.meters[currentContainer.name];
                                    currentContainer.updateMeters(resultMeters, answer.insertIndex);
                                    form.updateSvgContent(answer.elementProperties.svgContent);
                                    form.checkStartMode();
                                }
                            );
                            break;
                        }
                    }
                }
            }
        }

        this.changeVisibleState(false);
    }

    menu.onshow = function () {
        var formSelectedItem = form.getSelectedItem();
        var currentContainer = this.currentContainer || (formSelectedItem ? formSelectedItem.container : null);
        if (!currentContainer) return;
        var selectedItem = currentContainer.item || currentContainer.selectedItem;
        var variables = jsObject.options.report ? jsObject.GetVariablesFromDictionary(jsObject.options.report.dictionary) : [];
        var items = [];
        items.push(jsObject.Item("newItem", jsObject.loc.Dashboard.NewField, "Empty16.png", "newItem"));

        if (selectedItem && expressionControl.isEnabled) {
            items.push("separator1");
            if (currentContainer.multiItems) {
                items.push(jsObject.Item("duplicateField", jsObject.loc.Dashboard.DuplicateField, "Duplicate.png", "duplicateField"));
            }
            items.push(jsObject.Item("editExpression", jsObject.loc.Dashboard.EditExpression, "EditButton.png", "editExpression"));
            items.push(jsObject.Item("renameField", jsObject.loc.Buttons.Rename, "DataColumn.png", "renameField"));
            items.push(jsObject.Item("removeField", jsObject.loc.Dashboard.RemoveField, "Remove.png", "removeField"));
            if (currentContainer.multiItems) {
                items.push(jsObject.Item("removeAllFields", jsObject.loc.Dashboard.RemoveAllFields, "Empty16.png", "removeAllFields"));
            }

            if (variables.length > 0) {
                items.push("separator1_1");
                items.push(jsObject.Item("variable", jsObject.loc.PropertyMain.Variable, "Empty16.png", "variable", null, true));
            }

            if (selectedItem.itemObject) {
                var functions = selectedItem.itemObject.functions;
                if (functions && functions.length > 0) {
                    items.push("separator2");
                    for (var i = 0; i < functions.length; i++) {
                        if (i != 0 && functions[i] == "First") items.push("separator3");
                        items.push(jsObject.Item("Function_" + functions[i], jsObject.aggregateFunctionToHumanText(functions[i]), "CheckBox.png", "Function_" + functions[i], null, null, null, { width: 12, height: 12 }));
                    }
                }
            }
        }

        this.addItems(items);

        if (selectedItem && expressionControl.isEnabled) {
            for (var itemName in this.items) {
                if (itemName.indexOf("Function_") == 0) {
                    var funcItem = this.items[itemName];
                    var isSelected = selectedItem && selectedItem.itemObject.currentFunction &&
                        itemName.toLowerCase() == ("Function_" + selectedItem.itemObject.currentFunction).toLowerCase();
                    if (funcItem.cellImage) funcItem.cellImage.style.padding = "0 7px 0 7px";
                    funcItem.caption.style.fontWeight = isSelected ? "bold" : "normal";
                    funcItem.image.style.visibility = isSelected ? "visible" : "hidden";
                }
            }

            if (this.items.variable) {
                var itemIndex = currentContainer.getItemIndex(selectedItem);
                var varItems = [];
                for (var i = 0; i < variables.length; i++) {
                    varItems.push(jsObject.Item(variables[i].name, variables[i].name, "Empty16.png", variables[i].name));
                }
                var variablesMenu = jsObject.InitializeSubMenu(name ? name + "_VariablesSubMenu" : null, varItems, this.items.variable, this);
                variablesMenu.action = function (item) {
                    form.sendCommand(
                        {
                            command: "SetExpression",
                            itemIndex: itemIndex,
                            containerName: currentContainer.name,
                            expressionValue: StiBase64.encode(jsObject.CheckExpressionBrackets(item.key))
                        },
                        function (answer) {
                            var container = form.controls[currentContainer.name + "Block"].container;
                            container.updateMeters(answer.elementProperties.meters[currentContainer.name], itemIndex);
                            form.updateSvgContent(answer.elementProperties.svgContent);
                        }
                    );
                    menu.changeVisibleState(false);
                }
            }
        }
    }

    menu.onhide = function () {
        this.currentContainer = null;
    }

    if (expressionControl && expressionControl.editButton) {
        expressionControl.editButton.action = function () {
            menu.action({ key: "editExpression" });
        }
    }

    return menu;
}