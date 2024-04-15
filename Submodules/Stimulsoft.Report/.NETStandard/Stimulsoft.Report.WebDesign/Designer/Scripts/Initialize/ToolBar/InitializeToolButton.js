
StiMobileDesigner.prototype.ToolButton = function (name, caption, imageName, toolTip) {
    var jsObject = this;
    var button = this.SmallButton(name, "ToolButtons", caption, imageName, toolTip, null, "stiDesignerToolButton", true);
    button.style.height = "30px";
    button.style.minWidth = "60px";

    var innerTable = button.innerTable;
    innerTable.style.width = "100%";
    innerTable.style.height = "27px";

    if (button.caption) {
        button.caption.style.textAlign = "center";
        button.caption.style.padding = "3px 12px 0 12px";
    }

    var footer = button.footer = document.createElement("div");
    footer.style.margin = "0 10px 0 10px";
    footer.style.height = "3px";
    button.appendChild(footer);

    button.action = function () {
        this.setSelected(true);
        this.jsObject.ExecuteAction(this.name);
    }

    //Override
    button.onmouseover = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseenter();
    }

    button.onmouseenter = function () {
        if (!this.isEnabled || this.jsObject.options.isTouchClick || (jsObject.isOffice2013Theme() && this.isSelected)) return;
        this.className = this.overClass;
        this.isOver = true;
        this.footer.style.margin = "0";
    }

    button.onmouseleave = function () {
        if (!this.isEnabled || (jsObject.isOffice2013Theme() && this.isSelected)) return;
        this.className = this.defaultClass;
        this.isOver = false;
        this.footer.style.margin = "0 10px 0 10px";
    }

    button.setSelected = function (state) {
        if (this.groupName && state)
            for (var name in this.jsObject.options.buttons) {
                if (this.groupName == this.jsObject.options.buttons[name].groupName) {
                    this.jsObject.options.buttons[name].setSelected(false);
                }
            }

        this.isSelected = state;
        this.className = this.isOver && !jsObject.isOffice2013Theme() ? this.overClass : (state ? this.selectedClass : this.defaultClass);
        this.footer.className = state ? "stiDesignerStandartTabFooter" : "";
    }

    return button;
}

StiMobileDesigner.prototype.ToolButtonAdditional = function (name, caption, imageName, toolTip) {
    var button = this.SmallButton(name, null, caption, imageName, toolTip, null, this.isDarkToolBar() ? "stiDesignerToolButton" : null);
    button.style.height = "30px";
    button.style.minWidth = "30px";
    button.style.border = "0";
    button.innerTable.style.width = "100%";

    return button;
}