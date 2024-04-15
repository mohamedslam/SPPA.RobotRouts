
StiMobileDesigner.prototype.ImageGallery = function (name, width, height, headerText, contextMenu) {
    var imageGallery = document.createElement("div");
    imageGallery.jsObject = this;
    imageGallery.selectedItem = null;
    imageGallery.style.position = "relative";
    imageGallery.contextMenu = contextMenu;
    if (name != null) this.options.controls[name] = imageGallery;
    this.AddProgressToControl(imageGallery);

    if (headerText) {
        imageGallery.appendChild(this.FormBlockHeader(headerText));
    }

    var innerContainer = document.createElement("div");
    innerContainer.className = "stiDesignerImageGalleryContainer";
    innerContainer.innerTable = null;
    innerContainer.style.width = (width || 250) + "px";
    innerContainer.style.height = (height || 100) + "px";
    imageGallery.appendChild(innerContainer);
    imageGallery.innerContainer = innerContainer;

    imageGallery.clear = function () {
        if (innerContainer.innerTable) innerContainer.removeChild(innerContainer.innerTable);
        innerContainer.innerTable = null;
        this.selectedItem = null;
    }

    imageGallery.addItem = function (itemObject) {
        var item = this.jsObject.ImageGalleryItem(this, itemObject, height || 100);
        item.style.marginRight = "5px";
        if (!innerContainer.innerTable) {
            innerContainer.innerTable = this.jsObject.CreateHTMLTable();
            innerContainer.innerTable.style.margin = "5px 0 0 5px";
            innerContainer.appendChild(innerContainer.innerTable);
        }
        innerContainer.innerTable.addCell(item);

        return item;
    }

    imageGallery.addItems = function (itemObjects) {
        this.clear();
        if (itemObjects) {
            for (var i = 0; i < itemObjects.length; i++) {
                imageGallery.addItem(itemObjects[i]);
            }
        }
    }

    imageGallery.getItemByPropertyValue = function (propertyName, value) {
        var items = this.getItems();
        for (var i = 0; i < items.length; i++) {
            if (items[i].itemObject && items[i].itemObject[propertyName] == value) {
                return items[i];
            }
        }
        return null;
    }

    imageGallery.selectItemByPropertyValue = function (propertyName, value) {
        var item = imageGallery.getItemByPropertyValue(propertyName, value);
        if (item) item.select(true);
    }

    imageGallery.autoscroll = function () {
        if (this.selectedItem && innerContainer.offsetWidth > 0) {
            innerContainer.scrollLeft = 0;
            var xPos = this.jsObject.FindPosX(this.selectedItem, null, null, innerContainer);
            if (xPos + 150 > innerContainer.offsetWidth) innerContainer.scrollLeft = xPos - innerContainer.offsetWidth + 150;
        }
    }

    imageGallery.getItems = function () {
        var items = [];
        if (innerContainer.innerTable) {
            var row = innerContainer.innerTable.tr[0];
            for (var i = 0; i < row.childNodes.length; i++) {
                if (row.childNodes[i].firstChild.itemObject) {
                    items.push(row.childNodes[i].firstChild);
                }
            }
        }
        return items;
    }

    imageGallery.action = function (item) { };

    return imageGallery;
}

