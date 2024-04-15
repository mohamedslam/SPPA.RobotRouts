
StiMobileDesigner.prototype.ChangeVisibilityStateResizingIcons = function (component, state) {
    var jsObject = this;
    var controls = component.controls;
    var editDbsButton = controls.editDbsButton;
    var filtersDbsButton = controls.filtersDbsButton;
    var changeTypeDbsButton = controls.changeTypeDbsButton;
    var topNDbsButton = controls.topNDbsButton;

    if (component.controls.resizingPoints) {
        for (var i = 0; i <= 7; i++) {
            var resizingPoint = component.controls.resizingPoints[i];
            if (resizingPoint) {
                resizingPoint.style.display = state ? "" : "none";

                if (this.options.isTouchDevice) {
                    var compWidth = parseInt(component.getAttribute("width"));
                    var compHeight = parseInt(component.getAttribute("height"));
                    if ((compWidth < 70 && (i == 1 || i == 5)) || (compHeight < 70 && (i == 3 || i == 7))) {
                        resizingPoint.style.display = "none"
                    }
                }
            }
        }
    }

    var changeButtonsStates = function (state) {
        var visibility = state ? "visible" : "hidden";

        if (editDbsButton) editDbsButton.style.visibility = visibility;
        if (filtersDbsButton) filtersDbsButton.style.visibility = visibility;
        if (changeTypeDbsButton) changeTypeDbsButton.style.visibility = visibility;
        if (topNDbsButton) topNDbsButton.style.visibility = visibility;

        if (jsObject.options.currentForm && jsObject.options.currentForm.visible && jsObject.options.currentForm.dockingComponent == component) {
            if (editDbsButton) editDbsButton.style.visibility = "hidden";
            if (filtersDbsButton) filtersDbsButton.style.visibility = "hidden";
            if (changeTypeDbsButton) changeTypeDbsButton.style.visibility = "hidden";
            if (topNDbsButton) topNDbsButton.style.visibility = "hidden";
        }
    }

    if (editDbsButton || filtersDbsButton || changeTypeDbsButton || topNDbsButton) {
        if (state) {
            component.selTimer = setTimeout(function () { changeButtonsStates(true); }, 250);
        }
        else {
            clearTimeout(component.selTimer);
            changeButtonsStates(false);
        }
    }

    if (jsObject.options.isTouchDevice) {
        var page = jsObject.options.report.pages[component.properties.pageName];
        if (page && page.controls.componentButtonsPanel) {
            if (state)
                page.controls.componentButtonsPanel.show(component);
            else
                page.controls.componentButtonsPanel.hide();
        }
    }
}

StiMobileDesigner.prototype.FindAllChilds = function (component, allChilds) {
    var childsStr = component.properties.childs;
    if (childsStr) {
        var childs = childsStr.split(",");
        for (var indexChild = 0; indexChild < childs.length; indexChild++) {
            var child = this.options.report.pages[component.properties.pageName].components[childs[indexChild]];
            if (child) {
                child.startPosX = child.getAttribute("left");
                child.startPosY = child.getAttribute("top");
                allChilds[childs[indexChild]] = child;
                this.FindAllChilds(child, allChilds);
            }
        }
    }
    return allChilds;
}

StiMobileDesigner.prototype.GetAllChildsComponents = function (component) {
    var childs = {};
    this.FindAllChilds(component, childs);
    return childs;
}

StiMobileDesigner.prototype.SetComponentOnTopLevel = function (component) {
    var page = this.options.report.pages[component.properties.pageName];
    page.removeChild(component);
    page.appendChild(component);

    var allChilds = component.getAllChildsComponents();
    var parentIndex = parseInt(component.properties.parentIndex) + 1;

    do {
        var flag = false;
        for (var childName in allChilds) {
            var child = allChilds[childName];
            if (child.properties.parentIndex == parentIndex) {
                page.removeChild(child);
                page.appendChild(child);
                flag = true;
            }
        }
        parentIndex++;
    }
    while (flag)
}

