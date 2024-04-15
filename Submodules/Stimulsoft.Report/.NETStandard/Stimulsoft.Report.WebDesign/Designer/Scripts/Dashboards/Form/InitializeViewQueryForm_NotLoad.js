
StiMobileDesigner.prototype.InitializeViewQueryForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("ViewQueryForm", this.loc.QueryBuilder.ViewQuery, 1);
    var gridWidth = 600;
    var gridHeight = 500;
    form.container.style.width = gridWidth + "px";
    form.container.style.height = gridHeight + "px";
    form.hideButtonsPanel();
    this.AddProgressToControl(form.container);

    form.show = function (dashboard) {
        this.changeVisibleState(true);
        form.container.progress.show();
        setTimeout(function () {
            jsObject.SendCommandToDesignerServer("ViewQuery", { dashboardName: dashboard.properties.name }, function (answer) {
                form.container.progress.hide();
                var dataGrid = jsObject.DataGrid(gridWidth, gridHeight, true);
                form.container.appendChild(dataGrid);
                dataGrid.showData(answer.data, answer.sortLabels);

                dataGrid.action = function (headerButton) {
                    form.container.progress.show();
                    var columnName = headerButton.columnName;
                    headerButton.sortDirection = headerButton.sortDirection == "Ascending" ? "DESC" : "ASC";

                    jsObject.SendCommandToDesignerServer("ViewQuery", { dashboardName: dashboard.properties.name, sort: { columnName: columnName, direction: headerButton.sortDirection } },
                        function (answer) {
                            dataGrid.clear();
                            form.container.progress.hide();
                            dataGrid.showData(answer.data, answer.sortLabels);
                        });
                }
            });
        }, 0);
    }

    form.action = function () {
        this.changeVisibleState(false);
    }

    return form;
}