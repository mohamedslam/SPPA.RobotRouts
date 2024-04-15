
StiMobileDesigner.prototype.TextPropertiesGroup = function () {
    var textPropertiesGroup = this.PropertiesGroup("textPropertiesGroup", this.loc.PropertyMain.Text);
    textPropertiesGroup.style.display = "none";

    //Text
    var controlPropertyText = this.PropertyTextBoxWithEditButton("controlPropertyText", this.options.propertyControlWidth);
    controlPropertyText.textBox.action = function () {
        this.jsObject.ApplyPropertyValue("text", StiBase64.encode(this.value));
    }
    controlPropertyText.button.action = function () {
        this.jsObject.InitializeTextEditorForm(function (textEditorForm) {
            textEditorForm.propertyName = "text";
            textEditorForm.changeVisibleState(true);
        });
    }
    textPropertiesGroup.container.appendChild(this.Property("text", this.loc.PropertyMain.Text, controlPropertyText));

    //Cell Width
    var controlPropertyCellWidth = this.PropertyTextBox("controlPropertyCellWidth", this.options.propertyNumbersControlWidth);
    controlPropertyCellWidth.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue("cellWidth", this.value);
    }
    textPropertiesGroup.container.appendChild(this.Property("cellWidth", this.loc.PropertyMain.CellWidth, controlPropertyCellWidth));

    //Cell Height
    var controlPropertyCellHeight = this.PropertyTextBox("controlPropertyCellHeight", this.options.propertyNumbersControlWidth);
    controlPropertyCellHeight.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue("cellHeight", this.value);
    }
    textPropertiesGroup.container.appendChild(this.Property("cellHeight", this.loc.PropertyMain.CellHeight, controlPropertyCellHeight));

    //Horizontal Alignment
    var controlPropertyHorAlign = this.PropertyEnumExpressionControl("controlPropertyTextHorizontalAlignment", this.options.propertyControlWidth, this.GetHorizontalAlignmentItems(), true, false);
    controlPropertyHorAlign.action = function () {
        this.jsObject.ApplyPropertyExpressionControlValue("horAlignment", this.key, this.expression);
    }
    textPropertiesGroup.container.appendChild(this.Property("textHorizontalAlignment", this.loc.PropertyMain.HorAlignment, controlPropertyHorAlign, "HorAlignment"));

    //Vertical Alignment
    var controlPropertyVertAlign = this.PropertyEnumExpressionControl("controlPropertyTextVerticalAlignment", this.options.propertyControlWidth, this.GetVerticalAlignmentItems(), true, false);
    controlPropertyVertAlign.action = function () {
        this.jsObject.ApplyPropertyExpressionControlValue("vertAlignment", this.key, this.expression);
    }
    textPropertiesGroup.container.appendChild(this.Property("textVerticalAlignment", this.loc.PropertyMain.VertAlignment, controlPropertyVertAlign, "VertAlignment"));

    //Horizontal Spacing
    var controlPropertyHorizontalSpacing = this.PropertyTextBox("controlPropertyHorizontalSpacing", this.options.propertyNumbersControlWidth);
    controlPropertyHorizontalSpacing.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue("horizontalSpacing", this.value);
    }
    textPropertiesGroup.container.appendChild(this.Property("horizontalSpacing", this.loc.PropertyMain.HorSpacing, controlPropertyHorizontalSpacing, "HorSpacing"));

    //Vertical Spacing
    var controlPropertyVerticalSpacing = this.PropertyTextBox("controlPropertyVerticalSpacing", this.options.propertyNumbersControlWidth);
    controlPropertyVerticalSpacing.action = function () {
        this.value = Math.abs(this.jsObject.StrToDouble(this.value));
        this.jsObject.ApplyPropertyValue("verticalSpacing", this.value);
    }
    textPropertiesGroup.container.appendChild(this.Property("verticalSpacing", this.loc.PropertyMain.VertSpacing, controlPropertyVerticalSpacing, "VertSpacing"));

    //Text Format
    var controlPropertyTextFormat = this.PropertyTextBoxWithEditButton("controlPropertyTextFormat", this.options.propertyControlWidth, true);
    controlPropertyTextFormat.button.action = function () {
        this.jsObject.InitializeTextFormatForm(function (textFormatForm) {
            textFormatForm.show();
        });
    }
    textPropertiesGroup.container.appendChild(this.Property("textFormat", this.loc.PropertyMain.TextFormat, controlPropertyTextFormat));

    return textPropertiesGroup;
}