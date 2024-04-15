
StiMobileDesigner.prototype.InitializeStyleDesignerForm_ = function () {
    var jsObject = this;
    var styleDesignerForm = this.BaseFormPanel("styleDesignerForm", this.loc.Toolbars.StyleDesigner, 2, this.HelpLinks["styleDesigner"]);
    styleDesignerForm.container.style.paddingTop = "6px";

    //MainTable
    var mainTable = this.CreateHTMLTable();
    styleDesignerForm.container.appendChild(mainTable);
    mainTable.style.borderCollapse = "separate";

    //Toolbar
    var buttons = [
        ["addStyle", this.FormButton(null, null, this.loc.FormStyleDesigner.Add, null, null, null, null, null, "Down")],
        ["actions", this.FormButton(null, null, this.loc.FormDictionaryDesigner.Actions, null, null, null, null, "stiDesignerSmallButtonWithBorder", "Down")],
        ["getStyle", this.SmallButton(null, null, null, "Styles.StylesGet.png")],
        ["removeStyle", this.SmallButton(null, null, null, "Remove.png")],
        ["settings", this.SmallButton(null, null, null, "Settings.png", null, "Down")]
    ]

    var toolBar = styleDesignerForm.toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "0 0 0 8px";
    mainTable.addCell(toolBar);
        
    for (var i = 0; i < buttons.length; i++) {
        var button = buttons[i][1];
        button.style.margin = "4px";
        toolBar[buttons[i][0]] = button;
        toolBar.addCell(button);
    }

    var findTextbox = this.FindControl("styleDesignerFormFindTextbox", 150, this.options.isTouchDevice ? 28 : 24);
    styleDesignerForm.findTextbox = findTextbox;
    findTextbox.style.margin = "0 12px 0 3px";
    toolBar.addCell(findTextbox);

    findTextbox.onchange = function () {
        styleDesignerForm.stylesTree.findItems(this.getValue());
    }

    //Get Style
    toolBar.getStyle.action = function () {
        var componentsNames = styleDesignerForm.getSelectedComponentsNames();
        jsObject.SendCommandCreateStylesFromComponents(componentsNames);
    }

    //Remove Style
    toolBar.removeStyle.action = function () {
        var selectedItem = styleDesignerForm.stylesTree.selectedItem;
        if (selectedItem) selectedItem.remove();
    }

    //Actions
    var actionsMenu = this.InitializeStyleActionsMenu(styleDesignerForm);

    //Add Style Menu
    var addStyleMenu = this.VerticalMenu("addStyleMenu", styleDesignerForm.toolBar.addStyle, "Down", this.GetAddStyleMenuItems(), "stiDesignerMenuMiddleItem");
    addStyleMenu.innerContent.style.maxHeight = "450px";

    addStyleMenu.itemHasBasedStyle = function (styleType) {
        return (styleType == "StiChartStyle" || styleType == "StiGaugeStyle" || styleType == "StiMapStyle" || styleType == "StiCrossTabStyle" || styleType == "StiTableStyle" ||
            styleType == "StiCardsStyle" || styleType == "StiIndicatorStyle" || styleType == "StiProgressStyle");
    }

    addStyleMenu.action = function (menuItem) {
        this.changeVisibleState(false);

        if (this.itemHasBasedStyle(menuItem.key)) {
            var basedStyleForm = jsObject.options.forms.chooseBaseStyleForm || jsObject.ChooseBaseStyleForm();
            basedStyleForm.show(menuItem.key);
        }
        else {
            jsObject.SendCommandToDesignerServer("AddStyle", { type: menuItem.key }, function (answer) {
                if (answer.styleObject) {
                    jsObject.options.reportIsModified = true;
                    styleDesignerForm.stylesTree.addItem(answer.styleObject);
                }
            });
        }
    }

    //Settings
    this.InitializeSettingsStylesMenu(styleDesignerForm);

    //Items Tree
    styleDesignerForm.stylesTree = this.StylesTree(styleDesignerForm);
    var treeCell = mainTable.addCellInNextRow(styleDesignerForm.stylesTree);
    treeCell.style.width = "100%";
    treeCell.style.textAlign = "center";
    treeCell.style.padding = "12px";

    //Hint Text
    var createStylesHintItem = this.CreateStylesHintItem();
    styleDesignerForm.container.appendChild(createStylesHintItem);
    styleDesignerForm.createStylesHintItem = createStylesHintItem;

    createStylesHintItem.updateVisibleState = function () {
        var itemsCount = jsObject.GetCountObjects(styleDesignerForm.stylesTree.mainItem.childs);
        this.style.display = itemsCount > 0 || (itemsCount == 0 && findTextbox.getValue() != "") ? "none" : "";
    }

    createStylesHintItem.oncontextmenu = function (event) {
        return false;
    }

    createStylesHintItem.onmouseup = function (event) {
        toolBar.addStyle.action();
    }

    //PropertiesPanel
    styleDesignerForm.propertiesPanel = this.StyleDesignerFormPropertiesPanel(styleDesignerForm);
    var styleDesignerPropertiesPanel = this.options.propertiesPanel.styleDesignerPropertiesPanel;
    while (styleDesignerPropertiesPanel.childNodes[0]) styleDesignerPropertiesPanel.removeChild(styleDesignerPropertiesPanel.childNodes[0]);
    this.options.propertiesPanel.styleDesignerPropertiesPanel.appendChild(styleDesignerForm.propertiesPanel);

    //Methods
    toolBar.updateControls = function () {
        var selectedItem = styleDesignerForm.stylesTree.selectedItem;
        toolBar.removeStyle.setEnabled(selectedItem && selectedItem.itemObject.typeItem != "MainItem");

        var componentsNames = styleDesignerForm.getSelectedComponentsNames();
        toolBar.getStyle.setEnabled(componentsNames.length > 0);
    }

    styleDesignerForm.getSelectedComponentsNames = function () {
        var selectedComponents = jsObject.options.selectedObject ? [jsObject.options.selectedObject] : (jsObject.options.selectedObjects || []);
        var componentsNames = [];
        for (var i = 0; i < selectedComponents.length; i++) {
            if (selectedComponents[i].typeComponent != "StiPage" && selectedComponents[i].typeComponent != "StiReport") {
                componentsNames.push(selectedComponents[i].properties.name);
            }
        }
        return componentsNames;
    }
        
    styleDesignerForm.show = function (styleControl) {
        styleDesignerForm.changeVisibleState(true);
        styleDesignerForm.styleControl = styleControl;
        findTextbox.setValue("");

        this.settings = {
            filter: {
                "StiStyle": true,
                "StiChartStyle": true,
                "StiCrossTabStyle": true,
                "StiMapStyle": true,
                "StiGaugeStyle": true,
                "StiTableStyle": true,
                "StiCardsStyle": true,
                "StiDialogStyle": true,
                "StiIndicatorStyle": true,
                "StiProgressStyle": true
            },
            sort: "NoSorting"
        }
        jsObject.options.propertiesPanel.setStyleDesignerMode(true);
        this.copiedStyle = null;

        toolBar.settings.style.display = toolBar.addStyle.arrow.style.display = actionsMenu.items.createStyleCollection.style.display = styleDesignerForm.styleControl ? "none" : "";

        toolBar.addStyle.action = function () {
            if (styleDesignerForm.styleControl) {
                jsObject.SendCommandAddStyle("StiStyle");
            }
            else {
                addStyleMenu.changeVisibleState(!addStyleMenu.visible);
            }
        }

        this.stylesCollection = jsObject.CopyObject(styleDesignerForm.styleControl ? styleDesignerForm.styleControl.key : jsObject.options.report.stylesCollection);
        this.usingStyles = jsObject.GetAllUsingStyles();
        this.stylesTree.updateItems(this.stylesCollection);

        if (this.stylesTree.mainItem && this.stylesTree.mainItem.childsContainer.childNodes.length > 0)
            this.stylesTree.mainItem.childsContainer.childNodes[0].setSelected();
    }
       
    styleDesignerForm.addStylesCollection = function (newStylesCollection, collectionName, removeExistingStyles) {
        if (removeExistingStyles) this.stylesCollection = [];
        this.stylesCollection = this.stylesCollection.concat(newStylesCollection);
        this.stylesTree.openedItems[collectionName] = true;
        this.stylesTree.updateItems(this.stylesCollection, { typeItem: "Folder", collectionName: collectionName }, true, true);
    }

    styleDesignerForm.changeModalState = function (state) {
        jsObject.options.disabledPanels[this.level].changeVisibleState(state);
    }
    
    styleDesignerForm.onhide = function () {
        jsObject.options.propertiesPanel.setStyleDesignerMode(false);
    }

    styleDesignerForm.action = function () {
        this.changeVisibleState(false);

        if (styleDesignerForm.styleControl) {
            styleDesignerForm.styleControl.setKey(this.stylesCollection);
            styleDesignerForm.styleControl.action();
        }
        else {
            jsObject.SendCommandUpdateStyles(this.stylesCollection);
        }
    }

    return styleDesignerForm;
}

