
StiMobileDesigner.prototype.CollectionControl = function (name, checkBoxes, width, height, headerText, showAdditionalButtons) {
    var control = this.CreateHTMLTable();
    control.isEnabled = true;
    control.captionAll = this.loc.PropertyEnum.StiRestrictionsAll;
    control.captionNone = this.loc.PropertyEnum.StiRestrictionsNone;
    control.childControls = {};

    var button = control.button = this.SmallButton(null, null, control.captionNone, null, null, "Down", "stiDesignerSmallButtonWithBorder", true);
    button.style.height = (height || (this.options.controlsHeight - 2)) + "px";
    button.innerTable.style.width = "100%";
    if (width) button.style.width = width + "px";
    control.addCell(button);

    //Override Button
    var newCaption = document.createElement("div");
    newCaption.setAttribute("style", "text-overflow: ellipsis; text-align: center; overflow: hidden; white-space: nowrap; padding-left: 5px; width: " + (width - (this.options.isTouchDevice ? 40 : 30)) + "px;");
    newCaption.innerHTML = control.captionNone;

    button.arrow.style.marginBottom = "0px";
    button.arrowCell.style.width = "1px";
    button.caption.innerHTML = "";
    button.caption.appendChild(newCaption);
    button.caption = newCaption;

    var menu = control.menu = this.VerticalMenu(name + "Menu", button, "Down");
    menu.isDinamic = true;
    menu.innerContent.style.width = null;    
    menu.innerContent.style.minWidth = width ? width + "px" : null;
    menu.innerContent.style.maxHeight = "";

    button.action = function () {
        menu.changeVisibleState(!menu.visible);
    }

    if (headerText) {
        menu.innerContent.appendChild(this.FormBlockHeader(headerText));
    }

    var itemsContainer = document.createElement("div");
    itemsContainer.style.overflowY = "auto";
    itemsContainer.style.overflowX = menu.innerContent.style.overflowX = menu.innerContent.style.overflowY = "visible";
    itemsContainer.style.maxHeight = "380px";
    menu.innerContent.appendChild(itemsContainer);

    for (var i = 0; i < checkBoxes.length; i++) {
        var checkBox = this.CheckBox(null, checkBoxes[i][1]);
        checkBox.name = checkBoxes[i][0];
        checkBox.style.margin = "8px";
        control.childControls[checkBoxes[i][0]] = checkBox;
        itemsContainer.appendChild(checkBox);

        checkBox.action = function () {
            var trueFlag = 0;
            var key = "";
            var captionText = "";

            for (var i = 0; i < checkBoxes.length; i++) {
                if (control.childControls[checkBoxes[i][0]].isChecked) {
                    trueFlag++;
                    key += " " + checkBoxes[i][0] + ",";
                    captionText += " " + checkBoxes[i][1] + ",";
                }
            }
            if (trueFlag == 0) {
                button.caption.innerHTML = control.captionNone;
            }
            else if (trueFlag == checkBoxes.length) {
                button.caption.innerHTML = control.captionAll;
            }
            else {
                button.caption.innerHTML = captionText != "" ? captionText.substring(1, captionText.length - 1) : captionText;
            }
            control.key = key;
            control.action();
        }
    }

    if (showAdditionalButtons) {
        var buttonsTable = this.CreateHTMLTable();
        buttonsTable.setAttribute("align", "right");
        control.menu.innerContent.appendChild(buttonsTable);

        var selectAllButton = this.FormButton(null, null, this.loc.Dashboard.SelectAll.replace("&", "").replace("(", "").replace(")", ""));
        var removeAllButton = this.FormButton(null, null, this.loc.Buttons.RemoveAll);

        selectAllButton.style.margin = "8px";
        removeAllButton.style.margin = "8px 8px 8px 0";

        buttonsTable.addCell(selectAllButton);
        buttonsTable.addCell(removeAllButton);

        selectAllButton.action = function () {
            for (var i = 0; i < checkBoxes.length; i++) {
                var checkBox = control.childControls[checkBoxes[i][0]];
                checkBox.setChecked(true);
                if (i == checkBoxes.length - 1) checkBox.action();
            }
        }

        removeAllButton.action = function () {
            control.setKey([]);
            control.action();
        }
    }

    control.setKey = function (key) {
        this.key = key;
        var trueFlag = 0;
        var captionText = "";

        for (var i = 0; i < checkBoxes.length; i++) {
            var checked = key.indexOf(" " + checkBoxes[i][0] + ",") >= 0;
            control.childControls[checkBoxes[i][0]].setChecked(checked);
            if (checked) {
                trueFlag++;
                captionText += " " + checkBoxes[i][1] + ",";
            }
        }
        if (trueFlag == 0) {
            button.caption.innerHTML = control.captionNone;
        }
        else if (trueFlag == checkBoxes.length) {
            button.caption.innerHTML = control.captionAll;
        }
        else {
            button.caption.innerHTML = captionText != "" ? captionText.substring(1, captionText.length - 1) : captionText;
        }
    }

    control.setEnabled = function (state) {
        control.isEnabled = state;
        button.setEnabled(state);
    }

    control.action = function () { }

    return control;
}

