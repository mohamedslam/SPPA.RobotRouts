
StiMobileDesigner.prototype.BehaviorPropertiesGroup = function () {
    var jsObject = this;
    var behaviorPropertiesGroup = this.PropertiesGroup("behaviorPropertiesGroup", this.loc.PropertyCategory.BehaviorCategory);
    behaviorPropertiesGroup.style.display = "none";

    //Interaction Button
    var interactionButtonBlock = this.PropertyBlockWithButton("propertiesInteractionButtonBlock", "BigInteraction.png", this.loc.PropertyMain.Interaction + "...");
    behaviorPropertiesGroup.container.appendChild(interactionButtonBlock);

    interactionButtonBlock.button.action = function () {
        this.jsObject.InitializeInteractionForm(function (interactionForm) {
            interactionForm.show();
        });
    }

    //Anchor
    var controlPropertyAnchor = this.PropertyAnchorControl("controlPropertyAnchor", this.options.propertyControlWidth);
    controlPropertyAnchor.action = function () {
        this.jsObject.ApplyPropertyValue("anchor", this.key);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("anchor", this.loc.PropertyMain.Anchor, controlPropertyAnchor));

    //ArgumentFormat
    var controlPropertyArgumentFormat = this.PropertyTextBoxWithEditButton("chartElementArgumentFormat", this.options.propertyControlWidth, true);
    controlPropertyArgumentFormat.button.action = function () {
        this.jsObject.InitializeTextFormatForm(function (textFormatForm) {
            textFormatForm.show(null, "argumentFormat");
        });
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("argumentFormat", this.loc.PropertyMain.ArgumentFormat, controlPropertyArgumentFormat));

    //AutoWidth
    var controlPropertyAutoWidth = this.CheckBox("controlPropertyAutoWidth");
    controlPropertyAutoWidth.action = function () {
        this.jsObject.ApplyPropertyValue("autoWidth", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("autoWidth", this.loc.PropertyMain.AutoWidth, controlPropertyAutoWidth));

    //CalcInvisible
    var controlPropertyCalcInvisible = this.CheckBox("controlPropertyCalcInvisible");
    controlPropertyCalcInvisible.action = function () {
        this.jsObject.ApplyPropertyValue("calcInvisible", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("calcInvisible", this.loc.PropertyMain.CalcInvisible, controlPropertyCalcInvisible));

    //CanBreak
    var controlPropertyCanBreak = this.CheckBox("controlPropertyCanBreak");
    controlPropertyCanBreak.action = function () {
        this.jsObject.ApplyPropertyValue("canBreak", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("canBreak", this.loc.PropertyMain.CanBreak, controlPropertyCanBreak));

    //CanGrow
    var controlPropertyCanGrow = this.CheckBox("controlPropertyCanGrow");
    controlPropertyCanGrow.action = function () {
        this.jsObject.ApplyPropertyValue("canGrow", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("canGrow", this.loc.PropertyMain.CanGrow, controlPropertyCanGrow));

    //CanShrink
    var controlPropertyCanShrink = this.CheckBox("controlPropertyCanShrink");
    controlPropertyCanShrink.action = function () {
        this.jsObject.ApplyPropertyValue("canShrink", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("canShrink", this.loc.PropertyMain.CanShrink, controlPropertyCanShrink));

    //DockStyle
    var controlPropertyDockStyle = this.PropertyEnumExpressionControl("controlPropertyDockStyle", this.options.propertyControlWidth, this.GetDockStyleItems(), true, false);
    controlPropertyDockStyle.action = function () {
        this.jsObject.ApplyPropertyExpressionControlValue("dockStyle", this.key, this.expression);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("dockStyle", this.loc.PropertyMain.DockStyle, controlPropertyDockStyle));

    //Enabled
    var controlPropertyEnabled = this.PropertyBoolExpressionControl("controlPropertyEnabled", this.options.propertyControlWidth);
    controlPropertyEnabled.action = function () {
        this.jsObject.ApplyPropertyExpressionBoolValue("enabled", this.key);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("enabled", this.loc.PropertyMain.Enabled, controlPropertyEnabled));

    //GrowToHeight
    var controlPropertyGrowToHeight = this.CheckBox("controlPropertyGrowToHeight");
    controlPropertyGrowToHeight.action = function () {
        this.jsObject.ApplyPropertyValue("growToHeight", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("growToHeight", this.loc.PropertyMain.GrowToHeight, controlPropertyGrowToHeight));

    //IconAlignment
    var controlPropertyIconAlignment = this.PropertyDropDownList("controlPropertyIndicatorElementIconAlignment", this.options.propertyControlWidth, this.GetIconAlignmentItems(), true, false, false, true);
    controlPropertyIconAlignment.action = function () {
        this.jsObject.ApplyPropertyValue("iconAlignment", this.key);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("iconAlignment", this.loc.PropertyMain.IconAlignment, controlPropertyIconAlignment));

    //DashboardInteraction
    var controlPropertyDbsInteraction = this.PropertyDashboardInteractionControl("dashboardInteraction", this.options.propertyControlWidth);
    controlPropertyDbsInteraction.action = function () {
        this.jsObject.ApplyPropertyValue("dashboardInteraction", this.jsObject.GetControlValue(this));
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("dashboardInteraction", this.loc.PropertyMain.Interaction, controlPropertyDbsInteraction));

    //KeepGroupHeaderTogether
    var controlPropertyKeepGroupHeaderTogether = this.CheckBox("controlPropertyKeepGroupHeaderTogether");
    controlPropertyKeepGroupHeaderTogether.action = function () {
        this.jsObject.ApplyPropertyValue("keepGroupHeaderTogether", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("keepGroupHeaderTogether", this.loc.PropertyMain.KeepGroupHeaderTogether, controlPropertyKeepGroupHeaderTogether));

    //KeepGroupTogether
    var controlPropertyKeepGroupTogether = this.CheckBox("controlPropertyKeepGroupTogether");
    controlPropertyKeepGroupTogether.action = function () {
        this.jsObject.ApplyPropertyValue("keepGroupTogether", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("keepGroupTogether", this.loc.PropertyMain.KeepGroupTogether, controlPropertyKeepGroupTogether));

    //KeepGroupFooterTogether
    var controlPropertyKeepGroupFooterTogether = this.CheckBox("controlPropertyKeepGroupFooterTogether");
    controlPropertyKeepGroupFooterTogether.action = function () {
        this.jsObject.ApplyPropertyValue("keepGroupFooterTogether", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("keepGroupFooterTogether", this.loc.PropertyMain.KeepGroupFooterTogether, controlPropertyKeepGroupFooterTogether));

    //KeepHeaderTogether
    var controlPropertyKeepHeaderTogether = this.CheckBox("controlPropertyKeepHeaderTogether");
    controlPropertyKeepHeaderTogether.action = function () {
        this.jsObject.ApplyPropertyValue("keepHeaderTogether", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("keepHeaderTogether", this.loc.PropertyMain.KeepHeaderTogether, controlPropertyKeepHeaderTogether));

    //KeepFooterTogether
    var controlPropertyKeepFooterTogether = this.CheckBox("controlPropertyKeepFooterTogether");
    controlPropertyKeepFooterTogether.action = function () {
        this.jsObject.ApplyPropertyValue("keepFooterTogether", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("keepFooterTogether", this.loc.PropertyMain.KeepFooterTogether, controlPropertyKeepFooterTogether));

    //KeepDetails
    var controlPropertyKeepDetails = this.PropertyDropDownList("controlPropertyKeepDetails", this.options.propertyControlWidth, this.GetKeepDetailsItems(), true, false);
    controlPropertyKeepDetails.action = function () {
        this.jsObject.ApplyPropertyValue("keepDetails", this.key);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("keepDetails", this.loc.PropertyMain.KeepDetails, controlPropertyKeepDetails));

    //KeepDetailsTogether
    var controlPropertyKeepDetailsTogether = this.CheckBox("controlPropertyKeepDetailsTogether");
    controlPropertyKeepDetailsTogether.action = function () {
        this.jsObject.ApplyPropertyValue("keepDetailsTogether", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("keepDetailsTogether", this.loc.PropertyMain.KeepDetailsTogether, controlPropertyKeepDetailsTogether));

    //KeepReportSummaryTogether
    var controlPropertyKeepReportSummaryTogether = this.CheckBox("controlPropertyKeepReportSummaryTogether");
    controlPropertyKeepReportSummaryTogether.action = function () {
        this.jsObject.ApplyPropertyValue("keepReportSummaryTogether", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("keepReportSummaryTogether", this.loc.PropertyMain.KeepReportSummaryTogether, controlPropertyKeepReportSummaryTogether));

    //KeepSubReportTogether
    var controlPropertyKeepSubReportTogether = this.CheckBox("controlPropertyKeepSubReportTogether");
    controlPropertyKeepSubReportTogether.action = function () {
        this.jsObject.ApplyPropertyValue("keepSubReportTogether", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("keepSubReportTogether", this.loc.PropertyMain.KeepSubReportTogether, controlPropertyKeepSubReportTogether));

    //KeepCrossTabTogether
    var controlPropertyKeepCrossTabTogether = this.CheckBox("controlPropertyKeepCrossTabTogether");
    controlPropertyKeepCrossTabTogether.action = function () {
        this.jsObject.ApplyPropertyValue("keepCrossTabTogether", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("keepCrossTabTogether", this.loc.PropertyMain.KeepCrossTabTogether, controlPropertyKeepCrossTabTogether));

    //Margin
    var controlPropertyMargin = this.PropertyMarginsControl("controlPropertyMargin", this.options.propertyControlWidth + 61);
    controlPropertyMargin.action = function () {
        this.jsObject.ApplyPropertyValue("margin", this.getValue());
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("margin", this.loc.PropertyMain.Margin, controlPropertyMargin, "Margin"));

    //Padding
    var controlPropertyPadding = this.PropertyMarginsControl("controlPropertyPadding", this.options.propertyControlWidth + 61);
    controlPropertyPadding.action = function () {
        this.jsObject.ApplyPropertyValue("padding", this.getValue());
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("padding", this.loc.PropertyMain.Padding, controlPropertyPadding, "Padding"));

    //Printable
    var controlPropertyPrintable = this.PropertyBoolExpressionControl("controlPropertyPrintable", this.options.propertyControlWidth);
    controlPropertyPrintable.action = function () {
        this.jsObject.ApplyPropertyExpressionBoolValue("printable", this.key);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("printable", this.loc.PropertyMain.Printable, controlPropertyPrintable));

    //PrintAtBottom
    var controlPropertyPrintAtBottom = this.CheckBox("controlPropertyPrintAtBottom");
    controlPropertyPrintAtBottom.action = function () {
        this.jsObject.ApplyPropertyValue("printAtBottom", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("printAtBottom", this.loc.PropertyMain.PrintAtBottom, controlPropertyPrintAtBottom));

    //PrintIfEmpty
    var controlPropertyPrintIfEmpty = this.CheckBox("controlPropertyPrintIfEmpty");
    controlPropertyPrintIfEmpty.action = function () {
        this.jsObject.ApplyPropertyValue("printIfEmpty", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("printIfEmpty", this.loc.PropertyMain.PrintIfEmpty, controlPropertyPrintIfEmpty));

    //PrintIfDetailEmpty
    var controlPropertyPrintIfDetailEmpty = this.CheckBox("controlPropertyPrintIfDetailEmpty");
    controlPropertyPrintIfDetailEmpty.action = function () {
        this.jsObject.ApplyPropertyValue("printIfDetailEmpty", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("printIfDetailEmpty", this.loc.PropertyMain.PrintIfDetailEmpty, controlPropertyPrintIfDetailEmpty));

    //PrintOn
    var controlPropertyPrintOn = this.PropertyDropDownList("controlPropertyPrintOn", this.options.propertyControlWidth, this.GetPrintOnItems(), true, false);
    controlPropertyPrintOn.action = function () {
        this.jsObject.ApplyPropertyValue("printOn", this.key);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("printOn", this.loc.PropertyMain.PrintOn, controlPropertyPrintOn));

    //PrintOnAllPages
    var controlPropertyPrintOnAllPages = this.CheckBox("controlPropertyPrintOnAllPages");
    controlPropertyPrintOnAllPages.action = function () {
        this.jsObject.ApplyPropertyValue("printOnAllPages", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("printOnAllPages", this.loc.PropertyMain.PrintOnAllPages, controlPropertyPrintOnAllPages));

    //PrintOnEvenOddPages
    var controlPropertyPrintOnEvenOddPages = this.PropertyDropDownList("controlPropertyPrintOnEvenOddPages", this.options.propertyControlWidth, this.GetPrintOnEvenOddPagesItems(), true, false);
    controlPropertyPrintOnEvenOddPages.action = function () {
        this.jsObject.ApplyPropertyValue("printOnEvenOddPages", this.key);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("printOnEvenOddPages", this.loc.PropertyMain.PrintOnEvenOddPages, controlPropertyPrintOnEvenOddPages));

    //PrintOnPreviousPage
    var controlPropertyPrintOnPreviousPage = this.CheckBox("controlPropertyPrintOnPreviousPage");
    controlPropertyPrintOnPreviousPage.action = function () {
        this.jsObject.ApplyPropertyValue("printOnPreviousPage", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("printOnPreviousPage", this.loc.PropertyMain.PrintOnPreviousPage, controlPropertyPrintOnPreviousPage));

    //PrintHeadersFootersFromPreviousPage
    var controlPropertyPrintHeadersFootersFromPreviousPage = this.CheckBox("controlPropertyPrintHeadersFootersFromPreviousPage");
    controlPropertyPrintHeadersFootersFromPreviousPage.action = function () {
        this.jsObject.ApplyPropertyValue("printHeadersFootersFromPreviousPage", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("printHeadersFootersFromPreviousPage", this.loc.PropertyMain.PrintHeadersFootersFromPreviousPage, controlPropertyPrintHeadersFootersFromPreviousPage));

    //ResetPageNumber
    var controlPropertyResetPageNumber = this.CheckBox("controlPropertyResetPageNumber");
    controlPropertyResetPageNumber.action = function () {
        this.jsObject.ApplyPropertyValue("resetPageNumber", this.isChecked);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("resetPageNumber", this.loc.PropertyMain.ResetPageNumber, controlPropertyResetPageNumber));

    //Shift Mode
    var controlPropertyShiftMode = this.PropertyShiftModeControl("controlPropertyShiftMode", this.options.propertyControlWidth);
    controlPropertyShiftMode.action = function () {
        this.jsObject.ApplyPropertyValue("shiftMode", this.key);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("shiftMode", this.loc.PropertyMain.ShiftMode, controlPropertyShiftMode));

    //SizeMode
    var controlPropertySizeMode = this.PropertyDropDownList("controlPropertySizeMode", this.options.propertyControlWidth, this.GetSizeModeItems(), true, false);
    controlPropertySizeMode.action = function () {
        this.jsObject.ApplyPropertyValue("sizeMode", this.key);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("sizeMode", this.loc.PropertyMain.SizeMode, controlPropertySizeMode));

    //TextFormat
    var controlPropertyTextFormat = this.PropertyTextBoxWithEditButton("controlPropertyTextFormatDbsElement", this.options.propertyControlWidth, true);
    controlPropertyTextFormat.button.action = function () {
        this.jsObject.InitializeTextFormatForm(function (textFormatForm) {
            textFormatForm.show();
        });
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("textFormatDbsElement", this.loc.PropertyMain.TextFormat, controlPropertyTextFormat, "TextFormat"));

    //ValueFormat
    var controlPropertyValueFormat = this.PropertyTextBoxWithEditButton("chartElementValueFormat", this.options.propertyControlWidth, true);
    controlPropertyValueFormat.button.action = function () {
        this.jsObject.InitializeTextFormatForm(function (textFormatForm) {
            textFormatForm.show(null, "valueFormat");
        });
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("valueFormat", this.loc.PropertyMain.ValueFormat, controlPropertyValueFormat));

    //VerticalAlignment
    var controlPropertyVerticalAlignment = this.PropertyDropDownList("controlPropertyDataVerticalAlignment", this.options.propertyControlWidth, this.GetVerticalAlignmentItems(), true, false);
    controlPropertyVerticalAlignment.action = function () {
        this.jsObject.ApplyPropertyValue("vertAlignment", this.key);
    }
    behaviorPropertiesGroup.container.appendChild(this.Property("dataVerticalAlignment", this.loc.PropertyMain.VertAlignment, controlPropertyVerticalAlignment));

    //Title Group
    var titleGroup = this.TitlePropertiesGroup();
    behaviorPropertiesGroup.container.appendChild(titleGroup);

    return behaviorPropertiesGroup;
}