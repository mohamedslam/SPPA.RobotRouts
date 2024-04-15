
StiMobileDesigner.prototype.InitializePageSetupForm_ = function () {
    var jsObject = this;
    var pageSetupForm = this.BaseForm("pageSetup", this.loc.Toolbars.ToolbarPageSetup, 1, this.HelpLinks["watermark"]);
    pageSetupForm.mode = "Paper";
    pageSetupForm.width = this.options.isTouchDevice ? 570 : 535;
    pageSetupForm.height = this.options.isTouchDevice ? 450 : 420;

    //Main Table
    var mainTable = this.CreateHTMLTable();
    mainTable.className = "stiDesignerImageFormMainPanel";
    pageSetupForm.container.appendChild(mainTable);
    pageSetupForm.container.style.padding = "0px";

    //Buttons
    var buttonProps = [
        ["Paper", "PageOptions.PageOptionPaper.png", this.loc.PropertyMain.Paper],
        ["Columns", "PageOptions.PageOptionColumns.png", this.loc.PropertyMain.Columns],
        ["Watermark", "PageOptions.PageOptionWatermark.png", this.loc.PropertyMain.Watermark]
    ];

    //Add Panels && Buttons    
    var buttonsPanel = mainTable.addCell();
    buttonsPanel.style.verticalAlign = "top";
    buttonsPanel.style.paddingTop = "12px";
    pageSetupForm.mainButtons = {};
    pageSetupForm.panels = {};

    var panelsContainer = mainTable.addCell();
    panelsContainer.className = "stiDesignerPageSetupFormPanel";

    for (var i = 0; i < buttonProps.length; i++) {
        var panel = document.createElement("Div");
        if (i != 0) panel.style.display = "none";
        panelsContainer.appendChild(panel);
        pageSetupForm.panels[buttonProps[i][0]] = panel;

        var button = this.FormTabPanelButton(null, buttonProps[i][2], buttonProps[i][1], buttonProps[i][2], null, { width: 24, height: 24 }, 34);
        pageSetupForm.mainButtons[buttonProps[i][0]] = button;
        buttonsPanel.appendChild(button);
        button.panelName = buttonProps[i][0];

        button.action = function () {
            pageSetupForm.setMode(this.panelName);
        }

        //add marker
        var marker = document.createElement("div");
        marker.style.display = "none";
        marker.className = "stiUsingMarker";
        var markerInner = document.createElement("div");
        marker.appendChild(markerInner);
        button.style.position = "relative";
        button.appendChild(marker);
        button.marker = marker;
    }

    pageSetupForm.panels.Paper.appendChild(this.PageSetupPaperBlock(pageSetupForm));
    pageSetupForm.panels.Columns.appendChild(this.PageSetupColumnsBlock(pageSetupForm));
    pageSetupForm.panels.Watermark.appendChild(this.PageSetupWatermarkBlock(pageSetupForm));

    pageSetupForm.setMode = function (mode) {
        pageSetupForm.mode = mode;
        for (var panelName in pageSetupForm.panels) {
            pageSetupForm.panels[panelName].style.display = mode == panelName ? "" : "none";
            pageSetupForm.mainButtons[panelName].setSelected(mode == panelName);
        }
        if (mode == "Watermark") {
            jsObject.options.controls.pageSetupWatermarkText.textArea.focus();
            jsObject.options.controls.pageSetupWatermarkImageGallery.autoscroll();
        }
    }

    pageSetupForm.changeSizeValues = function () {
        var controls = jsObject.options.controls;
        var temp = controls.pageSetupPageWidth.getValue();
        controls.pageSetupPageWidth.setValue(controls.pageSetupPageHeight.getValue());
        controls.pageSetupPageHeight.setValue(temp);
        var leftMargin = controls.pageSetupLeftMargin.getValue();
        var topMargin = controls.pageSetupTopMargin.getValue();
        var rightMargin = controls.pageSetupRightMargin.getValue();
        var bottomMargin = controls.pageSetupBottomMargin.getValue();
        controls.pageSetupLeftMargin.setValue(controls.pageSetupOrientation.key == "Portrait" ? bottomMargin : topMargin);
        controls.pageSetupTopMargin.setValue(controls.pageSetupOrientation.key == "Portrait" ? leftMargin : rightMargin);
        controls.pageSetupRightMargin.setValue(controls.pageSetupOrientation.key == "Portrait" ? topMargin : bottomMargin);
        controls.pageSetupBottomMargin.setValue(controls.pageSetupOrientation.key == "Portrait" ? rightMargin : leftMargin);
        pageSetupForm.pageSample.update();
    }

    pageSetupForm.applyPropertiesToObject = function (object) {
        var controls = jsObject.options.controls;
        var buttons = jsObject.options.buttons;

        object.paperSize = controls.pageSetupPaperSize.key;
        object.unitWidth = controls.pageSetupPageWidth.getValue();
        object.unitHeight = controls.pageSetupPageHeight.getValue();
        object.orientation = controls.pageSetupOrientation.key;
        object.unitMargins = controls.pageSetupLeftMargin.getValue() + "!" + controls.pageSetupTopMargin.getValue() + "!" +
            controls.pageSetupRightMargin.getValue() + "!" + controls.pageSetupBottomMargin.getValue();
        object.mirrorMargins = controls.pageSetupMirrorMargins.isChecked;
        object.columns = controls.pageSetupNumberOfColumns.getValue();
        object.columnWidth = controls.pageSetupColumnWidth.getValue();
        object.columnGaps = controls.pageSetupColumnGaps.getValue();
        object.rightToLeft = controls.pageSetupColumnsRightToLeft.isChecked;
        object.waterMarkText = StiBase64.encode(controls.pageSetupWatermarkText.getValue());
        object.waterMarkAngle = controls.pageSetupWatermarkAngle.value;
        object.waterMarkFont = controls.pageSetupWatermarkFontName.key + "!" + controls.pageSetupWatermarkFontSize.key + "!" +
            (buttons.pageSetupWatermarkFontBold.isSelected ? "1" : "0") + "!" + (buttons.pageSetupWatermarkFontItalic.isSelected ? "1" : "0") + "!" +
            (buttons.pageSetupWatermarkFontUnderline.isSelected ? "1" : "0") + "!" + (buttons.pageSetupWatermarkFontStrikeout.isSelected ? "1" : "0");
        object.waterMarkTextBrush = controls.pageSetupWatermarkTextBrush.key;
        object.waterMarkEnabled = controls.pageSetupWatermarkEnabled.key != "False";
        object.waterMarkEnabledExpression = controls.pageSetupWatermarkEnabled.key == "Expression" ?
            StiBase64.encode(controls.pageSetupWatermarkEnabledExpression.value) : "";

        object.waterMarkRightToLeft = controls.pageSetupWatermarkRightToLeft.isChecked;
        object.waterMarkTextBehind = controls.pageSetupWatermarkShowBehind.isChecked;
        object.waterMarkImageAlign = controls.pageSetupWatermarkImageAlignment.key;
        object.waterMarkMultipleFactor = controls.pageSetupWatermarkMultipleFactor.value;
        object.waterMarkTransparency = controls.pageSetupWatermarkTransparency.value.toString();
        object.waterMarkRatio = controls.pageSetupWatermarkAspectRatio.isChecked;
        object.waterMarkImageBehind = controls.pageSetupWatermarkShowImageBehind.isChecked;
        object.waterMarkStretch = controls.pageSetupWatermarkImageStretch.isChecked;
        object.waterMarkTiling = controls.pageSetupWatermarkImageTiling.isChecked;

        if (controls.pageSetupWatermarkImage.imageHyperlink) {
            object.watermarkImageHyperlink = controls.pageSetupWatermarkImage.imageHyperlink;
            object.watermarkImageSrc = "";
            object.watermarkImageContentForPaint = controls.pageSetupWatermarkImage.src;
        }
        else {
            var imageSrc = controls.pageSetupWatermarkImage.src || "";
            if (jsObject.options.mvcMode) imageSrc = encodeURIComponent(imageSrc);
            object.watermarkImageSrc = imageSrc;
            object.watermarkImageHyperlink = "";
        }
    }

    pageSetupForm.updateMarkers = function () {
        var controls = jsObject.options.controls;
        this.mainButtons.Columns.marker.style.display = parseInt(controls.pageSetupNumberOfColumns.getValue()) > 0 ? "" : "none";
        this.mainButtons.Watermark.marker.style.display = controls.pageSetupWatermarkText.getValue() || controls.pageSetupWatermarkImage.imageHyperlink || controls.pageSetupWatermarkImage.src ? "" : "none";
    }

    pageSetupForm.action = function () {
        pageSetupForm.pageSample.innerHTML = ""; //clear page sample
        var currentPage = jsObject.options.currentPage;
        pageSetupForm.applyPropertiesToObject(currentPage.properties);

        this.changeVisibleState(false);
        jsObject.SendCommandSendProperties(currentPage, ["paperSize", "orientation", "unitWidth", "unitHeight", "unitMargins", "columns", "columnWidth", "columnGaps",
            "rightToLeft", "waterMarkText", "waterMarkAngle", "waterMarkFont", "waterMarkTextBrush", "waterMarkEnabled", "waterMarkEnabledExpression", "waterMarkRightToLeft",
            "waterMarkTextBehind", "waterMarkImageAlign", "waterMarkMultipleFactor", "waterMarkTransparency", "waterMarkRatio", "waterMarkImageBehind", "waterMarkStretch",
            "waterMarkTiling", "watermarkImageSrc", "mirrorMargins", "watermarkImageHyperlink"]);
    }

    pageSetupForm.onhide = function () {
        clearTimeout(this.markerTimer);
    }

    pageSetupForm.onshow = function () {
        var currentPage = jsObject.options.currentPage;
        var controls = jsObject.options.controls;
        var buttons = jsObject.options.buttons;
        pageSetupForm.setMode("Paper");
        pageSetupForm.panels.Watermark.showAllOptions = false;

        var waterMarkEnabledExpression = StiBase64.decode(currentPage.properties.waterMarkEnabledExpression);
        controls.pageSetupWatermarkEnabled.setKey(waterMarkEnabledExpression ? "Expression" : (currentPage.properties.waterMarkEnabled ? "True" : "False"));
        controls.pageSetupWatermarkEnabledExpression.value = waterMarkEnabledExpression;
        pageSetupForm.watermarkRows["EnabledExpression"].style.display = controls.pageSetupWatermarkEnabled.key == "Expression" ? "" : "none";

        pageSetupForm.changeVisibleStateMoreOptions(pageSetupForm.panels.Watermark.showAllOptions);

        controls.pageSetupPaperSize.setKey(currentPage.properties.paperSize);
        controls.pageSetupPageWidth.setValue(currentPage.properties.unitWidth);
        controls.pageSetupPageHeight.setValue(currentPage.properties.unitHeight);
        controls.pageSetupOrientation.setKey(currentPage.properties.orientation);

        var margins = currentPage.properties.unitMargins.split("!");
        controls.pageSetupLeftMargin.setValue(margins[0]);
        controls.pageSetupTopMargin.setValue(margins[1]);
        controls.pageSetupRightMargin.setValue(margins[2]);
        controls.pageSetupBottomMargin.setValue(margins[3]);
        controls.pageSetupMirrorMargins.setChecked(currentPage.properties.mirrorMargins);
        controls.pageSetupNumberOfColumns.setValue(currentPage.properties.columns);
        controls.pageSetupColumnWidth.setValue(currentPage.properties.columnWidth);
        controls.pageSetupColumnGaps.setValue(currentPage.properties.columnGaps);
        controls.pageSetupColumnsRightToLeft.setChecked(currentPage.properties.rightToLeft);
        controls.pageSetupWatermarkText.setValue(StiBase64.decode(currentPage.properties.waterMarkText));
        controls.pageSetupWatermarkAngle.value = currentPage.properties.waterMarkAngle;
        var font = currentPage.properties.waterMarkFont.split("!");
        controls.pageSetupWatermarkFontName.setKey(font[0]);
        controls.pageSetupWatermarkFontSize.setKey(font[1]);
        buttons.pageSetupWatermarkFontBold.setSelected(font[2] == "1");
        buttons.pageSetupWatermarkFontItalic.setSelected(font[3] == "1");
        buttons.pageSetupWatermarkFontUnderline.setSelected(font[4] == "1");
        buttons.pageSetupWatermarkFontStrikeout.setSelected(font[5] == "1");
        controls.pageSetupWatermarkTextBrush.setKey(currentPage.properties.waterMarkTextBrush);

        controls.pageSetupWatermarkRightToLeft.setChecked(currentPage.properties.waterMarkRightToLeft);
        controls.pageSetupWatermarkShowBehind.setChecked(currentPage.properties.waterMarkTextBehind);
        controls.pageSetupWatermarkImageAlignment.setKey(currentPage.properties.waterMarkImageAlign);
        controls.pageSetupWatermarkMultipleFactor.value = currentPage.properties.waterMarkMultipleFactor;
        controls.pageSetupWatermarkTransparency.setValue(parseInt(currentPage.properties.waterMarkTransparency));
        controls.pageSetupWatermarkImage.setImage(currentPage.properties.watermarkImageSrc);
        controls.pageSetupWatermarkAspectRatio.setChecked(currentPage.properties.waterMarkRatio);
        controls.pageSetupWatermarkShowImageBehind.setChecked(currentPage.properties.waterMarkImageBehind);
        controls.pageSetupWatermarkImageStretch.setChecked(currentPage.properties.waterMarkStretch);
        controls.pageSetupWatermarkImageTiling.setChecked(currentPage.properties.waterMarkTiling);
        pageSetupForm.pageSample.update();


        //Update Image Gallery
        controls.pageSetupWatermarkImage.itemName = null;
        controls.pageSetupWatermarkImage.imageHyperlink = currentPage.properties.watermarkImageHyperlink || "";

        if (jsObject.options.imagesGallery || jsObject.CheckImagesInDictionary()) {
            controls.pageSetupWatermarkImageGallery.changeVisibleState(true);
            controls.pageSetupWatermarkImageGallery.progress.firstChild.style.width = "32px";
            controls.pageSetupWatermarkImageGallery.progress.firstChild.style.height = "32px";
            controls.pageSetupWatermarkImageGallery.progress.show(170, -55);

            if (!jsObject.options.imagesGallery) {
                jsObject.SendCommandToDesignerServer("GetImagesGallery", null, function (answer) {
                    jsObject.options.imagesGallery = answer.imagesGallery;
                    pageSetupForm.fillImagesGallery(answer.imagesGallery);
                    pageSetupForm.selectGalleryItem();
                });
            }
            else {
                pageSetupForm.fillImagesGallery(jsObject.options.imagesGallery)
            }
        }
        else {
            controls.pageSetupWatermarkImageGallery.changeVisibleState(false);
        }

        pageSetupForm.selectGalleryItem();

        this.updateMarkers();
        this.markerTimer = setInterval(function () {
            pageSetupForm.updateMarkers();
        }, 250)
    };

    pageSetupForm.fillImagesGallery = function (imagesGallery) {
        var controls = jsObject.options.controls;
        controls.pageSetupWatermarkImageGallery.progress.hide();
        var allImages = [].concat(imagesGallery.variables, imagesGallery.resources);

        if (allImages.length > 0) {
            controls.pageSetupWatermarkImageGallery.addItems(allImages);
        }
        else {
            controls.pageSetupWatermarkImageGallery.changeVisibleState(false);
        }
    }

    pageSetupForm.selectGalleryItem = function () {
        var controls = jsObject.options.controls;
        var resourceIdent = jsObject.options.resourceIdent;
        var variableIdent = jsObject.options.variableIdent;
        var imageHyperlink = controls.pageSetupWatermarkImage.imageHyperlink;

        if (imageHyperlink.indexOf(resourceIdent) == 0 || imageHyperlink.indexOf(variableIdent) == 0) {
            var ident = imageHyperlink.indexOf(resourceIdent) == 0 ? resourceIdent : variableIdent;
            if (jsObject.options.imagesGallery) {
                var item = controls.pageSetupWatermarkImageGallery.getItemByPropertyValue("name", imageHyperlink.substring(imageHyperlink.indexOf(ident) + ident.length));
                if (item) item.action(true);
            }
        }
    }

    //Override
    if (pageSetupForm["buttonHelp"])
        pageSetupForm.buttonHelp.action = function () {
            switch (pageSetupForm.mode) {
                case "Watermark": jsObject.ShowHelpWindow(jsObject.HelpLinks["watermark"]); break;
                case "Columns": jsObject.ShowHelpWindow(jsObject.HelpLinks["columns"]); break;
                case "Paper": jsObject.ShowHelpWindow(pageSetupForm.helpUrl); break;
            }
        };

    return pageSetupForm
}

