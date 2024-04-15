
StiMobileDesigner.prototype.InitializeCrossTabForm_ = function () {
    var crossTabForm = this.BaseFormPanel("crossTabForm", " ", 1, this.HelpLinks["crosstabform"]);
    crossTabForm.container.style.paddingTop = "6px";
    crossTabForm.controls = {};

    //Tabs
    var tabs = [];
    tabs.push({ "name": "Data", "caption": this.loc.PropertyCategory.DataCategory });
    tabs.push({ "name": "CrossTab", "caption": this.loc.PropertyCategory.CrossTabCategory });
    tabs.push({ "name": "Styles", "caption": this.loc.PropertyMain.Styles });

    var tabbedPane = crossTabForm.tabbedPane = this.TabbedPane("crossTabFormTabbedPane", tabs, "stiDesignerStandartTab");
    tabbedPane.tabsPanel.style.marginLeft = "12px";
    crossTabForm.container.appendChild(tabbedPane);

    for (var i = 0; i < tabs.length; i++) {
        var tabsPanel = tabbedPane.tabsPanels[tabs[i].name];
        tabsPanel.style.width = "750px";
        tabsPanel.style.height = "500px";

        switch (tabs[i].name) {
            case "Data": this.InitializeCrossTabFormDataPanel(crossTabForm, tabsPanel); break;
            case "CrossTab": this.InitializeCrossTabFormCrossTabPanel(crossTabForm, tabsPanel); break;
            case "Styles": this.InitializeCrossTabFormStylesPanel(crossTabForm, tabsPanel); break;
        }
    }

    crossTabForm.updateMarkers = function () {
        var dataPanel = crossTabForm.tabbedPane.tabsPanels.Data;
        dataPanel.mainButtons["DataSource"].marker.style.display = this.controls.dataSourcesTree.selectedItem && this.controls.dataSourcesTree.selectedItem.itemObject.typeItem == "DataSource" ? "" : "none";
        dataPanel.mainButtons["Relation"].marker.style.display = this.controls.relationsTree.selectedItem && this.controls.relationsTree.selectedItem.itemObject.typeItem == "Relation" ? "" : "none";
        dataPanel.mainButtons["Sort"].marker.style.display = this.controls.sortControl.sortContainer.getCountItems() > 0 ? "" : "none";
        dataPanel.mainButtons["Filter"].marker.style.display = this.controls.filterControl.controls.filterContainer.getCountItems() > 0 ? "" : "none";
    }

    crossTabForm.onshow = function () {
        var selectedObject = this.jsObject.options.selectedObject;
        if (!selectedObject) this.changeVisibleState(false);
        this.selectedObject = selectedObject;

        //Show CrossTab Fields Properties
        this.jsObject.options.propertiesPanel.setEditCrossTabMode(true);

        //Data Panel
        if (selectedObject.properties.dataSource == "[Not Assigned]") {
            tabbedPane.showTabPanel("Data");
            tabbedPane.tabsPanels.Data.showPanel("DataSource");
        }
        else {
            tabbedPane.showTabPanel("CrossTab");
        }

        //DataSource & BusinessObject        
        this.controls.dataSourcesTree.build(true);

        var dataSourceItem = this.controls.dataSourcesTree.getItem(selectedObject.properties.dataSource, null, "DataSource");
        if (!dataSourceItem) {
            var fullName = selectedObject.properties.businessObject;
            if (fullName && fullName != "StiEmptyValue") {
                var lastName = fullName.substring(fullName.lastIndexOf(".") + 1);
                dataSourceItem = this.controls.dataSourcesTree.getItem(lastName, null, "BusinessObject");
            }
        }
        if (dataSourceItem) {
            dataSourceItem.setSelected();
            dataSourceItem.openTree();
        }

        //Relation
        this.controls.relationsTree.build(dataSourceItem ? dataSourceItem.itemObject : null);
        var relationItem = this.controls.relationsTree.getItem(selectedObject.properties.dataRelation, "nameInSource");
        if (relationItem) relationItem.setSelected();

        //Sort
        var sorts = selectedObject.properties.sortData != "" ? JSON.parse(StiBase64.decode(selectedObject.properties.sortData)) : [];
        this.controls.sortControl.currentDataSourceName = this.controls.dataSourcesTree.getCurrentDataSourceName();
        this.controls.sortControl.fill(sorts);

        //Filter
        this.controls.filterEngine.setKey(selectedObject.properties.filterEngine);
        var filters = selectedObject.properties.filterData != "" ? JSON.parse(StiBase64.decode(selectedObject.properties.filterData)) : [];
        this.controls.filterControl.currentDataSourceName = this.controls.dataSourcesTree.getCurrentDataSourceName();
        this.controls.filterControl.fill(filters, selectedObject.properties.filterOn, selectedObject.properties.filterMode);

        //CrossTab Panel
        tabbedPane.tabsPanels.CrossTab.rebuildColumnsTree();
        tabbedPane.tabsPanels.CrossTab.updateContainers(selectedObject.properties.crossTabFields);
        crossTabForm.controls.resultContainer.update(selectedObject.properties.crossTabFields.components);

        //Summary Direction
        StiMobileDesigner.setImageSource(crossTabForm.controls.summaryDirectionButton.image, this.jsObject.options, "CrossTab." + selectedObject.properties.summaryDirection + "Direction.png");
        crossTabForm.controls.summaryContainer.onAction();

        StiMobileDesigner.setImageSource(crossTabForm.controls.summaryDirectionButton.image, this.jsObject.options, "CrossTab." + selectedObject.properties.summaryDirection + "Direction.png");

        this.controls.stylesContainer.crossTabStyleIndex = selectedObject.properties.crossTabFields.crossTabStyleIndex;
        this.controls.stylesContainer.crossTabStyle = selectedObject.properties.crossTabFields.crossTabStyle;

        this.updateMarkers();
        this.markerTimer = setInterval(function () {
            crossTabForm.updateMarkers();
        }, 250)
    }

    crossTabForm.onhide = function () {
        this.jsObject.options.propertiesPanel.setEditCrossTabMode(false);
        clearTimeout(this.markerTimer);
        this.jsObject.DeleteTemporaryMenus();
    }

    crossTabForm.action = function () {
        this.changeVisibleState(false);
        var properties = this.selectedObject.properties;

        properties.dataSource = this.controls.dataSourcesTree.selectedItem && this.controls.dataSourcesTree.selectedItem.itemObject.typeItem == "DataSource"
            ? this.controls.dataSourcesTree.selectedItem.itemObject.name : "[Not Assigned]";
        properties.businessObject = this.controls.dataSourcesTree.selectedItem && this.controls.dataSourcesTree.selectedItem.itemObject.typeItem == "BusinessObject"
            ? this.controls.dataSourcesTree.selectedItem.getBusinessObjectStringFullName() : "[Not Assigned]";
        properties.dataRelation = this.controls.relationsTree.selectedItem && this.controls.relationsTree.selectedItem.itemObject.typeItem == "Relation"
            ? this.controls.relationsTree.selectedItem.itemObject.nameInSource : "[Not Assigned]";
        properties.filterEngine = this.controls.filterEngine.key;

        var filterResult = this.controls.filterControl.getValue();
        properties.filterOn = filterResult.filterOn;
        properties.filterMode = filterResult.filterMode;
        properties.filterData = StiBase64.encode(JSON.stringify(filterResult.filters));

        var sortResult = this.controls.sortControl.getValue();
        properties.sortData = sortResult.length == 0 ? "" : StiBase64.encode(JSON.stringify(sortResult));

        this.jsObject.SendCommandSendProperties(this.selectedObject, ["dataSource", "dataRelation", "filterData", "masterComponent", "businessObject",
            "countData", "filterEngine", "filterOn", "filterMode", "sortData"]);
    }

    crossTabForm.cancelAction = function () {
        this.jsObject.SendCommandCanceledEditComponent(this.selectedObject.properties.name);
    }

    crossTabForm.sendCommand = function (parameters) {
        this.jsObject.SendCommandUpdateCrossTabComponent(this.selectedObject.properties.name, parameters);
    }

    crossTabForm.recieveCommandResult = function (result) {

        switch (result.command) {
            case "InsertItemToContainer":
            case "RemoveItemFromContainer":
            case "SwapColumnsAndRows":
            case "ItemMoveDown":
            case "ItemMoveUp":
                {
                    tabbedPane.tabsPanels.CrossTab.updateContainers(result.fieldsProperties, result.containerName, result.selectedIndex);
                    crossTabForm.controls.resultContainer.update(result.fieldsProperties.components);
                    tabbedPane.tabsPanels.CrossTab.updateProperties();
                    break;
                }
            case "UpdateProperty":
            case "SetStyle":
                {
                    if (result.command == "SetStyle") {
                        crossTabForm.controls.stylesContainer.crossTabStyleIndex = result.fieldsProperties.crossTabStyleIndex;
                        crossTabForm.controls.stylesContainer.crossTabStyle = result.fieldsProperties.crossTabStyle;
                    }
                    crossTabForm.controls.resultContainer.update(result.fieldsProperties.components, result.selectedComponentName);
                    break;
                }
            case "ChangeSummaryDirection":
                {
                    crossTabForm.controls.resultContainer.update(result.fieldsProperties.components);
                    tabbedPane.tabsPanels.CrossTab.updateProperties();
                    break;
                }
        }
    }

    return crossTabForm;
}

