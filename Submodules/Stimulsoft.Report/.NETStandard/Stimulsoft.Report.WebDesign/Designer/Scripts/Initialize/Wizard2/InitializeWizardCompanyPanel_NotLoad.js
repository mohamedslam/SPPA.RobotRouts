
StiMobileDesigner.prototype.InitializeWizardCompanyPanel = function (templates, form, level) {
    var companyPanel = document.createElement("div");
    companyPanel.name = name;
    companyPanel.className = "wizardStepPanel";
    companyPanel.jsObject = this;
    companyPanel.style.maxHeight = "410px";
    companyPanel.level = level;

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "500px";
    mainTable.style.marginLeft = "auto";
    mainTable.style.marginRight = "auto";
    mainTable.style.marginTop = "10px";
    companyPanel.appendChild(mainTable);

    var imageControl = this.ImageControl("wizardCompanyImage", 200, 175, false);
    imageControl.style.background = "white";
    imageControl.hintText.style.whiteSpace = "normal";
    var icell = mainTable.addCell(imageControl);
    icell.style.verticalAlign = "top";
    icell.style.paddingRight = "10px";
    companyPanel.imageControl = imageControl;

    var variableTable = this.CreateHTMLTable();
    mainTable.addCell(variableTable);

    companyPanel.update = function (needReset) {
        if (needReset) {
            imageControl.setImage(form.getTemplate().logo ? "data:image/png;base64," + form.getTemplate().logo : "");
            while (variableTable.childNodes.length > 0) variableTable.removeChild(variableTable.childNodes[0]);
            var variables = form.getTemplate().variables;
            companyPanel.areas = [];

            for (var i in variables) {
                variableTable.addTextCellInNextRow(variables[i].locName).className = "wizardMappingText";
                var textArea = this.jsObject.TextArea(null, 430, variables[i].value.length > 50 || variables[i].value.indexOf("\n") > 0 ? 44 : 18);
                textArea.style.paddingTop = "4px";
                textArea.value = variables[i].value;
                textArea.vName = variables[i].name;
                variableTable.addCellInLastRow(textArea);
                companyPanel.areas.push(textArea);
            }
            if (companyPanel.level < form.stepLevel)
                form.stepLevel = companyPanel.level;
        }
        form.enableButtons(true, true, false);

        if (this.level > form.stepLevel)
            form.stepLevel = this.level;
    }

    companyPanel.getReportOptions = function (options) {
        options.company = [];
        for (var i in companyPanel.areas)
            options.company.push({ "name": companyPanel.areas[i].vName, "value": companyPanel.areas[i].value });
        options.logo = companyPanel.imageControl.imageContainer.src;
    }

    companyPanel.onShow = function () { };
    companyPanel.onHide = function () { };

    form.appendStepPanel(companyPanel, form.jsObject.loc.Wizards.infoSelectColumns);
    return companyPanel;
}