
StiMobileDesigner.prototype.CreateComponent = function (compObject, isCrossTabField, isDashboardElement) {
    var component = this.CreateSvgElement("g");
    var jsObject = component.jsObject = this;
    component.isCrossTabField = isCrossTabField;
    component.isDashboardElement = isDashboardElement;
    if (!ComponentCollection[compObject.typeComponent]) return null;

    //Set Properties  
    this.CreateComponentProperties(component, compObject);

    //Create Controls   
    component.controls = {};
    this.CreateComponentShadow(component);

    if (component.typeComponent == "StiButtonElement") {
        this.CreateComponentBackgroundGradient(component);
        this.CreateComponentBackgroundHatch(component);
        this.CreateComponentActionEvents(component);
    }

    this.CreateComponentBackGround(component);
    this.CreateComponentImageContent(component);
    this.CreateComponentSvgContent(component);

    if (component.typeComponent == "StiOnlineMapElement")
        this.CreateComponentIframeContent(component);

    if (component.typeComponent == "StiTableOfContents")
        this.CreateTableOfContentsContent(component);

    if (ComponentCollection[component.typeComponent][2] != "none")
        this.CreateComponentHeader(component);

    if (ComponentCollection[component.typeComponent][3] != "none")
        this.CreateComponentNameContent(component);

    if (ComponentCollection[component.typeComponent][6] != "0")
        this.CreateComponentCorners(component);

    if (component.typeComponent == "StiCrossTab")
        this.CreateCrossTabContainer(component);

    if (component.typeComponent == "StiPanelElement") {
        this.CreatePageWaterMarkImage(component);
        this.CreatePageWaterMark(component);
    }

    this.CreateComponentBorder(component);

    if (component.isDashboardElement) {
        this.CreateDashboardElementEditButton(component);
        this.CreateDashboardElementFiltersButton(component);
        this.CreateDashboardElementTopNButton(component);
        this.CreateDashboardElementChangeTypeButton(component);
        this.CreateDashboardElementTitleButton(component);
        this.CreateDashboardElementSortButton(component);
    }

    this.CreateComponentResizingPoints(component);

    //Create Methods
    this.CreateComponentEvents(component);
    component.repaint = function () { jsObject.RepaintComponent(this); }
    component.remove = function () { jsObject.RemoveComponent(this); }
    component.copy = function () { jsObject.CopyComponent(this); }
    component.clone = function () { return jsObject.CloneComponent(this); }
    component.cut = function () { jsObject.CutComponent(this); }
    component.setSelected = function () { jsObject.SetSelectedObject(this); }
    component.rename = function (newName) { jsObject.RenameComponent(this, newName); }
    component.changeVisibilityStateResizingIcons = function (state) { jsObject.ChangeVisibilityStateResizingIcons(this, state); }
    component.setOnTopLevel = function () { jsObject.SetComponentOnTopLevel(this); }
    component.getAllChildsComponents = function () { return jsObject.GetAllChildsComponents(this); }
    component.isEmptyClientRect = function () { return (this.properties.unitLeft == "0" && this.properties.unitTop == "0" && (this.properties.unitWidth == "0" || this.properties.unitHeight == "0")) }

    return component;
}

StiMobileDesigner.prototype.CreateComponentProperties = function (component, compObject) {
    component.properties = {};
    component.properties.name = compObject.name;
    component.typeComponent = compObject.typeComponent;
    var rect = compObject.componentRect.split("!");
    component.properties.unitLeft = rect[0];
    component.properties.unitTop = rect[1];
    component.properties.unitWidth = rect[2];
    component.properties.unitHeight = rect[3];
    component.properties.parentName = compObject.parentName;
    component.properties.parentIndex = compObject.parentIndex;
    component.properties.componentIndex = compObject.componentIndex;
    component.properties.childs = compObject.childs;
    component.properties.svgContent = compObject.svgContent;
    component.properties.pageName = compObject.pageName;
    this.WriteAllProperties(component, compObject.properties);
}

