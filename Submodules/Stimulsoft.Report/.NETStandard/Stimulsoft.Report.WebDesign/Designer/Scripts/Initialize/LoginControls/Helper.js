
StiMobileDesigner.prototype.AddSmallProgressMarkerToControl = function (control, toTheRight) {
    var progressMarker = document.createElement("div");
    progressMarker.className = "stiDesignerSmallProgressMarker";
    var offset = this.options.IsTouchDevice ? "-22px" : "-20px";
    progressMarker.style.left = offset;
    progressMarker.style.top = offset;
    progressMarker.style.display = "none";

    var progress = document.createElement("div");
    progressMarker.appendChild(progress);
    progress.className = "mobile_designer_loader_mini_color";

    progressMarker.changeVisibleState = function (state) {
        if (toTheRight) {
            this.style.left = ($(this.control).width() + 5) + "px";
            this.style.top = -($(this.control).height() / 2 + 8) + "px";
        }
        progressMarker.style.display = state ? "" : "none";
    }
    progressMarker.control = control;
    control.appendChild(progressMarker);
    control.progressMarker = progressMarker;
}

StiMobileDesigner.prototype.AddBigProgressMarkerToControl = function (control) {
    var progressMarker = document.createElement("div");
    progressMarker.className = "stiDesignerBigProgressMarker";
    progressMarker.style.display = "none";
    progressMarker.style.overflow = "hidden";
    var jsObject = this;

    var progress = document.createElement("div");
    progressMarker.appendChild(progress);
    progress.className = "mobile_designer_loader";

    progressMarker.changeVisibleState = function (state, left, top) {
        progressMarker.style.display = state ? "" : "none";
        if (state) {
            progressMarker.style.left = (left || control.offsetWidth / 2 - 32) + "px";
            progressMarker.style.top = (top || $(control).height() / 2 - 32) + "px";
        }
    }

    control.appendChild(progressMarker);
    control.progressMarker = progressMarker;
}

StiMobileDesigner.prototype.StartNewSession = function () {
    this.options.toolBar.userNameButton.style.display = "";
    this.options.toolBar.loginButton.style.display = "none";
    this.options.toolBar.signUpButton.style.display = "none";

    if (this.options.formsDesignerFrame) {
        this.options.formsDesignerFrame.checkLicense();
    }
}

StiMobileDesigner.prototype.FinishSession = function () {
    if (this.options.cloudParameters.sessionKey) {
        this.SendCloudCommand("UserLogout", {}, function () { }, function () { });
    }

    var expDate = new Date(new Date().getTime() - 1);
    this.SetCookie("sti_SessionKey", "", this.options.cookiesPath, this.options.cookiesDomain, null, expDate.toUTCString());
    this.SetCookie("sti_UserKey", "", this.options.cookiesPath, this.options.cookiesDomain, null, expDate.toUTCString());
    this.options.cloudParameters.sessionKey = null;
    this.options.cloudParameters.userKey = null;
    this.options.cloudParameters.workspace = null;
    this.options.cloudParameters.reportTemplateItemKey = null;
    this.options.toolBar.userNameButton.style.display = "none";
    this.options.toolBar.loginButton.style.display = "";
    this.options.toolBar.signUpButton.style.display = "";

    if (this.options.cloudMode) {
        this.UpdateWatermarksOnPages();
        this.UpdateWindowTitle();
        this.UpdateResourcesLimits();
    }

    if (this.options.formsDesignerFrame) {
        this.options.formsDesignerFrame.checkLicense();
    }
}

StiMobileDesigner.prototype.UpdateUserNameButton = function (user) {
    //Login Button
    this.options.toolBar.userNameButton.nameCell.innerHTML = user.FirstName && user.LastName ? user.FirstName + " " + user.LastName : user.UserName;
    this.options.toolBar.userNameButton.userImageCell.innerHTML = "";

    var userSmallImg = this.getUserImage(user, true);
    if (userSmallImg) {
        userSmallImg.className = "stiDesignerToolbarUserImage";
        this.options.toolBar.userNameButton.userImageCell.appendChild(userSmallImg);
    }

    //User Menu
    var userMenu = this.options.userMenu;
    if (userMenu) {
        userMenu.userImageCell.innerHTML = "";
        var userBigImg = this.getUserImage(user, false);
        if (userBigImg) {
            userBigImg.className = "stiDesignerToolbarUserImage";
            userMenu.userImageCell.appendChild(userBigImg);
        }
        var fn = user.FirstName && user.FirstName.length > 0 ? user.FirstName : "";
        var ln = user.LastName && user.LastName.length > 0 ? user.LastName : "";
        userMenu.nameCell.innerHTML = "<div style='font-weight:bold;font-size:18px;padding-bottom:10px'>" + fn + " " + ln + "</div>" + user.UserName;
    }
}

