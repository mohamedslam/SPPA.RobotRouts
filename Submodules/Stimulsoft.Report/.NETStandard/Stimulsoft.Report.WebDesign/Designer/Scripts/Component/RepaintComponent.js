
//-------------------component--------------------------------
StiMobileDesigner.prototype.RepaintComponent = function (component) {
    if (this.IsTableCell(component) && component.properties.enabled == false) {
        component.style.display = "none";
    };

    var pageMarginsPx = component.properties.pageName ? this.options.report.pages[component.properties.pageName].marginsPx : [0, 0, 0, 0];
    var compLeftPx = (this.ConvertUnitToPixel(this.StrToDouble(component.properties.unitLeft), component.isDashboardElement) * this.options.report.zoom + pageMarginsPx[0]);
    var compTopPx = (this.ConvertUnitToPixel(this.StrToDouble(component.properties.unitTop), component.isDashboardElement) * this.options.report.zoom + pageMarginsPx[1]);
    var compWidthPx = (this.ConvertUnitToPixel(this.StrToDouble(component.properties.unitWidth), component.isDashboardElement) * this.options.report.zoom);
    var compHeightPx = (this.ConvertUnitToPixel(this.StrToDouble(component.properties.unitHeight), component.isDashboardElement) * this.options.report.zoom);

    component.realWidth = compWidthPx;
    component.realHeight = compHeightPx;

    var roundedPaintRect = this.GetRoundedPaintRect([compLeftPx, compTopPx, compWidthPx, compHeightPx]);

    component.setAttribute("left", roundedPaintRect[0]);
    component.setAttribute("top", roundedPaintRect[1]);
    component.setAttribute("width", roundedPaintRect[2]);
    component.setAttribute("height", roundedPaintRect[3]);
    component.setAttribute("transform", "translate(" + roundedPaintRect[0] + ", " + roundedPaintRect[1] + ")");

    if (component.isDashboardElement) {
        var zoom = this.options.report.zoom;

        //calculate margins
        if (component.properties.margin) {
            var marginArray = component.properties.margin.split(";");
            component.marginPx = [];

            for (var i = 0; i < marginArray.length; i++) {
                component.marginPx.push(parseInt(parseInt(marginArray[i]) * zoom));
            }
        }

        //calculate corner radiuses
        var cornerRadius = component.properties.cornerRadius;
        if (cornerRadius) {
            component.cornerRadius = cornerRadius.split(";");
            component.cornersIsRounded = false;

            for (var i = 0; i < component.cornerRadius.length; i++) {
                component.cornerRadius[i] = parseInt(component.cornerRadius[i] * this.options.report.zoom);
                if (component.cornerRadius[i] != 0) {
                    component.cornersIsRounded = true;
                }
            }
        }

        //calculate margins for circle button element
        if (component.typeComponent == "StiButtonElement" && component.properties.buttonShapeType == "Circle") {
            component.cornersIsRounded = true;
            var width = roundedPaintRect[2];
            var height = roundedPaintRect[3];
            var size = Math.min(width, height);
            component.cornerRadius = [size, size, size, size];
            component.marginPx[0] = component.marginPx[2] = parseInt((width - size) / 2 * zoom);
            component.marginPx[1] = component.marginPx[3] = parseInt((height - size) / 2 * zoom);
        }
    }

    if (component.typeComponent == "StiCrossTab")
        this.RepaintCrossTabFields(component);

    if (ComponentCollection[component.typeComponent][6] != "0")
        this.RepaintCorners(component);

    this.RepaintBorder(component);
    this.RepaintShadow(component);

    if (component.isDashboardElement)
        this.RepaintDbsElementBackGround(component);
    else
        this.RepaintBackGround(component);

    this.RepaintContent(component);
    this.RepaintColumnsLines(component);

    if (component.typeComponent == "StiPanelElement") {
        this.RepaintPageWaterMarkImage(component);
        this.RepaintPageWaterMark(component);
        this.RepaintPageWaterMarkWeaves(component);
    }

    if (ComponentCollection[component.typeComponent][2] != "none")
        this.RepaintHeader(component);

    if ((ComponentCollection[component.typeComponent][3] != "none" || component.typeComponent.indexOf("StiTableCell") == 0))
        this.RepaintNameContent(component);

    if (component.typeComponent == "StiHorizontalLinePrimitive" || component.typeComponent == "StiVerticalLinePrimitive")
        this.RepaintPrimitiveLine(component);

    if (component.typeComponent == "StiRectanglePrimitive")
        this.RepaintPrimitiveRectangle(component);

    if (component.typeComponent == "StiRoundedRectanglePrimitive")
        this.RepaintPrimitiveRoundedRectangle(component);

    if (component.typeComponent == "StiTableOfContents")
        this.RepaintTableOfContents(component);

    if (component.isDashboardElement) {
        this.RepaintDashboardElementButtons(component);
        this.RepaintDashboardTitleButton(component);
        this.RepaintDashboardSortButton(component);
        this.RepaintChartTypeButtons(component);
        component.style.display = component.isEmptyClientRect() ? "none" : "";
    }

    this.RepaintResizingPoints(component);
}

//-------------------Border------------------------------
StiMobileDesigner.prototype.RepaintBorder = function (component) {
    var borderStyles = ["", "9,3", "9,2,2,2", "9,2,2,2,2,2", "2,2", "", "none"];
    var borderProps = (component.properties["border"]) ? component.properties.border.split("!") : ["none"];
    var borderSize = borderProps[1];
    var borderColor = borderProps[2];
    var borderStyle = borderProps[3];
    var showBorders = borderProps[0] == "none" || borderProps[0] == "0,0,0,0" || borderStyle == "6" ? false : true;
    var advSizes = [];
    var advStyles = [];
    var advShowBorders = [];
    var borderVisibleProps = []
    if (borderProps[0] != "none") borderVisibleProps = borderProps[0].split(",");
    var advBorders = borderProps.length > 8 ? borderProps[8].split(";") : null

    for (var borderNum = 0; borderNum < 8; borderNum++) {
        //Advanced borders
        if (advBorders) {
            var propIndex = borderNum < 4 ? borderNum * 3 : (borderNum - 4) * 3;
            borderStyle = advBorders[propIndex + 2];
            borderColor = advBorders[propIndex + 1];
            borderSize = borderStyle != "5" ? advBorders[propIndex] : "1";
            showBorders = borderProps[0] == "none" || borderProps[0] == "0,0,0,0" || borderStyle == "6" ? false : true;

            if (borderNum < 4) {
                advSizes.push(borderSize);
                advStyles.push(borderStyle);
                advShowBorders.push(showBorders);
            }
        }
        component.controls.borders[borderNum].style.stroke = showBorders
            ? (borderColor == "transparent" ? "transparent" : this.GetHTMLColor(borderColor))
            : (ComponentCollection[component.typeComponent][2] != "none" ? ComponentCollection[component.typeComponent][0] : "#c0c0c0");

        if (component.typeComponent == "StiTextInCells") component.controls.borders[borderNum].style.stroke = "transparent";
        component.controls.borders[borderNum].style.strokeWidth = borderSize;

        var strokeDasharray = showBorders ? borderStyles[borderStyle] : ComponentCollection[component.typeComponent][1];
        component.controls.borders[borderNum].style.strokeDasharray = strokeDasharray == "1" ? "" : strokeDasharray;

        if (showBorders) {
            if (borderNum >= 4) component.controls.borders[borderNum].style.visibility = (borderVisibleProps[borderNum - 4] == "1" && borderStyle == "5") ? "visible" : "hidden";
            else component.controls.borders[borderNum].style.visibility = borderVisibleProps[borderNum] == "1" ? "visible" : "hidden";
        }
        else {
            component.controls.borders[borderNum].style.visibility = (ComponentCollection[component.typeComponent][1] == "none" || borderNum >= 4) ? "hidden" : "visible";
        }
    }

    var xOffsets = component.xOffsets = [];
    var yOffsets = component.yOffsets = [];
    var bordOffsets = [];
    var cornOffsets = component.cornersIsRounded && component.cornerRadius ? component.cornerRadius : [0, 0, 0, 0];

    for (var i = 0; i < 4; i++) {
        var borderSize = parseInt(showBorders && borderProps[3] != "5" ? borderProps[1] : "1");
        if (advSizes.length > 0) {
            borderSize = parseInt(advShowBorders[i] && advStyles[i] != "5" ? advSizes[i] : "1");
        }
        xOffsets[i] = (borderSize % 2 != 0) ? this.options.xOffset : 0;
        yOffsets[i] = (borderSize % 2 != 0) ? this.options.yOffset : 0;
        bordOffsets[i] = component.cornersIsRounded ? 0 : parseInt(borderSize / 2);
        if (bordOffsets[i] < 2) bordOffsets[i] = 0;
    }

    var left = 0;
    var top = 0;
    var cWidth = parseInt(component.getAttribute("width"));
    var width = cWidth;
    var cHeight = parseInt(component.getAttribute("height"));
    var height = cHeight;

    if (component.isDashboardElement && component.marginPx) {
        left += component.marginPx[0];
        top += component.marginPx[1];
        width -= component.marginPx[2];
        height -= component.marginPx[3];
        cWidth = width - component.marginPx[0];
        cHeight = height - component.marginPx[1];
    }

    //if radiuses > sizes => correct
    if (cWidth > 0 && (cornOffsets[0] + cornOffsets[1] > cWidth || cornOffsets[2] + cornOffsets[3] > cWidth)) {
        var wFactor = (cornOffsets[0] + cornOffsets[1] > cWidth ? (cornOffsets[0] + cornOffsets[1]) : (cornOffsets[2] + cornOffsets[3])) / cWidth;
        cornOffsets[0] = parseInt(cornOffsets[0] / wFactor);
        cornOffsets[1] = parseInt(cornOffsets[1] / wFactor);
        cornOffsets[2] = parseInt(cornOffsets[2] / wFactor);
        cornOffsets[3] = parseInt(cornOffsets[3] / wFactor);
    }

    if (cHeight > 0 && (cornOffsets[1] + cornOffsets[2] > cHeight || cornOffsets[0] + cornOffsets[3] > cHeight)) {
        var hFactor = (cornOffsets[1] + cornOffsets[2] > cHeight ? (cornOffsets[1] + cornOffsets[2]) : (cornOffsets[0] + cornOffsets[3])) / cHeight;
        cornOffsets[0] = parseInt(cornOffsets[0] / hFactor);
        cornOffsets[1] = parseInt(cornOffsets[1] / hFactor);
        cornOffsets[2] = parseInt(cornOffsets[2] / hFactor);
        cornOffsets[3] = parseInt(cornOffsets[3] / hFactor);
    }

    var bordersPosition = [
        [left + xOffsets[0] + bordOffsets[0], top + yOffsets[0] + cornOffsets[0], left + xOffsets[0] + bordOffsets[0], height + yOffsets[0] - cornOffsets[3]],
        [left + xOffsets[1] + cornOffsets[0], top + yOffsets[1] + bordOffsets[1], width + xOffsets[1] - cornOffsets[1], top + yOffsets[1] + bordOffsets[1]],
        [width + xOffsets[2] - bordOffsets[2], top + yOffsets[2] + cornOffsets[1], width + xOffsets[2] - bordOffsets[2], height + yOffsets[2] - cornOffsets[2]],
        [width + xOffsets[3] - cornOffsets[2], height + yOffsets[3] - bordOffsets[3], left + xOffsets[3] + cornOffsets[3], height + yOffsets[3] - bordOffsets[3]],
        [left + 2 + xOffsets[0], top + 2 + yOffsets[0] + cornOffsets[0], left + 2 + xOffsets[0], height + yOffsets[0] - 2 - cornOffsets[3]],
        [left + 2 + xOffsets[1] + cornOffsets[0], top + 2 + yOffsets[1], width + xOffsets[1] - 2 - cornOffsets[1], top + 2 + yOffsets[1]],
        [width + xOffsets[2] - 2, top + 2 + yOffsets[2] + cornOffsets[1], width + xOffsets[2] - 2, height + yOffsets[2] - 2 - cornOffsets[2]],
        [width + xOffsets[3] - 2 - cornOffsets[2], height + yOffsets[3] - 2, left + 2 + xOffsets[3] + cornOffsets[3], height + yOffsets[3] - 2]
    ];

    for (var borderNum = 0; borderNum < 8; borderNum++) {
        component.controls.borders[borderNum].setAttribute("x1", bordersPosition[borderNum][0]);
        component.controls.borders[borderNum].setAttribute("y1", bordersPosition[borderNum][1]);
        component.controls.borders[borderNum].setAttribute("x2", bordersPosition[borderNum][2]);
        component.controls.borders[borderNum].setAttribute("y2", bordersPosition[borderNum][3]);
    }
}

