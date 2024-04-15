
StiMobileDesigner.prototype.RebuildPage = function (page, componentsProps) {
    for (var componentName in componentsProps) {
        var component = page.components[componentName];
        if (component) {
            component.properties.parentName = componentsProps[componentName].parentName;
            component.properties.parentIndex = componentsProps[componentName].parentIndex;
            component.properties.componentIndex = componentsProps[componentName].componentIndex;
            component.properties.childs = componentsProps[componentName].childs;
            component.properties.clientLeft = componentsProps[componentName].clientLeft;
            component.properties.clientTop = componentsProps[componentName].clientTop;
            if (componentsProps[componentName].svgContent) component.properties.svgContent = componentsProps[componentName].svgContent;

            var oldRect = component.properties.unitLeft + "!" + component.properties.unitTop + "!" +
                component.properties.unitWidth + "!" + component.properties.unitHeight;
            var newRect = componentsProps[componentName].componentRect;

            if (oldRect != newRect) {
                var rect = componentsProps[componentName].componentRect.split("!");
                component.properties.unitLeft = rect[0];
                component.properties.unitTop = rect[1];
                component.properties.unitWidth = rect[2];
                component.properties.unitHeight = rect[3];
                component.repaint();
            }
        }
    }

    page.updateComponentsLevels();
}

