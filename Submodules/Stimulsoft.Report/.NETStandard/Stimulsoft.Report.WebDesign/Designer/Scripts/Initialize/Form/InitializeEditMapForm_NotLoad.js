
StiMobileDesigner.prototype.InitializeEditMapForm_ = function () {
    var form = this.BaseFormPanel("editMapForm", this.loc.Components.StiMap, 1);
    var jsObject = this;

    //Main Table
    var mainTable = this.CreateHTMLTable();
    mainTable.className = "stiDesignerImageFormMainPanel";
    form.container.appendChild(mainTable);
    form.container.style.padding = "0px";

    //Buttons
    var buttonProps = [
        ["Choropleth", "Maps.MapChoropleth.png", this.loc.PropertyEnum.StiMapModeChoropleth],
        ["Online", "Maps.MapOnline.png", this.loc.PropertyEnum.StiMapModeOnline]
    ];

    //Add Panels && Buttons
    var buttonsPanel = mainTable.addCell();
    var panelsContainer = mainTable.addCell();
    buttonsPanel.style.paddingTop = "6px";
    buttonsPanel.style.verticalAlign = "top";
    form.mainButtons = {};
    form.panels = {};

    for (var i = 0; i < buttonProps.length; i++) {
        var panel = document.createElement("Div");
        panel.className = "stiDesignerEditFormPanel";
        panel.style.width = "550px";
        panel.style.height = "";
        panel.style.minHeight = "520px";

        if (i != 0) panel.style.display = "none";
        panelsContainer.appendChild(panel);
        form.panels[buttonProps[i][0]] = panel;

        var button = this.FormTabPanelButton(null, buttonProps[i][2], buttonProps[i][1], buttonProps[i][2], null, { width: 24, height: 24 }, 34);
        form.mainButtons[buttonProps[i][0]] = button;
        buttonsPanel.appendChild(button);
        button.panelName = buttonProps[i][0];

        button.action = function () {
            form.setMode(this.panelName);
            form.applyPropertiesToMapComponent();
        }
    }

    var choroplethTable = this.CreateHTMLTable();
    choroplethTable.style.width = "100%";
    form.panels.Choropleth.appendChild(choroplethTable);

    //Data From
    var dataFromControl = this.DropDownList(null, 200, null, this.GetChoroplethDataTypesItems(), true);
    form.addControlRow(choroplethTable, this.loc.Adapters.AdapterConnection.replace("{0}", ""), "dataFrom", dataFromControl, "12px");

    dataFromControl.action = function () {
        form.updateControlsStates();
        form.applyPropertiesToMapComponent();
    }

    //DataColumns
    var dataColumns = [
        ["key", this.loc.PropertyMain.Key, 100],
        ["name", this.loc.PropertyMain.Name, 140],
        ["value", this.loc.PropertyMain.Value, 100],
        ["group", this.loc.PropertyEnum.StiMapTypeGroup, 90],
        ["color", this.loc.PropertyMain.Color, 90]
    ];

    for (var i = 0; i < dataColumns.length; i++) {
        var container = this.DataColumnContainer(dataColumns[i][0], dataColumns[i][1], null, true);
        container.headerCell.style.padding = "6px 0 6px 0";
        form.addControlRow(choroplethTable, null, dataColumns[i][0] + "DataColumn", container, (dataColumns[i][0] == "key" ? "0px" : "6px") + " 12px 6px 12px");

        container.action = function () {
            form.applyPropertiesToMapComponent();
        }
    }

    //DataGrid
    var dataGridView = document.createElement("div");
    dataGridView.headers = {};
    dataGridView.columns = {};
    dataGridView.style.height = "268px";
    form.addControlRow(choroplethTable, null, "dataGridView", dataGridView, "0 12px 6px 12px");

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
                    textBox.style.textOverflow = "ellipsis";
                    if (k != 0) {
                        textBox.style.borderLeft = "0";
                    }
                    textBox.style.borderTop = "0";
                    textBox.style.borderRadius = "0";
                    textBox.rowIndex = i;
                    textBox.columnName = dataColumns[k][0];

                    if (dataColumns[k][0] == "key") {
                        textBox.readOnly = true;
                    }

                    if (data[i][dataColumns[k][0]]) {
                        textBox.value = data[i][dataColumns[k][0]];
                    }

                    textBox.action = function () {
                        jsObject.SendCommandUpdateMapData(form.mapProperties.name, this.rowIndex, this.columnName, this.value);
                    }

                    dataGridView.columns[dataColumns[k][0]].appendChild(textBox);
                }
            }
        }
    }

    //Map ID
    var mapIDControl = this.MapIDControl("editMapFormMapID", 524);
    form.addControlRow(choroplethTable, null, "mapID", mapIDControl, "6px 0 6px 12px");

    mapIDControl.action = function () {
        form.applyPropertiesToMapComponent(true);
        form.style.display = "";
    }

    //Map Type
    var mapTypeControl = this.DropDownList(null, 200, null, this.GetChoroplethMapTypesItems(), true);
    form.addControlRow(choroplethTable, this.loc.PropertyMain.MapType, "mapType", mapTypeControl, "6px 12px 6px 12px");

    mapTypeControl.action = function () {
        form.updateControlsStates();
        form.applyPropertiesToMapComponent();
    }

    //Display Name Type
    var displayNameTypeControl = this.DropDownList(null, 200, null, this.GetMapDisplayNameTypeItems(), true);
    form.addControlRow(choroplethTable, this.loc.PropertyMain.DisplayNameType, "displayNameType", displayNameTypeControl, "6px 12px 6px 12px");

    displayNameTypeControl.action = function () {
        form.applyPropertiesToMapComponent();
    }

    //Language
    var languageControl = this.DropDownList(null, 200, null, [], true);
    form.addControlRow(choroplethTable, this.loc.PropertyMain.Language, "language", languageControl, "6px 12px 6px 12px");

    languageControl.action = function () {
        form.applyPropertiesToMapComponent();
    }

    //Show Value
    var showValueCheckBox = this.CheckBox(null, this.loc.PropertyMain.ShowValue);
    form.addControlRow(choroplethTable, " ", "showValue", showValueCheckBox, "8px 12px 8px 12px");

    showValueCheckBox.action = function () {
        form.applyPropertiesToMapComponent();
    }

    //Color Each
    var colorEachCheckBox = this.CheckBox(null, this.loc.PropertyMain.ColorEach);
    form.addControlRow(choroplethTable, " ", "colorEach", colorEachCheckBox, "8px 12px 0px 12px");

    colorEachCheckBox.action = function () {
        form.applyPropertiesToMapComponent();
        dataGridView.updateColumnsState();
    }

    dataGridView.updateColumnsState = function () {
        dataGridView.headers.color.style.display = dataGridView.columns.color.style.display = mapTypeControl.key == "Individual" && colorEachCheckBox.isChecked ? "" : "none";
        dataGridView.headers.group.style.display = dataGridView.columns.group.style.display = mapTypeControl.key == "Group" || mapTypeControl.key == "HeatmapWithGroup" ? "" : "none";
    }

    //OnlineDataColumns
    var onlineDataColumns = [
        ["latitude", this.loc.PropertyMain.Latitude],
        ["longitude", this.loc.PropertyMain.Longitude]
    ];

    for (var i = 0; i < onlineDataColumns.length; i++) {
        var container = this.DataColumnContainer(onlineDataColumns[i][0], onlineDataColumns[i][1], null, true);
        container.style.margin = "0px 12px " + (i == onlineDataColumns.length - 1 ? "12px" : "0px") + " 12px";
        form.panels.Online.appendChild(container);
        form.controls[onlineDataColumns[i][0] + "DataColumn"] = container;

        container.action = function () {
            form.applyPropertiesToMapComponent();
        }
    }

    form.setMode = function (mode) {
        form.mode = mode;
        for (var panelName in form.panels) {
            form.panels[panelName].style.display = mode == panelName ? "" : "none";
            form.mainButtons[panelName].setSelected(mode == panelName);
        }
    }

    form.setValues = function () {
        form.setMode(this.mapProperties.mapMode);
        mapTypeControl.setKey(this.mapProperties.mapType);
        mapIDControl.setKey(this.mapProperties.mapID);
        displayNameTypeControl.setKey(this.mapProperties.displayNameType);
        showValueCheckBox.setChecked(this.mapProperties.showValue);
        colorEachCheckBox.setChecked(this.mapProperties.colorEach);
        dataFromControl.setKey(this.mapProperties.dataFrom);
        dataGridView.fillData(this.mapProperties.mapData);

        if (this.mapProperties.languages) {
            var items = [];
            for (var i = 0; i < this.mapProperties.languages.length; i++) {
                items.push(jsObject.Item("item" + i, this.mapProperties.languages[i].langName, null, this.mapProperties.languages[i].langKey));
            }
            languageControl.addItems(items);
        }
        languageControl.setKey(this.mapProperties.language);

        var columnsNames = ["key", "name", "value", "color", "group", "latitude", "longitude"];
        for (var i = 0; i < columnsNames.length; i++) {
            var dataColumnValue = this.mapProperties[columnsNames[i] + "DataColumn"];

            if (dataColumnValue)
                this.controls[columnsNames[i] + "DataColumn"].addColumn(dataColumnValue);
            else
                this.controls[columnsNames[i] + "DataColumn"].clear();
        }
    }

    form.getValues = function () {
        var props = {
            mapMode: form.mode,
            mapType: mapTypeControl.key,
            mapID: mapIDControl.key,
            showValue: showValueCheckBox.isChecked,
            colorEach: colorEachCheckBox.isChecked,
            displayNameType: displayNameTypeControl.key,
            language: languageControl.key,
            mapData: {},
            dataFrom: dataFromControl.key,
            keyDataColumn: this.controls.keyDataColumn.dataColumn,
            nameDataColumn: this.controls.nameDataColumn.dataColumn,
            valueDataColumn: this.controls.valueDataColumn.dataColumn,
            groupDataColumn: this.controls.groupDataColumn.dataColumn,
            colorDataColumn: this.controls.colorDataColumn.dataColumn,
            latitudeDataColumn: this.controls.latitudeDataColumn.dataColumn,
            longitudeDataColumn: this.controls.longitudeDataColumn.dataColumn,
        }

        return props;
    }

    form.updateControlsStates = function () {
        this.controls.keyDataColumnRow.style.display = dataFromControl.key == "DataColumns" ? "" : "none";
        this.controls.nameDataColumnRow.style.display = dataFromControl.key == "DataColumns" ? "" : "none";
        this.controls.valueDataColumnRow.style.display = dataFromControl.key == "DataColumns" ? "" : "none";
        this.controls.groupDataColumnRow.style.display = dataFromControl.key == "DataColumns" && (mapTypeControl.key == "Group" || mapTypeControl.key == "HeatmapWithGroup") ? "" : "none";
        this.controls.colorDataColumnRow.style.display = dataFromControl.key == "DataColumns" && mapTypeControl.key == "Individual" ? "" : "none";
        this.controls.languageRow.style.display = this.mapProperties.languages && this.mapProperties.languages.length > 0 ? "" : "none";
        dataGridView.style.display = dataFromControl.key == "Manual" ? "" : "none";
        dataGridView.updateColumnsState();
    }

    form.onshow = function () {
        form.jsObject.options.propertiesPanel.setDictionaryMode(true);
        form.jsObject.options.propertiesPanel.setEnabled(true);
        form.setValues();
        form.updateControlsStates();
    }

    form.onhide = function () {
        form.jsObject.options.propertiesPanel.setDictionaryMode(false);
    }

    form.cancelAction = function () {
        var mapComp = this.jsObject.options.report.getComponentByName(form.mapProperties.name);
        if (mapComp) {
            mapComp.properties.svgContent = form.mapSvgContent;
            mapComp.properties.iframeContent = form.mapIframeContent;
            mapComp.properties.cultures = form.mapCultures;
            mapComp.repaint();
        }
        form.jsObject.SendCommandCanceledEditComponent(form.mapProperties.name);
    }

    form.applyPropertiesToMapComponent = function (updateMapData) {
        form.jsObject.SendCommandSetMapProperties(form.mapProperties.name, form.getValues(), updateMapData);
    }

    form.action = function () {
        form.changeVisibleState(false);
        form.jsObject.RemoveStylesFromCache(form.currentMapComponent.properties.name, "StiMap");
        form.jsObject.SendCommandSendProperties(form.currentMapComponent, []);
    }

    return form;
}