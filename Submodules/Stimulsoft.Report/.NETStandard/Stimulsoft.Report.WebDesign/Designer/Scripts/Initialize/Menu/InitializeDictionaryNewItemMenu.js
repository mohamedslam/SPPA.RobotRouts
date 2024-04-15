
StiMobileDesigner.prototype.InitializeDictionaryNewItemMenu = function () {
    var jsObject = this;
    var menu = this.VerticalMenu("dictionaryNewItemMenu", this.options.dictionaryPanel.toolBar.controls["NewItem"], "Down", this.GetDictionaryNewItems());

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
        switch (menuItem.key) {
            case "dataSourceNew":
                {
                    jsObject.InitializeSelectConnectionForm(function (selectConnectionForm) {
                        selectConnectionForm.changeVisibleState(true);
                    });
                    break;
                }
            case "dataTransformationNew":
                {
                    if (jsObject.options.cloudMode && jsObject.GetCurrentPlanLimitValue("AllowDataTransformation") === false) {
                        jsObject.InitializeNotificationForm(function (form) {
                            form.show(jsObject.NotificationMessages("availableDataSources"), jsObject.NotificationMessages("availableDataSourcesInDesktopVersion"), "Notifications.Blocked.png");
                        });
                        return;
                    }
                    jsObject.InitializeEditDataTransformationForm(function (form) {
                        form.datasource = null;
                        form.changeVisibleState(true);
                    });
                    break;
                }
            case "businessObjectNew":
                {
                    jsObject.InitializeEditDataSourceForm(function (editDataSourceForm) {
                        editDataSourceForm.datasource = "BusinessObject";
                        editDataSourceForm.changeVisibleState(true);
                    });
                    break;
                }
            case "relationNew":
                {
                    jsObject.InitializeEditRelationForm(function (editRelationForm) {
                        editRelationForm.relation = null;
                        editRelationForm.changeVisibleState(true);
                    });
                    break;
                }
            case "columnNew":
            case "calcColumnNew":
                {
                    jsObject.InitializeEditColumnForm(function (editColumnForm) {
                        editColumnForm.column = menuItem.key == "columnNew" ? "column" : "calcColumn";
                        editColumnForm.changeVisibleState(true);
                    });
                    break;
                }
            case "parameterNew":
                {
                    jsObject.InitializeEditParameterForm(function (editParameterForm) {
                        editParameterForm.parameter = null;
                        editParameterForm.changeVisibleState(true);
                    });
                    break;
                }
            case "variableNew":
                {
                    jsObject.InitializeEditVariableForm(function (editVariableForm) {
                        editVariableForm.variable = null;
                        editVariableForm.changeVisibleState(true);
                    });
                    break;
                }
            case "categoryNew":
                {
                    jsObject.InitializeEditCategoryForm(function (editCategoryForm) {
                        editCategoryForm.category = null;
                        editCategoryForm.changeVisibleState(true);
                    });
                    break;
                }
            case "resourceNew":
                {
                    if (jsObject.options.dictionaryPanel.checkResourcesCount()) return;
                    jsObject.InitializeEditResourceForm(function (editResourceForm) {
                        editResourceForm.resource = null;
                        editResourceForm.changeVisibleState(true);
                    });
                    break;
                }
        }
    }

    return menu;
}