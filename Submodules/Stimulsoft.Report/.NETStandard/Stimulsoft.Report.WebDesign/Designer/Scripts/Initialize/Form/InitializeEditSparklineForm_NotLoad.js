
StiMobileDesigner.prototype.InitializeEditSparklineForm_ = function () {
    var jsObject = this;
    var form = this.DashboardBaseForm("editSparkline", this.loc.Components.StiSparkline, 1, this.HelpLinks["sparkline"]);
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";

    var controlsTable = this.CreateHTMLTable();
    form.container.appendChild(controlsTable);
    form.container.style.padding = "6px 0 6px 0";

    var valueBlock = this.SparklineFormDataColumnBlock(form, "value", this.loc.PropertyMain.Value);
    valueBlock.style.width = "calc(100% - 24px)";
    form.addControlRow(controlsTable, null, "valueBlock", valueBlock, "0px 12px 0px 12px");

    var parentValueContainer = form.controls.valueBlock.container.parentElement;

    var fieldControl = this.PropertyDataControl("sparklineFormField", 250, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Field, "fieldControl", fieldControl, "12px 12px 6px 0");

    fieldControl.action = function () {
        form.setPropertyValue("ValueDataColumn", StiBase64.encode(this.textBox.value));
    }

    //SparkLines Types
    var sparklinesTypes = ["Line", "Area", "Column", "WinLoss"];
    var sparklinesTable = this.CreateHTMLTable();
    sparklinesTable.buttons = {};
    for (var i = 0; i < sparklinesTypes.length; i++) {
        var button = this.FormButtonWithThemeBorder(null, null, null, "Meters.Sparklines" + sparklinesTypes[i] + ".png", this.loc.Chart["Sparklines" + sparklinesTypes[i]]);
        button.sparklinesType = sparklinesTypes[i];
        button.style.marginRight = "5px";
        sparklinesTable.addCell(button);
        sparklinesTable.buttons[sparklinesTypes[i]] = button;

        button.action = function () {
            this.select();
            form.setPropertyValue("Type", this.sparklinesType);
            form.updateControlsVisibleStates();
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

    //Colors
    var colorsTable = this.CreateHTMLTable();
    form.addControlRow(controlsTable, this.loc.PropertyMain.Colors, "colorsTable", colorsTable, "6px 12px 6px 0");

    var positiveColor = this.ColorControl("sparklinePositiveColor", null, null, 79, true);
    colorsTable.addCell(positiveColor);
    form.controls.positiveColor = positiveColor;

    positiveColor.action = function () {
        form.setPropertyValue("PositiveColor", this.key);
    }

    var negativeColor = this.ColorControl("sparklineNegativeColor", null, null, 79, true);
    negativeColor.style.marginLeft = "12px";
    colorsTable.addCell(negativeColor);
    form.controls.negativeColor = negativeColor;

    negativeColor.action = function () {
        form.setPropertyValue("NegativeColor", this.key);
    }

    form.checkStartMode = function () {
        var valueContainer = valueBlock.container;

        if (valueBlock.container.getCountItems() == 0) {
            form.container.appendChild(valueContainer);
            controlsTable.style.display = "none";
            valueContainer.style.height = valueContainer.style.maxHeight = "231px";
            valueContainer.style.width = "337px";
            valueContainer.style.margin = "6px 12px 6px 12px";
        }
        else {
            parentValueContainer.appendChild(valueContainer);
            controlsTable.style.display = "";
            valueContainer.style.height = "auto";
            valueContainer.style.width = "auto";
            valueContainer.style.margin = "0";
            valueContainer.style.maxHeight = "100px";
        }
    }

    form.updateControlsValues = function () {
        var sparklineProps = form.sparklineProperties;
        var valueDataColumn = StiBase64.decode(sparklineProps.valueDataColumn);

        valueBlock.container.clear();
        if (valueDataColumn) {
            var item = valueBlock.container.addItem(valueDataColumn, null, { typeItem: "Meter", valueDataColumn: valueDataColumn });
            if (item) item.select();
        }

        if (sparklinesTable.buttons[sparklineProps.type]) {
            sparklinesTable.buttons[sparklineProps.type].select();
        }

        fieldControl.textBox.value = StiBase64.decode(sparklineProps.valueDataColumn);
        highLowPoints.setChecked(sparklineProps.showHighLowPoints);
        firstLastPoints.setChecked(sparklineProps.showFirstLastPoints);
        positiveColor.setKey(sparklineProps.positiveColor);
        negativeColor.setKey(sparklineProps.negativeColor);

        form.updateControlsVisibleStates();
    }

    form.updateSvgContent = function (svgContent) {
        this.currentSparklineComponent.properties.svgContent = svgContent;
        this.currentSparklineComponent.repaint();
    }

    form.updateComponentProperties = function (properties) {
        for (var propertyName in properties) {
            this.currentSparklineComponent.properties[propertyName] = properties[propertyName];
        }
        jsObject.options.homePanel.updateControls();
        if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
    }

    form.updateControlsVisibleStates = function () {
        var showCheckboxes = sparklinesTable.buttons.Line.isSelected || sparklinesTable.buttons.Area.isSelected;
        form.controls.highLowPointsRow.style.display = form.controls.firstLastPointsRow.style.display = showCheckboxes ? "" : "none";
        negativeColor.style.display = !showCheckboxes ? "" : "none";
        form.controls.colorsTableText.innerHTML = showCheckboxes ? jsObject.loc.PropertyMain.Color : jsObject.loc.PropertyMain.Colors;
    }

    form.setPropertyValue = function (propertyName, propertyValue) {
        var params = {
            innerCommand: "SetPropertyValue",
            componentName: form.sparklineProperties.name,
            propertyName: propertyName,
            propertyValue: propertyValue
        }
        jsObject.SendCommandToDesignerServer("UpdateSparkline", params,
            function (answer) {
                form.sparklineProperties = answer.sparklineProperties;
                form.updateControlsValues();
                form.checkStartMode();
                form.correctTopPosition();
                form.updateSvgContent(answer.sparklineProperties.svgContent);
                form.updateComponentProperties(answer.sparklineProperties);
            });
    }

    form.onshow = function () {
        form.currentPanelName = form.jsObject.options.propertiesPanel.getCurrentPanelName();
        jsObject.options.propertiesPanel.showContainer("Dictionary");
        form.updateControlsValues();
        form.checkStartMode();
        form.correctTopPosition();
    }

    form.onhide = function () {
        var propertiesPanel = jsObject.options.propertiesPanel;
        if (propertiesPanel) {
            propertiesPanel.showContainer(form.currentPanelName);
        }
    }

    form.cancelAction = function () {
        jsObject.SendCommandCanceledEditComponent(form.sparklineProperties.name);
        if (form.oldSvgContent) {
            form.currentSparklineComponent.properties.svgContent = form.oldSvgContent;
            form.currentSparklineComponent.repaint();
        }
    }

    form.action = function () {
        this.changeVisibleState(false);
    }

    return form;
}

StiMobileDesigner.prototype.SparklineFormDataColumnBlock = function (form, containerName, headerText) {
    var block = this.DashboardDataColumnsBlock(form, null, containerName, headerText, false, false);
    var container = block.container;
    var jsObject = this;

    container.onmouseup = function (event) {
        if (event.button != 2 && jsObject.options.itemInDrag) {
            var itemObject = jsObject.CopyObject(jsObject.options.itemInDrag.originalItem.itemObject);
            if (itemObject && (itemObject.typeItem == "Column" || itemObject.typeItem == "Variable")) {
                form.setPropertyValue("ValueDataColumn", StiBase64.encode(jsObject.options.itemInDrag.originalItem.getResultForEditForm(true)));
            }
        }
        return false;
    }

    container.onRemove = function () {
        form.setPropertyValue("ValueDataColumn", "");
    }

    return block;
}