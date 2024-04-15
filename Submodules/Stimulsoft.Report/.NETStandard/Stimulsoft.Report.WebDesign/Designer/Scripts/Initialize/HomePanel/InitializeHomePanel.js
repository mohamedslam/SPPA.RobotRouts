
StiMobileDesigner.prototype.InitializeHomePanel = function () {
    var jsObject = this;
    var homePanel = this.ChildWorkPanel("homePanel", "stiDesignerHomePanel");
    this.options.workPanel.showPanel(homePanel);

    homePanel.mainTable = this.CreateHTMLTable();
    homePanel.mainTable.addCell(this.HomePanelUndoBlock());
    homePanel.mainTable.addCell(this.GroupBlockSeparator("homeUndoBlockSeparator"));
    homePanel.mainTable.addCell(this.HomePanelClipboardBlock());
    if (!this.options.isTouchDevice) homePanel.mainTable.addCell(this.GroupBlockSeparator());
    homePanel.mainTable.addCell(this.HomePanelFontBlock());
    homePanel.mainTable.addCell(this.GroupBlockSeparator());
    homePanel.mainTable.addCell(this.HomePanelAlignmentBlock());
    homePanel.mainTable.addCell(this.GroupBlockSeparator());
    homePanel.mainTable.addCell(this.HomePanelBordersBlock());
    homePanel.mainTable.addCell(this.GroupBlockSeparator());
    homePanel.mainTable.addCell(this.HomePanelTextFormatBlock());
    homePanel.mainTable.addCell(this.GroupBlockSeparator());
    homePanel.mainTable.addCell(this.HomePanelStyleBlock());
    homePanel.mainTable.addCell(this.GroupBlockSeparator());
    homePanel.appendChild(homePanel.mainTable);

    homePanel.onshow = function () {
        if (this.jsObject.options.report && this.jsObject.options.selectedObject) { this.updateControls(); }
    }

    homePanel.updateControls = function () {
        var controls = this.jsObject.options.controls;
        var buttons = this.jsObject.options.buttons;
        var report = this.jsObject.options.report;
        var currentObject = this.jsObject.options.selectedObject || this.jsObject.GetCommonObject(this.jsObject.options.selectedObjects);
        if (!currentObject) return;
        var styleObject = currentObject ? this.jsObject.getStyleObject(currentObject.properties.componentStyle) : null;

        //Clipboard
        var canRemove = report && ((currentObject.properties.restrictions &&
            (currentObject.properties.restrictions == "All" || currentObject.properties.restrictions.indexOf("AllowDelete") >= 0)) || !currentObject.properties.restrictions);

        var isNotReportOrPage = report && currentObject.typeComponent != "StiPage" && currentObject.typeComponent != "StiReport";
        buttons.removeComponent.setEnabled(isNotReportOrPage && canRemove);
        buttons.copyComponent.setEnabled(isNotReportOrPage && !this.jsObject.IsTableCell(this.jsObject.options.selectedObjects || currentObject));
        buttons.cutComponent.setEnabled(isNotReportOrPage && canRemove && !this.jsObject.IsTableCell(this.jsObject.options.selectedObjects || currentObject));
        buttons.pasteComponent.setEnabled(report && currentObject.typeComponent != "StiReport");

        //Font        
        var fontArray = (report && currentObject.properties["font"]) ? currentObject.properties["font"].split("!") : null;

        controls.homePanelFontName.setEnabled(fontArray != null && !styleObject.allowUseFont && !currentObject.properties.allowHtmlTags);
        if (controls.homePanelFontName.isEnabled) controls.homePanelFontName.setKey(fontArray[0]);
        controls.homePanelFontSize.setEnabled(fontArray != null && !styleObject.allowUseFont && !currentObject.properties.allowHtmlTags);
        if (controls.homePanelFontSize.isEnabled) controls.homePanelFontSize.setKey(fontArray[1]);
        buttons.homePanelFontBold.setEnabled(fontArray != null && !styleObject.allowUseFont && currentObject.typeComponent != "StiProgressElement" && !currentObject.properties.allowHtmlTags);
        if (buttons.homePanelFontBold.isEnabled) buttons.homePanelFontBold.setSelected(fontArray[2] == "StiEmptyValue" ? false : fontArray[2] == "1");
        buttons.homePanelFontItalic.setEnabled(fontArray != null && !styleObject.allowUseFont && currentObject.typeComponent != "StiProgressElement" && !currentObject.properties.allowHtmlTags);
        if (buttons.homePanelFontItalic.isEnabled) buttons.homePanelFontItalic.setSelected(fontArray[3] == "StiEmptyValue" ? false : fontArray[3] == "1");
        buttons.homePanelFontUnderline.setEnabled(fontArray != null && !styleObject.allowUseFont && currentObject.typeComponent != "StiProgressElement" && !currentObject.properties.allowHtmlTags);
        if (buttons.homePanelFontUnderline.isEnabled) buttons.homePanelFontUnderline.setSelected(fontArray[4] == "StiEmptyValue" ? false : fontArray[4] == "1");
        buttons.homePanelFontStrikeout.setEnabled(fontArray != null && !styleObject.allowUseFont && currentObject.typeComponent != "StiProgressElement" && currentObject.typeComponent != "StiTextElement" && !currentObject.properties.allowHtmlTags);
        if (buttons.homePanelFontStrikeout.isEnabled) buttons.homePanelFontStrikeout.setSelected(fontArray[5] == "StiEmptyValue" ? false : fontArray[5] == "1");

        var textBrush = report ? (currentObject.properties["textBrush"] || currentObject.properties["foreColor"]) : null;
        controls.homePanelTextColor.setEnabled(textBrush != null && !styleObject.allowUseTextBrush && currentObject.typeComponent != "StiProgressElement" && !currentObject.properties.allowHtmlTags);

        //dbs table element cells        
        if (currentObject.typeComponent == "StiTableElement") {
            var editTableElementForm = this.jsObject.options.forms.editTableElementForm;
            if (editTableElementForm && editTableElementForm.visible && editTableElementForm.controls.dataContainer.selectedItem) {
                textBrush = editTableElementForm.controls.dataContainer.selectedItem.itemObject.foreColor;
                controls.homePanelTextColor.setEnabled(true);
            }
        }

        if (controls.homePanelTextColor.isEnabled)
            controls.homePanelTextColor.setKey(textBrush == "StiEmptyValue" ? textBrush : this.jsObject.GetColorFromBrushStr(textBrush), currentObject.isDashboardElement);
        else
            controls.homePanelTextColor.setKey("255,255,255");

        //Alignment        
        var vertAlignment = (report && currentObject.properties["vertAlignment"]) ? currentObject.properties["vertAlignment"] : null;

        buttons.homePanelAlignTop.setEnabled(vertAlignment && !styleObject.allowUseVertAlignment);
        if (buttons.homePanelAlignTop.isEnabled) buttons.homePanelAlignTop.setSelected(vertAlignment != "StiEmptyValue" && vertAlignment == "Top");
        buttons.homePanelAlignMiddle.setEnabled(vertAlignment && !styleObject.allowUseVertAlignment);
        if (buttons.homePanelAlignMiddle.isEnabled) buttons.homePanelAlignMiddle.setSelected(vertAlignment != "StiEmptyValue" && vertAlignment == "Center");
        buttons.homePanelAlignBottom.setEnabled(vertAlignment && !styleObject.allowUseVertAlignment);
        if (buttons.homePanelAlignBottom.isEnabled) buttons.homePanelAlignBottom.setSelected(vertAlignment != "StiEmptyValue" && vertAlignment == "Bottom");
        buttons.homePanelAlignBottom.setEnabled(vertAlignment && !styleObject.allowUseVertAlignment);
        if (buttons.homePanelAlignBottom.isEnabled) buttons.homePanelAlignBottom.setSelected(vertAlignment != "StiEmptyValue" && vertAlignment == "Bottom");
        buttons.homePanelWordWrap.setEnabled(report && currentObject.properties["wordWrap"] != null);
        if (buttons.homePanelWordWrap.isEnabled) buttons.homePanelWordWrap.setSelected(currentObject.properties["wordWrap"] != "StiEmptyValue"
            && currentObject.properties["wordWrap"]);
        buttons.homePanelTextAngle.setEnabled(report && currentObject.properties["textAngle"] != null && currentObject.properties["textAngle"] != "StiEmptyValue");

        var horAlignment = (report && currentObject.properties["horAlignment"]) ? currentObject.properties["horAlignment"] : null;

        //dbs table element cells        
        if (currentObject.typeComponent == "StiTableElement") {
            var editTableElementForm = this.jsObject.options.forms.editTableElementForm;
            if (editTableElementForm && editTableElementForm.visible && editTableElementForm.controls.dataContainer.selectedItem) {
                horAlignment = editTableElementForm.controls.dataContainer.selectedItem.itemObject.horAlignment;
            }
        }

        buttons.homePanelAlignLeft.setEnabled(horAlignment && !styleObject.allowUseHorAlignment);
        if (buttons.homePanelAlignLeft.isEnabled) buttons.homePanelAlignLeft.setSelected(horAlignment != "StiEmptyValue" && horAlignment == "Left");
        buttons.homePanelAlignCenter.setEnabled(horAlignment && !styleObject.allowUseHorAlignment);
        if (buttons.homePanelAlignCenter.isEnabled) buttons.homePanelAlignCenter.setSelected(horAlignment != "StiEmptyValue" && horAlignment == "Center");
        buttons.homePanelAlignRight.setEnabled(horAlignment && !styleObject.allowUseHorAlignment);
        if (buttons.homePanelAlignRight.isEnabled) buttons.homePanelAlignRight.setSelected(horAlignment != "StiEmptyValue" && horAlignment == "Right");
        buttons.homePanelAlignWidth.setEnabled(horAlignment && !styleObject.allowUseHorAlignment && currentObject.typeComponent && currentObject.typeComponent != "StiImage" &&
            currentObject.typeComponent != "StiBarCode" && currentObject.typeComponent != "StiTableElement");
        if (buttons.homePanelAlignWidth.isEnabled) buttons.homePanelAlignWidth.setSelected(horAlignment != "StiEmptyValue" && horAlignment == "Width");
        buttons.homePanelLineSpacing.setEnabled(report && currentObject.properties["lineSpacing"] != null && currentObject.properties["lineSpacing"] != "StiEmptyValue");

        //Borders
        var borderArray = (report && currentObject.properties["border"]) ? currentObject.properties["border"].split("!") : null;
        var borderSides = (borderArray) ? borderArray[0].split(",") : null;
        var ignoreStyles = currentObject && !currentObject.properties.allowApplyStyle;

        buttons.groupBlockBordersButton.setEnabled(borderArray != null && ignoreStyles);
        buttons.homePanelBorderAll.setEnabled(borderArray != null && !styleObject.allowUseBorderSides && ignoreStyles);
        if (buttons.homePanelBorderAll.isEnabled) buttons.homePanelBorderAll.setSelected(borderArray[0] == "StiEmptyValue" ? false : borderArray[0] == "1,1,1,1");
        buttons.homePanelBorderNone.setEnabled(borderArray != null && !styleObject.allowUseBorderSides && ignoreStyles);
        if (buttons.homePanelBorderNone.isEnabled) buttons.homePanelBorderNone.setSelected(borderArray[0] == "StiEmptyValue" ? false : borderArray[0] == "0,0,0,0");
        buttons.homePanelBorderLeft.setEnabled(borderArray != null && !styleObject.allowUseBorderSides && ignoreStyles);
        if (buttons.homePanelBorderLeft.isEnabled) buttons.homePanelBorderLeft.setSelected(borderSides && borderSides[0] != "StiEmptyValue" ? borderSides[0] == "1" : false);
        buttons.homePanelBorderTop.setEnabled(borderArray != null && !styleObject.allowUseBorderSides && ignoreStyles);
        if (buttons.homePanelBorderTop.isEnabled) buttons.homePanelBorderTop.setSelected(borderSides && borderSides[1] != "StiEmptyValue" ? borderSides[1] == "1" : false);
        buttons.homePanelBorderRight.setEnabled(borderArray != null && !styleObject.allowUseBorderSides && ignoreStyles);
        if (buttons.homePanelBorderRight.isEnabled) buttons.homePanelBorderRight.setSelected(borderSides && borderSides[2] != "StiEmptyValue" ? borderSides[2] == "1" : false);
        buttons.homePanelBorderBottom.setEnabled(borderArray != null && !styleObject.allowUseBorderSides && ignoreStyles);
        if (buttons.homePanelBorderBottom.isEnabled) buttons.homePanelBorderBottom.setSelected(borderSides && borderSides[3] != "StiEmptyValue" ? borderSides[3] == "1" : false);
        buttons.homePanelShadow.setEnabled((borderArray != null && borderArray.length > 4 && !styleObject.allowUseBorderFormatting && ignoreStyles) || (currentObject && currentObject.properties.shadowVisible != null));
        if (buttons.homePanelShadow.isEnabled) buttons.homePanelShadow.setSelected(currentObject.properties.shadowVisible != null ? currentObject.properties.shadowVisible : (borderArray[4] == "StiEmptyValue" ? false : borderArray[4] == "1"));

        var borderColor = report ? ((borderArray ? borderArray[2] : null) || currentObject.properties["shapeBorderColor"]) : null;
        controls.homePanelBorderColor.setEnabled(borderColor != null && !styleObject.allowUseBorderFormatting && ignoreStyles);
        if (controls.homePanelBorderColor.isEnabled) controls.homePanelBorderColor.setKey(borderColor)
        else controls.homePanelBorderColor.setKey("255,255,255");

        var backGroundColor = report ? (currentObject.properties["brush"] || currentObject.properties["backColor"]) : null;
        controls.homePanelBackgroundColor.setEnabled(backGroundColor && !styleObject.allowUseBrush && ignoreStyles);

        if (controls.homePanelBackgroundColor.isEnabled)
            controls.homePanelBackgroundColor.setKey(this.jsObject.GetColorFromBrushStr(backGroundColor), currentObject.isDashboardElement || currentObject.isDashboard);
        else
            controls.homePanelBackgroundColor.setKey("255,255,255");

        var borderStyle = report ? ((borderArray ? borderArray[3] : null) || currentObject.properties["shapeBorderStyle"]) : null;
        controls.homePanelBorderStyle.setEnabled(borderStyle != null && !styleObject.allowUseBorderFormatting && ignoreStyles);
        if (controls.homePanelBorderStyle.isEnabled) controls.homePanelBorderStyle.setKey(borderStyle)
        else controls.homePanelBorderStyle.setKey("0");

        controls.groupBlockBorders.button.setEnabled(borderArray && !styleObject.allowUseBorderSides && !styleObject.allowUseBorderFormatting);

        //Conditions
        buttons.conditionsButton.setEnabled(report &&
            (currentObject.properties.conditions != null ||
                currentObject.properties.chartConditions != null ||
                currentObject.properties.indicatorConditions != null ||
                currentObject.properties.progressConditions != null ||
                currentObject.properties.tableConditions != null ||
                currentObject.properties.pivotTableConditions != null));

        var conditionsPresent = currentObject.properties.conditions ||
            (currentObject.properties.chartConditions && currentObject.properties.chartConditions.length > 0) ||
            (currentObject.properties.indicatorConditions && currentObject.properties.indicatorConditions.length > 0) ||
            (currentObject.properties.progressConditions && currentObject.properties.progressConditions.length > 0) ||
            (currentObject.properties.tableConditions && currentObject.properties.tableConditions.length > 0) ||
            (currentObject.properties.pivotTableConditions && currentObject.properties.pivotTableConditions.length > 0);

        buttons.conditionsButton.setMarkerVisibleState(buttons.conditionsButton.isEnabled && conditionsPresent);

        //TextFormat
        var textFormat = (report && currentObject.typeComponent != "StiReport" && currentObject.properties["textFormat"]) ? currentObject.properties["textFormat"] : null;

        //dbs table element cells        
        if (currentObject.typeComponent == "StiTableElement") {
            var editTableElementForm = this.jsObject.options.forms.editTableElementForm;
            if (editTableElementForm && editTableElementForm.visible && editTableElementForm.controls.dataContainer.selectedItem) {
                textFormat = editTableElementForm.controls.dataContainer.selectedItem.itemObject.textFormat;
            }
        }

        //dbs pivot table element cells        
        if (currentObject.typeComponent == "StiPivotTableElement") {
            var editPivotTableElementForm = this.jsObject.options.forms.editPivotTableElementForm;
            if (editPivotTableElementForm && editPivotTableElementForm.visible && editPivotTableElementForm.getSelectedItem()) {
                textFormat = editPivotTableElementForm.getSelectedItem().itemObject.textFormat;
            }
        }

        controls.homePanelTextFormat.setEnabled(textFormat != null);
        controls.groupBlockTextFormat.button.setEnabled(textFormat != null);
        if (controls.homePanelTextFormat.isEnabled) controls.homePanelTextFormat.setKey(textFormat.type)
        else controls.homePanelTextFormat.setKey("StiGeneralFormatService");

        //Interaction
        buttons.homePanelInteractionsButton.setEnabled(report && (currentObject.properties["dashboardInteraction"] != null || currentObject.properties["interaction"] != null));
        buttons.homePanelInteractionsButton.setMarkerVisibleState(buttons.homePanelInteractionsButton.isEnabled && jsObject.InteractionsPresent(currentObject));

        //Style
        var style = null;
        if (report && report.stylesCollection && currentObject.typeComponent != "StiReport") {
            if (currentObject.typeComponent == "StiChart") {
                style = currentObject.properties.chartStyle;
            }
            else if (currentObject.typeComponent == "StiGauge") {
                style = currentObject.properties.gaugeStyle;
            }
            else if (currentObject.typeComponent == "StiMap") {
                style = currentObject.properties.mapStyle;
            }
            else if (currentObject.typeComponent == "StiCrossTab") {
                style = {
                    crossTabStyleIndex: currentObject.properties.crossTabFields.crossTabStyleIndex,
                    crossTabStyle: currentObject.properties.crossTabFields.crossTabStyle
                }
            }
            else if (currentObject.typeComponent == "StiTable") {
                style = {
                    styleName: currentObject.properties.componentStyle,
                    styleId: currentObject.properties.styleId
                }
            }
            else if ((currentObject.isDashboard || currentObject.isDashboardElement) && currentObject.typeComponent) {
                if (currentObject.properties.elementStyle || currentObject.properties.customStyleName) {
                    style = {
                        ident: currentObject.properties.elementStyle,
                        name: currentObject.properties.customStyleName
                    }
                }
                else {
                    style = null;
                }
            }
            else {
                style = currentObject.properties.componentStyle;
            }
        }

        controls.homePanelStyle.setEnabled(style != null);

        if (controls.homePanelStyle.isEnabled)
            controls.homePanelStyle.setKey(style);
        else
            controls.homePanelStyle.setKey("[None]");
    }
}

