
StiMobileDesigner.prototype.PropertyRestrictionsControl = function (name, width) {
    var restrictionsControl = this.PropertyDropDownList(name, width, null, true, null, null, false);
    restrictionsControl.childControls = {};
    restrictionsControl.menu.isDinamic = false;
    restrictionsControl.menu.innerContent.style.overflowX = "visible";
    var jsObject = this;

    var checkBoxes = ["AllowMove", "AllowResize", "AllowSelect", "AllowChange", "AllowDelete"];
    for (var i = 0; i < checkBoxes.length; i++) {
        var checkBox = this.CheckBox(null, this.loc.PropertyEnum["StiRestrictions" + checkBoxes[i]]);
        checkBox.name = checkBoxes[i];
        checkBox.style.margin = "8px";
        restrictionsControl.childControls[checkBoxes[i]] = checkBox;
        restrictionsControl.menu.innerContent.appendChild(checkBox);

        checkBox.action = function () {
            var trueFlag = 0;
            var key = "";
            for (var i = 0; i < checkBoxes.length; i++) {
                if (restrictionsControl.childControls[checkBoxes[i]].isChecked) {
                    trueFlag++;
                    if (key != "") key += ", ";
                    key += checkBoxes[i];
                }
            }
            if (trueFlag == 0) key = "None";
            if (trueFlag == checkBoxes.length) key = "All";
            restrictionsControl.key = key;
            restrictionsControl.textBox.value = restrictionsControl.translateKey(key);
            restrictionsControl.action();
        }
    }

    restrictionsControl.setKey = function (key) {
        this.key = key;
        this.textBox.value = key != "StiEmptyValue" ? this.translateKey(key) : "";
        if (this.textBox.value) this.textBox.setAttribute("title", this.textBox.value);
        for (var i = 0; i < checkBoxes.length; i++) {
            restrictionsControl.childControls[checkBoxes[i]].setChecked(key != "StiEmptyValue" ? (key.indexOf(checkBoxes[i]) >= 0 || key == "All") : false);
        }
    }

    restrictionsControl.translateKey = function (key) {
        var notLocalizeValues = this.ownerIsProperty && jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.localizePropertyGrid;

        if (key == "All") return notLocalizeValues ? "All" : jsObject.loc.PropertyEnum["StiRestrictionsAll"];
        if (key == "None") return notLocalizeValues ? "None" : jsObject.loc.PropertyEnum["StiRestrictionsNone"];

        for (var i = 0; i < checkBoxes.length; i++) {
            if (!notLocalizeValues) {
                key = key.replace(checkBoxes[i], jsObject.loc.PropertyEnum["StiRestrictions" + checkBoxes[i]]);
            }
        }

        return key;
    }

    return restrictionsControl;
}