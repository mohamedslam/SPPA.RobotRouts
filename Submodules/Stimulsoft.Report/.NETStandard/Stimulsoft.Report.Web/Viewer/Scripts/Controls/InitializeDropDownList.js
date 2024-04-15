
StiJsViewer.prototype.DropDownList = function (name, width, toolTip, items, readOnly, showImage) {
    var dropDownList = this.CreateHTMLTable();
    dropDownList.style.fontFamily = this.options.toolbar.fontFamily;
    if (this.options.toolbar.fontColor != "") dropDownList.style.color = this.options.toolbar.fontColor;
    dropDownList.jsObject = this;
    dropDownList.name = name;
    dropDownList.key = null;
    dropDownList.imageCell = null;
    dropDownList.readOnly = readOnly;
    dropDownList.items = (items == null) ? {} : items;
    dropDownList.isEnabled = true;
    dropDownList.isSelected = false;
    dropDownList.isOver = false;
    dropDownList.isFocused = false;
    dropDownList.fullWidth = width + 2;
    dropDownList.className = "stiJsViewerDropDownList";

    if (toolTip) dropDownList.setAttribute("title", toolTip);
    var textBoxWidth = width - 23 - (showImage ? 38 : 0);

    if (name) {
        if (!this.controls.dropDownLists) this.controls.dropDownLists = {};
        this.controls.dropDownLists[name] = dropDownList;
    }

    //Image
    if (showImage) {
        var image = dropDownList.image = document.createElement("div");
        image.dropDownList = dropDownList;
        image.jsObject = this;
        image.className = "stiJsViewerDropDownListImage";

        dropDownList.imageCell.style.lineHeight = "0";
        dropDownList.imageCell = dropDownList.addCell(image);

        if (readOnly) {
            image.onclick = function () {
                if (!this.isTouchEndFlag && !this.jsObject.options.isTouchClick)
                    this.dropDownList.button.onclick();
            }

            image.ontouchend = function () {
                var this_ = this;
                this.isTouchEndFlag = true;
                clearTimeout(this.isTouchEndTimer);
                this.dropDownList.button.ontouchend();
                this.isTouchEndTimer = setTimeout(function () {
                    this_.isTouchEndFlag = false;
                }, 1000);
            }
        }
    }

    //TextBox
    var textBox = dropDownList.textBox = document.createElement("input");
    textBox.jsObject = this;
    textBox.style.width = textBoxWidth + "px";
    textBox.dropDownList = dropDownList;
    textBox.readOnly = readOnly;
    textBox.style.border = 0;
    textBox.style.cursor = readOnly ? "default" : "text";
    textBox.style.fontFamily = this.options.toolbar.fontFamily;
    textBox.style.height = this.options.isTouchDevice ? "26px" : "18px";
    textBox.style.lineHeight = textBox.style.height;
    textBox.className = "stiJsViewerDropDownList_TextBox";
    dropDownList.addCell(textBox);

    if (this.options.toolbar.fontColor != "") {
        textBox.style.color = this.options.toolbar.fontColor;
    }

    if (readOnly) {
        textBox.onclick = function () {
            if (!this.isTouchEndFlag && !this.jsObject.options.isTouchDevice && !this.jsObject.options.isTouchClick)
                this.dropDownList.button.onclick();
        }
        textBox.ontouchend = function () {
            var this_ = this;
            this.isTouchEndFlag = true;
            clearTimeout(this.isTouchEndTimer);
            this.dropDownList.button.ontouchend();
            this.isTouchEndTimer = setTimeout(function () {
                this_.isTouchEndFlag = false;
            }, 1000);
        }
    }
    textBox.action = function () {
        if (!this.dropDownList.readOnly) {
            this.dropDownList.setKey(this.value); this.dropDownList.action();
        }
    }

    textBox.onfocus = function () {
        this.isFocused = true;
        this.dropDownList.isFocused = true;
        this.dropDownList.setSelected(true);
    }

    textBox.onblur = function () {
        this.isFocused = false;
        this.dropDownList.isFocused = false;
        this.dropDownList.setSelected(false); this.action();
    }

    textBox.onkeypress = function (event) {
        if (this.dropDownList.readOnly) return false;
        if (event && event.keyCode == 13) {
            this.action();
            return false;
        }
    }

    //DropDownButton
    var button = dropDownList.button = this.SmallButton(null, null, "Arrows." + (this.options.isTouchDevice ? "Big" : "Small") + "ArrowDown.png", null, null, "stiJsViewerDropDownListButton", null, this.options.isTouchDevice ? { width: 16, height: 16 } : { width: 8, height: 8 });
    button.imageCell.style.padding = "0 7px";
    button.style.height = this.isTouchDevice ? "26px" : "21px";
    button.dropDownList = dropDownList;
    dropDownList.addCell(button);

    button.action = function () {
        if (!this.dropDownList.menu.visible) {
            if (this.dropDownList.menu.isDinamic) this.dropDownList.menu.addItems(this.dropDownList.items);
            this.dropDownList.menu.changeVisibleState(true);
        }
        else
            this.dropDownList.menu.changeVisibleState(false);
    }

    //Menu
    var menu = dropDownList.menu = this.DropDownListMenu(dropDownList);
    this.controls.mainPanel.appendChild(menu);
    menu.isDinamic = (items == null);
    if (items != null) menu.addItems(items);

    dropDownList.onmouseover = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseenter();
    }

    dropDownList.onmouseout = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseleave();
    }

    dropDownList.onmouseenter = function () {
        if (!this.isEnabled) return;
        this.isOver = true;
        if (!this.isSelected && !this.isFocused) this.className = "stiJsViewerDropDownListOver";
    }

    dropDownList.onmouseleave = function () {
        if (!this.isEnabled) return;
        this.isOver = false;
        if (!this.isSelected && !this.isFocused) this.className = "stiJsViewerDropDownList";
    }

    dropDownList.setEnabled = function (state) {
        this.isEnabled = state;
        this.button.setEnabled(state);
        this.textBox.disabled = !state;
        this.textBox.style.visibility = state ? "visible" : "hidden";
        this.className = state ? "stiJsViewerDropDownList" : "stiJsViewerDropDownListDisabled";
        if (this.imageCell) this.image.style.visibility = state ? "visible" : "hidden";
    }

    dropDownList.setSelected = function (state) {
        this.isSelected = state;
        this.className = state ? "stiJsViewerDropDownListOver" :
            (this.isEnabled ? (this.isOver ? "stiJsViewerDropDownListOver" : "stiJsViewerDropDownList") : "stiJsViewerDropDownListDisabled");
    }

    dropDownList.setKey = function (key) {
        this.key = key;
        for (var itemName in this.items)
            if (key == this.items[itemName].key) {
                this.textBox.value = this.items[itemName].caption;
                if (this.image) StiJsViewer.setImageSource(this.image, this.jsObject.options, this.jsObject.collections, this.items[itemName].imageName);
                return;
            }
        this.textBox.value = key.toString();
    }

    dropDownList.haveKey = function (key) {
        if (this.items && this.items.length) {
            for (var i = 0; i < this.items.length; i++)
                if (this.items[i].key == key) return true;
        }
        return false;
    }

    dropDownList.action = function () { }

    return dropDownList;
}

