
StiMobileDesigner.prototype.InitializeEditDataSourceFromCrossTabForm_ = function () {

    var form = this.BaseForm("editDataSourceFromCrossTabForm", this.loc.PropertyMain.DataSource, 3, this.HelpLinks["dataSourceEdit"]);
    form.datasource = null;
    form.mode = "Edit";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "5px 0 5px 0";
    form.container.appendChild(innerTable);

    var textBoxes = [
        ["name", this.loc.PropertyMain.Name],
        ["alias", this.loc.PropertyMain.Alias]
    ]

    for (var i = 0; i < textBoxes.length; i++) {
        form[textBoxes[i][0] + "ControlRow"] = innerTable.addRow();
        var text = innerTable.addCellInLastRow();
        text.className = "stiDesignerCaptionControlsBigIntervals";
        text.innerHTML = textBoxes[i][1];
        var textBox = this.TextBox(null, 230);
        form[textBoxes[i][0] + "Control"] = textBox;
        innerTable.addCellInLastRow(textBox).className = "stiDesignerControlCellsBigIntervals2";
    }

    var crossTabsTree = this.CrossTabsTree(400, 250);
    crossTabsTree.className = "stiCrossTabsTree";
    form.container.appendChild(crossTabsTree);

    form.onshow = function () {
        this.mode = "Edit";
        if (typeof (this.datasource) == "string") {
            this.datasource = this.jsObject.DataSourceObject(this.datasource, "");
            this.mode = "New";
        }
        this.editableDictionaryItem = this.mode == "Edit" && this.jsObject.options.dictionaryTree
            ? this.jsObject.options.dictionaryTree.selectedItem : null;
        this.nameControl.hideError();
        this.nameControl.focus();
        var caption = this.jsObject.loc.FormDictionaryDesigner["DataSource" + this.mode];

        if (caption) this.caption.innerHTML = caption;
        var props = ["name", "alias"];
        for (var i = 0; i < props.length; i++) {
            if (this.datasource[props[i]] != null) {
                this[props[i] + "Control"].value = this.datasource[props[i]];
            }
        }

        crossTabsTree.build(this.datasource.nameInSource);
    }

    form.action = function () {
        this.datasource["mode"] = this.mode;

        if (!this.nameControl.checkNotEmpty(this.jsObject.loc.PropertyMain.Name)) return;
        if ((this.mode == "New" || this.nameControl.value != this.datasource.name) &&
            !(this.nameControl.checkExists(this.jsObject.GetDataSourcesFromDictionary(this.jsObject.options.report.dictionary), "name") &&
                this.nameControl.checkExists(this.jsObject.GetVariablesFromDictionary(this.jsObject.options.report.dictionary), "name")))
            return;

        if (this.mode == "Edit") this.datasource.oldName = this.datasource.name;

        var props = ["name", "alias"];
        for (var i = 0; i < props.length; i++) {
            if (this.datasource[props[i]] != null) {
                this.datasource[props[i]] = this[props[i] + "Control"].value;
            }
        }

        if (crossTabsTree.selectedItem) {
            this.datasource.nameInSource = crossTabsTree.selectedItem.itemObject.nameInSource;
        }

        this.changeVisibleState(false);
        this.jsObject.SendCommandCreateOrEditDataSource(this.datasource);
    }

    return form;
}

StiMobileDesigner.prototype.CrossTabsTree = function (width, height) {
    var tree = this.Tree();
    if (width) tree.style.width = width + "px";
    if (height) tree.style.height = height + "px";

    tree.build = function (selectedCrossTabName) {
        tree.clear();

        var mainItem = this.jsObject.DictionaryTreeItemFromObject({ name: "Cross-Tabs", typeIcon: "Folder", nameInSource: "" }, tree, true);
        tree.appendChild(mainItem);
        mainItem.setOpening(true);

        if (this.jsObject.options.report) {
            var crossTabNames = this.jsObject.GetAllCrossTabsInReport();
            for (var i = 0; i < crossTabNames.length; i++) {
                var crossTabItem = this.jsObject.DictionaryTreeItemFromObject({ name: crossTabNames[i], typeIcon: "SmallComponents.StiCrossTab", nameInSource: crossTabNames[i] }, tree, true);
                mainItem.addChild(crossTabItem);
                if (crossTabNames[i] == selectedCrossTabName) {
                    crossTabItem.setSelected();
                }
            }
        }
    }

    return tree;
}