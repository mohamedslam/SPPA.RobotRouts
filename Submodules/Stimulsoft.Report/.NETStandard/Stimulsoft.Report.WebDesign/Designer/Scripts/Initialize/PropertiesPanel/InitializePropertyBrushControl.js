
StiMobileDesigner.prototype.PropertyBrushControl = function (name, toolTip, width) {
    var brushControl = this.CreateHTMLTable();
    var jsObject = brushControl.jsObject = this;
    brushControl.key = null;
    brushControl.isEnabled = true;
    brushControl.isBrushControl = true;

    if (name != null) {
        brushControl.name = name;
        this.options.controls[name] = brushControl;
    }
    else {
        brushControl.name = this.generateKey();
    }

    brushControl.button = this.SmallButton(name ? name + "Button" : null, null, this.loc.Report.StiSolidBrush, "BrushSolid.png", toolTip, "Down", "stiDesignerPropertiesBrushControlButton");
    brushControl.button.style.width = (width + 4) + "px";
    brushControl.button.caption.style.width = "100%";
    brushControl.addCell(brushControl.button);
    brushControl.menu = this.BrushMenu(name ? name + "BrushMenu" : null, brushControl.button);
    brushControl.menu.brushControl = brushControl;
    brushControl.button.brushMenu = brushControl.menu;

    //Override image
    var colorBar = this.BrushHeaderSvgImage();
    var imageCell = brushControl.button.image.parentElement;
    imageCell.removeChild(brushControl.button.image);
    imageCell.appendChild(colorBar);
    brushControl.button.image = colorBar;

    brushControl.setEnabled = function (state) {
        if (this.button.image) this.button.image.style.opacity = state ? "1" : "0.3";
        if (this.button.arrow) this.button.arrow.style.opacity = state ? "1" : "0.3";
        this.isEnabled = state;
        this.button.isEnabled = state;
        this.button.className = state ? this.button.defaultClass : this.button.disabledClass;
    }

    brushControl.button.action = function () {
        this.brushMenu.changeVisibleState(!this.brushMenu.visible);
    }

    brushControl.action = function () { }

    brushControl.setKey = function (key) {
        var notLocalizeValues = this.ownerIsProperty && jsObject.options.propertiesPanel && !jsObject.options.propertiesPanel.localizePropertyGrid;
        this.key = key;
        this.button.key = key;
        this.button.image.style.opacity = 1;

        if (key == "StiEmptyValue" || key == "none") {
            this.button.image.setBrush(["0"]);
            this.button.caption.innerHTML = "";
            return;
        }

        var brushTypes = ["Empty", "Solid", "Hatch", "Gradient", "Glare", "Glass"];
        var brushArray = key.split("!");
        var brushType = brushTypes[brushArray[0]];

        this.button.image.setBrush(brushArray);
        this.button.caption.innerHTML = notLocalizeValues ? brushType : jsObject.loc.Report["Sti" + brushType + "Brush"];
    }

    brushControl.menu.action = function () {
        this.brushControl.setKey(this.parentButton.key);
        this.brushControl.action();
    }

    return brushControl;
}