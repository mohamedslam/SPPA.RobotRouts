
StiMobileDesigner.prototype.InitializeEditTreeViewElementForm_ = function () {
    var form = this.DashboardBaseForm("editTreeViewElementForm", this.loc.Components.StiTreeView, 1, this.HelpLinks["treeViewElement"]);
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
    var fieldMenu = this.options.menus.treeViewFieldMenu || this.InitializeFilterElementFieldMenu("treeViewFieldMenu", field, form, true);
    field.menu = fieldMenu;
    fieldMenu.parentButton = field.button;
    field.textBox.setAttribute("placeholder", jsObject.loc.PropertyMain.Field);

    var keysBlock = this.TreeViewDataColumnsBlock(form, fieldMenu, "keys", this.loc.PropertyMain.Key, true);
    keysBlock.style.width = "calc(100% - 24px)";
    keysBlock.container.maxWidth = 280;
    var parentKeysContainer = keysBlock.container.parentElement;

    form.addControlRow(controlsTable, null, "keysBlock", keysBlock, "0px 12px 12px 12px");
    form.addControlRow(controlsTable, this.loc.PropertyMain.Field, "fieldCaption", null, "6px 12px 6px 12px");
    form.addControlRow(controlsTable, null, "field", field, "6px 12px 6px 12px");

    //SelectionMode
    var selectionMode = this.DropDownList("treeViewElementSelectionMode", 158, null, this.GetSelectionModeItems(), true, null, null, true);
    form.addControlRow(controlsTable, this.loc.PropertyMain.SelectionMode, "selectionMode", selectionMode, "12px 12px 6px 12px", null, true, null, true);
    selectionMode.action = function () {
        form.applyPropertiesToTreeViewElement("SelectionMode", this.key);
    }

    //ShowAllValue
    var showAllValue = this.CheckBox(null, this.loc.Dashboard.ShowAllValue);
    form.addControlRow(controlsTable, " ", "showAllValue", showAllValue, "6px 12px 6px 12px");
    showAllValue.action = function () {
        form.applyPropertiesToTreeViewElement("ShowAllValue", this.isChecked);
    }

    //ShowBlanks
    var showBlanks = this.CheckBox(null, this.loc.Dashboard.ShowBlanks);
    form.addControlRow(controlsTable, " ", "showBlanks", showBlanks, "6px 12px 6px 12px");
    showBlanks.action = function () {
        form.applyPropertiesToTreeViewElement("ShowBlanks", this.isChecked);
    }

    //ParentKey
    var parentKey = this.DropDownList("treeViewElementParentKey", 158, null, null, true, null, null, true);
    form.addControlRow(controlsTable, this.loc.Dashboard.ParentElement, "parentKey", parentKey, "6px 12px 6px 12px", null, true, null, true);
    parentKey.action = function () {
        form.applyPropertiesToTreeViewElement("ParentKey", this.key);
    }

    form.setValues = function () {
        var selectedItem = form.getSelectedItem();
        var meters = this.treeViewProperties.meters;

        keysBlock.container.updateMeters(meters.keys, keysBlock.container.getSelectedItemIndex());

        selectionMode.setKey(this.treeViewProperties.selectionMode);
        showAllValue.setChecked(this.treeViewProperties.showAllValue);
        showBlanks.setChecked(this.treeViewProperties.showBlanks);
        parentKey.setKey(parentKey.haveKey(this.treeViewProperties.parentKey) ? this.treeViewProperties.parentKey : "");
    }

    form.updateControls = function (treeViewProperties) {
        if (!treeViewProperties) return;
        form.treeViewProperties = treeViewProperties;
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
        parentKey.addItems(jsObject.GetFilterElementsItems(this.currentTreeViewElement.properties.elementKey));
        parentKey.setKey("");
        field.value = "";

        form.checkStartMode();

        form.sendCommand({ command: "GetTreeViewElementProperties" },
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

    form.applyPropertiesToTreeViewElement = function (propertyName, propertyValue) {
        form.sendCommand({ command: "SetPropertyValue", propertyName: propertyName, propertyValue: propertyValue },
            function (answer) {
                form.updateControls(answer.elementProperties);
                form.updateElementProperties(answer.elementProperties);
                form.updateSvgContent(answer.elementProperties.svgContent);
            }
        );
    }

    form.sendCommand = function (updateParameters, callbackFunction) {
        jsObject.SendCommandToDesignerServer("UpdateTreeViewElement",
            {
                componentName: form.currentTreeViewElement.properties.name,
                updateParameters: updateParameters
            },
            function (answer) {
                callbackFunction(answer);
            });
    }

    form.updateSvgContent = function (svgContent) {
        this.currentTreeViewElement.properties.svgContent = svgContent;
        this.currentTreeViewElement.repaint();
    }

    form.updateElementProperties = function (properties) {
        for (var propertyName in properties) {
            this.currentTreeViewElement.properties[propertyName] = properties[propertyName];
        }
        jsObject.options.homePanel.updateControls();
        if (jsObject.options.propertiesPanel) jsObject.options.propertiesPanel.updateControls();
    }

    return form;
}

StiMobileDesigner.prototype.TreeViewDataColumnsBlock = function (form, contextMenu, containerName, headerText, multiItems) {
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