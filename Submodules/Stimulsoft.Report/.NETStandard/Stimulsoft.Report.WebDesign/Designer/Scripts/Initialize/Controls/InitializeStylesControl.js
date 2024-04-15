
StiMobileDesigner.prototype.StylesControl = function (name) {
    var stylesControl = document.createElement("div");
    var jsObject = stylesControl.jsObject = this;
    stylesControl.name = name;
    stylesControl.key = null;
    stylesControl.isEnabled = true;
    this.options.controls[name] = stylesControl;

    //Button
    var button = stylesControl.innerButton = this.BigButton(name + "Button", null, " ", true, [this.loc.PropertyMain.Style, this.HelpLinks["style"]], true);
    stylesControl.appendChild(button);

    //Override
    button.style.width = "120px";
    button.style.padding = "3px";
    button.innerTable.style.height = this.options.isTouchDevice ? "83px" : "63px";

    button.cellArrow.style.padding = "0 0 5px 0";
    button.cellImage.style.display = "none";

    button.caption.style.padding = "0px";
    button.caption.style.height = "100%";

    var styleBox = stylesControl.styleBox = document.createElement("div");
    styleBox.maxHeight = this.options.isTouchDevice ? 75 : 60;
    styleBox.style.maxHeight = styleBox.maxHeight + "px";
    styleBox.style.overflow = "hidden";
    styleBox.style.position = "relative";

    styleBox.addStyleName = function (styleName, right, bottom) {
        styleBox.clearStyleName();
        if (styleName) {
            var nameBox = document.createElement("div");
            this.appendChild(nameBox);
            this.nameBox = nameBox;
            nameBox.style.maxWidth = "108px";
            nameBox.style.padding = "2px 5px 2px 5px";
            nameBox.style.overflow = "hidden";
            nameBox.style.textOverflow = "ellipsis";
            nameBox.style.right = (right != null ? right : 1) + "px";
            nameBox.style.bottom = (bottom != null ? bottom : 3) + "px";
            nameBox.style.opacity = "0.85";
            nameBox.style.position = "absolute";
            nameBox.style.background = "#eeeeee";
            nameBox.innerHTML = styleName;
        }
    }

    styleBox.clearStyleName = function () {
        if (this.nameBox) {
            this.removeChild(this.nameBox);
            this.nameBox = null;
        }
    }

    var styleInnerBox = document.createElement("div");
    styleBox.appendChild(styleInnerBox);
    styleInnerBox.innerHTML = button.caption.innerHTML;
    button.caption.innerHTML = "";
    button.caption.appendChild(styleBox);
    styleInnerBox.style.position = "relative";
    styleInnerBox.style.maxWidth = "118px";

    button.setEnabled = function (state) {
        if (this.arrow) this.arrow.style.opacity = state ? "1" : "0.3";
        if (this.caption) this.caption.style.opacity = state ? "1" : "0.3";
        this.isEnabled = state;
        if (!state && !this.isOver) this.isOver = false;
        this.className = state ? (this.isOver ? this.overClass : this.defaultClass) : this.disabledClass;
    }

    button.action = function () {
        jsObject.options.menus[stylesControl.name + "Menu"].changeVisibleState(!jsObject.options.menus[stylesControl.name + "Menu"].visible);
    }

    //Menu
    stylesControl.menu = this.VerticalMenu(name + "Menu", button, "Down");

    stylesControl.menu.action = function (menuItem) {
        menuItem.setSelected(true);
        this.changeVisibleState(false);
        stylesControl.setKey(menuItem.key);
        stylesControl.action();
    }

    stylesControl.menu.onshow = function () {
        if (stylesControl.typeComponent == "StiChart" || stylesControl.typeComponent == "StiCrossTab" ||
            stylesControl.typeComponent == "StiGauge" || stylesControl.typeComponent == "StiMap" ||
            stylesControl.typeComponent == "StiTable" || stylesControl.typeComponent == "StiDashboard" ||
            stylesControl.typeComponent == "StiSparkline" || jsObject.DbsElementHaveStyles(stylesControl.typeComponent)) {
            stylesControl.scrollToSelectedItem();
            return;
        }

        var stylesContent = jsObject.GetComponentStyleItems(true);

        //add styles
        this.addItems(stylesContent.styleItems);

        //add collections
        if (stylesContent.styleCollections.length > 0) {
            var firstItem = this.innerContent.childNodes.length > 0 ? this.innerContent.childNodes[0] : null;

            for (var i = 0; i < stylesContent.styleCollections.length; i++) {
                var collectionName = stylesContent.styleCollections[i];
                var collectionItem = jsObject.VerticalMenuItem(this, "cItem" + collectionName, collectionName, "CloudItems.BigFolder.png", collectionName, "stiDesignerMenuMiddleItem");

                collectionItem.action = function () {
                    var itemKey = this.key;
                    stylesControl.menu.changeVisibleState(false);
                    var messageForm = jsObject.MessageFormForApplyStyles();
                    messageForm.changeVisibleState(true);
                    messageForm.action = function (state) {
                        if (state) {
                            jsObject.SendCommandUpdateStyles(null, itemKey);
                        }
                    }
                }
                if (firstItem)
                    this.innerContent.insertBefore(collectionItem, firstItem);
                else
                    this.innerContent.appendChild(collectionItem);
            }
            var sep = jsObject.VerticalMenuSeparator(this, "CollectionsSeparator");
            sep.style.margin = "3px 2px 3px 2px";

            if (firstItem)
                this.innerContent.insertBefore(sep, firstItem);
            else
                this.innerContent.appendChild(sep);
        }

        for (var itemName in this.items)
            if (this.items[itemName].key == stylesControl.key)
                this.items[itemName].setSelected(true);

        stylesControl.scrollToSelectedItem();
    }

    stylesControl.scrollToSelectedItem = function () {
        setTimeout(function () {
            if (stylesControl.menu.items && stylesControl.key != "[None]") {

                for (var itemName in stylesControl.menu.items) {
                    var yPos = jsObject.FindPosY(stylesControl.menu.items[itemName], null, false, stylesControl.menu.innerContent);
                    if (stylesControl.menu.items[itemName].isSelected && yPos + 65 > stylesControl.menu.offsetHeight) {
                        stylesControl.menu.innerContent.scrollTop = yPos - 30;
                        return;
                    }
                }
            }
            stylesControl.menu.innerContent.scrollTop = 0;
        }, jsObject.options.menuAnimDuration);
    }

    stylesControl.addItemsToMenu = function (stylesContent) {
        styleBox.clearStyleName();

        while (this.menu.innerContent.childNodes[0]) {
            this.menu.innerContent.removeChild(this.menu.innerContent.childNodes[0]);
        }

        if (!stylesContent) return;

        var isEmpty = true;
        styleInnerBox.innerHTML = this.key != "StiEmptyValue" ? jsObject.loc.FormConditions.SelectStyle : "";
        stylesControl.menu.innerContent.style.maxHeight = "500px";
        stylesControl.menu.innerContent.style.width = "min-content";
        stylesControl.menu.style.width = "auto";

        if (this.typeComponent == "StiMap" || this.typeComponent == "StiGauge" || this.typeComponent == "StiChart") {

            for (var i = 0; i < stylesContent.length; i++) {
                var item;

                switch (this.typeComponent) {
                    case "StiMap": item = jsObject.VerticalMenuItemForMapStyles(this.menu, stylesContent[i]); break;
                    case "StiGauge": item = jsObject.VerticalMenuItemForGaugeStyles(this.menu, stylesContent[i]); break;
                    case "StiChart": item = jsObject.VerticalMenuItemForChartStyles(this.menu, stylesContent[i]); break;
                }

                this.menu.innerContent.appendChild(item);

                if (this.key && this.key.name == stylesContent[i].name && this.key.type == stylesContent[i].type) {
                    item.setSelected(true);
                    styleInnerBox.innerHTML = stylesContent[i].image;
                    var styleName = stylesContent[i].name;
                    if (!styleName) {
                        if (this.typeComponent == "StiChart" && stylesContent[i].type) {
                            styleName = stylesContent[i].type.replace("Sti", "");
                        }
                    }
                    styleBox.addStyleName(styleName, this.typeComponent == "StiChart" ? 2 : null);
                    isEmpty = false;
                }
            }
            this.repaint(isEmpty);
        }
        else if (this.typeComponent == "StiCrossTab") {
            var crossTabStyles = [];
            if (jsObject.options.report.stylesCollection) {
                for (var i = 0; i < jsObject.options.report.stylesCollection.length; i++) {
                    if (jsObject.options.report.stylesCollection[i].type == "StiCrossTabStyle") {
                        crossTabStyles.push({
                            key: {
                                crossTabStyle: jsObject.options.report.stylesCollection[i].properties.name
                            },
                            properties: jsObject.options.report.stylesCollection[i].properties
                        });
                    }
                }
            }

            for (var i = 0; i < stylesContent.length; i++) {
                crossTabStyles.push({
                    key: {
                        crossTabStyle: stylesContent[i].properties.name,
                        crossTabStyleIndex: i
                    },
                    properties: stylesContent[i].properties
                });
            }

            for (var i = 0; i < crossTabStyles.length; i++) {
                var item = jsObject.VerticalMenuItemForCrossTabStyles(this.menu, crossTabStyles[i]);
                this.menu.innerContent.appendChild(item);

                if ((crossTabStyles[i].key.crossTabStyleIndex != null && this.key.crossTabStyleIndex == crossTabStyles[i].key.crossTabStyleIndex) ||
                    (crossTabStyles[i].key.crossTabStyleIndex == null && this.key.crossTabStyle == crossTabStyles[i].key.crossTabStyle)) {
                    item.setSelected(true);
                    isEmpty = false;
                    styleInnerBox.innerHTML = item.tableContainer.innerHTML;
                    styleBox.addStyleName(crossTabStyles[i].key.crossTabStyle, 2, 1);
                }
            }

            this.repaint(isEmpty);
        }
        else if (this.typeComponent == "StiTable") {
            this.menu.addItems([jsObject.Item("styleNone", jsObject.loc.Report.No, null, "[None]", { type: "StiStyle", font: "Arial!8!0!0!0!0", brush: "1!transparent", textBrush: "1!0,0,0", border: "default" })]);

            for (var i = 0; i < stylesContent.length; i++) {
                var item = jsObject.VerticalMenuItemForTableStyles(this.menu, stylesContent[i]);
                this.menu.innerContent.appendChild(item);

                var isSelected = this.key.styleName && this.key.styleName != "[None]"
                    ? this.key.styleName == stylesContent[i].properties.name
                    : this.key.styleId == stylesContent[i].styleId;

                if (isSelected) {
                    item.setSelected(true);
                    styleInnerBox.innerHTML = item.tableContainer.innerHTML;
                    styleBox.addStyleName(stylesContent[i].properties.name || stylesContent[i].styleId, 2, 1);
                    isEmpty = false;
                }
            }

            this.repaint(isEmpty);
        }
        else if (this.typeComponent == "StiSparkline") {
            this.menu.addItems([jsObject.Item("styleNone", jsObject.loc.Report.No, null, "[None]", { type: "StiStyle", font: "Arial!8!0!0!0!0", brush: "1!transparent", textBrush: "1!0,0,0", border: "default" })]);

            if (this.key == "[None]")
                this.menu.items.styleNone.setSelected(true);

            for (var i = 0; i < stylesContent.length; i++) {
                var item = jsObject.VerticalMenuItemForSparklineStyles(this.menu, stylesContent[i]);
                this.menu.innerContent.appendChild(item);

                if (this.key && this.key == stylesContent[i].name) {
                    item.setSelected(true);
                    styleInnerBox.innerHTML = stylesContent[i].image;
                    styleBox.addStyleName(stylesContent[i].name, 2, 1);
                    isEmpty = false;
                }
            }

            this.repaint(isEmpty);
        }
        else if (this.typeComponent == "StiDashboard") {
            for (var i = 0; i < stylesContent.length; i++) {
                var item = jsObject.VerticalMenuItemForDashboardStyles(this.menu, stylesContent[i], true);
                this.menu.innerContent.appendChild(item);
                if (this.key.ident == stylesContent[i].ident) {
                    item.setSelected(true);
                    styleInnerBox.innerHTML = item.styleContainer.innerHTML;
                    isEmpty = false;
                }
            }
            this.repaint(isEmpty);
        }
        else if (this.typeComponent == "StiTableElement" || this.typeComponent == "StiPivotTableElement" || this.typeComponent == "StiCardsElement" ||
            this.typeComponent == "StiRegionMapElement" || this.typeComponent == "StiChartElement" ||
            this.typeComponent == "StiGaugeElement" || this.typeComponent == "StiProgressElement" ||
            this.typeComponent == "StiIndicatorElement" || this.typeComponent == "StiListBoxElement" ||
            this.typeComponent == "StiComboBoxElement" || this.typeComponent == "StiTreeViewElement" ||
            this.typeComponent == "StiTreeViewBoxElement" || this.typeComponent == "StiDatePickerElement" || this.typeComponent == "StiButtonElement") {

            var item;

            for (var i = 0; i < stylesContent.length; i++) {
                switch (this.typeComponent) {
                    case "StiTableElement": item = jsObject.VerticalMenuItemForTableOrPivotTableElementsStyles(this.menu, stylesContent[i]); break;
                    case "StiPivotTableElement": item = jsObject.VerticalMenuItemForTableOrPivotTableElementsStyles(this.menu, stylesContent[i], true); break;
                    case "StiRegionMapElement": item = jsObject.VerticalMenuItemForRegionMapElementStyles(this.menu, stylesContent[i]); break;
                    case "StiChartElement": item = jsObject.VerticalMenuItemForChartElementStyles(this.menu, stylesContent[i]); break;
                    case "StiGaugeElement": item = jsObject.VerticalMenuItemForGaugeElementStyles(this.menu, stylesContent[i]); break;
                    case "StiProgressElement": item = jsObject.VerticalMenuItemForProgressElementStyles(this.menu, stylesContent[i]); break;
                    case "StiIndicatorElement": item = jsObject.VerticalMenuItemForIndicatorElementStyles(this.menu, stylesContent[i]); break;
                    case "StiCardsElement": item = jsObject.VerticalMenuItemForCardsElementStyles(this.menu, stylesContent[i]); break;
                    case "StiButtonElement":
                    case "StiListBoxElement":
                    case "StiComboBoxElement":
                    case "StiTreeViewElement":
                    case "StiTreeViewBoxElement":
                    case "StiDatePickerElement": item = jsObject.VerticalMenuItemForFilterElementStyles(this.menu, stylesContent[i]); break;
                }

                this.menu.innerContent.appendChild(item);

                var isSelected = this.key.ident == "Custom"
                    ? this.key.name == stylesContent[i].name
                    : this.key.ident == stylesContent[i].ident;

                if (isSelected) {
                    item.setSelected(true);
                    styleInnerBox.innerHTML = item.styleContainer.innerHTML;

                    if (stylesContent[i].ident != "Auto" && !jsObject.IsFilterElement(this.typeComponent) && this.typeComponent != "StiButtonElement") {
                        var styleName = stylesContent[i].name || stylesContent[i].ident;
                        var rightPos = 1;
                        var bottomPos = 3;

                        if (this.typeComponent == "StiTableElement" || this.typeComponent == "StiPivotTableElement")
                            bottomPos = 1;
                        else if (this.typeComponent == "StiChartElement")
                            rightPos = 2;
                        else if (this.typeComponent == "StiGaugeElement")
                            bottomPos = 1;

                        styleBox.addStyleName(styleName, rightPos, bottomPos);
                    }
                    isEmpty = false;
                }

                if (stylesContent[i].ident == "Auto") {
                    this.menu.innerContent.appendChild(jsObject.VerticalMenuSeparator());
                }
            }

            this.repaint(isEmpty);
        }
    }

    stylesControl.updateItemsAndSetKey = function () {
        var jsObject = this.jsObject;
        var typeComponent = this.typeComponent;

        var componentName = jsObject.options.selectedObject ? jsObject.options.selectedObject.properties.name : null;
        if (!componentName && typeComponent && jsObject.options.selectedObjects && jsObject.options.selectedObjects.length > 0) {
            componentName = jsObject.options.selectedObjects[0].properties.name;
        }

        var updatePropertyGridStyleControl = function (styleItems) {
            if (jsObject.options.controls.controlPropertyElementStyle && styleItems) {
                var items = [];
                for (var i = 0; i < styleItems.length; i++) {
                    var itemKey = { ident: styleItems[i].ident, name: styleItems[i].name };
                    var itemName = styleItems[i].name || styleItems[i].ident;

                    items.push(jsObject.Item(itemName, itemName, null, itemKey));

                    if (styleItems[i].ident == "Auto" || (styleItems[i].ident == "Custom" && i < styleItems.length - 1 && styleItems[i + 1].ident != "Custom")) {
                        items.push("separator");
                    }
                }
                jsObject.options.controls.controlPropertyElementStyle.addItems(items);
            }
        }

        //Chart Styles
        if (typeComponent == "StiChart") {
            var chartStyles = componentName && jsObject.options.report.stylesCache ? jsObject.options.report.stylesCache.chartStyles[componentName] : null;
            if (!chartStyles) {
                jsObject.SendCommandToDesignerServer("GetChartStylesContent", { componentName: componentName },
                    function (answer) {
                        if (typeComponent == stylesControl.typeComponent) {
                            jsObject.AddStylesToCache(componentName, answer.stylesContent, "StiChart");
                            stylesControl.addItemsToMenu(answer.stylesContent);
                        }
                    });
                return;
            }
            stylesControl.addItemsToMenu(chartStyles);
        }
        //CrossTab Styles
        else if (typeComponent == "StiCrossTab") {
            var crossTabStyles = jsObject.options.report.stylesCache ? jsObject.options.report.stylesCache.crossTabStyles : null;
            if (!crossTabStyles) {
                jsObject.SendCommandGetCrossTabStylesContent(function (crossTabStylesContent) {
                    if (typeComponent == stylesControl.typeComponent) {
                        jsObject.AddStylesToCache(null, crossTabStylesContent, "StiCrossTab");
                        stylesControl.addItemsToMenu(crossTabStylesContent);
                    }
                });
                return;
            }
            stylesControl.addItemsToMenu(crossTabStyles);
        }
        //Table Styles
        else if (typeComponent == "StiTable") {
            var tableStyles = jsObject.options.report.stylesCache ? jsObject.options.report.stylesCache.tableStyles : null;
            if (!tableStyles) {
                jsObject.SendCommandGetTableStylesContent(function (tableStylesContent) {
                    if (typeComponent == stylesControl.typeComponent) {
                        jsObject.AddStylesToCache(null, tableStylesContent, "StiTable");
                        stylesControl.addItemsToMenu(tableStylesContent);
                    }
                });
                return;
            }
            stylesControl.addItemsToMenu(tableStyles);
        }
        //Gauge Styles
        else if (typeComponent == "StiGauge") {
            var gaugeStyles = componentName && jsObject.options.report.stylesCache ? jsObject.options.report.stylesCache.gaugeStyles[componentName] : null;
            if (!gaugeStyles) {
                jsObject.SendCommandGetGaugeStylesContent(function (gaugeStylesContent) {
                    if (typeComponent == stylesControl.typeComponent) {
                        jsObject.AddStylesToCache(componentName, gaugeStylesContent, "StiGauge");
                        stylesControl.addItemsToMenu(gaugeStylesContent);
                    }
                });
            }
            stylesControl.addItemsToMenu(gaugeStyles);
            return;
        }
        //Sparkline Styles
        else if (typeComponent == "StiSparkline") {
            var sparklineStyles = jsObject.options.report.stylesCache ? jsObject.options.report.stylesCache.sparklineStyles : null;
            if (!sparklineStyles) {
                jsObject.SendCommandGetSparklineStylesContent(function (sparklineStylesContent) {
                    if (typeComponent == stylesControl.typeComponent) {
                        jsObject.AddStylesToCache(null, sparklineStylesContent, "StiSparkline");
                        stylesControl.addItemsToMenu(sparklineStylesContent);
                    }
                });
                return;
            }
            stylesControl.addItemsToMenu(sparklineStyles);
        }
        //Map Styles
        else if (typeComponent == "StiMap") {
            var mapStyles = componentName && jsObject.options.report.stylesCache ? jsObject.options.report.stylesCache.mapStyles[componentName] : null;
            if (!mapStyles) {
                jsObject.SendCommandGetMapStylesContent(function (mapStylesContent) {
                    if (typeComponent == stylesControl.typeComponent) {
                        jsObject.AddStylesToCache(componentName, mapStylesContent, "StiMap");
                        stylesControl.addItemsToMenu(mapStylesContent);
                    }
                });
                return;
            }
            stylesControl.addItemsToMenu(mapStyles);
        }
        //Dashboard & DashboardElements
        else if (typeComponent == "StiDashboard" || jsObject.DbsElementHaveStyles(typeComponent)) {
            var stylesFromCache = componentName && jsObject.options.report.stylesCache ? jsObject.options.report.stylesCache[componentName] : null;

            if (!stylesFromCache) {
                jsObject.SendCommandToDesignerServer("GetDashboardStylesContent",
                    {
                        typeComponent: typeComponent,
                        componentName: componentName
                    },
                    function (answer) {
                        if (typeComponent == stylesControl.typeComponent) {
                            jsObject.AddStylesToCache(componentName, answer.stylesContent);
                            stylesControl.addItemsToMenu(answer.stylesContent);
                            updatePropertyGridStyleControl(answer.stylesContent);
                        }
                    });
                return;
            }
            stylesControl.addItemsToMenu(stylesFromCache);
            updatePropertyGridStyleControl(stylesFromCache);
        }
    }

    //Override 
    stylesControl.setKey = function (key) {
        this.key = key;
        var commonSelectedObject = this.jsObject.options.selectedObject || this.jsObject.GetCommonObject(this.jsObject.options.selectedObjects);
        this.typeComponent = (commonSelectedObject && commonSelectedObject.typeComponent)
            ? (commonSelectedObject.isDashboard ? "StiDashboard" : commonSelectedObject.typeComponent) : "Any";

        styleBox.clearStyleName();

        if (this.jsObject.options.report && (this.typeComponent == "StiChart" || this.typeComponent == "StiCrossTab" ||
            this.typeComponent == "StiGauge" || this.typeComponent == "StiMap" || this.typeComponent == "StiTable" ||
            this.typeComponent == "StiDashboard" || this.typeComponent == "StiSparkline" || this.jsObject.DbsElementHaveStyles(this.typeComponent))) {
            this.updateItemsAndSetKey();
        }
        else {
            styleInnerBox.innerHTML = key != "[None]"
                ? ((key == "StiEmptyValue" || (key.ident && key.ident == "StiEmptyValue") || (key.name && key.name == "StiEmptyValue")) ? "" : (typeof key == "string" ? key : ""))
                : this.jsObject.loc.FormConditions.SelectStyle;
            this.repaint();
        }
    };

    stylesControl.setEnabled = function (state) {
        this.isEnabled = state;
        button.setEnabled(state);
    }

    stylesControl.repaint = function (isEmpty) {
        var font = "Arial!8!0!0!0!0";
        var brush = "1!255,255,255";
        var textBrush = "1!0,0,0";
        var border = "default";

        styleInnerBox.style.top = "0px";
        styleInnerBox.style.margin = "0px";

        if (this.typeComponent == "StiChart" || this.typeComponent == "StiGauge" || this.typeComponent == "StiCrossTab" || this.typeComponent == "StiMap" ||
            this.typeComponent == "StiTable" || this.typeComponent == "StiSparkline" || this.typeComponent == "StiDashboard" || this.jsObject.DbsElementHaveStyles(this.typeComponent)) {
            this.jsObject.RepaintControlByAttributes(button.caption, font, brush, textBrush, border);

            if (!isEmpty && this.key != "[None]" && this.key != "StiEmptyValue" && !(this.jsObject.DbsElementHaveStyles(this.typeComponent) && this.key && this.key.ident == "Auto")) {
                button.caption.style.border = "0px";
                if (this.typeComponent == "StiChart" || this.typeComponent == "StiChartElement") {
                    styleInnerBox.style.margin = "-10px -10px -9px -10px";
                }
                else if (this.typeComponent == "StiGauge" || this.typeComponent == "StiGaugeElement") {
                    styleInnerBox.style.margin = "0px 0px 0px -2px";
                }
                else if (this.typeComponent == "StiMap" || this.typeComponent == "StiRegionMapElement") {
                    styleInnerBox.style.margin = "0 0 0 -5px";
                }
            }
        }
        else {
            if (this.jsObject.options.report) {
                var stylesCollection = this.jsObject.options.report.stylesCollection;
                for (var i = 0; i < stylesCollection.length; i++) {
                    var properties = stylesCollection[i].properties;
                    if (properties.name == this.key) {
                        if (properties["font"] != null && properties["allowUseFont"]) font = properties.font;
                        if (properties["brush"] != null && properties["allowUseBrush"]) brush = properties.brush;
                        if (properties["textBrush"] != null && properties["allowUseTextBrush"]) textBrush = properties.textBrush;
                        if (properties["border"] != null && properties["allowUseBorderSides"]) border = properties.border;
                    }
                }
            }
            var styleInnerBoxHeight = parseInt(font.split("!")[1]) * 1.33;
            styleInnerBox.style.top = (styleInnerBoxHeight > styleBox.maxHeight ? ((styleInnerBoxHeight - styleBox.maxHeight) / 2 * -1) : 0) + "px";
            this.jsObject.RepaintControlByAttributes(button.caption, font, brush, textBrush, border);
        }

        if (isEmpty || this.key == "[None]" || this.key == "StiEmptyValue" || (this.jsObject.DbsElementHaveStyles(this.typeComponent) && this.key && this.key.ident == "Auto")) {
            button.caption.style.fontSize = "12px";
        }
    }

    stylesControl.action = function () { };

    return stylesControl;
}