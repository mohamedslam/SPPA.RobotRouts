
StiMobileDesigner.prototype.PageAdditionalPropertiesGroup = function () {
    var pageAdditionalPropertiesGroup = this.PropertiesGroup("pageAdditionalPropertiesGroup", this.loc.PropertyCategory.PageAdditionalCategory);
    pageAdditionalPropertiesGroup.style.display = "none";

    //StopBeforePrint
    var controlPropertyStopBeforePrint = this.PropertyTextBox("controlPropertyStopBeforePrint", this.options.propertyNumbersControlWidth);
    controlPropertyStopBeforePrint.action = function () {
        this.value = Math.abs(this.jsObject.StrToInt(this.value));
        this.jsObject.ApplyPropertyValue("stopBeforePrint", this.value);
    }
    pageAdditionalPropertiesGroup.container.appendChild(this.Property("stopBeforePrint", this.loc.PropertyMain.StopBeforePrint, controlPropertyStopBeforePrint));

    //TitleBeforeHeader
    var controlPropertyTitleBeforeHeader = this.CheckBox("controlPropertyTitleBeforeHeader");
    controlPropertyTitleBeforeHeader.action = function () {
        this.jsObject.ApplyPropertyValue("titleBeforeHeader", this.isChecked);
    }
    pageAdditionalPropertiesGroup.container.appendChild(this.Property("titleBeforeHeader", this.loc.PropertyMain.TitleBeforeHeader, controlPropertyTitleBeforeHeader));

    //Mirror Margins
    var controlPropertyMirrorMargins = this.CheckBox("controlPropertyMirrorMargins");
    controlPropertyMirrorMargins.action = function () {
        this.jsObject.ApplyPropertyValue("mirrorMargins", this.isChecked);
    }
    pageAdditionalPropertiesGroup.container.appendChild(this.Property("mirrorMargins", this.loc.PropertyMain.MirrorMargins, controlPropertyMirrorMargins));

    //Unlimited Height
    var controlPropertyUnlimitedHeight = this.CheckBox("controlPropertyPageUnlimitedHeight");
    controlPropertyUnlimitedHeight.action = function () {
        this.jsObject.ApplyPropertyValue("unlimitedHeight", this.isChecked);
    }
    pageAdditionalPropertiesGroup.container.appendChild(this.Property("unlimitedHeight", this.loc.PropertyMain.UnlimitedHeight, controlPropertyUnlimitedHeight));

    //Unlimited Breakable
    var controlPropertyUnlimitedBreakable = this.CheckBox("controlPropertyPageUnlimitedBreakable");
    controlPropertyUnlimitedBreakable.action = function () {
        this.jsObject.ApplyPropertyValue("unlimitedBreakable", this.isChecked);
    }
    pageAdditionalPropertiesGroup.container.appendChild(this.Property("unlimitedBreakable", this.loc.PropertyMain.UnlimitedBreakable, controlPropertyUnlimitedBreakable));

    //SegmentPerWidth
    var segmentPerWidthControl = this.PropertyTextBox("controlPropertySegmentPerWidth", this.options.propertyNumbersControlWidth);
    segmentPerWidthControl.action = function () {
        this.value = this.jsObject.StrToCorrectPositiveInt(this.value) || 1;
        this.jsObject.ApplyPropertyValue("segmentPerWidth", this.value);
    }
    pageAdditionalPropertiesGroup.container.appendChild(this.Property("segmentPerWidth", this.loc.PropertyMain.SegmentPerWidth, segmentPerWidthControl));

    //SegmentPerHeight
    var segmentPerHeightControl = this.PropertyTextBox("controlPropertySegmentPerHeight", this.options.propertyNumbersControlWidth);
    segmentPerHeightControl.action = function () {
        this.value = this.jsObject.StrToCorrectPositiveInt(this.value) || 1;
        this.jsObject.ApplyPropertyValue("segmentPerHeight", this.value);
    }
    pageAdditionalPropertiesGroup.container.appendChild(this.Property("segmentPerHeight", this.loc.PropertyMain.SegmentPerHeight, segmentPerHeightControl));

    return pageAdditionalPropertiesGroup;
}