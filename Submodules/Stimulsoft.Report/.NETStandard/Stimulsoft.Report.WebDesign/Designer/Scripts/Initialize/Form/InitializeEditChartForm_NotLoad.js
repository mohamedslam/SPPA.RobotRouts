
StiMobileDesigner.prototype.InitializeEditChartForm_ = function () {
    //Edit Chart Form
    var jsObject = this;
    var editChartForm = this.BaseFormPanel("editChart", this.loc.Chart.ChartEditorForm, 1, this.HelpLinks["chartform"]);
    editChartForm.mode = "create";
    editChartForm.container.style.paddingTop = "6px";

    var lessOptionsButton = this.FormButton(null, null, this.loc.Buttons.LessOptions);
    lessOptionsButton.style.display = "inline-block";
    lessOptionsButton.style.margin = "12px";

    lessOptionsButton.action = function () {
        editChartForm.changeVisibleState(false);
        editChartForm.currentChartComponent.properties.editorType = "Simple";
        jsObject.SendCommandSendProperties([editChartForm.currentChartComponent], ["editorType"]);
        jsObject.InitializeEditChartSimpleForm(function (form) {
            form.currentChartComponent = editChartForm.currentChartComponent;
            form.chartProperties = editChartForm.chartProperties;
            form.oldSvgContent = editChartForm.oldSvgContent;
            form.changeVisibleState(true);
        });
    }

    var footerTable = this.CreateHTMLTable();
    footerTable.style.width = "100%";

    var buttonsPanel = editChartForm.buttonsPanel;
    editChartForm.removeChild(buttonsPanel);
    editChartForm.appendChild(footerTable);
    footerTable.addCell(lessOptionsButton).style.width = "1px";
    footerTable.addCell();
    footerTable.addCell(editChartForm.buttonOk).style.width = "1px";
    footerTable.addCell(editChartForm.buttonCancel).style.width = "1px";

    //Chart Image
    editChartForm.chartImage = document.createElement("div");
    editChartForm.chartImage.className = "stiDesignerChartImage";

    editChartForm.chartImage.update = function () {
        this.innerHTML = editChartForm.chartProperties.chartImage;
    }

    //Tabs
    var tabs = [];
    tabs.push({ "name": "Chart", "caption": this.loc.Components.StiChart });
    tabs.push({ "name": "Series", "caption": this.loc.Chart.Serieses });
    tabs.push({ "name": "Area", "caption": this.loc.Chart.Area });
    tabs.push({ "name": "Labels", "caption": this.loc.Chart.Labels });
    tabs.push({ "name": "Styles", "caption": this.loc.PropertyMain.Styles });

    var tabbedPane = this.TabbedPane("editChartTabbedPane", tabs, "stiDesignerStandartTab");
    editChartForm.tabbedPane = tabbedPane;
    editChartForm.container.appendChild(tabbedPane);

    tabbedPane.tabsPanel.style.marginLeft = "12px";

    for (var i = 0; i < tabs.length; i++) {
        var tabsPanel = null;

        switch (tabs[i].name) {
            case "Chart": tabsPanel = this.EditChartFormChartTabPanel(editChartForm); break;
            case "Series": tabsPanel = this.EditChartFormSeriesTabPanel(editChartForm); break;
            case "Area": tabsPanel = this.EditChartFormAreaTabPanel(editChartForm); break;
            case "Labels": tabsPanel = this.EditChartFormLabelsTabPanel(editChartForm); break;
            case "Styles": tabsPanel = this.EditChartFormStylesTabPanel(editChartForm); break;
        }

        if (tabsPanel) {
            tabsPanel.style.width = "750px";
            tabsPanel.style.height = "460px";
            tabbedPane.tabsPanels[tabs[i].name].appendChild(tabsPanel);
        }

        //Methods Tabs On Show Event
        tabbedPane.tabsPanels[tabs[i].name].onshow = function () {
            //Move Chart Image To Current Panel
            var imageContainer = editChartForm["imageContainer" + this.name + "Tab"];
            if (imageContainer) {
                imageContainer.appendChild(editChartForm.chartImage);
            }

            //Show Current Properties 
            editChartForm.jsObject.options.propertiesPanel.editChartPropertiesPanel.showInnerPanel(this.name);
            editChartForm.onChangeTabs();

            //On Show Events
            if (this.name == "Chart") {
                var seriesCount = editChartForm.seriesContainer.items.length;
                editChartForm.seriesWizardContainer.style.display = seriesCount == 0 ? "" : "none";
                editChartForm.chartTabMainTable.style.display = seriesCount == 0 ? "none" : "";
                editChartForm.chartPropertiesContainer.buttons.Common.action();
            }
            if (this.name == "Series") {
                editChartForm.seriesPropertiesContainer.update();
            }
            if (this.name == "Area") {
                editChartForm.areaPropertiesContainer.update();
            }
            if (this.name == "Labels") {
                var areaType = editChartForm.seriesContainer.selectedItem ? editChartForm.seriesContainer.selectedItem.itemObject.type : editChartForm.chartProperties.area.type;
                if (areaType != editChartForm.lastAreaTypeForLabels || editChartForm.chartProperties.style.type + editChartForm.chartProperties.style.name != editChartForm.lastStyleId) {
                    editChartForm.labelsContainer.labelsProgress.style.display = "";
                    editChartForm.labelsContainer.clear();
                    editChartForm.jsObject.SendCommandToDesignerServer("GetLabelsContent", { componentName: editChartForm.chartProperties.name }, function (answer) {
                        if (answer.labelsContent) {
                            editChartForm.labelsContainer.update(answer.labelsContent);
                        }
                    });
                }
                else {
                    editChartForm.labelPropertiesContainer.buttons.Common.action();
                }
            }
            if (this.name == "Styles") {
                var areaType = editChartForm.seriesContainer.selectedItem ? editChartForm.seriesContainer.selectedItem.itemObject.type : editChartForm.chartProperties.area.type;
                if (areaType != editChartForm.lastAreaTypeForStyles || editChartForm.jsObject.options.stylesIsModified) {
                    editChartForm.jsObject.options.stylesIsModified = false;
                    editChartForm.stylesContainer.stylesProgress.style.display = "";
                    editChartForm.stylesContainer.clear();
                    editChartForm.jsObject.SendCommandGetStylesContent({ componentName: editChartForm.chartProperties.name });
                }
            }
        }
    }

    editChartForm.onChangeTabs = function () {
        var containerNames = ["seriesConditionsPanel", "labelsConditionsPanel", "seriesFiltersPanel", "seriesTrendLinePanel"];
        for (var i = 0; i < containerNames.length; i++) {
            if (editChartForm[containerNames[i]].container.isModified) {
                editChartForm[containerNames[i]].container.isModified = false;
                editChartForm[containerNames[i]].container.sendValueToServer();
            }
        }
    }

    //Form Methods
    editChartForm.onshow = function () {
        editChartForm.lastSeries = null;
        editChartForm.lastAreaTypeForLabels = null;
        editChartForm.jsObject.options.propertiesPanel.setEditChartMode(true);
        editChartForm.seriesContainer.update();
        editChartForm.ConstantLinesContainer.update();
        editChartForm.StripsContainer.update();
        editChartForm.chartImage.update();
        tabbedPane.showTabPanel(editChartForm.chartProperties.series.length > 0 ? "Series" : "Chart");
    }

    editChartForm.onhide = function () {
        editChartForm.jsObject.options.propertiesPanel.setEditChartMode(false);
        this.jsObject.DeleteTemporaryMenus();
    }

    editChartForm.cancelAction = function () {
        editChartForm.jsObject.SendCommandCanceledEditComponent(editChartForm.chartProperties.name);
        if (editChartForm.oldSvgContent) {
            editChartForm.currentChartComponent.properties.svgContent = editChartForm.oldSvgContent;
            editChartForm.currentChartComponent.repaint();
        }
    }

    editChartForm.action = function () {
        var containerNames = ["seriesConditionsPanel", "labelsConditionsPanel", "seriesFiltersPanel", "seriesTrendLinePanel"];
        for (var i = 0; i < containerNames.length; i++) {
            if (editChartForm[containerNames[i]].container.isModified) {
                editChartForm[containerNames[i]].container.isModified = false;
                editChartForm[containerNames[i]].container.sendValueToServer(true);
                return;
            }
        }
        editChartForm.changeVisibleState(false);
        editChartForm.jsObject.RemoveStylesFromCache(editChartForm.currentChartComponent.properties.name, "StiChart");
        editChartForm.jsObject.SendCommandSendProperties(editChartForm.currentChartComponent, []);
    }

    return editChartForm;
}

//Chart Tab
StiMobileDesigner.prototype.EditChartFormChartTabPanel = function (editChartForm) {
    var panel = document.createElement("div");
    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "750px";
    mainTable.style.height = "460px";
    editChartForm.chartTabMainTable = mainTable;
    panel.appendChild(mainTable);

    //Toolbar
    var buttons = [
        ["add", " ", null, null],
        ["remove", null, "Remove.png", " "],
        ["separator"],
        ["moveUp", null, "Arrows.ArrowUpBlue.png", this.loc.QueryBuilder.MoveUp],
        ["moveDown", null, "Arrows.ArrowDownBlue.png", this.loc.QueryBuilder.MoveDown]
    ]

    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "6px 0 0 12px";
    editChartForm.stripOrConstantLinesToolBar = toolBar;

    var toolBarCell = mainTable.addCell(toolBar);
    toolBarCell.setAttribute("colspan", "2");

    toolBar.changeVisibleState = function (state) {
        toolBar.style.display = state ? "" : "none";
    }

    toolBar.setMode = function (mode) {
        toolBar.mode = mode; //Strips || ConstantLines
        toolBar.add.caption.innerHTML = mode == "Strips" ? this.jsObject.loc.Chart.AddStrip : this.jsObject.loc.Chart.AddConstantLine;
        toolBar.remove.setAttribute("title", mode == "Strips" ? this.jsObject.loc.Chart.RemoveStrip : this.jsObject.loc.Chart.RemoveConstantLine);
        editChartForm[mode + "Container"].onChange();
    }

    for (var i = 0; i < buttons.length; i++) {
        if (buttons[i][0] == "separator") {
            toolBar.addCell(this.HomePanelSeparator()).style.paddingRight = "4px";
            continue;
        }
        var buttonStyle = buttons[i][0] == "add" ? "stiDesignerFormButton" : "stiDesignerStandartSmallButton";
        var button = toolBar[buttons[i][0]] = this.SmallButton("editChartFormStripOrConstantLines" + buttons[i][0], null, buttons[i][1], buttons[i][2], buttons[i][1] || buttons[i][3], buttons[i][4], buttonStyle, true);
        button.style.marginRight = "4px";
        toolBar.addCell(button);
    }

    //Chart Properties Container
    editChartForm.chartPropertiesContainer = this.ChartPropertiesContainer(editChartForm);
    var cellForProperties = mainTable.addCell(editChartForm.chartPropertiesContainer);
    cellForProperties.setAttribute("rowspan", "2");
    cellForProperties.style.width = "1px";
    cellForProperties.style.verticalAlign = "top";

    //Add Strip && Constant Lines Container
    var stripAndConstantLinesCell = mainTable.addCellInNextRow();
    stripAndConstantLinesCell.style.width = "1px";
    stripAndConstantLinesCell.style.verticalAlign = "top";

    var containerNames = ["ConstantLines", "Strips"];
    for (var i = 0; i < containerNames.length; i++) {
        var container = this.StripOrConstantLinesContainer(editChartForm, containerNames[i], toolBar);
        editChartForm[containerNames[i] + "Container"] = container;
        stripAndConstantLinesCell.appendChild(container);
        container.style.display = "none";
    }

    //Add ConstantLine Or Strip
    toolBar.add.action = function (itemType) {
        editChartForm.jsObject.SendCommandAddConstantLineOrStrip({
            componentName: editChartForm.chartProperties.name,
            itemType: toolBar.mode
        });
    }

    //Remove ConstantLine Or Strip
    toolBar.remove.action = function () {
        editChartForm.jsObject.SendCommandRemoveConstantLineOrStrip({
            componentName: editChartForm.chartProperties.name,
            itemIndex: editChartForm[toolBar.mode + "Container"].getSelectedIndex(),
            itemType: toolBar.mode
        });
    }

    var itemMove = function (direction) {
        editChartForm.jsObject.SendCommandConstantLineOrStripMove({
            componentName: editChartForm.chartProperties.name,
            itemIndex: editChartForm[toolBar.mode + "Container"].getSelectedIndex(),
            itemType: toolBar.mode,
            direction: direction
        });
    }

    //Series Move Up
    toolBar.moveUp.action = function () { itemMove("Up"); }

    //Series Move Down
    toolBar.moveDown.action = function () { itemMove("Down"); }

    //Chart image container
    editChartForm.imageContainerChartTab = mainTable.addCellInLastRow();
    editChartForm.imageContainerChartTab.style.textAlign = "center";

    //Series wizard container
    var seriesWizardContainer = this.SeriesWizardContainer(editChartForm);
    editChartForm.seriesWizardContainer = seriesWizardContainer;
    panel.appendChild(seriesWizardContainer);

    return panel;
}

