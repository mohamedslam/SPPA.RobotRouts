
StiMobileDesigner.prototype.CheckBoxesControl = function (name, caption, imageName, checkBoxes, withBorders) {
    var control = this.CreateHTMLTable();
    control.isEnabled = true;
    control.key = "None";

    var button = control.button = this.SmallButton(name, null, caption, imageName, null, "Down", withBorders ? "stiDesignerSmallButtonWithBorder" : "stiDesignerStandartSmallButton", true);
    control.addCell(button);

    if (withBorders) button.style.height = (this.options.controlsHeight - 2) + "px";

    var menu = control.menu = this.VerticalMenu(name ? name + "Menu" : null, button, "Down");

    button.action = function () {
        menu.changeVisibleState(!control.menu.visible);
    }

    control.childControls = {};
    menu.isDinamic = false;
    menu.innerContent.style.width = null;
    menu.innerContent.style.overflowX = "visible";

    for (var i = 0; i < checkBoxes.length; i++) {
        var checkBox = this.CheckBox(null, checkBoxes[i][1]);
        checkBox.name = checkBoxes[i][0];
        checkBox.style.margin = "8px";
        control.childControls[checkBoxes[i][0]] = checkBox;
        menu.innerContent.appendChild(checkBox);

        checkBox.action = function () {
            var trueFlag = 0;
            var key = "";
            for (var i = 0; i < checkBoxes.length; i++) {
                if (control.childControls[checkBoxes[i][0]].isChecked) {
                    trueFlag++;
                    if (key != "") key += ", ";
                    key += checkBoxes[i][0];
                }
            }
            if (trueFlag == 0) key = "None";
            if (trueFlag == checkBoxes.length) key = "All";
            control.key = key;
            control.action();
        }
    }

    control.setKey = function (key) {
        this.key = key;
        for (var i = 0; i < checkBoxes.length; i++) {
            var checked = (key == "All" || key == checkBoxes[i][0] || key.indexOf(checkBoxes[i][0] + ",") == 0 ||
                key.indexOf(", " + checkBoxes[i][0] + ",") >= 0 || this.jsObject.EndsWith(key, checkBoxes[i][0]));
            control.childControls[checkBoxes[i][0]].setChecked(checked);
        }
    }

    control.setEnabled = function (state) {
        control.isEnabled = state;
        button.setEnabled(state);
    }

    control.action = function () { }

    return control;
}