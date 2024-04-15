
StiJsViewer.prototype.checkTrExp = function () {
    var jsObject = this;

    if (!jsObject.options.cloudMode && !jsObject.options.serverMode && !jsObject.options.standaloneJsMode && jsObject.options.reportDesignerMode == false && jsObject.options.alternateValid == false) {
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

        var trDays = Math.floor(((new Date()).getTime() - buildDate.getTime()) / 1000 / 60 / 60 / 24);
        if (trDays > 60) {
            setTimeout(function () {
                if (jsObject.options.jsMode && !jsObject.options.standaloneJsMode && typeof Stimulsoft != "undefined" && Stimulsoft.Base.StiLicense.key != null) {
                    return;
                }

                var notificationForm = jsObject.controls.forms.notificationForm || jsObject.InitializeNotificationForm();
                notificationForm.show(trDays > 120 ? jsObject.collections.loc.NoticesYourTrialHasExpired : jsObject.collections.loc.NoticesYouUsingTrialVersion, null, "Notifications.Warning.png");
                notificationForm.upgradeButton.caption.innerHTML = jsObject.collections.loc.ButtonOk;

                if (trDays > 120) {
                    notificationForm.upgradeButton.action = function () {
                        window.location.href = "https://www.stimulsoft.com/en/online-store";
                    }

                    notificationForm.cancelAction = function () {
                        window.location.href = "https://www.stimulsoft.com/en/online-store";
                    }
                }
                else {
                    notificationForm.upgradeButton.action = function () {
                        notificationForm.changeVisibleState(false);
                    }
                }
            }, 3000);
        }
    }
}

StiJsViewer.prototype.blockViewer = function () {
    var notificationForm = this.controls.forms.notificationForm || this.InitializeNotificationForm();
    notificationForm.show(
        "Your Stimulsoft Cloud subscription has been expired!",
        "Please update your subscription",
        "Notifications.Blocked.png"
    );
    for (var i = 0; i < this.controls.mainPanel.childNodes.length; i++) {
        var child = this.controls.mainPanel.childNodes[i];
        if (child != notificationForm && child.style) child.style.display = "none";
    }
    notificationForm.cancelAction = function () {
        window.location.href = "https://www.stimulsoft.com/en/online-store#cloud/cloud";
    }
}

StiJsViewer.prototype.checkCloudAuthorization = function (action) {
    var jsDesigner = this.options.jsDesigner;
    if (!this.options.cloudMode || !jsDesigner) return true;
    if (jsDesigner.options.cloudParameters && !jsDesigner.options.cloudParameters.sessionKey) {
        var actionText = action == "open" ? "before opening report file" : (action == "export" ? "before exporting report file" : "");
        var text = "Please login using your Stimulsoft account credentials or register a new account " + actionText + ".";
        var loginInfoForm = this.controls.forms.loginInfoForm;
        if (!loginInfoForm) {
            loginInfoForm = this.BaseForm("loginInfoForm", this.collections.loc["AuthorizationWindowTitleLogin"], 1);
            loginInfoForm.buttonsPanel.style.display = "none";

            var loginPanel = jsDesigner.CloudDemoPanel(text);
            loginPanel.image.style.display = "none";
            loginPanel.style.height = "auto";
            loginPanel.style.margin = "15px 0 15px 0";
            loginInfoForm.loginPanel = loginPanel;
            loginInfoForm.container.appendChild(loginPanel);

            loginPanel.action = function () {
                loginInfoForm.changeVisibleState(false);
            }
        }
        loginInfoForm.loginPanel.textContainer.innerHTML = text;
        loginInfoForm.changeVisibleState(true);
        return false;
    }
    return true;
};

StiJsViewer.prototype.getDefaultLocalization = function () {
    var browserLanguage = typeof navigator !== "undefined" ? navigator.defaultLocalization || navigator.language || navigator.browserLanguage : null;
    if (browserLanguage && browserLanguage.length > 1) return browserLanguage.substring(0, 2);
    return "en";
};

StiJsViewer.prototype.getImagesScalingFactor = function () {
    var devicePixelRatio = window.devicePixelRatio || (window.deviceXDPI && window.logicalXDPI ? window.deviceXDPI / window.logicalXDPI : 1);
    if (!devicePixelRatio || devicePixelRatio <= 1) return "1";
    else return (Math.round(devicePixelRatio * 100) / 100).toString();
};

StiJsViewer.prototype.scrollToAnchor = function (anchorName, componentGuid) {
    var aHyperlinks = this.controls.reportPanel.getElementsByTagName("a");
    var identicalAnchors = [];
    if (anchorName) anchorName = anchorName.replace(/!!#92/g, "\\");

    if (componentGuid) {
        for (var i = 0; i < aHyperlinks.length; i++) {
            if (aHyperlinks[i].getAttribute("guid") == componentGuid)
                identicalAnchors.push(aHyperlinks[i]);
        }
    }

    if (identicalAnchors.length == 0) {
        var guidIndex = anchorName.indexOf("#GUID#");
        if (identicalAnchors.length == 0) {
            var aHyperlinks = this.controls.reportPanel.getElementsByTagName("a");
            for (var i = 0; i < aHyperlinks.length; i++) {
                if (aHyperlinks[i].name && ((guidIndex >= 0 && (aHyperlinks[i].name.indexOf(anchorName.substring(guidIndex + 6)) >= 0 || anchorName.substring(0, guidIndex) == aHyperlinks[i].name)) || aHyperlinks[i].name == anchorName))
                    identicalAnchors.push(aHyperlinks[i]);
            }
        }
    }

    if (identicalAnchors.length > 0) {
        var jsObject = this;

        var anchor = identicalAnchors[0];
        var anchorParent = anchor.parentElement || anchor;
        var anchorHeight = anchorParent.offsetHeight;
        var anchorOffsetTop = anchorParent.offsetTop;
        for (var i = 0; i < identicalAnchors.length; i++) {
            var nextAnchorParent = identicalAnchors[i].parentElement || identicalAnchors[i];
            if (nextAnchorParent.offsetTop > anchorOffsetTop)
                anchorHeight = Math.max(anchorHeight, nextAnchorParent.offsetTop - anchorOffsetTop + nextAnchorParent.offsetHeight);
        }

        var date = new Date();
        var endTime = date.getTime() + this.options.scrollDuration;
        var targetTop = this.FindPosY(anchor, this.options.appearance.scrollbarsMode ? "stiJsViewerReportPanel" : null, true) - anchorParent.offsetHeight * 2;

        this.ShowAnimationForScroll(this.controls.reportPanel, targetTop, endTime, function () {
            var page = jsObject.getPageFromAnchorElement(anchor);
            var anchorParentTopPos = jsObject.FindPosY(anchorParent, "stiJsViewerReportPanel", true);
            var pageTopPos = page ? jsObject.FindPosY(page, "stiJsViewerReportPanel", true) : anchorParentTopPos;

            jsObject.removeBookmarksLabel();

            var label = document.createElement("div");
            jsObject.controls.bookmarksLabel = label;
            label.className = "stiJsViewerBookmarksLabel";

            var labelMargin = 20 * (jsObject.reportParams.zoom / 100);
            var labelWidth = page ? page.offsetWidth - labelMargin - 6 : anchorParent.offsetWidth;
            var labelHeight = anchorHeight - 2;
            label.style.width = labelWidth + "px";
            label.style.height = labelHeight + "px";

            var pageLeftMargin = page.margins ? jsObject.StrToInt(page.margins[3]) : 0;
            var pageTopMargin = page.margins ? jsObject.StrToInt(page.margins[0]) : 0;
            label.style.marginLeft = (labelMargin / 2 - pageLeftMargin) + "px";
            label.style.marginTop = (anchorParentTopPos - pageTopPos - pageTopMargin - (jsObject.reportParams.zoom / 100) - 1) + "px";

            page.insertBefore(label, page.childNodes[0]);
        });
    }
}

StiJsViewer.prototype.isWholeWord = function (str, word) {
    var symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
    var index = str.indexOf(word);
    var preSymbol = str.substring(index - 1, index);
    var nextSymbol = str.substring(index + word.length, index + word.length + 1);

    return ((preSymbol == "" || symbols.indexOf(preSymbol) == -1) && (nextSymbol == "" || symbols.indexOf(nextSymbol) == -1));
}

StiJsViewer.prototype.goToFindedElement = function (findLabel) {
    if (findLabel && findLabel.ownerElement) {
        var targetTop = this.FindPosY(findLabel.ownerElement, this.options.appearance.scrollbarsMode ? "stiJsViewerReportPanel" : null, true) - findLabel.ownerElement.offsetHeight - 50;
        var d = new Date();
        var endTime = d.getTime() + this.options.scrollDuration;
        var this_ = this;
        this.ShowAnimationForScroll(this.controls.reportPanel, targetTop, endTime, function () { });
    }
}

StiJsViewer.prototype.hideFindLabels = function () {
    for (var i = 0; i < this.controls.findHelper.findLabels.length; i++) {
        var findLabel = this.controls.findHelper.findLabels[i];
        var parentElement = findLabel.parentElement;
        parentElement.removeChild(findLabel);
        if (parentElement.oldPositionStyle) parentElement.style.position = parentElement.oldPositionStyle;
    }
    this.controls.findHelper.findLabels = [];
    this.options.findMode = false;
}

StiJsViewer.prototype.showFindLabels = function (text) {
    this.hideFindLabels();
    this.options.findMode = true;
    this.options.changeFind = false;
    this.controls.findHelper.lastFindText = text;
    var matchCase = this.controls.findPanel && this.controls.findPanel.controls.matchCase.isSelected;
    var matchWholeWord = this.controls.findPanel && this.controls.findPanel.controls.matchWholeWord.isSelected;
    var pages = this.controls.reportPanel.pages;
    var reportDisplayMode = this.options.displayModeFromReport || this.options.appearance.reportDisplayMode;

    var isRelativeParent = function (el) {
        return (el.parentElement && el.parentElement.style && el.parentElement.style.position == "relative");
    }

    for (var i = 0; i < pages.length; i++) {
        var page = pages[i];
        var pageElements = page.getElementsByTagName('*');
        for (var k = 0; k < pageElements.length; k++) {
            var innerText = pageElements[k].innerHTML;
            if (innerText && pageElements[k].childNodes.length == 1 && pageElements[k].childNodes[0].nodeName == "#text") {
                if (!matchCase) {
                    innerText = innerText.toLowerCase();
                    text = text.toLowerCase();
                }
                if (innerText.indexOf(text) >= 0) {
                    if (matchWholeWord && !this.isWholeWord(innerText, text)) {
                        continue;
                    }
                    var label = document.createElement("div");
                    label.ownerElement = pageElements[k];
                    label.className = "stiJsViewerFindLabel";
                    label.style.width = (pageElements[k].offsetWidth - 4) + "px";
                    var labelHeight = pageElements[k].offsetHeight - 4;
                    label.style.height = labelHeight + "px";
                    label.style.top = "0px";
                    label.style.left = "0px";
                    label.ownerElement.oldPositionStyle = label.ownerElement.style.position;
                    if (label.ownerElement.style.position != "absolute" && label.ownerElement.style.position != "fixed" && !(reportDisplayMode == "Div" && isRelativeParent(label.ownerElement))) {
                        label.ownerElement.style.position = "relative";
                    }
                    pageElements[k].insertBefore(label, pageElements[k].childNodes[0]);

                    label.setSelected = function (state) {
                        this.isSelected = state;
                        this.style.border = "2px solid " + (state ? "red" : "#8a8a8a");
                    }

                    if (this.controls.findHelper.findLabels.length == 0) label.setSelected(true);
                    this.controls.findHelper.findLabels.push(label);
                }
            }
        }
    }

    if (this.controls.findHelper.findLabels.length > 0) this.goToFindedElement(this.controls.findHelper.findLabels[0]);
}

StiJsViewer.prototype.selectFindLabel = function (direction) {
    var labels = this.controls.findHelper.findLabels;
    if (labels.length == 0) return;
    var selectedIndex = 0;
    for (var i = 0; i < labels.length; i++) {
        if (labels[i].isSelected) {
            labels[i].setSelected(false);
            selectedIndex = i;
            break;
        }
    }
    if (direction == "Next") {
        selectedIndex++;
        if (selectedIndex > labels.length - 1) selectedIndex = 0;
    }
    else {
        selectedIndex--;
        if (selectedIndex < 0) selectedIndex = labels.length - 1;
    }
    labels[selectedIndex].setSelected(true);
    this.goToFindedElement(labels[selectedIndex]);
}

StiJsViewer.prototype.scrollToPage = function (pageNumber) {
    var commonPagesHeight = 0;
    for (var i = 0; i < pageNumber; i++) {
        commonPagesHeight += this.controls.reportPanel.pages[i].offsetHeight + 10;
    }
    if (!this.options.appearance.scrollbarsMode) commonPagesHeight += this.FindPosY(this.controls.reportPanel, null, true);

    var d = new Date();
    var endTime = d.getTime() + this.options.scrollDuration;
    this.ShowAnimationForScroll(this.controls.reportPanel, commonPagesHeight, endTime);
}

StiJsViewer.prototype.removeBookmarksLabel = function () {
    if (this.controls.bookmarksLabel) {
        this.controls.bookmarksLabel.parentElement.removeChild(this.controls.bookmarksLabel);
        this.controls.bookmarksLabel = null;
    }
}

StiJsViewer.prototype.getPageFromAnchorElement = function (anchorElement) {
    var obj = anchorElement;
    while (obj.parentElement) {
        if (obj.className && obj.className.indexOf("stiJsViewerPage") == 0) {
            return obj;
        }
        obj = obj.parentElement;
    }
    return obj;
}

StiJsViewer.prototype.isContainted = function (array, item) {
    for (var index in array)
        if (item == array[index]) return true;

    return false;
}

StiJsViewer.prototype.IsTouchDevice = function () {
    return ('ontouchstart' in document.documentElement);
}

StiJsViewer.prototype.IsMobileDevice = function () {
    return /iPhone|iPad|iPod|Macintosh|Android|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);
}

