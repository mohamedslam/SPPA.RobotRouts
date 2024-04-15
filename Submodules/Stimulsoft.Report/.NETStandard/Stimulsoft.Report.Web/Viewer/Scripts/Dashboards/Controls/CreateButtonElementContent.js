
StiJsViewer.prototype.CreateButtonElementContent = function (element) {
    var elementAttrs = element.elementAttributes;
    var contentAttrs = elementAttrs.contentAttributes;

    element.controls = {};
    element.style.cursor = "pointer";

    if (contentAttrs.buttonShapeType == "Circle") {
        var size = Math.min(elementAttrs.width, elementAttrs.height);
        element.style.borderRadius = size + "px";

        var top = parseInt(element.style.top);
        var left = parseInt(element.style.left);

        var deltaTop = (elementAttrs.height - size) / 2;
        var deltaLeft = (elementAttrs.width - size) / 2;

        elementAttrs.width = elementAttrs.height = size;
        element.style.width = element.style.height = size + "px";

        element.style.top = parseInt(top + deltaTop) + "px";
        element.style.left = parseInt(left + deltaLeft) + "px";
    }

    var mainSvg = this.CreateSvgElement("svg");
    mainSvg.setAttribute("width", elementAttrs.width);
    mainSvg.setAttribute("height", elementAttrs.height);
    element.mainSvg = mainSvg;

    var contentPanel = element.contentPanel;
    contentPanel.appendChild(mainSvg);
    contentPanel.style.overflow = "visible";
    contentPanel.style.left = contentPanel.style.top = contentPanel.style.right = contentPanel.style.bottom = "0px";

    this.CreateButtonElementBackGround(element);
    this.CreateButtonElementGradientBrush(element);
    this.CreateButtonElementHatchBrush(element);
    this.CreateButtonElementSvgContent(element);
    this.CreateButtonElementActionEvents(element);

    this.RepaintButtonElementBackGround(element);
    this.RepaintButtonElementContent(element);
}

