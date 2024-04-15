
StiMobileDesigner.prototype.DataControl = function (name, width, showNoText) {
    var jsObject = this;
    var dataControl = this.TextBoxWithEditButton(name, width, null, true);

    dataControl.button.action = function () {
        this.key = this.textBox.value;

        jsObject.InitializeDataColumnForm(function (dataColumnForm) {
            dataColumnForm.parentButton = dataControl.button;
            dataColumnForm.changeVisibleState(true);

            dataColumnForm.action = function () {
                this.changeVisibleState(false);
                this.parentButton.textBox.value = showNoText && !this.dataTree.key ? "[" + jsObject.loc.FormFormatEditor.nameNo + "]" : this.dataTree.key;
                dataControl.action();
            }
        });
    }

    dataControl.action = function () { }

    return dataControl;
}