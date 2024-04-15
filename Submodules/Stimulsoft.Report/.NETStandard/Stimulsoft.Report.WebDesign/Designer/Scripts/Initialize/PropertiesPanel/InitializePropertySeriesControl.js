
StiMobileDesigner.prototype.PropertySeriesControl = function (name, width) {
    var jsObject = this;
    var control = this.PropertyTextBoxWithEditButton(name, width, true);
    control.key = null;

    control.button.action = function () {
        var chartComponent = jsObject.options.selectedObject;
        if (chartComponent) {
            jsObject.InitializeEditChartSeriesForm(function (form) {
                form.currentChartComponent = chartComponent;
                form.oldSvgContent = chartComponent.properties.svgContent;
                jsObject.SendCommandStartEditChartComponent(chartComponent.properties.name, form.name);
            });
        }
    }

    control.setKey = function (key) {
        var notLocalizeValues = this.ownerIsProperty && jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.localizePropertyGrid;
        this.key = key;
        this.textBox.value = "[" + ((key == null || key.length == 0)
            ? (notLocalizeValues ? "NotAssigned" : this.jsObject.loc.Report.NotAssigned)
            : (notLocalizeValues ? "Series" : this.jsObject.loc.PropertyMain.Series)) + "]";
    }

    control.action = function () { }

    return control;
}