StiJsViewer.prototype.CreateButtonElementActionEvents = function (element) {
    var jsObject = this;
    var elementAttrs = element.elementAttributes;
    var contentAttrs = elementAttrs.contentAttributes;

    element.isChecked = contentAttrs.checked;
    element.isOver = false;
    element.isPressed = false;

    element.getCurrentBrush = function (brushProperty, styleColorProperty) {
        var brush = contentAttrs[brushProperty] == "isStyleBrush" ? ("1;" + contentAttrs.styleColors[styleColorProperty]) : contentAttrs[brushProperty];

        if (this.isPressed) {
            brush = contentAttrs.buttonVisualStates.pressed[brushProperty] == "isStyleBrush"
                ? ("1;" + contentAttrs.styleColors["selected" + jsObject.upperFirstChar(styleColorProperty)])
                : (contentAttrs.buttonVisualStates.pressed[brushProperty] == "isDefaultBrush"
                    ? (contentAttrs[brushProperty] == "isStyleBrush" ? ("1;" + contentAttrs.styleColors["selected" + jsObject.upperFirstChar(styleColorProperty)]) : contentAttrs[brushProperty])
                    : contentAttrs.buttonVisualStates.pressed[brushProperty]);
        }
        else if (this.isOver) {
            brush = contentAttrs.buttonVisualStates.hover[brushProperty] == "isStyleBrush"
                ? ("1;" + contentAttrs.styleColors["hover" + jsObject.upperFirstChar(styleColorProperty)])
                : (contentAttrs.buttonVisualStates.hover[brushProperty] == "isDefaultBrush"
                    ? (contentAttrs[brushProperty] == "isStyleBrush" ? ("1;" + contentAttrs.styleColors["hover" + jsObject.upperFirstChar(styleColorProperty)]) : contentAttrs[brushProperty])
                    : contentAttrs.buttonVisualStates.hover[brushProperty]);
        }
        else if (this.isChecked) {
            brush = contentAttrs.buttonVisualStates.check[brushProperty] == "isStyleBrush"
                ? ("1;" + contentAttrs.styleColors["selected" + jsObject.upperFirstChar(styleColorProperty)])
                : (contentAttrs.buttonVisualStates.check[brushProperty] == "isDefaultBrush"
                    ? (contentAttrs[brushProperty] == "isStyleBrush" ? ("1;" + contentAttrs.styleColors["selected" + jsObject.upperFirstChar(styleColorProperty)]) : contentAttrs[brushProperty])
                    : contentAttrs.buttonVisualStates.check[brushProperty]);
        }

        return brush;
    }

    element.getCurrentFont = function () {
        var font;

        if (this.isPressed)
            font = contentAttrs.buttonVisualStates.pressed.font;
        else if (this.isOver)
            font = contentAttrs.buttonVisualStates.hover.font;
        else if (this.isChecked)
            font = contentAttrs.buttonVisualStates.check.font;

        if (!font) font = contentAttrs.font;

        return font;
    }

    element.getCurrentIconText = function () {
        var iconText;

        if (contentAttrs.buttonType == "Button") {
            iconText = contentAttrs.buttonIconSet.icon;
        }
        else {
            iconText = this.isChecked
                ? (contentAttrs.buttonVisualStates.check.iconSet.checkedIcon || contentAttrs.buttonIconSet.checkedIcon)
                : (contentAttrs.buttonVisualStates.check.iconSet.uncheckedIcon || contentAttrs.buttonIconSet.uncheckedIcon);
        }

        if (this.isPressed) {
            var pressedIconText = contentAttrs.buttonVisualStates.pressed.iconSet[contentAttrs.buttonType == "Button" ? "icon" : (this.isChecked ? "checkedIcon" : "uncheckedIcon")];
            if (pressedIconText) iconText = pressedIconText;
        }
        else if (this.isOver) {
            var hoverIconText = contentAttrs.buttonVisualStates.hover.iconSet[contentAttrs.buttonType == "Button" ? "icon" : (this.isChecked ? "checkedIcon" : "uncheckedIcon")];
            if (hoverIconText) iconText = hoverIconText;
        }

        return iconText;
    }

    element.repaint = function () {
        jsObject.RepaintButtonElementBackGround(this);
        jsObject.RepaintButtonElementContent(this);
    }

    jsObject.SubscribeToButtonElementActionEvents(element);
}

StiJsViewer.prototype.SubscribeToButtonElementActionEvents = function (element) {
    var jsObject = this;

    if (element.subscribedOnEvents) return;

    this.addEvent(element, 'mouseover', function (e) {
        element.isOver = true;
        element.repaint();
    });

    this.addEvent(element, 'mouseout', function (e) {
        element.isOver = false;
        element.isPressed = false;
        element.repaint();
    });

    this.addEvent(element, 'mousedown', function (e) {
        element.isPressed = true;
        element.repaint();
    });

    this.addEvent(element, 'mouseup', function (e) {
        element.isPressed = false;
        element.repaint();
    });

    this.addEvent(element, 'click', function (e) {
        if (element.elementAttributes.contentAttributes.buttonType != "Button") {
            element.isChecked = !element.isChecked;
        }
        jsObject.postInteraction({
            action: "DashboardButtonElementApplyEvents",
            dashboardFilteringParameters: {
                elementName: element.elementAttributes.name,
                isChecked: element.isChecked
            }
        });
    });

    element.subscribedOnEvents = true;
}