StiMobileDesigner.prototype.InteractionsPresent = function (component) {
    if (component) {
        var interaction = component.properties.interaction;
        if (interaction) {
            if (interaction.isBandInteraction && interaction.collapsingEnabled) return true;
            if (interaction.drillDownEnabled) return true;
            for (var i = 1; i <= 5; i++) {
                if (interaction["drillDownParameter" + i + "Name"] || interaction["drillDownParameter" + i + "Expression"])
                    return true;
            }
            if (interaction.sortingEnabled && interaction.sortingColumn) return true;
            if (interaction.isBandInteraction && interaction.selectionEnabled) return true;
            if (interaction.bookmark || interaction.hyperlink || interaction.tag || interaction.toolTip) return true;
        }
        var dashboardInteraction = component.properties.dashboardInteraction;
        if (dashboardInteraction && dashboardInteraction.isDefault === false) {
            return true;
        }
    }
    return false;
}

//Undo
StiMobileDesigner.prototype.HomePanelUndoBlock = function () {
    var undoGroupBlock = this.GroupBlock("groupBlockUndo", this.loc.MainMenu.menuEditUndo.replace("&", ""), false, null);
    var innerTable = this.CreateHTMLTable();
    undoGroupBlock.container.appendChild(innerTable);
    innerTable.style.height = innerTable.style.width = "100%";

    //Undo Button
    var undoButton = this.StandartSmallButton("undoButton", null, null, "Toolbar.Undo.png", [this.loc.MainMenu.menuEditUndo.replace("&", ""), this.HelpLinks["default"]], null);
    undoButton.style.display = "inline-block";
    innerTable.addCell(undoButton).style.textAlign = "center";

    //Redo
    var redoButton = this.StandartSmallButton("redoButton", null, null, "Toolbar.Redo.png", [this.loc.MainMenu.menuEditRedo.replace("&", ""), this.HelpLinks["default"]], null);
    redoButton.style.display = "inline-block";
    innerTable.addCellInNextRow(redoButton).style.textAlign = "center";

    return undoGroupBlock;
}

