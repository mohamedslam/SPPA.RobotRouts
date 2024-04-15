
StiJsViewer.prototype.AddDashboardElementToPage = function (page, elementAttributes) {
    var jsObject = this;
    var element = document.createElement("div");
    element.elementAttributes = elementAttributes;
    element.buttons = {};
    element.style.display = "inline-block";
    element.style.position = "absolute";
    element.style.overflow = "hidden";
    element.style.top = elementAttributes.top + "px";
    element.style.left = elementAttributes.left + "px";
    element.style.width = elementAttributes.width + "px";
    element.style.height = elementAttributes.height + "px";
    element.style.margin = elementAttributes.margin.split(",").join("px ") + "px";
    element.style.backgroundColor = elementAttributes.backColor;
    element.style.boxSizing = "border-box";
    page.appendChild(element);

    if (!this.isFilterElement(element.elementAttributes.type)) {
        var contentAttrs = elementAttributes.contentAttributes;
        var imagesPath = "Dashboards.Actions." + (element.elementAttributes.actionColors && element.elementAttributes.actionColors.isDarkStyle ? "Dark." : "Light.");

        //filters string panel
        var filtersStringPanel = document.createElement("div");
        filtersStringPanel.className = "stiJsViewerDashboardElementButtonsPanel";
        element.appendChild(filtersStringPanel);
        element.filtersStringPanel = filtersStringPanel;

        filtersStringPanel.checkVisibleState = function () {
            this.style.opacity = element.isMouseOver ? 0 : 0.9;
        }

        //buttons panel
        var buttonsPanel = document.createElement("div");
        buttonsPanel.className = "stiJsViewerDashboardElementButtonsPanel";
        element.appendChild(buttonsPanel);
        element.buttonsPanel = buttonsPanel;

        buttonsPanel.saveVisibleButtons = function () {
            element.visibleButtons = [];

            for (var name in element.buttons) {
                var button = element.buttons[name];
                if (button.style.display != "none") {
                    element.visibleButtons.push(button)
                }
            }
        }

        buttonsPanel.checkVisibleState = function () {
            var allwaysShowViewStates = contentAttrs.interaction && contentAttrs.interaction.viewsState == "Always";

            this.style.opacity = element.isMouseOver || allwaysShowViewStates ? 0.9 : 0;

            if (allwaysShowViewStates && element.visibleButtons) {
                for (var i = 0; i < element.visibleButtons.length; i++) {
                    element.visibleButtons[i].style.display = element.isMouseOver ? "" : "none";
                }
            }
        }

        buttonsPanel.updateViewsStates = function () {
            if (contentAttrs.interaction && contentAttrs.interaction.viewsState == "Always") {
                buttonsPanel.saveVisibleButtons();
                buttonsPanel.checkVisibleState();
            }
        }

        var corners = element.elementAttributes.cornerRadius;
        if (corners && parseInt(corners.topRight) > 10) {
            buttonsPanel.style.right = filtersStringPanel.style.right = "15px";
        }

        var buttonsTable = this.CreateHTMLTable();
        buttonsPanel.appendChild(buttonsTable);
        buttonsTable.className = "stiJsViewerToolBarTable";
        buttonsTable.style.border = 0;
        buttonsTable.style.margin = 0;
        buttonsTable.style.background = "transparent";
        buttonsTable.style.boxSizing = "border-box";

        // User View States
        var userViewStates = contentAttrs.userViewStates;

        if (userViewStates && userViewStates.length > 1 && contentAttrs.dataMode != "ManuallyEnteringData") {
            element.viewStateButtons = [];

            for (var i = 0; i < userViewStates.length; i++) {
                var isSelected = userViewStates[i].key == contentAttrs.selectedViewStateKey;
                var isDark = elementAttributes.actionColors && elementAttributes.actionColors.isDarkStyle;
                var buttonViewState = this.DbsElementToolButton(null, null, "Dashboards.Charts." + (isDark ? "Dark" : "Light") + "." + userViewStates[i].seriesType + ".png", userViewStates[i].name, null, null, elementAttributes.actionColors);
                buttonViewState.viewState = userViewStates[i];
                buttonViewState.style.border = "1px solid " + (isSelected ? (isDark ? "#eeeeee" : "#c6c6c6") : "transparent");
                element.viewStateButtons.push(buttonViewState);
                buttonsTable.addCell(buttonViewState);

                buttonViewState.action = function () {
                    for (var i = 0; i < element.viewStateButtons.length; i++) {
                        element.viewStateButtons[i].setSelected(false);
                    }
                    this.setSelected(true);
                    jsObject.ChangeChartElementViewState(element, this.viewState.key);
                };

                buttonViewState.setSelected = function (state) {
                    this.style.border = "1px solid " + (state ? (isDark ? "#eeeeee" : "#c6c6c6") : "transparent");
                };
            }
        }

        // Select columns
        if (element.elementAttributes.type == "StiTableElement" && contentAttrs.interaction.allowUserColumnSelection) {
            var buttonSelectColumns = this.DbsElementToolButton(null, null, imagesPath + "ColumnSelection.png",
                [this.collections.loc["SelectColumns"], this.helpLinks["DashboardElementToolbar"], { top: "auto" }], null, null, elementAttributes.actionColors);
            buttonsTable.addCell(buttonSelectColumns);
            element.buttons.selectColumns = buttonSelectColumns;

            buttonSelectColumns.action = function () {
                var menuStyle = (elementAttributes.actionColors && elementAttributes.actionColors.isDarkStyle) ? "stiJsViewerDbsDarkMenu" : "stiJsViewerDbsLightMenu";
                var selectColumnsMenu = jsObject.InitializeSelectColumnsMenu(element, menuStyle + "Item", menuStyle);
                selectColumnsMenu.changeVisibleState(true, this);
            };

            buttonSelectColumns.setSelectedBase = buttonSelectColumns.setSelected;

            buttonSelectColumns.setSelected = function (state) {
                buttonSelectColumns.setSelectedBase(state);
                buttonsPanel.checkVisibleState();
                filtersStringPanel.checkVisibleState();
            };

            if (contentAttrs.data.length > 0 && contentAttrs.data[0].length == 0 && contentAttrs.hiddenData.length > 0 && contentAttrs.hiddenData[0].length == 0) {
                buttonSelectColumns.style.display = "none";
            }
        }

        // Sort button
        var sortItems = element.elementAttributes.contentAttributes.sortItems;

        if (sortItems && sortItems.length > 0) {
            var buttonSort = this.DbsElementToolButton(null, null, imagesPath + "Sort.png",
                [this.collections.loc["Sort"], this.helpLinks["DashboardElementToolbar"], { top: "auto" }], null, null, elementAttributes.actionColors);
            buttonsTable.addCell(buttonSort);
            element.buttons.sort = buttonSort;

            buttonSort.action = function () {
                var menuStyle = (elementAttributes.actionColors && elementAttributes.actionColors.isDarkStyle) ? "stiJsViewerDbsDarkMenu" : "stiJsViewerDbsLightMenu";
                var sortMenu = jsObject.DbsElementSortMenu(element, menuStyle + "Item", menuStyle);
                sortMenu.changeVisibleState(true, this);
            };

            buttonSort.setSelectedBase = buttonSort.setSelected;

            buttonSort.setSelected = function (state) {
                buttonSort.setSelectedBase(state);
                buttonsPanel.checkVisibleState();
                filtersStringPanel.checkVisibleState();
            };
        }

        // MultiFilter button
        var buttonMultiFilter = this.DbsElementToolButton(null, null, imagesPath + "FilterMultiOptionOff.png",
            [this.collections.loc["FilterMode"], this.helpLinks["DashboardElementToolbar"], { top: "auto" }], null, null, elementAttributes.actionColors);
        buttonMultiFilter.isChecked = false;
        buttonsTable.addCell(buttonMultiFilter);
        element.buttons.multiFilter = buttonMultiFilter;

        buttonMultiFilter.setChecked = function (state) {
            this.isChecked = state;
            StiJsViewer.setImageSource(this.image, jsObject.options, jsObject.collections, imagesPath + "FilterMultiOption" + (state ? "On" : "Off") + ".png");
        };

        buttonMultiFilter.action = function () {
            this.setChecked(!this.isChecked);
            var options = jsObject.options;

            if (!options.multiFilterStates) {
                options.multiFilterStates = {};
            }
            if (this.isChecked) {
                options.multiFilterStates[elementAttributes.name] = true;
            }
            else {
                delete options.multiFilterStates[elementAttributes.name];
            }
        };

        // RemoveFilter button
        var buttonRemoveFilter = this.DbsElementToolButton(null, null, imagesPath + "RemoveFilter.png",
            [this.collections.loc["RemoveFilter"], this.helpLinks["DashboardElementToolbar"], { top: "auto" }], null, null, elementAttributes.actionColors);
        buttonsTable.addCell(buttonRemoveFilter);
        element.buttons.removeFilter = buttonRemoveFilter;

        buttonRemoveFilter.action = function () {
            this.style.display = "none";
            buttonMultiFilter.style.display = "none";

            if (element["resetAllSelectedGeoms"]) {
                element.resetAllSelectedGeoms();
            }

            if (element.elementAttributes && element.elementAttributes.contentAttributes) {
                element.elementAttributes.contentAttributes.filtersString = "";
                jsObject.UpdateFiltersStringPanel(element);
            }

            jsObject.ApplyFiltersToDashboardElement(element, [], true);
        };

        //DrillDown Buttons
        if (contentAttrs.interaction && contentAttrs.interaction.allowUserDrillDown) {
            var buttonsProps = [
                ["drillDownSelected", this.collections.loc["DrillDownSelected"], null, null],
                ["drillDownCancel", this.collections.loc["ButtonCancel"], null, null],
                ["drillUp", null, imagesPath + "DrillUp.png", this.collections.loc["DrillUp"]],
                ["drillDown", null, imagesPath + "DrillDown.png", this.collections.loc["DrillDown"]]
            ];

            for (var i = 0; i < buttonsProps.length; i++) {
                var button = this.DbsElementToolButton(null, buttonsProps[i][1], buttonsProps[i][2], buttonsProps[i][3], null, null, elementAttributes.actionColors);
                element.buttons[buttonsProps[i][0]] = button;
                buttonsTable.addCell(button);

                if (buttonsProps[i][0] == "drillDownSelected" || buttonsProps[i][0] == "drillDownCancel") {
                    button.style.marginLeft = "4px";
                    button.showBorders = true;
                    button.applyStyleColors(elementAttributes.actionColors);
                }
            }

            element.buttons.drillDownSelected.action = function () {
                element.isDrillSelectionActivated = false;
                jsObject.ApplyDrillDownToDashboardElement(element, element.getFilters());
            }

            element.buttons.drillDownCancel.action = function () {
                element.isDrillSelectionActivated = false;
                element.updateSelectedGeoms();
                jsObject.UpdateButtonsPanel(element);
            }

            element.buttons.drillUp.action = function () {
                element.isDrillSelectionActivated = false;
                jsObject.ApplyDrillUpToDashboardElement(element);
            }

            element.buttons.drillDown.action = function () {
                element.isDrillSelectionActivated = true;
                jsObject.UpdateButtonsPanel(element);
            }
        }

        // FullScreen element button
        var actionColors = jsObject.copyObject(elementAttributes.actionColors);
        actionColors.foreColor = actionColors.selectedForeColor = actionColors.hotForeColor = actionColors.hotSelectedForeColor = actionColors.isDarkStyle ? "#d6d6d6" : "#787878";

        var buttonFullScreen = this.DbsElementToolButton(null, this.collections.loc.Close, imagesPath + "CloseFullScreen.png",
            [this.collections.loc["FullScreenToolTip"], this.helpLinks["DashboardElementToolbar"], { top: "auto" }], null, null, actionColors);
        buttonFullScreen.jsObject = this;
        buttonFullScreen.elementName = elementAttributes.name;

        buttonFullScreen.action = function () {
            jsObject.reportParams.elementName = jsObject.reportParams.elementName == this.elementName ? null : this.elementName;
            jsObject.postAction("GetPages");
        };

        buttonFullScreen.setFullScreenState = function (state) {
            this.caption.style.display = state ? "" : "none";
            if (jsObject.options.isTouchDevice) buttonFullScreen.style.width = state ? "auto" : "32px";
            this.imageName = state ? this.imageName.replace("Open", "Close") : this.imageName.replace("Close", "Open");
            StiJsViewer.setImageSource(this.image, jsObject.options, jsObject.collections, this.imageName);
        }

        buttonFullScreen.setFullScreenState(this.reportParams.elementName == elementAttributes.name);
        buttonsTable.addCell(buttonFullScreen);
        element.buttons.fullScreen = buttonFullScreen;

        // ViewData button
        var buttonViewData = this.DbsElementToolButton(null, null, imagesPath + "ViewData.png",
            [this.collections.loc["ViewData"], this.helpLinks["DashboardElementToolbar"], { top: "auto" }], null, null, elementAttributes.actionColors);
        buttonsTable.addCell(buttonViewData);
        element.buttons.viewData = buttonViewData;

        buttonViewData.action = function () {
            var viewDataForm = jsObject.InitializeViewDataForm();
            viewDataForm.show(element);
        };

        // Export element menu button
        var buttonExport = this.DbsElementToolButton(null, null, imagesPath + "Save.png",
            [this.collections.loc["Save"], this.helpLinks["DashboardElementToolbar"], { top: "auto" }], null, null, elementAttributes.actionColors);
        buttonExport.jsObject = this;
        buttonExport.elementName = elementAttributes.name;
        buttonExport.elementType = elementAttributes.type;

        buttonExport.action = function () {
            var menuStyle = (elementAttributes.actionColors && elementAttributes.actionColors.isDarkStyle) ? "stiJsViewerDbsDarkMenu" : "stiJsViewerDbsLightMenu";
            var saveMenu = jsObject.InitializeSaveDashboardMenu(menuStyle + "Item", menuStyle, true);
            saveMenu.changeVisibleState(true, this);
        };

        buttonExport.setSelectedBase = buttonExport.setSelected;

        buttonExport.setSelected = function (state) {
            buttonExport.setSelectedBase(state);
            buttonsPanel.checkVisibleState();
            filtersStringPanel.checkVisibleState();
        };

        buttonsTable.addCell(buttonExport);
        element.buttons.export = buttonExport;

        // Events
        jsObject.addEvent(element, "mouseover", function (event) {
            this.isMouseOver = true;
            buttonsPanel.checkVisibleState();
            filtersStringPanel.checkVisibleState();
        });

        jsObject.addEvent(element, "mouseout", function (event) {
            this.isMouseOver = false;
            buttonsPanel.saveVisibleButtons();

            if (!this.buttons.export.isSelected && (!this.buttons.sort || !this.buttons.sort.isSelected) && (!this.buttons.selectColumns || !this.buttons.selectColumns.isSelected)) {
                buttonsPanel.checkVisibleState();
                filtersStringPanel.checkVisibleState();
            }
        });
    }

    this.PaintDashboardElementBorder(element);
    this.PaintDashboardElementShadow(element);
    this.PaintDashboardElementCornerRadius(element);
    this.InsertContentToDashboardElement(element);
    this.UpdateButtonsPanel(element);
    this.UpdateFiltersStringPanel(element);

    if (elementAttributes.contentAttributes.isTimeExpression) {
        jsObject.AddRefreshTimerToDashboardElement(element);
    }

    if (element.buttonsPanel) {
        element.buttonsPanel.updateViewsStates();
    }

    return element;
}

