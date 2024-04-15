
StiMobileDesigner.prototype.InitializeSaveDescriptionForm_ = function () {
    var saveDescriptionForm = this.BaseForm("saveDescriptionForm", this.loc.Services.categoryDesigner, 3);

    var requestChangesCheckBox = this.CheckBox(null, this.loc.Cloud.RequestChangesWhenSavingToCloud);
    requestChangesCheckBox.style.marginLeft = "12px";

    requestChangesCheckBox.action = function () {
        this.jsObject.options.requestChangesWhenSaving = this.isChecked;
        this.jsObject.SetCookie("StimulsoftMobileDesignerRequestChangesWhenSaving", this.isChecked ? "true" : "false");
    }

    var footerTable = this.CreateHTMLTable();
    footerTable.style.width = "100%";
    var buttonsPanel = saveDescriptionForm.buttonsPanel;
    saveDescriptionForm.removeChild(buttonsPanel);
    saveDescriptionForm.appendChild(footerTable);
    footerTable.addCell(requestChangesCheckBox).style.width = "1px";
    footerTable.addCell();
    footerTable.addCell(saveDescriptionForm.buttonOk).style.width = "1px";
    footerTable.addCell(saveDescriptionForm.buttonCancel).style.width = "1px";

    saveDescriptionForm.blockHeader = this.FormBlockHeader(this.loc.Cloud.TextDescriptionChanges.replace(":", ""));
    saveDescriptionForm.container.appendChild(saveDescriptionForm.blockHeader);

    var textArea = this.TextArea("saveDescriptionFormTextArea", 530, 150);
    textArea.style.margin = "12px";
    saveDescriptionForm.container.appendChild(textArea);
    saveDescriptionForm.textArea = textArea;

    textArea.onkeydown = function (e) {
        if (((e.keyCode == 13) || (e.keyCode == 10)) && (e.ctrlKey == false)) {
            saveDescriptionForm.action();
            return false;
        }
        if (((e.keyCode == 13) || (e.keyCode == 10)) && (e.ctrlKey == true)) {
            textArea.value += "\r\n";
            textArea.setSelRange(textArea, textArea.value.length, textArea.value.length);
            return false;
        }
    }

    //This method fixed IExpoler bug
    textArea.setSelRange = function (inputEl, selStart, selEnd) {
        if (inputEl.setSelectionRange) {
            inputEl.focus();
            inputEl.setSelectionRange(selStart, selEnd);
        } else if (inputEl.createTextRange) {
            var range = inputEl.createTextRange();
            range.collapse(true);
            range.moveEnd('character', selEnd);
            range.moveStart('character', selStart);
            range.select();
        }
    }

    saveDescriptionForm.onshow = function () {
        textArea.value = "";
        textArea.focus();
        requestChangesCheckBox.setChecked(this.jsObject.options.requestChangesWhenSaving);
    }

    saveDescriptionForm.buttonClose.action = function () { saveDescriptionForm.action(true) };
    saveDescriptionForm.buttonCancel.action = function () { saveDescriptionForm.action(true) };

    saveDescriptionForm.action = function (ignoreCustomMessage) {
        this.changeVisibleState(false);
        if (this.jsObject.options.cloudParameters) {
            this.jsObject.SendCommandItemResourceSave(this.jsObject.options.cloudParameters.reportTemplateItemKey, !ignoreCustomMessage ? StiBase64.encode(textArea.value) : null);
        }
        if (this.nextFunc) this.nextFunc();
    }

    return saveDescriptionForm;
}