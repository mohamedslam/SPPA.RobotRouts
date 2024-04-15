
StiMobileDesigner.prototype.ReportTreePanel = function () {
    var reportTreePanel = document.createElement("div");
    var jsObject = reportTreePanel.jsObject = this;
    reportTreePanel.className = "stiDesignerPropertiesPanelInnerContent";
    this.options.reportTreePanel = reportTreePanel;
    reportTreePanel.style.top = "35px";

    var findTable = this.CreateHTMLTable();
    findTable.style.margin = "6px 0 6px 0";
    reportTreePanel.appendChild(findTable);

    var findTextbox = this.FindControl(null, this.options.propertiesGridWidth - (this.options.isTouchDevice ? 30 : 26) * 2 - 24, this.options.isTouchDevice ? 28 : 23);
    var compsButton = this.StandartSmallButton(null, null, null, "ComponentsTab.png");
    var evntButton = this.StandartSmallButton(null, null, null, "EventsTab.png");

    findTextbox.style.marginLeft = "6px";
    compsButton.style.marginLeft = "4px";
    evntButton.style.marginLeft = "4px";

    compsButton.setSelected(true);
    evntButton.setSelected(true);

    findTable.addCell(findTextbox);
    findTable.addCell(compsButton).style.width = "1px";
    findTable.addCell(evntButton).style.width = "1px";

    var reportTree = this.ReportTree();
    reportTreePanel.appendChild(reportTree);

    reportTree.compsButton = compsButton;
    reportTree.evntButton = evntButton;
    reportTree.findTextbox = findTextbox;

    reportTreePanel.setFocused = function (state) {
        this.isFocused = state;
        var selectedItem = reportTree.selectedItem;

        if (selectedItem) {
            if (state)
                selectedItem.button.className = selectedItem.button.className.replace(" stiDesignerTreeItemButtonSelectedNotActive", "");
            else if (selectedItem.button.className.indexOf("stiDesignerTreeItemButtonSelectedNotActive") < 0)
                selectedItem.button.className += " stiDesignerTreeItemButtonSelectedNotActive";
        }
    }

    reportTreePanel.onmousedown = function () {
        this.setFocused(true);
        jsObject.options.reportTreePanelPressed = true;
    }

    reportTreePanel.oncontextmenu = function (event) {
        return false;
    }

    findTextbox.action = function () {
        var findValue = findTextbox.getValue().toLowerCase();

        for (var key in reportTree.items) {
            var item = reportTree.items[key];
            var itemText = item.button.captionCell.innerHTML.toLowerCase();

            if (this.lastFindValue == findValue && reportTree.selectedItem) {
                if (item == reportTree.selectedItem) {
                    this.lastFindValue = null;
                }
                continue;
            }

            if (findValue != "" && itemText.indexOf(findValue) >= 0) {
                item.button.action();
                item.openTree();
                break;
            }
        }
        this.lastFindValue = findValue;
    }

    reportTree.compsButton.action = function () {
        this.setSelected(!this.isSelected);
        reportTree.build(!reportTree.compsButton.isSelected && reportTree.evntButton.isSelected);
    }

    reportTree.evntButton.action = function () {
        this.setSelected(!this.isSelected);
        reportTree.build(!reportTree.compsButton.isSelected && reportTree.evntButton.isSelected);
    }

    return reportTreePanel;
}

