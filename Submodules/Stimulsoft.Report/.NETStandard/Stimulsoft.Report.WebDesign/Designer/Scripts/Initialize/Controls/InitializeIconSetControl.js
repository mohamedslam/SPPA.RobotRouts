
StiMobileDesigner.prototype.IconSetControl = function (name, width) {
    var control = this.CreateHTMLTable();
    control.isEnabled = true;
    control.key = "None";
    control.childControls = {};

    var button = this.SmallButton(null, null, null, " ", null, "Down", "stiDesignerFormButton", true, { width: 120, height: 16 });
    button.style.width = width ? width + "px" : "170px";

    button.innerTable.style.width = "100%";
    button.imageCell.style.width = "100%";
    button.image.style.display = "none";

    control.addCell(button);

    var menu = this.VerticalMenu(name + "Menu", button, "Down");
    button.action = function () { menu.changeVisibleState(!menu.visible); }

    menu.isDinamic = false;
    menu.innerContent.style.width = null;
    menu.innerContent.style.overflowX = "visible";
    menu.innerContent.style.maxHeight = "500px";
    var table = this.CreateHTMLTable();
    table.style.margin = "3px";
    menu.innerContent.appendChild(table);

    var keys = ["ArrowsColored3", "ArrowsGray3", "ArrowsColored4", "ArrowsGray4", "ArrowsColored5", "ArrowsGray5", "Triangles3", "Flags3", "SymbolsUncircled3", "SymbolsCircled3",
        "TrafficLightsUnrimmed3", "TrafficLightsRimmed3", "TrafficLights4", "Signs3", "RedToBlack4", "Ratings3", "Quarters5", "Ratings4", "QuartersGreen5", "Ratings5",
        "QuartersRed5", "Stars3", "Squares5", "Stars5", "Latin4"];

    for (var i = 0; i < keys.length; i++) {
        var innerButton = this.SmallButton(null, null, null, "IconSets." + keys[i] + ".png", null, null, null, true, { width: 120, height: 16 });
        innerButton.innerTable.style.width = "100%";
        innerButton.style.width = "125px";
        innerButton.style.margin = "3px";
        control.childControls[keys[i]] = innerButton;
        innerButton.key = keys[i];
        var cell = (i % 2 == 0) ? table.addCellInNextRow() : table.addCellInLastRow();
        cell.appendChild(innerButton);

        innerButton.action = function () {
            control.setKey(this.key);
            control.action();
            menu.changeVisibleState(false);
        }
    }

    control.setKey = function (key) {
        this.key = key;
        for (var i = 0; i < keys.length; i++) {
            control.childControls[keys[i]].setSelected(keys[i] == key);
        }
        button.image.style.display = key != "None" ? "" : "none";
        if (key != "None") StiMobileDesigner.setImageSource(button.image, this.jsObject.options, "IconSets." + key + ".png");
    }

    control.setEnabled = function (state) {
        control.isEnabled = state;
        button.setEnabled(state);
    }

    control.action = function () { }

    return control;
}