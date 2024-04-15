
StiMobileDesigner.prototype.InitializeNotificationForm_ = function (name) {
    var jsObject = this;
    var form = this.BaseForm(name || "notificationForm", this.loc.FormDesigner.title, 4);
    form.hideButtonsPanel();

    var table = this.CreateHTMLTable();
    form.table = table;
    form.container.appendChild(table);

    var image = document.createElement("img");
    form.image = image;
    image.style.marginTop = "20px";
    table.addCell(image).style.textAlign = "center";

    var messageBlock = document.createElement("div");
    form.messageBlock = messageBlock;
    messageBlock.className = "stiDesignerNotificationFormMessage";
    table.addCellInNextRow(messageBlock);

    var descriptionBlock = document.createElement("div");
    form.descriptionBlock = descriptionBlock;
    descriptionBlock.className = "stiDesignerNotificationFormDescription";
    table.addCellInNextRow(descriptionBlock);

    var upgradeButton = this.NotificationFormActiveButton(this.loc.Cloud.ButtonPurchase);
    form.upgradeButton = upgradeButton;
    upgradeButton.style.display = "inline-block";
    upgradeButton.style.margin = "20px 30px 30px 30px";
    table.addCellInNextRow(upgradeButton).style.textAlign = "center";

    var downTextBlock = document.createElement("div");
    form.downTextBlock = downTextBlock;
    downTextBlock.className = "stiDesignerNotificationFormDescription";
    downTextBlock.style.fontSize = "12px";
    downTextBlock.style.padding = "0px 25px 20px 25px";

    table.addCellInNextRow(downTextBlock);

    form.show = function (message, description, imageName, imageSizes, downText) {
        messageBlock.style.display = message ? "" : "none";
        messageBlock.innerHTML = message;
        descriptionBlock.style.display = description ? "" : "none";
        descriptionBlock.innerHTML = description;
        image.style.display = imageName ? "" : "none";
        downTextBlock.style.display = downText ? "" : "none";
        downTextBlock.innerHTML = downText || "";

        if (imageName) {
            image.style.width = (imageSizes ? imageSizes.width : 112) + "px";
            image.style.height = (imageSizes ? imageSizes.height : 112) + "px";
            StiMobileDesigner.setImageSource(image, jsObject.options, imageName);
        }

        upgradeButton.caption.innerHTML = jsObject.loc.Cloud.ButtonPurchase;

        upgradeButton.action = function () {
            jsObject.openNewWindow("https://www.stimulsoft.com/en/online-store#cloud/cloud");
            form.changeVisibleState(false);
        }

        form.cancelAction = function () { }

        this.changeVisibleState(true);
    }

    return form;
}

StiMobileDesigner.prototype.InitializeNotificationCheckActivatedForm_ = function () {
    var jsObject = this;
    var form = this.InitializeNotificationForm_("notificationCheckActivatedForm");
    form.upgradeButton.style.display = "none";
    var buttonsTable = this.CreateHTMLTable();
    buttonsTable.style.margin = "20px 30px 30px 30px";
    buttonsTable.style.display = "inline-block";
    form.upgradeButton.parentElement.appendChild(buttonsTable);

    var resendButton = this.NotificationFormActiveButton(this.loc.Cloud.ButtonResendEmail);
    resendButton.style.marginRight = "20px";
    buttonsTable.addCell(resendButton).style.verticalAlign = "top";

    resendButton.action = function () {
        jsObject.SendCloudCommand("UserActivate", { UserName: form.userName, ResultSuccess: true }, function () { });
        resendButton.setEnabled(false);
    }

    var cancelButton = this.FormButton(null, null, this.loc.PropertyEnum.DialogResultCancel);
    cancelButton.style.display = "inline-block";
    cancelButton.style.minWidth = "200px";
    cancelButton.style.height = "30px";
    buttonsTable.addCell(cancelButton).style.verticalAlign = "top";

    cancelButton.action = function () {
        form.changeVisibleState(false);
    }

    form.show = function (userName) {
        resendButton.setEnabled(true);
        form.userName = userName;
        form.messageBlock.style.display = "";
        form.messageBlock.innerHTML = jsObject.loc.Notices.AuthAccountIsNotActivated;
        form.descriptionBlock.style.display = "none";
        form.image.style.width = form.image.style.height = "112px";
        StiMobileDesigner.setImageSource(form.image, jsObject.options, "Notifications.CheckEmail.png");
        this.changeVisibleState(true);
    }

    form.cancelAction = function () {
        if (jsObject.options.standaloneJsMode) {
            var userKey = jsObject.options.UserKey;
            if (userKey) {
                jsObject.SendCloudCommand("UserGet", { UserKey: userKey },
                    function (data) {
                        jsObject.options.user = data.ResultUser;
                        jsObject.options.UserName = data.ResultUser.UserName;
                    });
            }
        }
        else {
            var cloudParameters = jsObject.options.cloudParameters;
            if (cloudParameters) {
                jsObject.SendCloudCommand("UserGet", { UserKey: jsObject.options.cloudParameters.userKey },
                    function (data) {
                        cloudParameters.user = data.ResultUser;
                        cloudParameters.userName = data.ResultUser.UserName;
                    });
            }
        }
    }

    return form;
}

