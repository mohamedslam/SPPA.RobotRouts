
StiMobileDesigner.prototype.InitializeEditUserForm = function () {
    var jsObject = this;
    var form = this.BaseForm("editUserForm", this.loc.Cloud.WindowTitleUserNew, 4, null, true);
    form.controls = {};

    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.margin = "5px 0 5px 0";
    form.container.appendChild(controlsTable);

    var controlProps = [
        ["FirstName", this.loc.Cloud.TextFirstName, this.TextBox(null, 200), "3px 0 3px 0"],
        ["LastName", this.loc.Cloud.TextLastName, this.TextBox(null, 200), "3px 0 3px 0"],
        ["UserName", this.loc.Cloud.TextUserName, this.TextBox(null, 200), "3px 0 3px 0"],
        ["Password", this.loc.Password.StiSavePasswordForm, this.TextBox(null, 150), "3px 0 3px 0"],
        ["CurrentPassword", this.loc.Cloud.LabelCurrentPassword.replace(":", ""), this.TextBox(null, 150), "3px 0 3px 0"],
        ["NewPassword", this.loc.Cloud.LabelNewPassword.replace(":", ""), this.TextBox(null, 150), "3px 0 3px 0"],
        ["DeveloperRole", this.loc.Cloud.TextRole, this.DropDownList(null, 150, null, this.GetDeveloperRoleItems()), "3px 0 3px 0"],
        ["Enabled", "", this.CheckBox(null, this.loc.PropertyMain.Enabled), "6px 0 3px 0"],
        ["Newsletters", "", this.CheckBox(null, this.loc.Cloud.TextNewsLetters), "3px 0 3px 0"]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        form.controls[controlProps[i][0] + "Row"] = controlsTable.addRow();
        controlsTable.addTextCellInLastRow(controlProps[i][1]).className = "stiDesignerCaptionControlsBigIntervals";
        var control = controlProps[i][2];
        control.style.margin = controlProps[i][3];
        form.controls[controlProps[i][0]] = control;
        controlsTable.addCellInLastRow(control).className = "stiDesignerControlCellsBigIntervals";
    }

    form.controls.Password.setAttribute("type", "password");
    form.controls.CurrentPassword.setAttribute("type", "password");
    form.controls.NewPassword.setAttribute("type", "password");

    form.show = function (user, usersTable, fileMenuTeamPanel) {
        this.changeVisibleState(true);
        this.user = user;
        this.usersTable = usersTable;
        this.fileMenuTeamPanel = fileMenuTeamPanel;
        this.caption.innerHTML = user ? jsObject.loc.Cloud.WindowTitleUserEdit : jsObject.loc.Cloud.WindowTitleUserNew;
        form.controls.FirstName.value = user ? user.FirstName : "";
        form.controls.LastName.value = user ? user.LastName : "";
        form.controls.UserName.value = user ? user.UserName : "";
        form.controls.UserName.readOnly = user ? true : false;
        form.controls.Password.value = form.controls.CurrentPassword.value = form.controls.NewPassword.value = "";
        form.controls.PasswordRow.style.display = user ? "none" : "";
        form.controls.CurrentPasswordRow.style.display = form.controls.NewPasswordRow.style.display = user ? "" : "none";
        form.controls.DeveloperRole.setKey(user ? user.DeveloperRole : "Developer");
        form.controls.Enabled.setChecked(user ? user.Enabled : true);
        form.controls.Newsletters.setChecked(user ? user.Newsletters : true);
        form.controls.FirstName.focus();
    }

    form.action = function () {
        var isEditMode = this.user != null;

        if (this.controls.FirstName.checkEmpty(jsObject.loc.Notices.AuthFirstNameIsNotSpecified, null, null, true) &&
            this.controls.LastName.checkEmpty(jsObject.loc.Notices.AuthLastNameIsNotSpecified, null, null, true) &&
            this.controls.UserName.checkEmpty(jsObject.loc.Notices.AuthUserNameIsNotSpecified, null, null, true) &&
            this.controls.UserName.checkEmail(jsObject.loc.Notices.AuthUserNameShouldLookLikeAnEmailAddress)) {
            if (
                (isEditMode && (
                    !this.controls.CurrentPassword.checkEmpty(jsObject.loc.Notices.AuthPasswordIsNotSpecified, null, null, true) ||
                    !this.controls.CurrentPassword.checkLength(6, jsObject.loc.Notices.AuthPasswordIsTooShort) ||
                    !this.controls.NewPassword.checkEmpty(jsObject.loc.Notices.AuthPasswordIsNotSpecified, null, null, true) ||
                    !this.controls.NewPassword.checkLength(6, jsObject.loc.Notices.AuthPasswordIsTooShort)
                )) ||
                (!isEditMode && (
                    !this.controls.Password.checkEmpty(jsObject.loc.Notices.AuthPasswordIsNotSpecified, null, null, true) ||
                    !this.controls.Password.checkLength(6, jsObject.loc.Notices.AuthPasswordIsTooShort)
                ))
            ) { return }

            var params = {};

            if (isEditMode) {
                params.User = jsObject.CopyObject(form.user);
                params.User.FirstName = this.controls.FirstName.value;
                params.User.LastName = this.controls.LastName.value;
                params.User.Password = this.controls.NewPassword.value;
                params.User.DeveloperRole = this.controls.DeveloperRole.key;
                params.User.Enabled = this.controls.Enabled.isChecked;
                params.User.Newsletters = this.controls.Newsletters.isChecked;
                params.CurrentPassword = form.controls.CurrentPassword.value;
            }
            else {
                params.User = {
                    FirstName: this.controls.FirstName.value,
                    LastName: this.controls.LastName.value,
                    UserName: this.controls.UserName.value,
                    Password: this.controls.Password.value,
                    DeveloperRole: this.controls.DeveloperRole.key,
                    Enabled: this.controls.Enabled.isChecked,
                    Newsletters: this.controls.Newsletters.isChecked
                }
            }

            form.fileMenuTeamPanel.progress.show();

            jsObject.SendCloudCommand("UserSave", params,
                function (data) {
                    form.usersTable.buildUsers();
                },
                function (data, msg) {
                    form.fileMenuTeamPanel.showError(data, msg);
                });

            form.changeVisibleState(false);
        }
    }

    return form;
}