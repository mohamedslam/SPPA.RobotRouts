
StiMobileDesigner.prototype.ShowAnimationFileMenu = function (object, finishValue, endTime) {
    var currentValue = object.offsetWidth;
    clearTimeout(object.animationTimer);
    var d = new Date();
    var t = d.getTime();
    var step = Math.round((finishValue - currentValue) / ((Math.abs(endTime - t) + 1) / 30));
    if (Math.abs(step) > Math.abs(finishValue - currentValue)) step = finishValue - currentValue;

    currentValue = currentValue + step;
    var resultValue;

    if (t < endTime) {
        resultValue = currentValue;
        object.animationTimer = setTimeout(function () { object.jsObject.ShowAnimationFileMenu(object, finishValue, endTime); }, 30);
    }
    else {
        resultValue = finishValue;
        object.animationTimer = null;
        if (finishValue == 0) object.style.display = "none";
    }

    object.style.width = resultValue + "px";
}

StiMobileDesigner.prototype.ShowAnimationVerticalMenu = function (menu, finishPos, endTime) {
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
        menu.animationTimer = setTimeout(function () { menu.jsObject.ShowAnimationVerticalMenu(menu, finishPos, endTime); }, 30);
    }
    else {
        resultPos = finishPos;
        menu.style.overflow = "visible";
        menu.animationTimer = null;
    }

    menu.innerContent.style.top = resultPos + "px";
}

StiMobileDesigner.prototype.ShowAnimationHorizontalMenu = function (menu, finishPos, endTime) {
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
    }

    menu.innerContent.style.left = resultPos + "px";
}

StiMobileDesigner.prototype.ShowAnimationForm = function (form, endTime) {
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
        if (form["oncompleteshow"] != null) form.oncompleteshow();
    }

    form.style.opacity = resultOpacity / 100;
}