/* Primitives */
StiMobileDesigner.prototype.RepaintPrimitiveLine = function (component) {
    var countLines = component.properties.style == "5" ? 2 : 1;
    var lineSize = countLines == 2 ? 1 : parseInt(component.properties.size);
    lineSize = parseInt(lineSize * this.options.report.zoom);
    if (lineSize == 0) lineSize = 1;
    var spacesSize = lineSize < 2 ? 2 : lineSize;
    var borderStyles = ["", lineSize * 3 + "," + spacesSize, lineSize * 6 + "," + spacesSize + "," + spacesSize + "," + spacesSize,
        lineSize * 6 + "," + spacesSize + "," + spacesSize + "," + spacesSize + "," + spacesSize + "," + spacesSize, lineSize + "," + spacesSize, "", "none"];
    var XOffset = (lineSize % 2 != 0) ? this.options.xOffset : 0;
    var YOffset = (lineSize % 2 != 0) ? this.options.yOffset : 0;
    var modifyWidth = parseInt(component.getAttribute("width"));
    var modifyHeight = parseInt(component.getAttribute("height"));
    var startPos = countLines == 2 ? 0 : 1;

    for (var i = 0; i < countLines; i++) {
        var line = component.controls.borders[i];
        line.style.strokeWidth = lineSize;
        line.style.stroke = borderStyles[component.properties.style] == "none" ? "transparent" : this.GetHTMLColor(component.properties.color);
        var strokeDasharray = borderStyles[component.properties.style];
        line.style.strokeDasharray = strokeDasharray == "1" ? "" : strokeDasharray;
        line.style.visibility = "visible";
        var isHorLine = component.typeComponent == "StiHorizontalLinePrimitive";
        line.setAttribute("x1", isHorLine ? XOffset : -1 + XOffset + (i == 0 ? startPos : 2));
        line.setAttribute("y1", isHorLine ? -1 + YOffset + (i == 0 ? startPos : 2) : 0 + YOffset);
        line.setAttribute("x2", isHorLine ? modifyWidth + XOffset : -1 + XOffset + (i == 0 ? startPos : 2));
        line.setAttribute("y2", isHorLine ? -1 + YOffset + (i == 0 ? startPos : 2) : modifyHeight + YOffset);
    }
}

StiMobileDesigner.prototype.RepaintPrimitiveRectangle = function (component) {
    var countLines = component.properties.style == "5" ? 8 : 4;
    var lineSize = countLines == 8 ? 1 : parseInt(component.properties.size);
    lineSize = parseInt(lineSize * this.options.report.zoom);
    if (lineSize == 0) lineSize = 1;
    var spacesSize = lineSize < 2 ? 2 : lineSize;
    var borderStyles = ["", lineSize * 3 + "," + spacesSize, lineSize * 6 + "," + spacesSize + "," + spacesSize + "," + spacesSize,
        lineSize * 6 + "," + spacesSize + "," + spacesSize + "," + spacesSize + "," + spacesSize + "," + spacesSize, lineSize + "," + spacesSize, "", "none"];
    var XOffset = (lineSize % 2 != 0) ? this.options.xOffset : 0;
    var YOffset = (lineSize % 2 != 0) ? this.options.yOffset : 0;
    var modifyWidth = parseInt(component.getAttribute("width"));
    var modifyHeight = parseInt(component.getAttribute("height"));

    var linePositions = [
        [0 + XOffset, 0 + YOffset, 0 + XOffset, modifyHeight + YOffset],
        [0 + XOffset, 0 + YOffset, modifyWidth + XOffset, 0 + YOffset],
        [modifyWidth + XOffset, 0 + YOffset, modifyWidth + XOffset, modifyHeight + YOffset],
        [modifyWidth + XOffset, modifyHeight + YOffset, 0 + XOffset, modifyHeight + YOffset],
        [2 + XOffset, 2 + YOffset, 2 + XOffset, modifyHeight + YOffset - 2],
        [2 + XOffset, 2 + YOffset, modifyWidth + XOffset - 2, 2 + YOffset],
        [modifyWidth + XOffset - 2, 2 + YOffset, modifyWidth + XOffset - 2, modifyHeight + YOffset - 2],
        [modifyWidth + XOffset - 2, modifyHeight + YOffset - 2, 2 + XOffset, modifyHeight + YOffset - 2]
    ];

    for (var i = 0; i < countLines; i++) {
        var line = component.controls.borders[i];
        line.style.strokeWidth = lineSize;
        line.style.stroke = borderStyles[component.properties.style] == "none" ? "transparent" : this.GetHTMLColor(component.properties.color);

        var strokeDasharray = borderStyles[component.properties.style];
        line.style.strokeDasharray = strokeDasharray == "1" ? "" : strokeDasharray;
        line.style.visibility = ((component.properties.leftSide && (i == 0 || i == 4)) || (component.properties.rightSide && (i == 2 || i == 6)) ||
            (component.properties.topSide && (i == 1 || i == 5)) || (component.properties.bottomSide && (i == 3 || i == 7))) ? "visible" : "hidden";
        line.setAttribute("x1", linePositions[i][0]);
        line.setAttribute("y1", linePositions[i][1]);
        line.setAttribute("x2", linePositions[i][2]);
        line.setAttribute("y2", linePositions[i][3]);
    }
}

StiMobileDesigner.prototype.RepaintPrimitiveRoundedRectangle = function (component) {
    var countRect = component.properties.style == "5" ? 2 : 1;
    var lineSize = countRect == 2 ? 1 : parseInt(component.properties.size);
    lineSize = parseInt(lineSize * this.options.report.zoom);
    if (lineSize == 0) lineSize = 1;
    var spacesSize = lineSize < 2 ? 2 : lineSize;
    var borderStyles = ["", lineSize * 3 + "," + spacesSize, lineSize * 6 + "," + spacesSize + "," + spacesSize + "," + spacesSize,
        lineSize * 6 + "," + spacesSize + "," + spacesSize + "," + spacesSize + "," + spacesSize + "," + spacesSize, lineSize + "," + spacesSize, "", "none"];
    var XOffset = (lineSize % 2 != 0) ? this.options.xOffset : 0;
    var YOffset = (lineSize % 2 != 0) ? this.options.yOffset : 0;

    if (!component.controls.roundedRectangles) {
        component.controls.roundedRectangles = [];
        for (var i = 1; i <= 2; i++) {
            var rect = ("createElementNS" in document) ? document.createElementNS('http://www.w3.org/2000/svg', 'rect') : document.createElement("rect");
            component.controls.roundedRectangles.push(rect);
            component.appendChild(rect);
        }
    }

    component.controls.roundedRectangles[1].style.visibility = countRect == 2 ? "visible" : "hidden";

    for (var i = 0; i < countRect; i++) {
        var rect = component.controls.roundedRectangles[i];
        var width = parseInt(component.getAttribute("width"));
        var height = parseInt(component.getAttribute("height"));
        rect.setAttribute("width", i == 0 ? width : width - 4);
        rect.setAttribute("height", i == 0 ? height : height - 4);
        rect.setAttribute("x", i == 0 ? 0 + XOffset : 2 + XOffset);
        rect.setAttribute("y", i == 0 ? 0 + YOffset : 2 + YOffset);
        var round = this.StrToDouble(component.properties.round);
        rect.setAttribute("rx", round * 100);
        rect.setAttribute("ry", round * 100);
        rect.style.fill = "none";
        rect.style.stroke = borderStyles[component.properties.style] == "none" ? "transparent" : this.GetHTMLColor(component.properties.color);
        rect.style.strokeWidth = lineSize;
        var strokeDasharray = borderStyles[component.properties.style];
        rect.style.strokeDasharray = strokeDasharray == "1" ? "" : strokeDasharray;
    }
}

//--------------Resizing Points---------------------------------
StiMobileDesigner.prototype.RepaintResizingPoints = function (component) {
    var jsObject = this;
    var resizingPoints = component.controls.resizingPoints;
    if (!resizingPoints) return;
    var modifyWidth = parseInt(component.getAttribute("width"));
    var modifyHeight = parseInt(component.getAttribute("height"));
    var smallSize = jsObject.options.isTouchDevice ? 30 : 15;
    var offset = modifyWidth < smallSize || modifyHeight < smallSize ? 5 : 0;

    for (var i = 0; i <= 7; i++) {
        var resizingPoint = resizingPoints[i];
        if (!resizingPoint) {
            continue;
        }
        else {
            if (component.isDashboardElement) {
                var dashboard = jsObject.options.report && jsObject.options.report.pages[component.properties.pageName];
                if (dashboard && dashboard.properties.selectionCornerColor) {
                    if (jsObject.options.isTouchDevice && resizingPoint.circle)
                        resizingPoint.circle.setAttribute("fill", component.properties.locked ? "red" : "#696969");
                    else
                        resizingPoint.style.stroke = resizingPoint.style.fill = component.properties.locked ? "red" : jsObject.GetHTMLColor(dashboard.properties.selectionCornerColor);
                }
            }
            else {
                var isBand = jsObject.IsBandComponent(component) || jsObject.IsCrossBandComponent(component);
                if (jsObject.options.isTouchDevice && resizingPoint.circle)
                    resizingPoint.circle.setAttribute("fill", component.properties.locked && !jsObject.IsTableCell(component) ? "red" : (isBand ? (resizingPoint.circle.isCircle ? "#ffffff" : "#696969") : "#696969"));
                else
                    resizingPoint.style.stroke = component.properties.locked && !jsObject.IsTableCell(component) ? "red" : "#696969";
            }

            var width = parseInt(resizingPoint.getAttribute("width"));
            var height = parseInt(resizingPoint.getAttribute("height"));

            if (i == 0 || i == 6 || i == 7) resizingPoint.setAttribute("x", parseInt(- width / 2) + jsObject.options.xOffset - offset);
            if (i == 2 || i == 3 || i == 4) resizingPoint.setAttribute("x", parseInt(modifyWidth - width / 2) + jsObject.options.xOffset + offset);
            if (i == 1 || i == 5) {
                var x = parseInt(modifyWidth / 2 - width / 2) + jsObject.options.xOffset;
                if (component.typeComponent == "StiVerticalLinePrimitive") x--;
                resizingPoint.setAttribute("x", x);
            }
            if (i == 0 || i == 1 || i == 2) resizingPoint.setAttribute("y", parseInt(-height / 2) + jsObject.options.yOffset - offset);
            if (i == 4 || i == 5 || i == 6) resizingPoint.setAttribute("y", parseInt(modifyHeight - height / 2) + jsObject.options.yOffset + offset);
            if (i == 3 || i == 7) {
                var y = parseInt(modifyHeight / 2 - height / 2) + jsObject.options.yOffset;
                if (component.typeComponent == "StiHorizontalLinePrimitive") y--;
                resizingPoint.setAttribute("y", y);
            }
        }
    }
}

