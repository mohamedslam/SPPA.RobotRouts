
StiJsViewer.prototype.CreateHTMLTable = function (rowsCount, cellsCount) {
    var table = document.createElement("table");
    var jsObject = table.jsObject = this;
    this.clearStyles(table);
    table.cellPadding = 0;
    table.cellSpacing = 0;
    table.tbody = document.createElement("tbody");
    table.appendChild(table.tbody);
    table.tr = [];
    table.tr[0] = document.createElement("tr");
    this.clearStyles(table.tr[0]);
    table.tbody.appendChild(table.tr[0]);

    table.addCell = function (control) {
        var cell = document.createElement("td");
        jsObject.clearStyles(cell);
        this.tr[0].appendChild(cell);
        if (control) cell.appendChild(control);

        return cell;
    }

    table.insertCell = function (index, control) {
        var cell = this.tr[0].insertCell(index);
        jsObject.clearStyles(cell);
        if (control) cell.appendChild(control);

        return cell;
    }

    table.addCellInNextRow = function (control) {
        var rowCount = this.tr.length;
        this.tr[rowCount] = document.createElement("tr");
        jsObject.clearStyles(this.tr[rowCount]);
        this.tbody.appendChild(this.tr[rowCount]);
        var cell = document.createElement("td");
        jsObject.clearStyles(cell);
        this.tr[rowCount].appendChild(cell);
        if (control) cell.appendChild(control);

        return cell;
    }

    table.addCellInLastRow = function (control) {
        var rowCount = this.tr.length;
        var cell = document.createElement("td");
        jsObject.clearStyles(cell);
        this.tr[rowCount - 1].appendChild(cell);
        if (control) cell.appendChild(control);

        return cell;
    }

    table.addTextCellInLastRow = function (text) {
        var rowCount = this.tr.length;
        var cell = document.createElement("td");
        jsObject.clearStyles(cell);
        this.tr[rowCount - 1].appendChild(cell);
        cell.innerText = text;

        return cell;
    }

    table.addTextCellInNextRow = function (text) {
        var rowCount = this.tr.length;
        this.tr[rowCount] = document.createElement("tr");
        jsObject.clearStyles(this.tr[rowCount]);
        this.appendChild(this.tr[rowCount]);
        var cell = document.createElement("td");
        jsObject.clearStyles(cell);
        this.tr[rowCount].appendChild(cell);
        cell.innerText = text;

        return cell;
    }

    table.addCellInRow = function (rowNumber, control) {
        var cell = document.createElement("td");
        jsObject.clearStyles(cell);
        this.tr[rowNumber].appendChild(cell);
        if (control) cell.appendChild(control);

        return cell;
    }

    table.addTextCell = function (text) {
        var cell = document.createElement("td");
        jsObject.clearStyles(cell);
        this.tr[0].appendChild(cell);
        cell.innerText = text;

        return cell;
    }

    table.addRow = function () {
        var rowCount = this.tr.length;
        this.tr[rowCount] = document.createElement("tr");
        jsObject.clearStyles(this.tr[rowCount]);
        this.tbody.appendChild(this.tr[rowCount]);

        return this.tr[rowCount];
    }

    table.insertRow = function (rowIndex) {
        var tr = document.createElement("tr");
        this.tr.splice(rowIndex, 0, tr);
        jsObject.clearStyles(tr);
        this.tbody.insertBefore(tr, this.tbody.childNodes[rowIndex]);
        return tr;
    }

    table.clearRow = function (row) {
        if (typeof row == "undefined") row = 0;
        while (this.tr[row].childNodes[0]) {
            this.tr[row].removeChild(this.tr[row].childNodes[0]);
        }
    }

    table.removeRow = function (row) {
        if (typeof row == "undefined") row = 0;
        this.tbody.removeChild(this.tbody.childNodes[row]);
        this.tr.splice(row, 1);
    }

    table.rowsCount = function () {
        return this.tr.length;
    }

    table.cellsCount = function (row) {
        if (typeof row == "undefined") row = 0;
        return this.tr[row].childNodes.length;
    }

    return table;
}

StiJsViewer.prototype.TextBlock = function (text) {
    var textBlock = document.createElement("div");
    textBlock.style.fontFamily = this.options.toolbar.fontFamily
    textBlock.style.fontSize = "12px";
    textBlock.style.paddingTop = "2px";
    textBlock.innerText = text;

    return textBlock;
}