StiJsViewer.prototype.PaintDashboardElementBorder = function (element) {
    element.style.border = 0;
    var border = element.elementAttributes.border;

    if (border) {
        var styles = ["solid", "dashed", "dashed", "dotted", "dotted", "double", "none"];
        var borderStyle = border.size + "px " + styles[border.style] + " " + border.color;
        if (border.left) element.style.borderLeft = borderStyle;
        if (border.top) element.style.borderTop = borderStyle;
        if (border.right) element.style.borderRight = borderStyle;
        if (border.bottom) element.style.borderBottom = borderStyle;
    }
}

StiJsViewer.prototype.PaintDashboardElementShadow = function (element) {
    var shadow = element.elementAttributes.shadow;
    if (shadow && shadow.visible) {
        var shadowLocation = shadow.location.split(";");
        element.style.boxShadow = shadowLocation[0] + "px " + shadowLocation[1] + "px " + shadow.size + "px " + shadow.color;
    }
}

StiJsViewer.prototype.PaintDashboardElementCornerRadius = function (element) {
    var corners = element.elementAttributes.cornerRadius;
    if (corners) {
        element.style.borderRadius = parseInt(corners.topLeft) + "px " + parseInt(corners.topRight) + "px " + parseInt(corners.bottomRight) + "px " + parseInt(corners.bottomLeft) + "px";
    }
}

