
StiMobileDesigner.prototype.InitializeCreateDataForm_ = function () {
    var form = this.BaseForm("createDataForm", this.loc.PropertyMain.Data, 1);
    form.controls = {};

    //Tabs
    var tabs = [];
    tabs.push({ "name": "Data", "caption": this.loc.PropertyCategory.DataCategory });
    tabs.push({ "name": "Styles", "caption": this.loc.PropertyMain.Styles });

    var tabbedPane = this.TabbedPane("createDataFormTabbedPane", tabs, "stiDesignerStandartTab");
    tabbedPane.style.margin = "12px";
    form.tabbedPane = tabbedPane;
    form.container.appendChild(tabbedPane);

    for (var i = 0; i < tabs.length; i++) {
        var tabsPanel = tabbedPane.tabsPanels[tabs[i].name];
        tabsPanel.style.width = "630px";
        tabsPanel.style.height = "460px";

        switch (tabs[i].name) {
            case "Data": this.InitializeCreateDataFormDataPanel(form, tabsPanel); break;
            case "Styles": this.InitializeCreateDataFormStylesPanel(form, tabsPanel); break;
        }
    }

    form.show = function (dataSource, point, pageName) {
        form.changeVisibleState(true);
        tabbedPane.showTabPanel("Data");
        form.dataSource = dataSource;
        form.point = point;
        form.pageName = pageName;
        form.controls.columnsContainer.clear();
        form.controls.columnsTree.build(dataSource);
        form.controls.themesContainer.update();
    }

    form.action = function () {
        var params = {
            dataSource: form.dataSource,
            point: form.point,
            pageName: form.pageName,
            columns: [],
            settings: {
                data: form.controls.data.isChecked,
                table: form.controls.table.isChecked,
                header: form.controls.header.isChecked,
                footer: form.controls.footer.isChecked
            },
            theme: form.selectedThemeButton ? form.selectedThemeButton.key : null
        };

        for (var i = 0; i < form.controls.columnsContainer.childNodes.length; i++) {
            params.columns.push(form.controls.columnsContainer.childNodes[i].itemObject);
        }

        form.changeVisibleState(false);
        this.jsObject.SendCommandCreateDataComponent(params);
    }

    return form;
}

