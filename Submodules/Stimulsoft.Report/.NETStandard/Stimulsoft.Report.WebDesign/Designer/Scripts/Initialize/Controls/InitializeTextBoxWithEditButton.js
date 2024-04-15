
StiMobileDesigner.prototype.TextBoxWithEditButton = function (name, width, height, readOnly, showClearButton, showSimpleEditor) {
    var jsObject = this;
    var control = this.CreateHTMLTable();
    control.isEnabled = true;
    control.style.borderCollapse = "separate";

    var textHeight = height || this.options.controlsHeight;
    var buttonWidth = this.options.controlsButtonsWidth;
    var buttonHeight = textHeight - 4;

    var textBoxWidth = width - (showClearButton ? ((buttonWidth + 2) * 2) : (buttonWidth + 2));
    var textBox = control.textBox = this.TextBox(name, textBoxWidth, textHeight);
    textBox.mainControl = control;    
    textBox.style.borderRight = "0";
    control.addCell(textBox).style.fontSize = "0px";

    if (jsObject.allowRoundedControls()) {
        textBox.style.borderRadius = "3px 0 0 3px";
    }

    var button = control.button = this.StandartSmallButton(name + "Button", null, null, "EditButton.png", this.loc.QueryBuilder.Edit, null, null);
    button.imageCell.style.padding = "0";
    button.textBox = textBox;

    if (showSimpleEditor) {
        button.action = function () {
            jsObject.InitializeTextEditorFormOnlyText(function (form) {
                form.controlTextBox = textBox;

                form.showFunction = function () {
                    this.textArea.value = textBox.value;
                }

                form.actionFunction = function () {
                    textBox.value = this.textArea.value;
                    control.action();
                }

                form.changeVisibleState(true);
            });
        }
    }

    //Override
    button.innerTable.style.width = "100%";
    button.style.width = buttonWidth + "px";
    button.style.height = buttonHeight + "px";

    var baseCellClass = "stiDesignerTextBoxEditButton stiDesignerTextBoxEditButton";
    var buttonCell = control.buttonCell = control.addCell(button);    
    buttonCell.className = baseCellClass + "Default";

    if (jsObject.allowRoundedControls()) {
        buttonCell.style.borderRadius = button.style.borderRadius = "0 3px 3px 0";
    }

    if (readOnly) {
        textBox.readOnly = readOnly;
        textBox.style.cursor = "default";
        textBox.onclick = function () { if (!jsObject.options.isTouchDevice) this.mainControl.button.onclick(); }
        textBox.ontouchend = function () { this.mainControl.button.ontouchend(); }
    }

    if (showClearButton) {
        var clearButton = control.clearButton = this.StandartSmallButton(name + "ClearButton", null, null, "SmallCross.png", this.loc.Gui.monthcalendar_clearbutton, null, null, { width: 8, height: 8 });
        clearButton.innerTable.style.width = "100%";
        clearButton.style.width = buttonWidth + "px";
        clearButton.style.height = buttonHeight + "px";

        var clearButtonCell = control.clearButtonCell = control.addCell(clearButton);
        clearButtonCell.className = baseCellClass + "Default";

        if (jsObject.allowRoundedControls()) {
            clearButtonCell.style.borderRadius = clearButton.style.borderRadius = "0 3px 3px 0";
        }

        clearButton.action = function () {
            textBox.value = "";
            control.action();
        }

        button.style.borderRadius = buttonCell.style.borderRadius = "0";
        buttonCell.style.borderLeft = buttonCell.style.borderRight = "0";
    }

    control.onmouseover = function () {
        if (!jsObject.options.isTouchDevice) this.onmouseenter();
    }

    control.onmouseenter = function () {
        if (!this.isEnabled || jsObject.options.isTouchClick) return;
        this.isOver = true;
        textBox.onmouseenter();
        buttonCell.className = baseCellClass + "Over";
        if (control.clearButtonCell) control.clearButtonCell.className = baseCellClass + "Over";
    }

    control.onmouseleave = function () {
        if (!this.isEnabled) return;
        this.isOver = false;
        textBox.onmouseleave();
        buttonCell.className = baseCellClass + "Default";
        if (control.clearButtonCell) control.clearButtonCell.className = baseCellClass + "Default";
    }

    control.action = function () { };

    textBox.action = function () { control.action(); }

    control.setEnabled = function (state) {
        this.isEnabled = state;
        this.textBox.setEnabled(state);
        this.button.setEnabled(state);
        buttonCell.className = baseCellClass + (state ? "Default" : "Disabled");

        if (showClearButton) {
            this.clearButton.setEnabled(state);
            this.clearButtonCell.className = baseCellClass + (state ? "Default" : "Disabled");
        }
    }

    return control;
}

