
StiJsViewer.prototype.CreateTreeViewBoxElementContent = function (element) {
    var jsObject = this;
    var elementAttrs = element.elementAttributes;
    var contentAttrs = elementAttrs.contentAttributes;

    var mainButton = this.SmallButton(null, " ", null, null, "Down", null, contentAttrs.settings);
    jsObject.ApplyAttributesToObject(mainButton.innerTable, elementAttrs);
    mainButton.innerTable.style.minHeight = "auto";
    mainButton.style.height = mainButton.innerTable.style.height = "100%";
    mainButton.style.border = "0";
    mainButton.style.overflow = "hidden";
    mainButton.caption.style.width = "100%";
    mainButton.caption.style.padding = "0 4px 0 4px";
    mainButton.arrow.style.width = mainButton.arrow.style.height = "16px";
    StiJsViewer.setImageSource(mainButton.arrow, this.options, this.collections, "Dashboards.IconCloseItem" + (contentAttrs.settings.isDarkStyle ? "White.png" : ".png"));

    var corners = elementAttrs.cornerRadius;
    if (corners) {
        mainButton.style.borderRadius = parseInt(corners.topLeft) + "px " + parseInt(corners.topRight) + "px " + parseInt(corners.bottomRight) + "px " + parseInt(corners.bottomLeft) + "px";
    }

    element.contentPanel.appendChild(mainButton);
    element.contentPanel.style.overflow = "hidden";

    var menuName = elementAttrs.name + "TreeViewBoxMenu";
    var menu = this.controls.menus && this.controls.menus[menuName] ? this.controls.menus[menuName] : this.VerticalMenu(menuName, mainButton, "Down");
    menu.parentButton = mainButton;
    element.menu = menu;

    mainButton.action = function () {
        menu.changeVisibleState(!menu.visible);
    }

    menu.onshow = function () {
        menu.isModified = false;
    }

    menu.onHide = function () {
        if (menu.isModified) {
            jsObject.ApplyFiltersToDashboardElement(element, element.getFilters(), true);
        }
    }

    var parentPanel = menu.innerContent;
    parentPanel.style.width = mainButton.offsetWidth + "px";
    parentPanel.style.backgroundColor = jsObject.excludeOpacity(elementAttrs.backColor);
    parentPanel.style.height = !this.options.isMobileDevice ? "300px" : "100%";
    parentPanel.style.padding = "4px";

    var actionFunction = function () {
        if (contentAttrs.selectionMode == "One") {
            menu.changeVisibleState(false);
        }
        mainButton.update();
    }

    jsObject.CreateTreeViewItemsContent(element, parentPanel, actionFunction);

    var tree = element.itemsPanel;
    tree.style.left = tree.style.right = tree.style.bottom = "4px";
    tree.style.top = (contentAttrs.showAllValue && contentAttrs.selectionMode == "Multi" ? elementAttrs.contentAttributes.settings.itemHeight + 9 : 4) + "px";

    mainButton.update = function () {
        if (contentAttrs.selectionMode == "One") {
            for (var itemKey in tree.items) {
                var item = tree.items[itemKey];
                if (item.isSelected) {
                    mainButton.caption.innerText = item.button.captionCell.innerText;
                }
            }
        }
        else {
            var onRemoveButton = function (button) {
                if (button.item.value == "StiUnCheckAll") {
                    element.setStatesForAllItems(false);
                    jsObject.ApplyFiltersToDashboardElement(element, element.getFilters());
                }
                else {
                    button.item.setChecked(false);
                    tree.onChecked(button.item);
                }
            }

            var checkItems = [];
            var unCheckItems = [];

            var items = tree.getItemsLastLevel();

            for (var i = 0; i < items.length; i++) {
                if (items[i].isChecked)
                    checkItems.push(items[i]);
                else
                    unCheckItems.push(items[i]);
            }

            mainButton.caption.innerText = "";
            var captionWidth = mainButton.caption.offsetWidth;

            if (unCheckItems.length == 0 && contentAttrs.showAllValue) {
                mainButton.caption.innerText = "";
                mainButton.caption.appendChild(jsObject.ComboBoxCaptionButton(jsObject.collections.loc.All, { value: "StiUnCheckAll" }, onRemoveButton, contentAttrs.settings, elementAttrs));
            }
            else if (checkItems.length > 0 || (unCheckItems.length == 0 && !contentAttrs.showAllValue)) {
                var captionNotFit = false;

                for (var i = 0; i < checkItems.length; i++) {
                    mainButton.caption.appendChild(jsObject.ComboBoxCaptionButton(checkItems[i].button.captionCell.innerText, checkItems[i], onRemoveButton, contentAttrs.settings, elementAttrs));

                    if (mainButton.caption.offsetWidth > captionWidth) {
                        mainButton.caption.innerText = "";
                        captionNotFit = true;
                        break;
                    }
                }
                if (captionNotFit) {
                    mainButton.caption.appendChild(jsObject.ComboBoxCaptionButton(jsObject.collections.loc.DashboardNSelected.replace("{0}", checkItems.length),
                        { value: "StiUnCheckAll" }, onRemoveButton, contentAttrs.settings, elementAttrs));
                }
            }
        }

        mainButton.setEnabled(jsObject.getCountObjects(tree.mainItem.childs));
    }

    mainButton.update();
}