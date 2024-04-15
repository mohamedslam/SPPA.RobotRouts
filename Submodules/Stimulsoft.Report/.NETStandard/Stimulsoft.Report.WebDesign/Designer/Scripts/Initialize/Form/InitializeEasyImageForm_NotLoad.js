
StiMobileDesigner.prototype.InitializeEasyImageForm_ = function () {
    var form = this.BaseForm("easyImageForm", this.loc.Components.StiImage, 3);

    var imageSrcContainer = this.ImageControl(null, 400, 400);
    imageSrcContainer.style.margin = "12px";
    form.imageSrcContainer = imageSrcContainer;
    form.container.appendChild(imageSrcContainer);

    return form;
}