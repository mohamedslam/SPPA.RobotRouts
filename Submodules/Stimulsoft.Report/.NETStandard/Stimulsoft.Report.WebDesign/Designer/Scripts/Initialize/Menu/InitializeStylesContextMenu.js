
StiMobileDesigner.prototype.InitializeStylesContextMenu = function (styleDesignerForm) {

    var items = [];
    items.push(this.Item("addStyle", this.loc.FormStyleDesigner.Add, "StyleAdd.png", "addStyle", null, true));
    items.push(this.Item("removeStyle", this.loc.FormStyleDesigner.Remove, "Remove.png", "removeStyle"));
    items.push("separator1");
    items.push(this.Item("duplicateStyle", this.loc.FormStyleDesigner.Duplicate, "Styles.StyleDuplicate.png", "duplicateStyle"));
    items.push("separator2");
    items.push(this.Item("cutStyle", this.loc.MainMenu.menuEditCut.replace("&", ""), "Cut.png", "cutStyle"));
    items.push(this.Item("copyStyle", this.loc.MainMenu.menuEditCopy.replace("&", ""), "Copy.png", "copyStyle"));
    items.push(this.Item("pasteStyle", this.loc.MainMenu.menuEditPaste.replace("&", ""), "PasteSmall.png", "pasteStyle"));
    items.push("separator3");
    items.push(this.Item("createStyleCollection", this.loc.FormStyleDesigner.CreateStyleCollection, "StylesCreate.png", "createStyleCollection"));

    var menu = this.BaseContextMenu("stylesContextMenu", "Up", items);

    this.InitializeSubMenu("addStyleContextMenu", this.GetAddStyleMenuItems(), menu.items["addStyle"], menu, "stiDesignerMenuMiddleItem");

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
        var selectedItem = styleDesignerForm.stylesTree.selectedItem;

        switch (menuItem.key) {
            case "StiStyle":
            case "StiChartStyle":
            case "StiCrossTabStyle":
                {
                    this.jsObject.SendCommandAddStyle(menuItem.key);
                    break;
                }
            case "removeStyle":
                {
                    if (selectedItem) selectedItem.remove();
                    break;
                }
            case "duplicateStyle":
                {
                    if (selectedItem && selectedItem.itemObject.properties) {
                        var newName = selectedItem.itemObject.properties.name + "_" + this.jsObject.loc.Report.CopyOf;
                        styleDesignerForm.stylesTree.addItem(styleDesignerForm.jsObject.CopyObject(selectedItem.itemObject), newName);
                    }
                    break;
                }
            case "cutStyle":
                {
                    if (selectedItem && selectedItem.itemObject.properties) {
                        styleDesignerForm.copiedStyle = this.jsObject.CopyObject(selectedItem.itemObject);
                        selectedItem.remove();
                    }
                    break;
                }
            case "copyStyle":
                {
                    if (selectedItem && selectedItem.itemObject.properties) {
                        styleDesignerForm.copiedStyle = this.jsObject.CopyObject(selectedItem.itemObject);
                    }
                    break;
                }
            case "pasteStyle":
                {
                    if (styleDesignerForm.copiedStyle) {
                        var newName = styleDesignerForm.copiedStyle.properties.name + "_" + this.jsObject.loc.Report.CopyOf;
                        styleDesignerForm.stylesTree.addItem(styleDesignerForm.jsObject.CopyObject(styleDesignerForm.copiedStyle), newName);
                    }
                    break;
                }
            case "createStyleCollection":
                {
                    this.jsObject.InitializeCreateStyleCollectionForm(function (createStyleCollectionForm) {
                        createStyleCollectionForm.changeVisibleState(true);
                    });
                    break;
                }
        }
    }

    menu.onshow = function () {
        var selectedItem = styleDesignerForm.stylesTree.selectedItem;
        menu.items.removeStyle.setEnabled(selectedItem && selectedItem.itemObject.typeItem != "MainItem");
        menu.items.copyStyle.setEnabled(selectedItem && selectedItem.itemObject.properties);
        menu.items.cutStyle.setEnabled(selectedItem && selectedItem.itemObject.properties);
        menu.items.duplicateStyle.setEnabled(selectedItem && selectedItem.itemObject.properties);
        menu.items.pasteStyle.setEnabled(styleDesignerForm.copiedStyle);
    }

    return menu;
}