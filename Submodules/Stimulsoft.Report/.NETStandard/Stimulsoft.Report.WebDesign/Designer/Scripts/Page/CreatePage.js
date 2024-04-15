
StiMobileDesigner.prototype.CreatePage = function (pageObject, isDashboard) {
    var page = this.CreateSvgElement("svg");
    page.jsObject = this;
    page.typeComponent = "StiPage";
    page.style.margin = this.options.paintPanelPadding + "px";
    page.style.display = "none";
    page.style.border = "1px solid " + (!isDashboard ? "#c6c6c6" : "transparent");
    page.style.overflow = "hidden";
    page.isDashboard = isDashboard;
    page.valid = pageObject.valid;

    if (this.options.cloudMode) {
        var permissions = this.GetCloudPermissions();
        page.valid = (page.isDashboard && permissions.dAvailable) || (!page.isDashboard && permissions.rAvailable);
    }

    //Set Properties
    page.properties = {};
    page.properties.name = pageObject.name;
    page.properties.pageIndex = pageObject.pageIndex;
    page.properties.largeHeightAutoFactor = "1";
    this.WriteAllProperties(page, pageObject.properties);

    //Create Controls
    page.controls = {};
    this.CreatePageBackgroundGradient(page);
    this.CreatePageBackgroundHatch(page);
    this.CreatePageWaterMarkGradient(page);
    this.CreatePageWaterMarkImage(page);
    this.CreatePageWaterMark(page);
    this.CreatePageBorders(page);

    if (this.options.isTouchDevice)
        this.CreateComponentButtonsPanel(page);

    if (this.options.report && this.options.report.info.showGrid)
        this.CreatePageGridLines(page);

    if (!page.valid)
        this.CreatePageWaterMarkBack(page);

    //Create Methods
    this.CreatePageEvents(page);

    var jsObject = this;
    page.repaint = function (rebuildGrigLines) { jsObject.RepaintPage(this, rebuildGrigLines); }
    page.remove = function () { jsObject.RemovePage(this); }
    page.rebuild = function (componentsProps) { jsObject.RebuildPage(this, componentsProps); }
    page.setSelected = function () { jsObject.SetSelectedObject(this); }
    page.rename = function (newName) { jsObject.RenamePage(this, newName); }
    page.addComponents = function () { jsObject.AddComponents(this); }
    page.removeComponents = function () { jsObject.RemoveComponents(this); }
    page.updateComponentsLevels = function () { jsObject.UpdateComponentsLevels(this); }
    page.updateWatermarkLevels = function () { jsObject.UpdateWatermarkLevels(this); }
    page.repaintAllComponents = function () { jsObject.RepaintAllComponentsOnPage(this); }

    return page;
}

