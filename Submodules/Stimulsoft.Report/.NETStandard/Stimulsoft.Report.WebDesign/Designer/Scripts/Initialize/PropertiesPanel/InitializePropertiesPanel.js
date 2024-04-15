
StiMobileDesigner.prototype.InitializePropertiesPanel = function () {
    var jsObject = this;

    if (this.options.propertiesPanel) {
        this.options.mainPanel.removeChild(this.options.propertiesPanel);
        if (this.options.propertiesPanel.showButtonsPanel) {
            this.options.mainPanel.removeChild(this.options.propertiesPanel.showButtonsPanel);
        }
    }

    var propertiesPanel = document.createElement("div");
    propertiesPanel.id = this.options.mobileDesigner.id + "propertiesPanel";
    this.options.propertiesPanel = propertiesPanel;
    this.options.mainPanel.appendChild(propertiesPanel);
    propertiesPanel.jsObject = this;
    propertiesPanel.fixedViewMode = false;

    if (this.options.propertiesGridPosition == "Right") {
        propertiesPanel.className = "stiDesignerPropertiesPanelRight";
        propertiesPanel.style.right = "0px";
    }
    else {
        propertiesPanel.className = "stiDesignerPropertiesPanel";
        propertiesPanel.style.left = (this.options.toolbox ? this.options.toolbox.offsetWidth : 0) + "px";
    }

    propertiesPanel.style.bottom = this.options.statusPanel.offsetHeight + "px";
    propertiesPanel.style.top = (this.options.toolBar.offsetHeight + this.options.workPanel.offsetHeight + this.options.infoPanel.offsetHeight) + "px";
    propertiesPanel.style.width = this.options.propertiesGridWidth + "px";
    propertiesPanel.style.zIndex = 2;
    propertiesPanel.dictionaryMode = false;
    propertiesPanel.editFormControl = null;
    propertiesPanel.eventsMode = false;
    propertiesPanel.dictionaryDataMode = false;
    propertiesPanel.isEnabled = true;

    //Show Properties Panel Button
    propertiesPanel.showButtonsPanel = this.PropertiesPanelShowButtonsPanel(propertiesPanel);
    this.options.mainPanel.appendChild(propertiesPanel.showButtonsPanel);

    //Header
    propertiesPanel.header = this.PropertiesPanelHeader(propertiesPanel);
    propertiesPanel.appendChild(propertiesPanel.header);

    //Components List
    var componentsListPanel = document.createElement("div");
    componentsListPanel.className = "stiDesignerComponentsListPanel";

    var componentsTable = this.CreateHTMLTable();
    componentsListPanel.appendChild(componentsTable);

    var componentsList = this.DropDownList("componentsList", this.options.propertiesGridWidth - (this.options.isTouchDevice ? 54 : 50), null, null, true, null, this.options.isTouchDevice ? 28 : 24, true);
    componentsTable.addCell(componentsList);

    var sortButton = this.StandartSmallButton("componentsListSort", null, null, "SortAZ.png", this.loc.Report.Alphabetical);
    sortButton.style.marginLeft = "5px";
    componentsTable.addCell(sortButton);

    sortButton.action = function () {
        this.setSelected(!this.isSelected);
    }

    componentsList.button.action = function () {
        if (!componentsList.menu.visible) {
            var componentsItems = jsObject.GetAllComponentsItems(sortButton.isSelected);
            componentsList.menu.addItems(componentsItems);
            componentsList.menu.changeVisibleState(true);
        }
        else
            componentsList.menu.changeVisibleState(false);
    }

    componentsList.action = function () {
        if (this.key && jsObject.options.report) {
            if (jsObject.Is_array(this.key) && this.key.length >= 3) {
                //Is CrossTabField
                var crossTab = jsObject.options.report.getComponentByName(this.key[1]);
                if (crossTab) {
                    var crossFields = crossTab.controls.crossTabContainer.childNodes;
                    for (var i = 0; i < crossFields.length; i++) {
                        if (crossFields[i].properties.name == this.key[2]) {
                            crossFields[i].action();
                        }
                    }
                }
            }
            else if (jsObject.EndsWith(this.key, " : " + jsObject.loc.Components.StiReport)) {
                //Is Report
                this.jsObject.options.report.setSelected();
            }
            else if (jsObject.EndsWith(this.key, " : " + jsObject.loc.Components.StiPage)) {
                //Is Page
                var page = jsObject.options.report.pages[this.key.replace(" : " + jsObject.loc.Components.StiPage, "")];
                if (page) jsObject.options.paintPanel.showPage(page);
            }
            else if (jsObject.EndsWith(this.key, " : " + jsObject.loc.Components.StiDashboard)) {
                //Is Dashboard
                var page = jsObject.options.report.pages[this.key.replace(" : " + jsObject.loc.Components.StiDashboard, "")];
                if (page) jsObject.options.paintPanel.showPage(page);
            }
            else {
                //Other Components
                var componentName = this.key.indexOf(" : ") ? this.key.substring(0, this.key.lastIndexOf(" : ")) : this.key;
                var component = jsObject.options.report.getComponentByName(componentName);
                if (component) {
                    if (component.properties.pageName &&
                        component.properties.pageName != jsObject.options.currentPage.properties.name &&
                        jsObject.options.report.pages[component.properties.pageName]) {
                        jsObject.options.paintPanel.showPage(jsObject.options.report.pages[component.properties.pageName]);
                    }
                    component.setSelected();
                }
            }

            jsObject.UpdatePropertiesControls();
        }
    }

    componentsListPanel.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        propertiesPanel.containers["Properties"].style.top = state ? (jsObject.options.isTouchDevice ? "74px" : "69px") : "35px";
    }

    propertiesPanel.appendChild(componentsListPanel);

    propertiesPanel.setEventsMode = function (state) {
        this.propertiesToolBar.controls.PropertiesTab.setSelected(!state);
        this.propertiesToolBar.controls.EventsTab.setSelected(state);
        this.eventsMode = state;
        if (this.editCrossTabMode) {
            if (this.editCrossTabPropertiesPanel)
                this.editCrossTabPropertiesPanel.style.display = state ? "none" : "";
        }
        else
            this.mainPropertiesPanel.style.display = state ? "none" : "";

        if (state)
            this.eventsPropertiesPanel.show();
        else
            this.eventsPropertiesPanel.hide();
    }

    propertiesPanel.oncontextmenu = function (event) {
        return false;
    }

    //Container Properties 
    propertiesPanel.containers = {};
    propertiesPanel.containers["Properties"] = document.createElement("div");
    propertiesPanel.containers["Properties"].className = "stiDesignerPropertiesPanelInnerContent";
    propertiesPanel.containers["Properties"].style.top = this.options.isTouchDevice ? "74px" : "69px"; //"35px";
    propertiesPanel.appendChild(propertiesPanel.containers["Properties"]);
    propertiesPanel.containers["Properties"].style.display = "none";
    propertiesPanel.containers["Properties"].style.overflowX = "hidden";

    //Add Main Properties
    propertiesPanel.mainPropertiesPanel = document.createElement("div");
    propertiesPanel.containers["Properties"].appendChild(propertiesPanel.mainPropertiesPanel);

    propertiesPanel.eventsPropertiesPanel = this.EventsPropertiesPanel("div");
    propertiesPanel.containers["Properties"].appendChild(propertiesPanel.eventsPropertiesPanel);

    //Add Style Designer Properties
    propertiesPanel.styleDesignerPropertiesPanel = document.createElement("div");
    propertiesPanel.styleDesignerPropertiesPanel.style.display = "none";
    propertiesPanel.containers["Properties"].appendChild(propertiesPanel.styleDesignerPropertiesPanel);

    //Add Dictionary Data Properties
    propertiesPanel.dictionaryDataPropertiesPanel = this.DictionaryDataPropertiesPanel("div");
    propertiesPanel.containers["Properties"].appendChild(propertiesPanel.dictionaryDataPropertiesPanel);

    //Container Dictionary    
    propertiesPanel.containers["Dictionary"] = this.DictionaryPanel();
    propertiesPanel.containers["Dictionary"].style.display = "none";
    propertiesPanel.appendChild(propertiesPanel.containers["Dictionary"]);

    //Container Report Tree    
    propertiesPanel.containers["ReportTree"] = this.ReportTreePanel();
    propertiesPanel.containers["ReportTree"].style.display = "none";
    propertiesPanel.appendChild(propertiesPanel.containers["ReportTree"]);

    //Footer
    propertiesPanel.footer = document.createElement("div");
    propertiesPanel.footer.className = "stiDesignerPropertiesPanelFooter";
    propertiesPanel.appendChild(propertiesPanel.footer);
    propertiesPanel.footer.appendChild(this.PropertiesPanelFooter());

    //Design Button
    var designButtonBlock = this.PropertyBlockWithButton("propertiesDesignButtonBlock", "StiText.png", this.loc.Buttons.Design + "...");
    designButtonBlock.style.display = "none";
    designButtonBlock.style.marginTop = "5px";
    propertiesPanel.mainPropertiesPanel.appendChild(designButtonBlock);

    designButtonBlock.button.action = function () {
        jsObject.ShowComponentForm(jsObject.options.selectedObject || jsObject.GetCommonObject(jsObject.options.selectedObjects));
    }

    //Disabled Panel
    var disabledPanel = document.createElement("div");
    disabledPanel.className = "stiDesignerDisabledPanelOfPropertiesPanel";
    disabledPanel.style.display = "none";
    propertiesPanel.appendChild(disabledPanel);

    propertiesPanel.getCurrentPanelName = function (state) {
        if (propertiesPanel.containers["Dictionary"] && propertiesPanel.containers["Dictionary"].style.display == "") return "Dictionary";
        else if (propertiesPanel.containers["Properties"] && propertiesPanel.containers["Properties"].style.display == "") return "Properties";
        else if (propertiesPanel.containers["ReportTree"] && propertiesPanel.containers["ReportTree"].style.display == "") return "ReportTree";
        return null;
    }

    propertiesPanel.setEnabled = function (state) {
        this.isEnabled = state;
        disabledPanel.style.display = !state ? "" : "none";
    }

    if (!this.options.showPanelPropertiesAndDictionary) {
        propertiesPanel.style.display = "none";
        propertiesPanel.showButtonsPanel.style.display = "none";
    }

    propertiesPanel.changeVisibleState = function (state) {
        var jsObject = propertiesPanel.jsObject;

        if (jsObject.options.showPropertiesGrid || jsObject.options.showDictionary || jsObject.options.showReportTree) {
            propertiesPanel.style.display = state ? "" : "none";
        }
        StiMobileDesigner.setImageSource(propertiesPanel.hideButton.image, jsObject.options, propertiesPanel.fixedViewMode ? "HidePanelFixedMode.png" : "HidePanel.png");
        propertiesPanel.className = propertiesPanel.fixedViewMode
            ? "stiDesignerPropertiesPanelFixedMode"
            : (propertiesPanel.jsObject.options.propertiesGridPosition == "Right" ? "stiDesignerPropertiesPanelRight" : "stiDesignerPropertiesPanel");
        var marginVert = propertiesPanel.fixedViewMode ? 5 : 0;
        var marginLeft = propertiesPanel.fixedViewMode ? 30 : 0;
        propertiesPanel.style.bottom = (jsObject.options.statusPanel.offsetHeight + marginVert) + "px";
        propertiesPanel.style.top = (jsObject.options.toolBar.offsetHeight + jsObject.options.workPanel.offsetHeight + jsObject.options.infoPanel.offsetHeight + marginVert) + "px";

        if (jsObject.options.propertiesGridPosition == "Right") {
            propertiesPanel.style.right = marginLeft + "px";
        }
        else {
            propertiesPanel.style.left = ((jsObject.options.toolbox ? jsObject.options.toolbox.offsetWidth : 0) + marginLeft) + "px";
        }

        if (!state) {
            var buttons = jsObject.options.buttons;
            if (buttons["showPropertiesPanelButton"]) buttons["showPropertiesPanelButton"].setSelected(false);
            if (buttons["showDictionaryPanelButton"]) buttons["showDictionaryPanelButton"].setSelected(false);
            if (buttons["showReportTreePanelButton"]) buttons["showReportTreePanelButton"].setSelected(false);
        }
        else {
            propertiesPanel.footer.style.display = !propertiesPanel.fixedViewMode ? "" : "none";
            var bottom = (propertiesPanel.fixedViewMode ? 0 : 30) + "px";
            propertiesPanel.containers["Properties"].style.bottom = bottom;
            propertiesPanel.containers["Dictionary"].style.bottom = bottom;
        }

        var toolbox = jsObject.options.toolbox;

        if (jsObject.options.propertiesGridPosition == "Right") {
            jsObject.options.paintPanel.style.right = (propertiesPanel.fixedViewMode ? 0 : propertiesPanel.offsetWidth) + "px";
            if (jsObject.options.pagesPanel) {
                jsObject.options.pagesPanel.style.right = jsObject.options.paintPanel.style.right;
                jsObject.options.pagesPanel.style.left = (toolbox ? toolbox.offsetWidth : 0) + "px";
            }
        }
        else {
            jsObject.options.paintPanel.style.left = ((propertiesPanel.fixedViewMode ? 0 : propertiesPanel.offsetWidth) + (toolbox ? toolbox.offsetWidth : 0)) + "px";
            if (jsObject.options.pagesPanel) jsObject.options.pagesPanel.style.left = jsObject.options.paintPanel.style.left;
        }

        if (jsObject.options.pagesPanel) {
            jsObject.options.pagesPanel.updateScrollButtons();
        }
    }

    propertiesPanel.setStyleDesignerMode = function (state) {
        if (state) {
            propertiesPanel.returnToPanel = propertiesPanel.getCurrentPanelName();
        }
        propertiesPanel.styleDesignerMode = state;
        propertiesPanel.showContainer("Properties");
        var buttons = jsObject.options.buttons;

        if (state && buttons.showPropertiesPanelButton) {
            buttons.showPropertiesPanelButton.setSelected(true);
        }

        propertiesPanel.mainPropertiesPanel.style.display = state ? "none" : "";
        propertiesPanel.styleDesignerPropertiesPanel.style.display = state ? "" : "none";
        componentsListPanel.changeVisibleState(!state);

        buttons.DictionaryTabButton.style.display = state ? "none" : "";
        if (buttons.showDictionaryPanelButton) {
            buttons.showDictionaryPanelButton.style.display = state ? "none" : "";
        }

        buttons.ReportTreeTabButton.style.display = state ? "none" : "";
        if (buttons.showReportTreePanelButton) {
            buttons.showReportTreePanelButton.style.display = state ? "none" : "";
        }

        if (state) {
            if (propertiesPanel.editCrossTabMode) propertiesPanel.editCrossTabPropertiesPanel.style.display = "none";
        }
        else {
            if (propertiesPanel.editChartMode) { propertiesPanel.setEditChartMode(true); }
            else if (propertiesPanel.editDbsMeterMode) { propertiesPanel.setEditDbsMeterMode(true); }
            else if (propertiesPanel.editCrossTabMode) { propertiesPanel.setEditCrossTabMode(true); }
            else if (propertiesPanel.editBarCodeMode) { propertiesPanel.setEditBarCodeMode(true); }
            else if (propertiesPanel.editChartSeriesMode) { propertiesPanel.setEditChartSeriesMode(true); }
            else if (propertiesPanel.editChartStripsMode) { propertiesPanel.setEditChartStripsMode(true); }
            else if (propertiesPanel.editChartConstLinesMode) { propertiesPanel.setEditChartConstLinesMode(true); }
            else if (propertiesPanel.returnToPanel) propertiesPanel.showContainer(propertiesPanel.returnToPanel);
        }
    }

    propertiesPanel.setDictionaryDataMode = function (state) {
        this.dictionaryDataMode = state;
        this.mainPropertiesPanel.style.display = state ? "none" : "";

        if (state)
            this.dictionaryDataPropertiesPanel.show();
        else
            this.dictionaryDataPropertiesPanel.hide();
    }

    propertiesPanel.setEditMode = function (state, editForm, ignoreHideButtons) {
        var buttons = jsObject.options.buttons;
        if (state && buttons.showPropertiesPanelButton) {
            buttons.showPropertiesPanelButton.setSelected(true);
            propertiesPanel.setZIndex(true, editForm.level);
        }
        propertiesPanel.mainPropertiesPanel.style.display = state ? "none" : "";

        buttons.DictionaryTabButton.style.display = state && !ignoreHideButtons ? "none" : "";
        if (buttons.showDictionaryPanelButton) {
            buttons.showDictionaryPanelButton.style.display = state && !ignoreHideButtons ? "none" : "";
        }
        buttons.ReportTreeTabButton.style.display = state && !ignoreHideButtons ? "none" : "";
        if (buttons.showReportTreePanelButton) {
            buttons.showReportTreePanelButton.style.display = state && !ignoreHideButtons ? "none" : "";
        }
        if (!state && propertiesPanel.returnToPanel) {
            propertiesPanel.showContainer(propertiesPanel.returnToPanel);
        }
    }

    propertiesPanel.setEditChartSeriesMode = function (state) {
        propertiesPanel.editChartSeriesMode = state;
        if (!propertiesPanel.editChartPropertiesPanel) {
            propertiesPanel.editChartPropertiesPanel = jsObject.ChartPropertiesPanel();
            propertiesPanel.containers.Properties.appendChild(propertiesPanel.editChartPropertiesPanel);
        }
        propertiesPanel.showContainer("Properties");
        propertiesPanel.editChartPropertiesPanel.style.display = state ? "" : "none";
        propertiesPanel.mainPropertiesPanel.style.display = state ? "none" : "";
    }

    propertiesPanel.setEditChartStripsMode = function (state) {
        propertiesPanel.editChartStripsMode = state;
        if (!propertiesPanel.editChartPropertiesPanel) {
            propertiesPanel.editChartPropertiesPanel = jsObject.ChartPropertiesPanel();
            propertiesPanel.containers.Properties.appendChild(propertiesPanel.editChartPropertiesPanel);
        }
        propertiesPanel.showContainer("Properties");
        propertiesPanel.editChartPropertiesPanel.style.display = state ? "" : "none";
        propertiesPanel.mainPropertiesPanel.style.display = state ? "none" : "";
    }

    propertiesPanel.setEditChartConstLinesMode = function (state) {
        propertiesPanel.editChartConstLinesMode = state;
        if (!propertiesPanel.editChartPropertiesPanel) {
            propertiesPanel.editChartPropertiesPanel = jsObject.ChartPropertiesPanel();
            propertiesPanel.containers.Properties.appendChild(propertiesPanel.editChartPropertiesPanel);
        }
        propertiesPanel.showContainer("Properties");
        propertiesPanel.editChartPropertiesPanel.style.display = state ? "" : "none";
        propertiesPanel.mainPropertiesPanel.style.display = state ? "none" : "";
    }

    propertiesPanel.setEditChartMode = function (state) {
        if (state) { propertiesPanel.returnToPanel = propertiesPanel.getCurrentPanelName(); }
        propertiesPanel.editChartMode = state;
        propertiesPanel.showContainer("Properties");
        componentsListPanel.changeVisibleState(!state);
        jsObject.InitializeEditChartForm(function (editChartForm) {
            propertiesPanel.setEditMode(state, editChartForm);
            if (!propertiesPanel.editChartPropertiesPanel) {
                propertiesPanel.editChartPropertiesPanel = jsObject.ChartPropertiesPanel();
                propertiesPanel.containers["Properties"].appendChild(propertiesPanel.editChartPropertiesPanel);
            }
            propertiesPanel.editChartPropertiesPanel.style.display = state ? "" : "none";
        });
    }

    propertiesPanel.setEditCrossTabMode = function (state) {
        if (state) { propertiesPanel.returnToPanel = propertiesPanel.getCurrentPanelName(); }
        if (propertiesPanel.eventsMode) propertiesPanel.setEventsMode(false);
        propertiesPanel.editCrossTabMode = state;
        if (state) propertiesPanel.showContainer("Properties");
        componentsListPanel.changeVisibleState(!state);
        var selectedObject = jsObject.options.selectedObject;

        jsObject.InitializeCrossTabForm(function (editCrossTabForm) {
            propertiesPanel.setEditMode(state, editCrossTabForm, selectedObject && selectedObject.typeComponent == "StiCrossField");
            if (!propertiesPanel.editCrossTabPropertiesPanel) {
                propertiesPanel.editCrossTabPropertiesPanel = jsObject.CrossTabPropertiesPanel();
                propertiesPanel.containers["Properties"].appendChild(propertiesPanel.editCrossTabPropertiesPanel);
            }
            propertiesPanel.editCrossTabPropertiesPanel.style.display = state ? "" : "none";
        });
    }

    propertiesPanel.setEditBarCodeMode = function (state) {
        if (state) { propertiesPanel.returnToPanel = propertiesPanel.getCurrentPanelName(); }
        propertiesPanel.editBarCodeMode = state;
        propertiesPanel.showContainer("Properties");
        componentsListPanel.changeVisibleState(!state);
        jsObject.InitializeBarCodeForm(function (barCodeForm) {
            propertiesPanel.setEditMode(state, barCodeForm);
            if (!propertiesPanel.editBarCodePropertiesPanel) {
                propertiesPanel.editBarCodePropertiesPanel = jsObject.BarCodePropertiesPanel();
                propertiesPanel.containers["Properties"].appendChild(propertiesPanel.editBarCodePropertiesPanel);
            }
            propertiesPanel.editBarCodePropertiesPanel.style.display = state ? "" : "none";
        });
    }

    propertiesPanel.setEditDbsMeterMode = function (state) {
        propertiesPanel.editDbsMeterMode = state;
        if (!propertiesPanel.editDbsMeterPropertiesPanel) {
            propertiesPanel.editDbsMeterPropertiesPanel = jsObject.DbsMeterPropertiesPanel();
            propertiesPanel.containers.Properties.appendChild(propertiesPanel.editDbsMeterPropertiesPanel);
        }
        propertiesPanel.editDbsMeterPropertiesPanel.style.display = state ? "" : "none";
        propertiesPanel.mainPropertiesPanel.style.display = state ? "none" : "";
    }

    propertiesPanel.setDictionaryMode = function (state) {
        if (state) {
            propertiesPanel.returnToPanel = propertiesPanel.getCurrentPanelName();
            propertiesPanel.returnToEventsMode = propertiesPanel.eventsMode;
        }
        propertiesPanel.setEnabled(!state);
        propertiesPanel.dictionaryMode = state;
        var buttons = jsObject.options.buttons;
        if (jsObject.options.showDictionary) {
            propertiesPanel.showContainer("Dictionary");
        }
        if (state && buttons.showDictionaryPanelButton) {
            buttons.showDictionaryPanelButton.setSelected(true);
        }
        buttons.PropertiesTabButton.style.display = state ? "none" : "";
        buttons.ReportTreeTabButton.style.display = state ? "none" : "";
        if (buttons.showPropertiesPanelButton) {
            buttons.showPropertiesPanelButton.style.display = state ? "none" : "";
        }
        if (buttons.showReportTreePanelButton) {
            buttons.showReportTreePanelButton.style.display = state ? "none" : "";
        }
        if (!state) {
            if (propertiesPanel.editChartMode) { propertiesPanel.setEditChartMode(true); }
            else if (propertiesPanel.editChartSeriesMode) { propertiesPanel.setEditChartSeriesMode(true); }
            else if (propertiesPanel.editChartStripsMode) { propertiesPanel.setEditChartStripsMode(true); }
            else if (propertiesPanel.editChartConstLinesMode) { propertiesPanel.setEditChartConstLinesMode(true); }
            else if (propertiesPanel.editCrossTabMode) { propertiesPanel.setEditCrossTabMode(true); }
            else if (propertiesPanel.editBarCodeMode) { propertiesPanel.setEditBarCodeMode(true); }
            else if (propertiesPanel.editDbsMeterMode) { propertiesPanel.setEditDbsMeterMode(true); }
            else if (propertiesPanel.returnToPanel) propertiesPanel.showContainer(propertiesPanel.returnToPanel);
            if (propertiesPanel.returnToEventsMode) propertiesPanel.setEventsMode(true);
        }
    }

    propertiesPanel.showContainer = function (containerName) {
        this.currentContainerName = containerName;
        for (var name in this.containers) {
            this.containers[name].style.display = name == containerName ? "" : "none";
            if (jsObject.options.buttons[containerName + "TabButton"]) jsObject.options.buttons[containerName + "TabButton"].setSelected(true);
            jsObject.options.propertiesPanel.headerCaption.innerHTML = jsObject.loc.Panels[containerName == "StyleDesignerProperties" ? "Properties" : containerName];
        }
        if (jsObject.options.buttons["show" + containerName + "PanelButton"] && propertiesPanel.fixedViewMode) {
            jsObject.options.buttons["show" + containerName + "PanelButton"].setSelected(true);
        }
        if (containerName == "ReportTree" && jsObject.options.reportTree) {
            jsObject.options.reportTree.selectedItem = null;
            jsObject.options.reportTree.build(!jsObject.options.reportTree.compsButton.isSelected && jsObject.options.reportTree.evntButton.isSelected);
        }

        var showToolbar = containerName == "Properties" &&
            !propertiesPanel.styleDesignerMode &&
            !propertiesPanel.editChartMode &&
            !propertiesPanel.editCrossTabMode &&
            !propertiesPanel.editBarCodeMode &&
            !propertiesPanel.editChartSeriesMode &&
            !propertiesPanel.editChartStripsMode &&
            !propertiesPanel.editChartConstLinesMode &&
            !propertiesPanel.editDbsMeterMode &&
            !propertiesPanel.dictionaryDataMode

        propertiesPanel.propertiesToolBar.changeVisibleState(showToolbar);

        if (propertiesPanel.dictionaryDataMode) propertiesPanel.setDictionaryDataMode(false);
        if (!showToolbar && propertiesPanel.eventsMode) propertiesPanel.setEventsMode(false);

        if (jsObject.options.dictionaryPanel && containerName == "Dictionary") {
            jsObject.options.dictionaryPanel.checkDataHintItemVisibility();
        }
    }

    propertiesPanel.setZIndex = function (state, level) {
        var zIndex = state ? (level * 10 + 1) : 2;
        propertiesPanel.style.zIndex = zIndex;
        propertiesPanel.showButtonsPanel.style.zIndex = zIndex;
    }

    propertiesPanel.onmousedown = function () {
        if (this.isTouchStartFlag) return;
        this.ontouchstart(true);
    }

    propertiesPanel.ontouchstart = function (mouseProcess) {
        var this_ = this;
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        jsObject.options.propertiesPanelPressed = true;
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    //Prepare places for groups
    var groupNames = ["Dashboard", "Chart", "ChartAdditional", "Data", "Primitive", "Cell", "Hierarchical", "CrossTab", "Check", "ZipCode", "Container", "Shape", "ShapeText", "Gauge", "Map",
        "BarCodeAdditional", "ImageAdditional", "Text", "TextAdditional", "Page", "PageAdditional", "Table", "TableOfContents", "TextElement", "ImageElement", "TableElement", "CardsElement", "PdfSignature",
        "ButtonElement", "RegionMapElement", "OnlineMapElement", "IndicatorElement", "ProgressElement", "GaugeElement", "PivotTableElement", "ComboBoxElement", "DatePickerElement", "ListBoxElement",
        "TreeViewElement", "TreeViewBoxElement", "HeaderTable", "FooterTable", "PageAndColumnBreak", "Columns", "Position", "Appearance", "Behavior", "Design", "Export", "ReportDescription",
        "ReportData", "ReportGlobalization", "ReportEngine", "ReportView"
    ];

    propertiesPanel.places = {};

    for (var i = 0; i < groupNames.length; i++) {
        var place = document.createElement("div");
        propertiesPanel.places[groupNames[i]] = place;
        propertiesPanel.mainPropertiesPanel.appendChild(place);
    }

    propertiesPanel.updateControls = function () {
        if (this.eventsMode) {
            this.eventsPropertiesPanel.update();
        }
        if (this.dictionaryDataMode) {
            this.setDictionaryDataMode(false);
        }
        var controls = jsObject.options.controls;
        var buttons = jsObject.options.buttons;
        var properties = jsObject.options.properties;
        var propertiesGroups = jsObject.options.propertiesGroups;
        var report = jsObject.options.report;
        var currentObject = jsObject.options.selectedObject || jsObject.GetCommonObject(jsObject.options.selectedObjects);
        var currProps = currentObject ? currentObject.properties : null;
        var styleObject = currProps ? jsObject.getStyleObject(currProps.componentStyle) : null;

        var levelDifficulty = 3;
        if (jsObject.options.designerSpecification == "BICreator") levelDifficulty = 2;
        else if (jsObject.options.designerSpecification == "Beginner") levelDifficulty = 1;

        if (propertiesPanel.editChartSeriesMode) {
            propertiesPanel.setEditChartSeriesMode(false);
        }

        if (propertiesPanel.editChartStripsMode) {
            propertiesPanel.setEditChartStripsMode(false);
        }

        if (propertiesPanel.editChartConstLinesMode) {
            propertiesPanel.setEditChartConstLinesMode(false);
        }

        if (propertiesPanel.editDbsMeterMode) {
            propertiesPanel.setEditDbsMeterMode(false);
        }

        designButtonBlock.style.display = "none";

        //Dashboard Group
        var showDashboardGroup = (report && currentObject.isDashboard);
        if (showDashboardGroup && !propertiesGroups.dashboardPropertiesGroup) propertiesPanel.places["Dashboard"].appendChild(jsObject.DashboardPropertiesGroup());
        if (propertiesGroups.dashboardPropertiesGroup) propertiesGroups.dashboardPropertiesGroup.style.display = showDashboardGroup ? "" : "none";
        if (showDashboardGroup) {
            controls.controlPropertyDashboardWidth.value = jsObject.StrToDouble(currProps.unitWidth);
            controls.controlPropertyDashboardHeight.value = jsObject.StrToDouble(currProps.unitHeight);
            controls.controlPropertyDeviceWidth.value = jsObject.StrToInt(currProps.deviceWidth);
            properties.deviceWidth.style.display = currProps.dashboardViewMode == "Mobile" ? "" : "none";
        }

        //Page Group
        var showPageGroup = (report && currentObject.typeComponent == "StiPage" && !currentObject.isDashboard);
        if (showPageGroup && !propertiesGroups.pagePropertiesGroup) propertiesPanel.places["Page"].appendChild(jsObject.PagePropertiesGroup());
        if (propertiesGroups.pagePropertiesGroup) propertiesGroups.pagePropertiesGroup.style.display = showPageGroup ? "" : "none";
        if (showPageGroup) {
            controls.controlPropertyPageSize.setKey(currProps.paperSize);
            controls.controlPropertyPageWidth.value = jsObject.StrToDouble(currProps.unitWidth);
            controls.controlPropertyPageHeight.value = jsObject.StrToDouble(currProps.unitHeight);
            controls.controlPropertyPageOrientation.setKey(currProps.orientation);
            controls.controlPropertyPageMargins.setValue(currProps.unitMargins, true);
            properties.pageNumberOfCopies.style.display = levelDifficulty > 1 ? "" : "none";
            controls.controlPropertyPageNumberOfCopies.value = currProps.numberOfCopies;
        }

        //Page Additional
        var showPageAdditionalGroup = (report && currentObject.typeComponent == "StiPage" && !currentObject.isDashboard && levelDifficulty > 1);
        if (showPageAdditionalGroup && !propertiesGroups.pageAdditionalPropertiesGroup) propertiesPanel.places["PageAdditional"].appendChild(jsObject.PageAdditionalPropertiesGroup());
        if (propertiesGroups.pageAdditionalPropertiesGroup) propertiesGroups.pageAdditionalPropertiesGroup.style.display = showPageAdditionalGroup ? "" : "none";
        if (showPageAdditionalGroup) {
            properties.stopBeforePrint.style.display = levelDifficulty > 1 ? "" : "none";
            controls.controlPropertyStopBeforePrint.value = currProps.stopBeforePrint;
            properties.titleBeforeHeader.style.display = levelDifficulty > 1 ? "" : "none";
            controls.controlPropertyTitleBeforeHeader.setChecked(currProps.titleBeforeHeader);
            properties.mirrorMargins.style.display = levelDifficulty > 1 ? "" : "none";
            controls.controlPropertyMirrorMargins.setChecked(currProps.mirrorMargins);
            properties.unlimitedHeight.style.display = levelDifficulty > 2 ? "" : "none";
            controls.controlPropertyPageUnlimitedHeight.setChecked(currProps.unlimitedHeight);
            properties.unlimitedBreakable.style.display = levelDifficulty > 2 ? "" : "none";
            controls.controlPropertyPageUnlimitedBreakable.setChecked(currProps.unlimitedBreakable);
            properties.segmentPerWidth.style.display = levelDifficulty > 2 ? "" : "none";
            controls.controlPropertySegmentPerWidth.value = currProps.segmentPerWidth;
            properties.segmentPerHeight.style.display = levelDifficulty > 2 ? "" : "none";
            controls.controlPropertySegmentPerHeight.value = currProps.segmentPerHeight;
        }

        //TableOfContents Group
        var showTableOfContentsGroup = (report && currentObject.typeComponent == "StiTableOfContents");
        if (showTableOfContentsGroup && !propertiesGroups.tableOfContentsPropertiesGroup) propertiesPanel.places["TableOfContents"].appendChild(jsObject.TableOfContentsPropertiesGroup());
        if (propertiesGroups.tableOfContentsPropertiesGroup) propertiesGroups.tableOfContentsPropertiesGroup.style.display = showTableOfContentsGroup ? "" : "none";
        if (showTableOfContentsGroup) {
            properties.tableOfContentsIndent.style.display = levelDifficulty > 1 ? "" : "none";
            controls.controlPropertyTableOfContentsIndent.value = jsObject.StrToDouble(currProps.tableOfContentsIndent);
            properties.tableOfContentsMargins.style.display = levelDifficulty > 1 ? "" : "none";
            controls.controlPropertyTableOfContentsMargins.setValue(currProps["tableOfContentsMargins"] == "StiEmptyValue" ? "" : currProps.tableOfContentsMargins);
            controls.controlPropertyTableOfContentsNewPageBefore.setChecked(currProps.tableOfContentsNewPageBefore);
            controls.controlPropertyTableOfContentsNewPageAfter.setChecked(currProps.tableOfContentsNewPageAfter);
            controls.controlPropertyTableOfContentsRightToLeft.setChecked(currProps.tableOfContentsRightToLeft);
            properties.tableOfContentsStyles.propertyControl.setKey(currProps.tableOfContentsStyles);
        }

        //Columns Group
        var showColumns = report && currProps["columns"] != null;
        if (showColumns && !propertiesGroups.columnsPropertiesGroup) propertiesPanel.places["Columns"].appendChild(jsObject.ColumnsPropertiesGroup());
        if (propertiesGroups.columnsPropertiesGroup) propertiesGroups.columnsPropertiesGroup.style.display = showColumns ? "" : "none";
        if (showColumns) {
            properties.columns.style.display = currProps["columns"] != null ? "" : "none";
            if (currProps["columns"] != null) controls.controlPropertyColumns.value = currProps.columns != "StiEmptyValue" ? currProps.columns : "";
            properties.columnWidth.style.display = currProps["columnWidth"] != null ? "" : "none";
            if (currProps["columnWidth"] != null) controls.controlPropertyColumnWidth.value = currProps.columnWidth != "StiEmptyValue" ? jsObject.StrToDouble(currProps.columnWidth) : "";
            properties.columnGaps.style.display = currProps["columnGaps"] != null ? "" : "none";
            if (currProps["columnGaps"] != null) controls.controlPropertyColumnGaps.value = currProps.columnGaps != "StiEmptyValue" ? jsObject.StrToDouble(currProps.columnGaps) : "";
            properties.rightToLeft.style.display = currProps["rightToLeft"] != null ? "" : "none";
            if (currProps["rightToLeft"] != null) controls.controlPropertyRightToLeft.setChecked(currProps.rightToLeft);
            properties.columnDirection.style.display = currProps["columnDirection"] != null ? "" : "none";
            if (currProps["columnDirection"] != null) controls.controlPropertyColumnDirection.setKey(currProps.columnDirection != "StiEmptyValue" ? currProps.columnDirection : "");
            properties.minRowsInColumn.style.display = currProps["minRowsInColumn"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["minRowsInColumn"] != null) controls.controlPropertyMinRowsInColumn.value = currProps.minRowsInColumn != "StiEmptyValue" ? currProps.minRowsInColumn : "";
        }

        //Appearance Group 
        var showAppearance = report && ((currProps["backColor"] != null && (currentObject.isDashboard || currentObject.isDashboardElement)) ||
            currProps["brush"] != null || currProps["border"] != null || currProps["componentStyle"] != null || currProps["elementStyle"] != null);
        if (showAppearance && !propertiesGroups.appearancePropertiesGroup) propertiesPanel.places["Appearance"].appendChild(jsObject.AppearancePropertiesGroup());
        if (propertiesGroups.appearancePropertiesGroup) propertiesGroups.appearancePropertiesGroup.style.display = showAppearance ? "" : "none";
        if (showAppearance) {
            var usedStyle = false;
            if (jsObject.IsTableCell(currentObject)) {
                var table = jsObject.options.currentPage.components[currProps.parentName];
                if (table && (table.properties.componentStyle != "[None]" || table.properties.styleId != "")) {
                    usedStyle = true;
                }
            }

            properties.brush.style.display = currProps["brush"] != null && !styleObject.allowUseBrush && !usedStyle ? "" : "none";
            if (currProps.allowApplyStyle === true || (currentObject.typeComponent == "StiSparkline" && styleObject.name)) properties.brush.style.display = "none";
            if (currProps["brush"] != null) {
                var expression = currProps.expressions && currProps.expressions["brush"] != null ? StiBase64.decode(currProps.expressions["brush"]) : null;
                controls.controlPropertyBrush.showFromStyle = currentObject.typeComponent == "StiButtonElement";
                controls.controlPropertyBrush.setKey(currProps.brush, expression);
            }
            properties.textBrush.style.display = currProps["textBrush"] != null && !styleObject.allowUseTextBrush && !usedStyle && currentObject.typeComponent != "StiShape" ? "" : "none";
            if (currProps["textBrush"] != null) {
                var expression = currProps.expressions && currProps.expressions["textBrush"] != null ? StiBase64.decode(currProps.expressions["textBrush"]) : null;
                controls.controlPropertyTextBrush.showFromStyle = currentObject.typeComponent == "StiButtonElement";
                controls.controlPropertyTextBrush.setKey(currProps.textBrush, expression);
            }
            properties.border.style.display = currProps["border"] != null && !styleObject.allowUseBorderFormatting && !styleObject.allowUseBorderSides && !currProps.allowApplyStyle && !usedStyle ? "" : "none";
            if (currProps["border"] != null) controls.controlPropertyBorder.value = jsObject.BorderObjectToShotStr(jsObject.BordersStrToObject(currProps.border), !propertiesPanel.localizePropertyGrid);
            properties.shapeBorderColor.style.display = currProps["shapeBorderColor"] != null ? "" : "none";
            if (currProps["shapeBorderColor"] != null) {
                var expression = currProps.expressions && currProps.expressions["borderColor"] != null ? StiBase64.decode(currProps.expressions["borderColor"]) : null;
                controls.controlPropertyShapeBorderColor.setKey(currProps.shapeBorderColor, currentObject.isDashboardElement || currentObject.isDashboard, expression);
            }
            properties.checkContourColor.style.display = currProps["contourColor"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["contourColor"] != null) {
                var expression = currProps.expressions && currProps.expressions.contourColor != null ? StiBase64.decode(currProps.expressions.contourColor) : null;
                controls.controlPropertyCheckContourColor.setKey(currProps.contourColor, currentObject.isDashboardElement, expression);
            }
            properties.negativeSeriesColors.style.display = currProps["negativeSeriesColors"] != null ? "" : "none";
            if (currProps["negativeSeriesColors"] != null) controls.controlPropertyNegativeSeriesColors.setKey(currProps.negativeSeriesColors);
            properties.seriesColors.style.display = currProps["seriesColors"] != null ? "" : "none";
            if (currProps["seriesColors"] != null) controls.controlPropertySeriesColors.setKey(currProps.seriesColors);
            properties.paretoSeriesColors.style.display = currProps["paretoSeriesColors"] != null ? "" : "none";
            if (currProps["paretoSeriesColors"] != null) controls.controlPropertyParetoSeriesColors.setKey(currProps.paretoSeriesColors);
            properties.foreColor.style.display = currProps["foreColor"] != null && currentObject.typeComponent != "StiShape" && currentObject.typeComponent != "StiButtonElement" ? "" : "none";
            if (currProps["foreColor"] != null) {
                var expression = currProps.expressions && currProps.expressions["foreColor"] != null ? StiBase64.decode(currProps.expressions["foreColor"]) : null;
                controls.controlPropertyForeColor.setKey(currProps.foreColor, currentObject.isDashboardElement, expression);
            }
            properties.backColor.style.display = currProps["backColor"] != null && currentObject.typeComponent != "StiButtonElement" ? "" : "none";
            if (currProps["backColor"] != null) {
                var expression = currProps.expressions && currProps.expressions["backColor"] != null ? StiBase64.decode(currProps.expressions["backColor"]) : null;
                controls.controlPropertyBackColor.setKey(currProps.backColor, currentObject.isDashboardElement || currentObject.isDashboard, expression);
            }
            properties.iconColor.style.display = currProps["iconColor"] != null ? "" : "none";
            if (currProps["iconColor"] != null) {
                var expression = currProps.expressions && currProps.expressions["iconColor"] != null ? StiBase64.decode(currProps.expressions["iconColor"]) : null;
                controls.controlPropertyIconColor.setKey(currProps.iconColor, currentObject.isDashboardElement, expression);
            }
            properties.iconBrush.style.display = currProps["iconBrush"] != null ? "" : "none";
            if (currProps["iconBrush"] != null) {
                var expression = currProps.expressions && currProps.expressions["iconBrush"] != null ? StiBase64.decode(currProps.expressions["iconBrush"]) : null;
                controls.controlPropertyIconBrush.showFromStyle = currentObject.typeComponent == "StiButtonElement";
                controls.controlPropertyIconBrush.setKey(currProps.iconBrush, expression);
            }
            properties.glyphColor.style.display = currProps["glyphColor"] != null ? "" : "none";
            if (currProps["glyphColor"] != null) controls.controlPropertyGlyphColor.setKey(currProps.glyphColor, currentObject.isDashboardElement);
            properties.font.style.display = currProps["font"] != null && !styleObject.allowUseFont && !currentObject.isDashboardElement ? "" : "none";
            if (currProps["font"] != null && !currentObject.isDashboardElement) properties.font.propertyControl.setKey(currProps["font"]);
            properties.dbsElementFont.style.display = currProps["font"] != null && currentObject.isDashboardElement && currentObject.typeComponent != "StiProgressElement" ? "" : "none";
            if (currProps["font"] != null && currentObject.isDashboardElement && currentObject.typeComponent != "StiProgressElement") properties.dbsElementFont.propertyControl.setKey(currProps["font"]);
            properties.fontSizeMode.style.display = currProps["fontSizeMode"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["fontSizeMode"] != null) controls.controlPropertyFontSizeMode.setKey(currProps.fontSizeMode);
            properties.headerForeColor.style.display = currProps["headerForeColor"] != null ? "" : "none";
            if (currProps["headerForeColor"] != null) controls.controlPropertyHeaderForeColor.setKey(currProps.headerForeColor, currentObject.isDashboardElement);
            properties.headerFont.style.display = currProps["headerFont"] != null ? "" : "none";
            if (currProps["headerFont"] != null) properties.headerFont.propertyControl.setKey(currProps["headerFont"]);
            properties.footerForeColor.style.display = currProps["footerForeColor"] != null ? "" : "none";
            if (currProps["footerForeColor"] != null) controls.controlPropertyFooterForeColor.setKey(currProps.footerForeColor, currentObject.isDashboardElement);
            properties.footerFont.style.display = currProps["footerFont"] != null ? "" : "none";
            if (currProps["footerFont"] != null) properties.footerFont.propertyControl.setKey(currProps["footerFont"]);

            properties.conditions.style.display = currProps["conditions"] != null ? "" : "none";
            if (currProps["conditions"] != null) {
                var conditionsText = "[" + (currProps.conditions != "" && currProps.conditions != "StiEmptyValue"
                    ? (!propertiesPanel.localizePropertyGrid ? "Conditions" : jsObject.loc.PropertyMain.Conditions)
                    : (!propertiesPanel.localizePropertyGrid ? "NoConditions" : jsObject.loc.FormConditions.NoConditions)) + "]";
                controls.controlPropertyConditions.value = conditionsText;
            }
            var showComponentStyle = currProps["componentStyle"] != null && currentObject.typeComponent != "StiChart" && currentObject.typeComponent != "StiGauge" &&
                currentObject.typeComponent != "StiMap" && currentObject.typeComponent != "StiCrossTab";

            properties.chartConditions.style.display = currProps.chartConditions != "StiEmptyValue" && currProps["chartConditions"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps.name != "StiEmptyValue" && currProps["chartConditions"] != null) {
                properties.chartConditions.propertyControl.setKey(currProps.chartConditions);
                properties.chartConditions.propertyControl.valueMeters = currProps.valueMeters;
            }

            properties.indicatorConditions.style.display = currProps.indicatorConditions != "StiEmptyValue" && currProps["indicatorConditions"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps.name != "StiEmptyValue" && currProps["indicatorConditions"] != null) {
                properties.indicatorConditions.propertyControl.setKey(currProps.indicatorConditions);
            }

            properties.progressConditions.style.display = currProps.progressConditions != "StiEmptyValue" && currProps["progressConditions"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps.name != "StiEmptyValue" && currProps["progressConditions"] != null) {
                properties.progressConditions.propertyControl.setKey(currProps.progressConditions);
            }

            properties.tableConditions.style.display = currProps.tableConditions != "StiEmptyValue" && currProps["tableConditions"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps.name != "StiEmptyValue" && currProps["tableConditions"] != null) {
                properties.tableConditions.propertyControl.setKey(currProps.tableConditions);
                properties.tableConditions.propertyControl.valueMeters = currProps.valueMeters;
            }

            properties.pivotTableConditions.style.display = currProps.pivotTableConditions != "StiEmptyValue" && currProps["pivotTableConditions"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps.name != "StiEmptyValue" && currProps["pivotTableConditions"] != null) {
                properties.pivotTableConditions.propertyControl.setKey(currProps.pivotTableConditions);
                properties.pivotTableConditions.propertyControl.valueMeters = currProps.valueMeters;
            }

            properties.componentStyle.style.display = showComponentStyle ? "" : "none";
            if (showComponentStyle) {
                var expression = currProps.expressions && currProps.expressions["componentStyle"] != null ? StiBase64.decode(currProps.expressions["componentStyle"]) : null;
                controls.controlPropertyComponentStyle.menu.addItems(jsObject.GetComponentStyleItems());
                controls.controlPropertyComponentStyle.setKey(currProps.componentStyle, expression);
            }
            properties.elementStyle.style.display = currProps["elementStyle"] != null ? "" : "none";
            if (currProps["elementStyle"] != null) {
                controls.controlPropertyElementStyle.setKey(currProps["elementStyle"] != "Custom" ? currProps["elementStyle"] : (currProps["customStyleName"] || ""));

                if (controls.controlPropertyElementStyle.menu) {
                    controls.controlPropertyElementStyle.menu.onshow = function () {
                        for (var itemName in this.items) {
                            var item = this.items[itemName];
                            if (item.key &&
                                ((item.key.ident != "Custom" && item.key.ident == currProps["elementStyle"]) ||
                                    (item.key.ident == "Custom" && item.key.name == currProps["customStyleName"]))) {
                                item.setSelected(true);
                                break;
                            }
                        }
                    };
                }
            }
            properties.oddStyle.style.display = currProps["oddStyle"] != null ? "" : "none";
            if (currProps["oddStyle"] != null) {
                var expression = currProps.expressions && currProps.expressions["oddStyle"] != null ? StiBase64.decode(currProps.expressions["oddStyle"]) : null;
                controls.controlPropertyOddStyle.menu.addItems(jsObject.GetComponentStyleItems(false, true));
                controls.controlPropertyOddStyle.setKey(currProps.oddStyle, expression);
            }
            properties.evenStyle.style.display = currProps["evenStyle"] != null ? "" : "none";
            if (currProps["evenStyle"] != null) {
                var expression = currProps.expressions && currProps.expressions["evenStyle"] != null ? StiBase64.decode(currProps.expressions["evenStyle"]) : null;
                controls.controlPropertyEvenStyle.menu.addItems(jsObject.GetComponentStyleItems(false, true));
                controls.controlPropertyEvenStyle.setKey(currProps.evenStyle, expression);
            }
            properties.useParentStyles.style.display = currProps["useParentStyles"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["useParentStyles"] != null) controls.controlPropertyUseParentStyles.setChecked(currProps.useParentStyles);

            properties.cornerRadius.style.display = currProps["cornerRadius"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["cornerRadius"] != null) controls.controlPropertyCornerRadius.setValue(currProps.cornerRadius == "StiEmptyValue" ? "" : currProps.cornerRadius);

            properties.dashboardWatermark.style.display = currProps.dashboardWatermark != "StiEmptyValue" && currProps.dashboardWatermark != null && levelDifficulty > 1 ? "" : "none";
            if (currProps.dashboardWatermark != "StiEmptyValue" && currProps.dashboardWatermark != null) properties.dashboardWatermark.propertyControl.setKey(currProps.dashboardWatermark);

            //Shadow Group
            var showShadow = report && currProps["shadowVisible"] != null && currProps["shadowColor"] != null && levelDifficulty > 1;
            if (propertiesGroups.shadowPropertiesGroup) propertiesGroups.shadowPropertiesGroup.style.display = showShadow ? "" : "none";
            if (showShadow) {
                properties.shadowVisible.style.display = currProps["shadowVisible"] != null ? "" : "none";
                if (currProps["shadowVisible"] != null) controls.controlPropertyShadowVisible.setChecked(currProps.shadowVisible);
                properties.shadowColor.style.display = currProps["shadowColor"] != null ? "" : "none";
                if (currProps["shadowColor"] != null) controls.controlPropertyShadowColor.setKey(currProps.shadowColor);
                properties.shadowLocation.style.display = currProps["shadowLocation"] != null ? "" : "none";
                if (currProps["shadowLocation"] != null) controls.controlPropertyShadowLocation.setValue(currProps.shadowLocation);
                properties.shadowSize.style.display = currProps["shadowSize"] != null ? "" : "none";
                if (currProps["shadowSize"] != null) controls.controlPropertyShadowSize.value = currProps.shadowSize;
            }

            //Visual States Group
            var showVisualStates = report && currentObject.typeComponent == "StiButtonElement" && currProps.buttonVisualStates != null && levelDifficulty > 1;
            if (propertiesGroups.visualStatesPropertiesGroup) propertiesGroups.visualStatesPropertiesGroup.style.display = showVisualStates ? "" : "none";

            if (showVisualStates) {
                var checkedProps = currProps.buttonVisualStates.check;
                var hoverProps = currProps.buttonVisualStates.hover;
                var pressedProps = currProps.buttonVisualStates.pressed;

                if (checkedProps && hoverProps && pressedProps) {
                    properties.visualStatesCheckBorder.style.display = checkedProps.border != null ? "" : "none";
                    if (checkedProps.border != null) properties.visualStatesCheckBorder.propertyControl.setKey(checkedProps.border);
                    properties.visualStatesCheckBrush.style.display = checkedProps.brush != null ? "" : "none";
                    if (checkedProps.brush != null) properties.visualStatesCheckBrush.propertyControl.setKey(checkedProps.brush);
                    properties.visualStatesCheckIconBrush.style.display = checkedProps.iconBrush != null ? "" : "none";
                    if (checkedProps.iconBrush != null) properties.visualStatesCheckIconBrush.propertyControl.setKey(checkedProps.iconBrush);
                    properties.visualStatesCheckTextBrush.style.display = checkedProps.textBrush != null ? "" : "none";
                    if (checkedProps.textBrush != null) properties.visualStatesCheckTextBrush.propertyControl.setKey(checkedProps.textBrush);
                    properties.visualStatesCheckFont.style.display = checkedProps.font != null ? "" : "none";
                    if (checkedProps.font != null) properties.visualStatesCheckFont.propertyControl.setKey(checkedProps.font);

                    if (checkedProps.iconSet != null) {
                        properties.visualStatesCheckIconSetIcon.propertyControl.setKey(checkedProps.iconSet.icon);
                        properties.visualStatesCheckIconSetCheckedIcon.propertyControl.setKey(checkedProps.iconSet.checkedIcon);
                        properties.visualStatesCheckIconSetUncheckedIcon.propertyControl.setKey(checkedProps.iconSet.uncheckedIcon);
                    }

                    properties.visualStatesHoverBorder.style.display = hoverProps.border != null ? "" : "none";
                    if (hoverProps.border != null) properties.visualStatesHoverBorder.propertyControl.setKey(hoverProps.border);
                    properties.visualStatesHoverBrush.style.display = hoverProps.brush != null ? "" : "none";
                    if (hoverProps.brush != null) properties.visualStatesHoverBrush.propertyControl.setKey(hoverProps.brush);
                    properties.visualStatesHoverIconBrush.style.display = hoverProps.iconBrush != null ? "" : "none";
                    if (hoverProps.iconBrush != null) properties.visualStatesHoverIconBrush.propertyControl.setKey(hoverProps.iconBrush);
                    properties.visualStatesHoverTextBrush.style.display = hoverProps.textBrush != null ? "" : "none";
                    if (hoverProps.textBrush != null) properties.visualStatesHoverTextBrush.propertyControl.setKey(hoverProps.textBrush);
                    properties.visualStatesHoverFont.style.display = hoverProps.font != null ? "" : "none";
                    if (hoverProps.font != null) properties.visualStatesHoverFont.propertyControl.setKey(hoverProps.font);

                    if (hoverProps.iconSet != null) {
                        properties.visualStatesHoverIconSetIcon.propertyControl.setKey(hoverProps.iconSet.icon);
                        properties.visualStatesHoverIconSetCheckedIcon.propertyControl.setKey(hoverProps.iconSet.checkedIcon);
                        properties.visualStatesHoverIconSetUncheckedIcon.propertyControl.setKey(hoverProps.iconSet.uncheckedIcon);
                    }

                    properties.visualStatesPressedBorder.style.display = pressedProps.border != null ? "" : "none";
                    if (pressedProps.border != null) properties.visualStatesPressedBorder.propertyControl.setKey(pressedProps.border);
                    properties.visualStatesPressedBrush.style.display = pressedProps.brush != null ? "" : "none";
                    if (pressedProps.brush != null) properties.visualStatesPressedBrush.propertyControl.setKey(pressedProps.brush);
                    properties.visualStatesPressedIconBrush.style.display = pressedProps.iconBrush != null ? "" : "none";
                    if (pressedProps.iconBrush != null) properties.visualStatesPressedIconBrush.propertyControl.setKey(pressedProps.iconBrush);
                    properties.visualStatesPressedTextBrush.style.display = pressedProps.textBrush != null ? "" : "none";
                    if (pressedProps.textBrush != null) properties.visualStatesPressedTextBrush.propertyControl.setKey(pressedProps.textBrush);
                    properties.visualStatesPressedFont.style.display = pressedProps.font != null ? "" : "none";
                    if (pressedProps.font != null) properties.visualStatesPressedFont.propertyControl.setKey(pressedProps.font);

                    if (pressedProps.iconSet != null) {
                        properties.visualStatesPressedIconSetIcon.propertyControl.setKey(pressedProps.iconSet.icon);
                        properties.visualStatesPressedIconSetCheckedIcon.propertyControl.setKey(pressedProps.iconSet.checkedIcon);
                        properties.visualStatesPressedIconSetUncheckedIcon.propertyControl.setKey(pressedProps.iconSet.uncheckedIcon);
                    }
                }
            }
        }

        //Behavior Group
        var showBehavior = report && currProps["enabled"] != null;
        if (showBehavior && !propertiesGroups.behaviorPropertiesGroup) propertiesPanel.places["Behavior"].appendChild(jsObject.BehaviorPropertiesGroup());
        if (propertiesGroups.behaviorPropertiesGroup) propertiesGroups.behaviorPropertiesGroup.style.display = showBehavior ? "" : "none";
        if (showBehavior) {
            controls.propertiesInteractionButtonBlock.style.display = currProps["interaction"] != null && levelDifficulty > 1 ? "" : "none";
            properties.anchor.style.display = currProps["anchor"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["anchor"] != null) controls.controlPropertyAnchor.setKey(currProps.anchor);
            properties.autoWidth.style.display = currProps["autoWidth"] != null ? "" : "none";
            if (currProps["autoWidth"] != null) controls.controlPropertyAutoWidth.setChecked(currProps.autoWidth);
            properties.calcInvisible.style.display = currProps["calcInvisible"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["calcInvisible"] != null) controls.controlPropertyCalcInvisible.setChecked(currProps.calcInvisible);
            properties.canGrow.style.display = currProps["canGrow"] != null ? "" : "none";
            if (currProps["canGrow"] != null) controls.controlPropertyCanGrow.setChecked(currProps.canGrow);
            properties.canShrink.style.display = currProps["canShrink"] != null ? "" : "none";
            if (currProps["canShrink"] != null) controls.controlPropertyCanShrink.setChecked(currProps.canShrink);
            properties.canBreak.style.display = currProps["canBreak"] != null ? "" : "none";
            if (currProps["canBreak"] != null) controls.controlPropertyCanBreak.setChecked(currProps.canBreak);
            properties.growToHeight.style.display = currProps["growToHeight"] != null ? "" : "none";
            if (currProps["growToHeight"] != null) controls.controlPropertyGrowToHeight.setChecked(currProps.growToHeight);
            properties.dockStyle.style.display = currProps["dockStyle"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["dockStyle"] != null) {
                var expression = currProps.expressions && currProps.expressions["dockStyle"] != null ? StiBase64.decode(currProps.expressions["dockStyle"]) : null;
                controls.controlPropertyDockStyle.setKey(currProps.dockStyle, expression);
            }
            properties.enabled.style.display = currProps["enabled"] != null ? "" : "none";
            if (currProps["enabled"] != null) {
                var expression = currProps.expressions && currProps.expressions["enabled"] != null ? StiBase64.decode(currProps.expressions["enabled"]) : null;
                controls.controlPropertyEnabled.setKey(expression != null ? expression : (currProps["enabled"] ? "True" : "False"));
            }
            var showIconAlign = !currProps["isSeriesPresent"] && currProps["iconAlignment"] != null;
            properties.iconAlignment.style.display = showIconAlign ? "" : "none";
            if (showIconAlign) controls.controlPropertyIndicatorElementIconAlignment.setKey(currProps.iconAlignment);
            properties.printOn.style.display = currProps["printOn"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["printOn"] != null) controls.controlPropertyPrintOn.setKey(currProps.printOn);
            properties.printable.style.display = currProps["printable"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["printable"] != null) {
                var expression = currProps.expressions && currProps.expressions["printable"] != null ? StiBase64.decode(currProps.expressions["printable"]) : null;
                controls.controlPropertyPrintable.setKey(expression != null ? expression : (currProps["printable"] ? "True" : "False"));
            }
            properties.printIfEmpty.style.display = currProps["printIfEmpty"] != null ? "" && levelDifficulty > 1 : "none";
            if (currProps["printIfEmpty"] != null) controls.controlPropertyPrintIfEmpty.setChecked(currProps.printIfEmpty);
            properties.printOnAllPages.style.display = currProps["printOnAllPages"] != null ? "" && levelDifficulty > 1 : "none";
            if (currProps["printOnAllPages"] != null) controls.controlPropertyPrintOnAllPages.setChecked(currProps.printOnAllPages);
            properties.printOnEvenOddPages.style.display = currProps["printOnEvenOddPages"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["printOnEvenOddPages"] != null) controls.controlPropertyPrintOnEvenOddPages.setKey(currProps.printOnEvenOddPages);
            properties.resetPageNumber.style.display = currProps["resetPageNumber"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["resetPageNumber"] != null) controls.controlPropertyResetPageNumber.setChecked(currProps.resetPageNumber);
            properties.printOnPreviousPage.style.display = currProps["printOnPreviousPage"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["printOnPreviousPage"] != null) controls.controlPropertyPrintOnPreviousPage.setChecked(currProps.printOnPreviousPage);
            properties.printHeadersFootersFromPreviousPage.style.display = currProps["printHeadersFootersFromPreviousPage"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["printHeadersFootersFromPreviousPage"] != null) controls.controlPropertyPrintHeadersFootersFromPreviousPage.setChecked(currProps.printHeadersFootersFromPreviousPage);
            properties.printAtBottom.style.display = currProps["printAtBottom"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["printAtBottom"] != null) controls.controlPropertyPrintAtBottom.setChecked(currProps.printAtBottom);
            properties.printIfDetailEmpty.style.display = currProps["printIfDetailEmpty"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["printIfDetailEmpty"] != null) controls.controlPropertyPrintIfDetailEmpty.setChecked(currProps.printIfDetailEmpty);
            properties.keepGroupTogether.style.display = currProps["keepGroupTogether"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["keepGroupTogether"] != null) controls.controlPropertyKeepGroupTogether.setChecked(currProps.keepGroupTogether);
            properties.keepGroupHeaderTogether.style.display = currProps["keepGroupHeaderTogether"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["keepGroupHeaderTogether"] != null) controls.controlPropertyKeepGroupHeaderTogether.setChecked(currProps.keepGroupHeaderTogether);
            properties.keepGroupFooterTogether.style.display = currProps["keepGroupFooterTogether"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["keepGroupFooterTogether"] != null) controls.controlPropertyKeepGroupFooterTogether.setChecked(currProps.keepGroupFooterTogether);
            properties.keepHeaderTogether.style.display = currProps["keepHeaderTogether"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["keepHeaderTogether"] != null) controls.controlPropertyKeepHeaderTogether.setChecked(currProps.keepHeaderTogether);
            properties.keepFooterTogether.style.display = currProps["keepFooterTogether"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["keepFooterTogether"] != null) controls.controlPropertyKeepFooterTogether.setChecked(currProps.keepFooterTogether);
            properties.keepDetails.style.display = currProps["keepDetails"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["keepDetails"] != null) controls.controlPropertyKeepDetails.setKey(currProps.keepDetails);
            properties.keepDetailsTogether.style.display = currProps["keepDetailsTogether"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["keepDetailsTogether"] != null) controls.controlPropertyKeepDetailsTogether.setChecked(currProps.keepDetailsTogether);
            properties.keepReportSummaryTogether.style.display = currProps["keepReportSummaryTogether"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["keepReportSummaryTogether"] != null) controls.controlPropertyKeepReportSummaryTogether.setChecked(currProps.keepReportSummaryTogether);
            properties.keepSubReportTogether.style.display = currProps["keepSubReportTogether"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["keepSubReportTogether"] != null) controls.controlPropertyKeepSubReportTogether.setChecked(currProps.keepSubReportTogether);
            properties.keepCrossTabTogether.style.display = currProps["keepCrossTabTogether"] != null ? "" : "none";
            if (currProps["keepCrossTabTogether"] != null) controls.controlPropertyKeepCrossTabTogether.setChecked(currProps.keepCrossTabTogether);
            properties.shiftMode.style.display = currProps["shiftMode"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["shiftMode"] != null) controls.controlPropertyShiftMode.setKey(currProps.shiftMode);
            properties.margin.style.display = currProps["margin"] != null ? "" : "none";
            if (currProps["margin"] != null) controls.controlPropertyMargin.setValue(currProps.margin == "StiEmptyValue" ? "" : currProps.margin);
            properties.padding.style.display = currProps["padding"] != null ? "" : "none";
            if (currProps["padding"] != null) controls.controlPropertyPadding.setValue(currProps.padding == "StiEmptyValue" ? "" : currProps.padding);
            properties.sizeMode.style.display = currProps["sizeMode"] != null && currentObject.typeComponent == "StiEmptyBand" ? "" : "none";
            if (currProps["sizeMode"] != null) controls.controlPropertySizeMode.setKey(currProps.sizeMode);
            properties.dashboardInteraction.style.display = currProps.name != "StiEmptyValue" && currProps["dashboardInteraction"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps.name != "StiEmptyValue" && currProps["dashboardInteraction"] != null) properties.dashboardInteraction.propertyControl.setKey(currProps.dashboardInteraction);
            properties.textFormatDbsElement.style.display = currProps["textFormat"] != null && currentObject.isDashboardElement && levelDifficulty > 1 ? "" : "none";
            if (currProps["textFormat"] != null) controls.controlPropertyTextFormatDbsElement.value = jsObject.GetTextFormatLocalizedName(currProps.textFormat.type, !propertiesPanel.localizePropertyGrid);
            properties.argumentFormat.style.display = currProps["argumentFormat"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["argumentFormat"] != null) controls.chartElementArgumentFormat.value = jsObject.GetTextFormatLocalizedName(currProps.argumentFormat.type, !propertiesPanel.localizePropertyGrid);
            properties.valueFormat.style.display = currProps["valueFormat"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["valueFormat"] != null) controls.chartElementValueFormat.value = jsObject.GetTextFormatLocalizedName(currProps.valueFormat.type, !propertiesPanel.localizePropertyGrid);
            properties.dataVerticalAlignment.style.display = currProps["vertAlignment"] != null && currentObject.typeComponent == "StiOverlayBand" ? "" : "none";
            if (currProps["vertAlignment"] != null && currentObject.typeComponent == "StiOverlayBand") controls.controlPropertyDataVerticalAlignment.setKey(currProps.vertAlignment);

            //Title Group
            var showTitle = report && currProps["titleText"] != null;
            if (propertiesGroups.titlePropertiesGroup) propertiesGroups.titlePropertiesGroup.style.display = showTitle ? "" : "none";
            if (showTitle) {
                properties.titleText.style.display = currProps["titleText"] != null ? "" : "none";
                if (currProps["titleText"] != null) controls.controlPropertyTitleText.value = currProps.titleText != "StiEmptyValue" ? StiBase64.decode(currProps.titleText) : "";
                properties.titleBackColor.style.display = currProps["titleBackColor"] != null ? "" : "none";
                if (currProps["titleBackColor"] != null) controls.controlPropertyTitleBackColor.setKey(currProps.titleBackColor, currentObject.isDashboardElement);
                properties.titleForeColor.style.display = currProps["titleForeColor"] != null ? "" : "none";
                if (currProps["titleForeColor"] != null) controls.controlPropertyTitleForeColor.setKey(currProps.titleForeColor, currentObject.isDashboardElement);
                if (currProps["titleFont"] != null) properties.titleFont.propertyControl.setKey(currProps.titleFont);
                properties.titleHorAlignment.style.display = currProps["titleHorAlignment"] != null ? "" : "none";
                if (currProps["titleHorAlignment"] != null) controls.controlPropertyTitleHorAlignment.setKey(currProps.titleHorAlignment);
                properties.titleVisible.style.display = currProps["titleVisible"] != null ? "" : "none";
                if (currProps["titleVisible"] != null) controls.controlPropertyTitleVisible.setChecked(currProps.titleVisible);
                properties.titleSizeMode.style.display = currProps["titleSizeMode"] != null ? "" : "none";
                if (currProps["titleSizeMode"] != null) controls.controlPropertyTitleSizeMode.setKey(currProps.titleSizeMode);
            }
        }

        //Position Group
        var showPosition = (report && currentObject.typeComponent != "StiPage" && currentObject.typeComponent != "StiReport" &&
            currentObject.typeComponent != "StiTable" && !jsObject.IsTableCell(currentObject));
        if (jsObject.options.selectedObjects) {
            for (var i = 0; i < jsObject.options.selectedObjects.length; i++)
                if (jsObject.IsTableCell(jsObject.options.selectedObjects[i])) showPosition = false;
        }
        if (showPosition && !propertiesGroups.positionPropertiesGroup) propertiesPanel.places["Position"].appendChild(jsObject.PositionPropertiesGroup());
        if (propertiesGroups.positionPropertiesGroup) propertiesGroups.positionPropertiesGroup.style.display = showPosition ? "" : "none";
        if (showPosition) {
            var positionArray = jsObject.options.selectedObjects
                ? jsObject.GetCommonPositionsArray(jsObject.options.selectedObjects)
                : (ComponentCollection[currentObject.typeComponent] ? ComponentCollection[currentObject.typeComponent][5].split(",") : ["0", "0", "0", "0"]);
            properties.left.style.display = positionArray[0] == "1" ? "" : "none";
            var leftValue = currProps.clientLeft || currProps.unitLeft;
            if (leftValue == "StiEmptyValue") leftValue = "";
            controls.controlPropertyLeft.value = leftValue;
            properties.top.style.display = positionArray[1] == "1" ? "" : "none";
            var topValue = currProps.clientTop || currProps.unitTop;
            if (topValue == "StiEmptyValue") topValue = "";
            controls.controlPropertyTop.value = topValue;
            properties.width.style.display = positionArray[2] == "1" ? "" : "none";
            controls.controlPropertyWidth.value = currProps.unitWidth != "StiEmptyValue" ? currProps.unitWidth : "";
            properties.height.style.display = positionArray[3] == "1" ? "" : "none";
            controls.controlPropertyHeight.value = currProps.unitHeight != "StiEmptyValue" ? currProps.unitHeight : "";
            properties.minSize.style.display = currProps["minSize"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["minSize"] != null) controls.controlPropertyMinSize.setValue(currProps["minSize"] == "StiEmptyValue" ? "" : currProps.minSize);
            properties.maxSize.style.display = currProps["maxSize"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["maxSize"] != null) controls.controlPropertyMaxSize.setValue(currProps["maxSize"] == "StiEmptyValue" ? "" : currProps.maxSize);
            properties.minHeight.style.display = currProps["minHeight"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["minHeight"] != null) controls.controlPropertyMinHeight.value = currProps["minHeight"] == "StiEmptyValue" ? "" : currProps.minHeight;
            properties.maxHeight.style.display = currProps["maxHeight"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["maxHeight"] != null) controls.controlPropertyMaxHeight.value = currProps["maxHeight"] == "StiEmptyValue" ? "" : currProps.maxHeight;
            properties.minWidth.style.display = currProps["minWidth"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["minWidth"] != null) controls.controlPropertyMinWidth.value = currProps["minWidth"] == "StiEmptyValue" ? "" : currProps.minWidth;
            properties.maxWidth.style.display = currProps["maxWidth"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["maxWidth"] != null) controls.controlPropertyMaxWidth.value = currProps["maxWidth"] == "StiEmptyValue" ? "" : currProps.maxWidth;
        }

        //Page And Column Break Group
        var showPageAndColumnBreak = report && (currProps["newPageBefore"] != null || currProps["newPageAfter"] != null ||
            currProps["newColumnBefore"] != null || currProps["newColumnAfter"] != null || currProps["skipFirst"] != null);
        if (showPageAndColumnBreak && !propertiesGroups.pageAndColumnBreakPropertiesGroup) propertiesPanel.places["PageAndColumnBreak"].appendChild(jsObject.PageAndColumnBreakPropertiesGroup());
        if (propertiesGroups.pageAndColumnBreakPropertiesGroup) propertiesGroups.pageAndColumnBreakPropertiesGroup.style.display = showPageAndColumnBreak ? "" : "none";
        if (showPageAndColumnBreak) {
            properties.newPageBefore.style.display = currProps["newPageBefore"] != null ? "" : "none";
            if (currProps["newPageBefore"] != null) controls.controlPropertyNewPageBefore.setChecked(currProps.newPageBefore);
            properties.newPageAfter.style.display = currProps["newPageAfter"] != null ? "" : "none";
            if (currProps["newPageAfter"] != null) controls.controlPropertyNewPageAfter.setChecked(currProps.newPageAfter);
            properties.newColumnBefore.style.display = currProps["newColumnBefore"] != null ? "" : "none";
            if (currProps["newColumnBefore"] != null) controls.controlPropertyNewColumnBefore.setChecked(currProps.newColumnBefore);
            properties.newColumnAfter.style.display = currProps["newColumnAfter"] != null ? "" : "none";
            if (currProps["newColumnAfter"] != null) controls.controlPropertyNewColumnAfter.setChecked(currProps.newColumnAfter);
            properties.skipFirst.style.display = currProps["skipFirst"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["skipFirst"] != null) controls.controlPropertySkipFirst.setChecked(currProps.skipFirst);
            properties.breakIfLessThan.style.display = currProps["breakIfLessThan"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["breakIfLessThan"] != null) controls.controlPropertyBreakIfLessThan.value = currProps.breakIfLessThan;
            properties.limitRows.style.display = currProps["limitRows"] != null && levelDifficulty > 2 ? "" : "none";
            if (currProps["limitRows"] != null) controls.controlPropertyLimitRows.value = currProps.limitRows != "StiEmptyValue" ? StiBase64.decode(currProps.limitRows) : "";
        }

        //CellGroup
        var showCellGroup = report && jsObject.IsTableCell(currentObject);
        if (showCellGroup && !propertiesGroups.cellPropertiesGroup) propertiesPanel.places["Cell"].appendChild(jsObject.CellPropertiesGroup());
        if (propertiesGroups.cellPropertiesGroup) propertiesGroups.cellPropertiesGroup.style.display = showCellGroup ? "" : "none";
        if (showCellGroup) {
            properties.cellType.style.display = currProps["cellType"] != null ? "" : "none";
            if (currProps["cellType"] != null) controls.controlPropertyCellType.setKey(currProps.cellType);
            properties.cellDockStyle.style.display = currProps["cellDockStyle"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["cellDockStyle"] != null) controls.controlPropertyCellDockStyle.setKey(currProps.cellDockStyle);
            properties.fixedWidth.style.display = currProps["fixedWidth"] != null ? "" : "none";
            if (currProps["fixedWidth"] != null) controls.controlPropertyFixedWidth.setChecked(currProps.fixedWidth);
        }

        //Rich Text
        var showRichText = report && currentObject.typeComponent && (currentObject.typeComponent == "StiRichText" || currentObject.typeComponent == "StiTableCellRichText");
        if (showRichText) designButtonBlock.style.display = "";

        //Text Group
        var showText = report && currentObject.typeComponent && (currentObject.typeComponent == "StiText" || currentObject.typeComponent == "StiTextInCells" ||
            currentObject.typeComponent == "StiTableCell");

        if (showText && !propertiesGroups.textPropertiesGroup) propertiesPanel.places["Text"].appendChild(jsObject.TextPropertiesGroup());
        if (propertiesGroups.textPropertiesGroup) propertiesGroups.textPropertiesGroup.style.display = showText ? "" : "none";
        if (showText) {
            designButtonBlock.style.display = "";
            properties.text.style.display = currProps["text"] != null ? "" : "none";
            if (currProps["text"] != null) controls.controlPropertyText.value = currProps.text != "StiEmptyValue" ? StiBase64.decode(currProps.text) : "";
            properties.textHorizontalAlignment.style.display = currProps["horAlignment"] != null && !styleObject.allowUseHorAlignment ? "" : "none";
            if (currProps["horAlignment"] != null) {
                var expression = currProps.expressions && currProps.expressions["horAlignment"] != null ? StiBase64.decode(currProps.expressions["horAlignment"]) : null;
                controls.controlPropertyTextHorizontalAlignment.setKey(currProps.horAlignment, expression);
            }
            properties.textVerticalAlignment.style.display = currProps["vertAlignment"] != null && !styleObject.allowUseVertAlignment ? "" : "none";
            if (currProps["vertAlignment"] != null) {
                var expression = currProps.expressions && currProps.expressions["vertAlignment"] != null ? StiBase64.decode(currProps.expressions["vertAlignment"]) : null;
                controls.controlPropertyTextVerticalAlignment.setKey(currProps.vertAlignment, expression);
            }
            properties.textFormat.style.display = currProps["textFormat"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["textFormat"] != null) controls.controlPropertyTextFormat.value = jsObject.GetTextFormatLocalizedName(currProps.textFormat.type, !propertiesPanel.localizePropertyGrid);
            properties.cellWidth.style.display = currProps["cellWidth"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["cellWidth"] != null) controls.controlPropertyCellWidth.value = currProps.cellWidth != "StiEmptyValue" ? jsObject.StrToDouble(currProps.cellWidth) : "";
            properties.cellHeight.style.display = currProps["cellHeight"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["cellHeight"] != null) controls.controlPropertyCellHeight.value = currProps.cellHeight != "StiEmptyValue" ? jsObject.StrToDouble(currProps.cellHeight) : "";
            properties.horizontalSpacing.style.display = currProps["horizontalSpacing"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["horizontalSpacing"] != null)
                controls.controlPropertyHorizontalSpacing.value = currProps.horizontalSpacing != "StiEmptyValue" ? jsObject.StrToDouble(currProps.horizontalSpacing) : "";
            properties.verticalSpacing.style.display = currProps["verticalSpacing"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["verticalSpacing"] != null)
                controls.controlPropertyVerticalSpacing.value = currProps.verticalSpacing != "StiEmptyValue" ? jsObject.StrToDouble(currProps.verticalSpacing) : "";
        }

        //Text Additional Group
        var showAdditionalText = report && (showText || currentObject.typeComponent == "StiRichText" || currentObject.typeComponent == "StiTableCellRichText");
        if (showAdditionalText && !propertiesGroups.textAdditionalPropertiesGroup)
            propertiesPanel.places["TextAdditional"].appendChild(jsObject.TextAdditionalPropertiesGroup());
        if (propertiesGroups.textAdditionalPropertiesGroup) propertiesGroups.textAdditionalPropertiesGroup.style.display = showAdditionalText ? "" : "none";

        if (showAdditionalText) {
            properties.textAngle.style.display = currProps["textAngle"] != null && levelDifficulty > 2 ? "" : "none";
            if (currProps["textAngle"] != null) controls.controlPropertyTextAngle.value = currProps.textAngle != "StiEmptyValue" ? jsObject.StrToDouble(currProps.textAngle) : "";
            properties.textMargins.style.display = currProps["textMargins"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["textMargins"] != null) controls.controlPropertyTextMargins.setValue(currProps["textMargins"] == "StiEmptyValue" ? "" : currProps.textMargins);
            properties.wordWrap.style.display = currProps["wordWrap"] != null ? "" : "none";
            if (currProps["wordWrap"] != null) controls.controlPropertyWordWrap.setChecked(currProps.wordWrap);
            properties.editableText.style.display = currProps["editableText"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["editableText"] != null) controls.controlPropertyEditableText.setChecked(currProps.editableText);
            properties.hideZeros.style.display = currProps["hideZeros"] != null ? "" : "none";
            if (currProps["hideZeros"] != null) controls.controlPropertyHideZeros.setChecked(currProps.hideZeros);
            properties.lineSpacing.style.display = currProps["lineSpacing"] != null ? "" : "none";
            if (currProps["lineSpacing"] != null) controls.controlPropertyLineSpacing.value = currProps.lineSpacing != "StiEmptyValue" ? jsObject.StrToDouble(currProps.lineSpacing) : "";
            properties.onlyText.style.display = currProps["onlyText"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["onlyText"] != null) controls.controlPropertyOnlyText.setChecked(currProps.onlyText);
            properties.continuousText.style.display = currProps["continuousText"] != null ? "" : "none";
            if (currProps["continuousText"] != null) controls.controlPropertyContinuousText.setChecked(currProps.continuousText);
            properties.maxNumberOfLines.style.display = currProps["maxNumberOfLines"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["maxNumberOfLines"] != null) controls.controlPropertyMaxNumberOfLines.value = currProps.maxNumberOfLines != "StiEmptyValue" ? currProps.maxNumberOfLines : "";
            properties.allowHtmlTags.style.display = currProps["allowHtmlTags"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["allowHtmlTags"] != null) controls.controlPropertyAllowHtmlTags.setChecked(currProps.allowHtmlTags);
            properties.rightToLeftText.style.display = currProps["rightToLeft"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["rightToLeft"] != null) controls.controlPropertyRightToLeftText.setChecked(currProps.rightToLeft);
            properties.trimming.style.display = currProps["trimming"] != null ? "" : "none";
            if (currProps["trimming"] != null) controls.controlPropertyTrimming.setKey(currProps.trimming);
            properties.textOptionsRightToLeft.style.display = currProps["textOptionsRightToLeft"] != null ? "" : "none";
            if (currProps["textOptionsRightToLeft"] != null) controls.controlPropertyTextOptionsRightToLeft.setChecked(currProps.textOptionsRightToLeft);
            properties.processAt.style.display = currProps["processAt"] != null && levelDifficulty > 2 ? "" : "none";
            if (currProps["processAt"] != null) controls.controlPropertyProcessAt.setKey(currProps.processAt);
            properties.processingDuplicates.style.display = currProps["processingDuplicates"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["processingDuplicates"] != null) controls.controlPropertyProcessingDuplicates.setKey(currProps.processingDuplicates);
            properties.shrinkFontToFit.style.display = currProps["shrinkFontToFit"] != null && levelDifficulty > 2 ? "" : "none";
            if (currProps["shrinkFontToFit"] != null) controls.controlPropertyShrinkFontToFit.setChecked(currProps.shrinkFontToFit);
            properties.detectUrls.style.display = currProps["detectUrls"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["detectUrls"] != null) controls.controlPropertyDetectUrls.setChecked(currProps.detectUrls);
            properties.shrinkFontToFitMinimumSize.style.display = currProps["shrinkFontToFitMinimumSize"] != null && levelDifficulty > 2 ? "" : "none";
            if (currProps["shrinkFontToFitMinimumSize"] != null) controls.controlPropertyShrinkFontToFitMinimumSize.value = currProps.shrinkFontToFitMinimumSize != "StiEmptyValue" ? currProps.shrinkFontToFitMinimumSize : "";
            properties.linesOfUnderline.style.display = currProps["linesOfUnderline"] != null && levelDifficulty > 2 ? "" : "none";
            if (currProps["linesOfUnderline"] != null) controls.controlPropertyLinesOfUnderline.setKey(currProps.style != "StiEmptyValue" ? currProps.linesOfUnderline : "6");

            properties.renderTo.style.display = currProps["renderTo"] != null && levelDifficulty > 2 ? "" : "none";
            if (currProps["renderTo"] != null) {
                controls.controlPropertyRenderTo.addItems(jsObject.GetRenderToItems(currentObject));
                controls.controlPropertyRenderTo.setKey(currProps.renderTo);
            }
        }

        //Image Group
        var showImage = report && currentObject.typeComponent && (currentObject.typeComponent == "StiImage" || currentObject.typeComponent == "StiTableCellImage");
        if (showImage) designButtonBlock.style.display = "";

        //Image Additional Group
        if (showImage && !propertiesGroups.imageAdditionalPropertiesGroup) propertiesPanel.places["ImageAdditional"].appendChild(jsObject.ImageAdditionalPropertiesGroup());
        if (propertiesGroups.imageAdditionalPropertiesGroup) propertiesGroups.imageAdditionalPropertiesGroup.style.display = showImage ? "" : "none";
        if (showImage) {
            var expHorAlign = currProps.expressions && currProps.expressions["horAlignment"] != null ? StiBase64.decode(currProps.expressions["horAlignment"]) : null;
            controls.controlPropertyImageHorizontalAlignment.setKey(currProps.horAlignment, expHorAlign);
            var expVertAlign = currProps.expressions && currProps.expressions["vertAlignment"] != null ? StiBase64.decode(currProps.expressions["vertAlignment"]) : null;
            controls.controlPropertyImageVerticalAlignment.setKey(currProps.vertAlignment, expVertAlign);
            controls.controlPropertyImageAspectRatio.setChecked(currProps.ratio);
            controls.controlPropertyImageStretch.setChecked(currProps.stretch);
            properties.imageRotation.style.display = levelDifficulty > 1 ? "" : "none";
            controls.controlPropertyImageRotation.setKey(currProps.rotation);
            properties.imageMultipleFactor.style.display = levelDifficulty > 1 ? "" : "none";
            controls.controlPropertyImageMultipleFactor.value = currProps.imageMultipleFactor != "StiEmptyValue" ? jsObject.StrToDouble(currProps.imageMultipleFactor) : "";
            properties.imageProcessingDuplicates.style.display = levelDifficulty > 1 ? "" : "none";
            controls.controlPropertyImageProcessingDuplicates.setKey(currProps.imageProcessingDuplicates);
            properties.imageSmoothing.style.display = levelDifficulty > 1 ? "" : "none";
            controls.controlPropertyImageSmoothing.setChecked(currProps.imageSmoothing);
            properties.imageMargins.style.display = levelDifficulty > 1 ? "" : "none";
            controls.controlPropertyImageMargins.setValue(currProps["imageMargins"] == "StiEmptyValue" ? "" : currProps.imageMargins);
        }

        //Gauge Group
        var showGauge = report && currentObject.typeComponent && currentObject.typeComponent == "StiGauge";
        if (showGauge && !propertiesGroups.gaugePropertiesGroup) propertiesPanel.places["Gauge"].appendChild(jsObject.GaugePropertiesGroup());
        if (propertiesGroups.gaugePropertiesGroup) propertiesGroups.gaugePropertiesGroup.style.display = showGauge ? "" : "none";
        if (showGauge) {
            designButtonBlock.style.display = "";
            properties.allowApplyStyleGaugeComp.style.display = currProps["allowApplyStyle"] != null;
            if (currProps["allowApplyStyle"] != null) controls.controlPropertyGaugeAllowApplyStyle.setChecked(currProps.allowApplyStyle);
            properties.shortValueGaugeComp.style.display = currProps["shortValue"] != null;
            if (currProps["shortValue"] != null) controls.controlPropertyGaugeShortValue.setChecked(currProps.shortValue);
        }

        //BarCode Group
        var showBarCode = report && currentObject.typeComponent == "StiBarCode";
        if (showBarCode) designButtonBlock.style.display = "";

        //BarCode Additional Group
        if (showBarCode && !propertiesGroups.barCodeAdditionalPropertiesGroup) propertiesPanel.places["BarCodeAdditional"].appendChild(jsObject.BarCodeAdditionalPropertiesGroup());
        if (propertiesGroups.barCodeAdditionalPropertiesGroup) propertiesGroups.barCodeAdditionalPropertiesGroup.style.display = showBarCode ? "" : "none";
        if (showBarCode) {
            if (currProps["horAlignment"] != null) {
                var expression = currProps.expressions && currProps.expressions["horAlignment"] != null ? StiBase64.decode(currProps.expressions["horAlignment"]) : null;
                controls.controlPropertyBarCodeHorizontalAlignment.setKey(currProps.horAlignment, expression);
            }
            if (currProps["vertAlignment"] != null) {
                var expression = currProps.expressions && currProps.expressions["vertAlignment"] != null ? StiBase64.decode(currProps.expressions["vertAlignment"]) : null;
                controls.controlPropertyBarCodeVerticalAlignment.setKey(currProps.vertAlignment, expression);
            }
        }

        //Shape Group
        var showShape = report && currentObject.typeComponent == "StiShape";
        if (showShape) designButtonBlock.style.display = "";

        //Shape Text Group
        if (showShape && !propertiesGroups.shapeTextPropertiesGroup) propertiesPanel.places["ShapeText"].appendChild(jsObject.ShapeTextPropertiesGroup());
        if (propertiesGroups.shapeTextPropertiesGroup) propertiesGroups.shapeTextPropertiesGroup.style.display = showShape ? "" : "none";
        if (showShape) {
            properties.shapeHorAlignment.style.display = currProps["horAlignment"] != null ? "" : "none";
            if (currProps["horAlignment"] != null) {
                var expression = currProps.expressions && currProps.expressions["horAlignment"] != null ? StiBase64.decode(currProps.expressions["horAlignment"]) : null;
                controls.controlPropertyShapeHorizontalAlignment.setKey(currProps.horAlignment, expression);
            }
            properties.shapeVertAlignment.style.display = currProps["vertAlignment"] != null ? "" : "none";
            if (currProps["vertAlignment"] != null) {
                var expression = currProps.expressions && currProps.expressions["vertAlignment"] != null ? StiBase64.decode(currProps.expressions["vertAlignment"]) : null;
                controls.controlPropertyShapeVerticalAlignment.setKey(currProps.vertAlignment, expression);
            }
            properties.shapeForeColor.style.display = currProps["foreColor"] != null ? "" : "none";
            if (currProps["foreColor"] != null) {
                var expression = currProps.expressions && currProps.expressions["foreColor"] != null ? StiBase64.decode(currProps.expressions["foreColor"]) : null;
                controls.controlPropertyShapeForeColor.setKey(currProps.foreColor, currentObject.isDashboardElement, expression);
            }
            properties.shapeTextMargins.style.display = currProps["textMargins"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["textMargins"] != null) controls.controlPropertyShapeTextMargins.setValue(currProps["textMargins"] == "StiEmptyValue" ? "" : currProps.textMargins);
            properties.shapeText.style.display = currProps["shapeText"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["shapeText"] != null) controls.controlPropertyShapeText.value = StiBase64.decode(currProps["shapeText"]);
        }

        //Primitive Group
        var showPrimitive = report && currProps.isPrimitiveComponent;
        if (showPrimitive && !propertiesGroups.primitivePropertiesGroup) propertiesPanel.places["Primitive"].appendChild(jsObject.PrimitivePropertiesGroup());
        if (propertiesGroups.primitivePropertiesGroup) propertiesGroups.primitivePropertiesGroup.style.display = showPrimitive ? "" : "none";
        if (showPrimitive) {
            controls.controlPropertyPrimitiveColor.setKey(currProps.color);
            controls.controlPropertyPrimitiveStyle.setKey(currProps.style != "StiEmptyValue" ? currProps.style : "6");
            controls.controlPropertyPrimitiveSize.value = currProps.size != "StiEmptyValue" ? jsObject.StrToDouble(currProps.size) : "";
            properties.primitiveRound.style.display = currProps["round"] != null && currProps.round != "StiEmptyValue" ? "" : "none";
            if (currProps["round"] != null) controls.controlPropertyPrimitiveRound.value = currProps.round;

            properties.primitiveLeftSide.style.display = currProps["leftSide"] != null && currProps.leftSide != "StiEmptyValue" ? "" : "none";
            if (currProps["leftSide"] != null) controls.controlPropertyPrimitiveLeftSide.setChecked(currProps.leftSide);
            properties.primitiveRightSide.style.display = currProps["rightSide"] != null && currProps.rightSide != "StiEmptyValue" ? "" : "none";
            if (currProps["rightSide"] != null) controls.controlPropertyPrimitiveRightSide.setChecked(currProps.rightSide);
            properties.primitiveTopSide.style.display = currProps["topSide"] != null && currProps.topSide != "StiEmptyValue" ? "" : "none";
            if (currProps["topSide"] != null) controls.controlPropertyPrimitiveTopSide.setChecked(currProps.topSide);
            properties.primitiveBottomSide.style.display = currProps["bottomSide"] != null && currProps.bottomSide != "StiEmptyValue" ? "" : "none";
            if (currProps["bottomSide"] != null) controls.controlPropertyPrimitiveBottomSide.setChecked(currProps.bottomSide);

            propertiesGroups.startCapPropertiesGroup.style.display = currProps["startCapColor"] != null ? "" : "none";
            if (currProps["startCapColor"] != null) controls.controlPropertyStartCapColor.setKey(currProps.startCapColor);
            if (currProps["startCapFill"] != null) controls.controlPropertyStartCapFill.setChecked(currProps.startCapFill);
            if (currProps["startCapWidth"] != null) controls.controlPropertyStartCapWidth.value = currProps.startCapWidth;
            if (currProps["startCapHeight"] != null) controls.controlPropertyStartCapHeight.value = currProps.startCapHeight;
            if (currProps["startCapStyle"] != null) controls.controlPropertyStartCapStyle.setKey(currProps.startCapStyle);

            propertiesGroups.endCapPropertiesGroup.style.display = currProps["endCapColor"] != null ? "" : "none";
            if (currProps["endCapColor"] != null) controls.controlPropertyEndCapColor.setKey(currProps.endCapColor);
            if (currProps["endCapFill"] != null) controls.controlPropertyEndCapFill.setChecked(currProps.endCapFill);
            if (currProps["endCapWidth"] != null) controls.controlPropertyEndCapWidth.value = currProps.endCapWidth;
            if (currProps["endCapHeight"] != null) controls.controlPropertyEndCapHeight.value = currProps.endCapHeight;
            if (currProps["endCapStyle"] != null) controls.controlPropertyEndCapStyle.setKey(currProps.endCapStyle);
        }

        //Container Group
        var showContainer = report && currentObject.typeComponent == "StiClone";
        if (showContainer && !propertiesGroups.containerPropertiesGroup) propertiesPanel.places["Container"].appendChild(jsObject.ContainerPropertiesGroup());
        if (propertiesGroups.containerPropertiesGroup) propertiesGroups.containerPropertiesGroup.style.display = showContainer ? "" : "none";
        if (showContainer) {
            if (currProps["container"] != null) {
                var containerText = currProps.container != "[Not Assigned]"
                    ? (currProps.container == "StiEmptyValue" ? "" : currProps.container)
                    : "[" + jsObject.loc.Report.NotAssigned + "]";
                controls.controlPropertyCloneContainer.value = containerText;
            }
        }

        //ZipCode Group
        var showZipCode = report && currentObject.typeComponent == "StiZipCode";
        if (showZipCode && !propertiesGroups.zipCodePropertiesGroup) propertiesPanel.places["ZipCode"].appendChild(jsObject.ZipCodePropertiesGroup());
        if (propertiesGroups.zipCodePropertiesGroup) propertiesGroups.zipCodePropertiesGroup.style.display = showZipCode ? "" : "none";
        if (showZipCode) {
            designButtonBlock.style.display = "";
            controls.controlPropertyZipCode.value = currProps.code != "StiEmptyValue" ? StiBase64.decode(currProps.code) : "";
            controls.controlPropertyZipCodeSize.value = currProps.size != "StiEmptyValue" ? jsObject.StrToDouble(currProps.size) : "";
            controls.controlPropertyZipCodeRatio.setChecked(currProps.ratio);
            controls.controlPropertyZipCodeSpaceRatio.value = currProps.spaceRatio != "StiEmptyValue" ? jsObject.StrToDouble(currProps.spaceRatio) : "";
            controls.controlPropertyZipCodeUpperMarks.setChecked(currProps.upperMarks);
        }

        //Check Group
        var showCheck = report && (currentObject.typeComponent == "StiCheckBox" || currentObject.typeComponent == "StiTableCellCheckBox");
        if (showCheck && !propertiesGroups.checkPropertiesGroup) propertiesPanel.places["Check"].appendChild(jsObject.CheckPropertiesGroup());
        if (propertiesGroups.checkPropertiesGroup) propertiesGroups.checkPropertiesGroup.style.display = showCheck ? "" : "none";
        if (showCheck) {
            controls.controlPropertyChecked.value = currProps.checked != "StiEmptyValue" ? StiBase64.decode(currProps.checked) : "";
            if (currProps.checkStyleForTrue == "StiEmptyValue") controls.controlPropertyCheckStyleForTrue.setKey("None");
            if (currProps.checkStyleForFalse == "StiEmptyValue") controls.controlPropertyCheckStyleForFalse.setKey("None");
            properties.checkStyleForTrue.style.display = levelDifficulty > 1 ? "" : "none";
            controls.controlPropertyCheckStyleForTrue.setKey(currProps.checkStyleForTrue);
            properties.checkStyleForFalse.style.display = levelDifficulty > 1 ? "" : "none";
            controls.controlPropertyCheckStyleForFalse.setKey(currProps.checkStyleForFalse);
            properties.checkValues.style.display = levelDifficulty > 1 ? "" : "none";
            controls.controlPropertyCheckValues.setKey(currProps.checkValues);
            properties.checkSize.style.display = levelDifficulty > 1 ? "" : "none";
            controls.controlPropertyCheckSize.value = currProps.size != "StiEmptyValue" ? jsObject.StrToDouble(currProps.size) : "";
            properties.checkEditable.style.display = levelDifficulty > 1 ? "" : "none";
            controls.controlPropertyCheckEditable.setChecked(currProps.editable);
        }

        //CrossTab Group
        var showCrossTab = report && currentObject.typeComponent == "StiCrossTab";
        if (showCrossTab && !propertiesGroups.crossTabPropertiesGroup) propertiesPanel.places["CrossTab"].appendChild(jsObject.CrossTabPropertiesGroup());
        if (propertiesGroups.crossTabPropertiesGroup) propertiesGroups.crossTabPropertiesGroup.style.display = showCrossTab ? "" : "none";
        if (showCrossTab) {
            controls.controlPropertyCrossTabEmptyValue.value = currProps.crossTabEmptyValue != "StiEmptyValue" ? StiBase64.decode(currProps.crossTabEmptyValue) : "";
            controls.controlPropertyCrossTabHorAlign.setKey(currProps.crossTabHorAlign);
            controls.controlPropertyCrossTabPrintIfEmpty.setChecked(currProps.printIfEmpty);
            controls.controlPropertyCrossTabRightToLeft.setChecked(currProps.rightToLeft);
            controls.controlPropertyCrossTabWrap.setChecked(currProps.crossTabWrap);
            properties.crossTabWrapGap.style.display = currProps.crossTabWrap ? "" : "none";
            controls.controlPropertyCrossTabWrapGap.value = currProps.crossTabWrapGap != "StiEmptyValue" ? jsObject.StrToDouble(currProps.crossTabWrapGap) : "";
        }

        //Hierarchical Group
        var showHierarchical = report && currentObject.typeComponent == "StiHierarchicalBand";
        if (showHierarchical && !propertiesGroups.hierarchicalPropertiesGroup) propertiesPanel.places["Hierarchical"].appendChild(jsObject.HierarchicalPropertiesGroup());
        if (propertiesGroups.hierarchicalPropertiesGroup) propertiesGroups.hierarchicalPropertiesGroup.style.display = showHierarchical ? "" : "none";
        if (showHierarchical) {
            controls.controlPropertyKeyDataColumn.value = currProps.keyDataColumn != "StiEmptyValue"
                ? (currProps.keyDataColumn ? currProps.keyDataColumn : jsObject.loc.Report.NotAssigned)
                : "";
            controls.controlPropertyMasterKeyDataColumn.value = currProps.masterKeyDataColumn != "StiEmptyValue"
                ? (currProps.masterKeyDataColumn ? currProps.masterKeyDataColumn : jsObject.loc.Report.NotAssigned)
                : "";
            controls.controlPropertyParentValue.value = currProps.parentValue != "StiEmptyValue" ? StiBase64.decode(currProps.parentValue) : "";
            controls.controlPropertyIndent.value = currProps.indent != "StiEmptyValue" ? jsObject.StrToDouble(currProps.indent) : "";
            controls.controlPropertyHeaders.value = currProps.headers != "StiEmptyValue" ? StiBase64.decode(currProps.headers) : "";
            controls.controlPropertyFooters.value = currProps.footers != "StiEmptyValue" ? StiBase64.decode(currProps.footers) : "";
        }

        //Design Group
        var showDesignGroup = report && currentObject.typeComponent != "StiReport";
        if (showDesignGroup && !propertiesGroups.designPropertiesGroup) propertiesPanel.places["Design"].appendChild(jsObject.DesignPropertiesGroup());
        if (propertiesGroups.designPropertiesGroup) propertiesGroups.designPropertiesGroup.style.display = showDesignGroup ? "" : "none";

        if (showDesignGroup) {
            properties.componentName.style.display = currProps["name"] != null && currProps.name != "StiEmptyValue" ? "" : "none";
            if (currProps["name"] != null) controls.controlPropertyComponentName.value = currProps.name;
            properties.aliasName.style.display = currProps["aliasName"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["aliasName"] != null) controls.controlPropertyAlias.value = currProps.aliasName != "StiEmptyValue" ? StiBase64.decode(currProps.aliasName) : "";
            properties.globalizedName.style.display = currProps["globalizedName"] != null && levelDifficulty > 2 ? "" : "none";
            if (currProps["globalizedName"] != null) controls.controlPropertyGlobalizedName.value = currProps.globalizedName != "StiEmptyValue" ? currProps.globalizedName : "";
            properties.largeHeight.style.display = currProps["largeHeight"] != null && levelDifficulty > 2 ? "" : "none";
            if (currProps["largeHeight"] != null) controls.controlPropertyLargeHeight.setChecked(currProps.largeHeight);
            properties.largeHeightFactor.style.display = currProps["largeHeightFactor"] != null && levelDifficulty > 2 ? "" : "none";
            if (currProps["largeHeightFactor"] != null) controls.controlPropertyLargeHeightFactor.value = currProps.largeHeightFactor != "StiEmptyValue" ? currProps.largeHeightFactor : "";
            properties.restrictions.style.display = currProps["restrictions"] != null && jsObject.options.modifyRestrictions && !jsObject.IsTableCell(currentObject) && levelDifficulty > 2 ? "" : "none";
            if (currProps["restrictions"] != null) controls.controlPropertyRestrictions.setKey(currProps.restrictions);
            properties.locked.style.display = currProps["locked"] != null && levelDifficulty > 2 ? "" : "none";
            if (currProps["locked"] != null) controls.controlPropertyLocked.setChecked(currProps.locked);
            properties.linked.style.display = currProps["linked"] != null && levelDifficulty > 2 ? "" : "none";
            if (currProps["linked"] != null) controls.controlPropertyLinked.setChecked(currProps.linked);
            properties.pageIcon.style.display = currProps["pageIcon"] != null && levelDifficulty > 2 ? "" : "none";
            if (currProps["pageIcon"] != null) controls.controlPropertyPageIcon.setKey(currProps.pageIcon);
        }

        //Data Group
        var showDataGroup = report && currentObject.typeComponent && (currentObject.typeComponent == "StiChart" || currentObject.typeComponent == "StiSparkline" ||
            currentObject.typeComponent == "StiDataBand" || currentObject.typeComponent == "StiCrossDataBand" || currentObject.typeComponent == "StiCrossTab");
        if (showDataGroup && !propertiesGroups.dataPropertiesGroup) propertiesPanel.places["Data"].appendChild(jsObject.DataPropertiesGroup());
        if (propertiesGroups.dataPropertiesGroup) propertiesGroups.dataPropertiesGroup.style.display = showDataGroup ? "" : "none";
        if (showDataGroup) {
            properties.dataSource.style.display = currProps["dataSource"] != null ? "" : "none";
            controls.controlPropertyDataSource.addItems();
            if (currProps["dataSource"] != null) controls.controlPropertyDataSource.setKey(currProps.dataSource);
            properties.masterComponent.style.display = currProps["masterComponent"] != null ? "" : "none";
            controls.controlPropertyMasterComponent.addItems();
            if (currProps["masterComponent"] != null) controls.controlPropertyMasterComponent.setKey(currProps.masterComponent);
            properties.countData.style.display = currProps["countData"] != null ? "" : "none";
            if (currProps["countData"] != null) controls.controlPropertyCountData.value = currProps.countData;
            properties.filters.style.display = currProps["filterData"] != null ? "" : "none";
            if (currProps["filterData"] != null) properties.filters.propertyControl.setKey({ filterData: currProps.filterData, filterOn: currProps.filterOn, filterMode: currProps.filterMode });
            properties.sortData.style.display = currProps["sortData"] != null ? "" : "none";
            if (currProps["sortData"] != null) properties.sortData.propertyControl.setKey(currProps.sortData);
            properties.valueDataColumn.style.display = currProps["valueDataColumn"] != null ? "" : "none";
            if (currProps["valueDataColumn"] != null) controls.controlPropertyDataValueDataColumn.value = StiBase64.decode(currProps.valueDataColumn);            
            properties.multipleInitialization.style.display = currProps["multipleInitialization"] != null ? "" : "none";
            if (currProps["multipleInitialization"] != null) properties.multipleInitialization.propertyControl.setChecked(currProps.multipleInitialization);

            properties.dataRelation.style.display = currProps["dataRelation"] != null ? "" : "none";
            if (currProps["valueDataColumn"] != null) {
                var valueDataColumn = StiBase64.decode(currProps["valueDataColumn"]);
                var dataSourceName = valueDataColumn && valueDataColumn.indexOf(".") > 0 ? valueDataColumn.substring(0, valueDataColumn.indexOf(".")) : "";
                var dataSource = jsObject.GetDataSourceByNameFromDictionary(dataSourceName);
                var relations = jsObject.GetRelationsInSourceItems(dataSource);
                controls.controlPropertyDataRelation.addItems(relations);
            }
            else {
                var dataSourceControl = jsObject.options.controls.controlPropertyDataSource;
                var dataSource = jsObject.GetDataSourceByNameFromDictionary(dataSourceControl.key);
                var relations = jsObject.GetRelationsInSourceItems(dataSource);
                controls.controlPropertyDataRelation.addItems(relations);
            }
            if (currProps["dataRelation"] != null) controls.controlPropertyDataRelation.setKey(currProps.dataRelation);
        }

        //Map Group
        var showMapGroup = report && currentObject.typeComponent && currentObject.typeComponent == "StiMap";
        if (showMapGroup && !propertiesGroups.mapPropertiesGroup) propertiesPanel.places["Map"].appendChild(jsObject.MapPropertiesGroup());
        if (propertiesGroups.mapPropertiesGroup) propertiesGroups.mapPropertiesGroup.style.display = showMapGroup ? "" : "none";
        if (showMapGroup) {
            designButtonBlock.style.display = "";
            properties.colorEachMap.style.display = currProps["colorEach"] != null ? "" : "none";
            if (currProps["colorEach"] != null) properties.colorEachMap.propertyControl.setChecked(currProps.colorEach);
            properties.showValueMap.style.display = currProps["showValue"] != null ? "" : "none";
            if (currProps["showValue"] != null) properties.showValueMap.propertyControl.setChecked(currProps.showValue);
            properties.stretchMap.style.display = currProps["stretch"] != null ? "" : "none";
            if (currProps["stretch"] != null) properties.stretchMap.propertyControl.setChecked(currProps.stretch);
            properties.displayNameTypeMap.style.display = currProps["displayNameType"] != null ? "" : "none";
            if (currProps["displayNameType"] != null) properties.displayNameTypeMap.propertyControl.setKey(currProps.displayNameType);
        }

        //MathFormula
        var showMathFormula = report && currentObject.typeComponent && (currentObject.typeComponent == "StiMathFormula");
        if (showMathFormula) designButtonBlock.style.display = "";

        //Electronic Signature
        var showElectronicSignature = report && currentObject.typeComponent && (currentObject.typeComponent == "StiElectronicSignature");
        if (showElectronicSignature) designButtonBlock.style.display = "";

        //Design Button
        var showCondition = report && currentObject.typeComponent && (currentObject.typeComponent == "StiGroupHeaderBand" || currentObject.typeComponent == "StiCrossGroupHeaderBand");
        var showData = report && currProps["dataSource"] != null && currentObject.typeComponent && currentObject.typeComponent != "StiChart" && currentObject.typeComponent != "StiCrossTab";
        if (showCondition || showData) designButtonBlock.style.display = "";

        //TableGroup
        var showTableGroup = report && currentObject.typeComponent == "StiTable";
        if (showTableGroup) {
            if (!propertiesGroups.tablePropertiesGroup) propertiesPanel.places["Table"].appendChild(jsObject.TablePropertiesGroup());
            if (!propertiesGroups.headerTablePropertiesGroup) propertiesPanel.places["HeaderTable"].appendChild(jsObject.HeaderOrFooterTablePropertiesGroup("header"));
            if (!propertiesGroups.footerTablePropertiesGroup) propertiesPanel.places["FooterTable"].appendChild(jsObject.HeaderOrFooterTablePropertiesGroup("footer"));
        }
        if (propertiesGroups.tablePropertiesGroup) propertiesGroups.tablePropertiesGroup.style.display = showTableGroup ? "" : "none";
        if (propertiesGroups.headerTablePropertiesGroup) propertiesGroups.headerTablePropertiesGroup.style.display = showTableGroup ? "" : "none";
        if (propertiesGroups.footerTablePropertiesGroup) propertiesGroups.footerTablePropertiesGroup.style.display = showTableGroup ? "" : "none";

        if (showTableGroup) {
            var tableProperties = ["tableAutoWidth", "autoWidthType", "columnCount", "rowCount", "headerRowsCount", "footerRowsCount", "tableRightToLeft", "dockableTable", "headerPrintOn",
                "headerCanGrow", "headerCanShrink", "headerCanBreak", "headerPrintAtBottom", "headerPrintIfEmpty", "headerPrintOnAllPages", "headerPrintOnEvenOddPages",
                "footerPrintOn", "footerCanGrow", "footerCanShrink", "footerCanBreak", "footerPrintAtBottom", "footerPrintIfEmpty", "footerPrintOnAllPages", "footerPrintOnEvenOddPages"];

            for (var i = 0; i < tableProperties.length; i++) {
                var upperPropertyName = jsObject.UpperFirstChar(tableProperties[i]);
                if (properties[tableProperties[i]]) properties[tableProperties[i]].style.display = currProps[tableProperties[i]] != null ? "" : "none";
                if (currProps[tableProperties[i]] != null && controls["controlProperty" + upperPropertyName])
                    jsObject.SetControlValue(controls["controlProperty" + upperPropertyName], currProps[tableProperties[i]]);
            }
        }

        //Chart
        var showChartGroup = report && (currentObject.typeComponent == "StiChartElement" || currentObject.typeComponent == "StiChart");
        if (showChartGroup && !propertiesGroups.chartPropertiesGroup) propertiesPanel.places["Chart"].appendChild(jsObject.ChartPropertiesGroup());
        if (propertiesGroups.chartPropertiesGroup) propertiesGroups.chartPropertiesGroup.style.display = showChartGroup && levelDifficulty > 1 ? "" : "none";
        if (showChartGroup) {
            var multiSelected = currProps.name == "StiEmptyValue";
            var isCircleChart = currProps.isSunburstChart || currProps.isPieChart || currProps.isPictorialStackedChart;

            properties.groupChart.style.display = currProps["group"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["group"] != null) controls.chartElementGroup.value = currProps.group != "StiEmptyValue" ? currProps.group : "";
            properties.chartConstantLines.style.display = !isCircleChart && !multiSelected && currProps["chartConstantLines"] != null ? "" : "none";
            if (!isCircleChart && !multiSelected && currProps["chartConstantLines"] != null) properties.chartConstantLines.propertyControl.setKey(currProps.chartConstantLines);
            properties.chartTrendLines.style.display = !isCircleChart && !multiSelected && currProps["chartTrendLines"] != null ? "" : "none";
            if (!isCircleChart && !multiSelected && currProps["chartTrendLines"] != null) properties.chartTrendLines.propertyControl.setKey(currProps.chartTrendLines);
            properties.chartSeries.style.display = !multiSelected && currProps["chartSeries"] != null ? "" : "none";
            if (currProps["chartSeries"] != null) properties.chartSeries.propertyControl.setKey(currProps.chartSeries);
            properties.chartStrips.style.display = !multiSelected && currProps["chartStrips"] != null ? "" : "none";
            if (currProps["chartStrips"] != null) properties.chartStrips.propertyControl.setKey(currProps.chartStrips);
            properties.dataTransformationChart.style.display = levelDifficulty > 1 && currProps.name != "StiEmptyValue" && currProps["dataTransformation"] ? "" : "none";

            properties.crossFilteringChart.style.display = currProps["crossFiltering"] != null ? "" : "none";
            if (currProps["crossFiltering"] != null) controls.chartElementCrossFiltering.setChecked(currProps.crossFiltering);
            properties.areaAllowApplyStyle.style.display = currProps["areaAllowApplyStyle"] != null ? "" : "none";
            if (currProps["areaAllowApplyStyle"] != null) controls.chartElementAreaAllowApplyStyle.setChecked(currProps.areaAllowApplyStyle);
            properties.areaBrush.style.display = currProps["areaBrush"] != null && currProps.areaAllowApplyStyle === false ? "" : "none";
            if (currProps["areaBrush"] != null) controls.chartElementAreaBrush.setKey(currProps.areaBrush);
            properties.areaBorderColor.style.display = currProps["areaBorderColor"] != null && currProps.areaAllowApplyStyle === false ? "" : "none";
            if (currProps["areaBorderColor"] != null) controls.chartElementAreaBorderColor.setKey(currProps.areaBorderColor);
            properties.areaBorderThickness.style.display = currProps["areaBorderThickness"] != null && currProps.areaAllowApplyStyle === false ? "" : "none";
            if (currProps["areaBorderThickness"] != null) controls.chartElementAreaBorderThickness.value = currProps.areaBorderThickness;
            properties.areaColorEach.style.display = currProps["areaColorEach"] != null ? "" : "none";
            if (currProps["areaColorEach"] != null) controls.chartElementAreaColorEach.setChecked(currProps.areaColorEach);
            properties.areaReverseHor.style.display = currProps["isAxisAreaChart"] && currProps["areaReverseHor"] != null ? "" : "none";
            if (currProps["areaReverseHor"] != null) controls.chartElementAreaReverseHor.setChecked(currProps.areaReverseHor);
            properties.areaReverseVert.style.display = currProps["isAxisAreaChart"] && currProps["areaReverseVert"] != null ? "" : "none";
            if (currProps["areaReverseVert"] != null) controls.chartElementAreaReverseVert.setChecked(currProps.areaReverseVert);
            properties.areaSideBySide.style.display = (currProps["isAxisAreaChart"] || (currProps["isAxisAreaChart3D"] && currProps["isClusteredColumnChart3D"])) && currProps["areaSideBySide"] != null ? "" : "none";
            if (currProps["areaSideBySide"] != null) controls.chartElementAreaSideBySide.setChecked(currProps.areaSideBySide);

            properties.areaGridLinesHorAllowApplyStyle.style.display = currProps["areaGridLinesHorAllowApplyStyle"] != null ? "" : "none";
            if (currProps["areaGridLinesHorAllowApplyStyle"] != null) controls.chartElementAreaGridLinesHorAllowApplyStyle.setChecked(currProps.areaGridLinesHorAllowApplyStyle);
            properties.areaGridLinesHorMinorColor.style.display = currProps["areaGridLinesHorMinorColor"] != null && currProps.areaGridLinesHorAllowApplyStyle === false ? "" : "none";
            if (currProps["areaGridLinesHorMinorColor"] != null) controls.chartElementAreaGridLinesHorMinorColor.setKey(currProps.areaGridLinesHorMinorColor);
            properties.areaGridLinesHorMinorCount.style.display = currProps["areaGridLinesHorMinorCount"] != null ? "" : "none";
            if (currProps["areaGridLinesHorMinorCount"] != null) controls.chartElementAreaGridLinesHorMinorCount.value = jsObject.ExtractBase64Value(currProps.areaGridLinesHorMinorCount);
            properties.areaGridLinesHorMinorStyle.style.display = currProps["areaGridLinesHorMinorStyle"] != null && currProps.areaGridLinesHorAllowApplyStyle === false ? "" : "none";
            if (currProps["areaGridLinesHorMinorStyle"] != null) controls.chartElementAreaGridLinesHorMinorStyle.setKey(currProps.areaGridLinesHorMinorStyle);
            properties.areaGridLinesHorColor.style.display = currProps["areaGridLinesHorColor"] != null && (currentObject.typeComponent == "StiChartElement" || currProps.areaGridLinesHorAllowApplyStyle === false) ? "" : "none";
            if (currProps["areaGridLinesHorColor"] != null) controls.chartElementAreaGridLinesHorColor.setKey(currProps.areaGridLinesHorColor);
            properties.areaGridLinesHorVisible.style.display = currProps["areaGridLinesHorVisible"] != null ? "" : "none";
            if (currProps["areaGridLinesHorVisible"] != null) controls.chartElementAreaGridLinesHorVisible.setChecked(currProps.areaGridLinesHorVisible);
            properties.areaGridLinesHorMinorVisible.style.display = currProps["areaGridLinesHorMinorVisible"] != null ? "" : "none";
            if (currProps["areaGridLinesHorMinorVisible"] != null) controls.chartElementAreaGridLinesHorMinorVisible.setChecked(currProps.areaGridLinesHorMinorVisible);
            properties.areaGridLinesHorStyle.style.display = currProps["areaGridLinesHorStyle"] != null && currProps.areaGridLinesHorAllowApplyStyle === false ? "" : "none";
            if (currProps["areaGridLinesHorStyle"] != null) controls.chartElementAreaGridLinesHorStyle.setKey(currProps.areaGridLinesHorStyle);

            properties.areaGridLinesVertAllowApplyStyle.style.display = currProps["areaGridLinesVertAllowApplyStyle"] != null ? "" : "none";
            if (currProps["areaGridLinesVertAllowApplyStyle"] != null) controls.chartElementAreaGridLinesVertAllowApplyStyle.setChecked(currProps.areaGridLinesVertAllowApplyStyle);
            properties.areaGridLinesVertMinorColor.style.display = currProps["areaGridLinesVertMinorColor"] != null && currProps.areaGridLinesVertAllowApplyStyle === false ? "" : "none";
            if (currProps["areaGridLinesVertMinorColor"] != null) controls.chartElementAreaGridLinesVertMinorColor.setKey(currProps.areaGridLinesVertMinorColor);
            properties.areaGridLinesVertMinorCount.style.display = currProps["areaGridLinesVertMinorCount"] != null ? "" : "none";
            if (currProps["areaGridLinesVertMinorCount"] != null) controls.chartElementAreaGridLinesVertMinorCount.value = jsObject.ExtractBase64Value(currProps.areaGridLinesVertMinorCount);
            properties.areaGridLinesVertMinorStyle.style.display = currProps["areaGridLinesVertMinorStyle"] != null && currProps.areaGridLinesVertAllowApplyStyle === false ? "" : "none";
            if (currProps["areaGridLinesVertMinorStyle"] != null) controls.chartElementAreaGridLinesVertMinorStyle.setKey(currProps.areaGridLinesVertMinorStyle);
            properties.areaGridLinesVertColor.style.display = currProps["areaGridLinesVertColor"] != null && (currentObject.typeComponent == "StiChartElement" || currProps.areaGridLinesVertAllowApplyStyle === false) ? "" : "none";
            if (currProps["areaGridLinesVertColor"] != null) controls.chartElementAreaGridLinesVertColor.setKey(currProps.areaGridLinesVertColor);
            properties.areaGridLinesVertVisible.style.display = currProps["areaGridLinesVertVisible"] != null ? "" : "none";
            if (currProps["areaGridLinesVertVisible"] != null) controls.chartElementAreaGridLinesVertVisible.setChecked(currProps.areaGridLinesVertVisible);
            properties.areaGridLinesVertMinorVisible.style.display = currProps["areaGridLinesVertMinorVisible"] != null ? "" : "none";
            if (currProps["areaGridLinesVertMinorVisible"] != null) controls.chartElementAreaGridLinesVertMinorVisible.setChecked(currProps.areaGridLinesVertMinorVisible);
            properties.areaGridLinesVertStyle.style.display = currProps["areaGridLinesVertStyle"] != null && currProps.areaGridLinesVertAllowApplyStyle === false ? "" : "none";
            if (currProps["areaGridLinesVertStyle"] != null) controls.chartElementAreaGridLinesVertStyle.setKey(currProps.areaGridLinesVertStyle);

            properties.areaInterlacingHorAllowApplyStyle.style.display = currProps["areaInterlacingHorAllowApplyStyle"] != null ? "" : "none";
            if (currProps["areaInterlacingHorAllowApplyStyle"] != null) controls.chartElementAreaInterlacingHorAllowApplyStyle.setChecked(currProps.areaInterlacingHorAllowApplyStyle);
            properties.areaInterlacingHorInterlacedBrush.style.display = currProps["areaInterlacingHorInterlacedBrush"] != null && currProps.areaInterlacingHorAllowApplyStyle === false ? "" : "none";
            if (currProps["areaInterlacingHorInterlacedBrush"] != null) controls.chartElementAreaInterlacingHorInterlacedBrush.setKey(currProps.areaInterlacingHorInterlacedBrush);
            properties.areaInterlacingHorColor.style.display = currProps["areaInterlacingHorColor"] != null ? "" : "none";
            if (currProps["areaInterlacingHorColor"] != null) controls.chartElementAreaInterlacingHorColor.setKey(currProps.areaInterlacingHorColor);
            properties.areaInterlacingHorVisible.style.display = currProps["areaInterlacingHorVisible"] != null ? "" : "none";
            if (currProps["areaInterlacingHorVisible"] != null) controls.chartElementAreaInterlacingHorVisible.setChecked(currProps.areaInterlacingHorVisible);

            properties.areaInterlacingVertAllowApplyStyle.style.display = currProps["areaInterlacingVertAllowApplyStyle"] != null ? "" : "none";
            if (currProps["areaInterlacingVertAllowApplyStyle"] != null) controls.chartElementAreaInterlacingVertAllowApplyStyle.setChecked(currProps.areaInterlacingVertAllowApplyStyle);
            properties.areaInterlacingVertInterlacedBrush.style.display = currProps["areaInterlacingVertInterlacedBrush"] != null && currProps.areaInterlacingVertAllowApplyStyle === false ? "" : "none";
            if (currProps["areaInterlacingVertInterlacedBrush"] != null) controls.chartElementAreaInterlacingVertInterlacedBrush.setKey(currProps.areaInterlacingVertInterlacedBrush);
            properties.areaInterlacingVertColor.style.display = currProps["areaInterlacingVertColor"] != null ? "" : "none";
            if (currProps["areaInterlacingVertColor"] != null) controls.chartElementAreaInterlacingVertColor.setKey(currProps.areaInterlacingVertColor);
            properties.areaInterlacingVertVisible.style.display = currProps["areaInterlacingVertVisible"] != null ? "" : "none";
            if (currProps["areaInterlacingVertVisible"] != null) controls.chartElementAreaInterlacingVertVisible.setChecked(currProps.areaInterlacingVertVisible);

            properties.labelsLabelsType.style.display = currProps["labelsLabelsType"] != null ? "" : "none";
            if (currProps["labelsLabelsType"] != null && currProps["labelsServiceName"] != null) {
                controls.chartElementLabelsLabelsType.setKey(currProps.labelsLabelsType);
                controls.chartElementLabelsLabelsType.textBox.value = currProps.labelsServiceName;
            }
            properties.labelsAngle.style.display = currProps["labelsAngle"] != null ? "" : "none";
            if (currProps["labelsAngle"] != null) controls.chartElementLabelsAngle.value = this.jsObject.StrToDouble(currProps.labelsAngle);
            properties.labelsAntialiasing.style.display = currProps["labelsAntialiasing"] != null ? "" : "none";
            if (currProps["labelsAntialiasing"] != null) controls.chartElementLabelsAntialiasing.setChecked(currProps.labelsAntialiasing);
            properties.labelsDrawBorder.style.display = currProps["labelsDrawBorder"] != null ? "" : "none";
            if (currProps["labelsDrawBorder"] != null) controls.chartElementLabelsDrawBorder.setChecked(currProps.labelsDrawBorder);
            properties.labelsFormat.style.display = currProps["labelsFormat"] != null ? "" : "none";
            if (currProps["labelsFormat"] != null) controls.chartElementLabelsFormat.value = jsObject.ExtractBase64Value(currProps.labelsFormat);
            properties.labelsLineColor.style.display = currProps["labelsLineColor"] != null ? "" : "none";
            if (currProps["labelsLineColor"] != null) controls.chartElementLabelsLineColor.setKey(currProps.labelsLineColor);
            properties.labelsLegendValueType.style.display = currProps["labelsLegendValueType"] != null ? "" : "none";
            if (currProps["labelsLegendValueType"] != null) controls.chartElementLabelsLegendValueType.setKey(currProps.labelsLegendValueType);
            properties.labelsLineLength.style.display = currProps["labelsLineLength"] != null ? "" : "none";
            if (currProps["labelsLineLength"] != null) controls.chartElementLabelsLineLength.value = jsObject.ExtractBase64Value(currProps.labelsLineLength);
            properties.labelsMarkerAlignment.style.display = currProps["labelsMarkerAlignment"] != null ? "" : "none";
            if (currProps["labelsMarkerAlignment"] != null) controls.chartElementLabelsMarkerAlignment.setKey(currProps.labelsMarkerAlignment);
            properties.labelsMarkerSize.style.display = currProps["labelsMarkerSize"] != null ? "" : "none";
            if (currProps["labelsMarkerSize"] != null) controls.chartElementLabelsMarkerSize.setValue(currProps.labelsMarkerSize);
            properties.labelsMarkerVisible.style.display = currProps["labelsMarkerVisible"] != null ? "" : "none";
            if (currProps["labelsMarkerVisible"] != null) controls.chartElementLabelsMarkerVisible.setChecked(currProps.labelsMarkerVisible);
            properties.labelsPreventIntersection.style.display = currProps["labelsPreventIntersection"] != null ? "" : "none";
            if (currProps["labelsPreventIntersection"] != null) controls.chartElementLabelsPreventIntersection.setChecked(currProps.labelsPreventIntersection);
            properties.labelsShowInPercent.style.display = currProps["labelsShowInPercent"] != null ? "" : "none";
            if (currProps["labelsShowInPercent"] != null) controls.chartElementLabelsShowInPercent.setChecked(currProps.labelsShowInPercent);
            properties.labelsShowNulls.style.display = currProps["labelsShowNulls"] != null ? "" : "none";
            if (currProps["labelsShowNulls"] != null) controls.chartElementLabelsShowNulls.setChecked(currProps.labelsShowNulls);
            properties.labelsShowZeros.style.display = currProps["labelsShowZeros"] != null ? "" : "none";
            if (currProps["labelsShowZeros"] != null) controls.chartElementLabelsShowZeros.setChecked(currProps.labelsShowZeros);
            properties.labelsStep.style.display = currProps["labelsStep"] != null ? "" : "none";
            if (currProps["labelsStep"] != null) controls.chartElementLabelsStep.value = jsObject.ExtractBase64Value(currProps.labelsStep);
            properties.labelsUseSeriesColor.style.display = currProps["labelsUseSeriesColor"] != null ? "" : "none";
            if (currProps["labelsUseSeriesColor"] != null) controls.chartElementLabelsUseSeriesColor.setChecked(currProps.labelsUseSeriesColor);
            properties.labelsValueType.style.display = currProps["labelsValueType"] != null ? "" : "none";
            if (currProps["labelsValueType"] != null) controls.chartElementLabelsValueType.setKey(currProps.labelsValueType);
            properties.labelsValueTypeSeparator.style.display = currProps["labelsValueTypeSeparator"] != null ? "" : "none";
            if (currProps["labelsValueTypeSeparator"] != null) controls.chartElementLabelsValueTypeSeparator.value = jsObject.ExtractBase64Value(currProps.labelsValueTypeSeparator);
            properties.labelsVisible.style.display = currProps["labelsVisible"] != null ? "" : "none";
            if (currProps["labelsVisible"] != null) controls.chartElementLabelsVisible.setChecked(currProps.labelsVisible);
            properties.labelsWidth.style.display = currProps["labelsWidth"] != null ? "" : "none";
            if (currProps["labelsWidth"] != null) controls.chartElementLabelsWidth.value = jsObject.ExtractBase64Value(currProps.labelsWidth);
            properties.labelsWordWrap.style.display = currProps["labelsWordWrap"] != null ? "" : "none";
            if (currProps["labelsWordWrap"] != null) controls.chartElementLabelsWordWrap.setChecked(currProps.labelsWordWrap);
            properties.labelsAllowApplyStyle.style.display = currProps["labelsAllowApplyStyle"] != null ? "" : "none";
            if (currProps["labelsAllowApplyStyle"] != null) controls.chartElementLabelsAllowApplyStyle.setChecked(currProps.labelsAllowApplyStyle);
            properties.labelsFont.style.display = currProps["labelsFont"] != null && (currentObject.typeComponent == "StiChartElement" || currProps.labelsAllowApplyStyle === false) ? "" : "none";
            if (currProps["labelsFont"] != null) properties.labelsFont.propertyControl.setKey(currProps.labelsFont);
            properties.labelsBrush.style.display = currProps["labelsBrush"] != null && (currentObject.typeComponent == "StiChartElement" || currProps.labelsAllowApplyStyle === false) ? "" : "none";
            if (currProps["labelsBrush"] != null) controls.chartElementLabelsBrush.setKey(currProps.labelsBrush);
            properties.labelsLabelColor.style.display = currProps["labelsLabelColor"] != null && (currentObject.typeComponent == "StiChartElement" || currProps.labelsAllowApplyStyle === false) ? "" : "none";
            if (currProps["labelsLabelColor"] != null) controls.chartElementLabelsLabelColor.setKey(currProps.labelsLabelColor);
            properties.labelsBorderColor.style.display = currProps["labelsBorderColor"] != null && (currentObject.typeComponent == "StiChartElement" || currProps.labelsAllowApplyStyle === false) ? "" : "none";
            if (currProps["labelsBorderColor"] != null) controls.chartElementLabelsBorderColor.setKey(currProps.labelsBorderColor);
            properties.labelsForeColor.style.display = currProps["labelsForeColor"] != null && (currentObject.typeComponent == "StiChartElement" || currProps.labelsAllowApplyStyle === false) ? "" : "none";
            if (currProps["labelsForeColor"] != null) controls.chartElementLabelsForeColor.setKey(currProps.labelsForeColor, currentObject.isDashboardElement);
            properties.labelsAutoRotate.style.display = currProps["labelsAutoRotate"] != null && currProps["labelsPosition"] != "TwoColumns" ? "" : "none";
            if (currProps["labelsAutoRotate"] != null) controls.chartElementLabelsAutoRotate.setChecked(currProps.labelsAutoRotate);
            properties.labelsPosition.style.display = currProps["labelsPosition"] != null ? "" : "none";
            if (currProps["labelsPosition"] != null) {
                controls.chartElementLabelsPosition.addItems(jsObject.GetLabelsPositionItems(currProps));
                controls.chartElementLabelsPosition.setKey(currProps.labelsPosition);
            }
            properties.labelsStyle.style.display = currProps["labelsStyle"] != null ? "" : "none";
            if (currProps["labelsStyle"] != null) {
                controls.chartElementLabelsStyle.addItems(jsObject.GetLabelsStyleItems(currProps));
                controls.chartElementLabelsStyle.setKey(currProps.labelsStyle);
            }
            properties.labelsTextAfter.style.display = currProps["labelsTextAfter"] != null ? "" : "none";
            if (currProps["labelsTextAfter"] != null) controls.chartElementLabelsTextAfter.value = jsObject.ExtractBase64Value(currProps.labelsTextAfter);
            properties.labelsTextBefore.style.display = currProps["labelsTextBefore"] != null ? "" : "none";
            if (currProps["labelsTextBefore"] != null) controls.chartElementLabelsTextBefore.value = jsObject.ExtractBase64Value(currProps.labelsTextBefore);

            properties.legendAllowApplyStyle.style.display = currProps["legendAllowApplyStyle"] != null ? "" : "none";
            if (currProps["legendAllowApplyStyle"] != null) controls.chartElementLegendAllowApplyStyle.setChecked(currProps.legendAllowApplyStyle);
            properties.legendBorderColor.style.display = currProps["legendBorderColor"] != null && (currentObject.typeComponent == "StiChartElement" || currProps.legendAllowApplyStyle === false) ? "" : "none";
            if (currProps["legendBorderColor"] != null) controls.chartElementLegendBorderColor.setKey(currProps.legendBorderColor);
            properties.legendBrush.style.display = currProps["legendBrush"] != null && (currentObject.typeComponent == "StiChartElement" || currProps.legendAllowApplyStyle === false) ? "" : "none";
            if (currProps["legendBrush"] != null) controls.chartElementLegendBrush.setKey(currProps.legendBrush);
            properties.legendFont.style.display = currProps["legendFont"] != null && (currentObject.typeComponent == "StiChartElement" || currProps.legendAllowApplyStyle === false) ? "" : "none";
            if (currProps["legendFont"] != null) properties.legendFont.propertyControl.setKey(currProps.legendFont);
            properties.legendColumns.style.display = currProps["legendColumns"] != null ? "" : "none";
            if (currProps["legendColumns"] != null) controls.chartElementLegendColumns.value = this.jsObject.StrToCorrectPositiveInt(currProps.legendColumns);
            properties.legendDirection.style.display = currProps["legendDirection"] != null ? "" : "none";
            if (currProps["legendDirection"] != null) controls.chartElementLegendDirection.setKey(currProps.legendDirection);
            properties.legendVisibility.style.display = currProps["legendVisibility"] != null ? "" : "none";
            if (currProps["legendVisibility"] != null) controls.chartElementLegendVisibility.setKey(currProps.legendVisibility);
            properties.legendHorAlignment.style.display = currProps["legendHorAlignment"] != null ? "" : "none";
            if (currProps["legendHorAlignment"] != null) controls.chartElementLegendHorAlignment.setKey(currProps.legendHorAlignment);
            properties.legendVertAlignment.style.display = currProps["legendVertAlignment"] != null ? "" : "none";
            if (currProps["legendVertAlignment"] != null) controls.chartElementLegendVertAlignment.setKey(currProps.legendVertAlignment);
            properties.legendLabelsColor.style.display = currProps["legendLabelsColor"] != null ? "" : "none";
            if (currProps["legendLabelsColor"] != null) controls.chartElementLegendLabelsColor.setKey(currProps.legendLabelsColor, currentObject.isDashboardElement);
            properties.legendLabelsFont.style.display = currProps["legendLabelsFont"] != null ? "" : "none";
            if (currProps["legendLabelsFont"] != null) properties.legendLabelsFont.propertyControl.setKey(currProps.legendLabelsFont);
            properties.legendLabelsValueType.style.display = currProps["legendLabelsValueType"] != null && (currProps["isDoughnutChart"] || currProps["isPieChart"]) ? "" : "none";
            if (currProps["legendLabelsValueType"] != null) properties.legendLabelsValueType.propertyControl.setKey(currProps.legendLabelsValueType);
            properties.legendTitleColor.style.display = currProps["legendTitleColor"] != null ? "" : "none";
            if (currProps["legendTitleColor"] != null) controls.chartElementLegendTitleColor.setKey(currProps.legendTitleColor, currentObject.isDashboardElement);
            properties.legendTitleFont.style.display = currProps["legendTitleFont"] != null ? "" : "none";
            if (currProps["legendTitleFont"] != null) properties.legendTitleFont.propertyControl.setKey(currProps.legendTitleFont);
            properties.legendTitleText.style.display = currProps["legendTitleText"] != null ? "" : "none";
            if (currProps["legendTitleText"] != null) controls.chartElementLegendTitleText.value = jsObject.ExtractBase64Value(currProps.legendTitleText);

            properties.xAxisLabelsAngle.style.display = currProps["xAxisLabelsAngle"] != null && !currProps.isAxisAreaChart3D ? "" : "none";
            if (currProps["xAxisLabelsAngle"] != null) controls.chartElementXAxisLabelsAngle.value = this.jsObject.StrToDouble(currProps.xAxisLabelsAngle);
            properties.xAxisLabelsColor.style.display = currProps["xAxisLabelsColor"] != null ? "" : "none";
            if (currProps["xAxisLabelsColor"] != null) controls.chartElementXAxisLabelsColor.setKey(currProps.xAxisLabelsColor);
            properties.xAxisLabelsFont.style.display = currProps["xAxisLabelsFont"] != null ? "" : "none";
            if (currProps["xAxisLabelsFont"] != null) properties.xAxisLabelsFont.propertyControl.setKey(currProps.xAxisLabelsFont);
            properties.xAxisLabelsPlacement.style.display = currProps["xAxisLabelsPlacement"] != null && !currProps.isAxisAreaChart3D ? "" : "none";
            if (currProps["xAxisLabelsPlacement"] != null) controls.chartElementXAxisLabelsPlacement.setKey(currProps.xAxisLabelsPlacement);
            properties.xAxisLabelsTextAfter.style.display = currProps["xAxisLabelsTextAfter"] != null ? "" : "none";
            if (currProps["xAxisLabelsTextAfter"] != null) controls.chartElementXAxisLabelsTextAfter.value = jsObject.ExtractBase64Value(currProps.xAxisLabelsTextAfter);
            properties.xAxisLabelsTextAlignment.style.display = currProps["xAxisLabelsTextAlignment"] != null && !currProps.isAxisAreaChart3D ? "" : "none";
            if (currProps["xAxisLabelsTextAlignment"] != null) controls.chartElementXAxisLabelsTextAlignment.setKey(currProps.xAxisLabelsTextAlignment);
            properties.xAxisLabelsTextBefore.style.display = currProps["xAxisLabelsTextBefore"] != null ? "" : "none";
            if (currProps["xAxisLabelsTextBefore"] != null) controls.chartElementXAxisLabelsTextBefore.value = jsObject.ExtractBase64Value(currProps.xAxisLabelsTextBefore);
            properties.xAxisLabelsStep.style.display = currProps["xAxisLabelsStep"] != null && !currProps.isAxisAreaChart3D ? "" : "none";
            if (currProps["xAxisLabelsStep"] != null) controls.chartElementXAxisLabelsStep.value = jsObject.ExtractBase64Value(currProps.xAxisLabelsStep);
            properties.xAxisRangeAuto.style.display = currProps["xAxisRangeAuto"] != null ? "" : "none";
            if (currProps["xAxisRangeAuto"] != null) controls.chartElementXAxisRangeAuto.setChecked(currProps.xAxisRangeAuto);
            properties.xAxisRangeMinimum.style.display = currProps["xAxisRangeMinimum"] != null ? "" : "none";
            if (currProps["xAxisRangeMinimum"] != null) controls.chartElementXAxisRangeMinimum.value = jsObject.StrToDouble(currProps.xAxisRangeMinimum);
            properties.xAxisRangeMaximum.style.display = currProps["xAxisRangeMaximum"] != null ? "" : "none";
            if (currProps["xAxisRangeMaximum"] != null) controls.chartElementXAxisRangeMaximum.value = jsObject.StrToDouble(currProps.xAxisRangeMaximum);
            properties.xAxisTitleAlignment.style.display = currProps["xAxisTitleAlignment"] != null ? "" : "none";
            if (currProps["xAxisTitleAlignment"] != null) controls.chartElementXAxisTitleAlignment.setKey(currProps.xAxisTitleAlignment);
            properties.xAxisTitleColor.style.display = currProps["xAxisTitleColor"] != null ? "" : "none";
            if (currProps["xAxisTitleColor"] != null) controls.chartElementXAxisTitleColor.setKey(currProps.xAxisTitleColor);
            properties.xAxisTitleDirection.style.display = currProps["xAxisTitleDirection"] != null ? "" : "none";
            if (currProps["xAxisTitleDirection"] != null) controls.chartElementXAxisTitleDirection.setKey(currProps.xAxisTitleDirection);
            properties.xAxisTitleFont.style.display = currProps["xAxisTitleFont"] != null ? "" : "none";
            if (currProps["xAxisTitleFont"] != null) properties.xAxisTitleFont.propertyControl.setKey(currProps.xAxisTitleFont);
            properties.xAxisTitlePosition.style.display = currProps["xAxisTitlePosition"] != null ? "" : "none";
            if (currProps["xAxisTitlePosition"] != null) controls.chartElementXAxisTitlePosition.setKey(currProps.xAxisTitlePosition);
            properties.xAxisTitleText.style.display = currProps["xAxisTitleText"] != null ? "" : "none";
            if (currProps["xAxisTitleText"] != null) controls.chartElementXAxisTitleText.value = jsObject.ExtractBase64Value(currProps.xAxisTitleText);
            properties.xAxisTitleVisible.style.display = currProps["xAxisTitleVisible"] != null ? "" : "none";
            if (currProps["xAxisTitleVisible"] != null) controls.chartElementXAxisTitleVisible.setChecked(currProps.xAxisTitleVisible);
            properties.xAxisVisible.style.display = currProps["xAxisVisible"] != null ? "" : "none";
            if (currProps["xAxisVisible"] != null) controls.chartElementXAxisVisible.setChecked(currProps.xAxisVisible);

            properties.xAxisShowEdgeValues.style.display = currProps["xAxisShowEdgeValues"] != null ? "" : "none";
            if (currProps["xAxisShowEdgeValues"] != null) {
                controls.chartElementXAxisShowEdgeValues.addItems(jsObject.GetShowEdgeValuesItems(currentObject.typeComponent == "StiChart"));
                controls.chartElementXAxisShowEdgeValues.setKey(currProps.xAxisShowEdgeValues);
            }

            properties.xAxisStartFromZero.style.display = currProps["xAxisStartFromZero"] != null && !currProps.isAxisAreaChart3D ? "" : "none";
            if (currProps["xAxisStartFromZero"] != null) {
                controls.chartElementXAxisStartFromZero.addItems(jsObject.GetShowEdgeValuesItems(currentObject.typeComponent == "StiChart"));
                controls.chartElementXAxisStartFromZero.setKey(currProps.xAxisStartFromZero);
            }

            properties.xTopAxisLabelsAngle.style.display = currProps["xTopAxisLabelsAngle"] != null ? "" : "none";
            if (currProps["xTopAxisLabelsAngle"] != null) controls.chartElementXTopAxisLabelsAngle.value = this.jsObject.StrToDouble(currProps.xTopAxisLabelsAngle);
            properties.xTopAxisLabelsColor.style.display = currProps["xTopAxisLabelsColor"] != null ? "" : "none";
            if (currProps["xTopAxisLabelsColor"] != null) controls.chartElementXTopAxisLabelsColor.setKey(currProps.xTopAxisLabelsColor);
            properties.xTopAxisLabelsFont.style.display = currProps["xTopAxisLabelsFont"] != null ? "" : "none";
            if (currProps["xTopAxisLabelsFont"] != null) properties.xTopAxisLabelsFont.propertyControl.setKey(currProps.xTopAxisLabelsFont);
            properties.xTopAxisLabelsPlacement.style.display = currProps["xTopAxisLabelsPlacement"] != null ? "" : "none";
            if (currProps["xTopAxisLabelsPlacement"] != null) controls.chartElementXTopAxisLabelsPlacement.setKey(currProps.xTopAxisLabelsPlacement);
            properties.xTopAxisLabelsTextAfter.style.display = currProps["xTopAxisLabelsTextAfter"] != null ? "" : "none";
            if (currProps["xTopAxisLabelsTextAfter"] != null) controls.chartElementXTopAxisLabelsTextAfter.value = jsObject.ExtractBase64Value(currProps.xTopAxisLabelsTextAfter);
            properties.xTopAxisLabelsTextAlignment.style.display = currProps["xTopAxisLabelsTextAlignment"] != null ? "" : "none";
            if (currProps["xTopAxisLabelsTextAlignment"] != null) controls.chartElementXTopAxisLabelsTextAlignment.setKey(currProps.xTopAxisLabelsTextAlignment);
            properties.xTopAxisLabelsTextBefore.style.display = currProps["xTopAxisLabelsTextBefore"] != null ? "" : "none";
            if (currProps["xTopAxisLabelsTextBefore"] != null) controls.chartElementXTopAxisLabelsTextBefore.value = jsObject.ExtractBase64Value(currProps.xTopAxisLabelsTextBefore);
            properties.xTopAxisLabelsStep.style.display = currProps["xTopAxisLabelsStep"] != null ? "" : "none";
            if (currProps["xTopAxisLabelsStep"] != null) controls.chartElementXTopAxisLabelsStep.value = jsObject.ExtractBase64Value(currProps.xTopAxisLabelsStep);
            properties.xTopAxisTitleAlignment.style.display = currProps["xTopAxisTitleAlignment"] != null ? "" : "none";
            if (currProps["xTopAxisTitleAlignment"] != null) controls.chartElementXTopAxisTitleAlignment.setKey(currProps.xTopAxisTitleAlignment);
            properties.xTopAxisTitleColor.style.display = currProps["xTopAxisTitleColor"] != null ? "" : "none";
            if (currProps["xTopAxisTitleColor"] != null) controls.chartElementXTopAxisTitleColor.setKey(currProps.xTopAxisTitleColor);
            properties.xTopAxisTitleDirection.style.display = currProps["xTopAxisTitleDirection"] != null ? "" : "none";
            if (currProps["xTopAxisTitleDirection"] != null) controls.chartElementXTopAxisTitleDirection.setKey(currProps.xTopAxisTitleDirection);
            properties.xTopAxisTitleFont.style.display = currProps["xTopAxisTitleFont"] != null ? "" : "none";
            if (currProps["xTopAxisTitleFont"] != null) properties.xTopAxisTitleFont.propertyControl.setKey(currProps.xTopAxisTitleFont);
            properties.xTopAxisTitlePosition.style.display = currProps["xTopAxisTitlePosition"] != null ? "" : "none";
            if (currProps["xTopAxisTitlePosition"] != null) controls.chartElementXTopAxisTitlePosition.setKey(currProps.xTopAxisTitlePosition);
            properties.xTopAxisTitleText.style.display = currProps["xTopAxisTitleText"] != null ? "" : "none";
            if (currProps["xTopAxisTitleText"] != null) controls.chartElementXTopAxisTitleText.value = jsObject.ExtractBase64Value(currProps.xTopAxisTitleText);
            properties.xTopAxisTitleVisible.style.display = currProps["xTopAxisTitleVisible"] != null ? "" : "none";
            if (currProps["xTopAxisTitleVisible"] != null) controls.chartElementXTopAxisTitleVisible.setChecked(currProps.xTopAxisTitleVisible);
            properties.xTopAxisVisible.style.display = currProps["xTopAxisVisible"] != null ? "" : "none";
            if (currProps["xTopAxisVisible"] != null) controls.chartElementXTopAxisVisible.setChecked(currProps.xTopAxisVisible);

            properties.xTopAxisShowEdgeValues.style.display = currProps["xTopAxisShowEdgeValues"] != null ? "" : "none";
            if (currProps["xTopAxisShowEdgeValues"] != null) {
                controls.chartElementXTopAxisShowEdgeValues.addItems(jsObject.GetShowEdgeValuesItems(currentObject.typeComponent == "StiChart"));
                controls.chartElementXTopAxisShowEdgeValues.setKey(currProps.xTopAxisShowEdgeValues);
            }

            properties.yAxisLabelsAngle.style.display = currProps["yAxisLabelsAngle"] != null && !currProps.isAxisAreaChart3D ? "" : "none";
            if (currProps["yAxisLabelsAngle"] != null) controls.chartElementYAxisLabelsAngle.value = this.jsObject.StrToDouble(currProps.yAxisLabelsAngle);
            properties.yAxisLabelsColor.style.display = currProps["yAxisLabelsColor"] != null ? "" : "none";
            if (currProps["yAxisLabelsColor"] != null) controls.chartElementYAxisLabelsColor.setKey(currProps.yAxisLabelsColor);
            properties.yAxisLabelsFont.style.display = currProps["yAxisLabelsFont"] != null ? "" : "none";
            if (currProps["yAxisLabelsFont"] != null) properties.yAxisLabelsFont.propertyControl.setKey(currProps.yAxisLabelsFont);
            properties.yAxisLabelsPlacement.style.display = currProps["yAxisLabelsPlacement"] != null && !currProps.isAxisAreaChart3D ? "" : "none";
            if (currProps["yAxisLabelsPlacement"] != null) controls.chartElementYAxisLabelsPlacement.setKey(currProps.yAxisLabelsPlacement);
            properties.yAxisLabelsTextAfter.style.display = currProps["yAxisLabelsTextAfter"] != null ? "" : "none";
            if (currProps["yAxisLabelsTextAfter"] != null) controls.chartElementYAxisLabelsTextAfter.value = jsObject.ExtractBase64Value(currProps.yAxisLabelsTextAfter);
            properties.yAxisLabelsTextAlignment.style.display = currProps["yAxisLabelsTextAlignment"] != null && !currProps.isAxisAreaChart3D ? "" : "none";
            if (currProps["yAxisLabelsTextAlignment"] != null) controls.chartElementYAxisLabelsTextAlignment.setKey(currProps.yAxisLabelsTextAlignment);
            properties.yAxisLabelsTextBefore.style.display = currProps["yAxisLabelsTextBefore"] != null ? "" : "none";
            if (currProps["yAxisLabelsTextBefore"] != null) controls.chartElementYAxisLabelsTextBefore.value = jsObject.ExtractBase64Value(currProps.yAxisLabelsTextBefore);
            properties.yAxisLabelsStep.style.display = currProps["yAxisLabelsStep"] != null && !currProps.isAxisAreaChart3D ? "" : "none";
            if (currProps["yAxisLabelsStep"] != null) controls.chartElementYAxisLabelsStep.value = jsObject.ExtractBase64Value(currProps.yAxisLabelsStep);
            properties.yAxisRangeAuto.style.display = currProps["yAxisRangeAuto"] != null ? "" : "none";
            if (currProps["yAxisRangeAuto"] != null) controls.chartElementYAxisRangeAuto.setChecked(currProps.yAxisRangeAuto);
            properties.yAxisRangeMinimum.style.display = currProps["yAxisRangeMinimum"] != null ? "" : "none";
            if (currProps["yAxisRangeMinimum"] != null) controls.chartElementYAxisRangeMinimum.value = jsObject.StrToDouble(currProps.yAxisRangeMinimum);
            properties.yAxisRangeMaximum.style.display = currProps["yAxisRangeMaximum"] != null ? "" : "none";
            if (currProps["yAxisRangeMaximum"] != null) controls.chartElementYAxisRangeMaximum.value = jsObject.StrToDouble(currProps.yAxisRangeMaximum);
            properties.yAxisTitleAlignment.style.display = currProps["yAxisTitleAlignment"] != null ? "" : "none";
            if (currProps["yAxisTitleAlignment"] != null) controls.chartElementYAxisTitleAlignment.setKey(currProps.yAxisTitleAlignment);
            properties.yAxisTitleColor.style.display = currProps["yAxisTitleColor"] != null ? "" : "none";
            if (currProps["yAxisTitleColor"] != null) controls.chartElementYAxisTitleColor.setKey(currProps.yAxisTitleColor);
            properties.yAxisTitleDirection.style.display = currProps["yAxisTitleDirection"] != null ? "" : "none";
            if (currProps["yAxisTitleDirection"] != null) controls.chartElementYAxisTitleDirection.setKey(currProps.yAxisTitleDirection);
            properties.yAxisTitleFont.style.display = currProps["yAxisTitleFont"] != null ? "" : "none";
            if (currProps["yAxisTitleFont"] != null) properties.yAxisTitleFont.propertyControl.setKey(currProps.yAxisTitleFont);
            properties.yAxisTitlePosition.style.display = currProps["yAxisTitlePosition"] != null ? "" : "none";
            if (currProps["yAxisTitlePosition"] != null) controls.chartElementYAxisTitlePosition.setKey(currProps.yAxisTitlePosition);
            properties.yAxisTitleText.style.display = currProps["yAxisTitleText"] != null ? "" : "none";
            if (currProps["yAxisTitleText"] != null) controls.chartElementYAxisTitleText.value = jsObject.ExtractBase64Value(currProps.yAxisTitleText);
            properties.yAxisTitleVisible.style.display = currProps["yAxisTitleVisible"] != null ? "" : "none";
            if (currProps["yAxisTitleVisible"] != null) controls.chartElementYAxisTitleVisible.setChecked(currProps.yAxisTitleVisible);
            properties.yAxisVisible.style.display = currProps["yAxisVisible"] != null ? "" : "none";
            if (currProps["yAxisVisible"] != null) controls.chartElementYAxisVisible.setChecked(currProps.yAxisVisible);
            properties.yAxisStartFromZero.style.display = currProps["yAxisStartFromZero"] != null && !currProps.isAxisAreaChart3D ? "" : "none";
            if (currProps["yAxisStartFromZero"] != null) controls.chartElementYAxisStartFromZero.setChecked(currProps.yAxisStartFromZero);

            properties.yRightAxisLabelsAngle.style.display = currProps["yRightAxisLabelsAngle"] != null ? "" : "none";
            if (currProps["yRightAxisLabelsAngle"] != null) controls.chartElementYRightAxisLabelsAngle.value = this.jsObject.StrToDouble(currProps.yRightAxisLabelsAngle);
            properties.yRightAxisLabelsColor.style.display = currProps["yRightAxisLabelsColor"] != null ? "" : "none";
            if (currProps["yRightAxisLabelsColor"] != null) controls.chartElementYRightAxisLabelsColor.setKey(currProps.yRightAxisLabelsColor);
            properties.yRightAxisLabelsFont.style.display = currProps["yRightAxisLabelsFont"] != null ? "" : "none";
            if (currProps["yRightAxisLabelsFont"] != null) properties.yRightAxisLabelsFont.propertyControl.setKey(currProps.yRightAxisLabelsFont);
            properties.yRightAxisLabelsPlacement.style.display = currProps["yRightAxisLabelsPlacement"] != null ? "" : "none";
            if (currProps["yRightAxisLabelsPlacement"] != null) controls.chartElementYRightAxisLabelsPlacement.setKey(currProps.yRightAxisLabelsPlacement);
            properties.yRightAxisLabelsTextAfter.style.display = currProps["yRightAxisLabelsTextAfter"] != null ? "" : "none";
            if (currProps["yRightAxisLabelsTextAfter"] != null) controls.chartElementYRightAxisLabelsTextAfter.value = jsObject.ExtractBase64Value(currProps.yRightAxisLabelsTextAfter);
            properties.yRightAxisLabelsTextAlignment.style.display = currProps["yRightAxisLabelsTextAlignment"] != null ? "" : "none";
            if (currProps["yRightAxisLabelsTextAlignment"] != null) controls.chartElementYRightAxisLabelsTextAlignment.setKey(currProps.yRightAxisLabelsTextAlignment);
            properties.yRightAxisLabelsTextBefore.style.display = currProps["yRightAxisLabelsTextBefore"] != null ? "" : "none";
            if (currProps["yRightAxisLabelsTextBefore"] != null) controls.chartElementYRightAxisLabelsTextBefore.value = jsObject.ExtractBase64Value(currProps.yRightAxisLabelsTextBefore);
            properties.yRightAxisTitleAlignment.style.display = currProps["yRightAxisTitleAlignment"] != null ? "" : "none";
            if (currProps["yRightAxisTitleAlignment"] != null) controls.chartElementYRightAxisTitleAlignment.setKey(currProps.yRightAxisTitleAlignment);
            properties.yRightAxisTitleColor.style.display = currProps["yRightAxisTitleColor"] != null ? "" : "none";
            if (currProps["yRightAxisTitleColor"] != null) controls.chartElementYRightAxisTitleColor.setKey(currProps.yRightAxisTitleColor);
            properties.yRightAxisTitleDirection.style.display = currProps["yRightAxisTitleDirection"] != null ? "" : "none";
            if (currProps["yRightAxisTitleDirection"] != null) controls.chartElementYRightAxisTitleDirection.setKey(currProps.yRightAxisTitleDirection);
            properties.yRightAxisTitleFont.style.display = currProps["yRightAxisTitleFont"] != null ? "" : "none";
            if (currProps["yRightAxisTitleFont"] != null) properties.yRightAxisTitleFont.propertyControl.setKey(currProps.yRightAxisTitleFont);
            properties.yRightAxisTitlePosition.style.display = currProps["yRightAxisTitlePosition"] != null ? "" : "none";
            if (currProps["yRightAxisTitlePosition"] != null) controls.chartElementYRightAxisTitlePosition.setKey(currProps.yRightAxisTitlePosition);
            properties.yRightAxisTitleText.style.display = currProps["yRightAxisTitleText"] != null ? "" : "none";
            if (currProps["yRightAxisTitleText"] != null) controls.chartElementYRightAxisTitleText.value = jsObject.ExtractBase64Value(currProps.yRightAxisTitleText);
            properties.yRightAxisTitleVisible.style.display = currProps["yRightAxisTitleVisible"] != null ? "" : "none";
            if (currProps["yRightAxisTitleVisible"] != null) controls.chartElementYRightAxisTitleVisible.setChecked(currProps.yRightAxisTitleVisible);
            properties.yRightAxisVisible.style.display = currProps["yRightAxisVisible"] != null ? "" : "none";
            if (currProps["yRightAxisVisible"] != null) controls.chartElementYRightAxisVisible.setChecked(currProps.yRightAxisVisible);
            properties.yRightAxisStartFromZero.style.display = currProps["yRightAxisStartFromZero"] != null ? "" : "none";
            if (currProps["yRightAxisStartFromZero"] != null) controls.chartElementYRightAxisStartFromZero.setChecked(currProps.yRightAxisStartFromZero);

            var showMarker = jsObject.ShowChartElementMarkerProperty(currProps.valueMeters);
            properties.markerAngle.style.display = showMarker && currProps["markerAngle"] != null ? "" : "none";
            if (showMarker && currProps["markerAngle"] != null) controls.chartElementMarkerAngle.value = jsObject.StrToDouble(currProps.markerAngle);
            properties.markerSize.style.display = showMarker && currProps["markerSize"] != null ? "" : "none";
            if (showMarker && currProps["markerSize"] != null) controls.chartElementMarkerSize.value = jsObject.StrToDouble(currProps.markerSize);
            properties.markerType.style.display = showMarker && currProps["markerType"] != null ? "" : "none";
            if (showMarker && currProps["markerType"] != null) controls.chartElementMarkerType.setKey(currProps.markerType);
            properties.markerVisible.style.display = showMarker && currProps["markerVisible"] != null ? "" : "none";
            if (showMarker && currProps["markerVisible"] != null) controls.chartElementMarkerVisible.setKey(currProps.markerVisible);

            properties.chartTitleAllowApplyStyle.style.display = currProps["chartTitleAllowApplyStyle"] != null ? "" : "none";
            if (currProps["chartTitleAllowApplyStyle"] != null) controls.chartElementTitleAllowApplyStyle.setChecked(currProps.chartTitleAllowApplyStyle);
            properties.chartTitleAntialiasing.style.display = currProps["chartTitleAntialiasing"] != null ? "" : "none";
            if (currProps["chartTitleAntialiasing"] != null) controls.chartElementTitleAntialiasing.setChecked(currProps.chartTitleAntialiasing);
            properties.chartTitleBrush.style.display = currProps["chartTitleBrush"] != null && currProps.chartTitleAllowApplyStyle === false ? "" : "none";
            if (currProps["chartTitleBrush"] != null) controls.chartElementTitleBrush.setKey(currProps.chartTitleBrush);
            properties.chartTitleDock.style.display = currProps["chartTitleDock"] != null ? "" : "none";
            if (currProps["chartTitleDock"] != null) controls.chartElementTitleDock.setKey(currProps.chartTitleDock);
            properties.chartTitleFont.style.display = currProps["chartTitleFont"] != null && currProps.chartTitleAllowApplyStyle === false ? "" : "none";
            if (currProps["chartTitleFont"] != null) properties.chartTitleFont.propertyControl.setKey(currProps.chartTitleFont);
            properties.chartTitleSpacing.style.display = currProps["chartTitleSpacing"] != null ? "" : "none";
            if (currProps["chartTitleSpacing"] != null) controls.chartElementTitleSpacing.value = jsObject.ExtractBase64Value(currProps.chartTitleSpacing);
            properties.chartTitleAlignment.style.display = currProps["chartTitleAlignment"] != null ? "" : "none";
            if (currProps["chartTitleAlignment"] != null) controls.chartElementTitleAlignment.setKey(currProps.chartTitleAlignment);
            properties.chartTitleText.style.display = currProps["chartTitleText"] != null ? "" : "none";
            if (currProps["chartTitleText"] != null) controls.chartElementTitleText.value = jsObject.ExtractBase64Value(currProps.chartTitleText);
            properties.chartTitleVisible.style.display = currProps["chartTitleVisible"] != null ? "" : "none";
            if (currProps["chartTitleVisible"] != null) controls.chartElementTitleVisible.setChecked(currProps.chartTitleVisible);

            properties.chartTableAllowApplyStyle.style.display = currProps["chartTableAllowApplyStyle"] != null ? "" : "none";
            if (currProps["chartTableAllowApplyStyle"] != null) controls.chartElementTableAllowApplyStyle.setChecked(currProps.chartTableAllowApplyStyle);
            properties.chartTableGridLineColor.style.display = currProps["chartTableGridLineColor"] != null && currProps.chartTableAllowApplyStyle === false ? "" : "none";
            if (currProps["chartTableGridLineColor"] != null) controls.chartElementTableGridLineColor.setKey(currProps.chartTableGridLineColor);
            properties.chartTableGridLinesHor.style.display = currProps["chartTableGridLinesHor"] != null ? "" : "none";
            if (currProps["chartTableGridLinesHor"] != null) controls.chartElementTableGridLinesHor.setChecked(currProps.chartTableGridLinesHor);
            properties.chartTableGridLinesVert.style.display = currProps["chartTableGridLinesVert"] != null ? "" : "none";
            if (currProps["chartTableGridLinesVert"] != null) controls.chartElementTableGridLinesVert.setChecked(currProps.chartTableGridLinesVert);
            properties.chartTableGridOutline.style.display = currProps["chartTableGridOutline"] != null ? "" : "none";
            if (currProps["chartTableGridOutline"] != null) controls.chartElementTableGridOutline.setChecked(currProps.chartTableGridOutline);
            properties.chartTableMarkerVisible.style.display = currProps["chartTableMarkerVisible"] != null ? "" : "none";
            if (currProps["chartTableMarkerVisible"] != null) controls.chartElementTableMarkerVisible.setChecked(currProps.chartTableMarkerVisible);
            properties.chartTableVisible.style.display = currProps["chartTableVisible"] != null ? "" : "none";
            if (currProps["chartTableVisible"] != null) controls.chartElementTableVisible.setChecked(currProps.chartTableVisible);
            properties.chartTableDataCellsTextColor.style.display = currProps["chartTableDataCellsTextColor"] != null ? "" : "none";
            if (currProps["chartTableDataCellsTextColor"] != null) controls.chartElementTableDataCellsTextColor.setKey(currProps.chartTableDataCellsTextColor);
            properties.chartTableDataCellsShrinkFontToFit.style.display = currProps["chartTableDataCellsShrinkFontToFit"] != null ? "" : "none";
            if (currProps["chartTableDataCellsShrinkFontToFit"] != null) controls.chartElementTableDataCellsShrinkFontToFit.setChecked(currProps.chartTableDataCellsShrinkFontToFit);
            properties.chartTableDataCellsShrinkFontToFitMinimumSize.style.display = currProps["chartTableDataCellsShrinkFontToFitMinimumSize"] != null ? "" : "none";
            if (currProps["chartTableDataCellsShrinkFontToFitMinimumSize"] != null) controls.chartElementTableDataCellsShrinkFontToFitMinimumSize.value = currProps.chartTableDataCellsShrinkFontToFitMinimumSize;
            properties.chartTableDataCellsFont.style.display = currProps["chartTableDataCellsFont"] != null ? "" : "none";
            if (currProps["chartTableDataCellsFont"] != null) properties.chartTableDataCellsFont.propertyControl.setKey(currProps.chartTableDataCellsFont);
            properties.chartTableHeaderTextAfter.style.display = currProps["chartTableHeaderTextAfter"] != null ? "" : "none";
            if (currProps["chartTableHeaderTextAfter"] != null) controls.chartElementTableHeaderTextAfter.value = jsObject.ExtractBase64Value(currProps.chartTableHeaderTextAfter);
            properties.chartTableHeaderTextColor.style.display = currProps["chartTableHeaderTextColor"] != null ? "" : "none";
            if (currProps["chartTableHeaderTextColor"] != null) controls.chartElementTableHeaderTextColor.setKey(currProps.chartTableHeaderTextColor);
            properties.chartTableHeaderWordWrap.style.display = currProps["chartTableHeaderWordWrap"] != null ? "" : "none";
            if (currProps["chartTableHeaderWordWrap"] != null) controls.chartElementTableHeaderWordWrap.setChecked(currProps.chartTableHeaderWordWrap);
            properties.chartTableHeaderBrush.style.display = currProps["chartTableHeaderBrush"] != null ? "" : "none";
            if (currProps["chartTableHeaderBrush"] != null) controls.chartElementTableHeaderBrush.setKey(currProps.chartTableHeaderBrush);
            properties.chartTableHeaderFont.style.display = currProps["chartTableHeaderFont"] != null ? "" : "none";
            if (currProps["chartTableHeaderFont"] != null) properties.chartTableHeaderFont.propertyControl.setKey(currProps.chartTableHeaderFont);

            properties.options3DDistance.style.display = currProps["options3DDistance"] != null ? "" : "none";
            if (currProps["options3DDistance"] != null) controls.chartElementOptions3DDistance.value = jsObject.ExtractBase64Value(currProps.options3DDistance);
            properties.options3DHeight.style.display = currProps["options3DHeight"] != null ? "" : "none";
            if (currProps["options3DHeight"] != null) controls.chartElementOptions3DHeight.value = jsObject.ExtractBase64Value(currProps.options3DHeight);
            properties.options3DLighting.style.display = currProps["options3DLighting"] != null ? "" : "none";
            if (currProps["options3DLighting"] != null) controls.chartElementOptions3DLighting.setKey(currProps.options3DLighting);
            properties.options3DOpacity.style.display = currProps["options3DOpacity"] != null ? "" : "none";
            if (currProps["options3DOpacity"] != null) controls.chartElementOptions3DOpacity.value = jsObject.ExtractBase64Value(currProps.options3DOpacity);

            var innerGroups = propertiesGroups.chartPropertiesGroup.innerGroups;
            innerGroups.xAxis.style.display = currProps.isAxisAreaChart || currProps.isAxisAreaChart3D || currProps.isRadarChart ? "" : "none";
            innerGroups.yAxis.style.display = currProps.isAxisAreaChart || currProps.isAxisAreaChart3D || currProps.isRadarChart ? "" : "none";
            innerGroups.xTopAxis.style.display = currProps.isAxisAreaChart ? "" : "none";
            innerGroups.yRightAxis.style.display = currProps.isAxisAreaChart ? "" : "none";
            innerGroups.yAxisTitle.style.display = currProps.isAxisAreaChart && !currProps.isRadarChart ? "" : "none";
            innerGroups.xAxisTitle.style.display = currProps.isAxisAreaChart && !currProps.isRadarChart ? "" : "none";
            innerGroups.areaGridLinesHor.style.display = currProps.isAxisAreaChart || (currProps.isAxisAreaChart3D && currentObject.typeComponent == "StiChart") ? "" : "none";
            innerGroups.areaGridLinesVert.style.display = currProps.isAxisAreaChart || (currProps.isAxisAreaChart3D && currentObject.typeComponent == "StiChart") ? "" : "none";
            innerGroups.areaInterlacingHor.style.display = currProps.isAxisAreaChart || (currProps.isAxisAreaChart3D && currentObject.typeComponent == "StiChart") ? "" : "none";
            innerGroups.areaInterlacingVert.style.display = currProps.isAxisAreaChart || (currProps.isAxisAreaChart3D && currentObject.typeComponent == "StiChart") ? "" : "none";
            innerGroups.marker.style.display = showMarker ? "" : "none";
            innerGroups.area.style.display = !currProps.isSunburstChart ? "" : "none";
            innerGroups.title.style.display = innerGroups.table.style.display = currentObject.typeComponent == "StiChart" && levelDifficulty > 1 ? "" : "none";
            innerGroups.labels.style.display = levelDifficulty > 1 ? "" : "none";
            innerGroups.legend.style.display = levelDifficulty > 1 ? "" : "none";
            innerGroups.options3D.style.display = levelDifficulty > 1 && currProps.isPie3dChart ? "" : "none";
            innerGroups.xAxisRange.style.display = currProps.isAxisAreaChart || currProps.isRadarChart ? "" : "none";
            innerGroups.yAxisRange.style.display = currProps.isAxisAreaChart || currProps.isRadarChart ? "" : "none";
        }

        //TableElement
        var showTableElementGroup = report && currentObject.typeComponent == "StiTableElement";
        if (showTableElementGroup) {
            if (!propertiesGroups.tableElementPropertiesGroup) propertiesPanel.places["TableElement"].appendChild(jsObject.TableElementPropertiesGroup());
        }
        if (propertiesGroups.tableElementPropertiesGroup) propertiesGroups.tableElementPropertiesGroup.style.display = showTableElementGroup ? "" : "none";
        if (showTableElementGroup) {
            properties.crossFilteringTable.style.display = currProps["crossFiltering"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["crossFiltering"] != null) controls.controlPropertyTableElementCrossFiltering.setChecked(currProps.crossFiltering);
            properties.groupTable.style.display = currProps["group"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["group"] != null) controls.controlPropertyTableElementGroup.value = currProps.group != "StiEmptyValue" ? currProps.group : "";
            properties.sizeMode.style.display = currProps["sizeMode"] != null ? "" : "none";
            if (currProps["sizeMode"] != null) controls.controlPropertyTableElementSizeMode.setKey(currProps.sizeMode != "StiEmptyValue" ? currProps.sizeMode : "");
            properties.frozenColumns.style.display = "none" //currProps["frozenColumns"] != null ? "" : "none"; //TO DO
            //if (currProps["frozenColumns"] != null) controls.controlPropertyTableElementFrozenColumns.value = currProps.frozenColumns != "StiEmptyValue" ? currProps.frozenColumns : "";
            properties.dataTransformationTable.style.display = levelDifficulty > 1 && currProps.name != "StiEmptyValue" && currProps["dataTransformation"] ? "" : "none";
        }

        //CardsElement
        var showCardsElementGroup = report && currentObject.typeComponent == "StiCardsElement";
        if (showCardsElementGroup) {
            if (!propertiesGroups.cardsElementPropertiesGroup) propertiesPanel.places["CardsElement"].appendChild(jsObject.CardsElementPropertiesGroup());
        }
        if (propertiesGroups.cardsElementPropertiesGroup) propertiesGroups.cardsElementPropertiesGroup.style.display = showCardsElementGroup ? "" : "none";
        if (showCardsElementGroup) {
            properties.crossFilteringCards.style.display = currProps["crossFiltering"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["crossFiltering"] != null) controls.controlPropertyCardsCrossFiltering.setChecked(currProps.crossFiltering);
            properties.groupCards.style.display = currProps["group"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["group"] != null) controls.controlPropertyCardsGroup.value = currProps.group != "StiEmptyValue" ? currProps.group : "";
            properties.cardsColorEach.style.display = currProps["cardsColorEach"] != null ? "" : "none";
            if (currProps["cardsColorEach"] != null) controls.controlPropertyCardsColorEach.setChecked(currProps.cardsColorEach);
            properties.cardsColumnCount.style.display = currProps["columnCount"] != null ? "" : "none";
            if (currProps["columnCount"] != null) controls.controlPropertyCardsColumnCount.value = currProps.columnCount != "StiEmptyValue" ? currProps.columnCount : "";
            properties.cardsOrientation.style.display = currProps["orientation"] != null ? "" : "none";
            if (currProps["orientation"] != null) controls.controlPropertyCardsOrientation.setKey(currProps.orientation != "StiEmptyValue" ? currProps.orientation : "");
            properties.cardsCornerRadius.style.display = currProps["cardsCornerRadius"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["cardsCornerRadius"] != null) controls.controlPropertyCardsCornerRadius.setValue(currProps.cardsCornerRadius == "StiEmptyValue" ? "" : currProps.cardsCornerRadius);
            properties.cardsMargin.style.display = currProps["cardsMargin"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["cardsMargin"] != null) controls.controlPropertyCardsMargin.setValue(currProps.cardsMargin == "StiEmptyValue" ? "" : currProps.cardsMargin);
            properties.cardsPadding.style.display = currProps["cardsPadding"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["cardsPadding"] != null) controls.controlPropertyCardsPadding.setValue(currProps.cardsPadding == "StiEmptyValue" ? "" : currProps.cardsPadding);
            properties.dataTransformationCards.style.display = levelDifficulty > 1 && currProps.name != "StiEmptyValue" && currProps["dataTransformation"] ? "" : "none";
        }

        //RegionMapElement
        var showRegionMapElementGroup = report && currentObject.typeComponent == "StiRegionMapElement";
        if (showRegionMapElementGroup) {
            if (!propertiesGroups.regionMapElementPropertiesGroup) propertiesPanel.places["RegionMapElement"].appendChild(jsObject.RegionMapElementPropertiesGroup());
        }
        if (propertiesGroups.regionMapElementPropertiesGroup) propertiesGroups.regionMapElementPropertiesGroup.style.display = showRegionMapElementGroup ? "" : "none";
        if (showRegionMapElementGroup) {
            properties.crossFilteringRegionMap.style.display = currProps["crossFiltering"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["crossFiltering"] != null) controls.controlPropertyRegionMapElementCrossFiltering.setChecked(currProps.crossFiltering);
            properties.groupRegionMap.style.display = currProps["group"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["group"] != null) controls.controlPropertyRegionMapElementGroup.value = currProps.group != "StiEmptyValue" ? currProps.group : "";
            properties.shortValueRegionMap.style.display = currProps["shortValue"] != null ? "" : "none";
            if (currProps["shortValue"] != null) controls.controlPropertyRegionMapElementShortValue.setChecked(currProps.shortValue);
            properties.showValueRegionMap.style.display = currProps["showValue"] != null ? "" : "none";
            if (currProps["showValue"] != null) controls.controlPropertyRegionMapElementShowValue.setChecked(currProps.showValue);
            properties.showZerosRegionMap.style.display = currProps["showZeros"] != null ? "" : "none";
            if (currProps["showZeros"] != null) controls.controlPropertyRegionMapElementShowZeros.setChecked(currProps.showZeros);
            properties.dataTransformationRegionMap.style.display = levelDifficulty > 1 && currProps.name != "StiEmptyValue" && currProps["dataTransformation"] ? "" : "none";
        }

        //OnlineMapElement
        var showOnlineMapElementGroup = report && currentObject.typeComponent == "StiOnlineMapElement";
        if (showOnlineMapElementGroup) {
            if (!propertiesGroups.onlineMapElementPropertiesGroup) propertiesPanel.places["OnlineMapElement"].appendChild(jsObject.OnlineMapElementPropertiesGroup());
        }
        if (propertiesGroups.onlineMapElementPropertiesGroup) propertiesGroups.onlineMapElementPropertiesGroup.style.display = showOnlineMapElementGroup ? "" : "none";
        if (showOnlineMapElementGroup) {
            properties.crossFilteringOnlineMap.style.display = currProps["crossFiltering"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["crossFiltering"] != null) controls.onlineMapElementCrossFiltering.setChecked(currProps.crossFiltering);
            properties.groupOnlineMap.style.display = currProps["group"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["group"] != null) controls.controlPropertyOnlineMapElementGroup.value = currProps.group != "StiEmptyValue" ? currProps.group : "";
            properties.dataTransformationOnlineMap.style.display = levelDifficulty > 1 && currProps.name != "StiEmptyValue" && currProps["dataTransformation"] ? "" : "none";
        }

        //PivotTableElement
        var showPivotTableElementGroup = report && currentObject.typeComponent == "StiPivotTableElement";
        if (showPivotTableElementGroup) {
            if (!propertiesGroups.pivotTableElementPropertiesGroup) propertiesPanel.places["PivotTableElement"].appendChild(jsObject.PivotTableElementPropertiesGroup());
        }
        if (propertiesGroups.pivotTableElementPropertiesGroup) propertiesGroups.pivotTableElementPropertiesGroup.style.display = showPivotTableElementGroup ? "" : "none";
        if (showPivotTableElementGroup) {
            properties.crossFilteringPivotTable.style.display = currProps["crossFiltering"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["crossFiltering"] != null) controls.controlPropertyPivotTableCrossFiltering.setChecked(currProps.crossFiltering);
            properties.groupPivotTable.style.display = currProps["group"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["group"] != null) controls.controlPropertyPivotTableElementGroup.value = currProps.group != "StiEmptyValue" ? currProps.group : "";
            properties.dataTransformationPivotTable.style.display = levelDifficulty > 1 && currProps.name != "StiEmptyValue" && currProps["dataTransformation"] ? "" : "none";
        }

        //ComboBoxElement
        var showComboBoxElementGroup = report && currentObject.typeComponent == "StiComboBoxElement";
        if (showComboBoxElementGroup) {
            if (!propertiesGroups.comboBoxElementPropertiesGroup) propertiesPanel.places["ComboBoxElement"].appendChild(jsObject.ComboBoxElementPropertiesGroup());
        }
        if (propertiesGroups.comboBoxElementPropertiesGroup) propertiesGroups.comboBoxElementPropertiesGroup.style.display = showComboBoxElementGroup ? "" : "none";
        if (showComboBoxElementGroup) {
            properties.groupComboBox.style.display = currProps["group"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["group"] != null) controls.controlPropertyComboBoxElementGroup.value = currProps.group != "StiEmptyValue" ? currProps.group : "";
            properties.dataTransformationComboBox.style.display = levelDifficulty > 1 && currProps.name != "StiEmptyValue" && currProps["dataTransformation"] ? "" : "none";
        }

        //ListBoxElement
        var showListBoxElementGroup = report && currentObject.typeComponent == "StiListBoxElement";
        if (showListBoxElementGroup) {
            if (!propertiesGroups.listBoxElementPropertiesGroup) propertiesPanel.places["ListBoxElement"].appendChild(jsObject.ListBoxElementPropertiesGroup());
        }
        if (propertiesGroups.listBoxElementPropertiesGroup) propertiesGroups.listBoxElementPropertiesGroup.style.display = showListBoxElementGroup ? "" : "none";
        if (showListBoxElementGroup) {
            properties.groupListBox.style.display = currProps["group"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["group"] != null) controls.controlPropertyListBoxElementGroup.value = currProps.group != "StiEmptyValue" ? currProps.group : "";
            properties.dataTransformationListBox.style.display = levelDifficulty > 1 && currProps.name != "StiEmptyValue" && currProps["dataTransformation"] ? "" : "none";
        }

        //TreeViewBoxElement
        var showTreeViewBoxElementGroup = report && currentObject.typeComponent == "StiTreeViewBoxElement";
        if (showTreeViewBoxElementGroup) {
            if (!propertiesGroups.treeViewBoxElementPropertiesGroup) propertiesPanel.places["TreeViewBoxElement"].appendChild(jsObject.TreeViewBoxElementPropertiesGroup());
        }
        if (propertiesGroups.treeViewBoxElementPropertiesGroup) propertiesGroups.treeViewBoxElementPropertiesGroup.style.display = showTreeViewBoxElementGroup ? "" : "none";
        if (showTreeViewBoxElementGroup) {
            properties.groupTreeViewBox.style.display = currProps["group"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["group"] != null) controls.controlPropertyTreeViewBoxElementGroup.value = currProps.group != "StiEmptyValue" ? currProps.group : "";
            properties.dataTransformationTreeViewBox.style.display = levelDifficulty > 1 && currProps.name != "StiEmptyValue" && currProps["dataTransformation"] ? "" : "none";
        }

        //TreeViewElement
        var showTreeViewElementGroup = report && currentObject.typeComponent == "StiTreeViewElement";
        if (showTreeViewElementGroup) {
            if (!propertiesGroups.treeViewElementPropertiesGroup) propertiesPanel.places["TreeViewElement"].appendChild(jsObject.TreeViewElementPropertiesGroup());
        }
        if (propertiesGroups.treeViewElementPropertiesGroup) propertiesGroups.treeViewElementPropertiesGroup.style.display = showTreeViewElementGroup ? "" : "none";
        if (showTreeViewElementGroup) {
            properties.groupTreeView.style.display = currProps["group"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["group"] != null) controls.controlPropertyTreeViewElementGroup.value = currProps.group != "StiEmptyValue" ? currProps.group : "";
            properties.dataTransformationTreeView.style.display = levelDifficulty > 1 && currProps.name != "StiEmptyValue" && currProps["dataTransformation"] ? "" : "none";
        }

        //DatePickerElement
        var showDatePickerElementGroup = report && currentObject.typeComponent == "StiDatePickerElement";
        if (showDatePickerElementGroup) {
            if (!propertiesGroups.datePickerElementPropertiesGroup) propertiesPanel.places["DatePickerElement"].appendChild(jsObject.DatePickerElementPropertiesGroup());
        }
        if (propertiesGroups.datePickerElementPropertiesGroup) propertiesGroups.datePickerElementPropertiesGroup.style.display = showDatePickerElementGroup ? "" : "none";
        if (showDatePickerElementGroup) {
            properties.groupDatePicker.style.display = currProps["group"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["group"] != null) controls.controlPropertyDatePickerElementGroup.value = currProps.group != "StiEmptyValue" ? currProps.group : "";
            properties.dataTransformationDatePicker.style.display = levelDifficulty > 1 && currProps.name != "StiEmptyValue" && currProps["dataTransformation"] ? "" : "none";
        }

        //IndicatorElement
        var showIndicatorElementGroup = report && currentObject.typeComponent == "StiIndicatorElement";
        if (showIndicatorElementGroup) {
            if (!propertiesGroups.indicatorElementPropertiesGroup) propertiesPanel.places["IndicatorElement"].appendChild(jsObject.IndicatorElementPropertiesGroup());
        }
        if (propertiesGroups.indicatorElementPropertiesGroup) propertiesGroups.indicatorElementPropertiesGroup.style.display = showIndicatorElementGroup ? "" : "none";
        if (showIndicatorElementGroup) {
            properties.crossFilteringIndicator.style.display = currProps["crossFiltering"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["crossFiltering"] != null) controls.controlPropertyIndicatorElementCrossFiltering.setChecked(currProps.crossFiltering);
            properties.groupIndicator.style.display = currProps["group"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["group"] != null) controls.controlPropertyIndicatorElementGroup.value = currProps.group != "StiEmptyValue" ? currProps.group : "";
            var showTargetMode = currProps["isTargetPresent"] && currProps["targetMode"] != null;
            properties.targetMode.style.display = showTargetMode ? "" : "none";
            if (showTargetMode) controls.controlPropertyIndicatorElementTargetMode.setKey(currProps.targetMode);
            properties.dataTransformationIndicator.style.display = levelDifficulty > 1 && currProps.name != "StiEmptyValue" && currProps["dataTransformation"] ? "" : "none";
        }

        //ProgressElement
        var showProgressElementGroup = report && currentObject.typeComponent == "StiProgressElement";
        if (showProgressElementGroup) {
            if (!propertiesGroups.progressElementPropertiesGroup) propertiesPanel.places["ProgressElement"].appendChild(jsObject.ProgressElementPropertiesGroup());
        }
        if (propertiesGroups.progressElementPropertiesGroup) propertiesGroups.progressElementPropertiesGroup.style.display = showProgressElementGroup ? "" : "none";
        if (showProgressElementGroup) {
            properties.crossFilteringProgress.style.display = currProps["crossFiltering"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["crossFiltering"] != null) controls.controlPropertyProgressElementCrossFiltering.setChecked(currProps.crossFiltering);
            properties.groupProgress.style.display = currProps["group"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["group"] != null) controls.controlPropertyProgressElementGroup.value = currProps.group != "StiEmptyValue" ? currProps.group : "";
            var showColorEach = currProps["isSeriesPresent"] && currProps["colorEach"] != null;
            properties.colorEach.style.display = showColorEach ? "" : "none";
            if (showColorEach) controls.controlPropertyProgressElementColorEach.setChecked(currProps.colorEach);
            properties.dataTransformationProgress.style.display = levelDifficulty > 1 && currProps.name != "StiEmptyValue" && currProps["dataTransformation"] ? "" : "none";
        }

        //GaugeElement
        var showGaugeElementGroup = report && currentObject.typeComponent == "StiGaugeElement";
        if (showGaugeElementGroup) {
            if (!propertiesGroups.gaugeElementPropertiesGroup) propertiesPanel.places["GaugeElement"].appendChild(jsObject.GaugeElementPropertiesGroup());
        }
        if (propertiesGroups.gaugeElementPropertiesGroup) propertiesGroups.gaugeElementPropertiesGroup.style.display = showGaugeElementGroup ? "" : "none";
        if (showGaugeElementGroup) {
            properties.crossFilteringGauge.style.display = currProps["crossFiltering"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["crossFiltering"] != null) controls.controlPropertyGaugeElementCrossFiltering.setChecked(currProps.crossFiltering);
            properties.groupGauge.style.display = currProps["group"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["group"] != null) controls.controlPropertyGaugeElementGroup.value = currProps.group != "StiEmptyValue" ? currProps.group : "";
            properties.labelsVisibleGauge.style.display = currProps["labelsVisible"] != null ? "" : "none";
            if (currProps["labelsVisible"] != null) controls.controlPropertyGaugeElementLabelsVisible.setChecked(currProps.labelsVisible);
            properties.labelsPlacementGauge.style.display = currProps["labelsPlacement"] != null ? "" : "none";
            if (currProps["labelsPlacement"] != null) controls.controlPropertyGaugeElementLabelsPlacement.setKey(currProps.labelsPlacement);
            properties.targetSettingsShowLabel.style.display = currProps["targetSettingsShowLabel"] != null ? "" : "none";
            if (currProps["targetSettingsShowLabel"] != null) controls.gaugeElementTargetShowLabel.setChecked(currProps.targetSettingsShowLabel);
            properties.targetSettingsPlacement.style.display = currProps["targetSettingsPlacement"] != null ? "" : "none";
            if (currProps["targetSettingsPlacement"] != null) controls.gaugeElementTargetPlacement.setKey(currProps.targetSettingsPlacement);

            properties.dataTransformationGauge.style.display = levelDifficulty > 1 && currProps.name != "StiEmptyValue" && currProps["dataTransformation"] ? "" : "none";
        }

        //TextElement
        var showTextElementGroup = report && currentObject.typeComponent == "StiTextElement" && currProps.name != "StiEmptyValue";
        if (showTextElementGroup) {
            if (!propertiesGroups.textElementPropertiesGroup) propertiesPanel.places["TextElement"].appendChild(jsObject.TextElementPropertiesGroup());
        }
        if (propertiesGroups.textElementPropertiesGroup) propertiesGroups.textElementPropertiesGroup.style.display = showTextElementGroup ? "" : "none";
        if (showTextElementGroup) {
            properties.crossFilteringText.style.display = currProps["crossFiltering"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["crossFiltering"] != null) controls.textElementCrossFiltering.setChecked(currProps.crossFiltering);
            properties.groupText.style.display = currProps["group"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["group"] != null) controls.textElementGroup.value = currProps.group != "StiEmptyValue" ? currProps.group : "";
            properties.textSizeMode.style.display = currProps["textSizeMode"] != null && currProps.textSizeMode != "StiEmptyValue" ? "" : "none";
            if (currProps["textSizeMode"] != null) controls.textElementSizeMode.setKey(currProps.textSizeMode);

            properties.textElementText.style.display = currProps["text"] != null ? "" : "none";
            properties.textElementText.propertyControl.textBox.value = "[" + jsObject.loc.PropertyMain.Text + "]";
        }

        //ImageElement
        var showImageElementGroup = report && currentObject.typeComponent == "StiImageElement" && currProps.name != "StiEmptyValue";
        if (showImageElementGroup) {
            if (!propertiesGroups.imageElementPropertiesGroup) propertiesPanel.places["ImageElement"].appendChild(jsObject.ImageElementPropertiesGroup());
        }
        if (propertiesGroups.imageElementPropertiesGroup) propertiesGroups.imageElementPropertiesGroup.style.display = showImageElementGroup ? "" : "none";
        if (showImageElementGroup) {
            properties.crossFilteringImage.style.display = currProps["crossFiltering"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["crossFiltering"] != null) controls.imageElementCrossFiltering.setChecked(currProps.crossFiltering);
            properties.groupImage.style.display = currProps["group"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["group"] != null) controls.imageElementGroup.value = currProps.group != "StiEmptyValue" ? currProps.group : "";
            properties.imageElementHorAlignment.propertyControl.setKey(currProps.horAlignment);
            properties.imageElementVertAlignment.propertyControl.setKey(currProps.vertAlignment);
            properties.imageElementAspectRatio.propertyControl.setChecked(currProps.ratio);
        }

        //ButtonElement
        var showButtonElementGroup = report && currentObject.typeComponent == "StiButtonElement";
        if (showButtonElementGroup) {
            if (!propertiesGroups.buttonElementPropertiesGroup) propertiesPanel.places["ButtonElement"].appendChild(jsObject.ButtonElementPropertiesGroup());
        }
        if (propertiesGroups.buttonElementPropertiesGroup) propertiesGroups.buttonElementPropertiesGroup.style.display = showButtonElementGroup ? "" : "none";
        if (showButtonElementGroup) {
            properties.buttonText.style.display = currProps["buttonText"] != null ? "" : "none";
            if (currProps["buttonText"] != null) controls.controlButtonText.value = currProps.buttonText != "StiEmptyValue" ? StiBase64.decode(currProps.buttonText) : "";
            properties.buttonChecked.style.display = currProps["buttonChecked"] != null && currProps.buttonType != "Button" ? "" : "none";
            if (currProps["buttonChecked"] != null && currProps.buttonType != "Button") controls.controlButtonChecked.setChecked(currProps.buttonChecked);
            properties.buttonGroup.style.display = currProps["buttonGroup"] != null && currProps.buttonType == "RadioButton" ? "" : "none";
            if (currProps["buttonGroup"] != null && currProps.buttonType == "RadioButton") controls.controlButtonGroup.value = currProps.buttonGroup;
            properties.buttonIconAlignment.style.display = currProps["iconAlignment"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["iconAlignment"] != null) controls.controlButtonIconAlignment.setKey(currProps.iconAlignment);
            properties.buttonHorAlignment.style.display = currProps["horAlignment"] != null ? "" : "none";
            if (currProps["horAlignment"] != null) controls.controlButtonHorAlignment.setKey(currProps.horAlignment);
            properties.buttonVertAlignment.style.display = currProps["vertAlignment"] != null ? "" : "none";
            if (currProps["vertAlignment"] != null) controls.controlButtonVertAlignment.setKey(currProps.vertAlignment);
            properties.buttonType.style.display = currProps["buttonType"] != null ? "" : "none";
            if (currProps["buttonType"] != null) controls.controlButtonType.setKey(currProps.buttonType);
            properties.buttonShapeType.style.display = currProps["buttonShapeType"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["buttonShapeType"] != null) controls.controlButtonShapeType.setKey(currProps.buttonShapeType);
            properties.buttonStretch.style.display = currProps["buttonStretch"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["buttonStretch"] != null) controls.controlButtonStretch.setKey(currProps.buttonStretch);
            properties.buttonWordWrap.style.display = currProps["wordWrap"] != null && levelDifficulty > 1 ? "" : "none";
            if (currProps["wordWrap"] != null) controls.controlButtonWordWrap.setChecked(currProps.wordWrap);

            var showIconSet = currProps["buttonIconSet"] != null;
            propertiesGroups.buttonElementPropertiesGroup.innerGroups.iconSet.style.display = showIconSet && levelDifficulty > 1 ? "" : "none";

            properties.buttonIconSetIcon.style.display = showIconSet ? "" : "none";
            if (showIconSet) controls.controlButtonIconSetIcon.setKey(currProps.buttonIconSet.icon);
            properties.buttonIconSetCheckedIcon.style.display = showIconSet ? "" : "none";
            if (showIconSet) controls.controlButtonIconSetCheckedIcon.setKey(currProps.buttonIconSet.checkedIcon);
            properties.buttonIconSetUncheckedIcon.style.display = showIconSet ? "" : "none";
            if (showIconSet) controls.controlButtonIconSetUncheckedIcon.setKey(currProps.buttonIconSet.uncheckedIcon);
        }

        //Chart 
        var showChart = report && currentObject.typeComponent && currentObject.typeComponent == "StiChart" && jsObject.options.selectedObject;
        if (showChart) designButtonBlock.style.display = "";

        //Sparkline
        var showSparkline = report && currentObject.typeComponent && currentObject.typeComponent == "StiSparkline" && jsObject.options.selectedObject;
        if (showSparkline) designButtonBlock.style.display = "";

        //Chart Additional
        if (showChart && !propertiesGroups.chartAdditionalPropertiesGroup) propertiesPanel.places["ChartAdditional"].appendChild(jsObject.ChartAdditionalPropertiesGroup());
        if (propertiesGroups.chartAdditionalPropertiesGroup) propertiesGroups.chartAdditionalPropertiesGroup.style.display = showChart && levelDifficulty > 1 ? "" : "none";
        if (showChart) {
            properties.allowApplyStyle.style.display = currProps["allowApplyStyle"] != null && levelDifficulty > 2 ? "" : "none";
            if (currProps["allowApplyStyle"] != null) controls.controlPropertyChartAllowApplyStyle.setChecked(currProps.allowApplyStyle);
            properties.horizontalSpacing.style.display = currProps["horizontalSpacing"] != null ? "" : "none";
            if (currProps["horizontalSpacing"] != null) controls.controlPropertyHorSpacing.value = currProps.horizontalSpacing != "StiEmptyValue" ? currProps.horizontalSpacing : "";
            properties.processAtEnd.style.display = currProps["processAtEnd"] != null ? "" : "none";
            if (currProps["processAtEnd"] != null) controls.controlPropertyChartProcessAtEnd.setChecked(currProps.processAtEnd);
            properties.chartRotation.style.display = currProps["chartRotation"] != null && levelDifficulty > 2 ? "" : "none";
            if (currProps["chartRotation"] != null) controls.controlPropertyChartRotation.setKey(currProps.chartRotation != "StiEmptyValue" ? currProps.chartRotation : "");
            properties.verticalSpacing.style.display = currProps["verticalSpacing"] != null ? "" : "none";
            if (currProps["verticalSpacing"] != null) controls.controlPropertyVertSpacing.value = currProps.verticalSpacing != "StiEmptyValue" ? currProps.verticalSpacing : "";
        }

        //CrossTab 
        var showCrossTab = report && currentObject.typeComponent && currentObject.typeComponent == "StiCrossTab" && jsObject.options.selectedObject;
        if (showCrossTab) designButtonBlock.style.display = "";

        //SubReport
        var showSubReport = report && currentObject.typeComponent && currentObject.typeComponent == "StiSubReport";
        if (showSubReport) designButtonBlock.style.display = "";

        //PdfSignature
        var showPdfSignatureGroup = report && currentObject.typeComponent == "StiPdfDigitalSignature";
        if (showPdfSignatureGroup) {
            if (!propertiesGroups.pdfSignaturePropertiesGroup) propertiesPanel.places["PdfSignature"].appendChild(jsObject.PdfSignaturePropertiesGroup());
        }
        if (propertiesGroups.pdfSignaturePropertiesGroup) propertiesGroups.pdfSignaturePropertiesGroup.style.display = showPdfSignatureGroup ? "" : "none";
        if (showPdfSignatureGroup) {
            properties.placeholder.style.display = currProps["placeholder"] != null ? "" : "none";
            if (currProps["placeholder"] != null) controls.controlPropertyPlaceholder.value = StiBase64.decode(currProps.placeholder);
        }

        //Export Group
        var showExport = report && currentObject && (currProps["excelValue"] != null || currProps["excelSheet"] != null || currProps["exportAsImage"] != null) && levelDifficulty > 2;
        if (showExport && !propertiesGroups.exportPropertiesGroup) propertiesPanel.places["Export"].appendChild(jsObject.ExportPropertiesGroup());
        if (propertiesGroups.exportPropertiesGroup) propertiesGroups.exportPropertiesGroup.style.display = showExport ? "" : "none";
        if (showExport) {
            properties.excelValue.style.display = currProps["excelValue"] != null ? "" : "none";
            if (currProps["excelValue"] != null) controls.controlPropertyExcelValue.value = currProps.excelValue != "StiEmptyValue" ? StiBase64.decode(currProps.excelValue) : "";
            properties.excelSheet.style.display = currProps["excelSheet"] != null ? "" : "none";
            if (currProps["excelSheet"] != null) controls.controlPropertyExcelSheet.value = currProps.excelSheet != "StiEmptyValue" ? StiBase64.decode(currProps.excelSheet) : "";
            properties.exportAsImage.style.display = currProps["exportAsImage"] != null ? "" : "none";
            if (currProps["exportAsImage"] != null) controls.controlPropertyExportAsImage.setChecked(currProps.exportAsImage);
        }

        //Report
        var showReportGroup = report && currentObject.typeComponent == "StiReport";
        var onlyDbs = report && !report.pagesPresent() && report.dashboardsPresent();

        if (showReportGroup) {
            if (!propertiesGroups.reportDescriptionPropertiesGroup)
                propertiesPanel.places["ReportDescription"].appendChild(jsObject.ReportDescriptionPropertiesGroup());
            if (!propertiesGroups.reportDataPropertiesGroup)
                propertiesPanel.places["ReportData"].appendChild(jsObject.ReportDataPropertiesGroup());
            if (!propertiesGroups.reportGlobalizationPropertiesGroup)
                propertiesPanel.places["ReportGlobalization"].appendChild(jsObject.ReportGlobalizationPropertiesGroup());
            if (!propertiesGroups.reportEnginePropertiesGroup)
                propertiesPanel.places["ReportEngine"].appendChild(jsObject.ReportEnginePropertiesGroup());
            if (!propertiesGroups.reportViewPropertiesGroup)
                propertiesPanel.places["ReportView"].appendChild(jsObject.ReportViewPropertiesGroup());
        }

        var reportDescriptionGroup = propertiesGroups.reportDescriptionPropertiesGroup;
        var reportDataGroup = propertiesGroups.reportDataPropertiesGroup;
        var reportGlobalizationGroup = propertiesGroups.reportGlobalizationPropertiesGroup;
        var reportEngineGroup = propertiesGroups.reportEnginePropertiesGroup;
        var reportViewGroup = propertiesGroups.reportViewPropertiesGroup;

        if (reportDescriptionGroup) reportDescriptionGroup.style.display = showReportGroup ? "" : "none";
        if (reportDataGroup) reportDataGroup.style.display = showReportGroup && !onlyDbs ? "" : "none";
        if (reportGlobalizationGroup) reportGlobalizationGroup.style.display = showReportGroup ? "" : "none";
        if (reportEngineGroup) reportEngineGroup.style.display = showReportGroup ? "" : "none";
        if (reportViewGroup) reportViewGroup.style.display = showReportGroup ? "" : "none";

        if (showReportGroup) {
            reportDescriptionGroup.changeOpenedState(true);
            reportDataGroup.changeOpenedState(true);
            reportGlobalizationGroup.changeOpenedState(true);
            reportEngineGroup.changeOpenedState(true);
            reportViewGroup.changeOpenedState(true);
            controls["controlReportPropertyReportUnit"].setKey(currProps["reportUnit"]);

            var propertyNames = ["ReportName", "ReportAlias", "ReportAuthor", "ReportDescription", "ReportImage", "AutoLocalizeReportOnRun", "CacheAllData", "CacheTotals",
                "CalculationMode", "ConvertNulls", "Collate", "Culture", "EngineVersion", "NumberOfPass", "PreviewMode", "RefreshTime", "ReportCacheMode",
                "ParametersOrientation", "RequestParameters", "RetrieveOnlyUsedData", "ScriptLanguage", "StopBeforePage", "StoreImagesInResources", "ParametersDateFormat", "ParameterWidth"];

            for (var i = 0; i < propertyNames.length; i++) {
                var controlProperty = controls["controlReportProperty" + propertyNames[i]];
                var propertyValue = currProps[jsObject.LowerFirstChar(propertyNames[i])];
                if (controlProperty) {
                    controlProperty.setValue(propertyValue);
                    if (propertyNames[i] == "RefreshTime") {
                        controlProperty.textBox.value = controlProperty.key;
                    }
                }
            }

            var propertiesNoDbs = ["cacheAllData", "convertNulls", "reportCacheMode", "cacheTotals", "collate", "numberOfPass", "reportUnit", "scriptLanguage",
                "stopBeforePage", "retrieveOnlyUsedData", "engineVersion"];

            for (var i = 0; i < propertiesNoDbs.length; i++) {
                if (jsObject.options.properties[propertiesNoDbs[i]]) {
                    jsObject.options.properties[propertiesNoDbs[i]].style.display = onlyDbs ? "none" : "";
                }
            }

            if (jsObject.options.cloudMode || jsObject.options.serverMode) {
                properties.reportName.style.display = properties.reportAlias.style.display = properties.reportAuthor.style.display = properties.reportDescription.style.display = "none";
            }

            if (jsObject.options.jsMode) {
                properties.reportImage.style.display = "none";

                if (reportEngineGroup && onlyDbs) {
                    reportEngineGroup.style.display = "none";
                }
            }
        }

        //Set Design Button Image        
        if (report && designButtonBlock.style.display == "") {
            if (currentObject.typeComponent && StiMobileDesigner.checkImageSource(jsObject.options, currentObject.typeComponent + ".png")) {
                designButtonBlock.image.style.display = "";
                StiMobileDesigner.setImageSource(designButtonBlock.image, jsObject.options, currentObject.typeComponent + ".png");
            }
            else {
                designButtonBlock.image.style.display = "none";
            }
        }

        //Components List
        var compListValue = "";
        var selectedObject = jsObject.options.selectedObject;

        if (selectedObject) {
            if (selectedObject.typeComponent == "StiReport") {
                compListValue = jsObject.ExtractBase64Value(jsObject.options.report.properties.reportName) + " : " + jsObject.loc.Components.StiReport;
            }
            else {
                compListValue = selectedObject.properties.name;
                var componentType = "";

                if (selectedObject.properties.cellType) {
                    componentType = jsObject.loc.Components["Sti" + selectedObject.properties.cellType];
                }
                else {
                    componentType = jsObject.loc.Components[selectedObject.typeComponent];
                }

                if (componentType) compListValue += " : " + componentType;
            }
        }

        componentsList.setKey(compListValue);
        propertiesPanel.openFirstGroup();
    }

    propertiesPanel.updatePropertiesCaptions = function () {
        for (var propertyName in jsObject.options.properties) {
            var property = jsObject.options.properties[propertyName];
            if (property.caption) {
                property.caption.innerHTML = this.localizePropertyGrid ? property.captionText : property.getOriginalPropertyName();
            }
        }
        if (this.editChartPropertiesPanel) this.editChartPropertiesPanel.updatePropertiesCaptions();
        if (this.eventsPropertiesPanel) this.eventsPropertiesPanel.updatePropertiesCaptions();
        if (jsObject.options.forms.styleDesignerForm) jsObject.options.forms.styleDesignerForm.propertiesPanel.updatePropertiesCaptions();
    }

    propertiesPanel.updatePropertiesValues = function () {
        var localizePropertyGrid = jsObject.options.propertiesPanel.localizePropertyGrid;

        for (var propertyName in jsObject.options.properties) {
            var property = jsObject.options.properties[propertyName];
            var propertyControl = property.propertyControl;
            if (propertyControl && propertyControl.ownerIsProperty) {
                //Dropdown lists
                if (propertyControl.items && propertyControl.menu && propertyControl.addItems) {
                    var items = propertyControl.items;
                    if (items.length > 0) {
                        for (var i = 0; i < items.length; i++) {
                            if (localizePropertyGrid) {
                                if (items[i].captionText != null) propertyControl.items[i].caption = items[i].captionText;
                            }
                            else {
                                items[i].captionText = items[i].caption;
                                items[i].caption = items[i].key;
                            }
                        }
                        propertyControl.addItems(items);
                    }
                }
                if (propertyControl.setKey && propertyControl.key != null) {
                    propertyControl.setKey(propertyControl.key);
                }
            }
        }
    }

    propertiesPanel.openFirstGroup = function () {
        for (var i = 0; i < propertiesPanel.mainPropertiesPanel.childNodes.length; i++) {
            var propertiesGroup = propertiesPanel.mainPropertiesPanel.childNodes[i];
            if (propertiesGroup.firstChild && propertiesGroup.firstChild.style.display == "" && propertiesGroup.firstChild["changeOpenedState"] != null) {
                if (!propertiesGroup.firstChild.openedByUser) {
                    propertiesGroup.firstChild.changeOpenedState(true);
                }
                break;
            }
        }
    }

    propertiesPanel.updateBySpecification = function () {
        if (jsObject.options.cloudMode || jsObject.options.standaloneJsMode) {
            var specification = jsObject.options.designerSpecification;
            this.propertiesToolBar.controls.PropertiesTab.style.display = specification == "Developer" ? "" : "none";
            this.propertiesToolBar.controls.EventsTab.style.display = specification != "Developer" || jsObject.options.isJava ? "none" : "";
            this.updateControls();
        }
    }

    var lastContainerName = "Dictionary";

    if (this.options.showPropertiesGrid && (lastContainerName == "Properties" || !lastContainerName || (!this.options.showDictionary && this.options.showPropertiesGrid)))
        propertiesPanel.showContainer("Properties");
    else if (this.options.showDictionary && (lastContainerName == "Dictionary" || !lastContainerName || (!this.options.showPropertiesGrid && !this.options.showDictionary)))
        propertiesPanel.showContainer("Dictionary")
    else if (this.options.showReportTree && (lastContainerName == "ReportTree" || !lastContainerName || (!this.options.showPropertiesGrid && !this.options.showDictionary && this.options.showReportTree)))
        propertiesPanel.showContainer("ReportTree");

    if (this.options.dictionaryPanel)
        this.options.dictionaryPanel.checkDataHintItemVisibility();
}