//Clipboard
StiMobileDesigner.prototype.HomePanelClipboardBlock = function () {
    var jsObject = this;
    var clipboardGroupBlock = this.GroupBlock("groupBlockClipboard", this.loc.Toolbars.ToolbarClipboard, false, null);
    var innerTable = this.CreateHTMLTable();
    clipboardGroupBlock.container.appendChild(innerTable);
    innerTable.style.height = "100%";
    if (this.options.isTouchDevice) clipboardGroupBlock.style.display = "none";

    //Paste
    var pasteCell = innerTable.addCell();
    pasteCell.setAttribute("rowspan", "3");
    var pasteButton = this.StandartBigButton("pasteComponent", null, this.loc.MainMenu.menuEditPaste.replace("&", ""), "Paste.png",
        [this.loc.HelpDesigner.menuEditPaste, this.HelpLinks["clipboard"]]);
    pasteCell.appendChild(pasteButton);
    pasteCell.style.padding = "1px 1px 0 1px";

    pasteButton.onmousedown = function () {
        if (this.isTouchStartFlag || !this.isEnabled) return;
        jsObject.options.buttonPressed = this;
        jsObject.PasteCurrentClipboardComponent();
    }

    pasteButton.action = function () {
        jsObject.readTextFromClipboard(function (clipboardResult) {
            if (clipboardResult)
                jsObject.SendCommandGetFromClipboard(clipboardResult)
            else
                jsObject.ExecuteAction("pasteComponent");
        });
    }

    //Copy
    var copyButton = this.StandartSmallButton("copyComponent", null, this.loc.MainMenu.menuEditCopy.replace("&", ""), "Copy.png", [this.loc.HelpDesigner.menuEditCopy, this.HelpLinks["clipboard"]], null);
    innerTable.addCell(copyButton).style.padding = "1px 1px 0 1px";
    copyButton.style.height = this.options.isTouchDevice ? "26px" : "22px";

    //Cut
    var cutButton = this.StandartSmallButton("cutComponent", null, this.loc.MainMenu.menuEditCut.replace("&", ""), "Cut.png", [this.loc.HelpDesigner.menuEditCut, this.HelpLinks["clipboard"]], null);
    innerTable.addCellInNextRow(cutButton).style.padding = "0 1px 0 1px";
    cutButton.style.height = this.options.isTouchDevice ? "26px" : "22px";

    //Remove
    var removeButton = this.StandartSmallButton("removeComponent", null, this.loc.MainMenu.menuEditDelete.replace("&", ""), "Remove.png", [this.loc.HelpDesigner.menuEditDelete, this.HelpLinks["clipboard"]], null);
    innerTable.addCellInNextRow(removeButton).style.padding = "0 1px 0 1px";
    removeButton.style.height = this.options.isTouchDevice ? "26px" : "22px";

    return clipboardGroupBlock;
}

