
StiMobileDesigner.prototype.BrushMenu = function (name, parentButton) {
    var jsObject = this;
    var menu = this.BaseMenu(name, parentButton, "Down");
    menu.innerContent.style.padding = "2px";
    menu.selectedHeaderButton = null;

    //Default State & Style
    menu.defStateButton = this.StandartSmallButton(null, null, this.loc.Report.FromDefaultState);
    menu.defStateButton.style.display = "none";
    menu.innerContent.appendChild(menu.defStateButton);

    menu.fromStyleButton = this.StandartSmallButton(null, null, this.loc.FormStyleDesigner.FromStyle);
    menu.fromStyleButton.style.display = "none";
    menu.innerContent.appendChild(menu.fromStyleButton);

    menu.defStyleSeparator = this.FormSeparator();
    menu.defStyleSeparator.style.display = "none";
    menu.innerContent.appendChild(menu.defStyleSeparator);

    menu.headerButtons = [];
    menu.headerButtons.push(this.BrushHeaderButton(menu, this.loc.Report.StiEmptyBrush, "BrushEmpty.png", null, 0));
    menu.headerButtons.push(this.BrushHeaderButton(menu, this.loc.Report.StiSolidBrush, "BrushSolid.png", null, 1));
    menu.headerButtons.push(this.BrushHeaderButton(menu, this.loc.Report.StiHatchBrush, "BrushHatch.png", null, 2));
    menu.headerButtons.push(this.BrushHeaderButton(menu, this.loc.Report.StiGradientBrush, "BrushGradient.png", null, 3));
    menu.headerButtons.push(this.BrushHeaderButton(menu, this.loc.Report.StiGlareBrush, "BrushGlare.png", null, 4));
    menu.headerButtons.push(this.BrushHeaderButton(menu, this.loc.Report.StiGlassBrush, "BrushGlass.png", null, 5));

    var headerButtonsTable = this.CreateHTMLTable();
    headerButtonsTable.cellSpacing = 3;
    menu.innerContent.appendChild(headerButtonsTable);

    for (var i = 0; i < menu.headerButtons.length; i++) {
        if (i < 3)
            headerButtonsTable.addCell(menu.headerButtons[i]);
        else {
            if (i == 3) headerButtonsTable.addRow();
            headerButtonsTable.addCellInLastRow(menu.headerButtons[i]);
        }
    }

    var middleSep = this.Separator();
    menu.innerContent.appendChild(middleSep);

    //Panels
    menu.panels = [];
    for (var i = 0; i < menu.headerButtons.length; i++) {
        menu.panels[i] = document.createElement("div");
        menu.panels[i].key = i;
        menu.panels[i].jsObject = this;
        menu.panels[i].style.display = "none";
        menu.panels[i].style.padding = "4px 0 4px 0";
        menu.innerContent.appendChild(menu.panels[i]);
        menu.panels[i].parentMenu = menu;

        menu.panels[i].show = function () {
            for (var i = 0; i < this.parentMenu.panels.length; i++) {
                if (this.parentMenu.panels[i] == this) {
                    this.parentMenu.headerButtons[i].setSelected(true);
                    this.parentMenu.selectedHeaderButton = this.parentMenu.headerButtons[i];
                    this.parentMenu.panels[i].style.display = "";
                }
                else
                    this.parentMenu.panels[i].style.display = "none";
            }
        }
    }

    //Add Panels
    menu.panels[1].appendChild(this.SolidBrushTable(menu));
    menu.panels[2].appendChild(this.HatchBrushTable(menu));
    menu.panels[3].appendChild(this.GradientBrushTable(menu));
    menu.panels[4].appendChild(this.GlareBrushTable(menu));
    menu.panels[5].appendChild(this.GlassBrushTable(menu));

    menu.updateControls = function () {
        var brushStr = this.parentButton.key;
        if (!brushStr) return;

        var controls = jsObject.options.controls;
        var brushArray = brushStr.split("!");

        if (brushStr == "StiEmptyValue" || brushStr == "none" || brushStr == "isStyleBrush" || brushStr == "isDefaultBrush" || (this.brushControl && this.brushControl.expression)) {
            menu.panels[0].show();
            menu.headerButtons[0].setSelected(false);
            middleSep.style.display = "none";
            brushArray = ["1", "transparent"];
        }
        else {
            menu.panels[brushArray[0]].show();
            middleSep.style.display = brushArray[0] != "0" ? "" : "none";
        }

        var solidBrushArray = this.getSolidBrush(brushArray);
        var hatchBrushArray = this.getHatchBrush(brushArray);
        var gradientBrushArray = this.getGradientBrush(brushArray);
        var glareBrushArray = this.getGlareBrush(brushArray);
        var glassBrushArray = this.getGlassBrush(brushArray);

        controls[this.name + "SolidColor"].setKey(solidBrushArray[1]);
        controls[this.name + "HatchForeColor"].setKey(hatchBrushArray[1]);
        controls[this.name + "HatchBackColor"].setKey(hatchBrushArray[2]);
        controls[this.name + "HatchStyle"].setKey(hatchBrushArray[3]);
        controls[this.name + "GradientStartColor"].setKey(gradientBrushArray[1]);
        controls[this.name + "GradientEndColor"].setKey(gradientBrushArray[2]);
        controls[this.name + "GradientAngle"].value = gradientBrushArray[3];
        controls[this.name + "GlareStartColor"].setKey(glareBrushArray[1]);
        controls[this.name + "GlareEndColor"].setKey(glareBrushArray[2]);
        controls[this.name + "GlareAngle"].value = glareBrushArray[3];
        controls[this.name + "GlareFocus"].value = glareBrushArray[4];
        controls[this.name + "GlareScale"].value = glareBrushArray[5];
        controls[this.name + "GlassColor"].setKey(glassBrushArray[1]);
        controls[this.name + "GlassBlend"].value = glassBrushArray[2];
        controls[this.name + "GlassDrawHatch"].setChecked(glassBrushArray[3] == "1");

        menu.headerButtons[1].image.setBrush(solidBrushArray);
        menu.headerButtons[2].image.setBrush(hatchBrushArray);
        menu.headerButtons[3].image.setBrush(gradientBrushArray);
        menu.headerButtons[4].image.setBrush(glareBrushArray);
        menu.headerButtons[5].image.setBrush(glassBrushArray);
    }

    menu.getSolidBrush = function (brushArray) {
        switch (brushArray[0]) {
            case "0": return ["1", "255,255,255"];
            case "1": return brushArray;
            case "2":
            case "3":
            case "4": return ["1", jsObject.getMixingColors(brushArray[1], brushArray[2])];
            case "5": return ["1", brushArray[1]];
        }
    }

    menu.getHatchBrush = function (brushArray) {
        switch (brushArray[0]) {
            case "0": return ["2", "128,128,128", "255,255,255", "3"];
            case "1": return ["2", menu.getForeColor(brushArray[1]), menu.getBackColor(brushArray[1]), "3"];
            case "2": return brushArray;
            case "3":
            case "4": return ["2", brushArray[1], brushArray[2], "3"];
            case "5": return ["2", menu.getForeColor(brushArray[1]), menu.getBackColor(brushArray[1]), "3"];
        }
    }

    menu.getGradientBrush = function (brushArray) {
        switch (brushArray[0]) {
            case "0": return ["3", "128,128,128", "255,255,255", "0"];
            case "1": return ["3", menu.getForeColor(brushArray[1]), menu.getBackColor(brushArray[1]), "0"];
            case "2":
            case "4": return ["3", brushArray[1], brushArray[2], "0"];
            case "3": return brushArray;
            case "5": return ["3", menu.getForeColor(brushArray[1]), menu.getBackColor(brushArray[1]), "0"];
        }
    }

    menu.getGlareBrush = function (brushArray) {
        switch (brushArray[0]) {
            case "0": return ["4", "128,128,128", "255,255,255", "0", "0.5", "1"];
            case "1": return ["4", menu.getForeColor(brushArray[1]), menu.getBackColor(brushArray[1]), "0", "0.5", "1"];
            case "2":
            case "3": return ["4", brushArray[1], brushArray[2], "0", "0.5", "1"];
            case "4": return brushArray;
            case "5": return ["4", menu.getForeColor(brushArray[1]), menu.getBackColor(brushArray[1]), "0", "0.5", "1"];
        }
    }

    menu.getGlassBrush = function (brushArray) {
        switch (brushArray[0]) {
            case "0": return ["5", "128,128,128", "0.2", "1"];
            case "1":
            case "2":
            case "3":
            case "4": return ["5", brushArray[1], "0.2", "1"];
            case "5": return brushArray;
        }
    }

    menu.getForeColor = function (color) {
        return (jsObject.isItTooLight(color) ? jsObject.getDarkColor(color, 100) : jsObject.getLightColor(color, 50));
    }

    menu.getBackColor = function (color) {
        return (jsObject.isItTooDark(color) ? jsObject.getLightColor(color, 100) : jsObject.getDarkColor(color, 50));
    }

    menu.onshow = function () {
        this.updateControls();
    }

    menu.action = function () { };

    menu.onchanged = function () { };

    menu.applyKey = function () {
        var controls = jsObject.options.controls;
        this.parentButton.key = "0";

        switch (this.selectedHeaderButton.key) {
            case 1: {
                this.parentButton.key = "1!" + controls[this.name + "SolidColor"].key;
                break;
            }
            case 2: {
                this.parentButton.key = "2!" + controls[this.name + "HatchForeColor"].key + "!" + controls[this.name + "HatchBackColor"].key + "!" + controls[this.name + "HatchStyle"].key;
                break;
            }
            case 3: {
                this.parentButton.key = "3!" + controls[this.name + "GradientStartColor"].key + "!" + controls[this.name + "GradientEndColor"].key + "!" + controls[this.name + "GradientAngle"].value;
                break;
            }
            case 4: {
                this.parentButton.key = "4!" + controls[this.name + "GlareStartColor"].key + "!" + controls[this.name + "GlareEndColor"].key + "!" + controls[this.name + "GlareAngle"].value +
                    "!" + controls[this.name + "GlareFocus"].value + "!" + controls[this.name + "GlareScale"].value;
                break;
            }
            case 5: {
                this.parentButton.key = "5!" + controls[this.name + "GlassColor"].key + "!" + controls[this.name + "GlassBlend"].value + "!" + (controls[this.name + "GlassDrawHatch"].isChecked ? "1" : "0");
                break;
            }
        }
        this.action();
        this.onchanged();
        this.updateControls();
    }

    //Expression
    var expPanel = document.createElement("div");
    expPanel.style.display = "none";
    menu.expressionPanel = expPanel;
    menu.innerContent.appendChild(expPanel);

    var sep1 = this.FormSeparator();
    expPanel.appendChild(sep1);
    var expHeader = this.FormBlockHeader(this.loc.PropertyMain.Expression);
    expPanel.appendChild(expHeader);

    var expContainer = this.StandartSmallButton(null, " ");
    expContainer.style.maxWidth = "300px";
    expContainer.onmouseout = expContainer.onmouseover = expContainer.onmouseenter = expContainer.onmouseleave = null;
    expContainer.action = function () { };
    expPanel.appendChild(expContainer);

    var sep2 = this.FormSeparator();
    expPanel.appendChild(sep2);
    expPanel.expressionButton = this.StandartSmallButton(null, this.loc.FormRichTextEditor.Insert);
    expPanel.appendChild(expPanel.expressionButton);

    menu.showExpression = function (expression) {
        expPanel.style.display = "";
        expPanel.expressionButton.caption.innerHTML = expression != null ? jsObject.loc.Dashboard.EditExpression : jsObject.loc.FormRichTextEditor.Insert;
        expHeader.style.display = expContainer.style.display = sep2.style.display = expression != null ? "" : "none";
        sep1.style.display = expression != null ? "none" : "";
        if (expression != null) expContainer.caption.innerHTML = expression;
    };

    menu.hideExpression = function () {
        expPanel.style.display = "none";
    };

    menu.onhide = function () {
        menu.hideExpression();
        menu.onchanged = function () { };
    }

    return menu;
}

