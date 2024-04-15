
StiMobileDesigner.prototype.InitializeSetupToolboxForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("setupToolboxForm", this.loc.FormDesigner.SetupToolbox, 4);

    //Tree
    var tree = this.Tree();
    tree.className = "stiSimpleContainerWithBorder";
    tree.style.overflow = "auto";
    tree.style.width = "350px";
    tree.style.height = "400px";
    tree.style.padding = "12px";
    tree.style.margin = "12px";

    tree.groups = {};
    form.container.appendChild(tree);

    var showToolbox = this.CheckBox(null, this.loc.MainMenu.menuViewShowToolbox);
    showToolbox.style.margin = "15px 100px 12px 15px";
    form.container.appendChild(showToolbox);

    var showInsertTab = this.CheckBox(null, this.loc.MainMenu.menuViewShowInsertTab);
    showInsertTab.style.margin = "12px 100px 15px 15px";
    form.container.appendChild(showInsertTab);

    showToolbox.action = function () {
        if (!showInsertTab.isChecked && !this.isChecked)
            showInsertTab.setChecked(true);
    }

    showInsertTab.action = function () {
        if (!showToolbox.isChecked && !this.isChecked)
            showToolbox.setChecked(true);
    }

    form.buildTree = function () {
        tree.clear();

        var allComponents = {
            bands: {
                image: "SmallComponents.StiReportTitleBand.png",
                text: jsObject.loc.Report.Bands,
                items: jsObject.options.designerSpecification != "Beginner"
                    ? ["StiReportTitleBand", "StiReportSummaryBand", "StiPageHeaderBand", "StiPageFooterBand", "StiGroupHeaderBand",
                        "StiGroupFooterBand", "StiHeaderBand", "StiFooterBand", "StiColumnHeaderBand", "StiColumnFooterBand", "StiDataBand", "StiHierarchicalBand",
                        "StiChildBand", "StiEmptyBand", "StiOverlayBand", "StiTableOfContents"]
                    : ["StiReportTitleBand", "StiReportSummaryBand", "StiPageHeaderBand", "StiPageFooterBand", "StiGroupHeaderBand",
                        "StiGroupFooterBand", "StiHeaderBand", "StiFooterBand", "StiDataBand", "StiTableOfContents"]
            },
            crossBands: {
                image: "SmallComponents.StiCrossGroupHeaderBand.png",
                text: jsObject.loc.Report.CrossBands,
                items: ["StiCrossGroupHeaderBand", "StiCrossGroupFooterBand", "StiCrossHeaderBand", "StiCrossFooterBand", "StiCrossDataBand"]
            },
            components: {
                image: "SmallComponents.StiText.png",
                text: jsObject.loc.Report.Components,
                items: jsObject.options.designerSpecification != "Beginner"
                    ? ["StiText", "StiTextInCells", "StiRichText", "StiImage", "StiPanel", "StiClone", "StiCheckBox", "StiSubReport", "StiZipCode", "StiTable", "StiCrossTab", "StiSparkline", "StiMathFormula"]
                    : ["StiText", "StiRichText", "StiImage", "StiCheckBox", "StiZipCode", "StiCrossTab", "StiSparkline", "StiMathFormula"]
            },
            signatures: {
                image: "SmallComponents.StiElectronicSignature.png",
                text: jsObject.loc.Components.StiSignature,
                items: ["StiElectronicSignature", "StiPdfDigitalSignature"]
            },
            barcodes: {
                image: "SmallComponents.StiBarCode.png",
                text: jsObject.loc.Components.StiBarCode,
                items: []
            },
            shapes: {
                image: "SmallComponents.StiShape.png",
                text: jsObject.loc.Report.Shapes,
                items: []
            },
            charts: {
                image: "SmallComponents.StiChart.png",
                text: jsObject.loc.Components.StiChart,
                items: []
            },
            maps: {
                image: "SmallComponents.StiMap.png",
                text: jsObject.loc.Components.StiMap,
                items: []
            },
            gauges: {
                image: "SmallComponents.StiGauge.png",
                text: jsObject.loc.Components.StiGauge,
                items: []
            },
            dashboards: {
                image: "SmallComponents.StiDashboard.png",
                text: jsObject.loc.Permissions.ItemDashboards,
                items: ["StiTableElement", "StiCardsElement", "StiPivotTableElement", "StiChartElement", "StiGaugeElement", "StiIndicatorElement", "StiProgressElement", "StiRegionMapElement",
                    "StiOnlineMapElement", "StiImageElement", "StiTextElement", "StiPanelElement", "StiShapeElement", "StiButtonElement", "StiComboBoxElement", "StiDatePickerElement", "StiListBoxElement",
                    "StiTreeViewBoxElement", "StiTreeViewElement"]
            }
        }

        var selectedComponents = jsObject.GetComponentsIntoInsertTab();

        var rootItem = jsObject.TreeItemForSetupToolboxForm("Components", "SmallComponents.StiText.png", null, tree);
        rootItem.button.style.display = "none";
        rootItem.iconOpening.style.display = "none";
        rootItem.setOpening(true);
        tree.appendChild(rootItem);

        var isDashboard = jsObject.options.currentPage && jsObject.options.currentPage.isDashboard;

        for (var groupName in allComponents) {
            var groupItem = jsObject.TreeItemForSetupToolboxForm(allComponents[groupName].text, allComponents[groupName].image, null, tree);
            tree.groups[groupName] = groupItem;
            groupItem.setOpening(true);
            rootItem.addChild(groupItem);

            groupItem.style.display = (groupName == "dashboards" && isDashboard) ||
                (((groupName == "bands" && jsObject.IsVisibilityBands()) ||
                    (groupName == "crossBands" && jsObject.IsVisibilityCrossBands()) ||
                    (groupName == "components" && jsObject.IsVisibilityComponents()) ||
                    (groupName == "shapes" && jsObject.IsVisibilityShapes()) ||
                    (groupName == "signatures" && jsObject.IsVisibilitySignatures()) ||
                    (groupName == "barcodes" && jsObject.options.visibilityComponents.StiBarCode) ||
                    (groupName == "charts" && jsObject.options.visibilityComponents.StiChart) ||
                    (groupName == "maps" && jsObject.options.visibilityComponents.StiMap && jsObject.options.designerSpecification != "Beginner") ||
                    (groupName == "gauges" && jsObject.options.visibilityComponents.StiGauge && jsObject.options.designerSpecification != "Beginner")) && !isDashboard) ? "" : "none";

            if (selectedComponents[groupName]) {
                groupItem.setChecked(selectedComponents[groupName].categoryVisible);
            }

            if (groupName == "dashboards") {
                groupItem.iconOpening.style.display = "none";
                groupItem.button.style.display = "none";
            }

            for (var i = 0; i < allComponents[groupName].items.length; i++) {
                var componentType = allComponents[groupName].items[i];

                if (!jsObject.options.visibilityComponents[componentType] &&
                    !jsObject.options.visibilityBands[componentType] &&
                    !jsObject.options.visibilityCrossBands[componentType] &&
                    (jsObject.options.dashboardAssemblyLoaded && !jsObject.options.visibilityDashboardElements[componentType]))
                    continue;

                var text = jsObject.loc.Components[groupName == "dashboards" ? componentType.replace("Element", "") : componentType];
                var image = (groupName == "dashboards" ? "Dashboards.SmallComponents." : "SmallComponents.") + componentType + ".png";

                var componentItem = jsObject.TreeItemForSetupToolboxForm(text, image, { componentName: componentType }, tree);
                groupItem.addChild(componentItem);

                if (selectedComponents[groupName]) {
                    componentItem.setChecked(jsObject.IsContains(selectedComponents[groupName].items, componentType));
                }
            }
        }
    }

    form.onshow = function () {
        form.buildTree();
        tree.style.display = this.jsObject.options.componentsIntoInsertTab ? "none" : "";
        showInsertTab.setChecked(this.jsObject.options.showInsertTab);
        showInsertTab.setEnabled(this.jsObject.options.showInsertButton); //supported old property
        showToolbox.setChecked(this.jsObject.options.showToolbox);
    }

    form.getSelectedComponents = function () {
        var components = {
            bands: { categoryVisible: false, items: [] },
            crossBands: { categoryVisible: false, items: [] },
            components: { categoryVisible: false, items: [] },
            shapes: { categoryVisible: false, items: [] },
            signatures: { categoryVisible: false, items: [] },
            barcodes: { categoryVisible: false, items: [] },
            charts: { categoryVisible: false, items: [] },
            maps: { categoryVisible: false, items: [] },
            gauges: { categoryVisible: false, items: [] }
        };

        if (this.jsObject.options.dashboardAssemblyLoaded) {
            components.dashboards = { categoryVisible: false, items: [] };
        }

        for (var groupName in components) {
            components[groupName].categoryVisible = tree.groups[groupName].isChecked;

            for (var i = 0; i < tree.groups[groupName].childsContainer.childNodes.length; i++) {
                var componentItem = tree.groups[groupName].childsContainer.childNodes[i];
                if (componentItem.isChecked && componentItem.style.display == "") {
                    components[groupName].items.push(componentItem.itemObject.componentName);
                }
            }
        }

        return components;
    }

    form.action = function () {
        this.changeVisibleState(false);
        var components = this.getSelectedComponents();

        if (this.jsObject.options.insertPanel) this.jsObject.options.insertPanel.update(components);
        this.jsObject.SetCookie("StimulsoftMobileDesignerComponentsIntoInsertTab_NewVers2", JSON.stringify(components));
        this.jsObject.options.showInsertTab = showInsertTab.isChecked;
        this.jsObject.options.showToolbox = showToolbox.isChecked;
        this.jsObject.SetCookie("StimulsoftMobileDesignerSetupToolbox", JSON.stringify({
            showToolbox: showToolbox.isChecked,
            showInsertTab: showInsertTab.isChecked
        }));

        if (this.jsObject.options.buttons.insertToolButton) {
            this.jsObject.options.buttons.insertToolButton.parentElement.style.display = (!this.jsObject.options.showInsertButton ? false : this.jsObject.options.showInsertTab) ? "" : "none";
            if (this.jsObject.options.workPanel.currentPanel == this.jsObject.options.insertPanel && !showInsertTab.isChecked) {
                this.jsObject.options.workPanel.showPanel(this.jsObject.options.homePanel);
                this.jsObject.options.buttons.homeToolButton.setSelected(true);
            }
        }

        this.jsObject.options.toolbox.changeVisibleState(showToolbox.isChecked);
        if (showToolbox.isChecked) this.jsObject.options.toolbox.update(components);
    }

    return form;
}

StiMobileDesigner.prototype.TreeItemForSetupToolboxForm = function (caption, imageName, itemObject, tree) {
    var item = this.TreeItem(caption, imageName, itemObject, tree, true);
    item.style.margin = "2px 0 2px 0";

    item.checkBox.action = function () {
        this.treeItem.setChecked(this.isChecked);
    }

    return item;
}