//Font
StiMobileDesigner.prototype.HomePanelFontBlock = function () {
    var fontGroupBlock = this.GroupBlock("groupBlockFont", this.loc.Toolbars.ToolbarFont, false, null);
    var innerTable = this.CreateHTMLTable();
    fontGroupBlock.container.appendChild(innerTable);
    innerTable.style.height = "100%";
    var upTable = this.CreateHTMLTable();
    var downTable = this.CreateHTMLTable();
    innerTable.addCell(upTable);
    innerTable.addCellInNextRow(downTable);

    //Name
    var fontName = this.FontList("homePanelFontName", this.options.isTouchDevice ? 131 : 105, null, null, [this.loc.HelpDesigner.FontName, this.HelpLinks["font"]]);

    fontName.action = function () {
        if (this.key == "Aharoni") { this.jsObject.options.buttons.homePanelFontBold.setSelected(true); }
        this.jsObject.options.buttons.homePanelFontBold.isEnabled = !(this.key == "Aharoni");
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            var font = this.jsObject.FontStrToObject(selectedObjects[i].properties.font);
            font.name = this.key;
            selectedObjects[i].properties.font = this.jsObject.FontObjectToStr(font);
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["font"]);
    };

    upTable.addCell(fontName).style.padding = "0 1px 0 2px";

    //Size    
    var fontSize = this.DropDownList("homePanelFontSize", 40, [this.loc.HelpDesigner.FontSize, this.HelpLinks["font"]], this.GetFontSizeItems(), false);

    fontSize.action = function () {
        var sizeValue = Math.abs(this.jsObject.StrToDouble(this.key));
        if (sizeValue == 0) sizeValue = 1;
        this.setKey(sizeValue.toString());
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            var font = this.jsObject.FontStrToObject(selectedObjects[i].properties.font);
            font.size = this.key;
            selectedObjects[i].properties.font = this.jsObject.FontObjectToStr(font);
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["font"]);
    }

    upTable.addCell(fontSize).style.padding = "0 2px 0 1px";

    //Bold
    var boldButton = this.StandartSmallButton("homePanelFontBold", null, null, "Bold.png", [this.loc.HelpDesigner.FontStyleBold, this.HelpLinks["font"]], null);

    boldButton.action = function () {
        this.setSelected(!this.isSelected);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            var font = this.jsObject.FontStrToObject(selectedObjects[i].properties.font);
            font.bold = this.isSelected ? "1" : "0";
            selectedObjects[i].properties.font = this.jsObject.FontObjectToStr(font);
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["font"]);
    }

    downTable.addCell(boldButton).style.padding = "0 2px 0 2px";

    //Italic
    var italicButton = this.StandartSmallButton("homePanelFontItalic", null, null, "Italic.png", [this.loc.HelpDesigner.FontStyleItalic, this.HelpLinks["font"]], null);

    italicButton.action = function () {
        this.setSelected(!this.isSelected);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            var font = this.jsObject.FontStrToObject(selectedObjects[i].properties.font);
            font.italic = this.isSelected ? "1" : "0";
            selectedObjects[i].properties.font = this.jsObject.FontObjectToStr(font);
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["font"]);
    }

    downTable.addCell(italicButton).style.padding = "0 2px 0 2px";

    //Underline
    var underlineButton = this.StandartSmallButton("homePanelFontUnderline", null, null, "Underline.png", [this.loc.HelpDesigner.FontStyleUnderline, this.HelpLinks["font"]], null);

    underlineButton.action = function () {
        this.setSelected(!this.isSelected);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            var font = this.jsObject.FontStrToObject(selectedObjects[i].properties.font);
            font.underline = this.isSelected ? "1" : "0";
            selectedObjects[i].properties.font = this.jsObject.FontObjectToStr(font);
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["font"]);
    }

    downTable.addCell(underlineButton).style.padding = "0 2px 0 2px";

    //Strikeout
    var strikeoutButton = this.StandartSmallButton("homePanelFontStrikeout", null, null, "Strikeout.png", [this.loc.HelpDesigner.FontStyleStrikeout, this.HelpLinks["font"]], null);

    strikeoutButton.action = function () {
        this.setSelected(!this.isSelected);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            var font = this.jsObject.FontStrToObject(selectedObjects[i].properties.font);
            font.strikeout = this.isSelected ? "1" : "0";
            selectedObjects[i].properties.font = this.jsObject.FontObjectToStr(font);
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["font"]);
    }

    downTable.addCell(strikeoutButton).style.padding = "0 2px 0 2px";

    //Separator
    downTable.addCell(this.HomePanelSeparator());

    //Color
    var textColor = this.ColorControlWithImage("homePanelTextColor", "TextColor.png", [this.loc.HelpDesigner.TextColor, this.HelpLinks["font"]], true);

    textColor.action = function () {
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        var editTableElementForm = this.jsObject.options.forms.editTableElementForm;

        //dbs table element cells        
        if (selectedObjects && selectedObjects.length == 1 && selectedObjects[0].typeComponent == "StiTableElement" &&
            editTableElementForm && editTableElementForm.visible && editTableElementForm.controls.dataContainer.selectedItem) {
            editTableElementForm.setPropertyValue("ForeColor", this.key);
        }
        else {
            for (var i = 0; i < selectedObjects.length; i++) {
                if (selectedObjects[i].typeComponent == "StiBarCode" || selectedObjects[i].typeComponent == "StiZipCode" || selectedObjects[i].isDashboardElement)
                    selectedObjects[i].properties.foreColor = this.key
                else
                    selectedObjects[i].properties.textBrush = "1!" + this.key;
            }
            this.jsObject.SendCommandSendProperties(selectedObjects, ["textBrush", "foreColor"]);
        }
    }

    downTable.addCell(textColor).style.padding = "0 2px 0 2px";

    return fontGroupBlock;
}

