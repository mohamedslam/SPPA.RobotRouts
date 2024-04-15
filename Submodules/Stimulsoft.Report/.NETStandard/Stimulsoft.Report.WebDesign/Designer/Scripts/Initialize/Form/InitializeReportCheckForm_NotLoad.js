
StiMobileDesigner.prototype.InitializeReportCheckForm_ = function () {
    var jsObject = this;
    var reportCheckForm = this.BaseForm("reportCheckForm", this.loc.Report.ReportChecker, 3, this.HelpLinks["reportCheck"]);
    reportCheckForm.hideButtonsPanel();

    var checkPanel = jsObject.InitializeCheckPanel();
    reportCheckForm.container.appendChild(checkPanel);

    reportCheckForm.show = function (checkItems) {
        checkPanel.update(checkItems);
        reportCheckForm.changeVisibleState(true);
    }

    return reportCheckForm;
}


StiMobileDesigner.prototype.InitializeCheckPanel = function () {
    var checkPanel = document.createElement("div");
    checkPanel.className = "stiDesignerCheckPanel";
    var jsObject = checkPanel.jsObject = this;
    checkPanel.controls = {};

    var toolBar = document.createElement("div");
    toolBar.style.textAlign = "right";

    var settingsButton = this.SmallButton(null, null, null, "Settings.png", null, "Down", "stiDesignerFormButton");
    settingsButton.style.display = "inline-block";
    settingsButton.style.margin = "6px 12px 6px 0";
    toolBar.appendChild(settingsButton);

    checkPanel.appendChild(toolBar);
    checkPanel.appendChild(this.FormSeparator());

    var settingsMenu = this.VerticalMenu("reportCheckSettingsMenu", settingsButton, "Down", []);

    settingsButton.action = function () {
        settingsMenu.changeVisibleState(!settingsMenu.visible);
    }

    //Toolbar
    var buttons = [
        ["Error", this.loc.Report.Errors, "ReportChecker.Error.png"],
        ["Warning", this.loc.Report.Warnings, "ReportChecker.Warning.png"],
        ["Information", this.loc.Report.InformationMessages, "ReportChecker.Information.png"],
        ["ReportRenderingMessage", this.loc.Report.ReportRenderingMessages, "ReportChecker.ReportRenderingMessage.png"]
    ]

    for (var i = 0; i < buttons.length; i++) {
        var button = this.StandartSmallButton(null, null, buttons[i][1], buttons[i][2], null, null, true);
        button.name = buttons[i][0];
        button.style.margin = "4px";
        checkPanel.controls[buttons[i][0] + "Button"] = button;
        settingsMenu.innerContent.appendChild(button);
        button.setSelected(true);

        button.action = function () {
            this.setSelected(!this.isSelected);
            checkPanel.container.fill(checkPanel.checkItems);
        }
    }

    //Container
    var container = this.ReportCheckContainer(checkPanel);
    checkPanel.container = container;
    checkPanel.appendChild(container);

    checkPanel.update = function (checkItems) {
        this.checkItems = checkItems;

        if (checkItems) {
            container.progress.hide();
            container.fill(checkItems);
        }
        else {
            jsObject.SendCommandGetReportCheckItems(function (answer) {
                checkPanel.checkItems = answer.checkItems;
                container.progress.hide();
                container.fill(answer.checkItems);
            });
        }
    }

    return checkPanel;
}

StiMobileDesigner.prototype.ReportCheckContainer = function (checkPanel) {
    var container = document.createElement("div");
    var jsObject = container.jsObject = this;
    container.className = "stiDesignerCheckContainer";
    this.AddProgressToControl(container);

    container.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
    }

    container.fill = function (checkItems) {
        this.clear();
        if (!checkItems) return;
        var num = 1;

        for (var i = 0; i < checkItems.length; i++) {
            if (checkPanel.controls[checkItems[i].status + "Button"] && checkPanel.controls[checkItems[i].status + "Button"].isSelected) {
                var checkItem = jsObject.ReportCheckContainerItem(num, checkItems[i], checkPanel);
                checkItems[i].index = i;
                num++;
                this.appendChild(checkItem);
            }
        }

        if (this.childNodes.length == 0) {
            var noIssues = document.createElement("div");
            noIssues.innerHTML = jsObject.loc.Report.NoIssues;
            noIssues.style.textAlign = "center";
            noIssues.style.marginTop = "200px";
            this.appendChild(noIssues);
        }
    }

    return container;
}

