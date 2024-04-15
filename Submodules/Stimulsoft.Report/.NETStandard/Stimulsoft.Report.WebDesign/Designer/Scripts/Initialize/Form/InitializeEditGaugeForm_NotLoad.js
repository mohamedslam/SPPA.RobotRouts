
StiMobileDesigner.prototype.InitializeEditGaugeForm_ = function () {
    //Edit Chart Form
    var form = this.BaseFormPanel("editGauge", this.loc.Components.StiGauge, 1);

    form.addControlRow = function (table, textControl, controlName, control, margin) {
        if (!this.controls) this.controls = {};
        this.controls[controlName] = control;
        this.controls[controlName + "Row"] = table.addRow();

        if (textControl != null) {
            var text = table.addCellInLastRow();
            this.controls[controlName + "Text"] = text;
            text.innerHTML = textControl;
            text.className = "stiDesignerCaptionControls";
            text.style.paddingLeft = "12px";
            text.style.minWidth = "70px";
        }

        if (control) {
            control.style.margin = margin;
            var controlCell = table.addCellInLastRow(control);
            if (textControl == null) controlCell.setAttribute("colspan", 2);
        }

        return controlCell;
    }

    var controlsTable = this.CreateHTMLTable();
    form.container.appendChild(controlsTable);
    form.container.style.padding = "0 0 6px 0";

    var valueTable = this.CreateHTMLTable();
    var headerCell = valueTable.addTextCell(this.loc.PropertyMain.Value);
    headerCell.className = "stiDesignerTextContainer";
    headerCell.style.padding = "12px 0 12px 0";

    var valueExpression = this.ExpressionControl(null, 280);
    valueTable.addCellInNextRow(valueExpression);
    form.addControlRow(controlsTable, null, "valueTable", valueTable, "6px 12px 6px 12px");
    valueExpression.action = function () {
        form.applyPropertiesToGaugeComponent();
    }

    //Type
    var types = ["FullCircular", "HalfCircular", "Linear", "HorizontalLinear", "Bullet"];
    var typesTable = this.CreateHTMLTable();
    typesTable.buttons = {};

    for (var i = 0; i < types.length; i++) {
        var button = this.FormButtonWithThemeBorder(null, null, null, "Gauge.Small." + types[i] + ".png", this.loc.PropertyEnum["StiGaugeType" + types[i]]);
        button.type = types[i];
        button.style.marginRight = "6px";
        typesTable.addCell(button);
        typesTable.buttons[types[i]] = button;

        button.action = function () {
            this.select();
            form.applyPropertiesToGaugeComponent();
        }

        button.select = function () {
            for (var name in typesTable.buttons) {
                typesTable.buttons[name].setSelected(false);
            }
            this.setSelected(true);
        }
    }

    form.addControlRow(controlsTable, this.loc.PropertyMain.Type, "typesTable", typesTable, "6px 12px 6px 12px");

    //CalculationMode
    var calculationMode = this.DropDownList(null, 161, null, this.GetGaugeCalculationModeItems(), true, null, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Mode, "calculationMode", calculationMode, "6px 12px 6px 12px");
    calculationMode.action = function () {
        form.updateControlsStates();
        form.applyPropertiesToGaugeComponent();
    }

    //Min Max
    var minMaxTable = this.CreateHTMLTable();
    minMaxTable.addTextCell(this.loc.PropertyMain.Minimum).className = "stiDesignerTextContainer";
    minMaxTable.addCell();
    minMaxTable.addTextCell(this.loc.PropertyMain.Maximum).className = "stiDesignerTextContainer";
    form.addControlRow(controlsTable, " ", "minMaxTable", minMaxTable, "6px 12px 6px 12px");

    var minControl = this.TextBoxEnumerator(null, 70);
    minMaxTable.addCellInNextRow(minControl);
    minControl.action = function () {
        form.applyPropertiesToGaugeComponent();
    }

    minMaxTable.addTextCellInLastRow(" - ").style.padding = "7px 5px 7px 5px";
    var maxControl = this.TextBoxEnumerator(null, 70);
    minMaxTable.addCellInLastRow(maxControl);
    maxControl.action = function () {
        form.applyPropertiesToGaugeComponent();
    }

    form.onshow = function () {
        form.setValues();
        form.updateControlsStates();
    }

    form.setValues = function () {
        valueExpression.textBox.value = this.gaugeProperties.indicatorColumn || "";
        if (typesTable.buttons[this.gaugeProperties.type]) {
            typesTable.buttons[this.gaugeProperties.type].select();
        }
        calculationMode.setKey(this.gaugeProperties.calculationMode);
        minControl.setValue(this.gaugeProperties.minimum);
        maxControl.setValue(this.gaugeProperties.maximum);
    }

    form.getValues = function () {
        var props = {
            indicatorColumn: valueExpression.textBox.value,
            calculationMode: calculationMode.key,
            minimum: minControl.getValue(),
            maximum: maxControl.getValue()
        }

        for (var i = 0; i < types.length; i++) {
            if (typesTable.buttons[types[i]].isSelected) {
                props.type = typesTable.buttons[types[i]].type;
                break;
            }
        }

        return props;
    }

    form.updateControlsStates = function () {
        this.controls.minMaxTableRow.style.display = calculationMode.key == "Custom" ? "" : "none";
    }

    form.cancelAction = function () {
        var gaugeComp = this.jsObject.options.report.getComponentByName(form.gaugeProperties.name);
        if (gaugeComp) {
            gaugeComp.properties.svgContent = form.gaugeSvgContent;
            gaugeComp.repaint();
        }
        form.jsObject.SendCommandCanceledEditComponent(form.gaugeProperties.name);
    }

    form.applyPropertiesToGaugeComponent = function () {
        form.jsObject.SendCommandSetGaugeProperties(form.gaugeProperties.name, form.getValues());
    }

    form.action = function () {
        form.changeVisibleState(false);
        form.jsObject.RemoveStylesFromCache(form.currentGaugeComponent.properties.name, "StiGauge");
        form.jsObject.SendCommandSendProperties(form.currentGaugeComponent, []);
    }

    return form;
}