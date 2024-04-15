
StiMobileDesigner.prototype.InitializeFileMenuSubscriptionsPanel = function (parentContainer) {
    var jsObject = this;
    var panel = document.createElement("div");
    panel.className = "stiDesignerAccountChildPanel";
    panel.controls = {};
    this.options.subscriptionsPanel = panel;

    jsObject.AddProgressToControl(panel);

    var footerPanel = document.createElement("div");
    footerPanel.className = "stiDesignerAccountFooterPanel";
    panel.appendChild(footerPanel);

    var refreshButton = this.FormButton(null, null, this.loc.PropertyMain.Refresh, null);
    refreshButton.style.display = "inline-block";
    refreshButton.style.margin = "12px";
    footerPanel.appendChild(refreshButton);
    panel.controls.refreshButton = refreshButton;

    refreshButton.action = function () {
        if (jsObject.options.standaloneJsMode) {
            if (jsObject.options.UserName && jsObject.options.SessionKey) {
                panel.progress.show();
                jsObject.LicenseActivate(function () {
                    panel.progress.hide();
                    panel.buildProducts();
                });
            }
        }
        else if (jsObject.options.cloudParameters && jsObject.options.cloudParameters.user && jsObject.options.cloudParameters.sessionKey) {
            var user = jsObject.options.cloudParameters.user;
            var owner = user ? (user.FirstName + " " + user.LastName) : "";
            var userName = user ? user.UserName : "";
            var params = {
                UserName: jsObject.options.cloudParameters.user.UserName,
                Type: "Developer",
                Format: "Base64",
                ResultSvr: false,
                Version: jsObject.options.shortProductVersion
            };
            panel.progress.show();
            jsObject.SendCloudCommand("LicenseActivate", params,
                function (data) {
                    panel.progress.hide();
                    if (data.ResultLicenseKey) {
                        panel.addProductsToContainer(data.ResultLicenseKey.Products, owner, userName);
                    }
                },
                function (data, msg) {
                    panel.showError(data, msg);
                });
        }
    }

    var infoHeader = this.FormBlockHeader(this.loc.Cloud.LicenseInformation);
    panel.appendChild(infoHeader);

    var fields = [
        ["owner", this.loc.Cloud.TextOwner],
        ["userName", this.loc.Cloud.labelUserName.replace(":", "")]
    ]

    var infoTable = this.CreateHTMLTable();
    infoTable.style.margin = "7px 0 7px 0";

    panel.appendChild(infoTable);
    for (var i = 0; i < fields.length; i++) {
        var labelCell = infoTable.addTextCellInLastRow(fields[i][1]);
        labelCell.className = "stiDesignerCaptionControls";
        labelCell.style.padding = "7px 25px 7px 14px";

        var cell = infoTable.addCellInLastRow();
        cell.className = "stiDesignerCaptionControls";
        panel.controls[fields[i][0] + "Cell"] = cell;
        infoTable.addRow();
    }

    var productsHeader = this.FormBlockHeader(this.loc.Cloud.Products);
    panel.appendChild(productsHeader);

    var productsContainer = this.EasyContainer(550);
    productsContainer.style.padding = "4px 4px 0 4px";
    productsContainer.style.height = "calc(100% - 171px)";
    panel.appendChild(productsContainer);

    productsContainer.addProduct = function (productName, licenseText, isTrial) {
        var text = "<font style='font-size: 15px'>" + jsObject.GetProductFullName(productName) + "</font><br>" + licenseText;
        productsContainer.addItem(productName, text, "Products." + productName + ".png", isTrial);
    }

    productsContainer.addItem = function (productName, caption, imageName, isTrial) {
        var item = jsObject.StandartSmallButton(null, null, caption, StiMobileDesigner.checkImageSource(jsObject.options, imageName) ? imageName : "Products.Ultimate.png", null, null, null, null, true);

        item.style.height = "52px";
        item.style.marginBottom = "4px";
        item.style.lineHeight = "1.6 ";
        item.productName = productName;

        var imageBox = document.createElement("div");
        imageBox.style.margin = "0 6px 0 4px";
        item.imageCell.appendChild(imageBox);
        item.imageCell.style.padding = "0";
        imageBox.appendChild(item.image);

        item.image.style.width = "32px";
        item.image.style.height = "32px";
        item.image.style.margin = "6px";

        item.innerTable.addCell().style.width = "100%";

        var itemButton = function (caption, backColor, overBackColor) {
            var button = jsObject.StandartSmallButton(null, null, caption);
            button.style.margin = "0 15px 0 0";
            button.style.background = backColor;
            button.style.color = "#ffffff";
            button.style.display = "none";
            button.caption.style.padding = "0 10px 0 10px";

            button.onmouseenter = function () {
                button.style.background = overBackColor;
            }

            button.onmouseleave = function () {
                button.style.background = backColor;
            }

            button.onmousedown = function () {
                button.style.background = backColor;
            }

            button.onmouseup = function () {
                button.style.background = overBackColor;
            }

            return button;
        }

        if (productName == "BICloud") {
            var showMoreButton = itemButton(jsObject.loc.Buttons.ShowMore, "#2184d0", "#1e78bc");
            item.innerTable.addCell(showMoreButton).style.width = "1px";
            item.showMoreButton = showMoreButton;

            showMoreButton.action = function () {
                jsObject.openNewWindow("https://www.stimulsoft.com/en/products/cloud");
            }
        }

        var renewButton = itemButton(jsObject.loc.Buttons.Upgrade, "#d7634a", "#c4462b");
        renewButton.isTrial = isTrial;
        renewButton.productName = productName;
        item.innerTable.addCell(renewButton).style.width = "1px";

        renewButton.action = function () {
            var language = jsObject.options.standaloneJsMode ? (Stimulsoft.Base.Localization.StiLocalization.cultureName == "Russian" ? "ru" : "en") : (jsObject.IsRusCulture(jsObject.options.cultureName) ? "ru" : "en");
            var type = this.isTrial ? "" : "?type=renewal";
            var sessionKey = jsObject.options.standaloneJsMode ? jsObject.options.SessionKey : (jsObject.options.cloudParameters ? jsObject.options.cloudParameters.sessionKey : null);
            var sessionKeyParam = sessionKey ? ((this.isTrial ? "?sessionKey=" : "&sessionKey=") + sessionKey) : "";
            var url = "https://www.stimulsoft.com/" + language + "/online-store/purchase/" + jsObject.GetUrlProductName(this.productName) + type + sessionKeyParam;
            jsObject.openNewWindow(url);
        }

        item.onmouseenter = function () {
            if (!this.isEnabled || (this["haveMenu"] && this.isSelected) || jsObject.options.isTouchClick) return;
            this.className = this.overClass;
            this.isOver = true;
            renewButton.style.display = "";
            if (item.showMoreButton) item.showMoreButton.style.display = "";
        }

        item.onmouseleave = function () {
            this.isOver = false;
            if (!this.isEnabled) return;
            this.className = this.isSelected ? this.selectedClass : this.defaultClass;
            renewButton.style.display = "none";
            if (item.showMoreButton) item.showMoreButton.style.display = "none";
        }

        this.appendChild(item);
    }

    panel.presentsCloudProducts = function (products) {
        for (var i = 0; i < products.length; i++) {
            var productName = jsObject.options.standaloneJsMode ? Stimulsoft.System.Enum.getName(Stimulsoft.Base.Licenses.StiProductIdent, products[i].ident) : products[i].Ident;
            if (productName == "BICloud" || productName == "CloudDashboards" || productName == "CloudReports")
                return true;
        }
        return false;
    }

    panel.buildProducts = function () {
        var products = null;
        var user = jsObject.options.standaloneJsMode ? jsObject.options.user : (jsObject.options.cloudParameters ? jsObject.options.cloudParameters.user : null);
        var owner = user ? (user.FirstName + " " + user.LastName) : "";
        var userName = user ? user.UserName : "";
        panel.progress.show();

        if (jsObject.options.standaloneJsMode) {
            var licenseKey = Stimulsoft.Base.StiLicense.licenseKey;
            if (licenseKey) {
                owner = licenseKey.owner;
                userName = licenseKey.userName;
                products = licenseKey.products;
            }
            panel.addProductsToContainer(products, owner, userName);
            panel.progress.hide();
        }
        else {
            var params = {
                UserName: userName,
                Type: "Developer",
                Format: "Base64",
                ResultSvr: false,
                Version: jsObject.options.shortProductVersion
            };
            jsObject.SendCloudCommand("LicenseActivate", params,
                function (data) {
                    if (data.ResultLicenseKey) {
                        panel.addProductsToContainer(data.ResultLicenseKey.Products, owner, userName);
                        panel.progress.hide();
                    }
                },
                function (data, msg) {
                    panel.showError(data, msg);
                });
        }
    }

    panel.addProductsToContainer = function (products, owner, userName) {
        productsContainer.clear();

        if (products && products.length > 0) {
            if (!jsObject.options.standaloneJsMode && jsObject.options.productIdentKeys) {
                for (var i = 0; i < products.length; i++) {
                    if (jsObject.options.productIdentKeys[products[i].Ident] != null)
                        products[i].ident = jsObject.options.productIdentKeys[products[i].Ident];
                }
            }
            products.sort(jsObject.SortByIdent);

            for (var i = 0; i < products.length; i++) {
                var expirationDate = jsObject.options.standaloneJsMode ? products[i].expirationDate.innerDate : jsObject.JSONDateFormatToDate(products[i].ExpirationDate);
                var productName = jsObject.options.standaloneJsMode ? Stimulsoft.System.Enum.getName(Stimulsoft.Base.Licenses.StiProductIdent, products[i].ident) : products[i].Ident;
                var licenseText = jsObject.loc.Cloud.ExpiredDate + ": " + expirationDate.toLocaleDateString();

                if (expirationDate < new Date()) {
                    licenseText = "<font style='color:#c04401'>" + licenseText + "</font>";
                }

                productsContainer.addProduct(productName, licenseText, false);
            }

            if (!panel.presentsCloudProducts(products)) {
                productsContainer.addProduct("BICloud", "Basic License", true);
            }
        }
        else {
            productsContainer.addProduct("BIDesigner", "Trial Version", true);
            productsContainer.addProduct("BICloud", "Trial License", true);
        }

        panel.controls.ownerCell.innerHTML = owner;
        panel.controls.userNameCell.innerHTML = userName;
    }

    panel.show = function () {
        this.style.display = "";
        this.visible = true;
        this.style.left = jsObject.FindPosX(parentContainer, "stiDesignerMainPanel") + "px";
        this.style.top = jsObject.FindPosY(parentContainer, "stiDesignerMainPanel") + "px";

        if ((jsObject.options.cloudMode && !jsObject.options.cloudParameters.sessionKey) || (jsObject.options.standaloneJsMode && !jsObject.options.SessionKey)) {
            jsObject.options.forms.authForm.show();
            return;
        }

        this.buildProducts();
    }

    panel.hide = function () {
        this.style.display = "none";
        this.visible = false;
    }

    panel.showError = function (data, msg) {
        panel.progress.hide();
        if (msg || data) {
            var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
            errorMessageForm.show(msg || jsObject.formatResultMsg(data));
        }
    }

    return panel;
}

