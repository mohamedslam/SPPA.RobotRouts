
StiJsViewer.prototype.InitializeAboutPanel = function () {
    var aboutPanel = document.createElement("div");
    this.controls.aboutPanel = aboutPanel;
    this.controls.mainPanel.appendChild(aboutPanel);
    var jsObject = aboutPanel.jsObject = this;
    aboutPanel.className = "stiJsViewerClearAllStyles stiJsViewerAboutPanel";
    aboutPanel.style.backgroundSize = "contain";
    aboutPanel.style.display = "none";

    var header = document.createElement("div");
    header.innerHTML = "Stimulsoft Reports";
    header.className = "stiJsViewerAboutPanelHeader";
    aboutPanel.appendChild(header);

    var icon = document.createElement("img");
    icon.style.marginTop = "30px";
    icon.style.width = icon.style.height = "94px";
    StiJsViewer.setImageSource(icon, this.options, this.collections, "About.png");
    aboutPanel.appendChild(icon);

    var copyRight = document.createElement("div");
    copyRight.innerHTML = "Copyright 2003-" + new Date().getFullYear() + " Stimulsoft";
    copyRight.className = "stiJsViewerAboutPanelCopyright";
    aboutPanel.appendChild(copyRight);

    var version = document.createElement("div");
    version.innerHTML = "Version " + this.options.productVersion.trim();
    if (!this.options.jsMode) version.innerHTML += ", " + this.options.frameworkType;
    // eslint-disable-next-line no-undef
    else if (typeof jsHelper != "undefined" && jsHelper.url) version.innerHTML += ", PHP";
    else version.innerHTML += ", JS";
    version.className = "stiJsViewerAboutPanelVersion";
    aboutPanel.appendChild(version);

    var allRight = document.createElement("div");
    allRight.innerHTML = "All rights reserved";
    allRight.className = "stiJsViewerAboutPanelVersion";
    aboutPanel.appendChild(allRight);

    var userInfo = document.createElement("div");
    userInfo.className = "stiJsViewerAboutPanelVersion";
    userInfo.style.marginTop = "20px";
    userInfo.style.fontWeight = "bold";
    aboutPanel.appendChild(userInfo);

    var sep = this.FormSeparator();
    sep.style.marginTop = "20px";
    aboutPanel.appendChild(sep);

    var stiLink = document.createElement("div");
    stiLink.innerHTML = "www.stimulsoft.com";
    stiLink.className = "stiJsViewerAboutPanelStiLink";
    aboutPanel.appendChild(stiLink);

    stiLink.onclick = function (event) {
        if (event) {
            event.stopPropagation();
            event.preventDefault();
        }
        aboutPanel.jsObject.openNewWindow("https://www.stimulsoft.com", undefined, undefined, false);
    };

    aboutPanel.ontouchend = function () { this.changeVisibleState(false); }
    aboutPanel.onclick = function () { this.changeVisibleState(false); }

    aboutPanel.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        aboutPanel.updateUserInfo();
        jsObject.setObjectToCenter(this);
        jsObject.controls.disabledPanels[2].changeVisibleState(state);
    }

    aboutPanel.updateUserInfo = function () {
        userInfo.style.display = "none";
        userInfo.style.color = "#444444";
        var userName = jsObject.options.licenseUserName || "";

        if (!jsObject.options.cloudMode &&
            !jsObject.options.serverMode &&
            !jsObject.options.standaloneJsMode &&
            jsObject.options.reportDesignerMode == false &&
            jsObject.options.licenseIsValid == false) {
            userInfo.style.display = "";
            userInfo.style.color = "red";
            if (userName) userName += ", ";
            userInfo.innerHTML = userName + jsObject.getBackText(true) + " Version";
        }
        else if (userName && (jsObject.options.cloudMode || jsObject.options.standaloneJsMode)) {
            userInfo.style.display = "";
            userInfo.innerHTML = userName;
        }
    }

    return aboutPanel;
}