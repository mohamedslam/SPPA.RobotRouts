
StiMobileDesigner.prototype.InitializeSaveReportForm_ = function () {
    var jsObject = this;
    var saveReportForm = this.BaseForm("saveReport", this.loc.A_WebViewer.SaveReport, 3);
    saveReportForm.buttonOk.caption.innerHTML = this.loc.A_WebViewer.SaveReport;
    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "5px";
    saveReportForm.container.appendChild(innerTable);

    innerTable.addTextCell(this.loc.Cloud.labelFileName.replace(":", "")).className = "stiDesignerCaptionControlsBigIntervals";

    saveReportForm.reportNameTextBox = this.TextBox("saveReportTextBox", 200);
    var textBoxCell = innerTable.addCell(saveReportForm.reportNameTextBox);
    textBoxCell.className = "stiDesignerControlCellsBigIntervals";

    saveReportForm.show = function (saveAs, nextFunc) {
        this.caption.innerHTML = saveAs ? this.jsObject.loc.MainMenu.menuFileSaveAs.replace("...", "") : this.jsObject.loc.A_WebViewer.SaveReport;
        this.isSaveAs = saveAs;
        this.nextFunc = nextFunc;
        this.changeVisibleState(true);
        var report = this.jsObject.options.report;
        if (report) {
            var fileName = report.properties.reportFile;
            if (!fileName) fileName = (StiBase64.decode(report.properties.reportName.replace("Base64Code;", "")) || "Report") + ".mrt";
            if ((this.jsObject.options.cloudMode || this.jsObject.options.serverMode) && this.jsObject.options.cloudParameters.reportTemplateItemKey) {
                fileName = this.jsObject.options.cloudParameters.reportName + ".mrt";
            }
            if (report.encryptedPassword) {
                if (jsObject.EndsWith(fileName.toLowerCase(), ".mrt") || jsObject.EndsWith(fileName.toLowerCase(), ".mrz")) {
                    fileName = fileName.substring(0, fileName.length - 4) + ".mrx";
                }
            }
            this.reportNameTextBox.value = fileName;
        }
        this.reportNameTextBox.focus();
    }

    saveReportForm.action = function () {
        var report = this.jsObject.options.report;
        if (report) {
            var reportFile = this.reportNameTextBox.value;
            var isNewReport = !report.properties.reportFile;
            report.properties.reportFile = reportFile;
            var reportName = reportFile.substring(reportFile.lastIndexOf("/")).substring(reportFile.lastIndexOf("\\"));
            this.jsObject.SetWindowTitle(reportName ? reportName + " - " + this.jsObject.loc.FormDesigner.title : this.jsObject.loc.FormDesigner.title);
        }

        this.changeVisibleState(false);

        if (this.isSaveAs)
            this.jsObject.SendCommandSaveAsReport(null, isNewReport);
        else
            this.jsObject.SendCommandSaveReport(isNewReport);

        if (this.nextFunc) this.nextFunc();
    }

    this.addEvent(saveReportForm.reportNameTextBox, 'keydown', function (e) {
        if (e && e.keyCode == 13 && saveReportForm.jsObject.options.controlsIsFocused) {
            saveReportForm.action();
        }
    });

    return saveReportForm;
}