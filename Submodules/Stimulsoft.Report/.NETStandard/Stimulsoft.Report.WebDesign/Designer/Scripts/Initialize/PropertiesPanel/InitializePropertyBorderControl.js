//Border Control
StiMobileDesigner.prototype.PropertyBorderControl = function (name, width) {
    var jsObject = this;
    var borderControl = this.PropertyTextBoxWithEditButton(name, width, true);
    borderControl.key = null;

    borderControl.button.action = function () {
        jsObject.InitializeBorderSetupForm(function (borderSetupForm) {

            borderSetupForm.actionFunction = function (border) {
                borderSetupForm.finishFlag = true;
                borderSetupForm.changeVisibleState(false);
                borderControl.setKey(jsObject.BordersObjectToStr(border));
                borderControl.action();
            }

            borderSetupForm.showFunction = function () {
                borderSetupForm.border = borderControl.key;
            }

            borderSetupForm.changeVisibleState(true);
        });
    }

    borderControl.setKey = function (key) {
        this.key = key;
        this.textBox.value = jsObject.BorderObjectToShotStr(jsObject.BordersStrToObject(key));
    }

    borderControl.action = function () { }

    return borderControl;
}

StiMobileDesigner.prototype.PropertyComplexBorderControl = function (name, width) {
    var jsObject = this;
    var control = this.CreateHTMLTable();
    if (!name) name = this.generateKey();
    control.name = name;
    control.key = null;
    control.isEnabled = true;
    this.options.controls[name] = control;

    var button = this.SmallButton(name + "Button", null, " ", null, null, "Down", "stiDesignerPropertiesBrushControlButton");
    button.style.width = (width + 4) + "px";
    button.caption.style.width = "100%";
    control.addCell(button);

    var menu = this.BaseMenu(name + "Menu", button, "Down");
    menu.innerContent.style.minWidth = width + "px";

    button.action = function () {
        menu.changeVisibleState(!menu.visible);
    }

    var defStateButton = this.StandartSmallButton(null, null, this.loc.Report.FromDefaultState);
    menu.innerContent.appendChild(defStateButton);

    var currStateButton = this.StandartSmallButton(null, null, this.loc.Report.FromCurrentState);
    currStateButton.style.width = currStateButton.innerTable.style.width = "100%";
    menu.innerContent.appendChild(currStateButton);

    var editButton = this.StandartSmallButton(null, null, null, "EditButton.png", this.loc.QueryBuilder.Edit);
    editButton.image.style.opacity = "0.8";
    currStateButton.innerTable.addCell(editButton).style.width = "1px";

    editButton.onmouseenter = function () {
        this.image.style.opacity = "1";
    }

    editButton.onmouseleave = function () {
        this.image.style.opacity = "0.8";
    }

    defStateButton.action = function () {
        control.setKey("");
        control.action();
        menu.changeVisibleState(false);
    }

    currStateButton.action = function () {
        menu.changeVisibleState(false);

        jsObject.InitializeBorderSetupForm(function (form) {
            form.actionFunction = function (border) {
                form.finishFlag = true;
                form.changeVisibleState(false);
                control.setKey(jsObject.BordersObjectToStr(border));
                control.action();
            }

            form.showFunction = function () {
                form.border = control.key || "0,0,0,0!1!128,128,128!0";
            }

            form.changeVisibleState(true);
        });
    }

    control.setKey = function (key) {
        this.key = key;
        button.caption.innerText = key == "" ? jsObject.loc.Report.FromDefaultState : jsObject.BorderObjectToShotStr(jsObject.BordersStrToObject(key));
        defStateButton.setSelected(key == "");
        currStateButton.setSelected(key != "");
    }

    control.setEnabled = function (state) {
        button.setEnabled(state);
    }

    control.action = function () { }

    return control;
}