//--------------Dashboard ---------------------------------
StiMobileDesigner.prototype.RepaintDashboardElementButtons = function (component) {
    var modifyWidth = parseInt(component.getAttribute("width"));
    var typeComp = component.typeComponent;
    var editDbsButton = component.controls.editDbsButton;
    var filtersDbsButton = component.controls.filtersDbsButton;
    var changeTypeDbsButton = component.controls.changeTypeDbsButton;
    var topNDbsButton = component.controls.topNDbsButton;

    editDbsButton.style.display = typeComp != "StiPanelElement" ? "" : "none";
    filtersDbsButton.style.display = changeTypeDbsButton.style.display = typeComp != "StiPanelElement" && typeComp != "StiImageElement" && typeComp != "StiTextElement" && typeComp != "StiShapeElement" && typeComp != "StiButtonElement" ? "" : "none";
    topNDbsButton.style.display = component.properties.topN && !component.properties.isScatterChart ? "" : "none";

    if (editDbsButton.style.display == "" || filtersDbsButton.style.display == "" || changeTypeDbsButton.style.display == "" || topNDbsButton.style.display == "") {
        var xPos = modifyWidth + 8;
        var yPos = 3;
        var dashboard = this.options.report && this.options.report.pages[component.properties.pageName];
        var flipButtons = dashboard && parseInt(component.getAttribute("left")) + xPos + 28 > dashboard.widthPx;

        //flip buttons if crossed with online map frame
        if (!flipButtons && this.options.report && component.properties.pageName) {
            var components = this.options.report.pages[component.properties.pageName].components;
            for (var compName in components) {
                var comp = components[compName];
                if (comp.typeComponent == "StiOnlineMapElement" && comp != component) {
                    if (parseInt(component.getAttribute("left")) + xPos + 28 > parseInt(comp.getAttribute("left")) &&
                        parseInt(component.getAttribute("top")) + 100 > parseInt(comp.getAttribute("top")) &&
                        parseInt(component.getAttribute("top")) < parseInt(comp.getAttribute("top")) + parseInt(component.getAttribute("height"))) {
                        flipButtons = true;
                    }
                }
            }
        }

        if (flipButtons) {
            if ((parseInt(component.getAttribute("left")) - 44) > 0) {
                xPos = - 36;
            }
            else {
                xPos = 8;
                yPos += 4;
            }
        }

        if (editDbsButton && editDbsButton.style.display == "") {
            editDbsButton.setAttribute("x", xPos);
            editDbsButton.setAttribute("y", yPos);
            yPos += 35;
        }
        if (filtersDbsButton && filtersDbsButton.style.display == "") {
            filtersDbsButton.setAttribute("x", xPos);
            filtersDbsButton.setAttribute("y", yPos);
            yPos += 35;
        }
        if (topNDbsButton && topNDbsButton.style.display == "") {
            topNDbsButton.setAttribute("x", xPos);
            topNDbsButton.setAttribute("y", yPos);
            yPos += 35;
        }
        if (changeTypeDbsButton && changeTypeDbsButton.style.display == "") {
            changeTypeDbsButton.setAttribute("x", xPos);
            changeTypeDbsButton.setAttribute("y", yPos);
        }
    }
}

StiMobileDesigner.prototype.RepaintDashboardTitleButton = function (component) {
    var modifyWidth = parseInt(component.getAttribute("width"));
    var titleButton = component.controls.titleButton;
    titleButton.style.display = component.properties.titleVisible != null ? "" : "none";
    if (titleButton && titleButton.style.display == "" && component.marginPx) {
        var zoom = this.options.report.zoom;
        titleButton.setAttribute("x", modifyWidth - component.marginPx[2] - 20 - parseInt(6 * zoom));
        titleButton.setAttribute("y", component.marginPx[1] + parseInt(6 * zoom));
        titleButton.innerRect.style.fill = titleButton.rect.style.stroke = component.properties.isDarkStyle ? "#ffffff" : "#ababab";
    }
}

StiMobileDesigner.prototype.RepaintDashboardSortButton = function (component) {
    var modifyWidth = parseInt(component.getAttribute("width"));
    var typeComp = component.typeComponent;
    var sortButton = component.controls.sortButton;
    sortButton.style.display = typeComp == "StiChartElement" || typeComp == "StiIndicatorElement" || typeComp == "StiProgressElement" || typeComp == "StiGaugeElement" ? "" : "none";

    if (sortButton && sortButton.style.display == "" && component.marginPx) {
        var zoom = this.options.report.zoom;
        sortButton.setAttribute("x", modifyWidth - component.marginPx[2] - 40 - parseInt(8 * zoom));
        sortButton.setAttribute("y", component.marginPx[1] + parseInt(6 * zoom));
        StiMobileDesigner.setImageSource(sortButton.img, this.options, "Dashboards.Actions." + (component.properties.isDarkStyle ? "Dark" : "Light") + ".Sort.png");
    }
}

StiMobileDesigner.prototype.RepaintChartTypeButtons = function (component) {
    var jsObject = this;

    if (component.controls.chartTypesButtons) {
        for (var i = 0; i < component.controls.chartTypesButtons.length; i++) {
            var button = component.controls.chartTypesButtons[i];
            component.removeChild(button);
        }
        component.controls.chartTypesButtons = [];
    }

    if (component.typeComponent == "StiChartElement") {
        if (component.properties.userViewStates && component.properties.userViewStates.length > 1 && component.properties.dataMode != "ManuallyEnteringData") {
            if (!component.controls.chartTypesButtons) component.controls.chartTypesButtons = [];
            var modifyWidth = parseInt(component.getAttribute("width"));
            var zoom = jsObject.options.report.zoom;
            var startLeftPos = modifyWidth - component.marginPx[2];
            var buttonWidth = 20 + parseInt(4 * zoom);
            if (component.controls.titleButton && component.controls.titleButton.style.display == "") startLeftPos -= buttonWidth;
            if (component.controls.sortButton && component.controls.sortButton.style.display == "") startLeftPos -= buttonWidth;

            for (var i = component.properties.userViewStates.length - 1; i >= 0; i--) {
                var index = component.properties.userViewStates.length - 1 - i;
                var viewState = component.properties.userViewStates[i];
                var button = this.CreateDashboardElementToolButton(component, viewState.seriesType, "ChartSeries." + (component.properties.isDarkStyle ? "Dark." : "Light.") + viewState.seriesType + ".png");
                button.setAttribute("x", startLeftPos - buttonWidth * (index + 1));
                button.setAttribute("y", startLeftPos - buttonWidth * (index + 1));
                button.setAttribute("y", component.marginPx[1] + parseInt(6 * zoom));
                button.viewState = viewState;

                button.setSelected = function (state) {
                    this.isSelected = state;
                    this.backRect.style.stroke = state ? (component.properties.isDarkStyle ? "#ffffff" : "#ababab") : "transparent";
                }

                button.onmouseout = function (e) {
                    this.backRect.style.fill = "transparent";
                };

                button.action = function () {
                    for (var i = 0; i < component.controls.chartTypesButtons.length; i++) {
                        component.controls.chartTypesButtons[i].setSelected(false);
                    }
                    this.setSelected(true);
                    var currViewState = this.viewState;

                    jsObject.SendCommandToDesignerServer("UpdateChartElement",
                        {
                            componentName: component.properties.name,
                            updateParameters: {
                                command: "SetUserViewState",
                                viewStateKey: currViewState.key,
                                seriesType: currViewState.seriesType
                            }
                        },
                        function (answer) {
                            if (answer.elementProperties) {
                                for (var propertyName in answer.elementProperties) {
                                    component.properties[propertyName] = answer.elementProperties[propertyName];
                                }

                                jsObject.RemoveStylesFromCache(component.properties.name);
                                jsObject.options.homePanel.updateControls();

                                if (jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.editDbsMeterMode) {
                                    jsObject.options.propertiesPanel.updateControls();
                                }
                            }
                            component.repaint();
                            var chartForm = jsObject.options.forms.editChartElementForm;
                            if (chartForm && chartForm.visible && chartForm.currentChartElement == component) {
                                chartForm.chartElementProperties = answer.elementProperties;
                                chartForm.setValues();
                                chartForm.updateControlsVisibleStates();
                                chartForm.checkStartMode();
                                chartForm.correctTopPosition();
                            }
                        });
                }

                button.setSelected(viewState.key == component.properties.selectedViewStateKey);

                component.appendChild(button);
                component.controls.chartTypesButtons.push(button);
            }
        }
    }
}

//-------------------Corners--------------------------------------
StiMobileDesigner.prototype.RepaintCorners = function (component) {
    var borderProps = component.properties.border ? component.properties.border.split("!") : ["none"];
    var showAllBorder = borderProps[0] == "1,1,1,1" ? true : false;
    var corners = component.controls.corners;

    for (var i = 0; i < 4; i++) {
        corners[i].style.display = showAllBorder ? "none" : "";
    }

    if (showAllBorder) return;

    var modifyWidth = parseInt(component.getAttribute("width")) + this.options.xOffset;
    var modifyHeight = parseInt(component.getAttribute("height")) + this.options.yOffset;

    var widthCorn = 4 + this.options.xOffset;
    var heightCorn = 4 + this.options.yOffset;

    var tempX = modifyWidth - widthCorn;
    var tempY = modifyHeight - heightCorn;

    corners[0].setAttribute("points", this.options.xOffset + " " + heightCorn + ", " + this.options.xOffset + " " + this.options.yOffset + ", " + widthCorn + " " + this.options.yOffset);
    corners[1].setAttribute("points", tempX + " " + this.options.yOffset + ", " + modifyWidth + " " + this.options.yOffset + ", " + modifyWidth + " " + heightCorn);
    corners[2].setAttribute("points", modifyWidth + " " + tempY + ", " + modifyWidth + " " + modifyHeight + ", " + tempX + " " + modifyHeight);
    corners[3].setAttribute("points", this.options.xOffset + " " + tempY + ", " + this.options.xOffset + " " + modifyHeight + ", " + widthCorn + " " + modifyHeight);
}

//---------------------BackGround--------------------------------
StiMobileDesigner.prototype.RepaintComponentComplexBackground = function (component, brush, x, y, width, height) {
    if (brush) {
        var brushArray = brush.split("!");

        switch (brushArray[0]) {
            case "2": {
                var hatchBrush = component.jsObject.GetSvgHatchBrush(brushArray, x, y, width, height);
                component.controls.svgHatchBrush.appendChild(hatchBrush);

                if (component && component.cornersIsRounded) {
                    var backPathes = component.controls.backPathes;
                    var path = this.CreateSvgElement("path");
                    path.setAttribute("d", backPathes[0].getAttribute("d"));
                    path.setAttribute("fill", hatchBrush.rect.getAttribute("fill"));
                    hatchBrush.appendChild(path);
                    hatchBrush.rect.style.visibility = "hidden";
                }
                break;
            }
            case "3":
            case "4": {
                var grad = component.controls.gradient;
                grad.applyBrush(brushArray, component.cornersIsRounded && component.cornerRadius);

                if (component.cornersIsRounded && component.cornerRadius) {
                    var backPathes = component.controls.backPathes;
                    grad.path.setAttribute("d", backPathes[0].getAttribute("d"));
                }
                else {
                    grad.rect.setAttribute("x", x);
                    grad.rect.setAttribute("y", y);
                    grad.rect.setAttribute("width", width);
                    grad.rect.setAttribute("height", height);
                }
                break;
            }
        }
    }
}