StiJsViewer.prototype.GetDashboardElementTitleWidth = function (title) {
    var jsObject = this;
    var titleWidth = 0;
    if (title) {
        var measurePanel = document.createElement("div");
        var titleText = StiBase64.decode(title.text);
        var titleFont = title.font;
        measurePanel.style.display = "inline-block";
        this.controls.mainPanel.appendChild(measurePanel);
        measurePanel.innerHTML = titleText;
        measurePanel.style.fontFamily = titleFont.name;
        measurePanel.style.fontSize = jsObject.StrToDouble(titleFont.size) + "pt";
        measurePanel.style.fontWeight = titleFont.bold ? "bold" : "normal";
        measurePanel.style.fontStyle = titleFont.italic ? "italic" : "normal";
        measurePanel.style.textDecoration = "";
        if (titleFont.strikeout) measurePanel.style.textDecoration = "line-through";
        if (titleFont.underline) measurePanel.style.textDecoration += " underline";
        measurePanel.style.whiteSpace = "nowrap";
        measurePanel.style.padding = "3px";
        titleWidth = measurePanel.offsetWidth;
        this.controls.mainPanel.removeChild(measurePanel);
    }
    return titleWidth;
}

StiJsViewer.prototype.PaintDashboardElementTitle = function (element, contentAttrs) {
    var jsObject = this;
    var title = contentAttrs.title;

    if (title && title.visible) {
        var applyFontToPanel = function (panel, font) {
            panel.style.fontFamily = font.name;
            panel.style.fontSize = jsObject.StrToDouble(font.size) + "pt";
            panel.style.fontWeight = font.bold ? "bold" : "normal";
            panel.style.fontStyle = font.italic ? "italic" : "normal";
            panel.style.textDecoration = "";
            if (font.strikeout) panel.style.textDecoration = "line-through";
            if (font.underline) panel.style.textDecoration += " underline";
            panel.style.whiteSpace = "nowrap";
            panel.style.padding = "3px";
        }

        var titleText = StiBase64.decode(title.text);
        var titlePanel = document.createElement("div");
        var titleTable = this.CreateHTMLTable();
        titleTable.style.width = titleTable.style.height = "100%";
        titlePanel.appendChild(titleTable);
        titlePanel.style.position = "absolute";
        titlePanel.style.overflow = "hidden";
        titlePanel.style.left = titlePanel.style.top = titlePanel.style.right = "0px";
        var textCell = titleTable.addTextCell(titleText);
        textCell.style.verticalAlign = "middle"
        titlePanel.style.background = title.backColor;
        titlePanel.style.color = title.foreColor;
        titlePanel.style.textAlign = title.horAlignment.toLowerCase();
        applyFontToPanel(titlePanel, title.font);
        element.appendChild(titlePanel);
        element.titlePanel = titlePanel;

        var measurePanel = document.createElement("div");
        measurePanel.style.display = "inline-block";
        this.controls.mainPanel.appendChild(measurePanel);
        measurePanel.innerHTML = titleText;
        applyFontToPanel(measurePanel, title.font);
        var factorX = measurePanel.offsetWidth > titlePanel.offsetWidth && measurePanel.offsetWidth != 0 ? (titlePanel.offsetWidth - 6) / measurePanel.offsetWidth : 1;
        var factorY = measurePanel.offsetHeight > titlePanel.offsetHeight && measurePanel.offsetHeight != 0 ? (titlePanel.offsetHeight - 6) / measurePanel.offsetHeight : 1;
        var factor = Math.min(Math.abs(factorX), Math.abs(factorY));

        titlePanel.style.height = titlePanel.offsetHeight + "px";
        titlePanel.style.fontSize = parseFloat(title.font.size.toString()) * factor + "pt";

        var corners = element.elementAttributes.cornerRadius;
        if (corners) {
            if (parseInt(corners.topLeft) > 0) {
                textCell.style.paddingLeft = (parseInt(corners.topLeft) / 2) + "px";
            }
            if (parseInt(corners.topRight) > 0) {
                textCell.style.paddingRight = (parseInt(corners.topRight) / 2) + "px";
            }
        }

        this.controls.mainPanel.removeChild(measurePanel);
    }
}

