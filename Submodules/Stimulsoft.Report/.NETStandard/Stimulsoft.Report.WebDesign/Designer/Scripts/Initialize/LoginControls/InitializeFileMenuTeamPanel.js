
StiMobileDesigner.prototype.InitializeFileMenuTeamPanel = function (parentContainer) {
    var jsObject = this;
    var panel = document.createElement("div");
    panel.className = "stiDesignerAccountChildPanel";
    panel.controls = {};
    this.options.fileMenuTeamPanel = panel;

    jsObject.AddProgressToControl(panel);

    //Users 
    var usersPanel = document.createElement("div");
    panel.appendChild(usersPanel);

    var usersHeader = this.AccountPanelHeader(this.loc.Cloud.MyTeam);
    usersPanel.appendChild(usersHeader);

    var toolBarUsers = this.CreateHTMLTable();
    toolBarUsers.style.margin = "12px";
    usersPanel.appendChild(toolBarUsers);

    var newUserButton = this.TeamUsersToolButton(this.loc.MainMenu.menuFileNew.replace("&", ""), "Teams.UserAdd.png");
    toolBarUsers.addCell(newUserButton).style.padding = "0 4px 0 4px";
    var editUserButton = this.TeamUsersToolButton(this.loc.MainMenu.menuEditEdit, "EditButton.png");
    toolBarUsers.addCell(editUserButton).style.padding = "0 4px 0 4px";;
    var removeUserButton = this.TeamUsersToolButton(this.loc.Buttons.Delete, "Remove.png");
    toolBarUsers.addCell(removeUserButton).style.padding = "0 4px 0 4px";;
    var inviteUserButton = this.TeamUsersToolButton(this.loc.Cloud.Invite, "Teams.Invite.png");
    toolBarUsers.addCell(inviteUserButton).style.padding = "0 4px 0 4px";

    var usersTable = jsObject.AccountTeamPanelUsersTable();
    usersPanel.appendChild(usersTable);

    usersTable.buildUsers = function () {
        panel.progress.show();
        usersPanel.style.display = "";
        usersTable.clear();
        editUserButton.setEnabled(false);
        removeUserButton.setEnabled(false);

        jsObject.SendCloudCommand("UserFetchAll", {}, function (data) {
            panel.progress.hide();
            if (data.ResultUsers) {
                for (var i = 0; i < data.ResultUsers.length; i++) {
                    usersTable.addUserRow(data.ResultUsers[i]);
                }
            }
        }, function (data, msg) {
            panel.showError(data, msg);
        });
    }

    usersTable.onSelected = function () {
        editUserButton.setEnabled(this.selectedItem != null);
        removeUserButton.setEnabled(this.selectedItem != null);
    }

    removeUserButton.action = function () {
        if (usersTable.selectedItem) {
            panel.progress.show();
            var user = usersTable.selectedItem.itemObject;

            jsObject.SendCloudCommand("UserDelete", { UserName: user.UserName, UserKey: user.Key, AllowMoveToRecycleBin: true }, function (data) {
                usersTable.buildUsers();
            }, function (data, msg) {
                panel.showError(data, msg);
            });
        }
    }

    newUserButton.action = function () {
        var editUserForm = jsObject.options.forms.editUserForm || jsObject.InitializeEditUserForm();
        editUserForm.show(null, usersTable, panel);
    }

    editUserButton.action = function () {
        if (usersTable.selectedItem) {
            var editUserForm = jsObject.options.forms.editUserForm || jsObject.InitializeEditUserForm();
            editUserForm.show(usersTable.selectedItem.itemObject, usersTable, panel);
        }
    }

    //Invitations
    var invitationsPanel = document.createElement("div");
    panel.appendChild(invitationsPanel);

    var inviteHeader = this.AccountPanelHeader(this.loc.Cloud.Invitations);
    invitationsPanel.appendChild(inviteHeader);

    var toolBarInvite = this.CreateHTMLTable();
    toolBarInvite.style.margin = "12px";
    invitationsPanel.appendChild(toolBarInvite);

    var removeInviteButton = this.TeamUsersToolButton(this.loc.Buttons.Delete, "Remove.png");
    toolBarInvite.addCell(removeInviteButton).style.padding = "0 4px 0 4px";

    var inviteTable = jsObject.AccountTeamPanelInviteTable();
    invitationsPanel.appendChild(inviteTable);

    inviteTable.buildInvitations = function () {
        panel.progress.show();
        removeInviteButton.setEnabled(false);
        inviteTable.clear();
        invitationsPanel.style.display = "none";

        jsObject.SendCloudCommand("UserFetchAllInvite", {}, function (data) {
            panel.progress.hide();
            if (data.ResultInvitations && data.ResultInvitations.length > 0) {
                invitationsPanel.style.display = "";
                for (var i = 0; i < data.ResultInvitations.length; i++) {
                    inviteTable.addUserRow(data.ResultInvitations[i]);
                }
            }
        }, function (data, msg) {
            panel.showError(data, msg);
        });
    }

    inviteTable.onSelected = function () {
        removeInviteButton.setEnabled(this.selectedItem != null);
    }

    removeInviteButton.action = function () {
        if (inviteTable.selectedItem) {
            panel.progress.show();
            var invite = inviteTable.selectedItem.itemObject;

            jsObject.SendCloudCommand("UserDeleteInvite", { InviteKey: invite.Key }, function (data) {
                inviteTable.buildInvitations();
            }, function (data, msg) {
                panel.showError(data, msg);
            });
        }
    }

    inviteUserButton.action = function () {
        var inviteForm = jsObject.options.forms.inviteForm || jsObject.InitializeInviteForm();
        inviteForm.show(inviteTable, panel);
    }

    panel.show = function () {
        this.style.display = "";
        this.visible = true;
        this.style.left = jsObject.FindPosX(parentContainer, "stiDesignerMainPanel") + "px";
        this.style.top = jsObject.FindPosY(parentContainer, "stiDesignerMainPanel") + "px";

        usersPanel.style.display = "none";
        invitationsPanel.style.display = "none";

        if ((jsObject.options.cloudMode && !jsObject.options.cloudParameters.sessionKey) || (jsObject.options.standaloneJsMode && !jsObject.options.SessionKey)) {
            jsObject.options.forms.authForm.show();
            return;
        }

        usersTable.buildUsers();
        inviteTable.buildInvitations();
    }

    panel.hide = function () {
        this.style.display = "none";
        this.visible = false;
    }

    panel.showError = function (data, msg) {
        panel.progress.hide();
        if (msg || data) {
            var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
            errorMessageForm.show(msg || jsObject.formatResultMsg(data));
        }
    }

    return panel;
}