StiJsViewer.prototype.DropDownListMenu = function (dropDownList) {
    var menu = this.VerticalMenu(dropDownList.name || this.newGuid().replace(/-/g, ''), dropDownList.button, "Down", dropDownList.items, "stiJsViewerMenuStandartItem", "stiJsViewerDropdownMenu");
    menu.dropDownList = dropDownList;
    menu.innerContent.style.minWidth = dropDownList.fullWidth + "px";

    menu.changeVisibleState = function (state) {
        var mainClassName = "stiJsViewerMainPanel";
        if (state) {
            this.onshow();
            this.style.display = "";
            this.visible = true;
            this.style.overflow = "hidden";
            this.parentButton.dropDownList.setSelected(true);
            this.parentButton.setSelected(true);
            this.jsObject.options.currentDropDownListMenu = this;
            this.style.width = this.innerContent.offsetWidth + "px";
            this.style.height = this.innerContent.offsetHeight + "px";
            this.style.left = (this.jsObject.FindPosX(this.parentButton.dropDownList, mainClassName) + 1) + "px";
            var browserHeight = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
            var animationDirection =
                (this.jsObject.FindPosY(dropDownList) + dropDownList.offsetHeight + this.offsetHeight > browserHeight) &&
                    (this.jsObject.FindPosY(dropDownList) - this.offsetHeight > 0)
                    ? "Up" : "Down";
            this.style.top = animationDirection == "Down"
                ? (this.jsObject.FindPosY(dropDownList, mainClassName) + this.parentButton.offsetHeight + 2) + "px"
                : (this.jsObject.FindPosY(dropDownList, mainClassName) - this.offsetHeight) + "px";
            this.innerContent.style.top = (animationDirection == "Down" ? -this.innerContent.offsetHeight : this.innerContent.offsetHeight) + "px";

            var d = new Date();
            var endTime = d.getTime();
            if (this.jsObject.options.toolbar.menuAnimation) endTime += this.jsObject.options.menuAnimDuration;
            this.jsObject.ShowAnimationVerticalMenu(this, 0, endTime);
        }
        else {
            clearTimeout(this.innerContent.animationTimer);
            this.visible = false;
            this.parentButton.dropDownList.setSelected(false);
            this.parentButton.setSelected(false);
            this.style.display = "none";
            if (this.jsObject.options.currentDropDownListMenu == this) this.jsObject.options.currentDropDownListMenu = null;
        }
    }

    menu.onmousedown = function () {
        if (!this.isTouchStartFlag) this.ontouchstart(true);
    }

    menu.ontouchstart = function (mouseProcess) {
        var this_ = this;
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        this.jsObject.options.dropDownListMenuPressed = this;
        this.isTouchStartTimer = setTimeout(function () {
            this_.isTouchStartFlag = false;
        }, 1000);
    }

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
        this.dropDownList.key = menuItem.key;
        this.dropDownList.textBox.value = menuItem.caption.innerHTML;
        if (this.dropDownList.image) StiJsViewer.setImageSource(this.dropDownList.image, this.jsObject.options, this.jsObject.collections, menuItem.imageName);
        this.dropDownList.action();
    }

    menu.onshow = function () {
        if (this.dropDownList.key == null) return;
        for (var itemName in this.items) {
            if (this.dropDownList.key == this.items[itemName].key) {
                this.items[itemName].setSelected(true);
                return;
            }
            else if (this.items[itemName]["setSelected"]) {
                this.items[itemName].setSelected(false);
            }
        }
    }

    return menu;
}