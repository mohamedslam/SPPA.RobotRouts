
// Str To Double
StiMobileDesigner.prototype.StrToDouble = function (value) {
    if (value == null)
        return null;

    var result = parseFloat(value.toString().replace(",", "."));
    if (result)
        return result;
    else
        return 0;
}

// Str To Int
StiMobileDesigner.prototype.StrToInt = function (value) {
    var result = parseInt(value);
    if (result)
        return result;
    else
        return 0;
}

// Str To Correct Byte
StiMobileDesigner.prototype.StrToCorrectPositiveInt = function (value) {
    var result = this.StrToInt(value);
    if (result >= 0)
        return result;
    else
        return 0;
}

// Str To Correct Byte
StiMobileDesigner.prototype.StrToCorrectByte = function (value) {
    var result = parseInt(value);
    if (result) {
        if (result > 255) return 255;
        if (result < 0) return 0;
        return result;
    }
    else
        return 0;
}

StiMobileDesigner.prototype.FontStrToObject = function (fontStr) {
    var fontArray = fontStr.split("!");
    var font = {
        "name": fontArray[0],
        "size": fontArray[1],
        "bold": fontArray[2],
        "italic": fontArray[3],
        "underline": fontArray[4],
        "strikeout": fontArray[5]
    }
    return font;
}

StiMobileDesigner.prototype.FontObjectToStr = function (font) {
    return font.name + "!" + font.size + "!" + font.bold + "!" + font.italic + "!" + font.underline + "!" + font.strikeout;
}

StiMobileDesigner.prototype.BordersStrToObject = function (borderStr) {
    var borderArray = borderStr.split("!");
    var borderSides = borderArray[0].split(",");
    var border = {
        left: borderSides[0],
        top: borderSides[1],
        right: borderSides[2],
        bottom: borderSides[3],
        size: borderArray[1],
        color: borderArray[2],
        style: borderArray[3],
        dropShadow: borderArray[4],
        sizeShadow: borderArray[5],
        brushShadow: borderArray[6],
        topmost: borderArray[7]
    }
    if (borderArray.length > 8) {
        var advBorders = borderArray[8].split(";");
        border.advancedBorder = {
            leftSize: advBorders[0],
            leftColor: advBorders[1],
            leftStyle: advBorders[2],
            topSize: advBorders[3],
            topColor: advBorders[4],
            topStyle: advBorders[5],
            rightSize: advBorders[6],
            rightColor: advBorders[7],
            rightStyle: advBorders[8],
            bottomSize: advBorders[9],
            bottomColor: advBorders[10],
            bottomStyle: advBorders[11]
        }
    }

    return border;
}

StiMobileDesigner.prototype.BordersObjectToStr = function (border) {
    var strBorder = border.left + "," + border.top + "," + border.right + "," + border.bottom + "!" + border.size + "!" + border.color + "!" + border.style;
    if (border.dropShadow != null && border.sizeShadow != null && border.brushShadow != null && border.topmost != null) {
        strBorder += ("!" + border.dropShadow + "!" + border.sizeShadow + "!" + border.brushShadow + "!" + border.topmost);
    }
    if (border.advancedBorder) {
        var advBorder = border.advancedBorder;
        strBorder += ("!" + advBorder.leftSize + ";" + advBorder.leftColor + ";" + advBorder.leftStyle + ";" + advBorder.topSize + ";" + advBorder.topColor + ";" + advBorder.topStyle + ";" +
            advBorder.rightSize + ";" + advBorder.rightColor + ";" + advBorder.rightStyle + ";" + advBorder.bottomSize + ";" + advBorder.bottomColor + ";" + advBorder.bottomStyle);
    }

    return strBorder;
}

StiMobileDesigner.prototype.BorderObjectToShotStr = function (border, notLocalize) {
    var borderStr = "";
    if (border.left == "1") borderStr += (notLocalize ? "Left" : this.loc.PropertyEnum.StiBorderSidesLeft);
    if (border.top == "1") { if (borderStr != "") borderStr += ","; borderStr += (notLocalize ? "Top" : this.loc.PropertyEnum.StiBorderSidesTop); }
    if (border.right == "1") { if (borderStr != "") borderStr += ","; borderStr += (notLocalize ? "Right" : this.loc.PropertyEnum.StiBorderSidesRight); }
    if (border.bottom == "1") { if (borderStr != "") borderStr += ","; borderStr += (notLocalize ? "Bottom" : this.loc.PropertyEnum.StiBorderSidesBottom); }
    if (borderStr == "") return (notLocalize ? "None" : this.loc.PropertyEnum.StiCheckStyleNone);
    if (border.left == "1" && border.top == "1" && border.right == "1" && border.bottom == "1") return (notLocalize ? "All" : this.loc.PropertyEnum.StiBorderSidesAll);

    return borderStr;
}

StiMobileDesigner.prototype.ConvertUnitToPixel = function (unitValue, isDashboard) {
    var unit = (this.options.report != null) ? this.options.report.properties.reportUnit : "cm";

    if (isDashboard) unit = "hi";

    switch (unit) {
        case "cm":
            {
                return unitValue * 100 / 2.54;
            }

        case "mm":
            {
                return unitValue * 10 / 2.54;
            }

        case "hi":
            {
                return unitValue;
            }

        case "in":
            {
                return unitValue * 100;
            }
    }
}

StiMobileDesigner.prototype.ConvertPixelToUnit = function (pixelsValue, isDashboard) {
    var unit = (this.options.report != null) ? this.options.report.properties.reportUnit : "cm";

    if (isDashboard) unit = "hi";

    switch (unit) {
        case "cm":
            {
                return pixelsValue * 2.54 / 100;
            }

        case "mm":
            {
                return pixelsValue * 2.54 / 10;
            }

        case "hi":
            {
                return pixelsValue;
            }

        case "in":
            {
                return pixelsValue / 100;
            }
    }
}