StiJsViewer.prototype.PaintNoResult = function (element) {
    var elementAttrs = element.elementAttributes;

    if (elementAttrs.height > 70 && elementAttrs.width > 70) {
        var showText = elementAttrs.height > 100 && elementAttrs.width > 100;
        var table = this.CreateHTMLTable();

        table.style.display = "inline-block";
        table.style.position = "relative";
        table.style.top = "calc(50% - " + (showText ? 25 : 15) + "px)";

        var img = document.createElement("img");
        StiJsViewer.setImageSource(img, this.options, this.collections, "Dashboards.NoResult.png");
        table.addCell(img);

        if (showText)
            table.addTextCellInNextRow(this.collections.loc["NoResult"]).className = "stiJsViewerNoResultText";

        element.contentPanel.appendChild(table);
    }
}

StiJsViewer.prototype.UpdateButtonsPanel = function (element) {
    var elementAttrs = element.elementAttributes;
    var contentAttrs = elementAttrs ? elementAttrs.contentAttributes : null;

    if (element.buttons.removeFilter) {
        element.buttons.removeFilter.style.display = contentAttrs && contentAttrs.filters && contentAttrs.filters.length > 0 && !element.isDrillSelectionActivated ? "" : "none";
    }

    if (element.buttons.multiFilter) {
        element.buttons.multiFilter.style.display = element.buttons.removeFilter.style.display == "" && !element.isDrillSelectionActivated &&
            (elementAttrs.type == "StiChartElement" || elementAttrs.type == "StiRegionMapElement") ? "" : "none";

        if (this.options.multiFilterStates && this.options.multiFilterStates[elementAttrs.name]) {
            element.buttons.multiFilter.setChecked(true);
        }
    }

    if (element.buttons.fullScreen) {
        element.buttons.fullScreen.style.display = elementAttrs.layout && elementAttrs.layout.fullScreenButton && !element.isDrillSelectionActivated ? "" : "none";
    }

    if (element.buttons.export) {
        element.buttons.export.style.display = elementAttrs.layout && elementAttrs.layout.saveButton && !element.isDrillSelectionActivated ? "" : "none";
    }

    if (element.buttons.viewData) {
        element.buttons.viewData.style.display = elementAttrs.layout && elementAttrs.layout.viewDataButton && !element.isDrillSelectionActivated ? "" : "none";
    }

    if (element.buttons.sort) {
        element.buttons.sort.style.display = contentAttrs && contentAttrs.sortItems && contentAttrs.sortItems.length > 0 ? "" : "none";
    }

    if (contentAttrs.interaction && contentAttrs.interaction.allowUserDrillDown) {
        element.buttons.drillUp.onmouseoutAction();
        element.buttons.drillDown.onmouseoutAction();
        element.buttons.drillDownSelected.onmouseoutAction();
        element.buttons.drillDownCancel.onmouseoutAction();
        element.buttons.drillUp.style.display = !element.isDrillSelectionActivated && contentAttrs.interaction.drillDownCurrentLevel > 0 ? "" : "none";
        element.buttons.drillDown.style.display = !element.isDrillSelectionActivated && contentAttrs.interaction.drillDownCurrentLevel < contentAttrs.interaction.drillDownLevelCount - 1 ? "" : "none";
        element.buttons.drillDownSelected.style.display = element.buttons.drillDownCancel.style.display = element.isDrillSelectionActivated ? "" : "none";
    }
}

StiJsViewer.prototype.UpdateFiltersStringPanel = function (element) {
    var elementAttrs = element.elementAttributes;
    var foreColor = elementAttrs.actionColors.foreColor;
    var contentAttrs = elementAttrs ? elementAttrs.contentAttributes : null;
    var fsPanel = element.filtersStringPanel;

    if (fsPanel) {
        fsPanel.innerHTML = "";
        fsPanel.style.color = foreColor;

        if (contentAttrs && contentAttrs.filtersString) {
            var imagesPath = "Dashboards.Actions." + (elementAttrs.actionColors && elementAttrs.actionColors.isDarkStyle ? "Dark." : "Light.");
            var textCont = document.createElement("div");
            textCont.className = "stiJsViewerFiltersStringText";
            textCont.innerHTML = contentAttrs.filtersString;
            var table = this.CreateHTMLTable();
            table.addCell(textCont);
            var img = document.createElement("img");
            img.style.width = img.style.height = "16px";
            StiJsViewer.setImageSource(img, this.options, this.collections, imagesPath + "Filter.png");
            table.addCell(img).style.paddingLeft = "5px";
            fsPanel.appendChild(table);
            var textWidth = elementAttrs.width - 40;
            var title = contentAttrs.title;

            if (title) {
                if (title.visible && title.text) {
                    var titleWidth = this.GetDashboardElementTitleWidth(title);
                    if (title.horAlignment == "Left") {
                        textWidth = elementAttrs.width - titleWidth - 40;
                        fsPanel.style.right = "5px";
                        fsPanel.style.left = "auto";
                    }
                    else if (title.horAlignment == "Center") {
                        textWidth = (elementAttrs.width - titleWidth) / 2 - 40;
                        fsPanel.style.right = "5px";
                        fsPanel.style.left = "auto";
                    }
                    else if (title.horAlignment == "Right") {
                        textWidth = elementAttrs.width - titleWidth - 40;
                        fsPanel.style.right = "auto";
                        fsPanel.style.left = "5px";
                    }
                }
                else {
                    textWidth = 0;
                    fsPanel.style.display = "none";
                }
            }

            textCont.style.maxWidth = (textWidth > 70 ? textWidth : 0) + "px";
        }

        fsPanel.checkVisibleState();
    }
}