StiMobileDesigner.prototype.StripOrConstantLinesContainer = function (editChartForm, containerType, toolBar) {
    var container = this.Container("editChartFormContainer" + containerType, 200, 400);
    container.containerType = containerType;
    container.style.margin = "11px 12px 0 12px";
    container.className = "stiDesignerSeriesContainer";

    container.onChange = function () {
        toolBar.remove.setEnabled(this.items.length > 0);
        var selectedIndex = this.getSelectedIndex();
        toolBar.moveUp.setEnabled(selectedIndex != -1 && selectedIndex > 0);
        toolBar.moveDown.setEnabled(selectedIndex != -1 && selectedIndex < this.items.length - 1);
        var editChartPropertiesPanel = editChartForm.jsObject.options.propertiesPanel.editChartPropertiesPanel;
        editChartPropertiesPanel.innerPanels["Chart"].showInnerPanel(this.containerType);
    }

    container.update = function (notSelectedAfter) {
        this.clear();
        var collection = this.containerType == "ConstantLines" ? editChartForm.chartProperties.constantLines : editChartForm.chartProperties.strips;

        for (var i = 0; i < collection.length; i++) {
            this.addItemAndNotAction(collection[i].name, collection[i]);
        }
        if (this.items.length > 0 && !notSelectedAfter) this.items[0].action();
    }

    return container;
}

//Chart Properties Container
StiMobileDesigner.prototype.ChartPropertiesContainer = function (editChartForm) {
    var propertiesContainer = this.ContainerWithBigItems("editChartFormChartPropertiesContainer", 120, 460);
    propertiesContainer.className = "stiDesignerSeriesPropertiesContainer";
    propertiesContainer.buttons = {};

    var propNames = [
        ["Common", this.loc.Chart.Common],
        ["Legend", this.loc.PropertyMain.Legend],
        ["Title", this.loc.PropertyMain.Title],
        ["ConstantLines", this.loc.PropertyMain.ConstantLines],
        ["Strips", this.loc.PropertyMain.Strips],
        ["Table", this.loc.PropertyMain.Table]]

    for (var i = 0; i < propNames.length; i++) {
        var button = propertiesContainer.addItemAndNotAction(propNames[i][0], propNames[i][1], "StiChart_" + propNames[i][0] + ".png", {});
        button.style.minWidth = "95px";
        propertiesContainer.buttons[propNames[i][0]] = button;
    }

    propertiesContainer.onAction = function () {
        var selectedName = this.selectedItem.name;
        var editChartPropertiesPanel = editChartForm.jsObject.options.propertiesPanel.editChartPropertiesPanel;
        editChartPropertiesPanel.innerPanels["Chart"].showInnerPanel(selectedName);

        editChartForm.stripOrConstantLinesToolBar.changeVisibleState(selectedName == "ConstantLines" || selectedName == "Strips");
        editChartForm.StripsContainer.style.display = (selectedName == "Strips") ? "" : "none";
        editChartForm.ConstantLinesContainer.style.display = (selectedName == "ConstantLines") ? "" : "none";

        if (selectedName == "ConstantLines" || selectedName == "Strips") {
            editChartForm.stripOrConstantLinesToolBar.setMode(selectedName);
        }
    }

    return propertiesContainer;
}

//Series Tab
StiMobileDesigner.prototype.EditChartFormSeriesTabPanel = function (editChartForm) {
    var mainTable = this.CreateHTMLTable();

    //Toolbar
    var buttons = [
        ["addSeries", this.loc.Chart.AddSeries.replace("&", ""), null, null, "Down"],
        ["removeSeries", null, "Remove.png", this.loc.Chart.RemoveSeries.replace("&", "")],
        ["separator"],
        ["moveUp", null, "Arrows.ArrowUpBlue.png", this.loc.Chart.MoveSeriesUp],
        ["moveDown", null, "Arrows.ArrowDownBlue.png", this.loc.Chart.MoveSeriesDown]
    ]

    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "6px 0 0 12px";
    editChartForm.seriesToolBar = toolBar;
    var toolBarCell = mainTable.addCell(toolBar);
    editChartForm.toolBarCell = toolBarCell;
    toolBarCell.setAttribute("colspan", "2");

    toolBar.changeVisibleState = function (state) {
        toolBar.style.display = state ? "" : "none";
    }

    for (var i = 0; i < buttons.length; i++) {
        if (buttons[i][0] == "separator") {
            toolBar.addCell(this.HomePanelSeparator()).style.paddingRight = "4px";
            continue;
        }
        var buttonStyle = buttons[i][0] == "addSeries" ? "stiDesignerFormButton" : "stiDesignerStandartSmallButton";
        var button = this.SmallButton("editChartForm" + buttons[i][0], null, buttons[i][1], buttons[i][2], buttons[i][1] || buttons[i][3], buttons[i][4], buttonStyle, true);
        button.style.marginRight = "4px";
        toolBar[buttons[i][0]] = button;
        toolBar.addCell(button);
    }

    //Add Series Menu
    var addSeriesMenu = this.InitializeAddSeriesMenu(editChartForm);
    addSeriesMenu.innerContent.style.maxHeight = null;
    toolBar.addSeries.action = function () { addSeriesMenu.changeVisibleState(!this.isSelected); }

    addSeriesMenu.action = function (menuItem) {
        addSeriesMenu.jsObject.SendCommandAddSeries({
            componentName: editChartForm.chartProperties.name,
            seriesType: menuItem.key
        });
    }

    //Remove Series
    toolBar.removeSeries.action = function () {
        editChartForm.jsObject.SendCommandRemoveSeries({
            componentName: editChartForm.chartProperties.name,
            seriesIndex: editChartForm.seriesContainer.getSelectedIndex()
        });
    }

    var seriesMove = function (direction) {
        editChartForm.jsObject.SendCommandSeriesMove({
            componentName: editChartForm.chartProperties.name,
            seriesIndex: editChartForm.seriesContainer.getSelectedIndex(),
            direction: direction
        });
    }

    //Series Move Up
    toolBar.moveUp.action = function () { seriesMove("Up"); }

    //Series Move Down
    toolBar.moveDown.action = function () { seriesMove("Down"); }

    //Series Properties Container
    editChartForm.seriesPropertiesContainer = this.SeriesPropertiesContainer(editChartForm);
    var cellForProperties = mainTable.addCell(editChartForm.seriesPropertiesContainer);
    cellForProperties.setAttribute("rowspan", "2");
    cellForProperties.style.width = "1px";
    cellForProperties.style.verticalAlign = "top";

    //Series Container
    var seriesContainer = editChartForm.seriesContainer = this.Container("editChartFormSeriesContainer", 200, 400);
    seriesContainer.className = "stiDesignerSeriesContainer";
    seriesContainer.style.margin = "11px 12px 0 12px";

    var seriesCell = mainTable.addCellInNextRow(seriesContainer);
    seriesCell.style.width = "1px";
    seriesCell.style.verticalAlign = "top";

    //Series Labels Container
    var seriesLabelsProgress = this.Progress();
    seriesLabelsProgress.className = "stiDesignerChartFormProgress";
    seriesLabelsProgress.style.display = "none";

    var seriesLabelsContainer = editChartForm.seriesLabelsContainer = this.LabelsContainer(editChartForm, true);
    seriesLabelsContainer.seriesLabelsProgress = seriesLabelsProgress;
    seriesCell.appendChild(seriesLabelsProgress);
    seriesCell.appendChild(seriesLabelsContainer);
    seriesLabelsContainer.style.display = "none";
    seriesLabelsContainer.style.margin = "6px 0 0 12px";

    seriesContainer.onChange = function () {
        editChartForm.seriesPropertiesContainer.update();
        toolBar.removeSeries.setEnabled(seriesContainer.items.length > 0);
        var selectedIndex = seriesContainer.getSelectedIndex();
        toolBar.moveUp.setEnabled(selectedIndex != -1 && selectedIndex > 0);
        toolBar.moveDown.setEnabled(selectedIndex != -1 && selectedIndex < seriesContainer.items.length - 1);
    }

    seriesContainer.update = function (notSelectedAfter) {
        seriesContainer.clear();
        for (var i = 0; i < editChartForm.chartProperties.series.length; i++) {
            seriesContainer.addItemAndNotAction(editChartForm.chartProperties.series[i].name, editChartForm.chartProperties.series[i]);
        }
        if (seriesContainer.items.length > 0 && !notSelectedAfter) seriesContainer.items[0].action();
    }

    //Middle Cell
    editChartForm.middleTable = this.CreateHTMLTable();
    editChartForm.middleTable.style.height = "100%";
    editChartForm.middleTable.style.width = "100%";
    editChartForm.middleCell = mainTable.addCellInLastRow(editChartForm.middleTable);
    editChartForm.middleCell.style.verticalAlign = "top";

    //Conditions container
    editChartForm.seriesConditionsPanel = this.EditChartFormConditionsPanel(editChartForm, "SeriesConditions");
    editChartForm.middleCell.appendChild(editChartForm.seriesConditionsPanel);
    editChartForm.seriesConditionsPanel.style.display = "none";

    //Filters container
    editChartForm.seriesFiltersPanel = this.EditChartFormFiltersPanel(editChartForm);
    editChartForm.middleCell.appendChild(editChartForm.seriesFiltersPanel);
    editChartForm.seriesFiltersPanel.style.display = "none";

    //TrendLines container
    editChartForm.seriesTrendLinePanel = this.EditChartFormTrendLinesPanel(editChartForm);
    editChartForm.middleCell.appendChild(editChartForm.seriesTrendLinePanel);
    editChartForm.seriesTrendLinePanel.style.display = "none";

    //Series Caption
    editChartForm.seriesCaptionCell = editChartForm.middleTable.addCell();
    editChartForm.seriesCaptionCell.className = "stiDesignerChartFormSeriesCaptionCell";
    editChartForm.seriesCaptionCell.style.height = "1px";

    //Chart image container
    editChartForm.imageContainerSeriesTab = editChartForm.middleTable.addCellInNextRow();
    editChartForm.imageContainerSeriesTab.style.textAlign = "center";

    //Show Series Labels Buttons
    editChartForm.showSeriesLabelsPanel = this.ShowSeriesLabelsPanel(editChartForm);
    editChartForm.showSeriesLabelsPanelCell = editChartForm.middleTable.addCellInNextRow(editChartForm.showSeriesLabelsPanel);
    editChartForm.showSeriesLabelsPanelCell.style.height = "1px";

    return mainTable;
}


