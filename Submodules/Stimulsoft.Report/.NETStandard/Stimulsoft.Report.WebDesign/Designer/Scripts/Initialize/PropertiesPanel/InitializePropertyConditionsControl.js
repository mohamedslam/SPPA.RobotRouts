
StiMobileDesigner.prototype.PropertyConditionsControl = function (name, width) {
    var jsObject = this;
    var conditionsControl = this.PropertyTextBoxWithEditButton(name, width, true);
    conditionsControl.key = null;

    conditionsControl.button.action = function () {
        jsObject.InitializeConditionsForm(function (conditionsForm) {

            conditionsForm.action = function () {
                conditionsForm.changeVisibleState(false);
                conditionsForm.controls.itemsContainer.onPreChangeSelection();
                var conditions = [];
                for (var i = 0; i < conditionsForm.controls.itemsContainer.childNodes.length; i++) {
                    conditions.push(conditionsForm.controls.itemsContainer.childNodes[i].itemObject);
                }
                conditionsControl.setKey(conditions.length > 0 ? StiBase64.encode(JSON.stringify(conditions)) : "");
                conditionsControl.action();
            }

            conditionsForm.show(conditionsControl.key);
        });
    }

    conditionsControl.setKey = function (key) {
        this.key = key;
        this.textBox.value = "[" + (key == "" ? jsObject.loc.FormConditions.NoConditions : jsObject.loc.PropertyMain.Conditions) + "]";
    }

    conditionsControl.action = function () { }

    return conditionsControl;
}

StiMobileDesigner.prototype.PropertyChartConditionsControl = function (name, width) {
    var control = this.PropertyTextBoxWithEditButton(name, width, true);
    control.key = null;

    control.button.action = function () {
        this.jsObject.InitializeChartConditionsForm(function (form) {
            form.show();
        });
    }

    control.setKey = function (key) {
        this.key = key;
        this.textBox.value = "[" + ((key == null || key.length == 0) ? this.jsObject.loc.Report.NotAssigned : this.jsObject.loc.PropertyMain.Conditions) + "]";
    }

    control.action = function () { }

    return control;
}

StiMobileDesigner.prototype.PropertyTableConditionsControl = function (name, width) {
    var control = this.PropertyChartConditionsControl(name, width, true);

    control.button.action = function () {
        this.jsObject.InitializeTableConditionsForm(function (form) {
            form.show();
        });
    }

    return control;
}

StiMobileDesigner.prototype.PropertyPivotTableConditionsControl = function (name, width) {
    var control = this.PropertyChartConditionsControl(name, width, true);

    control.button.action = function () {
        this.jsObject.InitializePivotTableConditionsForm(function (form) {
            form.show();
        });
    }

    return control;
}

StiMobileDesigner.prototype.PropertyIndicatorConditionsControl = function (name, width) {
    var control = this.PropertyChartConditionsControl(name, width, true);

    control.button.action = function () {
        this.jsObject.InitializeIndicatorConditionsForm(function (form) {
            form.show();
        });
    }

    return control;
}

StiMobileDesigner.prototype.PropertyProgressConditionsControl = function (name, width) {
    var control = this.PropertyChartConditionsControl(name, width, true);

    control.button.action = function () {
        this.jsObject.InitializeProgressConditionsForm(function (form) {
            form.show();
        });
    }

    return control;
}