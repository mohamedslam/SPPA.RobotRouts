
StiMobileDesigner.prototype.InitializeDictionarySettingsMenu = function (parentButton) {
    var jsObject = this;
    var dictMenu = this.VerticalMenu("dictionarySettingsMenu", parentButton, "Down");
    dictMenu.controls = {};

    var settingsControls = [
        ["commonGroup", this.FormBlockHeader(this.loc.Chart.Common), "0px"],
        ["createFieldOnDoubleClick", this.CheckBox(null, this.loc.PropertyMain.CreateFieldOnDoubleClick), "8px"],
        ["createLabel", this.CheckBox(null, this.loc.PropertyMain.CreateLabel), "0px 8px 8px 8px"],
        ["useAliases", this.CheckBox(null, this.loc.PropertyMain.UseAliases), "0px 8px 8px 8px"],
        ["sortGroup", this.FormBlockHeader(this.loc.PropertyMain.Sort), "0px"],
        ["ascending", this.VerticalMenuItem(dictMenu, "ascending", this.loc.FormBand.Ascending, "SortAZ.png", "ascending"), "2px"],
        ["descending", this.VerticalMenuItem(dictMenu, "descending", this.loc.FormBand.Descending, "SortZA.png", "descending"), "2px"],
        ["noSorting", this.VerticalMenuItem(dictMenu, "noSorting", this.loc.FormBand.NoSort, "Empty16.png", "noSorting"), "2px"]
    ]

    var setSettingsToCookie = function () {
        var settings = {};
        settings.createFieldOnDoubleClick = dictMenu.controls.createFieldOnDoubleClick.isChecked;
        settings.createLabel = dictMenu.controls.createLabel.isChecked;
        jsObject.options.useAliases = settings.useAliases = dictMenu.controls.useAliases.isChecked;
        settings.sort = dictMenu.controls.ascending.isSelected ? "ascending" : (dictMenu.controls.descending.isSelected ? "descending" : "noSorting");
        jsObject.SetCookie("StiMobileDesignerDictionarySettings", JSON.stringify(settings));
    }

    for (var i = 0; i < settingsControls.length; i++) {
        var control = settingsControls[i][1];
        control.name = settingsControls[i][0];
        dictMenu.controls[control.name] = control;
        control.style.margin = settingsControls[i][2];
        dictMenu.innerContent.appendChild(control);

        if (control.name != "commonGroup" && control.name != "sortGroup") {
            control.action = function () {
                if (this.name == "ascending" || this.name == "descending" || this.name == "noSorting") {
                    dictMenu.controls.ascending.setSelected(this.name == "ascending");
                    dictMenu.controls.descending.setSelected(this.name == "descending");
                    dictMenu.controls.noSorting.setSelected(this.name == "noSorting");
                    jsObject.options.dictionarySorting = this.name;
                    dictMenu.changeVisibleState(false);

                    if (jsObject.options.report) {
                        if (this.name == "noSorting")
                            jsObject.SendCommandSynchronizeDictionary();
                        else
                            jsObject.options.dictionaryTree.build(jsObject.options.report.dictionary, true);
                    }
                }
                setSettingsToCookie();

                if (this.name == "useAliases") {
                    jsObject.SendCommandUpdateReportAliases();
                }
            }
        }
    }

    var settings = {
        createFieldOnDoubleClick: false,
        createLabel: false,
        useAliases: false,
        sort: "noSorting"
    };

    var jsonSettings = this.GetCookie("StiMobileDesignerDictionarySettings");
    if (jsonSettings) settings = JSON.parse(jsonSettings);

    dictMenu.controls.createFieldOnDoubleClick.setChecked(settings.createFieldOnDoubleClick);
    dictMenu.controls.createLabel.setChecked(settings.createLabel);
    dictMenu.controls.useAliases.setChecked(jsObject.options.useAliasesDictionary == "Auto" ? settings.useAliases : jsObject.options.useAliasesDictionary == "True");

    dictMenu.controls.ascending.setSelected(settings.sort == "ascending");
    dictMenu.controls.descending.setSelected(settings.sort == "descending");
    dictMenu.controls.noSorting.setSelected(settings.sort == "noSorting" || !settings.sort);

    setSettingsToCookie();

    return dictMenu;
}