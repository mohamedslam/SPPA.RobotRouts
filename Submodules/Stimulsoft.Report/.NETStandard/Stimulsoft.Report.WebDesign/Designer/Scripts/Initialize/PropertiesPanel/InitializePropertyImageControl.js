
StiMobileDesigner.prototype.PropertyImageControl = function (name, width) {
    var imageControl = this.PropertyTextBoxWithEditButton(name ? name + "TextBox" : null, width, true);
    imageControl.key = null;
    if (name) this.options.controls[name] = imageControl;

    imageControl.button.action = function () {
        this.jsObject.InitializeEasyImageForm(function (form) {
            form.changeVisibleState(true);
            form.imageSrcContainer.setImage(imageControl.key);

            form.action = function () {
                imageControl.setKey(form.imageSrcContainer.src);
                form.changeVisibleState(false);
                imageControl.action();
            }
        });
    }

    imageControl.setKey = function (key) {
        this.key = key;
        this.textBox.value = key ? "[" + this.jsObject.loc.Components.StiImage + "]" : "";
    }

    imageControl.action = function () { };

    return imageControl;
}