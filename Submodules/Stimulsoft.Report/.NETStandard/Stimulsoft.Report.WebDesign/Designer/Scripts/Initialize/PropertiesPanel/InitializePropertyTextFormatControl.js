
StiMobileDesigner.prototype.PropertyTextFormatControl = function (name, width) {
    var textFormatControl = this.PropertyTextBoxWithEditButton(name, width, true);
    textFormatControl.key = null;
    var jsObject = this;

    textFormatControl.button.action = function () {
        jsObject.InitializeTextFormatForm(function (textFormatForm) {

            textFormatForm.action = function () {
                if (textFormatForm.controls.formatsContainer.selectedItem) {
                    var resultTextFormat = textFormatForm.controls.formatsContainer.selectedItem.itemObject;
                    textFormatControl.setKey(resultTextFormat);
                }
                textFormatForm.changeVisibleState(false);
                textFormatControl.action();
            }

            textFormatForm.show(textFormatControl.key);
        });
    }

    textFormatControl.setKey = function (key) {
        var notLocalizeValues = this.ownerIsProperty && jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.localizePropertyGrid;
        this.key = key;
        this.textBox.value = notLocalizeValues ? key.type : jsObject.GetTextFormatLocalizedName(key.type);
    }

    textFormatControl.action = function () { }

    return textFormatControl;
}