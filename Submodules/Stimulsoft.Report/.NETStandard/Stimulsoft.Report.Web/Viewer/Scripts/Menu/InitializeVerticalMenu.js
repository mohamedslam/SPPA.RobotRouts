
StiJsViewer.prototype.VerticalMenu = function (name, parentButton, animDirection, items, itemStyleName, menuStyleName, rightToLeft, imageSizes) {
    var menu = this.BaseMenu(name, parentButton, animDirection, menuStyleName, rightToLeft);
    menu.itemStyleName = itemStyleName;

    if (this.options.isMobileDevice) {
        menu.style.left = "0";
        menu.style.top = "0";
        menu.style.bottom = "0";
        menu.style.height = "100%";

        menu.innerContent.style.maxHeight = "100%";
        menu.innerContent.style.height = "100%";
        menu.innerContent.style.borderLeftWidth = "0";
        menu.innerContent.style.borderTopWidth = "0";
        menu.innerContent.style.borderBottomWidth = "0";
    }

    menu.clear = function () {
        this.items = {};
        while (this.innerContent.childNodes[0]) {
            this.innerContent.removeChild(this.innerContent.childNodes[0]);
        }
    }

    menu.addItems = function (items) {
        while (this.innerContent.childNodes[0]) {
            this.innerContent.removeChild(this.innerContent.childNodes[0]);
        }
        if (items && items.length) {
            for (var index = 0; index < items.length; index++) {
                if (typeof (items[index]) != "string")
                    this.innerContent.appendChild(this.jsObject.VerticalMenuItem(this, items[index].name, items[index].caption, items[index].imageName, items[index].key, this.itemStyleName, items[index].haveSubMenu, items[index].imageSizes || imageSizes));
                else
                    this.innerContent.appendChild(this.jsObject.VerticalMenuSeparator(this, items[index]));
            }
        }
    }

    menu.addItems(items);

    return menu;
}