StiJsViewer.prototype.RepaintButtonElementBackGround = function (element) {
    var elementAttrs = element.elementAttributes;
    var contentAttrs = elementAttrs.contentAttributes;

    //remove old complex brushes
    element.controls.gradient.rect.style.display = "none";
    element.controls.svgHatchBrush.clear();
    element.controls.gradient.rect.style.pointerEvents = "none";
    element.controls.svgHatchBrush.style.pointerEvents = "none";

    //simple brush
    var backRect = element.controls.backRect;
    backRect.style.pointerEvents = "none";

    var isSimpleBrush = true;
    var backColor;
    var brush = contentAttrs.brush;

    //button action colors
    var brush = element.getCurrentBrush("brush", "backColor");
    var brushArray = brush.split(";");

    if (brushArray[0] == "0" || brushArray[0] == "1" || brushArray[0] == "5") {
        backColor = brushArray[0] == "0" ? "transparent" : brushArray[1];
    }
    else {
        isSimpleBrush = false;
        backColor = "transparent";
    }

    backRect.style.fill = backColor;

    //complex brushes
    if (!isSimpleBrush) {
        this.RepaintButtonElementComplexBackground(element, brush);
    }
}

StiJsViewer.prototype.RepaintButtonElementComplexBackground = function (element, brush) {
    if (brush) {
        var brushArray = brush.split(";");

        switch (brushArray[0]) {
            case "2": {
                var hatchBrush = this.GetSvgHatchBrush(brushArray);
                element.controls.svgHatchBrush.appendChild(hatchBrush);
                break;
            }
            case "3":
            case "4": {
                var grad = element.controls.gradient;
                grad.applyBrush(brushArray);
                break;
            }
        }
    }
}

