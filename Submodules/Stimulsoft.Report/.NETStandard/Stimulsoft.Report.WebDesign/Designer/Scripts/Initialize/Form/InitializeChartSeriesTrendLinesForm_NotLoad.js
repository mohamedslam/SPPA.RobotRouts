
StiMobileDesigner.prototype.InitializeChartSeriesTrendLinesForm_ = function () {
    var form = this.BaseForm("chartSeriesTrendLinesForm", this.loc.Chart.TrendLinesEditorForm, 1);

    var linesPanel = this.EditChartFormTrendLinesPanel(form);
    form.container.appendChild(linesPanel);

    form.onhide = function () {
        this.resultControl = null;
        this.jsObject.DeleteTemporaryMenus();
    }

    form.onshow = function () {
        linesPanel.container.addItems(this.resultControl.key);
    }

    form.action = function () {
        this.resultControl.setKey(linesPanel.container.getValue());
        this.resultControl.action();
        form.changeVisibleState(false);
    }

    return form;
}