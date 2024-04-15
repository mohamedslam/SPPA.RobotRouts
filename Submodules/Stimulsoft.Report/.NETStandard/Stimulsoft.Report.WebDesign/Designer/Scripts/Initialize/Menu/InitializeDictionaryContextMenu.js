
StiMobileDesigner.prototype.InitializeDictionaryContextMenu = function () {
    var jsObject = this;
    var items = this.GetDictionaryNewItems();
    items.push("separator3_0");
    items.push(this.Item("viewData", this.loc.FormDictionaryDesigner.ViewData, "Query.ViewData.png", "viewData"));
    items.push("separator3");
    items.push(this.Item("editItem", this.loc.QueryBuilder.Edit, "Edit.png", "editItem"));
    items.push(this.Item("duplicateItem", this.loc.Buttons.Duplicate, "Duplicate.png", "duplicateItem"));
    items.push(this.Item("properties", this.loc.Panels.Properties, "Properties.png", "properties"));
    items.push(this.Item("deleteItem", this.loc.MainMenu.menuEditDelete.replace("&", ""), "Remove.png", "deleteItem"));
    items.push("separator4");
    items.push(this.Item("expandAll", this.loc.Report.ExpandAll, "ExpandAll.png", "expandAll"));
    items.push(this.Item("collapseAll", this.loc.Report.CollapseAll, "CollapseAll.png", "collapseAll"));

    var menu = this.BaseContextMenu("dictionaryContextMenu", "Up", items);
    menu.innerContent.style.maxHeight = "450px";

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
            case "dataSourceNewFromResource":
                {
                    if (jsObject.options.dictionaryTree.selectedItem) {
                        jsObject.SendCommandCreateDatabaseFromResource({ name: jsObject.options.dictionaryTree.selectedItem.itemObject.name }, true);
                    }
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
            case "properties":
                {
                    var propPanel = jsObject.options.propertiesPanel;
                    if (propPanel) {
                        propPanel.showContainer("Properties");
                        propPanel.setDictionaryDataMode(true);
                    }
                    break;
                }
            case "duplicateItem": { jsObject.DuplicateItemDictionaryTree(); break; }
            case "editItem": { jsObject.EditItemDictionaryTree(); break; }
            case "deleteItem": { jsObject.DeleteItemDictionaryTree(); break; }
            case "expandAll": { jsObject.SetOpeningAllChildDictionaryTree(true); break; }
            case "collapseAll": { jsObject.SetOpeningAllChildDictionaryTree(false); break; }
            case "viewData":
                {
                    var selectedItemObject = jsObject.options.dictionaryTree.selectedItem.itemObject;

                    if (selectedItemObject.typeItem == "BusinessObject") {
                        jsObject.InitializeViewDataForm(function (viewDataForm) {
                            viewDataForm.show(selectedItemObject);
                        });
                    }
                    else if (selectedItemObject.typeItem == "DataSource") {
                        if (selectedItemObject.typeDataSource == "StiDataTransformation") {
                            jsObject.InitializeViewDataForm(function (viewDataForm) {
                                viewDataForm.show(selectedItemObject);
                            });
                        }
                        else {
                            jsObject.InitializeEditDataSourceForm(function (editDataSourceForm) {
                                editDataSourceForm.datasource = jsObject.CopyObject(selectedItemObject);
                                editDataSourceForm.onshow();
                                editDataSourceForm.checkParametersValuesAndShowValuesForm(function (parameters) {
                                    jsObject.InitializeViewDataForm(function (viewDataForm) {
                                        viewDataForm.show(parameters);
                                    });
                                });
                            });
                        }
                    }
                    break;
                }
            case "menuMakeThisRelationActive": {
                var relation = jsObject.options.dictionaryTree.selectedItem.itemObject;
                if (relation && relation.typeItem == "Relation") {
                    relation = jsObject.CopyObject(relation);
                    relation.active = true;
                    relation.copyModeActivated = false;
                    relation["mode"] = "Edit";
                    relation["oldNameInSource"] = relation.nameInSource;
                    jsObject.SendCommandCreateOrEditRelation(relation);
                }
                break;
            }
        }
    }

    return menu;
}