
StiJsViewer.prototype.InitializeDisabledPanels = function () {
    this.controls.disabledPanels = {};
    for (var i = 1; i < 5; i++) {
        var disabledPanel = document.createElement("div");
        disabledPanel.jsObject = this;
        disabledPanel.style.display = "none";
        this.controls.mainPanel.appendChild(disabledPanel);
        this.controls.disabledPanels[i] = disabledPanel;
        disabledPanel.style.zIndex = 10 * i;
        disabledPanel.className = "stiJsViewerDisabledPanel";

        disabledPanel.changeVisibleState = function (state) {
            this.style.display = state ? "" : "none";
        }

        disabledPanel.onmousedown = function () {
            if (!this.isTouchStartFlag) disabledPanel.ontouchstart(true);
        }

        disabledPanel.ontouchstart = function (mouseProcess) {
            var this_ = this;
            this.isTouchStartFlag = mouseProcess ? false : true;
            clearTimeout(this.isTouchStartTimer);
            disabledPanel.jsObject.options.disabledPanelPressed = true;
            this.isTouchStartTimer = setTimeout(function () {
                this_.isTouchStartFlag = false;
            }, 1000);
        }
    }
}