
StiMobileDesigner.prototype.InitializeMobileViewComponentsForm_ = function () {
    var jsObject = this;
    var form = this.DashboardBaseForm("mobileViewComponentsForm", this.loc.Report.Components, 2);
    form.hideButtonsPanel();

    var elementsContainer = this.UnplacedElementsContainer();
    form.container.appendChild(elementsContainer);

    form.show = function (elements) {
        if (jsObject.options.currentPage) {
            elementsContainer.clear();
            form.changeVisibleState(true);

            if (jsObject.options.buttons.pageViewComponentsButton)
                jsObject.options.buttons.pageViewComponentsButton.setSelected(true);
            if (jsObject.options.paintPanel.offsetHeight)
                elementsContainer.style.height = (jsObject.options.paintPanel.offsetHeight - 40) + "px";

            form.style.top = (parseInt(jsObject.options.paintPanel.style.top) + 5) + "px";
            form.style.left = (parseInt(jsObject.options.paintPanel.style.left) + jsObject.options.currentPage.widthPx + 20) + "px";

            if (elements) {
                for (var i = 0; i < elements.length; i++) {
                    elementsContainer.addItem(elements[i]);
                }
            }
        }
    }

    form.onhide = function () {
        if (jsObject.options.buttons.pageViewComponentsButton)
            jsObject.options.buttons.pageViewComponentsButton.setSelected(false);
    }

    return form;
}

StiMobileDesigner.prototype.UnplacedElementsContainer = function () {
    var jsObject = this;
    var container = this.EasyContainer(150, 500);

    container.addItem = function (itemObject) {
        var item = jsObject.UnplacedElementsContainerItem(itemObject);
        container.appendChild(item);
    }

    return container;
}

StiMobileDesigner.prototype.UnplacedElementsContainerItem = function (itemObject) {
    var jsObject = this;
    var item = this.StandartBigButton();
    item.itemObject = itemObject;
    item.style.minHeight = "auto";
    item.setAttribute("title", itemObject.name);
    var imgContainer = document.createElement("div");
    var imgCell = item.innerTable.addCell(imgContainer);
    imgCell.style.textAlign = "center";
    imgCell.style.padding = "6px";

    if (itemObject.svgContent) {
        var svgContent = StiBase64.decode(itemObject.svgContent);
        if (svgContent.indexOf("<svg") == 0) {
            var fixHeight = itemObject.type == "StiComboBoxElement" || itemObject.type == "StiTreeViewBoxElement" || itemObject.type == "StiDatePickerElement";
            svgContent = "<svg width='110' " + (!fixHeight ? "height='110' viewBox='0 0 240 240' " : "") + svgContent.substring(4);
        }
        imgContainer.innerHTML = svgContent;
    }

    item.onmousedown = function (event) {
        if (this.isTouchStartFlag) return;
        this.ontouchstart(event, true);
    }

    item.ontouchstart = function (event, mouseProcess) {
        var this_ = this;
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
        if (event && !this.isTouchStartFlag) event.preventDefault();
        if (event.button != 2) {
            var unplacedElementInDrag = jsObject.UnplacedElementForDragDrop(this.itemObject);
            unplacedElementInDrag.ownerButton = this;
            unplacedElementInDrag.beginingOffset = 0;
            jsObject.options.unplacedElementInDrag = unplacedElementInDrag;
        }
    }

    return item;
}