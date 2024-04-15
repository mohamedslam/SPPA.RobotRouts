
StiMobileDesigner.prototype.InitializeConditionsForm_ = function () {

    var form = this.BaseForm("conditionsForm", this.loc.PropertyMain.Conditions, 1, this.HelpLinks["conditions"]);
    form.controls = {};

    //Main Table
    var mainTable = this.CreateHTMLTable();
    form.container.appendChild(mainTable);
    form.container.style.padding = "6px 0 0 0";

    //Toolbar
    var buttons = [
        ["addCondition", this.FormButton(null, null, this.loc.Chart.AddCondition.replace("&", ""), null, null, null, null, null, "Down")],
        ["removeCondition", this.SmallButton(null, null, null, "Remove.png")],
        ["moveUp", this.SmallButton(null, null, null, "Arrows.ArrowUpBlue.png")],
        ["moveDown", this.SmallButton(null, null, null, "Arrows.ArrowDownBlue.png")]
    ]

    var toolBar = this.CreateHTMLTable();
    toolBar.style.marginLeft = "8px";
    form.container.appendChild(toolBar);

    for (var i = 0; i < buttons.length; i++) {
        var button = buttons[i][1];
        button.style.margin = "4px";
        form.controls[buttons[i][0]] = button;
        toolBar.addCell(button);
    }

    //Add Condition Menu
    var conditionsMenu = this.VerticalMenu("addConditionMenu", form.controls.addCondition, "Down", this.GetAddConditionMenuItems(), "stiDesignerMenuMiddleItem")
    form.controls.addCondition.action = function () { conditionsMenu.changeVisibleState(!conditionsMenu.visible); }

    var middleTable = this.CreateHTMLTable();
    form.container.appendChild(middleTable);

    //Items Container
    var itemsContainer = form.controls.itemsContainer = this.EasyContainer(180, 380);
    itemsContainer.style.margin = "12px";
    middleTable.addCell(itemsContainer);

    itemsContainer.updateNumbers = function () {
        for (var i = 0; i < itemsContainer.childNodes.length; i++) {
            var caption = itemsContainer.childNodes[i].caption;
            if (caption) {
                var oldText = caption.innerHTML;
                var newText = oldText.substring(oldText.indexOf(".") + 1);
                newText = (i + 1) + "." + newText;
                caption.innerHTML = newText;
            }
        }
    }

    itemsContainer.addItem_ = itemsContainer.addItem;

    itemsContainer.addItem = function (name, itemObject, caption, image, notAction) {
        var item = itemsContainer.addItem_(name, itemObject, caption, image, notAction);
        item.image.style.width = item.image.style.height = "24px";
        item.style.height = "34px";
        item.style.margin = "4px";

        return item;

    }

    //Main Container
    var mainContainer = form.controls.mainContainer = document.createElement("div");
    mainContainer.style.margin = "12px";
    mainContainer.style.width = "700px";
    mainContainer.style.height = "380px";

    middleTable.addCell(mainContainer).className = "stiDesignerConditionsFormMainContainerCell";
    mainContainer.conditionPanels = {};

    var conditionTypes = ["StiHighlightCondition", "StiDataBarCondition", "StiColorScaleCondition", "StiIconSetCondition"];
    for (var i = 0; i < conditionTypes.length; i++) {
        var conditionPanel;

        switch (conditionTypes[i]) {
            case "StiHighlightCondition": conditionPanel = form.jsObject.StiHighlightCondition(form); break;
            case "StiDataBarCondition": conditionPanel = form.jsObject.StiDataBarCondition(form); break;
            case "StiColorScaleCondition": conditionPanel = form.jsObject.StiColorScaleCondition(form); break;
            case "StiIconSetCondition": conditionPanel = form.jsObject.StiIconSetCondition(form); break;
        }

        if (conditionPanel) {
            mainContainer.appendChild(conditionPanel);
            mainContainer.conditionPanels[conditionTypes[i]] = conditionPanel;
            conditionPanel.style.display = "none";
        }
    }

    mainContainer.showConditionPanel = function (selectedItem) {
        for (var i = 0; i < conditionTypes.length; i++) {
            mainContainer.conditionPanels[conditionTypes[i]].style.display = "none";
        }
        if (selectedItem) {
            var currentConditionPanel = mainContainer.conditionPanels[selectedItem.itemObject.ConditionType];
            currentConditionPanel.style.display = "";
            currentConditionPanel.fillFromItemObject(selectedItem.itemObject);
        }
    }

    itemsContainer.onPreChangeSelection = function () {
        if (itemsContainer.selectedItem) {
            var currentPanel = mainContainer.conditionPanels[itemsContainer.selectedItem.itemObject.ConditionType];
            currentPanel.saveToItemObject(itemsContainer.selectedItem.itemObject);
        }
    }

    itemsContainer.onAction = function () {
        var count = itemsContainer.getCountItems();
        var index = itemsContainer.selectedItem ? itemsContainer.selectedItem.getIndex() : -1;
        form.controls.moveUp.setEnabled(index > 0);
        form.controls.moveDown.setEnabled(index != -1 && index < count - 1);
        form.controls.removeCondition.setEnabled(count > 0);
        itemsContainer.updateNumbers();
        mainContainer.showConditionPanel(itemsContainer.selectedItem);
    }

    form.controls.moveUp.action = function () {
        if (itemsContainer.selectedItem) {
            itemsContainer.onPreChangeSelection();
            itemsContainer.selectedItem.move("Up");
        }
    }

    form.controls.moveDown.action = function () {
        if (itemsContainer.selectedItem) {
            itemsContainer.onPreChangeSelection();
            itemsContainer.selectedItem.move("Down");
        }
    }

    form.controls.removeCondition.action = function () {
        if (itemsContainer.selectedItem) { itemsContainer.selectedItem.remove(); }
    }

    conditionsMenu.action = function (menuItem) {
        this.changeVisibleState(false);
        itemsContainer.onPreChangeSelection();
        var conditionType = menuItem.key;
        var conditionName = conditionType.replace("Sti", "");
        var caption = this.jsObject.loc.PropertyMain[conditionName];
        var conditionObject = this.jsObject.CopyObject(this.jsObject.options.conditions[conditionType]);
        var newItem = itemsContainer.addItem(conditionType, conditionObject, caption, "Conditions.Small" + conditionType + ".png");
        newItem.action();
    }

    form.onhide = function () {
        this.jsObject.DeleteTemporaryMenus();
    }

    form.show = function (conditionsValue) {
        var commonSelectedObject = this.jsObject.options.selectedObject || this.jsObject.GetCommonObject(this.jsObject.options.selectedObjects);
        itemsContainer.clear();
        for (var itemName in conditionsMenu.items) {
            if (itemName != "StiHighlightCondition")
                conditionsMenu.items[itemName].setEnabled(conditionsValue != null ||
                    (commonSelectedObject && (commonSelectedObject.typeComponent == "StiText" || commonSelectedObject.typeComponent == "StiTextInCells")));
        }

        if (conditionsValue != null || (commonSelectedObject && commonSelectedObject.properties.conditions && commonSelectedObject.properties.conditions != "StiEmptyValue")) {
            var conditions = conditionsValue || commonSelectedObject.properties.conditions ? form.jsObject.CopyObject(JSON.parse(StiBase64.decode(conditionsValue || commonSelectedObject.properties.conditions))) : [];
            var newItem = null;
            for (var i = 0; i < conditions.length; i++) {
                var conditionName = conditions[i].ConditionType.replace("Sti", "");
                var caption = this.jsObject.loc.PropertyMain[conditionName];
                newItem = itemsContainer.addItem(conditions[i].ConditionType, conditions[i], caption, "Small" + conditions[i].ConditionType + ".png");
            }
            if (newItem) newItem.action();
        }

        form.changeVisibleState(true);
    }

    form.action = function () {
        form.changeVisibleState(false);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        if (selectedObjects) {
            itemsContainer.onPreChangeSelection();
            var conditions = [];
            for (var i = 0; i < itemsContainer.childNodes.length; i++) {
                conditions.push(itemsContainer.childNodes[i].itemObject);
            }
            for (var i = 0; i < selectedObjects.length; i++) {
                selectedObjects[i].properties.conditions = conditions.length > 0 ? StiBase64.encode(JSON.stringify(conditions)) : "";
            }
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["conditions"]);
    }

    return form;
}

