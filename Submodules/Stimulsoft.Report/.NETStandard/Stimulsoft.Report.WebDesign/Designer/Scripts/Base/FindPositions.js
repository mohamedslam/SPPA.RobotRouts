
StiMobileDesigner.prototype.FindPosX = function (obj, mainClassName, noScroll, mainElement) {
    var curleft = noScroll ? 0 : this.GetScrollXOffset(obj, mainClassName, mainElement);
    if (obj.offsetParent) {
        while (mainClassName ? obj.className != mainClassName : (mainElement ? obj != mainElement.offsetParent : obj != null)) {
            curleft += obj.offsetLeft;
            if (!obj.offsetParent) {
                break;
            }
            obj = obj.offsetParent;

        }
    } else if (obj.x) {
        curleft += obj.x;
    }
    return curleft;
}

StiMobileDesigner.prototype.FindPosY = function (obj, mainClassName, noScroll, mainElement) {
    var curtop = noScroll ? 0 : this.GetScrollYOffset(obj, mainClassName, mainElement);
    if (obj.offsetParent) {
        while (mainClassName ? obj.className != mainClassName : (mainElement ? obj != mainElement.offsetParent : obj != null)) {
            curtop += obj.offsetTop;
            if (!obj.offsetParent) {
                break;
            }
            obj = obj.offsetParent;
        }
    } else if (obj.y) {
        curtop += obj.y;
    }
    return curtop;
}

StiMobileDesigner.prototype.GetScrollXOffset = function (obj, mainClassName, mainElement) {
    var scrollleft = 0;
    if (obj.parentElement) {
        while (mainClassName ? obj.className != mainClassName : obj != mainElement) {
            if ("scrollLeft" in obj) { scrollleft -= obj.scrollLeft }
            if (!obj.parentElement) {
                break;
            }
            obj = obj.parentElement;
        }
    }

    return scrollleft;
}

StiMobileDesigner.prototype.GetScrollYOffset = function (obj, mainClassName, mainElement) {
    var scrolltop = 0;
    if (obj.parentElement) {
        while (mainClassName ? obj.className != mainClassName : obj != mainElement) {
            if ("scrollTop" in obj) { scrolltop -= obj.scrollTop }
            if (!obj.parentElement) {
                break;
            }
            obj = obj.parentElement;
        }
    }

    return scrolltop;
}

StiMobileDesigner.prototype.FindPagePositions = function (mainElement) {
    var posXPaintPanel = this.FindPosX(this.options.paintPanel, null, false, mainElement);
    var posYPaintPanel = this.FindPosY(this.options.paintPanel, null, false, mainElement);

    return {
        posX: posXPaintPanel + this.options.paintPanelPadding + 1,
        posY: posYPaintPanel + this.options.paintPanelPadding + 1
    }
}

StiMobileDesigner.prototype.FindMousePosOnSvgPage = function (page, evnt) {
    var pagePositions = this.FindPagePositions();
    var mouseClientX = 0;
    var mouseClientY = 0;

    if (evnt) {
        if (evnt.touches != null) {
            mouseClientX = evnt.touches[0].pageX;
            mouseClientY = evnt.touches[0].pageY;
        }
        else {
            mouseClientX = evnt.clientX || evnt.x;
            mouseClientY = evnt.clientY || evnt.y;
        }
    }

    var point = {};
    point.xUnits = this.ConvertPixelToUnit((mouseClientX - pagePositions.posX - page.marginsPx[0]) / this.options.report.zoom, page.isDashboard);
    point.yUnits = this.ConvertPixelToUnit((mouseClientY - pagePositions.posY - page.marginsPx[1]) / this.options.report.zoom, page.isDashboard);
    point.xPixels = parseInt(mouseClientX - pagePositions.posX);
    point.yPixels = parseInt(mouseClientY - pagePositions.posY);

    return point;
}

StiMobileDesigner.prototype.FindMousePosOnMainPanel = function (evnt) {
    var posXMainPanel = this.FindPosX(this.options.mainPanel, null, false);
    var posYMainPanel = this.FindPosY(this.options.mainPanel, null, false);

    var mouseClientX = 0;
    var mouseClientY = 0;

    if (evnt.touches != null) {
        mouseClientX = evnt.touches[0].pageX;
        mouseClientY = evnt.touches[0].pageY;
    }
    else {
        mouseClientX = evnt.clientX || evnt.x;
        mouseClientY = evnt.clientY || evnt.y;
    }

    var point = {};
    point.xPixels = parseInt(mouseClientX - posXMainPanel);
    point.yPixels = parseInt(mouseClientY - posYMainPanel);

    return point;
}


