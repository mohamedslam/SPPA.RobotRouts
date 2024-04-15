
StiMobileDesigner.prototype.PropertyTopNControl = function (name, width) {
    var control = this.PropertyTextBoxWithEditButton(name, width, true);
    control.key = null;
    var jsObject = this;

    control.button.action = function () {
        this.jsObject.InitializeTopNForm(false, function (form) {
            form.show(control);
        });
    }

    control.setKey = function (key) {
        var notLocalizeValues = this.ownerIsProperty && jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.localizePropertyGrid;
        this.key = key;
        this.textBox.value = "[" + ((key == null || key.mode == "None")
            ? (notLocalizeValues ? "NotAssigned" : this.jsObject.loc.Report.NotAssigned)
            : (notLocalizeValues ? "TopN" : this.jsObject.loc.PropertyMain.TopN)) + "]";
    }

    control.action = function () { }

    return control;
}