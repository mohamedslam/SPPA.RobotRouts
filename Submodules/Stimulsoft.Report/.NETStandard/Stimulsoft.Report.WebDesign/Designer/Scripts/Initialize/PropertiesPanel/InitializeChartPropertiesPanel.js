
StiMobileDesigner.prototype.ChartPropertiesPanel = function () {
    var chartPropertiesPanel = document.createElement("div");
    var jsObject = chartPropertiesPanel.jsObject = this;
    chartPropertiesPanel.style.display = "none";

    //Add Inner Panels    
    var panelNames = [
        ["Chart", ["Common", "Legend", "Title", "ConstantLines", "Strips", "Table"]],
        ["Series", ["Common", "Conditions", "Filters", "Marker", "LineMarker", "Interaction", "TrendLine", "TopN", "SeriesLabels"]],
        ["Area", ["AllProperties"]],
        ["Labels", ["Common", "Conditions"]]];

    var showPanel = function (panelName, panels) {
        for (var name in panels) {
            panels[name].style.display = (name == panelName) ? "" : "none";
        }
    }

    chartPropertiesPanel.innerPanels = {};

    for (var i = 0; i < panelNames.length; i++) {
        var parentPanel = document.createElement("div");
        chartPropertiesPanel.appendChild(parentPanel);
        parentPanel.style.display = "none";
        var parentName = panelNames[i][0];
        parentPanel.panelName = parentName;
        chartPropertiesPanel.innerPanels[parentName] = parentPanel;
        parentPanel.innerPanels = {};
        var childNames = panelNames[i][1];

        parentPanel.showInnerPanel = function (panelName) {
            showPanel(panelName, this.innerPanels);
            chartPropertiesPanel.currentChildPanelName = panelName;
            chartPropertiesPanel.currentParentPanelName = this.panelName;

            //Update Properties Grid
            var propertiesValues = null;
            var editChartForm = jsObject.options.forms.editChart;

            switch (this.panelName) {
                case "Chart":
                    {
                        if (panelName == "Common") {
                            this.innerPanels["Common"].updateDataControls();
                        }
                        if (panelName == "ConstantLines") {
                            var selectedIndexConstantLines = editChartForm.ConstantLinesContainer.getSelectedIndex();
                            if (selectedIndexConstantLines != -1) propertiesValues = editChartForm.chartProperties.constantLines[selectedIndexConstantLines].properties;
                        }
                        else if (panelName == "Strips") {
                            var selectedIndexStrips = editChartForm.StripsContainer.getSelectedIndex();
                            if (selectedIndexStrips != -1) propertiesValues = editChartForm.chartProperties.strips[selectedIndexStrips].properties;
                        }
                        else {
                            propertiesValues = editChartForm.chartProperties.properties[panelName];
                        }

                        break;
                    }
                case "Series":
                    {
                        var seriesIndex = editChartForm.seriesContainer.getSelectedIndex();
                        if (seriesIndex != -1) {
                            if (panelName != "SeriesLabels") {
                                propertiesValues = editChartForm.chartProperties.series[seriesIndex].properties[panelName];
                            }
                            else {
                                propertiesValues = editChartForm.chartProperties.series[seriesIndex].labels.properties.Common;
                            }
                        }
                        if (panelName == "Interaction") {
                            var drillDownPageControl = this.innerPanels["Interaction"].groups["Main"].properties["DrillDownPage"].control;
                            if (drillDownPageControl) drillDownPageControl.addItems(jsObject.GetDrillDownPageItems());
                        }
                        break;
                    }
                case "Area":
                    {
                        panelName = "AllProperties";
                        var buttonName = editChartForm.areaPropertiesContainer.selectedItem ? editChartForm.areaPropertiesContainer.selectedItem.name : "";
                        propertiesValues = editChartForm.chartProperties.area.properties[buttonName];
                        break;
                    }
                case "Labels":
                    {
                        propertiesValues = editChartForm.chartProperties.labels.properties[panelName];
                        break;
                    }
            }
            chartPropertiesPanel.updateProperties(this.innerPanels[panelName].groups, propertiesValues);
            chartPropertiesPanel.updatePropertiesUsedInStyles();
            if (this.panelName == "Chart") this.innerPanels["Common"].updateDataRelationControl();
        }

        for (var k = 0; k < childNames.length; k++) {
            var childPanel = document.createElement("div");
            childPanel.style.display = "none";
            parentPanel.appendChild(childPanel);
            var childName = childNames[k];
            childPanel.panelName = childName;
            parentPanel.innerPanels[childName] = childPanel;
        }
    }

    chartPropertiesPanel.showInnerPanel = function (panelName) {
        showPanel(panelName, this.innerPanels);
    }

    chartPropertiesPanel.updatePropertiesCaptions = function () {
        for (var panelName in this.innerPanels) {
            var childPanels = this.innerPanels[panelName].innerPanels;
            for (var childPanelName in childPanels) {
                var groups = childPanels[childPanelName].groups;
                for (var groupName in groups) {
                    var propertyGroup = groups[groupName];
                    for (var propertyName in propertyGroup.properties) {
                        var property = propertyGroup.properties[propertyName];
                        property.updateCaption();
                    }
                }
            }
        }
    }

    chartPropertiesPanel.updateProperties = function (propertiesGroups, propertiesValues) {
        if (propertiesValues == null) propertiesValues = {};

        for (var groupName in propertiesGroups) {
            var propertyGroup = propertiesGroups[groupName];
            var showGroup = false;

            for (var propertyName in propertyGroup.properties) {
                var property = propertyGroup.properties[propertyName];
                if (property.isUserProperty) {
                    var showProperty = propertiesValues[property.name] != null;
                    property.style.display = showProperty ? "" : "none";
                    property.isVisible = showProperty;
                    if (showProperty) {
                        showGroup = true;
                        jsObject.SetPropertyValue(property, propertiesValues[property.name]);
                    }
                }
            }
            propertyGroup.style.display = showGroup ? "" : "none";

            if (propertyGroup.innerGroups) {
                chartPropertiesPanel.updateProperties(propertyGroup.innerGroups, propertiesValues);
            }
        }
    }

    chartPropertiesPanel.updatePropertiesUsedInStyles = function () {
        if (jsObject.options.showPropertiesWhichUsedFromStyles) return;

        var changeVisibleStateProperty = function (parentPanelName, childPanelName, groupName, propertyName, visibleState) {
            try {
                var property = chartPropertiesPanel.innerPanels[parentPanelName].innerPanels[childPanelName].groups[groupName].properties[propertyName];
                if (property) {
                    if (!property.isVisible) return;
                    chartPropertiesPanel.innerPanels[parentPanelName].innerPanels[childPanelName].groups[groupName].properties[propertyName].style.display = visibleState ? "" : "none";
                }
            }
            catch (e) {
                return e.Message;
            }
        }

        var parentPanelName = this.currentParentPanelName || "Series";
        var childPanelName = this.currentChildPanelName || "Common";

        if (parentPanelName == "Series" && childPanelName == "Common") {
            var allowApplyStyle = this.innerPanels["Series"].innerPanels["Common"].groups["Appearance"].properties["AllowApplyStyle"].control.isChecked;
            changeVisibleStateProperty("Series", "Common", "Appearance", "Brush", !allowApplyStyle);
            changeVisibleStateProperty("Series", "Common", "Appearance", "BorderColor", !allowApplyStyle);
            changeVisibleStateProperty("Series", "Common", "Appearance", "BorderColorNegative", !allowApplyStyle);
            changeVisibleStateProperty("Series", "Common", "Appearance", "BorderThickness", !allowApplyStyle);
            changeVisibleStateProperty("Series", "Common", "Appearance", "CornerRadus", !allowApplyStyle);
            changeVisibleStateProperty("Series", "Common", "Appearance", "Lighting", !allowApplyStyle);
            changeVisibleStateProperty("Series", "Common", "Appearance", "ShowShadow", !allowApplyStyle);
            var allowApplyBrushNegative = this.innerPanels["Series"].innerPanels["Common"].groups["Appearance"].properties["AllowApplyBrushNegative"].control.isChecked;
            changeVisibleStateProperty("Series", "Common", "Appearance", "BrushNegative", !allowApplyBrushNegative);
            var allowApplyColorNegative = this.innerPanels["Series"].innerPanels["Common"].groups["Appearance"].properties["AllowApplyColorNegative"].control.isChecked;
            changeVisibleStateProperty("Series", "Common", "Appearance", "LineColorNegative", allowApplyColorNegative);
            var allowApplyLineColorProperty = this.innerPanels["Series"].innerPanels["Common"].groups["Appearance"].properties["AllowApplyLineColor"];
            var allowApplyLineColor = allowApplyLineColorProperty.control.isChecked;
            changeVisibleStateProperty("Series", "Common", "Appearance", "LineColor", allowApplyLineColorProperty.style.display == "" ? !allowApplyLineColor : !allowApplyStyle);
        }
        if (parentPanelName == "Series" && childPanelName == "SeriesLabels") {
            var allowApplyStyle = this.innerPanels["Series"].innerPanels["SeriesLabels"].groups["Main"].properties["AllowApplyStyle"].control.isChecked;
            changeVisibleStateProperty("Series", "SeriesLabels", "Main", "LineColor", !allowApplyStyle);
            changeVisibleStateProperty("Series", "SeriesLabels", "Main", "Antialiasing", !allowApplyStyle);
            changeVisibleStateProperty("Series", "SeriesLabels", "Main", "LabelColor", !allowApplyStyle);
            changeVisibleStateProperty("Series", "SeriesLabels", "Main", "BorderColor", !allowApplyStyle);
            changeVisibleStateProperty("Series", "SeriesLabels", "Main", "Brush", !allowApplyStyle);
            changeVisibleStateProperty("Series", "SeriesLabels", "Main", "Font", !allowApplyStyle);
        }
        if (parentPanelName == "Chart" && childPanelName == "Table") {
            var allowApplyStyle = this.innerPanels["Chart"].innerPanels["Table"].groups["Main"].properties["AllowApplyStyle"].control.isChecked;
            changeVisibleStateProperty("Chart", "Table", "DataCells", "DataCells.TextColor", !allowApplyStyle);
            changeVisibleStateProperty("Chart", "Table", "Main", "GridLineColor", !allowApplyStyle);
            changeVisibleStateProperty("Chart", "Table", "Main", "Font", !allowApplyStyle);
        }
        if (parentPanelName == "Chart" && childPanelName == "ConstantLines") {
            var allowApplyStyle = this.innerPanels["Chart"].innerPanels["ConstantLines"].groups["Chart"].properties["AllowApplyStyle"].control.isChecked;
            changeVisibleStateProperty("Chart", "ConstantLines", "Title", "Antialiasing", !allowApplyStyle);
            changeVisibleStateProperty("Chart", "ConstantLines", "Title", "Font", !allowApplyStyle);
            changeVisibleStateProperty("Chart", "ConstantLines", "Behavior", "LineColor", !allowApplyStyle);
        }
        if (parentPanelName == "Chart" && childPanelName == "Strips") {
            var allowApplyStyle = this.innerPanels["Chart"].innerPanels["Strips"].groups["Chart"].properties["AllowApplyStyle"].control.isChecked;
            changeVisibleStateProperty("Chart", "Strips", "Behavior", "StripBrush", !allowApplyStyle);
            changeVisibleStateProperty("Chart", "Strips", "Title", "Antialiasing", !allowApplyStyle);
            changeVisibleStateProperty("Chart", "Strips", "Title", "Font", !allowApplyStyle);
            changeVisibleStateProperty("Chart", "Strips", "Title", "TitleColor", !allowApplyStyle);
        }
        if (parentPanelName == "Chart" && childPanelName == "Legend") {
            var allowApplyStyle = this.innerPanels["Chart"].innerPanels["Legend"].groups["Main"].properties["AllowApplyStyle"].control.isChecked;
            changeVisibleStateProperty("Chart", "Legend", "Main", "ShowShadow", !allowApplyStyle);
            changeVisibleStateProperty("Chart", "Legend", "Main", "BorderColor", !allowApplyStyle);
            changeVisibleStateProperty("Chart", "Legend", "Main", "Brush", !allowApplyStyle);
            changeVisibleStateProperty("Chart", "Legend", "Main", "TitleColor", !allowApplyStyle);
            changeVisibleStateProperty("Chart", "Legend", "Main", "LabelsColor", !allowApplyStyle);
            changeVisibleStateProperty("Chart", "Legend", "Main", "TitleFont", !allowApplyStyle);
            changeVisibleStateProperty("Chart", "Legend", "Main", "Font", !allowApplyStyle);
        }
        if (parentPanelName == "Chart" && childPanelName == "Title") {
            var allowApplyStyle = this.innerPanels["Chart"].innerPanels["Title"].groups["Main"].properties["AllowApplyStyle"].control.isChecked;
            changeVisibleStateProperty("Chart", "Title", "Main", "Antialiasing", !allowApplyStyle);
            changeVisibleStateProperty("Chart", "Title", "Main", "Brush", !allowApplyStyle);
            changeVisibleStateProperty("Chart", "Title", "Main", "Font", !allowApplyStyle);
        }
        if (parentPanelName == "Area") {
            var allowApplyStyleMain = this.innerPanels["Area"].innerPanels["AllProperties"].groups["Main"].properties["AllowApplyStyle"].control.isChecked;
            changeVisibleStateProperty("Area", "AllProperties", "Main", "Brush", !allowApplyStyleMain);
            changeVisibleStateProperty("Area", "AllProperties", "Main", "BorderColor", !allowApplyStyleMain);
            changeVisibleStateProperty("Area", "AllProperties", "Main", "ShowShadow", !allowApplyStyleMain);
            changeVisibleStateProperty("Area", "AllProperties", "Main", "LineColor", !allowApplyStyleMain);
            changeVisibleStateProperty("Area", "AllProperties", "Main", "LineStyle", !allowApplyStyleMain);
            changeVisibleStateProperty("Area", "AllProperties", "Main", "MinorColor", !allowApplyStyleMain);
            changeVisibleStateProperty("Area", "AllProperties", "Main", "MinorStyle", !allowApplyStyleMain);
            changeVisibleStateProperty("Area", "AllProperties", "Main", "Color", !allowApplyStyleMain);
            changeVisibleStateProperty("Area", "AllProperties", "Main", "Style", !allowApplyStyleMain);
            changeVisibleStateProperty("Area", "AllProperties", "Main", "InterlacedBrush", !allowApplyStyleMain);

            var allowApplyStyleLabels = this.innerPanels["Area"].innerPanels["AllProperties"].groups["Labels"].properties["Labels.AllowApplyStyle"].control.isChecked;
            changeVisibleStateProperty("Area", "AllProperties", "Labels", "Labels.Color", !allowApplyStyleLabels);
            changeVisibleStateProperty("Area", "AllProperties", "Labels", "Labels.Font", !allowApplyStyleLabels);
            changeVisibleStateProperty("Area", "AllProperties", "Labels", "Labels.Antialiasing", !allowApplyStyleLabels);

            var allowApplyStyleTitle = this.innerPanels["Area"].innerPanels["AllProperties"].groups["Title"].properties["Title.AllowApplyStyle"].control.isChecked;
            changeVisibleStateProperty("Area", "AllProperties", "Title", "Title.Color", !allowApplyStyleTitle);
            changeVisibleStateProperty("Area", "AllProperties", "Title", "Title.Font", !allowApplyStyleTitle);
            changeVisibleStateProperty("Area", "AllProperties", "Title", "Title.Antialiasing", !allowApplyStyleTitle);
        }
        if (parentPanelName == "Labels" && childPanelName == "Common") {
            var allowApplyStyle = this.innerPanels["Labels"].innerPanels["Common"].groups["Main"].properties["AllowApplyStyle"].control.isChecked;
            changeVisibleStateProperty("Labels", "Common", "Main", "Brush", !allowApplyStyle);
            changeVisibleStateProperty("Labels", "Common", "Main", "BorderColor", !allowApplyStyle);
            changeVisibleStateProperty("Labels", "Common", "Main", "Font", !allowApplyStyle);
            changeVisibleStateProperty("Labels", "Common", "Main", "LabelColor", !allowApplyStyle);
            changeVisibleStateProperty("Labels", "Common", "Main", "LineColor", !allowApplyStyle);
            changeVisibleStateProperty("Labels", "Common", "Main", "Antialiasing", !allowApplyStyle);
        }
    }

    chartPropertiesPanel.showAllSeriesGroups = function (series, firstAction) {
        this.showInnerPanel("Series");
        var seriesPanel = this.innerPanels.Series;
        var oldScrollTop = jsObject.options.propertiesPanel.containers.Properties.scrollTop;

        showPanel("Common", seriesPanel.innerPanels);
        seriesPanel.innerPanels.Interaction.style.display = "";
        seriesPanel.innerPanels.TopN.style.display = "";
        seriesPanel.innerPanels.SeriesLabels.style.display = "";

        if (firstAction) {
            seriesPanel.innerPanels.Interaction.groups.Main.changeOpenedState(false);
            seriesPanel.innerPanels.TopN.groups.Main.changeOpenedState(false);
            seriesPanel.innerPanels.SeriesLabels.groups.Main.changeOpenedState(false);
        }

        this.updateProperties(seriesPanel.innerPanels.Common.groups, series.properties.Common);
        this.updateProperties(seriesPanel.innerPanels.Interaction.groups, series.properties.Interaction);
        this.updateProperties(seriesPanel.innerPanels.TopN.groups, series.properties.TopN);
        this.updateProperties(seriesPanel.innerPanels.SeriesLabels.groups, series.labels.properties.Common);
        this.updatePropertiesUsedInStyles();

        var labelsTypeProperty = seriesPanel.innerPanels.SeriesLabels.groups.Main.properties.SeriesLabelsType
        labelsTypeProperty.style.display = "";
        labelsTypeProperty.propertyControl.setKey(series.labels.type);
        labelsTypeProperty.propertyControl.textBox.value = series.labels.serviceName;

        var showSeriesLabelsProperty = seriesPanel.innerPanels.SeriesLabels.groups.Main.properties.ShowSeriesLabels;
        showSeriesLabelsProperty.style.display = "";
        showSeriesLabelsProperty.propertyControl.setKey(series.properties.Common.ShowSeriesLabels);
        showSeriesLabelsProperty.panelName = "Series";

        if (!firstAction) {
            jsObject.options.propertiesPanel.containers.Properties.scrollTop = oldScrollTop;
        }

        var drillDownPageProperty = seriesPanel.innerPanels.Interaction.groups.Main.properties.DrillDownPage;
        drillDownPageProperty.propertyControl.addItems(jsObject.GetDrillDownPageItems());
    }

    chartPropertiesPanel.showAllStripsGroups = function (strip, firstAction) {
        this.showInnerPanel("Chart");
        var chartPanel = this.innerPanels.Chart;
        showPanel("Strips", chartPanel.innerPanels);

        if (firstAction) {
            chartPanel.innerPanels.Strips.groups.Behavior.changeOpenedState(true);
            chartPanel.innerPanels.Strips.groups.Title.changeOpenedState(true);
        }

        this.updateProperties(chartPanel.innerPanels.Strips.groups, strip.properties);
        var currentParentPanelName = this.currentParentPanelName;
        var currentChildPanelName = this.currentChildPanelName;
        this.currentParentPanelName = "Chart";
        this.currentChildPanelName = "Strips";
        this.updatePropertiesUsedInStyles();
        this.currentParentPanelName = currentParentPanelName;
        this.currentChildPanelName = currentChildPanelName;
    }

    chartPropertiesPanel.showAllConstantLinesGroups = function (constantLine, firstAction) {
        this.showInnerPanel("Chart");
        var chartPanel = this.innerPanels.Chart;
        showPanel("ConstantLines", chartPanel.innerPanels);

        if (firstAction) {
            chartPanel.innerPanels.ConstantLines.groups.Behavior.changeOpenedState(true);
            chartPanel.innerPanels.ConstantLines.groups.Title.changeOpenedState(true);
        }

        this.updateProperties(chartPanel.innerPanels.ConstantLines.groups, constantLine.properties);
        var currentParentPanelName = this.currentParentPanelName;
        var currentChildPanelName = this.currentChildPanelName;
        this.currentParentPanelName = "Chart";
        this.currentChildPanelName = "ConstantLines";
        this.updatePropertiesUsedInStyles();
        this.currentParentPanelName = currentParentPanelName;
        this.currentChildPanelName = currentChildPanelName;
    }

    //Add Chart Properties    
    this.AddChartCommonProperties(chartPropertiesPanel);
    this.AddChartLegendProperties(chartPropertiesPanel);
    this.AddChartTitleProperties(chartPropertiesPanel);
    this.AddChartTableProperties(chartPropertiesPanel);
    this.AddChartConstantLinesProperties(chartPropertiesPanel);
    this.AddChartStripsProperties(chartPropertiesPanel);

    //Add Series Properties    
    this.AddSeriesCommonProperties(chartPropertiesPanel);
    this.AddSeriesMarkerOrLineMarkerProperties(chartPropertiesPanel, "Marker");
    this.AddSeriesMarkerOrLineMarkerProperties(chartPropertiesPanel, "LineMarker");
    this.AddSeriesInteractionProperties(chartPropertiesPanel);
    this.AddSeriesTopNProperties(chartPropertiesPanel);

    //Add Area Properties
    this.AddAreaAllProperties(chartPropertiesPanel);

    //Add SeriesLabels Properties
    this.AddLabelsCommonProperties(chartPropertiesPanel);
    this.AddLabelsCommonProperties(chartPropertiesPanel, true);

    return chartPropertiesPanel;
}