StiMobileDesigner.prototype.BrushHeaderButton = function (menu, caption, imageName, toolTip, key) {
    var button = this.StandartSmallButton(null, null, caption, imageName, toolTip, null, null, { width: 32, height: 16 });
    button.key = key;
    button.menu = menu;

    button.imageCell.removeChild(button.image);
    button.image = this.BrushHeaderSvgImage();
    button.imageCell.appendChild(button.image);

    button.action = function () {
        menu.panels[this.key].show();
        menu.applyKey();
    }

    button.setSelected = function (state) {
        if (state && menu && menu.headerButtons) {
            for (var i = 0; i < menu.headerButtons.length; i++) {
                menu.headerButtons[i].setSelected(false);
            }
        }
        this.isSelected = state;
        this.className = this.isEnabled ? (state ? this.selectedClass : (this.isOver ? this.overClass : this.defaultClass)) : this.disabledClass;
    }

    return button;
}

StiMobileDesigner.prototype.BrushHeaderSvgImage = function () {
    var jsObject = this;
    var svgImg = this.CreateSvgElement("svg");
    svgImg.setAttribute("height", 14);
    svgImg.setAttribute("width", 30);
    svgImg.style.border = "1px solid #ababab";
    svgImg.style.borderRadius = this.allowRoundedControls() ? "3px" : "0";

    svgImg.addGradientToBrushHeaderSvgImage = function () {
        this.gradient = jsObject.AddGradientBrushToElement(this);
    }

    svgImg.addGlassToBrushHeaderSvgImage = function () {
        var rect1 = this.glassRect1 = jsObject.CreateSvgElement("rect");
        rect1.setAttribute("x", "0");
        rect1.setAttribute("y", "0");
        rect1.setAttribute("width", 30);
        rect1.setAttribute("height", 7);
        this.appendChild(rect1);
        var rect2 = this.glassRect2 = jsObject.CreateSvgElement("rect");
        rect2.setAttribute("x", "0");
        rect2.setAttribute("y", 7);
        rect2.setAttribute("width", 30);
        rect2.setAttribute("height", 7);
        this.appendChild(rect2);
    }

    svgImg.setBrush = function (brushArray) {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        this.style.background = "rgb(255,255,255)";

        if (brushArray) {
            switch (brushArray[0]) {
                case "0": {
                    svgImg.style.background = "rgb(255,255,255)";
                    break;
                }
                case "1": {
                    svgImg.style.background = jsObject.GetHTMLColor(brushArray[1]);
                    break;
                }
                case "2": {
                    this.appendChild(jsObject.GetSvgHatchBrush(brushArray, 0, 0, 30, 14));
                    break;
                }
                case "3":
                case "4": {
                    this.addGradientToBrushHeaderSvgImage();
                    this.gradient.applyBrush(brushArray);
                    break;
                }
                case "5": {
                    svgImg.addGlassToBrushHeaderSvgImage();
                    svgImg.glassRect1.style.fill = svgImg.glassRect2.style.fill = jsObject.GetHTMLColor(brushArray[1]); //TO DO Glass Brush
                    break;
                }
            }
        }
    }

    return svgImg;
}

