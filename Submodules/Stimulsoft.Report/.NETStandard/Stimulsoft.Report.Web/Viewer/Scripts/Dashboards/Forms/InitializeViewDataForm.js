
StiJsViewer.prototype.InitializeViewDataForm = function () {
    var jsObject = this;
    var form = this.BaseForm("ViewDataForm", this.collections.loc["ViewData"], 1);
    var gridWidth = 600;
    var gridHeight = 500;
    form.container.style.width = gridWidth + "px";
    form.container.style.height = gridHeight + "px";
    form.container.style.padding = "0px";
    form.container.style.position = "relative";
    form.container.style.margin = "10px";
    form.buttonsPanel.style.display = "none";
    this.AddProgressToControl(form.container);

    form.show = function (element) {
        var elementAttrs = element.elementAttributes;
        this.changeVisibleState(true);
        form.container.progress.show();

        jsObject.postAjax(jsObject.getActionRequestUrl(jsObject.options.requestUrl, jsObject.options.actions.viewerEvent),
            {
                action: "DashboardViewData",
                dashboardFilteringParameters: {
                    elementName: elementAttrs.name
                }
            },
            function (answer) {
                form.container.progress.hide();
                var answerObject = JSON.parse(answer);
                var dataGrid = jsObject.DataGrid(gridWidth, gridHeight, answerObject.settings, elementAttrs, element, true);

                form.style.background = form.header.style.background = form.container.style.background = elementAttrs.backColor || answerObject.settings.cellBackColor;
                if (elementAttrs.foreColor) form.header.style.color = elementAttrs.foreColor;
                                
                StiJsViewer.setImageSource(form.buttonClose.image, jsObject.options, jsObject.collections, elementAttrs.actionColors.isDarkStyle ? "CloseWhite.png" : "CloseForm.png");
                form.style.borderColor = elementAttrs.actionColors.isDarkStyle ? "#eeeeee" : "";

                form.container.appendChild(dataGrid);
                dataGrid.showData(answerObject.data);
                form.container.style.width = form.container.style.height = "auto";

                if (!answerObject.data || answerObject.data.length == 0) {
                    var emptyText = document.createElement("div");
                    emptyText.setAttribute("style", "font-family:'Arial'; font-size:12px; text-align:center; position:absolute; width:300px; left:calc(50% - 150px); top:50%; color:" + elementAttrs.foreColor + ";");
                    emptyText.innerHTML = jsObject.collections.loc.DataNotFound;
                    dataGrid.appendChild(emptyText);
                }
            });
    }

    form.action = function () {
        this.changeVisibleState(false);
    }

    return form;
}