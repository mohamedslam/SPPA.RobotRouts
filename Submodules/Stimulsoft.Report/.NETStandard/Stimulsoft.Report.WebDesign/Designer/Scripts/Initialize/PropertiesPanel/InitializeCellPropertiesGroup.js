
StiMobileDesigner.prototype.CellPropertiesGroup = function () {
    var cellPropertiesGroup = this.PropertiesGroup("cellPropertiesGroup", this.loc.PropertyCategory.CellCategory);
    cellPropertiesGroup.style.display = "none";

    var props = [
        ["cellType", this.loc.PropertyMain.CellType, this.PropertyDropDownList("controlPropertyCellType", this.options.propertyControlWidth, this.GetCellTypeItems(), true)],
        ["cellDockStyle", this.loc.PropertyMain.CellDockStyle, this.PropertyDropDownList("controlPropertyCellDockStyle", this.options.propertyControlWidth, this.GetCellDockStyleItems(), true)],
        ["fixedWidth", this.loc.PropertyMain.FixedWidth, this.CheckBox("controlPropertyFixedWidth")]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        cellPropertiesGroup.container.appendChild(this.Property(props[i][0], props[i][1], control));

        control.action = function () {
            if (this.propertyName == "cellType") {
                this.jsObject.SendCommandChangeTableComponent({
                    command: "convertTo",
                    cellType: this.jsObject.GetControlValue(this)
                })
            }
            else {
                this.jsObject.ApplyPropertyValue(this.propertyName, this.jsObject.GetControlValue(this));
            }
        }
    }

    return cellPropertiesGroup;
}