//Show Series Labels Buttons
StiMobileDesigner.prototype.ShowSeriesLabelsPanel = function (editChartForm) {
    var mainTable = this.CreateHTMLTable();

    var textCell = mainTable.addTextCell(this.loc.PropertyMain.ShowSeriesLabels);
    textCell.className = "stiDesignerCaptionControls";
    textCell.style.padding = "4px 12px 4px 30px";

    var showSeriesLabels = mainTable.mainControl = this.DropDownList("showSeriesLabelsChartForm", 150, null, this.GetShowSeriesLabelsItems(), true);
    mainTable.addCell(showSeriesLabels);

    showSeriesLabels.action = function () {
        var seriesIndex = editChartForm.seriesContainer.getSelectedIndex();

        if (seriesIndex != -1 && editChartForm.chartProperties.series[seriesIndex] != null) {
            var propertyControl = editChartForm.jsObject.options.propertiesPanel.editChartPropertiesPanel.innerPanels["Series"].innerPanels["Common"].groups["Common"].properties["ShowSeriesLabels"].control;
            propertyControl.setKey(this.key);
            propertyControl.action();
        }
    }

    return mainTable;
}

//Categories Series Properties Container
StiMobileDesigner.prototype.SeriesPropertiesContainer = function (editChartForm) {
    var propertiesContainer = this.ContainerWithBigItems("editChartFormSeriesPropertiesContainer", 120, 460);
    propertiesContainer.className = "stiDesignerSeriesPropertiesContainer";
    propertiesContainer.buttons = {};

    var propNames = ["Common", "Conditions", "Filters", "Marker", "LineMarker", "Interaction", "TrendLine", "TopN", "SeriesLabels"];

    for (var i = 0; i < propNames.length; i++) {
        var caption = propNames[i] == "Common" ? this.loc.Chart.Common : this.loc.PropertyMain[propNames[i]];
        var button = propertiesContainer.addItemAndNotAction(propNames[i], caption, "Series_" + propNames[i] + ".png", {});
        button.style.minWidth = "95px";
        propertiesContainer.buttons[propNames[i]] = button;
        button.style.display = "none";
    }

    propertiesContainer.onAction = function () {
        var selectedName = this.selectedItem.name;
        var editChartPropertiesPanel = editChartForm.jsObject.options.propertiesPanel.editChartPropertiesPanel;
        editChartPropertiesPanel.innerPanels["Series"].showInnerPanel(selectedName);
        editChartForm.onChangeTabs();

        editChartForm.seriesContainer.style.display = (selectedName != "SeriesLabels" && selectedName != "Conditions" && selectedName != "Filters" && selectedName != "TrendLine") ? "" : "none";
        editChartForm.middleTable.style.display = (selectedName != "Conditions" && selectedName != "Filters" && selectedName != "TrendLine") ? "" : "none";
        editChartForm.seriesToolBar.changeVisibleState(selectedName != "SeriesLabels" && selectedName != "Conditions" && selectedName != "Filters" && selectedName != "TrendLine");
        editChartForm.seriesLabelsContainer.style.display = (selectedName == "SeriesLabels") ? "" : "none";
        editChartForm.seriesCaptionCell.style.display = (selectedName == "SeriesLabels") ? "" : "none";
        editChartForm.showSeriesLabelsPanelCell.style.display = (selectedName == "SeriesLabels") ? "" : "none";
        editChartForm.seriesConditionsPanel.style.display = selectedName == "Conditions" ? "" : "none";
        editChartForm.seriesFiltersPanel.style.display = selectedName == "Filters" ? "" : "none";
        editChartForm.seriesTrendLinePanel.style.display = selectedName == "TrendLine" ? "" : "none";

        if (selectedName == "SeriesLabels") {
            var seriesIndex = editChartForm.seriesContainer.getSelectedIndex();
            if (seriesIndex != -1) {
                editChartForm.seriesCaptionCell.innerHTML = editChartForm.chartProperties.series[seriesIndex].name;
                editChartForm.showSeriesLabelsPanel.mainControl.setKey(editChartForm.chartProperties.series[seriesIndex].properties.Common.ShowSeriesLabels);
            }

            if (editChartForm.seriesContainer.selectedItem != null && (
                editChartForm.seriesContainer.selectedItem != editChartForm.lastSeries ||
                editChartForm.chartProperties.style.type + editChartForm.chartProperties.style.name != editChartForm.lastStyleIdForSeriesLables)
            ) {
                editChartForm.seriesLabelsContainer.clear();
                editChartForm.seriesLabelsContainer.seriesLabelsProgress.style.display = "";
                editChartForm.jsObject.SendCommandToDesignerServer("GetLabelsContent", { componentName: editChartForm.chartProperties.name, seriesIndex: seriesIndex }, function (answer) {
                    if (answer.labelsContent) {
                        editChartForm.seriesLabelsContainer.update(answer.labelsContent);
                    }
                });
            }
        }

        if (selectedName == "Conditions" || selectedName == "Filters" || selectedName == "TrendLine") {
            var container = editChartForm["series" + selectedName + "Panel"].container;
            container.clear();
            var seriesIndex = editChartForm.seriesContainer.getSelectedIndex();
            if (seriesIndex != -1) {
                container.addItems(editChartForm.chartProperties.series[seriesIndex][selectedName == "Conditions" ? "conditions" : (selectedName == "TrendLine" ? "trendLines" : "filters")]);
                if (selectedName == "Filters") container.toolBar.filterType.setKey(editChartForm.chartProperties.series[seriesIndex].filterMode);
            }
        }
    }

    propertiesContainer.update = function () {
        var currentSeriesType = editChartForm.seriesContainer.selectedItem ? editChartForm.seriesContainer.selectedItem.itemObject.type : null;
        //debugger;
        var showTopN = false;
        var showInteraction = false;
        var showSeriesLabels = false;
        var showTrendLine = false;
        var showLineMarker = false;
        var showMarker = false;
        if (editChartForm.jsObject.options.isJava && currentSeriesType != null) {
            currentSeriesType = currentSeriesType.substring(currentSeriesType.lastIndexOf(".") + 1);
        }
        switch (currentSeriesType) {
            case "StiClusteredColumnSeries":
            case "StiClusteredBarSeries":
                {
                    showTopN = true;
                    showInteraction = true;
                    showSeriesLabels = true;
                    showTrendLine = true;
                    break;
                }
            case "StiStackedColumnSeries":
            case "StiFullStackedColumnSeries":
            case "StiStackedBarSeries":
            case "StiFullStackedBarSeries":
            case "StiPieSeries":
            case "StiPie3dSeries":
            case "StiFunnelSeries":
            case "StiFunnelWeightedSlicesSeries":
            case "StiDoughnutSeries":
            case "StiTreemapSeries":
            case "StiSunburstSeries":
                {
                    showTopN = true;
                    showInteraction = true;
                    showSeriesLabels = true;
                    break;
                }
            case "StiLineSeries":
            case "StiSteppedLineSeries":
            case "StiAreaSeries":
            case "StiSteppedAreaSeries":
                {
                    showTopN = true;
                    showLineMarker = true;
                    showMarker = true;
                    showInteraction = true;
                    showSeriesLabels = true;
                    showTrendLine = true;
                    break;
                }
            case "StiStackedLineSeries":
            case "StiFullStackedLineSeries":
            case "StiStackedAreaSeries":
            case "StiFullStackedAreaSeries":
                {
                    showTopN = true;
                    showLineMarker = true;
                    showMarker = true;
                    showInteraction = true;
                    showSeriesLabels = true;
                    break;
                }
            case "StiSplineSeries":
            case "StiSplineAreaSeries":
                {
                    showTopN = true;
                    showMarker = true;
                    showInteraction = true;
                    showSeriesLabels = true;
                    showTrendLine = true;
                    break;
                }
            case "StiStackedSplineSeries":
            case "StiFullStackedSplineSeries":
            case "StiStackedSplineAreaSeries":
            case "StiFullStackedSplineAreaSeries":
            case "StiRadarPointSeries":
            case "StiRadarLineSeries":
            case "StiRadarAreaSeries":
                {
                    showTopN = true;
                    showMarker = true;
                    showInteraction = true;
                    showSeriesLabels = true;
                    break;
                }
            case "StiRangeSeries":
            case "StiSteppedRangeSeries":
                {
                    showLineMarker = true;
                    showMarker = true;
                    showInteraction = true;
                    showSeriesLabels = true;
                    break;
                }
            case "StiSplineRangeSeries":
                {
                    showMarker = true;
                    showInteraction = true;
                    showSeriesLabels = true;
                    break;
                }
            case "StiRangeBarSeries":
            case "StiCandlestickSeries":
            case "StiStockSeries":
            case "StiGanttSeries":
                {
                    showInteraction = true;
                    showSeriesLabels = true;
                    break;
                }
            case "StiScatterSeries":
            case "StiScatterLineSeries":
                {
                    showLineMarker = true;
                    showMarker = true;
                    showInteraction = true;
                    showSeriesLabels = true;
                    showTrendLine = true;
                    break;
                }
            case "StiScatterSplineSeries":
                {
                    showMarker = true;
                    showInteraction = true;
                    showSeriesLabels = true;
                    showTrendLine = true;
                    break;
                }
            case "StiBubbleSeries":
                {
                    showInteraction = true;
                    showSeriesLabels = true;
                    showTrendLine = true;
                    break;
                }
            case "StiParetoSeries":
                {
                    showInteraction = true;
                    showSeriesLabels = true;
                    break;
                }
            case "StiPictorialStackedSeries":
                {
                    showInteraction = true;
                    break;
                }
        }

        propertiesContainer.buttons.Common.style.display = "";
        propertiesContainer.buttons.Conditions.style.display = currentSeriesType ? "" : "none";
        propertiesContainer.buttons.Filters.style.display = currentSeriesType ? "" : "none";
        propertiesContainer.buttons.Marker.style.display = showMarker ? "" : "none";
        propertiesContainer.buttons.LineMarker.style.display = showLineMarker ? "" : "none";
        propertiesContainer.buttons.Interaction.style.display = showInteraction ? "" : "none";
        propertiesContainer.buttons.TrendLine.style.display = showTrendLine ? "" : "none";
        propertiesContainer.buttons.TopN.style.display = showTopN ? "" : "none";
        propertiesContainer.buttons.SeriesLabels.style.display = showSeriesLabels ? "" : "none";

        propertiesContainer.buttons.Common.action();
    }

    return propertiesContainer;
}

//Area Tab
StiMobileDesigner.prototype.EditChartFormAreaTabPanel = function (editChartForm) {
    var mainTable = this.CreateHTMLTable();

    //Chart image container
    editChartForm.imageContainerAreaTab = mainTable.addCell();
    editChartForm.imageContainerAreaTab.style.textAlign = "center";

    //Area Properties Container    
    editChartForm.areaPropertiesContainer = this.AreaPropertiesContainer(editChartForm);
    mainTable.addCell(editChartForm.areaPropertiesContainer).style.width = "1px";

    return mainTable;
}

