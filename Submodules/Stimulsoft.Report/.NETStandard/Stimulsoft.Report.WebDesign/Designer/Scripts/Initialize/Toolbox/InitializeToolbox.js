
StiMobileDesigner.prototype.InitializeToolbox = function () {
    var toolbox = document.createElement("div");
    this.options.toolbox = toolbox;
    this.options.mainPanel.appendChild(toolbox);
    toolbox.className = "stiDesignerToolbox";
    var jsObject = toolbox.jsObject = this;
    toolbox.style.display = "none";
    toolbox.visible = false;
    toolbox.style.left = "0px";
    toolbox.style.bottom = this.options.statusPanel.offsetHeight + "px";
    toolbox.style.top = (this.options.toolBar.offsetHeight + this.options.workPanel.offsetHeight + this.options.infoPanel.offsetHeight) + "px";
    toolbox.style.width = this.options.isTouchDevice ? "38px" : "30px";
    toolbox.style.zIndex = 2;
    toolbox.buttons = {};
    toolbox.selectedComponent = null;

    toolbox.resetChoose = function () {
        jsObject.options.drawComponent = false;
        jsObject.options.paintPanel.changeCursorType(false);

        if (this.selectedComponent) {
            this.selectedComponent.setSelected(false);
            this.selectedComponent = null;
        }

        if (this.buttons.bands) this.buttons.bands.setSelected(false);
        if (this.buttons.crossBands) this.buttons.crossBands.setSelected(false);
        if (this.buttons.components) this.buttons.components.setSelected(false);
        if (this.buttons.signatures) this.buttons.signatures.setSelected(false);
        if (this.buttons.barCodes) this.buttons.barCodes.setSelected(false);
        if (this.buttons.shapes) this.buttons.shapes.setSelected(false);
        if (this.buttons.charts) this.buttons.charts.setSelected(false);
        if (this.buttons.maps) this.buttons.maps.setSelected(false);
        if (this.buttons.gauges) this.buttons.gauges.setSelected(false);

        var buttons = jsObject.options.buttons;
        if (buttons.toolBoxMapsElements) buttons.toolBoxMapsElements.setSelected(false);
        if (buttons.toolBoxFiltersElements) buttons.toolBoxFiltersElements.setSelected(false);
    }

    toolbox.setChoose = function (selectedElement) {
        jsObject.options.drawComponent = true;
        jsObject.options.paintPanel.setCopyStyleMode(false);
        jsObject.options.paintPanel.changeCursorType(true);
        jsObject.options.toolbox.selectedComponent = selectedElement;

        if (selectedElement.menu && selectedElement.menu.parentButton) {
            selectedElement.menu.parentButton.setSelected(true);
        }
        else {
            selectedElement.setSelected(true);
        }
    }

    toolbox.update = function (components) {
        if (toolbox.mainTable) {
            toolbox.removeChild(toolbox.mainTable);
            toolbox.buttons = {};
        }

        toolbox.mainTable = jsObject.CreateHTMLTable();
        toolbox.mainTable.style.margin = "2px 0 0 2px";
        toolbox.appendChild(toolbox.mainTable);

        if (toolbox.moreButton) {
            toolbox.removeChild(toolbox.moreButton);
        }

        var moreButton = jsObject.ToolboxButton(null, "Arrows.ArrowRight.png", jsObject.loc.Gui.sys_morebuttons);
        moreButton.style.position = "absolute";
        moreButton.style.bottom = moreButton.style.left = "2px";
        moreButton.style.display = "none";
        toolbox.appendChild(moreButton);
        toolbox.moreButton = moreButton;

        var moreMenu = jsObject.HorizontalMenu("moreButtonsToolbox", moreButton, "Right", []);
        moreMenu.buttons = [];
        toolbox.moreMenu = moreMenu;

        moreMenu.clear = function () {
            while (this.innerContent.childNodes[0]) this.innerContent.removeChild(this.innerContent.childNodes[0]);
            this.buttons = [];
        }

        moreButton.action = function () {
            moreMenu.changeVisibleState(!moreMenu.visible);
        }

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
                    var image = (isDashboardElement ? "Dashboards.SmallComponents." : "SmallComponents.") + componentTypes[i] + ".png";
                    var tooltip = ["<b>" + text + "</b><br><br><table><tr><td style='vertical-align: top;'>" + (jsObject.loc.HelpComponents[componentTypes[i]] || "") + "</td></tr></table>", jsObject.HelpLinks["insertcomponent"]];
                    var button = jsObject.ToolboxButton("toolBox" + componentTypes[i], image, tooltip, false, true);

                    button.action = function () {
                        toolbox.resetChoose();
                        this.setSelected(!this.isSelected);
                        if (this.isSelected) toolbox.setChoose(this);
                    }

                    button.ondblclick = function () {
                        var params = { createdByDblClick: true }
                        var currComp = jsObject.options.selectedObject;
                        if (currComp && currComp.typeComponent != "StiPage" && currComp.typeComponent != "StiReport") {
                            params.currentComponent = currComp.properties.name;
                        }
                        jsObject.SendCommandCreateComponent(jsObject.options.currentPage.properties.name, this.name, "0!0!0!0", params);
                        if (jsObject.options.toolbox) jsObject.options.toolbox.resetChoose();
                    }

                    if (isDashboardElement) {
                        if (componentTypes[i] == "StiShapeElement") {
                            button = jsObject.ToolboxButton("toolBoxShapesElements", "SmallComponents.StiShape.png", tooltip, true, false);

                            button.action = function () {
                                var shapesMenu = jsObject.ShapesMenu("toolboxShapesElementsMenu", this, true, true, true);
                                shapesMenu.changeVisibleState(!shapesMenu.visible);
                            }
                        }
                        else if (componentTypes[i] == "StiRegionMapElement" || componentTypes[i] == "StiOnlineMapElement") {
                            if (mapsMenuCreated) continue;

                            tooltip = ["<b>" + jsObject.loc.PropertyMain.Maps + "</b><br><br>" +
                                "<table><tr><td style='vertical-align: top;'>" + jsObject.loc.HelpComponents.StiMapCategory + "</td></tr></table>", jsObject.HelpLinks["insertcomponent"]];

                            button = jsObject.ToolboxButton("toolBoxMapsElements", "SmallComponents.StiMap.png", tooltip, true, false);

                            var mapsMenu = jsObject.MapsElementsMenu("toolboxMapsElementsMenu", button, true, componentTypes);
                            mapsMenuCreated = true;

                            button.action = function () {
                                mapsMenu.changeVisibleState(!mapsMenu.visible);
                            }
                        }
                        else if (jsObject.IsFilterElement(componentTypes[i])) {

                            if (filterMenuCreated) continue;

                            tooltip = ["<b>" + jsObject.loc.PropertyMain.Filters + "</b><br><br>" +
                                "<table><tr><td style='vertical-align: top;'>" + jsObject.loc.HelpComponents.StiFilterCategory + "</td></tr></table>", jsObject.HelpLinks["insertcomponent"]];

                            button = jsObject.ToolboxButton("toolBoxFiltersElements", "Dashboards.SmallComponents.StiFilterElement.png", tooltip, true, false);

                            var filtersMenu = jsObject.FiltersElementsMenu("toolboxFiltersElementsMenu", button, true, componentTypes);
                            filterMenuCreated = true;

                            button.action = function () {
                                filtersMenu.changeVisibleState(!filtersMenu.visible);
                            }
                        }
                    }

                    button.isDashboardElement = isDashboardElement;
                    button.toolboxOwner = true;
                    button.name = componentTypes[i];
                    toolbox.buttons[componentTypes[i]] = button;
                    toolbox.mainTable.addCellInNextRow(button);
                    button.setEnabled(jsObject.options.report != null);

                    button.onmouseenter = function () {
                        var this_ = this;
                        if (!this.isEnabled || (this["haveMenu"] && this.isSelected) || jsObject.options.isTouchClick) return;
                        this.className = this.overClass;
                        this.isOver = true;
                        if (jsObject.options.showTooltips && this.toolTip && typeof (this.toolTip) == "object")
                            jsObject.options.toolTip.showWithDelay(
                                this.toolTip[0],
                                this.toolTip[1],
                                jsObject.options.isTouchDevice ? 38 : 32,
                                jsObject.FindPosY(this_, "stiDesignerMainPanel")
                            );
                    }
                }
            }
        }

        var componentTypes = jsObject.options.componentsIntoInsertTab || components || jsObject.GetComponentsIntoInsertTab();

        if (!jsObject.options.componentsIntoInsertTab) {
            var menuButtons = [
                ["bands", "Toolbox.Bands.png"],
                ["crossBands", "Toolbox.CrossBands.png"],
                ["components", "Toolbox.Components.png"],
                ["signatures", "Toolbox.Signatures.png"],
                ["barCodes", "SmallComponents.StiBarCode.png"],
                ["shapes", "SmallComponents.StiShape.png"],
                ["charts", "SmallComponents.StiChart.png"],
                ["gauges", "SmallComponents.StiGauge.png"],
                ["maps", "SmallComponents.StiMap.png"]
            ];

            for (var i = 0; i < menuButtons.length; i++) {
                var button = jsObject.ToolboxButton("toolBox" + menuButtons[i][0], menuButtons[i][1], null, menuButtons[i][0] != "maps", false);
                toolbox.mainTable.addCellInNextRow(button);
                toolbox.buttons[menuButtons[i][0]] = button;
                button.setEnabled(jsObject.options.report != null);
            }

            toolbox.buttons.separatorGroups = jsObject.ToolboxSeparator();
            toolbox.mainTable.addCellInNextRow(toolbox.buttons.separatorGroups);

            var bandsMenu = jsObject.BandsMenu("toolboxBandsMenu", toolbox.buttons.bands, true);
            toolbox.buttons.bands.style.display = componentTypes["bands"] && componentTypes["bands"].categoryVisible && jsObject.IsVisibilityBands() ? "" : "none";
            toolbox.buttons.bands.action = function () {
                bandsMenu.changeVisibleState(!bandsMenu.visible);
            }

            var crossBandsMenu = jsObject.CrossBandsMenu("toolboxCrossBandsMenu", toolbox.buttons.crossBands, true);
            toolbox.buttons.crossBands.style.display = componentTypes["crossBands"] && componentTypes["crossBands"].categoryVisible && jsObject.IsVisibilityCrossBands() ? "" : "none";
            toolbox.buttons.crossBands.action = function () {
                crossBandsMenu.changeVisibleState(!crossBandsMenu.visible);
            }

            var componentsMenu = jsObject.ComponentsMenu("toolboxComponentsMenu", toolbox.buttons.components, true);
            toolbox.buttons.components.style.display = componentTypes["components"] && componentTypes["components"].categoryVisible && jsObject.IsVisibilityComponents() ? "" : "none";
            toolbox.buttons.components.action = function () {
                componentsMenu.changeVisibleState(!componentsMenu.visible);
            }

            toolbox.buttons.signatures.style.display = componentTypes["signatures"] && componentTypes["signatures"].categoryVisible && jsObject.IsVisibilitySignatures() ? "" : "none";
            toolbox.buttons.signatures.action = function () {
                var signaturesMenu = jsObject.SignaturesMenu("toolboxSignaturesMenu", toolbox.buttons.signatures, true);
                signaturesMenu.changeVisibleState(!signaturesMenu.visible);
            }

            toolbox.buttons.barCodes.style.display = componentTypes["barcodes"] && componentTypes["barcodes"].categoryVisible && jsObject.options.visibilityComponents.StiBarCode ? "" : "none";
            toolbox.buttons.barCodes.action = function () {
                var barCodesMenu = jsObject.BarCodesMenu("toolboxBarCodesMenu", toolbox.buttons.barCodes, true);
                barCodesMenu.changeVisibleState(!barCodesMenu.visible);
            }

            toolbox.buttons.shapes.style.display = componentTypes["shapes"] && componentTypes["shapes"].categoryVisible && jsObject.IsVisibilityShapes() ? "" : "none";
            toolbox.buttons.shapes.action = function () {
                var shapesMenu = jsObject.ShapesMenu("toolboxShapesMenu", toolbox.buttons.shapes, false, false, true);
                shapesMenu.changeVisibleState(!shapesMenu.visible);
            }

            toolbox.buttons.charts.style.display = componentTypes["charts"] && componentTypes["charts"].categoryVisible && jsObject.options.visibilityComponents.StiChart ? "" : "none";
            toolbox.buttons.charts.action = function () {
                var chartsMenu = jsObject.ChartsMenu("toolboxChartsMenu", toolbox.buttons.charts, true);
                chartsMenu.changeVisibleState(!chartsMenu.visible);
            }

            toolbox.buttons.gauges.style.display = componentTypes["gauges"] && componentTypes["gauges"].categoryVisible && jsObject.options.visibilityComponents.StiGauge && jsObject.options.designerSpecification != "Beginner" ? "" : "none";
            toolbox.buttons.gauges.action = function () {
                var gaugesMenu = jsObject.GaugesMenu("toolboxGaugesMenu", toolbox.buttons.gauges, true);
                gaugesMenu.changeVisibleState(!gaugesMenu.visible);
            }

            toolbox.buttons.maps.style.display = componentTypes["maps"] && componentTypes["maps"].categoryVisible && jsObject.options.visibilityComponents.StiMap && jsObject.options.designerSpecification != "Beginner" ? "" : "none";
            toolbox.buttons.maps.name = "StiMap";

            jsObject.AddDragEventsToComponentButton(toolbox.buttons.maps);

            toolbox.buttons.maps.action = function () {
                jsObject.InitializeMapCategoriesForm(function (form) {
                    toolbox.buttons.maps.setSelected(true);
                    form.show(toolbox);
                });
            }
        }

        var addSetupButtonSeparator = false;

        if (jsObject.Is_array(componentTypes)) {
            addComponentButtons(componentTypes);
        }
        else {
            var addSep = false;
            for (var groupName in componentTypes) {
                if (componentTypes[groupName].length > 0) {
                    var componentsInGroup = componentTypes[groupName];
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
                            toolbox.buttons["separator" + groupName] = jsObject.ToolboxSeparator();
                            toolbox.mainTable.addCellInNextRow(toolbox.buttons["separator" + groupName]);
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
                toolbox.setupButtonSeparator = jsObject.ToolboxSeparator();
                toolbox.mainTable.addCellInNextRow(toolbox.setupButtonSeparator);
            }
            var setupToolboxButton = jsObject.ToolboxButton(null, "Toolbox.SmallSetupToolbox.png", jsObject.loc.FormDesigner.SetupToolbox, false, false);
            toolbox.mainTable.addCellInNextRow(setupToolboxButton);

            setupToolboxButton.action = function () {
                this.jsObject.InitializeSetupToolboxForm(function (form) {
                    form.changeVisibleState(true);
                });
            }
        }

        toolbox.setMode(true);
        toolbox.resize();
    }

    toolbox.getCurrentMode = function () {
        return (jsObject.options.report && jsObject.options.currentPage && jsObject.options.currentPage.isDashboard ? "Dashboard" : "Page");
    }

    toolbox.changeVisibleState = function (state) {
        this.visible = state;
        this.style.display = state ? "" : "none";
        var paintPanel = jsObject.options.paintPanel;
        var pagesPanel = jsObject.options.pagesPanel;
        var propertiesPanel = jsObject.options.propertiesPanel;
        var marginLeft = propertiesPanel.fixedViewMode ? 30 : 0;
        if (jsObject.options.propertiesGridPosition == "Right") {
            propertiesPanel.style.right = marginLeft + "px";
        }
        else {
            propertiesPanel.style.left = (this.offsetWidth + marginLeft) + "px";
        }
        propertiesPanel.showButtonsPanel.style.left = this.offsetWidth + "px";

        if (jsObject.options.propertiesGridPosition == "Right") {
            paintPanel.style.left = this.offsetWidth + "px";
            paintPanel.style.right = (propertiesPanel.fixedViewMode ? 0 : propertiesPanel.offsetWidth) + "px";
            if (pagesPanel) {
                pagesPanel.style.right = paintPanel.style.right;
                pagesPanel.style.left = (jsObject.options.toolbox ? jsObject.options.toolbox.offsetWidth : 0) + "px";
            }
        }
        else {
            paintPanel.style.left = ((propertiesPanel.fixedViewMode ? 0 : propertiesPanel.offsetWidth) + this.offsetWidth) + "px";
            if (pagesPanel) pagesPanel.style.left = paintPanel.style.left;
        }

        if (pagesPanel) {
            pagesPanel.updateScrollButtons();
        }
    }

    toolbox.setMode = function (manually) {
        var mode = toolbox.getCurrentMode();
        if (this.mode == mode && !manually) return;
        this.mode = mode;

        if (toolbox.setupButtonSeparator)
            toolbox.setupButtonSeparator.style.display = "none";

        for (var name in this.buttons) {
            var button = this.buttons[name];
            button.parentNode.style.display = (mode == "Dashboard" && button.isDashboardElement && name.indexOf("separator") < 0) || (mode == "Page" && !button.isDashboardElement) ? "" : "none";

            if (button.parentNode.style.display == "" && toolbox.setupButtonSeparator) {
                toolbox.setupButtonSeparator.style.display = "";
            }
        }

        toolbox.resize();
    }

    toolbox.presentsScroll = function () {
        return (toolbox.mainTable && toolbox.mainTable.offsetHeight && toolbox.mainTable.offsetHeight > toolbox.offsetHeight - 40);
    }

    toolbox.resize = function () {
        clearTimeout(this.resizeTimer);
        setTimeout(function () {
            if (!jsObject.options.isTouchDevice) {
                var moreMenu = toolbox.moreMenu;
                var mode = toolbox.getCurrentMode();

                if (moreMenu && moreMenu.buttons.length > 0) {
                    for (var i = 0; i < moreMenu.buttons.length; i++) {
                        var button = moreMenu.buttons[i];
                        button.parentCell.appendChild(button);
                        button.parentCell.style.display = (mode == "Dashboard" && button.isDashboardElement) || (mode == "Page" && !button.isDashboardElement) ? "" : "none";
                    }
                    if (moreMenu.visible) {
                        moreMenu.changeVisibleState(false);
                    }
                    moreMenu.clear();
                }

                if (toolbox.moreButton) {
                    toolbox.moreButton.style.display = "none";
                }

                if (toolbox.presentsScroll()) {
                    var moreTable = jsObject.CreateHTMLTable();
                    moreMenu.innerContent.appendChild(moreTable);

                    for (var i = toolbox.mainTable.tr.length - 1; i >= 0; i--) {
                        var buttonCell = toolbox.mainTable.tr[i].firstChild;
                        if (buttonCell && buttonCell.style.display == "" && buttonCell.firstChild && buttonCell.firstChild.name) {
                            var button = buttonCell.firstChild;
                            button.parentCell = button.parentElement;
                            moreTable.insertCell(0, button);
                            moreMenu.buttons.push(button);
                            if (!toolbox.presentsScroll()) break;
                        }
                    }

                    if (toolbox.moreButton && moreMenu.buttons.length > 0) {
                        toolbox.moreButton.style.display = "";
                    }
                }
            }
        }, 0);
    }

    this.addEvent(window, "resize", function (event) {
        toolbox.resize();
    });

    if (this.options.showToolbox) {
        toolbox.changeVisibleState(true);
        toolbox.update();
        toolbox.setMode();
    }

    setTimeout(function () { toolbox.resize(); }, 100);

    return toolbox;
}