StiJsViewer.prototype.getOSName = function () {
    var appVersion = navigator ? navigator.appVersion : null;

    if (!appVersion) return null;
    if (appVersion.indexOf("Win") != -1) return "Windows";
    if (appVersion.indexOf("Mac") != -1) return "MacOS";
    if (appVersion.indexOf("X11") != -1) return "UNIX";
    if (appVersion.indexOf("Linux") != -1) return "Linux";

    return null;
}

StiJsViewer.prototype.checkWin11 = function () {
    return false;
}

StiJsViewer.prototype.SetZoom = function (zoomIn) {
    var zoomValues = ["25", "50", "75", "100", "150", "200"];

    for (var i = 0; i < zoomValues.length; i++)
        if (zoomValues[i] == this.reportParams.zoom) break;

    if (zoomIn && i < zoomValues.length - 1) this.postAction("Zoom" + zoomValues[i + 1]);
    if (!zoomIn && i > 0) this.postAction("Zoom" + zoomValues[i - 1]);
}

StiJsViewer.prototype.getCssParameter = function (css) {
    if (css.indexOf(".gif]") > 0 || css.indexOf(".png]") > 0) return css.substr(css.indexOf("["), css.indexOf("]") - css.indexOf("[") + 1);
    return null;
}

StiJsViewer.prototype.newGuid = (function () {
    var CHARS = '0123456789abcdefghijklmnopqrstuvwxyz'.split('');
    return function (len, radix) {
        var chars = CHARS, uuid = [], rnd = Math.random;
        radix = radix || chars.length;

        if (len) {
            for (var i = 0; i < len; i++) uuid[i] = chars[0 | rnd() * radix];
        } else {
            var r;
            uuid[8] = uuid[13] = uuid[18] = uuid[23] = '-';
            uuid[14] = '4';

            for (var i = 0; i < 36; i++) {
                if (!uuid[i]) {
                    r = 0 | rnd() * 16;
                    uuid[i] = chars[(i == 19) ? (r & 0x3) | 0x8 : r & 0xf];
                }
            }
        }

        return uuid.join('');
    };
})();

StiJsViewer.prototype.generateKey = function () {
    return this.newGuid().replace(/-/g, '');
}

StiJsViewer.prototype.Item = function (name, caption, imageName, key, haveSubMenu, imageSizes) {
    var item = {
        name: name,
        caption: caption,
        imageName: imageName,
        key: key,
        haveSubMenu: haveSubMenu,
        imageSizes: imageSizes
    }

    return item;
}

StiJsViewer.prototype.StrToInt = function (value) {
    var result = parseInt(value);
    if (result) return result;
    return 0;
}

StiJsViewer.prototype.StrToCorrectByte = function (value) {
    var result = parseInt(value);
    if (result) {
        if (result > 255) return 255;
        if (result < 0) return 0;
        return result;
    }
    else
        return 0;
}

StiJsViewer.prototype.StrToDouble = function (value) {
    if (value == null)
        return null;

    var result = parseFloat(value.toString().replace(",", ".").replace(" ", ""));
    if (result)
        return result;
    else
        return 0;
}

StiJsViewer.prototype.formatDate = function (formatDate, formatString, typeDateTime) {
    var yyyy = formatDate.getFullYear();
    var yy = yyyy.toString().substring(2);
    var m = formatDate.getMonth() + 1;
    var mm = m < 10 ? "0" + m : m;
    var d = formatDate.getDate();
    var dd = d < 10 ? "0" + d : d;

    var h = formatDate.getHours();
    var hh = h < 10 ? "0" + h : h;
    var h12 = h > 12 ? h - 12 : (h > 0 ? h : 12);
    var hh12 = h12 < 10 ? "0" + h12 : h12;
    var n = formatDate.getMinutes();
    var nn = n < 10 ? "0" + n : n;
    var s = formatDate.getSeconds();
    var ss = s < 10 ? "0" + s : s;
    var tt = h < 12 ? "AM" : "PM";

    var removeSubstring = function (str, start, end) {
        return str.substring(0, start) + str.substring(end);
    }

    if (typeDateTime == "Time") {
        var minPos = -1;
        var maxPos = -1;
        var keys = ["y", "Y", "M", "d"];

        for (var i = 0; i < keys.length; i++) {
            var keyIndex = formatString.indexOf(keys[i]);
            if (keyIndex >= 0) {
                minPos = minPos < 0 ? keyIndex : Math.min(minPos, keyIndex);
                maxPos = Math.max(maxPos, formatString.lastIndexOf(keys[i]) + 1);
            }
        }
        if (minPos >= 0 && maxPos > 0) {
            formatString = removeSubstring(formatString, minPos, maxPos);
        }
    }
    else {
        formatString = formatString.replace(/yyyy/gi, yyyy);
        formatString = formatString.replace(/yy/gi, yy);
        formatString = formatString.replace(/Y/, yyyy);
        formatString = formatString.replace(/MM/g, mm);
        formatString = formatString.replace(/M/g, m);
        formatString = formatString.replace(/dd/g, dd);
        formatString = formatString.replace(/d/g, d);
    }

    if (typeDateTime == "Date") {
        var minPos = maxPos = -1;
        var keys = ["h", "H", "m", "s", "t"];

        for (var i = 0; i < keys.length; i++) {
            var keyIndex = formatString.indexOf(keys[i]);
            if (keyIndex >= 0) {
                minPos = minPos < 0 ? keyIndex : Math.min(minPos, keyIndex);
                maxPos = Math.max(maxPos, formatString.lastIndexOf(keys[i]) + 1);
            }
        }
        if (maxPos > 0) {
            formatString = removeSubstring(formatString, minPos, maxPos);
        }
    }
    else {
        formatString = formatString.replace(/hh/g, hh12);
        formatString = formatString.replace(/h/g, h12);
        formatString = formatString.replace(/HH/g, hh);
        formatString = formatString.replace(/H/g, h);
        formatString = formatString.replace(/mm/g, nn);
        formatString = formatString.replace(/m/g, n);
        formatString = formatString.replace(/ss/g, ss);
        formatString = formatString.replace(/s/g, s);
        formatString = formatString.replace(/tt/gi, tt);
        formatString = formatString.replace(/t/gi, tt.substr(0, 1));
    }


    if (typeDateTime && formatString) {
        var charIsDigit = function (char) {
            return ("0123456789".indexOf(char) >= 0);
        }
        var startIndex = -1;
        var endIndex = formatString.length - 1;

        for (var i = 0; i < formatString.length; i++) {
            if (charIsDigit(formatString[i])) {
                if (startIndex < 0) startIndex = i;
                endIndex = i;
            }
        }

        if (endIndex < formatString.length - 1) {
            var amIndex = formatString.toLowerCase().indexOf("am");
            var pmIndex = formatString.toLowerCase().indexOf("pm");
            if (amIndex > 0 || pmIndex > 0) endIndex = Math.max(amIndex, pmIndex) + 1;
        }

        formatString = formatString.substring(startIndex, endIndex + 1);
    }

    return formatString;
}

StiJsViewer.prototype.stringToTime = function (timeStr) {
    var timeArray = timeStr.split(":");
    var time = { hours: 0, minutes: 0, seconds: 0 };

    time.hours = this.StrToInt(timeArray[0]);
    if (timeArray.length > 1) time.minutes = this.StrToInt(timeArray[1]);
    if (timeArray.length > 2) time.seconds = this.StrToInt(timeArray[2]);

    if (time.hours < 0) time.hours = 0;
    if (time.minutes < 0) time.minutes = 0;
    if (time.seconds < 0) time.seconds = 0;

    if (time.hours > 23) time.hours = 23;
    if (time.minutes > 59) time.minutes = 59;
    if (time.seconds > 59) time.seconds = 59;

    return time;
}

StiJsViewer.prototype.dateTimeObjectToString = function (dateTimeObject, typeDateTimeObject) {
    var date = new Date(dateTimeObject.year, dateTimeObject.month - 1, dateTimeObject.day, dateTimeObject.hours, dateTimeObject.minutes, dateTimeObject.seconds);

    if (this.options.appearance.parametersPanelDateFormat != "") {
        var formattedDate = this.formatDate(date, this.options.appearance.parametersPanelDateFormat, typeDateTimeObject);
        if (formattedDate != "") return formattedDate;
    }

    return this.DateToLocaleString(date, typeDateTimeObject);
}

