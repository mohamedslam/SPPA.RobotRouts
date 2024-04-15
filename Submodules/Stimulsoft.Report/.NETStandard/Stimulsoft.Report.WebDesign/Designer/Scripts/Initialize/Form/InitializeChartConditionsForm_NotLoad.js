
StiMobileDesigner.prototype.InitializeChartConditionsForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("chartConditionsForm", this.loc.PropertyMain.Conditions, 1, this.HelpLinks["dashboardConditions"]);
    form.conditionsControl = this.ChartConditionsControl(this.options.isTouchDevice ? 730 : 715, 400);
    form.container.appendChild(form.conditionsControl);

    form.show = function () {
        var selectedObject = jsObject.options.selectedObject || (jsObject.options.selectedObjects && jsObject.options.selectedObjects.length > 0 ? jsObject.options.selectedObjects[0] : null);
        if (selectedObject && selectedObject.properties.chartConditions) {
            form.conditionsControl.valueMeters = selectedObject.properties.valueMeters;
            form.conditionsControl.argumentMeters = selectedObject.properties.argumentMeters;
            form.conditionsControl.seriesMeters = selectedObject.properties.seriesMeters;
            form.conditionsControl.fill(jsObject.CopyObject(selectedObject.properties.chartConditions));
            form.changeVisibleState(true);
        }
    }

    form.action = function () {
        jsObject.ApplyPropertyValue(["chartConditions"], [form.conditionsControl.getValue()]);
        form.changeVisibleState(false);
    }

    return form;
}