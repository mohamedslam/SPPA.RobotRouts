
StiMobileDesigner.prototype.ColumnsPropertiesGroup = function () {
    var columnsPropertiesGroup = this.PropertiesGroup("columnsPropertiesGroup", this.loc.PropertyMain.Columns);
    columnsPropertiesGroup.style.display = "none";

    //Columns
    var columnsControl = this.PropertyTextBox("controlPropertyColumns", this.options.propertyNumbersControlWidth);
    columnsControl.action = function () {
        this.value = this.jsObject.StrToCorrectPositiveInt(this.value);
        this.jsObject.ApplyPropertyValue("columns", this.value);
    }
    columnsPropertiesGroup.container.appendChild(this.Property("columns",
        this.loc.FormPageSetup.NumberOfColumns, columnsControl));

    //Column Width
    var columnWidthControl = this.PropertyTextBox("controlPropertyColumnWidth", this.options.propertyNumbersControlWidth);
    columnWidthControl.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue("columnWidth", this.value);
    }
    columnsPropertiesGroup.container.appendChild(this.Property("columnWidth",
        this.loc.PropertyMain.ColumnWidth, columnWidthControl));

    //Column Gaps
    var columnGapsControl = this.PropertyTextBox("controlPropertyColumnGaps", this.options.propertyNumbersControlWidth);
    columnGapsControl.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue("columnGaps", this.value);
    }
    columnsPropertiesGroup.container.appendChild(this.Property("columnGaps",
        this.loc.PropertyMain.ColumnGaps, columnGapsControl));

    //RightToLeft
    var rightToLeftControl = this.CheckBox("controlPropertyRightToLeft");
    rightToLeftControl.action = function () {
        this.jsObject.ApplyPropertyValue("rightToLeft", this.isChecked);
    }
    columnsPropertiesGroup.container.appendChild(this.Property("rightToLeft",
        this.loc.PropertyMain.RightToLeft, rightToLeftControl));

    //Column Direction
    var controlPropertyColumnDirection = this.PropertyDropDownList("controlPropertyColumnDirection", this.options.propertyControlWidth, this.GetColumnDirectionItems(), true, false);
    controlPropertyColumnDirection.action = function () {
        this.jsObject.ApplyPropertyValue("columnDirection", this.key);
    }
    columnsPropertiesGroup.container.appendChild(this.Property("columnDirection", this.loc.PropertyMain.ColumnDirection, controlPropertyColumnDirection));

    //Min Rows In Column
    var minRowsInColumnControl = this.PropertyTextBox("controlPropertyMinRowsInColumn", this.options.propertyNumbersControlWidth);
    minRowsInColumnControl.action = function () {
        this.value = this.jsObject.StrToCorrectPositiveInt(this.value);
        this.jsObject.ApplyPropertyValue("minRowsInColumn", this.value);
    }
    columnsPropertiesGroup.container.appendChild(this.Property("minRowsInColumn", this.loc.PropertyMain.MinRowsInColumn, minRowsInColumnControl));

    return columnsPropertiesGroup;
}