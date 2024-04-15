
StiJsViewer.prototype.InitializeFindPanel = function () {
    var findPanel = document.createElement("div");
    findPanel.style.display = "none";
    findPanel.visible = false;
    findPanel.controls = {};
    this.controls.findPanel = findPanel;
    this.controls.mainPanel.appendChild(findPanel);
    findPanel.jsObject = this;
    findPanel.className = "stiJsViewerToolBar";
    if (this.options.toolbar.displayMode == "Separated") findPanel.className += " stiJsViewerToolBarSeparated";

    var findPanelInnerContent = document.createElement("div");
    findPanel.innerContent = findPanelInnerContent;
    findPanel.appendChild(findPanelInnerContent);
    if (this.options.toolbar.displayMode == "Simple") findPanelInnerContent.style.paddingTop = "2px";

    var findPanelBlock = document.createElement("div");
    findPanelInnerContent.appendChild(findPanelBlock);
    findPanelBlock.className = "stiJsViewerToolBarTable";
    if (this.options.toolbar.displayMode == "Separated") findPanelBlock.style.border = "0px";
    findPanelBlock.style.boxSizing = "border-box";

    var controlsTable = this.CreateHTMLTable();
    findPanelBlock.appendChild(controlsTable);
    controlsTable.style.margin = "0";

    if (this.options.appearance.rightToLeft) {
        findPanelBlock.style.textAlign = "right";
        controlsTable.style.display = "inline-block";
    }

    var controlProps = [
        ["close", this.SmallButton(null, null, "CloseForm.png", null), "2px"],
        ["text", this.TextBlock(this.collections.loc.FindWhat.replace(":", "")), "2px"],
        ["findTextBox", this.TextBox(null, 170), "2px"],
        ["findPreviows", this.SmallButton(null, this.collections.loc.FindPrevious, "Arrows.ArrowUpBlue.png"), "2px"],
        ["findNext", this.SmallButton(null, this.collections.loc.FindNext, "Arrows.ArrowDownBlue.png"), "2px"],
        ["matchCase", this.SmallButton(null, this.collections.loc.MatchCase.replace("&", ""), null), "2px"],
        ["matchWholeWord", this.SmallButton(null, this.collections.loc.MatchWholeWord.replace("&", ""), null), "2px"]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        findPanel.controls[controlProps[i][0]] = controlProps[i][1];
        controlsTable.addCell(controlProps[i][1]);
        controlProps[i][1].style.margin = controlProps[i][2];
        if (this.options.toolbar.displayMode == "Separated" && controlProps[i][0] != "text" && controlProps[i][0] != "findTextBox") {
            controlProps[i][1].style.height = "28px";
        }
    }

    var find = function (direction) {
        if (findPanel.controls.findTextBox.value == "") {
            findPanel.jsObject.hideFindLabels();
            return;
        }
        if (findPanel.jsObject.controls.findHelper.lastFindText != findPanel.controls.findTextBox.value || findPanel.jsObject.options.changeFind)
            findPanel.jsObject.showFindLabels(findPanel.controls.findTextBox.value);
        else
            findPanel.jsObject.selectFindLabel(direction);
    }

    findPanel.controls.close.action = function () { findPanel.changeVisibleState(false); }
    findPanel.controls.findTextBox.onkeyup = function (e) { if (e && e.keyCode == 13) find("Next"); }
    findPanel.controls.matchCase.action = function () {
        this.setSelected(!this.isSelected);
        this.jsObject.options.changeFind = true;
    }
    findPanel.controls.matchWholeWord.action = function () {
        this.setSelected(!this.isSelected);
        this.jsObject.options.changeFind = true;
    }
    findPanel.controls.findPreviows.action = function () { find("Previows"); }
    findPanel.controls.findNext.action = function () { find("Next"); }

    findPanel.changeVisibleState = function (state) {
        var isStateChanged = this.visible != state;
        this.style.display = state ? "" : "none";

        if (state) {
            if (!this.visible) {
                if (findPanel.jsObject.controls.findHelper) {
                    findPanel.jsObject.controls.findHelper.lastFindText = "";
                }
                findPanel.controls.findTextBox.value = "";
                findPanel.controls.findTextBox.focus();
            }
        }
        else {
            this.jsObject.hideFindLabels();
        }

        this.visible = state;

        if (this.jsObject.options.toolbar.showFindButton) this.jsObject.controls.toolbar.controls.Find.setSelected(state);

        if (isStateChanged) this.jsObject.updateLayout();
    }
}

