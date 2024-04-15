
StiMobileDesigner.prototype.InitializeInteractionForm_ = function () {
    var jsObject = this;
    var interactionForm = this.BaseFormPanel("interactionForm", this.loc.PropertyMain.Interaction, 1);
    interactionForm.controls = {};
    var mainHeight = 450;

    //Main Table
    var mainTable = this.CreateHTMLTable();
    mainTable.className = "stiDesignerImageFormMainPanel";
    interactionForm.container.appendChild(mainTable);
    interactionForm.container.style.padding = "0px";

    //Buttons
    var buttonProps = [
        ["Common", "Interaction.InteractionCommon.png", this.loc.PropertyCategory.CommonCategory],
        ["DrillDown", "Interaction.InteractionDrillDown.png", this.loc.PropertyMain.DrillDown],
        ["DrillDownParameters", "Interaction.InteractionDrillDownParameters.png", this.loc.PropertyMain.DrillDownParameters],
        ["Sort", "Interaction.InteractionSort.png", this.loc.PropertyMain.Sort]
    ];

    //Add Panels && Buttons
    var buttonsPanel = mainTable.addCell();
    var panelsContainer = mainTable.addCell();
    buttonsPanel.style.verticalAlign = "top";
    buttonsPanel.style.paddingTop = "6px";
    interactionForm.mainButtons = {};
    interactionForm.panels = {};

    for (var i = 0; i < buttonProps.length; i++) {
        var panel = document.createElement("Div");
        panel.className = "stiDesignerEditFormPanel";
        panel.style.width = "550px";
        panel.style.height = mainHeight + "px";
        if (i != 0) panel.style.display = "none";
        panelsContainer.appendChild(panel);
        interactionForm.panels[buttonProps[i][0]] = panel;
        panel.onShow = function () { };

        var button = this.FormTabPanelButton(null, buttonProps[i][2], buttonProps[i][1], buttonProps[i][2], null, { width: 24, height: 24 }, 34);
        interactionForm.mainButtons[buttonProps[i][0]] = button;
        buttonsPanel.appendChild(button);
        button.panelName = buttonProps[i][0];
        button.action = function () {
            interactionForm.showPanel(this.panelName);
        }

        //add marker
        var marker = document.createElement("div");
        marker.style.display = "none";
        marker.className = "stiUsingMarker";
        var markerInner = document.createElement("div");
        marker.appendChild(markerInner);
        button.style.position = "relative";
        button.appendChild(marker);
        button.marker = marker;
    }

    var commonTable = this.CreateHTMLTable();
    commonTable.style.margin = "6px 0 6px 0";
    commonTable.style.width = "100%";
    interactionForm.panels.Common.appendChild(commonTable);

    var commonControls = [
        ["bookmark", this.loc.PropertyMain.Bookmark, this.ExpressionTextArea(null, 400, 34, true)],
        ["tooltip", this.loc.PropertyMain.ToolTip, this.ExpressionTextArea(null, 400, 34, true)],
        ["tag", this.loc.PropertyMain.Tag, this.ExpressionTextArea(null, 400, 34, true)],
        ["hyperlink", this.loc.PropertyMain.Hyperlink, this.ExpressionTextArea(null, 400, 34, true)],
        ["hyperlinkType", " ", this.DropDownList(null, 230, null, this.GetHyperlinkTypeItems(), true, false, null, true)],
        ["bandsHeader", null, this.FormBlockHeader(this.loc.Report.Bands)],
        ["selectionEnabled", " ", this.CheckBox(null, this.loc.PropertyMain.SelectionEnabled)],
        ["collapsingEnabled", " ", this.CheckBox(null, this.loc.PropertyMain.CollapsingEnabled)],
        ["collapseGroupFooter", " ", this.CheckBox(null, this.loc.PropertyMain.CollapseGroupFooter)],
        ["collapsedExpression", this.loc.PropertyMain.Expression, this.ExpressionTextArea(null, 400, 34, true)],
        ["crossHeader", null, this.FormBlockHeader(this.loc.Components.StiCrossHeaderBand)],
        ["crossHeaderCollapsingEnabled", " ", this.CheckBox(null, this.loc.PropertyMain.CollapsingEnabled)]
    ]

    for (var i = 0; i < commonControls.length; i++) {
        interactionForm.controls[commonControls[i][0] + "Row"] = commonTable.addRow();

        if (commonControls[i][1]) {
            var text = commonTable.addTextCellInLastRow(commonControls[i][1]);
            text.className = "stiDesignerCaptionControls";
            text.style.padding = "0 0 0 12px";
            text.style.width = "100%";
        }

        var control = commonControls[i][2];
        interactionForm.controls[commonControls[i][0]] = control;
        control.style.margin = commonControls[i][0] == "bandsHeader" ? "6px 0 6px 0" : "6px 12px 6px 12px";

        var controlCell = commonTable.addCellInLastRow(control);
        if (!commonControls[i][1]) controlCell.setAttribute("colspan", "2");
    }

    //DrillDown
    var drillDownEnabled = this.CheckBox(null, this.loc.PropertyMain.DrillDownEnabled);
    drillDownEnabled.style.margin = "12px 0 12px 15px";
    interactionForm.panels.DrillDown.appendChild(drillDownEnabled);
    interactionForm.panels.DrillDown.appendChild(this.FormBlockHeader(this.loc.PropertyMain.DrillDownPage));

    var pageContainer = this.EasyContainer(null, 120);
    pageContainer.className = "stiSimpleContainerWithBorder";
    pageContainer.style.overflow = "auto";
    pageContainer.style.margin = "12px";

    interactionForm.panels.DrillDown.appendChild(pageContainer);

    var drillRepHeader = this.FormBlockHeader(this.loc.PropertyMain.DrillDownReport);
    drillRepHeader.style.marginBottom = "6px";
    interactionForm.panels.DrillDown.appendChild(drillRepHeader);

    var drillDownReportTable = this.CreateHTMLTable();
    drillDownReportTable.addTextCell(this.loc.Cloud.labelFileName.replace(":", "")).className = "stiDesignerDrillDownFormCaptionControls";

    var drillDownFileName = this.TextBox(null, 250);
    drillDownReportTable.addCell(drillDownFileName).style.padding = "6px 12px 6px 12px";
    interactionForm.panels.DrillDown.appendChild(drillDownReportTable);

    var drillDownModeTable = this.CreateHTMLTable();
    drillDownModeTable.addTextCell(this.loc.PropertyMain.DrillDownMode).className = "stiDesignerDrillDownFormCaptionControls";

    var drillDownMode = this.DropDownList(null, 180, null, this.GetDrillDownModeItems(), true, false, null, true);
    drillDownModeTable.addCell(drillDownMode).style.padding = "6px 12px 6px 12px";
    interactionForm.panels.DrillDown.appendChild(drillDownModeTable);

    if (this.options.isJava) {
        drillDownModeTable.style.display = 'none';
    }

    var resourceIdent = this.options.resourceIdent;
    var reportGallery = this.ImageGallery(null, 300, 100, this.loc.Report.Gallery);
    reportGallery.style.marginTop = "6px";
    interactionForm.panels.DrillDown.appendChild(reportGallery);

    reportGallery.update = function () {
        reportGallery.clear();
        reportGallery.style.display = "none";

        var items = [];
        if (jsObject.options.report) {
            var resources = jsObject.options.report.dictionary.resources;
            for (var i = 0; i < resources.length; i++) {
                if (resources[i].type == "Report" || resources[i].type == "ReportSnapshot") {
                    items.push({
                        name: resources[i].name,
                        type: resources[i].type,
                        imageName: "Resources.BigResourceReport"
                    });
                }
            }
        }

        if (items.length > 0) {
            reportGallery.style.display = "";
            reportGallery.addItems(items);
        }

        if (drillDownFileName.value && drillDownFileName.value.indexOf(resourceIdent) >= 0) {
            var item = reportGallery.getItemByPropertyValue("name", drillDownFileName.value.replace(resourceIdent, ""));
            if (item) item.action(true);
        }
    }

    reportGallery.action = function (item) {
        drillDownFileName.value = resourceIdent + item.itemObject.name;
    }

    //DrillDown Parameters
    var newParameter = this.FormButton(null, null, this.loc.FormDictionaryDesigner.DataParameterNew);
    newParameter.style.margin = "12px 12px 0 12px";
    newParameter.style.display = "inline-block";
    interactionForm.panels.DrillDownParameters.appendChild(newParameter);

    var parametersTable = this.CreateHTMLTable();
    interactionForm.panels.DrillDownParameters.appendChild(parametersTable);

    var paramsCont = this.DataContainer(160, mainHeight - 70, false, " ");
    paramsCont.style.margin = "12px 12px 0 12px";
    paramsCont.className = "stiSimpleContainerWithBorder";
    paramsCont.style.overflow = "auto";
    parametersTable.addCell(paramsCont);

    paramsCont.onAction = function (actionName) {
        if (actionName == "select" && this.selectedItem && this.selectedItem.itemObject) {
            drillParamName.value = this.selectedItem.itemObject.name;
            drillParamValue.value = StiBase64.decode(this.selectedItem.itemObject.expression);
        }

        var countItems = paramsCont.getCountItems();
        drillParamName.setEnabled(countItems > 0);
        drillParamValue.setEnabled(countItems > 0);

        if (countItems == 0) {
            drillParamName.value = "";
            drillParamValue.value = "";
        }
    }

    paramsCont.updateHintText = function () { }

    paramsCont.updateItem = function () {
        if (this.selectedItem && this.selectedItem.itemObject) {
            this.selectedItem.itemObject.name = drillParamName.value;
            this.selectedItem.itemObject.expression = StiBase64.encode(drillParamValue.value);
            this.selectedItem.captionContainer.innerHTML = drillParamName.value;
        }
    }

    paramsCont.onmouseover = function () { }
    paramsCont.onmouseout = function () { }

    newParameter.action = function () {
        var countItems = paramsCont.getCountItems();
        if (countItems < 10) {
            var index = 1;
            var resultName = jsObject.loc.PropertyMain.Name;
            var nameIsFree = false;
            while (!nameIsFree) {
                var nameFinded = false;
                for (var i = 0; i < countItems; i++) {
                    var item = paramsCont.getItemByIndex(i);
                    if (item.itemObject.name.toLowerCase() == resultName.toLowerCase()) {
                        nameFinded = true;
                        break;
                    }
                }
                if (nameFinded) {
                    index++;
                    resultName = jsObject.loc.PropertyMain.Name + index;
                }
                else {
                    nameIsFree = true;
                }
            }
            var itemObject = {
                name: resultName,
                expression: ""
            }
            paramsCont.addItem(itemObject.name, false, itemObject).select();
        }
    }

    var drillDownTable = this.CreateHTMLTable();
    parametersTable.addCell(drillDownTable).style.verticalAlign = "top";
    drillDownTable.addTextCell(this.loc.PropertyMain.Name).className = "stiDesignerCaptionControls";

    var drillParamName = this.TextBox(null, 230);
    drillParamName.action = function () { paramsCont.updateItem(); }
    drillDownTable.addCell(drillParamName).style.padding = "8px 0 4px 0";

    var expCell = drillDownTable.addTextCellInNextRow(this.loc.PropertyMain.Value);
    expCell.className = "stiDesignerCaptionControls";
    expCell.style.verticalAlign = "top";
    expCell.style.paddingTop = "8px";

    var drillParamValue = this.TextArea(null, 230, 100);
    drillParamValue.addInsertButton();
    drillParamValue.action = function () { paramsCont.updateItem(); }
    drillDownTable.addCellInLastRow(drillParamValue).style.padding = "4px 0 8px 0";

    drillParamValue.insertButton.action = function () {
        var dictionaryTree = this.jsObject.options.dictionaryTree;
        if (dictionaryTree && dictionaryTree.selectedItem) {
            var text = dictionaryTree.selectedItem.getResultForEditForm();
            if (text && text.indexOf("{") == 0 && this.jsObject.EndsWith(text, "}")) {
                text = text.substring(1, text.length - 1);
            }
            drillParamValue.insertText(text);
            drillParamValue.action();
        }
    }

    //Sort
    var sortingEnabled = this.CheckBox(null, this.loc.PropertyMain.SortingEnabled);
    sortingEnabled.style.margin = "12px 0 12px 15px";
    interactionForm.panels.Sort.appendChild(sortingEnabled);
    interactionForm.panels.Sort.appendChild(this.FormBlockHeader(this.loc.PropertyMain.SortingColumn));
    this.AddProgressToControl(interactionForm);

    var treeCont = document.createElement("div");
    treeCont.className = "stiSimpleContainerWithBorder";
    treeCont.style.margin = "12px 12px 0 12px";
    treeCont.style.overflow = "auto";
    treeCont.style.height = "360px";
    interactionForm.panels.Sort.appendChild(treeCont);

    //Form Methods
    interactionForm.showPanel = function (selectedPanelName) {
        this.selectedPanelName = selectedPanelName;
        for (var panelName in this.panels) {
            this.panels[panelName].style.display = selectedPanelName == panelName ? "" : "none";
            this.mainButtons[panelName].setSelected(selectedPanelName == panelName);
            if (selectedPanelName == panelName) this.panels[panelName].onShow();
        }

        var propertiesPanel = interactionForm.jsObject.options.propertiesPanel;
        propertiesPanel.editFormControl = null;
        propertiesPanel.setEnabled(selectedPanelName == "DrillDownParameters");

        if (selectedPanelName == "Sort" && !interactionForm.dataTree) {
            interactionForm.progress.show();
            setTimeout(function () {
                interactionForm.dataTree = interactionForm.jsObject.InteractionColumnsTree();
                treeCont.appendChild(interactionForm.dataTree);
                interactionForm.dataTree.build();
                interactionForm.dataTree.setKey(interactionForm.interaction != "StiEmptyValue" ? interactionForm.interaction.sortingColumn : "");
                interactionForm.progress.hide();
            }, 0);
        }
    }

    interactionForm.updateMarkers = function () {
        this.mainButtons["Common"].marker.style.display = (this.interaction.isBandInteraction && this.controls.collapsingEnabled.isChecked) ||
            (this.interaction.isCrossHeaderInteraction && this.controls.crossHeaderCollapsingEnabled.isChecked) || this.controls.selectionEnabled.isChecked || this.controls.bookmark.textArea.value ||
            this.controls.hyperlink.textArea.value || this.controls.tag.textArea.value || this.controls.tooltip.textArea.value ? "" : "none";

        var countItems = paramsCont.getCountItems();
        this.mainButtons["DrillDown"].marker.style.display = drillDownEnabled.isChecked ? "" : "none";
        this.mainButtons["DrillDownParameters"].marker.style.display = countItems > 0 ? "" : "none";
        this.mainButtons["Sort"].marker.style.display = sortingEnabled.isChecked && ((interactionForm.dataTree && interactionForm.dataTree.key) ||
            (interactionForm.interaction != "StiEmptyValue" && interactionForm.interaction.sortingColumn)) ? "" : "none";
    }

    interactionForm.show = function (interactionValue) {
        var options = this.jsObject.options;
        var selectedObject = options.selectedObject || (options.selectedObjects && options.selectedObjects.length > 0 ? options.selectedObjects[0] : null);
        if (!selectedObject) return;
        this.interaction = interactionValue || selectedObject.properties.interaction;
        var isEmptyInteraction = this.interaction == "StiEmptyValue";

        this.jsObject.options.propertiesPanel.setDictionaryMode(true);
        this.changeVisibleState(true);
        this.showPanel("Common");

        this.controls.bandsHeaderRow.style.display = "none";
        this.controls.selectionEnabledRow.style.display = "none";
        this.controls.collapsingEnabledRow.style.display = "none";
        this.controls.collapseGroupFooterRow.style.display = "none";
        this.controls.collapsedExpressionRow.style.display = "none";
        this.controls.crossHeaderRow.style.display = "none";
        this.controls.crossHeaderCollapsingEnabledRow.style.display = "none";

        //Fill DrillDown Pages
        var items = this.jsObject.GetSubReportItems(true);
        for (var i = 0; i < items.length; i++) {
            var item = pageContainer.addItem(items[i].key, items[i], items[i].caption);
            if (i == 0) item.select();
        }

        //Fill DrillDown Parameters
        paramsCont.clear();

        for (var i = 1; i <= 10; i++) {
            var itemObject = {
                name: this.interaction["drillDownParameter" + i + "Name"] || "",
                expression: this.interaction["drillDownParameter" + i + "Expression"] || ""
            }
            if (itemObject.name || itemObject.expression) {
                var item = paramsCont.addItem(itemObject.name, false, itemObject);
                if (i == 1) item.action();
            }
            else break;
        }

        if (!isEmptyInteraction && this.interaction.isBandInteraction) {
            this.controls.bandsHeaderRow.style.display = "";
            this.controls.selectionEnabledRow.style.display = "";
            this.controls.collapsingEnabledRow.style.display = "";
            this.controls.collapseGroupFooterRow.style.display = "";
            this.controls.collapsedExpressionRow.style.display = "";
            this.controls.collapsingEnabled.setChecked(this.interaction.collapsingEnabled);
            this.controls.collapseGroupFooter.setChecked(this.interaction.collapseGroupFooter);
            this.controls.collapsedExpression.textArea.value = StiBase64.decode(this.interaction.collapsedValue);
            this.controls.selectionEnabled.setChecked(this.interaction.selectionEnabled);
        }

        if (!isEmptyInteraction && this.interaction.isCrossHeaderInteraction) {
            this.controls.crossHeaderRow.style.display = "";
            this.controls.crossHeaderCollapsingEnabledRow.style.display = "";
            this.controls.crossHeaderCollapsingEnabled.setChecked(this.interaction.crossHeaderCollapsingEnabled);
        }

        drillDownEnabled.setChecked(!isEmptyInteraction ? this.interaction.drillDownEnabled : false);
        drillDownFileName.value = !isEmptyInteraction ? this.interaction.drillDownReport : "";
        drillDownMode.setKey(!isEmptyInteraction ? this.interaction.drillDownMode : "MultiPage");
        if (!isEmptyInteraction) {
            var pageItem = pageContainer.getItemByName(this.interaction.drillDownPage);
            if (pageItem)
                pageItem.select();
            else if (pageContainer.childNodes.length > 0)
                pageContainer.childNodes[0].select();
        }

        sortingEnabled.setChecked(!isEmptyInteraction ? this.interaction.sortingEnabled : false);
        this.controls.bookmark.textArea.value = !isEmptyInteraction ? StiBase64.decode(this.interaction.bookmark) : "";
        this.controls.hyperlinkType.setKey(!isEmptyInteraction ? this.interaction.hyperlinkType : "HyperlinkExternalDocuments");
        this.controls.hyperlink.textArea.value = !isEmptyInteraction ? StiBase64.decode(this.interaction.hyperlink) : "";
        this.controls.tag.textArea.value = !isEmptyInteraction ? StiBase64.decode(this.interaction.tag) : "";
        this.controls.tooltip.textArea.value = !isEmptyInteraction ? StiBase64.decode(this.interaction.toolTip) : "";

        reportGallery.update();

        this.updateMarkers();
        this.markerTimer = setInterval(function () {
            interactionForm.updateMarkers();
        }, 250)
    }

    interactionForm.onhide = function () {
        this.jsObject.options.propertiesPanel.setDictionaryMode(false);
        clearTimeout(this.markerTimer);
    }

    interactionForm.applyValues = function () {
        if (this.interaction.isBandInteraction) {
            this.interaction.collapsingEnabled = this.controls.collapsingEnabled.isChecked;
            this.interaction.collapseGroupFooter = this.controls.collapseGroupFooter.isChecked;
            this.interaction.collapsedValue = StiBase64.encode(this.controls.collapsedExpression.textArea.value);
            this.interaction.selectionEnabled = this.controls.selectionEnabled.isChecked;
        }

        if (this.interaction.isCrossHeaderInteraction) {
            this.interaction.crossHeaderCollapsingEnabled = this.controls.crossHeaderCollapsingEnabled.isChecked;
        }

        this.interaction.drillDownPage = pageContainer.selectedItem && pageContainer.selectedItem.itemObject.name != "NotAssigned" ? pageContainer.selectedItem.itemObject.name : "";
        this.interaction.drillDownEnabled = drillDownEnabled.isChecked;
        this.interaction.drillDownReport = drillDownFileName.value;
        this.interaction.drillDownMode = drillDownMode.key;

        var countItems = paramsCont.getCountItems();
        for (var i = 0; i < 10; i++) {
            var name = "";
            var expression = "";
            if (i < countItems) {
                var itemObject = paramsCont.getItemByIndex(i).itemObject;
                name = itemObject.name;
                expression = itemObject.expression;
            }
            this.interaction["drillDownParameter" + (i + 1) + "Name"] = name;
            this.interaction["drillDownParameter" + (i + 1) + "Expression"] = expression;
        }

        this.interaction.sortingEnabled = sortingEnabled.isChecked;
        if (this.dataTree && this.dataTree.selectedItem) {
            interactionForm.interaction.sortingColumn = this.dataTree.getResult();
        }

        this.interaction.bookmark = StiBase64.encode(this.controls.bookmark.textArea.value);
        this.interaction.hyperlinkType = this.controls.hyperlinkType.key;
        this.interaction.hyperlink = StiBase64.encode(this.controls.hyperlink.textArea.value);
        this.interaction.tag = StiBase64.encode(this.controls.tag.textArea.value);
        this.interaction.toolTip = StiBase64.encode(this.controls.tooltip.textArea.value);
    }

    interactionForm.action = function () {
        this.applyValues();
        this.changeVisibleState(false);
        this.jsObject.ApplyPropertyValue("interaction", this.interaction);
    }

    return interactionForm;
}

