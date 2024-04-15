
StiMobileDesigner.prototype.InitializeFormsDesignerFrame = function (callbackFunc) {    
    var jsObject = this;
    var frame = this.options.formsDesignerFrame;

    jsObject.options.formsDesignerMode = true;
    
    if (!frame) {
        jsObject.options.processImage.show();

        frame = this.options.formsDesignerFrame = document.createElement("iframe");
        frame.className = "stiDesignerFormsFrame";
        frame.style.display = "none";
        frame.src = jsObject.options.formsDesignerUrl + "?s=" + jsObject.options.cloudParameters.sessionKey + "&i=" + jsObject.options.cloudParameters.reportTemplateItemKey;
        this.options.mainPanel.appendChild(frame);

        frame.show = function () {
            frame.style.display = "";
        }

        frame.hide = function () {
            frame.style.display = "none";
        }

        frame.close = function () {
            frame.hide();
            frame.formName = null;
            jsObject.options.formsDesignerMode = false;
            jsObject.SetWindowTitle(jsObject.loc.FormDesigner.title);
        }

        frame.sendCommand = function (data) {
            if (frame.contentWindow && data) {
                frame.contentWindow.postMessage(JSON.stringify(data), jsObject.options.formsDesignerUrl);
            }
        }

        frame.openForm = function (formName, formContent) {
            frame.formName = formName;
            frame.show();
            frame.sendCommand({ action: "openForm", formContent: formContent });
            jsObject.SetWindowTitle(formName + " - " + jsObject.loc.FormDesigner.title);
        }

        frame.checkLicense = function (completeFunc) {
            var cloudParams = jsObject.options.cloudParameters;
            var user = cloudParams ? cloudParams.user : null;
            var userKey = cloudParams ? cloudParams.userKey : null;
            var locName = jsObject.localizationControl.locName || "en";

            if (user && userKey) {
                var params = {
                    UserName: user ? user.UserName : "",
                    Type: "Developer",
                    Format: "Base64",
                    ResultSvr: false,
                    Version: jsObject.options.shortProductVersion
                };
                jsObject.SendCloudCommand("LicenseActivate", params,
                    function (data) {
                        if (data.ResultLicenseKey) {
                            frame.sendCommand({ action: "setLicenseProducts", products: jsObject.ConvertProductsToJSFormat(data.ResultLicenseKey.Products) });
                            frame.sendCommand({ action: "setLocalization", locName: locName });
                        }
                        if (completeFunc) completeFunc();
                    },
                    function (data, msg) {
                        if (completeFunc) completeFunc();
                    }
                );
            }
            else {
                frame.sendCommand({ action: "setLicenseProducts", products: [] });
                frame.sendCommand({ action: "setLocalization", locName: locName });
                if (completeFunc) completeFunc();
            }
        }

        frame.onload = function () {
            frame.checkLicense(function () {
                if (callbackFunc) callbackFunc(frame);
                jsObject.options.processImage.hide();
            });
        };
    }
    else {
        if (callbackFunc) callbackFunc(frame);
    }

    return frame;
}