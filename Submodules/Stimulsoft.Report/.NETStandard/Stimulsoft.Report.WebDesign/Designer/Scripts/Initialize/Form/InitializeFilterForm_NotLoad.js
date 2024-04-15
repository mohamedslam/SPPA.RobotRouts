
StiMobileDesigner.prototype.InitializeFilterForm_ = function () {
    //Filter Form
    var filterForm = this.BaseForm("filterForm", this.loc.PropertyMain.Filter, 2, this.HelpLinks["filter"]);
    var filterControl = filterForm.filterControl = this.FilterControl("filterFormFilterControl", null, 550, 350);

    filterForm.container.appendChild(filterControl);
    filterForm.resultControl = null;

    filterControl.style.margin = "12px 8px 12px 8px";
    filterControl.controls.toolBar.style.margin = "4px 4px 8px 4px";

    filterForm.onhide = function () {
        this.resultControl = null;
        this.jsObject.DeleteTemporaryMenus();
    }

    filterForm.onshow = function () {
        this.tempResultControl = (this.resultControl) ? this.resultControl : null;
        this.resultControl = null;
        filterControl.currentDataSourceName = null;

        var parent = (this.tempResultControl == null) ? this.jsObject.options.selectedObject.properties : this.tempResultControl.formResult;
        if (this.tempResultControl) filterControl.currentDataSourceName = this.tempResultControl.currentDataSourceName;

        var filters = parent.filterData != "" ? JSON.parse(StiBase64.decode(parent.filterData)) : [];
        filterControl.fill(filters, parent.filterOn, parent.filterMode);
    }

    filterForm.action = function () {
        var filterResult = filterControl.getValue();
        filterForm.changeVisibleState(false);

        if (!this.tempResultControl) {
            this.jsObject.options.selectedObject.properties.filterOn = filterResult.filterOn;
            this.jsObject.options.selectedObject.properties.filterMode = filterResult.filterMode;
            this.jsObject.options.selectedObject.properties.filterData = StiBase64.encode(JSON.stringify(filterResult.filters));
            this.jsObject.SendCommandSendProperties(this.jsObject.options.selectedObject, ["filterData", "filterOn", "filterMode"]);
        }
        else {
            this.tempResultControl.formResult = {
                filterData: StiBase64.encode(JSON.stringify(filterResult.filters)),
                filterOn: filterResult.filterOn,
                filterMode: filterResult.filterMode
            }
            if (this.tempResultControl["setKey"] != null) {
                this.tempResultControl.setKey(this.tempResultControl.formResult);
            }
            else {
                this.tempResultControl.value = this.jsObject.FilterDataToShortString(filterResult.filters);
            }
            this.tempResultControl.action();
        }
    }

    return filterForm;
}