StiMobileDesigner.prototype.ToolboxSeparator = function () {
    var sep = document.createElement("div");
    sep.className = "stiDesignerHomePanelSeparator";
    sep.style.height = "1px";
    sep.style.margin = "2px 0 2px 0";
    sep.style.width = this.options.isTouchDevice ? "30px" : "24px";

    return sep;
}

StiMobileDesigner.prototype.ToolboxButton = function (name, imageName, tooltip, haveMenu, haveDragEvent) {
    var button = this.StandartSmallButton(name, null, null, imageName, tooltip);
    button.style.width = this.options.isTouchDevice ? "32px" : "24px";
    button.style.height = this.options.isTouchDevice ? "32px" : "24px";
    button.innerTable.style.width = "100%";
    button.imageName = imageName;
    if (button.imageCell) button.imageCell.style.padding = "0";

    if (haveMenu) {
        var arrow = document.createElement("img");
        button.arrow = arrow;
        arrow.style.margin = "3px 0 3px 0";
        arrow.style.width = arrow.style.height = "8px";
        StiMobileDesigner.setImageSource(arrow, this.options, "Arrows.SmallArrowRight.png");
        var arrowCell = button.innerTable.addCellInNextRow();
        arrowCell.style.textAlign = "center";
        arrowCell.appendChild(arrow);
        button.style.height = this.options.isTouchDevice ? "34px" : "30px";
    }

    if (haveDragEvent) {
        this.AddDragEventsToComponentButton(button);
    }

    return button;
}

