
StiMobileDesigner.prototype.InitializeSettingsStylesMenu = function (styleDesignerForm) {
    var menu = this.VerticalMenu("settingsStylesMenu", styleDesignerForm.toolBar.settings, "Down")
    menu.innerContent.style.maxHeight = "500px";
    menu.controls = {};

    styleDesignerForm.toolBar.settings.action = function () {
        menu.changeVisibleState(!menu.visible);
    }

    var itemProps = [];
    itemProps.push(["header", this.loc.PropertyMain.Filter]);
    itemProps.push(["filter", "StiStyle", this.loc.Components.StiComponent, "Styles.StiComponentStyle.png"]);
    itemProps.push(["filter", "StiChartStyle", this.loc.Components.StiChart, "SmallComponents.StiChart.png"]);
    itemProps.push(["filter", "StiGaugeStyle", this.loc.Components.StiGauge, "SmallComponents.StiGauge.png"]);
    itemProps.push(["filter", "StiMapStyle", this.loc.Components.StiMap, "SmallComponents.StiMap.png"]);
    itemProps.push(["filter", "StiCrossTabStyle", this.loc.Components.StiCrossTab, "SmallComponents.StiCrossTab.png"]);
    itemProps.push(["filter", "StiTableStyle", this.loc.Components.StiTable, "SmallComponents.StiTable.png"]);

    if (this.options.dashboardAssemblyLoaded) {
        itemProps.push(["filter", "StiCardsStyle", this.loc.Components.StiCards, "Dashboards.SmallComponents.StiCardsElement.png"]);
        itemProps.push(["filter", "StiIndicatorStyle", this.loc.Components.StiIndicator, "Dashboards.SmallComponents.StiIndicatorElement.png"]);
        itemProps.push(["filter", "StiProgressStyle", this.loc.Components.StiProgress, "Dashboards.SmallComponents.StiProgressElement.png"]);
        itemProps.push(["filter", "StiDialogStyle", this.loc.PropertyCategory.ControlCategory, "Styles.StiDialogStyle.png"]);
    }

    itemProps.push(["header", this.loc.PropertyMain.Sort]);
    itemProps.push(["sort", "Ascending", this.loc.PropertyEnum.StiSortDirectionAsc, "SortAZ.png"]);
    itemProps.push(["sort", "Descending", this.loc.PropertyEnum.StiSortDirectionDesc, "SortZA.png"]);
    itemProps.push(["sort", "NoSorting", this.loc.FormBand.NoSort, "NoSort.png"]);

    for (var i = 0; i < itemProps.length; i++) {
        if (itemProps[i][0] == "header") {
            var header = this.StandartMenuHeader(itemProps[i][1]);
            menu.innerContent.appendChild(header);
        }
        else {
            var item = this.SettingsStylesMenuItem(itemProps[i][1], itemProps[i][0], itemProps[i][2], itemProps[i][3]);
            menu.controls[itemProps[i][1]] = item;
            menu.innerContent.appendChild(item);

            item.action = function () {
                if (this.groupName == "sort") {
                    styleDesignerForm.settings.sort = this.name;
                }
                else if (this.groupName == "filter") {
                    this.setSelected(!this.isSelected);
                    styleDesignerForm.settings.filter[this.name] = this.isSelected;
                }
                menu.updateItems();

                var selectedStyleObject = styleDesignerForm.stylesTree.selectedItem ? styleDesignerForm.stylesTree.selectedItem.itemObject : null;
                styleDesignerForm.stylesTree.updateItems(styleDesignerForm.stylesCollection, selectedStyleObject, true, true);
            }
        }
    }

    menu.updateItems = function () {
        var sort = styleDesignerForm.settings.sort;
        menu.controls.Ascending.setSelected(sort == "Ascending");
        menu.controls.Descending.setSelected(sort == "Descending");
        menu.controls.NoSorting.setSelected(sort == "NoSorting");
        var filter = styleDesignerForm.settings.filter;
        menu.controls.StiStyle.setSelected(filter.StiStyle);
        menu.controls.StiChartStyle.setSelected(filter.StiChartStyle);
        menu.controls.StiCrossTabStyle.setSelected(filter.StiCrossTabStyle);
        menu.controls.StiMapStyle.setSelected(filter.StiMapStyle);
        if (menu.controls.StiGaugeStyle) menu.controls.StiGaugeStyle.setSelected(filter.StiGaugeStyle);
        menu.controls.StiTableStyle.setSelected(filter.StiTableStyle);
        if (menu.controls.StiDialogStyle) menu.controls.StiDialogStyle.setSelected(filter.StiDialogStyle);
        if (menu.controls.StiIndicatorStyle) menu.controls.StiIndicatorStyle.setSelected(filter.StiIndicatorStyle);
        if (menu.controls.StiProgressStyle) menu.controls.StiProgressStyle.setSelected(filter.StiProgressStyle);
        if (menu.controls.StiCardsStyle) menu.controls.StiCardsStyle.setSelected(filter.StiCardsStyle);
    }

    menu.onshow = function () {
        menu.updateItems();
    }

    return menu;
}

StiMobileDesigner.prototype.SettingsStylesMenuItem = function (name, groupName, caption, imageName) {
    var item = this.StandartSmallButton(null, null, caption, imageName);
    item.name = name;
    item.groupName = groupName;
    item.style.margin = "4px";
    item.style.minWidth = "150px";
    item.style.height = "auto";

    if (item.image) {
        item.image.style.padding = "2px";
        item.image.style.border = "1px solid transparent";
        item.imageCell.style.textAlign = "center";
        item.imageCell.style.width = item.imageCell.style.height = "22px";
        item.imageCell.style.padding = "3px";
    }

    if (item.caption) {
        item.caption.style.padding = "0 5px 0 3px";
    }

    item.setSelected = function (state) {
        this.isSelected = state;
        this.image.style.border = "1px solid " + (this.isSelected ? "gray" : "transparent");
        if (this.name == "NoSorting") {
            this.image.style.display = this.isSelected ? "" : "none";
        }
    }

    item.onmouseenter = function () {
        if (!this.isEnabled || this.jsObject.options.isTouchClick) return;
        this.className = this.overClass;
        this.isOver = true;
    }

    item.onmouseleave = function () {
        this.isOver = false;
        if (!this.isEnabled) return;
        this.className = this.defaultClass;
    }

    return item;
}