StiMobileDesigner.prototype.CreateComponentResizingPoints = function (component) {
    var jsObject = this;

    var mousePoint = function (isBand) {
        var point = jsObject.CreateSvgElement("rect");
        point.setAttribute("width", isBand ? 5 : 4);
        point.setAttribute("height", isBand ? 5 : 4);
        point.style.fill = isBand ? "#ffffff" : "#696969";
        point.style.stroke = "#696969";

        return point;
    }

    var touchPoint = function (isBand, resizingType) {
        var isCircle = resizingType == "LeftTop" || resizingType == "RightTop" || resizingType == "RightBottom" || resizingType == "LeftBottom";
        var size = 24;
        var radius = 8;
        var radius2 = 5;

        var point = jsObject.CreateSvgElement("svg");
        point.setAttribute("width", size);
        point.setAttribute("height", size);

        var r = jsObject.CreateSvgElement("rect");
        r.setAttribute("width", size);
        r.setAttribute("height", size);
        r.setAttribute("stroke", "transparent");
        r.setAttribute("fill", "transparent");
        point.appendChild(r);

        if (!isBand) {
            var c = jsObject.CreateSvgElement(isCircle ? "circle" : "rect");
            if (isCircle) {
                c.setAttribute("cx", parseInt(size / 2));
                c.setAttribute("cy", parseInt(size / 2));
                c.setAttribute("r", radius);
            }
            else {
                var rectSize = radius * 2 - 2;
                c.setAttribute("x", parseInt((size - rectSize) / 2));
                c.setAttribute("y", parseInt((size - rectSize) / 2));
                c.setAttribute("rx", "2");
                c.setAttribute("ry", "2");
                c.setAttribute("width", rectSize);
                c.setAttribute("height", rectSize);
            }
            c.setAttribute("stroke", "#696969");
            c.setAttribute("fill", "#ffffff");
            c.setAttribute("stroke-width", "0.5");
            point.appendChild(c);
        }

        var c2 = jsObject.CreateSvgElement(isCircle ? "circle" : "rect");
        if (isCircle) {
            c2.setAttribute("cx", parseInt(size / 2));
            c2.setAttribute("cy", parseInt(size / 2));
            c2.setAttribute("r", radius2);
        }
        else {
            var rectSize = radius2 * 2 - 2;
            c2.setAttribute("x", parseInt((size - rectSize) / 2));
            c2.setAttribute("y", parseInt((size - rectSize) / 2));
            c2.setAttribute("width", rectSize);
            c2.setAttribute("height", rectSize);
        }
        c2.setAttribute("stroke", "#696969");
        c2.setAttribute("fill", isBand ? "#ffffff" : "#696969");
        c2.setAttribute("stroke-width", "1");
        c2.isCircle = isCircle;
        point.appendChild(c2);
        point.circle = c2;

        return point;
    }

    var createResizingPoint = function (isBand, resizingType) {
        var point = jsObject.options.isTouchDevice && !component.isCrossTabField ? touchPoint(isBand, resizingType) : mousePoint(isBand);
        point.resizingType = resizingType;
        point.style.display = "none";
        point.component = component;

        if (!isBand) {
            point.style.cursor = jsObject.GetCursorType(resizingType);

            point.onmousedown = function (event) {
                if (this.isTouchProcessFlag || jsObject.options.drawComponent) return;
                event.preventDefault();
                jsObject.options.startMousePos = [event.clientX || event.x, event.clientY || event.y];
                this.ontouchstart(null, true);
            }

            point.ontouchstart = function (event, mouseProcess) {
                var this_ = this;
                this.isTouchProcessFlag = mouseProcess ? false : true;

                if (jsObject.options.drawComponent) return;

                if (event && jsObject.options.isTouchDevice) {
                    event.preventDefault();
                    jsObject.options.startMousePos = [event.touches[0].pageX, event.touches[0].pageY];
                }

                if (jsObject.options.currentPage)
                    jsObject.options.currentPage.style.cursor = this.style.cursor;

                var startValues = {};
                startValues.height = parseInt(component.getAttribute("height"));
                startValues.width = parseInt(component.getAttribute("width"));
                startValues.left = parseInt(component.getAttribute("left"));
                startValues.top = parseInt(component.getAttribute("top"));
                jsObject.options.in_resize = [component, this.resizingType, startValues];

                if (jsObject.IsTableCell(component))
                    jsObject.options.in_resize.push(jsObject.GetAllResizingCells(component, this.resizingType, startValues));
                else if (component.typeComponent == "StiTable")
                    jsObject.options.in_resize.push(jsObject.GetAllResizingCells(component));

                if (jsObject.IsTableCell(component))
                    jsObject.options.in_resize.push(jsObject.GetAllResizingCells(component, this.resizingType, startValues));
                else if (component.typeComponent == "StiTable")
                    jsObject.options.in_resize.push(jsObject.GetAllResizingCells(component));

                setTimeout(function () {
                    this_.isTouchProcessFlag = false;
                }, 1000);
            }
        }

        return point;
    }

    component.controls.resizingPoints = [];
    var resizingType = ["LeftTop", "Top", "RightTop", "Right", "RightBottom", "Bottom", "LeftBottom", "Left"];

    for (var i = 0; i <= 7; i++) {
        if (((!jsObject.IsBandComponent(component) && !jsObject.IsCrossBandComponent(component)) ||
            (i % 2 == 0 && (jsObject.IsBandComponent(component) || jsObject.IsCrossBandComponent(component)))) &&
            (!((i != 3 && i != 7 && component.typeComponent == "StiHorizontalLinePrimitive") ||
                (i != 1 && i != 5 && component.typeComponent == "StiVerticalLinePrimitive")))) {
            var resizingPoint = createResizingPoint(jsObject.IsBandComponent(component) || jsObject.IsCrossBandComponent(component), resizingType[i]);
            component.controls.resizingPoints[i] = resizingPoint;
            component.appendChild(resizingPoint);
        }
        else
            component.controls.resizingPoints[i] = null;
    }

    if (jsObject.IsBandComponent(component)) {
        var pointIndex = component.typeComponent == "StiPageFooterBand" ? 1 : 5;
        component.controls.resizingPoints[pointIndex] = createResizingPoint(false, resizingType[pointIndex]);
        component.appendChild(component.controls.resizingPoints[pointIndex]);
    }

    if (jsObject.IsCrossBandComponent(component)) {
        component.controls.resizingPoints[3] = createResizingPoint(false, resizingType[3]);
        component.appendChild(component.controls.resizingPoints[3]);
    }
}

