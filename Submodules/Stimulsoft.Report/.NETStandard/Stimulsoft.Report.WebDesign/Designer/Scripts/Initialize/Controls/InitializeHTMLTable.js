
StiMobileDesigner.prototype.CreateHTMLTable = function (rowsCount, cellsCount) {
    var table = document.createElement("table");
    var jsObject = table.jsObject = this;
    this.ClearStyles(table);
    table.cellPadding = 0;
    table.cellSpacing = 0;
    table.tr = [];
    table.tr[0] = document.createElement("tr");
    this.ClearStyles(table.tr[0]);
    table.appendChild(table.tr[0]);

    table.addCell = function (control) {
        var cell = document.createElement("td");
        jsObject.ClearStyles(cell);
        this.tr[0].appendChild(cell);
        if (control) cell.appendChild(control);

        return cell;
    }

    table.insertCell = function (index, control) {
        var cell = this.tr[0].insertCell(index);
        jsObject.ClearStyles(cell);
        if (control) cell.appendChild(control);

        return cell;
    }

    table.addTextCellInNextRow = function (text) {
        var rowCount = this.tr.length;
        this.tr[rowCount] = document.createElement("tr");
        jsObject.ClearStyles(this.tr[rowCount]);
        this.appendChild(this.tr[rowCount]);
        var cell = document.createElement("td");
        jsObject.ClearStyles(cell);
        this.tr[rowCount].appendChild(cell);
        cell.innerHTML = text;

        return cell;
    }

    table.addCellInNextRow = function (control) {
        var rowCount = this.tr.length;
        this.tr[rowCount] = document.createElement("tr");
        jsObject.ClearStyles(this.tr[rowCount]);
        this.appendChild(this.tr[rowCount]);
        var cell = document.createElement("td");
        jsObject.ClearStyles(cell);
        this.tr[rowCount].appendChild(cell);
        if (control) cell.appendChild(control);

        return cell;
    }

    table.addCellInLastRow = function (control) {
        var rowCount = this.tr.length;
        var cell = document.createElement("td");
        jsObject.ClearStyles(cell);
        this.tr[rowCount - 1].appendChild(cell);
        if (control) cell.appendChild(control);

        return cell;
    }

    table.addTextCellInLastRow = function (text) {
        var rowCount = this.tr.length;
        var cell = document.createElement("td");
        jsObject.ClearStyles(cell);
        this.tr[rowCount - 1].appendChild(cell);
        cell.innerHTML = text;

        return cell;
    }

    table.addCellInRow = function (rowNumber, control) {
        var cell = document.createElement("td");
        jsObject.ClearStyles(cell);
        this.tr[rowNumber].appendChild(cell);
        if (control) cell.appendChild(control);

        return cell;
    }

    table.addTextCell = function (text) {
        var cell = document.createElement("td");
        jsObject.ClearStyles(cell);
        this.tr[0].appendChild(cell);
        cell.innerHTML = text;

        return cell;
    }

    table.addTextCellInRow = function (rowNumber, text) {
        var cell = document.createElement("td");
        jsObject.ClearStyles(cell);
        this.tr[rowNumber].appendChild(cell);
        cell.innerHTML = text;

        return cell;
    }

    table.addRow = function () {
        var rowCount = this.tr.length;
        this.tr[rowCount] = document.createElement("tr");
        jsObject.ClearStyles(this.tr[rowCount]);
        this.appendChild(this.tr[rowCount]);

        return this.tr[rowCount];
    }

    table.insertRow = function (rowIndex) {
        if (rowIndex < this.tr.length) {
            var newRow = document.createElement("tr");
            this.insertBefore(newRow, this.tr[rowIndex]);
            this.tr.splice(rowIndex, 0, newRow);
            return newRow;
        }
        else {
            return table.addRow();
        }
    }

    table.removeRow = function (rowIndex) {
        if (rowIndex < this.tr.length) {
            this.removeChild(this.tr[rowIndex]);
            this.tr.splice(rowIndex, 1);
        }
    }

    table.clear = function () {
        for (var i = 0; i < this.tr.length; i++) {
            var row = this.tr[i];
            row.parentElement.removeChild(row);
        }
        this.tr = [];
        this.tr[0] = document.createElement("tr");
        jsObject.ClearStyles(this.tr[0]);
        this.appendChild(this.tr[0]);
    }

    return table;
}