StiJsViewer.prototype.VerticalMenuItem = function (menu, itemName, caption, imageName, key, styleName, haveSubMenu, imageSizes) {
    var menuItem = document.createElement("div");
    menuItem.jsObject = this;
    menuItem.menu = menu;
    menuItem.name = itemName;
    menuItem.key = key;
    menuItem.caption_ = caption;
    menuItem.imageName = imageName;
    menuItem.styleName = styleName || "stiJsViewerMenuStandartItem";
    menuItem.id = this.generateKey();
    menuItem.className = menuItem.styleName;
    menu.items[itemName] = menuItem;
    menuItem.isEnabled = true;
    menuItem.isSelected = false;
    menuItem.haveSubMenu = haveSubMenu;
    menuItem.style.height = this.options.isMobileDevice ? "0.4in" : (this.options.isTouchDevice ? "30px" : "24px");

    var innerTable = this.CreateHTMLTable();
    menuItem.appendChild(innerTable);
    innerTable.style.height = "100%";
    innerTable.style.width = "100%";

    var isSmallItem = menuItem.styleName && menuItem.styleName.indexOf("MenuStandartItem") >= 0;

    var cellImage;
    var captionCell;
    var arrowCell;

    if (imageName != null) {
        cellImage = innerTable.addCell();
        menuItem.cellImage = cellImage;
        cellImage.style.width = "22px";
        cellImage.style.minWidth = "22px";
        cellImage.style.padding = "0";
        cellImage.style.textAlign = "center";
        cellImage.style.lineHeight = "0";

        var img = document.createElement("img");
        img.style.width = (imageSizes ? imageSizes.width : (isSmallItem ? 16 : 32)) + "px";
        img.style.height = (imageSizes ? imageSizes.height : (isSmallItem ? 16 : 32)) + "px";
        menuItem.image = img;
        cellImage.appendChild(img);

        if (StiJsViewer.checkImageSource(this.options, this.collections, imageName))
            StiJsViewer.setImageSource(img, this.options, this.collections, imageName);
        else
            img.style.display = "none";
    }

    if (caption != null) {
        captionCell = innerTable.addCell();
        menuItem.caption = captionCell;
        captionCell.style.padding = "0 20px 0 7px";
        captionCell.style.textAlign = "left";
        captionCell.style.whiteSpace = "nowrap";
        if (this.options.isMobileDevice) captionCell.style.fontSize = "0.16in";
        captionCell.innerText = caption;

        if (itemName && itemName.toString().indexOf("fontItem") == 0) {
            captionCell.style.fontSize = "16px";
            captionCell.style.fontFamily = key;
            captionCell.style.lineHeight = "1";
            menuItem.setAttribute("title", caption);
            menuItem.style.overflow = "hidden";
        }
    }

    if (haveSubMenu) {
        menuItem.arrowImg = document.createElement("img");
        arrowCell = innerTable.addCell();
        menuItem.arrowCell = arrowCell;
        arrowCell.style.lineHeight = "0";
        arrowCell.style.textAlign = "right";
        arrowCell.appendChild(menuItem.arrowImg);
        StiJsViewer.setImageSource(menuItem.arrowImg, this.options, this.collections, isSmallItem ? "Arrows.SmallArrowRight.png" : "Arrows.BigArrowRight.png");
        menuItem.arrowImg.style.width = menuItem.arrowImg.style.height = "16px";
    }

    if (this.options.appearance.rightToLeft) {
        if (captionCell && cellImage) {
            innerTable.tr[0].insertBefore(captionCell, cellImage);
            if (arrowCell) {
                innerTable.tr[0].insertBefore(arrowCell, captionCell);
                StiJsViewer.setImageSource(menuItem.arrowImg, this.options, this.collections, isSmallItem ? "Arrows.SmallArrowLeft.png" : "Arrows.BigArrowLeft.png");
            }
            captionCell.style.textAlign = "right";
            captionCell.style.padding = "0 7px 0 20px";
        }
    }

    menuItem.onmouseover = function () {
        if (this.isTouchProcessFlag || !this.isEnabled) return;
        this.className = this.styleName + " " + this.styleName + "Over";
    }

    menuItem.onmouseout = function () {
        if (this.isTouchProcessFlag || !this.isEnabled) return;
        this.className = this.styleName;
        if (this.isSelected) this.className += " " + this.styleName + "Selected";
    }

    menuItem.onclick = function () {
        if (this.isTouchProcessFlag || !this.isEnabled) return;
        this.action();
    }

    menuItem.onmousedown = function () {
        if (this.isTouchStartFlag || !this.isEnabled) return;
        this.jsObject.options.menuItemPressed = this;
    }

    menuItem.ontouchstart = function () {
        var this_ = this;
        this.isTouchStartFlag = true;
        clearTimeout(this.isTouchStartTimer);
        this.jsObject.options.fingerIsMoved = false;
        this.jsObject.options.menuItemPressed = this;
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    menuItem.ontouchend = function () {
        if (!this.isEnabled || this.jsObject.options.fingerIsMoved) return;
        this.isTouchProcessFlag = true;
        this.className = this.styleName + " " + this.styleName + "Over";
        var this_ = this;
        setTimeout(function () {
            this_.className = this_.styleName;
            this_.action();
        }, 150);
        setTimeout(function () {
            this_.isTouchProcessFlag = false;
        }, 1000);
    }

    menuItem.action = function () {
        this.menu.action(this);
    }

    menuItem.setEnabled = function (state) {
        this.isEnabled = state;
        this.className = this.styleName + " " + (state ? "" : (this.styleName + "Disabled"));
    }

    menuItem.setSelected = function (state) {
        if (!state) {
            this.isSelected = false;
            this.className = this.styleName;
            return;
        }
        if (this.menu.selectedItem != null) {
            this.menu.selectedItem.className = this.styleName;
            this.menu.selectedItem.isSelected = false;
        }
        this.className = this.styleName + " " + this.styleName + "Selected";
        this.menu.selectedItem = this;
        this.isSelected = true;
    }

    return menuItem;
}

StiJsViewer.prototype.VerticalMenuSeparator = function (menu, name) {
    var menuSeparator = document.createElement("div");
    menuSeparator.className = "stiJsViewerVerticalMenuSeparator";
    if (menu && name) menu.items[name] = menuSeparator;

    return menuSeparator;
}