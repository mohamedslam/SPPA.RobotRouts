
StiMobileDesigner.prototype.InitializeEditChartStripsForm_ = function () {
    var jsObject = this;
    var form = this.DashboardBaseForm("editChartStripsForm", this.loc.PropertyMain.Strips, 1);
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";

    var addStripButton = this.SmallButton(null, null, this.loc.Chart.AddStrip, null, null, null, "stiDesignerFormButton");
    addStripButton.style.margin = "12px 12px 0 12px";
    addStripButton.style.display = "inline-block";
    form.container.appendChild(addStripButton);

    var stripsContainer = this.ChartStripsContainer(form);
    stripsContainer.style.margin = "12px";
    form.stripsContainer = stripsContainer;
    form.container.appendChild(stripsContainer);

    addStripButton.action = function () {
        var params = {
            innerCommand: "AddStrip",
            componentName: form.chartProperties.name
        }

        jsObject.SendCommandToDesignerServer("UpdateChart", params,
            function (answer) {
                form.chartProperties = answer.chartProperties;
                var strips = form.chartProperties.strips;
                if (strips.length > 0) {
                    form.updateProperties(answer.chartProperties);
                    if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
                    form.updateSvgContent(answer.chartProperties.svgContent);
                    stripsContainer.addStrip(strips[strips.length - 1]).select();
                }
            });
    }

    form.updateSvgContent = function (svgContent) {
        form.currentChartComponent.properties.svgContent = svgContent;
        form.currentChartComponent.repaint();
    }

    form.updateProperties = function (properties) {
        form.currentChartComponent.properties.chartStrips = properties.strips;
    }

    form.onshow = function () {
        form.currentPanelName = jsObject.options.propertiesPanel.getCurrentPanelName();
        stripsContainer.fill(form.chartProperties.strips, 0);
    }

    form.onhide = function () {
        var propertiesPanel = jsObject.options.propertiesPanel;
        if (propertiesPanel && propertiesPanel.editChartStripsMode) {
            propertiesPanel.setEditChartStripsMode(false);
            propertiesPanel.showContainer(form.currentPanelName);
        }
    }

    form.cancelAction = function () {
        jsObject.SendCommandCanceledEditComponent(form.chartProperties.name);
        if (form.oldSvgContent) {
            form.currentChartComponent.properties.svgContent = form.oldSvgContent;
            form.currentChartComponent.repaint();
        }
    }

    form.action = function () {
        form.changeVisibleState(false);
    }

    return form;
}

StiMobileDesigner.prototype.ChartStripsContainer = function (form) {
    var jsObject = this;
    var container = this.DataContainer(null, null, true, " ");
    container.className = "stiSimpleContainerWithBorder";
    container.style.height = "300px";
    container.style.width = "300px";
    container.style.padding = "1px";
    container.multiItems = true;

    container.addStrip = function (stripObject) {
        stripObject.typeItem = "Strip";
        var item = this.addItem(stripObject.name + " (" + jsObject.ExtractBase64Value(stripObject.properties.MinValue) + " - " + jsObject.ExtractBase64Value(stripObject.properties.MaxValue) + ")", null, jsObject.CopyObject(stripObject));
        item.style.height = "26px";
        item.style.padding = "4px";
        item.closeButton.imageCell.style.padding = "0";
        item.closeButton.style.width = item.closeButton.style.height = "20px";
        item.closeButton.style.marginRight = "2px";
        item.firstChild.style.background = stripObject.properties.Visible ? jsObject.GetHTMLColor(jsObject.GetColorFromBrushStr(stripObject.properties.StripBrush)) : "#e6e6e6";
        item.firstChild.style.borderRadius = jsObject.allowRoundedControls() ? "3px" : "0";
        item.style.color = stripObject.properties.Visible ? "#444444" : "#ababab";
        return item;
    }

    container.fill = function (strips, selectedIndex) {
        this.clear();
        for (var i = 0; i < strips.length; i++) {
            this.addStrip(strips[i]);
        }
        if (selectedIndex != null) {
            var item = this.getItemByIndex(selectedIndex);
            if (item) item.select();
        }
    }

    container.onmouseup = function (event) {
        if (event.button != 2 && jsObject.options.itemInDrag) {
            var itemObject = jsObject.CopyObject(jsObject.options.itemInDrag.originalItem.itemObject);
            if (!itemObject) return;
            var typeItem = itemObject.typeItem;

            if (typeItem == "Strip") {
                var toIndex = this.getOverItemIndex();
                var fromIndex = this.getItemIndex(jsObject.options.itemInDrag.originalItem);
                if (toIndex != null && fromIndex != null && fromIndex != toIndex) {
                    var params = {
                        innerCommand: "MoveStrip",
                        componentName: form.chartProperties.name,
                        toIndex: toIndex,
                        fromIndex: fromIndex
                    }
                    jsObject.SendCommandToDesignerServer("UpdateChart", params,
                        function (answer) {
                            form.chartProperties = answer.chartProperties;
                            form.updateProperties(answer.chartProperties);
                            form.updateSvgContent(answer.chartProperties.svgContent);
                            container.fill(form.chartProperties.strips, toIndex);
                        });
                }
            }
        }

        return false;
    }

    container.onRemove = function (itemIndex) {
        var params = {
            innerCommand: "RemoveStrip",
            componentName: form.chartProperties.name,
            stripIndex: itemIndex
        }
        jsObject.SendCommandToDesignerServer("UpdateChart", params,
            function (answer) {
                form.chartProperties = answer.chartProperties;
                form.updateProperties(answer.chartProperties);
                if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
                form.updateSvgContent(answer.chartProperties.svgContent);
                if (container.selectedItem) container.selectedItem.select();
            });
    }

    container.oncontextmenu = function (event) {
        return false;
    }

    container.onAction = function (actionName) {
        var propertiesPanel = jsObject.options.propertiesPanel;
        if (propertiesPanel) {
            if (actionName == "select") {
                var stripIndex = container.getSelectedItemIndex();
                var strips = form.chartProperties.strips;
                if (stripIndex != null && stripIndex < strips.length) {
                    propertiesPanel.setEditChartStripsMode(true);
                    if (container.firstAction == null) container.firstAction = true;
                    var chartPropsPanel = jsObject.options.propertiesPanel.editChartPropertiesPanel;
                    chartPropsPanel.showAllStripsGroups(strips[stripIndex], container.firstAction);
                    container.firstAction = false;
                }
            }
            else if (actionName == "remove" && container.getCountItems() == 0 && propertiesPanel.editChartStripsMode) {
                propertiesPanel.setEditChartStripsMode(false);
            }
        }
    }

    container.onmouseover = function () { }
    container.onmouseout = function () { }
    container.updateHintText = function () { }

    return container;
}