StiMobileDesigner.prototype.RepaintDbsElementBackGround = function (component) {
    //remove old complex brushes
    var grad = component.controls.gradient;
    if (grad)
        grad.rect.style.display = grad.path.style.display = "none";

    if (component.controls.svgHatchBrush)
        component.controls.svgHatchBrush.clear();

    //simple brush
    var backRect = component.controls.background;
    var x = 0;
    var y = 0;
    var width = parseInt(component.getAttribute("width"));
    var height = parseInt(component.getAttribute("height"));

    var isSimpleBrush = true;
    var backColor = component.properties.realBackColor;
    var brush = component.properties.brush;

    if (component.marginPx) {
        x += component.marginPx[0];
        y += component.marginPx[1];
        width -= (component.marginPx[0] + component.marginPx[2]);
        height -= (component.marginPx[1] + component.marginPx[3]);
    }

    backRect.setAttribute("x", x);
    backRect.setAttribute("y", y);
    backRect.setAttribute("width", Math.max(0, width));
    backRect.setAttribute("height", Math.max(0, height));

    //button action colors
    if (component.typeComponent == "StiButtonElement" && component.properties.buttonVisualStates) {
        if (component.isOver) brush = component.properties.buttonVisualStates.hover.brush;

        if (brush == "isStyleBrush")
            brush = "1!" + (component.isOver ? component.properties.styleColors.hoverBackColor : component.properties.styleColors.backColor);
        else if (brush == "isDefaultBrush")
            brush = component.properties.brush == "isStyleBrush" ? ("1!" + component.properties.styleColors[component.isOver ? "hoverBackColor" : "backColor"]) : component.properties.brush;

        var brushArray = brush.split("!");
        if (brushArray[0] == "0" || brushArray[0] == "1" || brushArray[0] == "5") {
            backColor = brushArray[0] == "0" ? "transparent" : brushArray[1];
        }
        else {
            isSimpleBrush = false;
            backColor = "transparent";
        }
    }

    backRect.style.fill = this.GetHTMLColor(backColor);
    backRect.style.opacity = backRect.style.fill == "transparent" ? 0 : 1;

    //Corners rounded
    var backPathes = component.controls.backPathes;
    if (backPathes) {
        for (var i = 0; i < 5; i++) {
            backPathes[i].style.visibility = "hidden";
        }
        if (component.cornersIsRounded && component.cornerRadius) {
            var cr = component.cornerRadius;
            var mainPath = backPathes[0];
            mainPath.style.visibility = "visible";
            mainPath.style.fill = backRect.style.fill;
            backRect.style.fill = "transparent";
            backRect.style.opacity = 0;

            var yOffsets = component.yOffsets;
            var xOffsets = component.yOffsets;

            var d = "M" + (cr[0] + x) + "," + y + " h" + (width - cr[0] - cr[1]) + " a" + cr[1] + "," + cr[1] + " 0 0 1 " + cr[1] + "," + cr[1];
            d += " v" + (height - cr[1] - cr[2]) + " a" + cr[2] + "," + cr[2] + " 0 0 1 -" + cr[2] + "," + cr[2];
            d += " h-" + Math.abs(width - cr[2] - cr[3]) + " a" + cr[3] + "," + cr[3] + " 0 0 1 -" + cr[3] + ",-" + cr[3];
            d += " v-" + Math.abs(height - cr[3] - cr[0]) + " a" + cr[0] + "," + cr[0] + " 0 0 1 " + cr[0] + ",-" + cr[0] + " z";
            mainPath.setAttribute("d", d);

            var borders = component.controls.borders;
            var pathD = [
                "M" + (x + xOffsets[0]) + "," + (y + cr[0] + yOffsets[0]) + " a" + cr[0] + "," + cr[0] + " 0 0 1 " + cr[0] + ",-" + cr[0],
                "M" + (x + width - cr[1] + xOffsets[1]) + "," + (y + + yOffsets[1]) + " a" + cr[1] + "," + cr[1] + " 0 0 1 " + cr[1] + "," + cr[1],
                "M" + (x + width + xOffsets[2]) + "," + (y + height - cr[2] + yOffsets[2]) + " a" + cr[2] + "," + cr[2] + " 0 0 1 -" + cr[2] + "," + cr[2],
                "M" + (x + cr[3] + xOffsets[3]) + "," + (y + height + yOffsets[3]) + " a" + cr[3] + "," + cr[3] + " 0 0 1 -" + cr[3] + ",-" + cr[3]
            ];

            for (var i = 0; i < 4; i++) {
                var nextIndex = i + 1;
                if (nextIndex > 3) nextIndex = 0;
                var baseBorder = borders[i].style.visibility == "visible" ? borders[nextIndex] : (borders[nextIndex].style.visibility == "visible" ? borders[nextIndex] : null);
                if (baseBorder) {
                    var path = backPathes[i + 1];
                    path.style.visibility = "visible";
                    path.style.stroke = baseBorder.style.stroke;
                    path.style.strokeWidth = baseBorder.style.strokeWidth;
                    path.style.strokeDasharray = baseBorder.style.strokeDasharray;
                    path.style.fill = "transparent";
                    path.setAttribute("fill", "transparent");
                    path.setAttribute("d", pathD[i]);
                }
            }
        }
    }

    //complex brushes
    if (component.typeComponent == "StiButtonElement" && !isSimpleBrush) {
        this.RepaintComponentComplexBackground(component, brush, x, y, width, height);
    }
}

StiMobileDesigner.prototype.RepaintBackGround = function (component) {
    var backRect = component.controls.background;
    var width = parseInt(component.getAttribute("width"));
    var height = parseInt(component.getAttribute("height"));

    backRect.setAttribute("x", 0);
    backRect.setAttribute("y", 0);
    backRect.setAttribute("width", Math.max(0, width));
    backRect.setAttribute("height", Math.max(0, height));
    backRect.style.fill = "transparent";

    //Primitive components
    if (component.properties.isPrimitiveComponent) {
        var lineSize = parseInt(component.properties.size);
        backRect.style.opacity = 0.5;

        if (component.typeComponent == "StiRectanglePrimitive" || component.typeComponent == "StiRoundedRectanglePrimitive") {
            backRect.style.fill = "none";
            backRect.style.strokeWidth = lineSize + 4;

            if (component.typeComponent == "StiRoundedRectanglePrimitive") {
                var round = this.StrToDouble(component.properties.round);
                backRect.setAttribute("rx", round * 100);
                backRect.setAttribute("ry", round * 100);
            }

            component.onmouseover = function () {
                if (!this.jsObject.options.in_drag && !this.jsObject.options.in_resize) {
                    backRect.style.stroke = "rgb(255,231,105)";
                }
            }

            component.onmouseout = function () {
                backRect.style.stroke = "transparent";
            }
        }
        else {
            backRect.setAttribute("width", width + lineSize + 3);
            backRect.setAttribute("height", height + lineSize + 3);
            backRect.setAttribute("x", -2 - lineSize / 2);
            backRect.setAttribute("y", -2 - lineSize / 2);

            component.onmouseover = function () {
                if (!this.jsObject.options.in_drag && !this.jsObject.options.in_resize) {
                    backRect.style.fill = "rgb(255,231,105)";
                }
            }

            component.onmouseout = function () {
                backRect.style.fill = "transparent";
            }
        }

        return;
    }

    //Exception components
    if (component.typeComponent == "StiShape" || component.typeComponent == "StiShapeElement" || component.typeComponent == "StiTextInCells" || !component.properties["brush"]) {
        return;
    }

    var brushArray = component.properties.brush.split("!");

    switch (brushArray[0]) {
        case "0": {
            backRect.style.fill = "transparent";
            break;
        }
        case "1": {
            if (brushArray[1] == "transparent") {
                var defaultColor = ComponentCollection[component.typeComponent][0];
                backRect.style.fill = defaultColor;

                if (!component.isDashboardElement) {
                    backRect.style.opacity = (component.typeComponent == "StiText" || component.typeComponent == "StiTextInCells") ? 0.35 : 0.15;
                }
            }
            else {
                var colors = brushArray[1].split(",");
                if (colors.length == 3) {
                    backRect.style.fill = "rgb(" + colors[0] + "," + colors[1] + "," + colors[2] + ")";
                    backRect.style.opacity = 1;
                }
                else if (colors.length == 4) {
                    backRect.style.fill = "rgb(" + colors[1] + "," + colors[2] + "," + colors[3] + ")";
                    backRect.style.opacity = this.StrToInt(colors[0]) / 255;
                }
            }
            break;
        }
        case "2":
        case "3":
        case "4":
        case "5": {
            backRect.style.fill = "transparent";
            backRect.style.opacity = 1;
            break;
        }
    }

    //-------------- Conditions Icon ---------------------------------
    if (component.properties.conditions) {
        var cIcon = component.controls.conditionIcon;
        if (!cIcon) {
            cIcon = this.CreateSvgElement("image");
            component.appendChild(cIcon);
            component.controls.conditionIcon = cIcon;
            cIcon.setAttribute("height", 8);
            cIcon.setAttribute("width", 9);
            StiMobileDesigner.setImageSource(cIcon, this.options, "SmallCondition.png");
        }
        cIcon.setAttribute("x", 0);
        cIcon.setAttribute("y", parseInt(component.getAttribute("height")) - 9);
    }
    else {
        if (component.controls.conditionIcon) {
            component.removeChild(component.controls.conditionIcon);
            component.controls.conditionIcon = null;
        }
    }
}

//---------------------Header--------------------------------------
StiMobileDesigner.prototype.RepaintHeader = function (component) {
    var header = component.controls.header;

    if (!this.options.report.info.showHeaders) {
        header.style.display = "none";
        return;
    }

    var headerType = ComponentCollection[component.typeComponent][2];
    var backGroundColor = ComponentCollection[component.typeComponent][0];

    if (headerType != "none") {
        header.style.fill = backGroundColor;
        header.style.stroke = backGroundColor;
        header.setAttribute("width", component.getAttribute("width"));
        header.setAttribute("x", this.options.xOffset);
        header.style.opacity = 0.8;

        var headerSize = this.StrToInt((component.properties.headerSize ? this.StrToDouble(component.properties.headerSize) : 0) * this.options.report.zoom);

        if (headerType == "up") {
            header.setAttribute("y", -headerSize + this.options.yOffset - 1);
        }
        else {
            var height = parseInt(component.getAttribute("height"));
            header.setAttribute("y", height + this.options.yOffset);
        }

        header.setAttribute("height", headerSize + 1);
    }
}