StiMobileDesigner.prototype.CreateDashboardElementEditButton = function (component) {
    var button = this.ImageSvgButton("Dashboards.BigEdit.png", 30, 24, this.loc.MainMenu.menuEditEdit);
    button.style.visibility = "hidden";
    component.appendChild(button);
    component.controls.editDbsButton = button;

    button.action = function () {
        this.jsObject.ShowComponentForm(component);
    }
}

StiMobileDesigner.prototype.CreateDashboardElementFiltersButton = function (component) {
    var button = this.ImageSvgButton("BigFilter.png", 30, 24, this.loc.PropertyMain.Filter);
    button.style.visibility = "hidden";
    component.appendChild(button);
    component.controls.filtersDbsButton = button;

    button.action = function () {
        this.jsObject.InitializeElementDataFiltersForm(function (form) {
            form.currentElement = component;
            form.changeVisibleState(true);
        });
    }
}

StiMobileDesigner.prototype.CreateDashboardElementChangeTypeButton = function (component) {
    var jsObject = this;
    var button = this.ImageSvgButton("Dashboards.BigChangeType.png", 30, 24, this.loc.ChartRibbon.ChangeType);
    button.style.visibility = "hidden";
    component.appendChild(button);
    component.controls.changeTypeDbsButton = button;

    button.onmousedown = function (event) {
        if (event) event.stopPropagation();
        var menu = jsObject.InitializeChangeDbsElementTypeMenu();
        menu.show(component);
    }
}

StiMobileDesigner.prototype.CreateDashboardElementTopNButton = function (component) {
    var jsObject = this;
    var button = this.ImageSvgButton("Dashboards.BigTopN.png", 30, 24, this.loc.PropertyMain.TopN);
    button.style.visibility = "hidden";
    component.appendChild(button);
    component.controls.topNDbsButton = button;

    button.action = function () {
        jsObject.InitializeTopNForm(true, function (form) {
            form.show(null, null, component);
        });
    }
}

