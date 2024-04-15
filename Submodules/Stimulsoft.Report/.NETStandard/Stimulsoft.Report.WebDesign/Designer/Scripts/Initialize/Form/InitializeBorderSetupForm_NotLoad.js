
StiMobileDesigner.prototype.InitializeBorderSetupForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("borderSetup", this.loc.PropertyMain.Borders, 3, this.HelpLinks["borderform"]);
    form.controls = {};
    form.advControls = {};

    //Tabs
    var tabs = [];
    tabs.push({ "name": "Simple", "caption": this.loc.PropertyMain.Simple });
    tabs.push({ "name": "Advanced", "caption": this.loc.PropertyMain.Advanced });

    var tabbedPane = this.TabbedPane("borderFormTabbedPane", tabs, "stiDesignerStandartTab");
    tabbedPane.style.margin = "12px 12px 0 12px";

    form.tabbedPane = tabbedPane;
    form.container.appendChild(tabbedPane);

    for (var i = 0; i < tabs.length; i++) {
        var tabsPanel = tabbedPane.tabsPanels[tabs[i].name];
        var innerPanel;
        switch (tabs[i].name) {
            case "Simple": innerPanel = this.InitializeBorderFormSimplePanel(form); break;
            case "Advanced": innerPanel = this.InitializeBorderFormAdvancedPanel(form); break;
        }
        tabsPanel.appendChild(innerPanel);
    }

    form.onshow = function () {
        var currentObject = jsObject.options.selectedObject || jsObject.GetCommonObject(jsObject.options.selectedObjects);
        if (!currentObject) return;

        this.border = null;
        if (this.showFunction) this.showFunction();
        var borderArray = this.border ? this.border.split("!") : currentObject.properties["border"].split("!");
        var borderSides = borderArray[0].split(",");

        tabbedPane.showTabPanel("Simple");

        form.controls.borderLeftButton.setSelected(borderSides[0] == "1");
        form.controls.borderTopButton.setSelected(borderSides[1] == "1");
        form.controls.borderRightButton.setSelected(borderSides[2] == "1");
        form.controls.borderBottomButton.setSelected(borderSides[3] == "1");

        var styleItem = form.controls.stylesContainer.getItemByName(borderArray[3]);
        var selectedItem = styleItem || form.controls.stylesContainer.childNodes[0];
        selectedItem.select();

        form.controls.borderColor.setKey(borderArray[2] == "StiEmptyValue" ? "0,0,0" : borderArray[2]);
        form.controls.borderSize.setKey(borderArray[1] == "StiEmptyValue" ? "1" : borderArray[1]);

        form.controls.topmostCell.style.display = form.controls.shadowHeaderCell.style.display = form.controls.dropShadowCell.style.display = form.controls.shadowSizeRow.style.display =
            form.controls.shadowColorRow.style.display = tabbedPane.tabsPanel.style.display = borderArray.length > 4 ? "" : "none";

        form.controls.dropShadow.setChecked(false);

        if (borderArray.length > 4) {
            tabbedPane.showTabPanel(borderArray.length > 8 ? "Advanced" : "Simple");
            form.controls.dropShadow.setChecked(borderArray[4] == "1");
            form.controls.shadowSize.setKey(borderArray[5] == "StiEmptyValue" ? "1" : borderArray[5]);
            form.controls.shadowColor.setKey(borderArray[6] == "StiEmptyValue" ? "0,0,0" : jsObject.GetColorFromBrushStr(StiBase64.decode(borderArray[6])));
            form.controls.topmost.setChecked(borderArray[7] == "1");

            var advBorders = borderArray.length > 8 ? borderArray[8].split(";") : null;
            var sides = ["Left", "Top", "Right", "Bottom"];
            if (advBorders) tabbedPane.showTabPanel("Advanced");

            for (var i = 0; i < sides.length; i++) {
                var sideButton = form.advControls["border" + sides[i] + "Button"];
                sideButton.borderObject = {
                    size: advBorders ? advBorders[i * 3] : "1",
                    color: advBorders ? advBorders[i * 3 + 1] : "0,0,0",
                    style: advBorders ? (borderSides[i] == "1" ? advBorders[i * 3 + 2] : "6") : "0"
                };
                var styleItem = form.controls.stylesContainer.getItemByName(sideButton.borderObject.style);
                if (styleItem) StiMobileDesigner.setImageSource(sideButton.image, jsObject.options, styleItem.itemObject.imageName);
            }

            form.advControls.dropShadow.setChecked(borderArray[4] == "1");
            form.advControls.shadowSize.setKey(borderArray[5] == "StiEmptyValue" ? "1" : borderArray[5]);
            form.advControls.shadowColor.setKey(borderArray[6] == "StiEmptyValue" ? "0,0,0" : jsObject.GetColorFromBrushStr(StiBase64.decode(borderArray[6])));
            form.advControls.sampleBar.update();
        }

        form.controls.sampleBar.update();
    }

    form.action = function () {
        var border = {};
        border.left = form.controls.borderLeftButton.isSelected ? "1" : "0";
        border.top = form.controls.borderTopButton.isSelected ? "1" : "0";
        border.right = form.controls.borderRightButton.isSelected ? "1" : "0";
        border.bottom = form.controls.borderBottomButton.isSelected ? "1" : "0";
        border.style = form.controls.stylesContainer.selectedItem ? form.controls.stylesContainer.selectedItem.itemObject.key : "0";
        border.color = form.controls.borderColor.key;
        border.size = form.controls.borderSize.key;
        border.dropShadow = form.controls.dropShadow.isChecked ? "1" : "0";
        border.sizeShadow = form.controls.shadowSize.key;
        border.brushShadow = StiBase64.encode("1!" + form.controls.shadowColor.key);
        border.topmost = form.controls.topmost.isChecked ? "1" : "0";

        if (tabbedPane.selectedTab && tabbedPane.selectedTab.panelName == "Advanced") {
            border.advancedBorder = {
                leftSize: form.advControls.borderLeftButton.borderObject.size,
                leftColor: form.advControls.borderLeftButton.borderObject.color,
                leftStyle: form.advControls.borderLeftButton.borderObject.style,
                topSize: form.advControls.borderTopButton.borderObject.size,
                topColor: form.advControls.borderTopButton.borderObject.color,
                topStyle: form.advControls.borderTopButton.borderObject.style,
                rightSize: form.advControls.borderRightButton.borderObject.size,
                rightColor: form.advControls.borderRightButton.borderObject.color,
                rightStyle: form.advControls.borderRightButton.borderObject.style,
                bottomSize: form.advControls.borderBottomButton.borderObject.size,
                bottomColor: form.advControls.borderBottomButton.borderObject.color,
                bottomStyle: form.advControls.borderBottomButton.borderObject.style
            }
            border.dropShadow = form.advControls.dropShadow.isChecked ? "1" : "0";
            border.sizeShadow = form.advControls.shadowSize.key;
            border.brushShadow = StiBase64.encode("1!" + form.advControls.shadowColor.key);
        }

        this.finishFlag = false;
        if (this.actionFunction) this.actionFunction(border);
        if (this.finishFlag) return;

        var selectedObjects = jsObject.options.selectedObjects || [jsObject.options.selectedObject];
        if (selectedObjects) {
            for (var i = 0; i < selectedObjects.length; i++) {
                var borderProperty = selectedObjects[i].properties["border"];
                if (!borderProperty) continue;
                selectedObjects[i].properties.border = jsObject.BordersObjectToStr(border);
            }
            jsObject.UpdatePropertiesControls();
            jsObject.SendCommandSendProperties(selectedObjects, ["border"]);
        }
        this.changeVisibleState(false);
    }

    return form;
}