//---------------------NameContent----------------------------------
StiMobileDesigner.prototype.RepaintNameContent = function (component) {
    var compWidth = parseInt(component.getAttribute("width"));
    var compHeight = parseInt(component.getAttribute("height"));
    var nameText = component.controls.nameText;
    var nameContent = component.controls.nameContent;
    var typeComponent = component.typeComponent;
    if (nameText) nameText.textContent = "";
    var typeNameContent = ComponentCollection[component.typeComponent][3];
    if (typeNameContent == "none") return false;

    if (typeNameContent == "up" || typeNameContent == "down") {
        var headerSize = parseInt(component.controls.header.getAttribute("height"));
        if (headerSize) {
            nameContent.setAttribute("height", headerSize);
            nameContent.setAttribute("x", "0");
            nameContent.setAttribute("y", typeNameContent == "up" ? -headerSize : compHeight);

            var fontSize = Math.round(12 * this.options.report.zoom);
            nameText.style.fontSize = fontSize + "px";
            nameText.setAttribute("x", "2");
            nameText.setAttribute("y", Math.round(headerSize - (headerSize - fontSize) / 1.5));
        }
    }
    else if (typeNameContent == "center") {
        var bigFont = typeComponent != "StiImage" && typeComponent != "StiTableCellImage" && typeComponent != "StiRichText" && typeComponent != "StiTableCellRichText";
        var fontSize = Math.round((bigFont ? 18 : 12) * this.options.report.zoom);
        nameText.style.fontSize = fontSize + "px";
        component.controls.nameContent.setAttribute("height", compHeight);
        nameText.setAttribute("y", fontSize);
        nameText.setAttribute("x", "2");
        if (bigFont) {
            nameText.style.stroke = "none";
            nameText.style.fill = "#808080";
        }
    }

    nameContent.setAttribute("width", compWidth);

    if (typeComponent == "StiImage" || typeComponent == "StiTableCellImage" || typeComponent == "StiImageElement" || typeComponent == "StiRichText" || typeComponent == "StiTableCellRichText") {
        var imageDataColumn = StiBase64.decode(component.properties.imageDataColumn || component.properties.richTextDataColumn || "");
        var imageUrl = StiBase64.decode(component.properties.imageUrl || component.properties.richTextUrl || "");
        var imageData = StiBase64.decode(component.properties.imageData || "");
        var imageFile = StiBase64.decode(component.properties.imageFile || "");

        var text = imageDataColumn
            ? this.loc.PropertyMain.DataColumn + ": " + this.ReplaceRelationsToShortNames(imageDataColumn)
            : (imageUrl && typeComponent != "StiImageElement")
                ? (typeComponent == "StiRichText" || typeComponent == "StiTableCellRichText" ? "URL" : this.loc.PropertyMain.ImageURL) + ": " + imageUrl
                : imageData
                    ? this.loc.PropertyMain.ImageData + ": " + imageData
                    : imageFile
                        ? this.loc.MainMenu.menuFile.replace("&", "") + ": " + imageFile
                        : "";

        if ((typeComponent == "StiRichText" && component.properties.svgContent) || (typeComponent == "StiImage" || typeComponent == "StiTableCellImage") &&
            component.properties.imageContentForPaint && imageUrl.indexOf(this.options.cloudServerUrl) < 0) text = "";

        nameText.textContent = text;
    }
    else if (typeComponent == "StiSubReport") {
        var text = component.properties.subReportPage && component.properties.subReportPage != "[Not Assigned]"
            ? this.loc.Components.StiSubReport + ":  " + component.properties.subReportPage
            : component.properties.subReportUrl
                ? StiBase64.decode(component.properties.subReportUrl)
                : this.loc.Components.StiSubReport + ": " + this.loc.Report.NotAssigned;

        var textControl = component.controls.nameText;
        while (textControl.childNodes[0]) textControl.removeChild(textControl.childNodes[0]);
        var tspan1 = this.CreateSvgElement('tspan');
        var tspan2 = this.CreateSvgElement('tspan');
        textControl.appendChild(tspan1);
        textControl.appendChild(tspan2);
        tspan2.setAttribute("x", "3");
        tspan2.setAttribute("dy", 22 * this.options.report.zoom);
        tspan1.textContent = component.properties.name || this.loc.Components.StiSubReport;
        tspan2.textContent = text || "";
    }
    else if (!component.isDashboardElement) {
        nameText.textContent = component.properties.name;
        var aliasName = StiBase64.decode(component.properties.aliasName);
        if (aliasName) nameText.textContent = component.properties.name + " [" + aliasName + "] ";
    }

    if (typeComponent == "StiDataBand" || typeComponent == "StiHierarchicalBand" || typeComponent == "StiTable") {
        var notAssignedText = this.loc.Report.NotAssigned;

        if (component.properties.businessObject != null && component.properties.businessObject != "[Not Assigned]") {
            nameText.textContent += "; " + this.loc.PropertyMain.BusinessObject +
                ": " + (component.properties.businessObject == "[Not Assigned]" ? notAssignedText : component.properties.businessObject);
        }
        else {
            nameText.textContent += "; " + this.loc.PropertyMain.DataSource +
                ": " + (component.properties.dataSource == "[Not Assigned]"
                    ? (component.properties.countData == 0 ? notAssignedText : component.properties.countData)
                    : component.properties.dataSource);
        }

        if (component.properties.masterComponent != null && component.properties.masterComponent != "[Not Assigned]") {
            nameText.textContent += "; " + this.loc.PropertyMain.MasterComponent +
                ": " + (component.properties.masterComponent == "[Not Assigned]" ? notAssignedText : component.properties.masterComponent);
        }
    }
    if (typeComponent == "StiGroupHeaderBand") {
        nameText.textContent += "; " + this.loc.PropertyMain.Condition +
            ": " + (component.properties.condition ? StiBase64.decode(component.properties.condition) : "");
    }
}

//------------------------Content-------------------------------
StiMobileDesigner.prototype.RepaintContent = function (component) {
    //Button Element    
    if (component.typeComponent == "StiButtonElement") {
        this.RepaintButtonElementContent(component);
        return;
    }

    //Image
    this.RepaintImageContent(component);

    //Svg Content
    if (component.controls.imageContent.style.display != "" || component.properties.svgContent == "") {
        this.RepaintSvgContent(component, StiBase64.decode(component.properties.svgContent));
    }

    //Onine Map Frame
    if (this.options.currentPage && this.options.currentPage.properties.name == component.properties.pageName) {
        if (component.properties.iframeContent) {
            var iframeContentStr = StiBase64.decode(component.properties.iframeContent);
            if (!component.controls.iframeContent) {
                this.CreateComponentIframeContent(component);
            }
            this.RepaintIframeContent(component, iframeContentStr);
        }
        else {
            if (component.controls.iframeContent) {
                component.controls.iframeContent.parentNode.removeChild(component.controls.iframeContent);
                component.controls.iframeContent = null;
            }
        }
    }
}

StiMobileDesigner.prototype.ApplyColorToElement = function (element, color) {
    if (!color || color == "transparent") {
        element.style.fill = "transparent";
        element.style.fillOpacity = 0;
    }
    else {
        var cArray = color.split(",");
        if (cArray.length == 3) {
            element.style.fill = "rgb(" + cArray[0] + "," + cArray[1] + "," + cArray[2] + ")";
            element.style.opacity = 1;
        }
        else if (cArray.length == 4) {
            element.style.fill = "rgb(" + cArray[1] + "," + cArray[2] + "," + cArray[3] + ")";
            element.style.opacity = this.StrToInt(cArray[0]) / 255;
        }
    }
}

StiMobileDesigner.prototype.ApplyBrushToElement = function (element, parentElement, brushArray) {
    switch (brushArray[0]) {
        case "0":
        case "1":
        case "5": {
            this.ApplyColorToElement(element, brushArray[0] != "0" ? brushArray[1] : "transparent");
            break;
        }
        case "2": {
            var hatchBrush = this.GetSvgHatchBrush(brushArray);
            parentElement.appendChild(hatchBrush);
            hatchBrush.rect.style.visibility = "hidden";
            element.setAttribute("fill", hatchBrush.rect.getAttribute("fill"));
            break;
        }
        case "3":
        case "4": {
            var grad = this.AddGradientBrushToElement(parentElement);
            grad.applyBrush(brushArray);
            grad.rect.style.visibility = "hidden";
            element.setAttribute("fill", grad.rect.getAttribute("fill"));
            break;
        }
    }
}

//------------------------ButtonElement---------------------------------
StiMobileDesigner.prototype.RepaintButtonElementContent = function (component) {
    if (component.properties.buttonVisualStates) {
        var zoom = this.options.report.zoom;
        var compW = parseInt(component.getAttribute("width"));
        var compH = parseInt(component.getAttribute("height"));
        var iconAlignment = component.properties.iconAlignment;
        var horAlignment = component.properties.horAlignment;
        var vertAlignment = component.properties.vertAlignment;
        var iconText = component.properties.buttonType == "Button" ? component.properties.buttonIconSet.iconText : component.properties.buttonIconSet.uncheckedIconText;

        if (component.isOver) {
            var hoverIconText = component.properties.buttonVisualStates.hover.iconSet[component.properties.buttonType == "Button" ? "iconText" : "uncheckedIconText"];
            if (hoverIconText) iconText = hoverIconText;
        }

        var showIcon = iconAlignment != "None" && iconText;
        var showText = !showIcon || iconAlignment != "Center";

        var svgContent = component.controls.svgContent;
        svgContent.clear();
        svgContent.style.visibility = "visible";

        if (component.marginPx) {
            compW -= (component.marginPx[0] + component.marginPx[2]);
            compH -= (component.marginPx[1] + component.marginPx[3]);

            svgContent.setAttribute("x", component.marginPx[0]);
            svgContent.setAttribute("y", component.marginPx[1]);
            svgContent.setAttribute("width", Math.max(0, compW));
            svgContent.setAttribute("height", Math.max(0, compH));
        }

        var icon = this.CreateSvgElement('text');
        icon.textContent = iconText;
        icon.style.fontFamily = "Stimulsoft";
        icon.style.fontSize = (18 * zoom) + "pt";
        icon.style.pointerEvents = "none";

        if (showIcon) {
            svgContent.appendChild(icon);

            var iconBrush = component.isOver ? component.properties.buttonVisualStates.hover.iconBrush : component.properties.iconBrush;
            if (iconBrush == "isStyleBrush")
                iconBrush = "1!" + (component.isOver ? component.properties.styleColors.hoverIconColor : component.properties.styleColors.iconColor);
            else if (iconBrush == "isDefaultBrush")
                iconBrush = component.properties.iconBrush == "isStyleBrush" ? ("1!" + component.properties.styleColors[component.isOver ? "hoverIconColor" : "iconColor"]) : component.properties.iconBrush;

            var iconBrushArray = iconBrush.split("!");
            this.ApplyBrushToElement(icon, svgContent, iconBrushArray);
        }

        var fontStr = component.isOver ? component.properties.buttonVisualStates.hover.font : component.properties.font;
        var font = this.FontStrToObject(fontStr || component.properties.font);
        var fontSize = parseInt(font.size) * zoom;
        var buttonText = StiBase64.decode(component.properties.buttonText);

        var text = this.CreateSvgElement('text');
        text.setAttribute("text-anchor", "start");
        text.textContent = buttonText;
        text.style.pointerEvents = "none";

        text.style.fontSize = fontSize + "pt";
        text.style.fontFamily = font.name;
        text.style.fontWeight = font.bold == "1" ? "bold" : "normal";
        text.style.fontStyle = font.italic == "1" ? "italic" : "normal";
        text.style.textDecoration = "";
        if (font.strikeout == "1") text.style.textDecoration = "line-through";
        if (font.underline == "1") text.style.textDecoration += " underline";

        if (showText) {
            svgContent.appendChild(text);

            var textBrush = component.isOver ? component.properties.buttonVisualStates.hover.textBrush : component.properties.textBrush;
            if (textBrush == "isStyleBrush")
                textBrush = "1!" + (component.isOver ? component.properties.styleColors.hoverTextColor : component.properties.styleColors.textColor);
            else if (textBrush == "isDefaultBrush")
                textBrush = component.properties.textBrush == "isStyleBrush" ? ("1!" + component.properties.styleColors[component.isOver ? "hoverTextColor" : "textColor"]) : component.properties.textBrush;

            var textBrushArray = textBrush.split("!");
            this.ApplyBrushToElement(text, svgContent, textBrushArray);
        }

        var tSize = text.getBBox();
        var iSize = icon.getBBox();

        var iconX = 0;
        var iconY = 0;
        var iconM = 10;
        var iconW = iSize.width + iconM * 2;
        var iconH = iSize.height + iconM * 2;

        var textX = 0;
        var textY = 0;
        var textM = 5;
        var textYOffset = 0;

        var textSize = (showIcon && (iconAlignment == "Left" || iconAlignment == "Right") ? compW - iconW : compW) - textM * 2;

        if (component.properties.wordWrap && textSize > 0 && tSize.width > 0 && tSize.width > textSize) {
            //Measure button text for word wrap mode

            text.textContent = "";

            var blocks = [];
            var blockText = "";
            var spaceIndex = 0;

            for (var i = 0; i < buttonText.length; i++) {
                blockText += buttonText[i];
                if (buttonText[i] == " ") spaceIndex = i;

                text.textContent = blockText;
                var blockSize = text.getBBox();

                if ((blockSize && blockSize.width && blockSize.width > textSize) || i == buttonText.length - 1) {
                    if (spaceIndex > 0) {
                        var delta = i - spaceIndex;
                        i = spaceIndex;
                        text.textContent = blockText.substring(0, blockText.length - delta);
                        blockSize = text.getBBox();
                        spaceIndex = 0;
                    }
                    blocks.push({ text: text.textContent, width: blockSize.width });
                    blockText = "";
                }
            }

            text.textContent = "";
            var minXPos = 0;

            for (var i = 0; i < blocks.length; i++) {
                var tspan = this.CreateSvgElement('tspan');
                tspan.setAttribute("dy", i != 0 ? fontSize * 2 : 0);
                tspan.textContent = blocks[i].text;
                text.appendChild(tspan);

                var xPos = 0;

                if (blocks[i].width > 0) {
                    if (horAlignment == "Center" || horAlignment == "Width") {
                        xPos = textSize / 2 - blocks[i].width / 2;
                    }
                    if (horAlignment == "Right") {
                        xPos = textSize - blocks[i].width;
                    }
                }

                minXPos = (minXPos == 0) ? xPos : Math.min(minXPos, xPos);

                blocks[i].xPos = xPos;
                blocks[i].tspan = tspan;
            }

            for (var i = 0; i < blocks.length; i++) {
                blocks[i].tspan.setAttribute("x", parseInt(blocks[i].xPos - minXPos));
            }

            tSize = text.getBBox();

            var linesCount = blocks.length;
            if (linesCount > 1) {
                var lineHeight = tSize.height / linesCount;
                textYOffset = parseInt(tSize.height - lineHeight);
            }
        }

        if (horAlignment == "Left")
            textX = showIcon && iconAlignment == "Left" ? iconW + textM : textM;

        if (horAlignment == "Center" || horAlignment == "Width") {
            textX = compW / 2 - tSize.width / 2;
            if (showIcon && iconAlignment == "Left") textX = iconW + (compW - iconW) / 2 - tSize.width / 2;
            if (showIcon && iconAlignment == "Right") textX = (compW - iconW) / 2 - tSize.width / 2;
        }

        if (horAlignment == "Right")
            textX = showIcon && iconAlignment == "Right" ? compW - tSize.width - textM - iconW : compW - tSize.width - textM;

        if (vertAlignment == "Top") {
            text.setAttribute("dominant-baseline", "hanging");
            textY = showIcon && iconAlignment == "Top" ? iconH + textM : textM;
        }

        if (vertAlignment == "Center") {
            text.setAttribute("dominant-baseline", "middle");
            textY = compH / 2;
            if (showIcon && iconAlignment == "Top") textY = iconH + (compH - iconH) / 2;
            if (showIcon && iconAlignment == "Bottom") textY = (compH - iconH) / 2;
            textY -= textYOffset / 2;
        }

        if (vertAlignment == "Bottom") {
            textY = showIcon && iconAlignment == "Bottom" ? compH - iconH - textM : compH - textM;
            if (textYOffset > 0) textY -= (textYOffset + textM);
        }

        switch (iconAlignment) {
            case "Left":
                iconX = iconM;
                iconY = compH / 2;
                icon.setAttribute("dominant-baseline", "middle");
                break;

            case "Top":
                iconX = compW / 2 - iSize.width / 2;
                iconY = iconM;
                icon.setAttribute("dominant-baseline", "hanging");
                break;

            case "Right":
                iconX = compW - iSize.width - iconM;
                iconY = compH / 2;
                icon.setAttribute("dominant-baseline", "middle");
                break;

            case "Bottom":
                iconX = compW / 2 - iSize.width / 2;
                iconY = compH - iconM - 5 * zoom;
                break;

            case "Center":
                iconX = compW / 2 - iSize.width / 2;
                iconY = compH / 2;
                icon.setAttribute("dominant-baseline", "middle");
                break;
        }

        if (this.GetNavigatorName() == "MSIE") {
            textY += 3 * zoom;
            iconY += 5 * zoom;
        }

        icon.setAttribute("transform", "translate(" + iconX + "," + (iconY + 2 * zoom) + ")");
        text.setAttribute("transform", "translate(" + textX + "," + (textY + 1 * zoom) + ")");
    }
}

