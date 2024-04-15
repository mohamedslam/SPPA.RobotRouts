
StiMobileDesigner.prototype.SortFilterMenu = function (headerButton, form) {
    var menu = this.VerticalMenu("sortFilterMenu", headerButton, "Down", []);
    menu.innerContent.style.maxHeight = "1000px";
    var jsObject = this;

    menu.action = function (menuItem) {
        if (menuItem.name.indexOf("filterItem") == 0) {
            menuItem.setChecked(!menuItem.isChecked);
            menu.updateSelectAllItemState();
            this.dataTransformation.filterRules = this.getFilterRules();
            menu.updateRemoveFilterItemState();
            clearTimeout(menu.filterItemTimer);
            menu.filterItemTimer = setTimeout(function () {
                menu.dataGrid.update();
            }, 400);
            return;
        }

        switch (menuItem.name) {
            case "selectAll": {
                menu.selectAllFilterItems(!menuItem.isChecked);
                this.dataTransformation.filterRules = this.getFilterRules();
                menu.updateRemoveFilterItemState();
                this.dataGrid.update();
                return;
            }
            case "removeFilter": {
                var filterRules = menu.getThisColumnFilterRules();
                for (var i = 0; i < filterRules.length; i++) {
                    jsObject.RemoveElementFromArray(menu.dataTransformation.filterRules, filterRules[i]);
                }
                menu.selectAllFilterItems(true);
                menu.items.removeFilter.setEnabled(false);
                this.dataGrid.update();
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
                jsObject.InitializeFilterRulesForm(function (form) {
                    form.show(menu.columnObject, menu.getThisColumnFilterRules(), menu.getFilterItemsForEditors());

                    form.action = function () {
                        this.changeVisibleState(false);
                        var otherFilters = menu.getNotThisColumnFilterRules();
                        var resultFilters = form.filterControl.getFilterRules()
                        menu.dataTransformation.filterRules = otherFilters.concat(resultFilters);
                        menu.dataGrid.update();
                    }
                });
                break;
            }
            case "removeActions": {
                var actionRules = menu.getThisColumnActionRules();
                for (var i = 0; i < actionRules.length; i++) {
                    jsObject.RemoveElementFromArray(menu.dataTransformation.actionRules, actionRules[i]);
                }
                menu.dataGrid.update();
                break;
            }
            case "limitRows": {
                jsObject.InitializeLimitRowsForm(function (form) {
                    var actionsLimit = menu.getActionRules("Limit");
                    form.show(menu.columnObject, actionsLimit);

                    form.action = function () {
                        this.changeVisibleState(false);
                        for (var i = 0; i < actionsLimit.length; i++) {
                            jsObject.RemoveElementFromArray(menu.dataTransformation.actionRules, actionsLimit[i]);
                        }
                        if (parseInt(this.actionRule.startIndex) > 0 || parseInt(this.actionRule.rowsCount) >= 0) {
                            this.actionRule.key = menu.columnObject.key;
                            this.actionRule.path = menu.columnObject.path;
                            menu.dataTransformation.actionRules.push(this.actionRule);
                        }
                        menu.dataGrid.update();
                    }
                });
                break;
            }
            case "replaceValues": {
                jsObject.InitializeReplaceValuesForm(function (form) {
                    var thisColumnActionsReplace = menu.getThisColumnActionRules("Replace");
                    form.show(menu.columnObject, thisColumnActionsReplace, menu.getFilterItemsForEditors());

                    form.action = function () {
                        this.changeVisibleState(false);
                        for (var i = 0; i < thisColumnActionsReplace.length; i++) {
                            jsObject.RemoveElementFromArray(menu.dataTransformation.actionRules, thisColumnActionsReplace[i]);
                        }
                        menu.dataTransformation.actionRules = menu.dataTransformation.actionRules.concat(form.replaceControl.getReplaceRules());
                        menu.dataGrid.update();
                    }
                });
                break;
            }
            case "runningTotal": {
                jsObject.InitializeRunningTotalForm(function (form) {
                    var actionsRunningTotal = menu.getThisColumnActionRules("RunningTotal");
                    form.show(menu.columnObject, actionsRunningTotal);

                    form.action = function () {
                        this.changeVisibleState(false);
                        for (var i = 0; i < actionsRunningTotal.length; i++) {
                            jsObject.RemoveElementFromArray(menu.dataTransformation.actionRules, actionsRunningTotal[i]);
                        }
                        if (this.actionRule.initialValue) {
                            menu.dataTransformation.actionRules.push(this.actionRule);
                        }
                        menu.dataGrid.update();
                    }
                });
                break;
            }
            case "showAsPercentages": {
                menuItem.setChecked(!menuItem.isChecked);
                var actionsPercentage = menu.getThisColumnActionRules("Percentage");
                if (menuItem.isChecked) {
                    if (actionsPercentage.length == 0) {
                        menu.dataTransformation.actionRules.push(jsObject.DataActionRuleObject(menu.columnObject.key, menu.columnObject.path, "Percentage"));
                    }
                }
                else if (actionsPercentage.length > 0) {
                    jsObject.RemoveElementFromArray(menu.dataTransformation.actionRules, actionsPercentage[0]);
                }
                menu.dataGrid.update();
                break;
            }
        }

        this.changeVisibleState(false);
    }

    menu.onshow = function () {
        menu.clear();
        menu.dataTransformation = form.dataTransformation;
        menu.dataGrid = form.controls.dataGrid;

        var columnsContainer = form.controls.columnsContainer;
        var column = columnsContainer.getItemByIndex(headerButton.columnIndex);

        if (column && column.itemObject) {
            menu.columnObject = column.itemObject;

            var startItems = [];
            startItems.push(jsObject.Item("sortAsc", jsObject.ColumnIsDateType(menu.columnObject.type) ? jsObject.loc.Dashboard.SortOldestToNewest :
                (jsObject.ColumnIsNumericType(menu.columnObject.type) ? jsObject.loc.Dashboard.SortSmallestToLargest : jsObject.loc.Dashboard.SortAZ), null, "Ascending"));
            startItems.push(jsObject.Item("sortDesc", jsObject.ColumnIsDateType(menu.columnObject.type) ? jsObject.loc.Dashboard.SortNewestToOldest :
                (jsObject.ColumnIsNumericType(menu.columnObject.type) ? jsObject.loc.Dashboard.SortLargestToSmallest : jsObject.loc.Dashboard.SortZA), null, "Descending"));
            startItems.push(jsObject.Item("noSort", jsObject.loc.FormBand.NoSort, null, "None"));
            startItems.push("separatorSort");
            startItems.push(jsObject.Item("actions", jsObject.loc.FormDictionaryDesigner.Actions, null, "actions", null, true));
            startItems.push("separatorActions");

            menu.addItems(startItems);
            menu.updateItemsStates();

            //Add actions submenu
            if (menu.items["actions"]) {
                var actionsItems = [];
                actionsItems.push(jsObject.Item("limitRows", jsObject.loc.Dashboard.LimitRows, null, "limitRows"));

                if (menu.columnObject.type == "string") {
                    actionsItems.push(jsObject.Item("replaceValues", jsObject.loc.Dashboard.ReplaceValues, null, "replaceValues"));
                }

                if (jsObject.ColumnIsNumericType(menu.columnObject.type)) {
                    actionsItems.push(jsObject.Item("runningTotal", jsObject.loc.FormSystemTextEditor.RunningTotal, null, "runningTotal"));
                    actionsItems.push(jsObject.Item("showAsPercentages", jsObject.loc.Dashboard.ShowAsPercentages, null, "showAsPercentages"));
                }

                actionsItems.push("separator1");
                actionsItems.push(jsObject.Item("removeActions", jsObject.loc.Dashboard.RemoveActions.replace("&", ""), null, "removeActions"));

                var actionsMenu = jsObject.InitializeSubMenu("sortFilterMenuActions", [], menu.items["actions"], menu);

                //override
                actionsMenu.addItems = function (items) {
                    this.clear();
                    var itemsContainer = this.innerContent;

                    if (items && items.length) {
                        for (var i = 0; i < items.length; i++) {
                            if (typeof (items[i]) != "string")
                                itemsContainer.appendChild(jsObject.SortFilterMenuItem(this, items[i].name, items[i].caption, items[i].imageName, items[i].key, items[i].haveSubMenu));
                            else
                                itemsContainer.appendChild(jsObject.VerticalMenuSeparator(this, items[i]));
                        }
                    }
                }

                actionsMenu.addItems(actionsItems);

                actionsMenu.onshow = function () {
                    var actionRules = menu.getThisColumnActionRules();
                    actionsMenu.items["removeActions"].setEnabled(actionRules.length > 0);
                    actionsMenu.items["limitRows"].setChecked(actionRules.some(function (r) { return (r.type == "Limit") }));

                    if (actionsMenu.items["replaceValues"]) {
                        actionsMenu.items["replaceValues"].setChecked(actionRules.some(function (r) { return (r.type == "Replace") }));
                    }
                    if (actionsMenu.items["runningTotal"]) {
                        actionsMenu.items["runningTotal"].setChecked(actionRules.some(function (r) { return (r.type == "RunningTotal") }));
                    }
                    if (actionsMenu.items["showAsPercentages"]) {
                        actionsMenu.items["showAsPercentages"].setChecked(actionRules.some(function (r) { return (r.type == "Percentage") }));
                    }
                }
            }

            var fContainer = document.createElement("div");
            fContainer.style.height = "250px";
            fContainer.style.minWidth = "100%";
            fContainer.style.position = "relative";
            menu.innerContent.appendChild(fContainer);
            jsObject.AddProgressToControl(fContainer);
            fContainer.progress.show();

            setTimeout(function () {
                jsObject.SendCommandToDesignerServer("ExecuteCommandForDataTransformation",
                    {
                        parameters: {
                            command: "GetFilteredItems",
                            columns: columnsContainer.getColumns(),
                            columnIndex: headerButton.columnIndex,
                            sortRules: form.dataTransformation.sortRules,
                            filterRules: form.dataTransformation.filterRules,
                            actionRules: form.dataTransformation.actionRules,
                            elementName: form.elementName,
                            dataTransformationCacheGuid: form.dataTransformationCacheGuid
                        }
                    },
                    function (answer) {
                        fContainer.progress.hide();
                        menu.innerContent.removeChild(fContainer);

                        var filterItemsHelper = answer.filterItemsHelper;
                        if (!filterItemsHelper) return;
                        var items = [];
                        var filterItems = [];

                        menu.columnObject.path = filterItemsHelper.columnPath;

                        for (var i = 0; i < filterItemsHelper.mainItems.length; i++) {
                            var filterItem = filterItemsHelper.mainItems[i];
                            filterItems.push(jsObject.Item("filterItem" + i, filterItem.displayString, null, { filterString: filterItem.filterString }));
                        }

                        if (filterItemsHelper.haveNulls)
                            filterItems.push(jsObject.Item("filterItemIsNull", jsObject.loc.Dashboard.Nulls, null, { isNullValue: true, filterString: null }));

                        if (filterItemsHelper.haveBlanks)
                            filterItems.push(jsObject.Item("filterItemIsBlank", jsObject.loc.Dashboard.Blanks, null, { isBlankValue: true, filterString: "" }));

                        menu.filterItems = filterItems

                        if (filterItems.length > 0) {
                            if (jsObject.ColumnIsNumericType(menu.columnObject.type)) {
                                items.push(jsObject.Item("numberFilters", jsObject.loc.Dashboard.NumberFilters, null, "numberFilters", null, true));
                            }
                            else if (jsObject.ColumnIsDateType(menu.columnObject.type)) {
                                items.push(jsObject.Item("dateFilters", jsObject.loc.Dashboard.DateFilters, null, "dateFilters", null, true));
                            }
                            else if (menu.columnObject.type == "bool") {
                                items.push(jsObject.Item("booleanFilters", jsObject.loc.Dashboard.BooleanFilters, null, "booleanFilters", null, true));
                            }
                            else {
                                items.push(jsObject.Item("stringFilters", jsObject.loc.Dashboard.StringFilters, null, "stringFilters", null, true));
                            }
                            items.push(jsObject.Item("customFilter", jsObject.loc.Dashboard.CustomFilter.replace("&", "").replace("...", ""), null, "customFilter", null));
                            items.push(jsObject.Item("removeFilter", jsObject.loc.FormBand.RemoveFilter.replace("&", ""), null, "removeFilter", null));
                            items.push("separatorFilter");

                            items.push(jsObject.Item("selectAll", jsObject.loc.Dashboard.SelectAll.replace("&", ""), null, "selectAll"));

                            items = items.concat(filterItems);
                        }

                        menu.addItems(items);
                        menu.updateItemsStates();

                        //Add filter submenus
                        if (menu.items["numberFilters"]) {
                            var numberFiltersMenu = jsObject.InitializeSubMenu("sortFilterMenuNumberFilters", jsObject.GetFilterConditionItems("Numeric", false), menu.items["numberFilters"], menu);
                            numberFiltersMenu.action = function (menuItem) {
                                this.changeVisibleState(false);
                                menu.filterSubMenuAction(menuItem.key);
                            }
                        }

                        if (menu.items["dateFilters"]) {
                            var dateFiltersMenu = jsObject.InitializeSubMenu("sortFilterMenuDateFilters", jsObject.GetFilterConditionItems("DateTime", false), menu.items["dateFilters"], menu);
                            dateFiltersMenu.action = function (menuItem) {
                                this.changeVisibleState(false);
                                menu.filterSubMenuAction(menuItem.key);
                            }
                        }

                        if (menu.items["booleanFilters"]) {
                            var booleanFiltersMenu = jsObject.InitializeSubMenu("sortFilterMenuBooleanFilters", jsObject.GetFilterConditionItems("Boolean", false), menu.items["booleanFilters"], menu);
                            booleanFiltersMenu.action = function (menuItem) {
                                this.changeVisibleState(false);
                                menu.filterSubMenuAction(menuItem.key);
                            }
                        }

                        if (menu.items["stringFilters"]) {
                            var stringFiltersMenu = jsObject.InitializeSubMenu("sortFilterMenuStringFilters", jsObject.GetFilterConditionItems("String", false, true), menu.items["stringFilters"], menu);
                            stringFiltersMenu.action = function (menuItem) {
                                this.changeVisibleState(false);
                                menu.filterSubMenuAction(menuItem.key);
                            }
                        }
                    });
            }, this.jsObject.options.menuAnimDuration);
        }
    }

    menu.onhide = function () {
        if (this.dataGrid && this.dataGrid.selectedHeaderButton) {
            this.dataGrid.selectedHeaderButton.setSelected(false);
            this.dataGrid.selectedHeaderButton = null;
        }
    }

    menu.updateItemsStates = function () {
        if (this.items["actions"]) {
            this.items["actions"].setChecked(this.getThisColumnActionRules().length > 0);
        }

        var sortRule = this.getThisColumnSortRule();
        var sortDirection = sortRule ? sortRule.direction : "None";
        this.items["sortAsc"].setChecked(sortDirection == "Ascending");
        this.items["sortDesc"].setChecked(sortDirection == "Descending");
        this.items["noSort"].setChecked(sortDirection == "None");

        if (this.items["customFilter"]) {
            var filterRules = menu.getThisColumnFilterRules(this.dataTransformation, this.columnObject);
            var filterType = menu.getDataFilterConditionGroupType(filterRules);

            if (filterRules.length == 0) {
                this.selectAllFilterItems(true);
            }
            else {
                for (var itemName in this.items) {
                    var item = this.items[itemName];
                    if (item.name && item.name.indexOf("filterItem") == 0) {
                        var filterString = this.items[itemName].key != null ? this.items[itemName].key.filterString : null;
                        this.items[itemName].setChecked(this.checkCondition(filterType, filterString, filterRules));
                    }
                }
            }

            this.updateSelectAllItemState();
            this.updateRemoveFilterItemState();
        }
    }

    menu.filterSubMenuAction = function (itemKey) {
        this.changeVisibleState(false);

        jsObject.InitializeFilterRulesForm(function (form) {
            form.show(menu.columnObject, [jsObject.DataFilterRuleObject(menu.columnObject.key, menu.columnObject.path, itemKey)], menu.getFilterItemsForEditors());

            form.action = function () {
                this.changeVisibleState(false);
                var filters = menu.getNotThisColumnFilterRules();
                var resultFilters = form.filterControl.getFilterRules()
                menu.dataTransformation.filterRules = filters.concat(resultFilters);
                menu.dataGrid.update();
            }
        });
    }

    menu.setSortDirection = function (direction) {
        var currentRule = this.getThisColumnSortRule();
        var columnKey = this.columnObject.key;

        if (currentRule == null && direction != "None") {
            this.dataTransformation.sortRules.push(jsObject.DataSortRuleObject(columnKey, direction));
        }
        else {
            if (direction == "None") {
                jsObject.RemoveElementFromArray(this.dataTransformation.sortRules, currentRule);
            }
            else {
                currentRule.direction = direction;
            }
        }
        this.dataGrid.update();
    }

    menu.getFilterRules = function () {
        var filters = this.getNotThisColumnFilterRules();

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
                        filter = jsObject.DataFilterRuleObject(columnKey, columnPath, "IsNull");
                    else if (itemKey.isBlankValue)
                        filter = jsObject.DataFilterRuleObject(columnKey, columnPath, "IsBlank");
                    else
                        filter = jsObject.DataFilterRuleObject(columnKey, columnPath, "EqualTo", filterString);

                    filters.push(filter);
                }
                else if (!item.isChecked && conditionGroupType == "NotEqual") {
                    if (itemKey.isNullValue)
                        filter = jsObject.DataFilterRuleObject(columnKey, columnPath, "IsNotNull");
                    else if (itemKey.isBlankValue)
                        filter = jsObject.DataFilterRuleObject(columnKey, columnPath, "IsNotBlank");
                    else
                        filter = jsObject.DataFilterRuleObject(columnKey, columnPath, "NotEqualTo", filterString);

                    filters.push(filter);
                }
            }
        }

        return filters;
    }

    menu.getThisColumnSortRule = function () {
        var dataTransformation = this.dataTransformation;
        for (var i = 0; i < dataTransformation.sortRules.length; i++) {
            if (dataTransformation.sortRules[i].key == this.columnObject.key) {
                return dataTransformation.sortRules[i];
            }
        }
        return null;
    }

    menu.getThisColumnFilterRules = function () {
        var dataTransformation = this.dataTransformation;
        var filterRules = [];
        for (var i = 0; i < dataTransformation.filterRules.length; i++) {
            var filterRule = dataTransformation.filterRules[i];

            if (filterRule.key == this.columnObject.key || (!filterRule.key && filterRule.path == this.columnObject.path)) {
                filterRules.push(filterRule);
            }
        }
        return filterRules;
    }

    menu.getNotThisColumnFilterRules = function () {
        var dataTransformation = this.dataTransformation;
        var filterRules = [];
        for (var i = 0; i < dataTransformation.filterRules.length; i++) {
            var filterRule = dataTransformation.filterRules[i];
            if (filterRule.key == this.columnObject.key || (!filterRule.key && filterRule.path == this.columnObject.path)) {
                continue;
            }
            else {
                filterRules.push(filterRule);
            }
        }
        return filterRules;
    }

    menu.getThisColumnActionRules = function (actionType) {
        var dataTransformation = this.dataTransformation;
        var actionRules = [];
        for (var i = 0; i < dataTransformation.actionRules.length; i++) {
            var actionRule = dataTransformation.actionRules[i];

            if (actionRule.key == this.columnObject.key || (!actionRule.key && actionRule.path == this.columnObject.path)) {
                if (actionType) {
                    if (actionType == actionRule.type) {
                        actionRules.push(actionRule);
                    }
                }
                else {
                    actionRules.push(actionRule);
                }
            }
        }
        return actionRules;
    }

    menu.getActionRules = function (actionType) {
        var dataTransformation = this.dataTransformation;
        var actionRules = [];
        for (var i = 0; i < dataTransformation.actionRules.length; i++) {
            var actionRule = dataTransformation.actionRules[i];

            if (actionType) {
                if (actionType == actionRule.type) {
                    actionRules.push(actionRule);
                }
            }
            else {
                actionRules.push(actionRule);
            }
        }

        return actionRules;
    }

    menu.getNotThisColumnActionRules = function () {
        var dataTransformation = this.dataTransformation;
        var actionRules = [];
        for (var i = 0; i < dataTransformation.actionRules.length; i++) {
            var actionRule = dataTransformation.actionRules[i];

            if (!(actionRule.key == this.columnObject.key || (!actionRule.key && actionRule.path == this.columnObject.path))) {
                actionRules.push(dataTransformation.actionRules[i]);
            }
        }
        return actionRules;
    }

    menu.getDataFilterConditionGroupType = function (filterRules) {

        if (filterRules.length == 0) {
            return "Empty";
        }

        if (filterRules.every(function (r) { return (r.condition == "EqualTo" || r.condition == "IsBlank" || r.condition == "IsNull") })) {
            return "Equal";
        };

        if (filterRules.every(function (r) { return (r.condition == "NotEqualTo" || r.condition == "IsNotBlank" || r.condition == "IsNotNull") })) {
            return "NotEqual";
        };

        return "Custom";
    }

    menu.checkCondition = function (filterType, value, filterRules) {

        if (filterType == "Equal") {
            if (value == null)
                return filterRules.some(function (r) { return (r.condition == "IsNull") });

            if (value == "")
                return filterRules.some(function (r) { return (r.condition == "IsBlank") });

            return filterRules.some(function (r) { return (r.value == value) });
        }

        if (filterType == "NotEqual") {
            if (value == null)
                return !filterRules.some(function (r) { return (r.condition == "IsNotNull") });

            if (value == "")
                return !filterRules.some(function (r) { return (r.condition == "IsNotBlank") });

            return !filterRules.some(function (r) { return (r.value == value) });
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
            this.items["removeFilter"].setEnabled(menu.getThisColumnFilterRules().length != 0);
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
                    itemsContainer.appendChild(jsObject.SortFilterMenuItem(this, items[i].name, items[i].caption, items[i].imageName, items[i].key, items[i].haveSubMenu));

                    if (items[i].name == "selectAll") {
                        var filterContainer = document.createElement("div");
                        filterContainer.style.overflowY = "auto";
                        filterContainer.style.overflowX = "visible";
                        filterContainer.style.maxHeight = "250px";
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

StiMobileDesigner.prototype.SortFilterMenuItem = function (menu, itemName, caption, imageName, key, haveSubMenu, imageSizes) {
    var item = this.VerticalMenuItem(menu, itemName, (caption && caption.length > 40) ? caption.substring(0, 40) + "..." : caption, imageName || "CheckBox.png", key, null, haveSubMenu, imageSizes || (imageName ? null : { width: 12, height: 12 }));

    var imgRect = document.createElement("div");
    item.cellImage.appendChild(imgRect);
    imgRect.style.boxSizing = "border-box";
    imgRect.style.padding = "1px 0 0 1px";
    imgRect.appendChild(item.image);
    imgRect.style.width = imgRect.style.height = "20px";
    imgRect.style.border = "1px solid transparent";

    if (item.imageName == "CheckBox.png") {
        item.image.style.display = "none";
        imgRect.style.padding = "2px 0 0 4px";
    }

    item.setChecked = function (state) {
        this.isChecked = state;
        if (this.imageName == "CheckBox.png") {
            this.image.style.display = state ? "" : "none";
        }
    }

    return item;
}