
StiMobileDesigner.prototype.FontControl = function (name, width) {
    var control = this.CreateHTMLTable();
    if (!name) name = this.generateKey();
    control.name = name;
    control.key = null;
    control.isEnabled = true;
    this.options.controls[name] = control;

    var button = this.SmallButton(name + "Button", null, this.loc.FormConditions.ChangeFont.replace("...", ""), null, null, "Down", "stiDesignerPropertiesBrushControlButton");
    button.style.height = (this.options.controlsHeight - 2) + "px";
    button.style.width = (width || 100) + "px";
    button.caption.style.width = "100%";
    button.caption.style.textAlign = "center";
    control.addCell(button);

    var menu = this.BaseMenu(name + "Menu", button, "Down");
    menu.innerContent.style.minWidth = width + "px";

    button.action = function () {
        menu.changeVisibleState(!menu.visible);
    }

    var fontControl = control.innerControl = this.PropertyFontControl(name + "FontControl", null, true);
    fontControl.style.margin = "6px";
    menu.innerContent.appendChild(fontControl);

    fontControl.action = function () {
        control.setKey(this.key);
        control.action();
    }

    control.setKey = function (key) {
        this.key = key;
        fontControl.setKey(key || "Arial!10!0!0!0!0");
    }

    control.setEnabled = function (state) {
        button.setEnabled(state);
    }

    control.action = function () { }

    return control;
}