//Solid
StiMobileDesigner.prototype.SolidBrushTable = function (menu) {
    var table = this.CreateHTMLTable();
    var solidColorText = table.addCell();
    solidColorText.className = "stiDesignerCaptionControls";
    solidColorText.innerHTML = this.loc.PropertyMain.Color;

    var solidColor = this.ColorControl(menu.name + "SolidColor", null, null, null, true);
    solidColor.parentMenu = menu;
    solidColor.action = function () { this.parentMenu.applyKey(); }
    var solidColorCell = table.addCell(solidColor)
    solidColorCell.className = "stiDesignerControlCells";

    return table;
}

//Hatch
StiMobileDesigner.prototype.HatchBrushTable = function (menu) {
    var table = this.CreateHTMLTable();

    //Fore Color
    var hatchForeColorText = table.addCell();
    hatchForeColorText.className = "stiDesignerCaptionControls";
    hatchForeColorText.innerHTML = this.loc.PropertyMain.ForeColor;

    var hatchForeColor = this.ColorControl(menu.name + "HatchForeColor", null, null, null, true);
    hatchForeColor.parentMenu = menu;
    hatchForeColor.action = function () { this.parentMenu.applyKey(); }
    var hatchForeColorCell = table.addCell(hatchForeColor)
    hatchForeColorCell.className = "stiDesignerControlCells";

    //Back Color
    table.addRow();

    var hatchBackColorText = table.addCellInLastRow();
    hatchBackColorText.className = "stiDesignerCaptionControls";
    hatchBackColorText.innerHTML = this.loc.PropertyMain.BackColor;

    var hatchBackColor = this.ColorControl(menu.name + "HatchBackColor", null, null, null, true);
    hatchBackColor.parentMenu = menu;
    hatchBackColor.action = function () { this.parentMenu.applyKey(); }
    var hatchBackColorCell = table.addCellInLastRow(hatchBackColor);
    hatchBackColorCell.className = "stiDesignerControlCells";

    //Style
    table.addRow();

    var hatchStyleText = table.addCellInLastRow();
    hatchStyleText.className = "stiDesignerCaptionControls";
    hatchStyleText.innerHTML = this.loc.PropertyMain.Style;

    var items = [];
    if (this.loc) {
        var hatchStyles = [
            [this.loc.PropertyHatchStyle.BackwardDiagonal, "3"],
            [this.loc.PropertyHatchStyle.LargeGrid, "4"],
            [this.loc.PropertyHatchStyle.DarkDownwardDiagonal, "20"],
            [this.loc.PropertyHatchStyle.DarkHorizontal, "29"],
            [this.loc.PropertyHatchStyle.DarkUpwardDiagonal, "21"],
            [this.loc.PropertyHatchStyle.DarkVertical, "28"],
            [this.loc.PropertyHatchStyle.DashedDownwardDiagonal, "30"],
            [this.loc.PropertyHatchStyle.DashedHorizontal, "32"],
            [this.loc.PropertyHatchStyle.DashedUpwardDiagonal, "31"],
            [this.loc.PropertyHatchStyle.DashedVertical, "33"],
            [this.loc.PropertyHatchStyle.DiagonalBrick, "38"],
            [this.loc.PropertyHatchStyle.DiagonalCross, "5"],
            [this.loc.PropertyHatchStyle.Divot, "42"],
            [this.loc.PropertyHatchStyle.DottedDiamond, "44"],
            [this.loc.PropertyHatchStyle.DottedGrid, "43"],
            [this.loc.PropertyHatchStyle.ForwardDiagonal, "2"],
            [this.loc.PropertyHatchStyle.Horizontal, "0"],
            [this.loc.PropertyHatchStyle.HorizontalBrick, "39"],
            [this.loc.PropertyHatchStyle.LargeCheckerBoard, "50"],
            [this.loc.PropertyHatchStyle.LargeConfetti, "35"],
            [this.loc.PropertyHatchStyle.LightDownwardDiagonal, "18"],
            [this.loc.PropertyHatchStyle.LightHorizontal, "25"],
            [this.loc.PropertyHatchStyle.LightUpwardDiagonal, "19"],
            [this.loc.PropertyHatchStyle.LightVertical, "24"],
            [this.loc.PropertyHatchStyle.NarrowHorizontal, "27"],
            [this.loc.PropertyHatchStyle.NarrowVertical, "26"],
            [this.loc.PropertyHatchStyle.OutlinedDiamond, "51"],
            [this.loc.PropertyHatchStyle.Percent05, "6"],
            [this.loc.PropertyHatchStyle.Percent10, "7"],
            [this.loc.PropertyHatchStyle.Percent20, "8"],
            [this.loc.PropertyHatchStyle.Percent25, "9"],
            [this.loc.PropertyHatchStyle.Percent30, "10"],
            [this.loc.PropertyHatchStyle.Percent40, "11"],
            [this.loc.PropertyHatchStyle.Percent50, "12"],
            [this.loc.PropertyHatchStyle.Percent60, "13"],
            [this.loc.PropertyHatchStyle.Percent70, "14"],
            [this.loc.PropertyHatchStyle.Percent75, "15"],
            [this.loc.PropertyHatchStyle.Percent80, "16"],
            [this.loc.PropertyHatchStyle.Percent90, "17"],
            [this.loc.PropertyHatchStyle.Plaid, "41"],
            [this.loc.PropertyHatchStyle.Shingle, "45"],
            [this.loc.PropertyHatchStyle.SmallCheckerBoard, "49"],
            [this.loc.PropertyHatchStyle.SmallConfetti, "34"],
            [this.loc.PropertyHatchStyle.SmallGrid, "48"],
            [this.loc.PropertyHatchStyle.SolidDiamond, "52"],
            [this.loc.PropertyHatchStyle.Sphere, "47"],
            [this.loc.PropertyHatchStyle.Trellis, "46"],
            [this.loc.PropertyHatchStyle.Vertical, "1"],
            [this.loc.PropertyHatchStyle.Weave, "40"],
            [this.loc.PropertyHatchStyle.WideDownwardDiagonal, "22"],
            [this.loc.PropertyHatchStyle.WideUpwardDiagonal, "23"],
            [this.loc.PropertyHatchStyle.ZigZag, "36"],
        ]
        for (var i = 0; i < hatchStyles.length; i++) {
            items.push(this.Item("HatchStyleItem" + hatchStyles[i][1], hatchStyles[i][0], null, hatchStyles[i][1]));
        }
    }
    else {
        for (var i = 0; i < this.options.hatchStyles.length; i++) {
            items.push(this.Item("HatchStyleItem" + this.options.hatchStyles[i].key, this.options.hatchStyles[i].value, null, this.options.hatchStyles[i].key));
        }
    }

    var hatchStyle = this.DropDownList(menu.name + "HatchStyle", 150, null, items, true, false, null, true);
    hatchStyle.parentMenu = menu;
    if (hatchStyle.menu.innerContent) hatchStyle.menu.innerContent.style.maxHeight = "230px";
    hatchStyle.action = function () { this.parentMenu.applyKey(); }
    var hatchStyleCell = table.addCellInLastRow(hatchStyle);
    hatchStyleCell.className = "stiDesignerControlCells";

    return table;
}