StiMobileDesigner.prototype.CreateDashboardElementToolButton = function (component, buttonName, imageName) {
    var jsObject = this;
    var button = this.CreateSvgElement("svg");
    button.buttonName = buttonName;
    button.setAttribute("height", 20);
    button.setAttribute("width", 20);
    button.style.visibility = "hidden";

    var backRect = this.CreateSvgElement("rect");
    button.backRect = backRect;
    backRect.style.fill = "transparent";
    backRect.style.stroke = "transparent";
    backRect.setAttribute("x", 0);
    backRect.setAttribute("y", 0);
    backRect.setAttribute("height", 20);
    backRect.setAttribute("width", 20);
    button.appendChild(backRect);

    var img = this.CreateSvgElement("image");
    img.setAttribute("height", 16);
    img.setAttribute("width", 16);
    img.setAttribute("x", 2);
    img.setAttribute("y", 2);
    button.appendChild(img);
    button.img = img;
    StiMobileDesigner.setImageSource(img, this.options, imageName);

    button.onmouseover = function (e) {
        backRect.style.fill = "#bebebe";
    };

    button.onmouseout = function (e) {
        if (!button.isSelected) backRect.style.fill = "transparent";
    };

    button.show = function () {
        button.style.visibility = "visible";
    }

    button.hide = function () {
        button.style.visibility = "hidden";
    }

    button.onmousedown = function (event) {
        if (event) event.stopPropagation();
        this.action();
    }

    button.setSelected = function (state) {
        this.isSelected = state;
        backRect.style.fill = state ? "#bebebe" : "transparent";
    }

    button.action = function () { };

    button.componentOnMouseOver = function (e) {
        var isSelected = component == jsObject.options.selectedObject || jsObject.IsContains(jsObject.options.selectedObjects, component);
        if (isSelected) button.show();
    }

    button.componentOnMouseOut = function (e) {
        button.hide();
    }

    this.addEvent(component, 'mouseover', function (e) { button.componentOnMouseOver(e); });
    this.addEvent(component, 'mouseout', function (e) { button.componentOnMouseOut(e); });

    return button;
}

StiMobileDesigner.prototype.CreateDashboardElementSortButton = function (component) {
    var jsObject = this;
    var button = this.CreateDashboardElementToolButton(component, "sort", "Dashboards.Actions.Light.Sort.png");

    button.action = function () {
        var menu = jsObject.InitializeDashboardElementSortMenu();
        menu.show(component);
        this.setSelected(true);
    }

    button.componentOnMouseOver = function (e) {
        var isSelected = component == jsObject.options.selectedObject || jsObject.IsContains(jsObject.options.selectedObjects, component);
        if (isSelected && component.properties.allowUserSorting) {
            button.show();
        }
    }

    button.componentOnMouseOut = function (e) {
        if (!this.isSelected) button.hide();
    }

    component.appendChild(button);
    component.controls.sortButton = button;
}

