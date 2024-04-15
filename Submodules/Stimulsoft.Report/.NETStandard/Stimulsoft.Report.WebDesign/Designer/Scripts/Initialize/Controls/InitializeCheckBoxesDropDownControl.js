
StiMobileDesigner.prototype.CheckBoxesDropDownControl = function (name, width, checkBoxes, showAdditionalButtons) {
    var jsObject = this;
    var control = this.PropertyDropDownList(name, width, null, true, null, null, false);
    control.childControls = {};
    control.menu.isDinamic = false;
    control.menu.innerContent.style.overflowX = "visible";
    control.key = [];
    var labels = {};

    for (var i = 0; i < checkBoxes.length; i++) {
        var checkBox = this.CheckBox(null, checkBoxes[i].label);
        labels[checkBoxes[i].key] = checkBoxes[i].label;
        checkBox.key = checkBoxes[i].key;
        checkBox.style.margin = "8px";
        control.childControls[checkBox.key] = checkBox;
        control.menu.innerContent.appendChild(checkBox);

        checkBox.action = function () {
            var key = [];
            for (var i = 0; i < checkBoxes.length; i++) {
                if (control.childControls[checkBoxes[i].key].isChecked) {
                    key.push(checkBoxes[i].key);
                }
            }
            control.key = key;
            control.textBox.value = control.getValueText(key);
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
                var checkBox = control.childControls[checkBoxes[i].key];
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
        this.textBox.value = this.getValueText(key);
        if (this.textBox.value) this.textBox.setAttribute("title", this.textBox.value);
        for (var i = 0; i < checkBoxes.length; i++) {
            control.childControls[checkBoxes[i].key].setChecked(key.indexOf(checkBoxes[i].key) >= 0);
        }
    }

    control.getValueText = function (key) {
        var text = "";
        for (var i = 0; i < key.length; i++) {
            if (text != "") text += ", ";
            text += labels[key[i]] || key[i];
        }
        return text;
    }

    return control;
}