
StiJsViewer.prototype.CreateListBoxElementContent = function (element) {
    var jsObject = this;

    var itemsPanel = document.createElement("div");
    itemsPanel.style.position = "absolute";
    itemsPanel.style.left = itemsPanel.style.top = itemsPanel.style.right = itemsPanel.style.bottom = "0px";
    itemsPanel.style.overflow = "auto";
    itemsPanel.className = "stiJsViewerScrollContainer";

    jsObject.CreateListBoxItemsContent(element, itemsPanel, element.contentPanel);
}

StiJsViewer.prototype.CreateListBoxItemsContent = function (element, itemsPanel, parentPanel, actionFunction) {
    var jsObject = this;
    var elementAttrs = element.elementAttributes;
    var contentAttrs = elementAttrs.contentAttributes;
    var itemAllValue;
    var itemsCollection = [];
    element.itemsPanel = itemsPanel;

    while (parentPanel.childNodes[0]) {
        parentPanel.removeChild(parentPanel.childNodes[0]);
    }

    //Helper methods
    element.setStatesForAllItems = function (state) {
        if (contentAttrs.selectionMode == "Multi" && itemAllValue) {
            itemAllValue.setChecked(state);
        }
        for (var i = 0; i < itemsCollection.length; i++) {
            var item = itemsCollection[i];

            //Mode Multi
            if (item["setChecked"]) {
                item.setChecked(state);
            }
            else if (item["setSelected"]) {
                item.setSelected(state);
            }
        }
    }

    element.applyFiltersToItems = function () {
        for (var i = 0; i < itemsCollection.length; i++) {
            var item = itemsCollection[i];

            //Mode Multi
            if (item["setChecked"]) {
                var state = contentAttrs.filters[0].condition == "NotEqualTo" || contentAttrs.filters[0].condition == "IsFalse";

                if (state && contentAttrs.filters.some(function (f) { return ((f.condition == "NotEqualTo" && f.value == item.value) || f.condition == "IsFalse") })) {
                    item.setChecked(false);
                }
                else if (!state && contentAttrs.filters.some(function (f) { return (f.condition == "EqualTo" && f.value == item.value) })) {
                    item.setChecked(true);
                }
                else {
                    item.setChecked(state);
                }

                if (itemAllValue && !item.isChecked) {
                    itemAllValue.setChecked(false);
                }
            }
            //Mode One
            else if (item["setSelected"] && contentAttrs.filters.some(function (f) { return (f.condition == "EqualTo" && f.value == item.value) })) {
                item.setSelected(true);
                return;
            }
        }
    }

    element.getAllItemsCheckedState = function () {
        var states = {
            checkedItems: [],
            unCheckedItems: []
        };

        for (var i = 0; i < itemsCollection.length; i++) {
            var item = itemsCollection[i];
            if (item["setChecked"] && !item.isAllValue) {
                states[item.isChecked ? "checkedItems" : "unCheckedItems"].push(item);
            }
        }

        return states;
    }

    element.getFilters = function () {
        var filters = [];
        var states = element.getAllItemsCheckedState();

        if (states.checkedItems.length == 0) {
            filters.push({ condition: "IsFalse", path: contentAttrs.columnPath });
        }
        else if (states.unCheckedItems.length > states.checkedItems.length) {
            for (var i = 0; i < states.checkedItems.length; i++) {
                filters.push({ condition: "EqualTo", value: states.checkedItems[i].value, path: contentAttrs.columnPath });
            }
        }
        else {
            for (var i = 0; i < states.unCheckedItems.length; i++) {
                filters.push({ condition: "NotEqualTo", value: states.unCheckedItems[i].value, path: contentAttrs.columnPath });
            }
        }
        return filters;
    }

    element.itemAction = function (item) {
        if (contentAttrs.selectionMode == "Multi") {
            if (item.isAllValue) {
                element.setStatesForAllItems(item.isChecked);
            }
            else if (itemAllValue) {
                var itemsStates = element.getAllItemsCheckedState();
                itemAllValue.setChecked(itemsStates.unCheckedItems.length == 0);
            }

            if (element.menu && element.menu.visible) {
                element.menu.isModified = true;
            }
            else {
                clearTimeout(element.actionTimer);
                element.actionTimer = setTimeout(function () { jsObject.ApplyFiltersToDashboardElement(element, element.getFilters()); }, 500);
            }
        }
        else {
            element.setStatesForAllItems(false);
            item.setSelected(true);
            var filters = !item.isAllValue ? [{ condition: "EqualTo", value: item.value, path: contentAttrs.columnPath }] : [];
            jsObject.ApplyFiltersToDashboardElement(element, filters);
        }

        if (actionFunction) actionFunction();
    }

    if (contentAttrs.items) {
        //Horizontal orientation
        if (contentAttrs.orientation == "Horizontal") {
            parentPanel.horTable = this.CreateHTMLTable();
            parentPanel.horTable.style.height = "100%";
            parentPanel.appendChild(parentPanel.horTable);
            parentPanel.style.overflowX = "auto";
            parentPanel.style.overflowY = "hidden";
        }
        else {
            parentPanel.appendChild(itemsPanel);
        }

        if (contentAttrs.selectionMode == "Multi") {
            //Mode Multi
            if (contentAttrs.showAllValue) {
                itemAllValue = this.ListBoxElementCheckBox(this.collections.loc.DashboardAllValue, elementAttrs);
                itemAllValue.isAllValue = true;
                parentPanel.itemAllValue = itemAllValue;
                jsObject.ApplyAttributesToObject(itemAllValue.button.innerTable, elementAttrs);

                if (contentAttrs.orientation == "Horizontal") {
                    parentPanel.horTable.addCell(itemAllValue);
                    parentPanel.horTable.addCell(jsObject.ListBoxElementHorSeparator(elementAttrs));
                }
                else {
                    parentPanel.appendChild(itemAllValue);
                    parentPanel.appendChild(jsObject.ListBoxElementSeparator(elementAttrs));
                    parentPanel.appendChild(itemsPanel);
                    itemsPanel.style.top = (itemAllValue.offsetHeight + 3) + "px";
                }

                itemAllValue.action = function () {
                    element.itemAction(this);
                }
            }

            for (var i = 0; i < contentAttrs.items.length; i++) {
                var itemObj = contentAttrs.items[i];
                var item = this.ListBoxElementCheckBox(contentAttrs.items[i].label || (itemObj.value ? itemObj.value.toString() : ""), elementAttrs);
                item.value = itemObj.value;
                itemsCollection.push(item);

                jsObject.ApplyAttributesToObject(item.button.innerTable, elementAttrs);

                if (contentAttrs.orientation == "Horizontal") {
                    parentPanel.horTable.addCell(item);
                }
                else {
                    itemsPanel.appendChild(item);
                }

                item.action = function () {
                    element.itemAction(this);
                }
            }
        }
        else {
            //Mode One
            if (contentAttrs.showAllValue) {
                itemAllValue = this.ListBoxElementButton(this.collections.loc.DashboardAllValue, elementAttrs);
                itemAllValue.isAllValue = true;
                itemsCollection.push(itemAllValue);

                if (contentAttrs.orientation == "Horizontal") {
                    parentPanel.horTable.addCell(itemAllValue);
                    parentPanel.horTable.addCell(jsObject.ListBoxElementHorSeparator(elementAttrs));
                }
                else {
                    itemsPanel.appendChild(itemAllValue);
                    itemsPanel.appendChild(jsObject.ListBoxElementSeparator(elementAttrs));
                }

                itemAllValue.action = function () {
                    element.itemAction(this);
                }
            }

            for (var i = 0; i < contentAttrs.items.length; i++) {
                var itemObj = contentAttrs.items[i];
                var item = this.ListBoxElementButton(contentAttrs.items[i].label || (itemObj.value ? itemObj.value.toString() : ""), elementAttrs);
                item.value = itemObj.value;
                itemsCollection.push(item);

                if (contentAttrs.orientation == "Horizontal") {
                    parentPanel.horTable.addCell(item);
                }
                else {
                    itemsPanel.appendChild(item);
                }

                item.action = function () {
                    element.itemAction(this);
                }
            }
        }

        element.setStatesForAllItems(contentAttrs.selectionMode == "Multi");

        if (contentAttrs.filters.length > 0) {
            element.applyFiltersToItems();
        }
        else if (itemAllValue && contentAttrs.selectionMode == "One") {
            itemAllValue.setSelected(true);
        }

        if (contentAttrs.orientation == "Horizontal") {
            var itemsTotalWidth = 0;
            for (var i = 0; i < itemsCollection.length; i++) {
                itemsTotalWidth += itemsCollection[i].offsetWidth;
                var itemButton = itemsCollection[i].button || itemsCollection[i];
                itemButton.style.height = (parentPanel.offsetHeight - 2) + "px";
                itemButton.innerTable.style.height = "100%";
            }
            if (parentPanel.itemAllValue) {
                parentPanel.itemAllValue.button.style.height = (parentPanel.offsetHeight - 2) + "px";
                parentPanel.itemAllValue.button.innerTable.style.height = "100%";
            }
            var widthItemAll = (contentAttrs.showAllValue ? 5 : 0) + (parentPanel.itemAllValue ? parentPanel.itemAllValue.offsetWidth : 0);
            var contentWidth = itemsTotalWidth + widthItemAll;
            if (parentPanel.offsetWidth > 0 && contentWidth < parentPanel.offsetWidth) {
                parentPanel.style.overflowX = "hidden";
                var newWidth = (parentPanel.offsetWidth - widthItemAll) / itemsCollection.length;
                for (var i = 0; i < itemsCollection.length; i++) {
                    var itemButton = itemsCollection[i].button || itemsCollection[i];
                    itemButton.style.width = (newWidth - 2) + "px";
                    itemButton.innerTable.style.width = "100%";
                    if (itemButton.caption) {
                        itemButton.caption.style.textAlign = "center";
                        itemButton.caption.style.verticalAlign = "middle";
                    }
                }
            }
        }

        if (contentAttrs.items.length > 10 && ((elementAttrs.type == "StiListBoxElement" && contentAttrs.orientation == "Vertical") || elementAttrs.type == "StiComboBoxElement")) {
            var findTextbox = this.TextBox(null, 228);
            findTextbox.setAttribute("placeholder", this.collections.loc.TypeToSearch);
            itemsPanel.style.top = (parseInt(itemsPanel.style.top) + (this.options.isTouchDevice ? 33 : 28)) + "px"
            findTextbox.style.width = "calc(100% - 8px)";

            if (contentAttrs.settings.foreColor && contentAttrs.settings.backColor) {
                findTextbox.style.color = contentAttrs.settings.foreColor;
                findTextbox.style.background = contentAttrs.settings.backColor;
            }
            element.findTextbox = findTextbox;

            if (contentAttrs.selectionMode == "Multi" || (elementAttrs.type == "StiComboBoxElement" && contentAttrs.selectionMode == "One")) {
                findTextbox.style.margin = "5px";
                findTextbox.style.width = "calc(100% - 18px)";
                itemsPanel.style.top = (parseInt(itemsPanel.style.top) + 6) + "px";
            }

            if (parentPanel.childNodes.length > 0) {
                parentPanel.insertBefore(findTextbox, parentPanel.childNodes[0]);
            }
            else {
                parentPanel.appendChild(findTextbox);
            }

            findTextbox.onChange = function () {
                for (var i = 0; i < itemsPanel.childNodes.length; i++) {
                    var item = itemsPanel.childNodes[i];
                    var caption = item.button ? item.button.caption : item.caption;
                    item.style.display = (!this.value || (caption && caption.innerText.toLowerCase().indexOf(this.value.toLowerCase()) >= 0)) ? "" : "none";
                }
            }
        }
    }
}