//Paper Block
StiMobileDesigner.prototype.PageSetupPaperBlock = function (pageSetupForm) {
    var jsObject = this;
    var paperSizeBlock = this.FormBlock(pageSetupForm.width, pageSetupForm.height);

    var paperSizeTable = this.CreateHTMLTable();
    paperSizeTable.style.marginTop = "12px";
    paperSizeTable.style.display = "inline-block";
    paperSizeTable.style.width = "100%";
    paperSizeBlock.appendChild(paperSizeTable);

    //Paper Size
    paperSizeTable.addTextCell(this.loc.PropertyMain.Size).className = "stiDesignerCaptionControlsBigIntervals";

    var sizeControl = this.DropDownList("pageSetupPaperSize", 210, null, this.GetPageOrientationItems(false), true);
    sizeControl.style.marginLeft = "20px";
    paperSizeTable.addCellInLastRow(sizeControl).className = "stiDesignerControlCellsBigIntervals2";

    var sizeMenu = this.PageSizeMenu("pageSetupPageSizeMenu", sizeControl);
    sizeControl.menu = sizeMenu;

    sizeControl.textSizes = function (num) {
        if (num == 0) return "";
        var unit = jsObject.options.report.properties.reportUnit;
        var pageWidth = jsObject.ConvertPixelToUnit(jsObject.PaperSizesInPixels[num][0]).toFixed(1);
        var pageHeight = jsObject.ConvertPixelToUnit(jsObject.PaperSizesInPixels[num][1]).toFixed(1);

        return pageWidth + unit + " X " + pageHeight + unit;
    }

    sizeControl.setKey = function (key) {
        this.key = key;
        var text = jsObject.options.paperSizes[key];
        if (key != 0) text += " (" + this.textSizes(key) + ") ";
        this.textBox.value = text;
    }

    sizeMenu.action = function (menuItem) {
        menuItem.setSelected(true);
        this.changeVisibleState(false);
        this.parentButton.setKey(menuItem.key);
        if (menuItem.key == 0) return;
        var pageWidth = jsObject.ConvertPixelToUnit(jsObject.PaperSizesInPixels[parseInt(menuItem.key)][0]).toFixed(1);
        var pageHeight = jsObject.ConvertPixelToUnit(jsObject.PaperSizesInPixels[parseInt(menuItem.key)][1]).toFixed(1);
        if (Math.round(pageWidth) == pageWidth) pageWidth = Math.round(pageWidth);
        if (Math.round(pageHeight) == pageHeight) pageHeight = Math.round(pageHeight);
        var isPortrait = jsObject.options.controls.pageSetupOrientation.key == "Portrait";
        jsObject.options.controls.pageSetupPageWidth.setValue(isPortrait ? pageWidth : pageHeight);
        jsObject.options.controls.pageSetupPageHeight.setValue(isPortrait ? pageHeight : pageWidth);
        pageSetupForm.pageSample.update();
    }

    sizeMenu.onshow = function () {
        for (var itemName in this.items) {
            var num = parseInt(this.items[itemName].key);
            if (this.items[itemName].key == this.parentButton.key) this.items[itemName].setSelected(true);
            this.items[itemName].caption.innerHTML = "<b>" + jsObject.options.paperSizes[num] + "</b><br>  " + sizeControl.textSizes(num);
        }
    }

    //Page Width
    paperSizeTable.addTextCellInNextRow(this.loc.PropertyMain.Width).className = "stiDesignerCaptionControlsBigIntervals";

    var pageWidthControl = this.TextBoxEnumerator("pageSetupPageWidth", 80, null, null, null, 0, true);
    pageWidthControl.style.marginLeft = "20px";
    paperSizeTable.addCellInLastRow(pageWidthControl).className = "stiDesignerControlCellsBigIntervals2";
    pageWidthControl.action = function () { pageSetupForm.pageSample.update(); }

    //Page Height
    paperSizeTable.addTextCellInNextRow(this.loc.PropertyMain.Height).className = "stiDesignerCaptionControlsBigIntervals";

    var pageHeightControl = this.TextBoxEnumerator("pageSetupPageHeight", 80, null, null, null, 0, true);
    pageHeightControl.style.marginLeft = "20px";
    paperSizeTable.addCellInLastRow(pageHeightControl).className = "stiDesignerControlCellsBigIntervals2";
    pageHeightControl.action = function () { pageSetupForm.pageSample.update(); }

    var cellOrient = paperSizeTable.addCellInNextRow();
    cellOrient.style.padding = "8px 0 8px 0";
    cellOrient.setAttribute("colspan", "2");
    cellOrient.appendChild(this.FormSeparator());

    //Orientation
    paperSizeTable.addTextCellInNextRow(this.loc.PropertyMain.Orientation).className = "stiDesignerCaptionControlsBigIntervals";

    var orientControl = this.DropDownList("pageSetupOrientation", 210, null, this.GetPageOrientationItems(false), true);
    orientControl.style.marginLeft = "20px";
    paperSizeTable.addCellInLastRow(orientControl).className = "stiDesignerControlCellsBigIntervals2";
    orientControl.action = function () { pageSetupForm.changeSizeValues(); }

    //Margins
    var cellMargins = paperSizeTable.addCellInNextRow();
    cellMargins.style.padding = "8px 0 8px 0";
    cellMargins.setAttribute("colspan", "2");
    cellMargins.appendChild(this.FormSeparator());

    paperSizeTable.addTextCellInNextRow(this.loc.PropertyMain.Margins).className = "stiDesignerCaptionControlsBigIntervals";

    var marginsTable = this.CreateHTMLTable();
    paperSizeTable.addCellInLastRow(marginsTable).className = "stiDesignerControlCellsBigIntervals2";

    //Left
    var imgLeft = document.createElement("img");
    imgLeft.style.margin = "0 4px 0 0";
    StiMobileDesigner.setImageSource(imgLeft, this.options, "Arrows.MoveLeftGray.png");
    marginsTable.addCell(imgLeft);

    var leftControl = this.TextBoxEnumerator("pageSetupLeftMargin", 80, null, null, null, 0, true);
    leftControl.action = function () { pageSetupForm.pageSample.update(); }
    marginsTable.addCell(leftControl);

    //Right
    var imgRight = document.createElement("img");
    imgRight.style.margin = "0 4px 0 24px";
    StiMobileDesigner.setImageSource(imgRight, this.options, "Arrows.MoveRightGray.png");
    marginsTable.addCell(imgRight);

    var rightControl = this.TextBoxEnumerator("pageSetupRightMargin", 80, null, null, null, 0, true);
    rightControl.action = function () { pageSetupForm.pageSample.update(); }
    marginsTable.addCell(rightControl);

    //Top
    var imgTop = document.createElement("img");
    imgTop.style.margin = "10px 4px 0 0";
    StiMobileDesigner.setImageSource(imgTop, this.options, "Arrows.MoveUpGray.png");
    marginsTable.addCellInNextRow(imgTop);

    var topControl = this.TextBoxEnumerator("pageSetupTopMargin", 80, null, null, null, 0, true);
    topControl.style.marginTop = "10px";
    topControl.action = function () { pageSetupForm.pageSample.update(); }
    marginsTable.addCellInLastRow(topControl);

    //Bottom
    var imgBottom = document.createElement("img");
    imgBottom.style.margin = "10px 4px 0 24px";
    StiMobileDesigner.setImageSource(imgBottom, this.options, "Arrows.MoveDownGray.png");
    marginsTable.addCellInLastRow(imgBottom);

    var bottomControl = this.TextBoxEnumerator("pageSetupBottomMargin", 80, null, null, null, 0, true);
    bottomControl.style.marginTop = "10px";
    bottomControl.action = function () { pageSetupForm.pageSample.update(); }
    marginsTable.addCellInLastRow(bottomControl);

    //Margins
    paperSizeTable.addCellInNextRow();
    var mirMrgControl = this.CheckBox("pageSetupMirrorMargins", this.loc.PropertyMain.MirrorMargins);
    mirMrgControl.action = function () { pageSetupForm.pageSample.update(); }
    mirMrgControl.style.margin = "12px 0 0 4px";
    paperSizeTable.addCellInLastRow(mirMrgControl).className = "stiDesignerCaptionControlsBigIntervals";

    return paperSizeBlock;
}

