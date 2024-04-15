
StiMobileDesigner.prototype.GetCloudPermissions = function () {
    var currentPlan = this.ConvertCloudPlanNumberValueToEnumValue(this.GetCloudPlanNumberValue());
    return {
        rAvailable: (currentPlan.indexOf("RBase") >= 0 || currentPlan.indexOf("RSingle") >= 0 || currentPlan.indexOf("RTeam") >= 0 || currentPlan.indexOf("REnterprise") >= 0),
        dAvailable: (currentPlan.indexOf("DBase") >= 0 || currentPlan.indexOf("DSingle") >= 0 || currentPlan.indexOf("DTeam") >= 0 || currentPlan.indexOf("DEnterprise") >= 0)
    }
}

StiMobileDesigner.prototype.GetCloudPlanNumberValue = function () {
    var workspace = this.options.cloudParameters ? this.options.cloudParameters.workspace : null;
    if (!workspace && this.options.workspace) workspace = this.options.workspace;
    return (workspace ? (workspace.ResultPlanIdent || 0) : 0);
}

StiMobileDesigner.prototype.ConvertCloudPlanEnumValueToNumberValue = function (strValue) {
    var value = 0;
    if (strValue.indexOf("RBase") >= 0) value += 1;
    if (strValue.indexOf("RSingle") >= 0) value += 2;
    if (strValue.indexOf("RTeam") >= 0) value += 4;
    if (strValue.indexOf("REnterprise") >= 0) value += 8;
    if (strValue.indexOf("DBase") >= 0) value += 32;
    if (strValue.indexOf("DSingle") >= 0) value += 64;
    if (strValue.indexOf("DTeam") >= 0) value += 128;
    if (strValue.indexOf("DEnterprise") >= 0) value += 256;
    if (strValue.indexOf("Expiried") >= 0) value += 512;

    return value;
}

StiMobileDesigner.prototype.ConvertCloudPlanNumberValueToEnumValue = function (value) {
    var tText = this.getBackText(true);
    var enumValues = {
        0: tText,
        1: "RBase",
        2: "RSingle",
        4: "RTeam",
        8: "REnterprise",
        32: "DBase",
        64: "DSingle",
        128: "DTeam",
        256: "DEnterprise",
        512: "Expiried"
    }
    if (!enumValues[value]) {
        var values = "";
        for (var enumValue in enumValues) {
            var enumValueNumber = parseInt(enumValue);
            if (enumValueNumber != 0 && (value & enumValueNumber) == enumValueNumber) {
                value -= enumValueNumber;
                values += (values.length > 0 ? ", " : "") + enumValues[enumValueNumber];
            }
        }
        if (values.length == 0) values = tText;
        return values;
    }
    else {
        return enumValues[value]
    }
}

StiMobileDesigner.prototype.GetCurrentPlanLimitValue = function (limitName) {
    var limitValue = null;
    var tText = this.getBackText(true);
    var currentPlan = this.ConvertCloudPlanNumberValueToEnumValue(this.GetCloudPlanNumberValue());
    var plansLimits = this.options.plansLimits;
    if (plansLimits) {
        if (currentPlan.indexOf(tText) >= 0) {
            var tLimits = plansLimits[tText.toLowerCase()];
            limitValue = tLimits[limitName];
        }
        if (currentPlan.indexOf("Base") >= 0)
            limitValue = plansLimits.base[limitName];
        if (currentPlan.indexOf("Single") >= 0)
            limitValue = plansLimits.single[limitName];
        if (currentPlan.indexOf("Team") >= 0)
            limitValue = plansLimits.team[limitName];
        if (currentPlan.indexOf("Enterprise") >= 0)
            limitValue = plansLimits.enterprise[limitName];
    }
    return limitValue;
}

StiMobileDesigner.prototype.UpdateWindowTitle = function () {
    if (document.title) {
        document.title = document.title.replace(" " + this.getBackText(), "");
        if (this.options.currentPage && !this.options.currentPage.valid) {
            document.title += " " + this.getBackText();
        }
    }
}

StiMobileDesigner.prototype.UpdateWatermarksOnPages = function () {
    var report = this.options.report;
    if (report) {
        var permissions = this.GetCloudPermissions();
        for (var pageName in this.options.report.pages) {
            var page = this.options.report.pages[pageName];
            if (page.controls.waterMarkBackParent) {
                page.removeChild(page.controls.waterMarkBackParent);
                page.controls.waterMarkBackParent = null;
                page.valid = true;
            }
            if ((page.isDashboard && !permissions.dAvailable) || (!page.isDashboard && !permissions.rAvailable)) {
                this.CreatePageWaterMarkBack(page);
                this.RepaintWaterMarkBack(page);
                page.valid = false;
            }
        }
    }
    if (this.options.cloudMode) {
        var permissions = this.GetCloudPermissions();
        if (!permissions.dAvailable && !permissions.rAvailable) {
            this.options.infoPanel.show();
        }
        else {
            this.options.infoPanel.hide();
        }
    }
}

StiMobileDesigner.prototype.UpdateResourcesLimits = function () {
    var maxResourceSize = this.GetCurrentPlanLimitValue("MaxResourceSize");
    if (maxResourceSize) this.options.reportResourcesMaximumSize = maxResourceSize;
    var maxResourcesCount = this.GetCurrentPlanLimitValue("MaxResources");
    if (maxResourcesCount) this.options.reportResourcesMaximumCount = maxResourcesCount;
    var maxFileSize = this.GetCurrentPlanLimitValue("MaxFileSize");
    if (maxFileSize) this.options.reportMaxFileSize = maxFileSize;
}