//------------------------TableOfContents-------------------------------
StiMobileDesigner.prototype.RepaintTableOfContents = function (component) {
    var jsObject = this;
    var content = component.controls.tableOfContentsContent;
    var styles = component.properties.tableOfContentsStyles;

    if (content && styles) {
        while (content.childNodes[0]) content.removeChild(content.childNodes[0]);
        var indent = parseInt(component.properties.tableOfContentsIndent);
        var textElem = this.CreateSvgElement("text");
        content.appendChild(textElem);

        content.setAttribute("x", "0");
        content.setAttribute("y", "0");
        content.setAttribute("width", parseInt(component.getAttribute("width")));
        content.setAttribute("height", parseInt(component.getAttribute("height")));

        var addLine = function (index, style, xPos) {
            var text = jsObject.loc.PropertyMain.Heading + index + "   .............................................   " + index;
            var textColor = jsObject.GetColorFromBrushStr(style.properties.textBrush);
            textColor = textColor != "transparent" ? "rgb(" + textColor + ")" : textColor;
            var textFont = jsObject.FontStrToObject(style.properties.font);

            var tspan = jsObject.CreateSvgElement('tspan');
            tspan.textContent = text;
            textElem.appendChild(tspan);

            tspan.style.fill = textColor;
            tspan.style.fontSize = (parseInt(textFont.size) * jsObject.options.report.zoom) + "pt";
            tspan.style.fontFamily = textFont.name;

            tspan.setAttribute("x", xPos);
            tspan.setAttribute("dy", (parseInt(textFont.size) + 10) * jsObject.options.report.zoom);
        }

        for (var i = 0; i < styles.length; i++) {
            var xPos = indent * (i + 1) * jsObject.options.report.zoom;
            addLine(i + 1, styles[i], xPos);
            if (i == styles.length - 1) {
                addLine(i + 2, styles[i], xPos);
                addLine(i + 3, styles[i], xPos);
            }
        }
    }
}

//------------------------SvgContent-------------------------------
StiMobileDesigner.prototype.RepaintInnerContent = function (component, innerObjects) {
    if (!innerObjects || !this.options.in_resize || component.typeComponent == "StiCrossTab" || component.typeComponent == "StiCrossField") return;
    var compStartWidth = this.options.in_resize[2].width || component.startWidth;
    var compStartHeight = this.options.in_resize[2].height || component.startHeight;
    var compRealWidth = this.StrToDouble(component.getAttribute("width"));
    var compRealHeight = this.StrToDouble(component.getAttribute("height"));

    if (component.typeComponent == "StiShape") {
        var svgContent = component.controls.svgContent;
        var viewBox = svgContent.getAttribute("viewBox");
        if (!viewBox) {
            component.shapeSvgContent = svgContent;
            svgContent.setAttribute("viewBox", "0 0 " + compRealWidth + " " + compRealHeight);
            svgContent.setAttribute("preserveAspectRatio", "none");
        }
        return;
    }

    if (innerObjects.image) {
        if (component.typeComponent == "StiZipCode") {
            if (!component.properties.ratio) innerObjects.image.setAttribute("preserveAspectRatio", "none");
            innerObjects.image.setAttribute("width", compRealWidth);
            innerObjects.image.setAttribute("height", compRealHeight);
            return;
        }
    }

    if (innerObjects.rect) {
        innerObjects.rect.setAttribute("width", component.getAttribute("width"));
        innerObjects.rect.setAttribute("height", component.getAttribute("height"));
    }

    var innerObject = innerObjects.image || innerObjects.g || innerObjects.text;
    if (innerObject) {
        var x = this.StrToDouble(innerObject.getAttribute("x") || "0");
        var y = this.StrToDouble(innerObject.getAttribute("y") || "0");

        if (innerObjects.g) {
            //barcode component || text component(angle != 0)
            switch (component.properties.textAngle || component.properties.barCodeAngle) {
                case "90":
                    if (component.properties.vertAlignment == "Bottom") x += (compRealWidth - compStartWidth) / this.options.report.zoom;
                    if (component.properties.vertAlignment == "Center") x += ((compRealWidth - compStartWidth) / 2) / this.options.report.zoom;
                    if (component.properties.horAlignment == "Left") y += (compRealHeight - compStartHeight) / this.options.report.zoom;
                    if (component.properties.horAlignment == "Center") y += ((compRealHeight - compStartHeight) / 2) / this.options.report.zoom;
                    break;
                case "180":
                    if (component.properties.horAlignment == "Left") x += (compRealWidth - compStartWidth) / this.options.report.zoom;
                    if (component.properties.horAlignment == "Center") x += ((compRealWidth - compStartWidth) / 2) / this.options.report.zoom;
                    if (component.properties.vertAlignment == "Top") y += (compRealHeight - compStartHeight) / this.options.report.zoom;
                    if (component.properties.vertAlignment == "Center") y += ((compRealHeight - compStartHeight) / 2) / this.options.report.zoom;
                    break;
                case "270":
                    if (component.properties.vertAlignment == "Top") x += (compRealWidth - compStartWidth) / this.options.report.zoom;
                    if (component.properties.vertAlignment == "Center") x += ((compRealWidth - compStartWidth) / 2) / this.options.report.zoom;
                    if (component.properties.horAlignment == "Right") y += (compRealHeight - compStartHeight) / this.options.report.zoom;
                    if (component.properties.horAlignment == "Center") y += ((compRealHeight - compStartHeight) / 2) / this.options.report.zoom;
                    break;
                default:
                    if (component.typeComponent == "StiBarCode") {
                        if (component.properties.vertAlignment == "Bottom") y += (compRealHeight - compStartHeight) / this.options.report.zoom;
                        if (component.properties.vertAlignment == "Center") y += ((compRealHeight - compStartHeight) / 2) / this.options.report.zoom;
                        if (component.properties.horAlignment == "Right") x += (compRealWidth - compStartWidth) / this.options.report.zoom;
                        if (component.properties.horAlignment == "Center") x += ((compRealWidth - compStartWidth) / 2) / this.options.report.zoom;
                    }
                    else {
                        x += ((compRealWidth - compStartWidth) / 2) / this.options.report.zoom
                        y += ((compRealHeight - compStartHeight) / 2) / this.options.report.zoom;
                    }
            }
            var oldTransform = innerObject.getAttribute("transform");
            var newTransform = "translate(" + x + "," + y + ")";
            if (oldTransform && oldTransform.indexOf("translate") >= 0) {
                var startIndex = oldTransform.indexOf("translate(") + "translate(".length;
                var endIndex = oldTransform.indexOf(")", startIndex);
                newTransform = oldTransform.substring(0, startIndex) + x + "," + y + oldTransform.substring(endIndex);
            }
            innerObject.setAttribute("transform", newTransform);
        }
        else {
            if (component.typeComponent == "StiImageElement" && !component.properties.imageSrc && !component.properties.icon && !component.properties.imageUrl) return;

            switch (component.properties.horAlignment) {
                case "Center": x += ((compRealWidth - compStartWidth) / 2) / this.options.report.zoom; break;
                case "Right": x += (compRealWidth - compStartWidth) / this.options.report.zoom; break;
            }

            switch (component.properties.vertAlignment) {
                case "Center": y += ((compRealHeight - compStartHeight) / 2) / this.options.report.zoom; break;
                case "Bottom": y += (compRealHeight - compStartHeight) / this.options.report.zoom; break;
            }

            innerObject.setAttribute("x", x);
            innerObject.setAttribute("y", y);
        }
    }
}