//Data
StiMobileDesigner.prototype.InitializeCrossTabFormDataPanel = function (crossTabForm, parentPanel) {

    //Main Table
    var mainTable = this.CreateHTMLTable();
    mainTable.className = "stiDesignerImageFormMainPanel";
    parentPanel.appendChild(mainTable);
    mainTable.style.width = "100%";

    var buttonProps = [
        ["DataSource", "CrossTab.CrossTabDataSource.png", this.loc.PropertyMain.DataSource],
        ["Relation", "CrossTab.CrossTabDataRelation.png", this.loc.PropertyMain.DataRelation],
        ["Sort", "CrossTab.CrossTabSort.png", this.loc.PropertyMain.Sort],
        ["Filter", "CrossTab.CrossTabFilters.png", this.loc.PropertyMain.Filters]
    ];

    //Add Panels && Buttons
    var buttonsPanel = mainTable.addCell();
    buttonsPanel.style.verticalAlign = "top";

    var panelsContainer = mainTable.addCell();
    panelsContainer.style.width = "100%";
    parentPanel.mainButtons = {};
    parentPanel.panels = {};

    for (var i = 0; i < buttonProps.length; i++) {
        var panel = document.createElement("div");
        panel.className = "stiDesignerEditFormPanel";
        panel.style.width = "100%";
        panel.style.height = "500px";
        if (i != 0) panel.style.display = "none";
        panelsContainer.appendChild(panel);
        parentPanel.panels[buttonProps[i][0]] = panel;
        panel.onShow = function () { };

        var button = this.FormTabPanelButton(null, buttonProps[i][2], buttonProps[i][1], buttonProps[i][2], null, { width: 24, height: 24 }, 34);
        parentPanel.mainButtons[buttonProps[i][0]] = button;
        buttonsPanel.appendChild(button);
        button.panelName = buttonProps[i][0];

        button.action = function () {
            parentPanel.showPanel(this.panelName);
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

    parentPanel.showPanel = function (selectedPanelName) {
        this.selectedPanelName = selectedPanelName;
        for (var panelName in this.panels) {
            this.panels[panelName].style.display = selectedPanelName == panelName ? "" : "none";
            this.mainButtons[panelName].setSelected(selectedPanelName == panelName);
            if (selectedPanelName == panelName) this.panels[panelName].onShow();
        }
    }

    //Data Source

    var dataSourcesTree = crossTabForm.controls.dataSourcesTree = this.DataSourcesTree(null, 480);
    dataSourcesTree.className = "stiSimpleContainerWithBorder";
    dataSourcesTree.style.margin = "6px 14px 0 12px";
    dataSourcesTree.style.overflow = "auto";
    dataSourcesTree.oneLevelBusinessObjects = true;
    parentPanel.panels.DataSource.appendChild(dataSourcesTree);

    dataSourcesTree.action = function () {
        crossTabForm.tabbedPane.showTabPanel("CrossTab");
    }

    dataSourcesTree.onActionItem = function () {
        crossTabForm.tabbedPane.tabsPanels.CrossTab.rebuildColumnsTree();
        crossTabForm.sendCommand({
            command: "ChangedDataSource",
            dataSourceName: dataSourcesTree.getCurrentDataSourceName(),
            dataSourceType: dataSourcesTree.selectedItem ? dataSourcesTree.selectedItem.itemObject.typeItem : ""
        });
    }

    dataSourcesTree.sinchronizeWithMainDictionary = function () {
        if (this.jsObject.options.dictionaryTree && this.selectedItem && this.selectedItem.itemObject.typeItem && this.selectedItem.itemObject.name) {
            var item = this.jsObject.options.dictionaryTree.getItemByNameAndType(this.selectedItem.itemObject.name, this.selectedItem.itemObject.typeItem);
            if (item) item.setSelected();
        }
    }

    dataSourcesTree.getCurrentDataSourceName = function () {
        var dataSourceName = this.selectedItem &&
            (this.selectedItem.itemObject.typeItem == "DataSource" || this.selectedItem.itemObject.typeItem == "BusinessObject")
            ? this.selectedItem.itemObject.name : null;
        return dataSourceName;
    }

    //Relation
    var relationsTree = this.RelationsTree(null, 480);
    relationsTree.className = "stiSimpleContainerWithBorder";
    relationsTree.style.margin = "6px 14px 0 12px";
    relationsTree.style.overflow = "auto";
    crossTabForm.controls.relationsTree = relationsTree;
    parentPanel.panels.Relation.appendChild(relationsTree);

    parentPanel.panels.Relation.onShow = function () {
        var dataSource = crossTabForm.jsObject.GetDataSourceByNameFromDictionary(dataSourcesTree.selectedItem ? dataSourcesTree.selectedItem.itemObject.name : "");
        var currRelationNameInSource = relationsTree.selectedItem ? relationsTree.selectedItem.itemObject.nameInSource : null;
        relationsTree.build(dataSource);
        if (currRelationNameInSource) {
            var relationItem = relationsTree.getItem(currRelationNameInSource, "nameInSource");
            if (relationItem) relationItem.setSelected();
        }
    }

    //Sort
    var sortControl = crossTabForm.controls.sortControl = this.SortControl("crossTabFormSortControl" + this.generateKey(), null, null, 442);
    parentPanel.panels.Sort.appendChild(sortControl);

    sortControl.toolBar.style.margin = "6px 0 0 12px";

    var sortContainer = sortControl.sortContainer
    sortContainer.className = "stiSimpleContainerWithBorder";
    sortContainer.style.margin = "12px 12px 0 12px";
    sortContainer.style.overflow = "auto";

    parentPanel.panels.Sort.onShow = function () {
        var currentDataSourceName = dataSourcesTree.getCurrentDataSourceName();
        if (sortControl.currentDataSourceName != currentDataSourceName) sortControl.sortContainer.clear();
        sortControl.currentDataSourceName = currentDataSourceName;
    }

    //Filter
    var filterControl = crossTabForm.controls.filterControl = this.FilterControl("crossTabFormFilterControl" + this.generateKey(), null, null, 400);
    parentPanel.panels.Filter.appendChild(filterControl);

    filterControl.controls.toolBar.style.margin = "6px 0 0 12px";
    filterControl.controls.toolBar.style.width = "calc(100% - 24px)";

    var filterContainer = filterControl.controls.filterContainer;
    filterContainer.className = "stiSimpleContainerWithBorder";
    filterContainer.style.margin = "12px 12px 0 12px";
    filterContainer.style.overflow = "auto";

    parentPanel.panels.Filter.onShow = function () {
        var currentDataSourceName = dataSourcesTree.getCurrentDataSourceName();
        if (filterControl.currentDataSourceName != currentDataSourceName) filterControl.controls.filterContainer.clear();
        filterControl.currentDataSourceName = currentDataSourceName;
    }

    var filterTable = this.CreateHTMLTable();
    parentPanel.panels.Filter.appendChild(filterTable);

    var filterEngine = crossTabForm.controls.filterEngine = this.DropDownList(null, 125, this.loc.PropertyMain.FilterEngine, this.GetFilterIngineItems(), true, false);
    filterTable.addTextCell(this.loc.PropertyMain.FilterEngine).style.paddingLeft = "12px";
    filterTable.addCell(filterEngine).style.padding = "12px";

    parentPanel.rebuildTrees = function (objectName, objectType) {
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
                }
            }
            else if (objectType == "Relation") {
                var dataSource = crossTabForm.jsObject.GetDataSourceByNameFromDictionary(dataSourcesTree.selectedItem ? dataSourcesTree.selectedItem.itemObject.name : "");
                relationsTree.build(dataSource);
                var relationItem = relationsTree.getItem(objectName, "nameInSource");
                if (relationItem) relationItem.setSelected();
            }
        }
    }
}

