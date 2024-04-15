
StiMobileDesigner.prototype.InitializeEditDataSourceFromOtherDatasourcesForm_ = function () {

    var editDataSourceForm = this.BaseForm("editDataSourceFromOtherDatasourcesForm", this.loc.PropertyMain.DataSource, 1, this.HelpLinks["dataSourceFromOtherDatasources"]);
    editDataSourceForm.datasource = null;
    editDataSourceForm.mode = "Edit";
    editDataSourceForm.controls = {};

    //Main Table
    var mainTable = this.CreateHTMLTable();
    mainTable.className = "stiDesignerImageFormMainPanel";
    editDataSourceForm.container.appendChild(mainTable);
    editDataSourceForm.container.style.padding = "0px";

    var buttonProps = [
        ["DataSource", "DataForm.DataFormDataSource.png", this.loc.PropertyMain.DataSource],
        ["Sort", "DataForm.DataFormSort.png", this.loc.PropertyMain.Sort],
        ["Filter", "DataForm.DataFormFilters.png", this.loc.PropertyMain.Filters],
        ["Groups", "VirtualDataSourceForm.DataGroups.png", this.loc.Wizards.Groups],
        ["Results", "VirtualDataSourceForm.DataSummary.png", this.loc.Wizards.Results]
    ];

    //Add Panels && Buttons
    var buttonsPanel = mainTable.addCell();
    var panelsContainer = mainTable.addCell();
    buttonsPanel.style.verticalAlign = "top";
    buttonsPanel.style.paddingTop = "6px";
    editDataSourceForm.mainButtons = {};
    editDataSourceForm.panels = {};

    for (var i = 0; i < buttonProps.length; i++) {
        var panel = document.createElement("Div");
        panel.className = "stiDesignerEditFormPanel";
        panel.style.width = "560px";
        panel.style.display = i != 0 ? "none" : "inline-block";
        panelsContainer.appendChild(panel);
        editDataSourceForm.panels[buttonProps[i][0]] = panel;
        panel.onShow = function () { };

        var button = this.FormTabPanelButton(null, buttonProps[i][2], buttonProps[i][1], buttonProps[i][2], null, { width: 24, height: 24 }, 34);
        editDataSourceForm.mainButtons[buttonProps[i][0]] = button;
        buttonsPanel.appendChild(button);
        button.panelName = buttonProps[i][0];
        button.action = function () {
            editDataSourceForm.showPanel(this.panelName);
        }
    }

    //Data Source
    var textBoxesTable = this.CreateHTMLTable();
    editDataSourceForm.panels.DataSource.appendChild(textBoxesTable);

    var textBoxes = [
        ["name", this.loc.PropertyMain.Name, "12px 0 6px 12px"],
        ["alias", this.loc.PropertyMain.Alias, "6px 0 12px 12px"]
    ]

    for (var i = 0; i < textBoxes.length; i++) {
        if (i != 0) textBoxesTable.addRow();
        var textCell = textBoxesTable.addTextCellInLastRow(textBoxes[i][1]);
        textCell.className = "stiDesignerCaptionControlsBigIntervals";
        textCell.style.minWidth = "130px";

        var textBox = this.TextBox(null, 230);
        textBox.style.margin = textBoxes[i][2];
        editDataSourceForm.controls[textBoxes[i][0]] = textBox;
        textBoxesTable.addCellInLastRow(textBox);
    }

    editDataSourceForm.controls.name.action = function () {
        if (this.oldValue == editDataSourceForm.controls.alias.value) {
            editDataSourceForm.controls.alias.value = this.value;
        }
    }

    var dataSourcesTree = this.DataSourcesTree(null, this.options.isTouchDevice ? 346 : 356);
    dataSourcesTree.className = "stiSimpleContainerWithBorder";
    dataSourcesTree.style.margin = "0 12px 0 12px";
    dataSourcesTree.style.overflow = "auto";
    editDataSourceForm.panels.DataSource.appendChild(dataSourcesTree);

    dataSourcesTree.action = function () {
        editDataSourceForm.action();
    }

    //Sort
    var sortControl = this.SortControl("editDataSourceFormSortControl" + this.generateKey(), null, null, 388);
    editDataSourceForm.panels.Sort.appendChild(sortControl);

    sortControl.toolBar.style.margin = "12px 0 0 12px";

    var sortContainer = sortControl.sortContainer
    sortContainer.className = "stiSimpleContainerWithBorder";
    sortContainer.style.margin = "12px 12px 0 12px";
    sortContainer.style.overflow = "auto";

    editDataSourceForm.panels.Sort.onShow = function () {
        var currentDataSourceName = editDataSourceForm.getCurrentDataSourceName();
        if (sortControl.currentDataSourceName != currentDataSourceName) sortControl.sortContainer.clear();
        sortControl.currentDataSourceName = currentDataSourceName;
    }

    //Filter
    var filterControl = this.FilterControl("editDataSourceFormFilterControl" + this.generateKey(), null, null, 388);
    editDataSourceForm.panels.Filter.appendChild(filterControl);

    filterControl.controls.toolBar.style.margin = "12px 0 0 12px";
    filterControl.controls.filterOn.parentElement.style.display = "none";

    var filterContainer = filterControl.controls.filterContainer;
    filterContainer.className = "stiSimpleContainerWithBorder";
    filterContainer.style.margin = "12px 12px 0 12px";
    filterContainer.style.overflow = "auto";

    editDataSourceForm.panels.Filter.onShow = function () {
        var currentDataSourceName = editDataSourceForm.getCurrentDataSourceName();
        if (filterControl.currentDataSourceName != currentDataSourceName) filterControl.controls.filterContainer.clear();
        filterControl.currentDataSourceName = currentDataSourceName;
    }

    //Groups
    var groupsControl = this.GroupsControl("editDataSourceFormGroupsControl" + this.generateKey(), null, 388);
    editDataSourceForm.panels.Groups.appendChild(groupsControl);

    groupsControl.toolBar.style.margin = "12px 0 0 12px";

    var sortContainer = groupsControl.sortContainer
    sortContainer.className = "stiSimpleContainerWithBorder";
    sortContainer.style.margin = "12px 12px 0 12px";
    sortContainer.style.overflow = "auto";

    editDataSourceForm.panels.Groups.onShow = function () {
        var currentDataSourceName = editDataSourceForm.getCurrentDataSourceName();
        if (groupsControl.currentDataSourceName != currentDataSourceName) groupsControl.sortContainer.clear();
        groupsControl.currentDataSourceName = currentDataSourceName;
    }

    //Results
    var resultsControl = this.ResultsControl("editDataSourceFormResultsControl" + this.generateKey(), null, 388);
    editDataSourceForm.panels.Results.appendChild(resultsControl);

    resultsControl.toolBar.style.margin = "12px 0 0 12px";

    var resultContainer = resultsControl.resultContainer
    resultContainer.className = "stiSimpleContainerWithBorder";
    resultContainer.style.margin = "12px 12px 0 12px";
    resultContainer.style.overflow = "auto";

    editDataSourceForm.panels.Results.onShow = function () {
        var currentDataSourceName = editDataSourceForm.getCurrentDataSourceName();
        if (resultsControl.currentDataSourceName != currentDataSourceName) resultsControl.resultContainer.clear();
        resultsControl.currentDataSourceName = currentDataSourceName;
    }

    editDataSourceForm.getCurrentDataSourceName = function () {
        return dataSourcesTree.selectedItem ? dataSourcesTree.selectedItem.itemObject.name : null;
    }

    editDataSourceForm.rebuildTrees = function (objectName) {
        if (objectName) {
            dataSourcesTree.build(true, true);
            var dataSourceItem = dataSourcesTree.getItem(objectName, null, "DataSource");
            if (dataSourceItem) {
                dataSourceItem.setSelected();
                dataSourceItem.openTree();
                dataSourcesTree.update();
            }
        }
    }

    editDataSourceForm.showPanel = function (selectedPanelName) {
        this.selectedPanelName = selectedPanelName;
        for (var panelName in this.panels) {
            this.panels[panelName].style.display = selectedPanelName == panelName ? "inline-block" : "none";
            this.mainButtons[panelName].setSelected(selectedPanelName == panelName);
            if (selectedPanelName == panelName) this.panels[panelName].onShow();
        }
    }

    editDataSourceForm.onhide = function () {
        this.jsObject.DeleteTemporaryMenus();
    }

    editDataSourceForm.onshow = function () {
        this.showPanel("DataSource");
        this.mode = "Edit";

        if (typeof (this.datasource) == "string") {
            this.datasource = this.jsObject.DataSourceObject(this.datasource);
            this.mode = "New";
        }
        this.caption.innerHTML = this.jsObject.loc.FormDictionaryDesigner["DataSource" + this.mode];

        this.editableDictionaryItem = this.mode == "Edit" && this.jsObject.options.dictionaryTree
            ? this.jsObject.options.dictionaryTree.selectedItem : null;

        //DataSource                
        this.controls.name.hideError();
        this.controls.name.focus();
        this.controls.name.value = this.datasource.name;
        this.controls.alias.value = this.datasource.alias;

        dataSourcesTree.build(true, true);
        var selfDataSourceItem = dataSourcesTree.getItem(this.datasource.name, null, "DataSource");
        if (selfDataSourceItem && selfDataSourceItem.parent) {
            if (this.jsObject.GetCountObjects(selfDataSourceItem.parent.childs) == 1)
                selfDataSourceItem.parent.remove();
            else
                selfDataSourceItem.remove();
        }

        var dataSourceItem = dataSourcesTree.getItem(this.datasource.nameInSource, null, "DataSource");
        if (dataSourceItem) {
            dataSourceItem.setSelected();
            dataSourceItem.openTree();
        }

        //Sort
        var sorts = this.datasource.sortData != "" ? JSON.parse(StiBase64.decode(this.datasource.sortData)) : [];
        sortControl.currentDataSourceName = editDataSourceForm.getCurrentDataSourceName();
        sortControl.fill(sorts);

        //Filter
        var filters = this.datasource.filterData != "" ? JSON.parse(StiBase64.decode(this.datasource.filterData)) : [];
        filterControl.currentDataSourceName = editDataSourceForm.getCurrentDataSourceName();
        filterControl.fill(filters, this.datasource.filterOn, this.datasource.filterMode);

        //Groups
        var groups = this.datasource.groupsData != "" ? JSON.parse(StiBase64.decode(this.datasource.groupsData)) : [];
        groupsControl.currentDataSourceName = editDataSourceForm.getCurrentDataSourceName();
        groupsControl.fill(groups);

        //Results
        var results = this.datasource.resultsData != "" ? JSON.parse(StiBase64.decode(this.datasource.resultsData)) : [];
        resultsControl.currentDataSourceName = editDataSourceForm.getCurrentDataSourceName();
        resultsControl.fill(results);
    }

    editDataSourceForm.action = function () {
        this.datasource["mode"] = this.mode;

        if ((this.mode == "New" || this.datasource.name != this.controls.name.value) &&
            !(this.controls.name.checkExists(this.jsObject.GetDataSourcesFromDictionary(this.jsObject.options.report.dictionary), "name") &&
                this.controls.name.checkExists(this.jsObject.GetVariablesFromDictionary(this.jsObject.options.report.dictionary), "name"))) {
            this.showPanel("DataSource");
            return;
        }

        if (this.mode == "Edit") this.datasource.oldName = this.datasource.name;

        this.datasource.name = this.controls.name.value;
        this.datasource.alias = this.controls.alias.value;
        this.datasource.nameInSource = dataSourcesTree.selectedItem && dataSourcesTree.selectedItem.itemObject.typeItem != "NoItem"
            ? dataSourcesTree.selectedItem.itemObject.name : "";

        var filterResult = filterControl.getValue();
        this.datasource.filterOn = filterResult.filterOn;
        this.datasource.filterMode = filterResult.filterMode;
        this.datasource.filterData = StiBase64.encode(JSON.stringify(filterResult.filters));

        var sortResult = sortControl.getValue();
        this.datasource.sortData = sortResult.length == 0 ? "" : StiBase64.encode(JSON.stringify(sortResult));

        var groupColumns = groupsControl.getValue();
        this.datasource.groupsData = groupColumns.length == 0 ? "" : StiBase64.encode(JSON.stringify(groupColumns));

        var results = resultsControl.getValue();
        this.datasource.resultsData = results.length == 0 ? "" : StiBase64.encode(JSON.stringify(results));

        if (!this.controls.name.checkNotEmpty(this.jsObject.loc.PropertyMain.Name)) {
            this.showPanel("DataSource");
            return;
        }

        this.changeVisibleState(false);
        this.jsObject.SendCommandCreateOrEditDataSource(this.datasource);
    }

    return editDataSourceForm;
}