
StiMobileDesigner.prototype.InitializeParametersValuesForm_ = function () {

    var editParametersValuesForm = this.BaseForm("editParametersValuesForm", this.loc.FormTitles.SqlExpressionsForm, 3);

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "6px";
    editParametersValuesForm.container.appendChild(innerTable);

    editParametersValuesForm.show = function (formParameters, variablesParams, completeFunction) {
        editParametersValuesForm.controls = {};
        editParametersValuesForm.formParameters = formParameters;
        editParametersValuesForm.completeFunction = completeFunction;

        if (variablesParams) {
            for (var i = 0; i < variablesParams.length; i++) {
                innerTable.addRow();
                var variableName = variablesParams[i];
                var variableValue = "";

                if (variableName && variableName.indexOf("{") == 0 && this.jsObject.EndsWith(variableName, "}")) {
                    variableName = variableName.substring(1, variableName.length - 1);
                    var variable = this.jsObject.GetVariableByNameFromDictionary(variableName);
                    if (variable && variable.value && variable.type != "image" && variable.type != "object")
                        variableValue = StiBase64.decode(variable.value);
                }

                innerTable.addTextCellInLastRow(variableName).className = "stiDesignerCaptionControlsBigIntervals";
                var textBox = this.jsObject.TextBox(200);
                textBox.value = variableValue;
                editParametersValuesForm.controls[variablesParams[i]] = textBox;
                innerTable.addCellInLastRow(textBox).className = "stiDesignerControlCellsBigIntervals2";
            }
        }

        for (var i = 0; i < formParameters.parameters.length; i++) {
            innerTable.addRow();
            innerTable.addTextCellInLastRow(formParameters.parameters[i].name).className = "stiDesignerCaptionControlsBigIntervals";
            editParametersValuesForm.controls[formParameters.parameters[i].name] = this.jsObject.TextBox(200);
            innerTable.addCellInLastRow(editParametersValuesForm.controls[formParameters.parameters[i].name]).className = "stiDesignerControlCellsBigIntervals2";
        }

        editParametersValuesForm.changeVisibleState(true);
    }

    editParametersValuesForm.action = function (parameterNames) {
        var parametersValues = {};
        for (var name in editParametersValuesForm.controls) {
            parametersValues[name] = StiBase64.encode(editParametersValuesForm.controls[name].value);
        }

        editParametersValuesForm.formParameters.parametersValues = parametersValues;
        editParametersValuesForm.completeFunction(editParametersValuesForm.formParameters);
        editParametersValuesForm.changeVisibleState(false);
    }

    return editParametersValuesForm;
}