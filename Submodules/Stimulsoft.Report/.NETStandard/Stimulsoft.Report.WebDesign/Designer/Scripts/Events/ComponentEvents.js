
StiMobileDesigner.prototype.CreateComponentEvents = function (component) {
    var jsObject = this;

    //Component Double Click
    component.ondblclick = function (event) {
        var options = jsObject.options;
        this.completeMouseDown = false;
        if (this.completeDblClick) return;
        this.completeDblClick = true;
        var canSelected = (this.properties.restrictions && (this.properties.restrictions == "All" || this.properties.restrictions.indexOf("AllowSelect") >= 0)) ||
            !this.properties.restrictions;
        if (!canSelected) return;
        this.setSelected();
        if (!options.selectedObjects && (!this.isDashboardElement || (component.controls.editDbsButton && (!jsObject.options.currentForm || jsObject.options.currentForm.dockingComponent != component)))) {
            jsObject.ShowComponentForm(this);
        }
        if (options.selectingRect) {
            options.selectingRect.parentPage.removeChild(options.selectingRect);
            options.selectingRect = false;
        }
    }

    //Component Touch Start
    component.ontouchstart = function (event, mouseProcess) {
        var options = jsObject.options;
        if (options.clipboardMode) return;

        var this_ = this;
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);

        if (options.isTouchDevice && event) {
            this.contextMenuTimer = setTimeout(function () {
                jsObject.options.currentPage.showContextMenu(event);
            }, 1500);
        }

        var canSelected = (this.properties.restrictions && (this.properties.restrictions == "All" || this.properties.restrictions.indexOf("AllowSelect") >= 0)) ||
            !this.properties.restrictions;
        if (!canSelected) return;

        if (options.paintPanel.copyStyleMode) {
            jsObject.SetStylePropertiesToComponent(this, options.copyStyleProperties);
            return;
        }

        options.mouseMoved = false;
        options.componentIsTouched = false;

        if (options.isTouchDevice && event) {
            event.preventDefault();
            if (event.touches.length > 1) {
                options.zoomWithTouch = true;
                return;
            }
            options.startMousePos = [event.touches[0].pageX, event.touches[0].pageY];
            options.currentComponent = this;
        }

        if (!options.selectedObjects) {
            if (jsObject.options.currentPage) jsObject.options.currentPage.updateComponentsLevels();
            if (jsObject.options.currentComponent) jsObject.options.currentComponent.setOnTopLevel();
        }

        var selectedObject = options.selectedObject;
        if (selectedObject != null && selectedObject.typeComponent != "StiPage" && selectedObject.typeComponent != "StiReport") {
            selectedObject.changeVisibilityStateResizingIcons(false);
        }

        if (!options.in_resize && !options.drawComponent) {
            if (options.selectedObjects && jsObject.IsContains(options.selectedObjects, options.currentComponent)) {
                var selectedObjects = options.selectedObjects;
                options.in_drag = [selectedObjects, [], [], []];
                for (var i = 0; i < selectedObjects.length; i++) {
                    options.in_drag[1].push(parseInt(selectedObjects[i].getAttribute("left")));
                    options.in_drag[2].push(parseInt(selectedObjects[i].getAttribute("top")));
                    options.in_drag[3].push(selectedObjects[i].getAllChildsComponents());
                }
                if (this.properties.isPrimitiveComponent) {
                    this.controls.background.style.fill = "transparent";
                    this.controls.background.style.stroke = "transparent";
                }
            }
            else if (options.currentComponent) {
                var xPosComponent = parseInt(options.currentComponent.getAttribute("left"));
                var yPosComponent = parseInt(options.currentComponent.getAttribute("top"));
                options.currentComponent.startPosLeft = xPosComponent;
                options.currentComponent.startPosTop = yPosComponent;
                var childs = options.currentComponent.getAllChildsComponents();
                options.in_drag = [options.currentComponent, xPosComponent, yPosComponent, childs];
                for (var name in childs) {
                    childs[name].startPosLeft = parseInt(childs[name].getAttribute("left"));
                    childs[name].startPosTop = parseInt(childs[name].getAttribute("top"));
                }
            }
        }

        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    //Component Touch End
    component.ontouchend = function (event) {
        var this_ = this;
        var options = jsObject.options;
        this.isTouchEndFlag = true;

        clearTimeout(this.isTouchEndTimer);
        clearTimeout(this.contextMenuTimer);

        if (event) event.preventDefault();
        var canSelected = (this.properties.restrictions && (this.properties.restrictions == "All" || this.properties.restrictions.indexOf("AllowSelect") >= 0)) || !this.properties.restrictions;

        if (!canSelected) return;
        if (options.zoomWithTouch) return;

        options.componentIsTouched = true;

        var thisComponent;
        if (options.in_resize) thisComponent = options.in_resize[0];
        if (options.in_drag) thisComponent = options.in_drag[0];

        if (!options.mouseMoved) {
            if (thisComponent && !jsObject.Is_array(thisComponent)) {
                thisComponent.setSelected();
                jsObject.UpdatePropertiesControls();
            }
        }
        else {
            var components = jsObject.Is_array(thisComponent) ? thisComponent : [thisComponent];
            var marginsPx;

            for (var i = 0; i < components.length; i++) {
                jsObject.ApplyComponentSizes(components[i]);
            }

            if (jsObject.Is_array(thisComponent)) {
                jsObject.PaintSelectedLines();
                jsObject.UpdatePropertiesControls();
            }

            if (options.in_resize) {
                var components = [options.in_resize[0]];
                if (options.in_resize.length > 3 && options.in_resize[0].typeComponent != "StiTable") {
                    components = components.concat(options.in_resize[3]);
                }
                jsObject.SendCommandChangeRectComponent(components, "ResizeComponent", null, options.in_resize[1]);
            }

            if (options.in_drag) {
                jsObject.SendCommandChangeRectComponent(options.in_drag[0], "MoveComponent");
            }
        }

        options.in_resize = false;
        options.in_drag = false;
        options.movingCloneComponents = false;
        options.gridOffset = false;

        this.isTouchEndTimer = setTimeout(function () {
            this_.isTouchEndFlag = false;
        }, 1000);
    }

    //Component Touch Move
    component.ontouchmove = function (event) {
        var options = jsObject.options;
        if (event) event.preventDefault();
        if (options.zoomWithTouch) return;

        jsObject.mouseCurrentXPos = event.touches[0].pageX;
        jsObject.mouseCurrentYPos = event.touches[0].pageY;

        var canMoveOrResize = true;

        if (options.report.info.alignToGrid && options.startMousePos) {
            var gridSize = (options.currentPage && options.currentPage.isDashboard
                ? options.currentPage.properties.gridSize
                : options.report.gridSize) * options.report.zoom;

            canMoveOrResize = false;

            if (!options.gridOffset) {
                options.gridOffset = {
                    x: options.startMousePos[0],
                    y: options.startMousePos[1]
                }
            }

            var xCountGridLines = parseInt((options.gridOffset.x - jsObject.mouseCurrentXPos) / gridSize);
            var yCountGridLines = parseInt((options.gridOffset.y - jsObject.mouseCurrentYPos) / gridSize);

            if (Math.abs(xCountGridLines) >= 1 || Math.abs(yCountGridLines) >= 1) {
                canMoveOrResize = true;
                options.mouseMoved = true;

                options.gridOffset.x = options.gridOffset.x - xCountGridLines * gridSize;
                jsObject.mouseCurrentXPos = jsObject.RoundXY(options.gridOffset.x, 0)[0];

                options.gridOffset.y = options.gridOffset.y - yCountGridLines * gridSize;
                jsObject.mouseCurrentYPos = jsObject.RoundXY(options.gridOffset.y, 0)[0];
            }
        }

        if (options.in_drag && canMoveOrResize) {
            jsObject.MoveComponent(jsObject.mouseCurrentXPos, jsObject.mouseCurrentYPos);
        }

        if (options.in_resize && canMoveOrResize)
            if (jsObject.Is_array(options.in_resize[0]))
                jsObject.ResizeComponents(jsObject.mouseCurrentXPos, jsObject.mouseCurrentYPos);
            else
                jsObject.ResizeComponent(jsObject.mouseCurrentXPos, jsObject.mouseCurrentYPos);
    }

    //Component MouseDown
    component.onmousedown = function (event) {
        var options = jsObject.options;
        if (this.isTouchStartFlag) return;

        if (options.controlsIsFocused) {
            options.controlsIsFocused.blur(); //fixed bug when drag&drop component from toolbar
        }

        if ((options.CTRL_pressed || options.SHIFT_pressed) && event && event.button == 2 && options.selectedObjects && jsObject.IsContains(options.selectedObjects, this)) {
            this.ontouchstart(null, true);
            return;
        }

        if (event && event.button != 2 && this.completeMouseDown) {
            component.completeDblMouseDown = true;
            return;
        }

        var canSelected = (this.properties.restrictions && (this.properties.restrictions == "All" || this.properties.restrictions.indexOf("AllowSelect") >= 0)) ||
            !this.properties.restrictions;
        if (!canSelected) return;

        options.startCopyWithCTRL = options.CTRL_pressed && !options.clipboardMode;
        options.currentComponent = this;
        event.preventDefault();
        options.startMousePos = [event.clientX || event.x, event.clientY || event.y];
        this.ontouchstart(null, true);

        component.completeMouseDown = true;
        component.completeDblMouseDown = false;
        component.completeDblClick = false;
        setTimeout(function () { component.completeMouseDown = false; }, 300);
    }

    //Component MouseUp
    component.onmouseup = function (event) {
        var options = jsObject.options;
        if (this.isTouchEndFlag || options.isTouchClick) return;
        options.componentIsTouched = true;
        if (options.CTRL_pressed) options.CTRL_pressed = this;
        if (options.SHIFT_pressed) options.SHIFT_pressed = this;
        if (options.startCopyWithCTRL) options.startCopyWithCTRL = this;

        if (options.itemInDrag && jsObject.options.itemInDrag.itemObject && jsObject.options.itemInDrag.itemObject.typeItem == "Style") {            
            jsObject.ApplyStyleToComponent(this, jsObject.options.itemInDrag.itemObject.name);
            options.mainPanel.removeChild(options.itemInDrag);
            options.itemInDrag = false;
        }

        //drop dictionary item
        if (jsObject.CanDropDictionaryItem(this.typeComponent) && options.itemInDrag) {
            jsObject.DropDictionaryItemToDashboardElement(this, options.itemInDrag, event);
            options.startInsertDataToElement = true;
        }

        if (this.completeDblMouseDown) {
            this.ondblclick();
        }
    }

    if (this.CanDropDictionaryItem(component.typeComponent)) {
        component.onmouseover = function () {
            if (jsObject.options.itemInDrag && jsObject.options.itemInDrag.originalItem.itemObject) {
                var typeItem = jsObject.options.itemInDrag.originalItem.itemObject.typeItem;
                if (typeItem == "Column" || typeItem == "DataSource" || typeItem == "BusinessObject") {
                    component.borderIsSelected = true;
                    for (var i = 0; i < 4; i++) {
                        var border = component.controls.borders[i];
                        border.style.strokeDasharray = "6,4";
                        border.style.strokeWidth = 2;
                        border.style.stroke = jsObject.options.themeColors[jsObject.GetThemeColor()];

                        //fix bug with offset - repaint lines
                        border.setAttribute("x1", parseInt(border.getAttribute("x1")));
                        border.setAttribute("y1", parseInt(border.getAttribute("y1")));
                        border.setAttribute("x2", parseInt(border.getAttribute("x2")));
                        border.setAttribute("y2", parseInt(border.getAttribute("y2")));
                    }
                }
            }
        }

        component.onmouseout = function () {
            if (this.borderIsSelected) {
                jsObject.RepaintBorder(component);
                this.borderIsSelected = false;
            }
        }
    }
}