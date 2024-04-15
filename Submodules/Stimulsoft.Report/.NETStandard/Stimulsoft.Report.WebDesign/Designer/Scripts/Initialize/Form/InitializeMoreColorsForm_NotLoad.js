
StiMobileDesigner.prototype.InitializeMoreColorsForm_ = function () {
    var jsObject = this;
    var moreColorsForm = this.BaseForm("moreColors", this.loc.PropertyCategory.ColorsCategory, 4);
    moreColorsForm.container.style.paddingTop = "6px";

    var formWidth = 320;
    var formHeight = 180;

    //Override methods
    moreColorsForm.onmousedown = function () { if (jsObject.options.isTouchDevice) return; this.ontouchstart(); }
    moreColorsForm.ontouchstart = function () { jsObject.options.colorDialogPressed = this; }

    var tabs = [];
    tabs.push({ "name": "custom", "caption": this.loc.FormColorBoxPopup.Custom });
    tabs.push({ "name": "webColors", "caption": this.loc.FormColorBoxPopup.Web });

    var tabbedPane = this.TabbedPane("moreColorsTabbedPane", tabs, "stiDesignerStandartTab");
    tabbedPane.style.margin = "0 12px 12px 12px";
    moreColorsForm.container.appendChild(tabbedPane);

    //Custom
    tabbedPane.tabsPanels.custom.appendChild(this.ColorFormCustomTable(moreColorsForm));
    tabbedPane.tabsPanels.custom.style.width = formWidth + "px";
    tabbedPane.tabsPanels.custom.style.height = formHeight + "px";

    //Web Colors
    var webColorPanel = this.ColorFormWebColorPanel(moreColorsForm);
    webColorPanel.style.height = (formHeight + 10) + "px";

    tabbedPane.tabsPanels.webColors.appendChild(webColorPanel);
    tabbedPane.tabsPanels.webColors.style.width = formWidth + "px";
    tabbedPane.tabsPanels.webColors.style.height = formHeight + "px";

    moreColorsForm.onshow = function () {
        var options = jsObject.options;
        options.controls.moreColorsTabbedPane.showTabPanel("custom");
        this.webColorsButtons[0].action();
        var colorDialog = options.menus.colorDialog || jsObject.InitializeColorDialog();
        var key = colorDialog.parentButton.colorControl.key;
        if (key == null) key = "255,255,255,255";
        if (key == "transparent") key = "0,255,255,255";
        var colors = key.split(",");

        if (colors.length == 4) {
            options.controls.colorFormAlfaCanal.value = colors[0];
            colors.splice(0, 1);
        }
        else
            options.controls.colorFormAlfaCanal.value = "255";

        options.controls.colorFormRedColor.value = colors[0];
        options.controls.colorFormGreenColor.value = colors[1];
        options.controls.colorFormBlueColor.value = colors[2];
        this.updateColorBar();
        this.updateHex();

        var isDashboard = jsObject.options.report && jsObject.options.currentPage && jsObject.options.currentPage.isDashboard;
        this.webColorsButtons[0].caption.innerText = isDashboard ? jsObject.loc.FormStyleDesigner.FromStyle : jsObject.loc.PropertyColor.Transparent;
    }

    moreColorsForm.updateColorBar = function () {
        var options = jsObject.options;
        options.controls.colorFormColorBar.style.opacity = jsObject.StrToInt(options.controls.colorFormAlfaCanal.value) / 255;
        options.controls.colorFormColorBar.style.background = "rgb(" + options.controls.colorFormRedColor.value + "," +
            options.controls.colorFormGreenColor.value + "," + options.controls.colorFormBlueColor.value + ")";
    }

    moreColorsForm.action = function () {
        this.changeVisibleState(false);
        var options = jsObject.options;
        var key = "transparent";
        if (options.controls.moreColorsTabbedPane.selectedTab.panelName == "custom") {
            if (options.controls.colorFormAlfaCanal.value != "0") {
                key = options.controls.colorFormRedColor.value + "," + options.controls.colorFormGreenColor.value + "," + options.controls.colorFormBlueColor.value;
                if (options.controls.colorFormAlfaCanal.value != "255") key = options.controls.colorFormAlfaCanal.value + "," + key;
            }
        }
        else {
            key = this.selectedWebColorsButton.key;
        }

        var colorDialog = jsObject.options.menus.colorDialog || jsObject.InitializeColorDialog();
        colorDialog.parentButton.choosedColor(key);
    }

    return moreColorsForm;
}