//GridLines
StiMobileDesigner.prototype.CreatePageGridLines = function (page) {
    var jsObject = this;
    var gridSize = page.isDashboard ? page.properties.gridSize : this.options.report.gridSize;
    var pageWidth = this.ConvertUnitToPixel(this.StrToDouble(page.properties.unitWidth), page.isDashboard);
    var pageHeight = this.ConvertUnitToPixel(this.StrToDouble(page.properties.unitHeight), page.isDashboard);
    var marginsStr = page.properties.unitMargins.split("!");
    var verticalMarginsPx = parseInt(this.ConvertUnitToPixel(this.StrToDouble(marginsStr[1]) + this.StrToDouble(marginsStr[3]), page.isDashboard));
    var horizontalMarginsPx = parseInt(this.ConvertUnitToPixel(this.StrToDouble(marginsStr[0]) + this.StrToDouble(marginsStr[2]), page.isDashboard));

    if (page.isDashboard) {
        pageWidth += horizontalMarginsPx;
        pageHeight += verticalMarginsPx;
    }

    var segmentPerHeight = this.StrToDouble(page.properties.segmentPerHeight);
    var segmentPerWidth = this.StrToDouble(page.properties.segmentPerWidth);
    if (segmentPerWidth > 1) pageWidth = ((pageWidth - horizontalMarginsPx) * segmentPerWidth) + horizontalMarginsPx;
    if (segmentPerHeight > 1) pageHeight = ((pageHeight - verticalMarginsPx) * segmentPerHeight) + verticalMarginsPx;
    var largeHeightFactor = page.properties.largeHeight ? this.StrToInt(page.properties.largeHeightFactor) : this.StrToDouble(page.properties.largeHeightAutoFactor);

    if (page.controls.gridLines) {
        for (var i = 0; i < page.controls.gridLines.length; i++)
            page.removeChild(page.controls.gridLines[i]);
    }

    var margins = page.properties.unitMargins.split("!");
    var marginsPx = [];
    for (var i = 0; i < 4; i++) {
        marginsPx[i] = this.ConvertUnitToPixel(this.StrToDouble(margins[i]), page.isDashboard);
    }

    var startXPos = marginsPx[0];
    var endXPos = pageWidth - marginsPx[2];
    var startYPos = marginsPx[1];
    var endYPos = largeHeightFactor > 1 ? (pageHeight - marginsPx[3] - marginsPx[1]) * largeHeightFactor + marginsPx[1] : pageHeight - marginsPx[3];
    var gridColor = page.isDashboard ? this.GetHTMLColor(page.properties[jsObject.options.report.info.gridMode == "Lines" ? "gridLinesColor" : "gridDotsColor"]) : "#dcdcdc";

    var createGridLine = function (x1, y1, x2, y2) {
        var line = jsObject.CreateSvgElement("line");
        line.style.stroke = gridColor;
        line.positions = { x1: x1, y1: y1, x2: x2, y2: y2 };
        page.controls.gridLines.push(line);
        page.insertBefore(line, page.controls.borders[0]);

        line.repaint = function () {
            var XOffset = jsObject.options.xOffset;
            var YOffset = jsObject.options.yOffset;
            line.setAttribute("x1", line.positions.x1 * jsObject.options.report.zoom + XOffset);
            line.setAttribute("y1", line.positions.y1 * jsObject.options.report.zoom + YOffset);
            line.setAttribute("x2", line.positions.x2 * jsObject.options.report.zoom + XOffset);
            line.setAttribute("y2", line.positions.y2 * jsObject.options.report.zoom + YOffset);
            line.style.strokeDasharray = jsObject.options.report.info.gridMode == "Lines" ? "1" : ("1," + (gridSize * jsObject.options.report.zoom - 1));
            if (page.isDashboard) line.style.stroke = gridColor;
        }

        return line;
    }

    page.controls.gridLines = [];

    var x = startXPos;
    var i = 0;
    while (x <= endXPos) {
        if (!(page.isDashboard && (x == startXPos || x + gridSize > endXPos || i % 2 != 0))) {
            var line = createGridLine(x, startYPos, x, endYPos);
            if (i % 2 == 0) line.style.stroke = page.isDashboard ? gridColor : "#bebebe";
        }
        x += gridSize;
        i++;
    }

    if (jsObject.options.report.info.gridMode == "Lines") {
        var y = startYPos;
        i = 0;
        while (y <= endYPos) {
            if (!(page.isDashboard && (y == startYPos || y + gridSize > endYPos || i % 2 != 0))) {
                var line = createGridLine(startXPos, y, endXPos, y);
                if (i % 2 == 0) line.style.stroke = page.isDashboard ? gridColor : "#bebebe";
            }
            y += gridSize;
            i++;
        }
    }
}

// Borders
StiMobileDesigner.prototype.CreatePageBorders = function (page) {
    page.controls.borders = [];
    for (var i = 0; i < 10; i++) {
        //9 & 10 - shadow lines, must to first create
        page.controls.borders[9 - i] = this.CreateSvgElement("line");
        page.appendChild(page.controls.borders[9 - i]);
    }
}

// Watermark
StiMobileDesigner.prototype.CreatePageWaterMark = function (page) {
    page.controls.waterMarkParent = this.CreateSvgElement("g");
    page.appendChild(page.controls.waterMarkParent);
    page.controls.waterMarkChild = this.CreateSvgElement("g");
    page.controls.waterMarkParent.appendChild(page.controls.waterMarkChild);
    page.controls.waterMarkText = this.CreateSvgElement("text");
    page.controls.waterMarkChild.appendChild(page.controls.waterMarkText);
}

