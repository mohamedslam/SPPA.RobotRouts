
StiMobileDesigner.prototype.InitializeLimitRowsForm_ = function () {

    var form = this.BaseForm("limitRowsForm", this.loc.Dashboard.LimitRows, 2);
    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "24px 12px 12px 12px";
    form.container.appendChild(innerTable);

    innerTable.addTextCell(this.loc.Dashboard.SkipFirstRows).className = "stiDesignerTextContainer";

    var firstRowControl = this.TextBox(null, 250);
    innerTable.addCellInNextRow(firstRowControl).style.padding = "5px 0 12px 0";

    firstRowControl.action = function () {
        this.value = Math.abs(this.jsObject.StrToInt(this.value));
        form.actionRule.startIndex = this.value;
    }

    innerTable.addTextCellInNextRow(this.loc.Dashboard.RowsCount).className = "stiDesignerTextContainer";

    var rowsCountControl = this.DropDownList("rowsCountControl", 250, null, this.GetRowsCountItems(), false);
    innerTable.addCellInNextRow(rowsCountControl).style.padding = "5px 0 12px 0";

    rowsCountControl.action = function () {
        var result = this.jsObject.StrToInt(this.key);
        if (result < -1) result = -1;
        form.actionRule.rowsCount = result.toString();
    }

    innerTable.addTextCellInNextRow(this.loc.Dashboard.Priority).className = "stiDesignerTextContainer";

    var priorityControl = this.DropDownList("priorityControl", 250, null, this.GetDataPriorityItems(), true);
    innerTable.addCellInNextRow(priorityControl).style.padding = "5px 0 12px 0";

    priorityControl.action = function () {
        form.actionRule.priority = this.key;
    }

    form.show = function (columnObject, actionRules) {
        this.changeVisibleState(true);
        this.actionRule = (actionRules.length == 0)
            ? this.jsObject.DataActionRuleObject(columnObject.key, columnObject.path, "Limit")
            : this.jsObject.CopyObject(actionRules[0]);

        firstRowControl.value = this.actionRule.startIndex;
        rowsCountControl.setKey(this.actionRule.rowsCount.toString());
        priorityControl.setKey(this.actionRule.priority);

        firstRowControl.focus();
    }

    return form;
}