
StiMobileDesigner.prototype.InitializeWhoAreYouForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("whoAreYouForm", this.loc.FormDesigner.title, 4);
    form.buttonOk.style.display = "none";
    form.buttonCancel.caption.innerHTML = this.loc.Cloud.ButtonLater;
    form.buttons = {};

    var header = document.createElement("div");
    header.className = "stiDesignerBigText";
    header.style.padding = "20px 20px 10px 20px";
    header.innerHTML = this.loc.Desktop.ChooseYourSkillLevel;
    form.container.appendChild(header);

    var mainTable = this.CreateHTMLTable();
    mainTable.style.margin = "10px";
    form.container.appendChild(mainTable);

    var buttonsParams = [
        ["Beginner", this.loc.Report.Basic, "WhoAreYou.Beginner.png", this.loc.Desktop.WhoAreYouBeginnerDescription],
        ["BICreator", this.loc.Report.Standard, "WhoAreYou.Creator.png", this.loc.Desktop.WhoAreYouCreatorDescription],
        ["Developer", this.loc.Report.Professional, "WhoAreYou.Developer.png", this.loc.Desktop.WhoAreYouDeveloperDescription]
    ]

    for (var i = 0; i < buttonsParams.length; i++) {
        var button = jsObject.WhoAreYouFormButton(buttonsParams[i][1], buttonsParams[i][2], buttonsParams[i][3], buttonsParams[i][0]);
        mainTable.addCell(button).style.padding = "10px";
        form.buttons[buttonsParams[i][0]] = button;

        button.action = function () {
            var profilePanel = jsObject.options.profilePanel;

            if (profilePanel && profilePanel.visible) {
                var specificButton = profilePanel.controls.specificButton;
                specificButton.caption.innerHTML = jsObject.DesignerSpecificationToSkillLevelLoc(this.key);
                profilePanel.startData.DesignerSpecification = profilePanel.data.DesignerSpecification = jsObject.options.designerSpecification = this.key;
                profilePanel.progress.show();

                jsObject.SendCloudCommand("UserSave", { User: profilePanel.startData }, function (data) {
                    profilePanel.progress.hide();
                });
            }
            else {
                jsObject.ApplyDesignerSpecification(this.key);
            }

            form.changeVisibleState(false);
        }
    }

    form.show = function () {
        if (jsObject.options.designerIsBlocked) return;
        form.changeVisibleState(true);
        for (var i = 0; i < buttonsParams.length; i++) {
            form.buttons[buttonsParams[i][0]].setSelected(false);
        }
        if (form.buttons[jsObject.options.designerSpecification]) {
            form.buttons[jsObject.options.designerSpecification].setSelected(true);
        }
    }

    return form;
}

StiMobileDesigner.prototype.WhoAreYouFormButton = function (caption, image, description, key) {
    var button = this.BigButton(null, null, description, image, null, null, "stiDesignerStandartBigButton", true, null, { width: 64, height: 64 });
    button.style.width = "220px";
    button.caption.style.padding = "30px 10px 15px 10px";
    button.caption.style.fontSize = "13px";
    button.caption.style.height = "100px";
    button.caption.style.lineHeight = "1.3";
    button.caption.style.verticalAlign = "top";
    button.caption.style.textAlign = "left";
    button.cellImage.style.height = "80px";
    button.cellImage.style.padding = "0";
    button.key = key;

    var rowsParent = button.innerTable.rows[0].parentElement;
    var captionRow = document.createElement("tr");
    rowsParent.insertBefore(captionRow, button.innerTable.rows[0]);
    var captionCell = document.createElement("td");
    captionRow.appendChild(captionCell);
    captionCell.innerHTML = caption;
    captionCell.className = "stiDesignerBigText";
    captionCell.style.height = "80px";

    var barsRow = document.createElement("tr");
    rowsParent.insertBefore(barsRow, button.innerTable.rows[2]);
    var barsCell = document.createElement("td");
    barsRow.appendChild(barsCell);
    barsCell.appendChild(this.SkillLevelBarsPanel(key));
    barsCell.style.textAlign = "center";

    return button;
}

StiMobileDesigner.prototype.SkillLevelBarsPanel = function (key) {
    var panel = this.CreateHTMLTable();
    panel.style.display = "inline-block";
    for (var i = 0; i < 5; i++) {
        var bar = document.createElement("div");
        var barColor = ((key == "Beginner" && i < 1) || (key == "BICreator" && i < 3) || key == "Developer") ? "#888888" : "#ffffff";
        bar.setAttribute("style", "border: 2px solid #888888; width: 9px; height: 9px; margin: 3px; background:" + barColor);
        panel.addCell(bar);
    }
    return panel;
}