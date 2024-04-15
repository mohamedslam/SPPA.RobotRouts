
StiMobileDesigner.prototype.InitializeShareForm_ = function () {
    var jsObject = this;
    var shareForm = this.BaseForm("shareForm", this.loc.Cloud.ShareWindowTitleNew, 1, this.HelpLinks["share"]);
    shareForm.controls = {};
    shareForm.container.style.padding = "0px";
    shareForm.container.style.width = "580px";
    shareForm.buttonOk.caption.innerHTML = this.loc.Buttons.Save;
    shareForm.progress = this.AddProgressToControl(shareForm.container);
    this.AddSmallProgressMarkerToControl(shareForm.buttonOk);

    var embeddedImage = this.CheckBox(null, this.loc.Export.EmbeddedImageData);
    embeddedImage.style.margin = "12px";
    shareForm.buttonsPanel.style.width = "100%";
    shareForm.buttonsPanel.firstChild.style.width = "100%";
    shareForm.buttonsPanel.firstChild.tr[0].insertCell(0).appendChild(embeddedImage);
    shareForm.buttonOk.parentElement.style.width = "1px";
    shareForm.buttonCancel.parentElement.style.width = "1px";
    embeddedImage.style.display = "none";

    embeddedImage.action = function () {
        jsObject.SetCookie("StimulsoftShareFormEmbeddedImage", this.isChecked ? "True" : "False");
    }

    var controlsTable = this.CreateHTMLTable();
    shareForm.container.appendChild(controlsTable);
    controlsTable.style.width = "100%";

    shareForm.addControlRow = function (table, textControl, controlName, control, margin) {
        this.controls[controlName] = control;
        this.controls[controlName + "Row"] = table.addRow();

        if (textControl != null) {
            var text = table.addCellInLastRow();
            this.controls[controlName + "Text"] = text;
            text.innerHTML = textControl;
            text.className = "stiDesignerCaptionControls";
            text.style.paddingLeft = "15px";
            text.style.width = "1px";
        }

        if (control) {
            control.style.margin = margin;
            var controlCell = table.addCellInLastRow(control);
            if (textControl == null) controlCell.setAttribute("colspan", 2);
        }

        return controlCell;
    }

    var buttons = [];
    buttons.push(["sharePrivate", "Share.BigPrivateShare.png", "<b>" + this.loc.Cloud.WizardPrivateShare + "</b><br>" + this.loc.Cloud.WizardPrivateShareDescription]);
    buttons.push(["shareTeam", "Share.BigTeamShare.png", "<b>" + this.loc.Cloud.WizardTeamShare + "</b><br>" + this.loc.Cloud.WizardTeamShareDescription]);
    buttons.push(["sharePublic", "Share.BigPublicShare.png", "<b>" + this.loc.Cloud.WizardPublicShare + "</b><br>" + this.loc.Cloud.WizardPublicShareDescription]);

    var buttonsPanel = document.createElement("div");
    buttonsPanel.style.margin = "10px 0px 10px 0px";
    controlsTable.addCellInNextRow(buttonsPanel).colSpan = 2;

    var actionFunction = function () {
        if (!this.isEnabled) return;
        if (this.groupName != null) {
            this.setSelected(true);
        }
        shareForm.shareLevel = this.name.substr(5);
        shareForm.updateControlsStates();
    };

    for (var i = 0; i < buttons.length; i++) {
        var button = jsObject.FlatButton(buttons[i][0], buttons[i][2], buttons[i][1], "shareFormButtons");
        buttonsPanel.appendChild(button);
        buttonsPanel[buttons[i][0]] = button;
        button.action = actionFunction;
    }

    var sep1 = this.FormSeparator();
    sep1.style.marginBottom = "5px";
    controlsTable.addCellInNextRow(sep1).setAttribute("colspan", "2");

    var controlEndDate = this.DateControlWithCheckBox2(null, 250, null);
    shareForm.addControlRow(controlsTable, this.loc.Cloud.LabelEndDate.replace(":", ""), "endDate", controlEndDate, "5px 5px 5px 17px").parentNode.firstChild;
    shareForm.controlEndDate = controlEndDate;
    var date = new Date();
    date.setDate(date.getDate() + 1);
    controlEndDate.setKey(date);

    //Result Type
    var resultType = this.DropDownList(null, 266, null, this.GetExportFormatTypesItems(), true, true, false, null, { width: 16, height: 16 });
    resultType.image.style.width = "16px";
    var resultTypeValue = this.GetCookie("StimulsoftShareFormResultType") || "ReportSnapshot";
    resultType.setKey(resultTypeValue);
    shareForm.addControlRow(controlsTable, this.loc.Cloud.LabelResultType.replace(":", ""), "resultType", resultType, "5px 5px 5px 40px").parentNode.firstChild;

    resultType.action = function () {
        var shareUrl = shareForm.getShareLinkWithExport(shareForm.controls.linkControl.shareUrl);

        shareForm.controls.linkControl.value = shareForm.controls.buttonEmbedCode.isSelected
            ? "<iframe src='" + shareUrl +
            "' style='width:1200px; height:600px; overflow:hidden; border: 1px solid gray;'></iframe>"
            : shareUrl;

        jsObject.SendCloudCommand("", { ShareUrl: shareUrl },
            function (data) { if (data.ResultSuccess && data.QRCodeImage) shareForm.controls.qrCodeContainer.src = data.QRCodeImage; },
            function (data) { },
            null,
            jsObject.options.standaloneJsMode ? "service/qrcode/" : jsObject.options.cloudParameters.restUrl + "service/qrcode/" + jsObject.generateKey()
        );

        this.jsObject.SetCookie("StimulsoftShareFormResultType", this.key)
    }

    //Separator
    var sep2 = this.FormSeparator();
    sep2.style.marginTop = "5px";
    controlsTable.addCellInNextRow(sep2).setAttribute("colspan", "2");

    //Link & Embed code
    var linkToolbar = this.CreateHTMLTable();
    shareForm.addControlRow(controlsTable, " ", "linkToolbar", linkToolbar, "8px 4px 4px 35px");

    var buttons = ["Share", "EmbedCode", "QRCode", "Twitter", "Facebook"];
    for (var i = 0; i < buttons.length; i++) {
        var button = shareForm.controls["button" + buttons[i]] = this.FormButtonWithThemeBorder(null, null, this.loc.Cloud["TabItem" + buttons[i]] || buttons[i]);
        button.name = buttons[i];
        button.style.marginLeft = "4px";
        linkToolbar.addCell(button);

        button.action = function () {
            for (var i = 0; i < buttons.length; i++) {
                shareForm.controls["button" + buttons[i]].setSelected(false);
            }
            this.setSelected(true);

            var shareUrl = shareForm.controls.linkControl.shareUrl;
            var reportName = jsObject.options.cloudParameters.reportName;
            var linkValue = shareForm.getShareLinkWithExport(shareUrl);

            if (this.name == "EmbedCode") {
                linkValue = "<iframe src='" + shareForm.getShareLinkWithExport(shareUrl) + "' style='width:1200px; height:600px; overflow:hidden; border: 1px solid gray;'></iframe>"
            }
            else if (this.name == "Facebook") {
                linkValue = "https://www.facebook.com/sharer/sharer.php?u=" + shareUrl + "&via=stimulsoft&text=" + reportName;
            }
            else if (this.name == "Twitter") {
                linkValue = "https://twitter.com/intent/tweet?url=" + shareUrl;
            }

            shareForm.controls.linkControl.value = linkValue;

            if (shareForm.controls.copyBtn) shareForm.controls.copyBtn.style.display = this.name != "QRCode" ? "" : "none";
            shareForm.controls.refreshBtn.style.display = this.name == "Share" ? "" : "none";
            shareForm.controls.linkContainer.style.display = this.name != "QRCode" ? "" : "none";
            shareForm.controls.qrCodeContainer.style.display = this.name != "QRCode" ? "none" : "";
            shareForm.controls.externalLinkBtn.style.display = this.name != "QRCode" && this.name != "EmbedCode" ? "" : "none";

            button.jsObject.SetCookie("ShareFormDefaultMode", this.name);
        }
    }
    shareForm.controls.buttonShare.setSelected(true);

    var controlsBlock = document.createElement("div");
    controlsBlock.style.height = "110px";
    shareForm.addControlRow(controlsTable, " ", "controlsBlock", controlsBlock).style.padding = "4px 20px 0 40px";

    //Link
    var linkContainer = this.CreateHTMLTable();
    shareForm.controls.linkContainer = linkContainer;
    linkContainer.style.width = "100%";
    controlsBlock.appendChild(linkContainer);
    var linkControl = this.TextArea(null, null, 90);
    linkControl.style.border = "0px";
    linkControl.readOnly = true;
    linkControl.style.width = "100%";
    shareForm.controls.linkControl = linkControl;
    linkContainer.addCell(linkControl);
    linkContainer.className = "stiResourceContainerWithBorder";
    linkContainer.style.background = "white";

    var buttonsCell = linkContainer.addCell();
    buttonsCell.style.verticalAlign = "top";
    buttonsCell.style.padding = "0 0 0 5px";
    buttonsCell.style.width = "1px";

    var linkButtons = [];
    linkButtons.push(["copyBtn", "Copy.png", this.loc.MainMenu.menuEditCopy.replace("&", "")]);
    linkButtons.push(["refreshBtn", "Share.Refresh.png", this.loc.PropertyMain.Refresh]);
    linkButtons.push(["externalLinkBtn", "Share.ExternalLink.png", this.loc.Buttons.Open]);

    for (var i = 0; i < linkButtons.length; i++) {
        var button = this.FormButton(null, null, null, linkButtons[i][1], linkButtons[i][2]);
        buttonsCell.appendChild(button);
        shareForm.controls[linkButtons[i][0]] = button;
        button.style.minWidth = "";
        button.style.margin = "3px 3px 0 0";
        button.style.width = "22px";
        button.style.height = "21px";
        button.imageCell.style.padding = "0";
    }

    if (shareForm.controls.copyBtn) {
        shareForm.controls.copyBtn.action = function () {
            this.jsObject.copyTextToClipboard(shareForm.controls.linkControl.value);
        };
    }

    shareForm.controls.refreshBtn.action = function () {
        if (shareForm.itemKey && shareForm.sessionKey) {
            shareForm.progress.show();
            jsObject.SendCloudCommand("ItemRefreshShareUrl", { ItemKey: shareForm.itemKey, SessionKey: shareForm.sessionKey },
                function (data) {
                    if (data.ResultSuccess && data.ResultUrl) {
                        shareForm.progress.hide();
                        shareForm.controls.linkControl.value = shareForm.getShareLinkWithExport(data.ResultUrl);
                        shareForm.controls.linkControl.shareUrl = data.ResultUrl;

                        jsObject.SendCloudCommand("", { ShareUrl: shareForm.getShareLinkWithExport(data.ResultUrl) },
                            function (data) { if (data.ResultSuccess && data.QRCodeImage) shareForm.controls.qrCodeContainer.src = data.QRCodeImage; },
                            function (data) { },
                            null,
                            jsObject.options.standaloneJsMode ? "service/qrcode/" : jsObject.options.cloudParameters.restUrl + "service/qrcode/" + jsObject.generateKey()
                        );

                        var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                        errorMessageForm.show(jsObject.loc.Messages.ShareURLOfTheItemHasBeenUpdated, "Info");
                    }
                },
                function (data, msg) {
                    shareForm.progress.hide();
                    if (msg || data) {
                        var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                        errorMessageForm.show(msg || jsObject.formatResultMsg(data));
                    }
                });
        }
    };

    shareForm.controls.externalLinkBtn.action = function () {
        var shareUrl = shareForm.controls.linkControl.value;

        if (shareForm.startsFromPrivateShare) {
            var msg = jsObject.IsRusCulture(jsObject.options.cultureName) ? "Сохраните доступ к отчету, перед тем как открыть ссылку." : "Save access to the report before opening the link.";
            var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
            errorMessageForm.show(msg, "Info");
        }
        else {
            jsObject.openNewWindow(shareUrl);
        }
    }

    //QrCode
    var qrCodeContainer = document.createElement("img");
    shareForm.controls.qrCodeContainer = qrCodeContainer;
    qrCodeContainer.style.display = "none";
    controlsBlock.appendChild(qrCodeContainer);

    //Warning compilation mode
    var warningSep = this.FormSeparator();
    shareForm.container.appendChild(warningSep);

    var warningText = this.CreateHTMLTable();
    warningText.className = "stiDesignerTextContainer";
    var img = document.createElement("img");
    img.style.margin = "10px 10px 10px 20px";
    img.style.width = img.style.height = "32px";
    StiMobileDesigner.setImageSource(img, this.options, "ReportChecker.Warning32.png");
    warningText.addCell(img).style.width = "1px";
    warningText.addTextCell(this.loc.Messages.RenderingWillOccurInTheInterpretationMode).style.padding = "10px";
    shareForm.container.appendChild(warningText);

    shareForm.updateShareLevel = function () {
        if (this.shareLevel == "Private") buttonsPanel["sharePrivate"].action();
        else if (this.shareLevel == "Team") buttonsPanel["shareTeam"].action();
        else if (this.shareLevel == "Public") buttonsPanel["sharePublic"].action();
    }

    shareForm.updateControlsStates = function () {
        controlEndDate.setEnabled(shareForm.shareLevel != "Private");
        resultType.setEnabled(shareForm.shareLevel != "Private");
        shareForm.controls.copyBtn.setEnabled(shareForm.shareLevel != "Private");
        shareForm.controls.refreshBtn.setEnabled(shareForm.shareLevel != "Private");
        shareForm.controls.externalLinkBtn.setEnabled(shareForm.shareLevel != "Private");
        shareForm.controls.buttonShare.setEnabled(shareForm.shareLevel != "Private");
        shareForm.controls.buttonEmbedCode.setEnabled(shareForm.shareLevel != "Private");
        shareForm.controls.buttonQRCode.setEnabled(shareForm.shareLevel != "Private");
        shareForm.controls.buttonTwitter.setEnabled(shareForm.shareLevel != "Private");
        shareForm.controls.buttonFacebook.setEnabled(shareForm.shareLevel != "Private");
        linkControl.setEnabled(shareForm.shareLevel != "Private");
    }

    shareForm.getShareLinkWithExport = function (shareLink) {
        if (resultType.key == "ReportSnapshot")
            return shareLink;

        return shareLink + "/" + resultType.key.toLowerCase();
    }

    shareForm.fillShareInfo = function () {
        this.itemKey = jsObject.options.cloudParameters ? jsObject.options.cloudParameters.reportTemplateItemKey : null;

        if (this.itemKey) {
            shareForm.progress.show();
            jsObject.SendCloudCommand("ItemGetShareInfo", { ItemKey: this.itemKey },
                function (data) {
                    shareForm.progress.hide();
                    shareForm.controls.linkControl.shareUrl = data.ResultUrl;
                    shareForm.controls.linkControl.value = shareForm.getShareLinkWithExport(data.ResultUrl);

                    jsObject.SendCloudCommand("", { ShareUrl: shareForm.getShareLinkWithExport(data.ResultUrl) },
                        function (data) { if (data.ResultSuccess && data.QRCodeImage) shareForm.controls.qrCodeContainer.src = data.QRCodeImage; },
                        function (data) { },
                        null,
                        jsObject.options.standaloneJsMode ? "service/qrcode/" : jsObject.options.cloudParameters.restUrl + "service/qrcode/" + jsObject.generateKey()
                    );

                    shareForm.shareLevels.push(data.ResultShareLevel);
                    var shareLevel = shareForm.shareLevels[0];

                    for (var i = 0; i < shareForm.shareLevels.length; i++) {
                        if (shareForm.shareLevels[i] != shareLevel) shareLevel = false;
                    }

                    if (shareLevel) {
                        shareForm.shareLevel = shareLevel;
                        shareForm.startsFromPrivateShare = shareLevel == "Private";
                        shareForm.updateShareLevel();
                    }

                    if (data.ResultShareExpires) {
                        controlEndDate.setKey(shareForm.jsObject.JSONDateFormatToDate(data.ResultShareExpires));
                        controlEndDate.setChecked(true);
                    } else {
                        controlEndDate.setChecked(false);
                    }
                },
                function (data, msg) {
                    shareForm.progress.hide();
                    if (msg || data) {
                        var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
                        errorMessageForm.show(msg || jsObject.formatResultMsg(data));
                    }
                });
        }
    }

    shareForm.checkReportSavingToCloud = function () {
        if (jsObject.options.cloudParameters && jsObject.options.cloudParameters.reportTemplateItemKey) {
            shareForm.fillShareInfo();
        }
        else {
            var messageForm = jsObject.MessageFormForSaveReportToCloud();
            messageForm.changeVisibleState(true);
            messageForm.action = function (state) {
                if (state) {
                    var maxFileSize = jsObject.GetCurrentPlanLimitValue("MaxFileSize");
                    var fileSize = jsObject.options.report ? jsObject.options.report.fileSize : null;
                    if (fileSize && maxFileSize && fileSize > maxFileSize) {
                        jsObject.InitializeNotificationForm(function (form) {
                            form.show(
                                jsObject.loc.Notices.QuotaMaximumFileSizeExceeded + "<br>" + jsObject.loc.PropertyMain.Maximum + ": " + jsObject.GetHumanFileSize(maxFileSize, true),
                                jsObject.NotificationMessages("upgradeYourPlan"),
                                "Notifications.Elements.png"
                            );
                        });
                        return;
                    }
                   
                    var saveAsForm = jsObject.InitializeOnlineSaveAsForm();
                    saveAsForm.show();

                    saveAsForm.cancelAction = function () {
                        shareForm.changeVisibleState(false);
                    }
                }
                else {
                    shareForm.changeVisibleState(false);
                }
            }
            messageForm.cancelAction = function () {
                shareForm.changeVisibleState(false);
            }
        }
    }

    shareForm.show = function () {
        this.changeVisibleState(true);
        shareForm.buttonOk.progressMarker.changeVisibleState(false);
        shareForm.shareLevels = [];
        shareForm.itemKey = null;
        shareForm.sessionKey = jsObject.options.SessionKey || (jsObject.options.cloudParameters && jsObject.options.cloudParameters.sessionKey);
        warningText.style.display = warningSep.style.display = jsObject.options.report && jsObject.options.report.properties.calculationMode == "Compilation" ? "" : "none";
        embeddedImage.setChecked(jsObject.GetCookie("StimulsoftShareFormEmbeddedImage") == "True");

        if (shareForm.sessionKey) {
            shareForm.checkReportSavingToCloud();
        }
        else {
            jsObject.options.forms.authForm.show();
        }
    }

    shareForm.action = function () {
        if (this.itemKey) {
            var params = {
                ItemKeys: [],
                ShareLevel: this.shareLevel,
                AllowSignalsReturn: true
            }
            params.ItemKeys.push(this.itemKey);
            shareForm.buttonOk.progressMarker.changeVisibleState(true);

            if (controlEndDate.isChecked && shareForm.shareLevel != "Private") {
                params.ShareExpires = jsObject.DateToJSONDateFormat(controlEndDate.key);
            }

            var isTwitter = shareForm.shareLevel != "Private" && shareForm.controls.buttonTwitter.isSelected;
            var isFacebook = shareForm.shareLevel != "Private" && shareForm.controls.buttonFacebook.isSelected;

            if (embeddedImage.isChecked) {
                if (jsObject.options.jsMode)
                    jsObject.options.controller.exec("service/savereportthumbnail", { ReportItemKey: this.itemKey, SessionKey: shareForm.sessionKey }, function (data) { });
                else
                    jsObject.SendCommandToDesignerServer("SaveReportThumbnail", { sessionKey: shareForm.sessionKey, reportItemKey: this.itemKey }, function () { });
            }

            jsObject.SendCloudCommand("ItemSetShareInfo", params,
                function (data) {
                    shareForm.buttonOk.progressMarker.changeVisibleState(false);
                    shareForm.changeVisibleState(false);

                    if (isFacebook || isTwitter) {
                        jsObject.openNewWindow(shareForm.controls.linkControl.value);
                    }
                },
                function (data) {
                    shareForm.buttonOk.progressMarker.changeVisibleState(false);
                    shareForm.changeVisibleState(false);
                });
        }
        else {
            shareForm.changeVisibleState(false);
        }
    }

    return shareForm;
}

StiMobileDesigner.prototype.FlatButton = function (name, caption, imageName, groupName) {
    var button = this.SmallButton(name, groupName, caption, imageName, null, null, null, null, { width: 32, height: 32 }, true);
    button.style.clear = "both";
    button.style.minHeight = "65px";
    button.innerTable.style.height = "65px";
    button.imageCell.style.padding = "0px 0px 0px 135px";
    button.caption.style.padding = "0px 10px 0px 10px";
    button.caption.style.whiteSpace = "normal";
    button.caption.style.lineHeight = "1.5";

    return button;
}