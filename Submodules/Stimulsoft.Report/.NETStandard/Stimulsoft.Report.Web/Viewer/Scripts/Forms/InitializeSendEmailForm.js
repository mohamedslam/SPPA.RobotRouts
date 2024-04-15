
StiJsViewer.prototype.InitializeSendEmailForm = function (form) {
    var sendEmailForm = this.BaseForm("sendEmailForm", this.collections.loc["EmailOptions"], 1);
    sendEmailForm.style.fontFamily = this.options.toolbar.fontFamily;
    if (this.options.toolbar.fontColor != "") sendEmailForm.style.color = this.options.toolbar.fontColor;
    sendEmailForm.style.fontSize = "12px";
    sendEmailForm.controls = {};

    var controlsWidth = this.options.isMobileDevice ? 200 : 280;
    var controlProps = [
        ["Email", this.collections.loc["Email"], this.TextBox("sendEmailFormEmail", controlsWidth)],
        ["EmailCc", "Cc", this.TextBox("sendEmailFormEmailCc", controlsWidth)],
        ["EmailBcc", "Bcc", this.TextBox("sendEmailFormEmailBcc", controlsWidth)],
        ["Subject", this.collections.loc["Subject"], this.TextBox("sendEmailFormSubject", controlsWidth)],
        ["Message", this.collections.loc["Message"], this.TextArea("sendEmailFormMessage", controlsWidth, 70)],
        ["AttachmentCell", this.collections.loc["Attachment"], document.createElement("div")]
    ]

    var controlsTable = this.CreateHTMLTable();
    sendEmailForm.container.appendChild(controlsTable);

    var emailBccTable = this.CreateHTMLTable();
    emailBccTable.style.display = !this.options.jsMode ? "inline-block" : "none";

    var buttonCc = this.SmallButton(null, "Cc", null, null, null, "stiJsViewerHyperlinkButton");
    var buttonBcc = this.SmallButton(null, "Bcc", null, null, null, "stiJsViewerHyperlinkButton");
    buttonCc.caption.style.padding = buttonBcc.caption.style.padding = "0 5px 0 5px";
    buttonCc.style.height = buttonBcc.style.height = "16px";
    emailBccTable.addCell(buttonCc);
    emailBccTable.addCell(buttonBcc);

    controlsTable.addCell();
    var bccCell = controlsTable.addCell(emailBccTable);
    bccCell.style.textAlign = "right";
    bccCell.style.lineHeight = "0";
    controlsTable.addRow();

    buttonCc.action = function () {
        this.style.display = "none";
        sendEmailForm.controls["EmailCcRow"].style.display = "";
    }

    buttonBcc.action = function () {
        this.style.display = "none";
        sendEmailForm.controls["EmailBccRow"].style.display = "";
    }

    for (var i = 0; i < controlProps.length; i++) {
        var control = controlProps[i][2];
        control.style.margin = this.options.isMobileDevice ? "4px 4px 12px 4px" : "4px";
        sendEmailForm.controls[controlProps[i][0]] = control;
        var textCell = controlsTable.addTextCellInLastRow(controlProps[i][1]);
        textCell.className = "stiJsViewerCaptionControls";
        controlsTable[this.options.isMobileDevice ? "addCellInNextRow" : "addCellInLastRow"](control);
        if (this.options.isMobileDevice) textCell.style.padding = "0 25px 0 4px";
        if (i < controlProps.length - 1) {            
            sendEmailForm.controls[controlProps[i + 1][0] + "Row"] = controlsTable.addRow();
        }
    }        

    sendEmailForm.show = function (exportFormat, exportSettings) {
        this.changeVisibleState(true);
        this.exportSettings = exportSettings;
        this.exportFormat = exportFormat;

        for (var i in this.controls) {
            this.controls[i].value = "";
        }

        this.controls["Email"].value = this.jsObject.options.email.defaultEmailAddress;
        this.controls["Message"].value = this.jsObject.options.email.defaultEmailMessage;
        this.controls["Subject"].value = this.jsObject.options.email.defaultEmailSubject;
        this.controls["EmailCc"].value = this.controls["EmailBcc"].value = "";

        buttonCc.style.display = buttonBcc.style.display = "";
        sendEmailForm.controls["EmailCcRow"].style.display = sendEmailForm.controls["EmailBccRow"].style.display = "none";

        var ext = this.exportFormat.toLowerCase().replace("image", "");
        switch (ext) {
            case "excel": ext = "xls"; break;
            case "excel2007": ext = "xlsx"; break;
            case "excelxml": ext = "xls"; break;
            case "html5": ext = "html"; break;
            case "jpeg": ext = "jpg"; break;
            case "ppt2007": ext = "ppt"; break;
            case "text": ext = "txt"; break;
            case "word2007": ext = "docx"; break;
        }

        this.controls["AttachmentCell"].innerHTML = this.jsObject.reportParams.reportFileName + "." + ext;
    }

    sendEmailForm.action = function () {
        sendEmailForm.exportSettings["Email"] = sendEmailForm.controls["Email"].value;
        sendEmailForm.exportSettings["Subject"] = sendEmailForm.controls["Subject"].value;
        sendEmailForm.exportSettings["Message"] = sendEmailForm.controls["Message"].value;

        if (sendEmailForm.controls["EmailCc"].value) {
            sendEmailForm.exportSettings["EmailCc"] = sendEmailForm.controls["EmailCc"].value;
        }
        if (sendEmailForm.controls["EmailBcc"].value) {
            sendEmailForm.exportSettings["EmailBcc"] = sendEmailForm.controls["EmailBcc"].value;
        }

        sendEmailForm.changeVisibleState(false);
        sendEmailForm.jsObject.postEmail(sendEmailForm.exportFormat, sendEmailForm.exportSettings);
    }
}