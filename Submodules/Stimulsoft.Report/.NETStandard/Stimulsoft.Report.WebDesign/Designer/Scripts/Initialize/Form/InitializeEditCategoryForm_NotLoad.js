
StiMobileDesigner.prototype.InitializeEditCategoryForm_ = function () {
    var jsObject = this;

    var editCategoryForm = this.BaseForm("editCategoryForm", this.loc.PropertyMain.Category, 3);
    editCategoryForm.category = null;
    editCategoryForm.mode = "Edit";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "5px 10px 5px 0";
    editCategoryForm.container.appendChild(innerTable);

    innerTable.addTextCell(this.loc.PropertyMain.Category).className = "stiDesignerCaptionControlsBigIntervals";
    var categoryControl = editCategoryForm.categoryControl = this.TextBox("editCategoryFormCategoryControl", 250);
    innerTable.addCell(categoryControl).className = "stiDesignerControlCellsBigIntervals2";

    var showOnParamsRow = innerTable.addRow();
    innerTable.addCellInLastRow();
    var showOnParamsControl = this.CheckBox(null, this.loc.Report.ShowOnParametersPanel);
    showOnParamsControl.style.margin = "6px 0 6px 0";
    innerTable.addCellInLastRow(showOnParamsControl).className = "stiDesignerControlCellsBigIntervals2";

    var collapsedRow = innerTable.addRow();
    innerTable.addCellInLastRow();
    var collapsedControl = this.CheckBox(null, this.loc.PropertyMain.Collapsed);
    innerTable.addCellInLastRow(collapsedControl).className = "stiDesignerControlCellsBigIntervals2";

    var switchPanel = document.createElement("div");
    switchPanel.style.textAlign = "right";
    switchPanel.style.padding = "12px 0 12px 0";
    switchPanel.appendChild(this.FormSeparator());
    editCategoryForm.container.appendChild(switchPanel);

    switchPanel.updateVisibleState = function () {
        this.style.display = jsObject.options.report.properties.parametersOrientation == "Horizontal" && showOnParamsControl.isChecked ? "" : "none";
    }

    var textPanel = document.createElement("div");
    textPanel.className = "stiCreateDataHintText";
    textPanel.style.maxWidth = "400px";
    textPanel.style.padding = "12px";
    textPanel.style.lineHeight = "1.3";
    textPanel.style.textAlign = "justify";
    textPanel.innerHTML = this.loc.Messages.SwitchParametersOrientation.replace("{0}", this.loc.PropertyEnum.StiOrientationVertical);
    switchPanel.appendChild(textPanel);

    var switchButton = this.FormButton(null, null, this.loc.Buttons.SwitchTo.replace("{0}", this.loc.PropertyEnum.StiOrientationVertical));
    switchButton.style.margin = "8px 12px 8px 0";
    switchButton.style.display = "inline-block";
    switchPanel.appendChild(switchButton);

    switchButton.action = function () {        
        jsObject.options.report.properties.parametersOrientation = "Vertical";        
        jsObject.SendCommandSetReportProperties(["parametersOrientation"]);
        switchPanel.updateVisibleState();
    }

    showOnParamsControl.action = function () {
        collapsedControl.setEnabled(this.isChecked);
        switchPanel.updateVisibleState();
    }

    editCategoryForm.onshow = function () {
        this.mode = "Edit";
        if (this.category == null) {
            this.category = jsObject.CategoryObject();
            this.mode = "New";
        }
        this.caption.innerHTML = jsObject.loc.FormDictionaryDesigner["Category" + this.mode];

        showOnParamsRow.style.display = collapsedRow.style.display = this.mode == "Edit" && jsObject.options.dictionaryTree.selectedItem.parent.itemObject.typeItem != "VariablesMainItem" ? "none" : "";

        showOnParamsControl.setChecked(this.category.requestFromUser);
        collapsedControl.setChecked(this.category.readOnly);
        collapsedControl.setEnabled(showOnParamsControl.isChecked);
        switchPanel.updateVisibleState();

        categoryControl.value = this.category.name;
        categoryControl.hideError();
        categoryControl.focus();
    }

    editCategoryForm.action = function () {
        this.category.mode = this.mode;

        if (!categoryControl.checkNotEmpty(jsObject.loc.PropertyMain.Category))
            return;

        if ((this.mode == "New" || categoryControl.value != this.category.name) && !categoryControl.checkExists(jsObject.GetVariableCategoriesFromDictionary(jsObject.options.report.dictionary), "name"))
            return;

        if (this.mode == "Edit")
            this.category.oldName = this.category.name;

        this.category.name = categoryControl.value;
        this.category.requestFromUser = showOnParamsControl.isChecked;
        this.category.readOnly = collapsedControl.isEnabled && collapsedControl.isChecked;

        this.changeVisibleState(false);

        if (this.mode == "Edit")
            jsObject.SendCommandEditCategory(this.category);
        else
            jsObject.SendCommandCreateVariablesCategory(this.category);
    }

    return editCategoryForm;
}