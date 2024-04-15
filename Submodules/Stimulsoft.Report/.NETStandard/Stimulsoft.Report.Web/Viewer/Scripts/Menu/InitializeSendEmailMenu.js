
StiJsViewer.prototype.InitializeSendEmailMenu = function () {
    var sendEmailMenu = this.InitializeBaseSaveMenu("sendEmailMenu", this.controls.toolbar.controls["SendEmail"]);
    var jsObject = this;

    sendEmailMenu.action = function (menuItem) {
        this.changeVisibleState(false);

        if (!jsObject.checkCloudAuthorization("export")) return;

        if (jsObject.options.email.showExportDialog)
            jsObject.controls.forms.exportForm.show(menuItem.key, jsObject.options.actions.emailReport);
        else if (jsObject.options.email.showEmailDialog) {
            jsObject.controls.forms.sendEmailForm.show(menuItem.key, jsObject.getDefaultExportSettings(menuItem.key));
        }
        else {
            var exportSettings = jsObject.getDefaultExportSettings(menuItem.key);
            exportSettings["Email"] = jsObject.options.email.defaultEmailAddress;
            exportSettings["Message"] = jsObject.options.email.defaultEmailMessage;
            exportSettings["Subject"] = jsObject.options.email.defaultEmailSubject;
            jsObject.postEmail(menuItem.key, exportSettings);
        }
    }
}