StiMobileDesigner.prototype.ResizeComponents = function (mouseCurrentXPos, mouseCurrentYPos) {
    var resizingType = this.options.in_resize[1];
    var deltaX = mouseCurrentXPos - this.options.startMousePos[0];
    var deltaY = mouseCurrentYPos - this.options.startMousePos[1];
    var fixedComponents = this.options.in_resize[3];
    var selectArea = this.options.in_resize[4];
    var selectAreaWidth = selectArea.right - selectArea.left;
    var selectAreaHeight = selectArea.bottom - selectArea.top;
    var pageMarginsPx = this.options.currentPage.marginsPx;

    for (var i = 0; i < this.options.in_resize[0].length; i++) {
        var component = this.options.in_resize[0][i];
        var startValues = this.options.in_resize[2][i];

        var canResize = !component.properties.locked && (
            (component.properties.restrictions && (component.properties.restrictions == "All" || component.properties.restrictions.indexOf("AllowResize") >= 0)) ||
            !component.properties.restrictions);

        if (this.IsTableCell(component) || component.typeComponent == "StiTable" || !canResize) continue;

        var newWidth = startValues.width;
        var newHeight = startValues.height;
        var newLeft = startValues.left;
        var newTop = startValues.top;

        switch (resizingType) {
            case "MultiLeftTop":
                {
                    var scaleX = deltaX < 0 ? (selectAreaWidth + Math.abs(deltaX)) / selectAreaWidth : (selectAreaWidth - Math.abs(deltaX)) / selectAreaWidth;
                    newWidth = startValues.width * scaleX;
                    newLeft = selectArea.right - ((selectArea.right - ((startValues.left - pageMarginsPx[0]) + startValues.width)) * scaleX) - newWidth + pageMarginsPx[0];

                    var scaleY = deltaY < 0 ? (selectAreaHeight + Math.abs(deltaY)) / selectAreaHeight : (selectAreaHeight - Math.abs(deltaY)) / selectAreaHeight;
                    newHeight = startValues.height * scaleY;
                    newTop = selectArea.bottom - ((selectArea.bottom - ((startValues.top - pageMarginsPx[1]) + startValues.height)) * scaleY) - newHeight + pageMarginsPx[1];
                    break;
                }
            case "MultiTop":
                {
                    var scaleY = deltaY < 0 ? (selectAreaHeight + Math.abs(deltaY)) / selectAreaHeight : (selectAreaHeight - Math.abs(deltaY)) / selectAreaHeight;
                    newHeight = startValues.height * scaleY;
                    newTop = selectArea.bottom - ((selectArea.bottom - ((startValues.top - pageMarginsPx[1]) + startValues.height)) * scaleY) - newHeight + pageMarginsPx[1];
                    break;
                }
            case "MultiRightTop":
                {
                    var scaleX = (selectAreaWidth + deltaX) / selectAreaWidth;
                    newWidth = startValues.width * scaleX;
                    if (!this.IsContains(fixedComponents.left, component)) {
                        newLeft = ((startValues.left - pageMarginsPx[0] - selectArea.left) * scaleX) + pageMarginsPx[0] + selectArea.left;
                    }
                    var scaleY = deltaY < 0 ? (selectAreaHeight + Math.abs(deltaY)) / selectAreaHeight : (selectAreaHeight - Math.abs(deltaY)) / selectAreaHeight;
                    newHeight = startValues.height * scaleY;
                    newTop = selectArea.bottom - ((selectArea.bottom - ((startValues.top - pageMarginsPx[1]) + startValues.height)) * scaleY) - newHeight + pageMarginsPx[1];
                    break;
                }
            case "MultiRight":
                {
                    var scaleX = (selectAreaWidth + deltaX) / selectAreaWidth;
                    newWidth = startValues.width * scaleX;
                    if (!this.IsContains(fixedComponents.left, component)) {
                        newLeft = ((startValues.left - pageMarginsPx[0] - selectArea.left) * scaleX) + pageMarginsPx[0] + selectArea.left;
                    }
                    break;
                }
            case "MultiRightBottom":
                {
                    var scaleX = (selectAreaWidth + deltaX) / selectAreaWidth;
                    var scaleY = (selectAreaHeight + deltaY) / selectAreaHeight;
                    newWidth = startValues.width * scaleX;
                    newHeight = startValues.height * scaleY;
                    if (!this.IsContains(fixedComponents.left, component)) {
                        newLeft = ((startValues.left - pageMarginsPx[0] - selectArea.left) * scaleX) + pageMarginsPx[0] + selectArea.left;
                    }
                    if (!this.IsContains(fixedComponents.top, component)) {
                        newTop = ((startValues.top - pageMarginsPx[1] - selectArea.top) * scaleY) + pageMarginsPx[1] + selectArea.top;
                    }
                    break;
                }
            case "MultiBottom":
                {
                    var scaleY = (selectAreaHeight + deltaY) / selectAreaHeight;
                    newHeight = startValues.height * scaleY;
                    if (!this.IsContains(fixedComponents.top, component)) {
                        newTop = ((startValues.top - pageMarginsPx[1] - selectArea.top) * scaleY) + pageMarginsPx[1] + selectArea.top;
                    }
                    break;
                }
            case "MultiLeftBottom":
                {
                    var scaleX = deltaX < 0 ? (selectAreaWidth + Math.abs(deltaX)) / selectAreaWidth : (selectAreaWidth - Math.abs(deltaX)) / selectAreaWidth;
                    newWidth = startValues.width * scaleX;
                    newLeft = selectArea.right - ((selectArea.right - ((startValues.left - pageMarginsPx[0]) + startValues.width)) * scaleX) - newWidth + pageMarginsPx[0];

                    var scaleY = (selectAreaHeight + deltaY) / selectAreaHeight;
                    newHeight = startValues.height * scaleY;
                    if (!this.IsContains(fixedComponents.top, component)) {
                        newTop = ((startValues.top - pageMarginsPx[1] - selectArea.top) * scaleY) + pageMarginsPx[1] + selectArea.top;
                    }
                    break;
                }
            case "MultiLeft":
                {
                    var scaleX = deltaX < 0 ? (selectAreaWidth + Math.abs(deltaX)) / selectAreaWidth : (selectAreaWidth - Math.abs(deltaX)) / selectAreaWidth;
                    newWidth = startValues.width * scaleX;
                    newLeft = selectArea.right - ((selectArea.right - ((startValues.left - pageMarginsPx[0]) + startValues.width)) * scaleX) - newWidth + pageMarginsPx[0];
                    break;
                }
        }

        if (newWidth > 0 && newHeight > 0) {
            var pageMarginsPx = this.options.report.pages[component.properties.pageName].marginsPx;
            component.properties.unitLeft = this.ConvertPixelToUnit((newLeft - pageMarginsPx[0]) / this.options.report.zoom, component.isDashboardElement);
            component.properties.unitTop = this.ConvertPixelToUnit((newTop - pageMarginsPx[1]) / this.options.report.zoom, component.isDashboardElement);
            component.properties.unitWidth = this.ConvertPixelToUnit(newWidth / this.options.report.zoom, component.isDashboardElement);
            component.properties.unitHeight = this.ConvertPixelToUnit(newHeight / this.options.report.zoom, component.isDashboardElement);
            component.repaint();
        }
    }

    this.PaintSelectedLines();
}