StiMobileDesigner.prototype.ReportTree = function () {
    var reportTree = this.Tree();
    reportTree.className = "stiDesignerDictionaryItemsContainer";
    reportTree.style.top = this.options.isTouchDevice ? "45px" : "40px";
    reportTree.style.padding = "0 6px 6px 6px";
    reportTree.openingKeys = {};
    this.options.reportTree = reportTree;
    var jsObject = this;


    reportTree.addComponents = function (parentItem, components, onlyEvents, pageItem) {
        var tempArray = [];
        for (var componentName in components) {
            tempArray.push({ index: jsObject.StrToInt(components[componentName].properties.componentIndex), component: components[componentName] });
        }
        tempArray.sort(jsObject.SortByIndex);

        for (var i = 0; i < tempArray.length; i++) {
            var component = tempArray[i].component;
            var componentName = component.properties.name;
            var imagesPath = component.isDashboardElement ? "Dashboards.SmallComponents." : "SmallComponents.";
            var imageName = StiMobileDesigner.checkImageSource(jsObject.options, imagesPath + component.typeComponent + ".png") != null ? imagesPath + component.typeComponent + ".png" : "SmallComponents.StiText.png";

            var componentAlias = component.properties.aliasName != null ? StiBase64.decode(component.properties.aliasName) : component.properties.alias;
            var captionText = (!componentAlias || componentName == componentAlias) ? componentName : componentName + " [" + componentAlias + "]";
            if (jsObject.options.useAliases && jsObject.options.showOnlyAliasForComponents && componentAlias) captionText = componentAlias;

            var componentItem = jsObject.ReportTreeItem(captionText, imageName, component, reportTree, null, componentName);
            if (!onlyEvents && reportTree.compsButton.isSelected) parentItem.addChild(componentItem);
            if (reportTree.evntButton.isSelected || onlyEvents) reportTree.addEvents(onlyEvents ? pageItem : componentItem, component.properties.events, captionText, component);
            if (reportTree.openingKeys[componentItem.id]) componentItem.setOpening(true);

            if (component.typeComponent == "StiCrossTab") {
                var crossTabComponents = {}
                var crossTabChilds = component.controls.crossTabContainer.childNodes;
                if (crossTabChilds.length > 0) {
                    for (var i = 0; i < crossTabChilds.length; i++) {
                        crossTabComponents[crossTabChilds[i].properties.name] = crossTabChilds[i];
                    }
                    reportTree.addComponents(componentItem, crossTabComponents, onlyEvents, pageItem);
                }
            }
            else {
                var childsStr = component.properties.childs;
                if (childsStr) {
                    var childNames = childsStr.split(",");
                    var childs = {};

                    for (var indexChild = 0; indexChild < childNames.length; indexChild++) {
                        var child = jsObject.options.report.pages[component.properties.pageName].components[childNames[indexChild]];
                        if (child && child.properties.parentName == componentName) childs[child.properties.name] = child;
                    }
                    reportTree.addComponents(componentItem, childs, onlyEvents, pageItem);
                }
            }
        }
    }

    reportTree.addPages = function (parentItem, pages, onlyEvents) {
        var pagesByIndexes = [];
        for (var pageName in pages) {
            var page = pages[pageName];
            pagesByIndexes[parseInt(page.properties.pageIndex)] = page;
        }
        for (var i = 0; i < pagesByIndexes.length; i++) {
            var page = pagesByIndexes[i];
            if (!page) continue;
            var pageName = page.properties.name;
            var pageAlias = StiBase64.decode(page.properties.aliasName);
            var captionText = (!pageAlias || pageName == pageAlias) ? pageName : pageName + " [" + pageAlias + "]";
            if (jsObject.options.useAliases && jsObject.options.showOnlyAliasForPages && pageAlias) captionText = pageAlias;

            var pageItem = jsObject.ReportTreeItem(captionText, "SmallComponents." + (page.isDashboard ? "StiDashboard.png" : "StiPage.png"), page, reportTree, null, pageName);
            parentItem.addChild(pageItem);
            if (reportTree.openingKeys[pageItem.id] || onlyEvents) pageItem.setOpening(true);
            var pageChilds = {};
            for (var compName in page.components) {
                if (page.components[compName].properties.parentName == pageName)
                    pageChilds[compName] = page.components[compName];
            }
            if (reportTree.evntButton.isSelected || onlyEvents) reportTree.addEvents(pageItem, page.properties.events);
            reportTree.addComponents(pageItem, pageChilds, onlyEvents, pageItem);
        }
    }

    reportTree.addEvents = function (parentItem, events, ownerName, owner) {
        for (var eventName in events) {
            if (eventName && events[eventName]) {
                var captionText = eventName.replace("Event", "");
                if (ownerName) captionText = ownerName + "." + captionText;
                var eventItem = jsObject.ReportTreeItem(captionText, "EventsTab.png", { typeComponent: "StiEvent", eventName: eventName }, reportTree, null, parentItem.id + "_" + eventName);
                eventItem.button.owner = owner;
                parentItem.addChild(eventItem);

                eventItem.button.ondblclick = function () {
                    var parentItem = this.treeItem.parent;
                    if (parentItem && parentItem.itemObject.properties) {
                        var eventOwner = null;

                        if (parentItem.itemObject.typeComponent == "StiPage") {
                            eventOwner = this.owner || jsObject.options.report.pages[parentItem.itemObject.properties.name];
                        }
                        else if (parentItem.itemObject.typeComponent == "StiReport") {
                            eventOwner = this.owner || jsObject.options.report;
                        }
                        else {
                            eventOwner = jsObject.options.report.getComponentByName(parentItem.itemObject.properties.name);
                        }

                        if (eventOwner != null) {
                            var pageName = null;

                            if (parentItem.itemObject.typeComponent == "StiPage") {
                                pageName = parentItem.itemObject.properties.name;
                            }
                            else if (eventOwner.properties && eventOwner.properties.pageName) {
                                pageName = eventOwner.properties.pageName;
                            }
                            if (pageName && pageName != jsObject.options.currentPage.properties.name && jsObject.options.report.pages[pageName]) {
                                jsObject.options.paintPanel.showPage(jsObject.options.report.pages[pageName]);
                            }

                            eventOwner.setSelected();
                            jsObject.options.propertiesPanel.setEventsMode(true);

                            var eventsProps = jsObject.options.propertiesPanel.eventsPropertiesPanel.properties;
                            if (eventsProps && eventsProps[this.treeItem.itemObject.eventName]) {
                                eventsProps[this.treeItem.itemObject.eventName].propertyControl.button.action();
                            }
                        }
                    }
                }
            }
        }
    }

    reportTree.build = function (onlyEvents) {
        if (jsObject.options.propertiesPanel.containers.ReportTree.style.display == "none") return;
        reportTree.clear();

        if (jsObject.options.report) {
            var reportCaption = jsObject.loc.Components.StiReport + " [" + StiBase64.decode(jsObject.options.report.properties.reportName.replace("Base64Code;", "")) + "]";
            reportTree.reportItem = jsObject.ReportTreeItem(reportCaption, "SmallComponents.StiReport.png", jsObject.options.report, reportTree, null, "reportItem");
            reportTree.appendChild(reportTree.reportItem);
            reportTree.reportItem.setOpening(true);

            if (reportTree.evntButton.isSelected || onlyEvents) reportTree.addEvents(reportTree.reportItem, jsObject.options.report.properties.events);
            reportTree.addPages(reportTree.reportItem, jsObject.options.report.pages, onlyEvents);
        }

        if (reportTree.selectedItem && reportTree.items[reportTree.selectedItem.id]) {
            reportTree.items[reportTree.selectedItem.id].setSelected();
        }
        else {
            var selectedObject = jsObject.options.selectedObjects ? jsObject.options.selectedObjects[0] : jsObject.options.selectedObject;
            if (selectedObject) {
                var item = selectedObject.typeComponent == "StiReport" ? reportTree.reportItem : reportTree.items[selectedObject.properties.name];
                if (item) item.setSelected();
            }
        }
    }

    reportTree.onActionItem = function (item) {
        if (item.itemObject) {
            if (item.itemObject.typeComponent == "StiPage") {
                jsObject.options.paintPanel.showPage(item.itemObject);
            }
            else if (item.itemObject.typeComponent == "StiCrossField") {
                var crossTab = jsObject.options.report.getComponentByName(item.itemObject.properties.parentCrossTabName);
                if (crossTab) {
                    var crossFields = crossTab.controls.crossTabContainer.childNodes;
                    for (var i = 0; i < crossFields.length; i++) {
                        if (crossFields[i].properties.name == item.itemObject.properties.name) {
                            crossFields[i].action();
                        }
                    }
                }
            }
            else if (item.itemObject.properties && item.itemObject.properties.pageName && item.itemObject.properties.pageName != jsObject.options.currentPage.properties.name &&
                jsObject.options.report.pages[item.itemObject.properties.pageName]) {
                jsObject.options.paintPanel.showPage(jsObject.options.report.pages[item.itemObject.properties.pageName]);
            }

            if (item.itemObject.setSelected) {
                item.itemObject.setSelected();
            }

            jsObject.UpdatePropertiesControls();
        }
    };

    reportTree.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        this.items = {};
    }

    reportTree.reset = function () {
        reportTree.openingKeys = {};
        reportTree.selectedItem = null;
    }

    return reportTree;
}

