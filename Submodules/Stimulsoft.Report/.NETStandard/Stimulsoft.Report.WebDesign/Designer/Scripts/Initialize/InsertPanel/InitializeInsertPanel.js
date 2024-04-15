
StiMobileDesigner.prototype.InitializeInsertPanel = function () {
    var jsObject = this;
    var insertPanel = this.ChildWorkPanel("insertPanel", "stiDesignerInsertPanel");
    insertPanel.style.display = "none";
    insertPanel.selectedComponent = null;
    insertPanel.buttons = {};

    insertPanel.resetChoose = function () {
        jsObject.options.drawComponent = false;
        jsObject.options.paintPanel.changeCursorType(false);

        if (this.selectedComponent) {
            this.selectedComponent.setSelected(false);
            this.selectedComponent = null;
        }

        var buttons = jsObject.options.buttons;
        if (buttons.insertBands) buttons.insertBands.setSelected(false);
        if (buttons.insertCrossBands) buttons.insertCrossBands.setSelected(false);
        if (buttons.insertComponents) buttons.insertComponents.setSelected(false);
        if (buttons.insertBarCodes) buttons.insertBarCodes.setSelected(false);
        if (buttons.insertShapes) buttons.insertShapes.setSelected(false);
        if (buttons.insertSignatures) buttons.insertSignatures.setSelected(false);
        if (buttons.insertCharts) buttons.insertCharts.setSelected(false);
        if (buttons.insertMaps) buttons.insertMaps.setSelected(false);
        if (buttons.insertGauges) buttons.insertGauges.setSelected(false);
        if (buttons.insertMapsElements) buttons.insertMapsElements.setSelected(false);
        if (buttons.insertFiltersElements) buttons.insertFiltersElements.setSelected(false);
    }

    insertPanel.setChoose = function (selectedElement) {
        jsObject.options.drawComponent = true;
        jsObject.options.paintPanel.setCopyStyleMode(false);
        jsObject.options.paintPanel.changeCursorType(true);
        this.selectedComponent = selectedElement;

        if (selectedElement.menu && selectedElement.menu.parentButton) {
            selectedElement.menu.parentButton.setSelected(true);
        }
        else {
            selectedElement.setSelected(true);
        }
    }

    var innerTable = this.CreateHTMLTable();
    insertPanel.appendChild(innerTable);

    var groupBlockInsertPages = this.GroupBlock("groupBlockInsertPages", this.loc.FormDictionaryDesigner.NewItem, false, null);
    innerTable.addCell(groupBlockInsertPages);
    var pagesSeparator = this.GroupBlockSeparator();
    innerTable.addCell(pagesSeparator);

    if (!this.options.componentsIntoInsertTab) {
        innerTable.addCell(this.GroupBlock("groupBlockGroupsComponents", this.loc.PropertyMain.Categories, false, null));
        this.options.controls.groupsComponentsSep = this.GroupBlockSeparator();
        innerTable.addCell(this.options.controls.groupsComponentsSep);
    }

    innerTable.addCell(this.GroupBlock("groupBlockMainComponents", this.loc.Report.Components, false, null));
    innerTable.addCell(this.GroupBlockSeparator());

    //Pages
    var pagesTable = this.GroupBlockInnerTable();
    pagesTable.style.width = "100%";
    this.options.controls.groupBlockInsertPages.container.appendChild(pagesTable);

    var pageButton = this.BigButton("insertPanelAddPage", null, this.loc.A_WebViewer.Page, "BlankPage.png",
        [this.loc.HelpDesigner.PageNew, this.HelpLinks["insertcomponent"]], false, "stiDesignerStandartBigButton", false, 70);
    pageButton.style.display = this.options.showNewPageButton === false ? "none" : "inline-block";
    pagesTable.addCell(pageButton).style.textAlign = "center";
    pagesSeparator.style.display = this.options.showNewPageButton === false ? "none" : "";
    groupBlockInsertPages.style.display = this.options.showNewPageButton === false ? "none" : "";

    if (this.options.dashboardAssemblyLoaded) {
        var dashboardButton = this.BigButton("insertPanelAddDashboard", null, this.loc.Components.StiDashboard, "StiDashboard.png",
            [this.loc.Wizards.groupCreateNewDashboard, this.HelpLinks["insertcomponent"]], false, "stiDesignerStandartBigButton", false, 70);
        dashboardButton.style.display = this.options.showNewDashboardButton === false ? "none" : "inline-block";
        pagesTable.addCell(dashboardButton).style.textAlign = "center";
        pagesSeparator.style.display = this.options.showNewPageButton === false && this.options.showNewDashboardButton === false ? "none" : "";
        groupBlockInsertPages.style.display = this.options.showNewPageButton === false && this.options.showNewDashboardButton === false ? "none" : "";
    }

    //Groups
    if (!this.options.componentsIntoInsertTab) {
        var groupsTable = this.GroupBlockInnerTable();
        this.options.controls.groupBlockGroupsComponents.container.appendChild(groupsTable);

        groupsTable.addCell(this.BigButton("insertBands", null, this.loc.Report.Bands, "Bands.png", [this.loc.Report.Bands, this.HelpLinks["insertcomponent"]], true));
        this.BandsMenu("bandsMenu", this.options.buttons.insertBands);

        groupsTable.addCell(this.BigButton("insertCrossBands", null, this.loc.Report.CrossBands, "CrossBands.png", [this.loc.Report.CrossBands, this.HelpLinks["insertcomponent"]], true));
        this.CrossBandsMenu("crossBandsMenu", this.options.buttons.insertCrossBands);

        groupsTable.addCell(this.BigButton("insertComponents", null, this.loc.Report.Components, "Components.png", [this.loc.Report.Components, this.HelpLinks["insertcomponent"]], true));
        this.ComponentsMenu("componentsMenu", this.options.buttons.insertComponents);

        groupsTable.addCell(this.BigButton("insertSignatures", null, this.loc.Components.StiSignature, "Signatures.png", [this.loc.Components.StiSignature, this.HelpLinks["insertcomponent"]], true));
        this.SignaturesMenu("signaturesMenu", this.options.buttons.insertSignatures);

        groupsTable.addCell(this.BigButton("insertBarCodes", null, this.loc.Components.StiBarCode, "StiBarCode.png", [this.loc.Components.StiBarCode, this.HelpLinks["insertcomponent"]], true));
        this.BarCodesMenu("barCodesMenu", this.options.buttons.insertBarCodes);

        groupsTable.addCell(this.BigButton("insertShapes", null, this.loc.Report.Shapes, "StiShape.png", [this.loc.Report.Shapes, this.HelpLinks["insertcomponent"]], true));
        this.ShapesMenu("shapesMenu", this.options.buttons.insertShapes);

        groupsTable.addCell(this.BigButton("insertCharts", null, this.loc.Components.StiChart, "StiChart.png", [this.loc.Components.StiChart, this.HelpLinks["insertcomponent"]], true));
        this.ChartsMenu("chartsMenu", this.options.buttons.insertCharts);

        groupsTable.addCell(this.BigButton("insertGauges", null, this.loc.Components.StiGauge, "StiGauge.png", [this.loc.Components.StiGauge, this.HelpLinks["insertcomponent"]], true, "stiDesignerStandartBigButton", null, 60));
        this.GaugesMenu("gaugesMenu", this.options.buttons.insertGauges);

        var insertMapsButton = this.BigButton("insertMaps", null, this.loc.Components.StiMap, "StiMap.png", [this.loc.Components.StiMap, this.HelpLinks["insertcomponent"]], false);
        insertMapsButton.name = "StiMap";
        groupsTable.addCell(insertMapsButton);

        this.AddDragEventsToComponentButton(insertMapsButton);

        insertMapsButton.action = function () {
            jsObject.InitializeMapCategoriesForm(function (form) {
                insertMapsButton.setSelected(true);
                form.show(insertPanel);
            });
        }
    }

    //Main
    insertPanel.update = function (components) {
        if (insertPanel.mainTable) {
            jsObject.options.controls.groupBlockMainComponents.container.removeChild(insertPanel.mainTable);
            insertPanel.buttons = {};
        }

        insertPanel.mainTable = jsObject.GroupBlockInnerTable();
        jsObject.options.controls.groupBlockMainComponents.container.appendChild(insertPanel.mainTable);

        var addComponentButtons = function (componentTypes) {
            var mapsMenuCreated = false;
            var filterMenuCreated = false;
            for (var i = 0; i < componentTypes.length; i++) {
                if (jsObject.options.visibilityComponents[componentTypes[i]] ||
                    jsObject.options.visibilityBands[componentTypes[i]] ||
                    jsObject.options.visibilityCrossBands[componentTypes[i]] ||
                    (jsObject.options.dashboardAssemblyLoaded && jsObject.options.visibilityDashboardElements[componentTypes[i]])) {
                    var isDashboardElement = jsObject.options.dashboardAssemblyLoaded && jsObject.options.visibilityDashboardElements[componentTypes[i]];
                    var text = isDashboardElement ? jsObject.loc.Components[componentTypes[i].replace("Element", "")] : jsObject.loc.Components[componentTypes[i]];
                    var image = (isDashboardElement ? "Dashboards.BigComponents." : "") + componentTypes[i] + ".png";
                    var tooltip = ["<b>" + text + "</b><br><br><table><tr><td style='vertical-align: top;'>" + (jsObject.loc.HelpComponents[componentTypes[i]] || "") + "</td></tr></table>", jsObject.HelpLinks["insertcomponent"]];
                    var button = jsObject.ComponentButton(componentTypes[i], text, image, "stiDesignerStandartBigButton", tooltip);

                    if (isDashboardElement) {
                        if (componentTypes[i] == "StiShapeElement") {
                            button = jsObject.BigButton("insertShapesElements", null, jsObject.loc.Report.Shapes, "StiShape.png", tooltip, true);

                            var shapesMenu = jsObject.ShapesMenu("shapesElementsMenu", button, true, true);

                            button.action = function () {
                                shapesMenu.changeVisibleState(!shapesMenu.visible);
                            }
                        }
                        else if (componentTypes[i] == "StiRegionMapElement" || componentTypes[i] == "StiOnlineMapElement") {
                            if (mapsMenuCreated) continue;

                            tooltip = ["<b>" + jsObject.loc.PropertyMain.Maps + "</b><br><br>" +
                                "<table><tr><td style='vertical-align: top;'>" + jsObject.loc.HelpComponents.StiMapCategory + "</td></tr></table>", jsObject.HelpLinks["insertcomponent"]];

                            button = jsObject.BigButton("insertMapsElements", null, jsObject.loc.Components.StiMap, "Styles.StiMapStyle32.png", tooltip, true);

                            var mapsMenu = jsObject.MapsElementsMenu("mapsElementsMenu", button, false, componentTypes);
                            mapsMenuCreated = true;

                            button.action = function () {
                                mapsMenu.changeVisibleState(!mapsMenu.visible);
                            }
                        }
                        else if (jsObject.IsFilterElement(componentTypes[i])) {
                            if (filterMenuCreated) continue;

                            tooltip = ["<b>" + jsObject.loc.PropertyMain.Filters + "</b><br><br>" +
                                "<table><tr><td style='vertical-align: top;'>" + jsObject.loc.HelpComponents.StiFilterCategory + "</td></tr></table>", jsObject.HelpLinks["insertcomponent"]];

                            button = jsObject.BigButton("insertFiltersElements", null, jsObject.loc.PropertyMain.Filters, "Dashboards.BigComponents.StiFilterElement.png", tooltip, true);

                            var filtersMenu = jsObject.FiltersElementsMenu("filtersElementsMenu", button, false, componentTypes);
                            filterMenuCreated = true;

                            button.action = function () {
                                filtersMenu.changeVisibleState(!filtersMenu.visible);
                            }
                        }
                    }

                    button.isDashboardElement = isDashboardElement;
                    button.caption.style.maxWidth = componentTypes[i] == "StiImage" || componentTypes[i] == "StiImageElement" ||
                        componentTypes[i] == "StiGaugeElement" || componentTypes[i] == "StiChildBand" ? "85px" : "60px";
                    button.allwaysEnabled = false;
                    insertPanel.mainTable.addCell(button);
                    insertPanel.buttons[componentTypes[i]] = button;
                    button.setEnabled(jsObject.options.report != null);
                }
            }
        }
        var componentTypes = jsObject.options.componentsIntoInsertTab || components || jsObject.GetComponentsIntoInsertTab();
        var addSetupButtonSeparator = false;

        if (jsObject.Is_array(componentTypes)) {
            addComponentButtons(componentTypes);
        }
        else {
            var addSep = false;
            for (var groupName in componentTypes) {
                if (componentTypes[groupName].items.length > 0) {
                    var componentsInGroup = componentTypes[groupName].items;
                    var visibleComponents = [];
                    for (var i = 0; i < componentsInGroup.length; i++) {
                        if (jsObject.options.visibilityComponents[componentsInGroup[i]] ||
                            jsObject.options.visibilityBands[componentsInGroup[i]] ||
                            jsObject.options.visibilityCrossBands[componentsInGroup[i]] ||
                            (jsObject.options.dashboardAssemblyLoaded && jsObject.options.visibilityDashboardElements[componentsInGroup[i]])) {
                            visibleComponents.push(componentsInGroup[i]);
                        }
                    }
                    if (visibleComponents.length > 0) {
                        if (addSep && groupName != "dashboards") {
                            insertPanel.buttons["separator" + groupName] = jsObject.InsertPanelSeparator();
                            insertPanel.mainTable.addCell(insertPanel.buttons["separator" + groupName]);
                        }
                        addSep = true;
                        addSetupButtonSeparator = true;
                    }
                }
                addComponentButtons(componentTypes[groupName].items);
            }
        }

        if (jsObject.options.showSetupToolboxButton) {
            if (addSetupButtonSeparator) {
                insertPanel.setupButtonSeparator = insertPanel.mainTable.addCell(jsObject.InsertPanelSeparator());
            }
            var setupToolboxButton = jsObject.BigButton("insertPanelSetupToolbox", null, jsObject.loc.FormDesigner.SetupToolbox, "SetupToolbox.png",
                null, null, "stiDesignerStandartBigButton", false, 70);
            insertPanel.mainTable.addCell(setupToolboxButton);

            setupToolboxButton.action = function () {
                this.jsObject.InitializeSetupToolboxForm(function (form) {
                    form.changeVisibleState(true);
                });
            }
        }

        var buttonNames = ["insertBands", "insertCrossBands", "insertComponents", "insertSignatures", "insertBarCodes", "insertShapes", "insertCharts", "insertMaps", "insertGauges",
            "insertPanelAddPage", "insertPanelAddDashboard"];

        for (var i = 0; i < buttonNames.length; i++) {
            var button = jsObject.options.buttons[buttonNames[i]];
            if (button) {
                if (!jsObject.options.componentsIntoInsertTab && componentTypes) {
                    var buttonCell = button.parentElement;
                    if (buttonNames[i] == "insertBands")
                        buttonCell.style.display = componentTypes["bands"] && componentTypes["bands"].categoryVisible && jsObject.IsVisibilityBands() ? "" : "none";
                    else if (buttonNames[i] == "insertCrossBands")
                        buttonCell.style.display = componentTypes["crossBands"] && componentTypes["crossBands"].categoryVisible && jsObject.IsVisibilityCrossBands() ? "" : "none";
                    else if (buttonNames[i] == "insertComponents")
                        buttonCell.style.display = componentTypes["components"] && componentTypes["components"].categoryVisible && jsObject.IsVisibilityComponents() ? "" : "none";
                    else if (buttonNames[i] == "insertSignatures")
                        buttonCell.style.display = componentTypes["signatures"] && componentTypes["signatures"].categoryVisible && jsObject.IsVisibilitySignatures() ? "" : "none";
                    else if (buttonNames[i] == "insertBarCodes")
                        buttonCell.style.display = componentTypes["barcodes"] && componentTypes["barcodes"].categoryVisible && jsObject.options.visibilityComponents.StiBarCode ? "" : "none";
                    else if (buttonNames[i] == "insertShapes")
                        buttonCell.style.display = componentTypes["shapes"] && componentTypes["shapes"].categoryVisible && jsObject.IsVisibilityShapes() ? "" : "none";
                    else if (buttonNames[i] == "insertCharts")
                        buttonCell.style.display = componentTypes["charts"] && componentTypes["charts"].categoryVisible && jsObject.options.visibilityComponents.StiChart ? "" : "none";
                    else if (buttonNames[i] == "insertMaps")
                        buttonCell.style.display = componentTypes["maps"] && componentTypes["maps"].categoryVisible && jsObject.options.visibilityComponents.StiMap && jsObject.options.designerSpecification != "Beginner" ? "" : "none";
                    else if (buttonNames[i] == "insertGauges")
                        buttonCell.style.display = componentTypes["gauges"] && componentTypes["gauges"].categoryVisible && jsObject.options.visibilityComponents.StiGauge && jsObject.options.designerSpecification != "Beginner" ? "" : "none";
                }
                if (button.style.display != "none") {
                    button.setEnabled(jsObject.options.report != null);
                }
            }
        }

        insertPanel.setMode(true);
    }

    insertPanel.setMode = function (manually) {
        var mode = jsObject.options.report && jsObject.options.currentPage && jsObject.options.currentPage.isDashboard ? "Dashboard" : "Page";
        if (this.mode == mode && !manually) return;
        this.mode = mode;

        if (!jsObject.options.componentsIntoInsertTab) {
            jsObject.options.controls.groupBlockGroupsComponents.style.display =
                jsObject.options.controls.groupsComponentsSep.style.display = mode == "Page" ? "" : "none";
        }

        if (insertPanel.setupButtonSeparator)
            insertPanel.setupButtonSeparator.style.display = "none";

        for (var name in this.buttons) {
            var button = this.buttons[name];
            button.parentNode.style.display = (mode == "Dashboard" && button.isDashboardElement && name.indexOf("separator") < 0) || (mode == "Page" && !button.isDashboardElement) ? "" : "none";

            if (button.parentNode.style.display == "" && insertPanel.setupButtonSeparator) {
                insertPanel.setupButtonSeparator.style.display = "";
            }
        }
    }

    insertPanel.update();
    insertPanel.setMode();
}