StiMobileDesigner.prototype.CreateDashboardElementTitleButton = function (component) {
    var button = this.CreateSvgElement("svg");
    button.setAttribute("height", 20);
    button.setAttribute("width", 20);
    button.style.visibility = "hidden";
    var jsObject = this;

    var backRect = this.CreateSvgElement("rect");
    backRect.style.fill = "transparent";
    backRect.style.stroke = "transparent";
    backRect.setAttribute("x", 0);
    backRect.setAttribute("y", 0);
    backRect.setAttribute("height", 20);
    backRect.setAttribute("width", 20);
    button.appendChild(backRect);

    var rect = this.CreateSvgElement("rect");
    rect.style.fill = "transparent";
    rect.style.stroke = "#ababab";
    rect.setAttribute("x", 2.5);
    rect.setAttribute("y", 2.5);
    rect.setAttribute("height", 15);
    rect.setAttribute("width", 15);
    button.rect = rect;
    button.appendChild(rect);

    var innerRect = this.CreateSvgElement("rect");
    innerRect.style.fill = "#ababab";
    innerRect.setAttribute("x", 5);
    innerRect.setAttribute("y", 5);
    innerRect.setAttribute("height", 10);
    innerRect.setAttribute("width", 10);
    innerRect.style.visibility = "hidden";
    button.innerRect = innerRect;
    button.appendChild(innerRect);

    component.appendChild(button);
    component.controls.titleButton = button;

    this.addEvent(button, 'mouseover', function (e) {
        backRect.style.fill = "#bebebe";
    });

    this.addEvent(button, 'mouseout', function (e) {
        backRect.style.fill = "transparent";
    });

    this.addEvent(component, 'mouseover', function (e) {
        var isSelected = component == jsObject.options.selectedObject || jsObject.IsContains(jsObject.options.selectedObjects, component);
        if (isSelected) {
            button.show();
        }
    });

    this.addEvent(component, 'mouseout', function (e) {
        button.hide();
    });

    button.show = function () {
        button.style.visibility = "visible";
        innerRect.style.visibility = component.properties.titleVisible ? "visible" : "hidden";
    }

    button.hide = function () {
        button.style.visibility = "hidden";
        innerRect.style.visibility = "hidden";
    }

    button.onmouseup = function () {
        component.properties.titleVisible = !component.properties.titleVisible;
        innerRect.style.visibility = component.properties.titleVisible ? "visible" : "hidden";
        jsObject.SendCommandSendProperties([component], ["titleVisible"]);
    }
}

StiMobileDesigner.prototype.CreateComponentActionEvents = function (component) {
    var jsObject = this;

    this.addEvent(component, 'mouseover', function (e) {
        component.isOver = true;
        jsObject.RepaintDbsElementBackGround(component);
        jsObject.RepaintButtonElementContent(component);
    });

    this.addEvent(component, 'mouseout', function (e) {
        component.isOver = false;
        jsObject.RepaintDbsElementBackGround(component);
        jsObject.RepaintButtonElementContent(component);
    });
}

StiMobileDesigner.prototype.CreateComponentCorners = function (component) {
    component.controls.corners = [];
    for (var i = 0; i < 4; i++) {
        var corner = this.CreateSvgElement("polyline");
        corner.setAttribute("fill", "none");
        corner.setAttribute("stroke-width", "0,1px");
        corner.setAttribute("stroke", "black");
        component.controls.corners[i] = corner;
        component.appendChild(corner);
    }
}

StiMobileDesigner.prototype.CreateComponentBorder = function (component) {
    component.controls.borders = [];
    for (var i = 0; i < 8; i++) {
        component.controls.borders[i] = this.CreateSvgElement("line");
        component.appendChild(component.controls.borders[i]);
    }
}

StiMobileDesigner.prototype.CreateComponentBackGround = function (component) {
    var backGround = this.CreateSvgElement("rect");
    backGround.style.stroke = "transparent";
    component.controls.background = backGround;
    component.appendChild(backGround);

    if (component.isDashboardElement) {
        component.controls.backPathes = [];

        for (var i = 0; i < 5; i++) {
            var path = this.CreateSvgElement("path");
            component.controls.backPathes.push(path);
            component.appendChild(path);
        }
    }
}