StiJsViewer.prototype.InsertContentToDashboardElement = function (element) {
    var jsObject = this;
    var elementType = element.elementAttributes.type;
    var elementAttrs = element.elementAttributes;
    var contentAttrs = elementAttrs.contentAttributes;

    var oldScrollTop = element.itemsPanel ? element.itemsPanel.scrollTop : 0;
    if (elementType == "StiTableElement" && element.dataGrid) element.dataGrid.saveGridStates();

    //clear old content
    while (element.childNodes[0]) {
        element.removeChild(element.childNodes[0]);
    }

    //svg content
    if (contentAttrs.svgContent && !contentAttrs.svgContentIsVertScrollable && !contentAttrs.svgContentIsHorScrollable) {
        if (elementType == "StiImageElement" && contentAttrs.svgContent.indexOf("gifContent:") == 0) {
            this.InsertGifContentToDashboardElement(element, contentAttrs.svgContent.replace("gifContent:", ""));
        }
        else {
            element.innerHTML = contentAttrs.svgContent;

            if (elementType == "StiTextElement" || elementType == "StiImageElement") {
                jsObject.AddInteractionsToDashboardElement(element);
            }
            else if (elementType == "StiIndicatorElement") {
                jsObject.AddInteractionsToIndicatorElement(element, element.childNodes[0]);
            }
        }
    }
    //interactive controls content
    else if (elementType == "StiChartElement" || elementType == "StiRegionMapElement") {
        this.InsertHTMLContentToDashboardElement(element, contentAttrs.htmlContent, function (frameWindow) {
            jsObject.AddInteractionsToHtmlContentDashboardElement(element, frameWindow);
        });
    }
    else if (elementType == "StiOnlineMapElement") {
        this.InsertHTMLContentToDashboardElement(element, StiBase64.decode(contentAttrs.htmlContent), function () { });
    }
    else {
        var contentPanel = document.createElement("div");
        contentPanel.style.position = "absolute";
        contentPanel.style.overflow = "auto";
        contentPanel.style.left = contentPanel.style.top = contentPanel.style.right = contentPanel.style.bottom = "0px";
        element.contentPanel = contentPanel;
        element.appendChild(contentPanel);

        jsObject.PaintDashboardElementTitle(element, contentAttrs);

        if (elementAttrs.padding) {
            var padding = elementAttrs.padding.split(",");

            if (elementType == "StiTableElement" || elementType == "StiPivotTableElement" || elementType == "StiListBoxElement" || elementType == "StiTreeViewElement") {
                padding = jsObject.CorrectPaddingsByCornerRadius(element, padding);
            }

            contentPanel.style.top = (element.titlePanel ? (element.titlePanel.offsetHeight + parseInt(padding[0])) : padding[0]) + "px";
            contentPanel.style.right = padding[1] + "px";
            contentPanel.style.bottom = padding[2] + "px";
            contentPanel.style.left = padding[3] + "px";
        }

        if (!contentAttrs.svgContent && (elementType == "StiIndicatorElement" || elementType == "StiProgressElement" || elementType == "StiGaugeElement")) {
            jsObject.PaintNoResult(element);
        }

        if (contentAttrs.svgContent && (contentAttrs.svgContentIsVertScrollable || contentAttrs.svgContentIsHorScrollable)) {
            contentPanel.innerHTML = contentAttrs.svgContent;
            contentPanel.className = "stiJsViewerScrollContainer";

            if (contentAttrs.svgContentIsVertScrollable && !contentAttrs.svgContentIsHorScrollable) {
                contentPanel.style.overflowX = "hidden";
            }
            if (contentAttrs.svgContentIsHorScrollable && !contentAttrs.svgContentIsVertScrollable) {
                contentPanel.style.overflowY = "hidden";
            }
            if (elementType == "StiIndicatorElement") {
                jsObject.AddInteractionsToIndicatorElement(element, contentPanel.childNodes[0]);
            }
        }
        else if (elementType == "StiListBoxElement") {
            this.CreateListBoxElementContent(element);
        }
        else if (elementType == "StiComboBoxElement") {
            this.CreateComboBoxElementContent(element);
        }
        else if (elementType == "StiDatePickerElement") {
            this.CreateDatePickerElementContent(element);
        }
        else if (elementType == "StiTreeViewElement") {
            this.CreateTreeViewElementContent(element);
        }
        else if (elementType == "StiTreeViewBoxElement") {
            this.CreateTreeViewBoxElementContent(element);
        }
        else if (elementType == "StiTableElement") {
            this.CreateTableElementContent(element);
        }
        else if (elementType == "StiPivotTableElement") {
            this.CreatePivotTableElementContent(element);
        }
        else if (elementType == "StiButtonElement") {
            this.CreateButtonElementContent(element);
        }
        else if (elementType == "StiPanelElement" && contentAttrs.dashboardWatermark) {
            jsObject.AddWatermarkToPanel(element, contentAttrs.dashboardWatermark);
        }

        if (element.itemsPanel) element.itemsPanel.scrollTop = oldScrollTop;
    }

    contentAttrs.svgContent = null;
    contentAttrs.htmlContent = null;

    //return buttonsPanel and filtersStringPanel to the place
    if (element.filtersStringPanel) element.appendChild(element.filtersStringPanel);
    if (element.buttonsPanel) element.appendChild(element.buttonsPanel);
}

StiJsViewer.prototype.DbsElementToolButton = function (name, captionText, imageName, toolTip, arrow, styleName, styleColors) {
    var button = this.SmallButton(name, captionText, imageName, toolTip, arrow, styleName, styleColors);
    var toolTip = this.controls.toolTip;

    button.onmouseoverAction_ = button.onmouseoverAction;

    button.onmouseoverAction = function () {
        this.onmouseoverAction_();
        toolTip.ownerButton = this;
    }

    button.onmouseoutAction_ = button.onmouseoutAction;

    button.onmouseoutAction = function () {
        toolTip.ownerButton = null;
        this.onmouseoutAction_();
    }

    return button;
}

StiJsViewer.prototype.ApplyFiltersToDashboardElement = function (element, filters, noDelay) {
    this.filterGuid = this.newGuid();

    if (this.isFilterElement(element.elementAttributes.type)) {
        for (var i = 0; i < filters.length; i++) {
            if (filters[i].value == "" || filters[i].value == null) {
                if (filters[i].condition == "EqualTo") {
                    filters[i].condition = element.elementAttributes.contentAttributes.isStringColumnType ? "IsBlankOrNull" : "IsNull";
                }
                else if (filters[i].condition == "NotEqualTo") {
                    filters[i].condition = element.elementAttributes.contentAttributes.isStringColumnType ? "IsNotBlank" : "IsNotNull";
                }
            }
        }
    }

    this.postInteraction({
        action: "DashboardFiltering",
        dashboardFilteringParameters: {
            elementName: element.elementAttributes.name,
            elementGroup: element.elementAttributes.group,
            filters: filters,
            filterGuid: this.filterGuid
        }
    });

    this.controls.processImage.hide();
    clearTimeout(this.dashboardProcessTimeout);

    var jsObject = this;

    jsObject.dashboardProcessTimeout = setTimeout(function () {
        jsObject.controls.processImage.show();
    }, noDelay ? 0 : 800);
}

