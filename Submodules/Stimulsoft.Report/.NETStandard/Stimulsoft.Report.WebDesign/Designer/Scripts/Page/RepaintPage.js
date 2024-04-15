
//--------------------------Page---------------------------------------

StiMobileDesigner.prototype.RepaintPage = function (page, rebuildGrigLines) {
    page.widthPx = parseInt(this.ConvertUnitToPixel(this.StrToDouble(page.properties.unitWidth), page.isDashboard) * this.options.report.zoom);
    page.heightPx = parseInt(this.ConvertUnitToPixel(this.StrToDouble(page.properties.unitHeight), page.isDashboard) * this.options.report.zoom);

    var marginsStr = page.properties.unitMargins.split("!");
    var verticalMarginsPx = parseInt(this.ConvertUnitToPixel(this.StrToDouble(marginsStr[1]) + this.StrToDouble(marginsStr[3]), page.isDashboard) * this.options.report.zoom);
    var horizontalMarginsPx = parseInt(this.ConvertUnitToPixel(this.StrToDouble(marginsStr[0]) + this.StrToDouble(marginsStr[2]), page.isDashboard) * this.options.report.zoom);

    if (page.isDashboard) {
        page.widthPx += horizontalMarginsPx;
        page.heightPx += verticalMarginsPx;
    }

    var segmentPerHeight = this.StrToDouble(page.properties.segmentPerHeight);
    var segmentPerWidth = this.StrToDouble(page.properties.segmentPerWidth);
    if (segmentPerWidth > 1) page.widthPx = ((page.widthPx - horizontalMarginsPx) * segmentPerWidth) + horizontalMarginsPx;
    if (segmentPerHeight > 1) page.heightPx = ((page.heightPx - verticalMarginsPx) * segmentPerHeight) + verticalMarginsPx;

    var largeHeightFactor = (page.properties.largeHeight) ? this.StrToInt(page.properties.largeHeightFactor) : this.StrToDouble(page.properties.largeHeightAutoFactor);
    page.heightPx = (page.heightPx - verticalMarginsPx) * largeHeightFactor + verticalMarginsPx;

    page.setAttribute("width", page.widthPx);
    page.setAttribute("height", page.heightPx);

    if (this.options.report && this.options.report.info.showGrid)
        this.RepaintGridLines(page, rebuildGrigLines);

    this.RepaintPageBorder(page);
    this.RepaintPageBrush(page);
    this.RepaintPageWaterMark(page);
    this.RepaintPageWaterMarkImage(page);
    this.RepaintPageWaterMarkWeaves(page);
    this.RepaintMultiSelectObjects(page);
    this.RepaintColumnsLines(page);

    if (page.controls.waterMarkBackParent)
        this.RepaintWaterMarkBack(page);
}

StiMobileDesigner.prototype.RepaintGridLines = function (page, rebuildGrigLines) {
    if (rebuildGrigLines) this.CreatePageGridLines(page);
    if (!page.controls.gridLines) return;
    for (var i = 0; i < page.controls.gridLines.length; i++) {
        var line = page.controls.gridLines[i].repaint();
    }
}

StiMobileDesigner.prototype.RepaintLargeHeightLines = function (page) {
    var largeHeightFactor = page.properties.largeHeight ? this.StrToInt(page.properties.largeHeightFactor) : this.StrToDouble(page.properties.largeHeightAutoFactor);
    if (largeHeightFactor > 1) {
        var redBorder = function () {
            var redBorder = ("createElementNS" in document) ? document.createElementNS("http://www.w3.org/2000/svg", "line") : document.createElement("line");
            redBorder.style.strokeDasharray = "2,2";
            redBorder.style.stroke = "#ff0000";

            return redBorder;
        }

        if (!page.controls.redOutBorder && !page.controls.redInnerBorder) {
            page.controls.redOutBorder = redBorder();
            page.controls.redInnerBorder = redBorder();
            page.appendChild(page.controls.redOutBorder);
            page.appendChild(page.controls.redInnerBorder);
        }
        else {
            page.controls.redOutBorder.style.display = "";
            page.controls.redInnerBorder.style.display = "";
        }

        var innerBorderY = (page.heightPx - page.marginsPx[3] - page.marginsPx[1]) / largeHeightFactor + page.marginsPx[1];
        var innerBorderPosition = [page.widthPx - page.marginsPx[2] + this.options.xOffset, innerBorderY + this.options.yOffset, page.marginsPx[0] + this.options.xOffset, innerBorderY + this.options.yOffset];
        var outBorderPosition = [0, innerBorderY + page.marginsPx[3] + this.options.yOffset, page.widthPx, innerBorderY + page.marginsPx[3] + this.options.yOffset];
        var attrs = ["x1", "y1", "x2", "y2"];

        for (var i = 0; i < attrs.length; i++) {
            page.controls.redInnerBorder.setAttribute(attrs[i], innerBorderPosition[i]);
            page.controls.redOutBorder.setAttribute(attrs[i], outBorderPosition[i]);
        }
    }
    else if (page.controls.redOutBorder && page.controls.redInnerBorder) {
        page.controls.redOutBorder.style.display = "none";
        page.controls.redInnerBorder.style.display = "none";
    }
}

