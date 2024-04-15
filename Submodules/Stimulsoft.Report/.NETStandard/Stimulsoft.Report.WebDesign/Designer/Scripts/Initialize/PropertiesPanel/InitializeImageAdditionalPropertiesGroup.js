
StiMobileDesigner.prototype.ImageAdditionalPropertiesGroup = function () {
    var jsObject = this;
    var imageAdditionalPropertiesGroup = this.PropertiesGroup("imageAdditionalPropertiesGroup", this.loc.PropertyCategory.ImageAdditionalCategory);
    imageAdditionalPropertiesGroup.style.display = "none";

    //Image Aspect Ratio
    var controlPropertyImageAspectRatioControl = this.CheckBox("controlPropertyImageAspectRatio");
    controlPropertyImageAspectRatioControl.action = function () {
        jsObject.ApplyPropertyValue("ratio", this.isChecked);
    }
    imageAdditionalPropertiesGroup.container.appendChild(this.Property("imageAspectRatio",
        this.loc.PropertyMain.AspectRatio, controlPropertyImageAspectRatioControl, "AspectRatio"));

    //Horizontal Alignment
    var controlPropertyHorAlign = this.PropertyEnumExpressionControl("controlPropertyImageHorizontalAlignment",
        this.options.propertyControlWidth, this.GetHorizontalAlignmentItems(true), true, false);
    controlPropertyHorAlign.action = function () {
        jsObject.ApplyPropertyExpressionControlValue("horAlignment", this.key, this.expression);
    }
    imageAdditionalPropertiesGroup.container.appendChild(this.Property("imageHorizontalAlignment",
        this.loc.PropertyMain.HorAlignment, controlPropertyHorAlign, "HorAlignment"));

    //Vertical Alignment
    var controlPropertyVertAlign = this.PropertyEnumExpressionControl("controlPropertyImageVerticalAlignment",
        this.options.propertyControlWidth, this.GetVerticalAlignmentItems(), true, false);
    controlPropertyVertAlign.action = function () {
        jsObject.ApplyPropertyExpressionControlValue("vertAlignment", this.key, this.expression);
    }
    imageAdditionalPropertiesGroup.container.appendChild(this.Property("imageVerticalAlignment",
        this.loc.PropertyMain.VertAlignment, controlPropertyVertAlign, "VertAlignment"));

    //Image Rotation
    var controlPropertyImageRotation = this.PropertyDropDownList("controlPropertyImageRotation",
        this.options.propertyControlWidth, this.GetImageRotationItems(), true, false);
    controlPropertyImageRotation.action = function () {
        jsObject.ApplyPropertyValue("rotation", this.key);
    }
    imageAdditionalPropertiesGroup.container.appendChild(this.Property("imageRotation",
        this.loc.PropertyMain.ImageRotation, controlPropertyImageRotation));

    //Image Margins
    var controlPropertyImageMargins = this.PropertyMarginsControl("controlPropertyImageMargins", this.options.propertyControlWidth + 61);
    controlPropertyImageMargins.action = function () {
        jsObject.ApplyPropertyValue("imageMargins", this.getValue());
    }
    imageAdditionalPropertiesGroup.container.appendChild(this.Property("imageMargins", this.loc.PropertyMain.Margins, controlPropertyImageMargins, "Margins"));

    //MultipleFactor
    var controlPropertyImageMultipleFactor = this.PropertyTextBox("controlPropertyImageMultipleFactor", this.options.propertyNumbersControlWidth);
    controlPropertyImageMultipleFactor.action = function () {
        this.value = Math.abs(jsObject.StrToDouble(this.value));
        jsObject.ApplyPropertyValue("imageMultipleFactor", this.value);
    }
    imageAdditionalPropertiesGroup.container.appendChild(this.Property("imageMultipleFactor",
        this.loc.PropertyMain.ImageMultipleFactor, controlPropertyImageMultipleFactor, "MultipleFactor"));

    //Processing Duplicates
    var controlPropertyImageProcessingDuplicates = this.PropertyDropDownList("controlPropertyImageProcessingDuplicates",
        this.options.propertyControlWidth, this.GetImageProcessingDuplicatesItems(), true, false);
    controlPropertyImageProcessingDuplicates.action = function () {
        jsObject.ApplyPropertyValue("imageProcessingDuplicates", this.key);
    }
    imageAdditionalPropertiesGroup.container.appendChild(this.Property("imageProcessingDuplicates",
        this.loc.PropertyMain.ProcessingDuplicates, controlPropertyImageProcessingDuplicates));

    //Image Smoothing
    var controlPropertyImageSmoothingControl = this.CheckBox("controlPropertyImageSmoothing");
    controlPropertyImageSmoothingControl.action = function () {
        jsObject.ApplyPropertyValue("imageSmoothing", this.isChecked);
    }
    imageAdditionalPropertiesGroup.container.appendChild(this.Property("imageSmoothing",
        this.loc.PropertyMain.Smoothing, controlPropertyImageSmoothingControl, "Smoothing"));

    //Image Stretch
    var controlPropertyImageStretchControl = this.CheckBox("controlPropertyImageStretch");
    controlPropertyImageStretchControl.action = function () {
        jsObject.ApplyPropertyValue("stretch", this.isChecked);
    }
    imageAdditionalPropertiesGroup.container.appendChild(this.Property("imageStretch",
        this.loc.PropertyMain.ImageStretch, controlPropertyImageStretchControl, "Stretch"));

    return imageAdditionalPropertiesGroup;
}