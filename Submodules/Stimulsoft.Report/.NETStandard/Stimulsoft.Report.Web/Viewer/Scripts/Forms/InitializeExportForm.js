
StiJsViewer.prototype.InitializeExportForm = function () {
    var jsObject = this;
    var exportForm = this.BaseForm("exportForm", this.collections.loc["ExportFormTitle"], 1);
    exportForm.style.fontFamily = this.options.toolbar.fontFamily;
    if (this.options.toolbar.fontColor != "") exportForm.style.color = this.options.toolbar.fontColor;
    exportForm.style.fontSize = "12px";
    exportForm.controls = {};
    exportForm.labels = {};
    exportForm.container.style.padding = "12px 0 0 0";

    exportForm.addControlToParentControl = function (label, control, parentControl, name) {
        if (parentControl.innerTable == null) {
            parentControl.innerTable = jsObject.CreateHTMLTable();
            parentControl.innerTable.style.width = "100%";
            parentControl.appendChild(parentControl.innerTable);
        }
        control.parentRow = parentControl.innerTable.addRow();
        var cellForLabel = parentControl.innerTable.addCellInLastRow();
        var cellForControl = (label != null) ? parentControl.innerTable.addCellInLastRow() : cellForLabel;
        if (label != null) {
            cellForLabel.style.padding = "0 8px 0 8px";
            cellForLabel.style.minWidth = "150px";
            if (label) cellForLabel.innerHTML = label;
            exportForm.labels[name] = cellForLabel;
            var tooltip = control.getAttribute("title");
            if (tooltip != null) cellForLabel.setAttribute("title", tooltip);
        }
        else {
            cellForControl.setAttribute("colspan", "2");
        }
        cellForControl.appendChild(control);
    }

    var mrgn = "8px";

    //0-name, 1-label, 2-control, 3-parentControlName, 4-margin
    var controlProps = [
        ["SavingReportGroup", null, this.GroupPanel(this.collections.loc["SavingReport"], true, 390, "6px 0 6px 0"), null, "0 12px 12px 12px"],
        ["SaveReportMdc", null, this.RadioButton(exportForm.name + "SaveReportMdc", exportForm.name + "SavingReportGroup", this.collections.loc["SaveReportMdc"], null), "SavingReportGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["SaveReportMdz", null, this.RadioButton(exportForm.name + "SaveReportMdz", exportForm.name + "SavingReportGroup", this.collections.loc["SaveReportMdz"], null), "SavingReportGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["SaveReportMdx", null, this.RadioButton(exportForm.name + "SaveReportMdx", exportForm.name + "SavingReportGroup", this.collections.loc["SaveReportMdx"], null), "SavingReportGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["SaveReportPassword", this.collections.loc["PasswordSaveReport"], this.TextBox(null, 140, this.collections.loc["PasswordSaveReportTooltip"]), "SavingReportGroup.container", "4px " + mrgn + " 0px " + mrgn],
        ["PageRangeGroup", null, this.GroupPanel(this.collections.loc["PagesRange"], true, 390, "6px 0 6px 0"), null, "0 12px 12px 12px"],
        ["PageRangeAll", null, this.RadioButton(exportForm.name + "PagesRangeAll", exportForm.name + "PageRangeGroup", this.collections.loc["PagesRangeAll"], this.collections.loc["PagesRangeAllTooltip"]), "PageRangeGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["PageRangeCurrentPage", null, this.RadioButton(exportForm.name + "PagesRangeCurrentPage", exportForm.name + "PageRangeGroup", this.collections.loc["PagesRangeCurrentPage"], this.collections.loc["PagesRangeCurrentPageTooltip"]), "PageRangeGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["PageRangePages", null, this.RadioButton(exportForm.name + "PagesRangePages", exportForm.name + "PageRangeGroup", this.collections.loc["PagesRangePages"], this.collections.loc["PagesRangePagesTooltip"]), "PageRangeGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["PageRangePagesText", null, this.TextBox(null, 130, this.collections.loc["PagesRangePagesTooltip"]), "PageRangePages.lastCell", "0 0 0 30px"],
        ["SettingsGroup", null, this.GroupPanel(this.collections.loc["SettingsGroup"], true, 390, "6px 0 6px 0"), null, "0 12px 12px 12px"],
        ["ImageType", this.collections.loc["Type"], this.DropDownListForExportForm(null, 160, this.collections.loc["TypeTooltip"], this.GetImageTypesItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["DataType", this.collections.loc["Type"], this.DropDownListForExportForm(null, 160, this.collections.loc["TypeTooltip"], this.GetDataTypesItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["ExcelType", this.collections.loc["Type"], this.DropDownListForExportForm(null, 160, this.collections.loc["TypeTooltip"], this.GetExcelTypesItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["HtmlType", this.collections.loc["Type"], this.DropDownListForExportForm(null, 160, this.collections.loc["TypeTooltip"], this.GetHtmlTypesItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["Zoom", this.collections.loc["ZoomHtml"], this.DropDownListForExportForm(null, 160, this.collections.loc["ZoomHtmlTooltip"], this.GetZoomItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["ImageFormatForHtml", this.collections.loc["ImageFormatForHtml"], this.DropDownListForExportForm(null, 160, this.collections.loc["ImageFormatForHtmlTooltip"], this.GetImageFormatForHtmlItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["ExportMode", this.collections.loc["ExportMode"], this.DropDownListForExportForm(null, 160, this.collections.loc["ExportModeTooltip"], this.GetExportModeItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["UseEmbeddedImages", null, this.CheckBox(null, this.collections.loc["EmbeddedImageData"], this.collections.loc["EmbeddedImageDataTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["AddPageBreaks", null, this.CheckBox(null, this.collections.loc["AddPageBreaks"], this.collections.loc["AddPageBreaksTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["ImageResolution", this.collections.loc["ImageResolution"], this.DropDownListForExportForm(null, 160, this.collections.loc["ImageResolutionTooltip"], this.GetImageResolutionItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["ImageCompressionMethod", this.collections.loc["ImageCompressionMethod"], this.DropDownListForExportForm(null, 160, this.collections.loc["ImageCompressionMethodTooltip"], this.GetImageCompressionMethodItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["ImageResolutionMode", this.collections.loc["ImageResolutionMode"], this.DropDownListForExportForm(null, 160, this.collections.loc["ImageResolutionModeTooltip"], this.GetImageResolutionModeItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["AllowEditable", this.collections.loc["AllowEditable"], this.DropDownListForExportForm(null, 160, this.collections.loc["AllowEditableTooltip"], this.GetAllowEditableItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["ImageQuality", this.collections.loc["ImageQuality"], this.DropDownListForExportForm(null, 160, this.collections.loc["ImageQualityTooltip"], this.GetImageQualityItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["ContinuousPages", null, this.CheckBox(null, this.collections.loc["ContinuousPages"], this.collections.loc["ContinuousPagesTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["StandardPdfFonts", null, this.CheckBox(null, this.collections.loc["StandardPDFFonts"], this.collections.loc["StandardPDFFontsTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["EmbeddedFonts", null, this.CheckBox(null, this.collections.loc["EmbeddedFonts"], this.collections.loc["EmbeddedFontsTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["UseUnicode", null, this.CheckBox(null, this.collections.loc["UseUnicode"], this.collections.loc["UseUnicodeTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["Compressed", null, this.CheckBox(null, this.collections.loc["Compressed"], this.collections.loc["CompressedTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["ExportRtfTextAsImage", null, this.CheckBox(null, this.collections.loc["ExportRtfTextAsImage"], this.collections.loc["ExportRtfTextAsImageTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["PdfACompliance", null, this.CheckBox(null, this.collections.loc["PdfACompliance"], this.collections.loc["PdfAComplianceTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["KillSpaceLines", null, this.CheckBox(null, this.collections.loc["KillSpaceLines"], this.collections.loc["KillSpaceLinesTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["PutFeedPageCode", null, this.CheckBox(null, this.collections.loc["PutFeedPageCode"], this.collections.loc["PutFeedPageCodeTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["DrawBorder", null, this.CheckBox(null, this.collections.loc["DrawBorder"], this.collections.loc["DrawBorderTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["CutLongLines", null, this.CheckBox(null, this.collections.loc["CutLongLines"], this.collections.loc["CutLongLinesTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["BorderType", this.collections.loc["BorderType"], this.DropDownListForExportForm(null, 160, this.collections.loc["BorderTypeTooltip"], this.GetBorderTypeItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["ZoomX", this.collections.loc["ZoomXY"] ? this.collections.loc["ZoomXY"].replace(":", "") + " X " : "", this.DropDownListForExportForm(null, 160, this.collections.loc["ZoomXYTooltip"], this.GetZoomItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["ZoomY", this.collections.loc["ZoomXY"] ? this.collections.loc["ZoomXY"].replace(":", "") + " Y " : "", this.DropDownListForExportForm(null, 160, this.collections.loc["ZoomXYTooltip"], this.GetZoomItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["EncodingTextOrCsvFile", this.collections.loc["EncodingData"], this.DropDownListForExportForm(null, 160, this.collections.loc["EncodingDataTooltip"], this.GetEncodingDataItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["ImageFormat", this.collections.loc["ImageFormat"], this.DropDownListForExportForm(null, 160, this.collections.loc["ImageFormatTooltip"], this.GetImageFormatItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["DitheringType", this.collections.loc["MonochromeDitheringType"], this.DropDownListForExportForm(null, 160, this.collections.loc["MonochromeDitheringTypeTooltip"], this.GetMonochromeDitheringTypeItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["TiffCompressionScheme", this.collections.loc["TiffCompressionScheme"], this.DropDownListForExportForm(null, 160, this.collections.loc["TiffCompressionSchemeTooltip"], this.GetTiffCompressionSchemeItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["CompressToArchive", null, this.CheckBox(null, this.collections.loc["CompressToArchive"], this.collections.loc["CompressToArchiveTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["CutEdges", null, this.CheckBox(null, this.collections.loc["CutEdges"], this.collections.loc["CutEdgesTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["MultipleFiles", null, this.CheckBox(null, this.collections.loc["MultipleFiles"], this.collections.loc["MultipleFilesTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["ExportDataOnly", null, this.CheckBox(null, this.collections.loc["ExportDataOnly"], this.collections.loc["ExportDataOnlyTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["UseDefaultSystemEncoding", null, this.CheckBox(null, this.collections.loc["UseDefaultSystemEncoding"], this.collections.loc["UseDefaultSystemEncodingTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["EncodingDifFile", this.collections.loc["EncodingDifFile"], this.DropDownListForExportForm(null, 160, this.collections.loc["EncodingDifFileTooltip"], this.GetEncodingDifFileItems(), true), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["ExportModeRtf", this.collections.loc["ExportModeRtf"], this.DropDownListForExportForm(null, 160, this.collections.loc["ExportModeRtfTooltip"], this.GetExportModeRtfItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["UsePageHeadersAndFooters", null, this.CheckBox(null, this.collections.loc["UsePageHeadersFooters"], this.collections.loc["UsePageHeadersFootersTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["RemoveEmptySpaceAtBottom", null, this.CheckBox(null, this.collections.loc["RemoveEmptySpace"], this.collections.loc["RemoveEmptySpaceTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["Separator", this.collections.loc["Separator"], this.TextBox(null, 160, this.collections.loc["SeparatorTooltip"]), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["DataExportMode", this.collections.loc["BandsFilter"], this.DropDownListForExportForm(null, 160, this.collections.loc["BandsFilterTooltip"], this.GetDataExportModeItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["SkipColumnHeaders", null, this.CheckBox(null, this.collections.loc["SkipColumnHeaders"], this.collections.loc["SkipColumnHeadersTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["ExportObjectFormatting", null, this.CheckBox(null, this.collections.loc["ExportObjectFormatting"], this.collections.loc["ExportObjectFormattingTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["UseOnePageHeaderAndFooter", null, this.CheckBox(null, this.collections.loc["UseOnePageHeaderFooter"], this.collections.loc["UseOnePageHeaderFooterTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["ExportEachPageToSheet", null, this.CheckBox(null, this.collections.loc["ExportEachPageToSheet"], this.collections.loc["ExportEachPageToSheetTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["ExportPageBreaks", null, this.CheckBox(null, this.collections.loc["ExportPageBreaks"], this.collections.loc["ExportPageBreaksTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["EncodingDbfFile", this.collections.loc["EncodingDbfFile"], this.DropDownListForExportForm(null, 160, this.collections.loc["EncodingDbfFileTooltip"], this.GetEncodingDbfFileItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["UseDigitalSignature", " ", this.CheckBox(null, this.collections.loc["DigitalSignatureButton"], this.collections.loc["UseDigitalSignatureTooltip"]), "SettingsGroup.container", "6px " + mrgn + " 6px " + mrgn],
        ["CertificateThumbprint", null, this.DropDownListForExportForm(null, 160, null, this.GetPdfSecurityCertificatesItems(), true), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["DocumentSecurityButton", " ", this.SmallButton(null, this.collections.loc["DocumentSecurityButton"], null, null, "Down", "stiJsViewerFormButton"), "SettingsGroup.container", "4px " + mrgn + " 4px " + mrgn],
        ["OpenAfterExport", null, this.CheckBox(null, this.collections.loc["OpenAfterExport"], this.collections.loc["OpenAfterExportTooltip"]), null, "6px 0 6px 12px"],
        ["DocumentSecurityMenu", null, this.BaseMenu(exportForm.name + "DocumentSecurityMenu", null, "Down", "stiJsViewerDropdownPanel"), null, null],
        ["PasswordInputUser", this.collections.loc["UserPassword"], this.TextBox(null, 160, this.collections.loc["UserPasswordTooltip"]), "DocumentSecurityMenu.innerContent", "8px " + mrgn + " 4px " + mrgn],
        ["PasswordInputOwner", this.collections.loc["OwnerPassword"], this.TextBox(null, 160, this.collections.loc["OwnerPasswordTooltip"]), "DocumentSecurityMenu.innerContent", "4px " + mrgn + " 4px " + mrgn],
        ["PrintDocument", null, this.CheckBox(null, this.collections.loc["AllowPrintDocument"], this.collections.loc["AllowPrintDocumentTooltip"]), "DocumentSecurityMenu.innerContent", "6px " + mrgn + " 6px " + mrgn],
        ["ModifyContents", null, this.CheckBox(null, this.collections.loc["AllowModifyContents"], this.collections.loc["AllowModifyContentsTooltip"]), "DocumentSecurityMenu.innerContent", "6px " + mrgn + " 6px " + mrgn],
        ["CopyTextAndGraphics", null, this.CheckBox(null, this.collections.loc["AllowCopyTextAndGraphics"], this.collections.loc["AllowCopyTextAndGraphicsTooltip"]), "DocumentSecurityMenu.innerContent", "6px " + mrgn + " 6px " + mrgn],
        ["AddOrModifyTextAnnotations", null, this.CheckBox(null, this.collections.loc["AllowAddOrModifyTextAnnotations"], this.collections.loc["AllowAddOrModifyTextAnnotationsTooltip"]), "DocumentSecurityMenu.innerContent", "6px " + mrgn + " 6px " + mrgn],
        ["KeyLength", this.collections.loc["EncryptionKeyLength"], this.DropDownListForExportForm(null, 160, this.collections.loc["EncryptionKeyLengthTooltip"], this.GetEncryptionKeyLengthItems(), true), "DocumentSecurityMenu.innerContent", "4px " + mrgn + " 8px " + mrgn]
    ]

    //Add Controls To Form
    for (var i = 0; i < controlProps.length; i++) {
        var name = controlProps[i][0];
        var label = controlProps[i][1];
        var control = controlProps[i][2];
        var parentControlName = controlProps[i][3];
        exportForm.controls[name] = control;
        if (controlProps[i][4]) control.style.margin = controlProps[i][4];
        if (control.className == "stiJsViewerGroupPanel") control.container.style.paddingBottom = "6px";
        if (name == "DocumentSecurityMenu") continue;

        if (parentControlName != null) {
            var controlNamesArray = parentControlName.split(".");
            var parentControl = exportForm.controls[controlNamesArray[0]];
            if (controlNamesArray.length > 1) {
                for (var k = 1; k < controlNamesArray.length; k++) {
                    if (parentControl) parentControl = parentControl[controlNamesArray[k]]
                }
            }
            if (parentControl) exportForm.addControlToParentControl(label, control, parentControl, name);
            continue;
        }
        exportForm.addControlToParentControl(label, control, exportForm.container, name);
    }

    exportForm.controls.PageRangePages.lastCell.style.paddingLeft = "60px";
    exportForm.controls.DocumentSecurityMenu.parentButton = exportForm.controls.DocumentSecurityButton;

    var textBlock1 = document.createElement("div");
    textBlock1.innerHTML = "%";
    textBlock1.style.display = "inline-block";
    exportForm.controls.ImageQuality.style.display = "inline-block";
    exportForm.controls.ImageQuality.parentElement.appendChild(textBlock1);
    exportForm.controls.ImageQuality.style.verticalAlign = "middle";

    var textBlock2 = document.createElement("div");
    textBlock2.innerHTML = "dpi";
    textBlock2.style.display = "inline-block";
    exportForm.controls.ImageResolution.style.display = "inline-block";
    exportForm.controls.ImageResolution.parentElement.appendChild(textBlock2);
    exportForm.controls.ImageResolution.style.verticalAlign = "middle";

    var securButton = exportForm.controls.DocumentSecurityButton;
    securButton.innerTable.style.width = "100%";
    securButton.style.minWidth = "163px";
    securButton.caption.style.textAlign = "center";
    securButton.caption.style.width = "100%";
    securButton.style.display = "inline-block";

    var certificateControl = exportForm.controls.CertificateThumbprint;
    var securControlCell = exportForm.controls.UseDigitalSignature.parentElement;
    var securRow = exportForm.controls.UseDigitalSignature.parentRow
    securRow.firstChild.style.padding = "0px"
    securRow.firstChild.appendChild(exportForm.controls.UseDigitalSignature);
    securControlCell.appendChild(certificateControl);

    for (var itemName in certificateControl.menu.items) {
        var item = certificateControl.menu.items[itemName];
        item.style.height = "auto";
        item.style.lineHeight = "1.3";
        item.caption.style.padding = "8px 20px 8px 8px";
        item.caption.innerHTML = item.caption.innerText;
    }

    //Add Action Methods To Controls
    //Types Controls
    exportForm.controls.ImageType.action = function () {
        exportForm.showControlsByExportFormat("Image" + this.key, true);
    }

    exportForm.controls.DataType.action = function () {
        exportForm.showControlsByExportFormat(this.key, true);
    }

    exportForm.controls.ExcelType.action = function () {
        var exportFormat = this.key == "ExcelBinary" ? "Excel" : this.key;
        exportForm.showControlsByExportFormat(exportFormat, true);
    }

    exportForm.controls.HtmlType.action = function () {
        exportForm.showControlsByExportFormat(this.key, true);
    }

    exportForm.controls.CompressToArchive.action = function () {
        exportForm.controls.PageRangeAll.setEnabled(this.isChecked);
        exportForm.controls[exportForm.controls.PageRangeAll.isEnabled ? "PageRangeAll" : "PageRangeCurrentPage"].setChecked(true);
    }

    //Saving Report
    var controlNames = ["SaveReportMdc", "SaveReportMdz", "SaveReportMdx"];
    for (var i = 0; i < controlNames.length; i++) {
        exportForm.controls[controlNames[i]].controlName = controlNames[i];
        exportForm.controls[controlNames[i]].onChecked = function () {
            if (this.isChecked) { exportForm.controls.SaveReportPassword.setEnabled(this.controlName == "SaveReportMdx"); }
        }
    }
    //PdfACompliance
    exportForm.controls.PdfACompliance.onChecked = function () {
        var controlNames = ["StandardPdfFonts", "EmbeddedFonts", "UseUnicode"];
        if (this.isChecked) exportForm.controls.EmbeddedFonts.setChecked(true);
        for (var i = 0; i < controlNames.length; i++) {
            exportForm.controls[controlNames[i]].setEnabled(!this.isChecked);
        }
    }
    //EmbeddedFonts, UseUnicode
    var controlNames = ["EmbeddedFonts", "UseUnicode"];
    for (var i = 0; i < controlNames.length; i++) {
        exportForm.controls[controlNames[i]].onChecked = function () { if (this.isChecked) exportForm.controls.StandardPdfFonts.setChecked(false); };
    }
    //StandardPdfFonts
    exportForm.controls.StandardPdfFonts.onChecked = function () {
        if (!this.isChecked) return;
        var controlNames = ["EmbeddedFonts", "UseUnicode"];
        for (var i = 0; i < controlNames.length; i++) { exportForm.controls[controlNames[i]].setChecked(false); }
    }
    //ImageCompressionMethod
    exportForm.controls.ImageCompressionMethod.onChange = function () {
        exportForm.controls.ImageQuality.setEnabled(this.key == "Jpeg");
    }
    //ExportDataOnly
    exportForm.controls.ExportDataOnly.onChecked = function () {
        exportForm.controls.ExportObjectFormatting.setEnabled(this.isChecked);
        exportForm.controls.UseOnePageHeaderAndFooter.setEnabled(!this.isChecked);
    }
    //DataExportMode
    exportForm.controls.DataExportMode.onChange = function () {
        exportForm.controls.ExportObjectFormatting.setEnabled(this.key != "AllBands");
        exportForm.controls.UseOnePageHeaderAndFooter.setEnabled(this.key == "AllBands");
    }
    //UseDefaultSystemEncoding
    exportForm.controls.UseDefaultSystemEncoding.onChecked = function () {
        exportForm.controls.EncodingDifFile.setEnabled(!this.isChecked);
    }
    //UsePageHeadersAndFooters
    exportForm.controls.UsePageHeadersAndFooters.onChecked = function () {
        exportForm.controls.RemoveEmptySpaceAtBottom.setEnabled(!this.isChecked);
        if (!exportForm.controls.RemoveEmptySpaceAtBottom.isEnabled)
            exportForm.controls.RemoveEmptySpaceAtBottom.setChecked(true);
    }
    //ImageType
    exportForm.controls.ImageType.onChange = function () {
        var items = jsObject.GetImageFormatItems(this.key == "Emf");
        exportForm.controls.ImageFormat.menu.addItems(items);
        exportForm.controls.TiffCompressionScheme.setEnabled(this.key == "Tiff");
        exportForm.controls.MultipleFiles.setEnabled(this.key == "Tiff");
        exportForm.controls.CompressToArchive.action();
    }
    //MultipleFiles
    exportForm.controls.MultipleFiles.onChecked = function () {
        if (this.isChecked) {
            exportForm.controls.CompressToArchive.setChecked(true);
            exportForm.controls.CompressToArchive.action();
        }
    }
    //ImageFormat
    exportForm.controls.ImageFormat.onChange = function () {
        exportForm.controls.DitheringType.setEnabled(this.key == "Monochrome");
    }
    //DocumentSecurityButton
    exportForm.controls.DocumentSecurityButton.action = function () {
        jsObject.controls.menus[exportForm.name + "DocumentSecurityMenu"].changeVisibleState(!this.isSelected);
    }

    //UseDigitalSignature
    exportForm.controls.UseDigitalSignature.action = function () {
        if (jsObject.collections.pdfSecurityCertificates && jsObject.collections.pdfSecurityCertificates.length == 0) {
            var errorForm = jsObject.controls.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
            errorForm.show("Certificate Not Found!", "Warning");
            this.setChecked(false);
        }
    }

    exportForm.controls.UseDigitalSignature.onChecked = function () {
        if (jsObject.collections.pdfSecurityCertificates && jsObject.collections.pdfSecurityCertificates.length > 0) {
            exportForm.controls.CertificateThumbprint.setEnabled(this.isChecked);
        }
    }

    //Form Methods
    exportForm.setControlsValue = function (exportSettings, ignoreTypeControls) {
        var defaultExportSettings = exportSettings || jsObject.getDefaultExportSettings(exportForm.exportFormat);
        if (!defaultExportSettings) return;
        var exportControlNames = exportForm.getExportControlNames();

        //Reset Enabled States for All Controls
        for (var i in exportForm.controls) {
            if (exportForm.controls[i]["setEnabled"] != null) exportForm.controls[i].setEnabled(true);
        }

        for (var propertyName in defaultExportSettings) {
            if (jsObject.isContainted(exportControlNames, propertyName)) {
                if (propertyName == "ImageType" || propertyName == "DataType" || propertyName == "ExcelType" || propertyName == "HtmlType") {
                    if (ignoreTypeControls) continue;

                    switch (propertyName) {
                        case "ImageType":
                            if (!jsObject.options.exports.showExportToImageBmp && defaultExportSettings[propertyName] == "Bmp") defaultExportSettings[propertyName] = "Gif";
                            if (!jsObject.options.exports.showExportToImageGif && defaultExportSettings[propertyName] == "Gif") defaultExportSettings[propertyName] = "Jpeg";
                            if (!jsObject.options.exports.showExportToImageJpeg && defaultExportSettings[propertyName] == "Jpeg") defaultExportSettings[propertyName] = "Pcx";
                            if (!jsObject.options.exports.showExportToImagePcx && defaultExportSettings[propertyName] == "Pcx") defaultExportSettings[propertyName] = "Png";
                            if (!jsObject.options.exports.showExportToImagePng && defaultExportSettings[propertyName] == "Png") defaultExportSettings[propertyName] = "Tiff";
                            if (!jsObject.options.exports.showExportToImageTiff && defaultExportSettings[propertyName] == "Tiff") defaultExportSettings[propertyName] = "Emf";
                            if (!jsObject.options.exports.showExportToImageMetafile && defaultExportSettings[propertyName] == "Emf") defaultExportSettings[propertyName] = "Svg";
                            if (!jsObject.options.exports.showExportToImageSvg && defaultExportSettings[propertyName] == "Svg") defaultExportSettings[propertyName] = "Svgz";
                            if (!jsObject.options.exports.showExportToImageSvgz && defaultExportSettings[propertyName] == "Svgz") defaultExportSettings[propertyName] = "Bmp";
                            break;

                        case "DataType":
                            if (!jsObject.options.exports.showExportToCsv && defaultExportSettings[propertyName] == "Csv") defaultExportSettings[propertyName] = "Dbf";
                            if (!jsObject.options.exports.showExportToDbf && defaultExportSettings[propertyName] == "Dbf") defaultExportSettings[propertyName] = "Xml";
                            if (!jsObject.options.exports.showExportToXml && defaultExportSettings[propertyName] == "Xml") defaultExportSettings[propertyName] = "Dif";
                            if (!jsObject.options.exports.showExportToDif && defaultExportSettings[propertyName] == "Dif") defaultExportSettings[propertyName] = "Sylk";
                            if (!jsObject.options.exports.showExportToSylk && defaultExportSettings[propertyName] == "Sylk") defaultExportSettings[propertyName] = "Csv";
                            if (!jsObject.options.exports.showExportToJson && defaultExportSettings[propertyName] == "Json") defaultExportSettings[propertyName] = "Json";
                            break;

                        case "ExcelType":
                            if (!jsObject.options.exports.showExportToExcel2007 && defaultExportSettings[propertyName] == "Excel2007") defaultExportSettings[propertyName] = "ExcelBinary";
                            if (!jsObject.options.exports.showExportToExcel && defaultExportSettings[propertyName] == "ExcelBinary") defaultExportSettings[propertyName] = "ExcelXml";
                            if (!jsObject.options.exports.showExportToExcelXml && defaultExportSettings[propertyName] == "ExcelXml") defaultExportSettings[propertyName] = "Excel2007";
                            break;

                        case "HtmlType":
                            if (!jsObject.options.exports.showExportToHtml && defaultExportSettings[propertyName] == "Html") defaultExportSettings[propertyName] = "Html5";
                            if (!jsObject.options.exports.showExportToHtml5 && defaultExportSettings[propertyName] == "Html5") defaultExportSettings[propertyName] = "Mht";
                            if (!jsObject.options.exports.showExportToMht && defaultExportSettings[propertyName] == "Mht") defaultExportSettings[propertyName] = "Html";
                            break;
                    }
                }

                var control = exportForm.controls[propertyName];
                exportForm.setDefaultValueToControl(control, defaultExportSettings[propertyName]);
            }
        }

        //Exceptions
        if (exportForm.exportFormat == "Document") exportForm.controls.SaveReportMdc.setChecked(true);
        if (exportForm.exportFormat == "Pdf" && defaultExportSettings.StandardPdfFonts) exportForm.controls.StandardPdfFonts.setChecked(true);
        if (jsObject.isContainted(exportControlNames, "HtmlType") && defaultExportSettings.ImageFormat) exportForm.controls.ImageFormatForHtml.setKey(defaultExportSettings.ImageFormat);
        if (exportForm.exportFormat == "Rtf" && defaultExportSettings.ExportMode) exportForm.controls.ExportModeRtf.setKey(defaultExportSettings.ExportMode);
        if (jsObject.isContainted(exportControlNames, "ImageType") && defaultExportSettings.ImageZoom) exportForm.controls.Zoom.setKey(defaultExportSettings.ImageZoom.toString());
        if (exportForm.exportFormat == "Pdf") {
            var userAccessPrivileges = defaultExportSettings.UserAccessPrivileges;
            exportForm.controls.PrintDocument.setChecked(userAccessPrivileges.indexOf("PrintDocument") != -1 || userAccessPrivileges == "All");
            exportForm.controls.ModifyContents.setChecked(userAccessPrivileges.indexOf("ModifyContents") != -1 || userAccessPrivileges == "All");
            exportForm.controls.CopyTextAndGraphics.setChecked(userAccessPrivileges.indexOf("CopyTextAndGraphics") != -1 || userAccessPrivileges == "All");
            exportForm.controls.AddOrModifyTextAnnotations.setChecked(userAccessPrivileges.indexOf("AddOrModifyTextAnnotations") != -1 || userAccessPrivileges == "All");
            exportForm.controls.CertificateThumbprint.setEnabled(defaultExportSettings.UseDigitalSignature && defaultExportSettings.CertificateThumbprint);
        }
        //Encodings
        if (exportForm.exportFormat == "Difs" || exportForm.exportFormat == "Sylk") exportForm.controls.EncodingDifFile.setKey("437");
        if (exportForm.exportFormat == "Dbf" && defaultExportSettings.CodePage) exportForm.controls.EncodingDbfFile.setKey(defaultExportSettings.CodePage);
        if ((exportForm.exportFormat == "Text" || exportForm.exportFormat == "Csv") && defaultExportSettings.Encoding)
            exportForm.controls.EncodingTextOrCsvFile.setKey(defaultExportSettings.Encoding);

        //PageRange       
        var pageRangeAllIsDisabled = jsObject.isContainted(exportControlNames, "ImageType") && !exportForm.controls.CompressToArchive.isChecked;
        exportForm.controls[!pageRangeAllIsDisabled ? "PageRangeAll" : "PageRangeCurrentPage"].setChecked(true);
        exportForm.controls.PageRangeAll.setEnabled(!pageRangeAllIsDisabled);
    }

    exportForm.onhide = function () {
        jsObject.SetCookie("StimulsoftWebViewerExportSettingsOpeningGroups", JSON.stringify({
            SavingReportGroup: exportForm.controls.SavingReportGroup.isOpened,
            PageRangeGroup: exportForm.controls.PageRangeGroup.isOpened,
            SettingsGroup: exportForm.controls.SettingsGroup.isOpened
        }));

        try {
            exportForm.controls.PasswordInputUser.removeAttribute("type");
            exportForm.controls.PasswordInputOwner.removeAttribute("type");
            exportForm.controls.SaveReportPassword.removeAttribute("type");
        } catch (e) { }
    }

    exportForm.show = function (exportFormat, actionType) {
        exportForm.actionType = actionType;
        exportForm.showControlsByExportFormat(exportFormat || "Pdf");

        if (jsObject.options.exports.storeExportSettings) {
            var exportSettingsStr = jsObject.GetCookie("StimulsoftWebViewerExportSettings" + jsObject.GetCommonExportFormat(exportForm.exportFormat));
            if (exportSettingsStr) {
                var exportSettings = JSON.parse(exportSettingsStr);
                var exportFormat = exportSettings.ImageType || exportSettings.DataType || exportSettings.ExcelType || exportSettings.HtmlType;
                if (exportFormat == "ExcelBinary") exportFormat = "Excel";
                if (exportFormat) exportForm.showControlsByExportFormat(exportSettings.ImageType ? "Image" + exportFormat : exportFormat);
                exportForm.setControlsValue(exportSettings);
            }
        }

        var openingGroupsStr = jsObject.GetCookie("StimulsoftWebViewerExportSettingsOpeningGroups");
        var openingGroups = openingGroupsStr ? JSON.parse(openingGroupsStr) : null;

        exportForm.controls.SavingReportGroup.changeOpeningState(openingGroups ? openingGroups.SavingReportGroup : true);
        exportForm.controls.PageRangeGroup.changeOpeningState(openingGroups ? openingGroups.PageRangeGroup : true);
        exportForm.controls.SettingsGroup.changeOpeningState(openingGroups ? openingGroups.SettingsGroup : false);

        if (jsObject.options.exports.showOpenAfterExport === false) {
            exportForm.controls.OpenAfterExport.parentRow.style.display = "none";
            exportForm.controls.OpenAfterExport.setChecked(jsObject.options.exports.openAfterExport !== false);
        }

        exportForm.hideControlsForJSMode();

        if (jsObject.options.jsMode || jsObject.options.cloudMode) {
            exportForm.controls.UseDigitalSignature.parentRow.style.display = "none";
            exportForm.controls.CertificateThumbprint.parentRow.style.display = "none";
        }

        try {
            exportForm.controls.PasswordInputUser.setAttribute("type", "password");
            exportForm.controls.PasswordInputOwner.setAttribute("type", "password");
            exportForm.controls.SaveReportPassword.setAttribute("type", "password");
        } catch (e) { }

        exportForm.changeVisibleState(true);
    }

    exportForm.hideControlsForJSMode = function () {
        if (jsObject.options.jsMode) {
            exportForm.controls.EncodingTextOrCsvFile.parentRow.style.display = "none";
            exportForm.controls.OpenAfterExport.parentRow.style.display = "none";
            exportForm.controls.OpenAfterExport.setChecked(false);
            exportForm.controls.CompressToArchive.parentRow.style.display = "none";
            exportForm.controls.ImageResolutionMode.parentRow.style.display = "none";
            exportForm.controls.MultipleFiles.parentRow.style.display = "none";
        }
    }

    exportForm.action = function () {
        var exportSettingsObject = exportForm.getExportSettingsObject();
        exportForm.changeVisibleState(false);

        if (jsObject.options.exports.storeExportSettings) {
            jsObject.SetCookie("StimulsoftWebViewerExportSettings" + jsObject.GetCommonExportFormat(exportForm.exportFormat), JSON.stringify(exportSettingsObject));
        }

        if (exportForm.actionType == jsObject.options.actions.exportReport) {
            jsObject.postExport(exportForm.exportFormat, exportSettingsObject);
        }
        else if (jsObject.options.email.showEmailDialog) {
            jsObject.controls.forms.sendEmailForm.show(exportForm.exportFormat, exportSettingsObject);
        }
        else {
            exportSettingsObject["Email"] = jsObject.options.email.defaultEmailAddress;
            exportSettingsObject["Message"] = jsObject.options.email.defaultEmailMessage;
            exportSettingsObject["Subject"] = jsObject.options.email.defaultEmailSubject;
            jsObject.postEmail(exportForm.exportFormat, exportSettingsObject);
        }
    }

    exportForm.showControlsByExportFormat = function (exportFormat, ignoreTypeControls) {
        exportForm.exportFormat = exportFormat;
        for (var controlName in exportForm.controls) {
            var control = exportForm.controls[controlName];
            var exportControlNames = exportForm.getExportControlNames();
            if (control.parentRow) {
                control.parentRow.style.display =
                    (this.actionType == jsObject.options.actions.exportReport || controlName != "OpenAfterExport") &&
                        jsObject.isContainted(exportControlNames, controlName) ? "" : "none";
            }
        }
        exportForm.setControlsValue(null, ignoreTypeControls);
        exportForm.hideControlsForJSMode();
    }


    exportForm.setDefaultValueToControl = function (control, value) {
        if (control["setKey"] != null) control.setKey(value != null ? value.toString() : "");
        else if (control["setChecked"] != null) control.setChecked(value);
        else if (control["value"] != null) control.value = value;
    }

    exportForm.getValueFromControl = function (control) {
        if (control["isEnabled"] == false) return control["setChecked"] != null ? false : null;
        else if (control["setKey"] != null) return control.key;
        else if (control["setChecked"] != null) return control.isChecked;
        else if (control["value"] != null) return control.value;

        return null;
    }

    exportForm.getExportControlNames = function () {
        var controlNames = {
            Document: ["SavingReportGroup", "SaveReportMdc", "SaveReportMdz", "SaveReportMdx", "SaveReportPassword"],
            Pdf: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "ImageResolution", "ImageCompressionMethod",
                "ImageResolutionMode", "ImageQuality", /*"StandardPdfFonts",*/ "EmbeddedFonts", /*"UseUnicode", "Compressed",*/ "ExportRtfTextAsImage", "PdfACompliance", "DocumentSecurityButton",
                "OpenAfterExport", "AllowEditable", "PasswordInputUser", "PasswordInputOwner", "PrintDocument", "ModifyContents", "CopyTextAndGraphics",
                "AddOrModifyTextAnnotations", "KeyLength", "UseDigitalSignature", "CertificateThumbprint"],
            Xps: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "ImageResolution", "ImageQuality", "OpenAfterExport",
                "ExportRtfTextAsImage"],
            Ppt2007: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "ImageResolution", "ImageQuality"],
            Html: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "HtmlType", "Zoom", "ImageFormatForHtml",
                "ExportMode", "UseEmbeddedImages", "CompressToArchive", "AddPageBreaks", "OpenAfterExport"],
            Html5: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "HtmlType", "ImageFormatForHtml", "ImageResolution",
                "ImageQuality", "ContinuousPages", "OpenAfterExport"],
            Mht: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "HtmlType", "Zoom", "ImageFormatForHtml",
                "ExportMode", "AddPageBreaks"],
            Text: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "KillSpaceLines",
                "PutFeedPageCode", "DrawBorder", "CutLongLines", "BorderType", "ZoomX", "ZoomY", "EncodingTextOrCsvFile"],
            Rtf: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "ImageResolution",
                "ImageQuality", "ExportModeRtf", "UsePageHeadersAndFooters", "RemoveEmptySpaceAtBottom"],
            Word2007: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "ImageResolution",
                "ImageQuality", "UsePageHeadersAndFooters", "RemoveEmptySpaceAtBottom"],
            Odt: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "ImageResolution",
                "ImageQuality", "RemoveEmptySpaceAtBottom"],
            Excel: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "ExcelType", "ImageResolution",
                "ImageQuality", "ExportObjectFormatting", "UseOnePageHeaderAndFooter", "ExportEachPageToSheet", "ExportPageBreaks", "DataExportMode"],
            ExcelXml: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "ExcelType"],
            Excel2007: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "ExcelType", "ImageResolution",
                "ImageQuality", "ExportObjectFormatting", "UseOnePageHeaderAndFooter", "ExportEachPageToSheet", "ExportPageBreaks", "DataExportMode"],
            Ods: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "ImageResolution",
                "ImageQuality"],
            Csv: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "DataType", "EncodingTextOrCsvFile",
                "Separator", "SkipColumnHeaders", "DataExportMode"],
            Dbf: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "DataType", "EncodingDbfFile"],
            Dif: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "DataType", "ExportDataOnly",
                "UseDefaultSystemEncoding", "EncodingDifFile"],
            Sylk: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "DataType", "ExportDataOnly",
                "UseDefaultSystemEncoding", "EncodingDifFile"],
            Json: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "DataType", "DataExportMode"],
            Xml: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "DataType", "DataExportMode"],
            ImageBmp: ["PageRangeGroup", "PageRangeAll", "PageRangeCurrentPage", "PageRangePages", "PageRangePagesText", "SettingsGroup", "ImageType", "Zoom", "ImageResolution",
                "ImageFormat", "DitheringType", "TiffCompressionScheme", "CutEdges", "CompressToArchive", "MultipleFiles"]
        }

        controlNames.ImageGif = controlNames.ImageJpeg = controlNames.ImagePcx = controlNames.ImageJpeg = controlNames.ImagePng = controlNames.ImageTiff =
            controlNames.ImageEmf = controlNames.ImageSvg = controlNames.ImageSvgz = controlNames.ImageBmp;
        controlNames.ExcelBinary = controlNames.Excel;

        return controlNames[exportForm.exportFormat];
    }

    exportForm.getExportSettingsObject = function () {
        var exportSettings = {};
        var exportControlNames = exportForm.getExportControlNames();

        for (var i = 0; i < exportControlNames.length; i++) {
            var controls = exportForm.controls;
            var controlName = exportControlNames[i];
            var control = controls[controlName];
            if (control.groupName == exportForm.name + "SavingReportGroup" || control.groupName == exportForm.name + "PageRangeGroup" ||
                controlName == "PageRangePagesText") {
                continue;
            }
            else if (controlName == "SavingReportGroup") {
                exportSettings.Format = controls.SaveReportMdc.isChecked ? "Mdc" : (controls.SaveReportMdz.isChecked ? "Mdz" : "Mdx");
                if (exportSettings.Format == "Mdx") exportSettings.Password = controls.SaveReportPassword.value;
            }
            else if (controlName == "PageRangeGroup") {
                exportSettings.PageRange = controls.PageRangeAll.isChecked ? "All" :
                    (controls.PageRangeCurrentPage.isChecked ? (jsObject.reportParams.pageNumber + 1).toString() : controls.PageRangePagesText.value);
            }
            else if (controlName == "EmbeddedFonts") {
                exportSettings.EmbeddedFonts = control.isChecked;
            }
            else if (controlName == "RemoveEmptySpaceAtBottom") {
                exportSettings.RemoveEmptySpaceAtBottom = control.isChecked;
            }
            else {
                var value = exportForm.getValueFromControl(control);
                if (value != null) exportSettings[controlName] = value;
            }
        }

        //Exceptions
        if (exportForm.exportFormat == "Pdf") {
            exportSettings.UserAccessPrivileges = "";
            var controlNames = ["PrintDocument", "ModifyContents", "CopyTextAndGraphics", "AddOrModifyTextAnnotations"];
            for (var i = 0; i < controlNames.length; i++) {
                if (exportSettings[controlNames[i]]) {
                    if (exportSettings.UserAccessPrivileges != "") exportSettings.UserAccessPrivileges += ", ";
                    exportSettings.UserAccessPrivileges += controlNames[i];
                    delete exportSettings[controlNames[i]];
                }
            }
        }

        if (jsObject.isContainted(exportControlNames, "ImageType")) {
            exportSettings.ImageZoom = exportSettings.Zoom;
            delete exportSettings.Zoom;
        }
        var controlNames = [
            ["ImageFormatForHtml", "ImageFormat"],
            ["EncodingTextOrCsvFile", "Encoding"],
            ["ExportModeRtf", "ExportMode"],
            ["EncodingDifFile", "Encoding"],
            ["EncodingDbfFile", "CodePage"]
        ]
        for (var i = 0; i < controlNames.length; i++) {
            if (exportSettings[controlNames[i][0]] != null) {
                exportSettings[controlNames[i][1]] = exportSettings[controlNames[i][0]];
                delete exportSettings[controlNames[i][0]];
            }
        }

        return exportSettings;
    }
}

StiJsViewer.prototype.GetCommonExportFormat = function (format) {
    if (format == "Html" || format == "Html5" || format == "Mht") return "Html";
    if (format == "Excel" || format == "Excel2007" || format == "ExcelXml") return "Excel";
    if (format == "Csv" || format == "Dbf" || format == "Xml" || format == "Dif" || format == "Sylk" || format == "Json") return "Data";
    if (format == "ImageBmp" || format == "ImageGif" || format == "ImageJpeg" || format == "ImagePcx" || format == "ImagePng" ||
        format == "ImageTiff" || format == "ImageEmf" || format == "ImageSvg" || format == "ImageSvgz") return "Image";

    return format
}

StiJsViewer.prototype.DropDownListForExportForm = function (name, width, toolTip, items, readOnly, showImage) {
    var dropDownList = this.DropDownList(name, width, toolTip, items, readOnly, showImage);

    dropDownList.onChange = function () { };

    dropDownList.setKey = function (key) {
        dropDownList.key = key;
        dropDownList.onChange();
        for (var itemName in dropDownList.items)
            if (key == dropDownList.items[itemName].key) {
                this.textBox.value = dropDownList.items[itemName].caption;
                if (dropDownList.image) StiJsViewer.setImageSource(dropDownList.image, dropDownList.jsObject.options, dropDownList.jsObject.collections, dropDownList.items[itemName].imageName);
                return;
            }
        dropDownList.textBox.value = key.toString();
    }
    if (dropDownList.menu) {
        dropDownList.menu.action = function (menuItem) {
            this.changeVisibleState(false);
            this.dropDownList.key = menuItem.key;
            this.dropDownList.textBox.value = menuItem.caption.innerHTML;
            if (this.dropDownList.image) StiJsViewer.setImageSource(this.dropDownList.image, this.jsObject.options, this.jsObject.collections, menuItem.imageName);
            this.dropDownList.action();
            this.dropDownList.onChange();
        }
    }

    return dropDownList;
}