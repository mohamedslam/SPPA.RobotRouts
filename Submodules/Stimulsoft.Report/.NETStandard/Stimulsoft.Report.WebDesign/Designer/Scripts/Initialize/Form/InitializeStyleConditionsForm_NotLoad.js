
StiMobileDesigner.prototype.InitializeStyleConditionsForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("styleConditionsForm", this.loc.PropertyMain.Conditions, 3, this.HelpLinks["conditions"]);
    form.controls = {};
    form.container.style.padding = "0px";

    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "12px";
    form.container.appendChild(toolBar);

    var addButton = form.controls.addButton = this.FormButton(null, null, this.loc.Chart.AddCondition.replace("&", ""));
    toolBar.addCell(addButton);

    var separator = this.HomePanelSeparator();
    separator.style.margin = "0 2px 0 2px";
    toolBar.addCell(separator);

    var moveUpButton = form.controls.moveUpButton = this.StandartSmallButton(null, null, null, "Arrows.ArrowUpBlue.png", null, null);
    var moveDownButton = form.controls.moveDownButton = this.StandartSmallButton(null, null, null, "Arrows.ArrowDownBlue.png", null, null);

    moveUpButton.setEnabled(false);
    moveDownButton.setEnabled(false);

    toolBar.addCell(moveUpButton);
    toolBar.addCell(moveDownButton);

    if (!this.options.isTouchDevice) {
        moveUpButton.style.display = moveDownButton.style.display = separator.style.display = "none";
    }

    //Conditions Container
    var conditionsContainer = this.StyleConditionsContainer(form, null, 400);
    conditionsContainer.style.minWidth = "650px";
    conditionsContainer.style.margin = "0 12px 12px 12px";
    form.container.appendChild(conditionsContainer);

    addButton.action = function () {
        conditionsContainer.addCondition();
    }

    moveUpButton.action = function () {
        if (conditionsContainer.selectedItem) {
            conditionsContainer.selectedItem.move("Up");
        }
    }

    moveDownButton.action = function () {
        if (conditionsContainer.selectedItem) {
            conditionsContainer.selectedItem.move("Down");
        }
    }

    form.show = function (styleDesignerForm, propertyControl) {
        this.styleDesignerForm = styleDesignerForm;
        this.propertyControl = propertyControl;

        var conditions = styleDesignerForm.stylesTree.selectedItem.itemObject.properties.conditions;
        conditionsContainer.clear();
        form.changeVisibleState(true);

        if (!conditions) return;
        for (var i = 0; i < conditions.length; i++) conditionsContainer.addCondition(conditions[i], true);
        conditionsContainer.onAction();
    }

    form.action = function () {
        var conditions = [];

        for (var i = 0; i < conditionsContainer.childNodes.length; i++) {
            conditions.push(conditionsContainer.childNodes[i].getConditionObject());
        }

        this.styleDesignerForm.stylesTree.selectedItem.itemObject.properties.conditions = conditions;
        this.styleDesignerForm.isModified = true;
        this.propertyControl.value = "[" + (conditions.length > 0 ? jsObject.loc.PropertyMain.Conditions : jsObject.loc.FormConditions.NoConditions) + "]";

        form.changeVisibleState(false);
    }

    return form;
}

