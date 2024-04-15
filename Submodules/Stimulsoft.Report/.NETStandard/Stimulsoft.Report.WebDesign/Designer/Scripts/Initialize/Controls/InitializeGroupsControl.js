
StiMobileDesigner.prototype.GroupsControl = function (name, widthContainer, heightContainer) {
    var groupsControl = this.SortControl(name, null, widthContainer, heightContainer);

    //Override
    groupsControl.addSortButton.caption.innerHTML = this.loc.FormBand.AddGroup.replace("&", "");
    groupsControl.noSortText = this.loc.PropertyEnum.DialogResultNo;
    groupsControl.isGroupsControl = true;

    groupsControl.sortContainer.addSort = function (sortObject, notAction) {
        var sortItem = this.jsObject.SortItem(this);
        sortItem.textCell.innerHTML = "";
        sortItem.textCell.style.padding = "0px";

        var items = this.jsObject.GetSortDirectionItemsForGroupsControl();
        sortItem.direction.items = items;
        sortItem.direction.menu.addItems(items);

        this.appendChild(sortItem);
        sortItem.direction.setKey(sortObject.direction);
        sortItem.column.textBox.value = sortObject.column == "" ? groupsControl.noSortText : sortObject.column;
        sortItem.setSelected(true);

        if (!notAction) this.onAction();
    }

    groupsControl.getValue = function () {
        var result = [];
        for (var num = 0; num < this.sortContainer.childNodes.length; num++) {
            var item = this.sortContainer.childNodes[num];
            if (item.key && item.column.textBox.value != this.noSortText) {
                result.push({ "direction": item.direction.key, "column": item.column.textBox.value });
            }
        }
        return result;
    }

    return groupsControl;
}