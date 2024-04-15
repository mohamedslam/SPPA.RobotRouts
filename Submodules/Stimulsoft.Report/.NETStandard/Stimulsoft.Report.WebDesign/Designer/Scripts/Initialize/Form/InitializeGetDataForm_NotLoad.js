
StiMobileDesigner.prototype.InitializeGetDataForm_ = function () {
    var form = this.BaseForm("getDataForm", this.loc.Wizards.GetData, 1);
    var jsObject = this;
    var upTable = this.CreateHTMLTable();
    form.container.appendChild(upTable);

    var demoButton = this.NewReportCloudPanelWizardButton(this.loc.Wizards.UseDemoData);
    demoButton.style.margin = "12px";

    var buttonsTable = form.buttonsPanel.firstChild;
    buttonsTable.style.width = "700px";
    buttonsTable.insertCell(0).style.width = "100%";
    buttonsTable.insertCell(0, demoButton).style.lineHeight = "0";

    var reportButton = this.NewReportCloudPanelButton(null, this.loc.Components.StiReport, "Empty16.png", { width: 72, height: 101 }, null, null, true);
    reportButton.style.marginRight = "0";
    reportButton.setSelected(true);

    if (this.options.showFileMenuNewReport !== false) {
        upTable.addCell(reportButton).style.padding = "12px 12px 0 12px";
    }

    var dbsButton = this.NewReportCloudPanelButton(null, this.loc.Components.StiDashboard, "Empty16.png", { width: 108, height: 60 }, null, null, true);

    if (this.options.dashboardAssemblyLoaded && this.options.showFileMenuNewDashboard !== false) {
        upTable.addCell(dbsButton).style.verticalAlign = "bottom";
    }

    var changeReportType = function (reportType) {
        jsObject.SendCommandToDesignerServer("ChangeReportType", { reportType: reportType, zoom: jsObject.options.report ? jsObject.options.report.zoom.toString() : "1" },
            function (answer) {
                jsObject.CloseReport();
                jsObject.options.reportGuid = answer.reportGuid;
                jsObject.LoadReport(jsObject.ParseReport(answer.reportObject));
            });
    }

    reportButton.action = function () {
        if (this.isSelected) return;
        this.setSelected(true);
        dbsButton.setSelected(false);
        changeReportType("Report");
    }

    dbsButton.action = function () {
        if (this.isSelected) return;
        this.setSelected(true);
        reportButton.setSelected(false);
        changeReportType("Dashboard");
    }

    demoButton.action = function () {
        jsObject.SendCommandToDesignerServer("AddDemoDataToReport", { isDashboard: dbsButton.isSelected, zoom: jsObject.options.report ? jsObject.options.report.zoom.toString() : "1" },
            function (answer) {
                jsObject.CloseReport();
                jsObject.options.reportGuid = answer.reportGuid;
                jsObject.LoadReport(jsObject.ParseReport(answer.reportObject));
                form.action();
            });
    }

    var sep = this.FormSeparator();
    sep.style.margin = "6px 12px 6px 12px";
    sep.style.borderTopStyle = "solid";
    form.container.appendChild(sep);

    var newDataSourceButton = this.StandartSmallButton(null, null, this.loc.FormDictionaryDesigner.DataSourceNew, "DataSourceNew.png", null, null, true);
    newDataSourceButton.style.margin = "0 12px 3px 12px";
    newDataSourceButton.style.display = "none";
    form.container.appendChild(newDataSourceButton);

    newDataSourceButton.action = function () {
        jsObject.InitializeSelectConnectionForm(function (form) {
            form.changeVisibleState(true);
        });
    }

    var sep2 = this.FormSeparator();
    sep2.style.margin = "0 12px 0 12px";
    sep2.style.display = "none";
    form.container.appendChild(sep2);

    var dataPanel = document.createElement("div");
    form.container.appendChild(dataPanel);
    dataPanel.style.height = "300px";
    dataPanel.style.width = "700px";

    dataPanel.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
    }

    //Hint Text
    var createDataHintItem = this.CreateDataHintItem();
    createDataHintItem.style.marginTop = "70px";
    dataPanel.appendChild(createDataHintItem);

    createDataHintItem.onclick = function () {
        jsObject.InitializeSelectConnectionForm(function (form) {
            form.changeVisibleState(true);
        });
    }

    jsObject.AddDragAndDropToContainer(dataPanel, function (files, content) {
        if (!jsObject.options.report) return;

        var dataExts = ["xls", "xlsx", "csv", "dbf", "json", "xml"];
        var fileName = files[0].name || "Resource";
        var fileExt = fileName.substring(fileName.lastIndexOf(".") + 1).toLowerCase();
        var resourceType;

        if (fileExt == "xml") {
            resourceType = "Xml";
        }
        else if (fileExt == "xsd") {
            resourceType = "Xsd";
        }
        else if (fileExt == "xls" || fileExt == "xlsx") {
            resourceType = "Excel";
        }
        else if (fileExt == "csv") {
            resourceType = "Csv";
        }
        else if (fileExt == "dbf") {
            resourceType = "Dbf";
        }
        else if (fileExt == "json") {
            resourceType = "Json";
        }
        else {
            return;
        }

        var resourceName = jsObject.GetNewName("Resource", null, fileName.substring(0, fileName.lastIndexOf(".")));

        var resource = {};
        resource.mode = "New";
        resource.name = resourceName;
        resource.alias = resourceName;
        resource.type = resourceType;
        resource.loadedContent = jsObject.options.mvcMode ? encodeURIComponent(content) : content;
        resource.haveContent = true;

        jsObject.SendCommandCreateDatabaseFromResource(resource);
    });

    form.updateData = function () {
        dataPanel.clear();
        var dataPresents = jsObject.options.report.dictionary.databases.length > 0;
        createDataHintItem.style.display = dataPresents ? "none" : "";
        sep2.style.display = dataPresents ? "" : "none";
        dataPanel.style.height = dataPresents ? "270px" : "300px";
        newDataSourceButton.style.display = dataPresents ? "inline-block" : "none";
        dataPanel.appendChild(jsObject.WizardDataTree());
    }

    form.cancelAction = function () {
        if (this.oldReportContent) {
            jsObject.SendCommandToDesignerServer("RestoreOldReport", { oldReportContent: this.oldReportContent, zoom: jsObject.options.report ? jsObject.options.report.zoom.toString() : "1" },
                function (answer) {
                    jsObject.CloseReport();
                    jsObject.options.reportGuid = answer.reportGuid;
                    if (jsObject.options.cloudParameters && form.oldReportTemplateItemKey) {
                        jsObject.options.cloudParameters.reportTemplateItemKey = form.oldReportTemplateItemKey;
                    }
                    jsObject.LoadReport(jsObject.ParseReport(answer.reportObject));
                });
        }
    }

    form.action = function () {
        form.changeVisibleState(false);
        var fileMenu = jsObject.options.menus.fileMenu || jsObject.InitializeFileMenu();
        fileMenu.changeVisibleState(false);
        setTimeout(function () {
            jsObject.autoCreateDataComponent();
        }, 200);
    }

    return form;
}

