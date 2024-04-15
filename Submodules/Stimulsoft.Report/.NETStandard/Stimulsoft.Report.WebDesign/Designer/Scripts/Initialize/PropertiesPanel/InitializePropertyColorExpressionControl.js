
StiMobileDesigner.prototype.PropertyColorExpressionControl = function (name, toolTip, width) {
    var jsObject = this;
    var colorControl = this.PropertyColorControl(name, toolTip, width);
    colorControl.cutBrackets = true;
    if (name) this.options.controls[name] = colorControl;

    colorControl.button.action = function () {
        var colorDialog = this.jsObject.options.menus.colorDialog || this.jsObject.InitializeColorDialog();
        colorDialog.rightToLeft = this.colorControl.rightToLeft;
        colorDialog.noFillButton.caption.innerHTML = colorControl.isDbsElement && !colorDialog.visible
            ? this.jsObject.loc.FormStyleDesigner.FromStyle
            : this.jsObject.loc.Gui.colorpicker_nofill.replace("&", "");

        colorDialog.changeVisibleState(!colorDialog.visible, this);
        colorDialog.showExpression(colorControl.expression);

        colorDialog.expressionPanel.expressionButton.action = function () {
            colorDialog.changeVisibleState(false, colorControl.button);

            jsObject.InitializeExpressionEditorForm(function (form) {
                var propertiesPanel = jsObject.options.propertiesPanel;
                form.propertiesPanelZIndex = propertiesPanel.style.zIndex;
                form.propertiesPanelIsEnabled = propertiesPanel.isEnabled;
                form.resultControl = colorControl;

                form.onshow = function () {
                    var propertiesPanel = jsObject.options.propertiesPanel;
                    propertiesPanel.setDictionaryMode(true);
                    propertiesPanel.setEnabled(true);
                    propertiesPanel.editFormControl = form.expressionTextArea;
                    form.expressionTextArea.value = colorControl.expression || colorControl.getExpressionFromColor();
                    form.expressionTextArea.focus();
                }

                form.action = function () {
                    colorControl.setKey(colorControl.key, colorControl.isDbsElement, form.expressionTextArea.value != "" ? form.expressionTextArea.value : null);
                    form.changeVisibleState(false);
                    colorControl.action();
                }

                form.changeVisibleState(true);
            });
        }
    }

    colorControl.getExpressionFromColor = function () {
        return "\"" + colorControl.strColorToHex(this.key) + "\"";
    }

    colorControl.strColorToHex = function (strColor) {
        if (strColor) {
            if (strColor == "transparent") return "#00FFFFFF";
            var colors = strColor.split(",");
            return jsObject.RgbToHex(parseInt(colors[0]), parseInt(colors[1]), parseInt(colors[2]), colors.length > 3 ? parseInt(colors[3]) : null).toUpperCase();
        }
        return "";
    }

    colorControl.setKey = function (key, isDbsElement, expression) {
        var notLocalizeValues = this.ownerIsProperty && jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.localizePropertyGrid;
        this.key = key;
        this.expression = expression;
        this.isDbsElement = isDbsElement;

        if (expression) {
            this.button.image.style.display = "none";
            var textBlock = document.createElement("div");
            textBlock.style.width = (width - (jsObject.options.isTouchDevice ? 40 : 25)) + "px";
            textBlock.style.textOverflow = "ellipsis";
            textBlock.style.overflow = "hidden";
            textBlock.innerHTML = expression;
            this.button.caption.innerHTML = "";
            this.button.caption.appendChild(textBlock);
        }
        else {
            this.button.image.style.display = "";
            if (key == "StiEmptyValue") {
                this.button.image.style.opacity = 0;
                this.button.caption.innerHTML = "";
                return;
            }
            this.button.image.style.opacity = 1;
            var color;
            if (key == "transparent")
                color = "255,255,255";
            else {
                var colors = key.split(",");
                if (colors.length == 4) {
                    this.button.image.style.opacity = jsObject.StrToInt(colors[0]) / 255;
                    colors.splice(0, 1);
                }
                color = colors[0] + "," + colors[1] + "," + colors[2];
            }

            this.button.image.style.background = "rgb(" + color + ")";
            var colorName = jsObject.GetColorNameByRGB(this.key, notLocalizeValues);
            this.button.caption.innerHTML = (isDbsElement && key == "transparent") ? (notLocalizeValues ? "FromStyle" : jsObject.loc.FormStyleDesigner.FromStyle) : (colorName || this.key);
        }
    };

    return colorControl;
}