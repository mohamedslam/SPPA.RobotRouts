
StiMobileDesigner.prototype.PrimitivePropertiesGroup = function () {
    var primitivePropertiesGroup = this.PropertiesGroup("primitivePropertiesGroup", this.loc.PropertyCategory.PrimitiveCategory);
    primitivePropertiesGroup.style.display = "none";

    //Color
    var controlPropertyPrimitiveColor = this.PropertyColorControl("controlPropertyPrimitiveColor", null, this.options.propertyControlWidth);
    controlPropertyPrimitiveColor.action = function () {
        this.jsObject.ApplyPropertyValue("color", this.key);
    }
    primitivePropertiesGroup.container.appendChild(this.Property("primitiveColor", this.loc.PropertyMain.Color, controlPropertyPrimitiveColor));

    //Size
    var controlPropertyPrimitiveSize = this.PropertyTextBox("controlPropertyPrimitiveSize", this.options.propertyNumbersControlWidth);
    controlPropertyPrimitiveSize.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue("size", this.value);
    }
    primitivePropertiesGroup.container.appendChild(this.Property("primitiveSize", this.loc.PropertyMain.Size, controlPropertyPrimitiveSize));

    //Style
    var controlPropertyPrimitiveStyle = this.PropertyDropDownList("controlPropertyPrimitiveStyle", this.options.propertyControlWidth, this.GetBorderStyleItems(), true, true);
    controlPropertyPrimitiveStyle.action = function () {
        this.jsObject.ApplyPropertyValue("style", this.key);
    };
    primitivePropertiesGroup.container.appendChild(this.Property("primitiveStyle", this.loc.PropertyMain.Style, controlPropertyPrimitiveStyle))

    //Round
    var controlPropertyPrimitiveRound = this.PropertyTextBox("controlPropertyPrimitiveRound", this.options.propertyNumbersControlWidth);
    controlPropertyPrimitiveRound.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue("round", this.value);
    }
    primitivePropertiesGroup.container.appendChild(this.Property("primitiveRound", this.loc.PropertyMain.Round, controlPropertyPrimitiveRound));

    //LeftSide
    var controlPropertyPrimitiveLeftSide = this.CheckBox("controlPropertyPrimitiveLeftSide");
    controlPropertyPrimitiveLeftSide.action = function () {
        this.jsObject.ApplyPropertyValue("leftSide", this.isChecked);
    }
    primitivePropertiesGroup.container.appendChild(this.Property("primitiveLeftSide", this.loc.PropertyMain.LeftSide, controlPropertyPrimitiveLeftSide));

    //RightSide
    var controlPropertyPrimitiveRightSide = this.CheckBox("controlPropertyPrimitiveRightSide");
    controlPropertyPrimitiveRightSide.action = function () {
        this.jsObject.ApplyPropertyValue("rightSide", this.isChecked);
    }
    primitivePropertiesGroup.container.appendChild(this.Property("primitiveRightSide", this.loc.PropertyMain.RightSide, controlPropertyPrimitiveRightSide));

    //TopSide
    var controlPropertyPrimitiveTopSide = this.CheckBox("controlPropertyPrimitiveTopSide");
    controlPropertyPrimitiveTopSide.action = function () {
        this.jsObject.ApplyPropertyValue("topSide", this.isChecked);
    }
    primitivePropertiesGroup.container.appendChild(this.Property("primitiveTopSide", this.loc.PropertyMain.TopSide, controlPropertyPrimitiveTopSide));

    //BottomSide
    var controlPropertyPrimitiveBottomSide = this.CheckBox("controlPropertyPrimitiveBottomSide");
    controlPropertyPrimitiveBottomSide.action = function () {
        this.jsObject.ApplyPropertyValue("bottomSide", this.isChecked);
    }
    primitivePropertiesGroup.container.appendChild(this.Property("primitiveBottomSide", this.loc.PropertyMain.BottomSide, controlPropertyPrimitiveBottomSide));

    primitivePropertiesGroup.container.appendChild(this.PrimitiveCapPropertiesGroup("Start"));
    primitivePropertiesGroup.container.appendChild(this.PrimitiveCapPropertiesGroup("End"));

    return primitivePropertiesGroup;
}

StiMobileDesigner.prototype.PrimitiveCapPropertiesGroup = function (capType) {
    var propertyStartName = this.LowerFirstChar(capType);
    var capPropertiesGroup = this.PropertiesGroup(propertyStartName + "CapPropertiesGroup", this.loc.PropertyMain[capType + "Cap"]);
    capPropertiesGroup.style.display = "none";


    //Color
    var controlPropertyCapColor = this.PropertyColorControl("controlProperty" + capType + "CapColor", null, this.options.propertyControlWidth);
    controlPropertyCapColor.action = function () {
        this.jsObject.ApplyPropertyValue(propertyStartName + "CapColor", this.key);
    }
    capPropertiesGroup.container.appendChild(this.Property(capType + "startCapColor", this.loc.PropertyMain.Color, controlPropertyCapColor));

    //Fill
    var controlPropertyCapFill = this.CheckBox("controlProperty" + capType + "CapFill");
    controlPropertyCapFill.action = function () {
        this.jsObject.ApplyPropertyValue(this.jsObject.LowerFirstChar(capType) + "CapFill", this.isChecked);
    }
    capPropertiesGroup.container.appendChild(this.Property(propertyStartName + "CapFill", this.loc.PropertyMain.Fill, controlPropertyCapFill));

    //Width
    var controlPropertyCapWidth = this.PropertyTextBox("controlProperty" + capType + "CapWidth", this.options.propertyNumbersControlWidth);
    controlPropertyCapWidth.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue(propertyStartName + "CapWidth", this.value);
    }
    capPropertiesGroup.container.appendChild(this.Property(propertyStartName + "CapWidth", this.loc.PropertyMain.Width, controlPropertyCapWidth));

    //Height
    var controlPropertyCapHeight = this.PropertyTextBox("controlProperty" + capType + "CapHeight", this.options.propertyNumbersControlWidth);
    controlPropertyCapHeight.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue(propertyStartName + "CapHeight", this.value);
    }
    capPropertiesGroup.container.appendChild(this.Property(propertyStartName + "CapHeight", this.loc.PropertyMain.Height, controlPropertyCapHeight));

    //Style
    var controlPropertyCapStyle = this.PropertyDropDownList("controlProperty" + capType + "CapStyle", this.options.propertyControlWidth, this.GetBorderPrimitiveCapStyleItems(), true, true);
    controlPropertyCapStyle.action = function () {
        this.jsObject.ApplyPropertyValue("style", this.key);
    };
    capPropertiesGroup.container.appendChild(this.Property(propertyStartName + "CapStyle", this.loc.PropertyMain.Style, controlPropertyCapStyle));

    return capPropertiesGroup;
}