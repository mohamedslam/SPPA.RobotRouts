
StiMobileDesigner.prototype.InitializeDashboardInteractionForm_ = function () {
    var form = this.BaseForm("dashboardInteractionForm", this.loc.PropertyMain.Interaction, 1, this.HelpLinks["dashboardInteractions"]);
    form.controls = {};
    form.container.style.minWidth = "300px";

    var jsObject = this;

    var properties = [
        ["onHoverHeader", null, this.FormBlockHeader(this.loc.PropertyMain.OnHover), "6px 12px 6px 12px"],
        ["onHover", this.loc.PropertyMain.Mode, this.DropDownList("onHoverDbsInteraction", 220, null, this.GetOnHoverInteractionItems(), true, false, null, true), "6px 12px 6px 12px"],
        ["toolTip", this.loc.PropertyMain.ToolTip, this.RichTextBoxControl("toolTipDbsInteraction", 220), "6px 12px 6px 12px"],
        ["onClickHeader", null, this.FormBlockHeader(this.loc.PropertyMain.OnClick), "6px 12px 6px 12px"],
        ["onClick", this.loc.PropertyMain.Mode, this.DropDownList("onClickDbsInteraction", 220, null, this.GetOnClickInteractionItems(), true, false, null, true), "6px 12px 6px 12px"],
        ["hyperlink", this.loc.PropertyMain.Hyperlink, this.HyperlinkControlWithMenu("hyperlinkDbsInteraction", 220, []), "6px 12px 6px 12px"],
        ["drillDownPageKey", this.loc.Components.StiPage, this.DropDownList("drillDownPageKeyDbsInteraction", 220, null, [], true, false, null, true), "6px 12px 6px 12px"],
        ["newParameterButton", this.loc.PropertyMain.Parameters, this.FormButton(null, "newParameterButtonDbsInteraction", this.loc.MainMenu.menuEditDataParameterNew.replace("...", "")), "6px 12px 6px 12px"],
        ["drillDownParametersContainer", null, this.DashboardDrillDownParametersContainer(form, 379, this.loc.FormStyleDesigner.NotSpecified), "6px 12px 6px 12px"],
        ["drillDownParameterName", this.loc.PropertyMain.Name, this.DropDownList("drillDownParameterNameDbsInteraction", 220, null, [], false, false, null, true), "6px 12px 6px 12px"],
        ["drillDownParameterExpression", this.loc.PropertyMain.Value, this.HyperlinkControlWithMenu("drillDownParameterValueDbsInteraction", 220, []), "6px 12px 6px 12px"],
        ["onDataManipulationHeader", null, this.FormBlockHeader(this.loc.PropertyMain.OnDataManipulation), "6px 12px 6px 12px"],
        ["allowUserColumnSelection", null, this.CheckBox("allowUserColumnSelectionDbsInteraction", this.loc.Dashboard.AllowUserColumnSelection), "6px 12px 6px 24px"],
        ["allowUserSorting", null, this.CheckBox("allowUserSortingDbsInteraction", this.loc.Dashboard.AllowUserSorting), "6px 12px 6px 24px"],
        ["allowUserFiltering", null, this.CheckBox("allowUserFilteringDbsInteraction", this.loc.Dashboard.AllowUserFiltering), "6px 12px 6px 24px"],
        ["allowUserDrillDown", null, this.CheckBox("allowUserDrillDownDbsInteraction", this.loc.Dashboard.AllowUserDrillDown), "6px 12px 6px 24px"],
        ["drillDownFiltered", null, this.CheckBox("drillDownFiltered", this.loc.Dashboard.DrillDownFiltered), "6px 12px 6px 24px"],
        ["fullRowSelect", null, this.CheckBox("fullRowSelect", this.loc.Dashboard.FullRowSelect), "6px 12px 6px 24px"],
        ["layoutHeader", null, this.FormBlockHeader(this.loc.PropertyMain.Layout), "6px 12px 6px 12px"],
        ["viewsState", this.loc.QueryBuilder.Views, this.DropDownList("viewsStateDbsInteraction", 220, null, this.GetViewsStateInteractionItems(), true, false, null, true), "6px 12px 6px 12px"],
        ["showFullScreenButton", null, this.CheckBox("showFullScreenButtonDbsInteraction", this.loc.Buttons.ShowSpecific.replace("{0}", this.loc.FormViewer.FullScreen)), "6px 12px 6px 24px"],
        ["showSaveButton", null, this.CheckBox("showSaveButtonDbsInteraction", this.loc.Buttons.ShowSpecific.replace("{0}", this.loc.Buttons.Save)), "6px 12px 6px 24px"],
        ["showViewDataButton", null, this.CheckBox("showViewDataButtonDbsInteraction", this.loc.Buttons.ShowSpecific.replace("{0}", this.loc.FormTitles.ViewDataForm)), "6px 12px 6px 24px"],
        ["fieldInteractionsHeader", null, this.FormBlockHeader(this.loc.Dashboard.FieldInteractions), "6px 12px 6px 12px"],
        ["runFieldsEditorInfo", this.loc.Dashboard.RunFieldsEditorInfo, null, "6px 12px 6px 12px"],
        ["runFieldsEditorButton", null, this.FormButton(null, "runFieldsEditorButtonDbsInteraction", this.loc.Dashboard.RunFieldsEditor), "6px 12px 6px 12px"]
    ];

    var proprtiesTable = this.CreateHTMLTable();
    proprtiesTable.style.margin = "6px 0 6px 0";
    form.container.appendChild(proprtiesTable);

    for (var i = 0; i < properties.length; i++) {
        form.addControlRow(proprtiesTable, properties[i][1], properties[i][0], properties[i][2], properties[i][3]);
    }

    form.controls.allowUserSorting.captionCell.style.minWidth = form.controls.allowUserFiltering.captionCell.style.minWidth = form.controls.allowUserColumnSelection.captionCell.style.minWidth = "300px";
    form.controls.drillDownFiltered.captionCell.style.minWidth = form.controls.fullRowSelect.captionCell.style.minWidth = "300px";
    form.controls.showFullScreenButton.captionCell.style.minWidth = form.controls.showSaveButton.captionCell.style.minWidth = form.controls.showViewDataButton.captionCell.style.minWidth = "300px";
    form.controls.onHoverText.style.minWidth = form.controls.onClickText.style.minWidth = "130px";

    var runFieldsEditorInfoText = form.controls.runFieldsEditorInfoText;
    runFieldsEditorInfoText.setAttribute("colspan", "2");
    runFieldsEditorInfoText.style.color = "#949494";
    runFieldsEditorInfoText.style.padding = "6px 24px 6px 24px";
    runFieldsEditorInfoText.style.whiteSpace = "normal";
    runFieldsEditorInfoText.style.lineHeight = "1.5";

    form.controls.runFieldsEditorButton.style.display = "inline-block";
    form.controls.runFieldsEditorButton.parentElement.style.textAlign = "right";

    form.controls.runFieldsEditorButton.action = function () {
        var selectedObject = jsObject.options.selectedObject;
        if (selectedObject && selectedObject.typeComponent == "StiTableElement") {
            form.changeVisibleState(false);
            jsObject.ShowComponentForm(selectedObject);
        }
    };

    var newParameterButton = form.controls.newParameterButton;
    newParameterButton.style.display = "inline-block";
    newParameterButton.style.minWidth = "110px";
    newParameterButton.parentElement.style.textAlign = "right";

    var drillDownParametersContainer = form.controls.drillDownParametersContainer;

    drillDownParametersContainer.getItemCaption = function (name, expression) {
        if (!name) return jsObject.loc.FormStyleDesigner.NotSpecified;
        if (expression) return name + " - " + expression;
        return name;
    }

    newParameterButton.action = function () {
        var nameItems = form.getParameterNameItems();
        var itemName = jsObject.loc.PropertyMain.Name;

        if (nameItems.length > 2) {
            itemName = nameItems[2].name;
        }
        else {
            var index = 0;
            var flag = false;
            if (drillDownParametersContainer.getCountItems() > 0) {
                while (!flag) {
                    index++;
                    flag = true;
                    for (var i = 0; i < drillDownParametersContainer.childNodes.length; i++) {
                        if (drillDownParametersContainer.childNodes[i].itemObject.name.toLowerCase() == itemName.toLowerCase() + (index == 1 ? "" : index)) {
                            flag = false;
                            break;
                        }
                    }
                }
            }
            itemName += (index < 2 ? "" : index);
        }

        var item = drillDownParametersContainer.addItem(drillDownParametersContainer.getItemCaption(itemName), "Dashboards.Parameter.png", { name: itemName, expression: "" });
        item.select();
    }

    drillDownParametersContainer.onAction = function (actionName) {
        form.updateControlsVisibleStates();

        if (this.selectedItem) {
            form.controls.drillDownParameterName.setKey(this.selectedItem.itemObject.name);
            form.controls.drillDownParameterExpression.textBox.value = this.selectedItem.itemObject.expression;
        }
    }

    form.controls.drillDownParameterName.action = function () {
        if (drillDownParametersContainer.selectedItem) {
            if (this.key == "") this.setKey(jsObject.loc.PropertyMain.Name);
            drillDownParametersContainer.selectedItem.itemObject.name = this.key;
            drillDownParametersContainer.selectedItem.captionContainer.innerHTML = drillDownParametersContainer.getItemCaption(this.key, drillDownParametersContainer.selectedItem.itemObject.expression);
        }
    }

    form.controls.drillDownParameterExpression.action = function () {
        if (drillDownParametersContainer.selectedItem) {
            drillDownParametersContainer.selectedItem.itemObject.expression = this.textBox.value;
            drillDownParametersContainer.selectedItem.captionContainer.innerHTML = drillDownParametersContainer.getItemCaption(drillDownParametersContainer.selectedItem.itemObject.name, this.textBox.value);
        }
    }

    form.controls.onHover.action = function () {
        form.updateControlsVisibleStates();
    }

    form.controls.onClick.action = function () {
        form.updateControlsVisibleStates();
    }

    form.showOnEventsInteraction = function () {
        var ident = this.interaction.ident;
        return (ident == "Chart" || ident == "Image" || ident == "RegionMap" || ident == "TableColumn" || ident == "Text" || ident == "Indicator");
    }

    form.updateControlsVisibleStates = function () {
        var controls = form.controls;
        var ident = this.interaction.ident;

        //DataManipulation
        controls.onDataManipulationHeaderRow.style.display = this.interaction.allowUserSorting != null || this.interaction.allowUserFiltering != null ||
            this.interaction.allowUserDrillDown != null || this.interaction.allowUserColumnSelection != null || ident == "Table" ? "" : "none";
        controls.allowUserColumnSelectionRow.style.display = this.interaction.allowUserColumnSelection != null ? "" : "none";
        controls.allowUserSortingRow.style.display = this.interaction.allowUserSorting != null ? "" : "none";
        controls.allowUserFilteringRow.style.display = this.interaction.allowUserFiltering != null ? "" : "none";
        controls.allowUserDrillDownRow.style.display = this.interaction.allowUserDrillDown != null ? "" : "none";
        controls.drillDownFilteredRow.style.display = controls.fullRowSelectRow.style.display = ident == "Table" ? "" : "none";

        //Field Interactions
        controls.fieldInteractionsHeaderRow.style.display = controls.runFieldsEditorInfoRow.style.display = controls.runFieldsEditorButtonRow.style.display = ident == "Table" ? "" : "none";

        //OnHover
        controls.onHoverHeaderRow.style.display = controls.onHoverRow.style.display = controls.onClickHeaderRow.style.display = controls.onClickRow.style.display = form.showOnEventsInteraction() ? "" : "none";
        controls.toolTipRow.style.display = form.showOnEventsInteraction() && controls.onHover.key == "ShowToolTip" ? "" : "none";
        controls.hyperlinkRow.style.display = form.showOnEventsInteraction() && controls.onClick.key == "OpenHyperlink" ? "" : "none";

        //OnClick
        var onClickShowDashboard = form.showOnEventsInteraction() && controls.onClick.key == "ShowDashboard";
        controls.drillDownPageKeyRow.style.display = onClickShowDashboard ? "" : "none";
        controls.newParameterButtonRow.style.display = onClickShowDashboard ? "" : "none";
        controls.drillDownParametersContainerRow.style.display = onClickShowDashboard ? "" : "none";
        controls.drillDownParameterNameRow.style.display = controls.drillDownParameterExpressionRow.style.display = onClickShowDashboard && controls.drillDownParametersContainer.selectedItem ? "" : "none";

        //Layout
        controls.layoutHeaderRow.style.display = this.interaction.showFullScreenButton != null || this.interaction.showSaveButton != null || this.interaction.showViewDataButton != null ? "" : "none";
        controls.showFullScreenButtonRow.style.display = this.interaction.showFullScreenButton != null ? "" : "none";
        controls.showSaveButtonRow.style.display = this.interaction.showSaveButton != null ? "" : "none";
        controls.showViewDataButtonRow.style.display = this.interaction.showViewDataButton != null ? "" : "none";
        controls.viewsStateRow.style.display = this.interaction.viewsState ? "" : "none";

        form.correctTopPosition();
    }

    form.getParameterNameItems = function () {
        var items = [];
        items.push(jsObject.Item("itemTitle", "Title", null, "Title"));

        var variables = jsObject.GetVariablesFromDictionary(jsObject.options.report.dictionary);
        if (variables) {
            for (var i = 0; i < variables.length; i++) {
                items.push(jsObject.Item(variables[i].name, variables[i].name, null, variables[i].name));
            }
        }

        return items;
    }

    form.getDrillDownPageItems = function () {
        var items = [];

        if (jsObject.options.report) {
            items.push(jsObject.Item("notAssigned", "", null, ""));
            var currentPageName = jsObject.options.currentPage ? jsObject.options.currentPage.properties.name : null;

            for (var pageName in jsObject.options.report.pages) {
                if (pageName != currentPageName) {
                    var page = jsObject.options.report.pages[pageName];
                    var text = page.properties.aliasName && StiBase64.decode(page.properties.aliasName) != pageName ? pageName + " [" + StiBase64.decode(page.properties.aliasName) + "]" : pageName;
                    var pageKey = jsObject.options.report.pages[pageName].properties.pageKey;
                    items.push(jsObject.Item(pageKey, text, null, pageKey));
                }
            }
        }

        return items;
    }

    form.getResultInteraction = function () {
        var interaction = this.interaction || {};
        interaction.onHover = form.controls.onHover.key;
        interaction.toolTip = StiBase64.encode(form.controls.toolTip.key);
        interaction.onClick = form.controls.onClick.key;
        interaction.hyperlink = StiBase64.encode(form.controls.hyperlink.textBox.value);
        interaction.drillDownPageKey = form.controls.drillDownPageKey.key;
        interaction.viewsState = form.controls.viewsState.key;

        interaction.drillDownParameters = [];
        if (drillDownParametersContainer.getCountItems() > 0) {
            for (var i = 0; i < drillDownParametersContainer.childNodes.length; i++) {
                interaction.drillDownParameters.push(drillDownParametersContainer.childNodes[i].itemObject);
            }
        }

        interaction.allowUserColumnSelection = form.controls.allowUserColumnSelection.isChecked;
        interaction.allowUserSorting = form.controls.allowUserSorting.isChecked;
        interaction.allowUserFiltering = form.controls.allowUserFiltering.isChecked;
        interaction.drillDownFiltered = form.controls.drillDownFiltered.isChecked;
        interaction.fullRowSelect = form.controls.fullRowSelect.isChecked;
        interaction.allowUserDrillDown = form.controls.allowUserDrillDown.isChecked;
        interaction.showFullScreenButton = form.controls.showFullScreenButton.isChecked;
        interaction.showSaveButton = form.controls.showSaveButton.isChecked;
        interaction.showViewDataButton = form.controls.showViewDataButton.isChecked;

        return interaction;
    }

    form.show = function (interaction, columnNames, chartIsRange) {
        this.interaction = jsObject.CopyObject(interaction);

        if (this.interaction) {
            this.interactionIdent = this.interaction.ident;

            form.controls.hyperlink.interactionIdent = form.controls.toolTip.interactionIdent = form.controls.drillDownParameterExpression.interactionIdent = this.interactionIdent;
            form.controls.toolTip.columnNames = columnNames;
            form.controls.toolTip.chartIsRange = chartIsRange;
            form.controls.drillDownParameterExpression.columnNames = columnNames;

            var placeholder = "";

            switch (this.interaction.ident) {
                case "Chart": placeholder = "http://example.com/{argument}/{value}"; break;
                case "RegionMap": placeholder = "http://example.com/{ident}"; break;
                case "Indicator": placeholder = "http://example.com/{value}/{series}/{target}"; break;
                default: placeholder = "http://example.com/{value}"; break;
            }

            var expressionItems = jsObject.GetInsertExpressionItems(this.interaction.ident, columnNames, chartIsRange);
            var buttonWidth = jsObject.options.controlsButtonsWidth;
            var textBoxWidth = 220 - (buttonWidth + 2) * 2;
            var newTextBoxWidth = expressionItems.length > 0 ? textBoxWidth : (textBoxWidth + buttonWidth + 1);

            var hyperlinkControl = form.controls.hyperlink;
            hyperlinkControl.addItems(expressionItems);
            hyperlinkControl.textBox.setAttribute("placeholder", placeholder);
            hyperlinkControl.textBox.value = StiBase64.decode(this.interaction.hyperlink);
            hyperlinkControl.button.style.display = expressionItems.length > 0 ? "" : "none";
            hyperlinkControl.button.parentElement.style.borderRight = expressionItems.length > 0 ? "" : "0px";
            hyperlinkControl.editButton.parentElement.style.borderRight = expressionItems.length > 0 ? "0px" : "";
            hyperlinkControl.editButton.parentElement.style.borderRadius = expressionItems.length > 0 || !jsObject.allowRoundedControls() ? "0" : "0 3px 3px 0";
            hyperlinkControl.textBox.style.width = newTextBoxWidth + "px";

            form.controls.onClick.addItems(jsObject.GetOnClickInteractionItems(this.interaction.ident));
            form.controls.onHover.setKey(this.interaction.onHover);
            form.controls.toolTip.setKey(StiBase64.decode(this.interaction.toolTip));
            form.controls.onClick.setKey(this.interaction.onClick);

            form.controls.drillDownPageKey.addItems(form.getDrillDownPageItems());
            form.controls.drillDownPageKey.setKey(form.controls.drillDownPageKey.haveKey(this.interaction.drillDownPageKey) ? this.interaction.drillDownPageKey : "");
            form.controls.drillDownParameterName.addItems(form.getParameterNameItems());
            form.controls.drillDownParametersContainer.clear();

            var drillDownExpControl = form.controls.drillDownParameterExpression;
            drillDownExpControl.addItems(expressionItems);
            drillDownExpControl.button.style.display = expressionItems.length > 0 ? "" : "none";
            drillDownExpControl.button.parentElement.style.borderRight = expressionItems.length > 0 ? "" : "0px";
            drillDownExpControl.editButton.parentElement.style.borderRight = expressionItems.length > 0 ? "0px" : "";
            drillDownExpControl.editButton.parentElement.style.borderRadius = expressionItems.length > 0 || !jsObject.allowRoundedControls() ? "0" : "0 3px 3px 0";
            drillDownExpControl.textBox.style.width = newTextBoxWidth + "px";

            var drillDownParameters = this.interaction.drillDownParameters;
            if (drillDownParameters) {
                for (var i = 0; i < drillDownParameters.length; i++) {
                    var itemObject = { name: drillDownParameters[i].name, expression: drillDownParameters[i].expression };
                    drillDownParametersContainer.addItem(drillDownParametersContainer.getItemCaption(itemObject.name, itemObject.expression), "Dashboards.Parameter.png", itemObject);
                }
            }
            var firstItem = drillDownParametersContainer.getItemByIndex(0);
            if (firstItem) firstItem.select();

            if (this.interaction.allowUserColumnSelection != null) form.controls.allowUserColumnSelection.setChecked(this.interaction.allowUserColumnSelection);
            if (this.interaction.allowUserSorting != null) form.controls.allowUserSorting.setChecked(this.interaction.allowUserSorting);
            if (this.interaction.allowUserFiltering != null) form.controls.allowUserFiltering.setChecked(this.interaction.allowUserFiltering);
            if (this.interaction.allowUserDrillDown != null) form.controls.allowUserDrillDown.setChecked(this.interaction.allowUserDrillDown);
            if (this.interaction.drillDownFiltered != null) form.controls.drillDownFiltered.setChecked(this.interaction.drillDownFiltered);
            if (this.interaction.fullRowSelect != null) form.controls.fullRowSelect.setChecked(this.interaction.fullRowSelect);
            if (this.interaction.showFullScreenButton != null) form.controls.showFullScreenButton.setChecked(this.interaction.showFullScreenButton);
            if (this.interaction.showSaveButton != null) form.controls.showSaveButton.setChecked(this.interaction.showSaveButton);
            if (this.interaction.showViewDataButton != null) form.controls.showViewDataButton.setChecked(this.interaction.showViewDataButton);
            if (this.interaction.viewsState != null) form.controls.viewsState.setKey(this.interaction.viewsState);
        }

        form.updateControlsVisibleStates();
        form.changeVisibleState(true);
    }

    return form;
}

StiMobileDesigner.prototype.DashboardDrillDownParametersContainer = function (form, width, hintText) {
    var container = this.DataContainer(width, null, true, hintText);
    container.multiItems = true;
    container.style.minHeight = "31px";
    container.style.maxHeight = "125px";

    return container;
}