StiMobileDesigner.prototype.ResizeComponent = function (mouseCurrentXPos, mouseCurrentYPos, tableCell) {
    var component = tableCell || this.options.in_resize[0];
    var resizingType = tableCell ? tableCell.resizingType : this.options.in_resize[1];
    var startValues = tableCell ? tableCell.startValues : this.options.in_resize[2];
    var canResize = !component.properties.locked &&
        ((component.properties.restrictions && (component.properties.restrictions == "All" || component.properties.restrictions.indexOf("AllowResize") >= 0)) ||
            !component.properties.restrictions);
    component.properties.invertWidth = false;
    component.properties.invertHeight = false;

    var isTableCell = this.IsTableCell(component);
    if (!tableCell && !isTableCell && !canResize) return;

    var deltaX = this.options.startMousePos[0] - mouseCurrentXPos;
    var deltaY = this.options.startMousePos[1] - mouseCurrentYPos;

    var newWidth = startValues.width;
    var newHeight = startValues.height;
    var newLeft = startValues.left;
    var newTop = startValues.top;

    if (resizingType == "ResizeDiagonal" || resizingType == "ResizeWidth" || resizingType == "ResizeHeight") {
        var directWidth = resizingType == "ResizeDiagonal" || resizingType == "ResizeWidth" ? -1 : 0;
        var directHeight = resizingType == "ResizeDiagonal" || resizingType == "ResizeHeight" ? -1 : 0;

        newWidth = startValues.width + directWidth * deltaX;
        newHeight = startValues.height + directHeight * deltaY;
    }
    else if (resizingType == "ResizeHeightUp") {
        newTop = startValues.top - deltaY;
        newHeight = startValues.height + deltaY;
    }
    else {
        switch (resizingType) {
            case "LeftTop":
                {
                    newLeft = startValues.left - deltaX;
                    newTop = startValues.top - deltaY;
                    newWidth = startValues.width + deltaX;
                    newHeight = startValues.height + deltaY;
                    break;
                }
            case "Top":
                {
                    newTop = startValues.top - deltaY;
                    newHeight = startValues.height + deltaY;
                    break;
                }
            case "RightTop":
                {
                    newTop = startValues.top - deltaY;
                    newWidth = startValues.width - deltaX;
                    newHeight = startValues.height + deltaY;
                    break;
                }
            case "Right":
                {
                    newWidth = startValues.width - deltaX;
                    break;
                }
            case "RightBottom":
                {
                    newWidth = startValues.width - deltaX;
                    newHeight = startValues.height - deltaY;
                    break;
                }
            case "Bottom":
                {
                    newHeight = startValues.height - deltaY;
                    break;
                }
            case "LeftBottom":
                {
                    newLeft = startValues.left - deltaX;
                    newWidth = startValues.width + deltaX;
                    newHeight = startValues.height - deltaY;
                    break;
                }
            case "Left":
                {
                    newLeft = startValues.left - deltaX;
                    newWidth = startValues.width + deltaX;
                    break;
                }
        }
    }

    if (component.typeComponent == "StiTable" && resizingType == "Bottom") {
        this.ResizeAllTableCells(component, startValues.height != 0 ? newHeight / startValues.height : 1);
    }

    var jsObject = this;
    var checkStopResizing = function () {
        if (jsObject.options.in_resize[0].stopResizing) return true;
        else if (jsObject.options.in_resize.length > 3) {
            for (var i = 0; i < jsObject.options.in_resize[3].length; i++) {
                if (jsObject.options.in_resize[3][i].stopResizing) return true;
            }
        }
        return false;
    }

    var stopResizingAllComponents = false;
    if (isTableCell && !tableCell) this.options.oldPositions = [];

    if (newWidth > 0 && newHeight > 0) {
        if (isTableCell) {
            this.options.oldPositions.push([component, component.properties.unitLeft, component.properties.unitTop, component.properties.unitWidth, component.properties.unitHeight]);
        }

        component.stopResizing = false;
        stopResizingAllComponents = isTableCell ? checkStopResizing() : false;

        if (!stopResizingAllComponents) {
            var pageMarginsPx = this.options.report.pages[component.properties.pageName].marginsPx;
            component.properties.unitLeft = this.ConvertPixelToUnit((newLeft - pageMarginsPx[0]) / this.options.report.zoom, component.isDashboardElement);
            component.properties.unitTop = this.ConvertPixelToUnit((newTop - pageMarginsPx[1]) / this.options.report.zoom, component.isDashboardElement);
            component.properties.unitWidth = this.ConvertPixelToUnit(newWidth / this.options.report.zoom, component.isDashboardElement);
            component.properties.unitHeight = this.ConvertPixelToUnit(newHeight / this.options.report.zoom, component.isDashboardElement);
        }
    }
    else {
        if ((this.options.isTouchDevice && !isTableCell) || this.IsBandComponent(component) || this.IsCrossBandComponent(component)) {
            if (newWidth < 0) component.properties.unitWidth = this.ConvertPixelToUnit(1 / this.options.report.zoom, component.isDashboardElement);
            if (newHeight < 0) component.properties.unitHeight = this.ConvertPixelToUnit(1 / this.options.report.zoom, component.isDashboardElement);
        }
        else {
            if ((newWidth <= 0 || newHeight <= 0) && isTableCell) {
                component.stopResizing = true;
            }

            if (newWidth < 0) {
                component.properties.invertWidth = true;
                newLeft += newWidth;
                newWidth = Math.abs(newWidth);
            }
            if (newHeight < 0) {
                component.properties.invertHeight = true;
                newTop += newHeight;
                newHeight = Math.abs(newHeight);
            }

            stopResizingAllComponents = component.stopResizing;

            if (!stopResizingAllComponents) {
                var pageMarginsPx = this.options.report.pages[component.properties.pageName].marginsPx;
                component.properties.unitLeft = this.ConvertPixelToUnit((newLeft - pageMarginsPx[0]) / this.options.report.zoom, component.isDashboardElement);
                component.properties.unitTop = this.ConvertPixelToUnit((newTop - pageMarginsPx[1]) / this.options.report.zoom, component.isDashboardElement);
                component.properties.unitWidth = this.ConvertPixelToUnit(newWidth / this.options.report.zoom, component.isDashboardElement);
                component.properties.unitHeight = this.ConvertPixelToUnit(newHeight / this.options.report.zoom, component.isDashboardElement);
            }
        }
    }

    if (!stopResizingAllComponents) component.repaint();

    clearTimeout(component.posTimer);
    component.posTimer = setTimeout(function () {
        jsObject.options.statusPanel.showPositions(
            component.properties.unitLeft,
            component.properties.unitTop,
            component.properties.unitWidth,
            component.properties.unitHeight
        );
    }, 20);

    if (!tableCell && isTableCell) {
        this.ResizeTableCells(mouseCurrentXPos, mouseCurrentYPos);

        if (checkStopResizing() && this.options.oldPositions) {
            for (var i = 0; i < this.options.oldPositions.length; i++) {
                var comp = this.options.oldPositions[i][0];
                comp.properties.unitLeft = this.options.oldPositions[i][1];
                comp.properties.unitTop = this.options.oldPositions[i][2];
                comp.properties.unitWidth = this.options.oldPositions[i][3];
                comp.properties.unitHeight = this.options.oldPositions[i][4];
                comp.repaint();
            }
            this.options.oldPositions = [];
        }
    }
}

