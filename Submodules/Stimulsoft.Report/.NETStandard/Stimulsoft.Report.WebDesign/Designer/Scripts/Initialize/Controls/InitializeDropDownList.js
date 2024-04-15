
StiMobileDesigner.prototype.DropDownList = function (name, width, toolTip, items, readOnly, showImage, height, cutMenu, imageSizes, showEditButton, showClearButton) {
    var dropDownList = this.CreateHTMLTable();
    var jsObject = dropDownList.jsObject = this;
    dropDownList.style.borderCollapse = "separate";
    dropDownList.key = null;
    dropDownList.imageCell = null;
    dropDownList.readOnly = readOnly;
    dropDownList.items = (items == null) ? [] : items;
    dropDownList.isEnabled = true;
    dropDownList.isSelected = false;
    dropDownList.isOver = false;
    dropDownList.toolTip = toolTip;
    dropDownList.fullWidth = width;
    dropDownList.cutMenu = cutMenu;
    dropDownList.imageSizes = imageSizes;

    if (name != null) {
        dropDownList.name = name;
        this.options.controls[name] = dropDownList;
    }
    else {
        dropDownList.name = this.generateKey();
        dropDownList.isTemporary = true;
    }

    if (toolTip && typeof (toolTip) != "object") {
        dropDownList.setAttribute("title", toolTip);
    }

    //Image
    var imageCellBaseClass = "stiDesignerDropDownListImageCell stiDesignerDropDownListImageCell";

    if (showImage) {
        var image = dropDownList.image = document.createElement("img");
        image.style.width = (imageSizes ? imageSizes.width : 32) + "px";
        image.style.height = (imageSizes ? imageSizes.height : 16) + "px";
        image.className = "stiDesignerDropDownListImage";

        var imageCell = dropDownList.imageCell = dropDownList.addCell();
        imageCell.appendChild(image);
        imageCell.className = imageCellBaseClass + "Default";
        imageCell.style.borderRadius = this.allowRoundedControls() ? "3px 0 0 3px" : "0";
        imageCell.style.borderRight = "0";
    }

    var textHeight = height || this.options.controlsHeight;
    var buttonWidth = this.options.controlsButtonsWidth + 2;
    var buttonHeight = textHeight - 2;
    var textBoxWidth = width - ((showEditButton || showClearButton) ? (buttonWidth * (showEditButton && showClearButton ? 3 : 2)) : buttonWidth) - (showImage ? 38 : 0);

    var textBox = dropDownList.textBox = this.TextBox(name ? name + "TextBox" : null, textBoxWidth, textHeight);
    textBox.dropDownList = dropDownList;
    textBox.readOnly = readOnly;
    textBox.style.cursor = readOnly ? "default" : "text";
    textBox.style.borderRadius = showImage || !this.allowRoundedControls() ? "0" : "3px 0 0 3px";
    textBox.style.borderRight = "0";
    dropDownList.addCell(textBox).style.fontSize = "0px";

    if (dropDownList.readOnly) {
        textBox.onclick = function () {
            if (!this.isTouchEndFlag && !jsObject.options.isTouchClick) {
                this.dropDownList.button.onclick();
            }
        }
        textBox.ontouchend = function () {
            this.isTouchEndFlag = true;
            clearTimeout(this.isTouchEndTimer);
            this.dropDownList.button.ontouchend();
            this.isTouchEndTimer = setTimeout(function () {
                textBox.isTouchEndFlag = false;
            }, 1000);
        }
    }

    if (showImage) {
        textBox.style.borderLeft = "0px";
    }

    var buttonCellClass = "stiDesignerTextBoxEditButton stiDesignerTextBoxEditButton";
    var buttonCellDefaultClass = buttonCellClass + "Default";
    var buttonCellOverClass = buttonCellClass + "Over";
    var buttonCellDisableClass = buttonCellClass + "Disabled";

    //Edit Button
    if (showEditButton) {
        var editButton = this.SmallButton(name ? name + "EditButton" : null, null, null, "EditButton.png", this.loc.QueryBuilder.Edit);
        editButton.imageCell.style.padding = "0px";
        editButton.innerTable.style.width = "100%";
        editButton.style.width = buttonWidth + "px";
        editButton.style.height = buttonHeight + "px";
        editButton.style.border = "0";
        editButton.style.borderRadius = "0";
        dropDownList.editButton = editButton;

        var editButtonCell = dropDownList.addCell(editButton);
        dropDownList.editButtonCell = editButtonCell;
        editButtonCell.style.fontSize = "0px";
        editButtonCell.className = buttonCellDefaultClass;
        editButtonCell.style.borderLeft = editButtonCell.style.borderRight = "0";
    }

    //Clear Button
    if (showClearButton) {
        var clearButton = this.SmallButton(name ? name + "ClearButton" : null, null, null, "SmallCross.png", this.loc.Gui.monthcalendar_clearbutton, null, null, null, { width: 8, height: 8 });
        clearButton.imageCell.style.padding = "0px";
        clearButton.innerTable.style.width = "100%";
        clearButton.style.width = buttonWidth + "px";
        clearButton.style.height = buttonHeight + "px";
        clearButton.style.border = "0";
        clearButton.style.borderRadius = "0";
        dropDownList.clearButton = clearButton;

        var clearButtonCell = dropDownList.addCell(clearButton);
        dropDownList.clearButtonCell = clearButtonCell;
        clearButtonCell.style.fontSize = "0px";
        clearButtonCell.className = buttonCellDefaultClass;
        clearButtonCell.style.borderLeft = clearButtonCell.style.borderRight = "0";
    }

    //DropDownButton
    var dropDownButton = this.SmallButton(name ? name + "DropDownButton" : null, null, null, "Arrows.SmallArrowDown.png", null, null, (showEditButton || showClearButton) ? "stiDesignerStandartSmallButton" : "stiDesignerDropDownListButton", null, { width: 8, height: 8 });
    dropDownButton.imageCell.style.padding = "0px";
    dropDownButton.innerTable.style.width = "100%";
    dropDownButton.style.width = buttonWidth + "px";
    dropDownButton.style.height = buttonHeight + "px";
    dropDownButton.style.borderRadius = this.allowRoundedControls() ? "0 3px 3px 0" : "0";
    dropDownButton.dropDownList = dropDownList;
    dropDownList.button = dropDownButton;

    if (showEditButton || showClearButton) {
        dropDownButton.style.border = "0";
    }

    var dropDownButtonCell = dropDownList.addCell(dropDownButton);
    dropDownButtonCell.style.fontSize = "0px";
    dropDownButtonCell.style.borderRadius = this.allowRoundedControls() ? "0 3px 3px 0" : "0";

    if (showEditButton || showClearButton) {
        dropDownButtonCell.className = buttonCellDefaultClass;
    }

    //Menu
    dropDownList.menu = this.DropDownListMenu(dropDownList);
    dropDownList.menu.isDinamic = (items == null);
    if (items != null) dropDownList.menu.addItems(items);

    dropDownList.addItems = function (items) {
        dropDownList.items = items;
        dropDownList.menu.addItems(items);
    }

    dropDownList.clear = function () {
        dropDownList.items = [];
        dropDownList.menu.addItems([]);
    }

    dropDownList.getItemByKey = function (key) {
        if (!this.items) return null;
        if (this.items && this.items.length) {
            for (var i = 0; i < this.items.length; i++) {
                if (this.items[i].key == key) return this.items[i];
            }
        }
        return null;
    }

    dropDownList.onmouseover = function () {
        if (!this.jsObject.options.isTouchDevice) this.onmouseenter();
    }

    dropDownList.onmouseenter = function () {
        if (!this.isEnabled || this.jsObject.options.isTouchClick) return;
        this.isOver = true;

        if (!showEditButton && !showClearButton) {
            this.button.isOver = true;
        }

        if (!this.isSelected) {
            if (!showEditButton && !showClearButton) {
                this.button.className = this.button.overClass;
            }
            if (this.imageCell) {
                this.imageCell.className = imageCellBaseClass + "Over";
            }
            if (showEditButton) {
                this.editButtonCell.className = buttonCellOverClass;
            }
            if (showClearButton) {
                this.clearButtonCell.className = buttonCellOverClass;
            }
            if (showEditButton || showClearButton) {
                dropDownButtonCell.className = buttonCellOverClass;
            }
            textBox.className = "stiDesignerTextBox stiDesignerTextBoxOver";
        }

        if (this.jsObject.options.showTooltips && this.toolTip && typeof (this.toolTip) == "object" && !this.isSelected) {
            this.jsObject.options.toolTip.showWithDelay(
                this.toolTip[0],
                this.toolTip[1],
                this.toolTip.length == 3 ? this.toolTip[2].left : this.jsObject.FindPosX(this, "stiDesignerMainPanel"),
                this.toolTip.length == 3 ? this.toolTip[2].top : this.jsObject.options.toolBar.offsetHeight + this.jsObject.options.workPanel.offsetHeight - 1
            );
        }
    }

    dropDownList.onmouseleave = function () {
        if (!this.isEnabled) return;

        this.isOver = false;
        textBox.isOver = false;

        if (!showEditButton && !showClearButton) {
            this.button.isOver = false;
        }

        if (!this.isSelected) {
            if (!showEditButton && !showClearButton) {
                this.button.className = this.button.defaultClass;
            }
            if (this.imageCell) {
                this.imageCell.className = imageCellBaseClass + "Default";
            }
            if (showEditButton) {
                this.editButtonCell.className = buttonCellDefaultClass;
            }
            if (showClearButton) {
                this.clearButtonCell.className = buttonCellDefaultClass;
            }
            if (showEditButton || showClearButton) {
                dropDownButtonCell.className = buttonCellDefaultClass;
            }
            textBox.className = "stiDesignerTextBox stiDesignerTextBoxDefault";
        }

        if (this.jsObject.options.showTooltips && this.toolTip && typeof (this.toolTip) == "object") {
            this.jsObject.options.toolTip.hideWithDelay();
        }
    }

    dropDownList.setEnabled = function (state) {
        this.isEnabled = state;
        this.button.setEnabled(state);
        textBox.setEnabled(state);

        if (showEditButton) {
            this.editButton.setEnabled(state);
            this.editButtonCell.className = dropDownButtonCell.className = state ? buttonCellDefaultClass : buttonCellDisableClass;
        }
        if (showClearButton) {
            this.clearButton.setEnabled(state);
            this.clearButtonCell.className = dropDownButtonCell.className = state ? buttonCellDefaultClass : buttonCellDisableClass;
        }
        if (this.imageCell) {
            this.imageCell.className = imageCellBaseClass + (state ? "Default" : "Disabled");
            this.image.style.visibility = state ? "visible" : "hidden";
        }
    }

    dropDownList.setSelected = function (state) {
        this.isSelected = state;
        textBox.setSelected(state);
        this.button.setSelected(state);

        if (this.imageCell) {
            this.imageCell.className = imageCellBaseClass + (state ? "Selected" : "Default");
        }

        if (!state) {
            if (showEditButton) {
                this.editButtonCell.className = buttonCellDefaultClass;
            }
            if (showClearButton) {
                this.clearButtonCell.className = buttonCellDefaultClass;
            }
            if (showEditButton || showClearButton) {
                dropDownButtonCell.className = buttonCellDefaultClass;
            }
        }
    }

    dropDownList.setKey = function (key) {
        this.key = key;
        if (key == null) return;
        if (key == "StiEmptyValue") {
            textBox.value = "";
            return;
        }
        if (this.items && this.items.length) {
            for (var i = 0; i < this.items.length; i++)
                if (key == this.items[i].key) {
                    textBox.value = this.items[i].caption;
                    if (this.image) StiMobileDesigner.setImageSource(this.image, this.jsObject.options, this.items[i].imageName);
                    return;
                }
        }
        textBox.value = key.toString();
    }

    dropDownList.haveKey = function (key) {
        if (this.items && this.items.length) {
            for (var i = 0; i < this.items.length; i++)
                if (this.items[i].key == key) return true;
        }
        return false;
    }

    dropDownList.setKeyIfExist = function (key) {
        this.setKey(this.haveKey(key) || this.items.length == 0 ? key : this.items[0].key);
    }

    dropDownList.action = function () { }

    //Override methods
    if (!showEditButton && !showClearButton) {
        dropDownButton.onmouseover = null;
        dropDownButton.onmouseleave = null;
        dropDownButton.onmouseenter = null;
        textBox.onmouseover = null;
        textBox.onmouseenter = null;
        textBox.onmouseleave = null;
    }

    textBox.onfocus = function () {
        this.jsObject.options.controlsIsFocused = this;
        dropDownList.hideError();
    }

    textBox.onblur = function () {
        this.isOver = false;
        dropDownList.isOver = false;
        if (!showEditButton && !showClearButton) dropDownButton.isOver = false;
        dropDownList.setSelected(false);
        this.jsObject.options.controlsIsFocused = false;
        dropDownList.hideError();
        this.action();
    }

    textBox.action = function () {
        if (!dropDownList.readOnly) {
            dropDownList.setKey(this.value);
            dropDownList.action();
        }
    }

    dropDownButton.action = function () {
        dropDownList.hideError();
        if (!this.dropDownList.menu.visible) {
            if (this.dropDownList.menu.isDinamic) this.dropDownList.menu.addItems(this.dropDownList.items);
            if (this.jsObject.options.showTooltips && this.dropDownList.toolTip && typeof (this.dropDownList.toolTip) == "object") this.jsObject.options.toolTip.hide();
            this.dropDownList.menu.changeVisibleState(true);
        }
        else
            this.dropDownList.menu.changeVisibleState(false);
    }

    dropDownList.hideError = function () {
        textBox.hideError();
    }

    dropDownList.showError = function (text, thenHide) {
        var img = document.createElement("img");
        StiMobileDesigner.setImageSource(img, this.jsObject.options, "Warning.png");
        img.style.width = img.style.height = "12px";
        img.style.marginLeft = (width + 10) + "px";
        img.style.position = "absolute";
        img.style.marginTop = this.jsObject.options.isTouchDevice ? "7px" : "5px";
        img.title = text;

        if (textBox.parentElement) {
            textBox.hideError();
            textBox.errorImage = img;
            textBox.parentElement.insertBefore(img, textBox);
        }

        var i = 0;
        var intervalTimer = setInterval(function () {
            img.style.display = i % 2 != 0 ? "" : "none";
            i++;
            if (i > 5) {
                clearInterval(intervalTimer);
                if (thenHide) dropDownList.hideError();
            }
        }, 400);
    }

    return dropDownList;
}

