
StiMobileDesigner.prototype.InitializeSubReportForm_ = function () {

    //SubReport Form
    var subReportForm = this.BaseFormPanel("subReportForm", this.loc.Components.StiSubReport, 1, this.HelpLinks["subreportform"]);
    subReportForm.mode = "SubReportPage";

    //Main Table
    var mainTable = this.CreateHTMLTable();
    mainTable.className = "stiDesignerImageFormMainPanel";
    subReportForm.container.appendChild(mainTable);
    subReportForm.container.style.padding = "0px";

    //Buttons
    var buttonProps = [
        ["SubReportPage", "SubReportForm.SubReportPage.png", this.loc.Toolbars.TabPage],
        ["SubReportFile", "SubReportForm.SubReportFile.png", this.loc.MainMenu.menuFile.replace("&", "")],
        ["SubReportUrl", "SubReportForm.SubReportHyperlink.png", this.loc.PropertyMain.Hyperlink],
        ["SubReportParameters", "SubReportForm.SubReportParameters.png", this.loc.PropertyMain.Parameters]
    ];

    if (this.options.serverMode) {
        buttonProps.push(["SubReportServer", "SubReportForm.SubReportCloud.png", "Server"]);
    }

    //Add Panels && Buttons
    var buttonsPanel = mainTable.addCell();
    var panelsContainer = mainTable.addCell();
    buttonsPanel.style.verticalAlign = "top";
    buttonsPanel.style.paddingTop = "6px";
    subReportForm.mainButtons = {};
    subReportForm.panels = {};

    for (var i = 0; i < buttonProps.length; i++) {
        var panel = document.createElement("Div");
        panel.className = "stiDesignerEditFormPanel";
        if (i != 0) panel.style.display = "none";
        panelsContainer.appendChild(panel);
        subReportForm.panels[buttonProps[i][0]] = panel;

        var button = this.FormTabPanelButton(null, buttonProps[i][2], buttonProps[i][1], buttonProps[i][2], null, { width: 24, height: 24 }, 34);
        subReportForm.mainButtons[buttonProps[i][0]] = button;
        buttonsPanel.appendChild(button);

        if (this.options.isJava && i > 2) {
            button.style.display = 'none';
        }

        button.panelName = buttonProps[i][0];
        button.action = function () {
            subReportForm.setMode(this.panelName);
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

    //Page
    var pageContainer = this.Container("pageContainer", 419, 422);
    pageContainer.className = "stiSimpleContainerWithBorder";
    pageContainer.style.margin = "12px 0 0 12px";
    pageContainer.style.overflow = "auto";
    subReportForm.panels.SubReportPage.appendChild(pageContainer);

    //File Name
    var toolBarFileName = this.CreateHTMLTable();
    toolBarFileName.style.width = "100%";
    subReportForm.panels.SubReportFile.appendChild(toolBarFileName);

    var captionCell = toolBarFileName.addCell();
    captionCell.style.padding = "0 12px 0 15px";
    captionCell.innerHTML = this.loc.Cloud.labelFileName.replace(":", "");

    var fileNameTextBox = subReportForm.fileNameTextBox = this.TextBoxWithOpenDialog("subReportFormFileName", 300, ".mrt");
    if (!this.options.standaloneJsMode) fileNameTextBox.openButton.style.display = "none";
    fileNameTextBox.style.margin = "12px 14px 12px 0";
    toolBarFileName.addCell(fileNameTextBox).style.textAlign = "right";

    //Url
    var urlTextArea = subReportForm.urlTextArea = this.TextArea("subReportFormUrl", 419, 422);
    urlTextArea.style.margin = "12px 0 0 12px";
    subReportForm.panels.SubReportUrl.appendChild(urlTextArea);
    subReportForm.panels.SubReportUrl.style.overflow = "hidden";

    //Resources Gallery
    var resourcesGallery = this.ImageGallery(null, 450, 100, this.loc.PropertyMain.Resources);
    resourcesGallery.style.marginTop = "12px";
    subReportForm.panels.SubReportUrl.appendChild(resourcesGallery);

    resourcesGallery.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        urlTextArea.style.height = urlTextArea.style.minHeight = urlTextArea.style.maxHeight = (state ? 315 : 422) + "px";
    }

    resourcesGallery.update = function () {
        this.clear();
        if (!this.jsObject.options.report) return;

        var types = ["Report", "ReportSnapshot"];
        var resources = this.jsObject.options.report.dictionary.resources;
        var resourceIdent = this.jsObject.options.resourceIdent;
        var urlResourceName = urlTextArea.value.indexOf(resourceIdent) == 0 ? urlTextArea.value.substring(resourceIdent.length) : null;

        for (var i = 0; i < resources.length; i++) {
            if (types.indexOf(resources[i].type) >= 0) {
                var itemObject = this.jsObject.CopyObject(resources[i]);
                itemObject.imageName = "Resources.BigResource" + resources[i].type;
                var item = this.addItem(itemObject);

                item.action = function () {
                    resourcesGallery.action(this);
                }

                item.select = function (state) {
                    if (state) {
                        var items = resourcesGallery.getItems();
                        for (var i = 0; i < items.length; i++) {
                            items[i].setSelected(false);
                        }
                        this.setSelected(true);
                    }
                    else {
                        this.setSelected(false);
                    }
                }

                if (itemObject.name == urlResourceName) {
                    item.select(true);
                }
            }
        };

        resourcesGallery.changeVisibleState(this.innerContainer.innerTable);
    }

    resourcesGallery.action = function (item) {
        item.select(true);
        urlTextArea.value = this.jsObject.options.resourceIdent + item.itemObject.name;
    }

    //Parameters
    var parametersContainer = this.SubReportFormParametersContainer(subReportForm);
    subReportForm.panels.SubReportParameters.appendChild(parametersContainer);

    //Events
    urlTextArea.onmouseup = function () {
        if (this.jsObject.options.itemInDrag) {
            var dictionaryTree = this.jsObject.options.dictionaryTree;
            if (dictionaryTree.selectedItem) { this.value += dictionaryTree.selectedItem.getResultForEditForm(); }
        }
    }
    urlTextArea.ontouchend = function () { this.onmouseup(); }

    //Server
    if (this.options.serverMode) {
        subReportForm.cloudContainer = this.CloudContainer("subReportFormCloudContainer", ["SubReport"], null, 450);
        subReportForm.panels.SubReportServer.appendChild(subReportForm.cloudContainer);
        if (this.options.dictionaryTree.selectedItem) this.options.dictionaryTree.selectedItem.setSelected();
    }

    //Form Methods
    subReportForm.setMode = function (mode) {
        subReportForm.mode = mode;
        for (var panelName in subReportForm.panels) {
            subReportForm.panels[panelName].style.display = mode == panelName ? "" : "none";
            subReportForm.mainButtons[panelName].setSelected(mode == panelName);
        }
        var propertiesPanel = subReportForm.jsObject.options.propertiesPanel;
        propertiesPanel.editFormControl = null;
        propertiesPanel.setEnabled(mode == "SubReportUrl" || mode == "SubReportServer");
        if (mode == "SubReportUrl") {
            propertiesPanel.editFormControl = urlTextArea;
            urlTextArea.focus();
            resourcesGallery.update();
        }
        if (mode == "SubReportFile") {
            fileNameTextBox.textBox.focus();
        }
        if (mode == "SubReportServer") {
            propertiesPanel.editFormControl = subReportForm.cloudContainer;
        }
    }

    subReportForm.onhide = function () {
        subReportForm.jsObject.options.propertiesPanel.setDictionaryMode(false);
        clearTimeout(this.markerTimer);
    }

    subReportForm.updateMarkers = function () {
        this.mainButtons["SubReportPage"].marker.style.display = pageContainer.selectedItem && pageContainer.selectedItem.itemObject.name != "NotAssigned" ? "" : "none";
        this.mainButtons["SubReportFile"].marker.style.display = fileNameTextBox.textBox.value ? "" : "none";
        this.mainButtons["SubReportUrl"].marker.style.display = urlTextArea.value ? "" : "none";
        this.mainButtons["SubReportParameters"].marker.style.display = parametersContainer.getItems().length > 0 ? "" : "none";
    }

    subReportForm.show = function () {
        var selectedObject = this.jsObject.options.selectedObject || this.jsObject.GetCommonObject(this.jsObject.options.selectedObjects);
        if (!selectedObject) return;

        subReportForm.jsObject.options.propertiesPanel.setDictionaryMode(true);
        subReportForm.changeVisibleState(true);
        subReportForm.setMode("SubReportPage");
        urlTextArea.value = "";
        fileNameTextBox.textBox.value = "";
        resourcesGallery.changeVisibleState(false);

        var subReportPage = selectedObject.properties["subReportPage"] != "StiEmptyValue" ? selectedObject.properties["subReportPage"] : "[Not Assigned]";
        var subReportUrl = selectedObject.properties["subReportUrl"] != null && selectedObject.properties["subReportUrl"] != "StiEmptyValue"
            ? StiBase64.decode(selectedObject.properties["subReportUrl"]) : null;

        //Get subreport pages
        pageContainer.clear();
        var items = this.jsObject.GetSubReportItems();
        for (var i = 0; i < items.length; i++) {
            var item = pageContainer.addItemAndNotAction(null, items[i], items[i].caption);
            if (i == 0) item.selected();
        }

        if (subReportUrl) {
            if (subReportUrl.indexOf("file://") == 0) {
                subReportForm.setMode("SubReportFile");
                fileNameTextBox.textBox.value = subReportUrl.replace("file://", "");
            }
            else if (subReportUrl.indexOf(this.jsObject.options.cloudServerUrl) == 0) {
                subReportForm.setMode("SubReportServer");
                var key = subReportUrl.replace(this.jsObject.options.cloudServerUrl, "");
                var item = this.jsObject.options.dictionaryTree.getCloudItemByKey("SubReport", key);
                if (item && subReportForm.cloudContainer) subReportForm.cloudContainer.addItem(item.itemObject);
            }
            else {
                urlTextArea.value = subReportUrl;
                subReportForm.setMode("SubReportUrl");
            }
        }
        else if (subReportPage != "[Not Assigned]") {
            for (var i = 0; i < pageContainer.items.length; i++) {
                if (pageContainer.items[i].itemObject.name == subReportPage) {
                    pageContainer.items[i].selected();
                    break;
                }
            }
        }

        parametersContainer.fill(selectedObject.properties["subReportParameters"]);

        this.updateMarkers();
        this.markerTimer = setInterval(function () {
            subReportForm.updateMarkers();
        }, 250);
    }

    subReportForm.action = function () {
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        if (!selectedObjects) return;
        for (var i = 0; i < selectedObjects.length; i++) {
            var selectedObject = selectedObjects[i];

            switch (subReportForm.mode) {
                case "SubReportUrl":
                    {
                        selectedObject.properties.subReportUrl = StiBase64.encode(urlTextArea.value);
                        selectedObject.properties.subReportPage = "[Not Assigned]";
                        break;
                    }
                case "SubReportFile":
                    {
                        selectedObject.properties.subReportUrl = StiBase64.encode("file://" + fileNameTextBox.textBox.value);
                        selectedObject.properties.subReportPage = "[Not Assigned]";
                        break;
                    }
                case "SubReportPage":
                    {
                        selectedObject.properties.subReportPage = pageContainer.selectedItem ? pageContainer.selectedItem.itemObject.name : "[Not Assigned]";
                        selectedObject.properties.subReportUrl = "";
                        break;
                    }
                case "SubReportServer":
                    {
                        if (subReportForm.cloudContainer && subReportForm.cloudContainer.item && this.jsObject.options.cloudServerUrl)
                            selectedObject.properties.subReportUrl = StiBase64.encode(this.jsObject.options.cloudServerUrl + subReportForm.cloudContainer.item.itemObject.key);
                        break;
                    }
            }
        }
        selectedObject.properties["subReportParameters"] = parametersContainer.getItems();

        subReportForm.jsObject.SendCommandSendProperties(selectedObjects, ["subReportPage", "subReportUrl", "subReportParameters"]);
        this.changeVisibleState(false);
    }

    return subReportForm;
}

StiMobileDesigner.prototype.SubReportFormParametersContainer = function (subReportForm) {
    var container = document.createElement("div");

    //Toolbar
    var buttons = [
        ["add", this.loc.Buttons.Add, null],
        ["remove", null, "Remove.png"],
        ["separator"],
        ["moveUp", null, "Arrows.ArrowUpBlue.png"],
        ["moveDown", null, "Arrows.ArrowDownBlue.png"]
    ]

    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "12px 0 0 12px";
    toolBar.buttons = {};
    container.appendChild(toolBar);

    for (var i = 0; i < buttons.length; i++) {
        if (buttons[i][0] == "separator") {
            toolBar.addCell(this.HomePanelSeparator()).style.paddingRight = "4px";
            continue;
        }
        var buttonStyle = buttons[i][0] == "add" ? "stiDesignerFormButton" : "stiDesignerStandartSmallButton";
        var button = this.SmallButton(null, null, buttons[i][1], buttons[i][2], null, null, buttonStyle, true);
        button.style.marginRight = "4px";
        toolBar.buttons[buttons[i][0]] = button;
        toolBar.addCell(button);
    }

    var table = this.CreateHTMLTable();
    container.appendChild(table);

    //Items Container
    var itemsContainer = this.EasyContainer(160, 384);
    itemsContainer.className = "stiSimpleContainerWithBorder";
    itemsContainer.style.margin = "12px 12px 0 12px";
    itemsContainer.style.overflow = "auto";
    table.addCell(itemsContainer);

    //Main Container
    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.margin = "12px 0 0 0";
    table.addCell(controlsTable).style.verticalAlign = "top";

    controlsTable.addTextCell(this.loc.PropertyMain.Name).className = "stiDesignerCaptionControls";
    var nameControl = this.TextBox(null, 150);
    controlsTable.addCell(nameControl).className = "stiDesignerControlCellsBigIntervals";

    nameControl.action = function () {
        if (itemsContainer.selectedItem) { itemsContainer.selectedItem.itemObject.name = this.value; }
    }

    controlsTable.addTextCellInNextRow(this.loc.PropertyMain.Expression).className = "stiDesignerCaptionControls";
    var expressionControl = this.ExpressionControl(null, 150, null, null, true);
    controlsTable.addCellInLastRow(expressionControl).className = "stiDesignerControlCellsBigIntervals";

    expressionControl.action = function () {
        if (itemsContainer.selectedItem) { itemsContainer.selectedItem.itemObject.expression = StiBase64.encode(this.textBox.value); }
    }

    toolBar.buttons.moveUp.action = function () {
        if (itemsContainer.selectedItem) { itemsContainer.selectedItem.move("Up"); }
    }

    toolBar.buttons.moveDown.action = function () {
        if (itemsContainer.selectedItem) { itemsContainer.selectedItem.move("Down"); }
    }

    toolBar.buttons.add.action = function () {
        var index = itemsContainer.getCountItems() + 1;
        var item = itemsContainer.addItem(this.jsObject.loc.PropertyMain.Parameter + index, { name: this.jsObject.loc.PropertyMain.Parameter + index, expression: "" });
        item.action();
    }

    toolBar.buttons.remove.action = function () {
        if (itemsContainer.selectedItem) { itemsContainer.selectedItem.remove(); }
    }

    itemsContainer.onAction = function () {
        var count = this.getCountItems();
        var index = this.selectedItem ? this.selectedItem.getIndex() : -1;
        toolBar.buttons.moveUp.setEnabled(index > 0);
        toolBar.buttons.moveDown.setEnabled(index != -1 && index < count - 1);
        toolBar.buttons.remove.setEnabled(this.selectedItem);
        nameControl.setEnabled(count > 0);
        expressionControl.setEnabled(count > 0);

        if (this.selectedItem) {
            nameControl.value = this.selectedItem.itemObject.name;
            expressionControl.textBox.value = StiBase64.decode(this.selectedItem.itemObject.expression);
        }
    }

    container.fill = function (parameters) {
        itemsContainer.clear();
        if (!parameters) return;
        for (var i = 0; i < parameters.length; i++) {
            var item = itemsContainer.addItem(parameters[i].name, { name: parameters[i].name, expression: parameters[i].expression });
            if (i == 0) item.action();
        }
    }

    container.getItems = function () {
        var parameters = [];
        for (var i = 0; i < itemsContainer.childNodes.length; i++) {
            if (itemsContainer.childNodes[i].itemObject) {
                parameters.push(itemsContainer.childNodes[i].itemObject);
            }
        }

        return parameters;
    }

    return container;
}