StiMobileDesigner.prototype.WizardDataTree = function () {
    var dataTree = this.Tree(675, 245);
    dataTree.style.overflow = "auto";
    dataTree.style.padding = "12px";
    var jsObject = this;

    var mainItem = jsObject.DataTreeItem("Main", null, { "typeItem": "MainItem" }, dataTree);
    mainItem.childsRow.style.display = "";
    mainItem.button.style.display = "none";
    mainItem.iconOpening.style.display = "none";
    dataTree.appendChild(mainItem);

    dataTree.addTreeItems = function (collection, parentItem, buildOnlyOneLevel) {
        if (collection) {
            if (parentItem == mainItem) {
                collection.sort(jsObject.SortByName);
            }
            for (var i = 0; i < collection.length; i++) {
                var childItem = parentItem.addChild(jsObject.DictionaryTreeItemFromObject(collection[i]));
                var childCollection = [];

                switch (collection[i].typeItem) {
                    case "DataBase": { childCollection = collection[i].dataSources; break; }
                }
                this.addTreeItems(childCollection, childItem);
                childItem.setOpening(true);
            }
        }
    }

    if (jsObject.options.report) {
        dataTree.addTreeItems(jsObject.options.report.dictionary.databases, mainItem, true);
    }

    return dataTree;
}