//Conditions Panel
StiMobileDesigner.prototype.InitializeConditionsPanel = function (form) {
    var panel = document.createElement("div");
    panel.jsObject = this;
    panel.className = "stiDesignerConditionPanel";
    panel.controls = {};

    panel.textContainer = function (text, width) {
        var container = document.createElement("div");
        container.innerHTML = text;
        container.className = "stiDesignerTextContainer";
        if (width) container.style.width = width + "px";

        return container;
    }

    return panel;
}

//Highlight Condition
StiMobileDesigner.prototype.StiHighlightCondition = function (form) {
    var jsObject = this;
    var panel = this.InitializeConditionsPanel(form);

    var headerTable = this.CreateHTMLTable();
    panel.appendChild(headerTable);

    var addLevelButton = this.SmallButton(null, null, this.loc.FormConditions.AddLevel, null, null, null, "stiDesignerFormButton", true);
    addLevelButton.style.margin = "4px";
    headerTable.addCell(addLevelButton);

    var radioButtonAnd = this.RadioButton(null, null, this.loc.PropertyEnum.StiFilterModeAnd);
    var radioButtonOr = this.RadioButton(null, null, this.loc.PropertyEnum.StiFilterModeOr);
    radioButtonAnd.style.margin = "4px 4px 4px 10px";
    radioButtonOr.style.margin = "4px";
    headerTable.addCell(radioButtonAnd);
    headerTable.addCell(radioButtonOr);
    radioButtonAnd.action = function () { this.setChecked(true); radioButtonOr.setChecked(false); };
    radioButtonOr.action = function () { this.setChecked(true); radioButtonAnd.setChecked(false); };

    var breakIfTrue = this.CheckBox(null, this.loc.FormConditions.BreakIfTrue);
    breakIfTrue.style.marginRight = "10px";
    headerTable.addCell().style.width = "100%";
    headerTable.addCell(breakIfTrue);

    var filterControl = this.FilterControl("conditionsFormFilterControl" + this.generateKey(), null, null, null, true);
    filterControl.controls.toolBar.style.display = "none";
    filterControl.controls.filterContainer.className = null;
    panel.appendChild(filterControl);

    filterControl.controls.filterContainer.onAction = function () {
        for (var i = 0; i < this.childNodes.length; i++) {
            this.childNodes[i].removeButton.style.display = this.childNodes.length > 1 ? "" : "none";
        }
        radioButtonAnd.style.display = this.childNodes.length > 1 ? "" : "none";
        radioButtonOr.style.display = this.childNodes.length > 1 ? "" : "none";
    }

    addLevelButton.action = function () {
        filterControl.controls.addFilter.action();
    };

    var sampleBar = this.CreateHTMLTable();
    sampleBar.className = "stiDesignerConditionsFormSampleBar";
    sampleBar.style.width = "100px";
    sampleBar.style.height = "50px";
    sampleBar.style.margin = "4px";

    var sampleBarText = document.createElement("div");
    sampleBarText.style.overflow = "hidden";
    sampleBarText.style.maxWidth = "98px";
    sampleBarText.style.maxHeight = "48px";

    var textCell = sampleBar.addCell(sampleBarText);
    textCell.style.textAlign = "center";

    sampleBar.update = function () {
        sampleBarText.innerHTML = "AaBbCcYyZz";
        var selectedItem = panel.controls.Style.key == "[None]" ? null : panel.controls.Style.getItemByKey(panel.controls.Style.key);
        var permissionsControls = panel.controls.Permissions.childControls;
        var fontAttr = selectedItem && selectedItem.styleProperties.font ? selectedItem.styleProperties.font.split("!") : null;
        var brushColor = selectedItem && selectedItem.styleProperties.brush ? jsObject.GetColorFromBrushStr(selectedItem.styleProperties.brush) : (permissionsControls.BackColor.isChecked ? panel.controls.BackColor.key : "255,255,255");
        var textColor = selectedItem && selectedItem.styleProperties.textBrush ? jsObject.GetColorFromBrushStr(selectedItem.styleProperties.textBrush) : (permissionsControls.TextColor.isChecked ? panel.controls.TextColor.key : "0,0,0");
        var fontObject = panel.controls.Font.key.split("!");

        sampleBarText.style.fontFamily = fontAttr ? fontAttr[0] : (permissionsControls.Font.isChecked ? fontObject[0] : "Arial");
        sampleBarText.style.fontSize = fontAttr ? fontAttr[1] + "pt" : (permissionsControls.FontSize.isChecked ? fontObject[1] + "pt" : "8pt");
        sampleBarText.style.fontWeight = fontAttr ? (fontAttr[2] == "1" ? "bold" : "") : (permissionsControls.FontStyleBold.isChecked && fontObject[2] == "1" ? "bold" : "");
        sampleBarText.style.fontStyle = fontAttr ? (fontAttr[3] == "1" ? "italic" : "") : (permissionsControls.FontStyleItalic.isChecked && fontObject[3] == "1" ? "italic" : "");
        sampleBarText.style.textDecoration = fontAttr ? (fontAttr[4] == "1" ? "underline" : "") : (permissionsControls.FontStyleUnderline.isChecked && fontObject[4] == "1" ? "underline" : "");

        if (brushColor != "transparent") brushColor = "rgb(" + brushColor + ")";
        sampleBar.style.background = brushColor;

        if (textColor != "transparent") textColor = "rgb(" + textColor + ")";
        sampleBar.style.color = textColor;
    }

    var mainToolBar = this.CreateHTMLTable();
    mainToolBar.style.width = "100%";
    mainToolBar.addCell(sampleBar);
    panel.appendChild(mainToolBar);

    var upToolBar = this.CreateHTMLTable();
    upToolBar.className = "stiDesignerConditionsFormToolbarCell";
    var subToolBarsCell = mainToolBar.addCell(upToolBar);
    subToolBarsCell.style.width = "100%";
    subToolBarsCell.style.verticalAlign = "top";

    var downToolBar = this.CreateHTMLTable();
    subToolBarsCell.appendChild(downToolBar);

    var checkBoxes = [
        ["Font", this.loc.PropertyMain.FontName],
        ["FontSize", this.loc.PropertyMain.FontSize],
        ["FontStyleBold", this.loc.PropertyMain.FontBold],
        ["FontStyleItalic", this.loc.PropertyMain.FontItalic],
        ["FontStyleUnderline", this.loc.PropertyMain.FontUnderline],
        ["FontStyleStrikeout", this.loc.PropertyMain.FontStrikeout],
        ["TextColor", this.loc.PropertyMain.TextColor],
        ["BackColor", this.loc.PropertyMain.BackColor],
        ["Borders", this.loc.PropertyMain.Borders]
    ]

    var controlProps = [
        ["Font", this.FontControl(null, 120), "4px"],
        ["TextColor", this.ColorControlWithImage(null, "TextColor.png", null, true), "4px 2px 4px 2px"],
        ["BackColor", this.ColorControlWithImage(null, "BackgroundColor.png", null, true), "4px 2px 4px 2px"],
        ["Borders", this.BordersControl(null, 120), "4px"],
        ["Permissions", this.CheckBoxesControl("ConditionsFormPermissions", null, "Permissions.png", checkBoxes, true), "4px"],
        ["Style", this.DropDownList(null, 120, null, this.GetComponentStyleItems(null, null, this.options.conditionsPredefinedStyles), true, false), "4px"],
        ["Enabled", this.CheckBox(null, this.loc.FormConditions.ComponentIsEnabled), "4px 10px 4px 4px"]//,
        //["CanAssignExpression", this.CheckBox(null, this.loc.FormConditions.AssignExpression), "4px"],
        //["AssignExpression", this.ExpressionControl(null, 150, null, null, true), "4px"]
    ]

    var table = upToolBar;
    for (var i = 0; i < controlProps.length; i++) {
        if (controlProps[i][0] == "separator") {
            var sep = this.HomePanelSeparator();
            table.addCell(sep);
            sep.style.margin = "4px 2px 4px 2px";
            continue;
        }

        var control = controlProps[i][1];
        control.propertyName = controlProps[i][0];
        control.style.margin = controlProps[i][2];
        panel.controls[control.propertyName] = control;

        if (control.propertyName == "Enabled") {
            table.addCell().style.width = "100%";
            table = downToolBar;
        }

        table.addCell(control);

        control.action = function () {
            sampleBar.update();
            panel.updateEnabledStates();

            if (this.propertyName == "Style" && this.key == "[None]") {
                this.textBox.value = jsObject.loc.FormConditions.SelectStyle;
            }
        }
    }
    table.addCell().style.width = "100%";
    panel.controls.TextColor.button.style.height = panel.controls.BackColor.button.style.height = (this.options.controlsHeight - 2) + "px";

    panel.controls.Style.setKey_ = panel.controls.Style.setKey;
    panel.controls.Style.setKey = function (key) {
        this.setKey_(key);

        if (this.key == "[None]") {
            this.textBox.value = jsObject.loc.FormConditions.SelectStyle;
        }
    }

    panel.updateEnabledStates = function () {
        //panel.controls.AssignExpression.setEnabled(panel.controls.CanAssignExpression.isChecked);
        panel.controls.Permissions.setEnabled(panel.controls.Style.key == "[None]");

        var permissions = panel.controls.Permissions.key;
        var fontControls = panel.controls.Font.innerControl.controls;

        fontControls.fontName.setEnabled((permissions == "All" || permissions == "Font" || permissions.indexOf("Font,") == 0 || permissions.indexOf(", Font,") >= 0 || panel.jsObject.EndsWith(permissions, "Font")) && panel.controls.Style.key == "[None]");
        fontControls.fontSize.setEnabled((permissions == "All" || permissions.indexOf("FontSize") >= 0) && panel.controls.Style.key == "[None]");
        fontControls.boldButton.setEnabled((permissions == "All" || permissions.indexOf("FontStyleBold") >= 0) && panel.controls.Style.key == "[None]");
        fontControls.italicButton.setEnabled((permissions == "All" || permissions.indexOf("FontStyleItalic") >= 0) && panel.controls.Style.key == "[None]");
        fontControls.underlineButton.setEnabled((permissions == "All" || permissions.indexOf("FontStyleUnderline") >= 0) && panel.controls.Style.key == "[None]");
        fontControls.strikeoutButton.setEnabled((permissions == "All" || permissions.indexOf("FontStyleStrikeout") >= 0) && panel.controls.Style.key == "[None]");

        panel.controls.TextColor.setEnabled((permissions == "All" || permissions.indexOf("TextColor") >= 0) && panel.controls.Style.key == "[None]");
        panel.controls.BackColor.setEnabled((permissions == "All" || permissions.indexOf("BackColor") >= 0) && panel.controls.Style.key == "[None]");
        panel.controls.Borders.setEnabled((permissions == "All" || permissions.indexOf("Borders") >= 0) && panel.controls.Style.key == "[None]");
    }

    panel.fillFromItemObject = function (itemObject) {
        filterControl.controls.filterContainer.clear();
        filterControl.fill(JSON.parse(StiBase64.decode(itemObject.Filters)), "1", "And");
        filterControl.controls.filterContainer.onAction();

        radioButtonAnd.setChecked(itemObject.FilterMode == null || itemObject.FilterMode == "And");
        radioButtonOr.setChecked(itemObject.FilterMode == "Or");

        breakIfTrue.setChecked(itemObject.BreakIfTrue);
        panel.controls.Style.setKey(itemObject.Style || "[None]");
        panel.controls.Enabled.setChecked(itemObject.Enabled);
        panel.controls.TextColor.setKey(itemObject.TextColor);
        panel.controls.BackColor.setKey(itemObject.BackColor);
        panel.controls.Font.setKey(itemObject.Font);
        panel.controls.Permissions.setKey(itemObject.Permissions);
        panel.controls.Borders.setKey(itemObject.BorderSides);

        //panel.controls.CanAssignExpression.setChecked(itemObject.CanAssignExpression);
        //panel.controls.AssignExpression.textBox.value = itemObject.CanAssignExpression ? StiBase64.decode(itemObject.AssignExpression) : "";

        sampleBar.update();
        panel.updateEnabledStates();
    }

    panel.saveToItemObject = function (itemObject) {
        var filters = filterControl.getValue();
        if (filters.filters.length > 1) itemObject.FilterMode = radioButtonAnd.isChecked ? "And" : "Or";
        itemObject.Filters = StiBase64.encode(JSON.stringify(filters.filters));
        itemObject.BreakIfTrue = breakIfTrue.isChecked;
        itemObject.Permissions = panel.controls.Permissions.key;
        itemObject.Style = panel.controls.Style.key == "[None]" ? "" : panel.controls.Style.key;
        itemObject.Enabled = panel.controls.Enabled.isChecked;
        itemObject.TextColor = panel.controls.TextColor.key;
        itemObject.BackColor = panel.controls.BackColor.key;
        itemObject.BorderSides = panel.controls.Borders.key;
        itemObject.Font = panel.controls.Font.key;
        //itemObject.CanAssignExpression = panel.controls.CanAssignExpression.isChecked;
        //itemObject.AssignExpression = StiBase64.encode(panel.controls.AssignExpression.textBox.value);
    }

    return panel;
}

