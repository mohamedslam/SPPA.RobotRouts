//DropDown List
StiMobileDesigner.prototype.PropertyDropDownList = function (name, width, items, readOnly, showImage, toolTip, cutMenu) {
    var jsObject = this;

    if (jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.localizePropertyGrid && items) {
        for (var i = 0; i < items.length; i++) {
            items[i].captionText = items[i].caption;
            items[i].caption = items[i].key;
        }
    }

    var propertyDropDownList = this.DropDownList(name, width, toolTip, items, readOnly, showImage, this.options.propertyControlsHeight, cutMenu != null ? cutMenu : true);

    if (propertyDropDownList.menu.innerContent)
        propertyDropDownList.menu.innerContent.style.maxHeight = "230px";

    return propertyDropDownList;
}

//TextBox
StiMobileDesigner.prototype.PropertyTextBox = function (name, width) {
    return this.TextBox(name, width, this.options.propertyControlsHeight);
}

//TextBox With EditButton
StiMobileDesigner.prototype.PropertyTextBoxWithEditButton = function (name, width, readOnly, showClearButton, height, showSimpleEditor) {
    return this.TextBoxWithEditButton(name, width, height || this.options.propertyControlsHeight, readOnly, showClearButton, showSimpleEditor);
}

//Data Control With EditButton
StiMobileDesigner.prototype.PropertyDataControl = function (name, width, useFullNames, showClearButton, height) {
    var dataControl = this.PropertyTextBoxWithEditButton(name, width, true, showClearButton, height);

    dataControl.button.action = function () {
        this.key = this.textBox.value;
        var this_ = this;

        this.jsObject.InitializeDataColumnForm(function (dataColumnForm) {
            dataColumnForm.parentButton = this_;
            dataColumnForm.changeVisibleState(true);

            dataColumnForm.action = function () {
                this.changeVisibleState(false);
                if (useFullNames) {
                    var selectedItem = this.dataTree.selectedItem;
                    var itemObject = selectedItem ? selectedItem.itemObject : null;
                    this.dataTree.key = !itemObject || (itemObject.typeItem == "NoItem" || itemObject.typeItem != this.dataTree.selectingItemType) ? "" : selectedItem.getFullName(true);
                }
                this.parentButton.textBox.value = this.dataTree.key;
                if (dataControl["action"] != null) dataControl.action();
            }
        });
    }

    return dataControl;
}

//Expression Control
StiMobileDesigner.prototype.PropertyExpressionControl = function (name, width, readOnly, cutBrackets, notShowDictionary) {
    var expressionControl = this.ExpressionControl(name, width, this.options.propertyControlsHeight, readOnly != null ? readOnly : true, cutBrackets, false, notShowDictionary);

    return expressionControl;
}

//Font List
StiMobileDesigner.prototype.PropertyFontList = function (name, width, cutMenu) {
    var propertyFontList = this.FontList(name, width, this.options.propertyControlsHeight, cutMenu);
    if (propertyFontList.menu.innerContent) propertyFontList.menu.innerContent.style.maxHeight = "230px";
    return propertyFontList;
}

//PropertyDataDropDownList
StiMobileDesigner.prototype.PropertyDataDropDownList = function (name, width, toolTip) {
    var propertyDataDropDownList = this.PropertyDropDownList(name, width, null, true, false, toolTip);

    //Override
    propertyDataDropDownList.addItems = function (items) {
        this.items = [];
        this.items.push(this.jsObject.Item("NotAssigned", this.jsObject.loc.Report.NotAssigned, null, "[Not Assigned]"));
        if (!items) return;
        for (var i = 0; i < items.length; i++) {
            this.items.push(items[i]);
        }
        this.menu.addItems(this.items);
    }

    propertyDataDropDownList.reset = function () {
        this.setKey("[Not Assigned]");
    }

    return propertyDataDropDownList;
}

//PropertyBusinessObject
StiMobileDesigner.prototype.PropertyBusinessObject = function (name) {
    var control = this.PropertyTextBoxWithEditButton(name, this.options.propertyControlWidth, true);

    control.button.action = function () {
        var this_ = this;

        this.jsObject.InitializeDataBusinessObjectForm(function (dataBusinessObjectForm) {
            dataBusinessObjectForm.dataTree.build("BusinessObject");
            this_.key = control.textBox.value;
            dataBusinessObjectForm.parentButton = this_;
            dataBusinessObjectForm.changeVisibleState(true);

            dataBusinessObjectForm.action = function () {
                dataBusinessObjectForm.changeVisibleState(false);
                dataBusinessObjectForm.parentButton.textBox.value = this.dataTree.key != ""
                    ? this.dataTree.key
                    : dataBusinessObjectForm.jsObject.loc.Report.NotAssigned;
                control.action();
            }
        });
    }

    control.action = function () { };

    return control;
}

