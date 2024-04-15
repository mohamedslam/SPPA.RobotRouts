
StiMobileDesigner.prototype.InitializeDataColumnForm_ = function () {
    //Data Column Form
    var dataColumnForm = this.BaseForm("dataColumn", this.loc.PropertyMain.DataColumn, 4);
    dataColumnForm.dataTree = this.options.dataTree;
    dataColumnForm.parentButton = null;
    dataColumnForm.needBuildTree = true;

    this.AddProgressToControl(dataColumnForm);

    //Data Column Panel
    var dataColumnPanel = document.createElement("Div");
    dataColumnPanel.className = "stiDesignerDataColumnFormMainPanel";
    dataColumnForm.container.appendChild(dataColumnPanel);
    dataColumnForm.container.style.padding = "0px";

    dataColumnForm.onshow = function () {
        while (dataColumnPanel.childNodes[0]) {
            dataColumnPanel.removeChild(dataColumnPanel.childNodes[0]);
        }
        dataColumnForm.progress.show();
    }

    dataColumnForm.oncompleteshow = function () {
        dataColumnPanel.appendChild(dataColumnForm.dataTree);
        if (dataColumnForm.needBuildTree) dataColumnForm.dataTree.build(null, null, null, true);
        dataColumnForm.needBuildTree = true;
        var setKeyResult = dataColumnForm.dataTree.setKey(dataColumnForm.parentButton ? dataColumnForm.parentButton.key : "");
        //fixed bug with businessobject
        if (!setKeyResult && dataColumnForm.parentButton && dataColumnForm.parentButton.key && dataColumnForm.parentButton.key.indexOf(".") >= 0) {
            var key = dataColumnForm.parentButton.key;
            while (key.indexOf(".") >= 0 && !setKeyResult) {
                key = key.substring(key.indexOf(".") + 1);
                dataColumnForm.dataTree.setKey(key);
            }
        }
        dataColumnForm.dataTree.action = function () {
            if (dataColumnForm.parentButton) dataColumnForm.parentButton.key = this.key;
            dataColumnForm.action();
        }
        dataColumnForm.progress.hide();
    }

    return dataColumnForm;
}