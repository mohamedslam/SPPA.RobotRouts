
StiMobileDesigner.prototype.CloudContainer = function (name, allowedItemTypes, width, height) {
    var container = this.CreateHTMLTable();
    if (name != null) this.options.containers[name] = container;
    container.name = name != null ? name : this.generateKey();
    container.item = null;
    container.allowedItemTypes = allowedItemTypes || [];
    container.style.width = width ? width + "px" : "100%";

    //Items Container 
    var itemsContent = container.itemsContent = document.createElement("div");
    itemsContent.jsObject = this;
    itemsContent.container = container;
    container.addCell(itemsContent).style.verticalAlign = "top";
    if (height) container.style.height = height + "px";

    //Buttons Panel
    container.buttonsPanel = this.CreateHTMLTable();
    var buttonsCell = container.addCell(container.buttonsPanel);
    buttonsCell.style.width = "1px";
    buttonsCell.style.verticalAlign = "top";

    var buttons = [];
    buttons.push(["getItemButton", "GetItem.png"]);
    buttons.push(["deleteButton", "DeleteBlack.png"]);

    for (var i = 0; i < buttons.length; i++) {
        var button = this.SmallButton(null, null, null, buttons[i][1], null, null, "stiDesignerFormButton");
        container[buttons[i][0]] = button;
        button.container = container;
        button.style.margin = (i == 0 ? "4px" : "0px") + " 4px 4px 0";
        container.buttonsPanel.addCellInNextRow(button);
        button.setEnabled(false);
    }

    //Buttons Methods    

    container.getItemButton.action = function () {
        var selectedItem = this.jsObject.options.dictionaryTree.selectedItem;
        if (selectedItem) { container.addItem(selectedItem.itemObject); }
    }

    container.deleteButton.action = function () {
        container.clear();
    };

    container.addItem = function (itemObject) {
        if (!container.canInsertItem(itemObject)) return;
        container.clear();
        var item = container.jsObject.OneItemContainerItem(itemObject, container);
        itemsContent.appendChild(item);
        container.item = item;
        container.deleteButton.setEnabled(true);
    }

    //Events
    container.onmouseup = function () {
        if (!this.isTouchEndFlag && this.jsObject.options.itemInDrag && !this.jsObject.options.isTouchClick)
            this.getItemButton.action();
    }

    container.ontouchend = function () {
        var this_ = this;
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);
        if (this.jsObject.options.itemInDrag) this.getItemButton.action();
        this.isTouchEndTimer = setTimeout(function () {
            this_.isTouchEndFlag = false;
        }, 1000);
    }

    //Container Methods
    container.clear = function () {
        if (container.item) itemsContent.removeChild(container.item);
        container.deleteButton.setEnabled(false);
        container.item = null;
    }

    container.canInsertItem = function (itemObject) {
        if (!itemObject) return false;
        for (var i = 0; i < container.allowedItemTypes.length; i++) {
            if (container.allowedItemTypes[i] == itemObject.typeItem) {
                return true;
            }
        }
        return false;
    }

    return container;
}

//Container Item
StiMobileDesigner.prototype.OneItemContainerItem = function (itemObject, container, imageName) {
    var caption = itemObject.name;
    var imageName = imageName || ("Big" + itemObject.typeIcon + ".png");

    var item = this.SmallButton(null, null, caption, imageName, caption, null, "stiDesignerContainerItem", true, { width: 32, height: 32 });
    item.container = container;
    item.itemObject = itemObject;

    item.image.onerror = function () {
        StiMobileDesigner.setImageSource(this, item.jsObject.options, "BigFile.png");
    }

    //Override    
    item.caption.style.padding = "0px";
    item.imageCell.style.padding = "2px";
    item.image.style.marginTop = "0";
    item.style.margin = "10px 0 0 10px";

    var newCaption = document.createElement("div");
    newCaption.setAttribute("style", "text-overflow: ellipsis; overflow: hidden; white-space: nowrap; max-width: 200px; min-width: 65px; padding-right: 2px;");
    newCaption.innerHTML = caption;

    item.caption.innerHTML = "";
    item.caption.appendChild(newCaption);
    item.caption = newCaption;

    return item;
}

//Container Item
StiMobileDesigner.prototype.EasyContainerItem = function (caption, imageName) {
    var item = this.SmallButton(null, null, caption, imageName, caption, null, "stiDesignerContainerItem", true, { width: 32, height: 32 });

    //Override    
    item.caption.style.padding = "0px 15px 0 0";
    item.imageCell.style.padding = "2px";
    item.image.style.marginTop = "0";

    var newCaption = document.createElement("div");
    newCaption.setAttribute("style", "text-overflow: ellipsis; overflow: hidden; white-space: nowrap; max-width: 200px; min-width: 65px; padding-right: 2px;");
    newCaption.innerText = caption;

    item.caption.innerHTML = "";
    item.caption.appendChild(newCaption);
    item.caption = newCaption;

    return item;
}