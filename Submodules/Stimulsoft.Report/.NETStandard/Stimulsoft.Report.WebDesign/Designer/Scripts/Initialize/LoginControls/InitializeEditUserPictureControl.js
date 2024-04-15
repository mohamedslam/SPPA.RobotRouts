
StiMobileDesigner.prototype.EditUserPictureControl = function () {
    var control = document.createElement("table");
    control.jsObject = this;
    control.imgSource = $("<div style='width:100px;height:100px;border:dashed 1px #ababab;background-color:#f6f6f6' class='stiTextStyle'>")[0];
    var tr = control.appendChild(document.createElement("tr"));
    var td = tr.appendChild(document.createElement("td"));
    td.appendChild(control.imgSource);
    control.noImage = $("<div style='text-align:center;padding-top:40px'>100 x 100</div>")[0];
    control.imgSource.appendChild(control.noImage);
    control.image = $("<img style='max-width: 100px; max-height:100px'/>")[0];
    td = tr.appendChild($("<td style='vertical-align:top'></td>")[0]);
    var openButton = this.SmallButton(null, null, null, "Open.png", this.loc.MainMenu.menuFileOpen.replace("&", "").replace("...", ""), null, "stiDesignerSmallButtonWithBorder");
    td.appendChild(openButton);

    var deleteButton = this.SmallButton(null, null, null, "Remove.png", this.loc.MainMenu.menuEditDelete.replace("&", ""), null, "stiDesignerSmallButtonWithBorder");
    td.appendChild(deleteButton);
    deleteButton.style.marginTop = "2px";
    deleteButton.setEnabled(false);
    var this_ = this;

    deleteButton.action = function () {
        control.setImage("");
    }

    openButton.action = function () {
        var jsObject = this.jsObject;

        if (jsObject.options.canOpenFiles) {
            var openDialog = jsObject.InitializeOpenDialog("userPictureImageDialog", function (evt) {
                var files = evt.target.files;

                if (files[0].size > jsObject.options.maxUploadFileSize && jsObject.option.cloudMode) {
                    var message = jsObject.loc.File.MessageFailedAddFollowingFiles.replace("{0}", jsObject.convertToMB(jsObject.options.maxUploadFileSize, true));
                    var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                    errorMessageForm.show(message);
                    return;
                }

                for (var i = 0; i < files.length; i++) {
                    var f = files[i];
                    var reader = new FileReader();

                    reader.onload = (function (theFile) {
                        return function (e) {
                            jsObject.ResetOpenDialogs();
                            control.setImage(e.target.result.substring(e.target.result.indexOf("base64,") + 7));
                        };
                    })(f);

                    reader.readAsDataURL(f);
                }
            }, ".bmp,.gif,.jpeg,.jpg,.png,.tiff,.ico,.emf,.wmf");
            openDialog.action();
        }
    }

    control.setImage = function (img) {
        if (control.noImage.parentElement != null) {
            control.noImage.parentElement.removeChild(control.noImage);
        }
        if (control.image.parentElement != null) {
            control.image.parentElement.removeChild(control.image);
        }
        control.img = img;
        if (!img || img == "") {
            control.imgSource.style.borderStyle = "dashed";
            control.imgSource.appendChild(control.noImage);
            deleteButton.setEnabled(false);
        } else {
            control.imgSource.style.borderStyle = "none";
            control.image.src = "data:image/png;base64, " + img;
            control.imgSource.appendChild(control.image);
            deleteButton.setEnabled(true);
        }
        control.action();
    }

    control.action = function () { };

    return control;
}
