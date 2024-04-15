
StiMobileDesigner.prototype.InitializeRenamePageForm_ = function () {
    var form = this.BaseForm("renamePage", this.loc.Buttons.Rename, 1);
    var jsObject = this;

    var controlsTable = this.CreateHTMLTable();
    form.container.appendChild(controlsTable);
    form.container.style.padding = "6px 0 6px 0";

    var nameControl = this.TextBox(null, 200);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Name, "name", nameControl, "6px 12px 6px 12px");

    var aliasControl = this.TextBox(null, 200);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Alias, "alias", aliasControl, "6px 12px 6px 12px");

    form.show = function () {
        this.changeVisibleState(true);
        var currentPage = this.jsObject.options.currentPage;
        if (currentPage) {
            form.oldName = nameControl.value = currentPage.properties.name;
            form.oldAlias = aliasControl.value = StiBase64.decode(currentPage.properties.aliasName);
        }
        nameControl.focus();
    }

    form.action = function () {
        this.changeVisibleState(false);
        var currentPage = this.jsObject.options.currentPage;
        if (currentPage) {
            if (aliasControl.value != form.oldAlias) {
                this.jsObject.ApplyPropertyValue("aliasName", StiBase64.encode(aliasControl.value));
                if (this.jsObject.options.pagesPanel) this.jsObject.options.pagesPanel.pagesContainer.updatePages();
                if (this.jsObject.options.reportTree) this.jsObject.options.reportTree.build();
            }
            if (nameControl.value != form.oldName) {
                this.jsObject.SendCommandRenameComponent(currentPage, nameControl.value);
            }
        }
    }

    return form
}