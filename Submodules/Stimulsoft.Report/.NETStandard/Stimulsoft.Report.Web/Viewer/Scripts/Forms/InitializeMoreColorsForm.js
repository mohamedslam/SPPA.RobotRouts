
StiJsViewer.prototype.InitializeMoreColorsForm = function () {
    var jsObject = this;
    var moreColorsForm = this.BaseForm("moreColors", this.collections.loc["ColorsCategory"], 4);
    moreColorsForm.container.style.paddingTop = "6px";
    moreColorsForm.controls = {};

    var formWidth = 320;
    var formHeight = 180;

    //Override methods
    moreColorsForm.onmousedown = function () { if (jsObject.options.isTouchDevice) return; this.ontouchstart(); }
    moreColorsForm.ontouchstart = function () { jsObject.options.colorDialogPressed = this; }

    var tabs = [];
    tabs.push({ "name": "custom", "caption": this.collections.loc["Custom"] });
    tabs.push({ "name": "webColors", "caption": this.collections.loc["Web"] });

    var tabbedPane = this.TabbedPane("moreColorsTabbedPane", tabs);
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
        tabbedPane.showTabPanel("custom");
        this.webColorsButtons[0].action();

        var colorDialog = jsObject.controls.menus.colorDialog || jsObject.InitializeColorDialog();
        var key = colorDialog.parentButton.colorControl.key;

        if (key == null) key = "255,255,255,255";
        if (key == "transparent") key = "0,255,255,255";

        var colors = key.split(",");

        if (colors.length == 4) {
            moreColorsForm.controls.alfaCanal.value = colors[0];
            colors.splice(0, 1);
        }
        else
            moreColorsForm.controls.alfaCanal.value = "255";

        moreColorsForm.controls.colorFormRedColor.value = colors[0];
        moreColorsForm.controls.colorFormGreenColor.value = colors[1];
        moreColorsForm.controls.colorFormBlueColor.value = colors[2];

        this.updateColorBar();
        this.updateHex();
    }

    moreColorsForm.updateColorBar = function () {
        moreColorsForm.controls.colorFormColorBar.style.opacity = jsObject.StrToInt(moreColorsForm.controls.alfaCanal.value) / 255;
        moreColorsForm.controls.colorFormColorBar.style.background = "rgb(" + moreColorsForm.controls.colorFormRedColor.value + "," + moreColorsForm.controls.colorFormGreenColor.value + "," + moreColorsForm.controls.colorFormBlueColor.value + ")";
    }

    moreColorsForm.action = function () {
        this.changeVisibleState(false);
        var key = "transparent";

        if (tabbedPane.selectedTab.panelName == "custom") {
            if (moreColorsForm.controls.alfaCanal.value != "0") {
                key = moreColorsForm.controls.colorFormRedColor.value + "," + moreColorsForm.controls.colorFormGreenColor.value + "," + moreColorsForm.controls.colorFormBlueColor.value;
                if (moreColorsForm.controls.alfaCanal.value != "255") key = moreColorsForm.controls.alfaCanal.value + "," + key;
            }
        }
        else {
            key = this.selectedWebColorsButton.key;
        }

        var colorDialog = jsObject.controls.menus.colorDialog || jsObject.InitializeColorDialog();
        colorDialog.parentButton.choosedColor(key);
    }

    return moreColorsForm;
}

