
StiMobileDesigner.prototype.InitializeGlobalizationEditorForm_ = function () {
    var jsObject = this;
    var form = this.BaseFormPanel("globalizationEditorForm", this.loc.FormGlobalizationEditor.title, 1, this.HelpLinks["globalizationEditor"]);
    form.hideButtonsPanel();

    //MainTable
    var mainTable = this.CreateHTMLTable();
    form.container.appendChild(mainTable);
    mainTable.style.borderCollapse = "separate";

    //Toolbar
    var controls = [
        ["addCulture", this.FormButton(null, null, this.loc.FormGlobalizationEditor.AddCulture.replace("&", ""), null, null, null, null, null, "Down")],
        ["editCulture", this.StandartSmallButton(null, null, null, "EditButton.png")],
        ["removeCulture", this.StandartSmallButton(null, null, null, "Remove.png")],
        ["getCultureSettings", this.StandartSmallButton(null, null, null, "CultureEditor.GetCulture.png")],
        ["setCultureSettings", this.StandartSmallButton(null, null, null, "CultureEditor.SetCulture.png")],
        ["autoLocalize", this.CheckBox(null, this.loc.FormGlobalizationEditor.AutoLocalizeReportOnRun)]
    ]

    var toolBar = this.CreateHTMLTable();
    toolBar.style.margin = "0 0 0 8px";
    mainTable.addCell(toolBar).setAttribute("colspan", "3");

    for (var i = 0; i < controls.length; i++) {
        var control = controls[i][1];
        control.style.margin = "12px 4px 12px 4px";
        toolBar[controls[i][0]] = control;
        toolBar.addCell(control);
    }

    //Cultures Container
    var cultHeader = this.FormBlockHeader(this.loc.PropertyMain.Culture);
    cultHeader.style.margin = "0 12px 12px 12px";
    var cultCell = mainTable.addCellInNextRow(cultHeader);
    cultCell.style.verticalAlign = "top";

    var cultCont = this.EasyContainer(180, 225);
    cultCont.className = "stiSimpleContainerWithBorder";
    cultCont.style.margin = "0 12px 12px 12px";
    cultCell.appendChild(cultCont);

    cultCont.addItem_ = cultCont.addItem;

    cultCont.addItem = function (name, itemObject, caption, image, notAction) {
        var item = cultCont.addItem_(name, itemObject, caption, image, notAction);
        item.style.overflow = "hidden";
        item.setAttribute("title", caption);
                
        item.ondblclick = function () {
            toolBar.editCulture.action();
        }

        return item;
    }

    //Components Container
    var compHeader = this.FormBlockHeader(this.loc.Report.Components);
    compHeader.style.margin = "0 12px 12px 12px";
    cultCell.appendChild(compHeader);

    var compCont = this.EasyContainer(180, 225);
    compCont.className = "stiSimpleContainerWithBorder";
    compCont.style.margin = "0 12px 12px 12px";
    cultCell.appendChild(compCont);

    compCont.addItem_ = compCont.addItem;

    compCont.addItem = function (name, itemObject, caption, image, notAction) {
        var item = compCont.addItem_(name, itemObject, caption, image, notAction);
        item.style.overflow = "hidden";
        item.setAttribute("title", caption);
        
        return item;
    }

    //Properties Container
    var propHeader = this.FormBlockHeader(this.loc.Panels.Properties);
    propHeader.style.margin = "0 12px 12px 0";
    var propCell = mainTable.addCellInLastRow(propHeader);
    propCell.style.verticalAlign = "top";

    var propCont = this.EasyContainer(250, 500);
    propCont.className = "stiSimpleContainerWithBorder";
    propCont.style.margin = "0 12px 12px 0";
    propCell.appendChild(propCont);

    //Editor
    var editHeader = this.FormBlockHeader(this.loc.FormViewer.Editor);
    editHeader.style.margin = "0 12px 12px 0";
    var editCell = mainTable.addCellInLastRow(editHeader);
    editCell.style.verticalAlign = "top";

    var textArea = this.TextArea(null, 300, 500);
    textArea.style.margin = "0 12px 12px 0";
    textArea.addInsertButton();
    textArea.insertButton.style.display = "none";
    editCell.appendChild(textArea);

    //Add Culture
    var addCultureMenu = this.VerticalMenu("addCultureMenu", toolBar.addCulture, "Down");

    addCultureMenu.action = function (item) {
        addCultureMenu.changeVisibleState(false);

        jsObject.SendCommandAddGlobalizationStrings(item.key, function (answer) {
            if (answer.globalizationStrings) {
                var caption = form.getCultureDisplayName(answer.globalizationStrings.cultureName);
                var item = cultCont.addItem(null, answer.globalizationStrings, caption, null, true);
                item.action();
            }
        });
    }

    //Add Culture
    toolBar.addCulture.action = function () {
        var startItems = [];
        var otherItems = [];
        var startCultures = ["en-US", "en-GB", "zh-CN", "nb-NO", "el-GR", "fr-FR", "de-DE", "it-IT", "ko-KR", "pt-BR", "pt-PT", "es-ES", "ja-JP", "ar-SA"];

        for (var i = 0; i < form.cultures.length; i++) {
            var index = startCultures.indexOf(form.cultures[i].name);

            if (index >= 0) {
                startItems[index] = jsObject.Item(form.cultures[i].name, form.cultures[i].displayName, null, form.cultures[i].name);
            }
            else {
                otherItems.push(jsObject.Item(form.cultures[i].name, form.cultures[i].displayName, null, form.cultures[i].name));
            }
        }

        for (var i = 0; i < startItems.length; i++) {
            if (!startItems[i]) startItems.splice(i, 1);
        }

        startItems.push("separator");
        startItems = startItems.concat(otherItems);

        addCultureMenu.addItems(startItems);
        addCultureMenu.changeVisibleState(true);
    }

    //Edit Culture
    toolBar.editCulture.action = function () {
        if (cultCont.selectedItem) {
            var renameForm = jsObject.options.forms.renameCultureForm || jsObject.InitializeRenameCultureForm();
            var currCulture = cultCont.selectedItem.itemObject;

            renameForm.show(currCulture.cultureName);

            renameForm.action = function () {
                renameForm.changeVisibleState(false);

                jsObject.SendCommandToDesignerServer("EditGlobalizationStrings", { newName: renameForm.nameTextBox.value, index: cultCont.selectedItem.getIndex() }, function (answer) {
                    if (answer.success) {
                        cultCont.selectedItem.caption.innerText = currCulture.cultureName = renameForm.nameTextBox.value;
                    }
                });
            }
        }
    }

    //Remove Culture
    toolBar.removeCulture.action = function () {
        if (cultCont.selectedItem) {
            jsObject.SendCommandRemoveGlobalizationStrings(cultCont.selectedItem.getIndex(), function (answer) {
                if (answer.success) {
                    cultCont.selectedItem.remove();
                }
            });
        }
    }

    //Get Culture
    toolBar.getCultureSettings.action = function () {
        jsObject.GetCultureSettingsFromReport(cultCont.selectedItem.getIndex(), function (answer) {
            if (cultCont.selectedItem && answer.globalizationStrings) {
                cultCont.selectedItem.itemObject = answer.globalizationStrings;
                cultCont.selectedItem.action();
            }
        });
    }

    //Set Culture
    toolBar.setCultureSettings.action = function () {
        if (cultCont.selectedItem) {
            jsObject.SetCultureSettingsToReport(cultCont.selectedItem.itemObject.cultureName, function (answer) {
                for (var i = 0; i < cultCont.childNodes.length; i++) {
                    cultCont.childNodes[i].itemObject.reportItems = answer.reportItems;
                }
                cultCont.onAction();

                if (answer["reportGuid"] && answer["reportObject"]) {
                    jsObject.options.reportGuid = answer.reportGuid;
                    jsObject.LoadReport(jsObject.ParseReport(answer.reportObject), true);
                    jsObject.options.reportIsModified = true;
                    jsObject.options.buttons.undoButton.setEnabled(answer.enabledUndoButton);
                    jsObject.options.buttons.redoButton.setEnabled(true);
                }
                jsObject.BackToSelectedComponent(answer.selectedObjectName);
            });
        }
    }

    //Auto Localize
    toolBar.autoLocalize.action = function () {
        jsObject.options.report.properties.autoLocalizeReportOnRun = this.isChecked;
        jsObject.SendCommandSetReportProperties(["autoLocalizeReportOnRun"]);
    }

    cultCont.isReportProperty = function (propName) {
        return (propName == "ReportAlias" || propName == "ReportAuthor" || propName == "ReportDescription");
    }

    cultCont.getComponents = function () {
        var compNames = {};
        var compItems = [];
        var hasReport = false;

        if (this.selectedItem) {
            for (var itemName in this.selectedItem.itemObject.reportItems) {
                var compName = itemName.split(".")[0];
                if (this.isReportProperty(compName)) {
                    hasReport = true;
                }
                else {
                    compNames[compName] = compName;
                }
            }
            for (var compName in compNames) {
                compItems.push(compName);
                compItems.sort();
            }
            if (hasReport) {
                compItems = ["Report"].concat(compItems);
            }
        }

        return compItems;
    }

    cultCont.onAction = function (isEmptyAction) {
        if (isEmptyAction) return;

        propCont.clear();
        compCont.clear();
        textArea.value = "";

        if (this.selectedItem) {
            var compItems = this.getComponents();
            for (var i = 0; i < compItems.length; i++) {
                var item = compCont.addItem(null, {}, compItems[i], null, true);
                item.compName = compItems[i];
            }

            var reportItems = [];
            for (var itemName in this.selectedItem.itemObject.reportItems) {
                reportItems.push(itemName);
                reportItems.sort();
            }

            for (var i = 0; i < reportItems.length; i++) {
                var item = propCont.addItem(null, this.selectedItem.itemObject, reportItems[i], null, true);
                item.propertyName = reportItems[i];

                item.markIsChanged = function () {
                    if (this.caption) {
                        this.caption.style.fontWeight = "bold";
                        this.caption.style.color = "#000000";
                    }
                }

                if (this.selectedItem.itemObject.items[item.propertyName] != this.selectedItem.itemObject.reportItems[item.propertyName]) {
                    item.markIsChanged();
                }
            }

            if (compCont.childNodes.length > 0) {
                compCont.childNodes[0].action();
            }
        }

        toolBar.removeCulture.setEnabled(this.selectedItem);
        toolBar.editCulture.setEnabled(this.selectedItem);
        toolBar.getCultureSettings.setEnabled(this.selectedItem);
        toolBar.setCultureSettings.setEnabled(this.selectedItem);
    }

    propCont.update = function () {
        var compName = compCont.selectedItem ? compCont.selectedItem.compName : null;
        var firstItem = null;
        for (var i = 0; i < this.childNodes.length; i++) {
            var item = this.childNodes[i];
            var propFirst = item.propertyName.split(".")[0];
            var showItem = compName && (propFirst == compName || (compName == "Report" && cultCont.isReportProperty(propFirst)));
            item.style.display = showItem ? "" : "none";
            if (showItem && !firstItem) firstItem = item;
        }
        if (firstItem) firstItem.action();
    }

    propCont.onAction = function () {
        if (this.selectedItem) {
            var propertyName = this.selectedItem.propertyName;
            var text = this.selectedItem.itemObject.items[propertyName];
            if (text == null) text = this.selectedItem.itemObject.reportItems[propertyName];
            textArea.value = text;
            textArea.focus();
        }
    }

    compCont.onAction = function (isEmptyAction) {
        if (!isEmptyAction) propCont.update();
    }

    textArea.action = function () {
        if (cultCont.selectedItem && propCont.selectedItem) {
            var oldValue = cultCont.selectedItem.itemObject.items[propCont.selectedItem.propertyName];
            cultCont.selectedItem.itemObject.items[propCont.selectedItem.propertyName] = this.value;
            if (oldValue != this.value) {
                propCont.selectedItem.markIsChanged();
                jsObject.SendCommandApplyGlobalizationStrings(cultCont.selectedItem.getIndex(), propCont.selectedItem.propertyName, this.value);
            }
        }
    }

    form.getCultureDisplayName = function (cultureName) {
        if (form.cultures) {
            for (var i = 0; i < form.cultures.length; i++) {
                if (form.cultures[i].name == cultureName)
                    return form.cultures[i].displayName;
            }
        }
        return cultureName;
    }

    form.fill = function (globalizationStrings) {
        cultCont.clear();
        compCont.clear();
        propCont.clear();

        for (var i = 0; i < globalizationStrings.length; i++) {
            var caption = form.getCultureDisplayName(globalizationStrings[i].cultureName);
            cultCont.addItem(null, globalizationStrings[i], caption, null, true);
        }

        if (cultCont.childNodes.length > 0) {
            cultCont.childNodes[0].action();
        }
    }

    var propPanel = this.options.propertiesPanel;

    form.show = function (globProperties) {
        form.cultures = globProperties.cultures;
        form.fill(globProperties.globalizationStrings);

        propPanel.setDictionaryMode(true);
        propPanel.setEnabled(true);
        propPanel.editFormControl = textArea;

        toolBar.autoLocalize.setChecked(jsObject.options.report.properties.autoLocalizeReportOnRun);

        this.changeVisibleState(true);
        textArea.focus();
    }

    form.onhide = function () {
        propPanel.setDictionaryMode(false);
        propPanel.editFormControl = null;
        jsObject.SendCommandRemoveUnlocalizedGlobalizationStrings();
    }

    form.action = function () {
        this.changeVisibleState(false);
    }

    return form;
}

StiMobileDesigner.prototype.InitializeRenameCultureForm = function () {
    var form = this.BaseForm("renameCultureForm", this.loc.FormGlobalizationEditor.title, 3);

    var innerTable = this.CreateHTMLTable();
    innerTable.style.margin = "5px";
    form.container.appendChild(innerTable);

    innerTable.addTextCell(this.loc.PropertyMain.Name).className = "stiDesignerCaptionControlsBigIntervals";

    var nameTextBox = form.nameTextBox = this.TextBox(null, 300);
    innerTable.addCell(nameTextBox).className = "stiDesignerControlCellsBigIntervals";

    form.show = function (cultureName) {
        this.changeVisibleState(true);
        nameTextBox.value = cultureName || "";
        nameTextBox.focus();
    }

    return form;
}