
StiMobileDesigner.prototype.VerticalMenu = function (name, parentButton, animDirection, items, itemsStyle, cutMenu, imageSizes, itemsHeight) {
    var menu = this.BaseMenu(name, parentButton, animDirection);
    menu.itemsStyle = itemsStyle;
    menu.cutMenu = cutMenu;

    menu.clear = function () {
        while (this.innerContent.childNodes[0]) {
            this.innerContent.removeChild(this.innerContent.childNodes[0]);
        }
    }

    menu.addItems = function (items) {
        this.clear();

        if (items && items.length) {
            for (var i = 0; i < items.length; i++) {
                if (typeof (items[i]) != "string") {
                    var item = (items[i].styleProperties != null)
                        ? this.jsObject.VerticalMenuItemForStyles(this, items[i], this.itemsStyle, imageSizes)
                        : this.jsObject.VerticalMenuItem(this, items[i].name, items[i].caption, items[i].imageName, items[i].key, this.itemsStyle, items[i].haveSubMenu, items[i].imageSizes || imageSizes);
                    if (itemsHeight) item.style.height = itemsHeight + "px";
                    this.innerContent.appendChild(item);
                }
                else
                    this.innerContent.appendChild(this.jsObject.VerticalMenuSeparator(this, items[i]));
            }
        }
    }

    menu.addItems(items);

    return menu;
}