//Custom Tab
StiMobileDesigner.prototype.ColorFormCustomTable = function (moreColorsForm) {
    var jsObject = this;
    var customTable = this.CreateHTMLTable();
    customTable.style.width = "100%";
    customTable.style.height = "100%";

    var controlsTable = this.CreateHTMLTable();
    customTable.addCell(controlsTable).style.width = "1px";
    controlsTable.className = "stiColorFormControlsTable stiDesignerClearAllStyles";
    controlsTable.style.height = "100%";

    //Alfa Canal
    controlsTable.addRow();
    var cellAlfaCanal = controlsTable.addCellInLastRow();
    cellAlfaCanal.className = "stiDesignerCaptionControls";
    cellAlfaCanal.innerHTML = "Alpha";
    var alfaCanalControl = this.TextBox("colorFormAlfaCanal", 80);
    controlsTable.addCellInLastRow(alfaCanalControl);

    alfaCanalControl.action = function () {
        this.value = this.jsObject.StrToCorrectByte(this.value);
        this.jsObject.InitializeMoreColorsForm(function (moreColorsForm) {
            moreColorsForm.updateColorBar();
        });
    };

    //Red
    controlsTable.addRow();
    var cellRColor = controlsTable.addCellInLastRow();
    cellRColor.className = "stiDesignerCaptionControls";
    cellRColor.innerHTML = this.loc.PropertyColor.Red;
    var textRColor = this.ColorTextBox("colorFormRedColor", 80);
    controlsTable.addCellInLastRow(textRColor);

    //Green
    controlsTable.addRow();
    var cellGColor = controlsTable.addCellInLastRow();
    cellGColor.className = "stiDesignerCaptionControls";
    cellGColor.innerHTML = this.loc.PropertyColor.Green;
    var textGColor = this.ColorTextBox("colorFormGreenColor", 80);
    controlsTable.addCellInLastRow(textGColor);

    //Blue
    controlsTable.addRow();
    var cellBColor = controlsTable.addCellInLastRow();
    cellBColor.className = "stiDesignerCaptionControls";
    cellBColor.innerHTML = this.loc.PropertyColor.Blue;
    var textBColor = this.ColorTextBox("colorFormBlueColor", 80);
    controlsTable.addCellInLastRow(textBColor);

    //Hex
    controlsTable.addRow();
    var cellHexColor = controlsTable.addCellInLastRow();
    cellHexColor.className = "stiDesignerCaptionControls";
    cellHexColor.innerHTML = "Hex";
    var textHexColor = this.TextBox("colorFormHexColor", 80);
    controlsTable.addCellInLastRow(textHexColor);

    moreColorsForm.updateHex = function () {
        textHexColor.value = "#" + ((alfaCanalControl.value != "255" ? jsObject.DecToHex(parseInt(alfaCanalControl.value)) : "") +
            jsObject.DecToHex(parseInt(textRColor.value)) + jsObject.DecToHex(parseInt(textGColor.value)) + jsObject.DecToHex(parseInt(textBColor.value))).toUpperCase();
    }

    moreColorsForm.updateRGB = function () {
        if (textHexColor.value.indexOf("#") != 0) {
            textHexColor.value = "#" + textHexColor.value;
        }
        var rgb = jsObject.HexToRgb(textHexColor.value);
        textRColor.value = rgb ? rgb.r : 0;
        textGColor.value = rgb ? rgb.g : 0;
        textBColor.value = rgb ? rgb.b : 0;
        alfaCanalControl.value = rgb && rgb.a ? rgb.a : 255;
    }

    textRColor.onchange = textGColor.onchange = textBColor.onchange = alfaCanalControl.onchange = function () {
        moreColorsForm.updateHex();
    }

    textHexColor.onchange = function () {
        moreColorsForm.updateRGB();
        moreColorsForm.updateColorBar();
    }

    //Color Bar
    var colorBarContainer = document.createElement("div");
    colorBarContainer.className = "stiColorFormColorBar";
    colorBarContainer.style.marginLeft = "14px";
    StiMobileDesigner.setImageSource(colorBarContainer, this.options, "ColorBarBackground.png");
    customTable.addCell(colorBarContainer).style.textAlign = "center";

    var colorBar = document.createElement("div");
    colorBar.className = "stiColorFormColorBar";
    colorBar.style.border = "0px";
    this.options.controls.colorFormColorBar = colorBar;
    colorBarContainer.appendChild(colorBar);

    return customTable;
}

//Custom Tab
StiMobileDesigner.prototype.ColorFormWebColorPanel = function (moreColorsForm) {
    var webColorPanel = document.createElement("div");
    webColorPanel.className = "stiColorFormWebColorPanel";
    moreColorsForm.webColorsButtons = [];
    moreColorsForm.selectedWebColorsButton = null;

    for (var i = 0; i < this.ConstWebColors.length; i++) {
        var webColorButton = this.StandartSmallButton("webColorPanel" + this.ConstWebColors[i][0], "ColorFormWebColors", this.ConstWebColors[i][0], true, null, null);
        webColorButton.style.margin = "1px";
        moreColorsForm.webColorsButtons.push(webColorButton);

        webColorButton.action = function () {
            this.setSelected(true);
            var this_ = this;
            this.jsObject.InitializeMoreColorsForm(function (moreColorsForm) {
                moreColorsForm.selectedWebColorsButton = this_;
            });
        }

        //Override image
        var newImageParent = document.createElement("div");
        newImageParent.className = "stiColorControlImage";
        newImageParent.style.height = (this.options.controlsHeight - 8) + "px";

        var newImage = document.createElement("div");
        newImage.style.height = "100%";
        newImageParent.appendChild(newImage);

        var imageCell = webColorButton.image.parentElement;
        imageCell.removeChild(webColorButton.image);
        imageCell.appendChild(newImageParent);
        webColorButton.image = newImage;

        webColorButton.image.style.background = (this.ConstWebColors[i][1] != "transparent") ? "rgb(" + this.ConstWebColors[i][1] + ")" : "transparent";
        webColorButton.key = this.ConstWebColors[i][1];
        webColorPanel.appendChild(webColorButton);
    }

    return webColorPanel;
}

StiMobileDesigner.prototype.ColorTextBox = function (name, width) {
    var textBox = this.TextBox(name, width);

    textBox.action = function () {
        this.value = this.jsObject.StrToCorrectByte(this.value);
        this.jsObject.InitializeMoreColorsForm(function (moreColorsForm) {
            moreColorsForm.updateColorBar();
        });
    };

    return textBox;
}