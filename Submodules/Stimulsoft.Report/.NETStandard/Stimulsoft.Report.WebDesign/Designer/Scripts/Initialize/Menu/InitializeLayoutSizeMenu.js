
StiMobileDesigner.prototype.LayoutSizeMenu = function () {

    var menu = this.VerticalMenu("layoutSizeMenu", this.options.buttons.layoutSize, "Down", this.GetLayoutSizeItems())

    for (var name in menu.items) {
        if (menu.items[name].caption)
            menu.items[name].oldCaptionText = menu.items[name].caption.innerHTML;
    }

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
        this.jsObject.SendCommandChangeSizeComponents(menuItem.key);
    }

    menu.onshow = function (menuItem) {
        var compName = "";
        if (this.jsObject.options.selectedObjects && this.jsObject.options.selectedObjects.length > 0)
            compName = this.jsObject.options.selectedObjects[0].properties.name;

        for (var name in this.items) {
            if (this.items[name].caption)
                this.items[name].caption.innerHTML = this.items[name].oldCaptionText.replace("{0}", compName);
        }
    }

    return menu;
}