//CrossTab
StiMobileDesigner.prototype.InitializeCrossTabFormCrossTabPanel = function (crossTabForm, parentPanel) {
    var mainTable = this.CreateHTMLTable();
    parentPanel.appendChild(mainTable);
    parentPanel.selectedContainerItem = null;

    //ColumnsTree
    var columnsTree = this.DataTree();
    crossTabForm.controls.columnsTree = columnsTree;
    var columnsTreeContainer = document.createElement("div");
    columnsTreeContainer.appendChild(columnsTree);
    mainTable.addCell(columnsTreeContainer).className = "stiDesignerStyleDesignerFormToolbarCell";
    columnsTreeContainer.style.width = "250px";
    columnsTreeContainer.style.height = "300px";
    columnsTreeContainer.style.overflow = "auto";

    //Columns Containers
    var rightTable = this.CreateHTMLTable();
    mainTable.addCell(rightTable);

    var columnsContainer = this.CrossTabContainer(crossTabForm, "columns", 249, 150, this.loc.FormCrossTabDesigner.Columns.replace(":", ""));
    var rowsContainer = this.CrossTabContainer(crossTabForm, "rows", 249, 150, this.loc.FormCrossTabDesigner.Rows.replace(":", ""));
    var summaryContainer = this.CrossTabContainer(crossTabForm, "summary", 249, 150, this.loc.FormCrossTabDesigner.Summary.replace(":", ""));
    crossTabForm.controls.columnsContainer = columnsContainer;
    crossTabForm.controls.rowsContainer = rowsContainer;
    crossTabForm.controls.summaryContainer = summaryContainer;
    columnsContainer.onAction = function () { parentPanel.onActionContainers(this); }
    rowsContainer.onAction = function () { parentPanel.onActionContainers(this); }

    summaryContainer.onAction = function () {
        if (crossTabForm.controls.summaryDirectionButton) {
            crossTabForm.controls.summaryDirectionButton.style.display = this.getCountItems() > 1 ? "" : "none";
        }
        parentPanel.onActionContainers(this);
    }

    parentPanel.onActionContainers = function (aciveContainer) {
        this.selectedContainerItem = aciveContainer.selectedItem;
        if (columnsContainer != aciveContainer) columnsContainer.setNotActive();
        if (rowsContainer != aciveContainer) rowsContainer.setNotActive();
        if (summaryContainer != aciveContainer) summaryContainer.setNotActive();
        this.updateProperties();
    }

    parentPanel.updateProperties = function () {
        var crossTabPropPanel = crossTabForm.jsObject.options.propertiesPanel.editCrossTabPropertiesPanel;

        if (this.selectedContainerItem && crossTabForm.controls.resultPage) {
            for (var i = 0; i < crossTabForm.controls.resultPage.childNodes.length; i++) {
                var component = crossTabForm.controls.resultPage.childNodes[i];
                if (component.properties && component.properties.name == this.selectedContainerItem.itemObject.name) {
                    component.action();
                }
            }
        }
        else {
            if (crossTabForm.jsObject.options.selectedCrossTabField) crossTabForm.jsObject.options.selectedCrossTabField.setSelected(false);
            crossTabPropPanel.updateProperties(null);
        }
    }

    var rotateButton = this.CrossTabContainerToolButton(null, "CrossTab.Rotate.png", this.loc.FormCrossTabDesigner.Swap, 23);
    rotateButton.style.margin = "4px";
    rotateButton.style.display = "inline-block";
    rotateButton.action = function () {
        crossTabForm.sendCommand({
            command: "SwapColumnsAndRows"
        });
    }

    var emptyCell = rightTable.addCell(rotateButton)
    emptyCell.className = "stiDesignerCrossTabContainerCell";
    emptyCell.style.verticalAlign = "bottom";
    emptyCell.style.textAlign = "right";
    rightTable.addCell(columnsContainer).className = "stiDesignerCrossTabContainerCell";
    rightTable.addCellInNextRow(rowsContainer).className = "stiDesignerCrossTabContainerCell";
    rightTable.addCellInLastRow(summaryContainer).className = "stiDesignerCrossTabContainerCell";

    //Result Container
    var resultContainer = document.createElement("div");
    resultContainer.style.padding = "10px";
    resultContainer.style.overflow = "auto";
    resultContainer.style.position = "relative";
    crossTabForm.controls.resultContainer = resultContainer;

    //Summary Direction
    var summaryDirection = this.options.selectedObject ? this.options.selectedObject.properties.summaryDirection : "LeftToRight";
    var sumDirButton = this.StandartSmallButton(null, null, null, "CrossTab." + summaryDirection + "Direction.png");
    resultContainer.appendChild(sumDirButton);
    crossTabForm.controls.summaryDirectionButton = sumDirButton;

    if (sumDirButton.imageCell) sumDirButton.imageCell.style.padding = "2px";
    sumDirButton.style.display = "none";
    sumDirButton.style.top = "10px";
    sumDirButton.style.right = "10px";
    sumDirButton.style.width = "28px";
    sumDirButton.style.height = "28px";
    sumDirButton.style.position = "absolute";
    sumDirButton.style.display = "inline-block";

    sumDirButton.action = function () {
        var selectedObject = this.jsObject.options.selectedObject;
        if (!selectedObject) return;
        selectedObject.properties.summaryDirection = selectedObject.properties.summaryDirection == "LeftToRight" ? "UpToDown" : "LeftToRight";
        StiMobileDesigner.setImageSource(sumDirButton.image, this.jsObject.options, "CrossTab." + selectedObject.properties.summaryDirection + "Direction.png");

        crossTabForm.sendCommand({
            command: "ChangeSummaryDirection",
            summaryDirection: selectedObject.properties.summaryDirection
        });
    }

    parentPanel.resultCell = mainTable.addCellInNextRow(resultContainer);
    parentPanel.resultCell.setAttribute("colspan", "2");

    parentPanel.onshow = function () {
        parentPanel.resultCell.appendChild(resultContainer);
        resultContainer.style.width = "730px";
        resultContainer.style.height = "176px";
    }

    resultContainer.update = function (compObjects, selectedComponentName) {
        //update svg
        if (crossTabForm.controls.resultPage) resultContainer.removeChild(crossTabForm.controls.resultPage);
        var resultPage = crossTabForm.jsObject.CreateCrossTabResultPage();
        crossTabForm.controls.resultPage = resultPage;
        resultContainer.appendChild(resultPage);

        if (!compObjects) return;

        //paint components
        var oldZoom = crossTabForm.jsObject.options.report.zoom;
        crossTabForm.jsObject.options.report.zoom = 1;

        for (var i = 0; i < compObjects.length; i++) {
            var newComponent = crossTabForm.jsObject.CreateCrossTabFieldComponent(compObjects[i], true);
            newComponent.repaint();
            resultPage.appendChild(newComponent);
            newComponent.parentContainer = resultPage;

            if (selectedComponentName != null && newComponent.properties.name == selectedComponentName) {
                newComponent.action();
            }
        }

        crossTabForm.jsObject.options.report.zoom = oldZoom;

        try {
            var bbox = resultPage.getBBox();
            if (bbox) {
                resultPage.style.padding = "5px";
                resultPage.setAttribute("width", parseInt(bbox.x + bbox.width) + 5);
                resultPage.setAttribute("height", parseInt(bbox.y + bbox.height) + 5);
            }
        }
        catch (e) {
        }
    }

    parentPanel.updateContainers = function (fieldsProperties, activeContainerName, selectedIndex) {
        columnsContainer.fill(fieldsProperties.columns);
        rowsContainer.fill(fieldsProperties.rows);
        summaryContainer.fill(fieldsProperties.summary);

        var currContainer = crossTabForm.controls[activeContainerName + "Container"];
        if (currContainer && selectedIndex != null && selectedIndex < currContainer.getCountItems()) {
            var item = currContainer.getItemByIndex(selectedIndex);
            if (item) item.action();
        }
    }

    parentPanel.rebuildColumnsTree = function () {
        var currentDataSourceName = crossTabForm.controls.dataSourcesTree.getCurrentDataSourceName();

        if (currentDataSourceName) {
            columnsTree.build("Column", currentDataSourceName, true);
            if (columnsTree.mainItem) {
                for (var key in columnsTree.mainItem.childs) {
                    var itemObject = columnsTree.mainItem.childs[key].itemObject;
                    if (!itemObject) continue;
                    if (itemObject.typeItem == "NoItem") columnsTree.mainItem.childs[key].style.display = "none";
                    if (itemObject.typeItem == "DataSource") columnsTree.mainItem.childs[key].setOpening(true);
                }
            }
        }
        else {
            columnsTree.clear();
        }

        columnsContainer.clear();
        rowsContainer.clear();
        summaryContainer.clear();
        parentPanel.selectedContainerItem = null;
        resultContainer.update();
        parentPanel.updateProperties();
    }
}

