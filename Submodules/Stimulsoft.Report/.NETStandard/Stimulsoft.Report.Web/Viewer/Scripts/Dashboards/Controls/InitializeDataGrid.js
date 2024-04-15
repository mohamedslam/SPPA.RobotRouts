
StiJsViewer.prototype.DataGrid = function (width, height, settings, elementAttrs, element, isViewData) {
    var grid = document.createElement("div");
    var jsObject = grid.jsObject = this;
    grid.style.userSelect = "none";
    grid.style.position = "relative";

    if (jsObject.options.isTouchDevice) {
        grid.style.overflowX = "auto";
        grid.style.overflowY = "hidden";
        grid.className = "stiJsViewerContainerHideScroll";

        grid.onscroll = function () {
            grid.repaintHorScrolls();
        }
    }
    else {
        grid.style.overflow = "hidden";
    }

    if (width) grid.style.width = width + "px";
    if (height) grid.style.height = height + "px";

    grid.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
    }

    grid.showProgress = function () {
        grid.hideProgress();

        var panel = document.createElement("div");
        panel.style.position = "absolute";
        panel.style.zIndex = "1";
        panel.style.top = panel.style.left = panel.style.right = panel.style.bottom = "0px";
        panel.style.background = elementAttrs.backColor || "transparent";
        jsObject.AddProgressToControl(panel);
        grid.appendChild(panel);
        grid.progressPanel = panel;

        if (elementAttrs.actionColors && elementAttrs.actionColors.isDarkStyle)
            panel.progress.setToLightStyle();
        else
            panel.progress.setToDefaultStyle();

        panel.progress.show();
    }

    grid.hideProgress = function () {
        if (grid.progressPanel) {
            grid.removeChild(grid.progressPanel);
        }
        grid.progressPanel = null;
    }

    grid.createCssStyle = function () {
        var style = document.createElement('style');
        style.type = 'text/css';
        this.appendChild(style);

        return style;
    }

    grid.addRulesToStyle = function (style, name, rules) {
        var sheet = style.styleSheet || style.sheet || document.styleSheets[0];
        if (sheet && sheet.insertRule) {
            sheet.insertRule(name + "{" + rules + "}", 0);
        }
    }

    grid.hideRow = function (row) {
        for (var i = 0; i < row.childNodes.length; i++) {
            row.childNodes[i].style.display = "none";
        }
        row.isHidden = true;
    }

    grid.displayRow = function (row) {
        for (var i = 0; i < row.childNodes.length; i++) {
            row.childNodes[i].style.display = "";
        }
        row.style.height = "auto";
        row.isHidden = false;
    }

    grid.changeColumnsOrder = function (data) {
        var columnsOrder = jsObject.options.tablesColumnsOrder ? jsObject.options.tablesColumnsOrder[elementAttrs.name] : null;

        if (!columnsOrder || columnsOrder.length != data[0].length) {
            return data;
        }

        var newData = [];

        for (var i = 0; i < columnsOrder.length; i++) {
            var colIndex = columnsOrder[i];
            for (var i_row = 0; i_row < data.length; i_row++) {
                if (!newData[i_row]) newData[i_row] = [];
                newData[i_row][i] = data[i_row][colIndex];
            }
        }

        return newData;
    }

    grid.showData = function (data, hiddenData) {
        grid.hideProgress();
        grid.clear();
        grid.data = data;
        grid.hiddenData = hiddenData;
        grid.hideRows = false;
        grid.menuItems = {};

        if (!data || data.length == 0) return;

        grid.selectedCells = null;
        grid.showProgress();
        data = grid.changeColumnsOrder(data);

        grid.headerButtons = [];
        var interaction = elementAttrs.contentAttributes.interaction;
        var allowInteractions = isViewData || (interaction && (interaction.allowUserSorting || interaction.allowUserFiltering));
        var sortsCount = 0;
        var showFooter = false;
        var countColumns = data[0].length;

        for (var i = 0; i < countColumns; i++) {
            if (data[0][i].sortLabel) sortsCount++;
            if (data[0][i].showTotalSummary) showFooter = true;
        }

        var headerTable = jsObject.CreateHTMLTable();
        var headerTableRow = headerTable.tr[0];
        grid.appendChild(headerTable);

        var cellsContainer = document.createElement("div");
        cellsContainer.style.position = "relative";

        if (jsObject.options.isTouchDevice) {
            cellsContainer.style.overflowX = "hidden";
            cellsContainer.style.overflowY = "auto";
            cellsContainer.className = "stiJsViewerContainerHideScroll";
        }
        else {
            cellsContainer.style.overflow = "hidden";
        }

        grid.appendChild(cellsContainer);

        var dataTable = jsObject.CreateHTMLTable();
        var dataTableRow = dataTable.tr[0];
        cellsContainer.appendChild(dataTable);

        var footerTable = jsObject.CreateHTMLTable();
        var footerTableRow = footerTable.tr[0];
        if (showFooter && data.length > 2) grid.appendChild(footerTable);

        //Add header
        for (var i = 0; i < countColumns; i++) {
            var columnObj = data[0][i];
            var headerButton = jsObject.GridHeaderButton(grid, columnObj, settings, allowInteractions, columnObj.type == "StiSparklinesColumn", isViewData);
            grid.headerButtons.push(headerButton);            
            headerButton.style.minHeight = settings.headerHeight + "px";
            headerButton.style.background = settings.headerBackground;
            headerButton.style.fontFamily = isViewData ? "Arial" : (settings.headerFont.name || "Arial");
            headerButton.style.fontSize = isViewData ? "10pt" : ((settings.headerFont.size || "10") + "pt");
            headerButton.style.fontWeight = settings.headerFont.bold && !isViewData ? "bold" : "normal";
            headerButton.style.fontStyle = settings.headerFont.italic && !isViewData ? "italic" : "normal";
            headerButton.style.color = settings.headerForeColor;
            headerButton.style.textDecoration = "";
            if (settings.headerFont.strikeout) headerButton.style.textDecoration = "line-through";
            if (settings.headerFont.underline) headerButton.style.textDecoration += " underline";

            var headerCell = headerTable.addCell(headerButton);
            headerCell.style.padding = "0";

            if (i < countColumns - 1) {
                headerCell.style.borderRight = "1px solid " + settings.tableBorderColor;
            }

            headerButton.getPosIndex = function () {
                var headerCells = headerTable.tr[0].childNodes;
                for (var i = 0; i < headerCells.length; i++) {
                    if (headerCells[i].firstChild == this)
                        return i;
                }
            }

            if (allowInteractions && columnObj.type != "StiSparklinesColumn") {

                headerButton.action = function () {
                    grid.action(this);
                }

                headerButton.onclick = null;

                headerButton.onmouseup = function () {
                    if (jsObject.gridHeaderInDrag && jsObject.gridHeaderInDrag.moveInProgress) {
                        var fromIndex = jsObject.gridHeaderInDrag.posIndex;
                        var toIndex = this.getPosIndex();

                        var columnsOrder = jsObject.options.tablesColumnsOrder ? jsObject.options.tablesColumnsOrder[elementAttrs.name] : null;

                        if (!columnsOrder) {
                            columnsOrder = [];
                            for (var i = 0; i < countColumns; i++) {
                                columnsOrder[i] = i;
                            }
                        }

                        if (fromIndex < columnsOrder.length && toIndex < columnsOrder.length) {
                            var element = columnsOrder[fromIndex];
                            columnsOrder.splice(fromIndex, 1);
                            columnsOrder.splice(toIndex, 0, element);
                        }

                        jsObject.options.tablesColumnsOrder[elementAttrs.name] = columnsOrder;

                        grid.showData(grid.data);
                    }
                    else {
                        if (!this.ignoreMainAction) this.action();
                        this.ignoreMainAction = false;
                    }
                }

                headerButton.sortCell.onmouseup = function () {
                    if (jsObject.gridHeaderInDrag && jsObject.gridHeaderInDrag.moveInProgress)
                        return;

                    this.button.ignoreMainAction = true;
                    var sortDirection = this.button.sortDirection == "Ascending" ? "Descending" : "Ascending";
                    var columnKey = this.button.columnObject.key;
                    var columnObject = this.button.columnObject;

                    var getThisColumnSort = function () {
                        var contentAttrs = elementAttrs.contentAttributes;
                        for (var i = 0; i < contentAttrs.sorts.length; i++) {
                            if (contentAttrs.sorts[i].key == columnKey) {
                                return contentAttrs.sorts[i];
                            }
                        }
                        return null;
                    }

                    if (isViewData) {
                        grid.showProgress();
                        jsObject.postAjax(jsObject.getActionRequestUrl(jsObject.options.requestUrl, jsObject.options.actions.viewerEvent),
                            {
                                action: "DashboardViewData",
                                dashboardFilteringParameters: {
                                    elementName: elementAttrs.name,
                                    columnIndex: columnObject.columnIndex,
                                    sortDirection: sortDirection
                                }
                            },
                            function (answer) {
                                var answerObject = JSON.parse(answer);
                                grid.showData(answerObject.data);
                            });
                    }
                    else {
                        var currentSort = getThisColumnSort();
                        if (currentSort)
                            currentSort.direction = sortDirection;
                        else
                            elementAttrs.contentAttributes.sorts.push(jsObject.DataSortObject(columnKey, sortDirection));

                        jsObject.ApplySortsToDashboardElement(element, elementAttrs.contentAttributes.sorts, true);
                    }
                };

                headerButton.sortCell.onmouseover = function () {
                    this.isOver = true;
                    this.button.sortImage.style.opacity = "1";
                };

                headerButton.sortCell.onmouseout = function () {
                    this.isOver = false;
                    this.button.sortImage.style.opacity = this.button.sortDirection == "Ascending" || this.button.sortDirection == "Descending" ? "1" : "0.7";
                };
            }

            if (grid.selectedHeaderButton && grid.selectedHeaderButton.columnIndex == i) {
                headerButton.setSelected(true);
                grid.selectedHeaderButton = headerButton;
            }

            if (columnObj.sortLabel) {
                headerButton.showSortDirection(columnObj.sortLabel.direction, sortsCount > 1 ? columnObj.sortLabel.sortIndex : null);
            }

            if (columnObj.filterLabel) {
                headerButton.showFilter(true);
            }
        }

        if (data.length > 1) {
            dataTable.style.whiteSpace = "normal";
            dataTable.style.fontFamily = isViewData ? "Arial" : (elementAttrs.font.name || "Arial");
            dataTable.style.fontSize = isViewData ? "10pt" : ((elementAttrs.font.size || "10") + "pt");
            dataTable.style.fontWeight = elementAttrs.font.bold && !isViewData ? "bold" : "normal";
            dataTable.style.fontStyle = elementAttrs.font.italic && !isViewData ? "italic" : "normal";
            dataTable.style.textDecoration = "";
            if (elementAttrs.font.strikeout) dataTable.style.textDecoration = "line-through";
            if (elementAttrs.font.underline) dataTable.style.textDecoration += " underline";

            grid.guid = Math.floor(Math.random() * 10000000);
            var cellsRules = [];
            var cellsInterlacedRules = [];

            for (var i = 1; i < data.length; i++) {
                var rowCels = [];
                grid.hideRows = i > 500;

                for (var k = 0; k < data[i].length; k++) {
                    var columnObj = data[0][k];
                    var cellObj = data[i][k];

                    //Add footer
                    if (showFooter && i == data.length - 1) {
                        var footerDiv = document.createElement("div");
                        var footerCell = footerTable.addCell(footerDiv);
                        footerDiv.style.textAlign = columnObj.summaryAlignment ? columnObj.summaryAlignment.toLowerCase() : "right";
                        footerDiv.innerText = cellObj.text != null ? cellObj.text : "";
                        footerCell.style.padding = "0 4px 0 4px";
                        footerCell.style.background = settings.footerBackground;
                        footerCell.style.fontFamily = isViewData ? "Arial" : (settings.footerFont.name || "Arial");
                        footerCell.style.fontSize = isViewData ? "10pt" : ((settings.footerFont.size || "10") + "pt");
                        footerCell.style.fontWeight = settings.footerFont.bold && !isViewData ? "bold" : "normal";
                        footerCell.style.fontStyle = settings.footerFont.italic && !isViewData ? "italic" : "normal";
                        footerCell.style.textDecoration = "";
                        if (settings.footerFont.strikeout) footerCell.style.textDecoration = "line-through";
                        if (settings.footerFont.underline) footerCell.style.textDecoration += " underline";
                        footerCell.style.color = settings.footerForeColor;
                        footerCell.style.height = settings.footerHeight + "px";
                        footerCell.style.vertAlign = "middle";

                        if (k < data[i].length - 1) {
                            footerCell.style.borderRight = "1px solid " + settings.tableBorderColor;
                        }
                        continue;
                    }

                    var cell = dataTable.addCellInLastRow();
                    cell.columnObj = columnObj;
                    cell.cellObj = cellObj;

                    var isInterlaced = i % 2 == 0;
                    var backgroundStyle = cellObj.backColor ? cellObj.backColor : (isInterlaced ? settings.cellBackColorInterlaced : settings.cellBackColor);
                    var colorStyle = cellObj.textColor ? cellObj.textColor : (isInterlaced ? columnObj.cellForeColorInterlaced : columnObj.cellForeColor);
                    var totalRules = "background:" + backgroundStyle + ";color:" + colorStyle + ";text-align:" + columnObj.align + ";";
                    if (k < data[i].length - 1) totalRules += "border-right:1px solid " + settings.tableBorderColor + " !important;";

                    if (cellObj.textFontStyle) {
                        if (cellObj.textFontStyle.bold) cell.style.fontWeight = "bold";
                        if (cellObj.textFontStyle.italic) cell.style.fontStyle = "italic";
                        if (cellObj.textFontStyle.strikeout || elementAttrs.font.underline) {
                            var textDecoration = "";
                            if (cellObj.textFontStyle.strikeout) textDecoration += "line-through";
                            if (cellObj.textFontStyle.underline) textDecoration += " underline";
                            if (textDecoration) cell.style.textDecoration = textDecoration;
                        }
                    }

                    if (i <= 2) {
                        if (isInterlaced)
                            cellsInterlacedRules[k] = totalRules;
                        else
                            cellsRules[k] = totalRules;
                    }
                    else if ((isInterlaced && cellsInterlacedRules[k] != totalRules) || (!isInterlaced && cellsRules[k] != totalRules)) {
                        cell.style.background = backgroundStyle;
                        cell.style.color = colorStyle;
                    }

                    cell.className = "stiJsViewerClearAllStyles" + ((isInterlaced ? " cellInterlaced_N" : " cell_N") + k + "_" + grid.guid);
                    cell.indexDataRow = i;
                    cell.rowCels = rowCels;
                    cell.tableKey = elementAttrs.key;
                    cell.defBackground = backgroundStyle;
                    cell.defColor = colorStyle;

                    rowCels[k] = { owningColumnName: columnObj.label, value: cellObj.text != null ? cellObj.text : (cellObj.image != null ? cellObj.image : "") };

                    cell.setSelected = function () {
                        if (grid.selectedCells) {
                            for (var i = 0; i < grid.selectedCells.length; i++) {
                                grid.selectedCells[i].style.background = grid.selectedCells[i].oldBackground;
                                grid.selectedCells[i].style.color = grid.selectedCells[i].oldColor;
                            }
                            grid.selectedCells = null;
                        }

                        grid.selectedCells = [this];

                        if (interaction && interaction.fullRowSelect) {
                            grid.selectedCells = [];
                            var row = dataTable.tr[this.indexDataRow - 1];
                            for (var i = 0; i < row.childNodes.length; i++) {
                                grid.selectedCells.push(row.childNodes[i]);
                            }
                        }

                        for (var i = 0; i < grid.selectedCells.length; i++) {
                            var cell = grid.selectedCells[i];
                            cell.oldBackground = cell.defBackground;
                            cell.oldColor = cell.defColor;
                            cell.style.background = settings.cellSelectedBackColor;
                            cell.style.color = settings.cellSelectedForeColor;
                        }
                    }

                    cell.onmouseup = function (event) {
                        this.setSelected();
                        grid.saveGridStates();

                        if (event.button == 2) {
                            var items = [];
                            items.push(jsObject.Item("copy", jsObject.collections.loc["Copy"], null, "copy"));

                            var menuStyle = (elementAttrs.actionColors && elementAttrs.actionColors.isDarkStyle) ? "stiJsViewerDbsDarkMenu" : "stiJsViewerDbsLightMenu";
                            var menu = jsObject.VerticalMenu("dbsGridCellContextMenu", null, "Down", items, menuStyle + "Item", menuStyle);
                            var mousePos = jsObject.FindMousePosOnMainPanel(event);
                            menu.currentCell = this;

                            menu.changeVisibleState(true, null, null, null, null, null, mousePos.xPixels + 10, mousePos.yPixels + 10);

                            menu.action = function (menuItem) {
                                if (menuItem.key == "copy") {
                                    jsObject.copyTextToClipboard(this.currentCell.cellObj.text || this.currentCell.innerHTML);
                                }
                                menu.changeVisibleState(false);
                            }
                        }
                    }

                    cell.oncontextmenu = function (event) {
                        return false;
                    }

                    //Add Interactions
                    grid.addInteractionsToCell(cell, columnObj, cellObj);

                    var imageCell = null;

                    if (columnObj.type == "StiDataBarsColumn" || columnObj.type == "StiBubbleColumn") {
                        var table = jsObject.CreateHTMLTable();
                        table.style.width = "100%";
                        cell.appendChild(table);

                        if (columnObj.align == "left") {
                            if (cellObj.text) {
                                var cell1 = table.addTextCell(cellObj.text);
                                cell1.style.textAlign = "right";
                                cell1.style.paddingRight = "4px";
                            }
                            if (cellObj.image) {
                                var cell2 = table.addCell();
                                cell2.style.height = cellObj.imageHeight + "px";
                                cell2.style.width = (cellObj.imageWidth / 2) + "px";
                                cell2.style.backgroundImage = "url(" + cellObj.image + ")";
                                cell2.style.backgroundRepeat = "no-repeat";
                                cell2.style.backgroundPositionX = "right";
                                cell2.style.backgroundPositionY = "center";
                            }
                        }
                        else {
                            if (cellObj.image) {
                                var cell1 = table.addCell();
                                cell1.style.height = cellObj.imageHeight + "px";
                                cell1.style.width = (cellObj.imageWidth / 2) + "px";
                                cell1.style.backgroundImage = "url(" + cellObj.image + ")";
                                cell1.style.backgroundRepeat = "no-repeat";
                                cell1.style.backgroundPositionX = "left";
                                cell1.style.backgroundPositionY = "center";
                            }
                            if (cellObj.text) {
                                var cell2 = table.addTextCell(cellObj.text);
                                cell2.style.textAlign = "right";
                                cell2.style.paddingRight = "4px";
                            }
                        }
                    }
                    else if (columnObj.type == "StiSparklinesColumn") {
                        if (cellObj.sparkline) {
                            var svgContainer = document.createElement("div");
                            svgContainer.innerHTML = cellObj.sparkline;
                            svgContainer.style.width = cellObj.sparklineWidth + "px";
                            svgContainer.style.height = cellObj.sparklineHeight + "px";
                            cell.appendChild(svgContainer);
                        }
                    }
                    else if (columnObj.type == "StiColorScaleColumn") {
                        if (cellObj.text) {
                            cell.innerText = cellObj.text;
                        }
                    }
                    else if (columnObj.type == "StiIndicatorColumn") {
                        if (cellObj.imageWidth && cellObj.imageHeight && cellObj.textHeight && cellObj.textWidth) {
                            var div = document.createElement("div");
                            div.style.position = "relative";
                            div.style.height = Math.max(cellObj.textHeight, cellObj.imageHeight) + "px";
                            div.style.width = Math.max(cellObj.textWidth, cellObj.imageWidth) + "px";
                            div.style.display = "inline-block";
                            cell.appendChild(div);

                            var image = document.createElement("img");
                            image.src = cellObj.image;
                            image.style.width = cellObj.imageWidth + "px";
                            image.style.height = cellObj.imageHeight + "px";
                            image.style.left = "0px";
                            image.style.top = "0px";
                            image.style.position = "absolute";
                            div.appendChild(image);

                            var textTable = jsObject.CreateHTMLTable();
                            if (cellObj.text) textTable.addTextCell(cellObj.text);
                            textTable.style.height = cellObj.textHeight + "px";
                            textTable.style.width = cellObj.textWidth + "px";
                            textTable.style.left = (columnObj.align == "left" ? (cellObj.imageWidth - cellObj.textWidth) : (columnObj.align == "center" ? - 3 : 0)) + "px";
                            textTable.style.top = "0px";
                            textTable.style.position = "absolute";
                            div.appendChild(textTable);
                        }
                        else {
                            var table = jsObject.CreateHTMLTable();
                            table.style.width = "100%";
                            cell.appendChild(table);

                            var image = document.createElement("img");
                            image.src = cellObj.image;
                            if (cellObj.imageWidth) image.style.width = cellObj.imageWidth + "px";
                            if (cellObj.imageHeight) image.style.height = cellObj.imageHeight + "px";

                            if (columnObj.align == "left") {
                                if (cellObj.image) imageCell = table.addCell(image);
                                if (cellObj.text) table.addTextCell(cellObj.text).textAlign = columnObj.align;
                            }
                            else {
                                if (cellObj.text) table.addTextCell(cellObj.text).textAlign = columnObj.align;
                                if (cellObj.image) imageCell = table.addCell(image);
                            }
                        }
                    }
                    else if (columnObj.type == "StiDimensionColumn" && columnObj.showHyperlink) {
                        var value = cellObj.value != null ? cellObj.value : (cellObj.text != null ? cellObj.text : "");
                        cell.innerText = cellObj.text != null ? cellObj.text : "";
                        cell.style.textDecoration = "underline";
                        cell.style.color = cellObj.textColor ? cellObj.textColor : (i % 2 == 0 ? columnObj.cellForeColorInterlaced : columnObj.cellForeColor);
                        cell.link = columnObj.hyperlinkPattern ? grid.replaceRowValues(cell, jsObject.ReplaceAllKeysInText(columnObj.hyperlinkPattern, { value: value })) : null;
                        cell.style.cursor = cell.link ? "pointer" : "default";

                        if (cell.link) {
                            cell.onclick = function () {
                                var link = this.link;
                                if (jsObject.options.appearance.openLinksWindow == "_self")
                                    window.location.href = link;
                                else
                                    setTimeout(function () { jsObject.openNewWindow(link, jsObject.options.appearance.openLinksWindow, undefined, false); }, 50);
                            };
                        }
                    }
                    else {
                        if (columnObj.dataType == "image" || columnObj.dataType == "byte[]" || cellObj.image) {
                            var img = document.createElement("img");
                            img.style.maxWidth = img.style.maxHeight = "60px";
                            var imageValue = cellObj.image || cellObj.text;
                            if (imageValue && imageValue.indexOf("data:image") != 0) {
                                imageValue = "data:image/png;base64," + imageValue;
                            }
                            img.src = imageValue;
                            cell.style.width = cell.style.height = "60px";
                            cell.appendChild(img);
                        }
                        else if (columnObj.dataType == "bool") {
                            var boolValue = cellObj.boolValue;
                            if (boolValue == null && cellObj.text) {
                                boolValue = cellObj.text.toLowerCase() == "true";
                            }
                            var checkBox = jsObject.CheckBox();
                            checkBox.setChecked(boolValue);
                            checkBox.onmouseover = null;
                            checkBox.onmouseout = null;
                            checkBox.onmouseenter = null;
                            checkBox.onmouseleave = null;
                            checkBox.onclick = null;
                            checkBox.style.display = "inline-block";
                            if (cellObj.textColor) {
                                checkBox.imageBlock.style.border = "1px solid " + cellObj.textColor;
                            }
                            cell.style.width = "50px";
                            cell.style.textAlign = "center";
                            cell.appendChild(checkBox);
                        }
                        else {
                            var text = cellObj.text != null ? cellObj.text : "";
                            text = text.toString().replace(/\r\n/g, "<br>").replace(/\n/g, "<br>").replace(/\r/g, "<br>");

                            if (text.indexOf("<br>") >= 0) {
                                cell.innerHTML = text;
                            }
                            else {
                                cell.innerText = text;
                            }
                        }
                    }

                    if (imageCell) {
                        imageCell.style.width = "1px";
                        imageCell.style.verticalAlign = "middle";
                        imageCell.style.fontSize = "0";
                    }
                }

                if (i < data.length - 1) {
                    if (grid.hideRows) {
                        var lastRow = dataTable.tr[dataTable.tr.length - 1];
                        grid.hideRow(lastRow);
                    }
                    dataTable.addRow();
                }
            }

            var cellCommonRules = "padding:0 4px 0 4px !important;height: " + settings.cellHeight + "px;";
            for (var i = 0; i < cellsRules.length; i++) {
                var style = grid.createCssStyle();
                grid.addRulesToStyle(style, ".cell_N" + i + "_" + grid.guid, cellCommonRules + cellsRules[i]);
            }
            for (var i = 0; i < cellsInterlacedRules.length; i++) {
                var style = grid.createCssStyle();
                grid.addRulesToStyle(style, ".cellInterlaced_N" + i + "_" + grid.guid, cellCommonRules + cellsInterlacedRules[i]);
            }
        }

        var scrollHiddenColor = "rgba(191, 191, 191, 0)";
        var scrollDivColor = "rgba(191, 191, 191, 0.1)";
        var scrollColor = "rgba(191, 191, 191, 0.7)";
        var scrollSelectedColor = "rgba(191, 191, 191, 0.9)";

        //horizontal scroll 
        var cellsHScrollDiv = document.createElement("div");
        grid.parentElement.appendChild(cellsHScrollDiv);
        cellsHScrollDiv.style.bottom = "0";
        cellsHScrollDiv.style.left = "0";
        cellsHScrollDiv.style.width = "100%";
        cellsHScrollDiv.style.position = "absolute";
        cellsHScrollDiv.style.boxSizing = "border-box";
        cellsHScrollDiv.style.height = "8px";
        cellsHScrollDiv.style.touchAction = "none";
        cellsHScrollDiv.style.backgroundColor = scrollHiddenColor;
        cellsHScrollDiv.style.transition = "background-color 0.5s ease";
        cellsHScrollDiv.style.poinerEvents = "none";
        cellsHScrollDiv.className = "pivotUnselectable";

        var cellHScroll = document.createElement("div");
        cellsHScrollDiv.appendChild(cellHScroll);
        cellHScroll.style.height = "100%";
        cellHScroll.style.width = "73%";
        cellHScroll.style.backgroundColor = scrollHiddenColor;
        cellHScroll.style.transition = "background-color 0.5s ease";
        cellHScroll.className = "pivotUnselectable";
        grid.hScrollOffset = 0;
        grid.hOffset = 1;

        //vertical scroll 
        var cellsVScrollDiv = document.createElement("div");
        grid.parentElement.appendChild(cellsVScrollDiv);
        cellsVScrollDiv.style.top = "0";
        cellsVScrollDiv.style.right = "0";
        cellsVScrollDiv.style.height = "100%";
        cellsVScrollDiv.style.position = "absolute";
        cellsVScrollDiv.style.boxSizing = "border-box";
        cellsVScrollDiv.style.width = "8px";
        cellsVScrollDiv.style.touchAction = "none";
        cellsVScrollDiv.style.backgroundColor = scrollHiddenColor;
        cellsVScrollDiv.style.transition = "background-color 0.5s ease";
        cellsVScrollDiv.style.poinerEvents = "none";
        cellsVScrollDiv.className = "pivotUnselectable";

        var cellVScroll = document.createElement("div");
        cellsVScrollDiv.appendChild(cellVScroll);
        cellVScroll.style.width = "100%";
        cellVScroll.style.height = "73%";
        cellVScroll.style.backgroundColor = scrollHiddenColor;
        cellVScroll.style.transition = "background-color 0.5s ease";
        cellVScroll.className = "pivotUnselectable";
        grid.vScrollOffset = 0;
        grid.vOffset = 1;

        jsObject.addEvent(document.body, "mousemove", function (event) {
            if (grid.screenX) {
                var emptyWidth = grid.offsetWidth - grid.offsetWidth / grid.hOffset;
                var offset = Math.max(0, Math.min(emptyWidth, event.screenX - grid.screenX + grid.startXOffset));
                cellHScroll.style.transform = "translateX(" + offset + "px)";
                grid.scrollLeft = offset * grid.hOffset;
                grid.hScrollOffset = offset;
            }

            if (grid.screenY) {
                var emptyHeight = (grid.offsetHeight - headerTable.offsetHeight - footerTable.offsetHeight) - (grid.offsetHeight - headerTable.offsetHeight - footerTable.offsetHeight) / grid.vOffset;
                var offset = Math.max(0, Math.min(emptyHeight, event.screenY - grid.screenY + grid.startYOffset));
                cellVScroll.style.transform = "translateY(" + offset + "px)";
                cellsContainer.scrollTop = offset * grid.vOffset;
                grid.vScrollOffset = offset;
                grid.checkDisplaingRows();
            }
        }, grid);

        jsObject.addEvent(document.body, "mouseup", function (event) {
            grid.screenX = null;
            grid.screenY = null;
            if (!grid.mouseOver) {
                cellsHScrollDiv.style.backgroundColor = scrollHiddenColor;
                cellHScroll.style.backgroundColor = scrollHiddenColor;
                cellsVScrollDiv.style.backgroundColor = scrollHiddenColor;
                cellVScroll.style.backgroundColor = scrollHiddenColor;
            } else {
                if (grid.hOffset > 1)
                    cellHScroll.style.backgroundColor = scrollColor;
                if (grid.vOffset > 1)
                    cellVScroll.style.backgroundColor = scrollColor;
            }
        }, grid);

        cellHScroll.onmousedown = function (event) {
            event.stopPropagation();
            if (grid.hOffset > 1) {
                grid.screenX = event.screenX;
                grid.startXOffset = grid.hScrollOffset;
                cellHScroll.style.backgroundColor = scrollSelectedColor;
            }
        }

        cellVScroll.onmousedown = function (event) {
            event.stopPropagation();
            if (grid.vOffset > 1) {
                grid.screenY = event.screenY;
                grid.startYOffset = grid.vScrollOffset;
                cellVScroll.style.backgroundColor = scrollSelectedColor;
            }
        }

        cellsHScrollDiv.onmousedown = function (event) {
            var sWidth = grid.offsetWidth / grid.hOffset;
            var emptyWidth = grid.offsetWidth - sWidth;

            var mouseLeftPos = jsObject.FindMousePosOnMainPanel(event).xPixels;
            var scrollLeftPos = jsObject.FindPosX(cellsHScrollDiv, "stiJsViewerMainPanel", false);
            var transformStyle = cellHScroll.style.transform;

            if (transformStyle) {
                var transformX = cellHScroll.style.transform.substring(cellHScroll.style.transform.indexOf("(") + 1);
                if (transformX) transformX = transformX.substring(0, transformX.length - 1);
                scrollLeftPos += (parseInt(transformX) || 0)
            }

            if (scrollLeftPos > mouseLeftPos)
                sWidth *= -1;

            var offset = Math.max(0, Math.min(emptyWidth, sWidth + grid.hScrollOffset));
            cellHScroll.style.transform = "translateX(" + offset + "px)";
            grid.scrollLeft = offset * grid.hOffset;
            grid.hScrollOffset = offset;
        }

        cellsVScrollDiv.onmousedown = function (event) {
            var sHeight = (grid.offsetHeight - headerTable.offsetHeight - footerTable.offsetHeight) / grid.vOffset;
            var emptyHeight = (grid.offsetHeight - headerTable.offsetHeight - footerTable.offsetHeight) - sHeight;

            var mouseTopPos = jsObject.FindMousePosOnMainPanel(event).yPixels;
            var scrollTopPos = jsObject.FindPosY(cellsVScrollDiv, "stiJsViewerMainPanel", false);
            var transformStyle = cellVScroll.style.transform;

            if (transformStyle) {
                var transformY = cellVScroll.style.transform.substring(cellVScroll.style.transform.indexOf("(") + 1);
                if (transformY) transformY = transformY.substring(0, transformY.length - 1);
                scrollTopPos += (parseInt(transformY) || 0)
            }

            if (scrollTopPos > mouseTopPos)
                sHeight *= -1;

            var offset = Math.max(0, Math.min(emptyHeight, sHeight + grid.vScrollOffset));
            cellVScroll.style.transform = "translateY(" + offset + "px)";
            cellsContainer.scrollTop = offset * grid.vOffset;
            grid.vScrollOffset = offset;
            grid.checkDisplaingRows();
        }

        //grid.moveColumn = function (fromIndex, toIndex) {

        //}

        grid.checkDisplaingRows = function () {
            if (grid.hideRows) {
                clearTimeout(grid.scrollTimer);

                grid.scrollTimer = setTimeout(function () {
                    var commonRowsHeight = 0;
                    for (var i = 0; i < dataTable.tr.length; i++) {
                        commonRowsHeight += dataTable.tr[i].offsetHeight;
                        if (commonRowsHeight > cellsContainer.scrollTop) {
                            var startIndex = Math.max(i - 5, 0);
                            var endIndex = Math.min(startIndex + 100, dataTable.tr.length);
                            for (var k = startIndex; k < endIndex; k++) {
                                var row = dataTable.tr[k];
                                if (row.isHidden) grid.displayRow(row);
                            }
                            break;
                        }
                    }
                }, 20);
            }
        }

        grid.showScrolls = function () {
            if (grid.hOffset > 1) {
                cellsHScrollDiv.style.backgroundColor = scrollDivColor;
                cellHScroll.style.backgroundColor = scrollColor;
            }
            if (grid.vOffset > 1) {
                cellsVScrollDiv.style.backgroundColor = scrollDivColor;
                cellVScroll.style.backgroundColor = scrollColor;
            }
        }

        grid.hideScrolls = function () {
            if (!grid.screenX) {
                cellsHScrollDiv.style.backgroundColor = scrollHiddenColor;
                cellHScroll.style.backgroundColor = scrollHiddenColor;
            }
            if (!grid.screenY) {
                cellsVScrollDiv.style.backgroundColor = scrollHiddenColor;
                cellVScroll.style.backgroundColor = scrollHiddenColor;
            }
        }

        grid.parentElement.onmouseover = function () {
            if (jsObject.options.isTouchDevice) return;
            grid.mouseOver = true;
            grid.showScrolls();
        }

        grid.parentElement.onmouseout = function () {
            if (jsObject.options.isTouchDevice) return;
            grid.mouseOver = false;
            grid.hideScrolls();
        }

        grid.repaintVertScrolls = function () {
            if (dataTable.offsetHeight) {
                cellVScroll.style.transform = "translateY(" + (cellsVScrollDiv.offsetHeight - cellVScroll.offsetHeight) * (cellsContainer.scrollTop / (dataTable.offsetHeight - cellsContainer.offsetHeight)) + "px)";
            }
        }

        grid.repaintHorScrolls = function () {
            if (dataTable.offsetWidth) {
                cellHScroll.style.transform = "translateX(" + (cellsHScrollDiv.offsetWidth - cellHScroll.offsetWidth) * (grid.scrollLeft / (dataTable.offsetWidth - grid.offsetWidth)) + "px)";
            }
        }



        if (jsObject.options.isTouchDevice) {
            cellsContainer.onscroll = function () {
                grid.repaintVertScrolls();
            }
        }

        grid.addWheel = function (elem) {
            if ('onwheel' in document) {
                jsObject.addEvent(elem, "wheel", onWheel, grid);
            } else if ('onmousewheel' in document) {
                jsObject.addEvent(elem, "mousewheel", onWheel, grid);
            } else {
                jsObject.addEvent(elem, "MozMousePixelScroll", onWheel, grid);
            }

            function onWheel(e) {
                e = e || window.event;
                var delta = e.wheelDelta || e.deltaY || e.detail;
                if (e.wheelDelta == null && e.deltaY != null) delta *= -40; //fix for firefox
                grid.delta = delta;
                e.preventDefault ? e.preventDefault() : (e.returnValue = false);
                var visualContainerHeight = grid.offsetHeight - headerTable.offsetHeight - footerTable.offsetHeight;
                var sHeight = visualContainerHeight / grid.vOffset;
                var emptyHeight = visualContainerHeight - sHeight;
                var offset = Math.min(emptyHeight, Math.max(0, -delta / 5 + (grid.preOffset || grid.vScrollOffset)));
                grid.preOffset = offset;
                var newScrollTop = offset * grid.vOffset;

                if (offset > 0 && Math.abs(newScrollTop - cellsContainer.scrollTop) > visualContainerHeight) {
                    if (newScrollTop <= 0) newScrollTop = 1;
                    var scrollTopOffset = delta < 0 ? cellsContainer.scrollTop + visualContainerHeight : cellsContainer.scrollTop - visualContainerHeight;
                    offset = scrollTopOffset * offset / newScrollTop;
                    newScrollTop = offset * grid.vOffset;
                }

                cellVScroll.style.transform = "translateY(" + offset + "px)";
                cellsContainer.scrollTop = newScrollTop;
                grid.vScrollOffset = offset;
                grid.checkDisplaingRows();
            }
        }

        grid.saveGridStates = function () {
            if (!jsObject.tableElementGridStates) jsObject.tableElementGridStates = {};

            var gridStates = {
                selectedCells: this.selectedCells,
                cellVScrollTransform: cellVScroll.style.transform,
                cellsContainerScrollTop: cellsContainer.scrollTop,
                vScrollOffset: grid.vScrollOffset
            }

            if (settings.sizeMode != "Fit" && this.headerButtons && this.headerButtons.length > 0) {
                gridStates.columnsCount = this.headerButtons.length;
                gridStates.columnsWidth = [];
                for (var i = 0; i < this.headerButtons.length; i++) {
                    gridStates.columnsWidth[i] = parseInt(this.headerButtons[i].style.width.replace("px", ""));
                }
            }

            jsObject.tableElementGridStates[elementAttrs.key] = gridStates;
        }

        grid.restoreGridStates = function () {
            var gridStates = jsObject.tableElementGridStates ? jsObject.tableElementGridStates[elementAttrs.key] : null;
            if (gridStates) {
                cellVScroll.style.transform = gridStates.cellVScrollTransform;
                cellsContainer.scrollTop = gridStates.cellsContainerScrollTop;
                grid.vScrollOffset = gridStates.vScrollOffset;
                grid.checkDisplaingRows();
                if (gridStates.selectedCells && gridStates.selectedCells.length > 0) {
                    var rowIndex = gridStates.selectedCells[0].indexDataRow - 1;
                    var cellIndex = gridStates.selectedCells[0].cellIndex;
                    var row = dataTable.tr && dataTable.tr.length > rowIndex
                        ? dataTable.tr[rowIndex]
                        : dataTable.tr.length > 0 ? dataTable.tr[dataTable.tr.length - 1] : null;
                    if (row) {
                        var cell = row.childNodes && row.childNodes.length > cellIndex
                            ? row.childNodes[cellIndex]
                            : row.childNodes.length > 0 ? row.childNodes[row.childNodes.length - 1] : null;
                        if (cell) cell.setSelected();
                    }
                }
            }
        }

        //Update cell sizes
        setTimeout(function () {
            if ((headerTableRow.childNodes.length == 0 && dataTableRow.childNodes.length == 0) ||
                (headerTableRow.childNodes.length > 0 && dataTableRow.childNodes.length > 0 && headerTableRow.childNodes.length != dataTableRow.childNodes.length)) {
                grid.hideProgress();
                return;
            }

            var gridStates = jsObject.tableElementGridStates[elementAttrs.key];
            var tableRow = dataTableRow.childNodes.length > 0 ? dataTableRow : headerTableRow;
            var cellsRules = [];

            for (var i = 0; i < tableRow.childNodes.length; i++) {
                var width = Math.max(tableRow.childNodes[i].offsetWidth, headerTableRow.childNodes[i].offsetWidth);
                if (showFooter) width = Math.max(width, footerTableRow.childNodes[i].offsetWidth);
                if (width > 300) width = 300;

                var columnObj = tableRow.childNodes[i].columnObj;
                var sizePriority = false;

                if (columnObj && settings.sizeMode != "Fit") {
                    if (columnObj.sizeWidth) {
                        width = columnObj.sizeWidth;
                        sizePriority = true;
                    }
                    if (columnObj.sizeMaxWidth && width > columnObj.sizeMaxWidth) {
                        width = columnObj.sizeMaxWidth;
                        sizePriority = true;
                    }
                    if (columnObj.sizeMinWidth && width < columnObj.sizeMinWidth) {
                        width = columnObj.sizeMinWidth;
                    }
                }

                if (gridStates && gridStates.columnsCount > 0 && gridStates.columnsCount == tableRow.childNodes.length && i < gridStates.columnsWidth.length) {
                    if (gridStates.columnsWidth[i] > 0 && gridStates.columnsWidth[i] > width) width = gridStates.columnsWidth[i];
                }

                var headerButton = headerTableRow.childNodes[i].firstChild;
                headerButton.style.width = width + "px";

                if (showFooter) {
                    var footerDiv = footerTableRow.childNodes[i].firstChild;
                    footerDiv.style.width = width + "px";
                }

                if (settings.sizeMode != "Fit" && headerButton.caption.offsetWidth + 45 > headerButton.offsetWidth && headerButton.caption.offsetWidth != 0) {
                    if (!sizePriority) {
                        width = headerButton.caption.offsetWidth + 45;
                        headerButton.style.width = width + "px";
                        if (showFooter) footerDiv.style.width = width + "px";
                    }
                    else {
                        var captionContent = document.createElement("div");
                        captionContent.innerHTML = headerButton.caption.innerHTML;
                        captionContent.style.display = "block";
                        captionContent.style.textOverflow = "ellipsis";
                        captionContent.style.overflow = "hidden";
                        captionContent.style.width = Math.max(headerButton.offsetWidth - 45, 10) + "px";
                        headerButton.caption.innerHTML = "";
                        headerButton.caption.appendChild(captionContent);
                    }
                }

                var maxHeight = 0;
                for (var k = 0; k < dataTable.tr.length; k++) {
                    var row = dataTable.tr[k];
                    if (grid.hideRows) {
                        if (row.isHidden)
                            row.style.height = maxHeight + "px";
                        else if (k < 10)
                            maxHeight = Math.max(maxHeight, row.offsetHeight);
                    }
                    if (i < row.childNodes.length) {
                        var cell = row.childNodes[i];
                        var divCell = document.createElement("div");
                        var cellRules = "width:" + width + "px;text-align:" + cell.columnObj.align + ";overflow:hidden;white-space:" + (cell.columnObj.sizeWordWrap === false ? "nowrap" : "normal") + ";";
                        var type = cell.columnObj.type;
                        var usePadding = (type != "StiDataBarsColumn" && type != "StiSparklinesColumn" && type != "StiColorScaleColumn" && type != "StiIndicatorColumn") &&
                            !(columnObj.dataType == "image" || columnObj.dataType == "byte[]" || cellObj.image || columnObj.dataType == "bool");
                        if (usePadding) cellRules += "padding:2px 0 2px 0 !important;";
                        divCell.className = "cellContent_N" + i + "_" + grid.guid;
                        divCell.innerHTML = cell.innerHTML;
                        cell.innerHTML = "";
                        cell.appendChild(divCell);
                        if (!cellsRules[i]) cellsRules[i] = cellRules;
                    }
                }
            }

            for (var i = 0; i < cellsRules.length; i++) {
                var style = grid.createCssStyle();
                grid.addRulesToStyle(style, ".cellContent_N" + i + "_" + grid.guid, cellsRules[i]);
            }

            cellsContainer.style.minWidth = Math.max(width, headerTable.offsetWidth, footerTable.offsetWidth) + "px";
            cellsContainer.style.height = (grid.offsetHeight - headerTable.offsetHeight - footerTable.offsetHeight) + "px";

            if (settings.sizeMode == "Fit" && headerTable.offsetWidth < grid.offsetWidth && headerTable.offsetWidth > 0) {
                var xFactor = grid.offsetWidth / headerTable.offsetWidth;

                for (var i = 0; i < tableRow.childNodes.length; i++) {
                    var headerButton = headerTableRow.childNodes[i].firstChild;
                    var newWidth = headerButton.offsetWidth * xFactor - 8;

                    if (dataTableRow.childNodes.length > 0) {
                        for (var k = 0; k < dataTable.tr.length; k++) {
                            if (i < dataTable.tr[k].childNodes.length) {
                                var cell = dataTable.tr[k].childNodes[i];
                                var divCell = cell.firstChild;
                                headerButton.style.width = divCell.style.width = newWidth + "px";
                                if (showFooter) footerTableRow.childNodes[i].firstChild.style.width = newWidth + "px";
                            }
                        }
                    }
                    else {
                        headerButton.style.width = newWidth + "px";
                        if (showFooter) footerTableRow.childNodes[i].firstChild.style.width = newWidth + "px";
                    }
                }
            }

            grid.hOffset = (headerTable.offsetWidth - 4) / grid.offsetWidth;
            grid.vOffset = dataTable.offsetHeight / (grid.offsetHeight - headerTable.offsetHeight - footerTable.offsetHeight);
            cellHScroll.style.width = Math.min(grid.offsetWidth, grid.offsetWidth / grid.hOffset) + "px";
            cellVScroll.style.height = Math.min(grid.offsetHeight, (grid.offsetHeight - headerTable.offsetHeight - footerTable.offsetHeight) / grid.vOffset) + "px";
            cellsVScrollDiv.style.top = headerTable.offsetHeight + "px";
            cellsVScrollDiv.style.height = (grid.offsetHeight - headerTable.offsetHeight - footerTable.offsetHeight) + "px";

            grid.addWheel(grid.parentElement);
            grid.hideProgress();
            grid.restoreGridStates();
            if (jsObject.options.isTouchDevice) grid.showScrolls();
        }, 0);
    }

    grid.replaceRowValues = function (currentCell, inputText) {
        var indexDataRow = currentCell.indexDataRow;
        if (indexDataRow < this.data.length) {
            var columns = this.data[0];
            var cells = this.data[indexDataRow];

            if (this.hiddenData && this.hiddenData.length > 0 && indexDataRow < this.hiddenData.length) {
                columns = columns.concat(this.hiddenData[0]);
                cells = cells.concat(this.hiddenData[indexDataRow]);
            }

            for (var i = 0; i < cells.length; i++) {
                var columnName = columns[i].labelCorrect;
                var value = cells[i].value != null ? cells[i].value : (cells[i].text || "");
                if (columnName != null && value != null) {
                    inputText = inputText.replace(new RegExp("{row." + columnName.toLowerCase() + "}", 'g'), value).replace(new RegExp("{Row." + columnName + "}", 'g'), value);
                }
            }
        }
        return inputText;
    }

    grid.addInteractionsToCell = function (cell, columnObj, cellObj) {
        if (columnObj.interaction) {
            var value = cellObj.value != null ? cellObj.value : (cellObj.text != null ? cellObj.text : "");

            if (columnObj.interaction.onHover == "ShowToolTip" && columnObj.interaction.toolTip ||
                columnObj.interaction.onHover == "ShowHyperlink" && (columnObj.interaction.hyperlink || (columnObj.showHyperlink && columnObj.hyperlinkPattern))) {

                var toolTipText = columnObj.interaction.onHover == "ShowToolTip" ? columnObj.interaction.toolTip : (columnObj.interaction.hyperlink || columnObj.hyperlinkPattern);
                toolTipText = jsObject.CorrectTooltipText(grid.replaceRowValues(cell, jsObject.ReplaceAllKeysInText(toolTipText, { value: value })));

                if (columnObj.interaction.onHover == "ShowHyperlink") {
                    toolTipText = "<font style=\"color: #0645ad; text-decoration: underline;\">" + toolTipText + "</font>";
                }

                cell.setAttribute("_text1", toolTipText);

                if (!document._stiTooltip)
                    jsObject.CreateCustomTooltip(document, jsObject.controls.mainPanel);

                jsObject.AddCustomTooltip(cell, document);
            }

            if (columnObj.interaction.onClick == "OpenHyperlink" && columnObj.interaction.hyperlink) {
                cell.style.cursor = "pointer";
                cell.onclick = function () {
                    var link = grid.replaceRowValues(cell, jsObject.ReplaceAllKeysInText(columnObj.interaction.hyperlink, { value: value }));
                    if (jsObject.options.appearance.openLinksWindow == "_self")
                        window.location.href = link;
                    else
                        jsObject.openNewWindow(link, jsObject.options.appearance.openLinksWindow, undefined, false);
                }
            }
            else if (columnObj.interaction.onClick == "ApplyFilter") {
                cell.style.cursor = "pointer";
                cell.onclick = function () {
                    var filters = [jsObject.DataFilterObject(columnObj.key, columnObj.path, "EqualTo", value)];
                    jsObject.ApplyFiltersToDashboardElement(element, filters, true);
                }
            }
            else if (columnObj.interaction.onClick == "ShowDashboard" && columnObj.interaction.drillDownPageKey) {
                cell.style.cursor = "pointer";
                cell.onclick = function (event) {
                    var drillDownParameters = {
                        drillDownPageKey: columnObj.interaction.drillDownPageKey,
                        value: value,
                        rowCels: cell.rowCels,
                        tableKey: cell.tableKey,
                        parameters: []
                    }
                    var parameters = columnObj.interaction.drillDownParameters;
                    if (parameters) {
                        for (var i = 0; i < parameters.length; i++) {
                            drillDownParameters.parameters.push({
                                key: parameters[i].name,
                                value: grid.replaceRowValues(cell, parameters[i].expression ? jsObject.ReplaceAllKeysInText(parameters[i].expression, { value: value }) : "")
                            });
                        }
                    }
                    jsObject.postInteraction({
                        action: "DashboardDrillDown",
                        drillDownParameters: drillDownParameters,
                        pageNumber: 0
                    });
                }
            }
        }
    }

    grid.action = function () {

    }

    return grid;
}

