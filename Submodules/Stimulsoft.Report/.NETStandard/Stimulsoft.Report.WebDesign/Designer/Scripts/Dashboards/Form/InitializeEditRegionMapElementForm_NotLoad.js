
StiMobileDesigner.prototype.InitializeEditRegionMapElementForm_ = function () {
    var form = this.DashboardBaseForm("editRegionMapElementForm", this.loc.Components.StiMap, 1, this.HelpLinks["regionMapElement"]);
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";
    form.hideButtonsPanel();
    var jsObject = this;

    var controlsTable = this.CreateHTMLTable();
    form.container.appendChild(controlsTable);
    form.container.style.padding = "0 0 6px 0";

    var expressionControl = this.ExpressionControlWithMenu(null, 200, null, null);
    var mapExpressionMenu = this.options.menus.mapExpressionMenu || this.InitializeDataColumnExpressionMenu("mapExpressionMenu", expressionControl, form);
    mapExpressionMenu.parentButton = expressionControl.button;
    expressionControl.menu = mapExpressionMenu;
    expressionControl.textBox.setAttribute("placeholder", jsObject.loc.PropertyMain.Field);

    expressionControl.action = function () {
        if (this.currentContainer) {
            form.applyExpressionPropertyToRegionMapElement(this.currentContainer, this.textBox.value);
        }
    }

    //Data From
    var dataFromControl = this.DropDownList("regionMapElementDataFrom", 200, null, this.GetChoroplethDataTypesItems(), true);
    form.addControlRow(controlsTable, this.loc.Adapters.AdapterConnection.replace("{0}", ""), "dataFrom", dataFromControl, "12px");

    dataFromControl.action = function () {
        form.updateControlsVisibleStates();
        form.applyPropertiesToRegionMapElement();
    }

    //DataColumns
    var dataColumns = [
        ["key", this.loc.PropertyMain.Key, 90],
        ["name", this.loc.PropertyMain.Name, 130],
        ["value", this.loc.PropertyMain.Value, 90],
        ["group", this.loc.PropertyEnum.StiMapTypeGroup, 90],
        ["color", this.loc.PropertyMain.Color, 90]
    ];

    for (var i = 0; i < dataColumns.length; i++) {
        var container = this.DashboardDataColumnContainer(form, dataColumns[i][0], dataColumns[i][1], null, null, mapExpressionMenu);
        container.allowSelected = true;
        container.maxWidth = 475;
        form.addControlRow(controlsTable, null, dataColumns[i][0] + "DataColumn", container, "0px 12px 0 12px");
        form.controls[dataColumns[i][0] + "DataColumnRow"].style.display = "none";

        container.action = function (actionName) {
            if (actionName == "rename" && this.dataColumnObject) {
                form.sendCommand({
                    command: "RenameMeter",
                    containerName: form.jsObject.UpperFirstChar(this.name),
                    newLabel: this.dataColumnObject.label
                },
                    function (answer) {
                        form.updateControls(answer.elementProperties);
                        form.updateSvgContent(answer.elementProperties.svgContent);
                    }
                );
                return;
            }
            if (!this.dataColumnObject) {
                form.controls.expression.currentContainer = null;
                form.controls.expression.setEnabled(false);
            }
            form.applyDataColumnPropertyToRegionMapElement(this);
        }

        container.onSelected = function () {
            for (var i = 0; i < dataColumns.length; i++) {
                if (form.controls[dataColumns[i][0] + "DataColumn"] != this)
                    form.controls[dataColumns[i][0] + "DataColumn"].setSelected(false);
            }

            if (this.dataColumnObject && this.dataColumnObject.expression != null) {
                form.controls.expression.currentContainer = this;
                form.controls.expression.setEnabled(true);
                form.controls.expression.textBox.value = StiBase64.decode(this.dataColumnObject.expression);
            }
        }
    }

    form.addControlRow(controlsTable, this.loc.PropertyMain.Expression, "expression", expressionControl, "12px 12px 6px 12px");

    //DataGrid
    var dataGridView = document.createElement("div");
    dataGridView.headers = {};
    dataGridView.columns = {};
    dataGridView.style.height = "268px";
    form.addControlRow(controlsTable, null, "dataGridView", dataGridView, "0 12px 0 12px");

    var dataGridHeader = this.CreateHTMLTable();
    dataGridHeader.style.borderCollapse = "collapse";
    dataGridHeader.className = "stiMapDataGridHeader";

    var dataGridTable = this.CreateHTMLTable();
    dataGridTable.className = "stiMapDataGrid";
    dataGridView.appendChild(dataGridHeader);

    var scrollContainer = document.createElement("div");
    scrollContainer.style.height = "248px";
    scrollContainer.className = "stiMapScrollContainer";
    dataGridView.appendChild(scrollContainer);
    scrollContainer.appendChild(dataGridTable);

    for (var i = 0; i < dataColumns.length; i++) {
        var headerCell = dataGridHeader.addTextCell(dataColumns[i][1]);
        headerCell.style.width = (dataColumns[i][2] + 4) + "px";
        dataGridView.headers[dataColumns[i][0]] = headerCell;

        var columnCell = dataGridTable.addCell();
        columnCell.style.width = "1px";
        dataGridView.columns[dataColumns[i][0]] = columnCell;
    }

    dataGridView.fillData = function (data) {
        //clear columns
        for (var i = 0; i < dataColumns.length; i++) {
            while (dataGridView.columns[dataColumns[i][0]].childNodes[0])
                dataGridView.columns[dataColumns[i][0]].removeChild(dataGridView.columns[dataColumns[i][0]].childNodes[0]);
        }

        //add new data
        if (data) {
            for (var i = 0; i < data.length; i++) {
                for (var k = 0; k < dataColumns.length; k++) {
                    var textBox = jsObject.TextBox(null, dataColumns[k][2]);

                    if (dataColumns[k][0] == "key") {
                        textBox.readOnly = true;
                    }

                    if (k != 0) {
                        textBox.style.borderLeft = "0";
                    }

                    textBox.style.borderTop = "0";
                    textBox.style.borderRadius = "0";

                    if (data[i][dataColumns[k][0]]) {
                        textBox.value = data[i][dataColumns[k][0]];
                    }

                    textBox.rowIndex = i;
                    textBox.columnName = dataColumns[k][0];

                    dataGridView.columns[dataColumns[k][0]].appendChild(textBox);

                    textBox.action = function () {
                        form.sendCommand(
                            {
                                command: "UpdateMapData",
                                rowIndex: this.rowIndex,
                                columnName: this.columnName,
                                textValue: this.value
                            },
                            function (answer) {
                                form.updateControls(answer.elementProperties);
                                form.updateSvgContent(answer.elementProperties.svgContent);
                            }
                        );
                    }
                }
            }
        }
    }

    //Map ID
    var mapIDControl = this.MapIDControl("editREgionMapFormMapID", 450);
    form.addControlRow(controlsTable, null, "mapID", mapIDControl, "12px 12px 6px 12px");

    mapIDControl.action = function () {
        form.applyPropertiesToRegionMapElement(true);
        form.style.display = "";
    }

    //Map Type
    var mapTypeControl = this.DropDownList("regionMapElementMapType", 200, null, this.GetChoroplethMapTypesItems(), true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.MapType, "mapType", mapTypeControl, "6px 12px 6px 12px");

    mapTypeControl.action = function () {
        form.updateControlsVisibleStates();
        form.applyPropertiesToRegionMapElement();
    }

    //Display Name Type
    var displayNameTypeControl = this.DropDownList("regionMapElementDisplayName", 200, null, this.GetMapDisplayNameTypeItems(), true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.DisplayNameType, "displayNameType", displayNameTypeControl, "6px 12px 6px 12px");

    displayNameTypeControl.action = function () {
        form.applyPropertiesToRegionMapElement();
    }

    //Language
    var languageControl = this.DropDownList(null, 200, null, [], true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Language, "language", languageControl, "6px 12px 6px 12px");

    languageControl.action = function () {
        form.applyPropertiesToRegionMapElement();
    }

    //Show Value
    var showValueCheckBox = this.CheckBox(null, this.loc.PropertyMain.ShowValue);
    form.addControlRow(controlsTable, " ", "showValue", showValueCheckBox, "8px 12px 8px 12px");

    showValueCheckBox.action = function () {
        form.applyPropertiesToRegionMapElement();
    }

    //Color Each
    var colorEachCheckBox = this.CheckBox(null, this.loc.PropertyMain.ColorEach);
    form.addControlRow(controlsTable, " ", "colorEach", colorEachCheckBox, "8px 12px 8px 12px");

    colorEachCheckBox.action = function () {
        form.applyPropertiesToRegionMapElement();
        dataGridView.updateColumnsState();
    }

    //Show Bubble
    var showBubbleCheckBox = this.CheckBox(null, this.loc.PropertyMain.ShowBubble);
    form.addControlRow(controlsTable, " ", "showBubble", showBubbleCheckBox, "8px 12px 8px 12px");

    showBubbleCheckBox.action = function () {
        form.applyPropertiesToRegionMapElement();
    }

    dataGridView.updateColumnsState = function () {
        dataGridView.headers.color.style.display = dataGridView.columns.color.style.display = mapTypeControl.key == "Individual" && colorEachCheckBox.isChecked ? "" : "none";
        dataGridView.headers.group.style.display = dataGridView.columns.group.style.display = mapTypeControl.key == "Group" || mapTypeControl.key == "HeatmapWithGroup" ? "" : "none";
    }

    form.setValues = function () {
        mapTypeControl.setKey(this.mapProperties.mapType);
        mapIDControl.setKey(this.mapProperties.mapID);
        displayNameTypeControl.setKey(this.mapProperties.displayNameType);
        showValueCheckBox.setChecked(this.mapProperties.showValue);
        colorEachCheckBox.setChecked(this.mapProperties.colorEach);
        showBubbleCheckBox.setChecked(this.mapProperties.showBubble);
        dataFromControl.setKey(this.mapProperties.dataFrom);
        dataGridView.fillData(this.mapProperties.mapData);
        expressionControl.textBox.value = "";
        expressionControl.setEnabled(false);

        if (this.mapProperties.languages) {
            var items = [];
            for (var i = 0; i < this.mapProperties.languages.length; i++) {
                items.push(jsObject.Item("item" + i, this.mapProperties.languages[i].langName, null, this.mapProperties.languages[i].langKey));
            }
            languageControl.addItems(items);
        }
        languageControl.setKey(this.mapProperties.language);

        var meters = this.mapProperties.meters;

        for (var i = 0; i < dataColumns.length; i++) {
            var meter = meters[dataColumns[i][0]];
            var container = this.controls[dataColumns[i][0] + "DataColumn"];

            if (meter) {
                container.addColumn(meter.label, meter);
                if (container.isSelected && container.item) container.item.action();
            }
            else
                container.clear();
        }
    }

    form.getValues = function () {
        var props = {
            mapType: mapTypeControl.key,
            mapID: mapIDControl.key,
            showValue: showValueCheckBox.isChecked,
            colorEach: colorEachCheckBox.isChecked,
            showBubble: showBubbleCheckBox.isChecked,
            displayNameType: displayNameTypeControl.key,
            mapData: {},
            dataFrom: dataFromControl.key,
            language: languageControl.key,
        }

        return props;
    }

    form.updateControlsVisibleStates = function () {
        this.controls.keyDataColumnRow.style.display = dataFromControl.key == "DataColumns" ? "" : "none";
        this.controls.nameDataColumnRow.style.display = dataFromControl.key == "DataColumns" ? "" : "none";
        this.controls.valueDataColumnRow.style.display = dataFromControl.key == "DataColumns" ? "" : "none";
        this.controls.groupDataColumnRow.style.display = dataFromControl.key == "DataColumns" && (mapTypeControl.key == "Group" || mapTypeControl.key == "HeatmapWithGroup") ? "" : "none";
        this.controls.colorDataColumnRow.style.display = dataFromControl.key == "DataColumns" && mapTypeControl.key == "Individual" ? "" : "none";
        this.controls.expressionRow.style.display = dataFromControl.key == "DataColumns" ? "" : "none";
        this.controls.colorEachRow.style.display = mapTypeControl.key == "Individual" ? "" : "none";
        this.controls.languageRow.style.display = this.mapProperties.languages && this.mapProperties.languages.length > 0 ? "" : "none";
        dataGridView.style.display = dataFromControl.key == "Manual" ? "" : "none";
        dataGridView.updateColumnsState();
    }

    form.onshow = function () {
        form.currentPanelName = jsObject.options.propertiesPanel.getCurrentPanelName();
        if (jsObject.options.showDictionary) jsObject.options.propertiesPanel.showContainer("Dictionary");
        expressionControl.textBox.value = "";
        expressionControl.setEnabled(false);
        form.controls.colorEachRow.style.display = "none";

        for (var i = 0; i < dataColumns.length; i++) {
            var container = this.controls[dataColumns[i][0] + "DataColumn"];
            container.clear();
            this.controls[dataColumns[i][0] + "DataColumnRow"].style.display = "none";
        }

        form.sendCommand({ command: "GetRegionMapElementProperties" },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.correctTopPosition();
            }
        );
    }

    form.onhide = function () {
        jsObject.options.propertiesPanel.showContainer(form.currentPanelName);
    }

    form.updateControls = function (mapProperties) {
        form.mapProperties = mapProperties;
        form.setValues();
        form.updateControlsVisibleStates();
        for (var propName in mapProperties) {
            form.currentRegionMapElement.properties[propName] = mapProperties[propName];
        }
    }

    form.applyPropertiesToRegionMapElement = function (updateMapData) {
        form.sendCommand(
            {
                command: "SetProperties",
                properties: form.getValues(),
                updateMapData: updateMapData
            },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.updateElementProperties(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
                form.correctTopPosition();
                if (updateMapData) {
                    jsObject.RemoveStylesFromCache(answer.elementProperties.name);
                }
            }
        );
    }

    form.applyDataColumnPropertyToRegionMapElement = function (container) {
        form.sendCommand(
            {
                command: "SetDataColumn",
                containerName: jsObject.UpperFirstChar(container.name),
                dataColumnObject: container.dataColumnObject
            },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
                if (container.item) container.item.action();
            }
        );
    }

    form.applyExpressionPropertyToRegionMapElement = function (container, expressionValue) {
        if (container) {
            form.sendCommand(
                {
                    command: "SetExpression",
                    containerName: jsObject.UpperFirstChar(container.name),
                    expressionValue: StiBase64.encode(expressionValue)
                },
                function (answer) {
                    form.updateControls(answer.elementProperties);
                    form.updateSvgContent(answer.elementProperties.svgContent);
                }
            );
        }
    }

    form.sendCommand = function (updateParameters, callbackFunction) {
        jsObject.SendCommandToDesignerServer("UpdateRegionMapElement",
            {
                componentName: form.currentRegionMapElement.properties.name,
                updateParameters: updateParameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    form.updateSvgContent = function (svgContent) {
        this.currentRegionMapElement.properties.svgContent = svgContent;
        this.currentRegionMapElement.repaint();
    }

    form.updateElementProperties = function (properties) {
        for (var propertyName in properties) {
            this.currentRegionMapElement.properties[propertyName] = properties[propertyName];
        }
        jsObject.options.homePanel.updateControls();
        if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
    }

    return form;
}