
StiMobileDesigner.prototype.InitializeChangeDbsElementTypeMenu = function () {
    var jsObject = this;
    var dbsElements = ["StiTableElement", "StiCardsElement", "StiPivotTableElement", "StiChartElement", "StiGaugeElement", "StiIndicatorElement", "StiProgressElement",
        "StiRegionMapElement", "StiOnlineMapElement", "StiComboBoxElement", "StiDatePickerElement", "StiListBoxElement", "StiTreeViewBoxElement", "StiTreeViewElement"];

    var menu = this.BaseContextMenu("changeDbsElementTypeMenu", "Down", []);
    var innerContent = menu.innerContent;
    innerContent.style.minWidth = "192px";
    innerContent.style.padding = "1px";
    innerContent.style.lineHeight = "0";
    innerContent.style.overflow = "hidden";
    menu.buttons = {};

    for (var i = 0; i < dbsElements.length; i++) {
        var button = this.StandartSmallButton(null, null, null, "Dashboards.BigComponents." + dbsElements[i] + ".png", jsObject.loc.Components[dbsElements[i].replace("Element", "")], null, null, { width: 32, height: 32 });
        button.elementType = dbsElements[i];
        button.style.display = "inline-block";
        button.innerTable.style.width = "100%";
        button.style.margin = "1px";
        button.style.width = button.style.height = "44px";
        innerContent.appendChild(button);
        menu.buttons[button.elementType] = button;

        button.action = function () {
            this.select();
            menu.action(this.elementType);
        }

        button.select = function () {
            for (var name in menu.buttons) {
                menu.buttons[name].setSelected(menu.buttons[name] == this);
            }
        }
    }

    menu.show = function (component) {
        if (component) {
            this.innerContent.style.top = "0px";
            this.style.display = "";
            this.component = component;
            this.component.originalElementContent = null;
            this.component.originalElementType = component.typeComponent;

            var selButton = menu.buttons[component.typeComponent];
            if (selButton) selButton.select();

            var browserHeight = (window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight) - jsObject.FindPosY(jsObject.options.mainPanel);
            var browserWidth = (window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth) - jsObject.FindPosX(jsObject.options.mainPanel);
            var compWidth = parseInt(component.getAttribute("width"));
            var compLeft = parseInt(component.getAttribute("left"));
            var compTop = parseInt(component.getAttribute("top"));
            var pagePositions = jsObject.FindPagePositions(jsObject.options.mainPanel);
            var changeTypeDbsButton = component.controls.changeTypeDbsButton;

            var topPos = pagePositions.posY + compTop + 3;
            if (component.controls.editDbsButton) topPos += 35;
            if (component.controls.filtersDbsButton) topPos += 35;
            if (component.controls.topNDbsButton) topPos += 35;

            var leftPos = pagePositions.posX + compLeft + compWidth + 44;
            if (changeTypeDbsButton) {
                if (parseInt(changeTypeDbsButton.getAttribute("x")) == 8) leftPos = pagePositions.posX + compLeft + 44;
                else if (parseInt(changeTypeDbsButton.getAttribute("x")) < 0) leftPos = pagePositions.posX + compLeft;
            }

            if (topPos + this.innerContent.offsetHeight > browserHeight) {
                topPos = browserHeight - this.innerContent.offsetHeight - 10;
            }
            if (leftPos + this.innerContent.offsetWidth > browserWidth) {
                leftPos = browserWidth - this.innerContent.offsetWidth - 10;
            }

            this.style.left = leftPos + "px";
            this.style.top = topPos + "px";
            jsObject.options.currentMenu = this;
        }
    }

    menu.action = function (elementType) {
        if (this.component) {
            jsObject.SendCommandChangeTypeElement(this.component, elementType);
        }
    }

    return menu;
}