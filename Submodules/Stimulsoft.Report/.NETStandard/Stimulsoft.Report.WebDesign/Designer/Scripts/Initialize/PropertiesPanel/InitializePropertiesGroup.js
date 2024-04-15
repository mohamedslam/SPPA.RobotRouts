
StiMobileDesigner.prototype.PropertiesGroup = function (name, caption, width, nestingLevel) {
    var propertiesGroup = document.createElement("div");
    propertiesGroup.jsObject = this;
    if (name != null) this.options.propertiesGroups[name] = propertiesGroup;
    propertiesGroup.name = name;
    propertiesGroup.className = "stiDesignerPropertiesGroup";
    propertiesGroup.isOpened = false;
    if (width) propertiesGroup.style.width = width + "px";
    propertiesGroup.nestingLevel = nestingLevel != null ? nestingLevel : 0;
    propertiesGroup.style.margin = nestingLevel > 0 ? "5px 0 0 5px" : "5px 0 0 0";

    //Header
    propertiesGroup.header = document.createElement("div");
    propertiesGroup.appendChild(propertiesGroup.header);
    propertiesGroup.headerButton = this.SmallButton(name != null ? name + "HeaderButton" : null, null, caption, "Arrows.ArrowRight.png", null, null, "stiDesignerPropertiesGroupHeaderButton");
    propertiesGroup.header.appendChild(propertiesGroup.headerButton);
    propertiesGroup.headerButton.propertiesGroup = propertiesGroup;
    propertiesGroup.headerButton.action = function () {
        this.propertiesGroup.changeOpenedState(!this.propertiesGroup.isOpened);
        this.propertiesGroup.openedByUser = true;
    }

    propertiesGroup.changeOpenedState = function (state) {
        this.container.style.display = state ? "" : "none";
        this.isOpened = state;
        this.headerButton.setSelected(state);
        StiMobileDesigner.setImageSource(this.headerButton.image, this.jsObject.options, state ? "Arrows.ArrowDown.png" : "Arrows.ArrowRight.png");
    }

    propertiesGroup.container = document.createElement("div");
    propertiesGroup.appendChild(propertiesGroup.container);
    propertiesGroup.container.className = "stiDesignerPropertiesGroupContainer";
    propertiesGroup.container.style.display = "none";

    return propertiesGroup;
}