StiJsViewer.prototype.ListBoxElementButton = function (caption, elementAttrs) {
    var button = this.SmallButton(null, caption, null, null, null, null, elementAttrs.contentAttributes.settings);
    button.style.height = "auto";

    this.ApplyAttributesToObject(button.innerTable, elementAttrs);

    return button;
}

StiJsViewer.prototype.ListBoxElementCheckBox = function (caption, elementAttrs) {
    var checkItem = document.createElement("div");
    checkItem.isChecked = false;

    var button = this.SmallButton(null, caption, "CheckBox.png", null, null, null, elementAttrs.contentAttributes.settings, { width: 12, height: 12 });
    button.style.height = "auto";
    checkItem.appendChild(button);
    checkItem.button = button;

    var checkBox = this.CheckBox(null, null, null, elementAttrs.contentAttributes.settings);
    checkBox.onmouseenter = null;
    checkBox.onmouseleave = null;
    checkBox.onmouseover = null;
    checkBox.onmouseout = null;
    checkBox.action = function () { };
    checkBox.style.margin = "0 3px 0 3px";
    checkBox.image.parentNode.style.fontSize = "0px";

    if (elementAttrs && elementAttrs.foreColor) {
        checkBox.imageBlock.style.borderColor = elementAttrs.foreColor;
        checkBox.imageBlock.style.background = "transparent";
    }

    button.imageCell.removeChild(button.image);
    button.imageCell.appendChild(checkBox);
    button.imageCell.style.width = "1px";

    checkItem.setChecked = function (state) {
        this.isChecked = state;
        checkBox.setChecked(state);
    }

    button.action = function () {
        checkItem.setChecked(!checkItem.isChecked);
        checkItem.action();
    }

    checkItem.action = function () { }

    return checkItem;
}