StiMobileDesigner.prototype.InitializeBorderFormSimplePanel = function (form) {
    var panel = document.createElement("div");
    var mainTable = this.CreateHTMLTable();
    panel.appendChild(mainTable);

    var sidesCell = mainTable.addCell();
    var styleCell = mainTable.addCell();
    var controlsCell = mainTable.addCellInNextRow();
    controlsCell.setAttribute("colspan", "2");
    sidesCell.style.verticalAlign = "top";

    //Sides
    var sidesHeader = this.FormBlockHeader(this.loc.PropertyMain.Sides);
    sidesHeader.style.margin = "2px 0 0 0";
    sidesCell.appendChild(sidesHeader);

    var sidesTable = this.CreateHTMLTable();
    sidesTable.style.margin = "12px";
    sidesCell.appendChild(sidesTable);

    var buttonsAction = function () {
        if (this.name == "All" || this.name == "None") {
            form.controls.borderLeftButton.setSelected(this.name == "All");
            form.controls.borderTopButton.setSelected(this.name == "All");
            form.controls.borderRightButton.setSelected(this.name == "All");
            form.controls.borderBottomButton.setSelected(this.name == "All");
        }
        else {
            this.setSelected(!this.isSelected);
        }
        form.controls.sampleBar.update();
    }

    //All
    var borderAllButton = this.BorderFormButton("All", "BorderAll.png", this.loc.HelpDesigner.BorderSidesAll, 22, 22);
    form.controls.borderAllButton = borderAllButton;
    borderAllButton.action = buttonsAction;
    sidesTable.addCell(borderAllButton);

    //Top
    var borderTopButton = this.BorderFormButton("Top", null, this.loc.HelpDesigner.BorderSidesTop, 80, 22);
    form.controls.borderTopButton = borderTopButton;
    borderTopButton.action = buttonsAction;
    sidesTable.addCell(borderTopButton);
    sidesTable.addCell();

    //Left
    var borderLeftButton = this.BorderFormButton("Left", null, this.loc.HelpDesigner.BorderSidesLeft, 22, 80);
    form.controls.borderLeftButton = borderLeftButton;
    borderLeftButton.action = buttonsAction;
    sidesTable.addCellInNextRow(borderLeftButton);

    //SampleBar
    var sampleBar = this.BorderSampleBar(form);
    form.controls.sampleBar = sampleBar;
    sidesTable.addCellInLastRow(sampleBar);

    //Right
    var borderRightButton = this.BorderFormButton("Right", null, this.loc.HelpDesigner.BorderSidesRight, 22, 80);
    form.controls.borderRightButton = borderRightButton;
    borderRightButton.action = buttonsAction;
    sidesTable.addCellInLastRow(borderRightButton);

    //Bottom
    sidesTable.addCellInNextRow();
    var borderBottomButton = this.BorderFormButton("Bottom", null, this.loc.HelpDesigner.BorderSidesBottom, 80, 22);
    form.controls.borderBottomButton = borderBottomButton;
    borderBottomButton.action = buttonsAction;
    sidesTable.addCellInLastRow(borderBottomButton);

    //None
    var borderNoneButton = this.BorderFormButton("None", "BorderNone.png", this.loc.HelpDesigner.BorderSidesNone, 22, 22);
    borderNoneButton.action = buttonsAction;
    sidesTable.addCellInLastRow(borderNoneButton);

    //Style
    var styleHeader = this.FormBlockHeader(this.loc.PropertyMain.Style);
    styleHeader.style.margin = "2px 0 0 12px";
    styleCell.appendChild(styleHeader);

    var stylesContainer = this.EasyContainer(180, 190);
    stylesContainer.style.margin = "12px 12px 12px 24px";
    form.controls.stylesContainer = stylesContainer;
    styleCell.appendChild(stylesContainer);

    stylesContainer.onAction = function () {
        form.controls.sampleBar.update();
    }

    var styleItems = this.GetBorderStyleItems();
    for (var i = 0; i < styleItems.length; i++) {
        var item = stylesContainer.addItem(styleItems[i].key, styleItems[i], styleItems[i].caption, styleItems[i].imageName, true);
        item.image.style.width = "32px";
    }

    //Border & Shadow
    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.width = "100%";
    controlsCell.appendChild(controlsTable);

    var borderHeader = this.FormBlockHeader(this.loc.PropertyMain.Border);
    borderHeader.style.margin = "0 0 6px 0";
    controlsTable.addCell(borderHeader).setAttribute("colspan", "2");

    //BorderSize   
    controlsTable.addTextCellInNextRow(this.loc.PropertyMain.Size).className = "stiDesignerCaptionControlsBigIntervals";

    var borderSize = this.DropDownList("borderSetupBorderSize", 80, null, this.GetBorderSizeItems(), false);
    borderSize.action = function () {
        form.controls.sampleBar.update();
    }
    form.controls.borderSize = borderSize;
    controlsTable.addCellInLastRow(borderSize).className = "stiDesignerControlCellsBigIntervals2";

    //BorderColor
    controlsTable.addTextCellInNextRow(this.loc.PropertyMain.Color).className = "stiDesignerCaptionControlsBigIntervals";

    var borderColor = this.ColorControl("borderSetupBorderColor", this.loc.PropertyMain.BorderColor, null, null, true);
    borderColor.action = function () {
        form.controls.sampleBar.update();
    }
    form.controls.borderColor = borderColor;
    controlsTable.addCellInLastRow(borderColor).className = "stiDesignerControlCellsBigIntervals2";

    //Topmost    
    var topmost = form.controls.topmost = this.CheckBox("borderSetupTopmost", this.loc.PropertyMain.Topmost);
    topmost.style.margin = "8px 6px 8px 0";
    controlsTable.addTextCellInNextRow("");
    form.controls.topmostCell = controlsTable.addCellInLastRow(topmost);
    form.controls.topmostCell.setAttribute("colspan", "2");

    //Shadow
    var shadowHeader = this.FormBlockHeader(this.loc.PropertyMain.Shadow);
    shadowHeader.style.margin = "6px";
    form.controls.shadowHeaderCell = controlsTable.addCellInNextRow(shadowHeader);
    form.controls.shadowHeaderCell.setAttribute("colspan", "2");

    //ShadowSize
    form.controls.shadowSizeRow = controlsTable.addRow();
    controlsTable.addTextCellInLastRow(this.loc.PropertyMain.ShadowSize).className = "stiDesignerCaptionControlsBigIntervals";

    var shadowSize = this.DropDownList("borderSetupBorderSize", 80, null, this.GetBorderSizeItems(), false);
    shadowSize.action = function () {
        form.controls.sampleBar.update();
    }
    form.controls.shadowSize = shadowSize;
    controlsTable.addCellInLastRow(shadowSize).className = "stiDesignerControlCellsBigIntervals2";

    //ShadowColor   
    form.controls.shadowColorRow = controlsTable.addRow();
    controlsTable.addTextCellInLastRow(this.loc.PropertyMain.Color).className = "stiDesignerCaptionControlsBigIntervals";

    var shadowColor = this.ColorControl("borderSetupBorderColor", this.loc.PropertyMain.BorderColor, null, null, true);
    shadowColor.action = function () {
        form.controls.sampleBar.update();
    }
    form.controls.shadowColor = shadowColor;
    controlsTable.addCellInLastRow(shadowColor).className = "stiDesignerControlCellsBigIntervals2";

    //Drop Shadow
    var dropShadow = form.controls.dropShadow = this.CheckBox("borderSetupDropShadow", this.loc.PropertyMain.DropShadow);
    dropShadow.style.margin = "8px 6px 8px 0";

    controlsTable.addTextCellInNextRow("");
    form.controls.dropShadowCell = controlsTable.addCellInLastRow(dropShadow);
    form.controls.dropShadowCell.setAttribute("colspan", "2");

    dropShadow.action = function () {
        form.controls.sampleBar.update();
    }

    return panel;
}

