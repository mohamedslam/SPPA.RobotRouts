
StiMobileDesigner.prototype.InitializeNewFolderForm_ = function () {
    var form = this.BaseForm("newFolder", this.loc.Cloud.FolderWindowTitleNew, 2);
    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "5px";
    form.container.appendChild(innerTable);

    innerTable.addTextCell(this.loc.PropertyMain.Name).className = "stiDesignerCaptionControlsBigIntervals";

    form.nameTextBox = this.TextBox(null, 200);
    var textBoxCell = innerTable.addCell(form.nameTextBox);
    textBoxCell.className = "stiDesignerControlCellsBigIntervals2";

    form.show = function (actionFunction) {
        form.changeVisibleState(true);
        form.nameTextBox.value = form.jsObject.loc.Cloud.RibbonButtonFolder;
        form.nameTextBox.focus();
        form.actionFunction = actionFunction;
    }

    form.action = function () {
        form.actionFunction();
    }

    form.nameTextBox.actionOnKeyEnter = function () {
        form.action();
    }

    return form;
}