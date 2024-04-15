
StiMobileDesigner.prototype.VerticalMenuItemForDashboardStyles = function (menu, styleObject) {
    var itemName = styleObject.name || styleObject.ident;
    var itemLocName = styleObject.localizedName || itemName;
    var itemKey = { name: styleObject.name, ident: styleObject.ident };
    var item = this.VerticalMenuItem(menu, itemName, itemLocName, null, itemKey, "stiDesignerMenuBigItem");

    item.style.height = "auto";
    item.style.overflow = "hidden";
    item.caption.style.padding = "8px";
    item.caption.innerHTML = "";

    item.styleContainer = document.createElement("div");
    item.caption.appendChild(item.styleContainer);

    var styleTable = this.CreateHTMLTable();
    styleTable.style.width = "120px";
    item.styleContainer.appendChild(styleTable);

    var sampleText = styleTable.addTextCell("Aa");
    sampleText.style.height = "23px";
    sampleText.style.fontSize = "18px";
    sampleText.style.textAlign = "center";
    sampleText.style.color = styleObject.foreColor;
    sampleText.style.background = styleObject.backColor;
    sampleText.style.border = "1px solid " + styleObject.titleBackColor;

    var text = styleTable.addTextCellInNextRow(itemLocName);
    text.style.height = "18px";
    text.style.fontSize = "12px";
    text.style.textAlign = "center";
    text.style.color = styleObject.titleForeColor;
    text.style.background = styleObject.titleBackColor;
    text.style.border = sampleText.style.border;

    return item;
}

StiMobileDesigner.prototype.VerticalMenuItemForTableOrPivotTableElementsStyles = function (menu, styleObject, isPivotTableElement) {
    var itemName = styleObject.name || styleObject.ident;
    var itemLocName = styleObject.localizedName || itemName;
    var itemKey = { name: styleObject.name, ident: styleObject.ident };
    var item = this.VerticalMenuItem(menu, itemName, itemLocName, null, itemKey, "stiDesignerMenuBigItem");

    item.style.height = "auto";
    item.style.overflow = "hidden";
    item.caption.style.padding = "8px";
    item.caption.innerHTML = "";

    item.styleContainer = document.createElement("div");
    item.caption.appendChild(item.styleContainer);

    if (styleObject.ident == "Auto") {
        item.styleContainer.innerHTML = "Auto"
        item.styleContainer.style.fontSize = "12px";
        item.styleContainer.style.border = "1px dashed #c6c6c6";
        item.styleContainer.style.padding = "10px";
        item.styleContainer.style.textAlign = "center";
    }
    else {
        var styleTable = this.CreateHTMLTable();
        styleTable.style.width = "119px";
        styleTable.style.border = "1px solid " + styleObject.lineColor;
        styleTable.style.borderRight = "0px";
        item.styleContainer.appendChild(styleTable);

        for (var i = 0; i < 5; i++) {
            for (var k = 0; k < 3; k++) {
                var cell = styleTable.addCellInLastRow();
                cell.style.height = "10px";
                cell.style.borderRight = "1px solid " + styleObject.lineColor;
                cell.style.background = i == 0 || (isPivotTableElement && k == 0)
                    ? ((isPivotTableElement && k == 0)
                        ? styleObject.rowHeaderBackColor
                        : (isPivotTableElement ? styleObject.columnHeaderBackColor : styleObject.headerBackColor))
                    : (i == 1 || i == 3)
                        ? styleObject.cellBackColor
                        : styleObject.alternatingCellBackColor;

                if (isPivotTableElement && k == 0 && i < 4) {
                    cell.style.borderBottom = "1px solid " + styleObject.lineColor;
                }
            }
            if (i < 4) styleTable.addRow();
        }

        var text = document.createElement("div");
        item.text = text;
        text.className = "stiStylesControlItem";
        text.style.margin = "5px 0px 0px 3px";
        text.innerHTML = itemLocName;

        item.caption.appendChild(text);
    }

    return item;
}

