
StiMobileDesigner.prototype.InitializeWizardMappingPanel = function (templates, form, level) {
    var mappingPanel = document.createElement("div");
    mappingPanel.name = name;
    mappingPanel.className = "wizardStepPanel";
    mappingPanel.jsObject = this;
    mappingPanel.style.maxHeight = "410px";
    mappingPanel.level = level;

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "500px";
    mainTable.style.marginLeft = "auto";
    mainTable.style.marginRight = "auto";
    mainTable.style.marginTop = "10px";
    mappingPanel.appendChild(mainTable);

    mappingPanel.update = function (needReset) {
        if (needReset) {
            while (mainTable.childNodes.length > 0) mainTable.removeChild(mainTable.childNodes[0]);
            var datasource = form.getDatasource();
            var columns = datasource.columns;
            var columnNames = [{ caption: form.jsObject.loc.MainMenu.menuSelectColumn, name: "emptyWizardMapping" }];
            for (var i in columns)
                columnNames.push({ caption: columns[i].name, name: columns[i].name, key: "{" + datasource.name + "." + columns[i].name + "}" });

            var template = form.getTemplate();
            mappingPanel.headers = [];
            for (var i in template.headerComps) {
                var header = template.headerComps[i];
                mainTable.addTextCellInNextRow(header.text).className = "wizardMappingText";
                var list = mappingPanel.jsObject.DropDownList("wizardMapping" + i, 250, null, columnNames, true);
                list.menu.items.emptyWizardMapping.action();
                list.headerName = header.name;
                mainTable.addCellInLastRow(list);
                mappingPanel.headers.push(list);
            }
        }
        form.enableButtons(true, true, false);
        if (this.level > form.stepLevel)
            form.stepLevel = this.level;
    }

    mappingPanel.getReportOptions = function (options) {
        options.mapping = {};
        for (var i in mappingPanel.headers) {
            var list = mappingPanel.headers[i];
            var key = list.menu.dropDownList.key;
            if (key != null)
                options.mapping[list.headerName] = key;
        }
    }

    mappingPanel.onShow = function () { };
    mappingPanel.onHide = function () { };

    form.appendStepPanel(mappingPanel, form.jsObject.loc.Wizards.Mapping);
    return mappingPanel;
}