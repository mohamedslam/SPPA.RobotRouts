
StiMobileDesigner.prototype.InitializeDeleteItemsContextMenu = function (name) {

    var items = [];
    items.push(this.Item("delete", this.loc.Buttons.Delete, null, "delete"));
    items.push(this.Item("deleteAll", this.loc.Cloud.ButtonDeleteAll, null, "deleteAll"));

    var menu = this.BaseContextMenu(name, "Down", items);

    return menu;
}