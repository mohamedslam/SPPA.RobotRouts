
StiMobileDesigner.prototype.InitializeEditColumnForm_ = function () {
    var jsObject = this;
    var editColumnForm = this.BaseFormPanel("editColumnForm", this.loc.PropertyMain.Column, 3, this.HelpLinks["columnEdit"]);
    editColumnForm.column = null;
    editColumnForm.mode = "Edit";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "6px 0 6px 0";
    editColumnForm.container.appendChild(innerTable);

    //Name, NameInSource, Alias
    var textBoxes = [
        ["nameInSource", this.loc.PropertyMain.NameInSource],
        ["name", this.loc.PropertyMain.Name],
        ["alias", this.loc.PropertyMain.Alias]
    ]

    for (var i = 0; i < textBoxes.length; i++) {
        editColumnForm[textBoxes[i][0] + "ControlRow"] = innerTable.addRow();
        var text = innerTable.addCellInLastRow();
        text.className = "stiDesignerCaptionControlsBigIntervals";
        text.innerHTML = textBoxes[i][1];
        var control = editColumnForm[textBoxes[i][0] + "Control"] = this.TextBox("editColumnForm" + textBoxes[i][0], 250);
        innerTable.addCellInLastRow(control).className = "stiDesignerControlCellsBigIntervals2";
    }

    //Type
    editColumnForm["typeControlRow"] = innerTable.addRow();
    var textType = innerTable.addCellInLastRow();
    textType.className = "stiDesignerCaptionControlsBigIntervals";
    textType.innerHTML = this.loc.PropertyMain.Type;
    editColumnForm["typeControl"] = this.DropDownList("editColumnFormTypeControl", 150, null, this.GetColumnTypesItems(), true);
    innerTable.addCellInLastRow(editColumnForm["typeControl"]).className = "stiDesignerControlCellsBigIntervals2";

    //Value
    editColumnForm["valueControlRow"] = innerTable.addRow();
    var textValue = innerTable.addCellInLastRow();
    textValue.className = "stiDesignerCaptionControlsBigIntervals";
    textValue.innerHTML = this.loc.PropertyMain.Value;
    textValue.style.verticalAlign = "top";
    textValue.style.paddingTop = "8px";
    editColumnForm.valueControl = this.TextArea("editColumnFormValueControl", 300, 250);
    innerTable.addCellInLastRow(editColumnForm.valueControl).className = "stiDesignerControlCellsBigIntervals2";

    editColumnForm.nameControl.action = function () {
        if (this.oldValue == editColumnForm.aliasControl.value) {
            editColumnForm.aliasControl.value = this.value;
        }
    }

    editColumnForm.onhide = function () {
        var propertiesPanel = jsObject.options.propertiesPanel;
        if (propertiesPanel) {
            propertiesPanel.setDictionaryMode(false);
            propertiesPanel.setEnabled(this.propertiesPanelIsEnabled);
        }
    }

    editColumnForm.onshow = function () {
        this.mode = "Edit";
        var dictionaryTree = jsObject.options.dictionaryTree;
        this.currentParent = dictionaryTree.getCurrentColumnParent();

        if (typeof (this.column) == "string") {
            var parentObject = (this.currentParent.type == "BusinessObject")
                ? jsObject.GetBusinessObjectByNameFromDictionary(dictionaryTree.selectedItem.getBusinessObjectFullName())
                : jsObject.GetDataSourceByNameFromDictionary(this.currentParent.name);
            this.column = jsObject.ColumnObject(this.column == "calcColumn", parentObject ? parentObject.columns : null);
            this.column.isDataTransformationColumn = parentObject.typeItem == "DataSource" && parentObject.typeDataSource == "StiDataTransformation";
            this.mode = "New";
        }

        this.caption.innerHTML = jsObject.loc.FormDictionaryDesigner[(this.column.isCalcColumn ? "CalcColumn" : "Column") + this.mode];
        this.editableDictionaryItem = this.mode == "Edit" && jsObject.options.dictionaryTree ? jsObject.options.dictionaryTree.selectedItem : null;
        textValue.innerHTML = this.column.isDataTransformationColumn ? jsObject.loc.PropertyMain.Expression : jsObject.loc.PropertyMain.Value;

        this.nameInSourceControl.value = this.column.nameInSource;
        this.nameControl.focus();
        this.nameControl.value = this.column.name;
        this.aliasControl.value = this.column.alias;
        this.typeControl.setKey(this.column.type);
        this.valueControlRow.style.display = this.column.isCalcColumn || this.column.isDataTransformationColumn ? "" : "none";
        this.valueControl.value = StiBase64.decode(this.column.expression);
        this.valueControl.addInsertButton();

        var propertiesPanel = jsObject.options.propertiesPanel;
        if (propertiesPanel) {
            this.propertiesPanelIsEnabled = propertiesPanel.isEnabled;
            propertiesPanel.setDictionaryMode(true);
            propertiesPanel.setEnabled(true);
        }
    }

    editColumnForm.action = function () {
        this.column.mode = this.mode;
        this.column.currentParentType = this.currentParent.type;
        this.column.currentParentName = this.column.currentParentType == "DataSource"
            ? this.currentParent.name
            : (this.editableDictionaryItem || jsObject.options.dictionaryTree.selectedItem).getBusinessObjectFullName();
        if (this.mode == "Edit") this.column["oldName"] = this.column.name;
        this.column.nameInSource = this.nameInSourceControl.value;
        this.column.name = this.nameControl.value;
        this.column.alias = this.aliasControl.value;
        this.column.expression = StiBase64.encode(this.valueControl.value);
        this.column.type = this.typeControl.key;

        if (!this.nameControl.checkNotEmpty(jsObject.loc.PropertyMain.Name)) return;
        this.changeVisibleState(false);

        jsObject.SendCommandCreateOrEditColumn(this.column);
    }

    return editColumnForm;
}