
StiMobileDesigner.prototype.InitializeResourceViewDataForm_ = function () {
    var viewDataForm = this.BaseForm("resourceViewDataForm", this.loc.FormDictionaryDesigner.ViewData, 4);
    viewDataForm.buttonCancel.style.display = "none";

    var formWidth = 1000;
    var formHeight = 550;
    viewDataForm.container.style.width = formWidth + "px";
    viewDataForm.container.style.height = formHeight + "px";
    viewDataForm.container.style.padding = "12px";

    viewDataForm.show = function (dataTables, resourceName) {
        viewDataForm.caption.innerHTML = viewDataForm.jsObject.loc.FormDictionaryDesigner.ViewData + " - " + resourceName;

        while (this.container.childNodes[0]) this.container.removeChild(this.container.childNodes[0]);
        if (!dataTables) {
            this.changeVisibleState(true);
            return;
        }

        var tabs = [];
        for (var name in dataTables) {
            tabs.push({ "name": name, "caption": name });
        }

        var tabbedPane = this.jsObject.TabbedPane("resourceViewDataFormTabbedPane", tabs, "stiDesignerStandartTab");
        viewDataForm.container.appendChild(tabbedPane);

        tabbedPane.tabsPanel.style.width = formWidth + "px";
        tabbedPane.tabsPanel.style.overflowX = "auto";

        for (var i = 0; i < tabs.length; i++) {
            var tabsPanel = tabbedPane.tabsPanels[tabs[i].name];
            tabsPanel.style.overflow = "auto";
            tabsPanel.style.height = (formHeight - (this.jsObject.options.isTouchDevice ? 57 : 47)) + "px";
        }

        for (var name in dataTables) {
            var data = dataTables[name];

            var table = this.jsObject.CreateHTMLTable();
            tabbedPane.tabsPanels[name].appendChild(this.jsObject.FormSeparator());
            tabbedPane.tabsPanels[name].appendChild(table);
            if (data.length > 1) {
                var imgColumns = {};

                for (var i = 0; i < data[0].length; i++) {
                    var captionCell = table.addTextCell(data[0][i]);
                    captionCell.style.fontWeight = "bold";
                    captionCell.style.textAlign = "center";
                    captionCell.className = "stiDesignerViewDataTableCell";
                }

                for (var i = 1; i < data.length; i++) {
                    table.addRow();
                    for (var k = 0; k < data[i].length; k++) {
                        var cell = table.addCellInLastRow();
                        if (data[i][k].type == "Image") {
                            var img = document.createElement("img");
                            img.src = data[i][k].value;
                            cell.appendChild(img);
                        }
                        else {
                            cell.innerHTML = data[i][k].value;
                            cell.className = "stiDesignerViewDataTableCell";
                        }
                    }
                }
            }
        }

        this.changeVisibleState(true);
    }

    viewDataForm.action = function () {
        this.changeVisibleState(false);
    }

    return viewDataForm;
}