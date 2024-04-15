
StiMobileDesigner.prototype.InitializeFilterRulesForm_ = function () {

    var form = this.BaseForm("filterRulesForm", this.loc.PropertyMain.Filters, 2, this.HelpLinks["filter"]);

    var filterControl = form.filterControl = this.FilterRulesControl("dataTransformationFilterControl", 700, 350);
    form.container.appendChild(filterControl);

    filterControl.style.margin = "12px 8px 12px 8px";
    filterControl.controls.toolBar.style.margin = "4px 4px 8px 4px";

    form.show = function (columnObject, filterRules, filterItems) {
        this.changeVisibleState(true);
        filterControl.columnObject = columnObject;
        filterControl.filterItems = filterItems;
        filterControl.setFilterRules(this.jsObject.CopyObject(filterRules));
    }

    form.onhide = function () {
        this.jsObject.DeleteTemporaryMenus();
    }

    return form;
}