
StiMobileDesigner.prototype.InitializeDataBusinessObjectForm_ = function () {
    //Data Business Object Form
    var dataBusinessObjectForm = this.BaseForm("dataBusinessObject", this.loc.PropertyMain.BusinessObject.replace(" ", ""), 3);
    dataBusinessObjectForm.dataTree = this.options.dataTree;
    dataBusinessObjectForm.parentButton = null;

    //Data Business Object Panel
    var dataBusinessObjectPanel = document.createElement("Div");
    dataBusinessObjectPanel.className = "stiDesignerDataColumnFormMainPanel";
    dataBusinessObjectForm.container.appendChild(dataBusinessObjectPanel);
    dataBusinessObjectForm.container.style.padding = "0px";

    dataBusinessObjectForm.onshow = function () {
        dataBusinessObjectPanel.appendChild(dataBusinessObjectForm.dataTree);
        dataBusinessObjectForm.dataTree.setKey(dataBusinessObjectForm.parentButton ? dataBusinessObjectForm.parentButton.key : "");
        dataBusinessObjectForm.dataTree.action = function () {
            if (dataBusinessObjectForm.parentButton) dataBusinessObjectForm.parentButton.key = this.key;
            dataBusinessObjectForm.action();
        }
    }

    return dataBusinessObjectForm;
}