//Document MouseUp
StiMobileDesigner.prototype.DocumentMouseUp = function (evnt) {
    this.options.formInDrag = false;
    this.options.currentComponent = null;
    this.options.startCopyWithCTRL = false;

    if (this.options.itemInDrag) {
        this.options.mainPanel.removeChild(this.options.itemInDrag);
        this.options.itemInDrag = false;
    }

    if (this.options.componentButtonInDrag) {
        this.options.mainPanel.removeChild(this.options.componentButtonInDrag);
        this.options.componentButtonInDrag = false;
    }

    if (this.options.unplacedElementInDrag) {
        this.options.mainPanel.removeChild(this.options.unplacedElementInDrag);
        this.options.unplacedElementInDrag = false;
    }

    if (this.options.currentPage && this.options.selectingRect && evnt.button != 2) {
        this.MultiSelectComponents(this.options.currentPage);
        this.options.startPoint = false;
    }

    if (this.options.currentPage && this.options.cursorRect) {
        this.options.currentPage.removeChild(this.options.cursorRect);
        this.options.cursorRect = false;
        this.options.startPoint = false;
        if (this.options.insertPanel) this.options.insertPanel.resetChoose();
        if (this.options.toolbox) this.options.toolbox.resetChoose();
        this.options.selectedObject.setSelected();
    }

    if (this.options.controls.zoomScale) this.options.controls.zoomScale.button.ontouchend();
    if (this.options.movingSliderControl) this.options.movingSliderControl.sladerButton.ontouchend();
    if ((this.options.in_resize || this.options.in_drag) && !this.options.drawComponent) {
        var thisComponent;
        if (this.options.in_resize) thisComponent = this.options.in_resize[0];
        if (this.options.in_drag) thisComponent = this.options.in_drag[0];

        var addOrRemovingComponent = this.options.CTRL_pressed || this.options.SHIFT_pressed || this.options.startCopyWithCTRL;
        if (addOrRemovingComponent && !this.options.in_resize) {
            if (!(this.options.mouseMoved &&
                ((this.options.selectedObjects && this.IsContains(this.options.selectedObjects, addOrRemovingComponent)) ||
                    (this.options.selectedObject && this.options.selectedObject == addOrRemovingComponent)))) {

                this.DeleteSelectedLines();
                if (addOrRemovingComponent.typeComponent) {
                    if (this.Is_array(thisComponent)) {
                        if (this.IsContains(thisComponent, addOrRemovingComponent)) {
                            this.RemoveElementFromArray(thisComponent, addOrRemovingComponent);
                            if (thisComponent.length == 1) thisComponent = thisComponent[0];
                        }
                    }
                    else {
                        if (this.options.selectedObjects) {
                            this.options.selectedObjects.push(addOrRemovingComponent);
                            thisComponent = this.options.selectedObjects;
                        }
                        else {
                            if (this.options.selectedObject && this.options.selectedObject.typeComponent != "StiPage" &&
                                this.options.selectedObject.typeComponent != "StiReport" && this.options.selectedObject != addOrRemovingComponent) {
                                thisComponent = [this.options.selectedObject];
                                thisComponent.push(addOrRemovingComponent);
                                this.options.selectedObject = null;
                                this.options.selectedObjects = thisComponent;
                            }
                        }
                    }
                    this.PaintSelectedLines();
                    this.UpdatePropertiesControls();
                }
            }
        }
        var allowUpdateProperties = true;

        if (!this.Is_array(thisComponent)) {
            if (thisComponent == this.options.selectedObject) allowUpdateProperties = false;

            var forms = this.options.forms;

            if ((forms.editChartSeriesForm && forms.editChartSeriesForm.visible) ||
                (forms.editChartStripsForm && forms.editChartStripsForm.visible) ||
                (forms.editChartConstantLinesForm && forms.editChartConstantLinesForm.visible) ||
                (forms.editChartSimpleForm && forms.editChartSimpleForm.visible) ||
                (this.options.currentForm && this.options.currentForm.visible && this.options.currentForm.isNotModal)) {
                allowUpdateProperties = true;
            }

            if (forms.styleDesignerForm && forms.styleDesignerForm.visible) {
                forms.styleDesignerForm.changeModalState(true);
            }

            thisComponent.setSelected();
        }

        if (!this.options.mouseMoved) {
            if (allowUpdateProperties) this.UpdatePropertiesControls();
        }
        else {
            var components = this.Is_array(thisComponent) ? thisComponent : [thisComponent];
            var marginsPx;

            for (var i = 0; i < components.length; i++) {
                if (!marginsPx) {
                    var pageName = components[i].properties.pageName;
                    var page = this.options.report.pages[pageName];
                    if (!page) continue;
                    marginsPx = page.marginsPx;
                }
                var marginLeftPx = marginsPx[0];
                var marginTopPx = marginsPx[1];

                var leftPx = this.StrToDouble(components[i].getAttribute("left"));
                var topPx = this.StrToDouble(components[i].getAttribute("top"));

                var leftProperty = leftPx - marginLeftPx;
                var topProperty = topPx - marginTopPx;

                components[i].properties.unitLeft = this.ConvertPixelToUnit(leftProperty / this.options.report.zoom, components[i].isDashboardElement);
                components[i].properties.unitTop = this.ConvertPixelToUnit(topProperty / this.options.report.zoom, components[i].isDashboardElement);

                if (this.options.in_resize) {
                    var widthProperty = this.StrToDouble(components[i].getAttribute("width"));
                    var heightProperty = this.StrToDouble(components[i].getAttribute("height"));

                    components[i].properties.unitWidth = this.ConvertPixelToUnit(widthProperty / this.options.report.zoom, components[i].isDashboardElement);
                    components[i].properties.unitHeight = this.ConvertPixelToUnit(heightProperty / this.options.report.zoom, components[i].isDashboardElement);
                }
            }

            if (this.Is_array(thisComponent) && !this.options.clipboardMode) {
                this.PaintSelectedLines();
                this.UpdatePropertiesControls();
            }

            this.options.clipboardMode = false;

            if (this.options.in_resize) {
                var components = this.Is_array(this.options.in_resize[0]) ? this.options.in_resize[0] : [this.options.in_resize[0]];
                if (!this.Is_array(this.options.in_resize[0]) && this.IsTableCell(this.options.in_resize[0])) {
                    components = components.concat(this.options.in_resize[3]);
                }
                this.SendCommandChangeRectComponent(components, "ResizeComponent", null, this.options.in_resize[1]);
            }

            if (this.options.in_drag) {
                this.SendCommandChangeRectComponent(this.options.in_drag[0], "MoveComponent");
            }
        }
    }

    this.options.in_resize = false;
    this.PasteCurrentClipboardComponent();
    this.options.in_drag = false;
    this.options.movingCloneComponents = false;
    this.options.gridOffset = false;
    if (this.options.currentPage) this.options.currentPage.style.cursor = "";
}