StiMobileDesigner.prototype.ReportCheckContainerItem = function (num, checkObject, checkPanel) {
    var jsObject = this;
    var checkItem = document.createElement("div");
    checkItem.className = "stiDesignerCheckContainerItem";

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "100%";
    checkItem.appendChild(mainTable);
    var upTable = this.CreateHTMLTable();
    mainTable.addCell(upTable);

    //Image
    var img = document.createElement("img");
    img.style.marginRight = "12px";
    StiMobileDesigner.setImageSource(img, this.options, "ReportChecker." + checkObject.status + "32.png");
    upTable.addCell(img);

    var shortMessage = checkObject.shortMessage;
    if (!shortMessage) {
        switch (checkObject.status) {
            case "Error": shortMessage = this.loc.Report.Errors; break;
            case "Warning": shortMessage = this.loc.Report.Warnings; break;
            case "Information": shortMessage = this.loc.Report.InformationMessages; break;
            case "ReportRenderingMessage": shortMessage = this.loc.Report.ReportRenderingMessages; break;
        }
    }

    var messageText = num + ". <b>" + shortMessage + "</b><br><br>" + checkObject.longMessage;
    var textCell = upTable.addTextCell(messageText);

    var downTable = this.CreateHTMLTable();
    downTable.style.float = "right";
    mainTable.addCellInNextRow(downTable);

    //Actions
    for (var i = 0; i < checkObject.actions.length; i++) {
        if (checkObject.actions[i].name == "Edit") continue; //Temporary

        var actionButton = this.SmallButton(null, null, checkObject.actions[i].name, null, checkObject.actions[i].description, null, "stiDesignerFormButton", true);
        actionButton.actionIndex = i;
        actionButton.style.margin = "12px 0 12px 12px";
        downTable.addCell(actionButton);

        if (checkObject.actions[i].name == "Purchase") {
            actionButton.action = function () {
                jsObject.openNewWindow("https://www.stimulsoft.com/" + jsObject.options.helpLanguage + "/online-store");
            }
        }
        else {
            actionButton.action = function () {
                var this_ = this;
                this.setEnabled(false);
                jsObject.SendCommandActionCheck(checkObject.index, this.actionIndex, function (answer) {
                    this_.setEnabled(true);
                    if (answer.checkItems) {
                        checkPanel.container.fill(answer.checkItems);
                    }

                    if (answer.reportGuid && answer.reportObject) {
                        jsObject.options.reportGuid = answer.reportGuid;
                        jsObject.LoadReport(jsObject.ParseReport(answer.reportObject), true);
                        jsObject.options.reportIsModified = true;
                    }
                    jsObject.BackToSelectedComponent(answer.selectedObjectName);
                });
            }
        }
    }

    //Clipboard Button    
    var clipboardButton = this.SmallButton(null, null, null, "Copy.png", this.loc.HelpDesigner.CopyToClipboard, null, "stiDesignerFormButton", true);
    clipboardButton.style.margin = "12px 0 12px 12px";;
    clipboardButton.action = function () {
        jsObject.copyTextToClipboard(num + "." + shortMessage + "\r\n" + checkObject.longMessage);
    }
    downTable.addCell(clipboardButton);

    //View Button
    if (checkObject.previewVisible) {
        var viewButton = this.SmallButton(null, null, null, "View.png", this.loc.Toolbars.TabView, null, "stiDesignerFormButton", true);
        viewButton.style.margin = "12px 0 12px 12px";
        downTable.addCell(viewButton);

        viewButton.action = function () {
            viewButton.setEnabled(false);
            if (jsObject.options.jsMode) {
                viewButton.setEnabled(true);
                var pageIndex = 0;
                if (checkObject.pageIndex != null) pageIndex = checkObject.pageIndex;
                var pageSvg = jsObject.GetSvgPageForCheckPreview(pageIndex, checkObject.elementName);
                var checkPreviewPanel = jsObject.InitializeCheckPreviewPanel(viewButton, null);
                checkPreviewPanel.appendChild(pageSvg);
                checkPreviewPanel.show();
            }
            else {
                jsObject.SendCommandGetCheckPreview(checkObject.index, function (answer) {
                    viewButton.setEnabled(true);
                    var checkPreviewPanel = jsObject.InitializeCheckPreviewPanel(viewButton, answer.previewImage);
                    checkPreviewPanel.show();
                });
            }
        }
    }

    return checkItem;
}

StiMobileDesigner.prototype.InitializeCheckPreviewPanel = function (parentButton, previewImageSrc) {
    var checkPreviewPanel = document.createElement("div");
    if (this.options.checkPreviewPanel) {
        this.options.checkPreviewPanel.hide();
    }

    this.options.checkPreviewPanel = checkPreviewPanel;
    this.options.mainPanel.appendChild(checkPreviewPanel);
    checkPreviewPanel.className = "stiDesignerCheckPreviewPanel";
    checkPreviewPanel.jsObject = this;

    checkPreviewPanel.hide = function () {
        this.jsObject.options.mainPanel.removeChild(this);
        this.jsObject.options.checkPreviewPanel = null;
        parentButton.setSelected(false);
    }

    checkPreviewPanel.show = function () {
        checkPreviewPanel.style.left = (this.jsObject.FindPosX(parentButton, "stiDesignerMainPanel") + parentButton.offsetWidth + 5) + "px";
        var top = (this.jsObject.FindPosY(parentButton, "stiDesignerMainPanel") - checkPreviewPanel.offsetHeight - 5);
        checkPreviewPanel.style.top = (top < 0 ? 0 : top) + "px";
        parentButton.setSelected(true);
    }

    if (previewImageSrc) {
        var previewImage = document.createElement("img");
        previewImage.src = previewImageSrc;
        checkPreviewPanel.appendChild(previewImage);
    }

    return checkPreviewPanel;
}