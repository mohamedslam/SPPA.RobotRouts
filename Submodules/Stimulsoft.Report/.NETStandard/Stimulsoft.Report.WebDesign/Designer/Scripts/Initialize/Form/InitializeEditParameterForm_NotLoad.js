
StiMobileDesigner.prototype.InitializeEditParameterForm_ = function () {

    //Edit Column Form
    var editParameterForm = this.BaseForm("editParameterForm", this.loc.PropertyMain.Parameter, 3, this.HelpLinks["parameterEdit"]);
    editParameterForm.parameter = null;
    editParameterForm.mode = "Edit";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "6px 0 6px 0";
    editParameterForm.container.appendChild(innerTable);

    var controlProps = [
        ["name", this.loc.PropertyMain.Name, this.TextBox(null, 300)],
        ["type", this.loc.PropertyMain.Type.replace(":", ""), this.DropDownList("editParameterFormType", 150, null, null, true, false, null, true)],
        ["size", this.loc.PropertyMain.Size, this.TextBox(null, 150)],
        ["expression", this.loc.PropertyMain.Expression, this.TextArea(null, 300, 80)]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        editParameterForm[controlProps[i][0] + "ControlRow"] = innerTable.addRow();
        var text = innerTable.addCellInLastRow();
        text.className = "stiDesignerCaptionControlsBigIntervals";
        text.innerHTML = controlProps[i][1];
        var control = controlProps[i][2];
        control.textCell = text;
        editParameterForm[controlProps[i][0] + "Control"] = control;
        innerTable.addCellInLastRow(control).className = "stiDesignerControlCellsBigIntervals2";
    }

    editParameterForm.onshow = function () {
        //debugger;
        this.mode = "Edit";
        this.currentDataSource = this.jsObject.GetDataSourceByNameFromDictionary(this.jsObject.options.dictionaryTree.getCurrentColumnParent().name);
        if (this.currentDataSource && this.currentDataSource.parameterTypes) {
            this.typeControl.addItems(this.jsObject.GetParameterTypeItems(this.currentDataSource.parameterTypes));
        }

        if (this.parameter == null) {
            this.parameter = this.jsObject.ParameterObject(this.currentDataSource ? this.currentDataSource.parameters : null);
            this.mode = "New";
            if (this.currentDataSource && this.currentDataSource.parameterTypes) {
                for (var i = 0; i < this.currentDataSource.parameterTypes.length; i++) {
                    if (i == 0) this.parameter.type = this.currentDataSource.parameterTypes[0].typeValue.toString();
                    if (this.currentDataSource.parameterTypes[i].typeName == "Text") {
                        this.parameter.type = this.currentDataSource.parameterTypes[i].typeValue.toString();
                        break;
                    };
                }
            }
        }
        var caption = this.jsObject.loc.FormDictionaryDesigner["DataParameter" + this.mode];
        this.caption.innerHTML = caption;

        this.nameControl.value = this.parameter.name;
        this.typeControl.setKey(this.parameter.type);
        this.sizeControl.value = this.parameter.size;
        this.expressionControl.value = StiBase64.decode(this.parameter.expression);
    }

    editParameterForm.action = function () {
        this.parameter.mode = this.mode;
        this.parameter.currentParentName = this.currentDataSource.name;
        if (this.mode == "Edit") this.parameter["oldName"] = this.parameter.name;
        this.parameter.name = this.nameControl.value;
        this.parameter.type = this.typeControl.key;
        this.parameter.size = this.sizeControl.value;
        this.parameter.expression = StiBase64.encode(this.expressionControl.value);
        this.changeVisibleState(false);
        this.jsObject.SendCommandCreateOrEditParameter(this.parameter);
    }

    return editParameterForm;
}
