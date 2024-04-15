
StiMobileDesigner.prototype.InitializeTrendLinesForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("trendLinesForm", this.loc.Chart.TrendLinesEditorForm, 1, this.HelpLinks["chartElement"]);
    var trendLinesControl = form.trendLinesControl = this.TrendLinesControl(350, 420);
    form.container.appendChild(trendLinesControl);

    trendLinesControl.style.margin = "12px 8px 12px 8px";
    trendLinesControl.toolBar.style.margin = "4px 4px 8px 4px";

    trendLinesControl.container.className = "stiSimpleContainerWithBorder";
    trendLinesControl.container.style.margin = "4px";
    trendLinesControl.container.style.overflow = "auto";

    form.show = function (resultControl) {
        var selectedObject = jsObject.options.selectedObject || (jsObject.options.selectedObjects && jsObject.options.selectedObjects.length > 0 ? jsObject.options.selectedObjects[0] : null);
        if (selectedObject && selectedObject.properties.chartTrendLines) {
            form.trendLinesControl.valueMeters = selectedObject.properties.valueMeters;
        }
        form.resultControl = resultControl;
        var trendLines = resultControl.key;
        if (trendLines) form.trendLinesControl.fill(jsObject.CopyObject(trendLines));
        form.changeVisibleState(true);
    }

    form.action = function () {
        form.changeVisibleState(false);
        var trendLines = form.trendLinesControl.getValue();
        form.resultControl.setKey(trendLines);
        form.resultControl.action();
    }

    return form;
}