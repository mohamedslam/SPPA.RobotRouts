
StiMobileDesigner.prototype.IconControl = function (name, width, height, toolTip, customIconControl, showClearButton) {
    var jsObject = this;
    var showOpenCustomIcon = customIconControl != null;

    var control = this.DropDownList(name, width, toolTip, [], true, false, height, false, null, showOpenCustomIcon, showClearButton);
    control.items = {};
    control.textBox.style.fontFamily = "Stimulsoft";
    control.textBox.style.fontSize = height ? (height - 6) + "px" : "17px";
    control.textBox.style.color = "#4472c4";
    control.textBox.style.textAlign = width && width <= 80 ? "center" : "left";

    var groupLocalizedNames = {
        AccessibilityIcons: this.loc.PropertyEnum.StiFontIconGroupAccessibilityIcons,
        BrandIcons: this.loc.PropertyEnum.StiFontIconGroupBrandIcons,
        ChartIcons: this.loc.Components.StiChart,
        CurrencyIcons: this.loc.FormFormatEditor.Currency,
        DirectionalIcons: this.loc.PropertyEnum.StiFontIconGroupDirectionalIcons,
        FileTypeIcons: this.loc.PropertyMain.File,
        FormControlIcons: this.loc.PropertySystemColors.Control,
        GenderIcons: this.loc.PropertyEnum.StiFontIconGroupGenderIcons,
        HandIcons: this.loc.Toolbox.Hand,
        MedicalIcons: this.loc.PropertyEnum.StiFontIconGroupMedicalIcons,
        OtherIcons: this.loc.FormDictionaryDesigner.CsvSeparatorOther,
        PaymentIcons: this.loc.PropertyEnum.StiFontIconGroupPaymentIcons,
        SpinnerIcons: this.loc.PropertyEnum.StiFontIconGroupSpinnerIcons,
        TextEditorIcons: this.loc.Toolbox.TextEditor,
        TransportationIcons: this.loc.PropertyEnum.StiFontIconGroupTransportationIcons,
        VideoPlayerIcons: this.loc.PropertyEnum.StiFontIconGroupVideoPlayerIcons,
        WebApplicationIcons: this.loc.PropertyEnum.StiFontIconGroupWebApplicationIcons
    };

    var findTextbox = this.TextBox(null, 220);
    findTextbox.setAttribute("placeholder", this.loc.FormViewer.Find);
    findTextbox.style.margin = "4px";
    control.menu.innerContent.appendChild(findTextbox);

    var itemsContainer = document.createElement("div");
    itemsContainer.style.height = "350px";
    itemsContainer.style.overflow = "auto";
    control.menu.innerContent.appendChild(itemsContainer);
    control.menu.innerContent.style.maxHeight = "500px";

    itemsContainer.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
    }

    if (control.clearButton) {
        control.clearButton.action = function () {
            control.key = null;
            control.textBox.value = "";
            control.action();
        }
    }

    control.addItems = function (findText) {
        itemsContainer.clear();

        for (var i = 0; i < jsObject.options.fontIcons.length; i++) {
            var groupItems = jsObject.options.fontIcons[i].items;
            var groupName = jsObject.options.fontIcons[i].groupName;
            var groupHeader = jsObject.FormBlockHeader(groupLocalizedNames[groupName] || groupName);
            groupHeader.caption.style.padding = "4px 4px 4px 15px";
            itemsContainer.appendChild(groupHeader);

            var itemsTable = jsObject.CreateHTMLTable();
            itemsTable.style.margin = "2px 0 2px 0";
            itemsContainer.appendChild(itemsTable);

            var groupItemsCount = 0;

            for (var k = 0; k < groupItems.length; k++) {
                if (groupItemsCount != 0 && groupItemsCount % 6 == 0) {
                    itemsTable.addRow();
                }

                if (findText && groupItems[k].key && groupItems[k].key.toLowerCase().indexOf(findText.toLowerCase()) < 0) continue;

                groupItemsCount++;

                var itemButton = jsObject.StandartSmallButton(null, null, groupItems[k].text);
                itemButton.style.width = "35px";
                itemButton.innerTable.style.width = "100%";
                itemButton.caption.style.textAlign = "center";
                itemButton.key = groupItems[k].key;
                if (control.textBox.style.color) itemButton.caption.style.color = control.textBox.style.color;

                control.items[groupName + "_" + itemButton.key] = itemButton;

                if (itemButton.caption) {
                    itemButton.caption.style.padding = "0 5px 0 5px";
                    itemButton.caption.style.fontFamily = "Stimulsoft";
                    itemButton.caption.style.fontSize = "18px";
                }
                itemsTable.addCellInLastRow(itemButton);

                itemButton.action_ = function () {
                    control.setKey(this.key);
                    control.action();
                    control.menu.changeVisibleState(false);
                }

                if (jsObject.options.isTouchDevice) {
                    itemButton.action = function () {
                        this.action_();
                    };
                }
                else {
                    itemButton.onmousedown = function () {
                        this.action_();
                    }
                }
            }

            if (groupItemsCount == 0) groupHeader.style.display = "none";
        }

        if (showOpenCustomIcon) {
            StiMobileDesigner.setImageSource(control.editButton.image, jsObject.options, "3Dots.png");

            control.editButton.action = function () {
                customIconControl.openButton.action();
            }
        }
    }

    findTextbox.onchange = function () {
        control.addItems(this.value);
    }

    control.menu.onshow = function () {
        findTextbox.focus();
        for (var itemName in control.items) {
            var item = control.items[itemName];
            item.setSelected(item.key == control.key);
            if (item.caption) {
                item.caption.style.color = control.textBox.style.color;
            }
        }
    }

    control.menu.onhide = function () {
        findTextbox.value = "";
        control.addItems();
    }

    control.setKey = function (key) {
        this.key = key;
        this.textBox.value = "";

        for (var itemName in control.items) {
            var item = control.items[itemName];
            if (item.key == control.key) {
                this.textBox.value = item.caption.innerHTML;
                break;
            }
        }
    }

    control.addItems();

    return control;
}