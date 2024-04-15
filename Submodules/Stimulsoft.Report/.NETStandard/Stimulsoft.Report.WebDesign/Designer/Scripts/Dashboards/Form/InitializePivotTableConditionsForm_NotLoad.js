
StiMobileDesigner.prototype.InitializePivotTableConditionsForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("pivotTableConditionsForm", this.loc.PropertyMain.Conditions, 1, this.HelpLinks["dashboardConditions"]);
    form.conditionsControl = this.PivotTableConditionsControl(this.options.isTouchDevice ? 690 : 630, 400);
    form.container.appendChild(form.conditionsControl);

    form.show = function () {
        var selectedObject = jsObject.options.selectedObject || (jsObject.options.selectedObjects && jsObject.options.selectedObjects.length > 0 ? jsObject.options.selectedObjects[0] : null);
        if (selectedObject && selectedObject.properties.pivotTableConditions) {
            form.conditionsControl.valueMeters = selectedObject.properties.valueMeters;
            form.conditionsControl.fill(jsObject.CopyObject(selectedObject.properties.pivotTableConditions));
            form.changeVisibleState(true);
        }
    }

    form.action = function () {
        jsObject.ApplyPropertyValue(["pivotTableConditions"], [form.conditionsControl.getValue()]);
        form.changeVisibleState(false);
    }

    return form;
}