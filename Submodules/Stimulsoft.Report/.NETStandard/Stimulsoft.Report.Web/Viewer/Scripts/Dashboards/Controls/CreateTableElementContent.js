
StiJsViewer.prototype.CreateTableElementContent = function (element) {
    var jsObject = this;

    var elementAttrs = element.elementAttributes;
    var contentAttrs = elementAttrs.contentAttributes;
    if (!contentAttrs) return;

    var panel = document.createElement("div");
    panel.setAttribute("style", "-webkit-user-select: none; -moz-user-select: none; -ms-user-select: none; user-select: none;");
    panel.style.position = "absolute";
    panel.style.left = panel.style.top = panel.style.right = panel.style.bottom = "0px";
    panel.style.overflow = "hidden";
    element.contentPanel.appendChild(panel);

    var padding = elementAttrs.padding.split(",");
    padding = jsObject.CorrectPaddingsByCornerRadius(element, padding);

    var gridWidth = parseInt(element.style.width) - parseInt(padding[1]) - parseInt(padding[3]);
    var gridHeight = parseInt(element.style.height) - parseInt(padding[0]) - parseInt(padding[2]);

    var border = elementAttrs.border;
    if (border) {
        var horBorders = (border.left ? border.size : 0) + (border.right ? border.size : 0);
        var vertBorders = (border.top ? border.size : 0) + (border.bottom ? border.size : 0);
        gridWidth -= horBorders;
        gridHeight -= vertBorders;
    }

    if (element.titlePanel) {
        gridHeight -= element.titlePanel.offsetHeight;
    }

    var dataGrid = jsObject.DataGrid(gridWidth, gridHeight, contentAttrs.settings, elementAttrs, element);
    element.dataGrid = dataGrid;
    panel.appendChild(dataGrid);

    jsObject.hideDocToolTip();

    dataGrid.showData(contentAttrs.data, contentAttrs.hiddenData);

    dataGrid.action = function (headerButton) {
        var menuStyle = (elementAttrs.actionColors && elementAttrs.actionColors.isDarkStyle) ? "stiJsViewerDbsDarkMenu" : "stiJsViewerDbsLightMenu";

        var sortFilterMenu = jsObject.SortFilterMenu(headerButton, dataGrid, element, menuStyle + "Item", menuStyle);
        sortFilterMenu.changeVisibleState(true);
        this.selectedHeaderButton = headerButton;
    }
}

StiJsViewer.prototype.CorrectPaddingsByCornerRadius = function (element, padding) {
    var corners = element.elementAttributes.cornerRadius;

    if (!corners || (corners.topLeft == 0 && corners.topRight == 0 && corners.bottomLeft == 0 && corners.bottomRight == 0))
        return padding;

    var radFactor = 1.3;
    var leftOffset = Math.max(corners.topLeft / radFactor, corners.bottomLeft / radFactor);
    var topOffset = Math.max(corners.topLeft / radFactor, corners.topRight / radFactor);
    var rightOffset = Math.max(corners.topRight / radFactor, corners.bottomRight / radFactor);
    var bottomOffset = Math.max(corners.bottomLeft / radFactor, corners.bottomRight / radFactor);

    var paddingTop = parseInt(padding[0]);
    var paddingRight = parseInt(padding[1]);
    var paddingBottom = parseInt(padding[2]);
    var paddingLeft = parseInt(padding[3]);

    padding[0] = paddingTop > topOffset ? paddingTop : topOffset - paddingTop;
    padding[1] = paddingRight > rightOffset ? paddingRight : rightOffset - paddingRight;
    padding[2] = paddingBottom > bottomOffset ? paddingBottom : bottomOffset - paddingBottom;
    padding[3] = paddingLeft > leftOffset ? paddingLeft : leftOffset - paddingLeft;

    return padding;
}