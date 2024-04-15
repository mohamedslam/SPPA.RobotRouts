
StiMobileDesigner.prototype.InitializeEditPivotTableElementForm_ = function () {
    var form = this.DashboardBaseForm("editPivotTableElementForm", this.loc.Components.StiPivotTable, 1, this.HelpLinks["pivotTableElement"]);
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";
    form.hideButtonsPanel();
    var jsObject = this;

    var controlsTable = this.CreateHTMLTable();
    form.container.appendChild(controlsTable);
    form.container.style.padding = "0 0 6px 0";

    var expressionControl = this.ExpressionControlWithMenu(null, 305, null, null);
    var expressionMenu = this.options.menus.pivotTableElementExpressionMenu || this.InitializeDataColumnsExpressionMenu("pivotTableElementExpressionMenu", expressionControl, form);
    expressionControl.textBox.setAttribute("placeholder", jsObject.loc.PropertyMain.Field);

    var columnsBlock = this.PivotTableDataColumnsBlock(form, expressionMenu, "columns", this.loc.FormCrossTabDesigner.Columns.replace(":", ""), true);
    columnsBlock.style.width = "calc(100% - 24px)";
    columnsBlock.container.maxWidth = 280;
    form.addControlRow(controlsTable, null, "columnsBlock", columnsBlock, "0px 12px 0px 12px");
    var parentColumnsContainer = columnsBlock.container.parentElement;

    var rowsBlock = this.PivotTableDataColumnsBlock(form, expressionMenu, "rows", this.loc.FormCrossTabDesigner.Rows.replace(":", ""), true);
    rowsBlock.style.width = "calc(100% - 24px)";
    rowsBlock.container.maxWidth = 300;
    form.addControlRow(controlsTable, null, "rowsBlock", rowsBlock, "0px 12px 0px 12px");

    var swapButton = this.FormImageButton(null, "Dashboards.Swap.png", this.loc.FormCrossTabDesigner.Swap);
    swapButton.style.display = "inline-block";
    swapButton.style.marginTop = "2px";
    rowsBlock.header.style.width = "100%";
    rowsBlock.container.parentNode.setAttribute("colspan", "2");
    rowsBlock.addCell(swapButton).style.textAlign = "right";

    var summariesBlock = this.PivotTableDataColumnsBlock(form, expressionMenu, "summaries", this.loc.FormCrossTabDesigner.Summary.replace(":", ""), true);
    summariesBlock.container.maxWidth = 300;
    summariesBlock.style.width = "calc(100% - 24px)";
    form.addControlRow(controlsTable, null, "summariesBlock", summariesBlock, "0px 12px 12px 12px");

    var summaryDirectionButton = this.FormImageButton(null, "Dashboards.VerticalPlacement.png", this.loc.PropertyMain.Direction);
    summaryDirectionButton.style.display = "inline-block";
    summaryDirectionButton.style.marginTop = "2px";
    summariesBlock.header.style.width = "100%";
    summariesBlock.container.parentNode.setAttribute("colspan", "2");
    summariesBlock.addCell(summaryDirectionButton).style.textAlign = "right";

    swapButton.action = function () {
        form.sendCommand({ command: "SwapColumnsRows" },
            function (answer) {
                form.pivotProperties = answer.elementProperties;
                var meters = form.pivotProperties.meters;
                columnsBlock.container.updateMeters(meters.columns, meters.columns.length > 0 ? 0 : -1);
                rowsBlock.container.updateMeters(meters.rows, meters.columns.length == 0 && meters.rows.length > 0 ? 0 : -1);
                form.updateSvgContent(answer.elementProperties.svgContent);
            }
        );
    }

    form.addControlRow(controlsTable, this.loc.PropertyMain.Expression, "expressionControlCaption", null, null, "6px 12px 6px 12px");
    form.addControlRow(controlsTable, null, "expression", expressionControl, "6px 12px 6px 12px", null, null, true);

    summaryDirectionButton.action = function () {
        form.sendCommand({ command: "SwapSummaryDirection" },
            function (answer) {
                form.pivotProperties = answer.elementProperties;
                var meters = form.pivotProperties.meters;
                columnsBlock.container.updateMeters(meters.columns, meters.columns.length > 0 ? 0 : -1);
                rowsBlock.container.updateMeters(meters.rows, meters.columns.length == 0 && meters.rows.length > 0 ? 0 : -1);
                form.updateSvgContent(answer.elementProperties.svgContent);
                StiMobileDesigner.setImageSource(summaryDirectionButton.image, jsObject.options, form.pivotProperties.summaryDirection == "LeftToRight" ? "Dashboards.HorizontalPlacement.png" : "Dashboards.VerticalPlacement.png");
            }
        );
    }

    form.addControlRow(controlsTable, null, "expression", expressionControl, "6px 12px 6px 12px");
    expressionControl.style.display = "inline-block";
    expressionControl.parentNode.style.textAlign = "right";

    expressionControl.menu = expressionMenu;
    expressionMenu.parentButton = expressionControl.button;

    var topNBlock = this.PivotTableTopNBlock(form, "topN", this.loc.PropertyMain.TopN);
    form.addControlRow(controlsTable, this.loc.PropertyMain.TopN, "topN", topNBlock, "6px 12px 6px 12px", null, true, true);

    topNBlock.setKey = function (key) {
        var selectedItem = form.getSelectedItem();
        if (selectedItem) {
            var containerName = selectedItem.container.name;
            var itemIndex = selectedItem.container.getItemIndex(selectedItem);
            form.sendCommand(
                {
                    command: "SetTopN",
                    itemIndex: itemIndex,
                    containerName: containerName,
                    topN: key
                },
                function (answer) {
                    var container = form.controls[containerName + "Block"].container;
                    container.updateMeters(answer.elementProperties.meters[containerName], itemIndex);
                    form.updateSvgContent(answer.elementProperties.svgContent);
                }
            );
        }
    }

    topNBlock.clearButton.action = function () {
        var selectedItem = form.getSelectedItem();
        if (selectedItem) {
            var containerName = selectedItem.container.name;
            var itemIndex = selectedItem.container.getItemIndex(selectedItem);
            var topN = {
                mode: "None",
                count: 5,
                showOthers: true,
                othersText: "",
                measureField: ""
            };
            form.sendCommand(
                {
                    command: "SetTopN",
                    itemIndex: itemIndex,
                    containerName: containerName,
                    topN: topN
                },
                function (answer) {
                    var container = form.controls[containerName + "Block"].container;
                    container.updateMeters(answer.elementProperties.meters[containerName], itemIndex);
                    form.updateSvgContent(answer.elementProperties.svgContent);
                }
            );
        }
    }

    expressionControl.action = function () {
        var selectedItem = form.getSelectedItem();
        if (selectedItem) {
            var containerName = selectedItem.container.name;
            var itemIndex = selectedItem.container.getItemIndex(selectedItem);
            form.sendCommand(
                {
                    command: "SetExpression",
                    itemIndex: itemIndex,
                    containerName: containerName,
                    expressionValue: StiBase64.encode(expressionControl.textBox.value)
                },
                function (answer) {
                    var container = form.controls[containerName + "Block"].container;
                    container.updateMeters(answer.elementProperties.meters[containerName], itemIndex);
                    form.updateSvgContent(answer.elementProperties.svgContent);
                }
            );
        }
    }

    expressionControl.refreshExpressionHint = function () {
        var selectedItem = form.getSelectedItem();
        expressionControl.textBox.setAttribute("placeholder", selectedItem ? (selectedItem.container.name == "summaries" ? "Sum(" + jsObject.loc.PropertyMain.Field + ")" : jsObject.loc.PropertyMain.Field) : "");
    }

    form.setPropertyValue = function (propertyName, propertyValue) {
        var selectedItem = form.getSelectedItem();
        if (selectedItem) {
            var containerName = selectedItem.container.name;
            var itemIndex = selectedItem.container.getItemIndex(selectedItem);
            form.sendCommand(
                {
                    command: "SetPropertyValue",
                    propertyName: propertyName,
                    propertyValue: propertyValue,
                    itemIndex: itemIndex,
                    containerName: containerName
                },
                function (answer) {
                    var container = form.controls[containerName + "Block"].container;
                    container.updateMeters(answer.elementProperties.meters[containerName], itemIndex);
                    form.updateSvgContent(answer.elementProperties.svgContent);
                }
            );
        }
    }

    form.updateElementProperties = function (properties) {
        for (var propertyName in properties) {
            this.currentPivotTableElement.properties[propertyName] = properties[propertyName];
        }
        jsObject.options.homePanel.updateControls();
        if (jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.editDbsMeterMode) {
            jsObject.options.propertiesPanel.updateControls();
        }
        summaryDirectionButton.style.display = properties.meters.summaries.length > 1 ? "inline-block" : "none";
        StiMobileDesigner.setImageSource(summaryDirectionButton.image, jsObject.options, this.pivotProperties.summaryDirection == "LeftToRight" ? "Dashboards.HorizontalPlacement.png" : "Dashboards.VerticalPlacement.png");
    }

    form.setValues = function () {
        var meters = this.pivotProperties.meters;
        columnsBlock.container.updateMeters(meters.columns, meters.columns.length > 0 ? 0 : -1);
        rowsBlock.container.updateMeters(meters.rows, meters.columns.length == 0 && meters.rows.length > 0 ? 0 : -1);
        summariesBlock.container.updateMeters(meters.summaries, meters.columns.length == 0 && meters.rows.length == 0 ? 0 : -1);
        summaryDirectionButton.style.display = this.pivotProperties.meters.summaries.length > 1 ? "inline-block" : "none";
        StiMobileDesigner.setImageSource(summaryDirectionButton.image, jsObject.options, this.pivotProperties.summaryDirection == "LeftToRight" ? "Dashboards.HorizontalPlacement.png" : "Dashboards.VerticalPlacement.png");
    }

    form.checkStartMode = function () {
        var itemsCount = 0;
        var containers = ["columns", "rows", "summaries"];
        for (var i = 0; i < containers.length; i++) {
            var container = form.controls[containers[i] + "Block"].container;
            itemsCount += container.getCountItems();
        }

        if (itemsCount == 0) {
            form.container.appendChild(columnsBlock.container);
            controlsTable.style.display = "none";
            columnsBlock.container.style.height = columnsBlock.container.style.maxHeight = "260px";
            columnsBlock.container.style.width = "267px";
            columnsBlock.container.style.margin = "6px 12px 6px 12px";
        }
        else {
            parentColumnsContainer.appendChild(columnsBlock.container);
            controlsTable.style.display = "";
            columnsBlock.container.style.height = "auto";
            columnsBlock.container.style.width = "auto";
            columnsBlock.container.style.margin = "0";
            columnsBlock.container.style.maxHeight = "100px";
        }
    }

    form.onshow = function () {
        form.currentPanelName = jsObject.options.propertiesPanel.getCurrentPanelName();
        if (jsObject.options.showDictionary) jsObject.options.propertiesPanel.showContainer("Dictionary");
        expressionControl.textBox.value = "";
        expressionControl.setEnabled(false);
        columnsBlock.container.clear();
        rowsBlock.container.clear();
        summariesBlock.container.clear();
        form.checkStartMode();

        form.sendCommand({ command: "GetPivotTableElementProperties" },
            function (answer) {
                form.pivotProperties = answer.elementProperties;
                form.setValues();
                form.checkStartMode();
                form.correctTopPosition();
            }
        );
    }

    form.onhide = function () {
        var propertiesPanel = jsObject.options.propertiesPanel;
        if (propertiesPanel.editDbsMeterMode) {
            propertiesPanel.setEditDbsMeterMode(false);
        }
        propertiesPanel.showContainer(form.currentPanelName);
        jsObject.options.homePanel.updateControls();
    }

    form.getSelectedItem = function () {
        var containers = ["columns", "rows", "summaries"];
        for (var i = 0; i < containers.length; i++) {
            var container = form.controls[containers[i] + "Block"].container;
            if (container.selectedItem) {
                return container.selectedItem;
            }
        }
        return null;
    }

    form.applyExpressionPropertyToPivotTableElement = function (containerName, itemIndex, expressionValue) {
        form.sendCommand(
            {
                command: "SetExpression",
                itemIndex: itemIndex,
                containerName: containerName,
                expressionValue: StiBase64.encode(expressionValue)
            },
            function (answer) {
                var container = form.controls[containerName + "Block"].container;
                container.updateMeters(answer.elementProperties.meters[containerName], itemIndex);
                form.updateSvgContent(answer.elementProperties.svgContent);
            }
        );
    }

    form.sendCommand = function (updateParameters, callbackFunction) {
        form.jsObject.SendCommandToDesignerServer("UpdatePivotTableElement",
            {
                componentName: form.currentPivotTableElement.properties.name,
                updateParameters: updateParameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    form.updateSvgContent = function (svgContent) {
        this.currentPivotTableElement.properties.svgContent = svgContent;
        this.currentPivotTableElement.repaint();
    }

    return form;
}

StiMobileDesigner.prototype.PivotTableDataColumnsBlock = function (form, contextMenu, containerName, headerText, multiItems) {
    var jsObject = this;
    var block = this.DashboardDataColumnsBlock(form, contextMenu, containerName, headerText, multiItems);

    block.container.onAction = function (actionName) {
        if (actionName == "rename") {
            selectedItem = form.getSelectedItem();
            if (selectedItem) {
                var containerName = selectedItem.container.name;
                var itemIndex = selectedItem.container.getItemIndex(selectedItem);
                form.sendCommand({
                    command: "RenameMeter",
                    itemIndex: itemIndex,
                    containerName: containerName,
                    newLabel: selectedItem.itemObject.label
                },
                    function (answer) {
                        var container = form.controls[containerName + "Block"].container;
                        container.updateMeters(answer.elementProperties.meters[containerName], itemIndex);
                        form.updateSvgContent(answer.elementProperties.svgContent);
                    }
                );
            }
            return;
        }

        var selectedItem = null;
        var containers = ["columns", "rows", "summaries"];
        for (var i = 0; i < containers.length; i++) {
            var container = form.controls[containers[i] + "Block"].container;
            if (actionName != "clear" && this != container && container.selectedItem) {
                container.selectedItem.setSelected(false);
                container.selectedItem = null;
            }
            if (container.selectedItem) {
                selectedItem = container.selectedItem;
                form.controls.topN.selectedContainer = containers[i];
            }
        }
        form.controls.expression.setEnabled(selectedItem != null);
        form.controls.expression.textBox.value = selectedItem != null ? StiBase64.decode(selectedItem.itemObject.expression) : "";
        form.controls.expression.refreshExpressionHint();

        if (selectedItem && (selectedItem.container.name == "columns" || selectedItem.container.name == "rows")) {
            form.controls.topN.setEnabled(true);
            form.controls.topN.clearButton.setEnabled(selectedItem.itemObject.topN.mode != "None");
            form.controls.topN.textBox.value = selectedItem.itemObject.topN.stringContent;
            form.controls.topN.key = selectedItem.itemObject.topN;
        } else {
            form.controls.topN.setEnabled(false);
            form.controls.topN.key = null;
        }

        var propertiesPanel = jsObject.options.propertiesPanel;

        if (actionName == "select" && selectedItem) {
            propertiesPanel.setEditDbsMeterMode(true);
            propertiesPanel.editDbsMeterPropertiesPanel.updateProperties(form, selectedItem);
        }
        else if (actionName == "remove" && propertiesPanel.editDbsMeterMode && this.getCountItems() == 0) {
            propertiesPanel.setEditDbsMeterMode(false);
        }

        jsObject.options.homePanel.updateControls();
    }

    return block;
}

StiMobileDesigner.prototype.PivotTableTopNBlock = function (form, containerName, headerText) {
    var container = this.TextBoxWithEditButton(containerName, 180, null, true, true);

    container.button.action = function () {
        var summaries = [];

        if (form.controls.summariesBlock.container.getCountItems() > 0) {
            var childs = form.controls["summariesBlock"].container.childNodes;
            for (var i = 0; i < childs.length; i++) {
                summaries.push(this.jsObject.Item(childs[i].itemObject.label, childs[i].itemObject.label, null, childs[i].itemObject.label));
            }
        }

        this.jsObject.InitializeTopNForm(false, function (form) {
            form.show(container, summaries);
        });
    }

    return container;
}