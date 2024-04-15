
StiJsViewer.prototype.InitializeToolTip = function () {
    var toolTip = document.createElement("div");
    toolTip.id = this.controls.viewer.id + "ToolTip";
    var jsObject = toolTip.jsObject = this;
    this.controls.toolTip = toolTip;
    this.controls.mainPanel.appendChild(toolTip);
    toolTip.className = "stiJsViewerToolTip";
    toolTip.style.display = "none";
    toolTip.showTimer = null;
    toolTip.hideTimer = null;
    toolTip.visible = false;

    toolTip.innerTable = this.CreateHTMLTable();
    toolTip.appendChild(toolTip.innerTable);

    toolTip.textCell = toolTip.innerTable.addCell();
    toolTip.textCell.className = "stiJsViewerToolTipTextCell";

    if (this.options.appearance.showTooltipsHelp) {
        toolTip.helpButton = this.SmallButton(null, this.collections.loc["TellMeMore"], "HelpIcon.png", null, null, "stiJsViewerHyperlinkButton");
        toolTip.innerTable.addCellInNextRow(toolTip.helpButton);
        toolTip.helpButton.style.margin = "4px 8px 4px 8px";
    }
    else
        toolTip.textCell.style.border = 0;

    toolTip.show = function (text, helpUrl, leftPos, topPos, controlWidthForRightToLeft) {
        if ((this.visible && text == this.textCell.innerHTML) || jsObject.options.isTouchDevice) return;

        this.hide();

        if (jsObject.options.appearance.showTooltipsHelp) {
            this.helpButton.helpUrl = helpUrl;
            this.helpButton.action = function () {
                jsObject.showHelpWindow(this.helpUrl);
            }
        }

        if (this.ownerButton && this.ownerButton.styleColors) {
            this.style.background = this.ownerButton.styleColors.backColor;
            this.style.color = this.textCell.style.color = this.ownerButton.styleColors.isDarkStyle ? "#ffffff" : "#444444";
            if (this.helpButton) {
                this.helpButton.style.color = this.style.color;
            }
        }

        this.textCell.innerHTML = text;
        var d = new Date();
        var endTime = d.getTime() + 300;
        this.style.opacity = 1 / 100;
        this.style.display = "";

        var viewerLeft = jsObject.FindPosX(jsObject.controls.mainPanel);
        var viewerTop = jsObject.FindPosY(jsObject.controls.mainPanel);
        var browserWidth = (window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth);
        var browserHeight = (window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight);

        var left = controlWidthForRightToLeft != null ? leftPos - this.offsetWidth + controlWidthForRightToLeft : leftPos;
        if (left + this.offsetWidth > browserWidth - viewerLeft) left = browserWidth - viewerLeft - this.offsetWidth - 15;
        if (left < 0) left = 5;

        var top = topPos == "isNavigatePanelTooltip" ? jsObject.FindPosY(jsObject.controls.navigatePanel, "stiJsViewerMainPanel") - this.offsetHeight - 2 : topPos;
        if (top < 0) top = 5;
        if (top + this.offsetHeight > browserHeight - viewerTop) top = browserHeight - viewerTop - this.offsetHeight - 15;

        this.style.left = left + "px";
        this.style.top = top + "px";

        this.visible = true;
        jsObject.ShowAnimationForm(this, endTime);
    }

    toolTip.showWithDelay = function (text, helpUrl, leftPos, topPos, controlWidthForRightToLeft) {
        clearTimeout(this.showTimer);
        clearTimeout(this.hideTimer);
        var this_ = this;
        this.showTimer = setTimeout(function () {
            this_.show(text, helpUrl, leftPos, topPos, controlWidthForRightToLeft);
        }, 800);
    }

    toolTip.hide = function () {
        this.visible = false;
        clearTimeout(this.showTimer);
        this.style.display = "none";
        this.style.background = this.style.color = this.textCell.style.color = "";

        if (this.helpButton) {
            this.helpButton.style.color = "";
        }
    }

    toolTip.hideWithDelay = function () {
        clearTimeout(this.showTimer);
        clearTimeout(this.hideTimer);
        var this_ = this;
        this.hideTimer = setTimeout(function () {
            this_.hide();
        }, 500);
    }

    toolTip.onmouseover = function () {
        clearTimeout(this.showTimer);
        clearTimeout(this.hideTimer);
    }

    toolTip.onmouseout = function () {
        this.hideWithDelay();
    }
}