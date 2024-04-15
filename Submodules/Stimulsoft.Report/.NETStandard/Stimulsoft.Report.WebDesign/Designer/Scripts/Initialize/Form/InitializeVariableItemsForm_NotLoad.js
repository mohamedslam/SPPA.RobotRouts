
StiMobileDesigner.prototype.InitializeVariableItemsForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("variableItemsForm", this.loc.PropertyMain.Items, 4, this.HelpLinks["variableItems"]);
    var dataGridColums;
    var dataGridControl;

    var selColumButton = this.FormButton(null, null, this.loc.Wizards.SelectColumns);
    selColumButton.style.display = "inline-block";
    selColumButton.style.margin = "12px";

    selColumButton.action = function () {
        jsObject.InitializeSelectColumnsForVariableForm(function (selColumForm) {
            selColumForm.changeVisibleState(true);

            selColumForm.action = function () {
                selColumForm.changeVisibleState(false);

                jsObject.SendCommandToDesignerServer("GetVariableItemsFromDataColumn", {
                    keysColumn: selColumForm.controls.keys.textBox.value,
                    valuesColumn: selColumForm.controls.values.textBox.value
                }, function (answer) {
                    dataGridControl.clear();

                    var encodeValue = function (val) {
                        return (val != null ? StiBase64.encode(val.toString()) : "");
                    }

                    if (answer.items) {
                        var varItems = [];
                        for (var i = 0; i < answer.items.length; i++) {
                            varItems.push({
                                key: encodeValue(answer.items[i].key),
                                keyTo: null,
                                value: encodeValue(answer.items[i].value),
                                checked: true,
                                type: "value"
                            });
                        }
                        form.convertVariableItemsToDataGridValues(varItems);
                    }
                });
            }
        });
    }

    var footerTable = this.CreateHTMLTable();
    footerTable.style.width = "100%";
    var buttonsPanel = form.buttonsPanel;
    form.removeChild(buttonsPanel);
    form.appendChild(footerTable);
    footerTable.addCell(selColumButton).style.width = "1px";
    footerTable.addCell();
    footerTable.addCell(form.buttonOk).style.width = "1px";
    footerTable.addCell(form.buttonCancel).style.width = "1px";

    form.convertVariableItemsToDataGridValues = function (itemsFromColumns) {
        var items = itemsFromColumns || jsObject.CopyObject(this.variable.items);
        var dataGridValues = [];

        if (items) {
            for (var i = 0; i < items.length; i++) {
                var rowArray = [];
                var rowValues = {
                    key: items[i].key != null ? StiBase64.decode(items[i].key) : null,
                    keyTo: items[i].keyTo != null ? StiBase64.decode(items[i].keyTo) : null,
                    value: StiBase64.decode(items[i].value || ""),
                    checked: itemsFromColumns ? true : (this.showChecked && this.checkedStates && i < this.checkedStates.length ? this.checkedStates[i] : false),
                    type: items[i].type == "expression"
                }
                for (var k = 0; k < dataGridColums.length; k++) {
                    var columnName = dataGridColums[k][0];
                    rowArray.push(rowValues[columnName]);
                }
                dataGridValues.push(rowArray);
            }
            dataGridControl.setValue(dataGridValues);
        }
    }

    form.convertDataGridValuesToVariableItems = function () {
        var items = [];
        var dataGridValues = dataGridControl.getValue();

        if (dataGridValues) {
            for (var i = 0; i < dataGridValues.length; i++) {
                var valuesRow = dataGridValues[i];
                var item = {};

                for (var k = 0; k < dataGridColums.length; k++) {
                    var columnName = dataGridColums[k][0];
                    if (k < valuesRow.length) {
                        item[columnName] = columnName == "type"
                            ? (valuesRow[k] ? "expression" : "value")
                            : (columnName == "checked" ? valuesRow[k] : StiBase64.encode(valuesRow[k]));
                    }
                }
                items.push(item);
            }
        }

        if (items.length == 0)
            return null;

        return items;
    }

    form.show = function (variableForm) {
        this.variableForm = variableForm;
        this.variable = variableForm.variable;
        this.basicType = this.variable.basicType;
        this.varType = this.variable.type;
        this.showChecked = variableForm.controls.basicType.key == "List" && !variableForm.controls.allowUserValues.isChecked;
        this.checkedStates = this.variable.checkedStates;

        selColumButton.style.display = this.basicType != "Range" ? "" : "none";

        dataGridColums = [];

        var controlType = null;
        if (this.varType == "datetime") controlType = "datepicker";
        if (this.varType == "bool") controlType = "boolcombobox";

        if (this.showChecked) {
            dataGridColums.push(["checked", jsObject.loc.PropertyMain.Checked, "checkbox"]);
        }

        dataGridColums.push(["key", this.basicType == "Range" ? jsObject.loc.PropertyMain.RangeFrom : jsObject.loc.PropertyMain.Key, controlType]);

        if (this.basicType == "Range") {
            dataGridColums.push(["keyTo", jsObject.loc.PropertyMain.RangeTo, controlType]);
        }

        dataGridColums.push(["value", jsObject.loc.PropertyMain.Label]);
        dataGridColums.push(["type", jsObject.loc.PropertyMain.Expression, "checkbox"]);

        dataGridControl = jsObject.InitializeVariableManualDataControl(dataGridColums.length > 3 ? 600 : 450, 450, dataGridColums, 15);
        dataGridControl.style.margin = "12px";

        form.container.clear();
        form.container.appendChild(dataGridControl);

        this.convertVariableItemsToDataGridValues();
        this.changeVisibleState(true);
    }

    form.action = function () {
        var variableForm = this.variableForm;
        var items = this.convertDataGridValuesToVariableItems();

        variableForm.variable.items = items;
        variableForm.controls.items.textArea.value = variableForm.getItemsStr();

        if (this.showChecked && items) {
            variableForm.variable.checkedStates = [];
            for (var i = 0; i < items.length; i++) {
                if (items[i].checked != null) {
                    variableForm.variable.checkedStates[i] = items[i].checked;
                }
            }
        }

        this.changeVisibleState(false);
    }

    return form;
}