StiMobileDesigner.prototype.ImageGalleryItem = function (imageGallery, itemObject, containerHeight) {
    var item = document.createElement("div");
    item.jsObject = this;
    item.style.display = "inline-block";
    item.isEnabled = true;
    item.isSelected = false;
    item.isOver = false;
    item.className = "stiDesignerImageGalleryItem stiDesignerImageGalleryItemDefault";
    item.itemObject = itemObject;

    var toolTipText = itemObject.name;
    switch (itemObject.type) {
        case "StiResource": toolTipText = this.loc.PropertyMain.Resource + ": " + toolTipText; break;
        case "StiVariable": toolTipText = this.loc.PropertyMain.Variable + ": " + toolTipText; break;
        case "StiDataColumn": toolTipText = this.loc.PropertyMain.DataColumn + ": " + toolTipText; break;
    }

    item.title = toolTipText;

    var innerTable = this.CreateHTMLTable();
    item.innerTable = innerTable;
    innerTable.style.height = "100%";
    innerTable.style.width = "100%";
    item.appendChild(innerTable);

    var imageCellHeight = containerHeight >= 80 ? containerHeight - 43 : containerHeight - 28;

    var image = document.createElement("img");
    item.image = image;
    image.style.maxWidth = "71px";
    image.style.maxHeight = (imageCellHeight - 4) + "px";

    if (itemObject.imageName)
        StiMobileDesigner.setImageSource(image, this.options, itemObject.imageName + ".png");
    else if (itemObject.src)
        image.src = itemObject.src;

    var cellImage = innerTable.addCell(image);
    item.cellImage = cellImage;
    cellImage.style.padding = "0px";
    cellImage.style.height = imageCellHeight + "px";
    cellImage.style.textAlign = "center";

    var captionCell = innerTable.addCellInNextRow();
    captionCell.className = "stiItemGalleryCaption stiItemGalleryCaptionDefault";
    item.captionCell = captionCell;

    var caption = document.createElement("div");
    item.caption = caption;
    caption.innerHTML = itemObject.name;
    caption.className = "stiItemGalleryCaptionContainer";
    captionCell.appendChild(caption);

    if (containerHeight <= 80) {
        captionCell.style.display = "none";
        cellImage.style.maxWidth = "80px";
        cellImage.style.minWidth = "50px";
        image.style.margin = "0 2px 0 2px";
    }

    item.onmouseover = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseenter();
    }

    item.onmouseenter = function () {
        if (!this.isEnabled || this.jsObject.options.isTouchClick) return;
        this.isOver = true;
        this.className = "stiDesignerImageGalleryItem stiDesignerImageGalleryItemOver";
        captionCell.className = "stiItemGalleryCaption stiItemGalleryCaptionOver";
    }

    item.onmouseleave = function () {
        this.isOver = false;
        if (!this.isEnabled) return;
        this.className = "stiDesignerImageGalleryItem " + (this.isSelected ? "stiDesignerImageGalleryItemSelected" : "stiDesignerImageGalleryItemDefault");
        captionCell.className = "stiItemGalleryCaption " + (this.isSelected ? "stiItemGalleryCaptionSelected" : "stiItemGalleryCaptionDefault");
    }

    item.onmousedown = function () {
        if (this.isTouchStartFlag || !this.isEnabled) return;
        this.jsObject.options.buttonPressed = this;
    }

    item.onclick = function () {
        if (this.isTouchEndFlag || !this.isEnabled || this.jsObject.options.isTouchClick) return;
        this.action();
    }

    item.ontouchend = function () {
        if (!this.isEnabled || this.jsObject.options.fingerIsMoved) return;
        var this_ = this;
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);
        this.jsObject.options.buttonsTimer = [this, this.className, setTimeout(function () {
            this_.jsObject.options.buttonsTimer = null;
            this_.className = "stiDesignerImageGalleryItem stiDesignerImageGalleryItemDefault";
            captionCell.className = "stiItemGalleryCaption stiItemGalleryCaptionDefault";
            this_.action();
        }, 150)];
        this.className = "stiDesignerImageGalleryItem stiDesignerImageGalleryItemOver";
        captionCell.className = "stiItemGalleryCaption stiItemGalleryCaptionOver";

        this.isTouchEndTimer = setTimeout(function () {
            this_.isTouchEndFlag = false;
        }, 1000);
    }

    item.ontouchstart = function () {
        var this_ = this;
        this.isTouchStartFlag = true;
        clearTimeout(this.isTouchStartTimer);
        this.jsObject.options.fingerIsMoved = false;
        this.jsObject.options.buttonPressed = this;
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    item.setEnabled = function (state) {
        if (this.image) this.image.style.opacity = state ? "1" : "0.3";
        this.isEnabled = state;
        if (!state && !this.isOver) this.isOver = false;
        this.className = "stiDesignerImageGalleryItem " +
            (state ? (this.isOver ? "stiDesignerImageGalleryItemOver" : "stiDesignerImageGalleryItemDefault") : "stiDesignerImageGalleryItemDisabled");
        captionCell.className = "stiItemGalleryCaption " +
            (state ? (this.isOver ? "stiItemGalleryCaptionOver" : "stiItemGalleryCaptionDefault") : "stiItemGalleryCaptionDisabled");
    }

    item.setSelected = function (state) {
        this.isSelected = state;
        this.className = "stiDesignerImageGalleryItem " +
            (state ? "stiDesignerImageGalleryItemSelected" : (this.isEnabled ? (this.isOver ? "stiDesignerImageGalleryItemOver" : "stiDesignerImageGalleryItemDefault")
                : "stiDesignerImageGalleryItemDisabled"));
        captionCell.className = "stiItemGalleryCaption " +
            (state ? "stiItemGalleryCaptionSelected" : (this.isEnabled ? (this.isOver ? "stiItemGalleryCaptionOver" : "stiItemGalleryCaptionDefault")
                : "stiItemGalleryCaptionDisabled"));
    }

    item.action = function (autoscroll) {
        this.select(true, autoscroll);
        imageGallery.action(this);
    }

    item.select = function (state, autoscroll) {
        if (state && imageGallery.selectedItem) imageGallery.selectedItem.setSelected(false);
        this.setSelected(state ? true : false);
        imageGallery.selectedItem = state ? this : null;
        if (state && autoscroll) setTimeout(function () { imageGallery.autoscroll() });
    }

    item.remove = function () {
        if (this.parentElement && this.parentElement.parentElement) {
            this.parentElement.parentElement.removeChild(this.parentElement);
            if (this == imageGallery.selectedItem) {
                imageGallery.selectedItem = null;
            }
        }
    };

    if (imageGallery.contextMenu) {
        item.onmouseup = function (event) {
            if (this.isTouchEndFlag || this.jsObject.options.isTouchClick) return;
            if (event.button == 2) {
                event.stopPropagation();
                this.action();
                var point = this.jsObject.FindMousePosOnMainPanel(event);
                imageGallery.contextMenu.show(point.xPixels + 3, point.yPixels + 3, "Down", "Right");
            }
            return false;
        }

        imageGallery.oncontextmenu = function (event) {
            return false;
        }
    }

    return item;
}