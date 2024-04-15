
StiMobileDesigner.prototype.InitializeEditDataSourceForm_ = function () {
    var jsObject = this;
    var editDataSourceForm = this.BaseForm("editDataSourceForm", this.loc.PropertyMain.DataSource, 3, this.HelpLinks["dataSourceEdit"]);
    editDataSourceForm.datasource = null;
    editDataSourceForm.mode = "Edit";

    var saveCopyButton = this.FormButton(null, null, this.loc.Buttons.SaveCopy, null);
    saveCopyButton.style.display = "inline-block";
    saveCopyButton.style.margin = "12px";

    saveCopyButton.action = function () {
        if (!editDataSourceForm.nameControl.checkExists(jsObject.GetDataSourcesFromDictionary(jsObject.options.report.dictionary), "name")) {
            if (editDataSourceForm.nameControl.value == editDataSourceForm.aliasControl.value) {
                editDataSourceForm.aliasControl.value += "Copy";
            }
            editDataSourceForm.nameControl.value += "Copy";

            var resultName = editDataSourceForm.nameControl.value;
            var i = 2;
            while (!(editDataSourceForm.nameControl.checkExists(jsObject.GetDataSourcesFromDictionary(jsObject.options.report.dictionary), "name") &&
                editDataSourceForm.nameControl.checkExists(jsObject.GetVariablesFromDictionary(jsObject.options.report.dictionary), "name"))) {
                editDataSourceForm.nameControl.value = editDataSourceForm.aliasControl.value = resultName + i;
                i++;
            }
        }
        editDataSourceForm.mode = "New";
        editDataSourceForm.action();
    }

    var footerTable = this.CreateHTMLTable();
    footerTable.style.width = "100%";

    var buttonsPanel = editDataSourceForm.buttonsPanel;
    editDataSourceForm.removeChild(buttonsPanel);
    editDataSourceForm.appendChild(footerTable);

    footerTable.addCell(saveCopyButton).style.width = "1px";
    footerTable.addCell();
    footerTable.addCell(editDataSourceForm.buttonOk).style.width = "1px";
    footerTable.addCell(editDataSourceForm.buttonCancel).style.width = "1px";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "5px 0 5px 0";
    editDataSourceForm.container.appendChild(innerTable);

    var textBoxes = [
        ["nameInSource", this.loc.PropertyMain.NameInSource],
        ["category", this.loc.PropertyMain.Category],
        ["name", this.loc.PropertyMain.Name],
        ["alias", this.loc.PropertyMain.Alias]
    ]

    for (var i = 0; i < textBoxes.length; i++) {
        editDataSourceForm[textBoxes[i][0] + "ControlRow"] = innerTable.addRow();

        var text = innerTable.addCellInLastRow();
        text.className = "stiDesignerCaptionControlsBigIntervals";
        text.innerHTML = textBoxes[i][1];

        var textBox = editDataSourceForm[textBoxes[i][0] + "Control"] = (textBoxes[i][0] == "nameInSource")
            ? this.TextBoxWithEditButton("editDataSourceForm" + textBoxes[i][0], 320)
            : this.TextBox("editDataSourceForm" + textBoxes[i][0], 230);

        innerTable.addCellInLastRow(textBox).className = "stiDesignerControlCellsBigIntervals2";
    }

    editDataSourceForm.nameInSourceControl.button.action = function () {
        var editDataSourceForm = jsObject.options.forms.editDataSourceForm;
        jsObject.SendCommandGetAllConnections(editDataSourceForm.datasource["typeDataAdapter"]);
    }

    //Query Group
    var buttons = [
        ["runQueryScript", null, "Query.RunQuery.png", this.loc.FormDictionaryDesigner.Run],
        ["editQueryScript", null, "Query.SQL.png", this.loc.FormDictionaryDesigner.EditQuery],
        ["viewData", null, "Query.ViewData.png", this.loc.FormDictionaryDesigner.ViewData]
    ]

    editDataSourceForm.queryPanel = document.createElement("div");
    editDataSourceForm.container.appendChild(editDataSourceForm.queryPanel);
    editDataSourceForm.queryToolBar = this.CreateHTMLTable();
    editDataSourceForm.queryToolBar.style.margin = "5px 6px 5px 12px";


    var queryHeader = this.FormBlockHeader(this.loc.QueryBuilder.Query);
    queryHeader.style.margin = "0 12px 0 12px";
    editDataSourceForm.queryPanel.appendChild(queryHeader);
    editDataSourceForm.queryPanel.appendChild(editDataSourceForm.queryToolBar);

    for (var i = 0; i < buttons.length; i++) {
        var button = this.SmallButton(null, null, buttons[i][1], buttons[i][2], buttons[i][3], null, null, true);
        button.style.marginRight = "2px";
        editDataSourceForm.queryToolBar[buttons[i][0]] = button;
        editDataSourceForm.queryToolBar.addCell(button);
    }

    //Query Text
    var queryTextGroup = document.createElement("div");

    editDataSourceForm.queryPanel.appendChild(queryTextGroup);
    var queryTextControl = editDataSourceForm.queryTextControl = this.TextArea("editDataSourceFormQueryTextControl");
    queryTextControl.style.width = "calc(100% - 30px)";
    queryTextControl.style.margin = "0 12px 0 12px";
    queryTextControl.style.minHeight = "100px";
    queryTextControl.style.resize = "auto";
    queryTextGroup.appendChild(queryTextControl);

    var sqlPropertiesTable = this.CreateHTMLTable();
    sqlPropertiesTable.style.margin = "6px 0 6px 0";
    queryTextGroup.appendChild(sqlPropertiesTable);

    //Type
    sqlPropertiesTable.addTextCell(this.loc.PropertyMain.Type).className = "stiDesignerCaptionControlsBigIntervals";
    editDataSourceForm.queryTextTypeControl = this.DropDownList("editDataSourceFormQueryTextType", 160, null, this.GetQueryTextTypeItems(), true);
    sqlPropertiesTable.addCell(editDataSourceForm.queryTextTypeControl).className = "stiDesignerControlCellsBigIntervals2";
    editDataSourceForm.queryTextTypeControl.setKey("Table");

    //Command Timeout
    sqlPropertiesTable.addTextCellInNextRow(this.loc.FormDictionaryDesigner.QueryTimeout).className = "stiDesignerCaptionControlsBigIntervals";
    var commandTimeoutControl = editDataSourceForm.commandTimeoutControl = this.TextBoxEnumerator("editDataSourceFormCommandTimeout", 160, null, null, 10000000, 0);
    commandTimeoutControl.style.margin = "2px 0 2px 0";
    sqlPropertiesTable.addCellInLastRow(commandTimeoutControl).className = "stiDesignerControlCellsBigIntervals2";
    commandTimeoutControl.setValue(30);

    //ReconnectOnEachRow
    sqlPropertiesTable.addCellInNextRow();
    var reconnectOnEachRowControl = editDataSourceForm.reconnectOnEachRowControl = this.CheckBox("editDataSourceFormReconnectOnEachRow", this.loc.PropertyMain.ReconnectOnEachRow);
    reconnectOnEachRowControl.style.margin = "2px 0 2px 0";
    sqlPropertiesTable.addCellInLastRow(reconnectOnEachRowControl).className = "stiDesignerControlCellsBigIntervals2";
    reconnectOnEachRowControl.setChecked(false);

    editDataSourceForm.getFormParameters = function () {
        var formParameters = {
            databaseObject: editDataSourceForm.databaseObject,
            nameInSource: editDataSourceForm.nameInSourceControl.textBox.value,
            name: editDataSourceForm.nameControl.value,
            oldName: editDataSourceForm.datasource.oldName,
            alias: editDataSourceForm.aliasControl.value,
            sqlCommand: StiBase64.encode(queryTextControl.value),
            commandTimeout: editDataSourceForm.commandTimeoutControl.textBox.value,
            type: editDataSourceForm.queryTextTypeControl.key,
            typeDataAdapter: editDataSourceForm.datasource.typeDataAdapter,
            mode: editDataSourceForm.mode,
            typeItem: editDataSourceForm.datasource.typeItem,
            columns: [],
            parameters: []
        }

        for (var key in editDataSourceForm.columnsAndParametersTree.columnsItem.childs)
            formParameters.columns.push(editDataSourceForm.columnsAndParametersTree.columnsItem.childs[key].itemObject);
        for (var key in editDataSourceForm.columnsAndParametersTree.parametersItem.childs)
            formParameters.parameters.push(editDataSourceForm.columnsAndParametersTree.parametersItem.childs[key].itemObject);

        return formParameters;
    }

    //edit query button
    editDataSourceForm.queryToolBar.editQueryScript.action = function () {
        jsObject.InitializeTextEditorFormOnlyText(function (textEditorOnlyText) {
            textEditorOnlyText.controlTextBox = queryTextControl;

            textEditorOnlyText.showFunction = function () {
                this.textArea.value = queryTextControl.value;
            }

            textEditorOnlyText.actionFunction = function () {
                queryTextControl.value = this.textArea.value;
            }

            textEditorOnlyText.changeVisibleState(true);
        });
    }

    //run query button
    editDataSourceForm.queryToolBar.runQueryScript.action = function () {
        editDataSourceForm.checkParametersValuesAndShowValuesForm(function (params) {
            jsObject.SendCommandRunQueryScript(params);
        });
    }

    //view data button
    editDataSourceForm.queryToolBar.viewData.action = function () {
        editDataSourceForm.checkParametersValuesAndShowValuesForm(function (params) {
            jsObject.InitializeViewDataForm(function (viewDataForm) {
                viewDataForm.show(params);
            });
        });
    }

    //Columns Group
    var buttons = [
        ["columnNew", null, "ColumnNew.png", this.loc.FormDictionaryDesigner.ColumnNew],
        ["calculatedColumnNew", null, "CalcColumnNew.png", this.loc.FormDictionaryDesigner.CalcColumnNew],
        ["parameterNew", null, "ParameterNew.png", this.loc.FormDictionaryDesigner.DataParameterNew],
        ["removeItem", null, "Remove.png", this.loc.MainMenu.menuEditDelete.replace("&", "")],
        ["viewQuery", null, "Query.ViewData.png", this.loc.FormDictionaryDesigner.ViewData],
        ["retrieveColumns", this.loc.FormDictionaryDesigner.RetrieveColumns, "RetrieveColumnsArrow.png", null],
        ["retrieveColumnsAndParameters", null, "RetrieveColumns.png", null]
    ]

    editDataSourceForm.columnToolBar = this.CreateHTMLTable();
    editDataSourceForm.columnToolBar.style.margin = "5px 6px 5px 12px";

    var columnsHeader = editDataSourceForm.columnsHeader = this.FormBlockHeader(this.loc.FormPageSetup.Columns + " & " + this.loc.PropertyMain.Parameters);
    columnsHeader.style.margin = "0 12px 0 12px";
    editDataSourceForm.container.appendChild(columnsHeader);
    editDataSourceForm.container.appendChild(editDataSourceForm.columnToolBar);

    for (var i = 0; i < buttons.length; i++) {
        var button = this.SmallButton(buttons[i][0], null, buttons[i][1], buttons[i][2], buttons[i][3], buttons[i][0] == "retrieveColumnsAndParameters" ? "Down" : null, null, true);
        button.style.marginRight = "2px";
        editDataSourceForm.columnToolBar[buttons[i][0]] = button;
        editDataSourceForm.columnToolBar.addCell(button);

        if (buttons[i][0] == "removeItem") {
            editDataSourceForm.columnToolBar.addCell(this.HomePanelSeparator()).style.paddingRight = "2px";
        }
    }

    var columnsAndParametersMenu = this.ColumnsAndParametersMenu();
    editDataSourceForm.columnToolBar.retrieveColumnsAndParameters.action = function () {
        columnsAndParametersMenu.changeVisibleState(!columnsAndParametersMenu.visible);
    }
    if (this.options.isJava) {
        editDataSourceForm.columnToolBar.retrieveColumnsAndParameters.style.opacity = 0;
    }

    columnsAndParametersMenu.onshow = function () {
        this.items["retrieveParameters"].setEnabled(editDataSourceForm.queryTextTypeControl.key == "StoredProcedure");
    }

    editDataSourceForm.columnToolBar.removeItem.action = function () {
        var columnsAndParametersTree = editDataSourceForm.columnsAndParametersTree;
        if (columnsAndParametersTree.selectedItem) {
            if (columnsAndParametersTree.selectedItem == columnsAndParametersTree.columnsItem ||
                columnsAndParametersTree.selectedItem == columnsAndParametersTree.parametersItem) {
                columnsAndParametersTree.selectedItem.removeAllChilds();
            }
            else {
                editDataSourceForm.columnsAndParametersTree.selectedItem.remove();
            }
        }
    }

    editDataSourceForm.columnToolBar.columnNew.action = function () {
        var newColumn = jsObject.ColumnObject(false, editDataSourceForm.columnsAndParametersTree.getItemObjects("Column"));
        editDataSourceForm.columnsAndParametersTree.addItem(newColumn, true);
    }

    editDataSourceForm.columnToolBar.calculatedColumnNew.action = function () {
        var newColumn = jsObject.ColumnObject(true, editDataSourceForm.columnsAndParametersTree.getItemObjects("Column"));
        editDataSourceForm.columnsAndParametersTree.addItem(newColumn, true);
    }

    editDataSourceForm.columnToolBar.parameterNew.action = function () {
        var newParameter = jsObject.ParameterObject(editDataSourceForm.columnsAndParametersTree.getItemObjects("Parameter"));
        if (editDataSourceForm.datasource && editDataSourceForm.datasource.parameterTypes) {
            for (var i = 0; i < editDataSourceForm.datasource.parameterTypes.length; i++) {
                if (i == 0) newParameter.type = editDataSourceForm.datasource.parameterTypes[0].typeValue.toString();
                if (editDataSourceForm.datasource.parameterTypes[i].typeName == "Text") {
                    newParameter.type = editDataSourceForm.datasource.parameterTypes[i].typeValue.toString();
                    break;
                };
            }
        }
        editDataSourceForm.columnsAndParametersTree.addItem(newParameter, true);
    }

    editDataSourceForm.columnToolBar.retrieveColumns.action = function () {
        editDataSourceForm.checkParametersValuesAndShowValuesForm(function (params) {
            if (columnsAndParametersMenu.retrieveColumnsAllowRun.isChecked) params.retrieveColumnsAllowRun = true;
            jsObject.SendCommandRetrieveColumns(params);
        });
    }

    editDataSourceForm.columnToolBar.viewQuery.action = function () {
        jsObject.InitializeViewDataForm(function (viewDataForm) {
            viewDataForm.show(editDataSourceForm.getFormParameters());
        });
    }

    editDataSourceForm.checkParametersValuesAndShowValuesForm = function (completeFunction) {
        var formParameters = editDataSourceForm.getFormParameters();

        jsObject.SendCommandGetParamsFromQueryString(StiBase64.encode(this.queryTextControl.value), this.datasource.name, function (variablesParams) {
            if (jsObject.GetCountObjects(editDataSourceForm.columnsAndParametersTree.parametersItem.childs) > 0 || (variablesParams && variablesParams.length > 0)) {
                jsObject.InitializeParametersValuesForm(function (parametersValuesForm) {
                    parametersValuesForm.show(formParameters, variablesParams, completeFunction);
                });
            }
            else
                completeFunction(formParameters);
        })
    }

    var permissionDataColumns = this.options.permissionDataColumns;
    var permissionSqlParameters = this.options.permissionSqlParameters;
    editDataSourceForm.columnToolBar.columnNew.setEnabled(!permissionDataColumns || permissionDataColumns.indexOf("All") >= 0 || permissionDataColumns.indexOf("Create") >= 0);
    editDataSourceForm.columnToolBar.parameterNew.setEnabled(!permissionSqlParameters || permissionSqlParameters.indexOf("All") >= 0 || permissionSqlParameters.indexOf("Create") >= 0);
    editDataSourceForm.columnToolBar.calculatedColumnNew.setEnabled(!permissionDataColumns || permissionDataColumns.indexOf("All") >= 0 || permissionDataColumns.indexOf("Create") >= 0);

    //Columns And Parameters Tree
    var columnsContainerTable = this.CreateHTMLTable();
    editDataSourceForm.container.appendChild(columnsContainerTable);
    editDataSourceForm.columnsAndParametersTree = this.ColumnsAndParametersTree(editDataSourceForm, 250, 165);
    columnsContainerTable.addCell(editDataSourceForm.columnsAndParametersTree);

    editDataSourceForm.columnsContainerEditControlsTable = this.CreateHTMLTable();
    columnsContainerTable.addCell(editDataSourceForm.columnsContainerEditControlsTable).style.verticalAlign = "top";

    var controlsColumnsContainer = [
        ["nameInSource", this.loc.PropertyMain.NameInSource],
        ["name", this.loc.PropertyMain.Name],
        ["alias", this.loc.PropertyMain.Alias],
        ["expression", this.loc.PropertyMain.Expression],
        ["size", this.loc.PropertyMain.Size],
        ["type", this.loc.PropertyMain.Type]
    ]

    editDataSourceForm.columnsContainerEditControlsTable.editDataSourceForm = editDataSourceForm;
    editDataSourceForm.columnsContainerEditControlsTable.style.margin = "4px 0 0 12px";
    editDataSourceForm.columnsContainerEditControlsTable.style.minWidth = "320px";

    for (var i = 0; i < controlsColumnsContainer.length; i++) {
        editDataSourceForm[controlsColumnsContainer[i][0] + "ControlRowEditColumn"] = editDataSourceForm.columnsContainerEditControlsTable.addRow();

        var text = editDataSourceForm.columnsContainerEditControlsTable.addCellInLastRow();
        text.className = "stiDesignerCaptionControls";
        text.innerHTML = controlsColumnsContainer[i][1];

        var control = editDataSourceForm[controlsColumnsContainer[i][0] + "ControlEditColumn"] = controlsColumnsContainer[i][0] == "type"
            ? this.DropDownList("editDataSourceForm" + controlsColumnsContainer[i][0] + "ControlEditColumn", 130, null, null, true)
            : this.TextBox("editDataSourceForm" + controlsColumnsContainer[i][0] + "ControlEditColumn", 130);

        control.style.display = "inline-block";

        var cell = editDataSourceForm.columnsContainerEditControlsTable.addCellInLastRow(control);
        cell.className = "stiDesignerControlCellsBigIntervals2";
        cell.style.textAlign = "right";

        control.propertyName = controlsColumnsContainer[i][0];
        control.allwaysEnabled = true;

        control.action = function () {
            var editDataSourceForm = jsObject.options.forms.editDataSourceForm;
            var selectedItem = editDataSourceForm.columnsAndParametersTree.selectedItem;
            if (!selectedItem) return;
            selectedItem.itemObject[this.propertyName] = (this.propertyName) != "type"
                ? (this.propertyName != "expression" ? this.value : StiBase64.encode(this.value))
                : this.key;
            selectedItem.repaint();
        }
    }

    editDataSourceForm.nameControl.action = function () {
        if (this.oldValue == editDataSourceForm.aliasControl.value) {
            editDataSourceForm.aliasControl.value = this.value;
        }
    }

    editDataSourceForm.typeControlEditColumn.menu.innerContent.style.maxHeight = "200px";

    editDataSourceForm.onshow = function () {
        this.mode = "Edit";

        if (typeof (this.datasource) == "string" || this.datasource == null) {
            if (this.datasource == "BusinessObject") {
                this.datasource = jsObject.BusinessObject();
            }
            else {
                this.datasource = jsObject.DataSourceObject(this.datasource, this.nameInSource);
            }
            this.mode = "New";
        }

        saveCopyButton.style.display = this.mode == "Edit" ? "" : "none";

        this.editableDictionaryItem = this.mode == "Edit" && jsObject.options.dictionaryTree ? jsObject.options.dictionaryTree.selectedItem : null;
        this.nameControl.hideError();
        this.nameControl.focus();
        this.nameInSource = null;

        var caption = (this.mode == "New" && this.datasource.typeItem == "BusinessObject")
            ? jsObject.loc.FormDictionaryDesigner.NewBusinessObject
            : jsObject.loc.FormDictionaryDesigner[(this.datasource.typeItem == "DataSource" ? "DataSource" : "BusinessObject") + this.mode];

        if (caption) {
            this.caption.innerHTML = caption;
        }

        var props = ["nameInSource", "category", "name", "alias"];

        for (var i = 0; i < props.length; i++) {
            this[props[i] + "ControlRow"].style.display = this.datasource[props[i]] != null ? "" : "none";
            if (this.datasource[props[i]] != null)
                if (props[i] == "nameInSource") this[props[i] + "Control"].textBox.value = this.datasource[props[i]];
                else this[props[i] + "Control"].value = this.datasource[props[i]];
        }

        var isUndefined = this.datasource.typeDataSource == "StiUndefinedDataSource";
        var showParameters = this.datasource.parameterTypes != null && !isUndefined;

        this.queryPanel.style.display = (this.datasource.sqlCommand != null && this.datasource.type != null && !isUndefined) ? "" : "none";
        this.queryToolBar.style.display = this.datasource.typeDataSource != "StiMongoDbSource" ? "" : "none";
        this.columnToolBar.retrieveColumns.style.display = this.datasource.typeItem == "DataSource" ? "" : "none";
        this.columnToolBar.retrieveColumnsAndParameters.style.display = showParameters && !jsObject.options.jsMode ? "" : "none";
        this.columnToolBar.viewQuery.style.display = this.datasource.typeDataSource == "StiDataTableSource" ? "" : "none";
        this.columnToolBar.parameterNew.style.display = showParameters ? "" : "none";
        this.columnToolBar.calculatedColumnNew.style.display = this.datasource.typeDataSource != "StiMongoDbSource" ? "" : "none";

        sqlPropertiesTable.style.display = this.datasource.typeDataSource != "StiMongoDbSource" ? "" : "none";
        columnsHeader.caption.innerHTML = showParameters ? (jsObject.loc.FormPageSetup.Columns + " & " + jsObject.loc.PropertyMain.Parameters) : jsObject.loc.FormPageSetup.Columns;

        if (this.datasource.sqlCommand != null) this.queryTextControl.value = StiBase64.decode(this.datasource.sqlCommand);
        if (this.datasource.type != null) this.queryTextTypeControl.setKey(this.datasource.type);
        if (this.datasource.commandTimeout != null) this.commandTimeoutControl.setValue(this.datasource.commandTimeout);
        if (this.datasource.reconnectOnEachRow != null) this.reconnectOnEachRowControl.setChecked(this.datasource.reconnectOnEachRow);

        this.businessObjectFullName = null;
        if (this.datasource.typeItem == "BusinessObject") {
            this.businessObjectFullName = jsObject.options.dictionaryTree.selectedItem.getBusinessObjectFullName();
            this.categoryControl.setEnabled(this.businessObjectFullName == null || this.businessObjectFullName.length <= (this.mode == "Edit" ? 1 : 0));
        }
        else {
            this.categoryControl.setEnabled(true);
        }
        var columns = [];
        var parameters = [];

        if (this.mode == "Edit") {
            var currObject = (this.datasource.typeItem == "DataSource")
                ? jsObject.GetDataSourceByNameFromDictionary(this.datasource.name)
                : jsObject.GetBusinessObjectByNameFromDictionary(this.businessObjectFullName);

            if (currObject) {
                if (currObject.columns) {
                    columns = currObject.columns;
                }
                if (currObject.parameters) {
                    parameters = currObject.parameters;
                }
            }
        }

        this.columnsAndParametersTree.addColumnsAndParameters(columns, parameters, true);
        var permissionSqlParameters = jsObject.options.permissionSqlParameters;

        this.columnsAndParametersTree.parametersItem.style.display = (!permissionSqlParameters || permissionSqlParameters.indexOf("All") >= 0 || permissionSqlParameters.indexOf("View") >= 0) && this.datasource.parameterTypes ? "" : "none";
        this.columnsAndParametersTree.onSelectedItem();

        if (this.mode == "New" && this.datasource.sqlCommand != null) {
            jsObject.SendCommandGetSqlParameterTypes(this.datasource);
        }
    }

    editDataSourceForm.action = function () {
        this.datasource["mode"] = this.mode;

        if (!this.nameControl.checkNotEmpty(jsObject.loc.PropertyMain.Name)) return;

        if ((this.mode == "New" || this.nameControl.value != this.datasource.name) &&
            !(this.nameControl.checkExists(jsObject.GetDataSourcesFromDictionary(jsObject.options.report.dictionary), "name") &&
                this.nameControl.checkExists(jsObject.GetVariablesFromDictionary(jsObject.options.report.dictionary), "name"))) {
            return;
        }

        if (this.mode == "Edit") {
            this.datasource.oldName = this.datasource.name;
        }

        var props = ["nameInSource", "category", "name", "alias"];

        for (var i = 0; i < props.length; i++) {
            if (this.datasource[props[i]] != null) {
                if (props[i] == "nameInSource") {
                    this.datasource[props[i]] = this[props[i] + "Control"].textBox.value;
                }
                else {
                    this.datasource[props[i]] = this[props[i] + "Control"].value;
                }
            }
        }

        if (this.datasource["sqlCommand"] != null) {
            this.datasource.sqlCommand = StiBase64.encode(this.queryTextControl.value);
        }

        if (this.datasource["type"] != null) {
            this.datasource.type = this.queryTextTypeControl.key;
        }

        if (this.datasource["commandTimeout"] != null) {
            this.datasource.commandTimeout = this.commandTimeoutControl.textBox.value;
        }

        if (this.datasource["reconnectOnEachRow"] != null) {
            this.datasource.reconnectOnEachRow = this.reconnectOnEachRowControl.isChecked;
        }

        var columns = [];
        var parameters = [];

        for (var key in this.columnsAndParametersTree.columnsItem.childs) {
            columns.push(this.columnsAndParametersTree.columnsItem.childs[key].itemObject);
        }

        for (var key in this.columnsAndParametersTree.parametersItem.childs) {
            parameters.push(this.columnsAndParametersTree.parametersItem.childs[key].itemObject);
        }

        this.datasource.columns = columns;
        this.datasource.parameters = parameters;

        this.changeVisibleState(false);

        if (this.datasource.typeItem == "DataSource") {
            this.jsObject.SendCommandCreateOrEditDataSource(this.datasource);
        }
        else {
            this.datasource.businessObjectFullName = this.businessObjectFullName;
            this.jsObject.SendCommandCreateOrEditBusinessObject(this.datasource);
        }
    }

    return editDataSourceForm;
}

