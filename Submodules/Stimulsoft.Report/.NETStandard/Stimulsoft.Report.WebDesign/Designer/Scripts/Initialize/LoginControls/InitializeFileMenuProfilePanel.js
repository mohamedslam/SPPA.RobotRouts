
StiMobileDesigner.prototype.InitializeFileMenuProfilePanel = function (parentContainer) {
    var jsObject = this;
    var panel = document.createElement("div");
    this.options.profilePanel = panel;
    panel.className = "stiDesignerAccountChildPanel";
    panel.controls = {};
    panel.data = {};

    jsObject.AddProgressToControl(panel);

    var controlsTable = jsObject.CreateHTMLTable();
    controlsTable.className = "stiDesignerTextContainer";
    panel.appendChild(controlsTable);
    controlsTable.style.margin = "0 12px 0 12px";

    var footerPanel = document.createElement("div");
    footerPanel.className = "stiDesignerAccountFooterPanel";
    panel.appendChild(footerPanel);

    var saveButton = this.FormButton(null, null, this.loc.Buttons.Save, null);
    saveButton.style.display = "inline-block";
    saveButton.style.margin = "12px";
    footerPanel.appendChild(saveButton);
    panel.controls.saveButton = saveButton;

    //Picture
    controlsTable.addTextCellInLastRow(jsObject.loc.Administration.LabelPicture).style.padding = "10px 10px 10px 0px";

    var picControl = jsObject.EditUserPictureControl();
    picControl.style.margin = "7px 0 7px -3px";
    panel.controls.userPicture = picControl;
    controlsTable.addCellInLastRow(picControl);

    picControl.action = function () {
        if (this.img && this.img != "")
            panel.data.Picture = this.img;
        else
            delete panel.data.Picture;
    }

    //Names controls
    var cWidth = 250;
    createPanel(jsObject.loc.Administration.TextUser, "textUser", [
        ["FirstName", jsObject.loc.Administration.LabelFirstName, cWidth],
        ["LastName", jsObject.loc.Administration.LabelLastName, cWidth],
        ["UserName", jsObject.loc.Administration.LabelUserName, cWidth, true]],
        false, this);

    //Password
    controlsTable.addTextCellInNextRow(jsObject.loc.Password.StiSavePasswordForm).style.padding = "7px 70px 7px 0px";
    var passwordButton = jsObject.HiperLinkButton(null, jsObject.loc.MainMenu.menuEditEdit);
    passwordButton.style.fontSize = "12px";
    passwordButton.style.height = "35px";
    panel.controls.passwordButton = passwordButton;
    controlsTable.addCellInLastRow(passwordButton);

    passwordButton.action = function () {
        (jsObject.options.forms.changePasswordForm || jsObject.InitializeChangePasswordForm()).show();
    }

    //Designer Specification
    controlsTable.addTextCellInNextRow(jsObject.loc.Desktop.SkillLevel).style.padding = "7px 70px 7px 0px";
    var specificButton = jsObject.HiperLinkButton(null, "");
    specificButton.style.fontSize = "12px";
    specificButton.style.height = "35px";
    panel.controls.specificButton = specificButton;
    controlsTable.addCellInLastRow(specificButton);

    specificButton.action = function () {
        jsObject.InitializeWhoAreYouForm(function (form) {
            form.show();
        });
    }

    //Google
    controlsTable.addTextCellInNextRow(jsObject.loc.Cloud.ButtonLogInWith.replace("{0}", "Google")).style.padding = "7px 70px 7px 0px";
    var googleAuthTable = jsObject.CreateHTMLTable();
    controlsTable.addCellInLastRow(googleAuthTable);
    var googleEmail = googleAuthTable.addTextCell();
    panel.controls.googleEmail = googleEmail;

    var addButtonGoogle = jsObject.HiperLinkButton(null, jsObject.loc.Buttons.Add);
    addButtonGoogle.style.fontSize = "12px";
    addButtonGoogle.style.height = "35px";
    panel.controls.googleAddButton = addButtonGoogle;
    googleAuthTable.addCell(addButtonGoogle);
    jsObject.AddSmallProgressMarkerToControl(addButtonGoogle, true);

    addButtonGoogle.action = function () {
        jsObject.LoginWithGoogle(this, "AddLoginWithGoogle");
    }

    var removeButtonGoogle = jsObject.HiperLinkButton(null, jsObject.loc.Buttons.Remove);
    removeButtonGoogle.style.fontSize = "12px";
    removeButtonGoogle.style.height = "35px";
    removeButtonGoogle.style.margin = "0 15px 0 15px";
    panel.controls.googleRemoveButton = removeButtonGoogle;
    googleAuthTable.addCell(removeButtonGoogle);
    jsObject.AddSmallProgressMarkerToControl(removeButtonGoogle, true);

    removeButtonGoogle.action = function () {
        this.progressMarker.changeVisibleState(true);
        jsObject.SendCloudCommand("UserRemoveLoginWithGoogle", {}, function (data) {
            if (data.ResultSuccess && panel.visible) {
                panel.updateGoogleEmail(null);
            }
        });
    }

    //GitHub
    controlsTable.addTextCellInNextRow(jsObject.loc.Cloud.ButtonLogInWith.replace("{0}", "GitHub")).style.padding = "7px 70px 7px 0px";
    var gitHubAuthTable = jsObject.CreateHTMLTable();
    controlsTable.addCellInLastRow(gitHubAuthTable);
    var gitHubEmail = gitHubAuthTable.addTextCell();
    panel.controls.gitHubEmail = gitHubEmail;

    var addButtonGitHub = jsObject.HiperLinkButton(null, jsObject.loc.Buttons.Add);
    addButtonGitHub.style.fontSize = "12px";
    addButtonGitHub.style.height = "35px";
    panel.controls.gitHubAddButton = addButtonGitHub;
    gitHubAuthTable.addCell(addButtonGitHub);
    jsObject.AddSmallProgressMarkerToControl(addButtonGitHub, true);

    addButtonGitHub.action = function () {
        jsObject.LoginWithGitHub(this, "AddLoginWithGitHub");
    }

    var removeButtonGitHub = jsObject.HiperLinkButton(null, jsObject.loc.Buttons.Remove);
    removeButtonGitHub.style.fontSize = "12px";
    removeButtonGitHub.style.height = "35px";
    removeButtonGitHub.style.margin = "0 15px 0 15px";
    panel.controls.gitHubRemoveButton = removeButtonGitHub;
    gitHubAuthTable.addCell(removeButtonGitHub);
    jsObject.AddSmallProgressMarkerToControl(removeButtonGitHub, true);

    removeButtonGitHub.action = function () {
        this.progressMarker.changeVisibleState(true);
        jsObject.SendCloudCommand("UserRemoveLoginWithGitHub", {}, function (data) {
            if (data.ResultSuccess && panel.visible) {
                panel.updateGitHubEmail(null);
            }
        });
    }

    //Facebook
    controlsTable.addTextCellInNextRow(jsObject.loc.Cloud.ButtonLogInWith.replace("{0}", "Facebook")).style.padding = "7px 70px 7px 0px";
    var facebookAuthTable = jsObject.CreateHTMLTable();
    controlsTable.addCellInLastRow(facebookAuthTable);
    var facebookEmail = facebookAuthTable.addTextCell();
    panel.controls.facebookEmail = facebookEmail;

    var addButtonFacebook = jsObject.HiperLinkButton(null, jsObject.loc.Buttons.Add);
    addButtonFacebook.style.fontSize = "12px";
    addButtonFacebook.style.height = "35px";
    panel.controls.facebookAddButton = addButtonFacebook;
    facebookAuthTable.addCell(addButtonFacebook);
    jsObject.AddSmallProgressMarkerToControl(addButtonFacebook, true);

    addButtonFacebook.action = function () {
        jsObject.LoginWithFacebook(this, "AddLoginWithFacebook");
    }

    var removeButtonFacebook = jsObject.HiperLinkButton(null, jsObject.loc.Buttons.Remove);
    removeButtonFacebook.style.fontSize = "12px";
    removeButtonFacebook.style.height = "35px";
    removeButtonFacebook.style.margin = "0 15px 0 15px";
    panel.controls.facebookRemoveButton = removeButtonFacebook;
    facebookAuthTable.addCell(removeButtonFacebook);
    jsObject.AddSmallProgressMarkerToControl(removeButtonFacebook, true);

    removeButtonFacebook.action = function () {
        this.progressMarker.changeVisibleState(true);
        jsObject.SendCloudCommand("UserRemoveLoginWithFacebook", {}, function (data) {
            if (data.ResultSuccess && panel.visible) {
                panel.updateFacebookEmail(null);
            }
        });
    }

    function createPanel(caption, name, data, isLabel) {
        controlsTable.addRow();
        for (var i in data) {
            var row = controlsTable.addRow();
            row.style.height = "32px";
            var text = controlsTable.addCellInLastRow();
            text.style.padding = "7px 70px 7px 0px";
            text.innerHTML = data[i][1];
            var control = (isLabel || data[i][3]) ? controlsTable.addCellInLastRow() : jsObject.TextBox(data[i][0], data[i][2]);
            panel.controls[data[i][0]] = control;
            if (!isLabel) {
                controlsTable.addCellInLastRow(control);
                control.oninput = function () {
                    panel.data[this.name] = this.value;
                }
            } else {
                controlsTable.className = "stDesignerFormTextBeforeControl";
                controlsTable.style.verticalAlign = "top";
                controlsTable.style.paddingTop = "9px";
            }
        }
    }

    panel.populateFields = function (data, controls) {
        for (var i in data) {
            if (i in controls) {
                if (controls[i] instanceof HTMLInputElement) {
                    controls[i].value = data[i];
                } else {
                    controls[i].innerHTML = jsObject.JSONDateFormatToDate(data[i], true);
                }
            }
        }
    }

    panel.updateGoogleEmail = function (email) {
        panel.controls.googleEmail.innerHTML = email || "";
        panel.controls.googleAddButton.progressMarker.changeVisibleState(false);
        panel.controls.googleRemoveButton.progressMarker.changeVisibleState(false);
        panel.controls.googleAddButton.style.display = email ? "none" : "";
        panel.controls.googleRemoveButton.style.display = email ? "" : "none";
        if (panel.data) panel.data.GoogleId = email || "";
    }

    panel.updateGitHubEmail = function (email) {
        panel.controls.gitHubEmail.innerHTML = email || "";
        panel.controls.gitHubAddButton.progressMarker.changeVisibleState(false);
        panel.controls.gitHubRemoveButton.progressMarker.changeVisibleState(false);
        panel.controls.gitHubAddButton.style.display = email ? "none" : "";
        panel.controls.gitHubRemoveButton.style.display = email ? "" : "none";
        if (panel.data) panel.data.GitHubId = email || "";
    }

    panel.updateFacebookEmail = function (email) {
        panel.controls.facebookEmail.innerHTML = email || "";
        panel.controls.facebookAddButton.progressMarker.changeVisibleState(false);
        panel.controls.facebookRemoveButton.progressMarker.changeVisibleState(false);
        panel.controls.facebookAddButton.style.display = email ? "none" : "";
        panel.controls.facebookRemoveButton.style.display = email ? "" : "none";
        if (panel.data) panel.data.FacebookId = email || "";
    }

    panel.show = function () {
        this.style.display = "";
        this.visible = true;
        this.style.left = jsObject.FindPosX(parentContainer, "stiDesignerMainPanel") + "px";
        this.style.top = jsObject.FindPosY(parentContainer, "stiDesignerMainPanel") + "px";

        this.controls.specificButton.caption.innerHTML = "";
        this.controls.googleEmail.innerHTML = "";
        this.controls.googleAddButton.style.display = "none";
        this.controls.googleRemoveButton.style.display = "none";
        this.controls.googleAddButton.progressMarker.changeVisibleState(false);
        this.controls.googleRemoveButton.progressMarker.changeVisibleState(false);

        this.controls.gitHubEmail.innerHTML = "";
        this.controls.gitHubAddButton.style.display = "none";
        this.controls.gitHubRemoveButton.style.display = "none";
        this.controls.gitHubAddButton.progressMarker.changeVisibleState(false);
        this.controls.gitHubRemoveButton.progressMarker.changeVisibleState(false);

        this.controls.facebookEmail.innerHTML = "";
        this.controls.facebookAddButton.style.display = "none";
        this.controls.facebookRemoveButton.style.display = "none";
        this.controls.facebookAddButton.progressMarker.changeVisibleState(false);
        this.controls.facebookRemoveButton.progressMarker.changeVisibleState(false);

        saveButton.setEnabled(false);
        specificButton.setEnabled(false);
        passwordButton.setEnabled(false);

        if ((jsObject.options.cloudMode && !jsObject.options.cloudParameters.sessionKey) || (jsObject.options.standaloneJsMode && !jsObject.options.SessionKey)) {
            jsObject.options.forms.authForm.show();
            return;
        }

        panel.progress.show();
        var userKey = jsObject.options.standaloneJsMode ? jsObject.options.UserKey : jsObject.options.cloudParameters.userKey;

        jsObject.SendCloudCommand("UserGet", { UserKey: userKey },
            function (data) {
                if (data.ResultUser) {
                    panel.startData = jsObject.CopyObject(data.ResultUser);
                    panel.user = data.ResultUser;
                    panel.populateFields(data.ResultUser, panel.controls);
                    panel.controls.userPicture.setImage(data.ResultUser.Picture);
                    panel.controls.specificButton.caption.innerHTML = jsObject.DesignerSpecificationToSkillLevelLoc(data.ResultUser.DesignerSpecification);
                    panel.controls.googleEmail.innerHTML = data.ResultUser.GoogleId || "";
                    panel.controls.googleAddButton.style.display = !data.ResultUser.GoogleId ? "" : "none";
                    panel.controls.googleRemoveButton.style.display = data.ResultUser.GoogleId ? "" : "none";
                    panel.controls.gitHubEmail.innerHTML = data.ResultUser.GitHubId || "";
                    panel.controls.gitHubAddButton.style.display = !data.ResultUser.GitHubId ? "" : "none";
                    panel.controls.gitHubRemoveButton.style.display = data.ResultUser.GitHubId ? "" : "none";
                    panel.controls.facebookEmail.innerHTML = data.ResultUser.FacebookId || "";
                    panel.controls.facebookAddButton.style.display = !data.ResultUser.FacebookId ? "" : "none";
                    panel.controls.facebookRemoveButton.style.display = data.ResultUser.FacebookId ? "" : "none";
                    panel.data = jsObject.CopyObject(data.ResultUser);
                }
                panel.progress.hide();
                saveButton.setEnabled(true);
                specificButton.setEnabled(true);
                passwordButton.setEnabled(true);
            },
            function (data, msg) {
                panel.showError(data, msg);
            });
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

    saveButton.action = function () {
        if (panel.data.UserName == "demo" || (panel.controls["FirstName"].checkEmpty(this.jsObject.loc.Notices.AuthFirstNameIsNotSpecified) && panel.controls["LastName"].checkEmpty(this.jsObject.loc.Notices.AuthLastNameIsNotSpecified))) {
            panel.progress.show();
            jsObject.SendCloudCommand("UserSave", { User: panel.data },
                function (data) {
                    jsObject.setUserInfo();
                    panel.progress.hide();
                },
                function (data, msg) {
                    panel.showError(data, msg);
                });
        }
    }

    return panel;
}