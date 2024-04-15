
StiMobileDesigner.prototype.ThemeColorButton = function (color, name, groupName, value) {
    var button = $("<div class='stiThemeColorButton' style='background-color:" + color + "'><div class='stiThemeColorButtonIn'></div></div>")[0];
    button.jsObject = this;
    button.name = name != null ? name : this.generateKey();
    button.id = button.name;
    if (name != null) this.options.buttons[name] = button;
    button.groupName = groupName;
    button.isSelected = false;
    button.value = value;
    button.options = this.options;
    button.color = color;

    button.onclick = function () {
        if (button.groupName) {
            for (var bName in this.options.buttons) {
                if (this.options.buttons[bName].groupName == button.groupName) {
                    this.options.buttons[bName].setSelected(false);
                }
            }
        }
        this.setSelected(true);
    }

    button.setSelected = function (selected) {
        button.isSelected = selected;
        button.style.borderColor = selected ? "#f29436" : "";
        button.firstChild.style.border = selected ? "1px solid #ffe294" : "";
    }

    return button;
}
