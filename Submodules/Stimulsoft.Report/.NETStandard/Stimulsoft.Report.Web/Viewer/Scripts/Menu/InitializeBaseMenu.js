
StiJsViewer.prototype.BaseMenu = function (name, parentButton, animationDirection, menuStyleName, rightToLeft) {
    var parentMenu = document.createElement("div");
    parentMenu.className = "stiJsViewerParentMenu";
    var jsObject = parentMenu.jsObject = this;
    parentMenu.id = this.generateKey();
    parentMenu.name = name;
    parentMenu.items = {};
    parentMenu.parentButton = parentButton;
    parentMenu.type = null;
    if (parentButton) parentButton.haveMenu = true;
    parentMenu.animationDirection = animationDirection;
    parentMenu.rightToLeft = rightToLeft || this.options.appearance.rightToLeft;
    parentMenu.visible = false;
    parentMenu.style.display = "none";

    if (name) {
        if (!this.controls.menus) this.controls.menus = {};
        if (this.controls.menus[name] != null) {
            this.controls.menus[name].changeVisibleState(false, null, null, null, null, true);
            this.controls.mainPanel.removeChild(this.controls.menus[name]);
        }
        this.controls.menus[name] = parentMenu;
    }
    this.controls.mainPanel.appendChild(parentMenu);

    var menu = document.createElement("div");
    menu.style.overflowY = "auto";
    menu.style.overflowX = "hidden";
    menu.style.fontFamily = this.options.toolbar.fontFamily;
    if (this.options.toolbar.fontColor != "") menu.style.color = this.options.toolbar.fontColor;
    parentMenu.appendChild(menu);
    parentMenu.innerContent = menu;
    menu.className = menuStyleName || "stiJsViewerMenu";
    if (!this.options.isMobileDevice) menu.style.maxHeight = "450px";

    parentMenu.changeVisibleState = function (state, parentButton, rightAlign, leftOffset, onlyCorrectPosition, notAnimated, xPos, yPos) {
        var mainClassName = "stiJsViewerMainPanel";
        var animDirect = this.animDirect = onlyCorrectPosition && this.animDirect ? this.animDirect : this.animationDirection;
        var isVertMenu = animDirect == "Down" || animDirect == "Up";

        if (parentButton) {
            this.parentButton = parentButton;
            parentButton.haveMenu = true;
        }

        if (state) {
            this.style.display = "";
            this.visible = true;
            if (!onlyCorrectPosition) this.onshow();
            if (this.parentButton) this.parentButton.setSelected(true);
            jsObject.options[this.type == null ? (isVertMenu ? "currentMenu" : "currentHorMenu") : "current" + this.type] = this;
            this.style.width = this.innerContent.offsetWidth + "px";

            if (jsObject.options.isMobileDevice) {
                jsObject.controls.reportPanel.hideToolbar();
                this.style.marginLeft = "-" + this.style.width;
                setTimeout(function () {
                    parentMenu.style.transitionDuration = "200ms";
                    parentMenu.style.marginLeft = "0";
                });
                setTimeout(function () {
                    parentMenu.style.transitionDuration = "";
                }, notAnimated ? 0 : 200);
            }
            else {
                this.style.height = this.innerContent.offsetHeight + "px";
                this.style.overflow = "hidden";

                var viewerTop = jsObject.FindPosY(jsObject.controls.mainPanel);
                var viewerLeft = jsObject.FindPosX(jsObject.controls.mainPanel);
                var browserHeight = (window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight);
                var browserWidth = (window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth);

                if (xPos) {
                    this.style.left = xPos + "px";
                }
                else {
                    this.style.left = ((isVertMenu)
                        ? (this.rightToLeft || rightAlign
                            ? (jsObject.FindPosX(this.parentButton, mainClassName) - this.innerContent.offsetWidth + this.parentButton.offsetWidth) - (leftOffset || 0)
                            : jsObject.FindPosX(this.parentButton, mainClassName) - (leftOffset || 0))
                        : ((animDirect == "Right")
                            ? (jsObject.FindPosX(this.parentButton, mainClassName) + this.parentButton.offsetWidth + 2)
                            : (jsObject.FindPosX(this.parentButton, mainClassName) - this.parentButton.offsetWidth - 2))) + "px";
                }

                if (this.parentButton && animDirect == "Down" &&
                    jsObject.FindPosY(this.parentButton) + this.parentButton.offsetHeight + this.innerContent.offsetHeight > browserHeight &&
                    jsObject.FindPosY(this.parentButton) - this.innerContent.offsetHeight > 0) {
                    animDirect = this.animDirect = "Up";
                }

                if (yPos) {
                    this.style.top = yPos + "px";
                }
                else {
                    this.style.top = (isVertMenu)
                        ? ((animDirect == "Down")
                            ? (jsObject.FindPosY(this.parentButton, mainClassName) + this.parentButton.offsetHeight + 2) + "px"
                            : (jsObject.FindPosY(this.parentButton, mainClassName) - this.offsetHeight) + "px")
                        : (jsObject.FindPosY(this.parentButton, mainClassName) + this.parentButton.offsetHeight + this.innerContent.offsetHeight > browserHeight - viewerTop &&
                            (browserHeight - this.innerContent.offsetHeight - 10) > 0)
                            ? ((browserHeight - this.innerContent.offsetHeight - 10) + "px")
                            : (jsObject.FindPosY(this.parentButton, mainClassName) + "px");
                }

                var resultLeftPos = parseInt(this.style.left);
                var resultTopPos = parseInt(this.style.top);

                if (resultTopPos < 0) {
                    this.style.top = "10px";
                }
                else if (resultTopPos + this.innerContent.offsetHeight > browserHeight - viewerTop) {
                    this.style.top = (browserHeight - viewerTop - this.innerContent.offsetHeight - 10) + "px";
                }

                if (resultLeftPos < 0) {
                    this.style.left = "10px";
                }
                else if (resultLeftPos + this.innerContent.offsetWidth > browserWidth - viewerLeft) {
                    this.style.left = (browserWidth - viewerLeft - this.innerContent.offsetWidth - 10) + "px";
                }

                this.innerContent.style.top = (isVertMenu ? ((animDirect == "Down" ? -1 : 1) * this.innerContent.offsetHeight) : 0) + "px";
                this.innerContent.style.left = (!isVertMenu ? ((animDirect == "Right" ? -1 : 1) * this.innerContent.offsetWidth) : 0) + "px";

                var d = new Date();
                var endTime = d.getTime();
                if (jsObject.options.toolbar.menuAnimation) endTime += (!onlyCorrectPosition ? jsObject.options.menuAnimDuration : 0);

                if (isVertMenu)
                    jsObject.ShowAnimationVerticalMenu(this, (animDirect == "Down" ? 0 : -1), endTime);
                else
                    jsObject.ShowAnimationHorizontalMenu(this, (animDirect == "Right" ? 0 : -1), endTime);
            }
        }
        else {
            this.onHide();
            clearTimeout(this.innerContent.animationTimer);
            this.visible = false;
            if (this.parentButton) this.parentButton.setSelected(false);
            if (jsObject.options.isMobileDevice) {
                this.style.transitionDuration = "200ms";
                this.style.marginLeft = "-" + this.style.width;
                setTimeout(function () {
                    parentMenu.style.transitionDuration = "";
                    parentMenu.style.display = "none";
                }, notAnimated ? 0 : 200);
            }
            else {
                this.style.display = "none";
            }
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

    parentMenu.ontouchstart = function (e) {
        if (jsObject.options.isMobileDevice && typeof (e) != "boolean") {
            this.touchStartX = parseInt(e.changedTouches[0].clientX);
            this.lastTouches = [{ x: 0, y: 0, time: 0 }, { x: 0, y: 0, time: 0 }];
        }

        var this_ = this;
        this.isTouchStartFlag = e ? false : true;
        clearTimeout(this.isTouchStartTimer);
        jsObject.options.menuPressed = this;
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
        setTimeout(function () { jsObject.options.menuPressed = false }, 250); //touch fix
    }

    parentMenu.ontouchmove = function (e) {
        if (jsObject.options.isMobileDevice) {
            this.lastTouches.shift();
            this.lastTouches.push({
                x: e.changedTouches[0].clientX,
                y: e.changedTouches[0].clientY,
                time: new Date().getTime()
            });
        }
    }

    parentMenu.ontouchend = function (e) {
        if (jsObject.options.isMobileDevice) {
            var dX = this.lastTouches[1].x - this.lastTouches[0].x;
            var dT = new Date().getTime() - this.lastTouches[1].time;
            if (dX <= -5 && dT <= 14) this.changeVisibleState(false);
        }
    }

    parentMenu.correctPositions = function () {
        this.changeVisibleState(true, this.parentButton, null, null, true);
    }

    parentMenu.onshow = function () { };
    parentMenu.onHide = function () { };

    parentMenu.applyStyleColors = function (styleColors) {
        this.styleColors = styleColors;
        if (styleColors) {
            menu.style.borderColor = styleColors.separatorColor;
            menu.style.background = styleColors.backColor;
            menu.style.color = styleColors.foreColor;
        }
    }

    return parentMenu;
}