
StiMobileDesigner.prototype.InitializeFileMenuCheckForUpdatePanel = function (parentContainer) {
    var jsObject = this;
    var panel = document.createElement("div");
    panel.className = "stiDesignerAccountChildPanel";
    panel.controls = {};
    this.options.checkForUpdatePanel = panel;

    jsObject.AddProgressToControl(panel);

    var footerPanel = document.createElement("div");
    footerPanel.className = "stiDesignerAccountFooterPanel";
    panel.appendChild(footerPanel);

    var whatsNewButton = this.HiperLinkButton(null, this.loc.Report.WhatsNewInVersion, 23);
    whatsNewButton.style.display = "none";
    whatsNewButton.style.margin = "12px";
    whatsNewButton.style.fontSize = "12px";
    footerPanel.appendChild(whatsNewButton);
    panel.controls.whatsNewButton = whatsNewButton;

    var container = this.EasyContainer(600, 400);
    container.style.height = "calc(100% - 48px)";
    panel.appendChild(container);

    panel.show = function () {
        container.clear();
        this.style.display = "";
        this.visible = true;
        this.style.left = jsObject.FindPosX(parentContainer, "stiDesignerMainPanel") + "px";
        this.style.top = jsObject.FindPosY(parentContainer, "stiDesignerMainPanel") + "px";

        if ((jsObject.options.cloudMode && !jsObject.options.cloudParameters.sessionKey) || (jsObject.options.standaloneJsMode && !jsObject.options.SessionKey)) {
            jsObject.options.forms.authForm.show();
            return;
        }

        this.progress.show();

        jsObject.SendCloudCommand("DeveloperBuildGet", {},
            function (data) {
                panel.progress.hide();
                var isEmpty = true;

                if (data.ResultBuilds && data.ResultBuilds.All && data.ResultBuilds.All.length > 0) {
                    var currentVersion = jsObject.options.standaloneJsMode ? Stimulsoft.StiVersion.version : jsObject.options.shortProductVersion;

                    for (var i = 0; i < data.ResultBuilds.All.length; i++) {
                        if (currentVersion && jsObject.AllowBuildVersion(currentVersion, data.ResultBuilds.All[i].Version)) {
                            container.appendChild(jsObject.UpdateFormItem(data.ResultBuilds.All[i], i != 0));
                            isEmpty = false;
                        }
                        if (currentVersion == data.ResultBuilds.All[i].Version) {
                            whatsNewButton.style.display = "";
                            whatsNewButton.caption.innerHTML = jsObject.loc.Report.WhatsNewInVersion.replace("{0}", currentVersion);
                            var currentBuild = data.ResultBuilds.All[i];

                            whatsNewButton.action = function () {
                                this.jsObject.InitializeModificationsForm().show(currentBuild);
                            }
                        }
                    }
                    if (isEmpty) {
                        var emptyItem = jsObject.CreateHTMLTable();
                        emptyItem.className = "stiUpdateFormEmptyItem";
                        emptyItem.addTextCell(jsObject.loc.Report.NoNewVersions).style.textAlign = "center";
                        emptyItem.style.width = "100%";
                        emptyItem.style.height = "100%";
                        container.appendChild(emptyItem);
                    }
                }
            },
            function (data, msg) {
                panel.showError(data, msg);
            });
    }

    panel.hide = function () {
        this.style.display = "none";
        this.visible = false;
    }

    panel.showError = function (data, msg) {
        panel.progress.hide();
        if (msg || data) {
            var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
            errorMessageForm.show(msg || jsObject.formatResultMsg(data));
        }
    }

    return panel;
}

StiMobileDesigner.prototype.UpdateFormItem = function (developerBuild, hideImage) {
    var item = document.createElement("div");
    item.className = "stiUpdateFormItem";
    item.developerBuild = developerBuild;

    var innerTable = this.CreateHTMLTable();
    innerTable.style.width = "100%";
    item.appendChild(innerTable);

    var img = document.createElement("img");
    img.style.margin = "10px 20px 0px 20px";
    StiMobileDesigner.setImageSource(img, this.options, "Update.DesignerLogo64.png");
    img.style.width = img.style.height = "64px";
    img.style.visibility = hideImage ? "hidden" : "visible";
    var imgCell = innerTable.addCell(img);
    imgCell.style.verticalAlign = "top";
    imgCell.style.width = "1px";

    var textCell = innerTable.addCell();
    textCell.style.verticalAlign = "top";

    var header = document.createElement("div");
    header.className = "stiUpdateFormItemHeader";
    header.innerHTML = "Stimulsoft Designer " + developerBuild.Version;
    textCell.appendChild(header);

    var date = document.createElement("div");
    date.className = "stiUpdateFormItemDate";
    date.innerHTML = this.JSONDateFormatToDate(developerBuild.Date, "dd.MM.yyyy");
    textCell.appendChild(date);

    var description = document.createElement("div");
    description.className = "stiUpdateFormItemDescription";
    description.innerHTML = developerBuild.Description;
    textCell.appendChild(description);

    var showMoreButton = this.HiperLinkButton(null, this.loc.Buttons.ShowMore, 23);
    showMoreButton.style.fontSize = "12px";
    showMoreButton.style.margin = "4px 0 0 13px";
    showMoreButton.style.display = "inline-block";
    textCell.appendChild(showMoreButton);

    showMoreButton.action = function () {
        this.jsObject.InitializeModificationsForm().show(developerBuild);
    }

    var downloadButton = this.FormButton(null, null, this.loc.NuGet.DownloadAndInstall);
    downloadButton.style.margin = "8px";
    downloadButton.style.display = "inline-block";
    downloadButton.caption.style.padding = "0 10px 0 10px";
    var buttonCell = innerTable.addCellInNextRow();
    buttonCell.style.textAlign = "right";
    buttonCell.setAttribute("colspan", "2");
    buttonCell.appendChild(downloadButton);

    downloadButton.action = function () {
        this.jsObject.openNewWindow(developerBuild.Url);
    }

    return item;
}

StiMobileDesigner.prototype.AllowBuildVersion = function (currentVersionStr, buildVersionStr) {
    var currVers = this.DecodeBuildVersion(currentVersionStr);
    var buildVers = this.DecodeBuildVersion(buildVersionStr);

    return currVers.major < buildVers.major ||
        (currVers.major == buildVers.major && currVers.minor < buildVers.minor) ||
        (currVers.major == buildVers.major && currVers.minor == buildVers.minor && currVers.build < buildVers.build);
}

StiMobileDesigner.prototype.DecodeBuildVersion = function (version) {
    if (!version || version.trim().length == 0) return false;
    var strs = version.split('.');
    if (strs.length < 3) return false;

    var major = parseInt(strs[0]);
    var minor = parseInt(strs[1]);
    var build = parseInt(strs[2]);
    if (major == null || minor == null || build == null) return false;

    return {
        major: major,
        minor: minor,
        build: build
    };
}