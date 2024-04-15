
StiMobileDesigner.prototype.WizardFormCheckBox = function (name, caption, key) {
    var checkBox = this.CheckBox(null, caption);
    checkBox.style.margin = "9px 10px 9px 15px";
    checkBox.name = name;
    checkBox.key = key;
    checkBox.allwaysEnabled = true;

    return checkBox;
}

StiMobileDesigner.prototype.WizardFormColumnsOrderItem = function (name, caption) {
    var button = this.StandartSmallButton(name, null, caption, null, null, null);
    button.name = name;
    button.caption.style.fontSize = "12px";
    button.style.margin = "1px 1px 0 1px";
    button.caption.style.paddingLeft = "15px";
    button.key = caption;
    button.allwaysEnabled = true;

    return button;
}

StiMobileDesigner.prototype.WizardFormTabbedPane = function (name, tabNames) {
    var tabs = [];
    for (var i = 0; i < tabNames.length; i++) {
        tabs.push({ "name": tabNames[i], "caption": tabNames[i] });
    }
    var tabbedPane = this.TabbedPane(name, tabs, "stiDesignerStandartTab");

    return tabbedPane;
}

StiMobileDesigner.prototype.WizardFormSeparator = function (caption) {
    var separator = document.createElement("div");
    separator.className = "wizardFormSeparator";
    separator.innerHTML = caption;

    return separator;
}

StiMobileDesigner.prototype.WizardFormColumnsHeader = function (wizardForm, dataSource) {
    var header = document.createElement("div");
    header.className = "wizardFormSeparator";
    header.innerTable = this.CreateHTMLTable();
    header.innerTable.style.width = "100%";
    header.appendChild(header.innerTable);

    header.caption = header.innerTable.addCell();
    header.caption.style.width = "100%";
    header.caption.innerHTML = dataSource.name;

    header.relationText = header.innerTable.addCell();
    header.relationText.innerHTML = this.loc.PropertyMain.DataRelation + ": ";

    var relationsItems = dataSource.relations ? this.GetRelationsItemsFromDataSource(dataSource) : [];
    header.relations = this.DropDownList(null, 200, null, relationsItems, true, false, this.options.propertyControlsHeight);
    header.relations.wizardForm = wizardForm;
    header.relations.dataSourceName = dataSource.name;
    header.relations.action = function () {
        this.wizardForm.workPanels.columns.columnsHeaderKeys[this.dataSourceName] = this.key;
        this.wizardForm.workPanels.columns.update();
    };

    header.relations.style.marginLeft = "5px";
    header.innerTable.addCell(header.relations);
    header.relationText.style.display = relationsItems.length > 1 ? "" : "none";
    header.relations.style.display = relationsItems.length > 1 ? "" : "none";

    return header;
}