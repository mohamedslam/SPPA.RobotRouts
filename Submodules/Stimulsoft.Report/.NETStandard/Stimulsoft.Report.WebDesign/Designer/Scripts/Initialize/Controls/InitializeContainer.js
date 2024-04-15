
StiMobileDesigner.prototype.Container = function (name, width, height, numerable) {
    var container = document.createElement("div");
    container.jsObject = this;
    container.className = "stiDesignerItemsContainer";
    if (width) container.style.width = width + "px";
    if (height) container.style.height = height + "px";
    container.items = [];
    container.selectedItem = null;
    container.selectedItems = [];
    container.name = name;
    container.multiSelection = false;
    container.numerable = numerable;

    container.onChange = function () { };
    container.onAction = function () { };

    container.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        this.items = [];
        this.selectedItems = [];
        this.selectedItem = null;
        this.onChange();
    }

    container.getSelectedIndex = function (item) {
        var index = -1;
        for (var i = 0; i < container.items.length; i++) {
            if (container.items[i] == (item || container.selectedItem)) return i;
        }
        return index;
    }

    container.addItemAndNotAction = function (name, itemObject, caption, image) {
        if (!name) name = this.jsObject.generateKey();
        var item = this.jsObject.StandartSmallButton(null, null, caption || name, image);
        item.name = name;
        item.style.margin = "2px";
        item.itemObject = itemObject;

        if (this.numerable) {
            item.nCell = item.innerTable.addCell();
            item.nCell.style.width = "100%";
            item.nCell.style.textAlign = "right";
            item.nCell.style.paddingRight = "3px";
        }

        this.items.push(item);
        this.appendChild(item);
        item.container = this;

        item.action = function () {
            this.selected();
            this.container.onChange();
            this.container.onAction();
        }

        item.selected = function () {
            if (this.container.multiSelection && this.container.selectedItem != null && this.container.jsObject.options.SHIFT_pressed) {
                var from = this.container.getSelectedIndex();
                var to = this.container.getSelectedIndex(this);
                this.container.selectedItems = [];
                for (var i = 0; i < this.container.items.length; i++) {
                    var isSelected = i >= Math.min(from, to) && i <= Math.max(from, to);
                    this.container.items[i].setSelected(isSelected);
                    if (isSelected) this.container.selectedItems.push(this.container.items[i]);
                }
            } else if (this.container.multiSelection && this.container.jsObject.options.CTRL_pressed) {
                this.setSelected(!this.isSelected);
                if (!this.isSelected) {
                    this.container.selectedItem = null;
                    if (this.container.selectedItems.indexOf(this) >= 0) {
                        this.container.selectedItems.splice(this.container.selectedItems.indexOf(this), 1);
                    }
                } else {
                    this.container.selectedItem = this;
                    this.container.selectedItems.push(this);
                }
            } else {
                for (var i = 0; i < this.container.items.length; i++) {
                    this.container.items[i].setSelected(false);
                }
                this.setSelected(true);
                this.container.selectedItem = this;
                this.container.selectedItems = [this];
            }
            this.container.updateNumaration();
        };

        item.remove = function () {
            container.removeItem(this.name);
        };
        container.updateNumaration();
        return item;
    }

    container.addItem = function (name, itemObject) {
        var item = container.addItemAndNotAction(name, itemObject);
        item.action();

        return item;
    }

    container.removeItem = function (name) {
        var item = null;
        var i;
        for (i in this.items)
            if (this.items[i].name == name) {
                item = this.items[i];
                break;
            }
        if (item) {
            var prevItem = item.previousSibling;
            var nextItem = item.nextSibling;
            this.removeChild(item);
            this.items.splice(i, 1);
            this.selectedItem = null;
            if (this.items.length > 0) {
                if (nextItem) {
                    nextItem.setSelected(true);
                    this.selectedItem = nextItem;
                }
                else if (prevItem) {
                    prevItem.setSelected(true);
                    this.selectedItem = prevItem;
                }
                else {
                    this.items[0].setSelected(true);
                    this.selectedItem = this.items[0];
                }
            }
            this.onChange();
        }
        container.updateNumaration();
    }

    container.isContained = function (name) {
        for (var i = 0; i < this.items.length; i++) {
            if (this.items[i].name == name) return true;
        }
        return false;
    }

    container.updateNumaration = function () {
        if (container.numerable) {
            for (var i in this.items) {
                this.items[i].nCell.innerHTML = "";
            }
            for (var i = 0; i < this.selectedItems.length; i++) {
                this.selectedItems[i].nCell.innerHTML = (i + 1).toString();
            }
        }
    }

    return container;
}

