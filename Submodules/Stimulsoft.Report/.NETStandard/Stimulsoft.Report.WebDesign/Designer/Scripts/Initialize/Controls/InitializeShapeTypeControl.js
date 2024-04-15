
StiMobileDesigner.prototype.ShapeTypeControl = function (name, width) {
    var control = this.CreateHTMLTable();
    control.isEnabled = true;

    var button = this.SmallButton(null, null, "Shape ", "Shapes.StiRectangleShapeType.png", null, "Down", "stiDesignerFormButton", true);
    if (width) button.style.width = width + "px";
    button.innerTable.style.width = "100%";
    button.imageCell.style.width = "1px";
    button.arrowCell.style.width = "1px";
    control.addCell(button);

    var capBlock = document.createElement("div");
    capBlock.setAttribute("style", "text-overflow: ellipsis; overflow: hidden; white-space: nowrap;");
    if (width) capBlock.style.width = (width - (this.options.isTouchDevice ? 50 : 45)) + "px";
    button.caption.innerHTML = "";
    button.caption.appendChild(capBlock);

    var menu = this.ShapesMenu(name + "Menu", button, true);

    button.action = function () {
        menu.changeVisibleState(!menu.visible);
    }

    control.action = function () { }

    control.setKey = function (key) {
        this.key = key;

        for (var itemName in menu.items) {
            if (itemName == key) {
                StiMobileDesigner.setImageSource(button.image, this.jsObject.options, "Shapes." + key + ".png");

                var captText = this.jsObject.loc.Shapes[key.replace("ShapeType", "").replace("Sti", "")];
                if (key.indexOf("StiArrowShapeType") == 0) captText = this.jsObject.loc.Shapes.Arrow;

                capBlock.innerText = captText;
                control.setAttribute("title", captText);
                break;
            }
        }
    }

    control.setEnabled = function (state) {
        control.isEnabled = state;
        button.setEnabled(state);
    }

    menu.action = function (menuItem) {
        menu.changeVisibleState(false);
        control.setKey(menuItem.key);
        control.action();
    }

    return control;
}