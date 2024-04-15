
StiMobileDesigner.prototype.InitializeEditChartConstantLinesForm_ = function () {
    var jsObject = this;
    var form = this.DashboardBaseForm("editChartConstantLinesForm", this.loc.PropertyMain.ConstantLines, 1);
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";

    var addConstButton = this.SmallButton(null, null, this.loc.Chart.AddConstantLine, null, null, null, "stiDesignerFormButton");
    addConstButton.style.margin = "12px 12px 0 12px";
    addConstButton.style.display = "inline-block";
    form.container.appendChild(addConstButton);

    var constantLinesContainer = this.ChartConstantLinesContainer(form);
    constantLinesContainer.style.margin = "12px";
    form.constantLinesContainer = constantLinesContainer;
    form.container.appendChild(constantLinesContainer);

    addConstButton.action = function () {
        if (form.currentChartComponent.typeComponent == "StiChart") {
            var params = {
                innerCommand: "AddConstantLine",
                componentName: form.chartProperties.name
            }
            jsObject.SendCommandToDesignerServer("UpdateChart", params,
                function (answer) {
                    form.chartProperties = answer.chartProperties;
                    var constLines = form.chartProperties.constantLines;
                    if (constLines.length > 0) {
                        form.updateProperties(form.chartProperties);
                        if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
                        form.updateSvgContent(form.chartProperties.svgContent);
                        constantLinesContainer.addConstLine(constLines[constLines.length - 1]).select();
                    }
                });
        }
        else if (form.currentChartComponent.typeComponent == "StiChartElement") {
            var params = {
                updateParameters: { command: "AddConstantLine" },
                componentName: form.chartProperties.name
            }
            jsObject.SendCommandToDesignerServer("UpdateChartElement", params,
                function (answer) {
                    form.chartProperties = answer.elementProperties;
                    var constLines = form.chartProperties.chartConstantLines;
                    if (constLines.length > 0) {
                        form.updateProperties(form.chartProperties);
                        if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
                        form.updateSvgContent(form.chartProperties.svgContent);
                        constantLinesContainer.addConstLine(constLines[constLines.length - 1]).select();
                    }
                });
        }
    }

    form.updateSvgContent = function (svgContent) {
        form.currentChartComponent.properties.svgContent = svgContent;
        form.currentChartComponent.repaint();
    }

    form.updateProperties = function (properties) {
        form.currentChartComponent.properties.chartConstantLines = (properties.constantLines || properties.chartConstantLines);
    }

    form.onshow = function () {
        form.currentPanelName = jsObject.options.propertiesPanel.getCurrentPanelName();
        constantLinesContainer.fill((form.chartProperties.constantLines || form.chartProperties.chartConstantLines), 0);
    }

    form.onhide = function () {
        var propertiesPanel = jsObject.options.propertiesPanel;
        if (propertiesPanel && propertiesPanel.editChartConstLinesMode) {
            propertiesPanel.setEditChartConstLinesMode(false);
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

StiMobileDesigner.prototype.ChartConstantLinesContainer = function (form) {
    var jsObject = this;
    var container = this.DataContainer(null, null, true, " ");
    container.className = "stiSimpleContainerWithBorder";
    container.style.height = container.style.width = "300px";
    container.style.padding = "1px";
    container.multiItems = true;

    container.addConstLine = function (constLineObject) {
        constLineObject.typeItem = "ConstantLine";

        var text = jsObject.ExtractBase64Value(constLineObject.properties.AxisValue) + " (" + (jsObject.ExtractBase64Value(constLineObject.properties.Text) || constLineObject.name || jsObject.loc.Chart.ConstantLine) + ")";
        var item = this.addItem(text, null, jsObject.CopyObject(constLineObject));
        item.style.color = item.style.color = constLineObject.properties.Visible === false ? "#c6c6c6" : jsObject.GetHTMLColor(constLineObject.properties.LineColor);
        item.style.height = "26px";
        item.style.padding = "4px";
        item.closeButton.imageCell.style.padding = "0";
        item.closeButton.style.width = item.closeButton.style.height = "20px";
        item.closeButton.style.marginRight = "2px";

        var line = document.createElement("div");
        var styles = ["solid", "dashed", "dashed", "dashed", "dotted", "double", "none"];
        line.style.borderTop = constLineObject.properties.LineWidth + "px " + styles[constLineObject.properties.LineStyle] + " " + (constLineObject.properties.Visible === false ? "#e6e6e6" : jsObject.GetHTMLColor(constLineObject.properties.LineColor));
        line.style.width = "100%";

        var position = constLineObject.properties.Position;
        if (position == "LeftTop" || position == "CenterTop" || position == "RightTop")
            item.captionContainer.appendChild(line);
        else {
            item.captionContainer.innerHTML = "";
            item.captionContainer.appendChild(line);
            item.captionContainer.innerHTML += text;
        }

        item.captionContainer.style.textAlign = position == "LeftTop" || position == "LeftBottom" ? "left" : (position == "CenterTop" || position == "CenterBottom" ? "center" : "right");
        return item;
    }

    container.fill = function (constLines, selectedIndex) {
        this.clear();
        for (var i = 0; i < constLines.length; i++) {
            this.addConstLine(constLines[i]);
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

            if (typeItem == "ConstantLine") {
                var toIndex = this.getOverItemIndex();
                var fromIndex = this.getItemIndex(jsObject.options.itemInDrag.originalItem);
                if (toIndex != null && fromIndex != null && fromIndex != toIndex) {
                    if (form.currentChartComponent.typeComponent == "StiChart") {
                        var params = {
                            innerCommand: "MoveConstantLine",
                            componentName: form.chartProperties.name,
                            toIndex: toIndex,
                            fromIndex: fromIndex
                        }
                        jsObject.SendCommandToDesignerServer("UpdateChart", params,
                            function (answer) {
                                form.chartProperties = answer.chartProperties;
                                form.updateProperties(form.chartProperties);
                                form.updateSvgContent(form.chartProperties.svgContent);
                                container.fill(form.chartProperties.constantLines, toIndex);
                            });
                    }
                    else if (form.currentChartComponent.typeComponent == "StiChartElement") {
                        var params = {
                            updateParameters: {
                                command: "MoveConstantLine",
                                toIndex: toIndex,
                                fromIndex: fromIndex
                            },
                            componentName: form.chartProperties.name
                        }
                        jsObject.SendCommandToDesignerServer("UpdateChartElement", params,
                            function (answer) {
                                form.chartProperties = answer.elementProperties;
                                form.updateProperties(form.chartProperties);
                                form.updateSvgContent(form.chartProperties.svgContent);
                                container.fill(form.chartProperties.chartConstantLines, toIndex);
                            });
                    }
                }
            }
        }

        return false;
    }

    container.onRemove = function (itemIndex) {
        if (form.currentChartComponent.typeComponent == "StiChart") {
            var params = {
                innerCommand: "RemoveConstantLine",
                componentName: form.chartProperties.name,
                constLineIndex: itemIndex
            }
            jsObject.SendCommandToDesignerServer("UpdateChart", params,
                function (answer) {
                    form.chartProperties = answer.chartProperties;
                    form.updateProperties(form.chartProperties);
                    if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
                    form.updateSvgContent(form.chartProperties.svgContent);
                    if (container.selectedItem) container.selectedItem.select();
                });
        }
        else if (form.currentChartComponent.typeComponent == "StiChartElement") {
            var params = {
                updateParameters: {
                    command: "RemoveConstantLine",
                    constLineIndex: itemIndex
                },
                componentName: form.chartProperties.name
            }
            jsObject.SendCommandToDesignerServer("UpdateChartElement", params,
                function (answer) {
                    form.chartProperties = answer.elementProperties;
                    form.updateProperties(form.chartProperties);
                    if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
                    form.updateSvgContent(form.chartProperties.svgContent);
                    if (container.selectedItem) container.selectedItem.select();
                });
        }
    }

    container.oncontextmenu = function (event) {
        return false;
    }

    container.onAction = function (actionName) {
        var propertiesPanel = jsObject.options.propertiesPanel;
        if (propertiesPanel) {
            if (actionName == "select") {
                var constLineIndex = container.getSelectedItemIndex();
                var constantLines = form.chartProperties.constantLines || form.chartProperties.chartConstantLines;
                if (constLineIndex != null && constLineIndex < constantLines.length) {
                    propertiesPanel.setEditChartConstLinesMode(true);
                    if (container.firstAction == null) container.firstAction = true;
                    var chartPropsPanel = jsObject.options.propertiesPanel.editChartPropertiesPanel;
                    chartPropsPanel.showAllConstantLinesGroups(constantLines[constLineIndex], container.firstAction);
                    container.firstAction = false;
                }
            }
            else if (actionName == "remove" && container.getCountItems() == 0 && propertiesPanel.editChartConstLinesMode) {
                propertiesPanel.setEditChartConstLinesMode(false);
            }
        }
    }

    container.onmouseover = function () { }
    container.onmouseout = function () { }
    container.updateHintText = function () { }

    return container;
}