
StiJsViewer.prototype.ShowAnimationVerticalMenu = function (menu, finishPos, endTime) {
    var currentPos = menu.innerContent.offsetTop;
    clearTimeout(menu.animationTimer);

    var d = new Date();
    var t = d.getTime();
    var step = Math.round((finishPos - currentPos) / ((Math.abs(endTime - t) + 1) / 30));

    // Last step
    if (Math.abs(step) > Math.abs(finishPos - currentPos)) step = finishPos - currentPos;

    currentPos = currentPos + step;
    var resultPos;

    if (t < endTime) {
        resultPos = currentPos;
        menu.animationTimer = setTimeout(function () { menu.jsObject.ShowAnimationVerticalMenu(menu, finishPos, endTime) }, 30);
    }
    else {
        resultPos = finishPos;
        menu.style.overflow = "visible";
        menu.animationTimer = null;
        if (menu.completeShow) menu.completeShow();
    }

    menu.innerContent.style.top = resultPos + "px";
}

StiJsViewer.prototype.ShowAnimationHorizontalMenu = function (menu, finishPos, endTime) {
    var currentPos = menu.innerContent.offsetLeft;
    clearTimeout(menu.animationTimer);

    var d = new Date();
    var t = d.getTime();
    var step = Math.round((finishPos - currentPos) / ((Math.abs(endTime - t) + 1) / 30));

    // Last step
    if (Math.abs(step) > Math.abs(finishPos - currentPos)) step = finishPos - currentPos;

    currentPos = currentPos + step;
    var resultPos;

    if (t < endTime) {
        resultPos = currentPos;
        menu.animationTimer = setTimeout(function () { menu.jsObject.ShowAnimationHorizontalMenu(menu, finishPos, endTime); }, 30);
    }
    else {
        resultPos = finishPos;
        menu.style.overflow = "visible";
        menu.animationTimer = null;
        if (menu.completeShow) menu.completeShow();
    }

    menu.innerContent.style.left = resultPos + "px";
}

StiJsViewer.prototype.ShowAnimationForm = function (form, endTime) {
    if (!form.flag) { form.currentOpacity = 1; form.flag = true; }
    clearTimeout(form.animationTimer);

    var d = new Date();
    var t = d.getTime();
    var step = Math.round((100 - form.currentOpacity) / ((Math.abs(endTime - t) + 1) / 30));

    // Last step
    if (Math.abs(step) > Math.abs(100 - form.currentOpacity)) step = 100 - form.currentOpacity;

    form.currentOpacity = form.currentOpacity + step;
    var resultOpacity;

    if (t < endTime) {
        resultOpacity = form.currentOpacity;
        form.animationTimer = setTimeout(function () { form.jsObject.ShowAnimationForm(form, endTime) }, 30);
    }
    else {
        resultOpacity = 100;
        form.flag = false;
        form.animationTimer = null;
    }

    form.style.opacity = resultOpacity / 100;
}

StiJsViewer.prototype.ShowAnimationForScroll = function (reportPanel, finishScrollTop, endTime, completeFunction) {
    if (!reportPanel) return;
    var currentScrollTop = 0;
    if (reportPanel.jsObject.options.appearance.scrollbarsMode) currentScrollTop = reportPanel.scrollTop;
    else {
        currentScrollTop = document.documentElement.scrollTop;
        if (currentScrollTop == 0) currentScrollTop = document.getElementsByTagName('BODY')[0].scrollTop;
    }

    clearTimeout(reportPanel.jsObject.controls.reportPanel.scrollTimer);
    clearTimeout(reportPanel.animationTimer);
    var d = new Date();
    var t = d.getTime();
    var step = Math.round((finishScrollTop - currentScrollTop) / ((Math.abs(endTime - t) + 1) / 30));

    // Last step
    if (Math.abs(step) > Math.abs(finishScrollTop - currentScrollTop)) step = finishScrollTop - currentScrollTop;

    currentScrollTop += step;
    var resultScrollTop;
    var this_ = this;

    if (t < endTime) {
        resultScrollTop = currentScrollTop;
        reportPanel.animationTimer = setTimeout(function () {
            this_.ShowAnimationForScroll(reportPanel, finishScrollTop, endTime, completeFunction);
        }, 30);
    }
    else {
        resultScrollTop = finishScrollTop;
        if (completeFunction) completeFunction();
    }

    if (reportPanel.jsObject.options.appearance.scrollbarsMode)
        reportPanel.scrollTop = resultScrollTop;
    else
        window.scrollTo(0, resultScrollTop);
}

StiJsViewer.prototype.easeInOutQuad = function (t) {
    return t < .5 ? 2 * t * t : -1 + (4 - 2 * t) * t;
}

StiJsViewer.prototype.animation = function (timstamp) {
    var now = new Date().getTime();
    for (var i in window.this_.options.animations) {
        var an = window.this_.options.animations[i];
        var el = an.el;
        if (an.duration <= now - an.start) {
            for (var j in an.animations) {
                var ann = an.animations[j];
                el.style[ann.style] = parseFloat(ann.end) + ann.postfix;
            }
            if (ann.finish) ann.finish();
            window.this_.options.animations.splice(i, 1);
        } else {
            for (var i in an.animations) {
                var ann = an.animations[i];
                el.style[ann.style] = parseFloat(ann.start) + window.this_.easeInOutQuad((now - parseFloat(an.start)) / an.duration) * (parseFloat(ann.end) - parseFloat(ann.start)) + ann.postfix;
                console.log(el.style[ann.style]);
            }
        }
    }
    if (window.this_.options.animations.length > 0) {
        window.requestAnimationFrame(window.this_.animation);
    }
}

StiJsViewer.prototype.animate = function (element, animation) {
    element.style.transitionDuration = animation.duration + "ms";
    var prop = "";
    for (var i in animation.animations) {
        prop += ((prop != "") ? ", " : "") + (animation.animations[i].property || animation.animations[i].style);
    }
    element.style.transitionProperty = prop;
    for (var i in animation.animations) {
        var an = animation.animations[i];
        element.style[an.style] = an.end + an.postfix;
        if (an.finish) {
            setTimeout(function () {
                an.finish();
            }, animation.duration);
        }
    }
    setTimeout(function () {
        element.style.transitionDuration = "";
    }, animation.duration * 2);


}