StiMobileDesigner.prototype.PropertiesPanelHeader = function (propertiesPanel) {
    var header = document.createElement("div");
    header.className = "stiDesignerPropertiesPanelHeader";

    var headerTable = this.CreateHTMLTable();
    header.appendChild(headerTable);
    headerTable.style.height = "100%";
    headerTable.style.width = "100%";

    propertiesPanel.headerCaption = headerTable.addCell();
    propertiesPanel.headerCaption.style.width = "100%";
    propertiesPanel.headerCaption.style.paddingLeft = this.options.isTouchDevice ? "10px" : "8px";
    propertiesPanel.headerCaption.innerHTML = this.loc.Panels.Properties;

    //Properties Toolbar
    var toolBarProps = this.CreateHTMLTable();
    toolBarProps.style.height = "100%";
    propertiesPanel.propertiesToolBar = toolBarProps;
    toolBarProps.controls = {};
    headerTable.addCell(toolBarProps);

    var buttons = [
        ["PropertiesTab", this.loc.Report.PropertiesTab, "PropertiesTab.png"],
        ["EventsTab", this.loc.Report.EventsTab, "EventsTab.png"],
        ["Settings", this.loc.Export.Settings, "Settings.png"]
    ]

    for (var i = 0; i < buttons.length; i++) {
        var button = this.StandartSmallButton("propertiesToolbar" + buttons[i][0], null, null, buttons[i][2], buttons[i][1]);
        toolBarProps.controls[buttons[i][0]] = button;
        button.style.marginRight = this.options.isTouchDevice ? "3px" : "5px";
        toolBarProps.addCell(button);
    }

    if (this.options.isJava) {
        toolBarProps.controls.EventsTab.style.display = 'none';
    }

    toolBarProps.controls.PropertiesTab.setSelected(true);
    toolBarProps.controls.PropertiesTab.action = function () { propertiesPanel.setEventsMode(false); }
    toolBarProps.controls.EventsTab.action = function () { propertiesPanel.setEventsMode(true); }

    var localizeState = this.GetCookie("StimulsoftMobileDesignerLocalizePropertyGrid");
    propertiesPanel.localizePropertyGrid = localizeState ? localizeState == "true" : true;

    var propertiesSettingsMenu = this.InitializePropertiesSettingsMenu();

    toolBarProps.controls.Settings.action = function () {
        propertiesSettingsMenu.changeVisibleState(!propertiesSettingsMenu.visible);
    }

    toolBarProps.changeVisibleState = function (state) {
        toolBarProps.style.display = state ? "" : "none";
    }

    //Hide button
    propertiesPanel.hideButton = this.StandartSmallButton("hidePropertiesPanelButton", null, null, "HidePanel.png");
    propertiesPanel.hideButton.style.marginRight = this.options.isTouchDevice ? "3px" : "5px";
    headerTable.addCell(propertiesPanel.hideButton);

    propertiesPanel.hideButton.action = function () {
        propertiesPanel.fixedViewMode = !propertiesPanel.fixedViewMode;
        propertiesPanel.changeVisibleState(!propertiesPanel.fixedViewMode);
        propertiesPanel.showButtonsPanel.changeVisibleState(propertiesPanel.fixedViewMode);
    }

    return header;
}

