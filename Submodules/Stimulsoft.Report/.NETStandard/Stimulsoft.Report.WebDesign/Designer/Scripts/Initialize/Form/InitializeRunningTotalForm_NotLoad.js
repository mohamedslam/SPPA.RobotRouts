
StiMobileDesigner.prototype.InitializeRunningTotalForm_ = function () {

    var form = this.BaseForm("runningTotalForm", this.loc.FormSystemTextEditor.RunningTotal, 2);
    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "24px 12px 12px 12px";
    form.container.appendChild(innerTable);

    innerTable.addTextCell(this.loc.Dashboard.InitialValue).className = "stiDesignerTextContainer";

    var initialValueControl = this.TextBox(null, 250);
    innerTable.addCellInNextRow(initialValueControl).style.padding = "5px 0 12px 0";

    initialValueControl.action = function () {
        if (this.value != "" && (this.value.indexOf("{") < 0 || this.value.indexOf("}") < 1)) {
            this.value = this.jsObject.StrToDouble(this.value);
        }
        form.actionRule.initialValue = this.value;
    }

    form.show = function (columnObject, actionRules) {
        this.changeVisibleState(true);
        this.actionRule = (actionRules.length == 0)
            ? this.jsObject.DataActionRuleObject(columnObject.key, columnObject.path, "RunningTotal")
            : this.jsObject.CopyObject(actionRules[0]);
        if (this.actionRule.initialValue == null) {
            this.actionRule.initialValue = "0"
        }
        initialValueControl.value = this.actionRule.initialValue;
        initialValueControl.focus();
    }

    return form;
}