
StiMobileDesigner.prototype.InitializeChartSeriesFilterForm_ = function () {
    var form = this.BaseForm("chartSeriesFilterForm", this.loc.PropertyMain.Filter, 2, this.HelpLinks["filter"]);

    var filtersPanel = this.EditChartFormFiltersPanel(form);
    form.container.appendChild(filtersPanel);

    form.onhide = function () {
        this.resultControl = null;
        this.jsObject.DeleteTemporaryMenus();
    }

    form.onshow = function () {
        filtersPanel.container.addItems(this.resultControl.key);
    }

    form.action = function () {
        this.resultControl.setKey(filtersPanel.container.getValue());
        this.resultControl.action();
        form.changeVisibleState(false);
    }

    return form;
}