//Alignment
StiMobileDesigner.prototype.HomePanelAlignmentBlock = function () {
    var alignmentGroupBlock = this.GroupBlock("groupBlockAlignment", this.loc.Toolbars.ToolbarAlignment, false, null);
    var innerTable = this.CreateHTMLTable();
    alignmentGroupBlock.container.appendChild(innerTable);
    innerTable.style.height = "100%";
    var upTable = this.CreateHTMLTable();
    var downTable = this.CreateHTMLTable();
    innerTable.addCell(upTable);
    innerTable.addCellInNextRow(downTable);

    //Top
    var alignTopButton = this.StandartSmallButton("homePanelAlignTop", "homePanelVerticalAlign", null, "AlignTop.png", [this.loc.HelpDesigner.AlignTop, this.HelpLinks["alignment"]], null);

    alignTopButton.action = function () {
        this.setSelected(true);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            selectedObjects[i].properties.vertAlignment = "Top";
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["vertAlignment"]);
    }

    upTable.addCell(alignTopButton).style.padding = "0 2px 0 2px";

    //Middle
    var alignMiddleButton = this.StandartSmallButton("homePanelAlignMiddle", "homePanelVerticalAlign", null, "AlignMiddle.png", [this.loc.HelpDesigner.AlignMiddle, this.HelpLinks["alignment"]], null);

    alignMiddleButton.action = function () {
        this.setSelected(true);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            selectedObjects[i].properties.vertAlignment = "Center";
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["vertAlignment"]);
    }

    upTable.addCell(alignMiddleButton).style.padding = "0 2px 0 2px";

    //Bottom
    var alignBottomButton = this.StandartSmallButton("homePanelAlignBottom", "homePanelVerticalAlign", null, "AlignBottom.png", [this.loc.HelpDesigner.AlignBottom, this.HelpLinks["alignment"]], null);

    alignBottomButton.action = function () {
        this.setSelected(true);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            selectedObjects[i].properties.vertAlignment = "Bottom";
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["vertAlignment"]);
    }

    upTable.addCell(alignBottomButton).style.padding = "0 2px 0 2px";

    //Angle
    var angleButton = this.StandartSmallButton("homePanelTextAngle", null, null, "RotateText.png", [this.loc.HelpDesigner.Angle, this.HelpLinks["alignment"]], "Down");

    var angleMenu = this.VerticalMenu("homePanelTextAngleMenu", angleButton, "Down", this.GetTextAngleItems())

    angleMenu.action = function (menuItem) {
        this.changeVisibleState(false);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            selectedObjects[i].properties.textAngle = menuItem.key;
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["textAngle"]);
    }

    angleMenu.onshow = function () {
        var currentObject = this.jsObject.options.selectedObject || this.jsObject.GetCommonObject(this.jsObject.options.selectedObjects);
        var angleValue = currentObject.properties.textAngle ? this.jsObject.StrToDouble(currentObject.properties.textAngle) : null;
        for (var itemName in this.items) {
            this.items[itemName].setSelected(angleValue != null && this.jsObject.StrToDouble(this.items[itemName].key) == angleValue);
        }
    }

    angleButton.action = function () {
        angleMenu.changeVisibleState(!angleMenu.visible);
    }

    upTable.addCell(angleButton).style.padding = "0 2px 0 2px";

    //WordWrap
    var wordWrapButton = this.StandartSmallButton("homePanelWordWrap", null, null, "WordWrap.png", [this.loc.PropertyMain.WordWrap, this.HelpLinks["alignment"]], null);

    wordWrapButton.action = function () {
        this.setSelected(!this.isSelected);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            selectedObjects[i].properties.wordWrap = this.isSelected;
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["wordWrap"]);
    }

    upTable.addCell(wordWrapButton).style.padding = "0 2px 0 2px";

    var jsObject = this;

    var applyHorAlignment = function (value) {
        var selectedObjects = jsObject.options.selectedObjects || [jsObject.options.selectedObject];
        var editTableElementForm = jsObject.options.forms.editTableElementForm;

        //dbs table element cells        
        if (selectedObjects && selectedObjects.length == 1 && selectedObjects[0].typeComponent == "StiTableElement" &&
            editTableElementForm && editTableElementForm.visible && editTableElementForm.controls.dataContainer.selectedItem) {
            editTableElementForm.setPropertyValue("HorAlignment", value);
        }
        else {
            for (var i = 0; i < selectedObjects.length; i++) {
                selectedObjects[i].properties.horAlignment = value;
            }
            jsObject.SendCommandSendProperties(selectedObjects, ["horAlignment"]);
        }
    }

    //Left
    var alignLeftButton = this.StandartSmallButton("homePanelAlignLeft", "homePanelHorizontalAlign", null, "AlignLeft.png", [this.loc.HelpDesigner.AlignLeft, this.HelpLinks["alignment"]], null);

    alignLeftButton.action = function () {
        this.setSelected(true);
        applyHorAlignment("Left");
    }

    downTable.addCell(alignLeftButton).style.padding = "0 2px 0 2px";

    //Center
    var alignCenterButton = this.StandartSmallButton("homePanelAlignCenter", "homePanelHorizontalAlign", null, "AlignCenter.png", [this.loc.HelpDesigner.AlignCenter, this.HelpLinks["alignment"]], null);

    alignCenterButton.action = function () {
        this.setSelected(true);
        applyHorAlignment("Center");
    }

    downTable.addCell(alignCenterButton).style.padding = "0 2px 0 2px";

    //Right
    var alignRightButton = this.StandartSmallButton("homePanelAlignRight", "homePanelHorizontalAlign", null, "AlignRight.png", [this.loc.HelpDesigner.AlignRight, this.HelpLinks["alignment"]], null);

    alignRightButton.action = function () {
        this.setSelected(true);
        applyHorAlignment("Right");
    }

    downTable.addCell(alignRightButton).style.padding = "0 2px 0 2px";

    //Width
    var alignWidthButton = this.StandartSmallButton("homePanelAlignWidth", "homePanelHorizontalAlign", null, "AlignWidth.png", [this.loc.HelpDesigner.AlignWidth, this.HelpLinks["alignment"]], null);

    alignWidthButton.action = function () {
        this.setSelected(true);
        applyHorAlignment("Width");
    }

    downTable.addCell(alignWidthButton).style.padding = "0 2px 0 2px";

    //LineSpacing
    var lineSpacingButton = this.StandartSmallButton("homePanelLineSpacing", null, null, "LineSpacing.png", [this.loc.HelpDesigner.LineSpacing, this.HelpLinks["alignment"]], "Down");

    var lineSpacingMenu = this.VerticalMenu("homePanelLineSpacingMenu", lineSpacingButton, "Down", this.GetLineSpacingItems())

    lineSpacingMenu.action = function (menuItem) {
        this.changeVisibleState(false);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            selectedObjects[i].properties.lineSpacing = menuItem.key;
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["lineSpacing"]);
    }

    lineSpacingMenu.onshow = function () {
        var currentObject = this.jsObject.options.selectedObject || this.jsObject.GetCommonObject(this.jsObject.options.selectedObjects);
        var lineSpacingValue = currentObject.properties.lineSpacing ? this.jsObject.StrToDouble(currentObject.properties.lineSpacing) : null;
        for (var itemName in this.items) {
            this.items[itemName].setSelected(lineSpacingValue != null && this.jsObject.StrToDouble(this.items[itemName].key) == lineSpacingValue);
        }
    }

    lineSpacingButton.action = function () {
        lineSpacingMenu.changeVisibleState(!lineSpacingMenu.visible);
    }

    downTable.addCell(lineSpacingButton).style.padding = "0 2px 0 2px";

    return alignmentGroupBlock;
}

