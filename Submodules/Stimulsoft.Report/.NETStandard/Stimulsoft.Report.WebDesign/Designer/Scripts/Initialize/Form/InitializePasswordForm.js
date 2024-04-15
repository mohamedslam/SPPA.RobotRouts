
StiMobileDesigner.prototype.InitializePasswordForm = function () {

    var passwordForm = this.BaseForm("passwordForm", this.loc.Password.StiLoadPasswordForm, 2);
    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "6px";
    passwordForm.container.appendChild(innerTable);

    var textCell = innerTable.addCell();
    textCell.innerHTML = this.loc.Password.lbPasswordSave.replace(":", "");
    textCell.className = "stiDesignerCaptionControlsBigIntervals";

    var passwordTextBox = this.TextBox(null, 200);
    passwordTextBox.setAttribute("type", "Password")
    innerTable.addCell(passwordTextBox).className = "stiDesignerControlCellsBigIntervals2";

    passwordForm.show = function (actionFunc, text) {
        this.actionFunc = actionFunc;
        this.changeVisibleState(true);
        passwordTextBox.value = "";
        setTimeout(function () { passwordTextBox.focus(); }, 50); //Fix focusing bug
        if (text) textCell.innerHTML = text;
    }

    passwordForm.action = function () {
        if (!passwordTextBox.value) {
            var errorMessageForm = this.jsObject.options.forms.errorMessageForm || this.jsObject.InitializeErrorMessageForm();
            errorMessageForm.show(this.jsObject.loc.Notices.AuthPasswordIsNotSpecified);
            return;
        }

        this.changeVisibleState(false);
        if (this.actionFunc) this.actionFunc(passwordTextBox.value);
    }

    return passwordForm;
}