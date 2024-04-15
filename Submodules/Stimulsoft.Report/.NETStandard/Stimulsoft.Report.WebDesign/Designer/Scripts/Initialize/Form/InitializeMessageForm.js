
StiMobileDesigner.prototype.InitializeMessageForm = function () {
    var form = this.BaseForm("messageForm", " ", 4);
    form.messageText = "";
    form.messageImage = null;
    form.caption.style.textAlign = "center";
    form.container.style.fontSize = "13px";
    form.container.style.fontFamily = "Arial";
    form.container.style.padding = "20px 50px 20px 30px";
    form.container.style.lineHeight = "1.5";
    form.style.maxWidth = "600px";
    form.style.minWidth = "500px";

    form.onshow = function () {
        if (this.messageImage) {
            var innerTable = this.jsObject.CreateHTMLTable();
            var img = document.createElement("img");
            StiMobileDesigner.setImageSource(img, this.jsObject.options, this.messageImage);
            innerTable.addCell(img).style.padding = "0 20px 0 0";
            innerTable.addTextCell(this.messageText);
            this.container.innerHTML = "";
            this.container.appendChild(innerTable);
        }
        else {
            this.container.innerHTML = this.messageText;
        }
    }

    //Override 
    while (form.buttonsPanel.childNodes[0]) {
        form.buttonsPanel.removeChild(form.buttonsPanel.childNodes[0]);
    }

    var buttonsTable = this.CreateHTMLTable();
    form.buttonsPanel.appendChild(buttonsTable);

    //Yes
    form.buttonYes = this.FormButton(form, name + "ButtonYes", this.loc.FormFormatEditor.nameYes, null, null, null, null, "stiDesignerFormButtonTheme");
    form.buttonYes.action = function () {
        form.changeVisibleState(false);
        form.action(true);
    };
    buttonsTable.addCell(form.buttonYes).style.padding = "12px 0px 12px 12px";

    //No
    form.buttonNo = this.FormButton(form, name + "ButtonNo", this.loc.FormFormatEditor.nameNo, null);
    form.buttonNo.action = function () {
        form.changeVisibleState(false);
        form.action(false);
    };
    buttonsTable.addCell(form.buttonNo).style.padding = "12px";

    //Cancel
    form.buttonCancel = this.FormButton(form, name + "ButtonCancel", this.loc.Buttons.Cancel.replace("&", ""), null);
    form.buttonCancel.style.margin = "12px 12px 12px 0px";
    form.buttonCancel.action = function () {
        form.changeVisibleState(false);
    };
    buttonsTable.addCell(form.buttonCancel);

    return form;
}

StiMobileDesigner.prototype.MessageFormForSave = function () {
    var form = this.InitializeMessageForm();

    var fileName = this.options.formsDesignerMode && this.options.formsDesignerFrame
        ? (this.options.formsDesignerFrame.formName || "Form")
        : (this.options.cloudParameters && this.options.cloudParameters.reportTemplateItemKey && this.options.cloudParameters.reportName
            ? this.options.cloudParameters.reportName
            : (this.options.report ? (this.options.report.properties.reportFile || StiBase64.decode(this.options.report.properties.reportName.replace("Base64Code;", ""))) : "Report"));

    form.messageText = this.loc.Questions.qnSaveChanges.replace("{0}", fileName);
    form.caption.innerHTML = this.loc.FormDesigner.title.toUpperCase();
    form.buttonCancel.style.display = "";

    return form;
}

StiMobileDesigner.prototype.MessageFormForDelete = function () {
    var form = this.InitializeMessageForm();

    form.messageText = this.loc.Questions.qnRemove;
    form.caption.innerHTML = this.loc.FormDesigner.title.toUpperCase();
    form.buttonCancel.style.display = "none";

    return form;
}

StiMobileDesigner.prototype.MessageFormForReplaceItem = function (itemName) {
    var form = this.InitializeMessageForm();

    form.messageText = this.loc.Questions.qnReplace.replace("{0}", "\"" + itemName + "\"");
    form.caption.innerHTML = this.loc.FormDesigner.title.toUpperCase();
    form.buttonCancel.style.display = "";

    return form;
}

StiMobileDesigner.prototype.MessageFormForApplyStyles = function () {
    var form = this.InitializeMessageForm();

    form.messageText = this.loc.FormStyleDesigner.qnApplyStyleCollection;
    form.caption.innerHTML = this.loc.FormDesigner.title.toUpperCase();
    form.buttonCancel.style.display = "none";

    return form;
}

