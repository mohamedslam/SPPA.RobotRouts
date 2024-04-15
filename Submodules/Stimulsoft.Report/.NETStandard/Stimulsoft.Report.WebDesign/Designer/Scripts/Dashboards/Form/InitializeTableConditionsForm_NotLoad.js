
StiMobileDesigner.prototype.InitializeTableConditionsForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("tableConditionsForm", this.loc.PropertyMain.Conditions, 1, this.HelpLinks["dashboardConditions"]);
    form.conditionsControl = this.TableConditionsControl(this.options.isTouchDevice ? 780 : 720, 400);
    form.container.appendChild(form.conditionsControl);

    form.show = function () {
        var selectedObject = jsObject.options.selectedObject || (jsObject.options.selectedObjects && jsObject.options.selectedObjects.length > 0 ? jsObject.options.selectedObjects[0] : null);
        if (selectedObject && selectedObject.properties.tableConditions) {
            form.conditionsControl.valueMeters = selectedObject.properties.valueMeters;
            form.conditionsControl.fill(jsObject.CopyObject(selectedObject.properties.tableConditions));
            form.changeVisibleState(true);
        }
    }

    form.action = function () {
        jsObject.ApplyPropertyValue(["tableConditions"], [form.conditionsControl.getValue()]);
        form.changeVisibleState(false);
    }

    return form;
}