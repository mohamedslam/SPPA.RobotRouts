
StiMobileDesigner.prototype.ColumnsAndParametersMenu = function () {

    var menu = this.VerticalMenu("columnsAndParametersMenu", this.options.buttons.retrieveColumnsAndParameters, "Down", this.GetRetrieveColumnsAndParametersItems());

    var allowRun = this.CheckBox("retrieveColumnsAllowRun", this.loc.FormDictionaryDesigner.RetrieveColumnsAllowRun);
    allowRun.style.margin = "4px";
    menu.innerContent.appendChild(allowRun);
    menu.retrieveColumnsAllowRun = allowRun;

    allowRun.action = function () {
        menu.items["retrieveColumnsAndParameters"].setEnabled(!this.isChecked);
    }

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
        var editDataSourceForm = this.jsObject.options.forms.editDataSourceForm;

        switch (menuItem.key) {
            case "retrieveColumnsAndParameters":
                {
                    editDataSourceForm.columnToolBar.retrieveColumns.action();
                    break;
                }
            case "retrieveParameters":
                {
                    editDataSourceForm.checkParametersValuesAndShowValuesForm(function (params) {
                        params.onlyParameters = true;
                        editDataSourceForm.jsObject.SendCommandRetrieveColumns(params);
                    });
                    break;
                }
        }
    }

    return menu;
}