StiMobileDesigner.prototype.RepaintIframeContent = function (component, iframeContentStr) {
    var ifr = component.controls.iframeContent;

    if (component.cornerRadius) {
        ifr.style.borderRadius = component.cornerRadius[0] + "px " + component.cornerRadius[1] + "px " + component.cornerRadius[2] + "px " + component.cornerRadius[3] + "px";
    }

    if (ifr && ifr.iframeContent != iframeContentStr) {
        ifr.iframeContent = iframeContentStr;
        try {
            ifr.contentWindow.document.head.innerHTML = "";
            ifr.contentWindow.document.body.innerHTML = "";
        } catch (e) { }
        ifr.contentWindow.document.write(iframeContentStr);
    }

    var marginC = component.properties.margin ? component.properties.margin.split(";") : [0, 0, 0, 0];
    var paddingC = component.properties.padding ? component.properties.padding.split(";") : [0, 0, 0, 0];
    var marginP = component.properties.pageName ? this.options.report.pages[component.properties.pageName].marginsPx : [0, 0, 0, 0];
    var paddingP = this.options.paintPanelPadding + 2;
    var z = this.options.report.zoom;
    var headerHeight = component.properties.titleVisible ? 30 : 0;

    var left = this.StrToDouble(marginC[0]) + this.StrToDouble(paddingC[0]);
    var top = this.StrToDouble(marginC[1]) + this.StrToDouble(paddingC[1]);
    var width = this.ConvertUnitToPixel(this.StrToDouble(component.properties.unitWidth), component.isDashboardElement) - left - this.StrToDouble(marginC[2]) - this.StrToDouble(paddingC[2]) - 2;
    var height = this.ConvertUnitToPixel(this.StrToDouble(component.properties.unitHeight), component.isDashboardElement) - top - this.StrToDouble(marginC[3]) - this.StrToDouble(paddingC[3]) - headerHeight - 2;

    ifr.style.left = (paddingP + marginP[0] + (this.ConvertUnitToPixel(this.StrToDouble(component.properties.unitLeft), component.isDashboardElement) + left + width / 2) * z - width / 2) + "px";
    ifr.style.top = (paddingP + marginP[1] + (this.ConvertUnitToPixel(this.StrToDouble(component.properties.unitTop), component.isDashboardElement) + headerHeight + top + height / 2) * z - height / 2) + "px";
    ifr.style.width = width + "px";
    ifr.style.height = height + "px";
    var cs = "scale(" + z + ")";
    ifr.style.transform = cs;
    ifr.style.msTransform = cs;
    ifr.style.mozTransform = cs;
    ifr.style.webkitTransform = cs;

    if (!ifr.timerStarted && ifr.contentWindow.document.startViewTimer) {
        ifr.timerStarted = true;
        ifr.contentWindow.document.startViewTimer();
    }
}

StiMobileDesigner.prototype.RepaintSvgContent = function (component, svgContentStr) {
    var jsObject = this;
    var compWidth = parseInt(component.getAttribute("width"));
    var compHeight = parseInt(component.getAttribute("height"));
    var zoom = this.options.report.zoom;
    var svgContent = component.controls.svgContent;
    var typeComponent = component.typeComponent;
    var innerRects = [];

    if (svgContentStr == "") {
        while (svgContent.firstChild != null) svgContent.removeChild(svgContent.firstChild);
        svgContent.visibility = "hidden";
    }
    else {
        svgContent.style.visibility = "visible";
        var temp = document.createElement("div");

        temp.innerHTML = svgContentStr.indexOf("<svg") >= 0 ? svgContentStr.substring(svgContentStr.indexOf("<svg")) : "<svg>" + svgContentStr + "</svg>";

        while (svgContent.firstChild != null) svgContent.removeChild(svgContent.firstChild);
        var innerObjects = {};

        var writeElementXY = function (element) {
            innerObjects.g = element;
            var xyPosStr = element.getAttribute("transform");
            xyPosStr = xyPosStr.substring(xyPosStr.indexOf("translate(") + "translate(".length);
            xyPosStr = xyPosStr.substring(0, xyPosStr.indexOf(")"));
            var xyPos = xyPosStr.split(" ");
            if (xyPos.length == 1) xyPos = xyPosStr.split(",");
            innerObjects.g.setAttribute("x", xyPos[0]);
            innerObjects.g.setAttribute("y", xyPos.length > 1 ? xyPos[1] : 0);
        }

        Array.prototype.slice.call(temp.childNodes[0].childNodes).forEach(function (el) {
            svgContent.appendChild(el);
            if (el.tagName != null) {
                if (el.tagName == "svg") innerObjects.svg = el;
                if (el.tagName == "image") innerObjects.image = el;
                if (el.tagName == "text") innerObjects.text = el;
                if (el.tagName == "rect") {
                    if (!innerObjects.rect) innerObjects.rect = el;
                    innerRects.push(el);
                }
                if (el.tagName == "foreignObject") innerObjects.foreignObject = el;
                if (el.tagName == "g") {
                    if (typeComponent == "StiMap" || typeComponent == "StiRegionMapElement" || typeComponent == "StiOnlineMapElement" ||
                        typeComponent == "StiChart" || typeComponent == "StiChartElement" ||
                        typeComponent == "StiGauge" || typeComponent == "StiGaugeElement" ||
                        typeComponent == "StiBarCode" || typeComponent == "StiCheckBox" ||
                        typeComponent == "StiTableCellCheckBox" || typeComponent == "StiTableElement" || typeComponent == "StiPivotTableElement" ||
                        typeComponent == "StiProgressElement" || typeComponent == "StiIndicatorElement" || typeComponent == "StiCardsElement" ||
                        typeComponent == "StiListBoxElement" || typeComponent == "StiComboBoxElement" || typeComponent == "StiButtonElement" ||
                        typeComponent == "StiTreeViewElement" || typeComponent == "StiTreeViewBoxElement" || typeComponent == "StiDatePickerElement") {

                        if (el.getAttribute("transform") && el.getAttribute("transform").indexOf("translate") >= 0) {
                            writeElementXY(el);
                        }
                    }
                    else {
                        while (el.firstChild != null) {
                            if (el.firstChild.tagName == "g" && el.firstChild.getAttribute("transform") &&
                                el.firstChild.getAttribute("transform").indexOf("translate") >= 0) {
                                writeElementXY(el.firstChild);
                                break;
                            }
                            el = el.firstChild;
                        }
                    }
                }

                if (zoom != 1 && el.getAttribute != null) {
                    var oldTransform = el.getAttribute("transform");
                    var newTransform = "scale(" + zoom + ")";
                    if (oldTransform) newTransform += (" " + oldTransform);
                    el.setAttribute("transform", newTransform);
                }
            }
        });

        if (innerObjects.rect) {
            var fillAttr = innerObjects.rect.getAttribute("fill");
            if (fillAttr && fillAttr.indexOf("hatch") >= 0) {
                innerObjects.rect.setAttribute("transform", "scale(1)");
                innerObjects.rect.setAttribute("width", compWidth);
                innerObjects.rect.setAttribute("height", compHeight);
            }
            if (innerObjects.rect.getAttribute("fill-opacity") && component.controls.background.style.opacity && component.controls.background.style.opacity != "1")
                innerObjects.rect.setAttribute("fill-opacity", "0");
        }

        if (innerObjects.foreignObject) {
            //for allowHtmlTags == true
            var foreignObject = innerObjects.foreignObject;
            foreignObject.setAttribute("x", "0");
            foreignObject.setAttribute("y", "0");
            foreignObject.setAttribute("width", compWidth);
            foreignObject.setAttribute("height", compHeight);
            foreignObject.removeAttribute("transform");

            var innerContent = foreignObject.innerHTML;
            var div = document.createElement("div");
            div.style.display = "table-cell";
            div.style.textAlign = component.properties.horAlignment.toLowerCase();
            var vertAlignment = component.properties.vertAlignment.toLowerCase();
            if (vertAlignment == "center") vertAlignment = "middle";
            div.style.verticalAlign = vertAlignment;
            div.style.width = compWidth + "px";
            div.style.height = compHeight + "px";
            div.innerHTML = innerContent;
            foreignObject.innerHTML = "";
            foreignObject.appendChild(div);
        }

        this.RepaintInnerContent(component, innerObjects);
    }

    if (component.isDashboardElement && component.marginPx) {
        svgContent.setAttribute("x", component.marginPx[0]);
        svgContent.setAttribute("y", component.marginPx[1]);
        var newWidth = compWidth - (component.marginPx[0] + component.marginPx[2]);
        var newHeight = compHeight - (component.marginPx[1] + component.marginPx[3]);
        svgContent.setAttribute("width", newWidth >= 0 ? newWidth : 0);
        svgContent.setAttribute("height", newHeight >= 0 ? newHeight : 0);
    }
    else {
        svgContent.setAttribute("width", compWidth >= 0 ? compWidth : 0);
        svgContent.setAttribute("height", compHeight >= 0 ? compHeight : 0);
        svgContent.setAttribute("x", 0);
        svgContent.setAttribute("y", 0);
    }

    if (component.isDashboardElement && component.cornersIsRounded && innerRects.length > 0) {
        innerRects.forEach(function (r) {
            if (r.getAttribute("x") == "0" && r.getAttribute("y") == "0") {
                if (r.getAttribute("height") == "30" && component.cornerRadius && component.properties.titleVisible && component.properties.titleText) {
                    var zoom = jsObject.options.report.zoom;
                    var titleR = Math.max(parseInt(component.cornerRadius[0]), parseInt(component.cornerRadius[1]));
                    r.setAttribute("rx", titleR / zoom);
                    r.setAttribute("ry", titleR / zoom);
                    r.setAttribute("x", "1");
                    r.setAttribute("y", "1");
                    r.setAttribute("width", parseInt(r.getAttribute("width") - 1));

                    var newRect = jsObject.CreateSvgElement("rect");
                    if (r.parentNode) {
                        r.parentNode.appendChild(newRect);
                        newRect.setAttribute("x", "1");
                        newRect.setAttribute("y", Math.round(22 * zoom));
                        newRect.setAttribute("width", Math.round(parseInt(r.getAttribute("width")) * zoom));
                        newRect.setAttribute("height", Math.round(10 * zoom));
                        newRect.setAttribute("fill", r.getAttribute("fill"));
                        newRect.setAttribute("fill-opacity", r.getAttribute("fill-opacity"));
                    }
                }
                else {
                    r.setAttribute("fill", "transparent");
                    r.style.fill = "transparent";
                }
            }
        });
    }

    if (typeComponent == "StiShape" || typeComponent == "StiShapeElement") {
        svgContent.style.overflow = "visible";
    }
}