//Gradient
StiMobileDesigner.prototype.GradientBrushTable = function (menu) {

    var table = this.CreateHTMLTable();

    //Start Color
    var gradientStartColorText = table.addCell();
    gradientStartColorText.className = "stiDesignerCaptionControls";
    gradientStartColorText.innerHTML = this.loc.PropertyMain.StartColor;

    var gradientStartColor = this.ColorControl(menu.name + "GradientStartColor", null, null, null, true);
    gradientStartColor.parentMenu = menu;
    gradientStartColor.action = function () { this.parentMenu.applyKey(); }
    var gradientStartColorCell = table.addCell(gradientStartColor)
    gradientStartColorCell.className = "stiDesignerControlCells";

    //End Color
    table.addRow();

    var gradientEndColorText = table.addCellInLastRow();
    gradientEndColorText.className = "stiDesignerCaptionControls";
    gradientEndColorText.innerHTML = this.loc.PropertyMain.EndColor;

    var gradientEndColor = this.ColorControl(menu.name + "GradientEndColor", null, null, null, true);
    gradientEndColor.parentMenu = menu;
    gradientEndColor.action = function () { this.parentMenu.applyKey(); }
    var gradientEndColorCell = table.addCellInLastRow(gradientEndColor);
    gradientEndColorCell.className = "stiDesignerControlCells";

    //Angle
    table.addRow();

    var gradientAngleText = table.addCellInLastRow();
    gradientAngleText.className = "stiDesignerCaptionControls";
    gradientAngleText.innerHTML = this.loc.PropertyMain.Angle;

    var gradientAngle = this.TextBox(menu.name + "GradientAngle", 50);
    gradientAngle.parentMenu = menu;
    gradientAngle.action = function () {
        this.value = this.jsObject.StrToDouble(this.value);
        this.parentMenu.applyKey();
    }
    var gradientAngleCell = table.addCellInLastRow(gradientAngle);
    gradientAngleCell.className = "stiDesignerControlCells";

    return table;
}