StiMobileDesigner.prototype.ColumnsAndParametersTree = function (editDataSourceForm, width, height) {
    var jsObject = this;
    var tree = this.Tree();
    tree.style.width = width + "px";
    tree.style.height = height + "px";
    tree.className = "stiDesignerDataSourceFormColumnsAndParametersTree";

    tree.addRootItems = function () {
        this.rootItem = jsObject.ColumnsAndParametersTreeItem("root", "Connection.png", {}, this, editDataSourceForm);
        this.rootItem.setOpening(true);
        this.rootItem.button.style.display = "none";
        this.rootItem.iconOpening.style.display = "none";
        this.appendChild(this.rootItem);

        var permissionDataColumns = jsObject.options.permissionDataColumns;
        var permissionSqlParameters = jsObject.options.permissionSqlParameters;

        this.columnsItem = jsObject.ColumnsAndParametersTreeItem(jsObject.loc.FormPageSetup.Columns, "DataColumn.png", {}, this, editDataSourceForm);
        this.rootItem.addChild(this.columnsItem);
        this.columnsItem.style.display = (!permissionDataColumns || permissionDataColumns.indexOf("All") >= 0 || permissionDataColumns.indexOf("View") >= 0) ? "" : "none";

        this.parametersItem = jsObject.ColumnsAndParametersTreeItem(jsObject.loc.PropertyMain.Parameters, "Parameter.png", {}, this, editDataSourceForm);
        this.rootItem.addChild(this.parametersItem);
        this.parametersItem.style.display = (!permissionSqlParameters || permissionSqlParameters.indexOf("All") >= 0 || permissionSqlParameters.indexOf("View") >= 0) ? "" : "none";
    }

    tree.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        this.items = {};
        this.selectedItem = null;
        this.addRootItems();
    }

    tree.addItem = function (itemObject, selectedAfter) {
        var parent = itemObject.typeItem == "Column" ? this.columnsItem : this.parametersItem;
        var caption = this.getItemCaption(itemObject);
        var item = jsObject.ColumnsAndParametersTreeItem(caption, itemObject.typeIcon + ".png", itemObject, this, editDataSourceForm);
        parent.addChild(item);
        parent.setOpening(true);
        if (selectedAfter) item.setSelected();

        return item;
    }

    tree.addColumnsAndParameters = function (columns, parameters, clearBefore) {
        if (clearBefore) this.clear();
        var firstItem = null;

        for (var i = 0; i < columns.length; i++) {
            var item = this.addItem(columns[i]);
            if (!firstItem) firstItem = item;
        }
        for (var i = 0; i < parameters.length; i++) {
            var item = this.addItem(parameters[i]);
            if (!firstItem) firstItem = item;
        }

        if (firstItem) firstItem.setSelected();
    }

    tree.getItemObjects = function (type) {
        var parent = type == "Column" ? this.columnsItem : this.parametersItem;
        var itemObjects = [];
        for (var key in parent.childs) itemObjects.push(parent.childs[key].itemObject);

        return itemObjects;
    }

    tree.onSelectedItem = function () {
        var permissionDataColumns = jsObject.options.permissionDataColumns;
        var permissionSqlParameters = jsObject.options.permissionSqlParameters;

        var enabledRemove = this.selectedItem != null &&
            (((this.selectedItem.itemObject.typeItem == "Column" || this.selectedItem == this.columnsItem) &&
                (!permissionDataColumns || permissionDataColumns.indexOf("All") >= 0 || permissionDataColumns.indexOf("Delete") >= 0)) ||
                ((this.selectedItem.itemObject.typeItem == "Parameter" || this.selectedItem == this.parametersItem) &&
                    (!permissionSqlParameters || permissionSqlParameters.indexOf("All") >= 0 || permissionSqlParameters.indexOf("Delete") >= 0)))

        editDataSourceForm.columnToolBar.removeItem.setEnabled(enabledRemove);

        var showEditControls = this.selectedItem && this.selectedItem != this.columnsItem && this.selectedItem != this.parametersItem;

        editDataSourceForm.nameControlRowEditColumn.style.display = showEditControls ? "" : "none";
        editDataSourceForm.nameInSourceControlRowEditColumn.style.display = showEditControls && this.selectedItem.itemObject.typeItem == "Column" ? "" : "none";
        editDataSourceForm.aliasControlRowEditColumn.style.display = showEditControls && this.selectedItem.itemObject.typeItem == "Column" ? "" : "none";
        editDataSourceForm.expressionControlRowEditColumn.style.display = showEditControls && (this.selectedItem.itemObject.typeItem == "Parameter" || this.selectedItem.itemObject.isCalcColumn) ? "" : "none";
        editDataSourceForm.sizeControlRowEditColumn.style.display = showEditControls && this.selectedItem.itemObject.typeItem == "Parameter" ? "" : "none";
        editDataSourceForm.typeControlRowEditColumn.style.display = showEditControls ? "" : "none";

        var permissionDataColumns = jsObject.options.permissionDataColumns;
        var permissionSqlParameters = jsObject.options.permissionSqlParameters;
        var enabledEdit = this.selectedItem != null &&
            (((this.selectedItem.itemObject.typeItem == "Column") &&
                (!permissionDataColumns || permissionDataColumns.indexOf("All") >= 0 || permissionDataColumns.indexOf("Modify") >= 0)) ||
                ((this.selectedItem.itemObject.typeItem == "Parameter") &&
                    (!permissionSqlParameters || permissionSqlParameters.indexOf("All") >= 0 || permissionSqlParameters.indexOf("Modify") >= 0)));

        if (showEditControls) {
            var items = this.selectedItem.itemObject.typeItem == "Column"
                ? jsObject.GetColumnTypesItems()
                : jsObject.GetParameterTypeItems(editDataSourceForm.datasource.parameterTypes)

            editDataSourceForm.typeControlEditColumn.addItems(items);

            var properties = ["name", "nameInSource", "alias", "expression", "size", "type"];

            for (var i = 0; i < properties.length; i++) {
                if (properties[i] != "type") {
                    editDataSourceForm[properties[i] + "ControlEditColumn"].value =
                        properties[i] != "expression" ? this.selectedItem.itemObject[properties[i]] : StiBase64.decode(this.selectedItem.itemObject[properties[i]]);
                }
                else {
                    editDataSourceForm[properties[i] + "ControlEditColumn"].setKey(this.selectedItem.itemObject[properties[i]]);
                }
                editDataSourceForm[properties[i] + "ControlEditColumn"].setEnabled(enabledEdit);
            }
        }
    };

    tree.getItemCaption = function (itemObject) {
        return jsObject.GetItemCaption(itemObject);
    }

    tree.addRootItems();

    return tree;
}

