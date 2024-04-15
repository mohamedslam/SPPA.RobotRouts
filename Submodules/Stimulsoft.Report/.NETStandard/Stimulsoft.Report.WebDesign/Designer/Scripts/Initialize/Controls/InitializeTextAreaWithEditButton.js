
StiMobileDesigner.prototype.TextAreaWithEditButton = function (name, width, height, readOnly, addEditorForm) {
    var jsObject = this;
    var mainControl = this.CreateHTMLTable();

    var textArea = mainControl.textArea = this.TextArea(name, width, height);
    textArea.style.lineHeight = "1";
    mainControl.addCell(mainControl.textArea).style.lineHeight = "0";

    var button = mainControl.button = this.SmallButton(name + "Button", null, null, "EditButton.png", this.loc.QueryBuilder.Edit, null, "stiDesignerFormButton", true);
    mainControl.addCell(button).style.verticalAlign = "top";

    //Override
    button.imageCell.style.padding = "0";
    button.style.width = button.style.height = this.options.isTouchDevice ? "28px" : "24px";
    button.style.marginLeft = "4px";
    button.innerTable.style.width = "100%";
    button.textArea = mainControl.textArea;

    if (readOnly) {
        textArea.readOnly = readOnly;
        textArea.style.cursor = "default";
        textArea.mainControl = mainControl;

        textArea.onclick = function () {
            if (!this.isTouchEndFlag && !jsObject.options.isTouchClick)
                button.onclick();
        }

        textArea.ontouchend = function () {
            this.isTouchEndFlag = true;
            clearTimeout(this.isTouchEndTimer);
            button.ontouchend();
            this.isTouchEndTimer = setTimeout(function () {
                textArea.isTouchEndFlag = false;
            }, 1000);
        }
    }

    if (addEditorForm) {
        button.action = function () {
            jsObject.InitializeTextEditorFormOnlyText(function (form) {
                form.showFunction = function () {
                    this.textArea.value = textArea.value;
                }
                form.actionFunction = function () {
                    textArea.value = this.textArea.value;
                    mainControl.action();
                }
                form.changeVisibleState(true);
            });
        }
    }

    mainControl.setEnabled = function (state) {
        textArea.setEnabled(state);
        button.setEnabled(state);
    }

    textArea.action = function () {
        mainControl.action();
    }

    mainControl.action = function () { }

    return mainControl;
}

StiMobileDesigner.prototype.ExpressionTextArea = function (name, width, height, withBorder, cutBrackets) {
    var textAreaParent = document.createElement("div");
    var jsObject = textAreaParent.jsObject = this;
    if (name) this.options.controls[name] = textAreaParent;

    var textArea = this.TextArea(null, width, height);
    textAreaParent.appendChild(textArea);
    textAreaParent.textArea = textArea;
    textAreaParent.controlType == "ExpressionTextArea";

    textArea.style.borderStyle = withBorder ? "solid" : "dashed";
    textArea.style.overflowY = "hidden";
    textArea.addInsertButton();
    StiMobileDesigner.setImageSource(textArea.insertButton.image, this.options, "EditButton.png");
    textArea.insertButton.style.margin = "5px 0 0 " + (width - (this.options.isTouchDevice ? 31 : 25)) + "px";
    textArea.cutBrackets = cutBrackets;

    if (this.options.isTouchDevice && height && height < 35) {
        textArea.insertButton.style.height = textArea.insertButton.style.width = "23px";
        textArea.insertButton.imageCell.style.padding = "0 4px 0 4px";
        textArea.insertButton.style.margin = "5px 0 0 " + (width - 24) + "px";
    }

    textArea.insertButton.action = function () {
        jsObject.InitializeExpressionEditorForm(function (form) {
            var propertiesPanel = jsObject.options.propertiesPanel;
            form.propertiesPanelZIndex = propertiesPanel.style.zIndex;
            form.propertiesPanelIsEnabled = propertiesPanel.isEnabled;
            form.resultControl = textArea;
            form.changeVisibleState(true);
        });
    }

    textAreaParent.setValue = function (value) {
        textArea.value = value;
    }

    textAreaParent.getValue = function () {
        return textArea.value;
    }

    textAreaParent.setEnabled = function (state) {
        textArea.setEnabled(state);
        textArea.insertButton.setEnabled(state);
    }

    textAreaParent.action = function () { }

    textArea.action = function () {
        textAreaParent.action();
    }

    return textAreaParent;
}