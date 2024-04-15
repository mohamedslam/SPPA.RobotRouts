
StiMobileDesigner.prototype.IndicatorsControl = function (name, width) {
    var control = this.CreateHTMLTable();
    control.isEnabled = true;
    control.key = "None";
    control.childControls = {};

    var button = this.SmallButton(null, null, null, " ", null, "Down", "stiDesignerFormButton", true);
    button.style.width = width ? width + "px" : "50px";
    button.innerTable.style.width = "100%";
    button.imageCell.style.width = "100%";
    button.image.style.display = "none";
    control.addCell(button);

    var menu = this.VerticalMenu(name + "Menu", button, "Down");
    button.action = function () { menu.changeVisibleState(!menu.visible); }

    var noneButton = this.StandartSmallButton(null, null, this.loc.PropertyMain.NoIcon);
    control.childControls["None"] = noneButton;
    menu.innerContent.appendChild(noneButton);
    menu.innerContent.appendChild(this.VerticalMenuSeparator(menu, "separator"));
    noneButton.action = function () {
        control.setKey("None");
        control.action();
        menu.changeVisibleState(false);
    }

    menu.isDinamic = false;
    menu.innerContent.style.width = null;
    menu.innerContent.style.overflowX = "visible";
    menu.innerContent.style.maxHeight = "500px";

    var table = this.CreateHTMLTable();
    table.style.margin = "3px";
    menu.innerContent.appendChild(table);

    var keys = ["ArrowUpGreen", "ArrowRightYellow", "ArrowDownRed", "ArrowUpGray", "ArrowRightGray", "ArrowDownGray", "ArrowRightUpYellow", "ArrowRightDownYellow", "ArrowRightUpGray",
        "ArrowRightDownGray", "TriangleGreen", "MinusYellow", "TriangleRed", "FlagGreen", "FlagYellow", "FlagRed", "Latin1", "Latin2", "Latin3", "Latin4", "CheckGreen", "ExclamationYellow",
        "CrossRed", "CircleCheckGreen", "CircleExclamationYellow", "CircleCrossRed", "CircleGreen", "CircleYellow", "CircleRed", "CircleBlack", "TriangleYellow", "RhombRed",
        "FromRedToBlackRed", "FromRedToBlackPink", "FromRedToBlackGray", "LightsGreen", "LightsYellow", "LightsRed", "QuarterFull", "QuarterThreeFourth", "QuarterHalf",
        "QuarterQuarter", "QuarterNone", "QuarterFullGreen", "QuarterThreeFourthGreen", "QuarterHalfGreen", "QuarterQuarterGreen", "QuarterNoneGreen", "QuarterFullRed",
        "QuarterThreeFourthRed", "QuarterHalfRed", "QuarterQuarterRed", "QuarterNoneRed", "Rating0", "Rating1", "Rating2", "Rating3", "Rating4", "Square0", "Square1",
        "Square2", "Square3", "Square4", "StarFull", "StarThreeFourth", "StarHalf", "StarQuarter", "StarNone"
    ];

    for (var i = 0; i < keys.length; i++) {
        var innerButton = this.SmallButton(null, null, null, "Indicators." + keys[i] + ".png", null, null, null, true);
        innerButton.innerTable.style.width = "100%";
        innerButton.style.margin = "3px";
        control.childControls[keys[i]] = innerButton;
        innerButton.key = keys[i];
        var cell = (i % 8 == 0) ? table.addCellInNextRow() : table.addCellInLastRow();
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
        noneButton.setSelected(key == "None");
        button.image.style.display = key != "None" ? "" : "none";
        if (key != "None") StiMobileDesigner.setImageSource(button.image, this.jsObject.options, "Indicators." + key + ".png");
    }

    control.setEnabled = function (state) {
        control.isEnabled = state;
        button.setEnabled(state);
    }

    control.action = function () { }

    return control;
}