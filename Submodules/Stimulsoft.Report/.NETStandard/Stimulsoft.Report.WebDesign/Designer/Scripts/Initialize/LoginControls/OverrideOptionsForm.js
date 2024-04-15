
StiMobileDesigner.prototype.GetThemeTonesItems = function () {
    var items = [];
    items.push({ name: "White", color: "white" });
    items.push({ name: "LightGray", color: "#f6f6f6" });
    items.push({ name: "DarkGray", color: "#e5e5e5" });
    items.push({ name: "VeryDarkGray", color: "#6a6a6a" });

    return items;
}

StiMobileDesigner.prototype.GetThemeAccentsItems = function () {
    var items = [];
    items.push({ name: "Blue", color: "#2b579a" });
    items.push({ name: "Carmine", color: "#a4373a" });
    items.push({ name: "Green", color: "#207245" });
    items.push({ name: "Orange", color: "#d14625" });
    items.push({ name: "Purple", color: "#8653a5" });
    items.push({ name: "Teal", color: "#077568" });
    items.push({ name: "Violet", color: "#6d3069" });

    return items;
}

StiMobileDesigner.prototype.GetCurrentThemeIdent = function () {
    var theme = this.options.theme || "Office2022WhiteBlue";
    if (theme.indexOf("Office2013") >= 0) return "Office2013";
    else return "Office2022";
}

StiMobileDesigner.prototype.GetCurrentThemeTone = function () {
    var theme = this.options.theme || "Office2022WhiteBlue";
    if (theme.indexOf("VeryDarkGray") >= 0) return "VeryDarkGray";
    else if (theme.indexOf("DarkGray") >= 0) return "DarkGray";
    else if (theme.indexOf("LightGray") >= 0) return "LightGray";
    else if (theme.indexOf("Black") >= 0) return "Black";
    else return "White";
}

StiMobileDesigner.prototype.GetCurrentThemeAccent = function () {
    var theme = this.options.theme || "Office2022WhiteBlue";
    return theme.replace(this.GetCurrentThemeTone(), "").replace("Office2013", "").replace("Office2022", "");
}

StiMobileDesigner.prototype.getSelectedButton = function (groupName) {
    for (var i in this.options.buttons) {
        if (this.options.buttons[i].groupName == groupName && this.options.buttons[i].isSelected) {
            return this.options.buttons[i];
        }
    }
}