StiMobileDesigner.prototype.InitializeVariableManualDataControl = function (width, height, dataColumns, countRows) {
    var jsObject = this;
    var control = this.InitializeManualDataControl(width, height, dataColumns, countRows);

    var rowsHeight = 22;
    var leftCellWidth = 60;
    var cellWidth = ((width || 300) - leftCellWidth) / dataColumns.length;
    var mainTable = control.mainTable;
    var contextMenu = control.contextMenu;

    //override methods
    control.setRowToExpression = function (state, row) {
        row.isExpression = state;

        for (var k = 1; k < row.childNodes.length; k++) {
            var cell = row.childNodes[k];
            var columnName = cell.textBox.dataColumn;

            if (columnName == "key" || columnName == "keyTo") {
                if (cell.dateControl || cell.boolComboBox) {
                    cell.textBox.style.display = state ? "" : "none";
                }
                if (cell.dateControl) cell.dateControl.style.display = !state ? "" : "none";
                if (cell.boolComboBox) cell.boolComboBox.style.display = !state ? "" : "none";
            }
        }
    }

    control.rowIsExpression = function (row, rowValues) {
        for (var i = 1; i < row.childNodes.length; i++) {
            var cell = row.childNodes[i];
            var columnName = cell.textBox.dataColumn;
            var cellValue = rowValues[i - 1];

            if (columnName == "type" && cellValue) {
                return true;
            }
        }
        return false;
    }

    control.setValue = function (valArray) {
        this.clear();

        if (valArray) {
            for (var i = 0; i < valArray.length; i++) {
                if (i >= mainTable.tr.length - 1) {
                    control.countRows++;
                    control.addRow(control.countRows);
                }
                var row = mainTable.tr[i + 1];
                var rowValues = valArray[i];
                var isExpression = row.isExpression = control.rowIsExpression(row, rowValues);

                for (var k = 1; k < row.childNodes.length; k++) {
                    var cell = row.childNodes[k];
                    var controlType = cell.controlType;
                    var cellValue = rowValues[k - 1];
                    var columnName = cell.textBox.dataColumn;
                    var textBox = cell.textBox;

                    if (isExpression && (columnName == "key" || columnName == "keyTo")) {
                        textBox.value = cellValue;
                        textBox.style.display = "";
                        if (cell.boolComboBox) cell.boolComboBox.style.display = "none";
                        if (cell.dateControl) cell.dateControl.style.display = "none";
                    }
                    else {
                        switch (controlType) {
                            case "datepicker": {
                                var dateControl = cell.dateControl;
                                if (cellValue) dateControl.setKey(new Date(cellValue));
                                break;
                            }
                            case "boolcombobox": {
                                var boolComboBox = cell.boolComboBox;
                                boolComboBox.setKey(cellValue ? jsObject.UpperFirstChar(cellValue) : "");
                                break;
                            }
                            case "checkbox": {
                                var checkBox = cell.checkBox;
                                checkBox.setChecked(cellValue);
                                break;
                            }
                            default: {
                                textBox.value = cellValue || "";
                            }
                        }
                    }
                }
            }
        }
    }

    control.getValue = function () {
        var result = [];

        for (var i = 1; i <= control.countRows; i++) {
            var row = mainTable.tr[i];
            var emptyRow = true;
            var rowValues = [];

            for (var k = 1; k < row.childNodes.length; k++) {
                var cell = row.childNodes[k];
                var textBox = cell.textBox;
                var controlType = cell.controlType;
                var value = "";

                switch (controlType) {
                    case "datepicker": {
                        var dateControl = cell.dateControl;
                        if ((!row.isExpression && dateControl.textBox.value && dateControl.key) || (row.isExpression && textBox.value)) {
                            value = row.isExpression ? textBox.value : jsObject.DateToStringAmericanFormat(dateControl.key);
                            emptyRow = false;
                        }
                        break;
                    }
                    case "boolcombobox": {
                        var boolComboBox = cell.boolComboBox;
                        if ((!row.isExpression && boolComboBox.textBox.value && boolComboBox.key) || (row.isExpression && textBox.value)) {
                            value = row.isExpression ? textBox.value : boolComboBox.key;
                            emptyRow = false;
                        }
                        break;
                    }
                    case "checkbox": {
                        var checkBox = cell.checkBox;
                        value = checkBox.isChecked;
                        break;
                    }
                    default: {
                        if (textBox.value != "") {
                            value = textBox.value;
                            emptyRow = false;
                        }
                    }
                }

                rowValues.push(value);
            }

            if (!emptyRow) result.push(rowValues);
        }

        return result;
    }

    control.addRow = function (rowIndex) {
        var row = mainTable.insertRow(rowIndex);

        var leftCell = mainTable.addTextCellInRow(rowIndex, rowIndex)
        leftCell.className = "stiManualControlCell stiManualControlHeaderCell";
        leftCell.style.width = leftCellWidth + "px";
        leftCell.rowIndex = rowIndex;

        leftCell.onmousedown = function (event) {
            control.selectRow(this.rowIndex);
        }

        leftCell.onmouseup = function (event) {
            if (event.button == 2) {
                event.stopPropagation();
                contextMenu.currentCell = control.getCellByIndex(this.rowIndex, 1);
                contextMenu.updateItemsStates();
                var point = jsObject.FindMousePosOnMainPanel(event);
                contextMenu.show(point.xPixels + 10, point.yPixels + 20, "Down", "Right");
            }
            return false;
        }

        for (var k = 0; k < dataColumns.length; k++) {
            var textBox = jsObject.TextBox(null, cellWidth - 6, rowsHeight);
            textBox.style.border = "0";
            textBox.style.textAlign = "right";
            textBox.style.marginRight = "4px";
            textBox.rowIndex = rowIndex;
            textBox.columnIndex = k + 1;
            textBox.dataColumn = dataColumns[k][0];
            textBox.style.background = "transparent";

            var cell = mainTable.addCellInRow(rowIndex, textBox);
            cell.className = "stiManualControlCell";
            cell.style.lineHeight = "0";
            cell.style.width = cellWidth + "px";
            cell.style.height = rowsHeight + "px";
            cell.textBox = textBox;

            var controlType = dataColumns[k][2];
            if (controlType) {
                cell.controlType = controlType;
                textBox.style.display = "none";

                switch (controlType) {
                    case "datepicker": {
                        var dateControl = cell.dateControl = jsObject.DateControl(null, cellWidth);
                        dateControl.textBox.value = "";
                        dateControl.style.border = dateControl.textBox.style.border = dateControl.button.style.border = "0";
                        cell.appendChild(dateControl);
                        break;
                    }
                    case "boolcombobox": {
                        var boolComboBox = cell.boolComboBox = jsObject.DropDownList(null, cellWidth, null, jsObject.GetBoolItems(), true);
                        boolComboBox.textBox.style.border = boolComboBox.button.style.border = "0";
                        cell.appendChild(boolComboBox);
                        break;
                    }
                    case "checkbox": {
                        var checkBox = cell.checkBox = jsObject.CheckBox();
                        checkBox.style.display = "inline-block";
                        checkBox.row = row;
                        cell.appendChild(checkBox);

                        if (dataColumns[k][0] == "type") {
                            checkBox.action = function () {
                                control.setRowToExpression(this.isChecked, this.row);
                            }
                        }
                        break;
                    }
                }
            }

            if (this.visibilityParams && this.visibilityParams[dataColumns[k][0]] && !this.visibilityParams[dataColumns[k][0]].visible) {
                cell.style.display = "none";
            }

            textBox.action = function () {
                if (this.rowIndex == control.countRows) {
                    control.countRows++;
                    control.addRow(control.countRows);
                }
                control.action();
            }

            textBox.actionOnKeyEnter = function () {
                var nextCell = control.getCellByIndex(this.rowIndex + 1, this.columnIndex);
                if (nextCell) {
                    control.selectCell(nextCell);
                    nextCell.textBox.focus();
                }
            }

            textBox.onkeypress = function (event) {
                if (this.readOnly) return false;
                if (event && event.keyCode == 13) {
                    this.keyEnterPressed = true;
                    this.onblur();
                    this.actionOnKeyEnter();
                    return false;
                }
            }

            cell.setSelected = function (state) {
                this.className = state ? "stiManualControlCell stiManualControlCellSelected" : "stiManualControlCell";
                this.isSelected = state;
            }

            cell.onmousedown = function (event) {
                control.currentCell = this;

                if (jsObject.options.CTRL_pressed) {
                    this.setSelected(!this.isSelected);
                }
                else if (!(event.button == 2 && this.isSelected)) {
                    control.selectCell(this);
                }
            }

            cell.onmouseup = function (event) {
                if (event.button == 2) {
                    event.stopPropagation();
                    contextMenu.currentCell = this;
                    contextMenu.updateItemsStates();
                    var point = jsObject.FindMousePosOnMainPanel(event);
                    contextMenu.show(point.xPixels + 10, point.yPixels + 20, "Down", "Right");
                }
                return false;
            }
        }
    }

    control.moveRow = function (rowIndex, direction, columnIndex) {
        if (rowIndex < mainTable.tr.length) {
            var rowFrom = mainTable.tr[rowIndex];
            var rowTo = direction == "down" ? (rowIndex + 1 < mainTable.tr.length ? mainTable.tr[rowIndex + 1] : null) : (rowIndex - 1 > 0 ? mainTable.tr[rowIndex - 1] : null)

            if (rowFrom && rowTo) {
                for (var i = 1; i < rowFrom.childNodes.length; i++) {
                    var cellFrom = rowFrom.childNodes[i];
                    var cellTo = rowTo.childNodes[i];
                    var controlTypes = ["textBox", "dateControl", "boolComboBox", "checkBox"];

                    for (var k = 0; k < controlTypes.length; k++) {
                        var controlFrom = cellFrom[controlTypes[k]];
                        var controlTo = cellTo[controlTypes[k]];
                        if (controlFrom && controlTo) {
                            if (controlTypes[k] == "dateControl" || controlTypes[k] == "boolComboBox") {
                                var keyFrom = controlFrom.key;
                                var valueFrom = controlFrom.textBox.value;
                                controlFrom.setKey(controlTo.key);
                                controlFrom.textBox.value = controlTo.textBox.value;
                                controlTo.setKey(keyFrom);
                                controlTo.textBox.vlaue = valueFrom;
                            }
                            else if (controlTypes[k] == "checkBox") {
                                var valueFrom = controlFrom.isChecked;
                                controlFrom.setChecked(controlTo.isChecked);
                                controlTo.setChecked(valueFrom);
                            }
                            else {
                                var valueFrom = controlFrom.value;
                                controlFrom.value = controlTo.value;
                                controlTo.value = valueFrom;
                            }
                        }
                    }
                }
                var rowFromIsExpression = rowFrom.isExpression;
                control.setRowToExpression(rowTo.isExpression, rowFrom);
                control.setRowToExpression(rowFromIsExpression, rowTo);
            }
        }

        control.action();
        var selectedCell = control.getCellByIndex(direction == "down" ? rowIndex + 1 : rowIndex - 1, columnIndex);
        if (selectedCell) control.selectCell(selectedCell);
    }

    control.clear();

    return control;
}