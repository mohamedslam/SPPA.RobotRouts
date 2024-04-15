
StiJsViewer.prototype.InitializeDrillDownPanel = function () {
    var drillDownPanel = document.createElement("div");
    this.controls.drillDownPanel = drillDownPanel;
    this.controls.mainPanel.appendChild(drillDownPanel);
    var jsObject = drillDownPanel.jsObject = this;
    drillDownPanel.className = "stiJsViewerToolBar";
    if (this.options.toolbar.displayMode == "Separated") drillDownPanel.className += " stiJsViewerToolBarSeparated";
    drillDownPanel.style.display = "none";
    drillDownPanel.visible = false;

    var drillDownInnerContent = document.createElement("div");
    drillDownPanel.appendChild(drillDownInnerContent);
    if (this.options.toolbar.displayMode == "Simple") drillDownInnerContent.style.paddingTop = "2px";

    var drillDownInnerTable = this.CreateHTMLTable();
    drillDownInnerTable.className = "stiJsViewerToolBarTable";
    if (this.options.toolbar.displayMode == "Separated") drillDownInnerTable.style.border = "0px";
    drillDownInnerContent.appendChild(drillDownInnerTable);
    drillDownInnerTable.style.margin = "0";
    if (this.options.toolbar.fontColor != "") drillDownInnerTable.style.color = this.options.toolbar.fontColor;
    drillDownInnerTable.style.fontFamily = this.options.toolbar.fontFamily;
    drillDownInnerTable.style.boxSizing = "border-box";

    var buttonsTable = this.CreateHTMLTable();
    drillDownInnerTable.addCell(buttonsTable);

    drillDownPanel.buttonsRow = buttonsTable.rows[0];
    drillDownPanel.buttons = {};
    drillDownPanel.selectedButton = null;

    drillDownPanel.changeVisibleState = function (state) {
        var isStateChanged = this.visible != state;
        this.style.display = state ? "" : "none";
        this.visible = state;
        if (isStateChanged) jsObject.updateLayout();
    }

    drillDownPanel.addButton = function (caption, reportParams, drillDownParameters) {
        var name = "button" + (drillDownPanel.buttonsRow.children.length + 1);
        var button = jsObject.SmallButton(null, caption);
        button.name = name;
        button.style.display = "inline-block";
        button.reportParams = reportParams ? reportParams : this.reportParams = {};
        button.style.margin = "2px 1px 2px 2px";
        if (jsObject.options.toolbar.displayMode == "Separated") button.style.height = "28px";

        if (name != "button1" && drillDownParameters && drillDownParameters.length > 0 && drillDownParameters[drillDownParameters.length - 1].DrillDownMode == "SinglePage") {
            var removedButtons = [];
            for (var i = 0; i < drillDownPanel.buttonsRow.childNodes.length; i++) {
                var btn = drillDownPanel.buttonsRow.childNodes[i].firstChild;
                if (btn.name != "button1") {
                    removedButtons.push(btn);
                    delete drillDownPanel.buttons[btn.name];
                }
            }
            for (var i = 0; i < removedButtons.length; i++) {
                drillDownPanel.buttonsRow.removeChild(removedButtons[i].parentElement);
            }
        }

        drillDownPanel.buttons[name] = button;

        var cell = buttonsTable.addCell(button);
        cell.style.padding = "0";
        cell.style.border = "0";
        cell.style.lineHeight = "0";

        button.select = function () {
            if (drillDownPanel.selectedButton) drillDownPanel.selectedButton.setSelected(false);
            this.setSelected(true);
            drillDownPanel.selectedButton = this;
            jsObject.reportParams = this.reportParams;
            jsObject.controls.reportPanel.scrollTop = 0;

            if (jsObject.controls.dashboardsPanel.selectedButton)
                jsObject.controls.dashboardsPanel.selectedButton.reportParams = this.reportParams;
        }

        button.action = function () {
            if (this.style.display != "none") {
                var reportScrollPos = jsObject.controls.reportPanel.scrollTop;
                if (drillDownPanel.selectedButton && reportScrollPos) {
                    drillDownPanel.selectedButton.reportScrollPos = reportScrollPos;
                }
                this.select();
                jsObject.postAction("GetPages");
                jsObject.options.isParametersReceived = false;
            }
        };

        button.select();

        if (name != "button1") {
            var closeButton = jsObject.SmallButton(null, null, "CloseForm.png");
            closeButton.image.style.margin = "0 2px 0 2px";
            closeButton.style.display = "inline-block";
            closeButton.style.margin = "0 2px 0 0";
            closeButton.image.style.margin = "1px 0 0 -1px";
            closeButton.imageCell.style.padding = 0;
            closeButton.style.width = jsObject.options.isTouchDevice ? "22px" : "17px";
            closeButton.style.height = closeButton.style.width;
            closeButton.reportButton = button;
            button.innerTable.addCell(closeButton);

            closeButton.action = function () {
                this.reportButton.style.display = "none";
                if (this.reportButton.isSelected) drillDownPanel.buttons["button1"].action();
            };

            closeButton.onmouseenter = function (event) {
                this.reportButton.onmouseoutAction();
                this.onmouseoverAction();
                if (event) event.stopPropagation();
            }
        }

        return button;
    }

    drillDownPanel.reset = function () {
        if (buttonsTable.tr[0].childNodes.length > 0) {
            drillDownPanel.buttons = {};
            while (buttonsTable.tr[0].childNodes.length > 0) {
                buttonsTable.tr[0].removeChild(buttonsTable.tr[0].childNodes[buttonsTable.tr[0].childNodes.length - 1]);
            }
        }
        drillDownPanel.changeVisibleState(false);
    }

    return drillDownPanel;
}