StiMobileDesigner.prototype.PropertiesPanelFooter = function () {
    var jsObject = this;
    var propertiesPanel = jsObject.options.propertiesPanel;
    var footerTable = this.CreateHTMLTable();
    footerTable.style.height = "100%";

    var buttonProps = [
        ["PropertiesTabButton", this.loc.Panels.Properties, "Properties.png", this.options.showPropertiesGrid],
        ["DictionaryTabButton", this.loc.Panels.Dictionary, "Dictionary.png", this.options.showDictionary],
        ["ReportTreeTabButton", this.loc.Panels.ReportTree, "ReportTree.png", this.options.showReportTree]
    ]
    var buttons = {};

    for (var i = 0; i < buttonProps.length; i++) {
        var button = buttons[buttonProps[i][0]] = this.TabButton(buttonProps[i][0], "PropertiesGridTabs", buttonProps[i][1], buttonProps[i][2]);
        button.style.margin = "0 0 0 3px";

        var tubButtonCell = footerTable.addCell(button);
        //tubButtonCell.style.verticalAlign = "top";
        tubButtonCell.style.display = buttonProps[i][3] ? "" : "none";
    }

    buttons["PropertiesTabButton"].action = function () {
        if (!this.isSelected) propertiesPanel.showContainer("Properties");
    }

    buttons["DictionaryTabButton"].action = function () {
        if (!this.isSelected) propertiesPanel.showContainer("Dictionary");
    }

    buttons["ReportTreeTabButton"].action = function () {
        if (!this.isSelected) propertiesPanel.showContainer("ReportTree");
    }

    return footerTable;
}