StiMobileDesigner.prototype.InitializeNotificationCheckTrDaysForm_ = function () {
    var jsObject = this;
    var form = this.InitializeNotificationForm_("notificationCheckTrDaysForm");
    form.upgradeButton.style.display = "none";
    var buttonsTable = this.CreateHTMLTable();
    buttonsTable.style.margin = "20px 30px 30px 30px";
    buttonsTable.style.display = "inline-block";
    form.upgradeButton.parentElement.appendChild(buttonsTable);

    var imageCell = form.image.parentElement;
    imageCell.style.position = "relative";
    var countBlock = document.createElement("div");
    countBlock.className = "stiDesignerNotificationFormCountBlock";
    imageCell.appendChild(countBlock);

    var continueButton = this.FormButton(null, null, this.loc.DesignerFx.Continue);
    continueButton.style.minWidth = "150px";
    continueButton.style.height = "30px";
    continueButton.style.marginRight = "20px";
    buttonsTable.addCell(continueButton).style.verticalAlign = "top";

    continueButton.action = function () {
        form.changeVisibleState(false);
    }

    var purchaseButton = this.NotificationFormActiveButton(this.loc.Cloud.ButtonPurchase);
    purchaseButton.style.minWidth = "150px";
    purchaseButton.style.display = "inline-block";
    buttonsTable.addCell(purchaseButton).style.verticalAlign = "top";

    purchaseButton.action = function () {
        jsObject.openNewWindow("https://www.stimulsoft.com/en/online-store#cloud/cloud");
        form.changeVisibleState(false);
    }

    form.show = function (days) {
        var isRus = jsObject.IsRusCulture(jsObject.options.cultureName);
        form.messageBlock.style.display = "";
        form.messageBlock.innerHTML = jsObject.loc.Notices.YourTrialWillExpire.replace("{0}", days);
        form.descriptionBlock.style.display = "";
        form.descriptionBlock.innerHTML = jsObject.loc.Notices.ActivationTrialExpired;
        form.image.style.height = "112px";
        form.image.style.width = isRus ? "239px" : "197px";
        StiMobileDesigner.setImageSource(form.image, jsObject.options, "Notifications." + (isRus ? "TrialDaysLeft_Ru.png" : "TrialDaysLeft_En.png"));
        countBlock.innerHTML = days;
        countBlock.style.left = isRus ? "calc(50% - 92px)" : "calc(50% - 72px)";
        this.changeVisibleState(true);
    }

    return form;
}

StiMobileDesigner.prototype.NotificationFormActiveButton = function (caption) {
    var button = (this.options.cloudMode || this.options.standaloneJsMode) ? this.LoginButton(null, caption) : this.FormButton(null, null, caption, null, null, null, null, "stiDesignerFormButtonTheme");
    button.style.display = "inline-block";
    button.innerTable.style.width = "100%";
    button.style.minWidth = "200px";
    button.style.width = "auto";
    button.style.height = (this.options.cloudMode || this.options.standaloneJsMode) ? "32px" : "30px";
    button.style.fontSize = "14px";

    return button;
}