StiMobileDesigner.prototype.CheckWatermarkPosition = function (page) {
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

StiMobileDesigner.prototype.AddComponents = function (page) {
    var components = [];
    for (var componentName in page.components) {
        var component = page.components[componentName];
        var parentIndex = parseInt(component.properties.parentIndex);
        if (!components[parentIndex]) components[parentIndex] = [];
        components[parentIndex].push({ index: this.StrToInt(component.properties.componentIndex), component: component });
    }
    for (var i = 0; i < components.length; i++) {
        var compsByIndex = components[i];
        if (compsByIndex) {
            compsByIndex.sort(this.SortByIndex);
            for (var k = 0; k < compsByIndex.length; k++) {
                page.appendChild(compsByIndex[k].component);
            }
        }
    }
}

StiMobileDesigner.prototype.RemoveComponents = function (page) {
    for (var componentName in page.components) {
        var component = page.components[componentName];
        if (component) page.removeChild(component);
    }
}

StiMobileDesigner.prototype.UpdateComponentsLevels = function (page) {
    this.RemoveComponents(page);
    this.AddComponents(page);
    page.updateWatermarkLevels();
}

StiMobileDesigner.prototype.AddPage = function (answer, notShowAfterCreated) {
    var page = this.CreatePage(answer);
    page.repaint();
    this.options.report.pages[page.properties.name] = page;
    this.options.report.pages[page.properties.name].components = {};
    this.ChangePageIndexes(answer.pageIndexes);
    this.options.paintPanel.addPage(page);
    if (!notShowAfterCreated) {
        this.options.paintPanel.showPage(page);
        page.setSelected();
    }
    this.UpdatePropertiesControls();
    this.options.pagesPanel.pagesContainer.updatePages();

    return page;
}

StiMobileDesigner.prototype.RemovePage = function (page) {
    if (this.options.paintPanel.getPagesCount() < 2) return;
    this.SendCommandRemovePage(page);

    this.RemoveAllOnlineMapsOnPage(page);
    this.options.paintPanel.removePage(page);
    delete this.options.report.pages[page.properties.name];
    this.ChangePageIndexes();

    var firstPage = this.options.paintPanel.findPageByIndex(0);
    if (firstPage) {
        firstPage.setSelected();
        this.UpdatePropertiesControls();
        this.options.paintPanel.showPage(firstPage);
    }
    this.options.pagesPanel.pagesContainer.updatePages();
}

StiMobileDesigner.prototype.RemoveAllOnlineMapsOnPage = function (page) {
    for (var componentName in page.components) {
        var comp = page.components[componentName];
        if (comp.typeComponent == "StiOnlineMapElement" && comp.controls && comp.controls.iframeContent) {
            comp.controls.iframeContent.parentNode.removeChild(comp.controls.iframeContent);
        }
    }
}

StiMobileDesigner.prototype.RepaintAllComponentsOnPage = function (page) {
    for (var componentName in page.components) {
        page.components[componentName].repaint();
    }
}

StiMobileDesigner.prototype.GetComponentByIndex = function (page, index) {
    for (var componentName in page.components) {
        if (parseInt(page.components[componentName].properties.componentIndex) == index)
            return page.components[componentName];
    }
    return null;
}

StiMobileDesigner.prototype.ChangePageIndexes = function (pagesIndexes) {
    if (pagesIndexes) {
        for (var pageName in pagesIndexes)
            this.options.report.pages[pageName].properties.pageIndex = pagesIndexes[pageName];
    }
    else {
        var offset = 0;
        for (var i = 0; i < this.options.paintPanel.getPagesCount(); i++) {
            if (!this.options.paintPanel.findPageByIndex(i)) offset = 1;
            if (offset == 1) {
                var page = this.options.paintPanel.findPageByIndex(i + offset);
                if (page) page.properties.pageIndex = i.toString();
            }
        }
    }
}

StiMobileDesigner.prototype.RenamePage = function (page, newName) {
    var oldName = page.properties.name;
    this.options.report.pages[newName] = page;
    delete this.options.report.pages[oldName];
    page.properties.name = newName;
    this.options.pagesPanel.pagesContainer.updatePages();

    for (var componentName in page.components) {
        var component = page.components[componentName];
        if (component) {
            if (oldName == component.properties.parentName) component.properties.parentName = newName;
            component.properties.pageName = newName;
        }
    }
}

StiMobileDesigner.prototype.CheckLargeHeight = function (page, largeHeightAutoFactor) {
    if (page && page.properties.largeHeight == "0" && largeHeightAutoFactor != page.properties.largeHeightAutoFactor) {
        page.properties.largeHeightAutoFactor = largeHeightAutoFactor;
        page.repaint(true);
    }
}

StiMobileDesigner.prototype.MultiSelectComponents = function (page) {
    if (page && this.options.selectingRect) {
        var selectedObjects = this.GetComponentsInSelectingRect(page, this.options.selectingRect);
        this.DeleteSelectedLines();
        this.options.selectedObjects = selectedObjects;
        page.removeChild(this.options.selectingRect);
    }
    this.options.selectingRect = false;
}

StiMobileDesigner.prototype.GetComponentsInSelectingRect = function (page, selectingRect) {
    var components = [];
    var bands = [];
    var pointsAttribute = selectingRect.getAttribute("points");
    if (!pointsAttribute) return null;
    var points = pointsAttribute.split(" ");
    var leftRect, rightRect, topRect, bottomRect;
    for (var i = 0; i < points.length; i++) {
        var point = points[i].split(",");
        var xPoint = this.StrToDouble(point[0]);
        var yPoint = this.StrToDouble(point[1]);
        if (!leftRect) {
            leftRect = rightRect = xPoint;
            topRect = bottomRect = yPoint;
        }
        else {
            if (xPoint <= leftRect) leftRect = xPoint;
            if (xPoint >= rightRect) rightRect = xPoint;
            if (yPoint <= topRect) topRect = yPoint;
            if (yPoint >= bottomRect) bottomRect = yPoint;
        }
    }

    for (var name in page.components) {
        var component = page.components[name];
        if (component.style.display == "none") continue;
        var leftComp = this.StrToDouble(component.getAttribute("left"));
        var topComp = this.StrToDouble(component.getAttribute("top"));
        var rightComp = leftComp + this.StrToDouble(component.getAttribute("width"));
        var bottomComp = topComp + this.StrToDouble(component.getAttribute("height"));

        if (!(leftComp > rightRect || rightComp < leftRect || topComp > bottomRect || bottomComp < topRect)) {
            if (this.IsBandComponent(component)) bands.push(component);
            else components.push(component);
        }
    }

    var result = (components.length == 0) ? bands : components;
    if (this.options.SHIFT_pressed) {
        result = bands.concat(components);
    }

    return result.length == 0 ? null : result;
}

StiMobileDesigner.prototype.SelectAllComponentsOnPage = function (page) {
    var components = [];
    for (var componentName in page.components) {
        components.push(page.components[componentName]);
    }
    if (components.length > 0) {
        this.options.selectedObjects = components;
        this.PaintSelectedLines();
        this.UpdatePropertiesControls();
    }
}

StiMobileDesigner.prototype.GetSvgPageForCheckPreview = function (pageIndex, elementName, zoom, onlyPage, pageProperties) {
    var oldPaintPanelPadding = this.options.paintPanelPadding;
    this.options.paintPanelPadding = 0;
    var previewZoom = zoom || 0.4;

    var page = this.options.paintPanel.findPageByIndex(pageIndex);

    var pageObject = {
        name: page.properties.name,
        pageIndex: page.properties.pageIndex,
        properties: pageProperties || this.CopyObject(page.properties),
        components: page.components,
        valid: page.valid
    }

    var showGrid = this.options.report.info.showGrid;
    this.options.report.info.showGrid = false;

    var pageSvg = this.CreatePage(pageObject);
    pageSvg.isDashboard = page.isDashboard;
    pageSvg.onmousedown = null;
    pageSvg.ondblclick = null;
    pageSvg.onmousedown = null;
    pageSvg.onmousemove = null;
    pageSvg.onmouseup = null;
    pageSvg.style.display = "";
    pageSvg.removeAttribute("class");
    pageSvg.style.boxShadow = "";
    pageSvg.repaint();

    this.options.report.info.showGrid = showGrid;

    if (!onlyPage) {
        for (var compName in page.components) {
            var cloneComponent = this.CloneComponent(page.components[compName]);
            cloneComponent.onmousedown = null;
            cloneComponent.ondblclick = null;
            cloneComponent.onmousedown = null;
            cloneComponent.onmousemove = null;
            cloneComponent.onmouseup = null;
            if (compName == elementName) {
                cloneComponent.properties.border = "1,1,1,1!3!255,0,0!0!0!4";
            }
            pageSvg.appendChild(cloneComponent);
            cloneComponent.repaint();
        }
    }

    var width = parseInt(pageSvg.getAttribute("width"));
    var height = parseInt(pageSvg.getAttribute("height"));
    pageSvg.setAttribute("viewBox", "0,0," + width + "," + height);
    pageSvg.setAttribute("width", (previewZoom * width) / this.options.report.zoom);
    pageSvg.setAttribute("height", (previewZoom * height) / this.options.report.zoom);

    this.options.paintPanelPadding = oldPaintPanelPadding;

    return pageSvg
}