
StiJsViewer.prototype.InitializeSubMenu = function (name, items, parentButton, parentMenu, itemStyleName, menuStyleName) {

    var menu = this.HorizontalMenu(name, parentButton, "Right", items, itemStyleName, menuStyleName);
    menu.parentMenu = parentMenu;
    parentButton.menu = menu;

    parentButton.showSubMenu = function () {
        if (parentMenu.currentSubMenu && parentButton.menu != parentMenu.currentSubMenu) {
            parentMenu.currentSubMenu.changeVisibleState(false);
        }
        if (!parentButton.menu.visible) {
            parentButton.menu.changeVisibleState(true);
            parentMenu.currentSubMenu = parentButton.menu;
        }
    }

    parentButton.action = function () {
        this.onmouseover();
    }

    if (!this.options.isTouchDevice) {
        parentButton.onmouseover = function () {
            if (!this.isEnabled) return;
            this.className = this.styleName + " " + this.styleName + "Over";
            this.isOver = true;
            clearTimeout(parentMenu.subMenuShowTimer);
            clearTimeout(parentMenu.subMenuHideTimer);
            parentMenu.subMenuShowTimer = setTimeout(function () {
                if (parentButton.isOver) parentButton.showSubMenu();
            }, 200);
        }

        try {
            for (var itemName in parentMenu.items) {
                if (itemName.indexOf("separator") != 0 && !parentMenu.items[itemName].haveSubMenu && parentMenu.items[itemName] != parentButton) {
                    parentMenu.items[itemName].onmouseenter = function () {
                        if (!this.isEnabled || this.jsObject.options.isTouchClick) return;
                        this.className = this.styleName + " " + this.styleName + "Over";
                        this.isOver = true;
                        clearTimeout(parentMenu.subMenuShowTimer);
                        clearTimeout(parentMenu.subMenuHideTimer);
                        parentMenu.subMenuHideTimer = setTimeout(function () {
                            if (menu.currentSubMenu) menu.currentSubMenu.changeVisibleState(false);
                            menu.changeVisibleState(false);
                        }, 200);
                    }
                }
            }
        }
        catch (e) { console.log(e); }

        menu.onmouseover = function () {
            clearTimeout(parentMenu.subMenuHideTimer);
        }
    }

    menu.action = function (menuItem) {
        menu.changeVisibleState(false);
        parentMenu.changeVisibleState(false);
        parentMenu.action(menuItem);
    }

    return menu;
}