//Document TouchEnd
StiMobileDesigner.prototype.DocumentTouchEnd = function (event) {
    this.options.currentComponent = null;
    this.options.clipboardMode = false;

    if (this.options.itemInDrag) {
        this.options.mainPanel.removeChild(this.options.itemInDrag);
        this.options.itemInDrag = false;
    }
    if (this.options.componentButtonInDrag) {
        this.options.mainPanel.removeChild(this.options.componentButtonInDrag);
        this.options.componentButtonInDrag = false;
    }
    if (this.options.unplacedElementInDrag) {
        this.options.mainPanel.removeChild(this.options.unplacedElementInDrag);
        this.options.unplacedElementInDrag = false;
    }
    if (this.options.currentPage && this.options.selectingRect) {
        this.MultiSelectComponents(this.options.currentPage);
        this.options.startPoint = false;
    }
}

//Document Mouse Move
StiMobileDesigner.prototype.DocumentMouseMove = function (evnt) {
    this.DictionaryItemMove(evnt);
    this.ComponentButtonMove(evnt);
    this.UnplacedElementMove(evnt);

    if (this.options.startPosZoomScaleButton) this.options.controls.zoomScale.ontouchmove(evnt, true);
    if (this.options.formInDrag) this.options.formInDrag[4].move(evnt);
    if (this.options.movingSliderControl) this.options.movingSliderControl.ontouchmove(evnt, true);

    if (this.options.in_resize || this.options.in_drag) {
        this.options.mouseMoved = true;

        this.mouseCurrentXPos = evnt.clientX || evnt.x;
        this.mouseCurrentYPos = evnt.clientY || evnt.y;

        if (this.options.startMousePos && this.mouseCurrentXPos == this.options.startMousePos[0] && this.mouseCurrentYPos == this.options.startMousePos[1])
            this.options.mouseMoved = false; //fixed bug IE

        var canMoveOrResize = true;

        if (this.options.report.info.alignToGrid && this.options.startMousePos) {
            var gridSize = (this.options.currentPage && this.options.currentPage.isDashboard
                ? this.options.currentPage.properties.gridSize
                : this.options.report.gridSize) * this.options.report.zoom;

            canMoveOrResize = false;

            if (!this.options.gridOffset) {
                this.options.gridOffset = {
                    x: this.options.startMousePos[0],
                    y: this.options.startMousePos[1]
                }
            }

            var xCountGridLines = parseInt((this.options.gridOffset.x - this.mouseCurrentXPos) / gridSize);
            var yCountGridLines = parseInt((this.options.gridOffset.y - this.mouseCurrentYPos) / gridSize);

            if (Math.abs(xCountGridLines) >= 1 || Math.abs(yCountGridLines) >= 1) {
                canMoveOrResize = true;

                this.options.gridOffset.x = this.options.gridOffset.x - xCountGridLines * gridSize;
                this.mouseCurrentXPos = this.RoundXY(this.options.gridOffset.x, 0)[0];

                this.options.gridOffset.y = this.options.gridOffset.y - yCountGridLines * gridSize;
                this.mouseCurrentYPos = this.RoundXY(this.options.gridOffset.y, 0)[0];
            }
        }

        if (this.options.in_drag && canMoveOrResize) {
            if ((this.options.CTRL_pressed || this.options.startCopyWithCTRL) && !this.options.clipboardMode)
                this.MoveCopyComponent(this.mouseCurrentXPos, this.mouseCurrentYPos);
            else
                this.MoveComponent(this.mouseCurrentXPos, this.mouseCurrentYPos);
        }

        if (this.options.in_resize && canMoveOrResize) {
            if (this.Is_array(this.options.in_resize[0]))
                this.ResizeComponents(this.mouseCurrentXPos, this.mouseCurrentYPos);
            else
                this.ResizeComponent(this.mouseCurrentXPos, this.mouseCurrentYPos);
        }
    }

    var styleForm = this.options.forms.styleDesignerForm;
    if (this.options.itemInDrag && styleForm && styleForm.visible) {
        styleForm.changeModalState(false);
    }
}

