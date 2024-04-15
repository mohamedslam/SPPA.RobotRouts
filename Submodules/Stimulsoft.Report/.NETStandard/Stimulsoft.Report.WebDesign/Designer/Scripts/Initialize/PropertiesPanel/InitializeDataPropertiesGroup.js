
StiMobileDesigner.prototype.DataPropertiesGroup = function () {
    var jsObject = this;
    var dataPropertiesGroup = this.PropertiesGroup("dataPropertiesGroup", this.loc.PropertyCategory.DataCategory);
    dataPropertiesGroup.style.display = "none";

    var props = [
        ["valueDataColumn", this.loc.PropertyMain.ValueDataColumn, this.PropertyDataControl("controlPropertyDataValueDataColumn", this.options.propertyControlWidth, true, false, this.options.propertyControlsHeight)],
        ["dataSource", this.loc.PropertyMain.DataSource, this.PropertyDataGroupDropDownList("controlPropertyDataSource", this.options.propertyControlWidth, null, true)],
        ["dataRelation", this.loc.PropertyMain.DataRelation, this.PropertyDataGroupDropDownList("controlPropertyDataRelation", this.options.propertyControlWidth, null, true)],
        ["masterComponent", this.loc.PropertyMain.MasterComponent, this.PropertyDataGroupDropDownList("controlPropertyMasterComponent", this.options.propertyControlWidth, null, true)],
        ["multipleInitialization", this.loc.PropertyMain.MultipleInitialization, this.CheckBox("controlPropertyMultipleInitialization")],
        ["countData", this.loc.PropertyMain.CountData, this.PropertyTextBox("controlPropertyCountData", this.options.propertyNumbersControlWidth)],
        ["filters", this.loc.PropertyMain.Filters, this.PropertyFilter("controlPropertyFilters")],
        ["sortData", this.loc.PropertyMain.Sort, this.PropertySort("controlPropertySort")]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        dataPropertiesGroup.container.appendChild(this.Property(props[i].length > 3 ? props[i][3] : props[i][0], props[i][1], control));

        if (control.propertyName == "filters" || control.propertyName == "sortData") {
            control.dataSourceControl = jsObject.options.controls.controlPropertyDataSource;
        }

        control.action = function () {
            var value = jsObject.GetControlValue(this);

            if (this.propertyName == "filters") {
                jsObject.ApplyPropertyValue(["filterData", "filterMode", "filterOn"], [value.filterData, value.filterMode, value.filterOn]);
            }
            else if (this.propertyName == "valueDataColumn") {
                jsObject.ApplyPropertyValue(["valueDataColumn"], StiBase64.encode(value));
            }
            else {
                jsObject.ApplyPropertyValue(this.propertyName, value);
            }
        }
    }

    return dataPropertiesGroup;
}



StiMobileDesigner.prototype.PropertyDataGroupDropDownList = function (name, width, toolTip) {
    var jsObject = this;
    var dropDownList = this.PropertyDataDropDownList(name, width, toolTip);

    //Override
    dropDownList.menu.onshow = function () {
        if (dropDownList.name == "controlPropertyDataSource") {
            dropDownList.addItems(jsObject.GetDataSourceItemsFromDictionary());
        }
        else if (dropDownList.name == "controlPropertyDataRelation") {
            if (jsObject.options.properties.valueDataColumn.style.display == "") {
                var valueDataColumn = jsObject.GetControlValue(jsObject.options.controls.controlPropertyDataValueDataColumn);
                var dataSourceName = valueDataColumn && valueDataColumn.indexOf(".") > 0 ? valueDataColumn.substring(0, valueDataColumn.indexOf(".")) : "";
                var dataSource = jsObject.GetDataSourceByNameFromDictionary(dataSourceName);
                var relations = jsObject.GetRelationsInSourceItems(dataSource);
                dropDownList.addItems(relations);
            }
            else {
                var dataSourceControl = jsObject.options.controls.controlPropertyDataSource;
                var dataSource = jsObject.GetDataSourceByNameFromDictionary(dataSourceControl.key);
                var relations = jsObject.GetRelationsInSourceItems(dataSource);
                dropDownList.addItems(relations);
            }
        }
        else if (dropDownList.name == "controlPropertyMasterComponent") {
            dropDownList.addItems(jsObject.GetMasterComponentItems());
        }

        if (dropDownList.key == null) return;

        for (var itemName in this.items) {
            if (dropDownList.key == this.items[itemName].key) {
                this.items[itemName].setSelected(true);
                return;
            }
            else if (itemName.indexOf("separator") != 0) {
                this.items[itemName].setSelected(false);
            }
        }
    }

    return dropDownList;
}