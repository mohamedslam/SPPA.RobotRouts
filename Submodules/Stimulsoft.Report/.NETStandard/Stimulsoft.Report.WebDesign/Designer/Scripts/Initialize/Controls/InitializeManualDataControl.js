
StiMobileDesigner.prototype.InitializeManualDataControl = function (width, height, dataColumns, countRows) {
    var jsObject = this;
    var control = document.createElement("div");
    control.style.overflow = "auto";
    control.countRows = countRows || 10;
    control.style.width = (width || 300) + "px";
    control.style.height = (height || 300) + "px";

    var rowsHeight = 22;
    var leftCellWidth = 60;
    var cellWidth = ((width || 300) - leftCellWidth) / dataColumns.length;

    var mainTable = control.mainTable = this.CreateHTMLTable();
    control.appendChild(mainTable);
    mainTable.style.borderCollapse = "collapse";
    mainTable.style.display = "inline-block";

    var contextMenu = control.contextMenu = this.BaseContextMenu("manualDataContextMenu", "Up", this.GetManualDataContextMenuItems());

    contextMenu.action = function (menuItem) {
        this.changeVisibleState(false);
        var textBox = this.currentCell.textBox;
        var rowIndex = textBox.rowIndex;
        var columnIndex = textBox.columnIndex;

        switch (menuItem.key) {
            case "InsertRowAbove": {
                control.insertRow(rowIndex, columnIndex);
                break;
            }
            case "InsertRowBelow": {
                control.insertRow(rowIndex + 1, columnIndex);
                break;
            }

            case "MoveUp": {
                control.moveRow(rowIndex, "up", columnIndex);
                break;
            }

            case "MoveDown": {
                control.moveRow(rowIndex, "down", columnIndex);
                break;
            }

            case "DeleteRow": {
                control.removeRow(rowIndex);
                control.action();
                break;
            }
        }
    }
    contextMenu.updateItemsStates = function () {
        var textBox = this.currentCell.textBox;
        var rowIndex = textBox.rowIndex;

        contextMenu.items.MoveDown.setEnabled(rowIndex < control.countRows);
        contextMenu.items.MoveUp.setEnabled(rowIndex > 1);
        contextMenu.items.DeleteRow.setEnabled(control.countRows > 1);
    }

    control.moveRow = function (rowIndex, direction, columnIndex) {
        if (rowIndex < mainTable.tr.length) {
            var row = mainTable.tr[rowIndex];
            var toRow = direction == "down" ? (rowIndex + 1 < mainTable.tr.length ? mainTable.tr[rowIndex + 1] : null) : (rowIndex - 1 > 0 ? mainTable.tr[rowIndex - 1] : null)

            if (row && toRow) {
                for (var k = 1; k < row.childNodes.length; k++) {
                    var valueFrom = row.childNodes[k].textBox.value;
                    var valueTo = toRow.childNodes[k].textBox.value;
                    row.childNodes[k].textBox.value = valueTo;
                    toRow.childNodes[k].textBox.value = valueFrom;
                }
            }
        }

        control.action();
        var selectedCell = control.getCellByIndex(direction == "down" ? rowIndex + 1 : rowIndex - 1, columnIndex);
        if (selectedCell) control.selectCell(selectedCell);
    }

    control.insertRow = function (rowIndex, columnIndex) {
        control.countRows++;
        control.addRow(rowIndex);
        control.updateColumnsVisibility();
        control.updateRowIndexes();
        control.action();
        var selectedCell = control.getCellByIndex(rowIndex, columnIndex);
        if (selectedCell) control.selectCell(selectedCell);
    }

    control.removeRow = function (rowIndex) {
        if (control.countRows == 1)
            return;

        mainTable.removeRow(rowIndex);
        control.countRows--;
        control.updateRowIndexes();

        for (var i = 1; i <= control.countRows; i++) {
            var row = mainTable.tr[i];
            for (var k = 1; k < row.childNodes.length; k++) {
                if (row.childNodes[k].isSelected) {
                    control.removeRow(i);
                    return;
                };
            }
        }

        var selectedCell = control.getCellByIndex(rowIndex < control.countRows ? rowIndex : control.countRows, 1);
        if (selectedCell) control.selectCell(selectedCell);
    }

    control.addRow = function (rowIndex) {
        mainTable.insertRow(rowIndex);

        var leftCell = mainTable.addTextCellInRow(rowIndex, rowIndex)
        leftCell.className = "stiManualControlCell stiManualControlHeaderCell";
        leftCell.style.width = leftCellWidth + "px";
        leftCell.rowIndex = rowIndex;

        leftCell.onmousedown = function (event) {
            control.selectRow(this.rowIndex);
        }

        leftCell.onmouseup = function (event) {
            if (event.button == 2) {
                event.stopPropagation();
                contextMenu.currentCell = control.getCellByIndex(this.rowIndex, 1);
                contextMenu.updateItemsStates();
                var point = jsObject.FindMousePosOnMainPanel(event);
                contextMenu.show(point.xPixels + 10, point.yPixels + 20, "Down", "Right");
            }
            return false;
        }

        for (var k = 0; k < dataColumns.length; k++) {
            var textBox = jsObject.TextBox(null, cellWidth, rowsHeight);
            textBox.style.border = "0px";
            textBox.style.textAlign = "right";
            textBox.style.marginRight = "4px";
            textBox.rowIndex = rowIndex;
            textBox.columnIndex = k + 1;
            textBox.dataColumn = dataColumns[k][0];
            textBox.style.background = "transparent";

            var cell = mainTable.addCellInRow(rowIndex, textBox);
            cell.className = "stiManualControlCell";
            cell.style.width = cellWidth + "px";
            cell.style.height = rowsHeight + "px";
            cell.textBox = textBox;

            if (this.visibilityParams && this.visibilityParams[dataColumns[k][0]] && !this.visibilityParams[dataColumns[k][0]].visible) {
                cell.style.display = "none";
            }

            textBox.action = function () {
                if (this.rowIndex == control.countRows) {
                    control.countRows++;
                    control.addRow(control.countRows);
                }
                control.action();
            }

            textBox.actionOnKeyEnter = function () {
                var nextCell = control.getCellByIndex(this.rowIndex + 1, this.columnIndex);
                if (nextCell) {
                    control.selectCell(nextCell);
                    nextCell.textBox.focus();
                }
            }

            textBox.onkeypress = function (event) {
                if (this.readOnly) return false;
                if (event && event.keyCode == 13) {
                    this.keyEnterPressed = true;
                    this.onblur();
                    this.actionOnKeyEnter();
                    return false;
                }
            }

            cell.setSelected = function (state) {
                this.className = state ? "stiManualControlCell stiManualControlCellSelected" : "stiManualControlCell";
                this.isSelected = state;
            }

            cell.onmousedown = function (event) {
                control.currentCell = this;

                if (jsObject.options.CTRL_pressed) {
                    this.setSelected(!this.isSelected);
                }
                else if (!(event.button == 2 && this.isSelected)) {
                    control.selectCell(this);
                }
            }

            cell.onmouseup = function (event) {
                if (event.button == 2) {
                    event.stopPropagation();
                    contextMenu.currentCell = this;
                    contextMenu.updateItemsStates();
                    var point = jsObject.FindMousePosOnMainPanel(event);
                    contextMenu.show(point.xPixels + 10, point.yPixels + 20, "Down", "Right");
                }
                return false;
            }
        }
    }

    control.selectCell = function (cell) {
        for (var i = 1; i <= control.countRows; i++) {
            var row = mainTable.tr[i];
            for (var k = 1; k < row.childNodes.length; k++) {
                row.childNodes[k].setSelected(row.childNodes[k] == cell);
            }
        }
    }

    control.selectAllCells = function () {
        for (var i = 1; i <= control.countRows; i++) {
            var row = mainTable.tr[i];
            for (var k = 1; k < mainTable.tr[i].childNodes.length; k++) {
                row.childNodes[k].setSelected(true);
            }
        }
    }

    control.selectRow = function (rowIndex) {
        for (var i = 1; i <= control.countRows; i++) {
            var row = mainTable.tr[i];
            for (var k = 1; k < row.childNodes.length; k++) {
                row.childNodes[k].setSelected(i == rowIndex);
            }
        }
    }

    control.selectColumn = function (columnIndex) {
        for (var i = 1; i <= control.countRows; i++) {
            var row = mainTable.tr[i];
            for (var k = 1; k < row.childNodes.length; k++) {
                row.childNodes[k].setSelected(k == columnIndex);
            }
        }
    }

    control.buildRows = function () {
        for (var i = 0; i <= control.countRows; i++) {
            if (i == 0) {
                var cornerCell = mainTable.addCell();
                cornerCell.className = "stiManualControlCell stiManualControlHeaderCell";

                cornerCell.onmousedown = function () {
                    control.selectAllCells();
                }

                for (var k = 0; k < dataColumns.length; k++) {
                    var headerCell = mainTable.addTextCell(dataColumns[k][1]);
                    headerCell.style.height = rowsHeight + "px";
                    headerCell.className = "stiManualControlCell stiManualControlHeaderCell";
                    headerCell.columnIndex = k + 1;

                    headerCell.onmousedown = function () {
                        control.selectColumn(this.columnIndex);
                    }
                }
                continue;
            }
            control.addRow(i);
        }
    }

    control.getCellByIndex = function (rowIndex, columnIndex) {
        if (rowIndex < mainTable.tr.length && columnIndex <= dataColumns.length) {
            var row = mainTable.tr[rowIndex];
            return (columnIndex < row.childNodes.length ? mainTable.tr[rowIndex].childNodes[columnIndex] : null);
        }
        return null;
    }

    control.updateRowIndexes = function () {
        for (var i = 1; i <= control.countRows; i++) {
            var row = mainTable.tr[i];
            row.childNodes[0].innerText = i;
            row.childNodes[0].rowIndex = i;

            for (var k = 1; k < row.childNodes.length; k++) {
                row.childNodes[k].textBox.rowIndex = i;
            }
        }
    }

    control.updateColumnsVisibility = function (params) {
        if (!params)
            params = this.visibilityParams;

        if (params) {
            var columnsCount = 0;
            for (var i = 0; i < dataColumns.length; i++) {
                if (params[dataColumns[i][0]] && params[dataColumns[i][0]].visible) {
                    columnsCount++;
                }
            }
            var cellsWidth = ((width || 300) - leftCellWidth) / columnsCount;

            for (var i = 1; i <= control.countRows; i++) {
                var row = mainTable.tr[i];

                for (var k = 1; k < row.childNodes.length; k++) {
                    var headerCell = mainTable.tr[0].childNodes[k];
                    var cell = row.childNodes[k];
                    var dataColumn = cell.textBox.dataColumn;
                    cell.textBox.style.width = cellsWidth + "px";
                    headerCell.style.display = cell.style.display = params[dataColumn] && params[dataColumn].visible ? "" : "none";
                    if (params[dataColumn]) headerCell.innerHTML = params[dataColumn].caption;
                }
            }

            this.visibilityParams = params;
        }
    }

    control.clear = function () {
        mainTable.clear();
        control.countRows = countRows || 10;
        control.buildRows();
    }

    control.getValue = function () {
        var result = [];
        var columns = [];

        for (var k = 0; k < dataColumns.length; k++) {
            columns.push(dataColumns[k][0]);
        }

        result.push(columns);

        for (var i = 1; i <= control.countRows; i++) {
            var row = mainTable.tr[i];
            var emptyRow = true;
            var rowValues = [];

            for (var k = 1; k < row.childNodes.length; k++) {
                var textBox = row.childNodes[k].textBox;
                if (textBox.value != "") emptyRow = false;
                rowValues.push(textBox.value);
            }

            if (!emptyRow) result.push(rowValues);
        }

        if (result.length == 1)
            return null;

        return result;
    }

    control.setValue = function (value) {
        this.clear();

        if (value) {
            var valArray = JSON.parse(value);
            if (valArray.length > 1) {
                for (var i = 1; i < valArray.length; i++) {
                    if (i >= mainTable.tr.length) {
                        control.countRows++;
                        control.addRow(control.countRows);
                    }
                    var row = mainTable.tr[i];
                    var rowValues = valArray[i];

                    for (var k = 1; k < row.childNodes.length; k++) {
                        var textBox = row.childNodes[k].textBox;
                        textBox.value = k - 1 < rowValues.length ? rowValues[k - 1] : "";
                    }
                }
            }
        }
    }

    this.addEvent(control, 'keyup', function (e) {
        if (e.keyCode == 46) {
            for (var i = 1; i <= control.countRows; i++) {
                var row = mainTable.tr[i];
                for (var k = 1; k < mainTable.tr[i].childNodes.length; k++) {
                    if (row.childNodes[k].isSelected && row.childNodes[k].textBox != jsObject.options.controlsIsFocused) {
                        row.childNodes[k].textBox.value = "";
                    }
                }
            }
        }
    });

    control.oncontextmenu = function () {
        return false;
    }

    control.action = function () { }

    control.buildRows();

    return control;
}