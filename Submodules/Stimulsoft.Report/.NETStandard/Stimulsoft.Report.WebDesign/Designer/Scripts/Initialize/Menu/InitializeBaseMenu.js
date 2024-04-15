
StiMobileDesigner.prototype.BaseMenu = function (name, parentButton, animationDirection, rightToLeft) {
    var parentMenu = document.createElement("div");

    if (name && this.options.menus[name] != null) {
        this.options.menus[name].changeVisibleState(false);
        this.options.mainPanel.removeChild(this.options.menus[name]);
    }

    this.options.mainPanel.appendChild(parentMenu);
    parentMenu.className = "stiDesignerParentMenu";
    var jsObject = parentMenu.jsObject = this;
    parentMenu.name = parentMenu.id = name || this.generateKey();
    if (!name) parentMenu.isTemporary = true;
    this.options.menus[parentMenu.name] = parentMenu;
    parentMenu.items = {};
    parentMenu.parentButton = parentButton;
    if (parentButton) parentButton.haveMenu = true;
    parentMenu.type = null;
    parentMenu.animationDirection = animationDirection;
    parentMenu.rightToLeft = rightToLeft;
    parentMenu.visible = false;
    parentMenu.style.display = "none";

    var menu = document.createElement("div");
    menu.style.overflowY = "auto";
    menu.style.overflowX = "hidden";
    menu.style.maxHeight = "380px";
    menu.style.minHeight = "21px";
    parentMenu.appendChild(menu);
    parentMenu.innerContent = menu;
    menu.className = "stiDesignerMenu";

    parentMenu.changeVisibleState = function (state, parentButton, xPos, yPos) {
        if (parentButton) this.parentButton = parentButton;
        var animDirect = this.animationDirection;
        var isVertMenu = animDirect == "Down" || animDirect == "Up";

        if (state) {
            if (!this.parentElement) jsObject.options.mainPanel.appendChild(this);
            this.style.display = "";
            this.visible = true;
            this.onshow();
            this.style.overflow = "hidden";
            if (parentButton) parentButton.haveMenu = true;
            if (this.parentButton) this.parentButton.setSelected(true);
            jsObject.options[this.type == null ? (isVertMenu ? "currentMenu" : "currentHorMenu") : "current" + this.type] = this;
            this.style.width = this.innerContent.offsetWidth + "px";
            this.style.height = this.innerContent.offsetHeight + "px";
            var browserHeight = (window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight) - jsObject.FindPosY(jsObject.options.mainPanel);
            var browserWidth = (window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth) - jsObject.FindPosX(jsObject.options.mainPanel);

            if (xPos) {
                this.style.left = xPos + "px";
            }
            else {
                this.style.left = (isVertMenu)
                    ? ((this.rightToLeft)
                        ? (jsObject.FindPosX(this.parentButton, "stiDesignerMainPanel") - this.innerContent.offsetWidth + this.parentButton.offsetWidth) + "px"
                        : (jsObject.FindPosX(this.parentButton, "stiDesignerMainPanel")) + "px")
                    : ((animDirect == "Right")
                        ? (jsObject.FindPosX(this.parentButton, "stiDesignerMainPanel") + this.parentButton.offsetWidth + 2) + "px"
                        : (jsObject.FindPosX(this.parentButton, "stiDesignerMainPanel") - this.parentButton.offsetWidth - 2) + "px");
            }

            if (this.parentButton && animDirect == "Down" &&
                jsObject.FindPosY(this.parentButton, "stiDesignerMainPanel") + this.parentButton.offsetHeight + this.innerContent.offsetHeight > browserHeight &&
                jsObject.FindPosY(this.parentButton, "stiDesignerMainPanel") - this.innerContent.offsetHeight > 0) {
                animDirect = "Up";
            }

            if (yPos) {
                this.style.top = yPos + "px";
            }
            else {
                this.style.top = (isVertMenu)
                    ? ((animDirect == "Down")
                        ? (jsObject.FindPosY(this.parentButton, "stiDesignerMainPanel") + this.parentButton.offsetHeight + 2) + "px"
                        : (jsObject.FindPosY(this.parentButton, "stiDesignerMainPanel") - this.offsetHeight) + "px")
                    : (jsObject.FindPosY(this.parentButton, "stiDesignerMainPanel") + this.parentButton.offsetHeight + this.innerContent.offsetHeight > browserHeight &&
                        (browserHeight - this.innerContent.offsetHeight - 10) > 0)
                        ? ((browserHeight - this.innerContent.offsetHeight - 10) + "px")
                        : (jsObject.FindPosY(this.parentButton, "stiDesignerMainPanel") + "px");

            }

            var resultLeftPos = parseInt(this.style.left);
            var resultTopPos = parseInt(this.style.top);

            if (resultTopPos < 0) {
                this.style.top = "10px";
            }
            else if (resultTopPos + this.innerContent.offsetHeight > browserHeight) {
                this.style.top = (browserHeight - this.innerContent.offsetHeight - 10) + "px";
            }

            if (resultLeftPos < 0) {
                this.style.left = "10px";
            }
            else if (resultLeftPos + this.innerContent.offsetWidth > browserWidth) {
                this.style.left = (browserWidth - this.innerContent.offsetWidth - 10) + "px";
            }

            this.innerContent.style.top = (isVertMenu ? ((animDirect == "Down" ? -1 : 1) * this.innerContent.offsetHeight) : 0) + "px";
            this.innerContent.style.left = (!isVertMenu ? ((animDirect == "Right" ? -1 : 1) * this.innerContent.offsetWidth) : 0) + "px";

            var d = new Date();
            var endTime = d.getTime() + jsObject.options.menuAnimDuration;
            if (isVertMenu)
                jsObject.ShowAnimationVerticalMenu(this, (animDirect == "Down" ? 0 : -1), endTime);
            else
                jsObject.ShowAnimationHorizontalMenu(this, (animDirect == "Right" ? 0 : -1), endTime);
        }
        else {
            clearTimeout(this.innerContent.animationTimer);
            this.visible = false;
            if (this.parentButton) this.parentButton.setSelected(false);
            this.style.display = "none";
            this.onhide();
            if (this.currentSubMenu) this.currentSubMenu.changeVisibleState(false);
            if (jsObject.options[this.type == null ? (isVertMenu ? "currentMenu" : "currentHorMenu") : "current" + this.type] == this)
                jsObject.options[this.type == null ? (isVertMenu ? "currentMenu" : "currentHorMenu") : "current" + this.type] = null;
        }
    }

    parentMenu.action = function (menuItem) {
        return menuItem;
    }

    parentMenu.onmousedown = function () {
        if (!this.isTouchStartFlag) this.ontouchstart(true);
    }

    parentMenu.ontouchstart = function (mouseProcess) {
        var this_ = this;
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        jsObject.options.menuPressed = this;
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    parentMenu.scrollToItem = function (item) {
        setTimeout(function () {
            if (parentMenu.items) {
                var yPos = jsObject.FindPosY(item, null, false, parentMenu.innerContent);
                parentMenu.innerContent.scrollTop = yPos - 30;
            }
        }, jsObject.options.menuAnimDuration);
    }

    parentMenu.onshow = function () { };
    parentMenu.onhide = function () { };

    return parentMenu;
}

StiMobileDesigner.prototype.Separator = function () {
    var separator = document.createElement("div");
    separator.className = "stiDesignerFormSeparator";

    return separator;
}

StiMobileDesigner.prototype.StandartMenuHeader = function (text) {
    var header = document.createElement("div");
    header.className = "stiDesignerGroupPanelHeader";
    header.style.height = "auto";
    header.style.padding = "5px";
    header.innerHTML = text;

    return header;
}