StiMobileDesigner.prototype.VerticalMenuItem = function (menu, itemName, captionText, imageName, key, style, haveSubMenu, imageSizes) {
    var menuItem = document.createElement("div");
    var jsObject = menuItem.jsObject = this;

    menu.items[itemName] = menuItem;
    menuItem.menu = menu;
    menuItem.name = itemName;
    menuItem.key = key;
    menuItem.captionText = captionText;
    menuItem.imageName = imageName;
    menuItem.id = this.generateKey();
    menuItem.isEnabled = true;
    menuItem.isSelected = false;
    menuItem.selectedItem = null;
    menuItem.haveSubMenu = haveSubMenu;
    menuItem.isOver = false;
    menuItem.style.boxSizing = "content-box";
    if (menu.cutMenu) menuItem.setAttribute("title", captionText);

    if (!style) style = "stiDesignerMenuStandartItem";

    var baseStyle = style + " " + style + (this.options.isTouchDevice ? "_Touch" : "_Mouse") + " " + style;
    var defaultClass = menuItem.defaultClass = baseStyle + "Default";
    var overClass = menuItem.overClass = baseStyle + "Over";
    var selectedClass = menuItem.selectedClass = baseStyle + "Selected";
    var disabledClass = menuItem.disabledClass = baseStyle + "Disabled";

    menuItem.style = style;
    menuItem.className = defaultClass;

    // eslint-disable-next-line no-debugger
    if (typeof itemName != "string") debugger;

    var innerTable = menuItem.innerTable = this.CreateHTMLTable();
    menuItem.appendChild(innerTable);
    innerTable.style.height = innerTable.style.width = "100%";

    var isSmallItem = menuItem.className && menuItem.className.indexOf("MenuStandartItem") >= 0;

    if (imageName != null) {
        var cellImage = menuItem.cellImage = innerTable.addCell();
        cellImage.style.width = "1px";
        cellImage.style.fontSize = "0px";
        cellImage.style.padding = "0 5px 0 5px";
        cellImage.style.textAlign = "left";

        if (StiMobileDesigner.checkImageSource(this.options, imageName)) {
            var img = menuItem.image = document.createElement("img");
            img.style.width = (imageSizes ? imageSizes.width : (isSmallItem ? 16 : 32)) + "px";
            img.style.height = (imageSizes ? imageSizes.height : (isSmallItem ? 16 : 32)) + "px";
            StiMobileDesigner.setImageSource(img, this.options, imageName);
            cellImage.appendChild(img);
        }
        else {
            cellImage.style.width = "16px";
        }
    }

    if (captionText != null || typeof (captionText) == "undefined") {
        var caption = menuItem.caption = innerTable.addCell();
        caption.style.padding = "0 20px 0 5px";
        caption.style.textAlign = "left";
        caption.style.whiteSpace = "nowrap";
        if (captionText) caption.innerHTML = captionText;

        if (itemName && itemName.toString().indexOf("fontItem") == 0) {
            caption.style.fontSize = "16px";
            caption.style.fontFamily = key;
            caption.style.lineHeight = "1";
            menuItem.setAttribute("title", captionText);
            menuItem.style.overflow = "hidden";
        }
    }

    if (haveSubMenu) {
        var arrowImg = menuItem.arrowImg = document.createElement("img");
        arrowImg.style.width = arrowImg.style.height = "8px";
        arrowImg.style.margin = "0 4px 0 4px";

        var arrowCell = menuItem.arrowCell = innerTable.addCell();
        arrowCell.style.textAlign = "right";
        arrowCell.appendChild(arrowImg);
        StiMobileDesigner.setImageSource(arrowImg, this.options, "Arrows.SmallArrowRight.png");
    }

    menuItem.onmouseenter = function () {
        if (!this.isEnabled || jsObject.options.isTouchClick) return;
        this.className = overClass;
        this.isOver = true;
    }

    menuItem.onmouseleave = function () {
        if (!this.isEnabled) return;
        this.className = this.isSelected ? selectedClass : defaultClass;
        this.isOver = false;
    }

    menuItem.onmouseover = function () {
        if (!jsObject.options.isTouchDevice) this.onmouseenter();
    }

    menuItem.onmousedown = function () {
        if (this.isTouchStartFlag || !this.isEnabled) return;
        jsObject.options.menuItemPressed = this;
    }

    menuItem.onclick = function () {
        if (this.isTouchEndFlag || !this.isEnabled || jsObject.options.isTouchClick) return;
        this.action();
    }

    menuItem.ontouchstart = function () {
        this.isTouchStartFlag = true;
        clearTimeout(this.isTouchStartTimer);
        jsObject.options.fingerIsMoved = false;
        jsObject.options.menuItemPressed = this;

        this.isTouchStartTimer = setTimeout(function () {
            menuItem.isTouchStartFlag = false;
        }, 1000);
    }

    menuItem.ontouchend = function () {
        if (!this.isEnabled || jsObject.options.fingerIsMoved) return;
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);
        this.className = overClass;

        setTimeout(function () {
            menuItem.className = defaultClass;
            menuItem.action();
        }, 150);

        this.isTouchEndTimer = setTimeout(function () {
            menuItem.isTouchEndFlag = false;
        }, 1000);
    }

    menuItem.action = function () {
        menu.action(this);
    }

    menuItem.setEnabled = function (state) {
        this.isEnabled = state;
        if (this.image) this.image.style.opacity = state ? "1" : "0.3";
        if (this.arrowImg) this.arrowImg.style.opacity = state ? "1" : "0.3";
        this.className = state ? defaultClass : disabledClass;
    }

    menuItem.setSelected = function (state) {
        if (!state) {
            this.isSelected = false;
            this.className = this.isEnabled ? defaultClass : disabledClass;
            return;
        }
        if (menu.selectedItem != null) {
            menu.selectedItem.className = defaultClass;
            menu.selectedItem.isSelected = false;
        }
        this.className = this.isEnabled ? selectedClass : disabledClass;
        this.isSelected = true;
        menu.selectedItem = this;
    }

    return menuItem;
}

StiMobileDesigner.prototype.VerticalMenuSeparator = function (menu, name) {
    var menuSeparator = document.createElement("div");
    menuSeparator.className = "stiDesignerVerticalMenuSeparator";
    if (menu) menu.items[name] = menuSeparator;

    return menuSeparator;
}

