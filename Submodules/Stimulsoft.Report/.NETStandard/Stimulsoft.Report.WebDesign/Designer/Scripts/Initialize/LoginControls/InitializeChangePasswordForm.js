
StiMobileDesigner.prototype.InitializeChangePasswordForm = function () {
    var jsObject = this;
    var form = this.BaseForm("changePasswordForm", this.loc.Navigator.ButtonChangePassword, 3, null, true);
    form.caption.style.padding = "0 10px 0 15px";
    form.container.style.padding = "7px";

    var textBoxes = [
        ["currentPassword", this.loc.Administration.LabelCurrentPassword, 250],
        ["newPassword", this.loc.Administration.LabelNewPassword, 250]
    ]
    for (var i = 0; i < textBoxes.length; i++) {
        var control = this.TextBox(null, textBoxes[i][2]);
        control.setAttribute("type", "password");
        form.addControlRow2(textBoxes[i][1], textBoxes[i][0], control, "4px 4px 4px 30px");
    }

    this.options.changePasswordForm = form;

    form.show = function () {
        form.buttonSave.setEnabled(true);
        this.changeVisibleState(true);
        form.controls.currentPassword.value = "";
        form.controls.newPassword.value = "";
        form.controls.currentPassword.focus();
    }

    form.action = function () {
        var profilePanel = jsObject.options.profilePanel;
        var showProgress = profilePanel != null && profilePanel.visible;

        if (form.controls.currentPassword.checkEmpty(jsObject.loc.Notices.AuthPasswordIsNotSpecified) &&
            form.controls.newPassword.checkEmpty(jsObject.loc.Notices.AuthPasswordIsNotSpecified) &&
            form.controls.newPassword.checkLength(6, jsObject.loc.Notices.AuthPasswordIsTooShort)) {
            form.changeVisibleState(false);
            if (showProgress) profilePanel.progress.show();

            jsObject.SendCloudCommand("UserChangePassword", { CurrentPassword: form.controls.currentPassword.value, NewPassword: form.controls.newPassword.value }, function () {
                if (showProgress) profilePanel.progress.hide();
            }, function (data, msg) {
                if (showProgress) profilePanel.progress.hide();
                jsObject.ShowMessagesTooltip(msg, form.controls.currentPassword);
            });
        }
    }

    return form;
}