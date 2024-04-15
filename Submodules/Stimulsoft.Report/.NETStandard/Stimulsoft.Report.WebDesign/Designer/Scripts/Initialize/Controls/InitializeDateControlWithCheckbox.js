
StiMobileDesigner.prototype.DateControlWithCheckBox = function (name, width, toolTip) {
    var control = this.CreateHTMLTable();
    control.key = new Date();
    control.isChecked = true;
    control.isEnabled = true;

    var dateControl = control.dateControl = this.DateControl(name, width, toolTip);
    dateControl.parentControl = control;
    control.addCell(dateControl);

    dateControl.action = function () {
        this.parentControl.action();
    }

    dateControl.setKey = function (key) {
        this.key = key;
        this.parentControl.key = key;
        this.textBox.value = this.parentControl.dateTimeFormat == "Time"
            ? key.toLocaleTimeString()
            : this.parentControl.dateTimeFormat == "Date"
                ? key.toLocaleDateString()
                : (this.parentControl.shortFormat ? key.toLocaleDateString() : key.toLocaleString());
    }

    var checkBox = control.checkBox = this.CheckBox(name + "CheckBox", this.loc.Report.NotAssigned);
    checkBox.style.marginLeft = "5px";
    checkBox.parentControl = control;
    control.addCell(checkBox);

    checkBox.action = function () {
        dateControl.setEnabled(!this.isChecked);
    }

    checkBox.setChecked = function (state) {
        this.image.style.visibility = (state) ? "visible" : "hidden";
        this.isChecked = state;
        this.parentControl.isChecked = state;
    }

    control.setEnabled = function (state) {
        checkBox.setEnabled(state);
        dateControl.setEnabled(state);
        this.isEnabled = state;
    }

    control.setKey = function (key) {
        this.key = key;
        dateControl.setKey(key);
    }

    control.setChecked = function (state) {
        checkBox.setChecked(state)
        this.isChecked = state;
        dateControl.setEnabled(!this.isChecked);
    }

    control.setChecked(false);

    control.action = function () { }

    return control;
}

StiMobileDesigner.prototype.DateControlWithCheckBox2 = function (name, width, toolTip) {
    var control = this.CreateHTMLTable();
    control.key = new Date();
    control.isChecked = true;
    control.isEnabled = true;
    if (name != null) this.options.controls[name] = control;

    var checkBox = control.checkBox = this.CheckBox(name + "CheckBox");
    checkBox.parentControl = control;
    control.addCell(checkBox);

    checkBox.setChecked = function (state) {
        this.image.style.visibility = (state) ? "visible" : "hidden";
        this.isChecked = state;
        this.parentControl.isChecked = state;
    }

    var dateControl = control.dateControl = this.DateControl(name, width, toolTip);
    dateControl.parentControl = control;
    control.addCell(dateControl).style.paddingLeft = "4px";

    dateControl.action = function () {
        this.parentControl.action();
    }

    dateControl.setKey = function (key) {
        this.key = key;
        this.parentControl.key = key;
        this.textBox.value = key.toLocaleString();
    }

    checkBox.action = function () {
        dateControl.setEnabled(this.isChecked);
    }

    control.setEnabled = function (state) {
        checkBox.setEnabled(state);
        dateControl.setEnabled(state && this.isChecked);
        this.isEnabled = state;
    }

    control.setKey = function (key) {
        this.key = key;
        dateControl.setKey(key);
    }

    control.setChecked = function (state) {
        checkBox.setChecked(state)
        this.isChecked = state;
        dateControl.setEnabled(this.isChecked);
    }

    control.setChecked(true);

    control.action = function () { }

    return control;
}