//Custom Tab
StiJsViewer.prototype.ColorFormCustomTable = function (moreColorsForm) {
    var jsObject = this;
    var customTable = this.CreateHTMLTable();
    customTable.style.width = "100%";
    customTable.style.height = "100%";

    var controlsTable = this.CreateHTMLTable();
    customTable.addCell(controlsTable).style.width = "1px";
    controlsTable.className = "stiJsViewerColorFormControlsTable stiJsViewerClearAllStyles";
    controlsTable.style.height = "100%";

    //Alfa Canal
    controlsTable.addRow();
    controlsTable.addTextCellInLastRow("Alpha").className = "stiJsViewerCaptionControls";

    var alfaCanalControl = moreColorsForm.controls.alfaCanal = this.TextBox("colorFormAlfaCanal", 80);
    controlsTable.addCellInLastRow(alfaCanalControl);

    alfaCanalControl.action = function () {
        this.value = jsObject.StrToCorrectByte(this.value);
        moreColorsForm.updateColorBar();
    };

    //Red
    controlsTable.addRow();
    controlsTable.addTextCellInLastRow(this.collection.loc["RedColor"]).className = "stiJsViewerCaptionControls";

    var textRColor = moreColorsForm.controls.colorFormRedColor = this.ColorTextBox("colorFormRedColor", 80);
    controlsTable.addCellInLastRow(textRColor);

    //Green
    controlsTable.addRow();
    controlsTable.addTextCellInLastRow(this.collection.loc["GreenColor"]).className = "stiJsViewerCaptionControls";

    var textGColor = moreColorsForm.controls.colorFormGreenColor = this.ColorTextBox("colorFormGreenColor", 80);
    controlsTable.addCellInLastRow(textGColor);

    //Blue
    controlsTable.addRow();
    controlsTable.addCellInLastRow(this.collection.loc["BlueColor"]).className = "stiJsViewerCaptionControls";

    var textBColor = moreColorsForm.controls.colorFormBlueColor = this.ColorTextBox("colorFormBlueColor", 80);
    controlsTable.addCellInLastRow(textBColor);

    //Hex
    controlsTable.addRow();
    controlsTable.addTextCellInLastRow("Hex").className = "stiJsViewerCaptionControls";

    var textHexColor = moreColorsForm.controls.colorFormHexColor = this.TextBox("colorFormHexColor", 80);
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
    colorBarContainer.className = "stiJsViewerColorFormColorBar";
    colorBarContainer.style.marginLeft = "14px";
    StiJsViewer.setImageSource(colorBarContainer, this.options, this.collections, "ColorControl.NoFill.png");
    customTable.addCell(colorBarContainer).style.textAlign = "center";

    var colorBar = moreColorsForm.controls.colorFormColorBar = document.createElement("div");
    colorBar.className = "stiJsViewerColorFormColorBar";
    colorBar.style.border = "0px";
    colorBarContainer.appendChild(colorBar);

    return customTable;
}

//Custom Tab
StiJsViewer.prototype.ColorFormWebColorPanel = function (moreColorsForm) {
    var webColorPanel = document.createElement("div");
    webColorPanel.className = "stiJsViewerColorFormWebColorPanel";
    moreColorsForm.webColorsButtons = [];
    moreColorsForm.selectedWebColorsButton = null;

    for (var i = 0; i < this.constWebColors.length; i++) {
        var webColorButton = this.SmallButton(null, this.constWebColors[i][0], true);
        webColorButton.style.margin = "1px";
        moreColorsForm.webColorsButtons.push(webColorButton);

        webColorButton.action = function () {
            for (var k = 0; k < moreColorsForm.webColorsButtons.length; k++) {
                moreColorsForm.webColorsButtons[k].setSelected(false);
            }
            this.setSelected(true);
            moreColorsForm.selectedWebColorsButton = this;
        }

        //Override image
        var newImageParent = document.createElement("div");
        newImageParent.className = "stiJsViewerColorControlImage";
        newImageParent.style.height = "28px";

        var newImage = document.createElement("div");
        newImage.style.height = "100%";
        newImageParent.appendChild(newImage);

        var imageCell = webColorButton.image.parentElement;
        imageCell.removeChild(webColorButton.image);
        imageCell.appendChild(newImageParent);
        webColorButton.image = newImage;

        webColorButton.image.style.background = (this.constWebColors[i][1] != "transparent") ? "rgb(" + this.constWebColors[i][1] + ")" : "transparent";
        webColorButton.key = this.constWebColors[i][1];
        webColorPanel.appendChild(webColorButton);
    }

    return webColorPanel;
}

StiJsViewer.prototype.ColorTextBox = function (name, width) {
    var jsObject = this;
    var textBox = this.TextBox(name, width);

    textBox.action = function () {
        this.value = jsObject.StrToCorrectByte(this.value);

        var moreColorsForm = jsObject.controls.forms.moreColors || jsObject.InitializeMoreColorsForm();
        moreColorsForm.updateColorBar();
    };

    return textBox;
}