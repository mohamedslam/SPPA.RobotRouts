
StiMobileDesigner.prototype.PropertyBrushExpressionControl = function (name, toolTip, width, showDefState, showFromStyle) {
    var jsObject = this;
    var brushControl = this.PropertyBrushControl(name, toolTip, width);
    if (name) this.options.controls[name] = brushControl;

    brushControl.cutBrackets = true;
    brushControl.showDefState = showDefState;
    brushControl.showFromStyle = showFromStyle;

    brushControl.button.action = function () {
        var brushMenu = this.brushMenu;
        brushMenu.showExpression(brushControl.expression);
        brushMenu.changeVisibleState(!this.brushMenu.visible);

        brushMenu.onchanged = function () {
            brushMenu.showExpression();
        };

        brushMenu.expressionPanel.expressionButton.action = function () {
            brushMenu.changeVisibleState(false);

            jsObject.InitializeExpressionEditorForm(function (form) {
                var propertiesPanel = jsObject.options.propertiesPanel;
                form.propertiesPanelZIndex = propertiesPanel.style.zIndex;
                form.propertiesPanelIsEnabled = propertiesPanel.isEnabled;
                form.resultControl = brushControl;

                form.onshow = function () {
                    var propertiesPanel = jsObject.options.propertiesPanel;
                    propertiesPanel.setDictionaryMode(true);
                    propertiesPanel.setEnabled(true);
                    propertiesPanel.editFormControl = form.expressionTextArea;
                    form.expressionTextArea.value = brushControl.expression || brushControl.getExpressionFromBrush();
                    form.expressionTextArea.focus();
                }

                form.action = function () {
                    brushControl.setKey(brushControl.key, form.expressionTextArea.value != "" ? form.expressionTextArea.value : null);
                    form.changeVisibleState(false);
                    brushControl.action();
                }

                form.changeVisibleState(true);
            });
        }

        if (brushControl.showDefState) {
            brushMenu.defStateButton.style.display = brushMenu.defStyleSeparator.style.display = "";

            brushMenu.defStateButton.action = function () {
                brushMenu.changeVisibleState(false);
                brushControl.setKey("isDefaultBrush");
                brushControl.action();
            }
        }

        if (brushControl.showFromStyle) {
            brushMenu.fromStyleButton.style.display = brushMenu.defStyleSeparator.style.display = "";

            brushMenu.fromStyleButton.action = function () {
                brushMenu.changeVisibleState(false);
                brushControl.setKey("isStyleBrush");
                brushControl.action();
            }
        }
    }

    brushControl.setKey = function (key, expression) {
        var notLocalizeValues = this.ownerIsProperty && jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.localizePropertyGrid;
        this.key = key;
        this.expression = expression;
        this.button.key = key;
        this.button.image.style.opacity = 1;
        this.button.image.style.display = "";

        var brushMenu = this.button.brushMenu;
        brushMenu.defStateButton.setSelected(key == "isDefaultBrush" && !expression);
        brushMenu.fromStyleButton.setSelected(key == "isStyleBrush" && !expression);

        if (expression || key == "isStyleBrush" || key == "isDefaultBrush") {
            var text = "";
            if (key == "isStyleBrush") text = jsObject.loc.FormStyleDesigner.FromStyle;
            if (key == "isDefaultBrush") text = jsObject.loc.Report.FromDefaultState;
            if (expression) text = expression;

            this.button.image.style.display = "none";
            var textBlock = document.createElement("div");
            textBlock.style.width = (width - (jsObject.options.isTouchDevice ? 40 : 25)) + "px";
            textBlock.style.textOverflow = "ellipsis";
            textBlock.style.overflow = "hidden";
            textBlock.innerHTML = text;
            this.button.caption.innerHTML = "";
            this.button.caption.appendChild(textBlock);
        }
        else {
            if (key == "StiEmptyValue" || key == "none") {
                this.button.image.setBrush(["0"]);
                this.button.caption.innerHTML = "";
                return;
            }

            var brushTypes = ["Empty", "Solid", "Hatch", "Gradient", "Glare", "Glass"];
            var brushArray = key.split("!");
            var brushType = brushTypes[brushArray[0]];

            this.button.image.setBrush(brushArray);
            this.button.caption.innerHTML = notLocalizeValues ? brushType : jsObject.loc.Report["Sti" + brushType + "Brush"];
        }
    }

    brushControl.getExpressionFromBrush = function () {
        var brushArray = this.key.split("!");
        switch (brushArray[0]) {
            case "1":
                return "SolidBrushValue(\"" + brushControl.strColorToHex(brushArray[1]) + "\")";

            case "2":
                return "HatchBrushValue(" + (jsObject.options.jsMode ? brushArray[3] : ("HatchStyle." + brushControl.hatchKeyToName(brushArray[3]))) + ", \"" + brushControl.strColorToHex(brushArray[1]) + "\", \"" + brushControl.strColorToHex(brushArray[2]) + "\")";

            case "3":
                return "GradientBrushValue(\"" + brushControl.strColorToHex(brushArray[1]) + "\", \"" + brushControl.strColorToHex(brushArray[2]) + "\", " + brushArray[3].replace(/,/g, ".") + ")";

            case "4":
                return "GlareBrushValue(\"" + brushControl.strColorToHex(brushArray[1]) + "\", \"" + brushControl.strColorToHex(brushArray[2]) + "\", " + brushArray[3].replace(/,/g, ".") + ", " + brushArray[4].replace(/,/g, ".") + ", " + brushArray[5].replace(/,/g, ".") + ")";

            case "5":
                return "GlassBrushValue(\"" + brushControl.strColorToHex(brushArray[1]) + "\", " + (brushArray[3] ? "true" : "false") + ", " + brushArray[2].replace(/,/g, ".") + ")";
        }

        return "";
    }

    brushControl.strColorToHex = function (strColor) {
        if (strColor) {
            if (strColor == "transparent")
                return "#00FFFFFF";

            var colors = strColor.split(",");
            if (colors.length == 4)
                return jsObject.RgbToHex(parseInt(colors[1]), parseInt(colors[2]), parseInt(colors[3]), parseInt(colors[0])).toUpperCase();
            else
                return jsObject.RgbToHex(parseInt(colors[0]), parseInt(colors[1]), parseInt(colors[2])).toUpperCase();
        }
        return "";
    }

    brushControl.hatchKeyToName = function (hatchKey) {
        var hatchStyles = {
            "3": "BackwardDiagonal",
            "4": "LargeGrid",
            "20": "DarkDownwardDiagonal",
            "29": "DarkHorizontal",
            "21": "DarkUpwardDiagonal",
            "28": "DarkVertical",
            "30": "DashedDownwardDiagonal",
            "32": "DashedHorizontal",
            "31": "DashedUpwardDiagonal",
            "33": "DashedVertical",
            "38": "DiagonalBrick",
            "5": "DiagonalCross",
            "42": "Divot",
            "44": "DottedDiamond",
            "43": "DottedGrid",
            "2": "ForwardDiagonal",
            "0": "Horizontal",
            "39": "HorizontalBrick",
            "50": "LargeCheckerBoard",
            "35": "LargeConfetti",
            "18": "LightDownwardDiagonal",
            "25": "LightHorizontal",
            "19": "LightUpwardDiagonal",
            "24": "LightVertical",
            "27": "NarrowHorizontal",
            "26": "NarrowVertical",
            "51": "OutlinedDiamond",
            "6": "Percent05",
            "7": "Percent10",
            "8": "Percent20",
            "9": "Percent25",
            "10": "Percent30",
            "11": "Percent40",
            "12": "Percent50",
            "13": "Percent60",
            "14": "Percent70",
            "15": "Percent75",
            "16": "Percent80",
            "17": "Percent90",
            "41": "Plaid",
            "45": "Shingle",
            "49": "SmallCheckerBoard",
            "34": "SmallConfetti",
            "48": "SmallGrid",
            "52": "SolidDiamond",
            "47": "Sphere",
            "46": "Trellis",
            "1": "Vertical",
            "40": "Weave",
            "22": "WideDownwardDiagonal",
            "23": "WideUpwardDiagonal",
            "36": "ZigZag"
        }
        return (hatchStyles[hatchKey] || "");
    }

    return brushControl;
}