//Columns Block
StiMobileDesigner.prototype.PageSetupColumnsBlock = function (pageSetupForm) {
    var columnsBlock = this.FormBlock(pageSetupForm.width, pageSetupForm.height);

    var columnsTable = this.CreateHTMLTable();
    columnsTable.style.marginTop = "8px";
    columnsTable.style.display = "inline-block";
    columnsTable.style.width = "100%";
    columnsBlock.appendChild(columnsTable);

    //NumberOfColumns
    columnsTable.addTextCellInLastRow(this.loc.FormPageSetup.NumberOfColumns.replace(":", "")).className = "stiDesignerCaptionControlsBigIntervals";

    var numColumnControl = this.TextBoxEnumerator("pageSetupNumberOfColumns", 80, null, null, null, 0);
    numColumnControl.style.marginLeft = "20px";
    columnsTable.addCellInLastRow(numColumnControl).className = "stiDesignerControlCellsBigIntervals2";

    //Column Width
    columnsTable.addTextCellInNextRow(this.loc.PropertyMain.ColumnWidth).className = "stiDesignerCaptionControlsBigIntervals";

    var widthColumnControl = this.TextBoxEnumerator("pageSetupColumnWidth", 80, null, null, null, 0, true);
    widthColumnControl.style.marginLeft = "20px";
    columnsTable.addCellInLastRow(widthColumnControl).className = "stiDesignerControlCellsBigIntervals2";

    //Column Gaps
    columnsTable.addTextCellInNextRow(this.loc.PropertyMain.ColumnGaps).className = "stiDesignerCaptionControlsBigIntervals";

    var gapsColumnControl = this.TextBoxEnumerator("pageSetupColumnGaps", 80, null, null, null, 0, true);
    gapsColumnControl.style.marginLeft = "20px";
    columnsTable.addCellInLastRow(gapsColumnControl).className = "stiDesignerControlCellsBigIntervals2";

    //Columns Right To Left
    columnsTable.addCellInNextRow();

    var rtLColControl = this.CheckBox("pageSetupColumnsRightToLeft", this.loc.PropertyMain.RightToLeft);
    rtLColControl.style.margin = "12px 0 0 20px";
    columnsTable.addCellInLastRow(rtLColControl);

    return columnsBlock;
}

