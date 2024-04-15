
StiMobileDesigner.prototype.InitializeDataContainerExpressionMenu = function (name, expressionControl, dataContainer, form) {
    var menu = this.BaseContextMenu(name, "Down", null);
    menu.rightToLeft = true;
    var jsObject = this;

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
        var itemIndex = dataContainer.getSelectedItemIndex();

        if (menuItem.key.indexOf("Function_") == 0) {
            form.sendCommand({ command: "SetFunction", itemIndex: itemIndex, function: menuItem.key.replace("Function_", "") },
                function (answer) {
                    dataContainer.updateMeters(answer.elementProperties.meters, itemIndex);
                    form.updateSvgContent(answer.elementProperties.svgContent);
                }
            );
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
                form.sendCommand({ command: "RemoveAllMeters" },
                    function (answer) {
                        dataContainer.updateMeters(answer.elementProperties.meters);
                        form.updateSvgContent(answer.elementProperties.svgContent);
                    }
                );
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
                var insertIndex = itemIndex + 1;
                if (insertIndex >= dataContainer.getCountItems()) insertIndex = -1;

                form.sendCommand({ command: "NewMeter", insertIndex: insertIndex, itemType: menuItem.key },
                    function (answer) {
                        dataContainer.updateMeters(answer.elementProperties.meters, insertIndex != -1 ? insertIndex : answer.elementProperties.meters.length - 1);
                        form.updateSvgContent(answer.elementProperties.svgContent);
                    }
                );
                break;
            }
            case "duplicateField": {
                form.sendCommand({ command: "DuplicateMeter", itemIndex: itemIndex },
                    function (answer) {
                        dataContainer.updateMeters(answer.elementProperties.meters, answer.insertIndex);
                        form.updateSvgContent(answer.elementProperties.svgContent);
                    }
                );
                break;
            }
        }
    }

    menu.onshow = function () {
        var variables = jsObject.options.report ? jsObject.GetVariablesFromDictionary(jsObject.options.report.dictionary) : [];
        var items = [];
        items.push(jsObject.Item("newDimension", jsObject.loc.Dashboard.NewDimension, "Meters.Dimension.png", "newDimension"));
        items.push(jsObject.Item("newMeasure", jsObject.loc.Dashboard.NewMeasure, "Meters.Measure.png", "newMeasure"));

        if (dataContainer.selectedItem) {
            items.push("separator1");
            items.push(jsObject.Item("duplicateField", jsObject.loc.Dashboard.DuplicateField, "Duplicate.png", "duplicateField"));
            items.push(jsObject.Item("editExpression", jsObject.loc.Dashboard.EditExpression, "EditButton.png", "editExpression"));
            items.push(jsObject.Item("renameField", jsObject.loc.Buttons.Rename, "DataColumn.png", "renameField"));
            items.push(jsObject.Item("removeField", jsObject.loc.Dashboard.RemoveField, "Remove.png", "removeField"));
            items.push(jsObject.Item("removeAllFields", jsObject.loc.Dashboard.RemoveAllFields, "Empty16.png", "removeAllFields"));

            if (variables.length > 0 && name == "tableElementExpressionMenu") {
                items.push("separator1_1");
                items.push(jsObject.Item("variable", jsObject.loc.PropertyMain.Variable, "Empty16.png", "variable", null, true));
            }

            var functions = dataContainer.selectedItem.itemObject.functions;
            if (functions && functions.length > 0) {
                items.push("separator2");
                for (var i = 0; i < functions.length; i++) {
                    if (i != 0 && functions[i] == "First") items.push("separator3");
                    items.push(jsObject.Item("Function_" + functions[i], jsObject.aggregateFunctionToHumanText(functions[i]), "CheckBox.png", "Function_" + functions[i], null, null, null, { width: 12, height: 12 }));
                }
            }
        }

        this.addItems(items);

        for (var itemName in this.items) {
            if (itemName.indexOf("Function_") == 0) {
                var funcItem = this.items[itemName];
                var isSelected = dataContainer.selectedItem && dataContainer.selectedItem.itemObject.currentFunction &&
                    itemName.toLowerCase() == ("Function_" + dataContainer.selectedItem.itemObject.currentFunction).toLowerCase();

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
            var varMenu = jsObject.InitializeSubMenu(name ? name + "_VariablesSubMenu" : null, varItems, this.items.variable, this);

            varMenu.action = function (item) {
                form.setPropertyValue("Expression", StiBase64.encode(jsObject.CheckExpressionBrackets(item.key)));
                menu.changeVisibleState(false);
            }
        }
    }

    if (expressionControl && expressionControl.editButton) {
        expressionControl.editButton.action = function () {
            menu.action({ key: "editExpression" });
        }
    }

    return menu;
}