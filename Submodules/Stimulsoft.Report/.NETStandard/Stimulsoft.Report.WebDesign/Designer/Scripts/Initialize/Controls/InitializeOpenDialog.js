
StiMobileDesigner.prototype.InitializeOpenDialog = function (nameDialog, actionFunction, fileMask) {
    if (this.options.openDialogs[nameDialog]) {
        this.options.mainPanel.removeChild(this.options.openDialogs[nameDialog]);
    }
    var inputFile = document.createElement("input");
    this.options.mainPanel.appendChild(inputFile);
    this.options.openDialogs[nameDialog] = inputFile;
    inputFile.style.display = "none";
    inputFile.id = nameDialog;
    inputFile.jsObject = this;
    inputFile.setAttribute("type", "file");
    inputFile.setAttribute("name", "files[]");
    inputFile.setAttribute("multiple", "");
    if (fileMask) inputFile.setAttribute("accept", fileMask);
    this.addEvent(inputFile, 'change', actionFunction);

    inputFile.action = function () {
        this.style.display = "";
        this.focus();
        this.click();
        this.style.display = "none";
    }

    return inputFile;
}

StiMobileDesigner.prototype.ResetOpenDialogs = function () {
    for (var name in this.options.openDialogs) {
        var openDialog = this.options.openDialogs[name];
        if (openDialog) {
            openDialog.setAttribute("name", "files[]");
            openDialog.setAttribute("multiple", "");
            openDialog.setAttribute("value", "");
        }
    }
}

//Open Report
StiMobileDesigner.prototype.StiHandleOpenReport = function (evt) {
    var jsObject = this.jsObject;
    var files = evt.target.files;
    if (files && files.length > 0) {
        var file = files[0];
        var fileName = file.name || "Report";
        var filePath = evt.target.value;
        var fileSize = file.size;

        var reader = new FileReader();
        reader.onload = (function () {
            return function (e) {
                if (jsObject.options.cloudMode) {
                    var maxFileSize = jsObject.GetCurrentPlanLimitValue("MaxFileSize");
                    if (maxFileSize && fileSize > maxFileSize) {
                        jsObject.InitializeNotificationForm(function (form) {
                            form.show(
                                jsObject.loc.Notices.QuotaMaximumFileSizeExceeded + "<br>" + jsObject.loc.PropertyMain.Maximum + ": " + jsObject.GetHumanFileSize(maxFileSize, true),
                                jsObject.NotificationMessages("upgradeYourPlan"),
                                "Notifications.Elements.png"
                            );
                        });
                        return;
                    }
                }
                jsObject.ResetOpenDialogs();
                jsObject.OpenReport(fileName, jsObject.options.mvcMode ? encodeURIComponent(e.target.result) : e.target.result, filePath, fileSize, true);
                jsObject.ReturnFocusToDesigner();
            };
        })(file);
        reader.readAsDataURL(file);
    }
}

StiMobileDesigner.prototype.OpenReport = function (fileName, fileContent, filePath, fileSize, addToRecent) {
    if (this.EndsWith(fileName.toString().toLowerCase(), ".mrx")) {
        var passwordForm = this.options.forms.passwordForm || this.InitializePasswordForm();
        passwordForm.show(function (password) {
            this.jsObject.SendCommandOpenReport(fileContent, fileName, { password: password }, filePath, fileSize, addToRecent);
        }, this.loc.Password.lbPasswordLoad);
    }
    else {
        this.SendCommandOpenReport(fileContent, fileName, { isPacked: this.EndsWith(fileName.toString().toLowerCase(), ".mrz") }, filePath, fileSize, addToRecent);
    }
}

//Open Style
StiMobileDesigner.prototype.StiHandleOpenStyle = function (evt) {
    var files = evt.target.files;
    var fileName = files[0] ? files[0].name : "Styles";

    for (var i = 0; i < files.length; i++) {
        var f = files[i];
        var reader = new FileReader();
        reader.jsObject = this.jsObject;

        reader.onload = (function (theFile) {
            return function (e) {
                reader.jsObject.ResetOpenDialogs();
                reader.jsObject.SendCommandOpenStyle(reader.jsObject.options.mvcMode ? encodeURIComponent(e.target.result) : e.target.result, fileName);
                reader.jsObject.ReturnFocusToDesigner();
            };
        })(f);

        reader.readAsDataURL(f);
    }
}

//Open Page
StiMobileDesigner.prototype.StiHandleOpenPage = function (evt) {
    var files = evt.target.files;
    var fileName = files[0] ? files[0].name : "Page";

    for (var i = 0; i < files.length; i++) {
        var f = files[i];
        var reader = new FileReader();
        reader.jsObject = this.jsObject;

        reader.onload = (function (theFile) {
            return function (e) {
                reader.jsObject.ResetOpenDialogs();
                reader.jsObject.SendCommandOpenPage(reader.jsObject.options.mvcMode ? encodeURIComponent(e.target.result) : e.target.result, fileName);
                reader.jsObject.ReturnFocusToDesigner();
            };
        })(f);

        reader.readAsDataURL(f);
    }
}

//Open Dictionary
StiMobileDesigner.prototype.StiHandleOpenDictionary = function (evt) {
    var files = evt.target.files;
    var fileName = files[0] ? files[0].name : "Dictionary";

    for (var i = 0; i < files.length; i++) {
        var f = files[i];
        var reader = new FileReader();
        reader.jsObject = this.jsObject;

        reader.onload = (function (theFile) {
            return function (e) {
                reader.jsObject.ResetOpenDialogs();
                reader.jsObject.SendCommandOpenDictionary(reader.jsObject.options.mvcMode ? encodeURIComponent(e.target.result) : e.target.result, fileName);
                reader.jsObject.ReturnFocusToDesigner();
            };
        })(f);

        reader.readAsDataURL(f);
    }
}

//Merge Dictionary
StiMobileDesigner.prototype.StiHandleMergeDictionary = function (evt) {
    var files = evt.target.files;
    var fileName = files[0] ? files[0].name : "Dictionary";

    for (var i = 0; i < files.length; i++) {
        var f = files[i];
        var reader = new FileReader();
        reader.jsObject = this.jsObject;

        reader.onload = (function (theFile) {
            return function (e) {
                reader.jsObject.ResetOpenDialogs();
                reader.jsObject.SendCommandMergeDictionary(reader.jsObject.options.mvcMode ? encodeURIComponent(e.target.result) : e.target.result, fileName);
                reader.jsObject.ReturnFocusToDesigner();
            };
        })(f);

        reader.readAsDataURL(f);
    }
}

StiMobileDesigner.prototype.GetDecodedFileContent = function (fileContent) {
    if (fileContent) {
        if (fileContent.indexOf("base64,") >= 0)
            return StiBase64.decode(fileContent.substr(fileContent.indexOf("base64,") + 7));
        else
            return fileContent;

    }
    return "";
}