StiMobileDesigner.prototype.AddChartCommonProperties = function (chartPropertiesPanel) {
    var panel = chartPropertiesPanel.innerPanels["Chart"].innerPanels["Common"];

    var groupNames = [
        ["Chart", this.loc.PropertyCategory.ChartCategory],
        ["Data", this.loc.PropertyMain.Data],
    ]
    this.AddGroupsToPropertiesPanel(groupNames, panel);

    var properties = [
        ["AllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("chartCommonAllowApplyStyle"), panel.groups["Chart"], "Checkbox"],
        ["ProcessAtEnd", this.loc.PropertyMain.ProcessAtEnd, this.CheckBox("chartCommonProcessAtEnd"), panel.groups["Chart"], "Checkbox"],
        ["Rotation", this.loc.PropertyMain.Rotation,
            this.PropertyDropDownList("chartCommonRotation", this.options.propertyControlWidth, this.GetImageRotationItems(), true), panel.groups["Chart"], "DropdownList"],
        ["HorSpacing", this.loc.PropertyMain.HorSpacing, this.PropertyTextBox("chartCommonHorSpacing", this.options.propertyNumbersControlWidth), panel.groups["Chart"], "Textbox"],
        ["VertSpacing", this.loc.PropertyMain.VertSpacing, this.PropertyTextBox("chartCommonVertSpacing", this.options.propertyNumbersControlWidth), panel.groups["Chart"], "Textbox"],
        ["DataSource", this.loc.PropertyMain.DataSource,
            this.PropertyChartDataDropDownList("chartCommonDataSource", this.options.propertyControlWidth, null, true), panel.groups["Data"], "DropdownList"],
        ["DataRelation", this.loc.PropertyMain.DataRelation,
            this.PropertyChartDataDropDownList("chartCommonDataRelation", this.options.propertyControlWidth, null, true), panel.groups["Data"], "DropdownList"],
        ["MasterComponent", this.loc.PropertyMain.MasterComponent,
            this.PropertyChartDataDropDownList("chartCommonMasterComponent", this.options.propertyControlWidth, null, true), panel.groups["Data"], "DropdownList"],
        ["BusinessObject", this.loc.PropertyMain.BusinessObject,
            this.PropertyBusinessObject("chartCommonBusinessObject"), panel.groups["Data"], "BusinessObject"],
        ["CountData", this.loc.PropertyMain.CountData, this.PropertyTextBox("chartCommonCountData", this.options.propertyNumbersControlWidth), panel.groups["Data"], "Textbox"],
        ["Filters", this.loc.PropertyMain.Filters, this.PropertyFilter("chartCommonFilters"), panel.groups["Data"], "Filters"],
        ["Sort", this.loc.PropertyMain.Sort, this.PropertySort("chartCommonSort"), panel.groups["Data"], "Sort"]
    ]

    for (var i = 0; i < properties.length; i++) {
        var property = this.AddPropertyToPropertiesGroup(properties[i][0], properties[i][1], properties[i][2], properties[i][3], properties[i][4]);
        property.panelName = "Chart";
    }

    panel.groups["Data"].properties["Filters"].control.dataSourceControl = panel.groups["Data"].properties["DataSource"].control;
    panel.groups["Data"].properties["Sort"].control.dataSourceControl = panel.groups["Data"].properties["DataSource"].control;

    panel.updateDataControls = function () {
        var properties = this.groups["Data"].properties;

        var dataSourceControl = properties["DataSource"].control;
        if (dataSourceControl) {
            dataSourceControl.addItems(chartPropertiesPanel.jsObject.GetDataSourceItemsFromDictionary());
        }
        var masterComponentControl = properties["MasterComponent"].control;
        if (masterComponentControl) {
            masterComponentControl.addItems(chartPropertiesPanel.jsObject.GetMasterComponentItems());
        }
    }

    panel.updateDataRelationControl = function () {
        var properties = this.groups["Data"].properties;
        var dataRelationControl = properties["DataRelation"].control;
        var dataSourceControl = properties["DataSource"].control;
        if (dataRelationControl && dataSourceControl) {
            var dataSource = chartPropertiesPanel.jsObject.GetDataSourceByNameFromDictionary(dataSourceControl.key);
            var relations = chartPropertiesPanel.jsObject.GetRelationsInSourceItems(dataSource);
            dataRelationControl.addItems(relations);
            if (relations == null) {
                dataRelationControl.reset();
            }
            else {
                if (chartPropertiesPanel.jsObject.GetRelationBySourceName(dataSource, dataRelationControl.key))
                    dataRelationControl.setKey(dataRelationControl.key);
                else
                    dataRelationControl.reset();
            }
        }
    }
}

StiMobileDesigner.prototype.AddChartLegendProperties = function (chartPropertiesPanel) {
    var panel = chartPropertiesPanel.innerPanels["Chart"].innerPanels["Legend"];

    var groupNames = [
        ["Main", this.loc.PropertyCategory.MainCategory]
    ]
    this.AddGroupsToPropertiesPanel(groupNames, panel);

    var properties = [
        ["AllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("chartLegendAllowApplyStyle"), panel.groups["Main"], "Checkbox"],
        ["BorderColor", this.loc.PropertyMain.BorderColor,
            this.PropertyColorControl("chartLegendBorderColor", null, this.options.propertyControlWidth), panel.groups["Main"], "Color"],
        ["Brush", this.loc.PropertyMain.Brush,
            this.PropertyBrushControl("chartLegendBrush", null, this.options.propertyControlWidth), panel.groups["Main"], "Brush"],
        ["Columns", this.loc.PropertyMain.Columns, this.PropertyTextBox("chartLegendColumns", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["ColumnWidth", this.loc.PropertyMain.ColumnWidth, this.PropertyTextBox("chartLegendColumnWidth", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["Direction", this.loc.PropertyMain.Direction,
            this.PropertyDropDownList("chartLegendDirection", this.options.propertyControlWidth, this.GetXAxisTitleDirectionItems(), true), panel.groups["Main"], "DropdownList"],
        ["Font", this.loc.PropertyMain.Font, this.PropertyFontControl("chartLegendFont", null, true), panel.groups["Main"], "Font"],
        ["HideSeriesWithEmptyTitle", this.loc.PropertyMain.HideSeriesWithEmptyTitle, this.CheckBox("chartLegendHideSeriesWithEmptyTitle"), panel.groups["Main"], "Checkbox"],
        ["HorAlignment", this.loc.PropertyMain.HorAlignment,
            this.PropertyDropDownList("chartLegendHorAlignment", this.options.propertyControlWidth, this.GetLegendHorAlignmentItems(), true), panel.groups["Main"], "DropdownList"],
        ["HorSpacing", this.loc.PropertyMain.HorSpacing, this.PropertyTextBox("chartLegendHorSpacing", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["LabelsColor", this.loc.PropertyMain.LabelsColor,
            this.PropertyColorControl("chartLegendLabelsColor", null, this.options.propertyControlWidth), panel.groups["Main"], "Color"],
        ["MarkerAlignment", this.loc.PropertyMain.MarkerAlignment,
            this.PropertyDropDownList("chartLegendMarkerAlignment", this.options.propertyControlWidth, this.GetXAxisTextAlignmentItems(), true), panel.groups["Main"], "DropdownList"],
        ["MarkerBorder", this.loc.PropertyMain.MarkerBorder, this.CheckBox("chartLegendMarkerBorder"), panel.groups["Main"], "Checkbox"],
        ["MarkerSize", this.loc.PropertyMain.MarkerSize, this.PropertySizeControl("schartLegendMarkerSize", this.options.propertyNumbersControlWidth + 40), panel.groups["Main"], "Size"],
        ["MarkerVisible", this.loc.PropertyMain.MarkerVisible, this.CheckBox("chartLegendMarkerVisible"), panel.groups["Main"], "Checkbox"],
        ["ShowShadow", this.loc.PropertyMain.ShowShadow, this.CheckBox("chartLegendShowShadow"), panel.groups["Main"], "Checkbox"],
        ["Title", this.loc.PropertyMain.Title, this.PropertyTextBox("schartLegendTitle", this.options.propertyControlWidth), panel.groups["Main"], "Textbox"],
        ["TitleColor", this.loc.PropertyMain.TitleColor,
            this.PropertyColorControl("chartLegendTitleColor", null, this.options.propertyControlWidth), panel.groups["Main"], "Color"],
        ["TitleFont", this.loc.PropertyMain.TitleFont, this.PropertyFontControl("chartLegendTitleFont", null, true), panel.groups["Main"], "Font"],
        ["VertAlignment", this.loc.PropertyMain.VertAlignment,
            this.PropertyDropDownList("chartLegendVertAlignment", this.options.propertyControlWidth, this.GetLegendVertAlignmentItems(), true), panel.groups["Main"], "DropdownList"],
        ["VertSpacing", this.loc.PropertyMain.VertSpacing, this.PropertyTextBox("chartLegendVertSpacing", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["Visible", this.loc.PropertyMain.Visible, this.CheckBox("chartLegendVisible"), panel.groups["Main"], "Checkbox"]
    ]

    for (var i = 0; i < properties.length; i++) {
        var property = this.AddPropertyToPropertiesGroup(properties[i][0], properties[i][1], properties[i][2], properties[i][3], properties[i][4]);
        property.panelName = "Chart";
        property.ownerName = "Legend";
    }
}

StiMobileDesigner.prototype.AddChartTitleProperties = function (chartPropertiesPanel) {
    var panel = chartPropertiesPanel.innerPanels["Chart"].innerPanels["Title"];

    var groupNames = [
        ["Main", this.loc.PropertyCategory.MainCategory]
    ]
    this.AddGroupsToPropertiesPanel(groupNames, panel);

    var properties = [
        ["AllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("chartTitleAllowApplyStyle"), panel.groups["Main"], "Checkbox"],
        ["Alignment", this.loc.PropertyMain.Alignment,
            this.PropertyDropDownList("chartTitleAlignment", this.options.propertyControlWidth, this.GetXAxisTitleAlignmentItems(), true), panel.groups["Main"], "DropdownList"],
        ["Antialiasing", this.loc.PropertyMain.Antialiasing, this.CheckBox("chartTitleAntialiasing"), panel.groups["Main"], "Checkbox"],
        ["Brush", this.loc.PropertyMain.Brush, this.PropertyBrushControl("chartTitleBrush", null, this.options.propertyControlWidth), panel.groups["Main"], "Brush"],
        ["Dock", this.loc.PropertyMain.Dock,
            this.PropertyDropDownList("chartTitleDock", this.options.propertyControlWidth, this.GetDockItems(), true), panel.groups["Main"], "DropdownList"],
        ["Font", this.loc.PropertyMain.Font, this.PropertyFontControl("chartTitleFont", null, true), panel.groups["Main"], "Font"],
        ["Spacing", this.loc.PropertyMain.Spacing, this.PropertyTextBox("chartTitleSpacing", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["Text", this.loc.PropertyMain.Text, this.PropertyTextBox("chartTitleText", this.options.propertyControlWidth), panel.groups["Main"], "Textbox"],
        ["Visible", this.loc.PropertyMain.Visible, this.CheckBox("chartTitleVisible"), panel.groups["Main"], "Checkbox"]
    ]

    for (var i = 0; i < properties.length; i++) {
        var property = this.AddPropertyToPropertiesGroup(properties[i][0], properties[i][1], properties[i][2], properties[i][3], properties[i][4]);
        property.panelName = "Chart";
        property.ownerName = "Title";
    }
}

StiMobileDesigner.prototype.AddChartTableProperties = function (chartPropertiesPanel) {
    var panel = chartPropertiesPanel.innerPanels["Chart"].innerPanels["Table"];

    var groupNames = [
        ["Main", this.loc.PropertyCategory.MainCategory],
        ["DataCells", this.loc.PropertyCategory.DataCells],
        ["Header", this.loc.Components.StiHeaderBand]
    ]
    this.AddGroupsToPropertiesPanel(groupNames, panel);

    var properties = [
        ["AllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("chartTableAllowApplyStyle"), panel.groups["Main"], "Checkbox"],
        ["TextColor", this.loc.PropertyMain.TextColor,
            this.PropertyColorControl("chartTableTextColor", null, this.options.propertyControlWidth), panel.groups["Main"], "Color"],
        ["Font", this.loc.PropertyMain.Font, this.PropertyFontControl("chartTableFont", null, true), panel.groups["Main"], "Font"],
        ["GridLineColor", this.loc.PropertyMain.GridLineColor,
            this.PropertyColorControl("chartTableGridLineColor", null, this.options.propertyControlWidth), panel.groups["Main"], "Color"],
        ["GridLinesHor", this.loc.PropertyMain.GridLinesHor, this.CheckBox("chartTableGridLineHor"), panel.groups["Main"], "Checkbox"],
        ["GridLinesVert", this.loc.PropertyMain.GridLinesVert, this.CheckBox("chartTableGridLineVert"), panel.groups["Main"], "Checkbox"],
        ["GridOutline", this.loc.PropertyMain.GridOutline, this.CheckBox("chartTableGridOutline"), panel.groups["Main"], "Checkbox"],
        ["DataCells.TextColor", this.loc.PropertyMain.TextColor,
            this.PropertyColorControl("chartTableDataCellsTextColor", null, this.options.propertyControlWidth), panel.groups["DataCells"], "Color"],
        ["DataCells.ShrinkFontToFit", this.loc.PropertyMain.ShrinkFontToFit, this.CheckBox("chartTableDataCellsShrinkFontToFit"), panel.groups["DataCells"], "Checkbox"],
        ["DataCells.ShrinkFontToFitMinimumSize", this.loc.PropertyMain.ShrinkFontToFitMinimumSize, this.PropertyTextBox("chartTableDataCells", this.options.propertyNumbersControlWidth), panel.groups["DataCells"], "Textbox"],
        ["DataCells.Font", this.loc.PropertyMain.Font, this.PropertyFontControl("chartTableDataCellsFont", null, true), panel.groups["DataCells"], "Font"],
        ["Header.Format", this.loc.PropertyMain.Format,
            this.PropertyDropDownList("chartTableHeaderFormat", this.options.propertyControlWidth, this.GetTableHeaderFormatItems(), false), panel.groups["Header"], "DropdownList"],
        ["Header.TextAfter", this.loc.PropertyMain.TextAfter, this.PropertyTextBox("chartTableHeaderTextAfter", this.options.propertyControlWidth), panel.groups["Header"], "Textbox"],
        ["Header.TextColor", this.loc.PropertyMain.TextColor,
            this.PropertyColorControl("chartTableHeaderTextColor", null, this.options.propertyControlWidth), panel.groups["Header"], "Color"],
        ["Header.WordWrap", this.loc.PropertyMain.WordWrap, this.CheckBox("chartTableHeaderWordWrap"), panel.groups["Header"], "Checkbox"],
        ["Header.Brush", this.loc.PropertyMain.Brush, this.PropertyBrushControl("chartTableHeaderBrush", null, this.options.propertyControlWidth), panel.groups["Header"], "Brush"],
        ["Header.Font", this.loc.PropertyMain.Font, this.PropertyFontControl("chartTableHeaderFont", null, true), panel.groups["Header"], "Font"],
        ["MarkerVisible", this.loc.PropertyMain.MarkerVisible, this.CheckBox("chartTableMarkerVisible"), panel.groups["Main"], "Checkbox"],
        ["Visible", this.loc.PropertyMain.Visible, this.CheckBox("chartTableVisible"), panel.groups["Main"], "Checkbox"]
    ]

    for (var i = 0; i < properties.length; i++) {
        var property = this.AddPropertyToPropertiesGroup(properties[i][0], properties[i][1], properties[i][2], properties[i][3], properties[i][4]);
        property.panelName = "Chart";
        property.ownerName = "Table";
    }
}

StiMobileDesigner.prototype.AddChartConstantLinesProperties = function (chartPropertiesPanel) {
    var panel = chartPropertiesPanel.innerPanels["Chart"].innerPanels["ConstantLines"];

    var groupNames = [
        ["Chart", this.loc.PropertyCategory.ChartCategory],
        ["Behavior", this.loc.PropertyCategory.BehaviorCategory],
        ["Title", this.loc.PropertyCategory.TitleCategory]
    ]
    this.AddGroupsToPropertiesPanel(groupNames, panel);

    var properties = [
        ["AllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("chartConstantLinesAllowApplyStyle"), panel.groups["Chart"], "Checkbox"],
        ["AxisValue", this.loc.PropertyMain.AxisValue, this.PropertyTextBox("chartConstantLinesAxisValue", this.options.propertyControlWidth), panel.groups["Behavior"], "Textbox"],
        ["LineColor", this.loc.PropertyMain.LineColor,
            this.PropertyColorControl("chartConstantLinesLineColor", null, this.options.propertyControlWidth), panel.groups["Behavior"], "Color"],
        ["LineStyle", this.loc.PropertyMain.LineStyle,
            this.PropertyDropDownList("chartConstantLinesLineStyle", this.options.propertyControlWidth, this.GetBorderStyleItems(), true, true), panel.groups["Behavior"], "DropdownList"],
        ["LineWidth", this.loc.PropertyMain.LineWidth, this.PropertyTextBox("chartConstantLinesLineWidth", this.options.propertyNumbersControlWidth), panel.groups["Behavior"], "Textbox"],
        ["Orientation", this.loc.PropertyMain.Orientation,
            this.PropertyDropDownList("chartConstantLinesOrientation", this.options.propertyControlWidth, this.GetConstantLinesOrientationItems(), true), panel.groups["Behavior"], "DropdownList"],
        ["ShowBehind", this.loc.PropertyMain.ShowBehind, this.CheckBox("chartConstantLinesShowBehind"), panel.groups["Behavior"], "Checkbox"],
        ["Visible", this.loc.PropertyMain.Visible, this.CheckBox("chartConstantLinesVisible"), panel.groups["Behavior"], "Checkbox"],
        ["Antialiasing", this.loc.PropertyMain.Antialiasing, this.CheckBox("chartConstantLinesAntialiasing"), panel.groups["Title"], "Checkbox"],
        ["Font", this.loc.PropertyMain.Font, this.PropertyFontControl("chartConstantLinesFont", null, true), panel.groups["Title"], "Font"],
        ["Position", this.loc.PropertyMain.Position,
            this.PropertyDropDownList("chartConstantLinesPosition", this.options.propertyControlWidth, this.GetConstantLinesPositionItems(), true), panel.groups["Title"], "DropdownList"],
        ["Text", this.loc.PropertyMain.Text, this.PropertyTextBox("chartConstantLinesText", this.options.propertyControlWidth), panel.groups["Title"], "Textbox"],
        ["TitleVisible", this.loc.PropertyMain.Visible, this.CheckBox("chartConstantLinesTitleVisible"), panel.groups["Title"], "Checkbox"]
    ]

    for (var i = 0; i < properties.length; i++) {
        var property = this.AddPropertyToPropertiesGroup(properties[i][0], properties[i][1], properties[i][2], properties[i][3], properties[i][4]);
        property.panelName = "Chart";
        property.ownerName = "ConstantLines";
    }
}

StiMobileDesigner.prototype.AddChartStripsProperties = function (chartPropertiesPanel) {
    var panel = chartPropertiesPanel.innerPanels["Chart"].innerPanels["Strips"];

    var groupNames = [
        ["Chart", this.loc.PropertyCategory.ChartCategory],
        ["Behavior", this.loc.PropertyCategory.BehaviorCategory],
        ["Title", this.loc.PropertyCategory.TitleCategory]
    ]
    this.AddGroupsToPropertiesPanel(groupNames, panel);

    var properties = [
        ["AllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("chartStripsAllowApplyStyle"), panel.groups["Chart"], "Checkbox"],
        ["MaxValue", this.loc.PropertyMain.MaxValue, this.PropertyTextBox("chartStripsMaxValue", this.options.propertyNumbersControlWidth), panel.groups["Behavior"], "Textbox"],
        ["MinValue", this.loc.PropertyMain.MinValue, this.PropertyTextBox("chartStripsMinValue", this.options.propertyNumbersControlWidth), panel.groups["Behavior"], "Textbox"],
        ["Orientation", this.loc.PropertyMain.Orientation,
            this.PropertyDropDownList("chartStripsOrientation", this.options.propertyControlWidth, this.GetConstantLinesOrientationItems(), true), panel.groups["Behavior"], "DropdownList"],
        ["ShowBehind", this.loc.PropertyMain.ShowBehind, this.CheckBox("chartStripsShowBehind"), panel.groups["Behavior"], "Checkbox"],
        ["StripBrush", this.loc.PropertyMain.Brush,
            this.PropertyBrushControl("chartStripsStripBrush", null, this.options.propertyControlWidth), panel.groups["Behavior"], "Brush"],
        ["Visible", this.loc.PropertyMain.Visible, this.CheckBox("chartStripsVisible"), panel.groups["Behavior"], "Checkbox"],
        ["Antialiasing", this.loc.PropertyMain.Antialiasing, this.CheckBox("chartStripsAntialiasing"), panel.groups["Title"], "Checkbox"],
        ["Font", this.loc.PropertyMain.Font, this.PropertyFontControl("chartStripsFont", null, true), panel.groups["Title"], "Font"],
        ["Text", this.loc.PropertyMain.Text, this.PropertyTextBox("chartStripsText", this.options.propertyControlWidth), panel.groups["Title"], "Textbox"],
        ["TitleColor", this.loc.PropertyMain.TitleColor,
            this.PropertyColorControl("chartStripsTitleColor", null, this.options.propertyControlWidth), panel.groups["Title"], "Color"],
        ["TitleVisible", this.loc.PropertyMain.TitleVisible, this.CheckBox("chartStripsTitleVisible"), panel.groups["Title"], "Checkbox"]
    ]

    for (var i = 0; i < properties.length; i++) {
        var property = this.AddPropertyToPropertiesGroup(properties[i][0], properties[i][1], properties[i][2], properties[i][3], properties[i][4]);
        property.panelName = "Chart";
        property.ownerName = "Strips";
    }
}

StiMobileDesigner.prototype.AddSeriesCommonProperties = function (chartPropertiesPanel) {
    var panel = chartPropertiesPanel.innerPanels["Series"].innerPanels["Common"];

    var groupNames = [
        ["Value", this.loc.PropertyMain.Value],
        ["ValueOpen", this.loc.PropertyMain.ValueOpen],
        ["ValueClose", this.loc.PropertyMain.ValueClose],
        ["ValueHigh", this.loc.PropertyMain.ValueHigh],
        ["ValueLow", this.loc.PropertyMain.ValueLow],
        ["ValueEnd", this.loc.PropertyMain.ValueEnd],
        ["Argument", this.loc.PropertyMain.Argument],
        ["Weight", this.loc.PropertyCategory.WeightCategory],
        ["Data", this.loc.PropertyMain.Data],
        ["Appearance", this.loc.PropertyCategory.AppearanceCategory],
        ["Options3D", this.loc.PropertyMain.Options3D],
        ["Common", this.loc.PropertyCategory.CommonCategory],
        ["Series", this.loc.PropertyMain.Series]
    ]

    this.AddGroupsToPropertiesPanel(groupNames, panel);

    var properties = [
        ["ValueDataColumn", this.loc.PropertyMain.ValueDataColumn, this.PropertyDataControl("seriesCommonValueDataColumn", this.options.propertyControlWidth, true), panel.groups["Value"], "DataColumn"],
        ["Value", this.loc.PropertyMain.Value, this.PropertyExpressionControl("seriesCommonValue", this.options.propertyControlWidth), panel.groups["Value"], "Expression"],
        ["ListOfValues", this.loc.PropertyMain.ListOfValues, this.PropertyExpressionControl("seriesCommonListOfValues", this.options.propertyControlWidth), panel.groups["Value"], "Expression"],
        ["ValueDataColumnEnd", this.loc.PropertyMain.ValueDataColumnEnd, this.PropertyDataControl("seriesCommonValueDataColumnEnd", this.options.propertyControlWidth, true), panel.groups["ValueEnd"], "DataColumn"],
        ["ValueEnd", this.loc.PropertyMain.ValueEnd, this.PropertyExpressionControl("seriesCommonValueEnd", this.options.propertyControlWidth), panel.groups["ValueEnd"], "Expression"],
        ["ListOfValuesEnd", this.loc.PropertyMain.ListOfValuesEnd, this.PropertyExpressionControl("seriesCommonListOfValuesEnd", this.options.propertyControlWidth), panel.groups["ValueEnd"], "Expression"],
        ["ValueDataColumnClose", this.loc.PropertyMain.ValueDataColumnClose, this.PropertyDataControl("seriesCommonValueDataColumnClose", this.options.propertyControlWidth, true), panel.groups["ValueClose"], "DataColumn"],
        ["ValueClose", this.loc.PropertyMain.ValueClose, this.PropertyExpressionControl("seriesCommonValueClose", this.options.propertyControlWidth), panel.groups["ValueClose"], "Expression"],
        ["ListOfValuesClose", this.loc.PropertyMain.ListOfValuesClose, this.PropertyExpressionControl("seriesCommonListOfValuesClose", this.options.propertyControlWidth), panel.groups["ValueClose"], "Expression"],
        ["ValueDataColumnOpen", this.loc.PropertyMain.ValueDataColumnOpen, this.PropertyDataControl("seriesCommonValueDataColumnOpen", this.options.propertyControlWidth, true), panel.groups["ValueOpen"], "DataColumn"],
        ["ValueOpen", this.loc.PropertyMain.ValueOpen, this.PropertyExpressionControl("seriesCommonValueOpen", this.options.propertyControlWidth), panel.groups["ValueOpen"], "Expression"],
        ["ListOfValuesOpen", this.loc.PropertyMain.ListOfValuesOpen, this.PropertyExpressionControl("seriesCommonListOfValuesOpen", this.options.propertyControlWidth), panel.groups["ValueOpen"], "Expression"],
        ["ValueDataColumnHigh", this.loc.PropertyMain.ValueDataColumnHigh, this.PropertyDataControl("seriesCommonValueDataColumnHigh", this.options.propertyControlWidth, true), panel.groups["ValueHigh"], "DataColumn"],
        ["ValueHigh", this.loc.PropertyMain.ValueHigh, this.PropertyExpressionControl("seriesCommonValueHigh", this.options.propertyControlWidth), panel.groups["ValueHigh"], "Expression"],
        ["ListOfValuesHigh", this.loc.PropertyMain.ListOfValuesHigh, this.PropertyExpressionControl("seriesCommonListOfValuesHigh", this.options.propertyControlWidth), panel.groups["ValueHigh"], "Expression"],
        ["ValueDataColumnLow", this.loc.PropertyMain.ValueDataColumnLow, this.PropertyDataControl("seriesCommonValueDataColumnLow", this.options.propertyControlWidth, true), panel.groups["ValueLow"], "DataColumn"],
        ["ValueLow", this.loc.PropertyMain.ValueLow, this.PropertyExpressionControl("seriesCommonValueLow", this.options.propertyControlWidth), panel.groups["ValueLow"], "Expression"],
        ["ListOfValuesLow", this.loc.PropertyMain.ListOfValuesLow, this.PropertyExpressionControl("seriesCommonListOfValuesLow", this.options.propertyControlWidth), panel.groups["ValueLow"], "Expression"],
        ["WeightDataColumn", this.loc.PropertyMain.WeightDataColumn, this.PropertyDataControl("seriesCommonWeightDataColumn", this.options.propertyControlWidth, true), panel.groups["Weight"], "DataColumn"],
        ["Weight", this.loc.PropertyMain.Weight, this.PropertyExpressionControl("seriesCommonWeight", this.options.propertyControlWidth), panel.groups["Weight"], "Expression"],
        ["ListOfWeights", this.loc.PropertyMain.ListOfWeights, this.PropertyExpressionControl("seriesCommonListOfWeights", this.options.propertyControlWidth), panel.groups["Weight"], "Expression"],
        ["ArgumentDataColumn", this.loc.PropertyMain.ArgumentDataColumn, this.PropertyDataControl("seriesCommonArgumentDataColumn", this.options.propertyControlWidth, true), panel.groups["Argument"], "DataColumn"],
        ["Argument", this.loc.PropertyMain.Argument, this.PropertyExpressionControl("seriesCommonArgument", this.options.propertyControlWidth), panel.groups["Argument"], "Expression"],
        ["ListOfArguments", this.loc.PropertyMain.ListOfArguments, this.PropertyExpressionControl("seriesCommonListOfArguments", this.options.propertyControlWidth), panel.groups["Argument"], "Expression"],
        ["Conditions", this.loc.PropertyMain.Conditions, this.PropertyChartSeriesConditions("seriesCommonConditions"), panel.groups["Data"], "Conditions"],
        ["FilterMode", this.loc.PropertyMain.FilterMode, this.PropertyDropDownList("seriesCommonFilterMode", this.options.propertyControlWidth, this.GetFilterTypeItems(), true), panel.groups["Data"], "DropdownList"],
        ["Filters", this.loc.PropertyMain.Filters, this.PropertyChartSeriesFilter("seriesCommonFilters"), panel.groups["Data"], "Filters"],
        ["Format", this.loc.PropertyMain.Format, this.PropertyDropDownList("seriesCommonFormat", this.options.propertyControlWidth, this.GetTableHeaderFormatItems(), false), panel.groups["Data"], "DropdownList"],
        ["ShowNulls", this.loc.PropertyMain.ShowNulls, this.CheckBox("seriesCommonShowNulls"), panel.groups["Data"], "Checkbox"],
        ["ShowZeros", this.loc.PropertyMain.ShowZeros, this.CheckBox("seriesCommonShowZeros"), panel.groups["Data"], "Checkbox"],
        ["SortBy", this.loc.PropertyMain.SortBy, this.PropertyDropDownList("seriesCommonSortBy", this.options.propertyControlWidth, this.GetSortByItems(), true), panel.groups["Data"], "DropdownList"],
        ["SortDirection", this.loc.PropertyMain.SortDirection, this.PropertyDropDownList("seriesCommonSortDirection", this.options.propertyControlWidth, this.GetChartSortDirectionItems(), true), panel.groups["Data"], "DropdownList"],
        ["AutoSeriesKeyDataColumn", this.loc.PropertyMain.AutoSeriesKeyDataColumn, this.PropertyDataControl("seriesCommonAutoSeriesKeyDataColumn", this.options.propertyControlWidth, true), panel.groups["Series"], "DataColumn"],
        ["AutoSeriesColorDataColumn", this.loc.PropertyMain.AutoSeriesColorDataColumn, this.PropertyDataControl("seriesCommonAutoSeriesColorDataColumn", this.options.propertyControlWidth, true), panel.groups["Series"], "DataColumn"],
        ["AutoSeriesTitleDataColumn", this.loc.PropertyMain.AutoSeriesTitleDataColumn, this.PropertyDataControl("seriesCommonAutoSeriesTitleDataColumn", this.options.propertyControlWidth, true), panel.groups["Series"], "DataColumn"],
        ["Diameter", this.loc.PropertyMain.Diameter, this.PropertyTextBox("seriesCommonDiameter", this.options.propertyNumbersControlWidth), panel.groups["Appearance"], "Textbox"],
        ["AllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("seriesCommonAllowApplyStyle"), panel.groups["Appearance"], "Checkbox"],
        ["AllowApplyLineColor", this.loc.PropertyMain.AllowApplyLineColor, this.CheckBox("seriesCommonAllowApplyLineColor"), panel.groups["Appearance"], "Checkbox"],
        ["AllowApplyBrushNegative", this.loc.PropertyMain.AllowApplyBrushNegative, this.CheckBox("seriesCommonAllowApplyBrushNegative"), panel.groups["Appearance"], "Checkbox"],
        ["AllowApplyColorNegative", this.loc.PropertyMain.AllowApplyColorNegative, this.CheckBox("seriesCommonAllowApplyColorNegative"), panel.groups["Appearance"], "Checkbox"],
        ["AllowApplyBorderColor", this.loc.PropertyMain.AllowApplyBorderColor, this.CheckBox("seriesCommonAllowApplyBorderColor"), panel.groups["Appearance"], "Checkbox"],
        ["AllowApplyBrush", this.loc.PropertyMain.AllowApplyBrush, this.CheckBox("seriesCommonAllowApplyBrush"), panel.groups["Appearance"], "Checkbox"],
        ["BorderColor", this.loc.PropertyMain.BorderColor, this.PropertyColorControl("seriesCommonBorderColor", null, this.options.propertyControlWidth), panel.groups["Appearance"], "Color"],
        ["BorderColorNegative", this.loc.PropertyMain.BorderColorNegative, this.PropertyColorControl("seriesCommonBorderColorNegative", null, this.options.propertyControlWidth), panel.groups["Appearance"], "Color"],
        ["BorderThickness", this.loc.PropertyMain.BorderThickness, this.PropertyTextBox("seriesCommonBorderThickness", this.options.propertyNumbersControlWidth), panel.groups["Appearance"], "Textbox"],
        ["BorderWidth", this.loc.PropertyMain.BorderWidth, this.PropertyTextBox("seriesCommonBorderWidth", this.options.propertyNumbersControlWidth), panel.groups["Appearance"], "Textbox"],
        ["Brush", this.loc.PropertyMain.Brush, this.PropertyBrushControl("seriesCommonBrush", null, this.options.propertyControlWidth), panel.groups["Appearance"], "Brush"],
        ["BrushNegative", this.loc.PropertyMain.BrushNegative, this.PropertyBrushControl("seriesCommonBrushNegative", null, this.options.propertyControlWidth), panel.groups["Appearance"], "Brush"],
        ["Color", this.loc.PropertyMain.Color, this.PropertyColorControl("seriesCommonColor", null, this.options.propertyControlWidth), panel.groups["Appearance"], "Color"],
        ["ColumnShape", this.loc.PropertyMain.ColumnShape, this.PropertyDropDownList("seriesCommonColumnShape", this.options.propertyControlWidth, this.GetColumnShapeItems(), true), panel.groups["Appearance"], "DropdownList"],
        ["CornerRadius", this.loc.PropertyMain.CornerRadius, this.PropertyCornerRadiusControl("seriesCommonCornerRadius", this.options.propertyControlWidth + 61), panel.groups["Appearance"], "CornerRadius"],
        ["LabelsOffset", this.loc.PropertyMain.LabelsOffset, this.PropertyTextBox("seriesCommonLabelsOffset", this.options.propertyNumbersControlWidth), panel.groups["Appearance"], "Textbox"],
        ["Length", this.loc.PropertyMain.Length, this.PropertyTextBox("seriesCommonLength", this.options.propertyNumbersControlWidth), panel.groups["Common"], "Textbox"],
        ["Lighting", this.loc.PropertyMain.Lighting, this.CheckBox("seriesCommonLighting"), panel.groups["Appearance"], "Checkbox"],
        ["LineColor", this.loc.PropertyMain.LineColor, this.PropertyColorControl("seriesCommonLineColor", null, this.options.propertyControlWidth), panel.groups["Appearance"], "Color"],
        ["LineColorNegative", this.loc.PropertyMain.LineColorNegative, this.PropertyColorControl("seriesCommonLineColorNegative", null, this.options.propertyControlWidth), panel.groups["Appearance"], "Color"],
        ["LineStyle", this.loc.PropertyMain.LineStyle, this.PropertyDropDownList("seriesCommonLineStyle", this.options.propertyControlWidth, this.GetBorderStyleItems(), true, true), panel.groups["Appearance"], "DropdownList"],
        ["LineWidth", this.loc.PropertyMain.LineWidth, this.PropertyTextBox("seriesCommonLineWidth", this.options.propertyNumbersControlWidth), panel.groups["Appearance"], "Textbox"],
        ["ShowShadow", this.loc.PropertyMain.ShowShadow, this.CheckBox("seriesCommonShowShadow"), panel.groups["Appearance"], "Checkbox"],
        ["TopmostLine", this.loc.PropertyMain.TopmostLine, this.CheckBox("seriesCommonTopmostLine"), panel.groups["Appearance"], "Checkbox"],
        ["CutPieList", this.loc.PropertyMain.CutPieList, this.PropertyExpressionControl("seriesCommonCutPieList", this.options.propertyControlWidth), panel.groups["Common"], "Expression"],
        ["Distance", this.loc.PropertyMain.Distance, this.PropertyTextBox("seriesCommonDistance", this.options.propertyNumbersControlWidth), panel.groups["Common"], "Textbox"],
        ["Icon", this.loc.PropertyMain.Icon, this.IconControl("seriesCommonIcon", this.options.propertyNumbersControlWidth, this.options.propertyControlsHeight, null, null, true), panel.groups["Common"], "DropdownList"],
        ["PointAtCenter", this.loc.PropertyMain.PointAtCenter, this.CheckBox("seriesCommonPointAtCenter"), panel.groups["Common"], "Checkbox"],
        ["ShowInLegend", this.loc.PropertyMain.ShowInLegend, this.CheckBox("seriesCommonShowInLegend"), panel.groups["Common"], "Checkbox"],
        ["ShowSeriesLabels", this.loc.PropertyMain.ShowSeriesLabels, this.PropertyDropDownList("seriesCommonShowSeriesLabels", this.options.propertyControlWidth, this.GetShowSeriesLabelsItems(), true), panel.groups["Common"], "DropdownList"],
        ["StartAngle", this.loc.PropertyMain.StartAngle, this.PropertyTextBox("seriesCommonStartAngle", this.options.propertyNumbersControlWidth), panel.groups["Common"], "Textbox"],
        ["Tension", this.loc.PropertyMain.Tension, this.PropertyTextBox("seriesCommonTension", this.options.propertyControlWidth), panel.groups["Common"], "Textbox"],
        ["Title", this.loc.PropertyMain.Title, this.PropertyTextBox("seriesCommonTitle", this.options.propertyControlWidth), panel.groups["Common"], "Textbox"],
        ["TrendLines", this.loc.PropertyMain.TrendLines, this.PropertyChartSeriesTrendLines("seriesCommonTrendLines"), panel.groups["Common"], "TrendLines"],
        ["Width", this.loc.PropertyMain.Width, this.PropertyTextBox("seriesCommonWidth", this.options.propertyNumbersControlWidth), panel.groups["Common"], "Textbox"],
        ["XAxis", this.loc.PropertyMain.XAxis, this.PropertyDropDownList("seriesCommonXAxis", this.options.propertyControlWidth, this.GetXAxisItems(), true), panel.groups["Common"], "DropdownList"],
        ["YAxis", this.loc.PropertyMain.YAxis, this.PropertyDropDownList("seriesCommonYAxis", this.options.propertyControlWidth, this.GetYAxisItems(), true), panel.groups["Common"], "DropdownList"],
        ["Options3D.Distance", this.loc.PropertyMain.Distance, this.PropertyTextBox("seriesCommonOptions3DDistance", this.options.propertyNumbersControlWidth), panel.groups["Options3D"], "Textbox"],
        ["Options3D.Height", this.loc.PropertyMain.Height, this.PropertyTextBox("seriesCommonOptions3DHeight", this.options.propertyNumbersControlWidth), panel.groups["Options3D"], "Textbox"],
        ["Options3D.Lighting", this.loc.PropertyMain.Lighting, this.PropertyDropDownList("seriesCommonOptions3DLighting", this.options.propertyControlWidth, this.GetOptions3DLightingItems(), true), panel.groups["Options3D"], "DropdownList"],
        ["Options3D.Opacity", this.loc.PropertyMain.Opacity, this.PropertyTextBox("seriesCommonOptions3DOpacity", this.options.propertyNumbersControlWidth), panel.groups["Options3D"], "Textbox"]
    ]

    for (var i = 0; i < properties.length; i++) {
        var property = this.AddPropertyToPropertiesGroup(properties[i][0], properties[i][1], properties[i][2], properties[i][3], properties[i][4]);
        property.panelName = "Series";
    }
}

StiMobileDesigner.prototype.AddSeriesMarkerOrLineMarkerProperties = function (chartPropertiesPanel, markerName) {
    var panel = chartPropertiesPanel.innerPanels["Series"].innerPanels[markerName];

    var groupNames = [
        ["Main", this.loc.PropertyCategory.MainCategory]
    ]

    this.AddGroupsToPropertiesPanel(groupNames, panel);

    var properties = [
        ["Angle", this.loc.PropertyMain.Angle, this.PropertyTextBox("series" + markerName + "Angle", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["BorderColor", this.loc.PropertyMain.BorderColor,
            this.PropertyColorControl("series" + markerName + "BorderColor", null, this.options.propertyControlWidth), panel.groups["Main"], "Color"],
        ["Brush", this.loc.PropertyMain.Brush,
            this.PropertyBrushControl("series" + markerName + "Brush", null, this.options.propertyControlWidth), panel.groups["Main"], "Brush"],
        ["Icon", this.loc.PropertyMain.Icon, this.IconControl("series" + markerName + "Icon", this.options.propertyNumbersControlWidth, this.options.propertyControlsHeight, null, null, true), panel.groups["Main"], "DropdownList"],
        ["ShowInLegend", this.loc.PropertyMain.ShowInLegend, this.CheckBox("series" + markerName + "ShowInLegend"), panel.groups["Main"], "Checkbox"],
        ["Size", this.loc.PropertyMain.Size, this.PropertyTextBox("series" + markerName + "Size", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["Step", this.loc.PropertyMain.Step, this.PropertyTextBox("series" + markerName + "Step", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["Type", this.loc.PropertyMain.Type, this.PropertyDropDownList("series" + markerName + "Type", this.options.propertyControlWidth, this.GetMarkerTypeItems(), true), panel.groups["Main"], "DropdownList"],
        ["Visible", this.loc.PropertyMain.Visible, this.CheckBox("series" + markerName + "Visible"), panel.groups["Main"], "Checkbox"]
    ]

    for (var i = 0; i < properties.length; i++) {
        if (properties[i][0] == "Step" && markerName == "Marker") continue;
        var property = this.AddPropertyToPropertiesGroup(properties[i][0], properties[i][1], properties[i][2], properties[i][3], properties[i][4]);
        property.panelName = "Series";
        property.ownerName = markerName;
    }
}

StiMobileDesigner.prototype.AddSeriesInteractionProperties = function (chartPropertiesPanel) {
    var panel = chartPropertiesPanel.innerPanels["Series"].innerPanels["Interaction"];

    var groupNames = [
        ["Main", this.loc.PropertyMain.Interaction]
    ]

    this.AddGroupsToPropertiesPanel(groupNames, panel);
    var mainGroup = panel.groups["Main"];
    mainGroup.innerGroups = {};
    mainGroup.changeOpenedState(true);

    var innerGroupNames = [
        ["Hyperlink", this.loc.PropertyMain.Hyperlink, "main", 1],
        ["Tag", this.loc.PropertyMain.Tag, "main", 1],
        ["Tooltip", this.loc.PropertyMain.ToolTip, "main", 1]
    ]

    for (var i = 0; i < innerGroupNames.length; i++) {
        var innerGroup = this.PropertiesGroup(null, innerGroupNames[i][1], null, innerGroupNames[i][3]);
        innerGroup.properties = {};
        mainGroup.innerGroups[innerGroupNames[i][0]] = innerGroup;
        innerGroup.parentGroup = mainGroup;
    }

    var properties = [
        ["AllowSeries", this.loc.PropertyMain.AllowSeries, this.CheckBox("seriesInteractionAllowSeries"), mainGroup, "Checkbox"],
        ["AllowSeriesElements", this.loc.PropertyMain.AllowSeriesElements, this.CheckBox("seriesInteractionAllowSeriesElements"), mainGroup, "Checkbox"],
        ["DrillDownEnabled", this.loc.PropertyMain.DrillDownEnabled, this.CheckBox("seriesInteractionDrillDownEnabled"), mainGroup, "Checkbox"],
        ["DrillDownPage", this.loc.PropertyMain.DrillDownPage, this.PropertyDropDownList("seriesInteractionDrillDownPage", this.options.propertyControlWidth, this.GetDrillDownPageItems(), true), mainGroup, "DropdownList"],
        ["DrillDownReport", this.loc.PropertyMain.DrillDownReport, this.PropertyTextBox("seriesInteractionDrillDownReport", this.options.propertyControlWidth), mainGroup, "Textbox"],
        ["HyperlinkDataColumn", this.loc.PropertyMain.HyperlinkDataColumn, this.PropertyDataControl("seriesInteractionHyperlinkDataColumn", this.options.propertyControlWidth, true), mainGroup.innerGroups["Hyperlink"], "DataColumn"],
        ["Hyperlink", this.loc.PropertyMain.Hyperlink, this.PropertyExpressionControl("seriesInteractionHyperlink", this.options.propertyControlWidth), mainGroup.innerGroups["Hyperlink"], "Expression"],
        ["ListOfHyperlinks", this.loc.PropertyMain.ListOfHyperlinks, this.PropertyExpressionControl("seriesInteractionListOfHyperlinks", this.options.propertyControlWidth), mainGroup.innerGroups["Hyperlink"], "Expression"],
        ["TagDataColumn", this.loc.PropertyMain.TagDataColumn, this.PropertyDataControl("seriesInteractionTagDataColumn", this.options.propertyControlWidth, true), mainGroup.innerGroups["Tag"], "DataColumn"],
        ["Tag", this.loc.PropertyMain.Tag, this.PropertyExpressionControl("seriesInteractionTag", this.options.propertyControlWidth), mainGroup.innerGroups["Tag"], "Expression"],
        ["ListOfTags", this.loc.PropertyMain.ListOfTags, this.PropertyExpressionControl("seriesInteractionListOfTags", this.options.propertyControlWidth), mainGroup.innerGroups["Tag"], "Expression"],
        ["ToolTipDataColumn", this.loc.PropertyMain.ToolTipDataColumn, this.PropertyDataControl("seriesInteractionToolTipDataColumn", this.options.propertyControlWidth, true), mainGroup.innerGroups["Tooltip"], "DataColumn"],
        ["ToolTip", this.loc.PropertyMain.ToolTip, this.PropertyExpressionControl("seriesInteractionToolTip", this.options.propertyControlWidth), mainGroup.innerGroups["Tooltip"], "Expression"],
        ["ListOfToolTips", this.loc.PropertyMain.ListOfToolTips, this.PropertyExpressionControl("seriesInteractionListOfToolTips", this.options.propertyControlWidth), mainGroup.innerGroups["Tooltip"], "Expression"]
    ]

    for (var i = 0; i < properties.length; i++) {
        var property = this.AddPropertyToPropertiesGroup(properties[i][0], properties[i][1], properties[i][2], properties[i][3], properties[i][4]);
        property.panelName = "Series";
        property.ownerName = "Interaction";
    }

    for (var i = 0; i < innerGroupNames.length; i++) {
        mainGroup.innerGroups[innerGroupNames[i][0]].parentGroup.container.appendChild(mainGroup.innerGroups[innerGroupNames[i][0]]);
    }
}

StiMobileDesigner.prototype.AddSeriesTopNProperties = function (chartPropertiesPanel) {
    var panel = chartPropertiesPanel.innerPanels["Series"].innerPanels["TopN"];

    var groupNames = [
        ["Main", this.loc.PropertyMain.TopN]
    ]

    this.AddGroupsToPropertiesPanel(groupNames, panel);

    var properties = [
        ["Count", this.loc.PropertyMain.Count, this.PropertyTextBox("seriesTopNCount", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["Mode", this.loc.PropertyMain.Mode,
            this.PropertyDropDownList("seriesTopNMode", this.options.propertyControlWidth, this.GetTopNModeItems(), true), panel.groups["Main"], "DropdownList"],
        ["OthersText", this.loc.PropertyMain.OthersText, this.PropertyTextBox("seriesTopNOthersText", this.options.propertyControlWidth), panel.groups["Main"], "Textbox"],
        ["ShowOthers", this.loc.PropertyMain.ShowOthers, this.CheckBox("seriesTopNShowOthers"), panel.groups["Main"], "Checkbox"]
    ]
    for (var i = 0; i < properties.length; i++) {
        var property = this.AddPropertyToPropertiesGroup(properties[i][0], properties[i][1], properties[i][2], properties[i][3], properties[i][4]);
        property.panelName = "Series";
        property.ownerName = "TopN";
    }
}

StiMobileDesigner.prototype.AddAreaAllProperties = function (chartPropertiesPanel) {
    var panel = chartPropertiesPanel.innerPanels["Area"].innerPanels["AllProperties"];

    var groupNames = [
        ["Main", this.loc.PropertyCategory.MainCategory],
        ["DateTimeStep", this.loc.PropertyMain.DateTimeStep],
        ["Interaction", this.loc.PropertyMain.Interaction],
        ["Labels", this.loc.Chart.Labels],
        ["Range", this.loc.Chart.Range],
        ["Ticks", this.loc.PropertyMain.Ticks],
        ["Title", this.loc.PropertyCategory.TitleCategory]
    ]

    this.AddGroupsToPropertiesPanel(groupNames, panel);
    panel.groups["Main"].changeOpenedState(true);

    var properties = [
        ["AllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("areaAllowApplyStyle"), panel.groups["Main"], "Checkbox"],
        ["ArrowStyle", this.loc.PropertyMain.ArrowStyle, this.PropertyDropDownList("areaArrowStyle", this.options.propertyControlWidth, this.GetXAxisArrowStyleItems(), true),
            panel.groups["Main"], "DropdownList"],
        ["Brush", this.loc.PropertyMain.Brush, this.PropertyBrushControl("areaBrush", null, this.options.propertyControlWidth), panel.groups["Main"], "Brush"],
        ["BorderColor", this.loc.PropertyMain.BorderColor, this.PropertyColorControl("areaBorderColor", null, this.options.propertyControlWidth), panel.groups["Main"], "Color"],
        ["BorderThickness", this.loc.PropertyMain.BorderThickness, this.PropertyTextBox("areaBorderThickness", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["ColorEach", this.loc.PropertyMain.ColorEach, this.CheckBox("areaColorEach"), panel.groups["Main"], "Checkbox"],
        ["RadarStyle", this.loc.PropertyMain.RadarStyle,
            this.PropertyDropDownList("areaCommonRadarStyle", this.options.propertyControlWidth, this.GetRadarStyleItems(), true), panel.groups["Main"], "DropdownList"],
        ["ReverseHor", this.loc.PropertyMain.ReverseHor, this.CheckBox("areaReverseHor"), panel.groups["Main"], "Checkbox"],
        ["ReverseVert", this.loc.PropertyMain.ReverseVert, this.CheckBox("areaReverseVert"), panel.groups["Main"], "Checkbox"],
        ["ShowShadow", this.loc.PropertyMain.ShowShadow, this.CheckBox("areaShowShadow"), panel.groups["Main"], "Checkbox"],
        ["SideBySide", this.loc.PropertyMain.SideBySide, this.CheckBox("areaSideBySide"), panel.groups["Main"], "Checkbox"],
        ["DateTimeStep.Interpolation", this.loc.PropertyMain.Interpolation, this.CheckBox("areaDateTimeStepInterpolation"), panel.groups["DateTimeStep"], "Checkbox"],
        ["DateTimeStep.NumberOfValues", this.loc.PropertyMain.NumberOfValues,
            this.PropertyTextBox("areaDateTimeStepNumberOfValues", this.options.propertyNumbersControlWidth), panel.groups["DateTimeStep"], "Textbox"],
        ["DateTimeStep.Step", this.loc.PropertyMain.Step,
            this.PropertyDropDownList("areaDateTimeStep", this.options.propertyControlWidth, this.GetXAxisStepItems(), true), panel.groups["DateTimeStep"], "DropdownList"],
        ["Interaction.RangeScrollEnabled", this.loc.PropertyMain.RangeScrollEnabled, this.CheckBox("areaInteractionRangeScrollEnabled"), panel.groups["Interaction"], "Checkbox"],
        ["Interaction.ShowScrollBar", this.loc.PropertyMain.ShowScrollBar, this.CheckBox("areaInteractionShowScrollBar"), panel.groups["Interaction"], "Checkbox"],
        ["Labels.Brush", this.loc.PropertyMain.Brush,
            this.PropertyBrushControl("areaLabelsBrush", null, this.options.propertyControlWidth), panel.groups["Labels"], "Brush"],
        ["Labels.AllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("areaLabelsAllowApplyStyle"), panel.groups["Labels"], "Checkbox"],
        ["Labels.Angle", this.loc.PropertyMain.Angle, this.PropertyTextBox("areaLabelsAngle", this.options.propertyNumbersControlWidth), panel.groups["Labels"], "Textbox"],
        ["Labels.Antialiasing", this.loc.PropertyMain.Antialiasing, this.CheckBox("areaLabelsAntialiasing"), panel.groups["Labels"], "Checkbox"],
        ["Labels.Color", this.loc.PropertyMain.Color,
            this.PropertyColorControl("areaLabelsColor", null, this.options.propertyControlWidth), panel.groups["Labels"], "Color"],
        ["Labels.DrawBorder", this.loc.PropertyMain.DrawBorder, this.CheckBox("areaLabelsDrawBorder"), panel.groups["Labels"], "Checkbox"],
        ["Labels.Font", this.loc.PropertyMain.Font, this.PropertyFontControl("areaLabelsFont", null, true), panel.groups["Labels"], "Font"],
        ["Labels.Format", this.loc.PropertyMain.Format, this.PropertyDropDownList("areaLabelsFormat", this.options.propertyControlWidth, this.GetTableHeaderFormatItems(), false), panel.groups["Labels"], "DropdownList"],
        ["Labels.RotationLabels", this.loc.PropertyMain.RotationLabels, this.CheckBox("areaLabelsDrawBorder"), panel.groups["Labels"], "Checkbox"],
        ["Labels.Placement", this.loc.PropertyMain.Placement,
            this.PropertyDropDownList("areaLabelsPlacement", this.options.propertyControlWidth, this.GetXAxisPlacementItems(), true), panel.groups["Labels"], "DropdownList"],
        ["Labels.Step", this.loc.PropertyMain.Step, this.PropertyTextBox("areaLabelsStep", this.options.propertyNumbersControlWidth), panel.groups["Labels"], "Textbox"],
        ["Labels.TextAfter", this.loc.PropertyMain.TextAfter, this.PropertyTextBox("areaLabelsTextAfter", this.options.propertyControlWidth), panel.groups["Labels"], "Textbox"],
        ["Labels.TextAlignment", this.loc.PropertyMain.TextAlignment,
            this.PropertyDropDownList("areaLabelsTextAlignment", this.options.propertyControlWidth, this.GetXAxisTextAlignmentItems(), true), panel.groups["Labels"], "DropdownList"],
        ["Labels.TextBefore", this.loc.PropertyMain.TextBefore, this.PropertyTextBox("areaLabelsTextBefore", this.options.propertyControlWidth), panel.groups["Labels"], "Textbox"],
        ["Labels.Width", this.loc.PropertyMain.Width, this.PropertyTextBox("areaLabelsWidth", this.options.propertyNumbersControlWidth), panel.groups["Labels"], "Textbox"],
        ["Labels.WordWrap", this.loc.PropertyMain.WordWrap, this.CheckBox("areaLabelsWordWrap"), panel.groups["Labels"], "Checkbox"],
        ["LineColor", this.loc.PropertyMain.LineColor,
            this.PropertyColorControl("areaLabelsColor", null, this.options.propertyControlWidth), panel.groups["Main"], "Color"],
        ["LineStyle", this.loc.PropertyMain.LineStyle,
            this.PropertyDropDownList("areaLineStyle", this.options.propertyControlWidth, this.GetBorderStyleItems(), true, true), panel.groups["Main"], "DropdownList"],
        ["LineWidth", this.loc.PropertyMain.LineWidth, this.PropertyTextBox("areaLineWidth", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["LogarithmicScale", this.loc.PropertyMain.LogarithmicScale, this.CheckBox("areaLogarithmicScale"), panel.groups["Main"], "Checkbox"],
        ["Range.Auto", this.loc.PropertyMain.Auto, this.CheckBox("areaRangeAuto"), panel.groups["Range"], "Checkbox"],
        ["Range.Minimum", this.loc.PropertyMain.Minimum, this.PropertyTextBox("areaRangeMinimum", this.options.propertyNumbersControlWidth), panel.groups["Range"], "Textbox"],
        ["Range.Maximum", this.loc.PropertyMain.Maximum, this.PropertyTextBox("areaRangeMaximum", this.options.propertyNumbersControlWidth), panel.groups["Range"], "Textbox"],
        ["ShowEdgeValues", this.loc.PropertyMain.ShowEdgeValues, this.CheckBox("areaShowEdgeValues"), panel.groups["Main"], "Checkbox"],
        ["ShowXAxis", this.loc.PropertyMain.ShowXAxis,
            this.PropertyDropDownList("areaShowXAxis", this.options.propertyControlWidth, this.GetShowXAxisItems(), true), panel.groups["Main"], "DropdownList"],
        ["StartFromZero", this.loc.PropertyMain.StartFromZero, this.CheckBox("areaStartFromZero"), panel.groups["Main"], "Checkbox"],
        ["Ticks.Length", this.loc.PropertyMain.Length, this.PropertyTextBox("areaTicksLength", this.options.propertyNumbersControlWidth), panel.groups["Ticks"], "Textbox"],
        ["Ticks.LengthUnderLabels", this.loc.PropertyMain.LengthUnderLabels, this.PropertyTextBox("areaTicksLengthUnderLabels", this.options.propertyNumbersControlWidth), panel.groups["Ticks"], "Textbox"],
        ["Ticks.MinorCount", this.loc.PropertyMain.MinorCount, this.PropertyTextBox("areaTicksMinorCount", this.options.propertyNumbersControlWidth), panel.groups["Ticks"], "Textbox"],
        ["Ticks.MinorLength", this.loc.PropertyMain.MinorLength, this.PropertyTextBox("areaTicksMinorLength", this.options.propertyNumbersControlWidth), panel.groups["Ticks"], "Textbox"],
        ["Ticks.MinorVisible", this.loc.PropertyMain.MinorVisible, this.CheckBox("areaTicksMinorVisible"), panel.groups["Ticks"], "Checkbox"],
        ["Ticks.Step", this.loc.PropertyMain.Step, this.PropertyTextBox("areaTicksStep", this.options.propertyNumbersControlWidth), panel.groups["Ticks"], "Textbox"],
        ["Ticks.Visible", this.loc.PropertyMain.Visible, this.CheckBox("areaTicksVisible"), panel.groups["Ticks"], "Checkbox"],
        ["Title.Alignment", this.loc.PropertyMain.Alignment,
            this.PropertyDropDownList("areaTitleAlignment", this.options.propertyControlWidth, this.GetXAxisTitleAlignmentItems(), true), panel.groups["Title"], "DropdownList"],
        ["Title.AllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox("areaTitleAllowApplyStyle"), panel.groups["Title"], "Checkbox"],
        ["Title.Antialiasing", this.loc.PropertyMain.Antialiasing, this.CheckBox("areaTitleAntialiasing"), panel.groups["Title"], "Checkbox"],
        ["Title.Color", this.loc.PropertyMain.Color,
            this.PropertyColorControl("areaTitleColor", null, this.options.propertyControlWidth), panel.groups["Title"], "Color"],
        ["Title.Direction", this.loc.PropertyMain.Direction,
            this.PropertyDropDownList("areaTitleDirection", this.options.propertyControlWidth, this.GetXAxisTitleDirectionItems(), true), panel.groups["Title"], "DropdownList"],
        ["Title.Font", this.loc.PropertyMain.Font, this.PropertyFontControl("areaTitleFont", null, true), panel.groups["Title"], "Font"],
        ["Title.Text", this.loc.PropertyMain.Text, this.PropertyTextBox("areaTitleText", this.options.propertyControlWidth), panel.groups["Title"], "Textbox"],
        ["Color", this.loc.PropertyMain.Color, this.PropertyColorControl("areaColor", null, this.options.propertyControlWidth), panel.groups["Main"], "Color"],
        ["MinorColor", this.loc.PropertyMain.MinorColor, this.PropertyColorControl("areaMinorColor", null, this.options.propertyControlWidth), panel.groups["Main"], "Color"],
        ["InterlacedBrush", this.loc.PropertyMain.InterlacedBrush,
            this.PropertyBrushControl("areaInterlacedBrush", null, this.options.propertyControlWidth), panel.groups["Main"], "Brush"],
        ["MinorCount", this.loc.PropertyMain.MinorCount, this.PropertyTextBox("areaMinorCount", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["MinorStyle", this.loc.PropertyMain.MinorStyle,
            this.PropertyDropDownList("areaMinorStyle", this.options.propertyControlWidth, this.GetBorderStyleItems(), true, true), panel.groups["Main"], "DropdownList"],
        ["MinorVisible", this.loc.PropertyMain.MinorVisible, this.CheckBox("areaMinorVisible"), panel.groups["Main"], "Checkbox"],
        ["Style", this.loc.PropertyMain.Style,
            this.PropertyDropDownList("areaStyle", this.options.propertyControlWidth, this.GetBorderStyleItems(), true, true), panel.groups["Main"], "DropdownList"],
        ["Visible", this.loc.PropertyMain.Visible, this.CheckBox("areaVisible"), panel.groups["Main"], "Checkbox"]
    ]

    for (var i = 0; i < properties.length; i++) {
        var property = this.AddPropertyToPropertiesGroup(properties[i][0], properties[i][1], properties[i][2], properties[i][3], properties[i][4]);
        property.panelName = "Area";
    }
}

StiMobileDesigner.prototype.AddLabelsCommonProperties = function (chartPropertiesPanel, isSeriesLabels) {
    var panel = chartPropertiesPanel.innerPanels[isSeriesLabels ? "Series" : "Labels"].innerPanels[isSeriesLabels ? "SeriesLabels" : "Common"];
    var prefix = isSeriesLabels ? "seriesLabels" : "labels";

    var groupNames = [
        ["Main", this.loc.PropertyCategory.SeriesLabelsCategory]
    ]

    this.AddGroupsToPropertiesPanel(groupNames, panel);
    panel.groups["Main"].changeOpenedState(true);

    var properties = [
        ["ShowSeriesLabels", this.loc.PropertyMain.ShowSeriesLabels, this.PropertyDropDownList(prefix + "SeriesLabelsShowSeriesLabels", this.options.propertyControlWidth, this.GetShowSeriesLabelsItems(), true), panel.groups["Main"], "DropdownList"],
        ["SeriesLabelsType", this.loc.PropertyMain.Type, this.PropertyChartSeriesLabelsControl(prefix + "SeriesLabelsType", this.options.propertyControlWidth), panel.groups["Main"], "DropdownList"],
        ["AllowApplyStyle", this.loc.PropertyMain.AllowApplyStyle, this.CheckBox(prefix + "AllowApplyStyle"), panel.groups["Main"], "Checkbox"],
        ["Angle", this.loc.PropertyMain.Angle, this.PropertyTextBox(prefix + "Angle", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["Antialiasing", this.loc.PropertyMain.Antialiasing, this.CheckBox(prefix + "Antialiasing"), panel.groups["Main"], "Checkbox"],
        ["AutoRotate", this.loc.PropertyMain.AutoRotate, this.CheckBox(prefix + "AutoRotate"), panel.groups["Main"], "Checkbox"],
        ["DrawBorder", this.loc.PropertyMain.DrawBorder, this.CheckBox(prefix + "DrawBorder"), panel.groups["Main"], "Checkbox"],
        ["BorderColor", this.loc.PropertyMain.BorderColor,
            this.PropertyColorControl(prefix + "BorderColor", null, this.options.propertyControlWidth), panel.groups["Main"], "Color"],
        ["Brush", this.loc.PropertyMain.Brush,
            this.PropertyBrushControl(prefix + "Brush", null, this.options.propertyControlWidth), panel.groups["Main"], "Brush"],
        ["Font", this.loc.PropertyMain.Font, this.PropertyFontControl(prefix + "Font", null, true), panel.groups["Main"], "Font"],
        ["Format", this.loc.PropertyMain.Format, this.PropertyDropDownList(prefix + "Format", this.options.propertyControlWidth, this.GetTableHeaderFormatItems(), false), panel.groups["Main"], "DropdownList"],
        ["LabelColor", this.loc.PropertyMain.LabelColor,
            this.PropertyColorControl(prefix + "LabelColor", null, this.options.propertyControlWidth), panel.groups["Main"], "Color"],
        ["LineColor", this.loc.PropertyMain.LineColor,
            this.PropertyColorControl(prefix + "LineColor", null, this.options.propertyControlWidth), panel.groups["Main"], "Color"],
        ["LegendValueType", this.loc.PropertyMain.LegendValueType,
            this.PropertyDropDownList(prefix + "LegendValueType", this.options.propertyControlWidth, this.GetLegendValueTypeItems(), true), panel.groups["Main"], "DropdownList"],
        ["LineLength", this.loc.PropertyMain.Length, this.PropertyTextBox(prefix + "LineLength", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["MarkerAlignment", this.loc.PropertyMain.MarkerAlignment,
            this.PropertyDropDownList(prefix + "MarkerAlignment", this.options.propertyControlWidth, this.GetXAxisTextAlignmentItems(), true), panel.groups["Main"], "DropdownList"],
        ["MarkerSize", this.loc.PropertyMain.MarkerSize, this.PropertySizeControl(prefix + "MarkerSize", this.options.propertyNumbersControlWidth + 40), panel.groups["Main"], "Size"],
        ["MarkerVisible", this.loc.PropertyMain.MarkerVisible, this.CheckBox(prefix + "MarkerVisible"), panel.groups["Main"], "Checkbox"],
        ["PreventIntersection", this.loc.PropertyMain.PreventIntersection, this.CheckBox(prefix + "PreventIntersection"), panel.groups["Main"], "Checkbox"],
        ["ShowInPercent", this.loc.PropertyMain.ShowInPercent, this.CheckBox(prefix + "ShowInPercent"), panel.groups["Main"], "Checkbox"],
        ["ShowNulls", this.loc.PropertyMain.ShowNulls, this.CheckBox(prefix + "ShowNulls"), panel.groups["Main"], "Checkbox"],
        ["ShowZeros", this.loc.PropertyMain.ShowZeros, this.CheckBox(prefix + "ShowZeros"), panel.groups["Main"], "Checkbox"],
        ["Step", this.loc.PropertyMain.Step, this.PropertyTextBox(prefix + "Step", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["TextAfter", this.loc.PropertyMain.TextAfter, this.PropertyTextBox(prefix + "TextAfter", this.options.propertyControlWidth), panel.groups["Main"], "Textbox"],
        ["TextBefore", this.loc.PropertyMain.TextBefore, this.PropertyTextBox(prefix + "TextBefore", this.options.propertyControlWidth), panel.groups["Main"], "Textbox"],
        ["UseSeriesColor", this.loc.PropertyMain.UseSeriesColor, this.CheckBox(prefix + "UseSeriesColor"), panel.groups["Main"], "Checkbox"],
        ["ValueType", this.loc.PropertyMain.ValueType,
            this.PropertyDropDownList(prefix + "ValueType", this.options.propertyControlWidth, this.GetLegendValueTypeItems(), true), panel.groups["Main"], "DropdownList"],
        ["ValueTypeSeparator", this.loc.PropertyMain.ValueTypeSeparator, this.PropertyTextBox(prefix + "ValueTypeSeparator", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["Visible", this.loc.PropertyMain.Visible, this.CheckBox(prefix + "Visible"), panel.groups["Main"], "Checkbox"],
        ["Width", this.loc.PropertyMain.Width, this.PropertyTextBox(prefix + "Width", this.options.propertyNumbersControlWidth), panel.groups["Main"], "Textbox"],
        ["WordWrap", this.loc.PropertyMain.WordWrap, this.CheckBox(prefix + "WordWrap"), panel.groups["Main"], "Checkbox"]
    ]

    for (var i = 0; i < properties.length; i++) {
        var property = this.AddPropertyToPropertiesGroup(properties[i][0], properties[i][1], properties[i][2], properties[i][3], properties[i][4]);
        property.panelName = isSeriesLabels ? "SeriesLabels" : "Labels";
    }
}

StiMobileDesigner.prototype.AddGroupsToPropertiesPanel = function (groupNames, panel, childGroups) {
    panel.groups = {};

    for (var i = 0; i < groupNames.length; i++) {
        var group = this.PropertiesGroup(null, groupNames[i][1]);
        panel.groups[groupNames[i][0]] = group;
        panel.appendChild(group);
        group.properties = {};
    }

    if (groupNames.length > 0) {
        panel.groups[groupNames[0][0]].changeOpenedState(true);
    }
}

StiMobileDesigner.prototype.AddPropertyToPropertiesGroup = function (propertyName, propertyCaption, propertyControl, propertyGroup, controlType) {
    var jsObject = this;
    var property = this.Property(null, propertyCaption, propertyControl);
    property.isUserProperty = true;
    property.name = propertyName;
    property.control = propertyControl;
    property.controlType = controlType;
    property.updateCaption();
    propertyControl.property = property;
    propertyGroup.properties[propertyName] = property;
    propertyGroup.container.appendChild(property);

    property.getValue = function () {
        var type = property.controlType;
        if (type == "DropdownList" || type == "Color" || type == "Brush" || type == "Border" || type == "Font" || type == "Filters" || type == "Sort" ||
            type == "Conditions" || type == "TextFormat" || type == "Interaction" || type == "ImageControl" || type == "TrendLines" || type == "SeriesLabelsType")
            return property.control.key;
        else if (type == "Margins" || type == "Size" || type == "CornerRadius") return property.control.getValue();
        else if (type == "Checkbox") return property.control.isChecked;
        else if (type == "DataColumn" || type == "Expression") return "Base64Code;" + StiBase64.encode(property.control.textBox.value);
        else if (type == "BusinessObject") return (property.control.textBox.value == this.jsObject.loc.Report.NotAssigned ? "" : property.control.textBox.value);
        else if (type == "Textbox") return "Base64Code;" + StiBase64.encode(property.control.value);

        return null;
    }

    propertyControl.action = function () {
        var forms = jsObject.options.forms;
        var ownerName = null;

        if (forms.editChart && forms.editChart.visible) {
            var areaPropertiesContainer = forms.editChart.areaPropertiesContainer;

            if (propertyControl.property.panelName == "Area" && areaPropertiesContainer.selectedItem && areaPropertiesContainer.selectedItem.name != "Common") {
                ownerName = areaPropertiesContainer.selectedItem.name;
            }

            if (propertyControl.property.panelName == "Chart" && propertyControl.property.name == "DataSource") {
                var chartCommonPanel = jsObject.options.propertiesPanel.editChartPropertiesPanel.innerPanels["Chart"].innerPanels["Common"];
                chartCommonPanel.updateDataRelationControl();
                chartCommonPanel.groups["Data"].properties["Sort"].control.setKey("");
            }
        }

        if (propertyControl.property.name == "SeriesLabelsType") {
            var seriesIndex = -1;
            var componentName = "";
            if (forms.editChartSeriesForm && forms.editChartSeriesForm.visible) {
                seriesIndex = forms.editChartSeriesForm.seriesContainer.getSelectedItemIndex();
                componentName = forms.editChartSeriesForm.chartProperties.name;
            }
            else if (forms.editChartSimpleForm && forms.editChartSimpleForm.visible) {
                seriesIndex = forms.editChartSimpleForm.controls.valuesBlock.container.getSelectedItemIndex();
                componentName = forms.editChartSimpleForm.chartProperties.name;
            }
            jsObject.SendCommandSetLabelsType({
                componentName: componentName,
                labelsType: propertyControl.key,
                seriesIndex: seriesIndex
            });
            return;
        }

        jsObject.options.propertiesPanel.editChartPropertiesPanel.updatePropertiesUsedInStyles();
        jsObject.SendChartPropertyValue(propertyControl.property, ownerName);
    }

    return property;
}

StiMobileDesigner.prototype.SetPropertyValue = function (property, value) {
    if (typeof (value) == "string" && value.indexOf("Base64Code;") == 0) {
        value = value.replace("Base64Code;", "");
        value = StiBase64.decode(value);
    }

    var type = property.controlType;
    if (type == "DropdownList" || type == "Color" || type == "Brush" || type == "Border" || type == "Font" || type == "Filters" || type == "Sort" ||
        type == "Conditions" || type == "TextFormat" || type == "Interaction" || type == "ImageControl" || type == "TrendLines" || type == "SeriesLabelsType") {
        property.control.setKey(value);
    }
    else if (type == "Margins" || type == "Size" || type == "CornerRadius") { property.control.setValue(value); }
    else if (type == "Checkbox") { property.control.setChecked(value); }
    else if (type == "DataColumn" || type == "Expression") { property.control.textBox.value = value; }
    else if (type == "BusinessObject") { property.control.textBox.value = value == "" ? this.loc.Report.NotAssigned : value; }
    else if (type == "Textbox") { property.control.value = value; }
}

StiMobileDesigner.prototype.SendChartPropertyValue = function (property, ownerName) {
    var jsObject = this;
    var forms = jsObject.options.forms;

    var params = {
        panelName: property.panelName,
        propertyName: property.name,
        propertyValue: property.getValue()
    }

    if (property.ownerName || ownerName)
        params.ownerName = property.ownerName || ownerName;

    if (forms.editChart && forms.editChart.visible) {
        params.componentName = forms.editChart.chartProperties.name;

        if (property.panelName == "Series" || property.panelName == "SeriesLabels")
            params.seriesIndex = forms.editChart.seriesContainer.getSelectedIndex();

        if (property.panelName == "Chart" && (property.ownerName == "ConstantLines" || property.ownerName == "Strips"))
            params["index" + property.ownerName] = forms.editChart[property.ownerName + "Container"].getSelectedIndex();
    }
    else if (forms.editChartSimpleForm && forms.editChartSimpleForm.visible) {
        params.componentName = forms.editChartSimpleForm.chartProperties.name;
        params.seriesIndex = forms.editChartSimpleForm.controls.valuesBlock.container.getSelectedItemIndex() || 0;
    }
    else if (forms.editChartSeriesForm && forms.editChartSeriesForm.visible) {
        var seriesForm = forms.editChartSeriesForm;
        params.componentName = seriesForm.chartProperties.name;
        params.seriesIndex = seriesForm.seriesContainer.getSelectedItemIndex() || 0;
    }
    else if (forms.editChartStripsForm && forms.editChartStripsForm.visible) {
        var stripsForm = forms.editChartStripsForm;
        params.componentName = stripsForm.chartProperties.name;
        params.indexStrips = stripsForm.stripsContainer.getSelectedItemIndex() || 0;
    }
    else if (forms.editChartConstantLinesForm && forms.editChartConstantLinesForm.visible) {
        var constantLinesForm = forms.editChartConstantLinesForm;
        params.componentName = constantLinesForm.chartProperties.name;
        params.indexConstantLines = constantLinesForm.constantLinesContainer.getSelectedItemIndex() || 0;
    }

    jsObject.SendCommandSetChartPropertyValue(params);
}

//PropertyChartDataDropDownList
StiMobileDesigner.prototype.PropertyChartDataDropDownList = function (name, width, toolTip) {
    var propertyDataDropDownList = this.PropertyDataDropDownList(name, width, toolTip);

    //Override
    propertyDataDropDownList.reset = function () {
        this.setKey("");
    }

    return propertyDataDropDownList;
}