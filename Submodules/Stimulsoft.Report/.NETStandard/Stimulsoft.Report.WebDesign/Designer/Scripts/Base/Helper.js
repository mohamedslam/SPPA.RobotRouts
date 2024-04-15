
StiMobileDesigner.prototype.CreateSvgElement = function (tagName) {
    return ("createElementNS" in document ? document.createElementNS("http://www.w3.org/2000/svg", tagName) : document.createElement(tagName));
}

StiMobileDesigner.prototype.IsTouchDevice = function () {
    return ('ontouchstart' in document.documentElement) && this.isMobileDevice.any() != null;
}

StiMobileDesigner.prototype.isMobileDevice = {
    Android: function () {
        return navigator.userAgent.match(/Android/i);
    },
    BlackBerry: function () {
        return navigator.userAgent.match(/BlackBerry/i);
    },
    iOS: function () {
        return navigator.userAgent.match(/iPhone|iPad|iPod|Macintosh/i);
    },
    Opera: function () {
        return navigator.userAgent.match(/Opera Mini/i);
    },
    Windows: function () {
        return navigator.userAgent.match(/IEMobile/i);
    },
    any: function () {
        return (this.Android() || this.BlackBerry() || this.iOS() || this.Opera() || this.Windows());
    }
};

StiMobileDesigner.prototype.GetImagesScalingFactor = function () {
    var devicePixelRatio = window.devicePixelRatio || (window.deviceXDPI && window.logicalXDPI ? window.deviceXDPI / window.logicalXDPI : 1);
    if (!devicePixelRatio || devicePixelRatio <= 1) return "1";
    else return devicePixelRatio.toString();
};

StiMobileDesigner.prototype.SetSelectedObject = function (object) {
    //Hide resizing icons
    if (this.options.selectedObject != null && this.options.selectedObject.typeComponent != "StiPage" && this.options.selectedObject.typeComponent != "StiReport") {
        this.options.selectedObject.changeVisibilityStateResizingIcons(false);
    }

    //Show resizing icons
    if (this.options.selectedObjects == null ||
        (this.options.selectedObjects && this.options.selectedObjects.length == 1) ||
        (this.options.selectedObjects && !this.IsContains(this.options.selectedObjects, object))) {

        if (this.options.selectedObjects && this.options.selectedObjects.length == 1) { object = this.options.selectedObjects[0]; }
        this.options.selectedObject = object;
        if (object.typeComponent != "StiPage" && object.typeComponent != "StiReport") {
            object.changeVisibilityStateResizingIcons(true);

            if (object.controls) {
                if (object.controls.titleButton) object.controls.titleButton.show();
                if (object.controls.sortButton && object.properties.allowUserSorting) object.controls.sortButton.show();
                if (object.controls.chartTypesButtons) {
                    for (var i = 0; i < object.controls.chartTypesButtons.length; i++) {
                        object.controls.chartTypesButtons[i].show();
                    }
                }
            }
        }
        this.DeleteSelectedLines();
        this.options.selectedObjects = null;

        if (this.options.dictionaryPanel) {
            this.options.dictionaryPanel.setFocused(false);
        }
    }

    //Select current component in the report tree
    if (this.options.reportTree) {
        var item = object.typeComponent == "StiReport" ? this.options.reportTree.reportItem : this.options.reportTree.items[object.properties.name];
        if (item) {
            item.setSelected();
            item.openTree();
        }
    }

    //Show Positions on Status Panel
    if (object != null && object.typeComponent != "StiPage" && object.typeComponent != "StiReport") {
        this.options.statusPanel.showPositions(object.properties.clientLeft || object.properties.unitLeft, object.properties.clientTop || object.properties.unitTop, object.properties.unitWidth, object.properties.unitHeight);
    }
    else {
        this.options.statusPanel.showPositions();
    }

    //Return to main properties
    var propertiesPanel = this.options.propertiesPanel;
    if (object != null && object.typeComponent != "StiCrossField" && propertiesPanel.editCrossTabMode) {
        propertiesPanel.setEditCrossTabMode(false);
        if (propertiesPanel.currentContainerName == "Properties") {
            propertiesPanel.propertiesToolBar.changeVisibleState(true);
        }
    }

    if (object.typeComponent == "StiPage") {
        if (this.options.insertPanel) this.options.insertPanel.setMode();
        if (this.options.toolbox) this.options.toolbox.setMode();
    }
}

StiMobileDesigner.prototype.SetSelectedObjectsByNames = function (page, objectNames) {
    this.DeleteSelectedLines();
    var selectedObjects = this.options.selectedObject ? [this.options.selectedObject] : this.options.selectedObjects;
    if (!selectedObjects) return;

    for (var i = 0; i < selectedObjects.length; i++) {
        if (selectedObjects[i] != null && selectedObjects[i].typeComponent != "StiPage" && selectedObjects[i].typeComponent != "StiReport")
            selectedObjects[i].changeVisibilityStateResizingIcons(false);
    }
    this.options.selectedObject = null;
    this.options.selectedObjects = [];

    for (var i = 0; i < objectNames.length; i++) {
        var component = page.components[objectNames[i]];
        if (component) this.options.selectedObjects.push(component);
    }
    this.PaintSelectedLines();
}

StiMobileDesigner.prototype.PaintSelectedLines = function () {
    if (this.options.multiSelectHelperControls) this.DeleteSelectedLines();

    var selectedObjects = this.options.selectedObjects;
    if (!selectedObjects || selectedObjects.length == 0) return;

    //if multiselect only one object
    if (selectedObjects && selectedObjects.length == 1) {
        selectedObjects[0].setSelected();
        this.UpdatePropertiesControls();
        return;
    }

    //remove single select
    var selectedObject = this.options.selectedObject;
    if (selectedObject != null && selectedObject.typeComponent != "StiPage" && selectedObject.typeComponent != "StiReport") {
        selectedObject.changeVisibilityStateResizingIcons(false);
    }
    this.options.selectedObject = null;

    var lines = { vert: [], hor: [] };
    var jsObject = this;

    var createLine = function (page, x1, y1, x2, y2) {
        var line = ("createElementNS" in document) ? document.createElementNS("http://www.w3.org/2000/svg", "line") : document.createElement("line");
        line.page = page;
        line.coord = { x1: x1, y1: y1, x2: x2, y2: y2 };
        line.style.strokeWidth = "1";
        line.style.stroke = page.isDashboard ? jsObject.GetHTMLColor(page.properties.selectionBorderColor) : jsObject.options.themeColors[jsObject.GetThemeColor()];
        line.style.strokeDasharray = "3,2";

        line.repaint = function () {
            var pageMarginsPx = this.page.marginsPx;
            var zoom = jsObject.options.report.zoom;
            var coordX1 = jsObject.StrToDouble(this.coord.x1);
            var coordY1 = jsObject.StrToDouble(this.coord.y1);
            var coordX2 = jsObject.StrToDouble(this.coord.x2);
            var coordY2 = jsObject.StrToDouble(this.coord.y2);

            var x1 = jsObject.ConvertUnitToPixel(coordX1, this.page.isDashboard) * zoom + pageMarginsPx[0];
            var y1 = jsObject.ConvertUnitToPixel(coordY1, this.page.isDashboard) * zoom + pageMarginsPx[1];
            var x2 = jsObject.ConvertUnitToPixel(coordX2, this.page.isDashboard) * zoom + pageMarginsPx[0];
            var y2 = jsObject.ConvertUnitToPixel(coordY2, this.page.isDashboard) * zoom + pageMarginsPx[1];

            var roundedCoordinates = jsObject.GetRoundedLineCoordinates([x1, y1, x2, y2]);

            var xOffset = jsObject.options.xOffset;
            var yOffset = jsObject.options.yOffset;

            this.setAttribute("x1", roundedCoordinates[0] + xOffset);
            this.setAttribute("y1", roundedCoordinates[1] + yOffset);
            this.setAttribute("x2", roundedCoordinates[2] + xOffset);
            this.setAttribute("y2", roundedCoordinates[3] + yOffset);
        }

        return line;
    }

    var page;
    var linesPoints = [];
    for (var i = 0; i < selectedObjects.length; i++) {
        var component = selectedObjects[i];
        if (!page && this.options.report) page = this.options.report.pages[component.properties.pageName];
        var leftComp = this.StrToDouble(component.properties.unitLeft);
        var topComp = this.StrToDouble(component.properties.unitTop);
        var rightComp = leftComp + this.StrToDouble(component.properties.unitWidth);
        var bottomComp = topComp + this.StrToDouble(component.properties.unitHeight);

        if (!this.IsContains(lines.vert, leftComp)) lines.vert.push(leftComp);
        if (!this.IsContains(lines.vert, rightComp)) lines.vert.push(rightComp);
        if (!this.IsContains(lines.hor, topComp)) lines.hor.push(topComp);
        if (!this.IsContains(lines.hor, bottomComp)) lines.hor.push(bottomComp);

        for (var k = 0; k < component.controls.borders.length; k++) {
            component.controls.borders[k].style.display = "none";
        }
    }

    lines.hor = lines.hor.sort(function (a, b) { return a - b });
    lines.vert = lines.vert.sort(function (a, b) { return a - b });

    var left = jsObject.RoundPlus(lines.vert[0], 5);
    var right = jsObject.RoundPlus(lines.vert[lines.vert.length - 1], 5);
    var top = jsObject.RoundPlus(lines.hor[0], 5);
    var bottom = jsObject.RoundPlus(lines.hor[lines.hor.length - 1], 5);

    var multiSelectHelperControls = { page: page, lines: [], resizingPoints: [] };
    this.options.multiSelectHelperControls = multiSelectHelperControls;

    var createResizingPoint = function (resizingType) {
        var point = ("createElementNS" in document) ? document.createElementNS("http://www.w3.org/2000/svg", "rect") : document.createElement("rect");
        point.setAttribute("width", 4);
        point.setAttribute("height", 4);
        point.style.fill = "#696969";
        point.style.strokeWidth = "#696969";
        point.style.stroke = "#696969";
        point.resizingType = resizingType;
        point.style.cursor = jsObject.GetCursorType(resizingType);

        point.onmousedown = function (event) {
            event.preventDefault();
            jsObject.options.startMousePos = [event.clientX || event.x, event.clientY || event.y];
            if (jsObject.options.currentPage) jsObject.options.currentPage.style.cursor = this.style.cursor;
            var selectedObjects = jsObject.options.selectedObjects;
            var fixedComponents = { left: [], top: [], right: [], bottom: [] };
            var selectArea = {
                left: jsObject.ConvertUnitToPixel(left, page.isDashboard) * jsObject.options.report.zoom,
                top: jsObject.ConvertUnitToPixel(top, page.isDashboard) * jsObject.options.report.zoom,
                right: jsObject.ConvertUnitToPixel(right, page.isDashboard) * jsObject.options.report.zoom,
                bottom: jsObject.ConvertUnitToPixel(bottom, page.isDashboard) * jsObject.options.report.zoom
            }
            jsObject.options.in_resize = [[], "Multi" + this.resizingType, [], fixedComponents, selectArea];

            for (var i = 0; i < selectedObjects.length; i++) {
                var startValues = {
                    height: parseInt(selectedObjects[i].getAttribute("height")),
                    width: parseInt(selectedObjects[i].getAttribute("width")),
                    left: parseInt(selectedObjects[i].getAttribute("left")),
                    top: parseInt(selectedObjects[i].getAttribute("top"))
                }
                jsObject.options.in_resize[0].push(selectedObjects[i]);
                jsObject.options.in_resize[2].push(startValues);
                selectedObjects[i].startWidth = startValues.width;
                selectedObjects[i].startHeight = startValues.height;

                var compLeft = jsObject.RoundPlus(jsObject.StrToDouble(selectedObjects[i].properties.unitLeft), 5);
                var compRight = jsObject.RoundPlus(jsObject.StrToDouble(selectedObjects[i].properties.unitLeft) + jsObject.StrToDouble(selectedObjects[i].properties.unitWidth), 5);
                var compTop = jsObject.RoundPlus(jsObject.StrToDouble(selectedObjects[i].properties.unitTop), 5);
                var compBottom = jsObject.RoundPlus(jsObject.StrToDouble(selectedObjects[i].properties.unitTop) + jsObject.StrToDouble(selectedObjects[i].properties.unitHeight), 5);

                if (compLeft == left) fixedComponents.left.push(selectedObjects[i]);
                if (compRight == right) fixedComponents.right.push(selectedObjects[i]);
                if (compTop == top) fixedComponents.top.push(selectedObjects[i]);
                if (compBottom == bottom) fixedComponents.bottom.push(selectedObjects[i]);
            }
        }

        return point;
    }

    //Paint All Lines
    for (var i = 0; i < lines.hor.length; i++) {
        var line = createLine(page, left, lines.hor[i], right, lines.hor[i]);
        line.repaint();
        multiSelectHelperControls.lines.push(line);
        page.appendChild(line);
    }

    for (var i = 0; i < lines.vert.length; i++) {
        var line = createLine(page, lines.vert[i], top, lines.vert[i], bottom);
        line.repaint();
        multiSelectHelperControls.lines.push(line);
        page.appendChild(line);
    }

    //Paint Resizing Icons
    var pageMarginsPx = page.marginsPx;

    var leftPoint = this.ConvertUnitToPixel(left, page.isDashboard) * this.options.report.zoom + pageMarginsPx[0];
    var topPoint = this.ConvertUnitToPixel(top, page.isDashboard) * this.options.report.zoom + pageMarginsPx[1];
    var rightPoint = this.ConvertUnitToPixel(right, page.isDashboard) * this.options.report.zoom + pageMarginsPx[0];
    var bottomPoint = this.ConvertUnitToPixel(bottom, page.isDashboard) * this.options.report.zoom + pageMarginsPx[1];

    var resizingTypes = ["LeftTop", "Top", "RightTop", "Right", "RightBottom", "Bottom", "LeftBottom", "Left"];
    for (var i = 0; i < resizingTypes.length; i++) {
        var resizingPoint = createResizingPoint(resizingTypes[i]);
        multiSelectHelperControls.resizingPoints.push(resizingPoint);
        page.appendChild(resizingPoint);

        if (i == 0 || i == 6 || i == 7) resizingPoint.setAttribute("x", parseInt(leftPoint - 2) + this.options.xOffset);
        if (i == 2 || i == 3 || i == 4) resizingPoint.setAttribute("x", parseInt(rightPoint - 2) + this.options.xOffset);
        if (i == 1 || i == 5) resizingPoint.setAttribute("x", parseInt((rightPoint + leftPoint) / 2) - 2 + this.options.xOffset);
        if (i == 0 || i == 1 || i == 2) resizingPoint.setAttribute("y", parseInt(topPoint - 2) + this.options.yOffset);
        if (i == 4 || i == 5 || i == 6) resizingPoint.setAttribute("y", parseInt(bottomPoint - 2) + this.options.yOffset);
        if (i == 3 || i == 7) resizingPoint.setAttribute("y", parseInt((bottomPoint + topPoint) / 2 - 2) + this.options.yOffset);
    }
}

StiMobileDesigner.prototype.DeleteSelectedLines = function () {
    var selectedObjects = this.options.selectedObjects;
    if (selectedObjects) {
        for (var i = 0; i < selectedObjects.length; i++) {
            var borders = selectedObjects[i].controls.borders;
            for (var k = 0; k < borders.length; k++) {
                borders[k].style.display = "";
            }
        }
    }

    var multiSelectHelperControls = this.options.multiSelectHelperControls;
    if (multiSelectHelperControls) {
        for (var i = 0; i < multiSelectHelperControls.lines.length; i++) {
            var page = multiSelectHelperControls.page;
            page.removeChild(multiSelectHelperControls.lines[i]);
        }
        for (var i = 0; i < multiSelectHelperControls.resizingPoints.length; i++) {
            var page = multiSelectHelperControls.page;
            page.removeChild(multiSelectHelperControls.resizingPoints[i]);
        }
    }
    this.options.multiSelectHelperControls = null;
}

StiMobileDesigner.prototype.GetAllComponentsHaveImage = function (pageName) {
    var components = "";
    for (var componentName in this.options.report.pages[pageName].components) {
        var component = this.options.report.pages[pageName].components[componentName];
        if (component.controls.imageContent.href.baseVal != "") {
            if (components != "") components += ";";
            components += componentName;
        }
    }

    return (components != "") ? components : false;
}

StiMobileDesigner.prototype.UpdatePropertiesControls = function () {
    if (this.options.layoutPanel) this.options.layoutPanel.updateControls();
    if (this.options.pagePanel && this.options.workPanel.currentPanel == this.options.pagePanel) this.options.pagePanel.updateControls();
    this.options.homePanel.updateControls();
    this.options.propertiesPanel.updateControls();
    if (this.options.statusPanel && this.options.selectedObject) {
        this.options.statusPanel.componentNameCell.innerHTML = this.options.selectedObject.typeComponent == "StiReport"
            ? StiBase64.decode(this.options.selectedObject.properties.reportName.replace("Base64Code;", "")) : this.options.selectedObject.properties.name;
    }
    this.updateIframes();
}

StiMobileDesigner.prototype.updateIframes = function () {
    for (var i in this.options.report.pages) {
        var page = this.options.report.pages[i];
        for (var j in page.components) {
            var component = page.components[j];
            if (component.controls && component.controls.iframeContent)
                component.controls.iframeContent.style.display = page == this.options.currentPage ? "" : "none";
        }
    }
}

StiMobileDesigner.prototype.WriteAllProperties = function (object, properties) {
    for (var propertyName in properties) {
        if (propertyName == "imageContentForPaint" && properties[propertyName] == "keepPrevContent") {
            continue;
        }
        object.properties[propertyName] = properties[propertyName];

        if (propertyName == "gridSize") {
            object.properties.gridSize = this.StrToDouble(properties.gridSize);
        }
    }
}

StiMobileDesigner.prototype.SetObjectToCenter = function (object) {
    var leftPos = (this.options.mobileDesigner.offsetWidth / 2 - object.offsetWidth / 2);
    var topPos = (this.options.mobileDesigner.offsetHeight / 2 - object.offsetHeight / 2);
    object.style.left = leftPos > 0 ? leftPos + "px" : 0;
    object.style.top = topPos > 0 ? topPos + "px" : 0;
}

StiMobileDesigner.prototype.SetObjectToPropertiesPanelCorner = function (object) {
    var leftPos = this.FindPosX(this.options.propertiesPanel, "stiDesignerMainPanel") + this.options.propertiesPanel.offsetWidth + 10;
    var topPos = this.FindPosY(this.options.propertiesPanel, "stiDesignerMainPanel") + 10;
    object.style.left = leftPos + "px";
    object.style.top = topPos + "px";
}

StiMobileDesigner.prototype.Item = function (name, caption, imageName, key, styleProperties, haveSubMenu, type, imageSizes) {
    var item = {
        name: name,
        caption: caption,
        imageName: imageName,
        key: key,
        styleProperties: styleProperties,
        haveSubMenu: haveSubMenu,
        type: type,
        imageSizes: imageSizes
    }

    return item;
}

StiMobileDesigner.prototype.GetColorFromBrushStr = function (brushStr) {
    if (brushStr == "StiEmptyValue") return "StiEmptyValue";
    if (brushStr == "0") return "255,255,255";
    var brushArray = brushStr.split("!");

    return (brushArray.length > 1) ? brushArray[1] : brushArray[0];
}

StiMobileDesigner.prototype.GetSolidBrushFromStrColor = function (colorStr) {
    return "1!" + colorStr;
}

StiMobileDesigner.prototype.GetRelationBySourceName = function (parent, name) {
    if (!parent) return null;
    var relations = parent.relations;

    for (var i = 0; i < relations.length; i++) {
        if (relations[i].nameInSource == name)
            return relations[i];
    }

    return null;
}

StiMobileDesigner.prototype.FindComponentByName = function (name) {
    if (!this.options.report) return false;
    for (var pageName in this.options.report.pages)
        for (var componentName in this.options.report.pages[pageName].components) {
            var component = this.options.report.pages[pageName].components[componentName];
            if (component.properties.name == name) return component;
        }

    return false;
}

