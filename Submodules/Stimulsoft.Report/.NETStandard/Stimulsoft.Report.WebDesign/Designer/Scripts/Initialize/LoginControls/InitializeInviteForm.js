
StiMobileDesigner.prototype.InitializeInviteForm = function () {
    var jsObject = this;
    var form = this.BaseForm("inviteForm", this.loc.Cloud.Invite, 4, null, true);
    form.controls = {};

    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.margin = "5px 0 5px 0";
    form.container.appendChild(controlsTable);

    var controlProps = [
        ["UserName", this.loc.Cloud.TextUserName, this.TextBox(null, 200), "3px 0 3px 0"]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        form.controls[controlProps[i][0] + "Row"] = controlsTable.addRow();
        controlsTable.addTextCellInLastRow(controlProps[i][1]).className = "stiDesignerCaptionControlsBigIntervals";
        var control = controlProps[i][2];
        control.style.margin = controlProps[i][3];
        form.controls[controlProps[i][0]] = control;
        controlsTable.addCellInLastRow(control).className = "stiDesignerControlCellsBigIntervals";
    }

    var footerText = document.createElement("div");
    footerText.className = "stiDesignerAccountInviteFormFooter";
    footerText.innerHTML = this.loc.Cloud.LabelInviteUser;
    form.container.appendChild(footerText);

    form.show = function (inviteTable, fileMenuTeamPanel) {
        this.changeVisibleState(true);
        this.inviteTable = inviteTable;
        this.fileMenuTeamPanel = fileMenuTeamPanel;
        form.controls.UserName.value = "";
        form.controls.UserName.focus();
    }

    form.action = function () {
        var isEditMode = this.user != null;

        if (this.controls.UserName.checkEmpty(jsObject.loc.Notices.AuthUserNameIsNotSpecified, null, null, true) &&
            this.controls.UserName.checkEmail(jsObject.loc.Notices.AuthUserNameShouldLookLikeAnEmailAddress)) {

            form.fileMenuTeamPanel.progress.show();

            jsObject.SendCloudCommand("UserInvite", { UserName: form.controls.UserName.value },
                function (data) {
                    form.inviteTable.buildInvitations();
                },
                function (data, msg) {
                    form.fileMenuTeamPanel.showError(data, msg);
                });

            form.changeVisibleState(false);
        }
    }

    return form;
}