StiMobileDesigner.prototype.SetComponentToNewPos = function (component, page, startPosX, startPosY, allChilds) {
    var jsObject = this;
    var marginLeftPx = page.marginsPx[0];
    var marginTopPx = page.marginsPx[1];

    var canMove = !component.properties.locked &&
        ((component.properties.restrictions && (component.properties.restrictions == "All" || component.properties.restrictions.indexOf("AllowMove") >= 0)) ||
            !component.properties.restrictions);
    if (!canMove) return;

    var deltaX = jsObject.options.startMousePos[0] - jsObject.mouseCurrentXPos;
    var deltaY = jsObject.options.startMousePos[1] - jsObject.mouseCurrentYPos;

    var newPosX = startPosX - deltaX;
    var newPosY = startPosY - deltaY;

    if (newPosX < -component.realWidth) { deltaX = deltaX + newPosX; newPosX = -component.realWidth + 10 }
    if (newPosY < -component.realHeight) { deltaY = deltaY + newPosY; newPosY = -component.realHeight + 10; }
    if (newPosX > page.widthPx - 10) { deltaX = deltaX - newPosX; newPosX = page.widthPx - 10; }
    if (newPosY > page.heightPx - 10) { deltaY = deltaY - newPosY; newPosY = page.heightPx - 10; }

    component.setAttribute("transform", "translate(" + newPosX + ", " + newPosY + ")");
    component.setAttribute("left", newPosX);
    component.setAttribute("top", newPosY);
    jsObject.MoveAllChildsComponents(allChilds, deltaX, deltaY, marginLeftPx, marginTopPx);

    if (component.controls.iframeContent) {
        var pageMarginsPx = jsObject.options.report.pages[component.properties.pageName].marginsPx;
        component.properties.unitLeft = jsObject.ConvertPixelToUnit((newPosX - pageMarginsPx[0]) / jsObject.options.report.zoom, component.isDashboardElement);
        component.properties.unitTop = jsObject.ConvertPixelToUnit((newPosY - pageMarginsPx[1]) / jsObject.options.report.zoom, component.isDashboardElement);
        this.RepaintContent(component);
    }

    clearTimeout(component.posTimer);
    component.posTimer = setTimeout(function () {
        var pageMarginsPx = jsObject.options.report.pages[component.properties.pageName].marginsPx;
        component.properties.unitLeft = jsObject.ConvertPixelToUnit((newPosX - pageMarginsPx[0]) / jsObject.options.report.zoom, component.isDashboardElement);
        component.properties.unitTop = jsObject.ConvertPixelToUnit((newPosY - pageMarginsPx[1]) / jsObject.options.report.zoom, component.isDashboardElement);

        jsObject.options.statusPanel.showPositions(
            component.properties.unitLeft,
            component.properties.unitTop,
            component.properties.unitWidth,
            component.properties.unitHeight
        );
    }, 20);
}

StiMobileDesigner.prototype.MoveCopyComponent = function (mouseCurrentXPos, mouseCurrentYPos) {
    var jsObject = this;
    var in_drag = this.options.in_drag;
    if (!this.options.in_drag) return;

    if (!this.options.movingCloneComponents) {
        var components = this.Is_array(in_drag[0]) ? in_drag[0] : [in_drag[0]];
        var componentChilds = this.Is_array(in_drag[3]) ? in_drag[3] : [in_drag[3]];
        var movingCloneComponents = [];

        for (var i = 0; i < components.length; i++) {
            var component = components[i];
            var cloneComponent = component.clone();

            movingCloneComponents.push(cloneComponent);
            cloneComponent.repaint();
            cloneComponent.page = this.options.report.pages[cloneComponent.properties.pageName];
            cloneComponent.page.appendChild(cloneComponent);

            cloneComponent.cloneChilds = [];
            var childs = componentChilds[i];
            for (var childName in childs) {
                var cloneChild = childs[childName].clone();
                cloneChild.startPosX = childs[childName].startPosX;
                cloneChild.startPosY = childs[childName].startPosY;

                cloneChild.repaint();
                cloneComponent.cloneChilds.push(cloneChild);
                cloneComponent.page.appendChild(cloneChild);
            }
        }

        this.options.movingCloneComponents = movingCloneComponents;
    }

    var startPosX = this.Is_array(in_drag[1]) ? in_drag[1] : [in_drag[1]];
    var startPosY = this.Is_array(in_drag[2]) ? in_drag[2] : [in_drag[2]];

    for (var i = 0; i < this.options.movingCloneComponents.length; i++) {
        this.SetComponentToNewPos(
            this.options.movingCloneComponents[i],
            this.options.movingCloneComponents[i].page,
            startPosX[i],
            startPosY[i],
            this.options.movingCloneComponents[i].cloneChilds
        );
    }
}

StiMobileDesigner.prototype.MoveComponent = function (mouseCurrentXPos, mouseCurrentYPos) {

    if (this.options.multiSelectHelperControls && this.options.mouseMoved) {
        if (this.options.selectedObjects != null && !this.options.multiSelectHelperControls.setOnTopLevel) {
            for (var i = 0; i < this.options.selectedObjects.length; i++) this.options.selectedObjects[i].setOnTopLevel();
            this.options.multiSelectHelperControls.setOnTopLevel = true;
        }
        this.DeleteSelectedLines();
    }

    var jsObject = this;
    var in_drag = this.options.in_drag;

    if (this.Is_array(in_drag[0])) {
        for (var i = 0; i < in_drag[0].length; i++) {
            if (this.IsTableCell(in_drag[0][i])) continue;
            this.SetComponentToNewPos(in_drag[0][i], this.options.report.pages[in_drag[0][i].properties.pageName], in_drag[1][i], in_drag[2][i], in_drag[3][i]);
        }
    }
    else {
        if (this.IsTableCell(in_drag[0])) return;
        this.SetComponentToNewPos(in_drag[0], this.options.report.pages[in_drag[0].properties.pageName], in_drag[1], in_drag[2], in_drag[3]);
    }
}

StiMobileDesigner.prototype.MoveAllChildsComponents = function (allChilds, moveX, moveY, marginLeftPx, marginTopPx) {
    for (var childName in allChilds) {
        var child = allChilds[childName];
        var startX = parseInt(child.startPosX);
        var startY = parseInt(child.startPosY);

        var newLeft = startX - moveX;
        var newTop = startY - moveY;

        var newXPos = (newLeft - marginLeftPx) / this.options.report.zoom;
        var newYPos = (newTop - marginTopPx) / this.options.report.zoom;

        var oldLeft = child.getAttribute("left");
        var oldTop = child.getAttribute("top");

        child.setAttribute("transform", "translate(" + newLeft + ", " + newTop + ")");
        child.setAttribute("left", newLeft);
        child.setAttribute("top", newTop);
        child.properties.unitLeft = this.ConvertPixelToUnit(newXPos, child.isDashboardElement);
        child.properties.unitTop = this.ConvertPixelToUnit(newYPos, child.isDashboardElement);

        if (child.controls && child.controls.iframeContent) {
            this.RepaintContent(child);
        }
    }
}

