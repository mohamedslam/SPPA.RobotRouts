
StiMobileDesigner.prototype.InitializePublishForm_ = function () {
    var form = this.BaseForm("publishForm", this.loc.Cloud.ButtonPublish, 1);
    form.hideButtonsPanel();

    this.AddProgressToControl(form.container);

    var frame = document.createElement("iframe");
    frame.style.height = "770px";
    frame.style.width = "1200px";
    frame.style.border = "0px";
    frame.overflow = "auto";
    form.container.appendChild(frame);
    form.frame = frame;

    form.show = function (reportString, newWindow) {
        var jsObject = this.jsObject;
        form.container.progress.show();
        form.frame.src = "about:blank";

        var iframeDoc = null;

        form.frame.onload = function () {
            iframeDoc = frame.contentWindow.document;

            frame.onload = function () {
                form.container.progress.hide();
            }
        }

        var params = {
            includedToDesigner: jsObject.options.jsMode,
            themeName: (jsObject.options.jsMode && jsObject.options.themeIdent && jsObject.options.themeTone && jsObject.options.themeAccent
                ? jsObject.options.themeIdent + jsObject.options.themeTone + jsObject.options.themeAccent
                : jsObject.options.theme || "Office2013WhiteBlue"),
            localizationName: (!jsObject.options.jsMode ? jsObject.options.cultureName : jsObject.loc["@cultureName"])
        };

        if (reportString) {
            params.reportString = reportString;
        }

        var sessionKey = jsObject.options.SessionKey || (jsObject.options.cloudParameters ? jsObject.options.cloudParameters.sessionKey : null);

        if (sessionKey) {
            params.sessionKey = sessionKey;
        }

        if (jsObject.options.cloudParameters && jsObject.options.cloudParameters.userKey) {
            params.userKey = jsObject.options.cloudParameters.userKey;
        }

        var win = newWindow || this.jsObject.openNewWindow();
        if (win && win.document) {
            jsObject.PostForm(params, win.document, jsObject.options.publishUrl, true);
        }
    }

    return form;
}