//Borders
StiMobileDesigner.prototype.HomePanelBordersBlock = function () {
    var bordersGroupBlock = this.GroupBlock("groupBlockBorders", this.loc.Toolbars.ToolbarBorders, true, this.loc.Toolbars.ToolbarBorders);
    var innerTable = this.CreateHTMLTable();
    bordersGroupBlock.container.appendChild(innerTable);
    innerTable.style.height = "100%";
    var upTable = this.CreateHTMLTable();
    var downTable = this.CreateHTMLTable();
    innerTable.addCell(upTable);
    innerTable.addCellInNextRow(downTable);

    //BorderAll
    var borderAllButton = this.StandartSmallButton("homePanelBorderAll", "homePanelBorders", null, "BorderAll.png", [this.loc.HelpDesigner.BorderSidesAll, this.HelpLinks["border"]], null);

    borderAllButton.action = function () {
        var buttons = this.jsObject.options.buttons;
        this.setSelected(true);
        buttons.homePanelBorderLeft.setSelected(true);
        buttons.homePanelBorderTop.setSelected(true);
        buttons.homePanelBorderRight.setSelected(true);
        buttons.homePanelBorderBottom.setSelected(true);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            var border = this.jsObject.BordersStrToObject(selectedObjects[i].properties.border);
            border.left = "1";
            border.top = "1";
            border.right = "1";
            border.bottom = "1";
            selectedObjects[i].properties.border = this.jsObject.BordersObjectToStr(border);
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["border"]);
    }

    upTable.addCell(borderAllButton).style.padding = "0 2px 0 2px";

    //BorderNone
    var borderNoneButton = this.StandartSmallButton("homePanelBorderNone", "homePanelBorders", null, "BorderNone.png", [this.loc.HelpDesigner.BorderSidesNone, this.HelpLinks["border"]], null);

    borderNoneButton.action = function () {
        var buttons = this.jsObject.options.buttons;
        this.setSelected(true);
        buttons.homePanelBorderLeft.setSelected(false);
        buttons.homePanelBorderTop.setSelected(false);
        buttons.homePanelBorderRight.setSelected(false);
        buttons.homePanelBorderBottom.setSelected(false);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            var border = this.jsObject.BordersStrToObject(selectedObjects[i].properties.border);
            border.left = "0";
            border.top = "0";
            border.right = "0";
            border.bottom = "0";
            selectedObjects[i].properties.border = this.jsObject.BordersObjectToStr(border);
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["border"]);
    }

    upTable.addCell(borderNoneButton).style.padding = "0 2px 0 2px";

    //Separator
    upTable.addCell(this.HomePanelSeparator());

    //BorderLeft
    var borderLeftButton = this.StandartSmallButton("homePanelBorderLeft", null, null, "BorderLeft.png", [this.loc.HelpDesigner.BorderSidesLeft, this.HelpLinks["border"]], null);

    borderLeftButton.action = function () {
        this.setSelected(!this.isSelected);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            var border = this.jsObject.BordersStrToObject(selectedObjects[i].properties.border);
            border.left = this.isSelected ? "1" : "0";
            selectedObjects[i].properties.border = this.jsObject.BordersObjectToStr(border);
            switch (selectedObjects[i].properties.border.substring(0, 7)) {
                case "1,1,1,1": { this.jsObject.options.buttons.homePanelBorderAll.setSelected(true); break; }
                case "0,0,0,0": { this.jsObject.options.buttons.homePanelBorderNone.setSelected(true); break; }
                default: { this.jsObject.options.buttons.homePanelBorderAll.setSelected(false); this.jsObject.options.buttons.homePanelBorderNone.setSelected(false); break; }
            }
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["border"]);
    }

    upTable.addCell(borderLeftButton).style.padding = "0 2px 0 2px";

    //BorderTop
    var borderTopButton = this.StandartSmallButton("homePanelBorderTop", null, null, "BorderTop.png", [this.loc.HelpDesigner.BorderSidesTop, this.HelpLinks["border"]], null);

    borderTopButton.action = function () {
        this.setSelected(!this.isSelected);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            var border = this.jsObject.BordersStrToObject(selectedObjects[i].properties.border);
            border.top = this.isSelected ? "1" : "0";
            selectedObjects[i].properties.border = this.jsObject.BordersObjectToStr(border);
            switch (selectedObjects[i].properties.border.substring(0, 7)) {
                case "1,1,1,1": { this.jsObject.options.buttons.homePanelBorderAll.setSelected(true); break; }
                case "0,0,0,0": { this.jsObject.options.buttons.homePanelBorderNone.setSelected(true); break; }
                default: { this.jsObject.options.buttons.homePanelBorderAll.setSelected(false); this.jsObject.options.buttons.homePanelBorderNone.setSelected(false); break; }
            }
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["border"]);
    }

    upTable.addCell(borderTopButton).style.padding = "0 2px 0 2px";

    //BorderRight
    var borderRightButton = this.StandartSmallButton("homePanelBorderRight", null, null, "BorderRight.png", [this.loc.HelpDesigner.BorderSidesRight, this.HelpLinks["border"]], null);

    borderRightButton.action = function () {
        this.setSelected(!this.isSelected);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            var border = this.jsObject.BordersStrToObject(selectedObjects[i].properties.border);
            border.right = this.isSelected ? "1" : "0";
            selectedObjects[i].properties.border = this.jsObject.BordersObjectToStr(border);
            switch (selectedObjects[i].properties.border.substring(0, 7)) {
                case "1,1,1,1": { this.jsObject.options.buttons.homePanelBorderAll.setSelected(true); break; }
                case "0,0,0,0": { this.jsObject.options.buttons.homePanelBorderNone.setSelected(true); break; }
                default: { this.jsObject.options.buttons.homePanelBorderAll.setSelected(false); this.jsObject.options.buttons.homePanelBorderNone.setSelected(false); break; }
            }
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["border"]);
    }

    upTable.addCell(borderRightButton).style.padding = "0 2px 0 2px";

    //BorderBottom
    var borderBottomButton = this.StandartSmallButton("homePanelBorderBottom", null, null, "BorderBottom.png", [this.loc.HelpDesigner.BorderSidesBottom, this.HelpLinks["border"]], null);

    borderBottomButton.action = function () {
        this.setSelected(!this.isSelected);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            var border = this.jsObject.BordersStrToObject(selectedObjects[i].properties.border);
            border.bottom = this.isSelected ? "1" : "0";
            selectedObjects[i].properties.border = this.jsObject.BordersObjectToStr(border);
            switch (selectedObjects[i].properties.border.substring(0, 7)) {
                case "1,1,1,1": { this.jsObject.options.buttons.homePanelBorderAll.setSelected(true); break; }
                case "0,0,0,0": { this.jsObject.options.buttons.homePanelBorderNone.setSelected(true); break; }
                default: { this.jsObject.options.buttons.homePanelBorderAll.setSelected(false); this.jsObject.options.buttons.homePanelBorderNone.setSelected(false); break; }
            }
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["border"]);
    }

    upTable.addCell(borderBottomButton).style.padding = "0 2px 0 2px";

    //Shadow
    var shadowButton = this.StandartSmallButton("homePanelShadow", null, null, "Shadow.png", [this.loc.HelpDesigner.Shadow, this.HelpLinks["border"]], null);

    shadowButton.action = function () {
        this.setSelected(!this.isSelected);
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        var isDbsElement = false;
        for (var i = 0; i < selectedObjects.length; i++) {
            if (selectedObjects[i].properties.shadowVisible != null) {
                selectedObjects[i].properties.shadowVisible = this.isSelected;
                isDbsElement = true;
            }
            else {
                var border = this.jsObject.BordersStrToObject(selectedObjects[i].properties.border);
                border.dropShadow = this.isSelected ? "1" : "0";
                selectedObjects[i].properties.border = this.jsObject.BordersObjectToStr(border);
            }
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, [isDbsElement ? "shadowVisible" : "border"]);
    }
    downTable.addCell(shadowButton).style.padding = "0 2px 0 2px";

    //Separator
    downTable.addCell(this.HomePanelSeparator());

    //BackgroundColor
    var backgroundColor = this.ColorControlWithImage("homePanelBackgroundColor", "BackgroundColor.png", [this.loc.HelpDesigner.Background, this.HelpLinks["border"]]);

    backgroundColor.action = function () {
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            if (selectedObjects[i].isDashboard ||
                selectedObjects[i].isDashboardElement ||
                selectedObjects[i].typeComponent == "StiBarCode" ||
                selectedObjects[i].typeComponent == "StiRichText") {
                selectedObjects[i].properties.backColor = this.key;
            }
            else {
                selectedObjects[i].properties.brush = "1!" + this.key;
            }
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["brush", "backColor"]);
    }

    downTable.addCell(backgroundColor).style.padding = "0 2px 0 2px";

    //BorderColor
    var borderColor = this.ColorControlWithImage("homePanelBorderColor", "BorderColor.png", [this.loc.HelpDesigner.BorderColor, this.HelpLinks["border"]]);

    borderColor.action = function () {
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            if (selectedObjects[i].typeComponent == "StiShape")
                selectedObjects[i].properties.shapeBorderColor = this.key;
            else {
                var border = this.jsObject.BordersStrToObject(selectedObjects[i].properties.border);
                border.color = this.key;
                selectedObjects[i].properties.border = this.jsObject.BordersObjectToStr(border);
            }
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["border", "shapeBorderColor"]);
    }

    downTable.addCell(borderColor).style.padding = "0 2px 0 2px";

    //BorderStyle
    var borderStyle = this.ImageList("homePanelBorderStyle", false, true,
        [this.loc.HelpDesigner.BorderStyle, this.HelpLinks["border"]], this.GetBorderStyleItems());
    borderStyle.setKey("0");
    borderStyle.action = function () {
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        for (var i = 0; i < selectedObjects.length; i++) {
            if (selectedObjects[i].typeComponent == "StiShape")
                selectedObjects[i].properties.shapeBorderStyle = this.key;
            else {
                var border = this.jsObject.BordersStrToObject(selectedObjects[i].properties.border);
                border.style = this.key;
                selectedObjects[i].properties.border = this.jsObject.BordersObjectToStr(border);
            }
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, ["border", "shapeBorderStyle"]);
    }
    downTable.addCell(borderStyle).style.padding = "0 2px 0 2px";

    return bordersGroupBlock;
}

