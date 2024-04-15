
StiMobileDesigner.prototype.InitializeEditTableElementForm_ = function () {
    var form = this.DashboardBaseForm("editTableElementForm", this.loc.Components.StiTable, 1, this.HelpLinks["tableElement"]);
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";
    form.hideButtonsPanel();
    var jsObject = this;

    var controlsTable = this.CreateHTMLTable();
    form.container.appendChild(controlsTable);
    form.container.style.padding = "0 0 6px 0";

    //Data Container
    var dataContainer = this.TableElementDataContainer(340, 250, true, form);
    form.addControlRow(controlsTable, null, "dataContainer", dataContainer, "6px 12px 6px 12px");

    //Expression
    var expressionControl = this.ExpressionControlWithMenu(null, 338, null, null, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Expression, "expressionControlCaption", null, null, "6px 12px 6px 12px");
    form.addControlRow(controlsTable, null, "expressionControl", expressionControl, "6px 12px 6px 12px");
    form.expressionMenu = this.options.menus.tableElementExpressionMenu || this.InitializeDataContainerExpressionMenu("tableElementExpressionMenu", expressionControl, dataContainer, form);
    expressionControl.menu = form.expressionMenu;
    form.expressionMenu.parentButton = expressionControl.button;

    expressionControl.action = function () {
        form.setPropertyValue("Expression", StiBase64.encode(this.textBox.value));
    }

    //Column Types
    var meterTypes = ["Dimension", "Measure", "DataBars", "ColorScale", "Indicator", "Sparklines", "Bubble"];
    var meterTypesTable = this.CreateHTMLTable();
    meterTypesTable.buttons = {};

    for (var i = 0; i < meterTypes.length; i++) {
        var button = this.FormButtonWithThemeBorder(null, null, null, "Meters." + meterTypes[i] + ".png", this.loc.Dashboard[meterTypes[i]]);
        button.meterType = meterTypes[i];
        button.style.marginRight = "6px";

        if (this.options.isTouchDevice) {
            button.style.marginRight = "3px";
            button.style.width = button.style.height = "24px";
            button.imageCell.style.padding = "0";
        }

        meterTypesTable.addCell(button);
        meterTypesTable.buttons[meterTypes[i]] = button;

        button.action = function () {
            this.select();
            var itemIndex = dataContainer.getSelectedItemIndex();

            form.sendCommand({ command: "ConvertMeter", itemIndex: itemIndex, meterType: this.meterType },
                function (answer) {
                    dataContainer.updateMeters(answer.elementProperties.meters, itemIndex);
                    form.updateElementProperties(answer.elementProperties);
                    form.updateSvgContent(answer.elementProperties.svgContent);
                }
            );
        }

        button.select = function () {
            for (var name in meterTypesTable.buttons) {
                meterTypesTable.buttons[name].setSelected(false);
            }
            this.setSelected(true);
        }
    }

    form.addControlRow(controlsTable, this.loc.PropertyMain.Type, "meterTypesTable", meterTypesTable, "6px 12px 6px 0");
    form.controls.meterTypesTableText.style.minWidth = "100px";

    //Visibility
    var visibility = this.DropDownList("tableElementVisibility", 180, null, this.GetBoolAndExpressionItems(), true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Visible, "visibility", visibility, "6px 12px 6px 0", null, true);

    //Visibility Expression
    var visibilityExpression = this.ExpressionControl("tableElementVisibilityExpression", 180);
    form.addControlRow(controlsTable, " ", "visibilityExpression", visibilityExpression, "6px 12px 6px 0", null, true);

    visibility.action = function () {
        form.controls.visibilityExpressionRow.style.display = visibility.key == "Expression" ? "" : "none";
        form.setPropertyValue("Visibility", this.key);
    }

    visibilityExpression.action = function () {
        form.setPropertyValue("VisibilityExpression", StiBase64.encode(this.textBox.value));
    }

    //ShowTotalSummary
    var showTotalSummary = this.CheckBox(null, this.loc.PropertyMain.ShowTotal);
    form.addControlRow(controlsTable, " ", "showTotalSummary", showTotalSummary, "8px 12px 8px 0");

    showTotalSummary.action = function () {
        form.controls.summaryTypeRow.style.display = this.isChecked ? "" : "none";
        form.setPropertyValue("ShowTotalSummary", this.isChecked);
    }

    //SummaryType
    var summaryType = this.DropDownList("tableElementSummaryType", 180, null, this.GetSummaryColumnTypeItems(), true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.SummaryType, "summaryType", summaryType, "6px 12px 6px 0", null, true);

    summaryType.action = function () {
        form.setPropertyValue("SummaryType", this.key);
    }

    //Min Max Color
    var minMaxTable = this.CreateHTMLTable();
    minMaxTable.style.maxWidth = "150px";
    minMaxTable.addTextCell(this.loc.PropertyMain.Minimum).className = "stiDesignerTextContainer";
    minMaxTable.addCell();
    minMaxTable.addTextCell(this.loc.PropertyMain.Maximum).className = "stiDesignerTextContainer";
    form.addControlRow(controlsTable, " ", "minMaxTable", minMaxTable, "6px 12px 6px 0");

    var minimumColor = this.ColorControl("tableElementMinimumColor", null, null, 79, true);
    minMaxTable.addCellInNextRow(minimumColor);
    form.controls.minimumColor = minimumColor;

    minimumColor.action = function () {
        form.setPropertyValue("MinimumColor", this.key);
    }

    minMaxTable.addTextCellInLastRow(" - ").style.padding = this.options.isTouchDevice ? "10px 5px 10px 5px" : "7px 5px 7px 5px";
    var maximumColor = this.ColorControl("tableElementMaximumColor", null, null, 80, true);
    minMaxTable.addCellInLastRow(maximumColor);
    form.controls.maximumColor = maximumColor;

    maximumColor.action = function () {
        form.setPropertyValue("MaximumColor", this.key);
    }

    //Show Hyperlink
    var showHyperlink = this.CheckBox(null, this.loc.PropertyEnum.StiInteractionOnHoverShowHyperlink);
    form.addControlRow(controlsTable, " ", "showHyperlink", showHyperlink, "8px 12px 8px 0");

    showHyperlink.action = function () {
        form.controls.hyperlinkPatternRow.style.display = this.isChecked ? "" : "none";
        form.setPropertyValue("ShowHyperlink", this.isChecked);
    }

    //Hyperlink Pattern
    var hyperlinkPattern = this.HyperlinkControlWithMenu("tableElementHyperlinkPattern", 180, [], false, "www.{value}.com");
    form.addControlRow(controlsTable, this.loc.PropertyMain.Pattern, "hyperlinkPattern", hyperlinkPattern, "6px 12px 6px 0", null, true);

    hyperlinkPattern.action = function () {
        form.setPropertyValue("HyperlinkPattern", StiBase64.encode(this.textBox.value));
    }

    hyperlinkPattern.preOpening = function () {
        this.columnNames = dataContainer.getColumnNames();
    }

    hyperlinkPattern.button.action = function () {
        var expressionItems = jsObject.GetInsertExpressionItems("TableColumn", dataContainer.getColumnNames());
        hyperlinkPattern.addItems(expressionItems);
        hyperlinkPattern.menu.changeVisibleState(!hyperlinkPattern.menu.visible);
    }

    //SparkLines Types
    var sparklinesTypes = ["Line", "Area", "Column", "WinLoss"];
    var sparklinesTable = this.CreateHTMLTable();
    sparklinesTable.buttons = {};
    for (var i = 0; i < sparklinesTypes.length; i++) {
        var button = this.FormButtonWithThemeBorder(null, null, null, "Meters.Sparklines" + sparklinesTypes[i] + ".png", this.loc.Chart["Sparklines" + sparklinesTypes[i]]);
        button.sparklinesType = sparklinesTypes[i];
        button.style.marginRight = "6px";
        sparklinesTable.addCell(button);
        sparklinesTable.buttons[sparklinesTypes[i]] = button;

        button.action = function () {
            this.select();
            var itemIndex = dataContainer.getSelectedItemIndex();

            form.sendCommand({ command: "ChangeSparklinesType", itemIndex: itemIndex, sparklinesType: this.sparklinesType },
                function (answer) {
                    dataContainer.updateMeters(answer.elementProperties.meters, itemIndex);
                    form.updateElementProperties(answer.elementProperties);
                    form.updateSvgContent(answer.elementProperties.svgContent);
                }
            );
        }

        button.select = function () {
            for (var name in sparklinesTable.buttons) {
                sparklinesTable.buttons[name].setSelected(false);
            }
            this.setSelected(true);
        }
    }
    form.addControlRow(controlsTable, this.loc.Dashboard.Sparklines, "sparklinesTable", sparklinesTable, "6px 12px 6px 0");

    //HighLowPoints
    var highLowPoints = this.CheckBox(null, this.loc.Dashboard.HighLowPoints);
    form.addControlRow(controlsTable, " ", "highLowPoints", highLowPoints, "8px 12px 8px 0");

    highLowPoints.action = function () {
        form.setPropertyValue("ShowHighLowPoints", this.isChecked);
    }

    //FirstLastPoints
    var firstLastPoints = this.CheckBox(null, this.loc.Dashboard.FirstLastPoints);
    form.addControlRow(controlsTable, " ", "firstLastPoints", firstLastPoints, "8px 12px 8px 0");

    firstLastPoints.action = function () {
        form.setPropertyValue("ShowFirstLastPoints", this.isChecked);
    }

    //AllowCustomColors
    var allowCustomColors = this.DropDownList("tableElementAllowCustomColors", 180, null, this.GetAllowCustomColorsItems(), true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Color, "allowCustomColors", allowCustomColors, "6px 12px 6px 0", null, true);

    allowCustomColors.action = function () {
        form.setPropertyValue("AllowCustomColors", this.key == "Custom");
    }

    //Positive Negative Color
    var posNegTable = this.CreateHTMLTable();
    posNegTable.style.maxWidth = "150px";
    posNegTable.addTextCell(this.loc.PropertyMain.Positive).className = "stiDesignerTextContainer";
    posNegTable.addCell();
    posNegTable.addTextCell(this.loc.PropertyMain.Negative).className = "stiDesignerTextContainer";
    form.addControlRow(controlsTable, " ", "posNegTable", posNegTable, "6px 12px 6px 0");

    var positiveColor = this.ColorControl("tableElementPositiveColor", null, null, 79, true);
    posNegTable.addCellInNextRow(positiveColor);
    form.controls.positiveColor = positiveColor;

    positiveColor.action = function () {
        form.setPropertyValue("PositiveColor", this.key);
    }

    posNegTable.addTextCellInLastRow(" - ").style.padding = this.options.isTouchDevice ? "10px 5px 10px 5px" : "7px 5px 7px 5px";
    var negativeColor = this.ColorControl("tableElementNegativeColor", null, null, 80, true);
    posNegTable.addCellInLastRow(negativeColor);
    form.controls.negativeColor = negativeColor;

    negativeColor.action = function () {
        form.setPropertyValue("NegativeColor", this.key);
    }

    var singlePositiveColor = this.ColorControl("tableElementSinglePositiveColor", null, null, 180, true);
    form.addControlRow(controlsTable, " ", "singlePositiveColor", singlePositiveColor, "8px 12px 8px 0");

    singlePositiveColor.action = function () {
        form.setPropertyValue("PositiveColor", this.key);
    }

    //Interaction
    var interaction = this.PropertyDashboardInteractionControl(null, 180);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Interaction, "interaction", interaction, "6px 12px 6px 0", null, true);

    interaction.action = function () {
        form.setPropertyValue("Interaction", this.key);
    }

    interaction.preOpening = function () {
        this.columnNames = dataContainer.getColumnNames();
    }

    expressionControl.refreshExpressionHint = function () {
        if (meterTypesTable.buttons.Dimension.isSelected)
            expressionControl.textBox.setAttribute("placeholder", jsObject.loc.PropertyMain.Field);

        else if (meterTypesTable.buttons.Measure.isSelected || meterTypesTable.buttons.DataBars.isSelected || meterTypesTable.buttons.ColorScale.isSelected)
            expressionControl.textBox.setAttribute("placeholder", "Sum(" + jsObject.loc.PropertyMain.Field + ")");

        else if (meterTypesTable.buttons.Sparklines.isSelected)
            expressionControl.textBox.setAttribute("placeholder", jsObject.loc.PropertyMain.Field);

        else if (meterTypesTable.buttons.Indicator.isSelected)
            expressionControl.textBox.setAttribute("placeholder", "Sum(" + jsObject.loc.PropertyMain.Field + ") / Sum(" + jsObject.loc.PropertyMain.Target + ")");
        else
            expressionControl.textBox.setAttribute("placeholder", "");
    }

    form.setPropertyValue = function (propertyName, propertyValue) {
        form.sendCommand(
            {
                command: "SetPropertyValue",
                propertyName: propertyName,
                propertyValue: propertyValue,
                itemIndex: dataContainer.getSelectedItemIndex()
            },
            function (answer) {
                dataContainer.updateMeters(answer.elementProperties.meters, dataContainer.getSelectedItemIndex());
                form.updateElementProperties(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
            }
        );
    }

    form.onshow = function () {
        form.currentPanelName = jsObject.options.propertiesPanel.getCurrentPanelName();
        if (jsObject.options.showDictionary) jsObject.options.propertiesPanel.showContainer("Dictionary");
        dataContainer.clear();
        form.sendCommand({ command: "GetTableElementProperties" },
            function (answer) {
                dataContainer.updateMeters(answer.elementProperties.meters, 0);
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

    form.sendCommand = function (updateParameters, callbackFunction) {
        jsObject.SendCommandToDesignerServer("UpdateTableElement",
            {
                componentName: form.currentTableElement.properties.name,
                updateParameters: updateParameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    form.updateSvgContent = function (svgContent) {
        this.currentTableElement.properties.svgContent = svgContent;
        this.currentTableElement.repaint();
    }

    form.updateElementProperties = function (properties) {
        for (var propertyName in properties) {
            this.currentTableElement.properties[propertyName] = properties[propertyName];
        }
        jsObject.options.homePanel.updateControls();

        if (jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.editDbsMeterMode) {
            jsObject.options.propertiesPanel.updateControls();
        }
    }

    return form;
}

StiMobileDesigner.prototype.TableElementDataContainer = function (width, height, showItemImage, form) {
    var jsObject = this;
    var dataContainer = this.DataContainer(width, height, showItemImage);
    this.AddDroppedContainerToCollection(dataContainer);

    dataContainer.updateMeters = function (meters, selectedIndex) {
        var oldScrollTop = this.scrollTop;
        this.clear();
        for (var i = 0; i < meters.length; i++) {
            this.addItem(meters[i].currentFunction ? meters[i].label + " (" + meters[i].currentFunction + ")" : meters[i].label, "Meters." + meters[i].typeIcon + ".png", meters[i]);
        }
        if (selectedIndex != null && selectedIndex >= 0 && selectedIndex < meters.length) {
            this.childNodes[selectedIndex].select();
        }
        this.scrollTop = oldScrollTop;
    }

    dataContainer.onmouseup = function (event) {
        if (event.button == 2) {
            event.stopPropagation();
            var point = jsObject.FindMousePosOnMainPanel(event);
            form.expressionMenu.show(point.xPixels + 3, point.yPixels + 3, "Down", "Right");
        }
        else if (jsObject.options.itemInDrag) {
            var itemObject = jsObject.CopyObject(jsObject.options.itemInDrag.originalItem.itemObject);
            if (!itemObject) return;
            var typeItem = itemObject.typeItem;
            var commandName = jsObject.options.CTRL_pressed ? "MoveAndDuplicateMeter" : "MoveMeter";

            if (typeItem == "Meter") {
                var toIndex = this.getOverItemIndex();
                var fromIndex = this.getItemIndex(jsObject.options.itemInDrag.originalItem);
                if (toIndex != null && fromIndex != null && fromIndex != toIndex) {
                    form.sendCommand({ command: commandName, toIndex: toIndex, fromIndex: fromIndex },
                        function (answer) {
                            dataContainer.updateMeters(answer.elementProperties.meters, toIndex);
                            form.updateElementProperties(answer.elementProperties);
                            form.updateSvgContent(answer.elementProperties.svgContent);
                        }
                    );

                    var fromIndex = this.getItemIndex(jsObject.options.itemInDrag.originalItem);
                }
            }
            else if (typeItem == "Column" || typeItem == "DataSource" || typeItem == "BusinessObject" || typeItem == "Variable") {
                var draggedItem = {
                    itemObject: itemObject
                };

                if (typeItem == "Column") {
                    var columnParent = jsObject.options.dictionaryTree.getCurrentColumnParent();
                    if (columnParent) {
                        draggedItem.currentParentType = columnParent.type;
                        draggedItem.currentParentName = (columnParent.type == "BusinessObject") ? jsObject.options.itemInDrag.originalItem.getBusinessObjectFullName() : columnParent.name;
                    }
                }
                else if (typeItem == "DataSource" || typeItem == "BusinessObject") {
                    draggedItem.currentParentType = typeItem;
                    draggedItem.currentParentName = itemObject.name;
                }

                var params = {
                    command: "InsertMeters",
                    draggedItem: draggedItem
                }

                if (typeItem == "Column" || typeItem == "Variable") {
                    params.insertIndex = dataContainer.getOverItemIndex();
                }

                form.sendCommand(params,
                    function (answer) {
                        dataContainer.updateMeters(answer.elementProperties.meters, params.insertIndex != null ? params.insertIndex : answer.elementProperties.meters.length - 1);
                        form.updateElementProperties(answer.elementProperties);
                        form.updateSvgContent(answer.elementProperties.svgContent);
                        form.correctTopPosition();
                    }
                );
            }
        }

        return false;
    }

    dataContainer.oncontextmenu = function (event) {
        return false;
    }

    dataContainer.onRemove = function (itemIndex) {
        form.sendCommand({ command: "RemoveMeter", itemIndex: itemIndex },
            function (answer) {
                dataContainer.updateMeters(answer.elementProperties.meters, dataContainer.getSelectedItemIndex());
                form.updateElementProperties(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
            }
        );
    }

    dataContainer.onAction = function (actionName) {
        var controls = form.controls;
        var itemObject = this.selectedItem ? this.selectedItem.itemObject : null;

        if (itemObject) {
            if (actionName == "rename") {
                var itemIndex = dataContainer.getSelectedItemIndex();

                form.sendCommand({ command: "RenameMeter", itemIndex: itemIndex, newLabel: itemObject.label },
                    function (answer) {
                        dataContainer.updateMeters(answer.elementProperties.meters, itemIndex);
                        form.updateElementProperties(answer.elementProperties);
                        form.updateSvgContent(answer.elementProperties.svgContent);
                    }
                );
                return;
            }

            if (controls.meterTypesTable.buttons[itemObject.type]) {
                controls.meterTypesTable.buttons[itemObject.type].select();
            }
            if (itemObject.type == "Sparklines") {
                if (controls.sparklinesTable.buttons[itemObject.sparklinesType]) {
                    controls.sparklinesTable.buttons[itemObject.sparklinesType].select();
                }
                controls.highLowPoints.setChecked(itemObject.showHighLowPoints);
                controls.firstLastPoints.setChecked(itemObject.showFirstLastPoints);
                controls.singlePositiveColor.setKey(itemObject.positiveColor);
            }
            if (itemObject.type == "Sparklines" || itemObject.type == "Bubble") {
                controls.allowCustomColors.setKey(itemObject.allowCustomColors ? "Custom" : "FromStyle");
                controls.positiveColor.setKey(itemObject.positiveColor);
                controls.negativeColor.setKey(itemObject.negativeColor);
            }
            controls.expressionControl.textBox.value = StiBase64.decode(itemObject.expression);
            controls.visibility.setKey(itemObject.visibility);
            controls.visibilityExpression.textBox.value = StiBase64.decode(itemObject.visibilityExpression);

            if (itemObject.type == "Dimension") {
                controls.showHyperlink.setChecked(itemObject.showHyperlink);
                controls.hyperlinkPattern.textBox.value = StiBase64.decode(itemObject.hyperlinkPattern);
            }

            if (itemObject.type == "ColorScale") {
                controls.minimumColor.setKey(itemObject.minimumColor);
                controls.maximumColor.setKey(itemObject.maximumColor);
            }

            controls.interaction.setKey(itemObject.interaction);
            controls.summaryType.setKey(itemObject.summaryType);
            controls.showTotalSummary.setChecked(itemObject.showTotalSummary);
        }

        var propertiesPanel = jsObject.options.propertiesPanel;

        if (actionName == "select" && this.selectedItem) {
            propertiesPanel.setEditDbsMeterMode(true);
            propertiesPanel.editDbsMeterPropertiesPanel.updateProperties(form, this.selectedItem);
        }
        else if (actionName == "remove" && propertiesPanel.editDbsMeterMode && this.getCountItems() == 0) {
            propertiesPanel.setEditDbsMeterMode(false);
        }

        controls.meterTypesTableRow.style.display = controls.expressionControlRow.style.display = controls.expressionControlCaptionRow.style.display = itemObject ? "" : "none";
        controls.sparklinesTableRow.style.display = itemObject && controls.meterTypesTable.buttons.Sparklines.isSelected ? "" : "none";

        controls.visibilityRow.style.display = itemObject ? "" : "none";
        controls.visibilityExpressionRow.style.display = itemObject && controls.visibility.key == "Expression" ? "" : "none";

        controls.highLowPointsRow.style.display = controls.firstLastPointsRow.style.display =
            itemObject && controls.meterTypesTable.buttons.Sparklines.isSelected &&
                (controls.sparklinesTable.buttons.Line.isSelected || controls.sparklinesTable.buttons.Area.isSelected) ? "" : "none";

        controls.showHyperlinkRow.style.display = itemObject && itemObject.type == "Dimension" ? "" : "none";
        controls.hyperlinkPatternRow.style.display = controls.showHyperlinkRow.style.display == "" && controls.showHyperlink.isChecked ? "" : "none";
        controls.minMaxTableRow.style.display = itemObject && itemObject.type == "ColorScale" ? "" : "none";
        controls.allowCustomColorsRow.style.display = itemObject && (itemObject.type == "Sparklines" || itemObject.type == "Bubble") ? "" : "none";

        controls.posNegTableRow.style.display = itemObject && controls.allowCustomColors.key == "Custom" &&
            (itemObject.type == "Sparklines" && (controls.sparklinesTable.buttons.Column.isSelected || controls.sparklinesTable.buttons.WinLoss.isSelected) || itemObject.type == "Bubble") ? "" : "none";

        controls.singlePositiveColorRow.style.display =
            itemObject && itemObject.type == "Sparklines" && controls.allowCustomColors.key == "Custom" &&
                (controls.sparklinesTable.buttons.Line.isSelected || controls.sparklinesTable.buttons.Area.isSelected) ? "" : "none";

        controls.showTotalSummaryRow.style.display = itemObject ? "" : "none";
        controls.summaryTypeRow.style.display = itemObject && controls.showTotalSummary.isChecked ? "" : "none";
        controls.interactionRow.style.display = itemObject ? "" : "none";

        controls.expressionControl.refreshExpressionHint();

        form.correctTopPosition();
        jsObject.options.homePanel.updateControls();
    }

    dataContainer.getColumnNames = function () {
        var columns = [];
        if (this.getCountItems() > 0) {
            for (var i = 0; i < dataContainer.childNodes.length; i++) {
                var itemObject = this.childNodes[i].itemObject;
                if (itemObject) columns.push(itemObject.labelCorrect || itemObject.label);
            }
        }

        return columns;
    }

    return dataContainer;
}