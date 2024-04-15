

StiMobileDesigner.prototype.InitializeGroupHeaderForm_ = function () {
    var jsObject = this;
    var groupHeaderForm = this.BaseFormPanel("groupHeaderForm", this.loc.FormTitles.GroupConditionForm, 1);
    groupHeaderForm.dataTree = this.options.dataTree;
    groupHeaderForm.mode = "Expression";
    groupHeaderForm.controls = {};

    //Main Table
    var mainTable = this.CreateHTMLTable();    
    groupHeaderForm.container.appendChild(mainTable);
    groupHeaderForm.container.style.padding = "0px";

    //Buttons
    var buttonProps = [
        ["Expression", "GroupHeaderForm.GroupExpression.png", this.loc.PropertyMain.Expression],
        ["DataColumn", "GroupHeaderForm.GroupDataColumn.png", this.loc.PropertyMain.DataColumn],
        ["SummaryExpression", "GroupHeaderForm.GroupSummary.png", this.loc.PropertyMain.Summary]
    ];

    //Add Panels && Buttons
    var buttonsPanel = mainTable.addCell();
    var panelsContainer = mainTable.addCell();
    
    buttonsPanel.style.verticalAlign = "top";
    buttonsPanel.style.paddingTop = "6px";
    groupHeaderForm.mainButtons = {};
    groupHeaderForm.panels = {};

    for (var i = 0; i < buttonProps.length; i++) {
        var panel = document.createElement("Div");
        panel.className = "stiDesignerEditFormPanel";
        panel.style.height = buttonProps[i][0] == "SummaryExpression" ? "315px" : "350px";
        panel.style.width = "450px";

        panel.style.display = i == 0 ? "" : "none";
        panelsContainer.appendChild(panel);
        groupHeaderForm.panels[buttonProps[i][0]] = panel;        

        var button = this.FormTabPanelButton(null, buttonProps[i][2], buttonProps[i][1], buttonProps[i][2], null, { width: 24, height: 24 }, 34);
        groupHeaderForm.mainButtons[buttonProps[i][0]] = button;
        buttonsPanel.appendChild(button);
        button.panelName = buttonProps[i][0];

        button.action = function () {
            groupHeaderForm.setMode(this.panelName);
        }
    }

    //Text Areas
    var textAreas = ["Expression", "SummaryExpression"];
    for (var i = 0; i < textAreas.length; i++) {
        var height = textAreas[i] == "Expression" ? 323 : 288;
        var textArea = groupHeaderForm["textArea" + textAreas[i]] = this.TextArea("groupHeaderForm" + textAreas[i], 419, height);
        textArea.style.margin = "12px";
        groupHeaderForm.panels[textAreas[i]].appendChild(textArea);
        textArea.addInsertButton();
    }

    var controlProps = [
        ["sortDirection", this.loc.PropertyMain.SortDirection,
            this.DropDownList("groupHeaderFormSortDirection", 200, null, this.GetSortDirectionItems(), true, false, null, true), "6px"],
        ["summarySortDirection", this.loc.PropertyMain.SummarySortDirection,
            this.DropDownList("groupHeaderFormSummarySortDirection", 200, null, this.GetSortDirectionItems(), true, false, null, true), "6px"],
        ["summaryType", this.loc.PropertyMain.SummaryType,
            this.DropDownList("groupHeaderFormSummaryType", 200, null, this.GetSummaryTypeItems(), true, false, null, true), "6px"]
    ];

    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.margin = "6px 0 6px 0";

    var downCell = mainTable.addCellInNextRow(controlsTable)
    downCell.setAttribute("colspan", "2");
    downCell.className = "stiDesignerGroupHeaderFormControlsDownPanel";

    for (var i = 0; i < controlProps.length; i++) {
        groupHeaderForm.controls[controlProps[i][0] + "Row"] = i == 0 ? controlsTable.rows[0] : controlsTable.addRow();

        var textCell = controlsTable.addTextCellInLastRow(controlProps[i][1]);
        textCell.className = "stiDesignerCaptionControlsBigIntervals";
        textCell.style.minWidth = "146px";
        textCell.style.whiteSpace = "nowrap";
        textCell.style.padding = "0 6px 0 15px";

        var control = groupHeaderForm.controls[controlProps[i][0]] = controlProps[i][2];
        controlsTable.addCellInLastRow(control);
        control.style.margin = controlProps[i][3];
    }

    //Data Columns
    var columnsCont = document.createElement("div");
    columnsCont.className = "stiSimpleContainerWithBorder";
    columnsCont.style.margin = "12px";
    columnsCont.style.overflow = "auto";
    columnsCont.style.width = "423px";
    columnsCont.style.height = "323px";
    groupHeaderForm.panels.DataColumn.appendChild(columnsCont);

    //Form Methods
    groupHeaderForm.reset = function () {
        groupHeaderForm.dataTree.setKey("");
        groupHeaderForm.textAreaExpression.value = "";
        groupHeaderForm.textAreaSummaryExpression.value = "";
        groupHeaderForm.setMode("Expression");
    }

    groupHeaderForm.setMode = function (mode) {
        groupHeaderForm.mode = mode;

        for (var panelName in groupHeaderForm.panels) {
            groupHeaderForm.panels[panelName].style.display = mode == panelName ? "" : "none";
            groupHeaderForm.mainButtons[panelName].setSelected(mode == panelName);
        }

        var propertiesPanel = jsObject.options.propertiesPanel;
        propertiesPanel.setEnabled(mode == "Expression" || mode == "SummaryExpression");
        propertiesPanel.editFormControl = null;

        if (mode == "Expression") propertiesPanel.editFormControl = groupHeaderForm.textAreaExpression;
        if (mode == "SummaryExpression") propertiesPanel.editFormControl = groupHeaderForm.textAreaSummaryExpression;

        groupHeaderForm.controls.sortDirectionRow.style.display = mode == "SummaryExpression" ? "none" : "";
        groupHeaderForm.controls.summarySortDirectionRow.style.display = groupHeaderForm.controls.summaryTypeRow.style.display = mode == "SummaryExpression" ? "" : "none";
    }

    groupHeaderForm.onhide = function () {
        jsObject.options.propertiesPanel.setDictionaryMode(false);
    }

    groupHeaderForm.show = function () {

        groupHeaderForm.changeVisibleState(true);

        //Build Data Tree
        jsObject.options.propertiesPanel.setDictionaryMode(true);
        columnsCont.appendChild(groupHeaderForm.dataTree);
        groupHeaderForm.dataTree.build(null, null, null, true);

        groupHeaderForm.dataTree.action = function () {
            groupHeaderForm.action();
        }

        //Reset Controls
        groupHeaderForm.reset();

        var selectedObject = jsObject.options.selectedObject || jsObject.GetCommonObject(jsObject.options.selectedObjects);
        if (!selectedObject) return;

        var conditionValue = selectedObject.properties["condition"] != null && selectedObject.properties["condition"] != "StiEmptyValue"
            ? StiBase64.decode(selectedObject.properties["condition"]) : "";
        var subStringConditionValue = conditionValue.length > 1 ? conditionValue.substring(1, conditionValue.length - 1) : "";

        groupHeaderForm.textAreaExpression.value = conditionValue;

        if (groupHeaderForm.dataTree.setKey(subStringConditionValue)) {
            groupHeaderForm.setMode("DataColumn");
        }
        else {
            groupHeaderForm.setMode("Expression");
            groupHeaderForm.textAreaExpression.focus();
        }

        groupHeaderForm.textAreaSummaryExpression.value = selectedObject.properties["summaryExpression"] && selectedObject.properties["summaryExpression"] != "StiEmptyValue" ? StiBase64.decode(selectedObject.properties["summaryExpression"]) : "";
        groupHeaderForm.controls.sortDirection.setKey(selectedObject.properties["sortDirection"] != "StiEmptyValue" ? selectedObject.properties["sortDirection"] : "0");
        groupHeaderForm.controls.summarySortDirection.setKey(selectedObject.properties["summarySortDirection"] != "StiEmptyValue" ? selectedObject.properties["summarySortDirection"] : "0");
        groupHeaderForm.controls.summaryType.setKey(selectedObject.properties["summaryType"] != "StiEmptyValue" ? selectedObject.properties["summaryType"] : "13");
    }

    groupHeaderForm.action = function () {
        groupHeaderForm.changeVisibleState(false);

        var condition = groupHeaderForm.textAreaExpression.value;

        if (groupHeaderForm.mode == "DataColumn") {
            condition = (groupHeaderForm.dataTree.key ? "{" + groupHeaderForm.dataTree.key + "}" : "");
        }

        var selectedObjects = jsObject.options.selectedObjects || [jsObject.options.selectedObject];
        if (!selectedObjects) return;

        for (var i = 0; i < selectedObjects.length; i++) {
            var selectedObject = selectedObjects[i];
            selectedObject.properties.condition = StiBase64.encode(condition);
            selectedObject.properties.summaryExpression = StiBase64.encode(groupHeaderForm.textAreaSummaryExpression.value);
            selectedObject.properties.sortDirection = groupHeaderForm.controls.sortDirection.key;
            selectedObject.properties.summarySortDirection = groupHeaderForm.controls.summarySortDirection.key;
            selectedObject.properties.summaryType = groupHeaderForm.controls.summaryType.key;
        }

        jsObject.SendCommandSendProperties(selectedObjects, ["condition", "summaryExpression", "sortDirection", "summarySortDirection", "summaryType"]);
    }

    return groupHeaderForm;
}