
StiMobileDesigner.prototype.InitializeMapCategoriesForm_ = function () {
    var jsObject = this;
    var form = this.BaseForm("mapCategoriesForm", this.loc.Components.StiMap, 3);
    form.buttonOk.style.display = "none";

    var mainTable = this.CreateHTMLTable();
    mainTable.style.width = "100%";
    form.container.appendChild(mainTable);
    form.container.style.padding = "6px 0 6px 0";

    var categoryControl = this.DropDownList(null, 365, null, this.GetMapsCategoriesItems(), true);
    categoryControl.style.display = "inline-block";
    form.addControlRow(mainTable, this.loc.PropertyMain.Category, "category", categoryControl, "6px 12px 6px 0").style.textAlign = "right";

    var nameControl = this.TextBox(null, 365);
    nameControl.setAttribute("placeholder", this.loc.Editor.TypeToSearch);
    form.addControlRow(mainTable, this.loc.PropertyMain.Name, "name", nameControl, "6px 12px 6px 0").style.textAlign = "right";

    var mapsContainer = this.EasyContainer(505, 450);
    mapsContainer.style.margin = "6px 12px 6px 12px";
    mapsContainer.style.padding = "6px 0 0 0";
    form.container.appendChild(mapsContainer);

    mapsContainer.fill = function () {
        this.clear();
        this.scrollTop = 0;

        if (categoryControl.key == "All") {
            var popularMapsHeader = jsObject.MapsBlockHeader(jsObject.IsRusCulture(jsObject.options.cultureName) ? "Популярные карты" : "Popular maps");
            this.appendChild(popularMapsHeader);

            var popularItems = [];
            popularItems.push(jsObject.Item("World", "World", "Maps.Big.World.png", "World"));
            popularItems.push(jsObject.Item("USA", "USA", "Maps.Big.USA.png", "USA"));
            popularItems.push(jsObject.Item("Europe", "Europe", "Maps.Big.Europe.png", "Europe"));
            popularItems.push(jsObject.Item("Asia", "Asia", "Maps.Big.Asia.png", "Asia"));
            popularItems.push(jsObject.Item("China", "China", "Maps.Big.China.png", "China"));

            for (var i = 0; i < popularItems.length; i++) {
                var popularItem = jsObject.MapsBigButton(popularItems[i].key, popularItems[i].caption, popularItems[i].imageName);
                popularItem.isPopularItem = true;
                this.appendChild(popularItem);

                popularItem.action = function () {
                    form.action(this);
                }
            }

            var azHeader = jsObject.MapsBlockHeader("A-Z");
            this.appendChild(azHeader);
        }

        var items = [];
        if (categoryControl.key == "All" || categoryControl.key == "Europe")
            items = items.concat(jsObject.GetEuropeMapsItems());
        if (categoryControl.key == "All" || categoryControl.key == "NorthAmerica")
            items = items.concat(jsObject.GetNorthAmericaMapsItems());
        if (categoryControl.key == "All" || categoryControl.key == "SouthAmerica")
            items = items.concat(jsObject.GetSouthAmericaMapsItems());
        if (categoryControl.key == "All" || categoryControl.key == "Asia")
            items = items.concat(jsObject.GetAsiaMapsItems());
        if (categoryControl.key == "All" || categoryControl.key == "Oceania")
            items = items.concat(jsObject.GetOceaniaMapsItems());
        if (categoryControl.key == "All" || categoryControl.key == "Africa")
            items = items.concat(jsObject.GetAfricaMapsItems());

        if (categoryControl.key == "All" || categoryControl.key == "Custom") {
            var customMaps = jsObject.GetCustomMapResources();
            if (customMaps.length > 0) {
                for (var i = 0; i < customMaps.length; i++) {
                    items.push(jsObject.Item("customMap" + i, customMaps[i].name, "CustomMap.png", customMaps[i].name, customMaps[i].icon));
                }
            }
        }

        items.sort(jsObject.SortByName);

        for (var i = 0; i < items.length; i++) {
            var item = jsObject.MapsBigButton(items[i].key, items[i].caption, "Maps.Big." + items[i].imageName, items[i].styleProperties);
            this.appendChild(item);

            item.action = function () {
                form.action(this);
            }
        }
    }

    nameControl.onchange = function () {
        var text = this.value.toLowerCase();
        for (var i = 0; i < mapsContainer.childNodes.length; i++) {
            var item = mapsContainer.childNodes[i];
            if (item.className == "stiDesignerFormBlockHeader" || item.isPopularItem) {
                item.style.display = !text ? (item.isPopularItem ? "inline-block" : "") : "none";
            }
            else if (item.caption) {
                item.style.display = !text || item.caption.innerHTML.toLowerCase().indexOf(text) >= 0 ? "inline-block" : "none";
            }
        }
    }

    categoryControl.action = function () {
        mapsContainer.fill();
        nameControl.value = "";
        nameControl.focus();
        jsObject.SetCookie("StimulsoftMobileDesignerMapsCategory", this.key);
    }

    form.cancelAction = function () {
        var regionMapForm = jsObject.options.forms.editRegionMapElementForm;
        var mapForm = jsObject.options.forms.editMapForm;
        if (regionMapForm && regionMapForm.visible) regionMapForm.changeVisibleState(false);
        if (mapForm && mapForm.visible) mapForm.changeVisibleState(false);
    }

    form.show = function (ownerPanel, ownerControl, componentType) {
        this.changeVisibleState(true);
        this.ownerPanel = ownerPanel;
        this.ownerControl = ownerControl;
        this.componentType = componentType;
        nameControl.value = "";
        nameControl.focus();
        categoryControl.addItems(jsObject.GetMapsCategoriesItems());
        var defaultCategory = jsObject.GetCookie("StimulsoftMobileDesignerMapsCategory");
        categoryControl.setKey(defaultCategory || "All");
        mapsContainer.fill();
    }

    form.onhide = function () {
        nameControl.onblur();
        if (this.ownerPanel) this.ownerPanel.resetChoose();
    }

    form.action = function (item) {
        this.changeVisibleState(false);

        if (jsObject.options.jsMode && !jsObject.options.mapsResourcesLoaded) {
            var errorMessageForm = jsObject.options.forms.errorMessageForm || jsObject.InitializeErrorMessageForm();
            errorMessageForm.show("You cannot create a map. Please include js library 'stimulsoft.report.maps.js' !", "Warning");
            return;
        }

        if (this.ownerControl) {
            this.ownerControl.setKey(item.key);
            this.ownerControl.action();
        }
        else if (this.ownerPanel) {
            item.name = "Infographic;" + (this.componentType || "StiMap") + ";" + item.key;
            jsObject.options.drawComponent = true;
            jsObject.options.paintPanel.setCopyStyleMode(false);
            jsObject.options.paintPanel.changeCursorType(true);
            this.ownerPanel.selectedComponent = item;

            if (this.ownerPanel == jsObject.options.insertPanel)
                jsObject.options.buttons.insertMaps.setSelected(true);
            else
                jsObject.options.toolbox.buttons.maps.setSelected(true);
        }
    }

    return form;
}

StiMobileDesigner.prototype.MapsBigButton = function (key, caption, imageName, cusomIcon) {
    var button = this.BigButton(null, null, caption, imageName, null, null, "stiDesignerStandartBigButton", true, null, { width: 64, height: 64 });
    button.key = key;
    button.style.border = "1px solid #c6c6c6";
    button.style.display = "inline-block";
    button.style.width = button.style.height = "108px";
    button.style.margin = "0 12px 12px 0";

    button.cellImage.style.padding = "5px 0 0 0";
    button.cellImage.style.lineHeight = "0";
    button.cellImage.style.height = "1px";

    button.caption.style.padding = "0 2px 0 2px";
    button.caption.style.maxWidth = "96px";

    if (cusomIcon) {
        button.image.src = cusomIcon;
    }

    return button;
}

StiMobileDesigner.prototype.MapsBlockHeader = function (caption) {
    var header = this.FormBlockHeader(caption);
    header.style.background = "transparent";
    if (header.caption) header.caption.style.padding = "6px";

    return header;
}