StiJsViewer.prototype.RepaintButtonElementContent = function (element) {
    var elementAttrs = element.elementAttributes;
    var contentAttrs = elementAttrs.contentAttributes;

    var compW = elementAttrs.width;
    var compH = elementAttrs.height;
    var iconAlignment = contentAttrs.iconAlignment;
    var horAlignment = contentAttrs.horAlignment;
    var vertAlignment = contentAttrs.vertAlignment;
    var wordWrap = contentAttrs.wordWrap;
    var iconText = element.getCurrentIconText();
    var showIcon = iconAlignment != "None" && iconText;
    var showText = !showIcon || iconAlignment != "Center";

    var svgContent = element.controls.svgContent;
    svgContent.setAttribute("width", compW);
    svgContent.setAttribute("height", compH);
    svgContent.clear();

    var icon = this.CreateSvgElement('text');
    icon.textContent = iconText;
    icon.style.fontFamily = "Stimulsoft";
    icon.style.fontSize = 18 + "pt";
    icon.style.pointerEvents = "none";

    if (showIcon) {
        svgContent.appendChild(icon);
        var iconBrush = element.getCurrentBrush("iconBrush", "iconColor");
        this.ApplyBrushToElement(icon, svgContent, iconBrush.split(";"));
    }

    var font = element.getCurrentFont();
    var fontSize = parseInt(font.size);
    var buttonText = StiBase64.decode(contentAttrs.buttonText);

    var text = this.CreateSvgElement('text');
    text.setAttribute("text-anchor", "start");
    text.textContent = buttonText
    text.style.pointerEvents = "none";

    text.style.fontSize = fontSize + "pt";
    text.style.fontFamily = font.name;
    text.style.fontWeight = font.bold ? "bold" : "normal";
    text.style.fontStyle = font.italic ? "italic" : "normal";
    text.style.textDecoration = "";
    if (font.strikeout) text.style.textDecoration = "line-through";
    if (font.underline) text.style.textDecoration += " underline";

    if (showText) {
        svgContent.appendChild(text);
        var textBrush = element.getCurrentBrush("textBrush", "textColor");
        this.ApplyBrushToElement(text, svgContent, textBrush.split(";"));
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

    if (wordWrap && textSize > 0 && tSize.width > 0 && tSize.width > textSize) {
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
            iconY = compH - iconM - 5;
            break;

        case "Center":
            iconX = compW / 2 - iSize.width / 2;
            iconY = compH / 2;
            icon.setAttribute("dominant-baseline", "middle");
            break;
    }

    if (this.getNavigatorName() == "MSIE") {
        textY += 3;
        iconY += 5;
    }

    icon.setAttribute("transform", "translate(" + iconX + "," + (iconY + 2) + ")");
    text.setAttribute("transform", "translate(" + textX + "," + (textY + 1) + ")");
}

StiJsViewer.prototype.ApplyBrushToElement = function (element, parentElement, brushArray) {
    switch (brushArray[0]) {
        case "0":
        case "1":
        case "5": {
            element.setAttribute("fill", brushArray[0] != "0" ? brushArray[1] : "transparent");
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

StiJsViewer.prototype.CreateButtonElementBackGround = function (element) {
    var backRect = this.CreateSvgElement("rect");
    backRect.style.stroke = "transparent";
    backRect.setAttribute("width", "100%");
    backRect.setAttribute("height", "100%");
    element.mainSvg.appendChild(backRect);
    element.controls.backRect = backRect;
}

StiJsViewer.prototype.CreateButtonElementGradientBrush = function (element) {
    element.controls.gradient = this.AddGradientBrushToElement(element.mainSvg);
}

StiJsViewer.prototype.AddGradientBrushToElement = function (element) {
    var gradId = this.generateKey();
    var grad = this.CreateSvgElement("linearGradient");
    element.appendChild(grad);
    grad.setAttribute("id", gradId);
    grad.setAttribute("x1", "0%");
    grad.setAttribute("y1", "0%");
    grad.setAttribute("x2", "100%");
    grad.setAttribute("y2", "0%");
    grad.stop1 = this.CreateSvgElement("stop");
    grad.stop1.setAttribute("offset", "0");
    grad.appendChild(grad.stop1);
    grad.stop2 = this.CreateSvgElement("stop");
    grad.stop2.setAttribute("offset", "50%");
    grad.appendChild(grad.stop2);
    grad.stop3 = this.CreateSvgElement("stop");
    grad.stop3.setAttribute("offset", "100%");
    grad.appendChild(grad.stop3);
    grad.rect = this.CreateSvgElement("rect");
    grad.rect.setAttribute("width", "100%");
    grad.rect.setAttribute("height", "100%");
    grad.rect.setAttribute("fill", "url(#" + gradId + ")");
    grad.rect.style.display = "none";
    element.appendChild(grad.rect);

    grad.applyBrush = function (brushArray) {
        if (brushArray && brushArray.length >= 3) {
            grad.stop1.setAttribute("stop-color", brushArray[1]);

            if (brushArray[0] == "3") {
                if (grad.stop2.parentNode) {
                    grad.stop2.parentNode.removeChild(grad.stop2);
                }
                grad.stop3.setAttribute("stop-color", brushArray[2]);
            }
            else {
                grad.stop2.setAttribute("stop-color", brushArray[2]);
                grad.insertBefore(grad.stop2, grad.stop3);
                grad.stop3.setAttribute("stop-color", brushArray[1]);
            }

            var angle = parseInt(brushArray[3]) - 180;
            var pi = angle * (Math.PI / 180);
            var x1 = parseInt(50 + Math.cos(pi) * 50);
            var y1 = parseInt(50 + Math.sin(pi) * 50);
            var x2 = parseInt(50 + Math.cos(pi + Math.PI) * 50);
            var y2 = parseInt(50 + Math.sin(pi + Math.PI) * 50);

            grad.setAttribute("x1", x1 + "%");
            grad.setAttribute("y1", y1 + "%");
            grad.setAttribute("x2", x2 + "%");
            grad.setAttribute("y2", y2 + "%");
            grad.rect.style.display = "";
        }
    }

    return grad;
}

StiJsViewer.prototype.CreateButtonElementHatchBrush = function (element) {
    var svgHatchBrush = this.CreateSvgElement("svg");
    element.controls.svgHatchBrush = svgHatchBrush;
    element.mainSvg.appendChild(svgHatchBrush);

    svgHatchBrush.clear = function () {
        while (this.childNodes[0]) {
            this.removeChild(this.childNodes[0]);
        }
    }
}

StiJsViewer.prototype.CreateButtonElementSvgContent = function (element) {
    var svgContent = this.CreateSvgElement("svg");
    element.controls.svgContent = svgContent;
    element.mainSvg.appendChild(svgContent);

    svgContent.clear = function () {
        while (this.childNodes[0]) {
            this.removeChild(this.childNodes[0]);
        }
    }
}

StiJsViewer.prototype.GetSvgHatchBrush = function (brushProps) {
    var brushSvg = this.CreateSvgElement("svg");
    var brushId = this.generateKey();
    var foreColor = brushProps[1];
    var backColor = brushProps[2];
    var hatchNumber = parseInt(brushProps[3]);
    if (hatchNumber > 53) hatchNumber = 53;

    this.AddHatchBrushPatternToElement(brushSvg, brushId, hatchNumber, foreColor, backColor);

    var rect = this.CreateSvgElement("rect");
    brushSvg.rect = rect;
    rect.setAttribute("width", "100%");
    rect.setAttribute("height", "100%");
    rect.setAttribute("fill", "url(#" + brushId + ")");
    brushSvg.appendChild(rect);

    return brushSvg;
}

StiJsViewer.prototype.AddHatchBrushPatternToElement = function (element, patternId, hatchNumber, foreColor, backColor) {
    var brushPattern = this.CreateSvgElement("pattern");
    element.appendChild(brushPattern);

    brushPattern.setAttribute("id", patternId);
    brushPattern.setAttribute("x", "0");
    brushPattern.setAttribute("y", "0");
    brushPattern.setAttribute("width", "8");
    brushPattern.setAttribute("height", "8");
    brushPattern.setAttribute("patternUnits", "userSpaceOnUse");

    var sb = "";
    var hatchHex = this.GetHatchBrushData[hatchNumber];

    for (var index = 0; index < 16; index++) {
        sb += this.HexToByteString(hatchHex.charAt(index));
    }

    var brushRect = this.CreateSvgElement("rect");
    brushPattern.appendChild(brushRect);
    brushRect.setAttribute("x", "0");
    brushRect.setAttribute("y", "0");
    brushRect.setAttribute("width", "8");
    brushRect.setAttribute("height", "8");
    brushRect.setAttribute("fill", backColor);


    for (var indexRow = 0; indexRow < 8; indexRow++) {
        for (var indexColumn = 0; indexColumn < 8; indexColumn++) {

            var indexChar = sb.charAt(indexRow * 8 + indexColumn);

            if (indexChar == "1") {
                var brushRect2 = this.CreateSvgElement("rect");
                brushPattern.appendChild(brushRect2);
                brushRect2.setAttribute("x", indexColumn);
                brushRect2.setAttribute("y", indexRow.toString());
                brushRect2.setAttribute("width", "1");
                brushRect2.setAttribute("height", "1");
                brushRect2.setAttribute("fill", foreColor);

            }
        }
    }
}

StiJsViewer.prototype.GetHatchBrushData = [
    "000000FF00000000",	//HatchStyleHorizontal = 0
    "1010101010101010",	//HatchStyleVertical = 1,			
    "8040201008040201",	//HatchStyleForwardDiagonal = 2,	
    "0102040810204080",	//HatchStyleBackwardDiagonal = 3,	
    "101010FF10101010",	//HatchStyleCross = 4,			
    "8142241818244281",	//HatchStyleDiagonalCross = 5,	
    "8000000008000000",	//HatchStyle05Percent = 6,		
    "0010000100100001",	//HatchStyle10Percent = 7,		
    "2200880022008800",	//HatchStyle20Percent = 8,		
    "2288228822882288",	//HatchStyle25Percent = 9,		
    "2255885522558855",	//HatchStyle30Percent = 10,		
    "AA558A55AA55A855",	//HatchStyle40Percent = 11,		
    "AA55AA55AA55AA55",	//HatchStyle50Percent = 12,		
    "BB55EE55BB55EE55",	//HatchStyle60Percent = 13,		
    "DD77DD77DD77DD77",	//HatchStyle70Percent = 14,		
    "FFDDFF77FFDDFF77",	//HatchStyle75Percent = 15,		
    "FF7FFFF7FF7FFFF7",	//HatchStyle80Percent = 16,		
    "FF7FFFFFFFF7FFFF",	//HatchStyle90Percent = 17,		
    "8844221188442211",	//HatchStyleLightDownwardDiagonal = 18,	
    "1122448811224488",	//HatchStyleLightUpwardDiagonal = 19,	
    "CC663399CC663399",	//HatchStyleDarkDownwardDiagonal = 20,	
    "993366CC993366CC",	//HatchStyleDarkUpwardDiagonal = 21,	
    "E070381C0E0783C1",	//HatchStyleWideDownwardDiagonal = 22,	
    "C183070E1C3870E0",	//HatchStyleWideUpwardDiagonal = 23,	
    "4040404040404040",	//HatchStyleLightVertical = 24,			
    "00FF000000FF0000",	//HatchStyleLightHorizontal = 25,		
    "AAAAAAAAAAAAAAAA",	//HatchStyleNarrowVertical = 26,		
    "FF00FF00FF00FF00",	//HatchStyleNarrowHorizontal = 27,		
    "CCCCCCCCCCCCCCCC",	//HatchStyleDarkVertical = 28,			
    "FFFF0000FFFF0000",	//HatchStyleDarkHorizontal = 29,		
    "8844221100000000",	//HatchStyleDashedDownwardDiagonal = 30,
    "1122448800000000",	//HatchStyleDashedUpwardDiagonal = 311,	
    "F00000000F000000",	//HatchStyleDashedHorizontal = 32,		
    "8080808008080808",	//HatchStyleDashedVertical = 33,		
    "0240088004200110",	//HatchStyleSmallConfetti = 34,			
    "0C8DB130031BD8C0",	//HatchStyleLargeConfetti = 35,		
    "8403304884033048",	//HatchStyleZigZag = 36,			
    "00304A8100304A81",	//HatchStyleWave = 37,				
    "0102040818244281",	//HatchStyleDiagonalBrick = 38,		
    "202020FF020202FF",	//HatchStyleHorizontalBrick = 39,	
    "1422518854224588",	//HatchStyleWeave = 40,				
    "F0F0F0F0AA55AA55",	//HatchStylePlaid = 41,				
    "0100201020000102",	//HatchStyleDivot = 42,				
    "AA00800080008000",	//HatchStyleDottedGrid = 43,		
    "0020008800020088",	//HatchStyleDottedDiamond = 44,		
    "8448300C02010103",	//HatchStyleShingle = 45,			
    "33FFCCFF33FFCCFF",	//HatchStyleTrellis = 46,			
    "98F8F877898F8F77",	//HatchStyleSphere = 47,			
    "111111FF111111FF",	//HatchStyleSmallGrid = 48,			
    "3333CCCC3333CCCC",	//HatchStyleSmallCheckerBoard = 49,	
    "0F0F0F0FF0F0F0F0",	//HatchStyleLargeCheckerBoard = 50,	
    "0502058850205088",	//HatchStyleOutlinedDiamond = 51,	
    "10387CFE7C381000",	//HatchStyleSolidDiamond = 52,
    "0000000000000000"	//HatchStyleTotal = 53
];

StiJsViewer.prototype.HexToByteString = function (hex) {
    var result = "0000";
    switch (hex) {
        case "1":
            result = "0001";
            break;

        case "2":
            result = "0010";
            break;

        case "3":
            result = "0011";
            break;

        case "4":
            result = "0100";
            break;

        case "5":
            result = "0101";
            break;

        case "6":
            result = "0110";
            break;

        case "7":
            result = "0111";
            break;

        case "8":
            result = "1000";
            break;

        case "9":
            result = "1001";
            break;

        case "A":
            result = "1010";
            break;

        case "B":
            result = "1011";
            break;

        case "C":
            result = "1100";
            break;

        case "D":
            result = "1101";
            break;

        case "E":
            result = "1110";
            break;

        case "F":
            result = "1111";
            break;
    }

    return result;
}