StiJsViewer.prototype.getStringKey = function (key, parameter) {
    if (key == null) return "";

    return (parameter.params.type == "DateTime" || parameter.params.type == "DateTimeOffset" ? this.dateTimeObjectToString(key, parameter.params.dateTimeType) : key);
}

StiJsViewer.prototype.getCountObjects = function (objectArray) {
    var count = 0;
    if (objectArray)
        for (var singleObject in objectArray) { count++ };
    return count;
}

StiJsViewer.prototype.getDateTimeObject = function (date) {
    if (!date) date = new Date();
    var dateTimeObject = {};
    dateTimeObject.year = date.getFullYear();
    dateTimeObject.month = date.getMonth() + 1;
    dateTimeObject.day = date.getDate();
    dateTimeObject.hours = date.getHours();
    dateTimeObject.minutes = date.getMinutes();
    dateTimeObject.seconds = date.getSeconds();

    return dateTimeObject;
}

StiJsViewer.prototype.getNowTimeSpanObject = function () {
    var date = new Date();
    var timeSpanObject = {};
    timeSpanObject.hours = date.getHours();
    timeSpanObject.minutes = date.getMinutes();
    timeSpanObject.seconds = date.getSeconds();

    return timeSpanObject;
}

StiJsViewer.prototype.copyObject = function (o) {
    if (!o || "object" !== typeof o) {
        return o;
    }
    var c = "function" === typeof o.pop ? [] : {};
    var p, v;
    for (p in o) {
        // eslint-disable-next-line no-prototype-builtins
        if (o.hasOwnProperty(p)) {
            v = o[p];
            if (v && "object" === typeof v) {
                c[p] = this.copyObject(v);
            }
            else c[p] = v;
        }
    }
    return c;
}

StiJsViewer.prototype.getNavigatorName = function () {
    if (!navigator) return "Unknown";
    var userAgent = navigator.userAgent;

    if (this.IsTouchDevice()) {
        if (/iPad|Macintosh/i.test(userAgent)) return 'iPad';
    }

    if (userAgent.indexOf('Edge') >= 0) return 'Edge';
    if (userAgent.indexOf('MSIE') >= 0 || userAgent.indexOf('Trident') >= 0) return 'MSIE';
    if (userAgent.indexOf('Gecko') >= 0) {
        if (userAgent.indexOf('Chrome') >= 0) return 'Chrome';
        if (userAgent.indexOf('Safari') >= 0) return 'Safari';
        return 'Mozilla';
    }
    if (userAgent.indexOf('Opera') >= 0) return 'Opera';
    return 'Unknown';
}

StiJsViewer.prototype.showHelpWindow = function (url) {
    var helpLanguage;
    switch (this.options.cultureName) {
        case "ru": helpLanguage = "ru"; break;
        //case "de": helpLanguage = "de";
        default: helpLanguage = "en";
    }
    this.openNewWindow("https://www.stimulsoft.com/" + helpLanguage + "/documentation/online/" + url, undefined, undefined, false);
}

StiJsViewer.prototype.setObjectToCenter = function (object, defaultTop) {
    var leftPos = (this.controls.viewer.offsetWidth / 2 - object.offsetWidth / 2);
    var topPos = this.options.appearance.fullScreenMode ? (this.controls.viewer.offsetHeight / 2 - object.offsetHeight / 2) : (defaultTop ? defaultTop : 250);
    object.style.left = leftPos > 0 ? leftPos + "px" : 0;
    object.style.top = topPos > 0 ? topPos + "px" : 0;
}

StiJsViewer.prototype.strToInt = function (value) {
    var result = parseInt(value);
    if (result) return result;
    return 0;
}

StiJsViewer.prototype.strToCorrectPositiveInt = function (value) {
    var result = this.strToInt(value);
    if (result >= 0) return result;
    return 0;
}

StiJsViewer.prototype.getHTMLColor = function (color) {
    if (color.indexOf(",") > 0 && color.indexOf("rgb") < 0) return "rgb(" + color + ")";
    return color;
}
StiJsViewer.prototype.excludeOpacity = function (color) {
    if (color.indexOf("rgba") >= 0) {
        var rgbStr = color.replace("rgba(", "").replace(")", "");
        var rgb = rgbStr.split(",");
        if (rgb.length >= 3) {
            return "rgb(" + rgb[0] + ", " + rgb[1] + ", " + rgb[2] + ")";
        }
    }
    return color;
}

StiJsViewer.prototype.clearStyles = function (object) {
    object.className = "stiJsViewerClearAllStyles";
}

StiJsViewer.prototype.getDefaultExportSettings = function (exportFormat, isDashboardExport) {
    var exportSettings = null;

    if (isDashboardExport) {
        return this.options.exports.defaultSettings["Dashboard" + exportFormat];
    }

    switch (exportFormat) {
        case "Document":
            exportSettings = {};
            break;

        case "Pdf":
            exportSettings = this.options.exports.defaultSettings["StiPdfExportSettings"];
            break;

        case "Xps":
            exportSettings = this.options.exports.defaultSettings["StiXpsExportSettings"];
            break;

        case "Ppt2007":
            exportSettings = this.options.exports.defaultSettings["StiPpt2007ExportSettings"];
            break;

        case "Html":
            exportSettings = this.options.exports.defaultSettings["StiHtmlExportSettings"];
            exportSettings.HtmlType = "Html";
            break;

        case "Html5":
            exportSettings = this.options.exports.defaultSettings["StiHtmlExportSettings"];
            exportSettings.HtmlType = "Html5";
            break;

        case "Mht":
            exportSettings = this.options.exports.defaultSettings["StiHtmlExportSettings"];
            exportSettings.HtmlType = "Mht";
            break;

        case "Text":
            exportSettings = this.options.exports.defaultSettings["StiTxtExportSettings"];
            break;

        case "Rtf":
            exportSettings = this.options.exports.defaultSettings["StiRtfExportSettings"];
            break;

        case "Word2007":
            exportSettings = this.options.exports.defaultSettings["StiWord2007ExportSettings"];
            break;

        case "Odt":
            exportSettings = this.options.exports.defaultSettings["StiOdtExportSettings"];
            break;

        case "Excel":
            exportSettings = this.options.exports.defaultSettings["StiExcelExportSettings"];
            exportSettings.ExcelType = "ExcelBinary";
            break;

        case "ExcelXml":
            exportSettings = this.options.exports.defaultSettings["StiExcelExportSettings"];
            exportSettings.ExcelType = "ExcelXml";
            break;

        case "Excel2007":
            exportSettings = this.options.exports.defaultSettings["StiExcelExportSettings"];
            exportSettings.ExcelType = "Excel2007";
            break;

        case "Ods":
            exportSettings = this.options.exports.defaultSettings["StiOdsExportSettings"];
            break;

        case "ImageBmp":
            exportSettings = this.options.exports.defaultSettings["StiImageExportSettings"];
            exportSettings.ImageType = "Bmp";
            break;

        case "ImageGif":
            exportSettings = this.options.exports.defaultSettings["StiImageExportSettings"];
            exportSettings.ImageType = "Gif";
            break;

        case "ImageJpeg":
            exportSettings = this.options.exports.defaultSettings["StiImageExportSettings"];
            exportSettings.ImageType = "Jpeg";
            break;

        case "ImagePcx":
            exportSettings = this.options.exports.defaultSettings["StiImageExportSettings"];
            exportSettings.ImageType = "Pcx";
            break;

        case "ImagePng":
            exportSettings = this.options.exports.defaultSettings["StiImageExportSettings"];
            exportSettings.ImageType = "Png";
            break;

        case "ImageTiff":
            exportSettings = this.options.exports.defaultSettings["StiImageExportSettings"];
            exportSettings.ImageType = "Tiff";
            break;

        case "ImageSvg":
            exportSettings = this.options.exports.defaultSettings["StiImageExportSettings"];
            exportSettings.ImageType = "Svg";
            break;

        case "ImageSvgz":
            exportSettings = this.options.exports.defaultSettings["StiImageExportSettings"];
            exportSettings.ImageType = "Svgz";
            break;

        case "ImageEmf":
            exportSettings = this.options.exports.defaultSettings["StiImageExportSettings"];
            exportSettings.ImageType = "Emf";
            break;

        case "Xml":
            exportSettings = this.options.exports.defaultSettings["StiDataExportSettings"];
            exportSettings.DataType = "Xml";
            break;

        case "Csv":
            exportSettings = this.options.exports.defaultSettings["StiDataExportSettings"];
            exportSettings.DataType = "Csv";
            break;

        case "Dbf":
            exportSettings = this.options.exports.defaultSettings["StiDataExportSettings"];
            exportSettings.DataType = "Dbf";
            break;

        case "Dif":
            exportSettings = this.options.exports.defaultSettings["StiDataExportSettings"];
            exportSettings.DataType = "Dif";
            break;

        case "Sylk":
            exportSettings = this.options.exports.defaultSettings["StiDataExportSettings"];
            exportSettings.DataType = "Sylk";
            break;
    }

    return exportSettings;
}

StiJsViewer.prototype.addEvent = function (element, eventName, fn, mainElement) {
    if (!mainElement) mainElement = element;
    if (!this.viewerEvents) this.viewerEvents = [];
    if (element) {
        if (element.addEventListener) element.addEventListener(eventName, fn, false);
        else if (element.attachEvent) element.attachEvent("on" + eventName, fn);
        else element["on" + eventName] = fn;

        this.viewerEvents.push({
            element: element,
            eventName: eventName,
            fn: fn,
            mainElement: mainElement
        });
    }
}

StiJsViewer.prototype.removeAllEvents = function () {
    if (this.viewerEvents) {
        for (var i = 0; i < this.viewerEvents.length; i++) {
            var evnt = this.viewerEvents[i];
            var element = evnt.element;
            var eventName = evnt.eventName;
            var fn = evnt.fn;
            if (element.removeEventListener) element.removeEventListener(eventName, fn, false);
            else if (element.detachEvent) element.detachEvent('on' + eventName, fn);
            else element["on" + eventName] = null;
        }
        this.viewerEvents = [];
    }
}

StiJsViewer.prototype.lowerFirstChar = function (text) {
    return text.charAt(0).toLowerCase() + text.substr(1);
}

StiJsViewer.prototype.upperFirstChar = function (text) {
    return text.charAt(0).toUpperCase() + text.substr(1);
}

StiJsViewer.prototype.addHoverEventsToMenus = function () {
    if (this.options.toolbar.showMenuMode == "Hover") {
        var buttonsWithMenu = ["Print", "Save", "SendEmail", "Zoom", "ViewMode"];
        for (var i = 0; i < buttonsWithMenu.length; i++) {
            var button = this.controls.toolbar.controls[buttonsWithMenu[i]];
            if (button) {
                var menu = this.controls.menus[this.lowerFirstChar(button.name) + "Menu"];
                if (menu) {
                    menu.buttonName = button.name;

                    menu.onmouseover = function () {
                        clearTimeout(this.jsObject.options.toolbar["hideTimer" + this.buttonName + "Menu"]);
                    }

                    menu.onmouseout = function () {
                        var thisMenu = this;
                        this.jsObject.options.toolbar["hideTimer" + this.buttonName + "Menu"] = setTimeout(function () {
                            thisMenu.changeVisibleState(false);
                        }, this.jsObject.options.menuHideDelay);
                    }
                }
            }
        }
    }
}

