
StiMobileDesigner.prototype.InitializeCheckExpressionPopupPanel_ = function () {
    var panel = document.createElement("div");
    panel.jsObject = this;

    //Add Arrow
    var arrow = document.createElement("div");
    arrow.className = "stiDesignerCheckPopupPanelArrow";
    panel.appendChild(arrow);

    if (this.options.checkPopupPanel) {
        this.options.checkPopupPanel.hide();
    }

    this.options.checkPopupPanel = panel;
    this.options.mainPanel.appendChild(panel);
    panel.className = "stiDesignerCheckPopupPanel";
    panel.style.minWidth = "150px";
    panel.style.visibility = "hidden";
    panel.jsObject = this;

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "10px 20px 10px 10px";
    panel.appendChild(innerTable);

    var img = document.createElement("img");
    img.style.width = img.style.height = "32px";
    img.style.marginRight = "10px";
    innerTable.addCell(img);

    var textCell = innerTable.addCell();

    panel.hide = function () {
        this.jsObject.options.mainPanel.removeChild(this);
        this.jsObject.options.checkPopupPanel = null;
        clearTimeout(this.hideTimer);
    }

    panel.show = function (messageText, parentButton) {
        StiMobileDesigner.setImageSource(img, this.jsObject.options, "ReportChecker." + (messageText == "OK" ? "Information32.png" : "Error32.png"));
        textCell.innerHTML = messageText;

        setTimeout(function () { //fix a bug with arrow
            panel.style.left = (panel.jsObject.FindPosX(parentButton, "stiDesignerMainPanel") + 5) + "px";
            panel.style.top = (panel.jsObject.FindPosY(parentButton, "stiDesignerMainPanel") - panel.offsetHeight - 10) + "px";
            arrow.style.top = (panel.offsetHeight - 2) + "px";
            panel.style.visibility = "visible";
        }, 100);
    }

    //Hide after 15 sec.
    panel.hideTimer = setTimeout(function () {
        if (panel) {
            panel.hide();
        }
    }, 12000);

    return panel;
}