StiMobileDesigner.prototype.ReportTreeItem = function (caption, imageName, itemObject, tree, showCheckBox, id) {
    var jsObject = this;
    var reportTreeItem = this.TreeItem(caption, imageName, itemObject, tree, showCheckBox, id);

    reportTreeItem.onmousedown = function (event) {
        if (event) event.preventDefault();
        return false;
    }

    //Override
    reportTreeItem.iconOpening.action = function () {
        if (this.treeItem.tree.isDisable) return;
        this.treeItem.isOpening = !this.treeItem.isOpening;
        this.treeItem.childsRow.style.display = this.treeItem.isOpening ? "" : "none";
        var imgName = this.treeItem.isOpening ? "IconCloseItem.png" : "IconOpenItem.png";
        if (jsObject.options.isTouchDevice) imgName = imgName.replace(".png", "Big.png");
        StiMobileDesigner.setImageSource(this.treeItem.iconOpening, jsObject.options, imgName);
        this.treeItem.setSelected();
        this.treeItem.tree.onActionItem(this.treeItem);

        if (this.treeItem.id != "reportItem") {
            if (this.treeItem.isOpening) {
                this.treeItem.tree.openingKeys[this.treeItem.id] = true;
            }
            else {
                if (this.treeItem.tree.openingKeys[this.treeItem.id]) {
                    delete this.treeItem.tree.openingKeys[this.treeItem.id];
                }
            }
        }
    }

    reportTreeItem.openTree = function () {
        var item = this.parent;
        while (item != null) {
            item.isOpening = true;
            this.tree.openingKeys[item.id] = true;
            item.childsRow.style.display = "";
            StiMobileDesigner.setImageSource(item.iconOpening, jsObject.options, jsObject.options.isTouchDevice ? "IconCloseItemBig.png" : "IconCloseItem.png");
            item = item.parent;
        }
    }

    reportTreeItem.setSelected = function (selectAndAction) {
        var baseClass = "stiDesignerTreeItemButton stiDesignerTreeItemButton";
        var selectedItem = this.tree.selectedItem;

        if (selectedItem) {
            selectedItem.button.className = baseClass + "Default";
            selectedItem.isSelected = false;
        }

        this.button.className = baseClass + "Selected";
        this.tree.selectedItem = this;
        this.isSelected = true;
        this.tree.onSelectedItem(this);

        if (jsObject.options.reportTreePanel && !jsObject.options.reportTreePanel.isFocused) {
            if (this.button.className.indexOf("stiDesignerTreeItemButtonSelectedNotActive") < 0) this.button.className += " stiDesignerTreeItemButtonSelectedNotActive";
        }

        if (selectAndAction) {
            this.tree.onActionItem(this);
        }
    }

    return reportTreeItem;
}