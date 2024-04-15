
StiMobileDesigner.prototype.InitializeHyperlinkEditorForm_ = function () {
    var form = this.BaseForm("hyperlinkEditorForm", this.loc.PropertyMain.Hyperlink, 2, this.HelpLinks["dashboardInteractions"]);
    var expButton = this.SmallButton(null, null, null, "Function.png", this.loc.FormRichTextEditor.Insert, "Down", "stiDesignerFormButton");
    expButton.style.display = "inline-block";
    expButton.style.margin = "12px 12px 10px 12px";
    form.container.appendChild(expButton);

    var expMenu = this.VerticalMenu("insertExpressionMenu", expButton, "Down", this.GetInsertExpressionItems());

    expButton.action = function () {
        expMenu.changeVisibleState(!expMenu.visible);
    }

    var sep = this.FormSeparator();
    sep.style.margin = "0px 12px 0 12px";
    form.container.appendChild(sep);

    form.textArea = this.TextArea("textEditorOnlyTextTextArea", 600, 400);
    form.textArea.style.margin = "12px";
    form.textArea.style.border = "0";
    form.container.appendChild(form.textArea);
    form.textArea.addInsertButton();
    form.addBrackets = true;

    expMenu.action = function (menuItem) {
        if (form.addBrackets) {
            form.textArea.insertText("{" + menuItem.key + "}");
        } else {
            form.textArea.insertText(menuItem.key);
        }
        expMenu.changeVisibleState(false);
    }

    form.show = function (control) {
        var propPanel = form.jsObject.options.propertiesPanel;
        if (propPanel) {
            form.oldDictionaryMode = propPanel.dictionaryMode;
            form.oldDictionaryZIndex = propPanel.style.zIndex;
            propPanel.style.zIndex = propPanel.showButtonsPanel.style.zIndex = form.level * 10 + 1;
            propPanel.setDictionaryMode(true);
            propPanel.setEnabled(true);
            propPanel.editFormControl = form.textArea;
        }
        if (control.interactionIdent == "expandExpression") {
            form.addBrackets = false;
            form.caption.innerHTML = form.jsObject.loc.Dashboard.EditExpression;
        } else {
            form.caption.innerHTML = form.jsObject.loc.PropertyMain.Hyperlink;
        }
        form.textArea.value = control.textBox.value;
        expButton.style.display = control.interactionIdent != "Image" ? "inline-block" : "none";
        expMenu.addItems(this.jsObject.GetInsertExpressionItems(control.interactionIdent, control.columnNames, control.chartIsRange));
        form.changeVisibleState(true);
        form.textArea.focus();
    }

    form.onhide = function () {
        var propPanel = form.jsObject.options.propertiesPanel;
        if (propPanel) {
            propPanel.setDictionaryMode(form.oldDictionaryMode);
            propPanel.style.zIndex = propPanel.showButtonsPanel.style.zIndex = form.oldDictionaryZIndex;
            propPanel.editFormControl = null;
        }
    }

    return form;
}