StiMobileDesigner.prototype.RepaintPageSegmentLines = function (page) {
    var segmentPerHeight = this.StrToDouble(page.properties.segmentPerHeight);
    var segmentPerWidth = this.StrToDouble(page.properties.segmentPerWidth);

    //Remove old lines
    if (page.controls.pageSegmentLines) {
        for (var i = 0; i < page.controls.pageSegmentLines.length; i++) {
            page.removeChild(page.controls.pageSegmentLines[i]);
        }
    }

    page.controls.pageSegmentLines = [];

    //Add new lines
    var addSegmentLine = function (x1, y1, x2, y2, isBorder) {
        var line = ("createElementNS" in document) ? document.createElementNS("http://www.w3.org/2000/svg", "line") : document.createElement("line");
        if (!isBorder) line.style.strokeDasharray = "5,3";
        line.style.stroke = "#0000ff";
        line.setAttribute("x1", x1);
        line.setAttribute("y1", y1);
        line.setAttribute("x2", x2);
        line.setAttribute("y2", y2);
        if (isBorder)
            page.appendChild(line);
        else
            page.insertBefore(line, page.controls.borders[0]);
        page.controls.pageSegmentLines.push(line);

        return line;
    }

    var pageInnerWidth = page.widthPx - page.marginsPx[0] - page.marginsPx[2];
    var pageInnerHeight = page.heightPx - page.marginsPx[1] - page.marginsPx[3];

    if (segmentPerWidth > 1 || segmentPerHeight > 1) {
        var y1 = page.marginsPx[1];
        var y2 = page.heightPx - page.marginsPx[3];

        for (var i = 0; i <= segmentPerWidth; i++) {
            var x = page.marginsPx[0] + (pageInnerWidth / segmentPerWidth) * i;
            addSegmentLine(x, y1, x, y2, i == 0 || i == segmentPerWidth);
        }

        var x1 = page.marginsPx[0];
        var x2 = page.widthPx - page.marginsPx[2];

        for (var i = 0; i <= segmentPerHeight; i++) {
            var y = page.marginsPx[1] + (pageInnerHeight / segmentPerHeight) * i;
            addSegmentLine(x1, y, x2, y, i == 0 || i == segmentPerHeight);
        }
    }
}