//Styles
StiMobileDesigner.prototype.InitializeCrossTabFormStylesPanel = function (crossTabForm, parentPanel) {
    var mainTable = this.CreateHTMLTable();
    parentPanel.appendChild(mainTable);

    //Styles Container
    var stylesContainer = this.CrossTabStylesContainer();
    stylesContainer.style.margin = "6px 0 0 12px";
    crossTabForm.controls.stylesContainer = stylesContainer;

    var containerCell = mainTable.addCellInNextRow();
    containerCell.style.width = "1px";
    containerCell.style.verticalAlign = "top";
    this.AddProgressToControl(containerCell);
    containerCell.appendChild(stylesContainer);

    stylesContainer.fill = function (colorStyles) {
        containerCell.progress.hide();

        var userStyles = crossTabForm.jsObject.GetUserCrossTabStyles();
        var styles = userStyles.concat(colorStyles);

        for (var i = 0; i < styles.length; i++) {
            var button = this.addItemAndNotAction(styles[i].properties.name, styles[i].properties.name, null, styles[i]);
            button.style.margin = "3px";
            this.buttons[styles.name] = button;
            var sampleTable = crossTabForm.jsObject.CrossTabSampleTable(70, 70, 7, 7, styles[i].properties.columnHeaderBackColor, styles[i].properties.rowHeaderBackColor);
            sampleTable.style.margin = "5px 5px 3px 5px";
            button.insertBefore(sampleTable, button.innerTable);
            button.innerTable.removeChild(button.innerTable.tr[0]);
            button.innerTable.style.marginBottom = "5px";
            button.innerTable.style.height = "auto";
            button.style.display = "inline-block";

            if (button.caption) {
                button.caption.style.padding = "0px";
                button.caption.style.height = "25px";
            }

            if ((this.crossTabStyle == "" && styles[i].indexColorStyles == this.crossTabStyleIndex) ||
                (this.crossTabStyle != "" && styles[i].properties.name == this.crossTabStyle)) {
                button.selected();
            }
        }
    }

    stylesContainer.onAction = function () {
        if (this.selectedItem != null) {
            var params = {
                command: "SetStyle",
                styleName: this.selectedItem.itemObject.properties.name,
                color: this.selectedItem.itemObject.properties.color,
                indexColorStyles: this.selectedItem.itemObject.indexColorStyles
            }

            if (crossTabForm.jsObject.options.propertiesPanel.editCrossTabPropertiesPanel.propertiesValues) {
                params.selectedComponentName = crossTabForm.jsObject.options.propertiesPanel.editCrossTabPropertiesPanel.propertiesValues.name
            }

            crossTabForm.sendCommand(params);
        }
    }

    stylesContainer.update = function () {
        this.clear();
        containerCell.progress.show();
        crossTabForm.jsObject.SendCommandGetCrossTabColorStyles();
    }

    //result container
    parentPanel.resultCell = mainTable.addCellInLastRow();
    parentPanel.resultCell.style.verticalAlign = "top";

    parentPanel.onshow = function () {
        parentPanel.resultCell.appendChild(crossTabForm.controls.resultContainer);
        crossTabForm.controls.resultContainer.style.width = "510px";
        crossTabForm.controls.resultContainer.style.height = "490px";
        stylesContainer.update();
    }

    return mainTable;
}