StiMobileDesigner.prototype.InitializeBorderFormAdvancedPanel = function (form) {
    var jsObject = this;
    var panel = document.createElement("div");
    var mainTable = this.CreateHTMLTable();
    panel.appendChild(mainTable);

    var sidesCell = mainTable.addCell();
    var controlsCell = mainTable.addCellInNextRow();

    //Sides
    var sidesHeader = this.FormBlockHeader(this.loc.PropertyMain.Sides);
    sidesHeader.style.margin = "2px 0 6px 0";
    sidesHeader.firstChild.style.textAlign = "left";
    sidesCell.appendChild(sidesHeader);

    var sidesTable = this.CreateHTMLTable();
    sidesTable.style.display = "inline-block";
    sidesCell.appendChild(sidesTable);
    sidesCell.style.textAlign = "center";

    var sidesMenu = this.VerticalMenu("bordrFormSidesMenu", null, "Down", []);

    //Style
    var stylesContainer = this.EasyContainer(180, 190);
    sidesMenu.innerContent.appendChild(stylesContainer);

    var styleItems = this.GetBorderStyleItems();
    for (var i = 0; i < styleItems.length; i++) {
        var item = stylesContainer.addItem(styleItems[i].key, styleItems[i], styleItems[i].caption, styleItems[i].imageName, true);
        item.image.style.width = "32px";
    }

    stylesContainer.onAction = function () {
        sidesMenu.parentButton.borderObject.style = this.selectedItem.itemObject.key;
        StiMobileDesigner.setImageSource(sidesMenu.parentButton.image, jsObject.options, this.selectedItem.itemObject.imageName);
        form.advControls.sampleBar.update();
    }

    sidesMenu.innerContent.appendChild(this.FormSeparator());

    var menuTable = this.CreateHTMLTable();
    menuTable.style.width = "100%";
    menuTable.style.margin = "6px 0 6px 0";
    sidesMenu.innerContent.appendChild(menuTable);

    menuTable.addTextCell(this.loc.PropertyMain.Size).className = "stiDesignerCaptionControlsBigIntervals";

    //Size
    var borderSize = this.DropDownList("borderSetupAdvBorderSize", 80, null, this.GetBorderSizeItems(), false);
    borderSize.action = function () {
        sidesMenu.parentButton.borderObject.size = this.key;
        form.advControls.sampleBar.update();
    }
    menuTable.addCellInLastRow(borderSize).className = "stiDesignerControlCellsBigIntervals2";

    menuTable.addTextCellInNextRow(this.loc.PropertyMain.Color).className = "stiDesignerCaptionControlsBigIntervals";

    //Color
    var borderColor = this.ColorControl("borderSetupAdvBorderColor", this.loc.PropertyMain.BorderColor, null, null, true);
    borderColor.action = function () {
        sidesMenu.parentButton.borderObject.color = this.key;
        form.advControls.sampleBar.update();
    }
    menuTable.addCellInLastRow(borderColor).className = "stiDesignerControlCellsBigIntervals2";

    sidesMenu.onshow = function () {
        var borderObject = this.parentButton.borderObject;
        borderSize.setKey(borderObject.size);
        borderColor.setKey(borderObject.color);
        var styleItem = stylesContainer.getItemByName(borderObject.style);
        var selectedItem = styleItem || stylesContainer.childNodes[0];
        selectedItem.select();
    }

    var buttonsAction = function () {
        sidesMenu.parentButton = this;
        sidesMenu.changeVisibleState(true);
    }

    //Top
    sidesTable.addCell();
    var borderTopButton = this.BorderFormButton("Top", "BorderStyleNone.png", this.loc.HelpDesigner.BorderSidesTop, 80, 22, 32);
    form.advControls.borderTopButton = borderTopButton;
    borderTopButton.action = buttonsAction;
    sidesTable.addCell(borderTopButton);
    sidesTable.addCell();

    //Left
    var borderLeftButton = this.BorderFormButton("Left", "BorderStyleNone.png", this.loc.HelpDesigner.BorderSidesLeft, 22, 80, 32);
    form.advControls.borderLeftButton = borderLeftButton;
    borderLeftButton.image.className = "stiDesignerBorderImageRotate90";
    borderLeftButton.action = buttonsAction;
    sidesTable.addCellInNextRow(borderLeftButton);

    //SampleBar
    var sampleBar = this.BorderSampleBar(form);
    form.advControls.sampleBar = sampleBar;
    sidesTable.addCellInLastRow(sampleBar);

    sampleBar.update = function () {
        var sides = ["Left", "Top", "Right", "Bottom"];

        for (var i = 0; i < sides.length; i++) {
            var sideButton = form.advControls["border" + sides[i] + "Button"];

            var borderColor = jsObject.GetHTMLColor(sideButton.borderObject.color);
            var currentStyle = sideButton.borderObject.style;
            var currentborderSize = sideButton.borderObject.size;
            var borderSize = (currentStyle == "5") ? "3" : currentborderSize;
            var styles = ["solid", "dashed", "dashed", "dashed", "dotted", "double", "none"];

            this.bar.style["border" + sides[i]] = borderSize + "px " + styles[currentStyle] + " " + (currentStyle != "6" ? borderColor : "transparent");
        }

        this.bar.style.boxShadow = form.advControls.dropShadow.isChecked ? "3px 3px " + form.advControls.shadowSize.key + "px " + jsObject.GetHTMLColor(form.advControls.shadowColor.key) : "";
    }

    //Right
    var borderRightButton = this.BorderFormButton("Right", "BorderStyleNone.png", this.loc.HelpDesigner.BorderSidesRight, 22, 80, 32);
    form.advControls.borderRightButton = borderRightButton;
    borderRightButton.image.className = "stiDesignerBorderImageRotate90";
    borderRightButton.action = buttonsAction;
    sidesTable.addCellInLastRow(borderRightButton);

    //Bottom
    sidesTable.addCellInNextRow();
    var borderBottomButton = this.BorderFormButton("Bottom", "BorderStyleNone.png", this.loc.HelpDesigner.BorderSidesBottom, 80, 22, 32);
    form.advControls.borderBottomButton = borderBottomButton;
    borderBottomButton.action = buttonsAction;
    sidesTable.addCellInLastRow(borderBottomButton);
    sidesTable.addCellInLastRow();

    //Shadow
    var controlsTable = this.CreateHTMLTable();
    controlsTable.style.width = "100%";
    controlsCell.appendChild(controlsTable);

    var shadowHeader = this.FormBlockHeader(this.loc.PropertyMain.Shadow);
    shadowHeader.style.margin = "6px";
    form.advControls.shadowHeaderCell = controlsTable.addCellInNextRow(shadowHeader);
    form.advControls.shadowHeaderCell.setAttribute("colspan", "2");

    //ShadowSize
    form.advControls.shadowSizeRow = controlsTable.addRow();
    controlsTable.addTextCellInLastRow(this.loc.PropertyMain.ShadowSize).className = "stiDesignerCaptionControlsBigIntervals";

    var shadowSize = form.advControls.shadowSize = this.DropDownList("borderSetupBorderSize", 80, null, this.GetBorderSizeItems(), false);
    shadowSize.style.margin = "0 20px 0 20px";
    controlsTable.addCellInLastRow(shadowSize).className = "stiDesignerControlCellsBigIntervals2";

    shadowSize.action = function () {
        form.advControls.sampleBar.update();
    }

    //ShadowColor   
    form.advControls.shadowColorRow = controlsTable.addRow();
    controlsTable.addTextCellInLastRow(this.loc.PropertyMain.Color).className = "stiDesignerCaptionControlsBigIntervals";

    var shadowColor = form.advControls.shadowColor = this.ColorControl("borderSetupBorderColor", this.loc.PropertyMain.BorderColor, null, null, true);
    shadowColor.style.margin = "0 20px 0 20px";
    controlsTable.addCellInLastRow(shadowColor).className = "stiDesignerControlCellsBigIntervals2";

    shadowColor.action = function () {
        form.advControls.sampleBar.update();
    }

    //Drop Shadow
    var dropShadow = form.advControls.dropShadow = this.CheckBox("borderSetupDropShadow", this.loc.PropertyMain.DropShadow);
    dropShadow.style.margin = "8px 26px 8px 20px";

    controlsTable.addTextCellInNextRow("");
    form.advControls.dropShadowCell = controlsTable.addCellInLastRow(dropShadow);
    form.advControls.dropShadowCell.setAttribute("colspan", "2");

    dropShadow.action = function () {
        form.advControls.sampleBar.update();
    }

    return panel;
}