StiMobileDesigner.prototype.RepaintPageBorder = function (page) {
    var margins = page.properties.unitMargins.split("!");
    page.marginsPx = [];
    for (var i = 0; i < 4; i++) {
        page.marginsPx[i] = parseInt(this.ConvertUnitToPixel(this.StrToDouble(margins[i]), page.isDashboard) * this.options.report.zoom);
    }

    if (page.isDashboard) {
        for (var borderNum = 0; borderNum < 8; borderNum++) {
            page.controls.borders[borderNum].style.visibility = "hidden";
        }
    }
    else {
        var borderStyles = ["", "9,3", "9,2,2,2", "9,2,2,2,2,2", "2,2", "", "none"];
        var borderProps = page.properties.border.split("!");
        var borderVisibleProps = borderProps[0].split(",");
        var borderSize = borderProps[1];
        var borderColor = borderProps[2];
        var borderStyle = borderProps[3];
        var advSizes = [];
        var advStyles = [];
        var advShowBorder = [];
        var advBorders = borderProps.length > 8 ? borderProps[8].split(";") : null

        for (var borderNum = 0; borderNum < 8; borderNum++) {
            //Advanced borders
            if (advBorders) {
                var propIndex = borderNum < 4 ? borderNum * 3 : (borderNum - 4) * 3;
                borderStyle = advBorders[propIndex + 2];
                borderColor = advBorders[propIndex + 1];
                borderSize = borderStyle != "5" ? advBorders[propIndex] : "1";

                if (borderNum < 4) {
                    advSizes.push(borderSize);
                    advStyles.push(borderStyle);
                }
            }

            var showBorder = (borderVisibleProps[borderNum < 4 ? borderNum : borderNum - 4] == "1" && borderStyle != "6") ? true : false;
            page.controls.borders[borderNum].style.stroke = showBorder ? this.GetHTMLColor(borderColor) : "#787878";
            page.controls.borders[borderNum].style.strokeWidth = (showBorder && borderStyle != "5") ? borderSize : "1";
            page.controls.borders[borderNum].style.strokeDasharray = showBorder ? borderStyles[borderStyle] : "";

            if (page.isDashboard) {
                page.controls.borders[borderNum].style.visibility = "hidden";
            }
            else if (borderNum >= 4) {
                page.controls.borders[borderNum].style.visibility = (borderVisibleProps[borderNum - 4] == "1" && borderStyle == "5") ? "visible" : "hidden";
            }
            if (advBorders && borderNum < 4) {
                advShowBorder.push(showBorder);
            }
        }

        var XOffsets = [];
        var YOffsets = [];

        for (var i = 0; i < 4; i++) {
            var borderSize = parseInt(showBorder && borderStyle != "5" ? borderSize : "1");
            if (advSizes.length > 0) {
                borderSize = parseInt(advShowBorder[i] && advStyles[i] != "5" ? advSizes[i] : "1");
            }
            XOffsets[i] = (borderSize % 2 != 0) ? this.options.xOffset : 0;
            YOffsets[i] = (borderSize % 2 != 0) ? this.options.yOffset : 0;
        }

        var tempX = page.widthPx - page.marginsPx[2];
        var tempY = page.heightPx - page.marginsPx[3];
        var shadowSize = parseInt(borderProps.length > 4 ? parseInt(borderProps[5]) * this.options.report.zoom : 0);

        var bordersPosition = [
            [page.marginsPx[0] + XOffsets[0], page.marginsPx[1] + YOffsets[0], page.marginsPx[0] + XOffsets[0], tempY + YOffsets[0]],
            [page.marginsPx[0] + XOffsets[1], page.marginsPx[1] + YOffsets[1], tempX + XOffsets[1], page.marginsPx[1] + YOffsets[1]],
            [tempX + XOffsets[2], page.marginsPx[1] + YOffsets[2], tempX + XOffsets[2], tempY + YOffsets[2]],
            [tempX + XOffsets[3], tempY + YOffsets[3], page.marginsPx[0] + XOffsets[3], tempY + YOffsets[3]],
            [page.marginsPx[0] + XOffsets[0] + 2, page.marginsPx[1] + YOffsets[0] + 2, page.marginsPx[0] + XOffsets[0] + 2, tempY + YOffsets[0] - 2],
            [page.marginsPx[0] + XOffsets[1] + 2, page.marginsPx[1] + YOffsets[1] + 2, tempX + XOffsets[1] - 2, page.marginsPx[1] + YOffsets[1] + 2],
            [tempX + XOffsets[2] - 2, page.marginsPx[1] + YOffsets[2] + 2, tempX + XOffsets[2] - 2, tempY + YOffsets[2] - 2],
            [tempX + XOffsets[3] - 2, tempY + YOffsets[3] - 2, page.marginsPx[0] + XOffsets[3] + 2, tempY + YOffsets[3] - 2],
            [tempX + XOffsets[2] + shadowSize / 2, page.marginsPx[1] + YOffsets[2] + shadowSize, tempX + XOffsets[2] + shadowSize / 2, tempY + YOffsets[2] + shadowSize],
            [tempX + XOffsets[3] + shadowSize, tempY + YOffsets[3] + shadowSize / 2, page.marginsPx[0] + XOffsets[3] + shadowSize, tempY + YOffsets[3] + shadowSize / 2]
        ];

        for (borderNum = 0; borderNum < 10; borderNum++) {
            page.controls.borders[borderNum].setAttribute("x1", bordersPosition[borderNum][0]);
            page.controls.borders[borderNum].setAttribute("y1", bordersPosition[borderNum][1]);
            page.controls.borders[borderNum].setAttribute("x2", bordersPosition[borderNum][2]);
            page.controls.borders[borderNum].setAttribute("y2", bordersPosition[borderNum][3]);
        }

        this.RepaintLargeHeightLines(page);
        this.RepaintPageSegmentLines(page);

        //Shadow lines
        page.controls.borders[8].style.visibility = "hidden";
        page.controls.borders[9].style.visibility = "hidden";

        if (borderProps.length > 4) {
            var dropShadow = borderProps[4] == "1";
            if (dropShadow) {
                var shadowColor = this.GetHTMLColor(this.GetColorFromBrushStr(StiBase64.decode(borderProps[6])));
                page.controls.borders[8].style.visibility = "visible";
                page.controls.borders[8].style.stroke = shadowColor;
                page.controls.borders[8].style.strokeWidth = shadowSize;
                page.controls.borders[9].style.visibility = "visible";
                page.controls.borders[9].style.stroke = shadowColor;
                page.controls.borders[9].style.strokeWidth = shadowSize;
            }
        }
    }
}

