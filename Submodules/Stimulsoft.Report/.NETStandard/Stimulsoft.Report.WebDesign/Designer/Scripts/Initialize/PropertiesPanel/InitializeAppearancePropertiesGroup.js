
StiMobileDesigner.prototype.AppearancePropertiesGroup = function () {
    var appearancePropertiesGroup = this.PropertiesGroup("appearancePropertiesGroup", this.loc.PropertyCategory.AppearanceCategory);
    appearancePropertiesGroup.style.display = "none";
    var jsObject = this;

    //BackColor
    var controlPropertyBackColor = this.PropertyColorExpressionControl("controlPropertyBackColor", null, this.options.propertyControlWidth);
    controlPropertyBackColor.action = function () {
        jsObject.ApplyPropertyExpressionControlValue("backColor", this.key, this.expression);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("backColor", this.loc.PropertyMain.BackColor, controlPropertyBackColor));

    //Border
    var controlPropertyBorder = this.PropertyTextBoxWithEditButton("controlPropertyBorder", this.options.propertyControlWidth, true);
    controlPropertyBorder.button.action = function () {
        jsObject.InitializeBorderSetupForm(function (borderSetupForm) {
            borderSetupForm.actionFunction = function (border) {
                var notLocalizeValues = jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.localizePropertyGrid;
                jsObject.options.controls.controlPropertyBorder.value = jsObject.BorderObjectToShotStr(border, notLocalizeValues);
            }
            borderSetupForm.actionFunction = null;
            borderSetupForm.showFunction = null;
            borderSetupForm.changeVisibleState(true);
        });
    }

    appearancePropertiesGroup.container.appendChild(this.Property("border", this.loc.PropertyMain.Border, controlPropertyBorder));

    //Brush
    var controlPropertyBrush = this.PropertyBrushExpressionControl("controlPropertyBrush", null, this.options.propertyControlWidth, false, true);
    controlPropertyBrush.action = function () {
        jsObject.ApplyPropertyExpressionControlValue("brush", this.key, this.expression);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("brush", this.loc.PropertyMain.Brush, controlPropertyBrush));

    //ShapeBorderColor
    var controlPropertyShapeBorderColor = this.PropertyColorExpressionControl("controlPropertyShapeBorderColor", null, this.options.propertyControlWidth);
    controlPropertyShapeBorderColor.action = function () {
        jsObject.ApplyPropertyExpressionControlValue("shapeBorderColor", this.key, this.expression, "borderColor");
    }
    appearancePropertiesGroup.container.appendChild(this.Property("shapeBorderColor", this.loc.PropertyMain.BorderColor, controlPropertyShapeBorderColor));

    //Conditions
    var controlPropertyConditions = this.PropertyTextBoxWithEditButton("controlPropertyConditions", this.options.propertyControlWidth, true);
    controlPropertyConditions.button.action = function () {
        jsObject.InitializeConditionsForm(function (conditionsForm) {
            conditionsForm.show();
        });
    }
    appearancePropertiesGroup.container.appendChild(this.Property("conditions", this.loc.PropertyMain.Conditions, controlPropertyConditions));

    //ChartConditions
    var controlPropertyChartConditions = this.PropertyChartConditionsControl("chartElementChartConditions", this.options.propertyControlWidth);
    controlPropertyChartConditions.action = function () {
        jsObject.ApplyPropertyValue(["chartConditions"], [jsObject.GetControlValue(this)]);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("chartConditions", this.loc.PropertyMain.Conditions, controlPropertyChartConditions));

    //IndicatorConditions
    var controlPropertyIndicatorConditions = this.PropertyIndicatorConditionsControl("indicatorElementIndicatorConditions", this.options.propertyControlWidth);
    controlPropertyIndicatorConditions.action = function () {
        jsObject.ApplyPropertyValue(["indicatorConditions"], [jsObject.GetControlValue(this)]);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("indicatorConditions", this.loc.PropertyMain.Conditions, controlPropertyIndicatorConditions));

    //ProgressConditions
    var controlPropertyProgressConditions = this.PropertyProgressConditionsControl("progressElementProgressConditions", this.options.propertyControlWidth);
    controlPropertyProgressConditions.action = function () {
        jsObject.ApplyPropertyValue(["progressConditions"], [jsObject.GetControlValue(this)]);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("progressConditions", this.loc.PropertyMain.Conditions, controlPropertyProgressConditions));

    //TableConditions
    var controlPropertyTableConditions = this.PropertyTableConditionsControl("tableElementTableConditions", this.options.propertyControlWidth);
    controlPropertyTableConditions.action = function () {
        jsObject.ApplyPropertyValue(["tableConditions"], [jsObject.GetControlValue(this)]);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("tableConditions", this.loc.PropertyMain.Conditions, controlPropertyTableConditions));

    //PivotTableConditions
    var controlPropertyPivotTableConditions = this.PropertyPivotTableConditionsControl("pivotTableElementTableConditions", this.options.propertyControlWidth);
    controlPropertyPivotTableConditions.action = function () {
        jsObject.ApplyPropertyValue(["pivotTableConditions"], [jsObject.GetControlValue(this)]);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("pivotTableConditions", this.loc.PropertyMain.Conditions, controlPropertyPivotTableConditions));

    //Component Style
    var controlPropertyComponentStyle = this.PropertyEnumExpressionControl("controlPropertyComponentStyle", this.options.propertyControlWidth, this.GetComponentStyleItems(), true, false);
    controlPropertyComponentStyle.action = function () {
        jsObject.ApplyPropertyExpressionControlValue("componentStyle", this.key, this.expression, null, true);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("componentStyle", this.loc.PropertyMain.ComponentStyle, controlPropertyComponentStyle));

    //Contour Color
    var controlPropertyCheckContourColor = this.PropertyColorExpressionControl("controlPropertyCheckContourColor", null, this.options.propertyControlWidth);
    controlPropertyCheckContourColor.action = function () {
        this.jsObject.ApplyPropertyExpressionControlValue("contourColor", this.key, this.expression);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("checkContourColor", this.loc.PropertyMain.ContourColor, controlPropertyCheckContourColor, "ContourColor"));

    //Corner Radius
    var controlPropertyCornerRadius = this.PropertyCornerRadiusControl("controlPropertyCornerRadius", this.options.propertyControlWidth + 61);
    controlPropertyCornerRadius.action = function () {
        jsObject.ApplyPropertyValue("cornerRadius", this.getValue());
    }
    appearancePropertiesGroup.container.appendChild(this.Property("cornerRadius", this.loc.PropertyMain.CornerRadius, controlPropertyCornerRadius));

    //Font
    var controlPropertyFont = this.PropertyFontControl("font");
    appearancePropertiesGroup.container.appendChild(this.Property("font", this.loc.PropertyMain.Font, controlPropertyFont));

    //DbsElementFont
    var controlPropertyDbsElementFont = this.PropertyFontControl("dbsElementFont", "font");
    appearancePropertiesGroup.container.appendChild(this.Property("dbsElementFont", this.loc.PropertyMain.Font, controlPropertyDbsElementFont));

    //FontSizeMode
    var controlPropertyFontSizeMode = this.PropertyDropDownList("controlPropertyFontSizeMode", this.options.propertyControlWidth, this.GetFontSizeModeItems(), true, false);
    controlPropertyFontSizeMode.action = function () {
        jsObject.ApplyPropertyValue("fontSizeMode", this.key);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("fontSizeMode", this.loc.PropertyMain.FontSizeMode, controlPropertyFontSizeMode));

    //ForeColor
    var controlPropertyForeColor = this.PropertyColorExpressionControl("controlPropertyForeColor", null, this.options.propertyControlWidth);
    controlPropertyForeColor.action = function () {
        jsObject.ApplyPropertyExpressionControlValue("foreColor", this.key, this.expression);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("foreColor", this.loc.PropertyMain.ForeColor, controlPropertyForeColor));

    //FooterFont
    var controlPropertyFooterFont = this.PropertyFontControl("footerFont", "footerFont");
    appearancePropertiesGroup.container.appendChild(this.Property("footerFont", this.loc.PropertyMain.FooterFont, controlPropertyFooterFont));

    //FooterForeColor
    var controlPropertyFooterForeColor = this.PropertyColorControl("controlPropertyFooterForeColor", null, this.options.propertyControlWidth);
    controlPropertyFooterForeColor.action = function () {
        jsObject.ApplyPropertyValue("footerForeColor", this.key);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("footerForeColor", this.loc.PropertyMain.FooterForeColor, controlPropertyFooterForeColor));

    //GlyphColor
    var controlPropertyGlyphColor = this.PropertyColorControl("controlPropertyGlyphColor", null, this.options.propertyControlWidth);
    controlPropertyGlyphColor.action = function () {
        jsObject.ApplyPropertyValue("glyphColor", this.key);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("glyphColor", this.loc.PropertyMain.GlyphColor || "Glyph Color", controlPropertyGlyphColor));

    //HeaderFont
    var controlPropertyHeaderFont = this.PropertyFontControl("headerFont", "headerFont");
    appearancePropertiesGroup.container.appendChild(this.Property("headerFont", this.loc.PropertyMain.HeaderFont, controlPropertyHeaderFont));

    //HeaderForeColor
    var controlPropertyHeaderForeColor = this.PropertyColorControl("controlPropertyHeaderForeColor", null, this.options.propertyControlWidth);
    controlPropertyHeaderForeColor.action = function () {
        jsObject.ApplyPropertyValue("headerForeColor", this.key);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("headerForeColor", this.loc.PropertyMain.HeaderForeColor, controlPropertyHeaderForeColor));

    //Icon Brush
    var controlPropertyIconBrush = this.PropertyBrushExpressionControl("controlPropertyIconBrush", null, this.options.propertyControlWidth, false, true);
    controlPropertyIconBrush.action = function () {
        this.jsObject.ApplyPropertyExpressionControlValue("iconBrush", this.key, this.expression);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("iconBrush", this.loc.PropertyMain.IconBrush, controlPropertyIconBrush));

    //IconColor
    var controlPropertyIconColor = this.PropertyColorExpressionControl("controlPropertyIconColor", null, this.options.propertyControlWidth);
    controlPropertyIconColor.action = function () {
        jsObject.ApplyPropertyExpressionControlValue("iconColor", this.key, this.expression);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("iconColor", this.loc.PropertyMain.IconColor, controlPropertyIconColor));

    //NegativeSeriesColors
    var controlNegativeSeriesColors = this.PropertyCollectionColorsComplicatedControl("controlPropertyNegativeSeriesColors", null, this.options.propertyControlWidth, this.options.predefinedColors.negativeSets);
    controlNegativeSeriesColors.action = function () {
        jsObject.ApplyPropertyValue(["negativeSeriesColors"], [this.key]);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("negativeSeriesColors", this.loc.PropertyMain.NegativeSeriesColors, controlNegativeSeriesColors));

    //ParetoSeriesColors
    var controlParetoSeriesColors = this.PropertyCollectionColorsComplicatedControl("controlPropertyParetoSeriesColors", null, this.options.propertyControlWidth, this.options.predefinedColors.negativeSets);
    controlParetoSeriesColors.action = function () {
        jsObject.ApplyPropertyValue(["paretoSeriesColors"], [this.key]);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("paretoSeriesColors", this.loc.PropertyMain.ParetoSeriesColors, controlParetoSeriesColors));

    //SeriesColors
    var controlSeriesColors = this.PropertyCollectionColorsComplicatedControl("controlPropertySeriesColors", null, this.options.propertyControlWidth, this.options.predefinedColors.sets);
    controlSeriesColors.action = function () {
        jsObject.ApplyPropertyValue(["seriesColors"], [this.key]);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("seriesColors", this.loc.PropertyMain.SeriesColors, controlSeriesColors));

    //Element Style
    var controlPropertyElementStyle = this.PropertyDropDownList("controlPropertyElementStyle", this.options.propertyControlWidth, null, true, false);
    controlPropertyElementStyle.action = function () {
        var selectedObject = jsObject.options.selectedObject;
        if (selectedObject && selectedObject.isDashboard) {
            if ((this.key.ident != "Custom" && selectedObject.properties.elementStyle == this.key.ident) ||
                (this.key.ident == "Custom" && selectedObject.properties.customStyleName == this.key.name)) return;

            selectedObject.properties.elementStyle = this.key.ident;
            jsObject.SendCommandChangeDashboardStyle(selectedObject.properties.name, this.key.ident);
        }
        else {
            jsObject.ApplyPropertyValue(["elementStyle", "customStyleName"], [this.key.ident, this.key.name || ""], true);
        }
    }
    appearancePropertiesGroup.container.appendChild(this.Property("elementStyle", this.loc.PropertyMain.Style, controlPropertyElementStyle));

    //Text Brush
    var controlPropertyTextBrush = this.PropertyBrushExpressionControl("controlPropertyTextBrush", null, this.options.propertyControlWidth, false, true);
    controlPropertyTextBrush.action = function () {
        this.jsObject.ApplyPropertyExpressionControlValue("textBrush", this.key, this.expression);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("textBrush", this.loc.PropertyMain.TextBrush, controlPropertyTextBrush));

    //Odd Style
    var controlPropertyOddStyle = this.PropertyEnumExpressionControl("controlPropertyOddStyle", this.options.propertyControlWidth, this.GetComponentStyleItems(false, true), true, false);
    controlPropertyOddStyle.action = function () {
        jsObject.ApplyPropertyExpressionControlValue("oddStyle", this.key, this.expression);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("oddStyle", this.loc.PropertyMain.OddStyle, controlPropertyOddStyle));

    //Even Style
    var controlPropertyEvenStyle = this.PropertyEnumExpressionControl("controlPropertyEvenStyle", this.options.propertyControlWidth, this.GetComponentStyleItems(false, true), true, false);
    controlPropertyEvenStyle.action = function () {
        jsObject.ApplyPropertyExpressionControlValue("evenStyle", this.key, this.expression);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("evenStyle", this.loc.PropertyMain.EvenStyle, controlPropertyEvenStyle));

    //UseParentStyles
    var controlPropertyUseParentStyles = this.CheckBox("controlPropertyUseParentStyles");
    controlPropertyUseParentStyles.action = function () {
        jsObject.ApplyPropertyValue("useParentStyles", this.isChecked);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("useParentStyles", this.loc.PropertyMain.UseParentStyles, controlPropertyUseParentStyles));

    //Dashboard Watermark
    var controlPropertyDbsWatermark = this.PropertyDashboardWatermark("controlPropertyDbsWatermark");
    controlPropertyDbsWatermark.action = function () {
        jsObject.ApplyPropertyValue("dashboardWatermark", this.key);
    }
    appearancePropertiesGroup.container.appendChild(this.Property("dashboardWatermark", this.loc.PropertyMain.Watermark, controlPropertyDbsWatermark));

    //Shadow Group
    var shadowGroup = this.ShadowPropertiesGroup();
    appearancePropertiesGroup.container.appendChild(shadowGroup);

    //Visual States Group
    var visualStatesGroup = this.VisualStatesPropertiesGroup();
    appearancePropertiesGroup.container.appendChild(visualStatesGroup);

    return appearancePropertiesGroup;
}