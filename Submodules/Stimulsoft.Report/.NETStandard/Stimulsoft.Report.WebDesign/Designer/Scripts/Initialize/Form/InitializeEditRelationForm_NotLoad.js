
StiMobileDesigner.prototype.InitializeEditRelationForm_ = function () {

    //Edit Connection Form
    var editRelationForm = this.BaseForm("editRelationForm", this.loc.PropertyMain.Relation, 3, this.HelpLinks["relationEdit"]);
    editRelationForm.buttonsPanel.className = "";
    var buttonsTable = editRelationForm.buttonsPanel.firstChild;
    buttonsTable.style.width = "100%";

    var tdWidth = document.createElement("td");
    buttonsTable.firstChild.insertBefore(tdWidth, buttonsTable.firstChild.firstChild);

    var buttonSaveCopy = this.FormButton(editRelationForm, "editRelationFormSaveCopy", this.loc.Buttons.SaveCopy.replace("&", ""), null);
    editRelationForm.buttonSaveCopy = buttonSaveCopy;
    var td = buttonsTable.addCell(buttonSaveCopy);
    td.style.width = "1px";
    td.style.paddingLeft = "15px";
    td.parentNode.insertBefore(td, td.parentNode.firstChild);

    buttonSaveCopy.action = function () {
        editRelationForm.action(true);
    }

    editRelationForm.buttonOk.parentNode.style.width = "1px";
    editRelationForm.buttonCancel.parentNode.style.width = "1px";
    editRelationForm.buttonCancel.parentNode.style.paddingRight = "4px";

    editRelationForm.relation = null;
    editRelationForm.mode = "Edit";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "6px 0 6px 0";
    editRelationForm.container.appendChild(innerTable);

    var textBoxes = [
        ["nameInSource", this.loc.PropertyMain.NameInSource],
        ["name", this.loc.PropertyMain.Name],
        ["alias", this.loc.PropertyMain.Alias]
    ]

    for (var i = 0; i < textBoxes.length; i++) {
        var textCell = i == 0 ? innerTable.addCell() : innerTable.addCellInNextRow();
        textCell.className = "stiDesignerCaptionControlsBigIntervals";
        textCell.innerHTML = textBoxes[i][1];

        var control = editRelationForm[textBoxes[i][0] + "Control"] = this.TextBox("editRelationForm" + textBoxes[i][0], 220);

        if (i == 0)
            innerTable.addCell(control).className = "stiDesignerControlCellsBigIntervals2";
        else
            innerTable.addCellInLastRow(control).className = "stiDesignerControlCellsBigIntervals2";

        control.onkeyup = function () {
            editRelationForm.checkErrors();
        }
    }

    editRelationForm.nameControl.action = function () {
        if (this.oldValue == editRelationForm.aliasControl.value) {
            editRelationForm.aliasControl.value = this.value;
        }
    }

    editRelationForm.container.appendChild(this.FormBlockHeader(this.loc.Export.Settings));
    var dataSourceTable = this.CreateHTMLTable();
    dataSourceTable.style.margin = "5px 0px 0px 15px";
    editRelationForm.container.appendChild(dataSourceTable);

    var cp = dataSourceTable.addTextCell(this.loc.FormDictionaryDesigner.Parent);
    cp.className = "stiDesignerCaptionControlsBigIntervals";
    cp.style.padding = "3px 0px 3px 0px";
    cp = dataSourceTable.addTextCell(this.loc.FormDictionaryDesigner.Child);
    cp.className = "stiDesignerCaptionControlsBigIntervals";
    cp.style.padding = "3px 0px 3px 0px";
    dataSourceTable.addRow();

    var containers = ["parentColumns", "childColumns"];
    var buttons = ["Left", "LeftAll", "Right", "RightAll"];
    var dropDownLists = [
        ["parentDataSource", this.loc.PropertyMain.ParentSource],
        ["childDataSource", this.loc.PropertyMain.ChildSource]
    ];

    for (var i = 0; i < dropDownLists.length; i++) {
        editRelationForm[dropDownLists[i][0] + "Control"] = this.DropDownList("editRelationForm" + dropDownLists[i][0], 220, null, null, true);
        dataSourceTable.addCellInLastRow(editRelationForm[dropDownLists[i][0] + "Control"]).className = "stiDesignerControlCellsBigIntervals";

        editRelationForm[dropDownLists[i][0] + "Control"].action = function () {
            //editRelationForm.checkErrors();
            var dataSource = this.jsObject.GetDataSourceByNameFromDictionary(this.key);
            var columns = this.jsObject.GetColumnsItemsFromDictionary(dataSource);
            if (this.name == "editRelationFormparentDataSource") {
                editRelationForm.parentColumnsContainer.addColumns(columns);
            }
            else {
                editRelationForm.childColumnsContainer.addColumns(columns);
            }
        }
    }

    dataSourceTable.addRow();
    for (var i = 0; i < containers.length; i++) {
        var allColumns = containers[i] + "Container";
        editRelationForm[allColumns] = this.Container(allColumns, 224, 200, true);
        editRelationForm[allColumns].multiSelection = true;
        editRelationForm[allColumns].className = "stiDesignerRelationFormColumnsContainer";
        dataSourceTable.addCellInLastRow(editRelationForm[allColumns]);
        editRelationForm[allColumns].onChange = function () {
            editRelationForm.checkErrors();
        }
        editRelationForm[allColumns].addColumns = function (columns) {
            this.clear();
            if (columns) {
                for (var i = 0; i < columns.length; i++) {
                    this.addItemAndNotAction(columns[i].caption, columns[i]);
                }
            }
        }
    }

    editRelationForm.container.appendChild(editRelationForm["activeRelationCheckbox"] = this.CheckBox(null, this.loc.Report.ActiveRelation, this.loc.HelpDesigner.ActiveRelation));
    editRelationForm["activeRelationCheckbox"].style.margin = "7px 0px 7px 15px";

    var errorBlock = document.createElement("div");
    errorBlock.className = "stiDesignerRelationFormErrorBlock";
    var errorBlockCaption = document.createElement("div");
    errorBlock.caption = errorBlockCaption;
    errorBlockCaption.style.padding = "6px 6px 6px 15px";
    errorBlock.appendChild(errorBlockCaption);
    editRelationForm.container.appendChild(errorBlock);
    editRelationForm.errorBlock = errorBlock;
    errorBlock.style.display = "none";

    editRelationForm.checkErrors = function () {
        if (this.checkEmptyNameInSourceError()) return true;
        if (this.checkEmptyNameError()) return true;
        if (this.checkEmptyAliasError()) return true;
        if (this.checkDuplicateNamesError()) return true;
        if (this.checkDuplicateNamesInSourceError()) return true;
        if (this.checkIdenticalColumnsError()) return true;
        if (this.checkColumnTypesError()) return true;

        if (!this.firstCheckInNewForm) {
            if (this.checkColumnCountError()) return true;
            if (this.checkParentDataSource()) return true;
            if (this.checkChildDataSourceErrors()) return true;
            if (this.checkParentColumnsError()) return true;
            if (this.checkChildColumnsError()) return true;
        }

        editRelationForm.hideErrorMessage();

        return false;
    }

    editRelationForm.checkEmptyNameInSourceError = function () {
        if (this.nameInSourceControl.value.length != 0) return false;
        editRelationForm.showErrorMessage(this.jsObject.loc.Errors.FieldRequire.replace("{0}", this.jsObject.loc.PropertyMain.NameInSource));
        return true;
    }

    editRelationForm.checkEmptyNameError = function () {
        if (this.nameControl.value.length != 0) return false;
        editRelationForm.showErrorMessage(this.jsObject.loc.Errors.FieldRequire.replace("{0}", this.jsObject.loc.PropertyMain.Name));
        return true;
    }

    editRelationForm.checkEmptyAliasError = function () {
        if (this.aliasControl.value.length != 0) return false;
        editRelationForm.showErrorMessage(this.jsObject.loc.Errors.FieldRequire.replace("{0}", this.jsObject.loc.PropertyMain.Alias));
        return true;
    }

    editRelationForm.checkDuplicateNamesError = function () {
        if (this.nameControl.value == this.relationOriginalName) return false;
        var childDataSource = this.jsObject.GetDataSourceByNameFromDictionary(this.childDataSourceControl.key);
        if (childDataSource) {
            for (var i = 0; i < childDataSource.relations.length; i++) {
                if (childDataSource.relations[i].name == this.nameControl.value) {
                    editRelationForm.showErrorMessage(this.jsObject.loc.Errors.NameExists.replace("{0}", this.nameControl.value));
                    return true;
                }
            }
        }
        return false;
    }

    editRelationForm.checkDuplicateNamesInSourceError = function () {
        if (this.nameInSourceControl.value == this.relationOriginalNameInSource) return false;
        var childDataSource = this.jsObject.GetDataSourceByNameFromDictionary(this.childDataSourceControl.key);
        if (childDataSource) {
            for (var i = 0; i < childDataSource.relations.length; i++) {
                if (childDataSource.relations[i].nameInSource == this.nameInSourceControl.value) {
                    editRelationForm.showErrorMessage(this.jsObject.loc.Errors.NameExists.replace("{0}", this.nameInSourceControl.value));
                    return true;
                }
            }
        }
        return false;
    }

    editRelationForm.checkParentDataSource = function () {
        if (this.parentDataSourceControl.key.length != 0) return false;
        editRelationForm.showErrorMessage(this.jsObject.loc.Errors.FieldRequire.replace("{0}", this.jsObject.loc.PropertyMain.ParentSource));
        return true;
    }

    editRelationForm.checkChildDataSourceErrors = function () {
        if (this.childDataSourceControl.key.length != 0) return false;
        editRelationForm.showErrorMessage(this.jsObject.loc.Errors.FieldRequire.replace("{0}", this.jsObject.loc.PropertyMain.ChildSource));
        return true;
    }

    editRelationForm.checkParentColumnsError = function () {
        if (this.parentColumnsContainer.selectedItems.length > 0) return false;
        editRelationForm.showErrorMessage(this.jsObject.loc.Errors.FieldRequire.replace("{0}", this.jsObject.loc.PropertyMain.ParentColumns));
        return true;
    }

    editRelationForm.checkChildColumnsError = function () {
        if (this.childColumnsContainer.selectedItems.length > 0) return false;
        editRelationForm.showErrorMessage(this.jsObject.loc.Errors.FieldRequire.replace("{0}", this.jsObject.loc.PropertyMain.ChildColumns));
        return true;
    }

    editRelationForm.checkIdenticalColumnsError = function () {
        if (this.parentDataSourceControl.key != this.childDataSourceControl.key) return false;
        if (this.parentColumnsContainer.selectedItems.length != this.childColumnsContainer.selectedItems.length) return false;

        for (var index = 0; index < this.parentColumnsContainer.selectedItems.length; index++) {
            var column1 = this.parentColumnsContainer.selectedItems[index].itemObject;
            var column2 = this.childColumnsContainer.selectedItems[index].itemObject;

            if (column1.name == column2.name && column1.type == column2.type) {
                editRelationForm.showErrorMessage("ParentKey and ChildKey are identical.");
                return true;
            }
        }
        return false;
    }

    editRelationForm.checkColumnTypesError = function () {
        if (this.parentColumnsContainer.selectedItems.length != this.childColumnsContainer.selectedItems.length) return false;

        for (var index = 0; index < this.parentColumnsContainer.selectedItems.length; index++) {
            var parent = this.parentColumnsContainer.selectedItems[index].itemObject;
            var child = this.childColumnsContainer.selectedItems[index].itemObject;

            if (parent.type != child.type) {
                editRelationForm.showErrorMessage("The '" + parent.type + "' type of the '" + parent.key + "' column is not equal to '" + child.type + "' type of '" + child.key + "'.");
                return true;
            }
        }
        return false;
    }

    editRelationForm.checkColumnCountError = function () {
        if (this.parentColumnsContainer.selectedItems.length == this.childColumnsContainer.selectedItems.length) return false;
        editRelationForm.showErrorMessage("The count of parent columns is not equal to the count of child columns!");
        return true;
    }

    editRelationForm.showErrorMessage = function (errorStr) {
        this.errorBlock.caption.innerHTML = errorStr;
        this.errorBlock.style.display = "";
        editRelationForm.buttonOk.setEnabled(false);
        editRelationForm.buttonSaveCopy.setEnabled(false);
    }

    editRelationForm.hideErrorMessage = function (errorStr) {
        this.errorBlock.style.display = "none";
        editRelationForm.buttonOk.setEnabled(true);
        editRelationForm.buttonSaveCopy.setEnabled(true);
    }

    editRelationForm.onshow = function () {
        this.mode = "Edit";
        if (this.relation == null) { this.relation = this.jsObject.RelationObject(); this.mode = "New"; }
        var caption = this.jsObject.loc.FormDictionaryDesigner["Relation" + this.mode];
        this.caption.innerHTML = caption;
        this.nameInSourceControl.value = this.relation.nameInSource;
        this.nameControl.hideError();
        this.nameControl.focus();
        this.nameControl.value = this.relation.name;
        this.relationOriginalName = this.relation.name;
        this.relationOriginalNameInSource = this.mode == "New" ? "" : this.relation.nameInSource;
        this.aliasControl.value = this.relation.alias;
        this.activeRelationCheckbox.setChecked(this.relation.active);
        this.firstCheckInNewForm = true;
        editRelationForm.buttonSaveCopy.style.visibility = this.mode == "New" ? "hidden" : "";

        var allDataSources = this.jsObject.GetDataSourcesFromDictionary(this.jsObject.options.report.dictionary);
        var allDataSourcesItems = [];
        for (var i = 0; i < allDataSources.length; i++) {
            allDataSourcesItems.push(this.jsObject.Item(allDataSources[i].name, allDataSources[i].name, null, allDataSources[i].name));
        }
        allDataSourcesItems.sort(this.jsObject.SortByName);
        this.parentDataSourceControl.items = allDataSourcesItems;
        this.childDataSourceControl.items = allDataSourcesItems;

        this.parentDataSourceControl.setKey(this.mode == "Edit" ? this.relation.parentDataSource : "");
        var currentParent = this.jsObject.options.dictionaryTree.getCurrentColumnParent();
        this.childDataSourceControl.setKey(this.mode == "Edit" ? this.relation.childDataSource : currentParent.name);

        var childDataSource = this.jsObject.GetDataSourceByNameFromDictionary(this.childDataSourceControl.key);
        var parentDataSource = this.jsObject.GetDataSourceByNameFromDictionary(this.parentDataSourceControl.key);
        var childColumns = this.jsObject.GetColumnsItemsFromDictionary(childDataSource);
        var parentColumns = this.jsObject.GetColumnsItemsFromDictionary(parentDataSource);

        this.parentColumnsContainer.clear();
        this.childColumnsContainer.clear();
        this.parentColumnsContainer.addColumns(parentColumns);
        this.childColumnsContainer.addColumns(childColumns);

        for (var i = 0; i < this.relation.parentColumns.length; i++) {
            for (var j = 0; j < this.parentColumnsContainer.items.length; j++)
                if (this.relation.parentColumns[i] == this.parentColumnsContainer.items[j].name) {
                    this.parentColumnsContainer.selectedItems.push(this.parentColumnsContainer.items[j]);
                    this.parentColumnsContainer.items[j].setSelected(true);
                }
        }
        for (var i = 0; i < this.relation.childColumns.length; i++) {
            for (var j = 0; j < this.childColumnsContainer.items.length; j++)
                if (this.relation.childColumns[i] == this.childColumnsContainer.items[j].name) {
                    this.childColumnsContainer.selectedItems.push(this.childColumnsContainer.items[j]);
                    this.childColumnsContainer.items[j].setSelected(true);
                }
        }
        this.parentColumnsContainer.updateNumaration();
        this.childColumnsContainer.updateNumaration();

        this.checkErrors();
    }

    editRelationForm.action = function (copyModeActivated) {
        this.firstCheckInNewForm = false;
        if (this.checkErrors()) return;

        this.relation["mode"] = this.mode;
        if (this.mode == "Edit") this.relation["oldNameInSource"] = this.relation.nameInSource;
        this.relation.nameInSource = this.nameInSourceControl.value;
        this.relation.name = this.nameControl.value;
        this.relation.alias = this.aliasControl.value;
        this.relation.active = this.activeRelationCheckbox.isChecked;
        this.relation["changedChildDataSource"] = this.relation.childDataSource != this.childDataSourceControl.key;
        this.relation.parentDataSource = this.parentDataSourceControl.key;
        this.relation.childDataSource = this.childDataSourceControl.key;
        this.relation.childColumns = [];
        this.relation.parentColumns = [];
        this.relation.copyModeActivated = copyModeActivated ? true : false;

        var props = ["parentColumns", "childColumns"];
        for (var i = 0; i < props.length; i++) {
            var selectedItems = this[props[i] + "Container"].selectedItems;
            if (selectedItems) {
                for (var k = 0; k < selectedItems.length; k++) {
                    this.relation[props[i]].push(selectedItems[k].name);
                }
            }
        }

        if (!this.nameControl.checkNotEmpty(this.jsObject.loc.PropertyMain.Name)) return;
        this.changeVisibleState(false);
        this.jsObject.SendCommandCreateOrEditRelation(this.relation);
    }

    return editRelationForm;
}