StiMobileDesigner.prototype.VerticalMenuItemForFilterElementStyles = function (menu, styleObject) {
    var itemName = styleObject.name || styleObject.ident;
    var itemLocName = styleObject.localizedName || itemName;
    var itemKey = { name: styleObject.name, ident: styleObject.ident };
    var item = this.VerticalMenuItem(menu, itemName, itemLocName, null, itemKey, "stiDesignerMenuBigItem");
    item.style.height = "auto";
    item.style.overflow = "hidden";
    item.caption.style.padding = "4px";
    item.caption.innerHTML = "";

    item.styleContainer = document.createElement("div");
    item.caption.appendChild(item.styleContainer);

    if (styleObject.ident == "Auto") {
        item.styleContainer.innerHTML = "Auto";
        item.styleContainer.style.fontSize = "12px";
        item.styleContainer.style.border = "1px dashed #c6c6c6";
        item.styleContainer.style.padding = "10px";
        item.styleContainer.style.textAlign = "center";
    }
    else {
        var control = this.CreateHTMLTable();
        control.style.background = styleObject.backColor;
        control.style.color = styleObject.foreColor;
        control.style.border = "1px solid #c6c6c6";
        control.style.fontFamily = "Arial";
        control.style.fontSize = "12px";

        var text = document.createElement("div");
        text.style.width = "91px";
        text.style.marginLeft = "5px";
        text.style.textAlign = "left";
        text.style.overflow = "hidden";
        text.style.textOverflow = "ellipsis";
        text.innerHTML = itemLocName;
        control.addCell(text).style.height = "30px";
        var buttonCell = control.addTextCell("&#9660;");
        buttonCell.style.padding = "0 5px 0 5px";
        buttonCell.style.background = styleObject.selectedBackColor;

        item.styleContainer.appendChild(control);
        item.caption.style.padding = "12px";
    }

    return item;
}

StiMobileDesigner.prototype.VerticalMenuItemForDasboardElementStyles = function (menu, styleObject) {
    var itemName = styleObject.name || styleObject.ident;
    var itemLocName = styleObject.localizedName || itemName;
    var itemKey = { name: styleObject.name, ident: styleObject.ident };
    var item = this.VerticalMenuItem(menu, itemName, itemLocName, null, itemKey, "stiDesignerMenuBigItem");
    item.style.height = "auto";
    item.style.overflow = "hidden";
    item.caption.style.padding = "4px";
    item.caption.innerHTML = "";

    item.styleContainer = document.createElement("div");
    item.caption.appendChild(item.styleContainer);

    if (styleObject.ident == "Auto") {
        item.styleContainer.innerHTML = "Auto"
        item.styleContainer.style.fontSize = "12px";
        item.styleContainer.style.border = "1px dashed #c6c6c6";
        item.styleContainer.style.padding = "10px";
        item.styleContainer.style.textAlign = "center";
    }
    else {
        item.styleContainer.innerHTML = styleObject.image;

        var text = document.createElement("div");
        item.text = text;
        text.className = "stiStylesControlItem";
        text.style.margin = "4px 0px 3px 3px";
        text.innerHTML = itemLocName;

        item.caption.appendChild(text);
    }

    return item;
}

StiMobileDesigner.prototype.VerticalMenuItemForRegionMapElementStyles = function (menu, styleObject) {
    var item = this.VerticalMenuItemForDasboardElementStyles(menu, styleObject);

    return item;
}

StiMobileDesigner.prototype.VerticalMenuItemForChartElementStyles = function (menu, styleObject) {
    var item = this.VerticalMenuItemForDasboardElementStyles(menu, styleObject);
    if (item.caption && styleObject.ident != "Auto") {
        item.caption.style.padding = "0";
        if (item.text) item.text.style.margin = "-7px 0px 5px 10px";
    }

    return item;
}

StiMobileDesigner.prototype.VerticalMenuItemForGaugeElementStyles = function (menu, styleObject) {
    var item = this.VerticalMenuItemForDasboardElementStyles(menu, styleObject);

    return item;
}

StiMobileDesigner.prototype.VerticalMenuItemForProgressElementStyles = function (menu, styleObject) {
    var item = this.VerticalMenuItemForDasboardElementStyles(menu, styleObject);

    return item;
}

StiMobileDesigner.prototype.VerticalMenuItemForIndicatorElementStyles = function (menu, styleObject) {
    var item = this.VerticalMenuItemForDasboardElementStyles(menu, styleObject);

    return item;
}

StiMobileDesigner.prototype.VerticalMenuItemForCardsElementStyles = function (menu, styleObject) {
    var item = this.VerticalMenuItemForDasboardElementStyles(menu, styleObject);

    return item;
}