
StiMobileDesigner.prototype.CollectionColorsComplicatedControl = function (name, toolTip, width, height, predefinedColors) {
    var control = this.DropDownList(name, width, toolTip, [], true, false, height);
    control.menu.innerContent.style.minWidth = "auto";
    control.key = [];
    control.colorsButtons = [];
    if (name) this.options.controls[name] = control;

    var menuContainer = control.menu.innerContent;
    menuContainer.style.maxHeight = "650px";

    var buttonFromStyle = this.StandartSmallButton(null, null, this.loc.FormStyleDesigner.FromStyle);
    buttonFromStyle.style.height = "26px";
    menuContainer.appendChild(buttonFromStyle);

    buttonFromStyle.action = function () {
        control.setKey([]);
        control.action();
        control.menu.changeVisibleState(false);
    }

    menuContainer.appendChild(this.FormBlockHeader(this.loc.FormColorBoxPopup.Custom));

    var butCustColors = this.StandartSmallButton(null, null, this.loc.FormStyleDesigner.NotSpecified, "NoFill.png");
    butCustColors.imageCell.style.padding = "5px 3px 5px 5px";
    butCustColors.style.height = "26px";
    menuContainer.appendChild(butCustColors);

    var buttonEditColors = this.StandartSmallButton(null, null, this.loc.FormStyleDesigner.EditColors);
    buttonEditColors.style.height = "26px";
    menuContainer.appendChild(buttonEditColors);

    butCustColors.setColors = function (colors) {
        this.setSelected(colors && colors.length > 0);

        if (colors && colors.length > 0) {
            this.imageCell.style.display = "none";
            this.caption.innerHTML = "";
            this.caption.style.padding = "0px";

            var colorsTable = this.jsObject.CreateHTMLTable();
            colorsTable.style.margin = "3px 0 3px 0";
            this.caption.appendChild(colorsTable);

            for (var k = 0; k < colors.length; k++) {
                var colorBox = document.createElement("div");
                colorBox.style.width = colorBox.style.height = "16px";
                colorBox.style.margin = "0 5px 0 5px";
                colorBox.style.background = this.jsObject.GetHTMLColor(colors[k]);
                colorsTable.addCell(colorBox);
            }
        }
        else {
            this.caption.innerHTML = this.jsObject.loc.FormStyleDesigner.NotSpecified;
            this.caption.style.padding = "0 5px 0 5px";
            this.imageCell.style.display = "";
        }
    }

    butCustColors.action = buttonEditColors.action = function () {
        this.jsObject.InitializeColorsCollectionForm(function (form) {
            form.action = function () {
                form.changeVisibleState(false);
                var colors = [];
                for (var i = 0; i < form.controls.colorsContainer.childNodes.length; i++) {
                    colors.push(form.controls.colorsContainer.childNodes[i].controls.colorControl.key);
                }
                control.setKey(colors);
                control.action();
            }
            form.show(control.key);
        });

        control.menu.changeVisibleState(false);
    }

    menuContainer.appendChild(this.FormBlockHeader(this.loc.FormStyleDesigner.Predefined));

    if (predefinedColors) {
        for (var i = 0; i < predefinedColors.length; i++) {
            var colors = predefinedColors[i];
            var colorsTable = this.CreateHTMLTable();
            colorsTable.style.margin = "3px 0px 3px 0px";
            var colorsButton = this.StandartSmallButton(null, null, "");
            control.colorsButtons.push(colorsButton);
            colorsButton.style.height = "auto";
            colorsButton.caption.appendChild(colorsTable);
            colorsButton.caption.style.padding = "0px";
            colorsButton.key = colors;

            colorsButton.action = function () {
                control.setKey(this.key);
                control.action();
                control.menu.changeVisibleState(false);
            }

            for (var k = 0; k < colors.length; k++) {
                var colorBox = document.createElement("div");
                colorBox.style.width = colorBox.style.height = "16px";
                colorBox.style.margin = "0 5px 0 5px";
                colorBox.style.background = this.GetHTMLColor(colors[k]);
                colorsTable.addCell(colorBox);
            }

            menuContainer.appendChild(colorsButton);
        }
    }

    control.menu.onshow = function () {
        buttonFromStyle.setSelected(control.key == null || control.key.length == 0);
        butCustColors.setColors(null);

        if (control && control.key.length > 0) {
            var isCustomColors = true;

            for (var i = 0; i < control.colorsButtons.length; i++) {
                control.colorsButtons[i].setSelected(false);
                if (this.jsObject.ArraysEqual(control.colorsButtons[i].key, control.key)) {
                    control.colorsButtons[i].setSelected(true);
                    isCustomColors = false;
                };
            }
            if (isCustomColors) {
                butCustColors.setColors(control.key);
            }
        }
    }

    control.setKey = function (key) {
        this.key = key;
        var text = (key == null || key.length == 0) ? this.jsObject.loc.FormStyleDesigner.FromStyle : this.jsObject.loc.PropertyMain.Colors;
        this.textBox.value = "[" + text + "]";
    }

    control.action = function () { }

    return control;
}