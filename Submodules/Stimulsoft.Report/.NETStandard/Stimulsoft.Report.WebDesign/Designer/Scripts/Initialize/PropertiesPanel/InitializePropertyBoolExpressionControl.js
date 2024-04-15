
StiMobileDesigner.prototype.PropertyBoolExpressionControl = function (name, width) {
    var jsObject = this;

    var items = [];
    items.push(jsObject.Item("itemTrue", jsObject.loc.PropertyEnum.StiExtendedStyleBoolTrue, null, "True"));
    items.push(jsObject.Item("itemFalse", jsObject.loc.PropertyEnum.StiExtendedStyleBoolFalse, null, "False"));
    items.push(jsObject.Item("expGroupHeader", jsObject.loc.PropertyMain.Expression, null, "expGroupHeader"));
    items.push(jsObject.Item("expressionItem", " ", null, "expressionItem"));
    items.push("separator_exp");
    items.push(jsObject.Item("editExpression", jsObject.loc.FormRichTextEditor.Insert, null, "EditExpression"));

    var control = this.PropertyDropDownList(name, width, items, true);
    control.cutBrackets = true;
    if (name) this.options.controls[name] = control;

    var expGroupHeaderItem = control.menu.items.expGroupHeader;
    expGroupHeaderItem.style.color = "#949494";
    expGroupHeaderItem.style.background = "#eeeeee";
    expGroupHeaderItem.onmouseout = null;
    expGroupHeaderItem.onmouseover = null
    expGroupHeaderItem.action = function () { };

    control.menu.onshow = function () {
        var isExpression = control.key != "True" && control.key != "False";
        this.items.expGroupHeader.style.display = this.items.expressionItem.style.display = isExpression ? "" : "none";
        this.items.editExpression.caption.innerHTML = isExpression ? jsObject.loc.Dashboard.EditExpression : jsObject.loc.FormRichTextEditor.Insert;
        this.items.expressionItem.caption.innerHTML = control.key;

        if (this.dropDownList.key == null) return;

        for (var itemName in this.items) {
            if (this.dropDownList.key == this.items[itemName].key) {
                this.items[itemName].setSelected(true);
                return;
            }
            else if (itemName.indexOf("separator") != 0) {
                this.items[itemName].setSelected(false);
            }
        }
    }

    control.menu.action = function (item) {
        this.changeVisibleState(false);

        switch (item.key) {
            case "True":
            case "False": {
                control.setKey(item.key);
                control.action();
                break;
            }
            case "EditExpression": {
                jsObject.InitializeExpressionEditorForm(function (form) {
                    var propertiesPanel = jsObject.options.propertiesPanel;
                    form.propertiesPanelZIndex = propertiesPanel.style.zIndex;
                    form.propertiesPanelIsEnabled = propertiesPanel.isEnabled;
                    form.resultControl = control;

                    form.onshow = function () {
                        var propertiesPanel = jsObject.options.propertiesPanel;
                        propertiesPanel.setDictionaryMode(true);
                        propertiesPanel.setEnabled(true);
                        propertiesPanel.editFormControl = form.expressionTextArea;
                        form.expressionTextArea.value = (control.key == "True" || control.key == "False") ? "" : control.key;
                        form.expressionTextArea.focus();
                    }

                    form.action = function () {
                        control.setKey(form.expressionTextArea.value);
                        form.changeVisibleState(false);
                        control.action();
                    }

                    form.changeVisibleState(true);
                });
                break;
            }
        }
    }

    control.action = function () { };

    return control;
}