StiMobileDesigner.prototype.RepaintPageBrush = function (page) {
    if (page.isDashboard) {
        page.style.background = this.GetHTMLColor(page.properties.realBackColor);
        return;
    }

    var brushArray = page.properties.brush.split("!");

    //remove old brushes
    page.controls.svgHatchBrush.clear();
    page.controls.gradient.rect.style.display = "none";

    switch (brushArray[0]) {
        case "0": {
            page.style.background = "rgb(255,255,255)";
            break;
        }
        case "1": {
            var color = this.GetHTMLColor(brushArray[1]);
            page.style.background = (color == "transparent" || color == "rgb(255,255,255,0") ? "white" : color;
            break;
        }
        case "2": {
            page.controls.svgHatchBrush.appendChild(this.GetSvgHatchBrush(brushArray, 0, 0, page.widthPx, page.heightPx));
            break;
        }
        case "3":
        case "4": {
            page.controls.gradient.applyBrush(brushArray);
            break;
        }
        case "5": {
            page.style.background = "linear-gradient(" + this.GetHTMLColor(brushArray[4]) + " 50%, " + this.GetHTMLColor(brushArray[1]) + " 50%)";
            break;
        }
    }
}

StiMobileDesigner.prototype.RepaintPageWaterMark = function (page) {
    var isDbsElement = page.isDashboard || page.typeComponent == "StiPanelElement";

    if (isDbsElement ? page.properties.dashboardWatermark == null : page.properties.waterMarkText == null)
        return;

    var textWaterMark = StiBase64.decode(isDbsElement ? page.properties.dashboardWatermark.text : page.properties.waterMarkText);

    if (!(isDbsElement ? page.properties.dashboardWatermark.textEnabled : page.properties.waterMarkEnabled) || textWaterMark == "") {
        page.controls.waterMarkParent.style.display = "none";
        return;
    }
    else {
        page.controls.waterMarkParent.style.display = "";
    }

    var cWaterMarkGradient = page.controls.waterMarkGradient;
    var cWaterMarkText = page.controls.waterMarkText;
    cWaterMarkText.textContent = textWaterMark;
    page.controls.waterMarkChild.setAttribute("transform", "rotate(-" + (isDbsElement ? page.properties.dashboardWatermark.textAngle : page.properties.waterMarkAngle) + ")");

    var fontArray = (isDbsElement ? page.properties.dashboardWatermark.textFont : page.properties.waterMarkFont).split("!");
    cWaterMarkText.style.fontFamily = fontArray[0];

    var fontSize = (fontArray[1] * this.options.report.zoom);
    cWaterMarkText.style.fontSize = fontSize + "pt";

    cWaterMarkText.style.fontWeight = fontArray[2] == "1" ? "bold" : "";
    cWaterMarkText.style.fontStyle = fontArray[3] == "1" ? "italic" : "";
    cWaterMarkText.style.textDecoration = "";
    if (fontArray[5] == "1") cWaterMarkText.style.textDecoration = "line-through";
    if (fontArray[4] == "1") cWaterMarkText.style.textDecoration += " underline";

    cWaterMarkText.style.textAnchor = "middle";

    if (isDbsElement) {
        this.ApplyColorToElement(cWaterMarkText, page.properties.dashboardWatermark.textColor);
    }
    else {
        var textBrushArray = page.properties.waterMarkTextBrush.split("!");
        if (textBrushArray[0] == "0") {
            cWaterMarkText.style.fill = "transparent";
        }
        else if (textBrushArray[0] == "1" || textBrushArray[0] == "5") {
            this.ApplyColorToElement(cWaterMarkText, textBrushArray[1]);
        }
        else if (textBrushArray[0] == "3") {
            cWaterMarkGradient.stop1.setAttribute("stop-color", this.GetHTMLColor(textBrushArray[1]));
            if (cWaterMarkGradient.stop2.parentNode) cWaterMarkGradient.stop2.parentNode.removeChild(cWaterMarkGradient.stop2);
            cWaterMarkGradient.stop3.setAttribute("stop-color", this.GetHTMLColor(textBrushArray[2]));
            var angle = this.StrToInt(textBrushArray[3]);
            cWaterMarkGradient.setAttribute("x2", Math.abs(angle - 90) + "%");
            cWaterMarkGradient.setAttribute("y2", angle + "%");
            cWaterMarkText.style.fill = "url(#" + cWaterMarkGradient.id + ")";
            cWaterMarkText.style.stroke = "none";
            cWaterMarkText.style.fillOpacity = Math.min(this.GetOpacityFromColor(textBrushArray[1]), this.GetOpacityFromColor(textBrushArray[2]));

        }
        else if (textBrushArray[0] == "4") {
            cWaterMarkGradient.stop1.setAttribute("stop-color", this.GetHTMLColor(textBrushArray[1]));
            cWaterMarkGradient.stop2.setAttribute("stop-color", this.GetHTMLColor(textBrushArray[2]));
            cWaterMarkGradient.insertBefore(cWaterMarkGradient.stop2, cWaterMarkGradient.stop3);
            cWaterMarkGradient.stop3.setAttribute("stop-color", this.GetHTMLColor(textBrushArray[1]));
            var brushProps = page.properties.brush.split("!");
            var angle = this.StrToInt(brushProps[3]);
            cWaterMarkGradient.setAttribute("x2", Math.abs(angle - 90) + "%");
            cWaterMarkGradient.setAttribute("y2", angle + "%");
            cWaterMarkText.style.fillOpacity = Math.min(this.GetOpacityFromColor(textBrushArray[1]), this.GetOpacityFromColor(textBrushArray[2]));
        }
    }

    var pageWidthPx = page.typeComponent == "StiPanelElement" ? parseInt(page.getAttribute("width")) : page.widthPx
    var pageHeightPx = page.typeComponent == "StiPanelElement" ? parseInt(page.getAttribute("height")) : page.heightPx

    page.controls.waterMarkParent.setAttribute("transform", "translate(" + (pageWidthPx / 2) + ", " + (pageHeightPx / 2) + ")");
}