StiMobileDesigner.prototype.StyleConditionsContainer = function (form, width, height) {
    var container = document.createElement("div");
    var jsObject = container.jsObject = this;
    container.className = "stiSimpleContainerWithBorder";
    container.style.overflow = "auto";
    container.styleConditionsForm = form;
    container.selectedItem = null;

    if (width) container.style.width = width + "px";
    if (height) container.style.height = height + "px";

    container.addCondition = function (conditionObject, notAction) {
        var conditionItem = jsObject.StyleConditionsItem(this, conditionObject);
        this.appendChild(conditionItem);
        conditionItem.setSelected();
        if (!notAction) this.onAction();
    }

    container.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        form.controls.moveUpButton.setEnabled(false);
        form.controls.moveDownButton.setEnabled(false);
    }

    container.getCountItems = function () {
        return this.childNodes.length;
    }

    container.getOverItemIndex = function () {
        for (var i = 0; i < this.childNodes.length; i++)
            if (this.childNodes[i].isOver) return i;

        return null;
    }

    container.getItemIndex = function (item) {
        for (var i = 0; i < this.childNodes.length; i++)
            if (this.childNodes[i] == item) return i;

        return null;
    }

    container.getItemByIndex = function (index) {
        if (index != null && !this.hintText && index >= 0 && index < this.childNodes.length) {
            return this.childNodes[index];
        }

        return null;
    }

    container.getSelectedItemIndex = function () {
        return this.selectedItem ? this.getItemIndex(this.selectedItem) : null;
    }

    container.moveItem = function (fromIndex, toIndex) {
        if (fromIndex < this.childNodes.length && toIndex < this.childNodes.length) {
            var fromItem = this.childNodes[fromIndex];
            if (fromIndex < toIndex) {
                if (toIndex < this.childNodes.length - 1) {
                    this.insertBefore(fromItem, this.childNodes[toIndex + 1]);
                }
                else {
                    this.appendChild(fromItem);
                }
            }
            else {
                this.insertBefore(fromItem, this.childNodes[toIndex]);
            }
            return fromItem;
        }
    }

    container.onmouseup = function (event) {
        if (event.button != 2 && jsObject.options.itemInDrag) {
            var itemObject = jsObject.CopyObject(jsObject.options.itemInDrag.itemObject);
            if (!itemObject) return;
            var typeItem = itemObject.typeItem;

            if (typeItem == "ConditionsItem") {
                var toIndex = this.getOverItemIndex();
                var fromIndex = this.getSelectedItemIndex();
                if (toIndex != null && fromIndex != null && fromIndex != toIndex) {
                    container.moveItem(fromIndex, toIndex);
                    container.onAction();
                }
            }
        }

        return false;
    }

    container.onAction = function () {
        var count = this.getCountItems();
        var index = this.selectedItem ? this.selectedItem.getIndex() : -1;
        form.controls.moveUpButton.setEnabled(index > 0);
        form.controls.moveDownButton.setEnabled(index != -1 && index < count - 1);
    }

    return container;
}

