
StiMobileDesigner.prototype.OverrideCreateHTMLTable = function () {

    this.OldCreateHTMLTable = this.CreateHTMLTable;
    this.CreateHTMLTable = function (rowsCount, cellsCount) {
        var table = this.OldCreateHTMLTable(rowsCount, cellsCount);

        table.addCell = function (control, pos, style) {
            var cell = $("<td" + (style ? " style='" + style + "'" : "") + "></td>")[0];
            if (!pos) {
                this.tr[0].appendChild(cell)
            } else {
                this.tr[this.tr.length - 1].appendChild(cell)
            }

            if (control) cell.appendChild(control);

            return cell;
        }

        return table;
    }
}