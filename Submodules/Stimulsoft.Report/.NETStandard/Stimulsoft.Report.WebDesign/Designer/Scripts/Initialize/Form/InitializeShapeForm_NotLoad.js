
StiMobileDesigner.prototype.InitializeShapeForm_ = function () {
    var shapeForm = this.BaseFormPanel("shapeForm", this.loc.Components.StiShape, 1);
    shapeForm.isDockableToComponent = true;

    shapeForm.controls = {};
    var jsObject = this;

    //Properties
    var propertiesContainer = document.createElement("div");
    propertiesContainer.style.padding = "6px 0 6px 0";
    shapeForm.container.appendChild(propertiesContainer);

    var properties = [
        ["shapeType", this.loc.PropertyMain.Type, this.ShapeTypeControl("shapeFormShapeType", this.options.propertyControlWidth + 4), "6px 12px 6px 12px"],
        ["brush", this.loc.PropertyMain.Brush, this.PropertyBrushControl("shapeFormBrush", null, this.options.propertyControlWidth), "6px 12px 6px 12px"],
        ["separator1"],
        ["shapeBorderColor", this.loc.PropertyMain.BorderColor, this.PropertyColorControl("shapeFormBorderColor", null, this.options.propertyControlWidth), "6px 12px 6px 12px"],
        ["shapeBorderStyle", this.loc.PropertyMain.Style, this.PropertyDropDownList("shapeFormBorderStyle", this.options.propertyControlWidth, this.GetBorderStyleItems(), true, true), "6px 12px 6px 12px"],
        ["size", this.loc.PropertyMain.Size, this.PropertyTextBox("shapeFormSize", this.options.propertyNumbersControlWidth), "6px 12px 6px 12px"],
        ["separator2"],
        ["shapeText", this.loc.PropertyMain.Text, this.PropertyTextBox("shapeFormText", this.options.propertyControlWidth), "6px 12px 6px 12px"],
        ["font", this.loc.PropertyMain.Font, this.PropertyFontControl("shapeFormFont", "shapeFont", true), "6px 12px 6px 12px"],
        ["foreColor", this.loc.PropertyMain.ForeColor, this.PropertyColorControl("shapeFormForeColor", null, this.options.propertyControlWidth), "6px 12px 6px 12px"],
        ["backgroundColor", this.loc.PropertyMain.BackColor, this.PropertyColorControl("shapeFormBackgroundColor", null, this.options.propertyControlWidth), "6px 12px 6px 12px"]
    ];

    var proprtiesTable = this.CreateHTMLTable();
    propertiesContainer.appendChild(proprtiesTable);

    for (var i = 0; i < properties.length; i++) {
        var propertyName = properties[i][0];
        shapeForm.controls[propertyName + "Row"] = proprtiesTable.addRow();

        if (properties[i][0].indexOf("separator") == 0) {
            var sep = this.FormSeparator();
            sep.style.margin = "6px 0 6px 0";
            shapeForm.controls[control.propertyName] = sep;
            proprtiesTable.addCellInLastRow(sep).setAttribute("colspan", 2);
            continue;
        }

        var textCell = proprtiesTable.addTextCellInLastRow(properties[i][1]);
        textCell.className = "stiDesignerCaptionControlsBigIntervals";
        textCell.style.whiteSpace = "normal";
        textCell.style.minWidth = "130px";

        if (properties[i][0] == "font") {
            textCell.style.verticalAlign = "top";
            textCell.style.paddingTop = "10px";
        }

        var control = properties[i][2];
        control.propertyName = propertyName;
        control.style.margin = properties[i][3];
        proprtiesTable.addCellInLastRow(control);
        shapeForm.controls[propertyName] = control;

        jsObject.AddMainMethodsToPropertyControl(control);

        control.action = function () {
            shapeForm.applyShapeProperty({ name: this.propertyName, value: this.propertyName == "shapeText" ? StiBase64.encode(this.value) : this.getValue() });
        }
    }

    propertiesContainer.update = function () {
        var shapeProps = shapeForm.shape.properties;
        for (var i = 0; i < properties.length; i++) {
            if (properties[i][0].indexOf("separator") == 0) {
                continue;
            }
            var control = properties[i][2];
            if (shapeProps[control.propertyName] != null) {
                var value = shapeProps[control.propertyName];
                control.setValue(control.propertyName == "shapeText" ? StiBase64.decode(value) : value);
            }
        }
    }

    shapeForm.onshow = function () {
        var selectedObject = jsObject.options.selectedObject;

        if (!selectedObject || selectedObject.typeComponent != "StiShape") {
            this.changeVisibleState(false);
        }

        this.currentComponent = selectedObject;
        this.oldSvgContent = selectedObject.properties.svgContent;

        this.updateControls();
    }

    shapeForm.action = function () {
        this.changeVisibleState(false);
        jsObject.options.updateLastStyleProperties = true;
    }

    shapeForm.cancelAction = function () {
        jsObject.SendCommandCanceledEditComponent(shapeForm.shape.name);
        if (this.oldSvgContent) {
            this.currentComponent.properties.svgContent = this.oldSvgContent;
            this.currentComponent.repaint();
        }
    }

    shapeForm.applyShapeProperty = function (property) {
        jsObject.SendCommandToDesignerServer("ApplyShapeProperty", { componentName: shapeForm.shape.name, property: property }, function (answer) {
            shapeForm.shape = answer.shape;
            shapeForm.updateControls();
            shapeForm.updateSvgContent(answer.shape.svgContent);
        });
    }

    shapeForm.updateSvgContent = function (svgContent) {
        this.currentComponent.properties.svgContent = svgContent;
        this.currentComponent.repaint();
    }

    shapeForm.updateControls = function () {
        propertiesContainer.update();
    }

    return shapeForm;
}