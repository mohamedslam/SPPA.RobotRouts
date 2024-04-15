
StiJsViewer.prototype.InitializeNotificationForm = function () {
    var jsObject = this;
    var form = this.BaseForm("notificationForm", this.collections.loc.Viewer, 4);
    form.buttonsPanel.style.display = "none";

    var table = this.CreateHTMLTable();
    form.container.appendChild(table);

    var image = document.createElement("img");
    image.style.marginTop = "20px";
    table.addCell(image).style.textAlign = "center";

    var messageBlock = document.createElement("div");
    messageBlock.className = "stiJsViewerNotificationFormMessage";
    table.addCellInNextRow(messageBlock);

    var descriptionBlock = document.createElement("div");
    descriptionBlock.className = "stiJsViewerNotificationFormDescription";
    table.addCellInNextRow(descriptionBlock);

    var upgradeButton = this.LoginButton(null, this.collections.loc["Purchase"], null);
    upgradeButton.style.display = "inline-block";
    upgradeButton.innerTable.style.width = "100%";
    upgradeButton.style.minWidth = "200px";
    upgradeButton.style.width = "auto";
    upgradeButton.style.margin = "20px 30px 30px 30px";
    upgradeButton.style.fontSize = "14px";
    form.upgradeButton = upgradeButton;
    table.addCellInNextRow(upgradeButton).style.textAlign = "center";

    form.show = function (message, description, imageName, imageSizes) {
        messageBlock.style.display = message ? "" : "none";
        messageBlock.innerHTML = message;
        descriptionBlock.style.display = description ? "" : "none";
        descriptionBlock.innerHTML = description;
        image.style.display = imageName ? "" : "none";

        if (imageName) {
            image.style.width = (imageSizes ? imageSizes.width : 112) + "px";
            image.style.height = (imageSizes ? imageSizes.height : 112) + "px";
            StiJsViewer.setImageSource(image, jsObject.options, jsObject.collections, imageName);
        }

        upgradeButton.caption.innerHTML = jsObject.collections.loc["Purchase"];

        upgradeButton.action = function () {
            jsObject.openNewWindow("https://www.stimulsoft.com/en/online-store#cloud/cloud", undefined, undefined, false);
            form.changeVisibleState(false);
        }

        form.cancelAction = function () { }

        this.changeVisibleState(true);
    }

    return form;
}