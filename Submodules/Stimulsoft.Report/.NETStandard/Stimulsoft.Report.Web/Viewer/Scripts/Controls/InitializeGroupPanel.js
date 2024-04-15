
StiJsViewer.prototype.GroupPanel = function (caption, isOpened, width, innerPadding) {
    var groupPanel = document.createElement("div");
    groupPanel.style.fontFamily = this.options.toolbar.fontFamily;
    groupPanel.style.color = this.options.toolbarFontColor;
    var jsObject = groupPanel.jsObject = this;

    if (width) groupPanel.style.minWidth = width + "px";
    groupPanel.style.overflow = "hidden";
    groupPanel.isOpened = isOpened;

    var header = this.FormButton(null, caption, isOpened ? "Arrows.BigArrowDown.png" : "Arrows.BigArrowRight.png", null, "stiJsViewerSmallButtonWithBorder");
    header.imageCell.style.width = "1px";
    header.style.borderBottomLeftRadius = isOpened ? "0" : "";
    header.style.borderBottomRightRadius = isOpened ? "0" : "";
    groupPanel.appendChild(header);

    if (header.caption) {
        header.caption.style.textAlign = "left";
        header.caption.style.padding = "0 15px 0 5px";
    }

    var container = groupPanel.container = document.createElement("div");
    if (innerPadding) container.style.padding = innerPadding;
    container.style.display = isOpened ? "" : "none";
    container.className = "stiJsViewerGroupPanelContainer";
    groupPanel.appendChild(container);

    groupPanel.changeOpeningState = function (state) {
        groupPanel.isOpened = state;
        StiJsViewer.setImageSource(header.image, jsObject.options, jsObject.collections, state ? "Arrows.BigArrowDown.png" : "Arrows.BigArrowRight.png");
        container.style.display = state ? "" : "none";
        header.style.borderBottomLeftRadius = groupPanel.isOpened ? "0" : "";
        header.style.borderBottomRightRadius = groupPanel.isOpened ? "0" : "";
    }

    header.action = function () {
        groupPanel.isOpened = !groupPanel.isOpened;
        StiJsViewer.setImageSource(header.image, jsObject.options, jsObject.collections, groupPanel.isOpened ? "Arrows.BigArrowDown.png" : "Arrows.BigArrowRight.png");
        groupPanel.style.height = (groupPanel.isOpened ? header.offsetHeight : header.offsetHeight + container.offsetHeight) + "px";

        if (groupPanel.isOpened)
            container.style.display = "";

        header.style.borderBottomLeftRadius = groupPanel.isOpened ? "0" : "";
        header.style.borderBottomRightRadius = groupPanel.isOpened ? "0" : "";

        jsObject.animate(groupPanel, {
            duration: 150,
            animations: [{
                style: "height",
                start: groupPanel.isOpened ? header.offsetHeight : header.offsetHeight + container.offsetHeight,
                end: groupPanel.isOpened ? header.offsetHeight + container.offsetHeight : header.offsetHeight,
                postfix: "px",
                finish: function () {
                    container.style.display = groupPanel.isOpened ? "" : "none";
                    groupPanel.style.height = "";
                }
            }]
        });
    }

    return groupPanel;
}