//Document Touch Move
StiMobileDesigner.prototype.DocumentTouchMove = function (evnt) {
    this.DictionaryItemMove(evnt);
    this.ComponentButtonMove(evnt);
    this.UnplacedElementMove(evnt);
}

StiMobileDesigner.prototype.DictionaryItemMove = function (evnt) {
    if (this.options.itemInDrag) {
        if (this.options.itemInDrag.beginingOffset < 10) {
            this.options.itemInDrag.beginingOffset++;
        }
        else {
            this.options.itemInDrag.move(evnt);
            this.options.eventTouch = evnt;

            if (this.options.isTouchDevice) {
                this.OverOrOutEventsForDroppedContainers(evnt)
            }
        }
    }
}

StiMobileDesigner.prototype.ComponentButtonMove = function (evnt) {
    if (this.options.componentButtonInDrag) {
        if (this.options.componentButtonInDrag.beginingOffset < 10) {
            this.options.componentButtonInDrag.beginingOffset++;
        }
        else {
            this.options.componentButtonInDrag.move(evnt);
            this.options.eventTouch = evnt;
        }
    }
}

StiMobileDesigner.prototype.UnplacedElementMove = function (evnt) {
    if (this.options.unplacedElementInDrag) {
        if (this.options.unplacedElementInDrag.beginingOffset < 10) {
            this.options.unplacedElementInDrag.beginingOffset++;
        }
        else {
            this.options.unplacedElementInDrag.move(evnt);
            this.options.eventTouch = evnt;
        }
    }
}

StiMobileDesigner.prototype.OverOrOutEventsForDroppedContainers = function (evnt) {
    if (evnt.touches && evnt.touches.length > 0) {
        var itemX = evnt.touches[0].pageX;
        var itemY = evnt.touches[0].pageY;
        var mainPanelX = this.FindPosX(this.options.mainPanel, null, false);
        var mainPanelY = this.FindPosY(this.options.mainPanel, null, false);

        for (var i = 0; i < this.options.droppedContainers.length; i++) {
            var container = this.options.droppedContainers[i];
            if (container.offsetHeight && container.offsetWidth) {
                var containerX = mainPanelX + this.FindPosX(container, "stiDesignerMainPanel");
                var containerY = mainPanelY + this.FindPosY(container, "stiDesignerMainPanel");

                if (itemX + 5 > containerX && itemX - 5 < containerX + container.offsetWidth && itemY + 5 > containerY && itemY - 5 < containerY + container.offsetHeight && container.onmouseover)
                    container.onmouseover();
                else if (container.onmouseout)
                    container.onmouseout();
            }
        }
    }
}