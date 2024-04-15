
StiMobileDesigner.prototype.InitializeEditListBoxElementForm_ = function () {
    var form = this.DashboardBaseForm("editListBoxElementForm", this.loc.Components.StiListBox, 1, this.HelpLinks["lisBoxElement"]);
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";
    form.hideButtonsPanel();
    var jsObject = this;

    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.width = "330px";

    form.container.appendChild(controlsTable);
    form.container.style.padding = "0 0 6px 0";

    //field
    var field = this.ExpressionControlWithMenu(null, 305, null, null, true, true);
    var fieldMenu = this.options.menus.listBoxFieldMenu || this.InitializeFilterElementFieldMenu("listBoxFieldMenu", field, form, true);
    field.menu = fieldMenu;
    fieldMenu.parentButton = field.button;
    field.textBox.setAttribute("placeholder", jsObject.loc.PropertyMain.Field);

    //DataColumns
    var dataColumns = [
        ["key", this.loc.PropertyMain.Key],
        ["name", this.loc.PropertyMain.Name]
    ];

    for (var i = 0; i < dataColumns.length; i++) {
        var container = this.DashboardDataColumnContainer(form, dataColumns[i][0], dataColumns[i][1], null, null, fieldMenu);
        container.allowSelected = true;
        container.maxWidth = 300;
        form.addControlRow(controlsTable, null, dataColumns[i][0] + "DataColumn", container, "0px 12px " + (i == dataColumns.length - 1 ? "12px" : "0px" + " 12px"));

        container.action = function (actionName) {
            if (actionName == "rename" && this.dataColumnObject) {
                form.sendCommand({
                    command: "RenameMeter",
                    containerName: form.jsObject.UpperFirstChar(this.name),
                    newLabel: this.dataColumnObject.label
                },
                    function (answer) {
                        form.updateControls(answer.elementProperties);
                        form.updateElementProperties(answer.elementProperties);
                        form.updateSvgContent(answer.elementProperties.svgContent);
                    }
                );
                return;
            }

            if (!this.dataColumnObject) {
                form.controls.field.currentContainer = null;
                form.controls.field.textBox.value = "";
                form.controls.field.setEnabled(false);
            }

            form.applyDataColumnPropertyToListBoxElement(this);
        }

        container.onSelected = function () {
            for (var i = 0; i < dataColumns.length; i++) {
                if (form.controls[dataColumns[i][0] + "DataColumn"] != this)
                    form.controls[dataColumns[i][0] + "DataColumn"].setSelected(false);
            }
            if (this.dataColumnObject) {
                form.controls.field.currentContainer = this;
                form.controls.field.setEnabled(true);
                form.controls.field.textBox.value = this.dataColumnObject.expression;
            }
        }
    }

    var parentKeyContainer = form.controls.keyDataColumn.innerContainer.parentElement;

    form.addControlRow(controlsTable, this.loc.PropertyMain.Field, "fieldCaption", null, "6px 12px 6px 12px");
    form.addControlRow(controlsTable, null, "field", field, "6px 12px 6px 12px");

    //SelectionMode
    var selectionMode = this.DropDownList("listBoxElementSelectionMode", 158, null, this.GetSelectionModeItems(), true, null, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.SelectionMode, "selectionMode", selectionMode, "6px 12px 6px 12px", null, true, null, true);
    selectionMode.action = function () {
        form.applyPropertiesToListBoxElement("SelectionMode", this.key);
    }

    //Orientation
    var orientation = this.DropDownList("listBoxElementOrientation", 158, null, this.GetParametersOrientationItems(), true, null, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.Orientation, "orientation", orientation, "6px 12px 6px 12px", null, true, null, true);
    orientation.action = function () {
        form.applyPropertiesToListBoxElement("Orientation", this.key);
    }

    //ShowAllValue
    var showAllValue = this.CheckBox(null, this.loc.Dashboard.ShowAllValue);
    form.addControlRow(controlsTable, " ", "showAllValue", showAllValue, "6px 12px 6px 12px");
    showAllValue.action = function () {
        form.applyPropertiesToListBoxElement("ShowAllValue", this.isChecked);
    }

    //ShowBlanks
    var showBlanks = this.CheckBox(null, this.loc.Dashboard.ShowBlanks);
    form.addControlRow(controlsTable, " ", "showBlanks", showBlanks, "6px 12px 6px 12px");
    showBlanks.action = function () {
        form.applyPropertiesToListBoxElement("ShowBlanks", this.isChecked);
    }

    //parentKey
    var parentKey = this.DropDownList("listBoxElementParentKey", 158, null, null, true, null, null, true);
    form.addControlRow(controlsTable, this.loc.Dashboard.ParentElement, "parentKey", parentKey, "6px 12px 6px 12px", null, true, null, true);
    parentKey.action = function () {
        form.applyPropertiesToListBoxElement("ParentKey", this.key);
    }

    form.setValues = function () {
        selectionMode.setKey(this.listBoxProperties.selectionMode);
        orientation.setKey(this.listBoxProperties.orientation);
        showAllValue.setChecked(this.listBoxProperties.showAllValue);
        showBlanks.setChecked(this.listBoxProperties.showBlanks);
        parentKey.setKey(parentKey.haveKey(this.listBoxProperties.parentKey) ? this.listBoxProperties.parentKey : "");
        var meters = this.listBoxProperties.meters;

        for (var i = 0; i < dataColumns.length; i++) {
            var meter = meters[dataColumns[i][0]];
            var container = this.controls[dataColumns[i][0] + "DataColumn"];

            if (meter) {
                container.addColumn(meter.label, meter);
                if (container.isSelected && container.item) container.item.action();
            }
            else
                container.clear();
        }
    }

    form.checkStartMode = function () {
        var itemsCount = 0;
        for (var i = 0; i < dataColumns.length; i++) {
            var container = form.controls[dataColumns[i][0] + "DataColumn"];
            if (container.dataColumnObject) itemsCount++;
        }

        var keyContainer = form.controls.keyDataColumn.innerContainer;

        if (itemsCount == 0) {
            form.container.appendChild(keyContainer);
            controlsTable.style.display = "none";
            keyContainer.style.height = keyContainer.style.maxHeight = "210px";
            keyContainer.style.width = "267px";
            keyContainer.style.margin = "6px 12px 6px 12px";
        }
        else {
            parentKeyContainer.appendChild(keyContainer);
            controlsTable.style.display = "";
            keyContainer.style.height = "30px";
            keyContainer.style.width = "auto";
            keyContainer.style.margin = "0";
        }
    }

    form.onshow = function () {
        form.currentPanelName = jsObject.options.propertiesPanel.getCurrentPanelName();
        if (jsObject.options.showDictionary) jsObject.options.propertiesPanel.showContainer("Dictionary");

        selectionMode.setKey("Multi");
        orientation.setKey("Vertical");
        showAllValue.setChecked(false);
        showBlanks.setChecked(false);
        parentKey.addItems(jsObject.GetFilterElementsItems(this.currentListBoxElement.properties.elementKey));
        parentKey.setKey("");
        field.textBox.value = "";
        field.setEnabled(false);

        for (var i = 0; i < dataColumns.length; i++) {
            var container = this.controls[dataColumns[i][0] + "DataColumn"];
            container.clear();
        }

        form.checkStartMode();

        form.sendCommand({ command: "GetListBoxElementProperties" },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.checkStartMode();
                form.correctTopPosition();
                for (var i = 0; i < dataColumns.length; i++) {
                    var container = form.controls[dataColumns[i][0] + "DataColumn"];
                    if (container.item) {
                        container.item.action();
                        break;
                    }
                }
            }
        );
    }

    form.onhide = function () {
        jsObject.options.propertiesPanel.showContainer(form.currentPanelName);
    }

    form.updateControls = function (listBoxProperties) {
        if (!listBoxProperties) return;
        form.listBoxProperties = listBoxProperties;
        form.setValues();
    }

    form.applyDataColumnPropertyToListBoxElement = function (container) {
        form.sendCommand(
            {
                command: "SetDataColumn",
                containerName: jsObject.UpperFirstChar(container.name),
                dataColumnObject: container.dataColumnObject
            },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
                if (container.item) container.item.action();
                form.checkStartMode();
                form.correctTopPosition();
            }
        );
    }

    form.applyPropertiesToListBoxElement = function (propertyName, propertyValue) {
        form.sendCommand({ command: "SetPropertyValue", propertyName: propertyName, propertyValue: propertyValue },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.updateElementProperties(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
                form.correctTopPosition();
            }
        );
    }

    form.sendCommand = function (updateParameters, callbackFunction) {
        jsObject.SendCommandToDesignerServer("UpdateListBoxElement",
            {
                componentName: form.currentListBoxElement.properties.name,
                updateParameters: updateParameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    form.updateSvgContent = function (svgContent) {
        this.currentListBoxElement.properties.svgContent = svgContent;
        this.currentListBoxElement.repaint();
    }

    form.updateElementProperties = function (properties) {
        var jsObject = this.jsObject;
        for (var propertyName in properties) {
            this.currentListBoxElement.properties[propertyName] = properties[propertyName];
        }
        jsObject.options.homePanel.updateControls();
        if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
    }

    return form;
}