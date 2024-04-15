
StiMobileDesigner.prototype.FileEditorControl = function (name, width, height, filesMask) {
    var jsObject = this;
    var control = this.TextAreaWithEditButton(name, width, height);
    StiMobileDesigner.setImageSource(control.button.image, this.options, "Open.png");

    control.button.action = function () {
        if (jsObject.options.canOpenFiles) {
            var openDialog = jsObject.InitializeOpenDialog("fileEditorControlDialog", function (evt) {
                var files = evt.target.files;

                for (var i = 0; i < files.length; i++) {
                    var f = files[i];

                    var reader = new FileReader();

                    reader.onload = (function (theFile) {
                        return function (e) {
                            jsObject.ResetOpenDialogs();
                            control.textArea.value = e.target.result;
                            control.action();
                            jsObject.ReturnFocusToDesigner();
                        };
                    })(f);

                    reader.readAsText(f);
                }
            }, filesMask);
            openDialog.action();
        }
    }

    control.action = function () { }

    return control;
}