StiMobileDesigner.prototype.DropDownListMenu = function (dropDownList) {
    var jsObject = this;
    var menu = this.VerticalMenu(!dropDownList.isTemporary ? dropDownList.name + "Menu" : null, dropDownList.button, "Down", dropDownList.items, null, dropDownList.cutMenu, dropDownList.imageSizes || { width: 32, height: 16 });

    menu.dropDownList = dropDownList;

    if (dropDownList.cutMenu) {
        menu.innerContent.style.width = (dropDownList.fullWidth + 3) + "px";
        menu.innerContent.style.overflowX = "hidden";
    }
    else {
        menu.innerContent.style.minWidth = (dropDownList.fullWidth + 3) + "px";
    }

    menu.changeVisibleState = function (state) {
        if (state) {
            if (!this.parentElement) jsObject.options.mainPanel.appendChild(this);
            this.style.display = "";
            this.visible = true;
            this.onshow();
            this.style.overflow = "hidden";
            this.style.width = this.innerContent.offsetWidth + "px";
            this.style.height = this.innerContent.offsetHeight + "px";
            this.style.left = (jsObject.FindPosX(dropDownList, "stiDesignerMainPanel")) + "px";

            jsObject.options.currentDropDownListMenu = this;
            dropDownList.setSelected(true);

            var browserHeight = (window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight) - jsObject.FindPosY(jsObject.options.mainPanel);

            var animationDirection =
                (jsObject.FindPosY(dropDownList, "stiDesignerMainPanel") + dropDownList.offsetHeight + this.offsetHeight > browserHeight) &&
                    (jsObject.FindPosY(dropDownList, "stiDesignerMainPanel") - this.offsetHeight > 0)
                    ? "Up" : "Down";
            this.style.top = animationDirection == "Down"
                ? (jsObject.FindPosY(dropDownList, "stiDesignerMainPanel") + this.parentButton.offsetHeight + 2) + "px"
                : (jsObject.FindPosY(dropDownList, "stiDesignerMainPanel") - this.offsetHeight) + "px";
            this.innerContent.style.top = (animationDirection == "Down" ? -this.innerContent.offsetHeight : this.innerContent.offsetHeight) + "px";

            var d = new Date();
            var endTime = d.getTime() + jsObject.options.menuAnimDuration;
            jsObject.ShowAnimationVerticalMenu(this, 0, endTime);
        }
        else {
            clearTimeout(this.innerContent.animationTimer);
            this.visible = false;
            dropDownList.setSelected(false);
            this.style.display = "none";
            this.onhide();
            if (jsObject.options.currentDropDownListMenu == this) jsObject.options.currentDropDownListMenu = null;
        }
    }

    menu.onmousedown = function () {
        if (!this.isTouchStartFlag) this.ontouchstart(true);
    }

    menu.ontouchstart = function (mouseProcess) {
        this.isTouchStartFlag = mouseProcess ? false : true;
        clearTimeout(this.isTouchStartTimer);
        jsObject.options.dropDownListMenuPressed = this;
        this.isTouchStartTimer = setTimeout(function () {
            menu.isTouchStartFlag = false;
        }, 1000);
    }

    menu.action = function (menuItem) {
        this.changeVisibleState(false);
        dropDownList.key = menuItem.key;
        dropDownList.textBox.value = menuItem.captInnerCont ? menuItem.captInnerCont.innerHTML : menuItem.caption.innerHTML;
        if (dropDownList.image) StiMobileDesigner.setImageSource(dropDownList.image, jsObject.options, menuItem.imageName);
        dropDownList.action();
    }

    menu.onshow = function () {
        if (dropDownList.key == null) return;

        for (var itemName in this.items) {
            if (dropDownList.key == this.items[itemName].key) {
                this.items[itemName].setSelected(true);
                return;
            }
            else if (itemName.indexOf("separator") != 0) {
                this.items[itemName].setSelected(false);
            }
        }
    }

    return menu;
}

//Data DropDownList
StiMobileDesigner.prototype.DataDropDownList = function (name, toolTip) {
    var dataDropDownList = this.DropDownList(name, 135, toolTip, null, true);

    dataDropDownList.addItems = function (items) {
        this.items = [];
        this.items.push(this.jsObject.Item("NotAssigned", this.jsObject.loc.Report.NotAssigned, null, "[Not Assigned]"));
        if (!items) return;
        for (var i = 0; i < items.length; i++) {
            this.items.push(items[i]);
        }
    }

    dataDropDownList.reset = function () {
        this.setKey("[Not Assigned]");
    }

    return dataDropDownList;
}