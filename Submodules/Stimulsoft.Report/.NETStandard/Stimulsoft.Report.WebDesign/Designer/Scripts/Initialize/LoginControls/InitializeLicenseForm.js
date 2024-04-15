
StiMobileDesigner.prototype.InitializeLicenseForm = function (updateForm) {
    var form = this.BaseForm(null, "Stimulsoft Terms of Service", 3, null, true, true);

    var progress = $("<div style='width: 76px; height: 76px; overflow: hidden; margin-left: 312px; margin-top: 162px;'></div>")[0];
    var progressImg = document.createElement("div");
    progressImg.className = "mobile_designer_loader";
    progress.appendChild(progressImg);

    form.container.appendChild(progress);

    form.container.style.width = "700px";
    form.container.style.maxHeight = "400px";
    form.container.style.height = "400px";
    form.container.style.textAlign = "justify";
    form.container.style.overflow = "auto";
    form.container.style.padding = "10px";
    form.header.style.fontSize = "15px";

    form.container.parentElement.insertBefore(this.FormSeparator(), form.container);
    form.buttonSave.caption.innerHTML = this.loc.Update.ButtonIAgree;
    form.buttonSave.setEnabled(false);
    form.buttonSave.action = function () {
        form.changeVisibleState(false);
        if (updateForm) {
            updateForm.update();
        }
    }

    this.SendCloudCommand("SoftwareGetLicense", { Type: "Developer" }, function (data) {
        if (data.ResultLicense) {
            form.container.innerHTML = StiBase64.decode(data.ResultLicense);
            form.buttonSave.setEnabled(true);

            if (form.container.firstChild) {
                form.container.firstChild.className = "stiDesignerLicenseFormText";
            }
        }
    });

    form.changeVisibleState(true);
    return form;

}