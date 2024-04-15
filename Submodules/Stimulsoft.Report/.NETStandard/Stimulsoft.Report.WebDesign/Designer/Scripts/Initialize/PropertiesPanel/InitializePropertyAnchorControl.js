
StiMobileDesigner.prototype.PropertyAnchorControl = function (name, width) {
    var anchorControl = this.PropertyDropDownList(name, width, null, true, null, null, false);
    anchorControl.childControls = {};
    anchorControl.menu.isDinamic = false;
    anchorControl.menu.innerContent.style.overflowX = "visible";
    var jsObject = this;

    var checkBoxes = ["Top", "Bottom", "Left", "Right"];
    for (var i = 0; i < checkBoxes.length; i++) {
        var checkBox = this.CheckBox(null, this.loc.PropertyMain[checkBoxes[i]]);
        checkBox.name = checkBoxes[i];
        checkBox.style.margin = "8px";
        anchorControl.childControls[checkBoxes[i]] = checkBox;
        anchorControl.menu.innerContent.appendChild(checkBox);

        checkBox.action = function () {
            anchorControl.updateControlStates();
            var key = "";
            for (var i = 0; i < checkBoxes.length; i++) {
                if (anchorControl.childControls[checkBoxes[i]].isChecked) {
                    if (key != "") key += ", ";
                    key += checkBoxes[i];
                }
            }
            anchorControl.key = key;
            anchorControl.textBox.value = anchorControl.translateKey(key);
            anchorControl.action();
        }
    }

    anchorControl.updateControlStates = function () {
        if (!anchorControl.childControls.Bottom.isChecked) anchorControl.childControls.Top.setChecked(true);
        anchorControl.childControls.Top.setEnabled(anchorControl.childControls.Bottom.isChecked);
        if (!anchorControl.childControls.Right.isChecked) anchorControl.childControls.Left.setChecked(true);
        anchorControl.childControls.Left.setEnabled(anchorControl.childControls.Right.isChecked);
    }

    anchorControl.setKey = function (key) {
        this.key = key;
        this.textBox.value = key != "StiEmptyValue" ? this.translateKey(key) : "";
        if (this.textBox.value) this.textBox.setAttribute("title", this.textBox.value);
        for (var i = 0; i < checkBoxes.length; i++) {
            anchorControl.childControls[checkBoxes[i]].setChecked(key != "StiEmptyValue" ? (key.indexOf(checkBoxes[i]) >= 0) : false);
        }
        anchorControl.updateControlStates();
    }

    anchorControl.translateKey = function (key) {
        var notLocalizeValues = this.ownerIsProperty && jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.localizePropertyGrid;

        for (var i = 0; i < checkBoxes.length; i++) {
            if (!notLocalizeValues) {
                key = key.replace(checkBoxes[i], jsObject.loc.PropertyMain[checkBoxes[i]]);
            }
        }

        return key;
    }

    return anchorControl;
}