StiMobileDesigner.prototype.InsertPanelSeparator = function () {
    var sep = this.HomePanelSeparator();
    sep.style.height = this.options.isTouchDevice ? "90px" : "70px";

    return sep;
}

StiMobileDesigner.prototype.GetComponentsIntoInsertTab = function () {
    var componentsStr = this.GetCookie("StimulsoftMobileDesignerComponentsIntoInsertTab_NewVers2");
    if (componentsStr) {
        var components = JSON.parse(componentsStr);

        if (this.options.designerSpecification == "Beginner") {
            var excludeBands = ["StiColumnHeaderBand", "StiColumnFooterBand", "StiHierarchicalBand", "StiChildBand", "StiEmptyBand", "StiOverlayBand"];
            var excludeComponents = ["StiTextInCells", "StiPanel", "StiClone", "StiSubReport", "StiTable"];
            if (components.bands) {
                var newBands = [];
                for (var i = 0; i < components.bands.items.length; i++) {
                    var bandName = components.bands.items[i];
                    if (excludeBands.indexOf(bandName) < 0) newBands.push(bandName);
                }
                components.bands.items = newBands;
            }
            if (components.components) {
                var newComponents = [];
                for (var i = 0; i < components.components.items.length; i++) {
                    var componentName = components.components.items[i];
                    if (excludeComponents.indexOf(componentName) < 0) newComponents.push(componentName);
                }
                components.components.items = newComponents;
            }
        }

        return components;
    }
    else {
        var components = {
            bands: {
                categoryVisible: true,
                items: ["StiPageHeaderBand", "StiPageFooterBand", "StiGroupHeaderBand", "StiGroupFooterBand", "StiHeaderBand", "StiFooterBand", "StiDataBand"]
            },
            crossBands: {
                categoryVisible: true,
                items: []
            },
            components: {
                categoryVisible: true,
                items: ["StiText", "StiImage"]
            },
            signatures: {
                categoryVisible: true,
                items: [],
            },
            shapes: {
                categoryVisible: true,
                items: [],
            },
            barcodes: {
                categoryVisible: true,
                items: [],
            },
            charts: {
                categoryVisible: true,
                items: []
            },
            maps: {
                categoryVisible: true,
                items: []
            },
            gauges: {
                categoryVisible: true,
                items: []
            }
        }

        if (this.options.chartAssemblyLoaded === false)
            delete components.charts;

        if (this.options.dashboardAssemblyLoaded) {
            components.dashboards = {
                categoryVisible: false,
                items: ["StiTableElement", "StiCardsElement", "StiPivotTableElement", "StiChartElement", "StiGaugeElement", "StiIndicatorElement", "StiProgressElement",
                    "StiRegionMapElement", "StiOnlineMapElement", "StiImageElement", "StiTextElement", "StiPanelElement", "StiShapeElement", "StiButtonElement", "StiComboBoxElement",
                    "StiDatePickerElement", "StiListBoxElement", "StiTreeViewBoxElement", "StiTreeViewElement"]
            }

            if (this.options.chartAssemblyLoaded === false) {
                var chartIndex = components.dashboards.items.indexOf("StiChartElement");
                components.dashboards.items.splice(chartIndex, 1);
            }
        }

        return components;
    }
}
