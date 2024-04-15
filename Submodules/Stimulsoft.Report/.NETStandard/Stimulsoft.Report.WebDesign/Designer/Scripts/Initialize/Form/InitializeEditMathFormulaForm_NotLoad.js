
StiMobileDesigner.prototype.InitializeEditMathFormulaForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("editMathFormula", this.loc.Components.StiMathFormula, 1, this.HelpLinks["mathFormula"]);

    var editor = this.TextArea(null, 600, 150);
    editor.style.margin = "12px 12px 0 12px";
    form.container.appendChild(editor);

    var tabs = [];
    tabs.push({ name: "Basic", caption: this.loc.Report.Basic });
    tabs.push({ name: "Operators", caption: this.loc.MathFormula.Operators });
    tabs.push({ name: "Alphabets", caption: this.loc.MathFormula.Alphabets });
    tabs.push({ name: "Maths", caption: this.loc.MathFormula.Maths });
    tabs.push({ name: "Functions", caption: this.loc.PropertyMain.Functions });
    tabs.push({ name: "Arrows", caption: this.loc.MathFormula.Arrows });
    tabs.push({ name: "Formulas", caption: this.loc.MathFormula.Formulas });

    var tabbedPane = this.TabbedPane("editMathFormulaTabbedPane", tabs, "stiDesignerStandartTab");
    tabbedPane.style.margin = "12px";
    form.container.appendChild(tabbedPane);

    var description = document.createElement("div")
    description.className = "stiDesignerTextContainer";
    description.style.margin = "12px";
    description.innerHTML = this.loc.Messages.LatexFormat;
    form.container.appendChild(description);

    for (var i = 0; i < tabs.length; i++) {
        var tabsPanel = tabbedPane.tabsPanels[tabs[i].name];
        tabsPanel.style.width = "602px";
        tabsPanel.style.height = "250px";
        tabsPanel.style.overflow = "auto";
        tabsPanel.style.lineHeight = "0";

        tabsPanel.clear = function () {
            while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        }
    }

    tabbedPane.clear = function () {
        for (var i = 0; i < tabs.length; i++) {
            this.tabsPanels[tabs[i].name].clear();
        }
    }

    form.fillContainers = function (mathGroups) {
        if (mathGroups) {
            for (var i = 0; i < tabs.length; i++) {
                var groupType = tabs[i].name;
                var groupItems = mathGroups[groupType];
                var tabsPanel = tabbedPane.tabsPanels[groupType];
                if (groupItems && tabsPanel) {
                    for (var k = 0; k < groupItems.length; k++) {
                        var button = form.mathFormulaButton(groupItems[k]);
                        tabsPanel.appendChild(button);
                    }
                }
            }
        }
    }

    form.mathFormulaButton = function (itemObject) {
        var button = jsObject.SmallImageButtonWithBorder(null, null, " ");
        button.image.style.width = button.image.style.height = "auto";

        var iconData = itemObject.icon;

        if (iconData.indexOf("imageSizes=") == 0) {
            var imageSizes = iconData.substring(0, iconData.indexOf("data:image")).replace("imageSizes=", "").split(";");
            button.image.style.width = imageSizes[0] + "px";
            button.image.style.height = imageSizes[1] + "px";
            iconData = iconData.substring(iconData.indexOf("data:image"));
        }

        button.image.src = iconData;
        button.style.display = "inline-block";
        button.style.width = button.style.height = "auto";
        button.style.margin = "6px 0 0 6px";
        button.mathValue = itemObject.value;

        button.action = function () {
            editor.insertText(this.mathValue);
        }

        return button;
    }

    form.show = function (component) {
        this.currentComponent = component;
        this.changeVisibleState(true);

        tabbedPane.clear();
        editor.value = component.properties.laTexExpression ? StiBase64.decode(component.properties.laTexExpression) : "";
        editor.focus();

        if (!jsObject.mathGroups) {
            jsObject.SendCommandToDesignerServer("GetMathFormulaInfo", { imagesScalingFactor: jsObject.options.imagesScalingFactor }, function (answer) {
                if (answer.mathGroups) {
                    jsObject.mathGroups = answer.mathGroups;
                    form.fillContainers(answer.mathGroups);
                }
            });
        }
        else {
            form.fillContainers(jsObject.mathGroups);
        }
    }

    form.action = function () {
        this.changeVisibleState(false);
        this.currentComponent.properties.laTexExpression = StiBase64.encode(editor.value);
        jsObject.SendCommandSendProperties(this.currentComponent, ["laTexExpression"]);
    }

    return form;
}