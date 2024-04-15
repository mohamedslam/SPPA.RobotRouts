
StiMobileDesigner.prototype.InitializeEditTreeViewBoxElementForm_ = function () {
    var form = this.DashboardBaseForm("editTreeViewBoxElementForm", this.loc.Components.StiTreeViewBox, 1, this.HelpLinks["treeViewBoxElement"]);
    form.isDockableToComponent = true;
    form.container.style.borderTop = "0px";
    form.caption.style.padding = "0px 10px 0 12px";
    form.hideButtonsPanel();
    var jsObject = this;

    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.width = "330px";

    form.container.appendChild(controlsTable);
    form.container.style.padding = "0 0 6px 0";

    //Field
    var field = this.ExpressionControlWithMenu(null, 305, null, null, true, true);
    var fieldMenu = this.options.menus.treeViewBoxFieldMenu || this.InitializeFilterElementFieldMenu("treeViewBoxFieldMenu", field, form, true);
    field.menu = fieldMenu;
    fieldMenu.parentButton = field.button;
    field.textBox.setAttribute("placeholder", jsObject.loc.PropertyMain.Field);

    var keysBlock = this.TreeViewBoxDataColumnsBlock(form, fieldMenu, "keys", this.loc.PropertyMain.Key, true);
    keysBlock.style.width = "calc(100% - 24px)";
    keysBlock.container.maxWidth = 250;
    var parentKeysContainer = keysBlock.container.parentElement;

    form.addControlRow(controlsTable, null, "keysBlock", keysBlock, "0px 12px 12px 12px");
    form.addControlRow(controlsTable, this.loc.PropertyMain.Field, "fieldCaption", null, "6px 12px 6px 12px");
    form.addControlRow(controlsTable, null, "field", field, "6px 12px 6px 12px");

    //SelectionMode
    var selectionMode = this.DropDownList("treeViewBoxElementSelectionMode", 158, null, this.GetSelectionModeItems(), true, null, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.SelectionMode, "selectionMode", selectionMode, "12px 12px 6px 12px", null, true, null, true);
    selectionMode.action = function () {
        form.applyPropertiesToTreeViewBoxElement("SelectionMode", this.key);
    }

    //ShowAllValue
    var showAllValue = this.CheckBox(null, this.loc.Dashboard.ShowAllValue);
    form.addControlRow(controlsTable, " ", "showAllValue", showAllValue, "6px 12px 6px 12px");
    showAllValue.action = function () {
        form.applyPropertiesToTreeViewBoxElement("ShowAllValue", this.isChecked);
    }

    //ShowBlanks
    var showBlanks = this.CheckBox(null, this.loc.Dashboard.ShowBlanks);
    form.addControlRow(controlsTable, " ", "showBlanks", showBlanks, "6px 12px 6px 12px");
    showBlanks.action = function () {
        form.applyPropertiesToTreeViewBoxElement("ShowBlanks", this.isChecked);
    }

    //parentKey
    var parentKey = this.DropDownList("treeViewBoxElementParentKey", 158, null, null, true, null, null, true);
    form.addControlRow(controlsTable, this.loc.Dashboard.ParentElement, "parentKey", parentKey, "6px 12px 6px 12px", null, true, null, true);
    parentKey.action = function () {
        form.applyPropertiesToTreeViewBoxElement("ParentKey", this.key);
    }

    form.setValues = function () {
        var selectedItem = form.getSelectedItem();
        var meters = this.treeViewBoxProperties.meters;

        keysBlock.container.updateMeters(meters.keys, keysBlock.container.getSelectedItemIndex());

        selectionMode.setKey(this.treeViewBoxProperties.selectionMode);
        showAllValue.setChecked(this.treeViewBoxProperties.showAllValue);
        showBlanks.setChecked(this.treeViewBoxProperties.showBlanks);
        parentKey.setKey(parentKey.haveKey(this.treeViewBoxProperties.parentKey) ? this.treeViewBoxProperties.parentKey : "");
    }

    form.updateControls = function (treeViewBoxProperties) {
        if (!treeViewBoxProperties) return;
        form.treeViewBoxProperties = treeViewBoxProperties;
        form.setValues();
    }

    form.checkStartMode = function () {
        var itemsCount = keysBlock.container.getCountItems();

        if (itemsCount == 0) {
            form.container.appendChild(keysBlock.container);
            controlsTable.style.display = "none";
            keysBlock.container.style.height = keysBlock.container.style.maxHeight = "260px";
            keysBlock.container.style.width = "267px";
            keysBlock.container.style.margin = "6px 12px 6px 12px";
        }
        else {
            parentKeysContainer.appendChild(keysBlock.container);
            controlsTable.style.display = "";
            keysBlock.container.style.height = "auto";
            keysBlock.container.style.width = "auto";
            keysBlock.container.style.margin = "0";
            keysBlock.container.style.maxHeight = "100px";
        }
    }

    form.onshow = function () {
        form.currentPanelName = jsObject.options.propertiesPanel.getCurrentPanelName();
        if (jsObject.options.showDictionary) jsObject.options.propertiesPanel.showContainer("Dictionary");

        keysBlock.container.clear();
        selectionMode.setKey("Multi");
        showAllValue.setChecked(false);
        showBlanks.setChecked(false);
        parentKey.addItems(jsObject.GetFilterElementsItems(this.currentTreeViewBoxElement.properties.elementKey));
        parentKey.setKey("");
        field.textBox.value = "";
        field.setEnabled(false);

        form.checkStartMode();

        form.sendCommand({ command: "GetTreeViewBoxElementProperties" },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.checkStartMode();
                form.correctTopPosition();
                if (keysBlock.container.getCountItems() > 0) {
                    keysBlock.container.getItemByIndex(0).select();
                }
            }
        );
    }

    form.onhide = function () {
        jsObject.options.propertiesPanel.showContainer(form.currentPanelName);
    }

    form.getSelectedItem = function () {
        return keysBlock.selectedItem;
    }

    form.applyPropertiesToTreeViewBoxElement = function (propertyName, propertyValue) {
        form.sendCommand({ command: "SetPropertyValue", propertyName: propertyName, propertyValue: propertyValue },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.updateElementProperties(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
            }
        );
    }

    form.sendCommand = function (updateParameters, callbackFunction) {
        jsObject.SendCommandToDesignerServer("UpdateTreeViewBoxElement",
            {
                componentName: form.currentTreeViewBoxElement.properties.name,
                updateParameters: updateParameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    form.updateSvgContent = function (svgContent) {
        this.currentTreeViewBoxElement.properties.svgContent = svgContent;
        this.currentTreeViewBoxElement.repaint();
    }

    form.updateElementProperties = function (properties) {
        for (var propertyName in properties) {
            this.currentTreeViewBoxElement.properties[propertyName] = properties[propertyName];
        }
        jsObject.options.homePanel.updateControls();
        if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
    }

    return form;
}

StiMobileDesigner.prototype.TreeViewBoxDataColumnsBlock = function (form, contextMenu, containerName, headerText, multiItems) {
    var block = this.DashboardDataColumnsBlock(form, contextMenu, containerName, headerText, multiItems);

    block.container.onAction = function (actionName) {
        form.controls.field.currentContainer = this;

        if (actionName == "rename" && this.selectedItem) {
            var itemIndex = this.selectedItem.container.getItemIndex(this.selectedItem);
            form.sendCommand({
                command: "RenameMeter",
                itemIndex: itemIndex,
                newLabel: this.selectedItem.itemObject.label
            },
                function (answer) {
                    form.updateControls(answer.elementProperties);
                    form.updateElementProperties(answer.elementProperties);
                    form.updateSvgContent(answer.elementProperties.svgContent);
                }
            );
            return;
        }

        form.controls.field.setEnabled(this.selectedItem != null);
        form.controls.field.textBox.value = this.selectedItem ? this.selectedItem.itemObject.expression : "";
    }

    return block;
}