//Text Format
StiMobileDesigner.prototype.HomePanelTextFormatBlock = function () {
    var textFormatGroupBlock = this.GroupBlock("groupBlockTextFormat", this.loc.Toolbars.ToolbarTextFormat, true, null);
    var innerTable = this.GroupBlockInnerTable();
    textFormatGroupBlock.container.appendChild(innerTable);
    innerTable.style.height = "100%";
    innerTable.style.width = "100%";

    textFormatGroupBlock.button.action = function () {
        this.jsObject.InitializeTextFormatForm(function (textFormatForm) {
            var textFormat = null;

            //dbs table element cells
            var selectedObjects = textFormatForm.jsObject.options.selectedObjects || [textFormatForm.jsObject.options.selectedObject];
            if (selectedObjects && selectedObjects.length == 1) {
                if (selectedObjects[0].typeComponent == "StiTableElement") {
                    var editTableElementForm = textFormatForm.jsObject.options.forms.editTableElementForm;
                    if (editTableElementForm && editTableElementForm.visible && editTableElementForm.controls.dataContainer.selectedItem) {
                        textFormat = editTableElementForm.controls.dataContainer.selectedItem.itemObject.textFormat;
                    }

                } else if (selectedObjects[0].typeComponent == "StiPivotTableElement") {
                    var editPivotTableElementForm = textFormatForm.jsObject.options.forms.editPivotTableElementForm;
                    if (editPivotTableElementForm && editPivotTableElementForm.visible && editPivotTableElementForm.getSelectedItem())
                        textFormat = editPivotTableElementForm.getSelectedItem().itemObject.textFormat;
                }
            }

            textFormatForm.show(textFormat);
        });
    }

    //Format Button
    var formatTextControl = this.TextFormatControl("homePanelTextFormat");
    formatTextControl.setKey("StiGeneralFormatService");
    formatTextControl.action = function () {
        var selectedObjects = this.jsObject.options.selectedObjects || [this.jsObject.options.selectedObject];
        var editTableElementForm = this.jsObject.options.forms.editTableElementForm;
        var editPivotTableElementForm = this.jsObject.options.forms.editPivotTableElementForm;

        //dbs table element cells
        if (selectedObjects && selectedObjects.length == 1 && selectedObjects[0].typeComponent == "StiTableElement" &&
            editTableElementForm && editTableElementForm.visible && editTableElementForm.controls.dataContainer.selectedItem) {
            editTableElementForm.setPropertyValue("TextFormat", this.jsObject.options.textFormats[this.key]);
        } else if (selectedObjects && selectedObjects.length == 1 && selectedObjects[0].typeComponent == "StiPivotTableElement" &&
            editPivotTableElementForm && editPivotTableElementForm.visible && editPivotTableElementForm.getSelectedItem()) {
            editPivotTableElementForm.setPropertyValue("TextFormat", this.jsObject.options.textFormats[this.key]);
        }
        else {
            for (var i = 0; i < selectedObjects.length; i++) {
                selectedObjects[i].properties.textFormat = this.jsObject.options.textFormats[this.key];
            }
            this.jsObject.SendCommandSendProperties(selectedObjects, ["textFormat"]);
        }
    }
    innerTable.addCell(formatTextControl).style.padding = "2px";

    return textFormatGroupBlock;
}

