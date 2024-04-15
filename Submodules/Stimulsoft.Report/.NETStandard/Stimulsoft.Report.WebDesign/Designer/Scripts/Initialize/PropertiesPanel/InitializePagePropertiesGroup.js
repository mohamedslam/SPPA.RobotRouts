
StiMobileDesigner.prototype.PagePropertiesGroup = function () {
    var pagePropertiesGroup = this.PropertiesGroup("pagePropertiesGroup", this.loc.Toolbars.TabPage);
    pagePropertiesGroup.style.display = "none";

    //Page Size
    var pageSizeItems = [];
    for (var sizeNumber in this.options.paperSizes) {
        if (this.options.paperSizes[sizeNumber] != "none")
            pageSizeItems.push(this.Item("pageSizePropertyItem" + sizeNumber, this.options.paperSizes[sizeNumber], null, sizeNumber.toString()));
    }
    var pageSizeControl = this.PropertyDropDownList("controlPropertyPageSize", this.options.propertyControlWidth, pageSizeItems, true, false);
    pageSizeControl.action = function () {
        this.jsObject.options.currentPage.properties.paperSize = this.key;
        if (this.key != "0") {
            var pageWidth = this.jsObject.ConvertPixelToUnit(this.jsObject.PaperSizesInPixels[parseInt(this.key)][0]).toFixed(1);
            var pageHeight = this.jsObject.ConvertPixelToUnit(this.jsObject.PaperSizesInPixels[parseInt(this.key)][1]).toFixed(1);
            if (Math.round(pageWidth) == pageWidth) pageWidth = Math.round(pageWidth);
            if (Math.round(pageHeight) == pageHeight) pageHeight = Math.round(pageHeight);
            var orientation = this.jsObject.options.currentPage.properties.orientation;
            this.jsObject.options.currentPage.properties.unitWidth = (orientation == "Portrait") ? pageWidth.toString() : pageHeight.toString();
            this.jsObject.options.currentPage.properties.unitHeight = (orientation == "Portrait") ? pageHeight.toString() : pageWidth.toString();
        }
        this.jsObject.SendCommandSendProperties(this.jsObject.options.currentPage, ["paperSize", "unitWidth", "unitHeight"]);
    }
    pagePropertiesGroup.container.appendChild(this.Property("paperSize", this.loc.PropertyMain.PaperSize, pageSizeControl));

    //Page Width
    var pageWidthControl = this.PropertyTextBox("controlPropertyPageWidth", this.options.propertyNumbersControlWidth);
    pageWidthControl.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.options.currentPage.properties.unitWidth = this.value;
        this.jsObject.SendCommandSendProperties(this.jsObject.options.currentPage, ["unitWidth"]);
    }
    pagePropertiesGroup.container.appendChild(this.Property("pageWidth", this.loc.PropertyMain.PageWidth, pageWidthControl));

    //Page Height
    var pageHeightControl = this.PropertyTextBox("controlPropertyPageHeight", this.options.propertyNumbersControlWidth);
    pageHeightControl.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.options.currentPage.properties.unitHeight = this.value;
        this.jsObject.SendCommandSendProperties(this.jsObject.options.currentPage, ["unitHeight"]);
    }
    pagePropertiesGroup.container.appendChild(this.Property("pageHeight", this.loc.PropertyMain.PageHeight, pageHeightControl));

    //Page Orientation
    var pageOrientationControl = this.PropertyDropDownList("controlPropertyPageOrientation", this.options.propertyControlWidth, this.GetPageOrientationItems(false), true, false);
    pageOrientationControl.action = function () {
        this.jsObject.options.currentPage.properties.orientation = this.key;
        this.jsObject.SendCommandSendProperties(this.jsObject.options.currentPage, ["orientation"]);
    }
    pagePropertiesGroup.container.appendChild(this.Property("pageOrientation", this.loc.PropertyMain.Orientation, pageOrientationControl, "Orientation"));

    //Page Margins
    var pageMarginsControl = this.PropertyMarginsControl("controlPropertyPageMargins", this.options.propertyControlWidth + 61);
    pageMarginsControl.action = function () {
        this.jsObject.options.currentPage.properties.unitMargins = this.getValue(true);
        this.jsObject.SendCommandSendProperties(this.jsObject.options.currentPage, ["unitMargins"]);
    }
    pagePropertiesGroup.container.appendChild(this.Property("pageMargins", this.loc.PropertyMain.Margins, pageMarginsControl, "Margins"));

    //NumberOfCopies
    var pageNumberOfCopiesControl = this.PropertyTextBox("controlPropertyPageNumberOfCopies", this.options.propertyNumbersControlWidth);
    pageNumberOfCopiesControl.action = function () {
        this.value = Math.abs(this.jsObject.StrToInt(this.value));
        this.jsObject.ApplyPropertyValue("numberOfCopies", this.value);
    }
    pagePropertiesGroup.container.appendChild(this.Property("pageNumberOfCopies", this.loc.PropertyMain.NumberOfCopies, pageNumberOfCopiesControl));

    //Watermark Button
    var watermarkButtonBlock = this.PropertyBlockWithButton("propertiesWatermarkButtonBlock", "PageWatermark.png", this.loc.PropertyMain.Watermark + "...");
    pagePropertiesGroup.container.appendChild(watermarkButtonBlock);

    watermarkButtonBlock.button.action = function () {
        this.jsObject.InitializePageSetupForm(function (pageSetupForm) {
            pageSetupForm.changeVisibleState(true);
            pageSetupForm.setMode("Watermark");
        });
    }

    return pagePropertiesGroup;
}