
StiJsViewer.prototype.CreatePivotTableElementContent = function (element) {
    var jsObject = this;

    var elementAttrs = element.elementAttributes;
    var contentAttrs = elementAttrs.contentAttributes;
    if (!contentAttrs) return;

    var panel = document.createElement("div");
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

    var dataGrid = jsObject.PivotTable(gridWidth, gridHeight, contentAttrs.settings);
    panel.appendChild(dataGrid);

    dataGrid.showData(contentAttrs.data);
}