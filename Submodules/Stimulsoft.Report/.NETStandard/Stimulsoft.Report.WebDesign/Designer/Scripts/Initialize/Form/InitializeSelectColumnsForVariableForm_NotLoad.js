
StiMobileDesigner.prototype.InitializeSelectColumnsForVariableForm_ = function () {

    var form = this.BaseForm("selectColumnsForVariable", this.loc.Wizards.SelectColumns, 4);
    form.controls = {};

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "6px 0 6px 0";
    form.container.appendChild(innerTable);

    var controlProps = [
        ["keys", this.loc.PropertyMain.Keys, this.DataControl(null, 250)],
        ["values", this.loc.PropertyMain.Labels, this.DataControl(null, 250)]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        innerTable.addTextCellInNextRow(controlProps[i][1]).className = "stiDesignerCaptionControlsBigIntervals";
        form.controls[controlProps[i][0]] = controlProps[i][2];
        innerTable.addCellInLastRow(controlProps[i][2]).className = "stiDesignerControlCellsBigIntervals2";
    }

    form.onshow = function () {
        form.controls.keys.textBox.value = "";
        form.controls.values.textBox.value = "";
    }

    return form;
}