StiJsViewer.prototype.GetXmlValue = function (xml, key) {
    var string = xml.substr(0, xml.indexOf("</" + key + ">"));
    return string.substr(xml.indexOf("<" + key + ">") + key.length + 2);
}

StiJsViewer.prototype.DateToLocaleString = function (date, dateTimeType) {
    var timeString = date.toLocaleTimeString();
    var isAmericanFormat = timeString.toLowerCase().indexOf("am") >= 0 || timeString.toLowerCase().indexOf("pm") >= 0;
    var formatDate = isAmericanFormat ? "MM/dd/yyyy" : "dd.MM.yyyy";

    var yyyy = date.getFullYear();
    var yy = yyyy.toString().substring(2);
    var M = date.getMonth() + 1;
    var MM = M < 10 ? "0" + M : M;
    var d = date.getDate();
    var dd = d < 10 ? "0" + d : d;

    formatDate = formatDate.replace(/yyyy/i, yyyy);
    formatDate = formatDate.replace(/yy/i, yy);
    formatDate = formatDate.replace(/MM/i, MM);
    formatDate = formatDate.replace(/M/i, M);
    formatDate = formatDate.replace(/dd/i, dd);
    formatDate = formatDate.replace(/d/i, d);

    var h = date.getHours();
    var tt = "";

    if (isAmericanFormat) {
        tt = h < 12 ? " AM" : " PM";
        h = h > 12 ? h - 12 : h;
        if (h == 0) h = 12;
    }
    else
        h < 10 ? "0" + h : h;

    var m = date.getMinutes();
    m = m < 10 ? "0" + m : m;
    var s = date.getSeconds();
    s = s < 10 ? "0" + s : s;

    var formatTime = h + ":" + m + ":" + s + tt;

    if (dateTimeType == "Time") return formatTime;
    if (dateTimeType == "Date") return formatDate;

    return formatDate + " " + formatTime;
}

StiJsViewer.prototype.UpdateAllHyperLinks = function () {
    var aHyperlinks = this.controls.reportPanel.getElementsByTagName("a");
    var pointers = this.reportParams.tableOfContentsPointers;
    var bookmarksPanel = this.controls.bookmarksPanel;
    var jsObject = this;

    if (bookmarksPanel || (pointers && pointers.length > 0)) {
        for (var i = 0; i < aHyperlinks.length; i++) {
            aHyperlinks[i].hrefContent = aHyperlinks[i].getAttribute("href");

            if (aHyperlinks[i].hrefContent) {
                if (aHyperlinks[i].hrefContent.indexOf("#") == 0) {
                    var anchorParams = aHyperlinks[i].hrefContent.substring(1).split("#GUID#");
                    aHyperlinks[i].anchorName = anchorParams[0];
                    aHyperlinks[i].componentGuid = anchorParams.length > 1 ? anchorParams[1] : "";

                    aHyperlinks[i].onclick = function () {
                        var currAnchorName = this.anchorName;
                        var cuurCompGuid = this.componentGuid;

                        try {
                            currAnchorName = decodeURI(this.anchorName)
                        }
                        catch (e) {
                            currAnchorName = this.anchorName;
                        }

                        if (pointers.length > 0) {
                            var pageIndex = 1;

                            for (var i = 0; i < pointers.length; i++) {
                                if (cuurCompGuid) {
                                    if (pointers[i].componentGuid == cuurCompGuid) {
                                        pageIndex = pointers[i].pageIndex;
                                        break;
                                    }
                                }
                                else if (currAnchorName) {
                                    var pointerAnchor = pointers[i].anchor.indexOf("#") == 0 ? pointers[i].anchor.substring(1) : pointers[i].anchor;
                                    if (pointerAnchor == currAnchorName) {
                                        pageIndex = pointers[i].pageIndex;
                                        break;
                                    }
                                }
                            }

                            var anchorName = "";
                            if (currAnchorName) anchorName += currAnchorName;
                            if (cuurCompGuid) anchorName += ("#GUID#" + cuurCompGuid);
                            if (anchorName) {
                                jsObject.postAction("BookmarkAction", Math.max(pageIndex - 1, 0), anchorName);
                                return false;
                            }
                        }

                        if (bookmarksPanel) {
                            var aBookmarks = bookmarksPanel.getElementsByTagName("a");
                            for (var k = 0; k < aBookmarks.length; k++) {
                                var clickFunc = aBookmarks[k].getAttribute("onclick");
                                var escapeCurrAnchorName = currAnchorName.replace(/'/g, "\\\'");
                                if (clickFunc && clickFunc.indexOf("'" + escapeCurrAnchorName + "'") >= 0) {
                                    try {
                                        eval(clickFunc);
                                        return false;
                                    }
                                    catch (e) { }
                                }
                            }
                            for (var k = 0; k < document.anchors.length; k++) {
                                if (document.anchors[k].name == currAnchorName) {
                                    jsObject.scrollToAnchor(currAnchorName);
                                    return;
                                }
                            }
                            jsObject.postAction("BookmarkAction", 0);
                            return false;
                        }
                    };
                }
            }
        }
    }
}

StiJsViewer.prototype.helpLinks = {
    "Print": "user-manual/index.html?viewer_reports.htm#toolbar",
    "Save": "user-manual/index.html?viewer_reports.htm#toolbar",
    "SendEmail": "user-manual/index.html?viewer_reports.htm#toolbar",
    "Bookmarks": "user-manual/index.html?viewer_reports.htm#toolbar",
    "Parameters": "user-manual/index.html?viewer_reports.htm#toolbar",
    "FirstPage": "user-manual/index.html?viewer_reports.htm#statusbar",
    "PrevPage": "user-manual/index.html?viewer_reports.htm#statusbar",
    "NextPage": "user-manual/index.html?viewer_reports.htm#statusbar",
    "LastPage": "user-manual/index.html?viewer_reports.htm#statusbar",
    "FullScreen": "user-manual/index.html?viewer_reports.htm#toolbar",
    "Zoom": "user-manual/index.html?viewer_reports.htm#statusbar",
    "ViewMode": "user-manual/index.html?viewer_reports.htm#displayingmode",
    "Editor": "user-manual/index.html?viewer_reports.htm#toolbar",
    "Signature": "user-manual/index.html?viewer_reports.htm#toolbar",
    "Find": "user-manual/index.html?viewer_reports.htm#searchpanel",
    "DashboardToolbar": "user-manual/index.html?viewer_dashboards.htm#controlbuttonsofthedashboard",
    "DashboardElementToolbar": "user-manual/index.html?viewer_dashboards.htm#elementcontrols",
    "DashboardExport": "user-manual/index.html?exports_dashboards.htm",
    "DashboardPdfExport": "user-manual/index.html?exports_dashboards.htm#pdfexportsettings",
    "DashboardExcelExport": "user-manual/index.html?exports_dashboards.htm#excelexportsettings",
    "DashboardImageExport": "user-manual/index.html?exports_dashboards.htm#imageexportsettings",
    "DashboardDataExport": "user-manual/index.html?exports_dashboards.htm#exportsettingsofdata",
    "DashboardHtmlExport": "user-manual/index.html?exports_dashboards.htm#exportsettingsofhtml"
}

// inApp - parameter used in Stimulsoft.Reports.JS (in electron). Anywhere else it is ignored
StiJsViewer.prototype.openNewWindow = function (url, name, specs, inApp) {
    var win = window.open(url, name, specs);
    return win;
}

StiJsViewer.prototype.SetCookie = function (name, value, path, domain, secure, expires) {
    if (this.options.standaloneJsMode || typeof localStorage == "undefined" || name.indexOf("sti_") == 0 || name.indexOf("login") == 0) {
        //save to cookies
        if (value && typeof (value) == "string" && value.length >= 4096) return;
        var pathName = location.pathname;
        var expDate = new Date();
        expDate.setTime(expDate.getTime() + (365 * 24 * 3600 * 1000));
        document.cookie = name + "=" + escape(value) +
            "; samesite=strict; expires=" + (expires || expDate.toGMTString()) +
            ((path) ? "; path=" + path : "; path=/") +
            ((domain) ? "; domain=" + domain : "") +
            ((secure) ? "; secure" : "");
    }
    else {
        //save to localstorage
        localStorage.setItem(name, value);
    }
}

StiJsViewer.prototype.GetCookie = function (name) {
    var getCookie_ = function (name) {
        var cookie = " " + document.cookie;
        var search = " " + name + "=";
        var setStr = null;
        var offset = 0;
        var end = 0;
        if (cookie.length > 0) {
            offset = cookie.indexOf(search);
            if (offset != -1) {
                offset += search.length;
                end = cookie.indexOf(";", offset);
                if (end == -1) {
                    end = cookie.length;
                }
                setStr = unescape(cookie.substring(offset, end));
            }
        }
        return setStr;
    }

    if (this.options.standaloneJsMode || typeof localStorage == "undefined" || name.indexOf("sti_") == 0 || name.indexOf("login") == 0) {
        return getCookie_(name);
    }

    var value = localStorage.getItem(name);
    if (value != null) {
        return value;
    }
    else {
        value = getCookie_(name);
        if (value != null) {
            this.RemoveCookie(name);
            localStorage.setItem(name, value);
        }
        return value;
    }
}

StiJsViewer.prototype.RemoveCookie = function (name) {
    document.cookie = name + "=;expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/";
}

StiJsViewer.prototype.numberWithSpaces = function (x) {
    if (x == null) return "";
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, " ");
}