//Expression Control
StiMobileDesigner.prototype.ExpressionControl = function (name, width, height, readOnly, cutBrackets, showClearButton, notShowDictionary) {
    var jsObject = this;
    var expControl = this.TextBoxWithEditButton(name, width, height, readOnly, showClearButton);
    expControl.cutBrackets = cutBrackets;
    expControl.notShowDictionary = notShowDictionary;

    if (readOnly) {
        expControl.textBox.readOnly = readOnly;
        expControl.textBox.style.cursor = "default";
        expControl.textBox.mainControl = expControl;

        expControl.textBox.onclick = function () {
            if (!this.isTouchEndFlag)
                this.mainControl.button.onclick();
        }

        expControl.textBox.ontouchend = function () {
            var this_ = this;
            this.isTouchEndFlag = true;
            clearTimeout(this.isTouchEndTimer);
            this.mainControl.button.ontouchend();
            this.isTouchEndTimer = setTimeout(function () {
                this_.isTouchEndFlag = false;
            }, 1000);
        }
    }

    expControl.button.action = function () {
        if (name == "expandExpression") {
            jsObject.InitializeHyperlinkEditorForm(function (form) {
                expControl.interactionIdent = "expandExpression";
                form.show(expControl);

                form.action = function () {
                    form.changeVisibleState(false);
                    expControl.textBox.value = form.textArea.value;
                    expControl.action();
                }
            });

        } else {
            jsObject.InitializeExpressionEditorForm(function (form) {
                var propertiesPanel = jsObject.options.propertiesPanel;
                form.propertiesPanelZIndex = propertiesPanel.style.zIndex;
                form.propertiesPanelIsEnabled = propertiesPanel.isEnabled;
                form.resultControl = expControl;

                form.changeVisibleState(true);
            });
        }
    }

    return expControl;
}

StiMobileDesigner.prototype.ExpressionControlWithMenu = function (name, width, toolTip, items, readOnly, checkExpressionBrackets) {
    var jsObject = this;
    var dropDownList = this.DropDownList(name, width, toolTip, items, readOnly, false, null, false, null, true);
    dropDownList.cutBrackets = true;
    dropDownList.checkExpressionBrackets = true;

    dropDownList.button.action = function () {
        dropDownList.menu.currentContainer = null;
        dropDownList.hideError();
        if (!this.dropDownList.menu.visible) {
            if (this.dropDownList.menu.isDinamic) this.dropDownList.menu.addItems(this.dropDownList.items);
            if (jsObject.options.showTooltips && this.dropDownList.toolTip && typeof (this.dropDownList.toolTip) == "object") jsObject.options.toolTip.hide();
            this.dropDownList.menu.changeVisibleState(true);
        }
        else {
            this.dropDownList.menu.changeVisibleState(false);
        }
    }

    dropDownList.editButton.action = function () {
        jsObject.InitializeExpressionEditorForm(function (form) {
            var propertiesPanel = jsObject.options.propertiesPanel;
            form.propertiesPanelZIndex = propertiesPanel.style.zIndex;
            form.propertiesPanelIsEnabled = propertiesPanel.isEnabled;
            form.resultControl = dropDownList.textBox;
            form.resultControl.checkExpressionBrackets = checkExpressionBrackets;
            form.changeVisibleState(true);
        });
    }

    dropDownList.textBox.onmouseup = function () {
        if (jsObject.options.itemInDrag && jsObject.options.dictionaryTree.selectedItem) {
            dropDownList.textBox.value = jsObject.CheckExpressionBrackets(jsObject.options.dictionaryTree.selectedItem.getResultForEditForm(true, true));
            dropDownList.textBox.action();
        }
    }

    return dropDownList;
}

//RichTextBox Control
StiMobileDesigner.prototype.RichTextBoxControl = function (name, width, height, expressionItems) {
    var jsObject = this;
    var control = this.TextBoxWithEditButton(name, width, height, true, true);

    control.button.action = function () {
        jsObject.InitializeRichTextEditorForm(function (form) {
            form.show(control.key, control);

            form.action = function () {
                form.changeVisibleState(false);
                control.setKey(form.richTextEditor.getText());
                control.action();
            }
        });
    }

    control.clearButton.action = function () {
        control.setKey("");
        control.action();
    }

    control.setKey = function (key) {
        this.key = key;
        this.textBox.value = "[" + (!key ? jsObject.loc.FormStyleDesigner.NotSpecified : jsObject.loc.FormFormatEditor.Custom) + "]";
    }

    return control;
}

//HyperlinkTextBox Control
StiMobileDesigner.prototype.HyperlinkTextBoxControl = function (name, width, height, readOnly, hyperlinkSamplePattern) {
    var jsObject = this;
    var control = this.TextBoxWithEditButton(name, width, height, readOnly, true);

    if (hyperlinkSamplePattern) {
        control.textBox.setAttribute("placeholder", hyperlinkSamplePattern);
    }

    control.button.action = function () {
        control.preOpening();

        jsObject.InitializeHyperlinkEditorForm(function (form) {
            form.show(control);

            form.action = function () {
                form.changeVisibleState(false);
                control.textBox.value = form.textArea.value;
                control.action();
            }
        });
    }

    control.preOpening = function () { };

    return control;
}

//HyperlinkTextBox Control
StiMobileDesigner.prototype.HyperlinkControlWithMenu = function (name, width, items, readOnly, hyperlinkSamplePattern) {
    var jsObject = this;

    var control = this.DropDownList(name, width, null, items, readOnly, false, null, false, null, true);
    control.cutBrackets = true;

    if (hyperlinkSamplePattern) {
        control.textBox.setAttribute("placeholder", hyperlinkSamplePattern);
    }

    control.menu.action = function (menuItem) {
        this.changeVisibleState(false);
        control.key = menuItem.key;
        control.textBox.value = "{" + menuItem.key + "}";
        control.action();
    }

    control.menu.onshow = function () { }

    control.editButton.action = function () {
        control.preOpening();

        jsObject.InitializeHyperlinkEditorForm(function (form) {
            form.show(control);

            form.action = function () {
                form.changeVisibleState(false);
                control.textBox.value = form.textArea.value;
                control.action();
            }
        });
    }

    control.preOpening = function () { };

    return control;
}