StiMobileDesigner.prototype.AddDragEventsToComponentButton = function (button) {
    var jsObject = this;

    button.onmousedown = function (event) {
        if (this.isTouchStartFlag || !this.isEnabled) return;
        jsObject.options.buttonPressed = this;
        this.ontouchstart(event, true);
    }

    button.ontouchstart = function (event, mouseProcess) {
        var this_ = this;
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        jsObject.options.fingerIsMoved = false;
        jsObject.options.buttonPressed = this;
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);

        if (jsObject.options.controlsIsFocused) {
            jsObject.options.controlsIsFocused.blur(); //fixed bug when drag&drop component from toolbar
        }
        if (event && !this.isTouchStartFlag) event.preventDefault();
        if (event.button != 2 && this.name) {
            var componentButtonInDrag = this.isDashboardElement
                ? jsObject.DashboardElementForDragDrop(null, this.name, this.imageName)
                : jsObject.ComponentForDragDrop(null, this.name, this.imageName);

            if (componentButtonInDrag) {
                componentButtonInDrag.ownerButton = this;
                componentButtonInDrag.beginingOffset = 0;
                jsObject.options.componentButtonInDrag = componentButtonInDrag;
            }
        }
    }

    jsObject.addEvent(button, "touchend", function (event) {
        if (jsObject.options.componentButtonInDrag && jsObject.options.componentButtonInDrag.beginingOffset >= 10) {
            jsObject.DropDragableItemToActiveContainer(jsObject.options.componentButtonInDrag);
        }
    });
}