StiMobileDesigner.prototype.InitializeCreateDataFormDataPanel = function (form, parentPanel) {
    //Main Table
    var mainTable = this.CreateHTMLTable();
    parentPanel.appendChild(mainTable);

    //Columns Tree
    var treeCont = document.createElement("div");
    treeCont.className = "stiSimpleContainerWithBorder";
    treeCont.style.margin = "6px 12px 0 0";
    treeCont.style.height = "355px";
    mainTable.addCell(treeCont);

    var columnsTree = this.Tree(290, 305);
    columnsTree.style.overflow = "auto";
    form.controls.columnsTree = columnsTree;
    treeCont.appendChild(columnsTree);

    columnsTree.itemFromObject = function (itemObject) {
        var captionText = this.jsObject.options.dictionaryTree ? this.jsObject.options.dictionaryTree.getItemCaption(itemObject) : itemObject.name;
        var item = this.jsObject.TreeItem(captionText, itemObject.typeIcon + ".png", itemObject, columnsTree, true);

        item.getFullName = function (useNameInSourcesInRelation) {
            var currItem = this;
            var fullName = "";
            while (currItem.parent != null) {
                if (fullName != "") fullName = "." + fullName;
                if (currItem.itemObject.typeItem == "Relation" && useNameInSourcesInRelation && currItem.itemObject.nameInSource) {
                    fullName = currItem.itemObject.nameInSource + fullName;
                }
                else {
                    fullName = (currItem.itemObject.correctName || currItem.itemObject.name) + fullName;
                }
                currItem = currItem.parent;
            }
            return (currItem.itemObject.typeItem == "BusinessObject" ? currItem.itemObject.fullName : (currItem.itemObject.correctName || currItem.itemObject.name)) + "." + fullName;
        }

        return item;
    }

    columnsTree.build = function (dataSource) {
        columnsTree.clear();
        columnsTree.rootItem = this.itemFromObject(dataSource);
        columnsTree.rootItem.style.margin = "8px";
        columnsTree.rootItem.setOpening(true);
        columnsTree.appendChild(columnsTree.rootItem);
        columnsTree.addTreeItems(dataSource.relations, columnsTree.rootItem);
        columnsTree.addTreeItems(dataSource.columns, columnsTree.rootItem);
    }

    columnsTree.addTreeItems = function (collection, parentItem) {
        if (collection) {
            for (var i = 0; i < collection.length; i++) {
                var childItem = parentItem.addChild(columnsTree.itemFromObject(collection[i]));
                if (collection[i].typeItem == "Relation") {
                    this.addTreeItems(collection[i].relations, childItem);

                    var dataSource = this.jsObject.GetDataSourceByNameFromDictionary(collection[i].parentDataSource);
                    if (dataSource && dataSource.columns) {
                        this.addTreeItems(dataSource.columns, childItem);
                    }
                }
            }
        }
    }

    //Columns Container
    var rightPanel = document.createElement("div");
    rightPanel.className = "stiSimpleContainerWithBorder";
    rightPanel.style.margin = "6px 12px 0 0";
    rightPanel.style.height = "355px";
    mainTable.addCell(rightPanel);

    var columnsTable = this.CreateHTMLTable();
    var columnsContainer = form.controls.columnsContainer = this.EasyContainer(290, 340);
    columnsTable.addCell(columnsContainer);
    rightPanel.appendChild(columnsTable);

    columnsTree.onChecked = function (item) {
        var checkItem = function (parentItem) {
            for (var i = 0; i < parentItem.childsContainer.childNodes.length; i++) {
                var item = parentItem.childsContainer.childNodes[i];
                if (item.childsContainer && item.childsContainer.childNodes.length > 0) {
                    checkItem(item);
                }
                if (item.itemObject && item.itemObject.typeItem == "Column") {
                    var fullName = item.getFullName();
                    if (item.isChecked) {
                        if (!columnsContainer.isContained(fullName)) {
                            var newItem = columnsContainer.addItem(fullName, item.itemObject, fullName, item.itemObject.typeIcon + ".png");
                            newItem.itemObject.fullName = fullName;
                            if (item.itemObject.type == "byte[]" || item.itemObject.type == "image") {
                                newItem.itemObject.imageColumnFullName = item.getFullName(true);
                            }
                        }
                    }
                    else {
                        columnsContainer.removeItem(fullName);
                    }
                }
            }
        }
        checkItem(columnsTree.rootItem);
    };

    //Container Buttons Up & Down
    var containerButtons = document.createElement("div");
    columnsTable.addCell(containerButtons).style.verticalAlign = "top";

    var buttonUp = this.SmallButton("createDataFormButtonUp", null, null, "Arrows.ArrowUpBlue.png", null, null, "stiDesignerFormButton", true);
    buttonUp.style.margin = "4px";
    buttonUp.setEnabled(false);
    containerButtons.appendChild(buttonUp);

    buttonUp.action = function () {
        if (columnsContainer.selectedItem) { columnsContainer.selectedItem.move("Up"); }
    }

    var buttonDown = this.SmallButton("createDataFormButtonUp", null, null, "Arrows.ArrowDownBlue.png", null, null, "stiDesignerFormButton", true);
    buttonDown.style.margin = "0px 4px 4px 4px";
    buttonDown.setEnabled(false);
    containerButtons.appendChild(buttonDown);

    buttonDown.action = function () {
        if (columnsContainer.selectedItem) { columnsContainer.selectedItem.move("Down"); }
    }

    columnsContainer.onAction = function () {
        var count = columnsContainer.getCountItems();
        var index = columnsContainer.selectedItem ? columnsContainer.selectedItem.getIndex() : -1;
        buttonUp.setEnabled(index > 0);
        buttonDown.setEnabled(index != -1 && index < count - 1);
    }

    // Mark All & Reset
    var buttonsPanel = document.createElement("div");
    var buttonsTable = this.CreateHTMLTable();
    buttonsPanel.style.textAlign = "right";
    buttonsTable.style.display = "inline-block";
    buttonsPanel.appendChild(buttonsTable);
    treeCont.appendChild(buttonsPanel);

    var markAllButton = this.FormButton(null, "createDataFormMarkAll", this.loc.Wizards.MarkAll.replace("&", ""), null);
    var resetButton = this.FormButton(null, "createDataFormReset", this.loc.Wizards.Reset.replace("&", ""), null);

    buttonsTable.addCell(markAllButton).style.padding = "12px 12px 0 12px";
    buttonsTable.addCell(resetButton).style.padding = "12px 12px 0 0";

    markAllButton.action = function () {
        columnsTree.rootItem.setChecked(true);
        columnsTree.rootItem.checkBox.action();
    }

    resetButton.action = function () {
        columnsTree.rootItem.setChecked(false);
        columnsTree.rootItem.checkBox.action();
    }

    //Additional Controls
    var controlsTable = this.CreateHTMLTable();
    parentPanel.appendChild(controlsTable);
    var controlProps = [
        ["data", this.RadioButton("createDataFormDataRadioButton", "createDataForm", this.loc.PropertyMain.Data)],
        ["header", this.CheckBox("createDataFormHeaderCheckBox", this.loc.Components.StiHeaderBand)],
        ["table", this.RadioButton("createDataFormTableRadioButton", "createDataForm", this.loc.Components.StiTable)],
        ["footer", this.CheckBox("createDataFormFooterCheckBox", this.loc.Components.StiFooterBand)]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        if (i == 0 || i == 2) controlsTable.addRow();
        var control = controlProps[i][1];
        form.controls[controlProps[i][0]] = control;
        controlsTable.addCellInLastRow(control).style.padding = i <= 1 ? "15px 60px 5px 10px" : "5px 60px 10px 10px";
        if (controlProps[i][0] == "data") control.setChecked(true);
    }
}