StiMobileDesigner.prototype.setUserInfo = function (completeFunc) {
    var jsObject = this;
    var command = "CommandListRun";
    var params = {
        Commands: [],
        ResultSuccess: true
    }
    params.Commands.push({ Ident: "UserGet", UserKey: this.options.cloudParameters.userKey });
    params.Commands.push({ Ident: "WorkspaceGet", Version: this.options.shortProductVersion });
    params.Commands.push({ Ident: "DeveloperProductFetchAll", Version: this.options.shortProductVersion });

    this.SendCloudCommand(command, params,
        function (data) {
            var cloudParameters = jsObject.options.cloudParameters;
            var resultUserGet = data.ResultCommands[0];
            var resultWorkspaceGet = data.ResultCommands[1];
            var resultProductFetchAll = data.ResultCommands[2];
            cloudParameters.user = resultUserGet.ResultUser;
            cloudParameters.userName = resultUserGet.ResultUser.UserName;
            cloudParameters.workspace = resultWorkspaceGet;
            cloudParameters.products = resultProductFetchAll.ResultProducts;
            jsObject.UpdateUserNameButton(resultUserGet.ResultUser);
            jsObject.StartNewSession();

            if (jsObject.options.cloudMode) {
                jsObject.UpdateWatermarksOnPages();
                jsObject.UpdateWindowTitle();
                jsObject.UpdateResourcesLimits();
                jsObject.CheckSubscription();
                jsObject.UpdateDesignerSpecification();
                jsObject.UpdateDesignerControlsBySpecification();
            }
            if (completeFunc) completeFunc();

            if (!jsObject.options.licenseAlreadyActivated) {
                jsObject.options.licenseAlreadyActivated = true;

                setTimeout(function () {
                    jsObject.getNetworkData(function (networkData) {
                        jsObject.RunLicenseActivate(networkData);
                    });
                }, 8000);
            }
        },
        function (data) {
            jsObject.FinishSession();
        });

    if (!jsObject.options.licenseAlreadyActivated) {
        jsObject.options.licenseAlreadyActivated = true;

        setTimeout(function () {
            jsObject.getNetworkData(function (networkData) {
                jsObject.RunLicenseActivate(networkData);
            });
        }, 8000);
    }
}

StiMobileDesigner.prototype.getNetworkData = function (completeFunc) {
    try {
        $.getJSON('https://api.db-ip.com/v2/free/self', function (data) {
            completeFunc(data);
        });
    }
    catch (e) {
        completeFunc();
    }
}

StiMobileDesigner.prototype.getNetworkData = function (completeFunc) {
    try {
        $.getJSON('https://api.db-ip.com/v2/free/self', function (data) {
            completeFunc(data);
        });
    }
    catch (e) {
        completeFunc();
    }
}

StiMobileDesigner.prototype.getUserImage = function (user, isSmall) {
    var img;
    if (user.Picture) {
        img = isSmall
            ? $("<img style='width:22px;height:22px;' src='data:image/jpeg;base64, " + user.Picture + "'/>")[0]
            : $("<img style='width:82px;height:82px;' src='data:image/jpeg;base64, " + user.Picture + "'/>")[0];
    }
    else {
        var bgColor = this.getUserImgColor(user.Key);
        var sign = this.getUserSign(user);
        img = isSmall
            ? $("<div style='line-height:1;width:22px;height:22px;text-align:center;color:white;font-family:Arial;font-size:12px;overflow:hidden;background-color:" +
                bgColor + "'><div style='margin-top: 6px;'>" + sign + "</div></div>")[0]
            : $("<div style='line-height:1;width:82px;height:82px;text-align:center;color:white;font-family:Arial;font-size:42px;overflow:hidden;background-color:" +
                bgColor + "'><div style='margin-top: 21px;'>" + sign + "</div></div>")[0];
    }
    return img;
}

StiMobileDesigner.prototype.getUserSign = function (user) {
    var sb = "";
    if (user.FirstName != null && user.FirstName != "") sb += user.FirstName.substring(0, 1);
    if (user.LastName != null && user.LastName != "") sb += user.LastName.substring(0, 1);
    if (sb.length == 0) sb = user.UserName.substring(0, 1);
    return sb;
}

StiMobileDesigner.prototype.getUserImgColor = function (key) {
    var r = 0;
    var g = 0;
    var b = 0;
    for (var i = 0; i < key.length / 3; i += 3) {
        r += key.charCodeAt(i);
        g += key.charCodeAt(i + 1);
        b += key.charCodeAt(i + 2);
    }
    r = r % 100 + 100;
    g = g % 100 + 100;
    b = b % 100 + 100;
    return this.RgbToHex(r, g, b);
}

StiMobileDesigner.prototype.convertToMB = function (value, short) {
    var v = this.round((value / 1024) / 1024, 3);
    return (short ? Math.round(v) : v);
}

StiMobileDesigner.prototype.round = function (a, b) {
    b = b || 0;
    return Math.round(a * Math.pow(10, b)) / Math.pow(10, b);
}