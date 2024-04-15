
StiMobileDesigner.prototype.BarCodesMenu = function (menuName, parentButton, isToolboxMenu) {
    var jsObject = this;

    var menu = isToolboxMenu
        ? this.HorizontalMenu(menuName, parentButton, "Right", this.GetBarCodeCategoriesItems())
        : this.VerticalMenu(menuName, parentButton, "Down", this.GetBarCodeCategoriesItems());

    if (isToolboxMenu) menu.type = "Menu";

    var subMenus = [];
    subMenus.push(this.InitializeSubMenu(menuName + "TwoDimensionalMenu", this.GetBarCodeTwoDimensionalItems(), menu.items["TwoDimensional"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "EANUPCMenu", this.GetBarCodeEANUPCItems(), menu.items["EANUPC"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "GS1Menu", this.GetBarCodeGS1Items(), menu.items["GS1"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "PostMenu", this.GetBarCodePostItems(), menu.items["Post"], menu));
    subMenus.push(this.InitializeSubMenu(menuName + "OthersMenu", this.GetBarCodeOthersItems(), menu.items["Others"], menu));
    subMenus[subMenus.length - 1].firstChild.style.maxHeight = "700px"

    for (var i = 0; i < subMenus.length; i++) {
        for (var itemKey in subMenus[i].items) {
            var item = subMenus[i].items[itemKey];
            item.name = "StiBarCode;" + item.key;
            this.AddDragEventsToComponentButton(item);
        }
    }

    menu.action = function (menuItem) {
        if (menuItem.haveSubMenu) return;
        menu.changeVisibleState(false);

        var panel = isToolboxMenu ? jsObject.options.toolbox : jsObject.options.insertPanel;
        if (panel) panel.resetChoose();

        jsObject.options.drawComponent = true;

        var paintPanel = jsObject.options.paintPanel;
        paintPanel.setCopyStyleMode(false);
        paintPanel.changeCursorType(true);

        if (panel) panel.selectedComponent = menuItem;
    }

    return menu;
}
