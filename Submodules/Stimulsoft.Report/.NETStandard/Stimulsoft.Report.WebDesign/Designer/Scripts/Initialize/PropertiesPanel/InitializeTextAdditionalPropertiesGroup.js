
StiMobileDesigner.prototype.TextAdditionalPropertiesGroup = function () {
    var jsObject = this
    var textAdditionalPropertiesGroup = this.PropertiesGroup("textAdditionalPropertiesGroup", this.loc.PropertyCategory.TextAdditionalCategory);
    textAdditionalPropertiesGroup.style.display = "none";

    //AllowHtmlTags
    var controlPropertyAllowHtmlTags = this.CheckBox("controlPropertyAllowHtmlTags");
    controlPropertyAllowHtmlTags.action = function () {
        jsObject.ApplyPropertyValue("allowHtmlTags", this.isChecked);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("allowHtmlTags", this.loc.PropertyMain.AllowHtmlTags, controlPropertyAllowHtmlTags));

    //Text Angle
    var controlPropertyTextAngle = this.PropertyTextBox("controlPropertyTextAngle", this.options.propertyNumbersControlWidth);
    controlPropertyTextAngle.action = function () {
        this.value = Math.abs(jsObject.StrToInt(this.value));
        jsObject.ApplyPropertyValue("textAngle", this.value);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("textAngle", this.loc.PropertyMain.Angle, controlPropertyTextAngle, "Angle"));

    //Continuous Text
    var controlPropertyContinuousText = this.CheckBox("controlPropertyContinuousText");
    controlPropertyContinuousText.action = function () {
        jsObject.ApplyPropertyValue("continuousText", this.isChecked);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("continuousText", this.loc.PropertyMain.ContinuousText, controlPropertyContinuousText));

    //Detect Urls
    var controlPropertyDetectUrls = this.CheckBox("controlPropertyDetectUrls");
    controlPropertyDetectUrls.action = function () {
        jsObject.ApplyPropertyValue("detectUrls", this.isChecked);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("detectUrls", this.loc.PropertyMain.DetectUrls, controlPropertyDetectUrls));

    //EditableText
    var controlPropertyEditableText = this.CheckBox("controlPropertyEditableText");
    controlPropertyEditableText.action = function () {
        jsObject.ApplyPropertyValue("editableText", this.isChecked);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("editableText", this.loc.PropertyMain.Editable, controlPropertyEditableText, "Editable"));

    //Hide Zeros
    var controlPropertyHideZeros = this.CheckBox("controlPropertyHideZeros");
    controlPropertyHideZeros.action = function () {
        jsObject.ApplyPropertyValue("hideZeros", this.isChecked);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("hideZeros", this.loc.PropertyMain.HideZeros, controlPropertyHideZeros));

    //Lines Of Underline
    var controlPropertyLinesOfUnderline = this.PropertyDropDownList("controlPropertyLinesOfUnderline", this.options.propertyControlWidth, this.GetBorderStyleItems(), true, true);
    controlPropertyLinesOfUnderline.action = function () {
        this.jsObject.ApplyPropertyValue("linesOfUnderline", this.key);
    };
    textAdditionalPropertiesGroup.container.appendChild(this.Property("linesOfUnderline", this.loc.PropertyMain.LinesOfUnderline, controlPropertyLinesOfUnderline))

    //Line Spacing
    var controlPropertyLineSpacing = this.PropertyTextBox("controlPropertyLineSpacing", this.options.propertyNumbersControlWidth);
    controlPropertyLineSpacing.action = function () {
        this.value = Math.abs(jsObject.StrToDouble(this.value));
        jsObject.ApplyPropertyValue("lineSpacing", this.value);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("lineSpacing", this.loc.PropertyMain.LineSpacing, controlPropertyLineSpacing));

    //Text Margins
    var controlPropertyTextMargins = this.PropertyMarginsControl("controlPropertyTextMargins", this.options.propertyControlWidth + 61);
    controlPropertyTextMargins.action = function () {
        jsObject.ApplyPropertyValue("textMargins", this.getValue());
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("textMargins", this.loc.PropertyMain.Margins, controlPropertyTextMargins, "Margins"));

    //MaxNumberOfLines
    var controlPropertyMaxNumberOfLines = this.PropertyTextBox("controlPropertyMaxNumberOfLines", this.options.propertyNumbersControlWidth);
    controlPropertyMaxNumberOfLines.action = function () {
        this.value = Math.abs(jsObject.StrToInt(this.value));
        jsObject.ApplyPropertyValue("maxNumberOfLines", this.value);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("maxNumberOfLines", this.loc.PropertyMain.MaxNumberOfLines, controlPropertyMaxNumberOfLines));

    //Only Text
    var controlPropertyOnlyText = this.CheckBox("controlPropertyOnlyText");
    controlPropertyOnlyText.action = function () {
        jsObject.ApplyPropertyValue("onlyText", this.isChecked);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("onlyText", this.loc.PropertyMain.OnlyText, controlPropertyOnlyText));

    //ProcessAt
    var controlPropertyProcessAt = this.PropertyDropDownList("controlPropertyProcessAt", this.options.propertyControlWidth, this.GetProcessAtItems(), true, false);
    controlPropertyProcessAt.action = function () {
        jsObject.ApplyPropertyValue("processAt", this.key);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("processAt", this.loc.PropertyMain.ProcessAt, controlPropertyProcessAt));

    //ProcessingDuplicates
    var controlPropertyProcessingDuplicates = this.PropertyDropDownList("controlPropertyProcessingDuplicates", this.options.propertyControlWidth, this.GetProcessingDuplicatesItems(), true, false);
    controlPropertyProcessingDuplicates.action = function () {
        jsObject.ApplyPropertyValue("processingDuplicates", this.key);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("processingDuplicates", this.loc.PropertyMain.ProcessingDuplicates, controlPropertyProcessingDuplicates));

    //RenderTo
    var controlPropertyRenderTo = this.PropertyDropDownList("controlPropertyRenderTo", this.options.propertyControlWidth, null, true, false);
    controlPropertyRenderTo.action = function () {
        jsObject.ApplyPropertyValue("renderTo", this.key);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("renderTo", this.loc.PropertyMain.RenderTo, controlPropertyRenderTo));

    //RightToLeftText
    var controlPropertyRightToLeftText = this.CheckBox("controlPropertyRightToLeftText");
    controlPropertyRightToLeftText.action = function () {
        jsObject.ApplyPropertyValue("rightToLeft", this.isChecked);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("rightToLeftText", this.loc.PropertyMain.RightToLeft, controlPropertyRightToLeftText, "RightToLeft"));

    //TextOptionsRightToLeft
    var controlPropertyTextOptionsRightToLeft = this.CheckBox("controlPropertyTextOptionsRightToLeft");
    controlPropertyTextOptionsRightToLeft.action = function () {
        jsObject.ApplyPropertyValue("textOptionsRightToLeft", this.isChecked);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("textOptionsRightToLeft", this.loc.PropertyMain.RightToLeft, controlPropertyTextOptionsRightToLeft, "RightToLeft"));

    //ShrinkFontToFit
    var controlPropertyShrinkFontToFit = this.CheckBox("controlPropertyShrinkFontToFit");
    controlPropertyShrinkFontToFit.action = function () {
        jsObject.ApplyPropertyValue("shrinkFontToFit", this.isChecked);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("shrinkFontToFit", this.loc.PropertyMain.ShrinkFontToFit, controlPropertyShrinkFontToFit));

    //ShrinkFontToFitMinimumSize
    var controlPropertyShrinkFontToFitMinimumSize = this.PropertyTextBox("controlPropertyShrinkFontToFitMinimumSize", this.options.propertyNumbersControlWidth);
    controlPropertyShrinkFontToFitMinimumSize.action = function () {
        this.value = Math.abs(jsObject.StrToDouble(this.value));
        jsObject.ApplyPropertyValue("shrinkFontToFitMinimumSize", this.value);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("shrinkFontToFitMinimumSize", this.loc.PropertyMain.ShrinkFontToFitMinimumSize, controlPropertyShrinkFontToFitMinimumSize));

    //Trimming
    var controlPropertyTrimming = this.PropertyDropDownList("controlPropertyTrimming", this.options.propertyControlWidth, this.GetTrimmingItems(), true, false);
    controlPropertyTrimming.action = function () {
        jsObject.ApplyPropertyValue("trimming", this.key);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("trimming", this.loc.PropertyMain.Trimming, controlPropertyTrimming));

    //Word Wrap
    var controlPropertyWordWrap = this.CheckBox("controlPropertyWordWrap");
    controlPropertyWordWrap.action = function () {
        jsObject.ApplyPropertyValue("wordWrap", this.isChecked);
    }
    textAdditionalPropertiesGroup.container.appendChild(this.Property("wordWrap", this.loc.PropertyMain.WordWrap, controlPropertyWordWrap));

    return textAdditionalPropertiesGroup;
}