StiJsViewer.prototype.GetHumanFileSize = function (value, decimals) {
    var i = Math.floor(Math.log(value) / Math.log(1024));
    return ((value / Math.pow(1024, i)).toFixed(decimals) * 1) + ' ' + ['B', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'][i];
}

StiJsViewer.prototype.addCustomFontStyles = function (customFonts) {
    if (!customFonts) return;

    var existsStyles = this.controls.head ? this.controls.head.getElementsByTagName("style") : [];

    for (var i = 0; i < customFonts.length; i++) {
        if (this.controls.head && customFonts[i].contentForCss && customFonts[i].originalFontFamily) {
            var style = document.createElement("style");
            var cssText = "@font-face {\r\n" +
                "font-family: '" + customFonts[i].originalFontFamily + "';\r\n" +
                "src: url(" + customFonts[i].contentForCss + ");\r\n }";

            style.innerHTML = cssText;

            var existsThisStyle = false;

            for (var k = 0; k < existsStyles.length; k++) {
                if (existsStyles[k].innerHTML.indexOf("font-family: '" + customFonts[i].originalFontFamily + "'") > 0) {
                    existsThisStyle = true;
                    break;
                }
            }

            if (!existsThisStyle) {
                style.setAttribute("stimulsoft", "stimulsoft");
                this.controls.head.appendChild(style);
            }
        }
    }
}

StiJsViewer.prototype.changeFullScreenMode = function (fullScreenMode) {
    this.options.appearance.fullScreenMode = fullScreenMode;
    if (this.options.toolbar.visible && this.options.toolbar.showFullScreenButton) this.controls.toolbar.controls.FullScreen.setSelected(fullScreenMode);
    if (this.options.dashboardAssemblyLoaded && this.controls.buttons.FullScreenDashboard) this.controls.buttons.FullScreenDashboard.setFullScreenState(fullScreenMode);

    if (fullScreenMode) {
        this.controls.viewer.fullScreenOptions = {
            scrollbarsMode: this.options.appearance.scrollbarsMode,
            zIndex: this.controls.viewer.style.zIndex,
            position: this.controls.viewer.style.position,
            width: this.controls.viewer.style.width,
            height: this.controls.viewer.style.height,
            overflow: document.body.style.overflow
        };

        this.options.appearance.scrollbarsMode = true;
        this.controls.viewer.style.zIndex = "1000000";
        this.controls.viewer.style.position = this.options.reportDesignerMode ? "absolute" : "fixed";
        this.controls.viewer.style.width = null;
        this.controls.viewer.style.height = null;
        if (!this.options.reportDesignerMode) {
            document.body.style.overflow = "hidden";
        }
    }
    else if (this.controls.viewer.fullScreenOptions) {
        this.options.appearance.scrollbarsMode = this.controls.viewer.fullScreenOptions.scrollbarsMode;
        this.controls.viewer.style.zIndex = this.controls.viewer.fullScreenOptions.zIndex;
        this.controls.viewer.style.position = this.controls.viewer.fullScreenOptions.position;
        this.controls.viewer.style.width = this.controls.viewer.fullScreenOptions.width;
        this.controls.viewer.style.height = this.controls.viewer.fullScreenOptions.height;
        document.body.style.overflow = this.controls.viewer.fullScreenOptions.overflow;

        delete this.controls.viewer.fullScreenOptions;
    }

    this.updateLayout();
}

StiJsViewer.prototype.updateVisibleState = function () {
    if (this.reportParams.type == "Dashboard") {
        this.controls.dashboardsPanel.actionsTable.style.display = "inline-block";
        this.controls.toolbar.changeVisibleState(false);
        this.controls.drillDownPanel.changeVisibleState(false);
        if (this.controls.findPanel) this.controls.findPanel.changeVisibleState(false);
        if (this.controls.resourcesPanel) this.controls.resourcesPanel.changeVisibleState(false);
        if (this.controls.navigatePanel) this.controls.navigatePanel.changeVisibleState(false);
        if (this.controls.bookmarksPanel) this.controls.bookmarksPanel.style.display = "none";
    }
    else {
        this.controls.dashboardsPanel.actionsTable.style.display = "none";
        this.controls.toolbar.changeVisibleState(true);
        this.controls.drillDownPanel.changeVisibleState(this.controls.drillDownPanel.buttonsRow.children.length > 1);
        if (this.controls.findPanel) this.controls.findPanel.changeVisibleState(this.controls.toolbar.controls.Find.isSelected);
        if (this.controls.resourcesPanel) this.controls.resourcesPanel.changeVisibleState(this.controls.buttons["Resources"].isSelected);
        if (this.controls.navigatePanel) this.controls.navigatePanel.changeVisibleState(true);
        if (!this.controls.bookmarksPanel) this.InitializeBookmarksPanel();
        else if (this.controls.bookmarksPanel.visible) this.controls.bookmarksPanel.style.display = "";
    }
    if (!this.controls.parametersPanel) this.InitializeParametersPanel();
    else if (this.controls.parametersPanel.visible) this.controls.parametersPanel.style.display = "";
}

StiJsViewer.prototype.calculateLayout = function () {
    var reportLayout = { top: 0, right: 0, bottom: 0, left: 0, width: 0, height: 0 };
    var paramsLayout = { top: 0, left: 0, width: 0, height: 0 };

    if (this.controls.dashboardsPanel) reportLayout.top += this.controls.dashboardsPanel.offsetHeight;

    if (this.reportParams.type == "Report") {
        if (this.controls.toolbar && this.controls.toolbar.visible && !(this.options.isMobileDevice && this.options.toolbar.autoHide))
            reportLayout.top += this.controls.toolbar.offsetHeight;

        if (this.controls.drillDownPanel && this.controls.drillDownPanel.visible) reportLayout.top += this.controls.drillDownPanel.offsetHeight;
        if (this.controls.findPanel && this.controls.findPanel.visible) reportLayout.top += this.controls.findPanel.offsetHeight;
        if (this.controls.resourcesPanel && this.controls.resourcesPanel.visible) reportLayout.top += this.controls.resourcesPanel.offsetHeight;

        if (this.controls.bookmarksPanel && this.controls.bookmarksPanel.visible) {
            reportLayout.left += this.options.appearance.bookmarksTreeWidth;
            if (this.options.toolbar.displayMode == "Simple") reportLayout.left += 2;
        }

        if (this.controls.navigatePanel && this.controls.navigatePanel.visible && !(this.options.isMobileDevice && this.options.toolbar.autoHide))
            reportLayout.bottom = this.controls.navigatePanel.offsetHeight;
    }

    if (this.controls.parametersPanel && this.controls.parametersPanel.visible) {
        this.controls.parametersPanel.layout = paramsLayout;
        paramsLayout.top = reportLayout.top;

        var parametersPanelPosition = this.options.currentParametersPanelPosition || this.options.appearance.parametersPanelPosition;

        if (parametersPanelPosition == "Left") {
            paramsLayout.left = reportLayout.left;
            paramsLayout.width = this.controls.parametersPanel.firstChild.offsetWidth;
            reportLayout.left += paramsLayout.width;
            if (this.options.toolbar.displayMode == "Simple") reportLayout.left += 2;
        }

        if (parametersPanelPosition == "Top") {
            paramsLayout.height = this.controls.parametersPanel.offsetHeight;
            reportLayout.top += paramsLayout.height;
        }
    }

    if (this.controls.bookmarksPanel) this.controls.bookmarksPanel.layout = { top: reportLayout.top };

    if (this.options.toolbar.displayMode == "Simple" && reportLayout.top > 0) reportLayout.top += 2;

    if (this.controls.reportPanel.style.position == "relative") reportLayout.top = paramsLayout.height;

    var reportMargins = {
        top: parseInt(this.controls.reportPanel.style.marginTop ? this.controls.reportPanel.style.marginTop : 0),
        right: parseInt(this.controls.reportPanel.style.marginRight ? this.controls.reportPanel.style.marginRight : 0),
        bottom: parseInt(this.controls.reportPanel.style.marginBottom ? this.controls.reportPanel.style.marginBottom : 0),
        left: parseInt(this.controls.reportPanel.style.marginLeft ? this.controls.reportPanel.style.marginLeft : 0)
    };

    reportLayout.width = this.controls.reportPanel.offsetWidth - reportLayout.left - reportLayout.right + reportMargins.left + reportMargins.right;

    var fullHeight = this.controls.reportPanel.style.position == "absolute" || (this.reportParams.type == "Dashboard" && this.controls.reportPanel.offsetHeight > 100 && !this.options.isAutoHeight && !this.options.isFullScreenHeight);

    if (fullHeight) {
        reportLayout.height = this.controls.reportPanel.offsetHeight - reportLayout.top - reportLayout.bottom + reportMargins.top + reportMargins.bottom
        if (this.controls.reportPanel.style.position == "absolute") this.options.isFullScreenHeight = true;
    }
    else {
        reportLayout.height = parseInt(reportLayout.width * 0.56); // use 16:9 aspect ratio for automatic height
        this.options.isAutoHeight = true;
    }

    this.controls.reportPanel.layout = reportLayout;
}

StiJsViewer.prototype.updateLayout = function () {
    this.controls.reportPanel.style.position = this.options.heightType != "Percentage" || this.options.appearance.scrollbarsMode ? "absolute" : "relative";
    this.controls.reportPanel.style.height = this.options.heightType != "Percentage" || this.options.appearance.scrollbarsMode ? "auto" : "calc(100% - 35px)";

    var showDasboardScroll = this.reportParams.type == "Dashboard" && this.reportParams.pagesArray && this.reportParams.pagesArray.length > 0 && this.reportParams.pagesArray[0].contentAlignment != "StretchXY";
    this.controls.reportPanel.style.overflow = (this.reportParams.type == "Report" && this.options.appearance.scrollbarsMode) || showDasboardScroll ? "auto" : "hidden";

    this.calculateLayout();

    if (this.controls.parametersPanel && this.controls.parametersPanel.visible) {
        this.controls.parametersPanel.style.top = this.controls.parametersPanel.layout.top + "px";
        this.controls.parametersPanel.style.left = this.controls.parametersPanel.layout.left + "px";
    }

    if (this.controls.bookmarksPanel && this.controls.bookmarksPanel.visible) {
        this.controls.bookmarksPanel.style.top = this.controls.bookmarksPanel.layout.top + "px";
    }

    this.controls.reportPanel.style.marginTop = this.controls.reportPanel.layout.top + "px";
    this.controls.reportPanel.style.marginLeft = this.controls.reportPanel.layout.left + "px";
    this.controls.reportPanel.style.marginBottom = this.controls.reportPanel.style.position == "absolute" ? this.controls.reportPanel.layout.bottom + "px" : 0;
    this.controls.reportPanel.style.paddingBottom = this.controls.reportPanel.style.position == "relative" ? this.controls.reportPanel.layout.bottom + "px" : 0;
}

StiJsViewer.prototype.isFilterElement = function (typeElement) {
    return (typeElement == "StiListBoxElement" ||
        typeElement == "StiDatePickerElement" ||
        typeElement == "StiComboBoxElement" ||
        typeElement == "StiTreeViewElement" ||
        typeElement == "StiTreeViewBoxElement");
}

StiJsViewer.prototype.HexToRgb = function (hex) {
    var result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    return result ? {
        r: parseInt(result[1], 16),
        g: parseInt(result[2], 16),
        b: parseInt(result[3], 16)
    } : null;
}

StiJsViewer.prototype.RgbToHex = function (rgb) {
    var hex = Number(rgb).toString(16);
    if (hex.length < 2) {
        hex = "0" + hex;
    }
    return hex;
};

StiJsViewer.prototype.DecToHex = function (d) {
    if (d > 15) {
        return d.toString(16)
    } else {
        return "0" + d.toString(16)
    }
}

StiJsViewer.prototype.FullColorHex = function (r, g, b) {
    var red = this.RgbToHex(r);
    var green = this.RgbToHex(g);
    var blue = this.RgbToHex(b);
    return red + green + blue;
};

StiJsViewer.prototype.LightenDarkenColor = function (col, amt) {
    var usePound = false;
    if (col[0] == "#") {
        col = col.slice(1);
        usePound = true;
    }
    var num = parseInt(col, 16);
    var r = (num >> 16) + amt;
    if (r > 255) r = 255;
    else if (r < 0) r = 0;
    var b = ((num >> 8) & 0x00FF) + amt;
    if (b > 255) b = 255;
    else if (b < 0) b = 0;
    var g = (num & 0x0000FF) + amt;
    if (g > 255) g = 255;
    else if (g < 0) g = 0;
    return (usePound ? "#" : "") + String("000000" + (g | (b << 8) | (r << 16)).toString(16)).slice(-6);
}

StiJsViewer.prototype.SetWindowIcon = function (imageSrc, doc_) {
    var doc = doc_ || document;
    var head = doc.head || doc.getElementsByTagName("head")[0];
    var link = doc.createElement("link"),
        oldLink = doc.getElementById("window-icon");
    link.id = "window-icon";
    link.rel = "icon";
    link.href = imageSrc;
    link.setAttribute("stimulsoft", "stimulsoft");
    if (oldLink) {
        head.removeChild(oldLink);
    }
    head.appendChild(link);
}

StiJsViewer.prototype.sortPropsInDrillDownParameters = function (in_array) {
    if (!in_array || !in_array.length) return in_array;
    var out_array = [];
    for (var i = 0; i < in_array.length; i++) {
        var propNames = [];
        var el = in_array[i];
        var copy_el = {};
        if (el.rowCels) el.rowCels = this.sortPropsInDrillDownParameters(el.rowCels);
        for (var p in el) {
            // eslint-disable-next-line no-prototype-builtins
            if (el.hasOwnProperty(p)) {
                propNames.push(p);
            }
        }
        propNames.sort();
        for (var k = 0; k < propNames.length; k++) {
            copy_el[propNames[k]] = el[propNames[k]];
        }
        out_array[i] = copy_el;
    }
    return out_array;
}

StiJsViewer.prototype.getBackText = function (withoutBrackets) {
    var backText = String.fromCharCode(84) + "r" + String.fromCharCode(105) + "a";
    if (withoutBrackets) return backText + String.fromCharCode(108);
    return String.fromCharCode(91) + backText + String.fromCharCode(108) + String.fromCharCode(93);
}

StiJsViewer.prototype.removeElementEvents = function (element) {
    var remove = function (node) {
        if (node == null) return;
        node.onmousedown = null;
        node.onmouseup = null;
        node.onclick = null;
        node.overrideonclick = null;
        node.ondblclick = null;
        node.overrideondblclick = null;
        node.onmousemove = null;
        node.onmouseover = null;
        node.overrideonmouseover = null;
        node.onmousewheel = null;
        node.onmouseout = null;
        node.overrideonmouseout = null;
        node.oncontextmenu = null;
        node.onmouseenter = null;
        node.overrideonmouseenter = null;
        node.onmouseleave = null;
        node.overrideonmouseleave = null;

        node.ontouchstart = null;
        node.ontouchmove = null;
        node.ontouchend = null;
        node.ontouchcancel = null;

        node.onkeydown = null;
        node.onkeypress = null;
        node.onkeyup = null;

        node.onfocus = null;
        node.onblur = null;
        node.onchange = null;
        node.onsubmit = null;

        node.onscroll = null;
        node.onresize = null;
        node.onhashchange = null;
        node.onload = null;
        node.onunload = null;

        node.onbeforeunload = null;
        node.ondrag = null;
        node.ondrop = null;

        for (var index = 0; index < node.childNodes.length; index++) {
            remove(node.childNodes[index]);
        }
    };

    for (var index = 0; index < element.childNodes.length; index++) {
        remove(element.childNodes[index]);
    }
}

StiJsViewer.prototype.maskTextBox = function (textBox, mask) {
    var caretTimeoutId;
    var len;

    var autoclear = true;
    var placeholder = "_";

    var defs = {
        "9": "[0-9]",
        a: "[A-Za-z]",
        "*": "[A-Za-z0-9]"
    };

    var tests = [];
    var partialPosition = len = mask.length;
    var firstNonMaskPos = null;
    var buffer = [];

    var maskSplit = mask.split("");
    for (var i = 0; i < maskSplit.length; i++) {
        var c = maskSplit[i];
        "?" == c ? (len--, partialPosition = i) : defs[c] ? (tests.push(new RegExp(defs[c])),
            null === firstNonMaskPos && (firstNonMaskPos = tests.length - 1), partialPosition > i) : tests.push(null);

        buffer[i] = c != "?" ? defs[c] ? getPlaceholder(i) : c : void 0;
    }

    var defaultBuffer = buffer.join("");
    var focusText = textBox.value;

    function caret(begin, end) {
        var range;
        return typeof begin == "number" ? (end = typeof end == "number" ? end : begin,
            textBox.setSelectionRange ? textBox.setSelectionRange(begin, end) : textBox.createTextRange && (range = textBox.createTextRange(),
                range.collapse(!0), range.moveEnd("character", end), range.moveStart("character", begin),
                range.select())
        ) : (textBox.setSelectionRange ? (begin = textBox.selectionStart, end = textBox.selectionEnd) : document.selection && document.selection.createRange && (range = document.selection.createRange(),
            begin = 0 - range.duplicate().moveStart("character", -1e5), end = begin + range.text.length),
        {
            begin: begin,
            end: end
        });
    }
    function getPlaceholder(i) {
        return placeholder.charAt(i < placeholder.length ? i : 0);
    }
    function seekNext(pos) {
        for (; ++pos < len && !tests[pos];);
        return pos;
    }
    function seekPrev(pos) {
        for (; --pos >= 0 && !tests[pos];);
        return pos;
    }
    function shiftL(begin, end) {
        var i, j;
        if (!(0 > begin)) {
            for (i = begin, j = seekNext(end); len > i; i++) if (tests[i]) {
                if (!(len > j && tests[i].test(buffer[j]))) break;
                buffer[i] = buffer[j], buffer[j] = getPlaceholder(j), j = seekNext(j);
            }
            writeBuffer(), caret(Math.max(firstNonMaskPos, begin));
        }
    }
    function shiftR(pos) {
        var i, c, j, t;
        for (i = pos, c = getPlaceholder(pos); len > i; i++) if (tests[i]) {
            if (j = seekNext(i), t = buffer[i], buffer[i] = c, !(len > j && tests[j].test(t))) break;
            c = t;
        }
    }
    function blurEvent() {
        checkVal();
    }
    function keydownEvent(e) {
        var pos, begin, end;
        var k = e.which || e.keyCode;

        if (8 === k || 46 === k || 127 === k) {
            pos = caret();
            begin = pos.begin;

            end = pos.end, end - begin === 0 &&

                (begin = 46 !== k ? seekPrev(begin) :
                    end = seekNext(begin - 1),
                    end = 46 === k ? seekNext(end) : end),

                clearBuffer(begin, end),
                shiftL(begin, end - 1),
                e.preventDefault();
        }
        else {
            if (13 === k) {
                blurEvent.call(this, e);
            }
            if (27 === k) {
                textBox.value = focusText;
                caret(0, checkVal());
                e.preventDefault();
            }
        }
    }
    function keypressEvent(e) {
        var p, c, next;
        var k = e.which || e.keyCode;
        var pos = caret();

        if (!(e.ctrlKey || e.altKey || e.metaKey || 32 > k) && k && 13 !== k) {
            if (pos.end - pos.begin !== 0 && (clearBuffer(pos.begin, pos.end), shiftL(pos.begin, pos.end - 1)),
                p = seekNext(pos.begin - 1),
                len > p && (c = String.fromCharCode(k), tests[p].test(c))) {
                shiftR(p);
                buffer[p] = c;
                writeBuffer();
                next = seekNext(p);
                caret(next);
            }
            e.preventDefault();
        }
    }
    function clearBuffer(start, end) {
        var i;
        for (i = start; end > i && len > i; i++) tests[i] && (buffer[i] = getPlaceholder(i));
    }
    function writeBuffer() {
        textBox.value = buffer.join("");
    }
    function checkVal(allow) {
        var c;
        var i;
        var pos = 0;
        var test = textBox.value;
        var lastMatch = -1;

        for (i = 0; len > i; i++) {
            if (tests[i]) {
                for (buffer[i] = getPlaceholder(i); pos++ < test.length;) if (c = test.charAt(pos - 1),
                    tests[i].test(c)) {
                    buffer[i] = c;
                    lastMatch = i;
                    break;
                }
                if (pos > test.length) {
                    clearBuffer(i + 1, len);
                    break;
                }
            } else {
                if (buffer[i] === test.charAt(pos))
                    pos++;

                if (partialPosition > i)
                    lastMatch = i;
            }
        }

        if (allow) {
            writeBuffer();
        }
        else {
            if (partialPosition > lastMatch + 1) {
                if (autoclear || buffer.join("") === defaultBuffer) {
                    if (textBox.value) {
                        textBox.value = "";
                        clearBuffer(0, len);
                    }
                }
                else {
                    writeBuffer();
                }
            }
            else {
                writeBuffer();
                textBox.value = textBox.value.substring(0, lastMatch + 1);
            }
        }

        return partialPosition ? i : firstNonMaskPos;
    }
    function inputPasteEvent(e) {
        setTimeout(function () {
            var pos = checkVal(!0);
            caret(pos);
        }, 0);
    }
    function focusEvent(e) {
        clearTimeout(caretTimeoutId);
        focusText = textBox.value;
        var pos = checkVal();
        caretTimeoutId = setTimeout(function () {
            textBox === document.activeElement;
            writeBuffer();
            if (pos == mask.replace("?", "").length)
                caret(0, pos)
            else
                caret(pos);
        }, 10);
    }


    this.addEvent(textBox, "focus", focusEvent);
    this.addEvent(textBox, "blur", blurEvent);
    this.addEvent(textBox, "keydown", keydownEvent);
    this.addEvent(textBox, "keypress", keypressEvent);
    this.addEvent(textBox, "mask", inputPasteEvent);
    this.addEvent(textBox, "paste", inputPasteEvent);

    checkVal();
}

StiJsViewer.prototype.applyPreviewSettingsToDashboardsPanel = function (previewSettings) {    
    var needRebuildDbsPanel = false;

    if (this.options.toolbar.alignment == "Default" && previewSettings.dashboardToolbarHorAlignment && previewSettings.dashboardToolbarHorAlignment != "Right") {
        this.options.previewSettingsDbsToolbarAlign = previewSettings.dashboardToolbarHorAlignment.toLowerCase();
        needRebuildDbsPanel = true;
    }

    if (!this.options.appearance.rightToLeft && previewSettings.dashboardToolbarReverse) {
        this.options.previewSettingsDbsToolbarReverse = true;
        needRebuildDbsPanel = true;
    }

    if (needRebuildDbsPanel) {        
        this.InitializeDashboardsPanel();
        this.options.dashboardsPanelRebuilded = true;

        if (this.controls.buttons.FullScreenDashboard) {
            this.controls.buttons.FullScreenDashboard.setFullScreenState(this.options.appearance.fullScreenMode);
        }
    }
}

StiJsViewer.prototype.applyPreviewSettingsToViewer = function (previewSettings) {
    var toolBar = this.controls.toolbar;
    var needRebuildToolbar = false;

    if (!this.options.isMobileDevice) {
        if (this.options.toolbar.alignment == "Default" && previewSettings.reportToolbarHorAlignment && previewSettings.reportToolbarHorAlignment != "Left") {
            this.options.previewSettingsRepToolbarAlign = previewSettings.reportToolbarHorAlignment.toLowerCase();
            needRebuildToolbar = true;
        }

        if (!this.options.appearance.rightToLeft && previewSettings.reportToolbarReverse) {
            this.options.previewSettingsRepToolbarReverse = true;
            needRebuildToolbar = true;
        }

        if (needRebuildToolbar) {
            toolBar = this.InitializeToolBar();
            this.options.toolBarRebuilded = true;
        }
    }

    if (toolBar && previewSettings) {
        if (toolBar.controls.Print) toolBar.controls.Print.style.display = previewSettings.reportPrint ? "" : "none";
        if (toolBar.controls.Open) toolBar.controls.Open.style.display = previewSettings.reportOpen ? "" : "none";
        if (toolBar.controls.Save) toolBar.controls.Save.style.display = previewSettings.reportSave ? "" : "none";
        if (toolBar.controls.SendEmail) toolBar.controls.SendEmail.style.display = previewSettings.reportSendEMail ? "" : "none";
        if (toolBar.controls.Editor) toolBar.controls.Editor.style.display = previewSettings.reportEditor ? "" : "none";
        if (toolBar.controls.Find) toolBar.controls.Find.style.display = previewSettings.reportFind ? "" : "none";
        if (toolBar.controls.Signature) toolBar.controls.Signature.style.display = previewSettings.reportSignature ? "" : "none";
        if (toolBar.controls.ViewMode) toolBar.controls.ViewMode.style.display = previewSettings.reportPageViewMode ? "" : "none";
        if (toolBar.controls.Parameters) toolBar.controls.Parameters.style.display = previewSettings.reportParameters ? "" : "none";
        if (toolBar.controls.Bookmarks) toolBar.controls.Bookmarks.style.display = previewSettings.reportBookmarks ? "" : "none";
        if (toolBar.controls.Resources) toolBar.controls.Resources.style.display = previewSettings.reportResources ? "" : "none";
        if (toolBar.controls.Zoom) toolBar.controls.Zoom.style.display = previewSettings.reportZoom ? "" : "none";
        if (toolBar.controls.PageControl) {
            toolBar.controls.PageControl.style.display = previewSettings.reportPageControl ? "" : "none";
            if (toolBar.controls.FirstPage) toolBar.controls.FirstPage.style.display = previewSettings.reportPageControl ? "" : "none";
            if (toolBar.controls.PrevPage) toolBar.controls.PrevPage.style.display = previewSettings.reportPageControl ? "" : "none";
            if (toolBar.controls.NextPage) toolBar.controls.NextPage.style.display = previewSettings.reportPageControl ? "" : "none";
            if (toolBar.controls.LastPage) toolBar.controls.LastPage.style.display = previewSettings.reportPageControl ? "" : "none";
        }
        if (!previewSettings.reportToolbar && this.options.toolbar.visible) {
            toolBar.oldWidth = toolBar.style.width;
            toolBar.oldHeight = toolBar.style.height;
            toolBar.style.width = toolBar.style.height = "0px";
            this.updateLayout();
        }
        if (previewSettings.reportToolbar && this.options.toolbar.visible && toolBar.oldWidth != null && toolBar.oldHeight != null) {
            toolBar.style.width = toolBar.oldWidth;
            toolBar.style.height = toolBar.oldHeight;
            toolBar.oldWidth = toolBar.oldHeight = null;
            this.updateLayout();
        }
        var navigatePanel = this.controls.navigatePanel;
        if (navigatePanel) {
            if (!previewSettings.reportStatusBar) {
                navigatePanel.oldWidth = navigatePanel.style.width;
                navigatePanel.oldHeight = navigatePanel.style.height;
                navigatePanel.style.width = navigatePanel.style.height = "0px";
                this.updateLayout();
            }
            if (previewSettings.reportStatusBar && navigatePanel.oldWidth != null && navigatePanel.oldHeight != null) {
                navigatePanel.style.width = navigatePanel.oldWidth;
                navigatePanel.style.height = navigatePanel.oldHeight;
                navigatePanel.oldWidth = navigatePanel.oldHeight = null;
                this.updateLayout();
            }
        }
    }
}

StiJsViewer.prototype.copyTextToClipboard = function (text) {
    var textArea = document.createElement("textarea");
    textArea.setAttribute("style", "position: fixed; top: 0; left: 0; width: 2em; height: 2em; padding: 0; border: none; outline: none; box-shadow: none; background: transparent;");
    textArea.value = text;

    document.body.appendChild(textArea);
    textArea.select();

    try {
        document.execCommand('copy');
    } catch (err) {
        console.log(err);
    }

    document.body.removeChild(textArea);
}

StiJsViewer.prototype.hideDocToolTip = function () {
    if (document._stiTooltip) {
        document._stiTooltip.cx = -0.2;
    }
}

StiJsViewer.prototype.addGradientBrushToElement = function (element) {
    var grad = this.CreateSvgElement("linearGradient");
    element.appendChild(grad);
    var gradId = "gradient" + this.generateKey();
    grad.setAttribute("id", gradId);
    grad.setAttribute("x1", "0%");
    grad.setAttribute("y1", "0%");
    grad.setAttribute("x2", "100%");
    grad.setAttribute("y2", "0%");
    grad.stop1 = this.CreateSvgElement("stop");
    grad.stop1.setAttribute("offset", "0");
    grad.appendChild(grad.stop1);
    grad.stop2 = this.CreateSvgElement("stop");
    grad.stop2.setAttribute("offset", "50%");
    grad.appendChild(grad.stop2);
    grad.stop3 = this.CreateSvgElement("stop");
    grad.stop3.setAttribute("offset", "100%");
    grad.appendChild(grad.stop3);
    grad.rect = this.CreateSvgElement("rect");
    grad.rect.setAttribute("x", "0");
    grad.rect.setAttribute("y", "0");
    grad.rect.setAttribute("width", "100%");
    grad.rect.setAttribute("height", "100%");
    grad.rect.setAttribute("fill", "url(#" + gradId + ")");
    grad.rect.style.display = "none";
    element.appendChild(grad.rect);
    grad.path = this.CreateSvgElement("path");
    grad.path.setAttribute("fill", "url(#" + gradId + ")");
    grad.path.style.display = "none";
    element.appendChild(grad.path);

    grad.applyBrush = function (brushArray, isRounded) {
        if (brushArray && brushArray.length >= 3) {
            grad.stop1.setAttribute("stop-color", brushArray[1]);

            if (brushArray[0] == "3") {
                if (grad.stop2.parentNode) {
                    grad.stop2.parentNode.removeChild(grad.stop2);
                }
                grad.stop3.setAttribute("stop-color", brushArray[2]);
            }
            else {
                grad.stop2.setAttribute("stop-color", brushArray[2]);
                grad.insertBefore(grad.stop2, grad.stop3);
                grad.stop3.setAttribute("stop-color", brushArray[1]);
            }

            var angle = parseInt(brushArray[3]) - 180;
            var pi = angle * (Math.PI / 180);
            var x1 = parseInt(50 + Math.cos(pi) * 50);
            var y1 = parseInt(50 + Math.sin(pi) * 50);
            var x2 = parseInt(50 + Math.cos(pi + Math.PI) * 50);
            var y2 = parseInt(50 + Math.sin(pi + Math.PI) * 50);

            grad.setAttribute("x1", x1 + "%");
            grad.setAttribute("y1", y1 + "%");
            grad.setAttribute("x2", x2 + "%");
            grad.setAttribute("y2", y2 + "%");

            if (isRounded)
                grad.path.style.display = "";
            else
                grad.rect.style.display = "";
        }
    }

    return grad;
}

StiJsViewer.prototype.getSvgHatchBrush = function (brushProps) {
    var brushSvg = this.CreateSvgElement("svg");
    var brushId = this.newGuid();
    var foreColor = brushProps[1];
    var backColor = brushProps[2];
    var hatchNumber = parseInt(brushProps[3]);
    if (hatchNumber > 53) hatchNumber = 53;

    this.addHatchBrushPatternToElement(brushSvg, brushId, hatchNumber, foreColor, backColor);

    var rect = this.CreateSvgElement("rect");
    brushSvg.rect = rect;
    rect.setAttribute("x", 0);
    rect.setAttribute("y", 0);
    rect.setAttribute("width", "100%");
    rect.setAttribute("height", "100%");
    rect.setAttribute("fill", "url(#" + brushId + ")");
    brushSvg.appendChild(rect);

    return brushSvg;
}

StiJsViewer.prototype.addHatchBrushPatternToElement = function (element, patternId, hatchNumber, foreColor, backColor) {
    var brushPattern = this.CreateSvgElement("pattern");
    element.appendChild(brushPattern);

    brushPattern.setAttribute("id", patternId);
    brushPattern.setAttribute("x", "0");
    brushPattern.setAttribute("y", "0");
    brushPattern.setAttribute("width", "8");
    brushPattern.setAttribute("height", "8");
    brushPattern.setAttribute("patternUnits", "userSpaceOnUse");

    var sb = "";
    var hatchHex = this.hatchData[hatchNumber];

    for (var index = 0; index < 16; index++) {
        sb += this.hexToByteString(hatchHex.charAt(index));
    }

    var brushRect = this.CreateSvgElement("rect");
    brushPattern.appendChild(brushRect);
    brushRect.setAttribute("x", "0");
    brushRect.setAttribute("y", "0");
    brushRect.setAttribute("width", "8");
    brushRect.setAttribute("height", "8");
    brushRect.setAttribute("fill", backColor);


    for (var indexRow = 0; indexRow < 8; indexRow++) {
        for (var indexColumn = 0; indexColumn < 8; indexColumn++) {

            var indexChar = sb.charAt(indexRow * 8 + indexColumn);

            if (indexChar == "1") {
                var brushRect2 = this.CreateSvgElement("rect");
                brushPattern.appendChild(brushRect2);
                brushRect2.setAttribute("x", indexColumn);
                brushRect2.setAttribute("y", indexRow.toString());
                brushRect2.setAttribute("width", "1");
                brushRect2.setAttribute("height", "1");
                brushRect2.setAttribute("fill", foreColor);

            }
        }
    }
}

StiJsViewer.prototype.stopAllTimers = function () {
    this.stopRefreshReportTimer();
    this.stopAutoUpdateCache();
}

StiJsViewer.prototype.removeAllEvents = function (node) {
    var events = ["onload", "onclick", "onerror", "onfocus", "onabort", "onbeforeunload", "onblur", "onchange", "oninvalid", "onresize",
        "onmousedown", "onmouseover", "onmouseout", "onmouseeneter", "onmouseleave", "onunload", "onmousemove", "ontouchstart", "ontouchmove", "ontouchend"];

    for (var i = 0; i < events.length; i++) {
        node[events[i]] = null;
    }
}

StiJsViewer.prototype.hatchData = [
    "000000FF00000000",	//HatchStyleHorizontal = 0
    "1010101010101010",	//HatchStyleVertical = 1,			
    "8040201008040201",	//HatchStyleForwardDiagonal = 2,	
    "0102040810204080",	//HatchStyleBackwardDiagonal = 3,	
    "101010FF10101010",	//HatchStyleCross = 4,			
    "8142241818244281",	//HatchStyleDiagonalCross = 5,	
    "8000000008000000",	//HatchStyle05Percent = 6,		
    "0010000100100001",	//HatchStyle10Percent = 7,		
    "2200880022008800",	//HatchStyle20Percent = 8,		
    "2288228822882288",	//HatchStyle25Percent = 9,		
    "2255885522558855",	//HatchStyle30Percent = 10,		
    "AA558A55AA55A855",	//HatchStyle40Percent = 11,		
    "AA55AA55AA55AA55",	//HatchStyle50Percent = 12,		
    "BB55EE55BB55EE55",	//HatchStyle60Percent = 13,		
    "DD77DD77DD77DD77",	//HatchStyle70Percent = 14,		
    "FFDDFF77FFDDFF77",	//HatchStyle75Percent = 15,		
    "FF7FFFF7FF7FFFF7",	//HatchStyle80Percent = 16,		
    "FF7FFFFFFFF7FFFF",	//HatchStyle90Percent = 17,		
    "8844221188442211",	//HatchStyleLightDownwardDiagonal = 18,	
    "1122448811224488",	//HatchStyleLightUpwardDiagonal = 19,	
    "CC663399CC663399",	//HatchStyleDarkDownwardDiagonal = 20,	
    "993366CC993366CC",	//HatchStyleDarkUpwardDiagonal = 21,	
    "E070381C0E0783C1",	//HatchStyleWideDownwardDiagonal = 22,	
    "C183070E1C3870E0",	//HatchStyleWideUpwardDiagonal = 23,	
    "4040404040404040",	//HatchStyleLightVertical = 24,			
    "00FF000000FF0000",	//HatchStyleLightHorizontal = 25,		
    "AAAAAAAAAAAAAAAA",	//HatchStyleNarrowVertical = 26,		
    "FF00FF00FF00FF00",	//HatchStyleNarrowHorizontal = 27,		
    "CCCCCCCCCCCCCCCC",	//HatchStyleDarkVertical = 28,			
    "FFFF0000FFFF0000",	//HatchStyleDarkHorizontal = 29,		
    "8844221100000000",	//HatchStyleDashedDownwardDiagonal = 30,
    "1122448800000000",	//HatchStyleDashedUpwardDiagonal = 311,	
    "F00000000F000000",	//HatchStyleDashedHorizontal = 32,		
    "8080808008080808",	//HatchStyleDashedVertical = 33,		
    "0240088004200110",	//HatchStyleSmallConfetti = 34,			
    "0C8DB130031BD8C0",	//HatchStyleLargeConfetti = 35,		
    "8403304884033048",	//HatchStyleZigZag = 36,			
    "00304A8100304A81",	//HatchStyleWave = 37,				
    "0102040818244281",	//HatchStyleDiagonalBrick = 38,		
    "202020FF020202FF",	//HatchStyleHorizontalBrick = 39,	
    "1422518854224588",	//HatchStyleWeave = 40,				
    "F0F0F0F0AA55AA55",	//HatchStylePlaid = 41,				
    "0100201020000102",	//HatchStyleDivot = 42,				
    "AA00800080008000",	//HatchStyleDottedGrid = 43,		
    "0020008800020088",	//HatchStyleDottedDiamond = 44,		
    "8448300C02010103",	//HatchStyleShingle = 45,			
    "33FFCCFF33FFCCFF",	//HatchStyleTrellis = 46,			
    "98F8F877898F8F77",	//HatchStyleSphere = 47,			
    "111111FF111111FF",	//HatchStyleSmallGrid = 48,			
    "3333CCCC3333CCCC",	//HatchStyleSmallCheckerBoard = 49,	
    "0F0F0F0FF0F0F0F0",	//HatchStyleLargeCheckerBoard = 50,	
    "0502058850205088",	//HatchStyleOutlinedDiamond = 51,	
    "10387CFE7C381000",	//HatchStyleSolidDiamond = 52,
    "0000000000000000"	//HatchStyleTotal = 53
];

StiJsViewer.prototype.hexToByteString = function (hex) {
    switch (hex) {
        case "1": return "0001";
        case "2": return "0010";
        case "3": return "0011";
        case "4": return "0100";
        case "5": return "0101";
        case "6": return "0110";
        case "7": return "0111";
        case "8": return "1000";
        case "9": return "1001";
        case "A": return "1010";
        case "B": return "1011";
        case "C": return "1100";
        case "D": return "1101";
        case "E": return "1110";
        case "F": return "1111";
    }
    return "0000";
}

StiJsViewer.prototype.constWebColors = [
    ["Transparent", "transparent"],
    ["Black", "0,0,0"],
    ["DimGray", "105,105,105"],
    ["Gray", "128,128,128"],
    ["DarkGray", "169,169,169"],
    ["Silver", "192,192,192"],
    ["LightGray", "211,211,211"],
    ["Gainsboro", "220,220,220"],
    ["WhiteSmoke", "245,245,245"],
    ["White", "255,255,255"],
    ["RosyBrown", "188,143,143"],
    ["IndianRed", "205,92,92"],
    ["Brown", "165,42,42"],
    ["Firebrick", "178,34,34"],
    ["LightCoral", "240,128,12"],
    ["Maroon", "128,0,0"],
    ["DarkRed", "139,0,0"],
    ["Red", "255,0,0"],
    ["Snow", "255,250,250"],
    ["MistyRose", "255,228,225"],
    ["Salmon", "250,128,114"],
    ["Tomato", "255,99,71"],
    ["DarkSalmon", "233,150,122"],
    ["Coral", "255,127,80"],
    ["OrangeRed", "255,69,0"],
    ["LightSalmon", "255,160,122"],
    ["Sienna", "160,82,45"],
    ["SeaShell", "255,245,23"],
    ["Chocolate", "210,105,30"],
    ["SaddleBrown", "139,69,19"],
    ["SandyBrown", "244,164,96"],
    ["PeachPuff", "255,218,185"],
    ["Peru", "205,133,63"],
    ["Linen", "250,240,230"],
    ["Bisque", "255,228,196"],
    ["DarkOrange", "255,140,0"],
    ["BurlyWood", "222,184,135"],
    ["Tan", "210,180,140"],
    ["AntiqueWhite", "250,235,215"],
    ["NavajoWhite", "255,222,173"],
    ["BlanchedAlmond", "255,235,205"],
    ["PapayaWhip", "255,239,213"],
    ["Moccasin", "255,228,181"],
    ["Orange", "255,165,0"],
    ["Wheat", "245,222,179"],
    ["OldLace", "253,245,230"],
    ["FloralWhite", "255,250,240"],
    ["DarkGoldenrod", "184,134,11"],
    ["Goldenrod", "218,165,32"],
    ["Cornsilk", "255,248,220"],
    ["Gold", "255,215,0"],
    ["Khaki", "240,230,140"],
    ["LemonChiffon", "255,250,205"],
    ["PaleGoldenrod", "238,232,170"],
    ["DarkKhaki", "189,183,107"],
    ["Beige", "245,245,220"],
    ["LightGoldenrodYellow", "250,250,210"],
    ["Olive", "128,128,0"],
    ["Yellow", "255,255,0"],
    ["LightYellow", "255,255,224"],
    ["Ivory", "255,255,240"],
    ["OliveDrab", "107,142,35"],
    ["YellowGreen", "154,205,50"],
    ["DarkOliveGreen", "85,107,47"],
    ["GreenYellow", "173,255,47"],
    ["Chartreuse", "127,255,0"],
    ["LawnGreen", "124,252,0"],
    ["DarkSeaGreen", "143,188,139"],
    ["ForestGreen", "34,139,34"],
    ["LimeGreen", "50,205,50"],
    ["LightGreen", "144,238,144"],
    ["PaleGreen", "152,251,152"],
    ["DarkGreen", "0,100,0"],
    ["Green", "0,128,0"],
    ["Lime", "0,255,0"],
    ["Honeydew", "240,255,240"],
    ["SeaGreen", "46,139,87"],
    ["MediumSeaGreen", "60,179,113"],
    ["SpringGreen", "0,255,127"],
    ["MintCream", "245,255,250"],
    ["MediumSpringGreen", "0,250,154"],
    ["MediumAquamarine", "102,205,170"],
    ["Aquamarine", "127,255,212"],
    ["Turquoise", "64,224,20"],
    ["LightSeaGreen", "32,178,170"],
    ["MediumTurquoise", "72,209,204"],
    ["DarkSlateGray", "47,79,79"],
    ["PaleTurquoise", "175,238,23"],
    ["Teal", "0,128,12"],
    ["DarkCyan", "0,139,139"],
    ["Aqua", "0,255,255"],
    ["Cyan", "0,255,255"],
    ["LightCyan", "224,255,255"],
    ["Azure", "240,255,255"],
    ["DarkTurquoise", "0,206,209"],
    ["CadetBlue", "95,158,160"],
    ["PowderBlue", "176,224,230"],
    ["LightBlue", "173,216,230"],
    ["DeepSkyBlue", "0,191,255"],
    ["SkyBlue", "135,206,235"],
    ["LightSkyBlue", "135,206,250"],
    ["SteelBlue", "70,130,180"],
    ["AliceBlue", "240,248,255"],
    ["DodgerBlue", "30,144,255"],
    ["SlateGray", "112,128,144"],
    ["LightSlateGray", "119,136,153"],
    ["LightSteelBlue", "176,196,222"],
    ["CornflowerBlue", "100,149,237"],
    ["RoyalBlue", "65,105,225"],
    ["MidnightBlue", "25,25,112"],
    ["Lavender", "230,230,250"],
    ["Navy", "0,0,12"],
    ["DarkBlue", "0,0,139"],
    ["MediumBlue", "0,0,205"],
    ["Blue", "0,0,255"],
    ["GhostWhite", "248,248,255"],
    ["SlateBlue", "106,90,205"],
    ["DarkSlateBlue", "72,61,139"],
    ["MediumSlateBlue", "123,104,23"],
    ["MediumPurple", "147,112,219"],
    ["BlueViolet", "138,43,226"],
    ["Indigo", "75,0,130"],
    ["DarkOrchid", "153,50,204"],
    ["DarkViolet", "148,0,211"],
    ["MediumOrchid", "186,85,211"],
    ["Thistle", "216,191,216"],
    ["Plum", "221,160,221"],
    ["Violet", "238,130,23"],
    ["Purple", "128,0,12"],
    ["DarkMagenta", "139,0,139"],
    ["Magenta", "255,0,255"],
    ["Fuchsia", "255,0,255"],
    ["Orchid", "218,112,214"],
    ["MediumVioletRed", "199,21,133"],
    ["DeepPink", "255,20,147"],
    ["HotPink", "255,105,180"],
    ["LavenderBlush", "255,240,245"],
    ["PaleVioletRed", "219,112,147"],
    ["Crimson", "220,20,60"],
    ["Pink", "255,192,203"],
    ["LightPink", "255,182,193"]
];