StiMobileDesigner.prototype.RemoveComponent = function (component) {
    var components = this.Is_array(component) ? component : [component];
    var page = this.options.report.pages[components[0].properties.pageName];
    if (!page) return;

    this.SendCommandRemoveComponent(component);

    for (var i = 0; i < components.length; i++) {
        var childs = components[i].getAllChildsComponents();
        for (var indexChild in childs) {
            var child = childs[indexChild];
            page.removeChild(child);
            if (child.controls && child.controls.iframeContent) {
                child.controls.iframeContent.parentNode.removeChild(child.controls.iframeContent);
            }
            delete page.components[child.properties.name];
            child = undefined;
        }
        if (components[i].isDashboardElement && this.options.currentForm && this.options.currentForm.dockingComponent == components[i]) {
            this.options.currentForm.changeVisibleState(false);
        }
        if (page.components[components[i].properties.name]) {
            page.removeChild(components[i]);
            delete page.components[components[i].properties.name];
        }
        if (components[i].controls && components[i].controls.iframeContent) {
            components[i].controls.iframeContent.parentNode.removeChild(components[i].controls.iframeContent);
        }
        delete components[i];
    }
    this.options.selectedObjects = null;
    page.setSelected();
    this.UpdatePropertiesControls();
}

StiMobileDesigner.prototype.MoveSelectedComponentsToUnplaced = function () {
    if (this.options.selectedObjects || this.options.selectedObject) {
        var components = this.options.selectedObjects || [this.options.selectedObject];
        for (var i = 0; i < components.length; i++) {
            components[i].properties.unitLeft = components[i].properties.unitTop = components[i].properties.unitWidth = components[i].properties.unitHeight = "0";
            components[i].repaint();
        }
        this.SendCommandChangeRectComponent(components, "ResizeComponent", true, null, true);
    }
}

StiMobileDesigner.prototype.CopyComponent = function (component) {
    this.SendCommandSetToClipboard(component);
}

StiMobileDesigner.prototype.CutComponent = function (component) {
    this.SendCommandSetToClipboard(component);
    this.RemoveComponent(component);
}

StiMobileDesigner.prototype.RenameComponent = function (component, newName) {
    var page = this.options.report.pages[component.properties.pageName];
    page.components[newName] = component;
    delete page.components[component.properties.name];
    component.properties.name = newName;
    component.repaint();
}

StiMobileDesigner.prototype.RepaintColumnsLines = function (component) {
    var jsObject = this;
    var columnsCount = component.properties.columns ? jsObject.StrToInt(component.properties.columns) : 0;

    //remove old lines
    if (component.controls.columnLines) {
        for (var i = 0; i < component.controls.columnLines.length; i++) {
            component.removeChild(component.controls.columnLines[i]);
        }
        component.controls.columnLines = null;
    }

    //add new lines
    if (columnsCount > 1 && component.controls) {
        var topMargin = component.marginsPx ? component.marginsPx[1] : 0;
        var bottomMargin = component.marginsPx ? component.marginsPx[3] : 0;
        var leftMargin = component.marginsPx ? component.marginsPx[0] : 0;
        var rightMargin = component.marginsPx ? component.marginsPx[2] : 0;
        var componentWidthPx = parseInt(component.getAttribute("width")) - leftMargin - rightMargin;
        var componentHeightPx = parseInt(component.getAttribute("height")) - topMargin - bottomMargin;
        var columnGapsPx = jsObject.ConvertUnitToPixel(jsObject.StrToDouble(component.properties.columnGaps), component.isDashboardElement) * jsObject.options.report.zoom;

        var getColumnWidthPx = function () {
            var panelColumnWidthPx = jsObject.ConvertUnitToPixel(jsObject.StrToDouble(component.properties.columnWidth), component.isDashboardElement) * jsObject.options.report.zoom;
            if (panelColumnWidthPx == 0) {
                if (columnsCount == 0) return componentWidthPx;
                panelColumnWidthPx = (componentWidthPx / columnsCount) - columnGapsPx;
            }
            return panelColumnWidthPx;
        }

        var addRedLine = function (x1, y1, x2, y2) {
            var redLine = ("createElementNS" in document) ? document.createElementNS("http://www.w3.org/2000/svg", "line") : document.createElement("line");
            component.appendChild(redLine);
            if (!component.controls.columnLines) component.controls.columnLines = [];
            component.controls.columnLines.push(redLine);

            redLine.style.strokeDasharray = "2,2";
            redLine.style.stroke = "#ff0000";
            var roundedCoordinates = jsObject.GetRoundedLineCoordinates([x1, y1, x2, y2]);

            redLine.setAttribute("x1", roundedCoordinates[0] + jsObject.options.xOffset);
            redLine.setAttribute("y1", roundedCoordinates[1] + jsObject.options.yOffset);
            redLine.setAttribute("x2", roundedCoordinates[2] + jsObject.options.xOffset);
            redLine.setAttribute("y2", roundedCoordinates[3] + jsObject.options.yOffset);

            return redLine;
        }

        var columnWidthPx = getColumnWidthPx();
        var pos = columnWidthPx;

        for (var index = 1; index < columnsCount; index++) {
            addRedLine(pos + leftMargin, 0 + topMargin, pos + leftMargin, componentHeightPx + topMargin);
            addRedLine(pos + leftMargin + columnGapsPx, 0 + topMargin, pos + leftMargin + columnGapsPx, componentHeightPx + topMargin);
            pos += columnWidthPx + columnGapsPx;
        }
        if (pos < componentWidthPx)
            addRedLine(pos + leftMargin, 0 + topMargin, pos + leftMargin, componentHeightPx + topMargin);
    }
}

StiMobileDesigner.prototype.ResizeTableCells = function (mouseCurrentXPos, mouseCurrentYPos) {
    var resizingCells = this.options.in_resize[3];
    if (resizingCells) {
        for (var i = 0; i < resizingCells.length; i++) {
            this.ResizeComponent(mouseCurrentXPos, mouseCurrentYPos, resizingCells[i]);
        }
    }
}

StiMobileDesigner.prototype.ResizeAllTableCells = function (table, cellsZoom) {
    var resizingCells = this.options.in_resize[3];
    var tableTop = parseInt(table.getAttribute("top"));
    var deltaY = cellsZoom * tableTop - tableTop;
    var pageMarginsPx = this.options.report.pages[table.properties.pageName].marginsPx;

    if (resizingCells) {
        for (var i = 0; i < resizingCells.length; i++) {
            var cell = resizingCells[i];

            var newHeight = cell.startValues.height * cellsZoom;
            var newTop = cell.startValues.top * cellsZoom - deltaY;

            if (cell.startValues.top != newTop) cell.properties.unitTop = this.ConvertPixelToUnit((newTop - pageMarginsPx[1]) / this.options.report.zoom);
            if (cell.startValues.height != newHeight) cell.properties.unitHeight = this.ConvertPixelToUnit(newHeight / this.options.report.zoom);
            cell.repaint();
        }
    }
}

