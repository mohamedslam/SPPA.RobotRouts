
StiMobileDesigner.prototype.InitializeStyleActionsMenu = function (styleDesignerForm) {
    var jsObject = this;
    var menu = this.VerticalMenu("styleActionsMenu", styleDesignerForm.toolBar.actions, "Down", this.GetStylesActionsMenuItems());

    styleDesignerForm.toolBar.actions.action = function () {
        menu.changeVisibleState(!menu.visible);
    }

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
        switch (menuItem.key) {
            case "openStyle": {
                if (jsObject.options.canOpenFiles) {
                    jsObject.InitializeOpenDialog("loadStyleFromFile", jsObject.StiHandleOpenStyle, ".sts");
                    jsObject.options.openDialogs.loadStyleFromFile.action();
                }
                break;
            }
            case "saveStyle": {
                jsObject.SendCommandSaveStyle(styleDesignerForm.stylesCollection);
                break;
            }
            case "createStyleCollection": {
                jsObject.InitializeCreateStyleCollectionForm(function (createStyleCollectionForm) {
                    createStyleCollectionForm.changeVisibleState(true);
                });
                break;
            }
        }
    }

    return menu;
}