StiMobileDesigner.prototype.RepaintPageWaterMarkImage = function (page) {
    var isDbsElement = (page.isDashboard || page.typeComponent == "StiPanelElement") && page.properties.dashboardWatermark;
    var imageControl = page.controls.waterMarkImage;

    var watermarkImageSrc = isDbsElement ? page.properties.dashboardWatermark.image : (page.properties.watermarkImageSrc || page.properties.watermarkImageContentForPaint);
    if (watermarkImageSrc && (!isDbsElement || page.properties.dashboardWatermark.imageEnabled)) {
        var isWmfImage = watermarkImageSrc.indexOf("data:image/x-wmf") >= 0;
        var zoom = this.options.report.zoom;
        var multipleFactor = this.StrToDouble(isDbsElement ? page.properties.dashboardWatermark.imageMultipleFactor : page.properties.waterMarkMultipleFactor);
        var stretch = (isDbsElement ? page.properties.dashboardWatermark.imageStretch : page.properties.waterMarkStretch) || isWmfImage;
        var aspectRatio = (isDbsElement ? page.properties.dashboardWatermark.imageAspectRatio : page.properties.waterMarkRatio) && !isWmfImage;
        var sizeWatermark = (isDbsElement ? page.properties.dashboardWatermark.imageSize : page.properties.watermarkImageSize).split(";");
        var imageTransparency = parseInt(isDbsElement ? page.properties.dashboardWatermark.imageTransparency : page.properties.waterMarkTransparency);

        var setImageSrc = function () {
            var widthWatermark = sizeWatermark[0] * multipleFactor * zoom;
            var heightWatermark = sizeWatermark[1] * multipleFactor * zoom;

            if (page.typeComponent == "StiPanelElement") {
                page.widthPx = parseInt(page.getAttribute("width"));
                page.heightPx = parseInt(page.getAttribute("height"));
            }

            var newWidth = stretch ? page.widthPx : widthWatermark;
            var newHeight = stretch ? page.heightPx : heightWatermark;

            var imageTiling = isDbsElement ? page.properties.dashboardWatermark.imageTiling : page.properties.waterMarkTiling;

            if (imageTiling) {
                imageControl.style.display = "none";
                if (page.controls.gradient) page.controls.gradient.rect.style.display = "none";
                page.style.backgroundImage = "url(" + watermarkImageSrc + ")";
                page.style.backgroundRepeat = "repeat";
                page.style.backgroundSize = parseInt((widthWatermark / page.widthPx) * 100) + "%";
            }
            else {
                page.style.backgroundImage = "";
                imageControl.style.display = "";
                imageControl.href.baseVal = watermarkImageSrc;

                if (isWmfImage && page.properties.watermarkImageContentForPaint) {
                    watermarkImageSrc = page.properties.watermarkImageContentForPaint; //Wmf image type
                }

                if (!isNaN(imageTransparency)) {
                    imageControl.style.opacity = Math.abs((imageTransparency - 255) / 255);
                }

                if (stretch && aspectRatio && widthWatermark > 0 && heightWatermark > 0) {
                    var wFactor = page.widthPx / widthWatermark;
                    var hFactor = page.heightPx / heightWatermark;
                    if (Math.abs(wFactor) < Math.abs(hFactor)) {
                        newWidth = page.widthPx;
                        newHeight = heightWatermark * wFactor;
                    }
                    else {
                        newHeight = page.heightPx;
                        newWidth = widthWatermark * hFactor;
                    }
                }

                var x = 0;
                var y = 0;
                var imageAlignment = isDbsElement ? page.properties.dashboardWatermark.imageAlignment : page.properties.waterMarkImageAlign;

                if (imageAlignment.indexOf("Center") >= 0) x = page.widthPx / 2 - newWidth / 2;
                else if (imageAlignment.indexOf("Right") >= 0) x = page.widthPx - newWidth;

                if (imageAlignment.indexOf("Middle") >= 0) y = page.heightPx / 2 - newHeight / 2;
                else if (imageAlignment.indexOf("Bottom") >= 0) y = page.heightPx - newHeight;

                imageControl.setAttribute("x", stretch && !aspectRatio ? 0 : x);
                imageControl.setAttribute("y", stretch && !aspectRatio ? 0 : y);
                imageControl.setAttribute("width", newWidth);
                imageControl.setAttribute("height", newHeight);

                if (stretch && !aspectRatio)
                    imageControl.setAttribute("preserveAspectRatio", "none");
                else
                    imageControl.removeAttribute("preserveAspectRatio");
            }

            if (page.typeComponent == "StiPanelElement") {
                page.controls.svgContent.appendChild(page.controls.waterMarkImage);
            }
        }

        if (sizeWatermark[0] == 0 || sizeWatermark[1] == 0) {
            var image = new window.Image();

            image.onload = function () {
                sizeWatermark[0] = image.width;
                sizeWatermark[1] = image.height;
                setImageSrc();
            }

            image.src = watermarkImageSrc;
        }
        else {
            setImageSrc();
        }
    }
    else {
        page.style.backgroundImage = page.style.background || "";
        imageControl.style.display = "none";
        imageControl.href.baseVal = "";
    }
}

