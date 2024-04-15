
StiJsViewer.prototype.SortFilterMenu = function (headerButton, dataGrid, element, itemStyleName, menuStyleName) {
    if (this.controls.menus && this.controls.menus.viewerSortFilterMenu) {
        var sortFilterMenu = this.controls.menus.viewerSortFilterMenu
        this.controls.menus.viewerSortFilterMenu = null;
        this.controls.mainPanel.removeChild(sortFilterMenu);
    }

    var menu = this.VerticalMenu("viewerSortFilterMenu", headerButton, "Down", [], itemStyleName, menuStyleName);
    if (!this.options.isMobileDevice) menu.innerContent.style.maxHeight = "1000px";

    var jsObject = this;

    menu.action = function (menuItem) {
        if (menuItem.name.indexOf("filterItem") == 0) {
            menuItem.setChecked(!menuItem.isChecked);
            menu.updateSelectAllItemState();
            menu.contentAttributes.filters = this.getFilters();
            menu.updateRemoveFilterItemState();
            menu.isModified = true;
            return;
        }

        switch (menuItem.name) {
            case "selectAll": {
                menu.selectAllFilterItems(!menuItem.isChecked);
                menu.contentAttributes.filters = menuItem.isChecked ? [] : [jsObject.DataFilterObject(this.columnObject.key, this.columnObject.path, "IsFalse")];
                menu.updateRemoveFilterItemState();
                menu.isModified = true;
                return;
            }
            case "removeFilter": {
                var filters = menu.getThisColumnFilters();
                for (var i = 0; i < filters.length; i++) {
                    jsObject.RemoveElementFromArray(menu.contentAttributes.filters, filters[i]);
                }
                menu.selectAllFilterItems(true);
                menu.items.removeFilter.setEnabled(false);
                menu.isModified = true;
                break;
            }
            case "sortAsc":
            case "sortDesc":
            case "noSort": {
                this.items["sortAsc"].setChecked(menuItem.key == "Ascending");
                this.items["sortDesc"].setChecked(menuItem.key == "Descending");
                this.items["noSort"].setChecked(menuItem.key == "None");
                this.setSortDirection(menuItem.key);
                break;
            }
            case "customFilter": {
                var filtersForm = jsObject.InitializeFiltersForm();

                filtersForm.show(menu.columnObject, menu.getThisColumnFilters(), menu.getFilterItemsForEditors());

                filtersForm.action = function () {
                    this.changeVisibleState(false);
                    var otherFilters = menu.getNotThisColumnFilters();
                    var resultFilters = filtersForm.filterControl.getFilters()
                    menu.contentAttributes.filters = otherFilters.concat(resultFilters);
                    jsObject.ApplyFiltersToDashboardElement(element, menu.contentAttributes.filters, true);
                }
                break;
            }
        }

        this.changeVisibleState(false);
    }

    menu.onshow = function () {
        var elementAttrs = element.elementAttributes;
        var contentAttrs = elementAttrs.contentAttributes;
        menu.clear();
        menu.columnObject = headerButton.columnObject;
        menu.contentAttributes = contentAttrs;
        menu.isModified = false;

        var startItems = [];

        if (contentAttrs.interaction.allowUserSorting) {
            startItems.push(jsObject.Item("sortAsc", jsObject.ColumnIsDateType(menu.columnObject.dataType) ? jsObject.collections.loc["DashboardSortOldestToNewest"] :
                (jsObject.ColumnIsNumericType(menu.columnObject.dataType) ? jsObject.collections.loc["DashboardSortSmallestToLargest"] : jsObject.collections.loc["DashboardSortAZ"]), null, "Ascending"));
            startItems.push(jsObject.Item("sortDesc", jsObject.ColumnIsDateType(menu.columnObject.dataType) ? jsObject.collections.loc["DashboardSortNewestToOldest"] :
                (jsObject.ColumnIsNumericType(menu.columnObject.dataType) ? jsObject.collections.loc["DashboardSortLargestToSmallest"] : jsObject.collections.loc["DashboardSortZA"]), null, "Descending"));
            startItems.push(jsObject.Item("noSort", jsObject.collections.loc["FormBandNoSort"], null, "None"));

            menu.addItems(startItems);
            menu.updateItemsStates();
        }

        if (contentAttrs.interaction.allowUserFiltering) {
            var fContainer = document.createElement("div");
            fContainer.style.height = "280px";
            fContainer.style.minWidth = !contentAttrs.interaction.allowUserSorting ? "150px" : "100%";
            fContainer.style.position = "relative";
            menu.innerContent.appendChild(fContainer);
            jsObject.AddProgressToControl(fContainer);

            if (elementAttrs.actionColors && elementAttrs.actionColors.isDarkStyle)
                fContainer.progress.setToLightStyle();
            else
                fContainer.progress.setToDefaultStyle();

            fContainer.progress.show();

            var addMenuItems = function (filterItemsHelper) {
                fContainer.progress.hide();
                menu.innerContent.removeChild(fContainer);

                var items = [];
                var filterItems = [];

                menu.columnObject.path = filterItemsHelper.columnPath;

                if (contentAttrs.interaction.allowUserSorting) items.push("separatorSort");

                for (var i = 0; i < filterItemsHelper.mainItems.length; i++) {
                    var filterItem = filterItemsHelper.mainItems[i];
                    filterItems.push(jsObject.Item("filterItem" + i, filterItem.displayString, null, { filterString: filterItem.filterString }));
                }

                if (filterItemsHelper.haveNulls)
                    filterItems.push(jsObject.Item("filterItemIsNull", jsObject.collections.loc["DashboardNulls"], null, { isNullValue: true, filterString: null }));

                if (filterItemsHelper.haveBlanks)
                    filterItems.push(jsObject.Item("filterItemIsBlank", jsObject.collections.loc["DashboardBlanks"], null, { isBlankValue: true, filterString: "" }));

                menu.filterItems = filterItems

                if (jsObject.ColumnIsNumericType(menu.columnObject.dataType)) {
                    items.push(jsObject.Item("numberFilters", jsObject.collections.loc["DashboardNumberFilters"], null, "numberFilters", true));
                }
                else if (jsObject.ColumnIsDateType(menu.columnObject.dataType)) {
                    items.push(jsObject.Item("dateFilters", jsObject.collections.loc["DashboardDateFilters"], null, "dateFilters", true));
                }
                else if (menu.columnObject.dataType == "bool") {
                    items.push(jsObject.Item("booleanFilters", jsObject.collections.loc["DashboardBooleanFilters"], null, "booleanFilters", true));
                }
                else {
                    items.push(jsObject.Item("stringFilters", jsObject.collections.loc["DashboardStringFilters"], null, "stringFilters", true));
                }

                items.push(jsObject.Item("customFilter", jsObject.collections.loc["DashboardCustomFilter"], null, "customFilter", null));
                items.push(jsObject.Item("removeFilter", jsObject.collections.loc["RemoveFilter"], null, "removeFilter", null));

                if (filterItems.length > 0) {
                    items.push("separatorFilter");
                    items.push(jsObject.Item("selectAll", jsObject.collections.loc["SelectAll"], null, "selectAll"));
                }

                items = items.concat(filterItems);
                menu.addItems(items);
                menu.updateItemsStates();

                //Add filter submenus
                if (menu.items["numberFilters"]) {
                    var numberFiltersMenu = jsObject.InitializeSubMenu("viewerSortFilterMenuNumberFilters", jsObject.GetFilterConditionItems("Numeric", false), menu.items["numberFilters"], menu, itemStyleName, menuStyleName);
                    numberFiltersMenu.action = function (menuItem) {
                        this.changeVisibleState(false);
                        menu.filterSubMenuAction(menuItem.key);
                    }
                }

                if (menu.items["dateFilters"]) {
                    var dateFiltersMenu = jsObject.InitializeSubMenu("viewerSortFilterMenuDateFilters", jsObject.GetFilterConditionItems("DateTime", false), menu.items["dateFilters"], menu, itemStyleName, menuStyleName);
                    dateFiltersMenu.action = function (menuItem) {
                        this.changeVisibleState(false);
                        menu.filterSubMenuAction(menuItem.key);
                    }
                }

                if (menu.items["booleanFilters"]) {
                    var booleanFiltersMenu = jsObject.InitializeSubMenu("viewerSortFilterMenuBooleanFilters", jsObject.GetFilterConditionItems("Boolean", false), menu.items["booleanFilters"], menu, itemStyleName, menuStyleName);
                    booleanFiltersMenu.action = function (menuItem) {
                        this.changeVisibleState(false);
                        menu.filterSubMenuAction(menuItem.key);
                    }
                }

                if (menu.items["stringFilters"]) {
                    var stringFiltersMenu = jsObject.InitializeSubMenu("viewerSortFilterMenuStringFilters", jsObject.GetFilterConditionItems("String", false, true), menu.items["stringFilters"], menu, itemStyleName, menuStyleName);
                    stringFiltersMenu.action = function (menuItem) {
                        this.changeVisibleState(false);
                        menu.filterSubMenuAction(menuItem.key);
                    }
                }

                if (!jsObject.options.isMobileDevice && menu.parentButton && menu.parentButton.offsetWidth && menu.parentButton.offsetHeight) {
                    menu.correctPositions();
                }
            }

            var columnKey = menu.columnObject.key;

            setTimeout(function () {
                if (dataGrid.menuItems && dataGrid.menuItems[columnKey]) {
                    addMenuItems(dataGrid.menuItems[columnKey]);
                }
                else {
                    jsObject.postAjax(jsObject.getActionRequestUrl(jsObject.options.requestUrl, jsObject.options.actions.viewerEvent),
                        {
                            action: "DashboardGettingFilterItems",
                            dashboardFilteringParameters: {
                                elementName: elementAttrs.name,
                                elementGroup: elementAttrs.group,
                                columnIndex: headerButton.columnIndex
                            }
                        },
                        function (answer) {
                            var filterItemsHelper = JSON.parse(jsObject.options.server.useCompression ? StiGZipHelper.unpack(answer) : answer);
                            if (!filterItemsHelper || !filterItemsHelper.mainItems) {
                                fContainer.progress.hide();
                                menu.innerContent.removeChild(fContainer);
                                if (!jsObject.options.isMobileDevice && menu.parentButton && menu.parentButton.offsetWidth && menu.parentButton.offsetHeight) {
                                    menu.correctPositions();
                                }
                                return;
                            }
                            addMenuItems(filterItemsHelper);
                            if (dataGrid.menuItems) {
                                dataGrid.menuItems[columnKey] = filterItemsHelper;
                            }
                        });
                }
            }, jsObject.options.isMobileDevice ? 250 : jsObject.options.menuAnimDuration);
        }
    }

    menu.onHide = function () {
        if (menu.isModified) {
            jsObject.ApplyFiltersToDashboardElement(element, menu.contentAttributes.filters, true);
        }
    }

    menu.updateItemsStates = function () {
        if (this.contentAttributes.interaction.allowUserSorting) {
            var sort = this.getThisColumnSort();
            var sortDirection = sort ? sort.direction : "None";
            this.items["sortAsc"].setChecked(sortDirection == "Ascending");
            this.items["sortDesc"].setChecked(sortDirection == "Descending");
            this.items["noSort"].setChecked(sortDirection == "None");
        }

        if (this.contentAttributes.interaction.allowUserFiltering) {
            var filters = menu.getThisColumnFilters(this.contentAttributes, this.columnObject);
            var filterType = menu.getDataFilterConditionGroupType(filters);

            if (filters.length == 0) {
                this.selectAllFilterItems(true);
            }
            else {
                for (var itemName in this.items) {
                    var item = this.items[itemName];
                    if (item.name && item.name.indexOf("filterItem") == 0) {
                        var filterString = this.items[itemName].key != null ? this.items[itemName].key.filterString : null;
                        this.items[itemName].setChecked(this.checkCondition(filterType, filterString, filters));
                    }
                }
            }

            this.updateSelectAllItemState();
            this.updateRemoveFilterItemState();
        }
    }

    menu.filterSubMenuAction = function (itemKey) {
        this.changeVisibleState(false);

        var filtersForm = jsObject.InitializeFiltersForm();
        filtersForm.show(menu.columnObject, [jsObject.DataFilterObject(menu.columnObject.key, menu.columnObject.path, itemKey, menu.columnObject.dataType == "bool" ? "False" : null)], menu.getFilterItemsForEditors());

        filtersForm.action = function () {
            this.changeVisibleState(false);
            var filters = menu.getNotThisColumnFilters();
            var resultFilters = filtersForm.filterControl.getFilters();
            menu.contentAttributes.filters = filters.concat(resultFilters);
            jsObject.ApplyFiltersToDashboardElement(element, menu.contentAttributes.filters, true);
        }
    }

    menu.setSortDirection = function (direction) {
        var currentSort = this.getThisColumnSort();
        var columnKey = this.columnObject.key;

        if (currentSort == null && direction != "None") {
            this.contentAttributes.sorts.push(jsObject.DataSortObject(columnKey, direction));
        }
        else {
            if (direction == "None") {
                jsObject.RemoveElementFromArray(this.contentAttributes.sorts, currentSort);
            }
            else {
                currentSort.direction = direction;
            }
        }

        jsObject.ApplySortsToDashboardElement(element, this.contentAttributes.sorts, true);
    }

    menu.getFilters = function () {
        var filters = this.getNotThisColumnFilters();

        if (this.items["selectAll"].isChecked)
            return filters;

        var itemsCount = menu.filterItems.length;
        var itemsCheckedCount = menu.getFilterItemsCheckedCount();
        var conditionGroupType = itemsCheckedCount > 0 && itemsCheckedCount <= itemsCount / 2 ? "Equal" : "NotEqual";
        var columnKey = this.columnObject.key;
        var columnPath = this.columnObject.path;
        var filter;

        for (var itemName in this.items) {
            var item = this.items[itemName];
            if (item.name && item.name.indexOf("filterItem") == 0) {
                var itemKey = item.key;
                var filterString = itemKey.filterString;

                if (item.isChecked && conditionGroupType == "Equal") {
                    if (itemKey.isNullValue)
                        filter = jsObject.DataFilterObject(columnKey, columnPath, "IsNull");
                    else if (itemKey.isBlankValue)
                        filter = jsObject.DataFilterObject(columnKey, columnPath, "IsBlank");
                    else
                        filter = jsObject.DataFilterObject(columnKey, columnPath, "EqualTo", filterString);

                    filters.push(filter);
                }
                else if (!item.isChecked && conditionGroupType == "NotEqual") {
                    if (itemKey.isNullValue)
                        filter = jsObject.DataFilterObject(columnKey, columnPath, "IsNotNull");
                    else if (itemKey.isBlankValue)
                        filter = jsObject.DataFilterObject(columnKey, columnPath, "IsNotBlank");
                    else
                        filter = jsObject.DataFilterObject(columnKey, columnPath, "NotEqualTo", filterString);

                    filters.push(filter);
                }
            }
        }

        return filters;
    }

    menu.getThisColumnSort = function () {
        var contentAttrs = this.contentAttributes;
        for (var i = 0; i < contentAttrs.sorts.length; i++) {
            if (contentAttrs.sorts[i].key == this.columnObject.key) {
                return contentAttrs.sorts[i];
            }
        }
        return null;
    }

    menu.getThisColumnFilters = function () {
        var contentAttrs = this.contentAttributes;
        var filters = [];
        for (var i = 0; i < contentAttrs.filters.length; i++) {
            var filter = contentAttrs.filters[i];

            if (filter.key == this.columnObject.key || (!filter.key && filter.path == this.columnObject.path)) {
                filters.push(filter);
            }
        }
        return filters;
    }

    menu.getNotThisColumnFilters = function () {
        var contentAttrs = this.contentAttributes;
        var filters = [];
        for (var i = 0; i < contentAttrs.filters.length; i++) {
            var filter = contentAttrs.filters[i];
            if (filter.key == this.columnObject.key || (!filter.key && filter.path == this.columnObject.path)) {
                continue;
            }
            else {
                filters.push(filter);
            }
        }
        return filters;
    }

    menu.getDataFilterConditionGroupType = function (filters) {

        if (filters.length == 0) {
            return "Empty";
        }

        if (filters.every(function (r) { return (r.condition == "EqualTo" || r.condition == "IsBlank" || r.condition == "IsNull") })) {
            return "Equal";
        };

        if (filters.every(function (r) { return (r.condition == "NotEqualTo" || r.condition == "IsNotBlank" || r.condition == "IsNotNull") })) {
            return "NotEqual";
        };

        return "Custom";
    }

    menu.checkCondition = function (filterType, value, filters) {

        if (filterType == "Equal") {
            if (value == null)
                return filters.some(function (r) { return (r.condition == "IsNull") });

            if (value == "")
                return filters.some(function (r) { return (r.condition == "IsBlank") });

            return filters.some(function (r) { return (r.value == value) });
        }

        if (filterType == "NotEqual") {
            if (value == null)
                return !filters.some(function (r) { return (r.condition == "IsNotNull") });

            if (value == "")
                return !filters.some(function (r) { return (r.condition == "IsNotBlank") });

            return !filters.some(function (r) { return (r.value == value) });
        }

        return false;
    }

    menu.selectAllFilterItems = function (state) {
        if (this.items["selectAll"])
            this.items["selectAll"].setChecked(state);

        for (var itemName in this.items) {
            var item = this.items[itemName];
            if (item.name && item.name.indexOf("filterItem") == 0) {
                this.items[itemName].setChecked(state);
            }
        }
    }

    menu.updateSelectAllItemState = function () {
        var isChecked = true;

        for (var itemName in this.items) {
            var item = this.items[itemName];
            if (item.name && item.name.indexOf("filterItem") == 0) {
                if (!this.items[itemName].isChecked) {
                    isChecked = false;
                    break;
                }
            }
        }

        if (this.items["selectAll"])
            this.items["selectAll"].setChecked(isChecked);
    }

    menu.updateRemoveFilterItemState = function () {
        if (this.items["removeFilter"])
            this.items["removeFilter"].setEnabled(menu.getThisColumnFilters().length != 0);
    }

    menu.getFilterItemsCheckedCount = function () {
        var checkedCount = 0;

        for (var itemName in this.items) {
            var item = this.items[itemName];
            if (item.name && item.name.indexOf("filterItem") == 0 && item.isChecked) {
                checkedCount++;
            }
        }

        return checkedCount;
    }

    menu.getFilterItemsForEditors = function () {
        var items = [];

        if (this.filterItems) {
            for (var i = 0; i < this.filterItems.length; i++) {
                var filterItem = this.filterItems[i];
                if (filterItem.key && !filterItem.key.isNullValue && !filterItem.key.isBlankValue) {
                    items.push(jsObject.Item("filterItem" + i, filterItem.key.filterString, null, filterItem.key.filterString));
                }
            }
        }

        return items;
    }

    //override
    menu.addItems = function (items) {
        var itemsContainer = this.innerContent;

        if (items && items.length) {
            for (var i = 0; i < items.length; i++) {
                if (typeof (items[i]) != "string") {
                    itemsContainer.appendChild(jsObject.SortFilterMenuItem(this, items[i].name, items[i].caption, items[i].imageName, items[i].key, items[i].haveSubMenu, itemStyleName));

                    if (items[i].name == "selectAll") {
                        var filterContainer = document.createElement("div");
                        filterContainer.style.overflowY = "auto";
                        filterContainer.style.overflowX = "visible";
                        if (!jsObject.options.isMobileDevice) filterContainer.style.maxHeight = "180px";
                        filterContainer.style.borderTop = "1px dotted #c6c6c6";
                        this.innerContent.appendChild(filterContainer);
                        itemsContainer = filterContainer;
                    }
                }
                else {
                    itemsContainer.appendChild(jsObject.VerticalMenuSeparator(this, items[i]));
                }
            }
        }
    }

    return menu;
}

StiJsViewer.prototype.SortFilterMenuItem = function (menu, itemName, caption, imageName, key, haveSubMenu, itemStyleName, imageSizes) {
    var item = this.VerticalMenuItem(menu, itemName, (caption && caption.length > 40) ? caption.substring(0, 40) + "..." : caption,
        imageName || "CheckBox.png", key, itemStyleName || "stiJsViewerMenuStandartItem", haveSubMenu, imageSizes || (imageName ? null : { width: 12, height: 12 }));

    var jsObject = this;
    var imgRect = document.createElement("div");
    item.cellImage.appendChild(imgRect);
    imgRect.appendChild(item.image);
    imgRect.style.boxSizing = "border-box";
    imgRect.style.padding = "1px 0 0 1px";
    imgRect.style.boxSizing = "border-box";
    imgRect.style.width = imgRect.style.height = "20px";
    imgRect.style.border = "1px solid transparent";

    if (item.imageName == "CheckBox.png") {
        item.image.style.display = "none";
        imgRect.style.padding = "2px 0 0 2px";
    }

    item.setChecked = function (state) {
        this.isChecked = state;
        if (this.imageName == "CheckBox.png") {
            this.image.style.display = state ? "" : "none";
        }
    }

    return item;
}