// Background Gradient
StiMobileDesigner.prototype.AddGradientBrushToElement = function (element) {
    var jsObject = this;
    var grad = this.CreateSvgElement("linearGradient");
    element.appendChild(grad);
    var gradId = "gradient" + this.generateKey();
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
    grad.rect.setAttribute("x", "0");
    grad.rect.setAttribute("y", "0");
    grad.rect.setAttribute("width", "100%");
    grad.rect.setAttribute("height", "100%");
    grad.rect.setAttribute("fill", "url(#" + gradId + ")");
    grad.rect.style.display = "none";
    element.appendChild(grad.rect);
    grad.path = this.CreateSvgElement("path");
    grad.path.setAttribute("fill", "url(#" + gradId + ")");
    grad.path.style.display = "none";
    element.appendChild(grad.path);

    grad.applyBrush = function (brushArray, isRounded) {
        if (brushArray && brushArray.length >= 3) {
            grad.stop1.setAttribute("stop-color", jsObject.GetHTMLColor(brushArray[1]));

            if (brushArray[0] == "3") {
                if (grad.stop2.parentNode) {
                    grad.stop2.parentNode.removeChild(grad.stop2);
                }
                grad.stop3.setAttribute("stop-color", jsObject.GetHTMLColor(brushArray[2]));
            }
            else {
                grad.stop2.setAttribute("stop-color", jsObject.GetHTMLColor(brushArray[2]));
                grad.insertBefore(grad.stop2, grad.stop3);
                grad.stop3.setAttribute("stop-color", jsObject.GetHTMLColor(brushArray[1]));
            }

            var angle = jsObject.StrToInt(brushArray[3]) - 180;
            var pi = angle * (Math.PI / 180);
            var x1 = parseInt(50 + Math.cos(pi) * 50);
            var y1 = parseInt(50 + Math.sin(pi) * 50);
            var x2 = parseInt(50 + Math.cos(pi + Math.PI) * 50);
            var y2 = parseInt(50 + Math.sin(pi + Math.PI) * 50);

            grad.setAttribute("x1", x1 + "%");
            grad.setAttribute("y1", y1 + "%");
            grad.setAttribute("x2", x2 + "%");
            grad.setAttribute("y2", y2 + "%");

            if (isRounded)
                grad.path.style.display = "";
            else
                grad.rect.style.display = "";
        }
    }

    return grad;
}

StiMobileDesigner.prototype.CreateComponentBackgroundGradient = function (component) {
    var grad = this.AddGradientBrushToElement(component);
    component.controls.gradient = grad;
}

StiMobileDesigner.prototype.CreateComponentBackgroundHatch = function (component) {
    var svgHatchBrush = this.CreateSvgElement("svg");
    component.controls.svgHatchBrush = svgHatchBrush;
    component.appendChild(svgHatchBrush);

    svgHatchBrush.clear = function () {
        while (this.childNodes[0]) {
            this.removeChild(this.childNodes[0]);
        }
    }
}

StiMobileDesigner.prototype.CreateComponentHeader = function (component) {
    var header = this.CreateSvgElement("rect");
    component.controls.header = header;
    component.appendChild(header);
}

StiMobileDesigner.prototype.CreateComponentNameContent = function (component) {
    var nameContent = this.CreateSvgElement("svg");
    component.controls.nameContent = nameContent;
    component.appendChild(nameContent);

    var nameText = this.CreateSvgElement("text");
    nameText.style.fontFamily = "Arial";
    nameText.style.fill = "black";
    component.controls.nameText = nameText;
    nameContent.appendChild(nameText);
}

StiMobileDesigner.prototype.CreateTableOfContentsContent = function (component) {
    var content = this.CreateSvgElement("svg");
    component.controls.tableOfContentsContent = content;
    component.appendChild(content);
}

StiMobileDesigner.prototype.CreateComponentImageContent = function (component) {
    var parentImageContent = this.CreateSvgElement("svg");
    var imageContent = this.CreateSvgElement("image");
    parentImageContent.appendChild(imageContent);
    component.controls.imageContent = imageContent;
    component.controls.parentImageContent = parentImageContent;
    component.appendChild(parentImageContent);
}

StiMobileDesigner.prototype.CreateComponentSvgContent = function (component) {
    var svgContent = this.CreateSvgElement("svg");
    component.controls.svgContent = svgContent;
    component.appendChild(svgContent);

    svgContent.clear = function () {
        while (this.childNodes[0]) {
            this.removeChild(this.childNodes[0]);
        }
    }
}

StiMobileDesigner.prototype.CreateComponentIframeContent = function (component) {
    var parentImageContent = this.CreateSvgElement("svg");
    var mapImageContent = this.CreateSvgElement("image");
    parentImageContent.appendChild(mapImageContent);
    component.controls.mapTileContent = mapImageContent;

    mapImageContent = this.CreateSvgElement("image");
    parentImageContent.appendChild(mapImageContent);
    component.controls.mapLabelContent = mapImageContent;

    component.controls.parentMapImageContent = parentImageContent;
    component.appendChild(parentImageContent);

    var iframeContent = document.createElement("iframe");
    iframeContent.style.position = "absolute";
    iframeContent.style.pointerEvents = "none";
    iframeContent.style.border = "none";
    this.options.paintPanel.appendChild(iframeContent);
    component.controls.iframeContent = iframeContent;
}