StiMobileDesigner.prototype.VerticalMenuItemForStyles = function (menu, itemAttrs, style, imageSizes) {
    var item = this.VerticalMenuItem(menu, itemAttrs.name, itemAttrs.caption, itemAttrs.imageName, itemAttrs.key, style, null, imageSizes)
    item.itemAttrs = itemAttrs;

    var styleProperties = itemAttrs.styleProperties.type == "StiChartStyle" ? itemAttrs.styleProperties.properties : itemAttrs.styleProperties;

    //Override Styles
    item.style.margin = "2px";
    item.style.padding = "3px";
    item.style.overflow = "hidden";
    item.style.minWidth = "120px";
    item.innerTable.style.width = "100%";
    item.innerTable.style.boxSizing = "border-box";
    item.caption.style.padding = "0px 5px 0px 5px";

    var maxHeight = this.options.isTouchDevice ? 28 : 22;
    var capBlock = document.createElement("div");
    capBlock.style.maxHeight = maxHeight + "px";
    capBlock.style.maxWidth = "200px";
    capBlock.style.overflow = "hidden";
    capBlock.style.padding = "1px 0 1px 0";

    var captInnerCont = item.captInnerCont = document.createElement("div");
    capBlock.appendChild(captInnerCont);
    captInnerCont.innerHTML = item.caption.innerHTML;

    item.caption.innerHTML = "";
    item.caption.appendChild(capBlock);
    item.captInnerCont = captInnerCont;
    captInnerCont.style.position = "relative";

    if (styleProperties.font) {
        var captInnerContHeight = parseInt(styleProperties.font.split("!")[1]) * 1.33;
        captInnerCont.style.top = (captInnerContHeight > maxHeight ? ((captInnerContHeight - maxHeight) / 2 * -1) : 0) + "px";
    }

    this.RepaintControlByAttributes(item.innerTable, styleProperties.font, styleProperties.brush, styleProperties.textBrush, styleProperties.border);

    return item;
}

StiMobileDesigner.prototype.VerticalMenuItemForChartStyles = function (menu, properties) {
    var item = this.VerticalMenuItem(menu, properties.type + "_" + properties.name, properties.name, null, { type: properties.type, name: properties.name }, "stiDesignerMenuBigItem");
    item.style.height = "auto";

    var text = document.createElement("div");
    text.className = "stiStylesControlItem";
    text.style.margin = "-7px 0px 7px 10px";
    text.innerHTML = properties.name ? properties.name : properties.type.replace("Sti", "");

    item.caption.style.padding = "0px";
    item.caption.innerHTML = properties.image;
    item.caption.appendChild(text);

    return item;
}

StiMobileDesigner.prototype.VerticalMenuItemForGaugeStyles = function (menu, properties) {
    var item = this.VerticalMenuItem(menu, properties.type + "_" + properties.name, properties.name, null, { type: properties.type, name: properties.name }, "stiDesignerMenuBigItem");
    item.style.height = "auto";

    var text = document.createElement("div");
    text.className = "stiStylesControlItem";
    text.style.margin = "3px 0px 0px 3px";
    text.innerHTML = properties.name ? properties.name : properties.type.replace("Sti", "");

    item.caption.style.padding = "7px";
    item.caption.innerHTML = properties.image;
    item.caption.appendChild(text);

    return item;
}

StiMobileDesigner.prototype.VerticalMenuItemForMapStyles = function (menu, properties) {
    return this.VerticalMenuItemForGaugeStyles(menu, properties);
}

StiMobileDesigner.prototype.VerticalMenuItemForCrossTabStyles = function (menu, styleObject) {
    var item = this.VerticalMenuItem(menu, styleObject.properties.name, styleObject.properties.name, null, styleObject.key, "stiDesignerMenuBigItem");

    item.style.height = "auto";
    item.caption.innerHTML = "";
    item.caption.style.padding = "7px";

    item.tableContainer = document.createElement("div");
    item.caption.appendChild(item.tableContainer);
    item.tableContainer.appendChild(this.CrossTabSampleTable(118, 47, 13, 5, styleObject.properties.columnHeaderBackColor, styleObject.properties.rowHeaderBackColor));

    var text = document.createElement("div");
    text.className = "stiStylesControlItem";
    text.style.margin = "5px 0px 0px 3px";
    text.innerHTML = styleObject.properties.name;

    item.caption.appendChild(text);

    return item;
}

