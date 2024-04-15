
StiMobileDesigner.prototype.LayoutAlignMenu = function () {

    var menu = this.VerticalMenu("layoutAlignMenu", this.options.buttons.alignLayout, "Down", this.GetLayoutAlignItems())

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
        this.jsObject.SendCommandChangeArrangeComponents(menuItem.key);
    }

    menu.onshow = function () {
        var itemNames = ["AlignLeft", "AlignCenter", "AlignRight", "AlignTop", "AlignMiddle", "AlignBottom", "MakeHorizontalSpacingEqual", "MakeVerticalSpacingEqual"];
        for (var i = 0; i < itemNames.length; i++)
            this.items[itemNames[i]].setEnabled(this.jsObject.options.selectedObjects);
    }

    return menu;
}