StiMobileDesigner.prototype.ApplyComponentSizes = function (component) {
    if (!component) return;
    var pageName = component.properties.pageName;
    var marginsPx = this.options.report.pages[pageName].marginsPx;

    var marginLeftPx = marginsPx[0];
    var marginTopPx = marginsPx[1];

    var leftPx = this.StrToDouble(component.getAttribute("left"));
    var topPx = this.StrToDouble(component.getAttribute("top"));

    var leftProperty = leftPx - marginLeftPx;
    var topProperty = topPx - marginTopPx;

    component.properties.unitLeft = this.ConvertPixelToUnit(leftProperty / this.options.report.zoom, component.isDashboardElement);
    component.properties.unitTop = this.ConvertPixelToUnit(topProperty / this.options.report.zoom, component.isDashboardElement);

    if (this.options.in_resize) {
        //debugger;
        var widthProperty = this.StrToDouble(component.getAttribute("width"));
        var heightProperty = this.StrToDouble(component.getAttribute("height"));

        component.properties.unitWidth = this.ConvertPixelToUnit(widthProperty / this.options.report.zoom, component.isDashboardElement);
        component.properties.unitHeight = this.ConvertPixelToUnit(heightProperty / this.options.report.zoom, component.isDashboardElement);
    }
}

StiMobileDesigner.prototype.CloneComponent = function (component) {
    var compObject = {
        properties: this.CopyObject(component.properties)
    }

    compObject.typeComponent = component.typeComponent;
    compObject.name = component.properties.name;
    compObject.parentName = component.properties.parentName;
    compObject.parentIndex = component.properties.parentIndex;
    compObject.componentIndex = component.properties.componentIndex;
    compObject.childs = component.properties.childs;
    compObject.svgContent = component.properties.svgContent;
    compObject.pageName = component.properties.pageName;
    compObject.componentRect = component.properties.unitLeft + "!" + component.properties.unitTop + "!" +
        component.properties.unitWidth + "!" + component.properties.unitHeight;

    return (component.properties.isDashboardElement ? this.CreateDashboardElement(compObject) : this.CreateComponent(compObject));
}

StiMobileDesigner.prototype.BaseObjectForDragDrop = function (typeComponent, isDashboardElement, itemObject, imageName) {
    var basicShapes = ["StiHorizontalLinePrimitive", "StiVerticalLinePrimitive", "StiRectanglePrimitive", "StiRoundedRectanglePrimitive"];
    var sizes = this.GetComponentDefaultSizes(typeComponent);
    var titleText = this.loc.Components[isDashboardElement ? typeComponent.replace("Element", "") : typeComponent];
    var dragObj = null

    if (basicShapes.indexOf(typeComponent) >= 0) {
        dragObj = this.StandartBigButton(null, null, null, typeComponent + ".png");
        dragObj.style.opacity = "0.7";
    }
    else if (sizes && titleText) {
        dragObj = ("createElementNS" in document) ? document.createElementNS("http://www.w3.org/2000/svg", "svg") : document.createElement("svg");

        var componentRect = isDashboardElement
            ? "0!0!" + sizes.width + "!" + sizes.height
            : "0!0!" + this.ConvertPixelToUnit(sizes.width) + "!" + this.ConvertPixelToUnit(sizes.height);

        var compObject = {
            componentRect: componentRect,
            typeComponent: typeComponent,
            properties: {
                border: "0,0,0,0!1!0,0,0!0!0!0!0",
                brush: "1!transparent",
                svgContent: "",
                aliasName: StiBase64.encode(titleText),
                name: titleText
            }
        };

        if (typeComponent == "StiSubReport" && itemObject) {
            compObject.properties.subReportUrl = StiBase64.encode("resource://" + itemObject.name);
        }

        if (this.IsBandComponent({ typeComponent: typeComponent }) || this.IsCrossBandComponent({ typeComponent: typeComponent })) {
            compObject.properties.headerSize = 15;
        }

        var component = isDashboardElement ? this.CreateDashboardElement(compObject) : this.CreateComponent(compObject);
        dragObj.appendChild(component);

        this.RepaintComponent(component);

        dragObj.setAttribute("width", parseInt(component.getAttribute("width")) + 5);
        dragObj.setAttribute("height", parseInt(component.getAttribute("height")) + 5);
    }
    else {
        var image = imageName || (typeComponent + ".png");

        if (!StiMobileDesigner.checkImageSource(this.options, image)) {
            if (typeComponent.indexOf("StiShape;") == 0 || typeComponent.indexOf("StiShapeElement;") == 0) {
                var shapeTypeArray = typeComponent.split(";");
                if (shapeTypeArray.length == 2) {
                    image = "Shapes." + shapeTypeArray[1] + ".png";
                }
            }
            else if (typeComponent.indexOf("StiBarCode;") == 0) {
                var barCodeTypeArray = typeComponent.split(";");
                if (barCodeTypeArray.length == 2) {
                    image = "BarCodes." + barCodeTypeArray[1] + ".png";
                }
            }
            else if (typeComponent.indexOf("Infographic;StiChart;") == 0) {
                var chartTypeArray = typeComponent.split(";");
                if (chartTypeArray.length == 3) {
                    image = "Charts.Big." + chartTypeArray[2].replace("Sti", "").replace("Series", "") + ".png";
                }
            }
            else if (typeComponent.indexOf("Infographic;StiGauge;") == 0) {
                var gaugeTypeArray = typeComponent.split(";");
                if (gaugeTypeArray.length == 3) {
                    image = "Gauge.Big." + gaugeTypeArray[2] + ".png";
                }
            }
        }

        if (isDashboardElement) {
            image = "Dashboards.BigComponents." + typeComponent + ".png";
        }

        if (StiMobileDesigner.checkImageSource(this.options, image)) {
            dragObj = this.StandartBigButton(null, null, this.loc.Components[typeComponent], image);
            dragObj.style.opacity = "0.7";
        }
    }

    if (dragObj) {
        this.options.mainPanel.appendChild(dragObj);
        dragObj.jsObject = this;
        dragObj.style.position = "absolute";
        dragObj.style.display = "none";
        dragObj.style.zIndex = "300";

        dragObj.move = function (event, offsetX, offsetY) {
            this.style.display = "";
            var clientX = event.touches ? event.touches[0].pageX : event.clientX;
            var clientY = event.touches ? event.touches[0].pageY : event.clientY;

            var designerOffsetX = this.jsObject.FindPosX(this.jsObject.options.mainPanel);
            var designerOffsetY = this.jsObject.FindPosY(this.jsObject.options.mainPanel);
            clientX -= designerOffsetX;
            clientY -= designerOffsetY;

            if (offsetX) clientX += offsetX;
            if (offsetY) clientY += offsetY;

            this.style.left = (clientX + 2) + "px";
            this.style.top = (sizes ? clientY + 2 : clientY + 10) + "px";
        }

        return dragObj;
    }

    return null;
}

