
StiMobileDesigner.prototype.InitializePropertiesSettingsMenu = function () {
    var jsObject = this;
    var menu = this.VerticalMenu("propertiesSettingsMenu", this.options.propertiesPanel.propertiesToolBar.controls["Settings"], "Down");
    menu.controls = {};

    var settingsControls = [
        ["localizeProperties", this.CheckBox(null, this.loc.FormDesigner.LocalizePropertyGrid), "8px"]
    ]

    for (var i = 0; i < settingsControls.length; i++) {
        var control = settingsControls[i][1];
        menu.controls[settingsControls[i][0]] = control;
        control.style.margin = settingsControls[i][2];
        menu.innerContent.appendChild(control);
    }

    if (this.options.cloudMode || this.options.standaloneJsMode) {
        menu.innerContent.appendChild(this.VerticalMenuSeparator(menu, "separator"));

        var itemsNames = [
            ["Beginner", this.loc.Report.Basic],
            ["BICreator", this.loc.Report.Standard],
            ["Developer", this.loc.Report.Professional]
        ];

        for (var i = 0; i < itemsNames.length; i++) {
            var control = this.CheckBox(null, itemsNames[i][1])
            control.key = itemsNames[i][0];
            control.style.margin = "8px";
            menu.controls[control.key] = control;
            menu.innerContent.appendChild(control);

            control.select = function () {
                for (var k = 0; k < itemsNames.length; k++) {
                    menu.controls[itemsNames[k][0]].setChecked(false);
                }
                this.setChecked(true);
            }

            control.action = function () {
                this.select();
                jsObject.ApplyDesignerSpecification(this.key);
            }
        }
    }

    menu.controls.localizeProperties.action = function () {
        jsObject.options.propertiesPanel.localizePropertyGrid = this.isChecked;
        jsObject.SetCookie("StimulsoftMobileDesignerLocalizePropertyGrid", this.isChecked ? "true" : "false");
        jsObject.options.propertiesPanel.updatePropertiesCaptions();
        jsObject.options.propertiesPanel.updatePropertiesValues();
    }

    menu.onshow = function () {
        menu.controls.localizeProperties.setChecked(jsObject.options.propertiesPanel.localizePropertyGrid);

        if ((jsObject.options.cloudMode || jsObject.options.standaloneJsMode)) {
            menu.controls.Beginner.setChecked(false);
            menu.controls.BICreator.setChecked(false);
            menu.controls.Developer.setChecked(false);

            if (menu.controls[jsObject.options.designerSpecification])
                menu.controls[jsObject.options.designerSpecification].select();
        }
    }

    return menu;
}