//Glare
StiMobileDesigner.prototype.GlareBrushTable = function (menu) {
    var jsObject = this;
    var table = this.CreateHTMLTable();

    //Start Color
    var glareStartColorText = table.addCell();
    glareStartColorText.className = "stiDesignerCaptionControls";
    glareStartColorText.innerHTML = this.loc.PropertyMain.StartColor;

    var glareStartColor = this.ColorControl(menu.name + "GlareStartColor", null, null, null, true);
    glareStartColor.parentMenu = menu;
    glareStartColor.action = function () { this.parentMenu.applyKey(); }
    var glareStartColorCell = table.addCell(glareStartColor)
    glareStartColorCell.className = "stiDesignerControlCells";

    //End Color
    table.addRow();

    var glareEndColorText = table.addCellInLastRow();
    glareEndColorText.className = "stiDesignerCaptionControls";
    glareEndColorText.innerHTML = this.loc.PropertyMain.EndColor;

    var glareEndColor = this.ColorControl(menu.name + "GlareEndColor", null, null, null, true);
    glareEndColor.parentMenu = menu;
    glareEndColor.action = function () { this.parentMenu.applyKey(); }
    var glareEndColorCell = table.addCellInLastRow(glareEndColor);
    glareEndColorCell.className = "stiDesignerControlCells";

    //Angle
    table.addRow();

    var glareAngleText = table.addCellInLastRow();
    glareAngleText.className = "stiDesignerCaptionControls";
    glareAngleText.innerHTML = this.loc.PropertyMain.Angle;

    var glareAngle = this.TextBox(menu.name + "GlareAngle", 50);
    glareAngle.parentMenu = menu;
    glareAngle.action = function () {
        this.value = jsObject.StrToDouble(this.value);
        this.parentMenu.applyKey();
    }
    var glareAngleCell = table.addCellInLastRow(glareAngle);
    glareAngleCell.className = "stiDesignerControlCells";

    //Focus
    table.addRow();

    var glareFocusText = table.addCellInLastRow();
    glareFocusText.className = "stiDesignerCaptionControls";
    glareFocusText.innerHTML = this.loc.PropertyMain.Focus;

    var glareFocus = this.TextBox(menu.name + "GlareFocus", 50);
    glareFocus.parentMenu = menu;
    glareFocus.action = function () {
        var value = jsObject.StrToDouble(this.value);
        if (value > 1 || value < 0) {
            (jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm()).show("Focus must be in range between 0 and 1!", "Warning");
            this.value = "0.5";
            return;
        }
        this.value = value;
        this.parentMenu.applyKey();
    }
    var glareFocusCell = table.addCellInLastRow(glareFocus);
    glareFocusCell.className = "stiDesignerControlCells";

    //Scale
    table.addRow();

    var glareScaleText = table.addCellInLastRow();
    glareScaleText.className = "stiDesignerCaptionControls";
    glareScaleText.innerHTML = this.loc.PropertyMain.Scale;

    var glareScale = this.TextBox(menu.name + "GlareScale", 50);
    glareScale.parentMenu = menu;
    glareScale.action = function () {
        var value = jsObject.StrToDouble(this.value);
        if (value > 1 || value < 0) {
            (jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm()).show("Scale must be in range between 0 and 1!", "Warning");
            this.value = "1";
            return;
        }
        this.value = value;
        this.parentMenu.applyKey();
    }
    var glareScaleCell = table.addCellInLastRow(glareScale);
    glareScaleCell.className = "stiDesignerControlCells";

    return table;
}