StiJsViewer.prototype.ChangeTableElementSelectColumns = function (element) {
    var jsObject = this;
    var elementName = element.elementAttributes.name;
    jsObject.selectColumnsInProgress = true;

    jsObject.postAjax(jsObject.getActionRequestUrl(jsObject.options.requestUrl, jsObject.options.actions.viewerEvent),
        {
            action: "ChangeTableElementSelectColumns",
            tableElementName: elementName,
            tableElementHiddenColumns: jsObject.tableElementHiddenColumns
        },
        function (answer) {
            if (answer) {
                jsObject.controls.processImage.hide();
                var element = jsObject.controls.reportPanel.getDashboardElementByName(elementName);
                var elementAttributes = JSON.parse(jsObject.options.server.useCompression ? StiGZipHelper.unpack(answer) : answer);
                if (element && elementAttributes) {
                    element.elementAttributes = elementAttributes;
                    jsObject.UpdateButtonsPanel(element);
                    jsObject.UpdateFiltersStringPanel(element);
                    jsObject.InsertContentToDashboardElement(element);
                    jsObject.selectColumnsInProgress = false;
                    if (jsObject.waitToSelectColumns) {
                        jsObject.ChangeTableElementSelectColumns(element);
                        jsObject.waitToSelectColumns = false;
                    }
                }
            }
        });
}

StiJsViewer.prototype.ChangeChartElementViewState = function (element, viewStateKey) {
    var jsObject = this;
    var elementName = element.elementAttributes.name;
    jsObject.controls.processImage.show();

    jsObject.postAjax(jsObject.getActionRequestUrl(jsObject.options.requestUrl, jsObject.options.actions.viewerEvent),
        {
            action: "ChangeChartElementViewState",
            chartElementName: elementName,
            chartElementViewStateKey: viewStateKey
        },
        function (answer) {
            if (answer) {
                jsObject.controls.processImage.hide();
                var element = jsObject.controls.reportPanel.getDashboardElementByName(elementName);
                var elementAttributes = JSON.parse(jsObject.options.server.useCompression ? StiGZipHelper.unpack(answer) : answer);
                if (element && elementAttributes) {
                    element.elementAttributes = elementAttributes;
                    jsObject.UpdateButtonsPanel(element);
                    jsObject.UpdateFiltersStringPanel(element);
                    jsObject.InsertContentToDashboardElement(element);
                    if (element.viewStateButtons) {
                        for (var i = 0; i < element.viewStateButtons.length; i++) {
                            element.viewStateButtons[i].onmouseoutAction();
                        }
                    }
                }
            }
        });
}

StiJsViewer.prototype.ApplySortsToDashboardElement = function (element, sorts, noDelay) {
    this.postInteraction({
        action: "DashboardSorting",
        dashboardSortingParameters: {
            elementName: element.elementAttributes.name,
            sorts: sorts
        }
    });

    this.controls.processImage.hide();
    clearTimeout(this.dashboardProcessTimeout);

    var jsObject = this;

    jsObject.dashboardProcessTimeout = setTimeout(function () {
        jsObject.controls.processImage.show();
    }, noDelay ? 0 : 800);
}

StiJsViewer.prototype.ApplyDrillUpToDashboardElement = function (element) {
    this.postInteraction({
        action: "DashboardElementDrillUp",
        dashboardElementDrillDownParameters: {
            elementName: element.elementAttributes.name
        }
    });

    this.controls.processImage.hide();
    clearTimeout(this.dashboardProcessTimeout);

    var jsObject = this;

    jsObject.dashboardProcessTimeout = setTimeout(function () {
        jsObject.controls.processImage.show();
    }, 800);
}

StiJsViewer.prototype.ApplyDrillDownToDashboardElement = function (element, filters) {
    this.postInteraction({
        action: "DashboardElementDrillDown",
        dashboardElementDrillDownParameters: {
            elementName: element.elementAttributes.name,
            filters: filters
        }
    });

    this.controls.processImage.hide();
    clearTimeout(this.dashboardProcessTimeout);

    var jsObject = this;

    jsObject.dashboardProcessTimeout = setTimeout(function () {
        jsObject.controls.processImage.show();
    }, 800);
}

StiJsViewer.prototype.SetFilterSortGuid = function () {
    this.filterSortGuid = this.newGuid();

    return this.filterSortGuid;
}

StiJsViewer.prototype.CheckFilterSortGuid = function (guid) {
    var newGuid = this.newGuid();

    if (!this.filterSortGuids) this.filterSortGuids = [];
    this.filterSortGuids.push(newGuid);

    return newGuid;
}

StiJsViewer.prototype.InsertHTMLContentToDashboardElement = function (element, htmlContent, completeLoadFunction) {
    var jsObject = this;
    var elementType = element.elementAttributes.type;
    var elementAttrs = element.elementAttributes;
    var top = 0;
    var left = 0;
    var right = 0;
    var bottom = 0;
    var width = parseInt(element.style.width.replace("px", ""));
    var height = parseInt(element.style.height.replace("px", ""));

    if (elementType == "StiOnlineMapElement" && elementAttrs.padding) {
        var padding = elementAttrs.padding.split(",");
        top = parseInt(padding[0]);
        right = parseInt(padding[1]);
        bottom = parseInt(padding[2]);
        left = parseInt(padding[3]);
        width = width - left - right;
        height = height - bottom - top;
    }

    if (htmlContent) {
        var frame = element.frame || document.createElement("iframe");
        frame.setAttribute("scrolling", "no");
        frame.setAttribute("style", "position: absolute; overflow: hidden; border: none; left:" + left + "px;top:" + top + "px;right:" + right + "px;bottom:" + bottom + "px;width:" + width + "px;height:" + height + "px;");

        element.appendChild(frame);
        element.frame = frame;

        var corners = elementAttrs.cornerRadius;
        if (elementType == "StiOnlineMapElement" && corners) {
            frame.style.borderRadius = parseInt(corners.topLeft) + "px " + parseInt(corners.topRight) + "px " + parseInt(corners.bottomRight) + "px " + parseInt(corners.bottomLeft) + "px";
        }

        var frameDoc = frame.contentWindow.document;
        if (frameDoc) {
            if (completeLoadFunction) {
                frame.onload = function () {
                    completeLoadFunction(this.contentWindow);
                }
            }

            frameDoc.open();
            frameDoc.write(htmlContent);
            frameDoc.close();

            //correct html content            
            var domElements = frameDoc.getElementsByClassName("StiPageContainer");
            if (domElements && domElements.length > 0) {
                var pageContainer = domElements[0];
                pageContainer.style.position = "absolute";
                pageContainer.style.top = "0px";

                for (var i = 0; i < pageContainer.childNodes.length; i++) {
                    var child = pageContainer.childNodes[i];
                    if (child.className && child.tagName && child.tagName.toLowerCase() == "div") {
                        child.style.width = (width - 2) + "px";
                        child.style.boxSizing = "border-box";
                    }
                    child = null;
                }
                pageContainer = null;
            }

            frameDoc.onmousedown = function () {
                jsObject.controls.viewer.pressedDown();
                if (jsObject.options.jsDesigner) {
                    jsObject.options.jsDesigner.options.mobileDesigner.pressedDown();
                }
            }

            domElements = null;
            frameDoc = null;
        }
    }
}