StiMobileDesigner.prototype.StylePlacementCollectionControl = function (name, checkBoxes, width, height, headerText, showAdditionalButtons) {
    var control = this.CollectionControl(name, checkBoxes, width, height, headerText, showAdditionalButtons);
    var button = control.button;

    for (var i = 0; i < checkBoxes.length; i++) {
        var checkBox = control.childControls[checkBoxes[i][0]];

        if (checkBox.name == "DataEvenStyle" || checkBox.name == "DataOddStyle") {
            checkBox.style.marginLeft = "15px";
        }

        checkBox.action = function () {
            var trueFlag = 0;
            var key = "";
            var captionText = "";

            if (this.name == "DataEvenStyle" || this.name == "DataOddStyle") {
                for (var i = 0; i < checkBoxes.length; i++) {
                    control.childControls[checkBoxes[i][0]].setChecked(this.name == checkBoxes[i][0]);
                }
                key += " " + this.name + ",";
                captionText += " " + this.captionCell.innerHTML + ",";
                trueFlag = 1;
            }
            else {
                control.childControls["DataEvenStyle"].setChecked(false);
                control.childControls["DataOddStyle"].setChecked(false);

                for (var i = 0; i < checkBoxes.length; i++) {
                    if (control.childControls[checkBoxes[i][0]].isChecked) {
                        trueFlag++;
                        key += " " + checkBoxes[i][0] + ",";
                        captionText += " " + checkBoxes[i][1] + ",";
                    }
                }

                if (trueFlag == checkBoxes.length - 2) {
                    button.caption.innerHTML = captionText != "" ? captionText.substring(1, captionText.length - 1) : captionText;
                    control.key = " AllExeptStyles,";
                    control.action();
                    return;
                }
            }

            if (trueFlag == 0) {
                button.caption.innerHTML = control.captionNone;
            }
            else {
                button.caption.innerHTML = captionText != "" ? captionText.substring(1, captionText.length - 1) : captionText;
            }

            control.key = key;
            control.action();
        }
    }

    control.setKey = function (key) {
        this.key = key;
        var captionText = "";

        if (key == " AllExeptStyles,") {
            for (var i = 0; i < checkBoxes.length; i++) {
                if (checkBoxes[i][0] != "DataEvenStyle" && checkBoxes[i][0] != "DataOddStyle") {
                    captionText += " " + checkBoxes[i][1] + ",";
                    control.childControls[checkBoxes[i][0]].setChecked(true);
                }
                else {
                    control.childControls[checkBoxes[i][0]].setChecked(false);
                }
            }
            button.caption.innerHTML = captionText != "" ? captionText.substring(1, captionText.length - 1) : captionText;
        }
        else {
            var trueFlag = 0;
            for (var i = 0; i < checkBoxes.length; i++) {
                var checked = key.indexOf(" " + checkBoxes[i][0] + ",") >= 0;
                control.childControls[checkBoxes[i][0]].setChecked(checked);
                if (checked) {
                    trueFlag++;
                    captionText += " " + checkBoxes[i][1] + ",";
                }
            }
            if (trueFlag == 0) {
                button.caption.innerHTML = control.captionNone;
            }
            else {
                button.caption.innerHTML = captionText != "" ? captionText.substring(1, captionText.length - 1) : captionText;
            }
        }
    }

    return control;
}