StiJsViewer.prototype.ApplyAttributesToObject = function (obj, elementAttrs) {
    if (elementAttrs.contentAttributes.settings) {
        obj.style.minHeight = elementAttrs.contentAttributes.settings.itemHeight + "px";
        obj.style.height = "0px"; //fix a bug in the Firefox
    }

    if (elementAttrs.font) {
        obj.style.fontFamily = elementAttrs.font.name;
        obj.style.fontSize = elementAttrs.font.size + "pt";
        if (elementAttrs.font.bold) obj.style.fontWeight = "bold";
        if (elementAttrs.font.italic) obj.style.fontStyle = "italic";
        if (elementAttrs.font.underline) obj.style.textDecoration = "underline";
    }
}

StiJsViewer.prototype.ListBoxElementSeparator = function (elementAttrs) {
    var sep = document.createElement("div");
    sep.style.height = "1px";
    sep.style.margin = "1px";
    sep.style.background = elementAttrs.contentAttributes.settings.separatorColor;

    return sep;
}

StiJsViewer.prototype.ListBoxElementHorSeparator = function (elementAttrs) {
    var sep = document.createElement("div");
    sep.style.height = "100%";
    sep.style.width = "1px";
    sep.style.margin = "5px 2px 5px 2px";
    sep.style.background = elementAttrs.contentAttributes.settings.separatorColor;

    return sep;
}