StiMobileDesigner.prototype.PropertiesPanelShowButtonsPanel = function (propertiesPanel) {
    var jsObject = this;
    var buttonsPanel = document.createElement("div");
    buttonsPanel.className = "stiDesignerPropertiesPanelShowButtonsPanel";
    buttonsPanel.style.display = "none";
    buttonsPanel.style.zIndex = 2;
    buttonsPanel.style.top = (this.options.toolBar.offsetHeight + this.options.workPanel.offsetHeight + this.options.infoPanel.offsetHeight + 40) + "px";
    buttonsPanel.style.left = (this.options.toolbox ? this.options.toolbox.offsetWidth : 0) + "px";
    var jsObject = this;

    buttonsPanel.onmousedown = function () {
        if (this.isTouchStartFlag) return;
        this.ontouchstart(true);
    }

    buttonsPanel.ontouchstart = function (mouseProcess) {
        var this_ = this;
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        jsObject.options.propertiesPanelPressed = true;
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    var buttonProps = [
        ["Properties", this.options.showPropertiesGrid],
        ["Dictionary", this.options.showDictionary],
        ["ReportTree", this.options.showReportTree]
    ]

    for (var i = 0; i < buttonProps.length; i++) {
        if (buttonProps[i][1]) {
            var button = this.SmallButton("show" + buttonProps[i][0] + "PanelButton", "PropertiesPanelGroup", null, null, null, null, "stiDesignerVerticalButton", true);
            button.innerHTML = this.loc.Panels[buttonProps[i][0]];
            button.panelName = buttonProps[i][0];
            buttonsPanel.appendChild(button);

            if ((buttonProps[i][0] == "Dictionary" && this.options.showPropertiesGrid) ||
                (buttonProps[i][0] == "ReportTree" && (this.options.showPropertiesGrid || this.options.showDictionary))) {
                button.style.marginTop = "70px";
            }

            button.action = function () {
                var show = !this.isSelected;
                propertiesPanel.changeVisibleState(show);
                if (show) {
                    propertiesPanel.showContainer(this.panelName);
                    this.setSelected(true);
                }
            }
        }
    }

    buttonsPanel.changeVisibleState = function (state) {
        buttonsPanel.style.display = state ? "" : "none";
        buttonsPanel.style.top = (jsObject.options.toolBar.offsetHeight + jsObject.options.workPanel.offsetHeight + jsObject.options.infoPanel.offsetHeight + 40) + "px";
    }

    return buttonsPanel;
}

StiMobileDesigner.prototype.AddMainMethodsToPropertyControl = function (control) {
    if (!control) return;

    control.getValue = function () {
        var type = this.controlType;
        if (type == "DropdownList" || type == "Image" || (!type && this["setKey"])) { return this.key; }
        else if (type == "Checkbox" || (!type && this["setChecked"])) { return this.isChecked; }
        else if (type == "Textbox" || (!type && this["value"] != null)) { return "Base64Code;" + StiBase64.encode(this.value); }
        else if (type == "Expression" || (!type && this["textBox"] != null)) { return "Base64Code;" + StiBase64.encode(this.textBox.value); }
        else if (type == "ExpressionTextArea" || (!type && this["textArea"] != null)) { return this.textArea.value }

        return null;
    }

    control.setValue = function (value) {
        if (typeof (value) == "string" && value.indexOf("Base64Code;") == 0) {
            value = value.replace("Base64Code;", "");
            value = StiBase64.decode(value);
        }

        var type = this.controlType;
        if (type == "DropdownList" || type == "Image" || (!type && this["setKey"])) { this.setKey(value); }
        else if (type == "Checkbox" || (!type && this["setChecked"])) { this.setChecked(value); }
        else if (type == "Textbox" || (!type && this["value"] != null)) { this.value = value; }
        else if (type == "Expression" || (!type && this["textBox"] != null)) { this.textBox.value = value; }
        else if (type == "ExpressionTextArea" || (!type && this["textArea"] != null)) { this.textArea.value = value; }
    }
}

StiMobileDesigner.prototype.ExtractBase64Value = function (value) {
    if (value == "StiEmptyValue")
        return "";

    if (typeof value == "string" && value.indexOf("Base64Code;") == 0)
        return StiBase64.decode(value.replace("Base64Code;", ""));

    return value;
}