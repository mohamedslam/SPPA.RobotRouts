
StiMobileDesigner.prototype.InitializeDashboardElementSortMenu = function () {
    var jsObject = this;
    var menu = this.BaseContextMenu("dashboardElementSortMenu", "Down", []);

    menu.fillItems = function () {
        jsObject.SendCommandToDesignerServer("GetDbsElementSortItems", { elementName: this.component.properties.name }, function (answer) {
            if (answer.sortItems) {
                var sortItems = answer.sortItems;
                menu.sortDirection = "Ascending";
                menu.clear();

                if (sortItems && sortItems.length > 0) {
                    var sortChecked = false;
                    menu.sortDirection = sortItems[0].sortDirection;

                    for (var i = 0; i < sortItems.length; i++) {
                        menu.addItem("sortItem" + i, jsObject.loc.PropertyMain.SortBy + " " + sortItems[i].text, sortItems[i].key, sortItems[i].checked);
                        if (sortItems[i].checked) sortChecked = true;
                    }

                    menu.addItem("sortItemNone", jsObject.loc.PropertyEnum.StiSortDirectionNone, "sortNone", !sortChecked);

                    if (sortChecked) {
                        menu.innerContent.appendChild(jsObject.VerticalMenuSeparator(menu, "separator"));
                        menu.addItem("sortAsc", jsObject.loc.PropertyEnum.StiSortDirectionAsc, "sortAsc", menu.sortDirection == "Ascending");
                        menu.addItem("sortDesc", jsObject.loc.PropertyEnum.StiSortDirectionDesc, "sortDesc", menu.sortDirection == "Descending");
                    }
                }
            }
        });
    }

    menu.addItem = function (name, caption, key, checked) {
        var item = jsObject.SortFilterMenuItem(this, name, caption, null, key, false);
        item.setChecked(checked);
        menu.innerContent.appendChild(item);
        menu.items[name] = item;

        item.action = function () {
            menu.changeVisibleState(false);
            if (this.isChecked) return;
            var sorts = [];

            if (this.name.indexOf("sortItem") == 0) {
                for (var itemName in menu.items) {
                    if (itemName.indexOf("sortItem") == 0) {
                        menu.items[itemName].setChecked(false);
                    }
                }
                this.setChecked(true);

                if (this.key != "sortNone") {
                    sorts = [jsObject.DataSortObject(this.key, menu.sortDirection)];
                }
            }
            else if (this.key == "sortAsc" || this.key == "sortDesc") {
                var sortKey = null;
                for (var itemName in menu.items) {
                    if (itemName.indexOf("sortItem") == 0 && itemName.indexOf("sortItemNone") != 0 && menu.items[itemName].isChecked) {
                        sortKey = menu.items[itemName].key;
                        break;
                    }
                }
                menu.items.sortAsc.setChecked(this.key == "sortAsc");
                menu.items.sortDesc.setChecked(this.key == "sortDesc");
                if (sortKey) sorts = [jsObject.DataSortObject(sortKey, this.key == "sortAsc" ? "Ascending" : "Descending")];
            }

            menu.changeVisibleState(false);

            jsObject.SendCommandToDesignerServer("ApplySortsToDashboardElement", {
                elementName: menu.component.properties.name,
                sorts: sorts,
                zoom: jsObject.options.report.zoom.toString()
            },
                function (answer) {
                    if (answer.svgContent) {
                        menu.component.properties.svgContent = answer.svgContent;
                        menu.component.repaint();
                    }
                }
            );
        }

        return item;
    }

    menu.onhide = function () {
        menu.component.controls.sortButton.setSelected(false);
        menu.component.controls.sortButton.hide();
    }

    menu.show = function (component) {
        if (component) {
            this.innerContent.style.top = "0px";
            this.style.display = "";
            this.component = component;

            var browserHeight = (window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight) - jsObject.FindPosY(jsObject.options.mainPanel);
            var browserWidth = (window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth) - jsObject.FindPosX(jsObject.options.mainPanel);
            var compWidth = parseInt(component.getAttribute("width"));
            var compLeft = parseInt(component.getAttribute("left"));
            var compTop = parseInt(component.getAttribute("top"));
            var pagePositions = jsObject.FindPagePositions(jsObject.options.mainPanel);
            var zoom = jsObject.options.report.zoom;
            var topPos = pagePositions.posY + compTop + component.marginPx[1] + 25 + parseInt(6 * zoom);
            var leftPos = pagePositions.posX + compLeft + compWidth - component.marginPx[2] - 40 - parseInt(8 * zoom);

            if (topPos + this.innerContent.offsetHeight > browserHeight) {
                topPos = browserHeight - menu.innerContent.offsetHeight - 10;
            }

            if (leftPos + this.innerContent.offsetWidth > browserWidth) {
                leftPos = browserWidth - menu.innerContent.offsetWidth - 10;
            }

            this.fillItems();

            this.style.left = leftPos + "px";
            this.style.top = topPos + "px";
            jsObject.options.currentMenu = this;
        }
    }

    return menu;
}

StiMobileDesigner.prototype.DataSortObject = function (key, direction) {
    return {
        typeItem: "SortRule",
        key: key,
        direction: direction
    }
}