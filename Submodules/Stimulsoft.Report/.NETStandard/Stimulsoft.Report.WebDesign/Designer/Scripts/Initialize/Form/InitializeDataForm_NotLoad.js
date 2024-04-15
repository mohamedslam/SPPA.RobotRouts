
StiMobileDesigner.prototype.InitializeDataForm_ = function () {
    var jsObject = this;
    var dataForm = this.BaseFormPanel("dataForm", this.loc.PropertyMain.Data, 1, this.HelpLinks["data"]);

    //Main Table
    var mainTable = this.CreateHTMLTable();
    mainTable.className = "stiDesignerImageFormMainPanel";
    dataForm.container.appendChild(mainTable);
    dataForm.container.style.padding = "0px";

    var buttonProps = [
        ["DataSource", "DataForm.DataFormDataSource.png", this.loc.PropertyMain.DataSource],
        ["Relation", "DataForm.DataFormDataRelation.png", this.loc.PropertyMain.DataRelation],
        ["MasterComponent", "DataForm.DataFormMasterComponent.png", this.loc.PropertyMain.MasterComponent],
        ["Sort", "DataForm.DataFormSort.png", this.loc.PropertyMain.Sort],
        ["Filter", "DataForm.DataFormFilters.png", this.loc.PropertyMain.Filters]
    ];

    //Add Panels && Buttons
    var buttonsPanel = mainTable.addCell();
    var panelsContainer = mainTable.addCell();
    buttonsPanel.style.verticalAlign = "top";
    buttonsPanel.style.paddingTop = "6px";
    dataForm.mainButtons = {};
    dataForm.panels = {};

    for (var i = 0; i < buttonProps.length; i++) {
        var panel = document.createElement("Div");
        panel.className = "stiDesignerEditFormPanel";
        panel.style.width = "560px";
        panel.style.height = "455px";
        panel.style.overflow = "visible";
        panel.style.display = i != 0 ? "none" : "inline-block";
        panelsContainer.appendChild(panel);
        dataForm.panels[buttonProps[i][0]] = panel;
        panel.onShow = function () { };

        var button = this.FormTabPanelButton(null, buttonProps[i][2], buttonProps[i][1], buttonProps[i][2], null, { width: 24, height: 24 }, 34);
        dataForm.mainButtons[buttonProps[i][0]] = button;
        buttonsPanel.appendChild(button);
        button.panelName = buttonProps[i][0];
        button.action = function () {
            dataForm.setMode(this.panelName);
        }

        //add marker
        var marker = document.createElement("div");
        marker.style.display = "none";
        marker.className = "stiUsingMarker";
        var markerInner = document.createElement("div");
        marker.appendChild(markerInner);
        button.style.position = "relative";
        button.appendChild(marker);
        button.marker = marker;
    }

    //Data Source
    var toolBarDataSource = this.CreateHTMLTable();
    dataForm.panels.DataSource.appendChild(toolBarDataSource);
    var newDataSourceButton = this.FormButton(null, "dataFormNewDataSourceButton", this.loc.FormDictionaryDesigner.DataSourceNew);
    var newBusinessObjectButton = this.FormButton(null, "dataFormNewBusinessObjectButton", this.loc.FormDictionaryDesigner.NewBusinessObject);
    newDataSourceButton.setEnabled(!this.options.permissionDataSources || this.options.permissionDataSources.indexOf("All") >= 0 || this.options.permissionDataSources.indexOf("Create") >= 0);
    newBusinessObjectButton.setEnabled(!this.options.permissionBusinessObjects || this.options.permissionBusinessObjects.indexOf("All") >= 0 || this.options.permissionBusinessObjects.indexOf("Create") >= 0);

    toolBarDataSource.style.margin = "12px 12px 0 12px";
    toolBarDataSource.addCell(newDataSourceButton);
    toolBarDataSource.addCell(newBusinessObjectButton).style.paddingLeft = "12px";

    newDataSourceButton.action = function () {
        jsObject.InitializeSelectConnectionForm(function (selectConnectionForm) {
            selectConnectionForm.changeVisibleState(true);
        });
    };

    newBusinessObjectButton.action = function () {
        dataForm.sinchronizeWithMainDictionary();
        jsObject.InitializeEditDataSourceForm(function (editDataSourceForm) {
            editDataSourceForm.datasource = "BusinessObject";
            editDataSourceForm.changeVisibleState(true);
        });
    };

    var dataSourcesTree = this.DataSourcesTree(null, 356);
    dataSourcesTree.className = "stiSimpleContainerWithBorder";
    dataSourcesTree.style.margin = "12px";
    dataSourcesTree.style.overflow = "auto";
    dataForm.panels.DataSource.appendChild(dataSourcesTree);
    dataSourcesTree.action = function () { dataForm.action(); }

    var toolBar2DataSource = this.CreateHTMLTable();
    dataForm.panels.DataSource.appendChild(toolBar2DataSource);

    var textCount = toolBar2DataSource.addTextCell(this.loc.PropertyMain.CountData);
    textCount.className = "stiDesignerCaptionControls";
    textCount.style.padding = "0 15px 0 12px";

    var countData = this.TextBox(null, 70);
    toolBar2DataSource.addCell(countData);

    countData.action = function () {
        this.value = jsObject.StrToCorrectPositiveInt(this.value);
        dataSourcesTree.update();
    }

    dataSourcesTree.update = function () {
        dataSourcesTree.setEnabled(countData.value == "0");
    }

    //Relation
    var toolBarRelation = this.CreateHTMLTable();
    toolBarRelation.style.margin = "12px 12px 0 12px";
    dataForm.panels.Relation.appendChild(toolBarRelation);

    var newRelationButton = this.FormButton(null, "dataFormNewRelationButton", this.loc.FormDictionaryDesigner.RelationNew);
    newRelationButton.setEnabled(!this.options.permissionDataRelations || this.options.permissionDataRelations.indexOf("All") >= 0 || this.options.permissionDataRelations.indexOf("Create") >= 0);
    toolBarRelation.addCell(newRelationButton);

    newRelationButton.action = function () {
        dataForm.sinchronizeWithMainDictionary();
        jsObject.InitializeEditRelationForm(function (editRelationForm) {
            editRelationForm.relation = null;
            editRelationForm.changeVisibleState(true);
        });
    };

    var relationsTree = this.RelationsTree(null, 390);
    relationsTree.className = "stiSimpleContainerWithBorder";
    relationsTree.style.margin = "12px 12px 0 12px";
    relationsTree.style.overflow = "auto";
    dataForm.panels.Relation.appendChild(relationsTree);

    relationsTree.action = function () {
        dataForm.action();
    }

    dataForm.panels.Relation.onShow = function () {
        var dataSource = jsObject.GetDataSourceByNameFromDictionary(dataSourcesTree.selectedItem ? dataSourcesTree.selectedItem.itemObject.name : "");
        var currRelationNameInSource = relationsTree.selectedItem ? relationsTree.selectedItem.itemObject.nameInSource : null;
        relationsTree.build(dataSource);
        if (currRelationNameInSource) {
            var relationItem = relationsTree.getItem(currRelationNameInSource, "nameInSource");
            if (relationItem) relationItem.setSelected();
        }
    }

    //MasterComponent
    var masterComponentsTree = this.MasterComponentsTree();
    masterComponentsTree.className = "stiSimpleContainerWithBorder";
    masterComponentsTree.style.margin = "12px 12px 0 12px";
    masterComponentsTree.style.overflow = "auto";
    masterComponentsTree.style.height = "428px";
    dataForm.panels.MasterComponent.appendChild(masterComponentsTree);

    masterComponentsTree.action = function () {
        dataForm.action();
    }

    //Sort
    var sortControl = this.SortControl("dataFormSortControl" + this.generateKey(), null, null, 390);
    dataForm.panels.Sort.appendChild(sortControl);

    sortControl.toolBar.style.margin = "12px 0 0 12px";

    var sortContainer = sortControl.sortContainer
    sortContainer.className = "stiSimpleContainerWithBorder";
    sortContainer.style.margin = "12px 12px 0 12px";
    sortContainer.style.overflow = "auto";

    dataForm.panels.Sort.onShow = function () {
        var currentDataSourceName = dataForm.getCurrentDataSourceName();
        if (sortControl.currentDataSourceName != currentDataSourceName) sortControl.sortContainer.clear();
        sortControl.currentDataSourceName = currentDataSourceName;
    }

    //Filter
    var filterControl = this.FilterControl("dataFormFilterControl" + this.generateKey(), null, null, 390);
    dataForm.panels.Filter.appendChild(filterControl);

    filterControl.controls.toolBar.style.margin = "12px 0 0 12px";
    filterControl.controls.filterOn.style.margin = "0 20px 0px 20px";

    var filterContainer = filterControl.controls.filterContainer;
    filterContainer.className = "stiSimpleContainerWithBorder";
    filterContainer.style.margin = "12px 12px 0 12px";
    filterContainer.style.overflow = "auto";

    dataForm.panels.Filter.onShow = function () {
        var currentDataSourceName = dataForm.getCurrentDataSourceName();
        if (filterControl.currentDataSourceName != currentDataSourceName) filterControl.controls.filterContainer.clear();
        filterControl.currentDataSourceName = currentDataSourceName;
    }

    //Form Methods
    dataForm.sinchronizeWithMainDictionary = function () {
        if (jsObject.options.dictionaryTree && dataSourcesTree.selectedItem && dataSourcesTree.selectedItem.itemObject.typeItem && dataSourcesTree.selectedItem.itemObject.name) {
            var item = jsObject.options.dictionaryTree.getItemByNameAndType(dataSourcesTree.selectedItem.itemObject.name, dataSourcesTree.selectedItem.itemObject.typeItem);
            if (item) item.setSelected();
        }
    }

    dataForm.getCurrentDataSourceName = function () {
        var dataSourceName = null;
        if (dataSourcesTree.selectedItem) {
            if (dataSourcesTree.selectedItem.itemObject.typeItem == "DataSource")
                dataSourceName = dataSourcesTree.selectedItem.itemObject.name;
            else if (dataSourcesTree.selectedItem.itemObject.typeItem == "BusinessObject")
                dataSourceName = dataSourcesTree.selectedItem.getBusinessObjectStringFullName();
        }
        return dataSourceName;
    }

    dataForm.setMode = function (mode) {
        dataForm.mode = mode;
        for (var panelName in dataForm.panels) {
            dataForm.panels[panelName].style.display = mode == panelName ? "inline-block" : "none";
            dataForm.mainButtons[panelName].setSelected(mode == panelName);
            if (mode == panelName) dataForm.panels[panelName].onShow();
        }
        var propertiesPanel = jsObject.options.propertiesPanel;
        propertiesPanel.setEnabled(false);
    }

    dataForm.onhide = function () {
        jsObject.options.propertiesPanel.setDictionaryMode(false);
        clearTimeout(this.markerTimer);
        jsObject.DeleteTemporaryMenus();
    }

    dataForm.rebuildTrees = function (objectName, objectType) {
        if (objectName) {
            if (objectType == "DataSource" || objectType == "BusinessObject") {
                dataSourcesTree.build(true);
                var dataSourceItem = dataSourcesTree.getItem(objectName, null, "DataSource");
                if (!dataSourceItem) {
                    var fullName = objectName;
                    if (fullName && fullName != "StiEmptyValue") {
                        var lastName = fullName.substring(fullName.lastIndexOf(".") + 1);
                        dataSourceItem = dataSourcesTree.getItem(lastName, null, "BusinessObject");
                    }
                }
                if (dataSourceItem) {
                    dataSourceItem.setSelected();
                    dataSourceItem.openTree();
                    dataSourcesTree.update();
                }
            }
            else if (objectType == "Relation") {
                var dataSource = jsObject.GetDataSourceByNameFromDictionary(dataSourcesTree.selectedItem ? dataSourcesTree.selectedItem.itemObject.name : "");
                relationsTree.build(dataSource);
                var relationItem = relationsTree.getItem(objectName, "nameInSource");
                if (relationItem) relationItem.setSelected();
            }
        }
    }

    dataForm.updateMarkers = function () {
        this.mainButtons["DataSource"].marker.style.display = dataSourcesTree.selectedItem && dataSourcesTree.selectedItem.itemObject.typeItem == "DataSource" ? "" : "none";
        this.mainButtons["Relation"].marker.style.display = relationsTree.selectedItem && relationsTree.selectedItem.itemObject.typeItem == "Relation" ? "" : "none";
        this.mainButtons["MasterComponent"].marker.style.display = masterComponentsTree.selectedItem && masterComponentsTree.selectedItem.itemObject.typeItem != "NoItem" ? "" : "none";
        this.mainButtons["Sort"].marker.style.display = sortControl.sortContainer.getCountItems() > 0 ? "" : "none";
        this.mainButtons["Filter"].marker.style.display = filterControl.controls.filterContainer.getCountItems() > 0 ? "" : "none";
    }

    dataForm.onshow = function () {
        var currentObject = jsObject.options.selectedObject || jsObject.GetCommonObject(jsObject.options.selectedObjects);
        if (!currentObject) return;

        newBusinessObjectButton.style.display = jsObject.options.jsMode || jsObject.options.designerSpecification != "Developer" ? "none" : "";

        //DataSource & BusinessObject
        dataForm.setMode("DataSource");
        dataSourcesTree.build(true);
        jsObject.options.propertiesPanel.setDictionaryMode(true);

        countData.value = currentObject.typeComponent && currentObject.properties.countData != "StiEmptyValue" ? currentObject.properties.countData : "0";

        var dataSourceItem = currentObject.properties.dataSource != "StiEmptyValue" ? dataSourcesTree.getItem(currentObject.properties.dataSource, null, "DataSource") : null;
        if (!dataSourceItem) {
            var fullName = currentObject.properties.businessObject;
            if (fullName && fullName != "StiEmptyValue") {
                dataSourceItem = dataSourcesTree.getBusinessObjectItemByFullName(fullName);
            }
        }
        if (dataSourceItem) {
            dataSourceItem.setSelected();
            dataSourceItem.openTree();
        }
        dataSourcesTree.update();

        //Relation
        relationsTree.build(dataSourceItem ? dataSourceItem.itemObject : null);
        var relationItem = currentObject.properties.dataRelation != "StiEmptyValue" ? relationsTree.getItem(currentObject.properties.dataRelation, "nameInSource") : null;
        if (relationItem) relationItem.setSelected();

        //MasterComponent
        masterComponentsTree.build();
        var masterComponentItem = currentObject.properties.masterComponent != "StiEmptyValue" ? masterComponentsTree.getItem(currentObject.properties.masterComponent) : null;
        if (masterComponentItem) masterComponentItem.setSelected();

        var currentDataSourceName = dataForm.getCurrentDataSourceName();

        //Sort
        var sorts = currentObject.properties.sortData != "" && currentObject.properties.sortData != "StiEmptyValue" ? JSON.parse(StiBase64.decode(currentObject.properties.sortData)) : [];
        sortControl.currentDataSourceName = currentDataSourceName;
        sortControl.fill(sorts);

        //Filter
        var filters = currentObject.properties.filterData != "StiEmptyValue" && currentObject.properties.filterData != "" ? JSON.parse(StiBase64.decode(currentObject.properties.filterData)) : [];
        filterControl.currentDataSourceName = currentDataSourceName;
        filterControl.fill(
            filters,
            currentObject.properties.filterOn != "StiEmptyValue" ? currentObject.properties.filterOn : true,
            currentObject.properties.filterMode != "StiEmptyValue" ? currentObject.properties.filterMode : "And"
        );

        this.updateMarkers();
        this.markerTimer = setInterval(function () {
            dataForm.updateMarkers();
        }, 250)
    }

    dataForm.action = function () {
        this.changeVisibleState(false);
        var selectedObjects = jsObject.options.selectedObjects || [jsObject.options.selectedObject];
        if (!selectedObjects) return;
        for (var i = 0; i < selectedObjects.length; i++) {
            var currentObject = selectedObjects[i];
            currentObject.properties.dataSource = dataSourcesTree.selectedItem && dataSourcesTree.selectedItem.itemObject.typeItem == "DataSource" ? dataSourcesTree.selectedItem.itemObject.name : "[Not Assigned]";
            currentObject.properties.businessObject = dataSourcesTree.selectedItem && dataSourcesTree.selectedItem.itemObject.typeItem == "BusinessObject" ? dataSourcesTree.selectedItem.getBusinessObjectStringFullName() : "[Not Assigned]";
            currentObject.properties.dataRelation = relationsTree.selectedItem && relationsTree.selectedItem.itemObject.typeItem == "Relation" ? relationsTree.selectedItem.itemObject.nameInSource : "[Not Assigned]";
            currentObject.properties.masterComponent = masterComponentsTree.selectedItem && masterComponentsTree.selectedItem.itemObject.typeItem != "NoItem" ? masterComponentsTree.selectedItem.itemObject.name : "[Not Assigned]";
            currentObject.properties.countData = countData.value;

            var filterResult = filterControl.getValue();
            currentObject.properties.filterOn = filterResult.filterOn;
            currentObject.properties.filterMode = filterResult.filterMode;
            currentObject.properties.filterData = StiBase64.encode(JSON.stringify(filterResult.filters));

            var sortResult = sortControl.getValue();
            currentObject.properties.sortData = sortResult.length == 0 ? "" : StiBase64.encode(JSON.stringify(sortResult));
        }

        jsObject.SendCommandSendProperties(selectedObjects, ["dataSource", "dataRelation", "filterData", "masterComponent", "businessObject",
            "countData", "filterOn", "filterMode", "sortData"]);
    }

    return dataForm;
}