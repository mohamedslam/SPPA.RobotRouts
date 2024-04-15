
StiMobileDesigner.prototype.InitializeSignatureImageForm_ = function () {
    var form = this.BaseForm("signatureImage", this.loc.Components.StiImage, 3);

    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "12px";
    form.container.appendChild(toolBar);

    var controls = [
        ["horAlignLeft", this.StandartSmallButton("signImageAlignLeft", "SignatureImageHorAlign", null, "AlignLeft.png", this.loc.Toolbars.AlignLeft), "0"],
        ["horAlignCenter", this.StandartSmallButton("signImageAlignCenter", "SignatureImageHorAlign", null, "AlignCenter.png", this.loc.Toolbars.AlignCenter), "4px"],
        ["horAlignRight", this.StandartSmallButton("signImageAlignRight", "SignatureImageHorAlign", null, "AlignRight.png", this.loc.Toolbars.AlignRight), "4px"],
        ["vertAlignTop", this.StandartSmallButton("signImageAlignTop", "SignatureImageVertAlign", null, "AlignTop.png", this.loc.Toolbars.AlignTop), "12px"],
        ["vertAlignMiddle", this.StandartSmallButton("signImageAlignMiddle", "SignatureImageVertAlign", null, "AlignMiddle.png", this.loc.Toolbars.AlignMiddle), "4px"],
        ["vertAlignBottom", this.StandartSmallButton("signImageAlignBottom", "SignatureImageVertAlign", null, "AlignBottom.png", this.loc.Toolbars.AlignBottom), "4px"],
        ["aspectRatio", this.CheckBox(null, this.loc.PropertyMain.AspectRatio), "12px"],
        ["stretch", this.CheckBox(null, this.loc.PropertyMain.Stretch), "12px"]
    ]

    for (var i = 0; i < controls.length; i++) {
        var controlName = controls[i][0];
        var control = controls[i][1];
        form[controlName] = control;
        toolBar.addCell(control);
        control.style.marginLeft = controls[i][2];

        control.action = function () {
            if (this["setSelected"]) this.setSelected(true);
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
