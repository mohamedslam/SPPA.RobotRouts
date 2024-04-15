
StiMobileDesigner.prototype.ImageSvgButton = function (imageName, buttonSize, imageSize, toolTip) {
    var button = this.CreateSvgElement("svg");
    var width = buttonSize || 24;
    var height = buttonSize || 24;
    button.setAttribute("height", width);
    button.setAttribute("width", height);
    button.jsObject = this;

    var rect = this.CreateSvgElement("rect");
    rect.style.fill = "#ffffff";
    rect.style.stroke = "#808080";
    rect.setAttribute("height", width);
    rect.setAttribute("width", height);
    button.appendChild(rect);

    var img = this.CreateSvgElement("image");
    img.setAttribute("height", imageSize || 16);
    img.setAttribute("width", imageSize || 16);
    img.setAttribute("x", imageSize ? (width - imageSize) / 2 : 6);
    img.setAttribute("y", imageSize ? (width - imageSize) / 2 : 6);
    button.appendChild(img);
    StiMobileDesigner.setImageSource(img, this.options, imageName);

    if (!this.options.isTouchDevice) {
        button.onmouseover = function () { rect.style.fill = "#d3d3d3"; };
        button.onmouseout = function () { rect.style.fill = "#ffffff"; };
    }

    if (toolTip) {
        var title = this.CreateSvgElement("title");
        title.textContent = toolTip;
        img.appendChild(title);
    }

    button.onmousedown = function () {
        if (this.isTouchStartFlag) return;
        rect.style.fill = "#ffffff";
        this.action();
    }

    button.ontouchstart = function () {
        this.isTouchStartFlag = true;
        rect.style.fill = "#ffffff";
        this.action();
        clearTimeout(this.isTouchStartTimer);
        this.isTouchStartTimer = setTimeout(function () {
            button.isTouchStartFlag = false;
        }, 1000);

    }

    button.action = function () { };

    return button;
}

StiMobileDesigner.prototype.TextSvgButton = function (caption, width, height) {
    var button = this.CreateSvgElement("svg");
    button.setAttribute("width", width);
    button.setAttribute("height", height);
    button.isEnabled = true;

    var rect = this.CreateSvgElement("rect");
    button.rect = rect;
    rect.setAttribute("x", 0);
    rect.setAttribute("y", 0);
    rect.setAttribute("width", width);
    rect.setAttribute("height", height);
    rect.setAttribute("class", "stiDesignerSvgButton");
    button.appendChild(rect);

    var fontSize = 12;
    var text = this.CreateSvgElement("text");
    button.text = text;
    text.setAttribute("fill", "#ffffff");
    text.setAttribute("font-family", "Arial");
    text.setAttribute("font-size", fontSize);
    text.setAttribute("x", 0);
    text.setAttribute("y", parseInt((height - fontSize) / 2) + 9);
    text.textContent = caption;
    button.appendChild(text);

    if (!this.options.isTouchDevice) {
        button.onmouseover = function () {
            if (this.isEnabled)
                rect.setAttribute("class", "stiDesignerSvgButtonOver");
        };

        button.onmouseout = function () {
            if (this.isEnabled)
                rect.setAttribute("class", "stiDesignerSvgButton");
        };
    }

    button.onmousedown = function (event) {
        if (this.isTouchStartFlag || !this.isEnabled) return;
        rect.setAttribute("class", "stiDesignerSvgButton");
        this.action();
        event.stopPropagation();
        event.preventDefault();
    }

    button.ontouchstart = function (event) {
        this.isTouchStartFlag = true;
        if (!this.isEnabled) return;
        rect.setAttribute("class", "stiDesignerSvgButton");
        this.action();
        clearTimeout(this.isTouchStartTimer);
        this.isTouchStartTimer = setTimeout(function () {
            button.isTouchStartFlag = false;
        }, 1000);
        event.stopPropagation();
        event.preventDefault();
    }

    button.setEnabled = function (state) {
        this.isEnabled = state;
        rect.setAttribute("class", state ? "stiDesignerSvgButton" : "stiDesignerSvgButtonDisabled");
    }

    button.action = function () { };

    return button;
}