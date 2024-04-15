
StiMobileDesigner.prototype.InitializeSortForm_ = function () {
    //Sort Form
    var sortForm = this.BaseForm("sortForm", this.loc.PropertyMain.Sort, 2, this.HelpLinks["sort"]);
    var sortControl = sortForm.sortControl = this.SortControl("sortFormSortControl", null, 500, 300);

    sortForm.container.appendChild(sortControl);
    sortForm.resultControl = null;

    sortControl.style.margin = "12px 8px 12px 8px";
    sortControl.toolBar.style.margin = "4px 4px 8px 4px";

    sortForm.onshow = function () {
        this.tempResultControl = (this.resultControl) ? this.resultControl : null;
        this.resultControl = null;
        sortControl.currentDataSourceName = null;

        var sortData = (this.tempResultControl == null) ? this.jsObject.options.selectedObject.properties.sortData : this.tempResultControl.formResult;
        if (this.tempResultControl) sortControl.currentDataSourceName = this.tempResultControl.currentDataSourceName;

        var sorts = (sortData != "") ? JSON.parse(StiBase64.decode(sortData)) : [];
        sortControl.fill(sorts);
        sortControl.sortContainer.addSort({ "column": this.jsObject.loc.FormBand.NoSort, "direction": "ASC" });
    }

    sortForm.onhide = function () {
        this.jsObject.DeleteTemporaryMenus();
    }

    sortForm.action = function () {
        var result = sortControl.getValue();
        sortForm.changeVisibleState(false);

        if (!this.tempResultControl) {
            this.jsObject.options.selectedObject.properties.sortData = result.length == 0 ? "" : StiBase64.encode(JSON.stringify(result));
            this.jsObject.SendCommandSendProperties(this.jsObject.options.selectedObject, ["sortData"]);
        }
        else {
            this.tempResultControl.formResult = result.length == 0 ? "" : StiBase64.encode(JSON.stringify(result));
            if (this.tempResultControl["setKey"] != null) {
                this.tempResultControl.setKey(this.tempResultControl.formResult);
            }
            else {
                this.tempResultControl.value = this.jsObject.SortDataToShortString(this.tempResultControl.formResult);
            }
            this.tempResultControl.action();
        }
    }

    return sortForm;
}