StiMobileDesigner.prototype.RepaintPageWaterMarkWeaves = function (page) {
    if ((page.isDashboard || page.typeComponent == "StiPanelElement") && page.properties.dashboardWatermark) {
        var dashboardWatermark = page.properties.dashboardWatermark;
        var watermarkCacheKey = JSON.stringify({ watermark: dashboardWatermark, zoom: this.options.report.zoom });
        if (page.watermarkCacheKey && page.watermarkCacheKey == watermarkCacheKey) return;

        //clear old weaves
        if (page.weavesImages) {
            for (var i = 0; i < page.weavesImages.length; i++) {
                if (page.weavesImages[i].parentNode) {
                    page.weavesImages[i].parentNode.removeChild(page.weavesImages[i]);
                }
            }
        }
        page.weavesImages = [];

        var majorImage = dashboardWatermark.weaveMajorImage;
        var minorImage = dashboardWatermark.weaveMinorImage;

        if (page.typeComponent == "StiPanelElement") {
            page.widthPx = parseInt(page.getAttribute("width"));
            page.heightPx = parseInt(page.getAttribute("height"));
        }

        if (majorImage || minorImage) {
            this.PaintWatermarkWeave(page);
        }
    }
}

StiMobileDesigner.prototype.PaintWatermarkWeave = function (page) {
    var zoom = this.options.report.zoom;
    var dashboardWatermark = page.properties.dashboardWatermark;
    var dist = parseInt(dashboardWatermark.weaveDistance * zoom);
    var angle = parseInt(dashboardWatermark.weaveAngle);

    var centralX = page.widthPx / 2;
    var centralY = page.heightPx / 2;

    var posX = centralX;
    var posY = centralY;

    for (var step = 0; step < 30; step++) {
        var forwardRad = (angle + 90) * (Math.PI / 180);
        var x = posX + dist * step * Math.cos(forwardRad);
        var y = posY + dist * step * Math.sin(forwardRad);

        if (!this.DrawWeaveLine(page, dist, angle, x, y, step)) break;
    }

    posX = centralX;
    posY = centralY;

    for (var step = 1; step < 30; step++) {
        var backwardRad = (angle - 90) * (Math.PI / 180);
        var x = posX + dist * step * Math.cos(backwardRad);
        var y = posY + dist * step * Math.sin(backwardRad);

        if (!this.DrawWeaveLine(page, dist, angle, x, y, -step)) break;
    }
}