//Watermark Block
StiMobileDesigner.prototype.PageSetupWatermarkBlock = function (pageSetupForm) {
    var jsObject = this;
    var watermarkBlock = this.FormBlock(pageSetupForm.width, pageSetupForm.height);
    watermarkBlock.showAllOptions = false;
    pageSetupForm.watermarkRows = {};

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "100%";
    mainTable.style.height = pageSetupForm.height + "px";
    watermarkBlock.appendChild(mainTable);

    var pageSample = document.createElement("div");
    pageSetupForm.pageSample = pageSample;
    pageSample.style.margin = "10px 5px 5px 5px";

    pageSample.update = function (imageSize) {
        this.innerHTML = "";
        var currentPage = jsObject.options.currentPage;
        var pagePoperties = jsObject.CopyObject(currentPage.properties);
        if (imageSize) pagePoperties.watermarkImageSize = imageSize;
        pageSetupForm.applyPropertiesToObject(pagePoperties);
        pagePoperties.columns = "0";

        var pageWidth = parseInt(jsObject.ConvertUnitToPixel(jsObject.StrToDouble(pagePoperties.unitWidth)));
        var pageHeight = parseInt(jsObject.ConvertUnitToPixel(jsObject.StrToDouble(pagePoperties.unitHeight)));
        if (pagePoperties.segmentPerHeight && pagePoperties.segmentPerWidth) {
            var marginsStr = pagePoperties.unitMargins.split("!");
            var verticalMarginsPx = parseInt(jsObject.ConvertUnitToPixel(jsObject.StrToDouble(marginsStr[1]) + jsObject.StrToDouble(marginsStr[3]), currentPage.isDashboard));
            var horizontalMarginsPx = parseInt(jsObject.ConvertUnitToPixel(jsObject.StrToDouble(marginsStr[0]) + jsObject.StrToDouble(marginsStr[2]), currentPage.isDashboard));
            var segmentPerHeight = jsObject.StrToDouble(pagePoperties.segmentPerHeight);
            var segmentPerWidth = jsObject.StrToDouble(pagePoperties.segmentPerWidth);
            if (segmentPerWidth > 1) pageWidth = ((pageWidth - horizontalMarginsPx) * segmentPerWidth) + horizontalMarginsPx;
            if (segmentPerHeight > 1) pageHeight = ((pageHeight - verticalMarginsPx) * segmentPerHeight) + verticalMarginsPx;
        }

        var vertZoom = 380 / pageHeight;
        var horZoom = 125 / pageWidth;

        var pageSvg = jsObject.GetSvgPageForCheckPreview(jsObject.options.currentPage.properties.pageIndex, null, Math.min(vertZoom, horZoom), true, pagePoperties);
        pageSample.appendChild(pageSvg);
    }

    mainTable.addCell(pageSample).className = "stiDesignerPageSetupFormSampleCell";
    var watermarkTable = this.CreateHTMLTable();
    watermarkTable.style.width = "100%";
    mainTable.addCell(watermarkTable).style.verticalAlign = "top";

    //Text
    watermarkTable.addRow();
    watermarkTable.style.margin = "0 0 6px 0";
    var cellWatermarkText = watermarkTable.addCellInLastRow();
    cellWatermarkText.style.padding = "0 0 6px 0";
    cellWatermarkText.setAttribute("colspan", "2");
    cellWatermarkText.appendChild(this.FormBlockHeader(this.loc.Toolbars.ToolbarWatermarkText));

    watermarkTable.addRow();
    var watermarkTextControl = this.ExpressionTextArea("pageSetupWatermarkText", pageSetupForm.width - 171, 50);
    watermarkTextControl.action = function () { pageSample.update(); }
    watermarkTextControl.style.margin = "2px 0px 2px 8px";
    watermarkTable.addCellInLastRow(watermarkTextControl).setAttribute("colspan", "2");

    //Angle
    pageSetupForm.watermarkRows["Angle"] = watermarkTable.addRow();
    watermarkTable.addTextCellInLastRow(this.loc.PropertyMain.Angle).className = "stiDesignerCaptionControlsBigIntervals";

    var watermarkAngleControl = this.TextBoxIntValue("pageSetupWatermarkAngle", 50);
    watermarkAngleControl.style.marginTop = "2px";
    watermarkAngleControl.action = function () { pageSample.update(); }
    watermarkTable.addCellInLastRow(watermarkAngleControl).className = "stiDesignerControlCellsBigIntervals";

    //Font
    pageSetupForm.watermarkRows["Font"] = watermarkTable.addRow();
    watermarkTable.addTextCellInLastRow(this.loc.PropertyMain.Font).className = "stiDesignerCaptionControlsBigIntervals";

    var fontTable = this.CreateHTMLTable();
    watermarkTable.addCellInLastRow(fontTable).className = "stiDesignerControlCellsBigIntervals";

    var fontName = this.FontList("pageSetupWatermarkFontName", 96);
    fontName.action = function () {
        if (this.key == "Aharoni") { jsObject.options.buttons.pageSetupWatermarkFontBold.setSelected(true); }
        jsObject.options.buttons.pageSetupWatermarkFontBold.isEnabled = !(this.key == "Aharoni");
        pageSample.update();
    };

    fontTable.addCell(fontName);

    var sizeItems = [];
    for (var i = 0; i < this.options.fontSizes.length; i++) {
        sizeItems.push(this.Item("sizesFont" + i, this.options.fontSizes[i], null, this.options.fontSizes[i]));
    }
    var fontSize = this.DropDownList("pageSetupWatermarkFontSize", 60, this.loc.HelpDesigner.FontSize, sizeItems, false);
    fontSize.action = function () {
        this.setKey(Math.abs(jsObject.StrToDouble(this.key)).toString());
        pageSample.update();
    }
    fontTable.addCell(fontSize).style.padding = "0 4px 0 4px";

    pageSetupForm.watermarkRows["Font2"] = watermarkTable.addRow();
    var fontTable2 = this.CreateHTMLTable();
    watermarkTable.addCellInLastRow();
    watermarkTable.addCellInLastRow(fontTable2).className = "stiDesignerControlCellsBigIntervals";

    var textBrushButton = this.BrushControl("pageSetupWatermarkTextBrush", null, "TextColor.png", this.loc.PropertyMain.TextBrush);
    textBrushButton.action = function () { pageSample.update(); }
    fontTable2.addCell(textBrushButton).style.padding = "0 2px 0 0";

    var boldButton = this.StandartSmallButton("pageSetupWatermarkFontBold", null, null, "Bold.png", this.loc.PropertyMain.Bold, null);
    boldButton.action = function () {
        this.setSelected(!this.isSelected);
        pageSample.update();
    }
    fontTable2.addCell(boldButton).style.padding = "0 2px 0 4px";

    var italicButton = this.StandartSmallButton("pageSetupWatermarkFontItalic", null, null, "Italic.png", this.loc.PropertyMain.Italic, null);
    italicButton.action = function () {
        this.setSelected(!this.isSelected);
        pageSample.update();
    }
    fontTable2.addCell(italicButton).style.padding = "0 2px 0 2px";

    var underlineButton = this.StandartSmallButton("pageSetupWatermarkFontUnderline", null, null, "Underline.png", this.loc.PropertyMain.Underline, null);
    underlineButton.action = function () {
        this.setSelected(!this.isSelected);
        pageSample.update();
    }
    fontTable2.addCell(underlineButton).style.padding = "0 2px 0 2px";

    var strikeoutButton = this.StandartSmallButton("pageSetupWatermarkFontStrikeout", null, null, "Strikeout.png", this.loc.PropertyMain.FontStrikeout, null);
    strikeoutButton.action = function () {
        this.setSelected(!this.isSelected);
        pageSample.update();
    }
    fontTable2.addCell(strikeoutButton).style.padding = "0 2px 0 2px";

    //Watermark Enabled
    pageSetupForm.watermarkRows["Enabled"] = watermarkTable.addRow();
    watermarkTable.addTextCellInLastRow(this.loc.PropertyMain.Enabled).className = "stiDesignerCaptionControlsBigIntervals";
    var watermarkEnabledControl = this.DropDownList("pageSetupWatermarkEnabled", 167, null, this.GetBoolAndExpressionItems(), false, null, null, true);
    watermarkTable.addCellInLastRow(watermarkEnabledControl).className = "stiDesignerControlCellsBigIntervals";

    watermarkEnabledControl.action = function () {
        pageSetupForm.watermarkRows["EnabledExpression"].style.display = this.key == "Expression" ? "" : "none";
        pageSample.update();
    }

    //Watermark Enabled Expression
    pageSetupForm.watermarkRows["EnabledExpression"] = watermarkTable.addRow();
    watermarkTable.addTextCellInLastRow(this.loc.PropertyMain.Expression).className = "stiDesignerCaptionControlsBigIntervals";

    var watermarkExpressionControl = this.ExpressionControl("pageSetupWatermarkEnabledExpression", 180);
    watermarkExpressionControl.action = function () { pageSample.update(); }
    watermarkTable.addCellInLastRow(watermarkExpressionControl).className = "stiDesignerControlCellsBigIntervals";

    //Watermark Right To Left
    pageSetupForm.watermarkRows["RightToLeft"] = watermarkTable.addRow();
    var watermarkRightToLeftCell = watermarkTable.addCellInLastRow();
    watermarkRightToLeftCell.className = "stiDesignerCaptionControlsBigIntervals";
    watermarkRightToLeftCell.style.paddingTop = "6px";
    watermarkRightToLeftCell.style.paddingBottom = "6px";
    watermarkRightToLeftCell.setAttribute("colspan", "2");

    var watermarkRightToLeftControl = this.CheckBox("pageSetupWatermarkRightToLeft", this.loc.PropertyMain.RightToLeft);
    watermarkRightToLeftControl.action = function () { pageSample.update(); }
    watermarkRightToLeftCell.appendChild(watermarkRightToLeftControl);

    //Watermark Show Behind
    pageSetupForm.watermarkRows["ShowBehind"] = watermarkTable.addRow();
    var watermarkShowBehindCell = watermarkTable.addCellInLastRow();
    watermarkShowBehindCell.className = "stiDesignerCaptionControlsBigIntervals";
    watermarkShowBehindCell.style.paddingTop = "6px";
    watermarkShowBehindCell.style.paddingBottom = "6px";
    watermarkShowBehindCell.setAttribute("colspan", "2");

    var watermarkShowBehindControl = this.CheckBox("pageSetupWatermarkShowBehind", this.loc.PropertyMain.ShowBehind);
    watermarkShowBehindControl.action = function () { pageSample.update(); }
    watermarkShowBehindCell.appendChild(watermarkShowBehindControl);

    //Watermark Image
    watermarkTable.addRow();
    var cellWatermarkImage = watermarkTable.addCellInLastRow();
    cellWatermarkImage.style.padding = "6px 0 6px 0";
    cellWatermarkImage.setAttribute("colspan", "2");
    cellWatermarkImage.appendChild(this.FormBlockHeader(this.loc.Toolbars.ToolbarWatermarkImage));

    watermarkTable.addRow();
    var watermarkImage = this.ImageControl("pageSetupWatermarkImage", pageSetupForm.width - 167, 80);
    watermarkImage.style.margin = "2px 0 2px 8px";
    watermarkTable.addCellInLastRow(watermarkImage).setAttribute("colspan", "2");

    //Watermark Image Gallery
    var watermarkImageGallery = this.ImageGallery("pageSetupWatermarkImageGallery", pageSetupForm.width - 151, 60);
    watermarkTable.addCellInNextRow(watermarkImageGallery).setAttribute("colspan", "2");

    watermarkImageGallery.action = function (item) {
        watermarkImage.setImage(item.itemObject.src);
        pageSample.update(watermarkImage.imageContainer.naturalWidth + ";" + watermarkImage.imageContainer.naturalHeight);
        watermarkImage.imageHyperlink = item.itemObject.type == "StiResource"
            ? jsObject.options.resourceIdent + item.itemObject.name
            : (item.itemObject.type == "StiVariable" ? jsObject.options.variableIdent + item.itemObject.name : "");
    }

    watermarkImageGallery.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
    }

    watermarkImage.action = function () {
        var imageSize = null;
        if (this.src) {
            imageSize = this.imageContainer.naturalWidth + ";" + this.imageContainer.naturalHeight;
        }
        pageSample.update(imageSize);
        this.imageHyperlink = "";
        this.itemName = null;
        if (watermarkImageGallery.selectedItem) watermarkImageGallery.selectedItem.select(false);
    }

    //Image Align
    pageSetupForm.watermarkRows["ImageAlign"] = watermarkTable.addRow();
    watermarkTable.addTextCellInLastRow(this.loc.PropertyMain.ImageAlignment).className = "stiDesignerCaptionControlsBigIntervals";

    var imageAlignList = this.DropDownList("pageSetupWatermarkImageAlignment", 167, null, this.GetImageAlignItems(), true, false, null, true);
    imageAlignList.style.marginTop = "2px";
    imageAlignList.action = function () { pageSample.update(); }

    watermarkTable.addCellInLastRow(imageAlignList).className = "stiDesignerControlCellsBigIntervals";

    //MultipleFactor
    pageSetupForm.watermarkRows["MultipleFactor"] = watermarkTable.addRow();
    watermarkTable.addTextCellInLastRow(this.loc.PropertyMain.ImageMultipleFactor).className = "stiDesignerCaptionControlsBigIntervals";

    var watermarkMFactorControl = this.TextBoxDoubleValue("pageSetupWatermarkMultipleFactor", 50);
    watermarkMFactorControl.action = function () { pageSample.update(); }
    watermarkTable.addCellInLastRow(watermarkMFactorControl).className = "stiDesignerControlCellsBigIntervals";

    //Transparency
    pageSetupForm.watermarkRows["Transparency"] = watermarkTable.addRow();
    watermarkTable.addTextCellInLastRow(this.loc.PropertyMain.ImageTransparency).className = "stiDesignerCaptionControlsBigIntervals";

    var watermarkTransparencyControl = this.SladerControl("pageSetupWatermarkTransparency", 150, 0, 255);
    watermarkTransparencyControl.action = function () { pageSample.update(); }
    watermarkTable.addCellInLastRow(watermarkTransparencyControl).className = "stiDesignerControlCellsBigIntervals";

    //AspectRatio
    pageSetupForm.watermarkRows["AspectRatio"] = watermarkTable.addRow();
    var watermarkAspectRatioCell = watermarkTable.addCellInLastRow();
    watermarkAspectRatioCell.className = "stiDesignerCaptionControlsBigIntervals";
    watermarkAspectRatioCell.style.paddingTop = "8px";
    watermarkAspectRatioCell.style.paddingBottom = "6px";
    watermarkAspectRatioCell.setAttribute("colspan", "2");

    var watermarkAspectRatioControl = this.CheckBox("pageSetupWatermarkAspectRatio", this.loc.PropertyMain.AspectRatio);
    watermarkAspectRatioControl.action = function () { pageSample.update(); }
    watermarkAspectRatioCell.appendChild(watermarkAspectRatioControl);

    //Show Image Behind
    pageSetupForm.watermarkRows["ShowImageBehind"] = watermarkTable.addRow();
    var watermarkShowImgBehindCell = watermarkTable.addCellInLastRow();
    watermarkShowImgBehindCell.className = "stiDesignerCaptionControlsBigIntervals";
    watermarkShowImgBehindCell.style.paddingTop = "6px";
    watermarkShowImgBehindCell.style.paddingBottom = "6px";
    watermarkShowImgBehindCell.setAttribute("colspan", "2");

    var watermarkShowImgBehindControl = this.CheckBox("pageSetupWatermarkShowImageBehind", this.loc.PropertyMain.ShowImageBehind);
    watermarkShowImgBehindControl.action = function () { pageSample.update(); }
    watermarkShowImgBehindCell.appendChild(watermarkShowImgBehindControl);

    //Image Stretch
    pageSetupForm.watermarkRows["ImageStretch"] = watermarkTable.addRow();
    var watermarkStretchCell = watermarkTable.addCellInLastRow();
    watermarkStretchCell.className = "stiDesignerCaptionControlsBigIntervals";
    watermarkStretchCell.style.paddingTop = "6px";
    watermarkStretchCell.style.paddingBottom = "6px";
    watermarkStretchCell.setAttribute("colspan", "2");

    var watermarkStretchControl = this.CheckBox("pageSetupWatermarkImageStretch", this.loc.PropertyMain.ImageStretch);
    watermarkStretchControl.action = function () { pageSample.update(); }
    watermarkStretchCell.appendChild(watermarkStretchControl);

    //Image Tiling
    pageSetupForm.watermarkRows["ImageTiling"] = watermarkTable.addRow();
    var watermarkTilingCell = watermarkTable.addCellInLastRow();
    watermarkTilingCell.className = "stiDesignerCaptionControlsBigIntervals";
    watermarkTilingCell.style.paddingTop = "6px";
    watermarkTilingCell.style.paddingBottom = "6px";
    watermarkTilingCell.setAttribute("colspan", "2");

    var watermarkTilingControl = this.CheckBox("pageSetupWatermarkImageTiling", this.loc.PropertyMain.ImageTiling);
    watermarkTilingControl.action = function () { pageSample.update(); }
    watermarkTilingCell.appendChild(watermarkTilingControl);

    //More   
    watermarkTable.addRow();
    var moreButton = this.FormButton(null, "pageSetupWatermarkMoreOptions", watermarkBlock.showAllOptions ? this.loc.Buttons.LessOptions : this.loc.Buttons.MoreOptions);
    moreButton.style.display = "inline-block";
    moreButton.style.margin = "12px 7px 4px 8px";
    moreButton.style.minWidth = "120px";
    var watermarkTextControlCell = watermarkTable.addCellInLastRow(moreButton);
    watermarkTextControlCell.setAttribute("colspan", "2");
    watermarkTextControlCell.style.textAlign = "right";

    moreButton.action = function () {
        pageSetupForm.changeVisibleStateMoreOptions(!watermarkBlock.showAllOptions);
        jsObject.SetObjectToCenter(pageSetupForm);
    }

    pageSetupForm.changeVisibleStateMoreOptions = function (state) {
        watermarkBlock.showAllOptions = state;
        for (var name in pageSetupForm.watermarkRows) {
            pageSetupForm.watermarkRows[name].style.display = state ? "" : "none";
        }
        pageSetupForm.watermarkRows["EnabledExpression"].style.display = state && watermarkEnabledControl.key == "Expression" ? "" : "none";
        moreButton.caption.innerHTML = state ? jsObject.loc.Buttons.LessOptions : jsObject.loc.Buttons.MoreOptions;
    }

    pageSetupForm.changeVisibleStateMoreOptions(watermarkBlock.showAllOptions);

    return watermarkBlock;
}