StiMobileDesigner.prototype.MessageFormForRenderReportInCompilationMode = function () {
    var form = this.InitializeMessageForm();

    form.messageImage = "ReportChecker.Warning32.png";
    form.messageText = "Stimulsoft Cloud cannot view this report in the compilation mode. Change the report to the interpretation mode.";
    form.caption.innerHTML = this.loc.FormDesigner.title.toUpperCase();
    form.buttonYes.caption.innerHTML = this.loc.Buttons.Ok.replace("&", "");
    form.buttonNo.style.display = "none";
    form.buttonCancel.style.display = "none";

    return form;
}

StiMobileDesigner.prototype.MessageFormForSaveReportToCloud = function () {
    var form = this.InitializeMessageForm();

    form.messageImage = "ReportChecker.Information32.png";
    form.messageText = this.loc.Messages.ShareYourReportYouShouldSave;
    form.caption.innerHTML = this.loc.FormDesigner.title.toUpperCase();
    form.buttonYes.caption.innerHTML = this.loc.Buttons.Save;
    form.buttonNo.caption.innerHTML = this.loc.Buttons.Cancel.replace("&", "");
    form.buttonCancel.style.display = "none";

    return form;
}

StiMobileDesigner.prototype.MessageFormForRemoveMobileSurface = function () {
    var form = this.InitializeMessageForm();

    form.messageText = this.loc.Questions.qnRemove;
    form.caption.innerHTML = this.loc.FormDesigner.title.toUpperCase();
    form.buttonCancel.style.display = "none";

    return form;
}

StiMobileDesigner.prototype.InitializeErrorMessageForm = function () {
    var form = this.BaseForm("errorMessageForm", this.loc.Errors.Error, 4);
    form.container.style.borderTop = "0px";
    form.buttonCancel.style.display = "none";
    form.caption.style.textAlign = "center";
    form.container.style.fontSize = "14px";
    form.container.style.fontFamily = "Arial";

    var table = this.CreateHTMLTable();
    form.container.appendChild(table);

    form.image = document.createElement("img");
    form.image.style.width = form.image.style.height = "32px";
    form.image.style.padding = "15px";
    StiMobileDesigner.setImageSource(form.image, this.options, "ReportChecker.Error32.png");
    table.addCellInLastRow(form.image);

    form.description = table.addCellInLastRow();
    form.description.className = "stiDesignerMessagesFormDescription";

    form.show = function (messageText, messageType) {
        this.messageText = messageText;
        this.messageType = messageType;

        if (messageText.indexOf("Timeout response from the server") >= 0) {
            console.log(messageText);
            return;
        }
        if (this.visible && this.messageText == messageText && this.messageType == messageType) {
            return;
        }
        if (this.jsObject.options.ignoreAllErrors) return;
        if (this.visible && this.jsObject.options.jsMode) {
            this.description.innerHTML += "<br/><br/>" + messageText;
            this.jsObject.SetObjectToCenter(this);
            return;
        }
        if (this.jsObject.options.forms.errorMessageForm) { //Fixed Bug
            this.jsObject.options.mainPanel.removeChild(this.jsObject.options.forms.errorMessageForm);
            this.jsObject.options.mainPanel.appendChild(this.jsObject.options.forms.errorMessageForm);
        }

        this.caption.innerHTML = this.jsObject.loc.FormDesigner.title;

        if (messageType == "Warning")
            StiMobileDesigner.setImageSource(this.image, this.jsObject.options, "ReportChecker.Warning32.png");
        else if (messageType == true || messageType == "Info")
            StiMobileDesigner.setImageSource(this.image, this.jsObject.options, "ReportChecker.Information32.png"); //messageType === true - for backward compatibility
        else {
            StiMobileDesigner.setImageSource(this.image, this.jsObject.options, "ReportChecker.Error32.png");
            this.caption.innerHTML = this.jsObject.loc.Errors.Error;
        }

        this.changeVisibleState(true);
        this.description.innerHTML = messageText;
        var processImage = this.jsObject.options.processImage || this.jsObject.InitializeProcessImage();
        processImage.hide();
    }

    form.action = function () {
        this.changeVisibleState(false);
        if (this.onAction) {
            this.onAction();
            this.onAction = null;
        }
    }

    return form;
}

StiMobileDesigner.prototype.InitializeMessageFormForChangeRequestTimeout = function () {
    var form = this.BaseForm("formChangeRequestTimeout", this.loc.FormDesigner.title.toUpperCase(), 4);
    form.caption.style.textAlign = "center";
    form.container.style.fontSize = "13px";
    form.container.style.fontFamily = "Arial";
    form.container.style.padding = "20px 50px 20px 30px";
    form.container.style.lineHeight = "1.6";
    form.style.maxWidth = "600px";
    form.style.minWidth = "500px";
    form.buttonCancel.style.display = "none";

    var innerTable = this.CreateHTMLTable();
    var img = document.createElement("img");
    img.style.width = img.style.height = "32px";
    StiMobileDesigner.setImageSource(img, this.options, "ReportChecker.Information32.png");
    innerTable.addCell(img).style.padding = "0 20px 0 0";
    var textCell = innerTable.addTextCell(this.loc.Messages.ChangeRequestTimeout);
    form.container.appendChild(innerTable);

    form.show = function (timeoutValue, sqlTimeoutValue) {
        textCell.innerHTML = this.jsObject.loc.Messages.ChangeRequestTimeout.replace("{0}", sqlTimeoutValue);
        form.changeVisibleState(true);
    }

    return form;
}