StiMobileDesigner.prototype.InteractionColumnsTree = function () {
    var tree = this.DataTree();
    tree.style.padding = "0px";

    tree.build = function () {
        this.clear();

        this.mainItem = this.jsObject.HiddenMainTreeItem(this, true);
        this.appendChild(this.mainItem);

        var dictionary = this.jsObject.options.report.dictionary;
        if (dictionary == null) return;

        var dataBands = this.jsObject.GetDataBandsForInteractionSort();
        if (dataBands) {
            for (var i = 0; i < dataBands.length; i++) {
                var dataSource = this.jsObject.GetDataSourceByNameFromDictionary(dataBands[i].dataSourceName);
                if (dataSource) {
                    var copyDataSource = this.jsObject.CopyObject(dataSource);
                    copyDataSource.name = copyDataSource.correctName = copyDataSource.alias = dataBands[i].componentName;
                    this.addTreeItems([copyDataSource], this.mainItem);
                }
            }
        }
    }

    tree.getResult = function () {
        if (!this.selectedItem || this.selectedItem.itemObject.typeItem == "NoItem") return "";

        var currItem = this.selectedItem;
        var fullName = "";
        while (currItem.parent != null) {
            if (fullName != "") fullName = "." + fullName;
            var name = currItem.itemObject.typeItem == "Relation" ? currItem.itemObject.nameInSource : (currItem.itemObject.correctName || currItem.itemObject.name);
            fullName = name + fullName;
            currItem = currItem.parent;
        }
        return fullName;
    }

    return tree;
}