
StiJsViewer.prototype.InitializeFiltersForm = function () {
    var form = this.BaseForm("filtersForm", this.collections.loc["Filters"], 2);
    form.container.style.padding = 0;

    var filterControl = this.FiltersControl("filtersFormFilterControl", 700, 350);
    form.filterControl = filterControl;
    form.container.appendChild(filterControl);

    form.show = function (columnObject, filters, filterItems) {
        this.changeVisibleState(true);
        filterControl.columnObject = columnObject;
        filterControl.filterItems = filterItems;
        filterControl.setFilters(this.jsObject.copyObject(filters));
    }

    return form;
}