//Categories Area Properties Container
StiMobileDesigner.prototype.AreaPropertiesContainer = function (editChartForm) {
    var propertiesContainer = this.ContainerWithBigItems("editChartFormAreaPropertiesContainer", 120, 460);
    propertiesContainer.className = "stiDesignerSeriesPropertiesContainer";
    propertiesContainer.buttons = {};

    var propNames = ["Common", "XAxis", "YAxis", "XTopAxis", "YRightAxis", "GridLinesHor", "GridLinesHorRight", "GridLinesVert", "InterlacingHor", "InterlacingVert"];

    for (var i = 0; i < propNames.length; i++) {
        var captionText = propNames[i] == "Common" ? this.loc.Chart.Common : this.loc.PropertyMain[propNames[i]];
        var button = propertiesContainer.addItemAndNotAction(propNames[i], captionText, "Area_" + propNames[i] + ".png", {});
        button.style.minWidth = "95px";
        propertiesContainer.buttons[propNames[i]] = button;
        button.style.display = "none";
    }

    propertiesContainer.onAction = function () {
        var editChartPropertiesPanel = editChartForm.jsObject.options.propertiesPanel.editChartPropertiesPanel;
        editChartPropertiesPanel.innerPanels["Area"].showInnerPanel("AllProperties");
    }

    propertiesContainer.update = function () {
        var Container = {};
        Container.Common = true;
        Container.GridLinesHor_IStiGridLinesHor = false;
        Container.GridLinesHorRight = false;
        Container.GridLinesVert_IStiGridLinesVert = false;
        Container.InterlacingHor = false;
        Container.InterlacingVert = false;
        Container.XAxis_IStiXAxis = false;
        Container.XTopAxis = false;
        Container.YAxis_IStiYAxis = false;
        Container.YRightAxis = false;
        Container.GridLinesHor_IStiRadarGridLinesHor = false;
        Container.GridLinesVert_IStiRadarGridLinesVert = false;
        Container.XAxis_IStiXRadarAxis = false;
        Container.YAxis_IStiYRadarAxis = false;

        var areaType = editChartForm.seriesContainer.items.length > 0 ? editChartForm.chartProperties.area.type : "";

        if (areaType == "StiClusteredColumnArea" || areaType == "StiStackedColumnArea" || areaType == "StiFullStackedColumnArea" ||
            areaType == "StiRangeArea" || areaType == "StiSplineRangeArea" || areaType == "StiSteppedRangeArea" || areaType == "StiRangeBarArea" ||
            areaType == "StiClusteredBarArea" || areaType == "StiStackedBarArea" || areaType == "StiFullStackedBarArea" ||
            areaType == "StiScatterArea" || areaType == "StiCandlestickArea" || areaType == "StiStockArea" || areaType == "StiGanttArea" ||
            areaType == "StiBubbleArea" || areaType == "StiParetoArea" || areaType == "StiWaterfallArea" || areaType == "StiRibbonArea") {
            Container.GridLinesHor_IStiGridLinesHor = true;
            Container.GridLinesHorRight = true;
            Container.GridLinesVert_IStiGridLinesVert = true;
            Container.InterlacingHor = true;
            Container.InterlacingVert = true;
            Container.XAxis_IStiXAxis = true;
            Container.XTopAxis = true;
            Container.YAxis_IStiYAxis = true;
            Container.YRightAxis = true;
        }
        else if (areaType == "StiClusteredColumnArea3D" || areaType == "StiStackedColumnArea3D" || areaType == "StiFullStackedColumnArea3D") {
            Container.GridLinesHor_IStiGridLinesHor = true;
            Container.GridLinesVert_IStiGridLinesVert = true;
            Container.InterlacingHor = true;
            Container.InterlacingVert = true;
            Container.XAxis_IStiXAxis = true;
            Container.YAxis_IStiYAxis = true;
        }
        else if (areaType == "StiRadarAreaArea" || areaType == "StiRadarLineArea" || areaType == "StiRadarPointArea") {
            Container.GridLinesHor_IStiRadarGridLinesHor = true;
            Container.GridLinesVert_IStiRadarGridLinesVert = true;
            Container.InterlacingHor = true;
            Container.InterlacingVert = true;
            Container.XAxis_IStiXRadarAxis = true;
            Container.YAxis_IStiYRadarAxis = true;
        }

        Container.XAxis = Container.XAxis_IStiXAxis || Container.XAxis_IStiXRadarAxis;
        Container.YAxis = Container.YAxis_IStiYAxis || Container.YAxis_IStiYRadarAxis;
        Container.GridLinesHor = Container.GridLinesHor_IStiGridLinesHor || Container.GridLinesHor_IStiRadarGridLinesHor;
        Container.GridLinesVert = Container.GridLinesVert_IStiGridLinesVert || Container.GridLinesVert_IStiRadarGridLinesVert;

        for (var i = 0; i < propNames.length; i++) {
            propertiesContainer.buttons[propNames[i]].style.display = Container[propNames[i]] ? "" : "none";
            if (propNames[i] == "XAxis") {
                StiMobileDesigner.setImageSource(propertiesContainer.buttons[propNames[i]].image, editChartForm.jsObject.options,
                    "Area_" + (Container.XAxis_IStiXAxis ? "XAxis.png" : "XAxis_IStiXRadarAxis.png"));
            }
            if (propNames[i] == "YAxis") {
                StiMobileDesigner.setImageSource(propertiesContainer.buttons[propNames[i]].image, editChartForm.jsObject.options,
                    "Area_" + (Container.YAxis_IStiYAxis ? "YAxis.png" : "YAxis_IStiYRadarAxis.png"));
            }
            if (propNames[i] == "GridLinesHor") {
                StiMobileDesigner.setImageSource(propertiesContainer.buttons[propNames[i]].image, editChartForm.jsObject.options,
                    "Area_" + (Container.GridLinesHor_IStiGridLinesHor ? "GridLinesHor.png" : "GridLinesHor_IStiRadarGridLinesHor.png"));
            }
            if (propNames[i] == "GridLinesVert") {
                StiMobileDesigner.setImageSource(propertiesContainer.buttons[propNames[i]].image, editChartForm.jsObject.options,
                    "Area_" + (Container.GridLinesVert_IStiGridLinesVert ? "GridLinesVert.png" : "GridLinesVert_IStiRadarGridLinesVert.png"));
            }
        }
        propertiesContainer.buttons.Common.action();

        if (areaType == "") {
            propertiesContainer.buttons.Common.style.display = "none";
            var editChartPropertiesPanel = editChartForm.jsObject.options.propertiesPanel.editChartPropertiesPanel;
            editChartPropertiesPanel.innerPanels["Area"].innerPanels["AllProperties"].style.display = "none";
        }
    }

    return propertiesContainer;
}

//Labels Tab
StiMobileDesigner.prototype.EditChartFormLabelsTabPanel = function (editChartForm) {
    var mainTable = this.CreateHTMLTable();

    //Labels Container
    var labelsProgress = this.Progress();
    labelsProgress.className = "stiDesignerChartFormProgress";
    labelsProgress.style.display = "none";

    var labelsContainer = editChartForm.labelsContainer = this.LabelsContainer(editChartForm);
    labelsContainer.style.margin = "6px 0 0 12px";

    var containerCell = mainTable.addCell();
    containerCell.style.width = "1px";
    containerCell.style.verticalAlign = "top";
    containerCell.appendChild(labelsProgress);
    containerCell.appendChild(labelsContainer);
    labelsContainer.labelsProgress = labelsProgress;

    //Chart image container
    var imageContainerLabelsTab = editChartForm.imageContainerLabelsTab = mainTable.addCell();
    imageContainerLabelsTab.style.textAlign = "center";

    //Conditions container
    var labelsConditionsPanel = editChartForm.labelsConditionsPanel = this.EditChartFormConditionsPanel(editChartForm, "LabelsConditions");
    var labelsConditionsCell = editChartForm.labelsConditionsCell = mainTable.addCell(labelsConditionsPanel);
    labelsConditionsCell.style.display = "none";
    labelsConditionsCell.style.verticalAlign = "top";

    //Labels Properties Container
    var labelPropsCont = editChartForm.labelPropertiesContainer = this.ContainerWithBigItems("editChartFormLabelsPropertiesContainer", 120, 460);
    labelPropsCont.className = "stiDesignerSeriesPropertiesContainer";
    labelPropsCont.buttons = {};

    var labelPropsCell = mainTable.addCell(labelPropsCont)
    labelPropsCell.style.width = "1px";
    labelPropsCell.style.verticalAlign = "top";

    labelPropsCont.buttons.Common = labelPropsCont.addItemAndNotAction("Common", this.loc.Chart.Common, "Labels_Common.png", {});
    labelPropsCont.buttons.Conditions = labelPropsCont.addItemAndNotAction("Conditions", this.loc.PropertyMain.Conditions, "Series_Conditions.png", {});

    labelPropsCont.onAction = function () {
        var selectedName = this.selectedItem.name;
        var editChartPropertiesPanel = editChartForm.jsObject.options.propertiesPanel.editChartPropertiesPanel;
        editChartPropertiesPanel.innerPanels["Labels"].showInnerPanel(selectedName);
        editChartForm.onChangeTabs();

        labelsContainer.style.display = (selectedName == "Common") ? "" : "none";
        imageContainerLabelsTab.style.display = (selectedName == "Common") ? "" : "none";
        labelsConditionsCell.style.display = (selectedName == "Conditions") ? "" : "none";

        if (selectedName == "Conditions") {
            var container = labelsConditionsPanel.container;
            container.clear();
            container.addItems(editChartForm.chartProperties.conditions);
        }
    }

    return mainTable;
}

//Labels Container
StiMobileDesigner.prototype.LabelsContainer = function (editChartForm, isSeriesLabels) {
    var labelsContainer = this.ContainerWithBigItems(isSeriesLabels ? "editChartFormSeriesLabelsContainer" : "editChartFormLabelsContainer", 200, 432, 80);
    labelsContainer.className = "stiDesignerSeriesContainer";
    labelsContainer.style.paddingTop = "6px";
    labelsContainer.buttons = {};
    labelsContainer.isSeriesLabels = isSeriesLabels;

    labelsContainer.onAction = function () {
        if (labelsContainer.selectedItem != null) {
            var seriesIndex = editChartForm.seriesContainer.getSelectedIndex();
            var params = {
                componentName: editChartForm.chartProperties.name,
                labelsType: labelsContainer.selectedItem.name
            };
            if (labelsContainer.isSeriesLabels) {
                params.seriesIndex = seriesIndex;
            }
            editChartForm.jsObject.SendCommandSetLabelsType(params);
        }
    }

    labelsContainer.update = function (labelsContent) {
        editChartForm[!labelsContainer.isSeriesLabels ? "lastStyleId" : "lastStyleIdForSeriesLables"] = editChartForm.chartProperties.style.type + editChartForm.chartProperties.style.name;
        editChartForm.labelsContainer.labelsProgress.style.display = "none";
        editChartForm.seriesLabelsContainer.seriesLabelsProgress.style.display = "none";

        if (!labelsContainer.isSeriesLabels)
            editChartForm.lastAreaTypeForLabels = editChartForm.seriesContainer.selectedItem ? editChartForm.seriesContainer.selectedItem.itemObject.type : editChartForm.chartProperties.area.type;
        else
            editChartForm.lastSeries = editChartForm.seriesContainer.selectedItem;

        labelsContainer.clear();
        var labelsType = editChartForm.chartProperties.labels.type;
        if (labelsContainer.isSeriesLabels) {
            var seriesIndex = editChartForm.seriesContainer.getSelectedIndex();
            if (seriesIndex != -1) labelsType = editChartForm.chartProperties.series[seriesIndex].labels.type;
        }
        for (var i = 0; i < labelsContent.length; i++) {
            var name = labelsContent[i].type;
            var button = labelsContainer.addItemAndNotAction(name, labelsContent[i].caption, " ", {});
            button.caption.style.height = "24px";
            button.caption.style.verticalAlign = "top";
            button.cellImage.removeChild(button.image);
            button.image = document.createElement("div");
            button.image.innerHTML = labelsContent[i].image;
            button.cellImage.appendChild(button.image);
            labelsContainer.buttons[name] = button;

            if (labelsType == name) {
                button.selected();
            }
        }
        if (!labelsContainer.isSeriesLabels) editChartForm.labelPropertiesContainer.buttons.Common.action();
    }

    return labelsContainer;
}

