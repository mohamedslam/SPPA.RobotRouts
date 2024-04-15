
StiMobileDesigner.prototype.ProcessDesignerFormsEvents = function (data) {
    var jsObject = this;
    if (data) {
        switch (data.action) {
            case "InterfaceEvent": {
                if (data.event == "RibbonFileClick") {
                    jsObject.options.formsDesignerFrame.hide();
                    jsObject.ExecuteAction("fileButton");
                }
                break;
            }
            case "CheckIsFormChanged": {
                switch (data.nextAction) {
                    case "openReport": {
                        jsObject.ActionOpenReport(data.isFormChanged);
                        break;
                    }
                    case "newReport": {
                        jsObject.ActionNewReport(data.isFormChanged);
                        break;
                    }
                    case "newDashboard": {
                        jsObject.ActionNewDashboard(data.isFormChanged);
                        break;
                    }
                    case "newForm": {
                        jsObject.ActionNewForm(data.isFormChanged);
                        break;
                    }
                    case "openReportFromCloudItem": {
                        if (jsObject.options.openPanel) {
                            jsObject.options.openPanel.openReportFromCloudItem(data.params.itemObject, data.params.notSaveToRecent, data.isFormChanged);
                        }
                        break;
                    }                        
                }
                break;
            }
            case "ItemResourceSave": {
                jsObject.SendCommandItemResourceSave(data.params.itemKey, data.params.customMessage, data.formContent);
                break;
            }
        }
    }
}