StiMobileDesigner.prototype.DrawWeaveLine = function (page, dist, angle, posX, posY, shift) {
    var isAny = false;
    var isOnce = false;
    var rad = angle * (Math.PI / 180);
    var dashboardWatermark = page.properties.dashboardWatermark;
    var imageMajor = dashboardWatermark.weaveMajorImage;
    var imageMinor = dashboardWatermark.weaveMinorImage;

    for (var step = 0; step < 30; step++) {
        var x = posX + dist * step * Math.cos(rad);
        var y = posY + dist * step * Math.sin(rad);
        var image = ((step + shift) & 1) == 0 ? imageMajor : imageMinor;

        if (image == null) continue;

        if (this.ContainsWeaveImage(page, image, angle, x, y)) {
            this.DrawWeaveImage(page, image, angle, x, y);

            isAny = true;
            isOnce = true;
        }
        else {
            if (isOnce) break;
        }
    }

    for (var step = 1; step < 30; step++) {
        var x = posX - dist * step * Math.cos(rad);
        var y = posY - dist * step * Math.sin(rad);
        var image = ((-step + shift) & 1) == 0 ? imageMajor : imageMinor;

        if (image == null) continue;

        if (this.ContainsWeaveImage(page, image, angle, x, y)) {
            this.DrawWeaveImage(page, image, angle, x, y);

            isAny = true;
            isOnce = true;
        }
        else {
            if (isOnce) break;
        }
    }

    return isAny;
}