//------------------------ImageContent-------------------------------
StiMobileDesigner.prototype.RepaintImageContent = function (component) {
    var imageContent = component.controls.imageContent;

    if ((component.typeComponent == "StiImage" || component.typeComponent == "StiTableCellImage") &&
        (component.properties.imageSrc || component.properties.imageContentForPaint)) {
        //Only StiImage
        try {
            var imageSrc = component.properties.imageSrc || component.properties.imageContentForPaint;
            var isWmfImage = imageSrc.indexOf("data:image/x-wmf") >= 0;
            var isSvgImage = imageSrc.indexOf("data:image/svg+xml") >= 0;

            if (isWmfImage && component.properties.imageContentForPaint) {
                imageSrc = component.properties.imageContentForPaint; //Wmf image type
            }
            var zoom = this.options.report.zoom;
            var image = new window.Image();
            if (imageSrc) image.src = imageSrc;

            imageContent.style.display = "";
            var parentImageContent = component.controls.parentImageContent
            var componentWidth = parseInt(component.realWidth);
            var componentHeight = parseInt(component.realHeight);
            var multipleFactor = component.properties.imageMultipleFactor != null ? this.StrToDouble(component.properties.imageMultipleFactor) : 1;
            var stretch = component.properties.stretch || isWmfImage;
            var aspectRatio = component.properties.ratio && !isWmfImage;

            image.onload = function () {
                //#region Helpers
                function StiPoint(x, y) {
                    this.x = x;
                    this.y = y;
                }
                var identityMatrix = [
                    [1, 0, 0],
                    [0, 1, 0],
                    [0, 0, 1]
                ];
                //Rotation transformation matrix is calculated as follows:
                //| cos(A) -sin(A) 0 |
                //| sin(A)  cos(A) 0 |
                //|   0       0    1 |
                //This is "left-handed" coordinate system, as usual in computer graphics
                //(when Y axis points up, X points to the left). So rotation directions are treated:
                // - clockwise as positive, so (A) degrees clockwise is just (A) degrees;
                // - counter-clockwise as negative, (A) degrees counter-clockwise is (-A) degrees.
                var turn90DegreesClockwiseMatrix = [
                    [0, -1, 0],
                    [1, 0, 0],
                    [0, 0, 1]
                ];
                var turn90DegreesCounterClockwiseMatrix = [
                    [0, 1, 0],
                    [-1, 0, 0],
                    [0, 0, 1]
                ];
                var turn180DegreesMatrix = [
                    [-1, 0, 0],
                    [0, -1, 0],
                    [0, 0, 1]
                ];
                var horizontalFlipMatrix = [
                    [-1, 0, 0],
                    [0, 1, 0],
                    [0, 0, 1]
                ];
                var verticalFlipMatrix = [
                    [1, 0, 0],
                    [0, -1, 0],
                    [0, 0, 1]
                ];

                function translationTransform(deltaX, deltaY) {
                    return [
                        [1, 0, deltaX],
                        [0, 1, deltaY],
                        [0, 0, 1]
                    ];
                }

                function scaleTransform(factorX, factorY, anchorX, anchorY) {
                    factorY = factorY || factorX;
                    anchorX = anchorX || 0;
                    anchorY = anchorY || 0;
                    return [
                        [factorX, 0, anchorX * (1 - factorX)],
                        [0, factorY, anchorY * (1 - factorY)],
                        [0, 0, 1]
                    ];
                }

                function multiply3x3Matrices(left, right) {
                    var result = [
                        [],
                        [],
                        []
                    ];

                    for (var i = 0; i < 3; ++i)
                        for (var j = 0; j < 3; ++j)
                            result[i][j] = left[i][0] * right[0][j] + left[i][1] * right[1][j] + left[i][2] * right[2][j];

                    return result;
                }

                function getImageTransformation(width, height, scaleFactor, rotation, horizontalAlignment, verticalAlignment, stretch, preserveAspectRatio, containerWidth, containerHeight) {
                    //We will use a list of consecutive transformation matrices.
                    var transformations = [];

                    //First, we scale image: we need final sizes to proceed.
                    transformations.push(scaleTransform(scaleFactor));

                    width *= scaleFactor;
                    height *= scaleFactor;

                    //Next, we apply rotation/flip.
                    //We also find axis-aligned bounding box (AABB) of rotated image.
                    //Here image is rectangle and is rotated only by +90/-90/180 degrees,
                    //so AABB is, actually, the image itself, just, well, rotated.
                    var rotationMatrix;
                    var boundingBoxLow; //Top-left point.
                    var boundingBoxHigh; //Bottom-right point.
                    switch (rotation) {
                        case "Rotate90CW":
                            rotationMatrix = turn90DegreesClockwiseMatrix;
                            boundingBoxLow = new StiPoint(-height, 0);
                            boundingBoxHigh = new StiPoint(0, width);
                            break;
                        case "Rotate90CCW":
                            rotationMatrix = turn90DegreesCounterClockwiseMatrix;
                            boundingBoxLow = new StiPoint(0, -width);
                            boundingBoxHigh = new StiPoint(height, 0);
                            break;
                        case "Rotate180":
                            rotationMatrix = turn180DegreesMatrix;
                            boundingBoxLow = new StiPoint(-width, -height);
                            boundingBoxHigh = new StiPoint(0, 0);
                            break;
                        case "FlipHorizontal":
                            rotationMatrix = horizontalFlipMatrix;
                            boundingBoxLow = new StiPoint(-width, 0);
                            boundingBoxHigh = new StiPoint(0, height);
                            break;
                        case "FlipVertical":
                            rotationMatrix = verticalFlipMatrix;
                            boundingBoxLow = new StiPoint(0, -height);
                            boundingBoxHigh = new StiPoint(width, 0);
                            break;
                        //"None" also goes here.
                        default:
                            rotationMatrix = identityMatrix;
                            boundingBoxLow = new StiPoint(0, 0);
                            boundingBoxHigh = new StiPoint(width, height);
                            break;
                    }

                    transformations.push(rotationMatrix);

                    //Next, we align image. It's done by applying translation.
                    //Alignment is applied inside container. We align our AABB inside it.
                    //Let's consider the example: we need to left-align our image.
                    //To do that we set left-most X coordinate of image to left-most
                    //X coordinate of container. Let's call these coordinates "anchors".
                    //So, to left-align we must move anchor of image to anchor of container.
                    //Concrete anchor is determined by alignment, and translation distance is
                    //distance between current anchor positions.
                    var boundingBoxAnchorX;
                    var containerAnchorX;
                    switch (horizontalAlignment) {
                        case "Left":
                            boundingBoxAnchorX = boundingBoxLow.x;
                            containerAnchorX = 0;
                            break;
                        case "Center":
                            boundingBoxAnchorX = (boundingBoxLow.x + boundingBoxHigh.x) / 2; //Middle between bounds.
                            containerAnchorX = containerWidth / 2;
                            break;
                        case "Right":
                            boundingBoxAnchorX = boundingBoxHigh.x;
                            containerAnchorX = containerWidth;
                            break;
                    }

                    var boundingBoxAnchorY;
                    var containerAnchorY;
                    switch (verticalAlignment) {
                        case "Top":
                            boundingBoxAnchorY = boundingBoxLow.y;
                            containerAnchorY = 0;
                            break;
                        case "Center":
                            boundingBoxAnchorY = (boundingBoxLow.y + boundingBoxHigh.y) / 2; //Middle between bounds.
                            containerAnchorY = containerHeight / 2;
                            break;
                        case "Bottom":
                            boundingBoxAnchorY = boundingBoxHigh.y;
                            containerAnchorY = containerHeight;
                            break;
                    }

                    var alignmentTranslationMatrix = translationTransform(containerAnchorX - boundingBoxAnchorX, containerAnchorY - boundingBoxAnchorY);
                    transformations.push(alignmentTranslationMatrix);

                    //Next, stretching.
                    //Stretching is, essentially, scaling image to be of container's size.
                    if (stretch) {
                        var stretchingFactorX = containerWidth / (boundingBoxHigh.x - boundingBoxLow.x);
                        var stretchingFactorY = containerHeight / (boundingBoxHigh.y - boundingBoxLow.y);

                        //If aspect ratio is preserved, we scale sides by same factor.
                        //If scaling enlarges image, we enlarge it until first bound is met,
                        //i. e. as less as possible. If scaling shrinks image, it's shrunk until
                        //all bounds are met, it's factor with max absolute value, but since we're
                        //shrinking, factors are negative, so we again need minimum factor.
                        if (preserveAspectRatio) {
                            stretchingFactorX = Math.min(stretchingFactorX, stretchingFactorY);
                            stretchingFactorY = stretchingFactorX;
                        }

                        //Scaling anchor point (one that remains at the same place after scaling)
                        //is alignment anchor point, it's already calculated.
                        var stretchingMatrix = scaleTransform(stretchingFactorX, stretchingFactorY, containerAnchorX, containerAnchorY);
                        transformations.push(stretchingMatrix);
                    }

                    //Matrices in array are stored in application order, for ex. [C, B, A].
                    //They are applied as follows: A * (B * (C * vec)), matrix multiplication is associative, so:
                    // A * (B * (C * vec)) ==  (A * B * C) * vec ==  (A * (B * C)) * vec.
                    //Therefore, reduceStep: (C, B) => B * C.
                    return transformations.reduce(function (previousMatrix, matrix) { return multiply3x3Matrices(matrix, previousMatrix); });
                }

                function svgStringOf3x3Matrix(matrix) {
                    return "matrix(" +
                        matrix[0][0] + ", " +
                        matrix[1][0] + ", " +
                        matrix[0][1] + ", " +
                        matrix[1][1] + ", " +
                        matrix[0][2] + ", " +
                        matrix[1][2] + ")";
                }
                //#endregion Helpers

                //Margins order: [Left, Top, Right, Bottom].
                var margins = component.properties.imageMargins
                    ? component.properties.imageMargins.split(";").map(function (piece) { return parseInt(piece) * zoom; })
                    : [0, 0, 0, 0];

                var containerWidth = componentWidth - margins[0] - margins[2];
                var containerHeight = componentHeight - margins[1] - margins[3];

                var scaleFactor = multipleFactor * zoom;

                var imageTransform = getImageTransformation(
                    image.width, image.height,
                    scaleFactor,
                    component.properties.rotation,
                    component.properties.horAlignment, component.properties.vertAlignment,
                    stretch, aspectRatio,
                    containerWidth, containerHeight);

                //Everything's ready, now put data into svg.
                //Image data.
                imageContent.href.baseVal = imageSrc;
                //Calculated transform.
                imageContent.setAttribute("transform", svgStringOf3x3Matrix(imageTransform).toString());
                //Workaround for IE11. It turns out IE requires these specified explicitly, otherwise image is not displayed.
                imageContent.setAttribute("x", "0");
                imageContent.setAttribute("y", "0");
                imageContent.setAttribute("width", image.width);
                imageContent.setAttribute("height", image.height);

                //Parent container, ensures clipping when image exceeds margins.
                parentImageContent.setAttribute("x", margins[0]);
                parentImageContent.setAttribute("y", margins[1]);
                parentImageContent.setAttribute("width", containerWidth < 0 ? 0 : containerWidth);
                parentImageContent.setAttribute("height", containerHeight < 0 ? 0 : containerHeight);

                image = null;
            }
        }
        catch (e) { }
    }
    else {
        imageContent.style.display = "none";
    }
}

//---------------------BackGround--------------------------------
StiMobileDesigner.prototype.RepaintShadow = function (component) {
    var borderProps = (component.properties["border"]) ? component.properties.border.split("!") : null;

    if (borderProps && borderProps.length > 4) {
        var cShadow = component.controls.shadow;
        var showShadow = borderProps != null && borderProps[4] == "1";
        cShadow.style.display = showShadow ? "" : "none";

        if (!showShadow) return;

        var shadowSize = this.StrToInt(borderProps[5]);
        cShadow.setAttribute("width", parseInt(component.getAttribute("width")) + shadowSize);
        cShadow.setAttribute("height", parseInt(component.getAttribute("height")) + shadowSize);
        cShadow.setAttribute("x", shadowSize);
        cShadow.setAttribute("y", shadowSize);

        var shadowColor = this.GetColorFromBrushStr(StiBase64.decode(borderProps[6]));
        cShadow.style.fill = this.GetHTMLColor(shadowColor);
    }

    component.style.filter = "";
    var shadowVisible = component.properties.shadowVisible;

    if (shadowVisible) {
        var shadowColor = this.GetHTMLColor(component.properties.shadowColor);
        var shadowLocation = component.properties.shadowLocation.split(";");
        var shadowSize = component.properties.shadowSize;
        component.style.filter = "drop-shadow(" + shadowLocation[0] + "px " + shadowLocation[1] + "px " + shadowSize + "px " + shadowColor + ")";
    }
}

//-------------------CrossTab Fields------------------------------
StiMobileDesigner.prototype.RepaintCrossTabFields = function (component, selectedComponentName) {
    while (component.controls.crossTabContainer.childNodes[0]) component.controls.crossTabContainer.removeChild(component.controls.crossTabContainer.childNodes[0]);
    component.controls.crossTabContainer.setAttribute("width", component.getAttribute("width"));
    component.controls.crossTabContainer.setAttribute("height", component.getAttribute("height"));

    if (component.properties.crossTabFields) {
        var compObjects = component.properties.crossTabFields.components;

        for (var i = 0; i < compObjects.length; i++) {
            var newComponent = this.CreateCrossTabFieldComponent(compObjects[i]);
            newComponent.repaint();
            component.controls.crossTabContainer.appendChild(newComponent);
            newComponent.parentContainer = component.controls.crossTabContainer;

            if (selectedComponentName != null && newComponent.properties.name == selectedComponentName) {
                newComponent.action();
            }
        }
    }
}
