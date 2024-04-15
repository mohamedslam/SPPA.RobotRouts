﻿
StiMobileDesigner.prototype.InitializeColorDialog = function () {
    var jsObject = this;
    var colorDialog = this.BaseMenu("colorDialog", null, "Down");
    colorDialog.style.zIndex = "100";
    colorDialog.type = "ColorDialog";

    //Override methods
    colorDialog.onmousedown = function () {
        if (this.isTouchStartFlag) return;
        this.ontouchstart(true);
    }

    colorDialog.ontouchstart = function (mouseProcess) {
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        jsObject.options.colorDialogPressed = this;

        this.isTouchStartTimer = setTimeout(function () {
            colorDialog.isTouchStartFlag = false;
        }, 1000);
    }

    //Colors Table
    var header1 = document.createElement("div");
    colorDialog.innerContent.appendChild(header1);
    header1.innerHTML = this.loc.Gui.colorpicker_themecolorslabel;
    header1.className = "stiColorDialogHeader";

    var colorTable;
    for (var i = 0; i < 70; i++) {
        if (i == 60) {
            var header2 = document.createElement("div");
            colorDialog.innerContent.appendChild(header2);
            header2.innerHTML = this.loc.Gui.colorpicker_standardcolorslabel;
            header2.className = "stiColorDialogHeader";
        }
        if (i % 10 == 0) {
            colorTable = this.CreateHTMLTable();
            colorDialog.innerContent.appendChild(colorTable);
        }
        var colorButton = this.ColorDialogButton("Number" + i, this.ConstColorsArray[i][1]);
        colorButton.colorDialog = colorDialog;
        var colorCell = colorTable.addCell(colorButton);
        colorButton.action = function () { this.colorDialog.action(this.key); }
        if (i < 10) colorCell.style.padding = "2px 2px 6px 2px";
        if (i >= 10 && i < 60) colorCell.style.padding = "0 2px 0 2px";
        if (i >= 60) colorCell.style.padding = "2px";
    }

    //Separator
    colorDialog.innerContent.appendChild(this.FormSeparator());

    var noFillButton = this.StandartSmallButton("noFillButton", null, this.loc.Gui.colorpicker_nofill.replace("&", ""), "NoFill.png", null, null);
    noFillButton.style.margin = "1px 0 1px 0";
    colorDialog.innerContent.appendChild(noFillButton);
    colorDialog.noFillButton = noFillButton;

    noFillButton.action = function () {
        colorDialog.action("0,255,255,255");
    }

    //Separator2
    colorDialog.innerContent.appendChild(this.FormSeparator());

    //More Colors Button
    var moreColorsButton = this.StandartSmallButton("moreColorsButton", null,
        this.loc.Gui.colorpicker_morecolors.replace("&", ""), "MoreColors.png", null, null);
    moreColorsButton.style.marginTop = "1px";
    colorDialog.innerContent.appendChild(moreColorsButton);

    moreColorsButton.action = function () {
        colorDialog.changeVisibleState(false);
        jsObject.InitializeMoreColorsForm(function (moreColorsForm) {
            moreColorsForm.changeVisibleState(true);
        });
    }

    //Expression
    var expPanel = document.createElement("div");
    expPanel.style.display = "none";
    colorDialog.expressionPanel = expPanel;
    colorDialog.innerContent.appendChild(expPanel);

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
    var expButton = expPanel.expressionButton = this.StandartSmallButton(null, this.loc.FormRichTextEditor.Insert);
    expPanel.appendChild(expButton);

    colorDialog.showExpression = function (expression) {
        expPanel.style.display = "";
        expButton.caption.innerHTML = expression != null ? jsObject.loc.Dashboard.EditExpression : jsObject.loc.FormRichTextEditor.Insert;
        expHeader.style.display = expContainer.style.display = sep2.style.display = expression != null ? "" : "none";
        sep1.style.display = expression != null ? "none" : "";
        if (expression != null) expContainer.caption.innerHTML = expression;
    };

    colorDialog.hideExpression = function () {
        expPanel.style.display = "none";
    };

    colorDialog.onhide = function () {
        colorDialog.hideExpression();
    }

    colorDialog.action = function (key) {
        this.changeVisibleState(false);
        this.parentButton.choosedColor(key);
    }

    return colorDialog;
}

