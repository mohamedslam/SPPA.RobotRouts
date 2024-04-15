
StiMobileDesigner.prototype.InitializeChartSeriesConditionsForm_ = function () {
    var form = this.BaseForm("chartSerieConditionsForm", this.loc.PropertyMain.Conditions, 2, this.HelpLinks["conditions"]);

    var conditionsPanel = this.EditChartFormConditionsPanel(form, "SeriesConditions");
    form.container.appendChild(conditionsPanel);

    form.onhide = function () {
        this.resultControl = null;
        this.jsObject.DeleteTemporaryMenus();
    }

    form.onshow = function () {
        conditionsPanel.container.addItems(this.resultControl.key);
    }

    form.action = function () {
        this.resultControl.setKey(conditionsPanel.container.getValue());
        this.resultControl.action();
        form.changeVisibleState(false);
    }

    return form;
}