StiMobileDesigner.prototype.newGuid = (function () {
    var CHARS = '0123456789abcdefghijklmnopqrstuvwxyz'.split('');
    return function (len, radix) {
        var chars = CHARS, uuid = [], rnd = Math.random;
        radix = radix || chars.length;

        if (len) {
            for (var i = 0; i < len; i++) uuid[i] = chars[0 | rnd() * radix];
        } else {
            var r;
            uuid[8] = uuid[13] = uuid[18] = uuid[23] = '-';
            uuid[14] = '4';

            for (var i = 0; i < 36; i++) {
                if (!uuid[i]) {
                    r = 0 | rnd() * 16;
                    uuid[i] = chars[(i == 19) ? (r & 0x3) | 0x8 : r & 0xf];
                }
            }
        }

        return uuid.join('');
    };
})();

StiMobileDesigner.prototype.generateKey = function () {
    return this.newGuid().replace(/-/g, '');
}

StiMobileDesigner.prototype.GetCountObjects = function (objectArray) {
    return objectArray ? Object.keys(objectArray).length : 0;
}

StiMobileDesigner.prototype.SetEnabledAllControls = function (state) {
    for (var name in this.options.buttons)
        if (!this.options.buttons[name].allwaysEnabled && this.options.buttons[name]["setEnabled"])
            this.options.buttons[name].setEnabled(state);

    for (var name in this.options.controls)
        if (!this.options.controls[name].allwaysEnabled && this.options.controls[name]["setEnabled"])
            this.options.controls[name].setEnabled(state);

    if (!state && this.options.statusPanel) this.options.statusPanel.componentNameCell.innerHTML = "";
    if (!state && this.options.dictionaryPanel) this.options.dictionaryPanel.createDataHintItem.style.display = "none";
}

StiMobileDesigner.prototype.CreateMetaTag = function (head) {
    if (this.options.head) {
        var meta = document.createElement("META");
        meta.setAttribute("content", "width=device-width; initial-scale=1.0; maximum-scale=1.0; user-scalable=0;");
        meta.setAttribute("stimulsoft", "stimulsoft");
        this.options.head.appendChild(meta);
    }
}

StiMobileDesigner.prototype.AddCustomOpenTypeFontsCss = function () {
    for (var i = 0; i < this.options.fontNames.length; i++) {        
        var fontData = this.options.fontNames[i].data;
        if (fontData) {
            this.AddCustomFontsCss(this.GetCustomFontsCssText(fontData, this.options.fontNames[i].value));
        }
    }
}

StiMobileDesigner.prototype.GetCustomFontsCssText = function (fontData, fontFamilyName) {
    var cssText = "@font-face {\r\n" +
        "font-family: '" + fontFamilyName + "';\r\n" +
        "src: url(" + fontData + ");\r\n }";

    return cssText;
}

StiMobileDesigner.prototype.AddCustomFontsCss = function (cssText) {
    if (this.options.head && cssText) {
        var style = document.createElement("style");
        style.setAttribute("stimulsoft", "stimulsoft");
        style.innerHTML = cssText;
        this.options.head.appendChild(style);
        return style;
    }
    return null;
}

