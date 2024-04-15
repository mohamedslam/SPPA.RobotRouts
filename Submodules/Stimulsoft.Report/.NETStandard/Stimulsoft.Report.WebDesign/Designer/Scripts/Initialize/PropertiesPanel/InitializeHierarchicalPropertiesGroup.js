
StiMobileDesigner.prototype.HierarchicalPropertiesGroup = function () {
    var hierarchicalPropertiesGroup = this.PropertiesGroup("hierarchicalPropertiesGroup", this.loc.PropertyCategory.HierarchicalCategory);
    hierarchicalPropertiesGroup.style.display = "none";

    var actionFunctionForKeyContol = function (button, propertyName) {
        button.key = "";
        button.jsObject.InitializeDataColumnForm(function (dataColumnForm) {
            dataColumnForm.parentButton = button;
            dataColumnForm.changeVisibleState(true);

            dataColumnForm.action = function () {
                dataColumnForm.changeVisibleState(false);
                var columnKey = dataColumnForm.dataTree.key == ""
                    ? "" : dataColumnForm.dataTree.key.substring(dataColumnForm.dataTree.key.lastIndexOf(".") + 1, dataColumnForm.dataTree.key.length);
                dataColumnForm.parentButton.textBox.value = columnKey || dataColumnForm.jsObject.loc.Report.NotAssigned;
                this.jsObject.ApplyPropertyValue(propertyName, columnKey);
            }
        });
    }

    //KeyDataColumn
    var controlPropertyKeyDataColumn = this.PropertyTextBoxWithEditButton("controlPropertyKeyDataColumn", this.options.propertyControlWidth, true);
    controlPropertyKeyDataColumn.button.action = function () { actionFunctionForKeyContol(this, "keyDataColumn"); }
    hierarchicalPropertiesGroup.container.appendChild(this.Property("keyDataColumn", this.loc.PropertyMain.KeyDataColumn, controlPropertyKeyDataColumn));

    //MasterKeyDataColumn
    var controlPropertyMasterKeyDataColumn = this.PropertyTextBoxWithEditButton("controlPropertyMasterKeyDataColumn", this.options.propertyControlWidth, true);
    controlPropertyMasterKeyDataColumn.button.action = function () { actionFunctionForKeyContol(this, "masterKeyDataColumn") };
    hierarchicalPropertiesGroup.container.appendChild(this.Property("masterKeyDataColumn", this.loc.PropertyMain.MasterKeyDataColumn, controlPropertyMasterKeyDataColumn));

    //ParentValue
    var controlPropertyParentValue = this.PropertyTextBox("controlPropertyParentValue", this.options.propertyControlWidth);
    controlPropertyParentValue.action = function () {
        this.jsObject.ApplyPropertyValue("parentValue", StiBase64.encode(this.value));
    }
    hierarchicalPropertiesGroup.container.appendChild(this.Property("parentValue", this.loc.PropertyMain.ParentValue, controlPropertyParentValue));

    //Indent
    var controlPropertyIndent = this.PropertyTextBox("controlPropertyIndent", this.options.propertyControlWidth);
    controlPropertyIndent.action = function () {
        this.value = this.jsObject.StrToDouble(this.value);
        this.jsObject.ApplyPropertyValue("indent", this.value);
    }
    hierarchicalPropertiesGroup.container.appendChild(this.Property("indent", this.loc.PropertyMain.Indent, controlPropertyIndent));

    //Headers
    var controlPropertyHeaders = this.PropertyTextBox("controlPropertyHeaders", this.options.propertyControlWidth);
    controlPropertyHeaders.action = function () {
        this.jsObject.ApplyPropertyValue("headers", StiBase64.encode(this.value));
    }
    hierarchicalPropertiesGroup.container.appendChild(this.Property("headers", this.loc.PropertyMain.Headers, controlPropertyHeaders));

    //Footers
    var controlPropertyFooters = this.PropertyTextBox("controlPropertyFooters", this.options.propertyControlWidth);
    controlPropertyFooters.action = function () {
        this.jsObject.ApplyPropertyValue("footers", StiBase64.encode(this.value));
    }
    hierarchicalPropertiesGroup.container.appendChild(this.Property("footers", this.loc.PropertyMain.Footers, controlPropertyFooters));

    return hierarchicalPropertiesGroup;
}