StiMobileDesigner.prototype.GetRoundedLineCoordinates = function (coords) {
    return [this.RoundXY(coords[0], 0)[0], this.RoundXY(coords[1], 0)[0], this.RoundXY(coords[2], 0)[0], this.RoundXY(coords[3], 0)[0]];
}

StiMobileDesigner.prototype.GetRoundedPaintRect = function (rectComponent) {
    var restX = 0;
    var restY = 0;
    var isXRound = false;
    var isYRound = false;

    var xyLeft = this.RoundXY(rectComponent[0], restX, isXRound);
    var xyTop = this.RoundXY(rectComponent[1], restY, isYRound);

    var leftComp = xyLeft[0];
    var topComp = xyTop[0];
    var widthComp = this.RoundWH(rectComponent[2], xyLeft[1], xyLeft[2]);
    var heightComp = this.RoundWH(rectComponent[3], xyTop[1], xyTop[2]);

    return [leftComp, topComp, widthComp, heightComp];
}

StiMobileDesigner.prototype.RoundXY = function (value, rest, isRound) {
    var v = parseInt(value);
    rest = value - v;
    isRound = false;

    var result = 0;

    if (rest > 0.501953) {
        result = v + 1;
        rest -= 0.501953;
        isRound = true;
    }
    else {
        result = v;
        isRound = false;
    }

    return [result, rest, isRound];
}

StiMobileDesigner.prototype.RoundWH = function (value, rest, isRound) {
    var roundValue = (isRound) ? 1.0 : 0.501953;

    var v = parseInt(value);
    var rest1 = value - v;
    rest1 += rest;
    var result = (rest1 >= roundValue) ? (v + 1) : v;

    return result;
}

StiMobileDesigner.prototype.ConvertAllComponentsToCurrentUnit = function (pages, components) {
    for (var indexPage = 0; indexPage < pages.length; indexPage++) {
        var pagesAttr = pages[indexPage];
        var page = this.options.report.pages[pagesAttr.name];
        var size = pagesAttr.size.split("!");
        if (page) {
            page.properties.unitWidth = size[0];
            page.properties.unitHeight = size[1];
            page.properties.unitMargins = pagesAttr.margins;
            page.properties.columnWidth = pagesAttr.columnWidth;
            page.properties.columnGaps = pagesAttr.columnGaps;
            page.repaint(true);
        }
    }

    for (var indexComp = 0; indexComp < components.length; indexComp++) {
        var compAttr = components[indexComp];
        var component = this.options.report.pages[compAttr.pageName].components[compAttr.name];
        var rect = compAttr.componentRect.split("!");
        var clientPos = compAttr.compPos.split("!");
        if (component) {
            if (compAttr.columnWidth != null) component.properties.columnWidth = compAttr.columnWidth;
            if (compAttr.columnGaps != null) component.properties.columnGaps = compAttr.columnGaps;
            component.properties.clientLeft = clientPos[0];
            component.properties.clientTop = clientPos[1];
            component.properties.unitLeft = rect[0];
            component.properties.unitTop = rect[1];
            component.properties.unitWidth = rect[2];
            component.properties.unitHeight = rect[3];
            if (compAttr.crossTabFields) component.properties.crossTabFields = compAttr.crossTabFields;
            component.repaint();
        }
    }
}

// Date To String American Format
StiMobileDesigner.prototype.DateToStringAmericanFormat = function (date, shortFormat) {
    if (date == null) date = new Date();
    var day = date.getDate();
    var month = date.getMonth() + 1;
    var year = date.getFullYear();
    var halfDay = "AM";
    var hours = date.getHours();
    if (hours == 0) hours = 12
    else if (hours >= 12) {
        if (hours != 12) hours -= 12;
        halfDay = "PM";
    }
    var minutes = date.getMinutes();
    if (minutes.toString().length == 1) minutes = "0" + minutes;
    var seconds = date.getSeconds();
    if (seconds.toString().length == 1) seconds = "0" + seconds;

    var result = month + "/" + day + "/" + year;
    if (!shortFormat) result += (" " + hours + ":" + minutes + ":" + seconds + " " + halfDay);

    return result;
}

StiMobileDesigner.prototype.DateAmericanFormatToLocalFormat = function (dateStr, dateTimeFormat) {
    var date = new Date(dateStr);

    if (dateTimeFormat == null || dateTimeFormat == "DateAndTime") return date.toLocaleString();
    if (dateTimeFormat == "Date") return date.toLocaleDateString();
    if (dateTimeFormat == "Time") return date.toLocaleTimeString();
}


StiMobileDesigner.prototype.RoundByGridSize = function (value) {
    var unit = (this.options.report != null) ? this.options.report.properties.reportUnit : "cm";
    var gridSize;

    switch (unit) {
        case "cm": gridSize = this.StrToDouble(this.options.report.info.gridSizeCentimetres); break;
        case "mm": gridSize = this.StrToDouble(this.options.report.info.gridSizeMillimeters); break;
        case "hi": gridSize = this.StrToDouble(this.options.report.info.gridSizeHundredthsOfInch); break;
        case "in": gridSize = this.StrToDouble(this.options.report.info.gridSizeInch); break;
    }

    return (gridSize > 0 ? Math.round(value / gridSize) * gridSize : value).toFixed(2);
}

StiMobileDesigner.prototype.RoundPlus = function (x, n) {
    if (isNaN(x) || isNaN(n)) return false;
    var m = Math.pow(10, n);

    return Math.round(x * m) / m;
}