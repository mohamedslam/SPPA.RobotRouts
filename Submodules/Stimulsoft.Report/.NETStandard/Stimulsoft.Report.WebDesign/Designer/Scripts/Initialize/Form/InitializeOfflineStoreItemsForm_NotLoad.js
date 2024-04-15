
StiMobileDesigner.prototype.InitializeOfflineStoreItemsForm_ = function () {
    var form = this.BaseForm("offlineStoreItemsForm", this.loc.FormDesigner.title, 1);
    var jsObject = this;

    var downloadButton = this.NewReportCloudPanelWizardButton(this.loc.Buttons.Open);
    downloadButton.style.margin = "12px 0 12px 12px";

    var wizardButton = this.NewReportCloudPanelWizardButton(this.loc.Wizards.RunWizard);
    wizardButton.style.margin = "12px";

    var buttonsTable = form.buttonsPanel.firstChild;
    buttonsTable.insertCell(0, wizardButton).style.lineHeight = "0";
    buttonsTable.insertCell(0, downloadButton).style.lineHeight = "0";
    form.buttonOk.parentElement.style.display = "none";

    var mainTable = this.CreateHTMLTable();
    form.container.appendChild(mainTable);

    var sampleImage = document.createElement("img");
    sampleImage.style.margin = "12px 40px";
    sampleImage.style.border = "1px solid #808080";
    mainTable.addCell(sampleImage);

    var textTable = this.CreateHTMLTable();
    mainTable.addCell(textTable).style.verticalAlign = "top";

    var headerCell = textTable.addCell();
    headerCell.style.fontSize = "28px";
    headerCell.style.padding = "12px 40px 12px 0";
    headerCell.className = "stiDesignerTextContainer";

    var creatorCell = textTable.addCellInNextRow();
    creatorCell.className = "stiDesignerTextContainer";
    creatorCell.style.color = "#5a83ad";

    var descriptCell = textTable.addCellInNextRow();
    descriptCell.className = "stiDesignerTextContainer";
    descriptCell.style.padding = "12px 40px 12px 0";
    descriptCell.style.maxWidth = "250px";
    descriptCell.style.lineHeight = "1.3";

    form.show = function (itemObject) {
        form.itemObject = itemObject;
        StiMobileDesigner.setImageSource(sampleImage, jsObject.options, itemObject.image.name);
        sampleImage.style.width = (itemObject.image.size.width * 1.5) + "px";
        sampleImage.style.height = (itemObject.image.size.height * 1.5) + "px";
        headerCell.innerHTML = itemObject.text;
        creatorCell.innerHTML = "Stimulsoft";
        descriptCell.innerHTML = itemObject.description;
        form.changeVisibleState(true);
    }

    downloadButton.action = function () {
        form.changeVisibleState(false);
        var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
        fileMenu.changeVisibleState(false);
        setTimeout(function () {
            jsObject.SendCommandOpenWizardReport(form.itemObject.resourceName);
        }, 200);
    }

    wizardButton.action = function () {
        form.changeVisibleState(false);

        if (form.itemObject.wizard.action) {
            jsObject.StartWizardForm2(form.itemObject.wizard.action, form.itemObject.wizard.templateName);
        }
    }

    return form;
}