StiMobileDesigner.prototype.MessageFormForDeleteUsedResource = function () {
    var form = this.InitializeMessageForm();
    form.onshow = function () { };
    form.caption.innerHTML = this.loc.FormDesigner.title.toUpperCase();
    form.buttonYes.caption.innerHTML = this.loc.Buttons.ForceDelete;
    form.buttonNo.caption.innerHTML = this.loc.Buttons.Cancel.replace("&", "");
    form.buttonCancel.style.display = "none";
    form.container.innerHTML = "";
    form.container.style.textAlign = "right";
    form.container.style.padding = "0";
    form.container.style.width = "500px";

    var innerTable = this.CreateHTMLTable();
    var img = document.createElement("img");
    img.style.width = img.style.height = "32px";
    StiMobileDesigner.setImageSource(img, this.options, "ReportChecker.Error32.png");
    innerTable.addCell(img).style.padding = "20px";
    var textCell = innerTable.addTextCell();
    textCell.style.paddingRight = "20px";
    textCell.style.textAlign = "left";
    form.container.appendChild(innerTable);

    var whereUsedButton = this.SmallButton(null, null, this.loc.Cloud.ButtonWhereUsed, null, null, null, "stiDesignerHyperlinkButton");
    whereUsedButton.style.display = "inline-block";
    whereUsedButton.style.marginRight = "5px";
    form.container.appendChild(whereUsedButton);

    var sep = this.FormSeparator();
    sep.style.display = "none";
    form.container.appendChild(sep);

    var itemsContainer = this.EasyContainer(490);
    itemsContainer.style.padding = "0 10px 10px 0";
    itemsContainer.style.textAlign = "left";
    itemsContainer.style.display = "none";
    form.container.appendChild(itemsContainer);

    whereUsedButton.action = function () {
        itemsContainer.style.display = sep.style.display = sep.style.display == "none" ? "" : "none";
    }

    form.show = function (resourceName, usedObjects) {
        itemsContainer.clear();
        for (var i = 0; i < usedObjects.length; i++) {
            var text = (!usedObjects[i].alias || usedObjects[i].name == usedObjects[i].alias) ? usedObjects[i].name : usedObjects[i].name + " [" + usedObjects[i].alias + "]";
            var imageName = (usedObjects[i].typeItem == "Component" ? usedObjects[i].type : "Connections.BigDataSource") + ".png"
            var item = this.jsObject.OneItemContainerItem({ name: text }, null, imageName);
            itemsContainer.appendChild(item);
        }
        textCell.innerHTML = this.jsObject.loc.Messages.ResourceCannotBeDeleted.replace("{0}", resourceName);
        this.changeVisibleState(true);
    }

    return form;
}

StiMobileDesigner.prototype.MessageFormEmbedsAllData = function () {
    var form = this.InitializeMessageForm();

    form.messageImage = "ReportChecker.Warning32.png";
    form.messageText = this.loc.Messages.ThisFunctionEmbedsAllReportDataToTheReport.replace("{0}", "<br>");
    form.caption.innerHTML = this.loc.FormDesigner.title.toUpperCase();
    form.buttonCancel.style.display = "none";

    return form;
}

StiMobileDesigner.prototype.MessageFormForSwitchingEventMode = function () {
    var form = this.InitializeMessageForm();

    form.messageImage = "ReportChecker.Warning32.png";
    form.messageText = this.loc.Messages.SwitchingBetweenModes;
    form.caption.innerHTML = this.loc.FormDesigner.title.toUpperCase();
    form.buttonCancel.style.display = "none";

    return form;
}

StiMobileDesigner.prototype.MessageFormNewDictionary = function () {
    var form = this.InitializeMessageForm();

    form.messageText = this.loc.Questions.qnDictionaryNew;
    form.caption.innerHTML = this.loc.FormDictionaryDesigner.title.toUpperCase();
    form.buttonCancel.style.display = "none";

    return form;
}

StiMobileDesigner.prototype.MessageFormRestoreDefaults = function () {
    var form = this.InitializeMessageForm();
    form.style.minWidth = "350px";

    form.messageText = this.loc.Questions.qnRestoreDefault;
    form.caption.innerHTML = this.loc.PropertyMain.Options;
    form.buttonCancel.style.display = "none";

    return form;
}