// Watermark Chars
StiMobileDesigner.prototype.CreatePageWaterMarkBack = function (page) {
    page.controls.waterMarkBackParent = this.CreateSvgElement("g");
    page.appendChild(page.controls.waterMarkBackParent);
    page.controls.waterMarkBackChild = this.CreateSvgElement("g");
    page.controls.waterMarkBackParent.appendChild(page.controls.waterMarkBackChild);
    page.controls.waterMarkBackText = this.CreateSvgElement("text");
    page.controls.waterMarkBackChild.appendChild(page.controls.waterMarkBackText);
    this.options.infoPanel.checkState();
}

// Watermark Image
StiMobileDesigner.prototype.CreatePageWaterMarkImage = function (page) {
    page.controls.waterMarkImage = this.CreateSvgElement("image");
    page.appendChild(page.controls.waterMarkImage);
}

// Watermark Levels
StiMobileDesigner.prototype.UpdateWatermarkLevels = function (page) {
    if (page && !page.isDashboard) {
        if (page.properties.waterMarkText && page.properties.waterMarkEnabled && !page.properties.waterMarkTextBehind) {
            var txtParent = page.controls.waterMarkParent;
            if (txtParent && txtParent.parentNode) {
                txtParent.parentNode.removeChild(txtParent);
                page.appendChild(txtParent);
            }
        }
        if ((page.properties.watermarkImageSrc || page.properties.watermarkImageContentForPaint) && !page.properties.waterMarkImageBehind) {
            var imgParent = page.controls.waterMarkImage;
            if (imgParent && imgParent.parentNode) {
                imgParent.parentNode.removeChild(imgParent);
                page.appendChild(imgParent);
            }
        }
    }
}

// Background Gradient
StiMobileDesigner.prototype.CreatePageBackgroundGradient = function (page) {
    page.controls.gradient = this.AddGradientBrushToElement(page);
}

// WaterMark Gradient
StiMobileDesigner.prototype.CreatePageWaterMarkGradient = function (page) {
    var grad = this.CreateSvgElement("linearGradient");
    page.controls.waterMarkGradient = grad;
    page.appendChild(grad);
    grad.setAttribute("id", "waterMarkGradient" + this.generateKey());
    grad.setAttribute("x1", "0%");
    grad.setAttribute("y1", "0%");
    grad.setAttribute("x2", "100%");
    grad.setAttribute("y2", "0%");
    grad.stop1 = this.CreateSvgElement("stop");
    grad.appendChild(grad.stop1);
    grad.stop2 = this.CreateSvgElement("stop");
    grad.appendChild(grad.stop2);
    grad.stop3 = this.CreateSvgElement("stop");
    grad.appendChild(grad.stop3);
    grad.stop1.setAttribute("offset", "0");
    grad.stop2.setAttribute("offset", "50%");
    grad.stop3.setAttribute("offset", "100%");
}

// Background Hatch
StiMobileDesigner.prototype.CreatePageBackgroundHatch = function (page) {
    var svgHatchBrush = this.CreateSvgElement("svg");
    page.controls.svgHatchBrush = svgHatchBrush;
    page.appendChild(svgHatchBrush);

    svgHatchBrush.clear = function () {
        while (this.childNodes[0]) {
            this.removeChild(this.childNodes[0]);
        }
    }
}