StiMobileDesigner.prototype.GetUrlProductName = function (productName) {
    var name = "ultimate";
    if (productName == null)
        return name;

    switch (productName) {
        case "DbsJs":
            name = "dashboards-js";
            break;

        case "DbsWeb":
            name = "dashboards-web";
            break;

        case "DbsWin":
            name = "dashboards-win";
            break;

        case "BIDesigner":
        case "BIDesktop":
        case "BIServer":
            name = productName.substring(2).toLowerCase();
            break;

        case "BICloud":
            name = "cloud";
            break;

        case "CloudDashboards":
            name = "dashboards-cloud";
            break;

        case "CloudReports":
            name = "reports-cloud";
            break;

        default:
            name = productName.toLowerCase();
            break;
    }

    return name;
}

StiMobileDesigner.prototype.GetProductFullName = function (productName) {
    var name = "";

    switch (productName) {
        case "Ultimate":
            name = "Ultimate";
            break;

        case "DbsJs":
            name = "Dashboards.JS";
            break;

        case "DbsWeb":
            name = "Dashboards.WEB";
            break;

        case "DbsWin":
            name = "Dashboards.WIN";
            break;

        case "DbsAngular":
            name = "Dashboards.Angular";
            break;

        case "Js":
            name = "Reports.JS";
            break;

        case "Php":
            name = "Reports.PHP";
            break;

        case "Angular":
            name = "Reports.Angular";
            break;

        case "BIDesigner":
            name = "Designer";
            break;

        case "BIDesktop":
            name = "Desktop";
            break;

        case "BIServer":
            name = "Server";
            break;

        case "BICloud":
            name = "Cloud";
            break;

        case "CloudDashboards":
            name = "Dashboards Cloud";
            break;

        case "CloudReports":
            name = "Reports Cloud";
            break;

        default:
            name = "Reports." + productName;
            break;
    }

    return "Stimulsoft " + name;
}

StiMobileDesigner.prototype.SortByIdent = function (a, b) {
    if (a.ident && b.ident) {
        if (a.ident < b.ident) return -1;
        if (a.ident > b.ident) return 1;
    }
    return 0
}