//DataBar Condition
StiMobileDesigner.prototype.StiDataBarCondition = function (form) {
    var panel = this.InitializeConditionsPanel(form);

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "100%";
    mainTable.style.margin = "4px 0 4px 0";
    panel.appendChild(mainTable);

    var sampleBar = document.createElement("div");
    var innerContainer = document.createElement("div");
    sampleBar.appendChild(innerContainer);
    sampleBar.className = "stiDesignerConditionsFormSampleBar";
    sampleBar.style.margin = "0px";
    innerContainer.style.margin = "2px";
    innerContainer.style.width = "148px";
    innerContainer.style.border = "1px solid #ffffff";
    innerContainer.style.height = this.options.isTouchDevice ? "20px" : "15px";

    sampleBar.update = function () {
        var borderColor = panel.controls.ShowBorder.key == "Solid"
            ? (panel.controls.PositiveBorderColor.key != "transparent" ? "rgb(" + panel.controls.PositiveBorderColor.key + ")" : "#ffffff")
            : "#ffffff";
        innerContainer.style.border = "1px solid " + borderColor;

        var backColor = panel.controls.PositiveColor.key != "transparent" ? "rgb(" + panel.controls.PositiveColor.key + ")" : "transparent";
        if (panel.controls.BrushType.key == "Solid") {
            innerContainer.style.background = backColor;
        }
        else {
            var direction = panel.controls.Direction.key == "LeftToRight" || panel.controls.Direction.key == "Default" ? "left" : "right";
            if (panel.jsObject.GetNavigatorName() == "MSIE") {
                innerContainer.style.background = "-ms-linear-gradient(" + direction + ", " + backColor + ", #ffffff)";
            }
            else {
                innerContainer.style.background = "-webkit-gradient(linear, " + direction + " top, " + (direction == "left" ? "right" : "left") + " top, from(" + backColor + "), to(#ffffff))";
                innerContainer.style.background = "-moz-linear-gradient(" + direction + ",  " + backColor + ", #ffffff)";
            }
        }
    }

    var controlProps = [
        [this.loc.PropertyMain.Column, [{ name: "Column", control: this.DataControl(null, 280) }]],
        ["separator"],
        ["", [{ name: "MinimumCaption", control: panel.textContainer(this.loc.PropertyMain.Minimum, 176) },
        { name: "MaximumCaption", control: panel.textContainer(this.loc.PropertyMain.Maximum, 176) }]
        ],
        [this.loc.PropertyMain.Type, [{ name: "MinimumType", control: this.DropDownList(null, 170, null, this.GetConditionsMinimumTypeItems(), true) },
        { name: "MaximumType", control: this.DropDownList(null, 170, null, this.GetConditionsMaximumTypeItems(), true, false) }]
        ],
        [this.loc.PropertyMain.Value, [{ name: "MinimumValue", control: this.TextBox(null, 170) }, { name: "MaximumValue", control: this.TextBox(null, 170) }]],
        [this.loc.PropertyMain.Direction, [{ name: "Direction", control: this.DropDownList(null, 140, null, this.GetConditionsDirectionItems(), true) }]],
        ["separator"],
        ["", [{ name: "EmptyCaption", control: panel.textContainer("", 157) }, { name: "PositiveCaption", control: panel.textContainer(this.loc.PropertyMain.Positive, 156) },
        { name: "NegativeCaption", control: panel.textContainer(this.loc.PropertyMain.Negative, 156) }]
        ],
        [this.loc.PropertyMain.BrushType, [
            { name: "BrushType", control: this.DropDownList(null, 150, null, this.GetConditionsBrushTypeItems(), true, false, null, true) },
            { name: "PositiveColor", control: this.ColorControl(null, null, null, 150, true) }, { name: "NegativeColor", control: this.ColorControl(null, null, null, 150, true) }]
        ],
        [this.loc.PropertyMain.Borders, [
            { name: "ShowBorder", control: this.DropDownList(null, 150, null, this.GetConditionsShowBordersItems(), true, false, null, true) },
            { name: "PositiveBorderColor", control: this.ColorControl(null, null, null, 150, true) }, { name: "NegativeBorderColor", control: this.ColorControl(null, null, null, 150, true) }]
        ],
        [this.loc.FormFormatEditor.Sample, [{ name: "SampleBar", control: sampleBar }]]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        if (controlProps[i][0] == "separator") {
            var sep = this.FormSeparator();
            mainTable.addCellInLastRow(sep).setAttribute("colspan", "2");
            sep.style.margin = "4px 0 4px 0";
            sep.style.width = "100%";
            mainTable.addRow();
            continue;
        }
        var captionCell = mainTable.addTextCellInLastRow(controlProps[i][0]);
        captionCell.className = "stiDesignerCaptionControlsBigIntervals";
        captionCell.style.minWidth = "100px";
        captionCell.style.textAlign = "left";

        var innerTable = this.CreateHTMLTable();
        innerTable.style.margin = "4px";
        mainTable.addCellInLastRow(innerTable);
        var controlsArray = controlProps[i][1];

        for (var k = 0; k < controlsArray.length; k++) {
            var controlParams = controlsArray[k];
            var control = controlParams.control
            control.propertyName = controlParams.name;
            control.style.marginRight = "8px";
            panel.controls[control.propertyName] = control;
            innerTable.addCell(control);

            control.action = function () {
                sampleBar.update();
                panel.updateEnabledStates();
            }
        }

        mainTable.addRow();
    }

    panel.updateEnabledStates = function () {
        panel.controls.MinimumValue.setEnabled(panel.controls.MinimumType.key != "Auto" && panel.controls.MinimumType.key != "Minimum");
        panel.controls.MaximumValue.setEnabled(panel.controls.MaximumType.key != "Auto" && panel.controls.MinimumType.key != "Maximum");
        panel.controls.PositiveBorderColor.setEnabled(panel.controls.ShowBorder.key != "None");
        panel.controls.NegativeBorderColor.setEnabled(panel.controls.ShowBorder.key != "None");
    }

    panel.fillFromItemObject = function (itemObject) {
        panel.controls.Column.textBox.value = itemObject.Column;
        panel.controls.MaximumType.setKey(itemObject.MaximumType);
        panel.controls.MinimumType.setKey(itemObject.MinimumType);
        panel.controls.MinimumValue.value = itemObject.MinimumValue;
        panel.controls.MaximumValue.value = itemObject.MaximumValue;
        panel.controls.Direction.setKey(itemObject.Direction);
        panel.controls.BrushType.setKey(itemObject.BrushType);
        panel.controls.PositiveColor.setKey(itemObject.PositiveColor);
        panel.controls.NegativeColor.setKey(itemObject.NegativeColor);
        panel.controls.ShowBorder.setKey(itemObject.ShowBorder ? "Solid" : "None");
        panel.controls.PositiveBorderColor.setKey(itemObject.PositiveBorderColor);
        panel.controls.NegativeBorderColor.setKey(itemObject.NegativeBorderColor);

        panel.updateEnabledStates();
        sampleBar.update();
    }

    panel.saveToItemObject = function (itemObject) {
        itemObject.Column = panel.controls.Column.textBox.value;
        itemObject.MaximumType = panel.controls.MaximumType.key;
        itemObject.MinimumType = panel.controls.MinimumType.key;
        itemObject.MinimumValue = panel.controls.MinimumValue.value.toString();
        itemObject.MaximumValue = panel.controls.MaximumValue.value.toString();
        itemObject.Direction = panel.controls.Direction.key;
        itemObject.BrushType = panel.controls.BrushType.key;
        itemObject.PositiveColor = panel.controls.PositiveColor.key;
        itemObject.NegativeColor = panel.controls.NegativeColor.key;
        itemObject.ShowBorder = panel.controls.ShowBorder.key == "Solid" ? true : false;
        itemObject.PositiveBorderColor = panel.controls.PositiveBorderColor.key;
        itemObject.NegativeBorderColor = panel.controls.NegativeBorderColor.key;
    }

    return panel;
}

