
StiMobileDesigner.prototype.PropertyEnumExpressionControl = function (name, width, items, readOnly, showImage, toolTip, cutMenu) {
    var jsObject = this;

    items.push(jsObject.Item("expGroupHeader", jsObject.loc.PropertyMain.Expression, null, "expGroupHeader"));
    items.push(jsObject.Item("expressionItem", " ", null, "expressionItem"));
    items.push("separator_exp");
    items.push(jsObject.Item("editExpression", jsObject.loc.FormRichTextEditor.Insert, null, "editExpression"));

    var control = this.PropertyDropDownList(name, width, items, readOnly, showImage, toolTip, cutMenu);
    control.cutBrackets = true;
    if (name) this.options.controls[name] = control;

    var repaintHeaderItem = function () {
        var expGroupHeaderItem = control.menu.items.expGroupHeader;
        expGroupHeaderItem.style.color = "#949494";
        expGroupHeaderItem.style.background = "#eeeeee";
        expGroupHeaderItem.onmouseout = null;
        expGroupHeaderItem.onmouseover = null
        expGroupHeaderItem.action = function () { };
    }

    repaintHeaderItem();

    control.menu.addItems_ = control.menu.addItems;

    control.menu.addItems = function (items) {
        if (items) {
            items.push(jsObject.Item("expGroupHeader", jsObject.loc.PropertyMain.Expression, null, "expGroupHeader"));
            items.push(jsObject.Item("expressionItem", " ", null, "expressionItem"));
            items.push("separator_exp");
            items.push(jsObject.Item("editExpression", jsObject.loc.FormRichTextEditor.Insert, null, "editExpression"));
        }
        this.addItems_(items);
        repaintHeaderItem();
    }

    control.menu.onshow = function () {
        this.items.expGroupHeader.style.display = this.items.expressionItem.style.display = control.expression ? "" : "none";
        this.items.editExpression.caption.innerHTML = control.expression ? jsObject.loc.Dashboard.EditExpression : jsObject.loc.FormRichTextEditor.Insert;
        this.items.expressionItem.caption.innerHTML = control.expression || "";

        var key = control.expression || this.dropDownList.key;
        if (key == null) return;

        for (var itemName in this.items) {
            if (key == this.items[itemName].key || (this.items[itemName].caption && key == this.items[itemName].caption.innerHTML)) {
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
            case "editExpression": {
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
                        form.expressionTextArea.value = control.expression || "";
                        form.expressionTextArea.focus();
                    }

                    form.action = function () {
                        control.setKey(control.key, form.expressionTextArea.value);
                        form.changeVisibleState(false);
                        control.action();
                    }

                    form.changeVisibleState(true);
                });
                break;
            }
            case "expressionItem": {
                control.setKey(control.key, control.expression);
                control.action();
                break
            }
            default: {
                control.setKey(item.key);
                control.action();
            }
        }
    }

    control.setKey = function (key, expression) {
        this.key = key;
        this.expression = expression;

        if (expression) {
            this.textBox.value = expression;
        }
        else {
            if (key == null) return;
            if (key == "StiEmptyValue") {
                this.textBox.value = "";
                return;
            }
            if (this.items && this.items.length) {
                for (var i = 0; i < this.items.length; i++)
                    if (key == this.items[i].key) {
                        this.textBox.value = this.items[i].caption;
                        if (this.image) StiMobileDesigner.setImageSource(this.image, this.jsObject.options, this.items[i].imageName);
                        return;
                    }
            }
            this.textBox.value = key.toString();
        }
    }

    control.action = function () { };

    return control;
}