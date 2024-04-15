
StiJsViewer.prototype.PivotTable = function (width, height, settings) {
    var pivot = document.createElement("div");
    var jsObject = pivot.jsObject = this;
    pivot.style.userSelect = "none";
    pivot.width = width;
    pivot.height = height;
    pivot.settings = settings;

    if (width) pivot.style.width = width + "px";
    if (height) pivot.style.height = height + "px";

    //create styles
    pivot.guid = Math.floor(Math.random() * 10000000);

    if (!jsObject.controls.css) {
        var css = document.getElementById(jsObject.options.viewerId + "Styles");
        if (!css) {
            css = document.createElement("STYLE");
            css.id = jsObject.options.viewerId + "Styles";
            css.setAttribute('type', 'text/css');
            css.setAttribute("stimulsoft", "stimulsoft");
            jsObject.controls.head.appendChild(css);
            jsObject.controls.css = css;
        }
    }

    var sheet = jsObject.controls.css.styleSheet || jsObject.controls.css.sheet || document.styleSheets[0];

    sheet.insertRule(".pivotUnselectable {-webkit-user-select:none; -moz-user-select:none; -ms-user-select:none; user-select:none;}", 0);
    sheet.insertRule(".pivotTable" + pivot.guid + " {text-align:center; font-family:" + settings.fontName + "; font-size:" + settings.fontSize + "pt; line-height:" + (settings.cellHeight - 10) + "px; white-space: nowrap;" +
        "-webkit-user-select:none; -moz-user-select:none; -ms-user-select:none; user-select:none;}", 0);
    sheet.insertRule(".pivotColumnHeader" + pivot.guid + " {color:" + settings.columnHeaderForeColor + "; background-color:" + settings.columnHeaderBackColor + " }", 0);
    sheet.insertRule(".pivotColumnHeader" + pivot.guid + ":hover {background-color:" + settings.hotColumnHeaderBackColor + "}", 0);
    sheet.insertRule(".pivotRowHeader" + pivot.guid + " {color:" + settings.rowHeaderForeColor + "; background-color:" + settings.rowHeaderBackColor + ";box-sizing: content-box; }", 0);
    sheet.insertRule(".pivotRowHeader" + pivot.guid + ":hover {background-color:" + settings.hotRowHeaderBackColor + "}", 0);
    sheet.insertRule(".pivotCell" + pivot.guid + "{text-align: right; border-right: 1px solid " + pivot.settings.lineColor + "}", 0);
    sheet.insertRule(".pivotCellTable" + pivot.guid + "{}", 0);
    sheet.insertRule(".pivotCellTable" + pivot.guid + " tr{}", 0);
    sheet.insertRule(".pivotCellTable" + pivot.guid + " tr:nth-child(even){color:" + settings.alternatingCellForeColor + "; background-color:" + settings.alternatingCellBackColor + " }", 0);
    sheet.insertRule(".pivotCellTable" + pivot.guid + " tr:nth-child(odd){color:" + settings.cellForeColor + "; background-color:" + settings.cellBackColor + " }", 0);
    sheet.insertRule(".pivotCellTable2" + pivot.guid + " tr:nth-child(odd){color:" + settings.alternatingCellForeColor + "; background-color:" + settings.alternatingCellBackColor + "; }", 0);
    sheet.insertRule(".pivotCellTable2" + pivot.guid + " tr:nth-child(even){color:" + settings.cellForeColor + "; background-color:" + settings.cellBackColor + "; }", 0);
    sheet.insertRule(".pivotSelectedCell" + pivot.guid + " {color:" + settings.selectedCellForeColor + "; background-color:" + settings.selectedCellBackColor + "; text-align: right}", 0);

    pivot.sheet = sheet;

    jsObject.AddProgressToControl(pivot);

    pivot.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
    }

    pivot.showData = function (data) {
        pivot.progress.show();
        setTimeout(function () {
            pivot.showDataInt(data)
        }, 0);
    }

    pivot.showDataInt = function (data) {
        //data.cells.splice(14, 1000);
        pivot.data = data;
        if (!data || data.length == 0) return;


        for (var i in data.sizes) {
            var size = data.sizes[i].split(";");
            var minWidth = size[0] > 0 ? "min-width:" + size[0] + "px; " : "";
            var maxWidth = size[1] > 0 ? "max-width:" + size[1] + "px; " : "";
            var wordWrap = size[2] == "True" ? "; word-wrap : anywhere; white-space: normal;" : "";
            pivot.sheet.insertRule(".pivotCell" + pivot.guid + i + " {" + minWidth + maxWidth + wordWrap + ";overflow:hidden}", 0);
        }

        var table = jsObject.CreateHTMLTable();
        this.appendChild(table);
        var td = table.addCell();
        td.style.verticalAlign = "top";
        var leftDiv = document.createElement("div");
        td.appendChild(leftDiv);
        this.cornerDiv = document.createElement("div");
        leftDiv.appendChild(this.cornerDiv);
        this.cornerTable = jsObject.CreateHTMLTable();
        this.cornerTable.className = "pivotTable" + pivot.guid;
        this.cornerTable.style.width = "100%";
        this.cornerDiv.appendChild(this.cornerTable);
        //populate left up corner
        for (var y = 0; y < data.headerHeight; y++) {
            if (y != 0) this.cornerTable.addRow();
            for (var x = 0; x < data.headerWidth; x++) {
                var cell = data.cells[x][y];
                if (cell.guid == cell.parentCellGuid)
                    this.td(cell, this.cornerTable, this.cornerTable.tr.length - 1, cell.colSpan, cell.rowSpan, ["Bottom", "Right"], undefined, "pivotRowHeader" + pivot.guid);
            }
        }

        //populate guids
        this.cellGuid = {};
        this.childrenTable = {};
        for (var y = 0; y < data.cells[0].length; y++) {
            for (var x = 0; x < data.cells.length; x++) {
                if ((x < data.headerWidth || y < data.headerHeight) && data.cells[x][y].guid == data.cells[x][y].parentCellGuid) {
                    var cell = data.cells[x][y];
                    this.cellGuid[cell.guid] = cell;
                }
                if (cell && cell.parentGuid != null && cell.parentGuid != "" && !cell.IsTotal && cell.parentCellGuid == cell.guid) {
                    if (this.childrenTable[cell.parentGuid] == null)
                        this.childrenTable[cell.parentGuid] = [];

                    this.childrenTable[cell.parentGuid].push(cell);
                }
            }
        }

        //left header rows
        this.headerRowsDiv = document.createElement("div");
        leftDiv.appendChild(this.headerRowsDiv);
        leftDiv.style.overflow = "hidden";
        this.headerRowsDiv.style.position = "relative";
        this.headerRowsDiv.style.overflow = "hidden";
        this.headerRowsDiv.style.zoom = "1";
        this.headerRowsTable = jsObject.CreateHTMLTable();
        this.headerRowsTable.className = "pivotTable" + pivot.guid;
        this.headerRowsTable.style.width = "100%";
        this.addWheel(this.headerRowsTable);
        this.headerRowsDiv.appendChild(this.headerRowsTable);
        var y = data.headerHeight;
        this.hasTotalsColumn = false;
        if (data.headerWidth > 0) {
            for (var y = data.headerHeight; y < data.cells[0].length; y++) {
                if (y != data.headerHeight) {
                    this.headerRowsTable.addRow();
                }
                var x = 0;
                while (x < data.headerWidth) {
                    var cell = data.cells[x][y];
                    if ((cell.guid == cell.parentCellGuid) || (data.pivotRowsCount == 0 && cell.parentCellGuid === undefined && pivot.data.pivotSummaryDirection == "LeftToRight")) {
                        this.td3(cell, this.headerRowsTable, ["Bottom", "Right"], "pivotRowHeader" + pivot.guid);
                    }
                    x += cell.colSpan;
                }
            }

            this.headerRowsTable.addRow();
            for (var x = 0; x < data.headerWidth; x++) {
                this.headerRowsTable.addCellInLastRow().style.height = "0px";
            }
        }

        //top header cols
        td = table.addCell();
        td.style.verticalAlign = "top";
        var rightDiv = document.createElement("div");
        td.appendChild(rightDiv);
        this.headerColsDiv = document.createElement("div");
        this.headerColsDiv.style.position = "relative";
        this.headerColsDiv.style.overflow = "hidden";
        this.headerColsDiv.style.zoom = "1";

        rightDiv.appendChild(this.headerColsDiv);
        this.headerColsTable = jsObject.CreateHTMLTable();
        this.headerColsTable.className = "pivotTable" + pivot.guid;
        this.headerColsDiv.appendChild(this.headerColsTable);
        if (jsObject.getNavigatorName() == "Edge" || jsObject.getNavigatorName() == "MSIE") {
            this.headerColsTable.style.tableLayout = "fixed";
        }

        this.openedColumns = [];
        this.openedRows = [];

        for (var y = 0; y < data.headerHeight; y++) {
            if (y != 0) this.headerColsTable.addRow();
            var x = data.headerWidth;
            while (x < data.cells.length) {
                var cell = data.cells[x][y];
                if (cell.guid == cell.parentCellGuid) {
                    this.td3(cell, this.headerColsTable, ["Bottom", "Right"], "pivotRowHeader" + pivot.guid);
                }
                x += cell.colSpan;
            }
        }

        pivot.maxColLevel = data.headerHeight - (data.pivotSummariesCount > 1 && data.pivotSummaryDirection == "LeftToRight" ? 1 : 0) - (data.pivotRowsCount > 0 ? 1 : 0);
        pivot.maxRowLevel = data.headerWidth - (data.pivotSummariesCount > 1 && data.pivotSummaryDirection == "UpToDown" ? 1 : 0);

        //cells
        this.contentDiv = document.createElement("div");
        this.contentDiv.style.position = "relative";
        this.contentDiv.style.overflow = "hidden";
        rightDiv.appendChild(this.contentDiv);
        this.cellsDiv = document.createElement("div");
        this.cellsDiv.style.position = "relative";
        this.cellsDiv.style.overflow = jsObject.options.isTouchDevice ? "auto" : "hidden";
        this.cellsDiv.style.zoom = "1";
        if (jsObject.options.isTouchDevice) this.cellsDiv.className = "stiJsViewerContainerHideScroll";
        this.contentDiv.appendChild(this.cellsDiv);

        this.cellsTable = jsObject.CreateHTMLTable();
        this.cellsDiv.appendChild(this.cellsTable);
        this.cellsTable.className = "pivotTable" + pivot.guid + " pivotCellTable" + pivot.guid;

        //main table
        this.table = jsObject.CreateHTMLTable();
        this.cellsDiv.appendChild(this.table);
        this.table.className = "pivotTable" + pivot.guid + " pivotCellTable" + pivot.guid;
        this.table.style.position = "absolute";
        this.addWheel(this.table);
        this.dataCols = [];
        this.dataRows = [];

        this.scrollHiddenColor = "rgba(191, 191, 191, 0)";
        this.scrollDivColor = "rgba(191, 191, 191, 0.1)";
        this.scrollColor = "rgba(191, 191, 191, 0.7)";
        this.scrollSelectedColor = "rgba(191, 191, 191, 0.9)";

        //horizontal scroll 
        this.cellsHScrollDiv = document.createElement("div");
        this.contentDiv.appendChild(this.cellsHScrollDiv);
        this.cellsHScrollDiv.style.bottom = "0";
        this.cellsHScrollDiv.style.left = "0";
        this.cellsHScrollDiv.style.width = "100%";
        this.cellsHScrollDiv.style.position = "absolute";
        this.cellsHScrollDiv.style.boxSizing = "border-box";
        this.cellsHScrollDiv.style.height = "8px";
        this.cellsHScrollDiv.style.touchAction = "none";
        this.cellsHScrollDiv.style.backgroundColor = this.scrollHiddenColor;
        this.cellsHScrollDiv.style.transition = "background-color 0.5s ease";
        this.cellsHScrollDiv.style.poinerEvents = "none";
        this.cellsHScrollDiv.className = "pivotUnselectable";

        this.cellHScroll = document.createElement("div");
        this.cellsHScrollDiv.appendChild(this.cellHScroll);
        this.cellHScroll.style.height = "100%";
        this.cellHScroll.style.width = "73%";
        this.cellHScroll.style.backgroundColor = this.scrollHiddenColor;
        this.cellHScroll.style.transition = "background-color 0.5s ease";
        this.cellHScroll.className = "pivotUnselectable";
        this.hScrollOffset = 0;
        this.hOffset = 1;

        //vertical scroll 
        this.cellsVScrollDiv = document.createElement("div");
        this.contentDiv.appendChild(this.cellsVScrollDiv);
        this.cellsVScrollDiv.style.top = "0";
        this.cellsVScrollDiv.style.right = "0";
        this.cellsVScrollDiv.style.height = "100%";
        this.cellsVScrollDiv.style.position = "absolute";
        this.cellsVScrollDiv.style.boxSizing = "border-box";
        this.cellsVScrollDiv.style.width = "8px";
        this.cellsVScrollDiv.style.touchAction = "none";
        this.cellsVScrollDiv.style.backgroundColor = this.scrollHiddenColor;
        this.cellsVScrollDiv.style.transition = "background-color 0.5s ease";
        this.cellsVScrollDiv.style.poinerEvents = "none";
        this.cellsVScrollDiv.className = "pivotUnselectable";

        this.cellVScroll = document.createElement("div");
        this.cellsVScrollDiv.appendChild(this.cellVScroll);
        this.cellVScroll.style.width = "100%";
        this.cellVScroll.style.height = "73%";
        this.cellVScroll.style.backgroundColor = this.scrollHiddenColor;
        this.cellVScroll.style.transition = "background-color 0.5s ease";
        this.cellVScroll.className = "pivotUnselectable";
        this.vScrollOffset = 0;
        this.vOffset = 1;

        this.cellsDiv.onscroll = function () {
            pivot.headerColsDiv.scrollLeft = pivot.cellsDiv.scrollLeft;
            pivot.headerRowsDiv.scrollTop = pivot.cellsDiv.scrollTop;
            if (jsObject.options.isTouchDevice) pivot.repaintScrolls();
            pivot.updateData();
            pivot.headerColsDiv.scrollLeft = pivot.cellsDiv.scrollLeft;
            pivot.headerRowsDiv.scrollTop = pivot.cellsDiv.scrollTop;
        }

        jsObject.addEvent(document.body, "mousemove", function (event) {
            if (pivot.screenX) {
                var emptyWidth = (pivot.width - pivot.headerWidth) - (pivot.width - pivot.headerWidth) / pivot.hOffset;
                var offset = Math.max(0, Math.min(emptyWidth, event.screenX - pivot.screenX + pivot.startXOffset));
                pivot.cellHScroll.style.transform = "translateX(" + offset + "px)";
                pivot.cellsDiv.scrollLeft = offset * pivot.hOffset;
                pivot.hScrollOffset = offset;
            }

            if (pivot.screenY) {
                var emptyHeight = (pivot.height - pivot.headerHeight) - (pivot.height - pivot.headerHeight) / pivot.vOffset;
                var offset = Math.max(0, Math.min(emptyHeight, event.screenY - pivot.screenY + pivot.startYOffset));
                pivot.cellVScroll.style.transform = "translateY(" + offset + "px)";
                pivot.cellsDiv.scrollTop = offset * pivot.vOffset;
                pivot.vScrollOffset = offset;
            }
        }, pivot);

        jsObject.addEvent(document.body, "mouseup", function (event) {
            pivot.screenX = null;
            pivot.screenY = null;
            if (!pivot.mouseOver) {
                pivot.cellsHScrollDiv.style.backgroundColor = pivot.scrollHiddenColor;
                pivot.cellHScroll.style.backgroundColor = pivot.scrollHiddenColor;
                pivot.cellsVScrollDiv.style.backgroundColor = pivot.scrollHiddenColor;
                pivot.cellVScroll.style.backgroundColor = pivot.scrollHiddenColor;
            } else {
                if (pivot.hOffset > 1)
                    pivot.cellHScroll.style.backgroundColor = pivot.scrollColor;
                if (pivot.vOffset > 1)
                    pivot.cellVScroll.style.backgroundColor = pivot.scrollColor;
            }
        }, pivot);

        this.cellHScroll.onmousedown = function (event) {
            event.stopPropagation();
            if (pivot.hOffset > 1) {
                pivot.screenX = event.screenX;
                pivot.startXOffset = pivot.hScrollOffset;
                pivot.cellHScroll.style.backgroundColor = pivot.scrollSelectedColor;
            }
        }

        this.cellVScroll.onmousedown = function (event) {
            event.stopPropagation();
            if (pivot.vOffset > 1) {
                pivot.screenY = event.screenY;
                pivot.startYOffset = pivot.vScrollOffset;
                pivot.cellVScroll.style.backgroundColor = pivot.scrollSelectedColor;
            }
        }

        this.cellsHScrollDiv.onmousedown = function (event) {
            var sWidth = (pivot.width - pivot.headerWidth) / pivot.hOffset;
            var emptyWidth = (pivot.width - pivot.headerWidth) - sWidth;

            if (pivot.cellHScroll.getBoundingClientRect().x > event.screenX)
                sWidth *= -1;

            var offset = Math.max(0, Math.min(emptyWidth, sWidth + pivot.hScrollOffset));
            pivot.cellHScroll.style.transform = "translateX(" + offset + "px)";
            pivot.cellsDiv.scrollLeft = offset * pivot.hOffset;
            pivot.hScrollOffset = offset;
        }

        this.cellsVScrollDiv.onmousedown = function (event) {
            var sHeight = (pivot.height - pivot.headerHeight) / pivot.vOffset;
            var emptyHeight = (pivot.height - pivot.headerHeight) - sHeight;

            if (pivot.cellVScroll.getBoundingClientRect().y > event.screenY)
                sHeight *= -1;

            var offset = Math.max(0, Math.min(emptyHeight, sHeight + pivot.vScrollOffset));
            pivot.cellVScroll.style.transform = "translateY(" + offset + "px)";
            pivot.cellsDiv.scrollTop = offset * pivot.vOffset;
            pivot.vScrollOffset = offset;
        }

        var rowHeights = [];
        for (var y = 0; y < pivot.headerColsTable.rows.length; y++) {
            rowHeights.push(pivot.headerColsTable.rows[y].getBoundingClientRect().height);
        }

        for (var y = 0; y < pivot.headerColsTable.rows.length; y++) {
            try {
                pivot.headerColsTable.rows[y].style.height = pivot.cornerTable.rows[y].style.height = rowHeights[y] + 'px';
            } catch (e) { }
        }

        if (pivot.headerRowsTable.rows.length > 0 && pivot.cornerTable.rows.length > 0) {
            var cornerRow = 0;
            var max = 0;
            for (var y = 0; y < pivot.cornerTable.rows.length; y++) {
                var ln = pivot.cornerTable.rows[y].cells.length;
                if (ln > max) {
                    cornerRow = y;
                    max = ln;
                }
            }

            if (pivot.headerRowsTable.rows[0].cells.length > pivot.cornerTable.rows[cornerRow].cells.length) {
                pivot.cornerTable.addCellInRow(cornerRow);
            }

            var colWidths = [];
            for (var x = 0; x < pivot.headerRowsTable.rows[0].cells.length; x++) {
                colWidths.push(Math.max(pivot.headerRowsTable.rows[0].cells[x].getBoundingClientRect().width, pivot.cornerTable.rows[cornerRow].cells[x].getBoundingClientRect().width));
            }

            pivot.setColGroup(pivot.headerRowsTable, colWidths, 0, colWidths.length);
            pivot.setColGroup(pivot.cornerTable, colWidths, 0, colWidths.length);
        }

        //close headers
        for (var y = data.headerHeight - 1; y >= 0; y--) {
            for (var x = data.headerWidth; x < data.cells.length; x++)
                if (data.cells[x][y].dependedCellGuis) {
                    data.cells[x][y].changeTotal(false, false);
                }
        }

        for (var x = data.headerWidth - 1; x >= 0; x--)
            for (var y = data.headerHeight; y < data.cells[0].length; y++) {
                if (data.cells[x][y].dependedCellGuis) {
                    data.cells[x][y].changeTotal(false, false);
                }
            }

        pivot.updateData = function (forceNewTable) {
            var dataCols = [];
            var headersCols = {};
            var dataRows = [];
            var tableOffsetX = pivot.headerColsTable.getBoundingClientRect().width;//   pivot.cellsTable.getBoundingClientRect().width;
            var tableOffsetY = 0;
            var toIndex = pivot.data.pivotRowsCount == 0 || pivot.data.pivotColumnsCount == 0 ? 0 : 1;
            if (pivot.data.pivotSummariesCount > 1 && pivot.data.pivotSummaryDirection == "LeftToRight") {
                toIndex = pivot.headerColsTable.rows.length - 1;
            }
            var hctRect = pivot.headerColsTable.getBoundingClientRect();
            for (var i = pivot.headerColsTable.rows.length - 1; i >= toIndex; i--) {
                var row = pivot.headerColsTable.rows[i];
                if (row.cells.length > 0) {
                    var colIndex = 0;
                    var viewWidth = pivot.cellsDiv.getBoundingClientRect().width + pivot.cellsDiv.scrollLeft;
                    while (colIndex < row.cells.length && (row.cells[colIndex].getBoundingClientRect().left - hctRect.left) < viewWidth) {
                        var cell = row.cells[colIndex];
                        var rect = cell.getBoundingClientRect();
                        var rectLeft = rect.left - hctRect.left;
                        if (rect.width > 0 && rectLeft <= viewWidth && rectLeft + rect.width >= pivot.cellsDiv.scrollLeft && !cell.cell.opened) {
                            tableOffsetX = Math.min(tableOffsetX, rectLeft);
                            var column = cell.cell.pcolumn ? cell.cell.pcolumn : (cell.cell.totalCellGuid ? this.cellGuid[cell.cell.totalCellGuid].column : cell.cell.column);
                            dataCols.push(column);
                            headersCols[column] = cell;
                        }
                        colIndex++;
                    }
                }
            }

            // Rows
            pivot.headerRowsY = 0;
            var rowIndex = 0;
            var height = 0;
            while (rowIndex < pivot.headerRowsTable.rows.length && height < pivot.cellsDiv.scrollTop) {
                if (pivot.headerRowsTable.rows[rowIndex].cells.length > 0) {
                    tableOffsetY = height;
                    pivot.headerRowsY = rowIndex;
                    height += pivot.headerRowsTable.rows[rowIndex].getBoundingClientRect().height;
                }
                rowIndex++;
            }

            rowIndex = Math.max(0, rowIndex - 1);

            var viewHeight = pivot.cellsDiv.scrollTop + pivot.cellsDiv.getBoundingClientRect().height;
            while (rowIndex < pivot.headerRowsTable.rows.length && (height <= viewHeight + 20 || dataRows.length == 0)) {
                if (pivot.headerRowsTable.rows[rowIndex].cells.length > 0) {
                    var cell = null;
                    var colIndex = pivot.headerRowsTable.rows[rowIndex].cells.length - 1;
                    var cellHeight;
                    while (colIndex >= 0 && cell == null) {
                        var tempCell = pivot.headerRowsTable.rows[rowIndex].cells[colIndex];
                        cellHeight = tempCell.getBoundingClientRect().height;
                        if (cellHeight > 0) {
                            cell = tempCell;
                        }
                        colIndex--;
                    }
                    if (cell && cell.cell && cellHeight > 0) {
                        height += cellHeight;
                        dataRows.push(cell.cell.prow ? cell.cell.prow : (cell.cell.totalCellGuid ? this.cellGuid[cell.cell.totalCellGuid].row : cell.cell.row));
                    }
                }
                rowIndex++;
            }

            dataCols = pivot.getUnique(dataCols);
            dataRows = pivot.getUnique(dataRows);

            dataRows.sort(function (a, b) {
                return a - b;
            });

            dataCols.sort(function (a, b) {
                return a - b;
            });

            //make table            
            pivot.table.style.left = tableOffsetX + "px";
            pivot.table.style.top = tableOffsetY + "px";

            if (pivot.dataCols.length == 0 || dataCols[0] > pivot.dataCols[pivot.dataCols.length - 1] || pivot.dataCols[0] > dataCols[dataCols.length - 1] ||
                dataRows[0] > pivot.dataRows[pivot.dataRows.length - 1] || pivot.dataRows[0] > dataRows[dataRows.length - 1] || forceNewTable) {//new table
                pivot.table.tbody.innerHTML = "";

                for (var yy in dataRows) {
                    var y = dataRows[yy];
                    var tr = document.createElement("tr");
                    tr.className = "stiJsViewerClearAllStyles";
                    pivot.table.tbody.appendChild(tr);
                    for (var xx in dataCols) {
                        var x = dataCols[xx];
                        pivot.td2(pivot.data.cells[x][y], -1, -1, pivot.table);
                    }
                }
            } else if (dataRows[0] < pivot.dataRows[0] || dataRows[dataRows.length - 1] < pivot.dataRows[pivot.dataRows.length - 1]) {//scroll up
                var yy = 0;

                while (dataRows[yy] < pivot.dataRows[0]) {
                    var tr = document.createElement("tr");
                    tr.className = "stiJsViewerClearAllStyles";
                    pivot.table.tbody.insertBefore(tr, pivot.table.tbody.childNodes[yy]);
                    for (var xx in dataCols) {
                        pivot.td2(pivot.data.cells[dataCols[xx]][dataRows[yy]], -1, yy, pivot.table);
                    }
                    yy++;
                }
                yy = pivot.dataRows.length - 1;
                while (pivot.dataRows[yy] > dataRows[dataRows.length - 1]) {
                    pivot.table.tbody.removeChild(pivot.table.tbody.lastChild);
                    yy--;
                }
            } else if (dataRows[dataRows.length - 1] > pivot.dataRows[pivot.dataRows.length - 1] || dataRows[0] > pivot.dataRows[0]) {//scroll down
                var yy = 0;

                while (pivot.dataRows[yy] < dataRows[0]) {
                    pivot.table.tbody.removeChild(pivot.table.rows[0]);
                    yy++;
                }
                yy = dataRows.length - 1;
                while (dataRows[yy] > pivot.dataRows[pivot.dataRows.length - 1]) {
                    yy--;
                }
                yy++;
                while (yy < dataRows.length) {
                    var tr = document.createElement("tr");
                    tr.className = "stiJsViewerClearAllStyles";
                    pivot.table.tbody.appendChild(tr);
                    for (var xx in dataCols) {
                        pivot.td2(pivot.data.cells[dataCols[xx]][dataRows[yy]], -1, yy, pivot.table);
                    }
                    yy++;
                }
            } else if (dataCols[0] < pivot.dataCols[0] || dataCols[dataCols.length - 1] < pivot.dataCols[pivot.dataCols.length - 1]) {//scroll left
                var xx = 0;

                while (dataCols[xx] < pivot.dataCols[0]) {
                    for (var yy in dataRows) {
                        pivot.td2(pivot.data.cells[dataCols[xx]][dataRows[yy]], xx, yy, pivot.table);
                    }
                    xx++;
                }
                xx = pivot.dataCols.length - 1;
                while (pivot.dataCols[xx] > dataCols[dataCols.length - 1]) {
                    for (var i = 0; i < pivot.table.rows.length; i++) {
                        var row = pivot.table.rows[i];
                        try {
                            row.removeChild(row.cells[row.cells.length - 1]);
                        } catch (e) { }
                    }
                    xx--;
                }
            } else if (dataCols[dataCols.length - 1] > pivot.dataCols[pivot.dataCols.length - 1] || dataCols[0] > pivot.dataCols[0]) {//scroll right
                var xx = 0;

                while (pivot.dataCols[xx] < dataCols[0]) {
                    for (var i = 0; i < pivot.table.rows.length; i++) {
                        var row = pivot.table.rows[i];
                        try {
                            row.removeChild(row.cells[0]);
                        } catch (e) { }
                    }
                    xx++;
                }
                xx = dataCols.length - 1;
                while (dataCols[xx] > pivot.dataCols[pivot.dataCols.length - 1]) {
                    xx--;
                }
                xx++;
                while (xx < dataCols.length) {
                    var colIndex = pivot.table.rows[0].cells.length;
                    for (var yy in dataRows) {
                        pivot.td2(pivot.data.cells[dataCols[xx]][dataRows[yy]], colIndex, yy, pivot.table);
                    }
                    xx++;
                }
            }

            var oddEvenTable = pivot.headerRowsY % 2 ? "" : "2";
            pivot.table.className = "pivotTable" + pivot.guid + " pivotCellTable" + oddEvenTable + pivot.guid;

            pivot.updateTableSize(dataRows, dataCols, headersCols);

            pivot.dataRows = dataRows;
            pivot.dataCols = dataCols;

            if (pivot.data.onlySummaries && pivot.data.cells.length == 2) {
                for (var i in pivot.data.cells[0]) {
                    var tr = document.createElement("tr");
                    tr.className = "stiJsViewerClearAllStyles";
                    pivot.table.tbody.appendChild(tr);
                    pivot.td2(pivot.data.cells[0][i], 0, i, pivot.table, true);
                    pivot.td2(pivot.data.cells[1][i], 1, i, pivot.table, true);
                }
            }
        }

        pivot.updateTableSize = function (dataRows, dataCols, headersCols) {
            var columnsWidth = pivot.headerColsTable.getBoundingClientRect().width;
            var rowsHeight = pivot.headerRowsTable.getBoundingClientRect().height;

            if (pivot.table.rows.length == 0) {
                return;
            }

            for (var i in dataRows) {
                pivot.table.rows[i].style.height = null;
            }

            for (var i in dataCols) {
                var cell = headersCols[dataCols[i]];
                pivot.table.rows[0].cells[i].style.minWidth = pivot.table.rows[0].cells[i].style.maxWidth = null;
                cell.style.width = cell.style.minWidth = cell.style.maxWidth = null;
            }


            var rowIndex = pivot.headerRowsY;

            for (var i in dataRows) {
                var height = Math.max(pivot.headerRowsTable.rows[rowIndex].getBoundingClientRect().height, pivot.table.rows[i].getBoundingClientRect().height);
                pivot.table.rows[i].style.height = pivot.headerRowsTable.rows[rowIndex].style.height = height + "px";
                rowIndex++;
                while (rowIndex < pivot.headerRowsTable.rows.length && pivot.headerRowsTable.rows[rowIndex].getBoundingClientRect().height == 0)
                    rowIndex++;
            }

            var widths = {};

            for (var i in dataCols) {
                var cell = headersCols[dataCols[i]];
                var offset = cell.cell.column == pivot.data.cells.length - 1 ? 1 : 0;
                var width = (Math.max(cell.getBoundingClientRect().width, pivot.table.rows[0].cells[i].getBoundingClientRect().width) - 7 + offset) + "px";
                widths[i] = width;
                pivot.table.rows[0].cells[i].style.width = pivot.table.rows[0].cells[i].style.minWidth = pivot.table.rows[0].cells[i].style.maxWidth = width;
                cell.style.width = cell.style.minWidth = cell.style.maxWidth = width;
            }

            for (var i in dataCols) {
                var cell = headersCols[dataCols[i]];
                pivot.table.rows[0].cells[i].style.width = pivot.table.rows[0].cells[i].style.minWidth = pivot.table.rows[0].cells[i].style.maxWidth = widths[i];
                cell.style.width = cell.style.minWidth = cell.style.maxWidth = widths[i];
            }

            if (columnsWidth != pivot.headerColsTable.getBoundingClientRect().width || rowsHeight != pivot.headerRowsTable.getBoundingClientRect().height) {
                pivot.updateSizes();
            }
        }

        pivot.getUnique = function (array) {
            var u = {}, a = [];
            for (var i = 0, l = array.length; i < l; ++i) {
                // eslint-disable-next-line no-prototype-builtins
                if (u.hasOwnProperty(array[i])) {
                    continue;
                }
                a.push(array[i]);
                u[array[i]] = 1;
            }
            return a;
        }

        pivot.showScrolls = function () {
            if (pivot.hOffset > 1) {
                pivot.cellsHScrollDiv.style.backgroundColor = pivot.scrollDivColor;
                pivot.cellHScroll.style.backgroundColor = pivot.scrollColor;
            }
            if (pivot.vOffset > 1) {
                pivot.cellsVScrollDiv.style.backgroundColor = pivot.scrollDivColor;
                pivot.cellVScroll.style.backgroundColor = pivot.scrollColor;
            }
        }

        pivot.hideScrolls = function () {
            if (!pivot.screenX) {
                pivot.cellsHScrollDiv.style.backgroundColor = pivot.scrollHiddenColor;
                pivot.cellHScroll.style.backgroundColor = pivot.scrollHiddenColor;
            }
            if (!pivot.screenY) {
                pivot.cellsVScrollDiv.style.backgroundColor = pivot.scrollHiddenColor;
                pivot.cellVScroll.style.backgroundColor = pivot.scrollHiddenColor;
            }
        }

        this.contentDiv.onmouseover = function () {
            if (jsObject.options.isTouchDevice) return;
            pivot.mouseOver = true;
            pivot.showScrolls();
        }

        this.contentDiv.onmouseout = function () {
            if (jsObject.options.isTouchDevice) return;
            pivot.mouseOver = false;
            pivot.hideScrolls();
        }

        pivot.repaintScrolls = function () {
            var offset = this.cellsDiv.scrollLeft / this.hOffset;
            pivot.cellHScroll.style.transform = "translateX(" + offset + "px)";
            this.hScrollOffset = offset;

            offset = this.cellsDiv.scrollTop / this.vOffset;
            pivot.cellVScroll.style.transform = "translateY(" + offset + "px)";
            this.vScrollOffset = offset;
        }

        //process opened headers
        for (var x = 0; x < data.cells.length; x++) {//cols
            for (var y = 0; y < data.headerHeight; y++) {
                var cell = data.cells[x][y];
                if (cell.guid == cell.parentCellGuid && cell.o == true) {
                    cell.changeTotal(true, false);
                }
            }
        }

        for (var y = data.headerHeight; y < data.cells[0].length; y++) {//rows
            for (var x = 0; x < data.headerWidth; x++) {
                var cell = data.cells[x][y];
                if (cell.guid == cell.parentCellGuid && cell.o == true) {
                    cell.changeTotal(true, false);
                }
            }
        }

        //pivot.style.visibility = "hidden";  
        setTimeout(function () {
            pivot.updateSizes();
            pivot.updateData();
            pivot.style.visibility = "";
            if (jsObject.options.isTouchDevice) pivot.showScrolls();
        }, 100);

    }

    pivot.action = function () {

    }

    pivot.addWheel = function (elem) {
        pivot.onWheel = function (e) {
            e = e || window.event;
            var delta = e.wheelDelta || e.deltaY || e.detail;
            if (e.wheelDelta == null && e.deltaY != null) delta *= -40; //fix for firefox
            var info = document.getElementById('delta');
            e.preventDefault ? e.preventDefault() : (e.returnValue = false);
            var sHeight = (pivot.height - pivot.headerHeight) / pivot.vOffset;
            var emptyHeight = (pivot.height - pivot.headerHeight) - sHeight;
            var offset = Math.max(0, Math.min(emptyHeight, -delta / 5 + pivot.vScrollOffset));
            pivot.cellVScroll.style.transform = "translateY(" + offset + "px)";
            pivot.cellsDiv.scrollTop = offset * pivot.vOffset;
            pivot.vScrollOffset = offset;
        }

        if ('onwheel' in document) {
            jsObject.addEvent(elem, "wheel", pivot.onWheel, pivot);
        } else if ('onmousewheel' in document) {
            jsObject.addEvent(elem, "mousewheel", pivot.onWheel, pivot);
        } else {
            jsObject.addEvent(elem, "MozMousePixelScroll", pivot.onWheel, pivot);
        }
    }

    pivot.selectCell = function (cell) {
        if (pivot.selectedCell && pivot.selectedCell.td) {
            pivot.selectedCell.td.className = "pivotCell" + pivot.guid;;
            pivot.selectedCell = null;
        }
        if (cell) {
            pivot.selectedCell = cell;
            cell.td.className = "pivotSelectedCell" + pivot.guid;
        } else
            pivot.selectedCell = null;
    }

    pivot.updateSizes = function () {
        //reset sizes
        this.cornerTable.style.height = this.headerColsTable.style.height = null;

        pivot.headerWidth = this.cornerTable.getBoundingClientRect().width;
        pivot.headerHeight = Math.max(this.cornerTable.getBoundingClientRect().height, this.headerColsTable.getBoundingClientRect().height);

        pivot.cellsTable.style.width = pivot.headerColsTable.getBoundingClientRect().width + "px";
        pivot.cellsTable.style.height = pivot.headerRowsTable.getBoundingClientRect().height + "px";

        this.cornerTable.style.height = pivot.headerHeight + "px";
        this.headerColsTable.style.height = pivot.headerHeight + "px";

        var contentHeight = this.height - pivot.headerHeight;
        var contentWidth = this.width - pivot.headerWidth;

        this.cellsDiv.style.height = this.contentDiv.style.height = contentHeight + "px";
        this.cellsDiv.style.width = this.contentDiv.style.width = contentWidth + "px";
        this.headerColsDiv.style.width = contentWidth + "px";
        this.headerRowsDiv.style.height = contentHeight + "px";

        var offsetWidth = this.headerColsTable.getBoundingClientRect().width;
        var offsetHeight = this.headerRowsTable.getBoundingClientRect().height;

        this.hOffset = offsetWidth / contentWidth;
        this.cellHScroll.style.width = ((contentWidth / offsetWidth) * contentWidth) + "px";

        this.vOffset = offsetHeight / contentHeight;
        this.cellVScroll.style.height = ((contentHeight / offsetHeight) * contentHeight) + "px";

        var offset = this.cellsDiv.scrollLeft / this.hOffset;
        pivot.cellHScroll.style.transform = "translateX(" + offset + "px)";
        this.hScrollOffset = offset;

        offset = this.cellsDiv.scrollTop / this.vOffset;
        pivot.cellVScroll.style.transform = "translateY(" + offset + "px)";
        this.vScrollOffset = offset;

        pivot.progress.hide();
    }

    pivot.setupArrow = function (cell) {
        var tds = [cell.htd, cell.td];
        var data = this.data;
        var pivot = this;
        var color = cell.isColumn ? pivot.settings.columnHeaderForeColor : pivot.settings.rowHeaderForeColor;
        cell.opened = true;
        for (var i in [0]) {
            var ch = tds[i];
            if (!ch) continue;
            ch.innerHTML = "";
            var table = jsObject.CreateHTMLTable();
            table.style.width = "100%";
            var trClosed = document.createElement("div");
            trClosed.style.width = "0";
            trClosed.style.height = "0";
            trClosed.style.borderStyle = "solid";
            trClosed.style.borderWidth = "4px 0 4px 6px";
            trClosed.style.borderColor = "transparent transparent transparent " + color;
            trClosed.style.display = cell.opened ? "none" : "";
            trClosed.style.marginRight = "3px";

            var trOpened = document.createElement("div");
            trOpened.style.width = "0";
            trOpened.style.height = "0";
            trOpened.style.borderStyle = "solid";
            trOpened.style.borderWidth = "0 0 6px 6px";
            trOpened.style.borderColor = "transparent transparent " + color + " transparent";
            trOpened.style.marginRight = "3px";
            trOpened.style.display = !cell.opened ? "none" : "";

            table.addCell(trClosed).style.width = "6px";
            table.addCell(trOpened).style.width = "6px";
            table.addTextCell(ch.cell.text);

            ch.trOpened = trOpened;
            ch.trClosed = trClosed;

            ch.appendChild(table);

            if (i == 0) {
                ch.style.cursor = "pointer";
                ch.onclick = function (e) {
                    var cell = e.currentTarget.cell;
                    pivot.progress.show();
                    setTimeout(function () {
                        cell.changeTotal(!cell.opened, true);
                        pivot.progress.hide();
                    }, 0);
                }
            }

            ch.setOpened = function (opened) {
                this.trOpened.style.display = opened ? "" : "none";
                this.trClosed.style.display = !opened ? "" : "none";
            }
        }
    }

    pivot.td2 = function (cell, colIndex, rowIndex, table, disableSelection) {
        var td = document.createElement("td");
        this.jsObject.clearStyles(td);;
        if (colIndex > -1) {
            table.rows[rowIndex].insertBefore(td, table.rows[rowIndex].cells[colIndex]);
        } else if (rowIndex > -1) {
            table.rows[rowIndex].appendChild(td);
        } else {
            table.tbody.rows[table.tbody.rows.length - 1].appendChild(td);
        }

        for (var st in cell.style)
            td.style[st] = cell.style[st];

        td.className = "pivotCell" + pivot.guid + " pivotCell" + pivot.guid + cell.s;
        td.innerHTML = cell.text;

        td.cell = cell;
        cell.td = td;
        td.style.padding = "3px";
        cell.opened = false;

        if (pivot.data.cells.length == 0 || cell.row == pivot.data.cells[0].length - 2) {
            td.style.borderBottom = "1px solid " + pivot.settings.lineColor;
        }

        if (cell.column == pivot.data.cells.length - 1) {
            td.style.borderRight = "none";
        }

        if (!disableSelection) {
            td.onmousedown = function (event) {
                pivot.selectCell(this.cell);
            }
        }

        return td;
    }

    pivot.td = function (cell, table, rowIndex, colSpan, rowSpan, borders, tds, className) {
        var td;
        if (!isNaN(rowIndex)) {
            td = table.addCellInRow(rowIndex);
        } else {
            td = document.createElement("td");
            this.jsObject.clearStyles(td);
            rowIndex.parentNode.insertBefore(td, rowIndex);
        }

        for (var st in cell.style)
            td.style[st] = cell.style[st];

        td.className = className + " pivotCell" + pivot.guid + cell.s;
        td.style.padding = "3px";
        td.innerHTML = cell.text;
        td.colSpan = colSpan;
        td.rowSpan = rowSpan;
        for (var i in borders)
            if (!(borders[i] == "Right" && cell.column == pivot.data.cells.length - 1))
                td.style["border" + borders[i]] = "1px solid " + pivot.settings.lineColor;

        if (pivot.data.cells.length > 0 && cell.row == pivot.data.cells[0].length - 1) {
            td.style.borderBottom = "none";
        }
        //td.style.color = "white";
        td.cell = cell;
        cell[tds] = td;
        cell.opened = false;

        if (cell.row >= pivot.data.headerHeight && cell.column >= pivot.data.headerWidth && tds == "td") {
            td.onmousedown = function (event) {
                pivot.selectCell(this.cell);
            }
        }

        return td;
    }

    pivot.td3 = function (cell, table, borders, className) {
        var td = table.addCellInRow(table.tr.length - 1);

        for (var st in cell.style)
            td.style[st] = cell.style[st];

        td.className = className + " pivotCell" + pivot.guid + cell.s;
        td.style.padding = "3px";
        td.innerHTML = /*"col:" + cell.column + " row:" + cell.row + "[" + */ cell.text;// + "]";
        td.colSpan = cell.colSpan;
        td.rowSpan = cell.rowSpan;
        for (var i in borders)
            if (!(borders[i] == "Right" && cell.column == pivot.data.cells.length - 1))
                td.style["border" + borders[i]] = "1px solid " + pivot.settings.lineColor;

        if (pivot.data.cells.length > 0 && cell.row == pivot.data.cells[0].length - 1) {
            td.style.borderBottom = "none";
        }
        td.cell = cell;
        cell["htd"] = td;
        cell.opened = cell.dependedCellGuis ? true : false;

        cell.changeTotal = function (open, updateData) {
            if (!open && !this.totalCellGuid) {
                return;
            }

            this.opened = open;
            this.htd.setOpened(open);

            if (!open) {
                pivot.hideChildren(this);
            }

            var span = 0;

            if (open) {
                for (var g in this.dependedCellGuis) {
                    var dCell = pivot.cellGuid[this.dependedCellGuis[g]];

                    pivot.openSummCells(dCell);

                    if (dCell.dependedCellGuis) {
                        dCell.changeTotal(dCell.opened);
                    }

                    span += this.isColumn ? dCell.htd.colSpan : dCell.htd.rowSpan;
                }
                if (this.totalCellGuid) {
                    pivot.openSummCells(pivot.cellGuid[this.totalCellGuid]);
                }
            }


            if (this.isColumn) {
                var summCount = pivot.data.pivotSummariesCount > 1 && pivot.data.pivotSummaryDirection == "LeftToRight" ? pivot.data.pivotSummariesCount : 1;
                var newSpan = open ? span + (this.totalCellGuid ? summCount : 0) : summCount;

                pivot.updateSpan(this, "colSpan", newSpan - this.htd.colSpan);
                this.htd.rowSpan = open ? 1 : pivot.maxColLevel - this.l;
            } else {
                var summCount = pivot.data.pivotSummariesCount > 1 && pivot.data.pivotSummaryDirection == "UpToDown" ? pivot.data.pivotSummariesCount : 1;
                var newSpan = open ? span + (this.totalCellGuid ? summCount : 0) : summCount;

                pivot.updateSpan(this, "rowSpan", newSpan - this.htd.rowSpan);
                this.htd.colSpan = open ? 1 : pivot.maxRowLevel - this.l;
            }

            pivot.updateSizes();
            if (updateData && pivot.updateData) {
                pivot.updateData(true);
            }
        }

        cell.setVisible = function (visible) {
            cell.htd.style.display = visible ? "" : "none";

            if (!cell.isColumn) {
                var rowDisplayNone = true;
                var tds = cell.htd.parentNode.childNodes;
                for (var i = 0; i < tds.length; i++) {
                    if (tds[i].style && tds[i].style.display != 'none') {
                        rowDisplayNone = false;
                    }
                }
                cell.htd.parentNode.style.display = rowDisplayNone ? 'none' : '';
            }
        }

        if (cell.dependedCellGuis) {
            pivot.setupArrow(cell);
        }

        return td;
    }

    pivot.openSummCells = function (cell) {
        cell.setVisible(true);
        if (cell.isColumn) {
            if (pivot.data.pivotSummariesCount > 1 && pivot.data.pivotSummaryDirection == "LeftToRight") {
                for (var x = cell.column; x < cell.column + pivot.data.pivotSummariesCount; x++) {
                    var cl = pivot.data.cells[x][pivot.data.headerHeight - 1];
                    if (cl.setVisible) {
                        cl.setVisible(true);
                    }
                    cl.pcolumn = null;
                }
            }
        } else {
            if (pivot.data.pivotSummariesCount > 1 && pivot.data.pivotSummaryDirection == "UpToDown") {
                for (var y = cell.row; y < cell.row + pivot.data.pivotSummariesCount; y++) {
                    var cl = pivot.data.cells[pivot.data.headerWidth - 1][y];
                    if (cl.setVisible) {
                        cl.setVisible(true);
                    }
                    cl.prow = null;
                }
            }
        }
    }

    pivot.updateSpan = function (cell, span, value) {
        cell.htd[span] = cell.htd[span] + value;
        var parent = pivot.cellGuid[cell.parentGuid];
        if (parent) {
            pivot.updateSpan(parent, span, value);
        }
    }

    pivot.hideChildren = function (cell) {
        if (cell.isColumn) {
            var sums = pivot.data.pivotSummariesCount > 1 && pivot.data.pivotSummaryDirection == "LeftToRight" ? pivot.data.pivotSummariesCount : 0;
            for (var x = cell.column; x < cell.column + cell.colSpan; x++) {
                for (var y = cell.row + 1; y < pivot.data.headerHeight; y++) {
                    if (pivot.data.cells[x][y].setVisible && !(x < cell.column + sums && y == pivot.data.headerHeight - 1)) {
                        pivot.data.cells[x][y].setVisible(false);
                    }
                    if (sums > 1 && x < cell.column + sums && y == pivot.data.headerHeight - 1) {
                        pivot.data.cells[x][y].pcolumn = pivot.cellGuid[cell.totalCellGuid].column + (x - cell.column);
                    }
                }
            }
        } else {
            var sums = pivot.data.pivotSummariesCount > 1 && pivot.data.pivotSummaryDirection == "UpToDown" ? pivot.data.pivotSummariesCount : 0;
            for (var y = cell.row; y < cell.row + cell.rowSpan; y++) {
                for (var x = cell.column + 1; x < pivot.data.headerWidth; x++) {
                    if (pivot.data.cells[x][y].setVisible && !(y < cell.row + sums && x == pivot.data.headerWidth - 1)) {
                        pivot.data.cells[x][y].setVisible(false);
                    }
                    if (sums > 1 && y < cell.row + sums && x == pivot.data.headerWidth - 1) {
                        pivot.data.cells[x][y].prow = pivot.cellGuid[cell.totalCellGuid].row + (y - cell.row);
                    }
                }
            }
        }
    }

    pivot.setColGroup = function (table, widths, start, end) {
        var colgroup = document.createElement("colgroup");
        for (var i = start; i < end; i++) {
            var col = document.createElement("col");
            col.style.width = col.style.minWidth = col.style.maxWidth = widths[i] != 0 ? widths[i] + "px" : "auto";
            colgroup.appendChild(col);
        }
        table.insertBefore(colgroup, table.firstChild);
    }

    return pivot;
}