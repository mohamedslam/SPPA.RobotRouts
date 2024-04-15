
StiMobileDesigner.prototype.DashboardDataColumnsBlock = function (form, contextMenu, containerName, headerText, multiItems, showItemImage, allowManuallyData) {
    var table = this.CreateHTMLTable();
    var jsObject = this;

    if (headerText) {
        var header = table.addTextCell(headerText);
        table.header = header;
        header.className = "stiDesignerTextContainer";
        header.style.padding = "12px 0 12px 0";
    }

    var container = this.DataContainer(null, null, showItemImage, null, allowManuallyData);
    container.multiItems = multiItems;
    container.name = containerName;
    table.container = container;
    container.style.minHeight = "31px";
    container.style.maxHeight = "100px";
    table.addCellInNextRow(container);
    jsObject.AddDroppedContainerToCollection(container);

    container.updateMeters = function (meters, selectedIndex) {
        var oldScrollTop = this.scrollTop;
        this.style.height = this.offsetHeight + "px";

        this.clear();
        for (var i = 0; i < meters.length; i++) {
            var imageName = "Meters." + meters[i].typeIcon + ".png";
            if (meters[i].seriesType) {
                imageName = "ChartSeries.Light." + meters[i].seriesType + ".png";

                if (!StiMobileDesigner.checkImageSource(jsObject.options, imageName)) {
                    imageName = "ChartSeries.Light.ClusteredColumn.png";
                }
            }
            this.addItem(meters[i].currentFunction ? meters[i].label + " (" + meters[i].currentFunction + ")" : meters[i].label, imageName, meters[i]);
        }
        if (selectedIndex != null && selectedIndex < meters.length && selectedIndex >= 0) {
            this.childNodes[selectedIndex].select();
        }
        this.scrollTop = oldScrollTop;
        this.style.height = "auto";
        this.style.paddingBottom = (multiItems && this.getCountItems() > 0) ? "30px" : "0px";
    }

    container.onmouseup = function (event) {
        if (event.button == 2) {
            event.stopPropagation();
            if (contextMenu) {
                var point = this.jsObject.FindMousePosOnMainPanel(event);
                contextMenu.currentContainer = this;
                contextMenu.container = this;
                contextMenu.show(point.xPixels + 3, point.yPixels + 3, "Down", "Right");
            }
        }
        else if (this.jsObject.options.itemInDrag) {
            var itemObject = this.jsObject.CopyObject(this.jsObject.options.itemInDrag.originalItem.itemObject);
            if (!itemObject) return;
            var typeItem = itemObject.typeItem;

            if (typeItem == "Meter") {
                var toIndex = this.getOverItemIndex();
                var fromContainerName = this.jsObject.options.itemInDrag.originalItem.container.name;
                var fromContainer = form.controls[fromContainerName + "Block"].container;
                var fromIndex = fromContainer.getItemIndex(this.jsObject.options.itemInDrag.originalItem);
                var commandName = this.jsObject.options.CTRL_pressed ? "MoveAndDuplicateMeter" : "MoveMeter";

                if (containerName != fromContainerName || (toIndex != null && fromIndex != null && fromIndex != toIndex)) {
                    var params = {
                        command: commandName,
                        toContainerName: containerName,
                        fromContainerName: fromContainerName,
                        toIndex: toIndex,
                        fromIndex: fromIndex
                    };

                    if (form.oldSeriesType) {
                        params.oldSeriesType = form.oldSeriesType; //for chart element
                    }

                    form.sendCommand(params,
                        function (answer) {
                            if (containerName != fromContainerName) {
                                var fromContainer = form.controls[fromContainerName + "Block"].container;
                                fromContainer.updateMeters(answer.elementProperties.meters[fromContainerName]);
                            }
                            var selectedIndex = toIndex != null ? toIndex : container.getCountItems();
                            container.updateMeters(answer.elementProperties.meters[containerName], selectedIndex);
                            form.updateSvgContent(answer.elementProperties.svgContent);
                            if (form.updateElementProperties) form.updateElementProperties(answer.elementProperties);
                            form.checkStartMode();
                        }
                    );
                }
            }
            else if (typeItem == "Column" || typeItem == "DataSource" || typeItem == "BusinessObject" || typeItem == "Variable") {
                var draggedItem = {
                    itemObject: itemObject
                };

                if (typeItem == "Column") {
                    var columnParent = this.jsObject.options.dictionaryTree.getCurrentColumnParent();
                    if (columnParent) {
                        draggedItem.currentParentType = columnParent.type;
                        draggedItem.currentParentName = (columnParent.type == "BusinessObject") ? this.jsObject.options.itemInDrag.originalItem.getBusinessObjectFullName() : columnParent.name;
                    }
                }
                else if (typeItem == "DataSource" || typeItem == "BusinessObject") {
                    draggedItem.currentParentType = typeItem;
                    draggedItem.currentParentName = itemObject.name;
                }

                var params = {
                    command: "InsertMeters",
                    containerName: containerName,
                    draggedItem: draggedItem
                }

                if (typeItem == "Column" || typeItem == "Variable") {
                    params.insertIndex = container.getOverItemIndex();
                }

                if (form.oldSeriesType) {
                    params.oldSeriesType = form.oldSeriesType; //for chart element
                }

                form.sendCommand(params,
                    function (answer) {
                        var insertIndex = params.insertIndex != null ? params.insertIndex : answer.elementProperties.meters[containerName].length - 1;
                        container.updateMeters(answer.elementProperties.meters[containerName], insertIndex);
                        form.updateSvgContent(answer.elementProperties.svgContent);
                        if (form.updateElementProperties) form.updateElementProperties(answer.elementProperties);
                        form.checkStartMode();
                        form.correctTopPosition();
                    }
                );
            }
        }

        return false;
    }

    container.onRemove = function (itemIndex) {
        form.sendCommand({ command: "RemoveMeter", containerName: containerName, itemIndex: itemIndex },
            function (answer) {
                container.updateMeters(answer.elementProperties.meters[containerName], container.getSelectedItemIndex());
                form.updateSvgContent(answer.elementProperties.svgContent);
                if (form.updateElementProperties) form.updateElementProperties(answer.elementProperties);
                form.checkStartMode();
            }
        );
    }

    container.oncontextmenu = function (event) {
        return false;
    }

    return table;
}