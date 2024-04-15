
StiMobileDesigner.prototype.GroupPanel = function (caption) {
    var groupPanel = document.createElement("div");
    groupPanel.jsObject = this;

    var header = this.FormBlockHeader(caption);
    groupPanel.header = header;
    groupPanel.appendChild(header);

    var container = document.createElement("div");
    groupPanel.container = container;
    groupPanel.appendChild(container);

    groupPanel.clear = function () {
        while (container.childNodes[0]) container.removeChild(container.childNodes[0]);
    }

    return groupPanel;
}

StiMobileDesigner.prototype.InteractiveGroupPanel = function (caption) {
    var groupPanel = document.createElement("div");
    groupPanel.jsObject = this;
    groupPanel.isOpening = false;

    var header = this.SmallButton(null, null, caption, "Arrows.ArrowRight.png", null, null, "stiDesignerGroupPanelHeader", true);
    groupPanel.appendChild(header);

    var container = document.createElement("div");
    container.style.display = "none";
    groupPanel.appendChild(container);
    groupPanel.container = container;

    header.action = function () {
        groupPanel.setOpening(!groupPanel.isOpening);
    }

    groupPanel.setOpening = function (state) {
        this.isOpening = state;
        container.style.display = state ? "" : "none";
        StiMobileDesigner.setImageSource(header.image, this.jsObject.options, state ? "Arrows.ArrowDown.png" : "Arrows.ArrowRight.png");
    }

    return groupPanel;
}