StiMobileDesigner.prototype.ContainerWithBigItems = function (name, width, height, itemMaxWidth) {
    var container = this.Container(name, width, height);

    container.addItemAndNotAction = function (name, caption, imageName, itemObject) {
        var item = this.jsObject.BigButton(null, null, caption, imageName, null, null, "stiDesignerStandartBigButton", null, itemMaxWidth);
        if (item.cellImage) item.cellImage.style.padding = "5px";
        if (item.caption) item.caption.style.padding = "0 5px 5px 5px";

        item.name = name;
        item.itemObject = itemObject;
        item.container = this;
        item.style.margin = "0 6px 2px 6px";
        item.style.minHeight = "0";

        this.items.push(item);
        this.appendChild(item);

        item.action = function () {
            item.selected();
            this.container.onChange();
            this.container.onAction();
        }

        item.selected = function () {
            for (var i = 0; i < this.container.items.length; i++) {
                this.container.items[i].setSelected(false);
            }
            this.setSelected(true);
            this.container.selectedItem = this;
        };

        item.remove = function () {
            container.removeItem(this.name);
        };

        item.getIndex = function () {
            for (var i = 0; i < container.childNodes.length; i++)
                if (container.childNodes[i] == this) return i;
        };

        return item;
    }

    container.addItem = function (name, caption, imageName, itemObject) {
        var item = container.addItemAndNotAction(name, caption, imageName, itemObject);
        item.action();

        return item;
    }

    return container;
}

StiMobileDesigner.prototype.EasyContainer = function (width, height) {
    var container = document.createElement("div");
    container.jsObject = this;
    if (width) container.style.width = width + "px";
    if (height) container.style.height = height + "px";
    container.style.overflow = "auto";
    container.selectedItem = null;

    container.onAction = function () { };
    container.onSelected = function (item) { };
    container.onPreChangeSelection = function () { };

    container.clear = function () {
        while (this.childNodes[0]) this.removeChild(this.childNodes[0]);
        this.selectedItem = null;
        this.onAction();
    }

    container.addItem = function (name, itemObject, caption, image, notAction) {
        if (!name) name = this.jsObject.generateKey();
        var item = this.jsObject.StandartSmallButton(null, null, caption || name, image);
        item.itemObject = itemObject;
        item.name = name;
        item.style.margin = "1px";
        container.appendChild(item);
        if (!notAction) container.onAction();

        item.action = function () {
            var isEmptyAction = container.selectedItem == this;

            if (container.selectedItem) {
                container.onPreChangeSelection();
                container.selectedItem.setSelected(false);
            }

            this.setSelected(true);
            container.selectedItem = this;
            container.onAction(isEmptyAction);
            container.onSelected(this);
        }

        item.select = function () {
            if (container.selectedItem) container.selectedItem.setSelected(false);
            this.setSelected(true);
            container.selectedItem = this;
        }

        item.remove = function () {
            container.removeChild(this);
            if (this == container.selectedItem) {
                container.selectedItem = null;
                var count = container.getCountItems();
                if (count > 0) {
                    container.selectedItem = container.childNodes[count - 1];
                    container.selectedItem.setSelected(true);
                }
            }
            container.onAction();
        };

        item.getIndex = function () {
            for (var i = 0; i < container.childNodes.length; i++)
                if (container.childNodes[i] == this) return i;
        };

        item.move = function (direction) {
            var index = this.getIndex();
            container.removeChild(this);
            var count = container.getCountItems();
            var newIndex = direction == "Up" ? index - 1 : index + 1;
            if (direction == "Up" && newIndex == -1) newIndex = 0;
            if (direction == "Down" && newIndex >= count) {
                container.appendChild(this);
                container.onAction();
                return;
            }
            container.insertBefore(this, container.childNodes[newIndex]);
            container.onAction();
        }

        return item;
    }

    container.removeItem = function (name) {
        for (var i = 0; i < container.childNodes.length; i++) {
            var item = container.childNodes[i];
            if (item.name == name) {
                if (item == container.selectedItem) container.selectedItem = null;
                container.removeChild(item);
                container.onAction();
                break;
            }
        }
    }

    container.isContained = function (name) {
        for (var i = 0; i < container.childNodes.length; i++) {
            if (container.childNodes[i].name == name) return true;
        }
        return false;
    }

    container.getCountItems = function () {
        return container.childNodes.length;
    }

    container.getItemByName = function (name) {
        for (var i = 0; i < container.childNodes.length; i++) {
            if (container.childNodes[i].name == name) return container.childNodes[i];
        }
        return null;
    }

    return container;
}