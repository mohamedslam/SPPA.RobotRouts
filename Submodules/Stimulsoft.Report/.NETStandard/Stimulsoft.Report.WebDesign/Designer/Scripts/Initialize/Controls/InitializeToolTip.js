
StiMobileDesigner.prototype.InitializeToolTip = function () {
    var toolTip = document.createElement("div");
    var jsObject = toolTip.jsObject = this;
    toolTip.id = this.options.mobileDesigner.id + "ToolTip";
    toolTip.className = "stiDesignerToolTip";
    toolTip.style.display = "none";
    toolTip.showTimer = null;
    toolTip.hideTimer = null;
    toolTip.visible = false;

    this.options.toolTip = toolTip;
    this.options.mainPanel.appendChild(toolTip);

    toolTip.innerTable = this.CreateHTMLTable();
    toolTip.appendChild(toolTip.innerTable);

    toolTip.textCell = toolTip.innerTable.addCell();
    toolTip.textCell.className = "stiDesignerToolTipTextCell";

    toolTip.helpButton = this.SmallButton("ToolTipHelpButton", null, this.loc.HelpDesigner.TellMeMore, "HelpIcon2.png", null, null, "stiDesignerHyperlinkButton", true);
    toolTip.innerTable.addCellInNextRow(toolTip.helpButton);
    toolTip.helpButton.style.margin = "4px 8px 4px 8px";
    toolTip.helpButton.style.display = this.options.showTooltipsHelp ? "" : "none";
    if (!this.options.showTooltipsHelp) toolTip.textCell.style.borderBottom = "0px";

    toolTip.show = function (text, helpUrl, leftPos, topPos) {
        if (this.visible && text == this.textCell.innerHTML) return;
        this.hide();
        this.textCell.innerHTML = text;
        this.helpButton.helpUrl = helpUrl;
        this.helpButton.action = function () { jsObject.ShowHelpWindow(this.helpUrl); }
        this.style.left = leftPos + "px";
        this.style.top = topPos + "px";
        var d = new Date();
        var endTime = d.getTime() + 300;
        this.style.opacity = 1 / 100;
        this.style.display = "";
        this.visible = true;
        jsObject.ShowAnimationForm(this, endTime);
    }

    toolTip.showWithDelay = function (text, helpUrl, leftPos, topPos) {
        clearTimeout(this.showTimer);
        clearTimeout(this.hideTimer);
        toolTip.showTimer = setTimeout(function () { jsObject.options.toolTip.show(text, helpUrl, leftPos, topPos); }, 300);
    }

    toolTip.hide = function () {
        this.visible = false;
        clearTimeout(this.showTimer);
        this.style.display = "none";
    }

    toolTip.hideWithDelay = function () {
        clearTimeout(this.showTimer);
        clearTimeout(this.hideTimer);
        toolTip.hideTimer = setTimeout(function () { jsObject.options.toolTip.hide(); }, 500);
    }

    toolTip.onmouseover = function () {
        clearTimeout(this.showTimer);
        clearTimeout(this.hideTimer);
    }

    toolTip.onmouseout = function () {
        this.hideWithDelay();
    }
}

StiMobileDesigner.prototype.GetTooltipImageSize = function (componentType) {
    var sizes = {
        "StiChartElement": [85, 84],
        "StiComboBoxElement": [99, 21],
        "StiDatePickerElement": [126, 20],
        "StiGaugeElement": [93, 48],
        "StiImageElement": [75, 65],
        "StiIndicatorElement": [80, 80],
        "StiListBoxElement": [91, 81],
        "StiOnlineMapElement": [100, 59],
        "StiPanelElement": [103, 64],
        "StiPivotTableElement": [100, 51],
        "StiProgressElement": [80, 80],
        "StiRegionMapElement": [100, 59],
        "StiShapeElement": [74, 52],
        "StiTableElement": [100, 51],
        "StiTextElement": [100, 24],
        "StiTreeViewBoxElement": [126, 101],
        "StiTreeViewElement": [107, 138]
    }

    return (componentType && sizes[componentType] ? sizes[componentType] : [32, 32]);
}