
StiMobileDesigner.prototype.ImageList = function (name, showCaption, showImage, toolTip, items, imageSizes) {
    var imageList = this.CreateHTMLTable();
    imageList.jsObject = this;
    imageList.name = name || this.generateKey();
    imageList.key = null;
    imageList.isEnabled = true;
    imageList.items = (items == null) ? {} : items;
    if (name) this.options.controls[name] = imageList;

    var button = imageList.button = this.StandartSmallButton(name ? name + "Button" : null, null, showCaption ? " " : null, showImage ? " " : null, toolTip, "Down", null, imageSizes || { width: 32, height: 16 });
    button.imageList = imageList;
    imageList.addCell(button);

    var menu = imageList.menu = this.VerticalMenu(name ? name + "Menu" : null, imageList.button, "Down", items, null, null, imageSizes || { width: 32, height: 16 });
    menu.type = "ImageListMenu";
    menu.imageList = imageList;
    menu.isDinamic = (items == null);
    this.options.mainPanel.appendChild(menu);
    if (items != null) menu.addItems(items);

    imageList.setKey = function (key) {
        this.key = key;
        if (button.image != null && key == "StiEmptyValue") {
            StiMobileDesigner.setImageSource(button.image, this.jsObject.options, "BorderStyleNone.png");
        }
        for (var itemName in this.items) {
            if (key == this.items[itemName].key) {
                if (button.caption != null) button.caption.innerHTML = this.items[itemName].caption;
                if (button.image != null) StiMobileDesigner.setImageSource(button.image, this.jsObject.options, this.items[itemName].imageName);
                return;
            }
        }
    }

    button.action = function () {
        if (!menu.visible) {
            if (menu.isDinamic) menu.addItems(this.imageList.items);
            menu.changeVisibleState(true);
        }
        else
            menu.changeVisibleState(false);
    }

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
        this.imageList.key = menuItem.key;
        if (button.caption != null) button.caption.innerHTML = menuItem.caption.innerHTML;
        if (button.image != null) StiMobileDesigner.setImageSource(button.image, this.jsObject.options, menuItem.imageName);
        this.imageList.action();
    }

    menu.onshow = function () {
        if (this.imageList.key == null) return;
        for (var itemName in this.items) {
            if (this.imageList.key == this.items[itemName].key) {
                this.items[itemName].setSelected(true);
                return;
            }
        }
    }

    menu.onmousedown = function () {
        if (!this.isTouchStartFlag) this.ontouchstart(true);
    }

    menu.ontouchstart = function (mouseProcess) {
        var this_ = this;
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        this.jsObject.options.imageListMenuPressed = this;
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    //Override methods
    imageList.setEnabled = function (state) {
        this.isEnabled = state;
        button.setEnabled(state);
    }

    imageList.action = function () { }

    return imageList;
}