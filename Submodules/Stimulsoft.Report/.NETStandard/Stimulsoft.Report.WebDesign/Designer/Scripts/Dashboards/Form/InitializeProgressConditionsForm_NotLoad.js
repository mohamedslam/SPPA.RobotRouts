
StiMobileDesigner.prototype.InitializeProgressConditionsForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("progressConditionsForm", this.loc.PropertyMain.Conditions, 1, this.HelpLinks["dashboardConditions"]);
    form.conditionsControl = this.ProgressConditionsControl(this.options.isTouchDevice ? 685 : 605, 400);
    form.container.appendChild(form.conditionsControl);

    form.show = function () {
        var selectedObject = jsObject.options.selectedObject || (jsObject.options.selectedObjects && jsObject.options.selectedObjects.length > 0 ? jsObject.options.selectedObjects[0] : null);
        if (selectedObject && selectedObject.properties.progressConditions) {
            form.conditionsControl.fill(jsObject.CopyObject(selectedObject.properties.progressConditions));
            form.changeVisibleState(true);
        }
    }

    form.action = function () {
        jsObject.ApplyPropertyValue(["progressConditions"], [form.conditionsControl.getValue()]);
        form.changeVisibleState(false);
    }

    return form;
}