//ColorScale Condition
StiMobileDesigner.prototype.StiColorScaleCondition = function (form) {
    var panel = this.InitializeConditionsPanel(form);

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "100%";
    mainTable.style.margin = "4px 0 4px 0";
    panel.appendChild(mainTable);

    var sampleBar = document.createElement("div");
    var innerContainer = document.createElement("div");
    sampleBar.appendChild(innerContainer);
    sampleBar.className = "stiDesignerConditionsFormSampleBar";
    sampleBar.style.margin = "0px";
    innerContainer.style.margin = "2px";
    innerContainer.style.width = "332px";
    innerContainer.style.border = "1px solid #ffffff";
    innerContainer.style.height = this.options.isTouchDevice ? "20px" : "15px";

    sampleBar.update = function () {
        innerContainer.style.width = panel.controls.ScaleType.key == "Color3" ? "506px" : "332px";
        var minColor = panel.controls.MinimumColor.key != "transparent" ? "rgb(" + panel.controls.MinimumColor.key + ")" : "transparent";
        var maxColor = panel.controls.MaximumColor.key != "transparent" ? "rgb(" + panel.controls.MaximumColor.key + ")" : "transparent";

        if (panel.jsObject.GetNavigatorName() == "MSIE") {
            innerContainer.style.background = "-ms-linear-gradient(left, " + minColor + ", " + maxColor + ")";
        }
        else {
            innerContainer.style.background = "-webkit-gradient(linear, left top, right top, from(" + minColor + "), to(" + maxColor + "))";
            innerContainer.style.background = "-moz-linear-gradient(left,  " + minColor + ", " + maxColor + ")";
        }
    }

    var controlProps = [
        [this.loc.PropertyMain.Column, [{ name: "Column", control: this.DataControl(null, 280) }]],
        [this.loc.PropertyMain.ColorScaleType, [{ name: "ScaleType", control: this.DropDownList(null, 180, null, this.GetConditionsColorScaleTypeItems(), true, false) }]],
        ["separator"],
        ["", [{ name: "MinimumCaption", control: panel.textContainer(this.loc.PropertyMain.Minimum, 166) },
        { name: "MidCaption", control: panel.textContainer(this.loc.PropertyMain.Mid, 166) },
        { name: "MaximumCaption", control: panel.textContainer(this.loc.PropertyMain.Maximum, 166) }]
        ],
        [this.loc.PropertyMain.Type, [{ name: "MinimumType", control: this.DropDownList(null, 160, null, this.GetConditionsValueTypeItems(), true) },
        { name: "MidType", control: this.DropDownList(null, 160, null, this.GetConditionsValueTypeItems(), true, false) },
        { name: "MaximumType", control: this.DropDownList(null, 160, null, this.GetConditionsValueTypeItems(), true, false) }]
        ],
        [this.loc.PropertyMain.Value, [{ name: "MinimumValue", control: this.TextBox(null, 160) }, { name: "MidValue", control: this.TextBox(null, 160) },
        { name: "MaximumValue", control: this.TextBox(null, 160) }]
        ],
        [this.loc.PropertyMain.Color, [{ name: "MinimumColor", control: this.ColorControl(null, null, null, 160, true) },
        { name: "MidColor", control: this.ColorControl(null, null, null, 160, true) }, { name: "MaximumColor", control: this.ColorControl(null, null, null, 160, true) }]
        ],
        [this.loc.FormFormatEditor.Sample, [{ name: "SampleBar", control: sampleBar }]]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        if (controlProps[i][0] == "separator") {
            var sep = this.FormSeparator();
            mainTable.addCellInLastRow(sep).setAttribute("colspan", "2");
            sep.style.margin = "4px 0 4px 0";
            sep.style.width = "100%";
            mainTable.addRow();
            continue;
        }
        var captionCell = mainTable.addTextCellInLastRow(controlProps[i][0]);
        captionCell.className = "stiDesignerCaptionControlsBigIntervals";
        captionCell.style.minWidth = "100px";
        captionCell.style.textAlign = "left";

        var innerTable = this.CreateHTMLTable();
        innerTable.style.margin = "4px";
        mainTable.addCellInLastRow(innerTable);
        var controlsArray = controlProps[i][1];

        for (var k = 0; k < controlsArray.length; k++) {
            var controlParams = controlsArray[k];
            var control = controlParams.control
            control.propertyName = controlParams.name;
            control.style.marginRight = "8px";
            panel.controls[control.propertyName] = control;
            innerTable.addCell(control);

            control.action = function () {
                sampleBar.update();
                panel.updateEnabledStates();
            }
        }

        mainTable.addRow();
    }

    panel.updateEnabledStates = function () {
        panel.controls.MinimumValue.setEnabled(panel.controls.MinimumType.key != "Auto");
        panel.controls.MidValue.setEnabled(panel.controls.MidType.key != "Auto");
        panel.controls.MaximumValue.setEnabled(panel.controls.MaximumType.key != "Auto");
        panel.controls.MidCaption.style.display = panel.controls.ScaleType.key == "Color3" ? "" : "none";
        panel.controls.MidType.style.display = panel.controls.ScaleType.key == "Color3" ? "" : "none";
        panel.controls.MidValue.style.display = panel.controls.ScaleType.key == "Color3" ? "" : "none";
        panel.controls.MidColor.style.display = panel.controls.ScaleType.key == "Color3" ? "" : "none";
    }

    panel.fillFromItemObject = function (itemObject) {
        panel.controls.Column.textBox.value = itemObject.Column;
        panel.controls.ScaleType.setKey(itemObject.ScaleType);
        panel.controls.MinimumType.setKey(itemObject.MinimumType);
        panel.controls.MidType.setKey(itemObject.MidType);
        panel.controls.MaximumType.setKey(itemObject.MaximumType);
        panel.controls.MinimumValue.value = itemObject.MinimumValue;
        panel.controls.MidValue.value = itemObject.MidValue;
        panel.controls.MaximumValue.value = itemObject.MaximumValue;
        panel.controls.MinimumColor.setKey(itemObject.MinimumColor);
        panel.controls.MidColor.setKey(itemObject.MidColor);
        panel.controls.MaximumColor.setKey(itemObject.MaximumColor);

        panel.updateEnabledStates();
        sampleBar.update();
    }

    panel.saveToItemObject = function (itemObject) {
        itemObject.Column = panel.controls.Column.textBox.value;
        itemObject.ScaleType = panel.controls.ScaleType.key;

        itemObject.MinimumType = panel.controls.MinimumType.key;
        itemObject.MidType = panel.controls.MidType.key;
        itemObject.MaximumType = panel.controls.MaximumType.key;
        itemObject.MinimumValue = panel.controls.MinimumValue.value.toString();
        itemObject.MidValue = panel.controls.MidValue.value.toString();
        itemObject.MaximumValue = panel.controls.MaximumValue.value.toString();
        itemObject.MinimumColor = panel.controls.MinimumColor.key;
        itemObject.MidColor = panel.controls.MidColor.key;
        itemObject.MaximumColor = panel.controls.MaximumColor.key;
    }

    return panel;
}

