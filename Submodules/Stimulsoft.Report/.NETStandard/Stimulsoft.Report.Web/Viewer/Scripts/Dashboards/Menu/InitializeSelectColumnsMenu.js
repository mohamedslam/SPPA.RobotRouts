
StiJsViewer.prototype.InitializeSelectColumnsMenu = function (element, itemStyleName, menuStyleName) {
    var menu = this.VerticalMenu("dbsElementSortMenu", element.buttons.sort, "Down", [], itemStyleName, menuStyleName);
    if (!this.options.isMobileDevice) menu.innerContent.style.maxHeight = "1000px";

    var jsObject = this;

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
    }

    menu.onshow = function () {
        menu.clear();
        var data = element.elementAttributes.contentAttributes.data;
        var hiddenData = element.elementAttributes.contentAttributes.hiddenData;
        var columnsArray = [];
        if (data) {
            for (var i = 0; i < data[0].length; i++) {
                var columnObj = data[0][i];
                columnsArray[columnObj.columnIndex] = columnObj;
            }
        }
        if (hiddenData) {
            for (var i = 0; i < hiddenData[0].length; i++) {
                var columnObj = hiddenData[0][i];
                if (jsObject.tableElementHiddenColumns && jsObject.tableElementHiddenColumns[element.elementAttributes.key + columnObj.key] != null)
                    columnsArray[columnObj.columnIndex] = columnObj;
            }
        }
        for (var i = 0; i < columnsArray.length; i++) {
            var columnObj = columnsArray[i];
            if (columnObj) {
                var hideColumn = jsObject.tableElementHiddenColumns && jsObject.tableElementHiddenColumns[element.elementAttributes.key + columnObj.key] == true;
                menu.addItem("sortItem" + i, columnObj.label, columnObj.key, !hideColumn);
            }
        }
    }

    menu.addItem = function (name, caption, key, checked) {
        var item = jsObject.SortFilterMenuItem(this, name, caption, null, key, false, itemStyleName);
        item.setChecked(checked);
        menu.innerContent.appendChild(item);
        menu.items[name] = item;

        item.action = function () {
            this.setChecked(!this.isChecked);
            if (!jsObject.tableElementHiddenColumns) {
                jsObject.tableElementHiddenColumns = {};
            }
            jsObject.tableElementHiddenColumns[element.elementAttributes.key + this.key] = !this.isChecked;
            if (!jsObject.selectColumnsInProgress) {
                clearTimeout(menu.actionTimer);
                menu.actionTimer = setTimeout(function () { jsObject.ChangeTableElementSelectColumns(element); }, 400);
            }
            else {
                jsObject.waitToSelectColumns = true;
            }
        }

        return item;
    }

    return menu;
}