StiJsViewer.prototype.InsertGifContentToDashboardElement = function (element, imageSrc) {
    var image = new window.Image();
    var img = document.createElement("img");
    img.style.position = "absolute";
    element.appendChild(img);

    image.onload = function () {
        var contentAttrs = element.elementAttributes.contentAttributes;
        var widthEl = parseInt(element.style.width.replace("px", ""));
        var heightEl = parseInt(element.style.height.replace("px", ""));
        var aspectRatio = contentAttrs.aspectRatio;
        var horAlignment = contentAttrs.horAlignment;
        var vertAlignment = contentAttrs.vertAlignment;

        img.style.left = img.style.top = "0";

        if (!aspectRatio) {
            img.style.width = img.style.height = "100%";
        }
        else if (image.width > 0 && image.height > 0) {
            var wFactor = widthEl / image.width;
            var hFactor = heightEl / image.height;

            if (Math.abs(wFactor) < Math.abs(hFactor)) {
                img.style.width = "100%";
                img.style.height = "auto";

                if (vertAlignment == "Center") {
                    img.style.top = (heightEl / 2 - image.height * wFactor / 2) + "px";
                }
                else if (vertAlignment == "Bottom") {
                    img.style.bottom = "0";
                    img.style.top = "";
                }
            }
            else {
                img.style.height = "100%";
                img.style.width = "auto";

                if (horAlignment == "Center") {
                    img.style.left = (widthEl / 2 - image.width * hFactor / 2) + "px";
                }
                else if (horAlignment == "Right") {
                    img.style.right = "0";
                    img.style.left = "";
                }
            }
        }

        img.src = imageSrc;
        image = null;
    }

    image.src = imageSrc;
}

StiJsViewer.prototype.AddWatermarkToPanel = function (panel, wProps) {
    if (!panel.wPanel) {
        var pWidth = parseInt(panel.style.width);
        var pHeight = parseInt(panel.style.height);

        var wPanel = this.CreateSvgElement("svg");
        wPanel.setAttribute("style", "position:absolute; left:0; top:0;");
        wPanel.setAttribute("width", pWidth);
        wPanel.setAttribute("height", pHeight);

        panel.appendChild(wPanel);
        panel.wPanel = wPanel;

        this.PaintImageWaterMark(panel, pWidth, pHeight, wProps);
        this.PaintTextWaterMark(panel, pWidth, pHeight, wProps);
        this.PaintWeaveWaterMark(panel, pWidth, pHeight, wProps);
    }
}

StiJsViewer.prototype.PaintTextWaterMark = function (panel, pWidth, pHeight, wProps) {
    if (wProps.textEnabled && wProps.text) {
        var textWaterMark = StiBase64.decode(wProps.text);

        var waterMarkParent = this.CreateSvgElement("g");
        panel.wPanel.appendChild(waterMarkParent);

        var waterMarkChild = this.CreateSvgElement("g");
        waterMarkParent.appendChild(waterMarkChild);
        waterMarkChild.setAttribute("transform", "rotate(-" + wProps.textAngle + ")");

        var textEl = this.CreateSvgElement("text");
        waterMarkChild.appendChild(textEl);

        textEl.textContent = textWaterMark;
        textEl.style.fontFamily = wProps.textFont.name;
        textEl.style.fontSize = wProps.textFont.size + "pt";
        textEl.style.fontWeight = wProps.textFont.bold ? "bold" : "";
        textEl.style.fontStyle = wProps.textFont.italic ? "italic" : "";
        textEl.style.textDecoration = "";
        if (wProps.textFont.strikeout) textEl.style.textDecoration = "line-through";
        if (wProps.textFont.underline) textEl.style.textDecoration += " underline";

        textEl.style.textAnchor = "middle";
        textEl.style.fill = wProps.textColor;

        waterMarkParent.setAttribute("transform", "translate(" + (pWidth / 2) + ", " + (pHeight / 2) + ")");
    }
}

StiJsViewer.prototype.PaintImageWaterMark = function (panel, pWidth, pHeight, wProps) {
    if (wProps.imageEnabled && wProps.image) {
        var imageEl = this.CreateSvgElement("image");
        panel.wPanel.appendChild(imageEl);

        var isWmfImage = wProps.image.indexOf("data:image/x-wmf") >= 0;
        var multipleFactor = this.StrToDouble(wProps.imageMultipleFactor);
        var stretch = wProps.imageStretch || isWmfImage;
        var aspectRatio = wProps.imageAspectRatio && !isWmfImage;
        var sizeWatermark = wProps.imageSize.split(";");
        var imageTransparency = parseInt(wProps.imageTransparency);
        var imageTiling = wProps.imageTiling;

        var widthWatermark = sizeWatermark[0] * multipleFactor;
        var heightWatermark = sizeWatermark[1] * multipleFactor;

        var newWidth = stretch ? pWidth : widthWatermark;
        var newHeight = stretch ? pHeight : heightWatermark;

        if (imageTiling) {
            imageEl.style.display = "none";
            panel.wPanel.style.backgroundImage = "url(" + wProps.image + ")";
            panel.wPanel.style.backgroundRepeat = "repeat";
            panel.wPanel.style.backgroundSize = parseInt((widthWatermark / pWidth) * 100) + "%";
        }
        else {
            imageEl.href.baseVal = wProps.image;

            if (!isNaN(imageTransparency)) {
                imageEl.style.opacity = Math.abs((imageTransparency - 255) / 255);
            }

            if (stretch && aspectRatio && widthWatermark > 0 && heightWatermark > 0) {
                var wFactor = pWidth / widthWatermark;
                var hFactor = pHeight / heightWatermark;
                if (Math.abs(wFactor) > Math.abs(hFactor)) {
                    newWidth = pWidth;
                    newHeight = heightWatermark * wFactor;
                }
                else {
                    newHeight = pHeight;
                    newWidth = widthWatermark * hFactor;
                }
            }

            var x = 0;
            var y = 0;
            var imageAlignment = wProps.imageAlignment;

            if (imageAlignment.indexOf("Center") >= 0) x = pWidth / 2 - newWidth / 2;
            else if (imageAlignment.indexOf("Right") >= 0) x = pWidth - newWidth;

            if (imageAlignment.indexOf("Middle") >= 0) y = pHeight / 2 - newHeight / 2;
            else if (imageAlignment.indexOf("Bottom") >= 0) y = pHeight - newHeight;

            imageEl.setAttribute("x", stretch && !aspectRatio ? 0 : x);
            imageEl.setAttribute("y", stretch && !aspectRatio ? 0 : y);
            imageEl.setAttribute("width", newWidth);
            imageEl.setAttribute("height", newHeight);

            if (stretch && !aspectRatio)
                imageEl.setAttribute("preserveAspectRatio", "none");
            else
                imageEl.removeAttribute("preserveAspectRatio");
        }
    }
}

