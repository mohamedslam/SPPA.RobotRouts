
StiMobileDesigner.prototype.InitializeModificationsForm = function () {
    var form = this.BaseForm("modificationsForm", this.loc.Report.WhatsNewInVersion, 4, null, true);
    form.buttonCancel.style.display = "none";
    form.container.style.padding = "0px";
    form.container.style.width = "600px";
    form.productsButtons = {};
    form.modifications = null;
    var jsObject = this;

    this.AddProgressToControl(form);

    var header = document.createElement("div");
    header.className = "stiUpdateFormItemHeader";
    form.container.appendChild(header);

    var date = document.createElement("div");
    date.className = "stiUpdateFormItemDate";
    form.container.appendChild(date);

    var description = document.createElement("div");
    description.style.marginBottom = "15px";
    description.className = "stiUpdateFormItemDescription";
    form.container.appendChild(description);

    var containerModifications = this.EasyContainer(600, 400);
    containerModifications.className = "stiModificationsFormContainer";
    form.container.appendChild(containerModifications);

    containerModifications.addHeader = function (text) {
        var header = document.createElement("div");
        header.className = "stiModificationsFormContainerHeaderItem";
        header.innerHTML = text;
        containerModifications.appendChild(header);
    }

    containerModifications.addItem = function (text, color) {
        var item = document.createElement("div");
        item.className = "stiModificationsFormContainerItem";
        item.style.borderLeft = "2px solid " + color;
        item.innerHTML = text;
        containerModifications.appendChild(item);
    }

    var productsPanel = document.createElement("div");
    form.container.appendChild(productsPanel);
    productsPanel.style.display = "none";
    productsPanel.style.height = "50px";
    productsPanel.className = "stiModificationsFormContainer";
    var productsTable = this.CreateHTMLTable();
    productsTable.style.margin = "8px 0 0 6px";
    productsPanel.appendChild(productsTable);

    var products = ["Ultimate", "Web", "Net", "Wpf", "Js", "Php", "Java", "Flex", "Server"];

    productsTable.addTextCell(this.loc.Cloud.Products + ":").className = "stiDesignerCaptionControls";

    for (var i = 0; i < products.length; i++) {
        var button = this.ModificationsFormProductsButton(products[i], "Update.Products." + products[i] + ".png");
        button.style.marginRight = "6px";
        productsTable.addCell(button);
        form.productsButtons[products[i]] = button;
        button.setSelected(true);

        button.action = function () {
            this.setSelected(!this.isSelected);
            if (this.productName == "Ultimate") {
                for (var i = 1; i < products.length; i++) {
                    form.productsButtons[products[i]].setSelected(this.isSelected);
                }
            }
            else {
                var selectUltimate = true;
                for (var i = 1; i < products.length; i++) {
                    if (!form.productsButtons[products[i]].isSelected) selectUltimate = false;
                }
                form.productsButtons["Ultimate"].setSelected(selectUltimate);
            }
            form.buildModificationsList();
        }
    }

    productsPanel.getChoosedProducts = function () {
        var choosedProducts = [];
        for (var i = 0; i < products.length; i++) {
            if (form.productsButtons[products[i]] && form.productsButtons[products[i]].isSelected) {
                choosedProducts.push(products[i]);
            }
        }
        return choosedProducts;
    }

    form.show = function (developerBuild) {
        this.changeVisibleState(true);
        form.caption.innerHTML = jsObject.loc.Report.WhatsNewInVersion.replace("{0}", developerBuild.Version);
        form.progress.show();
        header.innerHTML = "Stimulsoft Designer " + developerBuild.Version;
        date.innerHTML = jsObject.JSONDateFormatToDate(developerBuild.Date, "dd.MM.yyyy");
        description.innerHTML = developerBuild.Description;

        var url = "https://www.stimulsoft.com/en/changes/ultimate?action=json&build=" + developerBuild.Version;

        $.getJSON(url, function (data) {
            form.progress.hide();
            if (data) {
                form.modifications = data;
                form.buildModificationsList();
            }
        });
    }

    form.buildModificationsList = function () {
        containerModifications.clear();
        if (!form.modifications) return;
        var modifyTypes = {};

        for (var i = 0; i < form.modifications.length; i++) {
            var modification = form.modifications[i];
            if (!modifyTypes[modification.Type]) modifyTypes[modification.Type] = [];
            modifyTypes[modification.Type].push(modification.Description);
        }

        if (form.modifications.length == 0) {
            var emptyItem = jsObject.CreateHTMLTable();
            emptyItem.className = "stiUpdateFormEmptyItem";
            emptyItem.addTextCell(jsObject.loc.Report.NoFixes).style.textAlign = "center";
            emptyItem.style.width = "100%";
            emptyItem.style.height = "100%";
            containerModifications.appendChild(emptyItem);
            return;
        }

        var allTypes = [
            ["1", jsObject.loc.Report.NewFeatures, "#fe8f00"],
            ["2", jsObject.loc.Report.FixedBugs, "#5ca5ff"],
            ["3", jsObject.loc.Report.Enhancements, "#b9b9b9"]
        ];

        for (var i = 0; i < allTypes.length; i++) {
            var items = modifyTypes[allTypes[i][0]];
            if (items && items.length > 0) {
                containerModifications.addHeader(allTypes[i][1]);
                for (var k = 0; k < items.length; k++) {
                    containerModifications.addItem(items[k], allTypes[i][2]);
                }
            }
        }
    }

    form.action = function () {
        this.changeVisibleState(false);
    }

    return form;
}

StiMobileDesigner.prototype.ModificationsFormProductsButton = function (productName, imageName) {
    var button = this.StandartSmallButton(null, null, null, imageName);
    button.style.width = button.style.height = "32px";
    button.image.style.width = button.image.style.height = "25px";
    button.image.style.background = "#2980b9";
    button.imageCell.style.padding = "0";
    button.innerTable.style.width = "100%";
    button.productName = productName;

    var checkImage = document.createElement("img");
    checkImage.style.position = "absolute";
    checkImage.style.display = "none";
    checkImage.style.margin = "16px 0 0 -10px";
    StiMobileDesigner.setImageSource(checkImage, this.options, "Update.CircleCheckGreen.png");
    button.imageCell.appendChild(checkImage);

    button.setSelected = function (state) {
        this.isSelected = state;
        checkImage.style.display = state ? "" : "none";
    }

    button.onmouseenter = function () {
        if (!this.isEnabled || this.jsObject.options.isTouchClick) return;
        this.className = this.overClass;
        this.isOver = true;
    }

    button.onmouseleave = function () {
        this.isOver = false;
        if (!this.isEnabled) return;
        this.className = this.defaultClass;
    }

    return button;
}