StiMobileDesigner.prototype.CheckSubscription = function () {
    if (this.GetCloudPlanNumberValue() == 512)
        this.BlockDesigner();
    else if (this.CheckUserTrExpired())
        this.CheckTrDays();
}

StiMobileDesigner.prototype.CheckTrDays = function () {
    var jsObject = this;
    var user = jsObject.options.cloudParameters.user;
    if (user && user.Days > 0 && user.Created) {
        try {
            var userCreated = jsObject.JSONDateFormatToDate(user.Created);
            var userTotalDays = Math.round((new Date().getTime() - userCreated.getTime()) / 1000 / 60 / 60 / 24);
            var daysLeft = user.Days - userTotalDays;
            if (daysLeft > 0) {
                setTimeout(function () {
                    jsObject.InitializeNotificationCheckTrDaysForm(function (form) {
                        form.show(daysLeft);
                    });
                }, 2000);
            }
        }
        catch (e) { console.log(e); }
    }
}

StiMobileDesigner.prototype.BlockDesigner = function () {
    var jsObject = this;
    jsObject.InitializeNotificationForm(function (form) {
        form.show(jsObject.NotificationMessages("cloudSubscriptionExpired"), jsObject.NotificationMessages("updateYourSubscription"), "Notifications.Blocked.png");

        jsObject.options.designerIsBlocked = true;
        jsObject.HideAllDesignerControls(form);

        form.upgradeButton.action = function () {
            form.changeVisibleState(false);
            window.location.href = "https://www.stimulsoft.com/en/online-store#cloud/cloud";
        }

        form.cancelAction = function () {
            window.location.href = "https://www.stimulsoft.com/en/online-store#cloud/cloud";
        }
    });
}

StiMobileDesigner.prototype.CheckUserTrExpired = function () {
    var jsObject = this;
    var user = jsObject.options.cloudParameters ? jsObject.options.cloudParameters.user : null;

    if (user) {
        if (user.Expired) {
            jsObject.InitializeNotificationForm(function (form) {
                form.show(jsObject.NotificationMessages("trSubscriptionExpired"), jsObject.NotificationMessages("continueToUse"),
                    "Notifications.Time.png", null, jsObject.NotificationMessages("trSubscriptionExpiredDescription"));

                jsObject.options.designerIsBlocked = true;
                jsObject.HideAllDesignerControls(form);

                var link = jsObject.options.cloudMode ? "https://www.stimulsoft.com/en/online-store#cloud/cloud" : "https://www.stimulsoft.com/en/online-store";

                form.upgradeButton.action = function () {
                    form.changeVisibleState(false);
                    window.location.href = link;
                }

                form.cancelAction = function () {
                    window.location.href = link;
                }
            });
            return false;
        }
        else if (user.Enabled === false) {
            jsObject.InitializeNotificationForm(function (form) {
                form.show(jsObject.NotificationMessages("accountLocked"), jsObject.NotificationMessages("accountLockedDescription"), "Notifications.Blocked.png");

                form.upgradeButton.caption.innerHTML = jsObject.loc.Buttons.Ok.replace("&", "");

                form.upgradeButton.action = function () {
                    form.changeVisibleState(false);
                    jsObject.FinishSession();
                    jsObject.options.forms.authForm.show();
                }

                form.cancelAction = function () {
                    jsObject.FinishSession();
                    jsObject.options.forms.authForm.show();
                }
            });
            return false;
        }
    }

    return true;
}

StiMobileDesigner.prototype.CheckUserActivated = function () {
    var user = this.options.cloudParameters ? this.options.cloudParameters.user : null;

    if (user && user.Activated === false) {
        this.InitializeNotificationCheckActivatedForm(function (form) {
            form.show(user.UserName);
        });
        return false;
    }

    return true;
}

StiMobileDesigner.prototype.HideAllDesignerControls = function (currentForm) {
    for (var i = 0; i < this.options.mainPanel.childNodes.length; i++) {
        var child = this.options.mainPanel.childNodes[i];
        if (child != currentForm && child.style) child.style.display = "none";
    }
}

StiMobileDesigner.prototype.RunLicenseActivate = function (networkData) {
    var jsObject = this;

    var licenseActivate = function (machineName) {
        if (jsObject.options.cloudParameters && jsObject.options.cloudParameters.user) {
            var params = {
                UserName: jsObject.options.cloudParameters.user.UserName,
                Type: "Developer",
                Format: "Base64",
                ResultSvr: false,
                MachineName: machineName,
                Version: jsObject.options.shortProductVersion
            };
            jsObject.SendCloudCommand("LicenseActivate", params);
        }
    }

    if (networkData && networkData.ipAddress) {
        jsObject.SendCommandToDesignerServer("EncryptMachineName", { ipAddress: networkData.ipAddress, countryName: networkData.countryName }, function (answer) {
            licenseActivate(answer.machineName)
        });
    }
    else {
        licenseActivate();
    }
}

StiMobileDesigner.prototype.ConvertProductsToJSFormat = function (products) {
    var jsProducts = [];

    for (var i = 0; i < products.length; i++) {
        jsProducts.push({
            ident: this.options.productsIdents[products[i].Ident],
            expirationDate: this.JSONDateFormatToDate(products[i].ExpirationDate).getTime()
        });
    }

    return jsProducts;
}