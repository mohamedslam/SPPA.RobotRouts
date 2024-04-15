
StiMobileDesigner.prototype.PropertyTrendLinesControl = function (name, width) {
    var control = this.PropertyTextBoxWithEditButton(name, width, true);
    control.key = null;
    var jsObject = this;

    control.button.action = function () {
        this.jsObject.InitializeTrendLinesForm(function (form) {
            form.show(control);
        });
    }

    control.setKey = function (key) {
        var notLocalizeValues = this.ownerIsProperty && jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.localizePropertyGrid;
        this.key = key;
        this.textBox.value = "[" + ((key == null || key.length == 0)
            ? (notLocalizeValues ? "NotAssigned" : jsObject.loc.Report.NotAssigned)
            : (notLocalizeValues ? "TrendLines" : jsObject.loc.PropertyMain.TrendLines)) + "]";
    }

    control.action = function () { }

    return control;
}