//Styles Tab
StiMobileDesigner.prototype.EditChartFormStylesTabPanel = function (editChartForm) {
    var mainTable = this.CreateHTMLTable();

    //Styles Container
    var stylesProgress = this.Progress();
    stylesProgress.className = "stiDesignerChartFormProgress";
    stylesProgress.style.display = "none";

    var stylesContainer = editChartForm.stylesContainer = this.StylesContainer(editChartForm);
    stylesContainer.style.paddingTop = "6px";
    stylesContainer.style.margin = "6px 0 0 12px";

    var containerCell = mainTable.addCellInNextRow();
    containerCell.style.width = "1px";
    containerCell.style.verticalAlign = "top";
    containerCell.appendChild(stylesProgress);
    containerCell.appendChild(stylesContainer);
    stylesContainer.stylesProgress = stylesProgress;

    //Chart image container
    editChartForm.imageContainerStylesTab = mainTable.addCellInLastRow();
    editChartForm.imageContainerStylesTab.style.textAlign = "center";

    return mainTable;
}

//Styles Container
StiMobileDesigner.prototype.StylesContainer = function (editChartForm) {
    var stylesContainer = this.ContainerWithBigItems("editChartFormStylesContainer", 200, 432, 80);
    stylesContainer.className = "stiDesignerSeriesContainer";
    stylesContainer.buttons = {};

    stylesContainer.onAction = function () {
        if (stylesContainer.selectedItem != null) {
            var params = {
                componentName: editChartForm.chartProperties.name,
                styleType: stylesContainer.selectedItem.itemObject.type,
                styleName: stylesContainer.selectedItem.itemObject.name
            }
            editChartForm.jsObject.SendCommandSetChartStyle(params);
        }
    }

    stylesContainer.update = function (stylesContent) {
        editChartForm.lastAreaTypeForStyles = editChartForm.seriesContainer.selectedItem ? editChartForm.seriesContainer.selectedItem.itemObject.type : editChartForm.chartProperties.area.type;
        stylesContainer.clear();
        for (var i = 0; i < stylesContent.length; i++) {
            var name = stylesContent[i].type + stylesContent[i].name;
            var caption = stylesContent[i].name || stylesContent[i].type.replace("Sti", "");
            var button = stylesContainer.addItemAndNotAction(name, caption, " ", stylesContent[i]);
            button.cellImage.removeChild(button.image);
            button.cellImage.style.padding = "5px 5px 0 5px";
            button.image = document.createElement("div");
            button.image.innerHTML = stylesContent[i].image;
            button.cellImage.appendChild(button.image);
            stylesContainer.buttons[name] = button;

            if (editChartForm.chartProperties.style.type + editChartForm.chartProperties.style.name == name) {
                button.selected();
            }
        }
        editChartForm.stylesContainer.stylesProgress.style.display = "none";
    }

    return stylesContainer;
}

StiMobileDesigner.prototype.EditChartFormConditionsPanel = function (editChartForm, containerType) {
    var panel = document.createElement("div");

    //Toolbar
    var buttons = [
        ["add", this.loc.Chart.AddCondition.replace("&", ""), null, null],
        ["separator"],
        ["moveUp", null, "Arrows.ArrowUpBlue.png", this.loc.QueryBuilder.MoveUp],
        ["moveDown", null, "Arrows.ArrowDownBlue.png", this.loc.QueryBuilder.MoveDown]
    ]

    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "6px 0 0 12px";
    panel.appendChild(toolBar);

    for (var i = 0; i < buttons.length; i++) {
        if (buttons[i][0].indexOf("separator") >= 0) {
            toolBar[buttons[i][0]] = this.HomePanelSeparator();
            toolBar.addCell(toolBar[buttons[i][0]]).style.paddingRight = "4px";
            continue;
        }
        var buttonStyle = buttons[i][0] == "add" ? "stiDesignerFormButton" : "stiDesignerStandartSmallButton";
        var button = this.SmallButton(null, null, buttons[i][1], buttons[i][2], buttons[i][1] || buttons[i][3], buttons[i][4], buttonStyle, true);
        button.style.marginRight = "4px";
        toolBar[buttons[i][0]] = button;
        toolBar.addCell(button);
    }

    toolBar.moveUp.setEnabled(false);
    toolBar.moveDown.setEnabled(false);

    if (!this.options.isTouchDevice) {
        toolBar.moveUp.style.display = toolBar.moveDown.style.display = toolBar.separator.style.display = "none";
    }

    var conditionContainer = this.EditChartFormContainer(editChartForm, containerType, toolBar);
    panel.container = conditionContainer;
    panel.appendChild(conditionContainer);
    conditionContainer.style.margin = "12px 12px 0 12px";
    conditionContainer.style.width = "602px";
    conditionContainer.style.height = "400px";

    toolBar.add.action = function () {
        conditionContainer.addItem();
    }

    toolBar.moveUp.action = function () {
        if (conditionContainer.selectedItem)
            conditionContainer.selectedItem.move("Up");
    }

    toolBar.moveDown.action = function () {
        if (conditionContainer.selectedItem)
            conditionContainer.selectedItem.move("Down");
    }

    return panel;
}

StiMobileDesigner.prototype.EditChartFormFiltersPanel = function (editChartForm, hideFilterType) {
    var panel = document.createElement("div");

    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "6px 0 0 12px";
    toolBar.style.width = "calc(100% - 24px)";
    panel.appendChild(toolBar);

    //Toolbar
    var buttons = [
        ["add", this.loc.FormBand.AddFilter.replace("&", ""), null, null],
        ["separator"],
        ["moveUp", null, "Arrows.ArrowUpBlue.png", this.loc.QueryBuilder.MoveUp],
        ["moveDown", null, "Arrows.ArrowDownBlue.png", this.loc.QueryBuilder.MoveDown]
    ]

    for (var i = 0; i < buttons.length; i++) {
        if (buttons[i][0].indexOf("separator") >= 0) {
            toolBar[buttons[i][0]] = this.HomePanelSeparator();
            toolBar.addCell(toolBar[buttons[i][0]]).style.paddingRight = "4px";
            continue;
        }
        var buttonStyle = buttons[i][0] == "add" ? "stiDesignerFormButton" : "stiDesignerStandartSmallButton";
        var button = this.SmallButton(null, null, buttons[i][1], buttons[i][2], buttons[i][1] || buttons[i][3], buttons[i][4], buttonStyle, true);
        button.style.marginRight = "4px";
        toolBar[buttons[i][0]] = button;
        toolBar.addCell(button).style.width = "1px";
    }

    toolBar.moveUp.setEnabled(false);
    toolBar.moveDown.setEnabled(false);

    if (!this.options.isTouchDevice) {
        toolBar.moveUp.style.display = toolBar.moveDown.style.display = toolBar.separator.style.display = "none";
    }

    if (!hideFilterType) {
        var filterTypeText = toolBar.addCell();
        filterTypeText.className = "stiDesignerCaptionControls";
        filterTypeText.innerHTML = this.loc.PropertyMain.Type;
        filterTypeText.style.padding = "0 8px 0 0";

        toolBar.filterType = this.DropDownList("EditChartFormFilterType", 80, null, this.GetFilterTypeItems(), true);
        toolBar.addCell(toolBar.filterType).style.width = "1px";
    }

    //Container
    var filterContainer = this.EditChartFormContainer(editChartForm, "SeriesFilters", toolBar);
    panel.container = filterContainer;
    panel.appendChild(filterContainer);
    filterContainer.style.margin = "12px 12px 0 12px";
    filterContainer.style.width = "602px";
    filterContainer.style.height = "400px";

    toolBar.add.action = function () {
        filterContainer.addItem();
    }
    toolBar.moveUp.action = function () {
        if (filterContainer.selectedItem)
            filterContainer.selectedItem.move("Up");
    }
    toolBar.moveDown.action = function () {
        if (filterContainer.selectedItem)
            filterContainer.selectedItem.move("Down");
    }
    if (!hideFilterType) {
        toolBar.filterType.action = function () {
            filterContainer.isModified = true;
        }
    }

    return panel;
}

StiMobileDesigner.prototype.EditChartFormTrendLinesPanel = function (editChartForm) {
    var panel = document.createElement("div");

    //Toolbar
    var buttons = [
        ["add", this.loc.Chart.AddTrendLine, null, null],
        ["separator"],
        ["moveUp", null, "Arrows.ArrowUpBlue.png", this.loc.QueryBuilder.MoveUp],
        ["moveDown", null, "Arrows.ArrowDownBlue.png", this.loc.QueryBuilder.MoveDown]
    ]

    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "6px 0 0 12px";
    panel.appendChild(toolBar);

    for (var i = 0; i < buttons.length; i++) {
        if (buttons[i][0].indexOf("separator") >= 0) {
            toolBar[buttons[i][0]] = this.HomePanelSeparator();
            toolBar.addCell(toolBar[buttons[i][0]]).style.paddingRight = "4px";
            continue;
        }
        var buttonStyle = buttons[i][0] == "add" ? "stiDesignerFormButton" : "stiDesignerStandartSmallButton";
        var button = this.SmallButton(null, null, buttons[i][1], buttons[i][2], buttons[i][1] || buttons[i][3], buttons[i][4], buttonStyle, true);
        button.style.marginRight = "4px";
        toolBar[buttons[i][0]] = button;
        toolBar.addCell(button);
    }

    toolBar.moveUp.setEnabled(false);
    toolBar.moveDown.setEnabled(false);

    if (!this.options.isTouchDevice) {
        toolBar.moveUp.style.display = toolBar.moveDown.style.display = toolBar.separator.style.display = "none";
    }

    var trendContainer = this.EditChartFormTrendLinesContainer(editChartForm, toolBar);
    panel.container = trendContainer;
    panel.appendChild(trendContainer);
    trendContainer.style.margin = "12px 12px 0 12px";
    trendContainer.style.width = "602px";
    trendContainer.style.height = "400px";

    toolBar.add.action = function () {
        trendContainer.addItem();
    }
    toolBar.moveUp.action = function () {
        if (trendContainer.selectedItem)
            trendContainer.selectedItem.move("Up");
    }
    toolBar.moveDown.action = function () {
        if (trendContainer.selectedItem)
            trendContainer.selectedItem.move("Down");
    }

    return panel;
}