StiMobileDesigner.prototype.VerticalMenuItemForTableStyles = function (menu, styleObject) {
    var captionText = styleObject.properties.name || styleObject.styleId;

    var styleKey = {
        styleName: styleObject.properties.name,
        styleId: styleObject.styleId
    };

    var item = this.VerticalMenuItem(menu, captionText, captionText, null, styleKey, "stiDesignerMenuBigItem");

    item.style.height = "auto";
    item.caption.innerHTML = "";
    item.caption.style.padding = "7px";

    item.tableContainer = document.createElement("div");
    item.caption.appendChild(item.tableContainer);
    item.tableContainer.appendChild(this.SampleTable(118, 47, 13, 5, styleObject.properties.headerColor, styleObject.properties.footerColor, styleObject.properties.dataColor, styleObject.properties.gridColor));

    var text = document.createElement("div");
    text.className = "stiStylesControlItem";
    text.style.margin = "5px 0px 0px 3px";
    text.innerHTML = captionText;

    item.caption.appendChild(text);

    return item;
}

StiMobileDesigner.prototype.VerticalMenuItemForSparklineStyles = function (menu, properties) {
    var item = this.VerticalMenuItem(menu, properties.type + "_" + properties.name, properties.name, null, properties.name, "stiDesignerMenuBigItem");
    item.style.height = "auto";

    var text = document.createElement("div");
    text.className = "stiStylesControlItem";
    text.style.margin = "3px 0px 0px 3px";
    text.innerHTML = properties.name ? properties.name : properties.type.replace("Sti", "");

    item.caption.style.padding = "7px";
    item.caption.innerHTML = properties.image;
    item.caption.appendChild(text);

    return item;
}

StiMobileDesigner.prototype.CrossTabSampleTable = function (width, height, countColumns, countRows, columnHeaderColor, rowHeaderColor) {
    var table = this.CreateHTMLTable();
    table.style.width = width + "px";
    table.style.height = height + "px";
    table.style.borderCollapse = "collapse";

    var colHTMLColor = columnHeaderColor == "transparent" ? "transparent" : "rgb(" + this.RgbaToRgb(columnHeaderColor) + ")";
    var rowHTMLColor = rowHeaderColor == "transparent" ? "transparent" : "rgb(" + this.RgbaToRgb(rowHeaderColor) + ")";

    for (var row = 0; row < countRows; row++) {
        for (var col = 0; col < countColumns; col++) {
            var cell = table.addCellInLastRow();
            cell.style.border = "1px solid #d3d3d3";
            if (row == 0 && col != 0) cell.style.background = colHTMLColor;
            else if (col == 0 && row != 0) cell.style.background = rowHTMLColor;
            else cell.style.background = "#ffffff";
        }
        if (row < countRows - 1) table.addRow();
    }

    return table;
}

StiMobileDesigner.prototype.SampleTable = function (width, height, countColumns, countRows, headerColor, footerColor, dataColor, borderColor) {
    var table = this.CreateHTMLTable();
    table.style.width = width + "px";
    table.style.height = height + "px";
    table.style.borderCollapse = "collapse";

    var headerHTMLColor = headerColor == "transparent" ? "transparent" : "rgb(" + this.RgbaToRgb(headerColor) + ")";
    var footerHTMLColor = footerColor == "transparent" ? "transparent" : "rgb(" + this.RgbaToRgb(footerColor) + ")";
    var dataHTMLColor = dataColor == "transparent" ? "transparent" : "rgb(" + this.RgbaToRgb(dataColor) + ")";

    for (var row = 0; row < countRows; row++) {
        for (var col = 0; col < countColumns; col++) {
            var cell = table.addCellInLastRow();
            cell.style.border = "1px solid " + (borderColor ? "rgb(" + this.RgbaToRgb(borderColor) + ")" : "#d3d3d3");
            if (row == 0) cell.style.background = headerHTMLColor;
            else if (row == countRows - 1) cell.style.background = footerHTMLColor;
            else cell.style.background = dataHTMLColor;
        }
        if (row < countRows - 1) table.addRow();
    }

    return table;
}

StiMobileDesigner.prototype.RgbaToRgb = function (rgba) {
    var colorArray = rgba.split(",");
    if (colorArray.length == 4) return rgba.substring(rgba.indexOf(",") + 1);
    else return rgba;
}