StiMobileDesigner.prototype.OverrideOptionsForm = function () {
    var jsObject = this;

    this.OldInitializeOptionsForm = this.InitializeOptionsForm;
    this.InitializeOptionsForm = function () {
        this.OldInitializeOptionsForm(function (optionsForm) {
            var buttonName = "Gui";
            var buttonsPanel = optionsForm.container.firstChild.rows[0].firstChild;
            var panelsContainer = optionsForm.container.firstChild.rows[0].childNodes[1];

            //Add button
            var button = jsObject.FormTabPanelButton(null, "GUI", "OptionsForm.DesignerOptionsGui.png", "GUI", null, { width: 24, height: 24 }, 34);
            optionsForm.mainButtons[buttonName] = button;
            buttonsPanel.appendChild(button);
            button.panelName = buttonName;

            button.action = function () {
                optionsForm.setMode(this.panelName);
            }

            //Add panel
            var container = document.createElement("Div");
            container.className = "stiDesignerEditFormPanel";
            container.style.display = "none";
            container.style.overflow = "hidden";
            panelsContainer.appendChild(container);
            optionsForm.panels[buttonName] = container;

            container.appendChild(jsObject.FormBlockHeader(jsObject.loc.Gui.colorpicker_themecolorslabel));

            var innerTable = jsObject.CreateHTMLTable();
            innerTable.style.margin = "6px 0 6px 0";
            container.appendChild(innerTable);

            var themeItems = [
                jsObject.Item("Office2013", "Office2013", null, "Office2013"),
                jsObject.Item("Office2022", "Office2022", null, "Office2022")
            ];
            
            var themeControl = jsObject.DropDownList("optionsFormTheme", 180, null, themeItems, true, false, null, true);
            themeControl.style.margin = "6px 0 6px 0";
            themeControl.setKey(jsObject.GetCurrentThemeIdent());

            innerTable.addTextCellInLastRow(jsObject.loc.PropertyMain.Style).className = "stiDesignerCaptionControlsBigIntervals";
            innerTable.addCellInLastRow(themeControl);

            innerTable.addTextCellInNextRow(jsObject.loc.Options.LabelBackground.replace(":", "")).className = "stiDesignerCaptionControlsBigIntervals";

            var themeTonesPanel = $("<div></div>")[0];
            themeTonesPanel.style.height = "28px";
            themeTonesPanel.style.margin = "6px 0 6px 0";
            var themeTones = jsObject.GetThemeTonesItems();
            var currentThemeTone = jsObject.GetCurrentThemeTone();

            for (var i = 0; i < themeTones.length; i++) {
                var button = jsObject.ThemeColorButton(themeTones[i].color, "themeTone" + themeTones[i].color, "themeTone", themeTones[i].name);
                if (currentThemeTone == themeTones[i].name || (currentThemeTone == "Black" && themeTones[i].name == "VeryDarkGray")) {
                    button.setSelected(true);
                }
                themeTonesPanel.appendChild(button);
            }
            innerTable.addCellInLastRow(themeTonesPanel);

            innerTable.addTextCellInNextRow(jsObject.loc.Options.LabelForeground.replace(":", "")).className = "stiDesignerCaptionControlsBigIntervals";

            var themeAccentPanel = $("<div></div>")[0];
            themeAccentPanel.style.height = "28px";
            themeAccentPanel.style.margin = "6px 0 6px 0";
            var themeAccents = jsObject.GetThemeAccentsItems();

            for (var i = 0; i < themeAccents.length; i++) {
                var button = jsObject.ThemeColorButton(themeAccents[i].color, "themeAccent" + themeAccents[i].name, "themeAccent", themeAccents[i].name);
                if (jsObject.GetCurrentThemeAccent() == themeAccents[i].name) {
                    button.setSelected(true);
                }
                themeAccentPanel.appendChild(button);
            }

            innerTable.addCellInLastRow(themeAccentPanel);

            optionsForm.restoreDefaults.action = function () {
                optionsForm.fill(jsObject.options.defaultDesignerOptions);
                for (var i = 0; i < themeTonesPanel.childNodes.length; i++) {
                    var button = themeTonesPanel.childNodes[i];
                    if (button.value == "White") button.onclick();
                }
                for (var i = 0; i < themeAccentPanel.childNodes.length; i++) {
                    var button = themeAccentPanel.childNodes[i];
                    if (button.value == "Blue") button.onclick();
                }
            }

            optionsForm.oldAction = optionsForm.action;

            optionsForm.action = function () {
                optionsForm.oldAction();

                var themeIdent = themeControl.key;
                var themeTone = jsObject.getSelectedButton("themeTone").value;
                var themeAccent = jsObject.getSelectedButton("themeAccent").value;

                if (themeIdent == "Office2022" && themeTone == "VeryDarkGray") {
                    themeTone = "Black";
                }

                var newTheme = themeIdent + themeTone + themeAccent;

                if (jsObject.options.theme != newTheme) {
                    var messageForm = jsObject.InitializeMessageForm();

                    messageForm.messageText = jsObject.options.helpLanguage == "ru" ? "Перезагрузить дизайнер, чтобы применить новую тему?" : "Restart designer to apply new theme?";
                    messageForm.caption.innerHTML = jsObject.loc.FormDesigner.title.toUpperCase();
                    messageForm.buttonNo.style.display = "none";
                    messageForm.buttonOk.caption.innerHTML = jsObject.loc.Buttons.Ok.replace("&", "");
                    messageForm.changeVisibleState(true);

                    messageForm.action = function (state) {
                        if (state) {
                            var cloudParameters = jsObject.options.cloudParameters;

                            var designerParams = {
                                cp: jsObject.GetCloudPlanNumberValue(),
                                localizationName: cloudParameters.localizationName || jsObject.options.cultureName,
                                themeName: newTheme
                            }

                            if (cloudParameters.sessionKey) {
                                designerParams.sessionKey = cloudParameters.sessionKey;
                                designerParams.userKey = cloudParameters.userKey;

                                if (cloudParameters.reportTemplateItemKey) {
                                    designerParams.reportTemplateItemKey = cloudParameters.reportTemplateItemKey;
                                    designerParams.reportType = cloudParameters.isDashboard ? "dbs" : "rep";
                                    if (cloudParameters.versionKey) designerParams.versionKey = cloudParameters.versionKey;
                                }
                            }
                            else {
                                designerParams.demomode = true;
                            }

                            if (cloudParameters.reportName) designerParams.reportName = StiBase64.encode(cloudParameters.reportName);

                            jsObject.PostForm({ "designerParams": StiBase64.encode(JSON.stringify(designerParams)) }, document, window.location.href, true);
                        }
                    }
                }
            }

            optionsForm.show();
        });
    }
}