//Container
StiMobileDesigner.prototype.EditChartFormContainer = function (editChartForm, containerType, toolBar) {
    var container = document.createElement("div");
    var jsObject = container.jsObject = this;
    container.className = "stiDesignerChartFormContainer";
    container.style.position = "relative";
    container.selectedItem = null;
    container.isModified = false;
    container.containerType = containerType;
    container.toolBar = toolBar;
    container.editChartForm = editChartForm;

    container.addItems = function (conditionsObject) {
        this.clear();
        for (var i = 0; i < conditionsObject.length; i++) {
            this.addItem(conditionsObject[i]);
        }
        var items = container.getItems();
        if (items.length > 0) items[0].setSelected();
    }

    container.addItem = function (itemObject) {
        var item = containerType == "SeriesConditions" || containerType == "LabelsConditions"
            ? jsObject.EditChartFormConditionsContainerItem(container)
            : jsObject.EditChartFormFiltersContainerItem(container);
        this.appendChild(item);
        item.setSelected();
        this.onChange();
        var defaultItemObject = null;
        if (itemObject == null) {
            container.isModified = true;
            defaultItemObject = {
                FieldIs: "Argument",
                DataType: "String",
                Condition: "EqualTo",
                Value: ""
            }
            if (containerType == "SeriesConditions" || containerType == "LabelsConditions")
                defaultItemObject.Color = "255,255,255";

        }
        item.setKey(itemObject || defaultItemObject);

        item.remove = function () {
            var prevItem = item.previousSibling;
            var nextItem = item.nextSibling;
            container.removeChild(item);
            if (container.selectedItem == this) container.selectedItem = null;
            var items = container.getItems();
            if (items.length > 0) {
                if (nextItem) {
                    nextItem.setSelected();
                }
                else if (prevItem) {
                    prevItem.setSelected();
                }
                else items[0].setSelected();
            }
            container.onChange(items);
            container.isModified = true;
        }
    }

    container.getItems = function (itemObject) {
        var items = [];
        for (var i = 0; i < container.childNodes.length; i++) {
            if (!container.childNodes[i].isContainerItem) continue;
            items.push(container.childNodes[i]);
        }
        return items;
    }

    container.getValue = function () {
        var result = [];
        for (var i = 0; i < container.childNodes.length; i++) {
            if (!container.childNodes[i].isContainerItem) continue;
            result.push(container.childNodes[i].getValue());
        }
        return result;
    }

    container.sendValueToServer = function (andCloseForm) {
        var params = {};
        params.value = container.getValue();
        if (this.containerType == "SeriesFilters") params.filterMode = this.toolBar.filterType.key;
        params.componentName = editChartForm.chartProperties.name;
        params.containerType = this.containerType;
        if (this.containerType == "SeriesConditions" || this.containerType == "SeriesFilters") params.seriesIndex = editChartForm.seriesContainer.getSelectedIndex();
        if (andCloseForm) params.andCloseForm = andCloseForm;
        jsObject.SendCommandSendContainerValue(params);
    }

    container.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        container.selectedItem = null;
        container.onChange();
    }

    container.getCountItems = function () {
        var itemsCount = 0;
        for (var i = 0; i < this.childNodes.length; i++) {
            if (this.childNodes[i].key) itemsCount++;
        }
        return itemsCount;
    }

    container.getOverItemIndex = function () {
        for (var i = 0; i < this.childNodes.length; i++)
            if (this.childNodes[i].isOver) return i;

        return null;
    }

    container.getItemIndex = function (item) {
        for (var i = 0; i < this.childNodes.length; i++)
            if (this.childNodes[i] == item) return i;

        return null;
    }

    container.getItemByIndex = function (index) {
        if (index != null && !this.hintText && index >= 0 && index < this.childNodes.length) {
            return this.childNodes[index];
        }

        return null;
    }

    container.getSelectedItemIndex = function () {
        return this.selectedItem ? this.getItemIndex(this.selectedItem) : null;
    }

    container.moveItem = function (fromIndex, toIndex) {
        if (fromIndex < this.childNodes.length && toIndex < this.childNodes.length) {
            var fromItem = this.childNodes[fromIndex];
            if (fromIndex < toIndex) {
                if (toIndex < this.childNodes.length - 1) {
                    this.insertBefore(fromItem, this.childNodes[toIndex + 1]);
                }
                else {
                    this.appendChild(fromItem);
                }
            }
            else {
                this.insertBefore(fromItem, this.childNodes[toIndex]);
            }
            return fromItem;
        }
    }

    container.onmouseup = function (event) {
        if (event.button != 2 && jsObject.options.itemInDrag) {
            var itemObject = jsObject.CopyObject(jsObject.options.itemInDrag.itemObject);
            if (!itemObject) return;
            var typeItem = itemObject.typeItem;

            if (typeItem == "ChartFormContainerItem") {
                var toIndex = this.getOverItemIndex();
                var fromIndex = this.getSelectedItemIndex();
                if (toIndex != null && fromIndex != null && fromIndex != toIndex) {
                    container.moveItem(fromIndex, toIndex);
                    container.onChange();
                }
            }
        }

        return false;
    }

    container.onChange = function (items) {
        var items = items || container.getItems();
        if (toolBar.moveUp) toolBar.moveUp.setEnabled(container.selectedItem != null && container.selectedItem.getIndex() > 0);
        if (toolBar.moveDown) toolBar.moveDown.setEnabled(container.selectedItem != null && container.selectedItem.getIndex() < items.length - 1);
        this.checkEmptyPanel();
    }

    container.checkEmptyPanel = function () {
        var itemsCount = 0;
        var emptyPanel = null;
        for (var i = 0; i < this.childNodes.length; i++) {
            if (this.childNodes[i].key)
                itemsCount++;
            else
                emptyPanel = this.childNodes[i];
        }
        if (itemsCount > 0) {
            if (emptyPanel) this.removeChild(emptyPanel);
        }
        else {
            if (!emptyPanel) {
                emptyPanel = jsObject.EmptyTextPanel(containerType == "SeriesFilters" ? "BigFilter.png" : "BigConditions.png", containerType == "SeriesFilters" ? jsObject.loc.FormBand.NoFilters : jsObject.loc.Chart.NoConditions, "0.5");
                this.appendChild(emptyPanel);
            }
        }
    }

    return container;
}

//Container Item
StiMobileDesigner.prototype.EditChartFormContainerItem = function (container) {
    var item = document.createElement("div");
    var jsObject = item.jsObject = this;
    item.isSelected = false;
    item.isContainerItem = true;
    item.key = this.generateKey();
    item.className = "stiDesignerChartFilterPanel";

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "100%";
    item.appendChild(mainTable);
    var mainCell = mainTable.addCell();

    //Remove Button
    var removeButton = this.StandartSmallButton(null, null, null, "RemoveBlack.png");
    removeButton.style.visibility = "hidden";
    removeButton.style.display = "inline-block";
    removeButton.style.margin = "2px 2px 2px 0";
    item.removeButton = removeButton;

    var closeCell = mainTable.addCell(removeButton);
    closeCell.style.width = "1px";
    closeCell.style.verticalAlign = "top";

    removeButton.action = function () {
        item.remove();
    }

    item.innerTable = this.CreateHTMLTable();
    item.innerTable.style.margin = "6px 0 6px 6px";
    mainCell.appendChild(item.innerTable);

    //Captions
    item.fieldIsCaption = item.innerTable.addCell();
    item.fieldIsCaption.innerHTML = this.loc.PropertyMain.FieldIs;

    item.dataTypeCaption = item.innerTable.addCell();
    item.dataTypeCaption.innerHTML = this.loc.PropertyMain.DataType;

    item.conditionCaption = item.innerTable.addCell();
    item.conditionCaption.innerHTML = this.loc.PropertyMain.Condition;

    item.valueCaption = item.innerTable.addCell();
    item.valueCaption.innerHTML = this.loc.PropertyMain.Value;

    //FieldIs
    var seriesIndex = container.editChartForm.seriesContainer ? container.editChartForm.seriesContainer.getSelectedIndex() : -1;
    var seriesType = seriesIndex != -1 ? container.editChartForm.chartProperties.series[seriesIndex].type : null;
    var items = [];
    if (container.containerType != "SeriesFilters") {
        items = this.GetConditionsFieldIsItems();
    }
    else {
        if (seriesType == "StiGanttSeries") {
            items = this.GetSeriesGanttFiltersFieldIsItems();
        }
        else if (seriesType == "StiCandlestickSeries" || seriesType == "StiStockSeries") {
            items = this.GetSeriesFinancialFiltersFieldIsItems();
        }
        else {
            items = this.GetFiltersFieldIsItems();
        }
    }

    item.fieldIs = this.DropDownList(null, 110, null, items, true, false);
    item.fieldIs.style.margin = "3px 7px 3px 0";
    item.innerTable.addCellInNextRow(item.fieldIs);
    if (container.containerType == "SeriesFilters") item.fieldIs.action = function () { item.updateControls(); };

    //Data Type
    item.dataType = this.DropDownList(null, 110, null, this.GetConditionsDataTypeItems(), true, false);
    item.dataType.style.margin = "3px 7px 3px 0";
    item.innerTable.addCellInLastRow(item.dataType);
    item.dataType.action = function () { item.updateControls(); };

    //Condition
    item.condition = this.DropDownList(null, 110, null, null, true, false);
    item.condition.style.margin = "3px 7px 3px 0";
    item.innerTable.addCellInLastRow(item.condition);

    //Value
    item.value = this.ExpressionControl(null, 160, null, null, true);
    item.value.style.margin = "3px 7px 3px 0";
    item.value.button.style.display = "none"; //temporary
    item.value.button.parentElement.style.width = "3px"; //temporary
    var valueCell = item.innerTable.addCellInLastRow(item.value);

    //Value Date
    item.valueDate = this.DateControl(null, 143, null, true);
    item.valueDate.style.margin = "3px 7px 3px 0";
    valueCell.appendChild(item.valueDate);

    item.innerTable2 = this.CreateHTMLTable();
    item.innerTable2.style.margin = "0 0 6px 6px";
    mainCell.appendChild(item.innerTable2);

    item.setSelected = function () {
        for (var i = 0; i < container.childNodes.length; i++) {
            if (!container.childNodes[i].isContainerItem) continue;
            container.childNodes[i].className = "stiDesignerChartFilterPanel";
            container.childNodes[i].isSelected = false;
            container.childNodes[i].removeButton.style.visibility = "hidden";
        }
        item.isSelected = true;
        item.className = "stiDesignerChartFilterPanelSelected";
        item.removeButton.style.visibility = "visible";
        container.selectedItem = this;
    }

    item.getIndex = function () {
        var index = -1;
        for (var i = 0; i < container.childNodes.length; i++) {
            if (!container.childNodes[i].isContainerItem) continue;
            index++;
            if (container.childNodes[i] == this) return index;
        }
        return -1;
    }

    item.move = function (direction) {
        var items = container.getItems();
        var currentIndex = this.getIndex();
        var currentItem = items[currentIndex];
        container.removeChild(currentItem);

        if (direction == "Up") {
            currentIndex--;
            if (currentIndex != -1) container.insertBefore(currentItem, items[currentIndex]);
        }
        else {
            currentIndex++;
            if (currentIndex > items.length - 2)
                container.appendChild(currentItem);
            else
                container.insertBefore(currentItem, items[currentIndex + 1]);
        }

        container.onChange();
        container.isModified = true;
    }

    item.setKey = function (key) {
        if (key.FieldIs != null) item.fieldIs.setKey(key.FieldIs);
        if (key.DataType != null) item.dataType.setKey(key.DataType);
        if (key.Color != null) item.color.setKey(key.Color);
        if (key.Value != null) {
            var value = StiBase64.decode(key.Value);

            if (key.DataType == "DateTime")
                item.valueDate.setKey(value ? new Date(value) : new Date());
            else
                item.value.textBox.value = value;
        }
        item.updateControls();
        if (key.Condition != null) item.condition.setKey(key.Condition);
    }

    item.getValue = function () {
        var value = StiBase64.encode(item.dataType.key == "DateTime" ? jsObject.DateToStringAmericanFormat(item.valueDate.key, true) : item.value.textBox.value);

        var result = {
            FieldIs: item.fieldIs.key,
            DataType: item.dataType.key,
            Condition: item.condition.key,
            Value: value
        }
        if (container.containerType != "SeriesFilters") result.Color = item.color.key;

        return result;
    }

    if (jsObject.options.isTouchDevice) {
        item.onclick = function () {
            if (!this.parentElement) return;
            this.setSelected();
            container.onChange();
            container.isModified = true;
        }
    }
    else {
        item.onmousedown = function (event) {
            this.setSelected();
            container.onChange();
            container.isModified = true;

            if (this.isTouchStartFlag || (event && event.target && event.target.nodeName && event.target.nodeName.toLowerCase() == "input")) return;
            event.preventDefault();

            var options = jsObject.options;
            if (options.controlsIsFocused && options.controlsIsFocused.action) {
                options.controlsIsFocused.blur();
                options.controlsIsFocused = null;
            }

            if (event.button != 2 && !options.controlsIsFocused) {
                var itemInDrag = jsObject.TreeItemForDragDrop({ name: jsObject.loc.PropertyMain.Filter, typeItem: "ChartFormContainerItem" }, null, true);
                if (itemInDrag.button.captionCell) itemInDrag.button.captionCell.style.padding = "5px 20px 5px 10px";
                itemInDrag.beginingOffset = 0;
                options.itemInDrag = itemInDrag;
            }
        }

        item.onmouseover = function () {
            this.isOver = true;
            if (jsObject.options.itemInDrag && jsObject.options.itemInDrag.itemObject) {
                var typeItem = jsObject.options.itemInDrag.itemObject.typeItem;
                if (typeItem == "ChartFormContainerItem") {
                    this.style.borderStyle = "dashed";
                    this.style.borderColor = jsObject.options.themeColors[jsObject.GetThemeColor()];
                }
            }
        }

        item.onmouseout = function () {
            this.isOver = false;
            this.style.borderStyle = "solid";
            this.style.borderColor = "";
        }
    }

    return item;
}