StiMobileDesigner.prototype.ComponentForDragDrop = function (itemObject, typeComponent, imageName) {
    var dragComp = this.BaseObjectForDragDrop(typeComponent, false, itemObject, imageName);
    if (dragComp) dragComp.itemObject = itemObject;

    return dragComp;
}

StiMobileDesigner.prototype.DashboardElementForDragDrop = function (itemObject, typeComponent, imageName) {
    var dragElement = this.BaseObjectForDragDrop(typeComponent, true, itemObject, imageName);
    if (dragElement) dragElement.itemObject = itemObject;

    return dragElement;
}

StiMobileDesigner.prototype.UnplacedElementForDragDrop = function (itemObject) {
    var dragObj = document.createElement("div");
    if (itemObject.svgContent) dragObj.innerHTML = StiBase64.decode(itemObject.svgContent);
    this.options.mainPanel.appendChild(dragObj);
    dragObj.style.position = "absolute";
    dragObj.style.display = "none";
    dragObj.style.zIndex = "300";
    dragObj.style.boxShadow = "0px 0px 10px rgba(0,0,0,0.8)";
    dragObj.style.lineHeight = "0";
    dragObj.jsObject = this;

    dragObj.move = function (event, offsetX, offsetY) {
        this.style.display = "";
        var clientX = event.touches ? event.touches[0].pageX : event.clientX;
        var clientY = event.touches ? event.touches[0].pageY : event.clientY;

        var designerOffsetX = this.jsObject.FindPosX(this.jsObject.options.mainPanel);
        var designerOffsetY = this.jsObject.FindPosY(this.jsObject.options.mainPanel);
        clientX -= designerOffsetX;
        clientY -= designerOffsetY;

        if (offsetX) clientX += offsetX;
        if (offsetY) clientY += offsetY;

        this.style.left = (clientX + 2) + "px";
        this.style.top = (clientY + 2) + "px";
    }

    return dragObj;
}

StiMobileDesigner.prototype.GetComponentDefaultSizes = function (typeComponent) {
    var sizes = {
        width: 100,
        height: 100
    }
    if (this.IsBandComponent({ typeComponent: typeComponent })) {
        sizes.width = 300;
        sizes.height = 50;
    }
    else if (this.IsCrossBandComponent({ typeComponent: typeComponent })) {
        sizes.width = 30;
        sizes.height = 300;
    }
    else {
        switch (typeComponent) {
            case "StiText":
            case "StiTextInCells":
            case "StiRichText":
            case "StiCheckBox":
                sizes.width = 60;
                sizes.height = 20;
                break;
            case "StiBarCode":
                sizes.width = 240;
                sizes.height = 110;
                break;
            case "StiShape":
                sizes.width = 240;
                sizes.height = 110;
                break;
            case "StiZipCode":
                sizes.width = 200;
                sizes.height = 40;
                break;
            case "StiCrossTab":
                sizes.width = 300;
                sizes.height = 100;
                break;
            case "StiImage":
            case "StiPanel":
            case "StiClone":
            case "StiSubReport":
                sizes.width = 100;
                sizes.height = 100;
                break;
            case "StiChart":
                sizes.width = 200;
                sizes.height = 200;
                break;
            case "StiMap":
                sizes.width = 240;
                sizes.height = 240;
                break;
            case "StiGauge":
                sizes.width = 140;
                sizes.height = 140;
                break;
            case "StiSparkline":
            case "StiMathFormula":
                sizes.width = 150;
                sizes.height = 100;
                break;
            case "StiTableElement":
            case "StiChartElement":
            case "StiGaugeElement":
            case "StiPivotTableElement":
            case "StiRegionMapElement":
            case "StiOnlineMapElement":
            case "StiCardsElement":
                sizes.width = 280;
                sizes.height = 280;
                break;
            case "StiIndicatorElement":
            case "StiProgressElement":
                sizes.width = 160;
                sizes.height = 160;
                break;
            case "StiImageElement":
                sizes.width = 120;
                sizes.height = 120;
                break;
            case "StiPanelElement":
                sizes.width = 300;
                sizes.height = 300;
                break;
            case "StiTextElement":
                sizes.width = 100;
                sizes.height = 40;
                break;
            case "StiListBoxElement":
            case "StiTreeViewElement":
                sizes.width = 200;
                sizes.height = 300;
                break;
            case "StiComboBoxElement":
            case "StiTreeViewBoxElement":
            case "StiDatePickerElement":
                sizes.width = 200;
                sizes.height = 40;
                break;
            case "StiButtonElement":
                sizes.width = 140;
                sizes.height = 40;
                break;
            case "StiElectronicSignature":
            case "StiPdfDigitalSignature":
                sizes.width = 380;
                sizes.height = 110;
                break;
        }
    }

    return sizes;
}

StiMobileDesigner.prototype.MoveSelectorByComponents = function (keyCode) {
    var selComp = this.options.selectedObject;
    if (selComp && selComp.typeComponent != "StiPage" && selComp.typeComponent != "StiReport") {
        var getAttrValue = function (comp, attr) {
            return parseInt(comp.getAttribute(attr));
        }

        var jsObject = this;
        var leftPos = getAttrValue(selComp, "left");
        var topPos = getAttrValue(selComp, "top");
        var comps = [];
        var findedLeft = null;
        var findedTop = null;
        var resultComp = null;
        var isBand = jsObject.IsBandComponent(selComp);

        var selectCompByMinPosition = function (direction) {
            if (comps.length > 1) {
                var temp = [];
                for (var i = 0; i < comps.length; i++) {
                    var findedIsBand = jsObject.IsBandComponent(comps[i]);
                    if ((isBand && findedIsBand) || (!isBand && !findedIsBand)) {
                        temp.push(comps[i]);
                    }
                }
                comps = temp;
            }

            var minDelta = null;
            for (var i = 0; i < comps.length; i++) {
                var pos = getAttrValue(comps[i], direction == "vert" ? "top" : "left");
                var delta = Math.abs((direction == "vert" ? topPos : leftPos) - pos);
                if (minDelta == null || delta < minDelta) {
                    minDelta = delta;
                    resultComp = comps[i];
                }
            }
            if (resultComp != null && resultComp != selComp) {
                resultComp.setSelected();
                resultComp.setOnTopLevel();
                jsObject.UpdatePropertiesControls();
            }
        }

        //Arrow Left or Right
        if (keyCode == 39 || keyCode == 37) {
            for (var componentName in this.options.currentPage.components) {
                var comp = this.options.currentPage.components[componentName];
                var left = getAttrValue(comp, "left");
                if ((keyCode == 39 && left > leftPos && (findedLeft == null || left <= findedLeft)) || (keyCode == 37 && left < leftPos && (findedLeft == null || left >= findedLeft))) {
                    if (left == findedLeft)
                        comps.push(comp)
                    else {
                        comps = [comp];
                        findedLeft = left;
                    }
                }
            }
            selectCompByMinPosition("vert");
        }

        //Arrow Up or Down
        if (keyCode == 38 || keyCode == 40) {
            for (var componentName in this.options.currentPage.components) {
                var comp = this.options.currentPage.components[componentName];
                var top = getAttrValue(comp, "top");
                if ((keyCode == 40 && top > topPos && (findedTop == null || top <= findedTop)) || (keyCode == 38 && top < topPos && (findedTop == null || top >= findedTop))) {
                    if (top == findedTop)
                        comps.push(comp)
                    else {
                        comps = [comp];
                        findedTop = top;
                    }
                }
            }
            selectCompByMinPosition("hor");
        }
    }
}