StiMobileDesigner.prototype.ColumnsAndParametersTreeItem = function (caption, imageName, itemObject, tree, editDataSourceForm) {
    var treeItem = this.TreeItem(caption, imageName, itemObject, tree);
    treeItem.visible = true;
    treeItem.style.width = "100%";
    treeItem.button.style.width = "100%";
    treeItem.button.imageCell.style.width = "1px";
    treeItem.button.captionCell.style.width = "1px";
    treeItem.button.captionCell.style.whiteSpace = "nowrap";
    treeItem.button.style.border = "0px";
    treeItem.button.style.height = this.options.isTouchDevice ? "28px" : "23px";

    //space
    treeItem.button.addCell().style.width = "100%";

    treeItem.button.onmouseover = function () {
        if (this.jsObject.options.isTouchDevice) return;
        this.className = "stiDesignerTreeItemButton stiDesignerTreeItemButtonSelected";
    }

    treeItem.button.onmouseout = function () {
        if (this.jsObject.options.isTouchDevice) return;
        this.className = "stiDesignerTreeItemButton stiDesignerTreeItemButton" + (this.treeItem.isSelected ? "Selected" : "Default");
    }

    treeItem.setSelected = function () {
        if (this.tree.selectedItem) {
            this.tree.selectedItem.button.className = "stiDesignerTreeItemButton stiDesignerTreeItemButtonDefault";
            this.tree.selectedItem.isSelected = false;
        }
        this.button.className = "stiDesignerTreeItemButton stiDesignerTreeItemButtonSelected";
        this.tree.selectedItem = this;
        this.isSelected = true;
        this.tree.onSelectedItem(this);
    }

    treeItem.repaint = function () {
        if (this.itemObject.typeItem == "Column") {
            var imageName = this.jsObject.GetTypeIcon(this.itemObject.type) + ".png";
            if (this.itemObject.isCalcColumn) imageName = imageName.replace("DataColumn", "CalcColumn");
            StiMobileDesigner.setImageSource(this.button.image, this.jsObject.options, imageName);
        }
        this.button.captionCell.innerHTML = this.tree.getItemCaption(this.itemObject);
    }

    return treeItem;
}