StiMobileDesigner.prototype.ShowComponentForm = function (component, additionalParams) {
    var jsObject = this;
    if (!component) return;
    var canChange = (component.properties.restrictions && (component.properties.restrictions == "All" || component.properties.restrictions.indexOf("AllowChange") >= 0)) ||
        !component.properties.restrictions;
    if (!canChange) return;

    if (component.typeComponent == "StiText" || component.typeComponent == "StiTextInCells" ||
        component.typeComponent == "StiZipCode" || component.typeComponent == "StiTableCell") {
        this.InitializeTextEditorForm(function (textEditorForm) {
            textEditorForm.propertyName = component.typeComponent == "StiZipCode" ? "code" : "text";
            textEditorForm.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiImage" || component.typeComponent == "StiTableCellImage") {
        this.InitializeImageForm(function (imageForm) {
            imageForm.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiBarCode") {
        jsObject.HideOthersEditForms();
        this.SendCommandStartEditBarCodeComponent(component.properties.name);
    }
    else if (component.typeComponent == "StiDataBand" || component.typeComponent == "StiCrossDataBand" ||
        component.typeComponent == "StiHierarchicalBand" || component.typeComponent == "StiTable") {
        this.InitializeDataForm(function (dataForm) {
            dataForm.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiChart") {
        if (component.properties.editorType == "Simple") {
            this.InitializeEditChartSimpleForm(function (editChartForm) {
                editChartForm.currentChartComponent = component;
                editChartForm.oldSvgContent = component.properties.svgContent;
                jsObject.HideOthersEditForms();
                jsObject.SendCommandStartEditChartComponent(component.properties.name, editChartForm.name);
            });
        }
        else {
            this.InitializeEditChartForm(function (editChartForm) {
                editChartForm.currentChartComponent = component;
                editChartForm.oldSvgContent = component.properties.svgContent;
                jsObject.HideOthersEditForms();
                jsObject.SendCommandStartEditChartComponent(component.properties.name, editChartForm.name);
            });
        }
    }
    else if (component.typeComponent == "StiSparkline") {
        this.InitializeEditSparklineForm(function (form) {
            form.currentSparklineComponent = component;
            form.oldSvgContent = component.properties.svgContent;
            jsObject.HideOthersEditForms();
            jsObject.SendCommandStartEditSparklineComponent(component.properties.name);
        });
    }
    else if (component.typeComponent == "StiMathFormula") {
        this.InitializeEditMathFormulaForm(function (form) {
            form.show(component);
        });
    }
    else if (component.typeComponent == "StiElectronicSignature") {
        this.InitializeEditElectronicSignatureForm(function (form) {
            form.show(component);
        });
    }
    else if (component.typeComponent == "StiMap") {
        this.InitializeEditMapForm(function (editMapForm) {
            editMapForm.currentMapComponent = component;
            jsObject.HideOthersEditForms();
            jsObject.SendCommandStartEditMapComponent(component.properties.name, additionalParams);
        });
    }
    else if (component.typeComponent == "StiGauge") {
        this.InitializeEditGaugeForm(function (editGaugeForm) {
            editGaugeForm.currentGaugeComponent = component;
            jsObject.HideOthersEditForms();
            jsObject.SendCommandStartEditGaugeComponent(component.properties.name);
        });
    }
    else if (component.typeComponent == "StiRichText" || component.typeComponent == "StiTableCellRichText") {
        jsObject.InitializeEditRichTextForm(function (editRichTextForm) {
            editRichTextForm.show();
        });
    }
    else if (component.typeComponent == "StiSubReport") {
        jsObject.InitializeSubReportForm(function (subReportForm) {
            subReportForm.show();
        });
    }
    else if (component.typeComponent == "StiGroupHeaderBand" || component.typeComponent == "StiCrossGroupHeaderBand") {
        jsObject.InitializeGroupHeaderForm(function (groupHeaderForm) {
            groupHeaderForm.show();
        });
    }
    else if (component.typeComponent == "StiCrossTab") {
        jsObject.HideOthersEditForms();
        this.SendCommandStartEditCrossTabComponent(component.properties.name);
    }
    else if (component.typeComponent == "StiClone") {
        this.InitializeCloneContainerForm(function (cloneContainerForm) {
            cloneContainerForm.show();
        });
    }
    else if (component.typeComponent == "StiShape") {
        jsObject.HideOthersEditForms();
        this.SendCommandStartEditShapeComponent(component.properties.name);
    }
    else if (component.typeComponent == "StiTableOfContents") {
        jsObject.InitializeTableOfContentsForm(function (form) {
            form.show();
        });
    }
    else if (component.typeComponent == "StiTableElement") {
        this.InitializeEditTableElementForm(function (form) {
            form.currentTableElement = component;
            form.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiImageElement") {
        this.InitializeEditImageElementForm(function (form) {
            form.currentImageElement = component;
            form.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiTextElement") {
        this.InitializeEditTextElementForm(function (form) {
            form.currentTextElement = component;
            form.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiRegionMapElement") {
        this.InitializeEditRegionMapElementForm(function (form) {
            form.currentRegionMapElement = component;
            if (additionalParams && additionalParams.createdByDragged) {
                form.changeVisibleState(true);
                form.style.display = "none";
                form.controls.mapID.button.action();
            }
            else {
                form.changeVisibleState(true);
            }
        });
    }
    else if (component.typeComponent == "StiOnlineMapElement") {
        this.InitializeEditOnlineMapElementForm(function (form) {
            form.currentOnlineMapElement = component;
            form.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiProgressElement") {
        this.InitializeEditProgressElementForm(function (form) {
            form.currentProgressElement = component;
            form.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiIndicatorElement") {
        this.InitializeEditIndicatorElementForm(function (form) {
            form.currentIndicatorElement = component;
            form.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiChartElement") {
        this.InitializeEditChartElementForm(function (form) {
            form.currentChartElement = component;
            form.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiGaugeElement") {
        this.InitializeEditGaugeElementForm(function (form) {
            form.currentGaugeElement = component;
            form.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiShapeElement") {
        this.InitializeEditShapeElementForm(function (form) {
            form.currentShapeElement = component;
            form.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiPivotTableElement") {
        this.InitializeEditPivotTableElementForm(function (form) {
            form.currentPivotTableElement = component;
            form.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiListBoxElement") {
        this.InitializeEditListBoxElementForm(function (form) {
            form.currentListBoxElement = component;
            form.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiComboBoxElement") {
        this.InitializeEditComboBoxElementForm(function (form) {
            form.currentComboBoxElement = component;
            form.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiTreeViewElement") {
        this.InitializeEditTreeViewElementForm(function (form) {
            form.currentTreeViewElement = component;
            form.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiTreeViewBoxElement") {
        this.InitializeEditTreeViewBoxElementForm(function (form) {
            form.currentTreeViewBoxElement = component;
            form.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiDatePickerElement") {
        this.InitializeEditDatePickerElementForm(function (form) {
            form.currentDatePickerElement = component;
            form.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiCardsElement") {
        this.InitializeEditCardsElementForm(function (form) {
            form.currentCardsElement = component;
            form.changeVisibleState(true);
        });
    }
    else if (component.typeComponent == "StiButtonElement") {
        if (this.options.propertiesPanel && this.options.propertiesPanel.eventsPropertiesPanel) {
            this.options.propertiesPanel.eventsPropertiesPanel.update();
            this.options.propertiesPanel.eventsPropertiesPanel.properties.ClickEvent.propertyControl.button.action();
        }
    }
}

StiMobileDesigner.prototype.HideOthersEditForms = function () {
    var form = this.options.currentForm;
    if (form && form.visible && (form.name == "editChartSimpleForm" || form.name == "editSparkline" || form.name == "barCodeForm" || form.name == "shapeForm")) {
        form.buttonClose.action();
    }
}

StiMobileDesigner.prototype.UpdateStateUndoRedoButtons = function () {
    this.options.buttons.undoButton.setEnabled(true);
    this.options.buttons.redoButton.setEnabled(false);
}

StiMobileDesigner.prototype.BackToSelectedComponent = function (componentName) {
    var selectedComponent = this.FindComponentByName(componentName);
    if (!selectedComponent) selectedComponent = this.options.report.pages[componentName];
    if (!selectedComponent) return;

    if (selectedComponent.typeComponent == "StiPage") {
        if (this.options.currentPage.properties.name != selectedComponent.properties.name)
            this.options.paintPanel.showPage(selectedComponent);
    }
    else {
        if (this.options.currentPage.properties.name != selectedComponent.properties.pageName) {
            var selectedPage = this.options.report.pages[selectedComponent.properties.pageName];
            if (selectedPage) this.options.paintPanel.showPage(selectedPage);
        }
        selectedComponent.setSelected();
        this.SetComponentOnTopLevel(selectedComponent);
        this.UpdatePropertiesControls();
    }
}

StiMobileDesigner.prototype.GetConstMargins = function () {
    var constMargins = {
        "cm": { "marginsNormal": "1", "marginsNarrow": "0.5", "marginsWide": "2" },
        "in": { "marginsNormal": "0.4", "marginsNarrow": "0.2", "marginsWide": "0.8" },
        "hi": { "marginsNormal": "39", "marginsNarrow": "19.5", "marginsWide": "78" },
        "mm": { "marginsNormal": "9.9", "marginsNarrow": "4.95", "marginsWide": "19.8" }
    }

    return constMargins;
}

StiMobileDesigner.prototype.GetUnitShortName = function (unit) {
    if (unit == "Centimeters") return "cm";
    if (unit == "HundredthsOfInch") return "hi";
    if (unit == "Inches") return "in";
    if (unit == "Millimeters") return "mm";
    return "cm";
}

StiMobileDesigner.prototype.SortArrayToSortStr = function (sortArray) {
    var sortStr = this.loc.FormBand.SortBy + " ";
    for (var i = 0; i < sortArray.length; i++) {
        sortStr += sortArray[i].column;
        if (i != sortArray.length - 1) sortStr += ",";
    }

    return sortStr;
}

StiMobileDesigner.prototype.SetWatermarkImagePos = function (page, widthWatermark, heightWatermark) {
    switch (page.properties.waterMarkImageAlign) {
        case "TopLeft": {
            page.controls.waterMarkImage.setAttribute("x", "0");
            page.controls.waterMarkImage.setAttribute("y", "0");
            break;
        }
        case "TopCenter": {
            page.controls.waterMarkImage.setAttribute("x", page.widthPx / 2 - widthWatermark / 2);
            page.controls.waterMarkImage.setAttribute("y", "0");
            break;
        }
        case "TopRight": {
            page.controls.waterMarkImage.setAttribute("x", page.widthPx - widthWatermark);
            page.controls.waterMarkImage.setAttribute("y", "0");
            break;
        }
        case "MiddleLeft": {
            page.controls.waterMarkImage.setAttribute("x", "0");
            page.controls.waterMarkImage.setAttribute("y", page.heightPx / 2 - heightWatermark / 2);
            break;
        }
        case "MiddleCenter": {
            page.controls.waterMarkImage.setAttribute("x", page.widthPx / 2 - widthWatermark / 2);
            page.controls.waterMarkImage.setAttribute("y", page.heightPx / 2 - heightWatermark / 2);
            break;
        }
        case "MiddleRight": {
            page.controls.waterMarkImage.setAttribute("x", page.widthPx - widthWatermark);
            page.controls.waterMarkImage.setAttribute("y", page.heightPx / 2 - heightWatermark / 2);
            break;
        }
        case "BottomLeft": {
            page.controls.waterMarkImage.setAttribute("x", "0");
            page.controls.waterMarkImage.setAttribute("y", page.heightPx - heightWatermark);
            break;
        }
        case "BottomCenter": {
            page.controls.waterMarkImage.setAttribute("x", page.widthPx / 2 - widthWatermark / 2);
            page.controls.waterMarkImage.setAttribute("y", page.heightPx - heightWatermark);
            break;
        }
        case "BottomRight": {
            page.controls.waterMarkImage.setAttribute("x", page.widthPx - widthWatermark);
            page.controls.waterMarkImage.setAttribute("y", page.heightPx - heightWatermark);
            break;
        }
    };
}

StiMobileDesigner.prototype.GetElementNumberInArray = function (element, array) {
    if (!array || !array.length) return -1
    return array.indexOf(element);
}

StiMobileDesigner.prototype.ShowHelpWindow = function (url) {
    this.openNewWindow("https://www.stimulsoft.com/" + (this.options.helpLanguage || "en") + "/documentation/online/" + url, undefined, undefined, false);
}

StiMobileDesigner.prototype.HelpLinks = {
    "clipboard": "user-manual/index.html?reports_designer_ribbon_mode_2013_ribbon_tabs_home_clipboard.htm",
    "font": "user-manual/index.html?reports_designer_ribbon_mode_2013_ribbon_tabs_home_font.htm",
    "alignment": "user-manual/index.html?reports_designer_ribbon_mode_2013_ribbon_tabs_home_alignment.htm",
    "border": "user-manual/index.html?reports_designer_ribbon_mode_2013_ribbon_tabs_home_borders.htm",
    "borderform": "user-manual/index.html?report_internals_appearance_borders_simple_borders.htm",
    "textformat": "user-manual/index.html?report_internals_text_formatting.htm",
    "style": "user-manual/index.html?report_internals_appearance_styles.htm",
    "insertcomponent": "user-manual/index.html?reports_designer_ribbon_mode_2013_ribbon_tabs_insert.htm",
    "page": "user-manual/index.html?reports_designer_ribbon_mode_2013_ribbon_tabs_page_page_setup.htm",
    "preview": "user-manual/index.html?reports_designer_previewing_reports.htm",
    "data": "user-manual/index.html?report_internals_creating_lists_data_source_of_data_band.htm",
    "filter": "user-manual/index.html?report_internals_creating_lists_data_filterting.htm",
    "image": "user-manual/index.html?report_internals_graphic_information_output_resources_of_images.htm",
    "watermark": "user-manual/index.html?report_internals_watermarks_watermark_property.htm",
    "columns": "user-manual/index.html?report_internals_columns_columns_on_page.htm",
    "sort": "user-manual/index.html?report_internals_creating_lists_data_sorting.htm",
    "expression": "user-manual/index.html?report_internals_expressions.htm",
    "connectionNew": "user-manual/index.html?data_data_dictionary_datasources_creating_data_source.htm",
    "connectionEdit": "user-manual/index.html?data_data_dictionary_connection.htm",
    "relationEdit": "user-manual/index.html?data_data_dictionary_relation_creating_relation.htm",
    "columnEdit": "user-manual/index.html?data_data_dictionary_datasources_creating_and_editing_data_columns.htm",
    "dataSourceEdit": "user-manual/data_data_dictionary_datasources.htm",
    "dataSourceFromOtherDatasources": "user-manual/data_data_dictionary_datasources_data_from_other_data_source.htm",
    "variableEdit": "user-manual/index.html?data_data_dictionary_variables.htm",
    "variableItems": "user-manual/index.html?data_data_dictionary_variables_panel_request_from_user_items_dialog.htm",
    "styleDesigner": "user-manual/index.html?report_internals_appearance_style_designer.htm",
    "createStyleCollection": "user-manual/index.html?report_internals_appearance_style_designer_creating_collection_of_styles.htm",
    "conditions": "user-manual/index.html?report_internals_conditional_formatting.htm",
    "reportSetup": "user-manual/index.html?reports_designer_ribbon_mode_2013_main_menu_dialog_report_setup.htm",
    "options": "user-manual/reports_designer_ribbon_mode_2013_main_menu_dialog_options.htm",
    "parameterEdit": "user-manual/index.html?data_data_dictionary_datasources_queries_parameters.htm",
    "wizard": "user-manual/index.html?reports_designer_creating_reports_in_designer_overview_master-detail_report_wizard.htm",
    "layout": "user-manual/reports_designer_ribbon_mode_2013_ribbon_tabs_layout_arrange.htm",
    "textFormat": "user-manual/report_internals_text_formatting.htm",
    "reportCheck": "user-manual/reports_designer_report_checker.htm",
    "globalizationEditor": "user-manual/reports_designer_globalization_editor.htm",
    "viewOptions": "user-manual/index.html?reports_designer_ribbon_mode_2013_ribbon_tabs_page_viewing_options.htm",
    "richtextform": "user-manual/index.html?report_internals_rich_text_output_rich_text_editor.htm",
    "barcodeform": "user-manual/report_internals_barcodes_editor.htm",
    "cloneform": "user-manual/report_internals_panels_cloning.htm",
    "subreportform": "user-manual/report_internals_sub-reports.htm",
    "crosstabform": "user-manual/index.html?report_internals_crosstable_cross_table_editor_2.htm",
    "chartform": "user-manual/reports_internals_charts_editor.htm",
    "onlineOpenReport": "cloud-reports/stimulsoft_cloud_create_and_open_report.htm",
    "onlineSaveReport": "cloud-reports/stimulsoft_cloud_saving_and_download_report.htm",
    "share": "server-manual/toolbar_share.htm",
    "interactions": "user-manual/report_internals_interaction.htm?toc=0",
    "default": "user-manual/index.html?introduction.htm",
    "tableElement": "user-manual/index.html?dashboards_table.htm",
    "chartElement": "user-manual/index.html?dashboards_chart.htm",
    "gaugeElement": "user-manual/index.html?dashboards_gauge.htm",
    "pivotTableElement": "user-manual/index.html?dashboards_pivot_table.htm",
    "indicatorElement": "user-manual/index.html?dashboards_indicator.htm",
    "progressElement": "user-manual/index.html?dashboards_progress.htm",
    "regionMapElement": "user-manual/index.html?dashboards_maps_region_map.htm",
    "lisBoxElement": "user-manual/index.html?dashboards_data_filtering_list_box.htm",
    "comboBoxElement": "user-manual/index.html?dashboards_data_filtering_combo_box.htm",
    "treeViewElement": "user-manual/index.html?dashboards_data_filtering_tree_view.htm",
    "treeViewBoxElement": "user-manual/index.html?dashboards_data_filtering_tree_view_box.htm",
    "datePickerElement": "user-manual/index.html?dashboards_data_filtering_date_picker.htm",
    "imageElement": "user-manual/index.html?dashboards_image.htm",
    "textElement": "user-manual/index.html?dashboards_text.htm",
    "shapeElement": "user-manual/index.html?dashboards_shape.htm",
    "onlineMapElement": "user-manual/index.html?dashboards_maps_online_map.htm",
    "topN": "user-manual/index.html?dashboards_data_filtering_top_n.htm",
    "elementDataFilters": "user-manual/index.html?dashboards_data_filtering_filters.htm",
    "dashboardInteractions": "user-manual/index.html?dashboards_interaction.htm",
    "dashboardConditions": "user-manual/index.html?dashboards_conditions.htm",
    "dataTransformation": "user-manual/index.html?dashboards_data_filtering_data_transformation.htm",
    "sparkline": "user-manual/index.html?report_internals_sparkline.htm#editor"
}

StiMobileDesigner.prototype.NotificationMessages = function (key) {
    var isEn = this.options.helpLanguage != "ru";
    var messages = {
        "openReportInTrial": isEn ? "You cannot open a report from the file in the Trial subscription!" : "Вы не можете открыть отчет из файла при наличии тестовой подписки!",
        "saveReportInTrial": isEn ? "You cannot save your report to a file in the Trial subscription!" : "Вы не можете сохранить отчет в файл при наличии тестовой подписки!",
        "availableDataSources": isEn ? "Upgrade your subscription to get all types of available data sources!" : "Обновите вашу подписку, чтобы получить все типы доступных источников данных!",
        "availableDataSourcesInDesktopVersion": isEn
            ? "You can use all types of data sources in a desktop version of the Stimulsoft Designer. It is available <a href='https://www.stimulsoft.com/en/downloads' target='_blank'>here</a>."
            : "Вы можете использовать все типы источников данных в настольной версии Stimulsoft Designer. Он доступен <a href='https://www.stimulsoft.com/en/downloads' target='_blank'>тут</a>.",
        "upgradeYourPlan": isEn ? "Upgrade your plan and get more possibilities for your report." : "Обновите вашу подписку и получите больше возможностей для вашего отчета. ",
        "updateYourSubscription": isEn ? "Please update your subscription" : "Пожалуйста, обновите вашу подписку",
        "subscriptionExpired": this.loc.Notices.SubscriptionExpired,
        "cloudSubscriptionExpired": isEn ? "Your Stimulsoft Cloud subscription has been expired!" : "Срок вашей подписки на Stimulsoft Cloud истек!",
        "trSubscriptionExpired": this.loc.Notices.YourTrialHasExpired,
        "trSubscriptionExpiredDescription": isEn
            ? "If you are interested in extending your trial, please <a href='mailto: sales@stimulsoft.com?subject=Extend%20My%20Stimulsoft%20Trial'>tell us why</a>."
            : "Если вы заинтересованы в продлении пробной версии, пожалуйста <a href='mailto: sales@stimulsoft.com?subject=Extend%20My%20Stimulsoft%20Trial'>сообщите нам почему</a>.",
        "accountLocked": this.loc.Notices.ActivationLockedAccountExt,
        "accountLockedDescription": isEn ? "Please contact our sales department at sales@stimulsoft.com <br> to resolve this." : "Пожалуйста, свяжитесь с нами по адресу sales@stimulsoft.com, <br> чтобы решить эту проблему.",
        "continueToUse": isEn ? "You can continue to use Stimulsoft Designer by purchasing the software." : "Вы можете продолжить использовать Stimulsoft Designer, купив программное обеспечение."
    }
    return messages[key];
}

StiMobileDesigner.prototype.ResizeDesigner = function () {
    if (this.options.maximizeMode)
        this.MinimizeDesigner();
    else
        this.MaximizeDesigner();
}

StiMobileDesigner.prototype.MaximizeDesigner = function () {
    var designer = this.options.mainPanel.parentElement;
    this.options.maximizeMode = true;

    designer.setAttribute("styleHistory", designer.getAttribute("style"));
    this.options.mainPanel.setAttribute("styleHistory", this.options.mainPanel.getAttribute("style"));
    designer.removeAttribute("style");
    this.options.mainPanel.removeAttribute("style");

    designer.style.position = "fixed";
    designer.style.zIndex = "1000000";
    designer.style.top = "0px";
    designer.style.left = "0px";
    designer.style.bottom = "0px";
    designer.style.right = "0px";
}

StiMobileDesigner.prototype.MinimizeDesigner = function () {
    var designer = this.options.mainPanel.parentElement;
    designer.removeAttribute("style");
    this.options.maximizeMode = false;

    designer.setAttribute("style", designer.getAttribute("styleHistory"));
    this.options.mainPanel.setAttribute("style", this.options.mainPanel.getAttribute("styleHistory"));
}

StiMobileDesigner.prototype.ActionExitDesigner = function () {
    if (this.options.haveExitDesignerEvent)
        this.SendCommandExitDesigner();
    else
        history.back();
}

StiMobileDesigner.prototype.GetConnectionNames = function (databaseType, shortName) {
    var databaseConnection = this.loc.Database.Database;
    var connectionTypeNames = {
        "StiMSAccessDatabase": { "name": databaseConnection.replace("{0}", "MS Access"), "shortName": "MS Access" },
        "StiXmlDatabase": { "name": this.loc.Database.DatabaseXml, "shortName": "XML Data" },
        "StiOdbcDatabase": { "name": databaseConnection.replace("{0}", "ODBC"), "shortName": "ODBC" },
        "StiOleDbDatabase": { "name": databaseConnection.replace("{0}", "OLE DB"), "shortName": "OLE DB" },
        "StiSqlDatabase": { "name": databaseConnection.replace("{0}", "MS SQL"), "shortName": "MS SQL" },
        "StiJdbcDatabase": { "name": databaseConnection.replace("{0}", "JDBC"), "shortName": "JDBC" },
        "StiDB2Database": { "name": databaseConnection.replace("{0}", "IBM DB2"), "shortName": "IBM DB2" },
        "StiFirebirdDatabase": { "name": databaseConnection.replace("{0}", "Firebird"), "shortName": "Firebird" },
        "StiInformixDatabase": { "name": databaseConnection.replace("{0}", "Informix"), "shortName": "Informix" },
        "StiMySqlDatabase": { "name": databaseConnection.replace("{0}", "MySql"), "shortName": "MySQL" },
        "StiOracleDatabase": { "name": databaseConnection.replace("{0}", "Oracle"), "shortName": "Oracle" },
        "StiOracleODPDatabase": { "name": databaseConnection.replace("{0}", "Oracle ODP.NET"), "shortName": "Oracle ODP.NET" },
        "StiPostgreSQLDatabase": { "name": databaseConnection.replace("{0}", "PostgreSQL"), "shortName": "PostgreSQL" },
        "StiSqlCeDatabase": { "name": databaseConnection.replace("{0}", "SQLServerCE"), "shortName": "SQL CE" },
        "StiSQLiteDatabase": { "name": databaseConnection.replace("{0}", "SQLite"), "shortName": "SQLite" },
        "StiTeradataDatabase": { "name": databaseConnection.replace("{0}", "Teradata"), "shortName": "Teradata" },
        "StiSybaseAdsDatabase": { "name": databaseConnection.replace("{0}", "SybaseAds"), "shortName": "SybaseAds" },
        "StiSybaseAseDatabase": { "name": databaseConnection.replace("{0}", "SybaseAse"), "shortName": "SybaseAse" },
        "StiUniDirectDatabase": { "name": databaseConnection.replace("{0}", "Uni Direct"), "shortName": "Uni Direct" },
        "StiVistaDBDatabase": { "name": databaseConnection.replace("{0}", "VistaDB"), "shortName": "VistaDB" },
        "StiDotConnectUniversalDatabase": { "name": databaseConnection.replace("{0}", "DotConnectUniversal"), "shortName": "DotConnectUniversal" },
        "StiEffiProzDatabase": { "name": databaseConnection.replace("{0}", "EffiProz"), "shortName": "EffiProz" },
        "StiMongoDBDatabase": { "name": databaseConnection.replace("{0}", "MongoDB"), "shortName": "MongoDB" },
        "StiJsonDatabase": { "name": this.loc.Database.DatabaseJson, "shortName": "JSON Data" },
        "StiODataDatabase": { "name": databaseConnection.replace("{0}", "OData"), "shortName": "OData" },
        "StiGoogleSheetsDatabase": { "name": databaseConnection.replace("{0}", "Google Sheets"), "shortName": "Google Sheets" },
        "StiAzureTableStorageDatabase": { "name": databaseConnection.replace("{0}", "Azure Table Storage"), "shortName": "Azure Table Storage" },
        "StiCosmosDbDatabase": { "name": databaseConnection.replace("{0}", "Cosmos DB"), "shortName": "Cosmos DB" },
        "StiDataWorldDatabase": { "name": databaseConnection.replace("{0}", "Data.World"), "shortName": "Data.World" },
        "StiQuickBooksDatabase": { "name": databaseConnection.replace("{0}", "QuickBooks"), "shortName": "QuickBooks" },
        "StiFirebaseDatabase": { "name": databaseConnection.replace("{0}", "Firebase"), "shortName": "Firebase" },
        "StiGisDatabase": { "name": databaseConnection.replace("{0}", "Gis Data"), "shortName": "Gis Data" },
        "StiBigQueryDatabase": { "name": databaseConnection.replace("{0}", "Big Query"), "shortName": "Big Query" },
        "StiAzureSqlDatabase": { "name": databaseConnection.replace("{0}", "Azure SQL"), "shortName": "Azure SQL" },
        "StiAzureBlobStorageDatabase": { "name": databaseConnection.replace("{0}", "Azure Blob Storage"), "shortName": "Azure Blob Storage" },
        "StiGraphQLDatabase": { "name": databaseConnection.replace("{0}", "GraphQL"), "shortName": "GraphQL" },
    }

    if (!connectionTypeNames[databaseType]) return "";
    return shortName ? connectionTypeNames[databaseType].shortName : connectionTypeNames[databaseType].name;
}

StiMobileDesigner.prototype.GetLocalizedAdapterName = function (dataAdapterType) {
    switch (dataAdapterType) {
        case "StiDataTableAdapterService": return this.loc.Adapters.AdapterDataTables;
        case "StiDB2AdapterService": return this.loc.Adapters.AdapterDB2Connection;
        case "StiFirebirdAdapterService": return this.loc.Adapters.AdapterFirebirdConnection;
        case "StiInformixAdapterService": return this.loc.Adapters.AdapterInformixConnection;
        case "StiOracleAdapterService": return this.loc.Adapters.AdapterOracleConnection;
        case "StiPostgreSQLAdapterService": return this.loc.Adapters.AdapterPostgreSQLConnection;
        case "StiSqlCeAdapterService": return this.loc.Adapters.AdapterSqlCeConnection;
        case "StiSQLiteAdapterService": return this.loc.Adapters.AdapterSQLiteConnection;
        case "StiTeradataAdapterService": return this.loc.Adapters.AdapterTeradataConnection;
        case "StiVistaDBAdapterService": return this.loc.Adapters.AdapterVistaDBConnection;
        case "StiOleDbAdapterService": return this.loc.Adapters.AdapterOleDbConnection;
        case "StiSqlAdapterService": return this.loc.Adapters.AdapterSqlConnection;
        case "StiOdbcAdapterService": return this.loc.Adapters.AdapterOdbcConnection;
        case "StiMSAccessAdapterService": return this.loc.Adapters.AdapterConnection.replace("{0}", "MS Access");
        case "StiDBaseAdapterService": return this.loc.Adapters.AdapterDBaseFiles;
        case "StiMySqlAdapterService": return this.loc.Adapters.AdapterMySQLConnection;
        case "StiCrossTabAdapterService": return this.loc.Adapters.AdapterCrossTabDataSource;
        case "StiCsvAdapterService": return this.loc.Adapters.AdapterCsvFiles;
        case "StiDataViewAdapterService": return this.loc.Adapters.AdapterDataViews;
        case "StiUserAdapterService": return this.loc.Adapters.AdapterUserSources;
        case "StiBusinessObjectAdapterService": return this.loc.Adapters.AdapterBusinessObjects;
        case "StiVirtualAdapterService": return this.loc.Adapters.AdapterVirtualSource;
    }

    return this.loc.Adapters.AdapterConnection.replace("{0}", dataAdapterType.replace("Sti", "").replace("AdapterService", ""));
}

StiMobileDesigner.prototype.DataBaseObjects = [
    { dataBaseType: "StiDB2Database", dataAdapterType: "StiDB2AdapterService", dataSourceType: "StiDB2Source" },
    { dataBaseType: "StiDotConnectUniversalDatabase", dataAdapterType: "StiDotConnectUniversalAdapterService", dataSourceType: "StiDotConnectUniversalSource" },
    { dataBaseType: "StiFirebirdDatabase", dataAdapterType: "StiFirebirdAdapterService", dataSourceType: "StiFirebirdSource" },
    { dataBaseType: "StiInformixDatabase", dataAdapterType: "StiInformixAdapterService", dataSourceType: "StiInformixSource" },
    { dataBaseType: "StiMSAccessDatabase", dataAdapterType: "StiMSAccessAdapterService", dataSourceType: "StiMSAccessSource" },
    { dataBaseType: "StiMySqlDatabase", dataAdapterType: "StiMySqlAdapterService", dataSourceType: "StiMySqlSource" },
    { dataBaseType: "StiJDBCDatabase", dataAdapterType: "StiJdbcAdapterService", dataSourceType: "StiJDBCSource" },
    { dataBaseType: "StiODataDatabase", dataAdapterType: "StiODataAdapterService", dataSourceType: "StiODataSource" },
    { dataBaseType: "StiOdbcDatabase", dataAdapterType: "StiOdbcAdapterService", dataSourceType: "StiOdbcSource" },
    { dataBaseType: "StiOleDbDatabase", dataAdapterType: "StiOleDbAdapterService", dataSourceType: "StiOleDbSource" },
    { dataBaseType: "StiOracleDatabase", dataAdapterType: "StiOracleAdapterService", dataSourceType: "StiOracleSource" },
    { dataBaseType: "StiPostgreSQLDatabase", dataAdapterType: "StiPostgreSQLAdapterService", dataSourceType: "StiPostgreSQLSource" },
    { dataBaseType: "StiSqlCeDatabase", dataAdapterType: "StiSqlCeAdapterService", dataSourceType: "StiSqlCeSource" },
    { dataBaseType: "StiSqlDatabase", dataAdapterType: "StiSqlAdapterService", dataSourceType: "StiSqlSource" },
    { dataBaseType: "StiSQLiteDatabase", dataAdapterType: "StiSQLiteAdapterService", dataSourceType: "StiSQLiteSource" },
    { dataBaseType: "StiSybaseDatabase", dataAdapterType: "StiSybaseAdapterService", dataSourceType: "StiSybaseSource" },
    { dataBaseType: "StiTeradataDatabase", dataAdapterType: "StiTeradataAdapterService", dataSourceType: "StiTeradataSource" },
    { dataBaseType: "StiVistaDBDatabase", dataAdapterType: "StiVistaDBAdapterService", dataSourceType: "StiVistaDBSource" },
    { dataBaseType: "StiCsvDatabase", dataAdapterType: "StiDataTableAdapterService", dataSourceType: "StiDataTableSource" },
    { dataBaseType: "StiGisDatabase", dataAdapterType: "StiDataTableAdapterService", dataSourceType: "StiDataTableSource" },
    { dataBaseType: "StiDBaseDatabase", dataAdapterType: "StiDataTableAdapterService", dataSourceType: "StiDataTableSource" },
    { dataBaseType: "StiExcelDatabase", dataAdapterType: "StiDataTableAdapterService", dataSourceType: "StiDataTableSource" },
    { dataBaseType: "StiJsonDatabase", dataAdapterType: "StiDataTableAdapterService", dataSourceType: "StiDataTableSource" },
    { dataBaseType: "StiJsonDatabase", dataAdapterType: "StiDataTableAdapterService", dataSourceType: "StiDataTableSource" },
    { dataBaseType: "StiMongoDbDatabase", dataAdapterType: "StiMongoDbAdapterService", dataSourceType: "StiMongoDbSource" },
    { dataBaseType: "StiAzureTableStorageDatabase", dataAdapterType: "StiAzureTableStorageAdapterService", dataSourceType: "StiAzureTableStorageSource" },
    { dataBaseType: "StiGoogleSheetsDatabase", dataAdapterType: "StiGoogleSheetsAdapterService", dataSourceType: "StiGoogleSheetsSource" },
    { dataBaseType: "StiCosmosDbDatabase", dataAdapterType: "StiCosmosDbAdapterService", dataSourceType: "StiCosmosDbSource" },
    { dataBaseType: "StiDataWorldDatabase", dataAdapterType: "StiDataWorldAdapterService", dataSourceType: "StiDataWorldSource" },
    { dataBaseType: "StiQuickBooksDatabase", dataAdapterType: "StiQuickBooksAdapterService", dataSourceType: "StiQuickBooksSource" },
    { dataBaseType: "StiFirebaseDatabase", dataAdapterType: "StiFirebaseAdapterService", dataSourceType: "StiFirebaseSource" },
    { dataBaseType: "StiBigQueryDatabase", dataAdapterType: "StiBigQueryAdapterService", dataSourceType: "StiBigQuerySource" },
    { dataBaseType: "StiAzureSqlDatabase", dataAdapterType: "StiSqlAdapterService", dataSourceType: "StiSqlSource" },
    { dataBaseType: "StiAzureBlobStorageDatabase", dataAdapterType: "StiAzureBlobStorageAdapterService", dataSourceType: "StiAzureBlobStorageSource" },
    { dataBaseType: "StiGraphQLDatabase", dataAdapterType: "StiGraphQLAdapterService", dataSourceType: "StiGraphQLSource" },
    { dataBaseType: null, dataAdapterType: "StiBusinessObjectAdapterService", dataSourceType: "StiBusinessObjectSource" },
    { dataBaseType: null, dataAdapterType: "StiVirtualAdapterService", dataSourceType: "StiVirtualSource" },
    { dataBaseType: null, dataAdapterType: "StiCrossTabAdapterService", dataSourceType: "StiCrossTabDataSource" },
    { dataBaseType: "StiCustomDatabase", dataAdapterType: "StiCustomAdapterService", dataSourceType: "StiCustomSource" } // For Js Only
]

StiMobileDesigner.prototype.GetDataSourceTypeFromDataAdapterType = function (dataAdapterType) {
    for (var i = 0; i < this.DataBaseObjects.length; i++) {
        if (this.DataBaseObjects[i].dataAdapterType == dataAdapterType) return this.DataBaseObjects[i].dataSourceType;
    }
    return "StiUndefinedDataSource";
}

StiMobileDesigner.prototype.GetDataAdapterTypeFromDatabaseType = function (dataBaseType) {
    for (var i = 0; i < this.DataBaseObjects.length; i++) {
        if (this.DataBaseObjects[i].dataBaseType == dataBaseType) return this.DataBaseObjects[i].dataAdapterType;
    }
    return null;
}

StiMobileDesigner.prototype.CopyObject = function (o) {
    if (!o || "object" !== typeof o) {
        return o;
    }
    var c = "function" === typeof o.pop ? [] : {};
    var p, v;
    for (p in o) {
        // eslint-disable-next-line no-prototype-builtins
        if (o.hasOwnProperty(p)) {
            v = o[p];
            if (v && "object" === typeof v) {
                c[p] = this.CopyObject(v);
            }
            else c[p] = v;
        }
    }
    return c;
}

StiMobileDesigner.prototype.ClearStyles = function (object) {
    object.className = "stiDesignerClearAllStyles";
}

StiMobileDesigner.prototype.RepaintControlByAttributes = function (control, font, brush, textBrush, border) {
    //Font
    if (font) {
        var fontAttr = font.split("!");
        control.style.fontFamily = fontAttr[0];
        control.style.fontSize = fontAttr[1] + "pt";
        control.style.fontWeight = (fontAttr[2] == "1") ? "bold" : "";
        control.style.fontStyle = (fontAttr[3] == "1") ? "italic" : "";
        control.style.textDecoration = (fontAttr[4] == "1") ? "underline" : "";
    }

    //Brush
    if (brush) {
        var brushColor = this.GetColorFromBrushStr(brush);
        control.style.background = this.GetHTMLColor(brushColor);
    }

    //TextBrush
    if (textBrush) {
        var textBrushColor = this.GetColorFromBrushStr(textBrush);
        control.style.color = this.GetHTMLColor(textBrushColor);
    }

    //Border    
    control.style.border = "1px dashed #b4b4b4";
    if (border && border != "default") {
        var borderStyles = ["solid", "dashed", "dashed", "dashed", "dotted", "double", "none"];
        var borderAttr = border.split("!");
        var borderSides = borderAttr[0].split(",");
        var borderColor = this.GetHTMLColor(borderAttr[2]);
        var borderStyle = borderStyles[borderAttr[3]];
        var borderSize = borderAttr[1];

        if (borderSides[0] == "1") control.style.borderLeft = borderSize + "px " + borderStyle + " " + borderColor;
        if (borderSides[1] == "1") control.style.borderTop = borderSize + "px " + borderStyle + " " + borderColor;
        if (borderSides[2] == "1") control.style.borderRight = borderSize + "px " + borderStyle + " " + borderColor;
        if (borderSides[3] == "1") control.style.borderBottom = borderSize + "px " + borderStyle + " " + borderColor;
    }
}

StiMobileDesigner.prototype.FilterDataToShortString = function (filterData) {
    return (this.GetCountObjects(filterData) != 0)
        ? "[" + this.loc.PropertyMain.Filters + "]"
        : "[" + this.loc.FormBand.NoFilters + "]";
}

StiMobileDesigner.prototype.SortDataToShortString = function (sortData) {
    if (sortData == "")
        return "[" + this.loc.FormBand.NoSort + "]";
    else {
        var sorts = JSON.parse(StiBase64.decode(sortData));
        return this.SortArrayToSortStr(sorts);
    }
}

StiMobileDesigner.prototype.GetThemeColor = function () {
    var themeColor = this.options.theme;
    themeColor = themeColor.replace("Office2013White", "").replace("Office2013LightGray", "").replace("Office2013VeryDarkGray", "").replace("Office2013DarkGray", "");
    themeColor = themeColor.replace("Office2022White", "").replace("Office2022LightGray", "").replace("Office2022DarkGray", "").replace("Office2022Black", "");

    return (themeColor || "Blue");
}

StiMobileDesigner.prototype.SortByName = function (a, b) {
    if (a.name && b.name) {
        if (a.name < b.name) return -1;
        if (a.name > b.name) return 1;
    }
    return 0
}

StiMobileDesigner.prototype.SortByNameDescending = function (a, b) {
    if (a.name && b.name) {
        if (a.name > b.name) return -1;
        if (a.name < b.name) return 1;
    }
    return 0
}

StiMobileDesigner.prototype.SortByIndex = function (a, b) {
    if (a.index != null && b.index != null) {
        if (a.index < b.index) return -1;
        if (a.index > b.index) return 1;
    }
    return 0
}

StiMobileDesigner.prototype.SortByCaption = function (a, b) {
    if (a.caption && b.caption) {
        if (a.caption < b.caption) return -1;
        if (a.caption > b.caption) return 1;
    }
    return 0
}

StiMobileDesigner.prototype.SortByDisplayValue = function (a, b) {
    if (a.displayValue && b.displayValue) {
        if (a.displayValue < b.displayValue) return -1;
        if (a.displayValue > b.displayValue) return 1;
    }
    return 0
}

StiMobileDesigner.prototype.SortByLeft = function (a, b) {
    if (a.properties && a.properties.unitLeft != null && b.properties && b.properties.unitLeft != null) {
        var aLeft = parseFloat(a.properties.unitLeft);
        var bLeft = parseFloat(b.properties.unitLeft);
        if (aLeft < bLeft) return -1;
        if (aLeft > bLeft) return 1;
    }
    return 0
}

StiMobileDesigner.prototype.GetNavigatorName = function () {
    var useragent = navigator.userAgent;
    var navigatorname = "Unknown";
    if (this.GetIEVersion() > 0) {
        navigatorname = "MSIE";
    }
    else if (useragent.indexOf('Gecko') != -1) {
        if (useragent.indexOf('Chrome') != -1)
            navigatorname = "Google Chrome";
        else navigatorname = "Mozilla";
    }
    else if (useragent.indexOf('Mozilla') != -1) {
        navigatorname = "old Netscape or Mozilla";
    }
    else if (useragent.indexOf('Opera') != -1) {
        navigatorname = "Opera";
    }

    return navigatorname;
}

StiMobileDesigner.prototype.GetOSName = function () {
    var appVersion = navigator ? navigator.appVersion : null;

    if (!appVersion) return null;
    if (appVersion.indexOf("Win") != -1) return "Windows";
    if (appVersion.indexOf("Mac") != -1) return "MacOS";
    if (appVersion.indexOf("X11") != -1) return "UNIX";
    if (appVersion.indexOf("Linux") != -1) return "Linux";

    return null;
}

StiMobileDesigner.prototype.checkWin11 = function () {
    return false;
}

StiMobileDesigner.prototype.GetIEVersion = function () {
    var sAgent = window.navigator.userAgent;
    var Idx = sAgent.indexOf("MSIE");

    // If IE, return version number.
    if (Idx > 0)
        return parseInt(sAgent.substring(Idx + 5, sAgent.indexOf(".", Idx)));

    // If IE 11 then look for Updated user agent string.
    // eslint-disable-next-line no-extra-boolean-cast
    else if (!!navigator.userAgent.match(/Trident\/7\./))
        return 11;

    else
        return 0; //It is not IE
}

StiMobileDesigner.prototype.IsContains = function (array, item) {
    if (!array) return false;
    for (var index in array)
        if (item == array[index]) return true;

    return false;
}

StiMobileDesigner.prototype.GetSystemVariableDescription = function (name) {
    var text = "<b>" + name + "</b><br><br>";
    text += this.loc.SystemVariables[name];

    return text;
}

StiMobileDesigner.prototype.GetFunctionDescription = function (itemObject) {
    var text = "<b>" + itemObject.descriptionHeader + "</b><br/><br/>";
    text += itemObject.description + "<br/><br/>";

    if (itemObject.argumentNames && itemObject.argumentNames.length > 0 &&
        itemObject.argumentDescriptions && itemObject.argumentDescriptions.length > 0 &&
        itemObject.argumentNames.length == itemObject.argumentDescriptions.length) {
        text += "<b>" + this.loc.PropertyMain.Parameters + "</b><br/>";
        for (var i = 0; i < itemObject.argumentNames.length; i++) {
            text += itemObject.argumentNames[i] + " - " + itemObject.argumentDescriptions[i] + "<br/>";
        }
        text += "<br/>";
    }

    text += "<b>" + this.loc.PropertyMain.ReturnValue + "</b><br/>";
    text += itemObject.returnDescription + "<br/><br/>";

    return text;
}

StiMobileDesigner.prototype.EndsWith = function (str, suffix) {
    return str.indexOf(suffix, str.length - suffix.length) !== -1;
};

StiMobileDesigner.prototype.ShowReportInTheViewer = function (reportName) {
    var params = {
        sessionKey: this.options.cloudParameters.sessionKey,
        reportName: reportName,
        attachedItems: this.options.report.getAttachedItems()
    };
    this.SendCommandCloneItemResourceSave(params);
};

StiMobileDesigner.prototype.UpperFirstChar = function (text) {
    return text.charAt(0).toUpperCase() + text.substr(1);
};

StiMobileDesigner.prototype.LowerFirstChar = function (text) {
    return text.charAt(0).toLowerCase() + text.substr(1);
};

StiMobileDesigner.prototype.IsBandComponent = function (component) {
    return ((component.typeComponent.indexOf("Band") != -1 && component.typeComponent.indexOf("Cross") == -1) || component.typeComponent == "StiTable");
};

StiMobileDesigner.prototype.IsCrossBandComponent = function (component) {
    return (component.typeComponent.indexOf("Cross") != -1 && component.typeComponent != "StiCrossTab" && component.typeComponent != "StiCrossField");
};

StiMobileDesigner.prototype.addEvent = function (element, eventName, fn) {
    if (!this.designerEvents) this.designerEvents = [];
    if (element) {
        if (element.addEventListener) element.addEventListener(eventName, fn, false);
        else if (element.attachEvent) element.attachEvent('on' + eventName, fn);
        else element["on" + eventName] = fn;

        this.designerEvents.push({
            element: element,
            eventName: eventName,
            fn: fn
        });
    }
}

StiMobileDesigner.prototype.removeAllEvents = function () {
    if (this.designerEvents) {
        for (var i = 0; i < this.designerEvents.length; i++) {
            var evnt = this.designerEvents[i];
            var element = evnt.element;
            var eventName = evnt.eventName;
            var fn = evnt.fn;
            if (element.removeEventListener) element.removeEventListener(eventName, fn, false);
            else if (element.detachEvent) element.detachEvent('on' + eventName, fn);
            else element["on" + eventName] = null;
        }
        this.designerEvents = [];
    }
}

StiMobileDesigner.prototype.Is_array = function (variable) {
    return (typeof variable == "object") && (variable instanceof Array);
}

StiMobileDesigner.prototype.RemoveElementFromArray = function (array, element) {
    for (var i = 0; i < array.length; i++) {
        if (element == array[i]) {
            array.splice(array.indexOf(element), 1);
        }
    }
}

StiMobileDesigner.prototype.GetCommonObject = function (objects) {
    if (!objects || objects.length == 0) return null;

    var commonObject = {};
    commonObject.jsObject = this;
    commonObject.properties = {};
    commonObject.typeComponent = null;
    commonObject.isDashboardElement = true;

    for (var i = 0; i < objects.length; i++) {
        for (var propertyName in objects[i].properties) {
            var typeComponent = objects[i].typeComponent;
            if (propertyName in commonObject.properties) continue;
            var propertyValue = objects[i].properties[propertyName];
            var sameValue = true;
            var sameTypeComponent = true;
            for (var k = 0; k < objects.length; k++) {
                var haveThisProperty = true;
                if (!(propertyName in objects[k].properties)) {
                    haveThisProperty = false;
                    break;
                }
                if (propertyValue != objects[k].properties[propertyName]) sameValue = false;
                if (typeComponent != objects[k].typeComponent) sameTypeComponent = false;
            }
            if (haveThisProperty) {
                commonObject.properties[propertyName] = sameValue ? propertyValue : "StiEmptyValue";
                if (propertyName == "font") commonObject.properties[propertyName] = this.GetCommonPropertyValue(objects, "font", "!");
                if (propertyName == "border") {
                    var borderSides = [];
                    for (var j = 0; j < objects.length; j++) {
                        if (!objects[j].properties.border) continue;
                        var borderSidesValue = objects[j].properties.border.split("!");
                        borderSides.push({ properties: { borderSides: borderSidesValue[0] } })
                    }
                    var borderSidesCommonValue = this.GetCommonPropertyValue(borderSides, "borderSides", ",");
                    var borderCommonValue = this.GetCommonPropertyValue(objects, "border", "!");
                    borderCommonValue = borderSidesCommonValue + borderCommonValue.substring(borderCommonValue.indexOf("!"));
                    commonObject.properties[propertyName] = borderCommonValue;
                }
                if (propertyName == "textFormat") {
                    var commonTextFormatType = "StiEmptyValue";
                    var textFormatValue = null;

                    for (var j = 0; j < objects.length; j++) {
                        if (commonTextFormatType != "StiEmptyValue" && commonTextFormatType != objects[j].properties.textFormat.type) {
                            commonTextFormatType = "StiEmptyValue";
                            break;
                        }
                        commonTextFormatType = objects[j].properties.textFormat.type;
                        if (objects[j].properties.textFormat) textFormatValue = objects[j].properties.textFormat;
                    }
                    if (commonTextFormatType == "StiEmptyValue") {
                        commonObject.properties.textFormat = { type: commonTextFormatType };
                    }
                    else {
                        commonObject.properties.textFormat = textFormatValue;
                    }
                }
                commonObject.typeComponent = sameTypeComponent ? typeComponent : null;
            }
        }

        if (!objects[i].isDashboardElement) commonObject.isDashboardElement = null;
    }

    return commonObject;
}

StiMobileDesigner.prototype.GetCommonPropertyValue = function (objects, propertyName, separator) {
    var commonArray = null;

    for (var i = 0; i < objects.length; i++) {
        if (!objects[i].properties[propertyName]) continue;
        var propertyArray = objects[i].properties[propertyName].split(separator);
        if (!commonArray) commonArray = this.CopyObject(propertyArray);
        for (var k = 0; k < propertyArray.length; k++) {
            if (commonArray[k] != propertyArray[k]) commonArray[k] = "StiEmptyValue";
        }
    }

    var result = "";
    for (var i = 0; i < commonArray.length; i++) {
        if (i != 0) result += separator;
        result += commonArray[i];
    }

    return result;
}

StiMobileDesigner.prototype.GetCommonPositionsArray = function (objects) {
    var commonArray = null;

    for (var i = 0; i < objects.length; i++) {
        if (!ComponentCollection[objects[i].typeComponent]) continue;
        var positionsArray = ComponentCollection[objects[i].typeComponent][5].split(",");
        if (!commonArray) commonArray = this.CopyObject(positionsArray);
        for (var k = 0; k < positionsArray.length; k++) {
            if (commonArray[k] != positionsArray[k]) commonArray[k] = "0";
        }
    }

    return commonArray;
}

StiMobileDesigner.prototype.ApplyPropertyValue = function (propertyName, propertyValue, updateAllProperties) {
    var selectedObjects = this.options.selectedObjects || [this.options.selectedObject];
    if (!selectedObjects) return;
    var propertyNames = this.Is_array(propertyName) ? propertyName : [propertyName];
    var propertyValues = this.Is_array(propertyValue) ? propertyValue : [propertyValue];

    if (propertyValues.length == propertyNames.length) {
        for (var i = 0; i < selectedObjects.length; i++)
            for (var k = 0; k < propertyNames.length; k++)
                selectedObjects[i].properties[propertyNames[k]] = propertyValues[k];
        this.SendCommandSendProperties(selectedObjects, propertyNames, updateAllProperties);
    }
}

StiMobileDesigner.prototype.ApplyPropertyExpressionBoolValue = function (propertyName, propertyValue) {
    var selectedObjects = this.options.selectedObjects || [this.options.selectedObject];
    if (selectedObjects) {
        for (var i = 0; i < selectedObjects.length; i++) {
            if (propertyValue == "True" || propertyValue == "False") {
                selectedObjects[i].properties[propertyName] = propertyValue == "True";
                if (selectedObjects[i].properties.expressions && selectedObjects[i].properties.expressions[propertyName] != null)
                    delete selectedObjects[i].properties.expressions[propertyName]
            }
            else if (selectedObjects[i].properties.expressions) {
                selectedObjects[i].properties.expressions[propertyName] = StiBase64.encode(propertyValue);
            }
        }
        this.SendCommandSendProperties(selectedObjects, [propertyName, "expressions"]);
    }
}

StiMobileDesigner.prototype.ApplyPropertyExpressionControlValue = function (propertyName, propertyValue, expression, originalPropertyName, updateAllProperties) {
    var selectedObjects = this.options.selectedObjects || [this.options.selectedObject];
    if (selectedObjects) {
        for (var i = 0; i < selectedObjects.length; i++) {
            if (!expression) {
                selectedObjects[i].properties[propertyName] = propertyValue;
                if (selectedObjects[i].properties.expressions && selectedObjects[i].properties.expressions[propertyName] != null)
                    delete selectedObjects[i].properties.expressions[propertyName]
            }
            else if (selectedObjects[i].properties.expressions) {
                selectedObjects[i].properties.expressions[originalPropertyName || propertyName] = StiBase64.encode(expression);
            }
        }
        this.SendCommandSendProperties(selectedObjects, [propertyName, "expressions"], updateAllProperties);
    }
}

StiMobileDesigner.prototype.ConcatColumns = function (columns1, columns2) {
    var commonColumns = this.CopyObject(columns1);
    for (var i = 0; i < columns2.length; i++) {
        var addColumn = true;
        for (var k = 0; k < columns1.length; k++) {
            if (columns1[k].name == columns2[i].name && columns1[k].nameInSource == columns2[i].nameInSource && columns1[k].alias == columns2[i].alias &&
                columns1[k].isCalcColumn == columns2[i].isCalcColumn && columns1[k].type == columns2[i].type) {
                addColumn = false;
            }
        }
        if (addColumn) commonColumns.push(columns2[i]);
    }

    return commonColumns;
}

StiMobileDesigner.prototype.SaveCurrentStylePropertiesToObject = function (object) {
    if (!object) object = {};
    var selectedObject = this.options.selectedObjects ? this.options.selectedObjects[0] : this.options.selectedObject;
    if (selectedObject) {
        var styleProps = this.GetStylePropertiesFromComponent(selectedObject);
        for (var i = 0; i < styleProps.length; i++) {
            object[styleProps[i].name] = styleProps[i].value;
        }
    }
}

StiMobileDesigner.prototype.SaveLastStyleProperties = function (component) {
    if (component && !component.isDashboard && !this.IsBandComponent(component) && component.typeComponent != "StiPage" && component.typeComponent != "StiReport") {
        this.options.lastStyleProperties = this.GetStylePropertiesFromComponent(component, true);
    }
}

StiMobileDesigner.prototype.GetStylePropertiesFromComponent = function (component, ignoreComponentStyle) {
    var properties = [];
    if (component) {
        var propertyNames = ["border", "brush", "backColor", "textBrush", "font", "horAlignment", "vertAlignment", "chartStyle"];

        if (!ignoreComponentStyle) {
            if (component.isDashboardElement) {
                propertyNames.push("elementStyle");
                propertyNames.push("customStyleName");
            }
            else
                propertyNames.push("componentStyle");
        }

        for (var i = 0; i < propertyNames.length; i++) {
            if (component.properties[propertyNames[i]] != null && component.properties[propertyNames[i]] != "StiEmptyValue") {
                properties.push({
                    name: propertyNames[i],
                    value: component.properties[propertyNames[i]]
                });
                if (propertyNames[i] == "brush" && (!component.properties.backColor || component.properties.backColor == "StiEmptyValue")) {
                    properties.push({
                        name: "backColor",
                        value: this.GetColorFromBrushStr(component.properties.brush)
                    });
                }
                if (propertyNames[i] == "backColor" && (!component.properties.brush || component.properties.brush == "StiEmptyValue")) {
                    properties.push({
                        name: "brush",
                        value: "1!" + component.properties.backColor
                    });
                }
            }
        }
    }
    return properties;
}

StiMobileDesigner.prototype.SetStylePropertiesToComponent = function (component, styleProperties) {
    if (styleProperties) {
        var propertyNames = [];
        for (var propertyName in styleProperties) {
            if (component.properties[propertyName] != null) {
                component.properties[propertyName] = styleProperties[propertyName];
                propertyNames.push(propertyName)
            }
        }
        this.SendCommandSendProperties(component, propertyNames, true);
    };
}

StiMobileDesigner.prototype.SetCookie = function (name, value, path, domain, secure, expires) {
    if (this.options.standaloneJsMode && this["LoadCookies"] && this["SaveCookies"]) {
        //save cookies to file for standalone version
        var cookies = this.LoadCookies();
        if (value != null) cookies[name] = value.toString();
        this.SaveCookies(cookies);
    }

    if (this.options.standaloneJsMode || typeof localStorage == "undefined" || name.indexOf("sti_") == 0 || name.indexOf("login") == 0) {
        //save to cookies
        if (value && typeof (value) == "string" && value.length >= 4096) return;
        var pathName = location.pathname;
        var expDate = new Date();
        expDate.setTime(expDate.getTime() + (365 * 24 * 3600 * 1000));
        document.cookie = name + "=" + escape(value) +
            "; samesite=strict;  expires=" + (expires || expDate.toGMTString()) +
            ((path) ? "; path=" + path : "; path=/") +
            ((domain) ? "; domain=" + domain : "") +
            ((secure) ? "; secure" : "");
    }
    else {
        //save to localstorage
        localStorage.setItem(name, value);
    }
}

StiMobileDesigner.prototype.GetCookie = function (name) {
    var getCookie_ = function (name) {
        var cookie = " " + document.cookie;
        var search = " " + name + "=";
        var setStr = null;
        var offset = 0;
        var end = 0;
        if (cookie.length > 0) {
            offset = cookie.indexOf(search);
            if (offset != -1) {
                offset += search.length;
                end = cookie.indexOf(";", offset);
                if (end == -1) {
                    end = cookie.length;
                }
                setStr = unescape(cookie.substring(offset, end));
            }
        }
        return setStr;
    }

    if (this.options.standaloneJsMode && this["LoadCookies"]) {
        //load cookies from file for standalone version
        var cookies = this.LoadCookies();
        if (cookies[name] != null)
            return cookies[name];
    }

    if (this.options.standaloneJsMode || typeof localStorage == "undefined" || name.indexOf("sti_") == 0 || name.indexOf("login") == 0) {
        return getCookie_(name);
    }

    var value = localStorage.getItem(name);
    if (value != null) {
        return value;
    }
    else {
        value = getCookie_(name);
        if (value != null) {
            this.RemoveCookie(name);
            localStorage.setItem(name, value);
        }
        return value;
    }
}

StiMobileDesigner.prototype.RemoveCookie = function (name) {
    document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/";
}

StiMobileDesigner.prototype.AddProgressToControl = function (control) {
    if (!control) return;
    var progress = this.Progress();
    progress.style.display = "none";
    control.appendChild(progress);
    control.progress = progress;
    progress.owner = control;

    return progress;
}

StiMobileDesigner.prototype.Progress = function () {
    var progress = document.createElement("div");
    progress.style.position = "absolute";
    progress.style.zIndex = "1000";
    progress.jsObject = this;

    var progressImage = document.createElement("div");
    progress.appendChild(progressImage);
    progressImage.className = "mobile_designer_loader";

    var progressText = document.createElement("div");
    progressText.className = "stiProgressText";
    progress.appendChild(progressText);
    progress.progressText = progressText;

    var buttonCancel = this.FormButton(null, null, this.loc.Buttons.Cancel.replace("&", ""), null);
    buttonCancel.style.position = "absolute";
    buttonCancel.style.display = "none";
    buttonCancel.style.top = "145px";
    buttonCancel.style.border = "1px solid #c6c6c6";
    buttonCancel.style.left = "calc(50% - 40px)";
    buttonCancel.style.height = "20px";
    progress.appendChild(buttonCancel);
    progress.buttonCancel = buttonCancel;

    progress.showCancelButton = function () {
        this.cancelTimer = setTimeout(function () {
            buttonCancel.style.display = "";
            buttonCancel.style.opacity = 1 / 100;
            var d = new Date();
            var endTime = d.getTime() + 300;
            progress.jsObject.ShowAnimationForm(buttonCancel, endTime);
        }, 3000);
    }

    progress.hideCancelButton = function () {
        buttonCancel.style.display = "none";
        clearTimeout(buttonCancel.animationTimer);
        clearTimeout(progress.cancelTimer);
    }

    progress.show = function (left, top, progressValue, commandGuid) {
        this.commandGuid = commandGuid;
        this.style.display = "";
        this.style.left = (left || (this.owner.offsetWidth / 2 - this.offsetWidth / 2)) + "px";
        this.style.top = (top || (this.owner.offsetHeight / 2 - this.offsetHeight / 2)) + "px";

        if (progressValue)
            this.progressText.innerHTML = typeof progressValue == "string" ? progressValue : (parseInt(parseFloat(progressValue) * 100) + "%");
        else
            this.progressText.innerHTML = "";
    }

    progress.hide = function () {
        this.style.display = "none";
        this.hideCancelButton();
    }

    return progress;
}

StiMobileDesigner.prototype.ProgressMini = function (color) {
    var progress = document.createElement("div");
    progress.style.width = "18px";
    progress.style.height = "18px";
    progress.style.overflow = "hidden";

    var progressImage = document.createElement("div");
    progress.appendChild(progressImage);

    progressImage.className = color == "white" ? "mobile_designer_loader_mini_white" : "mobile_designer_loader_mini_color";

    return progress;
}

StiMobileDesigner.prototype.HatchData = [
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

StiMobileDesigner.prototype.HexToByteString = function (hex) {
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

StiMobileDesigner.prototype.GetSvgHatchBrush = function (brushProps, x, y, width, height) {
    var brushSvg = this.CreateSvgElement("svg");
    var brushId = this.newGuid();
    var foreColor = brushProps[1];
    var backColor = brushProps[2];
    var hatchNumber = this.StrToInt(brushProps[3]);
    if (hatchNumber > 53) hatchNumber = 53;

    this.AddHatchBrushPatternToElement(brushSvg, brushId, hatchNumber, foreColor, backColor);

    var rect = this.CreateSvgElement("rect");
    brushSvg.rect = rect;
    rect.setAttribute("x", x || 0);
    rect.setAttribute("y", y || 0);
    rect.setAttribute("width", width || "100%");
    rect.setAttribute("height", height || "100%");
    rect.setAttribute("fill", "url(#" + brushId + ")");
    brushSvg.appendChild(rect);

    return brushSvg;
}

StiMobileDesigner.prototype.AddHatchBrushPatternToElement = function (element, patternId, hatchNumber, foreColor, backColor) {
    var brushPattern = this.CreateSvgElement("pattern");
    element.appendChild(brushPattern);

    brushPattern.setAttribute("id", patternId);
    brushPattern.setAttribute("x", "0");
    brushPattern.setAttribute("y", "0");
    brushPattern.setAttribute("width", "8");
    brushPattern.setAttribute("height", "8");
    brushPattern.setAttribute("patternUnits", "userSpaceOnUse");

    var sb = "";
    var hatchHex = this.HatchData[hatchNumber];

    for (var index = 0; index < 16; index++) {
        sb += this.HexToByteString(hatchHex.charAt(index));
    }

    var brushRect = this.CreateSvgElement("rect");
    brushPattern.appendChild(brushRect);
    brushRect.setAttribute("x", "0");
    brushRect.setAttribute("y", "0");
    brushRect.setAttribute("width", "8");
    brushRect.setAttribute("height", "8");
    brushRect.setAttribute("fill", this.GetHTMLColor(backColor));


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
                brushRect2.setAttribute("fill", this.GetHTMLColor(foreColor));

            }
        }
    }
}

StiMobileDesigner.prototype.GetTextFormatLocalizedName = function (type, notLocalize) {
    if (notLocalize) {
        return (type ? type.replace("Sti", "").replace("FormatService", "") : type);
    }

    switch (type) {
        case "StiGeneralFormatService": return this.loc.FormFormatEditor.General;
        case "StiNumberFormatService": return this.loc.FormFormatEditor.Number;
        case "StiCurrencyFormatService": return this.loc.FormFormatEditor.Currency;
        case "StiDateFormatService": return this.loc.FormFormatEditor.Date;
        case "StiTimeFormatService": return this.loc.FormFormatEditor.Time;
        case "StiPercentageFormatService": return this.loc.FormFormatEditor.Percentage;
        case "StiBooleanFormatService": return this.loc.FormFormatEditor.Boolean;
        case "StiCustomFormatService": return this.loc.FormFormatEditor.Custom;
    }

    return "";
}

StiMobileDesigner.prototype.GetUserCrossTabStyles = function () {
    var userStyles = [];

    if (this.options.report) {
        for (var i = 0; i < this.options.report.stylesCollection.length; i++) {
            if (this.options.report.stylesCollection[i].type == "StiCrossTabStyle")
                userStyles.push(this.options.report.stylesCollection[i]);
        }
    }

    return userStyles;
}

StiMobileDesigner.prototype.IsTableCell = function (component) {
    if (!component) return false;

    if (this.Is_array(component)) {
        for (var i = 0; i < component.length; i++) {
            if (!component[i].typeComponent) return false;
            else if (component[i].typeComponent.indexOf("StiTableCell") != 0) return false;
        }
        return true;
    }
    else if (component.typeComponent && component.typeComponent.indexOf("StiTableCell") == 0) return true;

    return false;
}

StiMobileDesigner.prototype.GetControlValue = function (control) {
    if (control["setKey"] != null) return control.key;
    else if (control["setChecked"] != null) return control.isChecked;
    else if (control["getValue"] != null) return control.getValue();
    else if (control["value"] != null) return control.value;
    else if (control["textBox"] != null && control.textBox["value"] != null) return control.textBox.value;
    else if (control["textArea"] != null && control.textArea["value"] != null) return control.textArea.value;
    else if (control["setImage"] != null) return control.src;
    return null;
}

StiMobileDesigner.prototype.SetControlValue = function (control, value) {
    if (control["setKey"] != null) control.setKey(value);
    else if (control["setChecked"] != null) return control.setChecked(value);
    else if (control["setValue"] != null) control.setValue(value);
    else if (control["value"] != null) return control.value = value;
    else if (control["textBox"] != null && control.textBox["value"] != null) control.textBox.value = value;
    else if (control["textArea"] != null && control.textArea["value"] != null) control.textArea.value = value;
    else if (control["setImage"] != null) return control.setImage(value);
}

StiMobileDesigner.prototype.HaveTableCell = function (components) {
    if (!components) return false;
    for (var i = 0; i < components.length; i++) {
        if (components[i].typeComponent.indexOf("StiTableCell") == 0) return true;
    }
    return false;
}

StiMobileDesigner.prototype.RebuildTable = function (table, cells, removeOldCells) {
    var page = this.options.report.pages[table.properties.pageName];

    //Remove old cells
    if (removeOldCells) {
        if (table.properties.childs) {
            var childNames = table.properties.childs.split(",");
            for (var indexChild = 0; indexChild < childNames.length; indexChild++) {
                var child = page.components[childNames[indexChild]];
                if (child) {
                    page.removeChild(child);
                    delete page.components[childNames[indexChild]];
                }
            }
        }
    }
    //Add or change new cells
    for (var i = 0; i < cells.length; i++) {
        var compObject = cells[i];

        var component = page.components[compObject.name];
        if (component) {
            this.CreateComponentProperties(component, compObject);
            if (ComponentCollection[component.typeComponent][3] != "none") this.CreateComponentNameContent(component);
        }
        else {
            component = this.CreateComponent(compObject);
            page.appendChild(component);
            page.components[compObject.name] = component;
        }
        component.repaint();
    }
}

StiMobileDesigner.prototype.CompareNumbers = function (num1, num2, accuracy) {
    return (Math.abs(num1 - num2) <= accuracy);
}

StiMobileDesigner.prototype.GetAllResizingCells = function (component, compResizingType, compStartValues) {
    var page = this.options.report.pages[component.properties.pageName];
    var cells = [];
    if (page) {
        var accuracy = 3;
        var table = component.typeComponent == "StiTable" ? component : page.components[component.properties.parentName];
        if (table && table.typeComponent == "StiTable") {
            var cellNames = table.properties.childs.split(",");
            for (var indexCell = 0; indexCell < cellNames.length; indexCell++) {
                var cell = page.components[cellNames[indexCell]];
                if (cell) {
                    var startValues = {};
                    startValues.height = parseInt(cell.getAttribute("height"));
                    startValues.width = parseInt(cell.getAttribute("width"));
                    startValues.left = parseInt(cell.getAttribute("left"));
                    startValues.top = parseInt(cell.getAttribute("top"));
                    cell.startValues = startValues;
                    if (cell == component) continue;
                    if (component.typeComponent == "StiTable") {
                        cells.push(cell);
                        continue;
                    }

                    switch (compResizingType) {
                        case "LeftTop":
                            {
                                if (this.CompareNumbers(compStartValues.left, startValues.left + startValues.width, accuracy) &&
                                    this.CompareNumbers(compStartValues.top, startValues.top, accuracy)) {
                                    cell.resizingType = "RightTop";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.left, startValues.left + startValues.width, accuracy) &&
                                    this.CompareNumbers(compStartValues.top, startValues.top + startValues.height, accuracy)) {
                                    cell.resizingType = "RightBottom";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.left, startValues.left, accuracy) &&
                                    this.CompareNumbers(compStartValues.top, startValues.top + startValues.height, accuracy)) {
                                    cell.resizingType = "LeftBottom";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.top, startValues.top + startValues.height, accuracy)) {
                                    cell.resizingType = "Bottom";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.top, startValues.top, accuracy)) {
                                    cell.resizingType = "Top";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.left, startValues.left + startValues.width, accuracy)) {
                                    cell.resizingType = "Right";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.left, startValues.left, accuracy)) {
                                    cell.resizingType = "Left";
                                    cells.push(cell);
                                }
                                break;
                            }
                        case "Top":
                            {
                                if (this.CompareNumbers(compStartValues.top, startValues.top + startValues.height, accuracy)) {
                                    cell.resizingType = "Bottom";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.top, startValues.top, accuracy)) {
                                    cell.resizingType = "Top";
                                    cells.push(cell);
                                }
                                break;
                            }
                        case "RightTop":
                            {
                                if (this.CompareNumbers(compStartValues.left + compStartValues.width, startValues.left + startValues.width, accuracy) &&
                                    this.CompareNumbers(compStartValues.top, startValues.top + startValues.height, accuracy)) {
                                    cell.resizingType = "RightBottom";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.left + compStartValues.width, startValues.left, accuracy) &&
                                    this.CompareNumbers(compStartValues.top, startValues.top + startValues.height, accuracy)) {
                                    cell.resizingType = "LeftBottom";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.left + compStartValues.width, startValues.left, accuracy) &&
                                    this.CompareNumbers(compStartValues.top, startValues.top, accuracy)) {
                                    cell.resizingType = "LeftTop";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.top, startValues.top + startValues.height, accuracy)) {
                                    cell.resizingType = "Bottom";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.top, startValues.top, accuracy)) {
                                    cell.resizingType = "Top";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.left + compStartValues.width, startValues.left + startValues.width, accuracy)) {
                                    cell.resizingType = "Right";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.left + compStartValues.width, startValues.left, accuracy)) {
                                    cell.resizingType = "Left";
                                    cells.push(cell);
                                }
                                break;
                            }
                        case "Right":
                        case "ResizeWidth":
                            {
                                if (this.CompareNumbers(compStartValues.left + compStartValues.width, startValues.left + startValues.width, accuracy)) {
                                    cell.resizingType = "Right";
                                    cells.push(cell);
                                }
                                if (this.CompareNumbers(compStartValues.left + compStartValues.width, startValues.left, accuracy)) {
                                    cell.resizingType = "Left";
                                    cells.push(cell);
                                }
                                break;
                            }
                        case "RightBottom":
                        case "ResizeDiagonal":
                            {
                                if (this.CompareNumbers(compStartValues.left + compStartValues.width, startValues.left + startValues.width, accuracy) &&
                                    this.CompareNumbers(compStartValues.top + compStartValues.height, startValues.top, accuracy)) {
                                    cell.resizingType = "RightTop";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.left + compStartValues.width, startValues.left, accuracy) &&
                                    this.CompareNumbers(compStartValues.top + compStartValues.height, startValues.top, accuracy)) {
                                    cell.resizingType = "LeftTop";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.left + compStartValues.width, startValues.left, accuracy) &&
                                    this.CompareNumbers(compStartValues.top + compStartValues.height, startValues.top + startValues.height, accuracy)) {
                                    cell.resizingType = "LeftBottom";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.top + compStartValues.height, startValues.top + startValues.height, accuracy)) {
                                    cell.resizingType = "Bottom";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.top + compStartValues.height, startValues.top, accuracy)) {
                                    cell.resizingType = "Top";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.left + compStartValues.width, startValues.left + startValues.width, accuracy)) {
                                    cell.resizingType = "Right";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.left + compStartValues.width, startValues.left, accuracy)) {
                                    cell.resizingType = "Left";
                                    cells.push(cell);
                                }
                                break;
                            }
                        case "Bottom":
                        case "ResizeHeight":
                            {
                                if (this.CompareNumbers(compStartValues.top + compStartValues.height, startValues.top + startValues.height, accuracy)) {
                                    cell.resizingType = "Bottom";
                                    cells.push(cell);
                                }
                                if (this.CompareNumbers(compStartValues.top + compStartValues.height, startValues.top, accuracy)) {
                                    cell.resizingType = "Top";
                                    cells.push(cell);
                                }
                                break;
                            }
                        case "LeftBottom":
                            {
                                if (this.CompareNumbers(compStartValues.left, startValues.left, accuracy) &&
                                    this.CompareNumbers(compStartValues.top + compStartValues.height, startValues.top, accuracy)) {
                                    cell.resizingType = "LeftTop";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.left, startValues.left + startValues.width, accuracy) &&
                                    this.CompareNumbers(compStartValues.top + compStartValues.height, startValues.top, accuracy)) {
                                    cell.resizingType = "RightTop";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.left, startValues.left + startValues.width, accuracy) &&
                                    this.CompareNumbers(compStartValues.top + compStartValues.height, startValues.top + startValues.height, accuracy)) {
                                    cell.resizingType = "RightBottom";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.top + compStartValues.height, startValues.top + startValues.height, accuracy)) {
                                    cell.resizingType = "Bottom";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.top + compStartValues.height, startValues.top, accuracy)) {
                                    cell.resizingType = "Top";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.left, startValues.left + startValues.width, accuracy)) {
                                    cell.resizingType = "Right";
                                    cells.push(cell);
                                }
                                else if (this.CompareNumbers(compStartValues.left, startValues.left, accuracy)) {
                                    cell.resizingType = "Left";
                                    cells.push(cell);
                                }
                                break;
                            }
                        case "Left":
                            {
                                if (this.CompareNumbers(compStartValues.left, startValues.left, accuracy)) {
                                    cell.resizingType = "Left";
                                    cells.push(cell);
                                }
                                if (this.CompareNumbers(compStartValues.left, startValues.left + startValues.width, accuracy)) {
                                    cell.resizingType = "Right";
                                    cells.push(cell);
                                }
                                break;
                            }
                    }
                }
            }
        }
    }

    return cells;
}

StiMobileDesigner.prototype.GetRecentConnectionsFromCookies = function () {
    var connectionsStr = this.GetCookie("StimulsoftMobileDesignerRecentConnections");
    return (connectionsStr ? JSON.parse(connectionsStr) : []);
}

StiMobileDesigner.prototype.SaveRecentConnectionToCookies = function (connectionObject) {
    var recentConnections = this.GetRecentConnectionsFromCookies();
    if (recentConnections.length >= 6) recentConnections.splice(0, 1);
    recentConnections.push(connectionObject);
    this.SetCookie("StimulsoftMobileDesignerRecentConnections", JSON.stringify(recentConnections));
}

StiMobileDesigner.prototype.GetConnectionNameFromPathData = function (path) {
    if (path.toLowerCase().indexOf("http") == 0 && path.indexOf("?") > 0) {
        path = path.substring(0, path.indexOf("?"));
    }
    var connectionName = path.replace(/^.*[\\\/]/, '');
    if (connectionName.lastIndexOf(".") > 0) connectionName = connectionName.substring(0, connectionName.lastIndexOf("."));
    connectionName = connectionName.replace(/:/g, '').replace(/\\/g, '').replace(/\//g, '');

    return connectionName;
}

StiMobileDesigner.prototype.SaveFileToRecentArray = function (name, path, containsDashboard, containsForm) {
    var recentArray = this.GetRecentArray("StimulsoftMobileDesignerRecentArray");
    var haveElement = false;

    for (var i = 0; i < recentArray.length; i++) {
        if (recentArray[i].name == name && recentArray[i].path == path) {
            var temp = recentArray[i];
            recentArray.splice(i, 1);
            recentArray.splice(0, 0, temp);
            haveElement = true;
            break;
        }
    }

    if (!haveElement) {
        recentArray.splice(0, 0, {
            name: name,
            path: path,
            containsDashboard: (containsDashboard != null ? containsDashboard : false),
            containsForm: (containsForm != null ? containsForm : false)
        });

        if (recentArray.length > 10) {
            recentArray.pop();
        }
    }

    this.SetCookie("StimulsoftMobileDesignerRecentArray", JSON.stringify(recentArray));
}

StiMobileDesigner.prototype.GetRecentArray = function () {
    var recentArray = this.GetCookie("StimulsoftMobileDesignerRecentArray");
    try {
        recentArray = recentArray != null ? JSON.parse(recentArray) : [];
    }
    catch (e) {
        recentArray = [];
    }
    return recentArray;
}

StiMobileDesigner.prototype.JSONDateFormatToDate = function (dateJSONFormat, format) {
    if (dateJSONFormat.substring(0, 6) == "/Date(") {
        var dateStr = dateJSONFormat.replace("/Date(", "").replace(")/", "");
        return typeof (format) == "boolean" ? this.formatDate(new Date(parseInt(dateStr)), this.options.STI_DATE_TIME_FORMAT) : (format ? this.formatDate(new Date(parseInt(dateStr)), format) : new Date(parseInt(dateStr)));
    } else {
        return dateJSONFormat;
    }
}

StiMobileDesigner.prototype.DateToJSONDateFormat = function (date) {
    var offset = date.getTimezoneOffset() * -1;
    var hoursOffset = Math.abs(parseInt(offset / 60));
    var minutesOffset = Math.abs(offset % 60);
    if (hoursOffset.toString().length == 1) hoursOffset = "0" + hoursOffset;
    if (minutesOffset.toString().length == 1) minutesOffset = "0" + minutesOffset;
    return "/Date(" + Date.parse(date).toString() + ")/";
}

StiMobileDesigner.prototype.formatDate = function (formatDate, formatString) {
    var yyyy = formatDate.getFullYear();
    var yy = yyyy.toString().substring(2);
    var m = formatDate.getMonth() + 1;
    var mm = m < 10 ? "0" + m : m;
    var d = formatDate.getDate();
    var dd = d < 10 ? "0" + d : d;

    var h = formatDate.getHours();
    var hh = h < 10 ? "0" + h : h;
    var n = formatDate.getMinutes();
    var nn = n < 10 ? "0" + n : n;
    var s = formatDate.getSeconds();
    var ss = s < 10 ? "0" + s : s;

    formatString = formatString.replace(/yyyy/i, yyyy);
    formatString = formatString.replace(/yy/i, yy);
    formatString = formatString.replace(/mm/i, mm);
    formatString = formatString.replace(/m/i, m);
    formatString = formatString.replace(/dd/i, dd);
    formatString = formatString.replace(/d/i, d);
    formatString = formatString.replace(/hh/i, hh);
    formatString = formatString.replace(/h/i, h);
    formatString = formatString.replace(/nn/i, nn);
    formatString = formatString.replace(/n/i, n);
    formatString = formatString.replace(/ss/i, ss);
    formatString = formatString.replace(/s/i, s);

    return formatString;
}

StiMobileDesigner.prototype.getBackText = function (withoutBrackets) {
    var backText = String.fromCharCode(84) + "r" + String.fromCharCode(105) + "a";
    if (withoutBrackets) return backText + String.fromCharCode(108);
    return String.fromCharCode(91) + backText + String.fromCharCode(108) + String.fromCharCode(93);
}

StiMobileDesigner.prototype.openNewWindow = function (url, name, params) {
    var win = window.open(url || "about:blank", name, params);
    return win;
}

StiMobileDesigner.prototype.stringToTime = function (timeStr) {
    var timeArray = timeStr.split(":");
    var time = { hours: 0, minutes: 0, seconds: 0 };

    time.hours = this.StrToInt(timeArray[0]);
    if (timeArray.length > 1) time.minutes = this.StrToInt(timeArray[1]);
    if (timeArray.length > 2) time.seconds = this.StrToInt(timeArray[2]);

    if (time.hours < 0) time.hours = 0;
    if (time.minutes < 0) time.minutes = 0;
    if (time.seconds < 0) time.seconds = 0;

    if (time.hours > 23) time.hours = 23;
    if (time.minutes > 59) time.minutes = 59;
    if (time.seconds > 59) time.seconds = 59;

    return time;
}

StiMobileDesigner.prototype.SetWindowTitle = function (text) {
    if (!this.options.allowChangeWindowTitle) return;

    if (text == null && this.currentTitleText) {
        text = this.currentTitleText;
    }

    this.currentTitleText = text;

    if (this.options.currentPage && !this.options.currentPage.valid) {
        text += " " + this.getBackText();
    }
    document.title = text;

    if (this.options.toolBar && this.options.toolBar.designerTitle) {
        this.options.toolBar.designerTitle.innerHTML = text;
    }
}

StiMobileDesigner.prototype.SetWindowIcon = function (imageSrc, doc_) {
    var doc = doc_ || document;
    var head = doc.head || doc.getElementsByTagName("head")[0];
    var link = doc.createElement("link"),
        oldLink = doc.getElementById("window-icon");
    link.id = "window-icon";
    link.rel = "icon";
    link.href = imageSrc;
    link.setAttribute("stimulsoft", "stimulsoft");
    if (oldLink) {
        head.removeChild(oldLink);
    }
    head.appendChild(link);
}

StiMobileDesigner.prototype.AddDragAndDropToContainer = function (container, draggableSuccessFunc) {
    if (!this.options.canOpenFiles) return;

    var jsObject = this;
    container.draggable = true;

    var stopEvent = function (event) {
        event.stopPropagation();
        event.preventDefault();
    };

    var dropFiles = function (files) {
        var reader = new FileReader();

        reader.onload = function (event) {
            try {
                draggableSuccessFunc(files, event.target.result);
            }
            catch (error) {
                var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                errorMessageForm.show(error.message);
            }
        };

        reader.onerror = function (event) {
            var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
            errorMessageForm.show(event.target.error.code);
        };

        if (files && files.length > 0) {
            reader.readAsDataURL(files[0]);
        }
    };

    container.draggable = false;

    jsObject.addEvent(container, "dragover", function (event) {
        stopEvent(event);
    });

    jsObject.addEvent(container, "drop", function (event) {
        stopEvent(event);
        event.preventDefault && event.preventDefault();
        dropFiles(event.dataTransfer.files);
        return false;
    });
}

StiMobileDesigner.prototype.GetHumanFileSize = function (size, short) {
    var i = Math.floor(Math.log(size) / Math.log(1024));
    return (short ? Math.round(size / Math.pow(1024, i)) : ((size / Math.pow(1024, i)).toFixed(2) * 1)) + ' ' + ['B', 'KB', 'MB', 'GB', 'TB'][i];
};

StiMobileDesigner.prototype.NumberWithSpaces = function (x) {
    if (x == null) return "";
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, " ");
}

StiMobileDesigner.prototype.GetFileNameFromPath = function (path) {
    var fileName = path;
    if (path.lastIndexOf("/") >= 0) fileName = fileName.substring(fileName.lastIndexOf("/") + 1);
    if (path.lastIndexOf("\\") >= 0) fileName = fileName.substring(fileName.lastIndexOf("\\") + 1);
    if (path.lastIndexOf(".") >= 0) fileName = fileName.substring(0, fileName.lastIndexOf("."));
    return fileName;
};

StiMobileDesigner.prototype.CheckImagesInDictionary = function () {
    if (this.options.report) {
        var dictionary = this.options.report.dictionary;
        for (var i = 0; i < dictionary.resources.length; i++) {
            if (dictionary.resources[i].type == "Image") return true;
        }
        var variables = this.GetCollectionFromCategoriesCollection(dictionary.variables);
        for (var i = 0; i < variables.length; i++) {
            if (variables[i].type == "image") return true;
        }
        var dataSources = this.GetDataSourcesFromDictionary(dictionary);
        for (var i = 0; i < dataSources.length; i++) {
            for (var k = 0; k < dataSources[i].columns.length; k++) {
                if (dataSources[i].columns[k].type == "image" ||
                    dataSources[i].columns[k].type == "byte[]" ||
                    dataSources[i].columns[k].type == "string") return true;
            }
        }
    }

    return false;
};

StiMobileDesigner.prototype.CheckRichTextInDictionary = function () {
    if (this.options.report) {
        var dictionary = this.options.report.dictionary;
        for (var i = 0; i < dictionary.resources.length; i++) {
            if (dictionary.resources[i].type == "Rtf" || dictionary.resources[i].type == "Txt") return true;
        }
        var variables = this.GetCollectionFromCategoriesCollection(dictionary.variables);
        for (var i = 0; i < variables.length; i++) {
            if (variables[i].type == "string") return true;
        }
        var dataSources = this.GetDataSourcesFromDictionary(dictionary);
        for (var i = 0; i < dataSources.length; i++) {
            for (var k = 0; k < dataSources[i].columns.length; k++) {
                if (dataSources[i].columns[k].type == "byte[]" ||
                    dataSources[i].columns[k].type == "string") return true;
            }
        }
    }

    return true;
};

StiMobileDesigner.prototype.ClearAllGalleries = function () {
    this.options.imagesGallery = null;
    this.options.richTextGallery = null;
}

//Fixed bug in JS Designer after runing open dialog
StiMobileDesigner.prototype.ReturnFocusToDesigner = function () {
    var tempTextBox = this.TextBox(null, 1);
    this.options.mainPanel.appendChild(tempTextBox);
    tempTextBox.focus();
    this.options.mainPanel.removeChild(tempTextBox);
}

StiMobileDesigner.prototype.AddResourceFile = function (file, content, callbackFunc) {
    var jsObject = this;

    if (this.options.dictionaryPanel && this.options.dictionaryPanel.checkResourcesCount())
        return;

    if (!jsObject.options.standaloneJsMode && file.size > this.options.reportResourcesMaximumSize) {
        var message = jsObject.loc.Notices.QuotaMaximumResourceSizeExceeded + "<br>" + jsObject.loc.PropertyMain.Maximum + ": " + jsObject.GetHumanFileSize(jsObject.options.reportResourcesMaximumSize, true);
        if (jsObject.options.cloudMode) {
            this.InitializeNotificationForm(function (form) {
                form.show(message, jsObject.NotificationMessages("upgradeYourPlan"), "Notifications.Resources.png");
            });
        }
        else {
            var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
            errorMessageForm.show(message, "Warning");
        }
        return;
    }

    var imageExts = ["gif", "png", "jpeg", "jpg", "bmp", "tiff", "ico", "emf", "wmf", "svg"];
    var reportExts = ["mrt", "mrz"];
    var reportSnapshotExts = ["mdc", "mdz"];
    var dataExts = ["xls", "xlsx", "csv", "dbf", "json", "xml", "wkt"];

    var fileName = file.name || "Resource";
    var fileExt = fileName.substring(fileName.lastIndexOf(".") + 1).toLowerCase();
    var resourceType;

    if (imageExts.indexOf(fileExt) >= 0) {
        resourceType = "Image";
    }
    else if (reportExts.indexOf(fileExt) >= 0) {
        resourceType = "Report";
    }
    else if (reportSnapshotExts.indexOf(fileExt) >= 0) {
        resourceType = "ReportSnapshot";
    }
    else if (fileExt == "ttf") {
        resourceType = "FontTtf";
    }
    else if (fileExt == "otf") {
        resourceType = "FontOtf";
    }
    else if (fileExt == "ttc") {
        resourceType = "FontTtc";
    }
    //else if (fileExt == "eot") {
    //    resourceType = "FontEot";
    //}
    //else if (fileExt == "woff") {
    //    resourceType = "FontWoff";
    //}
    else if (fileExt == "rtf") {
        resourceType = "Rtf";
    }
    else if (fileExt == "txt") {
        resourceType = "Txt";
    }
    else if (fileExt == "xml") {
        resourceType = "Xml";
    }
    else if (fileExt == "xsd") {
        resourceType = "Xsd";
    }
    else if (fileExt == "xls" || fileExt == "xlsx") {
        resourceType = "Excel";
    }
    else if (fileExt == "csv") {
        resourceType = "Csv";
    }
    else if (fileExt == "dbf") {
        resourceType = "Dbf";
    }
    else if (fileExt == "json") {
        resourceType = "Json";
    }
    else if (fileExt == "wkt") {
        resourceType = "Gis";
    }
    else if (fileExt == "doc" || fileExt == "docx") {
        resourceType = "Word";
    }
    else if (fileExt == "pdf") {
        resourceType = "Pdf";
    }
    else if (fileExt == "map") {
        resourceType = "Map";
    }
    else {
        return;
    }

    var resourceName = this.GetNewName("Resource", null, fileName.substring(0, fileName.lastIndexOf(".")));

    var resource = {};
    resource.mode = "New";
    resource.name = resourceName;
    resource.alias = resourceName;
    resource.type = resourceType;
    resource.loadedContent = this.options.mvcMode ? encodeURIComponent(content) : content;
    resource.haveContent = true;

    var propertiesPanel = this.options.propertiesPanel;

    if (this.options.showPropertiesGrid &&
        this.options.showDictionary &&
        !propertiesPanel.styleDesignerMode &&
        !propertiesPanel.editChartMode &&
        !propertiesPanel.editCrossTabMode &&
        !propertiesPanel.editBarCodeMode &&
        !propertiesPanel.editDbsMeterMode) {
        this.options.propertiesPanel.showContainer("Dictionary");
    }

    var jsObject = this;

    var createResource = function (resource) {
        if (callbackFunc) {
            jsObject.SendCommandCreateOrEditResource(resource, callbackFunc);
        }
        else if (dataExts.indexOf(fileExt) >= 0) {
            jsObject.SendCommandCreateDatabaseFromResource(resource);
        }
        else {
            jsObject.SendCommandCreateOrEditResource(resource);
        }
    }

    if (!this.options.report) {
        jsObject.SendCommandCreateReport(function () {
            createResource(resource);
        });
    }
    else {
        createResource(resource);
    }
}

StiMobileDesigner.prototype.InsertSvgContent = function (svgContainer, svgContent) {
    var temp = document.createElement("div");
    temp.innerHTML = "<svg>" + svgContent + "</svg>";
    while (svgContainer.firstChild != null) svgContainer.removeChild(svgContainer.firstChild);
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
        svgContainer.appendChild(el);
        if (el.tagName == "image") innerObjects.image = el;
        if (el.tagName == "text") innerObjects.text = el;
        if (el.tagName == "rect") innerObjects.rect = el;
        if (el.tagName == "g" && el.getAttribute("transform") && el.getAttribute("transform").indexOf("translate") >= 0) {
            writeElementXY(el);
        }
    });
}

StiMobileDesigner.prototype.GetCursorType = function (resizingType) {
    switch (resizingType) {
        case "LeftTop":
        case "RightBottom":
            return "nw-resize";
        case "RightTop":
        case "LeftBottom":
            return "sw-resize";
        case "Right":
        case "Left":
            return "e-resize";
        case "Top":
        case "Bottom":
            return "s-resize";
    }
}

StiMobileDesigner.prototype.PasteCurrentClipboardComponent = function () {
    if (this.options.clipboardMode && this.options.in_drag) {
        this.SendCommandChangeRectComponent(this.options.in_drag[0], "MoveComponent");
        this.options.clipboardMode = false;
        this.options.in_drag = false;
    }
}

StiMobileDesigner.prototype.getStyleObject = function (styleName) {
    var styleObject = {};
    if (!this.options.showPropertiesWhichUsedFromStyles && styleName && this.options.report && this.options.report.stylesCollection) {
        for (var i = 0; i < this.options.report.stylesCollection.length; i++) {
            if (this.options.report.stylesCollection[i].properties.name == styleName)
                return this.options.report.stylesCollection[i].properties;
        }
    }

    return styleObject;
}

StiMobileDesigner.prototype.getCloudName = function () {
    if (this.options.serverMode)
        return "Server";

    return "Stimulsoft Cloud";
}

StiMobileDesigner.prototype.copyTextToClipboard = function (text) {
    var textArea = document.createElement("textarea");
    textArea.setAttribute("style", "position: fixed; top: 0; left: 0; width: 2em; height: 2em; padding: 0; border: none; outline: none; box-shadow: none; background: transparent;");
    textArea.value = text;

    document.body.appendChild(textArea);
    textArea.select();

    try {
        document.execCommand('copy');
    } catch (err) {
        console.log(err);
    }

    document.body.removeChild(textArea);
}

StiMobileDesigner.prototype.readTextFromClipboard = function (callbackFunc) {
    if (navigator.clipboard && navigator.clipboard.readText) {
        this.ReturnFocusToDesigner();

        var clip = navigator.clipboard.readText()
        clip.then(function (text) {
            callbackFunc(text);
        });
        clip.catch(function () {
            callbackFunc();
        });
    }
    else callbackFunc();
}

StiMobileDesigner.prototype.CreateSvgElement = function (name) {
    return ("createElementNS" in document) ? document.createElementNS('http://www.w3.org/2000/svg', name) : document.createElement(name);
}

StiMobileDesigner.prototype.GetAllCrossTabsInReport = function () {
    var crossTabNames = [];
    var report = this.options.report;

    if (report) {
        for (var pageName in report.pages) {
            var page = report.pages[pageName];
            for (var componentName in page.components) {
                var component = page.components[componentName];
                if (component.typeComponent == "StiCrossTab") {
                    crossTabNames.push(component.properties.name);
                }
            }
        }
    }

    return crossTabNames;
}

StiMobileDesigner.prototype.GetReportFileName = function (withExt) {
    var fileName = withExt ? "Report.mrt" : "Report";

    if ((this.options.cloudMode || this.options.serverMode) && this.options.cloudParameters && this.options.cloudParameters.reportTemplateItemKey) {
        fileName = this.options.cloudParameters.reportName;
        if (withExt) fileName += ".mrt";
    }
    else if (this.options.formsDesignerMode && this.options.formsDesignerFrame && this.options.formsDesignerFrame.formName) {
        fileName = this.options.formsDesignerFrame.formName;
        if (!this.EndsWith(fileName, ".mrt") && withExt) fileName += ".mrt";
    }
    else if (this.options.report && (this.options.report.properties.reportFile || this.options.report.properties.reportName)) {
        fileName = this.options.report.properties.reportFile || this.ExtractBase64Value(this.options.report.properties.reportName);
        if (!withExt && (this.EndsWith(fileName.toLowerCase(), ".mrt") || this.EndsWith(fileName.toLowerCase(), ".mrz") || this.EndsWith(fileName.toLowerCase(), ".mrx"))) {
            fileName = fileName.substring(0, fileName.length - 4);
        }
        else if (withExt && !this.EndsWith(fileName.toLowerCase(), ".mrt") && !this.EndsWith(fileName.toLowerCase(), ".mrz") && !this.EndsWith(fileName.toLowerCase(), ".mrx")) {
            fileName += ".mrt";
        }
    }    
    if (this.options.report && this.options.report.encryptedPassword && withExt) {
        if (this.EndsWith(fileName.toLowerCase(), ".mrt") || this.EndsWith(fileName.toLowerCase(), ".mrz")) {
            fileName = fileName.substring(0, fileName.length - 4) + ".mrx";
        }
    }

    return fileName;
}

StiMobileDesigner.prototype.CheckRequestTimeout = function (data) {
    if (data && data.command) {
        if (data.command == "RunQueryScript" || data.command == "ViewData") {
            return data.commandTimeout ? this.StrToInt(data.commandTimeout) : null;
        }
        else if (data.command == "LoadReportToViewer" && this.options.report) {
            var requestTimeout = this.options.requestTimeout;
            var dataSources = this.GetDataSourcesFromDictionary(this.options.report.dictionary);
            for (var i = 0; i < dataSources.length; i++) {
                if (dataSources[i].commandTimeout && dataSources[i].commandTimeout > requestTimeout) {
                    requestTimeout = this.StrToInt(dataSources[i].commandTimeout);
                }
            }
            if (this.options.viewer) {
                this.options.viewer.jsObject.options.server.requestTimeout = requestTimeout;
            }

            return requestTimeout;
        }
    }

    return null;
}

StiMobileDesigner.prototype.DbsElementHaveStyles = function (typeComponent) {
    return (typeComponent == "StiChartElement" || typeComponent == "StiGaugeElement" ||
        typeComponent == "StiIndicatorElement" || typeComponent == "StiProgressElement" ||
        typeComponent == "StiRegionMapElement" || typeComponent == "StiOnlineMapElement" ||
        typeComponent == "StiTableElement" || typeComponent == "StiPivotTableElement" ||
        typeComponent == "StiListBoxElement" || typeComponent == "StiComboBoxElement" ||
        typeComponent == "StiTreeViewElement" || typeComponent == "StiTreeViewBoxElement" ||
        typeComponent == "StiDatePickerElement" || typeComponent == "StiCardsElement" ||
        typeComponent == "StiButtonElement");
}

StiMobileDesigner.prototype.CanDropDictionaryItem = function (typeComponent) {
    return (typeComponent == "StiChartElement" || typeComponent == "StiGaugeElement" ||
        typeComponent == "StiIndicatorElement" || typeComponent == "StiProgressElement" ||
        typeComponent == "StiRegionMapElement" || typeComponent == "StiOnlineMapElement" ||
        typeComponent == "StiTableElement" || typeComponent == "StiPivotTableElement" ||
        typeComponent == "StiListBoxElement" || typeComponent == "StiComboBoxElement" ||
        typeComponent == "StiTreeViewElement" || typeComponent == "StiTreeViewBoxElement" ||
        typeComponent == "StiDatePickerElement" || typeComponent == "StiCardsElement");
}

StiMobileDesigner.prototype.IsFilterElement = function (typeComponent) {
    return (typeComponent == "StiListBoxElement" ||
        typeComponent == "StiDatePickerElement" ||
        typeComponent == "StiComboBoxElement" ||
        typeComponent == "StiTreeViewElement" ||
        typeComponent == "StiTreeViewBoxElement");
}

StiMobileDesigner.prototype.RemoveStylesFromCache = function (componentName, typeComponent) {
    if (this.options.report.stylesCache) {
        if (typeComponent) {
            if (typeComponent == "StiChart") {
                if (!componentName) {
                    this.options.report.stylesCache.chartStyles = {};
                }
                else if (this.options.report.stylesCache.chartStyles[componentName]) {
                    delete this.options.report.stylesCache.chartStyles[componentName]
                }
                return;
            }
            if (typeComponent == "StiGauge") {
                if (!componentName) {
                    this.options.report.stylesCache.gaugeStyles = {};
                }
                else if (this.options.report.stylesCache.gaugeStyles[componentName]) {
                    delete this.options.report.stylesCache.gaugeStyles[componentName]
                }
                return;
            }
            if (typeComponent == "StiMap") {
                if (!componentName) {
                    this.options.report.stylesCache.mapStyles = {};
                }
                else if (this.options.report.stylesCache.mapStyles[componentName]) {
                    delete this.options.report.stylesCache.mapStyles[componentName]
                }
                return;
            }
        }

        if (componentName && this.options.report.stylesCache[componentName]) {
            delete this.options.report.stylesCache[componentName];
        }
    }
}

StiMobileDesigner.prototype.AddStylesToCache = function (componentName, stylesContent, typeComponent) {
    if (this.options.report) {
        if (!this.options.report.stylesCache) {
            this.options.report.stylesCache = {
                chartStyles: {},
                gaugeStyles: {},
                mapStyles: {},
                tableStyles: null,
                crossTabStyles: null
            };
        }
        if (typeComponent == "StiTable") {
            this.options.report.stylesCache.tableStyles = stylesContent;
        }
        if (typeComponent == "StiCrossTab") {
            this.options.report.stylesCache.crossTabStyles = stylesContent;
        }
        else if (componentName) {
            if (typeComponent == "StiChart") {
                this.options.report.stylesCache.chartStyles[componentName] = stylesContent;
            }
            else if (typeComponent == "StiGauge") {
                this.options.report.stylesCache.gaugeStyles[componentName] = stylesContent;
            }
            else if (typeComponent == "StiMap") {
                this.options.report.stylesCache.mapStyles[componentName] = stylesContent;
            }
            else {
                this.options.report.stylesCache[componentName] = stylesContent;
            }
        }
    }
}

StiMobileDesigner.prototype.GetItemCaption = function (itemObject) {
    if (!itemObject) return "";
    if (!itemObject.alias) return itemObject.name;
    return itemObject.name == itemObject.alias ? itemObject.name : itemObject.name + " [" + itemObject.alias + "]";
}

StiMobileDesigner.prototype.DecToHex = function (d) {
    if (d > 15) {
        return d.toString(16)
    } else {
        return "0" + d.toString(16)
    }
}

StiMobileDesigner.prototype.RgbToHex = function (r, g, b, a) {
    return ("#" + (a ? this.DecToHex(a) : "") + this.DecToHex(r) + this.DecToHex(g) + this.DecToHex(b));
};

StiMobileDesigner.prototype.HexToRgb = function (hex) {
    if (hex) {
        if (hex.length >= 9) {
            var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
            if (result) return { a: parseInt(result[1], 16), r: parseInt(result[2], 16), g: parseInt(result[3], 16), b: parseInt(result[4], 16) };
        }
        else {
            var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
            if (result) return { r: parseInt(result[1], 16), g: parseInt(result[2], 16), b: parseInt(result[3], 16) };
        }
    }
    return null;
}

StiMobileDesigner.prototype.RgbColorStrToHexColor = function (rgbColor) {
    if (rgbColor) {
        rgbColor = rgbColor.toString().toLowerCase().replace("rgb(", "").replace(")", "");
        var colorArray = rgbColor.split(",");

        if (colorArray.length == 3)
            return this.RgbToHex(parseInt(colorArray[0]), parseInt(colorArray[1]), parseInt(colorArray[2]));

        if (colorArray.length == 4)
            return this.RgbToHex(parseInt(colorArray[1]), parseInt(colorArray[2]), parseInt(colorArray[3]), parseInt(colorArray[0]))
    }
    return "#000000";
}

StiMobileDesigner.prototype.GetHTMLColor = function (colorStr) {
    if (!colorStr || colorStr == "transparent") return "transparent";

    var colorArray = colorStr.split(",");
    if (colorArray.length == 4) return ("rgb(" + colorArray[1] + "," + colorArray[2] + "," + colorArray[3] + "," + this.StrToInt(colorArray[0]) / 255 + ")");
    if (colorArray.length == 3) return ("rgb(" + colorArray[0] + "," + colorArray[1] + "," + colorArray[2] + ")");

    return "transparent";
}

StiMobileDesigner.prototype.GetOpacityFromColor = function (colorStr) {
    var color = colorStr.split(",")
    if (color.length == 4) {
        return parseInt(color[0]) / 255;
    }

    return 1;
}

StiMobileDesigner.prototype.GetColorNameByRGB = function (rgb, notLocalize) {
    if (rgb == "0,255,255,255" || rgb == "transparent") {
        return (notLocalize ? "Transparent" : this.loc.PropertyColor.Transparent);
    }
    for (var i = 0; i < this.ConstWebColors.length; i++) {
        if (this.ConstWebColors[i][1] == rgb) {
            return (notLocalize ? this.ConstWebColors[i][0] : (this.loc.PropertyColor[this.ConstWebColors[i][0]] || this.ConstWebColors[i][0]));
        }
    }

    return false;
}

StiMobileDesigner.prototype.ArraysEqual = function (a, b) {
    if (a === b) return true;
    if (a == null || b == null) return false;
    if (a.length != b.length) return false;

    for (var i = 0; i < a.length; ++i) {
        if (a[i] !== b[i]) return false;
    }
    return true;
}

StiMobileDesigner.prototype.RunWizard = function (wizardType) {
    var fileMenu = this.options.menus.fileMenu || this.InitializeFileMenu();

    if (this.EndsWith(wizardType, "StiBlankReportWizardService") || wizardType == "BlankReport") {
        this.ExecuteAction("blankReportButton");
    }
    else if (this.EndsWith(wizardType, "StiStandardReportWizardService") || wizardType == "StandardReport") {
        fileMenu.changeVisibleState(true);
        this.ExecuteAction("standartReportButton");
    }
    else if (this.EndsWith(wizardType, "StiMasterDetailWizardService") || wizardType == "MasterDetailReport") {
        fileMenu.changeVisibleState(true);
        this.ExecuteAction("masterDetailReportButton");
    }
    else if (this.EndsWith(wizardType, "StiLabelWizardService") || wizardType == "LabelReport") {
        fileMenu.changeVisibleState(true);
        this.ExecuteAction("labelReportButton");
    }
    else if (this.EndsWith(wizardType, "StiInvoicesReportWizardService") || wizardType == "InvoicesReport") {
        fileMenu.changeVisibleState(true);
        this.ExecuteAction("invoiceReportButton");
    }
    else if (this.EndsWith(wizardType, "StiOrdersReportWizardService") || wizardType == "OrdersReport") {
        fileMenu.changeVisibleState(true);
        this.ExecuteAction("orderReportButton");
    }
    else if (this.EndsWith(wizardType, "StiQuotationReportWizardService") || wizardType == "QuotationReport") {
        fileMenu.changeVisibleState(true);
        this.ExecuteAction("quotationReportButton");
    }
    else if (this.EndsWith(wizardType, "StiBlankDashboardWizardService") || wizardType == "BlankDashboard") {
        this.ExecuteAction("blankDashboardButton");
    }
    else if (this.EndsWith(wizardType, "StiFinancialDashboardWizardService") || wizardType == "FinancialDashboard") {
        this.SendCommandOpenWizardDashboard("Financial");
    }
    else if (this.EndsWith(wizardType, "StiOrdersDashboardWizardService") || wizardType == "OrdersDashboard") {
        this.SendCommandOpenWizardDashboard("Orders");
    }
    else if (this.EndsWith(wizardType, "StiSalesOverviewDashboardWizardService") || wizardType == "SalesOverviewDashboard") {
        this.SendCommandOpenWizardDashboard("SalesOverview");
    }
    else if (this.EndsWith(wizardType, "StiTicketsStatisticsDashboardWizardService") || wizardType == "TicketsStatisticsDashboard") {
        this.SendCommandOpenWizardDashboard("TicketsStatistics");
    }
    else if (this.EndsWith(wizardType, "StiTrafficAnalyticsDashboardWizardService") || wizardType == "TrafficAnalyticsDashboard") {
        this.SendCommandOpenWizardDashboard("TrafficAnalytics");
    }
    else if (this.EndsWith(wizardType, "StiVehicleProductionDashboardWizardService") || wizardType == "VehicleProductionDashboard") {
        this.SendCommandOpenWizardDashboard("VehicleProduction");
    }
    else if (this.EndsWith(wizardType, "StiWebsiteAnalyticsDashboardWizardService") || wizardType == "WebsiteAnalyticsDashboard") {
        this.SendCommandOpenWizardDashboard("WebsiteAnalytics");
    }
}

StiMobileDesigner.prototype.DeleteTemporaryMenus = function () {
    if (this.options.menus) {
        for (var name in this.options.menus) {
            var menu = this.options.menus[name];
            if (menu.isTemporary) {
                if (menu.parentElement) menu.parentElement.removeChild(menu);
                delete this.options.menus[name];
            }
        }
    }
};

StiMobileDesigner.prototype.IsVisibilityBands = function () {
    var componentTypes = ["StiReportTitleBand", "StiReportSummaryBand", "StiPageHeaderBand", "StiPageFooterBand", "StiGroupHeaderBand",
        "StiGroupFooterBand", "StiHeaderBand", "StiFooterBand", "StiColumnHeaderBand", "StiColumnFooterBand", "StiDataBand", "StiHierarchicalBand",
        "StiChildBand", "StiEmptyBand", "StiOverlayBand", "StiTableOfContents"];

    for (var i = 0; i < componentTypes.length; i++) {
        if (this.options.visibilityBands[componentTypes[i]])
            return true;
    }

    return false;
}

StiMobileDesigner.prototype.IsVisibilityCrossBands = function () {
    var componentTypes = ["StiCrossGroupHeaderBand", "StiCrossGroupFooterBand", "StiCrossHeaderBand", "StiCrossFooterBand", "StiCrossDataBand"];

    for (var i = 0; i < componentTypes.length; i++) {
        if (this.options.visibilityCrossBands[componentTypes[i]])
            return true;
    }

    return false;
}

StiMobileDesigner.prototype.IsVisibilityComponents = function () {
    var componentTypes = this.options.isJava
        ? ["StiText", "StiImage", "StiPanel", "StiClone", "StiCheckBox", "StiSubReport", "StiZipCode", "StiTable", "StiCrossTab"]
        : ["StiText", "StiTextInCells", "StiRichText", "StiImage", "StiPanel", "StiClone", "StiCheckBox", "StiSubReport", "StiZipCode", "StiTable", "StiCrossTab", "StiSparkline", "StiMathFormula", "StiElectronicSignature", "StiPdfDigitalSignature"];

    for (var i = 0; i < componentTypes.length; i++) {
        if (this.options.visibilityComponents[componentTypes[i]])
            return true;
    }

    return false;
}

StiMobileDesigner.prototype.IsVisibilityShapes = function () {
    var componentTypes = ["StiHorizontalLinePrimitive", "StiVerticalLinePrimitive", "StiRectanglePrimitive", "StiRoundedRectanglePrimitive", "StiShape"];

    for (var i = 0; i < componentTypes.length; i++) {
        if (this.options.visibilityComponents[componentTypes[i]])
            return true;
    }

    return false;
}

StiMobileDesigner.prototype.IsVisibilitySignatures = function () {
    var componentTypes = ["StiElectronicSignature", "StiPdfDigitalSignature"];

    for (var i = 0; i < componentTypes.length; i++) {
        if (this.options.visibilityComponents[componentTypes[i]])
            return true;
    }

    return false;
}

StiMobileDesigner.prototype.ReplaceAllInString = function (str, search, replace) {
    return str.split(search).join(replace);
}

StiMobileDesigner.prototype.CheckExpressionBrackets = function (str) {
    if (!str) return str;

    var isNumeric = function (s) {
        return !isNaN(parseInt(s));
    }

    var isLetter = function (s) {
        return (s.toLowerCase() != s.toUpperCase());
    }

    try {
        if (isNumeric(str[0]))
            return "[" + str + "]";

        for (var i = 0; i < str.length; i++) {
            if (!(isLetter(str[i]) || isNumeric(str[i]) || str[i] == "." || str[i] == "_"))
                return "[" + str + "]";
        }
    }
    catch (e) { }

    return str;
}

StiMobileDesigner.prototype.StartWizardForm2 = function (wizardAction, templateName) {
    var jsObject = this;

    this.InitializeWizardForm2(function (wizardForm) {
        wizardForm.typeReport = wizardAction == "invoiceReportButton" ? "Invoice" : (wizardAction == "orderReportButton" ? "Order" : (wizardAction == "quotationReportButton" ? "Quotation" : "Label"));
        wizardForm.dataSourcesFromServer = jsObject.options.report && jsObject.options.newReportDictionary == "DictionaryMerge"
            ? jsObject.GetDataSourcesAndBusinessObjectsFromDictionary(jsObject.options.report.dictionary) : [];

        var showWizard = function () {
            jsObject.SendCommandCreateReport(function () {
                if (templateName) {
                    wizardForm.createStepsComplete = function () {
                        if (templateName) {
                            var templatesPanel = wizardForm.stepPanels[0].content;
                            if (templatesPanel.buttons && templatesPanel.buttons[templateName]) {
                                templatesPanel.buttons[templateName].action();
                                wizardForm.createStepsComplete = function () { };
                            }
                        }
                    };
                }
                wizardForm.changeVisibleState(true);
            });
        }

        if (jsObject.options.report != null && jsObject.options.reportIsModified) {
            var messageForm = jsObject.MessageFormForSave();
            messageForm.changeVisibleState(true);
            messageForm.action = function (state) {
                if (state) {
                    jsObject.ActionSaveReport(function () { showWizard(); });
                }
                else {
                    showWizard();
                }
            }
        }
        else {
            showWizard();
        }
    });
}

StiMobileDesigner.prototype.GetParameterFromUrl = function (name, url) {
    var url_ = url || window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url_);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

StiMobileDesigner.prototype.IsRusCulture = function (culture) {
    return (culture == "ru" || culture == "be" || culture == "uk" || culture == "kz");
}

StiMobileDesigner.prototype.CheckOAuthParameters = function (parameters) {
    if (this.options.oAuthCode || this.options.oAuthState || this.options.oAuthError || this.options.gitHubAuthCode || this.options.gitHubAuthError || this.options.facebookAuthCode || this.options.facebookAuthError) {
        this.openNewWindow('', '_self', '');
        window.close();
        return;
    }
}

StiMobileDesigner.prototype.CheckCloudNotifications = function (params) {
    var jsObject = this;
    if (params.cloudNotificationMaxRefreshes) {
        jsObject.InitializeNotificationForm(function (form) {
            form.show(jsObject.loc.Notices.QuotaMaximumRefreshCountExceeded, jsObject.NotificationMessages("updateYourSubscription"), "Notifications.Warning.png");
            if (jsObject.options.viewer) jsObject.options.viewer.jsObject.controls.processImage.hide();
        });
    }
    else if (params.cloudNotificationMaxDataRows) {
        jsObject.InitializeNotificationForm(function (form) {
            var message = jsObject.loc.Notices.QuotaMaximumDataRowsCountExceeded;
            if (params.cloudNotificationMaxDataRows !== true)
                message += "<br>" + jsObject.loc.PropertyMain.Maximum + " " + jsObject.NumberWithSpaces(params.cloudNotificationMaxDataRows) + ".";
            form.show(message, jsObject.NotificationMessages("updateYourSubscription"), "Notifications.Lines.png");
        });
    }
}

StiMobileDesigner.prototype.ApplyDesignerSpecification = function (designerSpecification) {
    var jsObject = this;
    jsObject.options.designerSpecification = designerSpecification;

    var user = jsObject.options.cloudParameters ? jsObject.options.cloudParameters.user : null;
    var sessionKey = jsObject.options.cloudParameters ? jsObject.options.cloudParameters.sessionKey : null;

    if (jsObject.options.standaloneJsMode) {
        user = jsObject.options.user;
        sessionKey = jsObject.options.SessionKey;
    }

    if (user && sessionKey) {
        user.DesignerSpecification = jsObject.options.designerSpecification;

        if (jsObject.options.standaloneJsMode) {
            jsObject.SendCommand("UserSave", { User: user }, function (data) {
                jsObject.UpdateDesignerControlsBySpecification();
            });
        }
        else {
            jsObject.SendCloudCommand("UserSave", { User: user }, function (data) {
                jsObject.UpdateDesignerControlsBySpecification();
            });
        }
    }
    else {
        jsObject.SetCookie("StimulsoftMobileDesignerDesignerSpecification", jsObject.options.designerSpecification);
        jsObject.UpdateDesignerControlsBySpecification();
    }
}

StiMobileDesigner.prototype.UpdateDesignerControlsBySpecification = function () {
    var jsObject = this;
    var specification = jsObject.options.designerSpecification;

    if (specification == "Auto") {
        jsObject.InitializeWhoAreYouForm(function (form) {
            form.show();
        });
        return;
    }

    if (this.options.propertiesPanel)
        this.options.propertiesPanel.updateBySpecification();

    if (this.options.dictionaryPanel)
        this.options.dictionaryPanel.toolBar.controls.Actions.style.display = specification == "Developer" ? "" : "none";

    if (this.options.buttons.buttonPublish)
        this.options.buttons.buttonPublish.style.display = (this.options.cloudMode || this.options.standaloneJsMode) && specification == "Developer" ? "" : "none";

    if (this.options.dictionaryTree && this.options.dictionaryTree.mainItems)
        this.options.dictionaryTree.mainItems["BusinessObjects"].style.display = specification == "Developer" ? "" : "none";

    if (this.options.insertPanel)
        this.options.insertPanel.update();

    if (this.options.toolbox)
        this.options.toolbox.update();

    if (this.options.buttons.insertBands)
        this.BandsMenu("bandsMenu", this.options.buttons.insertBands);

    if (this.options.buttons.insertComponents)
        this.ComponentsMenu("componentsMenu", this.options.buttons.insertComponents);
};

StiMobileDesigner.prototype.UpdateDesignerSpecification = function () {
    var jsObject = this;
    jsObject.options.designerSpecification = "Developer";

    if (jsObject.options.cloudMode || jsObject.options.standaloneJsMode) {
        //jsObject.options.designerSpecification = jsObject.GetCookie("StimulsoftMobileDesignerDesignerSpecification") || "Auto"; //TO DO

        var user = jsObject.options.cloudParameters ? jsObject.options.cloudParameters.user : null;
        var sessionKey = jsObject.options.cloudParameters ? jsObject.options.cloudParameters.sessionKey : null;

        if (jsObject.options.standaloneJsMode) {
            user = jsObject.options.user;
            sessionKey = jsObject.options.SessionKey;
        }

        if (user && sessionKey) {
            jsObject.options.designerSpecification = user.DesignerSpecification;

            if (jsObject.options.designerSpecification == "Auto") {
                jsObject.options.designerSpecification = jsObject.GetDesignerSpecificationFromUserProducts();
            }
        }
    }
}

StiMobileDesigner.prototype.GetDesignerSpecificationFromUserProducts = function () {
    var products = this.options.cloudParameters ? this.options.cloudParameters.products : null;
    if (products) {
        for (var i = 0; i < products.length; i++) {
            if (this.IsBIProduct(products[i].Ident))
                return "BICreator";
        }
    }
    return "Auto";
}

StiMobileDesigner.prototype.DesignerSpecificationToSkillLevelLoc = function (specification) {
    switch (specification) {
        case "Auto": return this.loc.PropertyEnum.StiDesignerSpecificationAuto;
        case "Beginner": return this.loc.Report.Basic;
        case "BICreator": return this.loc.Report.Standard;
        case "Developer": return this.loc.Report.Professional;
    }
    return specification;
}

StiMobileDesigner.prototype.AddDroppedContainerToCollection = function (container) {
    if (this.options.droppedContainers.indexOf(container) < 0) {
        this.options.droppedContainers.push(container);
    }
}

StiMobileDesigner.prototype.DropDragableItemToActiveContainer = function (item) {
    var eventTouch = this.options.eventTouch;
    if (!eventTouch || !eventTouch.touches || !item) return;

    var page = this.options.currentPage;
    var itemX = eventTouch.touches[0].pageX;
    var itemY = eventTouch.touches[0].pageY;
    var mainPanelX = this.FindPosX(this.options.mainPanel, null, false);
    var mainPanelY = this.FindPosY(this.options.mainPanel, null, false);

    //drop to container
    for (var i = 0; i < this.options.droppedContainers.length; i++) {
        var container = this.options.droppedContainers[i];
        if (container.offsetHeight && container.offsetWidth) {
            var containerX = mainPanelX + this.FindPosX(container, "stiDesignerMainPanel");
            var containerY = mainPanelY + this.FindPosY(container, "stiDesignerMainPanel");
            if (itemX + 5 > containerX && itemX - 5 < containerX + container.offsetWidth &&
                itemY + 5 > containerY && itemY - 5 < containerY + container.offsetHeight) {
                if (container["onmouseup"]) container.onmouseup(eventTouch);
                return;
            }
        }
    }

    if (page && item) {
        var pagePositions = this.FindPagePositions();
        var pageX = mainPanelX + pagePositions.posX;
        var pageY = mainPanelY + pagePositions.posY;

        //drop to dbs element
        for (var i in page.components) {
            var component = page.components[i];
            if (component.isDashboardElement) {
                var compX = pageX + parseInt(component.getAttribute("left"));
                var compY = pageY + parseInt(component.getAttribute("top"));
                var compWidth = parseInt(component.getAttribute("width"));
                var compHeight = parseInt(component.getAttribute("height"));
                if (itemX + 5 > compX && itemX - 5 < compX + compWidth && itemY + 5 > compY && itemY - 5 < compY + compHeight) {
                    if (this.CanDropDictionaryItem(component.typeComponent)) {
                        this.DropDictionaryItemToDashboardElement(component, item, eventTouch);
                        this.options.startInsertDataToElement = true;
                    }
                    return;
                }
            }
        }

        //drop to page
        if (itemX + 5 > pageX && itemX - 5 < pageX + page.widthPx && itemY + 5 > pageY && itemY - 5 < pageY + page.heightPx) {
            page.ontouchend(eventTouch);
        }
    }
}

StiMobileDesigner.prototype.IsBIProduct = function (ident) {
    return (
        ident == "BIDesigner" ||
        ident == "BIDesktop" ||
        ident == "BIServer" ||
        ident == "BICloud" ||
        ident == "CloudReports" ||
        ident == "CloudDashboards"
    );
}

StiMobileDesigner.prototype.GetFirstDayOfWeek = function () {
    var date = new Date();
    var timeString = date.toLocaleTimeString();
    return (timeString.toLowerCase().indexOf("am") >= 0 || timeString.toLowerCase().indexOf("pm") >= 0 ? "Sunday" : "Monday");
}

StiMobileDesigner.prototype.GetDefaultLocalization = function () {
    var defaultLocalization = "en";
    var browserLanguage = navigator.defaultLocalization || navigator.language || navigator.browserLanguage;
    if (browserLanguage) defaultLocalization = browserLanguage.substring(0, 2);
    return defaultLocalization;
}

StiMobileDesigner.prototype.GetOnlyBaseLocalization = function (loc) {
    return (loc && (loc.toLowerCase() == "en" || loc.toLowerCase() == "ru" || loc.toLowerCase() == "de") ? loc : "en");
}

StiMobileDesigner.prototype.NeedToUseNewViewer = function () {
    var report = this.options.report;
    if (this.options.report) {
        for (var pageName in report.pages) {
            for (var componentName in report.pages[pageName].components) {
                var comp = report.pages[pageName].components[componentName];
                if (comp.typeComponent == "StiTableOfContents" ||
                    comp.typeComponent == "StiImage" && comp.properties.icon ||
                    comp.typeComponent == "StiSparkline" ||
                    comp.typeComponent == "StiMathFormula" ||
                    comp.typeComponent == "StiElectronicSignature" ||
                    comp.typeComponent == "StiPdfDigitalSignature") {
                    return true;
                }
            }
        }
    }
    return false;
}

StiMobileDesigner.prototype.isBlocklyValue = function (value) {
    return (value && value.indexOf(this.options.blocklyIdent) == 0)
}

StiMobileDesigner.prototype.getMixingColors = function (color1, color2, alpha) {
    if (alpha == null) alpha = 255;

    var color1Arr = color1 != "transparent" ? color1.split(",") : ["255", "255", "255"];
    if (color1Arr.length == 4) color1Arr.splice(0, 1);
    var color2Arr = color2 != "transparent" ? color2.split(",") : ["255", "255", "255"];
    if (color2Arr.length == 4) color2Arr.splice(0, 1);

    var r = parseInt(color2Arr[0]) * alpha / 255 + parseInt(color1Arr[0]) * (255 - alpha) / 255;
    var g = parseInt(color2Arr[1]) * alpha / 255 + parseInt(color1Arr[1]) * (255 - alpha) / 255;
    var b = parseInt(color2Arr[2]) * alpha / 255 + parseInt(color1Arr[2]) * (255 - alpha) / 255;

    return [r, g, b].join(",");
}

StiMobileDesigner.prototype.getLightColor = function (baseColor, value) {
    var baseColorArr = baseColor != "transparent" ? baseColor.split(",") : ["255", "255", "255"];
    if (baseColorArr.length == 4) baseColorArr.splice(0, 1);

    var r = parseInt(baseColorArr[0]);
    var g = parseInt(baseColorArr[1]);
    var b = parseInt(baseColorArr[2]);

    if (r + value > 255) r = 255;
    else r += value;

    if (g + value > 255) g = 255;
    else g += value;

    if (b + value > 255) b = 255;
    else b += value;

    return [r, g, b].join(",");
}

StiMobileDesigner.prototype.getDarkColor = function (baseColor, value) {
    var baseColorArr = baseColor != "transparent" ? baseColor.split(",") : ["255", "255", "255"];
    if (baseColorArr.length == 4) baseColorArr.splice(0, 1);

    var r = parseInt(baseColorArr[0]);
    var g = parseInt(baseColorArr[1]);
    var b = parseInt(baseColorArr[2]);

    if (r - value < 0) r = 0;
    else r -= value;

    if (g - value < 0) g = 0;
    else g -= value;

    if (b - value < 0) b = 0;
    else b -= value;

    return [r, g, b].join(",");
}

StiMobileDesigner.prototype.isItTooLight = function (color) {
    var colorArr = color != "transparent" ? color.split(",") : ["255", "255", "255"];
    if (colorArr.length == 4) colorArr.splice(0, 1);

    return parseInt(colorArr[0]) > 200 && parseInt(colorArr[1]) > 200 && parseInt(colorArr[2]) > 200;
}

StiMobileDesigner.prototype.isItTooDark = function (color) {
    var colorArr = color != "transparent" ? color.split(",") : ["255", "255", "255"];
    if (colorArr.length == 4) colorArr.splice(0, 1);

    return parseInt(colorArr[0]) < 50 && parseInt(colorArr[1]) < 50 && parseInt(colorArr[2]) < 50;
}

StiMobileDesigner.prototype.autoCreateDataComponent = function () {
    var dataSourcesItem = this.options.dictionaryTree.mainItems.DataSources;
    if (dataSourcesItem && dataSourcesItem.childsContainer.childNodes.length > 0) {
        var dataBaseItem = dataSourcesItem.childsContainer.childNodes[0];
        dataBaseItem.completeBuildTree();
        if (dataBaseItem && dataBaseItem.childsContainer.childNodes.length > 0) {
            var dataSourceItem = dataBaseItem.childsContainer.childNodes[0];
            if (dataSourceItem) {
                var itemObject = dataSourceItem.itemObject;
                var currentPage = this.options.currentPage;

                if (itemObject.typeItem == "DataSource" && currentPage) {
                    if (this.options.currentPage.isDashboard) {
                        this.SendCommandCreateTableElement({ currentParentType: itemObject.typeItem, currentParentName: itemObject.name }, { x: "0", y: "0" }, currentPage.properties.name);
                    }
                    else {
                        this.InitializeCreateDataForm(function (dataForm) {
                            dataForm.show(itemObject, { x: "0", y: "0" }, currentPage.properties.name);
                        });
                    }
                }
            }
        }
    }
}

StiMobileDesigner.prototype.checkFocusedTextControls = function () {
    var el = document.activeElement;

    if (el && el.tagName && (el.tagName.toLowerCase() == "input" || el.tagName.toLowerCase() == "textarea"))
        return true;

    return false;
}

StiMobileDesigner.prototype.SimpleTextContainer = function (text) {
    var cont = document.createElement("div");
    cont.className = "stiDesignerTextContainer";
    if (text) cont.innerText = text;

    return cont;
}

StiMobileDesigner.prototype.isOffice2013Theme = function () {
    return (this.options.theme && this.options.theme.indexOf("Office2013") >= 0)
}

StiMobileDesigner.prototype.isOffice2022Theme = function () {
    return (this.options.theme && this.options.theme.indexOf("Office2022") >= 0)
}

StiMobileDesigner.prototype.allowRoundedControls = function () {
    return this.isOffice2022Theme();
}

StiMobileDesigner.prototype.aggregateFunctionToHumanText = function (func) {
    if (func) {
        if (func.toLowerCase() == "distinctcount") {
            return "Count (Distinct)";
        }
    }

    return func;
}

StiMobileDesigner.prototype.ConstWebColors = [
    ["Transparent", "transparent"],
    ["Black", "0,0,0"],
    ["DimGray", "105,105,105"],
    ["Gray", "128,128,128"],
    ["DarkGray", "169,169,169"],
    ["Silver", "192,192,192"],
    ["LightGray", "211,211,211"],
    ["Gainsboro", "220,220,220"],
    ["WhiteSmoke", "245,245,245"],
    ["White", "255,255,255"],
    ["RosyBrown", "188,143,143"],
    ["IndianRed", "205,92,92"],
    ["Brown", "165,42,42"],
    ["Firebrick", "178,34,34"],
    ["LightCoral", "240,128,12"],
    ["Maroon", "128,0,0"],
    ["DarkRed", "139,0,0"],
    ["Red", "255,0,0"],
    ["Snow", "255,250,250"],
    ["MistyRose", "255,228,225"],
    ["Salmon", "250,128,114"],
    ["Tomato", "255,99,71"],
    ["DarkSalmon", "233,150,122"],
    ["Coral", "255,127,80"],
    ["OrangeRed", "255,69,0"],
    ["LightSalmon", "255,160,122"],
    ["Sienna", "160,82,45"],
    ["SeaShell", "255,245,23"],
    ["Chocolate", "210,105,30"],
    ["SaddleBrown", "139,69,19"],
    ["SandyBrown", "244,164,96"],
    ["PeachPuff", "255,218,185"],
    ["Peru", "205,133,63"],
    ["Linen", "250,240,230"],
    ["Bisque", "255,228,196"],
    ["DarkOrange", "255,140,0"],
    ["BurlyWood", "222,184,135"],
    ["Tan", "210,180,140"],
    ["AntiqueWhite", "250,235,215"],
    ["NavajoWhite", "255,222,173"],
    ["BlanchedAlmond", "255,235,205"],
    ["PapayaWhip", "255,239,213"],
    ["Moccasin", "255,228,181"],
    ["Orange", "255,165,0"],
    ["Wheat", "245,222,179"],
    ["OldLace", "253,245,230"],
    ["FloralWhite", "255,250,240"],
    ["DarkGoldenrod", "184,134,11"],
    ["Goldenrod", "218,165,32"],
    ["Cornsilk", "255,248,220"],
    ["Gold", "255,215,0"],
    ["Khaki", "240,230,140"],
    ["LemonChiffon", "255,250,205"],
    ["PaleGoldenrod", "238,232,170"],
    ["DarkKhaki", "189,183,107"],
    ["Beige", "245,245,220"],
    ["LightGoldenrodYellow", "250,250,210"],
    ["Olive", "128,128,0"],
    ["Yellow", "255,255,0"],
    ["LightYellow", "255,255,224"],
    ["Ivory", "255,255,240"],
    ["OliveDrab", "107,142,35"],
    ["YellowGreen", "154,205,50"],
    ["DarkOliveGreen", "85,107,47"],
    ["GreenYellow", "173,255,47"],
    ["Chartreuse", "127,255,0"],
    ["LawnGreen", "124,252,0"],
    ["DarkSeaGreen", "143,188,139"],
    ["ForestGreen", "34,139,34"],
    ["LimeGreen", "50,205,50"],
    ["LightGreen", "144,238,144"],
    ["PaleGreen", "152,251,152"],
    ["DarkGreen", "0,100,0"],
    ["Green", "0,128,0"],
    ["Lime", "0,255,0"],
    ["Honeydew", "240,255,240"],
    ["SeaGreen", "46,139,87"],
    ["MediumSeaGreen", "60,179,113"],
    ["SpringGreen", "0,255,127"],
    ["MintCream", "245,255,250"],
    ["MediumSpringGreen", "0,250,154"],
    ["MediumAquamarine", "102,205,170"],
    ["Aquamarine", "127,255,212"],
    ["Turquoise", "64,224,20"],
    ["LightSeaGreen", "32,178,170"],
    ["MediumTurquoise", "72,209,204"],
    ["DarkSlateGray", "47,79,79"],
    ["PaleTurquoise", "175,238,23"],
    ["Teal", "0,128,12"],
    ["DarkCyan", "0,139,139"],
    ["Aqua", "0,255,255"],
    ["Cyan", "0,255,255"],
    ["LightCyan", "224,255,255"],
    ["Azure", "240,255,255"],
    ["DarkTurquoise", "0,206,209"],
    ["CadetBlue", "95,158,160"],
    ["PowderBlue", "176,224,230"],
    ["LightBlue", "173,216,230"],
    ["DeepSkyBlue", "0,191,255"],
    ["SkyBlue", "135,206,235"],
    ["LightSkyBlue", "135,206,250"],
    ["SteelBlue", "70,130,180"],
    ["AliceBlue", "240,248,255"],
    ["DodgerBlue", "30,144,255"],
    ["SlateGray", "112,128,144"],
    ["LightSlateGray", "119,136,153"],
    ["LightSteelBlue", "176,196,222"],
    ["CornflowerBlue", "100,149,237"],
    ["RoyalBlue", "65,105,225"],
    ["MidnightBlue", "25,25,112"],
    ["Lavender", "230,230,250"],
    ["Navy", "0,0,12"],
    ["DarkBlue", "0,0,139"],
    ["MediumBlue", "0,0,205"],
    ["Blue", "0,0,255"],
    ["GhostWhite", "248,248,255"],
    ["SlateBlue", "106,90,205"],
    ["DarkSlateBlue", "72,61,139"],
    ["MediumSlateBlue", "123,104,23"],
    ["MediumPurple", "147,112,219"],
    ["BlueViolet", "138,43,226"],
    ["Indigo", "75,0,130"],
    ["DarkOrchid", "153,50,204"],
    ["DarkViolet", "148,0,211"],
    ["MediumOrchid", "186,85,211"],
    ["Thistle", "216,191,216"],
    ["Plum", "221,160,221"],
    ["Violet", "238,130,23"],
    ["Purple", "128,0,12"],
    ["DarkMagenta", "139,0,139"],
    ["Magenta", "255,0,255"],
    ["Fuchsia", "255,0,255"],
    ["Orchid", "218,112,214"],
    ["MediumVioletRed", "199,21,133"],
    ["DeepPink", "255,20,147"],
    ["HotPink", "255,105,180"],
    ["LavenderBlush", "255,240,245"],
    ["PaleVioletRed", "219,112,147"],
    ["Crimson", "220,20,60"],
    ["Pink", "255,192,203"],
    ["LightPink", "255,182,193"]
];