//Filters Container Item
StiMobileDesigner.prototype.EditChartFormFiltersContainerItem = function (container) {
    var item = this.EditChartFormContainerItem(container);

    item.updateControls = function () {
        this.condition.addItems(this.jsObject.GetFilterConditionItems(item.dataType.key, true));
        if (!this.condition.haveKey(this.condition.key) && this.condition.items != null && this.condition.items.length > 0) this.condition.setKey(this.condition.items[0].key);
        this.value.style.display = (this.dataType.key != "DateTime") ? "" : "none";
        this.valueDate.style.display = (this.dataType.key == "DateTime") ? "" : "none";
        this.dataType.setEnabled(this.fieldIs.key != "Expression");
        this.condition.setEnabled(this.fieldIs.key != "Expression");
    }

    return item;
}

//Conditions Container Item
StiMobileDesigner.prototype.EditChartFormConditionsContainerItem = function (container) {
    var item = this.EditChartFormContainerItem(container);

    //Color Caption
    item.colorCaption = item.innerTable2.addCellInNextRow();
    item.colorCaption.innerHTML = this.loc.PropertyMain.Color;

    //Color
    item.color = this.ColorControl(null, null, null, 110, true);
    item.color.style.margin = "3px 7px 3px 0";
    item.innerTable2.addCellInNextRow(item.color);

    item.updateControls = function () {
        this.condition.addItems(this.jsObject.GetFilterConditionItems(item.dataType.key, true));
        if (!this.condition.haveKey(this.condition.key) && this.condition.items != null && this.condition.items.length > 0) this.condition.setKey(this.condition.items[0].key);
        this.value.style.display = (this.dataType.key != "DateTime") ? "" : "none";
        this.valueDate.style.display = (this.dataType.key == "DateTime") ? "" : "none";
    }

    return item;
}

//Container
StiMobileDesigner.prototype.EditChartFormTrendLinesContainer = function (editChartForm, toolBar) {
    var container = document.createElement("div");
    var jsObject = container.jsObject = this;
    container.className = "stiDesignerChartFormContainer";
    container.style.position = "relative";
    container.selectedItem = null;
    container.isModified = false;
    container.editChartForm = editChartForm;

    container.addItems = function (lineObject) {
        this.clear();
        for (var i = 0; i < lineObject.length; i++) {
            this.addItem(lineObject[i]);
        }
        var items = container.getItems();
        if (items.length > 0) items[0].setSelected();
    }

    container.addItem = function (itemObject) {
        var item = jsObject.EditChartFormTrendLinesContainerItem(container);
        this.appendChild(item);
        item.setSelected();
        this.onChange();
        var defaultItemObject = null;
        if (itemObject == null) {
            container.isModified = true;
            defaultItemObject = {
                Type: "Linear",
                LineColor: "0,0,0",
                LineStyle: "Solid",
                LineWidth: "1",
                Text: "",
                Position: "LeftBottom",
                Font: "Arial!7!0!0!0!0",
                AllowApplyStyle: true
            }
        }
        item.setKey(itemObject || defaultItemObject);

        item.remove = function () {
            var prevItem = item.previousSibling;
            var nextItem = item.nextSibling;
            container.removeChild(item);
            if (container.selectedItem == this) container.selectedItem = null;
            var items = container.getItems();
            if (items.length > 0) {
                if (nextItem) {
                    nextItem.setSelected();
                }
                else if (prevItem) {
                    prevItem.setSelected();
                }
                else items[0].setSelected();
            }
            container.onChange(items);
            container.isModified = true;
        }
    }

    container.getItems = function () {
        var items = [];
        for (var i = 0; i < container.childNodes.length; i++) {
            if (!container.childNodes[i].isContainerItem) continue;
            items.push(container.childNodes[i]);
        }
        return items;
    }

    container.getValue = function () {
        var result = [];
        for (var i = 0; i < container.childNodes.length; i++) {
            if (!container.childNodes[i].isContainerItem) continue;
            result.push(container.childNodes[i].getValue());
        }
        return result;
    }

    container.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        container.selectedItem = null;
        container.onChange();
    }

    container.getCountItems = function () {
        var itemsCount = 0;
        for (var i = 0; i < this.childNodes.length; i++) {
            if (this.childNodes[i].key) itemsCount++;
        }
        return itemsCount;
    }

    container.getOverItemIndex = function () {
        for (var i = 0; i < this.childNodes.length; i++)
            if (this.childNodes[i].isOver) return i;

        return null;
    }

    container.getItemIndex = function (item) {
        for (var i = 0; i < this.childNodes.length; i++)
            if (this.childNodes[i] == item) return i;

        return null;
    }

    container.getItemByIndex = function (index) {
        if (index != null && !this.hintText && index >= 0 && index < this.childNodes.length) {
            return this.childNodes[index];
        }

        return null;
    }

    container.getSelectedItemIndex = function () {
        return this.selectedItem ? this.getItemIndex(this.selectedItem) : null;
    }

    container.moveItem = function (fromIndex, toIndex) {
        if (fromIndex < this.childNodes.length && toIndex < this.childNodes.length) {
            var fromItem = this.childNodes[fromIndex];
            if (fromIndex < toIndex) {
                if (toIndex < this.childNodes.length - 1) {
                    this.insertBefore(fromItem, this.childNodes[toIndex + 1]);
                }
                else {
                    this.appendChild(fromItem);
                }
            }
            else {
                this.insertBefore(fromItem, this.childNodes[toIndex]);
            }
            container.isModified = true;
            return fromItem;
        }
    }

    container.onmouseup = function (event) {
        if (event.button != 2 && jsObject.options.itemInDrag) {
            var itemObject = jsObject.CopyObject(jsObject.options.itemInDrag.itemObject);
            if (!itemObject) return;
            var typeItem = itemObject.typeItem;

            if (typeItem == "ChartFormContainerItem") {
                var toIndex = this.getOverItemIndex();
                var fromIndex = this.getSelectedItemIndex();
                if (toIndex != null && fromIndex != null && fromIndex != toIndex) {
                    container.moveItem(fromIndex, toIndex);
                    container.onChange();
                }
            }
        }

        return false;
    }

    container.sendValueToServer = function (andCloseForm) {
        var params = {};
        params.value = container.getValue();
        params.componentName = editChartForm.chartProperties.name;
        params.containerType = "SeriesTrendLines";
        params.seriesIndex = editChartForm.seriesContainer.getSelectedIndex();
        if (andCloseForm) params.andCloseForm = andCloseForm;
        jsObject.SendCommandSendContainerValue(params);
    }

    container.onChange = function (items) {
        var items = items || container.getItems();
        if (toolBar.moveUp) toolBar.moveUp.setEnabled(container.selectedItem != null && container.selectedItem.getIndex() > 0);
        if (toolBar.moveDown) toolBar.moveDown.setEnabled(container.selectedItem != null && container.selectedItem.getIndex() < items.length - 1);
        this.checkEmptyPanel();
    }

    container.checkEmptyPanel = function () {
        var itemsCount = 0;
        var emptyPanel = null;
        for (var i = 0; i < this.childNodes.length; i++) {
            if (this.childNodes[i].key)
                itemsCount++;
            else
                emptyPanel = this.childNodes[i];
        }
        if (itemsCount > 0) {
            if (emptyPanel) this.removeChild(emptyPanel);
        }
        else {
            if (!emptyPanel) {
                emptyPanel = jsObject.EmptyTextPanel("Series_TrendLine.png", jsObject.loc.PropertyMain.NoElements, "0.5");
                this.appendChild(emptyPanel);
            }
        }
    }

    return container;
}

