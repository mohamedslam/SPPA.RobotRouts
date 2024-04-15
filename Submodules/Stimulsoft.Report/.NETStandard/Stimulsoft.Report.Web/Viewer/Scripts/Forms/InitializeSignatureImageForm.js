
StiJsViewer.prototype.InitializeSignatureImageForm = function () {
    var form = this.BaseForm("signatureImage", this.collections.loc["Image"], 3);
    form.container.style.padding = "1px";

    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "12px";
    form.container.appendChild(toolBar);

    var controls = [
        ["horAlignLeft", this.SmallButton(null, null, "Font.AlignLeft.png", this.collections.loc["AlignLeft"]), "0"],
        ["horAlignCenter", this.SmallButton(null, null, "Font.AlignCenter.png", this.collections.loc["AlignCenter"]), "4px"],
        ["horAlignRight", this.SmallButton(null, null, "Font.AlignRight.png", this.collections.loc["AlignRight"]), "4px"],
        ["vertAlignTop", this.SmallButton(null, null, "Font.AlignTop.png", this.collections.loc["AlignTop"]), "12px"],
        ["vertAlignMiddle", this.SmallButton(null, null, "Font.AlignMiddle.png", this.collections.loc["AlignMiddle"]), "4px"],
        ["vertAlignBottom", this.SmallButton(null, null, "Font.AlignBottom.png", this.collections.loc["AlignBottom"]), "4px"],
        ["aspectRatio", this.CheckBox(null, this.collections.loc["AspectRatio"]), "12px"],
        ["stretch", this.CheckBox(null, this.collections.loc["Stretch"]), "12px"]
    ]

    for (var i = 0; i < controls.length; i++) {
        var controlName = controls[i][0];
        var control = controls[i][1];
        form[controlName] = control;
        toolBar.addCell(control);
        control.style.marginLeft = controls[i][2];
        control.controlName = controlName;

        control.action = function () {
            if (this.controlName.indexOf("horAlign") == 0) {
                form.horAlignLeft.setSelected(false);
                form.horAlignCenter.setSelected(false);
                form.horAlignRight.setSelected(false);
                this.setSelected(true);
            }
            if (this.controlName.indexOf("vertAlign") == 0) {
                form.vertAlignTop.setSelected(false);
                form.vertAlignMiddle.setSelected(false);
                form.vertAlignBottom.setSelected(false);
                this.setSelected(true);
            }
            form.imageSrcContainer.update();
        }
    }

    var imageSrcContainer = form.imageSrcContainer = this.ImageControl(null, 400, 400);
    imageSrcContainer.style.margin = "12px";
    form.container.appendChild(imageSrcContainer);

    imageSrcContainer.action = function () {
        this.update();
    }

    imageSrcContainer.update = function () {
        var imgCont = this.imageContainer;
        imgCont.style.width = imgCont.style.height = form.stretch.isChecked ? "400px" : "auto";

        if (form.stretch.isChecked && form.aspectRatio.isChecked) {
            imgCont.style.height = imgCont.offsetHeight > 0 && imgCont.offsetHeight > imgCont.offsetWidth ? "400px" : "auto";
            imgCont.style.width = imgCont.offsetHeight > 0 && imgCont.offsetHeight > imgCont.offsetWidth ? "auto" : "400px";
        }

        var imageCell = imgCont.parentElement;
        imageCell.style.textAlign = imageSrcContainer.imageContainer.src ? (form.horAlignLeft.isSelected ? "left" : (form.horAlignRight.isSelected ? "right" : "center")) : "center";
        imageCell.style.verticalAlign = imageSrcContainer.imageContainer.src ? (form.vertAlignTop.isSelected ? "top" : (form.vertAlignBottom.isSelected ? "bottom" : "middle")) : "middle";
    }

    form.show = function (imageProps) {
        this.changeVisibleState(true);
        this.horAlignLeft.setSelected(imageProps.horAlignment == "Left");
        this.horAlignCenter.setSelected(imageProps.horAlignment == "Center");
        this.horAlignRight.setSelected(imageProps.horAlignment == "Right");
        this.vertAlignTop.setSelected(imageProps.vertAlignment == "Top");
        this.vertAlignMiddle.setSelected(imageProps.vertAlignment == "Center");
        this.vertAlignBottom.setSelected(imageProps.vertAlignment == "Bottom");
        this.aspectRatio.setChecked(imageProps.aspectRatio);
        this.stretch.setChecked(imageProps.stretch);
        this.imageSrcContainer.setImage(imageProps.image, function () {
            form.imageSrcContainer.update();
        });
    }

    return form;
}