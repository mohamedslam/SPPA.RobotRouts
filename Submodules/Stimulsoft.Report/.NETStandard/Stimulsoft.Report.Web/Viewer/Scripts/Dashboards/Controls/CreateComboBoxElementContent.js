
StiJsViewer.prototype.CreateComboBoxElementContent = function (element) {
    var jsObject = this;
    var elementAttrs = element.elementAttributes;
    var contentAttrs = elementAttrs.contentAttributes;

    var mainButton = this.SmallButton(null, " ", null, null, "Down", null, contentAttrs.settings);
    jsObject.ApplyAttributesToObject(mainButton.innerTable, elementAttrs);
    mainButton.innerTable.style.minHeight = "auto";
    mainButton.style.height = mainButton.innerTable.style.height = "100%";
    mainButton.style.borderRadius = "0";
    mainButton.style.border = "0";
    mainButton.style.overflow = "hidden";
    mainButton.caption.style.width = "100%";
    mainButton.caption.style.padding = "0 4px 0 4px";
    mainButton.arrow.style.width = mainButton.arrow.style.height = "16px";
    StiJsViewer.setImageSource(mainButton.arrow, this.options, this.collections, "Dashboards.IconCloseItem" + (contentAttrs.settings.isDarkStyle ? "White.png" : ".png"));

    element.contentPanel.appendChild(mainButton);
    element.contentPanel.style.overflow = "hidden";

    var corners = elementAttrs.cornerRadius;
    if (corners) {
        mainButton.style.borderRadius = parseInt(corners.topLeft) + "px " + parseInt(corners.topRight) + "px " + parseInt(corners.bottomRight) + "px " + parseInt(corners.bottomLeft) + "px";
    }

    var menuName = elementAttrs.name + "ComboBoxMenu";
    var menu = this.controls.menus && this.controls.menus[menuName] ? this.controls.menus[menuName] : this.VerticalMenu(menuName, mainButton, "Down");
    menu.parentButton = mainButton;
    element.menu = menu;

    mainButton.action = function () {
        menu.changeVisibleState(!menu.visible);
    }

    menu.onshow = function () {
        menu.isModified = false;
        if (element.findTextbox) element.findTextbox.focus();
    }

    menu.onHide = function () {
        if (menu.isModified) {
            jsObject.ApplyFiltersToDashboardElement(element, element.getFilters(), true);
        }
    }

    var itemsPanel = document.createElement("div");
    itemsPanel.style.left = itemsPanel.style.top = itemsPanel.style.right = itemsPanel.style.bottom = "0px";
    itemsPanel.style.overflow = "auto";

    var parentPanel = menu.innerContent;
    parentPanel.style.width = mainButton.offsetWidth + "px";
    parentPanel.style.backgroundColor = jsObject.excludeOpacity(elementAttrs.backColor);

    if (contentAttrs.showAllValue && contentAttrs.selectionMode == "Multi") {
        parentPanel.style.overflow = this.options.isMobileDevice ? "auto" : "hidden";
        if (!this.options.isMobileDevice) itemsPanel.style.maxHeight = "350px";
    }

    mainButton.update = function () {
        if (contentAttrs.selectionMode == "One") {
            for (var i = 0; i < itemsPanel.childNodes.length; i++) {
                if (itemsPanel.childNodes[i].isSelected) {
                    mainButton.caption.innerText = itemsPanel.childNodes[i].caption.innerText;
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
                    button.item.action();
                }
            }

            var checkItems = [];
            var unCheckItems = [];

            for (var i = 0; i < itemsPanel.childNodes.length; i++) {
                if (itemsPanel.childNodes[i].button != null) {
                    if (itemsPanel.childNodes[i].isChecked)
                        checkItems.push(itemsPanel.childNodes[i]);
                    else
                        unCheckItems.push(itemsPanel.childNodes[i]);
                }
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
                    mainButton.caption.appendChild(jsObject.ComboBoxCaptionButton(checkItems[i].button.caption.innerText, checkItems[i], onRemoveButton, contentAttrs.settings, elementAttrs));

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

        mainButton.setEnabled(itemsPanel.childNodes.length > 0);
    }

    var actionFunction = function () {
        if (contentAttrs.selectionMode == "One") {
            menu.changeVisibleState(false);
        }
        mainButton.update();
    }

    jsObject.CreateListBoxItemsContent(element, itemsPanel, parentPanel, actionFunction);

    mainButton.update();

    //correct text position
    if (mainButton.offsetHeight && mainButton.innerTable.offsetHeight > mainButton.offsetHeight) {
        mainButton.innerTable.style.marginTop = -(mainButton.innerTable.offsetHeight - mainButton.offsetHeight) / 2 + "px";
    }
}

StiJsViewer.prototype.ComboBoxCaptionButton = function (caption, item, actionFunction, styleColors, elementAttrs) {
    var jsObject = this;
    var button = this.SmallButton(null, caption, null, null, "Down");
    button.item = item;

    //override
    button.style.display = "inline-block";
    StiJsViewer.setImageSource(button.arrow, this.options, this.collections, styleColors.isDarkStyle ? "CloseWhite.png" : "CloseForm.png");
    button.arrow.style.width = button.arrow.style.height = "16px";
    button.style.backgroundColor = styleColors.selectedBackColor;
    button.style.color = styleColors.selectedForeColor;
    button.style.border = "0";
    button.style.borderRadius = "0";
    button.style.boxSizing = "border-box";
    button.style.marginRight = "4px";
    button.style.height = "100%";
    button.style.fontSize = (elementAttrs && elementAttrs.font.size ? elementAttrs.font.size : "10") + "pt";
    button.arrow.parentNode.style.padding = "0 2px 0 0";
    button.arrow.style.margin = "0 0 1px 0";
    button.arrow.style.opacity = "0.6";
    if (button.caption) button.caption.style.padding = "0 2px 0 4px";

    button.onmouseoverAction = function () {
        if (this.jsObject.options.isTouchClick) return;
        this.isOver = true;
        this.style.backgroundColor = styleColors.hotSelectedBackColor;
        this.style.color = styleColors.hotSelectedForeColor;
    }

    button.onmouseoutAction = function () {
        this.isOver = false;
        this.style.backgroundColor = styleColors.selectedBackColor;
        this.style.color = styleColors.selectedForeColor;
    }

    if (this.options.isTouchDevice) {
        button.ontouchstart = function () { }
        button.ontouchend = function () { }

        button.arrow.ontouchstart = function () {
            jsObject.options.fingerIsMoved = false;
        }

        button.arrow.ontouchend = function (event) {
            if (jsObject.options.fingerIsMoved) return;
            button.parentNode.removeChild(button);
            actionFunction(button);
            event.stopPropagation();
            return false;
        }
    }
    else {
        button.arrow.onmouseover = function () {
            this.style.opacity = "1";
        }

        button.arrow.onmouseout = function () {
            this.style.opacity = "0.6";
        }

        button.arrow.onclick = function (event) {
            button.parentNode.removeChild(button);
            actionFunction(button);
            event.stopPropagation();
        }
    }

    return button;
}