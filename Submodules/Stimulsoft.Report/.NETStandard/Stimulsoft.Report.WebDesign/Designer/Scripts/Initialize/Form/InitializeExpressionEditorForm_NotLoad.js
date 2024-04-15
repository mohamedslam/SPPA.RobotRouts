
StiMobileDesigner.prototype.InitializeExpressionEditorForm_ = function () {
    return this.InitializeBaseExpressionEditorForm("expressionEditor", this.loc.PropertyMain.Expression, 3, this.HelpLinks["expression"]);
}

StiMobileDesigner.prototype.InitializeBaseExpressionEditorForm = function (name, caption, level, helpUrl) {
    var jsObject = this;
    var form = this.BaseFormPanel(name, caption, level, helpUrl);

    //Expression
    var textArea = this.TextArea(null, 436, 440);
    textArea.style.margin = "12px";
    form.expressionTextArea = textArea;
    form.container.appendChild(textArea);

    if (!form.resultControl || !form.resultControl.notShowDictionary) {
        textArea.addInsertButton();

        textArea.insertButton.action = function () {
            var dictionaryTree = jsObject.options.dictionaryTree;
            if (dictionaryTree && dictionaryTree.selectedItem) {
                var text = dictionaryTree.selectedItem.getResultForEditForm();
                if (form.resultControl) {
                    if (form.resultControl.checkExpressionBrackets) {
                        text = jsObject.CheckExpressionBrackets(dictionaryTree.selectedItem.getResultForEditForm(true, true));
                    }
                    if (form.resultControl.cutBrackets && text.indexOf("{") == 0 && jsObject.EndsWith(text, "}")) {
                        text = text.substr(1, text.length - 2);
                    }
                    if (form.resultControl.eventName && jsObject.options.jsMode) {
                        //for events expression in JS
                        var dataArray = text.split(".");
                        if (dataArray.length == 2) {
                            text = dataArray[0] + ".getData(\"" + dataArray[1] + "\")";
                        }
                    }
                }
                textArea.insertText(text);
            }
        }
    }

    form.onhide = function () {
        if (form.resultControl.notShowDictionary) return;
        var propertiesPanel = jsObject.options.propertiesPanel;
        propertiesPanel.setDictionaryMode(false);
        propertiesPanel.setEnabled(form.propertiesPanelIsEnabled);
        propertiesPanel.style.zIndex = form.propertiesPanelZIndex;
    }

    form.onshow = function () {
        if (!form.resultControl || !form.resultControl.notShowDictionary) {
            var propertiesPanel = jsObject.options.propertiesPanel;
            propertiesPanel.setDictionaryMode(true);
            propertiesPanel.setEnabled(true);
            propertiesPanel.editFormControl = textArea;
        }
        if (form.resultControl) {
            var resultTextBox = form.resultControl.textBox || form.resultControl;
            textArea.value = resultTextBox.useHiddenValue ? resultTextBox.hiddenValue : resultTextBox.value;
            textArea.cutBrackets = form.resultControl.cutBrackets;
        }
        textArea.focus();
    }

    form.action = function () {
        if (form.resultControl) {
            var resultTextBox = form.resultControl.textBox || form.resultControl;
            resultTextBox.value = textArea.value;
            if (resultTextBox.readOnly && resultTextBox.useHiddenValue) resultTextBox.hiddenValue = textArea.value;
            if (form.resultControl.action) form.resultControl.action();
        }
        form.changeVisibleState(false);
    }

    return form;
}