StiMobileDesigner.prototype.StyleConditionsItem = function (container, conditionObject) {
    var conditionItem = document.createElement("div");
    var jsObject = conditionItem.jsObject = this;
    conditionItem.key = this.generateKey();
    conditionItem.container = container;
    conditionItem.isSelected = false;
    conditionItem.className = "stiDesignerSortPanel";

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "100%";
    conditionItem.appendChild(mainTable);
    var mainCell = mainTable.addCell();
    conditionItem.innerTable = this.CreateHTMLTable();
    conditionItem.innerTable.style.margin = "6px";
    mainCell.appendChild(conditionItem.innerTable);

    //Remove Button
    var removeButton = this.StandartSmallButton(null, null, null, "RemoveBlack.png");
    removeButton.style.margin = "2px 2px 2px 0px";
    removeButton.style.visibility = "hidden";
    removeButton.style.display = "inline-block";
    conditionItem.removeButton = removeButton;
    var closeCell = mainTable.addCell(removeButton);
    closeCell.style.width = "1px";
    closeCell.style.verticalAlign = "top";

    removeButton.action = function () {
        conditionItem.remove();
    }

    conditionItem.controls = {};
    if (!conditionObject) conditionObject = this.StyleConditionObject();

    var placementItems = [
        ["ReportTitle", this.loc.Components.StiReportTitleBand],
        ["ReportSummary", this.loc.Components.StiReportSummaryBand],
        ["PageHeader", this.loc.Components.StiPageHeaderBand],
        ["PageFooter", this.loc.Components.StiPageFooterBand],
        ["GroupHeader", this.loc.Components.StiGroupHeaderBand],
        ["GroupFooter", this.loc.Components.StiGroupFooterBand],
        ["Header", this.loc.Components.StiHeaderBand],
        ["Footer", this.loc.Components.StiFooterBand],
        ["ColumnHeader", this.loc.Components.StiColumnHeaderBand],
        ["ColumnFooter", this.loc.Components.StiColumnFooterBand],
        ["Data", this.loc.Components.StiDataBand],
        ["DataOddStyle", this.loc.PropertyMain.OddStyle],
        ["DataEvenStyle", this.loc.PropertyMain.EvenStyle],
        ["Table", this.loc.Components.StiTable],
        ["Hierarchical", this.loc.Components.StiHierarchicalBand],
        ["Child", this.loc.Components.StiChildBand],
        ["Empty", this.loc.Components.StiEmptyBand],
        ["Overlay", this.loc.Components.StiOverlayBand],
        ["Panel", this.loc.Components.StiPanel],
        ["Page", this.loc.Components.StiPage]
    ]

    var componentTypeItems = [
        ["Text", this.loc.PropertyEnum.StiStyleComponentTypeText],
        ["Primitive", this.loc.PropertyEnum.StiStyleComponentTypePrimitive],
        ["Image", this.loc.PropertyEnum.StiStyleComponentTypeImage],
        ["CrossTab", this.loc.PropertyEnum.StiStyleComponentTypeCrossTab],
        ["Chart", this.loc.PropertyEnum.StiStyleComponentTypeChart],
        ["CheckBox", this.loc.PropertyEnum.StiStyleComponentTypeCheckBox]
    ]

    var locationItems = [
        ["TopLeft", this.loc.PropertyEnum.ContentAlignmentTopLeft],
        ["TopCenter", this.loc.PropertyEnum.ContentAlignmentTopCenter],
        ["TopRight", this.loc.PropertyEnum.ContentAlignmentTopRight],
        ["MiddleLeft", this.loc.PropertyEnum.ContentAlignmentMiddleLeft],
        ["MiddleCenter", this.loc.PropertyEnum.ContentAlignmentMiddleCenter],
        ["MiddleRight", this.loc.PropertyEnum.ContentAlignmentMiddleRight],
        ["BottomLeft", this.loc.PropertyEnum.ContentAlignmentBottomLeft],
        ["BottomCenter", this.loc.PropertyEnum.ContentAlignmentBottomCenter],
        ["BottomRight", this.loc.PropertyEnum.ContentAlignmentBottomRight],
        ["Left", this.loc.PropertyEnum.StiHorAlignmentLeft],
        ["Right", this.loc.PropertyEnum.StiHorAlignmentRight],
        ["Top", this.loc.PropertyEnum.StiVertAlignmentTop],
        ["Bottom", this.loc.PropertyEnum.StiVertAlignmentBottom],
        ["CenterHorizontal", this.loc.Toolbars.CenterHorizontally],
        ["CenterVertical", this.loc.Toolbars.CenterVertically]
    ]

    var controlProps = [
        ["placementCheckBox", this.CheckBox(null), "4px 4px 4px 50px"],
        ["operationPlacement", this.DropDownList(null, 140, null, this.GetFilterConditionItems("Boolean"), true), "4px 4px 4px 0"],
        ["placement", this.StylePlacementCollectionControl(conditionItem.key + "placement", placementItems, 200, null, null, true), "4px"],
        ["placementNestedLevelCheckBox", this.CheckBox(null), "4px 0 4px 4px"],
        ["operationPlacementNestedLevel", this.DropDownList(null, 149, null, this.GetFilterConditionItems("Numeric", true), true), "4px"],
        ["placementNestedLevel", this.TextBoxEnumerator(null, 33, null, false, 10000000, 1), "4px"],
        ["componentTypeCheckBox", this.CheckBox(null), "4px 4px 4px 50px"],
        ["operationComponentType", this.DropDownList(null, 140, null, this.GetFilterConditionItems("Boolean"), true), "4px 4px 4px 0"],
        ["componentType", this.CollectionControl(conditionItem.key + "componentType", componentTypeItems, 200, null, null, true), "4px"],
        ["locationCheckBox", this.CheckBox(null), "4px 4px 4px 50px"],
        ["operationLocation", this.DropDownList(null, 140, null, this.GetFilterConditionItems("Boolean"), true), "4px 4px 4px 0"],
        ["location", this.CollectionControl(conditionItem.key + "location", locationItems, 200, null, null, true), "4px"],
        ["componentNameCheckBox", this.CheckBox(null), "4px 4px 4px 50px"],
        ["operationComponentName", this.DropDownList(null, 140, null, this.GetFilterConditionItems("String", true), true), "4px 4px 4px 0"],
        ["componentName", this.TextBox(200), "4px"]
    ]

    var nestedLevelTable = this.CreateHTMLTable();

    for (var i = 0; i < controlProps.length; i++) {
        var control = controlProps[i][1];
        control.style.margin = controlProps[i][2];
        conditionItem.controls[controlProps[i][0]] = control;

        if (controlProps[i][0] == "placementCheckBox") {
            conditionItem.innerTable.addTextCellInLastRow(this.loc.PropertyEnum.StiStyleConditionTypePlacement).style.paddingLeft = "4px";
        }

        if (controlProps[i][0] == "componentTypeCheckBox") {
            conditionItem.innerTable.addTextCellInLastRow(this.loc.PropertyEnum.StiStyleConditionTypeComponentType).style.paddingLeft = "4px";
        }

        if (controlProps[i][0] == "locationCheckBox") {
            conditionItem.innerTable.addTextCellInLastRow(this.loc.PropertyEnum.StiStyleConditionTypeLocation).style.paddingLeft = "4px";
        }

        if (controlProps[i][0] == "componentNameCheckBox") {
            conditionItem.innerTable.addTextCellInLastRow(this.loc.PropertyEnum.StiStyleConditionTypeComponentName).style.paddingLeft = "4px";
        }

        if (controlProps[i][0] == "placementNestedLevelCheckBox") {
            conditionItem.innerTable.addCellInLastRow();
            conditionItem.innerTable.addCellInLastRow();
            conditionItem.innerTable.addCellInLastRow(nestedLevelTable).setAttribute("colspan", "3");
        }

        if (controlProps[i][0] == "placementNestedLevelCheckBox" || controlProps[i][0] == "operationPlacementNestedLevel" || controlProps[i][0] == "placementNestedLevel") {
            if (controlProps[i][0] == "placementNestedLevelCheckBox") {
                nestedLevelTable.addTextCell(this.loc.PropertyMain.NestedLevel).style.padding = "0 4px 0 53px";
            }
            nestedLevelTable.addCell(control);
        }
        else {
            conditionItem.innerTable.addCellInLastRow(control);
        }

        if (controlProps[i][0] == "placement" || controlProps[i][0] == "componentType" || controlProps[i][0] == "location" || controlProps[i][0] == "placementNestedLevel") {
            if (controlProps[i][0] != "placementNestedLevel") conditionItem.innerTable.addTextCellInLastRow(this.loc.FormBand.And).style.paddingLeft = "4px";
            conditionItem.innerTable.addRow();
        }
    }

    conditionItem.updateControls = function () {
        conditionItem.controls.operationPlacement.setEnabled(conditionItem.controls.placementCheckBox.isChecked);
        conditionItem.controls.placement.setEnabled(conditionItem.controls.placementCheckBox.isChecked);
        conditionItem.controls.placementNestedLevelCheckBox.setEnabled(conditionItem.controls.placementCheckBox.isChecked);
        conditionItem.controls.operationPlacementNestedLevel.setEnabled(conditionItem.controls.placementCheckBox.isChecked && conditionItem.controls.placementNestedLevelCheckBox.isChecked);
        conditionItem.controls.placementNestedLevel.setEnabled(conditionItem.controls.placementCheckBox.isChecked && conditionItem.controls.placementNestedLevelCheckBox.isChecked);
        conditionItem.controls.operationComponentType.setEnabled(conditionItem.controls.componentTypeCheckBox.isChecked);
        conditionItem.controls.componentType.setEnabled(conditionItem.controls.componentTypeCheckBox.isChecked);
        conditionItem.controls.operationLocation.setEnabled(conditionItem.controls.locationCheckBox.isChecked);
        conditionItem.controls.location.setEnabled(conditionItem.controls.locationCheckBox.isChecked);
        conditionItem.controls.operationComponentName.setEnabled(conditionItem.controls.componentNameCheckBox.isChecked);
        conditionItem.controls.componentName.setEnabled(conditionItem.controls.componentNameCheckBox.isChecked);
    }

    //Fill controls
    conditionItem.controls.placementCheckBox.setChecked(conditionObject.type.indexOf("Placement") >= 0);
    conditionItem.controls.placementNestedLevelCheckBox.setChecked(conditionObject.type.indexOf("PlacementNestedLevel") >= 0);
    conditionItem.controls.componentTypeCheckBox.setChecked(conditionObject.type.indexOf("ComponentType") >= 0);
    conditionItem.controls.locationCheckBox.setChecked(conditionObject.type.indexOf("Location") >= 0);
    conditionItem.controls.componentNameCheckBox.setChecked(conditionObject.type.indexOf("ComponentName") >= 0);
    conditionItem.controls.operationPlacement.setKey(conditionObject.operationPlacement);
    conditionItem.controls.placement.setKey(conditionObject.placement);
    conditionItem.controls.operationPlacementNestedLevel.setKey(conditionObject.operationPlacementNestedLevel);
    conditionItem.controls.placementNestedLevel.setValue(conditionObject.placementNestedLevel);
    conditionItem.controls.operationComponentType.setKey(conditionObject.operationComponentType);
    conditionItem.controls.componentType.setKey(conditionObject.componentType);
    conditionItem.controls.operationLocation.setKey(conditionObject.operationLocation);
    conditionItem.controls.location.setKey(conditionObject.location);
    conditionItem.controls.operationComponentName.setKey(conditionObject.operationComponentName);
    conditionItem.controls.componentName.value = conditionObject.componentName;
    conditionItem.updateControls();

    var checkBoxes = ["placementCheckBox", "placementNestedLevelCheckBox", "componentTypeCheckBox", "locationCheckBox", "componentNameCheckBox"];
    for (var i = 0; i < checkBoxes.length; i++) {
        conditionItem.controls[checkBoxes[i]].action = function () { conditionItem.updateControls(); }
    }

    conditionItem.getConditionObject = function () {
        var conditionObject = {};
        conditionObject.type = "";
        if (conditionItem.controls.placementCheckBox.isChecked) conditionObject.type += " Placement,";
        if (conditionItem.controls.placementNestedLevelCheckBox.isChecked && conditionItem.controls.placementCheckBox.isChecked) conditionObject.type += " PlacementNestedLevel,";
        if (conditionItem.controls.componentTypeCheckBox.isChecked) conditionObject.type += " ComponentType,";
        if (conditionItem.controls.locationCheckBox.isChecked) conditionObject.type += " Location,";
        if (conditionItem.controls.componentNameCheckBox.isChecked) conditionObject.type += " ComponentName,";
        conditionObject.operationPlacement = conditionItem.controls.operationPlacement.key;
        conditionObject.placement = conditionItem.controls.placement.key;
        conditionObject.operationPlacementNestedLevel = conditionItem.controls.operationPlacementNestedLevel.key;
        conditionObject.placementNestedLevel = conditionItem.controls.placementNestedLevel.textBox.value;
        conditionObject.operationComponentType = conditionItem.controls.operationComponentType.key;
        conditionObject.componentType = conditionItem.controls.componentType.key;
        conditionObject.operationLocation = conditionItem.controls.operationLocation.key;
        conditionObject.location = conditionItem.controls.location.key;
        conditionObject.operationComponentName = conditionItem.controls.operationComponentName.key;
        conditionObject.componentName = conditionItem.controls.componentName.value;

        return conditionObject;
    }

    conditionItem.setSelected = function () {
        for (var i = 0; i < container.childNodes.length; i++) {
            container.childNodes[i].className = "stiDesignerSortPanel";
            container.childNodes[i].isSelected = false;
            container.childNodes[i].removeButton.style.visibility = "hidden";
        }
        container.selectedItem = this;
        this.isSelected = true;
        this.className = "stiDesignerSortPanelSelected";
        this.removeButton.style.visibility = "visible";
    }

    conditionItem.remove = function () {
        if (container.selectedItem == this) {
            var prevItem = this.previousSibling;
            var nextItem = this.nextSibling;
            container.selectedItem = null;
            if (container.childNodes.length > 1) {
                if (nextItem) {
                    nextItem.setSelected();
                    container.selectedItem = nextItem;
                }
                else if (prevItem) {
                    prevItem.setSelected();
                    container.selectedItem = prevItem;
                }
            }
        }
        container.removeChild(this);
        container.onAction();
    }

    conditionItem.getIndex = function () {
        for (var i = 0; i < container.childNodes.length; i++)
            if (container.childNodes[i] == this) return i;
    };

    if (jsObject.options.isTouchDevice) {
        conditionItem.onclick = function () {
            if (!this.parentElement) return;
            this.setSelected();
        }
    }
    else {
        conditionItem.onmousedown = function (event) {
            this.setSelected();

            if (this.isTouchStartFlag || (event && event.target && event.target.nodeName && event.target.nodeName.toLowerCase() == "input")) return;
            event.preventDefault();

            var options = jsObject.options;
            if (options.controlsIsFocused && options.controlsIsFocused.action) {
                options.controlsIsFocused.blur();
                options.controlsIsFocused = null;
            }

            if (event.button != 2 && !options.controlsIsFocused) {
                var itemInDrag = jsObject.TreeItemForDragDrop({ name: jsObject.loc.PropertyMain.Condition, typeItem: "ConditionsItem" }, null, true);
                if (itemInDrag.button.captionCell) itemInDrag.button.captionCell.style.padding = "5px 20px 5px 10px";
                itemInDrag.beginingOffset = 0;
                options.itemInDrag = itemInDrag;
            }
        }

        conditionItem.onmouseover = function () {
            this.isOver = true;
            if (jsObject.options.itemInDrag && jsObject.options.itemInDrag.itemObject) {
                var typeItem = jsObject.options.itemInDrag.itemObject.typeItem;
                if (typeItem == "ConditionsItem") {
                    this.style.borderStyle = "dashed";
                    this.style.borderColor = jsObject.options.themeColors[jsObject.GetThemeColor()];
                }
            }
        }

        conditionItem.onmouseout = function () {
            this.isOver = false;
            this.style.borderStyle = "solid";
            this.style.borderColor = "";
        }
    }

    conditionItem.move = function (direction) {
        var index = this.getIndex();
        container.removeChild(this);
        var count = container.getCountItems();
        var newIndex = direction == "Up" ? index - 1 : index + 1;
        if (direction == "Up" && newIndex == -1) newIndex = 0;
        if (direction == "Down" && newIndex >= count) {
            container.appendChild(this);
            container.onAction();
            return;
        }
        container.insertBefore(this, container.childNodes[newIndex]);
        container.onAction();
    }

    return conditionItem;
}