
StiMobileDesigner.prototype.MapsElementsMenu = function (menuName, parentButton, isToolboxMenu, dashboardElements) {
    var jsObject = this;
    var elementTypes = ["StiRegionMapElement", "StiOnlineMapElement"];
    var items = [];

    for (var i = 0; i < elementTypes.length; i++) {
        if (dashboardElements.indexOf(elementTypes[i]) >= 0 && jsObject.options.visibilityDashboardElements[elementTypes[i]]) {
            items.push(this.Item(elementTypes[i], this.loc.Components[elementTypes[i].replace("Element", "")],
                "Dashboards." + (isToolboxMenu ? "SmallComponents." : "BigComponents.") + elementTypes[i] + ".png", elementTypes[i]));
        }
    }

    if (items.length == 0 && parentButton)
        parentButton.style.display = "none";

    var menu = isToolboxMenu
        ? this.HorizontalMenu(menuName, parentButton, "Right", [])
        : this.VerticalMenu(menuName, parentButton, "Down", []);

    var mapsHeader = this.FormBlockHeader(this.loc.PropertyMain.Maps);
    menu.innerContent.appendChild(mapsHeader);

    for (var i = 0; i < items.length; i++) {
        var tooltip = ["<b>" + jsObject.loc.Components[items[i].name.replace("Element", "")] + "</b><br><br>" +
            "<table><tr><td style='vertical-align: top;'>" + jsObject.loc.HelpComponents[items[i].name] + "</td></tr></table>", jsObject.HelpLinks["insertcomponent"]];

        var button = jsObject.SmallButton(null, null, items[i].caption, items[i].imageName, tooltip, null, null, null, !isToolboxMenu ? { width: 32, height: 32 } : null);
        menu.innerContent.appendChild(button);

        if (button.imageCell) button.imageCell.style.padding = "0 10px 0 5px";
        if (button.caption) button.caption.style.padding = "0 10px 0 0";
        button.style.height = isToolboxMenu ? "24px" : "42px";
        button.isDashboardElement = true;
        button.name = items[i].name;
        button.menu = menu;

        this.AddDragEventsToComponentButton(button);

        button.action = function () {
            menu.changeVisibleState(false);
            var panel = isToolboxMenu ? jsObject.options.toolbox : jsObject.options.insertPanel;
            panel.resetChoose();
            panel.setChoose(this);

            if (this.name == "StiRegionMapElement") {
                jsObject.InitializeMapCategoriesForm(function (form) {
                    form.show(panel, null, "StiRegionMapElement");
                });
            }
        }

        //override
        button.onmouseenter = function () {
            if (!this.isEnabled || (this["haveMenu"] && this.isSelected) || jsObject.options.isTouchClick) return;
            this.className = this.overClass;
            this.isOver = true;
            if (jsObject.options.showTooltips && this.toolTip && typeof (this.toolTip) == "object") {
                jsObject.options.toolTip.showWithDelay(
                    this.toolTip[0],
                    this.toolTip[1],
                    jsObject.FindPosX(this, "stiDesignerMainPanel") + this.offsetWidth,
                    jsObject.FindPosY(this, "stiDesignerMainPanel") + this.offsetHeight
                );
            }
        }
    }

    return menu;
}