StiMobileDesigner.prototype.StyleDesignerFormPropertiesPanel = function (styleDesignerForm) {
    var propertiesPanel = document.createElement("div");
    propertiesPanel.className = "stiDesignerStyleDesignerFormPropertiesPanel";
    propertiesPanel.propertiesGroups = {};
    var jsObject = this;

    //Add Properties Groups
    var propertiesGroups = [
        ["main", this.loc.PropertyCategory.MainCategory, 1],
        ["appearance", this.loc.PropertyCategory.AppearanceCategory, 1],
        ["heatmap", this.loc.PropertyMain.Heatmap, 2],
        ["heatmapWithGroup", this.loc.PropertyEnum.StiMapTypeHeatmapWithGroup, 2],
        ["area", this.loc.PropertyCategory.AreaCategory, 1],
        ["axis", this.loc.PropertyCategory.AxisCategory, 1],
        ["gridLines", this.loc.Chart.GridLines, 1],
        ["interlacing", this.loc.PropertyCategory.InterlacingCategory, 1],
        ["legend", this.loc.PropertyCategory.LegendCategory, 1],
        ["series", this.loc.Chart.Series, 1],
        ["seriesLabels", this.loc.PropertyCategory.SeriesLabelsCategory, 1],
        ["toolTip", this.loc.PropertyMain.ToolTip, 1],
        ["trendLine", this.loc.PropertyCategory.TrendLineCategory, 1],
        ["parameters", this.loc.PropertyCategory.ParametersCategory, 1]
    ];

    for (var i = 0; i < propertiesGroups.length; i++) {
        var propertiesGroup = this.PropertiesGroup(propertiesGroups[i][0] + "StylesDesigner", propertiesGroups[i][1], null, propertiesGroups[i][2]);
        propertiesGroup.style.display = "none";
        propertiesPanel.propertiesGroups[propertiesGroups[i][0]] = propertiesGroup;
        propertiesPanel.appendChild(propertiesGroup);

        if (propertiesGroups[i][2] == 1) {
            propertiesGroup.changeOpenedState(true);
        }
    }

    //Add Properties Controls
    propertiesPanel.properties_ = {};
    var propAttributes = [
        ["name", this.loc.PropertyMain.Name, this.PropertyTextBox("styleDesignerPropertyName", this.options.propertyControlWidth), "main"],
        ["description", this.loc.PropertyMain.Description, this.PropertyTextBox("styleDesignerPropertyDescription", this.options.propertyControlWidth), "main"],
        ["allowUseBackColor", this.loc.PropertyMain.AllowUseBackColor, this.CheckBox("styleDesignerPropertyAllowUseBackColor"), "parameters"],
        ["allowUseBorderFormatting", this.loc.PropertyMain.AllowUseBorderFormatting, this.CheckBox("styleDesignerPropertyAllowUseBorderFormatting"), "parameters"],
        ["allowUseBorderSides", this.loc.PropertyMain.AllowUseBorderSides, this.CheckBox("styleDesignerPropertyAllowUseBorderSides"), "parameters"],
        ["allowUseBorderSidesFromLocation", this.loc.PropertyMain.AllowUseBorderSidesFromLocation, this.CheckBox("styleDesignerPropertyAllowUseBorderSidesFromLocation"), "parameters"],
        ["allowUseBrush", this.loc.PropertyMain.AllowUseBrush, this.CheckBox("styleDesignerPropertyAllowUseBrush"), "parameters"],
        ["allowUseFont", this.loc.PropertyMain.AllowUseFont, this.CheckBox("styleDesignerPropertyAllowUseFont"), "parameters"],
        ["allowUseForeColor", this.loc.PropertyMain.AllowUseForeColor, this.CheckBox("styleDesignerPropertyAllowUseForeColor"), "parameters"],
        ["allowUseHorAlignment", this.loc.PropertyMain.AllowUseHorAlignment, this.CheckBox("styleDesignerPropertyAllowUseHorAlignment"), "parameters"],
        ["allowUseImage", this.loc.PropertyMain.AllowUseImage, this.CheckBox("styleDesignerPropertyAllowUseImage"), "parameters"],
        ["allowUseNegativeTextBrush", this.loc.PropertyMain.AllowUseNegativeTextBrush, this.CheckBox("styleDesignerPropertyAllowUseNegativeTextBrush"), "parameters"],
        ["allowUseTextBrush", this.loc.PropertyMain.AllowUseTextBrush, this.CheckBox("styleDesignerPropertyAllowUseTextBrush"), "parameters"],
        ["allowUseTextFormat", this.loc.PropertyMain.AllowUseTextFormat, this.CheckBox("styleDesignerPropertyAllowUseTextFormat"), "parameters"],
        ["allowUseVertAlignment", this.loc.PropertyMain.AllowUseVertAlignment, this.CheckBox("styleDesignerPropertyAllowUseVertAlignment"), "parameters"],
        ["alternatingCellBackColor", this.loc.PropertyMain.AlternatingCellBackColor || "Alternating Cell Back Color", this.PropertyColorControl("styleDesignerPropertyAlternatingCellBackColor", null, this.options.propertyControlWidth), "appearance"],
        ["alternatingCellForeColor", this.loc.PropertyMain.AlternatingCellForeColor || "Alternating Cell Fore Color", this.PropertyColorControl("styleDesignerPropertyAlternatingCellForeColor", null, this.options.propertyControlWidth), "appearance"],
        ["alternatingDataColor", this.loc.PropertyMain.AlternatingDataColor || "Alternating Data Color", this.PropertyColorControl("styleDesignerPropertyAlternatingDataColor", null, this.options.propertyControlWidth), "appearance"],
        ["alternatingDataForeground", this.loc.PropertyMain.AlternatingDataForeground || "Alternating Data Foreground", this.PropertyColorControl("styleDesignerPropertyAlternatingDataForeground", null, this.options.propertyControlWidth), "appearance"],
        ["axisLabelsColor", this.loc.PropertyMain.AxisLabelsColor, this.PropertyColorControl("styleDesignerAxisLabelsColor", null, this.options.propertyControlWidth), "axis"],
        ["axisLineColor", this.loc.PropertyMain.AxisLineColor, this.PropertyColorControl("styleDesignerAxisLineColor", null, this.options.propertyControlWidth), "axis"],
        ["axisTitleColor", this.loc.PropertyMain.AxisTitleColor, this.PropertyColorControl("styleDesignerAxisTitleColor", null, this.options.propertyControlWidth), "axis"],
        ["backColor", this.loc.PropertyMain.BackColor, this.PropertyColorControl("styleDesignerPropertyBackColor", null, this.options.propertyControlWidth), "appearance"],
        ["bandColor", this.loc.PropertyMain.BandColor || "Band Color", this.PropertyColorControl("styleDesignerPropertyBandColor", null, this.options.propertyControlWidth), "appearance"],
        ["basicStyleColor", this.loc.PropertyMain.BasicStyleColor, this.PropertyColorControl("styleDesignerPropertyBasicStyleColor", null, this.options.propertyControlWidth), "appearance"],
        ["border", this.loc.PropertyMain.Borders, this.PropertyBorderControl("styleDesignerPropertyBorder", this.options.propertyControlWidth), "appearance"],
        ["borderColor", this.loc.PropertyMain.BorderColor, this.PropertyColorControl("styleDesignerPropertyBorderColor", null, this.options.propertyControlWidth), "appearance"],
        ["borderSize", this.loc.PropertyMain.BorderSize, this.PropertyTextBox("styleDesignerPropertyBorderSize", this.options.propertyNumbersControlWidth), "appearance"],
        ["borderWidth", this.loc.PropertyMain.BorderWidth, this.PropertyTextBox("styleDesignerPropertyBorderWidth", this.options.propertyNumbersControlWidth), "appearance"],
        ["brush", this.loc.PropertyMain.Brush, this.PropertyBrushControl("styleDesignerPropertyBrush", null, this.options.propertyControlWidth), "appearance"],
        ["brushType", this.loc.PropertyMain.BrushType, this.PropertyDropDownList("styleDesignerPropertyBrushType", this.options.propertyControlWidth, this.GetChartStyleBrushTypeItems(), true, false), "appearance"],
        ["cellBackColor", this.loc.PropertyMain.CellBackColor || "Cell Back Color", this.PropertyColorControl("styleDesignerPropertyCellBackColor", null, this.options.propertyControlWidth), "appearance"],
        ["cellForeColor", this.loc.PropertyMain.CellForeColor || "Cell Fore Color", this.PropertyColorControl("styleDesignerPropertyCellForeColor", null, this.options.propertyControlWidth), "appearance"],
        ["color", this.loc.PropertyMain.Color, this.PropertyColorControl("styleDesignerPropertyColor", null, this.options.propertyControlWidth), "appearance"],
        ["colors", this.loc.PropertyCategory.ColorsCategory, this.PropertyColorsCollectionControl("styleDesignerPropertyColors", null, this.options.propertyControlWidth), "appearance"],
        ["collectionName", this.loc.PropertyMain.CollectionName, this.PropertyTextBox("styleDesignerPropertyCollectionName", this.options.propertyControlWidth), "main"],
        ["conditions", this.loc.PropertyMain.Conditions, this.PropertyTextBoxWithEditButton("styleDesignerPropertyConditions", this.options.propertyControlWidth, true), "main"],
        ["columnHeaderBackColor", this.loc.PropertyMain.ColumnHeaderBackColor || "Column Header Back Color", this.PropertyColorControl("styleDesignerPropertyColumnHeaderBackColor", null, this.options.propertyControlWidth), "appearance"],
        ["columnHeaderForeColor", this.loc.PropertyMain.ColumnHeaderForeColor || "Column Header Fore Color", this.PropertyColorControl("styleDesignerPropertyColumnHeaderForeColor", null, this.options.propertyControlWidth), "appearance"],
        ["chartAreaShowShadow", this.loc.PropertyMain.ChartAreaShowShadow, this.CheckBox("styleDesignerAreaShowShadow"), "area"],
        ["chartAreaBorderColor", this.loc.PropertyMain.ChartAreaBorderColor, this.PropertyColorControl("styleDesignerAreaBorderColor", null, this.options.propertyControlWidth), "area"],
        ["chartAreaBrush", this.loc.PropertyMain.ChartAreaBrush, this.PropertyBrushControl("styleDesignerAreaBrush", null, this.options.propertyControlWidth), "area"],
        ["dataColor", this.loc.PropertyMain.DataColor || "Data Color", this.PropertyColorControl("styleDesignerPropertyDataColor", null, this.options.propertyControlWidth), "appearance"],
        ["dataForeground", this.loc.PropertyMain.DataForeground || "Data Foreground", this.PropertyColorControl("styleDesignerPropertyDataForeground", null, this.options.propertyControlWidth), "appearance"],
        ["defaultColor", this.loc.PropertyMain.DefaultColor, this.PropertyColorControl("styleDesignerPropertyDefaultColor", null, this.options.propertyControlWidth), "appearance"],
        ["font", this.loc.PropertyMain.Font, this.PropertyFontControl("styleDesignerPropertyFont", null, true), "appearance"],
        ["footerColor", this.loc.PropertyMain.FooterColor || "Footer Color", this.PropertyColorControl("styleDesignerPropertyFooterColor", null, this.options.propertyControlWidth), "appearance"],
        ["footerForeground", this.loc.PropertyMain.FooterForeground || "Footer Foreground", this.PropertyColorControl("styleDesignerPropertyFooterForeground", null, this.options.propertyControlWidth), "appearance"],
        ["foreColor", this.loc.PropertyMain.ForeColor, this.PropertyColorControl("styleDesignerPropertyForeColor", null, this.options.propertyControlWidth), "appearance"],
        ["gridLinesHorColor", this.loc.PropertyMain.GridLinesHorColor, this.PropertyColorControl("styleDesignerGridLinesHorColor", null, this.options.propertyControlWidth), "gridLines"],
        ["gridLinesVertColor", this.loc.PropertyMain.GridLinesVertColor, this.PropertyColorControl("styleDesignerGridLinesVertColor", null, this.options.propertyControlWidth), "gridLines"],
        ["gridColor", this.loc.PropertyMain.GridColor || "Grid Color", this.PropertyColorControl("styleDesignerPropertyGridColor", null, this.options.propertyControlWidth), "appearance"],
        ["glyphColor", this.loc.PropertyMain.GlyphColor || "Glyph Color", this.PropertyColorControl("styleDesignerPropertyGlyphColor", null, this.options.propertyControlWidth), "appearance"],
        ["heatmap.color", this.loc.PropertyMain.Color, this.PropertyColorControl("styleDesignerPropertyHeatmapColor", null, this.options.propertyControlWidth), "heatmap", 2],
        ["heatmap.zeroColor", this.loc.PropertyMain.ZeroColor, this.PropertyColorControl("styleDesignerPropertyHeatmapZeroColor", null, this.options.propertyControlWidth), "heatmap", 2],
        ["heatmap.mode", this.loc.PropertyMain.Mode, this.PropertyDropDownList("styleDesignerPropertyHeatmapMode", this.options.propertyControlWidth, this.GetHeatmapModeItems(), true, false), "heatmap", 2],
        ["heatmapWithGroup.colors", this.loc.PropertyMain.Colors, this.PropertyColorsCollectionControl("styleDesignerPropertyHeatmapWithGroupColors", null, this.options.propertyControlWidth), "heatmapWithGroup", 2],
        ["heatmapWithGroup.zeroColor", this.loc.PropertyMain.ZeroColor, this.PropertyColorControl("styleDesignerPropertyHeatmapWithGroupZeroColor", null, this.options.propertyControlWidth), "heatmapWithGroup", 2],
        ["heatmapWithGroup.mode", this.loc.PropertyMain.Mode, this.PropertyDropDownList("styleDesignerPropertyHeatmapWithGroupMode", this.options.propertyControlWidth, this.GetHeatmapModeItems(), true, false), "heatmapWithGroup", 2],
        ["headerColor", this.loc.PropertyMain.HeaderColor || "Header Color", this.PropertyColorControl("styleDesignerPropertyHeaderColor", null, this.options.propertyControlWidth), "appearance"],
        ["headerForeground", this.loc.PropertyMain.HeaderForeground || "Header Foreground", this.PropertyColorControl("styleDesignerPropertyHeaderForeground", null, this.options.propertyControlWidth), "appearance"],
        ["horAlignment", this.loc.PropertyMain.HorAlignment, this.PropertyDropDownList("styleDesignerPropertyHorizontalAlignment", this.options.propertyControlWidth, this.GetHorizontalAlignmentItems(), true, false), "appearance"],
        ["hotBackColor", this.loc.PropertyMain.HotBackColor || "Hot Back Color", this.PropertyColorControl("styleDesignerPropertyHotBackColor", null, this.options.propertyControlWidth), "appearance"],
        ["hotColumnHeaderBackColor", this.loc.PropertyMain.HotColumnHeaderBackColor || "Hot Column Header Back Color", this.PropertyColorControl("styleDesignerPropertyHotColumnHeaderBackColor", null, this.options.propertyControlWidth), "appearance"],
        ["hotForeColor", this.loc.PropertyMain.HotForeColor || "Hot Fore Color", this.PropertyColorControl("styleDesignerPropertyHotForeColor", null, this.options.propertyControlWidth), "appearance"],
        ["hotGlyphColor", this.loc.PropertyMain.HotGlyphColor || "Hot Glyph Color", this.PropertyColorControl("styleDesignerPropertyHotGlyphColor", null, this.options.propertyControlWidth), "appearance"],
        ["hotHeaderColor", this.loc.PropertyMain.HotHeaderColor || "Hot Header Color", this.PropertyColorControl("styleDesignerPropertyHotHeaderColor", null, this.options.propertyControlWidth), "appearance"],
        ["hotSelectedBackColor", this.loc.PropertyMain.HotSelectedBackColor || "Hot Selected Back Color", this.PropertyColorControl("styleDesignerPropertyHotSelectedBackColor", null, this.options.propertyControlWidth), "appearance"],
        ["hotSelectedForeColor", this.loc.PropertyMain.HotSelectedForeColor || "Hot Selected Fore Color", this.PropertyColorControl("styleDesignerPropertyHotSelectedForeColor", null, this.options.propertyControlWidth), "appearance"],
        ["hotSelectedGlyphColor", this.loc.PropertyMain.HotSelectedGlyphColor || "Hot Selected Glyph Color", this.PropertyColorControl("styleDesignerPropertyHotSelectedGlyphColor", null, this.options.propertyControlWidth), "appearance"],
        ["hotRowHeaderBackColor", this.loc.PropertyMain.HotRowHeaderBackColor || "Hot Row Header Back Color", this.PropertyColorControl("styleDesignerPropertyHotRowHeaderBackColor", null, this.options.propertyControlWidth), "appearance"],
        ["image", this.loc.PropertyMain.Image, this.PropertyImageControl("styleDesignerPropertyImage", this.options.propertyControlWidth), "appearance"],
        ["individualColor", this.loc.PropertyMain.IndividualColor, this.PropertyColorControl("styleDesignerPropertyIndividualColor", null, this.options.propertyControlWidth), "appearance"],
        ["interlacingHorBrush", this.loc.PropertyMain.InterlacingHorBrush, this.PropertyBrushControl("styleDesignerInterlacingHorBrush", null, this.options.propertyControlWidth), "interlacing"],
        ["interlacingVertBrush", this.loc.PropertyMain.InterlacingVertBrush, this.PropertyBrushControl("styleDesignerInterlacingVertBrush", null, this.options.propertyControlWidth), "interlacing"],
        ["labelForeground", this.loc.PropertyMain.LabelForeground, this.PropertyColorControl("styleDesignerPropertyLabelForeground", null, this.options.propertyControlWidth), "appearance"],
        ["labelShadowForeground", this.loc.PropertyMain.LabelShadowForeground, this.PropertyColorControl("styleDesignerPropertyLabelShadowForeground", null, this.options.propertyControlWidth), "appearance"],                
        ["legendBorderColor", this.loc.PropertyMain.LegendBorderColor, this.PropertyColorControl("styleDesignerLegendBorderColor", null, this.options.propertyControlWidth), "legend"],
        ["legendLabelsColor", this.loc.PropertyMain.LegendLabelsColor, this.PropertyColorControl("styleDesignerLegendLabelsColor", null, this.options.propertyControlWidth), "legend"],
        ["legendTitleColor", this.loc.PropertyMain.LegendTitleColor, this.PropertyColorControl("styleDesignerLegendTitleColor", null, this.options.propertyControlWidth), "legend"],
        ["legendBrush", this.loc.PropertyMain.LegendBrush, this.PropertyBrushControl("styleDesignerLegendBrush", null, this.options.propertyControlWidth), "legend"],
        ["linearBarBorderBrush", this.loc.PropertyMain.LinearBarBorderBrush || "Linear Bar Border Brush", this.PropertyBrushControl("styleDesignerPropertyLinearBarBorderBrush", null, this.options.propertyControlWidth), "appearance"],
        ["linearBarBrush", this.loc.PropertyMain.LinearBarBrush || "Linear Bar Brush", this.PropertyBrushControl("styleDesignerPropertyLinearBarBrush", null, this.options.propertyControlWidth), "appearance"],
        ["linearBarEmptyBorderBrush", this.loc.PropertyMain.LinearBarEmptyBorderBrush || "Linear Bar Empty Border Brush", this.PropertyBrushControl("styleDesignerPropertyLinearBarEmptyBorderBrush", null, this.options.propertyControlWidth), "appearance"],
        ["linearBarEmptyBrush", this.loc.PropertyMain.LinearBarEmptyBrush || "Linear Bar Empty Brush", this.PropertyBrushControl("styleDesignerPropertyLinearBarEmptyBrush", null, this.options.propertyControlWidth), "appearance"],
        ["linearScaleBrush", this.loc.PropertyMain.LinearScaleBrush || "Linear Scale Brush", this.PropertyBrushControl("styleDesignerPropertyLinearScaleBrush", null, this.options.propertyControlWidth), "appearance"],
        ["lineColor", this.loc.PropertyMain.LineColor, this.PropertyColorControl("styleDesignerLineColor", null, this.options.propertyControlWidth), "appearance"],
        ["lineSpacing", this.loc.PropertyMain.LineSpacing, this.PropertyTextBox("styleDesignerPropertyLineSpacing", this.options.propertyNumbersControlWidth), "appearance"],
        ["markerVisible", this.loc.PropertyMain.MarkerVisible, this.CheckBox("styleDesignerMarkerVisible"), "legend"],
        ["markerBrush", this.loc.PropertyMain.MarkerBrush || "Marker Brush", this.PropertyBrushControl("styleDesignerPropertyMarkerBrush", null, this.options.propertyControlWidth), "appearance"],
        ["needleBorderBrush", this.loc.PropertyMain.NeedleBorderBrush || "Needle Border Brush", this.PropertyBrushControl("styleDesignerPropertyNeedleBorderBrush", null, this.options.propertyControlWidth), "appearance"],
        ["needleBorderWidth", this.loc.PropertyMain.NeedleBorderWidth, this.PropertyTextBox("styleDesignerPropertyNeedleBorderWidth", this.options.propertyNumbersControlWidth), "appearance"],
        ["needleBrush", this.loc.PropertyMain.NeedleBrush || "Needle Brush", this.PropertyBrushControl("styleDesignerPropertyNeedleBrush", null, this.options.propertyControlWidth), "appearance"],
        ["needleCapBorderBrush", this.loc.PropertyMain.NeedleCapBorderBrush || "Needle Cap Border Brush", this.PropertyBrushControl("styleDesignerPropertyNeedleCapBorderBrush", null, this.options.propertyControlWidth), "appearance"],
        ["needleCapBrush", this.loc.PropertyMain.NeedleCapBrush || "Needle Cap Brush", this.PropertyBrushControl("styleDesignerPropertyNeedleCapBrush", null, this.options.propertyControlWidth), "appearance"],
        ["negativeColor", this.loc.PropertyMain.NegativeColor || "Negative Color", this.PropertyColorControl("styleDesignerPropertyNegativeColor", null, this.options.propertyControlWidth), "appearance"],
        ["positiveColor", this.loc.PropertyMain.PositiveColor || "Positive Color", this.PropertyColorControl("styleDesignerPropertyPositiveColor", null, this.options.propertyControlWidth), "appearance"],
        ["rowHeaderBackColor", this.loc.PropertyMain.RowHeaderBackColor || "Row Header Back Color", this.PropertyColorControl("styleDesignerPropertyRowHeaderBackColor", null, this.options.propertyControlWidth), "appearance"],
        ["rowHeaderForeColor", this.loc.PropertyMain.RowHeaderForeColor || "Row Header Fore Color", this.PropertyColorControl("styleDesignerPropertyRowHeaderForeColor", null, this.options.propertyControlWidth), "appearance"],
        ["radialBarBorderBrush", this.loc.PropertyMain.RadialBarBorderBrush || "Radial Bar Border Brush", this.PropertyBrushControl("styleDesignerPropertyRadialBarBorderBrush", null, this.options.propertyControlWidth), "appearance"],
        ["radialBarBrush", this.loc.PropertyMain.RadialBarBrush || "Radial Bar Brush", this.PropertyBrushControl("styleDesignerPropertyRadialBarBrush", null, this.options.propertyControlWidth), "appearance"],
        ["radialBarEmptyBorderBrush", this.loc.PropertyMain.RadialBarEmptyBorderBrush || "Radial Bar Empty Border Brush", this.PropertyBrushControl("styleDesignerPropertyRadialBarEmptyBorderBrush", null, this.options.propertyControlWidth), "appearance"],
        ["radialBarEmptyBrush", this.loc.PropertyMain.RadialBarEmptyBrush || "Radial Bar Empty Brush", this.PropertyBrushControl("styleDesignerPropertyRadialBarEmptyBrush", null, this.options.propertyControlWidth), "appearance"],
        ["seriesLabelsBorderColor", this.loc.PropertyMain.SeriesLabelsBorderColor, this.PropertyColorControl("styleDesignerSeriesLabelsBorderColor", null, this.options.propertyControlWidth), "seriesLabels"],
        ["seriesLabelsColor", this.loc.PropertyMain.SeriesLabelsColor, this.PropertyColorControl("styleDesignerSeriesLabelsColor", null, this.options.propertyControlWidth), "seriesLabels"],
        ["seriesLabelsBrush", this.loc.PropertyMain.SeriesLabelsBrush, this.PropertyBrushControl("styleDesignerSeriesLabelsBrush", null, this.options.propertyControlWidth), "seriesLabels"],
        ["seriesBorderThickness", this.loc.PropertyMain.SeriesBorderThickness, this.PropertyTextBox("styleDesignerPropertySeriesBorderThickness", this.options.propertyNumbersControlWidth), "series"],
        ["seriesCornerRadius", this.loc.PropertyMain.SeriesCornerRadius, this.PropertyCornerRadiusControl("styleDesignerPropertySeriesCornerRadius", this.options.propertyControlWidth + 61), "series"],
        ["seriesLighting", this.loc.PropertyMain.SeriesLighting, this.CheckBox("styleDesignerPropertySeriesLighting"), "series"],
        ["seriesShowShadow", this.loc.PropertyMain.SeriesShowShadow, this.CheckBox("styleDesignerPropertySeriesShowShadow"), "series"],
        ["seriesShowBorder", this.loc.PropertyMain.SeriesShowBorder, this.CheckBox("styleDesignerPropertySeriesShowBorder"), "series"],
        ["selectedCellBackColor", this.loc.PropertyMain.SelectedCellBackColor || "Selected Cell Back Color", this.PropertyColorControl("styleDesignerPropertySelectedCellBackColor", null, this.options.propertyControlWidth), "appearance"],
        ["selectedCellForeColor", this.loc.PropertyMain.SelectedCellForeColor || "Selected Cell Fore Color", this.PropertyColorControl("styleDesignerPropertySelectedCellForeColor", null, this.options.propertyControlWidth), "appearance"],
        ["selectedDataColor", this.loc.PropertyMain.SelectedDataColor || "Selected Data Color", this.PropertyColorControl("styleDesignerPropertySelectedDataColor", null, this.options.propertyControlWidth), "appearance"],
        ["selectedDataForeground", this.loc.PropertyMain.SelectedDataForeground || "Selected Data Foreground", this.PropertyColorControl("styleDesignerPropertySelectedDataForeground", null, this.options.propertyControlWidth), "appearance"],
        ["selectedBackColor", this.loc.PropertyMain.SelectedBackColor || "Selected Back Color", this.PropertyColorControl("styleDesignerPropertySelectedBackColor", null, this.options.propertyControlWidth), "appearance"],
        ["selectedForeColor", this.loc.PropertyMain.SelectedForeColor || "Selected Fore Color", this.PropertyColorControl("styleDesignerPropertySelectedForeColor", null, this.options.propertyControlWidth), "appearance"],
        ["selectedGlyphColor", this.loc.PropertyMain.SelectedGlyphColor || "Selected Glyph Color", this.PropertyColorControl("styleDesignerPropertySelectedSelectedGlyphColor", null, this.options.propertyControlWidth), "appearance"],
        ["separatorColor", this.loc.PropertyMain.SeparatorColor || "Separator Color", this.PropertyColorControl("styleDesignerPropertySeparatorColor", null, this.options.propertyControlWidth), "appearance"],
        ["seriesColors", this.loc.PropertyMain.SeriesColors || "Series Colors", this.PropertyColorsCollectionControl("styleDesignerPropertySeriesColors", null, this.options.propertyControlWidth), "appearance"],
        ["styleColors", this.loc.PropertyMain.StyleColors, this.PropertyColorsCollectionControl("styleDesignerPropertyStyleColors", null, this.options.propertyControlWidth), "appearance"],
        ["negativeTextBrush", this.loc.PropertyMain.NegativeTextBrush, this.PropertyBrushControl("styleDesignerPropertyNegativeTextBrush", null, this.options.propertyControlWidth), "appearance"],
        ["textBrush", this.loc.PropertyMain.TextBrush, this.PropertyBrushControl("styleDesignerPropertyTextBrush", null, this.options.propertyControlWidth), "appearance"],
        ["textFormat", this.loc.PropertyMain.TextFormat, this.PropertyTextBoxWithEditButton("styleDesignerPropertyTextFormat", this.options.propertyControlWidth, true), "appearance"],
        ["toolTipBorder", this.loc.PropertyMain.ToolTipBorder || "Tool Tip Border", this.PropertyBorderControl("styleDesignerPropertyToolTipBorder", this.options.propertyControlWidth), "toolTip"],
        ["toolTipBrush", this.loc.PropertyMain.ToolTipBrush || "Tool Tip Brush", this.PropertyBrushControl("styleDesignerPropertyToolTipBrush", null, this.options.propertyControlWidth), "toolTip"],
        ["toolTipCornerRadius", this.loc.PropertyMain.ToolTipCornerRadius || "Tool Tip Corner Radius", this.PropertyCornerRadiusControl("styleDesignerPropertyToolTipCornerRadius", this.options.propertyControlWidth + 61), "toolTip"],
        ["toolTipTextBrush", this.loc.PropertyMain.ToolTipTextBrush || "Tool Tip Text Brush", this.PropertyBrushControl("styleDesignerPropertyToolTipTextBrush", null, this.options.propertyControlWidth), "toolTip"],
        ["trendLineShowShadow", this.loc.PropertyMain.TrendLineShowShadow, this.CheckBox("styleDesignerTrendLineShowShadow"), "trendLine"],
        ["trendLineColor", this.loc.PropertyMain.TrendLineColor, this.PropertyColorControl("styleDesignerTrendLineColor", null, this.options.propertyControlWidth), "trendLine"],
        ["tickLabelMajorFont", this.loc.PropertyMain.TickLabelMajorFont || "Tick Label Major Font", this.PropertyFontControl("styleDesignerPropertyTickLabelMajorFont", null, true), "appearance"],
        ["tickLabelMajorTextBrush", this.loc.PropertyMain.TickLabelMajorTextBrush || "Tick Label Major Text Brush", this.PropertyBrushControl("styleDesignerPropertyTickLabelMajorTextBrush", null, this.options.propertyControlWidth), "appearance"],
        ["tickLabelMinorFont", this.loc.PropertyMain.TickLabelMinorFont || "Tick Label Minor Font", this.PropertyFontControl("styleDesignerPropertyTickLabelMinorFont", null, true), "appearance"],
        ["tickLabelMinorTextBrush", this.loc.PropertyMain.TickLabelMinorTextBrush || "Tick Label Minor Text Brush", this.PropertyBrushControl("styleDesignerPropertyTickLabelMinorTextBrush", null, this.options.propertyControlWidth), "appearance"],
        ["tickMarkMajorBorder", this.loc.PropertyMain.TickMarkMajorBorder || "Tick Mark Major Border", this.PropertyBrushControl("styleDesignerPropertyTickMarkMajorBorder", null, this.options.propertyControlWidth), "appearance"],
        ["tickMarkMajorBrush", this.loc.PropertyMain.TickMarkMajorBrush || "Tick Mark Major Brush", this.PropertyBrushControl("styleDesignerPropertyTickMarkMajorBrush", null, this.options.propertyControlWidth), "appearance"],
        ["tickMarkMinorBorder", this.loc.PropertyMain.TickMarkMinorBorder || "Tick Mark Minor Border", this.PropertyBrushControl("styleDesignerPropertyTickMarkMinorBorder", null, this.options.propertyControlWidth), "appearance"],
        ["tickMarkMinorBrush", this.loc.PropertyMain.TickMarkMinorBrush || "Tick Mark Minor Brush", this.PropertyBrushControl("styleDesignerPropertyTickMarkMinorBrush", null, this.options.propertyControlWidth), "appearance"],
        ["totalCellColumnBackColor", this.loc.PropertyMain.TotalCellColumnBackColor || "Total Cell Column Back Color", this.PropertyColorControl("styleDesignerPropertyTotalCellColumnBackColor", null, this.options.propertyControlWidth), "appearance"],
        ["totalCellColumnForeColor", this.loc.PropertyMain.TotalCellColumnForeColor || "Total Cell Column Fore Color", this.PropertyColorControl("styleDesignerPropertyTotalCellColumnForeColor", null, this.options.propertyControlWidth), "appearance"],
        ["totalCellRowBackColor", this.loc.PropertyMain.TotalCellRowBackColor || "Total Cell Row Back Color", this.PropertyColorControl("styleDesignerPropertyTotalCellRowBackColor", null, this.options.propertyControlWidth), "appearance"],
        ["totalCellRowForeColor", this.loc.PropertyMain.TotalCellRowForeColor || "Total Cell Row Fore Color", this.PropertyColorControl("styleDesignerPropertyTotalCellRowForeColor", null, this.options.propertyControlWidth), "appearance"],
        ["trackColor", this.loc.PropertyMain.TrackColor || "Track Color", this.PropertyColorControl("styleDesignerPropertyTrackColor", null, this.options.propertyControlWidth), "appearance"],
        ["vertAlignment", this.loc.PropertyMain.VertAlignment, this.PropertyDropDownList("styleDesignerPropertyVerticalAlignment", this.options.propertyControlWidth, this.GetVerticalAlignmentItems(), true, false), "appearance"]
    ]

    for (var i = 0; i < propAttributes.length; i++) {
        var control = propAttributes[i][2];
        var property = this.Property(null, propAttributes[i][1], control, null, propAttributes[i].length > 4 ? propAttributes[i][4] : 1);
        property.name = propAttributes[i][0];
        property.updateCaption();
        propertiesPanel.propertiesGroups[propAttributes[i][3]].container.appendChild(property);
        propertiesPanel.properties_[propAttributes[i][0]] = property;
        control.propertyName = propAttributes[i][0];
        control.groupName = propAttributes[i][3];

        control.action = function () {
            var needUpdateItems = false;

            var selectedItem = styleDesignerForm.stylesTree.selectedItem;
            if (!selectedItem) return;

            var styleProperties = selectedItem.itemObject.properties;
            if (!styleProperties && selectedItem.itemObject.typeItem == "Folder") styleProperties = selectedItem.itemObject;
            if (!styleProperties) return;

            if (this.propertyName == "collectionName" && this.value != styleProperties.collectionName) {
                needUpdateItems = true;
                if (styleProperties.typeItem == "Folder") {
                    for (var itemKey in selectedItem.childs) {
                        var itemObject = selectedItem.childs[itemKey].itemObject;
                        if (itemObject && itemObject.properties) {
                            itemObject.properties.collectionName = this.value;
                        }
                    }
                }
            }

            if (this.propertyName == "name") {
                if (this.value != styleProperties.name && (!this.value || styleDesignerForm.stylesTree.checkExistStyleName(this.value))) {
                    var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                    var text = !this.value ? jsObject.loc.Errors.FieldRequire.replace("{0}", jsObject.loc.PropertyMain.Name) : jsObject.loc.Errors.NameExists.replace("{0}", this.value);
                    errorMessageForm.show(text);
                    this.value = styleProperties[this.propertyName];
                    return;
                }
                else {
                    if (styleProperties.oldName == null) {
                        styleProperties.oldName = styleProperties.name;
                    }
                }
            }
                       
            if (this["setKey"] != null) styleProperties[this.propertyName] = this.key;
            else if (this["getValue"] != null) styleProperties[this.propertyName] = this.getValue();
            else if (this["setChecked"] != null) styleProperties[this.propertyName] = this.isChecked;
            else if (this["value"] != null) styleProperties[this.propertyName] = this.value;
            else if (this["textBox"] != null && this.textBox["value"] != null) styleProperties[this.propertyName] = this.textBox.value;
            selectedItem.repaint();
            if (needUpdateItems) {
                styleDesignerForm.stylesTree.updateItems(styleDesignerForm.stylesCollection, selectedItem.itemObject, true, true);
            }
        }

        if (propAttributes[i][0] == "conditions") {
            control.button.action = function () {
                var this_ = this;
                this.jsObject.InitializeStyleConditionsForm(function (styleConditionsForm) {
                    styleConditionsForm.show(styleDesignerForm, this_.textBox);
                });
            }
        }

        else if (propAttributes[i][0] == "textFormat") {
            control.button.action = function () {
                var textBox = this.textBox;
                this.jsObject.InitializeTextFormatForm(function (textFormatForm) {
                    var selectedItem = styleDesignerForm.stylesTree.selectedItem;
                    if (selectedItem) {
                        textFormatForm.show(selectedItem.itemObject.properties.textFormat, "textFormat", selectedItem.itemObject, textBox);
                    }
                });
            }
        }
    }

    propertiesPanel.propertiesGroups.appearance.container.appendChild(propertiesPanel.propertiesGroups.heatmap);
    propertiesPanel.propertiesGroups.appearance.container.appendChild(propertiesPanel.propertiesGroups.heatmapWithGroup);

    propertiesPanel.updatePropertiesCaptions = function () {
        for (var propertyName in this.properties_) {
            this.properties_[propertyName].updateCaption();
        }
    }
        
    propertiesPanel.updateControls = function (styleObject) {
        var propertyValues = styleObject ? (styleObject.typeItem == "Folder" ? { collectionName: styleObject.collectionName } : styleObject.properties) : null;
        if (!propertyValues) propertyValues = {};
        var propertyRows = this.properties_;
        var showingGroups = {};

        for (var i = 0; i < propertiesGroups.length; i++) {
            propertiesPanel.propertiesGroups[propertiesGroups[i][0]].style.display = "none";
        }

        for (var name in propertyRows) {
            propertyRows[name].style.display = propertyValues[name] != null ? "" : "none";
            if (propertyValues[name] != null) {
                var propertyValue = propertyValues[name];
                var propertyControl = propertyRows[name].propertyControl;
                if (!propertyControl) continue;                
                if (propertiesPanel.propertiesGroups[propertyControl.groupName])
                    propertiesPanel.propertiesGroups[propertyControl.groupName].style.display = "";

                if (name == "conditions") {
                    var conditionsText = "[" + ((propertyValue && propertyValue.length > 0) 
                        ? styleDesignerForm.jsObject.loc.PropertyMain.Conditions : styleDesignerForm.jsObject.loc.FormConditions.NoConditions) + "]";
                    propertyControl.textBox.value = conditionsText;
                }
                else if (name == "textFormat") {
                    propertyControl.textBox.value = styleDesignerForm.jsObject.GetTextFormatLocalizedName(propertyValue ? propertyValue.type : null);
                }                
                else if (propertyControl["setKey"] != null) propertyControl.setKey(propertyValue);
                else if (propertyControl["setValue"] != null) propertyControl.setValue(propertyValue);
                else if (propertyControl["setChecked"] != null) propertyControl.setChecked(propertyValue);
                else if (propertyControl["value"] != null) propertyControl.value = propertyValue;
                else if (propertyControl["textBox"] != null && propertyControl.textBox["value"] != null) propertyControl.textBox.value = propertyValue;
            }
        }
    }

    return propertiesPanel;
}