//Style
StiMobileDesigner.prototype.HomePanelStyleBlock = function () {
    var jsObject = this;
    var styleGroupBlock = this.GroupBlock("groupBlockStyle", this.loc.Toolbars.ToolbarStyle, false, null);
    var innerTable = this.GroupBlockInnerTable();
    styleGroupBlock.container.appendChild(innerTable);
    innerTable.style.height = "100%";
    innerTable.style.width = "100%";

    //Conditions Button    
    var conditionsButton = this.StandartBigButton("conditionsButton", null, this.loc.PropertyMain.Conditions, "BigConditions.png", [this.loc.HelpDesigner.biConditions, this.HelpLinks["conditions"]], 100, 55);
    innerTable.addCell(conditionsButton).style.padding = "2px";
    conditionsButton.action = function () {
        var options = this.jsObject.options;
        var selectedObject = options.selectedObject || (options.selectedObjects && options.selectedObjects.length > 0 ? options.selectedObjects[0] : null);
        if (!selectedObject) return;

        if (selectedObject.typeComponent == "StiChartElement" && selectedObject.properties.chartConditions) {
            this.jsObject.InitializeChartConditionsForm(function (chartConditionsForm) {
                chartConditionsForm.show();
            });
        }
        else if (selectedObject.typeComponent == "StiPivotTableElement" && selectedObject.properties.pivotTableConditions) {
            this.jsObject.InitializePivotTableConditionsForm(function (pivotTableConditionsForm) {
                pivotTableConditionsForm.show();
            });
        }
        else if (selectedObject.typeComponent == "StiIndicatorElement" && selectedObject.properties.indicatorConditions) {
            this.jsObject.InitializeIndicatorConditionsForm(function (indicatorConditionsForm) {
                indicatorConditionsForm.show();
            });
        }
        else if (selectedObject.typeComponent == "StiProgressElement" && selectedObject.properties.progressConditions) {
            this.jsObject.InitializeProgressConditionsForm(function (progressConditionsForm) {
                progressConditionsForm.show();
            });
        }
        else if (selectedObject.typeComponent == "StiTableElement" && selectedObject.properties.tableConditions) {
            this.jsObject.InitializeTableConditionsForm(function (tableConditionsForm) {
                tableConditionsForm.show();
            });
        }
        else {
            this.jsObject.InitializeConditionsForm(function (conditionsForm) {
                conditionsForm.show();
            });
        }
    }

    //Interactions Button
    var interactionsButton = this.StandartBigButton("homePanelInteractionsButton", null, this.loc.PropertyMain.Interaction, "BigInteraction.png", [this.loc.HelpDesigner.Interaction, this.HelpLinks["interactions"]], 100, 55);
    innerTable.addCell(interactionsButton).style.padding = "2px";
    interactionsButton.action = function () {
        var options = this.jsObject.options;
        var selectedObject = options.selectedObject || (options.selectedObjects && options.selectedObjects.length > 0 ? options.selectedObjects[0] : null);
        if (!selectedObject) return;

        if (selectedObject.properties.dashboardInteraction) {
            this.jsObject.InitializeDashboardInteractionForm(function (form) {
                form.show(selectedObject.properties.dashboardInteraction, null, selectedObject.properties.isRange);

                form.action = function () {
                    form.changeVisibleState(false);
                    this.jsObject.ApplyPropertyValue(["dashboardInteraction"], [form.getResultInteraction()]);
                }
            });
        }
        else {
            this.jsObject.InitializeInteractionForm(function (interactionForm) {
                interactionForm.show();
            });
        }
    }

    //CopyStyle  Button    
    var copyStyleButton = this.StandartBigButton("copyStyleButton", null, this.loc.Toolbox.Style, "BigCopyStyle.png", [this.loc.Toolbox.Style, this.HelpLinks["default"]], 100, 55);

    innerTable.addCell(copyStyleButton).style.padding = "2px";
    copyStyleButton.action = function () {
        this.jsObject.options.paintPanel.setCopyStyleMode(!this.jsObject.options.paintPanel.copyStyleMode);
    }

    //Style Designer Button    
    var styleDesignerButton = this.StandartBigButton("styleDesignerButton", null, this.loc.Toolbars.StyleDesigner, "Styles.png", [this.loc.HelpDesigner.StyleDesigner, this.HelpLinks["styleDesigner"]], 100, 55);
    innerTable.addCell(styleDesignerButton).style.padding = "2px";
    styleDesignerButton.action = function () {
        this.jsObject.InitializeStyleDesignerForm(function (styleDesignerForm) {
            styleDesignerForm.show();
        });
    }

    //Styles Button
    var styleButton = this.StylesControl("homePanelStyle");
    styleButton.setKey("[None]");

    styleButton.action = function () {
        var commonSelectedObject = this.jsObject.options.selectedObject || this.jsObject.GetCommonObject(this.jsObject.options.selectedObjects);
        var selectedObjects = this.jsObject.options.selectedObject ? [this.jsObject.options.selectedObject] : this.jsObject.options.selectedObjects;
        var properties = [];

        for (var i = 0; i < selectedObjects.length; i++) {
            if (commonSelectedObject.typeComponent == "StiChart") {
                selectedObjects[i].properties.chartStyle = this.key;
                properties.push("chartStyle");
            }
            else if (commonSelectedObject.typeComponent == "StiGauge") {
                selectedObjects[i].properties.gaugeStyle = this.key;
                properties.push("gaugeStyle");
            }
            else if (commonSelectedObject.typeComponent == "StiMap") {
                selectedObjects[i].properties.mapStyle = this.key;
                properties.push("mapStyle");
            }
            else if (commonSelectedObject.typeComponent == "StiCrossTab") {
                selectedObjects[i].properties.crossTabStyle = this.key;
                properties.push("crossTabStyle");
            }
            else if (commonSelectedObject.typeComponent == "StiTable") {
                selectedObjects[i].properties.styleId = this.key.styleId || "";
                selectedObjects[i].properties.componentStyle = this.key.styleName || "[None]";

                this.jsObject.SendCommandChangeTableComponent({
                    command: "applyStyle",
                    styleId: selectedObjects[i].properties.styleId,
                    styleName: selectedObjects[i].properties.componentStyle
                });
                return;
            }
            else if (commonSelectedObject.isDashboard) {
                if ((this.key.ident != "Custom" && selectedObjects[i].properties.elementStyle == this.key.ident) ||
                    (this.key.ident == "Custom" && selectedObjects[i].properties.customStyleName == this.key.name)) return;

                selectedObjects[i].properties.elementStyle = this.key.ident;
                this.jsObject.SendCommandChangeDashboardStyle(selectedObjects[i].properties.name, this.key.ident);

                return;
            }
            else if (commonSelectedObject.isDashboardElement) {
                selectedObjects[i].properties.elementStyle = this.key.ident;
                selectedObjects[i].properties.customStyleName = this.key.name || "";
                properties.push("elementStyle");
                properties.push("customStyleName");
            }
            else {
                selectedObjects[i].properties.componentStyle = this.key;
                properties.push("componentStyle");
            }
        }
        this.jsObject.SendCommandSendProperties(selectedObjects, properties, true);
    }

    innerTable.addCell(styleButton).style.padding = "2px";

    return styleGroupBlock;
}

//Separator
StiMobileDesigner.prototype.HomePanelSeparator = function () {
    var separator = document.createElement("div");
    separator.style.width = "1px";
    separator.style.height = this.options.isTouchDevice ? "28px" : "23px";
    separator.className = "stiDesignerHomePanelSeparator";

    return separator;
}