StiMobileDesigner.prototype.CreateComponentShadow = function (component) {
    var shadow = this.CreateSvgElement("rect");
    component.controls.shadow = shadow;
    component.appendChild(shadow);
}

StiMobileDesigner.prototype.CreateCrossTabContainer = function (component) {
    var crossTabContainer = this.CreateSvgElement("svg");
    component.controls.crossTabContainer = crossTabContainer;
    component.appendChild(crossTabContainer);
}

StiMobileDesigner.prototype.CreateCrossTabFieldComponent = function (compObject, inCrossTabForm) {
    var component = this.CreateComponent(compObject, true);
    component.inCrossTabForm = inCrossTabForm;

    component.ontouchmove = null;
    component.onmouseup = null;
    component.ondblclick = null;

    //Override
    for (var i = 0; i <= 7; i++) {
        var resizingPoint = component.controls.resizingPoints[i];
        if (resizingPoint) {
            resizingPoint.onmousedown = null;
            resizingPoint.style.fill = "red";
            resizingPoint.style.strokeWidth = "red";
            resizingPoint.style.stroke = "red";
            resizingPoint.style.cursor = "default";
        }
    }

    component.onmousedown = function (event) {
        if (this.isTouchStartFlag) return;
        if (event) {
            event.preventDefault();
            event.stopPropagation();
            component.jsObject.options.mobileDesigner.pressedDown();
        }
        this.action();
    }

    component.ontouchstart = function () {
        var this_ = this;
        this.isTouchStartFlag = true;
        this.action();
        if (event) {
            event.preventDefault();
            event.stopPropagation();
        }
        clearTimeout(this.isTouchStartTimer);
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    if (inCrossTabForm) {
        component.setSelected = function (state) {
            if (!state) {
                this.jsObject.ChangeVisibilityStateResizingIcons(this, false);
                if (this == this.jsObject.options.selectedCrossTabField) this.jsObject.options.selectedCrossTabField = null;
                return;
            }
            if (this.jsObject.options.selectedCrossTabField) this.jsObject.options.selectedCrossTabField.setSelected(false);
            this.jsObject.options.selectedCrossTabField = this;
            this.jsObject.ChangeVisibilityStateResizingIcons(this, true);
            this.parentContainer.removeChild(this);
            this.parentContainer.appendChild(this);
        }

        component.action = function () {
            this.setSelected(true);
            if (!this.jsObject.options.propertiesPanel.editCrossTabMode) {
                this.jsObject.options.propertiesPanel.setEditCrossTabMode(true);
            }
            if (this.jsObject.options.propertiesPanel.editCrossTabPropertiesPanel) {
                this.jsObject.options.propertiesPanel.editCrossTabPropertiesPanel.updateProperties(this.properties);
            }
        }
    }
    else {
        component.action = function () {
            this.setSelected(true);

            var propertiesPanel = this.jsObject.options.propertiesPanel;
            propertiesPanel.returnToPanel = null;
            propertiesPanel.mainPropertiesPanel.style.display = "none";
            propertiesPanel.propertiesToolBar.changeVisibleState(false);

            if (propertiesPanel.eventsMode) propertiesPanel.setEventsMode(false);

            if (!propertiesPanel.editCrossTabPropertiesPanel) {
                propertiesPanel.editCrossTabPropertiesPanel = propertiesPanel.jsObject.CrossTabPropertiesPanel();
                propertiesPanel.containers["Properties"].appendChild(propertiesPanel.editCrossTabPropertiesPanel);
            }
            propertiesPanel.editCrossTabPropertiesPanel.style.display = "";

            if (this.jsObject.options.propertiesPanel.editCrossTabPropertiesPanel) {
                this.jsObject.options.propertiesPanel.editCrossTabPropertiesPanel.updateProperties(this.properties);
            }
            this.jsObject.UpdatePropertiesControls();
        }
    }



    return component;
}