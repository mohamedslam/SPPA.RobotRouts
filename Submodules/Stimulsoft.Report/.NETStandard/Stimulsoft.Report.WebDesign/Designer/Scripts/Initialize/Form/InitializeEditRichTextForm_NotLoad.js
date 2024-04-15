
StiMobileDesigner.prototype.InitializeEditRichTextForm_ = function () {

    //RichText Form
    var richTextForm = this.BaseFormPanel("richTextForm", this.loc.FormRichTextEditor.title, 1, this.HelpLinks["richtextform"]);
    richTextForm.dataTree = this.options.dataTree;
    richTextForm.mode = "RichTextExpression";
    var panelWidth = 680;
    var panelHeight = 500;
    var resourceIdent = this.options.resourceIdent;
    var variableIdent = this.options.variableIdent;

    //Main Table
    var mainTable = this.CreateHTMLTable();
    mainTable.className = "stiDesignerImageFormMainPanel";
    richTextForm.container.appendChild(mainTable);
    richTextForm.container.style.padding = "0px";

    //Buttons
    var buttonProps = [
        ["RichTextExpression", "RichTextForm.RichTextExpression.png", this.loc.PropertyMain.Text],
        ["RichTextDataColumn", "RichTextForm.RichTextDataColumn.png", this.loc.PropertyMain.DataColumn],
        ["RichTextUrl", "RichTextForm.RichTextHyperlink.png", this.loc.PropertyMain.Hyperlink],
        ["RichTextFile", "RichTextForm.RichTextFile.png", this.loc.MainMenu.menuFile.replace("&", "")]
    ];

    if (this.options.serverMode) {
        buttonProps.push(["RichTextServer", "RichTextForm.RichTextCloud.png", "Server"]);
    }

    //Add Panels && Buttons
    var buttonsPanel = mainTable.addCell();
    var panelsContainer = mainTable.addCell();
    buttonsPanel.style.verticalAlign = "top";
    buttonsPanel.style.paddingTop = "6px";
    richTextForm.mainButtons = {};
    richTextForm.panels = {};

    for (var i = 0; i < buttonProps.length; i++) {
        var panel = document.createElement("Div");
        panel.className = "stiDesignerEditFormPanel";
        if (i != 0) panel.style.display = "none";
        panel.style.height = panelHeight + "px";
        panel.style.width = panelWidth + "px";
        panel.style.overflow = "hidden";
        panelsContainer.appendChild(panel);
        richTextForm.panels[buttonProps[i][0]] = panel;

        var button = this.FormTabPanelButton(null, buttonProps[i][2], buttonProps[i][1], buttonProps[i][2], null, { width: 24, height: 24 }, 34);
        richTextForm.mainButtons[buttonProps[i][0]] = button;
        buttonsPanel.appendChild(button);
        button.panelName = buttonProps[i][0];
        button.action = function () {
            richTextForm.setMode(this.panelName);
        }
    }

    //RichText Expression
    var richTextControl = this.RichTextEditor("editRichTextFormEditor", panelWidth - 28, panelHeight - 174);
    richTextForm.panels.RichTextExpression.appendChild(richTextControl);

    var richTextMainGallery = this.ImageGallery(null, panelWidth, 85, this.loc.Report.Gallery);
    richTextForm.panels.RichTextExpression.appendChild(richTextMainGallery);

    richTextMainGallery.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        richTextControl.resize(panelWidth - 28, state ? panelHeight - 174 : panelHeight - 64);
    }

    richTextMainGallery.action = function (item) {
        richTextControl.setRichTextContent(item.itemObject);
        richTextControl.variableName = item.itemObject.type == "StiVariable" ? item.itemObject.name : null;
        richTextControl.resourceName = item.itemObject.type == "StiResource" ? item.itemObject.name : null;
        richTextControl.columnName = item.itemObject.type == "StiDataColumn" ? item.itemObject.name : null;
        richTextUrlControl.value = item.itemObject.type == "StiResource"
            ? resourceIdent + item.itemObject.name
            : (item.itemObject.type == "StiVariable" ? variableIdent + item.itemObject.name : "");

        if (item.itemObject.type == "StiResource" || item.itemObject.type == "StiVariable") {
            richTextControl.onLoadResourceContent = function (content) {
                richTextUrlContainer.setRichTextContent({ content: content });
            }
        }
        else {
            richTextControl.onLoadResourceContent = function () {
                richTextUrlContainer.setRichTextContent(null);
            }
        }

        if (item.itemObject.type == "StiDataColumn") {
            var item = dataColumnGallery.getItemByPropertyValue("name", item.itemObject.name);
            if (item) item.action(true);
        }
    }

    richTextControl.onchange = function () {
        richTextControl.variableName = null;
        richTextControl.resourceName = null;
        richTextControl.columnName = null;
        if (richTextMainGallery.selectedItem) {
            richTextMainGallery.selectedItem.select(false);
        }
    }

    //RichText DataColumn
    var dataTreePlace = document.createElement("div");
    dataTreePlace.className = "stiSimpleContainerWithBorder";
    dataTreePlace.style.margin = "12px";
    dataTreePlace.style.overflow = "auto";
    dataTreePlace.style.width = (panelWidth - 28) + "px";
    dataTreePlace.style.height = (panelHeight - 136) + "px";
    var dataColumnTree = this.options.dataTree;
    richTextForm.panels.RichTextDataColumn.appendChild(dataTreePlace);

    var dataColumnGallery = this.ImageGallery(null, panelWidth, 85, this.loc.Report.Gallery);
    richTextForm.panels.RichTextDataColumn.appendChild(dataColumnGallery);

    dataColumnGallery.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        dataTreePlace.style.height = (state ? panelHeight - 136 : panelHeight - 26) + "px";
    }

    dataColumnGallery.action = function (item) {
        dataColumnTree.setKey(item.itemObject.name);
        dataColumnTree.autoscroll();
    }

    //RichText Url
    var tableRichTextUrl = this.CreateHTMLTable();
    tableRichTextUrl.style.width = "100%";
    richTextForm.panels.RichTextUrl.appendChild(tableRichTextUrl);

    var textCell = tableRichTextUrl.addTextCell(this.loc.PropertyMain.Hyperlink);
    textCell.style.paddingLeft = "12px";
    textCell.style.textAlign = "left";
    textCell.style.width = "100px";

    var richTextUrlControl = this.TextBox(null, 500);
    richTextUrlControl.style.margin = "12px 14px 12px 0";
    tableRichTextUrl.addCell(richTextUrlControl).style.textAlign = "right";

    richTextUrlControl.onkeyup = function () {
        clearTimeout(this.keyTimer);
        this.keyTimer = setTimeout(function () {
            richTextUrlControl.action();
        }, 800);
    }

    var richTextUrlContainer = this.RichTextContainer(panelWidth - 28, panelHeight - 171);
    richTextUrlContainer.style.margin = "0 12px 12px 12px";
    richTextForm.panels.RichTextUrl.appendChild(richTextUrlContainer);

    var richTextUrlContextMenu = this.InitializeDeleteItemsContextMenu("richTextUrlContextMenu");

    var richTextUrlGallery = this.ImageGallery(null, panelWidth, 85, this.loc.Report.History, richTextUrlContextMenu);
    richTextForm.panels.RichTextUrl.appendChild(richTextUrlGallery);

    richTextUrlContextMenu.action = function (item) {
        this.changeVisibleState(false);
        switch (item.key) {
            case "delete": {
                if (richTextUrlGallery.selectedItem) {
                    richTextForm.removeRichTextItemFromHistory(richTextUrlGallery.selectedItem.itemObject.src, "StimulsoftMobileDesignerRichTextUrlHistory");
                    richTextUrlGallery.selectedItem.remove();
                }
                break;
            }
            case "deleteAll": {
                richTextUrlGallery.clear();
                this.jsObject.SetCookie("StimulsoftMobileDesignerRichTextUrlHistory", JSON.stringify([]));
                break;
            }
        }
    }

    richTextUrlGallery.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        var height = state ? panelHeight - 171 : panelHeight - 61;
        if (this.jsObject.options.isTouchDevice) height -= 5;
        richTextUrlContainer.resize(panelWidth - 28, height);
    }

    richTextUrlGallery.action = function (item) {
        richTextUrlContainer.setRichTextContent(item.itemObject);
        richTextUrlControl.value = item.itemObject.url;
    }

    richTextUrlControl.action = function () {
        if (this.value.indexOf(resourceIdent) == 0 || this.value.indexOf(variableIdent) == 0) {
            var ident = this.value.indexOf(resourceIdent) == 0 ? resourceIdent : variableIdent;
            var itemName = this.value.substring(this.value.indexOf(ident) + ident.length);
            var item = richTextMainGallery.getItemByPropertyValue("name", itemName);
            if (item) {
                item.action(true);
            }
            else {
                richTextUrlContainer.setRichTextContent(null);
                richTextControl.setRichTextContent(null);
                richTextControl.variableName = null;
                richTextControl.resourceName = null;
                richTextControl.columnName = null;
                if (richTextMainGallery.selectedItem) {
                    richTextMainGallery.selectedItem.select(false);
                }
            }
        }
        else {
            richTextUrlContainer.setRichTextContent({ url: this.value });
        }
    }

    richTextUrlControl.onmouseup = function () {
        if (this.jsObject.options.itemInDrag) {
            var dictionaryTree = richTextForm.jsObject.options.dictionaryTree;
            if (dictionaryTree.selectedItem) {
                this.value += dictionaryTree.selectedItem.getResultForEditForm();
            }
        }
    }

    richTextUrlControl.ontouchend = function () { this.onmouseup(); }

    //RichText File 
    var tableFileName = this.CreateHTMLTable();
    richTextForm.panels.RichTextFile.appendChild(tableFileName);

    var textCell2 = tableFileName.addTextCell(this.loc.Cloud.labelFileName.replace(":", ""));
    textCell2.style.paddingLeft = "12px";
    textCell2.style.textAlign = "left";
    textCell2.style.width = "100px";

    var fileNameControl = this.TextBoxWithOpenDialog("richTextFormFileName", 500, ".rtf");
    if (!this.options.standaloneJsMode) fileNameControl.openButton.style.display = "none";
    fileNameControl.style.margin = "12px 14px 12px 0";
    tableFileName.addCell(fileNameControl).style.textAlign = "right";

    //Image Server
    if (this.options.serverMode) {
        richTextForm.cloudContainer = this.CloudContainer("richTextFormCloudContainer", ["RichText"], null, panelHeight);
        richTextForm.panels.RichTextServer.appendChild(richTextForm.cloudContainer);
        if (this.options.dictionaryTree.selectedItem) this.options.dictionaryTree.selectedItem.setSelected();
    }

    //Form Methods
    richTextForm.setMode = function (mode) {
        richTextForm.mode = mode;
        for (var panelName in richTextForm.panels) {
            richTextForm.panels[panelName].style.display = mode == panelName ? "" : "none";
            richTextForm.mainButtons[panelName].setSelected(mode == panelName);
        }
        var propertiesPanel = richTextForm.jsObject.options.propertiesPanel;
        propertiesPanel.editFormControl = null;
        propertiesPanel.setEnabled(mode == "RichTextUrl" || mode == "RichTextServer" || mode == "RichTextExpression");
        if (mode == "RichTextUrl") propertiesPanel.editFormControl = richTextForm.richTextUrlControl;
        if (mode == "RichTextServer") propertiesPanel.editFormControl = richTextForm.cloudContainer;
        if (mode == "RichTextExpression") propertiesPanel.editFormControl = richTextControl;
    }

    richTextForm.onhide = function () {
        richTextForm.jsObject.options.propertiesPanel.setDictionaryMode(false);
    }

    richTextForm.addRichTextItemToHistory = function (richTextUrl, cookiesKey) {
        var historyStr = this.jsObject.GetCookie(cookiesKey);
        var historyArray = historyStr ? JSON.parse(historyStr) : [];
        var newItem = {
            name: this.jsObject.GetFileNameFromPath(richTextUrl),
            url: richTextUrl,
            imageName: "Resources.BigResourceRtf"
        }
        var haveThisItem = false;
        for (var i = 0; i < historyArray.length; i++) {
            if (historyArray[i].name == newItem.name && historyArray[i].url == newItem.url) {
                haveThisItem = true;
                break;
            }
        }
        if (!haveThisItem) {
            if (historyArray.length > 9) historyArray.splice(9, 10);
            historyArray.splice(0, 0, newItem);
            this.jsObject.SetCookie(cookiesKey, JSON.stringify(historyArray));
        }
    }

    richTextForm.removeRichTextItemFromHistory = function (richTextUrl, cookiesKey) {
        var historyStr = this.jsObject.GetCookie(cookiesKey);
        var historyArray = historyStr ? JSON.parse(historyStr) : [];
        for (var i = 0; i < historyArray.length; i++) {
            if (historyArray[i].url == richTextUrl) {
                historyArray.splice(i, 1);
                break;
            }
        }
        this.jsObject.SetCookie(cookiesKey, JSON.stringify(historyArray));
    }

    richTextForm.fillRichTextAndColumnsGalleries = function (richTextGallery) {
        richTextMainGallery.progress.hide();
        dataColumnGallery.progress.hide();
        var allItems = [].concat(richTextGallery.variables, richTextGallery.resources, richTextGallery.columns);

        if (allItems.length > 0) {
            richTextMainGallery.addItems(allItems);
            if (richTextGallery.columns.length > 0) {
                dataColumnGallery.addItems(richTextGallery.columns);
            }
            else {
                dataColumnGallery.changeVisibleState(false);
            }
        }
        else {
            richTextMainGallery.changeVisibleState(false);
            dataColumnGallery.changeVisibleState(false);
        }
    }

    richTextForm.show = function () {
        richTextForm.jsObject.options.propertiesPanel.setDictionaryMode(true);
        richTextForm.changeVisibleState(true);

        //Data Tree Build
        dataTreePlace.appendChild(dataColumnTree);
        dataColumnTree.build(null, null, null, true);
        dataColumnTree.action = function () { richTextForm.action(); }

        //Update galleries
        if (richTextForm.jsObject.options.richTextGallery || richTextForm.jsObject.CheckRichTextInDictionary()) {
            richTextMainGallery.changeVisibleState(true);
            dataColumnGallery.changeVisibleState(true);
            richTextMainGallery.progress.show(280, -25);
            dataColumnGallery.progress.show(280, -25);

            if (!richTextForm.jsObject.options.richTextGallery) {
                richTextForm.jsObject.SendCommandToDesignerServer("GetRichTextGallery", null, function (answer) {
                    richTextForm.jsObject.options.richTextGallery = answer.richTextGallery;
                    richTextForm.fillRichTextAndColumnsGalleries(answer.richTextGallery);
                    var itemName = richTextControl.variableName || richTextControl.resourceName || richTextControl.columnName;
                    if (itemName) {
                        var item = richTextMainGallery.getItemByPropertyValue("name", itemName);
                        if (item) item.action(true);
                    }
                });
            }
            else {
                richTextForm.fillRichTextAndColumnsGalleries(this.jsObject.options.richTextGallery)
            }
        }
        else {
            richTextMainGallery.changeVisibleState(false);
            dataColumnGallery.changeVisibleState(false);
        }

        var richTextUrlHistory = this.jsObject.GetCookie("StimulsoftMobileDesignerRichTextUrlHistory");
        richTextUrlGallery.changeVisibleState(richTextUrlHistory && JSON.parse(richTextUrlHistory).length > 0);
        if (richTextUrlHistory) richTextUrlGallery.addItems(JSON.parse(richTextUrlHistory));

        var selectedObject = this.jsObject.options.selectedObject || this.jsObject.GetCommonObject(this.jsObject.options.selectedObjects);
        if (!selectedObject) return;

        var props = ["richText", "richTextDataColumn", "richTextUrl"];
        for (var i = 0; i < props.length; i++) {
            richTextForm[props[i]] = selectedObject.properties[props[i]] != null && selectedObject.properties[props[i]] != "StiEmptyValue"
                ? StiBase64.decode(selectedObject.properties[props[i]]) : null;
        }

        if (richTextForm.richTextDataColumn) {
            richTextForm.setMode("RichTextDataColumn");
            richTextControl.columnName = richTextForm.richTextDataColumn;
            if (richTextForm.jsObject.options.richTextGallery) {
                var item = dataColumnGallery.getItemByPropertyValue("name", richTextForm.richTextDataColumn);
                if (item) item.select(true);
            }
            dataColumnTree.setKey(richTextForm.richTextDataColumn);
            setTimeout(function () { dataColumnTree.autoscroll(); });
        }
        else if (richTextForm.richTextUrl) {
            if (richTextForm.richTextUrl.indexOf("file://") == 0) {
                richTextForm.setMode("RichTextFile");
                fileNameControl.setValue(richTextForm.richTextUrl.replace("file://", ""));
            }
            else {
                if (richTextForm.richTextUrl.indexOf(resourceIdent) == 0 || richTextForm.richTextUrl.indexOf(variableIdent) == 0) {
                    var ident = richTextForm.richTextUrl.indexOf(resourceIdent) == 0 ? resourceIdent : variableIdent;
                    richTextUrlControl.value = richTextForm.richTextUrl;
                    richTextControl.resourceName = richTextForm.richTextUrl.substring(richTextForm.richTextUrl.indexOf(ident) + ident.length);
                    richTextForm.setMode("RichTextExpression");
                    if (richTextForm.jsObject.options.richTextGallery) {
                        var item = richTextMainGallery.getItemByPropertyValue("name", richTextControl.resourceName);
                        if (item) {
                            item.action(true);
                        }
                        else {
                            richTextForm.setMode("RichTextUrl");
                        }
                    }
                }
                else {
                    richTextForm.setMode("RichTextUrl");
                    richTextUrlControl.value = richTextForm.richTextUrl;
                    richTextUrlContainer.setRichTextContent({ url: richTextForm.richTextUrl });
                }
            }
        }
        else if (richTextForm.richText && richTextForm.richText.indexOf(this.jsObject.options.cloudServerUrl) == 0) {
            richTextForm.setMode("RichTextServer");
            var key = richTextForm.richText.replace(this.jsObject.options.cloudServerUrl, "");
            var item = this.jsObject.options.dictionaryTree.getCloudItemByKey("RichText", key);
            if (item && richTextForm.cloudContainer) richTextForm.cloudContainer.addItem(item.itemObject);
        }
        else {
            richTextForm.setMode("RichTextExpression");
            if (richTextForm.richText) richTextControl.setText(richTextForm.richText, null, true);
            richTextControl.variableName = null;
            richTextControl.resourceName = null;
            richTextControl.columnName = null;
        }
    }

    richTextForm.action = function () {
        this.changeVisibleState(false);

        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        if (!selectedObjects) return;
        var propertyNames = ["richText", "richTextDataColumn", "richTextUrl"];

        for (var i = 0; i < selectedObjects.length; i++) {
            var selectedObject = selectedObjects[i];

            for (var k = 0; k < propertyNames.length; k++) {
                selectedObject.properties[propertyNames[k]] = "";
            }

            switch (richTextForm.mode) {
                case "RichTextUrl":
                    {
                        selectedObject.properties.richTextUrl = StiBase64.encode(richTextUrlControl.value);
                        if (richTextUrlControl.value && /*richTextUrlContainer.src &&*/
                            richTextUrlControl.value.indexOf(resourceIdent) != 0 &&
                            richTextUrlControl.value.indexOf(variableIdent) != 0) {
                            richTextForm.addRichTextItemToHistory(richTextUrlControl.value, "StimulsoftMobileDesignerRichTextUrlHistory");
                        }
                        break;
                    }
                case "RichTextFile":
                    {
                        selectedObject.properties.richTextUrl = StiBase64.encode("file://" + fileNameControl.getValue());
                        break;
                    }
                case "RichTextDataColumn":
                    {
                        selectedObject.properties.richTextDataColumn = StiBase64.encode(dataColumnTree.key || "");
                        break;
                    }
                case "RichTextExpression":
                    {
                        if (richTextControl.variableName != null) {
                            selectedObject.properties.richTextUrl = StiBase64.encode(variableIdent + richTextControl.variableName);
                        }
                        else if (richTextControl.resourceName != null) {
                            selectedObject.properties.richTextUrl = StiBase64.encode(resourceIdent + richTextControl.resourceName);
                        }
                        else if (richTextControl.columnName != null) {
                            selectedObject.properties.richTextDataColumn = StiBase64.encode(richTextControl.columnName);
                        }
                        else {
                            selectedObject.properties.richText = StiBase64.encode(richTextControl.getText(true, true));
                        }
                        break;
                    }
                case "RichTextServer":
                    {
                        if (richTextForm.cloudContainer && richTextForm.cloudContainer.item && this.jsObject.options.cloudServerUrl) {
                            selectedObject.properties.richText = StiBase64.encode(this.jsObject.options.cloudServerUrl + richTextForm.cloudContainer.item.itemObject.key);
                        }
                        else
                            selectedObject.properties.richText = "";
                        break;
                    }
            }
        }
        richTextForm.jsObject.SendCommandSendProperties(selectedObjects, propertyNames);
    }

    return richTextForm;
}