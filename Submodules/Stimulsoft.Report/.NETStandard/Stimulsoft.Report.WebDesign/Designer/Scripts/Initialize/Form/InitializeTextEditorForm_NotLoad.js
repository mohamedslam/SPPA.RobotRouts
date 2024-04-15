
StiMobileDesigner.prototype.InitializeTextEditorForm_ = function () {
    var jsObject = this;

    //Text Editor Form
    var textEditorForm = this.BaseFormPanel("textEditor", this.loc.PropertyMain.Text, 3, this.HelpLinks["expression"]);
    textEditorForm.propertyName = "";
    textEditorForm.dataTree = this.options.dataTree;
    textEditorForm.mode = "Expression";

    this.AddProgressToControl(textEditorForm);

    //Main Table
    var mainTable = this.CreateHTMLTable();
    mainTable.className = "stiDesignerImageFormMainPanel";
    textEditorForm.container.appendChild(mainTable);
    textEditorForm.container.style.padding = "0px";

    //Buttons
    var buttonProps = [
        ["Expression", "TextForm.TextExpression.png", this.loc.PropertyMain.Expression],
        ["DataColumn", "TextForm.TextDataColumn.png", this.loc.PropertyMain.DataColumn],
        ["SystemVariable", "TextForm.TextSystemVariable.png", this.loc.PropertyMain.SystemVariable],
        ["SummaryText", "TextForm.TextSummary.png", this.loc.PropertyMain.Summary],
        ["RichText", "TextForm.TextHtml.png", "HTML"]
    ];

    //Add Panels && Buttons
    var buttonsPanel = mainTable.addCell();
    var panelsContainer = mainTable.addCell();
    buttonsPanel.style.verticalAlign = "top";
    buttonsPanel.style.paddingTop = "6px";
    textEditorForm.mainButtons = {};
    textEditorForm.panels = {};

    for (var i = 0; i < buttonProps.length; i++) {
        var panel = document.createElement("Div");
        panel.className = "stiDesignerEditFormPanel";
        panel.style.width = this.options.isTouchDevice ? "600px" : "550px";
        panel.style.display = i != 0 ? "none" : "inline-block";
        panelsContainer.appendChild(panel);
        textEditorForm.panels[buttonProps[i][0]] = panel;

        var button = this.FormTabPanelButton(null, buttonProps[i][2], buttonProps[i][1], buttonProps[i][2], null, { width: 24, height: 24 }, 34);
        textEditorForm.mainButtons[buttonProps[i][0]] = button;
        buttonsPanel.appendChild(button);
        button.panelName = buttonProps[i][0];
        button.action = function () {
            textEditorForm.setMode(this.panelName);
        }
    }

    //Expression
    var expTextArea = textEditorForm.expressionTextArea = this.TextArea("textEditorFormExpression", this.options.isTouchDevice ? 569 : 519, 422);
    expTextArea.style.margin = "12px 0 0 12px";
    textEditorForm.panels.Expression.appendChild(expTextArea);
    expTextArea.addInsertButton();

    expTextArea.insertButton.action = function () {
        var dictionaryTree = jsObject.options.dictionaryTree;
        if (dictionaryTree && dictionaryTree.selectedItem) {
            var resultDictItem = dictionaryTree.selectedItem.getResultForEditForm();
            expTextArea.insertText(resultDictItem);
        }
    }

    //Check Expression
    var checkExpression = this.FormButton(null, null, this.loc.Buttons.Check);
    checkExpression.style.margin = "12px";
    checkExpression.style.maxWidth = "50px";

    textEditorForm.buttonsPanel.style.width = "100%";
    textEditorForm.buttonsPanel.firstChild.style.width = "100%";
    textEditorForm.buttonsPanel.firstChild.tr[0].insertCell(0).appendChild(checkExpression);
    textEditorForm.buttonOk.parentElement.style.width = "1px";
    textEditorForm.buttonCancel.parentElement.style.width = "1px";

    checkExpression.action = function (autoStarted) {
        checkExpression.setEnabled(false);
        jsObject.SendCommandCheckExpression(
            StiBase64.encode(expTextArea.value), function (answer) {
                checkExpression.setEnabled(true);
                if (answer.checkResult) {
                    var message = StiBase64.decode(answer.checkResult);
                    if (message == "OK" && autoStarted) return;
                    jsObject.InitializeCheckExpressionPopupPanel(function (checkPopupPanel) {
                        checkPopupPanel.show(message, checkExpression);
                    });
                }
            });
    };

    //DataColumn
    var columnContainer = this.SystemVariablesTree();
    columnContainer.className = "stiSimpleContainerWithBorder";
    columnContainer.style.margin = "12px 0 0 12px";
    columnContainer.style.overflow = "auto";
    columnContainer.style.height = "390px";
    columnContainer.style.width = this.options.isTouchDevice ? "573px" : "523px";
    textEditorForm.panels.DataColumn.appendChild(columnContainer);

    var nullValTable = this.CreateHTMLTable();
    nullValTable.style.margin = "12px 0 0 5px";
    textEditorForm.panels.DataColumn.appendChild(nullValTable);
    nullValTable.addTextCell(this.loc.FormSystemTextEditor.LabelShowInsteadNullValues.replace(":", "")).className = "stiDesignerCaptionControls";
    var nullValuesControl = this.DropDownList("textFormNullValues", 125, null, this.GetInsteadNullValuesItems(), true);
    nullValTable.addCell(nullValuesControl);

    //System Variables
    var sysTreeCont = document.createElement("div");
    sysTreeCont.className = "stiSimpleContainerWithBorder";
    sysTreeCont.style.margin = "12px 0 0 12px";
    sysTreeCont.style.height = "422px";
    sysTreeCont.style.width = this.options.isTouchDevice ? "573px" : "523px";
    textEditorForm.panels.SystemVariable.appendChild(sysTreeCont);

    var systemVarsTree = this.SystemVariablesTree();
    systemVarsTree.style.height = "350px";
    systemVarsTree.style.overflow = "auto";
    sysTreeCont.appendChild(systemVarsTree);

    var systemVarsInfoPanel = document.createElement("div");
    systemVarsInfoPanel.className = "stiDesignerTextEditFormSystemVariablesInfo";
    sysTreeCont.appendChild(systemVarsInfoPanel);

    systemVarsTree.onSelectedItem = function (item) {
        systemVarsInfoPanel.innerHTML = jsObject.GetSystemVariableDescription(item.itemObject.name);
    };
    systemVarsTree.action = function () { textEditorForm.action(); };

    //Summary
    var summaryText = this.SummaryExpression("textEditorFormSummaryText");
    summaryText.style.height = "440px";
    textEditorForm.panels.SummaryText.appendChild(summaryText);

    //RichText
    var richTextEditor = this.RichTextEditor("textEditorFormRichText", this.options.isTouchDevice ? 573 : 523, 384, null, true);
    textEditorForm.panels.RichText.appendChild(richTextEditor);

    richTextEditor.action = function () {
        richTextEditor.isModified = true;
    }

    //Form Methods
    textEditorForm.reset = function () {
        textEditorForm.dataTree.setKey("");
        expTextArea.value = "";
        summaryText.reset();
        textEditorForm.setMode("Expression");
    }

    textEditorForm.setMode = function (mode) {
        textEditorForm.mode = mode;
        for (var panelName in textEditorForm.panels) {
            textEditorForm.panels[panelName].style.display = mode == panelName ? "inline-block" : "none";
            textEditorForm.mainButtons[panelName].setSelected(mode == panelName);
        }
        var propertiesPanel = jsObject.options.propertiesPanel;
        propertiesPanel.setEnabled(mode == "Expression" || mode == "RichText");
        propertiesPanel.editFormControl = null;
        if (mode == "Expression") {
            propertiesPanel.editFormControl = expTextArea;
            if (richTextEditor.isModified) expTextArea.value = richTextEditor.getText(true);
            expTextArea.focus();
        }
        else if (mode == "RichText") {
            var selectedObject = jsObject.options.selectedObject;
            var text = expTextArea.value;
            richTextEditor.setText(text, selectedObject ? jsObject.FontStrToObject(selectedObject.properties.font) : null, true);
            propertiesPanel.editFormControl = richTextEditor;
        }
        else if (mode == "SummaryText") {
            summaryText.controls.expressionTextArea.focus();
        }
        checkExpression.style.display = mode == "Expression" ? "" : "none";
    }

    textEditorForm.setDictionaryToTextEditorMode = function (state) {
        var dictTree = jsObject.options.dictionaryTree;
        if (dictTree && dictTree.mainItems.Formats && dictTree.mainItems.HtmlTags) {
            dictTree.mainItems.Formats.style.display = dictTree.mainItems.HtmlTags.style.display = state ? "" : "none";
        }
    }

    textEditorForm.onhide = function () {
        jsObject.options.propertiesPanel.setDictionaryMode(textEditorForm.oldDictionaryMode);
        this.setDictionaryToTextEditorMode(false);
    }

    textEditorForm.onshow = function () {
        richTextEditor.isModified = false;
        textEditorForm.setMode("Expression");
        textEditorForm.container.style.visibility = "hidden";
        var currentObject = jsObject.options.selectedObject || jsObject.GetCommonObject(jsObject.options.selectedObjects);
        textEditorForm.mainButtons.RichText.style.display = currentObject && currentObject.typeComponent == "StiText" ? "" : "none";
        textEditorForm.oldDictionaryMode = jsObject.options.propertiesPanel.dictionaryMode;
        nullValuesControl.setKey(currentObject.properties.nullValue);
    }

    textEditorForm.oncompleteshow = function () {
        if (this.showFunction) {
            this.showFunction();
            this.showFunction = null;
            return;
        }

        textEditorForm.progress.show();

        setTimeout(function () {
            textEditorForm.mainButtons.SummaryText.style.display = textEditorForm.propertyName == "text" ? "" : "none";
            jsObject.options.propertiesPanel.setDictionaryMode(true);
            textEditorForm.setDictionaryToTextEditorMode(true);

            //Build SystemVariables Tree
            systemVarsInfoPanel.innerHTML = "";
            systemVarsTree.build();
            if (systemVarsTree.firstChild && systemVarsTree.firstChild.childsContainer.childNodes.length > 0) {
                systemVarsTree.firstChild.childsContainer.childNodes[0].setSelected();
            }

            //Build Data Tree
            columnContainer.appendChild(textEditorForm.dataTree);
            textEditorForm.dataTree.build(null, null, null, true);
            textEditorForm.dataTree.action = function () {
                textEditorForm.action();
            }

            //Reset Controls
            textEditorForm.reset();

            if (jsObject.options.selectedObjects) {
                textEditorForm.setMode("Expression");
                expTextArea.focus();
            }
            else {
                var selectedObject = jsObject.options.selectedObject;

                var propertyValue = selectedObject.properties[textEditorForm.propertyName] != null
                    ? StiBase64.decode(selectedObject.properties[textEditorForm.propertyName])
                    : (textEditorForm.resultControl != null ? textEditorForm.resultControl.value : "");

                var textType = selectedObject.properties.textType;
                var subStringPropertyValue = propertyValue.length > 1 ? propertyValue.substring(1, propertyValue.length - 1) : "";
                expTextArea.value = propertyValue;


                if (textType == "DataColumn" || textEditorForm.dataTree.setKey(subStringPropertyValue)) {
                    if (textType == "DataColumn") textEditorForm.dataTree.setKey(subStringPropertyValue);
                    textEditorForm.setMode("DataColumn");
                    textEditorForm.dataTree.autoscroll();
                }
                else if (textType == "SystemVariables" || (textEditorForm.propertyName != "condition" && jsObject.options.report && jsObject.IsContains(jsObject.options.report.dictionary.systemVariables, subStringPropertyValue))) {
                    textEditorForm.setMode("SystemVariable");
                    var selectedItem = systemVarsTree.mainItem.getChildByName(subStringPropertyValue);
                    if (selectedItem) {
                        selectedItem.setSelected();
                        systemVarsTree.autoscroll();
                    }
                }
                else if (textType == "Totals") {
                    textEditorForm.setMode("SummaryText");
                    summaryText.fill(propertyValue);
                }
                else {
                    textEditorForm.setMode(selectedObject.properties.allowHtmlTags ? "RichText" : "Expression");
                    expTextArea.focus();
                }
            }
            textEditorForm.container.style.visibility = "visible";
            textEditorForm.progress.hide();
            expTextArea.focus();
        }, 50);
    }

    textEditorForm.action = function () {
        textEditorForm.changeVisibleState(false);

        if (this.actionFunction) {
            this.actionFunction();
            this.actionFunction = null;
            return;
        }

        var result = textEditorForm.mode == "SummaryText" ? summaryText.controls.expressionTextArea.value : expTextArea.value;
        var textType = textEditorForm.mode == "SummaryText" ? "Totals" : "Expression";

        if (textEditorForm.mode == "DataColumn") {
            result = (textEditorForm.dataTree.key ? "{" + textEditorForm.dataTree.key + "}" : "");
            textType = "DataColumn";
        }
        else if (textEditorForm.mode == "SystemVariable") {
            result = systemVarsTree.selectedItem ? "{" + systemVarsTree.selectedItem.itemObject.name + "}" : "";
            textType = "SystemVariables";
        }
        else if (textEditorForm.mode == "RichText") {
            result = richTextEditor.getText(true);
        }

        var propertyNames = [textEditorForm.propertyName];
        var propertyValues = [StiBase64.encode(result)];

        if (textEditorForm.propertyName == "text") {
            propertyNames.push("textType");
            propertyValues.push(textType);
        }

        var currentObject = jsObject.options.selectedObject || jsObject.GetCommonObject(jsObject.options.selectedObjects);
        if (currentObject && currentObject.typeComponent == "StiText") {
            if (textEditorForm.mode == "RichText") {
                propertyNames.push("allowHtmlTags");
                propertyValues.push(true);
            }
            propertyNames.push("nullValue");
            propertyValues.push(nullValuesControl.key);
        }

        jsObject.ApplyPropertyValue(propertyNames, propertyValues);
    }

    return textEditorForm;
}