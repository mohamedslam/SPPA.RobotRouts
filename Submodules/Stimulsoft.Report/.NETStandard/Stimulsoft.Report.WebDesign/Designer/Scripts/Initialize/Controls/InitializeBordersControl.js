
StiMobileDesigner.prototype.BordersControl = function (name, width) {
    var bControl = this.CreateHTMLTable();
    if (!name) name = this.generateKey();
    bControl.name = name;
    bControl.key = null;
    bControl.isEnabled = true;
    this.options.controls[name] = bControl;
    var controls = {};

    var button = this.SmallButton(name + "Button", null, this.loc.PropertyMain.Border, null, null, "Down", "stiDesignerPropertiesBrushControlButton");
    button.style.height = (this.options.controlsHeight - 2) + "px";
    button.style.width = (width || 100) + "px";
    button.caption.style.width = "100%";
    button.caption.style.textAlign = "center";
    bControl.addCell(button);

    var menu = this.BaseMenu(name + "Menu", button, "Down");
    menu.innerContent.style.minWidth = width + "px";

    button.action = function () {
        menu.changeVisibleState(!menu.visible);
    }

    var controlProps = [
        ["BorderAll", this.StandartSmallButton(null, null, this.loc.PropertyEnum.StiBorderSidesAll, "BorderAll.png"), "4px 4px 2px 4px"],
        ["BorderNone", this.StandartSmallButton(null, null, this.loc.PropertyEnum.StiBorderSidesNone, "BorderNone.png"), "2px 4px 4px 4px"],
        ["separator"],
        ["BorderLeft", this.StandartSmallButton(null, null, this.loc.PropertyEnum.StiBorderSidesLeft, "BorderLeft.png"), "4px 4px 4px 2px"],
        ["BorderTop", this.StandartSmallButton(null, null, this.loc.PropertyEnum.StiBorderSidesTop, "BorderTop.png"), "2px 4px 2px 4px"],
        ["BorderRight", this.StandartSmallButton(null, null, this.loc.PropertyEnum.StiBorderSidesRight, "BorderRight.png"), "2px 4px 2px 4px"],
        ["BorderBottom", this.StandartSmallButton(null, null, this.loc.PropertyEnum.StiBorderSidesBottom, "BorderBottom.png"), "2px 4px 4px 4px"]
    ]

    for (var i = 0; i < controlProps.length; i++) {
        if (controlProps[i][0] == "separator") {
            menu.innerContent.appendChild(this.VerticalMenuSeparator());
            continue;
        }

        var control = controlProps[i][1];
        control.propertyName = controlProps[i][0];
        control.style.margin = controlProps[i][2];
        controls[control.propertyName] = control;
        menu.innerContent.appendChild(control);

        control.action = function () {
            var key = "";
            var borders = ["BorderLeft", "BorderRight", "BorderTop", "BorderBottom"];
            if (this.propertyName == "BorderAll" || this.propertyName == "BorderNone") {
                if (!this.isSelected) {
                    controls.BorderAll.setSelected(this.propertyName == "BorderAll");
                    controls.BorderNone.setSelected(this.propertyName == "BorderNone");
                    for (var i = 0; i < borders.length; i++) {
                        controls[borders[i]].setSelected(this.propertyName == "BorderAll");
                    }
                    key = this.propertyName == "BorderAll" ? "All" : "None";
                }
            }
            else {
                this.setSelected(!this.isSelected);
                var countBorders = 0;
                for (var i = 0; i < borders.length; i++) {
                    if (controls[borders[i]].isSelected) {
                        var side = borders[i].replace("Border", "");
                        if (key != "") key += ", ";
                        key += side;
                        countBorders++;
                    }
                }
                controls.BorderAll.setSelected(countBorders == 4);
                controls.BorderNone.setSelected(countBorders == 0);
                if (countBorders == 4) key = "All";
                if (countBorders == 4) key = "None";
            }

            bControl.key = key;
            bControl.action();
        }
    }
        
    bControl.setKey = function (key) {
        this.key = key;
        controls.BorderAll.setSelected(key == "All");
        controls.BorderNone.setSelected(key == "None");
        controls.BorderLeft.setSelected(key == "All" || key.indexOf("Left") >= 0);
        controls.BorderRight.setSelected(key == "All" || key.indexOf("Right") >= 0);
        controls.BorderTop.setSelected(key == "All" || key.indexOf("Top") >= 0);
        controls.BorderBottom.setSelected(key == "All" || key.indexOf("Bottom") >= 0);
    }

    bControl.setEnabled = function (state) {
        button.setEnabled(state);
    }

    bControl.action = function () { }

    return bControl;
}