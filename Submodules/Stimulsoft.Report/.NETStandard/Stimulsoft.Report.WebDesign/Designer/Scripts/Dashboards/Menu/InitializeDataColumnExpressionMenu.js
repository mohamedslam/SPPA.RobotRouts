
StiMobileDesigner.prototype.InitializeDataColumnExpressionMenu = function (name, expressionControl, form) {
    var menu = this.BaseContextMenu(name, "Down", null);
    var jsObject = this;

    menu.onshow = function () {
        var currentContainer = this.currentContainer || expressionControl.currentContainer;
        var variables = jsObject.options.report ? jsObject.GetVariablesFromDictionary(jsObject.options.report.dictionary) : [];
        var items = [];
        items.push(jsObject.Item("newItem", jsObject.loc.Dashboard.NewField, "Empty16.png", "newItem"));

        if (currentContainer && currentContainer.dataColumnObject && expressionControl.isEnabled) {
            items.push("separator1");
            items.push(jsObject.Item("editExpression", jsObject.loc.Dashboard.EditExpression, "EditButton.png", "editExpression"));
            items.push(jsObject.Item("renameField", jsObject.loc.Buttons.Rename, "DataColumn.png", "renameField"));
            items.push(jsObject.Item("removeField", jsObject.loc.Dashboard.RemoveField, "Remove.png", "removeField"));

            if (variables.length > 0) {
                items.push("separator1_1");
                items.push(jsObject.Item("variable", jsObject.loc.PropertyMain.Variable, "Empty16.png", "variable", null, true));
            }

            if (currentContainer.dataColumnObject) {
                var functions = currentContainer.dataColumnObject.functions;
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

        if (currentContainer && currentContainer.dataColumnObject && expressionControl.isEnabled) {
            for (var itemName in this.items) {
                if (itemName.indexOf("Function_") == 0) {
                    var funcItem = this.items[itemName];
                    var isSelected = currentContainer.dataColumnObject && currentContainer.dataColumnObject.currentFunction &&
                        itemName.toLowerCase() == ("Function_" + currentContainer.dataColumnObject.currentFunction).toLowerCase();

                    if (funcItem.cellImage) funcItem.cellImage.style.padding = "0 7px 0 7px";
                    funcItem.caption.style.fontWeight = isSelected ? "bold" : "normal";
                    funcItem.image.style.visibility = isSelected ? "visible" : "hidden";
                }
            }

            if (this.items.variable) {
                var varItems = [];
                for (var i = 0; i < variables.length; i++) {
                    varItems.push(jsObject.Item(variables[i].name, variables[i].name, "Empty16.png", variables[i].name));
                }
                var variablesMenu = jsObject.InitializeSubMenu(name ? name + "_VariablesSubMenu" : null, varItems, this.items.variable, this);
                variablesMenu.action = function (item) {
                    form.sendCommand(
                        {
                            command: "SetExpression",
                            containerName: jsObject.UpperFirstChar(currentContainer.name),
                            expressionValue: StiBase64.encode(jsObject.CheckExpressionBrackets(item.key))
                        },
                        function (answer) {
                            form.updateControls(answer.elementProperties);
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

    menu.action = function (menuItem) {
        var currentContainer = this.currentContainer || expressionControl.currentContainer;
        if (currentContainer) {
            if (menuItem.key.indexOf("Function_") == 0) {
                form.sendCommand(
                    {
                        command: "SetFunction",
                        containerName: jsObject.UpperFirstChar(currentContainer.name),
                        function: menuItem.key.replace("Function_", "")
                    },
                    function (answer) {
                        form.updateControls(answer.elementProperties);
                        form.updateSvgContent(answer.elementProperties.svgContent);
                    }
                );
            }
            else {
                switch (menuItem.key) {
                    case "renameField": {
                        if (currentContainer.item) currentContainer.item.setEditable(true);
                        break;
                    }
                    case "removeField": {
                        currentContainer.clear();
                        currentContainer.action();
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
                    case "newItem": {
                        form.sendCommand(
                            {
                                command: "NewItem",
                                containerName: jsObject.UpperFirstChar(currentContainer.name)
                            },
                            function (answer) {
                                form.updateControls(answer.elementProperties);
                                form.updateSvgContent(answer.elementProperties.svgContent);
                                if (form.checkStartMode) form.checkStartMode();

                                if (currentContainer.item) {
                                    currentContainer.item.action();
                                }
                            }
                        );
                        break;
                    }
                }
            }
        }
        this.changeVisibleState(false);
    }

    if (expressionControl && expressionControl.editButton) {
        expressionControl.editButton.action = function () {
            menu.action({ key: "editExpression" });
        }
    }

    return menu;
}