StiMobileDesigner.prototype.AccountTeamPanelUsersTable = function () {
    var columns = [
        { name: "UserName", label: this.loc.Cloud.TextUserName },
        { name: "DeveloperRole", label: this.loc.Cloud.TextRole },
        { name: "FirstName", label: this.loc.Cloud.TextFirstName },
        { name: "LastName", label: this.loc.Cloud.TextLastName },
        { name: "Created", label: this.loc.Cloud.LabelCreated.replace(":", "") },
        { name: "Enabled", label: this.loc.PropertyMain.Enabled }
    ];

    return this.AccountTeamPanelTable(columns);
}

StiMobileDesigner.prototype.AccountTeamPanelInviteTable = function () {
    var columns = [
        { name: "UserName", label: this.loc.Cloud.TextUserName },
        { name: "Created", label: this.loc.Cloud.LabelCreated.replace(":", "") }
    ];

    return this.AccountTeamPanelTable(columns);
}

StiMobileDesigner.prototype.AccountTeamPanelTable = function (columns) {
    var jsObject = this;
    var table = this.CreateHTMLTable();
    table.style.width = "100%";

    for (var i = 0; i < columns.length; i++) {
        var headerCell = table.addTextCell(columns[i].label);
        headerCell.className = "stiDesignerTeamUsersCell";
        headerCell.style.fontWeight = "bold";
    }

    table.addUserRow = function (itemObject) {
        var userRow = table.addRow();
        userRow.itemObject = itemObject;

        for (var i = 0; i < columns.length; i++) {
            var value = itemObject[columns[i].name];

            if (columns[i].name == "Enabled") {
                var checkBox = jsObject.CheckBox();
                checkBox.setChecked(value);
                checkBox.onclick = function () { };
                checkBox.onmouseenter = function () { };
                checkBox.onmouseleave = function () { };
                var cell = table.addCellInLastRow(checkBox);
                cell.className = "stiDesignerTeamUsersCell";
                cell.style.textAlign = "center";
            }
            else {
                if (columns[i].name == "Created") {
                    value = jsObject.JSONDateFormatToDate(value).toLocaleDateString();
                }
                if (columns[i].name == "DeveloperRole") {
                    value = jsObject.GetDeveloperRoleName(value);
                }
                var cell = table.addTextCellInLastRow(value);
                cell.className = "stiDesignerTeamUsersCell";
            }
        }

        userRow.onmouseover = function () {
            if (!jsObject.options.isTouchDevice) this.onmouseenter();
        }

        userRow.onmouseenter = function () {
            if (jsObject.options.isTouchClick) return;
            this.className = "stiDesignerTeamUsersCell stiDesignerTeamUsersCellOver";
            this.isOver = true;
        }

        userRow.onmouseleave = function () {
            this.className = "stiDesignerTeamUsersCell " + (this.isSelected ? "stiDesignerTeamUsersCellSelect" : "");
            this.isOver = false;
        }

        userRow.selected = function () {
            if (table.selectedItem) {
                table.selectedItem.isSelected = false;
                table.selectedItem.className = "stiDesignerTeamUsersCell " + (table.selectedItem.isOver ? "stiDesignerTeamUsersCellOver" : "");
            }
            this.isSelected = true;
            this.className = "stiDesignerTeamUsersCell " + (this.isOver ? "stiDesignerTeamUsersCellOver" : "stiDesignerTeamUsersCellSelect");
            table.selectedItem = this;
        }

        userRow.onclick = function () {
            this.selected();
            table.onSelected();
        }

        return userRow;
    }

    table.clear = function () {
        if (table.tr.length > 0) {
            for (var i = 1; i < table.tr.length; i++) {
                table.tr[i].parentElement.removeChild(table.tr[i]);
            }
            table.tr.splice(1, table.tr.length - 1);
        }
        table.selectedItem = null;
    }

    table.onSelected = function () { }

    return table;
}

StiMobileDesigner.prototype.TeamUsersToolButton = function (caption, imageName) {
    var button = this.StandartSmallButton(null, null, caption, imageName);
    if (button.caption) button.caption.style.padding = "0 12px 0 0";
    if (button.imageCell) button.imageCell.style.padding = "0 0 0 12px";
    button.style.height = "28px";

    return button;
}

StiMobileDesigner.prototype.GetDeveloperRoleName = function (developerRole) {
    if (developerRole == "OwnerDeveloper")
        return this.loc.Cloud.TextOwner + " " + this.loc.Desktop.Developer;

    if (developerRole == "Owner")
        return this.loc.Cloud.TextOwner;

    if (developerRole == "Developer")
        return this.loc.Desktop.Developer;

    return "";
}

StiMobileDesigner.prototype.AccountPanelHeader = function (text) {
    var header = document.createElement("div");
    header.className = "stiDesignerAccountPanelHeader";
    header.innerHTML = text;

    return header;
}