StiMobileDesigner.prototype.CrossTabContainerToolButton = function (caption, imageName, toolTip, height) {
    var button = this.StandartSmallButton(null, null, caption, imageName, toolTip, null, true);
    if (button.imageCell) button.imageCell.style.padding = "0 3px 0 3px";
    button.style.height = height + "px";

    return button;
}

StiMobileDesigner.prototype.CrossTabContainer = function (crossTabForm, name, width, height, headerCaption) {
    var container = document.createElement("div");
    container.jsObject = this;
    container.selectedItem = null;
    container.onAction = function () { };
    container.name = name;

    //Toolbar
    var toolBar = this.CreateHTMLTable();
    toolBar.className = "stiDesignerrCrossTabContainerToolbar";
    container.appendChild(toolBar);
    var toolBarHeight = 23;

    var moveUpButton = this.CrossTabContainerToolButton(null, "Arrows.ArrowUpBlue.png", this.loc.QueryBuilder.MoveUp, toolBarHeight);
    var moveDownButton = this.CrossTabContainerToolButton(null, "Arrows.ArrowDownBlue.png", this.loc.QueryBuilder.MoveDown, toolBarHeight);
    var removeButton = this.CrossTabContainerToolButton(null, "Remove.png", this.loc.Buttons.Remove, toolBarHeight);
    toolBar.addTextCell(headerCaption).style.paddingLeft = "6px";
    toolBar.addCell().style.width = "100%";
    toolBar.addCell(moveUpButton);
    toolBar.addCell(moveDownButton);
    toolBar.addCell(removeButton);
    moveUpButton.setEnabled(false);
    moveDownButton.setEnabled(false);
    removeButton.setEnabled(false);

    var moveItem = function (direction) {
        if (container.selectedItem) {
            var params = {
                command: "ItemMove" + direction,
                indexForMoving: container.selectedItem.getIndex(),
                containerName: container.name
            }
            container.selectedItem.move(direction);
            params.selectedIndexAfterMoving = container.selectedItem.getIndex();
            crossTabForm.sendCommand(params);
        }
    }

    moveUpButton.action = function () {
        moveItem("Up");
    }

    moveDownButton.action = function () {
        moveItem("Down");
    }

    removeButton.action = function () {
        if (container.selectedItem) {
            var indexForRemove = container.selectedItem.getIndex();
            container.selectedItem.remove();

            crossTabForm.sendCommand({
                command: "RemoveItemFromContainer",
                indexForRemove: indexForRemove,
                containerName: container.name,
                selectIndexAfterRemoved: container.selectedItem ? container.selectedItem.getIndex() : null
            });
        }
    }

    toolBar.updateControls = function () {
        var count = container.getCountItems();
        var index = container.selectedItem ? container.selectedItem.getIndex() : -1;
        moveUpButton.setEnabled(index > 0);
        moveDownButton.setEnabled(index != -1 && index < count - 1);
        removeButton.setEnabled(count > 0 && index != -1);
    }

    //Container
    var innerContainer = document.createElement("div");
    innerContainer.style.width = width + "px";
    innerContainer.style.height = (height - toolBarHeight) + "px";
    innerContainer.style.overflow = "auto";
    container.innerContainer = innerContainer;
    container.appendChild(innerContainer);

    container.clear = function (runActionEvent) {
        while (innerContainer.childNodes[0]) innerContainer.removeChild(innerContainer.childNodes[0]);
        this.selectedItem = null;
        toolBar.updateControls();
        if (runActionEvent) this.onAction();
    }

    container.addItem = function (itemObject, runActionEvent, insertIndex) {
        var item = this.jsObject.StandartSmallButton(null, null, itemObject.alias, null);
        item.name = itemObject.name;
        item.itemObject = itemObject;

        if (insertIndex != null && insertIndex < innerContainer.childNodes.length)
            innerContainer.insertBefore(item, innerContainer.childNodes[insertIndex]);
        else
            innerContainer.appendChild(item);

        toolBar.updateControls();
        if (runActionEvent) container.onAction();

        item.select = function () {
            if (container.selectedItem) container.selectedItem.setSelected(false);
            this.setSelected(true);
            container.selectedItem = this;
            toolBar.updateControls();
        }

        item.action = function () {
            this.select();
            container.onAction();
        }

        item.remove = function () {
            innerContainer.removeChild(this);
            if (this == container.selectedItem) {
                container.selectedItem = null;
                var count = container.getCountItems();
                if (count > 0) {
                    innerContainer.childNodes[0].select();
                }
            }
            toolBar.updateControls();
            container.onAction();
        };

        item.getIndex = function () {
            for (var i = 0; i < innerContainer.childNodes.length; i++)
                if (innerContainer.childNodes[i] == this) return i;
        };

        item.move = function (direction) {
            var index = this.getIndex();
            innerContainer.removeChild(this);
            var count = container.getCountItems();
            var newIndex = direction == "Up" ? index - 1 : index + 1;
            if (direction == "Up" && newIndex == -1) newIndex = 0;
            if (direction == "Down" && newIndex >= count) {
                innerContainer.appendChild(this);
                toolBar.updateControls();
                container.onAction();
                return;
            }
            innerContainer.insertBefore(this, innerContainer.childNodes[newIndex]);
            toolBar.updateControls();
            container.onAction();
        }

        item.ontouchstart = function (event, mouseProcess) {
            var this_ = this;
            this.isTouchStartFlag = mouseProcess ? false : true;
            clearTimeout(this.isTouchStartTimer);

            if (event) event.preventDefault();
            this.action();

            if (event.button != 2) {
                var itemInDragObject = this.jsObject.TreeItemForDragDrop(this.itemObject, null, true);
                itemInDragObject.container = container;
                itemInDragObject.originalItem = this;
                itemInDragObject.beginingOffset = 0;
                this.jsObject.options.itemInDrag = itemInDragObject;
            }

            this.isTouchStartTimer = setTimeout(function () {
                this_.isTouchStartFlag = false;
            }, 1000);
        }

        //Mouse Down
        item.onmousedown = function (event) {
            if (this.isTouchStartFlag) return;
            this.ontouchstart(event, true);
        }

        return item;
    }

    container.getCountItems = function () {
        return innerContainer.childNodes.length;
    }

    container.getItemByName = function (name) {
        for (var i = 0; i < innerContainer.childNodes.length; i++) {
            if (innerContainer.childNodes[i].name == name) return innerContainer.childNodes[i];
        }
        return null;
    }

    container.getItemByIndex = function (index) {
        if (index < innerContainer.childNodes.length) return innerContainer.childNodes[index];
        return null;
    }

    container.getIndexCurrentOverItem = function () {
        for (var i = 0; i < innerContainer.childNodes.length; i++) {
            if (innerContainer.childNodes[i].isOver) return i;
        }
        return null;
    }

    //Events
    container.onmouseup = function () { if (this.jsObject.options.itemInDrag) container.insertItem(this.jsObject.options.itemInDrag); }
    container.ontouchend = function () { this.onmouseup(); }

    container.insertItem = function (draggedItem) {
        if (draggedItem && draggedItem.container != this) {
            var insertIndex = null;
            for (var i = 0; i < innerContainer.childNodes.length; i++) {
                if (innerContainer.childNodes[i].isOver) {
                    if (i + 1 < innerContainer.childNodes.length) insertIndex = i + 1;
                    break;
                }
            }

            var itemObject = draggedItem.itemObject;

            if (!draggedItem.container) {
                itemObject = {
                    type: "ItemFromDataSourcesTree",
                    columnFullName: draggedItem.originalItem.getFullName(),
                    alias: draggedItem.itemObject.alias
                }
            }

            var params = {
                command: "InsertItemToContainer",
                destinationContainerName: this.name,
                destinationIndex: insertIndex,
                itemObject: itemObject,
                sourceContainerName: draggedItem.container ? draggedItem.container.name : "datasources"
            }

            if (draggedItem.container) {
                params.sourceIndex = draggedItem.originalItem.getIndex();
                draggedItem.originalItem.remove();
            }

            var newItem = container.addItem(itemObject, null, insertIndex);
            newItem.select();
            container.onAction();

            crossTabForm.sendCommand(params);
        }
    }

    container.setNotActive = function () {
        if (this.selectedItem) this.selectedItem.setSelected(false);
        this.selectedItem = null;
        moveUpButton.setEnabled(false);
        moveDownButton.setEnabled(false);
        removeButton.setEnabled(false);
    }

    container.fill = function (itemObjects) {
        this.clear();
        for (var i = 0; i < itemObjects.length; i++) {
            this.addItem(itemObjects[i]);
        }
    }

    return container;
}

StiMobileDesigner.prototype.CreateCrossTabResultPage = function () {
    var resultPage = ("createElementNS" in document) ? document.createElementNS("http://www.w3.org/2000/svg", "svg") : document.createElement("svg");

    return resultPage;
}

//Styles Container
StiMobileDesigner.prototype.CrossTabStylesContainer = function () {
    var stylesContainer = this.ContainerWithBigItems("crossTabFormStylesContainer", 200, this.options.isTouchDevice ? 462 : 467, 80);
    stylesContainer.className = "stiDesignerSeriesContainer";
    stylesContainer.buttons = {};

    return stylesContainer;
}