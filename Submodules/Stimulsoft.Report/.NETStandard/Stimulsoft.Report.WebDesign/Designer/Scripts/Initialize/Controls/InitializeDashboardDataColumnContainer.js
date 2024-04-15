
StiMobileDesigner.prototype.DashboardDataColumnContainer = function (form, name, headerText, width, showItemImage, contextMenu, allowManuallyData) {
    var container = this.DataColumnContainer(name, headerText, width, showItemImage, allowManuallyData);
    var innerContainer = container.innerContainer;
    var jsObject = this;

    innerContainer.oncontextmenu = function (event) {
        return false;
    }

    innerContainer.oldonmouseup = innerContainer.onmouseup;

    innerContainer.onmouseup = function (event) {
        if (event.button == 2) {
            if (container.item) container.item.action();
            event.stopPropagation();
            if (contextMenu) {
                var point = jsObject.FindMousePosOnMainPanel(event);
                contextMenu.currentContainer = container;
                contextMenu.show(point.xPixels + 3, point.yPixels + 3, "Down", "Right");
            }
            return false;
        }
        else if (innerContainer.canInsert()) {
            var originalItem = jsObject.options.itemInDrag.originalItem;
            var itemObject = jsObject.CopyObject(originalItem.itemObject);
            if (originalItem.closeButton && originalItem.closeButton.clicked) return;

            if (itemObject.typeItem == "Meter") {
                var fromContainerName = jsObject.UpperFirstChar(originalItem.container.name);
                var toContainerName = jsObject.UpperFirstChar(container.name);
                var commandName = jsObject.options.CTRL_pressed ? "MoveAndDuplicateMeter" : "MoveMeter";

                if (toContainerName != fromContainerName) {
                    form.sendCommand(
                        {
                            command: commandName,
                            toContainerName: toContainerName,
                            fromContainerName: fromContainerName
                        },
                        function (answer) {
                            form.updateControls(answer.elementProperties);

                            if (answer.elementProperties.svgContent && form.updateSvgContent) {
                                form.updateSvgContent(answer.elementProperties.svgContent);
                            } else if (answer.elementProperties.iframeContent && form.updateIframeContent) {
                                form.updateIframeContent(answer.elementProperties.iframeContent);
                            }
                            if (form.updateElementProperties) {
                                form.updateElementProperties(answer.elementProperties);
                            }
                            if (container.item) {
                                container.item.action();
                            }
                        });
                }
            }
            else {
                innerContainer.oldonmouseup(event);
            }
        }
    }

    container.oncontextmenu = function (event) {
        return false;
    }

    return container;
}