
StiMobileDesigner.prototype.ColorsCollectionControl = function (name, toolTip, width, height) {
    var colorsControl = this.TextBoxWithEditButton(name ? name + "TextBox" : null, width, height, true);
    colorsControl.key = [];
    if (name) this.options.controls[name] = colorsControl;
    if (toolTip) colorsControl.setAttribute("title", toolTip);

    colorsControl.button.action = function () {
        this.jsObject.InitializeColorsCollectionForm(function (colorsCollectionForm) {

            colorsCollectionForm.action = function () {
                colorsCollectionForm.changeVisibleState(false);
                var colors = [];
                for (var i = 0; i < colorsCollectionForm.controls.colorsContainer.childNodes.length; i++) {
                    colors.push(colorsCollectionForm.controls.colorsContainer.childNodes[i].controls.colorControl.key);
                }
                colorsControl.setKey(colors);
                colorsControl.action();
            }

            colorsCollectionForm.show(colorsControl.key);
        });
    }

    colorsControl.setKey = function (key) {
        this.key = key;
        this.textBox.value = "[" + (!key || key.length == 0 ? this.jsObject.loc.Report.No : this.jsObject.loc.PropertyCategory.ColorsCategory) + "]";
    }

    colorsControl.action = function () { }

    return colorsControl;
}