//Property Filter
StiMobileDesigner.prototype.PropertyFilter = function (name) {
    var control = this.PropertyTextBoxWithEditButton(name, this.options.propertyControlWidth, true);

    control.button.action = function () {
        this.jsObject.InitializeFilterForm(function (filterForm) {
            filterForm.resultControl = control;
            filterForm.resultControl.formResult = control.key;

            if (control.dataSourceControl) {
                control.currentDataSourceName = control.dataSourceControl.key;
                filterForm.changeVisibleState(true);
            }
        });
    }

    control.setKey = function (key) {
        if (key == null) return;
        control.key = key;
        control.textBox.value = this.jsObject.FilterDataToShortString(JSON.parse(StiBase64.decode(key.filterData)));
    }

    return control;
}

//Property Sort
StiMobileDesigner.prototype.PropertySort = function (name) {
    var control = this.PropertyTextBoxWithEditButton(name, this.options.propertyControlWidth, true);

    control.button.action = function () {
        this.jsObject.InitializeSortForm(function (sortForm) {
            sortForm.resultControl = control;
            sortForm.resultControl.formResult = control.key;

            if (control.dataSourceControl) {
                control.currentDataSourceName = control.dataSourceControl.key;
                sortForm.changeVisibleState(true);
            }
        });
    }

    control.setKey = function (key) {
        if (key == null) return;
        control.key = key;
        control.textBox.value = this.jsObject.SortDataToShortString(key);
    }

    return control;
}

//Property Series Filter
StiMobileDesigner.prototype.PropertyChartSeriesFilter = function (name) {
    var jsObject = this;
    var control = this.PropertyTextBoxWithEditButton(name, this.options.propertyControlWidth, true);

    control.button.action = function () {
        jsObject.InitializeChartSeriesFilterForm(function (filterForm) {
            filterForm.resultControl = control;
            filterForm.changeVisibleState(true);
        });
    }

    control.setKey = function (key) {
        control.key = key;
        control.textBox.value = "[" + ((!key || key.length == 0) ? jsObject.loc.FormBand.NoFilters : jsObject.loc.PropertyMain.Filters) + "]";
    }

    return control;
}

//Property Series Conditions
StiMobileDesigner.prototype.PropertyChartSeriesConditions = function (name) {
    var jsObject = this;
    var control = this.PropertyTextBoxWithEditButton(name, this.options.propertyControlWidth, true);

    control.button.action = function () {
        jsObject.InitializeChartSeriesConditionsForm(function (conditionsForm) {
            conditionsForm.resultControl = control;
            conditionsForm.changeVisibleState(true);
        });
    }

    control.setKey = function (key) {
        control.key = key;
        control.textBox.value = "[" + ((!key || key.length == 0) ? jsObject.loc.FormConditions.NoConditions : jsObject.loc.FormConditions.title) + "]";
    }

    return control;
}

//Property Series TrendLines
StiMobileDesigner.prototype.PropertyChartSeriesTrendLines = function (name) {
    var control = this.PropertyTextBoxWithEditButton(name, this.options.propertyControlWidth, true);
    var jsObject = this;

    control.button.action = function () {
        jsObject.InitializeChartSeriesTrendLinesForm(function (form) {
            form.resultControl = control;
            form.changeVisibleState(true);
        });
    }

    control.setKey = function (key) {
        this.key = key;
        this.textBox.value = "[" + ((key == null || key.length == 0) ? jsObject.loc.Report.NotAssigned : jsObject.loc.PropertyMain.TrendLines) + "]";
    }

    return control;
}

//Property Series TrendLines
StiMobileDesigner.prototype.PropertyDashboardWatermark = function (name) {
    var control = this.PropertyTextBoxWithEditButton(name, this.options.propertyControlWidth, true);
    var jsObject = this;

    control.button.action = function () {
        jsObject.InitializeDashboardWatermarkForm(function (form) {
            form.show();
        });
    }

    control.setKey = function (key) {
        this.key = key;
        this.textBox.value = "[" + (key && (key.textEnabled || key.imageEnabled || key.weaveEnabled) ? jsObject.loc.PropertyMain.Watermark : jsObject.loc.Report.NotAssigned) + "]";
    }

    return control;
}

