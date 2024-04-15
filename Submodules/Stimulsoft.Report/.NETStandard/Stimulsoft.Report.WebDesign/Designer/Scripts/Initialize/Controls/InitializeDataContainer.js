
StiMobileDesigner.prototype.DataContainer = function (width, height, showItemImage, emptyText, allowManuallyData) {
    var container = document.createElement("div");
    var jsObject = container.jsObject = this;
    container.className = "stiDataContainer";
    if (width) {
        container.width = width;
        container.style.width = width + "px";
    }
    if (height) container.style.height = height + "px";
    container.selectedItem = null;

    container.updateHintText = function () {
        if (this.hintText) {
            this.removeChild(this.hintText);
            this.hintText = null;
        }
        if (this.childNodes.length == 0) {
            if (allowManuallyData && container.isMaximize) {
                var table = jsObject.CreateHTMLTable();
                table.setAttribute("style", "position: absolute; height: 95%; width: 100%; text-align: center");
                table.addCell().style.height = "35%";
                table.isHintElement = true;

                var text = document.createElement("div");
                text.className = "stiCreateDataHintText";
                text.style.padding = "6px";
                text.innerHTML = jsObject.loc.Dashboard.DragDropDataFromDictionary;
                var cell1 = table.addCellInNextRow(text);
                cell1.style.textAlign = "center";
                cell1.style.height = "20px";

                var separator = jsObject.SeparatorOr();
                separator.style.width = "calc(100% - 70px)";
                separator.style.display = "inline-block";
                var cell2 = table.addCellInNextRow(separator);
                cell2.style.textAlign = "center";
                cell2.style.height = "20px";

                var text2 = document.createElement("div");
                text2.className = "stiCreateDataHintHeaderText";
                text2.style.padding = "6px";
                text2.style.fontSize = "12px";
                text2.style.cursor = "pointer";
                text2.innerHTML = jsObject.loc.Report.EnterDataManually;
                var cell3 = table.addCellInNextRow(text2);
                cell3.style.textAlign = "center";
                cell3.style.height = "20px";

                text2.onclick = function () {
                    if (container.actionEnterManuallyData) container.actionEnterManuallyData();
                }

                var text3 = document.createElement("div");
                text3.className = "stiCreateDataHintHeaderText";
                text3.style.padding = "6px 6px 12px 6px";
                text3.style.fontSize = "12px";
                text3.style.cursor = "pointer";
                text3.innerHTML = jsObject.loc.Buttons.ShowMore;

                text3.onclick = function () {
                    if (container.actionShowMore) container.actionShowMore();
                }

                var cell4 = table.addCellInNextRow(text3)
                cell4.style.verticalAlign = "bottom";

                this.hintText = table;
                this.appendChild(table);
            }
            else {
                var hintText = document.createElement("div");
                hintText.isHintElement = true;
                hintText.className = "wizardFormHintText";
                hintText.innerHTML = emptyText || jsObject.loc.Dashboard.DragDropDataFromDictionary;
                hintText.style.width = "100%";
                hintText.style.textAlign = "center";
                hintText.style.top = "calc(50% - 9px)";
                this.appendChild(hintText);
                this.hintText = hintText;

            }
            this.style.borderStyle = "dashed";
        }
        else {
            this.style.borderStyle = "solid";
        }
    }

    container.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        this.selectedItem = null;
        this.style.paddingBottom = "0px";
        this.hintText = null;
        this.onAction("clear");
        this.updateHintText();
    }

    container.addItem = function (caption, imageName, itemObject, insertIndex) {
        var item = jsObject.DataContainerItem(caption, showItemImage ? imageName : null, itemObject, this);
        if (insertIndex != null && this.getCountItems() > 0 && insertIndex < this.getCountItems() - 1) {
            this.insertBefore(item, this.childNodes[insertIndex + 1]);
        }
        else {
            this.appendChild(item);
        }
        this.updateHintText();
        this.style.borderColor = "";

        return item;
    }

    container.getItemIndex = function (item) {
        for (var i = 0; i < this.childNodes.length; i++)
            if (this.childNodes[i] == item) return i;

        return null;
    }

    container.getItemByIndex = function (index) {
        if (index != null && !this.hintText && index >= 0 && index < this.childNodes.length) {
            return this.childNodes[index];
        }

        return null;
    }

    container.getSelectedItemIndex = function () {
        return this.selectedItem ? this.getItemIndex(this.selectedItem) : null;
    }

    container.getOverItemIndex = function () {
        for (var i = 0; i < this.childNodes.length; i++)
            if (this.childNodes[i].isOver) return i;

        return null;
    }

    container.getCountItems = function () {
        return (!this.hintText ? this.childNodes.length : 0);
    }

    container.moveItem = function (fromIndex, toIndex) {
        if (fromIndex < this.childNodes.length && toIndex < this.childNodes.length) {
            var fromItem = this.childNodes[fromIndex];
            //Move Down
            if (fromIndex < toIndex) {
                if (toIndex < this.childNodes.length - 1) {
                    this.insertBefore(fromItem, this.childNodes[toIndex + 1]);
                }
                else {
                    this.appendChild(fromItem);
                }
            }
            //Move Up
            else {
                this.insertBefore(fromItem, this.childNodes[toIndex]);
            }

            return fromItem;
        }
    }

    container.onmouseover = function () {
        if (jsObject.options.itemInDrag && jsObject.options.itemInDrag.originalItem.itemObject) {
            var typeItem = jsObject.options.itemInDrag.originalItem.itemObject.typeItem;
            if (typeItem == "Column" || typeItem == "DataSource" || typeItem == "BusinessObject" || typeItem == "Meter") {
                container.style.borderStyle = "dashed";
                container.style.borderColor = jsObject.options.themeColors[jsObject.GetThemeColor()];
            }
        }
    }

    container.onmouseout = function () {
        container.style.borderStyle = this.hintText ? "dashed" : "solid";
        container.style.borderColor = "";
    }

    container.onmouseup = function (event) {
        if (event.button != 2 && jsObject.options.itemInDrag) {
            var itemObject = jsObject.CopyObject(jsObject.options.itemInDrag.itemObject);
            if (!itemObject) return;

            var toIndex = this.getOverItemIndex();
            var fromIndex = this.getItemIndex(jsObject.options.itemInDrag.originalItem);

            if (toIndex != null && fromIndex != null && fromIndex != toIndex) {
                var item = container.moveItem(fromIndex, toIndex);
                if (item) item.select();
            }
        }
        return false;
    }

    container.onAction = function (actionName) { }
    container.onRemove = function () { }

    return container;
}