//Glass
StiMobileDesigner.prototype.GlassBrushTable = function (menu) {
    var jsObject = this;
    var table = this.CreateHTMLTable();

    //Color
    var glassColorText = table.addCell();
    glassColorText.className = "stiDesignerCaptionControls";
    glassColorText.innerHTML = this.loc.PropertyMain.Color;

    var glassColor = this.ColorControl(menu.name + "GlassColor", null, null, null, true);
    glassColor.parentMenu = menu;
    glassColor.action = function () { this.parentMenu.applyKey(); }
    var glassColorCell = table.addCell(glassColor)
    glassColorCell.className = "stiDesignerControlCells";

    //Blend
    table.addRow();

    var glassBlendText = table.addCellInLastRow();
    glassBlendText.className = "stiDesignerCaptionControls";
    glassBlendText.innerHTML = this.loc.PropertyMain.Blend;

    var glassBlend = this.TextBox(menu.name + "GlassBlend", 50);
    glassBlend.parentMenu = menu;
    glassBlend.action = function () {
        var value = jsObject.StrToDouble(this.value);
        if (value > 1 || value < 0) {
            (jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm()).show("Blend must be in range between 0 and 1!", "Warning");
            this.value = "1";
            return;
        }
        this.value = value;
        this.parentMenu.applyKey();
    }
    var glassBlendCell = table.addCellInLastRow(glassBlend);
    glassBlendCell.className = "stiDesignerControlCells";

    //Draw Hatch
    table.addRow();

    var drawHatchCell = table.addCellInLastRow();
    drawHatchCell.className = "stiDesignerCaptionControls";
    drawHatchCell.style.paddingTop = "5px";
    drawHatchCell.style.paddingBottom = "5px";
    drawHatchCell.setAttribute("colspan", "2");

    var drawHatch = this.CheckBox(menu.name + "GlassDrawHatch", this.loc.PropertyMain.DrawHatch);
    drawHatchCell.appendChild(drawHatch);
    drawHatch.parentMenu = menu;
    drawHatch.action = function () {
        this.parentMenu.applyKey();
    }

    return table;
}