StiMobileDesigner.prototype.InitializeCreateDataFormStylesPanel = function (form, parentPanel) {
    var jsObject = this;
    parentPanel.appendChild(this.FormBlockHeader(this.loc.Wizards.Themes));
    var themesContainer = this.ThemesContainer();
    themesContainer.style.height = "150px";
    form.controls.themesContainer = themesContainer;
    parentPanel.appendChild(themesContainer);

    themesContainer.update = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);

        var noneButton = form.jsObject.ThemeButton(form, form.jsObject.loc.PropertyEnum.StiCheckStyleNone, null, null);
        themesContainer.appendChild(noneButton);
        noneButton.action();

        if (form.jsObject.options.report.stylesCollection) {
            var stylesCollection = form.jsObject.options.report.stylesCollection;
            var collectionNames = {};
            var baseColors = {};
            var collections = [];

            for (var i = 0; i < stylesCollection.length; i++) {
                var stylesProps = stylesCollection[i].properties;
                var collectionName = stylesProps.collectionName;
                var styleName = stylesProps.name;

                if (collectionName != "") {
                    if (jsObject.EndsWith(styleName, "_Header1")) {
                        baseColors[collectionName] = jsObject.GetColorFromBrushStr(stylesProps.brush);
                    }
                    if (!collectionNames[collectionName]) {
                        collections.push(collectionName);
                        collectionNames[collectionName] = true;
                    }
                }
            }

            for (var i = 0; i < collections.length; i++) {
                var collectionButton = form.jsObject.ThemeButton(form, collections[i], baseColors[collections[i]], { type: "User", name: collections[i] });
                themesContainer.appendChild(collectionButton);
            }
        }
    }

    parentPanel.appendChild(this.FormBlockHeader(this.loc.Wizards.DefaultThemes));
    var defThemesContainer = this.ThemesContainer();
    defThemesContainer.style.height = "260px";
    parentPanel.appendChild(defThemesContainer);

    var defaultStyles = [
        { color: "144,215,207", name: this.loc.Chart.Style + "21" },
        { color: "191,113,127", name: this.loc.Chart.Style + "24" },
        { color: "80,101,161", name: this.loc.Chart.Style + "25" },
        { color: "255,136,94", name: this.loc.Chart.Style + "26" },
        { color: "35,171,223", name: this.loc.Chart.Style + "27" }
    ];

    for (var i = 0; i < defaultStyles.length; i++) {
        var button = this.ThemeButton(form, defaultStyles[i].name, defaultStyles[i].color, { type: "Default", name: defaultStyles[i].name, color: defaultStyles[i].color });
        defThemesContainer.appendChild(button);
    }
}

StiMobileDesigner.prototype.ThemesContainer = function () {
    var container = document.createElement("div");
    container.className = "styleDesignerItemsContainer";
    container.style.width = "635px";    

    return container;
}

StiMobileDesigner.prototype.ThemeButton = function (form, caption, baseColor, key) {
    var button = this.StandartBigButton(null, null, caption, " ", caption);
    button.key = key;

    button.style.float = "left";
    button.style.display = "inline-block";
    button.style.margin = "12px 0 0 12px";

    //override
    button.innerTable.style.margin = "10px";
    button.innerTable.style.width = "auto";
    button.cellImage.removeChild(button.image);
    button.cellImage.style.width = "50px";
    button.cellImage.style.height = "30px";

    if (baseColor) {
        var sampleTable = this.SampleTable(50, 30, 7, 5, baseColor, baseColor, this.getLightColor(baseColor, 100));
        sampleTable.style.display = "inline-table";
        button.cellImage.appendChild(sampleTable);
    }
    else {
        button.cellImage.style.background = "white";
        button.cellImage.style.border = "1px solid #d3d3d3";
    }

    button.caption.style.padding = "6px 3px 2px";

    button.action = function () {
        if (form.selectedThemeButton != null) {
            form.selectedThemeButton.setSelected(false);
        }
        this.setSelected(true);
        form.selectedThemeButton = this;
    }

    return button;
}