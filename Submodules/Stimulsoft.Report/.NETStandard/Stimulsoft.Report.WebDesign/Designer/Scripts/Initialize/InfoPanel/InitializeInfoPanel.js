
StiMobileDesigner.prototype.InitializeInfoPanel = function () {
    var infoPanel = document.createElement("div");
    var jsObject = infoPanel.jsObject = this;
    infoPanel.id = this.options.mobileDesigner.id + "infoPanel";
    infoPanel.className = "stiDesignerInfoPanel";
    this.options.infoPanel = infoPanel;
    this.options.mainPanel.appendChild(infoPanel);
    infoPanel.style.display = "none";

    var innerTable = this.CreateHTMLTable();
    innerTable.style.width = innerTable.style.height = "100%";
    infoPanel.appendChild(innerTable);

    var closeButton = this.StandartSmallButton(null, null, null, "CloseForm.png", null, null, null);
    closeButton.style.height = "24px";
    closeButton.style.marginLeft = "12px";
    innerTable.addCell(closeButton).style.width = "1px";

    closeButton.action = function () {
        infoPanel.hide();
    }

    var textCell = innerTable.addCell();
    textCell.style.overflow = "hidden";
    textCell.style.paddingLeft = "12px";

    infoPanel.setText = function (text) {
        textCell.innerHTML = text;
    }

    var upgradeButton = this.FormButton(null, null, this.loc.Buttons.Upgrade);
    upgradeButton.style.margin = "0 12px 0 12px";
    upgradeButton.style.height = "24px";
    innerTable.addCell(upgradeButton).style.width = "1px";

    upgradeButton.action = function () {
        jsObject.openNewWindow("https://www.stimulsoft.com/" + (jsObject.options.helpLanguage || "en") + "/online-store");
    }

    infoPanel.checkState = function () {
        if (!jsObject.options.cloudMode && !jsObject.options.standaloneJsMode && !jsObject.options.alternateValid) {
            infoPanel.show();
        }
    }

    infoPanel.show = function () {
        if (this.visibleState !== false && this.style.display == "none") {
            this.visibleState = true;

            if (!jsObject.options.menus.fileMenu || !jsObject.options.menus.fileMenu.visible)
                infoPanel.changeVisibleState(true);

            var buildDate = new Date();
            try {
                if (jsObject.options.jsMode && typeof Stimulsoft != "undefined") {
                    // eslint-disable-next-line no-undef
                    var innerDate = Stimulsoft.StiVersion.created.innerDate;
                    if (innerDate["getFullYear"] && innerDate.getFullYear() > 2017)
                        // eslint-disable-next-line no-undef
                        buildDate = Stimulsoft.StiVersion.created.innerDate;
                }
                else if (jsObject.options.buildDate) {
                    buildDate = new Date(jsObject.options.buildDate);
                }
            }
            catch (e) {
                buildDate = new Date();
            }

            if (!jsObject.options.cloudMode) {
                var trDays = Math.floor(((new Date()).getTime() - buildDate.getTime()) / 1000 / 60 / 60 / 24);
                if (trDays > 30) closeButton.style.display = "none";
                if (trDays > 60) setTimeout(function () { infoPanel.showTrMessage(trDays > 120); }, 3000);
            }
        }
    }

    infoPanel.showTrMessage = function (trExp) {
        if (!jsObject.options.cloudMode && !jsObject.options.standaloneJsMode && !jsObject.options.alternateValid) {
            jsObject.InitializeNotificationForm(function (form) {
                form.show(trExp ? jsObject.loc.Notices.YourTrialHasExpired : jsObject.loc.Notices.YouUsingTrialVersion, null, "Notifications.Warning.png");

                form.upgradeButton.caption.innerHTML = jsObject.loc.Buttons.Ok.replace("&", "");

                if (trExp) {
                    form.upgradeButton.action = function () {
                        form.changeVisibleState(false);
                        window.location.href = "https://www.stimulsoft.com/en/online-store";
                    }

                    form.cancelAction = function () {
                        window.location.href = "https://www.stimulsoft.com/en/online-store";
                    }
                }
                else {
                    form.upgradeButton.action = function () {
                        form.changeVisibleState(false);
                    }
                }
            });
        }
    }

    infoPanel.hide = function () {
        this.visibleState = false;
        infoPanel.changeVisibleState(false);
    }

    infoPanel.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";

        var toolBar = jsObject.options.toolBar;
        var paintPanel = jsObject.options.paintPanel;
        var pagesPanel = jsObject.options.pagesPanel;
        var toolbox = jsObject.options.toolbox;
        var propertiesPanel = jsObject.options.propertiesPanel;
        var workPanel = jsObject.options.workPanel;

        paintPanel.style.top = toolBar.offsetHeight + pagesPanel.offsetHeight + workPanel.offsetHeight + this.offsetHeight + "px";
        pagesPanel.style.top = toolBar.offsetHeight + workPanel.offsetHeight + this.offsetHeight + "px";
        propertiesPanel.style.top = toolBar.offsetHeight + workPanel.offsetHeight + this.offsetHeight + "px";
        if (toolbox) toolbox.style.top = toolBar.offsetHeight + workPanel.offsetHeight + this.offsetHeight + "px";
        propertiesPanel.showButtonsPanel.style.top = (toolBar.offsetHeight + workPanel.offsetHeight + infoPanel.offsetHeight + 40) + "px";
    }

    infoPanel.setText("You are using the trial version of Stimulsoft Reports and Dashboards. To use the software in production you should purchase a license.");
}