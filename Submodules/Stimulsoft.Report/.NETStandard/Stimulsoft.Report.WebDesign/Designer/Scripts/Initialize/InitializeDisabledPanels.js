
StiMobileDesigner.prototype.InitializeDisabledPanels = function () {
    this.options.disabledPanels = [];

    for (var i = 0; i <= 6; i++) {
        var disabledPanel = document.createElement("div");
        disabledPanel.jsObject = this;
        disabledPanel.style.display = "none";
        this.options.mainPanel.appendChild(disabledPanel);
        this.options.disabledPanels.push(disabledPanel);
        disabledPanel.style.zIndex = (i == 0) ? 1 : 10 * i;
        disabledPanel.className = "stiDesignerDisabledPanel";

        disabledPanel.changeVisibleState = function (state) {
            this.style.display = state ? "" : "none";
        }

        disabledPanel.onmousedown = function () {
            if (this.isTouchStartFlag) return;
            this.ontouchstart(true);
        }

        disabledPanel.ontouchstart = function (mouseProcess) {
            var this_ = this;
            this.isTouchStartFlag = mouseProcess ? false : true;
            clearTimeout(this.isTouchStartTimer);
            this.jsObject.options.disabledPanelPressed = true;
            this.isTouchStartTimer = setTimeout(function () {
                this_.isTouchStartFlag = false;
            }, 1000);
        }
    }
}