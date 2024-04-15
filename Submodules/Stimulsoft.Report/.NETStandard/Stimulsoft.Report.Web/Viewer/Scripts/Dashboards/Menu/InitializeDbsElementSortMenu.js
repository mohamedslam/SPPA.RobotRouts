
StiJsViewer.prototype.DbsElementSortMenu = function (element, itemStyleName, menuStyleName) {
    var menu = this.VerticalMenu("dbsElementSortMenu", element.buttons.sort, "Down", [], itemStyleName, menuStyleName);
    if (!this.options.isMobileDevice) menu.innerContent.style.maxHeight = "1000px";

    var jsObject = this;

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
    }

    menu.onshow = function () {
        var sortItems = element.elementAttributes.contentAttributes.sortItems;
        menu.sortDirection = "Ascending";
        menu.clear();

        if (sortItems && sortItems.length > 0) {
            var sortChecked = false;
            menu.sortDirection = sortItems[0].sortDirection;

            for (var i = 0; i < sortItems.length; i++) {
                var text = sortItems[i].isSortByVariation ? jsObject.collections.loc.Variation : sortItems[i].text;
                menu.addItem("sortItem" + i, jsObject.collections.loc["SortBy"] + " " + text, sortItems[i].key, sortItems[i].checked);
                if (sortItems[i].checked) sortChecked = true;
            }

            menu.addItem("sortItemNone", jsObject.collections.loc["SortNone"], "sortNone", !sortChecked);

            if (sortChecked) {
                menu.innerContent.appendChild(jsObject.VerticalMenuSeparator(this, "separator"));
                menu.addItem("sortAsc", jsObject.collections.loc["SortAsc"], "sortAsc", menu.sortDirection == "Ascending");
                menu.addItem("sortDesc", jsObject.collections.loc["SortDesc"], "sortDesc", menu.sortDirection == "Descending");
            }
        }
    }

    menu.addItem = function (name, caption, key, checked) {
        var item = jsObject.SortFilterMenuItem(this, name, caption, null, key, false, itemStyleName);
        item.setChecked(checked);
        menu.innerContent.appendChild(item);
        menu.items[name] = item;

        item.action = function () {
            menu.changeVisibleState(false);
            if (this.isChecked) return;
            var sorts = [];

            if (this.name.indexOf("sortItem") == 0) {
                for (var itemName in menu.items) {
                    if (itemName.indexOf("sortItem") == 0) {
                        menu.items[itemName].setChecked(false);
                    }
                }
                this.setChecked(true);

                if (this.key != "sortNone") {
                    sorts = [jsObject.DataSortObject(this.key, menu.sortDirection)];
                }
            }
            else if (this.key == "sortAsc" || this.key == "sortDesc") {
                var sortKey = null;
                for (var itemName in menu.items) {
                    if (itemName.indexOf("sortItem") == 0 && itemName.indexOf("sortItemNone") != 0 && menu.items[itemName].isChecked) {
                        sortKey = menu.items[itemName].key;
                        break;
                    }
                }
                menu.items.sortAsc.setChecked(this.key == "sortAsc");
                menu.items.sortDesc.setChecked(this.key == "sortDesc");
                if (sortKey) sorts = [jsObject.DataSortObject(sortKey, this.key == "sortAsc" ? "Ascending" : "Descending")];
            }

            jsObject.ApplySortsToDashboardElement(element, sorts, true);
        }

        return item;
    }

    return menu;
}