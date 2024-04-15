
StiMobileDesigner.prototype.PropertyShiftModeControl = function (name, width) {
    var shiftModeControl = this.PropertyDropDownList(name, width, null, true);
    shiftModeControl.childControls = {};
    shiftModeControl.menu.isDinamic = false;
    shiftModeControl.menu.innerContent.style.width = null;
    shiftModeControl.menu.innerContent.style.overflowX = "visible";
    var jsObject = this;

    var checkBoxes = ["IncreasingSize", "DecreasingSize", "OnlyInWidthOfComponent"];
    for (var i = 0; i < checkBoxes.length; i++) {
        var checkBox = this.CheckBox(null, this.loc.PropertyEnum["StiShiftMode" + checkBoxes[i]]);
        checkBox.name = checkBoxes[i];
        checkBox.style.margin = "8px";
        shiftModeControl.childControls[checkBoxes[i]] = checkBox;
        shiftModeControl.menu.innerContent.appendChild(checkBox);

        checkBox.action = function () {
            var trueFlag = 0;
            var key = "";
            for (var i = 0; i < checkBoxes.length; i++) {
                if (shiftModeControl.childControls[checkBoxes[i]].isChecked) {
                    trueFlag++;
                    if (key != "") key += ", ";
                    key += checkBoxes[i];
                }
            }
            if (trueFlag == 0) key = "None";
            if (trueFlag == checkBoxes.length) key = "All";
            shiftModeControl.key = key;
            shiftModeControl.textBox.value = shiftModeControl.translateKey(key);
            shiftModeControl.action();
        }
    }

    shiftModeControl.setKey = function (key) {
        this.key = key;
        this.textBox.value = key != "StiEmptyValue" ? this.translateKey(key) : "";
        if (this.textBox.value) this.textBox.setAttribute("title", this.textBox.value);
        for (var i = 0; i < checkBoxes.length; i++) {
            shiftModeControl.childControls[checkBoxes[i]].setChecked(key != "StiEmptyValue" ? (key.indexOf(checkBoxes[i]) >= 0 || key == "All") : false);
        }
    }

    shiftModeControl.translateKey = function (key) {
        var notLocalizeValues = this.ownerIsProperty && jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.localizePropertyGrid;

        if (key == "All") {
            var result = "";
            for (var i = 0; i < checkBoxes.length; i++) {
                if (i != 0) result += ", ";
                result += (notLocalizeValues ? checkBoxes[i] : jsObject.loc.PropertyEnum["StiShiftMode" + checkBoxes[i]]);
            }
            return result;
        }
        if (key == "None") return (notLocalizeValues ? "None" : jsObject.loc.PropertyEnum["StiShiftModeNone"]);

        for (var i = 0; i < checkBoxes.length; i++) {
            if (!notLocalizeValues) {
                key = key.replace(checkBoxes[i], jsObject.loc.PropertyEnum["StiShiftMode" + checkBoxes[i]]);
            }
        }

        return key;
    }

    return shiftModeControl;
}