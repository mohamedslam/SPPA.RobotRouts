
StiMobileDesigner.prototype.BarCodeTypeControl = function (name, width) {
    var control = this.CreateHTMLTable();
    control.isEnabled = true;

    var button = this.SmallButton(null, null, "BarCode ", "BarCodes.StiCodabarBarCodeType.png", null, "Down", "stiDesignerFormButton", true);
    if (width) button.style.width = width + "px";
    button.innerTable.style.width = "100%";
    button.imageCell.style.width = "1px";
    button.arrowCell.style.width = "1px";
    control.addCell(button);

    var capBlock = document.createElement("div");
    capBlock.setAttribute("style", "text-overflow: ellipsis; overflow: hidden; white-space: nowrap;");
    if (width) capBlock.style.width = (width - (this.options.isTouchDevice ? 50 : 45)) + "px";
    button.caption.innerHTML = "";
    button.caption.appendChild(capBlock);

    var allItems = {
        mainMenu: this.GetBarCodeCategoriesItems(),
        twoDimensional: this.GetBarCodeTwoDimensionalItems(),
        eANUPC: this.GetBarCodeEANUPCItems(),
        gS1: this.GetBarCodeGS1Items(),
        post: this.GetBarCodePostItems(),
        others: this.GetBarCodeOthersItems(),
    };

    var menu = this.VerticalMenu(name + "Menu", button, "Down", allItems.mainMenu);

    this.InitializeSubMenu(name + "TwoDimensionalMenu", allItems.twoDimensional, menu.items["TwoDimensional"], menu);
    this.InitializeSubMenu(name + "EANUPCMenu", allItems.eANUPC, menu.items["EANUPC"], menu);
    this.InitializeSubMenu(name + "GS1Menu", allItems.gS1, menu.items["GS1"], menu);
    this.InitializeSubMenu(name + "PostMenu", allItems.post, menu.items["Post"], menu);
    this.InitializeSubMenu(name + "OthersMenu", allItems.others, menu.items["Others"], menu).firstChild.style.maxHeight = "700px";

    button.action = function () {
        menu.changeVisibleState(!menu.visible);
    }

    control.action = function () { }

    control.setKey = function (key) {
        this.key = key;
        for (var items in allItems) {
            if (allItems[items].length) {
                for (var i = 0; i < allItems[items].length; i++) {
                    if (key == allItems[items][i].key) {
                        StiMobileDesigner.setImageSource(button.image, this.jsObject.options, allItems[items][i].imageName);
                        capBlock.innerText = allItems[items][i].caption;
                        capBlock.setAttribute("title", capBlock.innerText);
                    }
                }
            }
        }
    }

    control.setEnabled = function (state) {
        control.isEnabled = state;
        button.setEnabled(state);
    }

    menu.action = function (menuItem) {
        if (menuItem.haveSubMenu) return;
        menu.changeVisibleState(false);
        control.setKey(menuItem.key);
        control.action();
    }

    return control;
}