StiJsViewer.prototype.GridHeaderButton = function (grid, columnObj, settings, allowInteractions, disableEvents, isViewData) {
    var jsObject = this;

    var button = this.SmallButton(null, columnObj.label, null, null, isViewData ? null : "Down");
    button.sort = null;
    button.filter = null;
    button.columnObject = columnObj;
    button.columnIndex = columnObj.columnIndex;
    button.style.border = "0px";
    button.style.color = "#ffffff";
    button.style.borderRadius = 0;
    button.style.overflow = "hidden";
    button.style.padding = "0 4px 0 4px";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.height = "100%";
    innerTable.style.width = "100%";
    button.removeChild(button.innerTable);
    button.appendChild(innerTable);

    var sortImage = document.createElement("img");
    sortImage.style.marginLeft = "4px";
    sortImage.style.width = "8px";
    sortImage.style.height = "12px";
    sortImage.style.visibility = "hidden";
    StiJsViewer.setImageSource(sortImage, this.options, this.collections, "Dashboards.SortAsc.png");
    button.sortImage = sortImage;
    var sortCell = innerTable.addCell(sortImage);
    sortCell.button = button;
    sortCell.style.cursor = "pointer";
    button.sortCell = sortCell;

    var sortNumber = document.createElement("div");
    var numberCell = innerTable.addCell(sortNumber);
    sortNumber.style.width = "6px";
    sortNumber.style.marginRight = "4px";
    sortNumber.style.fontSize = "8px";
    sortNumber.style.color = "#ffffff";
    sortNumber.style.textAlign = "right";
    numberCell.style.verticalAlign = "bottom";
    numberCell.style.paddingBottom = "4px";

    button.caption = innerTable.addTextCell(columnObj.label);
    button.caption.style.whiteSpace = "nowrap";
    button.caption.style.textAlign = columnObj.headerAlignment ? columnObj.headerAlignment.toLowerCase() : "center";
    button.caption.style.width = "100%";

    var filterImage = document.createElement("img");
    filterImage.style.margin = "0 6px 0 6px";
    filterImage.style.width = "8px";
    filterImage.style.height = "8px";
    filterImage.style.visibility = "hidden";
    StiJsViewer.setImageSource(filterImage, this.options, this.collections, "Arrows.SmallArrowDownWhite.png");
    innerTable.addCell(filterImage);

    if (!disableEvents) {
        button.onmouseoverAction = function () {
            if (!allowInteractions) return;
            if (!this.isEnabled || jsObject.options.isTouchClick || (this["haveMenu"] && this.isSelected)) return;
            this.className = this.styleName + " " + this.styleName + "Over";
            this.isOver = true;
            if (!isViewData) filterImage.style.visibility = "visible";
            this.style.background = settings.headerMouseOverBackground;
            sortImage.style.visibility = "visible";
            sortImage.style.opacity = button.sortDirection == "Ascending" || button.sortDirection == "Descending" || sortImage.isOver ? "1" : "0.7";
        }

        button.onmouseoutAction = function () {
            if (!allowInteractions) return;
            this.isOver = false;
            if (!this.isEnabled) return;
            this.className = this.styleName + " " + this.styleName + (this.isSelected ? "Selected" : "Default");
            this.style.background = this.isSelected ? settings.headerSelectedBackground : settings.headerBackground;
            if (!this.filter && !isViewData) filterImage.style.visibility = "hidden";
            sortImage.style.visibility = button.sortDirection == "Ascending" || button.sortDirection == "Descending" ? "visible" : "hidden";
            sortImage.style.opacity = "1";
        }
    }

    button.setSelected = function (state) {
        this.isSelected = state;
        this.className = this.styleName + " " + this.styleName +
            (state ? "Selected" : (this.isEnabled ? (this.isOver ? "Over" : "Default") : "Disabled"));
        this.style.background = state && this.isEnabled
            ? settings.headerMouseSelectedBackground
            : (this.isOver ? settings.headerMouseOverBackground : settings.headerBackground);
    }

    button.showSortDirection = function (sortDirection, number) {
        this.sortDirection = sortDirection;
        sortImage.style.visibility = sortDirection == "Ascending" || sortDirection == "Descending" ? "visible" : "hidden";
        StiJsViewer.setImageSource(sortImage, jsObject.options, jsObject.collections, sortDirection == "Ascending" ? "Dashboards.SortAsc.png" : "Dashboards.SortDesc.png");
        sortNumber.innerHTML = number || "";
    }

    button.showFilter = function (filter) {
        StiJsViewer.setImageSource(filterImage, jsObject.options, jsObject.collections, filter ? "Dashboards.Actions.Dark.DropDownFilter.png" : "Arrows.SmallArrowDownWhite.png");
        filterImage.style.width = filter ? "16px" : "8px";
        filterImage.style.height = filter ? "16px" : "8px";
        filterImage.style.visibility = filter ? "visible" : "hidden";
        filterImage.style.margin = filter ? "0 2px 0 3px" : "0 8px 0 8px";
        this.filter = filter;
    }

    button.oncontextmenu = function () {
        return false;
    }

    button.onmousedown = function () {
        if (this.isTouchStartFlag || !this.isEnabled) return;
        jsObject.options.buttonPressed = this;
        jsObject.gridHeaderInDrag = jsObject.GridHeaderForDragDrop(this);
    }

    if (!allowInteractions || disableEvents) {
        button.action = function () { };
    }

    return button;
}

StiJsViewer.prototype.GridHeaderForDragDrop = function (headerButton) {
    var jsObject = this;

    var dragElem = document.createElement("div");
    dragElem.className = "stiJsViewerItemForDragDrop";
    dragElem.innerText = headerButton.caption.innerText;
    dragElem.style.display = "none";
    dragElem.beginingOffset = 0;
    dragElem.posIndex = headerButton.getPosIndex();

    this.controls.mainPanel.appendChild(dragElem);

    dragElem.move = function (event) {
        this.moveInProgress = true;
        this.style.display = "";

        var clientX = event.touches ? event.touches[0].pageX : event.clientX;
        var clientY = event.touches ? event.touches[0].pageY : event.clientY;

        var viewerOffsetX = jsObject.FindPosX(jsObject.controls.mainPanel);
        var viewerOffsetY = jsObject.FindPosY(jsObject.controls.mainPanel);

        clientX -= viewerOffsetX;
        clientY -= viewerOffsetY;

        this.style.left = clientX + "px";
        this.style.top = (clientY + 20) + "px";
    }

    return dragElem;
}