StiMobileDesigner.prototype.ContainsWeaveImage = function (page, image, angle, x, y) {
    if (image == null)
        return false;

    var zoom = this.options.report.zoom;
    var width = image.width * zoom / 2;
    var height = image.height * zoom / 2;
    var rad = angle * (Math.PI / 180);

    var p1 = {
        x: (-width) * Math.cos(rad) + (-height) * Math.sin(rad) + x,
        y: (-width) * Math.sin(rad) - (-height) * Math.cos(rad) + y
    };

    var p2 = {
        x: width * Math.cos(rad) + (-height) * Math.sin(rad) + x,
        y: width * Math.sin(rad) - (-height) * Math.cos(rad) + y
    };

    var p3 = {
        x: (-width) * Math.cos(rad) + height * Math.sin(rad) + x,
        y: (-width) * Math.sin(rad) - height * Math.cos(rad) + y
    }

    var p4 = {
        x: width * Math.cos(rad) + height * Math.sin(rad) + x,
        y: width * Math.sin(rad) - height * Math.cos(rad) + y
    }

    return this.ContainsPointAtPageRect(page, p1) || this.ContainsPointAtPageRect(page, p2) || this.ContainsPointAtPageRect(page, p3) || this.ContainsPointAtPageRect(page, p4);
}

StiMobileDesigner.prototype.ContainsPointAtPageRect = function (page, point) {
    return (point.x >= 0 && point.x <= page.widthPx && point.y >= 0 && point.y <= page.heightPx)
}

StiMobileDesigner.prototype.DrawWeaveImage = function (page, image, angle, x, y) {
    var zoom = this.options.report.zoom;
    var width = image.width * zoom;
    var height = image.height * zoom;

    var g1 = this.CreateSvgElement("g");
    g1.setAttribute("transform", "translate(" + x + "," + y + ") rotate(" + angle + ")");

    var svg = this.CreateSvgElement("svg");
    svg.setAttribute("x", -width / 2);
    svg.setAttribute("y", -height / 2);
    svg.setAttribute("width", width);
    svg.setAttribute("height", height);
    g1.appendChild(svg);

    var rect = this.CreateSvgElement("rect");
    rect.setAttribute("x", "0");
    rect.setAttribute("y", "0");
    rect.setAttribute("width", width);
    rect.setAttribute("height", height);
    rect.setAttribute("fill", "#ffffff");
    rect.setAttribute("fill-opacity", "0");
    svg.appendChild(rect);

    var g2 = this.CreateSvgElement("g");
    svg.appendChild(g2);

    var text = this.CreateSvgElement("text");
    text.textContent = image.text;
    text.setAttribute("x", "45%");
    text.setAttribute("dy", "1em");
    text.setAttribute("text-anchor", "middle");
    text.setAttribute("font-family", "Stimulsoft");
    text.setAttribute("font-size", image.size * 3.5 * zoom);
    g2.appendChild(text);

    this.ApplyColorToElement(text, image.color);

    page.weavesImages.push(g1);

    if (page.typeComponent == "StiPanelElement") {
        var svgContent = page.controls.svgContent;
        svgContent.style.visibility = "visible";
        svgContent.appendChild(g1);
    }
    else {
        page.insertBefore(g1, page.controls.borders[0]);
    }
}

StiMobileDesigner.prototype.RepaintWaterMarkBack = function (page) {
    var backText = page.controls.waterMarkBackText;
    backText.textContent = this.getBackText(true);
    page.controls.waterMarkBackChild.setAttribute("transform", "rotate(-45)");

    var fontSize = 100 * this.options.report.zoom;
    backText.style.fontFamily = "Arial";
    backText.style.fontSize = fontSize + "pt";
    backText.style.fontWeight = "bold";
    backText.style.textAnchor = "middle";
    backText.style.fill = "rgb(0,0,0)";
    backText.style.fillOpacity = "0.3";

    page.controls.waterMarkBackParent.setAttribute("transform", "translate(" + ((page.widthPx / 2) + this.options.report.zoom * 40) + ", " + (page.heightPx / 2) + ")");
}

StiMobileDesigner.prototype.RepaintMultiSelectObjects = function (page) {
    if (this.options.multiSelectHelperControls && this.options.multiSelectHelperControls.page == page) {
        var lines = this.options.multiSelectHelperControls.lines;
        for (var i = 0; i < lines.length; i++) {
            lines[i].repaint();
        }
    }
}
