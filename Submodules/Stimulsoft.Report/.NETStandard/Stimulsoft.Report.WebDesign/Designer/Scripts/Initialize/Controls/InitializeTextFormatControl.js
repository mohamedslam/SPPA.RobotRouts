
StiMobileDesigner.prototype.TextFormatControl = function (name) {
    var tFrmtControl = document.createElement("div");
    tFrmtControl.jsObject = this;
    tFrmtControl.name = name;
    tFrmtControl.key = null;
    tFrmtControl.isEnabled = true;
    this.options.controls[name] = tFrmtControl;

    //Button
    var button = tFrmtControl.button = this.BigButton(name + "Button", null, " ", " ", [this.loc.HelpDesigner.TextFormat, this.HelpLinks["textformat"]], true);
    button.textFormatControl = tFrmtControl;
    tFrmtControl.appendChild(button);

    button.action = function () {
        this.jsObject.options.menus[tFrmtControl.name + "Menu"].changeVisibleState(!this.jsObject.options.menus[tFrmtControl.name + "Menu"].visible);
    }

    //Menu
    var menu = tFrmtControl.menu = this.VerticalMenu(name + "Menu", button, "Down", this.GetTextFormatItems(true), "stiDesignerMenuMiddleItem");
    menu.textFormatControl = tFrmtControl;

    menu.action = function (menuItem) {
        menuItem.setSelected(true);
        this.changeVisibleState(false);
        tFrmtControl.setKey(menuItem.key);
        tFrmtControl.action();
    }

    menu.onshow = function () {
        if (this.items[tFrmtControl.key])
            this.items[tFrmtControl.key].setSelected(true);
        else {
            if (this.selectedItem) {
                this.selectedItem.setSelected(false);
                this.selectedItem = null;
            }
        }
    }

    //Override 
    tFrmtControl.setKey = function (key) {
        this.key = key;
        button.caption.innerHTML = key == "StiCustomFormatService" ? this.jsObject.loc.FormFormatEditor.Custom : "";
        button.innerTable.style.marginTop = key == "StiEmptyValue" ? "6px" : "0px";
        button.arrow.style.marginTop = key == "StiEmptyValue" ? "5px" : "0px";
        button.image.style.visibility = key == "StiEmptyValue" ? "hidden" : "visible";
        button.cellImage.style.border = key == "StiEmptyValue" ? "1px dashed rgb(180, 180, 180)" : "0px";
        button.cellImage.style.display = key == "StiCustomFormatService" ? "none" : "";
        button.caption.style.paddingTop = key == "StiCustomFormatService" ? "15px" : "0px";

        if (menu.items[key]) {
            button.caption.innerHTML = menu.items[key].caption.innerHTML;
            StiMobileDesigner.setImageSource(button.image, this.jsObject.options, menu.items[key].imageName);
        }
    };

    tFrmtControl.setEnabled = function (state) {
        this.isEnabled = state;
        if (button.image) button.image.style.opacity = state ? "1" : "0.3";
        button.setEnabled(state);
    }

    tFrmtControl.action = function () { };

    return tFrmtControl;
}