StiMobileDesigner.prototype.StylesTree = function (styleDesignerForm) {
    var tree = this.Tree(null, 500);
    tree.style.width = "100%";
    tree.style.minWidth = "500px";
    tree.style.overflowY = "auto";
    tree.style.overflowX = "hidden";
    tree.style.display = "inline-block";
    tree.openedItems = {};
    tree.styleDesignerForm = styleDesignerForm;
    var jsObject = this;

    tree.reset = function (notResetOpenedItems) {
        this.clear();
        if (!notResetOpenedItems) this.openedItems = {};
        this.mainItem = jsObject.StylesTreeItem("Main", null, { typeItem: "MainItem" }, tree);
        this.mainItem.style.width = "100%";
        this.mainItem.childsRow.style.display = "";
        this.mainItem.button.style.display = "none";
        this.mainItem.iconOpening.style.display = "none";
        this.appendChild(this.mainItem);
        styleDesignerForm.toolBar.updateControls();
        styleDesignerForm.propertiesPanel.updateControls();
        styleDesignerForm.createStylesHintItem.updateVisibleState();
    }

    tree.onSelectedItem = function (item) {
        styleDesignerForm.toolBar.updateControls();
        styleDesignerForm.propertiesPanel.updateControls(item.itemObject);
    }

    tree.onRemoveItem = function (item) {
        for (var i = 0; i < styleDesignerForm.stylesCollection.length; i++) {
            if (item.itemObject == styleDesignerForm.stylesCollection[i]) {
                styleDesignerForm.stylesCollection.splice(i, 1);
                break;
            }
        }
        if (item.parent && item.parent.itemObject.typeItem != "MainItem" && item.getCountElementsInCurrent() == 0) {
            item.parent.remove();
        }
        styleDesignerForm.createStylesHintItem.updateVisibleState();
    }

    tree.checkExistStyleName = function (styleName) {
        for (var i = 0; i < styleDesignerForm.stylesCollection.length; i++) {
            if (styleName == styleDesignerForm.stylesCollection[i].properties.name) {
                return true;
            }
        }
        return false;
    }

    tree.generateNewStyleName = function (baseNameForNewStyle) {
        var index = 1;
        var styleName = baseNameForNewStyle || jsObject.loc.FormStyleDesigner.Style;
        var findCompleted = false;

        while (!findCompleted) {
            findCompleted = true;
            if (tree.checkExistStyleName(styleName + index)) {
                findCompleted = false;
                index++;
            }     
        }

        return styleName + index;
    }

    tree.addItem = function (styleObject, baseNameForNewStyle, itemWasReplaced) {
        styleDesignerForm.findTextbox.setValue("");
        styleDesignerForm.findTextbox.textBox.onblur();

        if (!itemWasReplaced) {
            styleObject.properties.name = this.generateNewStyleName(baseNameForNewStyle);
        }
        styleDesignerForm.settings.filter[styleObject.type] = true;

        if (this.selectedItem) {
            var currItemObject = this.selectedItem.itemObject;
            var collectionName = currItemObject.collectionName || (currItemObject.properties ? currItemObject.properties.collectionName : "");
            if (collectionName != null) styleObject.properties.collectionName = collectionName;

            var i = 0;
            while (this.selectedItem.itemObject != styleDesignerForm.stylesCollection[i] && i < styleDesignerForm.stylesCollection.length) {
                i++;
            }
            styleDesignerForm.stylesCollection.splice(itemWasReplaced ? i : i + 1, 0, styleObject);
        }
        else {
            styleDesignerForm.stylesCollection.push(styleObject);
        }

        tree.updateItems(styleDesignerForm.stylesCollection, styleObject, true, !itemWasReplaced || (this.selectedItem && this.selectedItem.itemObject.typeItem == "Folder"));
    }

    tree.checkVisible = function (itemObject) {
        if (!itemObject) return false;
        if (itemObject.properties && styleDesignerForm.settings.filter[itemObject.type])
            return true;

        return false;
    }

    tree.compareObjects = function (object1, object2) {
        if (!object1 || !object2) return false;
        if (object1.properties && object2.properties) return object1 == object2;
        if (object1.typeItem == "Folder" && object2.typeItem == "Folder")
            return object1.collectionName == object2.collectionName;

        return false;
    }

    tree.findItems = function (text) {
        var choosedItems = [];
        for (var i = 0; i < styleDesignerForm.stylesCollection.length; i++) {
            var styleObject = styleDesignerForm.stylesCollection[i];
            var itemText = styleObject.properties.name;
            if (itemText) {
                var showItem = itemText.toLowerCase().indexOf(text.toLowerCase()) >= 0;
                if (showItem) choosedItems.push(styleObject);
            }
        }
        tree.updateItems(choosedItems, tree.selectedItem ? tree.selectedItem.itemObject : null);

        if (text != "") {
            for (var itemKey in this.mainItem.childs) {
                this.mainItem.childs[itemKey].setOpening(true);
            }
        }
    }

    tree.updateItems = function (stylesCollection, selectedStyleObject, notResetOpenedItems, scrollToSelectedItem) {
        this.currentScrollTop = this.scrollTop;
        this.reset(notResetOpenedItems);
        var collectionNames = [];
        var collectionStyles = {}
        var rootStyles = [];

        if (styleDesignerForm.settings.sort != "NoSorting") {
                stylesCollection.sort(function (a, b) {
                    if (a.properties.name && b.properties.name) {
                        var n = styleDesignerForm.settings.sort == "Ascending" ? -1 : 1;
                        if (a.properties.name < b.properties.name)
                            return n;
                        if (a.properties.name > b.properties.name)
                            return n * -1;
                    }
                    return 0
                });
        }
        
        for (var i = 0; i < stylesCollection.length; i++) {
            var styleObject = stylesCollection[i];
            var collectionName = styleObject.properties.collectionName;
            var styleItem = jsObject.StylesTreeItem(styleObject.properties.name, "Styles." + styleObject.type + "32.png", styleObject, tree);
            styleItem.marker.style.display = styleDesignerForm.usingStyles[styleObject.properties.name] ? "" : "none";

            if (collectionName) {
                if (!collectionStyles[collectionName]) {
                    collectionStyles[collectionName] = [];
                    collectionNames.push(collectionName);
                }
                collectionStyles[collectionName].push(styleItem);
            }
            else {
                rootStyles.push(styleItem);
            }
        }

        if (styleDesignerForm.settings.sort != "NoSorting") {
            collectionNames.sort(function (a, b) {
                if (a && b) {
                    var n = styleDesignerForm.settings.sort == "Ascending" ? -1 : 1;
                    if (a < b)
                        return n;
                    if (a > b)
                        return n * -1;
                }
                return 0
            });
        }

        for (var i = 0; i < collectionNames.length; i++) {
            var styleItems = collectionStyles[collectionNames[i]];
            if (styleItems) {
                var collectionItem = jsObject.StylesTreeItem(collectionNames[i], "CloudItems.BigFolder.png", { typeItem: "Folder", collectionName: collectionNames[i] }, tree);
                this.mainItem.addChild(collectionItem);
                if (this.compareObjects(selectedStyleObject, collectionItem.itemObject)) {
                    collectionItem.setSelected();
                }
                if (tree.openedItems[collectionNames[i]]) {
                    collectionItem.setOpening(true);
                }
                for (var k = 0; k < styleItems.length; k++) {
                    if (this.checkVisible(styleItems[k].itemObject)) {
                        var item = collectionItem.addChild(styleItems[k]);
                        item.captionContainer.style.maxWidth = "300px";

                        if (this.compareObjects(selectedStyleObject, styleItems[k].itemObject)) {
                            item.setSelected();
                            item.openTree();
                            tree.openedItems[collectionItem.itemObject.collectionName] = true;
                        }
                    }
                }
                if (jsObject.GetCountObjects(collectionItem.childs) == 0) {
                    collectionItem.parent.childsContainer.removeChild(collectionItem);
                    delete collectionItem.parent.childs[collectionItem.id];
                    delete tree.items[collectionItem.id];
                }
            }
        }

        for (var i = 0; i < rootStyles.length; i++) {
            if (this.checkVisible(rootStyles[i].itemObject)) {
                var item = this.mainItem.addChild(rootStyles[i]);
                if (this.compareObjects(selectedStyleObject, rootStyles[i].itemObject)) {
                    item.setSelected();
                    item.openTree();
                }
            }
        }

        this.scrollToPos(scrollToSelectedItem);

        styleDesignerForm.createStylesHintItem.updateVisibleState();
    }

    tree.scrollToPos = function (toSelectedItem) {
        if (toSelectedItem && this.selectedItem && this.selectedItem.itemObject.typeItem != "MainItem") {
            var yPos = this.jsObject.FindPosY(this.selectedItem, null, false, this);
            this.scrollTop = yPos - this.offsetHeight + 50;
        }
        else {
            this.scrollTop = this.currentScrollTop;
        }
    }

    tree.onmouseup = function (event) {
        if (event.button == 2) {
            event.stopPropagation();
            if (!this.jsObject.options.report) return;
            var stylesContextMenu = this.jsObject.options.menus.stylesContextMenu || this.jsObject.InitializeStylesContextMenu(styleDesignerForm);
            var point = this.jsObject.FindMousePosOnMainPanel(event);
            stylesContextMenu.show(point.xPixels + 3, point.yPixels + 3, "Down", "Right");
        }
        return false;
    }

    tree.oncontextmenu = function (event) {
        return false;
    }

    return tree;
}