StiMobileDesigner.prototype.EditChartFormTrendLinesContainerItem = function (container) {
    var item = document.createElement("div");
    var jsObject = item.jsObject = this;
    item.isSelected = false;
    item.isContainerItem = true;
    item.key = this.generateKey();
    item.className = "stiDesignerChartFilterPanel";

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "100%";
    item.appendChild(mainTable);
    var mainCell = mainTable.addCell();

    //Remove Button
    var removeButton = this.StandartSmallButton(null, null, null, "RemoveBlack.png");
    removeButton.style.visibility = "hidden";
    removeButton.style.display = "inline-block";
    removeButton.style.margin = "2px 2px 2px 0";
    item.removeButton = removeButton;

    var closeCell = mainTable.addCell(removeButton);
    closeCell.style.width = "1px";
    closeCell.style.verticalAlign = "top";

    removeButton.action = function () {
        item.remove();
    }

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "6px 0 0 6px";
    mainCell.appendChild(innerTable);

    //Captions
    innerTable.addTextCell(this.loc.PropertyMain.Type);
    innerTable.addTextCell(this.loc.PropertyMain.Color);
    innerTable.addTextCell(this.loc.PropertyMain.Style);
    innerTable.addTextCell(this.loc.PropertyMain.Width);

    var controlsWidth = this.options.isTouchDevice ? 95 : 105;

    //Type
    var typeControl = this.DropDownList(null, controlsWidth, null, this.GetTrendLinesTypeItems(), true);
    typeControl.style.margin = "3px 6px 3px 0";
    innerTable.addCellInNextRow(typeControl);

    //LineColor
    var lineColorControl = this.ColorControl(null, null, true, controlsWidth, true);
    lineColorControl.style.margin = "3px 6px 3px 0";
    innerTable.addCellInLastRow(lineColorControl);

    //LineStyle
    var lineStyleControl = this.DropDownList(null, controlsWidth, null, this.GetLineStyleItems(), true, true);
    lineStyleControl.style.margin = "3px 6px 3px 0";
    innerTable.addCellInLastRow(lineStyleControl);

    //LineWidth
    var lineWidthControl = this.TextBoxPositiveIntValue(null, controlsWidth);
    lineWidthControl.style.margin = "3px 6px 3px 0";
    innerTable.addCellInLastRow(lineWidthControl);

    innerTable.addTextCellInNextRow(this.loc.PropertyMain.Text).style.paddingTop = "6px";
    innerTable.addTextCellInLastRow(this.loc.PropertyMain.Position).style.paddingTop = "6px";
    innerTable.addTextCellInLastRow(this.loc.PropertyMain.Font).style.paddingTop = "6px";
    innerTable.addCellInLastRow();

    //Text
    var textControl = this.TextBox(null, controlsWidth);
    textControl.style.margin = "3px 6px 3px 0";
    innerTable.addCellInNextRow(textControl);

    //Position
    var positionControl = this.PropertyDropDownList(null, controlsWidth, this.GetTrendLinesPositionItems(), true);
    positionControl.style.margin = "3px 6px 3px 0";
    innerTable.addCellInLastRow(positionControl);

    //FontName
    var fontNameControl = this.FontList(null, controlsWidth);
    fontNameControl.style.margin = "3px 6px 3px 0";
    innerTable.addCellInLastRow(fontNameControl);

    //FontStyle
    var fontStyleTable = this.CreateHTMLTable();
    fontStyleTable.style.margin = "3px 6px 3px 0";
    innerTable.addCellInLastRow(fontStyleTable);

    var fontSizeControl = this.DropDownList(null, 40, null, this.GetFontSizeItems(), false);
    fontSizeControl.style.marginRight = "6px";
    fontStyleTable.addCell(fontSizeControl);

    var margin = this.options.isTouchDevice ? "1px" : "3px";

    var buttonBold = this.StandartSmallButton(null, null, null, "Bold.png");
    buttonBold.style.marginRight = margin;
    fontStyleTable.addCell(buttonBold);
    buttonBold.action = function () {
        this.setSelected(!this.isSelected);
    };

    var buttonItalic = this.StandartSmallButton(null, null, null, "Italic.png");
    buttonItalic.style.marginRight = margin;
    fontStyleTable.addCell(buttonItalic);
    buttonItalic.action = function () {
        this.setSelected(!this.isSelected);
    };

    var buttonUnderline = this.StandartSmallButton(null, null, null, "Underline.png");
    buttonUnderline.style.marginRight = margin;
    fontStyleTable.addCell(buttonUnderline);
    buttonUnderline.action = function () {
        this.setSelected(!this.isSelected);
    };

    var buttonStrikeout = this.StandartSmallButton(null, null, null, "Strikeout.png");
    buttonStrikeout.style.marginRight = margin;
    fontStyleTable.addCell(buttonStrikeout);
    buttonStrikeout.action = function () {
        this.setSelected(!this.isSelected);
    };

    //AllowApplyStyle
    var allowApplyStyleControl = this.CheckBox(null, this.loc.PropertyMain.AllowApplyStyle);
    allowApplyStyleControl.style.margin = "6px 6px 12px 0";
    innerTable.addCellInNextRow(allowApplyStyleControl).setAttribute("colspan", "2");

    allowApplyStyleControl.action = function () {
        lineColorControl.setEnabled(!this.isChecked);
    }

    item.setSelected = function () {
        for (var i = 0; i < container.childNodes.length; i++) {
            if (!container.childNodes[i].isContainerItem) continue;
            container.childNodes[i].className = "stiDesignerChartFilterPanel";
            container.childNodes[i].isSelected = false;
            container.childNodes[i].removeButton.style.visibility = "hidden";
        }
        item.isSelected = true;
        item.className = "stiDesignerChartFilterPanelSelected";
        item.removeButton.style.visibility = "visible";
        container.selectedItem = this;
    }

    item.getIndex = function () {
        var index = -1;
        for (var i = 0; i < container.childNodes.length; i++) {
            if (!container.childNodes[i].isContainerItem) continue;
            index++;
            if (container.childNodes[i] == this) return index;
        }
        return -1;
    }

    item.move = function (direction) {
        var items = container.getItems();
        var currentIndex = this.getIndex();
        var currentItem = items[currentIndex];
        container.removeChild(currentItem);

        if (direction == "Up") {
            currentIndex--;
            if (currentIndex != -1) container.insertBefore(currentItem, items[currentIndex]);
        }
        else {
            currentIndex++;
            if (currentIndex > items.length - 2)
                container.appendChild(currentItem);
            else
                container.insertBefore(currentItem, items[currentIndex + 1]);
        }

        container.onChange();
        container.isModified = true;
    }

    item.setKey = function (key) {
        typeControl.setKey(key.Type);
        lineColorControl.setKey(key.LineColor);
        lineStyleControl.setKey(key.LineStyle);
        lineWidthControl.value = key.LineWidth;
        textControl.value = StiBase64.decode(key.Text);
        positionControl.setKey(key.Position);
        allowApplyStyleControl.setChecked(key.AllowApplyStyle);
        var font = jsObject.FontStrToObject(key.Font);
        fontNameControl.setKey(font.name);
        fontSizeControl.setKey(font.size);
        buttonBold.setSelected(font.bold == "1");
        buttonItalic.setSelected(font.italic == "1");
        buttonUnderline.setSelected(font.underline == "1");
        buttonStrikeout.setSelected(font.strikeout == "1");
        lineColorControl.setEnabled(!key.AllowApplyStyle);
    }

    item.getValue = function () {
        var result = {
            Type: typeControl.key,
            LineColor: lineColorControl.key,
            LineStyle: lineStyleControl.key,
            LineWidth: lineWidthControl.value,
            Text: StiBase64.encode(textControl.value),
            Position: positionControl.key,
            Font: jsObject.FontObjectToStr({
                name: fontNameControl.key,
                size: fontSizeControl.key,
                bold: buttonBold.isSelected ? "1" : "0",
                italic: buttonItalic.isSelected ? "1" : "0",
                underline: buttonUnderline.isSelected ? "1" : "0",
                strikeout: buttonStrikeout.isSelected ? "1" : "0",
            }),
            AllowApplyStyle: allowApplyStyleControl.isChecked
        }
        return result;
    }

    if (jsObject.options.isTouchDevice) {
        item.onclick = function () {
            if (!this.parentElement) return;
            this.setSelected();
            container.onChange();
            container.isModified = true;
        }
    }
    else {
        item.onmousedown = function (event) {
            this.setSelected(true);
            container.onChange();
            container.isModified = true;

            if (this.isTouchStartFlag || (event && event.target && event.target.nodeName && event.target.nodeName.toLowerCase() == "input")) return;
            event.preventDefault();

            var options = jsObject.options;
            if (options.controlsIsFocused && options.controlsIsFocused.action) {
                options.controlsIsFocused.blur();
                options.controlsIsFocused = null;
            }

            if (event.button != 2 && !options.controlsIsFocused) {
                var itemInDrag = jsObject.TreeItemForDragDrop({ name: jsObject.loc.PropertyMain.TrendLine, typeItem: "ChartFormContainerItem" }, null, true);
                if (itemInDrag.button.captionCell) itemInDrag.button.captionCell.style.padding = "5px 20px 5px 10px";
                itemInDrag.beginingOffset = 0;
                options.itemInDrag = itemInDrag;
            }
        }

        item.onmouseover = function () {
            this.isOver = true;
            if (jsObject.options.itemInDrag && jsObject.options.itemInDrag.itemObject) {
                var typeItem = jsObject.options.itemInDrag.itemObject.typeItem;
                if (typeItem == "ChartFormContainerItem") {
                    this.style.borderStyle = "dashed";
                    this.style.borderColor = jsObject.options.themeColors[jsObject.GetThemeColor()];
                }
            }
        }

        item.onmouseout = function () {
            this.isOver = false;
            this.style.borderStyle = "solid";
            this.style.borderColor = "";
        }
    }

    return item;
}

StiMobileDesigner.prototype.SeriesWizardBigButton = function (name, caption, imageName) {
    var button = this.BigButton(name, null, caption, imageName, null, null, "stiDesignerStandartBigButton", true, null, { width: 170, height: 170 });
    button.style.display = "inline-block";
    button.cellImage.style.padding = "4px";
    button.caption.style.padding = "0 4px 4px 4px";
    button.image.style.border = "1px solid #c6c6c6";

    return button;
}

StiMobileDesigner.prototype.SeriesWizardContainer = function (editChartForm) {
    var seriesWizardContainer = document.createElement("div");
    editChartForm.seriesWizardContainer = seriesWizardContainer;
    seriesWizardContainer.className = "stiDesignerChartFormSeriesWizardContainer";
    seriesWizardContainer.style.width = "750px";
    seriesWizardContainer.style.height = "460px";

    var series = [
        "StiClusteredColumnSeries",
        "StiStackedColumnSeries",
        "StiFullStackedColumnSeries",
        "StiParetoSeries",
        "StiRibbonSeries",
        "StiClusteredBarSeries",
        "StiStackedBarSeries",
        "StiFullStackedBarSeries",
        "StiPieSeries",
        "StiPie3dSeries",
        "StiDoughnutSeries",
        "StiLineSeries",
        "StiSteppedLineSeries",
        "StiStackedLineSeries",
        "StiFullStackedLineSeries",
        "StiSplineSeries",
        "StiStackedSplineSeries",
        "StiFullStackedSplineSeries",
        "StiAreaSeries",
        "StiSteppedAreaSeries",
        "StiStackedAreaSeries",
        "StiFullStackedAreaSeries",
        "StiSplineAreaSeries",
        "StiStackedSplineAreaSeries",
        "StiFullStackedSplineAreaSeries",
        "StiGanttSeries",
        "StiScatterSeries",
        "StiBubbleSeries",
        "StiRadarPointSeries",
        "StiRadarLineSeries",
        "StiRadarAreaSeries",
        "StiRangeSeries",
        "StiSteppedRangeSeries",
        "StiRangeBarSeries",
        "StiSplineRangeSeries",
        "StiFunnelSeries",
        "StiCandlestickSeries",
        "StiStockSeries",
        "StiTreemapSeries",
        "StiWaterfallSeries",
        "StiPictorialSeries",
        "StiPictorialStackedSeries"
    ]

    for (var i = 0; i < series.length; i++) {
        var seriesType = series[i].replace("Sti", "").replace("Series", "");
        var caption = series[i] == "StiPie3dSeries" ? ("3D " + this.loc.Chart.Pie) : this.loc.Chart[seriesType];
        var button = this.SeriesWizardBigButton(null, caption, "Big" + series[i] + ".png");
        button.seriesType = series[i];
        button.style.margin = "10px 0px 10px 0px";
        button.style.maxWidth = "180px";
        button.caption.style.height = "25px";
        button.cellImage.style.padding = "4px 4px 0 4px";
        seriesWizardContainer.appendChild(button);

        button.action = function () {
            editChartForm.seriesWizardContainer.style.display = "none";
            editChartForm.chartTabMainTable.style.display = "";

            this.jsObject.SendCommandAddSeries({
                componentName: editChartForm.chartProperties.name,
                seriesType: this.seriesType
            });
        }
    }

    return seriesWizardContainer;
}