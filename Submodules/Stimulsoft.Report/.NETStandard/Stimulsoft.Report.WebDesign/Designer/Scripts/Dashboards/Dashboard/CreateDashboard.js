
StiMobileDesigner.prototype.CreateDashboard = function (pageObject) {
    var page = this.CreatePage(pageObject, true);

    //Override methods for dashboard

    page.ondblclick = function (event) {
        this.jsObject.options.pageIsDblClick = true;
        if (!this.jsObject.options.componentIsTouched) {
            this.jsObject.InitializeDashboardSetupForm(function (form) {
                form.changeVisibleState(true);
            });
        }
    }

    return page;
}