StiMobileDesigner.prototype.BorderSampleBar = function (form) {
    var jsObject = this;
    var sampleBar = this.CreateHTMLTable();
    sampleBar.setAttribute("style", "width: 80px; height: 80px; margin: 4px; background: #f5f5f5; border: 1px solid #d3d3d3;");

    var bar = document.createElement("div");
    bar.setAttribute("style", "width: 60px; height: 60px; display: inline-block;");
    sampleBar.bar = bar;
    sampleBar.addCell(bar).style.textAlign = "center";

    sampleBar.update = function () {
        var borderColor = jsObject.GetHTMLColor(form.controls.borderColor.key);
        var currentStyle = form.controls.stylesContainer.selectedItem ? form.controls.stylesContainer.selectedItem.itemObject.key : "0";
        var currentborderSize = form.controls.borderSize.key;
        var borderSize = (currentStyle == "5") ? "3" : currentborderSize;
        var styles = ["solid", "dashed", "dashed", "dashed", "dotted", "double", "none"];

        bar.style.borderLeft = borderSize + "px " + styles[currentStyle] + " " +
            (form.controls.borderLeftButton.isSelected ? borderColor : "transparent");
        bar.style.borderTop = borderSize + "px " + styles[currentStyle] + " " +
            (form.controls.borderTopButton.isSelected ? borderColor : "transparent");
        bar.style.borderRight = borderSize + "px " + styles[currentStyle] + " " +
            (form.controls.borderRightButton.isSelected ? borderColor : "transparent");
        bar.style.borderBottom = borderSize + "px " + styles[currentStyle] + " " +
            (form.controls.borderBottomButton.isSelected ? borderColor : "transparent");

        bar.style.boxShadow = form.controls.dropShadow.isChecked ? "3px 3px " + form.controls.shadowSize.key + "px " + jsObject.GetHTMLColor(form.controls.shadowColor.key) : "";
    }

    return sampleBar;
}

StiMobileDesigner.prototype.BorderFormButton = function (name, imageName, toolTip, width, height, imageWidth, imageHeight) {
    var button = this.FormButtonWithThemeBorder(name, null, null, imageName, toolTip);
    button.name = name;
    button.style.margin = "4px";
    button.style.width = width + "px";
    button.style.height = height + "px";
    button.innerTable.style.width = "100%";

    if (button.imageCell) {
        button.imageCell.style.padding = "0px";
    }

    if (button.image) {
        if (imageWidth) button.image.style.width = imageWidth + "px";
        if (imageHeight) button.image.style.height = imageHeight + "px";
    }

    return button;
}