// Component Buttons Panel 
StiMobileDesigner.prototype.CreateComponentButtonsPanel = function (page) {
    if (page.controls.componentButtonsPanel) {
        page.removeChild(page.controls.componentButtonsPanel);
    }

    var jsObject = this;
    var panel = this.CreateSvgElement("svg");
    panel.style.display = "none";
    page.controls.componentButtonsPanel = panel;
    page.appendChild(panel);
    panel.buttons = {};
    panel.separators = [];
    panel.arrowsHeight = 10;
    panel.buttonsHeight = 30;
    panel.realHeight = panel.buttonsHeight + panel.arrowsHeight;

    var buttonProps = [
        ["cut", this.loc.MainMenu.menuEditCut.replace("&", "")],
        ["copy", this.loc.MainMenu.menuEditCopy.replace("&", "")],
        ["paste", this.loc.MainMenu.menuEditPaste.replace("&", "")],
        ["remove", this.loc.MainMenu.menuEditDelete.replace("&", "")],
        ["edit", this.loc.MainMenu.menuEditEdit]
    ]

    for (var i = 0; i < buttonProps.length; i++) {
        var button = this.TextSvgButton(buttonProps[i][1], 70, panel.buttonsHeight);
        button.key = buttonProps[i][0];
        button.setAttribute("x", i * 60);
        panel.appendChild(button);
        panel.buttons[buttonProps[i][0]] = button;
        button.action = function () {
            panel.action(this.key)
        }
    }

    var arrow = jsObject.CreateSvgElement("polygon");
    panel.arrow = arrow;
    arrow.setAttribute("class", "stiDesignerSvgButtonArrow");
    arrow.style.display = "none";
    panel.appendChild(arrow);

    panel.action = function (key) {
        var comp = this.component;
        switch (key) {
            case "cut":
                jsObject.ExecuteAction("cutComponent");
                break;

            case "copy":
                jsObject.ExecuteAction("copyComponent");
                break;

            case "paste":
                jsObject.ExecuteAction("pasteComponent");
                break;

            case "remove":
                jsObject.ExecuteAction("removeComponent");
                break;

            case "edit": {
                jsObject.ShowComponentForm(comp);
                break;
            }
        }
    }

    panel.getShowingButtons = function () {
        var buttons = [];
        var comp = this.component;
        var options = jsObject.options;
        var restrictions = comp.properties.restrictions;
        var canRemove = (restrictions && (restrictions == "All" || restrictions.indexOf("AllowDelete") >= 0)) || !restrictions;
        var canChange = (restrictions && (restrictions == "All" || restrictions.indexOf("AllowChange") >= 0)) || !restrictions;
        var canShowEditForm = ComponentCollection[comp.typeComponent][7];
        var isNotReportOrPage = comp.typeComponent != "StiPage" && comp.typeComponent != "StiReport";

        if (isNotReportOrPage && canRemove && !jsObject.IsTableCell(options.selectedObjects || comp)) buttons.push("cut");
        if (isNotReportOrPage && !jsObject.IsTableCell(options.selectedObjects || comp)) buttons.push("copy");
        if (comp.typeComponent != "StiReport" && options.clipboard) buttons.push("paste");
        if (isNotReportOrPage && canRemove) buttons.push("remove");
        if (canChange && canShowEditForm) buttons.push("edit");

        return buttons;
    }

    panel.show = function (component) {
        panel.component = component;
        panel.style.display = "";
        page.removeChild(panel);
        page.appendChild(panel);

        var separator = function (x) {
            var sep = jsObject.CreateSvgElement("line");
            sep.setAttribute("x1", x);
            sep.setAttribute("y1", panel.arrowsHeight + 4);
            sep.setAttribute("x2", x);
            sep.setAttribute("y2", panel.arrowsHeight + panel.buttonsHeight - 4);
            sep.setAttribute("class", "stiDesignerSvgButtonSeparator");
            return sep;
        }

        var showingButtons = panel.getShowingButtons();
        var textMargin = 15;
        var buttonXPos = 0;
        var sepPositions = [];

        for (var i = 0; i < buttonProps.length; i++) {
            panel.buttons[buttonProps[i][0]].style.display = "none";
        }

        if (showingButtons.length == 0) {
            panel.hide();
            return;
        }

        for (var i = 0; i < showingButtons.length; i++) {
            var button = panel.buttons[showingButtons[i]];
            button.style.display = "";

            var isLast = (i == showingButtons.length - 1);
            var isPreLast = showingButtons.length > 1 ? (i == showingButtons.length - 2) : i == 0;
            var buttonsOffset = 6;

            if (i == 0 || i == showingButtons.length - 1) {
                button.rect.setAttribute("rx", "5");
                button.rect.setAttribute("ry", "5");
            }

            var textSizes = button.text.getBBox();
            var textWidth = parseInt(textSizes.width);
            var textHeight = parseInt(textSizes.height);
            var buttonWidth = textWidth + textMargin * 2;

            if (!isPreLast) buttonWidth += buttonsOffset;

            button.setAttribute("width", buttonWidth);
            button.setAttribute("x", buttonXPos);
            button.setAttribute("y", panel.arrowsHeight);
            button.rect.setAttribute("width", buttonWidth);
            button.text.setAttribute("x", isLast ? textMargin + buttonsOffset : textMargin);
            button.text.setAttribute("y", parseInt((panel.buttonsHeight - textHeight) / 2) + 12);

            if (isLast) {
                panel.realWidth = parseInt(buttonXPos + buttonWidth);
            }

            buttonXPos += buttonWidth - buttonsOffset;

            if (!isLast) {
                sepPositions.push(isPreLast ? buttonXPos + buttonsOffset : buttonXPos);
            }
        }

        var prevButton = showingButtons.length > 1 ? panel.buttons[showingButtons[showingButtons.length - 2]] : panel.buttons[showingButtons[0]];
        panel.removeChild(prevButton);
        panel.appendChild(prevButton);

        //add separators
        for (var i = 0; i < panel.separators.length; i++) {
            panel.removeChild(panel.separators[i]);
        }
        panel.separators = [];

        for (var i = 0; i < sepPositions.length; i++) {
            var sep = separator(sepPositions[i]);
            panel.appendChild(sep);
            panel.separators.push(sep);
        }

        panel.setPositons(component);
    }

    panel.hide = function () {
        panel.style.display = "none";
    }

    panel.setPositons = function (component) {
        var compWidth = parseInt(component.getAttribute("width"));
        var compHeight = parseInt(component.getAttribute("height"));
        var compLeft = parseInt(component.getAttribute("left"));
        var compTop = parseInt(component.getAttribute("top"));

        var pageWidth = parseInt(page.getAttribute("width"));
        var pageHeight = parseInt(page.getAttribute("height"));

        var panelHeight = panel.realHeight;
        var panelWidth = panel.realWidth;
        var panelMargin = 15;

        var paintPanel = jsObject.options.paintPanel;
        var paintPanelWidth = paintPanel.offsetWidth;
        var paintPanelHeight = paintPanel.offsetHeight;
        var paintPanelPadding = jsObject.options.paintPanelPadding;

        var arrowWidth = 20;
        var arrowHeight = panel.arrowsHeight;

        //Set Vertical Pos
        var panelTop = compTop + compHeight + panelMargin;
        var panelBottom = panelTop + panelHeight;

        if (panelBottom < pageHeight && panelBottom + paintPanelPadding < paintPanel.scrollTop + paintPanelHeight) {
            //show at bottom
            panel.setAttribute("y", panelTop);
            panel.arrow.style.display = "";
            panel.arrow.setAttribute("points",
                (panelWidth / 2 - arrowWidth / 2) + "," + panel.arrowsHeight + " " +
                (panelWidth / 2) + "," + 0 + " " +
                (panelWidth / 2 + arrowWidth / 2) + "," + panel.arrowsHeight
            );
        }
        else {
            panelTop = compTop - panelMargin - panelHeight - panel.arrowsHeight;
            if (panelTop > 0 && panelTop + paintPanelPadding > paintPanel.scrollTop) {
                //show at top
                panel.setAttribute("y", panelTop);
                panel.arrow.style.display = "";
                panel.arrow.setAttribute("points",
                    (panelWidth / 2 - arrowWidth / 2) + "," + panelHeight + " " +
                    (panelWidth / 2) + "," + (panelHeight + arrowHeight) + " " +
                    (panelWidth / 2 + arrowWidth / 2) + "," + panelHeight
                );

            }
            else {
                panel.arrow.style.display = "none";
                panel.setAttribute("y", 5);
            }
        }

        //Set Horizontal Pos
        var panelLeft = compLeft + (compWidth - panelWidth) / 2;

        if (panelLeft < 0) {
            panelLeft = 5;
        }

        if (panelLeft + paintPanelPadding < paintPanel.scrollLeft) {
            panelLeft = paintPanel.scrollLeft + 5;
        }

        if (panelLeft + panelWidth > pageWidth) {
            panelLeft = pageWidth - panelWidth - 5;
        }

        if (panelLeft + panelWidth + paintPanelPadding > paintPanel.scrollLeft + paintPanelWidth) {
            panelLeft = paintPanel.scrollLeft + paintPanelWidth - panelWidth - 5;
        }

        panel.setAttribute("x", panelLeft);
    }
}