StiMobileDesigner.prototype.StylesTreeItem = function (caption, imageName, itemObject, tree) {
    var item = this.TreeItem(caption, imageName, itemObject, tree, null, null, { width: 32, height: 32 });
    item.style.margin = "2px 0 2px 5px";
    item.style.width = "calc(100% - 40px)";
    item.button.style.width = "100%";

    //add marker
    var marker = document.createElement("div");
    marker.style.display = "none";
    marker.className = "stiUsingMarker";
    marker.style.right = "15px";
    marker.style.left = "auto";
    var markerInner = document.createElement("div");
    marker.appendChild(markerInner);
    item.button.style.position = "relative";
    item.button.appendChild(marker);
    item.marker = marker;

    //Override Item
    var innerButton = item.button;
    innerButton.style.overflow = "hidden";

    if (innerButton.imageCell) {
        innerButton.imageCell.parentNode.removeChild(innerButton.imageCell);
    }
    if (innerButton.captionCell) {
        innerButton.captionCell.parentNode.removeChild(innerButton.captionCell);
    }

    var innerTable = this.CreateHTMLTable();
    innerTable.style.width = "calc(100% - 8px)";
    innerTable.style.margin = "3px";
    innerButton.addCell(innerTable);

    if (innerButton.image) {
        var imageCell = innerTable.addCell(innerButton.image);
        innerTable.imageCell = imageCell;
        imageCell.style.width = "1px";
        imageCell.style.padding = itemObject.typeItem == "Folder" ? "0 3px 0 0px" : "0 3px 0 3px";
    }

    var captionContainer = item.captionContainer = document.createElement("div");
    var captionCell = innerTable.addCell(captionContainer);
    captionCell.style.padding = "0px 5px 0px 0px";
    captionContainer.style.maxHeight = "38px";
    captionContainer.style.maxWidth = "350px";
    captionContainer.style.overflow = "hidden";

    var captInnerCont = document.createElement("div");
    captionContainer.appendChild(captInnerCont);
    captInnerCont.innerHTML = caption;
    captInnerCont.style.position = "relative";
    captInnerCont.style.whiteSpace = "nowrap";
    captInnerCont.style.textOverflow = "ellipsis";
    captInnerCont.style.overflow = "hidden";
    captInnerCont.style.padding = "1px 0 1px 0";
    
    item.repaint = function () {
        var properties = this.itemObject.properties;
        if (!properties) return;
        var defaultFont = "Arial!10!0!0!0!0";
        var defaultBrush = "1!transparent";
        var defaultTextBrush = "1!0,0,0";
        var defaultBorder = "default";

        captInnerCont.innerHTML = properties["description"] || properties["name"];

        var font = (properties["font"] != null && properties["allowUseFont"]) ? properties["font"] : defaultFont;
        var brush = (properties["brush"] != null && properties["allowUseBrush"]) ? properties["brush"] : defaultBrush;
        var textBrush = (properties["textBrush"] != null && properties["allowUseTextBrush"]) ? properties["textBrush"] : defaultTextBrush;
        var border = (properties["border"] != null && properties["allowUseBorderSides"]) ? properties["border"] : defaultBorder;

        if (border) {            
            var borderArray = border.split("!");
            var borderDropShadow = border != "default" && borderArray.length > 6 && borderArray[4] == "1";
            if (borderDropShadow) {
                var borderShadowSize = borderArray[5];
                var borderShadowColor = this.jsObject.GetColorFromBrushStr(StiBase64.decode(borderArray[6]));                
                var cssColor = "transparent";
                var colors = borderShadowColor.split(",");
                if (colors.length == 3) cssColor = "rgba(" + borderShadowColor + ",1)";
                if (colors.length == 4) cssColor = "rgba(" + colors[1] + "," + colors[2] + "," + colors[3] + "," + this.jsObject.StrToInt(colors[0]) / 255 + ")";
                innerButton.style.boxShadow = cssColor + " " + borderShadowSize + "px " + borderShadowSize + "px 0px";
            }
            else {
                innerButton.style.boxShadow = "none";
            }
        }

        this.jsObject.RepaintControlByAttributes(innerTable, font, brush, textBrush, border);
        captInnerCont.style.top = (captInnerCont.offsetHeight > 40 ? ((captInnerCont.offsetHeight - 40) / 2 * -1) : 0) + "px";

        var props = this.itemObject.properties;

        if (this.itemObject.type == "StiCrossTabStyle") {
            var sampleCrossTab = this.jsObject.CrossTabSampleTable(32, 32, 5, 5, props.columnHeaderBackColor, props.rowHeaderBackColor);
            if (innerTable.imageCell) {
                innerTable.imageCell.innerHTML = "";
                innerTable.imageCell.style.padding = "2px 6px 2px 3px";
                innerTable.imageCell.appendChild(sampleCrossTab);
            }
        }
        else if (this.itemObject.type == "StiTableStyle") {
            var sampleTable = this.jsObject.SampleTable(32, 32, 5, 5, props.headerColor, props.footerColor, props.dataColor, props.gridColor);
            if (innerTable.imageCell) {
                innerTable.imageCell.innerHTML = "";
                innerTable.imageCell.style.padding = "2px 6px 2px 3px";
                innerTable.imageCell.appendChild(sampleTable);
            }
        }
    }

    item.changeFormModalState = function (state) {
        var form = tree.styleDesignerForm;
        if (form) {
            jsObject.options.disabledPanels[form.level].changeVisibleState(state);
        }
    }

    //Override
    item.button.onmouseup = function () {
        var itemInDrag = this.jsObject.options.itemInDrag;
        if (itemInDrag && itemInDrag.originalItem &&
            itemInDrag.originalItem != this.treeItem && itemInDrag.originalItem.itemObject.properties)
        {
            itemInDrag.originalItem.remove();
            this.treeItem.setSelected();
            tree.addItem(itemInDrag.originalItem.itemObject, null, true);
        }
    }
        
    item.button.onmousedown = function () {
        if (this.isTouchStartFlag) return;
        if (this.jsObject.options.controlsIsFocused && this.jsObject.options.controlsIsFocused.action) { //fixed bug with controls action onblur
            this.jsObject.options.controlsIsFocused.action();
        }
        if (tree.items[item.id]) {
            item.button.action();
        }
        if (event) event.preventDefault();
        
        if (item.itemObject.properties && event.button != 2 && !this.jsObject.options.controlsIsFocused) {
            var itemInDragObject = item.jsObject.TreeItemForDragDrop({
                name: item.itemObject.properties.name,
                typeItem: "Style",
                typeIcon: "Styles." + item.itemObject.type + "32"
            }, item.tree);

            if (itemInDragObject.button.captionCell) itemInDragObject.button.captionCell.style.padding = "3px 15px 3px 5px";
            if (itemInDragObject.button.imageCell) itemInDragObject.button.imageCell.style.padding = "2px 5px 2px 5px";
            itemInDragObject.originalItem = item;
            itemInDragObject.beginingOffset = 0;
            item.jsObject.options.itemInDrag = itemInDragObject;
        }
    }

    item.button.ontouchstart = function () {
        var this_ = this;
        this.isTouchStartFlag = true;
        clearTimeout(this.isTouchStartTimer);
        if (this.jsObject.options.controlsIsFocused && this.jsObject.options.controlsIsFocused.action) { //fixed bug with controls action onblur
            this.jsObject.options.controlsIsFocused.action();
        }
        if (tree.items[item.id]) {
            item.button.action();
        }
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    item.iconOpening.action = function () {
        if (tree.isDisable) return;
        item.isOpening = !item.isOpening;
        item.childsRow.style.display = item.isOpening ? "" : "none";
        var imgName = item.isOpening ? "IconCloseItem.png" : "IconOpenItem.png";
        if (this.jsObject.options.isTouchDevice) imgName = imgName.replace(".png", "Big.png");
        StiMobileDesigner.setImageSource(item.iconOpening, this.jsObject.options, imgName);
        item.setSelected();
        if (item.itemObject.typeItem == "Folder") {
            tree.openedItems[item.itemObject.collectionName] = item.isOpening;
            StiMobileDesigner.setImageSource(item.button.image, this.jsObject.options, "CloudItems." + (item.isOpening ? "BigFolderOpen.png" : "BigFolder.png"));
        }
    }

    item.openTree = function () {
        var item = this.parent;
        while (item != null) {
            item.isOpening = true;
            item.childsRow.style.display = "";
            StiMobileDesigner.setImageSource(item.iconOpening, this.jsObject.options, this.jsObject.options.isTouchDevice ? "IconCloseItemBig.png" : "IconCloseItem.png");
            if (item.itemObject.typeItem == "Folder") {
                StiMobileDesigner.setImageSource(item.button.image, this.jsObject.options, "CloudItems." + (item.isOpening ? "BigFolderOpen.png" : "BigFolder.png"));
                tree.openedItems[item.itemObject.collectionName] = item.isOpening;
            }
            item = item.parent;
        }        
    }

    item.setOpening = function (state) {
        if (this.buildChildsNotCompleted && state) this.completeBuildTree(true);
        this.isOpening = state;
        this.childsRow.style.display = state ? "" : "none";
        var imageType = state ? "Close" : "Open";
        StiMobileDesigner.setImageSource(this.iconOpening, this.jsObject.options, this.jsObject.options.isTouchDevice ? "Icon" + imageType + "ItemBig.png" : "Icon" + imageType + "Item.png");
        if (item.itemObject.typeItem == "Folder") {
            StiMobileDesigner.setImageSource(item.button.image, this.jsObject.options, "CloudItems." + (item.isOpening ? "BigFolderOpen.png" : "BigFolder.png"));
            tree.openedItems[item.itemObject.collectionName] = item.isOpening;
        }
    }    

    item.repaint();

    return item;
}

StiMobileDesigner.prototype.GetAllUsingStyles = function () {
    var styles = {};
    var report = this.options.report;
    if (report) {
        for (var pageName in report.pages) {
            var page = report.pages[pageName];
            if (page.properties.componentStyle && page.properties.componentStyle != "[None]") {
                styles[page.properties.componentStyle] = true;
            }
            for (var cName in page.components) {
                var component = page.components[cName];
                if (component.properties.componentStyle && component.properties.componentStyle != "[None]") {
                    styles[component.properties.componentStyle] = true;
                }
                if (component.properties.customStyleName) {
                    styles[component.properties.customStyleName] = true;
                }
                if (component.properties.evenStyle && component.properties.evenStyle != "[None]") {
                    styles[component.properties.evenStyle] = true;
                }
                if (component.properties.oddStyle && component.properties.oddStyle != "[None]") {
                    styles[component.properties.oddStyle] = true;
                }
                if (component.properties.chartStyle && component.properties.chartStyle.type == "StiCustomStyle") {
                    styles[component.properties.chartStyle.name] = true;
                }
                if (component.properties.gaugeStyle && component.properties.gaugeStyle.type == "StiCustomGaugeStyle") {
                    styles[component.properties.gaugeStyle.name] = true;
                }
                if (component.properties.mapStyle && component.properties.mapStyle.type == "StiMapStyle") {
                    styles[component.properties.mapStyle.name] = true;
                }
                if (component.properties.conditions) {
                    var conditions = JSON.parse(StiBase64.decode(component.properties.conditions));
                    for (var i = 0; i < conditions.length; i++) {
                        if (conditions[i].Style) styles[conditions[i].Style] = true;
                    }
                }
            }
        }
    }

    return styles;
}

StiMobileDesigner.prototype.CreateStylesHintItem = function () {
    var hintItem = this.CreateHTMLTable();
    var widthHintItem = 250;
    hintItem.className = "stiCreateDataHintItem";
    hintItem.style.top = "calc(50% - 60px)";
    hintItem.style.left = "calc(50% - " + widthHintItem / 2 + "px)";
    var img = document.createElement("img");
    img.style.width = img.style.height = "60px";
    StiMobileDesigner.setImageSource(img, this.options, "Styles.ItemCreateStyle.png");
    hintItem.addCell(img).style.textAlign = "center";
    hintItem.addTextCellInNextRow(this.loc.FormDictionaryDesigner.ClickHere).className = "stiCreateDataHintHeaderText";

    var text = document.createElement("div");
    text.className = "stiCreateDataHintText";
    text.innerHTML = this.loc.FormStyleDesigner.CreateNewComponentStyle;
    text.style.width = widthHintItem + "px";
    hintItem.addCellInNextRow(text).style.textAlign = "center";

    return hintItem;
}

StiMobileDesigner.prototype.ChooseBaseStyleForm = function () {
    var jsObject = this;
    var form = this.BaseForm("chooseBaseStyleForm", this.loc.FormStyleDesigner.CreateStyleOnBase, 2, this.HelpLinks["styleDesigner"]);
    form.stylesContents = {};

    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.margin = "12px";
    controlsTable.style.display = "inline-block";

    form.container.appendChild(controlsTable);
    form.container.style.minWidth = "350px";
    form.container.style.textAlign = "center";

    var stylesControl = this.StylesControl("baseStyleControl");
    stylesControl.className = "stiSimpleContainerWithBorder";
    stylesControl.innerButton.toolTip = null;
    stylesControl.menu.onshow = function () { }

    stylesControl.setKey = function (key) {
        this.key = key;
        this.addItemsToMenu(this.stylesContent);
        this.menu.innerContent.style.maxHeight = "380px";

        if (this.menu.parentElement) {
            jsObject.options.mainPanel.removeChild(this.menu); //fix a showing bug with style content
        }
    };

    stylesControl.action = function () {
        form.defaultStyleKeys[form.styleType] = this.key;
    }

    var textCell = controlsTable.addTextCell(this.loc.PropertyMain.Default);
    textCell.className = "stiDesignerCaptionControls";
    textCell.style.padding = "0 25px 0 0";

    controlsTable.addCell(stylesControl).className = "stiDesignerControlCellsBigIntervals2";
        
    form.loadDefaultStyleKeysFromCookies = function () {
        params.designerOptions = jsObject.GetCookie("StimulsoftDefaultStyleKeys");
    }

    form.saveDefaultStyleKeysToCookies = function () {
        jsObject.SetCookie("StimulsoftDefaultStyleKeys", JSON.stringify(form.defaultStyleKeys));
    }

    form.getDefaultStyleKeys = function () {
        var keys = jsObject.GetCookie("StimulsoftDefaultStyleKeys");

        if (keys) {
            return JSON.parse(keys);
        }
        else {
            return {
                "StiChartStyle": { type: "StiStyle29", name: "" },
                "StiGaugeStyle": { type: "StiGaugeStyleXF26", name: "Style26" },
                "StiMapStyle": { type: "StiMapStyleIdent", name: "Style25" },
                "StiCrossTabStyle": { crossTabStyle: null, crossTabStyleIndex: 0 },
                "StiTableStyle": { styleName: null, styleId: "Style21" },
                "StiCardsStyle": { ident: "Blue" },
                "StiIndicatorStyle": { ident: "Blue" },
                "StiProgressStyle": { ident: "Blue" }
            }
        }
    }

    form.ready = function () {
        this.changeVisibleState(true);
        stylesControl.setKey(this.defaultStyleKeys[this.styleType]);
    }

    form.getTypeComponent = function (styleType) {
        switch (styleType) {
            case "StiChartStyle": return "StiChart";
            case "StiGaugeStyle": return "StiGauge";
            case "StiMapStyle": return "StiMap";
            case "StiCrossTabStyle": return "StiCrossTab";
            case "StiTableStyle": return "StiTable";
            case "StiCardsStyle": return "StiCardsElement";
            case "StiIndicatorStyle": return "StiIndicatorElement";
            case "StiProgressStyle": return "StiProgressElement";
        }
        return null;
    }

    form.show = function (styleType) {
        this.styleType = styleType;
        this.defaultStyleKeys = this.getDefaultStyleKeys();

        stylesControl.typeComponent = form.getTypeComponent(styleType);
        stylesControl.stylesContent = form.stylesContents[styleType];

        if (stylesControl.stylesContent) {
            form.ready();
        }
        else {
            jsObject.SendCommandToDesignerServer("GetStylesContentByType", { styleType: styleType }, function (answer) {
                if (answer.stylesContent) {
                    form.stylesContents[styleType] = stylesControl.stylesContent = answer.stylesContent;                    
                    form.ready();
                }
            });
        }
    }

    form.action = function () {
        this.changeVisibleState(false);
        this.saveDefaultStyleKeysToCookies();

        jsObject.SendCommandToDesignerServer("CreateStyleBasedAnotherStyle", { styleType: this.styleType, styleObject: stylesControl.key }, function (answer) {
            var styleDesignerForm = jsObject.options.forms.styleDesignerForm;
            if (answer.styleObject && styleDesignerForm) {

                jsObject.options.reportIsModified = true;
                styleDesignerForm.stylesTree.addItem(answer.styleObject);
            }
        });
    }

    return form;
}