StiMobileDesigner.prototype.MoveComponentsByArrowButtons = function (keyCode, ignoreGridSize) {
    if (!this.options.report) return;

    var jsObject = this;
    var selectedObjects = this.options.selectedObjects || [this.options.selectedObject];
    var resultSelectedObjects = [];
    var gridSize = (this.options.currentPage && this.options.currentPage.isDashboard ? this.options.currentPage.properties.gridSize : this.ConvertPixelToUnit(this.options.report.gridSize)) * this.options.report.zoom;
    var offsetValue = ignoreGridSize ? 0.01 : gridSize;

    for (var i = 0; i < selectedObjects.length; i++) {
        var component = selectedObjects[i];
        var canMove = !component.properties.locked && ((component.properties.restrictions && (component.properties.restrictions == "All" || component.properties.restrictions.indexOf("AllowMove") >= 0)) || !component.properties.restrictions);
        if (!canMove || (component.properties.unitLeft == null && component.properties.unitTop == null)) continue;
        resultSelectedObjects.push(component);

        var leftValue = this.StrToDouble(selectedObjects[i].properties.unitLeft);
        var topValue = this.StrToDouble(selectedObjects[i].properties.unitTop);

        if (keyCode == 37 || keyCode == 39) {
            selectedObjects[i].properties.unitLeft = keyCode == 39 ? leftValue + offsetValue : leftValue - offsetValue;
        }

        if (keyCode == 38 || keyCode == 40) {
            selectedObjects[i].properties.unitTop = keyCode == 40 ? topValue + offsetValue : topValue - offsetValue;
        }
        component.repaint();
    }

    clearTimeout(jsObject.arrowActionsTimer);

    jsObject.arrowActionsTimer = setTimeout(function () {
        jsObject.SendCommandChangeRectComponent(resultSelectedObjects, "ResizeComponent", true);
    }, 1000);
}

StiMobileDesigner.prototype.ResizeComponentsByArrowButtons = function (keyCode) {
    var jsObject = this;
    var selectedObjects = this.options.selectedObjects || [this.options.selectedObject];
    var resultSelectedObjects = [];
    var offsetValue = (this.options.currentPage && this.options.currentPage.isDashboard ? this.options.currentPage.properties.gridSize : this.ConvertPixelToUnit(this.options.report.gridSize)) * this.options.report.zoom;

    for (var i = 0; i < selectedObjects.length; i++) {
        var component = selectedObjects[i];
        var canMove = !component.properties.locked && ((component.properties.restrictions && (component.properties.restrictions == "All" || component.properties.restrictions.indexOf("AllowMove") >= 0)) || !component.properties.restrictions);
        if (!canMove || (component.properties.unitLeft == null && component.properties.unitTop == null)) continue;
        resultSelectedObjects.push(component);

        var widthValue = this.StrToDouble(selectedObjects[i].properties.unitWidth);
        var heightValue = this.StrToDouble(selectedObjects[i].properties.unitHeight);

        if (keyCode == 37) {
            selectedObjects[i].properties.unitWidth = widthValue - offsetValue;
        }
        else if (keyCode == 38) {
            selectedObjects[i].properties.unitHeight = heightValue - offsetValue;
        }
        else if (keyCode == 40) {
            selectedObjects[i].properties.unitHeight = heightValue + offsetValue;
        }
        else if (keyCode == 39) {
            selectedObjects[i].properties.unitWidth = widthValue + offsetValue;
        }
        component.repaint();
    }

    clearTimeout(jsObject.arrowActionsTimer);

    jsObject.arrowActionsTimer = setTimeout(function () {
        jsObject.SendCommandChangeRectComponent(resultSelectedObjects, "ResizeComponent", true);
    }, 1000);
}

StiMobileDesigner.prototype.ApplyStyleToComponent = function (component, styleName) {
    if (component) {
        if (component.isDashboardElement) {
            if (component.properties.elementStyle != null) {
                component.properties.elementStyle = "Custom";
                component.properties.customStyleName = styleName;
                this.SendCommandSendProperties([component], ["elementStyle", "customStyleName"]);
            }
        }
        else if (component.typeComponent == "StiChart") {
            component.properties.chartStyle = { name: styleName, type: "StiCustomStyle" };
            this.SendCommandSendProperties([component], ["chartStyle"]);
        }
        else if (component.typeComponent == "StiGauge") {
            component.properties.gaugeStyle = { name: styleName, type: "StiCustomGaugeStyle" };
            this.SendCommandSendProperties([component], ["gaugeStyle"]);
        }
        else if (component.typeComponent == "StiMap") {
            component.properties.mapStyle = { name: styleName, type: "StiMapStyle" };
            this.SendCommandSendProperties([component], ["mapStyle"]);
        }
        else if (component.typeComponent == "StiCrossTab") {
            component.properties.crossTabStyle = { crossTabStyle: styleName };
            this.SendCommandSendProperties([component], ["crossTabStyle"]);
        }
        else if (component.typeComponent == "StiTable") {
            component.properties.styleId = "";
            component.properties.componentStyle = styleName;

            this.SendCommandChangeTableComponent({
                command: "applyStyle",
                styleId: component.properties.styleId,
                styleName: component.properties.componentStyle
            });
            return;
        }
        else if (component.properties.componentStyle != null) {
            component.properties.componentStyle = styleName;
            this.SendCommandSendProperties([component], ["componentStyle"]);
        }
    }
}