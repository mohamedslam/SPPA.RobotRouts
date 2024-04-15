
StiMobileDesigner.prototype.InitializeEnterEmailForm = function () {
    var jsObject = this;
    var form = this.BaseForm("enterEmailForm", "User Name (email)", 3);
    form.buttonCancel.style.display = "none";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "5px";
    form.container.appendChild(innerTable);

    var emailTextBox = this.TextBoxWithHintText(null, 280, this.loc.Authorization.TextUserName, this.loc.Authorization.TextUserName, "login.png");
    var controlCell = innerTable.addCell(emailTextBox.table);
    controlCell.className = "stiDesignerControlCellsBigIntervals";
    controlCell.style.padding = "12px";

    var textLabel = this.IsRusCulture(this.options.cultureName) ? "Для входа введите User Name (email)." : "Enter User Name (email) to log in.";
    var textCell = innerTable.addTextCellInNextRow(textLabel);
    textCell.className = "stiDesignerCaptionControlsBigIntervals";
    textCell.style.padding = "0 12px 12px 12px";

    form.show = function (completeFunc) {
        this.changeVisibleState(true);
        emailTextBox.value = "";
        emailTextBox.focus();
        this.completeFunc = completeFunc;
    }

    form.action = function () {
        if (emailTextBox.checkEmpty(jsObject.loc.Messages.ThisFieldIsNotSpecified) && emailTextBox.checkEmail(jsObject.loc.Notices.AuthUserNameShouldLookLikeAnEmailAddress)) {
            this.changeVisibleState(false);
            if (this.completeFunc) this.completeFunc(emailTextBox.value);
        }
    }

    return form;
}