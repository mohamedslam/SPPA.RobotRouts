
StiJsViewer.prototype.FindPosX = function (obj, mainClassName, noScroll) {
    var curleft = noScroll ? 0 : this.GetScrollXOffset(obj, mainClassName);
    if (obj.offsetParent) {
        while (obj.className != mainClassName) {
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

StiJsViewer.prototype.FindPosY = function (obj, mainClassName, noScroll) {
    var curtop = noScroll ? 0 : this.GetScrollYOffset(obj, mainClassName);
    if (obj.offsetParent) {
        while (obj.className != mainClassName) {
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

StiJsViewer.prototype.GetScrollXOffset = function (obj, mainClassName) {
    var scrollleft = 0;
    if (obj.parentElement) {
        while (obj.className != mainClassName) {
            if ("scrollLeft" in obj) { scrollleft -= obj.scrollLeft }
            if (!obj.parentElement) {
                break;
            }
            obj = obj.parentElement;
        }
    }

    return scrollleft;
}

StiJsViewer.prototype.GetScrollYOffset = function (obj, mainClassName) {
    var scrolltop = 0;
    if (obj.parentElement) {
        while (obj.className != mainClassName) {
            if ("scrollTop" in obj) { scrolltop -= obj.scrollTop }
            if (!obj.parentElement) {
                break;
            }
            obj = obj.parentElement;
        }
    }

    return scrolltop;
}

StiJsViewer.prototype.FindMousePosOnMainPanel = function (evnt) {
    var posXMainPanel = this.FindPosX(this.controls.mainPanel, null, false);
    var posYMainPanel = this.FindPosY(this.controls.mainPanel, null, false);

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