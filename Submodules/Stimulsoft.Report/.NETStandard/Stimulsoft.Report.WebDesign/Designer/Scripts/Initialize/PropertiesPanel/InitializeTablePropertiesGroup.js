
StiMobileDesigner.prototype.TablePropertiesGroup = function () {
    var tablePropertiesGroup = this.PropertiesGroup("tablePropertiesGroup", this.loc.PropertyCategory.TableCategory);
    tablePropertiesGroup.style.display = "none";

    var props = [
        ["tableAutoWidth", this.loc.PropertyMain.AutoWidth, this.PropertyDropDownList("controlPropertyTableAutoWidth", this.options.propertyControlWidth, this.GetTableAutoWidthItems(), true)],
        ["autoWidthType", this.loc.PropertyMain.AutoWidthType, this.PropertyDropDownList("controlPropertyAutoWidthType", this.options.propertyControlWidth, this.GetTableAutoWidthTypeItems(), true)],
        ["columnCount", this.loc.PropertyMain.ColumnCount, this.PropertyTextBox("controlPropertyColumnCount", this.options.propertyNumbersControlWidth)],
        ["rowCount", this.loc.PropertyMain.RowCount, this.PropertyTextBox("controlPropertyRowCount", this.options.propertyNumbersControlWidth)],
        ["headerRowsCount", this.loc.PropertyMain.HeaderRowsCount, this.PropertyTextBox("controlPropertyHeaderRowsCount", this.options.propertyNumbersControlWidth)],
        ["footerRowsCount", this.loc.PropertyMain.FooterRowsCount, this.PropertyTextBox("controlPropertyFooterRowsCount", this.options.propertyNumbersControlWidth)],
        ["tableRightToLeft", this.loc.PropertyMain.RightToLeft, this.CheckBox("controlPropertyTableRightToLeft")],
        ["dockableTable", this.loc.PropertyMain.DockableTable, this.CheckBox("controlPropertyDockableTable")]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        tablePropertiesGroup.container.appendChild(this.Property(props[i][0], props[i][1], control));

        control.action = function () {
            if (this.propertyName == "columnCount" || this.propertyName == "rowCount" || this.propertyName == "headerRowsCount" || this.propertyName == "footerRowsCount") {
                this.jsObject.SendCommandChangeTableComponent({
                    command: "changeColumnsOrRowsCount",
                    propertyName: this.propertyName,
                    countValue: this.jsObject.GetControlValue(this)
                });
            }
            else {
                this.jsObject.ApplyPropertyValue(this.propertyName, this.jsObject.GetControlValue(this));
            }
        }
    }

    return tablePropertiesGroup;
}

