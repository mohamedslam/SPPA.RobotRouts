StiJsViewer.prototype.InitializePasswordForm = function () {

    var passwordForm = this.BaseForm("passwordForm", this.collections.loc["PasswordSaveReport"].replace(":", ""), 2);
    passwordForm.style.fontFamily = this.options.toolbar.fontFamily;

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "5px";
    passwordForm.container.appendChild(innerTable);

    var textCell = innerTable.addCell();
    textCell.innerHTML = this.collections.loc["PasswordEnter"];
    textCell.className = "stiJsViewerCaptionControls";

    var passwordTextBox = this.TextBox(null, 200);
    passwordTextBox.setAttribute("type", "Password")
    innerTable.addCell(passwordTextBox).className = "stiJsViewerCaptionControls";

    passwordForm.show = function (actionFunc, text) {
        this.actionFunc = actionFunc;
        this.changeVisibleState(true);
        passwordTextBox.value = "";
        passwordTextBox.focus();
        if (text) textCell.innerHTML = text;
    }

    passwordForm.action = function () {
        this.changeVisibleState(false);
        if (this.actionFunc) this.actionFunc(passwordTextBox.value);
    }

    return passwordForm;
}