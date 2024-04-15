
StiMobileDesigner.prototype.DictionaryDataPropertiesPanel = function () {
    var panel = document.createElement("div");
    panel.style.display = "none";
    var jsObject = panel.jsObject = this;

    var group = this.PropertiesGroup("dictionaryDataPropertiesGroup", this.loc.PropertyCategory.DataCategory);
    group.changeOpenedState(true);
    group.properties = {};
    panel.appendChild(group);

    var propWidth = this.options.propertyControlWidth;

    var props = [
        ["name", this.loc.PropertyMain.Name, this.PropertyTextBox(null, propWidth)],
        ["nameInSource", this.loc.PropertyMain.NameInSource, this.PropertyTextBox(null, propWidth)],
        ["alias", this.loc.PropertyMain.Alias, this.PropertyTextBox(null, propWidth)],
        ["allowExpressions", this.loc.PropertyMain.AllowExpressions, this.CheckBox()],
        ["connectionString", this.loc.PropertyMain.ConnectionString, this.PropertyExpressionControl(null, propWidth, true, true, true)],
        ["connectOnStart", this.loc.PropertyMain.ConnectOnStart, this.CheckBox()],
        ["commandTimeOut", this.loc.PropertyMain.CommandTimeOut, this.PropertyTextBox(null, propWidth)],
        ["description", this.loc.PropertyMain.Description, this.TextBox(null, propWidth)],
        ["expression", this.loc.PropertyMain.Expression, this.PropertyExpressionControl(null, propWidth, true, true, true)],
        ["firstRowIsHeader", this.loc.PropertyMain.FirstRowIsHeader, this.CheckBox()],
        ["promptUserNameAndPassword", this.loc.FormDatabaseEdit.PromptUserNameAndPassword, this.CheckBox()],
        ["pathData", this.loc.PropertyMain.PathData, this.PropertyExpressionControl(null, propWidth, true, true, true)],
        ["pathSchema", this.loc.PropertyMain.PathSchema, this.PropertyExpressionControl(null, propWidth, true, true, true)],
        ["reconnectOnEachRow", this.loc.PropertyMain.ReconnectOnEachRow, this.CheckBox()],
        ["sqlCommand", this.loc.PropertyMain.SqlCommand, this.PropertyExpressionControl(null, propWidth, true, true, true)]
    ]

    for (var i = 0; i < props.length; i++) {
        var control = props[i][2];
        control.propertyName = props[i][0];
        var property = this.Property(props[i][0], props[i][1], control);
        group.properties[control.propertyName] = property;
        group.container.appendChild(property);

        control.action = function () {
            var propertyValue = jsObject.GetControlValue(this);

            if (propertyValue && panel.isEncodedProperty(this.propertyName)) {
                propertyValue = StiBase64.encode(propertyValue);
            }

            var params = {
                propertyName: this.propertyName,
                propertyValue: propertyValue,
                itemObject: panel.dictionaryItem.itemObject,
                currentParentObject: {
                    currentParentName: panel.dictionaryItemParent.name,
                    currentParentType: panel.dictionaryItemParent.type
                },
                isEncodedProperty: panel.isEncodedProperty(this.propertyName)
            };

            jsObject.SendCommandToDesignerServer("SetDictionaryElementProperty", params, function (answer) {
                jsObject.options.report.dictionary = answer.dictionary;
                jsObject.options.dictionaryTree.build(answer.dictionary, true);
            });
        }
    }

    panel.show = function () {
        this.style.display = "";
        this.dictionaryItem = jsObject.options.dictionaryTree.selectedItem;
        this.dictionaryItemParent = jsObject.options.dictionaryTree.getCurrentColumnParent();
        this.update();
    }

    panel.hide = function () {
        this.style.display = "none";
    }

    panel.isEncodedProperty = function (propName) {
        return (propName == "connectionString" || propName == "description" || propName == "sqlCommand" || propName == "expression" || propName == "pathData" || propName == "pathSchema");
    }

    panel.update = function () {
        if (this.dictionaryItem) {
            for (var propName in group.properties) {
                var property = group.properties[propName];
                var propertyValue = this.dictionaryItem.itemObject[propName];
                var showProperty = this.dictionaryItem && propertyValue != null;

                if (propertyValue && panel.isEncodedProperty(propName)) {
                    propertyValue = StiBase64.decode(propertyValue);
                }

                if (showProperty) {
                    jsObject.SetControlValue(property.propertyControl, propertyValue);
                    property.style.display = "";
                }
                else {
                    property.style.display = "none";
                }
            }
        }
    }

    return panel;
}