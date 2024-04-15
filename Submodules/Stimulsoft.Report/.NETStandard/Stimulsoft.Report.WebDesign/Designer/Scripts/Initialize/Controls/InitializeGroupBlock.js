
StiMobileDesigner.prototype.GroupBlock = function (name, caption, haveButton, buttonToolTip) {
    var groupBlock = document.createElement("div");
    groupBlock.jsObject = this;
    this.options.controls[name] = groupBlock;
    groupBlock.name = name;
    groupBlock.className = "stiDesignerGroupBlock";

    groupBlock.container = document.createElement("div");
    groupBlock.appendChild(groupBlock.container);
    groupBlock.container.className = (caption != null || typeof (caption) == "undefined") ? "stiDesignerGroupBlockContainer" : "stiDesignerGroupBlockContainerWithOutCaption";

    if (caption == null && typeof (caption) != "undefined") return groupBlock;

    groupBlock.downRow = document.createElement("div");
    groupBlock.appendChild(groupBlock.downRow);
    groupBlock.downRow.className = "stiDesignerGroupBlockDownRow";
    groupBlock.downRow.style.height = this.options.isTouchDevice ? "20px" : "15px";

    var innerTable = this.CreateHTMLTable();
    groupBlock.downRow.appendChild(innerTable);
    innerTable.style.width = "100%";
    innerTable.style.height = "100%";

    groupBlock.caption = innerTable.addCell();
    groupBlock.caption.className = "stiDesignerGroupBlockCaption";
    if (caption) groupBlock.caption.innerHTML = caption;

    if (haveButton) {
        var button = this.GroupBlockButton(name + "Button", buttonToolTip);
        innerTable.addCell(button);
        groupBlock.button = button;
    }

    return groupBlock;
}

StiMobileDesigner.prototype.GroupBlockButton = function (name, toolTip) {
    var button = this.SmallButton(name, null, null, this.options.isTouchDevice ? "GroupBlockButtonTouch.png" : "GroupBlockButton.png", toolTip, null, "stiDesignerGroupBlockButton", null,
        this.options.isTouchDevice ? { width: 12, height: 12 } : { width: 8, height: 8 });
    button.image.parentElement.style.padding = "";
    button.image.parentElement.style.textAlign = "center";
    button.childNodes[0].style.width = "100%";

    return button;
}

StiMobileDesigner.prototype.GroupBlockSeparator = function (name) {
    var separator = document.createElement("div");
    if (name) this.options.controls[name] = separator;
    separator.className = "stiDesignerGroupBlockSeparator";

    return separator;
}

StiMobileDesigner.prototype.GroupBlockInnerTable = function () {
    var table = this.CreateHTMLTable();

    table.addCell = function (control) {
        var cell = document.createElement("td");
        cell.className = "stiDesignerGroupBlockTableCell";
        this.tr[0].appendChild(cell);
        if (control) cell.appendChild(control);

        return cell;
    }

    return table;
}