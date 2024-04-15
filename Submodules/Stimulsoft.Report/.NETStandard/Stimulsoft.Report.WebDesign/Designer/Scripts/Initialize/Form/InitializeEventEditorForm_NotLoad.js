
StiMobileDesigner.prototype.InitializeEventEditorForm_ = function () {
    var jsObject = this;
    var form = this.InitializeBaseExpressionEditorForm("eventEditor", this.loc.FormTitles.EventEditorForm, 3, this.HelpLinks["expression"]);

    var switchButton = this.FormButton(null, null, this.loc.Buttons.SwitchTo.replace("{0}", "Blockly"), null);
    switchButton.style.display = this.options.blocklyNotSupported ? "none" : "inline-block";
    switchButton.style.margin = "12px";

    switchButton.action = function () {
        var messageForm = jsObject.MessageFormForSwitchingEventMode();
        messageForm.changeVisibleState(true);

        messageForm.action = function (state) {
            if (state) {
                if (form.resultControl) {
                    var resultTextBox = form.resultControl.textBox || form.resultControl;
                    resultTextBox.value = resultTextBox.hiddenValue = "";

                    var showBlocklyForm = function () {
                        jsObject.InitializeBlocklyEditorForm(function (blocklyForm) {
                            blocklyForm.resultControl = form.resultControl;
                            blocklyForm.show("", form.resultControl.eventName);
                        });
                    }
                    if (!jsObject.options.blocklyInitialized && !jsObject.options.jsMode) {
                        jsObject.LoadScriptWithProcessImage(jsObject.options.scriptsUrl + "BlocklyScripts;" + (jsObject.options.cultureName || "en"), function () {
                            jsObject.options.blocklyInitialized = true;
                            showBlocklyForm();
                        });
                    }
                    else {
                        showBlocklyForm();
                    }
                }
                form.changeVisibleState(false);
            }
            else {
                messageForm.changeVisibleState(false);
            }
        }
    }

    var footerTable = this.CreateHTMLTable();
    footerTable.style.width = "100%";

    var buttonsPanel = form.buttonsPanel;
    form.removeChild(buttonsPanel);
    form.appendChild(footerTable);

    footerTable.addCell(switchButton).style.width = "1px";
    footerTable.addCell();
    footerTable.addCell(form.buttonOk).style.width = "1px";
    footerTable.addCell(form.buttonCancel).style.width = "1px";

    return form;
}