StiMobileDesigner.prototype.DataContainerItem = function (caption, imageName, itemObject, container) {
    var jsObject = this;
    var item = this.StandartSmallButton(null, null, caption, imageName);
    item.style.border = "0";
    item.container = container;
    item.itemObject = itemObject;

    if (item.caption) {
        item.caption.style.width = "100%";
        var captCont = document.createElement("div");
        item.captionContainer = captCont;
        captCont.style.position = "relative";
        captCont.style.overflow = "hidden";
        captCont.style.textOverflow = "ellipsis";
        captCont.style.padding = "2px 0 2px 0";
        if (container.width || container.maxWidth) {
            var textMaxWidth = (container.width || container.maxWidth) - 50;
            if (imageName) textMaxWidth -= 25;
            captCont.style.maxWidth = textMaxWidth + "px";
        }
        captCont.innerHTML = caption;
        item.caption.innerHTML = "";
        item.caption.appendChild(captCont);
    }

    item.style.height = "30px";
    item.style.marginBottom = "1px";
    item.innerTable.style.width = "100%";

    var closeButton = this.StandartSmallButton(null, null, null, "CloseForm.png");
    closeButton.image.style.margin = "0 2px 0 2px";
    closeButton.imageCell.style.padding = this.options.isTouchDevice ? "0 4px 0 4px" : "0 3px 0 3px";
    closeButton.style.margin = "1px";
    closeButton.style.background = "white";
    closeButton.style.height = closeButton.style.width = "26px";
    closeButton.style.display = "none";
    closeButton.innerTable.style.width = "100%";
    item.closeButton = closeButton;
    item.innerTable.addCell(closeButton);

    closeButton.onmouseenter = function () {
        if (!this.isEnabled || jsObject.options.isTouchClick) return;
        this.className = this.overClass;
        this.isOver = true;
        closeButton.style.background = "lightgray";
    }

    closeButton.onmouseleave = function () {
        this.isOver = false;
        if (!this.isEnabled) return;
        this.className = this.isSelected ? this.selectedClass : this.defaultClass;
        this.style.background = "white";
    }

    closeButton.action = function () {
        item.closeButtonAction = true;
        item.remove();
    }

    item.setSelected_ = item.setSelected;
    item.setSelected = function (state) {
        this.setSelected_(state);
        if (jsObject.options.isTouchDevice) {
            closeButton.style.display = state ? "" : "none";
        }
    }

    item.onmousedown = function (event) {
        if (this.isTouchStartFlag || container.editableItem == this) return;
        if (event) event.preventDefault();
        var options = jsObject.options;

        if (options.controlsIsFocused && options.controlsIsFocused.action) {
            options.controlsIsFocused.blur();
            options.controlsIsFocused = null;
        }

        if (this.itemObject && event.button != 2 && !options.controlsIsFocused) {
            var itemInDragObject = jsObject.TreeItemForDragDrop({ name: caption, typeIcon: imageName ? imageName.replace(".png", "") : "" }, null, !imageName);
            if (itemInDragObject.button.captionCell) itemInDragObject.button.captionCell.style.padding = "3px 15px 3px 5px";
            if (itemInDragObject.button.imageCell) itemInDragObject.button.imageCell.style.padding = "2px 5px 2px 5px";
            itemInDragObject.originalItem = item;
            itemInDragObject.beginingOffset = 0;
            options.itemInDrag = itemInDragObject;
        }
    }

    item.onmouseup = function (event) {
        if (container.editableItem == this) return;
        if (event.button == 2) {
            item.action();
        }
    }

    item.action = function () {
        if (!item.closeButtonAction) item.select();
        item.closeButtonAction = false;
    }

    item.remove = function () {
        if (container.selectedItem == this) {
            var prevItem = this.previousSibling;
            var nextItem = this.nextSibling;
            container.selectedItem = null;
            if (container.childNodes.length > 1) {
                if (nextItem) {
                    nextItem.setSelected(true);
                    container.selectedItem = nextItem;
                }
                else if (prevItem) {
                    prevItem.setSelected(true);
                    container.selectedItem = prevItem;
                }
            }
        }
        container.onRemove(container.getItemIndex(this));
        container.removeChild(this);
        container.onAction("remove");
        container.updateHintText();
    }

    item.select = function () {
        if (container.selectedItem) container.selectedItem.setSelected(false);
        this.setSelected(true);
        container.selectedItem = this;
        container.onAction("select");
    }

    item.repaint = function (newCaption, newImage, newItemObject) {
        this.itemObject = newItemObject;

        if (newCaption != null && this.caption) {
            this.captionContainer.innerHTML = newCaption;
        }
        if (newImage != null && this.image && StiMobileDesigner.checkImageSource(jsObject.options, newImage)) {
            StiMobileDesigner.setImageSource(this.image, jsObject.options, newImage);
        }
    }

    item.setEditable = function (state) {
        if (state) {
            if (this.caption && this.itemObject && (this.itemObject.name != null || this.itemObject.label != null)) {
                var textBox = jsObject.TextBox(null);
                textBox.value = this.itemObject.name != null ? this.itemObject.name : this.itemObject.label;
                textBox.style.position = "absolute";
                textBox.style.width = (this.caption.offsetWidth - 15) + "px";
                textBox.style.left = "0px";
                textBox.style.top = "-8px";
                textBox.style.border = "0px";
                textBox.style.height = "20px";
                textBox.focus();

                container.editableItem = this;
                this.captionContainer.innerHTML = "";
                this.captionContainer.appendChild(textBox);
                this.captionContainer.style.overflow = "visible";
                this.editableTextBox = textBox;

                textBox.onblur = function () {
                    textBox.action();
                }

                var this_ = this;
                textBox.action = function () {
                    this_.setEditable(false);
                    jsObject.options.controlsIsFocused = null;
                }
            }
        }
        else {
            if (this.editableTextBox) {
                var oldText = this.itemObject.name != null ? this.itemObject.name : this.itemObject.label;
                var nameChanged = oldText != this.editableTextBox.value;
                if (nameChanged && this.editableTextBox.value) {
                    if (this.itemObject.name != null) {
                        var newObject = { name: this.editableTextBox.value };
                        if (container["checkColumnName"])
                            newObject = container.checkColumnName(newObject);

                        if (this.itemObject.alias && this.itemObject.name == this.itemObject.alias) {
                            this.itemObject.alias = newObject.name;
                        }
                        this.itemObject.name = newObject.name;
                        container.onAction();
                    }
                    else if (this.itemObject.label != null) {
                        this.itemObject.label = this.editableTextBox.value;
                        container.onAction("rename");
                    }
                }
                this.captionContainer.removeChild(this.editableTextBox);
                this.captionContainer.style.overflow = "hidden";
                this.captionContainer.innerHTML = this.itemObject.name != null
                    ? jsObject.GetItemCaption(this.itemObject)
                    : (this.itemObject.label != null ? this.itemObject.label : "");
            }
            this.editableTextBox = null;
            container.editableItem = null;
        }
    }

    item.onmouseenter = function () {
        if (!this.isEnabled || jsObject.options.isTouchClick) return;
        this.className = this.overClass;
        this.isOver = true;
        closeButton.style.display = "";
    }

    item.onmouseleave = function () {
        this.isOver = false;
        if (!this.isEnabled) return;
        this.className = this.isSelected ? this.selectedClass : this.defaultClass;
        closeButton.style.display = "none";
    }

    return item;
}
