
StiMobileDesigner.prototype.InitializeEditShapeElementForm_ = function () {
    var form = this.DashboardBaseForm("editShapeElementForm", this.loc.Components.StiShape, 1, this.HelpLinks["shapeElement"]);
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";
    form.hideButtonsPanel();
    var jsObject = this;

    var controlsTable = this.CreateHTMLTable();
    form.container.appendChild(controlsTable);
    form.container.style.padding = "0 0 8px 0";

    var shapeTypeControl = this.ShapeTypeControl("shapeElementFormShapeType", 164);
    shapeTypeControl.style.height = "21px";
    form.addControlRow(controlsTable, this.loc.PropertyMain.Type, "shapeType", shapeTypeControl, "8px 12px 8px 12px");
    shapeTypeControl.action = function () {
        form.setPropertyValue("ShapeType", this.key);
    }

    var sizeControl = this.TextBox("shapeElementFormSize", 160);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Size, "size", sizeControl, "8px 12px 8px 12px");
    sizeControl.action = function () {
        form.setPropertyValue("Size", this.value);
    }

    var strokeControl = this.PropertyColorControl("shapeElementFormStroke", null, 160)
    form.addControlRow(controlsTable, this.loc.PropertyMain.Stroke, "stroke", strokeControl, "8px 12px 8px 12px");
    strokeControl.action = function () {
        form.setPropertyValue("Stroke", this.key);
    }

    var fillControl = this.PropertyBrushControl("shapeElementFormFill", null, 160);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Fill, "fill", fillControl, "8px 12px 8px 12px");
    fillControl.action = function () {
        form.setPropertyValue("Fill", this.key);
    }

    form.onshow = function () {
        form.sendCommandToShapeElement({ command: "GetShapeElementProperties" },
            function (answer) {
                form.updateControls(answer.shapeElement, true);
            }
        );
    }

    form.setValues = function () {
        shapeTypeControl.setKey(this.shapeProperties.shapeType);
        sizeControl.value = this.shapeProperties.size;
        strokeControl.setKey(this.shapeProperties.stroke);
        fillControl.setKey(this.shapeProperties.fill);
    }

    form.updateControls = function (shapeElement, notRepaintElement) {
        form.shapeProperties = shapeElement;
        form.setValues();
        if (!notRepaintElement) {
            form.currentShapeElement.properties.svgContent = shapeElement.svgContent;
            form.currentShapeElement.repaint();
        }
    }

    form.setPropertyValue = function (propertyName, propertyValue) {
        form.sendCommandToShapeElement(
            {
                command: "SetPropertyValue",
                propertyName: propertyName,
                propertyValue: propertyValue
            },
            function (answer) {
                form.updateControls(answer.shapeElement);
                form.correctTopPosition();
            });
    }

    form.sendCommandToShapeElement = function (updateParameters, callbackFunction) {
        form.jsObject.SendCommandToDesignerServer("UpdateShapeElement",
            {
                componentName: form.currentShapeElement.properties.name,
                updateParameters: updateParameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    return form;
}
