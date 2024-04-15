
StiMobileDesigner.prototype.PropertyConstantLinesControl = function (name, width) {
    var jsObject = this;
    var control = this.PropertyTextBoxWithEditButton(name, width, true);
    control.key = null;

    control.button.action = function () {
        var chartComponent = jsObject.options.selectedObject;
        if (chartComponent) {
            jsObject.InitializeEditChartConstantLinesForm(function (form) {
                form.currentChartComponent = chartComponent;
                form.oldSvgContent = chartComponent.properties.svgContent;
                if (chartComponent.typeComponent == "StiChart")
                    jsObject.SendCommandStartEditChartComponent(chartComponent.properties.name, form.name);
                else if (chartComponent.typeComponent == "StiChartElement")
                    jsObject.SendCommandStartEditChartElement(chartComponent.properties.name, form.name);
            });
        }
    }

    control.setKey = function (key) {
        var notLocalizeValues = this.ownerIsProperty && jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.localizePropertyGrid;
        this.key = key;
        this.textBox.value = "[" + ((key == null || key.length == 0)
            ? (notLocalizeValues ? "NotAssigned" : this.jsObject.loc.Report.NotAssigned)
            : (notLocalizeValues ? "ConstantLines" : this.jsObject.loc.PropertyMain.ConstantLines)) + "]";
    }

    control.action = function () { }

    return control;
}