// Events
StiMobileDesigner.prototype.CreatePageEvents = function (page) {
    var jsObject = this;

    //Page Touch Start
    page.ontouchstart = function (event, mouseProcess, eventButton) {
        //debugger;
        var thisPage = this;
        var options = jsObject.options;

        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);

        if (options.isTouchDevice && event) {
            this.contextMenuTimer = setTimeout(function () {
                page.showContextMenu(event);
            }, 1500);
        }

        options.pagePressed = true;

        if (options.isTouchDevice && event) {
            options.eventTouch = event;
            options.thisPage = this;
            options.touchZoom.firstDistance = 0;
            options.touchZoom.secondDistance = 0;
            options.touchZoom.zoomStep = 0;
            options.zoomWithTouch = false;

            if (event.touches.length > 1) {
                if (options.selectedObject) options.selectedObject.setSelected();
                options.zoomWithTouch = true;
                options.in_resize = false;
                options.in_drag = false;
                options.movingCloneComponents = false;
                return;
            }
        }

        options.componentIsTouched = false;

        if (eventButton != 2 && options.drawComponent) {
            var selectedObject = options.selectedObject;
            if (selectedObject && selectedObject.typeComponent != "StiPage" && selectedObject.typeComponent != "StiReport")
                options.selectedObject.changeVisibilityStateResizingIcons(false);
            options.eventTouch.preventDefault();
            options.startPoint = jsObject.FindMousePosOnSvgPage(thisPage, options.eventTouch);
            options.currentPoint = options.startPoint;
            options.cursorRect = ("createElementNS" in document) ? document.createElementNS('http://www.w3.org/2000/svg', 'polygon') : document.createElement("polygon");
            options.cursorRect.style.strokeWidth = "1";
            options.cursorRect.style.stroke = page.isDashboard ? jsObject.GetHTMLColor(page.properties.selectionBorderColor) : options.themeColors[jsObject.GetThemeColor()];
            options.cursorRect.style.strokeDasharray = "3,3";
            options.cursorRect.style.fill = "transparent";
            thisPage.appendChild(options.cursorRect);
        }
        else if (eventButton != 2 && !options.in_drag && !options.in_resize && !options.paintPanel.copyStyleMode &&
            !(options.selectedObjects && jsObject.IsContains(options.selectedObjects, options.currentComponent))) {
            options.startPoint = jsObject.FindMousePosOnSvgPage(thisPage, options.eventTouch);
            options.currentPoint = options.startPoint;
            if (options.selectingRect) options.selectingRect.parentPage.removeChild(options.selectingRect);
            options.selectingRect = ("createElementNS" in document) ? document.createElementNS('http://www.w3.org/2000/svg', 'polygon') : document.createElement("polygon");
            options.selectingRect.style.fill = options.themeColors[jsObject.GetThemeColor()];
            options.selectingRect.style.fillOpacity = "0.1";
            options.selectingRect.style.strokeWidth = "1";
            options.selectingRect.style.stroke = page.isDashboard ? jsObject.GetHTMLColor(page.properties.selectionBorderColor) : options.themeColors[jsObject.GetThemeColor()];
            options.selectingRect.parentPage = thisPage;
            thisPage.appendChild(options.selectingRect);
        }

        this.isTouchStartTimer = setTimeout(function () {
            thisPage.isTouchStartFlag = false;
        }, 1000);
    }

    //Page Touch End
    page.ontouchend = function (event, mouseProcess) {
        var thisPage = this;
        var options = jsObject.options;
        this.isTouchEndFlag = mouseProcess ? false : true;

        clearTimeout(this.isTouchEndTimer);
        clearTimeout(this.contextMenuTimer);

        if (options.isTouchDevice) {
            options.thisPage = thisPage;
        }

        options.pageIsTouched = true;
        var point = jsObject.FindMousePosOnSvgPage(thisPage, options.eventTouch);

        thisPage.removingComponents = {};
        var movingCloneComponents = options.movingCloneComponents;

        if (movingCloneComponents) {
            for (var k = 0; k < movingCloneComponents.length; k++) {
                var movingCloneComponent = movingCloneComponents[k];
                thisPage.removingComponents[movingCloneComponent.properties.name] = movingCloneComponent;

                jsObject.ApplyComponentSizes(movingCloneComponent);
                jsObject.SendCommandCreateMovingCopyComponent(
                    movingCloneComponent.properties.name,
                    movingCloneComponent.properties.unitLeft + "!" + movingCloneComponent.properties.unitTop + "!" + movingCloneComponent.properties.unitWidth + "!" + movingCloneComponent.properties.unitHeight,
                    k == movingCloneComponents.length - 1,
                    function (answer) {
                        if (answer.oldComponentNames) {
                            for (var i = 0; i < answer.oldComponentNames.length; i++) {
                                var removingName = answer.oldComponentNames[i];
                                if (thisPage.removingComponents && thisPage.removingComponents[removingName]) {
                                    var removingComponent = thisPage.removingComponents[removingName];

                                    var page = jsObject.options.report.pages[removingComponent.properties.pageName];
                                    page.removeChild(removingComponent);
                                    if (removingComponent.controls && removingComponent.controls.iframeContent) removingComponent.controls.iframeContent.parentNode.removeChild(removingComponent.controls.iframeContent);
                                    for (var i = 0; i < removingComponent.cloneChilds.length; i++) {
                                        page.removeChild(removingComponent.cloneChilds[i]);
                                        if (removingComponent.cloneChilds[i].controls && removingComponent.cloneChilds[i].controls.iframeContent)
                                            try {
                                                removingComponent.cloneChilds[i].controls.iframeContent.parentNode.removeChild(removingComponent.cloneChilds[i].controls.iframeContent);
                                            } catch (e) { };
                                    }

                                    delete thisPage.removingComponents[removingName];
                                }
                            }
                        }
                    }
                );
            }
            options.movingCloneComponents = false;
            options.in_drag = false;
        }

        if (options.componentButtonInDrag) {
            jsObject.SendCommandCreateComponent(thisPage.properties.name, options.componentButtonInDrag.ownerButton.name, point.xUnits + "!" + point.yUnits + "!" + 0 + "!" + 0, { createdByDragged: true });
            options.mobileDesigner.pressedDown();
        }

        if (options.unplacedElementInDrag && options.report) {
            var ownerButton = options.unplacedElementInDrag.ownerButton;
            var itemObject = ownerButton.itemObject;
            var component = options.report.getComponentByName(itemObject.name);
            if (component) {
                if (ownerButton.parentElement) {
                    var elementsCount = ownerButton.parentElement.childNodes.length;
                    if (elementsCount == 1) {
                        if (options.forms.mobileViewComponentsForm && options.forms.mobileViewComponentsForm.visible) {
                            options.forms.mobileViewComponentsForm.changeVisibleState(false);
                        }
                    }
                    else {
                        options.unplacedElementInDrag.ownerButton.parentElement.removeChild(options.unplacedElementInDrag.ownerButton);
                    }
                }
                component.properties.unitLeft = point.xUnits.toString();
                component.properties.unitTop = point.yUnits.toString();
                component.properties.unitWidth = "200";
                component.properties.unitHeight = "200";
                jsObject.SendCommandChangeRectComponent(component, "ResizeComponent", true, null, true);
            }
            options.mobileDesigner.pressedDown();
        }

        if (options.itemInDrag) {
            var itemObject = options.itemInDrag.itemObject;
            if (itemObject.typeItem == "DataSource" || itemObject.typeItem == "BusinessObject") {
                if (this.isDashboard) {
                    if (!options.startInsertDataToElement) {
                        var draggedItem = {
                            currentParentType: itemObject.typeItem,
                            currentParentName: itemObject.name
                        };
                        jsObject.SendCommandCreateTableElement(draggedItem, { x: point.xUnits.toString(), y: point.yUnits.toString() }, thisPage.properties.name);
                    }
                }
                else {
                    jsObject.InitializeCreateDataForm(function (dataFrom) {
                        var currentItemObject = (itemObject.typeItem == "DataSource")
                            ? jsObject.GetDataSourceByNameFromDictionary(itemObject.name)
                            : jsObject.GetBusinessObjectByNameFromDictionary(itemObject.fullName);
                        dataFrom.show(currentItemObject || itemObject, { x: point.xUnits.toString(), y: point.yUnits.toString() }, thisPage.properties.name);
                    });
                }
            }
            else if ((itemObject.typeItem == "Resource" &&
                (itemObject.type == "Image" || itemObject.type == "Map" || itemObject.type == "Report" || itemObject.type == "ReportSnapshot" || itemObject.type == "Rtf")) ||
                (itemObject.typeItem == "Variable" && itemObject.type == "image") ||
                (itemObject.typeItem == "Column" && (itemObject.type == "byte[]" || itemObject.type == "image"))) {
                if (this.isDashboard) {
                    jsObject.SendCommandCreateElementFromResource(itemObject, { x: point.xUnits.toString(), y: point.yUnits.toString() }, thisPage.properties.name);
                }
                else {
                    jsObject.SendCommandCreateComponentFromResource(itemObject, { x: point.xUnits.toString(), y: point.yUnits.toString() }, thisPage.properties.name);
                }
            }
            else if (itemObject.typeItem != "Category" && itemObject.typeItem != "Style") {
                if (this.isDashboard) {
                    if (!options.startInsertDataToElement) {
                        if (itemObject.typeItem == "Variable") {
                            if (itemObject.type == "datetime") {
                                jsObject.SendCommandCreateDatePickerElement(itemObject, { x: point.xUnits.toString(), y: point.yUnits.toString() }, thisPage.properties.name);
                            }
                            else {
                                jsObject.SendCommandCreateComboBoxElement(itemObject, { x: point.xUnits.toString(), y: point.yUnits.toString() }, thisPage.properties.name);
                            }
                        }
                        else if (itemObject.typeItem == "Column" && itemObject.type == "datetime") {
                            var columnParent = jsObject.options.dictionaryTree.getCurrentColumnParent();
                            if (columnParent) {
                                itemObject.currentParentType = columnParent.type;
                                itemObject.currentParentName = (columnParent.type == "BusinessObject") ? jsObject.options.itemInDrag.originalItem.getBusinessObjectFullName() : columnParent.name;
                            }
                            jsObject.SendCommandCreateDatePickerElement(itemObject, { x: point.xUnits.toString(), y: point.yUnits.toString() }, thisPage.properties.name);
                        }
                        else {
                            jsObject.SendCommandCreateTextElement(itemObject, { x: point.xUnits.toString(), y: point.yUnits.toString() }, thisPage.properties.name);
                        }
                    }
                }
                else {
                    jsObject.SendCommandCreateTextComponent(itemObject, { x: point.xUnits.toString(), y: point.yUnits.toString() }, thisPage.properties.name);
                }
            }
        }

        var multiSelect = false;
        if (options.selectingRect && !options.CTRL_pressed) {
            jsObject.MultiSelectComponents(thisPage);
            multiSelect = true;
        }

        if (options.drawComponent) {
            thisPage.removeChild(options.cursorRect);
            options.cursorRect = false;

            var compWidth = Math.abs(options.startPoint.xUnits - options.currentPoint.xUnits)
            var compHeight = Math.abs(options.startPoint.yUnits - options.currentPoint.yUnits)
            var compLeft = (options.startPoint.xUnits < options.currentPoint.xUnits)
                ? options.startPoint.xUnits
                : options.currentPoint.xUnits;
            var compTop = (options.startPoint.yUnits < options.currentPoint.yUnits)
                ? options.startPoint.yUnits
                : options.currentPoint.yUnits;

            var selectedComponent = options.insertPanel && options.insertPanel.selectedComponent
                ? options.insertPanel.selectedComponent
                : (options.toolbox && options.toolbox.selectedComponent ? options.toolbox.selectedComponent : null);

            jsObject.SendCommandCreateComponent(
                thisPage.properties.name,
                selectedComponent.name,
                compLeft + "!" + compTop + "!" + compWidth + "!" + compHeight,
                (selectedComponent.rowCount && selectedComponent.columnCount ? { rowCount: selectedComponent.rowCount, columnCount: selectedComponent.columnCount } : null));

            if (options.insertPanel) options.insertPanel.resetChoose();
            if (options.toolbox) options.toolbox.resetChoose();
            options.startPoint = false;
        }
        else if ((!options.componentIsTouched || multiSelect) && !options.in_resize && !options.in_drag && !options.zoomWithTouch) {
            if (options.selectedObjects) {
                jsObject.PaintSelectedLines();
                if (options.selectingRect) {
                    options.selectingRect.parentPage.removeChild(options.selectingRect);
                    options.selectingRect = false;
                }
            }
            else {
                thisPage.setSelected();
                thisPage.updateComponentsLevels();
            }
            jsObject.UpdatePropertiesControls();
        }

        this.isTouchEndTimer = setTimeout(function () {
            thisPage.isTouchEndFlag = false;
        }, 1000);
    }

    //Page Touch Move
    page.ontouchmove = function (event) {
        var options = jsObject.options;
        if (options.isTouchDevice) {
            if (event && event.touches.length > 1) {
                event.preventDefault();
                options.in_resize = false;
                options.in_drag = false;
                options.movingCloneComponents = false;
                options.touchZoom.zoomStep++;

                if (options.touchZoom.firstDistance == 0)
                    options.touchZoom.firstDistance = Math.sqrt(Math.pow(event.touches[0].pageX - event.touches[1].pageX, 2) + Math.pow(event.touches[0].pageY - event.touches[1].pageY, 2));

                if (options.touchZoom.zoomStep > 7) {
                    options.touchZoom.zoomStep = 0;
                    options.touchZoom.secondDistance = Math.sqrt(Math.pow(event.touches[0].pageX - event.touches[1].pageX, 2) + Math.pow(event.touches[0].pageY - event.touches[1].pageY, 2));
                    if (options.touchZoom.secondDistance > options.touchZoom.firstDistance && (Math.round(options.report.zoom * 10) / 10 < 2))
                        options.report.zoom += 0.1; jsObject.PreZoomPage(options.currentPage);
                    if (options.touchZoom.secondDistance < options.touchZoom.firstDistance && (Math.round(options.report.zoom * 10) / 10 > 0.1))
                        options.report.zoom -= 0.1; jsObject.PreZoomPage(options.currentPage);
                    return;
                }
            }

            if (event) options.eventTouch = event;
            options.thisPage = this;
        }

        if ((options.startPoint && options.drawComponent && options.cursorRect) || options.selectingRect) {
            options.currentPoint = jsObject.FindMousePosOnSvgPage(options.thisPage, options.eventTouch);
            var x1 = options.startPoint.xPixels + options.xOffset;
            var y1 = options.startPoint.yPixels + options.yOffset;
            var x2 = options.currentPoint.xPixels + options.xOffset;
            var y2 = options.currentPoint.yPixels + options.yOffset;
            var rect = options.cursorRect || options.selectingRect;
            var selectedComponent = options.insertPanel && options.insertPanel.selectedComponent
                ? options.insertPanel.selectedComponent
                : (options.toolbox && options.toolbox.selectedComponent ? options.toolbox.selectedComponent : null);

            if (selectedComponent && selectedComponent.name == "StiHorizontalLinePrimitive") y2 = y1;
            if (selectedComponent && selectedComponent.name == "StiVerticalLinePrimitive") x2 = x1;
            if (rect) rect.setAttribute("points", x1 + "," + y1 + " " + x1 + "," + y2 + " " + x2 + "," + y2 + " " + x2 + "," + y1);
        }
    }


    //Page Mouse Down
    page.onmousedown = function (event) {
        var options = jsObject.options;
        if (!this.isTouchStartFlag && !options.isTouchClick) {
            options.eventTouch = event;
            options.thisPage = this;
            options.pagePressed = true;
            this.ontouchstart(null, true, event.button);
        }
    }

    //Page Mouse Up
    page.onmouseup = function (event) {
        var options = jsObject.options;
        if (!this.isTouchEndFlag && !options.isTouchClick) {
            options.thisPage = this;
            this.ontouchend(null, true);

            if (event.button == 2) {
                jsObject.DocumentMouseUp(event);
                page.showContextMenu(event);
                return false;
            }
        }
    }

    page.oncontextmenu = function () {
        return false;
    }

    page.showContextMenu = function (event) {
        var options = jsObject.options;
        var currentObject = options.selectedObject || jsObject.GetCommonObject(options.selectedObjects);
        if (currentObject && currentObject.typeComponent != "StiCrossField") {
            var componentContextMenu = jsObject.InitializeComponentContextMenu();
            var point = jsObject.FindMousePosOnMainPanel(event);
            componentContextMenu.show(point.xPixels + 3, point.yPixels + 3, "Down", "Right");
        }
        return false;
    }

    //Page Mouse Move
    page.onmousemove = function (event) {
        var options = jsObject.options;
        options.eventTouch = event;
        options.thisPage = this;
        this.ontouchmove();

        if (options.selectedObject != null && !options.in_resize && !options.in_drag &&
            (options.selectedObject.typeComponent == "StiPage" || options.selectedObject.typeComponent == "StiReport")) {
            var currentPoint = jsObject.FindMousePosOnSvgPage(options.currentPage, event);
            options.statusPanel.showPositions(currentPoint.xUnits, currentPoint.yUnits);
        }
    }

    //Page Double Click
    page.ondblclick = function (event) {
        jsObject.options.pageIsDblClick = true;
        if (!jsObject.options.componentIsTouched) {
            jsObject.InitializePageSetupForm(function (pageSetupForm) {
                pageSetupForm.changeVisibleState(true);
            });
        }
    }

    page.onmouseover = function () {
        jsObject.options.mouseOverPage = true;
    }

    page.onmouseout = function () {
        jsObject.options.mouseOverPage = false;
    }
}