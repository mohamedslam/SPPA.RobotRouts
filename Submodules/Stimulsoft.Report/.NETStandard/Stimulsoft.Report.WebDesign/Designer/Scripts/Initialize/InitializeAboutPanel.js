
StiMobileDesigner.prototype.InitializeAboutPanel = function () {
    var aboutPanel = document.createElement("div");
    this.options.aboutPanel = aboutPanel;
    this.options.mainPanel.appendChild(aboutPanel);
    var jsObject = aboutPanel.jsObject = this;
    aboutPanel.className = "stiDesignerAboutPanel";
    aboutPanel.style.backgroundSize = "contain";
    aboutPanel.style.display = "none";

    var header = document.createElement("div");
    header.innerHTML = "Stimulsoft Designer";
    header.className = "stiDesignerAboutPanelHeader";
    aboutPanel.appendChild(header);

    var icon = document.createElement("img");
    icon.style.marginTop = "30px";
    icon.style.width = icon.style.height = "94px";
    StiMobileDesigner.setImageSource(icon, this.options, "About.svg");
    aboutPanel.appendChild(icon);

    var copyRight = document.createElement("div");
    copyRight.innerHTML = "Copyright 2003-" + new Date().getFullYear() + " Stimulsoft";
    copyRight.className = "stiDesignerAboutPanelCopyright";
    aboutPanel.appendChild(copyRight);

    var version = document.createElement("div");
    version.innerHTML = "Version " + this.options.productVersion.trim();
    if (!this.options.jsMode) version.innerHTML += ", " + this.options.frameworkType;
    // eslint-disable-next-line no-undef
    else if (typeof jsHelper != "undefined" && jsHelper.url) version.innerHTML += ", PHP";
    else version.innerHTML += ", JS";
    version.className = "stiDesignerAboutPanelVersion";
    aboutPanel.appendChild(version);

    var allRight = document.createElement("div");
    allRight.style.marginTop = "20px";
    allRight.innerHTML = "All rights reserved";
    allRight.className = "stiDesignerAboutPanelVersion";
    aboutPanel.appendChild(allRight);

    var userInfo = document.createElement("div");
    userInfo.className = "stiDesignerAboutPanelVersion";
    userInfo.style.marginTop = "20px";
    userInfo.style.fontWeight = "bold";
    aboutPanel.appendChild(userInfo);

    var sep = this.FormSeparator();
    sep.style.marginTop = "20px";
    aboutPanel.appendChild(sep);

    var stiLink = document.createElement("div");
    stiLink.innerHTML = "www.stimulsoft.com";
    stiLink.className = "stiDesignerAboutPanelStiLink";
    aboutPanel.appendChild(stiLink);

    stiLink.onclick = function (event) {
        if (event) {
            event.stopPropagation();
            event.preventDefault();
        }
        jsObject.openNewWindow("https://www.stimulsoft.com", undefined, undefined, false);
    };

    aboutPanel.ontouchend = function () { this.changeVisibleState(false); }
    aboutPanel.onclick = function () { this.changeVisibleState(false); }

    aboutPanel.changeVisibleState = function (state) {
        this.style.display = state ? "" : "none";
        this.updateUserInfo();
        jsObject.SetObjectToCenter(this);
        if (!jsObject.options.disabledPanels) jsObject.InitializeDisabledPanels();
        jsObject.options.disabledPanels[2].changeVisibleState(state);
        if (jsObject.options.previewMode && jsObject.options.viewer) {
            (jsObject.options.viewer.jsObject.controls || jsObject.options.viewer.jsObject.options).disabledPanels[1].style.display = state ? "" : "none";
        }
        if (jsObject.options.buttons["About"]) jsObject.options.buttons["About"].setSelected(state);
        this.visible = state;
        jsObject.options.currentForm = state ? this : null;
    }

    aboutPanel.updateUserInfo = function () {
        userInfo.style.display = "none";
        userInfo.style.color = "";

        if (jsObject.options.cloudMode) {
            userInfo.style.display = "";
            var userName = jsObject.options.cloudParameters && jsObject.options.cloudParameters.userName ? jsObject.options.cloudParameters.userName : "";
            var cloudPlan = jsObject.GetCloudPlanNumberValue();

            if (cloudPlan == 0 || cloudPlan == 512) {
                userInfo.style.color = "red";
                if (userName) userName += ", ";
                userInfo.innerHTML = userName + (cloudPlan == 512 ? "Subscription expired" : jsObject.getBackText(true) + " Version");
            }
            else {
                userInfo.innerHTML = userName;
            }
        }
        else if (jsObject.options.standaloneJsMode) {
            userInfo.style.display = "";
            var userName = jsObject.options.UserName || "";

            if (!jsObject.CheckUserProductsExpired(true, true)) {
                userInfo.style.color = "red";
                if (userName) userName += ", ";
                userInfo.innerHTML = userName + "Subscription expired";
            }
            else {
                if (jsObject.options.currentPage && !jsObject.options.currentPage.valid) {
                    userInfo.style.color = "red";
                    if (userName) userName += ", ";
                    userName += jsObject.getBackText(true) + " Version";
                }
                userInfo.innerHTML = userName;
            }
        }
        else if (jsObject.options.alternateValid === false) {
            userInfo.style.color = "red";
            userInfo.style.display = "";
            userInfo.innerHTML = jsObject.getBackText(true) + " Version";
        }
    }

    return aboutPanel;
}