//IconSet Condition
StiMobileDesigner.prototype.StiIconSetCondition = function (form) {
    var panel = this.InitializeConditionsPanel(form);
    panel.countIconSetItems = 5;

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "100%";
    mainTable.style.margin = "4px 0 4px 0";
    panel.appendChild(mainTable);

    var controlProps = [
        [this.loc.PropertyMain.Column, [{ name: "Column", control: this.DataControl(null, 280) }]],
        [this.loc.PropertyMain.IconSet, [{ name: "IconSet", control: this.IconSetControl("ConditionFormIconSet", 224) },
        { name: "Reverse", control: this.FormButton(null, null, this.loc.Buttons.Reverse) }]],
        [this.loc.PropertyMain.Alignment, [{ name: "ContentAlignment", control: this.DropDownList(null, 220, null, this.GetConditionsAlignmentItems(), true, null, null, true) }]],
        ["separator"],
        [this.loc.PropertyMain.Icon, [{ name: "OperationCaption", control: panel.textContainer(this.loc.PropertyMain.Operation, 96) },
        { name: "TypeCaption", control: panel.textContainer(this.loc.PropertyMain.Type, 136) },
        { name: "ValueCaption", control: panel.textContainer(this.loc.PropertyMain.Value, 136) }]
        ]
    ]

    for (var i = 1; i <= 5; i++) {
        var nameIconSet = "IconSetItem" + i;
        var iconSetItem =
            [
                [{ name: nameIconSet + "Icon", control: this.IndicatorsControl("ConditionsForm" + nameIconSet) }, { name: nameIconSet + "Text", control: panel.textContainer("when value is") }]
            ]
        if (i != 5) {
            iconSetItem.push([{ name: nameIconSet + "Operation", control: this.DropDownList(null, 90, null, this.GetConditionsOperationItems(), true, null, null, true) },
            { name: nameIconSet + "ValueType", control: this.DropDownList(null, 130, null, this.GetConditionsIconSetValueTypeItems(), true, null, null, true) },
            { name: nameIconSet + "Value", control: this.TextBox(null, 130) }
            ]);
        }
        else iconSetItem.push([]);
        controlProps.push(iconSetItem);
    }

    var addInnerTable = function (controlsArray, mainTable) {
        var innerTable = panel.jsObject.CreateHTMLTable();
        innerTable.style.margin = "4px";
        mainTable.addCellInLastRow(innerTable);

        for (var k = 0; k < controlsArray.length; k++) {
            var controlParams = controlsArray[k];
            var control = controlParams.control
            control.propertyName = controlParams.name;
            control.style.marginRight = "8px";
            panel.controls[control.propertyName] = control;
            innerTable.addCell(control);

            control.action = function () {
                if (this.propertyName.indexOf("Operation") != -1) { this.textBox.value = this.textBox.value.replace("&gt;", ">"); }
                if (this.propertyName == "IconSet") { panel.showIconSetRows(this.key); }
                if (this.propertyName == "Reverse") { panel.reverseIcons(); }
                panel.updateControls();
            }
        }
        return innerTable;
    }

    var rowIndex = 0;
    for (var i = 0; i < controlProps.length; i++) {
        var row = mainTable.addRow();
        if (controlProps[i][0] == "separator") {
            var sep = this.FormSeparator();
            mainTable.addCellInLastRow(sep).setAttribute("colspan", "2");
            sep.style.margin = "4px 0 4px 0";
            sep.style.width = "100%";
            mainTable.addRow();
            continue;
        }

        if (typeof (controlProps[i][0]) == "object") {
            var innerTable = addInnerTable(controlProps[i][0], mainTable);
            innerTable.style.margin = "4px 8px 4px 12px";
            rowIndex++;
        }
        else {
            var captionCell = mainTable.addTextCellInLastRow(controlProps[i][0]);
            captionCell.className = "stiDesignerCaptionControlsBigIntervals";
            captionCell.style.minWidth = "170px";
            captionCell.style.textAlign = "left";
        }
        addInnerTable(controlProps[i][1], mainTable);
        if (rowIndex > 0) {
            panel.controls["IconSetRow" + rowIndex] = row;
        }
    }

    panel.reverseIcons = function () {
        var icons = [];
        for (var i = 1; i < panel.countIconSetItems; i++) {
            var iconControl = panel.controls["IconSetItem" + i + "Icon"];
            if (iconControl) icons.push(iconControl.key);
        }
        icons.push(panel.controls.IconSetItem5Icon.key);
        panel.controls.IconSetItem5Icon.setKey(icons[0]);
        var k = 1;
        for (var i = panel.countIconSetItems - 1; i >= 1; i--) {
            var iconControl = panel.controls["IconSetItem" + i + "Icon"];
            if (iconControl) iconControl.setKey(icons[k]);
            k++;
        }
    }

    panel.fillIconSetItem = function (index, iconSetItemObject) {
        if (!iconSetItemObject) return;
        var properties = ["Icon", "Operation", "ValueType", "Value"];
        for (var i = 0; i < properties.length; i++) {
            var control = panel.controls["IconSetItem" + index + properties[i]];
            if (control) {
                if (control["setKey"]) control.setKey(iconSetItemObject[properties[i]]);
                else control.value = iconSetItemObject[properties[i]];
            }
        }
    }

    panel.getIconSetItemObject = function (index) {
        var iconSetItemObject = {};
        var properties = ["Icon", "Operation", "ValueType", "Value"];
        for (var i = 0; i < properties.length; i++) {
            var control = panel.controls["IconSetItem" + index + properties[i]];
            if (control) {
                if (control["key"]) iconSetItemObject[properties[i]] = control.key;
                else iconSetItemObject[properties[i]] = control.value.toString();
            }
        }
        return iconSetItemObject;
    }

    panel.showIconSetRows = function (iconSet) {
        var icons = panel.jsObject.options.iconSetArrays[iconSet];
        var count = icons ? icons.length : 5;
        panel.countIconSetItems = count;
        var values = { "3": [67, 33], "4": [75, 50, 25], "5": [80, 60, 40, 20] };
        for (var i = 1; i < count; i++) {
            panel.controls["IconSetItem" + i + "Value"].value = values[count.toString()][i - 1];
            panel.controls["IconSetItem" + i + "Operation"].setKey("MoreThanOrEqual");
            panel.controls["IconSetItem" + i + "ValueType"].setKey("Percent");
            panel.controls["IconSetItem" + i + "Icon"].setKey(icons && icons.length >= count ? icons[i - 1] : "None");
            panel.controls["IconSetRow" + i].style.display = "";
        }
        for (var i = count; i < 5; i++) {
            panel.controls["IconSetRow" + i].style.display = "none";
        }
        panel.controls["IconSetItem5Icon"].setKey(icons && icons.length >= count ? icons[count - 1] : "None");
    }

    panel.updateControls = function () {
        panel.controls.IconSetItem1Text.innerHTML = panel.jsObject.loc.Report.WhenValueIs;
        for (var i = 2; i <= 4; i++) {
            var whenAnd = panel.jsObject.loc.Report.WhenAnd;
            whenAnd = whenAnd.replace("{0}", panel.controls["IconSetItem" + (i - 1) + "Operation"].key == "MoreThan" ? "<=" : "<");
            whenAnd = whenAnd.replace("{1}", panel.controls["IconSetItem" + (i - 1) + "Value"].value);
            panel.controls["IconSetItem" + i + "Text"].innerHTML = whenAnd;
        }
        var preIndex = panel.countIconSetItems - 1;
        var when = panel.jsObject.loc.Report.When.replace("{0}", panel.controls["IconSetItem" + preIndex + "Operation"].key == "MoreThan" ? "<=" : "<");
        when = when.replace("{1}", panel.controls["IconSetItem" + preIndex + "Value"].value);
        panel.controls.IconSetItem5Text.innerHTML = when;
    }

    panel.fillFromItemObject = function (itemObject) {
        panel.showIconSetRows(itemObject.IconSet);
        panel.controls.Column.textBox.value = itemObject.Column;
        panel.controls.IconSet.setKey(itemObject.IconSet);
        panel.controls.ContentAlignment.setKey(itemObject.ContentAlignment);

        var count = 1;
        while (itemObject["IconSetItem" + count] != null) { count++; }
        count--;

        for (var i = 1; i < count; i++) {
            panel.fillIconSetItem(i, itemObject["IconSetItem" + i]);
        }
        panel.fillIconSetItem(5, itemObject["IconSetItem" + count]);
        panel.updateControls();
    }

    panel.saveToItemObject = function (itemObject) {
        itemObject.Column = panel.controls.Column.textBox.value;
        itemObject.IconSet = panel.controls.IconSet.key;
        itemObject.ContentAlignment = panel.controls.ContentAlignment.key;

        for (var i = 1; i <= panel.countIconSetItems; i++) {
            itemObject["IconSetItem" + i] = panel.getIconSetItemObject(i == panel.countIconSetItems ? 5 : i);
        }
        for (var i = panel.countIconSetItems + 1; i <= 5; i++) {
            itemObject["IconSetItem" + i] = null;
        }
    }

    return panel;
}