//Property Sort
StiMobileDesigner.prototype.PropertyBlockWithButton = function (name, imageName, caption, buttonWidth) {
    var propertyBlock = this.CreateHTMLTable();
    propertyBlock.name = name;
    if (name) this.options.controls[name] = propertyBlock;
    propertyBlock.style.width = "100%";

    if (imageName) {
        var image = propertyBlock.image = document.createElement("img");
        image.style.width = image.style.height = "32px";
        image.style.margin = "4px 0px 4px 6px";
        image.setAttribute("draggable", "false");

        if (StiMobileDesigner.checkImageSource(this.options, imageName)) {
            StiMobileDesigner.setImageSource(image, this.options, imageName);
        }        

        var imageCell = propertyBlock.addCell(image);
        imageCell.style.width = "1px";
        imageCell.style.fontSize = "0";
        imageCell.style.lineHeight = "0";
    }

    var button = this.SmallButton(null, null, caption, null, null, null, "stiDesignerFormButton");
    propertyBlock.button = button;
    button.style.width = (buttonWidth || (this.options.propertyControlWidth + 128)) + "px";
    button.style.margin = "4px 0px 4px 8px";
    button.innerTable.style.width = "100%";
    button.caption.style.textAlign = "center";
    propertyBlock.addCell(button);

    return propertyBlock;
}

//ColorsCollection
StiMobileDesigner.prototype.PropertyColorsCollectionControl = function (name, toolTip, width) {
    var colorsControl = this.ColorsCollectionControl(name, toolTip, width, this.options.propertyControlsHeight);

    return colorsControl;
}

StiMobileDesigner.prototype.PropertyCollectionColorsComplicatedControl = function (name, toolTip, width, predefinedColors) {
    var colorsControl = this.CollectionColorsComplicatedControl(name, toolTip, width, this.options.propertyControlsHeight, predefinedColors);

    return colorsControl;
}

StiMobileDesigner.prototype.PropertyDataTransformationControl = function (name, width) {
    var jsObject = this;
    var control = this.PropertyTextBoxWithEditButton(name, width, true);
    control.textBox.value = "[" + this.loc.PropertyMain.DataTransformation + "]";

    control.button.action = function () {
        jsObject.InitializeEditElementDataTransformationForm(function (form) {
            form.changeVisibleState(true);
        });
    }

    return control;
}

//Blockly Expression Control
StiMobileDesigner.prototype.PropertyEventExpressionControl = function (name, width, height) {
    var expControl = this.TextBoxWithEditButton(name, width, height || this.options.propertyControlsHeight, true);
    expControl.cutBrackets = true;
    expControl.textBox.readOnly = true;
    expControl.textBox.style.cursor = "default";
    expControl.textBox.mainControl = expControl;

    var jsObject = this;

    expControl.textBox.onclick = function () {
        if (!this.isTouchEndFlag)
            this.mainControl.button.onclick();
    }

    expControl.textBox.ontouchend = function () {
        var this_ = this;
        this.isTouchEndFlag = true;
        clearTimeout(this.isTouchEndTimer);
        this.mainControl.button.ontouchend();
        this.isTouchEndTimer = setTimeout(function () {
            this_.isTouchEndFlag = false;
        }, 1000);
    }

    expControl.button.action = function () {
        var eventValue = expControl.textBox.hiddenValue;
        var showBlocklyForm = function () {
            jsObject.InitializeBlocklyEditorForm(function (form) {
                form.resultControl = expControl;
                form.show(eventValue, expControl.eventName);
            });
        }
        var showExpressionEventForm = function () {
            jsObject.InitializeEventEditorForm(function (form) {
                var propertiesPanel = jsObject.options.propertiesPanel;
                form.propertiesPanelZIndex = propertiesPanel.style.zIndex;
                form.propertiesPanelIsEnabled = propertiesPanel.isEnabled;
                form.resultControl = expControl;
                form.changeVisibleState(true);
            });
        }
        if (((eventValue == "" && jsObject.options.defaultScriptMode == "Blocks") || jsObject.isBlocklyValue(eventValue)) && !jsObject.options.blocklyNotSupported) {
            if (jsObject.options.jsMode) {
                showBlocklyForm();
            }
            else {
                if (!jsObject.options.blocklyInitialized) {
                    jsObject.LoadScriptWithProcessImage(jsObject.options.scriptsUrl + "BlocklyScripts;" + (jsObject.options.cultureName || "en"), function () {
                        jsObject.options.blocklyInitialized = true;
                        showBlocklyForm();
                    });
                }
                else {
                    showBlocklyForm();
                }
            }
        }
        else {
            showExpressionEventForm();
        }
    }

    return expControl;
}