StiJsViewer.prototype.PaintWeaveWaterMark = function (panel, pWidth, pHeight, wProps) {
    if (wProps.weaveEnabled && (wProps.weaveMajorImage || wProps.weaveMinorImage)) {
        var dist = parseInt(wProps.weaveDistance);
        var angle = parseInt(wProps.weaveAngle);
        var centralX = pWidth / 2;
        var centralY = pHeight / 2;

        var posX = centralX;
        var posY = centralY;

        for (var step = 0; step < 30; step++) {
            var forwardRad = (angle + 90) * (Math.PI / 180);
            var x = posX + dist * step * Math.cos(forwardRad);
            var y = posY + dist * step * Math.sin(forwardRad);

            if (!this.DrawWeaveLine(panel, pWidth, pHeight, wProps, dist, angle, x, y, step)) break;
        }

        posX = centralX;
        posY = centralY;

        for (var step = 1; step < 30; step++) {
            var backwardRad = (angle - 90) * (Math.PI / 180);
            var x = posX + dist * step * Math.cos(backwardRad);
            var y = posY + dist * step * Math.sin(backwardRad);

            if (!this.DrawWeaveLine(panel, pWidth, pHeight, wProps, dist, angle, x, y, -step)) break;
        }
    }
}

StiJsViewer.prototype.DrawWeaveLine = function (panel, pWidth, pHeight, wProps, dist, angle, posX, posY, shift) {
    var isAny = false;
    var isOnce = false;
    var rad = angle * (Math.PI / 180);
    var imageMajor = wProps.weaveMajorImage;
    var imageMinor = wProps.weaveMinorImage;

    for (var step = 0; step < 30; step++) {
        var x = posX + dist * step * Math.cos(rad);
        var y = posY + dist * step * Math.sin(rad);
        var image = ((step + shift) & 1) == 0 ? imageMajor : imageMinor;

        if (image == null) continue;

        if (this.ContainsWeaveImage(pWidth, pHeight, image, angle, x, y)) {
            this.DrawWeaveImage(panel, image, angle, x, y);

            isAny = true;
            isOnce = true;
        }
        else {
            if (isOnce) break;
        }
    }

    for (var step = 1; step < 30; step++) {
        var x = posX - dist * step * Math.cos(rad);
        var y = posY - dist * step * Math.sin(rad);
        var image = ((-step + shift) & 1) == 0 ? imageMajor : imageMinor;

        if (image == null) continue;

        if (this.ContainsWeaveImage(pWidth, pHeight, image, angle, x, y)) {
            this.DrawWeaveImage(panel, image, angle, x, y);

            isAny = true;
            isOnce = true;
        }
        else {
            if (isOnce) break;
        }
    }

    return isAny;
}

StiJsViewer.prototype.ContainsWeaveImage = function (pWidth, pHeight, image, angle, x, y) {
    if (image == null)
        return false;

    var width = image.width / 2;
    var height = image.height / 2;
    var rad = angle * (Math.PI / 180);

    var p1 = {
        x: (-width) * Math.cos(rad) + (-height) * Math.sin(rad) + x,
        y: (-width) * Math.sin(rad) - (-height) * Math.cos(rad) + y
    };

    var p2 = {
        x: width * Math.cos(rad) + (-height) * Math.sin(rad) + x,
        y: width * Math.sin(rad) - (-height) * Math.cos(rad) + y
    };

    var p3 = {
        x: (-width) * Math.cos(rad) + height * Math.sin(rad) + x,
        y: (-width) * Math.sin(rad) - height * Math.cos(rad) + y
    }

    var p4 = {
        x: width * Math.cos(rad) + height * Math.sin(rad) + x,
        y: width * Math.sin(rad) - height * Math.cos(rad) + y
    }

    return this.ContainsPointAtPanelRect(pWidth, pHeight, p1) || this.ContainsPointAtPanelRect(pWidth, pHeight, p2) || this.ContainsPointAtPanelRect(pWidth, pHeight, p3) || this.ContainsPointAtPanelRect(pWidth, pHeight, p4);
}

StiJsViewer.prototype.ContainsPointAtPanelRect = function (pWidth, pHeight, point) {
    return (point.x >= 0 && point.x <= pWidth && point.y >= 0 && point.y <= pHeight)
}

StiJsViewer.prototype.DrawWeaveImage = function (panel, image, angle, x, y) {
    var g1 = this.CreateSvgElement("g");
    g1.setAttribute("transform", "translate(" + x + "," + y + ") rotate(" + angle + ")");

    var svg = this.CreateSvgElement("svg");
    svg.setAttribute("x", -image.width / 2);
    svg.setAttribute("y", -image.height / 2);
    svg.setAttribute("width", image.width);
    svg.setAttribute("height", image.height);
    g1.appendChild(svg);

    var rect = this.CreateSvgElement("rect");
    rect.setAttribute("x", "0");
    rect.setAttribute("y", "0");
    rect.setAttribute("width", image.width);
    rect.setAttribute("height", image.height);
    rect.setAttribute("fill", "#ffffff");
    rect.setAttribute("fill-opacity", "0");
    svg.appendChild(rect);

    var g2 = this.CreateSvgElement("g");
    svg.appendChild(g2);

    var text = this.CreateSvgElement("text");
    text.textContent = image.text;
    text.setAttribute("x", "45%");
    text.setAttribute("dy", "1em");
    text.setAttribute("text-anchor", "middle");
    text.setAttribute("font-family", "Stimulsoft");
    text.setAttribute("font-size", image.size * 3.5);
    text.style.fill = image.color;
    g2.appendChild(text);

    panel.wPanel.appendChild(g1);
}

StiJsViewer.prototype.CreateSvgElement = function (tagName) {
    return ("createElementNS" in document ? document.createElementNS("http://www.w3.org/2000/svg", tagName) : document.createElement(tagName));
}