StiMobileDesigner.prototype.ColorDialogButton = function (name, key) {
    var button = document.createElement("div");
    var jsObject = button.jsObject = this;
    button.id = this.options.mobileDesigner.id + "ColorButton" + name;
    button.name = name;
    button.key = key;
    button.style.width = button.style.height = this.options.isTouchDevice ? "22px" : "13px";
    button.className = "stiColorDialogButton";
    button.style.background = (key != "transparent") ? "rgb(" + key + ")" : "transparent";

    button.onmouseover = function () {
        if (!jsObject.options.isTouchDevice) this.onmouseenter();
    }

    button.onmouseenter = function () {
        if (jsObject.options.isTouchClick) return;
        this.className = "stiColorDialogButtonOver";
    }

    button.onmouseleave = function () {
        this.className = "stiColorDialogButton";
    }

    button.onclick = function () {
        if (this.isTouchEndFlag || jsObject.options.isTouchClick) return;
        this.action();
    }

    button.ontouchend = function () {
        if (jsObject.options.fingerIsMoved) return;
        this.isTouchEndFlag = true;
        this.className = "stiColorDialogButtonOver";

        clearTimeout(this.isTouchEndTimer);
        jsObject.options.buttonsTimer = [this, this.className, setTimeout(function () {
            jsObject.options.buttonsTimer = null;
            button.className = "stiColorDialogButton";
            button.action();
        }, 150)];

        this.isTouchEndTimer = setTimeout(function () {
            button.isTouchEndFlag = false;
        }, 1000);
    }

    button.ontouchstart = function () {
        jsObject.options.fingerIsMoved = false;
    }

    button.action = function () { }

    return button;
}

StiMobileDesigner.prototype.ConstColorsArray = [
    ["White", "255,255,255"],
    ["Black", "0,0,0"],
    ["Gray", "231,230,230"],
    ["BlueGray", "68,84,106"],
    ["Blue", "91,155,213"],
    ["Orange", "237,125,49"],
    ["GrayDark", "165,165,165"],
    ["Gold", "255,192,0"],
    ["BlueDark", "68,114,196"],
    ["Green", "112,173,71"],

    ["White", "242,242,242"],
    ["Black", "127,127,127"],
    ["Gray", "208,206,206"],
    ["BlueGray", "214,220,228"],
    ["Blue", "222,235,246"],
    ["Orange", "251,229,213"],
    ["GrayDark", "237,237,237"],
    ["Gold", "255,242,204"],
    ["BlueDark", "217,226,243"],
    ["Green", "226,239,217"],

    ["White", "216,216,216"],
    ["Black", "89,89,89"],
    ["Gray", "174,171,171"],
    ["BlueGray", "173,185,202"],
    ["Blue", "189,215,238"],
    ["Orange", "247,203,172"],
    ["GrayDark", "219,219,219"],
    ["Gold", "254,229,153"],
    ["BlueDark", "180,198,231"],
    ["Green", "197,224,179"],

    ["White", "191,191,191"],
    ["Black", "63,63,63"],
    ["Gray", "117,112,112"],
    ["BlueGray", "132,150,176"],
    ["Blue", "156,195,229"],
    ["Orange", "244,177,131"],
    ["GrayDark", "201,201,201"],
    ["Gold", "255,217,101"],
    ["BlueDark", "142,170,219"],
    ["Green", "168,208,141"],

    ["White", "165,165,165"],
    ["Black", "38,38,38"],
    ["Gray", "58,56,56"],
    ["BlueGray", "50,63,79"],
    ["Blue", "46,117,181"],
    ["Orange", "197,90,17"],
    ["GrayDark", "123,123,123"],
    ["Gold", "191,144,0"],
    ["BlueDark", "47,84,150"],
    ["Green", "83,129,53"],

    ["White", "127,127,127"],
    ["Black", "12,12,12"],
    ["Gray", "23,22,22"],
    ["BlueGray", "34,42,53"],
    ["Blue", "30,78,121"],
    ["Orange", "131,60,11"],
    ["GrayDark", "82,82,82"],
    ["Gold", "127,96,0"],
    ["BlueDark", "31,56,100"],
    ["Green", "55,86,35"],

    ["White", "192,0,0"],
    ["Black", "255,0,0"],
    ["Gray", "255,192,0"],
    ["BlueGray", "255,255,0"],
    ["Blue", "146,208,80"],
    ["Orange", "0,176,80"],
    ["GrayDark", "0,176,240"],
    ["Gold", "0,112,192"],
    ["BlueDark", "0,32,96"],
    ["Green", "112,48,160"]
]