
StiMobileDesigner.prototype.ZoomControl = function () {
    var zoomControl = document.createElement("div");
    var zoomControlTable = this.CreateHTMLTable();
    zoomControl.appendChild(zoomControlTable);

    zoomControlTable.addCell(this.StatusPanelButton("zoomOut", null, "ZoomOut.png", this.loc.FormViewer.Zoom + " -", null, null, 35));
    zoomControlTable.addCell(this.ZoomScale()).style.padding = "0 5px 0 5px";
    zoomControlTable.addCell(this.StatusPanelButton("zoomIn", null, "ZoomIn.png", this.loc.FormViewer.Zoom + " +", null, null, 35));

    var zoomInfo = this.StatusPanelButton("zoomInfo", "100%", null, null, null, 35, 50);
    zoomControlTable.addCell(zoomInfo);
    zoomInfo.style.marginRight = "3px";
    zoomInfo.caption.style.padding = "0";
    if (zoomInfo.caption) zoomInfo.caption.style.textAlign = "center";

    var zoomMenu = this.ZoomMenu();

    zoomInfo.action = function () {
        zoomMenu.changeVisibleState(!zoomMenu.visible);
    }

    return zoomControl;
}

StiMobileDesigner.prototype.ZoomScale = function () {
    var zoomScale = document.createElement("div");
    zoomScale.jsObject = this;
    this.options.controls.zoomScale = zoomScale;
    zoomScale.className = this.options.isTouchDevice ? "stiDesignerZoomScale_Touch" : "stiDesignerZoomScale_Mouse";
    zoomScale.width = 100;
    zoomScale.style.width = zoomScale.width + "px";
    zoomScale.value = this.options.zoom;

    var zoomScaleLine = document.createElement("div");
    zoomScaleLine.className = this.options.isTouchDevice ? "stiDesignerZoomScaleLine_Touch" : "stiDesignerZoomScaleLine_Mouse";
    zoomScale.appendChild(zoomScaleLine);

    var zoomScaleMiddle = document.createElement("div");
    zoomScaleMiddle.className = "stiDesignerZoomScaleMiddle";
    zoomScaleMiddle.style.top = this.options.isTouchDevice ? "12px" : "7px";
    zoomScaleMiddle.style.left = "47px";
    zoomScale.appendChild(zoomScaleMiddle);

    var zoomScaleButton = document.createElement("div");
    zoomScaleButton.isPressed = false;
    zoomScaleButton.isEnabled = false;
    this.options.controls["zoomScaleButton"] = zoomScaleButton;
    zoomScaleButton.style.left = "45px";
    zoomScale.button = zoomScaleButton;
    zoomScaleButton.jsObject = this;
    zoomScaleButton.className = this.options.isTouchDevice ? "stiDesignerZoomScaleButton_Touch" : "stiDesignerZoomScaleButton_Mouse";
    if (this.options.isTouchDevice) {
        StiMobileDesigner.setImageSource(zoomScaleButton, this.options, "ZoomScaleButton.png");
        zoomScaleButton.style.backgroundSize = "contain";
    }
    zoomScale.appendChild(zoomScaleButton);

    zoomScale.setZoomPosition = function () {
        var leftPos = parseInt(this.offsetWidth * this.jsObject.options.report.zoom / 2) -
            (this.jsObject.options.isTouchDevice ? this.button.offsetWidth / 2 : this.button.offsetWidth);
        this.button.style.left = (this.jsObject.options.isTouchDevice ? leftPos - 2 : leftPos) + "px";
        this.jsObject.options.buttons.zoomInfo.caption.innerHTML = Math.round(this.jsObject.options.report.zoom * 100) + "%";
        this.value = this.jsObject.options.report.zoom;
    }

    zoomScaleButton.onmouseover = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseenter();
    }

    zoomScaleButton.onmouseenter = function () {
        if (this.jsObject.options.isTouchDevice || this.isPressed || !this.isEnabled || this.jsObject.options.isTouchClick) return;
        this.className = "stiDesignerZoomScaleButtonOver" + (this.jsObject.options.isTouchDevice ? "_Touch" : "_Mouse");
    }

    zoomScaleButton.onmouseleave = function () {
        if (this.jsObject.options.isTouchDevice || this.isPressed || !this.isEnabled) return;
        this.className = "stiDesignerZoomScaleButton" + (this.jsObject.options.isTouchDevice ? "_Touch" : "_Mouse");
    }

    zoomScaleButton.onmousedown = function (event) {
        if (this.isTouchStartFlag || !this.isEnabled) return;
        this.ontouchstart(event, true);
    }

    zoomScaleButton.ontouchstart = function (event, mouseProcess) {
        var this_ = this;
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        if (!this.isEnabled) return;
        this.jsObject.options.startPosZoomScaleButton = !mouseProcess ? event.touches[0].pageX : event.screenX;
        if (event) event.preventDefault();
        this.jsObject.options.startZoom = this.jsObject.options.report.zoom;
        this.isPressed = true;
        this.className = this.jsObject.options.isTouchDevice ? "stiDesignerZoomScaleButtonOver_Touch" : "stiDesignerZoomScaleButtonOver_Mouse";
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    zoomScaleButton.ontouchend = function (event) {
        if (!this.isEnabled) return;
        if (event) event.preventDefault();
        this.jsObject.options.startPosZoomScaleButton = false;
        this.className = this.jsObject.options.isTouchDevice ? "stiDesignerZoomScaleButton_Touch" : "stiDesignerZoomScaleButton_Mouse";
        this.isPressed = false;
    }

    zoomScaleButton.setEnabled = function (state) {
        this.isEnabled = state;
        this.className = (state ? "stiDesignerZoomScaleButton" : "stiDesignerZoomScaleButtonDisabled") +
            (this.jsObject.options.isTouchDevice ? "_Touch" : "_Mouse");
    }

    zoomScale.ontouchmove = function (event, mouseProcess) {
        if (event) event.preventDefault();
        var currentPos = mouseProcess ? (event.screenX) : event.touches[0].pageX;
        var deltaPos = (Math.round((this.jsObject.options.startPosZoomScaleButton - currentPos) / (this.offsetWidth / 20)) / 10);
        var newZoom = Math.round((this.jsObject.options.startZoom - deltaPos) * 10) / 10;
        if (newZoom < 0.1) newZoom = 0.1;
        if (newZoom > 2) newZoom = 2;
        if (this.jsObject.options.oldDeltaPos != deltaPos) {
            this.jsObject.options.report.zoom = newZoom;
            if (this.jsObject.options.currentPage) this.jsObject.PreZoomPage(this.jsObject.options.currentPage);
        }
        this.jsObject.options.oldDeltaPos = deltaPos;
    }

    zoomScale.onmousedown = function (event) {
        if (this.jsObject.options.report) {
            var scaleWidth = zoomScale.width;

            var zoomScalePosX = this.jsObject.FindPosX(this, null, true);
            var mousePosX = event.clientX || event.x;
            var deltaX = mousePosX - zoomScalePosX;

            if (!zoomScaleButton.isPressed && deltaX && deltaX > 0 && deltaX < scaleWidth) {
                var newZoom = Math.round((((deltaX / scaleWidth) * 2) + 0.05) * 10) / 10;
                if (newZoom < 0.1) newZoom = 0.1;
                if (newZoom > 2) newZoom = 2;
                if (this.jsObject.options.report.zoom != newZoom) {
                    this.jsObject.options.report.zoom = newZoom;
                    if (this.jsObject.options.currentPage) this.jsObject.PreZoomPage(this.jsObject.options.currentPage);
                }
            }
        }
    }

    return zoomScale;
}